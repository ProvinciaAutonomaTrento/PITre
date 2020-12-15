using System;
using System.Configuration;
using System.Data;
using Microsoft.Data.Odbc;
using log4net;

namespace DocsPaAS400
{
	public class AS400Database :DocsPaDB.DBProvider
	{
		public static OdbcConnection conn;
        private ILog logger = LogManager.GetLogger(typeof(AS400Database));

		public AS400Database() 
		{
			string connectionString =   ConfigurationManager.AppSettings[Constants.AS400_CONNECTION_STRING_PARAM_NAME];
			conn = new OdbcConnection(connectionString);
		}
		public static AS400Database getInstance()
		{
           return new AS400Database();
		}

		public  void  openConnection() 
		{
			logger.Debug("SqlServerAgent");
			if(conn.State.Equals(System.Data.ConnectionState.Closed))
				conn.Open(); 
		OdbcCommand 	cmd = new OdbcCommand();
			cmd = conn.CreateCommand();
			cmd.CommandTimeout = 30;
			logger.Debug("Connessione creata");
		}

		protected    Microsoft.Data.Odbc.OdbcDataAdapter getDataAdapter(string queryString) 
		{	
			Microsoft.Data.Odbc.OdbcDataAdapter da=null;
			Microsoft.Data.Odbc.OdbcTransaction tx=null;
			OdbcCommand command = new OdbcCommand(queryString, (OdbcConnection)conn);
			if(tx != null)
				command.Transaction = (OdbcTransaction)tx;
			if(da == null) 
				da = new OdbcDataAdapter(command);
				
			else
				da.SelectCommand = command;
			return da;
		}
	
		public   System.Data.IDataReader executeReader(string sqlString) 
		{
			openConnection();
			OdbcCommand cmd = new OdbcCommand();
			cmd = conn.CreateCommand();
			cmd.CommandText = sqlString;
			return cmd.ExecuteReader();
		}
		
		public   Object executeScalar(string sqlString) 
		{
			openConnection();
			OdbcCommand cmd = new OdbcCommand();
			cmd = conn.CreateCommand();
			cmd.CommandText = sqlString;
			return cmd.ExecuteScalar();
		}
	}
}
