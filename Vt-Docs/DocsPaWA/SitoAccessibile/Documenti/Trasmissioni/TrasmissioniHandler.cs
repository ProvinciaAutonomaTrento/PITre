using System;
using System.Collections;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SitoAccessibile.Ricerca;
using DocsPAWA.SitoAccessibile.Paging;

namespace DocsPAWA.SitoAccessibile.Documenti.Trasmissioni
{
	/// <summary>
	/// Classe per la gestione della logica relativa
	/// alle operazioni di reperimento delle trasmissioni di un documento.
	/// </summary>
	public class TrasmissioniHandler
	{
		public TrasmissioniHandler()
		{
		}

        /// <summary>
        /// Reperimento trasmissioni del documento, 
        /// con la possibilità di effettuare filtri
        /// </summary>
        /// <param name="searchType"></param>
        /// <param name="pagingContext"></param>
        /// <param name="filters"></param>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public Trasmissione[] GetTrasmissioniDocumento(SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum tipoTrasmissione, PagingContext pagingContext, FiltroRicerca[] filters, SchedaDocumento schedaDocumento)
        {
            Trasmissione[] retValue = null;

            TrasmissioneOggettoTrasm oggettoTrasm = new TrasmissioneOggettoTrasm();
            if (schedaDocumento != null)
                oggettoTrasm.infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);

            Utente utente = UserManager.getUtente();
            Ruolo ruolo = UserManager.getRuolo();

            DocsPaWebService ws = new DocsPaWebService();

            int pageCount;
            int recordCount;

            if (tipoTrasmissione == SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum.Effettuate)
            {
                retValue = ws.TrasmissioneGetQueryEffettuateDocPaging(
                    oggettoTrasm,
                    filters,
                    utente,
                    ruolo,
                    pagingContext.PageNumber,
                    out pageCount,
                    out recordCount);
            }
            else
            {
                pageCount = 0;
                recordCount = 0;

                retValue = ws.TrasmissioneGetQueryRicevute(oggettoTrasm, filters, utente, ruolo);
            }

            pagingContext.PageCount = pageCount;
            pagingContext.RecordCount = recordCount;

            return retValue;
        }
		
		/// <summary>
		/// Reperimento trasmissioni del documento
		/// </summary>
		/// <param name="searchType"></param>
		/// <param name="searchPagingInfo">Criteri di paginazione</param>
		/// <param name="schedaDocumento"></param>
		/// <returns></returns>
		public Trasmissione[] GetTrasmissioniDocumento(SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum tipoTrasmissione,PagingContext pagingContext,SchedaDocumento schedaDocumento)
		{
			return this.GetTrasmissioniDocumento(tipoTrasmissione,pagingContext,null,schedaDocumento);
		}

		/// <summary>
		/// Reperimento numero trasmissioni di un documento
		/// </summary>
		/// <param name="schedaDocumento"></param>
		/// <returns></returns>
		public int GetCountTrasmissioni(SchedaDocumento schedaDocumento)
		{	
			DocsPaWR.DocsPaWebService ws=new DocsPAWA.DocsPaWR.DocsPaWebService();
			return ws.DocumentoGetCountTrasmissioniDocumento(Convert.ToInt32(schedaDocumento.systemId));
		}
	}
}