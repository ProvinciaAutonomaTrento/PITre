using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using System.Threading.Tasks;
using RESTServices.Model.Domain;
using RESTServices.Model.Responses;
using RESTServices.Model.Requests;
using DocsPaVO.documento;
using System.Collections;


namespace RESTServices.Manager
{
    public class DocumentsManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocumentsManager));

        public static async Task<GetFiltersResponse> getDocumentsFilters(string token)
        {
            logger.Info("begin getDocumentsFilters");
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion

                List<Filter> listaFiltri = new List<Filter>();

                listaFiltri.Add(new Filter() { Name = "YEAR", Description = "Inserire il valore dell’anno dei documenti", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "IN_PROTOCOL", Description = "Con valore true cerca i protocolli in entrata", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "OUT_PROTOCOL", Description = "Con valore true cerca i protocolli in uscita", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "INTERNAL_PROTOCOL", Description = "Con valore true cerca i protocolli interni", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "NOT_PROTOCOL", Description = "Con valore true cerca i documenti non protocollati", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "PREDISPOSED", Description = "Con valore true cerca i predisposti", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "ATTACHMENTS", Description = "Con valore true cerca gli allegati", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "PRINTS", Description = "Con valore true cerca le stampe", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "NUM_PROTOCOL_FROM", Description = "Filtro utilizzato per la ricerca per intervallo su numero di protocollo, questo valore è il limite inferiore", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "NUM_PROTOCOL_TO", Description = "Filtro utilizzato per la ricerca per intervallo su numero di protocollo, questo valore è il limite superiore", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "CREATION_DATE_FROM", Description = "Filtro utilizzato per intervallo su data creazione, questo valore è il limite inferiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "CREATION_DATE_TO", Description = "Filtro utilizzato per intervallo su data creazione, questo valore è il limite superiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "PROTOCOL_DATE_FROM", Description = "Filtro utilizzato per intervallo su data protocollo, questo valore è il limite inferiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "PROTOCOL_DATE_TO", Description = "Filtro utilizzato per intervallo su data protocollo, questo valore è il limite superiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "SENDER_RECIPIENT", Description = "Filtro utilizzato sui mittenti/destinatari di un documento, inserire l’id del corrispondente", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "TEMPLATE", Description = "Filtro utilizzato per la ricerca della tipologia dei documenti", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "DOCNUMBER_FROM", Description = "Filtro utilizzato per intervallo sul docnumner questo valore è il limite inferiore", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "DOCNUMBER_TO", Description = "Filtro utilizzato per intervallo sul docnumner questo valore è il limite superiore", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "REGISTER", Description = "Filtro utilizzato con il codice del registro", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "OBJECT", Description = "Filtro utilizzato per la ricerca per oggetto di un documento", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "FULL_TEXT_SEARCH", Description = "Filtro utilizzato per la ricerca FullText", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "TEMPLATE_EXTRACTION", Description = "Filtro utilizzato per l'estrazione dei campi profilati di una tipologia. Necessita della presenza del filtro Template.", Type = FilterTypeEnum.String });

                GetFiltersResponse response = new GetFiltersResponse();
                response.Filters = listaFiltri;
                response.Code = GetFiltersResponseCode.OK;
                logger.Info("END - GetDocumentFilters");
                return response;

            }
            catch (Exception e)
            {
                logger.Error("eccezione GetDocumentFilters: " + e);
                GetFiltersResponse errorResp = new GetFiltersResponse();
                errorResp.Code = GetFiltersResponseCode.SYSTEM_ERROR;
                errorResp.ErrorMessage = e.Message;
                return errorResp;
            }
        }

        public static async Task<GetDocumentResponse> getDocument(string token, GetDocumentRequest request)
        {
            logger.Info("begin getDocument");
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];               

                #endregion
                GetDocumentResponse response = new GetDocumentResponse();

                Document responseDocument = new Document();

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();
                bool FileConFirma = false;
                if (!string.IsNullOrEmpty(request.GetFileWithSignature) && request.GetFileWithSignature == "1")
                    FileConFirma = true;
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
                    throw new Exception("Documento non trovato");
                }

                if (documento != null)
                {
                    //Dati in comune per tutti
                    responseDocument.Id = documento.systemId;
                    responseDocument.Object = documento.oggetto.descrizione;
                    if (!string.IsNullOrEmpty(documento.privato) && documento.privato.Equals("1"))
                    {
                        responseDocument.PrivateDocument = true;
                    }
                    else
                    {
                        responseDocument.PrivateDocument = false;
                    }
                    responseDocument.CreationDate = documento.dataCreazione;
                    responseDocument.DocNumber = documento.docNumber;
                    responseDocument.DocumentType = documento.tipoProto;

                    if (BusinessLogic.Documenti.DocumentConsolidation.IsConfigEnabled())
                    {
                        if (documento.ConsolidationState != null && documento.ConsolidationState.State > DocumentConsolidationStateEnum.None)
                        {
                            string message = string.Empty;
                            // Diabilitazione controlli su documento consolidato
                            if (documento.ConsolidationState.State == DocumentConsolidationStateEnum.Step1)
                            {
                                responseDocument.ConsolidationState = "ConsolidationState1";
                            }
                            else if (documento.ConsolidationState.State == DocumentConsolidationStateEnum.Step2)
                            {
                                responseDocument.ConsolidationState = "ConsolidationState2";
                            }
                        }
                    }
                    else
                    {
                        responseDocument.ConsolidationState = string.Empty;
                    }

                    if (documento.noteDocumento != null && documento.noteDocumento.Count > 0)
                    {
                        responseDocument.Note = new Note[documento.noteDocumento.Count];
                        int y = 0;
                        foreach (DocsPaVO.Note.InfoNota tempNot in documento.noteDocumento)
                        {
                            Note nota = new Note();
                            nota.Description = tempNot.Testo;
                            nota.Id = tempNot.Id;
                            nota.User = new User();
                            nota.User.Id = tempNot.UtenteCreatore.IdUtente;
                            nota.User.Description = tempNot.UtenteCreatore.DescrizioneUtente;
                            responseDocument.Note[y] = nota;
                            y++;
                        }
                    }

                    //Se il documento è in cestino
                    if (!string.IsNullOrEmpty(documento.inCestino) && documento.inCestino.Equals("1"))
                    {
                        responseDocument.InBasket = true;
                    }
                    else
                    {
                        responseDocument.InBasket = false;
                    }

                    if (documento.template != null)
                    {
                        responseDocument.Template = Utils.GetDetailsTemplateDoc(documento.template, documento.docNumber);
                    }

                    //Documento principale
                    if (documento.documenti != null && documento.documenti.Count > 0)
                    {
                        DocsPaVO.documento.FileRequest versioneCorrente = (DocsPaVO.documento.FileRequest)documento.documenti[0];
                        responseDocument.MainDocument = Utils.GetFile(versioneCorrente, request.GetFile, infoUtente, false, false, string.Empty, null, FileConFirma);
                    }

                    //Prendi allegati
                    if (documento.allegati != null && documento.allegati.Count > 0)
                    {
                        //int y = 0;
                        //responseDocument.Attachments = new File[documento.allegati.Count];
                        //foreach (DocsPaVO.documento.FileRequest tempAll in documento.allegati)
                        //{
                        //    responseDocument.Attachments[y] = Utils.GetFile(tempAll, request.GetFile, infoUtente, false, false, string.Empty, null, FileConFirma);
                        //    y++;
                        //}

                        if (string.IsNullOrEmpty(infoUtente.codWorkingApplication) || (infoUtente.codWorkingApplication.ToUpper() != "ALBO_TELEMATICO" && infoUtente.codWorkingApplication.ToUpper() != "GDOC" && !BusinessLogic.Amministrazione.SistemiEsterni.IsVerticalePubbAlbo(infoUtente.codWorkingApplication)))
                        {
                            int y = 0;
                            responseDocument.Attachments = new File[documento.allegati.Count];
                            foreach (DocsPaVO.documento.FileRequest tempAll in documento.allegati)
                            {
                                responseDocument.Attachments[y] = Utils.GetFile(tempAll, request.GetFile, infoUtente, false, false, string.Empty, null, FileConFirma);
                                y++;
                            }
                        }
                        else
                        {
                            ArrayList allegatiAlbo = new ArrayList();
                            foreach (DocsPaVO.documento.FileRequest tempAll in documento.allegati)
                            {
                                if (infoUtente.codWorkingApplication.ToUpper() == "GDOC")
                                {
                                    if (BusinessLogic.Documenti.AllegatiManager.getIsAllegatoEsterno(tempAll.versionId) != "1" &&
                                        BusinessLogic.Documenti.AllegatiManager.getIsAllegatoIS(tempAll.versionId) != "1" &&
                                        BusinessLogic.Documenti.AllegatiManager.getIsAllegatoPEC(tempAll.versionId) != "1")
                                    {
                                        allegatiAlbo.Add(Utils.GetFile(tempAll, request.GetFile, infoUtente, false, false, string.Empty, null, FileConFirma));

                                    }
                                }
                                else
                                {
                                    var AllegatiDaNonPubblicare = BusinessLogic.Amministrazione.SistemiEsterni.Albo_GetFileDaPubblicare(documento.systemId, "N");
                                    if (BusinessLogic.Documenti.AllegatiManager.getIsAllegatoEsterno(tempAll.versionId) != "1" &&
                                        BusinessLogic.Documenti.AllegatiManager.getIsAllegatoIS(tempAll.versionId) != "1" &&
                                        BusinessLogic.Documenti.AllegatiManager.getIsAllegatoPEC(tempAll.versionId) != "1" &&
                                        (AllegatiDaNonPubblicare == null || !AllegatiDaNonPubblicare.Contains(tempAll.docNumber)))
                                    {
                                        allegatiAlbo.Add(Utils.GetFile(tempAll, request.GetFile, infoUtente, false, false, string.Empty, null, FileConFirma));

                                    }
                                }
                            }
                            if (allegatiAlbo.Count > 0)
                                responseDocument.Attachments = (File[])allegatiAlbo.ToArray(typeof(File));
                            else
                                responseDocument.Attachments = null;
                        }

                    }

                    if (documento.protocollo != null)
                    {

                        if (documento.protocollo.protocolloAnnullato != null)
                        {
                            responseDocument.Annulled = true;
                            //responseDocument.AnnulmentDate = documento.protocollo.protocolloAnnullato.dataAnnullamento;
                            //responseDocument.ConsolidationState = documento.ConsolidationState.State;
                        }
                        else
                        {
                            responseDocument.Annulled = false;
                        }

                        if (!string.IsNullOrEmpty(documento.protocollo.segnatura))
                        {
                            responseDocument.Signature = documento.protocollo.segnatura;
                        }
                        else
                        {
                            responseDocument.Predisposed = true;
                        }

                        if (!string.IsNullOrEmpty(documento.protocollo.dataProtocollazione))
                        {
                            responseDocument.ProtocolDate = documento.protocollo.dataProtocollazione;
                        }
                        if (!string.IsNullOrEmpty(documento.protocollo.numero))
                        {
                            responseDocument.ProtocolNumber = documento.protocollo.numero;
                        }
                        if (!string.IsNullOrEmpty(documento.protocollo.anno))
                        {
                            responseDocument.ProtocolYear = documento.protocollo.anno;
                        }
                        

                        if (documento.registro != null)
                        {
                            responseDocument.Register = Utils.GetRegister(documento.registro);
                        }


                        //CASO PROTOCOLLO IN ARRIVO
                        if (documento.tipoProto.Equals("A"))
                        {
                            if ((ProtocolloEntrata)documento.protocollo != null && ((ProtocolloEntrata)documento.protocollo).mittente != null)
                            {
                                responseDocument.Sender = Utils.GetCorrespondent(((ProtocolloEntrata)documento.protocollo).mittente, infoUtente);
                            }

                            if ((ProtocolloEntrata)documento.protocollo != null && ((ProtocolloEntrata)documento.protocollo).mittenti != null && ((ProtocolloEntrata)documento.protocollo).mittenti.Count > 0)
                            {
                                responseDocument.MultipleSenders = new Correspondent[((ProtocolloEntrata)documento.protocollo).mittenti.Count];
                                int y = 0;
                                foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloEntrata)documento.protocollo).mittenti)
                                {
                                    responseDocument.MultipleSenders[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                                    y++;
                                }
                            }

                            if (((ProtocolloEntrata)documento.protocollo).descrizioneProtocolloMittente != null)
                            {
                                responseDocument.ProtocolSender = ((ProtocolloEntrata)documento.protocollo).descrizioneProtocolloMittente;
                            }

                            if (((ProtocolloEntrata)documento.protocollo) != null && !string.IsNullOrEmpty(((ProtocolloEntrata)documento.protocollo).dataProtocolloMittente))
                            {
                                responseDocument.DataProtocolSender = ((ProtocolloEntrata)documento.protocollo).dataProtocolloMittente;
                            }
                            if (((ProtocolloEntrata)documento.protocollo) != null)
                            {
                                if (!string.IsNullOrEmpty(((ProtocolloEntrata)documento.protocollo).numero))
                        {
                            responseDocument.ProtocolNumber = ((ProtocolloEntrata)documento.protocollo).numero;
                        }
                                if (!string.IsNullOrEmpty(((ProtocolloEntrata)documento.protocollo).anno))
                        {
                            responseDocument.ProtocolYear = ((ProtocolloEntrata)documento.protocollo).anno;
                        }
                            }

                            if (((ProtocolloEntrata)documento.protocollo).descMezzoSpedizione != null)
                            {
                                responseDocument.MeansOfSending = ((ProtocolloEntrata)documento.protocollo).descMezzoSpedizione;
                                //responseDocument.MeansOfSending = documento.descMezzoSpedizione;
                            }

                            if(!string.IsNullOrWhiteSpace(documento.descMezzoSpedizione))
                                responseDocument.MeansOfSending = documento.descMezzoSpedizione;

                            if (((ProtocolloEntrata)documento.protocollo) != null && string.IsNullOrEmpty(((ProtocolloEntrata)documento.protocollo).dataProtocolloMittente))
                            {
                                responseDocument.ProtocolDate = ((ProtocolloEntrata)documento.protocollo).dataProtocollazione;
                            }

                            if (documento.documenti != null && documento.documenti != null && documento.documenti.Count > 0)
                            {
                                responseDocument.ArrivalDate = ((DocsPaVO.documento.Documento)documento.documenti[0]).dataArrivo;
                            }
                        }

                        //CASO PROTOCOLLO IN PARTENZA
                        if (documento.tipoProto.Equals("P"))
                        {
                            if ((ProtocolloUscita)documento.protocollo != null && ((ProtocolloUscita)documento.protocollo).mittente != null)
                            {
                                responseDocument.Sender = Utils.GetCorrespondent(((ProtocolloUscita)documento.protocollo).mittente, infoUtente);
                            }

                            if ((ProtocolloUscita)documento.protocollo != null && ((ProtocolloUscita)documento.protocollo).destinatari != null && ((ProtocolloUscita)documento.protocollo).destinatari.Count > 0)
                            {
                                responseDocument.Recipients = new Correspondent[((ProtocolloUscita)documento.protocollo).destinatari.Count];
                                int y = 0;
                                foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloUscita)documento.protocollo).destinatari)
                                {
                                    responseDocument.Recipients[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                                    y++;
                                }
                            }

                            if ((ProtocolloUscita)documento.protocollo != null && ((ProtocolloUscita)documento.protocollo).destinatariConoscenza != null && ((ProtocolloUscita)documento.protocollo).destinatariConoscenza.Count > 0)
                            {
                                responseDocument.RecipientsCC = new Correspondent[((ProtocolloUscita)documento.protocollo).destinatariConoscenza.Count];
                                int y = 0;
                                foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloUscita)documento.protocollo).destinatariConoscenza)
                                {
                                    responseDocument.RecipientsCC[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                                    y++;
                                }
                            }
                        }

                        //CASO PROTOCOLLO INTERNO
                        if (documento.tipoProto.Equals("I"))
                        {
                            if ((ProtocolloInterno)documento.protocollo != null && ((ProtocolloInterno)documento.protocollo).mittente != null)
                            {
                                responseDocument.Sender = Utils.GetCorrespondent(((ProtocolloInterno)documento.protocollo).mittente, infoUtente);
                            }

                            if ((ProtocolloInterno)documento.protocollo != null && ((ProtocolloInterno)documento.protocollo).destinatari != null && ((ProtocolloInterno)documento.protocollo).destinatari.Count > 0)
                            {
                                responseDocument.Recipients = new Correspondent[((ProtocolloInterno)documento.protocollo).destinatari.Count];
                                int y = 0;
                                foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloInterno)documento.protocollo).destinatari)
                                {
                                    responseDocument.Recipients[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                                    y++;
                                }
                            }

                            if ((ProtocolloInterno)documento.protocollo != null && ((ProtocolloInterno)documento.protocollo).destinatariConoscenza != null && ((ProtocolloInterno)documento.protocollo).destinatariConoscenza.Count > 0)
                            {
                                responseDocument.RecipientsCC = new Correspondent[((ProtocolloInterno)documento.protocollo).destinatariConoscenza.Count];
                                int y = 0;
                                foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloInterno)documento.protocollo).destinatariConoscenza)
                                {
                                    responseDocument.RecipientsCC[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                                    y++;
                                }
                            }
                        }
                    }

                    // Prelievo Documenti collegati
                    ArrayList linkedDocs = BusinessLogic.Amministrazione.SistemiEsterni.getLinkedDocRest(documento.systemId);
                    if (linkedDocs != null && linkedDocs.Count > 0)
                    {
                        List<LinkedDocument> linkedDocs2 = new List<LinkedDocument>();
                        foreach(DocsPaVO.documento.BaseInfoDoc tempDoc in linkedDocs)
                        {
                            if(tempDoc.VersionLabel.ToUpper() == "PARENT")
                            {
                                responseDocument.ParentDocument = Utils.getLinkedDoc(tempDoc);
                            }
                            else if (tempDoc.VersionLabel.ToUpper() == "LINKED")
                            {
                                linkedDocs2.Add(Utils.getLinkedDoc(tempDoc));
                            }
                        }
                        if (linkedDocs2 != null && linkedDocs2.Count > 0)
                        {
                            responseDocument.LinkedDocuments = linkedDocs2.ToArray();
                        }
                    }
                }
                else
                {
                    throw new Exception("Documento non trovato");
                }

                response.Document = responseDocument;


                response.Code = GetDocumentResponseCode.OK;
                logger.Info("Controllo CodeApplication: " + infoUtente.codWorkingApplication);
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETDETTAGLIODOCUMENTO", request.IdDocument, "Aperto dettaglio del documento " + request.IdDocument, DocsPaVO.Logger.CodAzione.Esito.OK);

                logger.Info("END - GetDocument");
                return response;

            }
            catch (Exception e)
            {
                logger.Error("eccezione GetDocument: " + e);
                GetDocumentResponse errorResp = new GetDocumentResponse();
                errorResp.Code = GetDocumentResponseCode.SYSTEM_ERROR;
                errorResp.ErrorMessage = e.Message;
                return errorResp;
            }
        }

        public static async Task<CreateDocumentResponse> createDocument(string token, CreateDocumentRequest request,bool frompregressi=false)
        {
            logger.Info("begin createDocument");
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion

                bool setStatoDiagrammaIniziale = false;
                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                DocsPaVO.DiagrammaStato.Stato statoIniziale = null;
                bool noClassificationRequired = false;
                //se viene chiamato da importpreviousdocument non deve fare il controllo sull'obbligatorietà della fasc/class
                if (frompregressi)
                {
                    if (!noClassificationRequired)
                    {
                        //Se è attiva la fascicolazione obbligatoria non è possibile utilizzare questo metodo
                        string fascRapidaRequired = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "FE_FASC_RAPIDA_REQUIRED");
                        if (!string.IsNullOrEmpty(fascRapidaRequired) && fascRapidaRequired.Equals("TRUE"))
                            throw new Exception("Richiesta la fascicolazione rapida.");
                    }
                }

                CreateDocumentResponse response = new CreateDocumentResponse();
                if (request != null && string.IsNullOrEmpty(request.Document.DocumentType))
                {
                    throw new RestException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && string.IsNullOrEmpty(request.Document.Object))
                {
                    throw new RestException("MISSING_OBJECT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (!request.Document.DocumentType.ToUpper().Equals("A") && !request.Document.DocumentType.ToUpper().Equals("P") && !request.Document.DocumentType.ToUpper().Equals("I") && !request.Document.DocumentType.ToUpper().Equals("G")))
                {
                    throw new RestException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (request.Document.DocumentType.ToUpper().Equals("A") || request.Document.DocumentType.ToUpper().Equals("P") || request.Document.DocumentType.ToUpper().Equals("I")) && string.IsNullOrEmpty(request.CodeRegister))
                {
                    throw new RestException("REQUIRED_REGISTER");
                }

                if (request != null && request.Document != null && request.Document.MainDocument != null && request.Document.MainDocument.Content != null && request.Document.MainDocument.Content.Length > 0)
                {
                    int maxfilesize = Utils.maxFileSizePermitted();
                    if (maxfilesize > 0 && request.Document.MainDocument.Content.Length > maxfilesize)
                    {
                        throw new Exception("Dimensione del file troppo grande");
                    }
                }

                //Reperimento ruolo utente

                DocsPaVO.utente.Corrispondente mittente = new DocsPaVO.utente.Corrispondente();

                try
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                    if (ruolo == null)
                    {
                        //Ruolo non trovato
                        throw new RestException("ROLE_NO_EXIST");
                    }
                }
                catch
                {
                    //Ruolo non trovato
                    throw new RestException("ROLE_NO_EXIST");
                }


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
                        Correspondent c = request.Document.MultipleSenders[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }

                if (request.Document.Recipients != null)
                {
                    for (int i = 0; i < request.Document.Recipients.Length; i++)
                    {
                        Correspondent c = request.Document.Recipients[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }

                if (request.Document.RecipientsCC != null)
                {
                    for (int i = 0; i < request.Document.RecipientsCC.Length; i++)
                    {
                        Correspondent c = request.Document.RecipientsCC[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }
                //


                //Creazione schedaDocumento con i dati essenziali e obbligatori
                DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.NewSchedaDocumento(infoUtente, false);

                //Imposto l'oggetto
                if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.Object))
                {
                    schedaDoc.oggetto.descrizione = request.Document.Object;
                    schedaDoc.oggetto.daAggiornare = true;
                }

                //Imposto le note, soltanto visibili a tutti
                if (request != null && request.Document != null && request.Document != null && request.Document.Note != null && request.Document.Note.Length > 0)
                {
                    schedaDoc.noteDocumento = new List<DocsPaVO.Note.InfoNota>();
                    foreach (Note nota in request.Document.Note)
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

                //Imposto il template
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
                            throw new RestException("TEMPLATE_NOT_ROLE_EDITABLE");
                        }
                        // Modifica Lembo 11-01-2013: Utilizzo il metodo che controlla la visibilità dei campi.
                        //schedaDoc.template = Utils.GetTemplateFromPis(request.Document.Template, template, false);

                        // Modifica Elaborazione XML da PIS req.2

                        File fileDaPassare = ((request.Document.MainDocument != null && request.Document.MainDocument.Content != null) ? request.Document.MainDocument : null);

                        schedaDoc.template = Utils.GetTemplateFromPisVisibility(request.Document.Template, template, false, infoUtente.idGruppo, "D", "", infoUtente, fileDaPassare, request.CodeRegister, request.CodeRF);
                        schedaDoc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                        schedaDoc.tipologiaAtto.systemId = template.SYSTEM_ID.ToString();
                        schedaDoc.tipologiaAtto.descrizione = template.DESCRIZIONE.ToString();
                        int idDiagramma = 0;
                        idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociato(schedaDoc.template.SYSTEM_ID.ToString());
                        if (idDiagramma != 0)
                        {
                            setStatoDiagrammaIniziale = true;
                            diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                            if (diagramma != null)
                            {
                                if (diagramma.STATI != null && diagramma.STATI.Count > 0)
                                {
                                    foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                    {
                                        if (stato.STATO_INIZIALE == true) statoIniziale = stato;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //Template non trovato
                        throw new RestException("TEMPLATE_NOT_FOUND");
                    }
                }
                //

                //Se privato
                if (request != null && request.Document != null && request.Document.PrivateDocument)
                {
                    schedaDoc.privato = "1";
                }
                //

                //Se personale
                if (request != null && request.Document != null && request.Document.PersonalDocument)
                {
                    schedaDoc.personale = "1";
                }
                //

                DocsPaVO.documento.SchedaDocumento schedaDocResult = null;
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

                    //if (request.Document.Sender == null || string.IsNullOrEmpty(request.Document.Sender.CorrespondentType) || string.IsNullOrEmpty(request.Document.Sender.Id))
                        if (request.Document.Sender == null || string.IsNullOrEmpty(request.Document.Sender.CorrespondentType))
                        {
                        if (!request.Document.DocumentType.ToUpper().Equals("P"))
                        {
                            //Mittente non presente
                            throw new RestException("REQUIRED_SENDER");
                        }
                        else
                        {
                            mittente = Utils.RisolviCorrispondenteDisabled(ruolo.uo.systemId, infoUtente);
                        }
                    }
                    else
                    {
                        if (request.Document.Sender != null && !string.IsNullOrEmpty(request.Document.Sender.CorrespondentType) && request.Document.Sender.CorrespondentType.Equals("O"))
                        {
                            //mittente = Utils.GetCorrespondentFromPis(request.Document.Sender, infoUtente);
                            mittente = Utils.GetCorrespondentFromPisNewInsertOccasionale(request.Document.Sender, infoUtente);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(request.Document.Sender.Id))
                            {
                                mittente = Utils.RisolviCorrispondenteDisabled(request.Document.Sender.Id, infoUtente);
                                if (mittente == null)
                                    //Mittente non trovato
                                    throw new RestException("SENDER_NOT_FOUND");
                            }
                            else
                            {
                                throw new RestException("REQUIRED_SENDER");
                            }
                        }
                    }

                    if (request.Document.DocumentType.ToUpper().Equals("I") || request.Document.DocumentType.ToUpper().Equals("P"))
                    {
                        if (request.Document.Recipients == null || request.Document.Recipients.Length == 0)
                        {
                            //Destinatario non presente
                            throw new RestException("REQUIRED_RECIPIENT");
                        }
                    }

                    if (string.IsNullOrEmpty(request.CodeRegister))
                    {
                        //Registro mancante
                        throw new RestException("REQUIRED_REGISTER");
                    }
                    else
                    {
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeRegister);
                        if (reg == null)
                        {
                            //Registro mancante
                            throw new RestException("REGISTER_NOT_FOUND");
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
                            throw new RestException("RF_NOT_FOUND");
                        }
                    }
                }

                //CASO PROTOCOLLO IN ARRIVO (ANCHE PREDISPOSTO)
                if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("A"))
                {
                    schedaDoc.tipoProto = "A";
                    schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloEntrata();

                    ((ProtocolloEntrata)schedaDoc.protocollo).mittente = mittente;

                    if (request.Document.MultipleSenders != null && request.Document.MultipleSenders.Length > 0)
                    {
                        ((ProtocolloEntrata)schedaDoc.protocollo).mittenti = new ArrayList();
                        foreach (Correspondent corrTemp in request.Document.MultipleSenders)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = null;
                            if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                            {
                                //corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                corrBySys = Utils.GetCorrespondentFromPisNewInsertOccasionale(corrTemp, infoUtente);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(corrTemp.Id))
                                {
                                    corrBySys = Utils.RisolviCorrispondenteDisabled(corrTemp.Id, infoUtente);
                                    if (corrBySys == null)
                                    {
                                        //Mittente non trovato
                                        throw new RestException("SENDER_NOT_FOUND");
                                    }
                                }
                            }
                            ((ProtocolloEntrata)schedaDoc.protocollo).mittenti.Add(corrBySys);
                        }
                    }

                    //Data protocollo mittente
                    if (!string.IsNullOrEmpty(request.Document.DataProtocolSender))
                    {
                        ((ProtocolloEntrata)schedaDoc.protocollo).dataProtocolloMittente = request.Document.DataProtocolSender;
                    }

                    //Protocollo mittente
                    if (!string.IsNullOrEmpty(request.Document.ProtocolSender))
                    {
                        ((ProtocolloEntrata)schedaDoc.protocollo).descrizioneProtocolloMittente = request.Document.ProtocolSender;
                    }

                    //Data arrivo TODO
                    if (!string.IsNullOrEmpty(request.Document.ArrivalDate))
                    {
                        // Variabile Booleana per il controllo di validità della data passata in input;
                        bool valid = false;
                        // Data ritornata in output dal TryParseExact per la data arrivo passata in input
                        DateTime dateVal;

                        // Pattern di validità per una data valida
                        string pattern = "dd/MM/yyyy HH:mm:ss";
                        string pattern2 = "dd/MM/yyyy HH:mm";

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
                                if (!DateTime.TryParseExact(request.Document.ArrivalDate, pattern2, null, System.Globalization.DateTimeStyles.None, out dateVal))
                                {
                                    throw new Exception("Formato Data non corretto");
                                }
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

                            if (!DateTime.TryParseExact(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), pattern, null, System.Globalization.DateTimeStyles.None, out dateVal2))
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
                                    ((Documento)schedaDoc.documenti[0]).dataArrivo = request.Document.ArrivalDate;
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
                                    schedaDocResult.descMezzoSpedizione = request.Document.MeansOfSending;
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

                    ((ProtocolloUscita)schedaDoc.protocollo).mittente = mittente;

                    if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                    {
                        ((ProtocolloUscita)schedaDoc.protocollo).destinatari = new ArrayList();
                        foreach (Correspondent corrTemp in request.Document.Recipients)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = null;

                            // Verifica se occasionale
                            if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                            {
                                //corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                corrBySys = Utils.GetCorrespondentFromPisNewInsertOccasionale(corrTemp, infoUtente);
                            }
                            else
                            {
                                // Se non è occasionale, lo risolve sulla rubrica
                                corrBySys = Utils.RisolviCorrispondenteDisabled(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new RestException("RECIPIENT_NOT_FOUND");
                                }

                            }
                            if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "F" && corrBySys.tipoIE == null)
                            {
                                ArrayList corrByRF = BusinessLogic.Rubrica.RF.getCorrispondentiByCodRF(corrBySys.codiceRubrica);
                                foreach (DocsPaVO.utente.Corrispondente c2 in corrByRF)
                                {
                                    ((ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(c2);
                                }
                            }
                            else if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                            {
                                ArrayList corrByLista = BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiByCodLista(corrBySys.codiceRubrica, infoUtente.idAmministrazione, infoUtente);
                                foreach (DocsPaVO.utente.Corrispondente c3 in corrByLista)
                                {
                                    ((ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(c3);
                                }
                            }
                            else
                            {
                                ((ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(corrBySys);
                            }
                        }
                    }

                    if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                    {
                        ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                        foreach (Correspondent corrTemp in request.Document.RecipientsCC)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = null;

                            // Verifica se occasionale
                            if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                            {
                                //corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                corrBySys = Utils.GetCorrespondentFromPisNewInsertOccasionale(corrTemp, infoUtente);
                            }
                            else
                            {
                                corrBySys = Utils.RisolviCorrispondenteDisabled(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new RestException("RECIPIENT_NOT_FOUND");
                                }
                            }
                            if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "F" && corrBySys.tipoIE == null)
                            {
                                ArrayList corrByRF = BusinessLogic.Rubrica.RF.getCorrispondentiByCodRF(corrBySys.codiceRubrica);
                                foreach (DocsPaVO.utente.Corrispondente c2 in corrByRF)
                                {
                                    ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(c2);
                                }
                            }
                            else if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                            {
                                ArrayList corrByLista = BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiByCodLista(corrBySys.codiceRubrica, infoUtente.idAmministrazione, infoUtente);
                                foreach (DocsPaVO.utente.Corrispondente c3 in corrByLista)
                                {
                                    ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(c3);
                                }
                            }
                            else
                            {
                                ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
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

                    ((ProtocolloInterno)schedaDoc.protocollo).mittente = mittente;

                    if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                    {
                        ((ProtocolloInterno)schedaDoc.protocollo).destinatari = new ArrayList();
                        foreach (Correspondent corrTemp in request.Document.Recipients)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = Utils.RisolviCorrispondenteDisabled(corrTemp.Id, infoUtente);
                            if (corrBySys == null)
                            {
                                //Destinatario non trovato
                                throw new RestException("RECIPIENT_NOT_FOUND");
                            }
                            ((ProtocolloInterno)schedaDoc.protocollo).destinatari.Add(corrBySys);
                        }
                    }

                    if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                    {
                        ((ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                        foreach (Correspondent corrTemp in request.Document.RecipientsCC)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = Utils.RisolviCorrispondenteDisabled(corrTemp.Id, infoUtente);
                            if (corrBySys == null)
                            {
                                //Destinatario non trovato
                                throw new RestException("RECIPIENT_NOT_FOUND");
                            }
                            ((ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
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

                //Se ho anche un file da aggiungere
                if (schedaDocResult != null && !string.IsNullOrEmpty(schedaDocResult.docNumber) && request.Document.MainDocument != null)
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
                        throw new RestException("FILE_CREATION_ERROR");
                    }


                }
                // Gestione dei linked Documents
                

                if (schedaDocResult != null)
                {
                    if (setStatoDiagrammaIniziale)
                    {
                        BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(schedaDocResult.systemId, statoIniziale.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);

                    }
                    response.Document = Utils.GetDetailsDocument(schedaDocResult, infoUtente, false);
                    #region gestione Linked Docs
                    if (request.Document.ParentDocument != null)
                    {
                        if (BusinessLogic.Amministrazione.SistemiEsterni.updLinkedDocRest(request.Document.ParentDocument.Id, schedaDocResult.systemId))
                            response.Document.ParentDocument = request.Document.ParentDocument;
                    }
                    if (request.Document.LinkedDocuments != null && request.Document.LinkedDocuments.Length>0)
                    {
                        foreach (LinkedDocument ld in request.Document.LinkedDocuments)
                        {
                            if (ld.LinkType == "PARENT")
                            {
                                BusinessLogic.Amministrazione.SistemiEsterni.updLinkedDocRest(ld.Id, schedaDocResult.systemId);
                            }
                            else if(ld.LinkType=="LINKED")
                            {
                                BusinessLogic.Amministrazione.SistemiEsterni.updLinkedDocRest(schedaDocResult.systemId, ld.Id);
                            }
                        }
                    }
                    ArrayList linkedDocs = BusinessLogic.Amministrazione.SistemiEsterni.getLinkedDocRest(schedaDocResult.systemId);
                    if (linkedDocs != null && linkedDocs.Count > 0)
                    {
                        List<LinkedDocument> linkedDocs2 = new List<LinkedDocument>();
                        foreach (DocsPaVO.documento.BaseInfoDoc tempDoc in linkedDocs)
                        {
                            if (tempDoc.VersionLabel.ToUpper() == "PARENT")
                            {
                                response.Document.ParentDocument = Utils.getLinkedDoc(tempDoc);
                            }
                            else if (tempDoc.VersionLabel.ToUpper() == "LINKED")
                            {
                                linkedDocs2.Add(Utils.getLinkedDoc(tempDoc));
                            }
                        }
                        if (linkedDocs2 != null && linkedDocs2.Count > 0)
                        {
                            response.Document.LinkedDocuments = linkedDocs2.ToArray();
                        }
                    }
                    #endregion

                    if (setStatoDiagrammaIniziale)
                        response.Document.Template.StateDiagram.StateOfDiagram[0] = Utils.GetStateOfDiagram(statoIniziale, diagramma.SYSTEM_ID.ToString());

                    if (!string.IsNullOrEmpty(infoUtente.diSistema) && infoUtente.diSistema == "1")
                    {
                        bool pulito = Utils.CleanRightsExtSys(schedaDocResult.docNumber, infoUtente.idPeople, infoUtente.idGruppo);
                    }                   

                }
                response.Code = CreateDocumentResponseCode.OK;
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOADDDOCGRIGIA", schedaDocResult.docNumber, "Creazione del nuovo documento " + schedaDocResult.docNumber + ". Tipo documento: " + schedaDocResult.tipoProto + ". Predisposto: " + schedaDocResult.predisponiProtocollazione.ToString(), DocsPaVO.Logger.CodAzione.Esito.OK);

                logger.Info("END - createDocument");
                return response;
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                CreateDocumentResponse errorResp = new CreateDocumentResponse();
                errorResp.Code = CreateDocumentResponseCode.SYSTEM_ERROR;
                errorResp.ErrorMessage = pisEx.Description;
                return errorResp;

            }
            catch (Exception e)
            {
                logger.Error("eccezione createDocument: " + e);
                CreateDocumentResponse errorResp = new CreateDocumentResponse();
                errorResp.Code = CreateDocumentResponseCode.SYSTEM_ERROR;
                errorResp.ErrorMessage = e.Message;
                return errorResp;
            }
        }

        public static async Task<CreateDocumentResponse> createDocAndAddInPrj(string token, CreateDocAndAddInPrjRequest request)
        {
            logger.Info("begin createDocAndAddInPrj");
            CreateDocumentResponse response = new CreateDocumentResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                #region GetProject
                if (string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject) && string.IsNullOrEmpty(request.IdProject))
                {
                    throw new RestException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) || !string.IsNullOrEmpty(request.CodeProject)) && !string.IsNullOrEmpty(request.IdProject))
                {
                    throw new RestException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject)) || (string.IsNullOrEmpty(request.ClassificationSchemeId) && !string.IsNullOrEmpty(request.CodeProject)))
                {
                    throw new RestException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }

                Project responseProject = new Project();
                bool inserimentoInSottoFasc = false;
                DocsPaVO.fascicolazione.Fascicolo fascicolo = null;

                try
                {
                    if (!string.IsNullOrEmpty(request.IdProject))
                    {
                        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.IdProject, infoUtente);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.CodeProject) && (!string.IsNullOrEmpty(request.ClassificationSchemeId)))
                        {
                            string codiceFasc = "";
                            if (request.CodeProject.IndexOf("//") > -1)
                            {
                                string separatore = "//";
                                // MODIFICA per inserimento in sottocartelle
                                string[] separatoreAr = new string[] { separatore };

                                string[] pathCartelle = request.CodeProject.Split(separatoreAr, StringSplitOptions.RemoveEmptyEntries);
                                codiceFasc = pathCartelle[0];
                                inserimentoInSottoFasc = true;
                            }
                            else
                                codiceFasc = request.CodeProject;

                            DocsPaVO.fascicolazione.Fascicolo[] fascicoli = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceNoSecurity(codiceFasc, infoUtente.idAmministrazione, request.ClassificationSchemeId, false);
                            if (fascicoli != null && fascicoli.Length > 0)
                            {
                                fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(fascicoli[0].systemID, infoUtente);
                                if (fascicolo == null && fascicoli.Length > 1)
                                {
                                    fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(fascicoli[1].systemID, infoUtente);
                                }
                            }
                            else
                            {
                                throw new RestException("PROJECT_NOT_FOUND");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new RestException("PROJECT_NOT_FOUND");
                }

                if (fascicolo == null)
                {
                    throw new RestException("PROJECT_NOT_FOUND");
                }
                else
                {
                    if(fascicolo.stato == "C")
                        throw new RestException("PROJECT_NOT_FOUND");
                }
                #endregion
                #region CreateDocument
                bool setStatoDiagrammaIniziale = false;
                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                DocsPaVO.DiagrammaStato.Stato statoIniziale = null;
                
                if (request != null && string.IsNullOrEmpty(request.Document.DocumentType))
                {
                    throw new RestException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && string.IsNullOrEmpty(request.Document.Object))
                {
                    throw new RestException("MISSING_OBJECT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (!request.Document.DocumentType.ToUpper().Equals("A") && !request.Document.DocumentType.ToUpper().Equals("P") && !request.Document.DocumentType.ToUpper().Equals("I") && !request.Document.DocumentType.ToUpper().Equals("G")))
                {
                    throw new RestException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (request.Document.DocumentType.ToUpper().Equals("A") || request.Document.DocumentType.ToUpper().Equals("P") || request.Document.DocumentType.ToUpper().Equals("I")) && string.IsNullOrEmpty(request.CodeRegister))
                {
                    throw new RestException("REQUIRED_REGISTER");
                }

                if (request != null && request.Document != null && request.Document.MainDocument != null && request.Document.MainDocument.Content != null && request.Document.MainDocument.Content.Length > 0)
                {
                    int maxfilesize = Utils.maxFileSizePermitted();
                    if (maxfilesize > 0 && request.Document.MainDocument.Content.Length > maxfilesize)
                    {
                        throw new Exception("Dimensione del file troppo grande");
                    }
                }

                //Reperimento ruolo utente

                DocsPaVO.utente.Corrispondente mittente = new DocsPaVO.utente.Corrispondente();

                try
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                    if (ruolo == null)
                    {
                        //Ruolo non trovato
                        throw new RestException("ROLE_NO_EXIST");
                    }
                }
                catch
                {
                    //Ruolo non trovato
                    throw new RestException("ROLE_NO_EXIST");
                }


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
                        Correspondent c = request.Document.MultipleSenders[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }

                if (request.Document.Recipients != null)
                {
                    for (int i = 0; i < request.Document.Recipients.Length; i++)
                    {
                        Correspondent c = request.Document.Recipients[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }

                if (request.Document.RecipientsCC != null)
                {
                    for (int i = 0; i < request.Document.RecipientsCC.Length; i++)
                    {
                        Correspondent c = request.Document.RecipientsCC[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }
                //


                //Creazione schedaDocumento con i dati essenziali e obbligatori
                DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.NewSchedaDocumento(infoUtente, false);

                //Imposto l'oggetto
                if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.Object))
                {
                    schedaDoc.oggetto.descrizione = request.Document.Object;
                    schedaDoc.oggetto.daAggiornare = true;
                }

                //Imposto le note, soltanto visibili a tutti
                if (request != null && request.Document != null && request.Document != null && request.Document.Note != null && request.Document.Note.Length > 0)
                {
                    schedaDoc.noteDocumento = new List<DocsPaVO.Note.InfoNota>();
                    foreach (Note nota in request.Document.Note)
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

                //Imposto il template
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
                            throw new RestException("TEMPLATE_NOT_ROLE_EDITABLE");
                        }
                        // Modifica Lembo 11-01-2013: Utilizzo il metodo che controlla la visibilità dei campi.
                        //schedaDoc.template = Utils.GetTemplateFromPis(request.Document.Template, template, false);

                        // Modifica Elaborazione XML da PIS req.2

                        File fileDaPassare = ((request.Document.MainDocument != null && request.Document.MainDocument.Content != null) ? request.Document.MainDocument : null);

                        schedaDoc.template = Utils.GetTemplateFromPisVisibility(request.Document.Template, template, false, infoUtente.idGruppo, "D", "", infoUtente, fileDaPassare, request.CodeRegister, request.CodeRF);
                        schedaDoc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                        schedaDoc.tipologiaAtto.systemId = template.SYSTEM_ID.ToString();
                        schedaDoc.tipologiaAtto.descrizione = template.DESCRIZIONE.ToString();
                        int idDiagramma = 0;
                        idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociato(schedaDoc.template.SYSTEM_ID.ToString());
                        if (idDiagramma != 0)
                        {
                            setStatoDiagrammaIniziale = true;
                            diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                            if (diagramma != null)
                            {
                                if (diagramma.STATI != null && diagramma.STATI.Count > 0)
                                {
                                    foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                    {
                                        if (stato.STATO_INIZIALE == true) statoIniziale = stato;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //Template non trovato
                        throw new RestException("TEMPLATE_NOT_FOUND");
                    }
                }
                //

                //Se privato
                if (request != null && request.Document != null && request.Document.PrivateDocument)
                {
                    schedaDoc.privato = "1";
                }
                //

                //Se personale
                if (request != null && request.Document != null && request.Document.PersonalDocument)
                {
                    schedaDoc.personale = "1";
                }
                //

                DocsPaVO.documento.SchedaDocumento schedaDocResult = null;
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
                            throw new RestException("REQUIRED_SENDER");
                        }
                        else
                        {
                            mittente = Utils.RisolviCorrispondente(ruolo.uo.systemId, infoUtente);
                        }
                    }
                    else
                    {
                        if (request.Document.Sender != null && !string.IsNullOrEmpty(request.Document.Sender.CorrespondentType) && request.Document.Sender.CorrespondentType.Equals("O"))
                        {
                            //mittente = Utils.GetCorrespondentFromPis(request.Document.Sender, infoUtente);
                            mittente = Utils.GetCorrespondentFromPisNewInsertOccasionale(request.Document.Sender, infoUtente);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(request.Document.Sender.Id))
                            {
                                mittente = Utils.RisolviCorrispondente(request.Document.Sender.Id, infoUtente);
                                if (mittente == null)
                                    //Mittente non trovato
                                    throw new RestException("SENDER_NOT_FOUND");
                            }
                        }
                    }

                    if (request.Document.DocumentType.ToUpper().Equals("I") || request.Document.DocumentType.ToUpper().Equals("P"))
                    {
                        if (request.Document.Recipients == null || request.Document.Recipients.Length == 0)
                        {
                            //Destinatario non presente
                            throw new RestException("REQUIRED_RECIPIENT");
                        }
                    }

                    if (string.IsNullOrEmpty(request.CodeRegister))
                    {
                        //Registro mancante
                        throw new RestException("REQUIRED_REGISTER");
                    }
                    else
                    {
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeRegister);
                        if (reg == null)
                        {
                            //Registro mancante
                            throw new RestException("REGISTER_NOT_FOUND");
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
                            throw new RestException("RF_NOT_FOUND");
                        }
                    }
                }

                //CASO PROTOCOLLO IN ARRIVO (ANCHE PREDISPOSTO)
                if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("A"))
                {
                    schedaDoc.tipoProto = "A";
                    schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloEntrata();

                    ((ProtocolloEntrata)schedaDoc.protocollo).mittente = mittente;

                    if (request.Document.MultipleSenders != null && request.Document.MultipleSenders.Length > 0)
                    {
                        ((ProtocolloEntrata)schedaDoc.protocollo).mittenti = new ArrayList();
                        foreach (Correspondent corrTemp in request.Document.MultipleSenders)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = null;
                            if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                            {
                                //corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                corrBySys = Utils.GetCorrespondentFromPisNewInsertOccasionale(corrTemp, infoUtente);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(corrTemp.Id))
                                {
                                    corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                    if (corrBySys == null)
                                    {
                                        //Mittente non trovato
                                        throw new RestException("SENDER_NOT_FOUND");
                                    }
                                }
                            }
                            ((ProtocolloEntrata)schedaDoc.protocollo).mittenti.Add(corrBySys);
                        }
                    }

                    //Data protocollo mittente
                    if (!string.IsNullOrEmpty(request.Document.DataProtocolSender))
                    {
                        ((ProtocolloEntrata)schedaDoc.protocollo).dataProtocolloMittente = request.Document.DataProtocolSender;
                    }

                    //Protocollo mittente
                    if (!string.IsNullOrEmpty(request.Document.ProtocolSender))
                    {
                        ((ProtocolloEntrata)schedaDoc.protocollo).descrizioneProtocolloMittente = request.Document.ProtocolSender;
                    }

                    //Data arrivo TODO
                    if (!string.IsNullOrEmpty(request.Document.ArrivalDate))
                    {
                        // Variabile Booleana per il controllo di validità della data passata in input;
                        bool valid = false;
                        // Data ritornata in output dal TryParseExact per la data arrivo passata in input
                        DateTime dateVal;

                        // Pattern di validità per una data valida
                        string pattern = "dd/MM/yyyy HH:mm:ss";
                        string pattern2 = "dd/MM/yyyy HH:mm";

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
                                if (!DateTime.TryParseExact(request.Document.ArrivalDate, pattern2, null, System.Globalization.DateTimeStyles.None, out dateVal))
                                {
                                    throw new Exception("Formato Data non corretto");
                                }
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
                                    ((Documento)schedaDoc.documenti[0]).dataArrivo = request.Document.ArrivalDate;
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
                                    schedaDocResult.descMezzoSpedizione = request.Document.MeansOfSending;
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

                    ((ProtocolloUscita)schedaDoc.protocollo).mittente = mittente;

                    if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                    {
                        ((ProtocolloUscita)schedaDoc.protocollo).destinatari = new ArrayList();
                        foreach (Correspondent corrTemp in request.Document.Recipients)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = null;

                            // Verifica se occasionale
                            if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                            {
                                //corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                corrBySys = Utils.GetCorrespondentFromPisNewInsertOccasionale(corrTemp, infoUtente);
                            }
                            else
                            {
                                // Se non è occasionale, lo risolve sulla rubrica
                                corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new RestException("RECIPIENT_NOT_FOUND");
                                }

                            }
                            if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "F" && corrBySys.tipoIE == null)
                            {
                                ArrayList corrByRF = BusinessLogic.Rubrica.RF.getCorrispondentiByCodRF(corrBySys.codiceRubrica);
                                foreach (DocsPaVO.utente.Corrispondente c2 in corrByRF)
                                {
                                    ((ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(c2);
                                }
                            }
                            else if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                            {
                                ArrayList corrByLista = BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiByCodLista(corrBySys.codiceRubrica, infoUtente.idAmministrazione, infoUtente);
                                foreach (DocsPaVO.utente.Corrispondente c3 in corrByLista)
                                {
                                    ((ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(c3);
                                }
                            }
                            else
                            {
                                ((ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(corrBySys);
                            }
                        }
                    }

                    if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                    {
                        ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                        foreach (Correspondent corrTemp in request.Document.RecipientsCC)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = null;

                            // Verifica se occasionale
                            if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                            {
                                //corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                corrBySys = Utils.GetCorrespondentFromPisNewInsertOccasionale(corrTemp, infoUtente);
                            }
                            else
                            {
                                corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new RestException("RECIPIENT_NOT_FOUND");
                                }
                            }
                            if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "F" && corrBySys.tipoIE == null)
                            {
                                ArrayList corrByRF = BusinessLogic.Rubrica.RF.getCorrispondentiByCodRF(corrBySys.codiceRubrica);
                                foreach (DocsPaVO.utente.Corrispondente c2 in corrByRF)
                                {
                                    ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(c2);
                                }
                            }
                            else if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                            {
                                ArrayList corrByLista = BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiByCodLista(corrBySys.codiceRubrica, infoUtente.idAmministrazione, infoUtente);
                                foreach (DocsPaVO.utente.Corrispondente c3 in corrByLista)
                                {
                                    ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(c3);
                                }
                            }
                            else
                            {
                                ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
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

                    ((ProtocolloInterno)schedaDoc.protocollo).mittente = mittente;

                    if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                    {
                        ((ProtocolloInterno)schedaDoc.protocollo).destinatari = new ArrayList();
                        foreach (Correspondent corrTemp in request.Document.Recipients)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                            if (corrBySys == null)
                            {
                                //Destinatario non trovato
                                throw new RestException("RECIPIENT_NOT_FOUND");
                            }
                            ((ProtocolloInterno)schedaDoc.protocollo).destinatari.Add(corrBySys);
                        }
                    }

                    if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                    {
                        ((ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                        foreach (Correspondent corrTemp in request.Document.RecipientsCC)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                            if (corrBySys == null)
                            {
                                //Destinatario non trovato
                                throw new RestException("RECIPIENT_NOT_FOUND");
                            }
                            ((ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
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

                //Se ho anche un file da aggiungere
                if (schedaDocResult != null && !string.IsNullOrEmpty(schedaDocResult.docNumber) && request.Document.MainDocument != null)
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
                        throw new RestException("FILE_CREATION_ERROR");
                    }


                }

                if (schedaDocResult != null)
                {
                    if (setStatoDiagrammaIniziale)
                    {
                        BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(schedaDocResult.systemId, statoIniziale.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);

                    }
                    response.Document = Utils.GetDetailsDocument(schedaDocResult, infoUtente, false);
                    #region gestione Linked Docs
                    if (request.Document.ParentDocument != null)
                    {
                        if (BusinessLogic.Amministrazione.SistemiEsterni.updLinkedDocRest(request.Document.ParentDocument.Id, schedaDocResult.systemId))
                            response.Document.ParentDocument = request.Document.ParentDocument;
                    }
                    if (request.Document.LinkedDocuments != null && request.Document.LinkedDocuments.Length > 0)
                    {
                        foreach (LinkedDocument ld in request.Document.LinkedDocuments)
                        {
                            if (ld.LinkType == "PARENT")
                            {
                                BusinessLogic.Amministrazione.SistemiEsterni.updLinkedDocRest(ld.Id, schedaDocResult.systemId);
                            }
                            else if (ld.LinkType == "LINKED")
                            {
                                BusinessLogic.Amministrazione.SistemiEsterni.updLinkedDocRest(schedaDocResult.systemId, ld.Id);
                            }
                        }
                    }
                    ArrayList linkedDocs = BusinessLogic.Amministrazione.SistemiEsterni.getLinkedDocRest(schedaDocResult.systemId);
                    if (linkedDocs != null && linkedDocs.Count > 0)
                    {
                        List<LinkedDocument> linkedDocs2 = new List<LinkedDocument>();
                        foreach (DocsPaVO.documento.BaseInfoDoc tempDoc in linkedDocs)
                        {
                            if (tempDoc.VersionLabel.ToUpper() == "PARENT")
                            {
                                response.Document.ParentDocument = Utils.getLinkedDoc(tempDoc);
                            }
                            else if (tempDoc.VersionLabel.ToUpper() == "LINKED")
                            {
                                linkedDocs2.Add(Utils.getLinkedDoc(tempDoc));
                            }
                        }
                        if (linkedDocs2 != null && linkedDocs2.Count > 0)
                        {
                            response.Document.LinkedDocuments = linkedDocs2.ToArray();
                        }
                    }
                    #endregion
                    if (setStatoDiagrammaIniziale)
                        response.Document.Template.StateDiagram.StateOfDiagram[0] = Utils.GetStateOfDiagram(statoIniziale, diagramma.SYSTEM_ID.ToString());

                    if (!string.IsNullOrEmpty(infoUtente.diSistema) && infoUtente.diSistema == "1")
                    {
                        bool pulito = Utils.CleanRightsExtSys(schedaDocResult.docNumber, infoUtente.idPeople, infoUtente.idGruppo);
                    }

                }
                #endregion
                #region AddDocInProject
                if (fascicolo != null)
                {
                    if (!string.IsNullOrEmpty(request.CodeProject))
                    {
                        ArrayList fascicoli = new ArrayList();
                        //Il codice è di un sottofascicolo
                        if (request.CodeProject.IndexOf("//") > -1)
                        {

                            string separatore = "//";
                            // MODIFICA per inserimento in sottocartelle
                            string[] separatoreAr = new string[] { separatore };

                            string[] pathCartelle = request.CodeProject.Split(separatoreAr, StringSplitOptions.RemoveEmptyEntries);

                            fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloDaCodice(infoUtente, pathCartelle[0], null, false, false);

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
                    }
                    string msg = "";
                    //Caso del sottofascicolo
                    if (fascicolo.folderSelezionato != null)
                    {
                        BusinessLogic.Fascicoli.FolderManager.addDocFolder(infoUtente, schedaDocResult.systemId, fascicolo.folderSelezionato.systemID, false, out msg);
                    }
                    else
                    {
                        BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, schedaDocResult.systemId, fascicolo.systemID, false, out msg);
                    }
                    logger.DebugFormat("Documento {0} inserito nel fascicolo {1}", schedaDocResult.systemId, fascicolo.codice);
                }
                #endregion

                response.Code = CreateDocumentResponseCode.OK;
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOADDDOCGRIGIA", schedaDocResult.docNumber, "Creazione del nuovo documento " + schedaDocResult.docNumber + ". Tipo documento: " + schedaDocResult.tipoProto + ". Predisposto: " + schedaDocResult.predisponiProtocollazione.ToString(), DocsPaVO.Logger.CodAzione.Esito.OK);
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCADDINFASC", schedaDocResult.docNumber, "Inserimento del documento " + schedaDocResult.docNumber + " nel fascicolo " + fascicolo.systemID, DocsPaVO.Logger.CodAzione.Esito.OK);

                logger.Info("end createDocAndAddInPrj");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione createDocAndAddInPrj: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new CreateDocumentResponse();
                response.Code = CreateDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione createDocAndAddInPrj: " + e);
                response = new CreateDocumentResponse();
                response.Code = CreateDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }


        public static async Task<UploadFileToDocumentResponse> uploadFileToDocument(string token, UploadFileToDocumentRequest request)
        {
            logger.Info("begin uploadFileToDocument");
            UploadFileToDocumentResponse response = new UploadFileToDocumentResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion

                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new RestException("REQUIRED_ID");
                }

                if (request.File == null)
                {
                    throw new RestException("REQUIRED_FILE");
                }

                if (string.IsNullOrEmpty(request.Description))
                {
                    throw new RestException("REQUIRED_DESCRIPTION");
                }

                //
                // Controllo HashFile
                string hash = string.Empty;
                byte[] contentFile = null;
                string hashObbligatorio = string.Empty;
                //Chiave per abilitare l'utilizzo dell'hash obbligatorio
                hashObbligatorio = System.Configuration.ConfigurationManager.AppSettings["HASH_OBBLIGATORIO"];

                if (!string.IsNullOrEmpty(hashObbligatorio) && hashObbligatorio.Equals("1"))
                {
                    // Hash obbligatorio
                    if (string.IsNullOrEmpty(request.HashFile))
                    {
                        throw new Exception("Hash del file obbligatorio.");
                    }
                    else
                    {
                        hash = request.HashFile;
                    }
                    int maxfilesize = Utils.maxFileSizePermitted();
                    if (request.File.Content.Length <= 0)
                    {
                        throw new Exception("Content File obbligatorio.");
                    }
                    else if (maxfilesize > 0 && request.File.Content.Length > maxfilesize)
                    {
                        throw new Exception("Dimensione del file troppo grande");
                    }
                    else
                    {
                        contentFile = request.File.Content;
                    }

                    //
                    // Controllo Hash del File
                    if (!string.IsNullOrEmpty(hash) && contentFile != null)
                    {
                        System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed();
                        byte[] impronta = sha.ComputeHash(contentFile);
                        string impr = BitConverter.ToString(impronta).Replace("-", "");

                        if (!impr.Equals(hash))
                        {
                            throw new Exception("Hash del file non corrispondente.");
                        }
                    }

                }

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                string erroreMessage = string.Empty;


                if (!string.IsNullOrEmpty(request.IdDocument))
                {
                    documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                    if (documento != null)
                    {
                        //Crea un nuovo allegato
                        if (request.CreateAttachment)
                        {
                            DocsPaVO.documento.FileRequest allegato = new DocsPaVO.documento.Allegato
                            {
                                docNumber = request.IdDocument,
                                descrizione = request.Description
                            };

                            // Acquisizione dell'allegato
                            allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, ((DocsPaVO.documento.Allegato)allegato));
                            //definisco l'allegato come esterno
                            
                                if (!string.IsNullOrEmpty(request.AttachmentType) && request.AttachmentType == "E")
                                {
                                    if (!BusinessLogic.Documenti.AllegatiManager.setFlagAllegatiEsterni(allegato.versionId, allegato.docNumber))
                                    {
                                        BusinessLogic.Documenti.AllegatiManager.rimuoviAllegato((DocsPaVO.documento.Allegato)allegato, infoUtente);
                                        throw new RestException("ERROR_ADD_ATTACHMENT");
                                    }
                                }
                            

                            // Acquisizione file allegato
                            // 5. Acquisizione del file al documento
                            DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                            {
                                name = request.File.Name,
                                fullName = request.File.Name,
                                content = request.File.Content,
                                contentType = request.File.MimeType,
                                length = request.File.Content.Length,
                                bypassFileContentValidation = true
                            };


                            if (!BusinessLogic.Documenti.FileManager.putFile(ref allegato, fileDocumento, infoUtente, out erroreMessage))
                            {
                                throw new RestException("FILE_CREATION_ERROR");
                            }
                        }
                        else
                        {
                            //Crea il file o se esistente una nuova versione
                            // 5. Acquisizione del file al documento
                            DocsPaVO.documento.FileRequest versioneCorrente = (DocsPaVO.documento.FileRequest)documento.documenti[0];

                            DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                            {
                                name = request.File.Name,
                                fullName = request.File.Name,
                                content = request.File.Content,
                                contentType = request.File.MimeType,
                                length = request.File.Content.Length,
                                bypassFileContentValidation = true
                            };

                            string errorMessage;

                            //Se esiste già una versione del file ne creo una nuova
                            if (versioneCorrente != null && !string.IsNullOrEmpty(versioneCorrente.fileName))
                            {
                                DocsPaVO.documento.FileRequest fileRequest = new DocsPaVO.documento.Allegato
                                {
                                    docNumber = request.IdDocument,
                                    descrizione = request.Description
                                };

                                versioneCorrente = BusinessLogic.Documenti.VersioniManager.addVersion(fileRequest, infoUtente, false);
                            }

                            if (!BusinessLogic.Documenti.FileManager.putFile(ref versioneCorrente, fileDocumento, infoUtente, out errorMessage))
                            {
                                throw new RestException("FILE_CREATION_ERROR");
                            }
                        }
                    }
                    else
                    {
                        throw new RestException("DOCUMENT_NOT_FOUND");
                    }
                }
                else
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }


                if (request.CreateAttachment)
                    response.ResultMessage = "Creato allegato per il documento " + request.IdDocument;
                else
                    response.ResultMessage = "Aggiunta nuova versione per il documento " + request.IdDocument;

                if (request.CreateAttachment)
                {
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", request.IdDocument, "Aggiunto un nuovo allegato al documento " + request.IdDocument, DocsPaVO.Logger.CodAzione.Esito.OK);
                }
                else
                {
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIVERSIONE", request.IdDocument, string.Format("{0}{1}{2}", "Aggiunta al N.ro Doc.: ", request.IdDocument, " una nuova versione tramite PIS "), DocsPaVO.Logger.CodAzione.Esito.OK);
                }

                logger.Info("end uploadFileToDocument");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new UploadFileToDocumentResponse();
                response.Code = UploadFileToDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione uploadFileToDocument: " + e);
                response = new UploadFileToDocumentResponse();
                response.Code = UploadFileToDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;
                
            }
            return response;
        }

        public static async Task<GetFileDocumentByIdResponse> getFileDocumentById(string token, string idDocument, string idVersion)
        {
            logger.Info("begin GetFileDocumentById");
            GetFileDocumentByIdResponse response = new GetFileDocumentByIdResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion


                if (string.IsNullOrEmpty(idDocument))
                {
                    throw new RestException("REQUIRED_ID");
                }

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();
                bool FileConFirma = false;
                if (!string.IsNullOrEmpty(idDocument))
                    {
                        if (!string.IsNullOrEmpty(idVersion) && idVersion.Contains("CERCA.TRE"))
                            documento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, idDocument);
                        else
                            documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idDocument, idDocument);
                        if (documento != null && documento.documenti != null && documento.documenti.Count > 0)
                        {
                            int numVersione = 0;
                            if (!string.IsNullOrEmpty(idVersion))
                            {
                                if (idVersion.ToUpper() == "SIGNED")
                                {
                                    FileConFirma = true;
                                }
                                else if (idVersion.Contains("CERCA.TRE"))
                                {
                                    FileConFirma = true;
                                    if (idVersion.Contains("CERCA.TRE_"))
                                    {
                                        bool result = Int32.TryParse(idVersion.Replace("CERCA.TRE_", ""), out numVersione);
                                        if (!result)
                                        {
                                            throw new RestException("REQUIRED_INTEGER");
                                        }
                                        else
                                        {
                                            if (documento.documenti.Count < numVersione || numVersione <= 0)
                                            {
                                                throw new RestException("FILE_VERSION_NOT_FOUND");
                                            }
                                            else
                                            {
                                                numVersione = documento.documenti.Count - numVersione;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    bool result = Int32.TryParse(idVersion, out numVersione);
                                    if (!result)
                                    {
                                        throw new RestException("REQUIRED_INTEGER");
                                    }
                                    else
                                    {
                                        if (documento.documenti.Count < numVersione || numVersione <= 0)
                                        {
                                            throw new RestException("FILE_VERSION_NOT_FOUND");
                                        }
                                        else
                                        {
                                            numVersione = documento.documenti.Count - numVersione;
                                        }
                                    }
                                }
                            }
                            DocsPaVO.documento.FileRequest versione = (DocsPaVO.documento.FileRequest)documento.documenti[numVersione];
                            response.File = Utils.GetFile(versione, true, infoUtente, false, false, string.Empty, null, FileConFirma);
                        }
                        else
                        {
                            throw new RestException("DOCUMENT_NOT_FOUND");
                        }
                    }
                            
                response.Code = GetFileDocumentByIdResponseCode.OK;
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETFILE", idDocument, "Visualizzato il documento " + idDocument, DocsPaVO.Logger.CodAzione.Esito.OK);

                logger.Info("end GetFileDocumentById");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione GetFileDocumentById: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetFileDocumentByIdResponse();
                response.Code = GetFileDocumentByIdResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione GetFileDocumentById: " + e);
                response = new GetFileDocumentByIdResponse();
                response.Code = GetFileDocumentByIdResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetFileDocumentByIdResponse> getFileWithSignatureOrStamp(string token, string idDocument, string signature, string signOrStamp)
        {
            logger.Info("begin getFileWithSignatureOrStamp");
            GetFileDocumentByIdResponse response = new GetFileDocumentByIdResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion

                if (string.IsNullOrEmpty(idDocument) && string.IsNullOrEmpty(signature))
                {
                    throw new RestException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(idDocument) && !string.IsNullOrEmpty(signature))
                {
                    throw new RestException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }
                               
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/XML/labelPdf.xml");

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                try
                {
                    if (!string.IsNullOrEmpty(idDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idDocument, idDocument);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(signature))
                        {
                            documento = BusinessLogic.Documenti.DocManager.GetDettaglioBySignature(signature, infoUtente);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                try
                {
                    bool withSignature = true, withStamp = false;
                    if(!string.IsNullOrWhiteSpace(signOrStamp)&& signOrStamp.ToUpper() == "STAMP")
                    {
                        withSignature = false;
                        withStamp = true;
                    }
                    if (documento != null && documento.documenti != null && documento.documenti.Count > 0)
                    {
                        int numVersione = 0;
                        DocsPaVO.documento.FileRequest versione = (DocsPaVO.documento.FileRequest)documento.documenti[numVersione];
                        response.File = Utils.GetFile(versione, true, infoUtente, withSignature, withStamp, path, documento, false);
                        response.Code = GetFileDocumentByIdResponseCode.OK;
                    }
                    else
                    {
                        throw new RestException("DOCUMENT_NOT_FOUND");
                    }


                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETFILE", idDocument, "Visualizzato il documento " + idDocument, DocsPaVO.Logger.CodAzione.Esito.OK);

                logger.Info("end getFileWithSignatureOrStamp");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getFileWithSignatureOrStamp: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetFileDocumentByIdResponse();
                response.Code = GetFileDocumentByIdResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getFileWithSignatureOrStamp: " + e);
                response = new GetFileDocumentByIdResponse();
                response.Code = GetFileDocumentByIdResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetDocumentResponse> editDocument(string token, CreateDocumentRequest request)
        {
            logger.Info("begin editDocument");
            GetDocumentResponse response = new GetDocumentResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                if (request.Document == null || string.IsNullOrEmpty(request.Document.Id))
                {
                    throw new RestException("REQUIRED_DOCUMENT");
                }

                //Reperimento ruolo utente
                
                DocsPaVO.utente.Corrispondente mittente = new DocsPaVO.utente.Corrispondente();

                try
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                    if (ruolo == null)
                    {
                        //Ruolo non trovato
                        throw new RestException("ROLE_NO_EXIST");
                    }
                }
                catch
                {
                    //Ruolo non trovato
                    throw new RestException("ROLE_NO_EXIST");
                }


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
                        Correspondent c = request.Document.MultipleSenders[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }

                if (request.Document.Recipients != null)
                {
                    for (int i = 0; i < request.Document.Recipients.Length; i++)
                    {
                        Correspondent c = request.Document.Recipients[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }

                if (request.Document.RecipientsCC != null)
                {
                    for (int i = 0; i < request.Document.RecipientsCC.Length; i++)
                    {
                        Correspondent c = request.Document.RecipientsCC[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }
                //


                //Documento salvato
                DocsPaVO.documento.SchedaDocumento documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.Document.Id, request.Document.DocNumber);
                bool protocollaPredisposto = false;
                // Modifica per protocollare un predisposto
                if (!request.Document.Predisposed &&
                    (request.Document.DocumentType == "A" ||
                    request.Document.DocumentType == "P" ||
                    request.Document.DocumentType == "I") &&
                    documento.protocollo != null &&
                    !string.IsNullOrEmpty(documento.protocollo.daProtocollare) &&
                    documento.protocollo.daProtocollare == "1")
                {
                    protocollaPredisposto = true;
                }

                ///Imposto l'oggetto
                if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.Object))
                {
                    documento.oggetto.descrizione = request.Document.Object;
                    documento.oggetto.daAggiornare = true;
                }

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
                        // Modifica per gestire tipologie modificate in seguito al salvataggio di un documento.
                        if (documento.template != null)
                        {
                            template = documento.template;
                            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg1 in documento.template.ELENCO_OGGETTI)
                            {
                                DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue oldObjText = new DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue();
                                oldObjText.IDTemplate = documento.template.SYSTEM_ID.ToString();
                                oldObjText.ID_Doc_Fasc = documento.systemId;
                                oldObjText.ID_Oggetto = ogg1.SYSTEM_ID.ToString();
                                oldObjText.Valore = ogg1.VALORE_DATABASE;
                                oldObjText.Tipo_Ogg_Custom = ogg1.TIPO.DESCRIZIONE_TIPO;
                                oldObjText.ID_People = infoUtente.idPeople;
                                oldObjText.ID_Ruolo_In_UO = infoUtente.idCorrGlobali;
                                template.OLD_OGG_CUSTOM.Add(oldObjText);
                            }
                        }
                        //Modifica Lembo 10-01-2013: Controllo se l'utente è abilitato all'utilizzo del template.
                        //Solo ruoli con diritti di scrittura ammessi.
                        string controlFilter = string.Format("{0} and id_ruolo={1} and (diritti='2')", template.SYSTEM_ID.ToString(), infoUtente.idGruppo);
                        ArrayList controlloVisibilita = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getRuoliTipoDoc(controlFilter);
                        if (controlloVisibilita == null || controlloVisibilita.Count == 0)
                        {
                            throw new RestException("TEMPLATE_NOT_ROLE_EDITABLE");
                        }
                        //Modifica Lembo 11-01-2013: metodo che controlla la visibilità anche per l'edit.
                        //documento.template = Utils.GetTemplateFromPis(request.Document.Template, template, false);

                        // Modifica Elaborazione XML da PIS req.2. Non si può associare il template nella editDocument
                        documento.template = Utils.GetTemplateFromPisVisibility(request.Document.Template, template, false, infoUtente.idGruppo, "D", "", infoUtente, null, null, null, true);
                        documento.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                        documento.tipologiaAtto.systemId = template.SYSTEM_ID.ToString();
                        documento.tipologiaAtto.descrizione = template.DESCRIZIONE.ToString();
                        documento.daAggiornareTipoAtto = true;

                    }
                    else
                    {
                        //Template non trovato
                        throw new RestException("TEMPLATE_NOT_FOUND");
                    }
                }
                ///

                ///Se privato
                if (request != null && request.Document != null && request.Document.PrivateDocument)
                {
                    documento.privato = "1";
                }
                ///

                DocsPaVO.documento.SchedaDocumento schedaDocResult = null;
                DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione = new ResultProtocollazione();
                bool daAggiornareUffRef = false;

                ///CASO DOCUMENTO GRIGIO (NON PROTOCOLLATO)
                if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("G"))
                {

                }
                ///

                //Controlli solo per protocolli
                if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && !request.Document.DocumentType.ToUpper().Equals("G"))
                {
                    if (request.Document.Sender == null && !string.IsNullOrEmpty(request.Document.Sender.CorrespondentType))
                    {
                        //Mittente non presente
                        throw new RestException("REQUIRED_SENDER");
                    }
                    else
                    {
                        if (request.Document.Sender.CorrespondentType.Equals("O"))
                        {
                            mittente = Utils.GetCorrespondentFromPis(request.Document.Sender, infoUtente);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(request.Document.Sender.Id))
                            {
                                mittente = Utils.RisolviCorrispondente(request.Document.Sender.Id, infoUtente);
                                if (mittente == null)
                                {
                                    //Mittente non trovato
                                    throw new RestException("SENDER_NOT_FOUND");
                                }
                            }
                        }
                    }

                    if (request.Document.DocumentType.ToUpper().Equals("I") || request.Document.DocumentType.ToUpper().Equals("P"))
                    {
                        if (request.Document.Recipients == null || request.Document.Recipients.Length == 0)
                        {
                            //Destinatario non presente
                            throw new RestException("REQUIRED_RECIPIENT");
                        }
                    }

                }

                ///CASO PROTOCOLLO IN ARRIVO (ANCHE PREDISPOSTO)
                if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("A"))
                {
                    ((ProtocolloEntrata)documento.protocollo).mittente = mittente;
                    ((ProtocolloEntrata)documento.protocollo).daAggiornareMittente = true;

                    if (request.Document.MultipleSenders != null && request.Document.MultipleSenders.Length > 0)
                    {
                        ((ProtocolloEntrata)documento.protocollo).mittenti = new ArrayList();
                        foreach (Correspondent corrTemp in request.Document.MultipleSenders)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = null;
                            if (corrTemp.CorrespondentType.Equals("O"))
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
                                        throw new RestException("SENDER_NOT_FOUND");
                                    }
                                }
                            }
                            ((ProtocolloEntrata)documento.protocollo).mittenti.Add(corrBySys);
                            ((ProtocolloEntrata)documento.protocollo).daAggiornareMittentiMultipli = true;
                        }
                    }

                    //Data protocollo mittente
                    if (!string.IsNullOrEmpty(request.Document.DataProtocolSender))
                    {
                        ((ProtocolloEntrata)documento.protocollo).dataProtocolloMittente = request.Document.DataProtocolSender;
                    }

                    //Protocollo mittente
                    if (!string.IsNullOrEmpty(request.Document.ProtocolSender))
                    {
                        ((ProtocolloEntrata)documento.protocollo).descrizioneProtocolloMittente = request.Document.ProtocolSender;
                    }

                    //Data arrivo TODO
                    if (!string.IsNullOrEmpty(request.Document.ArrivalDate))
                    {
                        // Variabile Booleana per il controllo di validità della data passata in input;
                        bool valid = false;
                        // Data ritornata in output dal TryParseExact per la data arrivo passata in input
                        DateTime dateVal;

                        // Pattern di validità per una data valida
                        string pattern = "dd/MM/yyyy HH:mm:ss";
                        string pattern2 = "dd/MM/yyyy HH:mm";

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
                                if (!DateTime.TryParseExact(request.Document.ArrivalDate, pattern2, null, System.Globalization.DateTimeStyles.None, out dateVal))
                                {
                                    throw new Exception("Formato Data non corretto");
                                }
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

                            if (!DateTime.TryParseExact(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), pattern, null, System.Globalization.DateTimeStyles.None, out dateVal2))
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
                                    ((Documento)documento.documenti[0]).dataArrivo = request.Document.ArrivalDate;
                            }
                        }
                    }
                }
                ///

                ///CASO PROTOCOLLO IN PARTENZA (ANCHE PREDISPOSTO)
                if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("P"))
                {

                    ((ProtocolloUscita)documento.protocollo).mittente = mittente;
                    ((ProtocolloUscita)documento.protocollo).daAggiornareMittente = true;

                    if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                    {
                        // Controllo duplicazione destinatari occasionali
                        var destOrig = ((ProtocolloUscita)documento.protocollo).destinatari;
                        ((ProtocolloUscita)documento.protocollo).destinatari = new ArrayList();
                        foreach (Correspondent corrTemp in request.Document.Recipients)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = null;

                            // Verifica se occasionale
                            if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                            {
                                corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                                if (destOrig != null && destOrig.Count > 0)
                                {
                                    bool giapresente = false;
                                    foreach (DocsPaVO.utente.Corrispondente d1 in destOrig)
                                    {
                                        if (d1.descrizione.ToUpper() == corrBySys.descrizione.ToUpper()) giapresente = true;
                                    }
                                    if (giapresente) { corrBySys = null; continue; }
                                }
                            }
                            else
                            {
                                // Se non è occasionale, lo risolve sulla rubrica
                                corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new RestException("RECIPIENT_NOT_FOUND");
                                }
                            }
                            if (corrBySys != null)
                            {
                                ((ProtocolloUscita)documento.protocollo).destinatari.Add(corrBySys);
                                ((ProtocolloUscita)documento.protocollo).daAggiornareDestinatari = true;
                            }
                            else
                            {
                                ((ProtocolloUscita)documento.protocollo).daAggiornareDestinatari = false;
                            }
                        }
                        if (((ProtocolloUscita)documento.protocollo).destinatari == null || ((ProtocolloUscita)documento.protocollo).destinatari.Count < 1)
                            ((ProtocolloUscita)documento.protocollo).destinatari = destOrig;
                    }

                    if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                    {
                        ((ProtocolloUscita)documento.protocollo).destinatariConoscenza = new ArrayList();
                        foreach (Correspondent corrTemp in request.Document.RecipientsCC)
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
                                    throw new RestException("RECIPIENT_NOT_FOUND");
                                }
                            }
                            ((ProtocolloUscita)documento.protocollo).destinatariConoscenza.Add(corrBySys);
                            ((ProtocolloUscita)documento.protocollo).daAggiornareDestinatariConoscenza = true;
                        }
                    }

                    if (request.Document.Predisposed)
                    {
                        documento.predisponiProtocollazione = true;
                    }
                }
                ///

                ///CASO PROTOCOLLO INTERNO (ANCHE PREDISPOSTO)
                if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("I"))
                {

                    ((ProtocolloInterno)documento.protocollo).mittente = mittente;
                    ((ProtocolloInterno)documento.protocollo).daAggiornareMittente = true;

                    if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                    {
                        ((ProtocolloInterno)documento.protocollo).destinatari = new ArrayList();
                        foreach (Correspondent corrTemp in request.Document.Recipients)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                            if (corrBySys == null)
                            {
                                //Destinatario non trovato
                                throw new RestException("RECIPIENT_NOT_FOUND");
                            }
                            ((ProtocolloInterno)documento.protocollo).destinatari.Add(corrBySys);
                            ((ProtocolloInterno)documento.protocollo).daAggiornareDestinatari = true;
                        }
                    }

                    if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                    {
                        ((ProtocolloInterno)documento.protocollo).destinatariConoscenza = new ArrayList();
                        foreach (Correspondent corrTemp in request.Document.RecipientsCC)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                            if (corrBySys == null)
                            {
                                //Destinatario non trovato
                                throw new RestException("RECIPIENT_NOT_FOUND");
                            }
                            ((ProtocolloInterno)documento.protocollo).destinatariConoscenza.Add(corrBySys);
                            ((ProtocolloInterno)documento.protocollo).daAggiornareDestinatariConoscenza = true;
                        }
                    }

                    if (request.Document.Predisposed)
                    {
                        documento.predisponiProtocollazione = true;
                    }
                }

                if (!protocollaPredisposto)
                    schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, documento, false, out daAggiornareUffRef, ruolo);
                else
                {
                    // controllo se presente un RF
                    if (!string.IsNullOrEmpty(request.CodeRF))
                    {
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeRF);
                        // solo se è un rf e non è chiuso aggiungo le informazioni di protocollazione RF
                        if (reg != null && reg.chaRF == "1" && reg.stato != "C")                            
                        {
                            documento.id_rf_prot = reg.systemId;
                            documento.id_rf_invio_ricevuta = reg.systemId;
                            documento.cod_rf_prot = reg.codRegistro;
                        }
                        else
                        {
                            //RF non trovato
                            throw new RestException("RF_NOT_FOUND");
                        }
                    }

                    DocsPaVO.documento.ResultProtocollazione resultProto = new ResultProtocollazione();
                    schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(documento, ruolo, infoUtente, out resultProto);
                }
                if (schedaDocResult != null)
                {
                    response.Document = Utils.GetDetailsDocument(schedaDocResult, infoUtente, false);
                    // allego i documenti linkati anche in caso di edit
                    #region gestione Linked Docs
                    if (request.Document.ParentDocument != null)
                    {
                        if (BusinessLogic.Amministrazione.SistemiEsterni.updLinkedDocRest(request.Document.ParentDocument.Id, schedaDocResult.systemId))
                            response.Document.ParentDocument = request.Document.ParentDocument;
                    }
                    if (request.Document.LinkedDocuments != null && request.Document.LinkedDocuments.Length > 0)
                    {
                        foreach (LinkedDocument ld in request.Document.LinkedDocuments)
                        {
                            if (ld.LinkType == "PARENT")
                            {
                                BusinessLogic.Amministrazione.SistemiEsterni.updLinkedDocRest(ld.Id, schedaDocResult.systemId);
                            }
                            else if (ld.LinkType == "LINKED")
                            {
                                BusinessLogic.Amministrazione.SistemiEsterni.updLinkedDocRest(schedaDocResult.systemId, ld.Id);
                            }
                        }
                    }
                    #endregion
                    ArrayList linkedDocs = BusinessLogic.Amministrazione.SistemiEsterni.getLinkedDocRest(documento.systemId);
                    if (linkedDocs != null && linkedDocs.Count > 0)
                    {
                        List<LinkedDocument> linkedDocs2 = new List<LinkedDocument>();
                        foreach (DocsPaVO.documento.BaseInfoDoc tempDoc in linkedDocs)
                        {
                            if (tempDoc.VersionLabel.ToUpper() == "PARENT")
                            {
                                response.Document.ParentDocument = Utils.getLinkedDoc(tempDoc);
                            }
                            else if (tempDoc.VersionLabel.ToUpper() == "LINKED")
                            {
                                linkedDocs2.Add(Utils.getLinkedDoc(tempDoc));
                            }
                        }
                        if (linkedDocs2 != null && linkedDocs2.Count > 0)
                        {
                            response.Document.LinkedDocuments = linkedDocs2.ToArray();
                        }
                    }
                }

                response.Code = GetDocumentResponseCode.OK;
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSAVEDOCUMENTO", request.Document.Id, "Modificati i dati del documento " + request.Document.Id, DocsPaVO.Logger.CodAzione.Esito.OK);

                logger.Info("end editDocument");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione editDocument: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetDocumentResponse();
                response.Code = GetDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione editDocument: " + e);
                response = new GetDocumentResponse();
                response.Code = GetDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetDocumentResponse> protocolPredisposed(string token, ProtocolPredisposedRequest request)
        {
            logger.Info("begin protocolPredisposed");
            GetDocumentResponse response = new GetDocumentResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new RestException("REQUIRED_DOCUMENT");
                }

                //Reperimento ruolo utente
                
                DocsPaVO.utente.Corrispondente mittente = new DocsPaVO.utente.Corrispondente();

                try
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                    if (ruolo == null)
                    {
                        //Ruolo non trovato
                        throw new RestException("ROLE_NO_EXIST");
                    }
                }
                catch
                {
                    //Ruolo non trovato
                    throw new RestException("ROLE_NO_EXIST");
                }
                DocsPaVO.documento.SchedaDocumento schedaDocResult = null;
                DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione = new ResultProtocollazione();
                bool daAggiornareUffRef = false;
                DocsPaVO.documento.SchedaDocumento documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                if (documento == null) throw new RestException("DOCUMENT_NOT_FOUND");
                bool protocollaPredisposto = false;
                // Modifica per protocollare un predisposto
                if (documento.protocollo != null &&
                    !string.IsNullOrEmpty(documento.protocollo.daProtocollare) &&
                    documento.protocollo.daProtocollare == "1")
                {
                    protocollaPredisposto = true;
                }
                else
                {
                    throw new Exception("Documento non valido per la protocollazione");
                }
                if (!string.IsNullOrEmpty(request.CodeRF))
                {
                    DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeRF);
                    // solo se è un rf e non è chiuso aggiungo le informazioni di protocollazione RF
                    if (reg != null && reg.chaRF == "1" && reg.stato != "C")
                    {
                        documento.id_rf_prot = reg.systemId;
                        documento.id_rf_invio_ricevuta = reg.systemId;
                        documento.cod_rf_prot = reg.codRegistro;
                    }
                    else
                    {
                        //RF non trovato
                        throw new RestException("RF_NOT_FOUND");
                    }
                }

                DocsPaVO.documento.ResultProtocollazione resultProto = new ResultProtocollazione();
                schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(documento, ruolo, infoUtente, out resultProto);
                if (schedaDocResult != null)
                {
                    response.Document = Utils.GetDetailsDocument(schedaDocResult, infoUtente, false);
                    // allego i documenti linkati anche in caso di edit
                    ArrayList linkedDocs = BusinessLogic.Amministrazione.SistemiEsterni.getLinkedDocRest(documento.systemId);
                    if (linkedDocs != null && linkedDocs.Count > 0)
                    {
                        List<LinkedDocument> linkedDocs2 = new List<LinkedDocument>();
                        foreach (DocsPaVO.documento.BaseInfoDoc tempDoc in linkedDocs)
                        {
                            if (tempDoc.VersionLabel.ToUpper() == "PARENT")
                            {
                                response.Document.ParentDocument = Utils.getLinkedDoc(tempDoc);
                            }
                            else if (tempDoc.VersionLabel.ToUpper() == "LINKED")
                            {
                                linkedDocs2.Add(Utils.getLinkedDoc(tempDoc));
                            }
                        }
                        if (linkedDocs2 != null && linkedDocs2.Count > 0)
                        {
                            response.Document.LinkedDocuments = linkedDocs2.ToArray();
                        }
                    }
                }

                response.Code = GetDocumentResponseCode.OK;
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSAVEDOCUMENTO", documento.systemId, "Modificati i dati del documento " + documento.systemId, DocsPaVO.Logger.CodAzione.Esito.OK);

                logger.Info("end editDocument");

            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione protocolPredisposed: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetDocumentResponse();
                response.Code = GetDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione protocolPredisposed: " + e);
                response = new GetDocumentResponse();
                response.Code = GetDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<SearchDocumentsResponse> searchDocuments(string token, SearchRequest request)
        {
            logger.Info("begin searchDocuments");
            SearchDocumentsResponse response = new SearchDocumentsResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                if (request.Filters == null || request.Filters.Length == 0)
                {
                    throw new RestException("REQUIRED_FILTER");
                }

                //Chiamata al metodo CheckFilterType(request.Filters)
                Utils.CheckFilterTypes(request.Filters);

                //Converti i filtri di ricerca
                DocsPaVO.filtri.FiltroRicerca[][] qV = Utils.GetFiltersDocumentsFromPis(request.Filters, infoUtente);

                Document[] responseDocuments = null;

                int nRec = 0;
                int numTotPage = 0;
                // questo valore da problemi con la max row searchable. Lo imposto a true comunque.
                //bool allDocuments = false;
                bool allDocuments = true;
                int pageSize = 20;
                int numPage = 1;

                if ((request.ElementsInPage == null && request.PageNumber == null) || (request.ElementsInPage == 0 && request.PageNumber == 0))
                {
                    allDocuments = true;
                }
                else
                {
                    
                    pageSize = request.ElementsInPage>0?request.ElementsInPage:20;
                    numPage = request.PageNumber>0 ? request.PageNumber : 1;
                        allDocuments = false;                    
                }

                List<DocsPaVO.ricerche.SearchResultInfo> idProfileList = null;
                List<DocsPaVO.ricerche.SearchResultInfo> toSet = new List<DocsPaVO.ricerche.SearchResultInfo>();
                ArrayList objListaInfoDocumenti = null;
                numTotPage = 0;
                nRec = 0;
                #region Estrazione template da ricerca con filtro TEMPLATE_EXTRACTION
                // Tentativo di estrazione template per ricerca PIS
                bool estrazioneTemplate = false;
                Filter filtroTemplate = (from filtro in request.Filters where (filtro != null && !string.IsNullOrEmpty(filtro.Name) && filtro.Name.ToUpper() == "TEMPLATE") select filtro).FirstOrDefault();
                List<DocsPaVO.Grid.Field> visibilityFields = new List<DocsPaVO.Grid.Field>();
                if (filtroTemplate != null)
                {
                    Filter filtroEstrazione = (from filtro in request.Filters where (filtro != null && !string.IsNullOrEmpty(filtro.Name) && filtro.Name.ToUpper() == "TEMPLATE_EXTRACTION") select filtro).FirstOrDefault();
                    if (filtroEstrazione != null && filtroEstrazione.Value.ToUpper() == "TRUE")
                    {
                        estrazioneTemplate = true;
                        DocsPaVO.Grid.Field x1 = new DocsPaVO.Grid.Field();
                        DocsPaVO.ProfilazioneDinamica.Templates template = null;
                        if (filtroTemplate != null)
                        {
                            int idTplTemp = 0;
                            if (Int32.TryParse(filtroTemplate.Value, out idTplTemp))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(idTplTemp.ToString());

                            }
                            else if (filtroTemplate.Template != null && !string.IsNullOrEmpty(filtroTemplate.Template.Id))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(filtroTemplate.Template.Id);
                            }
                            else
                            {
                                if (filtroTemplate.Template != null && !string.IsNullOrEmpty(filtroTemplate.Template.Name))
                                {
                                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(filtroTemplate.Template.Name, infoUtente.idAmministrazione);
                                }
                            }
                        }
                        if (template != null)
                        {
                            DocsPaVO.Grid.Field campoProf = null;
                            int i = 0;
                            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in template.ELENCO_OGGETTI)
                            {
                                campoProf = new DocsPaVO.Grid.Field();
                                campoProf.AssociatedTemplateId = template.SYSTEM_ID.ToString();
                                campoProf.AssociatedTemplateName = template.DESCRIZIONE;
                                campoProf.CanAssumeMultiValues = false;
                                campoProf.CustomObjectId = ogg.SYSTEM_ID;
                                campoProf.FieldId = "T" + ogg.SYSTEM_ID;
                                campoProf.IsTruncable = true;
                                campoProf.Label = ogg.DESCRIZIONE;
                                campoProf.OriginalLabel = ogg.DESCRIZIONE;
                                campoProf.MaxLength = 1000000;
                                campoProf.OracleDbColumnName = ogg.SYSTEM_ID.ToString();
                                campoProf.SqlServerDbColumnName = ogg.SYSTEM_ID.ToString();
                                campoProf.Position = i;
                                i++;
                                campoProf.Visible = true;
                                campoProf.Width = 100;
                                visibilityFields.Add(campoProf);

                            }
                        }
                        else
                        {
                            throw new RestException("TEMPLATE_NOT_FOUND");
                        }

                    }
                }
                // fine estrazione template da ricerca
                #endregion
                if (!estrazioneTemplate)
                    objListaInfoDocumenti = BusinessLogic.Documenti.InfoDocManager.getQueryPagingCustom(infoUtente, qV, numPage, pageSize, true, allDocuments, false, null, null, out numTotPage, out nRec, true, out toSet);
                else
                    objListaInfoDocumenti = BusinessLogic.Documenti.InfoDocManager.getQueryPagingCustom(infoUtente, qV, numPage, pageSize, true, allDocuments, true, visibilityFields.ToArray(), null, out numTotPage, out nRec, true, out toSet);

                idProfileList = toSet;

                if (objListaInfoDocumenti != null && objListaInfoDocumenti.Count > 0)
                {
                    responseDocuments = new Document[objListaInfoDocumenti.Count];
                    int y = 0;
                    DocsPaVO.amministrazione.InfoAmministrazione infoAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(authInfoArray[4]);
                    foreach (DocsPaVO.Grids.SearchObject obj in objListaInfoDocumenti)
                    {
                        responseDocuments[y] = Utils.GetDocumentFromSearchObject(obj, estrazioneTemplate, visibilityFields.ToArray(), infoAmm.Codice);
                        y++;
                    }

                    response.TotalDocumentsNumber = toSet.Count;
                }
                else
                {
                    //Documenti non trovati
                    //throw new PisException("DOCUMENTS_NOT_FOUND");

                    response.TotalDocumentsNumber = 0;

                    responseDocuments = new Document[0];
                }

                //if (request.GetTotalDocumentsNumber && toSet != null)
                //    response.TotalDocumentsNumber = toSet.Count;

                response.Documents = responseDocuments;


                logger.Info("end searchDocuments");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione searchDocuments: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new SearchDocumentsResponse();
                response.Code = SearchDocumentsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione searchDocuments: " + e);
                response = new SearchDocumentsResponse();
                response.Code = SearchDocumentsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetDocAccessRightsResponse> getDocAccessRights(string token, string idDocument)
        {
            logger.Info("begin getDocAccessRights");
            GetDocAccessRightsResponse response = new GetDocAccessRightsResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                DocsPaVO.documento.SchedaDocumento documento = null;
                List<ObjectAccessRight> diritti = new List<ObjectAccessRight>();
                try
                {
                    if (!string.IsNullOrEmpty(idDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idDocument, idDocument);
                    }
                    else
                    {
                        throw new RestException("REQUIRED_ID");
                    }
                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                if (documento != null)
                {
                    ArrayList dirittiOggetto = BusinessLogic.Utenti.DirittiManager.getListaDirittiSemplificataWithFilter(infoUtente, idDocument, false, null);
                    if (dirittiOggetto != null && dirittiOggetto.Count > 0)
                    {
                        ObjectAccessRight oar = null;
                        foreach (DocsPaVO.documento.DirittoOggetto dirOgg in dirittiOggetto)
                        {
                            oar = new ObjectAccessRight();
                            oar.IdObject = dirOgg.idObj;
                            oar.AccessDate = dirOgg.dtaInsSecurity;
                            oar.AccessRights = dirOgg.accessRights.ToString();
                            switch (dirOgg.tipoDiritto)
                            {
                                case TipoDiritto.TIPO_ACQUISITO:
                                    oar.AccessRightsType = "Acquired";
                                    break;
                                case TipoDiritto.TIPO_CONSERVAZIONE:
                                    oar.AccessRightsType = "Storage";
                                    break;
                                case TipoDiritto.TIPO_DELEGATO:
                                    oar.AccessRightsType = "Delegate";
                                    break;
                                case TipoDiritto.TIPO_PROPRIETARIO:
                                    oar.AccessRightsType = "Owner";
                                    break;
                                case TipoDiritto.TIPO_TRASMISSIONE:
                                    oar.AccessRightsType = "Transmission";
                                    break;
                                case TipoDiritto.TIPO_TRASMISSIONE_IN_FASCICOLO:
                                    oar.AccessRightsType = "Project Transmission";
                                    break;
                            }
                            oar.Note = dirOgg.noteSecurity;
                            oar.SubjectCode = dirOgg.soggetto.codiceRubrica;
                            oar.SubjectDescription = dirOgg.soggetto.descrizione;
                            oar.SubjectType = dirOgg.soggetto.tipoCorrispondente;
                            if (oar.SubjectType == "P")
                                oar.SubjectId = ((DocsPaVO.utente.Utente)dirOgg.soggetto).idPeople;
                            else if (oar.SubjectType == "R")
                                oar.SubjectId = ((DocsPaVO.utente.Ruolo)dirOgg.soggetto).idGruppo;

                            diritti.Add(oar);
                            
                        }
                        response.AccessRights = diritti;
                        response.Code = GetDocAccessRightsResponseCode.OK;
                    }
                    else
                    {
                        throw new Exception("Errore nel reperimendo dei diritti");
                    }
                }


                logger.Info("end getDocAccessRights");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getDocAccessRights: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetDocAccessRightsResponse();
                response.Code = GetDocAccessRightsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getDocAccessRights: " + e);
                response = new GetDocAccessRightsResponse();
                response.Code = GetDocAccessRightsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetDocumentEventsResponse> getDocumentEvents(string token, string idDocument, bool allEvents)
        {
            logger.Info("begin getDocumentEvents");
            GetDocumentEventsResponse response = new GetDocumentEventsResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion

                string[] eventiModifica = { "ADD_VERSION", "AGG_DOC_GRIGIO", "AGG_PROT", "ANN_PRED", "ANNULLA_PROTO", "ANNULL_PROTOCOLLO", 
                                              "CESTINA_DOC", "CONSOLIDADOCUMENTO", "CREATE_DOC_AND_ADD_IN_FASC", "DEL_DOC", "DOC_ADD_INFASC", 
                                              "DOC_ADD_INFOLDER", "DOC_CAMBIO_STATO", "DOC_CAMBIO_STATO_ADMIN", "DOC_DEL_FROM_FOLDER", 
                                              "DOC_NEW_ALLEGATO", "DOC_PROT", "DOC_RIMUOVI_ALLEGATO", "DOC_RIMUOVI_VERSIONE", "DOC_SIGNATURE", 
                                              "DOC_SIGNATURE_P", "DOCUMENTOCONVERSIONEPDF", "DOCUMENTO_EXEC_ANNULLA_REPERTORIO", 
                                              "DOCUMENTO_REPERTORIATO", "DOCUMENTOTIMESTAMP", "DOC_VERIFIED", "MODIFICADOCSTATOFINALE", 
                                              "MODIFIED_OBJECT_DOC", "MODIFIED_OBJECT_PROTO", "MOD_MITT_DEST", "MOD_MITT_INT", "PUT_FILE", 
                                              "RECORD_PREDISPOSED", "SCAMBIA_DOC","DOC_VERIFIED","DOC_STEP_OVER","DOC_SIGNATURE_P",
                                              "DOC_SIGNATURE","INTERROTTO_PROCESSO_DOCUMENTO_DAL_PROPONENTE","INTERROTTO_PROCESSO_DOCUMENTO_DAL_TITOLARE",
                                              "CONCLUSIONE_PROCESSO_LF_DOCUMENTO","TRONCAMENTO_PROCESSO","AVVIATO_PROCESSO_DI_FIRMA_DOCUMENTO",
                                              "AVVIATO_PROCESSO_DI_FIRMA_ALLEGATO","INTERROTTO_PROCESSO_DOCUMENTO_DA_ADMIN" };
                ArrayList evMod = new ArrayList(eventiModifica);

                if (idDocument != null)
                {
                    ArrayList eventi = BusinessLogic.Documenti.Oggettario.getListaLog(idDocument, "DOCUMENTO");
                    if (eventi != null && eventi.Count > 0)
                    {
                        List<LogEvent> logEventi = new List<LogEvent>();
                        LogEvent loge = null;
                        foreach (DocsPaVO.documento.LogDocumento evento in eventi)
                        {
                            // inserire controllo sul tipo degli eventi
                            if (!allEvents && !evMod.Contains(evento.codAzione.ToUpper())) continue;
                            loge = new LogEvent();
                            loge.ActionCode = evento.codAzione;
                            loge.ActionDate = evento.dataAzione;
                            loge.AdministrationId = evento.idAmm;
                            loge.Id = evento.systemId;
                            loge.ObjectDescription = evento.descrOggetto;
                            loge.OperationExecuted = evento.chaEsito;
                            loge.OperatorDescription = evento.descProduttore;
                            loge.OperatorGroupID = evento.idGruppoOperatore;
                            loge.OperatorPeopleID = evento.idPeopleOPeratore;
                            loge.OperatorUsername = evento.userIdOperatore;
                            logEventi.Add(loge);

                        }

                        response.Events = logEventi;
                        response.Code = GetDocumentEventsResponseCode.OK;
                    }
                }


                logger.Info("end getDocumentEvents");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getDocumentEvents: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetDocumentEventsResponse();
                response.Code = GetDocumentEventsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getDocumentEvents: " + e);
                response = new GetDocumentEventsResponse();
                response.Code = GetDocumentEventsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<SearchDocumentsResponse> getModifiedDocuments(string token, string dateFrom, string dateTo, bool allEvents, bool modifiedOnly, string security)
        {
            logger.Info("begin getModifiedDocuments");
            SearchDocumentsResponse response = new SearchDocumentsResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                bool secBool = true;
                if (!string.IsNullOrWhiteSpace(security) && security.ToUpper() == "ALL_DOCS_NO_SECURITY")
                {
                    secBool = false;
                }

                ArrayList infoDocs = BusinessLogic.Documenti.InfoDocManager.getDocModifiedByDates(dateFrom, dateTo, modifiedOnly, allEvents, secBool, infoUtente);
                Document[] responseDocuments = null;
                if (infoDocs != null && infoDocs.Count > 0)
                {
                    responseDocuments = new Document[infoDocs.Count];
                    Document docX = null;
                    int id = 0;
                    foreach (InfoDocumento infDoc1 in infoDocs)
                    {
                        docX = new Document();
                        docX.DocNumber = infDoc1.docNumber;
                        docX.Signature = infDoc1.segnatura;
                        docX.Object = infDoc1.oggetto;

                        responseDocuments[id] = docX;
                        id++;
                    }

                    response.Documents = responseDocuments;
                    response.TotalDocumentsNumber = infoDocs.Count;

                }
                else
                {
                    response.TotalDocumentsNumber = 0;
                }
                response.Code = SearchDocumentsResponseCode.OK;


                logger.Info("end getModifiedDocuments");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getModifiedDocuments: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new SearchDocumentsResponse();
                response.Code = SearchDocumentsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getModifiedDocuments: " + e);
                response = new SearchDocumentsResponse();
                response.Code = SearchDocumentsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<CreateDocumentResponse> importPreviousDocument(string token, CreateDocumentRequest request)
        {
            logger.Info("begin importPreviousDocument");
            CreateDocumentResponse response = new CreateDocumentResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                if (request != null && string.IsNullOrEmpty(request.Document.DocumentType))
                {
                    throw new RestException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && string.IsNullOrEmpty(request.Document.Object))
                {
                    throw new RestException("MISSING_OBJECT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (!request.Document.DocumentType.ToUpper().Equals("A") && !request.Document.DocumentType.ToUpper().Equals("P") && !request.Document.DocumentType.ToUpper().Equals("I") && !request.Document.DocumentType.ToUpper().Equals("G")))
                {
                    throw new RestException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (request.Document.DocumentType.ToUpper().Equals("A") || request.Document.DocumentType.ToUpper().Equals("P") || request.Document.DocumentType.ToUpper().Equals("I")) && string.IsNullOrEmpty(request.CodeRegister))
                {
                    throw new RestException("REQUIRED_REGISTER");
                }

                if(request != null && string.IsNullOrEmpty(request.Document.ProtocolNumber))
                {
                    throw new Exception("Richiesto il numero di protocollo");
                }
                if (request != null && string.IsNullOrEmpty(request.Document.ProtocolYear))
                {
                    throw new Exception("Richiesto l'anno di protocollo");
                }
                string formatoSegnatura = BusinessLogic.Amministrazione.SistemiEsterni.getFormatoSegnatura(infoUtente.idAmministrazione);
                if (formatoSegnatura.ToUpper().Contains("COD_RF_PROT") && request.Document.DocumentType != "G" &&  string.IsNullOrWhiteSpace(request.CodeRF))
                {
                    throw new Exception("Necessario il codice dell'RF");
                }
                request.Document.Predisposed = true;
                DocsPaVO.utente.Registro reg = null;
                if (request.Document.DocumentType != "G")
                {
                    reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeRegister);
                    if (reg == null)
                    {
                        //Registro mancante
                        throw new RestException("REGISTER_NOT_FOUND");
                    }
                }
                response = await createDocument(token, request, true);
                if (response.Code == CreateDocumentResponseCode.OK)
                {
                    // metodi per l'update in profile, inserendo il numero di protocollo e l'anno di protocollo.
                    // DOCNAME, NUM_PROTO, NUM_ANNO_PROTO,DTA_PROTO,VAR_SEGNATURA, CHA_DA_PROTO, VAR_CHIAVE_PROTO (434788_2018_86107), ID_REGISTRO?
                    if (request.Document.DocumentType != "G")
                    {
                        
                        string segnatura = formatoSegnatura.ToUpper();
                        logger.Debug("Segnatura prima: "+segnatura);
                        if (segnatura.Contains("COD_AMM")) 
                            segnatura = segnatura.Replace("COD_AMM", BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione).Codice);
                        if (segnatura.Contains("COD_REG"))
                            segnatura = segnatura.Replace("COD_REG", reg.codRegistro);
                        
                        if (segnatura.Contains("DATA_COMP"))
                            segnatura = segnatura.Replace("DATA_COMP", request.Document.ProtocolDate);
                        if (segnatura.Contains("IN_OUT"))
                            segnatura = segnatura.Replace("IN_OUT", request.Document.DocumentType);
                        if (segnatura.Contains("COD_RF_PROT"))
                            segnatura = segnatura.Replace("COD_RF_PROT", request.CodeRF.ToUpper());
                        if (segnatura.Contains("DATA_ANNO"))
                            segnatura = segnatura.Replace("DATA_ANNO", request.Document.ProtocolYear);
                        if (segnatura.Contains("NUM_PROTO"))
                            segnatura = segnatura.Replace("NUM_PROTO", request.Document.ProtocolNumber.PadLeft(7,'0'));
                        logger.Debug("Segnatura dopo: "+segnatura);

                        bool modProfile = BusinessLogic.Amministrazione.SistemiEsterni.importPregressiRestWS(response.Document.Id, request.Document.ProtocolNumber, request.Document.ProtocolYear, request.Document.ProtocolDate, reg.systemId, reg.codRegistro, segnatura);
                        GetDocumentRequest reqGetDoc = new GetDocumentRequest();
                        reqGetDoc.GetFile = false;
                        reqGetDoc.IdDocument = response.Document.Id;
                        GetDocumentResponse respGetDoc = await getDocument(token, reqGetDoc);
                        if (respGetDoc != null && respGetDoc.Document != null)
                        {
                            response.Document = respGetDoc.Document;
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOADDDOCGRIGIA", respGetDoc.Document.Id, "Creazione del documento pregresso" + respGetDoc.Document.Id + ". Tipo documento: " + respGetDoc.Document.DocumentType + ". Predisposto: " + respGetDoc.Document.Predisposed.ToString(), DocsPaVO.Logger.CodAzione.Esito.OK);

                        }
                    }

                    
                }
                response.Code = CreateDocumentResponseCode.OK;
                
                logger.Info("end importPreviousDocument");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione importPreviousDocument: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new CreateDocumentResponse();
                response.Code = CreateDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione importPreviousDocument: " + e);
                response = new CreateDocumentResponse();
                response.Code = CreateDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetTemplateResponse> getTemplateDoc(string token, string descriptionTemplate, string idTemplate)
        {
            logger.Info("begin getTemplateDoc");
            GetTemplateResponse response = new GetTemplateResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                if (string.IsNullOrEmpty(descriptionTemplate) && string.IsNullOrEmpty(idTemplate))
                {
                    throw new RestException("REQUIRED_CODE_OR_ID_TEMPLATE");
                }

                if (!string.IsNullOrEmpty(descriptionTemplate) && !string.IsNullOrEmpty(idTemplate))
                {
                    throw new RestException("REQUIRED_ONLY_CODE_OR_ID_TEMPLATE");
                }

                Template templateResponse = new Template();

                //DocsPaVO.utente.Registro registro = null;
                DocsPaVO.ProfilazioneDinamica.Templates template = null;

                if (!string.IsNullOrEmpty(descriptionTemplate))
                {
                    // Causa errore in presenza di più template con stessa descrizione eseguo un fix
                    DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] listaTemplate = null;
                    listaTemplate = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getListTemplatesLiteByRole(infoUtente.idAmministrazione, infoUtente.idGruppo);

                    if (listaTemplate != null && listaTemplate.Length > 0)
                    {
                        foreach (DocsPaVO.ProfilazioneDinamicaLite.TemplateLite TL in listaTemplate)
                        {
                            if (descriptionTemplate.ToUpper().Equals(TL.name.ToUpper()))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(TL.system_id);
                            }
                        }
                    }
                    //template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(request.DescriptionTemplate);
                }
                else
                {
                    if (!string.IsNullOrEmpty(idTemplate))
                    {
                        try
                        {
                            template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(idTemplate);
                        }
                        catch
                        {
                            //Template non trovato
                            throw new RestException("TEMPLATE_NOT_FOUND");
                        }
                    }
                }
                //Modifica Lembo 10-01-2013: Controllo se l'utente è abilitato all'utilizzo del template.
                // per ora facciamo visualizzare sia quelli con diritti di scrittura che diritti di visualizzazione.
                string controlFilter = string.Format("{0} and id_ruolo={1} and (diritti='1' or diritti='2')", template.SYSTEM_ID.ToString(), infoUtente.idGruppo);
                ArrayList controlloVisibilita = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getRuoliTipoDoc(controlFilter);
                if (controlloVisibilita == null || controlloVisibilita.Count == 0)
                {
                    throw new RestException("TEMPLATE_NOT_ROLE_VISIBLE");
                }


                if (template != null)
                {
                    templateResponse.Name = template.DESCRIZIONE;
                    templateResponse.Id = template.SYSTEM_ID.ToString();

                    DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                    template.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom));


                    if (oggettiCustom != null && oggettiCustom.Length > 0)
                    {
                        templateResponse.Fields = new Field[oggettiCustom.Length];
                        int numField = 0;
                        //Modifica Lembo 11-01-2013: Prelevo i diritti per popolare l'attributo Rights
                        DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli[] dirittiOggetti =
                            (DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli[])(BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getDirittiCampiTipologiaDoc(infoUtente.idGruppo, template.SYSTEM_ID.ToString())).ToArray(typeof(DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli));

                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in oggettiCustom)
                        {

                            Field fieldTemp = new Field();

                            fieldTemp.Id = oggettoCustom.SYSTEM_ID.ToString();
                            fieldTemp.Name = oggettoCustom.DESCRIZIONE;
                            if (!string.IsNullOrEmpty(oggettoCustom.CAMPO_OBBLIGATORIO) && oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                            {
                                fieldTemp.Required = true;
                            }
                            else
                            {
                                fieldTemp.Required = false;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione"))
                            {
                                fieldTemp.Type = "MultipleChoise";
                                if (oggettoCustom.ELENCO_VALORI != null && oggettoCustom.ELENCO_VALORI.Count > 0)
                                {
                                    fieldTemp.MultipleChoice = new string[oggettoCustom.ELENCO_VALORI.Count];
                                    for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Count; i++)
                                    {
                                        fieldTemp.MultipleChoice[i] = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE;
                                    }
                                }
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Corrispondente"))
                            {
                                fieldTemp.Type = "Correspondent";
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CampoDiTesto"))
                            {
                                fieldTemp.Type = "TextField";
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("SelezioneEsclusiva"))
                            {
                                fieldTemp.Type = "ExclusiveSelection";
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("MenuATendina"))
                            {
                                fieldTemp.Type = "DropDown";
                                if (oggettoCustom.ELENCO_VALORI != null && oggettoCustom.ELENCO_VALORI.Count > 0)
                                {
                                    fieldTemp.MultipleChoice = new string[oggettoCustom.ELENCO_VALORI.Count];
                                    for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Count; i++)
                                    {
                                        fieldTemp.MultipleChoice[i] = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE;
                                    }
                                }
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore"))
                            {
                                fieldTemp.Type = "Counter";
                                if (oggettoCustom.TIPO_CONTATORE.Equals("A") || oggettoCustom.TIPO_CONTATORE.Equals("R"))
                                {
                                    DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                    if (reg != null)
                                    {
                                        fieldTemp.CodeRegisterOrRF = reg.codRegistro;
                                    }
                                }
                                fieldTemp.CounterToTrigger = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Data"))
                            {
                                fieldTemp.Type = "Date";
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore"))
                            {
                                fieldTemp.Type = "SubCounter";
                                if (oggettoCustom.TIPO_CONTATORE.Equals("A") || oggettoCustom.TIPO_CONTATORE.Equals("R"))
                                {
                                    DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                    if (reg != null)
                                    {
                                        fieldTemp.CodeRegisterOrRF = reg.codRegistro;
                                    }
                                }
                                fieldTemp.CounterToTrigger = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE + "-" + oggettoCustom.VALORE_SOTTOCONTATORE; ;
                            }
                            //Modifica Lembo 11-01-2013: popolo l'attributo Rights
                            if (!(oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Separatore"))
                            {
                                if (dirittiOggetti.FirstOrDefault(e => e.ID_OGGETTO_CUSTOM.ToUpperInvariant() == oggettoCustom.SYSTEM_ID.ToString().ToUpperInvariant()) != null)
                                {
                                    if (dirittiOggetti.FirstOrDefault(e => e.ID_OGGETTO_CUSTOM.ToUpperInvariant() == oggettoCustom.SYSTEM_ID.ToString().ToUpperInvariant()).INS_MOD_OGG_CUSTOM == "1")
                                        fieldTemp.Rights = "INSERT_AND_MODIFY";
                                    else if (dirittiOggetti.FirstOrDefault(e => e.ID_OGGETTO_CUSTOM.ToUpperInvariant() == oggettoCustom.SYSTEM_ID.ToString().ToUpperInvariant()).VIS_OGG_CUSTOM == "1")
                                        fieldTemp.Rights = "SEARCH_AND_VIEW";
                                    else
                                        fieldTemp.Rights = "NONE";
                                }
                                else
                                    fieldTemp.Rights = "NONE";
                            }
                            else
                            {
                                fieldTemp.Rights = "NONE";
                            }
                            templateResponse.Fields[numField] = fieldTemp;
                            numField++;
                        }
                    }

                    int idDiagramma = 0;
                    DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                    idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociato(template.SYSTEM_ID.ToString());
                    if (idDiagramma != 0)
                    {
                        diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                        if (diagramma != null)
                        {
                            StateDiagram responseDiagram = new StateDiagram();
                            responseDiagram.Description = diagramma.DESCRIZIONE;
                            responseDiagram.Id = diagramma.SYSTEM_ID.ToString();
                            int y = 0;
                            if (diagramma.STATI != null && diagramma.STATI.Count > 0)
                            {
                                responseDiagram.StateOfDiagram = new StateOfDiagram[diagramma.STATI.Count];

                                foreach (DocsPaVO.DiagrammaStato.Stato statoD in diagramma.STATI)
                                {
                                    StateOfDiagram stateDiagram = new StateOfDiagram();
                                    stateDiagram.Description = statoD.DESCRIZIONE;
                                    stateDiagram.Id = statoD.SYSTEM_ID.ToString();
                                    stateDiagram.InitialState = statoD.STATO_INIZIALE;
                                    stateDiagram.FinaleState = statoD.STATO_FINALE;
                                    stateDiagram.DiagramId = responseDiagram.Id;
                                    responseDiagram.StateOfDiagram[y] = stateDiagram;
                                    y++;
                                }
                                templateResponse.StateDiagram = responseDiagram;
                            }
                        }
                    }
                }
                else
                {
                    //Template non trovato
                    throw new RestException("TEMPLATE_NOT_FOUND");
                }

                response.Template = templateResponse;

                response.Code = GetTemplateResponseCode.OK;

                logger.Info("end getTemplateDoc");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getTemplateDoc: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetTemplateResponse();
                response.Code = GetTemplateResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getTemplateDoc: " + e);
                response = new GetTemplateResponse();
                response.Code = GetTemplateResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetTemplatesResponse> getDocumentTemplates(string token)
        {
            logger.Info("begin getDocumentTemplates");
            GetTemplatesResponse response = new GetTemplatesResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] listaTemplate = null;
                listaTemplate = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getListTemplatesLiteByRole(infoUtente.idAmministrazione, infoUtente.idGruppo);

                if (listaTemplate != null && listaTemplate.Length > 0)
                {
                    Template[] templateResponse = new Template[listaTemplate.Length];

                    for (int y = 0; y < listaTemplate.Length; y++)
                    {
                        Template tempTemp = new Template();
                        tempTemp.Name = listaTemplate[y].name;
                        tempTemp.Id = listaTemplate[y].system_id;
                        templateResponse[y] = tempTemp;
                    }

                    response.Templates = templateResponse;
                }
                else
                {
                    //Nessun template trovato
                    throw new RestException("TEMPLATES_NOT_FOUND");
                }


                response.Code = GetTemplatesResponseCode.OK;
                logger.Info("end getDocumentTemplates");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getDocumentTemplates: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetTemplatesResponse();
                response.Code = GetTemplatesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getDocumentTemplates: " + e);
                response = new GetTemplatesResponse();
                response.Code = GetTemplatesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<MessageResponse> addDocInProject(string token, string idDocument, string idProject, string codeProject)
        {
            logger.Info("begin addDocInProject");
            MessageResponse response = new MessageResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                if (string.IsNullOrWhiteSpace(idDocument))
                    throw new RestException("REQUIRED_ID");
                if (string.IsNullOrEmpty(codeProject) && string.IsNullOrEmpty(idProject))
                {
                    throw new RestException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if (!string.IsNullOrEmpty(codeProject) && !string.IsNullOrEmpty(idProject) && !string.IsNullOrEmpty(idDocument))
                {
                    throw new RestException("REQUIRED_ONLY_CODE_OR_ID_PROJECT");
                }

                string msg = string.Empty;
                DocsPaVO.documento.InfoDocumento infoDoc = null;
                ArrayList fascicoli = new ArrayList();
                DocsPaVO.fascicolazione.Fascicolo fascicolo = null;

                //Controllo se esiste il documento
                try
                {

                    infoDoc = BusinessLogic.Documenti.DocManager.GetInfoDocumento(infoUtente, idDocument, idDocument);
                }
                catch
                {
                    //Documento non trovato
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                if (infoDoc != null)
                {
                    //Controllo se esiste il fascicolo
                    try
                    {
                        if (!string.IsNullOrEmpty(idProject))
                        {
                            fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(idProject, infoUtente);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(codeProject))
                            {
                                //Il codice è di un sottofascicolo
                                if (codeProject.IndexOf("//") > -1)
                                {
                                    string separatore = "//";
                                    // MODIFICA per inserimento in sottocartelle
                                    string[] separatoreAr = new string[] { separatore };

                                    string[] pathCartelle = codeProject.Split(separatoreAr, StringSplitOptions.RemoveEmptyEntries);

                                    fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloDaCodice(infoUtente, pathCartelle[0], null, false, false);

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

                                    //int posSep = codeProject.IndexOf("//");
                                    //string codiceFascicolo = codeProject.Substring(0, posSep);
                                    //string descrFolder = codeProject.Substring(posSep + separatore.Length);
                                    //fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFolderDaCodiceFascicolo(infoUtente, codiceFascicolo, descrFolder, null, false, false);
                                    //if (fascicoli != null && fascicoli.Count > 0)
                                    //{
                                    //    if (fascicoli.Count == 1)
                                    //    {
                                    //        //calcolo fascicolo sottofascicolo
                                    //        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(((DocsPaVO.fascicolazione.Folder)fascicoli[0]).idFascicolo, infoUtente);
                                    //        if (fascicolo != null)
                                    //        {
                                    //            //folder selezionato è l'ultimo
                                    //            fascicolo.folderSelezionato = (DocsPaVO.fascicolazione.Folder)fascicoli[fascicoli.Count - 1];
                                    //        }
                                    //    }
                                    //    else
                                    //    {
                                    //        //Fascicoli multipli
                                    //        throw new PisException("MULTIPLE");
                                    //    }
                                    //}
                                }
                                else
                                {
                                    fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliDaCodice(infoUtente, codeProject, null, false, false, "I");
                                    if (fascicoli != null && fascicoli.Count > 0)
                                    {
                                        if (fascicoli.Count == 1)
                                        {
                                            fascicolo = (DocsPaVO.fascicolazione.Fascicolo)fascicoli[0];
                                        }
                                        else
                                        {
                                            //Fascicoli multipli
                                            throw new RestException("MULTIPLE");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //Fascicolo non trovato
                                throw new RestException("PROJECT_NOT_FOUND");
                            }
                        }
                    }
                    catch (RestException pisEx1)
                    {
                        //Fascicolo non trovato
                        throw new RestException("PROJECT_NOT_FOUND");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }

                    if (fascicolo != null)
                    {
                        if (fascicolo.stato == "C")
                            throw new RestException("PROJECT_NOT_FOUND");
                        //Caso del sottofascicolo
                        if (fascicolo.folderSelezionato != null)
                        {
                            BusinessLogic.Fascicoli.FolderManager.addDocFolder(infoUtente, idDocument, fascicolo.folderSelezionato.systemID, false, out msg);
                        }
                        else
                        {
                            BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, idDocument, fascicolo.systemID, false, out msg);
                        }
                        response.ResultMessage = string.Format("Documento {0} inserito nel fascicolo {1}",idDocument, fascicolo.codice);
                    }
                    else
                    {
                        //Fascicolo non trovato
                        throw new RestException("PROJECT_NOT_FOUND");
                    }
                }
                else
                {
                    //Documento non trovato
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                response.Code = MessageResponseCode.OK;
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCADDINFASC", idDocument, "Inserimento del documento " + idDocument + " nel fascicolo " + idProject, DocsPaVO.Logger.CodAzione.Esito.OK);

                logger.Info("end addDocInProject");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione addDocInProject: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new MessageResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione addDocInProject: " + e);
                response = new MessageResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            

            return response;
        }

        public static async Task<SearchDocumentsResponse> getDocumentsInProject(string token, GetDocumentsInProjectRequest request)
        {
            logger.Info("begin getDocumentsInProject");
            SearchDocumentsResponse response = new SearchDocumentsResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                if (string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject) && string.IsNullOrEmpty(request.IdProject))
                {
                    throw new RestException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) || !string.IsNullOrEmpty(request.CodeProject)) && !string.IsNullOrEmpty(request.IdProject))
                {
                    throw new RestException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject)) || (string.IsNullOrEmpty(request.ClassificationSchemeId) && !string.IsNullOrEmpty(request.CodeProject)))
                {
                    throw new RestException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }


                Document[] responseDocuments = null;

                int nRec = 0;
                int numTotPage = 0;
                bool allDocuments = false;
                int pageSize = 20;
                int numPage = 1;

                if ((request.ElementsInPage == null && request.PageNumber == null) || (request.ElementsInPage == 0 && request.PageNumber == 0))
                {
                    allDocuments = true;
                }
                else
                {
                    if ((request.ElementsInPage != null && request.PageNumber == null) || (request.ElementsInPage == null && request.PageNumber != null))
                    {
                        //Paginazione non valida
                        throw new RestException("INVALID_PAGINATION");
                    }
                    else
                    {

                        pageSize = request.ElementsInPage > 0 ? request.ElementsInPage : 20;
                        numPage = request.PageNumber > 0 ? request.PageNumber : 1;
                        allDocuments = false;
                    }
                }

                ArrayList objListaDocumenti = null;
                List<DocsPaVO.ricerche.SearchResultInfo> idProfileList = null;
                DocsPaVO.fascicolazione.Folder folder = null;
                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();
                DocsPaVO.fascicolazione.Fascicolo[] fascicoli = null;

                if (!string.IsNullOrEmpty(request.IdProject))
                {
                    fascicolo.systemID = request.IdProject;
                    folder = BusinessLogic.Fascicoli.FolderManager.getFolderByIdFascicolo(infoUtente.idPeople, infoUtente.idGruppo, fascicolo);
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.ClassificationSchemeId) && !string.IsNullOrEmpty(request.CodeProject))
                    {
                        if (request.CodeProject.IndexOf("//") > -1)
                        {
                            string separatore = "//";
                            // MODIFICA per inserimento in sottocartelle
                            string[] separatoreAr = new string[] { separatore };

                            string[] pathCartelle = request.CodeProject.Split(separatoreAr, StringSplitOptions.RemoveEmptyEntries);

                            fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloDaCodice(infoUtente, pathCartelle[0], null, false, false);

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
                            folder = fascicolo.folderSelezionato;

                            //int posSep = request.CodeProject.IndexOf("//");
                            //string codiceFascicolo = request.CodeProject.Substring(0, posSep);
                            //string descrFolder = request.CodeProject.Substring(posSep + separatore.Length);
                            //fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFolderDaCodiceFascicolo(infoUtente, codiceFascicolo, descrFolder, null, false, false);
                            //if (fascicoli != null && fascicoli.Count > 0)
                            //{
                            //    if (fascicoli.Count == 1)
                            //    {
                            //        //calcolo fascicolo sottofascicolo
                            //        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(((DocsPaVO.fascicolazione.Folder)fascicoli[0]).idFascicolo, infoUtente);
                            //        if (fascicolo != null)
                            //        {
                            //            //folder selezionato è l'ultimo
                            //            fascicolo.folderSelezionato = (DocsPaVO.fascicolazione.Folder)fascicoli[fascicoli.Count - 1];
                            //        }
                            //    }
                            //    else
                            //    {
                            //        //Fascicoli multipli
                            //        throw new PisException("MULTIPLE");
                            //    }
                            //}
                        }
                        else
                        {
                            fascicoli = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceConSecurity(request.CodeProject, infoUtente.idAmministrazione, request.ClassificationSchemeId, false, infoUtente);
                            if (fascicoli != null && fascicoli.Length > 0)
                            {
                                DocsPaVO.fascicolazione.Fascicolo searchFasc = null;

                                if (fascicoli.Length == 1)
                                {
                                    searchFasc = fascicoli[0];
                                }
                                else
                                {
                                    searchFasc = fascicoli[1];
                                }

                                folder = BusinessLogic.Fascicoli.FolderManager.getFolderByIdFascicolo(infoUtente.idPeople, infoUtente.idGruppo, searchFasc);
                            }
                        }
                    }
                }

                if (folder != null)
                {
                    // Lista dei fitri da restituire
                    DocsPaVO.filtri.FiltroRicerca[][] orderRicerca = new DocsPaVO.filtri.FiltroRicerca[1][];
                    orderRicerca = new DocsPaVO.filtri.FiltroRicerca[1][];
                    orderRicerca[0] = new DocsPaVO.filtri.FiltroRicerca[1];
                    DocsPaVO.filtri.FiltroRicerca[] fVlist = new DocsPaVO.filtri.FiltroRicerca[3];
                    fVlist[0] = new DocsPaVO.filtri.FiltroRicerca()
                    {
                        argomento = "ORACLE_FIELD_FOR_ORDER",
                        valore = "NVL (a.dta_proto, a.creation_time)",
                        nomeCampo = "D9",
                    };
                    fVlist[1] = new DocsPaVO.filtri.FiltroRicerca()
                    {
                        argomento = "SQL_FIELD_FOR_ORDER",
                        valore = "ISNULL (a.dta_proto, a.creation_time)",
                        nomeCampo = "D9",
                    };
                    fVlist[2] = new DocsPaVO.filtri.FiltroRicerca()
                    {
                        argomento = "ORDER_DIRECTION",
                        valore = "DESC",
                        nomeCampo = "D9",
                    };

                    orderRicerca[0] = fVlist;

                    objListaDocumenti = BusinessLogic.Fascicoli.FolderManager.getDocumentiPagingCustom(infoUtente, folder, null, numPage, out numTotPage, out nRec, true, out idProfileList, false, allDocuments, null, null, pageSize, orderRicerca);

                    if (objListaDocumenti != null && objListaDocumenti.Count > 0)
                    {
                        DocsPaVO.amministrazione.InfoAmministrazione infoAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(authInfoArray[4]);
                    
                        responseDocuments = new Document[objListaDocumenti.Count];
                        int y = 0;
                        foreach (DocsPaVO.Grids.SearchObject obj in objListaDocumenti)
                        {
                            responseDocuments[y] = Utils.GetDocumentFromSearchObject(obj, false, null, infoAmm.Codice);
                            y++;
                        }
                    }
                    else
                    {
                        //Documenti non trovati
                        throw new RestException("DOCUMENTS_NOT_FOUND");
                    }
                }
                else
                {
                    //Fascicolo non trovato
                    throw new RestException("PROJECT_NOT_FOUND");
                }

                if (idProfileList != null)
                    response.TotalDocumentsNumber = idProfileList.Count;

                response.Documents = responseDocuments;
                response.Code = SearchDocumentsResponseCode.OK;

                logger.Info("end getDocumentsInProject");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getDocumentsInProject: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new SearchDocumentsResponse();
                response.Code = SearchDocumentsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getDocumentsInProject: " + e);
                response = new SearchDocumentsResponse();
                response.Code = SearchDocumentsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<MessageResponse> editDocStateDiagram(string token, string idDocument, string signature, string stateOfDiagram)
        {
            logger.Info("begin editDocStateDiagram");
            MessageResponse response = new MessageResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion

                if (string.IsNullOrEmpty(idDocument) && string.IsNullOrEmpty(signature))
                {
                    throw new RestException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(idDocument) && !string.IsNullOrEmpty(signature))
                {
                    throw new RestException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }

                if (string.IsNullOrEmpty(stateOfDiagram))
                {
                    throw new RestException("REQUIRED_STATE_OF_DIAGRAM");
                }

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                try
                {
                    if (!string.IsNullOrEmpty(idDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idDocument, idDocument);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(signature))
                        {
                            documento = BusinessLogic.Documenti.DocManager.GetDettaglioBySignature(signature, infoUtente);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
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
                                            if (stato.DESCRIZIONE.ToUpper().Equals(stateOfDiagram.ToUpper()))
                                            {
                                                BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(documento.docNumber, stato.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                                result = true;
                                                Utils.ExecAutoTrasmByIdStatus(infoUtente, documento, stato, documento.template.SYSTEM_ID.ToString());
                                                
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
                                                    if (((DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j]).DESCRIZIONE.ToUpper().Equals(stateOfDiagram.ToUpper()))
                                                    {
                                                        BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(documento.docNumber, ((DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j]).SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                                        result = true;
                                                        Utils.ExecAutoTrasmByIdStatus(infoUtente, documento, (DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j], documento.template.SYSTEM_ID.ToString());
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    response.ResultMessage = string.Format("Documento {0} passato nello stato {1}", documento.systemId, stateOfDiagram);
                                    if (!result)
                                    {
                                        throw new RestException("STATEOFDIAGRAM_NOT_FOUND");
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(stateOfDiagram))
                                        {
                                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOC_CAMBIO_STATO", documento.systemId, String.Format("Stato passato a  {0}", stateOfDiagram.ToUpper()), DocsPaVO.Logger.CodAzione.Esito.OK);

                                        }
                                        else
                                        {
                                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOC_CAMBIO_STATO", documento.systemId, "Modificato lo stato del documento " + documento.systemId, DocsPaVO.Logger.CodAzione.Esito.OK);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new RestException("DIAGRAM_NOT_FOUND");
                        }
                    }
                    else
                    {
                        throw new RestException("TEMPLATE_NOT_FOUND");
                    }
                }
                else
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                response.Code = MessageResponseCode.OK;
                logger.Info("end editDocStateDiagram");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione editDocStateDiagram: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new MessageResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione editDocStateDiagram: " + e);
                response = new MessageResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetStateOfDiagramResponse> getDocumentStateDiagram(string token, string idDocument, string signature)
        {
            logger.Info("begin getDocumentStateDiagram");
            GetStateOfDiagramResponse response = new GetStateOfDiagramResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion


                if (string.IsNullOrEmpty(idDocument) && string.IsNullOrEmpty(signature))
                {
                    throw new RestException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(idDocument) && !string.IsNullOrEmpty(signature))
                {
                    throw new RestException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }

                StateOfDiagram stateOfDiagramResponse = new StateOfDiagram();

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                try
                {
                    if (!string.IsNullOrEmpty(idDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idDocument, idDocument);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(signature))
                        {
                            documento = BusinessLogic.Documenti.DocManager.GetDettaglioBySignature(signature, infoUtente);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                if (documento != null)
                {
                    if (documento.template != null)
                    {
                        int idDiagramma = 0;
                        DocsPaVO.DiagrammaStato.Stato stato = null;
                        DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                        idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociato(documento.template.SYSTEM_ID.ToString());
                        if (idDiagramma != 0)
                        {
                            diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                            if (diagramma != null)
                            {
                                stato = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoDoc(documento.docNumber);
                                if (stato != null)
                                {
                                    stateOfDiagramResponse = Utils.GetStateOfDiagram(stato, diagramma.SYSTEM_ID.ToString());
                                }
                                else
                                {
                                    throw new RestException("DOC_STATEOFDIAGRAM_NOT_FOUND");
                                }
                            }
                        }
                        else
                        {
                            throw new RestException("DIAGRAM_NOT_FOUND");
                        }
                    }
                    else
                    {
                        throw new RestException("TEMPLATE_NOT_FOUND");
                    }
                }
                else
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                response.StateOfDiagram = stateOfDiagramResponse;
                response.Code = GetStateOfDiagramResponseCode.OK;
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETDETTAGLIODOCUMENTO", idDocument, "Aperto il dettaglio del diagramma di stato del documento " + idDocument, DocsPaVO.Logger.CodAzione.Esito.OK);

                logger.Info("end getDocumentStateDiagram");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getDocumentStateDiagram: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetStateOfDiagramResponse();
                response.Code = GetStateOfDiagramResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getDocumentStateDiagram: " + e);
                response = new GetStateOfDiagramResponse();
                response.Code = GetStateOfDiagramResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<MessageResponse> sendDocument(string token, SendDocRequest request)
        {
            logger.Info("begin sendDocument");
            MessageResponse response = new MessageResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion

                if (string.IsNullOrEmpty(request.IdDocument) && string.IsNullOrEmpty(request.Signature))
                {
                    throw new RestException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(request.IdDocument) && !string.IsNullOrEmpty(request.Signature))
                {
                    throw new RestException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }

                //Reperimento ruolo utente
                ruolo = new DocsPaVO.utente.Ruolo();

                ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                if (ruolo == null)
                {
                    //Ruolo non trovato
                    throw new RestException("ROLE_NO_EXIST");
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
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                if (documento != null)
                {
                    ///TODO
                    DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizione = new DocsPaVO.Spedizione.SpedizioneDocumento();

                    //filtro dagli allegati da spedire quelli associati a notifiche di tipo PEC
                    ArrayList listAllegati = new ArrayList();
                    if (documento.allegati != null && documento.allegati.Count > 0)
                    {
                        foreach (Allegato a in documento.allegati)
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

                    infoSpedizione = Utils.GetInfoSpedizione(documento, infoUtente);

                    List<DocsPaVO.utente.Registro> listRegistriRF = new List<DocsPaVO.utente.Registro>();
                    string mailspedizione = "";
                    if (documento.registro != null && !string.IsNullOrEmpty(documento.registro.systemId))
                    {
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(documento.registro.systemId);
                        ArrayList arrReg = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(infoUtente.idCorrGlobali, "1", documento.registro.systemId);
                        foreach (DocsPaVO.utente.Registro registro in arrReg)
                        {
                            System.Data.DataSet ds = BusinessLogic.Amministrazione.RegistroManager.GetRightRuoloMailRegistro(registro.systemId, ruolo.systemId);
                            if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                            {
                                foreach (System.Data.DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                                {
                                    if (row["SPEDISCI"].ToString().Equals("1"))
                                    {
                                        listRegistriRF.Add(registro);
                                        mailspedizione = row["EMAIL_REGISTRO"].ToString();
                                        break;
                                    }
                                }
                            }
                        }

                        //prendo il registro corrente
                        System.Data.DataSet dsReg = BusinessLogic.Amministrazione.RegistroManager.GetRightRuoloMailRegistro(documento.registro.systemId, ruolo.systemId);
                        if (dsReg.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                        {
                            foreach (System.Data.DataRow row in dsReg.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                            {
                                if (row["SPEDISCI"].ToString().Equals("1"))
                                {
                                    listRegistriRF.Add(reg);
                                    mailspedizione = row["EMAIL_REGISTRO"].ToString();
                                    break;
                                }
                            }
                        }

                        if (listRegistriRF != null && listRegistriRF.Count > 0)
                        {
                            infoSpedizione.IdRegistroRfMittente = listRegistriRF[0].systemId;
                            infoSpedizione.mailAddress = listRegistriRF[0].email;
                            if (!string.IsNullOrEmpty(mailspedizione)) infoSpedizione.mailAddress = mailspedizione;
                        }
                        else
                        {
                            response.Code= MessageResponseCode.SYSTEM_ERROR;
                        }

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
                    logger.Debug("infoSpedizione.IdRegistroRfMittente = " + infoSpedizione.IdRegistroRfMittente);
                    logger.Debug("infoSpedizione.mailAddress =" + infoSpedizione.mailAddress);
                    infoSpedizione = BusinessLogic.Spedizione.SpedizioneManager.SpedisciDocumento(infoUtente, documento, infoSpedizione);
                    if (infoSpedizione.Spedito)
                        response.Code = MessageResponseCode.OK;
                    else
                        response.Code = MessageResponseCode.SYSTEM_ERROR;
                    if (!infoSpedizione.Spedito)
                    {
                        throw new RestException("SEND_DOCUMENT_FAILED");
                    }
                    else
                    {
                        response.ResultMessage = "Documento spedito";
                    }
                }
                else
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                response.Code = MessageResponseCode.OK;
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSPEDISCI", documento.systemId, "Spedizione documento " + documento.systemId, DocsPaVO.Logger.CodAzione.Esito.OK);

                logger.Info("end sendDocument");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione sendDocument: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new MessageResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione sendDocument: " + e);
                response = new MessageResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetStampResponse> getStampAndSignature(string token, string idDocument, string signature)
        {
            logger.Info("begin getStampAndSignature");
            GetStampResponse response = new GetStampResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion

                if (string.IsNullOrEmpty(idDocument) && string.IsNullOrEmpty(signature))
                {
                    throw new RestException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(idDocument) && !string.IsNullOrEmpty(signature))
                {
                    throw new RestException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                try
                {
                    if (!string.IsNullOrEmpty(idDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, idDocument, idDocument);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(signature))
                        {
                            documento = BusinessLogic.Documenti.DocManager.GetDettaglioBySignature(signature, infoUtente);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                try
                {

                    if (documento != null)
                    {
                        response.Stamp = Utils.GetStampAndSignature(documento, infoUtente);

                    }
                    else
                    {
                        throw new RestException("DOCUMENT_NOT_FOUND");
                    }


                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }


                response.Code = GetStampResponseCode.OK;
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETDETTAGLIODOCUMENTO", idDocument, "Aperto il dettaglio del diagramma di stato del documento " + idDocument, DocsPaVO.Logger.CodAzione.Esito.OK);

                logger.Info("end getStampAndSignature");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getStampAndSignature: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetStampResponse();
                response.Code = GetStampResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getStampAndSignature: " + e);
                response = new GetStampResponse();
                response.Code = GetStampResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<SendDocAdvResponse> sendDocAdvanced(string token, SendDocAdvRequest request)
        {
            logger.Info("begin sendDocAdvanced");
            SendDocAdvResponse response = new SendDocAdvResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion
                DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizione = new DocsPaVO.Spedizione.SpedizioneDocumento();

                //Reperimento ruolo utente
                ruolo = new DocsPaVO.utente.Ruolo();

                ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                if (ruolo == null)
                {
                    //Ruolo non trovato
                    throw new RestException("ROLE_NO_EXIST");
                }
                // Fine reperimento ruolo

                // Controllo documento
                if (string.IsNullOrEmpty(request.IdDocument) && string.IsNullOrEmpty(request.Signature))
                {
                    throw new RestException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(request.IdDocument) && !string.IsNullOrEmpty(request.Signature))
                {
                    throw new RestException("REQUIRED_ONLY_ID_OR_SIGNATURE");
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
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }
                // Fine controllo documento

                if (string.IsNullOrEmpty(request.CodeRegister) && string.IsNullOrEmpty(request.IdRegister))
                {
                    throw new RestException("REQUIRED_CODE_OR_ID_REGISTER");
                }

                if (!string.IsNullOrEmpty(request.CodeRegister) && !string.IsNullOrEmpty(request.IdRegister))
                {
                    throw new RestException("REQUIRED_ONLY_CODE_OR_ID_REGISTER");
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
                            throw new RestException("REGISTER_NOT_FOUND");
                        }
                    }
                }
                infoSpedizione.IdRegistroRfMittente = registro.systemId;
                string mailMittente = "";
                System.Data.DataSet ds = BusinessLogic.Amministrazione.RegistroManager.GetRightRuoloMailRegistro(registro.systemId, ruolo.systemId);
                bool ruoloAbilitatoSpedizione = false;
                if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                {
                    foreach (System.Data.DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
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
                    foreach (System.Data.DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
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
                    foreach (System.Data.DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
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
                        foreach (Correspondent corrPIS in request.Recipients)
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
                                    throw new RestException("RECIPIENT_NOT_FOUND");
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
                    if (!infoSpedizione.Spedito)
                    {
                        throw new RestException("SEND_DOCUMENT_FAILED");
                    }
                    //List<string> EsitoPerCorr = new List<string>();
                    List<SendingResult> SendRess = new List<SendingResult>();
                    SendingResult sendRes = null;
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
                            sendRes = new SendingResult();
                            sendRes.CorrespondentId = corr.Id;
                            sendRes.CorrespondentDescription = corr.DatiDestinatari[0].descrizione;
                            sendRes.Mail = corr.Email;
                            sendRes.PrefChannel = corr.DatiDestinatari[0].canalePref.descrizione;
                            sendRes.SendResult = corr.StatoSpedizione.Descrizione;
                            logger.DebugFormat("ID {0} - Dest: {1} - Mail {2} - Stato: {3} - CanPref {4}", sendRes.CorrespondentId, sendRes.CorrespondentDescription, sendRes.Mail, sendRes.SendResult, sendRes.PrefChannel);
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
                response.Code = SendDocAdvResponseCode.OK;
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSPEDISCI", documento.systemId, "Spedizione documento " + documento.systemId, DocsPaVO.Logger.CodAzione.Esito.OK);

                logger.Info("end sendDocAdvanced");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione sendDocAdvanced: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new SendDocAdvResponse();
                response.Code = SendDocAdvResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione sendDocAdvanced: " + e);
                response = new SendDocAdvResponse();
                response.Code = SendDocAdvResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

    }
}
