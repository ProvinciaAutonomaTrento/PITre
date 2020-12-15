using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P3SBCLib.DocsPaServices
{
    /// <summary>
    /// Classe per la gestione delle trasmissioni tramite modello utilizzate dai servizi SBC
    /// </summary>
    /// <remarks>
    /// ERRONEAMENTE CREDEVO CHE LE TRASMISSIONI AUTOMATICHE E I PASSAGGI DI STATO
    /// DEI FASCICOLI FOSSERO FATTE DIRETTAMENTE DA BACKEND CONTESTUALMENTE ALLA
    /// CREAZIONE DEL FASCICOLO.
    /// INVECE TALE TASK è EFFETTUATO NEL FRONTEND, PERTANTO HO RIPORTATO
    /// IN QUESTA CLASSE LA MAGGIOR PARTE DELLA LOGICA ESPRESSA 
    /// NELLA CLASSE "TrasmManager" DEL FRONTEND.
    /// I NOMI DEI METODI SONO STATI LASCIATI VOLUTAMENTE GLI STESSI (ECCETTO QUELLO PUBBLICO)
    /// PER MANTENERE LA LOGICA QUANTO PIù POSSIBILE INALTERATA.
    /// </remarks>
    internal sealed class DocsPaTxServiceHelper
    {
        /// <summary>
        /// 
        /// </summary>
        private DocsPaTxServiceHelper()
        { }

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="fascicolo"></param>
        /// <param name="idStato"></param>
        public static void ExecuteStateTx(
                DocsPaVO.utente.InfoUtente infoUtente,     
                DocsPaVO.fascicolazione.Fascicolo fascicolo, 
                string idStato)
        {
            // Reperimento degli eventuali modelli trasmissione associati al primo stato 
            System.Collections.ArrayList listModelliTrasmissione = BusinessLogic.DiagrammiStato.DiagrammiStato.isStatoTrasmAutoFasc(
                                    infoUtente.idAmministrazione,
                                    idStato,
                                    fascicolo.template.SYSTEM_ID.ToString());

            foreach (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione m in listModelliTrasmissione)
            {
                if (m.SINGLE == "1")
                {
                    effettuaTrasmissioneFascDaModello(infoUtente, m, idStato, fascicolo);
                }
                else
                {
                    foreach (DocsPaVO.Modelli_Trasmissioni.MittDest mitt in m.MITTENTE)
                    {
                        if (string.Compare(mitt.ID_CORR_GLOBALI.ToString(), infoUtente.idCorrGlobali, true) == 0)
                        {
                            effettuaTrasmissioneFascDaModello(infoUtente, m, idStato, fascicolo);
                            break;
                        }
                    }
                }
            }
        }

        #endregion 

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="corr"></param>
        /// <returns></returns>
        private static DocsPaVO.utente.Corrispondente[] queryUtenti(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Corrispondente corr)
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
        /// <param name="trasmissione"></param>
        /// <param name="corr"></param>
        /// <param name="ragione"></param>
        /// <param name="note"></param>
        /// <param name="tipoTrasm"></param>
        /// <param name="scadenza"></param>
        /// <returns></returns>
        public static DocsPaVO.trasmissione.Trasmissione addTrasmissioneSingola(
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

                DocsPaVO.utente.Corrispondente[] listaUtenti = queryUtenti(infoUtente, corr);
                
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
                    trasmissione = addTrasmissioneSingola(infoUtente, trasmissione, r, ragione, note, tipoTrasm, scadenza);

                return trasmissione;
            }

            if (trasmissioneSingola != null)
                trasmissione.trasmissioniSingole.Add(trasmissioneSingola);

            return trasmissione;
        }

        /// <summary>
        /// Tramissione di un fascicolo usando un modello di trasmissione
        /// Il parametro "idStato" puo' essere null o meno a seconda delle necessità
        /// </summary>
        /// <returns></returns>
        /// 
        public static void effettuaTrasmissioneFascDaModello(
                                        DocsPaVO.utente.InfoUtente infoUtente, 
                                        DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello, 
                                        string idStato, 
                                        DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();
             
            trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;
            trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;
            trasmissione.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo(fascicolo);
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
                        corr = getCorrispondenti(infoUtente, mittDest.CHA_TIPO_MITT_DEST, fascicolo, trasmissione);
                    }

                    if (corr != null)
                    {
                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById(mittDest.ID_RAGIONE.ToString());

                        trasmissione = addTrasmissioneSingola(infoUtente, trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA);
                    }
                }
            }
            
            trasmissione = impostaNotificheUtentiDaModello(infoUtente, trasmissione, modello);
            trasmissione = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(string.Empty, trasmissione);

            string desc = "";

            string notify = "1";
            if (trasmissione.NO_NOTIFY != null && trasmissione.NO_NOTIFY.Equals("1"))
            {
                notify = "0";
            }
            else
            {
                notify = "1";
            }

            if (trasmissione != null)
            {
                // LOG per documento
                if (trasmissione.infoDocumento != null && !string.IsNullOrEmpty(trasmissione.infoDocumento.idProfile))
                {
                    foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasmissione.trasmissioniSingole)
                    {
                        string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                        if (trasmissione.infoDocumento.segnatura == null)
                            desc = "Trasmesso Documento : " + trasmissione.infoDocumento.docNumber.ToString();
                        else
                            desc = "Trasmesso Documento : " + trasmissione.infoDocumento.segnatura.ToString();

                        BusinessLogic.UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, method, trasmissione.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, notify, single.systemId);
                    }
                }
                // LOG per fascicolo
                if (trasmissione.infoFascicolo != null && !string.IsNullOrEmpty(trasmissione.infoFascicolo.idFascicolo))
                {
                    foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasmissione.trasmissioniSingole)
                    {
                        string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                        desc = "Trasmesso Fascicolo ID: " + trasmissione.infoFascicolo.idFascicolo.ToString();
                        BusinessLogic.UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, method, trasmissione.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, notify, single.systemId);
                    }
                }
            }

            if (!string.IsNullOrEmpty(idStato))
                BusinessLogic.DiagrammiStato.DiagrammiStato.salvaStoricoTrasmDiagrammiFasc(trasmissione.systemId, fascicolo.systemID, idStato);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="tipo_destinatario"></param>
        /// <param name="fascicolo"></param>
        /// <param name="trasmissione"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Corrispondente getCorrispondenti(
                DocsPaVO.utente.InfoUtente infoUtente,
                string tipo_destinatario, 
                DocsPaVO.fascicolazione.Fascicolo fascicolo, 
                DocsPaVO.trasmissione.Trasmissione trasmissione)
        {
            DocsPaVO.utente.Corrispondente corr = null;

            //se la il modello di trasmissione ha come destinatario l'utente proprietario del documento
            
            //trsmissione a utente proprietario del documento
            if (tipo_destinatario == "UT_P")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(fascicolo.creatoreFascicolo.idPeople, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
            }
            //ruolo proprietario
            else if (tipo_destinatario == "R_P")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(fascicolo.creatoreFascicolo.idCorrGlob_Ruolo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
            }
            //uo proprietaria
            else if (tipo_destinatario == "UO_P")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(fascicolo.creatoreFascicolo.idCorrGlob_UO, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
            }
            //responsabile uo proprietario
            else if (tipo_destinatario == "RSP_P")
            {
                string idCorrGlobaliUo = fascicolo.creatoreFascicolo.idCorrGlob_UO;
                string idCorr = fascicolo.creatoreFascicolo.idCorrGlob_Ruolo;
                string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "R", idCorr);

                if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
            }
            //ruolo segretario uo del proprietario
            else if (tipo_destinatario == "R_S")
            {
                string idCorrGlobaliUo = fascicolo.creatoreFascicolo.idCorrGlob_UO;
                string idCorr = fascicolo.creatoreFascicolo.idCorrGlob_Ruolo;
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
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="objTrasm"></param>
        /// <param name="modello"></param>
        /// <returns></returns>
        private static DocsPaVO.trasmissione.Trasmissione impostaNotificheUtentiDaModello( 
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
                        tu.daNotificare = daNotificareSuModello(infoUtente, tu.utente.idPeople, ts.corrispondenteInterno.systemId, modello);
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
        private static bool daNotificareSuModello(
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
