using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using DocsPaUtils.Interfaces.DbManagement;

namespace DocsPaDB
{
	public class DBProvider : IDatabase
	{
		#region Variabili di classe

		protected string dbType="";
		protected string connectionString="";
		protected IDatabase databaseProvider=null;

		#endregion

		#region Costruttori

		public DBProvider()
		{
            dbType = ConfigurationManager.AppSettings["dbType"].ToUpper();
            connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();

			switch(dbType)
			{
				case "SQL":
					databaseProvider=new DocsPaDbManagement.Database.SqlServer.Database(connectionString);
					break;
				case "ORACLE":
					databaseProvider=new DocsPaDbManagement.Database.Oracle.Database(connectionString);
					break;
			}
		}

		#endregion

		#region IDatabase members

		public void InstantiateConnection(string dbConnectionString)
		{
			databaseProvider.InstantiateConnection(connectionString);
		}

		public bool BeginTransaction()
		{
			return databaseProvider.BeginTransaction();
		}

		public bool BeginTransaction(IsolationLevel isolationLevel)
		{
			return databaseProvider.BeginTransaction(isolationLevel);
		}

		public bool CommitTransaction()
		{
			return databaseProvider.CommitTransaction();
		}

		public bool RollbackTransaction()
		{
			return databaseProvider.RollbackTransaction();
		}

		public bool ExecuteQuery(out DataSet dataSet, string command)
		{
			return databaseProvider.ExecuteQuery(out dataSet, command);
		}

		public bool ExecuteQuery(out DataSet dataSet, string tableName, string command)
		{
			return databaseProvider.ExecuteQuery(out dataSet, tableName, command);
		}

		public bool ExecuteQuery(DataSet dataSet, string command)
		{
			return databaseProvider.ExecuteQuery(dataSet, command);
		}

		public bool ExecuteQuery(DataSet dataSet, string tableName, string command)
		{
			return databaseProvider.ExecuteQuery(dataSet, tableName, command);
		}

		public bool ExecutePaging(out DataSet dataSet, int startRecord, int recordsReturned, string command)
		{
			return databaseProvider.ExecutePaging(out dataSet, startRecord, recordsReturned, command);
		}

		public bool ExecutePaging(out DataSet dataSet, out int totalPages, out int totalRecords, int pageId, int recordsReturned, string command, string tableName)
		{
			return databaseProvider.ExecutePaging(out dataSet, out totalPages, out totalRecords, pageId, recordsReturned, command, tableName);
		}

		public int Count(string sqlClauses)
		{
			return databaseProvider.Count(sqlClauses);
		}

		public bool ExecuteNonQuery(string command)
		{
			return databaseProvider.ExecuteNonQuery(command);
		}

		public bool ExecuteNonQuery(string command, out int rowsAffected)
		{
			return databaseProvider.ExecuteNonQuery(command, out rowsAffected);
		}

		public bool ExecuteScalar(out string field, string command)
		{
			return databaseProvider.ExecuteScalar(out field, command);
		}

		public bool ExecuteLockedNonQuery(string command)
		{
			return databaseProvider.ExecuteLockedNonQuery(command);
		}

		public bool InsertLocked(out string field, string command, string tableName)
		{
			return databaseProvider.InsertLocked(out field, command, tableName);
		}

		public string GetNextSystemId()
		{
			return databaseProvider.GetNextSystemId();
		}

		public bool OpenConnection()
		{
			return databaseProvider.OpenConnection();
		}

		public bool CloseConnection()
		{
			return databaseProvider.CloseConnection();
		}

		public void Dispose()
		{
			databaseProvider.Dispose();
		}

		public int ExecuteStoreProcedure(string namestoreproc, ArrayList parametri)
		{
			return databaseProvider.ExecuteStoreProcedure(namestoreproc, parametri);
		}

		public int ExecuteStoredProcedure(string namestoreproc, ArrayList parametri, DataSet ds)
		{
			return databaseProvider.ExecuteStoredProcedure(namestoreproc, parametri, ds);
		}

		public string LastExceptionMessage
		{
			get {return databaseProvider.LastExceptionMessage;}
		}

		public bool ConnectionExists
		{
			get {return databaseProvider.ConnectionExists;}
		}

		public string DBType
		{
			get
			{
				return databaseProvider.DBType;
			}
		}

		public string GetLargeText(string tableName, string systemId, string columnName)
		{
			return databaseProvider.GetLargeText(tableName,systemId,columnName);
		}

		public bool SetLargeText(string tableName, string systemId, string columnName, string val)
		{
			return databaseProvider.SetLargeText(tableName,systemId,columnName,val);
		}

        public string GetSysdate()
        {
            return databaseProvider.GetSysdate();
        }

		public IDataReader ExecuteReader (string qry)
		{
			return databaseProvider.ExecuteReader(qry);
		}

		#endregion
	}
}
