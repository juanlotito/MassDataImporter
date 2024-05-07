using importacionmasiva.api.net.Models.DB;
using System.Data;

namespace importacionmasiva.api.net.Repositories.Interface
{
    public interface IImportacionRepositories
    {
        Task<TableDefinition> GetTableDefinition(string tableName, string registryName);
        Task<bool> TableExists(string tableName, string registryName);
        Task CreateTable(DataTable dataTable, string tableName, string registryName);
        Task BulkInsert(DataTable dataTable, string tableName, string registryName);
    }
}
