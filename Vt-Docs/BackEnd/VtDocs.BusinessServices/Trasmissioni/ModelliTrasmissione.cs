using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Trasmissioni
{
    /// <summary>
    /// 
    /// </summary>
    public class ModelliTrasmissione : Trasmissione
    {
        /// <summary>
        /// 
        /// </summary>
        private DocsPaVO.utente.InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        public ModelliTrasmissione(DocsPaVO.utente.InfoUtente infoUtente)
            : base(infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione[] GetModelli()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idOggetto"></param>
        /// <param name="tipoOggetto"></param>
        /// <param name="idStato"></param>
        public void ExecuteStato(string idOggetto,
                                    DocsPaVO.trasmissione.TipoOggetto tipoOggetto,
                                    string idStato)
        {
            System.Collections.ArrayList listModelliTrasmissione = null;

            string idTemplate = string.Empty;

            // Reperimento id del template
            if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
            {
                idTemplate = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getIdTemplate(idOggetto);

                // Reperimento degli eventuali modelli trasmissione associati allo stato del documento
                listModelliTrasmissione = BusinessLogic.DiagrammiStato.DiagrammiStato.isStatoTrasmAuto(
                                        this._infoUtente.idAmministrazione,
                                        idStato,
                                        idTemplate);
            }
            else if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.FASCICOLO)
            {
                idTemplate = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getIdTemplateFasc(idOggetto);

                // Reperimento degli eventuali modelli trasmissione associati allo stato del fascicolo
                listModelliTrasmissione = BusinessLogic.DiagrammiStato.DiagrammiStato.isStatoTrasmAutoFasc(
                                        this._infoUtente.idAmministrazione,
                                        idStato,
                                        idTemplate);
            }


            foreach (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione m in listModelliTrasmissione)
            {
                if (m.SINGLE == "1")
                {
                    this.EffettuaTrasmissioneDaModello(this._infoUtente, m, idOggetto, tipoOggetto);
                }
                else
                {
                    foreach (DocsPaVO.Modelli_Trasmissioni.MittDest mitt in m.MITTENTE)
                    {
                        if (string.Compare(mitt.ID_CORR_GLOBALI.ToString(), this._infoUtente.idCorrGlobali, true) == 0)
                        {
                            this.EffettuaTrasmissioneDaModello(this._infoUtente, m, idOggetto, tipoOggetto);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idOggetto">Id dell'oggetto da trasmettere</param>
        /// <param name="tipoOggetto">Tipo dell'oggetto da trasmettere</param>
        /// <param name="modello">Modello trasmissione da utilizzare</param>
        public virtual void Execute(string idOggetto,
                                    DocsPaVO.trasmissione.TipoOggetto tipoOggetto,
                                    DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello)
        {
            if (modello == null)
                throw new ApplicationException("Modello trasmissione non trovato");

            if (modello.SINGLE == "1")
            {
                EffettuaTrasmissioneDaModello(this._infoUtente, modello, idOggetto, tipoOggetto);
            }
            else
            {
                foreach (DocsPaVO.Modelli_Trasmissioni.MittDest mitt in modello.MITTENTE)
                {
                    if (string.Compare(mitt.ID_CORR_GLOBALI.ToString(), this._infoUtente.idCorrGlobali, true) == 0)
                    {
                        EffettuaTrasmissioneDaModello(this._infoUtente, modello, idOggetto, tipoOggetto);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idOggetto"></param>
        /// <param name="tipoOggetto"></param>
        /// <param name="idModello"></param>
        public virtual void Execute(string idOggetto,
                                    DocsPaVO.trasmissione.TipoOggetto tipoOggetto,
                                    string idModello)
        {
            // Reperimento del modello trasmissione
            DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modelloTrasmissione = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByID(this._infoUtente.idAmministrazione, idModello);

            this.Execute(idOggetto, tipoOggetto, modelloTrasmissione);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Tramissione di un fascicolo usando un modello di trasmissione
        /// Il parametro "idStato" puo' essere null o meno a seconda delle necessità
        /// </summary>
        /// <returns></returns>
        private void EffettuaTrasmissioneDaModello(
                                        DocsPaVO.utente.InfoUtente infoUtente,
                                        DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello,
                                        string idOggetto,
                                        DocsPaVO.trasmissione.TipoOggetto tipoOggetto)
        {
            DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();

            trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;

            DocsPaVO.documento.SchedaDocumento schedaDocumento = null;
            DocsPaVO.fascicolazione.Fascicolo schedaFascicolo = null;

            if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
            {
                trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;

                schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idOggetto, idOggetto);

                trasmissione.infoDocumento = new DocsPaVO.documento.InfoDocumento(schedaDocumento);
            }
            else if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.FASCICOLO)
            {
                trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;

                schedaFascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(idOggetto, infoUtente);

                trasmissione.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo(schedaFascicolo);
            }

            trasmissione.utente = BusinessLogic.Utenti.UserManager.getUtente(infoUtente.idPeople);
            trasmissione.ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(infoUtente.idCorrGlobali);

            if (modello != null)
                trasmissione.NO_NOTIFY = modello.NO_NOTIFY;

            //Parametri delle trasmissioni singole
            foreach (DocsPaVO.Modelli_Trasmissioni.RagioneDest ragioneDest in modello.RAGIONI_DESTINATARI)
            {
                foreach (DocsPaVO.Modelli_Trasmissioni.MittDest mittDest in ragioneDest.DESTINATARI)
                {
                    DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();

                    if (mittDest.CHA_TIPO_MITT_DEST == "D")
                    {
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(mittDest.VAR_COD_RUBRICA, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                    }
                    else
                    {
                        if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
                        {
                            corr = GetCorrispondente(infoUtente,
                                                        mittDest.CHA_TIPO_MITT_DEST,
                                                        schedaDocumento.creatoreDocumento.idPeople,
                                                        schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo,
                                                        schedaDocumento.creatoreDocumento.idCorrGlob_UO,
                                                        trasmissione);
                        }
                        else
                        {
                            corr = GetCorrispondente(infoUtente,
                                                        mittDest.CHA_TIPO_MITT_DEST,
                                                        schedaFascicolo.creatoreFascicolo.idPeople,
                                                        schedaFascicolo.creatoreFascicolo.idCorrGlob_Ruolo,
                                                        schedaFascicolo.creatoreFascicolo.idCorrGlob_UO,
                                                        trasmissione);
                        }
                    }

                    if (corr != null)
                    {
                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById(mittDest.ID_RAGIONE.ToString());

                        trasmissione = AddTrasmissioneSingola(infoUtente, trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA);
                    }
                }
            }

            trasmissione = ImpostaNotificheUtentiDaModello(infoUtente, trasmissione, modello);
            trasmissione = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(string.Empty, trasmissione);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="objTrasm"></param>
        /// <param name="modello"></param>
        /// <returns></returns>
        private DocsPaVO.trasmissione.Trasmissione ImpostaNotificheUtentiDaModello(
                DocsPaVO.utente.InfoUtente infoUtente,
                DocsPaVO.trasmissione.Trasmissione objTrasm,
                DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello)
        {
            if (objTrasm.trasmissioniSingole != null && objTrasm.trasmissioniSingole.Count > 0)
            {
                foreach (DocsPaVO.trasmissione.TrasmissioneSingola ts in objTrasm.trasmissioniSingole)
                {
                    foreach (DocsPaVO.trasmissione.TrasmissioneUtente tu in ts.trasmissioneUtente)
                    {
                        tu.daNotificare = DaNotificareSuModello(infoUtente, tu.utente.idPeople, ts.corrispondenteInterno.systemId, modello);
                    }
                }
            }

            return objTrasm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="currentIDPeople"></param>
        /// <param name="currentIDCorrGlobRuolo"></param>
        /// <param name="modello"></param>
        /// <returns></returns>
        private bool DaNotificareSuModello(
                                DocsPaVO.utente.InfoUtente infoUtente,
                                string currentIDPeople,
                                string currentIDCorrGlobRuolo,
                                DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello)
        {
            bool retValue = true;

            foreach (DocsPaVO.Modelli_Trasmissioni.RagioneDest rd in modello.RAGIONI_DESTINATARI)
            {
                foreach (DocsPaVO.Modelli_Trasmissioni.MittDest md in rd.DESTINATARI)
                {
                    if (md.ID_CORR_GLOBALI.Equals(Convert.ToInt32(currentIDCorrGlobRuolo)))
                    {
                        foreach (DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm unt in md.UTENTI_NOTIFICA)
                        {
                            if (unt.ID_PEOPLE.Equals(currentIDPeople))
                            {
                                if (unt.FLAG_NOTIFICA.Equals("1"))
                                    retValue = true;
                                else
                                    retValue = false;

                                return retValue;
                            }
                        }
                    }
                }
            }

            return retValue;
        }

        #endregion
    }
}
