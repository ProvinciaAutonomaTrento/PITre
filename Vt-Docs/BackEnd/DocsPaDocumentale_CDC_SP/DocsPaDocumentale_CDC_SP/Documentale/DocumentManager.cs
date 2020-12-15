using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.documento;
using DocsPaVO.utente;
using System.IO;
using System.Collections;
using System.Threading;
using System.Data;
using DocsPaDbManagement.Functions;
using System.Configuration;
using System.Data.OleDb;
using DocsPaUtils.Functions;
using DocsPaDB;
using System.Xml;
using DocsPaDocumentale_SP;
using DocumentManagerETDOCS = DocsPaDocumentale_ETDOCS.Documentale.DocumentManager;
using DocumentManagerSP = DocsPaDocumentale_SP.Documentale.DocumentManager;
using log4net;
using System.Configuration;


namespace DocsPaDocumentale_CDC_SP.Documentale
{
    /// <summary>
    /// 
    /// Classe documentale che si colloca ad un livello di astrazione pi� alto 
    /// rispetto ai componenti per la gestione dei singoli software documentali
    /// (ETDOCS e SP), in quanto:
    /// 
    /// o	utilizza prima lo strato ETDOCS per la generazione dei metadati 
    ///     (es. numero protocollo, segnatura, ecc.), successivamente 
    ///     lo strato SP per la memorizzazione dei metadati generati;
    /// o	fornisce un livello di astrazione ai layer pi� alti (BusinessLogic di DocsPa) 
    ///     verso i due strati documentali sottostanti;
    /// o	GFD � un gestore delle transazioni a due fasi;
    /// 
    /// <remarks>
    /// Da specifiche per l�integrazione con DocsPa, SP � utilizzato come:
    /// o	repository fisico dei file 
    /// o	contenitore di alcuni metadati dei documenti / fascicoli / trasmissioni 
    ///     per una �coerente� consultazione in sola lettura dall�interfaccia 
    ///     proprietaria di SP (Webtop)
    ///  o	non � utilizzato per l�erogazione dei metadati di protocollazione.
    /// </remarks>
    /// 
    /// </summary>
    public class DocumentManager : IDocumentManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocumentManager));
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        private IDocumentManager _documentManagerETDOCS = null;
        private IDocumentManager _documentManagerSP = null;



        /// <summary>
        /// Istanza interna della classe InfoUtente
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

            this._documentManagerSP = new DocumentManagerSP(infoUtente);

        }

        #endregion

        #region Public methods


        /// <summary>
        /// Creazione di un nuovo documento per la stampa registro.
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns>ID del documento o 'null' se si � verificato un errore</returns>
        public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo)
        {
            DocsPaVO.utente.Ruolo[] ruoliSuperiori;
            return this.CreateDocumentoStampaRegistro(schedaDocumento, ruolo, out ruoliSuperiori);
        }

        /// <summary>
        /// Creazione di un nuovo documento per la stampa registro.
        /// overload.
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="ruoliSuperiori"></param>
        /// <returns>ID del documento o 'null' se si � verificato un errore</returns>
        public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            bool retValue = false;
            ruoliSuperiori = null;

            try
            {
                retValue = this.DocumentManagerETDOCS.CreateDocumentoStampaRegistro(schedaDocumento, ruolo, out ruoliSuperiori);
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
        /// Crea la scheda documento Grigio
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns>True/false sulla creazione del documento</returns>
        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo)
        {
            DocsPaVO.utente.Ruolo[] ruoliSuperiori;
            return this.CreateDocumentoGrigio(schedaDocumento, ruolo, out ruoliSuperiori);
        }

        /// <summary>
        /// Crea la scheda documento Grigio
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="ruoliSuperiori"></param>
        /// <returns>True/false sulla creazione del documento</returns>
        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string conCopia = null)
        {
            logger.Info("BEGIN");
            bool retValue = false;
            ruoliSuperiori = null;

            try
            {
                retValue = this.DocumentManagerETDOCS.CreateDocumentoGrigio(schedaDocumento, ruolo, out ruoliSuperiori);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nella creazione di un documento grigio: {0}", ex.Message);
                logger.Debug(errorMessage, ex);
                retValue = false;
            }

            logger.Info("END");
            return retValue;
        }

        /// <summary>
        /// Predisposizione di un documento alla protocollazione
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns>True/False</returns>
        public bool PredisponiProtocollazione(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            bool retValue = false;

            try
            {
                retValue = this.DocumentManagerETDOCS.PredisponiProtocollazione(schedaDocumento);


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
        /// <returns>True/False</returns>
        public bool ProtocollaDocumentoPredisposto(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione)
        {
            bool retValue = false;
            risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;

            try
            {
                // Stacco del numero di protocollo da parte di cdcSP
                retValue = this._documentManagerSP.CreateProtocollo(schedaDocumento, ruolo, out risultatoProtocollazione);

                // Se il risultato è positivo, si procede con una protocollazione di predisposto su ETDocs
                if (retValue && risultatoProtocollazione == ResultProtocollazione.OK)
                    retValue = this._documentManagerETDOCS.ProtocollaDocumentoPredisposto(schedaDocumento, ruolo, out risultatoProtocollazione);

                // Se la protocollazione del predisposto è andata a buon fine, si procede con la put file su SP
                if (retValue && risultatoProtocollazione == ResultProtocollazione.OK)
                {
                    if (schedaDocumento.documenti.Count > 0)
                    {
                        List<FileRequest> versions = new List<DocsPaVO.documento.FileRequest>(
                                (DocsPaVO.documento.FileRequest[])
                                    schedaDocumento.documenti.ToArray(typeof(DocsPaVO.documento.FileRequest)));

                        // Ordinamento versioni
                        versions.Sort(
                                delegate(DocsPaVO.documento.FileRequest x, DocsPaVO.documento.FileRequest y)
                                {
                                    int versionX, versionY;
                                    Int32.TryParse(x.version, out versionX);
                                    Int32.TryParse(y.version, out versionY);

                                    return (versionX.CompareTo(versionY));
                                }
                            );

                        // Invio file ultima versione del doc principale
                        TransferFileToSP(versions[0]);
                    }

                    if (schedaDocumento.allegati != null && schedaDocumento.allegati.Count > 0)
                        foreach (FileRequest attachment in schedaDocumento.allegati)
                            this.TransferFileToSP(attachment);

                }

                // Protocollazione classica
                //retValue = this.CreateProtocollo(schedaDocumento, ruolo, out risultatoProtocollazione);

                /*
                if(retValue)
                    retValue = this.DocumentManagerETDOCS.ProtocollaDocumentoPredisposto(schedaDocumento, ruolo, out risultatoProtocollazione);
                */
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
        /// Funzione per il trasferimento di un documento da ETDocs a SP
        /// </summary>
        /// <param name="fileRequest">File da trasferire</param>
        private void TransferFileToSP(FileRequest fileRequest)
        {
            // Recupero informazioni sull'ultimo file
            FileDocumento fileDocument = new FileDocumento();
            this.GetFile(ref fileDocument, ref fileRequest);

            // Invio a SP del file
            if (fileDocument.content != null && fileDocument.content.Length > 0)
                this._documentManagerSP.PutFile(fileRequest, fileDocument, Path.GetExtension(fileRequest.fileName));
        }

        /// <summary>
        /// Crea il protoccolo.
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="sede"></param>
        /// <param name="risultatoProtocollazione"></param>
        /// <returns>True/False Creato protocollo</returns>
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
        /// <param name="risultatoProtocollazione">Parametro output: Esito pi� dettagliato dell'operazione</param>
        /// <param name="ruoliSuperiori"></param>
        /// <returns>Boolean, l'operazione di protocollazione � andata a buon fine o meno</returns>
        public bool AggiornaProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
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

        public bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string copiaDoc = null)
        {
            logger.Info("BEGIN");
            //Performance.PerformanceLogWriter.CreatePerformanceLogWriter(@"c:\temp\DocumentManager.txt");

            //Performance.PerformanceLogWriter log = new Performance.PerformanceLogWriter();
            //log.StartLogEntry("CreateProtocollo");
            logger.Debug("(SP) - inizio metodo di protocollazione semplice");
            bool retValue = false;

            risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;
            ruoliSuperiori = null;

            try
            {
                retValue = this.DocumentManagerSP.CreateProtocollo(schedaDocumento, ruolo, out risultatoProtocollazione, out ruoliSuperiori);
                //log.StartLogEntry("CreateProtocollo_DCTM", true);
                if (retValue)
                    retValue = this.DocumentManagerETDOCS.CreateProtocollo(schedaDocumento, ruolo, out risultatoProtocollazione, out ruoliSuperiori);
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
            logger.Debug("(SP) - fine metodo di protocollazione semplice");
            logger.Info("END");
            return retValue;

        }

        /// <summary>
        /// Save delle modifiche apportate al documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ufficioReferenteEnabled"></param>
        /// <param name="ufficioReferenteSaved"></param>
        /// <returns>True/False</returns>
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
        /// <returns>True/False</returns>
        public bool AddVersion(DocsPaVO.documento.FileRequest fileRequest, bool daInviare)
        {
            logger.Info("BEGIN");
            bool retValue = false;

            try
            {
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    retValue = this._documentManagerSP.AddVersion(fileRequest, daInviare);



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
        /// <returns>True/False</returns>
        public bool AddAttachment(DocsPaVO.documento.Allegato allegato, string putfile)
        {
            bool retValue = false;

            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                retValue = this._documentManagerSP.AddAttachment(allegato, putfile);



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
                this._documentManagerSP.ModifyAttatchment(allegato);
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

                retValue = ((DocumentManagerSP)this._documentManagerSP).Remove(new DocsPaVO.documento.InfoDocumento[1] { infoDocumentoAllegato }, false);

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
                    retValue = this._documentManagerSP.RemoveVersion(fileRequest);



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
            DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
            List<BaseInfoDoc> listaSchedeDoc = documenti.GetBaseInfoForDocument(null, docNumber, null);
            BaseInfoDoc schedaDocBase = listaSchedeDoc[0];
            SchedaDocumento schedaDocumento = documenti.GetDettaglioNoSecurity(this._infoUtente, schedaDocBase.IdProfile, schedaDocBase.DocNumber);

            try
            {
                if (!string.IsNullOrEmpty(_infoUtente.codWorkingApplication) && _infoUtente.codWorkingApplication == "DOCSPA")
                {
                    content = this.DocumentManagerSP.GetFile(docNumber, version, versionId, versionLabel);
                }
                else
                    // Il reperimento dei file viene dapprima effettuato dal documentale ETNOTEAM
                    content = this.DocumentManagerETDOCS.GetFile(docNumber, version, versionId, versionLabel);

            }
            catch (Exception ex)
            {
                logger.Debug("Errore nel reperimento del file nel documentale ETNOTEAM", ex);

                // Errore nel reperimento del file nel documentale ETNOTEAM
                content = null;
            }

            logger.Info("END");

            return content;
        }

        /// <summary>
        /// Operazione per il reperimento di un file di una versione / allegato di un documento
        /// 
        /// PreCondizioni:
        ///     la versione / allegato deve essere gi� stato creato come entit� persistente
        ///     il file deve essere gi� acquisito nel repository documentale
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
            DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
            List<BaseInfoDoc> listaSchedeDoc = documenti.GetBaseInfoForDocument(null, fileRequest.docNumber, null);
            BaseInfoDoc schedaDocBase = listaSchedeDoc[0];
            SchedaDocumento schedaDocumento = documenti.GetDettaglioNoSecurity(this._infoUtente, schedaDocBase.IdProfile, schedaDocBase.DocNumber);

            try
            {
                if (!string.IsNullOrEmpty(_infoUtente.codWorkingApplication) && _infoUtente.codWorkingApplication == "DOCSPA")
                //Devo verificare che il path del doc non sia di tipo vecchio, se è vecchio giro a ETDOCS.
                //porcata:
                {
                    //Qui devo trovare un modo elegante per farlo, per ora faccio così.
                    string sSPURL = System.Configuration.ConfigurationSettings.AppSettings["SP_Server"];
                    if (fileRequest.fileName.ToUpper().Contains(DocsPaDocumentale_SP.WsSPServices.CallerSP.GetServerRootnosites().ToUpper()) || fileRequest.path.ToUpper().Contains(DocsPaDocumentale_SP.WsSPServices.CallerSP.GetServerRootnosites().ToUpper()))
                        retValue = this.DocumentManagerSP.GetFile(ref fileDocumento, ref fileRequest);
                    else
                        retValue = this.DocumentManagerETDOCS.GetFile(ref fileDocumento, ref fileRequest);
                }
                else
                    retValue = this.DocumentManagerETDOCS.GetFile(ref fileDocumento, ref fileRequest);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nel reperimento del file nel documentale ETNOTEAM", ex);

                // Errore nel reperimento del file nel documentale ETNOTEAM
                retValue = false;
            }

            logger.Info("END");
            return retValue;
        }

        /// <summary>
        /// Operazione per l'inserimento di un file in una versione / allegato di un documento
        /// 
        /// PreCondizione:
        ///     la versione / allegato deve essere gi� stato creato come entit� persistente
        /// 
        ///     Attributi dell'oggetto FileRequest gi� valorizzati in ingresso
        ///         VersionId:      ID della versione / allegato in docspa
        ///         Version:        0 se allegato, > 0 se versione
        ///         SubVersion:     ! se allegato, stringa vuota se versione
        ///         VersionLabel:   A# se allegato (dove # � il num. dell'allegato, fino a 99)
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

            //CONTROLLO SULL?APP CHE MI CHIAMA!!!!!!!!!!!!!!!!
            DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
            List<BaseInfoDoc> listaSchedeDoc = documenti.GetBaseInfoForDocument(null, fileRequest.docNumber, null);
            BaseInfoDoc schedaDocBase = listaSchedeDoc[0];
            SchedaDocumento schedaDocumento = documenti.GetDettaglioNoSecurity(this._infoUtente, schedaDocBase.IdProfile, schedaDocBase.DocNumber);
            //FINE
            try
            {
                //************************************
                // _infoUtente.codWorkingApplication *
                //************************************
                if (!string.IsNullOrEmpty(_infoUtente.codWorkingApplication) && _infoUtente.codWorkingApplication == "DOCSPA")
                    //CHIAMATA AL DOCUMENTALE SHAREPOINT
                    retValue = this.DocumentManagerSP.PutFile(fileRequest, fileDocumento, estensione);
                else
                    //CHIAMATA AL DOCUMENTALE ETDOCS 
                    retValue = this.DocumentManagerETDOCS.PutFile(fileRequest, fileDocumento, estensione);
            }
            catch (Exception ex)
            {

                logger.Debug(string.Format("Errore nel metodo PutFile: {0}", ex.Message));
                logger.Debug(string.Format("Errore durante la scrittura del documento: {0}, non è stato possibile invocare il WebService di INPS", ex.Message));
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
            // in quanto � il gestore dei dati sulle versioni
            return this._documentManagerSP.GetLatestVersionId(docNumber);
        }

        /// <summary>
        /// Modifica le estensioni del file recuperato
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
                retValue = this.DocumentManagerSP.ModifyExtension(ref fileRequest, docNumber, version_id, version, subVersion, versionLabel);
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
                retValue = this.DocumentManagerSP.AnnullaProtocollo(ref schedaDocumento, protocolloAnnullato);


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
                this.DocumentManagerSP.ModifyVersion(fileRequest);
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
            return this.DocumentManagerSP.ModifyVersionSegnatura(versionId);
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
            return this.DocumentManagerSP.IsVersionWithSegnature(versionId);
        }

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
                retValue = this.DocumentManagerSP.AddDocumentoInCestino(infoDocumento);


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
                retValue = this.DocumentManagerSP.RestoreDocumentoDaCestino(infoDocumento);


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
                retValue = this.DocumentManagerSP.ScambiaAllegatoDocumento(allegato, documento);


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
                // Per PITRE, questo non � necessario in quanto � DCTM che gestisce
                // la cancellazione dei file.
                foreach (DocsPaVO.documento.InfoDocumento infoDocumento in items)
                {
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                    retValue = doc.EliminaDoc(this.InfoUtente, infoDocumento);

                    if (!retValue)
                        break;
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
            return this.DocumentManagerSP.GetFileExtension(docnumber, versionid);
        }

        /// <summary>
        /// Reperimento del nome originale del file nel documentale
        /// </summary>
        /// <param name="docnumber"></param>
        /// <param name="versionid"></param>
        /// <returns></returns>
        public string GetOriginalFileName(string docnumber, string versionid)
        {
            return this.DocumentManagerSP.GetOriginalFileName(docnumber, versionid);
        }

        /// <summary>
        /// Impostazione di un permesso su un documento
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        public bool AddPermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            bool retValue = this.DocumentManagerSP.AddPermission(infoDiritto);



            return retValue;
        }

        /// <summary>
        /// Revoca di un permesso su un documento
        /// </summary>
        /// <param name="documentInfo"></param>
        /// <returns></returns>
        public bool RemovePermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            bool retValue = this.DocumentManagerSP.RemovePermission(infoDiritto);



            return retValue;
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
        /// Documentale SharePoint
        /// </summary>
        protected IDocumentManager DocumentManagerSP
        {
            get
            {
                return this._documentManagerSP;
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
