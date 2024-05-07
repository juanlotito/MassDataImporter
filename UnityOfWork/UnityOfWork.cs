using System.Data;
using System.Data.Common;

namespace importacionmasiva.api.net.UnityOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IDbTransaction transaction)
        {
            State = IUnitOfWorkState.Open;
            Transaction = transaction;
        }

        public IUnitOfWorkState State { get; private set; }

        public IDbTransaction Transaction { get; private set; }

        public void Commit()
        {
            try
            {
                Transaction.Commit();
                State = IUnitOfWorkState.Comitted;
            }
            catch (Exception)
            {
                Transaction.Rollback();
                throw;
            }
        }

        public void Rollback()
        {
            Transaction.Rollback();
            State = IUnitOfWorkState.RolledBack;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Transaction?.Dispose();
            }
        }
    }
}
