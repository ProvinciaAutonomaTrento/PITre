using System;
using PCDCLIENTLib;
using log4net;

namespace DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib
{
	/// <summary>
	/// Questa classe gestisce tutti gli accessi a basso livello 
	/// al'oggetto PCDPutDoc della libreria Hummingbird
	/// </summary>
	public class InserimentoDocumento : DocsPaUtils.Interfaces.FileManagement.IGestioneErrori
	{
        private ILog logger = LogManager.GetLogger(typeof(InserimentoDocumento));
		// Istanza del documento
		protected PCDPutDoc putDocObject;

		#region Costruttori
		/// <summary>
		/// Questo costruttore instanzia un oggetto per l'acquisizione di un documento sul documentale
		/// </summary>
		/// <param name="dst"></param>
		/// <param name="library"></param>
		public InserimentoDocumento(string dst, string library) 
		{
			putDocObject = new PCDPutDoc();
			putDocObject.SetDST(dst);
			putDocObject.AddSearchCriteria("%TARGET_LIBRARY", library);
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

			result = putDocObject.ErrNumber; 

			if(result != 0)
			{
				if(customDescription != null)
				{
					logger.Debug(customDescription);
				}

				logger.Debug("PCD Error Number: " + putDocObject.ErrNumber);
				logger.Debug("PCD Error Description: " + putDocObject.ErrDescription);
			}

			return result;
		}
		#endregion

		#region Proprietà
		/// <summary>
		/// Restituisce l'istanza dell'oggetto per l'acquisizione del documento
		/// </summary>
		protected PCDPutDoc CurrentInstance
		{
			get
			{
				return this.putDocObject;
			}
		}

		/// <summary>
		/// Restituisce l'oggetto stream per l'inserimento del documento
		/// </summary>
		public InserimentoStream Stream
		{
			get
			{
				PCDPutStream stream = (PCDPutStream)putDocObject.GetPropertyValue("%CONTENT");
				InserimentoStream inserimentoStream = new InserimentoStream(stream);

				return inserimentoStream;
			}
		}
		
		/// <summary>
		/// </summary>
		public string VersionId
		{
			set
			{
				putDocObject.AddSearchCriteria("%VERSION_ID", value);
			}
		}

		/// <summary>
		/// </summary>
		public string DocumentNumber
		{
			set
			{
				putDocObject.AddSearchCriteria("%DOCUMENT_NUMBER", value);			
			}
		}
		#endregion

		#region Metodi
		/// <summary>
		/// </summary>
		public int Execute()
		{
			return putDocObject.Execute();
		}

		/// <summary>
		/// </summary>
		public int NextRow()
		{
			return putDocObject.NextRow();
		}
		#endregion
	}
}
