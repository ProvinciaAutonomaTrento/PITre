using System;
using System.Configuration;
using System.Web.UI;
using DocsPAWA.DocsPaWR;
using System.Collections;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using DocsPAWA.utils;
using log4net;
using MText;
using DocsPAWA.models;

namespace DocsPAWA
{
    /// <summary>
    /// Summary description for DocumentManager.
    /// </summary>

    public class DocumentManager : Page
    {

        protected static DocsPAWA.DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.getWS();
        private static ILog logger = LogManager.GetLogger(typeof(DocumentManager));

        public static string getSegnaturaCampiVariabili(Page page, SchedaDocumento schedaDocumento, string[] campiVariabili)
        {
            string retValue = "";

            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                Ruolo ruolo = UserManager.getRuolo(page);
                //retValue=docsPaWS.DocumentoGetSegnaturaCampiVariabili(schedaDocumento,campiVariabili,infoUtente);
                retValue = schedaDocumento.protocollo.segnatura;
                /* il log è scritto da ws.updateStampeDocumentoEffettuate()
                DocsPaWebService DocspaWS = new DocsPaWebService();
                DocspaWS.scriviLog(schedaDocumento, infoUtente, ruolo);*/
            }
            catch (System.Exception exc)
            {
                ErrorManager.redirect(page, exc);
            }

            return retValue;
        }
        /// <summary>
        /// restitiusci il valore nel web.config dei ws DOC_TYPE
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string getTypeId()
        {
            return docsPaWS.doucmentoGetDocType();
        }

        /// <summary>
        /// Reperimento di un nuovo oggetto SchedaDocumento 
        /// </summary>
        /// <returns>
        /// SchedaDocumento predisposta per l'inserimento
        /// </returns>
        public static SchedaDocumento NewSchedaDocumento(Page page)
        {
            DocsPaWR.SchedaDocumento newScheda = docsPaWS.NewSchedaDocumento(UserManager.getInfoUtente());

            //// Impostazione della nuova scheda documento e del primo documento in sessione

            //DocumentManager.setDocumentoInLavorazione(newScheda);
            DocumentManager.setDocumentoSelezionato(newScheda);
            FileManager.setSelectedFile(page, newScheda.documenti[0]);

            return newScheda;
        }
        public static string getAccessRightDocBySystemID(string idProfile, DocsPaWR.InfoUtente infoUtente)
        {
            return docsPaWS.getAccessRightDocBySystemID(idProfile, infoUtente);
        }
        /// <summary>
        /// Se true, risulta disabilita la gestione dei repository temporanei
        /// </summary>
        /// <returns></returns>
        public static bool IsDisabledSessionRepositoryContext(DocsPaWR.InfoUtente infoUtente)
        {
            return docsPaWS.IsDisabledSessionRepositoryContext(infoUtente);
        }

        /// <summary>
        /// Reperimento del dettaglio del documento
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static SchedaDocumento getDettaglioDocumento(Page page, string idProfile, string docNumber)
        {
            logger.Info("BEGIN");
            DocsPaWR.SchedaDocumento sd = new DocsPAWA.DocsPaWR.SchedaDocumento();

            try
            {
                if (idProfile == null && docNumber == null)
                {
                    sd = null;
                }
                else
                {
                    InfoUtente infoUtente = UserManager.getInfoUtente(page);
                    sd = docsPaWS.DocumentoGetDettaglioDocumento(infoUtente, idProfile, docNumber);
                    if ((sd == null))// || (sd.inCestino != null && sd.inCestino == "1"))
                    {
                        string errorMessage = string.Empty;

                        if (sd == null)
                        {
                            //verificata ACL, ritorna semmai un msg in out errorMessage
                            int rtn = verificaACL("D", idProfile, infoUtente, out errorMessage);
                            if (rtn == -1)
                                errorMessage = "Attenzione, non è stato possibile recuperare i dati del documento richiesto.\\nConsultare il log per maggiori dettagli.";
                        }
                        //else // è in cestino
                        //{
                        //    errorMessage = "Il documento è stato rimosso.\\nNon è più possibile visualizzarlo.";
                        //}

                        if (errorMessage.Equals(""))
                            errorMessage = "Attenzione, non è stato possibile recuperare i dati del documento richiesto.\\nConsultare il log per maggiori dettagli.";

                        page.Response.Write("<script>alert('" + errorMessage + "');</script>");
                        if (page.Session["protocolloEsistente"] != null && (bool)page.Session["protocolloEsistente"])
                        {
                            page.Session.Remove("protocolloEsistente");
                        }
                        else
                        {
                            // Redirect alla homepage di docspa
                            SiteNavigation.CallContextStack.Clear();
                            SiteNavigation.NavigationContext.RefreshNavigation();
                            string script = "<script>window.open('../GestioneRuolo.aspx','principale','width=410,height=300,scrollbars=NO');</script>";
                            page.Response.Write(script);
                        }
                    }
                }
            }
            catch (Exception es)
            {
                return null;
            }
            logger.Info("END");
            return sd;
        }

        /// <summary>
        /// Reperimento del dettaglio del documento senza considerare la security
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static SchedaDocumento getDettaglioDocumentoNoSecurity(Page page, string idProfile, string docNumber)
        {
            DocsPaWR.SchedaDocumento sd = new DocsPAWA.DocsPaWR.SchedaDocumento();

            try
            {
                if (idProfile == null && docNumber == null)
                {
                    sd = null;
                }
                else
                {
                    InfoUtente infoUtente = UserManager.getInfoUtente(page);
                    sd = docsPaWS.DocumentoGetDettaglioDocumentoNoSecurity(infoUtente, idProfile, docNumber);
                    if ((sd == null))// || (sd.inCestino != null && sd.inCestino == "1"))
                    {
                        string errorMessage = string.Empty;

                        if (sd == null)
                        {
                            //verificata ACL, ritorna semmai un msg in out errorMessage
                            int rtn = verificaACL("D", idProfile, infoUtente, out errorMessage);
                            if (rtn == -1)
                                errorMessage = "Attenzione, non è stato possibile recuperare i dati del documento richiesto.\\nConsultare il log per maggiori dettagli.";
                        }
                        if (errorMessage.Equals(""))
                            errorMessage = "Attenzione, non è stato possibile recuperare i dati del documento richiesto.\\nConsultare il log per maggiori dettagli.";

                        page.Response.Write("<script>alert('" + errorMessage + "');</script>");
                        if (page.Session["protocolloEsistente"] != null && (bool)page.Session["protocolloEsistente"])
                        {
                            page.Session.Remove("protocolloEsistente");
                        }
                        else
                        {
                            // Redirect alla homepage di docspa
                            SiteNavigation.CallContextStack.Clear();
                            SiteNavigation.NavigationContext.RefreshNavigation();
                            string script = "<script>window.open('../GestioneRuolo.aspx','principale','width=410,height=300,scrollbars=NO');</script>";
                            page.Response.Write(script);
                        }
                    }
                }
            }
            catch (Exception es)
            {
                return null;
            }

            return sd;
        }


        /// <summary>
        /// dettaglio documento da cestiono, non controlla se il documento è stato 
        /// rimosso permettendone così la visualizzazione
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static SchedaDocumento getDettaglioDocumentoDaCestino(Page page, string idProfile, string docNumber)
        {
            //SchedaDocumento result = null;
            DocsPaWR.SchedaDocumento sd = new DocsPAWA.DocsPaWR.SchedaDocumento();

            try
            {
                if (idProfile == null && docNumber == null)
                {
                    sd = null;
                    throw new Exception();

                }
                else
                {
                    InfoUtente infoUtente = UserManager.getInfoUtente(page);
                    sd = docsPaWS.DocumentoGetDettaglioDocumentoNoDataVista(infoUtente, idProfile, docNumber);

                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);

            }

            return sd;
        }

        public static SchedaDocumento getDettaglioDocumentoNoDataVista(Page page, string idProfile, string docNumber)
        {
            DocsPaWR.SchedaDocumento sd = new DocsPAWA.DocsPaWR.SchedaDocumento();

            try
            {
                if (idProfile == null && docNumber == null)
                {
                    sd = null;
                }
                else
                {
                    InfoUtente infoUtente = UserManager.getInfoUtente(page);
                    sd = docsPaWS.DocumentoGetDettaglioDocumentoNoDataVista(infoUtente, idProfile, docNumber);

                    string errorMessage = string.Empty;

                    if ((sd == null) || (sd.inCestino != null && sd.inCestino == "1"))
                    {
                        if (sd == null)
                        {
                            int rtn = verificaACL("D", idProfile, infoUtente, out errorMessage);
                            if (rtn == -1) //errore generico, negli altri casi il metodo  verificaACL ritorna un msg in  errorMessage.
                                errorMessage = "Attenzione, non è stato possibile recuperare i dati del documento richiesto.\\nConsultare il log per maggiori dettagli.";
                        }
                        else
                        {
                            errorMessage = "Il documento è stato rimosso.\\nNon è più possibile visualizzarlo.";
                        }

                        if (errorMessage.Equals(""))
                            errorMessage = "Attenzione, non è stato possibile recuperare i dati del documento richiesto.\\nConsultare il log per maggiori dettagli.";

                        page.Response.Write("<script>alert('" + errorMessage + "');</script>");
                        // Redirect alla homepage di docspa
                        SiteNavigation.CallContextStack.Clear();
                        SiteNavigation.NavigationContext.RefreshNavigation();
                        string script = "<script>window.open('../GestioneRuolo.aspx','principale','width=410,height=300,scrollbars=NO');</script>";
                        page.Response.Write(script);
                    }
                }
            }
            catch (Exception es)
            {
                return null;
                //ErrorManager.redirect(page, es);
            }

            return sd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="schedaDoc"></param>
        /// <param name="risultatoProtocollazione"></param>
        /// <returns></returns>
        public static SchedaDocumento protocolla(Page page,
                                                SchedaDocumento schedaDoc, out DocsPAWA.DocsPaWR.ResultProtocollazione risultatoProtocollazione)
        {
            logger.Info("BEGIN");
            SchedaDocumento result = null;
            risultatoProtocollazione = 0;

            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                Ruolo ruolo = UserManager.getRuolo(page);
                schedaDoc = setFusionFields(page, schedaDoc, infoUtente);

                result = docsPaWS.DocumentoProtocolla(schedaDoc, infoUtente, ruolo, out risultatoProtocollazione);
            }
            catch (Exception es)
            {
                if (result == null)
                {
                    result = schedaDoc;
                }

                ErrorManager.redirect(page, es, "protocollazione");
            }

            logger.Info("END");
            return result;
        }

        public static SchedaDocumento creaProtocollo(Page page, SchedaDocumento schedaDoc, DocsPaWR.Fascicolo fasc, out DocsPAWA.DocsPaWR.ResultProtocollazione risultatoProtocollazione)
        {
            string message = null;

            //ABBATANGELI GIANLUIGI - Gestione applicazioni esterne
            if (string.IsNullOrEmpty(schedaDoc.codiceApplicazione))
                schedaDoc.codiceApplicazione = (string.IsNullOrEmpty(ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]) ? "___" : ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"].ToString());

            SchedaDocumento result = null;
            string nameError = "protocollazione";
            //DocsPaWR.ResultProtocollazione risultatoProtocollazione;
            risultatoProtocollazione = DocsPAWA.DocsPaWR.ResultProtocollazione.OK;
            try
            {
                string valoreChiaveConsentiClass = string.Empty;
                valoreChiaveConsentiClass = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_BLOCCA_CLASS");
                if (fasc != null && !string.IsNullOrEmpty(fasc.isFascConsentita) && fasc.isFascConsentita == "0" && !string.IsNullOrEmpty(valoreChiaveConsentiClass) && valoreChiaveConsentiClass.Equals("1"))
                {
                    risultatoProtocollazione = DocsPAWA.DocsPaWR.ResultProtocollazione.ERRORE_DURANTE_LA_FASCICOLAZIONE;
                    message = "Non è possibile inserire documenti nel fascicolo selezionato. Selezionare un nodo foglia.";
                    throw new Exception(message);
                }
                else
                    result = protocolla(page, schedaDoc, out risultatoProtocollazione);

                //esitoProtocollazione = risultatoProtocollazione;

                if (result == null || risultatoProtocollazione != DocsPAWA.DocsPaWR.ResultProtocollazione.OK)
                {

                    switch (risultatoProtocollazione)
                    {
                        case DocsPAWA.DocsPaWR.ResultProtocollazione.AMMINISTRAZIONE_MANCANTE:
                            message = "Identificativo dell'amministrazione non trovata.";
                            break;
                        case DocsPAWA.DocsPaWR.ResultProtocollazione.DESTINATARIO_MANCANTE:
                            message = "Il destinatario è obbligatorio.";
                            break;
                        case DocsPAWA.DocsPaWR.ResultProtocollazione.MITTENTE_MANCANTE:
                            message = "Il mittente è obbligatorio.";
                            break;
                        case DocsPAWA.DocsPaWR.ResultProtocollazione.OGGETTO_MANCANTE:
                            message = "L'oggetto è obbligatorio.";
                            break;
                        case DocsPAWA.DocsPaWR.ResultProtocollazione.REGISTRO_MANCANTE:
                            message = "Il registro è obbligatorio.";
                            break;
                        case DocsPAWA.DocsPaWR.ResultProtocollazione.REGISTRO_CHIUSO:
                            message = "Il registro non è aperto.";
                            break;
                        case DocsPAWA.DocsPaWR.ResultProtocollazione.STATO_REGISTRO_ERRATO:
                            message = "Lo stato del registro non è corretto.";
                            break;
                        case DocsPAWA.DocsPaWR.ResultProtocollazione.DATA_SUCCESSIVA_ATTUALE:
                            message = "La data di protocollazione è successiva a quella attuale.";
                            break;
                        case DocsPAWA.DocsPaWR.ResultProtocollazione.DATA_ERRATA:
                            message = "La data di protocollazione non è valida.";
                            break;
                        case DocsPAWA.DocsPaWR.ResultProtocollazione.APPLICATION_ERROR:
                            //message = "Errore imprevisto dell'applicazione. Ritentare l'operazione.";
                            message = "In questo momento non è stato possibile protocollare il documento.<BR><BR>Ripetere l'operazione di protocollazione.";
                            break;
                        case DocsPAWA.DocsPaWR.ResultProtocollazione.DOCUMENTO_GIA_PROTOCOLLATO: //TODO: Se predisposto già protocollato
                            //message = "Errore imprevisto dell'applicazione. Ritentare l'operazione.";
                            message = "Il documento risulta già protocollato con segnatura:<BR><BR>" + result.protocollo.segnatura;
                            break;
                        case DocsPAWA.DocsPaWR.ResultProtocollazione.FORMATO_SEGNATURA_MANCANTE:
                            message = " Formato della segnatura non impostato. Contattare l'Amministratore";
                            break;
                        case DocsPAWA.DocsPaWR.ResultProtocollazione.ERRORE_WSPIA_PROTOCOLLO_ENTRATA_MITTENTE:
                            message = " WSPIA - Non è possibile utilizzare il mittente selezionato.";
                            break;

                        case DocsPAWA.DocsPaWR.ResultProtocollazione.ERRORE_WSPIA_CLASSIFICA_NODO_FOGLIA:
                            message = " WSPIA - Selezionare la voce di titolario di dettaglio";
                            break;
                        case DocsPAWA.DocsPaWR.ResultProtocollazione.ERRORE_WSPIA_PROTOCOLLAZIONE_SEMPLICE:
                            message = " WSPIA - Segnatura restituita nulla o vuota.";
                            break;
                    }

                    throw new Exception(message);
                }

                if (result != null && risultatoProtocollazione == DocsPAWA.DocsPaWR.ResultProtocollazione.OK)
                {
                    if (schedaDoc.protocollo.invioConferma != null && schedaDoc.protocollo.invioConferma.Equals("1"))
                    {
                        Registro reg = schedaDoc.registro;
                        // nel caso si arrivi dalla popup di scelta RF per invio ricezione automatica
                        if (!string.IsNullOrEmpty(schedaDoc.id_rf_invio_ricevuta))
                        {
                            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
                            reg = ws.GetRegistroBySistemId(schedaDoc.id_rf_invio_ricevuta);
                        }

                        //solo se la protocollazione è andata a buon fine invio la notifica
                        //if(!reg.invioRicevutaManuale.ToUpper().Equals("1"))

                        DocumentManager.DocumentoInvioRicevuta(page, result, reg);
                    }

                }


            }
            catch (Exception es)
            {
                if (result == null)
                {
                    result = schedaDoc;
                }

                ErrorManager.redirect(page, es, nameError);
            }

            return result;
        }

        public static bool DocumentoInvioRicevuta(Page page, DocsPaWR.SchedaDocumento schedaDocumento, DocsPAWA.DocsPaWR.Registro registro)
        {
            bool result = false;
            string errormessage = string.Empty;
            try
            {
                Ruolo ruolo = UserManager.getRuolo(page);
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                //se l'rf scelto per la spedizione è diverso da quello salvato in dpa_ass_doc_mail_interop in fase di scarico aggiorno la tabella ed effettuo la spedizione
                DataSet ds = DocsPAWA.utils.MultiCasellaManager.GetAssDocAddress(DocumentManager.getDocumentoSelezionato().docNumber);
                if (ds != null && ds.Tables["ass_doc_rf"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["ass_doc_rf"].Rows)
                    {
                        if (!row["registro"].ToString().Equals(registro.systemId) && (!string.IsNullOrEmpty(registro.email)) && 
                            (!schedaDocumento.interop.Equals("I")))
                            DocsPAWA.utils.MultiCasellaManager.UpdateAssDocAddress(DocumentManager.getDocumentoSelezionato().docNumber, registro.systemId, registro.email);
                    }
                }
                result = docsPaWS.ProtocolloInvioRicevutaDiRitorno(schedaDocumento, registro, ruolo, infoUtente, out errormessage);
                if (!result)
                {
                    throw new Exception();
                }

            }
            catch (Exception es)
            {
                if (errormessage == string.Empty)
                    ErrorManager.OpenErrorPage(page, "Non è stato possibile inviare la ricevuta di ritorno. Riprovare più tardi.", "invio ricevuta di ritorno");
                else
                    ErrorManager.OpenErrorPage(page, errormessage, "invio ricevuta di ritorno");
            }
            return result;
        }

