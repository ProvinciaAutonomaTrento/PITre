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
using DocsPaDocumentale_WSPIA.WsPiaServices;
using log4net;

namespace DocsPaDocumentale_WSPIA.Documentale
{
    public class DocumentManager : DocsPaDocumentale.Interfaces.IDocumentManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocumentManager));
        protected DocsPaVO.utente.InfoUtente _infoUtente;

        #region Costruttori
        /// <summary>
        /// </summary>
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

        ///// <summary>
        ///// Distrugge il documento dell'istanza della classe precedentemente creato.
        ///// </summary>
        ///// <returns></returns>
        //public bool DestroyNewDocument()
        //{
        //    return true;
        //}

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
            logger.Info("GetFile - BEGIN");
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
                logger.Debug("Errore durante l'acquisizione di un documento.", exception);

                fileCont = null;
            }
            logger.Info("GetFile - END");
            return fileCont;
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


            return retValue;
        }

        /// <summary>
        /// Modifica di un allegato
        /// </summary>
        /// <param name="allegato"></param>
        public void ModifyAttatchment(DocsPaVO.documento.Allegato allegato)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo)
        {
            return false;
        }

        //Emilio 26 Ottobre
        /// <summary>
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="ruoliSuperiori"></param>
        /// <returns></returns>
        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string conCopia = null)
        {
            ruoliSuperiori = null;
            return false;
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
            return false;
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
        /// </summary>
        /// <param name="fileDocumento"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public bool GetFile(ref DocsPaVO.documento.FileDocumento fileDocumento, ref DocsPaVO.documento.FileRequest fileRequest)
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
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="objSicurezza"></param>
        /// <returns></returns>
        /// 
        //Laura 10 Dicembre
        protected virtual string GetDocPathAdvanced(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente objSicurezza, string estensione)
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

                        //Laura 10 Dicembre
                        if (String.IsNullOrEmpty(fileRequest.path))
                        {

                    filePath = System.Configuration.ConfigurationManager.AppSettings["DOC_PATH"];
                    if (filePath == null) filePath = "";

                    filePath = filePath.Replace("AMMINISTRAZIONE", amministrazione);
                    filePath = filePath.Replace("REGISTRO", registro);
                    filePath = filePath.Replace("ANNO", anno);
                    filePath = filePath.Replace("ARRIVO_PARTENZA", arrivoPartenza);
                    filePath = filePath.Replace("UFFICIO", codiceUO);
                    filePath = filePath.Replace("UTENTE", objSicurezza.userId);

                            string pathDelFile = "";
                            try
                            {
                                string fileName = "";
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

                                pathDelFile = string.Format(@"{0}\{1}\{2}", System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"], filePath, fileName);



                                //Laura 7 febbraio 2013
                                if (!String.IsNullOrEmpty(fileRequest.fileName))
                                {
                                    FileInfo fname = new FileInfo(fileRequest.fileName);
                                    if (fname.Exists)
                                    {
                                        filePath = fileRequest.fileName;
                                        if (filePath.StartsWith(System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"])) {
                                                filePath = filePath.Substring(System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"].Length);
                                        }
                                                filePath = filePath.Substring(0,filePath.Length - fname.Name.Length);
                                        
                                    }
                                    else {
                                        FileInfo f = new FileInfo(@pathDelFile);
                                        if (!f.Exists)
                                        {
                                            filePath = System.Configuration.ConfigurationManager.AppSettings["DOC_PATH"];
                                            filePath = filePath.Replace("AMMINISTRAZIONE", amministrazione);
                                            filePath = filePath.Replace("REGISTRO", "");
                                            filePath = filePath.Replace("ANNO", anno);
                                            filePath = filePath.Replace("ARRIVO_PARTENZA", "");
                                            filePath = filePath.Replace("UFFICIO", codiceUO);
                                            filePath = filePath.Replace("UTENTE", objSicurezza.userId);
                                        }
                                    }
                                }
                                else
                                {

                                FileInfo f = new FileInfo(@pathDelFile);
                                    if (!f.Exists)
                                    {
                                    filePath = System.Configuration.ConfigurationManager.AppSettings["DOC_PATH"];
                                    filePath = filePath.Replace("AMMINISTRAZIONE", amministrazione);
                                    filePath = filePath.Replace("REGISTRO", "");
                                    filePath = filePath.Replace("ANNO", anno);
                                    filePath = filePath.Replace("ARRIVO_PARTENZA", "");
                                    filePath = filePath.Replace("UFFICIO", codiceUO);
                                    filePath = filePath.Replace("UTENTE", objSicurezza.userId);
                                }
                            }
                            }
                            catch (Exception e) {

                                filePath = filePath.Replace("AMMINISTRAZIONE", amministrazione);
                                filePath = filePath.Replace("REGISTRO", "");
                                filePath = filePath.Replace("ANNO", anno);
                                filePath = filePath.Replace("ARRIVO_PARTENZA", "");
                                filePath = filePath.Replace("UFFICIO", codiceUO);
                                filePath = filePath.Replace("UTENTE", objSicurezza.userId);
                    }

                        }
                        else
                            filePath = fileRequest.path;
                    }
                    else {
                        //Laura 7 febbraio 2013
                        if (!String.IsNullOrEmpty(fileRequest.fileName))
                        {
                            FileInfo fname = new FileInfo(fileRequest.fileName);
                            if (fname.Exists)
                            {
                                filePath = fileRequest.fileName;
                                if (filePath.StartsWith(System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"]))
                                {
                                    filePath = filePath.Substring(System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"].Length);
                                }
                                filePath = filePath.Substring(0, filePath.Length - fname.Name.Length);
                            }
                            else
                            {

                                filePath = System.Configuration.ConfigurationManager.AppSettings["DOC_PATH"];
                                if (filePath == null) filePath = "";

                                filePath = filePath.Replace("AMMINISTRAZIONE", amministrazione);
                                filePath = filePath.Replace("REGISTRO", registro);
                                filePath = filePath.Replace("ANNO", anno);
                                filePath = filePath.Replace("ARRIVO_PARTENZA", "");
                                filePath = filePath.Replace("UFFICIO", codiceUO);
                                filePath = filePath.Replace("UTENTE", objSicurezza.userId);
                            }
                        }
                        else
                        {
                        filePath = System.Configuration.ConfigurationManager.AppSettings["DOC_PATH"];
                        if (filePath == null) filePath = "";

                        filePath = filePath.Replace("AMMINISTRAZIONE", amministrazione);
                        filePath = filePath.Replace("REGISTRO", registro);
                        filePath = filePath.Replace("ANNO", anno);
                        filePath = filePath.Replace("ARRIVO_PARTENZA", "");
                        filePath = filePath.Replace("UFFICIO", codiceUO);
                        filePath = filePath.Replace("UTENTE", objSicurezza.userId);
                    }
                    }

                    //Fine Laura 10 Dicembre

                    //documento


                    //LAURA///////////////////////
                    //filePath = System.Configuration.ConfigurationManager.AppSettings["DOC_PATH"];
                    //if (filePath == null) filePath = "";

                    //filePath = filePath.Replace("AMMINISTRAZIONE", amministrazione);
                    //filePath = filePath.Replace("REGISTRO", registro);
                    //filePath = filePath.Replace("ANNO", anno);
                    //filePath = filePath.Replace("ARRIVO_PARTENZA", arrivoPartenza);
                    //filePath = filePath.Replace("UFFICIO", codiceUO);
                    //filePath = filePath.Replace("UTENTE", objSicurezza.userId);
                   

                   
                    //filePath = fileRequest.path;

                    if (filePath.StartsWith(@"\"))
                    {
                        filePath = filePath.Remove(0, 1);
                    }
                    ///////////////////////////////////////

                    filePath = filePath.Replace(@"\\", @"\");
                    if (filePath.EndsWith(@"\"))
                    {
                        filePath = filePath.Remove(filePath.Length - 1, 1);
                    }

                    //verifica se la directory esiste
                    //string appo = @System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"] + "\\" + filePath;


                    //laura 7 febbraio 2013
                    string appo = "";

                    if (filePath.StartsWith(System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"]))
                        //verifica se la directory esiste
                        appo = @filePath;
                    else
                        appo = @System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"] + "\\" + filePath;
                   

                    

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
            return false;
        }

        /// <summary>
        /// Inserimento del file nel sistema documentale
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="fileDoc"></param>
        /// <param name="objSicurezza"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public bool PutFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocumento, string estensione)
        {
            logger.Info("BEGIN");
            bool retValue = true;
            try
            {
                // Reperimento cartella documentale in cui inserire il file
                //Laura 10 Dicembre
                string relativeDocPath = GetDocPathAdvanced(fileRequest, this._infoUtente, estensione);

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

                //// Creazione path completo del file
                //fileRequest.path = string.Format(@"{0}\{1}\{2}",
                //                    System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"],
                //                    relativeDocPath,
                //                    fileName);

                ////memorizzo il percorso completo per l'attachment ai WS
                //string pathDelFile = fileRequest.path;

                string pathDelFile = string.Format(@"{0}\{1}\{2}", System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"], relativeDocPath, fileName);


                //preparazione degli oggetti di input e di output
                InputData inputFormData = new InputData();
                OutputData outputData = new OutputData();

                //recupero dal db le informazioni base relative al documento
                DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                List<BaseInfoDoc> listaSchedeDoc = documenti.GetBaseInfoForDocument(null, fileRequest.docNumber, null);
                BaseInfoDoc schedaDocBase = listaSchedeDoc[0];

                SchedaDocumento schedaDocumento = documenti.GetDettaglioNoSecurity(this._infoUtente, schedaDocBase.IdProfile, schedaDocBase.DocNumber);

                //WsPia viene preso in considerazione solo se si tratta di un protocollo e la chiamata non proviene da una applicazione esterna
                //if (
                //    (schedaDocBase.IsProto && !string.IsNullOrEmpty(_infoUtente.urlWA)) //E' un protocollo
                //    ||
                //    (schedaDocumento.documentoPrincipale != null && schedaDocumento.documentoPrincipale.tipoProto != "G" && !string.IsNullOrEmpty(_infoUtente.urlWA)) // E' un allegato di un protocollo
                //    )
                //{

                //    if (!schedaDocBase.IsAttachment)
                //    {
                //        logger.DebugFormat("(WsPia) - inizio metodo di associazione immagine per il documento numero {0}", fileRequest.docNumber);
                //        //invocazione metodo di associazione immagine dei Web Services di INPS

                //        string[] segnaturaScompattata = (schedaDocBase.Name).Split('.');

                //        inputFormData.codA00 = segnaturaScompattata[1];
                //        inputFormData.codAMM = segnaturaScompattata[0];
                //        inputFormData.codApp = (string.IsNullOrEmpty(ConfigurationManager.AppSettings["CODICE_APPLICAZIONE_PIU"]) ? "___" : ConfigurationManager.AppSettings["CODICE_APPLICAZIONE_PIU"].ToString());
                //        inputFormData.codiceUtente = _infoUtente.userId;
                //        inputFormData.segnatura = schedaDocBase.Name;

                //        //calcolo l'xml in formato stringa da inviare a INPS
                //        FormDatiXmlAssociaImg formDatiXmlAssImg = new FormDatiXmlAssociaImg();
                //        formDatiXmlAssImg.nomeFile = fileName;


                //        string xmlInFormatoStringa = XmlCreatorHelper.creaXmlAssociaImmagine(formDatiXmlAssImg);
                //        inputFormData.xml = xmlInFormatoStringa;

                //        //chiamata al metodo del WS
                //        outputData = CallWebService.associaImmagine(inputFormData, pathDelFile);

                //        //nel caso in cui l'esito sia nullo annoto l'eventuale codice d'errore restituito e la descrizione dell'errore restituita
                //        if (outputData == null || string.IsNullOrEmpty(outputData.esito))
                //        {
                //            logger.Debug("(WsPia)- Esito restituito da Web Service nullo o vuoto");
                //            logger.Debug("(WsPia)- La chiamata al metodo AssociaImmagine dei Web Services ha restituito errore con codice: " + outputData.codiceErrore + " ;la descrizione dell'errore è la seguente: " + outputData.descrizioneErrore);
                //            logger.Debug("(WsPia)- Il file non è stato acquisito sul documentale di INPS, è stato comunque acquisito su DocsPa");
                //        }
                //        logger.DebugFormat("(WsPia) - fine metodo di associazione immagine relativa al documento numero {0}", schedaDocumento.docNumber);
                //    }
                    if (!schedaDocBase.IsAttachment)
                    {
                        logger.DebugFormat("(WsPia) - inizio metodo di associazione immagine per il documento numero {0}", fileRequest.docNumber);
                        //invocazione metodo di associazione immagine dei Web Services di INPS

                        string[] segnaturaScompattata = (schedaDocBase.Name).Split('.');

                        //Emilio 5 Ottobre
                        if (segnaturaScompattata.Length > 1)
                        {

                        inputFormData.codA00 = segnaturaScompattata[1];
                        inputFormData.codAMM = segnaturaScompattata[0];
                            inputFormData.codApp = (string.IsNullOrEmpty(ConfigurationManager.AppSettings["CODICE_APPLICAZIONE_PIU"]) ? "___" : ConfigurationManager.AppSettings["CODICE_APPLICAZIONE_PIU"].ToString());
                        inputFormData.codiceUtente = _infoUtente.userId;
                        inputFormData.segnatura = schedaDocBase.Name;
                        }
                        //Fine Emilio 5 Ottobre

                        //calcolo l'xml in formato stringa da inviare a INPS
                        FormDatiXmlAssociaImg formDatiXmlAssImg = new FormDatiXmlAssociaImg();
                        formDatiXmlAssImg.nomeFile = fileName;


                        //Laura 7 febbraio 2013
                        if (fileRequest.version != "1")
                        {
                            formDatiXmlAssImg.versionamento = "1";
                        }
                        else
                            formDatiXmlAssImg.versionamento = "0";


                        string xmlInFormatoStringa = XmlCreatorHelper.creaXmlAssociaImmagine(formDatiXmlAssImg);
                        inputFormData.xml = xmlInFormatoStringa;

                        //chiamata al metodo del WS
                        outputData = CallWebService.associaImmagine(inputFormData, pathDelFile);

                        //nel caso in cui l'esito sia nullo annoto l'eventuale codice d'errore restituito e la descrizione dell'errore restituita
                        if (outputData == null || string.IsNullOrEmpty(outputData.esito))
                        {
                            logger.Debug("(WsPia)- Esito restituito da Web Service nullo o vuoto");
                            logger.Debug("(WsPia)- La chiamata al metodo AssociaImmagine dei Web Services ha restituito errore con codice: " + outputData.codiceErrore + " ;la descrizione dell'errore è la seguente: " + outputData.descrizioneErrore);
                            logger.Debug("(WsPia)- Il file non è stato acquisito sul documentale di INPS, è stato comunque acquisito su DocsPa");
                        }
                        logger.DebugFormat("(WsPia) - fine metodo di associazione immagine relativa al documento numero {0}", schedaDocumento.docNumber);
                    }



                    else if (schedaDocBase.IsAttachment)
                    {
                        logger.DebugFormat("(WsPia) - inizio metodo di associazione allegato per il documento numero {0}", schedaDocumento.docNumber);
                        //nel caso in cui si tratta di un allegato avvio il processo di AssociaAllegato dei Ws di INPS
                        FormDatiXmlAllegato formDatiXmlAll = new FormDatiXmlAllegato();
                        DocsPaVO.documento.SchedaDocumento schedaDoc = documenti.GetDettaglio(_infoUtente, null, fileRequest.docNumber, true);
                        DocsPaVO.documento.SchedaDocumento schedaDocPrinc = documenti.GetDettaglio(_infoUtente, null, schedaDoc.documentoPrincipale.docNumber, true);
                        string[] segnaturaScompattata = (schedaDoc.documentoPrincipale.segnatura).Split('.');

                        inputFormData.codA00 = segnaturaScompattata[1];
                        inputFormData.codAMM = segnaturaScompattata[0];
                        inputFormData.codApp = (string.IsNullOrEmpty(ConfigurationManager.AppSettings["CODICE_APPLICAZIONE_PIU"]) ? "___" : ConfigurationManager.AppSettings["CODICE_APPLICAZIONE_PIU"].ToString());
                        inputFormData.codiceUtente = _infoUtente.userId;
                        inputFormData.segnatura = schedaDoc.documentoPrincipale.segnatura;

                        if (schedaDocPrinc.tipoProto.Equals("A"))
                        {
                            formDatiXmlAll.categoriaProtocollo = "E";
                            if ((schedaDocPrinc.protocollo is ProtocolloEntrata) && (ProtocolloEntrata)schedaDocPrinc.protocollo != null)
                            {
                                Corrispondente mittente = ((ProtocolloEntrata)schedaDocPrinc.protocollo).mittente;
                                formDatiXmlAll.nominativoMittente = mittente.descrizione;
                                //laura 7 febbraio 2013
                                formDatiXmlAll.codiceMittente = mittente.codiceCorrispondente;
                                string tipoUrp = "";
                                if (mittente is Ruolo) tipoUrp = "R";
                                if (mittente is Utente) tipoUrp = "P";
                                if (mittente is UnitaOrganizzativa) tipoUrp = "U";
                                //mappatura del tipo del mittente
                                if (string.IsNullOrEmpty(mittente.tipoIE) && mittente.tipoCorrispondente == "O")
                                    formDatiXmlAll.tipoMittente = "A";
                                if (mittente.tipoIE == "I" && tipoUrp == "U")
                                    formDatiXmlAll.tipoMittente = "M";
                                else if (mittente.tipoIE == "E" && tipoUrp == "U" && string.IsNullOrEmpty(mittente.codfisc))
                                    formDatiXmlAll.tipoMittente = "A";
                                else if (mittente.tipoIE == "E" && tipoUrp == "U" && !string.IsNullOrEmpty(mittente.codfisc))
                                    formDatiXmlAll.tipoMittente = "G";
                                else if (mittente.tipoIE == "I" && tipoUrp == "R")
                                    formDatiXmlAll.tipoMittente = "A";
                                else if (mittente.tipoIE == "I" && tipoUrp == "P")
                                    formDatiXmlAll.tipoMittente = "F";
                                else if (mittente.tipoIE == "E" && tipoUrp == "P")
                                    formDatiXmlAll.tipoMittente = "F";

                            }
                            else logger.Debug("Attenzione non è stato possibile recuperare il Mittente al momento dell'invocazione di AssociaAllegato dei Ws di INPS");
                        }
                        else if (schedaDocPrinc.tipoProto.Equals("P"))
                        {
                            formDatiXmlAll.categoriaProtocollo = "U";
                            formDatiXmlAll.nominativoMittente = schedaDoc.documentoPrincipale.mittDoc;
                            formDatiXmlAll.tipoMittente = "M";


                            //Laura 10 Dicembre
                            Corrispondente destinatario;
                            if (((ProtocolloUscita)schedaDocPrinc.protocollo).destinatari != null)
                            {
                                for (int i = 0; i < ((ProtocolloUscita)schedaDocPrinc.protocollo).destinatari.Count; i++)
                                {
                                    destinatario = (Corrispondente)(((ProtocolloUscita)schedaDocPrinc.protocollo).destinatari[i]);
                                    DestinatarioXml dest = new DestinatarioXml();
                                    dest.nominativoDestinatario = destinatario.descrizione;
                                    string tipoUrp = "";
                                    if (destinatario is Ruolo) tipoUrp = "R";
                                    if (destinatario is Utente) tipoUrp = "P";
                                    if (destinatario is UnitaOrganizzativa) tipoUrp = "U";

                                    //mappatura del tipo del destinatario
                                    if (string.IsNullOrEmpty(destinatario.tipoIE) && destinatario.tipoCorrispondente == "O")
                                        dest.tipoDestinatario = "A";
                                    if (destinatario.tipoIE == "I" && tipoUrp == "U")
                                    {
                                        dest.tipoDestinatario = "M";
                                        dest.nominativoDestinatario = inputFormData.codAMM + @"\" + destinatario.descrizione;
                                        dest.codiceDestinatario = inputFormData.codAMM + "." + destinatario.codiceCorrispondente;
                                    }
                                    else if (destinatario.tipoIE == "E" && tipoUrp == "U" && string.IsNullOrEmpty(destinatario.codfisc))
                                        dest.tipoDestinatario = "A";
                                    else if (destinatario.tipoIE == "E" && tipoUrp == "U" && !string.IsNullOrEmpty(destinatario.codfisc))
                                        dest.tipoDestinatario = "G";
                                    else if (destinatario.tipoIE == "I" && tipoUrp == "R")
                                        dest.tipoDestinatario = "A";
                                    else if (destinatario.tipoIE == "I" && tipoUrp == "P")
                                        dest.tipoDestinatario = "F";
                                    else if (destinatario.tipoIE == "E" && tipoUrp == "P")
                                        dest.tipoDestinatario = "F";

                                    formDatiXmlAll.listaDestinatari.Add(dest);
                                }
                            }

                            //recupero la lista dei destinatari CC
                            if (((ProtocolloUscita)schedaDocPrinc.protocollo).destinatariConoscenza != null)
                            {
                                for (int i = 0; i < ((ProtocolloUscita)schedaDocPrinc.protocollo).destinatariConoscenza.Count; i++)
                                {
                                    destinatario = (Corrispondente)(((ProtocolloUscita)schedaDocPrinc.protocollo).destinatariConoscenza[i]);
                                    DestinatarioXml dest = new DestinatarioXml();
                                    dest.nominativoDestinatario = destinatario.descrizione;
                                    string tipoUrp = "";
                                    if (destinatario is Ruolo) tipoUrp = "R";
                                    if (destinatario is Utente) tipoUrp = "P";
                                    if (destinatario is UnitaOrganizzativa) tipoUrp = "U";

                                    //mappatura del tipo del destinatario
                                    if (string.IsNullOrEmpty(destinatario.tipoIE) && destinatario.tipoCorrispondente == "O")
                                        dest.tipoDestinatario = "A";
                                    if (destinatario.tipoIE == "I" && tipoUrp == "U")
                                    {
                                        dest.tipoDestinatario = "M";
                                        dest.nominativoDestinatario = inputFormData.codAMM + @"\" + destinatario.descrizione;
                                        dest.codiceDestinatario = inputFormData.codAMM + "." + destinatario.codiceCorrispondente;
                                    }
                                    else if (destinatario.tipoIE == "E" && tipoUrp == "U" && string.IsNullOrEmpty(destinatario.codfisc))
                                        dest.tipoDestinatario = "A";
                                    else if (destinatario.tipoIE == "E" && tipoUrp == "U" && !string.IsNullOrEmpty(destinatario.codfisc))
                                        dest.tipoDestinatario = "G";
                                    else if (destinatario.tipoIE == "I" && tipoUrp == "R")
                                        dest.tipoDestinatario = "A";
                                    else if (destinatario.tipoIE == "I" && tipoUrp == "P")
                                        dest.tipoDestinatario = "F";
                                    else if (destinatario.tipoIE == "E" && tipoUrp == "P")
                                        dest.tipoDestinatario = "F";

                                    formDatiXmlAll.listaDestinatariCC.Add(dest);
                                }
                            }



                        }


                        //recupero la lista dei documenti
                        DocumentoXml doc1 = new DocumentoXml();
                        formDatiXmlAll.flagProtocollo = "S";//valore statico
                        DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                        System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();
                        listaFascicoli = fascicoli.GetFascicoliDaDoc(_infoUtente, schedaDoc.documentoPrincipale.idProfile);
                        if (listaFascicoli.Count > 0)
                        {
                            formDatiXmlAll.classifica = listaFascicoli[0].ToString();
                        }


                        formDatiXmlAll.oggetto = schedaDoc.documentoPrincipale.oggetto;
                        formDatiXmlAll.riservato = "N";//valore statico
                        formDatiXmlAll.nomeFile = fileName;
                        //creazione dell'xml di inuput per il metodo associaAllegato di INPS
                        inputFormData.xml = XmlCreatorHelper.creaXmlPerAssociaAllegato(formDatiXmlAll);

                        //chiamata al metodo del WS
                        outputData = CallWebService.associaAllegato(inputFormData, pathDelFile);

                        if (outputData == null || string.IsNullOrEmpty(outputData.segnatura))
                        {
                            logger.Debug("Esito restituito da Web Service INPS nullo o vuoto");
                            logger.Debug("La chiamata al metodo AssociaAllegato dei Web Services di INPS ha restituito errore con codice: " + outputData.codiceErrore + " ;la descrizione dell'errore è la seguente: " + outputData.descrizioneErrore);
                            logger.Debug("Il file non è stato allegato sul documentale di INPS, è stato comunque acquisito su DocsPa");
                        }

                        logger.DebugFormat("(WsPia) - fine metodo di associazione allegato per il documento numero {0}", schedaDocumento.docNumber);
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

        //Emilio 5 Novembre
        /// <summary>
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <param name="objSicurezza"></param>
        /// <param name="objRuolo"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string copiaDoc = null)
        {
            logger.Info("BEGIN");
            bool retValue = false;
            risultatoProtocollazione = new ResultProtocollazione();
            ruoliSuperiori = null;

            //recupero la segnatura restituita dai Web Services di INPS 
            try
            {

                //preparazione degli oggetti di input e di output
                InputData formData = new InputData();
                OutputData outProto = new OutputData();

                formData.codA00 = schedaDocumento.registro.codRegistro;// "0003";
                formData.codAMM = schedaDocumento.registro.codAmministrazione; //"INPS";
                formData.codApp = (string.IsNullOrEmpty(ConfigurationManager.AppSettings["CODICE_APPLICAZIONE_PIU"]) ? "___" : ConfigurationManager.AppSettings["CODICE_APPLICAZIONE_PIU"].ToString()); //"998298940";
                formData.codiceUtente = schedaDocumento.userId;// "REGPROT";

                //preparazione dati per la produzione dell'xml da inviare a INPS

                FormDatiXmlProtocolla formDatiXml = new FormDatiXmlProtocolla();

                if (schedaDocumento.tipoProto.Equals("A"))
                {
                    formDatiXml.categoriaProtocollo = "E";
                    Corrispondente mittente = ((ProtocolloEntrata)schedaDocumento.protocollo).mittente;
                    formDatiXml.nominativoMittente = mittente.descrizione;
                    string tipoUrp = "";
                    if (mittente is Ruolo) tipoUrp = "R";
                    if (mittente is Utente) tipoUrp = "P";
                    if (mittente is UnitaOrganizzativa) tipoUrp = "U";
                    //mappatura del tipo del mittente
                    if (string.IsNullOrEmpty(mittente.tipoIE) && mittente.tipoCorrispondente == "O")
                        formDatiXml.tipoMittente = "A";
                    if (mittente.tipoIE == "I" && tipoUrp == "U")
                    {
                        if (mittente.codiceCorrispondente.Contains(formData.codA00))
                        {
                            logger.Debug("(WSPIA) - PROTOCOLLO IN INGRESSO SU REGISTRO : " + formData.codA00 + " CON MITTENTE: " + mittente.codiceCorrispondente + " NON CONSENTITO DA WSPIA");
                            risultatoProtocollazione = ResultProtocollazione.ERRORE_WSPIA_PROTOCOLLO_ENTRATA_MITTENTE;
                            return false;
                        }
                        formDatiXml.codiceMittente = formData.codAMM + "." + mittente.codiceCorrispondente;
                        formDatiXml.nominativoMittente = formData.codAMM + @"\" + mittente.codiceCorrispondente;
                        //formDatiXml.codiceMittente = formData.codAMM + "." + formData.codA00;
                        //formDatiXml.nominativoMittente = formData.codAMM + @"\" + formData.codA00;

                        formDatiXml.tipoMittente = "M";

                    }
                    else if (mittente.tipoIE == "E" && tipoUrp == "U" && string.IsNullOrEmpty(mittente.codfisc))
                        formDatiXml.tipoMittente = "A";
                    else if (mittente.tipoIE == "E" && tipoUrp == "U" && !string.IsNullOrEmpty(mittente.codfisc))
                        formDatiXml.tipoMittente = "G";
                    else if (mittente.tipoIE == "I" && tipoUrp == "R")
                        formDatiXml.tipoMittente = "A";
                    else if (mittente.tipoIE == "I" && tipoUrp == "P")
                        formDatiXml.tipoMittente = "F";
                    else if (mittente.tipoIE == "E" && tipoUrp == "P")
                        formDatiXml.tipoMittente = "F";


                }
                else if (schedaDocumento.tipoProto.Equals("P"))
                {
                    formDatiXml.categoriaProtocollo = "U";

                    //recupero la lista dei destinatari
                    Corrispondente destinatario;
                    if (((ProtocolloUscita)schedaDocumento.protocollo).destinatari != null)
                    {
                        for (int i = 0; i < ((ProtocolloUscita)schedaDocumento.protocollo).destinatari.Count; i++)
                        {
                            destinatario = (Corrispondente)(((ProtocolloUscita)schedaDocumento.protocollo).destinatari[i]);
                            DestinatarioXml dest = new DestinatarioXml();
                            dest.nominativoDestinatario = destinatario.descrizione;
                            string tipoUrp = "";
                            if (destinatario is Ruolo) tipoUrp = "R";
                            if (destinatario is Utente) tipoUrp = "P";
                            if (destinatario is UnitaOrganizzativa) tipoUrp = "U";

                            //mappatura del tipo del destinatario
                            if (string.IsNullOrEmpty(destinatario.tipoIE) && destinatario.tipoCorrispondente == "O")
                                dest.tipoDestinatario = "A";
                            if (destinatario.tipoIE == "I" && tipoUrp == "U")
                            {
                                dest.tipoDestinatario = "M";
                                dest.nominativoDestinatario = formData.codAMM + @"\" + destinatario.descrizione;
                                dest.codiceDestinatario = formData.codAMM + "." + destinatario.codiceCorrispondente;
                            }
                            else if (destinatario.tipoIE == "E" && tipoUrp == "U" && string.IsNullOrEmpty(destinatario.codfisc))
                                dest.tipoDestinatario = "A";
                            else if (destinatario.tipoIE == "E" && tipoUrp == "U" && !string.IsNullOrEmpty(destinatario.codfisc))
                                dest.tipoDestinatario = "G";
                            else if (destinatario.tipoIE == "I" && tipoUrp == "R")
                                dest.tipoDestinatario = "A";
                            else if (destinatario.tipoIE == "I" && tipoUrp == "P")
                                dest.tipoDestinatario = "F";
                            else if (destinatario.tipoIE == "E" && tipoUrp == "P")
                                dest.tipoDestinatario = "F";

                            formDatiXml.listaDestinatari.Add(dest);
                        }
                    }

                    //recupero la lista dei destinatari CC
                    if (((ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza != null)
                    {
                        for (int i = 0; i < ((ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza.Count; i++)
                        {
                            destinatario = (Corrispondente)(((ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza[i]);
                            DestinatarioXml dest = new DestinatarioXml();
                            dest.nominativoDestinatario = destinatario.descrizione;
                            string tipoUrp = "";
                            if (destinatario is Ruolo) tipoUrp = "R";
                            if (destinatario is Utente) tipoUrp = "P";
                            if (destinatario is UnitaOrganizzativa) tipoUrp = "U";

                            //mappatura del tipo del destinatario
                            if (string.IsNullOrEmpty(destinatario.tipoIE) && destinatario.tipoCorrispondente == "O")
                                dest.tipoDestinatario = "A";
                            if (destinatario.tipoIE == "I" && tipoUrp == "U")
                            {
                                dest.tipoDestinatario = "M";
                                dest.nominativoDestinatario = formData.codAMM + @"\" + destinatario.descrizione;
                                dest.codiceDestinatario = formData.codAMM + "." + destinatario.codiceCorrispondente;
                            }
                            else if (destinatario.tipoIE == "E" && tipoUrp == "U" && string.IsNullOrEmpty(destinatario.codfisc))
                                dest.tipoDestinatario = "A";
                            else if (destinatario.tipoIE == "E" && tipoUrp == "U" && !string.IsNullOrEmpty(destinatario.codfisc))
                                dest.tipoDestinatario = "G";
                            else if (destinatario.tipoIE == "I" && tipoUrp == "R")
                                dest.tipoDestinatario = "A";
                            else if (destinatario.tipoIE == "I" && tipoUrp == "P")
                                dest.tipoDestinatario = "F";
                            else if (destinatario.tipoIE == "E" && tipoUrp == "P")
                                dest.tipoDestinatario = "F";

                            formDatiXml.listaDestinatariCC.Add(dest);
                        }
                    }


                }


                //recupero la lista dei documenti
                DocumentoXml doc1 = new DocumentoXml();
                doc1.flagProtocollo = "S";//valore statico
                DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                //recuper il codice classifica che sarà utilizzato nel caso di doc predistosto
                string codiceFascicoloPerPredisposto = fascicoli.GetfascicolazioneFromDoc(schedaDocumento.systemId);
                if (!string.IsNullOrEmpty(schedaDocumento.codiceFascicolo))
                {
                    if ((schedaDocumento.codiceFascicolo).Contains("\\"))
                    {
                        string codicePerWspia = (schedaDocumento.codiceFascicolo).Remove(schedaDocumento.codiceFascicolo.IndexOf("\\"));
                        doc1.classifica = codicePerWspia;
                    }
                    else
                    {
                        doc1.classifica = schedaDocumento.codiceFascicolo;
                    }
                }
                else
                {

                    if (codiceFascicoloPerPredisposto.Contains("\\"))
                    {
                        string codicePerWspia = codiceFascicoloPerPredisposto.Remove(codiceFascicoloPerPredisposto.IndexOf("\\"));
                        doc1.classifica = codicePerWspia;
                    }
                    else
                    {
                        doc1.classifica = codiceFascicoloPerPredisposto;
                    }
                }
                doc1.oggetto = schedaDocumento.oggetto.descrizione;
                doc1.riservato = "N";//valore statico
                formDatiXml.listaDocumenti.Add(doc1);

                //calcolo l'xml in formato stringa da inviare a INPS
                string xmlInformatoStringa = XmlCreatorHelper.CreaXmlProtocolla(formDatiXml);

                logger.DebugFormat("XML per la richiesta di protocollo WSPia: {0}", xmlInformatoStringa);

                formData.xml = xmlInformatoStringa;

                //chiamata al WS di INPS
                outProto = CallWebService.protocollaDocumento(formData);

                logger.DebugFormat("Esito protocollazione WSPia: {0}", outProto.esito);

                if (outProto != null && !string.IsNullOrEmpty(outProto.segnatura))
                {
                    retValue = true;

                    //recupero la struttura della segnatura da DB
                    //sulla base di tale struttura verrà scompattata la segnatura ricevuta via Web Service

                    string codice = DocsPaDB.Utils.Personalization.getInstance(schedaDocumento.registro.idAmministrazione).FormatoSegnatura;
                    if (codice == null)
                    {
                        //in alcuni casi, l'oggetto Personalization non è not null, ma il formato segnatura è null!!
                        //per evitare il blocco della protocollazione a meno di un iisreset inserisco questo codice.
                        logger.Debug("Ricalcolo Personalization");
                        DocsPaDB.Utils.Personalization.Reset();
                        codice = DocsPaDB.Utils.Personalization.getInstance(schedaDocumento.registro.idAmministrazione).FormatoSegnatura;
                    }


                    //scompatto la segnatura ricevuta dai Web Services di INPS e recupero i dati del protocollo
                    string[] codiceScompattato = codice.Split('.');
                    string[] segnaturaScompattata = outProto.segnatura.Split('.');
                    string[] dataScompattata = null;
                    string numProtoTemp = "";


                    for (int i = 0; i < codiceScompattato.Length; i++)
                    {
                        if (codiceScompattato[i].Equals("NUM_PROTO"))
                        {
                            numProtoTemp = segnaturaScompattata[i];

                        }

                        if (codiceScompattato[i].Equals("DATA_COMP"))
                        {
                            schedaDocumento.protocollo.dataProtocollazione = segnaturaScompattata[i];
                            dataScompattata = segnaturaScompattata[i].Split('/');
                        }

                    }

                    //elimino gli eventuali zeri di padding presenti prima del numero protocollo

                    while (numProtoTemp.StartsWith("0") && numProtoTemp.Length != 1)
                    {

                        numProtoTemp = numProtoTemp.Substring(1);

                    }

                    //assegno i valori ricavati dalla segnatura ai campi dell'oggetto protocollo
                    //di schedaDocumento
                    schedaDocumento.protocollo.segnatura = outProto.segnatura;
                    schedaDocumento.protocollo.numero = numProtoTemp;
                    schedaDocumento.protocollo.anno = dataScompattata[2];

                    //Salvo il progressivo del protocollo per eventuale gestione separata da WSPIA
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                    if (doc.UpdateNumeroProtocolloWSPIA(schedaDocumento, ruolo))
                    {
                        logger.Debug("Salvataggio progressivo WSPIA");
                    } 
                    else
                        logger.Debug("Errore nel salvataggio del progressivo su DB");


                }

                else
                {

                    if (outProto != null && !string.IsNullOrEmpty(outProto.descrizioneErrore) && outProto.descrizioneErrore.Contains("foglia"))
                    {
                        logger.Debug("(WSPIA) - NODO FOGLIA NON SELEZIONATO");
                        risultatoProtocollazione = ResultProtocollazione.ERRORE_WSPIA_CLASSIFICA_NODO_FOGLIA;
                        return false;
                    }
                    logger.Debug("(WsPia)- Segnatura restituita da Web Service nulla o vuota");
                    logger.Debug("(WsPia)- La chiamata al metodo protocolla dei Web Services ha restituito errore con codice: " + outProto.codiceErrore + " ;la descrizione dell'errore è la seguente: " + outProto.descrizioneErrore);
                    risultatoProtocollazione = ResultProtocollazione.ERRORE_WSPIA_PROTOCOLLAZIONE_SEMPLICE;
                    return false;

                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nella protocollazione del documento numero {0}: {1}", schedaDocumento.docNumber, ex.Message);
                logger.Debug(errorMessage, ex);

                retValue = false;
                risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;
            }

            logger.Info("END");
            return retValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="daInviare"></param>
        /// <returns></returns>
        public bool AddVersion(DocsPaVO.documento.FileRequest fileRequest, bool daInviare)
        {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public string GetLatestVersionId(string docNumber)
        {
            string versionId = null;


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

        //private void ModificaEstensione(DocsPaVO.documento.FileRequest fileRequest, string version, string versionLabel, string version_id, string subVersion, string dataInserimento)
        //{
        //    using (DocsPaDB.DBProvider dbProvider = new DBProvider())
        //    {
        //        DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_Versions");

        //        if (fileRequest.descrizione == null)
        //            fileRequest.descrizione = string.Empty;

        //        string sqlString = "VERSION_LABEL = '" + versionLabel + "'" + ",DTA_CREAZIONE = " + DocsPaDbManagement.Functions.Functions.ToDate(dataInserimento) + ",COMMENTS = '" + fileRequest.descrizione.Replace("'", "''") + "'";

        //        q.setParam("param1", sqlString);
        //        q.setParam("param2", "VERSION_ID=" + version_id);
        //        string queryString = q.getSQL();
        //        logger.Debug(queryString);
        //        dbProvider.ExecuteNonQuery(queryString);

        //        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Components");
        //        q.setParam("param1", "PATH");
        //        q.setParam("param2", "VERSION_ID=" + version_id);
        //        queryString = q.getSQL();
        //        logger.Debug(queryString);
        //        dbProvider.ExecuteScalar(out fileRequest.fileName, queryString);

        //        fileRequest.version = version;
        //        fileRequest.subVersion = subVersion;
        //        fileRequest.versionLabel = versionLabel;
        //        fileRequest.dataInserimento = dataInserimento;
        //    }
        //}

        /// <summary>
        /// Annullamento di un protocollo
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="protocolloAnnullato"></param>
        /// <returns></returns>
        public bool AnnullaProtocollo(ref DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.documento.ProtocolloAnnullato protocolloAnnullato)
        {
            return false;
        }

        /// <summary>
        /// Modifica dei metadati di una versione
        /// </summary>
        /// <param name="fileRequest"></param>
        public void ModifyVersion(DocsPaVO.documento.FileRequest fileRequest)
        {

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
            return true;
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
            return true;
        }

        /// <summary>
        /// Rimozione di un documento grigio
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public string RemoveDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            return string.Empty;
        }

        /// <summary>
        /// Inserimento di un documento nel cestino
        /// </summary>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        public bool AddDocumentoInCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            return false;
        }

        /// <summary>
        /// Ripristino del documento dal cestino
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <returns></returns>
        public bool RestoreDocumentoDaCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            bool retValue = false;


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
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="removeFile"></param>
        /// <returns></returns>
        public virtual bool Remove(DocsPaVO.documento.InfoDocumento[] items, bool removeFile)
        {
            bool retValue = true;



            return retValue;
        }

        /// <summary>
        /// Rimozione documenti
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public bool Remove(params DocsPaVO.documento.InfoDocumento[] items)
        {
            return false;
        }

        private DocsPaVO.documento.InfoDocumento GetInfoDocumento(DataRow row)
        {

            return null;
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
            return false;
        }

        /// <summary>
        /// Revoca di un permesso su un documento
        /// </summary>
        /// <param name="documentInfo"></param>
        /// <returns></returns>
        public bool RemovePermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            return false;
        }

        /// <summary>
        /// Metodo per l'assegnazione di un diritto di tipo A ad un ruolo
        /// </summary>
        /// <param name="rights">Informazioni sul diritto da assegnare</param>
        /// <returns>True se è andato bene</returns>
        public bool AddPermissionToRole(DirittoOggetto rights)
        {
            return false;
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
