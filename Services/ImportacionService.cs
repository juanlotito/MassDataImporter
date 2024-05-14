using importacionmasiva.api.net.Helpers;
using importacionmasiva.api.net.Models.Enum;
using importacionmasiva.api.net.Repositories.Interface;
using importacionmasiva.api.net.Services.Interface;
using importacionmasiva.api.net.Utils.Csv;
using importacionmasiva.api.net.Utils.Excel;
using importacionmasiva.api.net.Utils.Exceptions;
using importacionmasiva.api.net.Utils.Txt;
using System.Data;

namespace importacionmasiva.api.net.Services
{
    public class ImportacionService : IImportacionService
    {
        private readonly IImportacionRepository _importacionRepositories;
        private readonly IImportacionLogsService _importacionLogsService;
        private readonly IExcelUtils _excelUtils;
        private readonly ICsvUtils _csvUtils;
        private readonly ITxtUtils _txtUtils;
        private readonly TableValidator _tableValidator;

        public ImportacionService(
            IImportacionRepository importacionRepositories,
            IImportacionLogsService importacionLogsService,
            IExcelUtils excelUtils,
            ICsvUtils csvUtils,
            ITxtUtils txtUtils,
            TableValidator tableValidator)
        {
            _importacionRepositories = importacionRepositories;
            _importacionLogsService = importacionLogsService;
            _excelUtils = excelUtils;
            _csvUtils = csvUtils;
            _txtUtils = txtUtils;
            _tableValidator = tableValidator;
        }

        public async Task ImportFromExcel(IFormFile dataset, string registryName, string tableName, DeleteAction deleteAction = DeleteAction.None)
        {
            //Se pasa como callback la función correspondiente para leer el archivo
            await ImportData(dataset, registryName, tableName, deleteAction, "xlsx", (file) => _excelUtils.ReadExcelToDataTable(file));
        }

        public async Task ImportFromTxt(IFormFile dataset, string registryName, string tableName, DeleteAction deleteAction = DeleteAction.None)
        {
            //Se pasa como callback la función correspondiente para leer el archivo
            await ImportData(dataset, registryName, tableName, deleteAction, "txt", (file) => _txtUtils.ReadTxtToDataTable(file));
        }

        public async Task ImportFromCsv(IFormFile dataset, string registryName, string tableName, DeleteAction deleteAction = DeleteAction.None)
        {
            //Se pasa como callback la función correspondiente para leer el archivo
            await ImportData(dataset, registryName, tableName, deleteAction, "csv", (file) => _csvUtils.ReadCsvToDataTable(file));
        }

        private async Task ImportData(IFormFile dataset, string registryName, string tableName, DeleteAction deleteAction, string tipoArchivo, Func<IFormFile, DataTable> readFileToDataTable)
        {
            if (dataset == null)
                throw new CustomException(400, "No se ha seleccionado ningún archivo.");

            int logId = -1;

            try
            {
                logId = await _importacionLogsService.LogInicio(tableName, tipoArchivo, dataset.FileName, registryName, deleteAction.ToString(), await _importacionRepositories.TableExists(tableName, registryName));

                //Se ejecuta la función pasada a modo de callback
                DataTable dataTable = readFileToDataTable(dataset);

                if (dataTable.Rows.Count == 0)
                    throw new CustomException(400, "El archivo no contiene registros.");

                if (await _importacionRepositories.TableExists(tableName, registryName))
                {
                    await HandleExistingTable(tableName, registryName, deleteAction, dataTable);
                }
                else
                {
                    await _importacionRepositories.CreateTable(dataTable, tableName, registryName);
                    await _importacionRepositories.BulkInsert(dataTable, tableName, registryName);
                }

                await _importacionLogsService.LogFinalizacion(logId, false, "Exitoso", registryName, dataTable.Rows.Count);
            }
            catch (Exception ex)
            {
                if (logId != -1)
                {
                    await _importacionLogsService.LogFinalizacion(logId, true, "Anulado", registryName, 0, ex.Message);
                }

                throw new CustomException(500, ex.Message);
            }
        }

        private async Task HandleExistingTable(string tableName, string registryName, DeleteAction deleteAction, DataTable dataTable)
        {
            if (deleteAction == DeleteAction.Delete)
                await _importacionRepositories.DeleteRecords(tableName, registryName);
            else if (deleteAction == DeleteAction.Truncate)
                await _importacionRepositories.TruncateTable(tableName, registryName);

            var tableDefinition = await _importacionRepositories.GetTableDefinition(tableName, registryName);

            _tableValidator.ValidateTableDefinition(tableDefinition, dataTable);

            await _importacionRepositories.BulkInsert(dataTable, tableName, registryName);
        }

    }
}
