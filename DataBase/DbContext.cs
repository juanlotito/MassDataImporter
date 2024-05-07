using importacionmasiva.api.net.UnityOfWork;
using System.Data;

namespace importacionmasiva.api.net.DataBase
{
    public class DbContext : IDbContext
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private IUnitOfWork _unitOfWork;
        private string _registryKey;
        private bool _disposed;

        public DbContext(IDbConnectionFactory connectionFactory, string registryKey)
        {
            _connectionFactory = connectionFactory;
            _registryKey = registryKey;
        }

        public IDbConnection Connection => _connection ?? (_connection = OpenConnection());

        public IDbTransaction Transaction => _transaction ?? (_transaction = Connection.BeginTransaction());

        public IUnitOfWork UnitOfWork => _unitOfWork ?? (_unitOfWork = new UnitOfWork(Transaction));

        public IDbContextState State { get; private set; } = IDbContextState.Closed;

        public void Commit()
        {
            try
            {
                UnitOfWork.Commit();
                Transaction.Commit();
                State = IDbContextState.Comitted;
            }
            catch
            {
                Transaction.Rollback();
                State = IDbContextState.RolledBack;
                throw;
            }
            finally
            {
                Dispose();
            }
        }

        private IDbConnection OpenConnection()
        {
            if (_connection == null)
            {
                var connection = _connectionFactory.CreateConnection(_registryKey);
                connection.Open();
                return connection;
            }
            return _connection;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _connection?.Close();
                    _connection?.Dispose();
                    _unitOfWork?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Rollback()
        {
            try
            {
                UnitOfWork.Rollback();
                State = IDbContextState.RolledBack;
            }
            finally
            {
                Dispose();
            }
        }

        public IDbTransaction newTransaction()
        {
            var tran = Connection.BeginTransaction();

            this._unitOfWork = _unitOfWork ?? (_unitOfWork = new UnitOfWork(tran));

            return Connection.BeginTransaction();
        }
    }
}
