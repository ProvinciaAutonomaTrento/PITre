using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using DocsPAWA.DocsPaWR;
using ProtocollazioneIngresso.Protocollo;
using log4net;
using System.Collections.Generic;
using System.Linq;

namespace DocsPAWA
{
    /// <summary>
    /// Summary description for FileManager.
    /// </summary>
    public class FileManager
    {
        private ProtocolloMng _protocolloMng = null;

        private static Hashtable fileManager;
        public string PageID;
        public string fileName;
        //private DateTime startTime;
        public string percDone = "0";
        //private int bytesDone;
        public string kbDone = "";
        public string kbTotal = "";
        public string note = "";
        public bool chiudi = false;

        private FileDocumento fileDoc = null;
        private HttpResponse response = null;

        private static DocsPAWA.DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.getWS();
        private static DocsPAWA.DocsPaWR.DocsPaWebService docsPaWSforCache = null;
        private static ILog logger = LogManager.GetLogger(typeof(FileManager));

        public static bool setdatavistaSP(DocsPaWR.InfoUtente infoutente, string idProfile, string docorFasc)
        {
            return docsPaWS.SetDataVistaSP(infoutente, idProfile, docorFasc);

        }

        public static FileRequest getSelectedFile(Page page)
        {
            return getSelectedFile();
        }

        public static FileRequest getSelectedFile()
        {
            return (FileRequest)HttpContext.Current.Session["FileManager.selectedFile"];
        }

        public static FileDocumento getSelectedReport(Page page)
        {
            return (FileDocumento)page.Session["FileManager.selectedReport"];
        }

        public static void setSelectedFile(Page page, FileRequest fileRequest)
        {
            setSelectedFile(page, fileRequest, true);
        }

        /// <summary>
        /// NELLA CONFIGURAZIONE: set_DATA_VISTA_GRD=2, Toglie la trasmissione dalla TDL, ma non setta la DATA VISTA.
        /// </summary>
        /// <param name="infoutente"></param>
        /// <param name="idProfile"></param>
        /// <param name="docorFasc"></param>
        /// <returns></returns>
        public static bool setdatavistaSP_TV(DocsPaWR.InfoUtente infoutente, string idProfile, string docorFasc, String idRegistro, string idTrasm)
        {
            return docsPaWS.SetDataVistaSP_TV(infoutente, idProfile, docorFasc, idRegistro, idTrasm);

        }
        public static void setSelectedFile(Page page, FileRequest fileRequest, bool refresh)
        {
            FileRequest sel = (FileRequest)getSelectedFile(page);
            if (sel != null && sel == fileRequest)
                return;

            page.Session["FileManager.selectedFile"] = fileRequest;
            //ricarica il frame destro
            if (refresh)
            {
                string nameSpace = page.GetType().BaseType.Namespace;
                string funct_dx;

                if (page.Session["refreshDxPageVisualizzatore"] == null && Convert.ToBoolean(page.Session["refreshDxPageVisualizzatore"]) != true)
                {
                    if (nameSpace.EndsWith(".popup"))
                        funct_dx = "window.opener.top.principale.iFrame_dx.document.location='../documento/tabdoc.aspx';";
                    else
                        funct_dx = "top.principale.iFrame_dx.document.location='tabdoc.aspx' ";

                    page.ClientScript.RegisterStartupScript(page.GetType(), "Messaggio", "try{" + funct_dx + "}catch(e){}", true);
                }

            }

        }

        private static bool controllaFileSize(FileRequest sel, FileRequest fileRequest)
        {
            if (sel.versionId.Equals(fileRequest.versionId) && fileRequest.fileSize == null)
                return true;
            return false;
        }

        public static void setSelectedFileReg(Page page, FileRequest fileRequest, string initialPath)
        {

            if ((FileRequest)getSelectedFile(page) == fileRequest)
                return;

            page.Session["FileManager.selectedFile"] = fileRequest;

            //carica la new pg
            //COMMENTATO PER L'APERTURA DELLA STAMPA DA MODALE
            //			string  funct_dx = " window.open('"+ initialPath +"/visualStampaReg.aspx?id="+page.Session.SessionID+"','top','width=800,height=600,toolbar=yes,directories=no,menubar=no,resizable=yes, scrollbars=no')"; 
            //			page.Response.Write("<script> " + funct_dx + "</script>");

        }

        public static void setSelectedFileReport(Page page, DocsPaWR.FileDocumento fileDoc, string initialPath)
        {
            page.Session["FileManager.selectedReport"] = fileDoc;

            //carica la new pg			
            string funct_dx = " window.open('" + initialPath + "/visualReport.aspx?id=" + page.Session.SessionID + "','top','width=800,height=600,top=100,left=100,toolbar=yes,directories=no,menubar=no,resizable=yes, scrollbars=no')";
            page.Response.Write("<script> " + funct_dx + "</script>");
            //apro una modale

            //page.Response.Write("<script>window.showModalDialog('"+ initialPath +"/visualReport.aspx?id="+page.Session.SessionID+"','','dialogWidth:800px;dialogHeight:600px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');</script> ");
            //			string sval=initialPath +"/visualReport.aspx?id="+page.Session.SessionID;
            //			page.Response.Write("<script>OpenMyDialog( sVal );</script> ");

        }

        public static void removeSelectedFile()
        {
            RemoveSessionValue("FileManager.selectedFile");
        }

        public static void removeSelectedFile(Page page)
        {
            removeSelectedFile();
        }

        public static void clearUnusedInstances()
        {
            try
            {
                if (fileManager != null)
                {
                    IEnumerator idList = fileManager.Keys.GetEnumerator();
                    while (idList != null && idList.MoveNext())
                        FileManager.getInstance((string)idList.Current).checkConnecion();
                }
            }
            catch (Exception) { }
        }

        private FileManager(string ID)
        {
            if (fileManager == null)
                fileManager = new Hashtable();
            PageID = ID;
            chiudi = false;
        }

        public FileManager() { }

        public static FileManager getInstance(string ID)
        {
            if (!(fileManager != null && fileManager.ContainsKey(ID)))
            {
                clearUnusedInstances();
                FileManager fm = new FileManager(ID);
                fileManager.Add(ID, fm);
            }
            return (FileManager)fileManager[ID];
        }

