using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Trasmissioni
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Trasmissione
    {
        /// <summary>
        /// 
        /// </summary>
        private DocsPaVO.utente.InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        public Trasmissione(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #region Public Methods

        #endregion

        #region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="trasmissione"></param>
        /// <param name="corr"></param>
        /// <param name="ragione"></param>
        /// <param name="note"></param>
        /// <param name="tipoTrasm"></param>
        /// <param name="scadenza"></param>
        /// <returns></returns>
        protected DocsPaVO.trasmissione.Trasmissione AddTrasmissioneSingola(
                                        DocsPaVO.utente.InfoUtente infoUtente,
                                        DocsPaVO.trasmissione.Trasmissione trasmissione,
                                        DocsPaVO.utente.Corrispondente corr,
                                        DocsPaVO.trasmissione.RagioneTrasmissione ragione,
                                        string note,
                                        string tipoTrasm,
                                        int scadenza)
        {
            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato

                foreach (DocsPaVO.trasmissione.TrasmissioneSingola ts in trasmissione.trasmissioniSingole)
                {
                    if (ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                    {
                        if (ts.daEliminare)
                        {
                            ts.daEliminare = false;
                            return trasmissione;
                        }
                        else
                            return trasmissione;
                    }
                }
            }

            // Aggiungo la trasmissione singola
            DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = tipoTrasm;
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragione;
            trasmissioneSingola.noteSingole = note;
            //Imposto la data di scadenza
            if (scadenza > 0)
            {
                string dataScadenza = "";
                System.DateTime data = System.DateTime.Now.AddDays(scadenza);
                dataScadenza = data.Day + "/" + data.Month + "/" + data.Year;
                trasmissioneSingola.dataScadenza = dataScadenza;
            }

            // Aggiungo la lista di trasmissioniUtente
            if (corr is DocsPaVO.utente.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;

                DocsPaVO.utente.Corrispondente[] listaUtenti = GetUtenti(infoUtente, corr);

                if (listaUtenti.Length == 0)
                    trasmissioneSingola = null;

                //ciclo per utenti se dest è gruppo o ruolo
                for (int i = 0; i < listaUtenti.Length; i++)
                {
                    DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                    trasmissioneUtente.utente = (DocsPaVO.utente.Utente)listaUtenti[i];
                    trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
                }
            }

            if (corr is DocsPaVO.utente.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
                DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPaVO.utente.Utente)corr;
                trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
            }

            if (corr is DocsPaVO.utente.UnitaOrganizzativa)
            {
                DocsPaVO.utente.UnitaOrganizzativa theUo = (DocsPaVO.utente.UnitaOrganizzativa)corr;
                DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = new DocsPaVO.addressbook.QueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                qca.ruolo = trasmissione.ruolo;

                System.Collections.ArrayList ruoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, theUo);
                foreach (DocsPaVO.utente.Ruolo r in ruoli)
                    trasmissione = AddTrasmissioneSingola(infoUtente, trasmissione, r, ragione, note, tipoTrasm, scadenza);

                return trasmissione;
            }

            if (trasmissioneSingola != null)
                trasmissione.trasmissioniSingole.Add(trasmissioneSingola);

            return trasmissione;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="corr"></param>
        /// <returns></returns>
        protected DocsPaVO.utente.Corrispondente[] GetUtenti(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Corrispondente corr)
        {
            //costruzione oggetto queryCorrispondente
            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();

            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;
            qco.idAmministrazione = infoUtente.idAmministrazione;
            qco.fineValidita = true;
            //corrispondenti interni
            qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;

            return (DocsPaVO.utente.Corrispondente[])
                BusinessLogic.Utenti.addressBookManager.getListaCorrispondenti(qco).ToArray(typeof(DocsPaVO.utente.Corrispondente));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="tipo_destinatario"></param>
        /// <param name="fascicolo"></param>
        /// <param name="trasmissione"></param>
        /// <returns></returns>
        protected DocsPaVO.utente.Corrispondente GetCorrispondente(
                DocsPaVO.utente.InfoUtente infoUtente,
                string tipo_destinatario,
                string idPeopleCreatore,
                string idCorrGlobRuoloCreatore,
                string idCorrGlobUoCreatore,
                DocsPaVO.trasmissione.Trasmissione trasmissione)
        {
            DocsPaVO.utente.Corrispondente corr = null;

            //se la il modello di trasmissione ha come destinatario l'utente proprietario del documento

            //trsmissione a utente proprietario del documento
            if (tipo_destinatario == "UT_P")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(idPeopleCreatore, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
            }
            //ruolo proprietario
            else if (tipo_destinatario == "R_P")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobRuoloCreatore, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
            }
            //uo proprietaria
            else if (tipo_destinatario == "UO_P")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobUoCreatore, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
            }
            //responsabile uo proprietario
            else if (tipo_destinatario == "RSP_P")
            {
                string idCorrGlobaliUo = idCorrGlobUoCreatore;
                string idCorr = idCorrGlobRuoloCreatore;
                string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "R", idCorr);

                if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
            }
            //ruolo segretario uo del proprietario
            else if (tipo_destinatario == "R_S")
            {
                string idCorrGlobaliUo = idCorrGlobUoCreatore;
                string idCorr = idCorrGlobRuoloCreatore;
                string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "S", idCorr);

                if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
            }
            //ruolo responsabile uo del mittente
            else if (tipo_destinatario == "RSP_M")
            {
                string idCorrGlobaliUo = trasmissione.ruolo.uo.systemId;
                string idCorr = trasmissione.ruolo.systemId;

                string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "R", idCorr);

                if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
            }
            //RUOLO SEGRETARIO UO DEL MITTENTE
            else if (tipo_destinatario == "S_M")
            {
                string idCorrGlobaliUo = trasmissione.ruolo.uo.systemId;
                string idCorr = trasmissione.ruolo.systemId;
                string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "S", idCorr);

                if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
            }

            return corr;
        }

        /// <summary>
        /// Inizializzazione della trasmissione
        /// </summary>
        /// <param name="tipoOggetto"></param>
        /// <param name="idOggetto"></param>
        /// <remarks>
        /// Le classi derivate dovranno utilizzare tale metodo per inizializzare l'oggetto trasmissione piuttosto che crearne uno nuovo
        /// </remarks>
        /// <returns></returns>
        protected virtual DocsPaVO.trasmissione.Trasmissione CreateTrasmissione(DocsPaVO.trasmissione.TipoOggetto tipoOggetto, string idOggetto)
        {
            DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();

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

            trasmissione.utente = BusinessLogic.Utenti.UserManager.getUtente(this._infoUtente.idPeople);
            trasmissione.ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(this._infoUtente.idCorrGlobali);

            return trasmissione;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="schedaFascicolo"></param>
        ///// <returns></returns>
        //protected virtual DocsPaVO.trasmissione.Trasmissione CreateTrasmissioneFascicolo(DocsPaVO.fascicolazione.Fascicolo schedaFascicolo)
        //{
        //    DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();

        //    trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;

        //    trasmissione.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo(schedaFascicolo);
        //    trasmissione.utente = BusinessLogic.Utenti.UserManager.getUtente(this._infoUtente.idPeople);
        //    trasmissione.ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(this._infoUtente.idCorrGlobali);

        //    return trasmissione;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="schedaDocumento"></param>
        ///// <returns></returns>
        //protected virtual DocsPaVO.trasmissione.Trasmissione CreateTrasmissioneDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        //{
        //    DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();

        //    trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;

        //    trasmissione.infoDocumento = new DocsPaVO.documento.InfoDocumento(schedaDocumento);
        //    trasmissione.utente = BusinessLogic.Utenti.UserManager.getUtente(this._infoUtente.idPeople);
        //    trasmissione.ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(this._infoUtente.idCorrGlobali);

        //    return trasmissione;
        //}

        #endregion
    }
}
