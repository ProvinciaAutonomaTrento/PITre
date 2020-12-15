using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Data;
using DocsPaVO;
using DocsPaVO.documento;
using DocsPaVO.utente;
using DocsPaDocumentale.Interfaces;
using DocsPaDocumentale_OCS.OCSServices;
using DocsPaDocumentale_OCS.CorteContentServices;
using log4net;

namespace DocsPaDocumentale_OCS.Documentale
{
    /// <summary>
    /// Classe per la gestione del documentale OCS.
    /// Ha la responsabilità di interagire direttamente 
    /// con le API esposte da OCS per soddisfare le richieste.
    /// </summary>
    public class DocumentManager : IDocumentManager
    {
        private ILog logger = LogManager.GetLogger(typeof(DocumentManager));
        #region Ctors, constants, variables

        /// <summary>
        /// Credenziali utente
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// Istanza webservices documenti
        /// </summary>
        private CorteContentServices.DocumentManagementSOAPHTTPBinding _wsDocument = null;

        /// <summary>
        /// Istanza webservices per la gestione delle versioni dei documenti
        /// </summary>
        private CorteContentServices.VersionManagementSOAPHTTPBinding _wsVersion = null;

        /// <summary>
        /// Istanza webservices per gli allegati dei documenti
        /// </summary>
        private CorteContentServices.AttachmentManagementSOAPHTTPBinding _wsAttachment = null;

        /// <summary>
        /// Inizializza l'istanza della classe acquisendo i dati relativi all'utente 
        /// ed alla libreria per la connessione al documentale.
        /// </summary>
        /// <param name="infoUtente">Dati relativi all'utente</param>
        public DocumentManager(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #endregion

        #region Gestione documenti grigi / protocollati / stampe registro

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo)
        {
            DocsPaVO.utente.Ruolo[] ruoliSuperiori;
            return this.CreateDocumentoGrigio(schedaDocumento, ruolo, out ruoliSuperiori);
        }

        /// <summary>
        /// Creazione di un nuovo documento grigio
        /// 
        /// PreCondizioni:
        ///     Il documento grigio deve essere già stato come entità in DocsPa
        /// 
        /// PostCondizioni:
        ///     I metadati del documento devono essere stati inseriti in OCS
        ///     Nessun dato deve essere modificato negli oggetti forniti in ingresso
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string conCopia = null)
        {
            ruoliSuperiori = null;
            bool retValue = false;

            try
            {
                CorteContentServices.ItemIdResponseType itemResp;
                //preparazione dell'oggetto documento con i metadati del protocollo
                //old:   CorteContentServices.DocumentCreateRequestType docCreateRequest = new CorteContentServices.DocumentCreateRequestType();
                CorteContentServices.VersionCreateDocumentRequestType docCreateRequest = new CorteContentServices.VersionCreateDocumentRequestType();

                docCreateRequest.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                docCreateRequest.document = new CorteContentServices.DocumentCreateType();
                docCreateRequest.document.info = setDocumentInfo(schedaDocumento, true);
                if (docCreateRequest.document.info == null)
                {
                    retValue = false;
                    logger.Debug("Errore in OCS.CreateDocumentoGrigio: non è stato possibile recuperare la location.");
                }
                docCreateRequest.document.categories = OCSServices.OCSDocumentHelper.getDocumentProperties(schedaDocumento, _infoUtente);

                docCreateRequest.version = new CorteContentServices.VersionDefinitionType();
                docCreateRequest.version.versionId = Convert.ToInt32(((DocsPaVO.documento.Documento)schedaDocumento.documenti[0]).versionId);

                itemResp = WsVersionInstance.CreateDocumentVersion(docCreateRequest);

                OCSUtils.throwExceptionIfInvalidResult(itemResp.result);

                //impostazione delle acl
                retValue = OCSDocumentHelper.setAclDocument(schedaDocumento.systemId, itemResp.itemId, this.InfoUtente, OCSUtils.getApplicationUserCredentials());

                if (retValue)
                    // gestione location su DocsPA CONTROLLARE LA FATTIBILITA' del campo PATH
                    DocsPaServices.DocsPaQueryHelper.updateLocation(docCreateRequest.document.info.location, schedaDocumento.docNumber);
                else
                    // Rimozione del documento in caso di errore
                    this.RemoveDocument(itemResp.itemId);

                if (retValue)
                    logger.Debug(string.Format("OCS.CreateDocumentoGrigio: creato nuovo documento, docnumber {0}", schedaDocumento.docNumber));
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.CreateDocumentoGrigio", ex);
            }

            return retValue;
        }

