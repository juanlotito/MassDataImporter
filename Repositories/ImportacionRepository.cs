using Dapper;
using importacionmasiva.api.net.DataBase;
using importacionmasiva.api.net.Models.DB;
using importacionmasiva.api.net.Repositories.Interface;
using importacionmasiva.api.net.Utils.Exceptions;
using System.Data;
using System.Data.SqlClient;

namespace importacionmasiva.api.net.Repositories
{
    public class ImportacionRepository : BaseRepository, IImportacionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ImportacionRepository(ILogger<TableDefinition> logger, IDbContext dbContext, IDbConnectionFactory connectionFactory) : base(logger, dbContext)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> TableExists(string tableName, string registryName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("El nombre de la tabla no puede estar vacío.", nameof(tableName));
            if (string.IsNullOrWhiteSpace(registryName))
                throw new ArgumentException("El nombre del registro no puede estar vacío.", nameof(registryName));

            try
            {
                string query = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @TableName) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END";

                using (var connection = _connectionFactory.CreateConnection(registryName))
                {
                    return await connection.QueryFirstOrDefaultAsync<bool>(query, new { TableName = tableName });
                }
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
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("El nombre de la tabla no puede estar vacío.", nameof(tableName));
            if (string.IsNullOrWhiteSpace(registryName))
                throw new ArgumentException("El nombre del registro no puede estar vacío.", nameof(registryName));

            try
            {
                string query = @"SELECT COLUMN_NAME 'Name' FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @TableName ORDER BY ORDINAL_POSITION";

                using (var connection = _connectionFactory.CreateConnection(registryName))
                {
                    var columns = await connection.QueryAsync<Column>(query, new { TableName = tableName });

                    return new TableDefinition { Columns = columns.ToList() };
                }
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
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable));
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("El nombre de la tabla no puede estar vacío.", nameof(tableName));
            if (string.IsNullOrWhiteSpace(registryName))
                throw new ArgumentException("El nombre del registro no puede estar vacío.", nameof(registryName));

            try
            {
                var columnDefinitions = dataTable.Columns
                    .Cast<DataColumn>()
                    .Select(column => $"[{column.ColumnName}] NVARCHAR(MAX)")
                    .ToArray();

                string columns = string.Join(", ", columnDefinitions);
                string createTableQuery = $"CREATE TABLE [{tableName}] ({columns})";

                using (var connection = _connectionFactory.CreateConnection(registryName))
                {
                    await connection.ExecuteAsync(createTableQuery);
                }
            }
            catch (SqlException sqlex)
            {
                switch (sqlex.Number)
                {
                    case 220:
                        throw new CustomException(422, "Revisá los códigos ingresados, el valor numérico no debe superar 255", sqlex);
                    case 105:
                        throw new CustomException(404, sqlex.Message, sqlex);
                    default:
                        throw new CustomException(500, sqlex.Message, sqlex);
                }
            }
            catch (Exception ex)
            {
                throw new CustomException(500, ex.Message, ex);
            }
        }

        public async Task BulkInsert(DataTable dataTable, string tableName, string registryName)
        {
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable));
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("El nombre de la tabla no puede estar vacío.", nameof(tableName));
            if (string.IsNullOrWhiteSpace(registryName))
                throw new ArgumentException("El nombre del registro no puede estar vacío.", nameof(registryName));

            try
            {
                using (var connection = (SqlConnection)_connectionFactory.CreateConnection(registryName))
                {
                    await connection.OpenAsync();

                    using (var bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.BatchSize = 100000;
                        bulkCopy.BulkCopyTimeout = 600;

                        foreach (DataColumn column in dataTable.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                        }

                        await bulkCopy.WriteToServerAsync(dataTable);
                    }
                }
            }
            catch (SqlException sqlex)
            {
                switch (sqlex.Number)
                {
                    case 220:
                        throw new CustomException(422, "Revisá los códigos ingresados, el valor numérico no debe superar 255", sqlex);
                    case 105:
                        throw new CustomException(404, sqlex.Message, sqlex);
                    default:
                        throw new CustomException(500, sqlex.Message, sqlex);
                }
            }
            catch (Exception ex)
            {
                throw new CustomException(500, ex.Message, ex);
            }
        }

        public async Task DeleteRecords(string tableName, string registryName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("El nombre de la tabla no puede estar vacío.", nameof(tableName));
            if (string.IsNullOrWhiteSpace(registryName))
                throw new ArgumentException("El nombre del registro no puede estar vacío.", nameof(registryName));

            try
            {
                string deleteQuery = $"DELETE FROM {tableName}";

                using (var connection = _connectionFactory.CreateConnection(registryName))
                {
                    await connection.ExecuteAsync(deleteQuery);
                }
            }
            catch (SqlException sqlex)
            {
                if (sqlex.Number == 4712)
                    throw new CustomException(422, "No se puede truncar la tabla porque tiene restricciones de clave foránea.", sqlex);

                throw new CustomException(sqlex.State.ToString() == "105" ? 404 : 500, sqlex.Message, sqlex);
            }
            catch (Exception ex)
            {
                throw new CustomException(500, ex.Message, ex);
            }
        }

        public async Task TruncateTable(string tableName, string registryName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("El nombre de la tabla no puede estar vacío.", nameof(tableName));
            if (string.IsNullOrWhiteSpace(registryName))
                throw new ArgumentException("El nombre del registro no puede estar vacío.", nameof(registryName));

            try
            {
                string truncateQuery = $"TRUNCATE TABLE {tableName}";

                using (var connection = _connectionFactory.CreateConnection(registryName))
                {
                    await connection.ExecuteAsync(truncateQuery);
                }
            }
            catch (SqlException sqlex)
            {
                if (sqlex.Number == 4712)
                    throw new CustomException(422, "No se puede truncar la tabla porque tiene restricciones de clave foránea.", sqlex);

                throw new CustomException(sqlex.State.ToString() == "105" ? 404 : 500, sqlex.Message, sqlex);
            }
            catch (Exception ex)
            {
                throw new CustomException(500, ex.Message, ex);
            }
        }

    }
}
