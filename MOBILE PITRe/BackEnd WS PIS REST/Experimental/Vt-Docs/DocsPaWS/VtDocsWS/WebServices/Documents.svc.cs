using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VtDocsWS.Services;
using System.ServiceModel.Activation;
using System.Diagnostics;
using System.Reflection;
using log4net;
using DocsPaWS.VtDocsWS;

namespace VtDocsWS.WebServices
{
    /// <summary>
    /// Metodi per la gestione dei documenti
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Documents : IDocuments
    {
        private ILog logger = LogManager.GetLogger(typeof(Documents));

        /// <summary>
        /// Servizio per aggiungere un documento in un fascicolo. Se inserito il codice la ricerca è effettuata solo sul titolario attivo e sui fascioli aperti
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.AddDocInProject.AddDocInProjectResponse AddDocInProject(Services.Documents.AddDocInProject.AddDocInProjectRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.AddDocInProject.AddDocInProjectResponse response = Manager.DocumentsManager.AddDocInProject(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCADDINFASC", request.IdDocument, "Inserimento del documento " + request.IdDocument + " nel fascicolo " + request.IdProject, esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per la creazione di un documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.CreateDocument.CreateDocumentResponse CreateDocument(Services.Documents.CreateDocument.CreateDocumentRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.CreateDocument.CreateDocumentResponse response = Manager.DocumentsManager.CreateDocument(request, false);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            
            string idDocumento = (response.Document != null ? Convert.ToString(response.Document.Id) : Convert.ToString(0));
            string tipoDocumento = (response.Document != null ? response.Document.DocumentType : "-");
            string documentoPredisposto = (response.Document != null ? (response.Document.Predisposed ? "Si" : "No") : "-");

            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOADDDOCGRIGIA", idDocumento, "Creazione del nuovo documento " + idDocumento + ". Tipo documento: " + tipoDocumento + ". Predisposto: " + documentoPredisposto, esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per la creazione di un documento da WordAddIn
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.CreateDocument.CreateDocumentResponse CreateDocumentFromWord(Services.Documents.CreateDocument.CreateDocumentRequest request)
        {
            logger.Info("BEGIN");

            bool existDate = false;
            //string creationDate = string.Empty;

            if (request != null && request.Document.Template != null)
            {
                foreach (Domain.Field profilato in request.Document.Template.Fields)
                {
                    if (profilato.Name == "Creation Date")
                    {
                        existDate = true;
                        profilato.Value = System.DateTime.Today.ToShortDateString();
                        //creationDate = profilato.Value;
                        break;
                    }
                }
            }
            
            if (!existDate)
            {
                List<Domain.Field> campiProfilo = new List<Domain.Field>();
                foreach (Domain.Field profilato in request.Document.Template.Fields)
                {
                    campiProfilo.Add(profilato);
                }

                Domain.Field profil = new Domain.Field();
                profil.Name = "Creation Date";
                profil.Value = System.DateTime.Today.ToShortDateString();

                request.Document.Template.Fields = campiProfilo.ToArray();
            }

            Services.Documents.CreateDocument.CreateDocumentResponse response = Manager.DocumentsManager.CreateDocument(request, false);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");

            string idDocumento = (response.Document != null ? Convert.ToString(response.Document.Id) : Convert.ToString(0));
            string tipoDocumento = (response.Document != null ? response.Document.DocumentType : "-");
            string documentoPredisposto = (response.Document != null ? (response.Document.Predisposed ? "Si" : "No") : "-");

            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOADDDOCGRIGIA", idDocumento, "Creazione del nuovo documento " + idDocumento + ". Tipo documento: " + tipoDocumento + ". Predisposto: " + documentoPredisposto, esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per cambiare lo stato di un diagramma associato ad un documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.EditDocStateDiagram.EditDocStateDiagramResponse EditDocStateDiagram(Services.Documents.EditDocStateDiagram.EditDocStateDiagramRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.EditDocStateDiagram.EditDocStateDiagramResponse response = Manager.DocumentsManager.EditDocStateDiagram(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            if (request != null && !string.IsNullOrEmpty(request.StateOfDiagram))
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOC_CAMBIO_STATO", request.IdDocument, String.Format("Stato passato a  {0}", request.StateOfDiagram.ToUpper()),esito);
                
            }
            else
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOC_CAMBIO_STATO", request.IdDocument, "Modificato lo stato del documento " + request.IdDocument, esito);
            }
            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per modificare un documento esistente
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.EditDocument.EditDocumentResponse EditDocument(Services.Documents.EditDocument.EditDocumentRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.EditDocument.EditDocumentResponse response = Manager.DocumentsManager.EditDocument(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSAVEDOCUMENTO", request.Document.Id, "Modificati i dati del documento " + request.Document.Id, esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per reperire il dettaglio di un documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.GetDocument.GetDocumentResponse GetDocument(Services.Documents.GetDocument.GetDocumentRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.GetDocument.GetDocumentResponse response = Manager.DocumentsManager.GetDocument(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETDETTAGLIODOCUMENTO", request.IdDocument, "Aperto dettaglio del documento " + request.IdDocument, esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }


        /// <summary>
        /// Servizio per il reperimento lo stato di un documento associato ad un diagramma di stato
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.GetDocumentStateDiagram.GetDocumentStateDiagramResponse GetDocumentStateDiagram(Services.Documents.GetDocumentStateDiagram.GetDocumentStateDiagramRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.GetDocumentStateDiagram.GetDocumentStateDiagramResponse response = Manager.DocumentsManager.GetDocumentStateDiagram(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETDETTAGLIODOCUMENTO", request.IdDocument, "Aperto il dettaglio del diagramma di stato del documento " + request.IdDocument, esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento del file associato ad un documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.GetFileDocumentById.GetFileDocumentByIdResponse GetFileDocumentById(Services.Documents.GetFileDocumentById.GetFileDocumentByIdRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.GetFileDocumentById.GetFileDocumentByIdResponse response = Manager.DocumentsManager.GetFileDocumentById(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request,"");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETFILE", request.IdDocument, "Visualizzato il documento " + request.IdDocument, esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento del file associato ad un documento con l'aggiunta della segnatura o del timbro
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.GetFileWithSignatureAndSignerInfo.GetFileWithSignatureAndSignerInfoResponse GetFileWithSignatureAndSignerInfo(Services.Documents.GetFileWithSignatureAndSignerInfo.GetFileWithSignatureAndSignerInfoRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.GetFileWithSignatureAndSignerInfo.GetFileWithSignatureAndSignerInfoResponse response = Manager.DocumentsManager.GetFileWithSignatureAndSignerInfo(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request,"");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETFILE", request.IdDocument, "Visualizzato il documento " + request.IdDocument, esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento del file associato ad un documento con l'aggiunta della segnatura o del timbro
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.GetFileWithSignatureOrStamp.GetFileWithSignatureOrStampResponse GetFileWithSignatureOrStamp(Services.Documents.GetFileWithSignatureOrStamp.GetFileWithSignatureOrStampRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.GetFileWithSignatureOrStamp.GetFileWithSignatureOrStampResponse response = Manager.DocumentsManager.GetFileWithSignatureOrStamp(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETFILE", request.IdDocument, "Visualizzato il documento " + request.IdDocument, esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento del dettaglio di una tipologia di documento dato il nome o l’id.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.GetTemplateDoc.GetTemplateDocResponse GetTemplateDoc(Services.Documents.GetTemplateDoc.GetTemplateDocRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.GetTemplateDoc.GetTemplateDocResponse response = Manager.DocumentsManager.GetTemplateDoc(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETFILE", request.IdTemplate, "Visualizzato il template documento " + request.IdTemplate, esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento di tutte le tipologie di documenti.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.GetTemplatesDocuments.GetTemplatesDocumentsResponse GetTemplatesDocuments(Services.Documents.GetTemplatesDocuments.GetTemplatesDocumentsRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.GetTemplatesDocuments.GetTemplatesDocumentsResponse response = Manager.DocumentsManager.GetTemplatesDocuments(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETFILE", "0", "Visualizzati i template documento", esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per la ricerca di documenti.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.SearchDocuments.SearchDocumentsResponse SearchDocuments(Services.Documents.SearchDocuments.SearchDocumentsRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.SearchDocuments.SearchDocumentsResponse response = Manager.DocumentsManager.SearchDocuments(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "SAVERICERCA", "0", "Ricerca documenti", esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per l’invio di un documento.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.SendDocument.SendDocumentResponse SendDocument(Services.Documents.SendDocument.SendDocumentRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.SendDocument.SendDocumentResponse response = Manager.DocumentsManager.SendDocument(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSPEDISCI", request.IdDocument, "Spedizione documento " + request.IdDocument, esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per reperire i documenti presenti in un fascicolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.GetDocumentsInProject.GetDocumentsInProjectResponse GetDocumentsInProject(Services.Documents.GetDocumentsInProject.GetDocumentsInProjectRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.GetDocumentsInProject.GetDocumentsInProjectResponse response = Manager.DocumentsManager.GetDocumentsInProject(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "GETDOCUMENTFILTERS", request.IdProject, "Elenco dei documenti presenti nel fascicolo " + request.IdProject, esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per reperire i documenti presenti in un fascicolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.UploadFileToDocument.UploadFileToDocumentResponse UploadFileToDocument(Services.Documents.UploadFileToDocument.UploadFileToDocumentRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.UploadFileToDocument.UploadFileToDocumentResponse response = Manager.DocumentsManager.UploadFileToDocument(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            if (request.CreateAttachment)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", request.IdDocument, "Aggiunto un nuovo allegato al documento " + request.IdDocument, esito);
            }
            else
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIVERSIONE", request.IdDocument, string.Format("{0}{1}{2}", "Aggiunta al N.ro Doc.: ", request.IdDocument, " una nuova versione tramite PIS "),esito);
            }
            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per reperire i documenti presenti in un fascicolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.UploadFileToDocument.UploadFileToDocumentResponse UploadFileToDocumentFromWord(Services.Documents.UploadFileToDocument.UploadFileToDocumentRequest request, string offerDate)
        {
            logger.Info("BEGIN");

            string actualDate = System.DateTime.Today.ToShortDateString();
            Services.Documents.UploadFileToDocument.UploadFileToDocumentResponse response = new Services.Documents.UploadFileToDocument.UploadFileToDocumentResponse();

            if (string.IsNullOrEmpty(offerDate.Trim()))
                offerDate = actualDate;

            if (DateTime.Parse(offerDate) <= DateTime.Parse(actualDate))
            {
                // Log
                //
                response = Manager.DocumentsManager.UploadFileToDocument(request);
                DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
                DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", request.IdDocument, "Aggiunto un nuovo allegato al documento " + request.IdDocument, esito);

                logger.Info("END");
            }
            else
            {
                response.Success = false;
                response.Error = new ResponseError();
                response.Error.Code = actualDate;
                response.Error.Description = "Non è possibile indicare una data futura.";
            }

            Utils.CheckFaultException(response);

            return response;
        }
        /// <summary>
        /// Servizio per reperire i dati completi del timbro e della segnatura
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.GetStampAndSignature.GetStampAndSignatureResponse GetStampAndSignature(Services.Documents.GetStampAndSignature.GetStampAndSignatureRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.GetStampAndSignature.GetStampAndSignatureResponse response = Manager.DocumentsManager.GetStampAndSignature(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETDETTAGLIODOCUMENTO", request.IdDocument, "Reperiti i dati completi del timbro e della segnatura per il documento " + request.IdDocument, esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio che restituisce la lista dei filtri applicabili alla ricerca documenti
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.GetDocumentFilters.GetDocumentFiltersResponse GetDocumentFilters(Services.Documents.GetDocumentFilters.GetDocumentFiltersRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.GetDocumentFilters.GetDocumentFiltersResponse response = Manager.DocumentsManager.GetDocumentFilters(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "GETDOCUMENTFILTERS", "0", "Lista dei filtri applicabili nella ricerca documenti", esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio che crea un nuovo documento e lo inserisce nel relativo fascicolo indicato
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public Services.Documents.CreateDocumentAndAddInProject.CreateDocumentAndAddInProjectResponse CreateDocumentAndAddInProject(Services.Documents.CreateDocumentAndAddInProject.CreateDocumentAndAddInProjectRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.CreateDocumentAndAddInProject.CreateDocumentAndAddInProjectResponse response = Manager.DocumentsManager.CreateDocumentAndAddInProject(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            string idDocumento = (response.Document != null ? Convert.ToString(response.Document.Id) : Convert.ToString(0));
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "CREATEDOCUMENTANDADDINPROJECT", idDocumento, "Creazione nuovo doc " + idDocumento + " e inserimento nel fascicolo " + request.IdProject, esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// 
        /// Servizio che permette alla coppia (ruolo,utente) di monitorare un documento presente nel sistema.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public Services.FollowDomainObject.FollowResponse FollowDocument(Services.FollowDomainObject.FollowRequest request)
        {
            logger.Info("BEGIN");
            Services.FollowDomainObject.FollowResponse response = Manager.DocumentsManager.FollowDocument(request);
            logger.Info("END");
            Utils.CheckFaultException(response);
            return response;
        }

        /// <summary>
        /// Consente di Annullare un Documento Protocollato
        /// </summary>
        /// <param name="Request">In input l'oggetto InvalidDocumentRequest</param>
        /// <returns>Restituisce l'oggetto InvalidDocumentResponse</returns>
        public Services.Documents.InvalidateDocument.InvalidateDocumentResponse InvalidateDocument(Services.Documents.InvalidateDocument.InvalidateDocumentRequest Request)
        {
            logger.Info("START INVALIDATEDOCUMENT");
            Services.Documents.InvalidateDocument.InvalidateDocumentResponse Response = Manager.DocumentsManager.InvalidateDocument(Request);
            logger.Info("END INVALIDATEDOCUMENT");
            return Response;
        }

        /// <summary>
        /// Servizio per il prelievo del Link Documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Services.Documents.GetLinkDocByID.GetLinkDocByIDResponse GetLinkDocByID(Services.Documents.GetLinkDocByID.GetLinkDocByIDRequest request)
        {
            logger.Info("BEGIN");
            Services.Documents.GetLinkDocByID.GetLinkDocByIDResponse response = Manager.DocumentsManager.GetLinkDocByID(request);
            logger.Info("END");
            Utils.CheckFaultException(response);
            return response;
        }

        public Services.Documents.GetEnvelopedFileById.GetEnvelopedFileByIdResponse GetEnvelopedFileById(Services.Documents.GetEnvelopedFileById.GetEnvelopedFileByIdRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.GetEnvelopedFileById.GetEnvelopedFileByIdResponse response = Manager.DocumentsManager.GetEnvelopedFileById(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOGETFILE", request.IdDocument, "Visualizzato il documento " + request.IdDocument, esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per aggiungere un allegato al documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Documents.AddAttachment.AddAttachmentResponse AddAttachment(Services.Documents.AddAttachment.AddAttachmentRequest request)
        {
            logger.Info("BEGIN");

            Services.Documents.AddAttachment.AddAttachmentResponse response = Manager.DocumentsManager.AddAttachment(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCNEWALLEGATO", request.IdDocument, "Aggiunto un nuovo allegato al documento " + request.IdDocument, esito);
            
            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }
    }
}
