using System;

namespace DocsPaDB.Diagnostics
{
	/// <summary>
	/// Classe che espone metodi per la diagnostica dell'interazione DocsPa/Database
	/// </summary>
	public class Database : DBProvider
	{
		public bool OpenConnectionCheck()
		{
			bool result = true;

			try
			{
				this.OpenConnection();
				result = this.ConnectionExists;
			}
			catch(Exception)
			{
				result = false;
			}

			return result;
		}

		public bool DPA_AmministraCheck()
		{
			bool result = true;

			try
			{
				System.Data.DataSet dataSet = new System.Data.DataSet();
				string command = "SELECT * FROM DPA_Amministra";
				result = this.ExecuteQuery(dataSet, command);

				if(dataSet == null || dataSet.Tables.Count == 0)
				{
					result = false;
				}
			}
			catch(Exception)
			{
				result = false;
			}

			return result;
		}
	}
}
