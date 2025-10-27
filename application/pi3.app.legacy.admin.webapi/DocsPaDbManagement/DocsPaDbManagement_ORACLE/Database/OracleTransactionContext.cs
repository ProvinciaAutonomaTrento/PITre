// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;
using DocsPaDB;

using DocsPaConnection = Oracle.ManagedDataAccess.Client.OracleConnection;
using DocsPaTransaction = Oracle.ManagedDataAccess.Client.OracleTransaction;
using DocsPaCommand = Oracle.ManagedDataAccess.Client.OracleCommand;
using DocsPaDataAdapter = Oracle.ManagedDataAccess.Client.OracleDataAdapter;
using DocsPaDataReader = Oracle.ManagedDataAccess.Client.OracleDataReader;


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

        public OracleTransactionContext(string dbKey, IsolationLevel iso, bool openConnection)
            : base(dbKey, iso, openConnection)
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
