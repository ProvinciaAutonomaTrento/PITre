using System;
using System.Data;
using System.Collections;

namespace DocsPaUtils.Interfaces.DbManagement
{
	/// <summary>
	/// Interfaccia per l'integrazione con i DB.
	/// </summary>
	public interface IDatabase : IDisposable
	{
		/// <summary>
		/// Instanzia una connessione.
		/// </summary>
		void InstantiateConnection(string dbConnectionString);

		/// <summary>
		/// Inizia una transazione sulla connessione.
		/// </summary>
		/// <returns>true = OK; false = Transazione non avviata</returns>
		bool BeginTransaction();

		bool BeginTransaction(IsolationLevel isolationLevel);

		/// <summary>
		/// Chiudi la transazione sulla connessione.
		/// </summary>
		/// <returns>true = OK; false = Errore in chiusura transazione</returns>
		bool CommitTransaction();

		/// <summary>
		/// Annulla una transazione sulla connessione.
		/// </summary>
		/// <returns>true = OK; false = Errore nell'annullamento della transazione</returns>
		bool RollbackTransaction();

		/// <summary>
		/// Esegue una query sul database.
		/// </summary>
		/// <param name="dataSet">Oggetto DataSet da popolare. Se ci sono errori durante 
		/// il caricamento dei dati l'oggetto DataSet ritorna 'null'.</param>
		/// <param name="command">Query da eseguire sul database</param>
		/// <returns>true = OK; false = Errore durnte l'esecuzione della query</returns>
		bool ExecuteQuery(out DataSet dataSet, string command);

		/// <summary>
		/// Esegue una query sul database.
		/// </summary>
		/// <param name="dataSet">Oggetto DataSet da popolare. Se ci sono errori durante 
		/// il caricamento dei dati l'oggetto DataSet ritorna 'null'.</param>
		/// <param name="command">Query da eseguire sul database</param>
		/// <param name="tableName">Nome della tabella da creare nel DataSet</param>
		/// <returns>true = OK; false = Errore durante l'esecuzione della query</returns>
		bool ExecuteQuery(out DataSet dataSet, string tableName, string command);

		/// <summary>
		/// Esegue una query sul database aggiungendo una tabella ad un DataSet esistente.
		/// </summary>
		/// <param name="dataSet">Oggetto DataSet sul quale aggiungere una tabella.</param>
		/// <param name="command">Query da eseguire sul database</param>
		/// <returns>true = OK; false = Errore durante l'esecuzione della query o il popolamento del DataSet</returns>
		bool ExecuteQuery(DataSet dataSet, string command);

		/// <summary>
		/// Esegue una query sul database aggiungendo una tabella ad un DataSet esistente.
		/// </summary>
		/// <param name="dataSet">Oggetto DataSet sul quale aggiungere una tabella.</param>
		/// <param name="command">Query da eseguire sul database</param>
		/// <param name="tableName">Nome della tabella da creare nel DataSet</param>
		/// <returns>true = OK; false = Errore durante l'esecuzione della query o il popolamento del DataSet</returns>
		bool ExecuteQuery(DataSet dataSet, string tableName, string command);

		/// <summary>
		/// Esegue una query sul database.
		/// </summary>
		/// <param name="dataSet">Oggetto DataSet da popolare. Se ci sono errori durante 
		/// il caricamento dei dati l'oggetto DataSet ritorna 'null'.</param>		
		/// <param name="startRecord">Posizione dalla quale iniziare a leggere i record</param>
		/// <param name="recordsReturned">Numero di record da leggere</param>
		/// <param name="command">Query da eseguire sul database</param>
		/// <returns>true = OK; false = Errore durante l'esecuzione della query</returns>
		bool ExecutePaging(out DataSet dataSet, int startRecord, int recordsReturned, string command);

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
		bool ExecutePaging(out DataSet dataSet, out int totalPages, out int totalRecords, int pageId, int recordsReturned, string command, string tableName);