        /// <summary>
        /// Creazione di un nuovo documento per la stampa registro.
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo)
        {
            DocsPaVO.utente.Ruolo[] ruoliSuperiori;
            return this.CreateDocumentoStampaRegistro(schedaDocumento, ruolo, out ruoliSuperiori);
        }

        /// <summary>
        /// Operazione per la creazione di un nuovo documento
        /// 
        /// PreCondizioni:
        ///     Il documento grigio deve essere già stato creato come entità in DocsPa
        /// 
        /// PostCondizioni:
        ///     I metadati del documento devono essere stati inseriti in OCS
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="ruoliSuperiori"></param>
        /// <returns></returns>
        public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            bool retValue = false;
            ruoliSuperiori = null;

            try
            {
                CorteContentServices.VersionCreateDocumentRequestType docCreateRequest = new CorteContentServices.VersionCreateDocumentRequestType();

                CorteContentServices.ItemIdResponseType itemResp;
                //preparazione dell'oggetto documento con i metadati della stampaRegistro 
                //CorteContentServices.DocumentCreateRequestType docCreateRequest = new CorteContentServices.DocumentCreateRequestType();
                docCreateRequest.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                docCreateRequest.document = new CorteContentServices.DocumentCreateType();
                //per reperire le informazioni sulla UO in cui è stato creato il documento:

                docCreateRequest.document.info = setDocumentInfo(schedaDocumento, true);
                docCreateRequest.document.info.description = schedaDocumento.oggetto.descrizione;
                docCreateRequest.document.categories = OCSServices.OCSDocumentHelper.getDocumentStampaRegistroProperties(schedaDocumento, _infoUtente);

                docCreateRequest.version = new CorteContentServices.VersionDefinitionType();
                docCreateRequest.version.versionId = Convert.ToInt32(((DocsPaVO.documento.Documento)schedaDocumento.documenti[0]).versionId);

                itemResp = WsVersionInstance.CreateDocumentVersion(docCreateRequest);

                OCSUtils.throwExceptionIfInvalidResult(itemResp.result);

                //NB SABRI: DA QUESTO PUNTO SE QUALCOSA SU OCS VA' MALE, IL DOCUMENTO DEVE ESSERE CANCELLATO
                //impostazione delle acl
                retValue = OCSServices.OCSDocumentHelper.setAclDocument(schedaDocumento.systemId, itemResp.itemId, this.InfoUtente, OCSUtils.getApplicationUserCredentials());

                if (retValue)
                    // gestione location su DocsPA CONTROLLARE LA FATTIBILITA' (NB: ANDREBBE SPOSTATO NELLO STRATO CDC)
                    DocsPaServices.DocsPaQueryHelper.updateLocation(docCreateRequest.document.info.location, schedaDocumento.docNumber);
                else
                    // Rimozione del documento in OCS
                    this.RemoveDocument(itemResp.itemId);

                if (retValue)
                    logger.Debug(string.Format("OCS.CreateDocumentoGrigio: creato nuovo documento, docnumber {0}", schedaDocumento.docNumber));
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.CreateDocumentoStampaRegistro", ex);
            }

            return retValue;
        }

        /// <summary>
        /// Rimozione di un documento grigio
        /// 
        /// BUG OCS:
        /// sembra che le dfs abbiano dei problemi nella cancellazione degli
        /// allegati, o più in generale nella cancellazione di tutti i documenti
        /// facenti parte di un vdoc.
        /// Per questo motivo, si è costretti a cancellare il documento grigio in
        /// due fasi, la prima per la canc di tutte le versioni, la seconda
        /// per la cancellazione degli allegati dove presenti.
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public string RemoveDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            string retValue = string.Empty;

            try
            {
                if (this.Remove(new DocsPaVO.documento.InfoDocumento(schedaDocumento)))
                {
                    retValue = "Del";

                    logger.Debug(string.Format("OCS.RemoveDocumentoGrigio: rimosso documento con docnumber {0}", schedaDocumento.docNumber));
                }
                else
                {
                    retValue = string.Format("Errore nella rimozione del documento {0} in OCS", schedaDocumento.docNumber);

                    logger.Debug(retValue);
                }
            }
            catch (Exception ex)
            {
                retValue = string.Format("Errore in OCS.RemoveDocumentoGrigio:\n{0}", ex.ToString());

                logger.Debug("Errore in OCS.RemoveDocumentoGrigio", ex);
            }

            return retValue;
        }

        /// <summary>
        /// Rimozione documenti
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public bool Remove(params DocsPaVO.documento.InfoDocumento[] items)
        {
            bool retValue = false;

            try
            {
                UserCredentialsType credentials = OCSUtils.getApplicationUserCredentials();

                // Per ogni documento da rimuovere, reperimento 
                // idOCS e cancellazione  
                // la cancellazione deve prevedere quella delle versioni e degli allegati e di eventuali link
                foreach (DocsPaVO.documento.InfoDocumento item in items)
                {
                    long idOcs = 0;

                    // Determinazione della categoria del documento da ricercare
                    string documentCategory = string.Empty;
                    string categoryField = string.Empty;
                    bool searchInCestino = false;

                    if (item.tipoProto == "R")
                    {
                        documentCategory = DocsPaDocumentale_OCS.DocsPaObjectType.ObjectTypes.CATEGOTY_STAMPA_REGISTRO;
                        categoryField = DocsPaDocumentale_OCS.DocsPaObjectType.TypeDocumentoStampaRegistro.DOC_NUMBER;
                        searchInCestino = false;
                    }
                    else
                    {
                        documentCategory = DocsPaDocumentale_OCS.DocsPaObjectType.ObjectTypes.CATEGOTY_PROTOCOLLO;
                        categoryField = DocsPaDocumentale_OCS.DocsPaObjectType.TypeDocumentoProtocollo.DOC_NUMBER;
                        searchInCestino = true;
                    }
                    
                    // Ricerca dell'id ocs dalla cartella 
                    idOcs = OCSDocumentHelper.getIdDocument(item.docNumber, documentCategory, categoryField, searchInCestino, credentials);
                             
                    // Cancellazione definiva del documento in OCS
                    retValue = this.RemoveDocument(idOcs);

                    if (!retValue)
                        break;
                }
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.Remove", ex);
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        protected virtual bool RemoveDocument(long itemId)
        {
            bool retValue = false;

            try
            {
                CorteContentServices.ItemIdRequestType itemReq = new CorteContentServices.ItemIdRequestType();
                CorteContentServices.ResultType result;
                itemReq.itemId = itemId;
                itemReq.userCredentials = OCSUtils.getApplicationUserCredentials();

                DocumentManagementSOAPHTTPBinding wsDocument = OCSServiceFactory.GetDocumentServiceInstance<DocumentManagementSOAPHTTPBinding>();
                result = wsDocument.DeleteDocument(itemReq);

                OCSUtils.throwExceptionIfInvalidResult(result);

                retValue = true;
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.RemoveDocument", ex);
            }

            return retValue;
        }

        /// <summary>
        /// Save delle modifiche apportate al documento
        ///
        /// PreCondizioni:
        ///     Le modifiche sul documento devono essere già state apportate con successo in DocsPa 
        /// 
        /// PostCondizioni:
        ///     I relativi oggetti in OCS che rappresentano la scheda documento devono essere aggiornati
        ///     in maniera coerente
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ufficioReferenteEnabled">
        /// Gestione ufficio referente, non di pertinenza per OCS
        /// </param>
        /// <param name="ufficioReferenteSaved">
        /// Gestione ufficio referente, non di pertinenza per OCS
        /// </param>
        /// <returns></returns>
        public bool SalvaDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento, bool ufficioReferenteEnabled, out bool ufficioReferenteSaved)
        {
            bool retValue = false;
            ufficioReferenteSaved = false;

            try
            {
                //bisogna reperire l'id del documento su OCS
                long idDoc = OCSDocumentHelper.getDocumentIdOCS(schedaDocumento.docNumber, null, OCSUtils.getApplicationUserCredentials());

                //bisogna preparare l'oggetto ModifyDocument di OCS
                CorteContentServices.DocumentModifyRequestType modifyReq = new CorteContentServices.DocumentModifyRequestType();
                CorteContentServices.ResultType result;
                modifyReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                modifyReq.document = new CorteContentServices.DocumentModifyType();
                modifyReq.document.documentId = idDoc;
                //NB: anche se non necessario devo valorizzare l'oggetto info DA ELIMINARE AL PIU' PRESTO
                modifyReq.document.info = new CorteContentServices.InfoType();
                modifyReq.document.info.location = DocsPaServices.DocsPaQueryHelper.getDocumentPath(schedaDocumento.docNumber);
                modifyReq.document.info.name = DocsPaServices.DocsPaQueryHelper.getDocumentName(schedaDocumento.docNumber, null);
                modifyReq.document.info.state = DocsPaDocumentale_OCS.CorteContentServices.StateType.Unlocked;

                modifyReq.document.categories = OCSDocumentHelper.getDocumentProperties(schedaDocumento, this.InfoUtente);
                result = WsDocumentInstance.ModifyDocument(modifyReq);

                OCSUtils.throwExceptionIfInvalidResult(result);

                retValue = true;
                logger.Debug(string.Format("OCS.SalvaDocumento: salvato documento con docnumber {0}", schedaDocumento.docNumber));
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.SalvaDocumento", ex);
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="sede"></param>
        /// <param name="risultatoProtocollazione"></param>
        /// <returns></returns>
        public bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione)
        {
            DocsPaVO.utente.Ruolo[] ruoliSuperiori;
            return this.CreateProtocollo(schedaDocumento, ruolo, out risultatoProtocollazione, out ruoliSuperiori);
        }

        /// <summary>
        /// Operazione per la creazione di un nuovo protocollo
        /// 
        /// PreCondizioni:
        ///     - Oggetto SchedaDocumento valido, completo
        ///       degli attributi e degli oggetti significativi per il protocollo 
        ///       (es. numero, dataProtocollo, segnatura, oggetto "Protocollo")
        ///     - Il documento deve essere già protocollato
        /// PostCondizioni:
        ///     - Operazioni di aggiornamento dati documento in "OCS" 
        ///       effettuate con successo
        /// 
        /// </summary>
        /// <param name="schedaDocumento">Oggetto contenente i metadati del documento da protocollare</param>
        /// <param name="ruolo">Ruolo dell'utente che protocolla il documento</param>
        /// <param name="risultatoProtocollazione">Parametro output: Esito più dettagliato dell'operazione</param>
        /// <returns>Boolean, l'operazione di protocollazione è andata a buon fine o meno</returns>
        public bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string conCopia = null)
        {
            // Scheda documento in input (attributi e oggetti valorizzati):
            //      schedaDocumento.oggetto     --> obbligatorio
            //      schedaDocumento.tipoProto   --> obbligatorio
            //          tipologia del documento, può essere:
            //                  - A: Protocollo in arrivo
            //                  - P: Protocollo in uscita
            //                  - I: Protocollo interno
            //              Negli ultimi 3 casi, l'istanza dell'attributo "Protocollo"
            //              di SchedaDocumento può essere:
            //                  - ProtocolloEntrata
            //                  - ProtocolloUscita
            //                  - ProtocolloInterno
            //              In caso di documento grigio, l'oggetto "Protocollo" non è sicuramente istanziato
            //      schedaDocumento.protocollatore  --> obbligatorio
            //          utente che protocolla il documento (non necessariamente chi lo ha creato)
            //          in breve contiene la user id e il ruolo
            //      schedaDocumento.protocollo --> obbligatorio
            //          fornisce i dati del protocollo creato
            //      schedaDocumento.registro --> obbligatori
            //          registro di protocollo
            //
            // Scheda documento in output (attributi e oggetti valorizzati):
            //      nessuna modifica

            ruoliSuperiori = null;
            bool retValue = false;

            // Controllo sulla tipologia di documento (non può essere grigio)
            if (schedaDocumento.tipoProto == "G")
            {
                risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;
            }
            else if (schedaDocumento.registro == null)
            {
                risultatoProtocollazione = ResultProtocollazione.REGISTRO_MANCANTE;
            }
            else if (schedaDocumento.registro.stato == "C")
            {
                risultatoProtocollazione = ResultProtocollazione.REGISTRO_CHIUSO;
            }
            else
            {
                retValue = this.CreateDocumentoGrigio(schedaDocumento, ruolo);

                if (retValue)
                    risultatoProtocollazione = ResultProtocollazione.OK;
                else
                    risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;
            }

            return retValue;
        }

        /// <summary>
        /// Annullamento di un protocollo
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="protocolloAnnullato"></param>
        /// <returns></returns>
        public bool AnnullaProtocollo(ref DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.documento.ProtocolloAnnullato protocolloAnnullato)
        {
            bool retValue = false;

            try
            {
                bool uffRefSaved;
                retValue = this.SalvaDocumento(schedaDocumento, false, out uffRefSaved);

                if (retValue)
                    logger.Debug(string.Format("OCS.AnnullaProtocollo: annullato protocollo con segnatura {0}", schedaDocumento.protocollo.segnatura));
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.AnnullaProtocollo", ex);
            }

            return retValue;
        }

        /// <summary>
        /// Predisposizione di un documento alla protocollazione
        /// 
        /// PreCondizioni:
        ///     Il documento deve essere già stato predisposto alla protocollazione
        ///     nelle entità di DocsPa
        /// 
        /// PostCondizioni:
        ///     In OCS, il documento deve risultare predisposto alla protocollazione
        ///     Nessun attributo deve essere modificato nella SchedaDocumento
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public bool PredisponiProtocollazione(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            bool retValue = false;

            try
            {
                //CONTROLLARE
                bool uffRefSaved;
                retValue = this.SalvaDocumento(schedaDocumento, false, out uffRefSaved);

                //sposta il documento nel path dei documenti protocollati
                if (retValue)
                {
                    retValue = this.SpostaDocumento(schedaDocumento);

                    //bisogna tornare indietro e cancellare il documento
                    //TODO:
                }
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.ProtocollaDocumentoPredisposto:", ex);
            }

            return retValue;
        }

        /// <summary>
        /// Protocollazione di un documento predisposto alla protocollazione
        /// 
        /// PreCondizioni:
        ///     Il documento deve essere già stato protocollato in DocsPa
        /// 
        /// PostCondizioni:
        ///     In OCS, il documento deve risultare come protocollato
        ///     Nessun oggetto fornito in ingresso deve essere modificato
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="risultatoProtocollazione"></param>
        /// <returns></returns>
        public bool ProtocollaDocumentoPredisposto(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione)
        {
            bool retValue = false;
            risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;

            try
            {
                bool uffRefSaved;
                retValue = this.SalvaDocumento(schedaDocumento, false, out uffRefSaved);

                
                if (retValue)
                {
                    //sposta il documento e tutti i suoi eventuali allegati nel path dei documenti protocollati
                    retValue = this.SpostaDocumento(schedaDocumento);
                }

                if (retValue)
                    risultatoProtocollazione = ResultProtocollazione.OK;
                else
                    risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;

            }
            catch (Exception ex)
            {
                retValue = false;
                risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;

                logger.Debug("Errore in OCS.ProtocollaDocumentoPredisposto", ex);
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public bool SpostaDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            bool retValue = false;

            try
            {
                //bisogna reperire l'id del documento su OCS
                long idDoc = OCSDocumentHelper.getDocumentIdOCS(schedaDocumento.docNumber, null, OCSUtils.getApplicationUserCredentials());
                //controllo che la nuova location sia diversa dalla vecchia 
                //(potrebbe accadere nel caso di documenti che da predisposti diventano protocollati ma il path non prevede il registro)
                string location = DocsPaServices.DocsPaQueryHelper.getDocumentPath(schedaDocumento.docNumber).ToUpper();
                string newLocation = OCSServices.OCSDocumentHelper.getDocumentLocation(schedaDocumento, this.InfoUtente, false).ToUpper();

                if (newLocation.Equals(location)) //non c'è bisogno che sposto il documento
                    return true;
                //bisogna preparare l'oggetto MoveDocument di OCS
                CorteContentServices.ItemFolderRequestType itemReq = new CorteContentServices.ItemFolderRequestType();
                CorteContentServices.ResultType result;
                itemReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                itemReq.itemId = idDoc;
                itemReq.folder = new CorteContentServices.FolderType();
                itemReq.folder.folderId = OCSServices.OCSFolderHelper.getFolderIdByName(newLocation, _infoUtente, true);

                if (itemReq.folder.folderId.Equals(-1))
                {
                    return false;
                }
                result = this.WsDocumentInstance.MoveDocument(itemReq);
                OCSUtils.throwExceptionIfInvalidResult(result);

                //NB: bisogna gestire anche lo spostamento degli allegati
                CorteContentServices.DocumentAttachListResponseType listAttach;
                CorteContentServices.ItemIdRequestType itemReqAll = new CorteContentServices.ItemIdRequestType();
                itemReqAll.itemId = idDoc;
                itemReqAll.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();

                DocsPaServices.DocsPaQueryHelper.updateLocation(
                OCSServices.OCSDocumentHelper.getDocumentLocation(schedaDocumento, this.InfoUtente, false),
                schedaDocumento.docNumber);

                listAttach = WsDocumentInstance.GetDocumentAttachments(itemReqAll);

                OCSUtils.throwExceptionIfInvalidResult(listAttach.result);
                foreach (DocumentAttachType attatch in listAttach.attachments)
                {
                    //sposto tutti gli allegati
                    retValue = this.SpostaDocumentoInOCS(attatch.attachId, itemReq.folder.folderId);

                    if (!retValue)
                        break;

                    //ATTENZIONE GESTIRE BENE GLI SPOSTAMENTI ANDATI MALE ED I ROOLBACK
                }


                retValue = true;

                logger.Debug(string.Format("OCS.SpostaDocumento: spostato documento con docNumber {0}", schedaDocumento.docNumber));
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.SpostaDocumento", ex);
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idOCS"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        private bool SpostaDocumentoInOCS(long idOCS, long folderId)
        {
            CorteContentServices.ItemFolderRequestType itemFolderReq = new CorteContentServices.ItemFolderRequestType();
            itemFolderReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
            itemFolderReq.itemId = idOCS;
            itemFolderReq.folder = new CorteContentServices.FolderType();
            itemFolderReq.folder.folderId = folderId;

            CorteContentServices.ResultType result = this.WsDocumentInstance.MoveDocument(itemFolderReq);

            return OCSUtils.isValidServiceResult(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <param name="isNewDoc"></param>
        /// <returns></returns>
        protected virtual CorteContentServices.InfoType setDocumentInfo(SchedaDocumento schedaDoc, bool isNewDoc)
        {
            CorteContentServices.InfoType infoDoc = new CorteContentServices.InfoType();

            if (isNewDoc)
            {
                infoDoc.name = schedaDoc.docNumber;
                infoDoc.location = OCSServices.OCSDocumentHelper.getDocumentLocation(schedaDoc, this.InfoUtente, isNewDoc);
                if (infoDoc.location == null)
                    return null;
                infoDoc.state = CorteContentServices.StateType.Unlocked;
                if (schedaDoc.autore != null)
                    infoDoc.author = schedaDoc.autore;
                else if (schedaDoc.creatoreDocumento != null && schedaDoc.creatoreDocumento.idPeople != null)
                    infoDoc.author = DocsPaServices.DocsPaQueryHelper.getCodiceUtente(schedaDoc.creatoreDocumento.idPeople);

                if (schedaDoc.dataCreazione != null)
                    infoDoc.creationDate = OCSServices.OCSUtils.getOCSDateFormat(schedaDoc.dataCreazione);
            }
            else
            {
                //devo impostare qualcosa se si tratta di un documento esistente? CONTROLLARE!!!
            }

            return infoDoc;
        }

        /// <summary>
        /// Modifica del nome del documento
        /// </summary>
        /// <param name="idOCS"></param>
        /// <param name="newName"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        protected virtual bool modifyDocumentName(long idOCS, string newName, string docNumber)
        {
            bool retValue = false;

            try
            {
                CorteContentServices.InfoType infoDoc = new CorteContentServices.InfoType();
                CorteContentServices.DocumentModifyRequestType modDocReq = new CorteContentServices.DocumentModifyRequestType();

                modDocReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                modDocReq.document = new CorteContentServices.DocumentModifyType();
                modDocReq.document.documentId = idOCS;
                modDocReq.document.info = new CorteContentServices.InfoType();
                modDocReq.document.info.name = newName;
                //devo istanziare l'oggetto location altrimenti si verifica un errore di parser xml
                modDocReq.document.info.location = DocsPaServices.DocsPaQueryHelper.getDocumentPath(docNumber); ;

                CorteContentServices.ResultType result = this.WsDocumentInstance.ModifyDocument(modDocReq);

                OCSUtils.throwExceptionIfInvalidResult(result);

                retValue = true;
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS. ModifyDocumentName", ex);
            }

            return retValue;
        }

        #endregion

        #region Gestione documenti in cestino

        /// <summary>
        /// Inserimento di un documento nel cestino
        /// 
        /// </summary>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        public bool AddDocumentoInCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            bool retValue = false;

            try
            {
                if (infoDocumento.allegato)
                {
                    // Nel caso il documento da rimuovere sia un allegato, viene rimosso definitivamente
                    retValue = this.RemoveAttatchment(infoDocumento.docNumber);
                }
                else
                {
                    // spostare  il documento ed i suoi allegati in un folder particolare che chiamiamo cestino
                    // eliminare la visibilità a tutti gli utenti tranne al superAdmin 
                    long idDoc = OCSDocumentHelper.getDocumentIdOCS(infoDocumento.docNumber, null, OCSUtils.getApplicationUserCredentials());

                    string pathCestino = OCSConfigurations.GetPathCestino();
                    long idFolder = OCSServices.OCSFolderHelper.getFolderIdByName(pathCestino, _infoUtente, false);
                    if (idFolder < 0)
                        return false;

                    CorteContentServices.ItemIdRequestType itemReq = new CorteContentServices.ItemIdRequestType();
                    itemReq.itemId = idDoc;
                    itemReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();

                    CorteContentServices.DocumentAttachListResponseType listAttach = this.WsDocumentInstance.GetDocumentAttachments(itemReq);

                    OCSUtils.throwExceptionIfInvalidResult(listAttach.result);

                    //sposto il documento principale e gli allegati
                    retValue = this.SpostaDocumentoInOCS(idDoc, idFolder);

                    //devo eliminare la visibilità:
                    retValue = OCSDocumentHelper.removeAclDocument(infoDocumento.idProfile, idDoc, this.InfoUtente, OCSUtils.getApplicationUserCredentials());

                    if (retValue)
                    {
                        foreach (DocumentAttachType attatch in listAttach.attachments)
                        {
                            //sposto tutti gli allegati
                            retValue = this.SpostaDocumentoInOCS(attatch.attachId, idFolder);

                            if (!retValue)
                                break;
                            //ATTENZIONE GESTIRE BENE GLI SPOSTAMENTI ANDATI MALE ED I ROOLBACK
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.AddDocumentoInCestino", ex);
            }

            return retValue;
        }

        /// <summary>
        /// Ripristino del documento dal cestino
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <returns></returns>
        public bool RestoreDocumentoDaCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            bool retValue = false;

            try
            {
                string pathCestino = OCSConfigurations.GetPathCestino();
                //OLD long idDoc = OCSServices.OCSUtils.getDocumentIdByCestino(infoDocumento.docNumber, null);
                long idDoc = OCSDocumentHelper.getDocumentIdOCSByPath(infoDocumento.docNumber, null, pathCestino, null, OCSUtils.getApplicationUserCredentials());
                string pathDocumento = DocsPaServices.DocsPaQueryHelper.getDocumentPath(infoDocumento.docNumber);
                long idFolder = OCSServices.OCSFolderHelper.getFolderIdByName(pathDocumento, _infoUtente, false);
                if (idFolder < 0)
                    return false;
                CorteContentServices.DocumentAttachListResponseType listAttach;
                CorteContentServices.ItemIdRequestType itemReq = new CorteContentServices.ItemIdRequestType();
                itemReq.itemId = idDoc;
                itemReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();

                listAttach = WsDocumentInstance.GetDocumentAttachments(itemReq);

                OCSUtils.throwExceptionIfInvalidResult(listAttach.result);

                //sposto il documento principale e gli allegati
                retValue = this.SpostaDocumentoInOCS(idDoc, idFolder);

                //ripristino la visibilità
                retValue = OCSDocumentHelper.setAclDocument(infoDocumento.idProfile, idDoc, this.InfoUtente, OCSUtils.getApplicationUserCredentials());

                if (retValue)
                {
                    foreach (DocumentAttachType attatch in listAttach.attachments)
                    {
                        //sposto tutti gli allegati
                        retValue = this.SpostaDocumentoInOCS(attatch.attachId, idFolder);

                        if (!retValue)
                            break;

                        //ATTENZIONE GESTIRE BENE GLI SPOSTAMENTI ANDATI MALE ED I ROOLBACK
                    }
                }
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.RestoreDocumentoDaCestino", ex);
            }

            return retValue;
        }

        #endregion

        #region Gestione file

        /// <summary>
        /// Operazione per l'inserimento di un file in una versione / allegato di un documento
        /// 
        /// PreCondizione:
        ///     la versione / allegato deve essere già stato creato come entità persistente
        /// 
        ///     Attributi dell'oggetto FileRequest già valorizzati in ingresso
        ///         VersionId:      ID della versione / allegato in docspa
        ///         Version:        0 se allegato, > 0 se versione
        ///         SubVersion:     ! se allegato, stringa vuota se versione ????
        ///         VersionLabel:   A# se allegato (dove # è il num. dell'allegato, fino a 99)
        ///                         (es. A01, A02, A03, ecc.)
        ///                         1, 2, 3, 4, ecc. se versione
        /// PostCondizioni:
        ///     il file deve essere stato associato alla versione / allegato
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="fileDocumento"></param>
        /// <param name="estensione"></param>
        /// <param name="objSicurezza"></param>
        /// <returns></returns>
        public bool PutFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocumento, string estensione)
        {
            bool retValue = false;

            try
            {
                // Creazione nome file
                //NB: a differenza degli altri documentali il nome del file coincide in tutte le versioni 
                //    quindi usiamo il docNumber invece del version_id
                string fileName = string.Format("{0}.{1}", fileRequest.docNumber, estensione);

                // Bisogna reperire l'id della versione OCS, conosciamo docNumber e version_id
                //  fileRequest.docNumber
                //  fileRequest.versionId
                long versionIdOCS = OCSServices.OCSDocumentHelper.getVersionIdOCS(fileRequest.docNumber, fileRequest.versionId);

                // Scrittura file
                CorteContentServices.ItemUploadRequestType itemUpload = new CorteContentServices.ItemUploadRequestType();
                CorteContentServices.ResultType result;
                itemUpload.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                itemUpload.physicalData = new CorteContentServices.ItemUploadType();
                itemUpload.physicalData.itemId = versionIdOCS;
                itemUpload.physicalData.physicalData = new CorteContentServices.UploadPhysicalDataType();
                itemUpload.physicalData.physicalData.fileName = fileName;
                itemUpload.physicalData.physicalData.content = fileDocumento.content;

                result = this.WsVersionInstance.UploadVersion(itemUpload);

                OCSUtils.throwExceptionIfInvalidResult(result);

                retValue = true;

                // La modifica del nome del documento viene effettuata solamente se
                // si sta facendo l'upload di un file per l'ultima versione
                if (DocsPaServices.DocsPaQueryHelper.getDocumentLatestVersionId(fileRequest.docNumber).Equals(fileRequest.versionId))
                {
                    long idOCS = OCSDocumentHelper.getDocumentIdOCSByPath(fileRequest.docNumber, fileRequest.versionId, fileRequest.path, fileRequest.docNumber, OCSUtils.getApplicationUserCredentials());

                    retValue = this.modifyDocumentName(idOCS, fileName, fileRequest.docNumber);
                }

                if (retValue)
                    logger.Debug(string.Format("Operazione PutFile in OCS eseguita con successo"));
                else
                    logger.Debug(string.Format("Errore in OCS.PutFile - modifyName"));
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.PutFile", ex);
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento (semplificato) del contenuto di un file associato ad una versione / allegato.
        /// 
        /// PreCondizioni:
        ///     La versione / allegato deve essere già stato creata come entità persistente
        ///     Il file deve essere già acquisito nel repository documentale
        /// 
        /// PostCondizioni:
        ///     Restituzione del solo contenuto del file dal documentale OCS
        /// 
        /// </summary>
        /// <remarks>Se almeno uno dei parametri forniti in ingresso non è valido, viene restituito null</remarks>
        /// <param name="docNumber"></param>
        /// <param name="version"></param>
        /// <param name="versionId"></param>
        /// <param name="versionLabel"></param>
        /// <returns></returns>
        public byte[] GetFile(string docNumber, string version, string versionId, string versionLabel)
        {
            // Verifica se il file è di tipo stampa registro
            bool isStampaRegistro = false; //TODO
            return this.GetFile(docNumber, version, versionId, versionLabel, isStampaRegistro);
        }

        /// <summary>
        /// Operazione per il reperimento di un file di una versione / allegato di un documento
        /// 
        /// PreCondizioni:
        ///     la versione / allegato deve essere già stato creato come entità persistente
        ///     il file deve essere già acquisito nel repository documentale
        /// 
        /// PostCondizioni:
        ///     i seguenti attributi dell'oggetto "FileDocumento" devono essere inizializzati:
        ///     - name              (nome "fittizio" del file con estensione)
        ///     - estensioneFile    (estensione del file)
        ///     - content           (array di byte, contenuto del file)
        ///     - contentType       (mimetype del file)
        ///     - length            (dimensione dell'array di byte)
        /// </summary>
        /// <param name="fileDocumento"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public bool GetFile(ref DocsPaVO.documento.FileDocumento fileDocumento, ref DocsPaVO.documento.FileRequest fileRequest)
        {
            bool retValue = false;

            try
            {
                // Verifica se il file da reperire è relativo ad una stampa registro
                bool isStampaRegistro = (fileRequest.descrizione == DocsPaVO.documento.Documento.STAMPA_REGISTRO);

                // Reperimento contenuto del file
                fileDocumento.content = this.GetFile(fileRequest.docNumber, fileRequest.version, fileRequest.versionId, fileRequest.versionLabel, isStampaRegistro);

                retValue = (fileDocumento.content != null);

                if (retValue)
                {
                    fileDocumento.name = fileRequest.fileName;
                    fileDocumento.length = fileDocumento.content.Length;

                    logger.Debug(string.Format("OCS.GetFile: reperimento file di dimensione {0} per documento con docnumber {0} e versionid {0}", fileDocumento.content.Length.ToString(), fileRequest.versionId, fileRequest.docNumber, fileRequest.versionId));
                }
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in OCS.GetFile:\n{0}", ex.ToString()));
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento (semplificato) del contenuto di un file associato ad una versione / allegato.
        /// 
        /// PreCondizioni:
        ///     La versione / allegato deve essere già stato creata come entità persistente
        ///     Il file deve essere già acquisito nel repository documentale
        /// 
        /// PostCondizioni:
        ///     Restituzione del solo contenuto del file dal documentale OCS
        /// 
        /// </summary>
        /// <remarks>Se almeno uno dei parametri forniti in ingresso non è valido, viene restituito null</remarks>
        /// <param name="docNumber"></param>
        /// <param name="version"></param>
        /// <param name="versionId"></param>
        /// <param name="versionLabel"></param>
        /// <param name="isStampaRegistro"></param>
        /// <returns></returns>
        protected virtual byte[] GetFile(string docNumber, string version, string versionId, string versionLabel, bool isStampaRegistro)
        {
            byte[] contentByteArray = null;

            try
            {
                // devo capire di quale versione si tratta
                // recuperare il numero di versione e poi il content ad essa relativo
                long idOCS = OCSServices.OCSDocumentHelper.getVersionIdOCS(docNumber, versionId);

                CorteContentServices.ItemIdRequestType itemRequest = new CorteContentServices.ItemIdRequestType();
                CorteContentServices.PhysicalDataResponseType physicalDataResponse;
                itemRequest.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                itemRequest.itemId = idOCS;

                physicalDataResponse = this.WsVersionInstance.DownloadVersion(itemRequest);

                OCSUtils.throwExceptionIfInvalidResult(physicalDataResponse.result);

                contentByteArray = physicalDataResponse.physicalData.content;
            }
            catch (Exception ex)
            {
                contentByteArray = null;

                logger.Debug("Errore in OCS.GetFile", ex);
            }

            return contentByteArray;
        }

        #endregion

        #region Gestione allegati al documento

        /// <summary>
        /// Inserimento dei soli metadati di un nuovo allegato per un documento
        /// 
        /// PreCondizioni:
        ///     L'entità allegato deve essere già stata aggiunta in DocsPa
        /// 
        /// PostCondizioni:
        ///     I dati utilizzati da OCS devono essere stati salvati
        /// 
        /// </summary>
        /// <param name="allegato"></param>
        /// <param name="putfile"></param>
        /// <returns></returns>
        public bool AddAttachment(DocsPaVO.documento.Allegato allegato, string putfile)
        {
            bool retValue = false;

            try
            {
                if (!string.IsNullOrEmpty(putfile) && putfile == "Y")
                {
                    // Se putfile == "Y", l'operazione si presume
                    // come andata a buon fine in quanto nessun
                    // attributo è da modificare in OCS
                    retValue = true;
                }
                else
                {
                    // Reperimento del docnumber relativo al documento principale che contiene l'allegato
                    string docNumber = DocsPaServices.DocsPaQueryHelper.getDocNumberDocumentoPrincipale(allegato.docNumber);
                    long idOCS;
                    //reperimento dell'id OCS
                    idOCS = OCSDocumentHelper.getDocumentIdOCS(docNumber, null, OCSUtils.getApplicationUserCredentials());
                    //creazione dell'oggetto Attachment di OCS
                    //CorteContentServices.AttachCreateRequestType attachRequest = new CorteContentServices.AttachCreateRequestType();
                    CorteContentServices.ItemCreateVersionRequestType attachRequest = new CorteContentServices.ItemCreateVersionRequestType();

                    CorteContentServices.ItemIdResponseType itemResponse;
                    attachRequest.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                    //attachRequest.attach = new CorteContentServices.AttachCreateType();
                    attachRequest.version = new CorteContentServices.ItemCreateVersionType();

                    attachRequest.version.itemId = idOCS;
                    //NB: anche se non necessario devo valorizzare l'oggetto info
                    attachRequest.version.version = new DocsPaDocumentale_OCS.CorteContentServices.CreateVersionType();

                    attachRequest.version.version.location = DocsPaServices.DocsPaQueryHelper.getDocumentPath(docNumber);
                    attachRequest.version.version.name = allegato.docNumber;
                    attachRequest.version.version.author = _infoUtente.userId;
                    attachRequest.version.version.description = "allegato DOC:" + docNumber;
                    attachRequest.version.version.categories = OCSDocumentHelper.getAttachmentProperties(allegato, this.InfoUtente);
                    attachRequest.version.versionId = Convert.ToInt32(allegato.versionId);

                    itemResponse = this.WsAttatchmentInstance.CreateVersionedAttach(attachRequest);

                    OCSUtils.throwExceptionIfInvalidResult(itemResponse.result);

                    //gestione location su DocsPA CONTROLLARE LA FATTIBILITA' del campo PATH
                    DocsPaServices.DocsPaQueryHelper.updateLocation(attachRequest.version.version.location, allegato.docNumber);

                    retValue = true;
                }
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.AddAttachment", ex);
            }

            return retValue;
        }

        /// <summary>
        /// Modifica dei dati di un allegato
        /// 
        /// PreCondizioni:
        ///     L'entità allegato deve essere già stata modificata in DocsPa 
        /// 
        /// PostCondizioni:
        ///     I dati utilizzati da OCS devono essere stati salvati
        /// </summary>
        /// <param name="allegato"></param>
        public void ModifyAttatchment(DocsPaVO.documento.Allegato allegato)
        {
            try
            {
                //Reperire l'id OCS dell'attachment
                long idOCS = OCSDocumentHelper.getAttachIdOCS(allegato.docNumber, null, OCSUtils.getApplicationUserCredentials());

                //Modificare i metadati dell'attachment
                CorteContentServices.AttachModifyRequestType attachModify = new CorteContentServices.AttachModifyRequestType();

                attachModify.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                attachModify.attach = new CorteContentServices.AttachModifyType();
                attachModify.attach.attachId = idOCS;
                //NB: inserisco l'oggetto info perchè nell'xsd è obbligatorio, ma dovrebbe essere corretto!!!
                attachModify.attach.info = new DocsPaDocumentale_OCS.CorteContentServices.InfoType();
                attachModify.attach.info.name = DocsPaServices.DocsPaQueryHelper.getDocumentName(allegato.docNumber, null);
                attachModify.attach.info.location = DocsPaServices.DocsPaQueryHelper.getDocumentPath(allegato.docNumber);

                attachModify.attach.categories = OCSDocumentHelper.getAttachmentProperties(allegato, this.InfoUtente);

                CorteContentServices.ResultType result = this.WsAttatchmentInstance.ModifyAttach(attachModify);

                OCSUtils.throwExceptionIfInvalidResult(result);

                logger.Debug(string.Format("OCS.modify: modificata versione docnumber: {0}", allegato.docNumber));
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in OCS.ModifyAttatchment", ex);

                throw new ApplicationException("Errore in OCS.ModifyAttatchment", ex);
            }
        }

        /// <summary>
        /// Rimozione di un allegato
        /// </summary>
        /// <param name="allegato"></param>
        /// <returns></returns>
        public bool RemoveAttatchment(DocsPaVO.documento.Allegato allegato)
        {
            return this.RemoveAttatchment(allegato.docNumber);
        }

        /// <summary>
        /// Rimozione dell'allegato
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        protected virtual bool RemoveAttatchment(string docNumber)
        {
            bool retValue = false;

            try
            {
                //Reperire l'id OCS dell'attachment
                long idDoc = OCSDocumentHelper.getAttachIdOCS(docNumber, null, OCSUtils.getApplicationUserCredentials());

                //cancellazione attachment su OCS
                CorteContentServices.ItemIdRequestType itemReq = new DocsPaDocumentale_OCS.CorteContentServices.ItemIdRequestType();
                CorteContentServices.ResultType result;
                itemReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                itemReq.itemId = idDoc;

                result = this.WsAttatchmentInstance.DeleteAttach(itemReq);

                OCSUtils.throwExceptionIfInvalidResult(result);

                retValue = true;
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.RemoveAttatchment", ex);
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allegato"></param>
        /// <param name="documento"></param>
        /// <returns></returns>
        public bool ScambiaAllegatoDocumento(DocsPaVO.documento.Allegato allegato, DocsPaVO.documento.Documento documento)
        {
            bool retValue = false;

            try
            {

                long idDoc = OCSDocumentHelper.getDocumentIdOCSByPath(documento.docNumber, null, documento.path, documento.fileName, OCSUtils.getApplicationUserCredentials());
                long idAll = OCSDocumentHelper.getDocumentIdOCSByPath(documento.docNumber, null, allegato.path, allegato.fileName, OCSUtils.getApplicationUserCredentials());
                CorteContentServices.ChangeVersionRequestType changeVersionReq = new CorteContentServices.ChangeVersionRequestType();
                CorteContentServices.ResultType result;
                changeVersionReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                changeVersionReq.attachId = idAll;
                changeVersionReq.documentId = idDoc;

                result = this.WsAttatchmentInstance.ChangeVersion(changeVersionReq);

                OCSUtils.throwExceptionIfInvalidResult(result);

                //si devono cambiare i nomi dei documenti nel caso in cui le estensioni dei file fossero diverse
                string estAllegato = allegato.fileName.Substring(allegato.fileName.LastIndexOf(".") + 1);
                string estDocumento = documento.fileName.Substring(documento.fileName.LastIndexOf(".") + 1);
                //devo ricalcolare l'id altrimenti mi rimangono i vecchi id e poi ci sono problemi con i metodi
                idDoc = OCSDocumentHelper.getDocumentIdOCSByPath(documento.docNumber, null, documento.path, documento.fileName, OCSUtils.getApplicationUserCredentials());
                idAll = OCSDocumentHelper.getDocumentIdOCSByPath(documento.docNumber, null, allegato.path, allegato.fileName, OCSUtils.getApplicationUserCredentials());

                if (!estAllegato.Equals(estDocumento))
                {
                    modifyDocumentName(idDoc, documento.docNumber + "." + estAllegato, documento.docNumber);

                    idAll = OCSDocumentHelper.getDocumentIdOCSByPath(documento.docNumber, null, allegato.path, allegato.fileName, OCSUtils.getApplicationUserCredentials());

                    modifyAttachmentName(idAll, allegato.docNumber + "." + estDocumento, allegato.docNumber);
                }

                retValue = true;

                logger.Debug(string.Format("OCS.ScambiaAllegatoDocumento: scambiato allegato con docnumber {0} con documento con docnumber {1}", allegato.docNumber, documento.docNumber));

            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.ScambiaAllegatoDocumento", ex);
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idOCS"></param>
        /// <param name="newName"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        protected virtual bool modifyAttachmentName(long idOCS, string newName, string docNumber)
        {
            bool retValue = false;
            try
            {
                CorteContentServices.InfoType infoDoc = new CorteContentServices.InfoType();
                CorteContentServices.AttachModifyRequestType modDocReq = new CorteContentServices.AttachModifyRequestType();
                CorteContentServices.ResultType result;
                modDocReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                modDocReq.attach = new CorteContentServices.AttachModifyType();
                modDocReq.attach.attachId = idOCS;
                modDocReq.attach.info = new CorteContentServices.InfoType();
                modDocReq.attach.info.name = newName;
                //devo istanziare l'oggetto location altrimenti si verifica un errore di parser xml
                modDocReq.attach.info.location = DocsPaServices.DocsPaQueryHelper.getDocumentPath(docNumber); ;

                result = this.WsAttatchmentInstance.ModifyAttach(modDocReq);

                OCSUtils.throwExceptionIfInvalidResult(result);

                retValue = true;
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS. ModifyDocumentName", ex);
            }
            return retValue;
        }



        #endregion

        #region Gestione versioni

        /// <summary>
        /// Operazione per l'inserimento dei metadati di una nuova versione di un documento
        /// 
        /// PreCondizioni:
        ///     la versione / allegato deve essere già stata inserita come entità in docspa
        ///  
        /// PostCondizioni:
        ///     i metadati della versione / allegato devono essere stati inseriti in OCS
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="daInviare"></param>
        /// <returns></returns>
        public bool AddVersion(DocsPaVO.documento.FileRequest fileRequest, bool daInviare)
        {
            bool retValue = false;

            try
            {
                //NB: questo controllo serve per evitare di creare una nuova versione quando non ce ne è bisogno
                if (!fileRequest.subVersion.Equals("!"))
                    return true;
                else
                {
                    //bisogna reperire l'id del documento su OCS
                    long idDoc = OCSDocumentHelper.getDocumentIdOCS(fileRequest.docNumber, fileRequest.versionId, OCSUtils.getApplicationUserCredentials());

                    //creazione di una nuova versione
                    CorteContentServices.ItemCreateVersionType itemVersion = new CorteContentServices.ItemCreateVersionType();
                    itemVersion.itemId = idDoc;
                    itemVersion.versionId = Int32.Parse(fileRequest.versionId);
                    itemVersion.version = new CorteContentServices.CreateVersionType();
                    itemVersion.version.description = fileRequest.descrizione;
                    //se non imposto questo dato, viene ereditato il content della versione precedente
                    itemVersion.version.physicalData = new DocsPaDocumentale_OCS.CorteContentServices.PhysicalDataType();
                    itemVersion.version.physicalData.content = new byte[0];

                    CorteContentServices.ItemCreateVersionRequestType itemVersionRequest = new CorteContentServices.ItemCreateVersionRequestType();
                    CorteContentServices.ItemIdResponseType itemResponse;
                    itemVersionRequest.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                    itemVersionRequest.version = itemVersion;

                    itemResponse = this.WsDocumentInstance.CreateNewVersionDocument(itemVersionRequest);

                    OCSUtils.throwExceptionIfInvalidResult(itemResponse.result);

                    //NB: se non è la prima versione del documento, su OCS bisogna modificare il nome del documento
                    // perchè la nuova versione non ha estensione, mentre quella precedente potrebbe avercela
                    //l'idDoc cambia, è quello della versione appena creata
                    retValue = this.modifyDocumentName(itemResponse.itemId, fileRequest.docNumber, fileRequest.docNumber);
                }
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.AddVersion", ex);
            }

            return retValue;
        }

        /// <summary>
        /// Modifica dei metadati di una versione
        /// </summary>
        /// <param name="fileReq"></param>
        public void ModifyVersion(DocsPaVO.documento.FileRequest fileReq)
        {

            try
            {
                //si deve trovare l'idOCS della versione (coincide con il versionId di DocsPA)
                long idOCS = OCSServices.OCSDocumentHelper.getVersionIdOCS(fileReq.docNumber, fileReq.versionId);

                //modifica versione su OCS
                CorteContentServices.VersionModifyRequestType modVersionReq = new DocsPaDocumentale_OCS.CorteContentServices.VersionModifyRequestType();
                CorteContentServices.ResultType result;
                modVersionReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                modVersionReq.version = new DocsPaDocumentale_OCS.CorteContentServices.VersionModifyType();
                modVersionReq.version.versionId = idOCS;
                modVersionReq.version.description = fileReq.descrizione;

                result = this.WsVersionInstance.ModifyVersion(modVersionReq);

                OCSUtils.throwExceptionIfInvalidResult(result);

                logger.Debug(string.Format("OCS.modify: modificata versione con versionId {0}", fileReq.versionId));
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in OCS.ModifyVersion", ex);

                throw ex;
            }
        }

        /// <summary>
        /// Setta a 1 cha_segnatura se la versione contiene un documento in formato pdf, con segnatura impressa
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns>
        /// bool che indica l'esito dell'operazione di update
        /// </returns>
        public bool ModifyVersionSegnatura(string versionId)
        {
            throw new NotImplementedException("Operazione 'UpdateSegnatura' non implementata in OCS");
        }

        /// <summary>
        /// Informa se la versione ha associato un file con impressa la segnatura
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns>
        /// bool che indica se la versione ha associato un file con impressa segnatura o meno
        /// </returns>
        public bool IsVersionWithSegnature(string versionId)
        {
            throw new NotImplementedException("Operazione 'IsVersionWithSegnature' non implementata in OCS");
        }

        /// <summary>
        /// Operazione per la rimozione dei metadati di una versione / allegato di un documento
        /// 
        /// PreCondizioni:
        ///     la versione / allegato deve essere già stata rimossa come entità di un documento
        ///     
        /// PostCondizioni:
        ///     i metadati della versione / allegato devono essere rimossi da OCS
        /// 
        /// <remarks>
        ///     In caso di rimozione di una versione, può essere rimossa solamente la versione corrente
        /// </remarks>
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public bool RemoveVersion(DocsPaVO.documento.FileRequest fileRequest)
        {
            bool retValue = false;

            try
            {
                //DA VERIFICARE SE L'ID E' QUELLO GIUSTO:
                //si deve trovare l'idOCS della versione
                long idOCS = OCSServices.OCSDocumentHelper.getVersionIdOCS(fileRequest.docNumber, fileRequest.versionId);

                //cancellazione versione su OCS
                CorteContentServices.ItemIdRequestType itemReq = new DocsPaDocumentale_OCS.CorteContentServices.ItemIdRequestType();
                CorteContentServices.ResultType result;
                itemReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                itemReq.itemId = idOCS;

                result = this.WsVersionInstance.DeleteVersion(itemReq);

                OCSUtils.throwExceptionIfInvalidResult(result);

                logger.Debug(string.Format("OCS.Remove: rimossa versione con versionId {0}", fileRequest.versionId));

                retValue = true;
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.RemoveVersion", ex);
            }

            return retValue;
        }

        #endregion

        #region Gestione ACL documento

        /// <summary>
        /// Impostazione della visibilità su un documento 
        /// (e dell'ownership, nel caso l'utente / ruolo rimosso fosse il proprietario)
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        public bool AddPermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            bool added = false;

            // se si revocano i diritti all'utente proprietario, 
            // si revocano anche al ruolo proprietario e viceversa. 
            // Il proprietario del documento diventa l'utente e il ruolo del revocante

            try
            {
                //TODO:
                //devono essere reimpostate le AclEventListener su OCSServices
                string idProfile = infoDiritto.idObj;

                UserCredentialsType credentials = OCSUtils.getApplicationUserCredentials();

                long idDoc = OCSDocumentHelper.getDocumentIdOCS(idProfile, null, credentials);

                bool result = OCSDocumentHelper.setAclDocument(idProfile, idDoc, this.InfoUtente, credentials);

                if (result)
                    added = true;
            }
            catch (Exception ex)
            {
                added = false;

                logger.Debug("Errore in OCS.RemovePermission", ex);
            }

            return added;
        }

        /// <summary>
        /// Revoca della visibilità su un documento (e dell'ownership, nel caso l'utente / ruolo rimosso è proprietario)
        /// </summary>
        /// <param name="documentInfo"></param>
        /// <returns></returns>
        public bool RemovePermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            bool removed = false;

            try
            {
                //TODO:
                //devono essere reimpostate le AclEventListener su OCSServices
                string idProfile = infoDiritto.idObj;

                UserCredentialsType credentials = OCSUtils.getApplicationUserCredentials();

                long idDoc = OCSDocumentHelper.getDocumentIdOCS(idProfile, null, credentials);

                bool result = OCSDocumentHelper.setAclDocument(idProfile, idDoc, this.InfoUtente, credentials);

                if (result)
                    removed = true;
            }
            catch (Exception ex)
            {
                removed = false;

                logger.Debug("Errore in OCS.RemovePermission", ex);
            }

            return removed;
        }

        /// <summary>
        /// Metodo per l'assegnazione di un diritto di tipo A ad un ruolo
        /// </summary>
        /// <param name="rights">Informazioni sul diritto da assegnare</param>
        /// <returns>True se è andato bene</returns>
        public bool AddPermissionToRole(DirittoOggetto rights)
        {
            return this.AddPermission(rights);
        }
        
        /// <summary>
        /// Aggiorna le ACL del documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        public virtual void RefreshAclDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            // Do nothing
        }

        #endregion

        #region OCS factory methods

        /// <summary>
        /// Reperimento istanza webservices per la gestione dei documenti
        /// </summary>
        protected DocumentManagementSOAPHTTPBinding WsDocumentInstance
        {
            get
            {
                if (this._wsDocument == null)
                    this._wsDocument = OCSServiceFactory.GetDocumentServiceInstance<DocumentManagementSOAPHTTPBinding>();
                return this._wsDocument;
            }
        }

        /// <summary>
        /// Reperimento istanza webservices per la gestione delle versioni dei documenti
        /// </summary>
        protected VersionManagementSOAPHTTPBinding WsVersionInstance
        {
            get
            {
                if (this._wsVersion == null)
                    this._wsVersion = OCSServiceFactory.GetDocumentServiceInstance<VersionManagementSOAPHTTPBinding>();
                return this._wsVersion;
            }
        }

        /// <summary>
        /// Reperimento istanza webservices per la gestione delle versioni dei documenti
        /// </summary>
        protected AttachmentManagementSOAPHTTPBinding WsAttatchmentInstance
        {
            get
            {
                if (this._wsAttachment == null)
                    this._wsAttachment = OCSServiceFactory.GetDocumentServiceInstance<AttachmentManagementSOAPHTTPBinding>();
                return this._wsAttachment;
            }
        }


        #endregion

        #region Not implemented methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="docNumber"></param>
        /// <param name="version_id"></param>
        /// <param name="version"></param>
        /// <param name="subVersion"></param>
        /// <param name="versionLabel"></param>
        /// <returns></returns>
        public bool ModifyExtension(ref DocsPaVO.documento.FileRequest fileRequest, string docNumber, string version_id, string version, string subVersion, string versionLabel)
        {
            throw new NotImplementedException("Operazione 'ModifyExtension' non implementata in OCS");
        }

        /// <summary>
        /// Reperimento dell'ultimo id di versione di un documento
        /// 
        /// IMPORTANTE: 
        /// L'operazione può non essere implementata in quanto non è 
        /// OCS che assegna il docNumber ma DocsPa.
        /// In tal caso non verrà richiamata dallo strato CDC.
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public string GetLatestVersionId(string docNumber)
        {
            throw new NotImplementedException("Operazione 'GetLatestVersionId' non implementata in OCS");
        }

        /// <summary>
        /// Reperimento dell'estensione del file associato al documento e alla versione richiesta
        /// 
        /// </summary>
        /// <param name="docnumber"></param>
        /// <param name="versionid"></param>
        /// <returns></returns>
        public string GetFileExtension(string docnumber, string versionid)
        {
            throw new NotImplementedException("Operazione 'GetFileExtension' non implementata in OCS");
        }

        /// <summary>
        /// Reperimento del nome del file presente nel repository del documentale
        /// </summary>
        /// <param name="docnumber"></param>
        /// <param name="versionid"></param>
        /// <returns></returns>
        public string GetOriginalFileName(string docnumber, string versionid)
        {
            throw new NotImplementedException("Operazione 'GetOriginalFileName' non implementata in OCS");
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected InfoUtente InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }
    }
}
