using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Collections;
using VtDocsWS.WebServices;
using log4net;
using System.Data;
using System.Text;
using DocsPaVO.documento;
using BusinessLogic.Documenti;
using System.Globalization;

namespace VtDocsWS.Manager
{
    public class DocumentsAdvancedManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocumentsAdvancedManager));

        public static Services.DocumentsAdvanced.RemoveDocument.RemoveDocumentResponse RemoveDocument(Services.DocumentsAdvanced.RemoveDocument.RemoveDocumentRequest request)
        {
            Services.DocumentsAdvanced.RemoveDocument.RemoveDocumentResponse response = new Services.DocumentsAdvanced.RemoveDocument.RemoveDocumentResponse();
            try
            {
                response.Success = true;
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "RemoveDocument");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new PisException("REQUIRED_ID");
                }

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                try
                {
                    if (!string.IsNullOrEmpty(request.IdDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                    }
                }
                catch (Exception e)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                if (documento != null)
                {
                    if (string.IsNullOrEmpty(infoUtente.diSistema) || infoUtente.diSistema != "1")
                    {
                        if (!Utils.isRuoloAuthorized(infoUtente, "DO_PRO_RIMUOVI"))
                            throw new Exception("Ruolo non autorizzato alla rimozione di un documento");
                    }
                    string errorMessage = string.Empty;
                    BusinessLogic.Documenti.DocManager.CestinaDocumento(infoUtente, documento, null, request.RemovalNote, out errorMessage);
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        throw new Exception(errorMessage);
                    }
                    else
                    {
                        response.ResultMessage = "Cancellazione del documento " + request.IdDocument + " effettuata";
                    }
                }
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.DocumentsAdvanced.SendDocumentAdvanced.SendDocumentAdvancedResponse SendDocumentAdvanced(Services.DocumentsAdvanced.SendDocumentAdvanced.SendDocumentAdvancedRequest request)
        {
            Services.DocumentsAdvanced.SendDocumentAdvanced.SendDocumentAdvancedResponse response = new Services.DocumentsAdvanced.SendDocumentAdvanced.SendDocumentAdvancedResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizione = new DocsPaVO.Spedizione.SpedizioneDocumento();


                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "RemoveDocument");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente



                //Reperimento ruolo utente
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                if (ruolo == null)
                {
                    //Ruolo non trovato
                    throw new PisException("ROLE_NO_EXIST");
                }
                // Fine reperimento ruolo

                // Controllo documento
                if (string.IsNullOrEmpty(request.IdDocument) && string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(request.IdDocument) && !string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                try
                {
                    if (!string.IsNullOrEmpty(request.IdDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.Signature))
                        {
                            documento = BusinessLogic.Documenti.DocManager.GetDettaglioBySignature(request.Signature, infoUtente);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }
                // Fine controllo documento

                if (string.IsNullOrEmpty(request.CodeRegister) && string.IsNullOrEmpty(request.IdRegister))
                {
                    throw new PisException("REQUIRED_CODE_OR_ID_REGISTER");
                }

                if (!string.IsNullOrEmpty(request.CodeRegister) && !string.IsNullOrEmpty(request.IdRegister))
                {
                    throw new PisException("REQUIRED_ONLY_CODE_OR_ID_REGISTER");
                }


                // Controllo Registro e mail
                infoSpedizione = Utils.GetInfoSpedizione(documento, infoUtente);
                DocsPaVO.utente.Registro registro = null;

                if (!string.IsNullOrEmpty(request.CodeRegister))
                {
                    registro = BusinessLogic.Utenti.RegistriManager.getRegistroByCodAOO(request.CodeRegister, infoUtente.idAmministrazione);
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.IdRegister))
                    {
                        try
                        {
                            registro = BusinessLogic.Utenti.RegistriManager.getRegistro(request.IdRegister);
                        }
                        catch
                        {
                            //Registro non trovato
                            throw new PisException("REGISTER_NOT_FOUND");
                        }
                    }
                }
                infoSpedizione.IdRegistroRfMittente = registro.systemId;
                string mailMittente = "";
                DataSet ds = BusinessLogic.Amministrazione.RegistroManager.GetRightRuoloMailRegistro(registro.systemId, ruolo.systemId);
                bool ruoloAbilitatoSpedizione = false;
                if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                    {
                        if (row["SPEDISCI"].ToString().Equals("1"))
                        {
                            //listRegistriRF.Add(registroX);
                            ruoloAbilitatoSpedizione = true;
                            break;
                        }
                    }
                }
                if (!ruoloAbilitatoSpedizione)
                {
                    throw new Exception("Il ruolo " + ruolo.codice + " non è abilitato alla spedizione su nessuna delle caselle del registro selezionato");
                }
                bool indirizzoPresente = false;
                ruoloAbilitatoSpedizione = false;
                if (!string.IsNullOrEmpty(request.SenderMail))
                {
                    foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                    {
                        if (row["EMAIL_REGISTRO"].ToString().ToUpper().Equals(request.SenderMail.ToUpper()))
                        {
                            //listRegistriRF.Add(registroX);
                            indirizzoPresente = true;
                            if (row["SPEDISCI"].ToString().Equals("1"))
                            {
                                ruoloAbilitatoSpedizione = true;
                                break;
                            }
                            break;
                        }
                    }

                    if (!indirizzoPresente)
                    {
                        throw new Exception("L'indirizzo " + request.SenderMail + " non è una casella configurata per il registro selezionato");
                    }
                    if (!ruoloAbilitatoSpedizione)
                        throw new Exception("Il ruolo " + ruolo.codice + " non è abilitato alla spedizione sulla casella email selezionata");
                    infoSpedizione.mailAddress = request.SenderMail;
                }
                else
                {
                    foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                    {
                        if (row["EMAIL_REGISTRO"].ToString().ToUpper().Equals(registro.email.ToUpper()))
                        {
                            //listRegistriRF.Add(registroX);
                            if (row["SPEDISCI"].ToString().Equals("1"))
                            {
                                ruoloAbilitatoSpedizione = true;
                                break;
                            }
                        }
                    }
                    if (!ruoloAbilitatoSpedizione) throw new Exception("Il ruolo " + ruolo.codice + " non è abilitato alla spedizione sulla casella principale del registro selezionato");
                    infoSpedizione.mailAddress = registro.email;
                }
                // Fine controllo registro e mail
                if (documento != null && !string.IsNullOrEmpty(documento.tipoProto) && (documento.tipoProto == "P"))
                {

                    //filtro dagli allegati da spedire quelli associati a notifiche di tipo PEC
                    ArrayList listAllegati = new ArrayList();
                    if (documento.allegati != null && documento.allegati.Count > 0)
                    {
                        foreach (DocsPaVO.documento.Allegato a in documento.allegati)
                        {
                            if (a.versionId != null)
                            {
                                if (!(BusinessLogic.Documenti.AllegatiManager.getIsAllegatoPEC(a.versionId)).Equals("1") &&
                                    !(BusinessLogic.Documenti.AllegatiManager.getIsAllegatoIS(a.versionId)).Equals("1"))
                                    listAllegati.Add(a);
                            }
                        }
                        documento.allegati = listAllegati;
                    }

                    if (request.Recipients == null || request.Recipients.Length < 1)
                    {

                        if (infoSpedizione != null)
                        {
                            if (infoSpedizione.DestinatariEsterni != null && infoSpedizione.DestinatariEsterni.Count > 0)
                            {
                                foreach (DocsPaVO.Spedizione.DestinatarioEsterno es in infoSpedizione.DestinatariEsterni)
                                {
                                    es.IncludiInSpedizione = true;
                                }
                            }

                            if (infoSpedizione.DestinatariInterni != null && infoSpedizione.DestinatariInterni.Count > 0)
                            {
                                foreach (DocsPaVO.Spedizione.DestinatarioInterno es in infoSpedizione.DestinatariInterni)
                                {
                                    es.IncludiInSpedizione = true;
                                }
                            }
                        }

                    }
                    else
                    {
                        foreach (Domain.Correspondent corrPIS in request.Recipients)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = null;

                            // Verifica se occasionale
                            if (corrPIS != null && !string.IsNullOrEmpty(corrPIS.CorrespondentType) && corrPIS.CorrespondentType.Equals("O"))
                            {
                                corrBySys = Utils.GetCorrespondentFromPis(corrPIS, infoUtente);
                            }
                            else
                            {
                                corrBySys = Utils.RisolviCorrispondente(corrPIS.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new PisException("RECIPIENT_NOT_FOUND");
                                }
                            }

                            if (infoSpedizione != null)
                            {
                                bool ctrlCorrDestinatario = false;
                                if (infoSpedizione.DestinatariEsterni != null && infoSpedizione.DestinatariEsterni.Count > 0)
                                {
                                    foreach (DocsPaVO.Spedizione.DestinatarioEsterno es in infoSpedizione.DestinatariEsterni)
                                    {
                                        if (es.DatiDestinatari[0].systemId == corrBySys.systemId)
                                        {
                                            es.IncludiInSpedizione = true;
                                            ctrlCorrDestinatario = true;
                                        }
                                    }
                                }

                                if (infoSpedizione.DestinatariInterni != null && infoSpedizione.DestinatariInterni.Count > 0)
                                {
                                    foreach (DocsPaVO.Spedizione.DestinatarioInterno es in infoSpedizione.DestinatariInterni)
                                    {
                                        if (es.DatiDestinatario.systemId == corrBySys.systemId)
                                        {
                                            es.IncludiInSpedizione = true;
                                            ctrlCorrDestinatario = true;
                                        }
                                    }
                                }
                                if (!ctrlCorrDestinatario)
                                    throw new Exception("Corrispondente " + corrBySys.descrizione + " non presente nei destinatari del protocollo.");
                            }
                        }

                    }

                    logger.Debug("infoSpedizione.IdRegistroRfMittente = " + infoSpedizione.IdRegistroRfMittente);
                    logger.Debug("infoSpedizione.mailAddress =" + infoSpedizione.mailAddress);
                    infoSpedizione = BusinessLogic.Spedizione.SpedizioneManager.SpedisciDocumento(infoUtente, documento, infoSpedizione);
                    response.Success = infoSpedizione.Spedito;
                    if (!response.Success)
                    {
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSPEDISCI", documento.systemId, "Spedizione documento " + documento.systemId, DocsPaVO.Logger.CodAzione.Esito.KO);
                        throw new PisException("SEND_DOCUMENT_FAILED");
                    }
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSPEDISCI", documento.systemId, "Spedizione documento " + documento.systemId, DocsPaVO.Logger.CodAzione.Esito.OK);

                    //List<string> EsitoPerCorr = new List<string>();
                    List<Domain.SendingResult> SendRess = new List<Domain.SendingResult>();
                    Domain.SendingResult sendRes = null;
                    foreach (DocsPaVO.Spedizione.DestinatarioEsterno corr in infoSpedizione.DestinatariEsterni)
                    {
                        if (corr.IncludiInSpedizione)
                        {
                            //DocsPaDB.Query_DocsPAWS.Interoperabilita interop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                            // Modifica per Occasionali.
                            string canalePref = (corr.DatiDestinatari[0].canalePref != null ? corr.DatiDestinatari[0].canalePref.descrizione : "MAIL");
                            //interop.InsertInStoricoSpedizioni(
                            //    documento.systemId,
                            //    corr.Id,
                            //    corr.StatoSpedizione.Descrizione,
                            //    corr.Email,
                            //    infoUtente.idGruppo,
                            //    canalePref,
                            //    (!string.IsNullOrEmpty(infoSpedizione.mailAddress) ? infoSpedizione.mailAddress : ""),
                            //    (!string.IsNullOrEmpty(infoSpedizione.IdRegistroRfMittente) ? infoSpedizione.IdRegistroRfMittente : "")
                            //    );
                            //EsitoPerCorr.Add(string.Format("ID {0} - Dest: {1} - Mail {2} - Stato: {3} - CanPref {4}", corr.Id, corr.DatiDestinatari[0].descrizione, corr.Email, corr.StatoSpedizione.Descrizione, corr.DatiDestinatari[0].canalePref.descrizione));
                            sendRes = new Domain.SendingResult();
                            sendRes.CorrespondentId = corr.Id;
                            sendRes.CorrespondentDescription = corr.DatiDestinatari[0].descrizione;
                            sendRes.Mail = corr.Email;
                            sendRes.PrefChannel = corr.DatiDestinatari[0].canalePref.descrizione;
                            sendRes.SendingRes = corr.StatoSpedizione.Descrizione;
                            logger.DebugFormat("ID {0} - Dest: {1} - Mail {2} - Stato: {3} - CanPref {4}", sendRes.CorrespondentId, sendRes.CorrespondentDescription, sendRes.Mail, sendRes.SendingRes, sendRes.PrefChannel);
                            SendRess.Add(sendRes);
                        }
                    }
                    //response.ResultRecipient = EsitoPerCorr.ToArray();
                    response.SendingResults = SendRess.ToArray();
                }
                else
                {
                    throw new Exception("Il documento non è un protocollo in partenza, non può essere spedito");
                }
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }


            return response;
        }

        public static Services.DocumentsAdvanced.FatturaEsitoNotifica.FatturaEsitoNotificaResponse FatturaEsitoNotifica(Services.DocumentsAdvanced.FatturaEsitoNotifica.FatturaEsitoNotificaRequest request)
        {
            Services.DocumentsAdvanced.FatturaEsitoNotifica.FatturaEsitoNotificaResponse response = new Services.DocumentsAdvanced.FatturaEsitoNotifica.FatturaEsitoNotificaResponse();
            try
            {
                response.Success = true;
                response.ResultMessage = "OK";
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "FatturaEsitoNotifica");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new PisException("REQUIRED_ID");
                }

                if (string.IsNullOrEmpty(request.EsitoNotifica))
                {
                    throw new Exception("Esito della notifica richiesto");
                }

                if (request.File == null)
                {
                    throw new Exception("File richiesto");
                }

                if (BusinessLogic.Amministrazione.SistemiEsterni.FattElEsitoNotifica(request.IdDocument, request.EsitoNotifica))
                {
                    DocsPaVO.documento.FileRequest allegato = new DocsPaVO.documento.Allegato
                    {
                        docNumber = request.IdDocument,
                        descrizione = request.File.Description
                    };
                    string erroreMessage = "";
                    allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, ((DocsPaVO.documento.Allegato)allegato));
                    DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                    {
                        name = request.File.Name,
                        fullName = request.File.Name,
                        content = request.File.Content,
                        contentType = request.File.MimeType,
                        length = request.File.Content.Length,
                        bypassFileContentValidation = true
                    };


                    //if (!BusinessLogic.Documenti.FileManager.putFile(ref allegato, fileDocumento, infoUtente, out erroreMessage))
                    if (!BusinessLogic.Documenti.FileManager.putFileBypassaControlli(ref allegato, fileDocumento, infoUtente, true, out erroreMessage))
                    {
                        throw new PisException("FILE_CREATION_ERROR");
                    }
                    else
                    {
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", request.IdDocument, "Aggiunto un nuovo allegato al documento " + request.IdDocument, DocsPaVO.Logger.CodAzione.Esito.OK);
                    }
                }
                else
                {
                    throw new Exception("Errore nella modifica dell'esito notifica. Controllare il parametro idDocument.");
                }

            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaResponse NuovaFattura(Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaRequest request)
        {
            Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaResponse response = new Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaResponse();
            try
            {
                response.Success = true;
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.documento.SchedaDocumento schedaDoc = null, schedaDocResult = null;
                DocsPaVO.documento.Allegato allegatoRapprFattura = null;
                //DocsPaVO.ProfilazioneDinamica.Templates template_temp = null;

                string passo = "", esito = "";
                int numPasso = 0;

                #region Autenticazione
                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "NuovaFattura");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente
                if (string.IsNullOrEmpty(request.CodeProject))
                {
                    throw new Exception("Codice del fascicolo richiesto");
                }

                if (request != null && string.IsNullOrEmpty(request.Document.DocumentType))
                {
                    throw new PisException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && string.IsNullOrEmpty(request.Document.Object))
                {
                    throw new PisException("MISSING_OBJECT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (!request.Document.DocumentType.ToUpper().Equals("A") && !request.Document.DocumentType.ToUpper().Equals("P") && !request.Document.DocumentType.ToUpper().Equals("I") && !request.Document.DocumentType.ToUpper().Equals("G")))
                {
                    throw new PisException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (request.Document.DocumentType.ToUpper().Equals("A") || request.Document.DocumentType.ToUpper().Equals("P") || request.Document.DocumentType.ToUpper().Equals("I")) && string.IsNullOrEmpty(request.CodeRegister))
                {
                    throw new PisException("REQUIRED_REGISTER");
                }
                #endregion
                #region Controllo presenza fattura
                if (!string.IsNullOrEmpty(request.Document.ConsolidationState))
                {
                    string idFatturaPresente = BusinessLogic.Amministrazione.SistemiEsterni.FattElCtrlDupl(request.Document.ConsolidationState, "", out passo, out esito);
                    if (!string.IsNullOrEmpty(idFatturaPresente))
                    {
                        logger.DebugFormat("Fattura presente con DocNum {0}. Passo {1}. Esito {2}", idFatturaPresente, passo, esito);
                        //throw new Exception(string.Format("Fattura con identificativo {0} già presente. Id PITre: {1}", request.Document.ConsolidationState, idFatturaPresente));
                        if (esito.ToUpper() == "OK")
                        {
                            numPasso = 15;
                            throw new Exception(string.Format("La fattura con IdInv {1} già presente con IdPiTre {0}.", idFatturaPresente, request.Document.ConsolidationState));
                        }
                        else if (esito.ToUpper() == "ERRORE")
                        {
                            schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idFatturaPresente, idFatturaPresente);
                            schedaDocResult = schedaDoc;

                            switch (passo.ToUpper())
                            {
                                case "CARICAMENTO FILE PRINCIPALE":
                                    numPasso = 8;
                                    break;
                                case "CREAZIONE DOCUMENTO IN RISPOSTA":
                                    numPasso = 9;
                                    break;
                                case "CARICAMENTO ALLEGATI":
                                    numPasso = 10;
                                    break;
                                case "INSERIMENTO IN FASCICOLO":
                                    numPasso = 11;
                                    break;
                                case "ASSOCIAZIONE TEMPLATE":
                                    numPasso = 12;
                                    break;
                                case "ESECUZIONE TRASMISSIONI":
                                    numPasso = 13;
                                    break;
                                default:
                                    numPasso = 8;
                                    break;
                            }
                        }

                    }
                }
                //else
                //{
                //    // Se assente non blocco la fattura
                //}
                #endregion
                #region Prelievo del titolario attivo
                string idTitolario = "";
                ArrayList titolari = new ArrayList();
                try
                {
                    titolari = BusinessLogic.Amministrazione.TitolarioManager.getTitolariUtilizzabili(infoUtente.idAmministrazione);
                }
                catch
                {
                    //Titolario attivo non trovato
                    throw new PisException("CLASSIFICATION_NOT_FOUND");
                }
                if (titolari != null && titolari.Count > 0)
                {
                    foreach (DocsPaVO.amministrazione.OrgTitolario tempTit in titolari)
                    {
                        if (tempTit.Stato == DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo)
                        {
                            idTitolario = tempTit.ID;
                            break;
                        }
                    }
                }
                #endregion
                #region Controllo del destinatario e della ragione della trasmissione
                if (request.TransmissionReceiver == null)
                {
                    //Destinatario non trovato
                    throw new PisException("REQUIRED_CORRESPONDENT");
                }
                if (string.IsNullOrEmpty(request.TransmissionReceiver.Code) && string.IsNullOrEmpty(request.TransmissionReceiver.Id))
                {
                    throw new PisException("REQUIRED_CODE_OR_ID_RECEIVER");
                }
                DocsPaVO.utente.Corrispondente corr = null;
                try
                {
                    if (!string.IsNullOrEmpty(request.TransmissionReceiver.Id))
                    {
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(request.TransmissionReceiver.Id);
                    }
                    else if (!string.IsNullOrEmpty(request.TransmissionReceiver.Code))
                    {
                        //corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(request.Receiver.Code, infoUtente);
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(request.TransmissionReceiver.Code, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                    }
                    if (corr == null)
                    {
                        throw new PisException("CORRESPONDENT_NOT_FOUND");
                    }

                }
                catch (Exception e)
                {
                    throw new PisException("CORRESPONDENT_NOT_FOUND");
                }

                DocsPaVO.trasmissione.RagioneTrasmissione ragione = null;
                if (request.TransmissionReason == null || string.IsNullOrEmpty(request.TransmissionReason))
                {
                    throw new PisException("REQUIRED_TRANSMISSION_REASON");
                }
                else if ((!string.IsNullOrEmpty(infoUtente.diSistema)) && (infoUtente.diSistema == "1") && (request.TransmissionReason != "COMPETENZA_SIST_ESTERNI"))
                {
                    throw new Exception("Ragione di trasmissione non valida per un sistema esterno. Ragione disponibile: COMPETENZA_SIST_ESTERNI");
                }
                try
                {
                    ragione = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, request.TransmissionReason.ToUpper());
                    if (ragione == null)
                        throw new PisException("TRANSMISSION_REASON_NOT_FOUND");
                }
                catch (Exception e)
                {
                    throw new PisException("TRANSMISSION_REASON_NOT_FOUND");
                }
                #endregion
                #region Controllo presenza fascicolo
                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();
                if (!string.IsNullOrEmpty(request.CodeProject))
                {
                    //Il codice è di un sottofascicolo
                    if (request.CodeProject.IndexOf("//") > -1)
                    {
                        string separatore = "//";
                        // MODIFICA per inserimento in sottocartelle
                        string[] separatoreAr = new string[] { separatore };

                        string[] pathCartelle = request.CodeProject.Split(separatoreAr, StringSplitOptions.RemoveEmptyEntries);

                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloDaCodice(infoUtente, pathCartelle[0], null, false, false);
                        if (fascicolo != null)
                        {
                            if (pathCartelle.Length > 1)
                            {
                                ArrayList cartelle = BusinessLogic.Fascicoli.FascicoloManager.getListaFolderDaIdFascicolo(infoUtente, fascicolo.systemID, null, false, false);
                                // caricamento folder base
                                string idParentFolder = fascicolo.systemID;
                                fascicolo.folderSelezionato = (DocsPaVO.fascicolazione.Folder)((from DocsPaVO.fascicolazione.Folder f in cartelle where f.idParent == idParentFolder select f).ToList().FirstOrDefault());
                                idParentFolder = fascicolo.folderSelezionato.systemID;
                                try
                                {
                                    for (int i = 1; i < pathCartelle.Length; i++)
                                    {

                                        fascicolo.folderSelezionato = (DocsPaVO.fascicolazione.Folder)((from DocsPaVO.fascicolazione.Folder f in cartelle where f.idParent == idParentFolder && f.descrizione == pathCartelle[i] select f).ToList().First());
                                        idParentFolder = fascicolo.folderSelezionato.systemID;
                                    }
                                }
                                catch (Exception e)
                                {
                                    throw new Exception("Cartella non trovata nel fascicolo");
                                }
                            }
                        }
                        else
                        {
                            throw new PisException("PROJECT_NOT_FOUND");
                        }


                    }
                    else
                    {
                        ArrayList fascicoli = new ArrayList();
                        fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliDaCodice(infoUtente, request.CodeProject, null, false, false, "I");
                        if (fascicoli != null && fascicoli.Count > 0)
                        {
                            if (fascicoli.Count == 1)
                            {
                                fascicolo = (DocsPaVO.fascicolazione.Fascicolo)fascicoli[0];
                            }
                            else
                            {
                                //Fascicoli multipli
                                throw new PisException("MULTIPLE");
                            }
                        }
                        else
                        {
                            //Fascicolo non trovato
                            throw new PisException("PROJECT_NOT_FOUND");
                        }
                    }
                }


                #endregion
                #region Creazione Scheda Documento
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                DocsPaVO.utente.Corrispondente mittente = new DocsPaVO.utente.Corrispondente();

                try
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                    if (ruolo == null)
                    {
                        //Ruolo non trovato
                        throw new PisException("ROLE_NO_EXIST");
                    }
                }
                catch
                {
                    //Ruolo non trovato
                    throw new PisException("ROLE_NO_EXIST");
                }

                if (numPasso < 8)
                {
                    // **************************************************************************
                    // Per accordi con il cliente viene effettuato questo controllo:
                    // se Type == "O" allore viene impostato il valore di CorrespondentType = "O"
                    // Controllo effettuato per: Sender, Recipients, RecipientsCC
                    // **************************************************************************
                    //
                    if (request.Document.Sender != null && !string.IsNullOrEmpty(request.Document.Sender.Type) && request.Document.Sender.Type.Equals("O"))
                        request.Document.Sender.CorrespondentType = "O";

                    if (request.Document.MultipleSenders != null)
                    {
                        for (int i = 0; i < request.Document.MultipleSenders.Length; i++)
                        {
                            Domain.Correspondent c = request.Document.MultipleSenders[i];
                            if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                                c.CorrespondentType = "O";
                        }
                    }

                    if (request.Document.Recipients != null)
                    {
                        for (int i = 0; i < request.Document.Recipients.Length; i++)
                        {
                            Domain.Correspondent c = request.Document.Recipients[i];
                            if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                                c.CorrespondentType = "O";
                        }
                    }

                    if (request.Document.RecipientsCC != null)
                    {
                        for (int i = 0; i < request.Document.RecipientsCC.Length; i++)
                        {
                            Domain.Correspondent c = request.Document.RecipientsCC[i];
                            if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                                c.CorrespondentType = "O";
                        }
                    }
                    //


                    ///Creazione schedaDocumento con i dati essenziali e obbligatori
                    schedaDoc = BusinessLogic.Documenti.DocManager.NewSchedaDocumento(infoUtente, false);

                    ///Imposto l'oggetto
                    if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.Object))
                    {
                        schedaDoc.oggetto.descrizione = request.Document.Object;
                        schedaDoc.oggetto.daAggiornare = true;
                    }

                    //Imposto le note, soltanto visibili a tutti
                    if (request != null && request.Document != null && request.Document != null && request.Document.Note != null && request.Document.Note.Length > 0)
                    {
                        schedaDoc.noteDocumento = new List<DocsPaVO.Note.InfoNota>();
                        foreach (Domain.Note nota in request.Document.Note)
                        {
                            DocsPaVO.Note.InfoNota tempNot = new DocsPaVO.Note.InfoNota();
                            tempNot.DaInserire = true;
                            tempNot.Testo = nota.Description;
                            tempNot.TipoVisibilita = DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti;
                            tempNot.UtenteCreatore = new DocsPaVO.Note.InfoUtenteCreatoreNota();
                            tempNot.UtenteCreatore.IdRuolo = infoUtente.idGruppo;
                            tempNot.UtenteCreatore.IdUtente = infoUtente.idPeople;
                            schedaDoc.noteDocumento.Add(tempNot);
                        }
                    }
                    //

                    ///Imposto il template
                    if (request != null && request.Document != null && request.Document.Template != null && (!string.IsNullOrEmpty(request.Document.Template.Id) || (!string.IsNullOrEmpty(request.Document.Template.Name))))
                    {
                        try
                        {

                            DocsPaVO.ProfilazioneDinamica.Templates template = null;
                            if (!string.IsNullOrEmpty(request.Document.Template.Id))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(request.Document.Template.Id);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(request.Document.Template.Name))
                                {
                                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(request.Document.Template.Name, infoUtente.idAmministrazione);
                                }
                            }

                            if (template != null)
                            {
                                //Modifica Lembo 10-01-2013: Controllo se l'utente è abilitato all'utilizzo del template.
                                //Solo ruoli con diritti di scrittura ammessi.
                                string controlFilter = string.Format("{0} and id_ruolo={1} and (diritti='2')", template.SYSTEM_ID.ToString(), infoUtente.idGruppo);
                                ArrayList controlloVisibilita = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getRuoliTipoDoc(controlFilter);
                                if (controlloVisibilita == null || controlloVisibilita.Count == 0)
                                {
                                    throw new PisException("TEMPLATE_NOT_ROLE_EDITABLE");
                                }
                                // Modifica Lembo 11-01-2013: Utilizzo il metodo che controlla la visibilità dei campi.
                                //schedaDoc.template = Utils.GetTemplateFromPis(request.Document.Template, template, false);

                                // Modifica Elaborazione XML da PIS req.2

                                Domain.File fileDaPassare = ((request.Document.MainDocument != null && request.Document.MainDocument.Content != null) ? request.Document.MainDocument : null);

                                schedaDoc.template = Utils.GetTemplateFromPisVisibility(request.Document.Template, template, false, infoUtente.idGruppo, "D", request.CodeApplication, infoUtente, fileDaPassare, request.CodeRegister, request.CodeRF, false, request.IdentificativoSdI);
                                schedaDoc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                                schedaDoc.tipologiaAtto.systemId = template.SYSTEM_ID.ToString();
                                schedaDoc.tipologiaAtto.descrizione = template.DESCRIZIONE.ToString();


                                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggcustom in schedaDoc.template.ELENCO_OGGETTI)
                                {
                                    if (oggcustom.CONTATORE_DA_FAR_SCATTARE == true)
                                        oggcustom.CONTATORE_DA_FAR_SCATTARE = false;
                                }


                                //foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggcustom in schedaDoc.template.ELENCO_OGGETTI)
                                //{
                                //    if (oggcustom.CONTATORE_DA_FAR_SCATTARE == true)
                                //        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTO_REPERTORIATO", schedaDoc.systemId, "Repertoriato documento", DocsPaVO.Logger.CodAzione.Esito.OK);
                                //}
                            }
                            else
                            {
                                //Template non trovato
                                throw new PisException("TEMPLATE_NOT_FOUND");
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Debug("Errore in ASSOCIAZIONE TEMPLATE: " + ex.Message);
                        }
                    }

                    ///Se privato
                    if (request != null && request.Document != null && request.Document.PrivateDocument)
                    {
                        schedaDoc.privato = "1";
                    }
                    ///

                    ///Se personale
                    if (request != null && request.Document != null && request.Document.PersonalDocument)
                    {
                        schedaDoc.personale = "1";
                    }
                    ///

                    DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione = new DocsPaVO.documento.ResultProtocollazione();
                    bool daAggiornareUffRef = false;

                    //CASO DOCUMENTO GRIGIO (NON PROTOCOLLATO)
                    if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("G"))
                    {
                        schedaDoc.tipoProto = "G";
                        schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                    }
                    //

                    //Controlli solo per protocolli
                    if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && !request.Document.DocumentType.ToUpper().Equals("G"))
                    {

                        if (request.Document.Sender == null || string.IsNullOrEmpty(request.Document.Sender.CorrespondentType))
                        {
                            if (!request.Document.DocumentType.ToUpper().Equals("P"))
                            {
                                //Mittente non presente
                                throw new PisException("REQUIRED_SENDER");
                            }
                            else
                            {
                                mittente = null;
                            }
                        }
                        else
                        {
                            if (request.Document.Sender != null && !string.IsNullOrEmpty(request.Document.Sender.CorrespondentType) && request.Document.Sender.CorrespondentType.Equals("O"))
                            {
                                mittente = Utils.GetCorrespondentFromPis(request.Document.Sender, infoUtente);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(request.Document.Sender.Id))
                                {
                                    mittente = Utils.RisolviCorrispondente(request.Document.Sender.Id, infoUtente);
                                    if (mittente == null)
                                        //Mittente non trovato
                                        throw new PisException("SENDER_NOT_FOUND");
                                }
                            }
                        }

                        if (request.Document.DocumentType.ToUpper().Equals("I") || request.Document.DocumentType.ToUpper().Equals("P"))
                        {
                            if (request.Document.Recipients == null || request.Document.Recipients.Length == 0)
                            {
                                //Destinatario non presente
                                throw new PisException("REQUIRED_RECIPIENT");
                            }
                        }

                        if (string.IsNullOrEmpty(request.CodeRegister))
                        {
                            //Registro mancante
                            throw new PisException("REQUIRED_REGISTER");
                        }
                        else
                        {
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeRegister);
                            if (reg == null)
                            {
                                //Registro mancante
                                throw new PisException("REGISTER_NOT_FOUND");
                            }
                            else
                            {
                                schedaDoc.registro = reg;
                            }
                        }

                        if (!string.IsNullOrEmpty(request.CodeRF))
                        {
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeRF);
                            if (reg != null)
                            {
                                schedaDoc.id_rf_prot = reg.systemId;
                                schedaDoc.id_rf_invio_ricevuta = reg.systemId;
                                schedaDoc.cod_rf_prot = reg.codRegistro;
                            }
                            else
                            {
                                //RF non trovato
                                throw new PisException("RF_NOT_FOUND");
                            }
                        }
                    }

                    //CASO PROTOCOLLO IN ARRIVO (ANCHE PREDISPOSTO)
                    if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("A"))
                    {
                        schedaDoc.tipoProto = "A";
                        schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloEntrata();

                        ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente = mittente;

                        if (request.Document.MultipleSenders != null && request.Document.MultipleSenders.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittenti = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.MultipleSenders)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = null;
                                if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                                {
                                    corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(corrTemp.Id))
                                    {
                                        corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                        if (corrBySys == null)
                                        {
                                            //Mittente non trovato
                                            throw new PisException("SENDER_NOT_FOUND");
                                        }
                                    }
                                }
                                ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittenti.Add(corrBySys);
                            }
                        }

                        //Data protocollo mittente
                        if (!string.IsNullOrEmpty(request.Document.DataProtocolSender))
                        {
                            ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).dataProtocolloMittente = request.Document.DataProtocolSender;
                        }

                        //Protocollo mittente
                        if (!string.IsNullOrEmpty(request.Document.ProtocolSender))
                        {
                            ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).descrizioneProtocolloMittente = request.Document.ProtocolSender;
                        }

                        //Data arrivo TODO
                        if (!string.IsNullOrEmpty(request.Document.ArrivalDate))
                        {
                            // Variabile Booleana per il controllo di validità della data passata in input;
                            bool valid = false;
                            // Data ritornata in output dal TryParseExact per la data arrivo passata in input
                            DateTime dateVal;

                            // Pattern di validità per una data valida
                            string pattern = "dd/MM/yyyy HH:mm";

                            try
                            {
                                /*
                                 *  Date Format Example: 01/03/2012
                                 *  dd = day (01)
                                 *  MM = month (03)
                                 *  yyyy = year (2012)
                                 *  / = date separator
                                 */

                                if (!DateTime.TryParseExact(request.Document.ArrivalDate, pattern, null, System.Globalization.DateTimeStyles.None, out dateVal))
                                {
                                    throw new Exception("Formato Data non corretto");
                                }

                                valid = true;
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Formato Data non corretto");
                            }

                            if (valid)
                            {
                                // Controllo che la data inserita non sia posteriore alla data odierna
                                DateTime dateVal2;
                                // Variabile Booleana per il controllo di validità della data DateTime.Now trasformato nel formato dd/MM/yyyy;
                                bool dateNow = true;

                                if (!DateTime.TryParseExact(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), pattern, null, System.Globalization.DateTimeStyles.None, out dateVal2))
                                {
                                    // Non dovrebbe entrare mai in questo controllo.
                                    dateNow = false;
                                    throw new Exception("Formato Data non corretto");
                                }

                                if (dateNow)
                                {
                                    // dateVal: data passata in input;
                                    // dateVal2: DateTime.Now convertito nel formato dd/MM/yyyy
                                    int result = DateTime.Compare(dateVal, dateVal2);

                                    if (result > 0)
                                        // Data di input è successiva alla data odierna;
                                        //((Documento)schedaDoc.documenti[0]).dataArrivo = string.Empty;
                                        throw new Exception("La data di arrivo non può essere posteriore alla data di segnatura");
                                    else
                                        // Data di input valida e conforme;
                                        ((DocsPaVO.documento.Documento)schedaDoc.documenti[0]).dataArrivo = request.Document.ArrivalDate;
                                }
                            }
                        }

                        if (request.Document.Predisposed)
                        {
                            schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                            if (schedaDocResult != null)
                            {
                                schedaDocResult.predisponiProtocollazione = true;
                                schedaDocResult.tipoProto = request.Document.DocumentType;
                                schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                            }
                        }
                        else
                        {
                            schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out risultatoProtocollazione);
                        }

                        if (schedaDocResult != null)
                        {
                            //Mezzo di spedizione
                            if (!string.IsNullOrEmpty(request.Document.MeansOfSending))
                            {
                                ArrayList mezziSpedizione = BusinessLogic.Amministrazione.MezziSpedizioneManager.ListaMezziSpedizione(infoUtente.idAmministrazione, true);
                                foreach (DocsPaVO.amministrazione.MezzoSpedizione m_spediz in mezziSpedizione)
                                {
                                    if (m_spediz.Descrizione.ToUpper().Equals(request.Document.MeansOfSending.ToUpper()))
                                    {
                                        BusinessLogic.Documenti.ProtoManager.collegaMezzoSpedizioneDocumento(infoUtente, m_spediz.IDSystem, schedaDocResult.systemId);
                                        break;
                                    }
                                }

                            }
                        }
                    }
                    //

                    //CASO PROTOCOLLO IN PARTENZA (ANCHE PREDISPOSTO)
                    if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("P"))
                    {
                        schedaDoc.tipoProto = "P";
                        schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloUscita();

                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente = mittente;

                        if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.Recipients)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = null;

                                // Verifica se occasionale
                                if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                                {
                                    corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                }
                                else
                                {
                                    // Se non è occasionale, lo risolve sulla rubrica
                                    corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                    if (corrBySys == null)
                                    {
                                        //Destinatario non trovato
                                        throw new PisException("RECIPIENT_NOT_FOUND");
                                    }

                                }
                                if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "F" && corrBySys.tipoIE == null)
                                {
                                    ArrayList corrByRF = BusinessLogic.Rubrica.RF.getCorrispondentiByCodRF(corrBySys.codiceRubrica);
                                    foreach (DocsPaVO.utente.Corrispondente c2 in corrByRF)
                                    {
                                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(c2);
                                    }
                                }
                                else if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                                {
                                    ArrayList corrByLista = BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiByCodLista(corrBySys.codiceRubrica, infoUtente.idAmministrazione, infoUtente);
                                    foreach (DocsPaVO.utente.Corrispondente c3 in corrByLista)
                                    {
                                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(c3);
                                    }
                                }
                                else
                                {
                                    ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(corrBySys);
                                }
                            }
                        }

                        if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.RecipientsCC)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = null;

                                // Verifica se occasionale
                                if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                                {
                                    corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                }
                                else
                                {
                                    corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                    if (corrBySys == null)
                                    {
                                        //Destinatario non trovato
                                        throw new PisException("RECIPIENT_NOT_FOUND");
                                    }
                                }
                                if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "F" && corrBySys.tipoIE == null)
                                {
                                    ArrayList corrByRF = BusinessLogic.Rubrica.RF.getCorrispondentiByCodRF(corrBySys.codiceRubrica);
                                    foreach (DocsPaVO.utente.Corrispondente c2 in corrByRF)
                                    {
                                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(c2);
                                    }
                                }
                                else if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                                {
                                    ArrayList corrByLista = BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiByCodLista(corrBySys.codiceRubrica, infoUtente.idAmministrazione, infoUtente);
                                    foreach (DocsPaVO.utente.Corrispondente c3 in corrByLista)
                                    {
                                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(c3);
                                    }
                                }
                                else
                                {
                                    ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
                                }

                                //((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
                            }
                        }

                        if (request.Document.Predisposed)
                        {
                            schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                            if (schedaDocResult != null)
                            {
                                schedaDocResult.predisponiProtocollazione = true;
                                schedaDocResult.tipoProto = request.Document.DocumentType;
                                schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                            }
                        }
                        else
                        {
                            schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out risultatoProtocollazione);
                        }
                    }
                    //

                    //CASO PROTOCOLLO INTERNO (ANCHE PREDISPOSTO)
                    if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("I"))
                    {
                        schedaDoc.tipoProto = "I";
                        schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloInterno();

                        ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).mittente = mittente;

                        if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatari = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.Recipients)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new PisException("RECIPIENT_NOT_FOUND");
                                }
                                ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatari.Add(corrBySys);
                            }
                        }

                        if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.RecipientsCC)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new PisException("RECIPIENT_NOT_FOUND");
                                }
                                ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
                            }
                        }

                        if (request.Document.Predisposed)
                        {
                            schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                            if (schedaDocResult != null)
                            {
                                schedaDocResult.predisponiProtocollazione = true;
                                schedaDocResult.tipoProto = request.Document.DocumentType;
                                schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                            }
                        }
                        else
                        {
                            schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out risultatoProtocollazione);
                        }
                    }
                    //

                    if (!string.IsNullOrEmpty(request.IdLottoPITre))
                    {
                        int testIdLotto = 0;
                        if (Int32.TryParse(request.IdLottoPITre, out testIdLotto))
                        {
                            BusinessLogic.Documenti.DocManager.UpdateRispostaProtocollo(request.IdLottoPITre, schedaDocResult.docNumber);
                        }
                        else
                        {
                            throw new Exception("Valore non valido per il campo IdLottoPI3");
                        }
                    }

                    //Log creazione documento
                    if (schedaDocResult != null)
                    {
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTO_CREATO", schedaDoc.systemId, "Creato documento", DocsPaVO.Logger.CodAzione.Esito.OK);
                    }


                    //Commentato, spostato sotto
                    //foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggcustom in schedaDoc.template.ELENCO_OGGETTI)
                    //{
                    //    if (oggcustom.CONTATORE_DA_FAR_SCATTARE == true && oggcustom.REPERTORIO == "1")
                    //        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTO_REPERTORIATO", schedaDoc.systemId, "Repertoriato documento", DocsPaVO.Logger.CodAzione.Esito.OK);
                    //}


                #endregion
                    #region Inserimento nella tabella di log delle fatture
                    bool insLogOk = BusinessLogic.Amministrazione.SistemiEsterni.FattElInsTabellaLog(schedaDocResult.systemId, request.Document.ConsolidationState, "");
                }
                    #endregion
                #region Caricamento File Principale
                try
                {
                    if (schedaDocResult != null && !string.IsNullOrEmpty(schedaDocResult.docNumber) && request.Document.MainDocument != null && numPasso < 9)
                    {
                        //Crea il file o se esistente una nuova versione
                        // 5. Acquisizione del file al documento
                        DocsPaVO.documento.FileRequest versioneCorrente = (DocsPaVO.documento.FileRequest)schedaDocResult.documenti[0];

                        DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                        {
                            name = request.Document.MainDocument.Name,
                            fullName = request.Document.MainDocument.Name,
                            content = request.Document.MainDocument.Content,
                            contentType = request.Document.MainDocument.MimeType,
                            length = request.Document.MainDocument.Content.Length,
                            bypassFileContentValidation = true
                        };

                        string errorMessage;

                        if (!BusinessLogic.Documenti.FileManager.putFile(ref versioneCorrente, fileDocumento, infoUtente, out errorMessage))
                        {
                            throw new PisException("FILE_CREATION_ERROR");
                        }
                        else
                        {
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "FILE_CARICATO", schedaDoc.systemId, "File caricato", DocsPaVO.Logger.CodAzione.Esito.OK);
                        }

                    }
                }
                catch (Exception exFilePrincipale)
                {
                    bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, "", "CARICAMENTO FILE PRINCIPALE", "ERRORE");
                    throw new Exception("Errore al passo CARICAMENTO FILE PRINCIPALE");
                }
                #endregion


                #region Creazione Documento in risposta
                if (schedaDocResult != null)
                {
                    response.Document = Utils.GetDetailsDocument(schedaDocResult, infoUtente, false);
                    if (!string.IsNullOrEmpty(infoUtente.diSistema) && infoUtente.diSistema == "1")
                    {
                        bool pulito = Utils.CleanRightsExtSys(schedaDocResult.docNumber, infoUtente.idPeople, infoUtente.idGruppo);
                    }
                }
                response.Success = true;
                #endregion
                #region Caricamento allegati
                try
                {
                    if (request.Document.Attachments != null && request.Document.Attachments.Length > 0 && numPasso < 11)
                    {
                        string erroreMessage = "";
                        foreach (Domain.File all in request.Document.Attachments)
                        {
                            DocsPaVO.documento.Allegato allegato = new DocsPaVO.documento.Allegato
                            {
                                docNumber = schedaDocResult.systemId,
                                descrizione = all.Description
                            };

                            // Acquisizione dell'allegato
                            allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, allegato);
                            //definisco l'allegato come esterno
                            try
                            {
                                if (!string.IsNullOrEmpty(all.VersionId) && all.VersionId == "E")
                                {
                                    if (!BusinessLogic.Documenti.AllegatiManager.setFlagAllegatiEsterni(allegato.versionId, allegato.docNumber))
                                    {
                                        BusinessLogic.Documenti.AllegatiManager.rimuoviAllegato(allegato, infoUtente);
                                        throw new PisException("ERROR_ADD_ATTACHMENT");
                                    }
                                    else
                                    {
                                        if (all.Description.ToUpper() == "FATTURA")
                                            allegatoRapprFattura = allegato;
                                    }
                                }
                            }
                            catch (PisException pisEx)
                            {
                                response.Error = new Services.ResponseError
                                {
                                    Code = pisEx.ErrorCode,
                                    Description = pisEx.Description
                                };

                                response.Success = false;
                                return response;
                            }

                            // Acquisizione file allegato
                            // 5. Acquisizione del file al documento
                            DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                            {
                                name = all.Name,
                                fullName = all.Name,
                                content = all.Content,
                                contentType = all.MimeType,
                                length = all.Content.Length,
                                bypassFileContentValidation = true
                            };
                            DocsPaVO.documento.FileRequest allegatoFR = allegato;

                            if (!BusinessLogic.Documenti.FileManager.putFile(ref allegatoFR, fileDocumento, infoUtente, out erroreMessage))
                            {
                                throw new PisException("FILE_CREATION_ERROR");
                            }
                        }
                    }
                }
                catch (Exception exCaricaAllegati)
                {
                    bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, "", "CARICAMENTO ALLEGATI", "ERRORE");
                    throw new Exception("Errore al passo CARICAMENTO ALLEGATI");
                }

                #endregion
                #region Inserimento in fascicolo
                try
                {
                    string msg = "";
                    if (numPasso < 12)
                    {
                        if (fascicolo != null)
                        {
                            //Caso del sottofascicolo
                            if (fascicolo.folderSelezionato != null)
                            {
                                BusinessLogic.Fascicoli.FolderManager.addDocFolder(infoUtente, schedaDocResult.systemId, fascicolo.folderSelezionato.systemID, false, out msg);
                            }
                            else
                            {
                                BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, schedaDocResult.systemId, fascicolo.systemID, false, out msg);
                            }
                        }
                        else
                        {
                            //Fascicolo non trovato
                            throw new PisException("PROJECT_NOT_FOUND");
                        }
                    }
                }
                catch (Exception exInFasc)
                {
                    bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, "", "INSERIMENTO IN FASCICOLO", "ERRORE");
                    throw new Exception("Errore al passo INSERIMENTO IN FASCICOLO");
                }
                #endregion

                #region Associazione template
                try
                {
                    if (numPasso < 13)
                    {
                        if (schedaDocResult != null)
                        {
                            DocsPaVO.ProfilazioneDinamica.Templates template = null;

                            if (!string.IsNullOrEmpty(request.Document.Template.Id))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(request.Document.Template.Id);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(request.Document.Template.Name))
                                {
                                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(request.Document.Template.Name, infoUtente.idAmministrazione);
                                }
                            }
                            if (template != null)
                            {
                                Domain.File fileDaPassare = ((request.Document.MainDocument != null && request.Document.MainDocument.Content != null) ? request.Document.MainDocument : null);

                                schedaDocResult.template = Utils.GetTemplateFromPisVisibility(request.Document.Template, template, false, infoUtente.idGruppo, "D", request.CodeApplication, infoUtente, fileDaPassare, request.CodeRegister, request.CodeRF, false, request.IdentificativoSdI);
                                schedaDocResult.daAggiornareTipoAtto = true;
                                schedaDocResult.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                                schedaDocResult.tipologiaAtto.systemId = schedaDocResult.template.SYSTEM_ID.ToString();
                                schedaDocResult.tipologiaAtto.descrizione = schedaDocResult.template.DESCRIZIONE.ToString();

                                bool daAggiornareUffRef = false;

                                schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                                if (schedaDocResult == null)
                                {
                                    throw new PisException("ASS_TEMPLATE_ERROR");
                                }

                                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggcustom in schedaDoc.template.ELENCO_OGGETTI)
                                {
                                    if (oggcustom.CONTATORE_DA_FAR_SCATTARE == true && oggcustom.REPERTORIO == "1")
                                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTO_REPERTORIATO", schedaDoc.systemId, "Repertoriato documento", DocsPaVO.Logger.CodAzione.Esito.OK);
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, "", "ASSOCIAZIONE TEMPLATE", "ERRORE");
                    logger.Error("Errore in ASSOCIAZIONE TEMPLATE :" + ex.Message);
                    throw new Exception("Errore in ASSOCIAZIONE TEMPLATE");
                }
                #endregion

                #region Esecuzione trasmissione
                try
                {
                    if (numPasso < 14)
                    {
                        string trasmType = "S";
                        if (!string.IsNullOrEmpty(request.TransmissionType) && request.TransmissionType == "T")
                        {
                            trasmType = "T";
                        }

                        //creazione oggetto trasmissione e popolamento campi.
                        DocsPaVO.trasmissione.Trasmissione transmission = new DocsPaVO.trasmissione.Trasmissione();
                        // Creazione dell'oggetto trasmissione
                        transmission = new DocsPaVO.trasmissione.Trasmissione();

                        // Impostazione dei parametri della trasmissione
                        // Note generali
                        // transmission.noteGenerali = ragione.note;

                        // Tipo oggetto
                        transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;

                        // Informazioni sul documento
                        transmission.infoDocumento = BusinessLogic.Documenti.DocManager.getInfoDocumento(schedaDocResult);

                        // Utente mittente della trasmissione
                        transmission.utente = utente;

                        // Ruolo mittente
                        // Se inserito in ingresso il CodeRole utilizzo il coderole (WIP)
                        transmission.ruolo = ruolo;
                        string generaNotifica = "1";
                        if (request.TransmissionNotify != null && !request.TransmissionNotify)
                        {
                            transmission.NO_NOTIFY = "1";
                            generaNotifica = "0";
                        }
                        else
                        {
                            transmission.NO_NOTIFY = "0";
                            generaNotifica = "1";
                        }
                        // Modifica per invio in mail dell'url del frontend
                        string urlfrontend = "";
                        if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                            urlfrontend = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();

                        transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(
                            transmission, corr, ragione, "", trasmType);
                        // il tipo trasm è S

                        DocsPaVO.trasmissione.Trasmissione resultTrasm = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(urlfrontend, transmission);
                        response.Success = true;
                        string desc = string.Empty;
                        // LOG per documento
                        if (resultTrasm.infoDocumento != null && !string.IsNullOrEmpty(resultTrasm.infoDocumento.idProfile))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in resultTrasm.trasmissioniSingole)
                            {
                                string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                if (resultTrasm.infoDocumento.segnatura == null)
                                    desc = "Trasmesso Documento : " + resultTrasm.infoDocumento.docNumber.ToString();
                                else
                                    desc = "Trasmesso Documento : " + resultTrasm.infoDocumento.segnatura.ToString();

                                BusinessLogic.UserLog.UserLog.WriteLog(resultTrasm.utente.userId, resultTrasm.utente.idPeople, resultTrasm.ruolo.idGruppo, resultTrasm.utente.idAmministrazione, method, resultTrasm.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                            }
                        }
                        // LOG per fascicolo
                        if (resultTrasm.infoFascicolo != null && !string.IsNullOrEmpty(resultTrasm.infoFascicolo.idFascicolo))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in resultTrasm.trasmissioniSingole)
                            {
                                string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                desc = "Trasmesso Fascicolo ID: " + resultTrasm.infoFascicolo.idFascicolo.ToString();
                                BusinessLogic.UserLog.UserLog.WriteLog(resultTrasm.utente.userId, resultTrasm.utente.idPeople, resultTrasm.ruolo.idGruppo, resultTrasm.utente.idAmministrazione, method, resultTrasm.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                            }
                        }
                    }
                }
                catch (Exception exTrasm)
                {
                    bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, "", "ESECUZIONE TRASMISSIONI", "ERRORE");
                    throw new Exception("Errore al passo ESECUZIONE TRASMISSIONI");
                }
                #endregion
                #region Update tabella di log delle fatture con status OK
                if (!string.IsNullOrEmpty(request.Document.ConsolidationState))
                {
                    bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, "", "", "OK");
                }
                #endregion
                #region Scambio del xml con la rappresentazione fattura - commentato
                //BusinessLogic.Documenti.AllegatiManager.scambiaAllegatoDocumento(infoUtente, allegatoRapprFattura, (DocsPaVO.documento.Documento)schedaDocResult.documenti[0]);

                #endregion

            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                //bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, pisEx.Description, "ERRORE");
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                //bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, ex.Message, "ERRORE");
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Response CaricaLottoInPi3(Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Request request)
        {
            Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Response response = new Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Response();
            try
            {
                response.Success = true;
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.documento.SchedaDocumento schedaDoc = null, schedaDocResult = null;

                string passo = "", esito = "";
                int numPasso = 0;

                #region Autenticazione
                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "CaricaLottoInPi3");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                DocsPaVO.utente.Corrispondente corr = null;
                DocsPaVO.trasmissione.RagioneTrasmissione ragione = null;

                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente
                if (string.IsNullOrEmpty(request.CodeProject))
                {
                    throw new Exception("Codice del fascicolo richiesto");
                }

                if (request != null && string.IsNullOrEmpty(request.Document.DocumentType))
                {
                    throw new PisException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && string.IsNullOrEmpty(request.Document.Object))
                {
                    throw new PisException("MISSING_OBJECT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (!request.Document.DocumentType.ToUpper().Equals("A") && !request.Document.DocumentType.ToUpper().Equals("P") && !request.Document.DocumentType.ToUpper().Equals("I") && !request.Document.DocumentType.ToUpper().Equals("G")))
                {
                    throw new PisException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (request.Document.DocumentType.ToUpper().Equals("A") || request.Document.DocumentType.ToUpper().Equals("P") || request.Document.DocumentType.ToUpper().Equals("I")) && string.IsNullOrEmpty(request.CodeRegister))
                {
                    throw new PisException("REQUIRED_REGISTER");
                }
                #endregion
                #region Prelievo del titolario attivo
                string idTitolario = "";
                ArrayList titolari = new ArrayList();
                try
                {
                    titolari = BusinessLogic.Amministrazione.TitolarioManager.getTitolariUtilizzabili(infoUtente.idAmministrazione);
                }
                catch
                {
                    //Titolario attivo non trovato
                    throw new PisException("CLASSIFICATION_NOT_FOUND");
                }
                if (titolari != null && titolari.Count > 0)
                {
                    foreach (DocsPaVO.amministrazione.OrgTitolario tempTit in titolari)
                    {
                        if (tempTit.Stato == DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo)
                        {
                            idTitolario = tempTit.ID;
                            break;
                        }
                    }
                }
                #endregion
                #region Controllo del registro per repertoriazione e spedizione a responsabile - COMMENTATO
                //if (string.IsNullOrEmpty(request.CodeRegister))
                //{
                //    throw new PisException("REQUIRED_REGISTER");
                //}
                //else
                //{
                //    DocsPaVO.utente.Registro regX = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeRegister);
                //    if (regX == null)
                //    {
                //        throw new PisException("REGISTER_NOT_FOUND");
                //    }
                //    else
                //    {
                //        if (!string.IsNullOrEmpty(regX.chaRF) && regX.chaRF.Equals("1"))
                //        {
                //            request.CodeRF = request.CodeRegister;
                //            request.CodeRegister = null;
                //        }

                //        string idRespRole = "";
                //        string idRespCorr = "";
                //        if (!string.IsNullOrEmpty(regX.chaRF) && regX.chaRF.Equals("1"))
                //            idRespRole = BusinessLogic.utenti.RegistriRepertorioPrintManager.GetResponsableRoleIdFromIdTemplate(request.Document.Template.Id, regX.systemId, null, out idRespCorr);
                //        else
                //            idRespRole = BusinessLogic.utenti.RegistriRepertorioPrintManager.GetResponsableRoleIdFromIdTemplate(request.Document.Template.Id, null, regX.systemId, out idRespCorr);

                //        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(idRespCorr);

                //        ragione = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, "RICEVIMENTO_FATTURA");
                //        if (ragione == null)
                //            throw new PisException("TRANSMISSION_REASON_NOT_FOUND");
                //    }

                //}
                #endregion
                #region Controllo del destinatario e della ragione della trasmissione
                if (request.TransmissionReceiver == null)
                {
                    //Destinatario non trovato
                    throw new PisException("REQUIRED_CORRESPONDENT");
                }
                if (string.IsNullOrEmpty(request.TransmissionReceiver.Code) && string.IsNullOrEmpty(request.TransmissionReceiver.Id))
                {
                    throw new PisException("REQUIRED_CODE_OR_ID_RECEIVER");
                }
                corr = null;
                try
                {
                    if (!string.IsNullOrEmpty(request.TransmissionReceiver.Id))
                    {
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(request.TransmissionReceiver.Id);
                    }
                    else if (!string.IsNullOrEmpty(request.TransmissionReceiver.Code))
                    {
                        //corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(request.Receiver.Code, infoUtente);
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(request.TransmissionReceiver.Code, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                    }
                    if (corr == null)
                    {
                        throw new PisException("CORRESPONDENT_NOT_FOUND");
                    }

                }
                catch (Exception e)
                {
                    throw new PisException("CORRESPONDENT_NOT_FOUND");
                }

                ragione = null;
                if (request.TransmissionReason == null || string.IsNullOrEmpty(request.TransmissionReason))
                {
                    throw new PisException("REQUIRED_TRANSMISSION_REASON");
                }
                else if ((!string.IsNullOrEmpty(infoUtente.diSistema)) && (infoUtente.diSistema == "1") && (request.TransmissionReason != "COMPETENZA_SIST_ESTERNI"))
                {
                    throw new Exception("Ragione di trasmissione non valida per un sistema esterno. Ragione disponibile: COMPETENZA_SIST_ESTERNI");
                }
                try
                {
                    ragione = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, request.TransmissionReason.ToUpper());
                    if (ragione == null)
                        throw new PisException("TRANSMISSION_REASON_NOT_FOUND");
                }
                catch (Exception e)
                {
                    throw new PisException("TRANSMISSION_REASON_NOT_FOUND");
                }
                #endregion
                #region Controllo presenza fascicolo
                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();
                if (!string.IsNullOrEmpty(request.CodeProject))
                {
                    //Il codice è di un sottofascicolo
                    if (request.CodeProject.IndexOf("//") > -1)
                    {
                        string separatore = "//";
                        // MODIFICA per inserimento in sottocartelle
                        string[] separatoreAr = new string[] { separatore };

                        string[] pathCartelle = request.CodeProject.Split(separatoreAr, StringSplitOptions.RemoveEmptyEntries);

                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloDaCodice(infoUtente, pathCartelle[0], null, false, false);
                        if (fascicolo != null)
                        {
                            if (pathCartelle.Length > 1)
                            {
                                ArrayList cartelle = BusinessLogic.Fascicoli.FascicoloManager.getListaFolderDaIdFascicolo(infoUtente, fascicolo.systemID, null, false, false);
                                // caricamento folder base
                                string idParentFolder = fascicolo.systemID;
                                fascicolo.folderSelezionato = (DocsPaVO.fascicolazione.Folder)((from DocsPaVO.fascicolazione.Folder f in cartelle where f.idParent == idParentFolder select f).ToList().FirstOrDefault());
                                idParentFolder = fascicolo.folderSelezionato.systemID;
                                try
                                {
                                    for (int i = 1; i < pathCartelle.Length; i++)
                                    {

                                        fascicolo.folderSelezionato = (DocsPaVO.fascicolazione.Folder)((from DocsPaVO.fascicolazione.Folder f in cartelle where f.idParent == idParentFolder && f.descrizione == pathCartelle[i] select f).ToList().First());
                                        idParentFolder = fascicolo.folderSelezionato.systemID;
                                    }
                                }
                                catch (Exception e)
                                {
                                    throw new Exception("Cartella non trovata nel fascicolo");
                                }
                            }
                        }
                        else
                        {
                            throw new PisException("PROJECT_NOT_FOUND");
                        }


                    }
                    else
                    {
                        ArrayList fascicoli = new ArrayList();
                        fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliDaCodice(infoUtente, request.CodeProject, null, false, false, "I");
                        if (fascicoli != null && fascicoli.Count > 0)
                        {
                            if (fascicoli.Count == 1)
                            {
                                fascicolo = (DocsPaVO.fascicolazione.Fascicolo)fascicoli[0];
                            }
                            else
                            {
                                //Fascicoli multipli
                                throw new PisException("MULTIPLE");
                            }
                        }
                        else
                        {
                            //Fascicolo non trovato
                            throw new PisException("PROJECT_NOT_FOUND");
                        }
                    }
                }


                #endregion
                #region Controllo presenza Lotto
                if (!string.IsNullOrEmpty(request.IdentificativoSdI))
                {
                    string idLottoPresente = BusinessLogic.Amministrazione.SistemiEsterni.FattElCtrlDupl(null, request.IdentificativoSdI, out passo, out esito);
                    if (!string.IsNullOrEmpty(idLottoPresente))
                    {
                        logger.DebugFormat("Lotto presente con DocNum {0}. Passo {1}. Esito {2}", idLottoPresente, passo, esito);
                        //throw new Exception(string.Format("Fattura con identificativo {0} già presente. Id PITre: {1}", request.Document.ConsolidationState, idFatturaPresente));
                        //schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idLottoPresente, idLottoPresente);
                        if (esito.ToUpper() == "OK")
                        {
                            numPasso = 15;
                            throw new Exception(string.Format("Il lotto con IdSdI {1} è già presente con IdPiTre {0}.", idLottoPresente, request.IdentificativoSdI));
                        }
                        else if (esito.ToUpper() == "ERRORE")
                        {
                            schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idLottoPresente, idLottoPresente);
                            schedaDocResult = schedaDoc;
                            switch (passo.ToUpper())
                            {
                                case "CARICAMENTO FILE PRINCIPALE":
                                    numPasso = 8;
                                    break;
                                case "CREAZIONE DOCUMENTO IN RISPOSTA":
                                    numPasso = 9;
                                    break;
                                case "CARICAMENTO ALLEGATI":
                                    numPasso = 10;
                                    break;
                                case "INSERIMENTO IN FASCICOLO":
                                    numPasso = 11;
                                    break;
                                case "ASSOCIAZIONE TEMPLATE":
                                    numPasso = 12;
                                    break;
                                case "ESECUZIONE TRASMISSIONI":
                                    numPasso = 13;
                                    break;
                                default:
                                    numPasso = 8;
                                    break;
                            }
                        }

                    }
                }
                #endregion
                #region Creazione Scheda Documento
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                DocsPaVO.utente.Corrispondente mittente = new DocsPaVO.utente.Corrispondente();

                try
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                    if (ruolo == null)
                    {
                        //Ruolo non trovato
                        throw new PisException("ROLE_NO_EXIST");
                    }
                }
                catch
                {
                    //Ruolo non trovato
                    throw new PisException("ROLE_NO_EXIST");
                }

                if (numPasso < 8)
                {

                    // **************************************************************************
                    // Per accordi con il cliente viene effettuato questo controllo:
                    // se Type == "O" allore viene impostato il valore di CorrespondentType = "O"
                    // Controllo effettuato per: Sender, Recipients, RecipientsCC
                    // **************************************************************************
                    //
                    if (request.Document.Sender != null && !string.IsNullOrEmpty(request.Document.Sender.Type) && request.Document.Sender.Type.Equals("O"))
                        request.Document.Sender.CorrespondentType = "O";

                    if (request.Document.MultipleSenders != null)
                    {
                        for (int i = 0; i < request.Document.MultipleSenders.Length; i++)
                        {
                            Domain.Correspondent c = request.Document.MultipleSenders[i];
                            if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                                c.CorrespondentType = "O";
                        }
                    }

                    if (request.Document.Recipients != null)
                    {
                        for (int i = 0; i < request.Document.Recipients.Length; i++)
                        {
                            Domain.Correspondent c = request.Document.Recipients[i];
                            if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                                c.CorrespondentType = "O";
                        }
                    }

                    if (request.Document.RecipientsCC != null)
                    {
                        for (int i = 0; i < request.Document.RecipientsCC.Length; i++)
                        {
                            Domain.Correspondent c = request.Document.RecipientsCC[i];
                            if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                                c.CorrespondentType = "O";
                        }
                    }
                    //


                    ///Creazione schedaDocumento con i dati essenziali e obbligatori
                    schedaDoc = BusinessLogic.Documenti.DocManager.NewSchedaDocumento(infoUtente, false);

                    ///Imposto l'oggetto
                    if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.Object))
                    {
                        schedaDoc.oggetto.descrizione = request.Document.Object;
                        schedaDoc.oggetto.daAggiornare = true;
                    }

                    //Imposto le note, soltanto visibili a tutti
                    if (request != null && request.Document != null && request.Document != null && request.Document.Note != null && request.Document.Note.Length > 0)
                    {
                        schedaDoc.noteDocumento = new List<DocsPaVO.Note.InfoNota>();
                        foreach (Domain.Note nota in request.Document.Note)
                        {
                            DocsPaVO.Note.InfoNota tempNot = new DocsPaVO.Note.InfoNota();
                            tempNot.DaInserire = true;
                            tempNot.Testo = nota.Description;
                            tempNot.TipoVisibilita = DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti;
                            tempNot.UtenteCreatore = new DocsPaVO.Note.InfoUtenteCreatoreNota();
                            tempNot.UtenteCreatore.IdRuolo = infoUtente.idGruppo;
                            tempNot.UtenteCreatore.IdUtente = infoUtente.idPeople;
                            schedaDoc.noteDocumento.Add(tempNot);
                        }
                    }
                    //

                    ///Imposto il template
                    if (request != null && request.Document != null && request.Document.Template != null && (!string.IsNullOrEmpty(request.Document.Template.Id) || (!string.IsNullOrEmpty(request.Document.Template.Name))))
                    {
                        DocsPaVO.ProfilazioneDinamica.Templates template = null;
                        if (!string.IsNullOrEmpty(request.Document.Template.Id))
                        {
                            template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(request.Document.Template.Id);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(request.Document.Template.Name))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(request.Document.Template.Name, infoUtente.idAmministrazione);
                            }
                        }

                        if (template != null)
                        {
                            //Modifica Lembo 10-01-2013: Controllo se l'utente è abilitato all'utilizzo del template.
                            //Solo ruoli con diritti di scrittura ammessi.
                            string controlFilter = string.Format("{0} and id_ruolo={1} and (diritti='2')", template.SYSTEM_ID.ToString(), infoUtente.idGruppo);
                            ArrayList controlloVisibilita = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getRuoliTipoDoc(controlFilter);
                            if (controlloVisibilita == null || controlloVisibilita.Count == 0)
                            {
                                throw new PisException("TEMPLATE_NOT_ROLE_EDITABLE");
                            }
                            // Modifica Lembo 11-01-2013: Utilizzo il metodo che controlla la visibilità dei campi.
                            //schedaDoc.template = Utils.GetTemplateFromPis(request.Document.Template, template, false);

                            // Modifica Elaborazione XML da PIS req.2

                            Domain.File fileDaPassare = ((request.Document.MainDocument != null && request.Document.MainDocument.Content != null) ? request.Document.MainDocument : null);

                            schedaDoc.template = Utils.GetTemplateFromPisVisibility(request.Document.Template, template, false, infoUtente.idGruppo, "D", request.CodeApplication, infoUtente, fileDaPassare, request.CodeRegister, request.CodeRF, false, request.IdentificativoSdI);
                            schedaDoc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                            schedaDoc.tipologiaAtto.systemId = template.SYSTEM_ID.ToString();
                            schedaDoc.tipologiaAtto.descrizione = template.DESCRIZIONE.ToString();

                            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggcustom in schedaDoc.template.ELENCO_OGGETTI)
                            {
                                if (oggcustom.CONTATORE_DA_FAR_SCATTARE == true)
                                    oggcustom.CONTATORE_DA_FAR_SCATTARE = false;
                            }


                        }
                        else
                        {
                            //Template non trovato
                            throw new PisException("TEMPLATE_NOT_FOUND");
                        }
                    }
                    ///

                    ///Se privato
                    if (request != null && request.Document != null && request.Document.PrivateDocument)
                    {
                        schedaDoc.privato = "1";
                    }
                    ///

                    ///Se personale
                    if (request != null && request.Document != null && request.Document.PersonalDocument)
                    {
                        schedaDoc.personale = "1";
                    }
                    ///

                    schedaDocResult = null;
                    DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione = new DocsPaVO.documento.ResultProtocollazione();
                    bool daAggiornareUffRef = false;

                    ///CASO DOCUMENTO GRIGIO (NON PROTOCOLLATO)
                    if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("G"))
                    {
                        schedaDoc.tipoProto = "G";
                        schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                    }
                    ///

                    //Controlli solo per protocolli
                    if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && !request.Document.DocumentType.ToUpper().Equals("G"))
                    {

                        if (request.Document.Sender == null || string.IsNullOrEmpty(request.Document.Sender.CorrespondentType))
                        {
                            if (!request.Document.DocumentType.ToUpper().Equals("P"))
                            {
                                //Mittente non presente
                                throw new PisException("REQUIRED_SENDER");
                            }
                            else
                            {
                                mittente = null;
                            }
                        }
                        else
                        {
                            if (request.Document.Sender != null && !string.IsNullOrEmpty(request.Document.Sender.CorrespondentType) && request.Document.Sender.CorrespondentType.Equals("O"))
                            {
                                mittente = Utils.GetCorrespondentFromPis(request.Document.Sender, infoUtente);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(request.Document.Sender.Id))
                                {
                                    mittente = Utils.RisolviCorrispondente(request.Document.Sender.Id, infoUtente);
                                    if (mittente == null)
                                        //Mittente non trovato
                                        throw new PisException("SENDER_NOT_FOUND");
                                }
                            }
                        }

                        if (request.Document.DocumentType.ToUpper().Equals("I") || request.Document.DocumentType.ToUpper().Equals("P"))
                        {
                            if (request.Document.Recipients == null || request.Document.Recipients.Length == 0)
                            {
                                //Destinatario non presente
                                throw new PisException("REQUIRED_RECIPIENT");
                            }
                        }

                        if (string.IsNullOrEmpty(request.CodeRegister))
                        {
                            //Registro mancante
                            throw new PisException("REQUIRED_REGISTER");
                        }
                        else
                        {
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeRegister);
                            if (reg == null)
                            {
                                //Registro mancante
                                throw new PisException("REGISTER_NOT_FOUND");
                            }
                            else
                            {
                                schedaDoc.registro = reg;
                            }
                        }

                        if (!string.IsNullOrEmpty(request.CodeRF))
                        {
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeRF);
                            if (reg != null)
                            {
                                schedaDoc.id_rf_prot = reg.systemId;
                                schedaDoc.id_rf_invio_ricevuta = reg.systemId;
                                schedaDoc.cod_rf_prot = reg.codRegistro;
                            }
                            else
                            {
                                //RF non trovato
                                throw new PisException("RF_NOT_FOUND");
                            }
                        }
                    }

                    ///CASO PROTOCOLLO IN ARRIVO (ANCHE PREDISPOSTO)
                    if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("A"))
                    {
                        schedaDoc.tipoProto = "A";
                        schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloEntrata();

                        ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente = mittente;

                        if (request.Document.MultipleSenders != null && request.Document.MultipleSenders.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittenti = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.MultipleSenders)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = null;
                                if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                                {
                                    corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(corrTemp.Id))
                                    {
                                        corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                        if (corrBySys == null)
                                        {
                                            //Mittente non trovato
                                            throw new PisException("SENDER_NOT_FOUND");
                                        }
                                    }
                                }
                                ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittenti.Add(corrBySys);
                            }
                        }

                        //Data protocollo mittente
                        if (!string.IsNullOrEmpty(request.Document.DataProtocolSender))
                        {
                            ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).dataProtocolloMittente = request.Document.DataProtocolSender;
                        }

                        //Protocollo mittente
                        if (!string.IsNullOrEmpty(request.Document.ProtocolSender))
                        {
                            ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).descrizioneProtocolloMittente = request.Document.ProtocolSender;
                        }

                        //Data arrivo TODO
                        if (!string.IsNullOrEmpty(request.Document.ArrivalDate))
                        {
                            // Variabile Booleana per il controllo di validità della data passata in input;
                            bool valid = false;
                            // Data ritornata in output dal TryParseExact per la data arrivo passata in input
                            DateTime dateVal;

                            // Pattern di validità per una data valida
                            string pattern = "dd/MM/yyyy HH:mm";

                            try
                            {
                                /*
                                 *  Date Format Example: 01/03/2012
                                 *  dd = day (01)
                                 *  MM = month (03)
                                 *  yyyy = year (2012)
                                 *  / = date separator
                                 */

                                if (!DateTime.TryParseExact(request.Document.ArrivalDate, pattern, null, System.Globalization.DateTimeStyles.None, out dateVal))
                                {
                                    throw new Exception("Formato Data non corretto");
                                }

                                valid = true;
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Formato Data non corretto");
                            }

                            if (valid)
                            {
                                // Controllo che la data inserita non sia posteriore alla data odierna
                                DateTime dateVal2;
                                // Variabile Booleana per il controllo di validità della data DateTime.Now trasformato nel formato dd/MM/yyyy;
                                bool dateNow = true;

                                if (!DateTime.TryParseExact(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), pattern, null, System.Globalization.DateTimeStyles.None, out dateVal2))
                                {
                                    // Non dovrebbe entrare mai in questo controllo.
                                    dateNow = false;
                                    throw new Exception("Formato Data non corretto");
                                }

                                if (dateNow)
                                {
                                    // dateVal: data passata in input;
                                    // dateVal2: DateTime.Now convertito nel formato dd/MM/yyyy
                                    int result = DateTime.Compare(dateVal, dateVal2);

                                    if (result > 0)
                                        // Data di input è successiva alla data odierna;
                                        //((Documento)schedaDoc.documenti[0]).dataArrivo = string.Empty;
                                        throw new Exception("La data di arrivo non può essere posteriore alla data di segnatura");
                                    else
                                        // Data di input valida e conforme;
                                        ((DocsPaVO.documento.Documento)schedaDoc.documenti[0]).dataArrivo = request.Document.ArrivalDate;
                                }
                            }
                        }

                        if (request.Document.Predisposed)
                        {
                            schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                            if (schedaDocResult != null)
                            {
                                schedaDocResult.predisponiProtocollazione = true;
                                schedaDocResult.tipoProto = request.Document.DocumentType;
                                schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                            }
                        }
                        else
                        {
                            schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out risultatoProtocollazione);
                        }

                        if (schedaDocResult != null)
                        {
                            //Mezzo di spedizione
                            if (!string.IsNullOrEmpty(request.Document.MeansOfSending))
                            {
                                ArrayList mezziSpedizione = BusinessLogic.Amministrazione.MezziSpedizioneManager.ListaMezziSpedizione(infoUtente.idAmministrazione, true);
                                foreach (DocsPaVO.amministrazione.MezzoSpedizione m_spediz in mezziSpedizione)
                                {
                                    if (m_spediz.Descrizione.ToUpper().Equals(request.Document.MeansOfSending.ToUpper()))
                                    {
                                        BusinessLogic.Documenti.ProtoManager.collegaMezzoSpedizioneDocumento(infoUtente, m_spediz.IDSystem, schedaDocResult.systemId);
                                        break;
                                    }
                                }

                            }
                        }
                    }
                    ///

                    ///CASO PROTOCOLLO IN PARTENZA (ANCHE PREDISPOSTO)
                    if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("P"))
                    {
                        schedaDoc.tipoProto = "P";
                        schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloUscita();

                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente = mittente;

                        if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.Recipients)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = null;

                                // Verifica se occasionale
                                if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                                {
                                    corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                }
                                else
                                {
                                    // Se non è occasionale, lo risolve sulla rubrica
                                    corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                    if (corrBySys == null)
                                    {
                                        //Destinatario non trovato
                                        throw new PisException("RECIPIENT_NOT_FOUND");
                                    }

                                }
                                if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "F" && corrBySys.tipoIE == null)
                                {
                                    ArrayList corrByRF = BusinessLogic.Rubrica.RF.getCorrispondentiByCodRF(corrBySys.codiceRubrica);
                                    foreach (DocsPaVO.utente.Corrispondente c2 in corrByRF)
                                    {
                                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(c2);
                                    }
                                }
                                else if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                                {
                                    ArrayList corrByLista = BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiByCodLista(corrBySys.codiceRubrica, infoUtente.idAmministrazione, infoUtente);
                                    foreach (DocsPaVO.utente.Corrispondente c3 in corrByLista)
                                    {
                                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(c3);
                                    }
                                }
                                else
                                {
                                    ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(corrBySys);
                                }
                            }
                        }

                        if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.RecipientsCC)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = null;

                                // Verifica se occasionale
                                if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                                {
                                    corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                }
                                else
                                {
                                    corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                    if (corrBySys == null)
                                    {
                                        //Destinatario non trovato
                                        throw new PisException("RECIPIENT_NOT_FOUND");
                                    }
                                }
                                if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "F" && corrBySys.tipoIE == null)
                                {
                                    ArrayList corrByRF = BusinessLogic.Rubrica.RF.getCorrispondentiByCodRF(corrBySys.codiceRubrica);
                                    foreach (DocsPaVO.utente.Corrispondente c2 in corrByRF)
                                    {
                                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(c2);
                                    }
                                }
                                else if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                                {
                                    ArrayList corrByLista = BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiByCodLista(corrBySys.codiceRubrica, infoUtente.idAmministrazione, infoUtente);
                                    foreach (DocsPaVO.utente.Corrispondente c3 in corrByLista)
                                    {
                                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(c3);
                                    }
                                }
                                else
                                {
                                    ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
                                }

                                //((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
                            }
                        }

                        if (request.Document.Predisposed)
                        {
                            schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                            if (schedaDocResult != null)
                            {
                                schedaDocResult.predisponiProtocollazione = true;
                                schedaDocResult.tipoProto = request.Document.DocumentType;
                                schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                            }
                        }
                        else
                        {
                            schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out risultatoProtocollazione);
                        }
                    }
                    ///

                    ///CASO PROTOCOLLO INTERNO (ANCHE PREDISPOSTO)
                    if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("I"))
                    {
                        schedaDoc.tipoProto = "I";
                        schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloInterno();

                        ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).mittente = mittente;

                        if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatari = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.Recipients)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new PisException("RECIPIENT_NOT_FOUND");
                                }
                                ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatari.Add(corrBySys);
                            }
                        }

                        if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.RecipientsCC)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new PisException("RECIPIENT_NOT_FOUND");
                                }
                                ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
                            }
                        }

                        if (request.Document.Predisposed)
                        {
                            schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                            if (schedaDocResult != null)
                            {
                                schedaDocResult.predisponiProtocollazione = true;
                                schedaDocResult.tipoProto = request.Document.DocumentType;
                                schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                            }
                        }
                        else
                        {
                            schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out risultatoProtocollazione);
                        }
                    }
                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggcustom in schedaDoc.template.ELENCO_OGGETTI)
                    {
                        if (oggcustom.CONTATORE_DA_FAR_SCATTARE == true && oggcustom.REPERTORIO == "1")
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTO_REPERTORIATO", schedaDoc.systemId, "Repertoriato documento", DocsPaVO.Logger.CodAzione.Esito.OK);
                    }

                    ///
                    // Log creazione documento
                    if (schedaDocResult != null)
                    {
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTO_CREATO", schedaDoc.systemId, "Creato documento", DocsPaVO.Logger.CodAzione.Esito.OK);
                    }

                #endregion
                    #region Inserimento nella tabella di log delle fatture
                    bool insLogOk = BusinessLogic.Amministrazione.SistemiEsterni.FattElInsTabellaLog(schedaDocResult.systemId, null, request.IdentificativoSdI);
                }
                    #endregion
                #region Caricamento File Principale
                try
                {
                    if (schedaDocResult != null && !string.IsNullOrEmpty(schedaDocResult.docNumber) && request.Document.MainDocument != null && numPasso < 9)
                    {
                        //Crea il file o se esistente una nuova versione
                        // 5. Acquisizione del file al documento
                        DocsPaVO.documento.FileRequest versioneCorrente = (DocsPaVO.documento.FileRequest)schedaDocResult.documenti[0];

                        DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                        {
                            name = request.Document.MainDocument.Name,
                            fullName = request.Document.MainDocument.Name,
                            content = request.Document.MainDocument.Content,
                            contentType = request.Document.MainDocument.MimeType,
                            length = request.Document.MainDocument.Content.Length,
                            bypassFileContentValidation = true
                        };

                        string errorMessage;

                        if (!BusinessLogic.Documenti.FileManager.putFile(ref versioneCorrente, fileDocumento, infoUtente, out errorMessage))
                        {
                            throw new PisException("FILE_CREATION_ERROR");
                        }
                        else
                        {
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "FILE_CARICATO", schedaDoc.systemId, "File caricato", DocsPaVO.Logger.CodAzione.Esito.OK);
                        }


                    }
                }
                catch (Exception exFilePrincipale)
                {
                    bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(null, request.IdentificativoSdI, "CARICAMENTO FILE PRINCIPALE", "ERRORE");
                    throw new Exception("Errore al passo CARICAMENTO FILE PRINCIPALE");
                }
                #endregion
                #region Creazione Documento in risposta
                if (schedaDocResult != null)
                {
                    response.Document = Utils.GetDetailsDocument(schedaDocResult, infoUtente, false);
                    if (!string.IsNullOrEmpty(infoUtente.diSistema) && infoUtente.diSistema == "1")
                    {
                        bool pulito = Utils.CleanRightsExtSys(schedaDocResult.docNumber, infoUtente.idPeople, infoUtente.idGruppo);
                    }
                }
                response.Success = true;
                #endregion
                #region Caricamento allegati
                try
                {
                    if (request.Document.Attachments != null && request.Document.Attachments.Length > 0 & numPasso < 11)
                    {
                        string erroreMessage = "";
                        foreach (Domain.File all in request.Document.Attachments)
                        {
                            DocsPaVO.documento.FileRequest allegato = new DocsPaVO.documento.Allegato
                            {
                                docNumber = schedaDocResult.systemId,
                                descrizione = all.Description
                            };

                            // Acquisizione dell'allegato
                            allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, ((DocsPaVO.documento.Allegato)allegato));
                            //definisco l'allegato come esterno
                            try
                            {
                                if (!string.IsNullOrEmpty(all.VersionId) && all.VersionId == "E")
                                {
                                    if (!BusinessLogic.Documenti.AllegatiManager.setFlagAllegatiEsterni(allegato.versionId, allegato.docNumber))
                                    {
                                        BusinessLogic.Documenti.AllegatiManager.rimuoviAllegato((DocsPaVO.documento.Allegato)allegato, infoUtente);
                                        throw new PisException("ERROR_ADD_ATTACHMENT");
                                    }
                                }
                            }
                            catch (PisException pisEx)
                            {
                                response.Error = new Services.ResponseError
                                {
                                    Code = pisEx.ErrorCode,
                                    Description = pisEx.Description
                                };

                                response.Success = false;
                                return response;
                            }

                            // Acquisizione file allegato
                            // 5. Acquisizione del file al documento
                            DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                            {
                                name = all.Name,
                                fullName = all.Name,
                                content = all.Content,
                                contentType = all.MimeType,
                                length = all.Content.Length,
                                bypassFileContentValidation = true
                            };


                            if (!BusinessLogic.Documenti.FileManager.putFile(ref allegato, fileDocumento, infoUtente, out erroreMessage))
                            {
                                throw new PisException("FILE_CREATION_ERROR");
                            }
                        }
                    }
                }
                catch (Exception exCaricaAllegati)
                {
                    bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(null, request.IdentificativoSdI, "CARICAMENTO ALLEGATI", "ERRORE");
                    throw new Exception("Errore al passo CARICAMENTO ALLEGATI");
                }

                #endregion
                #region Inserimento in fascicolo
                try
                {
                    string msg = "";
                    if (numPasso < 12)
                    {
                        if (fascicolo != null)
                        {
                            //Caso del sottofascicolo
                            if (fascicolo.folderSelezionato != null)
                            {
                                BusinessLogic.Fascicoli.FolderManager.addDocFolder(infoUtente, schedaDocResult.systemId, fascicolo.folderSelezionato.systemID, false, out msg);
                            }
                            else
                            {
                                BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, schedaDocResult.systemId, fascicolo.systemID, false, out msg);
                            }
                        }
                        else
                        {
                            //Fascicolo non trovato
                            throw new PisException("PROJECT_NOT_FOUND");
                        }
                    }
                }
                catch (Exception exInFasc)
                {
                    bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(null, request.IdentificativoSdI, "INSERIMENTO IN FASCICOLO", "ERRORE");
                    throw new Exception("Errore al passo INSERIMENTO IN FASCICOLO");
                }
                #endregion

                #region Associazione template
                try
                {
                    if (numPasso < 13)
                    {
                        if (schedaDocResult != null)
                        {
                            DocsPaVO.ProfilazioneDinamica.Templates template = null;

                            if (!string.IsNullOrEmpty(request.Document.Template.Id))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(request.Document.Template.Id);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(request.Document.Template.Name))
                                {
                                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(request.Document.Template.Name, infoUtente.idAmministrazione);
                                }
                            }
                            if (template != null)
                            {
                                Domain.File fileDaPassare = ((request.Document.MainDocument != null && request.Document.MainDocument.Content != null) ? request.Document.MainDocument : null);

                                schedaDocResult.template = Utils.GetTemplateFromPisVisibility(request.Document.Template, template, false, infoUtente.idGruppo, "D", request.CodeApplication, infoUtente, fileDaPassare, request.CodeRegister, request.CodeRF, false, request.IdentificativoSdI);
                                schedaDocResult.daAggiornareTipoAtto = true;
                                schedaDocResult.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                                schedaDocResult.tipologiaAtto.systemId = schedaDocResult.template.SYSTEM_ID.ToString();
                                schedaDocResult.tipologiaAtto.descrizione = schedaDocResult.template.DESCRIZIONE.ToString();

                                bool daAggiornareUffRef = false;

                                schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                                if (schedaDocResult == null)
                                {
                                    throw new PisException("ASS_TEMPLATE_ERROR");
                                }

                                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggcustom in schedaDocResult.template.ELENCO_OGGETTI)
                                {
                                    if (oggcustom.CONTATORE_DA_FAR_SCATTARE == true && oggcustom.REPERTORIO == "1")
                                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTO_REPERTORIATO", schedaDoc.systemId, "Repertoriato documento", DocsPaVO.Logger.CodAzione.Esito.OK);
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, "", "ASSOCIAZIONE TEMPLATE", "ERRORE");
                    logger.Error("Errore in ASSOCIAZIONE TEMPLATE :" + ex.Message);
                    throw new Exception("Errore in ASSOCIAZIONE TEMPLATE");
                }

                #endregion

                #region Esecuzione trasmissione
                try
                {
                    if (numPasso < 14)
                    {
                        string trasmType = "S";
                        if (!string.IsNullOrEmpty(request.TransmissionType) && request.TransmissionType == "T")
                        {
                            trasmType = "T";
                        }

                        //creazione oggetto trasmissione e popolamento campi.
                        DocsPaVO.trasmissione.Trasmissione transmission = new DocsPaVO.trasmissione.Trasmissione();
                        // Creazione dell'oggetto trasmissione
                        transmission = new DocsPaVO.trasmissione.Trasmissione();

                        // Impostazione dei parametri della trasmissione
                        // Note generali
                        // transmission.noteGenerali = ragione.note;

                        // Tipo oggetto
                        transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;

                        // Informazioni sul documento
                        transmission.infoDocumento = BusinessLogic.Documenti.DocManager.getInfoDocumento(schedaDocResult);

                        // Utente mittente della trasmissione
                        transmission.utente = utente;

                        // Ruolo mittente
                        // Se inserito in ingresso il CodeRole utilizzo il coderole (WIP)
                        transmission.ruolo = ruolo;
                        string generaNotifica = "1";
                        if (request.TransmissionNotify != null && !request.TransmissionNotify)
                        {
                            transmission.NO_NOTIFY = "1";
                            generaNotifica = "0";
                        }
                        else
                        {
                            transmission.NO_NOTIFY = "0";
                            generaNotifica = "1";
                        }
                        // Modifica per invio in mail dell'url del frontend
                        string urlfrontend = "";
                        if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                            urlfrontend = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();

                        transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(
                            transmission, corr, ragione, "", trasmType);
                        // il tipo trasm è S

                        DocsPaVO.trasmissione.Trasmissione resultTrasm = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(urlfrontend, transmission);
                        response.Success = true;
                        string desc = string.Empty;
                        // LOG per documento
                        if (resultTrasm.infoDocumento != null && !string.IsNullOrEmpty(resultTrasm.infoDocumento.idProfile))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in resultTrasm.trasmissioniSingole)
                            {
                                string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                if (resultTrasm.infoDocumento.segnatura == null)
                                    desc = "Trasmesso Documento : " + resultTrasm.infoDocumento.docNumber.ToString();
                                else
                                    desc = "Trasmesso Documento : " + resultTrasm.infoDocumento.segnatura.ToString();

                                BusinessLogic.UserLog.UserLog.WriteLog(resultTrasm.utente.userId, resultTrasm.utente.idPeople, resultTrasm.ruolo.idGruppo, resultTrasm.utente.idAmministrazione, method, resultTrasm.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                            }
                        }
                        // LOG per fascicolo
                        if (resultTrasm.infoFascicolo != null && !string.IsNullOrEmpty(resultTrasm.infoFascicolo.idFascicolo))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in resultTrasm.trasmissioniSingole)
                            {
                                string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                desc = "Trasmesso Fascicolo ID: " + resultTrasm.infoFascicolo.idFascicolo.ToString();
                                BusinessLogic.UserLog.UserLog.WriteLog(resultTrasm.utente.userId, resultTrasm.utente.idPeople, resultTrasm.ruolo.idGruppo, resultTrasm.utente.idAmministrazione, method, resultTrasm.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                            }
                        }
                    }
                }
                catch (Exception exTrasm)
                {
                    bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(null, request.IdentificativoSdI, "ESECUZIONE TRASMISSIONI", "ERRORE");
                    throw new Exception("Errore al passo ESECUZIONE TRASMISSIONI");
                }
                #endregion
                #region Update tabella di log delle fatture con status OK
                if (!string.IsNullOrEmpty(request.IdentificativoSdI))
                {
                    bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(null, request.IdentificativoSdI, "", "OK");
                }
                #endregion

            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static VtDocsWS.Services.DocumentsAdvanced.C3GetDocs.C3GetDocsResponse C3GetDocs(VtDocsWS.Services.DocumentsAdvanced.C3GetDocs.C3GetDocsRequest request)
        {
            VtDocsWS.Services.DocumentsAdvanced.C3GetDocs.C3GetDocsResponse response = new Services.DocumentsAdvanced.C3GetDocs.C3GetDocsResponse();
            try
            {
                response.Success = true;
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "RemoveDocument");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                int numAllegati = 0;
                string fromTime = "", toTime = "", optionTime = "";
                bool modificati = false;
                if (!string.IsNullOrEmpty(request.DateLimitsOptions))
                    optionTime = request.DateLimitsOptions;
                else
                {
                    if (!string.IsNullOrEmpty(request.FromDateTime))
                    {
                        fromTime = request.FromDateTime;
                        if (!string.IsNullOrEmpty(request.ToDateTime))
                            toTime = request.ToDateTime;
                    }
                    else
                    {
                        optionTime = "6";
                    }
                }

                if (!string.IsNullOrEmpty(request.Modified) && request.Modified.ToLower() == "true")
                    modificati = true;
                DataTable fromDB = null;
                if (!modificati)
                    fromDB = BusinessLogic.Amministrazione.SistemiEsterni.C3GetDocs(fromTime, toTime, optionTime);
                else
                    fromDB = BusinessLogic.Amministrazione.SistemiEsterni.C3GetDocsMod(fromTime, toTime, optionTime);
                Domain.C3Document docX = null;
                ArrayList listaDocs = new ArrayList();
                Domain.Correspondent corrTemp = null, corrMitt = null;
                ArrayList mitts = new ArrayList(), dest = new ArrayList(), destCC = new ArrayList();
                foreach (DataRow r in fromDB.Rows)
                {
                    docX = new Domain.C3Document();
                    docX.Id = r["SYSTEM_ID"].ToString();
                    docX.DocNumber = r["SYSTEM_ID"].ToString();
                    docX.DocumentType = r["CHA_TIPO_PROTO"].ToString();
                    docX.Object = r["OGGETTO"].ToString();
                    docX.CreationDate = r["DATACREAZIONE"].ToString();
                    if (docX.DocumentType != "G")
                    {
                        docX.Signature = r["DOCNAME"].ToString();
                        docX.ProtocolNumber = r["NUM_PROTO"].ToString();
                        docX.ProtocolYear = r["NUM_ANNO_PROTO"].ToString();
                        docX.ProtocolDate = r["DATAPROTO"].ToString();
                        if (r["REGISTRO"] != null && !string.IsNullOrEmpty(r["REGISTRO"].ToString()))
                        {
                            docX.Register = new Domain.Register();
                            docX.Register.Id = r["REGISTRO"].ToString().Split('§')[0];
                            docX.Register.Code = r["REGISTRO"].ToString().Split('§')[1];
                            docX.Register.Description = r["REGISTRO"].ToString().Split('§')[2];
                            docX.Register.IsRF = r["REGISTRO"].ToString().Split('§')[3] == "1" ? true : false;
                        }
                        if (r["MITT_DEST"] != null && !string.IsNullOrEmpty(r["MITT_DEST"].ToString()))
                        {
                            string[] arrCorr = r["MITT_DEST"].ToString().Split(';');
                            foreach (string sX in arrCorr)
                            {
                                corrTemp = new Domain.Correspondent();
                                if (sX.Contains("(D)"))
                                {
                                    corrTemp.Description = sX.Replace("(D)", "");
                                    dest.Add(corrTemp);
                                }
                                else if (sX.Contains("(CC)"))
                                {
                                    corrTemp.Description = sX.Replace("(CC)", "");
                                    destCC.Add(corrTemp);
                                }
                                else
                                {
                                    corrTemp.Description = sX;
                                    mitts.Add(corrTemp);
                                }
                            }

                            if (mitts.Count < 2 && mitts.Count > 0) corrMitt = (Domain.Correspondent)mitts[0];

                            if (corrMitt != null) docX.Sender = corrMitt;
                            if (corrMitt == null && mitts.Count > 0) docX.MultipleSenders = (Domain.Correspondent[])mitts.ToArray(typeof(Domain.Correspondent));
                            if (dest.Count > 0) docX.Recipients = (Domain.Correspondent[])dest.ToArray(typeof(Domain.Correspondent));
                            if (destCC.Count > 0) docX.RecipientsCC = (Domain.Correspondent[])destCC.ToArray(typeof(Domain.Correspondent));
                        }
                    }
                    docX.Author = r["AUTORE"].ToString();
                    docX.AuthorId = r["IDAUTORE"].ToString();
                    docX.AuthorRole = r["RUOLOCREATORE"].ToString();
                    docX.AuthorRoleId = r["IDRUOLOAUTORE"].ToString();
                    docX.AuthorUO = r["UOCREATRICE"].ToString();
                    if (r["TIPOLOGIA"] != null && !string.IsNullOrEmpty(r["TIPOLOGIA"].ToString()))
                    {
                        docX.Template = new Domain.Template();
                        docX.Template.Id = r["TIPOLOGIA"].ToString().Split('§')[0];
                        docX.Template.Name = r["TIPOLOGIA"].ToString().Split('§')[1];
                    }

                    if (r["DOC_PRINCIPALE"] != null && !string.IsNullOrEmpty(r["DOC_PRINCIPALE"].ToString()))
                    {
                        docX.MainDocument = new Domain.C3File();
                        docX.MainDocument.Name = r["DOC_PRINCIPALE"].ToString().Split('§')[0];
                        docX.MainDocument.VersionId = r["DOC_PRINCIPALE"].ToString().Split('§')[1];
                        docX.MainDocument.MimeType = r["DOC_PRINCIPALE"].ToString().Split('§')[2];
                        docX.MainDocument.VersionIdinDB = r["DOC_PRINCIPALE"].ToString().Split('§')[3];
                        docX.MainDocument.PathName = r["DOC_PRINCIPALE"].ToString().Split('§')[4];
                        docX.MainDocument.FileSize = r["DOC_PRINCIPALE"].ToString().Split('§')[5];
                    }

                    if (r["NUM_ALLEGATI"] != null && !string.IsNullOrEmpty(r["NUM_ALLEGATI"].ToString()) && Int32.Parse(r["NUM_ALLEGATI"].ToString()) > 0)
                    {
                        Domain.C3File allX = null;
                        ArrayList allXs = new ArrayList();
                        DataTable fromDBAll = BusinessLogic.Amministrazione.SistemiEsterni.C3GetAllByIdDoc(docX.Id);
                        foreach (DataRow rAll in fromDBAll.Rows)
                        {
                            allX = new Domain.C3File();
                            allX.Description = rAll["OGGETTO"].ToString();
                            allX.Id = rAll["SYSTEM_ID"].ToString();
                            if (rAll["FILE_ALLEGATO"] != null && !string.IsNullOrEmpty(rAll["FILE_ALLEGATO"].ToString()))
                            {
                                allX.Name = rAll["FILE_ALLEGATO"].ToString().Split('§')[0];
                                allX.VersionId = rAll["FILE_ALLEGATO"].ToString().Split('§')[1];
                                allX.MimeType = rAll["FILE_ALLEGATO"].ToString().Split('§')[2];
                                allX.VersionIdinDB = rAll["FILE_ALLEGATO"].ToString().Split('§')[3];
                                allX.PathName = rAll["FILE_ALLEGATO"].ToString().Split('§')[4];
                                allX.FileSize = rAll["FILE_ALLEGATO"].ToString().Split('§')[5];
                            }

                            allXs.Add(allX);
                        }
                        numAllegati += fromDBAll.Rows.Count;
                        if (allXs != null && allXs.Count > 0)
                            docX.Attachments = (Domain.C3File[])allXs.ToArray(typeof(Domain.File));
                    }

                    listaDocs.Add(docX);
                }

                response.Documents = (Domain.C3Document[])listaDocs.ToArray(typeof(Domain.C3Document));
                response.TotalAttachments = numAllegati.ToString();
                response.TotalDocuments = fromDB.Rows.Count.ToString();
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static bool BachecaCircolarePubblicata(VtDocsWS.Services.DocumentsAdvanced.CircolarePubblicata.CircolarePubblicataRequest request)
        {
            bool retval = false;
            try
            {
                if (!string.IsNullOrEmpty(request.Password) && request.Password == "Bacheca_20171010-ChgState!")
                {
                    ArrayList infoFiles = BusinessLogic.Amministrazione.SistemiEsterni.MIBACT_BACHECA_GetFileInfoDoc(request.EspiCodice);
                    if (infoFiles != null && infoFiles.Count > 0)
                    {
                        DocsPaVO.ExternalServices.MIGR_File_Info fileInfo = (DocsPaVO.ExternalServices.MIGR_File_Info)infoFiles[0];
                        DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtenteById(fileInfo.IdPeopleAutore);
                        DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuolo(fileInfo.IdCorrRuoloAutore);
                        DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                        DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                        try
                        {
                            if (!string.IsNullOrEmpty(request.EspiCodice))
                            {
                                documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.EspiCodice, request.EspiCodice);
                            }

                        }
                        catch (Exception ex)
                        {
                            throw new PisException("DOCUMENT_NOT_FOUND");
                        }

                        if (documento != null)
                        {
                            if (documento.template != null)
                            {
                                int idDiagramma = 0;
                                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                                DocsPaVO.DiagrammaStato.Stato statoAttuale = null;
                                idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociato(documento.template.SYSTEM_ID.ToString());
                                if (idDiagramma != 0)
                                {
                                    diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                                    if (diagramma != null)
                                    {
                                        if (diagramma.STATI != null && diagramma.STATI.Count > 0)
                                        {
                                            statoAttuale = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoDoc(documento.docNumber);
                                            bool result = false;


                                            if (statoAttuale == null)
                                            {
                                                foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                                {
                                                    if (stato.DESCRIZIONE.ToUpper().Equals("PUBBLICATA"))
                                                    {
                                                        BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(documento.docNumber, stato.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                                        result = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                for (int i = 0; i < diagramma.PASSI.Count; i++)
                                                {
                                                    DocsPaVO.DiagrammaStato.Passo step = (DocsPaVO.DiagrammaStato.Passo)diagramma.PASSI[i];
                                                    if (step.STATO_PADRE.SYSTEM_ID == statoAttuale.SYSTEM_ID)
                                                    {
                                                        for (int j = 0; j < step.SUCCESSIVI.Count; j++)
                                                        {
                                                            if (((DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j]).DESCRIZIONE.ToUpper().Equals("PUBBLICATA"))
                                                            {
                                                                BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(documento.docNumber, ((DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j]).SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                                                result = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (!result)
                                            {
                                                throw new PisException("STATEOFDIAGRAM_NOT_FOUND");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    throw new PisException("DIAGRAM_NOT_FOUND");
                                }
                            }
                            else
                            {
                                throw new PisException("TEMPLATE_NOT_FOUND");
                            }
                        }
                        else
                        {
                            throw new PisException("DOCUMENT_NOT_FOUND");
                        }
                        retval = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore " + ex);

            }

            return retval;
        }

        public static VtDocsWS.Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaResponse NuovaFatturaAttiva(VtDocsWS.Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaRequest request)
        {
            Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaResponse response = new Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaResponse();
            try
            {
                response.Success = true;
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.documento.SchedaDocumento schedaDoc = null, schedaDocResult = null;
                DocsPaVO.documento.Allegato allegatoRapprFattura = null;
                DocsPaVO.ExternalServices.FornitoreFattAttiva fornitoreFattAttiva = null;
                string passo = "", esito = "";
                string firmaCX = "";
                string stringaXml = null;
                int numPasso = 0;
                System.Xml.XmlDocument xmlDoc2 = null;
                DocsPaVO.utente.Registro regFattAttiva = null;

                #region Autenticazione
                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "NuovaFattura");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente
                //if (string.IsNullOrEmpty(request.CodeProject))
                //{
                //    throw new Exception("Codice del fascicolo richiesto");
                //}

                if (request != null && string.IsNullOrEmpty(request.Document.DocumentType))
                {
                    throw new PisException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && string.IsNullOrEmpty(request.Document.Object))
                {
                    throw new PisException("MISSING_OBJECT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (!request.Document.DocumentType.ToUpper().Equals("A") && !request.Document.DocumentType.ToUpper().Equals("P") && !request.Document.DocumentType.ToUpper().Equals("I") && !request.Document.DocumentType.ToUpper().Equals("G")))
                {
                    throw new PisException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (request.Document.DocumentType.ToUpper().Equals("A") || request.Document.DocumentType.ToUpper().Equals("P") || request.Document.DocumentType.ToUpper().Equals("I")) && string.IsNullOrEmpty(request.CodeRegister))
                {
                    throw new PisException("REQUIRED_REGISTER");
                }
                #endregion
                #region Controllo presenza fattura - commentato
                //if (!string.IsNullOrEmpty(request.Document.ConsolidationState))
                //{
                //    string idFatturaPresente = BusinessLogic.Amministrazione.SistemiEsterni.FattElCtrlDupl(request.Document.ConsolidationState, "", out passo, out esito);
                //    if (!string.IsNullOrEmpty(idFatturaPresente))
                //    {
                //        logger.DebugFormat("Fattura presente con DocNum {0}. Passo {1}. Esito {2}", idFatturaPresente, passo, esito);
                //        //throw new Exception(string.Format("Fattura con identificativo {0} già presente. Id PITre: {1}", request.Document.ConsolidationState, idFatturaPresente));
                //        if (esito.ToUpper() == "OK")
                //        {
                //            numPasso = 15;
                //            throw new Exception(string.Format("La fattura con IdInv {1} già presente con IdPiTre {0}.", idFatturaPresente, request.Document.ConsolidationState));
                //        }
                //        else if (esito.ToUpper() == "ERRORE")
                //        {
                //            schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idFatturaPresente, idFatturaPresente);
                //            schedaDocResult = schedaDoc;

                //            switch (passo.ToUpper())
                //            {
                //                case "CARICAMENTO FILE PRINCIPALE":
                //                    numPasso = 8;
                //                    break;
                //                case "CREAZIONE DOCUMENTO IN RISPOSTA":
                //                    numPasso = 9;
                //                    break;
                //                case "CARICAMENTO ALLEGATI":
                //                    numPasso = 10;
                //                    break;
                //                case "INSERIMENTO IN FASCICOLO":
                //                    numPasso = 11;
                //                    break;
                //                case "ASSOCIAZIONE TEMPLATE":
                //                    numPasso = 12;
                //                    break;
                //                case "ESECUZIONE TRASMISSIONI":
                //                    numPasso = 13;
                //                    break;
                //                default:
                //                    numPasso = 8;
                //                    break;
                //            }
                //        }

                //    }
                //}

                #endregion
                #region Controllo fornitore della fattura
                ArrayList Fornitori = BusinessLogic.Amministrazione.SistemiEsterni.FattElAttiveGetFornitori(infoUtente.idAmministrazione);
                byte[] fDocContent = null;
                if (request.Document != null && request.Document.MainDocument != null && request.Document.MainDocument.Content != null)
                {

                    if (request.Document.MainDocument.Name.ToUpper().EndsWith("P7M"))
                    {
                        //sbustare
                        DocsPaVO.documento.FileDocumento fdoc = new DocsPaVO.documento.FileDocumento();
                        fdoc.fullName = request.Document.MainDocument.Name;
                        fdoc.name = request.Document.MainDocument.Name;
                        fdoc.contentType = request.Document.MainDocument.MimeType;
                        fdoc.content = request.Document.MainDocument.Content;
                        fdoc.nomeOriginale = request.Document.MainDocument.Name;
                        fdoc.length = request.Document.MainDocument.Content.Length;
                        firmaCX = "C";
                        try
                        {
                            bool ctrlFirmaX = BusinessLogic.Documenti.FileManager.VerifyFileSignature(fdoc, DateTime.Now);
                            // Problema con il controllo della firma: 
                            // Sembra sia cambiato qualcosa nel DocsPaVO.documento.SignerInfo.CertificateInfo.RevocationStatus
                            //if (!ctrlFirmaX) throw new Exception("Errore nel controllo firma");

                            if (ctrlFirmaX) firmaCX = "C";
                        }
                        catch (Exception exXml)
                        {
                            // Vedo se c'è un errata codifica Base64
                            logger.Error("ERRORE nel file P7M: provo ad eliminare una codifica base64");

                            fdoc.content = Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(request.Document.MainDocument.Content));
                            try
                            {
                                bool ctrlFirmaX2 = BusinessLogic.Documenti.FileManager.VerifyFileSignature(fdoc, DateTime.Now);
                                if (!ctrlFirmaX2) throw new Exception("Errore nel controllo firma");
                                else firmaCX = "C";
                            }
                            catch (Exception exXml2)
                            {

                                throw new Exception("Errore nel file XML.P7M, elaborazione non possibile. Controllare la codifica del file.");
                            }
                        }
                        if (!fdoc.name.ToUpper().EndsWith("XML"))
                        {
                            throw new PisException("ELAB_XML_FILE_NOT_VALID");
                        }
                        else
                        {
                            stringaXml = System.Text.Encoding.UTF8.GetString(fdoc.content);
                            //stringaXml = System.Text.Encoding.Unicode.GetString(fdoc.content);
                            //var str = System.Text.Encoding.UTF8.GetString(fdoc.content);
                            //if(str.StartsWith("<"))
                            //{
                            //   str = System.Text.Encoding.Unicode.GetString(fdoc.content);
                            //}

                        }
                        //docPrincipale.Content = fdoc.content;
                        //docPrincipale.Name = fdoc.name;
                        fDocContent = fdoc.content;

                    }
                    if (string.IsNullOrEmpty(stringaXml))
                        stringaXml = System.Text.Encoding.UTF8.GetString(request.Document.MainDocument.Content);


                    stringaXml = stringaXml.Trim();
                    xmlDoc2 = new System.Xml.XmlDocument();
                    if (stringaXml.Contains("xml version=\"1.1\""))
                    {
                        logger.Debug("Versione XML 1.1. Provo conversione");
                        stringaXml = stringaXml.Replace("xml version=\"1.1\"", "xml version=\"1.0\"");
                    }
                    try
                    {
                        xmlDoc2.LoadXml(stringaXml);
                    }
                    catch (Exception bomUTF8)
                    {
                        string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                        if (stringaXml.StartsWith(byteOrderMarkUtf8))
                        {
                            stringaXml = stringaXml.Remove(0, byteOrderMarkUtf8.Length);
                        }
                        //xmlDoc2.LoadXml(stringaXml);

                        //potrebbe capitare che la codifica dell'xml è utf-16
                        try
                        {
                            xmlDoc2.LoadXml(stringaXml);
                        }
                        catch (Exception ex)
                        {
                            stringaXml = System.Text.Encoding.Unicode.GetString(fDocContent);
                            stringaXml = stringaXml.Trim();
                            xmlDoc2.LoadXml(stringaXml);
                        }
                    }
                    if (xmlDoc2.DocumentElement.NamespaceURI.ToLower().Contains("http://www.fatturapa.gov.it/sdi/fatturapa/v1") ||
                xmlDoc2.DocumentElement.NamespaceURI.ToLower().Contains("http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/"))
                    {
                        // controllo se la fattura è di uno dei fornitori in DB
                        bool isFattLottoAttivo = false;

                        string mappFornitore = string.Format("//*[name()='{0}']/*[name()='{1}']/*[name()='{2}']/*[name()='{3}']", "CedentePrestatore", "DatiAnagrafici", "IdFiscaleIVA", "IdCodice");
                        string codFornitore = (xmlDoc2.DocumentElement.SelectSingleNode(mappFornitore)).InnerXml;

                        if (!string.IsNullOrEmpty(codFornitore))
                        {
                            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                            ArrayList fornitori = dbAmm.FattElAttive_getCodFornitore(infoUtente.idAmministrazione);
                            //if (fornitori.Contains(codFornitore))
                            //    isFattLottoAttivo = true;                                       
                            foreach (DocsPaVO.ExternalServices.FornitoreFattAttiva forn in fornitori)
                            {
                                if (!string.IsNullOrEmpty(forn.CodFornitore) && forn.CodFornitore.ToUpper() == codFornitore.ToUpper())
                                {
                                    isFattLottoAttivo = true;
                                    fornitoreFattAttiva = forn;
                                }
                            }
                        }

                        if (!isFattLottoAttivo)
                        {
                            logger.Error("Fornitore della fattura o lotto attivo non corrispondente: " + codFornitore);
                            throw new Exception("Codice fornitore non corrispondente");
                        }
                        if (string.IsNullOrEmpty(firmaCX) && stringaXml.ToUpper().Contains("X509CERTIFICATE"))
                        {
                            firmaCX = "X";
                        }

                        if (string.IsNullOrEmpty(firmaCX))
                        {
                            System.Xml.XmlElement root = xmlDoc2.DocumentElement;
                            string tempOutX5 = root.Attributes["versione"].Value;
                            if (tempOutX5.ToUpper() != "FPR" && tempOutX5.ToUpper() != "FPR12")
                            {
                                logger.Info("FATTURA NON PRIVATA - Errore controllo firma");
                                throw new Exception("Fattura non firmata");
                            }
                            else
                                logger.Info("FATTURA PRIVATA - Bypass controllo firma");

                        }

                        System.Xml.XmlNodeList fatture = xmlDoc2.DocumentElement.SelectNodes("//*[name()='FatturaElettronicaBody']");
                        if (fatture.Count > 1)
                        {
                            throw new Exception("Lotto di fatture. Associazione a fattura elettronica interrotta.");

                        }

                        if (fornitoreFattAttiva != null)
                        {
                            regFattAttiva = BusinessLogic.Utenti.RegistriManager.getRegistro(fornitoreFattAttiva.IdRegistro);
                        }
                    }
                    else { throw new Exception("Il file principale non è una fattura."); }

                }

                #endregion
                #region Prelievo del titolario attivo
                string idTitolario = "";
                ArrayList titolari = new ArrayList();
                try
                {
                    titolari = BusinessLogic.Amministrazione.TitolarioManager.getTitolariUtilizzabili(infoUtente.idAmministrazione);
                }
                catch
                {
                    //Titolario attivo non trovato
                    throw new PisException("CLASSIFICATION_NOT_FOUND");
                }
                if (titolari != null && titolari.Count > 0)
                {
                    foreach (DocsPaVO.amministrazione.OrgTitolario tempTit in titolari)
                    {
                        if (tempTit.Stato == DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo)
                        {
                            idTitolario = tempTit.ID;
                            break;
                        }
                    }
                }
                #endregion
                #region Controllo del destinatario e della ragione della trasmissione - COMMENTATO
                if (request.TransmissionReceiver == null)
                {
                    //Destinatario non trovato
                    throw new PisException("REQUIRED_CORRESPONDENT");
                }
                if (string.IsNullOrEmpty(request.TransmissionReceiver.Code) && string.IsNullOrEmpty(request.TransmissionReceiver.Id))
                {
                    throw new PisException("REQUIRED_CODE_OR_ID_RECEIVER");
                }
                DocsPaVO.utente.Corrispondente corr = null;
                try
                {
                    if (!string.IsNullOrEmpty(request.TransmissionReceiver.Id))
                    {
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(request.TransmissionReceiver.Id);
                    }
                    else if (!string.IsNullOrEmpty(request.TransmissionReceiver.Code))
                    {
                        //corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(request.Receiver.Code, infoUtente);
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(request.TransmissionReceiver.Code, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                    }
                    if (corr == null)
                    {
                        throw new PisException("CORRESPONDENT_NOT_FOUND");
                    }

                }
                catch (Exception e)
                {
                    throw new PisException("CORRESPONDENT_NOT_FOUND");
                }

                DocsPaVO.trasmissione.RagioneTrasmissione ragione = null;
                if (request.TransmissionReason == null || string.IsNullOrEmpty(request.TransmissionReason))
                {
                    request.TransmissionReason = "RICEVIMENTO_FATTURA_ATTIVA";
                }

                try
                {
                    ragione = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, request.TransmissionReason.ToUpper());
                    if (ragione == null)
                        throw new PisException("TRANSMISSION_REASON_NOT_FOUND");
                }
                catch (Exception e)
                {
                    throw new PisException("TRANSMISSION_REASON_NOT_FOUND");
                }
                #endregion
                #region Controllo presenza fascicolo
                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();
                if (!string.IsNullOrEmpty(fornitoreFattAttiva.CodFascicolo))
                {
                    //Il codice è di un sottofascicolo
                    if (fornitoreFattAttiva.CodFascicolo.IndexOf("//") > -1)
                    {
                        string separatore = "//";
                        // MODIFICA per inserimento in sottocartelle
                        string[] separatoreAr = new string[] { separatore };

                        string[] pathCartelle = fornitoreFattAttiva.CodFascicolo.Split(separatoreAr, StringSplitOptions.RemoveEmptyEntries);

                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloDaCodice(infoUtente, pathCartelle[0], null, false, false);
                        if (fascicolo != null)
                        {
                            if (pathCartelle.Length > 1)
                            {
                                ArrayList cartelle = BusinessLogic.Fascicoli.FascicoloManager.getListaFolderDaIdFascicolo(infoUtente, fascicolo.systemID, null, false, false);
                                // caricamento folder base
                                string idParentFolder = fascicolo.systemID;
                                fascicolo.folderSelezionato = (DocsPaVO.fascicolazione.Folder)((from DocsPaVO.fascicolazione.Folder f in cartelle where f.idParent == idParentFolder select f).ToList().FirstOrDefault());
                                idParentFolder = fascicolo.folderSelezionato.systemID;
                                try
                                {
                                    for (int i = 1; i < pathCartelle.Length; i++)
                                    {

                                        fascicolo.folderSelezionato = (DocsPaVO.fascicolazione.Folder)((from DocsPaVO.fascicolazione.Folder f in cartelle where f.idParent == idParentFolder && f.descrizione == pathCartelle[i] select f).ToList().First());
                                        idParentFolder = fascicolo.folderSelezionato.systemID;
                                    }
                                }
                                catch (Exception e)
                                {
                                    throw new Exception("Cartella non trovata nel fascicolo");
                                }
                            }
                        }
                        else
                        {
                            throw new PisException("PROJECT_NOT_FOUND");
                        }


                    }
                    else
                    {
                        ArrayList fascicoli = new ArrayList();
                        fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliDaCodice(infoUtente, fornitoreFattAttiva.CodFascicolo, null, false, false, "I");
                        if (fascicoli != null && fascicoli.Count > 0)
                        {
                            if (fascicoli.Count == 1)
                            {
                                fascicolo = (DocsPaVO.fascicolazione.Fascicolo)fascicoli[0];
                            }
                            else
                            {
                                //Fascicoli multipli
                                throw new PisException("MULTIPLE");
                            }
                        }
                        else
                        {
                            //Fascicolo non trovato
                            throw new PisException("PROJECT_NOT_FOUND");
                        }
                    }
                }


                #endregion
                #region Creazione Scheda Documento
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                DocsPaVO.utente.Corrispondente mittente = new DocsPaVO.utente.Corrispondente();

                try
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                    if (ruolo == null)
                    {
                        //Ruolo non trovato
                        throw new PisException("ROLE_NO_EXIST");
                    }
                }
                catch
                {
                    //Ruolo non trovato
                    throw new PisException("ROLE_NO_EXIST");
                }

                if (numPasso < 8)
                {
                    // **************************************************************************
                    // Per accordi con il cliente viene effettuato questo controllo:
                    // se Type == "O" allore viene impostato il valore di CorrespondentType = "O"
                    // Controllo effettuato per: Sender, Recipients, RecipientsCC
                    // **************************************************************************
                    //
                    if (request.Document.Sender != null && !string.IsNullOrEmpty(request.Document.Sender.Type) && request.Document.Sender.Type.Equals("O"))
                        request.Document.Sender.CorrespondentType = "O";

                    if (request.Document.MultipleSenders != null)
                    {
                        for (int i = 0; i < request.Document.MultipleSenders.Length; i++)
                        {
                            Domain.Correspondent c = request.Document.MultipleSenders[i];
                            if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                                c.CorrespondentType = "O";
                        }
                    }

                    if (request.Document.Recipients != null)
                    {
                        for (int i = 0; i < request.Document.Recipients.Length; i++)
                        {
                            Domain.Correspondent c = request.Document.Recipients[i];
                            if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                                c.CorrespondentType = "O";
                        }
                    }

                    if (request.Document.RecipientsCC != null)
                    {
                        for (int i = 0; i < request.Document.RecipientsCC.Length; i++)
                        {
                            Domain.Correspondent c = request.Document.RecipientsCC[i];
                            if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                                c.CorrespondentType = "O";
                        }
                    }
                    //


                    ///Creazione schedaDocumento con i dati essenziali e obbligatori
                    schedaDoc = BusinessLogic.Documenti.DocManager.NewSchedaDocumento(infoUtente, false);

                    ///Imposto l'oggetto
                    if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.Object))
                    {
                        schedaDoc.oggetto.descrizione = request.Document.Object;
                        schedaDoc.oggetto.daAggiornare = true;
                    }

                    //Imposto le note, soltanto visibili a tutti
                    if (request != null && request.Document != null && request.Document != null && request.Document.Note != null && request.Document.Note.Length > 0)
                    {
                        schedaDoc.noteDocumento = new List<DocsPaVO.Note.InfoNota>();
                        foreach (Domain.Note nota in request.Document.Note)
                        {
                            DocsPaVO.Note.InfoNota tempNot = new DocsPaVO.Note.InfoNota();
                            tempNot.DaInserire = true;
                            tempNot.Testo = nota.Description;
                            tempNot.TipoVisibilita = DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti;
                            tempNot.UtenteCreatore = new DocsPaVO.Note.InfoUtenteCreatoreNota();
                            tempNot.UtenteCreatore.IdRuolo = infoUtente.idGruppo;
                            tempNot.UtenteCreatore.IdUtente = infoUtente.idPeople;
                            schedaDoc.noteDocumento.Add(tempNot);
                        }
                    }
                    //

                    ///Imposto il template
                    if (request != null && request.Document != null && request.Document.Template != null && (!string.IsNullOrEmpty(request.Document.Template.Id) || (!string.IsNullOrEmpty(request.Document.Template.Name))))
                    {
                        DocsPaVO.ProfilazioneDinamica.Templates template = null;
                        if (!string.IsNullOrEmpty(request.Document.Template.Id))
                        {
                            template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(request.Document.Template.Id);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(request.Document.Template.Name))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(request.Document.Template.Name, infoUtente.idAmministrazione);
                            }
                        }

                        if (template != null)
                        {
                            //Modifica Lembo 10-01-2013: Controllo se l'utente è abilitato all'utilizzo del template.
                            //Solo ruoli con diritti di scrittura ammessi.
                            string controlFilter = string.Format("{0} and id_ruolo={1} and (diritti='2')", template.SYSTEM_ID.ToString(), infoUtente.idGruppo);
                            ArrayList controlloVisibilita = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getRuoliTipoDoc(controlFilter);
                            if (controlloVisibilita == null || controlloVisibilita.Count == 0)
                            {
                                throw new PisException("TEMPLATE_NOT_ROLE_EDITABLE");
                            }
                            // Modifica Lembo 11-01-2013: Utilizzo il metodo che controlla la visibilità dei campi.
                            //schedaDoc.template = Utils.GetTemplateFromPis(request.Document.Template, template, false);

                            // Modifica Elaborazione XML da PIS req.2

                            Domain.File fileDaPassare = ((request.Document.MainDocument != null && request.Document.MainDocument.Content != null) ? request.Document.MainDocument : null);

                            schedaDoc.template = Utils.GetTemplateFromPisVisibility(request.Document.Template, template, false, infoUtente.idGruppo, "D", request.CodeApplication, infoUtente
                                , fileDaPassare,
                                regFattAttiva.chaRF == "1" ? null : regFattAttiva.codRegistro,
                                regFattAttiva.chaRF == "0" ? null : regFattAttiva.codRegistro,
                                false, request.IdentificativoSdI);
                            schedaDoc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                            schedaDoc.tipologiaAtto.systemId = template.SYSTEM_ID.ToString();
                            schedaDoc.tipologiaAtto.descrizione = template.DESCRIZIONE.ToString();

                            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggcustom in schedaDoc.template.ELENCO_OGGETTI)
                            {
                                if (oggcustom.CONTATORE_DA_FAR_SCATTARE == true)
                                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTO_REPERTORIATO", schedaDoc.systemId, "Repertoriato documento", DocsPaVO.Logger.CodAzione.Esito.OK);
                            }

                            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggcustom in schedaDoc.template.ELENCO_OGGETTI)
                            {
                                if (oggcustom.CONTATORE_DA_FAR_SCATTARE == true)
                                    oggcustom.CONTATORE_DA_FAR_SCATTARE = false;
                            }

                        }
                        else
                        {
                            //Template non trovato
                            throw new PisException("TEMPLATE_NOT_FOUND");
                        }
                    }
                    ///

                    ///Se privato
                    if (request != null && request.Document != null && request.Document.PrivateDocument)
                    {
                        schedaDoc.privato = "1";
                    }
                    ///

                    ///Se personale
                    if (request != null && request.Document != null && request.Document.PersonalDocument)
                    {
                        schedaDoc.personale = "1";
                    }
                    ///

                    DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione = new DocsPaVO.documento.ResultProtocollazione();
                    bool daAggiornareUffRef = false;

                    //CASO DOCUMENTO GRIGIO (NON PROTOCOLLATO)
                    if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("G"))
                    {
                        schedaDoc.tipoProto = "G";
                        schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                    }
                    //

                    //Controlli solo per protocolli
                    if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && !request.Document.DocumentType.ToUpper().Equals("G"))
                    {

                        if (request.Document.Sender == null || string.IsNullOrEmpty(request.Document.Sender.CorrespondentType))
                        {
                            if (!request.Document.DocumentType.ToUpper().Equals("P"))
                            {
                                //Mittente non presente
                                throw new PisException("REQUIRED_SENDER");
                            }
                            else
                            {
                                mittente = null;
                            }
                        }
                        else
                        {
                            if (request.Document.Sender != null && !string.IsNullOrEmpty(request.Document.Sender.CorrespondentType) && request.Document.Sender.CorrespondentType.Equals("O"))
                            {
                                mittente = Utils.GetCorrespondentFromPis(request.Document.Sender, infoUtente);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(request.Document.Sender.Id))
                                {
                                    mittente = Utils.RisolviCorrispondente(request.Document.Sender.Id, infoUtente);
                                    if (mittente == null)
                                        //Mittente non trovato
                                        throw new PisException("SENDER_NOT_FOUND");
                                }
                            }
                        }

                        if (request.Document.DocumentType.ToUpper().Equals("I") || request.Document.DocumentType.ToUpper().Equals("P"))
                        {
                            if (request.Document.Recipients == null || request.Document.Recipients.Length == 0)
                            {
                                //Destinatario non presente
                                throw new PisException("REQUIRED_RECIPIENT");
                            }
                        }

                        if (string.IsNullOrEmpty(request.CodeRegister))
                        {
                            //Registro mancante
                            throw new PisException("REQUIRED_REGISTER");
                        }
                        else
                        {
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeRegister);
                            if (reg == null)
                            {
                                //Registro mancante
                                throw new PisException("REGISTER_NOT_FOUND");
                            }
                            else
                            {
                                schedaDoc.registro = reg;
                            }
                        }

                        if (!string.IsNullOrEmpty(request.CodeRF))
                        {
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeRF);
                            if (reg != null)
                            {
                                schedaDoc.id_rf_prot = reg.systemId;
                                schedaDoc.id_rf_invio_ricevuta = reg.systemId;
                                schedaDoc.cod_rf_prot = reg.codRegistro;
                            }
                            else
                            {
                                //RF non trovato
                                throw new PisException("RF_NOT_FOUND");
                            }
                        }
                    }

                    //CASO PROTOCOLLO IN ARRIVO (ANCHE PREDISPOSTO)
                    if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("A"))
                    {
                        schedaDoc.tipoProto = "A";
                        schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloEntrata();

                        ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente = mittente;

                        if (request.Document.MultipleSenders != null && request.Document.MultipleSenders.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittenti = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.MultipleSenders)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = null;
                                if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                                {
                                    corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(corrTemp.Id))
                                    {
                                        corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                        if (corrBySys == null)
                                        {
                                            //Mittente non trovato
                                            throw new PisException("SENDER_NOT_FOUND");
                                        }
                                    }
                                }
                                ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittenti.Add(corrBySys);
                            }
                        }

                        //Data protocollo mittente
                        if (!string.IsNullOrEmpty(request.Document.DataProtocolSender))
                        {
                            ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).dataProtocolloMittente = request.Document.DataProtocolSender;
                        }

                        //Protocollo mittente
                        if (!string.IsNullOrEmpty(request.Document.ProtocolSender))
                        {
                            ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).descrizioneProtocolloMittente = request.Document.ProtocolSender;
                        }

                        //Data arrivo TODO
                        if (!string.IsNullOrEmpty(request.Document.ArrivalDate))
                        {
                            // Variabile Booleana per il controllo di validità della data passata in input;
                            bool valid = false;
                            // Data ritornata in output dal TryParseExact per la data arrivo passata in input
                            DateTime dateVal;

                            // Pattern di validità per una data valida
                            string pattern = "dd/MM/yyyy HH:mm";

                            try
                            {
                                /*
                                 *  Date Format Example: 01/03/2012
                                 *  dd = day (01)
                                 *  MM = month (03)
                                 *  yyyy = year (2012)
                                 *  / = date separator
                                 */

                                if (!DateTime.TryParseExact(request.Document.ArrivalDate, pattern, null, System.Globalization.DateTimeStyles.None, out dateVal))
                                {
                                    throw new Exception("Formato Data non corretto");
                                }

                                valid = true;
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Formato Data non corretto");
                            }

                            if (valid)
                            {
                                // Controllo che la data inserita non sia posteriore alla data odierna
                                DateTime dateVal2;
                                // Variabile Booleana per il controllo di validità della data DateTime.Now trasformato nel formato dd/MM/yyyy;
                                bool dateNow = true;

                                if (!DateTime.TryParseExact(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), pattern, null, System.Globalization.DateTimeStyles.None, out dateVal2))
                                {
                                    // Non dovrebbe entrare mai in questo controllo.
                                    dateNow = false;
                                    throw new Exception("Formato Data non corretto");
                                }

                                if (dateNow)
                                {
                                    // dateVal: data passata in input;
                                    // dateVal2: DateTime.Now convertito nel formato dd/MM/yyyy
                                    int result = DateTime.Compare(dateVal, dateVal2);

                                    if (result > 0)
                                        // Data di input è successiva alla data odierna;
                                        //((Documento)schedaDoc.documenti[0]).dataArrivo = string.Empty;
                                        throw new Exception("La data di arrivo non può essere posteriore alla data di segnatura");
                                    else
                                        // Data di input valida e conforme;
                                        ((DocsPaVO.documento.Documento)schedaDoc.documenti[0]).dataArrivo = request.Document.ArrivalDate;
                                }
                            }
                        }

                        if (request.Document.Predisposed)
                        {
                            schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                            if (schedaDocResult != null)
                            {
                                schedaDocResult.predisponiProtocollazione = true;
                                schedaDocResult.tipoProto = request.Document.DocumentType;
                                schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                            }
                        }
                        else
                        {
                            schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out risultatoProtocollazione);
                        }

                        if (schedaDocResult != null)
                        {
                            //Mezzo di spedizione
                            if (!string.IsNullOrEmpty(request.Document.MeansOfSending))
                            {
                                ArrayList mezziSpedizione = BusinessLogic.Amministrazione.MezziSpedizioneManager.ListaMezziSpedizione(infoUtente.idAmministrazione, true);
                                foreach (DocsPaVO.amministrazione.MezzoSpedizione m_spediz in mezziSpedizione)
                                {
                                    if (m_spediz.Descrizione.ToUpper().Equals(request.Document.MeansOfSending.ToUpper()))
                                    {
                                        BusinessLogic.Documenti.ProtoManager.collegaMezzoSpedizioneDocumento(infoUtente, m_spediz.IDSystem, schedaDocResult.systemId);
                                        break;
                                    }
                                }

                            }
                        }
                    }
                    //

                    //CASO PROTOCOLLO IN PARTENZA (ANCHE PREDISPOSTO)
                    if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("P"))
                    {
                        schedaDoc.tipoProto = "P";
                        schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloUscita();

                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente = mittente;

                        if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.Recipients)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = null;

                                // Verifica se occasionale
                                if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                                {
                                    corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                }
                                else
                                {
                                    // Se non è occasionale, lo risolve sulla rubrica
                                    corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                    if (corrBySys == null)
                                    {
                                        //Destinatario non trovato
                                        throw new PisException("RECIPIENT_NOT_FOUND");
                                    }

                                }
                                if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "F" && corrBySys.tipoIE == null)
                                {
                                    ArrayList corrByRF = BusinessLogic.Rubrica.RF.getCorrispondentiByCodRF(corrBySys.codiceRubrica);
                                    foreach (DocsPaVO.utente.Corrispondente c2 in corrByRF)
                                    {
                                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(c2);
                                    }
                                }
                                else if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                                {
                                    ArrayList corrByLista = BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiByCodLista(corrBySys.codiceRubrica, infoUtente.idAmministrazione, infoUtente);
                                    foreach (DocsPaVO.utente.Corrispondente c3 in corrByLista)
                                    {
                                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(c3);
                                    }
                                }
                                else
                                {
                                    ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(corrBySys);
                                }
                            }
                        }

                        if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.RecipientsCC)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = null;

                                // Verifica se occasionale
                                if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                                {
                                    corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                }
                                else
                                {
                                    corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                    if (corrBySys == null)
                                    {
                                        //Destinatario non trovato
                                        throw new PisException("RECIPIENT_NOT_FOUND");
                                    }
                                }
                                if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "F" && corrBySys.tipoIE == null)
                                {
                                    ArrayList corrByRF = BusinessLogic.Rubrica.RF.getCorrispondentiByCodRF(corrBySys.codiceRubrica);
                                    foreach (DocsPaVO.utente.Corrispondente c2 in corrByRF)
                                    {
                                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(c2);
                                    }
                                }
                                else if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                                {
                                    ArrayList corrByLista = BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiByCodLista(corrBySys.codiceRubrica, infoUtente.idAmministrazione, infoUtente);
                                    foreach (DocsPaVO.utente.Corrispondente c3 in corrByLista)
                                    {
                                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(c3);
                                    }
                                }
                                else
                                {
                                    ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
                                }

                                //((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
                            }
                        }

                        if (request.Document.Predisposed)
                        {
                            schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                            if (schedaDocResult != null)
                            {
                                schedaDocResult.predisponiProtocollazione = true;
                                schedaDocResult.tipoProto = request.Document.DocumentType;
                                schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                            }
                        }
                        else
                        {
                            schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out risultatoProtocollazione);
                        }
                    }
                    //

                    //CASO PROTOCOLLO INTERNO (ANCHE PREDISPOSTO)
                    if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("I"))
                    {
                        schedaDoc.tipoProto = "I";
                        schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloInterno();

                        ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).mittente = mittente;

                        if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatari = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.Recipients)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new PisException("RECIPIENT_NOT_FOUND");
                                }
                                ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatari.Add(corrBySys);
                            }
                        }

                        if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.RecipientsCC)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new PisException("RECIPIENT_NOT_FOUND");
                                }
                                ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
                            }
                        }

                        if (request.Document.Predisposed)
                        {
                            schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                            if (schedaDocResult != null)
                            {
                                schedaDocResult.predisponiProtocollazione = true;
                                schedaDocResult.tipoProto = request.Document.DocumentType;
                                schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                            }
                        }
                        else
                        {
                            schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out risultatoProtocollazione);
                        }
                    }
                    //

                    if (!string.IsNullOrEmpty(request.IdLottoPITre))
                    {
                        int testIdLotto = 0;
                        if (Int32.TryParse(request.IdLottoPITre, out testIdLotto))
                        {
                            BusinessLogic.Documenti.DocManager.UpdateRispostaProtocollo(request.IdLottoPITre, schedaDocResult.docNumber);
                        }
                        else
                        {
                            throw new Exception("Valore non valido per il campo IdLottoPI3");
                        }
                    }

                    //COMMENTATO- BISOGNA FARLO DOPO

                    //foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggcustom in schedaDoc.template.ELENCO_OGGETTI)
                    //{
                    //    if (oggcustom.CONTATORE_DA_FAR_SCATTARE == true && oggcustom.REPERTORIO == "1")
                    //        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTO_REPERTORIATO", schedaDoc.systemId, "Repertoriato documento", DocsPaVO.Logger.CodAzione.Esito.OK);
                    //}

                    // Log creazione documento
                    if (schedaDocResult != null)
                    {
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTO_CREATO", schedaDoc.systemId, "Creato documento", DocsPaVO.Logger.CodAzione.Esito.OK);
                    }


                #endregion
                    #region Inserimento nella tabella di log delle fatture - commentato
                    //bool insLogOk = BusinessLogic.Amministrazione.SistemiEsterni.FattElInsTabellaLog(schedaDocResult.systemId, request.Document.ConsolidationState, "");
                }
                    #endregion
                #region Caricamento File Principale
                try
                {
                    if (schedaDocResult != null && !string.IsNullOrEmpty(schedaDocResult.docNumber) && request.Document.MainDocument != null && numPasso < 9)
                    {
                        //Crea il file o se esistente una nuova versione
                        // 5. Acquisizione del file al documento
                        DocsPaVO.documento.FileRequest versioneCorrente = (DocsPaVO.documento.FileRequest)schedaDocResult.documenti[0];

                        DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                        {
                            name = request.Document.MainDocument.Name,
                            fullName = request.Document.MainDocument.Name,
                            content = request.Document.MainDocument.Content,
                            contentType = request.Document.MainDocument.MimeType,
                            length = request.Document.MainDocument.Content.Length,
                            bypassFileContentValidation = true
                        };

                        string errorMessage;

                        if (!BusinessLogic.Documenti.FileManager.putFile(ref versioneCorrente, fileDocumento, infoUtente, out errorMessage))
                        {
                            throw new PisException("FILE_CREATION_ERROR");
                        }
                        else
                        {
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "FILE_CARICATO", schedaDoc.systemId, "File caricato", DocsPaVO.Logger.CodAzione.Esito.OK);
                        }


                    }
                }
                catch (Exception exFilePrincipale)
                {
                    //bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, "", "CARICAMENTO FILE PRINCIPALE", "ERRORE");
                    throw new Exception("Errore al passo CARICAMENTO FILE PRINCIPALE");
                }
                #endregion
                #region Creazione Documento in risposta
                if (schedaDocResult != null)
                {
                    response.Document = Utils.GetDetailsDocument(schedaDocResult, infoUtente, false);
                    if (!string.IsNullOrEmpty(infoUtente.diSistema) && infoUtente.diSistema == "1")
                    {
                        bool pulito = Utils.CleanRightsExtSys(schedaDocResult.docNumber, infoUtente.idPeople, infoUtente.idGruppo);
                    }
                }
                response.Success = true;
                #endregion
                #region Caricamento allegati - DA MODIFICARE
                try
                {
                    if (request.Document.Attachments != null && request.Document.Attachments.Length > 0 && numPasso < 11)
                    {
                        string erroreMessage = "";
                        foreach (Domain.File all in request.Document.Attachments)
                        {
                            DocsPaVO.documento.Allegato allegato = new DocsPaVO.documento.Allegato
                            {
                                docNumber = schedaDocResult.systemId,
                                descrizione = all.Description
                            };

                            // Acquisizione dell'allegato
                            allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, allegato);
                            //definisco l'allegato come esterno
                            try
                            {
                                if (!string.IsNullOrEmpty(all.VersionId) && all.VersionId == "E")
                                {
                                    if (!BusinessLogic.Documenti.AllegatiManager.setFlagAllegatiEsterni(allegato.versionId, allegato.docNumber))
                                    {
                                        BusinessLogic.Documenti.AllegatiManager.rimuoviAllegato(allegato, infoUtente);
                                        throw new PisException("ERROR_ADD_ATTACHMENT");
                                    }
                                    else
                                    {
                                        if (all.Description.ToUpper() == "FATTURA")
                                            allegatoRapprFattura = allegato;
                                    }
                                }
                            }
                            catch (PisException pisEx)
                            {
                                response.Error = new Services.ResponseError
                                {
                                    Code = pisEx.ErrorCode,
                                    Description = pisEx.Description
                                };

                                response.Success = false;
                                return response;
                            }

                            // Acquisizione file allegato
                            // 5. Acquisizione del file al documento
                            DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                            {
                                name = all.Name,
                                fullName = all.Name,
                                content = all.Content,
                                contentType = all.MimeType,
                                length = all.Content.Length,
                                bypassFileContentValidation = true
                            };
                            DocsPaVO.documento.FileRequest allegatoFR = allegato;

                            if (!BusinessLogic.Documenti.FileManager.putFile(ref allegatoFR, fileDocumento, infoUtente, out erroreMessage))
                            {
                                throw new PisException("FILE_CREATION_ERROR");
                            }
                        }
                    }

                    BusinessLogic.Amministrazione.SistemiEsterni.FattElEstrAllegatiFE(schedaDocResult, ruolo, infoUtente);
                }
                catch (Exception exCaricaAllegati)
                {
                    //bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, "", "CARICAMENTO ALLEGATI", "ERRORE");
                    throw new Exception("Errore al passo CARICAMENTO ALLEGATI");
                }

                #endregion
                #region Inserimento in fascicolo
                try
                {
                    string msg = "";
                    if (numPasso < 12)
                    {
                        if (fascicolo != null)
                        {
                            //Caso del sottofascicolo
                            if (fascicolo.folderSelezionato != null)
                            {
                                BusinessLogic.Fascicoli.FolderManager.addDocFolder(infoUtente, schedaDocResult.systemId, fascicolo.folderSelezionato.systemID, false, out msg);
                            }
                            else
                            {
                                BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, schedaDocResult.systemId, fascicolo.systemID, false, out msg);
                            }
                        }
                        else
                        {
                            //Fascicolo non trovato
                            throw new PisException("PROJECT_NOT_FOUND");
                        }
                    }
                }
                catch (Exception exInFasc)
                {
                    //bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, "", "INSERIMENTO IN FASCICOLO", "ERRORE");
                    throw new Exception("Errore al passo INSERIMENTO IN FASCICOLO");
                }
                #endregion
                #region Associazione template
                try
                {
                    if (numPasso < 13)
                    {
                        if (schedaDocResult != null)
                        {
                            DocsPaVO.ProfilazioneDinamica.Templates template = null;

                            if (!string.IsNullOrEmpty(request.Document.Template.Id))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(request.Document.Template.Id);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(request.Document.Template.Name))
                                {
                                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(request.Document.Template.Name, infoUtente.idAmministrazione);
                                }
                            }
                            if (template != null)
                            {
                                Domain.File fileDaPassare = ((request.Document.MainDocument != null && request.Document.MainDocument.Content != null) ? request.Document.MainDocument : null);

                                schedaDocResult.template = Utils.GetTemplateFromPisVisibility(request.Document.Template, template, false, infoUtente.idGruppo, "D", request.CodeApplication, infoUtente, fileDaPassare, regFattAttiva.chaRF == "1" ? null : regFattAttiva.codRegistro, regFattAttiva.chaRF == "0" ? null : regFattAttiva.codRegistro,
                                 false, request.IdentificativoSdI);
                                schedaDocResult.daAggiornareTipoAtto = true;
                                schedaDocResult.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                                schedaDocResult.tipologiaAtto.systemId = schedaDocResult.template.SYSTEM_ID.ToString();
                                schedaDocResult.tipologiaAtto.descrizione = schedaDocResult.template.DESCRIZIONE.ToString();

                                bool daAggiornareUffRef = false;

                                schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                                if (schedaDocResult == null)
                                {
                                    throw new PisException("ASS_TEMPLATE_ERROR");
                                }

                                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggcustom in schedaDocResult.template.ELENCO_OGGETTI)
                                {
                                    if (oggcustom.CONTATORE_DA_FAR_SCATTARE == true && oggcustom.REPERTORIO == "1")
                                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTO_REPERTORIATO", schedaDoc.systemId, "Repertoriato documento", DocsPaVO.Logger.CodAzione.Esito.OK);
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, "", "ASSOCIAZIONE TEMPLATE", "ERRORE");
                    logger.Error("Errore in ASSOCIAZIONE TEMPLATE :" + ex.Message);
                    throw new Exception("Errore in ASSOCIAZIONE TEMPLATE");
                }
                #endregion
                #region Esecuzione trasmissione - COMMENTATO
                try
                {
                    if (numPasso < 14)
                    {
                        string trasmType = "S";
                        if (!string.IsNullOrEmpty(request.TransmissionType) && request.TransmissionType == "T")
                        {
                            trasmType = "T";
                        }

                        //creazione oggetto trasmissione e popolamento campi.
                        DocsPaVO.trasmissione.Trasmissione transmission = new DocsPaVO.trasmissione.Trasmissione();
                        // Creazione dell'oggetto trasmissione
                        transmission = new DocsPaVO.trasmissione.Trasmissione();

                        // Impostazione dei parametri della trasmissione
                        // Note generali
                        // transmission.noteGenerali = ragione.note;

                        // Tipo oggetto
                        transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;

                        // Informazioni sul documento
                        transmission.infoDocumento = BusinessLogic.Documenti.DocManager.getInfoDocumento(schedaDocResult);

                        // Utente mittente della trasmissione
                        transmission.utente = utente;

                        // Ruolo mittente
                        // Se inserito in ingresso il CodeRole utilizzo il coderole (WIP)
                        transmission.ruolo = ruolo;
                        string generaNotifica = "1";
                        if (request.TransmissionNotify != null && !request.TransmissionNotify)
                        {
                            transmission.NO_NOTIFY = "1";
                            generaNotifica = "0";
                        }
                        else
                        {
                            transmission.NO_NOTIFY = "0";
                            generaNotifica = "1";
                        }
                        // Modifica per invio in mail dell'url del frontend
                        string urlfrontend = "";
                        if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                            urlfrontend = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();

                        transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(
                            transmission, corr, ragione, "", trasmType);
                        // il tipo trasm è S

                        DocsPaVO.trasmissione.Trasmissione resultTrasm = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(urlfrontend, transmission);
                        response.Success = true;
                        string desc = string.Empty;
                        // LOG per documento
                        if (resultTrasm.infoDocumento != null && !string.IsNullOrEmpty(resultTrasm.infoDocumento.idProfile))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in resultTrasm.trasmissioniSingole)
                            {
                                string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                if (resultTrasm.infoDocumento.segnatura == null)
                                    desc = "Trasmesso Documento : " + resultTrasm.infoDocumento.docNumber.ToString();
                                else
                                    desc = "Trasmesso Documento : " + resultTrasm.infoDocumento.segnatura.ToString();

                                BusinessLogic.UserLog.UserLog.WriteLog(resultTrasm.utente.userId, resultTrasm.utente.idPeople, resultTrasm.ruolo.idGruppo, resultTrasm.utente.idAmministrazione, method, resultTrasm.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                            }
                        }
                        // LOG per fascicolo
                        if (resultTrasm.infoFascicolo != null && !string.IsNullOrEmpty(resultTrasm.infoFascicolo.idFascicolo))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in resultTrasm.trasmissioniSingole)
                            {
                                string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                desc = "Trasmesso Fascicolo ID: " + resultTrasm.infoFascicolo.idFascicolo.ToString();
                                BusinessLogic.UserLog.UserLog.WriteLog(resultTrasm.utente.userId, resultTrasm.utente.idPeople, resultTrasm.ruolo.idGruppo, resultTrasm.utente.idAmministrazione, method, resultTrasm.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                            }
                        }
                    }
                }
                catch (Exception exTrasm)
                {
                    //bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, "", "ESECUZIONE TRASMISSIONI", "ERRORE");
                    throw new Exception("Errore al passo ESECUZIONE TRASMISSIONI");
                }
                // Inserire inserimento in log fatture attive
                BusinessLogic.Amministrazione.SistemiEsterni.FattElAttive_InsertInLogPIS(schedaDocResult.systemId, corr.systemId, corr.codiceRubrica);
                BusinessLogic.Amministrazione.SistemiEsterni.FattElAttive_UpSecProprietario(schedaDocResult.systemId);
                #endregion
                #region Update tabella di log delle fatture con status OK - commentato
                //if (!string.IsNullOrEmpty(request.Document.ConsolidationState))
                //{
                //    bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, "", "", "OK");
                //}
                #endregion
                #region Scambio del xml con la rappresentazione fattura - commentato
                //BusinessLogic.Documenti.AllegatiManager.scambiaAllegatoDocumento(infoUtente, allegatoRapprFattura, (DocsPaVO.documento.Documento)schedaDocResult.documenti[0]);

                #endregion

            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                //bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, pisEx.Description, "ERRORE");
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                //bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, ex.Message, "ERRORE");
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static VtDocsWS.Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Response NuovoLottoAttivo(VtDocsWS.Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Request request)
        {
            Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Response response = new Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Response();
            try
            {
                response.Success = true;
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.documento.SchedaDocumento schedaDoc = null, schedaDocResult = null;
                DocsPaVO.ExternalServices.FornitoreFattAttiva fornitoreFattAttiva = null;

                string passo = "", esito = "";
                string firmaCX = "";
                DocsPaVO.utente.Registro regFattAttiva = null;

                int numPasso = 0;

                #region Autenticazione
                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "CaricaLottoInPi3");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                DocsPaVO.utente.Corrispondente corr = null;
                DocsPaVO.trasmissione.RagioneTrasmissione ragione = null;

                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente
                //if (string.IsNullOrEmpty(request.CodeProject))
                //{
                //    throw new Exception("Codice del fascicolo richiesto");
                //}

                if (request != null && string.IsNullOrEmpty(request.Document.DocumentType))
                {
                    throw new PisException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && string.IsNullOrEmpty(request.Document.Object))
                {
                    throw new PisException("MISSING_OBJECT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (!request.Document.DocumentType.ToUpper().Equals("A") && !request.Document.DocumentType.ToUpper().Equals("P") && !request.Document.DocumentType.ToUpper().Equals("I") && !request.Document.DocumentType.ToUpper().Equals("G")))
                {
                    throw new PisException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (request.Document.DocumentType.ToUpper().Equals("A") || request.Document.DocumentType.ToUpper().Equals("P") || request.Document.DocumentType.ToUpper().Equals("I")) && string.IsNullOrEmpty(request.CodeRegister))
                {
                    throw new PisException("REQUIRED_REGISTER");
                }
                #endregion
                #region Prelievo del titolario attivo
                string idTitolario = "";
                ArrayList titolari = new ArrayList();
                try
                {
                    titolari = BusinessLogic.Amministrazione.TitolarioManager.getTitolariUtilizzabili(infoUtente.idAmministrazione);
                }
                catch
                {
                    //Titolario attivo non trovato
                    throw new PisException("CLASSIFICATION_NOT_FOUND");
                }
                if (titolari != null && titolari.Count > 0)
                {
                    foreach (DocsPaVO.amministrazione.OrgTitolario tempTit in titolari)
                    {
                        if (tempTit.Stato == DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo)
                        {
                            idTitolario = tempTit.ID;
                            break;
                        }
                    }
                }
                #endregion
                #region Controllo fornitore della fattura
                ArrayList Fornitori = BusinessLogic.Amministrazione.SistemiEsterni.FattElAttiveGetFornitori(infoUtente.idAmministrazione);
                if (request.Document != null && request.Document.MainDocument != null && request.Document.MainDocument.Content != null)
                {
                    string stringaXml = null;
                    if (request.Document.MainDocument.Name.ToUpper().EndsWith("P7M"))
                    {
                        //sbustare
                        DocsPaVO.documento.FileDocumento fdoc = new DocsPaVO.documento.FileDocumento();
                        fdoc.fullName = request.Document.MainDocument.Name;
                        fdoc.name = request.Document.MainDocument.Name;
                        fdoc.contentType = request.Document.MainDocument.MimeType;
                        fdoc.content = request.Document.MainDocument.Content;
                        fdoc.nomeOriginale = request.Document.MainDocument.Name;
                        fdoc.length = request.Document.MainDocument.Content.Length;
                        firmaCX = "C";
                        try
                        {
                            bool ctrlFirmaX = BusinessLogic.Documenti.FileManager.VerifyFileSignature(fdoc, null);
                            // Problema con il controllo della firma: 
                            // Sembra sia cambiato qualcosa nel DocsPaVO.documento.SignerInfo.CertificateInfo.RevocationStatus
                            //if (!ctrlFirmaX) throw new Exception("Errore nel controllo firma");
                            if (ctrlFirmaX) firmaCX = "C";
                        }
                        catch (Exception exXml)
                        {
                            // Vedo se c'è un errata codifica Base64
                            logger.Error("ERRORE nel file P7M: provo ad eliminare una codifica base64");
                            fdoc.content = Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(request.Document.MainDocument.Content));
                            try
                            {
                                bool ctrlFirmaX2 = BusinessLogic.Documenti.FileManager.VerifyFileSignature(fdoc, null);
                                if (!ctrlFirmaX2) throw new Exception("Errore nel controllo firma");
                                else firmaCX = "C";
                            }
                            catch (Exception exXml2)
                            {

                                throw new Exception("Errore nel file XML.P7M, elaborazione non possibile. Controllare la codifica del file.");
                            }
                        }
                        if (!fdoc.name.ToUpper().EndsWith("XML"))
                        {
                            throw new PisException("ELAB_XML_FILE_NOT_VALID");
                        }
                        else
                        {
                            stringaXml = System.Text.Encoding.UTF8.GetString(fdoc.content);
                        }
                        //docPrincipale.Content = fdoc.content;
                        //docPrincipale.Name = fdoc.name;

                    }
                    if (string.IsNullOrEmpty(stringaXml))
                        stringaXml = System.Text.Encoding.UTF8.GetString(request.Document.MainDocument.Content);

                    stringaXml = stringaXml.Trim();
                    System.Xml.XmlDocument xmlDoc2 = new System.Xml.XmlDocument();
                    if (stringaXml.Contains("xml version=\"1.1\""))
                    {
                        logger.Debug("Versione XML 1.1. Provo conversione");
                        stringaXml = stringaXml.Replace("xml version=\"1.1\"", "xml version=\"1.0\"");
                    }
                    try
                    {
                        xmlDoc2.LoadXml(stringaXml);
                    }
                    catch (Exception bomUTF8)
                    {
                        string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                        if (stringaXml.StartsWith(byteOrderMarkUtf8))
                        {
                            stringaXml = stringaXml.Remove(0, byteOrderMarkUtf8.Length);
                        }
                        xmlDoc2.LoadXml(stringaXml);
                    }
                    if (xmlDoc2.DocumentElement.NamespaceURI.ToLower().Contains("http://www.fatturapa.gov.it/sdi/fatturapa/v1") ||
                xmlDoc2.DocumentElement.NamespaceURI.ToLower().Contains("http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/"))
                    {
                        // controllo se la fattura è di uno dei fornitori in DB
                        bool isFattLottoAttivo = false;

                        string mappFornitore = string.Format("//*[name()='{0}']/*[name()='{1}']/*[name()='{2}']/*[name()='{3}']", "CedentePrestatore", "DatiAnagrafici", "IdFiscaleIVA", "IdCodice");
                        string codFornitore = (xmlDoc2.DocumentElement.SelectSingleNode(mappFornitore)).InnerXml;

                        if (!string.IsNullOrEmpty(codFornitore))
                        {
                            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                            ArrayList fornitori = dbAmm.FattElAttive_getCodFornitore(infoUtente.idAmministrazione);
                            //if (fornitori.Contains(codFornitore))
                            //    isFattLottoAttivo = true;                                       
                            foreach (DocsPaVO.ExternalServices.FornitoreFattAttiva forn in fornitori)
                            {
                                if (!string.IsNullOrEmpty(forn.CodFornitore) && forn.CodFornitore.ToUpper() == codFornitore.ToUpper())
                                {
                                    isFattLottoAttivo = true;
                                    fornitoreFattAttiva = forn;
                                }
                            }
                        }

                        if (!isFattLottoAttivo)
                        {
                            logger.Error("Fornitore della fattura o lotto attivo non corrispondente: " + codFornitore);
                            throw new Exception("Codice fornitore non corrispondente");
                        }
                        if (string.IsNullOrEmpty(firmaCX) && stringaXml.ToUpper().Contains("X509CERTIFICATE"))
                        {
                            firmaCX = "X";
                        }

                        if (string.IsNullOrEmpty(firmaCX))
                        {
                            System.Xml.XmlElement root = xmlDoc2.DocumentElement;
                            string tempOutX5 = root.Attributes["versione"].Value;
                            if (tempOutX5.ToUpper() != "FPR" && tempOutX5.ToUpper() != "FPR12")
                            {
                                logger.Info("LOTTO NON PRIVATO - Errore controllo firma");
                                throw new Exception("Lotto non firmato");
                            }
                            else
                                logger.Info("LOTTO PRIVATO - Bypass controllo firma");

                        }

                        if (fornitoreFattAttiva != null)
                        {
                            regFattAttiva = BusinessLogic.Utenti.RegistriManager.getRegistro(fornitoreFattAttiva.IdRegistro);
                        }
                        System.Xml.XmlNodeList fatture = xmlDoc2.DocumentElement.SelectNodes("//*[name()='FatturaElettronicaBody']");
                        if (fatture != null && fatture.Count < 2)
                        {
                            throw new Exception("Il documento è una fattura elettronica. Associazione a lotto di fatture interrotta.");

                        }
                    }
                    else { throw new Exception("Il file principale non è una fattura."); }

                }

                #endregion
                #region Controllo del registro per repertoriazione e spedizione a responsabile - COMMENTATO
                //if (string.IsNullOrEmpty(request.CodeRegister))
                //{
                //    throw new PisException("REQUIRED_REGISTER");
                //}
                //else
                //{
                //    DocsPaVO.utente.Registro regX = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeRegister);
                //    if (regX == null)
                //    {
                //        throw new PisException("REGISTER_NOT_FOUND");
                //    }
                //    else
                //    {
                //        if (!string.IsNullOrEmpty(regX.chaRF) && regX.chaRF.Equals("1"))
                //        {
                //            request.CodeRF = request.CodeRegister;
                //            request.CodeRegister = null;
                //        }

                //        string idRespRole = "";
                //        string idRespCorr = "";
                //        if (!string.IsNullOrEmpty(regX.chaRF) && regX.chaRF.Equals("1"))
                //            idRespRole = BusinessLogic.utenti.RegistriRepertorioPrintManager.GetResponsableRoleIdFromIdTemplate(request.Document.Template.Id, regX.systemId, null, out idRespCorr);
                //        else
                //            idRespRole = BusinessLogic.utenti.RegistriRepertorioPrintManager.GetResponsableRoleIdFromIdTemplate(request.Document.Template.Id, null, regX.systemId, out idRespCorr);

                //        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(idRespCorr);

                //        ragione = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, "RICEVIMENTO_FATTURA");
                //        if (ragione == null)
                //            throw new PisException("TRANSMISSION_REASON_NOT_FOUND");
                //    }

                //}
                #endregion
                #region Controllo del destinatario e della ragione della trasmissione - COMMENTATO
                if (request.TransmissionReceiver == null)
                {
                    //Destinatario non trovato
                    throw new PisException("REQUIRED_CORRESPONDENT");
                }
                if (string.IsNullOrEmpty(request.TransmissionReceiver.Code) && string.IsNullOrEmpty(request.TransmissionReceiver.Id))
                {
                    throw new PisException("REQUIRED_CODE_OR_ID_RECEIVER");
                }
                try
                {
                    if (!string.IsNullOrEmpty(request.TransmissionReceiver.Id))
                    {
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(request.TransmissionReceiver.Id);
                    }
                    else if (!string.IsNullOrEmpty(request.TransmissionReceiver.Code))
                    {
                        //corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(request.Receiver.Code, infoUtente);
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(request.TransmissionReceiver.Code, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                    }
                    if (corr == null)
                    {
                        throw new PisException("CORRESPONDENT_NOT_FOUND");
                    }

                }
                catch (Exception e)
                {
                    throw new PisException("CORRESPONDENT_NOT_FOUND");
                }

                if (request.TransmissionReason == null || string.IsNullOrEmpty(request.TransmissionReason))
                {
                    request.TransmissionReason = "RICEVIMENTO_FATTURA_ATTIVA";
                }

                try
                {
                    ragione = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, request.TransmissionReason.ToUpper());
                    if (ragione == null)
                        throw new PisException("TRANSMISSION_REASON_NOT_FOUND");
                }
                catch (Exception e)
                {
                    throw new PisException("TRANSMISSION_REASON_NOT_FOUND");
                }
                #endregion
                #region Controllo presenza fascicolo
                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();
                if (!string.IsNullOrEmpty(fornitoreFattAttiva.CodFascicolo))
                {
                    //Il codice è di un sottofascicolo
                    if (fornitoreFattAttiva.CodFascicolo.IndexOf("//") > -1)
                    {
                        string separatore = "//";
                        // MODIFICA per inserimento in sottocartelle
                        string[] separatoreAr = new string[] { separatore };

                        string[] pathCartelle = request.CodeProject.Split(separatoreAr, StringSplitOptions.RemoveEmptyEntries);

                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloDaCodice(infoUtente, pathCartelle[0], null, false, false);
                        if (fascicolo != null)
                        {
                            if (pathCartelle.Length > 1)
                            {
                                ArrayList cartelle = BusinessLogic.Fascicoli.FascicoloManager.getListaFolderDaIdFascicolo(infoUtente, fascicolo.systemID, null, false, false);
                                // caricamento folder base
                                string idParentFolder = fascicolo.systemID;
                                fascicolo.folderSelezionato = (DocsPaVO.fascicolazione.Folder)((from DocsPaVO.fascicolazione.Folder f in cartelle where f.idParent == idParentFolder select f).ToList().FirstOrDefault());
                                idParentFolder = fascicolo.folderSelezionato.systemID;
                                try
                                {
                                    for (int i = 1; i < pathCartelle.Length; i++)
                                    {

                                        fascicolo.folderSelezionato = (DocsPaVO.fascicolazione.Folder)((from DocsPaVO.fascicolazione.Folder f in cartelle where f.idParent == idParentFolder && f.descrizione == pathCartelle[i] select f).ToList().First());
                                        idParentFolder = fascicolo.folderSelezionato.systemID;
                                    }
                                }
                                catch (Exception e)
                                {
                                    throw new Exception("Cartella non trovata nel fascicolo");
                                }
                            }
                        }
                        else
                        {
                            throw new PisException("PROJECT_NOT_FOUND");
                        }


                    }
                    else
                    {
                        ArrayList fascicoli = new ArrayList();
                        fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliDaCodice(infoUtente, fornitoreFattAttiva.CodFascicolo, null, false, false, "I");
                        if (fascicoli != null && fascicoli.Count > 0)
                        {
                            if (fascicoli.Count == 1)
                            {
                                fascicolo = (DocsPaVO.fascicolazione.Fascicolo)fascicoli[0];
                            }
                            else
                            {
                                //Fascicoli multipli
                                throw new PisException("MULTIPLE");
                            }
                        }
                        else
                        {
                            //Fascicolo non trovato
                            throw new PisException("PROJECT_NOT_FOUND");
                        }
                    }
                }


                #endregion
                #region Controllo presenza Lotto - COMMENTATO
                //if (!string.IsNullOrEmpty(request.IdentificativoSdI))
                //{
                //    string idLottoPresente = BusinessLogic.Amministrazione.SistemiEsterni.FattElCtrlDupl(null, request.IdentificativoSdI, out passo, out esito);
                //    if (!string.IsNullOrEmpty(idLottoPresente))
                //    {
                //        logger.DebugFormat("Lotto presente con DocNum {0}. Passo {1}. Esito {2}", idLottoPresente, passo, esito);
                //        //throw new Exception(string.Format("Fattura con identificativo {0} già presente. Id PITre: {1}", request.Document.ConsolidationState, idFatturaPresente));
                //        //schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idLottoPresente, idLottoPresente);
                //        if (esito.ToUpper() == "OK")
                //        {
                //            numPasso = 15;
                //            throw new Exception(string.Format("Il lotto con IdSdI {1} è già presente con IdPiTre {0}.", idLottoPresente, request.IdentificativoSdI));
                //        }
                //        else if (esito.ToUpper() == "ERRORE")
                //        {
                //            schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idLottoPresente, idLottoPresente);
                //            schedaDocResult = schedaDoc;
                //            switch (passo.ToUpper())
                //            {
                //                case "CARICAMENTO FILE PRINCIPALE":
                //                    numPasso = 8;
                //                    break;
                //                case "CREAZIONE DOCUMENTO IN RISPOSTA":
                //                    numPasso = 9;
                //                    break;
                //                case "CARICAMENTO ALLEGATI":
                //                    numPasso = 10;
                //                    break;
                //                case "INSERIMENTO IN FASCICOLO":
                //                    numPasso = 11;
                //                    break;
                //                case "ASSOCIAZIONE TEMPLATE":
                //                    numPasso = 12;
                //                    break;
                //                case "ESECUZIONE TRASMISSIONI":
                //                    numPasso = 13;
                //                    break;
                //                default:
                //                    numPasso = 8;
                //                    break;
                //            }
                //        }

                //    }
                //}
                #endregion
                #region Creazione Scheda Documento
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                DocsPaVO.utente.Corrispondente mittente = new DocsPaVO.utente.Corrispondente();

                try
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                    if (ruolo == null)
                    {
                        //Ruolo non trovato
                        throw new PisException("ROLE_NO_EXIST");
                    }
                }
                catch
                {
                    //Ruolo non trovato
                    throw new PisException("ROLE_NO_EXIST");
                }

                if (numPasso < 8)
                {

                    // **************************************************************************
                    // Per accordi con il cliente viene effettuato questo controllo:
                    // se Type == "O" allore viene impostato il valore di CorrespondentType = "O"
                    // Controllo effettuato per: Sender, Recipients, RecipientsCC
                    // **************************************************************************
                    //
                    if (request.Document.Sender != null && !string.IsNullOrEmpty(request.Document.Sender.Type) && request.Document.Sender.Type.Equals("O"))
                        request.Document.Sender.CorrespondentType = "O";

                    if (request.Document.MultipleSenders != null)
                    {
                        for (int i = 0; i < request.Document.MultipleSenders.Length; i++)
                        {
                            Domain.Correspondent c = request.Document.MultipleSenders[i];
                            if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                                c.CorrespondentType = "O";
                        }
                    }

                    if (request.Document.Recipients != null)
                    {
                        for (int i = 0; i < request.Document.Recipients.Length; i++)
                        {
                            Domain.Correspondent c = request.Document.Recipients[i];
                            if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                                c.CorrespondentType = "O";
                        }
                    }

                    if (request.Document.RecipientsCC != null)
                    {
                        for (int i = 0; i < request.Document.RecipientsCC.Length; i++)
                        {
                            Domain.Correspondent c = request.Document.RecipientsCC[i];
                            if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                                c.CorrespondentType = "O";
                        }
                    }
                    //


                    ///Creazione schedaDocumento con i dati essenziali e obbligatori
                    schedaDoc = BusinessLogic.Documenti.DocManager.NewSchedaDocumento(infoUtente, false);

                    ///Imposto l'oggetto
                    if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.Object))
                    {
                        schedaDoc.oggetto.descrizione = request.Document.Object;
                        schedaDoc.oggetto.daAggiornare = true;
                    }

                    //Imposto le note, soltanto visibili a tutti
                    if (request != null && request.Document != null && request.Document != null && request.Document.Note != null && request.Document.Note.Length > 0)
                    {
                        schedaDoc.noteDocumento = new List<DocsPaVO.Note.InfoNota>();
                        foreach (Domain.Note nota in request.Document.Note)
                        {
                            DocsPaVO.Note.InfoNota tempNot = new DocsPaVO.Note.InfoNota();
                            tempNot.DaInserire = true;
                            tempNot.Testo = nota.Description;
                            tempNot.TipoVisibilita = DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti;
                            tempNot.UtenteCreatore = new DocsPaVO.Note.InfoUtenteCreatoreNota();
                            tempNot.UtenteCreatore.IdRuolo = infoUtente.idGruppo;
                            tempNot.UtenteCreatore.IdUtente = infoUtente.idPeople;
                            schedaDoc.noteDocumento.Add(tempNot);
                        }
                    }
                    //

                    ///Imposto il template
                    if (request != null && request.Document != null && request.Document.Template != null && (!string.IsNullOrEmpty(request.Document.Template.Id) || (!string.IsNullOrEmpty(request.Document.Template.Name))))
                    {
                        DocsPaVO.ProfilazioneDinamica.Templates template = null;
                        if (!string.IsNullOrEmpty(request.Document.Template.Id))
                        {
                            template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(request.Document.Template.Id);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(request.Document.Template.Name))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(request.Document.Template.Name, infoUtente.idAmministrazione);
                            }
                        }

                        if (template != null)
                        {
                            //Modifica Lembo 10-01-2013: Controllo se l'utente è abilitato all'utilizzo del template.
                            //Solo ruoli con diritti di scrittura ammessi.
                            string controlFilter = string.Format("{0} and id_ruolo={1} and (diritti='2')", template.SYSTEM_ID.ToString(), infoUtente.idGruppo);
                            ArrayList controlloVisibilita = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getRuoliTipoDoc(controlFilter);
                            if (controlloVisibilita == null || controlloVisibilita.Count == 0)
                            {
                                throw new PisException("TEMPLATE_NOT_ROLE_EDITABLE");
                            }
                            // Modifica Lembo 11-01-2013: Utilizzo il metodo che controlla la visibilità dei campi.
                            //schedaDoc.template = Utils.GetTemplateFromPis(request.Document.Template, template, false);

                            // Modifica Elaborazione XML da PIS req.2

                            Domain.File fileDaPassare = ((request.Document.MainDocument != null && request.Document.MainDocument.Content != null) ? request.Document.MainDocument : null);

                            schedaDoc.template = Utils.GetTemplateFromPisVisibility(request.Document.Template, template, false, infoUtente.idGruppo, "D", request.CodeApplication, infoUtente
                                , fileDaPassare,
                                regFattAttiva.chaRF == "1" ? null : regFattAttiva.codRegistro,
                                regFattAttiva.chaRF == "0" ? null : regFattAttiva.codRegistro,
                                false, request.IdentificativoSdI);
                            schedaDoc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                            schedaDoc.tipologiaAtto.systemId = template.SYSTEM_ID.ToString();
                            schedaDoc.tipologiaAtto.descrizione = template.DESCRIZIONE.ToString();

                            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggcustom in schedaDoc.template.ELENCO_OGGETTI)
                            {
                                if (oggcustom.CONTATORE_DA_FAR_SCATTARE == true)
                                    oggcustom.CONTATORE_DA_FAR_SCATTARE = false;
                            }

                        }
                        else
                        {
                            //Template non trovato
                            throw new PisException("TEMPLATE_NOT_FOUND");
                        }
                    }
                    ///

                    ///Se privato
                    if (request != null && request.Document != null && request.Document.PrivateDocument)
                    {
                        schedaDoc.privato = "1";
                    }
                    ///

                    ///Se personale
                    if (request != null && request.Document != null && request.Document.PersonalDocument)
                    {
                        schedaDoc.personale = "1";
                    }
                    ///

                    schedaDocResult = null;
                    DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione = new DocsPaVO.documento.ResultProtocollazione();
                    bool daAggiornareUffRef = false;

                    ///CASO DOCUMENTO GRIGIO (NON PROTOCOLLATO)
                    if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("G"))
                    {
                        schedaDoc.tipoProto = "G";
                        schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                    }
                    ///

                    //Controlli solo per protocolli
                    if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && !request.Document.DocumentType.ToUpper().Equals("G"))
                    {

                        if (request.Document.Sender == null || string.IsNullOrEmpty(request.Document.Sender.CorrespondentType))
                        {
                            if (!request.Document.DocumentType.ToUpper().Equals("P"))
                            {
                                //Mittente non presente
                                throw new PisException("REQUIRED_SENDER");
                            }
                            else
                            {
                                mittente = null;
                            }
                        }
                        else
                        {
                            if (request.Document.Sender != null && !string.IsNullOrEmpty(request.Document.Sender.CorrespondentType) && request.Document.Sender.CorrespondentType.Equals("O"))
                            {
                                mittente = Utils.GetCorrespondentFromPis(request.Document.Sender, infoUtente);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(request.Document.Sender.Id))
                                {
                                    mittente = Utils.RisolviCorrispondente(request.Document.Sender.Id, infoUtente);
                                    if (mittente == null)
                                        //Mittente non trovato
                                        throw new PisException("SENDER_NOT_FOUND");
                                }
                            }
                        }

                        if (request.Document.DocumentType.ToUpper().Equals("I") || request.Document.DocumentType.ToUpper().Equals("P"))
                        {
                            if (request.Document.Recipients == null || request.Document.Recipients.Length == 0)
                            {
                                //Destinatario non presente
                                throw new PisException("REQUIRED_RECIPIENT");
                            }
                        }

                        if (string.IsNullOrEmpty(request.CodeRegister))
                        {
                            //Registro mancante
                            throw new PisException("REQUIRED_REGISTER");
                        }
                        else
                        {
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeRegister);
                            if (reg == null)
                            {
                                //Registro mancante
                                throw new PisException("REGISTER_NOT_FOUND");
                            }
                            else
                            {
                                schedaDoc.registro = reg;
                            }
                        }

                        if (!string.IsNullOrEmpty(request.CodeRF))
                        {
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeRF);
                            if (reg != null)
                            {
                                schedaDoc.id_rf_prot = reg.systemId;
                                schedaDoc.id_rf_invio_ricevuta = reg.systemId;
                                schedaDoc.cod_rf_prot = reg.codRegistro;
                            }
                            else
                            {
                                //RF non trovato
                                throw new PisException("RF_NOT_FOUND");
                            }
                        }
                    }

                    ///CASO PROTOCOLLO IN ARRIVO (ANCHE PREDISPOSTO)
                    if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("A"))
                    {
                        schedaDoc.tipoProto = "A";
                        schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloEntrata();

                        ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente = mittente;

                        if (request.Document.MultipleSenders != null && request.Document.MultipleSenders.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittenti = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.MultipleSenders)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = null;
                                if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                                {
                                    corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(corrTemp.Id))
                                    {
                                        corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                        if (corrBySys == null)
                                        {
                                            //Mittente non trovato
                                            throw new PisException("SENDER_NOT_FOUND");
                                        }
                                    }
                                }
                                ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittenti.Add(corrBySys);
                            }
                        }

                        //Data protocollo mittente
                        if (!string.IsNullOrEmpty(request.Document.DataProtocolSender))
                        {
                            ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).dataProtocolloMittente = request.Document.DataProtocolSender;
                        }

                        //Protocollo mittente
                        if (!string.IsNullOrEmpty(request.Document.ProtocolSender))
                        {
                            ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).descrizioneProtocolloMittente = request.Document.ProtocolSender;
                        }

                        //Data arrivo TODO
                        if (!string.IsNullOrEmpty(request.Document.ArrivalDate))
                        {
                            // Variabile Booleana per il controllo di validità della data passata in input;
                            bool valid = false;
                            // Data ritornata in output dal TryParseExact per la data arrivo passata in input
                            DateTime dateVal;

                            // Pattern di validità per una data valida
                            string pattern = "dd/MM/yyyy HH:mm";

                            try
                            {
                                /*
                                 *  Date Format Example: 01/03/2012
                                 *  dd = day (01)
                                 *  MM = month (03)
                                 *  yyyy = year (2012)
                                 *  / = date separator
                                 */

                                if (!DateTime.TryParseExact(request.Document.ArrivalDate, pattern, null, System.Globalization.DateTimeStyles.None, out dateVal))
                                {
                                    throw new Exception("Formato Data non corretto");
                                }

                                valid = true;
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Formato Data non corretto");
                            }

                            if (valid)
                            {
                                // Controllo che la data inserita non sia posteriore alla data odierna
                                DateTime dateVal2;
                                // Variabile Booleana per il controllo di validità della data DateTime.Now trasformato nel formato dd/MM/yyyy;
                                bool dateNow = true;

                                if (!DateTime.TryParseExact(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), pattern, null, System.Globalization.DateTimeStyles.None, out dateVal2))
                                {
                                    // Non dovrebbe entrare mai in questo controllo.
                                    dateNow = false;
                                    throw new Exception("Formato Data non corretto");
                                }

                                if (dateNow)
                                {
                                    // dateVal: data passata in input;
                                    // dateVal2: DateTime.Now convertito nel formato dd/MM/yyyy
                                    int result = DateTime.Compare(dateVal, dateVal2);

                                    if (result > 0)
                                        // Data di input è successiva alla data odierna;
                                        //((Documento)schedaDoc.documenti[0]).dataArrivo = string.Empty;
                                        throw new Exception("La data di arrivo non può essere posteriore alla data di segnatura");
                                    else
                                        // Data di input valida e conforme;
                                        ((DocsPaVO.documento.Documento)schedaDoc.documenti[0]).dataArrivo = request.Document.ArrivalDate;
                                }
                            }
                        }

                        if (request.Document.Predisposed)
                        {
                            schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                            if (schedaDocResult != null)
                            {
                                schedaDocResult.predisponiProtocollazione = true;
                                schedaDocResult.tipoProto = request.Document.DocumentType;
                                schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                            }
                        }
                        else
                        {
                            schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out risultatoProtocollazione);
                        }

                        if (schedaDocResult != null)
                        {
                            //Mezzo di spedizione
                            if (!string.IsNullOrEmpty(request.Document.MeansOfSending))
                            {
                                ArrayList mezziSpedizione = BusinessLogic.Amministrazione.MezziSpedizioneManager.ListaMezziSpedizione(infoUtente.idAmministrazione, true);
                                foreach (DocsPaVO.amministrazione.MezzoSpedizione m_spediz in mezziSpedizione)
                                {
                                    if (m_spediz.Descrizione.ToUpper().Equals(request.Document.MeansOfSending.ToUpper()))
                                    {
                                        BusinessLogic.Documenti.ProtoManager.collegaMezzoSpedizioneDocumento(infoUtente, m_spediz.IDSystem, schedaDocResult.systemId);
                                        break;
                                    }
                                }

                            }
                        }
                    }
                    ///

                    ///CASO PROTOCOLLO IN PARTENZA (ANCHE PREDISPOSTO)
                    if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("P"))
                    {
                        schedaDoc.tipoProto = "P";
                        schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloUscita();

                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente = mittente;

                        if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.Recipients)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = null;

                                // Verifica se occasionale
                                if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                                {
                                    corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                }
                                else
                                {
                                    // Se non è occasionale, lo risolve sulla rubrica
                                    corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                    if (corrBySys == null)
                                    {
                                        //Destinatario non trovato
                                        throw new PisException("RECIPIENT_NOT_FOUND");
                                    }

                                }
                                if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "F" && corrBySys.tipoIE == null)
                                {
                                    ArrayList corrByRF = BusinessLogic.Rubrica.RF.getCorrispondentiByCodRF(corrBySys.codiceRubrica);
                                    foreach (DocsPaVO.utente.Corrispondente c2 in corrByRF)
                                    {
                                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(c2);
                                    }
                                }
                                else if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                                {
                                    ArrayList corrByLista = BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiByCodLista(corrBySys.codiceRubrica, infoUtente.idAmministrazione, infoUtente);
                                    foreach (DocsPaVO.utente.Corrispondente c3 in corrByLista)
                                    {
                                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(c3);
                                    }
                                }
                                else
                                {
                                    ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(corrBySys);
                                }
                            }
                        }

                        if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.RecipientsCC)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = null;

                                // Verifica se occasionale
                                if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                                {
                                    corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                }
                                else
                                {
                                    corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                    if (corrBySys == null)
                                    {
                                        //Destinatario non trovato
                                        throw new PisException("RECIPIENT_NOT_FOUND");
                                    }
                                }
                                if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "F" && corrBySys.tipoIE == null)
                                {
                                    ArrayList corrByRF = BusinessLogic.Rubrica.RF.getCorrispondentiByCodRF(corrBySys.codiceRubrica);
                                    foreach (DocsPaVO.utente.Corrispondente c2 in corrByRF)
                                    {
                                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(c2);
                                    }
                                }
                                else if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                                {
                                    ArrayList corrByLista = BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiByCodLista(corrBySys.codiceRubrica, infoUtente.idAmministrazione, infoUtente);
                                    foreach (DocsPaVO.utente.Corrispondente c3 in corrByLista)
                                    {
                                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(c3);
                                    }
                                }
                                else
                                {
                                    ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
                                }

                                //((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
                            }
                        }

                        if (request.Document.Predisposed)
                        {
                            schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                            if (schedaDocResult != null)
                            {
                                schedaDocResult.predisponiProtocollazione = true;
                                schedaDocResult.tipoProto = request.Document.DocumentType;
                                schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                            }
                        }
                        else
                        {
                            schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out risultatoProtocollazione);
                        }
                    }
                    ///

                    ///CASO PROTOCOLLO INTERNO (ANCHE PREDISPOSTO)
                    if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("I"))
                    {
                        schedaDoc.tipoProto = "I";
                        schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloInterno();

                        ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).mittente = mittente;

                        if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatari = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.Recipients)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new PisException("RECIPIENT_NOT_FOUND");
                                }
                                ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatari.Add(corrBySys);
                            }
                        }

                        if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                        {
                            ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                            foreach (Domain.Correspondent corrTemp in request.Document.RecipientsCC)
                            {
                                DocsPaVO.utente.Corrispondente corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new PisException("RECIPIENT_NOT_FOUND");
                                }
                                ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
                            }
                        }

                        if (request.Document.Predisposed)
                        {
                            schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                            if (schedaDocResult != null)
                            {
                                schedaDocResult.predisponiProtocollazione = true;
                                schedaDocResult.tipoProto = request.Document.DocumentType;
                                schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                            }
                        }
                        else
                        {
                            schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out risultatoProtocollazione);
                        }
                    }
                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggcustom in schedaDoc.template.ELENCO_OGGETTI)
                    {
                        if (oggcustom.CONTATORE_DA_FAR_SCATTARE == true && oggcustom.REPERTORIO == "1")
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTO_REPERTORIATO", schedaDoc.systemId, "Repertoriato documento", DocsPaVO.Logger.CodAzione.Esito.OK);
                    }

                    ///
                    // Log creazione documento
                    if (schedaDocResult != null)
                    {
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTO_CREATO", schedaDoc.systemId, "Creato documento", DocsPaVO.Logger.CodAzione.Esito.OK);
                    }

                #endregion
                    #region Inserimento nella tabella di log delle fatture - COMMENTATO
                    //bool insLogOk = BusinessLogic.Amministrazione.SistemiEsterni.FattElInsTabellaLog(schedaDocResult.systemId, null, request.IdentificativoSdI);
                }
                    #endregion
                #region Caricamento File Principale
                try
                {
                    if (schedaDocResult != null && !string.IsNullOrEmpty(schedaDocResult.docNumber) && request.Document.MainDocument != null && numPasso < 9)
                    {
                        //Crea il file o se esistente una nuova versione
                        // 5. Acquisizione del file al documento
                        DocsPaVO.documento.FileRequest versioneCorrente = (DocsPaVO.documento.FileRequest)schedaDocResult.documenti[0];

                        DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                        {
                            name = request.Document.MainDocument.Name,
                            fullName = request.Document.MainDocument.Name,
                            content = request.Document.MainDocument.Content,
                            contentType = request.Document.MainDocument.MimeType,
                            length = request.Document.MainDocument.Content.Length,
                            bypassFileContentValidation = true
                        };

                        string errorMessage;

                        if (!BusinessLogic.Documenti.FileManager.putFile(ref versioneCorrente, fileDocumento, infoUtente, out errorMessage))
                        {
                            throw new PisException("FILE_CREATION_ERROR");
                        }
                        else
                        {
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "FILE_CARICATO", schedaDoc.systemId, "File caricato", DocsPaVO.Logger.CodAzione.Esito.OK);
                        }


                    }
                }
                catch (Exception exFilePrincipale)
                {
                    //bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(null, request.IdentificativoSdI, "CARICAMENTO FILE PRINCIPALE", "ERRORE");
                    throw new Exception("Errore al passo CARICAMENTO FILE PRINCIPALE");
                }
                #endregion
                #region Creazione Documento in risposta
                if (schedaDocResult != null)
                {
                    response.Document = Utils.GetDetailsDocument(schedaDocResult, infoUtente, false);
                    if (!string.IsNullOrEmpty(infoUtente.diSistema) && infoUtente.diSistema == "1")
                    {
                        bool pulito = Utils.CleanRightsExtSys(schedaDocResult.docNumber, infoUtente.idPeople, infoUtente.idGruppo);
                    }
                }
                response.Success = true;
                #endregion
                #region Caricamento allegati
                try
                {
                    if (request.Document.Attachments != null && request.Document.Attachments.Length > 0 & numPasso < 11)
                    {
                        string erroreMessage = "";
                        foreach (Domain.File all in request.Document.Attachments)
                        {
                            DocsPaVO.documento.FileRequest allegato = new DocsPaVO.documento.Allegato
                            {
                                docNumber = schedaDocResult.systemId,
                                descrizione = all.Description
                            };

                            // Acquisizione dell'allegato
                            allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, ((DocsPaVO.documento.Allegato)allegato));
                            //definisco l'allegato come esterno
                            try
                            {
                                if (!string.IsNullOrEmpty(all.VersionId) && all.VersionId == "E")
                                {
                                    if (!BusinessLogic.Documenti.AllegatiManager.setFlagAllegatiEsterni(allegato.versionId, allegato.docNumber))
                                    {
                                        BusinessLogic.Documenti.AllegatiManager.rimuoviAllegato((DocsPaVO.documento.Allegato)allegato, infoUtente);
                                        throw new PisException("ERROR_ADD_ATTACHMENT");
                                    }
                                }
                            }
                            catch (PisException pisEx)
                            {
                                response.Error = new Services.ResponseError
                                {
                                    Code = pisEx.ErrorCode,
                                    Description = pisEx.Description
                                };

                                response.Success = false;
                                return response;
                            }

                            // Acquisizione file allegato
                            // 5. Acquisizione del file al documento
                            DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                            {
                                name = all.Name,
                                fullName = all.Name,
                                content = all.Content,
                                contentType = all.MimeType,
                                length = all.Content.Length,
                                bypassFileContentValidation = true
                            };


                            if (!BusinessLogic.Documenti.FileManager.putFile(ref allegato, fileDocumento, infoUtente, out erroreMessage))
                            {
                                throw new PisException("FILE_CREATION_ERROR");
                            }
                        }
                    }
                }
                catch (Exception exCaricaAllegati)
                {
                    //bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(null, request.IdentificativoSdI, "CARICAMENTO ALLEGATI", "ERRORE");
                    throw new Exception("Errore al passo CARICAMENTO ALLEGATI");
                }

                #endregion
                #region Inserimento in fascicolo
                try
                {
                    string msg = "";
                    if (numPasso < 12)
                    {
                        if (fascicolo != null)
                        {
                            //Caso del sottofascicolo
                            if (fascicolo.folderSelezionato != null)
                            {
                                BusinessLogic.Fascicoli.FolderManager.addDocFolder(infoUtente, schedaDocResult.systemId, fascicolo.folderSelezionato.systemID, false, out msg);
                            }
                            else
                            {
                                BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, schedaDocResult.systemId, fascicolo.systemID, false, out msg);
                            }
                        }
                        else
                        {
                            //Fascicolo non trovato
                            throw new PisException("PROJECT_NOT_FOUND");
                        }
                    }
                }
                catch (Exception exInFasc)
                {
                    //bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(null, request.IdentificativoSdI, "INSERIMENTO IN FASCICOLO", "ERRORE");
                    throw new Exception("Errore al passo INSERIMENTO IN FASCICOLO");
                }
                #endregion

                #region Associazione template
                try
                {
                    if (numPasso < 13)
                    {
                        if (schedaDocResult != null)
                        {
                            DocsPaVO.ProfilazioneDinamica.Templates template = null;

                            if (!string.IsNullOrEmpty(request.Document.Template.Id))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(request.Document.Template.Id);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(request.Document.Template.Name))
                                {
                                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(request.Document.Template.Name, infoUtente.idAmministrazione);
                                }
                            }
                            if (template != null)
                            {
                                Domain.File fileDaPassare = ((request.Document.MainDocument != null && request.Document.MainDocument.Content != null) ? request.Document.MainDocument : null);

                                schedaDocResult.template = Utils.GetTemplateFromPisVisibility(request.Document.Template, template, false, infoUtente.idGruppo, "D", request.CodeApplication, infoUtente, fileDaPassare, regFattAttiva.chaRF == "1" ? null : regFattAttiva.codRegistro, regFattAttiva.chaRF == "0" ? null : regFattAttiva.codRegistro, false, request.IdentificativoSdI);
                                schedaDocResult.daAggiornareTipoAtto = true;
                                schedaDocResult.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                                schedaDocResult.tipologiaAtto.systemId = schedaDocResult.template.SYSTEM_ID.ToString();
                                schedaDocResult.tipologiaAtto.descrizione = schedaDocResult.template.DESCRIZIONE.ToString();

                                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggcustom in schedaDocResult.template.ELENCO_OGGETTI)
                                {
                                    if (oggcustom.REPERTORIO == "1")
                                    {
                                        oggcustom.CONTATORE_DA_FAR_SCATTARE = true;
                                    }
                                }

                                bool daAggiornareUffRef = false;

                                schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                                if (schedaDocResult == null)
                                {
                                    throw new PisException("ASS_TEMPLATE_ERROR");
                                }

                                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggcustom in schedaDocResult.template.ELENCO_OGGETTI)
                                {

                                    if (oggcustom.CONTATORE_DA_FAR_SCATTARE == true && oggcustom.REPERTORIO == "1")
                                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTO_REPERTORIATO", schedaDoc.systemId, "Repertoriato documento", DocsPaVO.Logger.CodAzione.Esito.OK);
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(request.Document.ConsolidationState, "", "ASSOCIAZIONE TEMPLATE", "ERRORE");
                    logger.Error("Errore in ASSOCIAZIONE TEMPLATE :" + ex.Message);
                    throw new Exception("Errore in ASSOCIAZIONE TEMPLATE");
                }
                #endregion

                #region Esecuzione trasmissione - COMMENTATO
                try
                {
                    if (numPasso < 14)
                    {
                        string trasmType = "S";
                        if (!string.IsNullOrEmpty(request.TransmissionType) && request.TransmissionType == "T")
                        {
                            trasmType = "T";
                        }

                        //creazione oggetto trasmissione e popolamento campi.
                        DocsPaVO.trasmissione.Trasmissione transmission = new DocsPaVO.trasmissione.Trasmissione();
                        // Creazione dell'oggetto trasmissione
                        transmission = new DocsPaVO.trasmissione.Trasmissione();

                        // Impostazione dei parametri della trasmissione
                        // Note generali
                        // transmission.noteGenerali = ragione.note;

                        // Tipo oggetto
                        transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;

                        // Informazioni sul documento
                        transmission.infoDocumento = BusinessLogic.Documenti.DocManager.getInfoDocumento(schedaDocResult);

                        // Utente mittente della trasmissione
                        transmission.utente = utente;

                        // Ruolo mittente
                        // Se inserito in ingresso il CodeRole utilizzo il coderole (WIP)
                        transmission.ruolo = ruolo;
                        string generaNotifica = "1";
                        if (request.TransmissionNotify != null && !request.TransmissionNotify)
                        {
                            transmission.NO_NOTIFY = "1";
                            generaNotifica = "0";
                        }
                        else
                        {
                            transmission.NO_NOTIFY = "0";
                            generaNotifica = "1";
                        }
                        // Modifica per invio in mail dell'url del frontend
                        string urlfrontend = "";
                        if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                            urlfrontend = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();

                        transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(
                            transmission, corr, ragione, "", trasmType);
                        // il tipo trasm è S

                        DocsPaVO.trasmissione.Trasmissione resultTrasm = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(urlfrontend, transmission);
                        response.Success = true;
                        string desc = string.Empty;
                        // LOG per documento
                        if (resultTrasm.infoDocumento != null && !string.IsNullOrEmpty(resultTrasm.infoDocumento.idProfile))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in resultTrasm.trasmissioniSingole)
                            {
                                string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                if (resultTrasm.infoDocumento.segnatura == null)
                                    desc = "Trasmesso Documento : " + resultTrasm.infoDocumento.docNumber.ToString();
                                else
                                    desc = "Trasmesso Documento : " + resultTrasm.infoDocumento.segnatura.ToString();

                                BusinessLogic.UserLog.UserLog.WriteLog(resultTrasm.utente.userId, resultTrasm.utente.idPeople, resultTrasm.ruolo.idGruppo, resultTrasm.utente.idAmministrazione, method, resultTrasm.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                            }
                        }
                        // LOG per fascicolo
                        if (resultTrasm.infoFascicolo != null && !string.IsNullOrEmpty(resultTrasm.infoFascicolo.idFascicolo))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in resultTrasm.trasmissioniSingole)
                            {
                                string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                desc = "Trasmesso Fascicolo ID: " + resultTrasm.infoFascicolo.idFascicolo.ToString();
                                BusinessLogic.UserLog.UserLog.WriteLog(resultTrasm.utente.userId, resultTrasm.utente.idPeople, resultTrasm.ruolo.idGruppo, resultTrasm.utente.idAmministrazione, method, resultTrasm.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                            }
                        }
                    }
                }
                catch (Exception exTrasm)
                {
                    //bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(null, request.IdentificativoSdI, "ESECUZIONE TRASMISSIONI", "ERRORE");
                    throw new Exception("Errore al passo ESECUZIONE TRASMISSIONI");
                }
                BusinessLogic.Amministrazione.SistemiEsterni.FattElAttive_InsertInLogPIS(schedaDocResult.systemId, corr.systemId, corr.codiceRubrica);
                BusinessLogic.Amministrazione.SistemiEsterni.FattElAttive_UpSecProprietario(schedaDocResult.systemId);
                #endregion
                #region Update tabella di log delle fatture con status OK - COMMENTATO
                //if (!string.IsNullOrEmpty(request.IdentificativoSdI))
                //{
                //    bool upLogTabFatture = BusinessLogic.Amministrazione.SistemiEsterni.FattElUpdTabellaLog(null, request.IdentificativoSdI, "", "OK");
                //}
                #endregion

            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }


        public static VtDocsWS.Services.DocumentsAdvanced.GetModifiedDocumentAdv.GetModifiedDocumentAdvResponse GetModifiedDocumentAdv(VtDocsWS.Services.DocumentsAdvanced.GetModifiedDocumentAdv.GetModifiedDocumentAdvRequest request)
        {
            Services.DocumentsAdvanced.GetModifiedDocumentAdv.GetModifiedDocumentAdvResponse response = new Services.DocumentsAdvanced.GetModifiedDocumentAdv.GetModifiedDocumentAdvResponse();

            try
            {

                logger.Debug("GetModifiedDocumentAdv_START request.FromDateTime= " + request.FromDateTime);
                response.Success = true;
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                #region Controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetModifiedDocumentAdv");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                #endregion

                #region Controllo limiti richiesta (MASSIMO LIMITE TEMPORALE - MASSIMO NUMERO DI EVENTI DA CERCARE

                //string maxValuesDefault = "60;5";
                //string maxValuesStr = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_MAX_VALORI_MODIFIED_DOC");

                //if (string.IsNullOrEmpty(maxValuesStr))
                //    maxValuesStr = maxValuesDefault;

                //string[] valuesArray = maxValuesStr.Split(';');

                //string maxTimeLimit = valuesArray[0];
                //string maxEvents = valuesArray[1];

                ////Controllo sul massimo limite temporale 
                ////IFormatProvider culture = new CultureInfo("it-IT", true);

                //logger.Debug("GetModifiedDocumentAdv request.FromDateTime= " + request.FromDateTime);

                //request.FromDateTime = request.FromDateTime.Replace(".", ":");

                ////DateTime dateVal = DateTime.ParseExact(request.FromDateTime, "dd/MM/yyyy HH:mm:ss", culture);
                //DateTime dateVal = DateTime.ParseExact(request.FromDateTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                //int gg = int.Parse(maxTimeLimit);

                //if (DateTime.Compare(DateTime.Today.AddDays(-gg), dateVal) > 0)
                //{
                //    logger.Info("Superato massimo limite temporale per la ricerca");
                //    response.Error = new Services.ResponseError
                //    {
                //        Code = "OVER_MAX_TIME_INTERVAL",
                //        Description = "Superato massimo limite temporale per la ricerca, intervallo massimo: " + maxTimeLimit + " giorni"
                //    };

                //    response.Success = false;

                //    return response;

                //    //throw new PisException("Superato massimo limite temporale per la ricerca, intervallo massimo: " + maxTimeLimit);
                //}

                ////Controllo sul massimo numero di eventi da cercare
                //string[] eventsSplitted = request.EventString.Trim().Split(';');
                //if (eventsSplitted.Length > Convert.ToInt32(maxEvents))
                //{
                //    logger.Info("Superato massimo numero di eventi per la ricerca");
                //    response.Error = new Services.ResponseError
                //    {
                //        Code = "OVER_MAX_EVENTS",
                //        Description = "Superato massimo numero di eventi per la ricerca, numero massimo: " + maxEvents
                //    };

                //    response.Success = false;

                //    return response;

                //    //throw new PisException("Superato massimo numero di eventi per la ricerca, numero massimo: " + maxEvents);

                //}

                #endregion


                ArrayList docs = BusinessLogic.Documenti.InfoDocManager.GetModifiedDcoumentAdv(request.FromDateTime, request.EventString);
                VtDocsWS.Domain.ModifiedDocument[] documents = null;
                documents = new Domain.ModifiedDocument[docs.Count];

                //foreach(DocsPaVO.Logger.CodAzione.infoOggetto doc in docs)
                //{
                //    VtDocsWS.Domain.ModifiedDocument d = new VtDocsWS.Domain.ModifiedDocument();
                //    d.DocumentId = doc.Oggetto;
                //    d.ActionDescription = doc.Codice;
                //    response.ModifiedDocuments.Add(d);
                //}

                int y = 0;
                foreach (DocsPaVO.documento.LogDocumento doc in docs)
                {
                    documents[y] = new Domain.ModifiedDocument()
                    {
                        DocumentId = doc.descrOggetto,
                        ActionDescription = doc.codAzione,
                        Operatore = doc.descProduttore,
                        DataEvento = doc.dataAzione
                    };
                    y++;
                }

                response.ModifiedDocuments = documents;

            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }
            return response;
        }

    }
}