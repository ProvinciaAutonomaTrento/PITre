using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.documento;
using DocsPaVO.utente;
using DocumentManagerETDOCS = DocsPaDocumentale_ETDOCS.Documentale.DocumentManager;
using log4net;
using DocsPaDocumentale_HERMES;

namespace DocsPaDocumentale_CONN_HERMES.Documentale
{
    /// <summary>
    /// </summary>
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
        private IDocumentManager DocumentManagerHERMES {get; set;}

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
            //this.DocumentManagerHERMES = new DocumentManagerHermesHB();
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
                    retValue = this.DocumentManagerHERMES.CreateDocumentoStampaRegistro(schedaDocumento, ruolo);
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
        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            logger.Info("BEGIN");
            //Performance.PerformanceLogWriter.CreatePerformanceLogWriter(@"c:\temp\DocumentManager.txt");

            //Performance.PerformanceLogWriter log = new Performance.PerformanceLogWriter();
            //log.StartLogEntry("CreateDocumentoGrigio");
            
            bool retValue = false;
            ruoliSuperiori = null;

            try
            {
                //log.StartLogEntry("CreateDocumentoGrigio_ETDOCS", true);
                retValue = this.DocumentManagerETDOCS.CreateDocumentoGrigio(schedaDocumento, ruolo, out ruoliSuperiori);
                //log.EndLogEntry();

                //log.StartLogEntry("CreateDocumentoGrigio_DCTM", true);
                if (retValue)
                    retValue = this.DocumentManagerDocumentum.CreateDocumentoGrigio(schedaDocumento, ruolo);
                //log.EndLogEntry();
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nella creazione di un documento grigio: {0}", ex.Message);
                logger.Debug(errorMessage, ex);
                retValue = false;
            }

            //log.EndLogEntry();
            //Performance.PerformanceLogWriter.FlushPerformanceLogWriter();
            logger.Info("END");
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
                    retValue = this.DocumentManagerDocumentum.PredisponiProtocollazione(schedaDocumento);
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
                    retValue = this.DocumentManagerDocumentum.ProtocollaDocumentoPredisposto(schedaDocumento, ruolo, out risultatoProtocollazione);
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
        public bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            logger.Info("BEGIN");
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
                    retValue = this.DocumentManagerDocumentum.CreateProtocollo(schedaDocumento, ruolo, out risultatoProtocollazione);
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
            logger.Info("END");
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
            //Performance.PerformanceLogWriter.CreatePerformanceLogWriter(@"c:\temp\DocumentManager.txt");

            //Performance.PerformanceLogWriter log = new Performance.PerformanceLogWriter();
            //log.StartLogEntry("SalvaDocumento");

            bool retValue = false;
            ufficioReferenteSaved = false;

            try
            {
                //log.StartLogEntry("SalvaDocumento_ETDOCS", true);
                retValue = this.DocumentManagerETDOCS.SalvaDocumento(schedaDocumento, ufficioReferenteEnabled, out ufficioReferenteSaved);
                //log.EndLogEntry();

                //log.StartLogEntry("SalvaDocumento_DCTM", true);
                if (retValue)
                    retValue = this.DocumentManagerDocumentum.SalvaDocumento(schedaDocumento, ufficioReferenteEnabled, out ufficioReferenteSaved);
                //log.EndLogEntry();
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nel salvataggio dei dati del documento: {0}", ex.Message);
                logger.Debug(errorMessage, ex);
                retValue = false;
            }

            //Performance.PerformanceLogWriter.FlushPerformanceLogWriter();