        public static bool DocumentoInvioNotificaAnnulla(Page page, string idProfile, DocsPAWA.DocsPaWR.Registro registro)
        {
            bool result = false;
            SchedaDocumento schedaDoc = getDocumentoSelezionato(page);
            DataSet ds = DocsPAWA.utils.MultiCasellaManager.GetAssDocAddress(DocumentManager.getDocumentoSelezionato().docNumber);
            if (ds != null && ds.Tables["ass_doc_rf"].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables["ass_doc_rf"].Rows)
                {
                    registro.email = row["mail"].ToString();
                }
            }
            //Andrea De Marco - MEV Gestione Eccezioni PEC - Aggiunta del schedaDoc.interop.Equals("E")
            //Per ripristino commentare De Marco e decommentare il codice sottostante
            //End De Marco
            if (schedaDoc.interop != null && !schedaDoc.interop.Equals("") && (!schedaDoc.interop.Equals("P") || !schedaDoc.interop.Equals("E")))
            //if (schedaDoc.interop != null && !schedaDoc.interop.Equals("") && !schedaDoc.interop.Equals("P"))
            {

                try
                {
                    result = docsPaWS.ProtocolloInvioNotificaAnnulla(idProfile, registro);
                    if (!result)
                    {
                        throw new Exception();
                    }
                }
                catch (Exception es)
                {
                    ErrorManager.OpenErrorPage(page, es, "invio notifica di annullamento");
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <returns></returns>
        public static bool TrasmettiProtocolloInterno(Page page, string serverName, SchedaDocumento schedaDoc, bool isEnableUffRef, out bool verificaRagioni, out string message)
        {
            bool result = true;
            verificaRagioni = true;
            message = "";

            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                Ruolo ruolo = UserManager.getRuolo(page);
                //				schedaDoc = setFusionFields(page, schedaDoc, infoUtente);

                if (!docsPaWS.TrasmettiProtocolloInterno(schedaDoc, ruolo, serverName, isEnableUffRef, infoUtente, out verificaRagioni, out message)) throw new Exception();
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        #region inserimento del mezzo di spedizione
        public static bool inserisciMetodoSpedizione(InfoUtente info, string idDocumentTypes, string idProfile)
        {
            bool result = false;
            result = docsPaWS.collegaMezzoSpedizioneDocumento(info, idDocumentTypes, idProfile);
            return result;
        }

        public static bool updateMetodoSpedizione(InfoUtente info, string oldDocumentTypes, string idDocumentTypes, string idProfile)
        {
            bool result = false;
            result = docsPaWS.updateMezzoSpedizioneDocumento(info, oldDocumentTypes, idDocumentTypes, idProfile);
            return result;
        }

        public static bool deleteMetodoSpedizione(InfoUtente info, string idProfile)
        {
            bool result = false;
            result = docsPaWS.deleteMezzoSpedizioneDocumento(info, idProfile);
            return result;
        }
        #endregion

        #region per le funzionalità di fascicolazione e trasmissione rapida
        public static SchedaDocumento protocolla(Page page, SchedaDocumento schedaDoc, DocsPaWR.Fascicolo fascicolo, DocsPAWA.DocsPaWR.TemplateTrasmissione template, out DocsPAWA.DocsPaWR.ResultProtocollazione risultatoProtocollazione, ref string returnMsg)
        {
            string segnatura = "";
            risultatoProtocollazione = DocsPAWA.DocsPaWR.ResultProtocollazione.OK;
            try
            {
                if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.codice))
                {
                    schedaDoc.codiceFascicolo = fascicolo.codice;
                }
                schedaDoc = DocumentManager.creaProtocollo(page, schedaDoc, fascicolo, out risultatoProtocollazione);

                //FASCICOLAZIONE
                if (schedaDoc.systemId != null && fascicolo != null)
                {
                    if (fascicolo.stato == "C")
                    {
                        returnMsg += "Il fascicolo scelto è chiuso. Pertanto il documento non è stato fascicolato";
                    }
                    else
                    {


                        if (schedaDoc.protocollo != null)
                            segnatura = schedaDoc.protocollo.segnatura;

                        string msg = string.Empty;
                        if (!DocumentManager.fascicolaRapida(page, schedaDoc.systemId, schedaDoc.docNumber, segnatura, fascicolo, out msg))
                        {
                            if (!string.IsNullOrEmpty(msg))
                                returnMsg += msg;
                            else
                                returnMsg += " Il documento non è stato fascicolato";
                        }
                        else
                        {
                            schedaDoc.fascicolato = "1";
                        }

                    }
                }
                //TRASMISSIONE RAPIDA
                if (schedaDoc.systemId != null && template != null)
                {
                    DocsPaWR.Trasmissione trasmissione = null;
                    DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);
                    DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(schedaDoc);

                    trasmissione = TrasmManager.addTrasmDaTemplate(page, infoDoc, template, infoUtente);
                    if (trasmissione == null || (trasmissione.systemId == "" || trasmissione.systemId == null))
                        returnMsg += " La trasmissione non è stata creata ";
                    else
                    {
                        trasmissione = TrasmManager.executeTrasm(page, trasmissione);
                        if (trasmissione == null)
                            returnMsg += " Il documento non è stato trasmesso";
                    }
                }

                return schedaDoc;
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }


        public static DocsPAWA.DocsPaWR.Fascicolo GetFascicoloPrimarioDoc(Page page, DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc)
        {
            DocsPaWR.Fascicolo retFasc = null;
            DocsPaWR.InfoDocumento infoDocumento = null;
            infoDocumento = getInfoDocumento(schedaDoc);
            try
            {
                //costruzione m_dataTableFascicoli
                if (infoDocumento != null)
                {
                    DocsPaWR.Fascicolo[] listaFascicoli = GetFascicoliDaDoc(page, infoDocumento.idProfile);
                    if (listaFascicoli != null)
                        if (listaFascicoli.Length > 0)
                        {
                            retFasc = listaFascicoli[0];
                        }
                }
                return retFasc;
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
                return retFasc;
            }
            catch (Exception)
            {
                return retFasc;
            }
        }

        public static bool getSeDocFascicolato(Page page, SchedaDocumento schedaDoc)
        {
            bool result = false;
            try
            {
                result = docsPaWS.DocumentoGetSeDocFascicolato(schedaDoc.systemId);
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return result;
        }

        #endregion
        public static SchedaDocumento salva(Page page, SchedaDocumento schedaDoc, bool enableUffRef, out bool daAggiornareUffRef)
        {
            logger.Info("BEGIN");
            daAggiornareUffRef = false;

            try
            {
                //ABBATANGELI GIANLUIGI - Se non trovo la chiave di configurazione CODICE_APPLICAZIONE, imposto il codice applicazione di default a DOC
                schedaDoc.codiceApplicazione = (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]) ? "___" : System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]);

                InfoUtente infoUtente = UserManager.getInfoUtente(page);

                SchedaDocumento result = docsPaWS.DocumentoSaveDocumento(UserManager.getRuolo(page), infoUtente, schedaDoc, enableUffRef, out daAggiornareUffRef);

                //Fine Celeste
                if (result == null)
                {
                    //string message = "Errore imprevisto dell'applicazione. Ritentare l'operazione.";
                    string message = "In questo momento non è stato possibile creare il documento. Si prega di ripetere  l'operazione.";

                    throw new Exception(message);
                }
                logger.Info("END");
                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        protected static SchedaDocumento setFusionFields(Page page, SchedaDocumento schedaDocumento, InfoUtente infoUtente)
        {
            logger.Info("BEGIN");
            // campi obbligatori per DocsFusion
            schedaDocumento.idPeople = infoUtente.idPeople;
            schedaDocumento.userId = infoUtente.userId;
            if (schedaDocumento.typeId == null)
            {
                schedaDocumento.typeId = DocumentManager.getTypeId();
            }
            if (schedaDocumento.appId == null)
                schedaDocumento.appId = "ACROBAT";

            logger.Info("END");
            return schedaDocumento;
        }

        public static SchedaDocumento creaDocumentoGrigio(Page page, SchedaDocumento schedaDoc)
        {
            logger.Info("BEGIN");
            SchedaDocumento result = null;
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                schedaDoc = setFusionFields(page, schedaDoc, infoUtente);

                //ABBATANGELI GIANLUIGI - Gestione applicazioni esterne
                if (string.IsNullOrEmpty(schedaDoc.codiceApplicazione))
                    schedaDoc.codiceApplicazione = (string.IsNullOrEmpty(ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]) ? "___" : ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"].ToString());

                result = docsPaWS.DocumentoAddDocGrigia(schedaDoc, infoUtente, UserManager.getRuolo(page));

                if (result == null)
                {
                    result = schedaDoc; //per ridare i dati precedentementi inseriti.
                    //string message = "Errore imprevisto dell'applicazione. Ritentare l'operazione.";
                    string message = "In questo momento non è stato possibile creare il documento. Si prega di ripetere  l'operazione.";

                    throw new Exception(message);

                }

                logger.Info("END");
                return result;
            }
            catch (Exception es)
            {
                ErrorManager.OpenErrorPage(page, es, "creazione documento");
            }

            return result;
        }

        public static void cambiaDocumentoPersonalePrivato(Page page, SchedaDocumento schedaDoc)
        {
            bool result = false;
            try
            {
                //InfoUtente infoUtente = UserManager.getInfoUtente(page);
                //schedaDoc = setFusionFields(page, schedaDoc, infoUtente);

                result = docsPaWS.DocumentoCambiaPersonalePrivato(schedaDoc.systemId, UserManager.getRuolo(page).idGruppo);

                if (!result)
                {
                    // result = schedaDoc; //per ridare i dati precedentementi inseriti.
                    //string message = "Errore imprevisto dell'applicazione. Ritentare l'operazione.";
                    string message = "In questo momento non è stato possibile modificare il documento. Si prega di ripetere  l'operazione.";

                    throw new Exception(message);

                }


            }
            catch (Exception es)
            {
                ErrorManager.OpenErrorPage(page, es, "cambia personale in privato");
            }

        }


        //rstituisce 
        // 1: se vi è un errore in caso d'inseriemnto
        // 2: se vi errore in caso di modifica
        // 0: se è andato tutto a buon fine
        public static int fascicolaRapida(Page page, string idProfile, string docnumber, string segnatura, Fascicolo fascicolo, bool ok)
        {
            InfoUtente infoUtente = UserManager.getInfoUtente(page);
            string msg = string.Empty;
            bool result = false;
            if (fascicolo != null && fascicolo.folderSelezionato != null)
            {
                result = docsPaWS.FascicolazioneAddDocFolder(infoUtente, idProfile, fascicolo.folderSelezionato, fascicolo.descrizione, out msg);
                if (result)
                    return 0;
                else
                    return 1;
            }
            else
            {
                result = docsPaWS.FascicolazioneAddDocFascicolo(infoUtente, idProfile, fascicolo, true, out msg);

                if (result)
                    return 0;
                else
                    return 2;
            }
        }

        public static bool fascicolaRapida(Page page, string idProfile, string docnumber, string segnatura, string idFascicolo, string codFasc)
        {
            InfoUtente infoUtente = UserManager.getInfoUtente(page);
            bool outValue = false;
            Fascicolo fasc = FascicoliManager.getFascicoloById(page, idFascicolo);
            string msg = string.Empty;
            bool result = docsPaWS.FascicolazioneAddDocFascicolo(infoUtente, idProfile, fasc, true, out msg);
            return result;
        }

        public static bool fascicolaRapida(Page page, string idProfile, string docnumber, string segnatura, Fascicolo fascicolo, out string msg)
        {
            InfoUtente infoUtente = UserManager.getInfoUtente(page);
            msg = string.Empty;
            bool result = false;
            if (fascicolo != null && fascicolo.folderSelezionato != null)
            {
                result = docsPaWS.FascicolazioneAddDocFolder(infoUtente, idProfile, fascicolo.folderSelezionato, fascicolo.descrizione, out msg);
            }
            else
            {
                result = docsPaWS.FascicolazioneAddDocFascicolo(infoUtente, idProfile, fascicolo, true, out msg);
            }
            return result;
        }

        public static bool addDocumentoInFascicolo(Page page, string idProfile, string idFascicolo, out string msg)
        {
            bool result = false;
            msg = string.Empty;

            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                Fascicolo fasc = FascicoliManager.getFascicoloById(page, idFascicolo);
                result = docsPaWS.FascicolazioneAddDocFascicolo(infoUtente, idProfile, fasc, false, out msg);
                //bool result = docsPaWS.FascicolazioneAddDocFascicolo(infoUtente, idProfile, idFascicolo, true, out outValue);
                //bool result = docsPaWS.FascicolazioneAddDocFascicolo(infoUtente.idPeople,infoUtente.idGruppo,idProfile,idFascicolo);
                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return result;
        }