        public void delInstance()
        {
            try
            {
                fileManager.Remove(this.PageID);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="file"></param>
        /// <param name="cartaceo"></param>
        public static void uploadFile(Page page, HttpPostedFile file, bool cartaceo)
        {
            logger.Info("BEGIN");
            string errorMessage = string.Empty;
            try
            {
                FileRequest fileReq = getSelectedFile(page);
                if (fileReq != null)
                {
                    InfoUtente infoUtente = UserManager.getInfoUtente(page);
                    FileDocumento fileDoc = new FileDocumento();
                    fileDoc.name = System.IO.Path.GetFileName(file.FileName);
                    fileDoc.fullName = fileDoc.name;
                    fileDoc.contentType = file.ContentType;
                    fileDoc.length = file.ContentLength;
                    fileDoc.content = new Byte[fileDoc.length];
                    fileDoc.cartaceo = cartaceo;
                    file.InputStream.Read(fileDoc.content, 0, fileDoc.length);

                    fileReq.cartaceo = cartaceo;
                    fileReq.fileName = System.IO.Path.GetFileName(file.FileName);
                    fileReq.fileSize = fileDoc.length.ToString();

                    //fileReq = docsPaWS.DocumentoPutFile(fileReq, fileDoc, infoUtente);
                    if (!docsPaWS.DocumentoPutFileNoException(ref fileReq, fileDoc, infoUtente, out errorMessage))
                    {
                        if (errorMessage != string.Empty)
                        {
                            aggiornaFileRequest(page, fileReq);

                            throw new Exception("Non è stato possibile acquisire il documento. <BR><BR>Ripetere l'operazione di acquisizione.");

                        }
                    }
                    aggiornaFileRequest(page, fileReq);
                }
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                //ErrorManager.redirect(page, es);
                ErrorManager.redirect(page, es, "acquisizione");
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e, "acquisizione");
                //ErrorManager.redirect(page, e);
            }
            logger.Info("END");
        }

        /// <summary>
        /// Funzione per l'updload del file di un documento.
        /// </summary>
        /// <param name="page">La pagina richiedente</param>
        /// <param name="file"></param>
        /// <param name="cartaceo"></param>
        /// <param name="conversionePdfServer">Valore che indica se è richiesta la conversione PDF lato server</param>
        /// <param name="conversionePdfServerSincrona">Valore che indica se è richiesta la conversione PDF sincrona</param>
        public static void uploadFile(Page page, HttpPostedFile file, bool cartaceo, bool conversionePdfServer, bool conversionePdfServerSincrona)
        {
            logger.Info("BEGIN");
            // Variabile di appoggio utilizzata per onctenere il documento convertito in PDF
            // a seguito di una richiesta di converisone sincrona lato server
            FileDocumento convSyncResult = null;

            string errorMessage = string.Empty;
            try
            {
                FileRequest fileReq = getSelectedFile(page);
                if (fileReq != null)
                {
                    InfoUtente infoUtente = UserManager.getInfoUtente(page);
                    FileDocumento fileDoc = new FileDocumento();
                    fileDoc.name = System.IO.Path.GetFileName(file.FileName);
                    fileDoc.fullName = fileDoc.name;
                    fileDoc.contentType = file.ContentType;
                    fileDoc.length = file.ContentLength;
                    fileDoc.content = new Byte[fileDoc.length];
                    fileDoc.cartaceo = cartaceo;
                    file.InputStream.Read(fileDoc.content, 0, fileDoc.length);

                    fileReq.cartaceo = cartaceo;
                    fileReq.fileName = System.IO.Path.GetFileName(file.FileName);
                    fileReq.fileSize = fileDoc.length.ToString();

                    // Se è stata richiesta la conversione PDF lato server sincrona...
                    if (conversionePdfServer && conversionePdfServerSincrona)
                        // ...la prima versione del documento è direttamente il file convertito in PDF
                        // Richedo quindi la conversione in pdf del content dell'oggetto FileDocumento
                        convSyncResult = docsPaWS.GeneratePDFInSyncMod(fileDoc);

                    // Se la conversione PDF ha avuto successo...
                    if (convSyncResult != null)
                        // Sostituisco il fileDoc con convSyncResult
                        fileDoc = convSyncResult;

                    //Chiamo il metodo che verifica se è un form pdf da processare
                    processFormPdf(page, fileDoc, fileReq);

                    //fileReq = docsPaWS.DocumentoPutFile(fileReq, fileDoc, infoUtente);
                    if (!docsPaWS.DocumentoPutFileNoException(ref fileReq, fileDoc, infoUtente, out errorMessage))
                    {
                        if (errorMessage != string.Empty)
                        {
                            aggiornaFileRequest(page, fileReq);
                            setSelectedFile(page, null);
                            throw new Exception(errorMessage);
                        }
                        else
                        {
                            throw new Exception("Non è stato possibile acquisire il documento. <BR><BR>Ripetere l'operazione di acquisizione.");
                        }
                    }
                    aggiornaFileRequest(page, fileReq);

                    //Se abilitata la conversione lato server (asincrona) chiamo il webmethod che mette in coda il file da convertire
                    if (conversionePdfServer && !conversionePdfServerSincrona)
                    {
                        bool isAllegato = (fileReq.GetType() == typeof(DocsPaWR.Allegato));

                        DocsPaWR.SchedaDocumento schedaDocumentoCorrente = null;

                        if (isAllegato)
                            schedaDocumentoCorrente = CheckInOut.CheckInOutServices.CurrentSchedaDocumento;
                        else
                            schedaDocumentoCorrente = DocumentManager.getDocumentoInLavorazione(page);

                        EnqueueServerPdfConversion(page,
                                                    UserManager.getInfoUtente(page),
                                                    fileDoc.content,
                                                    fileDoc.name,
                                                    schedaDocumentoCorrente);
                    }

                    // Se è stata richiesta la conversione lato server ma questa non è andata
                    // a buon fine...
                    if (conversionePdfServer && conversionePdfServerSincrona && convSyncResult == null)
                        // ...sollevo un'eccezione in modo da avvisare l'utente che il documento è stato
                        // acquisito ma non si è riusciti a convertire in pdf in modalità sincrona
                        throw new Exception("Non è stato possibile convertire il documento in pdf. Il documento è stato comunque acquisito nel formato originale.");
                }
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                //ErrorManager.redirect(page, es);
                ErrorManager.redirect(page, es, "acquisizione");
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e, "acquisizione");
                //ErrorManager.redirect(page, e);
            }
            logger.Info("END");
        }

        public static void uploadFile(Page page, HttpPostedFile file)
        {
            uploadFile(page, file, false);
        }

