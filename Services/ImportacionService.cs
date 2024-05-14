using importacionmasiva.api.net.Models.Enum;
using importacionmasiva.api.net.Repositories.Interface;
using importacionmasiva.api.net.Services.Interface;
using importacionmasiva.api.net.Utils.Excel;
using importacionmasiva.api.net.Utils.Exceptions;
using importacionmasiva.api.net.Utils.Txt;
using System.Data;

namespace importacionmasiva.api.net.Services
{
    public class ImportacionService : IImportacionService
    {
        private readonly IImportacionRepositories _importacionRepositories;
        private readonly ExcelUtils _excelUtils = new ExcelUtils();
        private readonly TxtUtils _txtUtils = new TxtUtils();

        public ImportacionService(IImportacionRepositories importacionRepositories)
        {
            _importacionRepositories = importacionRepositories;
        }

        public async Task ImportFromExcel(IFormFile dataset, string registryName, string tableName, DeleteAction deleteAction = DeleteAction.None)
        {
            try
            {
                DataTable dataTable = _excelUtils.ReadExcelToDataTable(dataset);

                if (await _importacionRepositories.TableExists(tableName, registryName))
                {
                    if (deleteAction == DeleteAction.Delete)
                        await _importacionRepositories.DeleteRecords(tableName, registryName);

        public async Task ImportFromCsv(IFormFile dataset, string registryName, string tableName, DeleteAction deleteAction = DeleteAction.None)
        {
            DataTable dataTable = _csvUtils.ReadCsvToDataTable(dataset);
            await ImportData(dataTable, registryName, tableName, deleteAction);
        }

                    var tableDefinition = await _importacionRepositories.GetTableDefinition(tableName, registryName);

                    if (tableDefinition.Columns.Count != dataTable.Columns.Count)
                        throw new CustomException(400, "El número de columnas no coincide con el de la tabla existente.");

                    for (int i = 0; i < tableDefinition.Columns.Count; i++)
                    {
                        if (tableDefinition.Columns[i].Name != dataTable.Columns[i].ColumnName)
                            throw new CustomException(400, $"El nombre de la columna '{dataTable.Columns[i].ColumnName}' no coincide con el de la tabla: '{tableDefinition.Columns[i].Name}'.");
                    }

                    await _importacionRepositories.BulkInsert(dataTable, tableName, registryName);
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

        public async Task ImportFromTxt(IFormFile dataset, string registryName, string tableName, DeleteAction deleteAction = DeleteAction.None)
        {
            try
            {
                DataTable dataTable = _txtUtils.ReadTxtToDataTable(dataset);

                if (await _importacionRepositories.TableExists(tableName, registryName))
                {
                    if (deleteAction == DeleteAction.Delete)
                        await _importacionRepositories.DeleteRecords(tableName, registryName);
                    else if (deleteAction == DeleteAction.Truncate)
                        await _importacionRepositories.TruncateTable(tableName, registryName);

                    var tableDefinition = await _importacionRepositories.GetTableDefinition(tableName, registryName);

                    if (tableDefinition.Columns.Count != dataTable.Columns.Count)
                        throw new CustomException(400, "El número de columnas no coincide con el de la tabla existente.");

                    for (int i = 0; i < tableDefinition.Columns.Count; i++)
                    {
                        if (tableDefinition.Columns[i].Name != dataTable.Columns[i].ColumnName)
                            throw new CustomException(400, $"El nombre de la columna '{dataTable.Columns[i].ColumnName}' no coincide con el de la tabla: '{tableDefinition.Columns[i].Name}'.");
                    }

                    await _importacionRepositories.BulkInsert(dataTable, tableName, registryName);
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

    }
}
