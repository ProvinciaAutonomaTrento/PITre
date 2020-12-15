using System;
using log4net;

namespace DocsPaDocumentale_FILENET.FilenetLib
{
	/// <summary>
	/// Questa classe implemanta la gestione dei file nel documentale FileNet
	/// </summary>
	public class FileManagement
	{
        private ILog logger = LogManager.GetLogger(typeof(FileManagement));
		#region Costruttori
		/// <summary>
		/// </summary>
		public FileManagement()
		{
		}
		#endregion

		#region Metodi
		/// <summary>Leggi un file</summary>
		/// <returns>True = OK; False = Errore</returns>
		public bool Read()
		{
			bool result = true; // Presume successo

			try
			{
				// TODO: Write code here
			}
			catch(Exception exception)
			{
				logger.Error("Errore durante la lettura del file.", exception);

				result = false;
			}

			return result;
		}

		/// <summary>Scrivi su un file</summary>
		/// <returns>True = OK; False = Errore</returns>
		public bool Write()
		{
			bool result = true; // Presume successo

			try
			{
				// TODO: Write code here
			}
			catch(Exception exception)
			{
				logger.Error("Errore durante la scrittura del file.", exception);

				result = false;
			}

			return result;
		}
		#endregion
	}
}