        public void uploadFile(Page page, FileDocumento fileDoc, bool addVersion)
        {
            logger.Info("BEGIN");
            try
            {
                FileRequest fileReq = getSelectedFile(page);

                fileReq.cartaceo = fileDoc.cartaceo;

                bool daInviare = true;
                if (addVersion)
                {
                    if (fileReq.fileName.Substring(fileReq.fileName.LastIndexOf(".") + 1).ToUpper().Equals("P7M"))
                        daInviare = false;
                    Documento documento = new Documento();
                    if (page.Request.QueryString["save"] != null && page.Request.QueryString["save"].Equals("true"))
                        documento.descrizione = "Impressa segnatura";
                    else
                        documento.descrizione = "File convertito";
                    documento.docNumber = fileReq.docNumber;
                    fileReq = DocumentManager.aggiungiVersione(page, documento, daInviare, false);
                }

                if (fileReq != null)
                {
                    fileName = fileDoc.fullName;
                    chiudi = false;
                    InfoUtente infoUtente = UserManager.getInfoUtente(page);

                    kbTotal = Convert.ToString(Math.Round((double)fileDoc.length / 1024, 2));
                    //bytesDone = 0;		
                    //int bytesToRead = 1024;

                    //startTime = System.DateTime.Now;

                    note = "Salvataggio del file...";

                    chiudi = true;

                    //Chiamo il metodo che verifica se è un form pdf da processare
                    processFormPdf(page, fileDoc, fileReq);

                    string nomeFile = fileReq.fileName;
                    fileReq = docsPaWS.DocumentoPutFile(fileReq, fileDoc, infoUtente);

                    if (fileReq == null)
                    {

                        throw new Exception("Attenzione,<BR>l'acquisizione file non effettuata.");
                    }
                    aggiornaFileRequest(page, fileReq);


                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
            logger.Info("END");
        }

        private static void aggiornaFileRequest(Page page, FileRequest newFileReq)
        {
            SchedaDocumento schedaDoc = DocumentManager.getDocumentoSelezionato(page);
            FileRequest fileReq = getSelectedFile(page);
            setSelectedFile(page, newFileReq, false);
            if (fileReq.GetType().Equals(typeof(Documento)))
            {
                for (int i = 0; i < schedaDoc.documenti.Length; i++)
                {
                    if (schedaDoc.documenti[i].versionId.Equals(fileReq.versionId))
                    {
                        //modifica perchè non risce a castare l'oggetto filerequest come documento
                        DocsPaWR.Documento documento = new DocsPAWA.DocsPaWR.Documento();
                        documento.applicazione = newFileReq.applicazione;
                        documento.daAggiornareFirmatari = newFileReq.daAggiornareFirmatari;
                        documento.dataInserimento = newFileReq.dataInserimento;
                        documento.descrizione = newFileReq.descrizione;
                        documento.docNumber = newFileReq.docNumber;
                        documento.docServerLoc = newFileReq.docServerLoc;
                        documento.fileName = newFileReq.fileName;
                        documento.fileSize = newFileReq.fileSize;
                        documento.firmatari = newFileReq.firmatari;
                        documento.firmato = newFileReq.firmato;
                        documento.idPeople = newFileReq.idPeople;
                        documento.path = newFileReq.path;
                        documento.subVersion = newFileReq.version;
                        documento.version = newFileReq.version;
                        documento.versionId = newFileReq.versionId;
                        documento.versionLabel = newFileReq.versionLabel;
                        documento.cartaceo = newFileReq.cartaceo;
                        documento.repositoryContext = newFileReq.repositoryContext;

                        // modifica necessaria per FILENET (A.B.)
                        if ((newFileReq.fNversionId != null) && (newFileReq.fNversionId != ""))
                            documento.fNversionId = newFileReq.fNversionId;


                        schedaDoc.documenti[i] = documento;
                        //*****fine modifica**********
                        //schedaDoc.documenti[i] = (Documento) newFileReq;
                        DocumentManager.setDocumentoSelezionato(page, schedaDoc);
                        DocumentManager.setDocumentoInLavorazione(page, schedaDoc);

                        return;
                    }
                }
            }
            if (fileReq.GetType().Equals(typeof(Allegato)))
            {
                Allegato tempAtt = schedaDoc.allegati.Where(e => AreEqualsAttacments(e, (Allegato)fileReq)).FirstOrDefault();
                if (tempAtt != null)
                {
                    tempAtt = (Allegato)fileReq;
                    DocumentManager.setDocumentoSelezionato(page, schedaDoc);
                    DocumentManager.setDocumentoInLavorazione(page, schedaDoc);
                }

                //for (int i = 0; i < schedaDoc.allegati.Length; i++)
                //{

                //    if(AreEqualsAttacments(schedaDoc.allegati[i], (Allegato)fileReq))
                //    //if (schedaDoc.allegati[i].docNumber == fileReq.docNumber)
                //    {
                //        schedaDoc.allegati[i] = (Allegato)newFileReq;
                //        DocumentManager.setDocumentoSelezionato(page, schedaDoc);
                //        DocumentManager.setDocumentoInLavorazione(page, schedaDoc);
                //        break;;
                //    }
                //}
            }
        }

        private static bool AreEqualsAttacments(Allegato att1, Allegato att2)
        {
            bool retVal = false;
            if (att1.docNumber == null && att2.docNumber == null)
                retVal = att1.repositoryContext != null && att2.repositoryContext != null && att1.repositoryContext.Token.Equals(att2.repositoryContext.Token);
            else
                retVal = att1.docNumber == att2.docNumber;

            return retVal;
        }

        /// <summary>
        /// funzione utilizzata per cancellare le istanze non più utilizzate
        /// </summary>
        public void checkConnecion()
        {
            if (response != null && !response.IsClientConnected)
            {
                fileDoc = null;
                this.delInstance();
            }
        }

        /// <summary>
        /// Funzione per il reperimento del file
        /// </summary>
        /// <param name="page">La pagina richiedente</param>
        /// <param name="versionId">L'id della versione</param>
        /// <param name="versionNumber">Il numero della versione</param>
        /// <param name="docNumber">Il numero del documento</param>
        /// <param name="path">Il path del documento</param>
        /// <param name="fileName">Il nome del file</param>
        /// <param name="showAsPdfFormat">True se bisogna convertire il file</param>
        /// <returns></returns>
        public FileDocumento GetFileAsEML(Page page,
                        string versionId,
                        string versionNumber,
                        string docNumber,
                        string path,
                        string fileName,
                        bool showAsPdfFormat)
        {
            logger.Info("BEGIN");
            // L'oggetto contente i dati per la richiesta del file
            FileRequest fileRequest = new FileRequest();

            // Compilazione dei campi dell'oggetto per la richiesta del file
            fileRequest.versionId = versionId;
            fileRequest.version = versionNumber;
            fileRequest.docNumber = docNumber;
            fileRequest.path = path;
            fileRequest.fileName = fileName;

            // Richiesta file
            FileDocumento res = this.GetFile(page, showAsPdfFormat, fileRequest);
            logger.Info("END");
            return res;
        }


        /// <summary>
        /// Funzione per il reperimento del file
        /// </summary>
        /// <param name="page">La pagina richiedente</param>
        /// <param name="versionId">L'id della versione</param>
        /// <param name="versionNumber">Il numero della versione</param>
        /// <param name="docNumber">Il numero del documento</param>
        /// <param name="path">Il path del documento</param>
        /// <param name="fileName">Il nome del file</param>
        /// <param name="showAsPdfFormat">True se bisogna convertire il file</param>
        /// <returns></returns>
        public FileDocumento GetFile(Page page,
                        string versionId,
                        string versionNumber,
                        string docNumber,
                        string path,
                        string fileName,
                        bool showAsPdfFormat)
        {
            logger.Info("BEGIN");
            // L'oggetto contente i dati per la richiesta del file
            FileRequest fileRequest = new FileRequest();

            // Compilazione dei campi dell'oggetto per la richiesta
            // del file
            fileRequest.versionId = versionId;
            fileRequest.version = versionNumber;
            fileRequest.docNumber = docNumber;
            fileRequest.path = path;
            fileRequest.fileName = fileName;

            // Richiesta file
            
            FileDocumento res= this.GetFile(page, showAsPdfFormat, fileRequest);
            logger.Info("END");
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="convertPdfInline"></param>
        /// <returns></returns>
        public FileDocumento getFile(Page page, bool convertPdfInline)
        {
            return this.getFile(page, getSelectedFile(page), convertPdfInline);
        }

        /// <summary>
        /// Funzione per il reperimento del file
        /// </summary>
        /// <param name="page">La pagina richiedente</param>
        /// <param name="convertPdfInline">True se il documento deve essre convertito in linea</param>
        /// <returns>L'oggetto con le informazioni ed il contenuto del file</returns>
        public FileDocumento GetFile(Page page, bool convertPdfInline, FileRequest fileRequest)
        {
            logger.Info("BEGIN");
            FileRequest fr = null;
            string msgErr = string.Empty;
            bool incache = false;
            bool activeCache = false;
            try
            {
                response = page.Response;

                if (fileRequest == null)
                    fr = getSelectedFile(page);
                else
                    fr = fileRequest;

                if (fr != null)
                {
                    InfoUtente infoUtente = UserManager.getInfoUtente(page);
                    bool isConverted = false;
                    //modifica
                    incache = docsPaWS.inCache(fr.docNumber, fr.versionId, infoUtente.idAmministrazione);
                    activeCache = docsPaWS.isActiveCache(infoUtente.idAmministrazione);

                    if (activeCache)
                    {
                        FileRequest fileR = null;
                        CacheConfig config = docsPaWS.getConfigurazioneCache(infoUtente.idAmministrazione);
               
                        if (incache)
                            fileR = docsPaWS.FileInCache(fr, infoUtente.idAmministrazione);

                        if (fileR != null)
                            fr = fileR;
                        else
                        {
                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fr.fileName);
                            incache = true;
                            //DocsPaWebService ws1 = new DocsPaWebService();
                            //ws1.Url = config.urlwscaching;
                            //ws1.Timeout = System.Threading.Timeout.Infinite;
                            //byte[] content = ws1.streamFileDalServerGenerale(fileRequest.docNumber, fileRequest.versionId);
                            //ws1 = null;

                            if (docsPaWSforCache == null)
                                docsPaWSforCache = ProxyManager.getWSForCache(config.urlwscaching);

                            byte[] content = docsPaWSforCache.streamFileDalServerGenerale(fr.docNumber, fr.versionId);
                  
                            if (content != null)
                            {     
                                string pathfile = config.doc_root_server_locale + "//" + fileInfo.Name;
                                bool ok = docsPaWS.copyFileInCache(content, fr, infoUtente);
                                if (ok)
                                {
                                    fileR = docsPaWS.FileInCache(fr, infoUtente.idAmministrazione);
                                    if (fileR != null)
                                        fr = fileR;
                                }
                                else
                                {
                                    msgErr = "errore durante la copia del file nella cache, riprovare più tardi";
                                    throw new Exception(msgErr);
                                }
                            }
                            else
                                throw new Exception("Errore durante il reperimento del file dal Server Generale");
                        }
                    }
                    string pagina = page.Request.UrlReferrer.ToString();
                    if (string.IsNullOrEmpty(fileRequest.path) &&
                        !pagina.ToLower().Contains("ModalVisualStampaReg".ToLower()) &&
                        !pagina.ToLower().Contains("docVisualizzaFrame".ToLower()) &&
                        !pagina.ToLower().Contains("timestampDocumento".ToLower()) &&
                        activeCache)
                    {
                        fileDoc = new FileDocumento();
                        fileDoc.fullName = fileRequest.fileName;
                        fileDoc.path = fileRequest.path;
                        Exception custom = new Exception(msgErr);
                        ErrorManager.redirectCache(page, custom, false);
                    }
                    else
                    {
                        //fine modifica
                        if (convertPdfInline)
                            fileDoc = docsPaWS.DocumentoGetFileAsPdfFormat(fr, infoUtente, out isConverted);
                        else
                        {
                            if (fileRequest.fileName.ToUpper().EndsWith(".EML"))
                                fileDoc = docsPaWS.DocumentoGetFileDocAsEML(fr, infoUtente, out msgErr);
                            else
                                fileDoc = docsPaWS.DocumentoGetFileDoc(fr, infoUtente, out msgErr);
                        }

                        if (fileDoc == null)
                            throw new Exception();
                        //modifica
                    }
                    //fine modifica
                }
            }
            catch (Exception e)
            {
                if (msgErr == "")
                    if (fr.fileName != null && fr.fileName.Equals(""))
                        msgErr = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br> " + fr.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";
                    else
                        msgErr = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file <br><br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";

                Exception custom = new Exception(msgErr);
                if (incache)
                {
                    bool cachePieno = false;
                    if (e.Message.ToLower().Contains("memoria cache piena"))
                        cachePieno = true;

                    ErrorManager.redirectCache(page, custom, cachePieno); ;
                }
                else
                    ErrorManager.redirect(page, custom);
 
            }
            logger.Info("END");
            return fileDoc;
        }

        public FileDocumento getFileAsEML(Page page, DocsPaWR.FileRequest fileRequest)
        {
            FileDocumento fd = null;
            fd = getFile(page, fileRequest, false);
            //return fd;

            logger.Info("BEGIN");
            string msgErr = string.Empty;
            bool incache = false;
            bool activeCache = false;
            try
            {
                if (fileRequest != null)
                {
                    InfoUtente infoUtente = UserManager.getInfoUtente();
                    bool isConverted = false;

                    //modifica chace
                    incache = docsPaWS.inCache(fileRequest.docNumber, fileRequest.versionId, infoUtente.idAmministrazione);
                    activeCache = docsPaWS.isActiveCache(infoUtente.idAmministrazione);

                    if (activeCache)
                    {
                        FileRequest fileR = null;
                        CacheConfig config = docsPaWS.getConfigurazioneCache(infoUtente.idAmministrazione);
                        if (incache)
                            fileR = docsPaWS.FileInCache(fileRequest, infoUtente.idAmministrazione);

                        if (fileR != null)
                            fileRequest = fileR;
                        else
                        {
                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileRequest.fileName);
                            incache = true;

                            if (docsPaWSforCache == null)
                                docsPaWSforCache = ProxyManager.getWSForCache(config.urlwscaching);

                            byte[] content = docsPaWSforCache.streamFileDalServerGenerale(fileRequest.docNumber, fileRequest.versionId);

                            if (content != null)
                            {
                                string pathfile = config.doc_root_server_locale + "//" + fileInfo.Name;
                                bool ok = docsPaWS.copyFileInCache(content, fileRequest, infoUtente);
                                if (ok)
                                {
                                    fileR = docsPaWS.FileInCache(fileRequest, infoUtente.idAmministrazione);
                                    if (fileR != null)
                                        fileRequest = fileR;
                                }
                                else
                                {
                                    msgErr = "errore durante la copia del file nella cache, riprovare più tardi";
                                    throw new Exception(msgErr);
                                }
                            }
                            else
                                throw new Exception("Errore durante il reperimento del file dal Server Generale");
                        }
                    }
                    string pagina = page.Request.UrlReferrer.ToString();

                    if (string.IsNullOrEmpty(fileRequest.path) &&
                        !pagina.ToLower().Contains("ModalVisualStampaReg".ToLower()) &&
                        !pagina.ToLower().Contains("docVisualizzaFrame".ToLower()) &&
                        !pagina.ToLower().Contains("timestampDocumento".ToLower()) &&
                        activeCache
                        )
                    {
                        fileDoc = new FileDocumento();
                        fileDoc.fullName = fileRequest.fileName;
                        fileDoc.path = fileRequest.path;
                        Exception custom = new Exception(msgErr);
                        ErrorManager.redirectCache(page, custom, false);
                    }
                    else
                    {
                        isConverted = false;
                        fileDoc = docsPaWS.DocumentoGetFileDocAsEML(fileRequest, infoUtente, out msgErr);
                        if (fileDoc == null)
                            throw new Exception();
                    }
                }
            }
            catch (Exception e)
            {
                if (msgErr == "")
                    if (fileRequest.fileName != null && fileRequest.fileName.Equals(""))
                        msgErr = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br> " + fileRequest.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";
                    else
                        msgErr = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file <br><br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";

                Exception custom = new Exception(msgErr);
                if (incache)
                {
                    bool cachePieno = false;
                    if (e.Message.ToLower().Contains("memoria cache piena"))
                        cachePieno = true;
                    ErrorManager.redirectCache(page, custom, cachePieno); ;
                }
                else
                    ErrorManager.redirect(page, custom);
            }
            logger.Info("END");
            return fileDoc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public FileDocumento getFile(Page page, DocsPaWR.FileRequest fileRequest, bool convertPdfInline)
        {
            return this.getFile(page, fileRequest, convertPdfInline, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="fileRequest"></param>
        /// <param name="convertPdfInline"></param>
        /// <returns></returns>
        public FileDocumento getFile(Page page, DocsPaWR.FileRequest fileRequest, bool convertPdfInline, bool getFileFirmato)
        {
            logger.Info("BEGIN");
            string msgErr = string.Empty;
             bool incache = false;
             bool activeCache = false;
            try
            {
                if (fileRequest != null)
                {
                    InfoUtente infoUtente = UserManager.getInfoUtente();

                    bool isConverted = false;

                    //modifica chace
                    incache = docsPaWS.inCache(fileRequest.docNumber, fileRequest.versionId, infoUtente.idAmministrazione);
                    activeCache = docsPaWS.isActiveCache(infoUtente.idAmministrazione);

                    if (activeCache)
                    {
                        FileRequest fileR = null;
                        CacheConfig config = docsPaWS.getConfigurazioneCache(infoUtente.idAmministrazione);
                        if (incache)
                            fileR = docsPaWS.FileInCache(fileRequest, infoUtente.idAmministrazione);

                        if (fileR != null)
                            fileRequest = fileR;

                        else
                        {
                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileRequest.fileName);

                            incache = true;
                            
                            if (docsPaWSforCache == null)
                                docsPaWSforCache = ProxyManager.getWSForCache(config.urlwscaching);

                            byte[] content = docsPaWSforCache.streamFileDalServerGenerale(fileRequest.docNumber, fileRequest.versionId);

                            if (content != null)
                            {
                                string pathfile = config.doc_root_server_locale + "//" + fileInfo.Name;
                                bool ok = docsPaWS.copyFileInCache(content, fileRequest, infoUtente);
                                if (ok)
                                {
                                    fileR = docsPaWS.FileInCache(fileRequest, infoUtente.idAmministrazione);
                                    if (fileR != null)
                                        fileRequest = fileR;
                                }
                                else
                                {
                                    msgErr = "errore durante la copia del file nella cache, riprovare più tardi";
                                    throw new Exception(msgErr);
                                }
                            }
                            else
                                throw new Exception("Errore durante il reperimento del file dal Server Generale");
                        }
                    }
                       string pagina = page.Request.UrlReferrer.ToString();
                   
                    if (string.IsNullOrEmpty(fileRequest.path) &&
                        !pagina.ToLower().Contains("ModalVisualStampaReg".ToLower()) && 
                        !pagina.ToLower().Contains("docVisualizzaFrame".ToLower()) &&
                        !pagina.ToLower().Contains("timestampDocumento".ToLower()) &&
                        activeCache
                        )
                    {
                        fileDoc = new FileDocumento();
                        fileDoc.fullName = fileRequest.fileName;
                        fileDoc.path = fileRequest.path;
                        Exception custom = new Exception(msgErr);
                        ErrorManager.redirectCache(page, custom, false);
                    }
                    else
                    {
                        isConverted = false;

                        if (convertPdfInline)
                            fileDoc = docsPaWS.DocumentoGetFileAsPdfFormat(fileRequest, infoUtente, out isConverted);
                        else if (getFileFirmato)
                            fileDoc = docsPaWS.DocumentoGetFileFirmato(fileRequest, infoUtente);
                        else
                            fileDoc = docsPaWS.DocumentoGetFileDoc(fileRequest, infoUtente, out msgErr);

                        if (fileDoc == null)
                            throw new Exception();
                    }
                    //fine modifica cache
                }
            }
            catch (Exception e)
            {
                if (msgErr == "")
                    if (fileRequest.fileName != null && fileRequest.fileName.Equals(""))
                        msgErr = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br> " + fileRequest.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";
                    else
                        msgErr = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file <br><br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";

                Exception custom = new Exception(msgErr);
                if (incache)
                {
                    bool cachePieno = false;
                    if (e.Message.ToLower().Contains("memoria cache piena"))
                        cachePieno = true;

                    ErrorManager.redirectCache(page, custom, cachePieno); ;
                }
                else
                    ErrorManager.redirect(page, custom);
 
            }
            logger.Info("END");
            return fileDoc;
        }

        /// <summary>
        /// Funzione utilizzata per il reperimento del file con segnatura quando non si ha a disposizione
        /// la scheda documento
        /// </summary>
        /// <param name="page">La pagina richiamante</param>
        /// <param name="versionId">L'id della versione</param>
        /// <param name="versionNumber">Il numero della versione</param>
        /// <param name="docNumber">Il numero del documento</param>
        /// <param name="path">Il path del documento</param>
        /// <param name="fileName">Il nome del file</param>
        /// <param name="position">Informazioni sulla label</param>
        /// <returns>L'oggetto con il documento</returns>
        public FileDocumento GetFileConSegnatura(Page page, string versionId, string versionNumber, string docNumber,
            string path, string fileName, labelPdf position)
        {
            logger.Info("BEGIN");
            // Costruzione del file request
            FileRequest fileRequest = new FileRequest();

            fileRequest.versionId = versionId;
            fileRequest.version = versionNumber;
            fileRequest.docNumber = docNumber;
            fileRequest.path = path;
            fileRequest.fileName = fileName;

            // Il risultato da restituire
            FileDocumento result = null;

            // Per questa operazione è necessaria la scheda documento
            SchedaDocumento sch = DocumentManager.getDettaglioDocumento(
                page, null, docNumber);

            try
            {
                result = this.getFileConSegnatura(page, sch, position, fileRequest);
            }
            catch (Exception e)
            {
                throw e;
            }
            logger.Info("END");
            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="fileRequest"></param>
        /// <param name="sch"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public FileDocumento getFileConSegnatura(Page page, DocsPaWR.FileRequest fileRequest, DocsPaWR.SchedaDocumento sch, labelPdf position)
        {
            logger.Info("BEGIN");
            bool incache = false;
            bool activeCache = false;
            try
            {
                if (fileRequest == null && fileDoc != null)
                    return fileDoc;

                if (fileRequest != null)
                {
                    InfoUtente infoUtente = UserManager.getInfoUtente(page);
                    //modifica
                    incache = docsPaWS.inCache(fileRequest.docNumber, fileRequest.versionId, infoUtente.idAmministrazione);
                    activeCache = docsPaWS.isActiveCache(infoUtente.idAmministrazione);

                    if (activeCache)
                    {
                        FileRequest fileR = null;
                        CacheConfig config = docsPaWS.getConfigurazioneCache(infoUtente.idAmministrazione);
                  if (incache)
                      fileR = docsPaWS.FileInCache(fileRequest, infoUtente.idAmministrazione);

                        if (fileR != null)
                            fileRequest = fileR;

                        else
                        {
                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileRequest.fileName);
                      
                            incache = true;
                            //DocsPaWebService ws1 = new DocsPaWebService();
                            //ws1.Url = config.urlwscaching;
                            //ws1.Timeout = System.Threading.Timeout.Infinite;
                            //byte[] content = ws1.streamFileDalServerGenerale(fileRequest.docNumber, fileRequest.versionId);
                            //ws1 = null;

                            if (docsPaWSforCache == null)
                                docsPaWSforCache = ProxyManager.getWSForCache(config.urlwscaching);

                            byte[] content = docsPaWSforCache.streamFileDalServerGenerale(fileRequest.docNumber, fileRequest.versionId);

                            if (content != null)
                            {
                                string pathfile = config.doc_root_server_locale + "//" + fileInfo.Name;
                                bool ok = docsPaWS.copyFileInCache(content, fileRequest, infoUtente);
                                if (ok)
                                {
                                    fileR = docsPaWS.FileInCache(fileRequest, infoUtente.idAmministrazione);
                                    if (fileR != null)
                                        fileRequest = fileR;
                                }
                                else
                                {
                                    string msgErr = "errore durante la copia del file nella cache, riprovare più tardi";
                                    throw new Exception(msgErr);
                                }
                            }
                            else
                                throw new Exception("Errore durante il reperimento del file dal Server Generale");
                        }
                    }
              
              string pagina = page.Request.UrlReferrer.ToString();
              if (string.IsNullOrEmpty(fileRequest.path) &&
                  !pagina.ToLower().Contains("ModalVisualStampaReg".ToLower()) && 
                  !pagina.ToLower().Contains("docVisualizzaFrame".ToLower()) &&
                  !pagina.ToLower().Contains("timestampDocumento".ToLower()) &&
                  activeCache
                  )
              {
                  fileDoc = new FileDocumento();
                  fileDoc.fullName = fileRequest.fileName;
                  fileDoc.path = fileRequest.path;
                  Exception custom = new Exception(string.Empty);
                  ErrorManager.redirectCache(page, custom, false);
              }
              else
              {
                  //fine modifica
                  fileDoc = docsPaWS.DocumentoGetFileConSegnatura(fileRequest, sch, infoUtente, position, false);

                  if (fileDoc == null)
                  {
                      throw new Exception();
                  }
                  //modifica
              }
                    //fine modifica
                }
                //this.delInstance();
            }
            catch(Exception e)
            {
                string msg = string.Empty;
                if (fileRequest.fileName != null && fileRequest.fileName.Equals(""))
                    msg = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br> " + fileRequest.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";
                else
                    msg = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file <br><br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";

                Exception custom = new Exception(msg);
                if (incache)
                {
                    bool cachePieno = false;
                    if (e.Message.ToLower().Contains("memoria cache piena"))
                        cachePieno = true;

                    ErrorManager.redirectCache(page, custom, cachePieno); ;
                }
                else
                    ErrorManager.redirect(page, custom);
 
            }
            logger.Info("END");
            return fileDoc;
        }

        public FileDocumento getFileConSegnatura(Page page, DocsPaWR.SchedaDocumento sch, labelPdf position,
            FileRequest fileRequest)
        {
            logger.Info("BEGIN");
            bool incache = false;
            bool activeCache = false;
            FileRequest fileReq = null;
            try
            {
                response = page.Response;

                if (sch == null)
                    fileReq = fileRequest;
                else
                    fileReq = getSelectedFile(page);

                if (fileReq == null && fileDoc != null)
                {
                    return fileDoc;
                }
                if (fileReq != null)
                {
                    InfoUtente infoUtente = UserManager.getInfoUtente(page);

                    //modifica
                    incache = docsPaWS.inCache(fileRequest.docNumber, fileRequest.versionId, infoUtente.idAmministrazione);
                    activeCache = docsPaWS.isActiveCache(infoUtente.idAmministrazione);

                    if (activeCache)
                    {
                        FileRequest fileR = null;
                        CacheConfig config = docsPaWS.getConfigurazioneCache(infoUtente.idAmministrazione);
                        
                        if (incache)
                            fileR = docsPaWS.FileInCache(fileRequest, infoUtente.idAmministrazione);

                        if (fileR != null)
                            fileRequest = fileR;

                        else
                        {
                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileRequest.fileName);
                            incache = true;
                            //DocsPaWebService ws1 = new DocsPaWebService();
                            //ws1.Url = config.urlwscaching;
                            //ws1.Timeout = System.Threading.Timeout.Infinite;
                            //byte[] content = ws1.streamFileDalServerGenerale(fileRequest.docNumber, fileRequest.versionId);
                            //ws1 = null;

                            if (docsPaWSforCache == null)
                                docsPaWSforCache = ProxyManager.getWSForCache(config.urlwscaching);

                            byte[] content = docsPaWSforCache.streamFileDalServerGenerale(fileReq.docNumber, fileReq.versionId);

                            if (content != null)
                            {
                                string pathfile = config.doc_root_server_locale + "//" + fileInfo.Name;
                                bool ok = docsPaWS.copyFileInCache(content, fileRequest, infoUtente);
                                if (ok)
                                {
                                    fileR = docsPaWS.FileInCache(fileRequest, infoUtente.idAmministrazione);
                                    if (fileR != null)
                                        fileRequest = fileR;
                                }
                                else
                                {
                                    string msgErr = "errore durante la copia del file nella cache, riprovare più tardi";
                                    throw new Exception(msgErr);
                                }
                            }
                            else
                                throw new Exception("Errore durante il reperimento del file dal Server Generale");
                        }
                    }
              
               string pagina = page.Request.UrlReferrer.ToString();
               if (string.IsNullOrEmpty(fileRequest.path) &&
                   !pagina.ToLower().Contains("ModalVisualStampaReg".ToLower()) && 
                   !pagina.ToLower().Contains("docVisualizzaFrame".ToLower()) &&
                   !pagina.ToLower().Contains("timestampDocumento".ToLower()) &&
                   activeCache
                   )
               {
                   fileDoc = new FileDocumento();
                   fileDoc.fullName = fileRequest.fileName;
                   fileDoc.path = fileRequest.path;
                   Exception custom = new Exception(string.Empty);
                   ErrorManager.redirectCache(page, custom, false);
               }
               else
               {
                   //fine modifica
                   fileDoc = docsPaWS.DocumentoGetFileConSegnatura(fileReq, sch, infoUtente, position, false);

                   if (fileDoc == null)
                   {
                       throw new Exception();
                   }
                   //modifica
               }
                    //fine modifica
             }
                //this.delInstance();
                if (fileDoc != null && page.Request.QueryString["save"] != null && page.Request.QueryString["save"].Equals("true"))
                {
                    //Creo la nuova versione per il doc con impressa la segnatura
                    uploadFile(page, fileDoc, true);
                    //aggiorno cha_segnature su VERSIONS(lo metto ad uno per indicare che il file acquisito ha impressa la segnatura)
                    docsPaWS.DocumentoVersioneConSegnatura(UserManager.getInfoUtente(), getSelectedFile().versionId);
                }

            }
            catch(Exception e)
            {
                string msg = string.Empty;
                if (fileReq.fileName != null && fileReq.fileName.Equals(""))
                    msg = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br> " + fileReq.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";
                else
                    msg = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file <br><br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";

                Exception custom = new Exception(msg);
                if (incache)
                {
                    bool cachePieno = false;
                    if (e.Message.ToLower().Contains("memoria cache piena"))
                        cachePieno = true;

                    ErrorManager.redirectCache(page, custom, cachePieno); ;
                }
                else
                    ErrorManager.redirect(page, custom);
 
            }
            logger.Info("END");
            return fileDoc;
        }

        public FileDocumento getVoidFileConSegnatura(DocsPaVO.documento.FileRequest fileRequest, DocsPaWR.SchedaDocumento sch, labelPdf position, Page pagina)
        {
            FileRequest fileReq = null;
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(pagina);
                fileDoc = docsPaWS.DocumentoGetFileConSegnatura(fileReq, sch, infoUtente, position, true);

                if (fileDoc == null)
                {
                    throw new Exception();
                }

            }
            catch (Exception ex)
            {
                ErrorManager.redirect(pagina, ex);
            }

            return fileDoc;

        }

        public FileDocumento getFile(Page page)
        {
            logger.Info("BEGIN");
            bool incache = false;
            bool activeCache = false;
            string msgErr = string.Empty;
            FileRequest fileReq = null;
            try
            {
                response = page.Response;
                fileReq = getSelectedFile(page);
                if (fileReq == null && fileDoc != null)
                {
                    return fileDoc;
                }
                if (fileReq != null)
                {
                    InfoUtente infoUtente = UserManager.getInfoUtente(page);

                    //modifica

                    incache = docsPaWS.inCache(fileReq.docNumber, fileReq.versionId, infoUtente.idAmministrazione);
                    activeCache = docsPaWS.isActiveCache(infoUtente.idAmministrazione);

                    if (activeCache)
                    {
                        FileRequest fileR = null;
                        CacheConfig config = docsPaWS.getConfigurazioneCache(infoUtente.idAmministrazione);
                        System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileReq.fileName);
                        if (incache)
                            fileR = docsPaWS.FileInCache(fileReq, infoUtente.idAmministrazione);

                        if (fileR != null)
                            fileReq = fileR;

                        else
                        {
                            incache = true;
                            //DocsPaWebService ws1 = new DocsPaWebService();
                            //ws1.Url = config.urlwscaching;
                            //ws1.Timeout = System.Threading.Timeout.Infinite;
                            if (docsPaWSforCache == null)
                                docsPaWSforCache = ProxyManager.getWSForCache(config.urlwscaching);

                            byte[] content = docsPaWSforCache.streamFileDalServerGenerale(fileReq.docNumber, fileReq.versionId);
                                //ws1.streamFileDalServerGenerale(fileReq.docNumber, fileReq.versionId);
                            //ws1 = null;
                            if (content != null)
                            {
                                string pathfile = config.doc_root_server_locale + "//" + fileInfo.Name;
                                bool ok = docsPaWS.copyFileInCache(content, fileReq, infoUtente);
                                if (ok)
                                {
                                    fileR = docsPaWS.FileInCache(fileReq, infoUtente.idAmministrazione);
                                    if (fileR != null)
                                        fileReq = fileR;
                                }
                                else
                                {
                                    msgErr = "errore durante la copia del file nella cache, riprovare più tardi";
                                    throw new Exception(msgErr);
                                }
                            }
                            else
                                throw new Exception("Errore durante il reperimento del file dal Server Generale");
                        }
                    }
                 
                 string pagina = page.Request.UrlReferrer.ToString();
                 if (string.IsNullOrEmpty(fileReq.path) &&
                     !pagina.ToLower().Contains("ModalVisualStampaReg".ToLower()) && 
                     !pagina.ToLower().Contains("docVisualizzaFrame".ToLower()) &&
                     !pagina.ToLower().Contains("timestampDocumento".ToLower()) && 
                     activeCache
                     )
                 {
                     fileDoc = new FileDocumento();
                     fileDoc.fullName = fileReq.fileName;
                     fileDoc.path = fileReq.path;
                     Exception custom = new Exception(msgErr);
                     ErrorManager.redirectCache(page, custom, false);
                 }
                 else
                 {
                     //fine modifica
                     fileDoc = docsPaWS.DocumentoGetFileDoc(fileReq, infoUtente, out msgErr);

                     if (fileDoc == null)
                     {
                         throw new Exception();
                     }
                     //modifica
                 }
                    //fine modifica
                }
                //this.delInstance();
            }
            catch(Exception e)
            {

                if (msgErr == "")
                    if (fileReq.fileName != null && fileReq.fileName.Equals(""))
                        msgErr = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br> " + fileReq.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";
                    else
                        msgErr = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file <br><br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";

                Exception custom = new Exception(msgErr);
                if (incache)
                {
                    bool cachePieno = false;
                    if (e.Message.ToLower().Contains("memoria cache piena"))
                        cachePieno = true;

                    ErrorManager.redirectCache(page, custom, cachePieno); ;
                }
                else
                    ErrorManager.redirect(page, custom);
 
            }
            logger.Info("END");
            return fileDoc;
        }

