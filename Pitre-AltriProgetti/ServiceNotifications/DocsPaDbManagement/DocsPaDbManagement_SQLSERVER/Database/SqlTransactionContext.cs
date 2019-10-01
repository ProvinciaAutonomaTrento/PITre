using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DocsPaDB;

namespace DocsPaDbManagement.Database.SqlServer
{
    /// <summary>
    /// Gestione contesto transazionale database SqlServer
    /// </summary>
    public class SqlTransactionContext : TransactionContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="iso"></param>
        public SqlTransactionContext(string dbKey, IsolationLevel iso) : base(dbKey, iso)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbKey"></param>
        public SqlTransactionContext(string dbKey) : base(dbKey)
        {
        }

        /// <summary>
        /// Creazione connessione database SqlServer
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected override IDbConnection OpenConnection(string connectionString)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Reperimento transazione corrente
        /// </summary>
        /// <returns></returns>
        internal SqlTransaction GetCurrentTransaction()
        {
            return (SqlTransaction) this.Transaction;
        }
    }
}
