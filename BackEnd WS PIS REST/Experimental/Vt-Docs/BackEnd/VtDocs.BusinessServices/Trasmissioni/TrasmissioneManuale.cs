using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Trasmissioni
{
    /// <summary>
    /// 
    /// </summary>
    public class TrasmissioneManuale : Trasmissione
    {
        /// <summary>
        /// 
        /// </summary>
        public enum TipiTrasmissioneEnum
        {
            Utente,
            Ruolo,
        }

        /// <summary>
        /// 
        /// </summary>
        private DocsPaVO.utente.InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        private TipiTrasmissioneEnum _tipoTrasmissione = TipiTrasmissioneEnum.Utente;

        /// <summary>
        /// 
        /// </summary>
        public TrasmissioneManuale(DocsPaVO.utente.InfoUtente infoUtente)
            : base(infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="tipoTrasmissione"></param>
        public TrasmissioneManuale(DocsPaVO.utente.InfoUtente infoUtente, TipiTrasmissioneEnum tipoTrasmissione)
            : this(infoUtente)
        {
            this._tipoTrasmissione = tipoTrasmissione;
        }

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idOggetto"></param>
        /// <param name="tipoOggetto"></param>
        /// <param name="ragione"></param>
        /// <param name="noteGenerali"></param>
        /// <param name="idDestinatari"></param>
        public virtual void SaveAsync(string idOggetto,
                        DocsPaVO.trasmissione.TipoOggetto tipoOggetto,
                        string ragione,
                        string noteGenerali,
                        bool notificaInToDoList,
                        params string[] idDestinatari)
        {

            // Avvio task di salvataggio trasmissione asincrono
            SaveTrasmissioneDelegate del = new SaveTrasmissioneDelegate(this.Save);
            IAsyncResult result = del.BeginInvoke(idOggetto, tipoOggetto, ragione, noteGenerali, notificaInToDoList, idDestinatari, this.SaveTrasmissioneCompleted, this);
        }

        /// <summary>
        /// Handler evento di trasmissione salvata
        /// </summary>
        /// <param name="result"></param>
        protected virtual void SaveTrasmissioneCompleted(IAsyncResult result)
        {
            if (result.IsCompleted)
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idOggetto"></param>
        /// <param name="tipoOggetto"></param>
        /// <param name="ragione"></param>
        /// <returns></returns>
        protected string GetIdTrasmissione(string idOggetto,
                                          DocsPaVO.trasmissione.TipoOggetto tipoOggetto,
                                            string ragione)
        {
            string idTrasmissione = string.Empty;

            DocsPaVO.trasmissione.RagioneTrasmissione ragioneTrasmissione = this.GetRagioneTrasmissione(ragione);


            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string filtroOggetto = string.Empty;

                if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
                    filtroOggetto = string.Format("T.ID_PROFILE = {0}", idOggetto);
                else
                    filtroOggetto = string.Format("T.ID_PROJECT = {0}", idOggetto);

                string commandText =
                    string.Format(
                    "SELECT T.SYSTEM_ID, CG.ID_PEOPLE, CG.ID_GRUPPO " +
                    "FROM DPA_TRASMISSIONE T " +
                    "INNER JOIN DPA_TRASM_SINGOLA TS ON TS.ID_TRASMISSIONE = T.SYSTEM_ID " +
                    "INNER JOIN DPA_CORR_GLOBALI CG ON CG.SYSTEM_ID = TS.ID_CORR_GLOBALE " +
                    "WHERE {0} AND TS.ID_RAGIONE = '{1}'",
                    filtroOggetto, ragioneTrasmissione.systemId);

                dbProvider.ExecuteScalar(out idTrasmissione, commandText);
            }

            return idTrasmissione;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idOggetto"></param>
        /// <param name="tipoOggetto"></param>
        /// <param name="ragione"></param>
        /// <returns></returns>
        protected string GetIdTrasmissioneDaInviare(string idOggetto,
                                                    DocsPaVO.trasmissione.TipoOggetto tipoOggetto,
                                                    string ragione)
        {
            string idTrasmissione = string.Empty;

            DocsPaVO.trasmissione.RagioneTrasmissione ragioneTrasmissione = this.GetRagioneTrasmissione(ragione);

            
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string filtroOggetto = string.Empty;
                
                if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
                    filtroOggetto = string.Format("T.ID_PROFILE = {0}", idOggetto);
                else
                    filtroOggetto = string.Format("T.ID_PROJECT = {0}", idOggetto);

                string commandText =
                    string.Format(
                    "SELECT T.SYSTEM_ID, CG.ID_PEOPLE, CG.ID_GRUPPO " +
                    "FROM DPA_TRASMISSIONE T " +
                    "INNER JOIN DPA_TRASM_SINGOLA TS ON TS.ID_TRASMISSIONE = T.SYSTEM_ID " +
                    "INNER JOIN DPA_CORR_GLOBALI CG ON CG.SYSTEM_ID = TS.ID_CORR_GLOBALE " +
                    "WHERE {0} AND T.DTA_INVIO IS NULL AND TS.ID_RAGIONE = '{1}'",
                    filtroOggetto, ragioneTrasmissione.systemId);

                dbProvider.ExecuteScalar(out idTrasmissione, commandText);
            }

            return idTrasmissione;
        }

        /// <summary>
        /// Salva la trasmissione manuale dell'oggetto agli utenti destinatari
        /// </summary>
        /// <param name="idOggetto">Id dell'oggetto da trasmettere</param>
        /// <param name="tipoOggetto"></param>
        /// <param name="ragione"></param>
        /// <param name="noteGenerali"></param>
        /// <param name="notificaInToDoList"></param>
        /// <param name="idDestinatari"></param>
        /// <remarks>
        /// Sarà applicata la ragione trasmissione predefinita per l'amministrazione
        /// </remarks>
        public virtual void Save(string idOggetto,
                                DocsPaVO.trasmissione.TipoOggetto tipoOggetto,
                                string ragione,
                                string noteGenerali,
                                bool notificaInToDoList,
                                params string[] idDestinatari)
        {
            //DocsPaVO.ricerche.SearchPagingContext pc = new DocsPaVO.ricerche.SearchPagingContext(1, Int32.MaxValue);

            //DocsPaVO.trasmissione.InfoTrasmissione[] txEffettuate = BusinessLogic.Trasmissioni.QueryTrasmManager.GetInfoTrasmissioniEffettuate(this._infoUtente, idOggetto, (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO ? "D" : "F"), pc);

            
            string idTrasmissione = this.GetIdTrasmissioneDaInviare(idOggetto, tipoOggetto, (ragione ?? string.Empty));

            bool prepareNew = false;

            if (string.IsNullOrEmpty(idTrasmissione))
            {
                prepareNew = true;
            }
            else
            {
                DocsPaVO.trasmissione.Trasmissione tx = this.GetTrasmissione(idTrasmissione, idOggetto, tipoOggetto);

                this.UpdateTrasmissione(tx, idDestinatari);

                tx = BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(tx);
            }

            if (prepareNew)
            {
                DocsPaVO.trasmissione.Trasmissione tx = this.PrepareTrasmissione(idOggetto, tipoOggetto, ragione, noteGenerali, notificaInToDoList, idDestinatari);

                tx = BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(tx);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idOggetto"></param>
        /// <param name="tipoOggetto"></param>
        /// <param name="ragione"></param>
        /// <param name="noteGenerali"></param>
        /// <param name="idDestinatari"></param>
        public virtual void ExecuteAsync(string idOggetto,
                                   DocsPaVO.trasmissione.TipoOggetto tipoOggetto,
                                   string ragione,
                                   string noteGenerali,
                                   bool notificaInToDoList,
                                   params string[] idDestinatari)
        {
            // Avvio task di trasmissione asincrono
            ExecuteTrasmissioneDelegate del = new ExecuteTrasmissioneDelegate(this.Save);
            IAsyncResult result = del.BeginInvoke(idOggetto, tipoOggetto, ragione, noteGenerali, notificaInToDoList, idDestinatari, this.ExecuteTrasmissioneCompleted, this);
        }

        /// <summary>
        /// Handler evento di trasmissione salvata
        /// </summary>
        /// <param name="result"></param>
        protected virtual void ExecuteTrasmissioneCompleted(IAsyncResult result)
        {
            if (result.IsCompleted)
            {

            }
        }

        /// <summary>
        /// Trasmissione manuale dell'oggetto agli utenti destinatari
        /// </summary>
        /// <param name="idOggetto">Id dell'oggetto da trasmettere</param>
        /// <param name="tipoOggetto">Tipo dell'oggetto da trasmettere</param>
        /// <param name="modello">Modello trasmissione da utilizzare</param>
        /// <remarks>
        /// Sarà applicata la ragione trasmissione predefinita per l'amministrazione
        /// </remarks>
        public virtual void Execute(string idOggetto,
                                      DocsPaVO.trasmissione.TipoOggetto tipoOggetto,
                                      string ragione,
                                      string noteGenerali,
                                      bool notificaInToDoList,
                                      params string[] idDestinatari)
        {
            //string idTrasmissione = this.GetIdTrasmissione(idOggetto, tipoOggetto, (ragione ?? string.Empty));
            
            string idTrasmissione = this.GetIdTrasmissioneDaInviare(idOggetto, tipoOggetto, (ragione ?? string.Empty));

            if (string.IsNullOrEmpty(idTrasmissione))
            {
                // Creazione della nuova trasmissione
                DocsPaVO.trasmissione.Trasmissione tx = this.PrepareTrasmissione(idOggetto, tipoOggetto, ragione, noteGenerali, notificaInToDoList, idDestinatari);

                tx = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(string.Empty, tx);
            }
            else
            {
                // Invio della trasmissione esistente
                DocsPaVO.trasmissione.Trasmissione tx = this.GetTrasmissione(idTrasmissione, idOggetto, tipoOggetto);

                // La trasmissione ancora non è stata inviata
                this.UpdateTrasmissione(tx, idDestinatari);

                tx = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(string.Empty, tx);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Definizione delegate per salvataggio trasmissione asincrona
        /// </summary>
        /// <param name="idOggetto"></param>
        /// <param name="tipoOggetto"></param>
        /// <param name="ragione"></param>
        /// <param name="noteGenerali"></param>
        /// <param name="idDestinatari"></param>
        protected delegate void SaveTrasmissioneDelegate(string idOggetto,
                                                        DocsPaVO.trasmissione.TipoOggetto tipoOggetto,
                                                        string ragione,
                                                        string noteGenerali,
                                                        bool notificaInToDoList,
                                                        params string[] idDestinatari);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idOggetto"></param>
        /// <param name="tipoOggetto"></param>
        /// <param name="ragione"></param>
        /// <param name="noteGenerali"></param>
        /// <param name="idDestinatari"></param>
        protected delegate void ExecuteTrasmissioneDelegate(string idOggetto,
                                              DocsPaVO.trasmissione.TipoOggetto tipoOggetto,
                                              string ragione,
                                              string noteGenerali,
                                              bool notificaInToDoList,
                                              params string[] idDestinatari);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trasmissione"></param>
        /// <param name="idDestinatari"></param>
        /// <returns></returns>
        protected virtual void UpdateTrasmissione(
                                    DocsPaVO.trasmissione.Trasmissione trasmissione,
                                    params string[] idDestinatari)
        {
            DocsPaVO.trasmissione.TrasmissioneSingola[] tsingole = (DocsPaVO.trasmissione.TrasmissioneSingola[])
                        trasmissione.trasmissioniSingole.ToArray(typeof(DocsPaVO.trasmissione.TrasmissioneSingola));

            foreach (string idDest in idDestinatari)
            {
                if (tsingole.Count(e => e.corrispondenteInterno.systemId == idDest) == 0)
                {
                    string codiceDestinatario = string.Empty;

                    if (this._tipoTrasmissione == TipiTrasmissioneEnum.Utente)
                    {
                        DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtenteById(idDest);

                        codiceDestinatario = utente.userId;
                    }
                    else
                    {
                        DocsPaVO.utente.Ruolo ruolo = null;

                        int idDestAsInt;
                        if (Int32.TryParse(idDest, out idDestAsInt))
                        {
                            // L'identificativo fornito è l'id del ruolo direttamente
                            ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(idDest);

                            if (ruolo == null)
                                ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(idDest);
                        }
                        else
                        {
                            // L'identificativo fornito è il codice del ruolo
                            ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(idDest);
                        }


                        codiceDestinatario = ruolo.codiceRubrica;
                    }

                    DocsPaVO.utente.Corrispondente corrispondenteDestinatario = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(
                                    codiceDestinatario, DocsPaVO.addressbook.TipoUtente.INTERNO, this._infoUtente);

                    if (corrispondenteDestinatario != null)
                    {
                        // Reperimento ragione trasmissione di default
                        string idRagioneDefault = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(this._infoUtente.idAmministrazione).IDRagioneCompetenza;

                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById(idRagioneDefault);

                        string tipoTrasm = (this._tipoTrasmissione == TipiTrasmissioneEnum.Utente ? "S" : "T");

                        trasmissione = AddTrasmissioneSingola(this._infoUtente,
                                                trasmissione,
                                                corrispondenteDestinatario,
                                                ragione,
                                                string.Empty,
                                                tipoTrasm,
                                                0);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idOggetto"></param>
        /// <param name="tipoOggetto"></param>
        /// <param name="ragione"></param>
        /// <param name="noteGenerali"></param>
        /// <param name="idDestinatari"></param>
        /// <returns></returns>
        protected virtual DocsPaVO.trasmissione.Trasmissione PrepareTrasmissione(
                                            string idOggetto,
                                            DocsPaVO.trasmissione.TipoOggetto tipoOggetto,
                                            string ragione,
                                            string noteGenerali,
                                            bool notificaInToDoList,
                                            params string[] idDestinatari)
        {
            DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();

            trasmissione.noteGenerali = noteGenerali;

            DocsPaVO.documento.SchedaDocumento schedaDocumento = null;
            DocsPaVO.fascicolazione.Fascicolo schedaFascicolo = null;

            if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
            {
                trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;

                schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglio(this._infoUtente, idOggetto, idOggetto);

                trasmissione.infoDocumento = new DocsPaVO.documento.InfoDocumento(schedaDocumento);
            }
            else if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.FASCICOLO)
            {
                trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;

                schedaFascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(idOggetto, this._infoUtente);

                trasmissione.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo(schedaFascicolo);
            }

            trasmissione.NO_NOTIFY = (notificaInToDoList ? "0" : "1");
            trasmissione.utente = BusinessLogic.Utenti.UserManager.getUtente(this._infoUtente.idPeople);
            trasmissione.ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(this._infoUtente.idCorrGlobali);

            foreach (string idDest in idDestinatari)
            {
                string codiceDestinatario = string.Empty;

                if (this._tipoTrasmissione == TipiTrasmissioneEnum.Utente)
                {
                    DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtenteById(idDest);

                    codiceDestinatario = utente.userId;
                }
                else
                {
                    DocsPaVO.utente.Ruolo ruolo = null;

                    int idDestAsInt;
                    if (Int32.TryParse(idDest, out idDestAsInt))
                    {
                        // L'identificativo fornito è l'id del ruolo direttamente
                        ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(idDest);

                        if (ruolo == null)
                            ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(idDest);
                    }
                    else
                    {
                        // L'identificativo fornito è il codice del ruolo
                        ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(idDest);
                    }

                    codiceDestinatario = ruolo.codiceRubrica;
                }

                DocsPaVO.utente.Corrispondente corrispondenteDestinatario = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(
                                codiceDestinatario, DocsPaVO.addressbook.TipoUtente.INTERNO, this._infoUtente);

                if (corrispondenteDestinatario != null)
                {
                    DocsPaVO.trasmissione.RagioneTrasmissione ragioneTrasmissione = this.GetRagioneTrasmissione(ragione);

                    string tipoTrasm = (this._tipoTrasmissione == TipiTrasmissioneEnum.Utente ? "S" : "T");

                    trasmissione = this.AddTrasmissioneSingola(this._infoUtente,
                                            trasmissione,
                                            corrispondenteDestinatario,
                                            ragioneTrasmissione,
                                            string.Empty,
                                            tipoTrasm,
                                            0);
                }
            }

            return trasmissione;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ragione"></param>
        /// <returns></returns>
        protected virtual DocsPaVO.trasmissione.RagioneTrasmissione GetRagioneTrasmissione(string ragione)
        {
            DocsPaVO.trasmissione.RagioneTrasmissione ragioneTrasmissione = null;

            if (string.IsNullOrEmpty(ragione))
            {
                string idRagioneDefault = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(this._infoUtente.idAmministrazione).IDRagioneCompetenza;

                // Reperimento ragione trasmissione predefinita
                ragioneTrasmissione = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById(idRagioneDefault);
            }
            else
            {
                // Reperimento della ragione trasmissione richiesta
                ragioneTrasmissione = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(this._infoUtente.idAmministrazione, ragione);
            }

            return ragioneTrasmissione;
        }

        /// <summary>
        /// Reperimento trasmissione corrente
        /// </summary>
        /// <param name="idTrasmissione"></param>
        /// <returns></returns>
        protected virtual DocsPaVO.trasmissione.Trasmissione GetTrasmissione(string idTrasmissione,
                                                                            string idOggetto,
                                                                            DocsPaVO.trasmissione.TipoOggetto tipoOggetto)
        {
            DocsPaVO.trasmissione.Trasmissione retValue = null;

            if (!string.IsNullOrEmpty(idTrasmissione))
            {
                // Reperimento oggetto trasmissione
                DocsPaVO.trasmissione.OggettoTrasm oggettoTrasm = new DocsPaVO.trasmissione.OggettoTrasm();

                if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
                {
                    DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglio(this._infoUtente, idOggetto, idOggetto);

                    oggettoTrasm.infoDocumento = new DocsPaVO.documento.InfoDocumento(schedaDocumento);
                }
                else if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.FASCICOLO)
                {
                    DocsPaVO.fascicolazione.Fascicolo schedaFascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(idOggetto, this._infoUtente);

                    oggettoTrasm.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo(schedaFascicolo);
                }

                System.Collections.Generic.List<DocsPaVO.filtri.FiltroRicerca> filters = new System.Collections.Generic.List<DocsPaVO.filtri.FiltroRicerca>();
                DocsPaVO.filtri.FiltroRicerca item = new DocsPaVO.filtri.FiltroRicerca();
                item.argomento = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.ID_TRASMISSIONE.ToString();
                item.valore = idTrasmissione;
                filters.Add(item);

                DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(this._infoUtente.idPeople);
                DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(this._infoUtente.idCorrGlobali);

                // Utilizzo del metodo di ricerca trasmissioni fornendo i filtri di ricerca
                int totalPageNumber;
                int recordCount;

                DocsPaVO.trasmissione.Trasmissione[] trasmissioni =
                    (DocsPaVO.trasmissione.Trasmissione[])
                        BusinessLogic.Trasmissioni.QueryTrasmManager.getQueryEffettuateDocMethodPaging(
                                                oggettoTrasm,
                                                utente,
                                                ruolo,
                                                filters.ToArray(),
                                                1,
                                                out totalPageNumber,
                                                out recordCount).ToArray(typeof(DocsPaVO.trasmissione.Trasmissione));

                if (trasmissioni.Length == 1)
                {
                    // Reperimento prima trasmissione estratta
                    retValue = trasmissioni[0];
                }
            }

            return retValue;
        }

        #endregion
    }
}
