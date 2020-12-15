using System;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.SitoAccessibile.Documenti.Versioni
{
	/// <summary>
	/// Classe per la gestione della logica relativa
	/// alle versioni dei documenti.
	/// </summary>
	public class VersioniHandler
	{
		public VersioniHandler()
		{
		}

		/// <summary>
		/// Verifica se un documento è stato acquisito o meno
		/// </summary>
		/// <param name="versione"></param>
		/// <returns></returns>
		public bool IsAcquired(Documento versione)
		{
			return (versione.fileName!=null && versione.fileName!=string.Empty &&
					versione.fileSize!=null && versione.fileSize!="0");
		}
	}
}
