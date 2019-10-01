//#define TRACE_DB

using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Reflection;
using DocsPaDB;

#if USE_ODP
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using Debugger			= DocsPaUtils.LogsManagement.Debugger;
using DocsPaConnection  = Oracle.DataAccess.Client.OracleConnection;
using DocsPaTransaction = Oracle.DataAccess.Client.OracleTransaction;
using DocsPaCommand		= Oracle.DataAccess.Client.OracleCommand;  
using DocsPaDataAdapter = Oracle.DataAccess.Client.OracleDataAdapter;  
using DocsPaDataReader	= Oracle.DataAccess.Client.OracleDataReader;  
#else
using System.Data.OracleClient;
using DocsPaConnection  = System.Data.OracleClient.OracleConnection;
using DocsPaTransaction = System.Data.OracleClient.OracleTransaction;
using DocsPaCommand		= System.Data.OracleClient.OracleCommand;  
using DocsPaDataAdapter = System.Data.OracleClient.OracleDataAdapter;  
using DocsPaDataReader	= System.Data.OracleClient.OracleDataReader;  
#endif

using DocsPaUtils.Data;
using DocsPaUtils.Interfaces.DbManagement;
using log4net;
using System.Collections.Generic;

namespace DocsPaDbManagement.Database.Oracle
{
	/// <summary>
	/// Questa classe implementa l'integrazione con il DB Oracle. Ogni istanza della classe
	/// comprende una singola connessione ed una singola transazione.
	/// </summary>
	public class Database : IDatabase	
	{
        private ILog logger = LogManager.GetLogger(typeof(Database));
		#region Private Attributes
		/// <summary>
		/// Istanza della connessione.
		/// </summary>
		private DocsPaConnection conn = null;

        /// <summary>
        /// Cache dei parametri delle stored procedures
        /// </summary>
        private static Dictionary<String, List<String>> spParamCacheCollection = new Dictionary<string, List<string>>();

		/// <summary>
		/// Istanza della transazione. Questo oggetto viente instanziato quando si apre una 
		/// nuova transazione e rimane valido fino alla chiusura della transazione stessa 
		/// oppure fino alla chiusura/deallocazione della connessione SqlConnection.
		/// </summary>
		private DocsPaTransaction transaction = null;

		/// <summary>
		/// Questo flag è 'true' se la connessione è stata creata in questa istanza, 
		/// 'false' se è stata acquisita da un'altra istanza (passata come parametro al costruttore).
		/// </summary>
		private bool isOwnConn = true; // Il valore default è 'true'

		/// <summary>
		/// Questa proprietà ritorna il messaggio dell'ultima eccezione avvenuta nell'istanza.
		/// </summary>
		private string LastException = "";
		#endregion

		#region Properties
		/// <summary>
		/// Questa proprietà ritorna un flag (isOwnConn) che indica se la connessione è stata
		/// creata in questa instanza o è stata acquisita.
		/// </summary>
		public bool IsOwnConnection
		{
			get
			{
				return isOwnConn;
			}
		}

		/// <summary>
		/// Questa proprietà ritorna 'true' se la connessione è instanziata ed aperta, 
		/// altrimenti ritorna 'false'
		/// </summary>
		public bool ConnectionExists
		{
			get
			{
				bool result = true; // Presume che esista una connessione aperta

				/* Future implementazioni dell'oggetto OracleConnection potrebbero richiedere
				 * la necessità di controllare ulteriori stati della connessione. 
				 */
				if(conn == null || conn.State == ConnectionState.Closed)
				{
					result = false;
				}

				return result;
			}
		}

		/// <summary>
		/// Questa proprietà ritorna 'true' se esiste una transazione attiva, 
		/// altrimenti ritorna 'false'
		/// </summary>
		public bool TransactionExists
		{
			get
			{
				bool result = false; // Presume che non esista una transazione

				if(transaction != null && transaction.Connection != null)
				{
					result = true;
				}

				return result;
			}
		}

		/// <summary>
		/// Questa proprietà ritorna la connessione.
		/// </summary>
		public DocsPaConnection Connection
		{
			get
			{
				return conn;
			}
		}

		/// <summary>
		/// Questa proprietà ritorna la transazione.
		/// </summary>
		public DocsPaTransaction Transaction
		{
			get
			{
				return transaction;
			}
		}

		/// <summary>
		/// Ritorna l'ultima eccezione generata
		/// </summary>
		/// <returns></returns>
		public string LastExceptionMessage
		{
			get
			{
				return this.LastException;
			}
		}

		public string DBType
		{
			get
			{
				return "oracle";
			}
		}
		#endregion

		#region Constructors
        /// <summary>
        /// Costruttore default per l'inizializzazione della connessione.
        /// </summary>
        public Database()
        {
            string connectionString = ConfigurationManager.AppSettings["connectionString"];

            if (TransactionContext.HasTransactionalContext)
                AcquireTransaction(((OracleTransactionContext)TransactionContext.Current).GetCurrentTransaction());
            else
                InstantiateConnection(connectionString);
        }