		/// <summary>Ritorna il numero di record in una data tabella</summary>
		/// <param name="sqlClauses">
		/// Codice SQL contenente la sintassi FROM ed eventuali condizioni/comandi aggiuntivi (WHERE, JOIN, ecc.)
		/// </param>
		/// <returns>
		/// Numero di record della tabella (>= 0) oppure -1 se si è verificata un'eccezione.
		/// </returns>
		int Count(string sqlClauses);

		/// <summary>
		/// Esegue un comando sul database.
		/// </summary>
		/// <param name="command">Comando da eseguire sul database</param>
		/// <returns>true = OK; false = Errore durante l'esecuzione del comando</returns>
		bool ExecuteNonQuery(string command);

		/// <summary>
		/// Esegue un comando sul database.
		/// </summary>
		/// <param name="command">Comando da eseguire sul database</param>
		/// <param name="rowsAffected">Numero di record aggiunti/modificati/cancellati</param>
		/// <returns>true = OK; false = Errore durnte l'esecuzione del comando</returns>
		bool ExecuteNonQuery(string command, out int rowsAffected);

		/// <summary>
		/// Esegue un comando sul database ritornando il primo campo del primo record.
		/// </summary>
		/// <param name="field">Campo restituito in formato stringa</param>
		/// <param name="command">Comando da eseguire sul database</param>
		/// <returns>true = OK; false = Errore durante l'esecuzione del comando</returns>
		bool ExecuteScalar(out string field, string command);
	
		/// <summary>
		/// Esegue un comando sul database in maniera esclusiva tramite l'eventuale 
		/// apertura/chiusura di una transazione.
		/// </summary>
		/// <param name="command">Query da eseguire sul database</param>
		/// <returns>true = OK; false = Errore durnte l'esecuzione della query</returns>
		bool ExecuteLockedNonQuery(string command);
		
		/// <summary>
		/// Esegue un comando sul database.
		/// </summary>
		/// <param name="field">System ID della insert effettuata</param>
		/// <param name="command">Comando da eseguire sul database</param>
		/// <param name="tableName">Nome tabella di destinazione</param>
		/// <returns>true = OK; false = Errore durnte l'esecuzione del comando</returns>
		bool InsertLocked(out string field, string command, string tableName);
		
		/// <summary>
		/// Ritorna l'ultimo System ID
		/// </summary>
		/// <returns>System ID</returns>
		string GetNextSystemId();

		/// <summary>
		/// Apre la connessione.
		/// </summary>
		/// <returns>true = OK; false = Errore durante l'apertura della connessione</returns>
		bool OpenConnection();

		/// <summary>
		/// Chiude la connessione.
		/// </summary>
		/// <returns>true = OK; false = Errore durnte la chiusura della connessione</returns>
		bool CloseConnection();

		/// <summary>
		/// Questo metodo è utilizzato per deallocare oggetti.
		/// </summary>
		void Dispose();
	
		/// <summary>
		/// Esegue una store procedure con n parametri di input
		/// </summary>
		/// <param name="namestoreproc"></param> 
		/// <param name="parametri"></param>
		/// <returns>
		/// Valore intero di ritorno della store procedure 
		///</returns>
		int ExecuteStoreProcedure(string namestoreproc, ArrayList parametri);

		/// <summary>
		/// Esegue una store procedure con n parametri di input e restituisce un DataSet con
		/// i dati raccolti dalla SP
		/// </summary>
		/// <param name="namestoreproc"></param>
		/// <param name="parametri"></param>
		/// <param name="ds"></param>
		/// <returns></returns>
		int ExecuteStoredProcedure(string namestoreproc, ArrayList parametri, DataSet ds);

		string LastExceptionMessage {get;}

		bool ConnectionExists {get;}
		string GetLargeText(string tableName, string systemId, string columnName);
		bool SetLargeText(string tableName, string systemId, string columnName, string val);
        string GetSysdate();

		IDataReader ExecuteReader (string qry);

		string DBType { get; }
	}
}

