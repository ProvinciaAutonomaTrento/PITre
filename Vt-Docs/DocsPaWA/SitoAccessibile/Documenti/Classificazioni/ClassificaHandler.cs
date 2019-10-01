using System;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.SitoAccessibile.Documenti.Classificazioni
{
	/// <summary>
	/// Classe per la gestione della logica relativa
	/// alle classificazioni dei documenti.
	/// </summary>
	public class ClassificaHandler
	{
		public ClassificaHandler()
		{
		}

		/// <summary>
		/// Reperimento numero classificazioni di un documento
		/// </summary>
		/// <param name="idProfile"></param>
		/// <returns></returns>
		public int GetCountClassificazioniDocumento(string idProfile)
		{
			InfoUtente infoUtente=UserManager.getInfoUtente();

			DocsPaWebService ws=new DocsPaWebService();
			return ws.FascicolazioneGetCountClassificazioniDocumento(idProfile,infoUtente.idGruppo,infoUtente.idPeople);
		}

		/// <summary>
		/// Reperimento dei fascicoli presenti nel documento
		/// </summary>
		/// <param name="idProfile"></param>
		/// <returns></returns>
		public Fascicolo[] GetFascicoliDocumento(string idProfile)
		{
			// Reperimento contesto utente corrente
			InfoUtente infoUtente = UserManager.getInfoUtente();

			DocsPaWR.DocsPaWebService ws=new DocsPaWebService();
			return ws.FascicolazioneGetFascicoliDaDoc(infoUtente, idProfile);
		}

		/// <summary>
		/// Reperimento dei fascicoli presenti nel documento
		/// </summary>
		/// <param name="infoDocumento"></param>
		/// <returns></returns>
		public Fascicolo[] GetFascicoliDocumento(InfoDocumento infoDocumento)
		{
			return this.GetFascicoliDocumento(infoDocumento.idProfile);
		}

		/// <summary>
		/// Reperimento dei fascicoli presenti nel documento
		/// </summary>
		/// <param name="schedaDocumento"></param>
		/// <returns></returns>
		public Fascicolo[] GetFascicoliDocumento(SchedaDocumento schedaDocumento)
		{
			return this.GetFascicoliDocumento(DocumentManager.getInfoDocumento(schedaDocumento));
		}

		/// <summary>
		/// Reperimento classificazione da codice
		/// </summary>
		/// <param name="codiceClassifica"></param>
		/// <param name="registro"></param>
		/// <returns></returns>
		public DocsPAWA.DocsPaWR.FascicolazioneClassificazione GetClassificazione(string codiceClassifica,Registro registro)
		{
			InfoUtente infoUtente=UserManager.getInfoUtente();
			return this.GetClassificazione(codiceClassifica,registro,infoUtente.idAmministrazione,infoUtente.idGruppo,infoUtente.idPeople);
		}

		/// <summary>
		/// Reperimento classificazione da codice
		/// </summary>
		/// <param name="codiceClassifica"></param>
		/// <param name="registro"></param>
		/// <param name="idAmministrazione"></param>
		/// <param name="idGruppo"></param>
		/// <param name="idPeople"></param>
		/// <returns></returns>
		public DocsPAWA.DocsPaWR.FascicolazioneClassificazione GetClassificazione(string codiceClassifica,Registro registro,string idAmministrazione,string idGruppo,string idPeople)
		{
			DocsPaWebService ws=new DocsPaWebService();
			
			DocsPaWR.FascicolazioneClassificazione[] retValue=ws.FascicolazioneGetTitolario(idAmministrazione,idGruppo,idPeople,registro,codiceClassifica,true);
            
            if (retValue != null && retValue.Length > 0)
				return retValue[0];
			else
				return null;
		}
	}
}