        /// <summary>
        /// Costruttore per l'inizializzazione della connessione.
        /// </summary>
        public Database(string dbConnectionString)
        {
            if (TransactionContext.HasTransactionalContext)
                AcquireTransaction(((OracleTransactionContext)TransactionContext.Current).GetCurrentTransaction());
            else
                InstantiateConnection(dbConnectionString);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dbConnectionString"></param>
		public void InstantiateConnection(string dbConnectionString)
		{
			// Inizializza una connessione
			try
			{
				conn = new DocsPaConnection(dbConnectionString);
				isOwnConn = true;
			}
			catch(Exception exception)
			{
				// Impossibile instanziare una connessione
				this.LastException = exception.ToString();
				logger.Debug("Errore durante la connessione al DB.", exception);
			}
		}
		#endregion

		#region Transaction Management

        /// <summary>
        /// Inizia una transazione sulla connessione. Ciò è possibile solo se la connessione
        /// è stata creata in questa istanza (vedi proprietà IsOwnConnection).
        /// </summary>
        /// <returns>true = OK; false = Transazione non avviata</returns>
        public bool BeginTransaction()
        {
            return this.BeginTransaction(IsolationLevel.ReadCommitted);
        }


        public bool BeginTransaction(IsolationLevel isolationLevel)
        {
            bool result = true; // Presume che non esista una transazione

            if (!TransactionContext.HasTransactionalContext && isOwnConn)
            {
                this.OpenConnection();

                if (!TransactionExists)
                    transaction = conn.BeginTransaction(isolationLevel);
            }
            else
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Chiudi la transazione sulla connessione. Ciò è possibile solo se la connessione
        /// è stata creata in questa istanza (vedi proprietà IsOwnConnection).
        /// </summary>
        /// <returns>true = OK; false = Errore in chiusura transazione</returns>
        public bool CommitTransaction()
        {
            bool result = true; // Presume successo

            if (!TransactionContext.HasTransactionalContext && 
                isOwnConn && ConnectionExists && TransactionExists)
            {
                transaction.Commit();
                transaction = null;
            }
            else
            {
                //La transazione non è più attiva
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Annulla una transazione sulla connessione. Ciò è possibile solo se la connessione
        /// è stata creata in questa istanza (vedi proprietà IsOwnConnection).
        /// </summary>
        /// <returns>true = OK; false = Errore nell'annullamento della transazione</returns>
        public bool RollbackTransaction()
        {
            bool result = true; // Presume successo

            if (!TransactionContext.HasTransactionalContext && isOwnConn && ConnectionExists && TransactionExists)
            {
                transaction.Rollback();
                transaction = null;
            }
            else
            {
                //La transazione non è più attiva
                result = false;
            }

            return result;
        }
		#endregion

		#region Database Access Management
		/// <summary>
		/// Esegue una query sul database.
		/// </summary>
		/// <param name="dataSet">Oggetto DataSet da popolare. Se ci sono errori durante 
		/// il caricamento dei dati l'oggetto DataSet ritorna 'null'.</param>
		/// <param name="command">Query da eseguire sul database</param>
		/// <returns>true = OK; false = Errore durnte l'esecuzione della query</returns>
		public bool ExecuteQuery(out DataSet dataSet, string command)
		{
			bool result = true; // Presume successo
#if TRACE_DB
			DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_DB");
#endif
			result = this.ExecuteQuery(out dataSet, "ResultTable", command);
#if TRACE_DB
			pt.WriteLogTracer(command);
#endif
			return result;
		}

		/// <summary>
		/// Esegue una query sul database aggiungendo una tabella ad un DataSet esistente.
		/// </summary>
		/// <param name="dataSet">Oggetto DataSet sul quale aggiungere una tabella.</param>
		/// <param name="command">Query da eseguire sul database.</param>
		/// <param name="tableName">Nome della tabella da creare nel DataSet</param>
		/// <returns>true = OK; false = Errore durante l'esecuzione della query o il popolamento del DataSet</returns>
		public bool ExecuteQuery(out DataSet dataSet, string tableName, string command)
		{
			bool result = true; // Presume successo

			dataSet = new DataSet();
			DocsPaCommand docsPaCommand = null;
			DocsPaDataAdapter docsPaDataAdapter = null;		
			
			//Esegui il comando
			try
			{				
				docsPaCommand = new DocsPaCommand(command, conn);
				docsPaCommand.Transaction = transaction;
				docsPaDataAdapter = new DocsPaDataAdapter();
				docsPaDataAdapter.SelectCommand = docsPaCommand;
#if TRACE_DB
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_DB");
#endif
				docsPaDataAdapter.Fill(dataSet, tableName);		
#if TRACE_DB
				pt.WriteLogTracer(command);
#endif
			}
			catch(Exception exception)
			{				
				result = false;
				dataSet = null;
				this.LastException = exception.ToString();
				logger.Debug("Errore ORACLE: " + command, exception);
			}
			finally
			{
				if(docsPaCommand != null)
				{
					docsPaCommand.Dispose();
				}

				if(docsPaDataAdapter != null)
				{
					docsPaDataAdapter.Dispose();
				}
			}
			
			return result;
		}

		/// <summary>
		/// Esegue una query sul database aggiungendo una tabella ad un DataSet esistente.
		/// </summary>
		/// <param name="dataSet">Oggetto DataSet sul quale aggiungere una tabella.</param>
		/// <param name="command">Query da eseguire sul database.</param>
		/// <returns>true = OK; false = Errore durante l'esecuzione della query o il popolamento del DataSet</returns>
		public bool ExecuteQuery(DataSet dataSet, string command)
		{
			bool result = true; // Presume successo
#if TRACE_DB
			DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_DB");
#endif
			result = this.ExecuteQuery(dataSet, "ResultTable", command);
#if TRACE_DB
			pt.WriteLogTracer(command);
#endif

			return result;
		}

		/// <summary>
		/// Esegue una query sul database aggiungendo una tabella ad un DataSet esistente.
		/// </summary>
		/// <param name="dataSet">Oggetto DataSet sul quale aggiungere una tabella.</param>
		/// <param name="command">Query da eseguire sul database.</param>
		/// <param name="tableName">Nome della tabella da creare nel DataSet</param>
		/// <returns>true = OK; false = Errore durante l'esecuzione della query o il popolamento del DataSet</returns>
		public bool ExecuteQuery(DataSet dataSet, string tableName, string command)
		{
			bool result = true; // Presume successo

			DocsPaCommand docsPaCommand = null;
			DocsPaDataAdapter docsPaDataAdapter = null;		
			
			//Esegui il comando
			try
			{				
				docsPaCommand = new DocsPaCommand(command, conn);
				docsPaCommand.Transaction = transaction;
				docsPaDataAdapter = new DocsPaDataAdapter();
				docsPaDataAdapter.SelectCommand = docsPaCommand;
#if TRACE_DB
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_DB");
#endif
				docsPaDataAdapter.Fill(dataSet, tableName);		
#if TRACE_DB		
				pt.WriteLogTracer(command);
#endif
			}
			catch(Exception exception)
			{				
				result = false;
				this.LastException = exception.ToString();
				logger.Debug("Errore ORACLE: " + command, exception);
			}
			finally
			{
				if(docsPaCommand != null)
				{
					docsPaCommand.Dispose();
				}

				if(docsPaDataAdapter != null)
				{
					docsPaDataAdapter.Dispose();
				}
			}
			
			return result;
		}

		/// <summary>
		/// Esegue una query sul database riportando un subset dei record disponibili.
		/// </summary>
		/// <param name="dataSet">Oggetto DataSet da popolare. Se ci sono errori durante 
		/// il caricamento dei dati l'oggetto DataSet ritorna 'null'.</param>		
		/// <param name="startRecord">Posizione dalla quale iniziare a leggere i record</param>
		/// <param name="recordsReturned">Numero di record da leggere</param>
		/// <param name="command">Query da eseguire sul database</param>
		/// <returns>true = OK; false = Errore durnte l'esecuzione della query</returns>
		public bool ExecutePaging(out DataSet dataSet, int startRecord, int recordsReturned, string command)
		{
			bool result = true; // Presume successo

			dataSet = new DataSet();
			DocsPaCommand docsPaCommand = null;
			DocsPaDataAdapter docsPaDataAdapter = null;		
			
			//Esegui il comando
			try
			{		
				docsPaCommand = new DocsPaCommand(command, conn);
				docsPaCommand.Transaction = transaction;
				docsPaDataAdapter = new DocsPaDataAdapter();
				docsPaDataAdapter.SelectCommand = docsPaCommand;
#if TRACE_DB
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_DB");
#endif
				docsPaDataAdapter.Fill(dataSet, startRecord, recordsReturned, "PagingTable");			
#if TRACE_DB
				pt.WriteLogTracer(command);
#endif
			}
			catch(Exception exception)
			{				
				result = false;
				dataSet = null;
				this.LastException = exception.ToString();
				logger.Debug("Errore durante il paging lato server.", exception);
			}
			finally
			{
				if(docsPaCommand != null)
				{
					docsPaCommand.Dispose();
				}

				if(docsPaDataAdapter != null)
				{
					docsPaDataAdapter.Dispose();
				}
			}

			return result;
		}

		/// <summary>Ritorna il numero di record in una data tabella</summary>
		/// <param name="sqlClauses">
		/// Codice SQL contenente la sintassi FROM ed eventuali condizioni/comandi aggiuntivi (WHERE, JOIN, ecc.)
		/// </param>
		/// <returns>
		/// Numero di record della tabella (>= 0) oppure -1 se si è verificata un'eccezione.
		/// </returns>
		public int Count(string sqlClauses)
		{
			int result = -1;
			DocsPaCommand docsPaCommand = null;
			string command = "SELECT COUNT(*) " + sqlClauses;

			// Esegui il comando
			try
			{	
				OpenConnection();
				docsPaCommand = new DocsPaCommand(command, conn);
				docsPaCommand.Transaction = transaction;
#if TRACE_DB
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_DB");
#endif
				DocsPaDataReader docsPaDataReader = docsPaCommand.ExecuteReader();
#if TRACE_DB
				pt.WriteLogTracer(command);
#endif
				if(docsPaDataReader.Read())
				{
					result = docsPaDataReader.GetInt32(0);
				}
			}
			catch(Exception exception)
			{
				result = -1;
				this.LastException = exception.ToString();
                logger.Debug("Errore ORACLE: " + command, exception);
			}
			finally
			{
				if(docsPaCommand != null)
				{
					docsPaCommand.Dispose();
				}

				// Non chiudere la connessione se si è all'interno di una transazione
				if(! TransactionExists)
				{
					CloseConnection();
				}
			}

			return result;
		}

		/// <summary>
		/// Esegue una query sul database riportando un subset dei record disponibili.
		/// </summary>
		/// <param name="dataSet">Oggetto DataSet da popolare. Se ci sono errori durante 
		/// il caricamento dei dati l'oggetto DataSet ritorna 'null'.</param>
		/// <param name="totalPages">Totale delle pagine disponibili.</param>
		/// <param name="totalRecords">Totale recod nella tabella.</param>
		/// <param name="pageId">ID della pagina da estrarre.</param>
		/// <param name="recordsReturned">Numero di record estratti.</param>
		/// <param name="command">Query da eseguire sul database</param>
		/// <param name="tableName">Nome della tabella</param>
		/// <returns>true = OK; false = Errore durnte l'esecuzione della query</returns>
		public bool ExecutePaging(out DataSet dataSet, out int totalPages, out int totalRecords, int pageId, int recordsReturned, string command, string tableName)
		{
			bool result = true; // Presume successo
			totalPages = 0;

			try
			{
				// Acquisizione dei dati
				result = this.ExecutePaging(out dataSet, (recordsReturned * (pageId - 1)), recordsReturned, command);
			
				// Acquisizione numero record e numero pagine
				
				DataSet dsRecCount = new DataSet();
#if TRACE_DB
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_DB");
#endif
				this.ExecuteQuery(dsRecCount,command);
#if TRACE_DB
				pt.WriteLogTracer(command);
#endif
				totalRecords = dsRecCount.Tables[0].Rows.Count;
				dsRecCount.Dispose();
				
				/*string partialCommand = command.Substring(command.LastIndexOf("FROM "));		
				totalRecords = this.Count(partialCommand);*/
				
				
				if(totalRecords < 0)
				{
					throw new Exception();
				}

				/*if (!(totalRecords == 0))
				{
					totalPages = totalRecords / recordsReturned;
				}*/
				//N.B: il codice seguente non è testato in diverse condizioni
				if(totalRecords > 0)
				{
					if ( totalRecords  <= recordsReturned )
					{
						totalPages = 1 ;
					}
					else
					{
						totalPages = totalRecords / recordsReturned;
						if(totalPages * recordsReturned < totalRecords)
						{
							totalPages++;
						}

					}
				
				}
				else
				{
				 
					totalPages = 0 ;   
 
				}

				// Verifica numero di pagine
				/*
				 if(totalPages * recordsReturned < totalRecords)
				{
					totalPages++;
				}
				*/
			}
			catch(Exception exception)
			{				
				result = false;
				dataSet = null;
				totalRecords = -1;
				totalPages   = -1;
				this.LastException = exception.ToString();
                logger.Debug("Errore ORACLE: " + command, exception);
			}

			return result;
		}

		/// <summary>
		/// Esegue un comando sul database. Il metodo non influisce su eventuali transazioni aperte.
		/// </summary>
		/// <param name="command">Comando da eseguire sul database</param>
		/// <returns>true = OK; false = Errore durnte l'esecuzione del comando</returns>
		public bool ExecuteNonQuery(string command)
		{
			int rowsAffected; // Questo parametro verrà ignorato
#if TRACE_DB
			DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_DB");
#endif
			bool result = this.ExecuteNonQuery(command, out rowsAffected);
#if TRACE_DB
			pt.WriteLogTracer(command);
#endif
			return result;
		}

		/// <summary>
		/// Esegue un comando sul database.
		/// </summary>
		/// <param name="command">Comando da eseguire sul database</param>
		/// <param name="rowsAffected">Numero di record aggiunti/modificati/cancellati</param>
		/// <returns>true = OK; false = Errore durnte l'esecuzione del comando</returns>
		public bool ExecuteNonQuery(string command, out int rowsAffected)
		{
			bool result = true; // Presume successo
			rowsAffected = 0; 
			DocsPaCommand docsPaCommand = null;

			//Esegui il comando
			try
			{
				OpenConnection();
				docsPaCommand = new DocsPaCommand(command, conn);
				docsPaCommand.Transaction = transaction;
#if TRACE_DB
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_DB");
#endif
				rowsAffected = docsPaCommand.ExecuteNonQuery();
#if TRACE_DB
				pt.WriteLogTracer(command);
#endif
			}
			catch(Exception exception)
			{
				result = false;
				this.LastException = exception.ToString();
                logger.Debug("Errore ORACLE: " + command, exception);
			}
			finally
			{
				if(docsPaCommand != null)
				{
					docsPaCommand.Dispose();
				}

				// Non chiudere la connessione se si è all'interno di una transazione
				if(! TransactionExists)
				{
					CloseConnection();
				}
			}

			return result;
		}

		/// <summary>
		/// Esegue un comando sul database ritornando il primo campo del primo record.
		/// Il metodo non influisce su eventuali transazioni aperte.
		/// </summary>
		/// <param name="field">Campo restituito in formato stringa</param>
		/// <param name="command">Comando da eseguire sul database</param>
		/// <returns>true = OK; false = Errore durnte l'esecuzione del comando</returns>
		public bool ExecuteScalar(out string field, string command)
		{
			bool result = true; // Presume successo

			DocsPaCommand docsPaCommand = null;
			field = null;

			//Esegui il comando
			try
			{
				OpenConnection();
				docsPaCommand = new DocsPaCommand(command, conn);
				docsPaCommand.Transaction = transaction;
#if TRACE_DB
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_DB");
#endif
				object executeScalarResult = docsPaCommand.ExecuteScalar();
#if TRACE_DB
				pt.WriteLogTracer(command);
#endif
				if(executeScalarResult != null)
				{
					field = executeScalarResult.ToString();
				}
			}
			catch(Exception exception)
			{
				result = false;
				this.LastException = exception.ToString();
                logger.Debug("Errore ORACLE: " + command, exception);
			}
			finally
			{
				if(docsPaCommand != null)
				{
					docsPaCommand.Dispose();
				}

				// Non chiudere la connessione se si è all'interno di una transazione
				if(! TransactionExists)
				{
					CloseConnection();
				}
			}

			return result;
		}		

		/// <summary>
		/// Esegue un comando sul database in maniera esclusiva tramite l'eventuale 
		/// apertura/chiusura di una transazione.
		/// </summary>
		/// <param name="command">Query da eseguire sul database</param>
		/// <returns>true = OK; false = Errore durnte l'esecuzione del comando</returns>
		public bool ExecuteLockedNonQuery(string command)
		{
			bool result = true; // Presume successo
			bool ownTransaction = false; // Presume che la transazione è già esistente	

			try
			{
				//Apri transazione
				if(! TransactionExists)
				{
					ownTransaction = BeginTransaction();
				}

				//Esegui il comando
#if TRACE_DB
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_DB");
#endif
				ExecuteNonQuery(command);
#if TRACE_DB
				pt.WriteLogTracer(command);
#endif

				//Apri transazione
				if(! ownTransaction)
				{
					CommitTransaction();
				}
			}
			catch(Exception exception)
			{
				result = false;
				RollbackTransaction();
				this.LastException = exception.ToString();
                logger.Debug("Errore ORACLE: " + command, exception);
			}
			
			return result;
		}

		/// <summary>
		/// Esegue un comando sul database.
		/// </summary>
		/// <param name="field">System ID della insert effettuata</param>
		/// <param name="command">Comando da eseguire sul database</param>
		/// <param name="tableName">Nome tabella di destinazione</param>
		/// <returns>true = OK; false = Errore durnte l'esecuzione del comando</returns>
		public bool InsertLocked(out string field, string command, string tableName)
		{
			field = null;
			bool result = true; // Presume successo
			bool ownTransaction = false; // Presume che la transazione è già esistente	

			try
			{
				// Apri transazione
				if(! TransactionExists)
				{
					ownTransaction = BeginTransaction();
				}

				// Esegui il comando
#if TRACE_DB
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_DB");
#endif
				ExecuteNonQuery(command);
#if TRACE_DB
				pt.WriteLogTracer(command);
#endif
				// Estrai system ID
#if TRACE_DB
				pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_DB");
#endif
				
				DocsPaDbManagement.Functions.Oracle.Functions f=new DocsPaDbManagement.Functions.Oracle.Functions();
				string sql =f.GetQueryLastSystemIdInserted(tableName);
				
				ExecuteScalar(out field, sql);
			
#if TRACE_DB
				pt.WriteLogTracer("SELECT " + ConfigurationManager.AppSettings["oracleSequenceName"] + ".currval FROM dual");
#endif
				// Chiude transazione
				if(ownTransaction)
				{
					CommitTransaction();
				}
			}
			catch(Exception exception)
			{
				result = false;
				RollbackTransaction();
				this.LastException = exception.ToString();
                logger.Debug("Errore ORACLE: " + command, exception);
			}
			finally
			{
				CloseConnection();
			}
			return result;
		}

		/// <summary>
		/// Ritorna l'ultimo System ID
		/// </summary>
		/// <returns>System ID</returns>
		public string GetNextSystemId()
		{
			// TODO: Questo metodo è relativo ad una sola tabella.
			//       Bisogna modificare il metodo per passare la tabella 
			//       e la chiave come parametri.

			string updateString = "UPDATE DOCS_UNIQUE_KEYS SET LASTKEY  = LASTKEY + 1 WHERE TBNAME = 'SYSTEMKEY'";
#if TRACE_DB
			DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_DB");
#endif
			this.ExecuteLockedNonQuery(updateString);   
#if TRACE_DB
			pt.WriteLogTracer(updateString);
#endif
			string selectString = "SELECT LASTKEY FROM DOCS_UNIQUE_KEYS WHERE TBNAME = 'SYSTEMKEY'";
			string sysID;
#if TRACE_DB
			pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_DB");
#endif
			this.ExecuteScalar(out sysID,selectString);
#if TRACE_DB
			pt.WriteLogTracer(selectString);
#endif
			return sysID;
		}

		public int ExecuteStoreProcedure(string namestoreproc, ArrayList parametri)
		{
			OracleCommand cmd = new OracleCommand();
			cmd.CommandText=namestoreproc;
			cmd.CommandType=CommandType.StoredProcedure;

            bool has_output_params = false;
            List<String> argomentiSP = getSPParams(namestoreproc);
			foreach(ParameterSP par in parametri )
			{
				OracleParameter p = new OracleParameter( par.Nome, par.Valore );
                switch (par.direzione)
                {
                    case DocsPaUtils.Data.DirectionParameter.ParamInput:
                        p.Direction = ParameterDirection.Input;
                        break;

                    case DocsPaUtils.Data.DirectionParameter.ParamOutput:
                        has_output_params = true;
                        p.Direction = ParameterDirection.Output;
                        break;

                    case DocsPaUtils.Data.DirectionParameter.ReturnValue:
                        p.Direction = ParameterDirection.ReturnValue;
                        break;
                }
                if (argomentiSP != null)
                {
                    if (argomentiSP.Contains(par.Nome.ToLower()))
                        cmd.Parameters.Add(p);
                }
                else
                {
                    cmd.Parameters.Add(p);
                }
			}


			OracleParameter parRetValue = new OracleParameter("returnvalue", DbType.Int32);
			parRetValue.Direction=ParameterDirection.Output;
			cmd.Parameters.Add(parRetValue);
			
			try
			{
				this.OpenConnection();
				cmd.Connection=conn;
				cmd.Transaction=transaction;
				cmd.ExecuteNonQuery();

                if (has_output_params)
                {
                    foreach (IDataParameter param in cmd.Parameters)
                    {
                        if (param.Direction == ParameterDirection.Output && param.ParameterName != "returnvalue")
                        {
                            ParameterSP p = ((ParameterSP)parametri[cmd.Parameters.IndexOf(param)]);

                            p.Valore = param.Value;
                        }
                    }
                }
				return (Int32) parRetValue.Value;

			}
			catch(Exception e)
			{
				this.LastException = e.ToString();
				logger.Debug("Errore ORACLE - store procedure: " + namestoreproc, e);	
				return 0;
			}
			finally
			{
				cmd.Dispose();
				if(! TransactionExists)
					CloseConnection();
			}
		}

		public int ExecuteStoredProcedure(string namestoreproc, ArrayList parametri, DataSet ds)
		{
			bool has_output_params = false;

			OpenConnection();
			
			OracleCommand cmd = conn.CreateCommand();
			
			cmd.Transaction=transaction;

			cmd.CommandText = namestoreproc;
			cmd.CommandType = CommandType.StoredProcedure;
			

			// Visto che Oracle è molto pignolo nella definizione dei parametri
			// lasciamo che sia lui a scegliere quelli che gli piacciono di più...
			//			System.Data.OracleClient.OracleCommandBuilder.DeriveParameters (cmd);
			//
			//			for (int n = 0; n < parametri.Count; n++) 
			//			{
			//				if (((ParameterSP) parametri[n]).direzione == DirectionParameter.ParamOutput)
			//					has_output_params = true;
			//				cmd.Parameters[n].Value = ((ParameterSP) parametri[n]).Valore;
			//			}

			string paramName = string.Empty;
            List<String> argomentiSP = getSPParams(namestoreproc);

			foreach(ParameterSP par in parametri )
			{
				paramName = par.Nome;

				if (!paramName.StartsWith("p_"))
					paramName="p_" + par.Nome;

				OracleParameter p = new OracleParameter();
				p.ParameterName = paramName;
				p.Value = par.Valore;
				p.Size = par.Dimensione;
                if (par.Size > 0)
                    p.Size = par.Size;
				p.DbType = par.Tipo;
				switch (par.direzione) 
				{
					case DocsPaUtils.Data.DirectionParameter.ParamInput:
						p.Direction = ParameterDirection.Input;
						break;

					case DocsPaUtils.Data.DirectionParameter.ParamOutput:
						has_output_params = true;
						p.Direction = ParameterDirection.Output;
						break;

					case DocsPaUtils.Data.DirectionParameter.ReturnValue:
						p.Direction = ParameterDirection.ReturnValue;
						break;
				}

                if (argomentiSP != null)
                {
                    if (argomentiSP.Contains(paramName.ToLower()))
                        cmd.Parameters.Add(p);
                }
                else
                {
                    cmd.Parameters.Add(p);
                }
			}


			// Se e solo se il dataset passato è diverso da NULL,
			// viene aggiunto al volo un parametro di tipo cursore
			// denominato "p_res_cursor" che la SP per convenzione
			// *deve* restituire. Il DataAdapter per Oracle intercetta 
			// la presenza di un parametro di tipo REF CURSOR OUT e riempie
			// il dataset con i dati eventualmente restituiti dalla SP.
			// Se il dataset passato è nullo non ci sono dati da restituire
			// ma comunque ne creiamo uno al volo per evitare problemi sulla
			// chiamata al metodo Fill del DataAdapter
			if (ds != null) 
			{
#if USE_ODP
				OracleParameter p = new OracleParameter("p_res_cursor", OracleDbType.RefCursor);
#else
				OracleParameter p = new OracleParameter("p_res_cursor", System.Data.OracleClient.OracleType.Cursor);
#endif
				p.Direction = ParameterDirection.Output;
				cmd.Parameters.Add(p);
			}
			else
				ds = new DataSet();

			try
			{
				OracleDataAdapter da = new OracleDataAdapter(cmd);
				da.Fill (ds);

				if (has_output_params) 
				{
					for (int n = 0; n < cmd.Parameters.Count; n++) 
					{
						OracleParameter pp = cmd.Parameters[n];
						if (pp.Direction == ParameterDirection.Output) 
						{
							((ParameterSP) parametri[n]).Valore = pp.Value;
							//							foreach (ParameterSP psp in parametri) 
							//							{
							//								if ((psp.Nome == pp.ParameterName) || (("p_" + psp.Nome) == pp.ParameterName))
							//								{
							//									psp.Valore = pp.Value;
							//									break;
							//								}
							//							}
						}
					}
				}
				return 1;

			}
			catch(Exception e)
			{
				this.LastException = e.ToString();
				logger.Debug("Errore Oracle - stored procedure: " + namestoreproc, e);	
				return 0;
			}
			finally
			{
				cmd.Dispose();
				if(!TransactionExists)
					CloseConnection();
			}
			return 1;
		}

		public string GetLargeText(string tableName, string systemId, string columnName)
		{
			string val = null;
            bool connOpen = false;
			try 
			{
				string qry = "select " + columnName + " " +
					"from " + tableName + " " +
					"where system_id=" + systemId;
                if (!conn.State.ToString().Equals("Open"))
                {
                    conn.Open();
                    connOpen = true;
                }
				System.Data.OracleClient.OracleCommand sqlCommandOracle =
					new System.Data.OracleClient.OracleCommand(qry,conn);     
				sqlCommandOracle.Transaction = transaction;    
				System.Data.OracleClient.OracleDataReader rstOracle = 
					sqlCommandOracle.ExecuteReader();                             
				rstOracle.Read();                              
				System.Data.OracleClient.OracleLob clob = rstOracle.GetOracleLob(0);      
                
				byte[] buffer = new byte[clob.Length];
				clob.Read(buffer,0,(int)clob.Length);
				val = System.Text.Encoding.Unicode.GetString(buffer);             
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				try { if(connOpen) conn.Close(); } 
				catch {}
			}
			return val;
		}

		public bool SetLargeText(string tableName, string systemId, string columnName, string val)
		{
			try 
			{
				string qry = "select " + columnName + " " +
					"from " + tableName + " " +
					"where system_id=" + systemId + " FOR UPDATE";

				if(conn.State.ToString().Equals("Open"))                     
				{                       
					byte[] newvalue = System.Text.Encoding.Unicode.GetBytes(val);                             
					System.Data.OracleClient.OracleCommand sqlCommandOracle =
						new System.Data.OracleClient.OracleCommand(qry,conn);     
                    sqlCommandOracle.Transaction = transaction;    
					System.Data.OracleClient.OracleDataReader rstOracle = 
						sqlCommandOracle.ExecuteReader();                             
					rstOracle.Read();                              
					System.Data.OracleClient.OracleLob clob = rstOracle.GetOracleLob(0);
                    clob.Erase();
					clob.Write(newvalue, 0, newvalue.Length);
				}      
                else return false;       

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

        public string GetSysdate()
        {
            string val = null;
            bool connOpen = false;
            try
            {
                string qry = "select sysdate from dual";
                if (!conn.State.ToString().Equals("Open"))
                {
                    conn.Open();
                    connOpen = true;
                }
                System.Data.OracleClient.OracleCommand sqlCommandOracle =
                    new System.Data.OracleClient.OracleCommand(qry, conn);
                sqlCommandOracle.Transaction = transaction;
                System.Data.OracleClient.OracleDataReader rstOracle =
                    sqlCommandOracle.ExecuteReader();
                rstOracle.Read();
                System.DateTime dt = rstOracle.GetDateTime(0);
                return dt.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                try { if (connOpen) conn.Close(); }
                catch { }
            }
            return val;
        }

		public IDataReader ExecuteReader (string qry)
		{
			DocsPaCommand cmd = null;
			try 
			{
				this.OpenConnection();

				cmd = new DocsPaCommand (qry, conn);
				cmd.Transaction = transaction;

				return cmd.ExecuteReader();
			}
			catch
			{
				return null;
			}
			finally 
			{
				if (cmd != null)
					cmd.Dispose();
				
			}
		}

		#endregion

		#region Connection Management
        /// <summary>
        /// Apre la connessione.
        /// </summary>
        /// <returns>true = Connessione aperta; false = Errore durnte l'apertura della connessione</returns>
        public bool OpenConnection()
        {
            bool result = true;

            if (!ConnectionExists)
            {
                if (TransactionContext.HasTransactionalContext)
                {
                    // Se esiste un contesto transazionale, viene fatto l'acquire
                    // della transaction
                    this.AcquireTransaction((OracleTransaction)((OracleTransactionContext)TransactionContext.Current).GetCurrentTransaction());
                }
                else
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception exception)
                    {
                        // Impossibilie aprire la connesione
                        result = false;
                        this.LastException = exception.ToString();
                        logger.Error("ERRORE: impossibile aprire la connessione con il DataBase, controllare i parametri inseriti nella stringa di connessione." + exception);
                  
                    }
                }
            }

            return result;
        }
	
		/// <summary>
		/// Chiude la connessione se non siamo all'interno di una transazione. Ciò è possibile solo se la 
		/// connessione è stata creata in questa istanza (vedi proprietà IsOwnConnection).
		/// </summary>
		/// <returns>true = OK; false = Connessione già chiusa o errore durante la chiusura della connessione</returns>
		public bool CloseConnection()
		{
			bool result = true;

		//	conn.Close();

		//	return true;

			if(isOwnConn && ConnectionExists)
			{
				if(! TransactionExists)
				{
					conn.Close();
					
				}
			}
			else
			{
				result = false;
			}

			return result;
		}

        /// <summary>
        /// Acquisisce una transazione/connessione già instanziata. In questa istanza viene inibita la possibilità 
        /// di chiudere la transazione/connessione (da utilizzare quando la transazione è già stata 
        /// aperta dall'oggetto che passa la connessione). 
        /// </summary>
        /// <param name="oracleTransaction">Transazione istanziata</param>
        /// <returns>true = OK; false = Una transazione/connessione è già stata acquisita o errore durante 
        /// l'acquisizione della connessione</returns>
        protected bool AcquireTransaction(DocsPaTransaction oracleTransaction)
        {
            bool result = true; // Presume successo

            /* Non si sta già usando una connessione acquisità quindi si può
             * procedere con l'eliminazione di un'eventuale connessione aperta
             * dal costruttore e con l'acquisizione della connessione passata 
             * come parametro.
             */
            try
            {
                // Elimina eventuali connessioni aperte
                if (conn != null)
                    conn.Dispose();
                
                // Acquisisci connessione esistente
                conn = oracleTransaction.Connection;
                transaction = oracleTransaction;
                isOwnConn = false;
            }
            catch (Exception exception)
            {
                // Impossibile acquisire una transazione
                result = false;
                this.LastException = exception.ToString();
                logger.Debug("Errore nell'acquisizione della transazione ORACLE.", exception);
            }

            return result;
        }

		#endregion

		#region Dispose Method

        /// <summary>
        /// Questo metodo è utilizzato per deallocare oggetti.
        /// </summary>
        public void Dispose()
        {
            if (!TransactionContext.HasTransactionalContext)
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }

                if (conn != null)
                {
                    conn.Dispose();
                }
            }
        }

		#endregion

		/// <summary>
		/// Se questo metodo viene implementato nella classe 
		/// DocsPaDbManagement.Database.Database creata per uno specifico DB Server,
		/// il costruttore statico presente nelle classi in DocsPaDB.Query_DocsPaWS
		/// lo chiamera'. Questo metodo deve verificare la presenza ed eventualmente
		/// creare le SP necessarie al funzionamento delle classi chiamanti
		/// </summary>
		/// <param name="sp_id">L'Id della classe chiamante per cui creare le SP</param>
		/// <returns></returns>
		public bool CheckStoredProcedures (string sp_id)
		{
			bool res = false;
			if (sp_id == "rubrica") 
			{
				check_types ();
				res |= check_sp ("dpa3_get_children");
				res |= check_sp ("dpa3_get_hierarchy");
			}
			return res;
		}

		private void check_types()
		{
			try 
			{
				//				this.Connection.Open();
				//				DocsPaCommand cmd = this.Connection.CreateCommand();
				//				cmd.CommandType = CommandType.Text;
				//				cmd.CommandText = "CREATE OR REPLACE PACKAGE pkgRubrica as TYPE r_cursor is REF CURSOR; end;";
				//				cmd.ExecuteNonQuery();
				//				cmd.CommandText = "ALTER PACKAGE pkgRubrica COMPILE";
				//				cmd.ExecuteNonQuery();

			}
			catch (Exception ex)
			{
				logger.Debug ("Errore nella creazione dei tipi " + ex.Message);
			}
			finally 
			{
				this.Connection.Close();
			}
		}

		private bool check_sp (string sp_name)
		{		
			return false;
			try 
			{
				this.Connection.Open();
				DocsPaCommand cmd = this.Connection.CreateCommand();
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "select count(*) from all_objects where object_type='PROCEDURE' and upper(object_name)=upper('" + sp_name + "')";
				bool exists = (Convert.ToInt64 (cmd.ExecuteScalar()) == 1);
				if (!exists) 
				{
					Stream s = get_resource_as_stream (sp_name);
					StreamReader sr = new StreamReader(s);
					cmd.CommandText = sr.ReadToEnd();
					sr.Close();
					cmd.ExecuteNonQuery();
					cmd.CommandText = String.Format ("ALTER PROCEDURE {0} COMPILE", sp_name);
					cmd.ExecuteNonQuery();
				}
				logger.Debug (String.Format ("Creazione della SP \"{0}\" eseguita con successo", sp_name));
				return !exists;
			}
			catch (Exception ex)
			{
				logger.Debug (String.Format ("Errore nella creazione della SP \"{0}\" ({1})", sp_name, ex.Message));
				return false;
			}
			finally 
			{
				this.Connection.Close();
			}
		}

		private Stream get_resource_as_stream (string filename)
		{
			Assembly theAssembly = Assembly.GetExecutingAssembly();
			string rsrc_name = "DocsPaDbManagement.StoredProc." + filename + ".sql";
			string[] rsrcs = theAssembly.GetManifestResourceNames();
			Array.Sort (rsrcs , CaseInsensitiveComparer.Default);
			int pos = Array.BinarySearch (rsrcs, rsrc_name, CaseInsensitiveComparer.Default);
				
			return (pos >= 0) ? theAssembly.GetManifestResourceStream(rsrcs[pos]) : null;
		}


        private List<String> getSPParams(string sp_name)
        {


            //gestione dei package
            string  sp_name1 = Path.GetExtension(sp_name);

            if (sp_name1.StartsWith("."))
                sp_name = sp_name1.Substring(1);

            if (spParamCacheCollection.ContainsKey(sp_name))
                return spParamCacheCollection[sp_name];


            List<String> retval = new List<string>();
            try
            {
                DataSet ds;
                string commandText = String.Format("select argument_name, in_out, data_type from user_arguments where lower(object_name) = lower('{0}') order by position", sp_name.ToUpperInvariant());
                this.ExecuteQuery(out ds,"PARAMETRI",commandText);
                foreach (DataRow parametriRow in ds.Tables["PARAMETRI"].Rows)
                {
                    retval.Add(parametriRow["argument_name"].ToString().ToLower());
                }

            }
            catch (Exception ex)
            {
                logger.Debug(String.Format("Errore nella query della SP \"{0}\" ({1})", sp_name, ex.Message));
                return null;
            }

            //aggiungo alla colletcion di cache
            try
            {
                spParamCacheCollection.Add(sp_name, retval);
            }
            catch (Exception ex)
            {
                //Gestione errore per spedizioni IS Massive (An item with the same key has already been added)
                logger.Error("Errore in AddsParm SP:" + ex.Message);
            }
            return retval;
        }


	}
}
