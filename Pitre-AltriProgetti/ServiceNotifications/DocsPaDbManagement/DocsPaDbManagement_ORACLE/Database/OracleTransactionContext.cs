using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OracleClient;
using DocsPaDB;

namespace DocsPaDbManagement.Database.Oracle
{
    /// <summary>
    /// Gestione contesto transazionale database SqlServer
    /// </summary>
    public class OracleTransactionContext : TransactionContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="iso"></param>
        public OracleTransactionContext(string dbKey, IsolationLevel iso)
            : base(dbKey, iso)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbKey"></param>
        public OracleTransactionContext(string dbKey) : base(dbKey)
        {
        }

        /// <summary>
        /// Creazione connessione database SqlServer
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected override IDbConnection OpenConnection(string connectionString)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Reperimento transazione corrente
        /// </summary>
        /// <returns></returns>
        internal OracleTransaction GetCurrentTransaction()
        {
            return (OracleTransaction)this.Transaction;
        }
    }
}
