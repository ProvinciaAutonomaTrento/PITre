using System;
using PCDCLIENTLib;
using log4net;

namespace DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib
{
	/// <summary>
	/// Questa classe gestisce tutti gli accessi a basso livello 
	/// all'oggetto PCDSearch della libreria Hummingbird
	/// </summary>
	public class Ricerca : DocsPaUtils.Interfaces.FileManagement.IGestioneErrori
	{
        private ILog logger = LogManager.GetLogger(typeof(Ricerca));
		// Istanza dell'oggetto ricerca
		protected PCDSearch search;

		#region Costruttori
		/// <summary>
		/// Questo costruttore instanzia un oggetto di ricerca sul documentale
		/// </summary>
		/// <param name="dst"></param>
		/// <param name="library"></param>
		/// <param name="searchObject"></param>
		public Ricerca(string dst, string library, string searchObject) 
		{
			// Create a search object.
			search = new PCDCLIENTLib.PCDSearch();

			// Set the Security Token.
			search.SetDST(dst);

			// Add the library that was passed in.
			search.AddSearchLib(library);

			// Use the versions form to access properties.
			search.SetSearchObject(searchObject);
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

			result = search.ErrNumber; 

			if(result != 0)
			{
				if(customDescription != null)
				{
					logger.Debug(customDescription);
				}

				logger.Debug("PCD Error Number: " + search.ErrNumber);
				logger.Debug("PCD Error Description: " + search.ErrDescription);
			}

			return result;
		}
		#endregion

		#region Proprietà
		/// <summary>
		/// Restituisce l'istanza dell'oggetto search
		/// </summary>
		protected PCDSearch CurrentInstance
		{
			get
			{
				return this.search;
			}
		}

		/// <summary>
		/// </summary>
		public string VersionId
		{
			get
			{
				return search.GetPropertyValue("VERSION_ID").ToString();
			}
		}
		#endregion

		#region Metodi
		/// <summary>
		/// </summary>
		public void AddReturnProperty(string returnProperty)
		{
			search.AddReturnProperty(returnProperty);
		}

		/// <summary>
		/// </summary>
		public void AddSearchCriteria(string propertyName, string criteria)
		{
			search.AddSearchCriteria(propertyName, criteria);
		}

		/// <summary>
		/// </summary>
		public void AddOrderByProperty(string propertyName, bool ascending)
		{
			int ascendingValue;

			if(ascending)
			{
				ascendingValue = 1;				
			}
			else
			{
				ascendingValue = 0;
			}

			search.AddOrderByProperty(propertyName, ascendingValue);
		}

		/// <summary>
		/// </summary>
		public int Execute()
		{
			return search.Execute();
		}

		/// <summary>
		/// </summary>
		public int NextRow()
		{
			return search.NextRow();
		}

		/// <summary>
		/// </summary>
		public int ReleaseResults()
		{
			return search.ReleaseResults();
		}
		#endregion
	}
}
