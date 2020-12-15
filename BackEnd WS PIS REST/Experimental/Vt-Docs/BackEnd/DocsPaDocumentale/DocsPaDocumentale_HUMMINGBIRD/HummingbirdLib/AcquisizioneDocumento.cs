using System;
using PCDCLIENTLib;
using log4net;

namespace DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib
{
	/// <summary>
	/// Questa classe gestisce tutti gli accessi a basso livello 
	/// all'oggetto PCDGetDoc della libreria Hummingbird
	/// </summary>
	public class AcquisizioneDocumento : DocsPaUtils.Interfaces.FileManagement.IGestioneErrori
	{
        private ILog logger = LogManager.GetLogger(typeof(AcquisizioneDocumento));
		// Istanza del documento
		protected PCDGetDoc getDocObject;

		#region Costruttori
		/// <summary>
		/// Questo costruttore instanzia un oggetto per l'acquisizione di un documento sul documentale
		/// </summary>
		/// <param name="dst"></param>
		/// <param name="library"></param>
		public AcquisizioneDocumento(string dst, string library) 
		{
			getDocObject = new PCDGetDoc();
			getDocObject.SetDST(dst);
			getDocObject.AddSearchCriteria("%TARGET_LIBRARY", library);
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

			result = getDocObject.ErrNumber; 

			if(result != 0)
			{
				if(customDescription != null)
				{
					logger.Debug(customDescription);
				}

				logger.Debug("PCD Error Number: " + getDocObject.ErrNumber);
				logger.Debug("PCD Error Description: " + getDocObject.ErrDescription);
			}

			return result;
		}
		#endregion

		#region Proprietà
		/// <summary>
		/// Restituisce l'istanza dell'oggetto per l'acquisizione del documento
		/// </summary>
		protected PCDGetDoc CurrentInstance
		{
			get
			{
				return this.getDocObject;
			}
		}

		/// <summary>
		/// Restituisce l'oggetto stream per l'acquisizione del documento
		/// </summary>
		public AcquisizioneStream Stream
		{
			get
			{
				PCDGetStream stream = (PCDGetStream)getDocObject.GetPropertyValue("%CONTENT");
				AcquisizioneStream acquisizioneStream = new AcquisizioneStream(stream);

				return acquisizioneStream;
			}
		}
		
		/// <summary>
		/// </summary>
		public string VersionId
		{
			set
			{
				getDocObject.AddSearchCriteria("%VERSION_ID", value);
			}
		}
		
		/// <summary>
		/// </summary>
		public string DocumentNumber
		{
			set
			{
				getDocObject.AddSearchCriteria("%DOCUMENT_NUMBER", value);			
			}
		}

		/// <summary>
		/// </summary>
		public string Author
		{
			get
			{
				return getDocObject.GetPropertyValue("AUTHOR").ToString();
			}
		}
		#endregion

		#region Metodi
		/// <summary>
		/// </summary>
		public void AddSearchCriteria(string propertyName, string criteria)
		{
			getDocObject.AddSearchCriteria(propertyName, criteria);
		}

		/// <summary>
		/// </summary>
		public int Execute()
		{
			return getDocObject.Execute();
		}

		/// <summary>
		/// </summary>
		public int NextRow()
		{
			return getDocObject.NextRow();
		}

		/// <summary>
		/// </summary>
		public int SetRow(int rowNumber)
		{
			return getDocObject.SetRow(rowNumber);
		}
		#endregion
	}
}
