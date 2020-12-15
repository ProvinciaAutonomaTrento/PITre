using System;
using System.Collections;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SitoAccessibile.Ricerca;
using DocsPAWA.SitoAccessibile.Paging;

namespace DocsPAWA.SitoAccessibile.Trasmissioni
{
	/// <summary>
	/// Classe per la gestione della logica relativa alle operazioni sulle trasmissioni
	/// </summary>
	public class TrasmissioniHandler
	{
		public TrasmissioniHandler()
		{
		}

		/// <summary>
		/// Reperimento ragioni trasmissione
		/// </summary>
		/// <returns></returns>
		public RagioneTrasmissione[] GetRagioniTrasmissione()
		{
			return this.GetRagioniTrasmissione(string.Empty);
		}

		/// <summary>
		/// Reperimento ragioni trasmissione
		/// </summary>
		/// <param name="accessRights"></param>
		/// <returns></returns>
		public RagioneTrasmissione[] GetRagioniTrasmissione(string accessRights)
		{
			DocsPaWR.TrasmissioneDiritti diritti=new DocsPAWA.DocsPaWR.TrasmissioneDiritti();
			
			if (accessRights!=null && accessRights!=string.Empty)
				diritti.accessRights=accessRights;

			diritti.idAmministrazione=UserManager.getUtente().idAmministrazione;

			DocsPaWebService ws=new DocsPaWebService();
			return ws.TrasmissioneGetRagioni(diritti,false);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipoTrasmissione"></param>
        /// <param name="oggettoTrasmesso"></param>
        /// <param name="pagingContext"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public Trasmissione[] GetTrasmissioni(SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum tipoTrasmissione,
                TrasmissioneOggettoTrasm oggettoTrasmesso, PagingContext pagingContext, FiltroRicerca[] filters)
        {
            Trasmissione[] retValue = null;

            Utente utente = UserManager.getUtente();
            Ruolo ruolo = UserManager.getRuolo();

            DocsPaWebService ws = new DocsPaWebService();

            int pageCount;
            int recordCount;

            if (tipoTrasmissione == SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum.Effettuate)
            {
                retValue = ws.TrasmissioneGetQueryEffettuateDocPaging(
                    oggettoTrasmesso,
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

                retValue = ws.TrasmissioneGetQueryRicevute(oggettoTrasmesso, filters, utente, ruolo);
            }

            pagingContext.PageCount = pageCount;
            pagingContext.RecordCount = recordCount;

            return retValue;
        }

        /// <summary>
        /// Reperimento di una trasmissione
        /// </summary>
        /// <param name="tipoTrasmissione"></param>
        /// <param name="idTrasmissione"></param>
        /// <returns></returns>
        public Trasmissione GetTrasmissione(TipiTrasmissioniEnum tipoTrasmissione, string idTrasmissione)
        {
            DocsPAWA.SitoAccessibile.Documenti.Trasmissioni.TrasmissioniHandler trasmissioniHandler = new DocsPAWA.SitoAccessibile.Documenti.Trasmissioni.TrasmissioniHandler();

            Trasmissione[] trasmissioni = trasmissioniHandler.GetTrasmissioniDocumento(tipoTrasmissione, new PagingContext(1), this.GetFilter(idTrasmissione), null);

            if (trasmissioni.Length > 0)
                return trasmissioni[0];
            else
                return null;
        }

		/// <summary>
		/// Verifica se, per la trasmissione, è richiesta la 
		/// gestione dell'accettazione/rifiuto da parte dell'utente
		/// </summary>
		/// <param name="trasmissione"></param>
		/// <returns></returns>
		public bool IsRequiredAccettazioneRifiuto(Trasmissione trasmissione)
		{
			bool retValue=false;

			if (trasmissione.trasmissioniSingole.Length>0)
			{
				TrasmissioneSingola trasmSingola=trasmissione.trasmissioniSingole[0];

				if (trasmSingola.trasmissioneUtente.Length>0)
				{
					TrasmissioneUtente trasmUtente=trasmSingola.trasmissioneUtente[0];

					retValue=(trasmSingola.ragione.tipo.Equals("W") &&
						trasmUtente.dataRifiutata.Equals(string.Empty) && 
						trasmUtente.dataAccettata.Equals(string.Empty));
				}
			}

			return retValue;
		}

		/// <summary>
		/// Accettazione trasmissione
		/// </summary>
		/// <param name="trasmissioneUtente"></param>
		/// <param name="noteAccettazione"></param>
		/// <returns></returns>
		public bool AccettaTrasmissione(TrasmissioneUtente trasmissioneUtente,string noteAccettazione, Trasmissione trasm)
		{
            string errore = string.Empty;
			trasmissioneUtente.noteAccettazione=noteAccettazione;
			trasmissioneUtente.dataAccettata=DateTime.Now.ToString("dd/MM/yyyy");
			trasmissioneUtente.tipoRisposta=DocsPaWR.TrasmissioneTipoRisposta.ACCETTAZIONE;

			DocsPaWebService ws=new DocsPaWebService();
            return ws.TrasmissioneExecuteAccRif(trasmissioneUtente, trasm.systemId, UserManager.getRuolo(), UserManager.getInfoUtente(), out errore);
		}

		/// <summary>
		/// Rifiuto di una trasmissione
		/// </summary>
		/// <param name="trasmissioneUtente"></param>
		/// <param name="noteRifiuto"></param>
		/// <returns></returns>
		public bool RifiutaTrasmissione(TrasmissioneUtente trasmissioneUtente,string noteRifiuto, Trasmissione trasmissione)
		{
			bool retValue=false;
            string errore = string.Empty;

			if (noteRifiuto==null || noteRifiuto==string.Empty)
				throw new ApplicationException("Note di rifiuto non impostate");

			trasmissioneUtente.noteRifiuto=noteRifiuto;
			trasmissioneUtente.dataRifiutata=DateTime.Now.ToString("dd/MM/yyyy");
			trasmissioneUtente.tipoRisposta=DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO;

			DocsPaWebService ws=new DocsPaWebService();

            if (ws.TrasmissioneExecuteAccRif(trasmissioneUtente, trasmissione.systemId, UserManager.getRuolo(), UserManager.getInfoUtente(), out errore))
				if(trasmissioneUtente.tipoRisposta.Equals(TrasmissioneTipoRisposta.RIFIUTO))
					retValue=ws.RitornaAlMittTrasmUt(trasmissioneUtente, UserManager.getInfoUtente());

			return retValue;
		}

        /// <summary>
        /// Costruzione filtro ricerca per singola trasmissione
        /// </summary>
        /// <param name="idTrasmissione"></param>
        /// <returns></returns>
        private FiltroRicerca[] GetFilter(string idTrasmissione)
        {
            ArrayList list = new ArrayList();
            FiltroRicerca filtroRicerca = new FiltroRicerca();
            filtroRicerca.argomento = DocsPaWR.FiltriTrasmissioneNascosti.ID_TRASMISSIONE.ToString();
            filtroRicerca.valore = idTrasmissione;
            list.Add(filtroRicerca);

            return (FiltroRicerca[])list.ToArray(typeof(FiltroRicerca));
        }
	}
}