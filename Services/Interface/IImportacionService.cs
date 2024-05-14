using importacionmasiva.api.net.Models.Enum;

namespace importacionmasiva.api.net.Services.Interface
{
    public interface IImportacionService
    {
        Task ImportFromExcel(IFormFile dataset, string registryName, string tableName, DeleteAction deleteRecords = DeleteAction.Truncate);

        Task ImportFromTxt(IFormFile dataset, string registryName, string tableName, DeleteAction deleteRecords = DeleteAction.Delete);

        Task ImportFromCsv(IFormFile dataset, string registryName, string tableName, DeleteAction deleteAction = DeleteAction.None);
    }
}
