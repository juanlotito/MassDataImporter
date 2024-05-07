using importacionmasiva.api.net.UnityOfWork;
using System.Data;

namespace importacionmasiva.api.net.DataBase
{
    public enum IDbContextState
    {
        Closed,
        Open,
        Comitted,
        RolledBack
    }

    public interface IDbContext : IDisposable
    {
        /// <summary>
        /// Representa el estado del contexto.
        /// </summary>
        IDbContextState State { get; }

        /// <summary>
        /// Representa la conexión.
        /// </summary>
        IDbConnection Connection { get; }

        /// <summary>
        /// Repesenta la transacción actual
        /// </summary>
        IDbTransaction Transaction { get; }

        /// <summary>
        /// Representa la Unit of work actual.
        /// </summary>
        IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Confirma los cambios IUnitOfWork
        /// Setea el estado en IDbContextState.Committed
        /// Finaliza UnitOfWork
        /// </summary>
        void Commit();

        /// <summary>
        /// Revierte los cambios IUnitOfWork
        /// Setea el estado en IDbContextState.Rolledback
        /// Finaliza UnitOfWork
        /// </summary>
        void Rollback();

        public IDbTransaction newTransaction();
    }
}
