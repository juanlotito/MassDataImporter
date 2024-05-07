using Dapper;
using importacionmasiva.api.net.DataBase;
using System.Data;

namespace importacionmasiva.api.net.Repositories
{
    public class BaseRepository
    {
        public readonly ILogger logger;

        private readonly IDbContext dbContext;

        private IDbConnection Connection => dbContext.UnitOfWork.Transaction.Connection ?? dbContext.Connection;

        private IDbTransaction Transaction => dbContext.UnitOfWork.Transaction ?? dbContext.newTransaction();

        public BaseRepository(ILogger logger, IDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        public async Task<List<T>> QueryAsync<T>(string sp, object parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            try
            {
                return (await Connection.QueryAsync<T>(sp, parameters, Transaction, null, commandType)).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new List<T>();
            }
        }

        public async Task<List<dynamic>> QueryAsync(string sp, object parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            try
            {
                return (await Connection.QueryAsync(sp, parameters, Transaction, null, commandType)).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new List<dynamic>();
            }
        }

        public async Task<T> QuerySingleAsync<T>(string sp, object parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            try
            {
                return await Connection.QuerySingleAsync<T>(sp, parameters, Transaction, null, commandType);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<T> QueryFirstAsync<T>(string sp, object parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            try
            {
                return await Connection.QueryFirstAsync<T>(sp, parameters, Transaction, null, commandType);
            }
            catch (Exception ex)
            {
                //var args = CreateArgs(sp, parameters, methodName);
                logger.LogError(ex.Message);
                return default(T);
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sp, object parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            try
            {
                var resp = await Connection.QueryFirstAsync<T>(sp, parameters, Transaction, null, commandType);
                return resp;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return default(T);
            }
        }

        public async Task<SqlMapper.GridReader> QueryMultipleAsync(string sp, object parameters = null)
        {
            try
            {
                return await Connection.QueryMultipleAsync(sp, parameters, Transaction, null, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<int> ExecuteAsync(string sp, object parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            try
            {
                return await Connection.ExecuteAsync(sp, parameters, Transaction, null, commandType);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }

    }
}
