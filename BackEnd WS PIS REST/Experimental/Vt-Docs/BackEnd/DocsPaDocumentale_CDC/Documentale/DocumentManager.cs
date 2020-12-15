using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.documento;
using DocsPaVO.utente;
using System.Data;
using DocsPaDbManagement.Functions;
using DocsPaUtils.Interfaces.DbManagement;

using DocumentManagerETDOCS = DocsPaDocumentale_ETDOCS.Documentale.DocumentManager;
using DocumentManagerOCS = DocsPaDocumentale_OCS.Documentale.DocumentManager;
using log4net;


namespace DocsPaDocumentale_CDC.Documentale
{
    public class DocumentManager : IDocumentManager
    {
        private ILog logger = LogManager.GetLogger(typeof(DocumentManager));
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        private IDocumentManager _documentManagerETDOCS = null;

        /// <summary>
        /// 
        /// </summary>
        private IDocumentManager _documentManagerOCS = null;

        /// <summary>
        /// 
        /// </summary>
        private InfoUtente _infoUtente = null;

        protected DocumentManager()
        { }

        /// <summary>
        /// Inizializza l'istanza della classe acquisendo i dati relativi all'utente 
        /// ed alla libreria per la connessione al documentale.
        /// </summary>
        /// <param name="infoUtente">Dati relativi all'utente</param>
        /// <param name="currentLibrary">Libreria per la connessione al documentale</param>
        public DocumentManager(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;

            this._documentManagerETDOCS = new DocumentManagerETDOCS(infoUtente);
            this._documentManagerOCS = new DocumentManagerOCS(infoUtente);
        }

        #endregion

        #region Public methods

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
        /// Creazione di un nuovo documento per la stampa registro.
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="ruoliSuperiori"></param>
        /// <returns>ID del documento o 'null' se si è verificato un errore</returns>
        public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            bool retValue = false;
            ruoliSuperiori = null;

            try
            {
                retValue = this.DocumentManagerETDOCS.CreateDocumentoStampaRegistro(schedaDocumento, ruolo, out ruoliSuperiori);

                if (retValue)
                    retValue = this.DocumentManagerOCS.CreateDocumentoStampaRegistro(schedaDocumento, ruolo);
            }
            catch (Exception ex)
            {
                retValue = false;

                string errorMessage = string.Format("Errore nella creazione di un documento stampa registro: {0}", ex.Message);
                logger.Debug(errorMessage, ex);
            }

