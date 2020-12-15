using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaUtils.Interfaces.DbManagement
{
    /// <summary>
    /// Stati del contesto transazionale
    /// </summary>
    public enum TransactionContextStateEnum
    {
        Unstarted,
        Started,
        Completed
    }

    /// <summary>
    /// Interfaccia per la gestione del contesto transazionale
    /// </summary>
    public interface ITransactionContext : IDisposable
    {   
        /// <summary>
        /// Stato della transazione
        /// </summary>
        TransactionContextStateEnum State
        {
            get;
        }

        /// <summary>
        /// Chiave del database
        /// </summary>
        string DbKey
        {
            get;
        }

        /// <summary>
        /// Indica se l'istanza è proprietaria della transazione
        /// </summary>
        bool IsTransactionOwner
        {
            get;
        }

        /// <summary>
        /// Imposta esplicitamente la transazione come completata con successo.
        /// Il commit della transazione deve essere effettuato nel metodo Dispose
        /// della classe che implementa l'interfaccia.
        /// </summary>
        void Complete();
    }
}
