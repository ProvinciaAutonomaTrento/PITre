using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.documento;
using DocsPaVO.utente;
using DocsPaUtils.LogsManagement;
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
using log4net;
using System.Net;


namespace DocsPaDocumentale_SP.Documentale
{
    /// <summary>
    /// Iacozzilli Giordano 08/10/2012
    /// Implemento l'interfaccia IdocumentManager di DocsPa
    /// Per usare i mie Put e Get del documento sotto Sharepoint2010.
    /// </summary>
    public class DocumentManager : DocsPaDocumentale.Interfaces.IDocumentManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocumentManager));
        protected DocsPaVO.utente.InfoUtente _infoUtente;
        #region Costruttori

        protected DocumentManager()
        {

        }

        /// <summary>
        /// Inizializza l'istanza della classe acquisendo i dati relativi all'utente 
        /// ed alla libreria per la connessione al documentale.
        /// </summary>
        /// <param name="infoUtente">Dati relativi all'utente</param>
        public DocumentManager(DocsPaVO.utente.InfoUtente infoUtente)
        {
            _infoUtente = infoUtente;
        }

        #endregion

        #region Metodi

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
                schedaDocumento.systemId = this.CreateDocument(schedaDocumento);
                schedaDocumento.docNumber = schedaDocumento.systemId;

                retValue = (!string.IsNullOrEmpty(schedaDocumento.systemId));

                if (retValue)
                {
                    using (DocsPaDB.Query_DocsPAWS.Report dbReport = new DocsPaDB.Query_DocsPAWS.Report())
                        dbReport.UpdProf(schedaDocumento, schedaDocumento.registro.systemId, schedaDocumento.docNumber);

                    using (DocsPaDB.Query_DocsPAWS.Documenti dbDocument = new DocsPaDB.Query_DocsPAWS.Documenti())
                        dbDocument.GetProfile(this._infoUtente, ref schedaDocumento);

                    // Impostazione visibilità documento ai ruoli superiori al ruolo corrente
                    using (DocsPaDB.Query_DocsPAWS.Documenti dbDocumenti = new DocsPaDB.Query_DocsPAWS.Documenti())
                        schedaDocumento = dbDocumenti.SetDocTrustees(schedaDocumento, ruolo, out ruoliSuperiori, null);
                }
            }
            catch (Exception exception)
            {
                retValue = false;

                logger.Debug("Errore nella creazione del documento per la stampa registro.", exception);
            }

            return retValue;
        }

        /// <summary>
        /// Acquisisce un documento
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="version"></param>
        /// <param name="versionId"></param>
        /// <param name="versionLabel"></param>
        /// <returns>
        /// Documento in formato binario o 'null' se si è verificato un errore.
        /// </returns>
        public byte[] GetFile(string docNumber, string version, string versionId, string versionLabel)
        {
            logger.Info("BEGIN");
            byte[] fileCont = null;

            try
            {
                DocsPaVO.documento.FileRequest fileRequest = new DocsPaVO.documento.FileRequest();
                DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento();

                fileRequest.versionId = versionId;
                this.GetFile(ref fileDocumento, ref fileRequest);

                fileCont = fileDocumento.content;
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante la GET(byte[]) di un documento.", exception);
            }
            logger.Info("END");
            return fileCont;
        }

        /// <summary>
        /// </summary>
        /// <param name="fileDocumento"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public bool GetFile(ref DocsPaVO.documento.FileDocumento fileDocumento, ref DocsPaVO.documento.FileRequest fileRequest)
        {
            logger.Info("BEGIN");
            bool result = true;

            try
            {
                DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();

                string version_id = fileRequest.versionId;

                string filePath = documentale.GetFileName(version_id);
                //Lo split finale serve per capire in quale site collection andare a prendere il file
                //lo spli -3 è dato da 1 (estensione file) 2 (Nome File) 3 (Nome Library), potevo ricavarlo 
                //ma diventava complicato in caso di parti di Anno (semestre, trimestre ecc).
                fileDocumento.content = WsSPServices.CallerSP.GetFileFromSP(filePath, filePath.Split('/')[filePath.Split('/').Length - 3]);

                fileDocumento.length = fileDocumento.content.Length;
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante la GET(bool) di un documento.", exception);
            }

            logger.Info("END");
            return result;

        }

        /// <summary>
        /// Crea un nuovo documento
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <returns></returns>
        protected virtual string CreateDocument(DocsPaVO.documento.SchedaDocumento schedaDoc)
        {
            string docNumber = null;

            try
            {
                DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();

                docNumber = documentale.CreateDocumentSP(schedaDoc);

                logger.DebugFormat("Documento creato con succeso. Numero documento {0}", schedaDoc.docNumber);

                if (docNumber != null)
                {
                    //aggiorna flag daInviare
                    string firstParam = "CHA_DA_INVIARE = '1'";

                    if (schedaDoc.documenti != null && schedaDoc.documenti.Count > 0)
                    {
                        int lastDocNum = schedaDoc.documenti.Count - 1;

                        if (((DocsPaVO.documento.Documento)schedaDoc.documenti[lastDocNum]).dataArrivo != null && !((DocsPaVO.documento.Documento)schedaDoc.documenti[lastDocNum]).dataArrivo.Equals(""))
                        {
                            firstParam += ", DTA_ARRIVO =" + DocsPaDbManagement.Functions.Functions.ToDate(((DocsPaVO.documento.Documento)schedaDoc.documenti[lastDocNum]).dataArrivo);
                        }
                    }

                    logger.DebugFormat("Aggiornamento informazioni versione per il documento con numero {0}", schedaDoc.docNumber);
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                    doc.UpdateVersions(firstParam, docNumber);
                    logger.DebugFormat("Aggiornamento informazioni versione per il documento con numero {0} riuscito correttamente.", schedaDoc.docNumber);
                }
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante la creazione di un documento.", exception);
                docNumber = null;
            }

            return docNumber;
        }

        /// <summary>
        /// Reperimento versionlabel per l'allegato
        /// </summary>
        /// <param name="docnumber"></param>
        /// <returns></returns>
        protected virtual string GetAllegatoVersionLabel(string docnumber)
        {
            DocsPaDB.Query_DocsPAWS.Documenti obj = new DocsPaDB.Query_DocsPAWS.Documenti();

            return obj.GetVersionLabelAllegato(docnumber, true);
        }

        /// <summary>
        /// </summary>
        /// <param name="allegato"></param>
        /// <returns></returns>
        public bool AddAttachment(DocsPaVO.documento.Allegato allegato, string putfile)
        {
            bool retValue = false;

            try
            {
                //Questo è per la chiamata dal modify allegato!!!
                if (!string.IsNullOrEmpty(putfile) && putfile == "Y")
                {
                    // Se putfile == "Y", l'operazione si presume
                    // come andata a buon fine in quanto nessun
                    // attributo è da modificare
                    retValue = true;
                }
                else
                {
                    // Creazione contesto transazionale.
                    // NB: IL CONTESTO VIENE CREATO SOLO SE NON NE E' STATO GIA' CREATO
                    // UNO AD UN LIVELLO APPLICATIVO PIU' ALTO
                    using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                    {
                        // Reperimento VersionLabel da associare all'allegato
                        allegato.versionLabel = this.GetAllegatoVersionLabel(allegato.docNumber);

                        //Version ID
                        allegato.versionId = this.GetLatestVersionId(allegato.docNumber);
                        // Creazione di un oggetto scheda documento per l'allegato da creare
                        DocsPaVO.documento.SchedaDocumento schedaDocumento = new DocsPaVO.documento.SchedaDocumento();

                        // Associazione del documento principale dell'allegato
                        DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                        schedaDocumento.documentoPrincipale = documenti.GetInfoDocumento(this._infoUtente.idGruppo, this._infoUtente.idPeople, allegato.docNumber, false);

                        schedaDocumento.tipoProto = "G"; // Creazione dell'allegato come documento grigio
                        schedaDocumento.oggetto = new DocsPaVO.documento.Oggetto();
                        schedaDocumento.oggetto.descrizione = allegato.descrizione;
                        schedaDocumento.userId = this._infoUtente.userId;
                        schedaDocumento.idPeople = this._infoUtente.idPeople;
                        schedaDocumento.privato = schedaDocumento.documentoPrincipale.privato;
                        schedaDocumento.personale = schedaDocumento.documentoPrincipale.personale;

                        DocsPaDB.Query_DocsPAWS.Utenti utentiDb = new DocsPaDB.Query_DocsPAWS.Utenti();
                        DocsPaVO.utente.Ruolo ruolo = utentiDb.GetRuolo(this._infoUtente.idCorrGlobali, false);
                        schedaDocumento.creatoreDocumento = new DocsPaVO.documento.CreatoreDocumento(this._infoUtente, ruolo);
                        schedaDocumento.protocollatore = new DocsPaVO.documento.Protocollatore(this._infoUtente, ruolo);

                        // creazione nuovo documento per allegato
                        DocsPaDB.Query_DocsPAWS.Documenti documentiDb = new DocsPaDB.Query_DocsPAWS.Documenti();

                        // Inserimento dei dati dell'allegato
                        int versionId;
                        documentiDb.CreateAllegato(this._infoUtente, schedaDocumento, allegato.descrizione, allegato.numeroPagine, allegato.ForwardingSource, out versionId);

                        retValue = (!string.IsNullOrEmpty(schedaDocumento.systemId));

                        if (retValue)
                        {
                            allegato.docNumber = schedaDocumento.docNumber;
                            allegato.versionId = versionId.ToString();

                            // Ricerca filename
                            DataSet ds;
                            documentiDb.GetPath(out ds, allegato.versionId, allegato.docNumber);
                            allegato.fileName = ds.Tables["PATH"].Rows[0]["PATH"].ToString();
                        }

                        if (retValue)
                            transactionContext.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(ex);
            }

            return retValue;
        }

        /// <summary>
        /// Modifica di un allegato
        /// </summary>
        /// <param name="allegato"></param>
        public void ModifyAttatchment(DocsPaVO.documento.Allegato allegato)
        {
            DocsPaDB.Query_DocsPAWS.Documenti obj = new DocsPaDB.Query_DocsPAWS.Documenti();
            obj.ModificaAllegato(allegato, this._infoUtente, true);
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
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="ruoliSuperiori"></param>
        /// <returns></returns>
        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string conCopia = null)
        {
            ruoliSuperiori = null;
            return true;
        }

        /// <summary>
        /// Predisposizione di un documento alla protocollazione
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public bool PredisponiProtocollazione(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            return true;
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
            risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;
            bool retValue = true;
            logger.DebugFormat("Protocollazione documento predisposto numero {0}", schedaDocumento.docNumber);
            try
            {
                retValue = this.CreateProtocollo(schedaDocumento, ruolo, out risultatoProtocollazione);

            }
            catch (Exception ex)
            {
                risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.APPLICATION_ERROR;

                string msg = string.Format("Errore nella protocollazione del documento predisposto numero {0}: {1}", schedaDocumento.docNumber, ex.Message);
                logger.Debug(msg);
                throw new ApplicationException(msg, ex);
            }

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
            ufficioReferenteSaved = false;
            return false;
        }

        /// <summary>
        /// Rimozione di un allegato
        /// </summary>
        /// <remarks>
        /// Con l'introduzione della nuova gestione,
        /// l'allegato verrà inserito nel cestino
        /// </remarks>
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

                retValue = this.Remove(dbDocumenti.GetInfoDocumento(this._infoUtente.idGruppo, this._infoUtente.idPeople, allegato.docNumber, false));

                if (retValue)
                    transactionContext.Complete();
            }

            return retValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public bool RemoveVersion(DocsPaVO.documento.FileRequest fileRequest)
        {
            return false;
        }

        /// <summary>
        /// Metodo che restituisce la directory dove salvare il documento, esclusa la doc root. Verifica che la Doc Root
        /// esista, nel caso contrario restituisce null
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        string GetDocPath(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente objSicurezza)
        {
            string result = null;
            try
            {
                if (fileRequest != null)
                {
                    string filePath = "";

                    //legge il record da DPA_CORR_GLOBALI in JOIN con DPA_AMMINISTRA
                    DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
                    System.Data.DataSet corrispondente;
                    if (!documentale.DOC_GetCorrByIdPeople(fileRequest.idPeople, out corrispondente))
                    {
                        logger.Debug("Errore nella lettura del corrispondente relativo al documento");
                        throw new Exception();
                    }
                    //legge l'amministrazione
                    string amministrazione = corrispondente.Tables[0].Rows[0]["VAR_CODICE_AMM"].ToString();
                    //legge l'id della uo di appartenenza del gruppo
                    string id = documentale.DOC_GetIdUoBySystemId(objSicurezza.idGruppo);
                    if (id == null)
                    {
                        logger.Debug("Errore nella lettura del gruppo relativo al documento");
                        throw new Exception();
                    }
                    //recupera il nome della UO
                    string codiceUO = documentale.DOC_GetUoById(id);
                    //legge la tabella profile
                    System.Data.DataSet documento;
                    if (!documentale.DOC_GetDocByDocNumber(fileRequest.docNumber, out documento))
                    {
                        logger.Debug("Errore nella lettura del documento: " + fileRequest.docNumber);
                        throw new Exception();
                    }
                    //legge l'anno di creazione del documento
                    string anno = System.DateTime.Parse(documento.Tables[0].Rows[0]["CREATION_DATE"].ToString()).Year.ToString();
                    //verifica se il documento è protocollato
                    string tipoProtocollo;
                    tipoProtocollo = documento.Tables[0].Rows[0]["CHA_TIPO_PROTO"].ToString().ToUpper();
                    //se A -> protocollo in arrivo; se P -> protocollo in partenza ; tutto il resto -> non protocollato
                    string registro = "";
                    if (tipoProtocollo == "A" || tipoProtocollo == "P" || tipoProtocollo == "I")
                    {
                        //crea il path nel caso di documento protocollato -> AMMINISTRAZIONE + REGISTRO + ANNO + [COD_UO] + [ARRIVO|PARTENZA]

                        //legge il registro della protocollazione
                        registro = documentale.DOC_GetRegistroById(documento.Tables[0].Rows[0]["ID_REGISTRO"].ToString());
                        if (registro == null)
                        {
                            logger.Debug("Errore nella lettura del registro");
                            throw new Exception();
                        }
                        /*
                            <add key="DOC_CODICE_UFFICIO" value="1"/>  
                            <add key="DOC_ARRIVO_PARTENZA" value="1"/>  
                        */
                        filePath += amministrazione + "\\" + anno + "\\" + registro;
                        if (System.Configuration.ConfigurationManager.AppSettings["DOC_CODICE_UFFICIO"] != null)
                        {
                            if (System.Configuration.ConfigurationManager.AppSettings["DOC_CODICE_UFFICIO"] == "1")
                            {
                                filePath += "\\" + codiceUO;
                            }
                        }
                        if (System.Configuration.ConfigurationManager.AppSettings["DOC_ARRIVO_PARTENZA"] != null)
                        {
                            if (System.Configuration.ConfigurationManager.AppSettings["DOC_ARRIVO_PARTENZA"] == "1")
                            {
                                if (tipoProtocollo == "A")
                                {
                                    filePath += "\\Arrivo";
                                }
                                if (tipoProtocollo == "P")
                                {
                                    filePath += "\\Partenza";
                                }
                                if (tipoProtocollo == "I")
                                {
                                    filePath += "\\Interno";
                                }
                            }
                        }
                    }
                    else
                    {
                        //crea il path nel caso di documento non protocollato -> AMMINISTRAZIONE + ANNO + [COD_UO]
                        filePath += amministrazione + "\\" + anno;
                        if (System.Configuration.ConfigurationManager.AppSettings["DOC_CODICE_UFFICIO"] != null)
                        {
                            if (System.Configuration.ConfigurationManager.AppSettings["DOC_CODICE_UFFICIO"] == "1")
                            {
                                filePath += "\\" + codiceUO;
                            }
                        }
                    }

                    //verifica se la directory esiste
                    DirectoryInfo docFullPath = new DirectoryInfo(System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"] + "\\" + filePath);
                    if (!docFullPath.Exists)
                    {
                        //crea la directory
                        docFullPath.Create();
                    }

                    //restituisce la directory
                    result = filePath;

                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore creazione path documentale per documento: " + fileRequest.docNumber, e);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="objSicurezza"></param>
        /// <returns></returns>
        protected virtual string GetDocPathAdvanced(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente objSicurezza)
        {
            string result = null;

            try
            {
                if (fileRequest != null)
                {
                    string filePath = "";

                    //legge il record da DPA_CORR_GLOBALI in JOIN con DPA_AMMINISTRA
                    DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
                    System.Data.DataSet corrispondente;
                    if (documentale.DOC_GetCorrByIdPeople(objSicurezza.idPeople, out corrispondente))
                    {
                        //logger.Debug ("Errore nella lettura del corrispondente relativo al documento");
                        //throw new Exception();
                    }
                    //legge l'amministrazione
                    string amministrazione = corrispondente.Tables[0].Rows[0]["VAR_CODICE_AMM"].ToString();

                    //legge l'id della uo di appartenenza del gruppo
                    string id = documentale.DOC_GetIdUoBySystemId(objSicurezza.idGruppo);
                    if (id == null)
                    {
                        logger.Debug("Errore nella lettura del gruppo relativo al documento");
                        throw new Exception();
                    }
                    //recupera il nome della UO
                    string codiceUO = documentale.DOC_GetUoById(id);
                    //legge la tabella profile
                    System.Data.DataSet documento;
                    if (!documentale.DOC_GetDocByDocNumber(fileRequest.docNumber, out documento))
                    {
                        logger.Debug("Errore nella lettura del documento: " + fileRequest.docNumber);
                        throw new Exception();
                    }
                    //legge l'anno di creazione del documento
                    //string anno=System.DateTime.Parse(documento.Tables[0].Rows[0]["CREATION_DATE"].ToString()).Year.ToString();
                    string anno = documento.Tables[0].Rows[0]["CREATION_YEAR"].ToString();
                    //verifica se il documento è protocollato
                    string tipoProtocollo;
                    tipoProtocollo = documento.Tables[0].Rows[0]["CHA_TIPO_PROTO"].ToString().ToUpper();
                    //se A -> protocollo in arrivo; se P -> protocollo in partenza ; tutto il resto -> non protocollato
                    string registro = "";
                    string arrivoPartenza = "";
                    if (tipoProtocollo == "A" || tipoProtocollo == "P" || tipoProtocollo == "I")
                    {
                        //crea il path nel caso di documento protocollato -> AMMINISTRAZIONE + REGISTRO + ANNO + [COD_UO] + [ARRIVO|PARTENZA]

                        //legge il registro della protocollazione
                        registro = documentale.DOC_GetRegistroById(documento.Tables[0].Rows[0]["ID_REGISTRO"].ToString());
                        if (registro == null)
                        {
                            logger.Debug("Errore nella lettura del registro");
                            registro = "";
                        }

                        if (tipoProtocollo == "A")
                        {
                            arrivoPartenza = "Arrivo";
                        }
                        if (tipoProtocollo == "P")
                        {
                            arrivoPartenza = "Partenza";
                        }
                        if (tipoProtocollo == "I")
                        {
                            arrivoPartenza = "Interno";
                        }
                    }

                    filePath = System.Configuration.ConfigurationManager.AppSettings["DOC_PATH"];
                    if (filePath == null) filePath = "";

                    filePath = filePath.Replace("AMMINISTRAZIONE", amministrazione);
                    filePath = filePath.Replace("REGISTRO", registro);
                    filePath = filePath.Replace("ANNO", anno);
                    filePath = filePath.Replace("ARRIVO_PARTENZA", arrivoPartenza);
                    filePath = filePath.Replace("UFFICIO", codiceUO);
                    filePath = filePath.Replace("UTENTE", objSicurezza.userId);
                    filePath = filePath.ToUpper().Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));

                    filePath = filePath.Replace(@"\\", @"\");
                    if (filePath.EndsWith(@"\"))
                    {
                        filePath = filePath.Remove(filePath.Length - 1, 1);
                    }

                    //verifica se la directory esiste
                    string appo = @System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"] + "\\" + filePath;
                    DirectoryInfo docFullPath = new DirectoryInfo(appo);


                    if (!docFullPath.Exists)
                    {
                        //crea la directory
                        docFullPath.Create();
                    }

                    //restituisce la directory
                    result = filePath;

                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore creazione path documentale per documento: " + fileRequest.docNumber, e);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Inserimento dei soli metadati del file nel sistema documentale
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="fileDocumento"></param>
        /// <param name="estensione"></param>
        /// <returns></returns>
        /// <remarks>
        /// Non inserisce il contenuto fisico del file
        /// </remarks>
        public bool PutFileMetadata(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocumento, string estensione)
        {
            logger.Info("BEGIN");
            bool retValue = false;

            try
            {
                // Calcolo impronta sul contenuto del file
                string printThumb = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(fileDocumento.content);

                // Aggiornamento database
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    ArrayList parameters = new ArrayList();

                    parameters.Add(new DocsPaUtils.Data.ParameterSP("versionId", fileRequest.versionId, 0, DocsPaUtils.Data.DirectionParameter.ParamInput));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("filePath", fileRequest.path, 500, DocsPaUtils.Data.DirectionParameter.ParamInput));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("fileSize", fileDocumento.content.Length, 0, DocsPaUtils.Data.DirectionParameter.ParamInput));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("printThumb", printThumb, 64, DocsPaUtils.Data.DirectionParameter.ParamInput));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("iscartaceo", (fileRequest.cartaceo ? 1 : 0), 0, DocsPaUtils.Data.DirectionParameter.ParamInput));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("estensione", estensione, 0, DocsPaUtils.Data.DirectionParameter.ParamInput));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("isFirmato", fileRequest.firmato, 0, DocsPaUtils.Data.DirectionParameter.ParamInput));

                    retValue = (dbProvider.ExecuteStoredProcedure("putFile", parameters, null) > 0);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore durante la scrittura del documento: {0}", ex.Message));

                retValue = false;
            }
            logger.Info("END");
            return retValue;
        }

        /// <summary>
        /// Inserimento file nel sistema documentale
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="fileDocumento"></param>
        /// <param name="estensione"></param>
        /// <returns></returns>
        public bool PutFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocumento, string estensione)
        {

            logger.Info("BEGIN");
            bool retValue = true;
            try
            {
                //recupero dal db le informazioni base relative al documento
                DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                List<BaseInfoDoc> listaSchedeDoc = documenti.GetBaseInfoForDocument(null, fileRequest.docNumber, null);
                BaseInfoDoc schedaDocBase = listaSchedeDoc[0];
                SchedaDocumento schedaDocumento = documenti.GetDettaglioNoSecurity(this._infoUtente, schedaDocBase.IdProfile, schedaDocBase.DocNumber);

                logger.DebugFormat("(SP) - inizio metodo di associazione immagine per il documento numero {0}", fileRequest.docNumber);

                // Reperimento cartella documentale in cui inserire il file
                string relativeDocPath = WsSPServices.CallerSP.GetPathAbsolute(DateTime.Now.Year.ToString(), _infoUtente.codWorkingApplication).Replace(@"/", "\\");

                if (string.IsNullOrEmpty(relativeDocPath))
                    throw new ApplicationException("Impossibile costruire il percorso relativo del file nel documentale");

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

                //Devo aggfiungere il prefisso DOCSPA_
                fileName = string.Format("{0}_{1}", _infoUtente.codWorkingApplication, fileName);

                fileRequest.path = (WsSPServices.CallerSP.GetPathAbsolute(DateTime.Now.Year.ToString(), _infoUtente.codWorkingApplication) + fileName).Replace("http://", "");

                // fileRequest.path = (WsSPServices.CallerSP.GetPathAbsolute(DateTime.Now.Year.ToString(), _infoUtente.codWorkingApplication) + fileName).Replace("http://", "");
                //Faccio la get del version id del doc principale dell'allegato (il padre dell'allegato)
                string versionIdPadre = string.Empty;
                if (schedaDocumento.documentoPrincipale != null && schedaDocumento.documentoPrincipale.docNumber != "")
                {
                    versionIdPadre = this.GetLatestVersionId(schedaDocumento.documentoPrincipale.docNumber);
                }

                retValue = WsSPServices.CallerSP.PutFileInSP(ref fileDocumento, ref schedaDocumento, _infoUtente, ref fileRequest, estensione);


                bool retPut = this.PutFileMetadata(fileRequest, fileDocumento, estensione);

                if (retValue && retPut)
                {
                    // Aggiornamento oggetto FileRequest
                    fileRequest.docServerLoc = WsSPServices.CallerSP.GetLibraryRoot(DateTime.Now.Year.ToString(), _infoUtente.codWorkingApplication).Replace(@"/", "\\").ToUpper();
                    fileRequest.path = @"\" + relativeDocPath.ToUpper();
                    fileRequest.fileName = fileName.ToUpper();
                    fileRequest.fileSize = fileDocumento.content.Length.ToString();
                }
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante la Put(bool) di un documento.", exception);
            }
            logger.Info("END");
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
        public bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string copiaDoc = null)
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
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="daInviare"></param>
        /// <returns></returns>
        public bool AddVersion(DocsPaVO.documento.FileRequest fileRequest, bool daInviare)
        {
            logger.Info("BEGIN");
            bool result = true;

            bool update = false;
            string oldApp = null;

            System.Data.DataSet ds;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

            try
            {
                if (fileRequest.applicazione != null)
                {
                    if (fileRequest.applicazione.systemId == null)
                    {
                        logger.Debug("sysid vuoto");

                        DocsPaVO.documento.Applicazione res = new DocsPaVO.documento.Applicazione();
                        doc.GetExt(fileRequest.applicazione.estensione, ref res);

                        fileRequest.applicazione = res;
                    }
                    logger.Debug("Update della tabella profile");
                    string param = "(APPLICATION is NULL OR APPLICATION != " + fileRequest.applicazione.systemId + ") AND DOCNUMBER=" + fileRequest.docNumber;
                    doc.GetApplication(out oldApp, fileRequest.docNumber, fileRequest.applicazione.systemId, param);

                    update = true;
                }

                DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
                documentale.AddVersion(ref fileRequest, this._infoUtente.idPeople, this._infoUtente.userId);

                //ESTRAZIONE DEL FILENAME, VERSION, LASTEDITTIME
                doc.SetCompVersions(fileRequest.versionId, fileRequest.docNumber, out ds);

                fileRequest.fileName = ds.Tables["VERS"].Rows[0]["PATH"].ToString();
                fileRequest.version = ds.Tables["VERS"].Rows[0]["VERSION"].ToString();
                fileRequest.subVersion = ds.Tables["VERS"].Rows[0]["SUBVERSION"].ToString();
                fileRequest.versionLabel = ds.Tables["VERS"].Rows[0]["VERSION_LABEL"].ToString();
                fileRequest.dataInserimento = ds.Tables["VERS"].Rows[0]["DTA_CREAZIONE"].ToString();
                DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
                string full_name_utente = u.getUtenteById(this._infoUtente.idPeople).descrizione;
                if (full_name_utente != null)
                    fileRequest.autore = full_name_utente;

                //EMosca 29/11/2004
                /*Aggiunto && oldApp!="" nell'if.
                 * oldApp risulta vuoto per tutte le versioni 
                 * (tranne Hummingbird che inserisce di default un pdf di size=0 alla creazione del doc.)
                 */
                if (update && oldApp != "")
                {
                    DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                    documenti.UpdateApplication(oldApp, fileRequest.docNumber);
                }

                DocsPaDB.Query_DocsPAWS.Documenti documenti2 = new DocsPaDB.Query_DocsPAWS.Documenti();
                documenti2.UpdateVersionManager(fileRequest, daInviare);

                logger.Debug("Fine addVersion");
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante l'aggiunta di una versione.", exception);

                if (update)
                {
                    DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                    documenti.UpdateApplication(oldApp, fileRequest.docNumber);
                }

                result = false;
            }
            logger.Info("END");
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public string GetLatestVersionId(string docNumber)
        {
            string versionId = null;

            DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
            versionId = documentale.GetLatestVersionId(docNumber);

            return versionId;
        }

        /// <summary>
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="estensione"></param>
        /// <param name="objSicurezza"></param>
        public bool ModifyExtension(ref DocsPaVO.documento.FileRequest fileRequest,
            string docNumber,
            string version_id,
            string version,
            string subVersion,
            string versionLabel)
        {
            return false;
        }

        /// <summary>
        /// Annullamento di un protocollo
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="protocolloAnnullato"></param>
        /// <returns></returns>
        public bool AnnullaProtocollo(ref DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.documento.ProtocolloAnnullato protocolloAnnullato)
        {
            bool retValue = false;

            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            retValue = doc.AnnullaProtocollo(this._infoUtente.idPeople, schedaDocumento.systemId, ref protocolloAnnullato);

            if (retValue)
                schedaDocumento.protocollo.protocolloAnnullato = protocolloAnnullato;

            return retValue;
        }

        /// <summary>
        /// Modifica dei metadati di una versione
        /// </summary>
        /// <param name="fileRequest"></param>
        public void ModifyVersion(DocsPaVO.documento.FileRequest fileRequest)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.ModificaVersione(fileRequest);
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
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.ModifyVersionSegnatura(versionId);
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
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.IsVersionWithSegnature(versionId);
        }

        /// <summary>
        /// Rimozione di un documento grigio
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public string RemoveDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.ExecRimuoviSchedaMethod(this._infoUtente.idPeople, schedaDocumento);
        }

        /// <summary>
        /// Inserimento di un documento nel cestino
        /// </summary>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        public bool AddDocumentoInCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            bool retValue = false;

            if (DocsPaDB.Query_DocsPAWS.Documenti.isEnabledProfilazioneAllegati && infoDocumento.allegato)
            {
                // Se è attiva la nuova gestione dell'allegato, la rimozione è definitiva
                retValue = this.Remove(infoDocumento);
            }
            else
            {
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                retValue = doc.CestinaDocumento(this._infoUtente.idPeople, infoDocumento.idProfile, infoDocumento.noteCestino);

                if (retValue &&
                    DocsPaDB.Query_DocsPAWS.Documenti.isEnabledProfilazioneAllegati &&
                    !infoDocumento.allegato)
                {
                    // Se risulta abilitata la nuova gestione dell'allegato e se 
                    // il documento che si è inserito nel cestino è principale,
                    // anche gli allegati devono essere inseriti nel cestino
                    foreach (DocsPaVO.documento.Allegato allegato in doc.GetAllegati(infoDocumento.docNumber, string.Empty))
                    {
                        DocsPaVO.documento.InfoDocumento infoDocAllegato = doc.GetInfoDocumento(this._infoUtente.idGruppo, this._infoUtente.idPeople, allegato.docNumber, false);

                        retValue = doc.CestinaDocumento(this._infoUtente.idPeople, infoDocAllegato.idProfile, infoDocAllegato.noteCestino);

                        if (!retValue)
                            break;
                    }
                }
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

            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

            retValue = doc.RiattivaDocumento(infoDocumento.docNumber);

            if (retValue &&
                DocsPaDB.Query_DocsPAWS.Documenti.isEnabledProfilazioneAllegati &&
                !infoDocumento.allegato)
            {
                // Se risulta abilitata la nuova gestione dell'allegato e se 
                // il documento che si è riattivato dal cestino è principale,
                // anche gli allegati devono essere ripristinati
                foreach (DocsPaVO.documento.Allegato allegato in doc.GetAllegati(infoDocumento.docNumber, string.Empty))
                {
                    DocsPaVO.documento.InfoDocumento infoDocAllegato = doc.GetInfoDocumento(this._infoUtente.idGruppo, this._infoUtente.idPeople, allegato.docNumber, false);

                    retValue = this.RestoreDocumentoDaCestino(infoDocAllegato);

                    if (!retValue)
                        break;
                }
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
            DocsPaDB.Query_DocsPAWS.Documenti documentiDb = new DocsPaDB.Query_DocsPAWS.Documenti();

            return documentiDb.ScambiaAllegatoDocumento(this._infoUtente, allegato, documento);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="removeFile"></param>
        /// <returns></returns>
        public virtual bool Remove(DocsPaVO.documento.InfoDocumento[] items, bool removeFile)
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

                    retValue = doc.EliminaDoc(this._infoUtente, infoDocumento);

                    if (!retValue)
                        break;
                }

                if (retValue)
                {
                    try
                    {
                        // retValue = this.DocumentManagerDocumentum.Remove(items);
                    }
                    catch (Exception ex1)
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
        /// Rimozione documenti
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public bool Remove(params DocsPaVO.documento.InfoDocumento[] items)
        {
            return this.Remove(items, true);
        }

        private DocsPaVO.documento.InfoDocumento GetInfoDocumento(DataRow row)
        {
            //Creiamo l'oggetto che restituiremo
            DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();

            //Popoliamo l'oggetto
            infoDoc.idProfile = row["SYSTEM_ID"].ToString();
            infoDoc.docNumber = row["DOCNUMBER"].ToString();
            infoDoc.numProt = row["NUM_PROTO"].ToString();
            infoDoc.dataApertura = row["DATA"].ToString();
            infoDoc.tipoProto = row["A_P"].ToString();
            //ArrayList mitt = this.GetElencoCorrispondenti(infoDoc.idProfile);
            //infoDoc.mittDest = mitt;

            infoDoc.idRegistro = row["ID_REGISTRO"].ToString();
            infoDoc.codRegistro = string.Empty;

            //			if (infoDoc.idRegistro.Length>0)
            //				infoDoc.codRegistro = row["codRegistro"].ToString();

            infoDoc.oggetto = row["OGGETTO"].ToString();
            infoDoc.segnatura = row["VAR_SEGNATURA"].ToString();

            return infoDoc;
        }

        public string GetFileExtension(string docnumber, string versionid)
        {
            return null;
        }

        public string GetOriginalFileName(string docnumber, string versionid)
        {
            return null;
        }

        /// <summary>
        /// Impostazione di un permesso su un documento
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        public bool AddPermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.RipristinaACL(infoDiritto, infoDiritto.soggetto.tipoCorrispondente, this._infoUtente);
        }

        /// <summary>
        /// Revoca di un permesso su un documento
        /// </summary>
        /// <param name="documentInfo"></param>
        /// <returns></returns>
        public bool RemovePermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.EditingACL(infoDiritto, infoDiritto.soggetto.tipoCorrispondente, this._infoUtente);
        }

        /// <summary>
        /// Metodo per l'assegnazione di un diritto di tipo A ad un ruolo
        /// </summary>
        /// <param name="rights">Informazioni sul diritto da assegnare</param>
        /// <returns>True se è andato bene</returns>
        public bool AddPermissionToRole(DirittoOggetto rights)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.AddPermissionToRole(rights);
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
    }
}