            return retValue;
        }

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
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="ruoliSuperiori"></param>
        /// <returns></returns>
        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string conCopia = null)
        {

            bool retValue = false;
            ruoliSuperiori = null;

            try
            {
                retValue = this.DocumentManagerETDOCS.CreateDocumentoGrigio(schedaDocumento, ruolo, out ruoliSuperiori);

                if (retValue)
                    retValue = this.DocumentManagerOCS.CreateDocumentoGrigio(schedaDocumento, ruolo);               
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nella creazione di un documento grigio: {0}", ex.Message);
                logger.Debug(errorMessage, ex);
                retValue = false;
            }

            return retValue;
        }

        /// <summary>
        /// Predisposizione di un documento alla protocollazione
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public bool PredisponiProtocollazione(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            bool retValue = false;

            try
            {
                retValue = this.DocumentManagerETDOCS.PredisponiProtocollazione(schedaDocumento);

                if (retValue)
                    retValue = this.DocumentManagerOCS.PredisponiProtocollazione(schedaDocumento);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nella predisposizione di un documento alla protocollazione: {0}", ex.Message);
                logger.Debug(errorMessage, ex);
                retValue = false;
            }

            return retValue;
        }

        /// <summary>
        /// Protocollazione di un documento predisposto
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
                retValue = this.DocumentManagerETDOCS.ProtocollaDocumentoPredisposto(schedaDocumento, ruolo, out risultatoProtocollazione);

                if (retValue)
                {
                    retValue = this.DocumentManagerOCS.ProtocollaDocumentoPredisposto(schedaDocumento, ruolo, out risultatoProtocollazione);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nella protocollazione del documento: {0}", ex.Message);
                logger.Debug(errorMessage, ex);

                retValue = false;
                risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;
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
        ///     - Fornire un oggetto SchedaDocumento valido, completo
        ///       degli attributi e degli oggetti significativi per il protocollo 
        ///       (es. numero, dataProtocollo, segnatura, oggetto "Protocollo")
        ///     
        /// PostCondizioni:
        ///     - Operazione di protocollazione documento effettuata con successo
        ///       
        /// 
        /// </summary>
        /// <param name="schedaDocumento">Oggetto contenente i metadati del documento da protocollare</param>
        /// <param name="ruolo">Ruolo dell'utente che protocolla il documento</param>
        /// <param name="risultatoProtocollazione">Parametro output: Esito più dettagliato dell'operazione</param>
        /// <param name="ruoliSuperiori"></param>
        /// <returns>Boolean, l'operazione di protocollazione è andata a buon fine o meno</returns>
        public bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string conCopia = null)
        {
            //Performance.PerformanceLogWriter.CreatePerformanceLogWriter(@"c:\temp\DocumentManager.txt");

            //Performance.PerformanceLogWriter log = new Performance.PerformanceLogWriter();
            //log.StartLogEntry("CreateProtocollo");

            bool retValue = false;
            risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;
            ruoliSuperiori = null;

            try
            {
                //log.StartLogEntry("CreateProtocollo_ETDOCS", true);
                retValue = this.DocumentManagerETDOCS.CreateProtocollo(schedaDocumento, ruolo, out risultatoProtocollazione, out ruoliSuperiori);
                //log.EndLogEntry();

                //log.StartLogEntry("CreateProtocollo_DCTM", true);
                if (retValue)
                    retValue = this.DocumentManagerOCS.CreateProtocollo(schedaDocumento, ruolo, out risultatoProtocollazione);
                //log.EndLogEntry();
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nella protocollazione del documento: {0}", ex.Message);
                logger.Debug(errorMessage, ex);

                retValue = false;
                risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;
            }

            //log.EndLogEntry();
            //Performance.PerformanceLogWriter.FlushPerformanceLogWriter();

            return retValue;
        }

        /// <summary>
        /// Save delle modifiche apportate al documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ufficioReferenteEnabled"></param>
        /// <param name="ufficioReferenteSaved"></param>
        /// <returns></returns>
        public bool SalvaDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento, bool ufficioReferenteEnabled, out bool ufficioReferenteSaved)
        {
            bool retValue = false;
            ufficioReferenteSaved = false;

            try
            {
                retValue = this.DocumentManagerETDOCS.SalvaDocumento(schedaDocumento, ufficioReferenteEnabled, out ufficioReferenteSaved);

                if (retValue)
                    retValue = this.DocumentManagerOCS.SalvaDocumento(schedaDocumento, ufficioReferenteEnabled, out ufficioReferenteSaved);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nel salvataggio dei dati del documento: {0}", ex.Message);
                logger.Debug(errorMessage, ex);
                retValue = false;
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
                retValue = this.DocumentManagerETDOCS.AnnullaProtocollo(ref schedaDocumento, protocolloAnnullato);

                if (retValue) retValue = this.DocumentManagerOCS.AnnullaProtocollo(ref schedaDocumento, protocolloAnnullato); ;
            }
            catch (Exception ex)
            {
                retValue = false;
                string errorMessage = string.Format("Errore nell'operazione 'AnnullaProtocollo': {0}", ex.Message);
                logger.Debug(errorMessage, ex);
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
                // Rimozione dei documenti dal database di docspa
                // NB: Non viene richiamato il metodo corrispondente nel documentale
                // etdocs in quanto quest'ultima elimina anche i file.
                // Per OCS, questo non è necessario in quanto è OCS che gestisce
                // la cancellazione dei file.
                foreach (DocsPaVO.documento.InfoDocumento infoDocumento in items)
                {
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                    retValue = doc.EliminaDoc(this.InfoUtente, infoDocumento);

                    if (!retValue)
                        break;
                }

                if (retValue)
                    retValue = this.DocumentManagerOCS.Remove(items);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nell'operazione 'RemoveDocumento': {0}", ex.Message);
                logger.Debug(errorMessage, ex);

                retValue = false;
            }

            return retValue;
        }

        #region Gestione ACL documento

        /// <summary>
        /// Impostazione di un permesso su un documento
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        public bool AddPermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            bool retValue = this.DocumentManagerETDOCS.AddPermission(infoDiritto);

            if (retValue)
                retValue = this.DocumentManagerOCS.AddPermission(infoDiritto);

            return retValue;
        }

        /// <summary>
        /// Revoca di un permesso su un documento
        /// </summary>
        /// <param name="documentInfo"></param>
        /// <returns></returns>
        public bool RemovePermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            bool retValue = this.DocumentManagerETDOCS.RemovePermission(infoDiritto);

            if (retValue)
                retValue = this.DocumentManagerOCS.RemovePermission(infoDiritto);

            return retValue;
        }

        #endregion

        #region Gestione versioni documento

        /// <summary>
        /// Inserimento di una nuova versione
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="daInviare"></param>
        /// <returns></returns>
        public bool AddVersion(DocsPaVO.documento.FileRequest fileRequest, bool daInviare)
        {
            bool retValue = false;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                retValue = this.DocumentManagerETDOCS.AddVersion(fileRequest, daInviare);

                if (retValue)
                    retValue = this.DocumentManagerOCS.AddVersion(fileRequest, daInviare);
                
                if (retValue)
                    transactionContext.Complete();
            }

            return retValue;
        }

        /// <summary>
        /// Modifica dei metadati di una versione
        /// </summary>
        /// <param name="fileRequest"></param>
        public void ModifyVersion(DocsPaVO.documento.FileRequest fileRequest)
        {
            try
            {   //TODO: gestire la transazionalità dei metodi
                this.DocumentManagerETDOCS.ModifyVersion(fileRequest);

                this.DocumentManagerOCS.ModifyVersion(fileRequest);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nell'operazione 'ModifyVersion': {0}", ex.Message);
                logger.Debug(errorMessage, ex);

                throw ex;
            }
        }

        /// <summary>
        /// Rimozione di una versione
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public bool RemoveVersion(DocsPaVO.documento.FileRequest fileRequest)
        {
            bool retValue = false;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                retValue = this.DocumentManagerETDOCS.RemoveVersion(fileRequest);

                if (retValue)
                    retValue = this.DocumentManagerOCS.RemoveVersion(fileRequest);
                
                if (retValue)
                    transactionContext.Complete();
            }
            
            return retValue;
        }

        #endregion

        #region Gestione allegati

        /// <summary>
        /// Inserimento di un allegato
        /// </summary>
        /// <param name="allegato"></param>
        /// <param name="putfile"></param>
        /// <returns></returns>
        public bool AddAttachment(DocsPaVO.documento.Allegato allegato, string putfile)
        {
            bool retValue = false;

            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                retValue = this.DocumentManagerETDOCS.AddAttachment(allegato, putfile);

                if (retValue)
                    retValue = this.DocumentManagerOCS.AddAttachment(allegato, putfile);

                if (retValue)
                    transactionContext.Complete();
            }

            return retValue;
        }

        /// <summary>
        /// Modifica di un allegato
        /// </summary>
        /// <param name="allegato"></param>
        public void ModifyAttatchment(DocsPaVO.documento.Allegato allegato)
        {
            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                this.DocumentManagerETDOCS.ModifyAttatchment(allegato);

                this.DocumentManagerOCS.ModifyAttatchment(allegato);

                transactionContext.Complete();
            }
        }

        /// <summary>
        /// Rimozione di un allegato
        /// </summary>
        /// <param name="allegato"></param>
        /// <returns></returns>
        public bool RemoveAttatchment(DocsPaVO.documento.Allegato allegato)
        {
            bool retValue = false;

            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                // nota: se cancello prima il docuemnto su docspa non riesco a risalire all'id su OCS
                // a meno chè non uso una ricerca ma al momento danno problemi!!!
                retValue = this.DocumentManagerOCS.RemoveAttatchment(allegato);

                if (retValue)
                {
                    // Se la nuova gestione degli allegati non risulta abilitata,
                    // il documento allegato viene rimosso direttamente
                    DocsPaDB.Query_DocsPAWS.Documenti dbDocumenti = new DocsPaDB.Query_DocsPAWS.Documenti();

                    // Reperimento dell'oggetto InfoDocumento relativo all'allegato
                    DocsPaVO.documento.InfoDocumento infoDocumentoAllegato = dbDocumenti.GetInfoDocumento(this.InfoUtente.idGruppo, this.InfoUtente.idPeople, allegato.docNumber, false);

                    retValue = ((DocsPaDocumentale_ETDOCS.Documentale.DocumentManager)this.DocumentManagerETDOCS).Remove(new DocsPaVO.documento.InfoDocumento[1] { infoDocumentoAllegato }, false);
                }

                if (retValue)
                    transactionContext.Complete();
            }

            return retValue;
        }

        /// <summary>
        /// Scambia il file associato ad un allegato con il file associato ad un documento
        /// </summary>
        /// <param name="allegato"></param>
        /// <param name="documento"></param>
        /// <returns></returns>
        public bool ScambiaAllegatoDocumento(DocsPaVO.documento.Allegato allegato, DocsPaVO.documento.Documento documento)
        {
            bool retValue = false;

            try
            {
                retValue = this.ScambiaFile(_infoUtente, allegato, documento);

                if (retValue)
                    retValue = this.DocumentManagerOCS.ScambiaAllegatoDocumento(allegato, documento);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nell'operazione 'ScambiaAllegatoDocumento': {0}", ex.Message);
                logger.Debug(errorMessage, ex);

                retValue = false;
            }

            return retValue;
        }

        #endregion

        #region Gestione file

        /// <summary>
        /// Reperimento del contentuto di un file dal repository OCS
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="version"></param>
        /// <param name="versionId"></param>
        /// <param name="versionLabel"></param>
        /// <returns></returns>
        public byte[] GetFile(string docNumber, string version, string versionId, string versionLabel)
        {
            return this.DocumentManagerOCS.GetFile(docNumber, version, versionId, versionLabel);
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
        /// <param name="idamministrazione"></param>
        /// <returns></returns>
        public bool GetFile(ref DocsPaVO.documento.FileDocumento fileDocumento, ref DocsPaVO.documento.FileRequest fileRequest)
        {
            return this.DocumentManagerOCS.GetFile(ref fileDocumento, ref fileRequest);
        }

        /// <summary>
        /// Operazione per l'inserimento di un file in una versione / allegato di un documento
        /// 
        /// PreCondizione:
        ///     la versione / allegato deve essere già stato creato come entità persistente
        /// 
        ///     Attributi dell'oggetto FileRequest già valorizzati in ingresso
        ///         VersionId:      ID della versione / allegato in docspa
        ///         Version:        0 se allegato, > 0 se versione
        ///         SubVersion:     ! se allegato, stringa vuota se versione
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
            //Performance.PerformanceLogWriter.CreatePerformanceLogWriter(@"c:\temp\DocumentManager.txt");

            //Performance.PerformanceLogWriter log = new Performance.PerformanceLogWriter();
            //log.StartLogEntry("PutFile");

            bool retValue = false;

            try
            {
                // Inserimento del file in OCS
                retValue = this.DocumentManagerOCS.PutFile(fileRequest, fileDocumento, estensione);

                if (retValue)
                {
                    // Aggiornamento attributi filerequest
                    // Per OCS abbiamo cambiato il nome del file, non il version_id ma il docNumber
                    //fileRequest.fileName = string.Format("{0}.{1}", fileRequest.versionId, estensione);
                    fileRequest.fileName = string.Format("{0}.{1}", fileRequest.docNumber, estensione);
                    fileRequest.fileSize = fileDocumento.content.Length.ToString();

                    this.SetFileAsInserted(fileRequest, fileDocumento.content);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore nel metodo PutFile: {0}", ex.Message));
                retValue = false;
            }

            //log.EndLogEntry();
            //Performance.PerformanceLogWriter.FlushPerformanceLogWriter();

            return retValue;
        }

        #endregion

        #region Gestione documenti in cestino

        /// <summary>
        /// Inserimento di un documento nel cestino
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public bool AddDocumentoInCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            bool retValue = false;

            try
            {
                // E' necessario richiamare prima i servizi ocs piuttosto che etdocs
                // in quanto, se si tratta di un allegato, il documento deve essere
                // direttamente rimosso. In tal caso è necessario accedere ai dati
                // per reperire il path del documento, cosa che non sarebbe possibile
                // se il documento viene prima rimosso da etdocs
                retValue = this.DocumentManagerOCS.AddDocumentoInCestino(infoDocumento);

                if (retValue)
                    retValue = this.DocumentManagerETDOCS.AddDocumentoInCestino(infoDocumento);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nell'operazione 'AddDocumentoInCestino': {0}", ex.Message);
                logger.Debug(errorMessage, ex);

                retValue = false;
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
                retValue = this.DocumentManagerETDOCS.RestoreDocumentoDaCestino(infoDocumento);

                if (retValue)
                    retValue = this.DocumentManagerOCS.RestoreDocumentoDaCestino(infoDocumento);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nell'operazione 'RestoreDocumentoDaCestino': {0}", ex.Message);
                logger.Debug(errorMessage, ex);

                retValue = false;
            }

            return retValue;
        }

        #endregion

        /// <summary>
        /// Reperimento della VersionId dell'ultima versione di un documento
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public string GetLatestVersionId(string docNumber)
        {
            // Viene richiamato soltanto il documentale etdocs,
            // in quanto è il gestore dei dati sulle versioni
            return this.DocumentManagerETDOCS.GetLatestVersionId(docNumber);
        }

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
            bool retValue = false;

            try
            {
                retValue = this.DocumentManagerETDOCS.ModifyExtension(ref fileRequest, docNumber, version_id, version, subVersion, versionLabel);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nell'operazione 'ModifyExtension': {0}", ex.Message);
                logger.Debug(errorMessage, ex);
                retValue = false;
            }

            return retValue;
        }
        
        /// <summary>
        /// Reperimento estensione applicazione
        /// </summary>
        /// <param name="docnumber"></param>
        /// <param name="versionid"></param>
        /// <returns></returns>
        public string GetFileExtension(string docnumber, string versionid)
        {
            return this.DocumentManagerETDOCS.GetFileExtension(docnumber, versionid);
        }

        /// <summary>
        /// Reperimento del nome originale del file nel documentale
        /// </summary>
        /// <param name="docnumber"></param>
        /// <param name="versionid"></param>
        /// <returns></returns>
        public string GetOriginalFileName(string docnumber, string versionid)
        {
            return this.DocumentManagerETDOCS.GetOriginalFileName(docnumber, versionid);
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
            return this.DocumentManagerETDOCS.ModifyVersionSegnatura(versionId);
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
            return this.DocumentManagerETDOCS.IsVersionWithSegnature(versionId);
        }

        /// <summary>
        /// Metodo per l'assegnazione di un diritto di tipo A ad un ruolo
        /// </summary>
        /// <param name="rights">Informazioni sul diritto da assegnare</param>
        /// <returns>True se è andato bene</returns>
        public bool AddPermissionToRole(DirittoOggetto rights)
        {
            return this.DocumentManagerETDOCS.AddPermissionToRole(rights);
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

        #region Protected methods

        /// <summary>
        /// Credenziali utente corrente
        /// </summary>
        protected InfoUtente InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }

        /// <summary>
        /// Documentale etdocs
        /// </summary>
        protected IDocumentManager DocumentManagerETDOCS
        {
            get
            {
                return this._documentManagerETDOCS;
            }
        }

        /// <summary>
        /// Documentale OCS
        /// </summary>
        protected IDocumentManager DocumentManagerOCS
        {
            get
            {
                return this._documentManagerOCS;
            }
        }

        /// <summary>
        /// Inserimento dei metadati di un file inserito in etdocs
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="fileContent"></param>
        /// <param name="fileExtension"></param>
        protected virtual void SetFileAsInserted(FileRequest fileRequest, byte[] fileContent)
        {
            // Aggiornamento tabella COMPONENTS				
            string varImpronta = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(fileContent);
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.UpdateComponents(fileRequest.fileSize, varImpronta, fileRequest.versionId, fileRequest.docNumber);

            DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
            documentale.UpdateFileName(fileRequest.fileName, fileRequest.versionId);

            //Aggiornamento tabella PROFILE
            int version;
            if (Int32.TryParse(fileRequest.version, out version))
            {
                if (version > 0)
                    doc.SetImg(fileRequest.docNumber);
            }
        }

        #endregion

        #region Private methods

        /// </summary>
        /// <param name="versionId"></param>
        /// <param name="dbProvider"></param>
        /// <returns></returns>
        private DataRow GetDatiScambiaAllegato(string versionId, DocsPaDB.DBProvider dbProvider)
        {
            DataRow row = null;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_SELECT_DATA_SCAMBIA_ALLEGATO");
            queryDef.setParam("versionId", versionId);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            DataSet ds;
            if (dbProvider.ExecuteQuery(out ds, commandText))
                row = ds.Tables[0].Rows[0];

            return row;
        }

        /// </summary>
        /// <param name="versionId"></param>
        /// <param name="subVersion"></param>
        /// <param name="cartaceo"></param>
        /// <param name="scartaFascCartacea"></param>
        /// <param name="dbProvider"></param>
        /// <returns></returns>
        private bool UpdateVersionScambiaAllegato(string versionId, string subVersion, string cartaceo, string scartaFascCartacea, DocsPaDB.DBProvider dbProvider)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_VERSION_SCAMBIA_ALLEGATO");

            queryDef.setParam("versionId", versionId);
            queryDef.setParam("subVersion", this.GetStringParamValue(subVersion, true));
            queryDef.setParam("cartaceo", (string.IsNullOrEmpty(cartaceo) ? "null" : cartaceo));
            queryDef.setParam("scartaFascCartacea", (string.IsNullOrEmpty(scartaFascCartacea) ? "null" : scartaFascCartacea));

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            int rowsAffected;
            dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

            return (rowsAffected > 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="versionId"></param>
        /// <param name="path"></param>
        /// <param name="fileSize"></param>
        /// <param name="impronta"></param>
        /// <param name="dbProvider"></param>
        /// <returns></returns>
        private bool UpdateComponentsScambiaAllegato(string versionId, string path, string fileSize, string impronta, DocsPaDB.DBProvider dbProvider)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_COMPONENTS_SCAMBIA_ALLEGATO");

            queryDef.setParam("versionId", versionId);
            queryDef.setParam("path", this.GetStringParamValue(path, true));
            queryDef.setParam("fileSize", (string.IsNullOrEmpty(fileSize) ? "0" : fileSize));
            queryDef.setParam("impronta", this.GetStringParamValue(impronta, true));

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            int rowsAffected;
            dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

            return (rowsAffected > 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="addApex"></param>
        /// <returns></returns>
        private string GetStringParamValue(object value, bool addApex)
        {
            string retValue = string.Empty;

            if (value != null)
            {
                retValue = value.ToString().Replace("'", "''");

                if (addApex)
                    retValue = string.Format("'{0}'", retValue);
            }
            else
                retValue = "Null";

            return retValue;
        }

        private bool ScambiaFile(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.Allegato allegato, DocsPaVO.documento.Documento documento)
        {
            bool retValue = false;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                DataRow rowDocumento = this.GetDatiScambiaAllegato(documento.versionId, dbProvider);
                DataRow rowAllegato = this.GetDatiScambiaAllegato(allegato.versionId, dbProvider);

                // Aggiornamento record VERSIONS per l'allegato con i dati del documento
                retValue = this.UpdateVersionScambiaAllegato(allegato.versionId,
                                    DocsPaUtils.Data.DataReaderHelper.GetValue<object>(rowDocumento, "SUBVERSION", false).ToString(),
                                    DocsPaUtils.Data.DataReaderHelper.GetValue<object>(rowDocumento, "CARTACEO", true, string.Empty).ToString(),
                                    DocsPaUtils.Data.DataReaderHelper.GetValue<object>(rowDocumento, "SCARTA_FASC_CARTACEA", true, string.Empty).ToString(),
                                    dbProvider);

                if (retValue)
                {
                    // Aggiornamento record VERSIONS per il documento con i dati dell'allegato
                    retValue = this.UpdateVersionScambiaAllegato(documento.versionId,
                                    DocsPaUtils.Data.DataReaderHelper.GetValue<object>(rowAllegato, "SUBVERSION", false).ToString(),
                                    DocsPaUtils.Data.DataReaderHelper.GetValue<object>(rowAllegato, "CARTACEO", true, string.Empty).ToString(),
                                    DocsPaUtils.Data.DataReaderHelper.GetValue<object>(rowAllegato, "SCARTA_FASC_CARTACEA", true, string.Empty).ToString(),
                                    dbProvider);
                }

                if (retValue)
                {
                    // Aggiornamento record COMPONENTS per l'allegato con i dati del documento
                    //bisogna calcolare il path: in realtà quello che cambia è solo l'estensione del file
                    string pathDocumento = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(rowDocumento, "PATH", true, string.Empty).ToString();
                    string pathAllegato = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(rowAllegato, "PATH", true, string.Empty).ToString();
                    string estDoc = pathDocumento.Substring(pathDocumento.LastIndexOf("."));
                    string estAll = pathAllegato.Substring(pathAllegato.LastIndexOf("."));
                    string newPathDoc = documento.docNumber + estAll;
                    string newPathAll = allegato.docNumber + estDoc;
                    retValue = this.UpdateComponentsScambiaAllegato(allegato.versionId,
                            //DocsPaUtils.Data.DataReaderHelper.GetValue<object>(rowDocumento, "PATH", true, string.Empty).ToString(),
                            newPathAll,
                            DocsPaUtils.Data.DataReaderHelper.GetValue<object>(rowDocumento, "FILE_SIZE", true, string.Empty).ToString(),
                            DocsPaUtils.Data.DataReaderHelper.GetValue<object>(rowDocumento, "VAR_IMPRONTA", true, string.Empty).ToString(), dbProvider);

                    if (retValue)
                    {
                        // Aggiornamento record COMPONENTS per il documento con i dati dell'allegato
                        retValue = this.UpdateComponentsScambiaAllegato(documento.versionId,
                            //DocsPaUtils.Data.DataReaderHelper.GetValue<object>(rowAllegato, "PATH", true, string.Empty).ToString(),
                            newPathDoc,
                            DocsPaUtils.Data.DataReaderHelper.GetValue<object>(rowAllegato, "FILE_SIZE", true, string.Empty).ToString(),
                            DocsPaUtils.Data.DataReaderHelper.GetValue<object>(rowAllegato, "VAR_IMPRONTA", true, string.Empty).ToString(),
                            dbProvider);
                    }
                }
            }

            return retValue;
        }


        #endregion
    }
}