            return retValue;
        }

        /// <summary>
        /// Inserimento di una nuova versione
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="daInviare"></param>
        /// <returns></returns>
        public bool AddVersion(DocsPaVO.documento.FileRequest fileRequest, bool daInviare)
        {
            logger.Info("BEGIN");
            bool retValue = false;

            try
            {
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    retValue = this.DocumentManagerETDOCS.AddVersion(fileRequest, daInviare);

                    if (retValue)
                        retValue = this.DocumentManagerDocumentum.AddVersion(fileRequest, daInviare);

                    if (retValue)
                        transactionContext.Complete();
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nell'inserimento della versione: {0}", ex.Message);
                logger.Debug(errorMessage, ex);
                retValue = false;
            }
            logger.Info("END");
            return retValue;
        }

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
                    retValue = this.DocumentManagerDocumentum.AddAttachment(allegato, putfile);
                
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
            try
            {
                this.DocumentManagerETDOCS.ModifyAttatchment(allegato);

                this.DocumentManagerDocumentum.ModifyAttatchment(allegato);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nella modifica dell'allegato: {0}", ex.Message);
                logger.Debug(errorMessage, ex);
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
                // Se la nuova gestione degli allegati non risulta abilitata,
                // il documento allegato viene rimosso direttamente
                DocsPaDB.Query_DocsPAWS.Documenti dbDocumenti = new DocsPaDB.Query_DocsPAWS.Documenti();

                // Reperimento dell'oggetto InfoDocumento relativo all'allegato
                DocsPaVO.documento.InfoDocumento infoDocumentoAllegato = dbDocumenti.GetInfoDocumento(this.InfoUtente.idGruppo, this.InfoUtente.idPeople, allegato.docNumber, false);

                retValue = ((DocsPaDocumentale_ETDOCS.Documentale.DocumentManager)this.DocumentManagerETDOCS).Remove(new DocsPaVO.documento.InfoDocumento[1] { infoDocumentoAllegato }, false);

                //quando funziona lo mettiamo in linea.. LL   
                //if (retValue)
                //    retValue = this.DocumentManagerDocumentum.RemoveAttatchment(allegato);

                if (retValue)
                    transactionContext.Complete();
            }
            
            return retValue;
        }

        /// <summary>
        /// Rimozione di una versione
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public bool RemoveVersion(DocsPaVO.documento.FileRequest fileRequest)
        {
            bool retValue = false;

            try
            {
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    retValue = this.DocumentManagerETDOCS.RemoveVersion(fileRequest);

                    if (retValue)
                        retValue = this.DocumentManagerDocumentum.RemoveVersion(fileRequest);

                    if (retValue)
                        transactionContext.Complete();
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nella rimozione della versione: {0}", ex.Message);
                logger.Debug(errorMessage, ex);
                retValue = false;
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento del contentuto di un file dal repository Documentum
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="version"></param>
        /// <param name="versionId"></param>
        /// <param name="versionLabel"></param>
        /// <returns></returns>
        public byte[] GetFile(string docNumber, string version, string versionId, string versionLabel)
        {
            logger.Info("BEGIN");
            byte[] content = null;

            if (this.PitreDualFileWritingMode)
            {
                // E' attiva la modalità di scrittura dei file su entrambi i documentali

                try
                {
                    // Il reperimento dei file viene dapprima effettuato dal documentale ETNOTEAM
                    content = this.DocumentManagerETDOCS.GetFile(docNumber, version, versionId, versionLabel);
                }
                catch (Exception ex)
                {
                    logger.Debug("Errore nel reperimento del file nel documentale ETNOTEAM", ex);

                    // Errore nel reperimento del file nel documentale ETNOTEAM
                    content = null;
                }

                if (content != null)
                {
                    // In caso di errore nel reperimento del file in ETNOTEAM, viene richiamato
                    // il corrispondente metodo in documentum
                    content = this.DocumentManagerDocumentum.GetFile(docNumber, version, versionId, versionLabel);
                }
            }
            else
                content = this.DocumentManagerDocumentum.GetFile(docNumber, version, versionId, versionLabel);

            logger.Info("END");
            return content;
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
            logger.Info("BEGIN");
            bool retValue = false;

            if (this.PitreDualFileWritingMode)
            {
                // E' attiva la modalità di scrittura dei file su entrambi i documentali

                try
                {
                    // Il reperimento dei file viene dapprima effettuato dal documentale DTCM
                    retValue = this.DocumentManagerDocumentum.GetFile(ref fileDocumento, ref fileRequest);
                   
                }
                catch (Exception ex)
                {
                    logger.Debug("Errore nel reperimento del file nel documentale DTCM", ex);

                    // Errore nel reperimento del file nel documentale ETNOTEAM
                    retValue = false;
                }

                if (!retValue)
                {
                    // In caso di errore nel reperimento del file in ETNOTEAM, viene richiamato
                    // il corrispondente metodo in documentum
                    retValue = this.DocumentManagerETDOCS.GetFile(ref fileDocumento, ref fileRequest);
                }
            }
            else
                retValue = this.DocumentManagerDocumentum.GetFile(ref fileDocumento, ref fileRequest);

            logger.Info("END");
            return retValue;
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
            logger.Info("BEGIN");
            bool retValue = false;

            try
            {
                if (this.PitreDualFileWritingMode)
                {
                    // E' attiva la modalità di scrittura dei file su entrambi i documentali
                    retValue = this.DocumentManagerETDOCS.PutFile(fileRequest, fileDocumento, estensione);
                    
                    if (retValue)
                    {
                        // Inserimento del file in documentum
                        retValue = this.DocumentManagerHERMES.PutFile(fileRequest, fileDocumento, estensione);
 
                            //this.DocumentManagerDocumentum.PutFile(fileRequest, fileDocumento, estensione);
                    }
                }
                else
                {
                    // Inserimento del file in documentum
                    retValue = this.DocumentManagerHERMES.PutFile(fileRequest, fileDocumento, estensione);

                    if (retValue)
                    {
                        // Sovrascrittura:
                        // L'aggiornamento dei metadati in etdocs inserisce in components il path completo
                        // del file. Per PITRE non è possibile in quanto il file è nel repository DCTM
                        // e pertanto non è possibile stabilirne il path.
                        // Viene quindi aggiornato solo il nome del file + estensione.


                        /*
                        string fileExtension = fileDocumento.estensioneFile;
                        if (string.IsNullOrEmpty(fileExtension))
                            fileExtension = estensione;

                        fileRequest.fileName = string.Format("{0}.{1}", fileRequest.versionId, fileExtension);
                        fileRequest.path = fileRequest.fileName;
                        fileRequest.fileSize = fileDocumento.content.Length.ToString();

                        // Aggiornamento dei metadati del file inserito nel documentale etdocs
                        ((DocsPaDocumentale_ETDOCS.Documentale.DocumentManager)this.DocumentManagerETDOCS).PutFileMetadata(fileRequest, fileDocumento, estensione);

                        // DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
                        // documentale.UpdateFileName(fileRequest.fileName, fileRequest.versionId);

                        //this.SetFileAsInserted(fileRequest, fileDocumento.content);
                         */

                        // Creazione nome file
                        string fileName = string.Empty;

                        if (!string.IsNullOrEmpty(fileRequest.fileName))
                        {
                            fileName = fileRequest.fileName;

                            string extensions = string.Empty;

                            while (!string.IsNullOrEmpty(Path.GetExtension(fileName)))
                            {
                                extensions = Path.GetExtension(fileName) + extensions;

                                fileName = Path.GetFileNameWithoutExtension(fileName);
                            }

                            fileName = string.Concat(fileRequest.versionId, extensions);
                        }
                        else
                            fileName = string.Format("{0}.{1}", fileRequest.versionId, estensione);

                        // Creazione path completo del file
                        fileRequest.path = fileName;

                        // Calcolo file size
                        fileRequest.fileSize = fileDocumento.content.Length.ToString();

                        // Aggiornamento dei metadati del file inserito nel documentale etdocs
                        ((DocsPaDocumentale_ETDOCS.Documentale.DocumentManager)this.DocumentManagerETDOCS).PutFileMetadata(fileRequest, fileDocumento, estensione);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore nel metodo PutFile: {0}", ex.Message));
                retValue = false;
            }

            logger.Info("END");
            return retValue;
        }

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

                if (retValue) retValue = this.DocumentManagerDocumentum.AnnullaProtocollo(ref schedaDocumento, protocolloAnnullato); ;
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
        /// Modifica dei metadati di una versione
        /// </summary>
        /// <param name="fileRequest"></param>
        public void ModifyVersion(DocsPaVO.documento.FileRequest fileRequest)
        {
            try
            {
                this.DocumentManagerETDOCS.ModifyVersion(fileRequest);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nell'operazione 'ModifyVersion': {0}", ex.Message);
                logger.Debug(errorMessage, ex);
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
            try
            {
                return this.DocumentManagerETDOCS.ModifyVersionSegnatura(versionId);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nell'operazione 'ModifyVersionSegnatura': {0}", ex.Message);
                logger.Debug(errorMessage, ex);
            }
            return false;
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
            try
            {
                return this.DocumentManagerETDOCS.IsVersionWithSegnature(versionId);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nell'operazione 'IsVersionWithSegnature': {0}", ex.Message);
                logger.Debug(errorMessage, ex);
            }
            return false;
        }
 
        ///// <summary>
        ///// Rimozione di un documento grigio
        ///// </summary>
        ///// <param name="schedaDocumento"></param>
        ///// <returns></returns>
        //public string RemoveDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        //{
        //    string retValue = string.Empty;

        //    try
        //    {
        //        if (this.DocumentManagerETDOCS.RemoveDocumentoGrigio(schedaDocumento).ToUpper() == "DEL")
        //            retValue = this.DocumentManagerDocumentum.RemoveDocumentoGrigio(schedaDocumento);
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = string.Format("Errore nell'operazione 'RemoveDocumentoGrigio': {0}", ex.Message);
        //        logger.Debug(errorMessage, ex);

        //        retValue = errorMessage;
        //    }

        //    return retValue;
        //}

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
                retValue = this.DocumentManagerETDOCS.AddDocumentoInCestino(infoDocumento);

                if (retValue)
                    retValue = this.DocumentManagerDocumentum.AddDocumentoInCestino(infoDocumento);
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
                    retValue = this.DocumentManagerDocumentum.RestoreDocumentoDaCestino(infoDocumento);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nell'operazione 'RestoreDocumentoDaCestino': {0}", ex.Message);
                logger.Debug(errorMessage, ex);

                retValue = false;
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
                retValue = this.DocumentManagerETDOCS.ScambiaAllegatoDocumento(allegato, documento);

                if (retValue)
                    retValue = this.DocumentManagerDocumentum.ScambiaAllegatoDocumento(allegato, documento);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nell'operazione 'ScambiaAllegatoDocumento': {0}", ex.Message);
                logger.Debug(errorMessage, ex);

                retValue = false;
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
                // Per PITRE, questo non è necessario in quanto è DCTM che gestisce
                // la cancellazione dei file.
                foreach (DocsPaVO.documento.InfoDocumento infoDocumento in items)
                {
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                    
                    retValue = doc.EliminaDoc(this.InfoUtente, infoDocumento);

                    if (!retValue)
                        break;
                }

                if (retValue)
                {
                    try 
                    {
                        retValue = this.DocumentManagerDocumentum.Remove(items);
                    }
                    catch(Exception ex1)
                    {
                         retValue = true;
                         string errorMessage1 = string.Format("Errore nell'operazione 'RemoveDocumento' da Documentum: {0}", ex1.Message);

                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nell'operazione 'RemoveDocumento': {0}", ex.Message);
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
        /// Impostazione di un permesso su un documento
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        public bool AddPermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            bool retValue = this.DocumentManagerETDOCS.AddPermission(infoDiritto);

            if (retValue)
                retValue = this.DocumentManagerDocumentum.AddPermission(infoDiritto);

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
                retValue = this.DocumentManagerDocumentum.RemovePermission(infoDiritto);

            return retValue;
        }

        /// <summary>
        /// Metodo per l'assegnazione di un diritto di tipo A ad un ruolo
        /// </summary>
        /// <param name="rights">Informazioni sul diritto da assegnare</param>
        /// <returns>True se è andato bene</returns>
        public bool AddPermissionToRole(DirittoOggetto rights)
        {
            bool retValue = this.DocumentManagerETDOCS.AddPermissionToRole(rights);

            if(retValue)
                retValue = this.DocumentManagerETDOCS.AddPermissionToRole(rights);

            return retValue;
        }

        /// <summary>
        /// Aggiorna le ACL del documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        public virtual void RefreshAclDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            try
            {
                this._documentManagerETDOCS.RefreshAclDocumento(schedaDocumento);
                //this._documentManagerDocumentum.RefreshAclDocumento(schedaDocumento);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore in Documentum.RefreshAclDocumento:\n{0}", ex.ToString());
                logger.Debug(errorMessage);

                throw new ApplicationException(errorMessage);
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Indica se è attiva la modalità di scrittura del file su entrambi i documentali
        /// </summary>
        protected bool PitreDualFileWritingMode
        {
            get
            {
                const string KEY = "PitreDualFileWritingMode";

                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[KEY]))
                    return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings[KEY]);
                else
                    return false;
            }
        }

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
        /// Documentale documentum
        /// </summary>
        protected IDocumentManager DocumentManagerDocumentum
        {
            get
            {
                //return this._documentManagerDocumentum;
                return null;
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

        #endregion
    }
}