        public static bool FascicolaDocumentoAM(Page page, string idProfile, Fascicolo fascicolo, out string msg)
        {
            bool result = false;
            msg = string.Empty;

            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                //Fascicolo fasc = FascicoliManager.getFascicoloById(page, idFascicolo);
                result = docsPaWS.FascicolaDocumentoAM(infoUtente, idProfile, fascicolo, false, out msg);
                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <param name="segnatura"></param>
        /// <param name="idFolder"></param>
        /// <param name="idFascicolo"></param>
        /// <param name="codFasc"></param>
        /// <param name="fascRapida"></param>
        /// <param name="outValue"></param>
        /*        public static void addDocumentoInFolder(Page page, string idProfile, string idFolder, bool fascRapida, out bool isInFolder)
                {
                    bool outValue = false;
                public static void addDocumentoInFolder(Page page, string idProfile, string idFolder, bool fascRapida, out bool isInFolder)
                {
                    bool outValue = false;
                    isInFolder = false;
                    bool result = false;
                    try
                    {
                        InfoUtente infoUtente = UserManager.getInfoUtente(page);

                        //True: se il documento è già classificato nella folder indicata, false altrimenti
                        isInFolder = docsPaWS.IsDocumentoClassificatoInFolder(idProfile, idFolder);

                        if (!isInFolder)
                        {
                            //se il doc non è già classificato nella folder indicata allora lo inserisco
                            result = docsPaWS.FascicolazioneAddDocFolder(infoUtente, idProfile, idFolder, out outValue);

                            if (!result)
                            {
                                throw new Exception();
                            }
                        }
                    }
                    catch (Exception es)
                    {
                        ErrorManager.redirect(page, es);
                    }
                }


                #region gestione Area di Lavoro
                public static void addAreaLavoro(Page page, SchedaDocumento schedaDocumento)
                {
                    try
                    {
                        Utente utente = UserManager.getUtente(page);
                        Ruolo ruolo = UserManager.getRuolo(page);
                        bool result = docsPaWS.DocumentoExecAddLavoro(getInfoDocumento(schedaDocumento).idProfile, getInfoDocumento(schedaDocumento).tipoProto, null, UserManager.getInfoUtente(page), (schedaDocumento.registro != null ? schedaDocumento.registro.systemId : ""));
                        Hashtable listInArea = new Hashtable();
                        listInArea.Add(schedaDocumento.docNumber, schedaDocumento);


                        //(ListaDoc[keyN].numProt+ListaDoc[keyN].dataApertura,ListaDoc[keyN]);
                        page.Session["listInArea"] = listInArea;


                        if (!result)
                        {
                            throw new Exception();
                        }
                    }
                    //			catch(System.Web.Services.Protocols.SoapException es) 
                    //			{
                    //				ErrorManager.redirect(page, es);
                    //			} 
                    catch (Exception es)
                    {
                        ErrorManager.redirect(page, es);
                    }
                }

                public static void eliminaDaAreaLavoro(Page page, string idProfile, Fascicolo fasc)
                {
                    try
                    {
                        Utente utente = UserManager.getUtente(page);
                        Ruolo ruolo = UserManager.getRuolo(page);
                        InfoUtente infoUtente = UserManager.getInfoUtente(page);
                        bool result = docsPaWS.DocumentoCancellaAreaLavoro(infoUtente.idPeople, infoUtente.idCorrGlobali, idProfile, fasc);

                        if (!result)
                        {
                            throw new Exception();
                        }
                    }
                    //			catch(System.Web.Services.Protocols.SoapException es) 
                    //			{
                    //				ErrorManager.redirect(page, es);
                    //			} 
                    catch (Exception es)
                    {
                        ErrorManager.redirect(page, es);
                    }
                }

                public static bool scollegaDocumento(Page page, string systemId)
                {
                    bool result = false;
                    try
                    {

                        result = docsPaWS.DocumentoScollegaCollegamento(systemId);
                        if (!result)
                        {
                            throw new Exception();
                        }
                    }

                    catch (Exception es)
                    {
                        ErrorManager.redirect(page, es);
                    }

                    return result;
                }


                protected static DocsPAWA.DocsPaWR.AreaLavoro addInListaAreaLav(DocsPAWA.DocsPaWR.AreaLavoro areaLav, object oggetto)
                {
                    object[] nuovaLista;

                    if (areaLav.lista != null)
                    {
                        int len = areaLav.lista.Length;
                        nuovaLista = new object[len + 1];
                        areaLav.lista.CopyTo(nuovaLista, 0);
                        nuovaLista[len] = oggetto;
                    }
                    else
                    {
                        nuovaLista = new object[1];
                        nuovaLista[0] = oggetto;
                    }
                    areaLav.lista = nuovaLista;
                    return areaLav;
                }

                #region Metodo Commentato
                //		public static DocsPAWA.DocsPaWR.AreaLavoro rimuoviDaListaAreaLav(DocsPAWA.DocsPaWR.AreaLavoro areaLav, int index) 
                //		{
                //			if(areaLav == null || areaLav.lista.Length < index)
                //				return areaLav;
                //			
                //			if(areaLav.lista.Length == 1)
                //				return null;
                //
                //			DocsPaWR.AreaLavoro nuovaAreaLav = new DocsPAWA.DocsPaWR.AreaLavoro();
                //			if (areaLav.lista != null && areaLav.lista.Length > 0) 
                //			{	
                //				for(int i = 0; i < areaLav.lista.Length; i++) 
                //				{
                //					if (i != index)
                //						nuovaAreaLav = addInListaAreaLav(nuovaAreaLav, areaLav.lista[i]);
                //				}
                //			}
                //			return nuovaAreaLav;
                //		}
                #endregion


                #endregion
                public static InfoDocumento getInfoDocumento(SchedaDocumento schedaDocumento)
                {
                    InfoDocumento infoDoc = new InfoDocumento();

                    //infoDoc.idProfile = schedaDocumento.docNumber;//schedaDocumento.systemId;
                    infoDoc.idProfile = schedaDocumento.systemId;
                    infoDoc.oggetto = schedaDocumento.oggetto.descrizione;
                    infoDoc.docNumber = schedaDocumento.docNumber;
                    infoDoc.tipoProto = schedaDocumento.tipoProto;
                    infoDoc.evidenza = schedaDocumento.evidenza;
                    } 
                    catch(Exception es) 
                    {
                        ErrorManager.redirect(page, es);
                    } 
                }?
        */

        public static bool addDocumentoInFolder(Page page, string idProfile, string idFolder, bool fascRapida, out bool isInFolder, out String message)
        {
            message = String.Empty;
            logger.Info("BEGIN");
            isInFolder = false;
            bool result = false;

            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);

                //True: se il documento è già classificato nella folder indicata, false altrimenti
                isInFolder = docsPaWS.IsDocumentoClassificatoInFolder(idProfile, idFolder);

                if (!isInFolder)
                {
                    Folder fol = FascicoliManager.getFolder(page, idFolder);
                    //se il doc non è già classificato nella folder indicata allora lo inserisco
                    bool outValue = false;
                    string msg = string.Empty;
                    Fascicolo fasc = DocsPAWA.FascicoliManager.getFascicoloById(page, fol.idFascicolo);
                    result = docsPaWS.FascicolazioneAddDocFolder(infoUtente, idProfile, fol, fasc.descrizione, out msg);
                    message = msg;
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            logger.Info("END");
            return result;
        }


        public static void addAreaLavoro(Page page, SchedaDocumento schedaDocumento)
        {
            try
            {
                Utente utente = UserManager.getUtente(page);
                Ruolo ruolo = UserManager.getRuolo(page);
                bool result = docsPaWS.DocumentoExecAddLavoro(getInfoDocumento(schedaDocumento).idProfile, getInfoDocumento(schedaDocumento).tipoProto, null, UserManager.getInfoUtente(page), (schedaDocumento.registro != null ? schedaDocumento.registro.systemId : ""));
                Hashtable listInArea = new Hashtable();
                listInArea.Add(schedaDocumento.docNumber, schedaDocumento);


                //(ListaDoc[keyN].numProt+ListaDoc[keyN].dataApertura,ListaDoc[keyN]);
                page.Session["listInArea"] = listInArea;


                if (!result)
                {
                    throw new Exception();
                }
            }
            //			catch(System.Web.Services.Protocols.SoapException es) 
            //			{
            //				ErrorManager.redirect(page, es);
            //			} 
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
        }

        public static void addAreaLavoro(Page page, InfoDocumento infoDocumento)
        {
            try
            {
                Utente utente = UserManager.getUtente(page);
                Ruolo ruolo = UserManager.getRuolo(page);
                bool result = docsPaWS.DocumentoExecAddLavoro(infoDocumento.idProfile, infoDocumento.tipoProto, null, UserManager.getInfoUtente(page), (infoDocumento.codRegistro != null ? infoDocumento.idRegistro : ""));
                // Hashtable listInArea = new Hashtable();

                // listInArea.Add(schedaDocumento.docNumber, schedaDocumento);


                //(ListaDoc[keyN].numProt+ListaDoc[keyN].dataApertura,ListaDoc[keyN]);
                // page.Session["listInArea"] = listInArea;


                if (!result)
                {
                    throw new Exception();
                }
            }
            //			catch(System.Web.Services.Protocols.SoapException es) 
            //			{
            //				ErrorManager.redirect(page, es);
            //			} 
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
        }

        public static void eliminaDaAreaLavoro(Page page, string idProfile, Fascicolo fasc)
        {
            try
            {
                Utente utente = UserManager.getUtente(page);
                Ruolo ruolo = UserManager.getRuolo(page);
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                bool result = docsPaWS.DocumentoCancellaAreaLavoro(infoUtente.idPeople, infoUtente.idCorrGlobali, idProfile, fasc);

                if (!result)
                {
                    throw new Exception();
                }
            }
            //			catch(System.Web.Services.Protocols.SoapException es) 
            //			{
            //				ErrorManager.redirect(page, es);
            //			} 
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
        }

        public static bool scollegaDocumento(Page page, string systemId)
        {
            bool result = false;
            try
            {

                result = docsPaWS.DocumentoScollegaCollegamento(systemId);
                if (!result)
                {
                    throw new Exception();
                }
            }

            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return result;
        }


        protected static DocsPAWA.DocsPaWR.AreaLavoro addInListaAreaLav(DocsPAWA.DocsPaWR.AreaLavoro areaLav, object oggetto)
        {
            object[] nuovaLista;

            if (areaLav.lista != null)
            {
                int len = areaLav.lista.Length;
                nuovaLista = new object[len + 1];
                areaLav.lista.CopyTo(nuovaLista, 0);
                nuovaLista[len] = oggetto;
            }
            else
            {
                nuovaLista = new object[1];
                nuovaLista[0] = oggetto;
            }
            areaLav.lista = nuovaLista;
            return areaLav;
        }


        public static InfoDocumento getInfoDocumento(SchedaDocumento schedaDocumento)
        {
            InfoDocumento infoDoc = new InfoDocumento();

            //infoDoc.idProfile = schedaDocumento.docNumber;//schedaDocumento.systemId;
            infoDoc.idProfile = schedaDocumento.systemId;
            infoDoc.oggetto = schedaDocumento.oggetto.descrizione;
            infoDoc.docNumber = schedaDocumento.docNumber;
            infoDoc.tipoProto = schedaDocumento.tipoProto;
            infoDoc.evidenza = schedaDocumento.evidenza;

            if (schedaDocumento.registro != null)
            {
                infoDoc.codRegistro = schedaDocumento.registro.codRegistro;
                infoDoc.idRegistro = schedaDocumento.registro.systemId;
            }

            if (schedaDocumento.protocollo != null)
            {
                infoDoc.numProt = schedaDocumento.protocollo.numero;
                infoDoc.daProtocollare = schedaDocumento.protocollo.daProtocollare;
                infoDoc.dataApertura = schedaDocumento.protocollo.dataProtocollazione;
                infoDoc.segnatura = schedaDocumento.protocollo.segnatura;

                if (schedaDocumento.protocollo.GetType().Equals(typeof(ProtocolloEntrata)))
                {
                    infoDoc.mittDest = new string[1];
                    ProtocolloEntrata pe = (ProtocolloEntrata)schedaDocumento.protocollo;

                    if (pe != null && pe.mittente != null && infoDoc.mittDest != null && infoDoc.mittDest.Length > 0)
                    {
                        infoDoc.mittDest[0] = pe.mittente.descrizione;
                    }
                }
                else if (schedaDocumento.protocollo.GetType().Equals(typeof(ProtocolloUscita)))
                {
                    ProtocolloUscita pu = (ProtocolloUscita)schedaDocumento.protocollo;
                    if (pu.destinatari != null)
                    {
                        infoDoc.mittDest = new string[pu.destinatari.Length];
                        for (int i = 0; i < pu.destinatari.Length; i++)
                            infoDoc.mittDest[i] = ((Corrispondente)pu.destinatari[i]).descrizione;
                    }
                }

            }
            else
            {
                infoDoc.dataApertura = schedaDocumento.dataCreazione;
            }

            infoDoc.privato = schedaDocumento.privato;
            infoDoc.personale = schedaDocumento.personale;

            return infoDoc;
        }

        public static DocsPAWA.DocsPaWR.ProtocolloDestinatario[] getDestinatariInteropAggConferma(Page page, string idProfile, Corrispondente corr)
        {
            try
            {
                DocsPaWR.ProtocolloDestinatario[] result = docsPaWS.InteroperabilitaAggiornamentoConferma(idProfile, corr);

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


        public static Oggetto[] getListaOggetti(Page page, string[] idRegistri, string queryDescrizione)
        {
            Oggetto[] result = null;

            //Tolgo i caratteri speciali dal campo descrizione oggetto
            queryDescrizione = queryDescrizione.Replace(System.Environment.NewLine, "");

            try
            {
                result = docsPaWS.DocumentoGetListaOggetti(getQueryOggetto(page, idRegistri, queryDescrizione, ""));

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return result;
        }

        //Metodo che consente la ricerca oltre che per descrizione anche per codice sull'oggettario
        public static Oggetto[] getListaOggettiByCod(Page page, string[] idRegistri, string queryDescrizione, string queryCodice)
        {
            Oggetto[] result = null;

            //Tolgo i caratteri speciali dal campo descrizione oggetto
            queryDescrizione = queryDescrizione.Replace(System.Environment.NewLine, "");

            try
            {
                result = docsPaWS.DocumentoGetListaOggetti(getQueryOggetto(page, idRegistri, queryDescrizione, queryCodice));

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return result;
        }

        protected static DocumentoQueryOggetto getQueryOggetto(Page page, string[] registri, string queryDescrizione, string queryCodice)
        {
            DocumentoQueryOggetto query = new DocumentoQueryOggetto();
            Utente utente = UserManager.getUtente(page);
            if (registri != null && registri.Length >= 0)
            {
                query.idRegistri = new string[registri.Length];
                for (int i = 0; i < registri.Length; i++)
                    query.idRegistri[i] = registri[i];
            }
            query.idAmministrazione = utente.idAmministrazione;// ConfigurationManager.AppSettings["ID_AMMINISTRAZIONE"];
            query.queryDescrizione = queryDescrizione;
            query.queryCodice = queryCodice;

            return query;
        }

        public static InfoDocumento[] getDocAll(DocsPAWA.DocsPaWR.InfoUtente Safe, Page page, DocsPaWR.FiltroRicerca[][] query)
        {
            InfoDocumento[] DocS = null;
            try
            {
                DocS = docsPaWS.DocumentoGetAll(query, Safe);
                if (DocS == null)
                    throw new Exception();
                return DocS;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        public static InfoDocumento[] getQueryInfoDocumento(string idGruppo, string idPeople, Page page, DocsPaWR.FiltroRicerca[][] query)
        {
            //docWS.getQuery(xmlQueryV,xmlSafe);
            try
            {
                InfoDocumento[] DocS = docsPaWS.DocumentoGetQueryDocumento(idGruppo, idPeople, query);
                if (DocS == null)
                {
                    throw new Exception();
                }
                return DocS;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            //catch () {}
            return null;
        }

        public static string getQueryTipoProto(Page page, string docNumber)
        {
            string result = "";
            try
            {

                result = docsPaWS.DocumentoGetTipoProto(docNumber);
                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
        }


        #region paginazione


        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="infoUtente"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static InfoDocumento[] FullTextSearch(Page page, InfoUtente infoUtente, ref FullTextSearchContext context)
        {
            InfoDocumento[] retValue = null;

            try
            {
                retValue = docsPaWS.FullTextSearch(infoUtente, ref context);

                if (retValue == null)
                    throw new Exception();

            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static int FullTextSearchMaxRows(Page page, InfoUtente infoUtente)
        {
            int retValue = 0;

            try
            {
                retValue = docsPaWS.FullTextSearchMaxRows(infoUtente);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return retValue;
        }

        /// <summary>
        /// Ricerca fulltext su documentale corrente
        /// </summary>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>
        /// <param name="page"></param>
        /// <param name="query"></param>
        /// <param name="numPage"></param>
        /// <param name="numTotPage"></param>
        /// <param name="nRec"></param>
        /// <returns></returns>
        public static InfoDocumento[] getQueryFullTextInfoDocumentoPaging(
                        Page page,
                        string testo,
                        string idRegistro,
                        InfoUtente infoUtente,
                        DocsPaWR.FiltroRicerca[][] query,
                        int numPage,
                        out int numTotPage,
                        out int nRec)
        {
            numTotPage = 0;
            nRec = 0;

            try
            {
                InfoDocumento[] DocS = docsPaWS.DocumentoGetQueryFullTextDocumentoPaging(testo, idRegistro, infoUtente, query, numPage, out numTotPage, out nRec);

                if (DocS == null)
                {
                    throw new Exception();
                }

                return DocS;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        /// <summary>
        /// Verifica se l'utente ha i diritti per accedere al documento
        /// </summary>
        /// <param name="page"></param>
        /// <param name="docNumber"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool CheckDocumentUserVisibility(Page page, string docNumber, DocsPaWR.InfoUtente infoUtente)
        {
            bool retValue = false;

            try
            {
                retValue = docsPaWS.DocumentoCheckUserVisibility(docNumber, infoUtente);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return retValue;
        }

        private static bool IsRicercaFullText(FiltroRicerca[][] objQueryList, out string textToSearch)
        {
            string oggetto = string.Empty;
            string testoContenuto = string.Empty;

            textToSearch = string.Empty;

            bool ricercaFullText = false;

            for (int i = 0; i < objQueryList.Length; i++)
            {
                for (int j = 0; j < objQueryList[i].Length; j++)
                {
                    FiltroRicerca f = objQueryList[i][j];

                    switch (f.argomento)
                    {
                        case "RICERCA_FULL_TEXT":
                            if (f.valore != "0")
                                ricercaFullText = true;
                            break;

                        case "TESTO_RICERCA_FULL_TEXT":
                            testoContenuto = f.valore;
                            break;

                        case "OGGETTO":
                            oggetto = f.valore;
                            break;

                        default:
                            break;
                    }
                }
            }

            textToSearch = (ricercaFullText ? testoContenuto : oggetto);
            return (ricercaFullText && textToSearch != string.Empty ? true : false);
        }

        public static InfoDocumento[] getQueryInfoDocumentoPaging(string idGruppo, string idPeople, Page page, DocsPaWR.FiltroRicerca[][] query, int numPage, out int numTotPage, out int nRec, bool comingPopUp, bool grigi, bool security, bool getIdProfilesList, out SearchResultInfo[] idProfilesList)
        {
            // La lista dei system id dei documenti restituiti dalla ricerca
            SearchResultInfo[] idProfiles = null;

            nRec = 0;
            numTotPage = 0;
            try
            {
                InfoDocumento[] DocS = null;
                string textToSearch = string.Empty;

                if (!IsRicercaFullText(query, out textToSearch))
                {
                    DocS = docsPaWS.DocumentoGetQueryDocumentoPaging(idGruppo, idPeople, query, comingPopUp, grigi, numPage, security, getIdProfilesList, out numTotPage, out nRec, out idProfiles);
                }
                else
                {
                    // reperimento oggetto infoutente
                    InfoUtente infoUtente = UserManager.getInfoUtente(page);

                    // Reperimento dalla sessione del contesto di ricerca fulltext
                    FullTextSearchContext context = page.Session["FULL_TEXT_CONTEXT"] as FullTextSearchContext;

                    if (context == null)
                        // Prima ricerca fulltext
                        context = new FullTextSearchContext();
                    else if (!textToSearch.Equals(context.TextToSearch))
                        // Se il testo inserito per la ricerca è differente
                        // da quello presente in sessione viene creato 
                        // un nuovo oggetto di contesto per la ricerca
                        context = new FullTextSearchContext();

                    // Impostazione indice pagina richiesta
                    context.RequestedPageNumber = numPage;
                    // Impostazione testo da ricercare
                    context.TextToSearch = textToSearch;

                    // Ricerca fulltext
                    DocS = FullTextSearch(page, infoUtente, ref context);

                    // Reperimento numero pagine e record totali
                    numTotPage = context.TotalPageNumber;
                    nRec = context.TotalRecordCount;

                    // Impostazione in sessione del contesto di ricerca fulltext
                    page.Session["FULL_TEXT_CONTEXT"] = context;
                }

                if (DocS == null)
                {
                    throw new Exception();
                }

                // Impostazione della lista dei sisyem id dei documento
                idProfilesList = idProfiles;

                return DocS;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            idProfilesList = idProfiles;
            return null;
        }

        public static int getNumDocInRisposta(string idGruppo, string idPeople, Page page, DocsPaWR.FiltroRicerca[][] query, bool security)
        {
            int numDocInRisposta = 0;
            try
            {

                string textToSearch = string.Empty;

                if (!IsRicercaFullText(query, out textToSearch))
                {
                    numDocInRisposta = docsPaWS.DocumentoGetNumDocInRisposta(idGruppo, idPeople, query, security);
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return numDocInRisposta;
        }

        #endregion paginazione

        #region ACL


        public static DocumentoDiritto[] getListaVisibilita(Page page, string idProfile, bool cercaRimossi)
        {
            DocumentoDiritto[] listaVisibilita = null;
            try
            {
                listaVisibilita = docsPaWS.DocumentoGetVisibilita(UserManager.getInfoUtente(), idProfile, cercaRimossi);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return listaVisibilita;
        }

        public static bool editingACL(DocsPAWA.DocsPaWR.DocumentoDiritto docDiritto, string personOrGroup, DocsPaWR.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.EditingACL(docDiritto, personOrGroup, infoUtente);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        public static bool ripristinaACL(DocsPAWA.DocsPaWR.DocumentoDiritto docDiritto, string personOrGroup, DocsPaWR.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.RipristinaACL(docDiritto, personOrGroup, infoUtente);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        public static int verificaDeletedACL(string idProfile, DocsPaWR.InfoUtente infoUtente)
        {
            int result = -1;
            try
            {
                result = docsPaWS.VerificaDeletedACL(idProfile, infoUtente);
            }
            catch (Exception e)
            {
                result = -1;

            }
            return result;
        }
        /// <summary>
        /// *  CASI:
        /// result = 0 --> ACL rimossa ;
        /// result = 1 --> documento rimosso ;
        /// result =2 --> documento in normali condizioni ;
        /// result =-1 --> errore generico ;
        /// </summary>
        /// <param name="tipoObj"></param>
        /// <param name="systemId"></param>
        /// <param name="infoUtente"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static int verificaACL(string tipoObj, string systemId, DocsPaWR.InfoUtente infoUtente, out string errorMessage)
        {
            int result = -1;
            errorMessage = "";
            try
            {
                result = docsPaWS.VerificaACL(tipoObj, systemId, infoUtente, out errorMessage);
            }
            catch (Exception e)
            {
                result = -1;

            }
            return result;
        }

        public static bool dirittoProprietario(string systemId, DocsPaWR.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.DirittoProprietario(systemId, infoUtente);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        public static StoriaDirittoDocumento[] getStoriaVisibilita(Page page, string idProfile, string tipoObj, DocsPaWR.InfoUtente infoUtente)
        {
            StoriaDirittoDocumento[] listaStoria = null;
            try
            {
                listaStoria = docsPaWS.DocumentoGetStoriaVisibilita(idProfile, tipoObj, infoUtente);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return listaStoria;
        }

        #endregion

        public static SchedaDocumento annullaProtocollo(Page page, string autoriz)
        {
            try
            {
                SchedaDocumento schedaDoc = getDocumentoSelezionato(page);
                Utente utente = (Utente)GetSessionValue("userData");
                Ruolo ruolo = (Ruolo)GetSessionValue("userRuolo");
                InfoDocumento infoDocumento = getInfoDocumento(schedaDoc);
                DocsPaWR.ProtocolloAnnullato protAnnullato = new DocsPAWA.DocsPaWR.ProtocolloAnnullato();
                protAnnullato.autorizzazione = autoriz;

                InfoUtente infoUtente = UserManager.getInfoUtente(page);

                schedaDoc.protocollo.protocolloAnnullato = protAnnullato;
                string msg = ProfilazioneDocManager.VerifyAndSetTipoDoc(UserManager.getInfoUtente(page), ref schedaDoc, page);
                if (string.IsNullOrEmpty(msg))
                {

                    if (!docsPaWS.DocumentoExecAnnullaProt(infoUtente, ref schedaDoc, protAnnullato))
                        throw new Exception();

                    setDocumentoSelezionato(page, schedaDoc);
                    setDocumentoInLavorazione(page, schedaDoc);
                }
                else
                {
                    page.ClientScript.RegisterStartupScript(page.GetType(), "controllaEValorizzaTipologiaDoc", "alert('" + msg + "');", true);
                }

                return schedaDoc;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static DocsPAWA.DocsPaWR.SendDocumentResponse spedisciDocMail(Page page, DocsPaWR.SchedaDocumento schedaDocumento, string mailAddress)
        {
            DocsPaWR.SendDocumentResponse retValue = null;

            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);

                retValue = docsPaWS.DocumentoSpedisciMail(schedaDocumento, infoUtente, mailAddress, true);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return retValue;
        }

        public static DocsPAWA.DocsPaWR.SendDocumentResponse spedisciDoc(Page page, DocsPaWR.SchedaDocumento schedaDocumento)
        {
            DocsPaWR.SendDocumentResponse retValue = null;

            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);

                retValue = docsPaWS.DocumentoSpedisci(schedaDocumento, infoUtente, true);
            }
            catch (Exception es)
            {
                //ErrorManager.redirect(page,es);
                ErrorManager.redirect(page, es, "Spedizione documento");
            }

            return retValue;
        }

        /// <summary>
        /// verificaSpedizioneDocumento ritorna i seguenti valori:
        /// 0: se il documento non è stato mai spedito
        /// 1: se è stato già spedito
        /// 2: se si è verificato un errore
        /// </summary>
        /// <param name="idProfile"></param>
        /// <returns></returns>
        public static int verificaSpedizioneDocumento(Page page, string idProfile)
        {
            int resultInt = 0; // 0 (il documento non è mai stato spedito)
            try
            {
                bool result = docsPaWS.DocumentoVerificaSpedizione(idProfile);
                if (result)// 1 (il documento è stato già spedito)
                    resultInt = 1;

            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
                resultInt = 2;
            }
            return resultInt;

        }

        public static void spedisciFax(Page page, DocsPaWR.SchedaDocumento schedaDocumento)
        {
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                Registro reg = UserManager.getRegistroSelezionato(page);
                bool result = docsPaWS.FaxInvio(infoUtente, reg, schedaDocumento);

                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
        }


        public static InfoDocumento getRisultatoRicerca(Page page)
        {
            return (InfoDocumento)GetSessionValue("tabRicDoc.InfoDoc");
        }

        public static void setRisultatoRicerca(Page page, InfoDocumento infoDoc)
        {
            SetSessionValue("tabRicDoc.InfoDoc", infoDoc);
        }

        public static void removeRisultatoRicerca(Page page)
        {
            RemoveSessionValue("tabRicDoc.InfoDoc");
        }

        public static void setDatagridDocumento(Page page, DataTable docs)
        {
            SetSessionValue("tabRicDoc.RigheDG", docs);
        }

        public static DataTable getDatagridDocumento(Page page)
        {
            return (DataTable)GetSessionValue("tabRicDoc.RigheDG");
        }

        public static void removeDatagridDocumento(Page page)
        {
            RemoveSessionValue("tabRicDoc.RigheDG");
        }

        public static SchedaDocumento getDocumentoSelezionato()
        {
            return (SchedaDocumento)GetSessionValue("gestioneDoc.schedaDocumento");
        }

        public static SchedaDocumento getDocumentoSelezionato(Page page)
        {
            return (SchedaDocumento)getDocumentoSelezionato();
        }

        public static void setDocumentoSelezionato(SchedaDocumento schedaDoc)
        {
            if (schedaDoc != null)
            {
                SchedaDocumento oldSchedaDoc = (SchedaDocumento)GetSessionValue("gestioneDoc.schedaDocumento");
                if (oldSchedaDoc != null && oldSchedaDoc.systemId != null && !(oldSchedaDoc.systemId.Equals(schedaDoc.systemId)))
                    FileManager.removeSelectedFile();
                SetSessionValue("gestioneDoc.schedaDocumento", schedaDoc);
            }
        }

        public static void setDocumentoSelezionato(Page page, SchedaDocumento schedaDoc)
        {
            setDocumentoSelezionato(schedaDoc);
        }

        public static void removeDocumentoSelezionato()
        {
            RemoveSessionValue("gestioneDoc.schedaDocumento");
            FileManager.removeSelectedFile();
        }

        public static void removeDocumentoSelezionato(Page page)
        {
            removeDocumentoSelezionato();
        }

        [Obsolete("Utilizzare il metodo getDocumentoSelezionato")]
        public static SchedaDocumento getDocumentoInLavorazione()
        {
            return (SchedaDocumento)GetSessionValue("tabDoc.schedaDocumento");
        }

        [Obsolete("Utilizzare il metodo getDocumentoSelezionato")]
        public static SchedaDocumento getDocumentoInLavorazione(Page page)
        {
            return getDocumentoInLavorazione();
        }

        /// <summary>
        /// Indica se il documento correntemente in lavorazione è 
        /// nel contesto della sessione di lavoro temporanea o meno.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Tramite la sessione di lavoro temporanea è possibile
        /// acquisire file anche se i metadati del documento ancora non sono stati salvati
        /// </remarks>
        public static bool onSessionRepositoryContext()
        {
            SchedaDocumento schedaDocumento = getDocumentoInLavorazione();

            return (schedaDocumento != null && string.IsNullOrEmpty(schedaDocumento.systemId) && schedaDocumento.repositoryContext != null);
        }

        [Obsolete("Utilizzare il metodo setDocumentoSelezionato")]
        public static void setDocumentoInLavorazione(SchedaDocumento schedaDoc)
        {
            SetSessionValue("tabDoc.schedaDocumento", schedaDoc);
        }

        [Obsolete("Utilizzare il metodo setDocumentoSelezionato")]
        public static void setDocumentoInLavorazione(Page page, SchedaDocumento schedaDoc)
        {
            setDocumentoInLavorazione(schedaDoc);
        }

        [Obsolete("Utilizzare il metodo removeDocumentoSelezionato")]
        public static void removeDocumentoInLavorazione()
        {
            RemoveSessionValue("tabDoc.schedaDocumento");
        }

        [Obsolete("Utilizzare il metodo removeDocumentoSelezionato")]
        public static void removeDocumentoInLavorazione(Page page)
        {
            removeDocumentoInLavorazione();
        }

        public static Corrispondente getCorrInDettagli(Page page)
        {
            return (Corrispondente)GetSessionValue("dettagliCorr.corr");
        }

        public static void setCorrInDettagli(Page page, Corrispondente corr)
        {
            SetSessionValue("dettagliCorr.corr", corr);
        }

        public static void removeCorrInDettagli(Page page)
        {
            RemoveSessionValue("dettagliCorr.corr");
        }

        //allegati

        /*
        public static Allegato getAllegatoInLavorazione(Page page) 
        {
            return (Allegato) page.Session["docAllegato.allegato"];
        }

        public static void setAllegatoInLavorazione(Page page, Allegato allegato) 
        {
            page.Session["docAllegato.allegato"] = allegato;
        }

        public static void removeAllegatoInLavorazione(Page page) 
        {
            page.Session.Remove("docAllegato.allegatoDocumento");
        }
        */

        //allegato - datagrid
        public static ArrayList getDataGridAllegati(Page page)
        {
            return (ArrayList)GetSessionValue("docAllegati.dataGridAl");
        }

        public static void setDataGridAllegati(Page page, ArrayList arrayDataGrid)
        {
            SetSessionValue("docAllegati.dataGridAl", arrayDataGrid);
        }

        public static void removeDataGridAllegati(Page page)
        {
            RemoveSessionValue("docAllegati.dataGridAl");
        }

        //documenti in risposta
        public static DataTable getDataGridProtocolliUscita(Page page)
        {
            return (DataTable)GetSessionValue("RicercaProtocolloUscita.RigheDG");
        }

        public static void setDataGridProtocolliUscita(Page page, DataTable docs)
        {
            SetSessionValue("RicercaProtocolloUscita.RigheDG", docs);
        }

        public static void removeDataGridProtocolliUscita(Page page)
        {
            RemoveSessionValue("RicercaProtocolloUscita.RigheDG");
        }

        public static DataTable getDataGridDestinatari(Page page)
        {
            return (DataTable)GetSessionValue("ScegliDestinatari.RigheDG");
        }

        public static void setDataGridDestinatari(Page page, DataTable docs)
        {
            SetSessionValue("ScegliDestinatari.RigheDG", docs);
        }

        public static void removeDataGridDestinatari(Page page)
        {
            RemoveSessionValue("ScegliDestinatari.RigheDG");
        }




        public static DataTable getDataGridProtocolliIngresso(Page page)
        {
            return (DataTable)GetSessionValue("RicercaProtocolloIngresso.RigheDG");
        }

        public static void setDataGridProtocolliIngresso(Page page, DataTable docs)
        {
            SetSessionValue("RicercaProtocolloIngresso.RigheDG", docs);
        }

        public static void removeDataGridProtocolliIngresso(Page page)
        {
            RemoveSessionValue("RicercaProtocolloIngresso.RigheDG");
        }


        //ricerca documenti in classifica
        public static DataTable getDataGridDocumentiPerClassifica(Page page)
        {
            return (DataTable)GetSessionValue("DocumentiPerClassifica.RigheDG");
        }

        public static void setDataGridDocumentiPerClassifica(Page page, DataTable docs)
        {
            SetSessionValue("DocumentiPerClassifica.RigheDG", docs);
        }

        public static void removeDataGridDocumentiPerClassifica(Page page)
        {
            RemoveSessionValue("DocumentiPerClassifica.RigheDG");
        }

        #region infoDocumento

        public static InfoDocumento getInfoDocumento(Page page)
        {
            return (InfoDocumento)GetSessionValue("RicercaProtocolliUscita.infoDoc");
        }

        public static void setInfoDocumento(Page page, InfoDocumento infoDoc)
        {
            SetSessionValue("RicercaProtocolliUscita.infoDoc", infoDoc);
        }

        public static void removeInfoDocumento(Page page)
        {
            RemoveSessionValue("RicercaProtocolliUscita.infoDoc");
        }
        #endregion

        #region ricerca documento
        public static InfoDocumento getInfoDocumentoRicerca(Page page)
        {
            return (InfoDocumento)GetSessionValue("RicercaDocumenti.infoDoc");
        }

        public static void setInfoDocumentoRicerca(Page page, InfoDocumento infoDoc)
        {
            SetSessionValue("RicercaDocumenti.infoDoc", infoDoc);
        }

        public static void removeInfoDocumentoRicerca(Page page)
        {
            RemoveSessionValue("RicercaDocumenti.infoDoc");
        }




        #endregion ricerca documento

        //versioni - datagrid
        public static ArrayList getDataGridVersioni(Page page)
        {
            return (ArrayList)GetSessionValue("docVersioni.dataGridVers");
        }

        public static void setDataGridVersioni(Page page, ArrayList arrayDataGrid)
        {
            SetSessionValue("docVersioni.dataGridVers", arrayDataGrid);
        }

        public static void removeDataGridVersioni(Page page)
        {
            RemoveSessionValue("docVersioni.dataGridVers");
        }


        //areaLavoro - datagrid
        public static ArrayList getDataGridAreaLavoro(Page page)
        {
            return (ArrayList)GetSessionValue("areaDiLavoro.dataGridAreaLav");
        }

        public static void setDataGridAreaLavoro(Page page, ArrayList arrayDataGrid)
        {
            SetSessionValue("areaDiLavoro.dataGridAreaLav", arrayDataGrid);
        }

        public static void removeDataGridAreaLavoro(Page page)
        {
            RemoveSessionValue("areaDiLavoro.dataGridAreaLav");
        }

        #region commento RISPOSTA AL PROTOCOLLO - attulamente non utilizzato

        //		//aggiunto per risposta al protocollo
        //		public static void setDataGridlistaDocInRisposta(Page page, ArrayList arrayDataGrid) 
        //		{
        //			page.Session["risposta.dataGridlistaDocInRis"] = arrayDataGrid;
        //		}
        //
        //		//aggiunto per risposta al protocollo
        //		public static ArrayList getDataGridlistaDocInRisposta(Page page) 
        //		{
        //			return (ArrayList) page.Session["risposta.dataGridlistaDocInRis"];
        //		}
        //
        //		//aggiunto per risposta al protocollo
        //		public static void removeDataGridlistaDocInRisposta(Page page) 
        //		{
        //			page.Session.Remove("risposta.dataGridlistaDocInRis");
        //		}
        #endregion

        #region Annulla Predisposizione

        public static SchedaDocumento annullaPredisposizione(InfoUtente infoUtente, SchedaDocumento schedaDocumento)
        {
            if (docsPaWS.DocumentoAnnullaPredisposizione(infoUtente, schedaDocumento))
            {

                schedaDocumento.protocollo = null;
                schedaDocumento.tipoProto = "G";
                schedaDocumento.predisponiProtocollazione = false;
                schedaDocumento.registro = null;
                schedaDocumento.mezzoSpedizione = "0";
                schedaDocumento.descMezzoSpedizione = "";

            }
            else
                schedaDocumento = null;
            return schedaDocumento;
        }

        #endregion

        #region Variabile sessione "TheHash"
        //Utilizzata per memorizzare l'hashtable 

        public static void removeHash(Page page)
        {
            page.Session.Remove("Hash");
        }

        public static Hashtable getHash(Page page)
        {
            return (Hashtable)page.Session["Hash"];
        }

        public static void setHash(Page page, Hashtable hashTable)
        {
            page.Session["Hash"] = hashTable;
        }
        #endregion

        #region Variabile sessione "ricDoc.listaFiltri"

        public static void removeFiltroRicDoc(Page page)
        {
            RemoveSessionValue("ricDoc.listaFiltri");
        }

        public static FiltroRicerca[][] getFiltroRicDoc(Page page)
        {
            return (FiltroRicerca[][])GetSessionValue("ricDoc.listaFiltri");
        }

        public static void setFiltroRicDoc(Page page, FiltroRicerca[][] filtroRicerca)
        {
            SetSessionValue("ricDoc.listaFiltri", filtroRicerca);
        }

        /// <summary>
        /// indipendentemente con quale ruolo o salvato una ricerca in ADL, devo ripete la ricerca
        /// salvata con il ruolo corrente, altrimenti vedo i documenti dell'ADL dell'altro mio ruolo
        /// </summary>
        /// <param name="qv"></param>
        /// <returns></returns>
        public static FiltroRicerca[][] setFiltroAdL(FiltroRicerca[][] qv, string inAdL)
        {
            if (inAdL != null && inAdL.Equals("1"))
                if (qv != null)
                    for (int i = 0; i < qv.Length; i++)
                        for (int j = 0; j < qv[i].Length; j++)
                        {
                            if (qv[i][j].argomento.Equals("DOC_IN_ADL"))
                            {
                                qv[i][j].valore = UserManager.getUtente().idPeople + "@" + UserManager.getRuolo().systemId;
                                break;
                            }

                        }

            return qv;
        }
        #endregion

        #region Variabile sessione "ricTrasm.listaFiltri"

        public static void removeFiltroRicTrasm(Page page)
        {
            RemoveSessionValue("ricTrasm.listaFiltri");
        }

        public static FiltroRicerca[] getFiltroRicTrasm(Page page)
        {
            return (FiltroRicerca[])GetSessionValue("ricTrasm.listaFiltri");
        }

        public static void setFiltroRicTrasm(Page page, FiltroRicerca[] filtroRicerca)
        {
            SetSessionValue("ricTrasm.listaFiltri", filtroRicerca);
        }
        #endregion

        #region Variabile sessione "elencoReg"
        public static void removeElencoReg(Page page)
        {
            RemoveSessionValue("elencoReg");
        }

        public static Registro[] getElencoReg(Page page)
        {
            return (Registro[])GetSessionValue("elencoReg");
        }

        public static void setElencoReg(Page page, Registro[] listaRegistro)
        {
            SetSessionValue("elencoReg", listaRegistro);
        }
        #endregion

        #region Variabile sessione "ListaDocProt"
        public static void removeListaDocProt(Page page)
        {
            RemoveSessionValue("tabRicDoc.Listadoc");
        }

        public static InfoDocumento[] getListaDocProt(Page page)
        {
            return (InfoDocumento[])GetSessionValue("tabRicDoc.Listadoc");
        }

        public static void setListaDocProt(Page page, InfoDocumento[] lista)
        {
            SetSessionValue("tabRicDoc.Listadoc", lista);
        }
        #endregion

        #region Variabile sessione "ListaDocNonProt"
        public static void removeListaNonDocProt(Page page)
        {
            RemoveSessionValue("tabRicDoc.Listadoc");
        }

        public static InfoDocumento[] getListaDocNonProt(Page page)
        {
            return (InfoDocumento[])GetSessionValue("tabRicDoc.Listadoc");
        }

        public static void setListaDocNonProt(Page page, InfoDocumento[] lista)
        {
            SetSessionValue("tabRicDoc.Listadoc", lista);
        }
        #endregion


        #region Variabile sessione "ricDoc.ListaParoleChiave"

        public static void removeListaParoleChiaveSel(Page page)
        {
            RemoveSessionValue("ricDoc.listaParoleChiave");
        }

        public static DocsPAWA.DocsPaWR.DocumentoParolaChiave[] getListaParoleChiaveSel(Page page)
        {
            return (DocsPAWA.DocsPaWR.DocumentoParolaChiave[])GetSessionValue("ricDoc.listaParoleChiave");
        }

        public static void setListaParoleChiaveSel(Page page, DocsPaWR.DocumentoParolaChiave[] listaParoleChiave)
        {
            SetSessionValue("ricDoc.listaParoleChiave", listaParoleChiave);
        }
        #endregion

        public static SchedaDocumento InoltraDocumento(Page page, SchedaDocumento schedaOLDDoc, bool eUffRef)
        {
            try
            {
                //data un documento grigio o un protocollo in ingresso 
                //crea un nuovo documento predisposto alla protocollazione in uscita
                //con lo stesso oggetto e con documento principale e allegati 
                //trasformati in allegati del nuovo documento
                SchedaDocumento schedaNewDoc = docsPaWS.DocumentoInoltraDoc(UserManager.getInfoUtente(page), UserManager.getRuolo(page), schedaOLDDoc);
                if (schedaNewDoc == null)
                {
                    string message = "Errore imprevisto dell'applicazione. Ritentare l'operazione.";
                    throw new Exception(message);
                }
                return schedaNewDoc;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        public static SchedaDocumento riproponiDati(Page page, SchedaDocumento schedaDoc, bool eUffRef)
        {
            try
            {
                //data una scheda documento ne crea una nuova riproponendo l'oggetto e il mittente (o destinatario)
                SchedaDocumento schedaNewDoc = NewSchedaDocumento(page);
                schedaNewDoc.oggetto = schedaDoc.oggetto;
                schedaNewDoc.registro = schedaDoc.registro;
                schedaNewDoc.tipoProto = schedaDoc.tipoProto;
                schedaNewDoc.idPeople = schedaDoc.idPeople;
                schedaNewDoc.userId = schedaDoc.userId;
                schedaNewDoc.typeId = schedaDoc.typeId;
                schedaNewDoc.appId = schedaDoc.appId;
                schedaNewDoc.privato = schedaDoc.privato;
                schedaNewDoc.mezzoSpedizione = schedaDoc.mezzoSpedizione;
                schedaNewDoc.descMezzoSpedizione = schedaDoc.descMezzoSpedizione;
                schedaNewDoc.tipologiaAtto = schedaDoc.tipologiaAtto;
                RemoveSessionValue("template");
                schedaNewDoc.template = schedaDoc.template;
                //schedaNewDoc.noteDocumento = schedaDoc.noteDocumento;

                switch (schedaDoc.tipoProto)
                {
                    case "A":
                        schedaNewDoc.protocollo = new DocsPAWA.DocsPaWR.ProtocolloEntrata();
                        ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaNewDoc.protocollo).mittente = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittente;
                        ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaNewDoc.protocollo).mittenteIntermedio = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenteIntermedio;
                        if (eUffRef)
                        {
                            ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaNewDoc.protocollo).ufficioReferente = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).ufficioReferente;
                        }
                        if (DocumentManager.isEnableMittentiMultipli())
                        {
                            ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaNewDoc.protocollo).mittenti = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti;
                        }
                        break;
                    case "P":
                        schedaNewDoc.protocollo = new DocsPAWA.DocsPaWR.ProtocolloUscita();
                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaNewDoc.protocollo).mittente = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).mittente;
                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaNewDoc.protocollo).destinatari = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari;
                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaNewDoc.protocollo).destinatariConoscenza = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza;
                        if (eUffRef)
                        {
                            ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaNewDoc.protocollo).ufficioReferente = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).ufficioReferente;
                        }

                        // Pulizia delle informazioni sui protocolli destinatari, se il protocollo è un protocollo
                        // in uscita
                        foreach (Corrispondente destinatario in ((ProtocolloUscita)schedaNewDoc.protocollo).destinatari)
                            destinatario.protocolloDestinatario = null;

                        break;
                    case "I":
                        schedaNewDoc.protocollo = new DocsPAWA.DocsPaWR.ProtocolloInterno();
                        ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).mittente = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).mittente;
                        ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).destinatari = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari;
                        ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).destinatariConoscenza = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza;
                        if (eUffRef)
                        {
                            ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaNewDoc.protocollo).ufficioReferente = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).ufficioReferente;
                        }
                        break;
                }
                return schedaNewDoc;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        public static SchedaDocumento riproponiConCopiaDoc(Page page, SchedaDocumento schedaOLDDoc, bool eUffRef)
        {
            try
            {
                //data una scheda documento ne crea una nuova riproponendo l'oggetto e il mittente (o destinatario), il documento principale e gli allegati
                SchedaDocumento schedaNewDoc = docsPaWS.DocumentoRiproponiConCopiaDoc(UserManager.getInfoUtente(page), UserManager.getRuolo(page), schedaOLDDoc);
                if (schedaNewDoc == null)
                {
                    string message = "Errore imprevisto dell'applicazione. Ritentare l'operazione.";
                    throw new Exception(message);
                }
                return schedaNewDoc;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        public static SchedaDocumento riproponiDatiGrigio(Page page, SchedaDocumento schedaDoc, bool eUffRef, bool daRisposta)
        {
            try
            {
                //data una scheda documento ne crea una nuova riproponendo l'oggetto, tipologia e parole chiave
                SchedaDocumento schedaNewDoc = NewSchedaDocumento(page);
                schedaNewDoc.oggetto = schedaDoc.oggetto;
                schedaNewDoc.docNumber = "";
                schedaNewDoc.dataCreazione = "";
                schedaNewDoc.registro = schedaDoc.registro;
                schedaNewDoc.idPeople = schedaDoc.idPeople;
                schedaNewDoc.userId = schedaDoc.userId;
                schedaNewDoc.typeId = schedaDoc.typeId;
                schedaNewDoc.privato = schedaDoc.privato;
                schedaNewDoc.paroleChiave = schedaDoc.paroleChiave;
                schedaNewDoc.tipologiaAtto = schedaDoc.tipologiaAtto;
                schedaNewDoc.template = schedaDoc.template;
                schedaNewDoc.fascicolato = schedaDoc.fascicolato;
                schedaNewDoc.folder = schedaDoc.folder;

                //se provengo dal bottone di creazione del doc grigio in risposta
                // Viene popolato l'oggetto risposta
                if (daRisposta)
                {
                    DocsPaWR.InfoDocumento infoDoc = getInfoDocumento(schedaDoc);
                    schedaNewDoc.rispostaDocumento = infoDoc;
                }

                return schedaNewDoc;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        public static SchedaDocumento riproponiDatiRispIngresso(Page page, SchedaDocumento schedaDoc, DocsPAWA.DocsPaWR.Corrispondente destSelezionato)
        {
            try
            {
                //data una scheda doc in uscita ne viene creata una in ingresso
                //riproponendo l'oggetto e scegliendo il mittente tra i vari destinatari.
                SchedaDocumento schedaNewDoc = NewSchedaDocumento(page);
                schedaNewDoc.oggetto = schedaDoc.oggetto;
                schedaNewDoc.registro = schedaDoc.registro;
                //schedaNewDoc.tipoProto = schedaDoc.tipoProto;
                schedaNewDoc.idPeople = schedaDoc.idPeople;
                schedaNewDoc.userId = schedaDoc.userId;
                schedaNewDoc.typeId = schedaDoc.typeId;
                schedaNewDoc.appId = schedaDoc.appId;
                schedaNewDoc.privato = "0";
                schedaNewDoc.tipoProto = "A";
                if (schedaDoc.tipoProto.Equals("P"))
                {
                    schedaNewDoc.protocollo = new DocsPAWA.DocsPaWR.ProtocolloEntrata();
                    ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaNewDoc.protocollo).mittente = new DocsPAWA.DocsPaWR.Corrispondente();
                    ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaNewDoc.protocollo).mittente = destSelezionato;
                }

                if (schedaDoc.oggetto != null)
                {
                    schedaNewDoc.oggetto = schedaDoc.oggetto;
                }

                //Viene popolato l'oggetto risposta al protocollo:
                if (schedaDoc.protocollo != null && schedaDoc.protocollo.numero != null
                    && !schedaDoc.protocollo.numero.Equals(""))
                {
                    DocsPaWR.InfoDocumento infoDoc = getInfoDocumento(schedaDoc);
                    schedaNewDoc.rispostaDocumento = infoDoc;
                }

                Boolean found = false;
                //Viene popolato l'oggetto risposta al protocollo:
                if (schedaDoc.protocollo != null && schedaDoc.protocollo.numero != null
                    && !schedaDoc.protocollo.numero.Equals(""))
                {
                    DocsPaWR.InfoDocumento infoDoc = getInfoDocumento(schedaDoc);
                    schedaNewDoc.rispostaDocumento = infoDoc;
                    found = true;
                }

                //Nel caso di predisposto alla protocollazione 20/08/2010 Fabio altrimenti non crea documento in risposta
                if (schedaDoc.protocollo != null && schedaDoc.docNumber != null && found == false && schedaDoc.protocollo.segnatura != null)
                {
                    DocsPaWR.InfoDocumento infoDoc = getInfoDocumento(schedaDoc);
                    schedaNewDoc.rispostaDocumento = infoDoc;
                }

                return schedaNewDoc;

            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        public static SchedaDocumento riproponiDatiRisp(Page page, SchedaDocumento schedaDoc)
        {
            try
            {
                //Data una scheda documento (quella del protocollo in ingresso) ne viene creata una nuova 
                //(per il protocollo in uscita) riproponendo l'oggetto e il mittente come destinatario del protocollo in uscita 
                SchedaDocumento schedaNewDoc = NewSchedaDocumento(page);
                schedaNewDoc.oggetto = schedaDoc.oggetto;
                schedaNewDoc.registro = schedaDoc.registro;
                //schedaNewDoc.tipoProto = schedaDoc.tipoProto;
                schedaNewDoc.idPeople = schedaDoc.idPeople;
                schedaNewDoc.userId = schedaDoc.userId;
                schedaNewDoc.typeId = schedaDoc.typeId;
                schedaNewDoc.appId = schedaDoc.appId;
                schedaNewDoc.privato = "0";
                schedaNewDoc.tipoProto = "P";


                if (schedaDoc.tipoProto.Equals("A"))
                {
                    schedaNewDoc.protocollo = new DocsPAWA.DocsPaWR.ProtocolloUscita();
                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaNewDoc.protocollo).destinatari = new DocsPAWA.DocsPaWR.Corrispondente[1];
                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaNewDoc.protocollo).destinatari[0] = new DocsPAWA.DocsPaWR.Corrispondente();
                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaNewDoc.protocollo).destinatari[0] = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittente;
                }

                if (schedaDoc.oggetto != null)
                {
                    schedaNewDoc.oggetto = schedaDoc.oggetto;
                }

                //Viene popolato l'oggetto risposta al protocollo:
                if (schedaDoc.protocollo != null && schedaDoc.protocollo.numero != null
                    && !schedaDoc.protocollo.numero.Equals(""))
                {
                    DocsPaWR.InfoDocumento infoDoc = getInfoDocumento(schedaDoc);
                    schedaNewDoc.rispostaDocumento = infoDoc;
                }

                return schedaNewDoc;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        public static DocsPAWA.DocsPaWR.AreaLavoro getListaAreaLavoro(Page page, string tipoDoc, string chaDaProto, int nPage, out int numRec, out int nRec, string idRegistro)
        {
            DocsPaWR.AreaLavoro result = null;
            numRec = 0;
            nRec = 0;
            try
            {
                DocsPaWR.AreaLavoroTipoOggetto tipoObj = DocsPAWA.DocsPaWR.AreaLavoroTipoOggetto.DOCUMENTO;
                DocsPaWR.AreaLavoroTipoFascicolo tipoFasc = new DocsPAWA.DocsPaWR.AreaLavoroTipoFascicolo();
                DocsPaWR.AreaLavoroTipoDocumento tipoDocumento = new DocsPAWA.DocsPaWR.AreaLavoroTipoDocumento();

                Utente utente = (Utente)GetSessionValue("userData");
                Ruolo ruolo = (Ruolo)GetSessionValue("userRuolo");

                if (tipoDoc.Equals("A"))
                    tipoDocumento = DocsPAWA.DocsPaWR.AreaLavoroTipoDocumento.ARRIVO;
                else if (tipoDoc.Equals("P"))
                    tipoDocumento = DocsPAWA.DocsPaWR.AreaLavoroTipoDocumento.PARTENZA;
                else if (tipoDoc.Equals("I"))
                    tipoDocumento = DocsPAWA.DocsPaWR.AreaLavoroTipoDocumento.INTERNO;
                else if (tipoDoc.Equals("T"))
                    tipoDocumento = DocsPAWA.DocsPaWR.AreaLavoroTipoDocumento.TUTTI;
                else
                    tipoDocumento = DocsPAWA.DocsPaWR.AreaLavoroTipoDocumento.GRIGIO;

                bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                result = docsPaWS.DocumentoGetAreaLavoroPaging(utente, ruolo, enableUfficioRef, tipoObj, tipoDocumento, tipoFasc, chaDaProto, nPage, idRegistro, out numRec, out nRec);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return result;
        }


        public static DocsPAWA.DocsPaWR.DocumentoParolaChiave[] getListaParoleChiave(Page page)
        {
            try
            {
                DocsPaWR.DocumentoParolaChiave[] listaParoleChiave;
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                listaParoleChiave = docsPaWS.DocumentoGetParoleChiave(infoUtente.idAmministrazione);

                if (listaParoleChiave == null)
                {
                    throw new Exception();
                }

                return listaParoleChiave;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;

        }

        public static DocsPAWA.DocsPaWR.TipologiaAtto[] getListaTipologiaAtto(Page page)
        {
            try
            {
                DocsPaWR.TipologiaAtto[] listaTipologiaAtto;
                listaTipologiaAtto = docsPaWS.DocumentoGetTipologiaAtto();

                if (listaTipologiaAtto == null)
                {
                    throw new Exception();
                }

                return listaTipologiaAtto;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;

        }

        public static DocsPAWA.DocsPaWR.TipologiaAtto[] getTipoAttoPDInsRic(Page page, string idAmministrazione, string idGruppo, string diritti)
        {
            try
            {
                DocsPaWR.TipologiaAtto[] listaTipologiaAtto;
                listaTipologiaAtto = docsPaWS.getTipoAttoPDInsRic(idAmministrazione, idGruppo, diritti);

                if (listaTipologiaAtto == null)
                {
                    throw new Exception();
                }

                return listaTipologiaAtto;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static DocsPAWA.DocsPaWR.TipologiaAtto[] getListaTipologiaAtto(Page page, string idAmministrazione)
        {
            try
            {
                DocsPaWR.TipologiaAtto[] listaTipologiaAtto;
                listaTipologiaAtto = docsPaWS.GetTipologiaAttoProfDin(idAmministrazione);

                if (listaTipologiaAtto == null)
                {
                    throw new Exception();
                }

                return listaTipologiaAtto;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;

        }

        #region DOCUMENTI CESTINATI-RIMOSSI
        //Verifica che l'utente possa cestinare il documento
        public static string verificaDirittiCestinaDocumento(Page page, DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento)
        {
            string result = "";
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                result = docsPaWS.VerificaDirittiCestinaDocumento(infoUtente, schedaDocumento);
                if (result.Equals(""))
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
        }

        //Cestina documenti grigi e predisposti 
        public static bool CestinaDocumento(Page page, DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento, string tipoDoc, string note, out string errorMsg)
        {
            bool result = true;
            errorMsg = string.Empty;
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                result = docsPaWS.DocumentoExecCestina(infoUtente, schedaDocumento, tipoDoc, note, out errorMsg);
                //if (!result)
                //{
                //    throw new Exception();
                //}
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
        }

        //Recupera i documenti nello stato "in cestino"
        public static InfoDocumento[] getDocInCestino(Page page, DocsPAWA.DocsPaWR.InfoUtente infoUtente, DocsPaWR.FiltroRicerca[][] filtriRicerca)
        {
            InfoDocumento[] listaDoc = null;
            try
            {
                if (filtriRicerca == null)
                    listaDoc = docsPaWS.DocumentoGetDocInCestino(infoUtente);
                else
                    listaDoc = docsPaWS.DocumentoGetDocInCestinoFiltro(infoUtente, filtriRicerca);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return listaDoc;
        }

        //Riattiva un documento (dalla stato in cestino allo stato attivo)
        public static bool riattivaDocumento(Page page, DocsPAWA.DocsPaWR.InfoUtente infoUtente, DocsPAWA.DocsPaWR.InfoDocumento infoDoc)
        {
            bool result = false;
            try
            {
                result = docsPaWS.DocumentoRiattivaDoc(infoUtente, infoDoc);
                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
        }

        //Vecchio metodo di rimozione scheda documento (documento grigio non trasmesso, nonn predisposto, non in area lavoro)
        public static string rimuoviSchedadocumento(Page page, DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento)
        {
            string result = "";
            try
            {
                //inserisco i dati relativi all'utente
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                result = docsPaWS.DocumentoExecRimuoviScheda(infoUtente, schedaDocumento);
                if (result.Equals(""))
                {
                    throw new Exception();
                }

            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
        }

        //Rimuove fisicamente tutta la lista (o una lista filtrata) dei documenti in cestino
        public static bool svuotaCestino(out bool docInCestino, Page page, DocsPAWA.DocsPaWR.InfoUtente infoUtente, DocsPaWR.InfoDocumento[] ListaDoc)
        {
            bool result = false;
            docInCestino = false;
            try
            {
                result = docsPaWS.SvuotaCestino(infoUtente, ListaDoc, out docInCestino);
                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
        }

        //Rimuove fisicamente un singolo documento
        public static bool EliminaDoc(Page page, DocsPAWA.DocsPaWR.InfoUtente infoUtente, DocsPaWR.InfoDocumento infoDoc)
        {
            bool result = false;
            try
            {
                result = docsPaWS.EliminaDoc(infoUtente, infoDoc);
                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(es);
                //ErrorManager.redirect(page, es);
            }
            return result;
        }

        #endregion

        public static DocsPAWA.DocsPaWR.DocumentoStoricoOggetto[] getStoriaModifiche(Page page, DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento, string tipo)
        {
            try
            {
                DocsPaWR.DocumentoStoricoOggetto[] result = docsPaWS.DocumentoGetListaStoriciOggetto(schedaDocumento.systemId);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
                return null;
            }
        }

        //get storico campi profilati di un tipo documento
        public static DocsPAWA.DocsPaWR.StoricoProfilati[] getStoriaProfilatiAtto(Page page, DocsPAWA.DocsPaWR.Templates template, string doc_number, string idGroup)
        {
            try
            {
                DocsPaWR.StoricoProfilati[] result = docsPaWS.DocumentoGetListaStoricoProfilati(template.ID_TIPO_ATTO, doc_number,idGroup);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
                return null;
            }
        }

        public static DocsPAWA.DocsPaWR.DocumentoLogDocumento[] getStoriaLog(Page page, string idOggetto, string varOggetto)
        {
            try
            {
                DocsPAWA.DocsPaWR.DocumentoLogDocumento[] result = docsPaWS.DocumentoGetListaLog(idOggetto, varOggetto);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
                return null;
            }
        }

        #region Gestione allegati

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public static DocsPAWA.DocsPaWR.Allegato[] getAllegati(DocsPaWR.SchedaDocumento schedaDocumento, string filterTipologiaAllegato, string simplifiedInteroperabilityId="")
        {
            DocsPaWR.Allegato[] allegati = null;

            try
            {
                DocsPaWR.InfoDocumento infoDoc = getInfoDocumento(schedaDocumento);

                allegati = docsPaWS.DocumentoGetAllegati(infoDoc.docNumber, filterTipologiaAllegato, simplifiedInteroperabilityId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return allegati;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public static string AllegatoIsPEC(string version_id)
        {
            string res = "0";
            try
            {
                res = docsPaWS.AllegatoIsPEC(version_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public static string AllegatoIsIS(string version_id)
        {
            string res = "0";
            try
            {
                res = docsPaWS.AllegatoIsIS(version_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allegato"></param>
        /// <returns></returns>
        public static DocsPAWA.DocsPaWR.Allegato aggiungiAllegato(DocsPaWR.Allegato allegato)
        {
            try
            {
                return docsPaWS.DocumentoAggiungiAllegato(UserManager.getInfoUtente(), allegato);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Errore nella creazione dell'allegato");
            }
        }

        /// <summary>
        /// Rimozione dell'allegato
        /// </summary>
        /// <param name="allegato"></param>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public static bool rimuoviAllegato(DocsPAWA.DocsPaWR.Allegato allegato, DocsPaWR.SchedaDocumento schedaDocumento)
        {
            //inserisco i dati relativi all'utente
            InfoUtente infoUtente = UserManager.getInfoUtente();

            return docsPaWS.DocumentoRimuoviAllegato(allegato, infoUtente, schedaDocumento);
        }

        public static Allegato[] rimuoviDaListaAllegati(Allegato[] lista, int index)
        {
            if (lista == null || lista.Length < index)
                return lista;

            if (lista.Length == 1)
                return null;

            Allegato[] nuovaLista = null;
            if (lista != null && lista.Length > 0)
            {
                for (int i = 0; i < lista.Length; i++)
                {
                    if (i != index)
                        nuovaLista = addAllegato(nuovaLista, lista[i]);
                }
            }
            return nuovaLista;
        }

        public static void modificaAllegato(Page page, DocsPaWR.Allegato allegato, string idDocumentoPrincipale)
        {
            try
            {
                bool result = docsPaWS.DocumentoModificaAllegato(UserManager.getInfoUtente(), allegato, idDocumentoPrincipale);

                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
        }

        public static void scambiaAllegato(Page page, DocsPAWA.DocsPaWR.Documento documento, DocsPAWA.DocsPaWR.Allegato allegato)
        {
            try
            {
                bool result = docsPaWS.DocumentoScambiaAllegato(UserManager.getInfoUtente(), allegato, documento);

                if (!result)
                {

                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
        }

        public static SchedaDocumento scambiaAllegatoinLista(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento, int index)
        {
            DocsPaWR.Allegato allegato = schedaDocumento.allegati[index];
            DocsPaWR.Documento documento = schedaDocumento.documenti[0];
            //creo il nuovo doc e il nuovo allegato
            DocsPaWR.Documento newDoc = new DocsPAWA.DocsPaWR.Documento();
            DocsPaWR.Allegato newAllegato = new DocsPAWA.DocsPaWR.Allegato();

            newDoc.applicazione = documento.applicazione;
            newDoc.dataInserimento = allegato.dataInserimento;
            newDoc.descrizione = documento.descrizione;
            newDoc.docNumber = allegato.docNumber;
            newDoc.docServerLoc = documento.docServerLoc;
            newDoc.fileName = allegato.fileName;
            newDoc.fileSize = allegato.fileSize;
            newDoc.firmatari = allegato.firmatari;
            newDoc.idPeople = allegato.idPeople;
            newDoc.path = allegato.path;
            newDoc.subVersion = allegato.subVersion;
            newDoc.version = documento.version;
            newDoc.versionId = documento.versionId;
            newDoc.versionLabel = documento.versionLabel;

            //nuovo allegato
            newAllegato.applicazione = allegato.applicazione;
            newAllegato.dataInserimento = documento.dataInserimento;
            newAllegato.descrizione = allegato.descrizione;
            newAllegato.docNumber = documento.docNumber;
            newAllegato.docServerLoc = documento.docServerLoc;
            newAllegato.fileName = documento.fileName;
            newAllegato.fileSize = documento.fileSize;
            newAllegato.firmatari = documento.firmatari;
            newAllegato.idPeople = documento.idPeople;
            newAllegato.path = documento.path;
            newAllegato.subVersion = documento.subVersion;
            newAllegato.version = allegato.version;
            newAllegato.versionId = allegato.versionId;
            newAllegato.versionLabel = allegato.versionLabel;

            schedaDocumento.allegati[index] = newAllegato;
            schedaDocumento.documenti[0] = newDoc;

            //Aggiorna flag daInviare
            schedaDocumento = docsPaWS.DocumentoSetFlagDaInviare(schedaDocumento);

            if (schedaDocumento == null)
            {
                throw new Exception();
            }

            return schedaDocumento;
        }

        public static Allegato[] addAllegato(Allegato[] lista, Allegato allegato)
        {
            Allegato[] nuovaLista;
            if (lista != null)
            {
                int len = lista.Length;
                nuovaLista = new Allegato[len + 1];
                lista.CopyTo(nuovaLista, 0);
                nuovaLista[len] = allegato;
            }
            else
            {
                nuovaLista = new Allegato[1];
                nuovaLista[0] = allegato;
            }
            return nuovaLista;
        }

        public static DocsPAWA.DocsPaWR.Allegato[] getAllegatiPerRimozione(string docNumber)
        {
            DocsPaWR.Allegato[] allegati = null;

            try
            {
                
                allegati = docsPaWS.DocumentoGetAllegati(docNumber, string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return allegati;
        }




        #endregion

        #region gestione versioni

        public static FileRequest aggiungiVersione(Page page, FileRequest fileRequest, bool daInviare, bool refresh)
        {
            logger.Info("BEGIN");
            string errmsg = "Non &egrave; stato possibile creare la versione. Ripetere l'operazione.";

            try
            {
                DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);
                fileRequest = docsPaWS.DocumentoAggiungiVersione(fileRequest, infoUtente);

                if (fileRequest == null)
                {
                    throw new Exception(errmsg);
                }

                SchedaDocumento schedaDocumento = getDocumentoSelezionato(page);
                schedaDocumento.documenti = addVersione(schedaDocumento.documenti, (Documento)fileRequest);

                if (daInviare)
                {
                    schedaDocumento = docsPaWS.DocumentoSetFlagDaInviare(schedaDocumento);

                    if (schedaDocumento == null)
                    {
                        throw new Exception();
                    }
                }

                setDocumentoSelezionato(page, schedaDocumento);
                SchedaDocumento schedaDocLav = DocumentManager.getDocumentoInLavorazione(page);
                schedaDocLav.documenti = schedaDocumento.documenti;
                DocumentManager.setDocumentoInLavorazione(page, schedaDocLav);
                FileManager.setSelectedFile(page, fileRequest, refresh);
                logger.Info("END");
                return fileRequest;
            }
            catch (Exception es)
            {
                //ErrorManager.redirect(page, es);
                ErrorManager.OpenErrorPage(page, es, "Creazione versione");
            }

            return null;
        }



        public static Documento[] addVersione(Documento[] lista, Documento documento)
        {
            Documento[] nuovaLista;
            if (lista != null)
            {
                int len = lista.Length;
                nuovaLista = new Documento[len + 1];
                lista.CopyTo(nuovaLista, 1);
                nuovaLista[0] = documento;
            }
            else
            {
                nuovaLista = new Documento[1];
                nuovaLista[0] = documento;
            }
            return nuovaLista;
        }



        public static void modificaVersione(Page page, DocsPaWR.Documento documento)
        {
            try
            {
                bool result = docsPaWS.DocumentoModificaVersione(UserManager.getInfoUtente(), documento);

                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
        }



        public static bool rimuoviVersione(Page page, DocsPAWA.DocsPaWR.Documento documento, DocsPaWR.SchedaDocumento schedaDocumento)
        {
            bool result = false;

            try
            {
                String mTextFQN = String.Empty;
                // Reperimento FQN associato al documento
                try
                {
                    mTextFQN = MTextUtils.GetMTextFullQualifiedName(documento.versionId, documento.docNumber);
                }
                catch { }

                //inserisco i dati relativi all'utente
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                result = docsPaWS.DocumentoRimuoviVersione(documento, infoUtente, schedaDocumento);

                // Se tutto è andato a buon fine e la versione rimossa ha associato un modello M/Text, viene invocata la cancellazione 
                // del documento dai server M/Text
                try
                {
                    if (result && MTextUtils.IsActiveMTextIntegration() && !String.IsNullOrEmpty(mTextFQN))
                    {
                        MTextModelProvider mTextProvider = ModelProviderFactory<MTextModelProvider>.GetInstance();
                        mTextProvider.DeleteDocument(mTextFQN);
                    }
                }
                catch (Exception) { }

                if (!result)
                {
                    throw new ApplicationException("Si è verificato un errore nella rimozione della versione del documento");
                }
            }
            catch (Exception es)
            {
                result = false;

                ErrorManager.redirect(page, es);
            }

            return result;
        }

        public static Documento[] rimuoviDaListaVersioni(Documento[] lista, int index)
        {
            ArrayList newList = new ArrayList(lista);

            newList.RemoveAt(index);

            return (Documento[])newList.ToArray(typeof(Documento));
        }

        /// <summary>
        /// restituisce true se alla versione con versionId è associato un file con impressa la segnatura
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public static bool IsVersionWithSegnature(string versionId)
        {
            return docsPaWS.IsVersionWithSegnature(UserManager.getInfoUtente(), versionId);
        }

        #endregion

        #region gestione catena documentale

        public static DocsPAWA.DocsPaWR.AnelloDocumentale getCatenaDoc(string idGruppo, string idPeople, string idProfile, Page page)
        {
            try
            {
                DocsPaWR.AnelloDocumentale result = docsPaWS.DocumentoGetCatenaDoc(idGruppo, idPeople, idProfile);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            //			catch(System.Web.Services.Protocols.SoapException es) 
            //			{
            //				ErrorManager.redirect(page, es);
            //				return null;
            //			} 
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
                return null;
            }
        }

        /// <summary>
        /// Reperimento oggetto "InfoDocumento" relativamente
        /// al documento mittente al documento richiesto
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>
        /// <param name="docNumber"></param>
        /// <param name="tipoDocumento"></param>
        /// <returns></returns>
        public static DocsPaWR.InfoDocumento GetCatenaDocumentoMittente(Page page, string idGruppo, string idPeople, string docNumber, string tipoDocumento)
        {
            try
            {
                return docsPaWS.DocumentoGetCatenaDocumentoMittente(idGruppo, idPeople, docNumber, tipoDocumento);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);

                return null;
            }
        }

        #endregion

        #region Gestione report
        public static FileDocumento getSchedaDocReport(Page page, SchedaDocumento schedaDoc, InfoUtente infoUtente)
        {
            try
            {
                FileDocumento result = docsPaWS.ReportSchedaDoc(infoUtente, schedaDoc);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            //			catch(System.Web.Services.Protocols.SoapException es) 
            //			{
            //				ErrorManager.redirect(page, es);
            //			} 
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static FileDocumento getBustaReport(Page page, SchedaDocumento schedaDoc)
        {
            try
            {
                FileDocumento result = docsPaWS.ReportBusta(schedaDoc);


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
        #endregion


        public static DocsPAWA.DocsPaWR.Fascicolo[] GetFascicoliDaDoc(Page page, string idProfile)
        {
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                DocsPaWR.Fascicolo[] listaFascicoli = docsPaWS.FascicolazioneGetFascicoliDaDoc(infoUtente, idProfile);

                /*if(listaFascicoli == null)
                {
                    throw new Exception();
                }*/

                return listaFascicoli;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static string GetClassificaDoc(Page page, string idProfile)
        {
            string codClassifica = System.String.Empty;
            try
            {
                //costruzione m_dataTableFascicoli
                if (idProfile != null)
                {
                    DocsPaWR.Fascicolo[] listaFascicoli = GetFascicoliDaDoc(page, idProfile);
                    if (listaFascicoli != null)
                        if (listaFascicoli.Length > 0)
                        {
                            DocsPaWR.Fascicolo fasc = listaFascicoli[0];
                            DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(page, fasc.idClassificazione, UserManager.getUtente(page).idAmministrazione);
                            codClassifica = gerClassifica[gerClassifica.Length - 1].codice;
                        }
                }
                return codClassifica;
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
                return codClassifica;
            }
            catch (Exception)
            {
                return codClassifica;
            }
        }


        public static string getFascicoloDoc(Page page, DocsPAWA.DocsPaWR.InfoDocumento infoDocumento)
        {

            string codFascicolo = System.String.Empty;
            try
            {
                //costruzione m_dataTableFascicoli
                if (infoDocumento != null)
                {
                    DocsPaWR.Fascicolo[] listaFascicoli = GetFascicoliDaDoc(page, infoDocumento.idProfile);


                    if (listaFascicoli != null)
                        if (listaFascicoli.Length > 0)
                        {
                            DocsPaWR.Fascicolo fasc = listaFascicoli[0];
                            codFascicolo = fasc.codice;
                        }
                }
                return codFascicolo;
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
                return codFascicolo;
            }
            catch (Exception)
            {
                return codFascicolo;
            }
        }

        #region OGGETTARIO
        public static Oggetto addOggetto(Page page, Oggetto oggetto, Registro registro, ref string errMsg)
        {
            //Tolgo i caratteri speciali dal campo descrizione oggetto
            oggetto.descrizione = oggetto.descrizione.Replace(System.Environment.NewLine, "");

            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                Oggetto result = docsPaWS.DocumentoAddOggetto(infoUtente, oggetto, registro, ref errMsg);

                if (result == null)
                {
                    //throw new Exception();
                }

                return result;
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }

            return null;
        }

        public static bool cancellaOggetto(Page page, Oggetto oggetto)
        {
            //Tolgo i caratteri speciali dal campo descrizione oggetto
            oggetto.descrizione = oggetto.descrizione.Replace(System.Environment.NewLine, "");

            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                bool result = docsPaWS.UpdateOggetto(infoUtente, oggetto);
                return result;
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }

            return false;
        }

        //public static Oggetto ModificaOggetto(Page page, Oggetto oggettoOld, Oggetto oggettoNew, Registro registro,ref string errMsg)
        //{ 
        //    //Tolgo i caratteri speciali dal campo descrizione oggetto
        //    oggettoOld.descrizione = oggettoOld.descrizione.Replace(System.Environment.NewLine, "");
        //    oggettoNew.descrizione = oggettoNew.descrizione.Replace(System.Environment.NewLine, "");
        //    try
        //    {
        //       InfoUtente infoUtente = UserManager.getInfoUtente(page);
        //       Oggetto result = docsPaWS.DocumentoModOggetto(infoUtente, oggettoOld, oggettoNew, registro, ref errMsg);

        //       if (result == null)
        //       {
        //          //throw new Exception();
        //       }

        //       return result;
        //    }
        //    catch (Exception e)
        //    {
        //    //    ErrorManager.redirect(page, e);
        //    }

        //    return null;


        //}

        #endregion
        public static DocumentoParolaChiave addParolaChiave(Page page, DocumentoParolaChiave parolaC)
        {
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                DocumentoParolaChiave result = docsPaWS.DocumentoAddParolaChiave(infoUtente.idAmministrazione, parolaC);

                if (result == null)
                {
                    //throw new Exception();
                }

                return result;
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return null;
        }


        public static TipologiaAtto addTipologiaAtto(Page page, TipologiaAtto tipoAtto)
        {
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                TipologiaAtto result = docsPaWS.DocumentoAddTipologiaAtto(tipoAtto, infoUtente);

                if (result == null)
                {
                    //throw new Exception();
                }

                return result;
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return null;
        }

        public static DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum cercaDuplicati(Page page, SchedaDocumento schedaDoc, string cercaDuplicati2, out InfoProtocolloDuplicato[] datiProtDupl)
        {
            DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum res = DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum.NessunDuplicato;
            datiProtDupl = new InfoProtocolloDuplicato[0];

            try
            {
                res = docsPaWS.DocumentoCercaDuplicatiInfo(schedaDoc, cercaDuplicati2, out datiProtDupl);
            }
            catch (Exception es)
            {
                res = DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum.ErroreGenerico;
                ErrorManager.redirect(page, es);
            }

            return res;
        }

        public static bool cercaDuplicati(Page page, SchedaDocumento schedaDoc)
        {
            bool res;

            try
            {
                bool result = docsPaWS.DocumentoCercaDuplicati(schedaDoc);

                res = result;
            }
            catch (Exception es)
            {
                res = false;
                ErrorManager.redirect(page, es);
            }

            return res;
        }

        public static DocsPAWA.DocsPaWR.DocumentoParolaChiave[] removeParoleChiave(DocsPAWA.DocsPaWR.DocumentoParolaChiave[] lista, int index)
        {
            // Lista da restituire
            IEnumerable<DocumentoParolaChiave> retVal = new List<DocumentoParolaChiave>();

            if (lista != null && index < lista.Length && lista.Length > 0)
            {

                // Prelevamento delle informazioni sulla parola chiave da eliminare
                DocumentoParolaChiave wordToRemove = lista[index];

                // Rimozione della parola chiave
                retVal = lista.Where(w => w.systemId != wordToRemove.systemId);
            }

            return retVal.ToArray();


            //if (lista == null || lista.Length < index)
            //    return lista;

            ////			if(lista.Length == 1)
            ////				return null;

            //DocsPaWR.DocumentoParolaChiave[] nuovaLista = null;
            //if (lista != null && lista.Length > 0)
            //{
            //    for (int i = 0; i < lista.Length; i++)
            //    {
            //        if (i != index)
            //            nuovaLista = addParolaChiave(nuovaLista, lista[i]);
            //    }
            //}
            //return nuovaLista;
        }

        [Obsolete("Metodo vecchio e inefficiente. Dato che era utilizzato solo da removeParoleChiave, utilizzatare direttamente removeParoleChiave", true)]
        public static DocsPAWA.DocsPaWR.DocumentoParolaChiave[] addParolaChiave(DocsPAWA.DocsPaWR.DocumentoParolaChiave[] lista, DocsPAWA.DocsPaWR.DocumentoParolaChiave parolaC)
        {
            DocsPaWR.DocumentoParolaChiave[] nuovaLista;
            if (lista != null)
            {
                int len = lista.Length;
                nuovaLista = new DocsPAWA.DocsPaWR.DocumentoParolaChiave[len + 1];
                lista.CopyTo(nuovaLista, 0);
                nuovaLista[len] = parolaC;
            }
            else
            {
                nuovaLista = new DocsPAWA.DocsPaWR.DocumentoParolaChiave[1];
                nuovaLista[0] = parolaC;
            }
            return nuovaLista;
        }



        #region GESTIONE LABEL ADL

        public static void setLabelADL(System.Web.UI.WebControls.Label lbl_ADL)
        {
            string label = ConfigSettings.getKey(ConfigSettings.KeysENUM.LABEL_ADL);
            if (label != null && !label.Equals(""))
                lbl_ADL.Text = label;
            else
                lbl_ADL.Text = label = "ADL";
        }
        #endregion

        #region variabile sessione "DataGridFascicoliContenitori"
        public static DataTable getDataGridFascicoliContenitori(Page page)
        {
            return (DataTable)GetSessionValue("DataGridFascicoliContenitori");
        }

        public static void setDataGridFascicoliContenitori(Page page, DataTable dataGridFascicoliContenitori)
        {
            SetSessionValue("DataGridFascicoliContenitori", dataGridFascicoliContenitori);
        }

        public static void removeDataGridFascicoliContenitori(Page page)
        {
            RemoveSessionValue("DataGridFascicoliContenitori");
        }
        #endregion

        #region Session BlockQuickProt
        public static bool getBlockQuickProt(Page page)
        {
            return (bool)GetSessionValue("docProtocollo.BlockQuickProt");
        }

        public static void setBlockQuickProt(Page page, bool infoDoc)
        {
            SetSessionValue("docProtocollo.BlockQuickProt", infoDoc);
        }

        public static void removeBlockQuickProt(Page page)
        {
            RemoveSessionValue("docProtocollo.BlockQuickProt");
        }
        #endregion

        #region Session BlockAllegati
        public static bool getBlockAllegati(Page page)
        {
            if (GetSessionValue("gestioneAllegato.BlockAllegati") != null)
                return (bool)GetSessionValue("gestioneAllegato.BlockAllegati");
            else
                return false;
        }

        public static void setBlockAllegati(Page page, bool value)
        {
            SetSessionValue("gestioneAllegato.BlockAllegati", value);
        }

        public static void removeBlockAllegati(Page page)
        {
            RemoveSessionValue("gestioneAllegato.BlockAllegati");
        }
        #endregion

        #region variabile sessione "classifSelezionata"
        public static DocsPAWA.DocsPaWR.FascicolazioneClassificazione getClassificazioneSelezionata(Page page)
        {
            return (DocsPAWA.DocsPaWR.FascicolazioneClassificazione)GetSessionValue("classifSelezionata");
        }

        public static void setClassificazioneSelezionata(Page page, DocsPAWA.DocsPaWR.FascicolazioneClassificazione classificazione)
        {
            SetSessionValue("classifSelezionata", classificazione);
        }

        public static void removeClassificazioneSelezionata(Page page)
        {
            RemoveSessionValue("classifSelezionata");
        }
        #endregion

        #region Gestione session per la verifica firma digitale documento

        public static void SetSignedDocument(DocsPAWA.DocsPaWR.FileDocumento signedDocument)
        {
            SetSessionValue("SignedDocument", signedDocument);
        }

        public static DocsPAWA.DocsPaWR.FileDocumento GetSignedDocument()
        {
            return (DocsPAWA.DocsPaWR.FileDocumento)GetSessionValue("SignedDocument");
        }

        public static void RemoveSignedDocument()
        {
            RemoveSessionValue("SignedDocument");
        }

        #endregion

        #region
        //numPag: numero della pagina dalla quale si è partiti nel datagrid
        public static string getMemoriaNumPag(Page page)
        {
            return (string)GetSessionValue("MemoriaNumPag");
        }

        public static void setMemoriaNumPag(Page page, string numPag)
        {
            SetSessionValue("MemoriaNumPag", numPag);
        }

        public static void removeMemoriaNumPag(Page page)
        {
            RemoveSessionValue("MemoriaNumPag");
        }

        //Tab: tab di ricerca di riferimento 
        public static string getMemoriaTab(Page page)
        {
            return (string)GetSessionValue("MemoriaTab");
        }

        public static void setMemoriaTab(Page page, string nomeTab)
        {
            SetSessionValue("MemoriaTab", nomeTab);
        }

        public static void removeMemoriaTab(Page page)
        {
            RemoveSessionValue("MemoriaTab");
        }

        //FiltriRicDoc: tab di ricerca di riferimento 
        public static DocsPAWA.DocsPaWR.FiltroRicerca[][] getMemoriaFiltriRicDoc(Page page)
        {
            return (DocsPAWA.DocsPaWR.FiltroRicerca[][])GetSessionValue("MemoriaFiltriRicDoc");
        }

        public static void setMemoriaFiltriRicDoc(Page page, DocsPAWA.DocsPaWR.FiltroRicerca[][] listaFiltriDoc)
        {
            SetSessionValue("MemoriaFiltriRicDoc", listaFiltriDoc);

            /* Rimuovi le informazioni relative al tasto back per le ricerche sui fascicoli. Questo 
             * e' necessario per esser certi che sia visualizzato sempre il tasto back relativo all'ultima
             * ricerca effettuata.
             */
            FascicoliManager.SetFolderViewTracing(page, false);
        }

        public static void removeMemoriaFiltriRicDoc(Page page)
        {
            RemoveSessionValue("MemoriaFiltriRicDoc");
        }

        public static void setMemoriaVisualizzaBack(Page page)
        {
            setMemoriaVisualizzaBack();
        }

        public static string getMemoriaVisualizzaBack(Page page)
        {
            return getMemoriaVisualizzaBack();
        }

        public static void RemoveMemoriaVisualizzaBack(Page page)
        {
            RemoveMemoriaVisualizzaBack();
        }

        public static void setMemoriaVisualizzaBack()
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["MemoriaVisualizzaBack"] = "true";
        }

        public static string getMemoriaVisualizzaBack()
        {
            return (string)HttpContext.Current.Session["MemoriaVisualizzaBack"];
        }

        public static void RemoveMemoriaVisualizzaBack()
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session.Remove("MemoriaVisualizzaBack");
        }

        #endregion


        /// <summary>
        /// Impostazione valore in sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="sessionValue"></param>
        private static void SetSessionValue(string sessionKey, object sessionValue)
        {
            System.Web.HttpContext.Current.Session[sessionKey] = sessionValue;
        }

        /// <summary>
        /// Reperimento valore da sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        private static Object GetSessionValue(string sessionKey)
        {
            return System.Web.HttpContext.Current.Session[sessionKey];
        }

        /// <summary>
        /// Rimozione chiave di sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        private static void RemoveSessionValue(string sessionKey)
        {
            System.Web.HttpContext.Current.Session.Remove(sessionKey);
        }

        #region Modifica Destinatri
        public static DocsPAWA.DocsPaWR.SchedaDocumento DO_AddDestinatrioModificato(DocsPAWA.DocsPaWR.SchedaDocumento scheda, string systemId)
        {
            return docsPaWS.DO_AddDestinatrioModificato(scheda, systemId);
        }

        public static DocsPAWA.DocsPaWR.SchedaDocumento DO_AddDestinatrioCCModificato(DocsPAWA.DocsPaWR.SchedaDocumento scheda, string systemId)
        {
            return docsPaWS.DO_AddDestinatrioCCModificato(scheda, systemId);
        }

        public static DocsPAWA.DocsPaWR.SchedaDocumento DO_ClearDestinatariModificati(DocsPAWA.DocsPaWR.SchedaDocumento scheda)
        {
            return docsPaWS.DO_ClearDestinatariModificati(scheda);
        }

        public static DocsPAWA.DocsPaWR.SchedaDocumento DO_ClearDestinatariCCModificati(DocsPAWA.DocsPaWR.SchedaDocumento scheda)
        {
            return docsPaWS.DO_ClearDestinatariCCModificati(scheda);
        }

        public static DocsPAWA.DocsPaWR.SchedaDocumento DO_RemoveDestinatarioModificati(DocsPAWA.DocsPaWR.SchedaDocumento scheda, string systemId)
        {
            return docsPaWS.DO_RemoveDestinatrioModificato(scheda, systemId);
        }

        public static DocsPAWA.DocsPaWR.SchedaDocumento DO_RemoveDestinatarioCCModificati(DocsPAWA.DocsPaWR.SchedaDocumento scheda, string systemId)
        {
            return docsPaWS.DO_RemoveDestinatrioCCModificato(scheda, systemId);
        }

        public static bool DO_UpdateVisibilita(Page page, DocsPaWR.SchedaDocumento scheda, DocsPAWA.DocsPaWR.Ruolo ruolo)
        {
            string serverName = Utils.getHttpFullPath(page);
            if (scheda.destinatariModificati.Length != 0 || scheda.destinatariCCModificati.Length != 0)
            {
                return docsPaWS.DO_TrasmettiDestinatariModificati(scheda, ruolo, serverName);
            }
            else
            {
                return true;
            }
        }

        //public static DocsPAWA.DocsPaWR.DocumentoStoricoMittente[] getStoriaModifiche(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento, string tipo)
        //{
        //    return docsPaWS.DO_GetListaStoriciMittente(schedaDocumento.systemId, tipo);
        //}

        public static DocsPAWA.DocsPaWR.DocumentoStoricoMittente[] getStoriaModifiche(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento, string tipo)
        {
            return docsPaWS.DO_GetListaStoriciMittente(schedaDocumento.systemId, tipo);
        }
        #endregion

        #region notifiche
        //areaLavoro - datagrid
        public static ArrayList getDataGridNotifiche(Page page)
        {
            return (ArrayList)GetSessionValue("Notifiche.dataGridNotifiche");
        }

        public static void setDataGridNotifiche(Page page, ArrayList arrayDataGrid)
        {
            SetSessionValue("Notifiche.dataGridNotifiche", arrayDataGrid);
        }

        public static void removeDataGridNotifiche(Page page)
        {
            RemoveSessionValue("Notifiche.dataGridNotifiche");
        }
        #endregion

        public static StatoInvio[] getListaSpedizioni(Page page, string idProfile)
        {
            StatoInvio[] listaSpedizioni = null;
            try
            {
                listaSpedizioni = docsPaWS.GetListaSpedizioni(idProfile);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return listaSpedizioni;
        }

        public static bool isDocInConversionePdf(DocsPAWA.DocsPaWR.InfoUtente infoUtente, string idProfile)
        {
            try
            {
                return docsPaWS.isDocInConversionePdf(infoUtente, idProfile);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(null, es);
                return false;
            }
        }

        public static void delDocRichiestaConversionePdf(DocsPAWA.DocsPaWR.InfoUtente infoUtente, string idProfile)
        {
            try
            {
                docsPaWS.delDocRichiestaConversionePdf(infoUtente, idProfile);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(null, es);
            }
        }

        public static string addAreaConservazione(Page page, string idProfile, string idProject, string docNumber, InfoUtente utente, string tipoOggetto)
        {
            string result = string.Empty;
            try
            {
                result = docsPaWS.DocumentoExecAddConservazione(idProfile, idProject, docNumber, utente, tipoOggetto);
                if (result == "-1")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
            return result;
        }

        public static string addAreaConservazioneAM(Page page, string idProfile, string idProject, string docNumber, InfoUtente utente, string tipoOggetto)
        {
            string result = string.Empty;
            try
            {
                result = docsPaWS.DocumentoExecAddConservazioneAM(idProfile, idProject, docNumber, utente, tipoOggetto);
                if (result == "-1")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
            return result;
        }

        public static bool updateStatoAreaCons(Page page, string sysId, string tipo_cons, string note, string descr, string idTipoSupp, InfoUtente infoUtente, bool consolida)
        {
            bool result = true;
            try
            {
                result = docsPaWS.DocumentoUpdateAreaCons(sysId, tipo_cons, note, descr, idTipoSupp, infoUtente, consolida);
                if (!result)
                    throw new Exception();
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
        }
        public static void eliminaDaAreaConservazione(Page page, string idProfile, Fascicolo fasc, string idIstanza, bool deleteIstanza, string systemId)
        {
            try
            {
                bool result = docsPaWS.DocumentoCancellaAreaConservazione(idProfile, fasc, idIstanza, deleteIstanza, systemId);
                if (!result)
                    throw new Exception();
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
        }

        public static int canDeleteAreaConservazione(Page page, string idProfile, string idPeople, string idGruppo)
        {
            int result = 0;
            try
            {
                result = docsPaWS.CanDeleteFromItemCons(idProfile, idPeople, idGruppo);
                if (result == -1)
                    throw new Exception();
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
        }
        public static int isPrimaConservazione(Page page, string idPeople, string idGruppo)
        {
            int result = 0;
            try
            {
                result = docsPaWS.DocumentoIsPrimaIstanzaCons(idPeople, idGruppo);
                if (result == -1)
                    throw new Exception();
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
        }
        public static string getPrimaIstanzaAreaCons(Page page, string idPeople, string idGruppo)
        {
            string result = string.Empty;
            try
            {
                result = docsPaWS.getPrimaIstanzaAreaCons(idPeople, idGruppo);
                if (result == "Errore")
                    throw new Exception();
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
 
        }
        #region areaConservazione

        /// <summary>
        /// Validazione istanza di conservazione
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static AreaConservazioneValidationResult validateIstanzaConservazione(string idConservazione, Page page)
        {
            AreaConservazioneValidationResult result = null;

            try
            {
                result = docsPaWS.ValidateIstanzaConservazione(idConservazione);
            }
            catch (Exception ex)
            {
                Exception originalEx = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(ex);

                ErrorManager.redirect(page, originalEx);
            }

            return result;
        }

        public static InfoConservazione[] getListaConservazione(string idPeolple, string idRuoloInUo, Page page)
        {
            InfoConservazione[] cons = null;
            try
            {
                cons = docsPaWS.ConservazioneGetInfoById(idPeolple, idRuoloInUo);
                if (cons == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return cons;
        }

        public static ItemsConservazione[] getItemsConservazione(string idIstanza, Page page)
        {
            ItemsConservazione[] itemsCons = null;
            try
            {
                itemsCons = docsPaWS.ConservazioneGetItemsById(idIstanza);
                if (itemsCons == null)
                    throw new Exception();
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return itemsCons;
        }

        public static TipoSupporto[] getListaTipiSupporti(Page page)
        {
            TipoSupporto[] tipoSupp = null;
            try
            {
                tipoSupp = docsPaWS.getListaTipoSupporto();
                if (tipoSupp == null)
                    throw new Exception();
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return tipoSupp;
        }

        public static int getItemSize(Page page, DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc, string idItem)
        {
            int size = 0;
            try
            {
                size = docsPaWS.SerializeScheda(schedaDoc, idItem);
                if (size == -1)
                    throw new Exception();
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return size;
        }

        public static void insertSizeInItemCons(Page page, string sysId, int size)
        {
            try
            {
                bool result = docsPaWS.UpdateSizeItemCons(sysId, size);
                if (!result)
                    throw new Exception();
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
        }

        public static bool insertAllInCons(Page page, DocsPAWA.DocsPaWR.FiltroRicerca[][] objFiltri, DocsPAWA.DocsPaWR.InfoUtente Safe)
        {
            bool res = true;
            int result = 1;
            try
            {
                result = docsPaWS.DocumentoInsertAllInConservazione(objFiltri, Safe, null);
                //if (!result)
                //    throw new Exception();
                if (result == -1)
                {
                    res = false;
                    page.Response.Write("<SCRIPT>alert(\"Nessun documento in elenco ha un file associato, impossibile inserirli in area conservazione\");</SCRIPT>");
                }
                else if (result == 0)
                {
                    res = true;
                    page.Response.Write("<SCRIPT>alert(\"Alcuni dei documenti presenti in elenco non hanno un file associato, impossibile inserirli in area conservazione\");</SCRIPT>");
                }
                else if (result == -2)
                {
                    res = false;
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return res;
        }

        public static void updateItemsConservazione(Page page, string tipoFile, string numAllegati, string sysId)
        {
            try
            {
                bool result = docsPaWS.updateItemsCons(tipoFile, numAllegati, sysId);
                if (!result)
                    throw new Exception();
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
        }

        public static DettItemsConservazione[] getStoriaConsDoc(Page page, string idProfile)
        {
            DettItemsConservazione[] dettItemsCons = null;
            try
            {
                dettItemsCons = docsPaWS.gettDettaglioItemsCons(idProfile);
                if (dettItemsCons == null)
                    throw new Exception();
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return dettItemsCons;
        }

        public static DettItemsConservazione[] getStoriaConsFasc(Page page, string idProject)
        {
            DettItemsConservazione[] dettItemsCons = null;
            try
            {
                dettItemsCons = docsPaWS.gettDettaglioConsFasc(idProject);
                if (dettItemsCons == null)
                    throw new Exception();
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return dettItemsCons;
        }

        public static InfoConservazione[] getListaConservazioneByFiltro(string filtro, Page page)
        {
            InfoConservazione[] cons = null;
            try
            {
                cons = docsPaWS.ConservazioneGetInfoByFiltro(filtro);
                if (cons == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return cons;
        }

        public static bool DeleteFromItemsConservazione(Page page, string idIstanza, string systemId, string idProject)
        {
            bool result = false;
            try
            {
                result = docsPaWS.ConservazioneDeleteFromItemsCons(idIstanza, systemId, idProject);
                if (!result)
                    throw new Exception();
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return result;
        }
        public static SchedaDocumento getDettaglioDocumentoPerRiabilitazioneIstanza(Page page, string idProfile, string docNumber, out string errorMessage)
        {
            DocsPaWR.SchedaDocumento sd = new DocsPAWA.DocsPaWR.SchedaDocumento();
            errorMessage = string.Empty;
            try
            {
                if (idProfile == null && docNumber == null)
                {
                    sd = null;
                }
                else
                {
                    InfoUtente infoUtente = UserManager.getInfoUtente(page);
                    sd = docsPaWS.DocumentoGetDettaglioDocumento(infoUtente, idProfile, docNumber);
                    if ((sd == null) || (sd.inCestino != null && sd.inCestino == "1"))
                    {
                        //errorMessage = string.Empty;

                        if (sd == null)
                        {
                            //verificata ACL, ritorna semmai un msg in out errorMessage
                            int rtn = verificaACL("D", idProfile, infoUtente, out errorMessage);
                            if (rtn == -1)
                                errorMessage = "Attenzione, non è stato possibile recuperare i dati del documento numero:" + idProfile + ".\\nConsultare il log per maggiori dettagli.";
                        }
                        else // è in cestino
                        {
                            errorMessage = "Il documento numero:" + idProfile + " è stato rimosso.\\nNon è più possibile visualizzarlo.";
                        }

                        if (errorMessage.Equals(""))
                            errorMessage = "Attenzione, non è stato possibile recuperare i dati del documento numero:" + idProfile + ".\\nConsultare il log per maggiori dettagli.";
                    }
                }
            }
            catch (Exception es)
            {
                return null;
            }

            return sd;
        }
        #endregion

        public static String[] GetTipoProtoRisposta(string idProfile)
        {
            String[] tipoProto = null;
            try
            {
                tipoProto = docsPaWS.DocumentoGetTipoProtoRisposta(idProfile);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(null, es);

            }
            return tipoProto;
        }

        #region trasferimento in deposito
        public static DocsPAWA.DocsPaWR.Templates[] getTipoAttoTrasfDeposito(Page page, string idAmministrazione, bool seRepertorio)
        {
            try
            {
                DocsPaWR.Templates[] listaTemplates;
                InfoUtente infoUtente = UserManager.getInfoUtente(page);

                listaTemplates = docsPaWS.getTemplatesArchivioDeposito(infoUtente, idAmministrazione, seRepertorio);
                if (listaTemplates == null)
                {
                    throw new Exception();
                }

                return listaTemplates;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        //Restituisce una lista di documenti appartenenti ad una data serie
        public static InfoDocumento[] getDocInSerie(string idGruppo, string idPeople, Page page, DocsPAWA.DocsPaWR.Templates template, int numPage, out int numTotPage, out int nRec, string valOggetto, string anno, string rfAOO)
        //public static InfoDocumento[] getDocInSerie(string idGruppo, string idPeople, Page page, DocsPAWA.DocsPaWR.Templates template, out int nRec)
        {
            nRec = 0;
            numTotPage = 0;
            InfoDocumento[] DocS = null;
            try
            {

                DocS = docsPaWS.DocumentoGetDocInSerie(idGruppo, idPeople, template, numPage, valOggetto, anno, rfAOO, out numTotPage, out nRec);
                //DocS = docsPaWS.DocumentoGetDocInSerie(idGruppo, idPeople, template, out nRec);
                if (DocS == null)
                {
                    throw new Exception();
                }
                return DocS;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }


        public static int trasfInDepositoSerie(InfoUtente utente, Page page, DocsPAWA.DocsPaWR.Templates template, string anno, string valOggetto, string tipoOp, string rfAOO)
        {
            int result = 0;
            try
            {
                result = docsPaWS.ExecTrasfInDepositoSerie(utente, template, anno, valOggetto, tipoOp, rfAOO);
                if (result == -1)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
            return result;
        }

        //Trasferimento di un singolo documento in deposito
        public static string trasfInDepositoDocumento(Page page, string idProfile, InfoUtente utente, bool serieOrFasc, string tipoOp)
        {
            string result = string.Empty;
            string sOrF = "";
            try
            {
                if (serieOrFasc)
                    sOrF = "1";
                else
                    sOrF = "0";
                result = docsPaWS.ExecTrasfInDepositoDocumento(idProfile, utente, sOrF, tipoOp);
                if (result == "-1")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
            return result;
        }


        #endregion

        #region ricercaPerFileAqcuisito

        public static ArrayList getExtFileAcquisiti(Page page)
        {
            ArrayList result = null;
            try
            {
                result = new ArrayList(docsPaWS.getListaExtFileAcquisiti(UserManager.getUtente().idAmministrazione));
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return result;
        }

        #endregion

        public static String GetProtocolloTitolario(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento)
        {
            string protTit = docsPaWS.isEnableProtocolloTitolario();
            string protocolloTitolario = string.Empty;
            //Protocollo Titolario presente nella scheda documento
            if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.protocolloTitolario) && !string.IsNullOrEmpty(protTit))
            {
                protocolloTitolario = schedaDocumento.protocolloTitolario;
            }
            else
            {
                //Protocollo titolario non presente nella scheda documento
                if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.systemId) && !string.IsNullOrEmpty(protTit))
                    protocolloTitolario = docsPaWS.getProtocolloTitolario(schedaDocumento);
            }

            return protocolloTitolario;
        }

        public static string getFirstDayOfWeek()
        {
            return docsPaWS.getFirstDayOfWeek();
        }

        public static string getLastDayOfWeek()
        {
            return docsPaWS.getLastDayOfWeek();
        }

        public static string getFirstDayOfMonth()
        {
            return docsPaWS.getFirstDayOfMonth();
        }

        public static string getLastDayOfMonth()
        {
            return docsPaWS.getLastDayOfMonth();
        }

        public static string toDay()
        {
            return docsPaWS.toDay();
        }

        /// <summary>
        /// Funzione per il reperimento delle informazioni di base relative ad un documento
        /// ed ai suoi allegati
        /// </summary>
        /// <param name="idProfile">L'id del documento</param>
        /// <param name="docNumber">Il numero del documento</param>
        /// <param name="versionNumber">Il numero di versione da caricare</param>
        /// <param name="isAttachement">True se si devono reperire informazioni su di un allegato</param>
        /// <returns>Un oggetto contente le informazioni di base sul documento e suoi allegati</returns>
        public static DocsPaWR.BaseInfoDoc[] GetBaseInfoForDocument(
            string idProfile,
            string docNumber,
            string versionNumber)
        {
            // Il risultato da restituire
            DocsPaWR.BaseInfoDoc[] schedaBase = null;

            try
            {
                // Richiesta, al web service, delle informazioni sul documento
                // e sui suoi eventuali allegati
                schedaBase = docsPaWS.GetBaseInfoForDocument(idProfile, docNumber, versionNumber);
            }
            catch (Exception e)
            {
                // Errore nel remperimento delle informazioni sul documento
                // Viene rilanciata al chiamante l'eccezione
                throw e;
            }

            // Restituzione della scheda del documento
            return schedaBase;

        }

        //public static string[] GetElencoNote(Page page, string idRegistroRf)
        //{
        //    string[] lista = null;

        //    try
        //    {
        //        InfoUtente infoUtente = UserManager.getInfoUtente(page);
        //        lista = docsPaWS.GetElencoNote(infoUtente, idRegistroRf);
        //    }
        //    catch (Exception e)
        //    {
        //        // Errore nel remperimento delle informazioni sul documento
        //        // Viene rilanciata al chiamante l'eccezione
        //        throw e;
        //    }

        //    // Restituzione della scheda del documento
        //    return lista;
        //}

        public static int isDocInADL(string idProfile, Page page)
        {
            try
            {
                int retValue = 0;
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                retValue = docsPaWS.isDocInADL(idProfile, infoUtente.idPeople);
                return retValue;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
                return -1;
            }
        }

        public static DocsPAWA.DocsPaWR.InfoDocumento GetInfoDocumento(string idProfile, string docNumber, Page page)
        {
            try
            {
                DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);
                return docsPaWS.GetInfoDocumento(infoUtente, idProfile, docNumber);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
                return null;
            }
        }

        /// <summary>
        /// Rimuove le sessioni da nuovo protocollo
        /// </summary>
        /// <returns></returns>
        public static void RemoveMemoriaSalvaDoc(Page page)
        {
            RemoveSessionValue("abilitaModificaSpedizione");
            RemoveSessionValue("isDocModificato");
            RemoveSessionValue("listaFascFascRapida");
            RemoveSessionValue("Modello");
            RemoveSessionValue("SessionDescCampo");
            RemoveSessionValue("template");
            RemoveSessionValue("docRiproposto");
            RemoveSessionValue("scDocCopiato");
            RemoveSessionValue("dettaglioNota");
            RemoveSessionValue("modelloTrasmCodBisSegn");
            RemoveSessionValue("isDocProtocollato");
            RemoveSessionValue("templateRiproposto");
            RemoveSessionValue("tipoAttoRipropostoTesto");
            RemoveSessionValue("tipoAttoRipropostoId");
            RemoveSessionValue("FascicoloRipr");
            RemoveSessionValue(" scDocCopiato");
            RemoveSessionValue(" modificaDescrDest");
            RemoveSessionValue("catenaDoc");

        }

        /// <summary>
        /// Se true, risulta abiltiata la gestione dei mittenti multipli
        /// </summary>
        /// <returns></returns>
        public static bool isEnableMittentiMultipli()
        {
            return docsPaWS.isEnableMittentiMultipli();
        }

        public static DocsPAWA.DocsPaWR.DocumentoStoricoDataArrivo[] getStoriaModifiche(string docnumber)
        {

            return docsPaWS.DocumentoGetListaStoricoData(docnumber);
        }


        /// <summary>
        /// Reperimento del tipo documento obbligatorio per l'Amministrazione attuale.
        /// </summary>
        /// <param name="IdAmministrazione"></param>
        /// <returns>restituisce tipo documento obbligatorio con il parametro di input IdAmministrazione</returns>
        public static string GetTipoDocObbl(string IdAmministrazione)
        {
            string tipoObbl = docsPaWS.getTipoDocObbl(IdAmministrazione);

            return tipoObbl;
        }

        public static DocsPAWA.DocsPaWR.Fascicolo[] GetFascicoliDaDocNoSecurity(Page page, string idProfile)
        {
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                DocsPaWR.Fascicolo[] listaFascicoli = docsPaWS.FascicolazioneGetFascicoliDaDocNoSecurity(infoUtente, idProfile);

                return listaFascicoli;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        #region Sessioni fascicoli non visibili
        public static void setDataFascicoliNonVisibili(Page page, List<string> DataFascicoliNonVisibili)
        {
            SetSessionValue("DataFascicoliNonVisibili", DataFascicoliNonVisibili);
        }

        public static void removeDataFascicoliNonVisibili(Page page)
        {
            RemoveSessionValue("DataFascicoliNonVisibili");
        }

        public static List<string> getDataFascicoliNonVisibili(Page page)
        {
            return (List<string>)GetSessionValue("DataFascicoliNonVisibili");
        }
        #endregion

        #region Timestamp

        /// <summary>
        /// Effettua una richiesta di marca temporale e verifica la validità della marca ottenuta prima di restituirla come output.
        /// </summary>
        /// <param name="richiesta"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static void executeAndSaveTSR(DocsPAWA.DocsPaWR.InfoUtente infoUtente, DocsPAWA.DocsPaWR.InputMarca richiesta, DocsPAWA.DocsPaWR.FileRequest fileRequest, out string message)
        {
            OutputResponseMarca outputResponseMarca = new OutputResponseMarca();
            docsPaWS.Timeout = System.Threading.Timeout.Infinite;
            outputResponseMarca = docsPaWS.executeAndSaveTSR(infoUtente, richiesta, fileRequest);

            message = "OK";
            if (outputResponseMarca == null)
                message = "Errore nel reperimento del timestamp. Servizio non disponibile.";
            if (outputResponseMarca != null && string.IsNullOrEmpty(outputResponseMarca.esito))
                message = "Errore nel reperimento del timestamp. Servizio non disponibile";
            if (outputResponseMarca != null && outputResponseMarca.esito == "KO")
                message = outputResponseMarca.descrizioneErrore;
        }

        /// <summary>
        /// Restituisce tutti i timestamp relativi alla specifica versione del documento
        /// </summary>
        /// <param name="richiesta"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static ArrayList getTimestampsDoc(DocsPAWA.DocsPaWR.InfoUtente infoUtente, DocsPAWA.DocsPaWR.FileRequest fileRequest)
        {
            docsPaWS.Timeout = System.Threading.Timeout.Infinite;
            return new ArrayList(docsPaWS.getTimestampsDoc(infoUtente, fileRequest));
        }


        /// <summary>
        /// Crea un TSD da un TSR e crea una nuova versione
        /// </summary>
        /// <param name="richiesta"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static FileRequest CreateTSDVersion(DocsPAWA.DocsPaWR.InfoUtente infoUtente, DocsPAWA.DocsPaWR.FileRequest fileRequest)
        {
            docsPaWS.Timeout = System.Threading.Timeout.Infinite;
            return docsPaWS.CreateTSDVersion(infoUtente, fileRequest);
        }


        /// <summary>
        /// Effettua una richiesta di marca temporale e verifica la validità della marca ottenuta prima di restituirla come output.
        /// Questa funzione, rispetto a quella di Paolo restituisce anche informazioni sul successo o l'insuccesso 
        /// della marcatura
        /// </summary>
        /// <param name="infoUtente">Informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="richiesta">Oggetto con le informazioni sulla marca temporale da richiedere</param>
        /// <param name="fileRequest">Informazioni sul file a cui applicare la marca</param>
        /// <param name="message">Esito dell'operazione</param>
        /// <returns>True se l'operazione è andata a buon fine. Indipendentemente dall'esito, il parametro message viene valorizzato con informazioni utili.</returns>
        public static bool ApplyTimeStamp(
            InfoUtente infoUtente,
            InputMarca richiesta,
            FileRequest fileRequest,
            out string message)
        {
            // Il risultato della funzione di applicazione della marca temporale
            OutputResponseMarca resultMarca;

            // Timeout ad infinito
            docsPaWS.Timeout = System.Threading.Timeout.Infinite;

            // Invocazione web service e recupero del risultato
            resultMarca = docsPaWS.executeAndSaveTSR(
                infoUtente,
                richiesta,
                fileRequest);

            // Impostazione del messaggio
            if (resultMarca == null || resultMarca.esito == "KO")
                message = "Errore durante il reperimento della marca temporale. Dettaglio: " + resultMarca.descrizioneErrore.ToString();
            else
                message = "Marcatura effettuata correttamente.";

            return resultMarca != null &&
                resultMarca.esito == "OK";

        }

        /// <summary>
        /// Effettua una richiesta di marca temporale e verifica la validità della marca ottenuta prima di restituirla come output.
        /// Questa funzione, rispetto a quella di Paolo restituisce anche informazioni sul successo o l'insuccesso 
        /// della marcatura
        /// </summary>
        /// <param name="infoUtente">Informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="richiesta">Oggetto con le informazioni sulla marca temporale da richiedere</param>
        /// <param name="fileRequest">Informazioni sul file a cui applicare la marca</param>
        /// <param name="message">Esito dell'operazione</param>
        /// <returns>True se l'operazione è andata a buon fine. Indipendentemente dall'esito, il parametro message viene valorizzato con informazioni utili.</returns>
        public static bool ApplyTimeStampAM(
            InfoUtente infoUtente,
            InputMarca richiesta,
            FileRequest fileRequest,
            out string message)
        {
            // Il risultato della funzione di applicazione della marca temporale
            OutputResponseMarca resultMarca;

            // Timeout ad infinito
            docsPaWS.Timeout = System.Threading.Timeout.Infinite;

            // Invocazione web service e recupero del risultato
            resultMarca = docsPaWS.executeAndSaveTSR_AM(
                infoUtente,
                richiesta,
                fileRequest);

            // Impostazione del messaggio
            if (resultMarca == null || resultMarca.esito == "KO")
                message = "Errore durante il reperimento della marca temporale. Dettaglio: " + resultMarca.descrizioneErrore.ToString();
            else
                message = "Marcatura effettuata correttamente.";

            return resultMarca != null &&
                resultMarca.esito == "OK";

        }

        #endregion

        /// <summary>
        /// Funzione per verificare se un documento è fascicolato in un fascicolo / sottofascicolo
        /// </summary>
        /// <param name="userInfo">Informazioni sull'utente richiedente</param>
        /// <param name="idProfile">Id del documento da verificare</param>
        /// <param name="project">Id del fascicolo da verificare. Valorizzare folderSelezionato per effettuare la verifica di fascicolazione in un sottofascicolo</param>
        /// <returns>True se il documento è già fascicolato nel fascicolo / sottofascicolo</returns>
        public static bool IsDocumentInFolderOrSubFolder(Page page, InfoUtente userInfo, String idProfile, Fascicolo project)
        {
            // Il valore da restituire
            bool toReturn = false;

            // Reperimento dell'array dei fascicoli in cui è fascicolato il documento
            //Fascicolo[] projects = GetFascicoliDaDocNoSecurity(page, idProfile);

            // Se è stato restituito l'array, viene ricercato un fascicolo con system id pari
            // all'idProject passato per parametro
            //toReturn = projects != null && projects.Where(e => e.systemID == project.systemID).FirstOrDefault() != null;

            try
            {
                toReturn = docsPaWS.IsDocumentInFolderOrSubFolder(userInfo, idProfile, project);
            }
            catch (Exception e)
            {
                toReturn = true;
            }

            // Restituzione del risultato
            return toReturn;
        }

        /// <summary>
        /// Funzione per la creazione di una scheda documento di inoltro
        /// </summary>
        /// <param name="idProfiles">Lista degli id dei documenti con cui creare gli allegati del documento</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha richiesto la creazione dell'utente</param>
        /// <param name="userRole">Il ruolo con cui creare il documento</param>
        /// <returns>La scheda pronta</returns>
        public static SchedaDocumento GetSchedaDocumentoInoltroMassivo(
            List<String> idProfiles,
            InfoUtente userInfo,
            Ruolo userRole,
            out String error)
        {
            // Il valore da restituire
            SchedaDocumento toReturn = null;

            toReturn = docsPaWS.GetSchedaDocumentoInoltroMassivo(
                idProfiles.ToArray(),
                userInfo,
                userRole,
                out error);

            return toReturn;

        }


        public static string GetFascicolazionePrimaria(Page page, string idProfile)
        {
            InfoUtente infoUtente = UserManager.getInfoUtente(page);
            string descrizioneFasc = string.Empty;
            try
            {
                descrizioneFasc = docsPaWS.GetFascicolazionePrimaria(infoUtente, idProfile);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            return descrizioneFasc;
        }

        public static bool cambiaFascPrimaria(Page page, string idProject, string idProfile)
        {
            InfoUtente infoUtente = UserManager.getInfoUtente(page);
            bool result = false;
            try
            {
                result = docsPaWS.CambiaFascicolazionePrimaria(infoUtente, idProject, idProfile);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
        }

        /// <summary>
        /// Funzione per il reperimento delle etichette custom definite per l'amministrazione
        /// </summary>
        /// <param name="page">La pagina chiamante</param>
        /// <returns>Lista delle etichette.</returns>
        public static EtichettaInfo[] GetLettereProtocolli(Page page)
        {
            // Lista delle etichette custom
            EtichettaInfo[] toReturn;

            // Informazioni sull'utente loggato
            InfoUtente userInfo;

            // Reperimento delle informazioni sull'utente loggato
            userInfo = UserManager.getInfoUtente(page);

            // Reperimento delle etichette
            toReturn = docsPaWS.getEtichetteDocumenti(userInfo, userInfo.idAmministrazione);

            List<EtichettaInfo> temp = new List<EtichettaInfo>(toReturn);
            temp.Add(new EtichettaInfo() { Etichetta = "R", Codice = "R" });
            temp.Add(new EtichettaInfo() { Etichetta = "C", Codice = "C" });
            // Restituzione delle label trovate
            return temp.ToArray();

        }

        /// <summary>
        /// Funzione per il calcolo dell'esito della pubblicazione.
        /// Funzione spostata dal tabRisultatiRicDoc
        /// </summary>
        /// <param name="idProfile">id profile del documento di cui mostrare l'esito</param>
        /// <param name="visualizzaColonna">Boh</param>
        /// <returns>L'esito della pubblicazione</returns>
        public static string esitoPubblicazione(string idProfile, bool visualizzaColonna)
        {
            string esito = string.Empty;
            if (visualizzaColonna)
            {
                DocsPaWR.PubblicazioneDocumenti pubblicazione = docsPaWS.getPubblicazioneDocumentiByIdProfile(idProfile);
                if (pubblicazione != null)
                {
                    if (!string.IsNullOrEmpty(pubblicazione.SYSTEM_ID))
                    {
                        if (pubblicazione.ESITO_PUBBLICAZIONE.Equals("1") &&
                            string.IsNullOrEmpty(pubblicazione.ERRORE_PUBBLICAZIONE))
                            esito = "Il documento è stato pubblicato";
                        else
                            if ((pubblicazione.ESITO_PUBBLICAZIONE.Equals("1") &&
                            !string.IsNullOrEmpty(pubblicazione.ERRORE_PUBBLICAZIONE))
                            || (pubblicazione.ESITO_PUBBLICAZIONE.Equals("0") &&
                            !string.IsNullOrEmpty(pubblicazione.ERRORE_PUBBLICAZIONE)))
                                esito = "Il documento non è stato pubblicato " + pubblicazione.ERRORE_PUBBLICAZIONE;
                            else
                                if ((pubblicazione.ESITO_PUBBLICAZIONE.Equals("0") &&
                                  string.IsNullOrEmpty(pubblicazione.ERRORE_PUBBLICAZIONE)))
                                    esito = "Il documento non è stato pubblicato";

                    }
                }
                else
                    esito = "Documento da pubblicare";
            }
            return esito;
        }

        /// <summary>
        /// Funzione per il calcolo del valore del contatore da visualizzare.
        /// Funzione spostata dalla pagina tabRisultatiRicDoc
        /// </summary>
        /// <param name="oggettoCustom">Oggetto contatore del template in uso</param>
        /// <param name="paramContatore">Valore della proprietà contatore dell'oggetto InfoDoc</param>
        /// <returns>Valore da mostrare a schermo</returns>
        public static string inserisciContatore(OggettoCustom oggettoCustom, string paramContatore)
        {
            string[] formatoContDaFunzione = paramContatore.Split('-');
            string[] formatoContDaSostituire = new string[] { "A", "A", "A" };

            //for (int i = 0; i < formatoContDaSostituire.Length; i++)
            //    formatoContDaSostituire[i] = string.Empty;

            formatoContDaFunzione.CopyTo(formatoContDaSostituire, 0);

            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return paramContatore;
            }

            //Imposto il contatore in funzione del formato
            string contatore = string.Empty;
            if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
            {


                contatore = oggettoCustom.FORMATO_CONTATORE;
                contatore = contatore.Replace("ANNO", formatoContDaSostituire[1].ToString());
                contatore = contatore.Replace("CONTATORE", formatoContDaSostituire[2].ToString());
                if (!string.IsNullOrEmpty(formatoContDaSostituire[0]))
                {
                    contatore = contatore.Replace("RF", formatoContDaSostituire[0].ToString());
                    contatore = contatore.Replace("AOO", formatoContDaSostituire[0].ToString());
                }
                else
                {
                    contatore = contatore.Replace("RF", "A");
                    contatore = contatore.Replace("AOO", "A");
                }
            }
            else
            {
                contatore = paramContatore;
            }
            return eliminaBlankSegnatura(contatore);

        }

        /// <summary>
        /// Funzione spostata dalla pagina tabRisultatiRicDoc
        /// </summary>
        /// <param name="paramSegnatura"></param>
        /// <returns></returns>
        private static string eliminaBlankSegnatura(string paramSegnatura)
        {
            char separatore = '|';
            string[] temp = paramSegnatura.Split('|');
            string appoggio = string.Empty;

            if (temp.Length == 1)
            {
                temp = paramSegnatura.Split('-');
                separatore = '-';
            }

            for (int i = 0; i < temp.Length; i++)
            {
                if (!temp[i].Equals("A"))
                {
                    appoggio += temp[i];
                    if (i != temp.Length - 1)
                        appoggio += separatore;
                }
            }
            return appoggio;
        }

        public static List<ImportResult> RimuoviVersioniMassivo(List<MassiveOperationTarget> idProfiles, RemoveVersionType type, Page page)
        {
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                List<ImportResult> res = new List<ImportResult>();
                foreach (MassiveOperationTarget temp in idProfiles)
                {
                    ImportResult ir = docsPaWS.DocumentoRimuoviVersioniDaGrigioAM(temp.Id, infoUtente, type);
                    res.Add(ir);
                }
                return res;
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
                return null;
            }
        }

        public static DocumentoDiritto[] getListaVisibilitaSemplificata(Page page, string idProfile, bool cercaRimossi)
        {
            DocumentoDiritto[] listaVisibilita = null;
            try
            {
                //Mev editing ACL
                //listaVisibilita = docsPaWS.DocumentoGetVisibilitaSemplificata(UserManager.getInfoUtente(), idProfile, cercaRimossi);
                listaVisibilita = docsPaWS.DocumentGetVisibilityWithFIlter(UserManager.getInfoUtente(), idProfile, cercaRimossi, null);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return listaVisibilita;
        }

        public static string getCodiciClassifica(string idProfile, DocsPaWR.InfoUtente infoUtente)
        {
            string result = string.Empty;

            result = docsPaWS.DocumentoGetCodiciClassifica(idProfile, infoUtente);

            return result;
        }

        public static bool TrasmettiProtocolloUfficioReferente(Page page, string serverName, SchedaDocumento schedaDoc, bool isEnableUffRef, out bool verificaRagioni, out string message)
        {
            bool result = true;
            verificaRagioni = true;
            message = "";

            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                Ruolo ruolo = UserManager.getRuolo(page);

                if (!docsPaWS.TrasmettiProtocolloUfficioReferente(schedaDoc, ruolo, serverName, isEnableUffRef, infoUtente, out verificaRagioni, out message)) throw new Exception();
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        public static DocsPAWA.DocsPaWR.SearchObject[] getQueryInfoDocumentoPagingCustom(InfoUtente infoUtente, Page page, DocsPaWR.FiltroRicerca[][] query, int numPage, out int numTotPage, out int nRec, bool security, bool getIdProfilesList, bool gridPersonalization, int pageSize, bool export, Field[] visibleFieldsTemplate, String[] documentsSystemId, out SearchResultInfo[] idProfilesList)
        {
            // La lista dei system id dei documenti restituiti dalla ricerca
            SearchResultInfo[] idProfiles = null;

            nRec = 0;
            numTotPage = 0;
            try
            {
                DocsPAWA.DocsPaWR.SearchObject[] DocS = null;
                string textToSearch = string.Empty;


                //if (!IsRicercaFullText(query, out textToSearch))
                {
                    DocS = docsPaWS.DocumentoGetQueryDocumentoPagingCustom(infoUtente, query, numPage, security, pageSize, getIdProfilesList, gridPersonalization, export, visibleFieldsTemplate,documentsSystemId, out numTotPage, out nRec, out idProfiles);
                }
                /*    else
                    {
                        // reperimento oggetto infoutente
                     InfoUtente infoUtente = UserManager.getInfoUtente(page);

                        // Reperimento dalla sessione del contesto di ricerca fulltext
                        FullTextSearchContext context = page.Session["FULL_TEXT_CONTEXT"] as FullTextSearchContext;

                        if (context == null)
                            // Prima ricerca fulltext
                            context = new FullTextSearchContext();
                        else if (!textToSearch.Equals(context.TextToSearch))
                            // Se il testo inserito per la ricerca è differente
                            // da quello presente in sessione viene creato 
                            // un nuovo oggetto di contesto per la ricerca
                            context = new FullTextSearchContext();

                        // Impostazione indice pagina richiesta
                        context.RequestedPageNumber = numPage;
                        // Impostazione testo da ricercare
                        context.TextToSearch = textToSearch;

                        // Ricerca fulltext
                        DocS = FullTextSearch(page, infoUtente, ref context);

                        // Reperimento numero pagine e record totali
                        numTotPage = context.TotalPageNumber;
                        nRec = context.TotalRecordCount;

                        // Impostazione in sessione del contesto di ricerca fulltext
                        page.Session["FULL_TEXT_CONTEXT"] = context;
                    }
                   * */

                if (DocS == null)
                {
                    throw new Exception();
                }

                // Impostazione della lista dei sisyem id dei documento
                idProfilesList = idProfiles;

                return DocS;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            idProfilesList = idProfiles;
            return null;
        }

        public static ItemsConservazione[] getItemsConservazioneLite(string idIstanza, Page page, InfoUtente infoUtente)
        {
            ItemsConservazione[] itemsCons = null;
            try
            {
                itemsCons = docsPaWS.ConservazioneGetItemsByIdLite(idIstanza, infoUtente);
                if (itemsCons == null)
                    throw new Exception();
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return itemsCons;
        }

        public static string getSegnaturaRepertorio(Page page, string docnumber, string codiceAmm)
        {
            string segnaturaRepertorio = string.Empty;
            try
            {
                segnaturaRepertorio = docsPaWS.GetSegnaturaRepertorio(docnumber, codiceAmm);
                if (segnaturaRepertorio == null)
                    throw new Exception();
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return segnaturaRepertorio;
        }

        public static bool UpdatePreferredInstance(Page page, string idIstanza, InfoUtente infoUtente, Ruolo ruolo)
        {
            bool result = false;
            try
            {
                result = docsPaWS.UpdatePreferredInstance(idIstanza, infoUtente, ruolo);
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return result;
        }

        public static string verificaStopWord(Page page, string stringaInserita)
        {
            string retValue = string.Empty;
            if (stringaInserita.Contains("%"))
            {
                string[] splitStringa = stringaInserita.Split('%');
                for (int a = 0; a < splitStringa.Length; a++)
                {
                    string ritorno = docsPaWS.VerificaStopWord(splitStringa[a]);
                    if (!string.IsNullOrEmpty(ritorno))
                        return ritorno;
                }
            }
            else
                retValue = docsPaWS.VerificaStopWord(stringaInserita);

            return retValue;
        }

        /// <summary>
        /// Metodo per la visualizzazione della segnatura di un documento a partire dal proprio id
        /// </summary>
        /// <param name="idProfile">Id del documento</param>
        /// <returns>Segnatura del documento</returns>
        public static String GetDocumentSignatureByProfileId(String idProfile)
        {
            return docsPaWS.GetDocumentSignatureByProfileId(idProfile);
 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public static string AllegatoIsEsterno(string version_id)
        {
            string res = "0";
            try
            {
                res = docsPaWS.AllegatoIsEsterno(version_id);
                if (string.IsNullOrEmpty(res))
                    res = "0";
            }
            catch (Exception ex)
            {
                throw ex;
                return "0";
            }

            return res;
        }

        public static Tab GetDocumentTab(string document_Id, InfoUtente info_User)
        {
            Tab docTab = null;
            try
            {
                docTab = docsPaWS.GetDocumentTab(document_Id, info_User);
            }
            catch (Exception ex1)
            {
                throw ex1;
            }
            return docTab;
        }

        public static int GetDocumentTrasmToCCDest(string document_Id, InfoUtente info_User)
        {
            int docTab = 0;
            try
            {
                docTab = docsPaWS.GetDocumentTrasmToCCDest(document_Id, info_User);
            }
            catch (Exception ex1)
            {
                throw ex1;
            }
            return docTab;
        }
    }
}

