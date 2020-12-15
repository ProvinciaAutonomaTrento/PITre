using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using NttDataWA.UserControls;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web.SessionState;


namespace NttDataWA.UIManager
{
    public class DocumentManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        /// <summary>
        /// Reperimento di un nuovo oggetto SchedaDocumento 
        /// </summary>
        /// <returns>
        /// SchedaDocumento predisposta per l'inserimento
        /// </returns>
        public static SchedaDocumento NewSchedaDocumento()
        {
            try
            {
                DocsPaWR.SchedaDocumento newScheda = docsPaWS.NewSchedaDocumento(UserManager.GetInfoUser());
                FileManager.setSelectedFile(newScheda.documenti[0]);
                return newScheda;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static SchedaDocumento Record(Page page, SchedaDocumento schedaDoc, DocsPaWR.Fascicolo fascicolo, DocsPaWR.TemplateTrasmissione template, out DocsPaWR.ResultProtocollazione risultatoProtocollazione, ref string returnMsg)
        {
            string segnatura = string.Empty;
            string message = string.Empty;
            risultatoProtocollazione = DocsPaWR.ResultProtocollazione.OK;
            try
            {
                if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.codice))
                {
                    schedaDoc.codiceFascicolo = fascicolo.codice;
                }
                schedaDoc = DocumentManager.creaProtocollo(page, schedaDoc, fascicolo, out risultatoProtocollazione, out message);

                if (!string.IsNullOrEmpty(message))
                {
                    returnMsg += message;
                    return null;
                }

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
                ////TRASMISSIONE RAPIDA
                //if (schedaDoc.systemId != null && template != null)
                //{
                //    DocsPaWR.Trasmissione trasmissione = null;
                //    DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);
                //    DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(schedaDoc);

                //    trasmissione = TrasmManager.addTrasmDaTemplate(page, infoDoc, template, infoUtente);
                //    if (trasmissione == null || (trasmissione.systemId == "" || trasmissione.systemId == null))
                //        returnMsg += " La trasmissione non è stata creata ";
                //    else
                //    {
                //        trasmissione = TrasmManager.executeTrasm(page, trasmissione);
                //        if (trasmissione == null)
                //            returnMsg += " Il documento non è stato trasmesso";
                //    }
                //}

                return schedaDoc;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Return the selected record from session
        /// </summary>
        /// <returns>SchedaDocumento</returns>
        public static SchedaDocumento getSelectedRecord()
        {
            try
            {
                return (SchedaDocumento)GetSessionValue("selectedRecord");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Return the selected record from session
        /// </summary>
        /// <returns>SchedaDocumento</returns>
        public static void setSelectedRecord(SchedaDocumento record)
        {
            try
            {
                RemoveSessionValue("selectedRecord");
                SetSessionValue("selectedRecord", record);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        #region Session Attachments, versions
        /// <summary>
        /// Set the versionId of the version selected attachment
        /// </summary>
        public static void setSelectedAttachId(string id)
        {
            try
            {
                RemoveSessionValue("selectedAttachmentId");
                SetSessionValue("selectedAttachmentId", id);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Removed the versionId of the version selected attachment
        /// </summary>
        public static void RemoveSelectedAttachId()
        {
            try
            {
                RemoveSessionValue("selectedAttachmentId");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Returns the versionId of the version selected attachment
        /// </summary>
        /// <returns>string</returns>
        public static string getSelectedAttachId()
        {
            try
            {
                return (string)GetSessionValue("selectedAttachmentId");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Get Selected Attachment
        /// </summary>
        /// <returns></returns>
        public static Allegato GetSelectedAttachment()
        {
            try
            {
                Allegato retValue = null;
                string versionId = DocumentManager.getSelectedAttachId();

                if (!string.IsNullOrEmpty(versionId))
                {
                    SchedaDocumento doc = DocumentManager.getSelectedRecord();
                    if (doc != null)
                    {
                        foreach (Allegato allegato in doc.allegati)
                        {
                            if (allegato.versionId.Equals(versionId))
                            {
                                retValue = allegato;
                                break;
                            }
                        }
                    }
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
        /// Set the version number of selected
        /// </summary>
        public static void setSelectedNumberVersion(string id)
        {
            try
            {
                RemoveSessionValue("selectedNumberVersion");
                SetSessionValue("selectedNumberVersion", id);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Returns the version number of selected
        /// </summary>
        /// <returns>string</returns>
        public static string getSelectedNumberVersion()
        {
            try
            {
                return (string)GetSessionValue("selectedNumberVersion");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Removed from the session version number selected
        /// </summary>
        public static void removeSelectedNumberVersion()
        {
            try
            {
                RemoveSessionValue("selectedNumberVersion");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
        #endregion

        #region Attachments
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return res;
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
                if (string.IsNullOrEmpty(res)) res = "0";
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="documento"></param>
        /// <param name="allegato"></param>
        public static bool swapAttachment(Page page, DocsPaWR.Documento doc, DocsPaWR.Allegato attach)
        {
            bool result = false;
            try
            {
                result = docsPaWS.DocumentoScambiaAllegato(UserManager.GetInfoUser(), attach, doc);
                //if (result)
                //{
                //    SchedaDocumento documentTab = DocumentManager.getDocumentDetails(null, getSelectedRecord().systemId, getSelectedRecord().docNumber);
                //    setSelectedRecord(documentTab);
                //    FileManager.setSelectedFile(documentTab.documenti[0]);
                //}
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="filterTipologiaAllegato"></param>
        /// <param name="simplifiedInteroperabilityId"></param>
        /// <returns></returns>
        public static DocsPaWR.Allegato[] getAttachments(DocsPaWR.SchedaDocumento schedaDocumento, string filterTipologiaAllegato, string simplifiedInteroperabilityId = "")
        {
            DocsPaWR.Allegato[] allegati = null;

            try
            {
                DocsPaWR.InfoDocumento infoDoc = getInfoDocumento(schedaDocumento);

                allegati = docsPaWS.DocumentoGetAllegati(infoDoc.docNumber, filterTipologiaAllegato, simplifiedInteroperabilityId);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return allegati;
        }

        /// <summary>
        /// Add new Attachment
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public static DocsPaWR.Allegato AddAttachment(DocsPaWR.Allegato attachment)
        {
            try
            {
                DocsPaWR.Allegato att = docsPaWS.DocumentoAggiungiAllegato(UserManager.GetInfoUser(), attachment);
                if (att == null)
                {
                    throw new NttDataWAException("", "Class: DocumentManager<br/>Method: AddAttachment");
                }
                return att;
            }
            //catch (Exception ex)
            //{
            //    throw new NttDataWAException("" + ex.Message, ex, "Class: DocumentManager<br/>Method: AddAttachment");
            //}
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Rimuove l'allegato selezionato nella griglia
        /// </summary>
        /// <returns></returns>
        public static bool RemoveAttachment(int selectedPage, int pageSize, DocsPaWR.Allegato allegato)
        {
            bool result = false;
            if (allegato != null)
            {
                try
                {
                    result = docsPaWS.DocumentoRimuoviAllegato(allegato, UserManager.GetInfoUser(), getSelectedRecord());

                    if (result)
                    {
                        // Delete attachment from document in session
                        List<DocsPaWR.Allegato> allegati = new List<DocsPaWR.Allegato>(getSelectedRecord().allegati);
                        allegati.Remove(allegato);
                        SchedaDocumento doc = getSelectedRecord();
                        doc.allegati = allegati.ToArray();
                        setSelectedRecord(doc);
                        // Update grid index
                        if (selectedPage > (int)Math.Round((allegati.Count / pageSize) + 0.49))
                        {
                            selectedPage = (int)Math.Round((allegati.Count / pageSize) + 0.49);
                        }
                        // Delete SelectedAttachment session
                        DocumentManager.setSelectedAttachId(null);
                    }
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return false;
                }
            }
            return result;
        }
        #endregion

        #region Versions
        /// <summary>
        /// Modify description version attachment or doc main
        /// </summary>
        /// <param name="doc"></param>
        public static bool ModifyVersion(string description)
        {
            bool result = false;
            SchedaDocumento documentTab;
            FileRequest fileReq = null;
            try
            {
                //ho selezionato un allegato
                if (!string.IsNullOrEmpty(getSelectedAttachId()))
                {
                    documentTab = getDocumentDetails(null, GetSelectedAttachment().docNumber, GetSelectedAttachment().docNumber);
                }
                else
                {
                    documentTab = getSelectedRecord();
                }

                if (getSelectedNumberVersion() != null && Convert.ToInt32(getSelectedNumberVersion()) == 0)
                {
                    fileReq = documentTab.documenti[0];
                    fileReq.descrizione = description;
                }
                else
                {
                    string versionId = (from doc in documentTab.documenti where doc.version.Equals(getSelectedNumberVersion()) select doc.versionId).FirstOrDefault();
                    foreach (Documento d in documentTab.documenti)
                    {
                        if (d.versionId.Equals(versionId))
                        {
                            fileReq = d;
                            fileReq.descrizione = description;
                            break;
                        }
                    }
                }
                result = docsPaWS.DocumentoModificaVersione(UserManager.GetInfoUser(), fileReq);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="documento"></param>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public static bool RemoveVersion(DocsPaWR.Documento document, DocsPaWR.SchedaDocumento documentTab, string type)
        {
            string errMsg = "Errore nell'eliminazione della versione";
            bool result = false;
            InfoUtente infoUtente = UserManager.GetInfoUser();
            try
            {
                result = docsPaWS.DocumentoRimuoviVersione(document, infoUtente, documentTab);
                //}
                //catch (Exception e)
                //{
                //    throw new NttDataWAException(errMsg + "<br/>" + e.Message, e, "Class: DocumentManager<br/>Method: RemoveVersion");
                //}
                if (result)//aggiorno la sessione
                {

                    SchedaDocumento newDocumentTab = DocumentManager.getDocumentDetails(null, getSelectedRecord().systemId, getSelectedRecord().docNumber);
                    setSelectedNumberVersion("0");//0 indica che la versione corrente da visualizzare nel content è l'ultima in ordine di creazione                
                    DocumentManager.setSelectedRecord(newDocumentTab);
                    if (type.Equals("D"))
                        FileManager.setSelectedFile(newDocumentTab.documenti[0]);
                    else
                        setSelectedAttachId((from att in newDocumentTab.allegati where att.docNumber.Equals(document.docNumber) select att.versionId).FirstOrDefault());
                }
                else
                {
                    //throw new NttDataWAException(errMsg, "Class: DocumentManager<br/>Method: RemoveVersion");
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        public static Documento[] addVersion(Documento[] list, Documento record)
        {
            Documento[] nuovaLista;
            if (list != null)
            {
                int len = list.Length;
                nuovaLista = new Documento[len + 1];
                list.CopyTo(nuovaLista, 1);
                nuovaLista[0] = record;
            }
            else
            {
                nuovaLista = new Documento[1];
                nuovaLista[0] = record;
            }
            return nuovaLista;
        }

        /// <summary>
        /// Adding a new version to the main document
        /// </summary>
        /// <param name="page"></param>
        /// <param name="fileRequest"></param>
        /// <param name="toSend"></param>
        /// <param name="refresh"></param>
        /// <returns></returns>
        public static FileRequest AddVersion(FileRequest fileRequest, bool toSend)
        {
            bool isAttachment = fileRequest.GetType() == typeof(Allegato) ? true : false;
            SchedaDocumento documentTab = getSelectedRecord();
            try
            {
                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                fileRequest = docsPaWS.DocumentoAggiungiVersione(fileRequest, infoUtente);
                if (fileRequest == null)
                {
                    throw new NttDataWAException("", "Class: DocumentManager\nMethod: AddVersion");
                }
                if (isAttachment)
                {
                    setSelectedRecord(getDocumentDetails(null, getSelectedRecord().docNumber, getSelectedRecord().docNumber));
                    setSelectedAttachId(fileRequest.versionId);
                }
                else
                {
                    if (documentTab.documenti != null && documentTab.documenti.Length > 0)
                    {
                        List<Documento> listNewDocument = new List<Documento>();
                        listNewDocument.Add(fileRequest as Documento);
                        listNewDocument.AddRange(documentTab.documenti);
                        documentTab.documenti = listNewDocument.ToArray();
                    }
                    else
                    {
                        documentTab.documenti = new Documento[1];
                        documentTab.documenti[0] = fileRequest as Documento;
                    }
                    setSelectedRecord(documentTab);
                    FileManager.setSelectedFile(fileRequest);
                }
                if (toSend)
                {
                    documentTab = docsPaWS.DocumentoSetFlagDaInviare(documentTab);

                    if (documentTab == null)
                    {
                        throw new NttDataWAException("", "Class: DocumentManager\nMethod: AddVersion");
                    }
                }
                return fileRequest;
            }
            //catch (Exception es)
            //{
            //    throw new NttDataWAException("" + es.Message, es, "Class: DocumentManager\nMethod: AddVersion");
            //}
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idProfile"></param>
        /// <param name="docnumber"></param>
        /// <param name="segnatura"></param>
        /// <param name="fascicolo"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool fascicolaRapida(Page page, string idProfile, string docnumber, string segnatura, Fascicolo fascicolo, out string msg)
        {
            msg = string.Empty;
            bool result = false;
            try
            {
                InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="schedaDoc"></param>
        /// <param name="fasc"></param>
        /// <param name="risultatoProtocollazione"></param>
        /// <returns></returns>
        public static SchedaDocumento creaProtocollo(Page page, SchedaDocumento schedaDoc, DocsPaWR.Fascicolo fasc, out DocsPaWR.ResultProtocollazione risultatoProtocollazione, out string  message)
        {
            message = string.Empty;

            SchedaDocumento result = null;
            //DocsPaWR.ResultProtocollazione risultatoProtocollazione;
            risultatoProtocollazione = DocsPaWR.ResultProtocollazione.OK;
            try
            {
                string valoreChiaveConsentiClass = string.Empty;
                valoreChiaveConsentiClass = Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_BLOCCA_CLASS.ToString());
                if (fasc != null && ((fasc.tipo.Equals("G") && fasc.isFascConsentita != null && fasc.isFascConsentita == "0") || (fasc.tipo.Equals("P") && !fasc.isFascicolazioneConsentita)) && !string.IsNullOrEmpty(valoreChiaveConsentiClass) && valoreChiaveConsentiClass.Equals("1"))
                {
                    risultatoProtocollazione = DocsPaWR.ResultProtocollazione.ERRORE_DURANTE_LA_FASCICOLAZIONE;
                    message = "Non è possibile inserire documenti nel fascicolo selezionato. Selezionare un nodo foglia.";
                    return null;
                }
                else
                {
                    result = protocolla(page, schedaDoc, out risultatoProtocollazione);
                }

                if (result == null || risultatoProtocollazione != DocsPaWR.ResultProtocollazione.OK)
                {

                    switch (risultatoProtocollazione)
                    {
                        case DocsPaWR.ResultProtocollazione.AMMINISTRAZIONE_MANCANTE:
                            message = "Identificativo dell'amministrazione non trovata.";
                            break;
                        case DocsPaWR.ResultProtocollazione.DESTINATARIO_MANCANTE:
                            message = "Il destinatario è obbligatorio.";
                            break;
                        case DocsPaWR.ResultProtocollazione.MITTENTE_MANCANTE:
                            message = "Il mittente è obbligatorio.";
                            break;
                        case DocsPaWR.ResultProtocollazione.OGGETTO_MANCANTE:
                            message = "L'oggetto è obbligatorio.";
                            break;
                        case DocsPaWR.ResultProtocollazione.REGISTRO_MANCANTE:
                            message = "Il registro è obbligatorio.";
                            break;
                        case DocsPaWR.ResultProtocollazione.REGISTRO_CHIUSO:
                            message = "Il registro non è aperto.";
                            break;
                        case DocsPaWR.ResultProtocollazione.STATO_REGISTRO_ERRATO:
                            message = "Lo stato del registro non è corretto.";
                            break;
                        case DocsPaWR.ResultProtocollazione.DATA_SUCCESSIVA_ATTUALE:
                            message = "La data di protocollazione è successiva a quella attuale.";
                            break;
                        case DocsPaWR.ResultProtocollazione.DATA_ERRATA:
                            message = "La data di protocollazione non è valida.";
                            break;
                        case DocsPaWR.ResultProtocollazione.APPLICATION_ERROR:
                            //message = "Errore imprevisto dell'applicazione. Ritentare l'operazione.";
                            message = "In questo momento non è stato possibile protocollare il documento.<BR><BR>Ripetere l'operazione di protocollazione.";
                            break;
                        case DocsPaWR.ResultProtocollazione.DOCUMENTO_GIA_PROTOCOLLATO: //TODO: Se predisposto già protocollato
                            //message = "Errore imprevisto dell'applicazione. Ritentare l'operazione.";
                            message = "Il documento risulta già protocollato con segnatura:<BR><BR>" + result.protocollo.segnatura;
                            break;
                        case DocsPaWR.ResultProtocollazione.FORMATO_SEGNATURA_MANCANTE:
                            message = " Formato della segnatura non impostato. Contattare l'Amministratore";
                            break;
                        case DocsPaWR.ResultProtocollazione.ERRORE_WSPIA_PROTOCOLLO_ENTRATA_MITTENTE:
                            message = " WSPIA - Non è possibile utilizzare il mittente selezionato.";
                            break;

                        case DocsPaWR.ResultProtocollazione.ERRORE_WSPIA_CLASSIFICA_NODO_FOGLIA:
                            message = " WSPIA - Selezionare la voce di titolario di dettaglio";
                            break;
                        case DocsPaWR.ResultProtocollazione.ERRORE_WSPIA_PROTOCOLLAZIONE_SEMPLICE:
                            message = " WSPIA - Segnatura restituita nulla o vuota.";
                            break;
                        case DocsPaWR.ResultProtocollazione.DOCUMENTO_IN_LIBRO_FIRMA_PASSO_NON_ATTESO:
                            message = "Passo non atteso.";
                            break;
                    }

                     return null;
                }

                //if (result != null && risultatoProtocollazione == DocsPaWR.ResultProtocollazione.OK)
                //{
                //    if (schedaDoc.protocollo.invioConferma != null && schedaDoc.protocollo.invioConferma.Equals("1"))
                //    {
                //        Registro reg = schedaDoc.registro;
                //        // nel caso si arrivi dalla popup di scelta RF per invio ricezione automatica
                //        if (!string.IsNullOrEmpty(schedaDoc.id_rf_invio_ricevuta))
                //        {
                //            DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
                //            reg = ws.GetRegistroBySistemId(schedaDoc.id_rf_invio_ricevuta);
                //        }

                //        //solo se la protocollazione è andata a buon fine invio la notifica
                //        //if(!reg.invioRicevutaManuale.ToUpper().Equals("1"))

                //        //DocumentManager.DocumentoInvioRicevuta(page, result, reg);
                //    }

                // }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="schedaDoc"></param>
        /// <param name="risultatoProtocollazione"></param>
        /// <returns></returns>
        public static SchedaDocumento protocolla(Page page, SchedaDocumento schedaDoc, out DocsPaWR.ResultProtocollazione risultatoProtocollazione)
        {
            SchedaDocumento result = null;
            risultatoProtocollazione = 0;

            try
            {
                InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
                Ruolo ruolo = UIManager.RoleManager.GetRoleInSession();
                schedaDoc = setFusionFields(page, schedaDoc, infoUtente);

                result = docsPaWS.DocumentoProtocolla(schedaDoc, infoUtente, ruolo, out risultatoProtocollazione);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        public static SchedaDocumento protocolla(Page page, SchedaDocumento schedaDoc, DocsPaWR.Fascicolo fascicolo, DocsPaWR.TemplateTrasmissione template, out DocsPaWR.ResultProtocollazione risultatoProtocollazione, ref string returnMsg)
        {
            string segnatura = string.Empty;
            string message = string.Empty;
            risultatoProtocollazione = DocsPaWR.ResultProtocollazione.OK;
            try
            {
                if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.codice))
                {
                    schedaDoc.codiceFascicolo = fascicolo.codice;
                }
                schedaDoc = DocumentManager.creaProtocollo(page, schedaDoc, fascicolo, out risultatoProtocollazione, out message);

                if (!string.IsNullOrEmpty(message))
                {
                    returnMsg += message;
                    return null;
                }

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
                    DocsPaWR.InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.EsitoRicercaDuplicatiEnum cercaDuplicati(SchedaDocumento schedaDoc, string cercaDuplicati2, out InfoProtocolloDuplicato[] datiProtDupl)
        {
            try
            {
                DocsPaWR.EsitoRicercaDuplicatiEnum res = DocsPaWR.EsitoRicercaDuplicatiEnum.NessunDuplicato;
                datiProtDupl = new InfoProtocolloDuplicato[0];
                res = docsPaWS.DocumentoCercaDuplicatiInfo(schedaDoc, cercaDuplicati2, out datiProtDupl);
                return res;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                datiProtDupl = null;
                return new EsitoRicercaDuplicatiEnum();
            }
        }

        /// <summary>
        /// Restore ACL
        /// </summary>
        /// <param name="docDiritto"></param>
        /// <param name="personOrGroup"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool RestoreACL(DocsPaWR.DocumentoDiritto docDiritto, string personOrGroup, DocsPaWR.InfoUtente infoUtente, string typeObject)
        {
            bool result = false;
            try
            {
                docDiritto.tipoOggetto = typeObject;
                result = docsPaWS.RipristinaACLWithType(docDiritto, personOrGroup, infoUtente, typeObject);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Remove ACL
        /// </summary>
        /// <param name="docDiritto"></param>
        /// <param name="personOrGroup"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool RemoveACL(DocsPaWR.DocumentoDiritto docDiritto, string personOrGroup, DocsPaWR.InfoUtente infoUtente, string typeObject)
        {
            bool result = false;
            try
            {
                docDiritto.tipoOggetto = typeObject;
                result = docsPaWS.EditingACLWithType(docDiritto, personOrGroup, infoUtente, typeObject);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Return a list of simplified visibility
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idProfile"></param>
        /// <param name="cercaRimossi"></param>
        /// <returns></returns>
        public static DocumentoDiritto[] GetSimpliedListVisibility(Page page, string idProfile, bool cercaRimossi)
        {
            DocumentoDiritto[] SimpliedListVisibility = null;
            try
            {
                SimpliedListVisibility = docsPaWS.DocumentoGetVisibilitaSemplificata(UserManager.GetInfoUser(), idProfile, cercaRimossi);
                return SimpliedListVisibility;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Return a list of simplified visibility with filters
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idProfile"></param>
        /// <param name="cercaRimossi"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static DocumentoDiritto[] GetSimpliedListVisibilityWithFilters(Page page, string idProfile, bool cercaRimossi, FilterVisibility[] filters)
        {
            DocumentoDiritto[] SimpliedListVisibility = null;
            try
            {
                SimpliedListVisibility = docsPaWS.DocumentGetVisibilityWithFIlter(UserManager.GetInfoUser(), idProfile, cercaRimossi, filters);
                return SimpliedListVisibility;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Return document atipic info
        /// </summary>
        /// <param name="tipoOggetto"></param>
        /// <param name="idDocOrFasc"></param>
        /// <returns></returns>
        public static DocsPaWR.InfoAtipicita GetInfoAtipicita(DocsPaWR.TipoOggettoAtipico tipoOggetto, string idDocOrFasc)
        {
            try
            {
                InfoAtipicita infoAtipicita = null;
                InfoUtente infoUtente = UserManager.GetInfoUser();
                if (infoUtente != null && !string.IsNullOrEmpty(idDocOrFasc) && GetValueAtipic())
                {
                    infoAtipicita = docsPaWS.GetInfoAtipicita(infoUtente, tipoOggetto, idDocOrFasc);
                }
                return infoAtipicita;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Return value atipic key db
        /// </summary>
        /// <returns></returns>
        public static bool GetValueAtipic()
        {
            try
            {
                bool result = false;
                InfoUtente infoUtente = null;

                try
                {
                    infoUtente = UserManager.GetInfoUser();
                }
                catch (Exception e)
                {
                    string[] amministrazione = ((string)HttpContext.Current.Session["AMMDATASET"]).Split('@');
                    string codiceAmministrazione = amministrazione[0];
                }

                string valoreChiaveAtipicita = NttDataWA.Utils.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ATIPICITA_DOC_FASC");
                if (string.IsNullOrEmpty(valoreChiaveAtipicita))
                    valoreChiaveAtipicita = NttDataWA.Utils.InitConfigurationKeys.GetValue("0", "ATIPICITA_DOC_FASC");

                if (!string.IsNullOrEmpty(valoreChiaveAtipicita) && valoreChiaveAtipicita.Equals("1"))
                    result = true;

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        protected static SchedaDocumento setFusionFields(Page page, SchedaDocumento schedaDocumento, InfoUtente infoUtente)
        {
            try
            {
                // campi obbligatori per DocsFusion
                schedaDocumento.idPeople = infoUtente.idPeople;
                schedaDocumento.userId = infoUtente.userId;
                if (schedaDocumento.typeId == null)
                {
                    schedaDocumento.typeId = DocumentManager.GetTypeId();
                }
                if (schedaDocumento.appId == null)
                    schedaDocumento.appId = "ACROBAT";

                return schedaDocumento;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// restitiusci il valore nel web.config dei ws DOC_TYPE
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string GetTypeId()
        {
            try
            {
                return docsPaWS.doucmentoGetDocType();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Reperimento del dettaglio del documento
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static SchedaDocumento getDocumentDetails(Page page, string idProfile, string docNumber)
        {
            DocsPaWR.SchedaDocumento sd = new DocsPaWR.SchedaDocumento();


            if (idProfile == null && docNumber == null)
            {
                sd = null;
            }
            else
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                sd = docsPaWS.DocumentoGetDettaglioDocumento(infoUtente, idProfile, docNumber);
                if ((sd == null))
                {
                    string errorMessage = string.Empty;

                    if (sd == null)
                    {
                        //verificata ACL, ritorna semmai un msg in out errorMessage
                        int rtn = verifyDocumentACL("D", idProfile, infoUtente, out errorMessage);
                        if (rtn == -1) errorMessage = "Attenzione, non è stato possibile recuperare i dati del documento richiesto.\\nConsultare il log per maggiori dettagli.";
                    }
                }
            }


            return sd;
        }

        public static List<DocumentoVisualizzato> GetLastDocumentsView()
        {
            return docsPaWS.GetLastDocumentsView(UserManager.GetInfoUser()).ToList();
        }

        /// <summary>
        /// Reperimento del dettaglio del documento, senza scrittura della data di lettura del documento
        /// /// </summary>
        /// <param name="page"></param>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static SchedaDocumento getDocumentDetailsNoDataCheck(Page page, string idProfile, string docNumber, out string errorMessage)
        {
            DocsPaWR.SchedaDocumento sd = new SchedaDocumento();
            errorMessage = string.Empty;
                if (idProfile == null && docNumber == null)
                {
                    sd = null;
                }
                else
                {
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    sd = docsPaWS.DocumentoGetDettaglioDocumentoNoDataVista(infoUtente, idProfile, docNumber);

                    if ((sd == null) || (sd.inCestino != null && sd.inCestino == "1"))
                    {
                        if (sd == null)
                        {
                            int rtn = verifyDocumentACL("D", idProfile, infoUtente, out errorMessage);
                            if (rtn == -1) //errore generico, negli altri casi il metodo  verificaACL ritorna un msg in  errorMessage.
                                errorMessage = "Attenzione, non è stato possibile recuperare i dati del documento richiesto.\\nConsultare il log per maggiori dettagli.";
                        }
                        else
                        {
                            errorMessage = "Il documento è stato rimosso.\\nNon è più possibile visualizzarlo.";
                        }

                        if (errorMessage.Equals(""))
                            errorMessage = "Attenzione, non è stato possibile recuperare i dati del documento richiesto.\\nConsultare il log per maggiori dettagli.";
                    }
                }
            return sd;
        }

        /// <summary>
        /// Ritorna le versioni del documento principale associato al documento con primary key docNumber
        /// </summary>
        /// <param name="infoUser"></param>
        /// <param name="docNumber">Identificativo del documento nella profile</param>
        /// <returns></returns>
        public static Documento[] GetVersionsMainDocument(InfoUtente infoUser, string docNumber)
        {
            try
            {
                return docsPaWS.GetVersionsMainDocument(infoUser, docNumber);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        /// <summary>
        /// Reperimento del dettaglio del documento senza considerare la security
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static SchedaDocumento getDocumentDetailsNoSecurity(Page page, string idProfile, string docNumber)
        {
            DocsPaWR.SchedaDocumento sd = new DocsPaWR.SchedaDocumento();

            try
            {
                if (idProfile == null && docNumber == null)
                {
                    sd = null;
                }
                else
                {
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    sd = docsPaWS.DocumentoGetDettaglioDocumentoNoSecurity(infoUtente, idProfile, docNumber);
                    if ((sd == null))// || (sd.inCestino != null && sd.inCestino == "1"))
                    {
                        string errorMessage = string.Empty;

                        if (sd == null)
                        {
                            //verificata ACL, ritorna semmai un msg in out errorMessage
                            int rtn = verifyDocumentACL("D", idProfile, infoUtente, out errorMessage);
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
                            page.Response.Redirect("~/Index.aspx");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return sd;
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool CheckRevocationAcl()
        {
            SchedaDocumento doc = DocumentManager.getSelectedRecord();
            try
            {
                if (doc != null)
                {
                    DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

                    // Il metodo verificaACL viene chiamato anche per acl dei fascicoli.
                    string errorMessage = "";
                    int result = DocumentManager.verifyDocumentACL("D", doc.systemId, UserManager.GetInfoUser(), out errorMessage);

                    // Imposta lo stato della revoca dell'acl
                    // ************************************
                    // *  CASI:
                    // *  result = 0 --> ACL rimossa
                    // *  result = 1 --> documento rimosso
                    // *  result =2 --> documento "normale"
                    // *  result -1 --> errore generico
                    // **************************************
                    return (result == 0);
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
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
        public static int verifyDocumentACL(string tipoObj, string systemId, DocsPaWR.InfoUtente infoUtente, out string errorMessage)
        {
            int result = -1;
            errorMessage = string.Empty;

            try
            {
                result = docsPaWS.VerificaACL(tipoObj, systemId, infoUtente, out errorMessage);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return -1;
            }
            return result;
        }

        /// <summary>
        /// restituisce true se alla versione con versionId è associato un file con impressa la segnatura
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public static bool IsVersionWithSegnature(string versionId)
        {
            try
            {
                return docsPaWS.IsVersionWithSegnature(UserManager.GetInfoUser(), versionId);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// get info document from schedaDocumento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public static InfoDocumento getInfoDocumento(SchedaDocumento schedaDocumento)
        {
            try
            {
                InfoDocumento infoDoc = new InfoDocumento();

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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Add to work area
        /// </summary>
        /// <param name="page"></param>
        /// <param name="schedaDocumento"></param>
        public static bool addWorkArea(Page page, SchedaDocumento schedaDocumento)
        {
            try
            {
                Utente utente = UserManager.GetUserInSession();
                Ruolo ruolo = UserManager.GetSelectedRole();

                // call ws
                bool result = docsPaWS.DocumentoExecAddLavoro(getInfoDocumento(schedaDocumento).idProfile, getInfoDocumento(schedaDocumento).tipoProto, null, UserManager.GetInfoUser(), (schedaDocumento.registro != null ? schedaDocumento.registro.systemId : ""));

                // add in session (TODO)
                //Hashtable listInArea = new Hashtable();
                //listInArea.Add(schedaDocumento.docNumber, schedaDocumento);
                //page.Session["listInArea"] = listInArea;

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static void addWorkArea(Page page, InfoDocumento infoDocumento)
        {
            try
            {
                Utente utente = UserManager.GetUserInSession();
                Ruolo ruolo = UserManager.GetSelectedRole();
                bool result = docsPaWS.DocumentoExecAddLavoro(infoDocumento.idProfile, infoDocumento.tipoProto, null, UserManager.GetInfoUser(), (infoDocumento.codRegistro != null ? infoDocumento.idRegistro : ""));

                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static SchedaDocumento AbortRecord(string text)
        {
            try
            {
                SchedaDocumento schedaDoc = getSelectedRecord();
                InfoDocumento infoDocumento = getInfoDocumento(schedaDoc);
                DocsPaWR.ProtocolloAnnullato protAnnullato = new DocsPaWR.ProtocolloAnnullato();
                protAnnullato.autorizzazione = text;

                InfoUtente infoUtente = UserManager.GetInfoUser();

                schedaDoc.protocollo.protocolloAnnullato = protAnnullato;
                //string msg = ProfilazioneDocManager.VerifyAndSetTipoDoc(UserManager.GetInfoUser(), ref schedaDoc, page);
                // if (string.IsNullOrEmpty(msg))
                // {

                if (!docsPaWS.DocumentoExecAnnullaProt(infoUtente, ref schedaDoc, protAnnullato))
                    throw new Exception();
                else
                {
                    //se il protocollo è in entrata ed è stato ricevuto per interoperabilità, allora invio una notifica di annullamento al mittente
                    if (schedaDoc.protocollo.GetType() == typeof(DocsPaWR.ProtocolloEntrata))
                    {
                        if (!string.IsNullOrEmpty(schedaDoc.documento_da_pec) || schedaDoc.typeId.Equals(SimplifiedInteroperabilityManager.SimplifiedInteroperabilityId))
                        {
                            ProtocolloInvioNotificaAnnulla(schedaDoc);
                        }
                    }
                }
                // }

                return schedaDoc;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        private static void ProtocolloInvioNotificaAnnulla(SchedaDocumento schedaDoc)
        {
            //Andrea De Marco - MEV Gestione Eccezioni PEC - Aggiunta del schedaDoc.interop.Equals("E")
            //Per ripristino commentare De Marco e decommentare il codice sottostante
            //End De Marco
            if (schedaDoc.interop != null && !schedaDoc.interop.Equals("") && !schedaDoc.interop.Equals("P") && !schedaDoc.interop.Equals("E"))
            //if (schedaDoc.interop != null && !schedaDoc.interop.Equals("") && !schedaDoc.interop.Equals("P"))
            {
                try
                {
                    DocsPaWR.Registro reg;
                    string idRegistro = (schedaDoc.protocollo as DocsPaWR.ProtocolloEntrata).mittente.idRegistro;
                    if (!string.IsNullOrEmpty(idRegistro))
                        reg = docsPaWS.GetRegistroBySistemId(idRegistro);
                    else
                        reg = docsPaWS.GetRegistroBySistemId(schedaDoc.registro.systemId);
                
                    docsPaWS.ProtocolloInvioNotificaAnnulla(schedaDoc.systemId, reg);
                }
                catch (System.Exception ex)
                {

                }
            }
        }

        public static SchedaDocumento UpdateDocument(Page page, SchedaDocumento schedaDoc, bool enableUffRef, out bool daAggiornareUffRef)
        {
            daAggiornareUffRef = false;

            try
            {
                //ABBATANGELI GIANLUIGI - Se non trovo la chiave di configurazione CODICE_APPLICAZIONE, imposto il codice applicazione di default a DOC
                schedaDoc.codiceApplicazione = (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]) ? "___" : System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]);

                InfoUtente infoUtente = UserManager.GetInfoUser();
                SchedaDocumento result = docsPaWS.DocumentoSaveDocumento(UserManager.GetSelectedRole(), infoUtente, schedaDoc, enableUffRef, out daAggiornareUffRef);

                if (result == null)
                {
                    string message = "In questo momento non è stato possibile creare il documento. Si prega di ripetere  l'operazione.";
                    throw new Exception(message);
                }

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static StoriaDirittoDocumento[] GetVisibilityHistory(string idProfile, string tipoObj, DocsPaWR.InfoUtente infouser)
        {
            StoriaDirittoDocumento[] HistoryList = null;
            try
            {
                HistoryList = docsPaWS.DocumentoGetStoriaVisibilita(idProfile, tipoObj, infouser);
                return HistoryList;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.TipologiaAtto[] GetCustomDocumentsLite(string idAdministration, string idGroup, string rights)
        {
            try
            {
                DocsPaWR.TipologiaAtto[] listaTipologiaAtto = docsPaWS.getTipoAttoPDInsRic(idAdministration, idGroup, rights);
                return listaTipologiaAtto;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.TipologiaAtto[] GetCustomDocumentsLiteWithStateDiagram(string idAdministration, string idGroup, string rights)
        {
            try
            {
                DocsPaWR.TipologiaAtto[] listaTipologiaAtto = docsPaWS.getTipoAttoPDInsRicWithStateDiagram(idAdministration, idGroup, rights);
                return listaTipologiaAtto;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.TipologiaAtto[] GetListCustomDocuments()
        {
            try
            {
                DocsPaWR.TipologiaAtto[] listaTipologiaAtto = docsPaWS.DocumentoGetTipologiaAtto();
                return listaTipologiaAtto;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.MezzoSpedizione[] GetMeansSending(string idAdm)
        {
            try
            {
                DocsPaWR.MezzoSpedizione[] result = docsPaWS.AmmListaMezzoSpedizione(idAdm, false);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocumentoDiritto[] getListaVisibilitaSemplificata(Page page, string idProfile, bool cercaRimossi)
        {
            DocumentoDiritto[] listaVisibilita = null;
            try
            {
                listaVisibilita = docsPaWS.DocumentoGetVisibilitaSemplificata(UserManager.GetInfoUser(), idProfile, cercaRimossi);
                return listaVisibilita;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.MezzoSpedizione[] GetMeansSendingInSession()
        {
            try
            {
                return (DocsPaWR.MezzoSpedizione[])GetSessionValue("meansSending");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void SetMeansSendingInSession(DocsPaWR.MezzoSpedizione[] meansSending)
        {
            try
            {
                SetSessionValue("meansSending", meansSending);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static bool dirittoProprietario(string systemId, DocsPaWR.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.DirittoProprietario(systemId, infoUtente);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        //Metodo che consente la ricerca oltre che per descrizione anche per codice sull'oggettario
        public static Oggetto[] getListaOggettiByCod(string[] idRegistri, string queryDescrizione, string queryCodice)
        {
            try
            {
                Oggetto[] result = null;

                //Tolgo i caratteri speciali dal campo descrizione oggetto
                queryDescrizione = queryDescrizione.Replace(System.Environment.NewLine, "");

                result = docsPaWS.DocumentoGetListaOggetti(getQueryOggetto(idRegistri, queryDescrizione, queryCodice));

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

        protected static DocumentoQueryOggetto getQueryOggetto(string[] registri, string queryDescrizione, string queryCodice)
        {
            try
            {
                DocumentoQueryOggetto query = new DocumentoQueryOggetto();
                Utente utente = UIManager.UserManager.GetUserInSession();
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione per il reperimento delle etichette custom definite per l'amministrazione
        /// </summary>
        /// <param name="page">La pagina chiamante</param>
        /// <returns>Lista delle etichette.</returns>
        public static List<EtichettaInfo> GetLettereProtocolli()
        {
            try
            {
                // Lista delle etichette custom
                List<EtichettaInfo> toReturn = GetDocumentLabelInSession();

                if (toReturn == null || toReturn.Count == 0)
                {
                    // Informazioni sull'utente loggato
                    InfoUtente userInfo = UIManager.UserManager.GetInfoUser();

                    // Reperimento delle etichette
                    EtichettaInfo[] tempLabel = docsPaWS.getEtichetteDocumenti(userInfo, userInfo.idAmministrazione);

                    List<EtichettaInfo> temp = new List<EtichettaInfo>(tempLabel);
                    temp.Add(new EtichettaInfo() { Etichetta = "R", Codice = "R" });
                    temp.Add(new EtichettaInfo() { Etichetta = "C", Codice = "C" });
                    // Restituzione delle label trovate

                    SetDocumentLabelInSession(temp);
                }
                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static List<EtichettaInfo> GetDocumentLabelInSession()
        {
            try
            {
                return (List<EtichettaInfo>)GetSessionValue("DocumentLabel");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void SetDocumentLabelInSession(List<EtichettaInfo> label)
        {
            try
            {
                SetSessionValue("DocumentLabel", label);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static string GetCodeLabel(string type)
        {
            try
            {
                string result = string.Empty;
                List<EtichettaInfo> label = GetLettereProtocolli();
                result = (string)(label.Where(f => f.Codice.Equals(type)).FirstOrDefault()).Descrizione;
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string GetDescriptionLabel(string type)
        {
            try
            {
                string result = string.Empty;
                List<EtichettaInfo> label = GetLettereProtocolli();
                result = (string)(label.Where(f => f.Codice.Equals(type)).FirstOrDefault()).Etichetta;
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool IsClassificationRqueredByTypeDoc(string type)
        {
            try
            {
                bool result = false;
                List<FascicolazioneTipiDocumento> list = GetFascicolazioneTipiDocumento();
                if (list != null && list.Count > 0)
                {
                    FascicolazioneTipiDocumento f = (from l in list where l.Codice.ToUpper().Equals(type.ToUpper()) select l).FirstOrDefault();
                    result = f != null ? f.FascicolazioneObbligatoria : false;
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        private static List<FascicolazioneTipiDocumento> GetFascicolazioneTipiDocumento()
        {
            try
            {
                // Lista delle etichette custom
                List<FascicolazioneTipiDocumento> toReturn = GetFascicolazioneTipiDocumentoInSession();

                if (toReturn == null || toReturn.Count == 0)
                {
                    InfoUtente userInfo = UIManager.UserManager.GetInfoUser();

                    toReturn = docsPaWS.GetFascicolazioneTipiDocumento(userInfo.idAmministrazione, userInfo).ToList();

                    SetFascicolazioneTipiDocumentoInSession(toReturn);
                }
                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static List<FascicolazioneTipiDocumento> GetFascicolazioneTipiDocumentoInSession()
        {
            try
            {
                return (List<FascicolazioneTipiDocumento>)GetSessionValue("FascicolazioneTipiDocumento");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void SetFascicolazioneTipiDocumentoInSession(List<FascicolazioneTipiDocumento> listTipiDoc)
        {
            try
            {
                SetSessionValue("FascicolazioneTipiDocumento", listTipiDoc);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
        public static bool inserisciMetodoSpedizione(InfoUtente info, string idDocumentTypes, string idProfile)
        {
            try
            {
                bool result = false;
                result = docsPaWS.collegaMezzoSpedizioneDocumento(info, idDocumentTypes, idProfile);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool getSeDocFascicolato(SchedaDocumento schedaDoc)
        {
            try
            {
                bool result = false;
                result = docsPaWS.DocumentoGetSeDocFascicolato(schedaDoc.systemId);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static DocsPaWR.ProtocolloDestinatario[] getDestinatariInteropAggConferma(string idProfile, Corrispondente corr)
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static SchedaDocumento creaDocumentoGrigio(Page page, SchedaDocumento schedaDoc)
        {
            SchedaDocumento result = null;
            try
            {
                schedaDoc.protocollo = null;
                schedaDoc.tipoProto = "G";
                schedaDoc.predisponiProtocollazione = false;
                schedaDoc.registro = null;
                schedaDoc.mezzoSpedizione = "0";
                schedaDoc.descMezzoSpedizione = "";

                InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
                schedaDoc = setFusionFields(page, schedaDoc, infoUtente);

                //ABBATANGELI GIANLUIGI - Gestione applicazioni esterne
                if (string.IsNullOrEmpty(schedaDoc.codiceApplicazione))
                    schedaDoc.codiceApplicazione = (string.IsNullOrEmpty(ConfigurationManager.AppSettings[Utils.WebConfigKeys.CODICE_APPLICAZIONE.ToString()]) ? "___" : ConfigurationManager.AppSettings[Utils.WebConfigKeys.CODICE_APPLICAZIONE.ToString()]);

                result = docsPaWS.DocumentoAddDocGrigia(schedaDoc, infoUtente, UIManager.RoleManager.GetRoleInSession());
                /*
                if (result == null)
                {
                    result = schedaDoc; //per ridare i dati precedentementi inseriti.
                    //string message = "Errore imprevisto dell'applicazione. Ritentare l'operazione.";
                    string message = "In questo momento non è stato possibile creare il documento. Si prega di ripetere  l'operazione.";

                    throw new Exception(message);

                }
                */
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        private static bool IsRicercaFullText(FiltroRicerca[][] objQueryList, out string textToSearch)
        {
            string oggetto = string.Empty;
            string testoContenuto = string.Empty;
            textToSearch = string.Empty; bool ricercaFullText = false;
            try
            {
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
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
                    InfoUtente infoUtente = UserManager.GetInfoUser();

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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            idProfilesList = idProfiles;
            return null;
        }

        public static InfoDocumento[] FullTextSearch(Page page, InfoUtente infoUtente, ref FullTextSearchContext context)
        {
            InfoDocumento[] retValue = null;

            try
            {
                retValue = docsPaWS.FullTextSearch(infoUtente, ref context);

                if (retValue == null)
                    throw new Exception();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return retValue;
        }

        public static DocsPaWR.AreaLavoro getListaAreaLavoro(Page page, string tipoDoc, string chaDaProto, int nPage, out int numRec, out int nRec, string idRegistro, DocsPaWR.FiltroRicerca[][] query)
        {
            DocsPaWR.AreaLavoro result = null;
            numRec = 0;
            nRec = 0;
            try
            {
                DocsPaWR.AreaLavoroTipoOggetto tipoObj = DocsPaWR.AreaLavoroTipoOggetto.DOCUMENTO;
                DocsPaWR.AreaLavoroTipoFascicolo tipoFasc = new DocsPaWR.AreaLavoroTipoFascicolo();
                DocsPaWR.AreaLavoroTipoDocumento tipoDocumento = new DocsPaWR.AreaLavoroTipoDocumento();

                Utente utente = UserManager.GetUserInSession();
                Ruolo ruolo = UserManager.GetSelectedRole();

                if (tipoDoc.Equals("A"))
                    tipoDocumento = DocsPaWR.AreaLavoroTipoDocumento.ARRIVO;
                else if (tipoDoc.Equals("P"))
                    tipoDocumento = DocsPaWR.AreaLavoroTipoDocumento.PARTENZA;
                else if (tipoDoc.Equals("I"))
                    tipoDocumento = DocsPaWR.AreaLavoroTipoDocumento.INTERNO;
                else if (tipoDoc.Equals("T"))
                    tipoDocumento = DocsPaWR.AreaLavoroTipoDocumento.TUTTI;
                else
                    tipoDocumento = DocsPaWR.AreaLavoroTipoDocumento.GRIGIO;

                bool enableUfficioRef = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ENABLE_UFFICIO_REF.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ENABLE_UFFICIO_REF.ToString()].Equals("1");

                result = docsPaWS.DocumentoGetAreaLavoroPaging1(utente, ruolo, enableUfficioRef, tipoObj, tipoDocumento, tipoFasc, chaDaProto, nPage, query, idRegistro, out numRec, out nRec);

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

        public static void setSelectedDocumentChain(string id)
        {
            try
            {
                RemoveSessionValue("DocumentChain");
                SetSessionValue("DocumentChain", id);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static string getSelectedDocumentChain()
        {
            try
            {
                return (string)GetSessionValue("DocumentChain");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static SchedaDocumento riproponiDatiRispIngresso(Page page, SchedaDocumento schedaDoc, DocsPaWR.Corrispondente destSelezionato)
        {
            try
            {
                //data una scheda doc in uscita ne viene creata una in ingresso
                //riproponendo l'oggetto e scegliendo il mittente tra i vari destinatari.
                SchedaDocumento schedaNewDoc = NewSchedaDocumento();
                schedaNewDoc.oggetto = schedaDoc.oggetto;
                schedaNewDoc.registro = schedaDoc.registro;
                schedaNewDoc.idPeople = schedaDoc.idPeople;
                schedaNewDoc.userId = schedaDoc.userId;
                schedaNewDoc.typeId = schedaDoc.typeId;
                schedaNewDoc.appId = schedaDoc.appId;
                schedaNewDoc.privato = "0";
                schedaNewDoc.tipoProto = "A";
                if (schedaDoc.tipoProto.Equals("P"))
                {
                    schedaNewDoc.protocollo = new DocsPaWR.ProtocolloEntrata();
                    ((DocsPaWR.ProtocolloEntrata)schedaNewDoc.protocollo).mittente = new DocsPaWR.Corrispondente();
                    ((DocsPaWR.ProtocolloEntrata)schedaNewDoc.protocollo).mittente = destSelezionato;
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void cambiaDocumentoPersonalePrivato(SchedaDocumento schedaDoc)
        {
            try
            {
                docsPaWS.DocumentoCambiaPersonalePrivato(schedaDoc.systemId, UIManager.RoleManager.GetRoleInSession().idGruppo);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return 0;
            }
            return numDocInRisposta;
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        public static SchedaDocumento salva(SchedaDocumento schedaDoc, bool enableUffRef, out bool daAggiornareUffRef)
        {
            daAggiornareUffRef = false;
            try
            {
                //ABBATANGELI GIANLUIGI - Se non trovo la chiave di configurazione CODICE_APPLICAZIONE, imposto il codice applicazione di default a DOC
                schedaDoc.codiceApplicazione = (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]) ? "___" : System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]);

                InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();

                SchedaDocumento result = docsPaWS.DocumentoSaveDocumento(UIManager.RoleManager.GetRoleInSession(), infoUtente, schedaDoc, enableUffRef, out daAggiornareUffRef);

                //Fine Celeste
                if (result == null)
                {
                    //string message = "Errore imprevisto dell'applicazione. Ritentare l'operazione.";
                    string message = "In questo momento non è stato possibile creare il documento. Si prega di ripetere  l'operazione.";

                    throw new Exception(message);
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        //rstituisce 
        // 1: se vi è un errore in caso d'inseriemnto
        // 2: se vi errore in caso di modifica
        // 3: se è attivo un processo di firma e non è in attesa di un passo di classificazione
        // 0: se è andato tutto a buon fine
        public static int fascicolaRapida(string idProfile, string docnumber, string segnatura, Fascicolo fascicolo, bool ok)
        {
            try
            {
                InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
                string msg = string.Empty;
                bool result = false;
                if (fascicolo != null && fascicolo.folderSelezionato != null)
                {
                    result = docsPaWS.FascicolazioneAddDocFolder(infoUtente, idProfile, fascicolo.folderSelezionato, fascicolo.descrizione, out msg);
                    if (result)
                        return 0;
                    else
                    {
                        if (msg.Equals(ResultFascicolazione.DOCUMENTO_IN_LIBRO_FIRMA_PASSO_NON_ATTESO.ToString()))
                            return 3;
                        return 1;
                    }
                }
                else
                {
                    result = docsPaWS.FascicolazioneAddDocFascicolo(infoUtente, idProfile, fascicolo, true, out msg);

                    if (result)
                        return 0;
                    else
                    {
                        if (msg.Equals(ResultFascicolazione.DOCUMENTO_IN_LIBRO_FIRMA_PASSO_NON_ATTESO.ToString()))
                            return 3;
                        return 2;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return -1;
            }
        }

        public static bool updateMetodoSpedizione(InfoUtente info, string oldDocumentTypes, string idDocumentTypes, string idProfile)
        {
            try
            {
                bool result = false;
                result = docsPaWS.updateMezzoSpedizioneDocumento(info, oldDocumentTypes, idDocumentTypes, idProfile);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool deleteMetodoSpedizione(InfoUtente info, string idProfile)
        {
            try
            {
                bool result = false;
                result = docsPaWS.deleteMezzoSpedizioneDocumento(info, idProfile);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool DO_UpdateVisibilita(DocsPaWR.SchedaDocumento scheda, DocsPaWR.Ruolo ruolo)
        {
            try
            {
                string serverName = Utils.utils.getHttpFullPath();
                if (scheda.destinatariModificati.Length != 0 || scheda.destinatariCCModificati.Length != 0)
                {
                    return docsPaWS.DO_TrasmettiDestinatariModificati(scheda, ruolo, serverName);
                }
                else
                {
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool cambiaFascPrimaria(Page page, string idProject, string idProfile)
        {
            InfoUtente infoUtente = UserManager.GetInfoUser();
            bool result = false;
            try
            {
                result = docsPaWS.CambiaFascicolazionePrimaria(infoUtente, idProject, idProfile);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        public static DocsPaWR.Fascicolo[] GetFascicoliDaDocNoSecurity(Page page, string idProfile)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                DocsPaWR.Fascicolo[] listaFascicoli = docsPaWS.FascicolazioneGetFascicoliDaDocNoSecurity(infoUtente, idProfile);
                return listaFascicoli;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.Fascicolo[] GetFascicoliDaDoc(Page page, string idProfile)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                DocsPaWR.Fascicolo[] listaFascicoli = docsPaWS.FascicolazioneGetFascicoliDaDoc(infoUtente, idProfile);

                return listaFascicoli;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void setDataGridFascicoliContenitori(Page page, DataTable dataGridFascicoliContenitori)
        {
            try
            {
                SetSessionValue("DataGridFascicoliContenitori", dataGridFascicoliContenitori);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static bool addDocumentoInFolder(Page page, string idProfile, string idFolder, bool fascRapida, out bool isInFolder, out String message)
        {
            message = String.Empty;
            isInFolder = false;
            bool result = false;

            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();

                //True: se il documento è già classificato nella folder indicata, false altrimenti
                isInFolder = docsPaWS.IsDocumentoClassificatoInFolder(idProfile, idFolder);

                if (!isInFolder)
                {
                    Folder fol = ProjectManager.getFolder(page, idFolder);
                    //se il doc non è già classificato nella folder indicata allora lo inserisco
                    string msg = string.Empty;
                    Fascicolo fasc = ProjectManager.getFascicoloById(page, fol.idFascicolo);
                    result = docsPaWS.FascicolazioneAddDocFolder(infoUtente, idProfile, fol, fasc.descrizione, out msg);
                    message = msg;
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
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
            message = string.Empty; ;

            try
            {
                if (!docsPaWS.TrasmettiProtocolloInterno(schedaDoc, RoleManager.GetRoleInSession(), serverName, isEnableUffRef, UIManager.UserManager.GetInfoUser(), out verificaRagioni, out message)) throw new Exception();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        /// <summary>
        /// Aggiornamento batch delle note
        /// </summary>
        /// <param name="oggettoAssociato"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public static InfoNota[] UpdateNote(AssociazioneNota oggettoAssociato, InfoNota[] note)
        {
            try
            {
                return docsPaWS.UpdateNote(UserManager.GetInfoUser(), oggettoAssociato, note);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
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

        #region Gestione session per la verifica firma digitale documento

        public static void SetSignedDocument(DocsPaWR.FileDocumento signedDocument)
        {
            SetSessionValue("SignedDocument", signedDocument);
        }

        public static DocsPaWR.FileDocumento GetSignedDocument()
        {
            return (DocsPaWR.FileDocumento)GetSessionValue("SignedDocument");
        }

        public static void RemoveSignedDocument()
        {
            RemoveSessionValue("SignedDocument");
        }

        #endregion

        public static int isDocInADL(string idProfile, Page page)
        {
            try
            {
                int retValue = 0;
                InfoUtente infoUtente = UserManager.GetInfoUser();
                retValue = docsPaWS.isDocInADL(idProfile, infoUtente.idPeople);
                return retValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return -1;
            }
        }

        public static List<DocsPaWR.ResultAddAreaLavoro> AddMassiveObjectInADL(List<DocsPaWR.WorkingArea> listAreaLavoro)
        {
            List<DocsPaWR.ResultAddAreaLavoro> results = new List<DocsPaWR.ResultAddAreaLavoro>();
            try
            {
                results = docsPaWS.AddMassiveObjectInADL(listAreaLavoro.ToArray(), UserManager.GetInfoUser()).ToList();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return results;
        }

        public static void eliminaDaAreaLavoro(Page page, string idProfile, Fascicolo fasc)
        {
            try
            {
                Utente utente = UserManager.GetUserInSession();
                Ruolo ruolo = RoleManager.GetRoleInSession();
                InfoUtente infoUtente = UserManager.GetInfoUser();
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void addAreaLavoro(Page page, SchedaDocumento schedaDocumento)
        {
            try
            {
                Utente utente = UserManager.GetUserInSession();
                Ruolo ruolo = RoleManager.GetRoleInSession();
                bool result = docsPaWS.DocumentoExecAddLavoro(getInfoDocumento(schedaDocumento).idProfile, getInfoDocumento(schedaDocumento).tipoProto, null, UserManager.GetInfoUser(), (schedaDocumento.registro != null ? schedaDocumento.registro.systemId : ""));
                Hashtable listInArea = new Hashtable();
                listInArea.Add(schedaDocumento.docNumber, schedaDocumento);
                page.Session["listInArea"] = listInArea;

                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void addAreaLavoro(HttpSessionState session, SchedaDocumento schedaDocumento)
        {
            try
            {
                Utente utente = UserManager.GetUserInSession();
                Ruolo ruolo = RoleManager.GetRoleInSession();
                bool result = docsPaWS.DocumentoExecAddLavoro(getInfoDocumento(schedaDocumento).idProfile, getInfoDocumento(schedaDocumento).tipoProto, null, UserManager.GetInfoUser(), (schedaDocumento.registro != null ? schedaDocumento.registro.systemId : ""));
                Hashtable listInArea = new Hashtable();
                listInArea.Add(schedaDocumento.docNumber, schedaDocumento);
                session["listInArea"] = listInArea;

                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static SchedaDocumento annullaPredisposizione(InfoUtente infoUtente, SchedaDocumento schedaDocumento)
        {
            try
            {
                if (docsPaWS.DocumentoAnnullaPredisposizione(infoUtente, schedaDocumento))
                {
                    schedaDocumento.protocollo = null;
                    schedaDocumento.tipoProto = "G";
                    schedaDocumento.predisponiProtocollazione = false;
                    schedaDocumento.registro = null;
                    schedaDocumento.mezzoSpedizione = "0";
                    schedaDocumento.descMezzoSpedizione = string.Empty;
                    //schedaDocumento.privato = "0";
                }
                else
                {
                    schedaDocumento = null;
                }
                return schedaDocumento;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void ConsolidateDocument(SchedaDocumento document, DocsPaWR.DocumentConsolidationStateEnum toState, InfoUtente infoUser)
        {
            try
            {
                // Contesto di esecuzione dell'azione di consolidamento nel dettaglio del documento
                DocsPaWR.DocumentConsolidationStateInfo info = docsPaWS.ConsolidateDocumentById(infoUser, document.systemId, toState);
                //document.ConsolidationState = info;
                SchedaDocumento docResult = document;
                docResult.ConsolidationState = info;
                DocumentManager.setSelectedRecord(docResult);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Verifica che l'utente possa cestinare il documento
        /// </summary>
        /// <param name="page"></param>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public static string verificaDirittiCestinaDocumento(Page page, DocsPaWR.SchedaDocumento schedaDocumento)
        {
            string result = "";
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                result = docsPaWS.VerificaDirittiCestinaDocumento(infoUtente, schedaDocumento);
                if (result.Equals(""))
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

        /// <summary>
        /// Cestina documenti grigi e predisposti
        /// </summary>
        /// <param name="page"></param>
        /// <param name="schedaDocumento"></param>
        /// <param name="tipoDoc"></param>
        /// <param name="note"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns> 
        public static bool CestinaDocumento(Page page, DocsPaWR.SchedaDocumento schedaDocumento, string tipoDoc, string note, out string errorMsg)
        {
            bool result = true;
            errorMsg = string.Empty;
            try
            {
                result = docsPaWS.DocumentoExecCestina(UserManager.GetInfoUser(), schedaDocumento, tipoDoc, note, out errorMsg);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public static DocsPaWR.Documento[] ListDocVersions
        {
            get
            {
                try
                {
                    DocsPaWR.Documento[] result = null;
                    if (HttpContext.Current.Session["listDocVersions"] != null)
                    {
                        result = HttpContext.Current.Session["listDocVersions"] as DocsPaWR.Documento[];
                    }
                    return result;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
            set
            {
                try
                {
                    HttpContext.Current.Session["listDocVersions"] = value;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsDocumentAnnul(string idProfile = "")
        {
            try
            {
                SchedaDocumento documentTab;
                if (!string.IsNullOrEmpty(idProfile))
                {
                    documentTab = getDocumentDetails(null, idProfile, idProfile);
                }
                else
                {
                    documentTab = getSelectedRecord();
                }
                if (documentTab!=null && documentTab.protocollo != null &&
                        documentTab.protocollo.protocolloAnnullato != null &&
                        !string.IsNullOrEmpty(documentTab.protocollo.protocolloAnnullato.dataAnnullamento))
                    return true;
                else
                    return false;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsNewDocument()
        {
            try
            {
                SchedaDocumento TabDocument = getSelectedRecord();
                if (TabDocument == null || string.IsNullOrEmpty(TabDocument.systemId))
                    return true;
                else
                    return false;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Funzione per verificare se il documento è bloccato
        /// </summary>
        /// <returns>True se il documento è bloccato</returns>
        public static bool IsDocumentCheckedOut()
        {
            bool retVal = false;
            try
            {
                DocsPaWR.FileRequest fileInfo = (UIManager.DocumentManager.getSelectedAttachId() != null) ?
                  UIManager.FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId()) :
                      UIManager.FileManager.GetFileRequest();

                if (fileInfo != null)
                {
                    retVal = CheckInOut.CheckInOutServices.IsCheckedOutDocument(fileInfo.docNumber, fileInfo.docNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord());
                }

                SchedaDocumento doc = DocumentManager.getSelectedRecord();
                if (!retVal && doc!=null)
                {
                    if (doc.checkOutStatus != null && !string.IsNullOrEmpty(doc.checkOutStatus.ID))
                    {
                        retVal = true;
                    }
                }
                return retVal;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return retVal;
            }
        }

        /// <summary>
        /// Verifica se il documento in sessione è in cestino
        /// </summary>
        /// <returns></returns>
        public static bool IsDocumentInBasket()
        {
            try
            {
                if (DocumentManager.getSelectedRecord() != null && !string.IsNullOrEmpty(DocumentManager.getSelectedRecord().inCestino) && DocumentManager.getSelectedRecord().inCestino.Equals("1"))
                    return true;
                else return false;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool IsDocumentInArchive()
        {
            try
            {
                if (DocumentManager.getSelectedRecord() != null && DocumentManager.getSelectedRecord().inArchivio != null && DocumentManager.getSelectedRecord().inArchivio.Equals("1"))
                    return true;
                else
                    return false;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool IsFatturaElettronica()
        {
            try
            {
                if (DocumentManager.getSelectedRecord() != null && DocumentManager.getSelectedRecord().tipologiaAtto != null 
                    && !string.IsNullOrEmpty(DocumentManager.getSelectedRecord().tipologiaAtto.descrizione)
                    && (DocumentManager.getSelectedRecord() .tipologiaAtto.descrizione.ToUpper().Equals("FATTURA ELETTRONICA") 
                    || DocumentManager.getSelectedRecord() .tipologiaAtto.descrizione.ToUpper().Equals("LOTTO DI FATTURE")))
                    return true;
                else return false;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool IsDocumentConsolidate()
        {
            try
            {
                if (DocumentManager.getSelectedRecord() != null &&
                    !string.IsNullOrEmpty(DocumentManager.getSelectedRecord().docNumber) &&
                    DocumentManager.getSelectedRecord().ConsolidationState != null &&
                    DocumentManager.getSelectedRecord().ConsolidationState.State != DocsPaWR.DocumentConsolidationStateEnum.None)
                    return true;
                else return false;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static DocsPaWR.DocumentoParolaChiave[] getListaParoleChiave(Page page)
        {
            try
            {
                DocsPaWR.DocumentoParolaChiave[] listaParoleChiave;
                InfoUtente infoUtente = UserManager.GetInfoUser();
                listaParoleChiave = docsPaWS.DocumentoGetParoleChiave(infoUtente.idAmministrazione);

                if (listaParoleChiave == null)
                {
                    throw new Exception();
                }
                return listaParoleChiave;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocumentoParolaChiave addParolaChiave(Page page, DocumentoParolaChiave parolaC)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                DocumentoParolaChiave result = docsPaWS.DocumentoAddParolaChiave(infoUtente.idAmministrazione, parolaC);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.DocumentoParolaChiave[] removeParoleChiave(DocsPaWR.DocumentoParolaChiave[] lista, int index)
        {
            try
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
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Restituisce il livello di accesso
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static string getAccessRightDocBySystemID(string idProfile, InfoUtente infoUtente)
        {
            string result = string.Empty;
            try
            {
                result = docsPaWS.getAccessRightDocBySystemID(idProfile, infoUtente);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        /// <summary>
        /// Restituisce il livello di accesso
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static string GetAccessRightDocByDocument(SchedaDocumento doc, InfoUtente infoUtente)
        {
            string result = string.Empty;
            try
            {
                if (doc != null && !string.IsNullOrEmpty(doc.systemId) && !string.IsNullOrEmpty(doc.accessRights))
                {
                   result = doc.accessRights;
                }
                else
                {
                    result = docsPaWS.getAccessRightDocBySystemID(doc.systemId, infoUtente);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }



        /// <summary>
        /// Modifica i diritti di un documento
        /// </summary>
        /// <param name="rigths">nuovi diritti da associare al doc</param>
        /// <param name="systemId">Id del doc per il quale desideriamo modificare i diritti</param>
        public static void RightsDocumentChanges(HMdiritti rights, string systemId)
        {
            try
            {
                docsPaWS.cambiaDirittiDocumenti(Convert.ToInt32(rights), systemId);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static DettItemsConservazione[] getStoriaConsDoc(string idProfile)
        {
            DettItemsConservazione[] dettItemsCons = null;
            try
            {
                dettItemsCons = docsPaWS.gettDettaglioItemsCons(idProfile);
                if (dettItemsCons == null)
                    throw new Exception();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return dettItemsCons;
        }

        public static DocsPaWR.TipologiaAtto[] getTipoAttoPDInsRic(Page page, string idAmministrazione, string idGruppo, string diritti)
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.TipologiaAtto[] getListaTipologiaAtto(Page page)
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static SchedaDocumento InoltraDocumento(Page page, SchedaDocumento schedaOLDDoc, bool eUffRef)
        {
            try
            {
                //data un documento grigio o un protocollo in ingresso 
                //crea un nuovo documento predisposto alla protocollazione in uscita
                //con lo stesso oggetto e con documento principale e allegati 
                //trasformati in allegati del nuovo documento
                SchedaDocumento schedaNewDoc = docsPaWS.DocumentoInoltraDoc(UserManager.GetInfoUser(), RoleManager.GetRoleInSession(), schedaOLDDoc);
                if (schedaNewDoc == null)
                {
                    string message = "Errore imprevisto dell'applicazione. Ritentare l'operazione.";
                    throw new Exception(message);
                }
                return schedaNewDoc;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static SchedaDocumento riproponiConCopiaDoc(Page page, SchedaDocumento schedaOLDDoc, bool eUffRef)
        {
            try
            {
                //data una scheda documento ne crea una nuova riproponendo l'oggetto e il mittente (o destinatario), il documento principale e gli allegati
                SchedaDocumento schedaNewDoc = docsPaWS.DocumentoRiproponiConCopiaDoc(UserManager.GetInfoUser(), RoleManager.GetRoleInSession(), schedaOLDDoc);
                if (schedaNewDoc == null)
                {
                    string message = "Errore imprevisto dell'applicazione. Ritentare l'operazione.";
                    throw new Exception(message);
                }
                return schedaNewDoc;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static SchedaDocumento CloneDocument(SchedaDocumento doc)
        {
            try
            {
                SchedaDocumento clone = doc;
                SchedaDocumento retVal = new SchedaDocumento();
                retVal = clone;
                return retVal;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.DocumentoStoricoOggetto[] getStoriaModifiche(Page page, DocsPaWR.SchedaDocumento schedaDocumento, string tipo)
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.DocumentoStoricoDataArrivo[] getStoriaModifiche(string docnumber)
        {
            try
            {
                return docsPaWS.DocumentoGetListaStoricoData(docnumber);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.DocumentoStoricoMittente[] getStoriaModifiche(DocsPaWR.SchedaDocumento schedaDocumento, string tipo)
        {
            try
            {
                return docsPaWS.DO_GetListaStoriciMittente(schedaDocumento.systemId, tipo);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// restituisce la storia del documento, del fascicolo, del folder
        /// </summary>
        /// <param name="idDocumento">system id del documento</param>
        /// <param name="TipoDocumento">tipologia dell  ricerca "DOCUMENTO, FASCICOLO, FOLDER"</param>
        /// <returns></returns>
        public static DocumentoLogDocumento[] getStoriaLog(string idDocumento, string TipoDocumento)
        {
            DocumentoLogDocumento[] result = null;
            try
            {
                result = docsPaWS.DocumentoGetListaLog(idDocumento, TipoDocumento);
                //gli eventi legati ad interazioni con applicazioni esterne devono essere filtrati
                //(FOLLOW_DOC_EXT_APP, FOLLOW_FASC_EXT_APP)
                if (result != null && result.Length > 0)
                {
                    result = (from l in result
                              where !l.codAzione.Equals("FOLLOW_DOC_EXT_APP") &&
                                  !l.codAzione.Equals("FOLLOW_FASC_EXT_APP")
                              select l).ToArray<DocumentoLogDocumento>();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        public static List<infoOggetto> GetLogAttiviByOggetto(string oggetto)
        {
            List<infoOggetto> result = new List<infoOggetto>();
            try
            {
                result = docsPaWS.GetLogAttiviByOggetto(oggetto, UserManager.GetInfoUser().idAmministrazione).ToList();
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        /// <summary>
        /// restituisce la storia del documento, del fascicolo, del folder
        /// </summary>
        /// <param name="idDocumento">system id del documento</param>
        /// <param name="TipoDocumento">tipologia dell  ricerca "DOCUMENTO, FASCICOLO, FOLDER"</param>
        /// <returns></returns>
        public static DocumentoLogDocumento[] getStoriaLog(string idDocumento, string idFolder, string TipoDocumento, FilterVisibility[] filter)
        {
            DocumentoLogDocumento[] result = null;
            try
            {
                result = docsPaWS.DocumentoGetListaLogFilter(idDocumento, idFolder, TipoDocumento, filter);
                //gli eventi legati ad interazioni con applicazioni esterne devono essere filtrati
                //(FOLLOW_DOC_EXT_APP, FOLLOW_FASC_EXT_APP)
                if (result != null && result.Length > 0)
                {
                    result = (from l in result
                              where !l.codAzione.Equals("FOLLOW_DOC_EXT_APP") &&
                                  !l.codAzione.Equals("FOLLOW_FASC_EXT_APP")
                              select l).ToArray<DocumentoLogDocumento>();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return retValue;
        }

        /// <summary>
        /// inserisce un documento in area di conservazione
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="idProject"></param>
        /// <param name="docNumber"></param>
        /// <param name="utente"></param>
        /// <param name="tipoOggetto"></param>
        /// <returns></returns>
        public static int addAreaConservazione(string idProfile, string idProject, string docNumber, InfoUtente utente, string tipoOggetto)
        {
            string result = "-1";
            try
            {
                result = docsPaWS.DocumentoExecAddConservazione(idProfile, idProject, docNumber, utente, tipoOggetto);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return -1;
            }
            return int.Parse(result);
        }

        public static SchedaDocumento getDettaglioDocumentoPerRiabilitazioneIstanza(string idProfile, string docNumber, out string errorMessage)
        {
            SchedaDocumento sd = new SchedaDocumento();
            errorMessage = string.Empty;
            try
            {
                if (idProfile == null && docNumber == null)
                {
                    sd = null;
                }
                else
                {
                    InfoUtente infoUtente = UserManager.GetInfoUser();
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }

            return sd;
        }

        public static bool DeleteValidateIstanzaConservazioneConPolicy(string idPolicy, string idConservazione, InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.DeleteValidateIstanzaConservazioneConPolicy(idPolicy, idConservazione, infoUtente);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return result;
            }
            return result;
        }

        /// <summary>
        /// veirifca se un documento si trova in area di conservaziones
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idProfile"></param>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public static bool canDeleteAreaConservazione(string idProfile, string idPeople, string idGruppo)
        {
            bool result = false;
            try
            {
                if (docsPaWS.CanDeleteFromItemCons(idProfile, idPeople, idGruppo) > 0)
                    result = true;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        public static bool eliminaDaAreaConservazione(string idProfile, Fascicolo fasc, string idIstanza, bool deleteIstanza, string systemId)
        {
            bool result = false;
            try
            {
                result = docsPaWS.DocumentoCancellaAreaConservazione(idProfile, fasc, idIstanza, deleteIstanza, systemId);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        /// <summary>
        /// Validazione istanza di conservazione
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static AreaConservazioneValidationResult validateIstanzaConservazione(string idConservazione)
        {
            AreaConservazioneValidationResult result = null;

            try
            {
                result = docsPaWS.ValidateIstanzaConservazione(idConservazione);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return result;
            }

            return result;
        }

        public static bool updateStatoAreaCons(string sysId, string tipo_cons, string note, string descr, string idTipoSupp, InfoUtente infoUtente, bool consolida)
        {
            bool result = true;
            try
            {
                result = docsPaWS.DocumentoUpdateAreaCons(sysId, tipo_cons, note, descr, idTipoSupp, infoUtente, consolida);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return result;
            }
            return result;
        }

        public static bool UpdatePreferredInstance(string idIstanza, InfoUtente infoUtente, Ruolo ruolo)
        {
            bool result = false;
            try
            {
                result = docsPaWS.UpdatePreferredInstance(idIstanza, infoUtente, ruolo);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return result;
            }
            return result;
        }

        public static bool ValidateIstanzaConservazioneConPolicy(string idPolicy, string idConservazione, InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.ValidateIstanzaConservazioneConPolicy(idPolicy, idConservazione, infoUtente);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return result;
            }
            return result;
        }

        public static bool EliminaDocumentiNonConformiPolicyDaIstanza(string idPolicy, string idConservazione, InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.EliminaDocumentiNonConformiPolicyDaIstanza(idPolicy, idConservazione, infoUtente);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return result;
            }
            return result;
        }

        public static Policy[] GetListaPolicy(int idAmm, string tipo)
        {
            Policy[] result;
            try
            {
                result = docsPaWS.GetListaPolicy(idAmm, tipo);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        public static int getItemSize(SchedaDocumento schedaDoc, string idItem)
        {
            int size = 0;
            try
            {
                size = docsPaWS.SerializeScheda(schedaDoc, idItem);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return 0;
            }
            return size;
        }

        public static bool insertSizeInItemCons(string sysId, int size)
        {
            bool result = false;
            try
            {
                result = docsPaWS.UpdateSizeItemCons(sysId, size);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        public static bool updateItemsConservazione(string tipoFile, string numAllegati, string sysId)
        {
            bool result = false;
            try
            {
                result = docsPaWS.updateItemsCons(tipoFile, numAllegati, sysId);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        public static InfoConservazione[] getListaConservazioneByFiltro(string filtro)
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
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return cons;
        }

        public static TipoIstanzaConservazione[] GetTipologieIstanzeConservazione()
        {
            TipoIstanzaConservazione[] cons = null;
            try
            {
                cons = docsPaWS.GetTipologieIstanzeConservazione();
                if (cons == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return cons;
        }

        /// <summary>
        /// veirifica se si deve creare un'istanza di prima cconservazione
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public static int isPrimaConservazione(string idPeople, string idGruppo)
        {
            int result = 0;
            try
            {
                result = docsPaWS.DocumentoIsPrimaIstanzaCons(idPeople, idGruppo);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return 0;
            }
            return result;
        }

        //get storico campi profilati di un tipo documento
        public static DocsPaWR.StoricoProfilati[] getStoriaProfilatiAtto(Page page, DocsPaWR.Templates template, string doc_number, string idGroup)
        {
            try
            {
                DocsPaWR.StoricoProfilati[] result = docsPaWS.DocumentoGetListaStoricoProfilati(template.ID_TIPO_ATTO, doc_number, idGroup);

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

        public static Tab GetDocumentTab(string documentId, DocsPaWR.InfoUtente infoUser)
        {
            try
            {
                return docsPaWS.GetDocumentTab(documentId, infoUser);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string getSegnaturaRepertorio(string docnumber, string codiceAmm)
        {
            string segnaturaRepertorio = string.Empty;
            try
            {
                segnaturaRepertorio = docsPaWS.GetSegnaturaRepertorio(docnumber, codiceAmm);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return segnaturaRepertorio;
        }

        public static string getSegnaturaRepertorioNoHTML(string docnumber, string codiceAmm)
        {
            string segnaturaRepertorio = string.Empty;
            try
            {
                segnaturaRepertorio = docsPaWS.getSegnaturaRepertorioNoHTML(docnumber, codiceAmm);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return segnaturaRepertorio;
        }

        public static bool isDocumentInConservazione(Page page, string idProfile, string docNumber)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(idProfile) && !string.IsNullOrEmpty(docNumber))
                {
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    if (!string.IsNullOrEmpty(docsPaWS.DocumentoGetDettaglioDocumento(infoUtente, idProfile, docNumber).inConservazione))
                        result = true;
                }

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        public static string GetFascicolazionePrimaria(Page page, string idProfile)
        {
            InfoUtente infoUtente = UserManager.GetInfoUser();
            string descrizioneFasc = string.Empty;
            try
            {
                descrizioneFasc = docsPaWS.GetFascicolazionePrimaria(infoUtente, idProfile);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return descrizioneFasc;
        }

        public static bool DocumentoInvioRicevuta(Page page, DocsPaWR.SchedaDocumento schedaDocumento, DocsPaWR.Registro registro)
        {
            bool result = false;
            string errormessage = string.Empty;
            try
            {
                Ruolo ruolo = RoleManager.GetRoleInSession();
                InfoUtente infoUtente = UserManager.GetInfoUser();
                //se l'rf scelto per la spedizione è diverso da quello salvato in dpa_ass_doc_mail_interop in fase di scarico aggiorno la tabella ed effettuo la spedizione
                DataSet ds = MultiCasellaManager.GetAssDocAddress(DocumentManager.getSelectedRecord().docNumber);
                if (ds != null && ds.Tables["ass_doc_rf"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["ass_doc_rf"].Rows)
                    {
                        if (!row["registro"].ToString().Equals(registro.systemId) && (!string.IsNullOrEmpty(registro.email)) &&
                            (!schedaDocumento.interop.Equals("I")))
                            MultiCasellaManager.UpdateAssDocAddress(DocumentManager.getSelectedRecord().docNumber, registro.systemId, registro.email);
                    }
                }
                result = docsPaWS.ProtocolloInvioRicevutaDiRitorno(schedaDocumento, registro, ruolo, infoUtente, out errormessage);
                if (!result)
                {
                    throw new Exception();
                }
            }
            //catch (Exception es)
            //{
            //    if (errormessage == string.Empty)
            //        ErrorManager.OpenErrorPage(page, "Non è stato possibile inviare la ricevuta di ritorno. Riprovare più tardi.", "invio ricevuta di ritorno");
            //    else
            //        ErrorManager.OpenErrorPage(page, errormessage, "invio ricevuta di ritorno");
            //}
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        public static Templates getTemplateById(string idTemplate, InfoUtente infoutente)
        {
            Templates template = null;
            try
            {
                template = docsPaWS.getTemplateById(idTemplate);
                if (template != null && template.IPER_FASC_DOC.Equals("1"))
                    template = DocumentManager.getTemplateCampiComuniById(idTemplate, infoutente);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return template;
        }

        //Se la tipologia è di campi comuni (Iperdocumento) richiamo il metodo che mi restituisce il temmplate
        //affinchè vengano visualizzati solo i campi comuni sui quali si ha visibilità rispetto alle tipologie
        //di documento associate al ruolo

        /// <summary>
        ///Se la tipologia è di campi comuni (Iperdocumento) richiamo il metodo che mi restituisce il temmplate
        ///affinchè vengano visualizzati solo i campi comuni sui quali si ha visibilità rispetto alle tipologie
        ///di documento associate al ruolo
        /// </summary>
        /// <param name="idTemplate"></param>
        /// <param name="infoutente"></param>
        /// <returns></returns>
        public static Templates getTemplateCampiComuniById(string idTemplate, InfoUtente infoutente)
        {
            Templates template = null;
            try
            {
                template = docsPaWS.getTemplateCampiComuniById(infoutente, idTemplate);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return template;
        }

        public static AssDocFascRuoli[] getDirittiCampiTipologiaDoc(string idRuolo, string idTemplate)
        {
            AssDocFascRuoli[] result = null;
            try
            {
                result = docsPaWS.getDirittiCampiTipologiaDoc(idRuolo, idTemplate);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        public static CheckOutStatus GetCheckOutDocumentStatus(string idProfile)
        {
            try
            {
                return docsPaWS.GetCheckOutDocumentStatus(idProfile, idProfile, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.FileDocumento StampaRicevutaProtocolloPdf()
        {
            try
            {
                return docsPaWS.StampaRicevutaProtocolloPdf(UserManager.GetInfoUser(), DocumentManager.getSelectedRecord().systemId);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.FileDocumento StampaRicevutaProtocolloRtf()
        {
            try
            {
                return docsPaWS.StampaRicevutaProtocolloRtf(UserManager.GetInfoUser(), DocumentManager.getSelectedRecord());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
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
                            DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(page, fasc.idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                            codClassifica = gerClassifica[gerClassifica.Length - 1].codice;
                        }
                }
                return codClassifica;
            }
            //catch (System.Web.Services.Protocols.SoapException es)
            //{
            //    ErrorManager.redirect(page, es);
            //    return codClassifica;
            //}
            //catch (Exception)
            //{
            //    return codClassifica;
            //}
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string getFascicoloDoc(Page page, NttDataWA.DocsPaWR.InfoDocumento infoDocumento)
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
            //catch (System.Web.Services.Protocols.SoapException es)
            //{
            //    ErrorManager.redirect(page, es);
            //    return codFascicolo;
            //}
            //catch (Exception)
            //{
            //    return codFascicolo;
            //}
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// restituisce tutte le estensioni 
        /// </summary>
        /// <param name="infoutente"></param>
        /// <returns></returns>
        public static string[] getExtFileAcquisiti(InfoUtente infoutente)
        {
            ArrayList result = null;
            try
            {
                result = new ArrayList(docsPaWS.getListaExtFileAcquisiti(infoutente.idAmministrazione));
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return (string[])result.ToArray(typeof(string));
        }

        public static DocsPaWR.SearchObject[] getQueryInfoDocumentoPagingCustom(InfoUtente infoUtente, Page page, DocsPaWR.FiltroRicerca[][] query, int numPage, out int numTotPage, out int nRec, bool security, bool getIdProfilesList, bool gridPersonalization, int pageSize, bool export, Field[] visibleFieldsTemplate, String[] documentsSystemId, out SearchResultInfo[] idProfilesList)
        {
            // La lista dei system id dei documenti restituiti dalla ricerca
            SearchResultInfo[] idProfiles = null;

            nRec = 0;
            numTotPage = 0;
            try
            {
                DocsPaWR.SearchObject[] DocS = null;
                string textToSearch = string.Empty;

                //modifiche ricerca fullText 27/01/2014
                //if (!IsRicercaFullText(query, out textToSearch))
                //{
                    DocS = docsPaWS.DocumentoGetQueryDocumentoPagingCustom(infoUtente, query, numPage, security, pageSize, getIdProfilesList, gridPersonalization, export, visibleFieldsTemplate, documentsSystemId, out numTotPage, out nRec, out idProfiles);
                //}
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

                // Impostazione della lista dei sisyem id dei documento
                idProfilesList = idProfiles;
                return DocS;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            idProfilesList = idProfiles;
            return null;
        }

        /// <summary>
        /// Metodo per aggiornate il mittente dei documenti di un corrispondente creato da interop
        /// </summary>
        /// <param name="oldCorrId">Vecchio Id mittente</param>
        /// <param name="newCorrId">Nuovo Id mittente</param>
        /// <param name="rows">Righe aggiornate</param>
        /// <return></return>
        public static void UpdateDocArrivoFromInterop(string oldCorrId, string newCorrId)
        {
            try
            {

                docsPaWS.UpdateDocArrivoFromInterop(oldCorrId, newCorrId);
            }
            catch (Exception ex)
            {

                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Metodo per gestire il caso di salvataggio del mittente come occasionale nella maschera K1 
        /// </summary>

        public static void UpdateDocArrivoFromInteropOccasionale(string docId, string newCorrId)
        {
            try
            {

                docsPaWS.UpdateDocArrivoFromInteropOccasionale(docId, newCorrId);
            }
            catch (Exception e)
            {

                UIManager.AdministrationManager.DiagnosticError(e);
            }
        }

        public static SearchItemList ListsSaveSearch(string idPeople, string idGroup)
        {
            SearchItemList result = null;
            try
            {
                result = docsPaWS.ListaRicercheSalvate(Int32.Parse(idPeople), Int32.Parse(idGroup), string.Empty, false, "D");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        public static ArrayList getExtFileAcquisiti(Page page)
        {
            ArrayList result = null;
            try
            {
                result = new ArrayList(docsPaWS.getListaExtFileAcquisiti(UserManager.GetUserInSession().idAmministrazione));
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        //Aggiunta Tatiana 11/04/2013
        public static DocsPaWR.Templates[] getTipoAttoTrasfDeposito(Page page, string idAmministrazione, bool seRepertorio)
        {
            try
            {
                DocsPaWR.Templates[] listaTemplates;
                InfoUtente infoUtente = UserManager.GetInfoUser();

                listaTemplates = docsPaWS.getTemplatesArchivioDeposito(infoUtente, idAmministrazione, seRepertorio);
                if (listaTemplates == null)
                {
                    throw new Exception();
                }

                return listaTemplates;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        //end Aggiunta Tatiana 11/04/2013

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

        public static void setFiltroRicFasc(Page page, FiltroRicerca[][] filtroRicerca)
        {
            try
            {
                page.Session["ricDoc.listaFiltri"] = filtroRicerca;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static bool IsEnabledProfilazioneAllegati()
        {
            try
            {
                return docsPaWS.IsEnabledProfilazioneAllegati();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
           
        }


        /// <summary>
        /// Controlla se un documento è stato ricevuto da una casella di posta con impostazione per il mantenimento delle mail ricevute
        /// come pendenti
        /// Per gestione pendenti tramite PEC
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public static bool IsDocPecPendente(string idDocument)
        {
            bool retval = false;
            try
            {

                retval = docsPaWS.InteroperabilitaIsDocPecPendente(idDocument);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return retval;
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;

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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return itemsCons;
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }

        public static FiltroRicerca[][] getFiltroRicDoc()
        {
            return (FiltroRicerca[][])GetSessionValue("ricDoc.listaFiltri");
        }

        public static FiltroRicerca[][] getMemoriaFiltriRicDoc()
        {
            return (FiltroRicerca[][])GetSessionValue("MemoriaFiltriRicDoc");
        }

        public static FiltroRicerca[] getFiltroRicTrasm()
        {
            return (FiltroRicerca[])GetSessionValue("ricTrasm.listaFiltri");
        }

        public static void setFiltroRicTrasm(Page page, FiltroRicerca[] filtroRicerca)
        {
            SetSessionValue("ricTrasm.listaFiltri", filtroRicerca);
        }

        public static DocsPaWR.InfoDocumento GetInfoDocumento(string idProfile, string docNumber, Page page)
        {
            try
            {
                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                return docsPaWS.GetInfoDocumento(infoUtente, idProfile, docNumber);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
                return null;
            }
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

        public static int isDocInADLRole(string idProfile, Page page)
        {
            try
            {
                int retValue = 0;
                InfoUtente infoUtente = UserManager.GetInfoUser();
                retValue = docsPaWS.isDocInADLRole(idProfile, infoUtente.idCorrGlobali);
                return retValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return -1;
            }
        }

        public static void eliminaDaAreaLavoroRole(Page page, string idProfile, Fascicolo fasc)
        {
            try
            {
                Utente utente = UserManager.GetUserInSession();
                Ruolo ruolo = RoleManager.GetRoleInSession();
                InfoUtente infoUtente = UserManager.GetInfoUser();
                bool result = docsPaWS.DocumentoCancellaAreaLavoro("0", infoUtente.idCorrGlobali, idProfile, fasc);

                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void addAreaLavoroRole(Page page, SchedaDocumento schedaDocumento)
        {
            try
            {
                Utente utente = UserManager.GetUserInSession();
                Ruolo ruolo = RoleManager.GetRoleInSession();
                bool result = docsPaWS.DocumentoExecAddLavoroRole(getInfoDocumento(schedaDocumento).idProfile, getInfoDocumento(schedaDocumento).tipoProto, null, UserManager.GetInfoUser(), (schedaDocumento.registro != null ? schedaDocumento.registro.systemId : ""));
                Hashtable listInArea = new Hashtable();
                listInArea.Add(schedaDocumento.docNumber, schedaDocumento);
                page.Session["listInArea"] = listInArea;

                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void addWorkAreaRole(Page page, InfoDocumento infoDocumento)
        {
            try
            {
                Utente utente = UserManager.GetUserInSession();
                Ruolo ruolo = UserManager.GetSelectedRole();
                bool result = docsPaWS.DocumentoExecAddLavoroRole(infoDocumento.idProfile, infoDocumento.tipoProto, null, UserManager.GetInfoUser(), (infoDocumento.codRegistro != null ? infoDocumento.idRegistro : ""));

                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            // Restituzione della scheda del documento
            return schedaBase;

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


        internal static bool IsDocAnnullatoByIdProfile(string p)
        {
            return docsPaWS.IsDocAnnullatoByIdProfile(p);
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

            try
            {
                toReturn = docsPaWS.IsDocumentInFolderOrSubFolder(userInfo, idProfile, project);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            // Restituzione del risultato
            return toReturn;
        }

        public static bool FascicolaDocumentoAM(Page page, string idProfile, Fascicolo fascicolo, out string msg)
        {
            bool result = false;
            msg = string.Empty;

            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                result = docsPaWS.FascicolaDocumentoAM(infoUtente, idProfile, fascicolo, false, out msg);
                if (!result)
                {
                    //throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            return result;
        }


        public static List<ImportResult> RimuoviVersioniMassivo(List<MassiveOperationTarget> idProfiles, RemoveVersionType type, Page page)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                List<ImportResult> res = new List<ImportResult>();
                foreach (MassiveOperationTarget temp in idProfiles)
                {
                    ImportResult ir = docsPaWS.DocumentoRimuoviVersioniDaGrigioAM(temp.Id, infoUtente, type);
                    res.Add(ir);
                }
                return res;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        #region variabile sessione "classifSelezionata"
        //Lnr 15/05/2013
        public static DocsPaWR.FascicolazioneClassificazione getClassificazioneSelezionata(Page page)
        {
            return (DocsPaWR.FascicolazioneClassificazione)GetSessionValue("classifSelezionata");
        }

        public static void setClassificazioneSelezionata(Page page, DocsPaWR.FascicolazioneClassificazione classificazione)
        {
            SetSessionValue("classifSelezionata", classificazione);
        }

        public static void removeClassificazioneSelezionata(Page page)
        {
            RemoveSessionValue("classifSelezionata");
        }
        #endregion  

        public static void setDocumentoSelezionato(Page page, SchedaDocumento schedaDoc)
        {
            //Lnr 16/05/2013
            setDocumentoSelezionato(schedaDoc);
        }

        public static SchedaDocumento getDettaglioDocumentoNoDataVista(Page page, string idProfile, string docNumber)
        {
            DocsPaWR.SchedaDocumento sd = new DocsPaWR.SchedaDocumento();

            try
            {
                if (idProfile == null && docNumber == null)
                {
                    sd = null;
                }
                else
                {
                    InfoUtente infoUtente = UserManager.GetInfoUser();
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

                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            return sd;
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }

        public static bool editingACL(DocumentoDiritto docDiritto, string personOrGroup, InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.EditingACL(docDiritto, personOrGroup, infoUtente);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                result = false;
            }
            return result;
        }

        public static bool ripristinaACL(DocsPaWR.DocumentoDiritto docDiritto, string personOrGroup, DocsPaWR.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.RipristinaACL(docDiritto, personOrGroup, infoUtente);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                result = false;
            }
            return result;
        }

        public static FileDocumento DocumentoGetFile(DocsPaWR.FileRequest fileRequest)
        {
            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
            return docsPaWS.DocumentoGetFile(fileRequest, infoUtente);
        }

        public static DateTime GetDataRiferimentoValitaDocumento() {
            DocsPaWR.FileRequest fileRequest = null;
            if (DocumentManager.getSelectedAttachId() != null) // ho aggiunto il file ad un allegato
            {
                fileRequest = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId());
            }
            else // ho aggiunto il file al documento principale
            {
                fileRequest = FileManager.GetFileRequest();
            }

            return docsPaWS.GetDataRiferimentoValitaDocumento(fileRequest, UserManager.GetInfoUser());
        }

        public static FileDocumento VerificaValiditaFirmaAllaData(DateTime date)
        {
            DocsPaWR.FileRequest fileRequest = null;
            if (DocumentManager.getSelectedAttachId() != null) // ho aggiunto il file ad un allegato
            {
                fileRequest = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId());
            }
            else // ho aggiunto il file al documento principale
            {
                fileRequest = FileManager.GetFileRequest();
            }

            return docsPaWS.VerificaValiditaFirmaAllaData(fileRequest, UserManager.GetInfoUser(), date);
        }

        public static CertificateInfo CertificateGetInfo(CertificateInfo certificate)
        {
            return docsPaWS.CertificateGetInfo(certificate, UserManager.GetInfoUser());
        }

        public static CertificateInfo VerifyCertificateExpired(CertificateInfo certificate)
        {
            return docsPaWS.VerifyCertificateExpired(certificate, UserManager.GetInfoUser());
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

        /// <summary>
        /// Reperimento del dettaglio del documento
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static SchedaDocumento getDocumentListVersions(Page page, string idProfile, string docNumber)
        {
            DocsPaWR.SchedaDocumento sd = new DocsPaWR.SchedaDocumento();


            if (idProfile == null && docNumber == null)
            {
                sd = null;
            }
            else
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                sd = docsPaWS.GetDocumentListVersions(infoUtente, idProfile, docNumber);
                if ((sd == null))
                {
                    string errorMessage = string.Empty;

                    if (sd == null)
                    {
                        ////verificata ACL, ritorna semmai un msg in out errorMessage
                        //int rtn = verifyDocumentACL("D", idProfile, infoUtente, out errorMessage);
                        //if (rtn == -1)
                        errorMessage = "Attenzione, non è stato possibile recuperare i dati del documento richiesto.\\nConsultare il log per maggiori dettagli.";
                    }
                }
            }


            return sd;
        }

        /// <summary>
        /// Restituisce il livello di accesso
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static string getAccessRightDocByDocument(SchedaDocumento document, InfoUtente infoUtente)
        {
            string result = string.Empty;
            try
            {
                if (document != null && !string.IsNullOrEmpty(document.systemId) && !string.IsNullOrEmpty(document.accessRights))
                {
                    result = document.accessRights;
                }
                else
                {
                    result = docsPaWS.getAccessRightDocBySystemID(document.systemId, infoUtente);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        public static void SetDataVistaSP(string idDoc)
        {
            try
            {
                docsPaWS.SetDataVistaSP(UIManager.UserManager.GetInfoUser(), idDoc, "D");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        #region Timestamp
        /// <summary>
        /// Restituisce tutti i timestamp relativi alla specifica versione del documento
        /// </summary>
        /// <param name="richiesta"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static  List<TimestampDoc> getTimestampsDoc(InfoUtente infoUtente, FileRequest fileRequest)
        {
            try
            {
                if (fileRequest != null && fileRequest.fileSize != null && fileRequest.fileSize.Length > 0)
                {
                    return docsPaWS.getTimestampsDoc(infoUtente, fileRequest).ToList();
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }


        public static DocsPaWR.TipologiaAtto[] ARCHIVE_GetListaTipologiaAtto(string idAmministrazione)
        {
            try
            {
                DocsPaWR.TipologiaAtto[] listaTipologiaAtto;
                listaTipologiaAtto = docsPaWS.ARCHIVE_GetTipologiaAtto(idAmministrazione);

                if (listaTipologiaAtto == null)
                {
                    throw new Exception();
                }
                return listaTipologiaAtto;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
		
        /// <summary>
        /// Effettua una richiesta di marca temporale e verifica la validità della marca ottenuta prima di restituirla come output.
        /// </summary>
        /// <param name="richiesta"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static OutputResponseMarca executeAndSaveTSR(InfoUtente infoUtente, InputMarca richiesta, FileRequest fileRequest)
        {
            try
            {
                return docsPaWS.executeAndSaveTSR(infoUtente, richiesta, fileRequest);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Crea un TSD da un TSR e crea una nuova versione
        /// </summary>
        /// <param name="richiesta"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static FileRequest CreateTSDVersion(InfoUtente infoUtente, FileRequest fileRequest)
        {
            try
            {
                return docsPaWS.CreateTSDVersion(infoUtente, fileRequest);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        #endregion

        public static bool ereditaVisibilita(string idAmm, string idModello)
        {
            try
            {
                return docsPaWS.ereditaVisibilita(idAmm, idModello);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        #region Documents removed

        //Recupera i documenti nello stato "in cestino"
        public static InfoDocumento[] getDocInCestino(InfoUtente infoUtente, DocsPaWR.FiltroRicerca[][] filtriRicerca)
        {
            InfoDocumento[] listaDoc = null;
            try
            {
                if (filtriRicerca == null)
                    listaDoc = docsPaWS.DocumentoGetDocInCestino(infoUtente);
                else
                    listaDoc = docsPaWS.DocumentoGetDocInCestinoFiltro(infoUtente, filtriRicerca);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return listaDoc;
        }

        //Rimuove fisicamente un singolo documento
        public static bool RemoveDocument(InfoUtente infoUtente, DocsPaWR.InfoDocumento infoDoc)
        {
            bool result = false;
            try
            {
                result = docsPaWS.EliminaDoc(infoUtente, infoDoc);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        //Riattiva un documento (dalla stato in cestino allo stato attivo)
        public static bool RestoreDocument(InfoUtente infoUtente, InfoDocumento infoDoc)
        {
            bool result = false;
            try
            {
                result = docsPaWS.DocumentoRiattivaDoc(infoUtente, infoDoc);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        //Rimuove fisicamente tutta la lista (o una lista filtrata) dei documenti in cestino
        public static bool RemoveAllDocuments(out bool docInCestino, InfoUtente infoUtente, DocsPaWR.InfoDocumento[] ListaDoc)
        {
            bool result = false;
            docInCestino = false;
            try
            {
                result = docsPaWS.SvuotaCestino(infoUtente, ListaDoc, out docInCestino);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        #endregion

        #region Identity Card

        public static FileInformation GetFileInformation(FileRequest fileRequest, InfoUtente infoUtente)
        {
            FileInformation fileInfo = null;
            try
            {
                fileInfo = docsPaWS.getFileInformation(fileRequest, infoUtente);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fileInfo;
        }
        #endregion


        public static bool IsDocumentoRepertoriato(Templates template)
        {
            bool isRepertoriato = false;
            if(template != null && template.ELENCO_OGGETTI != null && template.ELENCO_OGGETTI.Length > 0)
            {
                OggettoCustom repertorio = (from oggetto in template.ELENCO_OGGETTI where (oggetto.REPERTORIO != null && oggetto.REPERTORIO.Equals("1")) select oggetto).FirstOrDefault();
                if (repertorio != null && !string.IsNullOrEmpty(repertorio.VALORE_DATABASE))
                {
                    isRepertoriato = true;
                }
            }
            return isRepertoriato;
        }

        public static List<SchedaDocumento> GetSchedaDocuments(List<string> idDocumentList, Page page)
        {
            List<SchedaDocumento> list = new List<SchedaDocumento>();
            try
            {
                foreach (string id in idDocumentList)
                { 
                    list.Add(getDocumentListVersions(page, id, id)); 
                }
            }
            catch(Exception e)
            {
                return null;
            }
            return list;
        }

        public static string GetLabelTipoDocumento(string idTipoDocumento)
        { 
            string retValue = string.Empty;

            try
            {
                retValue = docsPaWS.GetLabelTipoDocumento(idTipoDocumento);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            return retValue;
        }

        #region MEV 1.5 F02_01 Conservazione

        public static bool checkAndValidateIstanzaConservazione(string idConservazione)
        {
            bool result = false;
            try
            {
                result = docsPaWS.asyncCheckAndValidateIstanzaConservazione(idConservazione);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                result = false;
            }

            return result;
        }

        public static bool convertAndSendForConservation(string idConservazione, string note, string descrizione, string tipoCons, string idTipoSupp, bool consolida)
        {
            bool result = false;
            try
            {
                result = docsPaWS.asyncConvertAndSendForConservation(idConservazione, tipoCons, note, descrizione, idTipoSupp, consolida);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                result = false;
            }

            return result;
        }

        public static List<ReportFormatiConservazione> getDettaglioReportConservazioneByIdIstanzaCons(string idConservazione)
        {
            List<ReportFormatiConservazione> result ;
            try
            {
                result = docsPaWS.getDettaglioListReportFormatiConservazioneByIdCons(idConservazione).Cast<DocsPaWR.ReportFormatiConservazione>().ToList(); 
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                result = null;
            }

            return result;
        }

        public static FileDocumento createReport(FiltroRicerca[] filtriReport, DataSet dataSet,
                                string tipoRep, string titoloReport, string Sottotitolo, string reportKey, string contextName, InfoUtente infoUtente)
        {
            FileDocumento result;
            try
            {
                result = docsPaWS.createReport(filtriReport, dataSet, tipoRep, titoloReport, Sottotitolo, reportKey, contextName, infoUtente);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                result = null;
            }

            return result;
        }

        #endregion

        // MEV CS 1.5
        #region CS 1.5 - Requisito F03_01
        /// <summary>
        /// inserimento di documenti in Area conservazione con il controllo dei vincoli della dimensione istanze violato
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="idProject"></param>
        /// <param name="docNumber"></param>
        /// <param name="utente"></param>
        /// <param name="tipoOggetto"></param>
        /// <param name="numDocIstanzaViolato"></param>
        /// <param name="dimIstanzaViolato"></param>
        /// <returns></returns>
        public static int addAreaConservazione_WithConstraint(string idProfile, string idProject, string docNumber, InfoUtente utente, string tipoOggetto, bool numDocIstanzaViolato, bool dimIstanzaViolato, int vincoloDimIstanza, int vincoloNumDocIstanza, int sizeItem)
        {
            string result = "-1";
            try
            {
                result = docsPaWS.DocumentoExecAddConservazione_WithConstraints(idProfile, idProject, docNumber, utente, tipoOggetto, numDocIstanzaViolato, dimIstanzaViolato, vincoloDimIstanza, vincoloNumDocIstanza, sizeItem);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return -1;
            }
            return int.Parse(result);
        }

        /// <summary>
        /// inserimento Massivo di documenti in Area conservazione con il controllo dei vincoli della dimensione istanze violato
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idProfile"></param>
        /// <param name="idProject"></param>
        /// <param name="docNumber"></param>
        /// <param name="utente"></param>
        /// <param name="tipoOggetto"></param>
        /// <param name="numDocIstanzaViolato"></param>
        /// <param name="dimIstanzaViolato"></param>
        /// <returns></returns>
        public static string addAreaConservazioneAM_WithConstraint(Page page, string idProfile, string idProject, string docNumber, InfoUtente utente, string tipoOggetto, bool numDocIstanzaViolato, bool dimIstanzaViolato, int vincoloDimIstanza, int vincoloNumDocIstanza, int sizeItem)
        {
            string result = string.Empty;
            try
            {
                result = docsPaWS.DocumentoExecAddConservazioneAM_WithConstraints(idProfile, idProject, docNumber, utente, tipoOggetto, numDocIstanzaViolato, dimIstanzaViolato, vincoloDimIstanza, vincoloNumDocIstanza, sizeItem);
                if (result == "-1")
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }

        /// <summary>
        /// Metodo per recuperare la dimensione dell'xml dei metadati di un documento
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <returns></returns>
        public static int getItemSize(SchedaDocumento schedaDoc)
        {
            int size = 0;
            try
            {
                size = docsPaWS.SerializeSchedaDoc(schedaDoc);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return 0;
            }
            return size;
        }

        /// <summary>
        /// Metodo per il calcolo della dimensione totale di un documento, comprensivo di XML dei Metadati dimensione degli allegati
        /// </summary>
        /// <param name="selectedDocument"></param>
        /// <returns></returns>
        public static int GetTotalDocumentSize(SchedaDocumento selectedDocument)
        {
            int totalSize = 0;

            int sizeXmlDoc = DocumentManager.getItemSize(
                    selectedDocument);

            int sizeDoc = Convert.ToInt32(selectedDocument.documenti[0].fileSize);

            int sizeAll = 0;
            for (int i = 0; i < selectedDocument.allegati.Length; i++)
            {
                sizeAll = sizeAll + Convert.ToInt32(selectedDocument.allegati[i].fileSize);
            }
            totalSize = sizeAll + sizeDoc + sizeXmlDoc;

            return totalSize;
        }

        /// <summary>
        /// Get della dimensione massima in un'istanza di conservazione
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static int getDimensioneMassimaIstanze(string idAmm)
        {
            int Dim = 0;
            
            try
            {
                //Dim = docsPaWS.getDimensioneMassimaIstanze_MB(idAmm);
                Dim = docsPaWS.getDimensioneMassimaIstanze_Byte(idAmm);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                Dim = 0;
            }
            return Dim;
        }

        /// <summary>
        /// Get del numero massimo di documenti in un'istanza di conservazione
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static int getNumeroDocMassimoIstanze(string idAmm)
        {
            int NumDocMax = 0;
            try
            {
                NumDocMax = docsPaWS.getDimensioneMassimaIstanze_NumDoc(idAmm);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                NumDocMax = 0;
            }
            return NumDocMax;
        }

        /// <summary>
        /// Metodo per reperire la percentuale di tolleranza sulla dimensione delle istanze da inviare in conservazione
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static int getPercentualeTolleranzaDinesioneIstanze(string idAmm)
        {
            int perc = 0;

            try
            {
                perc = docsPaWS.getPercentualeTolleranzaDinesioneIstanze(idAmm);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                perc = 0;
            }
            return perc;
        }

        /// <summary>
        /// Indica se il vincolo sul numero di documenti ammessi per l'istanza che si vuole costriure viene violato
        /// </summary>
        /// <param name="numDocIstanza">Numero corrente di documenti che si vogliono inserire nell'istanza di conservazione</param>
        /// <param name="vincoloNumDocIstanza">Numero di documenti massimi consentiti in un'istanza di conservazione</param>
        /// <returns></returns>
        public static bool isVincoloNumeroDocumentiIstanzaViolato(int numDocIstanza, int vincoloNumDocIstanza)
        {
            bool retVal = false;
            try
            {
                retVal = docsPaWS.isVincoloNumeroDocumentiIstanzaViolato(numDocIstanza, vincoloNumDocIstanza);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                retVal = false;
            }
            return retVal;
        }

        /// <summary>
        /// Indica se il vincolo sulla dimensione istanza che si vuole costriure viene violato
        /// </summary>
        /// <param name="DimensioneInIstanza">Dimensione raggiunta per il documento i-esimo</param>
        /// <param name="vincoloDimensioneInIStanza">Vincolo di dimensione massima ammessa per l'amministrazione</param>
        /// <param name="percentualeTolleranza">percentuale di tolleranza ammessa per l'amministrazione</param>
        /// <returns></returns>
        public static bool isVincoloDimensioneIstanzaViolato(int DimensioneInIstanza, int vincoloDimensioneInIStanza, int percentualeTolleranza)
        {
            bool retVal = false;
            try
            {
                retVal = docsPaWS.isVincoloDimensioneIstanzaViolato(DimensioneInIstanza, vincoloDimensioneInIStanza, percentualeTolleranza);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                retVal = false;
            }
            return retVal;
        }

        /// <summary>
        /// Metodo per get Dimensione corrente dell'istanza di conservazione
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static int getDimensioneCorrenteIstanzaByte_byIdIstanza(string idIstanza) 
        {
            int dim = 0;

            try 
            {
                dim = docsPaWS.getDimensioneCorrenteIstanzaByte_ByIDIstanza(idIstanza);
            }
            catch (System.Exception ex) 
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                dim = 0;
            }

            return dim;
        }

        /// <summary>
        /// Metodo per get numero documenti corrente dell'istanza di conservazione
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static int getNumeroDocIstanza_byIDIstanza(string idIstanza)
        {
            int numDoc = 0;

            try
            {
                numDoc = docsPaWS.getNumeroDocIstanza_ByIDIstanza(idIstanza);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                numDoc = 0;
            }

            return numDoc;
        }
        
        #endregion

        #region Versamento PARER

        public static bool checkConsolidamentoDoc(ArrayList list, InfoUtente infoUt)
        {

            bool result = false;

            try
            {
                result = docsPaWS.checkConsolidation(list.ToArray(), infoUt);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                result = false;
            }

            return result;
        }

        public static string getStatoConservazioneDoc(string idProfile)
        {

            string result = string.Empty;

            try
            {
                result = docsPaWS.getStatoConservazione(idProfile);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                result = null;
            }

            return result;
        }

        public static string AddDocToQueueCons(string idProfile, InfoUtente utente)
        {

            string result = string.Empty;

            try
            {
                result = docsPaWS.InsertDocInQueueCons(idProfile, utente);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                result = string.Empty;
            }

            return result;
        }

        public static string RecuperoStatoConservazione(string idProfile, InfoUtente utente)
        {

            string result = string.Empty;

            try
            {
                result = docsPaWS.recuperoStatoCons(idProfile, utente);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                result = string.Empty;
            }

            return result;
        }

        public static string GetRapportoVersamento(string idProfile, InfoUtente utente)
        {
            string result = string.Empty;

            try
            {
                result = docsPaWS.GetRapportoVersamento(idProfile, utente);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                result = string.Empty;
            }

            return result;
        }

        #endregion

        #region FIRMA ELETTRONICA

        public static List<FirmaElettronica> GetElectronicSignatureDocument(string docnumber, string versionId)
        {
            List<FirmaElettronica> listElectronicSignature = new List<FirmaElettronica>();
            try
            {
                listElectronicSignature = docsPaWS.GetElectronicSignatureDocument(docnumber, versionId, UserManager.GetInfoUser()).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
            return listElectronicSignature;
        }

        public static bool IsElectronicallySigned(string docnumber, string versionId)
        {
            bool result = false;
            try
            {
                result = docsPaWS.IsElectronicallySigned(docnumber, versionId);
            }
            catch (Exception e)
            {
                return result;
            }
            return result;
        }

        #endregion

        public static bool CheckDocumentIsSent(string idDocument)
        {
            try
            {
                return docsPaWS.CheckDocumentIsSent(idDocument);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        #region Integrazione RGS
        public static List<Flusso> GetListaFlussoDocumento(SchedaDocumento schedaDoc)
        {
            try
            {
                return docsPaWS.GetListaFlussoDocumento(schedaDoc, UserManager.GetInfoUser()).ToList();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        #endregion

        public static string GetTipoFirmaDocumento(string docnumber)
        {
            try
            {
                return docsPaWS.GetTipoFirmaDocumento(docnumber);
            }
            catch (System.Exception ex)
            {
                return NttDataWA.Utils.TipoFirma.NESSUNA_FIRMA;
            }
        }

        public static bool IsDocumentoInLibroFirma(SchedaDocumento schedaDoc)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(schedaDoc.systemId) && schedaDoc.documenti != null && schedaDoc.documenti.Count() > 0 && schedaDoc.documenti[0].inLibroFirma)
                result = true;

            return result;
        }

        public static bool ExistsTrasmPendenteConWorkflowDocumento(string idProfile, string idRuoloInUO, string idPeople)
        {
            bool result = false;
            InfoUtente infoUtente = UserManager.GetInfoUser();
            try
            {
                return docsPaWS.ExistsTrasmPendenteConWorkflowDocumento(idProfile, idRuoloInUO, idPeople, infoUtente);
            }
            catch(Exception e)
            {

            }

            return result;
        }

        public static bool ExistsTrasmPendenteSenzaWorkflowDocumento(string idProfile, string idRuoloInUO, string idPeople)
        {
            bool result = false;
            InfoUtente infoUtente = UserManager.GetInfoUser();
            try
            {
                return docsPaWS.ExistsTrasmPendenteSenzaWorkflowDocumento(idProfile, idRuoloInUO, idPeople, infoUtente);
            }
            catch (Exception e)
            {

            }

            return result;
        }

        public static bool IsDocumentoConforme(string idProfile)
        {
            bool result = false;
            InfoUtente infoUtente = UserManager.GetInfoUser();
            try
            {
                return docsPaWS.IsDocumentoConforme(idProfile, infoUtente);
            }
            catch (Exception e)
            {
                result = false;
            }

            return result;
        }

        public static List<InfoFile> GetListInfoFileDocument(string idProfile)
        {
            List<InfoFile> result = null;
            InfoUtente infoUtente = UserManager.GetInfoUser();
            try
            {
                result =  docsPaWS.GetListInfoFileDocument(idProfile, infoUtente).ToList();
            }
            catch (Exception e)
            {
                result = null;
            }

            return result;
        }
    }
}
