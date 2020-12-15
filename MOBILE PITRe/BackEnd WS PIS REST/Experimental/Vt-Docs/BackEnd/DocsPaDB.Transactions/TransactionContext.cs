using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Web;
using System.Collections;
using DocsPaUtils.Interfaces.DbManagement;

namespace DocsPaDB
{
    /// <summary>
    /// Classe per la gestione delle transazioni su singolo database.
    /// <remarks>
    /// Tutte le operazioni di accesso ai dati effettuate nell'ambito
    /// del ciclo di vita di un'istanza di "TransactionContext",
    /// vengono effettuate in un'unica transazione.
    /// Il commit del contesto transazionale deve essere 
    /// effettuato richiamando esplicitamente il metodo "Complete".
    /// </remarks>
    /// </summary>
    public abstract class TransactionContext : ITransactionContext
    {
        /// <summary>
        /// Chiave del contesto transazionale
        /// </summary>
        private const string TRANSACTION_CONTEXT_SLOT = "TransactionContext";

        /// <summary>
        /// Chiave del database
        /// </summary>
        private string _dbKey = string.Empty;

        /// <summary>
        /// Se true, l'oggetto è disposed
        /// </summary>
        protected bool _disposed = false;

        /// <summary>
        /// Transazione corrente
        /// </summary>
        private IDbTransaction _transaction = null;

        /// <summary>
        /// Stato del contesto transazionale
        /// </summary>
        private TransactionContextStateEnum _state = TransactionContextStateEnum.Unstarted;

        /// <summary>
        /// Se true, indica che l'instanza è proprietaria della transazione aperta
        /// </summary>
        private bool _isTransactionOwner = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="iso"></param>
        public TransactionContext(string dbKey, IsolationLevel iso)
        {
            this._dbKey = dbKey;

            if (string.IsNullOrEmpty(dbKey))
                dbKey = "connectionString";
            
            // Verifica che non sia già presente un contesto transazionale
            if (!HasTransactionalContext)
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[dbKey]))
                {
                    // Avvio nuova transazione
                    this._transaction = this.BeginTransaction(ConfigurationManager.AppSettings[dbKey], iso);

                    SetTransactionContext(this);

                    this._isTransactionOwner = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbKey"></param>
        public TransactionContext(string dbKey) : this(dbKey, IsolationLevel.Unspecified)
        {}

        /// <summary>
        /// Distruttore: deallocazione risorse
        /// </summary>
        ~TransactionContext()
        {
            this.Dispose();
        }

        /// <summary>
        /// Chiave del database
        /// </summary>
        public string DbKey
        {
            get { return this._dbKey; }
        }

        /// <summary>
        /// Indica se l'istanza è proprietaria della transazione
        /// </summary>
        public bool IsTransactionOwner
        {
            get { return this._isTransactionOwner; }
        }

        /// <summary>
        /// Stato della transazione
        /// </summary>
        public TransactionContextStateEnum State
        {
            get { return this._state; }
        }

        /// <summary>
        /// Imposta la transazione come completata con successo
        /// </summary>
        public void Complete()
        {
            if (this._isTransactionOwner && this._state == TransactionContextStateEnum.Started)
                this._state = TransactionContextStateEnum.Completed;
        }

        /// <summary>
        /// Verifica se è presente un contesto transazionale
        /// </summary>
        public static bool HasTransactionalContext
        {
            get { return (Current != null); }
        }

        /// <summary>
        /// Reperimento contesto transazionale corrente
        /// </summary>
        public static TransactionContext Current
        {
            get { return GetTransactionContext(); }
        }

        /// <summary>
        /// Deallocazione risorse
        /// </summary>
        public void Dispose()
        {
            if (!this._disposed)
            {
                if (this._isTransactionOwner && this._transaction != null)
                {
                    IDbConnection connection = this._transaction.Connection;

                    if (this.State == TransactionContextStateEnum.Completed)
                    {
                        // Se la transazione viene completata con successo
                        this._transaction.Commit();
                    }
                    else if (this.State == TransactionContextStateEnum.Started)
                    {
                        // Non è stato richiesto il completamento della transazione,
                        // viene effettuato il rollback
                        this._transaction.Rollback();
                    }

                    this._transaction.Dispose();
                    this._transaction = null;

                    // Chiusura connessione
                    connection.Dispose();
                    connection = null;

                    // Rimozione del contesto transazionale dal thread corrente
                    SetTransactionContext(null);

                    this._isTransactionOwner = false;
                }
                
                this._disposed = true;
            }
        }

        /// <summary>
        /// Avvio della transazione
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="iso"></param>
        protected IDbTransaction BeginTransaction(string connectionString, IsolationLevel iso)
        {
            IDbTransaction transaction = null;

            if (this._state == TransactionContextStateEnum.Unstarted)
            {
                // Creazione connessione
                IDbConnection connection = this.OpenConnection(connectionString);
                transaction = connection.BeginTransaction(iso);
                this._state = TransactionContextStateEnum.Started;
            }

            return transaction;
        }

        /// <summary>
        /// Reperimento transazione corrente
        /// </summary>
        protected IDbTransaction Transaction
        {
            get
            {
                return this._transaction;
            }
        }

        /// <summary>
        /// Creazione connessione concreta al database
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected abstract IDbConnection OpenConnection(string connectionString);

        /// <summary>
        /// Reperimento oggetto TransactionContext dal thread corrente
        /// </summary>
        /// <returns></returns>
        protected static TransactionContext GetTransactionContext()
        {
            if (HttpContext.Current == null)
            {
                LocalDataStoreSlot slot = Thread.GetNamedDataSlot(TRANSACTION_CONTEXT_SLOT);
                return (TransactionContext)Thread.GetData(slot);
            }
            else
            {
                return (TransactionContext)HttpContext.Current.Items[TRANSACTION_CONTEXT_SLOT];
            }
        }

        /// <summary>
        /// Impostazione oggetto TransactionContext nel thread corrente
        /// </summary>
        /// <param name="transactionContext"></param>
        protected static void SetTransactionContext(TransactionContext transactionContext)
        {
            if (HttpContext.Current == null)
            {
                LocalDataStoreSlot slot = Thread.GetNamedDataSlot(TRANSACTION_CONTEXT_SLOT);
                Thread.SetData(slot, transactionContext);
            }
            else
            {
                if (transactionContext != null)
                    HttpContext.Current.Items[TRANSACTION_CONTEXT_SLOT] = transactionContext;
                else
                    HttpContext.Current.Items.Remove(TRANSACTION_CONTEXT_SLOT);
            }
        }
    }
}