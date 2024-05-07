using System.Data;

namespace importacionmasiva.api.net.UnityOfWork
{
    public enum IUnitOfWorkState
    {
        Open,
        Comitted,
        RolledBack
    }

    public interface IUnitOfWork
    {
        /// <summary>
        /// Representa el estado actual de nuestra unidad de trabajo.
        /// </summary>
        IUnitOfWorkState State { get; }

        /// <summary>
        /// Representa la transacción actual.
        /// </summary>
        IDbTransaction Transaction { get; }

        /// <summary>
        /// Confirma los cambios.
        /// Cierra la Transaction.Connection
        /// Actualiza el estado a IUnitOfWorkState.Comitted
        /// Libera los recursos ejecutando Dispose Transaction.Connect & Transaction
        /// </summary>
        void Commit();

        /// <summary>
        /// Revierte los cambios.
        /// Cierra la Transaction.Connection
        /// Actualiza el estado a IUnitOfWorkState.RolledBack
        /// Libera los recursos ejecutando Dispose Transaction.Connect & Transaction
        /// </summary>
        void Rollback();

        void Dispose();
    }
}