        public FileDocumento getInfoFile(Page page, FileRequest fileRequest)
        {
            FileDocumento fileDocument = null;
            bool incache = false;
            bool activeCache = false;
            try
            {
                if (fileRequest != null)
                {
                    InfoUtente infoUtente = UserManager.getInfoUtente();
                    fileDocument = docsPaWS.DocumentoGetInfoFile(fileRequest, infoUtente);
                }
            }
            catch(Exception e)
            {
                string msg = string.Empty;
                if (fileRequest.fileName != null && fileRequest.fileName.Equals(""))
                    msg = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br> " + fileRequest.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";
                else
                    msg = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file <br><br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";

                Exception custom = new Exception(msg);
                    ErrorManager.redirect(page, custom);
 
            }

            return fileDocument;
        }

        public FileDocumento getInfoFile(Page page)
        {
            FileRequest fileReq = null;
            bool incache = false;
            bool activeCache = false;

            try
            {
                response = page.Response;
                fileReq = getSelectedFile(page);
                if (fileReq == null && fileDoc != null)
                {
                    return fileDoc;
                }
                if (fileReq != null && fileReq.fileSize != "0")
                {
                    InfoUtente infoUtente = UserManager.getInfoUtente(page);
                    fileDoc = docsPaWS.DocumentoGetInfoFile(fileReq, infoUtente);

                    if (fileDoc == null)
                    {
                        throw new Exception();
                    }
                }
                //this.delInstance();
            }
            catch(Exception e)
            {
                string msg = string.Empty;
                if (fileReq.fileName != null && fileReq.fileName.Equals(""))
                    msg = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br> " + fileReq.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";
                else
                    msg = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file <br><br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";

                Exception custom = new Exception(msg);
                ErrorManager.redirect(page, custom);
 
            }
            return fileDoc;
        }
        public FileDocumento getReport(Page page)
        {
            try
            {
                response = page.Response;
                return getSelectedReport(page);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
            return null;
        }
        /// <summary>
        /// restituisce l'estensone del file, se p7m l'estensione originale del documento contenuto nella busta o nelle buste pkcs7.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="estensione"></param>
        /// <returns></returns>
        public static string getEstensioneIntoSignedFile(Page page, string fullname)
        {
            string retValue = string.Empty;
            // Reperimento del nome del file con estensione
            string fileName = new System.IO.FileInfo(fullname).Name;

            string[] items = fileName.Split('.');
            
            // SKIZZO decommentare per icona giusta.
            for (int i = (items.Length - 1); i >= 0; i--)
            {
                if (!(items[i].ToUpper().EndsWith("P7M") 
                    || items[i].ToUpper().EndsWith("TSD") ||
                    items[i].ToUpper().EndsWith("M7M")
                    ))
                {
                    retValue = items[i];
                    break;
                }
            }

            return retValue;

            //string estensione = fullname.Substring(fullname.IndexOf(".") + 1);
            //if (estensione.ToUpper().EndsWith("P7M"))
            //    estensione = estensione.Substring(0, estensione.IndexOf("."));
            //return estensione;							

        }
        public static string getFileIcon(Page page, string estensione)
        {
            string icona = "icon_gen.gif";
            try
            {
                string ext = string.Empty;
                System.IO.FileInfo fi = null;
                if (estensione.IndexOf(".") != -1)
                {
                    fi = new System.IO.FileInfo(estensione);
                    ext = fi.Extension;
                    ext = ext.Replace(".", "");
                }
                else
                    ext = estensione;

                TypeIcon tipo = TypeIcon.getInstance();
                icona = (string)tipo[ext.ToLower()];
                if (icona == null)
                {
                    string pathAppo = HttpContext.Current.Request.PhysicalApplicationPath;
                    string path = pathAppo + @"images\tabDocImages\icon_" + ext + ".gif";
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
                    if (fileInfo.Exists)
                    {
                        tipo.Add(ext, "icon_" + ext + ".gif");
                        icona = "icon_" + ext + ".gif";
                    }
                    else
                    {
                        icona = "icon_gen.gif";
                    }
                }
            }
            catch (Exception) { }
            return "../images/tabDocImages/" + icona;
        }
        /// <summary>
        /// Impostazione valore in sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="sessionValue"></param>
        private static void SetSessionValue(string sessionKey, object sessionValue)
        {
            System.Web.HttpContext.Current.Session[sessionKey] = sessionValue;
        }

        /// <summary>
        /// Reperimento valore da sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        private static Object GetSessionValue(string sessionKey)
        {
            return System.Web.HttpContext.Current.Session[sessionKey];
        }

        /// <summary>
        /// Rimozione chiave di sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        private static void RemoveSessionValue(string sessionKey)
        {
            System.Web.HttpContext.Current.Session.Remove(sessionKey);
        }

        #region Metodo per la verifica esistenza file nel path delle immagini
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool fileExist(string fileName, string type)
        {
            string serverPath = docsPaWS.getpath(type);
            if (serverPath == "")
                throw new Exception("Attenzione, chiave configurazione " + type + " non valorizzata.");
            return System.IO.File.Exists(serverPath + "\\" + fileName);
        }
        #endregion

        public static string findFontColor(string idAmm)
        {
            return docsPaWS.findFontColor(idAmm);
        }

        public static string findPulsColor(string idAmm)
        {
            return docsPaWS.findPulsColor(idAmm);
        }

        public static bool changeColor(string idAmm, string color)
        {
            return docsPaWS.changeColor(idAmm, color);
        }

        public static bool changePulsColor(string idAmm, string color)
        {
            return docsPaWS.changePulsColor(idAmm, color);
        }

        public static string findRecords(string idAmm)
        {
            return docsPaWS.findRecords(idAmm);
        }

        public static bool modifyRecordNumber(string idAmm, char chk, int num)
        {
            return docsPaWS.modifyRecordNumber(idAmm, chk, num);
        }

        /// <summary>
        /// Metodo che permette la messa in coda di un file per la conversione pdf lato server
        /// </summary>
        /// <param name="page"></param>
        /// <param name="content"></param>
        /// <param name="filename"></param>
        public static void EnqueueServerPdfConversion(Page page, InfoUtente infoUtente, byte[] content, string filename, SchedaDocumento schDoc)
        {
            try
            {
                ObjServerPdfConversion objServerPdfConversion = new ObjServerPdfConversion();
                objServerPdfConversion.content = content;
                objServerPdfConversion.fileName = filename;

                //SchedaDocumento schDoc = DocumentManager.getDocumentoInLavorazione(page);
                objServerPdfConversion.idProfile = schDoc.systemId;
                objServerPdfConversion.docNumber = schDoc.docNumber;

                if (!string.IsNullOrEmpty(objServerPdfConversion.idProfile) && !string.IsNullOrEmpty(objServerPdfConversion.docNumber))
                    docsPaWS.EnqueueServerPdfConversion(infoUtente, objServerPdfConversion);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        /// <summary>
        /// Metodo che permette la messa in coda di un file per la conversione pdf lato server
        /// </summary>
        /// <param name="page"></param>
        /// <param name="content"></param>
        /// <param name="filename"></param>
        public static void EnqueueServerPdfConversionAM(Page page, InfoUtente infoUtente, byte[] content, string filename, SchedaDocumento schDoc)
        {
            try
            {
                ObjServerPdfConversion objServerPdfConversion = new ObjServerPdfConversion();
                objServerPdfConversion.content = content;
                objServerPdfConversion.fileName = filename;

                //SchedaDocumento schDoc = DocumentManager.getDocumentoInLavorazione(page);
                objServerPdfConversion.idProfile = schDoc.systemId;
                objServerPdfConversion.docNumber = schDoc.docNumber;

                if (!string.IsNullOrEmpty(objServerPdfConversion.idProfile) && !string.IsNullOrEmpty(objServerPdfConversion.docNumber))
                    docsPaWS.EnqueueServerPdfConversionAM(infoUtente, objServerPdfConversion);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public void uploadFile(Page page, FileDocumento fileDoc, bool addVersion, bool conversionePdfServer, bool conversionePdfServerSincrona)
        {
            logger.Info("BEGIN");
            bool newVersion = addVersion;
            FileDocumento convResult = fileDoc;

            try
            {
                // Se è rischiesta la coversione sincrona...
                if (conversionePdfServerSincrona)
                {
                    // ...si deve sostituire il content del documento con il content convertito in PDF
                    FileDocumento app = docsPaWS.GeneratePDFInSyncMod(fileDoc);

                    // ...se la ocnversione sincrona è andata a buon fine...
                    if (app != null)
                    {
                        // ...si deve impedire la creazione di una nuova versione...
                        newVersion = false;

                        // ...e si deve sostituire il fileDoc con convResult
                        convResult = app;

                    }

                }

                FileRequest fileReq = getSelectedFile(page);

                fileReq.cartaceo = convResult.cartaceo;

                bool daInviare = true;
                if (newVersion)
                {
                    if (fileReq.fileName.Substring(fileReq.fileName.LastIndexOf(".") + 1).ToUpper().Equals("P7M"))
                        daInviare = false;
                    Documento documento = new Documento();
                    documento.descrizione = "File convertito";
                    documento.docNumber = fileReq.docNumber;
                    fileReq = DocumentManager.aggiungiVersione(page, documento, daInviare, false);
                }

                if (fileReq != null)
                {
                    fileName = convResult.fullName;
                    chiudi = false;
                    InfoUtente infoUtente = UserManager.getInfoUtente(page);

                    kbTotal = Convert.ToString(Math.Round((double)convResult.length / 1024, 2));
                    //bytesDone = 0;		
                    //int bytesToRead = 1024;

                    //startTime = System.DateTime.Now;

                    note = "Salvataggio del file...";

                    chiudi = true;

                    //Chiamo il metodo che verifica se è un form pdf da processare
                    processFormPdf(page, fileDoc, fileReq);

                    string nomeFile = fileReq.fileName;
                    fileReq.fileSize = fileDoc.content.Length.ToString();
                    fileReq = docsPaWS.DocumentoPutFile(fileReq, convResult, infoUtente);



                    if (fileReq == null)
                    {

                        throw new Exception("Attenzione,<BR>l'acquisizione file non effettuata.");
                    }
                    aggiornaFileRequest(page, fileReq);

                    //Se abilitata la conversione lato server asincrona (e non quella sincrona), chiamo il webmethod che mette in coda il file da convertire
                    if (conversionePdfServer && !conversionePdfServerSincrona)
                    {
                        bool isAllegato = (fileReq.GetType() == typeof(DocsPaWR.Allegato));

                        DocsPaWR.SchedaDocumento schedaDocumentoCorrente = null;

                        if (isAllegato)
                            schedaDocumentoCorrente = CheckInOut.CheckInOutServices.CurrentSchedaDocumento;
                        else
                            schedaDocumentoCorrente = DocumentManager.getDocumentoInLavorazione(page);

                        EnqueueServerPdfConversion(page,
                                                    UserManager.getInfoUtente(page),
                                                    fileDoc.content,
                                                    fileDoc.name,
                                                    schedaDocumentoCorrente);
                    }

                    //Chiamo il metodo che verifica se è un form pdf da processare
                    //processFormPdf(page, fileDoc);

                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
            logger.Info("END");
        }

        private static void processFormPdf(Page page, FileDocumento fileDocumento, FileRequest fileRequest)
        {
            try
            {
                SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato(page);

                if (schedaDocumento.template == null && System.IO.Path.GetExtension(fileDocumento.fullName).ToUpper() == ".PDF" && docsPaWS.FunzioneEsistente("PROCESS_FORM_PDF"))
                {
                    InfoUtente infoUtente = UserManager.getInfoUtente(page);
                    DocsPaWR.ProcessFormInput processFormInput = new ProcessFormInput();
                    processFormInput.schedaDocumentoInput = schedaDocumento;
                    processFormInput.fileDocumentoInput = fileDocumento;

                    docsPaWS.Timeout = System.Threading.Timeout.Infinite; ;
                    DocsPaWR.ProcessFormOutput processFormOutput = docsPaWS.ProcessFormPdf(infoUtente, processFormInput);

                    if (processFormOutput != null && processFormOutput.schedaDocumentoOutput != null)
                    {
                        DocumentManager.setDocumentoSelezionato(processFormOutput.schedaDocumentoOutput);
                        if (processFormOutput.schedaDocumentoOutput.documenti != null && processFormOutput.schedaDocumentoOutput.documenti.Length >= 1)
                            fileRequest.firmato = ((DocsPaWR.FileRequest)processFormOutput.schedaDocumentoOutput.documenti[0]).firmato;
                        //SiteNavigation.CallContextStack.CurrentContext.ContextState["ProcessPdfLiveCycle"] = true;
                        SiteNavigation.CallContextStack.CurrentContext.ContextState["ProcessPdfLiveCycle"] = processFormOutput;
                    }
                }

                if (schedaDocumento.template == null && (System.IO.Path.GetExtension(fileDocumento.fullName).ToUpper() == ".TIF" || System.IO.Path.GetExtension(fileDocumento.fullName).ToUpper() == ".TIFF") && docsPaWS.FunzioneEsistente("PROCESS_FORM_PDF"))
                {
                    InfoUtente infoUtente = UserManager.getInfoUtente(page);
                    DocsPaWR.ProcessFormInput processFormInput = new ProcessFormInput();
                    processFormInput.schedaDocumentoInput = schedaDocumento;
                    processFormInput.fileDocumentoInput = fileDocumento;

                    docsPaWS.Timeout = System.Threading.Timeout.Infinite; ;
                    DocsPaWR.ProcessFormOutput processFormOutput = docsPaWS.ProcessBarcodeFormPdf(infoUtente, processFormInput);

                    if (processFormOutput != null && processFormOutput.schedaDocumentoOutput != null)
                    {
                        DocumentManager.setDocumentoSelezionato(processFormOutput.schedaDocumentoOutput);
                        if (processFormOutput.schedaDocumentoOutput.documenti != null && processFormOutput.schedaDocumentoOutput.documenti.Length >= 1)
                            fileRequest.firmato = ((DocsPaWR.FileRequest)processFormOutput.schedaDocumentoOutput.documenti[0]).firmato;
                        //SiteNavigation.CallContextStack.CurrentContext.ContextState["ProcessPdfLiveCycle"] = true;
                        SiteNavigation.CallContextStack.CurrentContext.ContextState["ProcessPdfLiveCycle"] = processFormOutput;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }
    }
}
