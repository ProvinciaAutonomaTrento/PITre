using System;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.SitoAccessibile.Documenti.Allegati
{
	/// <summary>
	/// Classe per la gestione della logica relativa
	/// agli allegati di un documento
	/// </summary>
	public class AllegatiHandler
	{
		public AllegatiHandler()
		{
		}

		/// <summary>
		/// Reperimento degli allegati del documento
		/// </summary>
		/// <param name="docNumber"></param>
		/// <returns></returns>
		public Allegato[] GetAllegatiDocumento(string docNumber)
		{
			DocsPaWebService ws=new DocsPaWebService();
			return ws.DocumentoGetAllegati(docNumber, string.Empty, string.Empty);
		}

		/// <summary>
		/// Reperimento degli allegati del documento
		/// </summary>
		/// <param name="infoDocumento"></param>
		/// <returns></returns>
		public Allegato[] GetAllegatiDocumento(InfoDocumento infoDocumento)
		{
			return this.GetAllegatiDocumento(infoDocumento.docNumber);
		}

		/// <summary>
		/// Reperimento degli allegati del documento
		/// </summary>
		/// <param name="schedaDocumento"></param>
		/// <returns></returns>
		public Allegato[] GetAllegatiDocumento(SchedaDocumento schedaDocumento)
		{		
			DocsPaWR.InfoDocumento infoDocumento=DocumentManager.getInfoDocumento(schedaDocumento);
			return this.GetAllegatiDocumento(infoDocumento);
		}

		/// <summary>
		/// Verifica se un allegato risulta acquisito o meno
		/// </summary>
		/// <param name="allegato"></param>
		/// <returns></returns>
		public bool IsAcquired(Allegato allegato)
		{
			return (allegato.fileName!=null && allegato.fileName!=string.Empty &&
				allegato.fileSize!=null && allegato.fileSize!="0");
		}
	}
}
