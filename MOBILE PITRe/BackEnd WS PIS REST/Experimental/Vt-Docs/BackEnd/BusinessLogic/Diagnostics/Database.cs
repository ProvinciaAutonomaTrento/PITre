using System;

namespace BusinessLogic.Diagnostics
{
	/// <summary>
	/// Classe per la diagnostica dell'interazione DocsPa/Database
	/// </summary>
	public class Database
	{
		/// <summary>
		/// Controlla il funzionamento della connessione alla tabella DPA_Amministra
		/// </summary>
		/// <returns></returns>
		public bool CheckDPA_Amministra(out string exceptionMessage)
		{
			bool result = true; // Presume successo
			exceptionMessage = null;

			try
			{	
				DocsPaDB.Diagnostics.Database database = new DocsPaDB.Diagnostics.Database();

				result = database.DPA_AmministraCheck();

				if(!result)
				{
					exceptionMessage = database.LastExceptionMessage;
					
					if(exceptionMessage == null)
					{
						exceptionMessage = "No table returned.";
					}
				}
			}
			catch(Exception)
			{
				result = false;
			}

			return result;
		}

		/// <summary>
		/// Controlla il funzionamento della connessione al database
		/// </summary>
		/// <returns></returns>
		public bool CheckConnectionOpening(out string exceptionMessage)
		{
			bool result = true; // Presume successo
			exceptionMessage = null;

			try
			{	
				DocsPaDB.Diagnostics.Database database = new DocsPaDB.Diagnostics.Database();

				result = database.OpenConnectionCheck();

				if(!result)
				{
					exceptionMessage = database.LastExceptionMessage;
				}
			}
			catch(Exception)
			{
				result = false;
			}

			return result;
		}

		/// <summary>
		/// Controlla il funzionamento della connessione al database
		/// </summary>
		/// <returns></returns>
		public bool CheckConnectionString(out string exceptionMessage)
		{
			bool result = true; // Presume successo
			exceptionMessage = null;

			try
			{				
				DocsPaDB.Diagnostics.Database database = new DocsPaDB.Diagnostics.Database();
				exceptionMessage = database.LastExceptionMessage;

				if(exceptionMessage != null && exceptionMessage != "")
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
