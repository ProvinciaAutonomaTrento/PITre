using System;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SitoAccessibile.Trasmissioni;
using DocsPAWA.SitoAccessibile.Documenti.Trasmissioni;
using DocsPAWA.SitoAccessibile.Paging;

namespace DocsPAWA.SitoAccessibile.Fascicoli
{
    /// <summary>
    /// Classe per la gestione della logica relativa ai registri
    /// </summary>
    public class FascicoloHandler
    {
        public FascicoloHandler()
        {
        }

        /// <summary>
        /// Reperimento di un fascicolo in base all'id
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public DocsPAWA.DocsPaWR.Fascicolo GetFascicolo(string idFascicolo, string idRegistro)
        {
            Registro registro = null;

            if (idRegistro != null && idRegistro != string.Empty)
                registro = this.GetRegistro(idRegistro);

            return this.GetFascicolo(idFascicolo, registro);
        }

        ///// <summary>
        ///// Reperimento di un fascicolo a partire dall'infofascicolo
        ///// </summary>
        ///// <param name="infoFascicolo"></param>
        ///// <returns></returns>
        //public DocsPAWA.DocsPaWR.Fascicolo GetFascicolo(DocsPaWR.InfoFascicolo infoFascicolo)
        //{
        //    //return ProxyManager.getWS().FascicolazioneGetDettaglioFascicolo(UserManager.getInfoUtente(), infoFascicolo, false);
        //}

        /// <summary>
        /// Reperimento di un fascicolo in base all'id
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="registro"></param>
        /// <returns></returns>
        public DocsPAWA.DocsPaWR.Fascicolo GetFascicolo(string idFascicolo, Registro registro)
        {
            return ProxyManager.getWS().FascicolazioneGetFascicoloById(idFascicolo, UserManager.getInfoUtente());

            //DocsPaWebService ws=new DocsPaWebService();
            //return ws.FascicolazioneGetFascicolo(idFascicolo,
            //                                    UserManager.getInfoUtente(),
            //                                    registro,
            //                                    false,false);
        }

        /// <summary>
        /// Reperimento del folder in cui è catalogato il fascicolo
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        public DocsPAWA.DocsPaWR.Folder GetFolder(DocsPAWA.DocsPaWR.Fascicolo fascicolo)
        {
            InfoUtente infoUtente = UserManager.getInfoUtente();

            DocsPaWebService ws = new DocsPaWebService();
            return ws.FascicolazioneGetFolder(infoUtente.idPeople, infoUtente.idGruppo, fascicolo);
        }

        /// <summary>
        /// Reperimento dei documenti contenuti in un folder
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="idFolder"></param>
        /// <param name="idRegistro"></param>
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        public InfoDocumento[] GetDocumenti(string idFascicolo,
                                            string idFolder,
                                            string idRegistro,
                                            Paging.PagingContext pagingContext)
        {
            InfoDocumento[] retValue = null;

            DocsPaWR.Fascicolo fascicolo = this.GetFascicolo(idFascicolo, idRegistro);

            Folder folder = this.SearchFolderById(this.GetFolder(fascicolo), idFolder);

            if (folder != null)
            {
                InfoUtente infoUtente = UserManager.getInfoUtente();

                int pageCount;
                int recordCount;

                DocsPaWebService ws = new DocsPaWebService();

                SearchResultInfo[] idProfile;

                retValue = ws.FascicolazioneGetDocumentiPaging(infoUtente.idGruppo, infoUtente.idPeople, folder, pagingContext.PageNumber, false, out pageCount, out recordCount, out idProfile);
                pagingContext.PageCount = pageCount;
                pagingContext.RecordCount = recordCount;
            }
            else
            {
                throw new ApplicationException("ID folder '" + idFolder + "' non trovato");
            }

            return retValue;
        }

        public Trasmissione[] GetTrasmissioniFascicolo(TipiTrasmissioniEnum tipoTrasmissione,
                                                        FiltroRicerca[] filters,
                                                        DocsPaWR.Fascicolo fascicolo)
        {
            return null;

        }

        /// <summary>
        /// Reperimento delle trasmissioni effettuate o ricevute relativamente ad un fascicolo
        /// </summary>
        /// <param name="tipoTrasmissione"></param>
        /// <param name="filters"></param>
        /// <param name="fascicolo"></param>
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        public Trasmissione[] GetTrasmissioniFascicolo(TipiTrasmissioniEnum tipoTrasmissione,
                                                        FiltroRicerca[] filters,
                                                        DocsPaWR.Fascicolo fascicolo,
                                                        Paging.PagingContext pagingContext)
        {
            if (fascicolo == null)
                throw new ApplicationException("Parametro 'fascicolo' non impostato");

            Trasmissione[] retValue = null;

            TrasmissioneOggettoTrasm oggettoTrasm = new TrasmissioneOggettoTrasm();
            oggettoTrasm.infoFascicolo = FascicoliManager.getInfoFascicoloDaFascicolo(fascicolo, null);

            Utente utente = UserManager.getUtente();
            Ruolo ruolo = UserManager.getRuolo();

            DocsPaWebService ws = new DocsPaWebService();

            int pageCount;
            int recordCount;

            if (tipoTrasmissione == TipiTrasmissioniEnum.Effettuate)
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
                retValue = ws.TrasmissioneGetQueryRicevutePaging(
                    oggettoTrasm,
                    filters,
                    utente,
                    ruolo,
                    pagingContext.PageNumber,
                    out pageCount,
                    out recordCount);
            }

            pagingContext.PageCount = pageCount;
            pagingContext.RecordCount = recordCount;

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        public string GetCodiceTitolario(DocsPAWA.DocsPaWR.Fascicolo fascicolo)
        {
            string retValue = string.Empty;

            DocsPaWebService ws = new DocsPaWebService();
            DocsPaWR.FascicolazioneClassifica[] classifiche = ws.FascicolazioneGetGerarchia(fascicolo.idClassificazione, UserManager.getUtente().idAmministrazione);
            if (classifiche != null)
                retValue = ((DocsPAWA.DocsPaWR.FascicolazioneClassifica)classifiche[classifiche.Length - 1]).codice;

            return retValue;
        }

        /// <summary>
        /// Ricerca ricorsiva di un folder 
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="idFolder"></param>
        /// <returns></returns>
        private Folder SearchFolderById(Folder folder, string idFolder)
        {
            Folder retValue = null;

            if (folder.systemID.Equals(idFolder))
            {
                retValue = folder;
            }
            else
            {
                foreach (Folder childfolder in folder.childs)
                {
                    if (childfolder.systemID.Equals(idFolder))
                    {
                        retValue = childfolder;
                        break;
                    }
                    else
                    {
                        retValue = this.SearchFolderById(childfolder, idFolder);

                        if (retValue != null)
                            break;
                    }
                }
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento registro da codice
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        private Registro GetRegistro(string idRegistro)
        {
            Registro retValue = null;

            foreach (Registro item in UserManager.getRuolo().registri)
            {
                if (item.systemId.Equals(idRegistro))
                {
                    retValue = item;
                    break;
                }
            }

            return retValue;
        }
    }
}