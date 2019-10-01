using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;

namespace NttDataWA.UIManager
{
    public class TrasmManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocOrFasc"></param>
        /// <param name="docOrFasc"></param>
        /// <param name="objListaFiltri"></param>
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        public static DocsPaWR.InfoTrasmissione[] GetInfoTrasmissioniFiltered(string idDocOrFasc, string docOrFasc, FiltroRicerca[] objListaFiltri, ref DocsPaWR.SearchPagingContext pagingContext)
        {
            DocsPaWR.InfoTrasmissione[] retValue = null;
            try
            {
                retValue = docsPaWS.GetInfoTrasmissioniFiltered(UserManager.GetInfoUser(), idDocOrFasc, docOrFasc, objListaFiltri, ref pagingContext);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return retValue;
        }

        //emilio Get Ragione trasmissione
        public static DocsPaWR.OrgRagioneTrasmissione GetRagioneTrasmissione(string id_ragione)
        {
            DocsPaWR.OrgRagioneTrasmissione retValue = null;
            try
            {
                retValue = docsPaWS.AmmGetRagioneTrasmissione(id_ragione);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// Set the selected Transmission id in session
        /// </summary>
        public static void setSelectedTransmissionId(string id)
        {
            try
            {
                RemoveSessionValue("selectedTransmissionId");
                SetSessionValue("selectedTransmissionId", id);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Return the selected Transmission id from session
        /// </summary>
        /// <returns>string</returns>
        public static string getSelectedTransmissionId()
        {
            try
            {
                return (string)GetSessionValue("selectedTransmissionId");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Trasmissione getSelectedTransmission()
        {
            try
            {
                return (Trasmissione)GetSessionValue("selectedTransmission");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void setSelectedTransmission(Trasmissione record)
        {
            try
            {
                RemoveSessionValue("selectedTransmission");
                SetSessionValue("selectedTransmission", record);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Reperimento trasmissione selezionata
        /// </summary>
        /// <returns></returns>
        public static DocsPaWR.Trasmissione GetTransmission(Page page, string typeObject)
        {
            try
            {
                // Reperimento id trasmissione selezionata
                string idTrasmissione = TrasmManager.getSelectedTransmissionId();
                DocsPaWR.Trasmissione trasmissioneSel = TrasmManager.GetTransmission(page, idTrasmissione, typeObject);
                TrasmManager.setSelectedTransmission(trasmissioneSel);
                return trasmissioneSel;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Trasmissione GetTransmission(Page page, string idTrasmissione, string typeObject)
        {
            try
            {
                Trasmissione retValue = null;

                if (!string.IsNullOrEmpty(idTrasmissione))
                {
                    DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPaWR.TrasmissioneOggettoTrasm();
                    if (typeObject.Equals("D"))
                    {
                        SchedaDocumento doc = DocumentManager.getSelectedRecord();
                        if (doc != null)
                            oggettoTrasm.infoDocumento = DocumentManager.getInfoDocumento(doc);
                    }
                    else
                    {
                        Fascicolo Prj = UIManager.ProjectManager.getProjectInSession();
                        if (Prj != null)
                            oggettoTrasm.infoFascicolo = ProjectManager.getInfoFascicoloDaFascicolo(Prj);
                    }

                    retValue = docsPaWS.GetTrasmissioneById(oggettoTrasm, UserManager.GetUserInSession(), UserManager.GetSelectedRole(), idTrasmissione);
                }

                return retValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Returns the details of the transmission starting with the id passed as a parameter of the single transmission
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idTrasmSing"></param>
        /// <param name="typeObject"></param>
        /// <param name="idObject"></param>
        /// <returns></returns>
        public static Trasmissione GetTransmissionByIdTrasmSing(Page page, string idTrasmSing, string typeObject, string idObject)
        {
            try
            {
                Trasmissione retValue = null;

                if (!string.IsNullOrEmpty(idTrasmSing))
                {
                    DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPaWR.TrasmissioneOggettoTrasm();
                    if (typeObject.Equals("D"))
                    {
                        SchedaDocumento doc = DocumentManager.getDocumentDetailsNoSecurity(page, idObject, idObject);
                        oggettoTrasm.infoDocumento = DocumentManager.getInfoDocumento(doc);
                    }
                    else if (typeObject.Equals("F"))
                    {
                        Fascicolo Prj = UIManager.ProjectManager.getFascicoloByIdNoSecurity(idObject);
                        oggettoTrasm.infoFascicolo = ProjectManager.getInfoFascicoloDaFascicolo(Prj);
                    }

                    retValue = docsPaWS.GetTrasmissioneByIdTrasmSing(oggettoTrasm, idTrasmSing, UserManager.GetUserInSession(), RoleManager.GetRoleInSession());
                }

                return retValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        // in theory no longer used
        /*
                public static Trasmissione[] getQueryEffettuateDocumentoPaging(Page page, TrasmissioneOggettoTrasm oggTrasm, Utente user, Ruolo ruolo, FiltroRicerca[] filterRic, int pageNumber, out int totalPageNumber, out int recordCount)
                {
                    totalPageNumber = 0;
                    recordCount = 0;

                    try
                    {
                        Trasmissione[] result = docsPaWS.TrasmissioneGetQueryEffettuateDocPaging(oggTrasm, filterRic, user, ruolo, pageNumber, out totalPageNumber, out recordCount);

                        if (result == null)
                        {
                            throw new Exception();
                        }

                        return result;
                    }
                    catch (Exception es)
                    {
                        ErrorManager.redirect(page, es);
                    }

                    return null;
                }

                public static Trasmissione[] getQueryRicevutePaging(Page page, TrasmissioneOggettoTrasm oggTrasm, Utente user, Ruolo ruolo, FiltroRicerca[] filterRic, int pageNumber, out int totalPageNumber, out int recordCount)
                {
                    totalPageNumber = 0;
                    recordCount = 0;

                    try
                    {
                        Trasmissione[] result = docsPaWS.TrasmissioneGetQueryRicevutePaging(oggTrasm, filterRic, user, ruolo, pageNumber, out totalPageNumber, out recordCount);

                        if (result == null)
                        {
                            throw new Exception();
                        }

                        return result;
                    }
                    catch (Exception es)
                    {
                        ErrorManager.redirect(page, es);
                    }

                    return null;
                }
        */

        public static FileDocumento getReportTrasm(Page page, DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm)
        {
            try
            {
                FileDocumento fileDoc = null;
                DocsPaWR.InfoUtente infoUt = infoUt = UserManager.GetInfoUser();

                int res = docsPaWS.ReportTrasmissioniDocFascUtente(oggettoTrasm, infoUt, out fileDoc);
                if (res != 0)
                {
                    throw new Exception("Si è verificato un errore nella creazione del report");
                }
                else
                {
                    if (res == 0 && fileDoc == null)
                    {
                        string msg = "Trasmissioni non trovate";
                        ScriptManager.RegisterStartupScript(page, page.GetType(), "ajaxDialogModal", "top.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', 'Stampa');", true);
                        return null;
                    }
                }
                return fileDoc;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Trasmissione executeTrasm(Page page, Trasmissione myTrasm)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                if (infoUtente.delegato != null)
                    myTrasm.delegato = infoUtente.delegato.idPeople;
                string server_path = utils.getHttpFullPath();
                Trasmissione result = docsPaWS.TrasmissioneExecuteTrasm(server_path, myTrasm, UserManager.GetInfoUser());

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void PerformAutomaticStateChange(Trasmissione trasmEff)
        {

            if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
            {
                Fascicolo Prj = UIManager.ProjectManager.getProjectInSession();
                DocsPaWR.Stato stato = DiagrammiManager.getStatoFasc(Prj.systemID);
                bool trasmWF = false;

                for (int i = 0; i < trasmEff.trasmissioniSingole.Length; i++)
                {
                    DocsPaWR.TrasmissioneSingola trasmSing = (DocsPaWR.TrasmissioneSingola)trasmEff.trasmissioniSingole[i];
                    if (trasmSing.ragione.tipo == "W") trasmWF = true;
                }

                if (stato != null && trasmWF)
                    DiagrammiManager.salvaStoricoTrasmDiagrammiFasc(trasmEff.systemId, Prj.systemID, Convert.ToString(stato.SYSTEM_ID));
            }

        }

        public static void PerformAutomaticStateChangeDoc(Trasmissione trasmEff)
        {

            if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
            {
                SchedaDocumento doc = UIManager.DocumentManager.getSelectedRecord();
                DocsPaWR.Stato stato = DiagrammiManager.GetStateDocument(doc.systemId);
                bool trasmWF = false;

                for (int i = 0; i < trasmEff.trasmissioniSingole.Length; i++)
                {
                    DocsPaWR.TrasmissioneSingola trasmSing = (DocsPaWR.TrasmissioneSingola)trasmEff.trasmissioniSingole[i];
                    if (trasmSing.ragione.tipo == "W") trasmWF = true;
                }

                if (stato != null && trasmWF)
                    DiagrammiManager.salvaStoricoTrasmDiagrammi(trasmEff.systemId, doc.systemId, Convert.ToString(stato.SYSTEM_ID));
            }

        }


        public static void salvaStoricoTrasmDiagrammi(string systemId, string docNumber, string SYSTEM_ID)
        {
            try
            {
                docsPaWS.salvaStoricoTrasmDiagrammi(systemId, docNumber, SYSTEM_ID);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static TemplateTrasmissione[] getTemplatesList(Page page, string tipoOggetto)
        {
            //tipo Oggetto: D=documento, F=fascicolo

            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();

                TemplateTrasmissione[] result = docsPaWS.TrasmissioneGetListaTemplate(infoUtente.idPeople, infoUtente.idCorrGlobali, tipoOggetto);

                if (result == null)
                {
                    throw new Exception();
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void SetDiagramStatusAndId(SchedaDocumento doc, string idAmm, ref string idDiagramma, ref string idStato)
        {
            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                {
                    DiagrammaStato dg = docsPaWS.getDgByIdTipoDoc(doc.tipologiaAtto.systemId, idAmm);
                    if (dg != null)
                    {
                        idDiagramma = dg.SYSTEM_ID.ToString();
                        Stato stato = docsPaWS.getStatoDoc(doc.docNumber);
                        if (stato != null) idStato = stato.SYSTEM_ID.ToString();
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static object[] getModelliPerTrasmLite(string idAmm, Registro[] registri, string idPeople, string idCorrGlobali, string idTipoDoc, string idDiagramma, string idStato, string typeDoc, string systemId, string idRuoloUtente, bool AllReg, string accessRights)
        {
            try
            {
                return docsPaWS.getModelliPerTrasmLite(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, "D", systemId, idRuoloUtente, AllReg, accessRights);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ModelloTrasmissione GetTemplateById(string idAmm, string templateId)
        {
            try
            {
                return docsPaWS.getModelloByID(idAmm, templateId);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool GetModelInheritsVisibility(string id)
        {
            try
            {
                return docsPaWS.ereditaVisibilita("null", id);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Passando il parametro idRegistro = null, viene automaticamente assegnato il primo registro
        /// utile del ruolo, con cui si sta creando il modello di tramissione.
        /// </summary>
        /// <param name="trasmissione"></param>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public static ModelloTrasmissione getModelloTrasmNuovo(Trasmissione trasmissione, string idRegistro)
        {
            try
            {
                DocsPaWR.ModelloTrasmissione modello = new ModelloTrasmissione();

                //MODELLO
                if (trasmissione.tipoOggetto.ToString() == "DOCUMENTO")
                    modello.CHA_TIPO_OGGETTO = "D";
                if (trasmissione.tipoOggetto.ToString() == "FASCICOLO")
                    modello.CHA_TIPO_OGGETTO = "F";

                modello.ID_AMM = trasmissione.ruolo.idAmministrazione;
                modello.ID_PEOPLE = trasmissione.utente.idPeople;
                modello.ID_REGISTRO = idRegistro;

                //Controllo che il registro non sia indefinito, 
                //in questo caso associo il primo registro utile del ruolo in questione
                if (modello.ID_REGISTRO == null)
                    modello.ID_REGISTRO = RoleManager.GetRoleInSession().registri[0].systemId;

                modello.NOME = trasmissione.noteGenerali;
                modello.SINGLE = "0";
                modello.VAR_NOTE_GENERALI = trasmissione.noteGenerali;

                //gestione della cessione dei diritti
                if (trasmissione.cessione != null && trasmissione.cessione.docCeduto)
                {
                    modello.CEDE_DIRITTI = "1";
                    if (trasmissione.cessione.idPeopleNewPropr != null && trasmissione.cessione.idPeopleNewPropr != "")
                        modello.ID_PEOPLE_NEW_OWNER = trasmissione.cessione.idPeopleNewPropr;
                    if (trasmissione.cessione.idRuoloNewPropr != null && trasmissione.cessione.idRuoloNewPropr != "")
                        modello.ID_GROUP_NEW_OWNER = trasmissione.cessione.idRuoloNewPropr;
                }
                else
                    modello.CEDE_DIRITTI = "0";


                //MITTENTE MODELLO
                modello.MITTENTE = new MittDest[1];
                modello.MITTENTE[0] = new MittDest();
                modello.MITTENTE[0].CHA_TIPO_MITT_DEST = "M";
                modello.MITTENTE[0].CHA_TIPO_URP = "P";
                modello.MITTENTE[0].DESCRIZIONE = trasmissione.ruolo.descrizione;
                modello.MITTENTE[0].ID_CORR_GLOBALI = Convert.ToInt32(trasmissione.ruolo.systemId);
                modello.MITTENTE[0].VAR_COD_RUBRICA = trasmissione.ruolo.codiceRubrica;

                //RAGIONI DESTINATARI MODELLO
                RagioneDest ragioneDest_1 = new RagioneDest();
                ArrayList trasmSingole = new ArrayList(trasmissione.trasmissioniSingole);

                if (trasmSingole.Count != 0)
                {
                    //Aggiungo la prima trasmissione singola
                    DocsPaWR.TrasmissioneSingola trasmSingola_1 = (TrasmissioneSingola)trasmSingole[0];
                    ragioneDest_1.CHA_TIPO_RAGIONE = trasmSingola_1.ragione.tipo;
                    ragioneDest_1.RAGIONE = trasmSingola_1.ragione.descrizione;

                    //Aggiungo il primo destinatario
                    MittDest dest_1 = new MittDest();

                    dest_1.CHA_TIPO_MITT_DEST = "D";
                    dest_1.CHA_TIPO_TRASM = trasmSingola_1.tipoTrasm;
                    if (trasmSingola_1.tipoDest.ToString() == "RUOLO")
                        dest_1.CHA_TIPO_URP = "R";
                    if (trasmSingola_1.tipoDest.ToString() == "UTENTE")
                        dest_1.CHA_TIPO_URP = "P";
                    dest_1.DESCRIZIONE = trasmSingola_1.corrispondenteInterno.descrizione;
                    dest_1.ID_CORR_GLOBALI = Convert.ToInt32(trasmSingola_1.corrispondenteInterno.systemId);
                    dest_1.ID_RAGIONE = Convert.ToInt32(trasmSingola_1.ragione.systemId);
                    dest_1.VAR_COD_RUBRICA = trasmSingola_1.corrispondenteInterno.codiceRubrica;
                    dest_1.VAR_NOTE_SING = trasmSingola_1.noteSingole;

                    // flag nascondi versioni precedenti
                    dest_1.NASCONDI_VERSIONI_PRECEDENTI = trasmSingola_1.hideDocumentPreviousVersions;

                    ////gestione utenti con notifica
                    dest_1.UTENTI_NOTIFICA = addNotificheUtenti(trasmSingola_1);

                    ragioneDest_1.DESTINATARI = utils.addToArrayMittDest(ragioneDest_1.DESTINATARI, dest_1);
                    modello.RAGIONI_DESTINATARI = utils.addToArrayRagioneDest(modello.RAGIONI_DESTINATARI, ragioneDest_1);

                    for (int i = 1; i < trasmissione.trasmissioniSingole.Length; i++)
                    {
                        TrasmissioneSingola trasmSingola_2 = (TrasmissioneSingola)trasmSingole[i];
                        int ragioneDestDaModificare = -1;

                        for (int j = 0; j < modello.RAGIONI_DESTINATARI.Length; j++)
                        {
                            DocsPaWR.RagioneDest ragioneDest_2 = (RagioneDest)modello.RAGIONI_DESTINATARI[j];
                            if (ragioneDest_2.RAGIONE == trasmSingola_2.ragione.descrizione)
                            {
                                ragioneDestDaModificare = j;
                                break;
                            }
                        }

                        if (ragioneDestDaModificare != -1)
                        {
                            //Aggiungo un destinatario ad una ragioneDest esistente
                            MittDest dest_2 = new MittDest();

                            dest_2.CHA_TIPO_MITT_DEST = "D";
                            dest_2.CHA_TIPO_TRASM = trasmSingola_2.tipoTrasm;
                            if (trasmSingola_2.tipoDest.ToString() == "RUOLO")
                                dest_2.CHA_TIPO_URP = "R";
                            if (trasmSingola_2.tipoDest.ToString() == "UTENTE")
                                dest_2.CHA_TIPO_URP = "U";
                            dest_2.DESCRIZIONE = trasmSingola_2.corrispondenteInterno.descrizione;
                            dest_2.ID_CORR_GLOBALI = Convert.ToInt32(trasmSingola_2.corrispondenteInterno.systemId);
                            dest_2.ID_RAGIONE = Convert.ToInt32(trasmSingola_2.ragione.systemId);
                            dest_2.VAR_COD_RUBRICA = trasmSingola_2.corrispondenteInterno.codiceRubrica;
                            dest_2.VAR_NOTE_SING = trasmSingola_2.noteSingole;

                            // flag nascondi versioni precedenti
                            dest_1.NASCONDI_VERSIONI_PRECEDENTI = trasmSingola_2.hideDocumentPreviousVersions;

                            ////gestione utenti con notifica
                            dest_2.UTENTI_NOTIFICA = addNotificheUtenti(trasmSingola_2);

                            ((RagioneDest)modello.RAGIONI_DESTINATARI[ragioneDestDaModificare]).DESTINATARI = utils.addToArrayMittDest(((RagioneDest)modello.RAGIONI_DESTINATARI[ragioneDestDaModificare]).DESTINATARI, dest_2);
                        }
                        else
                        {
                            //Aggiungo una nuova ragioneDest
                            RagioneDest ragioneDest_3 = new RagioneDest();

                            ragioneDest_3.CHA_TIPO_RAGIONE = trasmSingola_2.ragione.tipo;
                            ragioneDest_3.RAGIONE = trasmSingola_2.ragione.descrizione;

                            MittDest dest_3 = new MittDest();

                            dest_3.CHA_TIPO_MITT_DEST = "D";
                            dest_3.CHA_TIPO_TRASM = trasmSingola_2.tipoTrasm;
                            if (trasmSingola_2.tipoDest.ToString() == "RUOLO")
                                dest_3.CHA_TIPO_URP = "R";
                            if (trasmSingola_2.tipoDest.ToString() == "UTENTE")
                                dest_3.CHA_TIPO_URP = "U";
                            dest_3.DESCRIZIONE = trasmSingola_2.corrispondenteInterno.descrizione;
                            dest_3.ID_CORR_GLOBALI = Convert.ToInt32(trasmSingola_2.corrispondenteInterno.systemId);
                            dest_3.ID_RAGIONE = Convert.ToInt32(trasmSingola_2.ragione.systemId);
                            dest_3.VAR_COD_RUBRICA = trasmSingola_2.corrispondenteInterno.codiceRubrica;
                            dest_3.VAR_NOTE_SING = trasmSingola_2.noteSingole;

                            // flag nascondi versioni precedenti
                            dest_3.NASCONDI_VERSIONI_PRECEDENTI = trasmSingola_2.hideDocumentPreviousVersions;

                            ////gestione utenti con notifica
                            dest_3.UTENTI_NOTIFICA = addNotificheUtenti(trasmSingola_2);

                            ragioneDest_3.DESTINATARI = utils.addToArrayMittDest(ragioneDest_3.DESTINATARI, dest_3);
                            modello.RAGIONI_DESTINATARI = utils.addToArrayRagioneDest(modello.RAGIONI_DESTINATARI, ragioneDest_3);
                        }

                    }
                }

                return modello;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        private static UtentiConNotificaTrasm[] addNotificheUtenti(TrasmissioneSingola trasmSingola)
        {
            try
            {
                int contaU = 0;

                UtentiConNotificaTrasm[] listaUtentiNotifica = new UtentiConNotificaTrasm[trasmSingola.trasmissioneUtente.Length];

                foreach (TrasmissioneUtente trasmU in trasmSingola.trasmissioneUtente)
                {
                    listaUtentiNotifica[contaU] = new UtentiConNotificaTrasm();
                    listaUtentiNotifica[contaU].ID_PEOPLE = trasmU.utente.idPeople;
                    listaUtentiNotifica[contaU].CODICE_UTENTE = trasmU.utente.codiceRubrica;
                    if (trasmU.daNotificare)
                        listaUtentiNotifica[contaU].FLAG_NOTIFICA = "1";
                    else
                        listaUtentiNotifica[contaU].FLAG_NOTIFICA = "0";
                    contaU++;
                }
                return listaUtentiNotifica;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Corrispondente getCorrispondenti(string tipo_destinatario, SchedaDocumento schedaDocumento, Fascicolo fascicolo, Page page)
        {
            try
            {
                DocsPaWR.Corrispondente corr = new Corrispondente();
                //se la il modello di trasmissione ha come destinatario l'utente proprietario del documento
                if (schedaDocumento != null)
                {
                    if (tipo_destinatario == "UT_P")
                    {
                        string utenteProprietario = string.Empty;
                        if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                        {
                            //caso predispsosto con ruolo creatore diverso da protocollatore:
                            if (schedaDocumento.creatoreDocumento != null)
                            {
                                utenteProprietario = schedaDocumento.creatoreDocumento.idPeople;
                            }
                            else utenteProprietario = schedaDocumento.protocollatore.utente_idPeople;

                        }
                        else
                        {
                            utenteProprietario = schedaDocumento.creatoreDocumento.idPeople;
                        }
                        corr = UserManager.getCorrispondenteByIdPeople(page, utenteProprietario, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    //ruolo proprietario del documento
                    if (tipo_destinatario == "R_P")
                    {
                        string idCorrGlobaliRuolo = string.Empty;
                        if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                        {
                            //caso predispsosto con ruolo creatore diverso da protocollatore:
                            if (schedaDocumento.creatoreDocumento != null)
                            {
                                idCorrGlobaliRuolo = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                            }
                            else
                                idCorrGlobaliRuolo = schedaDocumento.protocollatore.ruolo_idCorrGlobali;
                        }
                        else
                        {
                            idCorrGlobaliRuolo = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                        }
                        // corr = UserManager.getCorrispondenteBySystemID(page, idCorrGlobaliRuolo);
                        corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuolo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    //trasmissione a UO del proprietario
                    if (tipo_destinatario == "UO_P")
                    {
                        string idCorrGlobaliUo = string.Empty;
                        if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                        {
                            //caso predispsosto con ruolo creatore diverso da protocollatore:
                            if (schedaDocumento.creatoreDocumento != null)
                            {
                                idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                            }
                            else
                                idCorrGlobaliUo = schedaDocumento.protocollatore.uo_idCorrGlobali;
                        }
                        else
                        {
                            idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                        }
                        corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliUo, DocsPaWR.AddressbookTipoUtente.INTERNO);

                    }//RUOLO Responsabile UO proprietario
                    if (tipo_destinatario == "RSP_P")
                    {
                        string idCorrGlobaliUo = string.Empty;
                        string idCorr = string.Empty;
                        if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                        {
                            //caso predispsosto con ruolo creatore diverso da protocollatore:
                            if (schedaDocumento.creatoreDocumento != null)
                            {
                                idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                                //idCorr = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                            }
                            else
                            {
                                idCorrGlobaliUo = schedaDocumento.protocollatore.uo_idCorrGlobali;
                                //idCorr = schedaDocumento.protocollatore.ruolo_idCorrGlobali;
                            }
                        }
                        else
                        {
                            idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                            //idCorr = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                        }
                        idCorr = UserManager.GetSelectedRole().systemId;
                        string idCorrGlobaliRuoloRespUo = UserManager.getRuoloRespUoFromUo(page, idCorrGlobaliUo, "R", idCorr);

                        if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                        {
                            corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuoloRespUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                        }
                        else
                        {
                            corr = null;
                        }
                    }
                    //Ruolo segretario UO PROPRIETARIO
                    if (tipo_destinatario == "R_S")
                    {
                        string idCorrGlobaliUo = string.Empty;
                        string idCorr = String.Empty;
                        if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                        {
                            //caso predispsosto con ruolo creatore diverso da protocollatore:
                            if (schedaDocumento.creatoreDocumento != null)
                            {
                                idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                                // idCorr = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                            }
                            else
                            {
                                idCorrGlobaliUo = schedaDocumento.protocollatore.uo_idCorrGlobali;
                                // idCorr = schedaDocumento.protocollatore.ruolo_idCorrGlobali;
                            }
                        }
                        else
                        {
                            idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                            //idCorr = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                        }
                        idCorr = UserManager.GetSelectedRole().systemId;
                        string idCorrGlobaliRuoloRespUo = UserManager.getRuoloRespUoFromUo(page, idCorrGlobaliUo, "S", idCorr);

                        if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                        {
                            corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuoloRespUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                        }
                        else
                        {
                            corr = null;
                        }
                    }
                    //ruolo responsabile uo mittente
                    if (tipo_destinatario == "RSP_M")
                    {
                        string idCorrGlobaliUo = UserManager.GetSelectedRole().uo.systemId;
                        string idCorr = UserManager.GetSelectedRole().systemId;

                        string idCorrGlobaliRuoloRespUo = UserManager.getRuoloRespUoFromUo(page, idCorrGlobaliUo, "R", idCorr);

                        if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                        {
                            corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuoloRespUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                        }
                        else
                        {
                            corr = null;
                        }
                    }

                    //ruolo segretario uo mittente
                    if (tipo_destinatario == "S_M")
                    {
                        string idCorrGlobaliUo = UserManager.GetSelectedRole().uo.systemId;
                        string idCorr = UserManager.GetSelectedRole().systemId;

                        string idCorrGlobaliRuoloRespUo = UserManager.getRuoloRespUoFromUo(page, idCorrGlobaliUo, "S", idCorr);

                        if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                        {
                            corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuoloRespUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                        }
                        else
                        {
                            corr = null;
                        }
                    }
                }
                else
                {
                    //trsmissione a utente proprietario del documento
                    if (tipo_destinatario == "UT_P")
                    {
                        string utenteProprietario = string.Empty;
                        utenteProprietario = fascicolo.creatoreFascicolo.idPeople;
                        corr = UserManager.getCorrispondenteByIdPeople(page, utenteProprietario, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    //ruolo proprietario
                    if (tipo_destinatario == "R_P")
                    {
                        string idCorrGlobaliRuolo = string.Empty;
                        idCorrGlobaliRuolo = fascicolo.creatoreFascicolo.idCorrGlob_Ruolo;
                        corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuolo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    //uo proprietaria
                    if (tipo_destinatario == "UO_P")
                    {
                        string idCorrGlobaliUo = string.Empty;
                        idCorrGlobaliUo = fascicolo.creatoreFascicolo.idCorrGlob_UO;
                        corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    //responsabile uo proprietario
                    if (tipo_destinatario == "RSP_P")
                    {
                        string idCorrGlobaliUo = string.Empty;
                        string idCorr = string.Empty;

                        idCorrGlobaliUo = fascicolo.creatoreFascicolo.idCorrGlob_UO;
                        idCorr = fascicolo.creatoreFascicolo.idCorrGlob_Ruolo;
                        string idCorrGlobaliRuoloRespUo = UserManager.getRuoloRespUoFromUo(page, idCorrGlobaliUo, "R", idCorr);

                        if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                        {
                            corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuoloRespUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                        }
                        else
                        {
                            corr = null;
                        }
                    }
                    //ruolo segretario uo del proprietario
                    if (tipo_destinatario == "R_S")
                    {
                        string idCorrGlobaliUo = string.Empty;
                        string idCorr = string.Empty;

                        idCorrGlobaliUo = fascicolo.creatoreFascicolo.idCorrGlob_UO;
                        idCorr = fascicolo.creatoreFascicolo.idCorrGlob_Ruolo;

                        string idCorrGlobaliRuoloRespUo = UserManager.getRuoloRespUoFromUo(page, idCorrGlobaliUo, "S", idCorr);

                        if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                        {
                            corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuoloRespUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                        }
                        else
                        {
                            corr = null;
                        }
                    }
                    //ruolo responsabile uo del mittente
                    if (tipo_destinatario == "RSP_M")
                    {
                        string idCorrGlobaliUo = UserManager.GetSelectedRole().uo.systemId;
                        string idCorr = UserManager.GetSelectedRole().systemId;

                        string idCorrGlobaliRuoloRespUo = UserManager.getRuoloRespUoFromUo(page, idCorrGlobaliUo, "R", idCorr);

                        if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                        {
                            corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuoloRespUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                        }
                        else
                        {
                            corr = null;
                        }
                    }
                    //RUOLO SEGRETARIO UO DEL MITTENTE
                    if (tipo_destinatario == "S_M")
                    {
                        string idCorrGlobaliUo = UserManager.GetSelectedRole().uo.systemId;
                        string idCorr = UserManager.GetSelectedRole().systemId;

                        string idCorrGlobaliRuoloRespUo = UserManager.getRuoloRespUoFromUo(page, idCorrGlobaliUo, "S", idCorr);

                        if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                        {
                            corr = UserManager.getCorrispondenteCompletoBySystemId(page, idCorrGlobaliRuoloRespUo, DocsPaWR.AddressbookTipoUtente.INTERNO);
                        }
                        else
                        {
                            corr = null;
                        }
                    }

                }
                return corr;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static RagioneTrasmissione getRagioneById(string id)
        {
            try
            {
                return docsPaWS.getRagioneById(id);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static TrasmissioneUtente[] addTrasmissioneUtente(TrasmissioneUtente[] array, TrasmissioneUtente nuovoElemento)
        {
            try
            {
                TrasmissioneUtente[] nuovaLista;
                if (array != null)
                {
                    int len = array.Length;
                    nuovaLista = new TrasmissioneUtente[len + 1];
                    array.CopyTo(nuovaLista, 0);
                    nuovaLista[len] = nuovoElemento;
                    return nuovaLista;
                }
                else
                {
                    nuovaLista = new TrasmissioneUtente[1];
                    nuovaLista[0] = nuovoElemento;
                    return nuovaLista;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static TrasmissioneSingola[] addTrasmissioneSingola(TrasmissioneSingola[] array, TrasmissioneSingola nuovoElemento)
        {
            try
            {
                TrasmissioneSingola[] nuovaLista;
                if (array != null)
                {
                    int len = array.Length;
                    nuovaLista = new TrasmissioneSingola[len + 1];
                    array.CopyTo(nuovaLista, 0);
                    nuovaLista[len] = nuovoElemento;
                    return nuovaLista;
                }
                else
                {
                    nuovaLista = new TrasmissioneSingola[1];
                    nuovaLista[0] = nuovoElemento;
                    return nuovaLista;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Trasmissione saveExecuteTrasm(Page page, Trasmissione trasmissione, DocsPaWR.InfoUtente infoUtente)
        {
            Trasmissione result = null;
            try
            {
                string server_path = utils.getHttpFullPath();

                if (infoUtente.delegato != null)
                    trasmissione.delegato = infoUtente.delegato.idPeople;
                docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                result = docsPaWS.TrasmissioneSaveExecuteTrasm(server_path, trasmissione, infoUtente);
                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        public static bool executeAccRif(Page page, TrasmissioneUtente trasmissioneUtente, string idTrasmissione, Fascicolo fasc, out string errore)
        {
            bool result = true;
            errore = string.Empty;

            try
            {
                if (fasc != null && !string.IsNullOrEmpty(fasc.systemID))
                    result = docsPaWS.TrasmissioneExecuteAccRifConFascicolazione(trasmissioneUtente, idTrasmissione, UserManager.GetSelectedRole(), UserManager.GetInfoUser(), fasc, out errore);
                else
                    result = docsPaWS.TrasmissioneExecuteAccRif(trasmissioneUtente, idTrasmissione, UserManager.GetSelectedRole(), UserManager.GetInfoUser(), out errore);
                if (result)
                {
                    // se è un rifiuto...
                    if (trasmissioneUtente.tipoRisposta.ToString().ToUpper().Equals("RIFIUTO"))
                    {
                        InfoUtente infoUtente = UserManager.GetInfoUser();

                        //il nuovo centro notifiche non prevede una nuova trasmissione per evidenziare il rifiuto
                        /*
                        // ...allora la trasmissione torna al mittente
                        if (!docsPaWS.RitornaAlMittTrasmUt(trasmissioneUtente, infoUtente))
                        {
                            //throw new Exception();
                            result = false;
                        }*/
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        /// <summary>
        /// Tramissione di un documento usando un modello di trasmissione
        /// Il parametro "idStato" puo' essere null o meno a seconda delle necessità
        /// </summary>
        /// <returns></returns>
        public static void effettuaTrasmissioneDocDaModello(DocsPaWR.ModelloTrasmissione modello, string idStato, InfoDocumento infoDocumento, Page page)
        {
            try
            {
                Trasmissione trasmissione = new DocsPaWR.Trasmissione();

                //Parametri della trasmissione
                trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;

                trasmissione.tipoOggetto = DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
                trasmissione.infoDocumento = infoDocumento;
                trasmissione.utente = UserManager.GetUserInSession();
                trasmissione.ruolo = UserManager.GetSelectedRole();
                if (modello != null)
                    trasmissione.NO_NOTIFY = modello.NO_NOTIFY;

                //Parametri delle trasmissioni singole
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                {

                    DocsPaWR.RagioneDest ragDest = (DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {
                        DocsPaWR.MittDest mittDest = (DocsPaWR.MittDest)destinatari[j];
                        DocsPaWR.Corrispondente corr = new Corrispondente();
                        if (mittDest.CHA_TIPO_MITT_DEST == "D")
                        {
                            corr = UserManager.getCorrispondenteByCodRubricaIE(page, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                        }
                        else
                        {
                            DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentDetails(page, infoDocumento.idProfile, infoDocumento.docNumber);
                            corr = getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, schedaDoc, null, page);
                        }
                        if (corr != null)
                        {
                            DocsPaWR.RagioneTrasmissione ragione = docsPaWS.getRagioneById(mittDest.ID_RAGIONE.ToString());
                            trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, page);
                        }
                    }
                }
                trasmissione = TrasmManager.impostaNotificheUtentiDaModello(trasmissione, modello);

                //
                // Aggiunto codice mancante per segnalazione Zanotti
                if (trasmissione != null && modello.CEDE_DIRITTI.Equals("1"))
                {

                    if (trasmissione.cessione == null)
                    {
                        DocsPaWR.CessioneDocumento cessione = new DocsPaWR.CessioneDocumento();
                        cessione.docCeduto = true;
                        cessione.idPeople = UserManager.GetInfoUser().idPeople;
                        cessione.idRuolo = UserManager.GetInfoUser().idGruppo;
                        cessione.userId = UserManager.GetInfoUser().userId;
                        cessione.idPeopleNewPropr = modello.ID_PEOPLE_NEW_OWNER;
                        cessione.idRuoloNewPropr = modello.ID_GROUP_NEW_OWNER;
                        trasmissione.cessione = cessione;
                    }
                }
                //
                // End Aggiunta codice per segnalazione Zanotti


                trasmissione = TrasmManager.saveExecuteTrasm(page, trasmissione, UIManager.UserManager.GetInfoUser());
                //trasmissione = TrasmManager.saveTrasm(page, trasmissione);
                //TrasmManager.executeTrasm(page, trasmissione);
                if (idStato != null && idStato != "")
                    DiagrammiManager.salvaStoricoTrasmDiagrammiFasc(trasmissione.systemId, infoDocumento.docNumber, idStato);

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Tramissione di un fascicolo usando un modello di trasmissione
        /// Il parametro "idStato" puo' essere null o meno a seconda delle necessità
        /// </summary>
        /// <returns></returns>
        public static void effettuaTrasmissioneFascDaModello(DocsPaWR.ModelloTrasmissione modello, string idStato, Fascicolo fascicolo, Page page)
        {
            try
            {
                Trasmissione trasmissione = new DocsPaWR.Trasmissione();
                DocsPaWR.DocsPaWebService wws = new DocsPaWebService();

                //Parametri della trasmissione
                trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;

                trasmissione.tipoOggetto = DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
                trasmissione.infoFascicolo = ProjectManager.getInfoFascicoloDaFascicolo(fascicolo);
                trasmissione.utente = UserManager.GetUserInSession();
                trasmissione.ruolo = RoleManager.GetRoleInSession();
                if (modello != null)
                    trasmissione.NO_NOTIFY = modello.NO_NOTIFY;
                //Parametri delle trasmissioni singole
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                {

                    DocsPaWR.RagioneDest ragDest = (DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {
                        DocsPaWR.MittDest mittDest = (DocsPaWR.MittDest)destinatari[j];
                        DocsPaWR.Corrispondente corr = new Corrispondente();
                        if (mittDest.CHA_TIPO_MITT_DEST == "D")
                        {
                            corr = UserManager.getCorrispondenteByCodRubricaIE(page, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                        }
                        else
                        {
                            corr = getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, null, fascicolo, page);
                        }
                        if (corr != null)
                        {
                            DocsPaWR.RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());
                            trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, page);
                        }
                    }
                }


                trasmissione = TrasmManager.impostaNotificheUtentiDaModello(trasmissione, modello);

                //
                // Aggiunto codice mancante per segnalazione Zanotti
                if (trasmissione != null && modello.CEDE_DIRITTI.Equals("1"))
                {

                    if (trasmissione.cessione == null)
                    {
                        DocsPaWR.CessioneDocumento cessione = new DocsPaWR.CessioneDocumento();
                        cessione.docCeduto = true;
                        cessione.idPeople = UserManager.GetInfoUser().idPeople;
                        cessione.idRuolo = UserManager.GetInfoUser().idGruppo;
                        cessione.userId = UserManager.GetInfoUser().userId;
                        cessione.idPeopleNewPropr = modello.ID_PEOPLE_NEW_OWNER;
                        cessione.idRuoloNewPropr = modello.ID_GROUP_NEW_OWNER;
                        trasmissione.cessione = cessione;
                    }
                }
                //
                // End Aggiunta codice per segnalazione Zanotti

                trasmissione = TrasmManager.saveExecuteTrasm(page, trasmissione, UIManager.UserManager.GetInfoUser());
                // TrasmManager.executeTrasm(page, trasmissione);
                if (idStato != null && idStato != "")
                    DiagrammiManager.salvaStoricoTrasmDiagrammiFasc(trasmissione.systemId, fascicolo.systemID, idStato);

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static Trasmissione saveTrasm(Page page, Trasmissione trasmissione)
        {
            Trasmissione result = null;

            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                if (infoUtente.delegato != null)
                    trasmissione.delegato = infoUtente.delegato.idPeople;
                result = docsPaWS.TrasmissioneSaveTrasm(trasmissione);

                if (result == null)
                    throw new Exception();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        private static DocsPaWR.Trasmissione impostaNotificheUtentiDaModello(DocsPaWR.Trasmissione objTrasm, DocsPaWR.ModelloTrasmissione modello)
        {
            try
            {
                if (objTrasm.trasmissioniSingole != null && objTrasm.trasmissioniSingole.Length > 0)
                {
                    for (int cts = 0; cts < objTrasm.trasmissioniSingole.Length; cts++)
                    {
                        if (objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length > 0)
                        {
                            for (int ctu = 0; ctu < objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length; ctu++)
                            {
                                objTrasm.trasmissioniSingole[cts].trasmissioneUtente[ctu].daNotificare = TrasmManager.daNotificareSuModello(objTrasm.trasmissioniSingole[cts].trasmissioneUtente[ctu].utente.idPeople, objTrasm.trasmissioniSingole[cts].corrispondenteInterno.systemId, modello);
                            }
                        }
                    }
                }
                return objTrasm;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        private static bool daNotificareSuModello(string currentIDPeople, string currentIDCorrGlobRuolo, DocsPaWR.ModelloTrasmissione modello)
        {
            bool retValue = true;
            try
            {
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                {
                    DocsPaWR.RagioneDest ragDest = (DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {
                        DocsPaWR.MittDest mittDest = (DocsPaWR.MittDest)destinatari[j];
                        if (mittDest.ID_CORR_GLOBALI.Equals(Convert.ToInt32(currentIDCorrGlobRuolo)))
                        {
                            if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length > 0)
                            {
                                for (int cut = 0; cut < mittDest.UTENTI_NOTIFICA.Length; cut++)
                                {
                                    if (mittDest.UTENTI_NOTIFICA[cut].ID_PEOPLE.Equals(currentIDPeople))
                                    {
                                        if (mittDest.UTENTI_NOTIFICA[cut].FLAG_NOTIFICA.Equals("1"))
                                            retValue = true;
                                        else
                                            retValue = false;

                                        return retValue;
                                    }
                                }
                            }
                        }
                    }
                }
                return retValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPaWR.Trasmissione trasmissione, DocsPaWR.Corrispondente corr, DocsPaWR.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, Page page)
        {
            try
            {
                return addTrasmissioneSingola(trasmissione, corr, ragione, note, tipoTrasm, scadenza, false, page);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPaWR.Trasmissione trasmissione, DocsPaWR.Corrispondente corr, DocsPaWR.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, bool nascondiVersioniPrecedenti, Page page)
        {
            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                {
                    DocsPaWR.TrasmissioneSingola ts = (DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                    if (ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                    {
                        if (ts.daEliminare)
                        {
                            ((DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                            return trasmissione;
                        }
                        else
                            return trasmissione;
                    }
                }
            }

            //Quando la ragione di trasmissione ha mantieni lettura, anche la trasmissione deve mantenere la lettura
            if (ragione != null && !string.IsNullOrEmpty(ragione.mantieniLettura) && ragione.mantieniLettura.Equals("1"))
            {
                if (trasmissione != null)
                {
                    trasmissione.mantieniLettura = true;
                }
            }

            // Mev Cessione Diritti - Mantieni Scrittura
            if (ragione != null && !string.IsNullOrEmpty(ragione.mantieniScrittura) && ragione.mantieniScrittura.Equals("1"))
            {
                if (trasmissione != null)
                {
                    trasmissione.mantieniScrittura = true;
                }
            }
            // End Mev


            // Aggiungo la trasmissione singola
            DocsPaWR.TrasmissioneSingola trasmissioneSingola = new DocsPaWR.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = tipoTrasm;
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragione;
            trasmissioneSingola.noteSingole = note;
            trasmissioneSingola.hideDocumentPreviousVersions = nascondiVersioniPrecedenti;

            //Imposto la data di scadenza
            if (scadenza > 0)
            {
                string dataScadenza = "";
                System.DateTime data = System.DateTime.Now.AddDays(scadenza);
                dataScadenza = data.Day + "/" + data.Month + "/" + data.Year;
                trasmissioneSingola.dataScadenza = dataScadenza;
            }

            // Aggiungo la lista di trasmissioniUtente
            if (corr is DocsPaWR.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
                DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr, page);
                if (listaUtenti.Length == 0)
                {
                    trasmissioneSingola = null;

                    //Andrea
                    //throw new ExceptionTrasmissioni("Non è presente alcun utente per la Trasmissione al ruolo: "
                    //                                + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                    //                                + ".");
                    //End Andrea
                }
                else
                {
                    //ciclo per utenti se dest è gruppo o ruolo
                    for (int i = 0; i < listaUtenti.Length; i++)
                    {
                        DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPaWR.TrasmissioneUtente();
                        trasmissioneUtente.utente = (DocsPaWR.Utente)listaUtenti[i];
                        trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                    }
                }
            }

            if (corr is DocsPaWR.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
                DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPaWR.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPaWR.Utente)corr;

                //Andrea
                if (trasmissioneUtente.utente == null)
                {
                    //throw new ExceptionTrasmissioni("L utente: " + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                    //                                + " è inesistente.");

                }
                //End Andrea
                else
                    trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
            }

            if (corr is DocsPaWR.UnitaOrganizzativa)
            {
                DocsPaWR.UnitaOrganizzativa theUo = (DocsPaWR.UnitaOrganizzativa)corr;
                DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = new AddressbookQueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                qca.ruolo = UserManager.GetSelectedRole();
                qca.queryCorrispondente = new AddressbookQueryCorrispondente();
                qca.queryCorrispondente.fineValidita = true;

                DocsPaWR.Ruolo[] ruoli = UserManager.getRuoliRiferimentoAutorizzati(page, qca, theUo);

                //Andrea
                if (ruoli == null || ruoli.Length == 0)
                {
                    //throw new ExceptionTrasmissioni("Manca un ruolo di riferimento per la UO: "
                    //                                + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                    //                                + ".");
                }
                //End Andrea
                else
                {
                    foreach (DocsPaWR.Ruolo r in ruoli)
                        trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm, scadenza, nascondiVersioniPrecedenti, page);
                }
                return trasmissione;
            }

            if (trasmissioneSingola != null)
                trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);

            return trasmissione;
        }

        private static DocsPaWR.Corrispondente[] queryUtenti(DocsPaWR.Corrispondente corr, Page page)
        {
            try
            {
                //costruzione oggetto queryCorrispondente
                DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPaWR.AddressbookQueryCorrispondente();
                qco.codiceRubrica = corr.codiceRubrica;
                qco.getChildren = true;
                qco.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                qco.fineValidita = true;

                //corrispondenti interni
                qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;
                return UserManager.getListaCorrispondenti(page, qco);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Trasmissione addTrasmDaTemplate(Page page, InfoDocumento infoDocumento, DocsPaWR.ModelloTrasmissione template, InfoUtente infoUtente)
        {
            try
            {
                DocsPaWR.Utente utente = UserManager.GetUserInSession();
                DocsPaWR.Ruolo ruolo = UserManager.GetSelectedRole();
                Trasmissione trasmissione = new Trasmissione();

                return trasmissione;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Trasmissione addTrasmDaTemplate(Page page, InfoDocumento infoDocumento, DocsPaWR.TemplateTrasmissione template, InfoUtente infoUtente)
        {
            try
            {
                DocsPaWR.Utente utente = UserManager.GetUserInSession();
                DocsPaWR.Ruolo ruolo = UserManager.GetSelectedRole();
                Trasmissione result;
                result = docsPaWS.TrasmissioneAddDaTempl(infoDocumento, template, utente, ruolo);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static TemplateTrasmissione[] getListaTemplate(Page page, Utente user, Ruolo ruolo, string tipoOggetto)
        {
            //tipo Oggetto: D=documento, F=fascicolo

            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();

                TemplateTrasmissione[] result = docsPaWS.TrasmissioneGetListaTemplate(infoUtente.idPeople, infoUtente.idCorrGlobali, tipoOggetto);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.TrasmissioneUtente[] getTrasmissioneUtenteInRuolo(DocsPaWR.InfoUtente infoUtente, string systemId)
        {
            try
            {
                return docsPaWS.getTrasmissioneUtenteInRuolo(infoUtente, systemId);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static TrasmissioneSingola RoleTransmissionWithHistoricized(List<TrasmissioneSingola> singleTransmissions, String roleCorrGlob)
        {
            try
            {
                TrasmissioneSingola retVal = null;
                // Se la lista di trasmissioni singole contiene un ruolo con id pari a quello passato, è stato
                // richiesto il dettaglio della trasmissione da un ruolo attivo, altrimenti bisogna restituire la
                // prima trasmissione utente che contiene uno dei ruoli padri del ruolo attuale.
                // Questo controllo andrebbe in realtà fatto da backend ma attualmente tutte le trasmissioni vengono gestite da
                // frontend
                retVal = singleTransmissions.Where(tu => tu.corrispondenteInterno.systemId == roleCorrGlob).FirstOrDefault();
                if (retVal == null)
                {
                    RoleChainResponse roleChain = docsPaWS.GetRoleChainsId(new RoleChainRequest() { IdCorrGlobRole = roleCorrGlob });
                    foreach (var id in roleChain.RoleChain)
                    {
                        retVal = singleTransmissions.Where(tu => tu.corrispondenteInterno.systemId == id).FirstOrDefault();
                        if (retVal != null) break;
                    }
                }

                return retVal;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool checkTrasm_UNO_TUTTI_AccettataRifiutata(DocsPaWR.TrasmissioneSingola trasmSingola)
        {
            try
            {
                return docsPaWS.checkTrasm_UNO_TUTTI_AccettataRifiutata(trasmSingola);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool getIfDocOrFascIsInToDoList(InfoUtente infoUtente, string systemId)
        {
            try
            {
                return docsPaWS.getIfDocOrFascIsInToDoList(infoUtente, systemId);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static RagioneTrasmissione[] getListaRagioni(Page page, string accessRights, bool flgDaRicercaTrasm)
        {
            try
            {
                DocsPaWR.TrasmissioneDiritti diritti = new DocsPaWR.TrasmissioneDiritti();
                if (!string.IsNullOrEmpty(accessRights))
                    diritti.accessRights = accessRights;

                diritti.idAmministrazione = UserManager.GetUserInSession().idAmministrazione;
                RagioneTrasmissione[] result = docsPaWS.TrasmissioneGetRagioni(diritti, flgDaRicercaTrasm);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static RagioneTrasmissione getReasonById(string id)
        {
            try
            {
                return docsPaWS.getRagioneById(id);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Restituisce la ragione di trasmissione NOTIFICA
        /// </summary>
        /// <param name="idAmm"> Id dell'amministrazione corrente</param>
        /// <returns></returns>
        public static RagioneTrasmissione GetReasonNotify(string idAmm)
        {
            RagioneTrasmissione ragione = docsPaWS.GetRagioneNotifica(idAmm);
            return ragione;
        }

        public static bool IsEnabledRF()
        {
            try
            {
                return docsPaWS.IsEnabledRF(string.Empty);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Ritorna il valore della chiav edi web.config, che gestisce
        /// se gli utenti di un ruolo destinatario di una trasmissione devono comparire di default
        /// flaggati o no. nome = UT_TX_RUOLO_CHECKED. Se la chiave non esiste ritorna TRUE condizione di default in DocsPA, quindi TUTTI
        /// gli utenti compaioni checked.
        /// </summary>
        /// <returns></returns>
        public static bool getTxRuoloUtentiChecked()
        {
            try
            {
                string value = System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.UT_TX_RUOLO_CHECKED.ToString()];
                if (value != null && value.ToLower() == "false")
                    return false;
                else
                    return true;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static Trasmissione removeTrasmSingola(Trasmissione trasmissione, int i, bool setDaEliminare)
        {
            ArrayList lista = new ArrayList();
            try
            {
                if (setDaEliminare)
                    trasmissione.trasmissioniSingole[i].daEliminare = true;
                else
                {
                    DocsPaWR.TrasmissioneSingola[] listaTrasmSingole = new DocsPaWR.TrasmissioneSingola[trasmissione.trasmissioniSingole.Length - 1];
                    int indice = 0;
                    for (int j = 0; j < trasmissione.trasmissioniSingole.Length; j++)
                    {
                        if (!j.Equals(i))
                        {
                            listaTrasmSingole[indice] = trasmissione.trasmissioniSingole[j];
                            indice++;
                        }
                    }
                    trasmissione.trasmissioniSingole = listaTrasmSingole;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return trasmissione;
        }

        public static bool deleteTrasm(Trasmissione trasmissione)
        {
            bool result;

            try
            {
                result = docsPaWS.TrasmissioniDeleteTrasmissione(trasmissione);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }

            return result;
        }

        public static void SelectSecurity(string IDObject, string idPeople, string boh, out string accessRights, out string idGruppoTrasm, out string tipoDiritto)
        {
            accessRights = string.Empty;
            idGruppoTrasm = string.Empty;
            tipoDiritto = string.Empty;
            try
            {
                docsPaWS.SelectSecurity(IDObject, idPeople, boh, out accessRights, out idGruppoTrasm, out tipoDiritto);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void setHashRagioneTrasmissione(Page page, Hashtable hash)
        {
            page.Session["m_hashTableRagioneTrasmissione"] = hash;
        }

        /// <summary>
        /// NELLA CONFIGURAZIONE: set_DATA_VISTA_GRD=2, Toglie la trasmissione dalla TDL, ma non setta la DATA VISTA.
        /// </summary>
        /// <param name="infoutente"></param>
        /// <param name="idProfile"></param>
        /// <param name="docorFasc"></param>
        /// <returns></returns>
        public static bool setdatavistaSP_TV(DocsPaWR.InfoUtente infoutente, string idProfile, string docorFasc, String idRegistro, string idTrasm)
        {
            try
            {
                return docsPaWS.SetDataVistaSP_TV(infoutente, idProfile, docorFasc, idRegistro, idTrasm);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool SetDataVistaSP(InfoUtente infoUser, string docNumber, string docOrFolder)
        {
            try
            {
                return docsPaWS.SetDataVistaSP(infoUser, docNumber, docOrFolder);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static Trasmissione[] getQueryEffettuatePagingLite(TrasmissioneOggettoTrasm oggTrasm, FiltroRicerca[] filterRic, int pageNumber, bool excel, int pageSize, out int totalPageNumber, out int recordCount)
        {
            totalPageNumber = 0;
            recordCount = 0;

            try
            {
                Trasmissione[] result = docsPaWS.TrasmissioneGetQueryEffettuatePagingLiteWithoutTrasmUtente(oggTrasm, filterRic, UIManager.UserManager.GetUserInSession(), UIManager.RoleManager.GetRoleInSession(), pageNumber, excel, pageSize, out totalPageNumber, out recordCount);

                return result;
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Trasmissione[] getQueryEffettuatePagingLiteWithTrasmUtente(TrasmissioneOggettoTrasm oggTrasm, FiltroRicerca[] filterRic, int pageNumber, bool excel, int pageSize, out int totalPageNumber, out int recordCount)
        {
            totalPageNumber = 0;
            recordCount = 0;

            try
            {
                Trasmissione[] result = docsPaWS.TrasmissioneGetQueryEffettuatePagingLite(oggTrasm, filterRic, UIManager.UserManager.GetUserInSession(), UIManager.RoleManager.GetRoleInSession(), pageNumber, excel, pageSize, out totalPageNumber, out recordCount);

                return result;
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }


        public static Trasmissione[] getQueryRicevuteLite(
                              TrasmissioneOggettoTrasm oggTrasm,
                              FiltroRicerca[] filterRic,
                              int pageNumber,
                              bool excel,
                              int pageSize,
                              out int totalPageNumber,
                              out int recordCount)
        {
            totalPageNumber = 0;
            recordCount = 0;

            try
            {

                Trasmissione[] result = docsPaWS.TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtente(oggTrasm, filterRic, UIManager.UserManager.GetUserInSession(), UIManager.RoleManager.GetRoleInSession(), pageNumber, excel, pageSize, out totalPageNumber, out recordCount);

                return result;
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            return null;
        }


        public static void setRagioneSel(Page page, RagioneTrasmissione ragione)
        {
            page.Session["gestioneNuovaTrasm.setRagioneSel"] = ragione;
        }

        public static RagioneTrasmissione getRagioneSel(Page page)
        {
            return (RagioneTrasmissione)page.Session["gestioneNuovaTrasm.setRagioneSel"];
        }

        public static void removeRagioneSel(Page page)
        {
            page.Session.Remove("gestioneNuovaTrasm.setRagioneSel");
        }

        public static void setGestioneTrasmissione(Page page, Trasmissione trasmissione)
        {
            page.Session["gestioneTrasm.GestTrasm"] = trasmissione;
        }

        public static Trasmissione getGestioneTrasmissione(Page page)
        {
            return (Trasmissione)page.Session["gestioneTrasm.GestTrasm"];
        }

        public static void removeGestioneTrasmissione(Page page)
        {
            page.Session.Remove("gestioneTrasm.GestTrasm");
        }

        public static RagioneTrasmissione[] getListaRagioni(Page page, DocsPaWR.SchedaDocumento schedaDocumento, bool flgDaRicercaTrasm)
        {
            try
            {
                DocsPaWR.TrasmissioneDiritti diritti = new DocsPaWR.TrasmissioneDiritti();
                if (schedaDocumento != null)
                    diritti.accessRights = schedaDocumento.accessRights;

                diritti.idAmministrazione = UserManager.GetUserInSession().idAmministrazione;
                RagioneTrasmissione[] result = docsPaWS.TrasmissioneGetRagioni(diritti, flgDaRicercaTrasm);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            return null;
        }

        public static RagioneTrasmissione[] getListaRagioniFasc(Page page, Fascicolo fasc)
        {
            try
            {
                DocsPaWR.TrasmissioneDiritti diritti = new DocsPaWR.TrasmissioneDiritti();
                if (fasc != null)
                    //if (fasc.stato == "C")
                    //{
                    //    diritti.accessRights = "45";		//lettura
                    //}
                    //else
                    //{
                    diritti.accessRights = fasc.accessRights;
                //}
                diritti.idAmministrazione = UserManager.GetUserInSession().idAmministrazione;
                //flgDaRicercaTrasm=true : vengo da ricerca trasmissioni cerco tutte le ragioni
                //flgDaRicercaTrasm=false : ricerca trasm con cha_vis='1'
                bool flgDaRicercaTrasm = false;
                RagioneTrasmissione[] result = docsPaWS.TrasmissioneGetRagioni(diritti, flgDaRicercaTrasm);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            return null;
        }

        public static Trasmissione saveExecuteTrasmAM(Page page, Trasmissione trasmissione, DocsPaWR.InfoUtente infoUtente)
        {
            Trasmissione result = null;
            try
            {
                string server_path = utils.getHttpFullPath();
                docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                result = docsPaWS.TrasmissioneSaveExecuteTrasmAM(server_path, trasmissione, infoUtente);
                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }

        /// <summary>
        /// Reperimento delle informazioni di stato relative ad una trasmissione utente
        /// </summary>
        /// <param name="idTrasmissioneUtente"></param>
        /// <returns></returns>
        public static DocsPaWR.StatoTrasmissioneUtente getStatoTrasmissioneUtente(string idTrasmissioneUtente)
        {
            //Lnr 15/05/2013
            try
            {
                return docsPaWS.GetStatoTrasmissioneUtente(UserManager.GetInfoUser(), idTrasmissioneUtente);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        //Tatiana 17/05/2013
        public static FileDocumento getReportTrasmUO(Page page, DocsPaWR.FiltroRicerca[] filtriTrasm, string UO)
        {
            try
            {
                FileDocumento fileDoc;
                int res = docsPaWS.ReportTrasmissioniUO(filtriTrasm, UO, UserManager.GetInfoUser(), out fileDoc);

                if (res != 0)
                {
                    throw new Exception("Si è verificato un errore nella creazione del report");
                }
                else
                    if (res == 0 && fileDoc == null)
                    {//Tat                        
                        return null;
                    }
                return fileDoc;
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            return null;
        }

        #region Session functions

        /// <summary>
        /// Reperimento valore da sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        private static System.Object GetSessionValue(string sessionKey)
        {
            try
            {
                return System.Web.HttpContext.Current.Session[sessionKey];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Impostazione valore in sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="sessionValue"></param>
        private static void SetSessionValue(string sessionKey, object sessionValue)
        {
            try
            {
                System.Web.HttpContext.Current.Session[sessionKey] = sessionValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Rimozione chiave di sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        private static void RemoveSessionValue(string sessionKey)
        {
            try
            {
                System.Web.HttpContext.Current.Session.Remove(sessionKey);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
        #endregion

        /// <summary>
        /// Invia solleciti alle trasmissioni effettuate
        /// </summary>
        /// <param name="trasm"></param>
        /// <param name="page"></param>
        public static bool sendSollecito(Trasmissione trasmissione)
        {
            bool res = false;

            //passo la path del front end per il link del documento nella email di sollecito che sarà inviata
            string path = utils.getHttpFullPath();

            try
            {
                res = docsPaWS.trasmissioniSendSollecito_newWA(path, trasmissione);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            return res;
        }

        public static RagioneTrasmissione[] getListaRagioniFasc(Fascicolo fasc)
        {
            try
            {
                DocsPaWR.TrasmissioneDiritti diritti = new DocsPaWR.TrasmissioneDiritti();
                if (fasc != null)
                {
                    //if (fasc.stato == "C")
                    //{
                    //    diritti.accessRights = "45";		//lettura
                    //}
                    //else
                    //{
                    diritti.accessRights = fasc.accessRights;
                    //}
                }
                diritti.idAmministrazione = UserManager.GetUserInSession().idAmministrazione;
                //flgDaRicercaTrasm=true : vengo da ricerca trasmissioni cerco tutte le ragioni
                //flgDaRicercaTrasm=false : ricerca trasm con cha_vis='1'
                bool flgDaRicercaTrasm = false;
                RagioneTrasmissione[] result = docsPaWS.TrasmissioneGetRagioni(diritti, flgDaRicercaTrasm);

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        #region Autenticazione sistemi esterni

        public static DocsPaWR.SistemaEsterno getSistemaEsterno(string idAmm, string codiceApp)
        {
            try
            {
                return docsPaWS.getSistemaEsternoByCodeApp(idAmm, codiceApp);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool AcquireRightsFromExtSys(string idObject, string idRuolo, string idUtente, string idSE, string idUSE)
        {
            try
            {
                return docsPaWS.AcquireRightsFromExtSys(idObject, idRuolo, idUtente, idSE, idUSE);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        #endregion

        public static bool DocumentAlreadyTransmitted_Opt(string idDocument)
        {
            try
            {
                if (!string.IsNullOrEmpty(idDocument))
                    return docsPaWS.DocumentAlreadyTransmitted_Opt(idDocument);
                else return false;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static List<DocsPaWR.InfoTrasmissione> GetTrasmissioniPendentiConWorkflow(string idDocOrFasc, string docOrFasc, string idRuoloInUO, string idPeople, out string[] idTrasmsSingola, ref DocsPaWR.SearchPagingContext pagingContext)
        {
            List<DocsPaWR.InfoTrasmissione> retValue = null;
            idTrasmsSingola = null;
            try
            {
                retValue = docsPaWS.GetTrasmissioniPendentiConWorkflow(idDocOrFasc, docOrFasc, idRuoloInUO, idPeople, ref pagingContext, out idTrasmsSingola).ToList();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return retValue;
        }

        public static bool AcceptTransmissions(List<string> idTrasmSingole, string noteAccettazione)
        {
            try
            {
                return docsPaWS.AcceptTransmissions(idTrasmSingole.ToArray(), noteAccettazione, UserManager.GetSelectedRole(), UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public static bool AcceptMassiveTrasmDocument(string idProfile)
        {
            bool result = false;
            InfoUtente infoUtente = UserManager.GetInfoUser();
            try
            {
                return docsPaWS.AcceptMassiveTrasmDocument(idProfile, UserManager.GetSelectedRole(), infoUtente);
            }
            catch (Exception e)
            {

            }

            return result;
        }

        public static bool ViewMassiveTrasmDocument(string idProfile)
        {
            bool result = false;
            InfoUtente infoUtente = UserManager.GetInfoUser();
            try
            {
                return docsPaWS.ViewMassiveTrasmDocument(idProfile, UserManager.GetSelectedRole(), infoUtente);
            }
            catch (Exception e)
            {

            }

            return result;
        }


        public static bool AcceptMassiveTrasmFasc(string idProject)
        {
            bool result = false;
            InfoUtente infoUtente = UserManager.GetInfoUser();
            try
            {
                return docsPaWS.AcceptMassiveTrasmFasc(idProject, UserManager.GetSelectedRole(), infoUtente);
            }
            catch (Exception e)
            {

            }

            return result;
        }

        public static bool ViewMassiveTrasmFasc(string idProject)
        {
            bool result = false;
            InfoUtente infoUtente = UserManager.GetInfoUser();
            try
            {
                return docsPaWS.ViewMassiveTrasmFasc(idProject, UserManager.GetSelectedRole(), infoUtente);
            }
            catch (Exception e)
            {

            }

            return result;
        }

    }
}
