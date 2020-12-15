using System;
using PCDCLIENTLib;
using log4net;

namespace DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib
{
	/// <summary>
	/// Questa classe gestisce tutti gli accessi a basso livello 
	/// all'oggetto PCDLogin della libreria Hummingbird
	/// </summary>
	public class Login : DocsPaUtils.Interfaces.FileManagement.IGestioneErrori
	{
        private ILog logger = LogManager.GetLogger(typeof(Login));
		// Istanza dell'oggetto login
		protected PCDLogin login;

		#region Costruttori
		/// <summary>
		/// Questo costruttore instanzia un oggetto login sul documentale
		/// </summary>
		/// <param name="networkType"></param>
		/// <param name="unitName"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		public Login(short networkType, string unitName, string userName, string password) 
		{
			login = new PCDLogin();
			login.AddLogin(networkType, unitName, userName, password);
		}	
		#endregion

		#region Gestione errori
		/// <summary>
		/// Ritorna l'ultimo codice di errore.
		/// </summary>
		/// <returns>
		/// Codice errore dell'ultima operazione (0 = nessun errore)
		/// </returns>
		public int GetErrorCode()
		{
			int result = GetErrorCode(null);

			return result;
		}

		/// <summary>
		/// Ritorna l'ultimo codice di errore.
		/// </summary>
		/// <param name="customDescription">Eventuale descrizione custom.</param>
		/// <returns>
		/// Codice errore dell'ultima operazione (0 = nessun errore)
		/// </returns>
		public int GetErrorCode(string customDescription)
		{
			int result = 0; // Presume successo

			result = login.ErrNumber; 

			if(result != 0)
			{
				if(customDescription != null)
				{
					logger.Debug(customDescription);
				}

				logger.Debug("PCD Error Number: " + login.ErrNumber);
				logger.Debug("PCD Error Description: " + login.ErrDescription);
			}

			return result;
		}
		#endregion

		#region Proprietà
		/// <summary>
		/// Restituisce l'istanza dell'oggetto login
		/// </summary>
		protected PCDLogin CurrentInstance
		{
			get
			{
				return this.login;
			}
		}
		#endregion

		#region Metodi
		/// <summary>
		/// </summary>
		public int Execute()
		{
			return login.Execute();
		}

		/// <summary>
		/// </summary>
		public string GetDST()
		{
			return login.GetDST();
		}
		#endregion
	}
}
