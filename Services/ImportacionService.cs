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
        private readonly IImportacionRepositories _importacionRepositories;
        private readonly IExcelUtils _excelUtils;
        private readonly ICsvUtils _csvUtils;
        private readonly ITxtUtils _txtUtils;
        private readonly TableValidator _tableValidator;

        public ImportacionService(
            IImportacionRepositories importacionRepositories,
            IExcelUtils excelUtils,
            ICsvUtils csvUtils,
            ITxtUtils txtUtils,
            TableValidator tableValidator)
        {
            _importacionRepositories = importacionRepositories;
            _excelUtils = excelUtils;
            _csvUtils = csvUtils;
            _txtUtils = txtUtils;
            _tableValidator = tableValidator;
        }

        public async Task ImportFromExcel(IFormFile dataset, string registryName, string tableName, DeleteAction deleteAction = DeleteAction.None)
        {
            DataTable dataTable = _excelUtils.ReadExcelToDataTable(dataset);
            await ImportData(dataTable, registryName, tableName, deleteAction);
        }

        public async Task ImportFromTxt(IFormFile dataset, string registryName, string tableName, DeleteAction deleteAction = DeleteAction.None)
        {
            DataTable dataTable = _txtUtils.ReadTxtToDataTable(dataset);
            await ImportData(dataTable, registryName, tableName, deleteAction);
        }

        public async Task ImportFromCsv(IFormFile dataset, string registryName, string tableName, DeleteAction deleteAction = DeleteAction.None)
        {
            DataTable dataTable = _csvUtils.ReadCsvToDataTable(dataset);
            await ImportData(dataTable, registryName, tableName, deleteAction);
        }

        private async Task ImportData(DataTable dataTable, string registryName, string tableName, DeleteAction deleteAction)
        {
            if (dataTable.Rows.Count == 0)
                throw new CustomException(400, "El archivo no contiene registros.");

            try
            {
                if (await _importacionRepositories.TableExists(tableName, registryName))
                {
                    await HandleExistingTable(tableName, registryName, deleteAction, dataTable);
                }
                else
                {
                    await _importacionRepositories.CreateTable(dataTable, tableName, registryName);
                    await _importacionRepositories.BulkInsert(dataTable, tableName, registryName);
                }
            }
            catch (Exception ex)
            {
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
