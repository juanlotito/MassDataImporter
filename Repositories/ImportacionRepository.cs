using Dapper;
using importacionmasiva.api.net.DataBase;
using importacionmasiva.api.net.Models.DB;
using importacionmasiva.api.net.Repositories.Interface;
using importacionmasiva.api.net.Utils.Exceptions;
using System.Data;
using System.Data.SqlClient;

namespace importacionmasiva.api.net.Repositories
{
    public class ImportacionRepository : BaseRepository, IImportacionRepositories
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ImportacionRepository(ILogger<TableDefinition> logger, IDbContext dbContext, IDbConnectionFactory connectionFactory) : base(logger, dbContext)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> TableExists(string tableName, string registryName)
        {
            try
            {
                string query = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @TableName) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END";
                
                using var dbContext = new DbContext(_connectionFactory, registryName);
                using var connection = _connectionFactory.CreateConnection(registryName);

                var exists = await connection.QueryFirstOrDefaultAsync<bool>(query, new { TableName = tableName });

                return exists;
            }
            catch (SqlException sqlex)
            {
                if (sqlex.Number == 220)
                    throw new CustomException(422, "Revisá los códigos ingresados, el valor numérico no debe superar 255", sqlex);

                throw new CustomException(sqlex.State.ToString() == "105" ? 404 : 500, sqlex.Message, sqlex);
            }
            catch (Exception ex)
            {
                throw new CustomException(500, ex.Message, ex);
            }
        }

        public async Task<TableDefinition> GetTableDefinition(string tableName, string registryName)
        {
            try 
            {
                string query = @"SELECT COLUMN_NAME 'Name' FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @TableName ORDER BY ORDINAL_POSITION";

                using var dbContext = new DbContext(_connectionFactory, registryName);
                using var connection = _connectionFactory.CreateConnection(registryName);

                var columns = await connection.QueryAsync<Column>(query, new { TableName = tableName });

                return new TableDefinition { Columns = columns.ToList() };
            }
            catch (SqlException sqlex)
            {
                if (sqlex.Number == 220)
                    throw new CustomException(422, "Revisá los códigos ingresados, el valor numérico no debe superar 255", sqlex);

                throw new CustomException(sqlex.State.ToString() == "105" ? 404 : 500, sqlex.Message, sqlex);
            }
            catch (Exception ex)
            {
                throw new CustomException(500, ex.Message, ex);
            }
        }

        public async Task CreateTable(DataTable dataTable, string tableName, string registryName)
        {
            try 
            {
                var columnDefinitions = new List<string>();

                foreach (DataColumn column in dataTable.Columns)
                {
                    columnDefinitions.Add($"[{column.ColumnName}] NVARCHAR(MAX)");
                }

                string columns = string.Join(", ", columnDefinitions);
                string createTableQuery = $"CREATE TABLE [{tableName}] ({columns})";

                using var dbContext = new DbContext(_connectionFactory, registryName);
                using var connection = _connectionFactory.CreateConnection(registryName);
                await connection.ExecuteAsync(createTableQuery);
            }
            catch (SqlException sqlex)
            {
                if (sqlex.Number == 220)
                    throw new CustomException(422, "Revisá los códigos ingresados, el valor numérico no debe superar 255", sqlex);

                throw new CustomException(sqlex.State.ToString() == "105" ? 404 : 500, sqlex.Message, sqlex);
            }
            catch (Exception ex)
            {
                throw new CustomException(500, ex.Message, ex);
            }
        }

        public async Task BulkInsert(DataTable dataTable, string tableName, string registryName)
        {
            try 
            {
                using var dbContext = new DbContext(_connectionFactory, registryName);
                using var connection = (SqlConnection)_connectionFactory.CreateConnection(registryName);

                connection.Open();

                using var bulkCopy = new SqlBulkCopy(connection)
                {
                    DestinationTableName = tableName,
                    BatchSize = 1000,
                    BulkCopyTimeout = 600
                };

                foreach (DataColumn column in dataTable.Columns)
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }

                await bulkCopy.WriteToServerAsync(dataTable);
            }
            catch (SqlException sqlex)
            {
                if (sqlex.Number == 220)
                    throw new CustomException(422, "Revisá los códigos ingresados, el valor numérico no debe superar 255", sqlex);

                throw new CustomException(sqlex.State.ToString() == "105" ? 404 : 500, sqlex.Message, sqlex);
            }
            catch (Exception ex)
            {
                throw new CustomException(500, ex.Message, ex);
            }
        }

    }
}
