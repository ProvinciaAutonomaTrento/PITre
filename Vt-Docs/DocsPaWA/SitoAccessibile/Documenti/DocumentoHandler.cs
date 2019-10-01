using System;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.SitoAccessibile.Documenti
{
	/// <summary>
	/// Classe per la gestione della logica relativa ad un documento
	/// </summary>
	public class DocumentoHandler
	{
		public DocumentoHandler()
		{
		}

		/// <summary>
		/// Reperimento di un documento
		/// </summary>
		/// <param name="idProfile"></param>
		/// <param name="docNumber"></param>
		/// <returns></returns>
		public SchedaDocumento GetDocumento(string idProfile,string docNumber) 
		{
			if (idProfile==null || idProfile==string.Empty)
				throw new ApplicationException("Parametro 'idProfile' non fornito");
				
			if (docNumber==null || docNumber==string.Empty)
				throw new ApplicationException("Parametro 'docNumber' non fornito");

			DocsPaWebService ws=new DocsPaWebService();
			return ws.DocumentoGetDettaglioDocumento(UserManager.getInfoUtente(),idProfile,docNumber);
		}

		/// <summary>
		/// Reperimento delle possibili parole chiavi da associare ad un documento
		/// </summary>
		/// <param name="idAmministrazione"></param>
		/// <returns></returns>
		public DocumentoParolaChiave[] GetParoleChiavi(string idAmministrazione)
		{
			DocsPaWebService ws=new DocsPaWebService();
			DocumentoParolaChiave[] retValue=ws.DocumentoGetParoleChiave(idAmministrazione);

			if (retValue==null)
				retValue=new DocumentoParolaChiave[0];

			return retValue;
		}

		/// <summary>
		/// Reperimento delle possibili parole chiavi da associare ad un documento
		/// </summary>
		/// <returns></returns>
		public DocumentoParolaChiave[] GetParoleChiavi()
		{
			return this.GetParoleChiavi(UserManager.getInfoUtente().idAmministrazione);
		}
	}
}
