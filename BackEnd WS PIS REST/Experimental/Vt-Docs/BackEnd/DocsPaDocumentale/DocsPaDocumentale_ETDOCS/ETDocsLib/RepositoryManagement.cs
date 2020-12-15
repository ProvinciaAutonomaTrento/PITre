using System;
using log4net;

namespace DocsPaDocumentale_ETDOCS.ETDocsLib
{
	/// <summary>
	/// Questa classe implemanta la gestione del repository 
	/// dei file nel documentale ETDocs
	/// </summary>
	public class RepositoryManagement
	{
        private ILog logger = LogManager.GetLogger(typeof(RepositoryManagement));
		#region Costruttori
		/// <summary>
		/// </summary>
		public RepositoryManagement()
		{
		}
		#endregion

		#region Metodi
		/// <summary>
		/// Controlla se un dato file esiste
		/// </summary>
		/// <returns>
		/// True = Il file esiste; False = Il file non esiste o si è 
		/// verificato un errore durante la ricerca
		/// </returns>
		public bool FileExists()
		{
			bool result = true; // Presume successo

			try
			{
				// TODO: Write code here
			}
			catch(Exception exception)
			{
				logger.Error("Errore durante la ricerca del file.", exception);

				result = false;
			}

			return result;
		}
		
		/// <summary>Crea un muovo file</summary>
		/// <returns>True = OK; False = Errore</returns>
		public bool CreateFile()
		{
			bool result = true; // Presume successo

			try
			{
				// TODO: Write code here
			}
			catch(Exception exception)
			{
				logger.Error("Errore durante la creazione del file.", exception);

				result = false;
			}

			return result;
		}

		/// <summary>Cancella un file</summary>
		/// <returns>True = OK; False = Errore</returns>
		public bool DeleteFile()
		{
			bool result = true; // Presume successo

			try
			{
				// TODO: Write code here
			}
			catch(Exception exception)
			{
				logger.Error("Errore durante la cancellazione del file.", exception);

				result = false;
			}

			return result;
		}
		#endregion
	}
} 