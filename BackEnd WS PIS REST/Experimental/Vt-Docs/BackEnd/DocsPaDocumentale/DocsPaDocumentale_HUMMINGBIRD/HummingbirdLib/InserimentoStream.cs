using System;
using PCDCLIENTLib;
using log4net;

namespace DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib
{
	/// <summary>
	/// Questa classe gestisce tutti gli accessi a basso livello 
	/// all'oggetto PCDGetStream della libreria Hummingbird
	/// </summary>
	public class InserimentoStream : DocsPaUtils.Interfaces.FileManagement.IGestioneErrori
	{
        private ILog logger = LogManager.GetLogger(typeof(InserimentoStream));
		// Istanza dello stream
		protected PCDPutStream streamDocObject;

		#region Costruttori
		/// <summary>
		/// Questo costruttore instanzia uno stream per l'acquisizione di un documento sul documentale
		/// </summary>
		/// <param name="stream"></param>
		public InserimentoStream(PCDPutStream stream) 
		{
			streamDocObject = stream;
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

			result = streamDocObject.ErrNumber; 

			if(result != 0)
			{
				if(customDescription != null)
				{
					logger.Debug(customDescription);
				}

				logger.Debug("PCD Error Number: " + streamDocObject.ErrNumber);
				logger.Debug("PCD Error Description: " + streamDocObject.ErrDescription);
			}

			return result;
		}
		#endregion

		#region Proprietà
		/// <summary>
		/// Restituisce l'istanza dell'oggetto stream
		/// </summary>
		protected PCDPutStream CurrentInstance
		{
			get
			{
				return this.streamDocObject;
			}
		}
		
		/// <summary>
		/// </summary>
		public int StreamSize
		{
			get
			{
				return (int)streamDocObject.GetPropertyValue("%ISTREAM_STATSTG_CBSIZE_LOWPART");
			}
		}
		#endregion

		#region Metodi
		/// <summary>
		/// Read data from the stream
		/// </summary>
		/// <param name="stream">Stream da scrivere</param>
		/// <param name="bytes"></param>
		/// <param name="bytesWritten"></param>
		public void Write(byte[] stream, int bytes, out int bytesWritten)
		{
			streamDocObject.Write(stream, bytes, out bytesWritten);
		}	

		/// <summary>
		/// </summary>
		public int SetComplete()
		{
			return streamDocObject.SetComplete();
		}
		#endregion
	}
}
