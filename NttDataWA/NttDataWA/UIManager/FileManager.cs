using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Linq;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;

namespace NttDataWA.UIManager
{
    public class FileManager
    {
        #region Fields
        public static bool chiudi = false;
        public string PageID;
        private HttpResponse response = null;
        private FileDocumento fileDoc = null;
        private FileDocumentoAnteprima fileDocA = null;
        private static Hashtable fileManager;
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();
        #endregion

        #region Constructors


        private FileManager(string ID)
        {
            if (fileManager != null)
            {
                fileManager.Clear();
                fileDoc = null;
                fileManager = null;
            }
            fileManager = new Hashtable();
            PageID = ID;
            chiudi = false;
        }

        public FileManager() { }

        #endregion

        #region Methods
        static public string GetMimeType(string filename)
        {
            try
            {
                string ext = System.IO.Path.GetExtension(filename);
                if (!string.IsNullOrEmpty(ext))
                {
                    ext = ext.ToLower();

                    InitMimeMap();
                    string mime;
                    if (mimemap.TryGetValue(ext, out mime))
                    {
                        return mime;
                    }
                }
                return "application/unknown";
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        static System.Collections.Generic.Dictionary<string, string> mimemap;

        static void InitMimeMap()
        {
            try
            {
                if (mimemap != null) return;
                lock (typeof(FileManager))
                {
                    if (mimemap != null) return;

                    System.Collections.Generic.Dictionary<string, string> map = new System.Collections.Generic.Dictionary<string, string>();

                    #region copy from registry of my pc
                    map.Add(".323", "text/h323");
                    map.Add(".act", "text/xml");
                    map.Add(".actproj", "text/plain");
                    map.Add(".addin", "text/xml");
                    map.Add(".ai", "application/postscript");
                    map.Add(".aif", "audio/aiff");
                    map.Add(".aifc", "audio/aiff");
                    map.Add(".aiff", "audio/aiff");
                    map.Add(".application", "application/x-ms-application");
                    map.Add(".asax", "application/xml");
                    map.Add(".ascx", "application/xml");
                    map.Add(".asf", "video/x-ms-asf");
                    map.Add(".ashx", "application/xml");
                    map.Add(".asmx", "application/xml");
                    map.Add(".aspx", "application/xml");
                    map.Add(".asx", "video/x-ms-asf");
                    map.Add(".au", "audio/basic");
                    map.Add(".avi", "video/avi");
                    map.Add(".bmp", "image/bmp");
                    map.Add(".cat", "application/vnd.ms-pki.seccat");
                    map.Add(".cd", "text/plain");
                    map.Add(".cer", "application/x-x509-ca-cert");
                    map.Add(".config", "application/xml");
                    map.Add(".crl", "application/pkix-crl");
                    map.Add(".crt", "application/x-x509-ca-cert");
                    map.Add(".cs", "text/plain");
                    map.Add(".csdproj", "text/plain");
                    map.Add(".csproj", "text/plain");
                    map.Add(".css", "text/css");
                    map.Add(".datasource", "application/xml");
                    map.Add(".dbs", "text/plain");
                    map.Add(".der", "application/x-x509-ca-cert");
                    map.Add(".dib", "image/bmp");
                    map.Add(".dll", "application/x-msdownload");
                    map.Add(".dtd", "application/xml-dtd");
                    map.Add(".doc", "application/msword");
                    map.Add(".docx", "application/msword");
                    map.Add(".eml", "message/rfc822");
                    map.Add(".eps", "application/postscript");
                    map.Add(".eta", "application/earthviewer");
                    map.Add(".etp", "text/plain");
                    map.Add(".exe", "application/x-msdownload");
                    map.Add(".ext", "text/plain");
                    map.Add(".fif", "application/fractals");
                    map.Add(".fky", "text/plain");
                    map.Add(".gif", "image/gif");
                    map.Add(".gz", "application/x-gzip");
                    map.Add(".hqx", "application/mac-binhex40");
                    map.Add(".hta", "application/hta");
                    map.Add(".htc", "text/x-component");
                    map.Add(".htm", "text/html");
                    map.Add(".html", "text/html");
                    map.Add(".htt", "text/webviewhtml");
                    map.Add(".hxa", "application/xml");
                    map.Add(".hxc", "application/xml");
                    map.Add(".hxd", "application/octet-stream");
                    map.Add(".hxe", "application/xml");
                    map.Add(".hxf", "application/xml");
                    map.Add(".hxh", "application/octet-stream");
                    map.Add(".hxi", "application/octet-stream");
                    map.Add(".hxk", "application/xml");
                    map.Add(".hxq", "application/octet-stream");
                    map.Add(".hxr", "application/octet-stream");
                    map.Add(".hxs", "application/octet-stream");
                    map.Add(".hxt", "application/xml");
                    map.Add(".hxv", "application/xml");
                    map.Add(".hxw", "application/octet-stream");
                    map.Add(".ico", "image/x-icon");
                    map.Add(".iii", "application/x-iphone");
                    map.Add(".ins", "application/x-internet-signup");
                    map.Add(".isp", "application/x-internet-signup");
                    map.Add(".jfif", "image/jpeg");
                    map.Add(".jpe", "image/jpeg");
                    map.Add(".jpeg", "image/jpeg");
                    map.Add(".jpg", "image/jpeg");
                    map.Add(".kci", "text/plain");
                    map.Add(".kml", "application/vnd.google-earth.kml+xml");
                    map.Add(".kmz", "application/vnd.google-earth.kmz");
                    map.Add(".latex", "application/x-latex");
                    map.Add(".lgn", "text/plain");
                    map.Add(".m1v", "video/mpeg");
                    map.Add(".m3u", "audio/x-mpegurl");
                    map.Add(".man", "application/x-troff-man");
                    map.Add(".master", "application/xml");
                    map.Add(".mfp", "application/x-shockwave-flash");
                    map.Add(".mht", "message/rfc822");
                    map.Add(".mhtml", "message/rfc822");
                    map.Add(".mid", "audio/mid");
                    map.Add(".midi", "audio/mid");
                    map.Add(".mp2", "video/mpeg");
                    map.Add(".mp2v", "video/mpeg");
                    map.Add(".mp3", "audio/mpeg");
                    map.Add(".mpa", "video/mpeg");
                    map.Add(".mpe", "video/mpeg");
                    map.Add(".mpeg", "video/mpeg");
                    map.Add(".mpg", "video/mpeg");
                    map.Add(".mpv2", "video/mpeg");
                    map.Add(".nmw", "application/nmwb");
                    map.Add(".nws", "message/rfc822");
                    map.Add(".odt", "application/vnd.oasis.opendocument.text");
                    map.Add(".ods", "application/vnd.oasis.opendocument.spreadsheet");
                    map.Add(".p10", "application/pkcs10");
                    map.Add(".p12", "application/x-pkcs12");
                    map.Add(".p7b", "application/x-pkcs7-certificates");
                    map.Add(".p7c", "application/pkcs7-mime");
                    map.Add(".p7m", "application/pkcs7-mime");
                    map.Add(".p7r", "application/x-pkcs7-certreqresp");
                    map.Add(".p7s", "application/pkcs7-signature");
                    map.Add(".pdf", "application/pdf");
                    map.Add(".pfx", "application/x-pkcs12");
                    map.Add(".pko", "application/vnd.ms-pki.pko");
                    map.Add(".png", "image/png");
                    map.Add(".ppt", "application/vnd.ms-powerpoint");
                    map.Add(".pptx", "application/vnd.ms-powerpoint");
                    map.Add(".prc", "text/plain");
                    map.Add(".prf", "application/pics-rules");
                    map.Add(".ps", "application/postscript");
                    map.Add(".psd", "image/vnd.adobe.photoshop");
                    map.Add(".ra", "audio/vnd.rn-realaudio");
                    map.Add(".rar", "application/x-rar-compressed");
                    map.Add(".ram", "audio/x-pn-realaudio");
                    map.Add(".rat", "application/rat-file");
                    map.Add(".rc", "text/plain");
                    map.Add(".rc2", "text/plain");
                    map.Add(".rct", "text/plain");
                    map.Add(".rdlc", "application/xml");
                    map.Add(".resx", "application/xml");
                    map.Add(".rm", "application/vnd.rn-realmedia");
                    map.Add(".rmi", "audio/mid");
                    map.Add(".rms", "application/vnd.rn-realmedia-secure");
                    map.Add(".rmvb", "application/vnd.rn-realmedia-vbr");
                    map.Add(".rp", "image/vnd.rn-realpix");
                    map.Add(".rpm", "audio/x-pn-realaudio-plugin");
                    map.Add(".rt", "text/vnd.rn-realtext");
                    map.Add(".rtf", "application/rtf");
                    map.Add(".rul", "text/plain");
                    map.Add(".rv", "video/vnd.rn-realvideo");
                    map.Add(".sct", "text/scriptlet");
                    map.Add(".settings", "application/xml");
                    map.Add(".sit", "application/x-stuffit");
                    map.Add(".sitemap", "application/xml");
                    map.Add(".skin", "application/xml");
                    map.Add(".sln", "text/plain");
                    map.Add(".smi", "application/smil");
                    map.Add(".smil", "application/smil");
                    map.Add(".snd", "audio/basic");
                    map.Add(".snippet", "application/xml");
                    map.Add(".sol", "text/plain");
                    map.Add(".sor", "text/plain");
                    map.Add(".spc", "application/x-pkcs7-certificates");
                    map.Add(".spl", "application/futuresplash");
                    map.Add(".sql", "text/plain");
                    map.Add(".sst", "application/vnd.ms-pki.certstore");
                    map.Add(".stl", "application/vnd.ms-pki.stl");
                    map.Add(".swf", "application/x-shockwave-flash");
                    map.Add(".tab", "text/plain");
                    map.Add(".tar", "application/x-tar");
                    map.Add(".tdl", "text/xml");
                    map.Add(".tgz", "application/x-compressed");
                    map.Add(".tif", "image/tiff");
                    map.Add(".tiff", "image/tiff");
                    map.Add(".torrent", "application/x-bittorrent");
                    map.Add(".trg", "text/plain");
                    map.Add(".txt", "text/plain");
                    map.Add(".udf", "text/plain");
                    map.Add(".udt", "text/plain");
                    map.Add(".uls", "text/iuls");
                    map.Add(".user", "text/plain");
                    map.Add(".usr", "text/plain");
                    map.Add(".vb", "text/plain");
                    map.Add(".vbdproj", "text/plain");
                    map.Add(".vbproj", "text/plain");
                    map.Add(".vcf", "text/x-vcard");
                    map.Add(".vddproj", "text/plain");
                    map.Add(".vdp", "text/plain");
                    map.Add(".vdproj", "text/plain");
                    map.Add(".viw", "text/plain");
                    map.Add(".vscontent", "application/xml");
                    map.Add(".vsi", "application/ms-vsi");
                    map.Add(".vspolicy", "application/xml");
                    map.Add(".vspolicydef", "application/xml");
                    map.Add(".vspscc", "text/plain");
                    map.Add(".vsscc", "text/plain");
                    map.Add(".vssettings", "text/xml");
                    map.Add(".vssscc", "text/plain");
                    map.Add(".vstemplate", "text/xml");
                    map.Add(".wav", "audio/wav");
                    map.Add(".wax", "audio/x-ms-wax");
                    map.Add(".wm", "video/x-ms-wm");
                    map.Add(".wma", "audio/x-ms-wma");
                    map.Add(".wmd", "application/x-ms-wmd");
                    map.Add(".wmv", "video/x-ms-wmv");
                    map.Add(".wmx", "video/x-ms-wmx");
                    map.Add(".wmz", "application/x-ms-wmz");
                    map.Add(".wpl", "application/vnd.ms-wpl");
                    map.Add(".wsc", "text/scriptlet");
                    map.Add(".wsdl", "application/xml");
                    map.Add(".wvx", "video/x-ms-wvx");
                    map.Add(".xdr", "application/xml");
                    map.Add(".xhash", "application/x-BaiduHashFile");
                    map.Add(".xls", "application/vnd.ms-excel");
                    map.Add(".xlsx", "application/vnd.ms-excel");
                    map.Add(".xml", "text/xml");
                    map.Add(".xmta", "application/xml");
                    map.Add(".xsc", "application/xml");
                    map.Add(".xsd", "application/xml");
                    map.Add(".xsl", "text/xml");
                    map.Add(".xslt", "application/xml");
                    map.Add(".xss", "application/xml");
                    map.Add(".z", "application/x-compress");
                    map.Add(".zip", "application/zip");
                    #endregion

                    mimemap = map;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Impostazione valore in sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="sessionValue"></param>
        private static void SetSessionValue(string sessionKey, object sessionValue)
        {
            try
            {
                System.Web.HttpContext.Current.Session[sessionKey] = sessionValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Reperimento valore da sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        private static object GetSessionValue(string sessionKey)
        {
            try
            {
                return System.Web.HttpContext.Current.Session[sessionKey];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Rimozione chiave di sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        private static void RemoveSessionValue(string sessionKey)
        {
            try
            {
                System.Web.HttpContext.Current.Session.Remove(sessionKey);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        #region file document manager
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public FileDocumento GetFile(Page page, DocsPaWR.FileRequest fileRequest, bool convertPdfInline)
        {
            try
            {
                return this.GetFile(page, fileRequest, convertPdfInline, false);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="fileRequest"></param>
        /// <param name="convertPdfInline"></param>
        /// <returns></returns>
        public FileDocumento GetFile(Page page, DocsPaWR.FileRequest fileRequest, bool convertPdfInline, bool getFileFirmato)
        {
            string msgErr = string.Empty;
            try
            {
                //luluciani modifica 17-01-2014
                response = page.Response;


                if (fileRequest != null)
                {
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    bool isConverted = false;
                    //modifica chace


                    string pagina = "";
                    if (page.Request.UrlReferrer != null)
                        pagina = page.Request.UrlReferrer.ToString();


                    isConverted = false;

                    if (convertPdfInline)
                        fileDoc = docsPaWS.DocumentoGetFileAsPdfFormat(fileRequest, infoUtente, out isConverted);
                    else if (getFileFirmato)
                        fileDoc = docsPaWS.DocumentoGetFileFirmato(fileRequest, infoUtente);
                    else
                        fileDoc = docsPaWS.DocumentoGetFileDoc(fileRequest, infoUtente, out msgErr);

                    if (fileDoc == null)
                        throw new Exception();

                    //fine modifica cache
                }
            }
            //catch (Exception e)
            //{
            //    if (msgErr == "")
            //        if (fileRequest.fileName != null && fileRequest.fileName.Equals(""))
            //            msgErr = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br> " + fileRequest.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";
            //        else
            //            msgErr = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file <br><br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";

            //    Exception custom = new Exception(msgErr);
            //    if (incache)
            //    {
            //        bool cachePieno = false;
            //        if (e.Message.ToLower().Contains("memoria cache piena"))
            //            cachePieno = true;

            //        ErrorManager.redirectCache(page, custom, cachePieno); ;
            //    }
            //    else
            //        ErrorManager.redirect(page, custom);

            //}
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fileDoc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public FileDocumento GetFileAsEML(Page page, DocsPaWR.FileRequest fileRequest)
        {
            FileDocumento fd = null;
            fd = GetFile(page, fileRequest, false);

            string msgErr = string.Empty;
            try
            {
                if (fileRequest != null)
                {
                    InfoUtente infoUtente = UserManager.GetInfoUser();

                    string pagina = page.Request.UrlReferrer.ToString();


                    fileDoc = docsPaWS.DocumentoGetFileDocAsEML(fileRequest, infoUtente, out msgErr);
                    if (fileDoc == null)
                        throw new Exception();

                }
            }
            //catch (Exception e)
            //{
            //    if (msgErr == "")
            //        if (fileRequest.fileName != null && fileRequest.fileName.Equals(""))
            //            msgErr = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br> " + fileRequest.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";
            //        else
            //            msgErr = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file <br><br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";

            //    Exception custom = new Exception(msgErr);
            //    if (incache)
            //    {
            //        bool cachePieno = false;
            //        if (e.Message.ToLower().Contains("memoria cache piena"))
            //            cachePieno = true;
            //        ErrorManager.redirectCache(page, custom, cachePieno); ;
            //    }
            //    else
            //        ErrorManager.redirect(page, custom);
            //}
            //logger.Info("END");
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fileDoc;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static FileRequest getSelectedFile()
        {
            try
            {
                return (FileRequest)FileManager.GetSessionValue("FileManager.selectedFile");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static MassSignature getSelectedMassSignature(string idDocument = null)
        {
            MassSignature retVal = null;

            try
            {
                if (string.IsNullOrEmpty(idDocument))
                    retVal = (MassSignature)FileManager.GetSessionValue("FileManager.massSignature");
                else
                {
                    List<DocsPaWR.MassSignature> listaFileMassivi = (List<DocsPaWR.MassSignature>)FileManager.GetSessionValue("FileManager.massSignature_List");
                    if (listaFileMassivi != null && listaFileMassivi.Count > 0)
                    {
                        retVal = (from l in listaFileMassivi where l.fileRequest.docNumber.Equals(idDocument) select l).FirstOrDefault();
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                retVal = null;
            }

            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void removeSelectedMassSignatureList()
        {
            try
            {
                RemoveSessionValue("FileManager.massSignature_List");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private static bool PermanentDisplaySignature
        {
            get
            {
                try
                {
                    if (HttpContext.Current.Session["permanentDisplaySignature"] != null)
                        return (bool)HttpContext.Current.Session["permanentDisplaySignature"];
                    else return false;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return false;
                }
            }
            set
            {
                try
                {
                    HttpContext.Current.Session["permanentDisplaySignature"] = value;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }

        /// <summary>
        /// Restituisce il FileRequest della versione del documento prinpale/allegato con versionId;
        /// Se il versionId non è presente allora restituisce il FileRequest dell'ultima versione del doc principale
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public static DocsPaWR.FileRequest GetFileRequest(string versionId = null)
        {
            try
            {
                SchedaDocumento doc = DocumentManager.getSelectedRecord();
                DocsPaWR.FileRequest fileRequest = null;
                if (doc != null)
                {
                    if (!string.IsNullOrEmpty(versionId))
                    {
                        //cerco tra le versioni del documento principale
                        fileRequest = fileRequest = doc.documenti.Where(e => e.versionId == versionId).FirstOrDefault();
                        if (fileRequest == null)
                        {
                            // Ricerca la versione negli allegati al documento
                            fileRequest = fileRequest = doc.allegati.Where(e => e.versionId == versionId).FirstOrDefault();
                        }
                        if (fileRequest == null)
                        {
                            foreach(Allegato all in doc.allegati)
                            {
                                Page page = new Page();
                                if (!string.IsNullOrEmpty(all.docNumber))
                                {
                                    SchedaDocumento temp = DocumentManager.getDocumentListVersions(page, all.docNumber, all.docNumber);
                                    fileRequest = temp.documenti.Where(e => e.versionId == versionId).FirstOrDefault();
                                    if (fileRequest != null)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        fileRequest = doc.documenti[0];
                    }
                }
                return fileRequest;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public FileDocumento getVoidFileConSegnatura(DocsPaWR.FileRequest fileRequest, DocsPaWR.SchedaDocumento sch, labelPdf position, Page pagina)
        {
            FileRequest fileReq = null;
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                fileDoc = docsPaWS.DocumentoGetFileConSegnatura(fileReq, sch, infoUtente, position, true);

                if (fileDoc == null)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fileDoc;
        }

        public void setFile(FileRequest fileRequest)
        {
            try
            {
                setSelectedFile(fileRequest);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="fileRequest"></param>
        /// <param name="refresh"></param>
        public static void setSelectedFile(FileRequest fileRequest)
        {
            try
            {
                FileRequest sel = (FileRequest)getSelectedFile();
                if (sel != null && sel == fileRequest) return;
                FileManager.SetSessionValue("FileManager.selectedFile", fileRequest);

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="fileRequest"></param>
        /// <param name="refresh"></param>
        public static void setMassSignature(FileRequest fileRequest, bool cosign, bool pades, string idDocument = null)
        {
            DocsPaWR.MassSignature tempMass = new MassSignature();
            DocsPaWR.MassSignature massSignature = null;
            tempMass.fileRequest = fileRequest;
            tempMass.cosign = cosign;
            tempMass.signPades = pades;
            try
            {
                massSignature = docsPaWS.getSha256(tempMass, UserManager.GetInfoUser());
            }
            catch(Exception e)
            {
                throw new ApplicationException("Errore dutante il calcolo dell'impronta del file");
            }
            try
            {           
                if (string.IsNullOrEmpty(idDocument))
                    FileManager.SetSessionValue("FileManager.massSignature", massSignature);
                else
                {
                    List<DocsPaWR.MassSignature> listaFileMassivi = (FileManager.GetSessionValue("FileManager.massSignature_List") != null ? (List<DocsPaWR.MassSignature>)FileManager.GetSessionValue("FileManager.massSignature_List") : new List<DocsPaWR.MassSignature>());

                    tempMass = (from l in listaFileMassivi where l.fileRequest.docNumber.Equals(idDocument) select l).FirstOrDefault();

                    if (tempMass == null)
                        listaFileMassivi.Add(massSignature);
                    else
                    {
                        listaFileMassivi.Remove(tempMass);
                        listaFileMassivi.Add(massSignature);
                    }

                    FileManager.SetSessionValue("FileManager.massSignature_List", listaFileMassivi);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void removeSelectedFile()
        {
            try
            {
                RemoveSessionValue("FileManager.selectedFile");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static FileManager getInstance(string ID)
        {
            try
            {
                if (ID != null)
                {
                    if (!(fileManager != null && fileManager.Count > 0 && fileManager.ContainsKey(ID)))
                    {
                        //clearUnusedInstances();
                        FileManager fm = new FileManager(ID);
                        fileManager.Add(ID, fm);
                    }
                    return (FileManager)fileManager[ID];
                }
                return new FileManager();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void delInstance()
        {
            try
            {
                fileManager.Remove(this.PageID);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void clearUnusedInstances()
        {
            try
            {
                if (fileManager != null && fileManager.Count > 0)
                {
                    IEnumerator idList = fileManager.Keys.GetEnumerator();
                    while (idList != null && idList.MoveNext())
                    {
                        if (fileManager.ContainsKey(idList))
                        {
                            FileManager.getInstance((string)idList.Current).checkConnecion();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// funzione utilizzata per cancellare le istanze non più utilizzate
        /// </summary>
        public void checkConnecion()
        {
            try
            {
                if (response != null && !response.IsClientConnected)
                {
                    fileDoc = null;
                    this.delInstance();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
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
        public FileDocumento GetFileConSegnatura(Page page, string versionId, string versionNumber, string docNumber, string path, string fileName, labelPdf position)
        {
            try
            {
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
                SchedaDocumento sch = DocumentManager.getDocumentDetails(page, null, docNumber);

                result = this.getFileConSegnatura(page, sch, position, fileRequest);

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public FileDocumento DocumentoGetFileConSegnaturaUsingLC(Page page, DocsPaWR.SchedaDocumento sch, labelPdf position, FileRequest fileRequest)
        {
            response = page.Response;

            if (fileRequest == null && fileDoc != null)
            {
                return fileDoc;
            }

            if (fileRequest != null)
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                fileDoc = docsPaWS.DocumentoGetFileConSegnaturaUsingLC(fileRequest, sch, infoUtente, position, false);
            }

            return fileDoc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="sch"></param>
        /// <param name="position"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public FileDocumentoAnteprima getPdfPreviewFile(Page page, FileRequest fileRequest, int firstPg, int lastPg, DocsPaWR.SchedaDocumento sch, labelPdf label)
        {
            try
            {
                string errore = string.Empty;

                response = page.Response;

                if (fileRequest == null && fileDocA != null)
                {
                    return fileDocA;
                }

                if (fileRequest != null)
                {
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    fileDocA = docsPaWS.DocumentoGetAnteprimaFilePdf(fileRequest, firstPg, lastPg, sch, label, infoUtente, out errore);
                    //fileDocA = docsPaWS.DocumentoGetAnteprimaFilePdf(fileRequest, infoUtente, out errore);
                }

                return fileDocA;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="sch"></param>
        /// <param name="position"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public FileDocumento getFileConSegnatura(Page page, DocsPaWR.SchedaDocumento sch, labelPdf position, FileRequest fileRequest)
        {
            try
            {
                response = page.Response;

                if (fileRequest == null && fileDoc != null)
                {
                    return fileDoc;
                }

                if (fileRequest != null)
                {
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    fileDoc = docsPaWS.DocumentoGetFileConSegnatura(fileRequest, sch, infoUtente, position, false);
                }

                return fileDoc;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Salva la segnatura nel documento
        /// </summary>
        /// <param name="page"></param>
        /// <param name="sch"></param>
        /// <param name="position"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public FileDocumento saveFileConSegnatura(Page page, DocsPaWR.SchedaDocumento sch, labelPdf position, FileRequest fileRequest)
        {
            try
            {
                response = page.Response;

                if (fileRequest == null && fileDoc != null)
                {
                    return fileDoc;
                }

                if (fileRequest != null)
                {
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    fileDoc = docsPaWS.DocumentoGetFileConSegnatura(fileRequest, sch, infoUtente, position, false);
                }

                if (fileDoc != null)
                {
                    //Creo la nuova versione per il doc con impressa la segnatura
                    FileRequest fileReq = uploadFile(page, fileDoc, true);
                    if (fileReq != null)
                    {
                        if (DocumentManager.getSelectedAttachId() != null)
                        {
                            DocumentManager.setSelectedAttachId(fileReq.versionId);
                        }
                        //aggiorno cha_segnature su VERSIONS(lo metto ad uno per indicare che il file acquisito ha impressa la segnatura)
                        docsPaWS.DocumentoVersioneConSegnatura(UserManager.GetInfoUser(), fileReq.versionId);
                    }
                }
                return fileDoc;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// restituisce l'estensone del file, se p7m l'estensione originale del documento contenuto nella busta o nelle buste pkcs7.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="estensione"></param>
        /// <returns></returns>
        public static string getEstensioneIntoP7M(string fullname)
        {
            try
            {
                string retValue = string.Empty;
                if (string.IsNullOrEmpty(fullname))
                    return retValue;
                // Reperimento del nome del file con estensione
                string fileName = new System.IO.FileInfo(fullname).Name;

                string[] items = fileName.Split('.');

                for (int i = (items.Length - 1); i >= 0; i--)
                {
                    if (!items[i].ToUpper().EndsWith("P7M"))
                    {
                        retValue = items[i];
                        break;
                    }
                }

                return retValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// restituisce l'estensone del file, se p7m l'estensione originale del documento contenuto nella busta o nelle buste pkcs7.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="estensione"></param>
        /// <returns></returns>
        public static string getEstensioneIntoSignedFile(string fullname)
        {
            string retValue = string.Empty;
            // Reperimento del nome del file con estensione
            if (!string.IsNullOrEmpty(fullname))
            {
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
            }

            return retValue;

            //string estensione = fullname.Substring(fullname.IndexOf(".") + 1);
            //if (estensione.ToUpper().EndsWith("P7M"))
            //    estensione = estensione.Substring(0, estensione.IndexOf("."));
            //return estensione;							

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string getFileIcon(Page page, string extension)
        {
            try
            {
                if (string.IsNullOrEmpty(extension))
                    return "~/Images/Icons/ico_no_file.png";
                string icona = "icon_gen.png";

                if (extension == "etnoteam")
                    extension = "pdf";

                string ext = string.Empty;
                System.IO.FileInfo fi = null;
                if (extension.IndexOf(".") != -1)
                {
                    fi = new System.IO.FileInfo(extension);
                    ext = fi.Extension;
                    ext = ext.Replace(".", "");
                }
                else
                    ext = extension;

                TypeIcon tipo = TypeIcon.getInstance();
                icona = (string)tipo[ext.ToLower()];
                if (icona == null)
                {
                    string pathAppo = HttpContext.Current.Request.PhysicalApplicationPath;
                    string path = pathAppo + @"images\Icons\icon_" + ext + ".png";
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
                    if (fileInfo.Exists)
                    {
                        tipo.Add(ext.ToLower(), "icon_" + ext.ToLower() + ".png");
                        icona = "icon_" + ext.ToLower() + ".png";
                    }
                    else
                    {
                        icona = "icon_gen.png";
                    }
                }

                return page.ResolveClientUrl("~/Images/Icons/" + icona);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string getFileIconBig(Page page, string extension)
        {
            try
            {
                if (string.IsNullOrEmpty(extension))
                    return page.ResolveClientUrl("~/Images/Icons/big_no_file.png");
                string icona = "big_gen.png";

                string ext = string.Empty;
                System.IO.FileInfo fi = null;
                if (extension.IndexOf(".") != -1)
                {
                    fi = new System.IO.FileInfo(extension);
                    ext = fi.Extension;
                    ext = ext.Replace(".", "");
                }
                else
                    ext = extension;

                TypeIcon tipo = TypeIcon.getInstanceBig();
                icona = (string)tipo[ext.ToLower()];
                if (icona == null)
                {
                    string pathAppo = HttpContext.Current.Request.PhysicalApplicationPath;
                    string path = pathAppo + @"images\Icons\big_" + ext + ".png";
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
                    if (fileInfo.Exists)
                    {
                        tipo.Add(ext.ToLower(), "big_" + ext.ToLower() + ".png");
                        icona = "big_" + ext.ToLower() + ".png";
                    }
                    else
                    {
                        icona = "big_gen.png";
                    }
                }

                return page.ResolveClientUrl("~/Images/Icons/" + icona);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string getFileIconSmall(Page page, string extension)
        {
            try
            {
                if (string.IsNullOrEmpty(extension))
                    return page.ResolveClientUrl("~/Images/Icons/small_no_file.png");
                string icona = "small_gen.png";

                string ext = string.Empty;
                System.IO.FileInfo fi = null;
                if (extension.IndexOf(".") != -1)
                {
                    fi = new System.IO.FileInfo(extension);
                    ext = fi.Extension;
                    ext = ext.Replace(".", "");
                }
                else
                    ext = extension;



                TypeIcon tipo = TypeIcon.getInstance();
                icona = (string)tipo[ext.ToLower()];
                if (icona == null)
                {
                    string pathAppo = HttpContext.Current.Request.PhysicalApplicationPath;
                    string path = pathAppo + @"images\Icons\small_" + ext + ".png";
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
                    if (fileInfo.Exists)
                    {
                        tipo.Add(ext.ToLower(), "icon_" + ext.ToLower() + ".png");
                        icona = "small_" + ext.ToLower() + ".png";
                    }
                    else
                    {
                        icona = "small_gen.png";
                    }
                }

                return page.ResolveClientUrl("~/Images/Icons/" + icona);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void SetSelectedAttachment(string id)
        {
            try
            {
                HttpContext.Current.Session["SelectedAttachment"] = id;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Get Selected Allegato
        /// </summary>
        /// <returns></returns>
        public static DocsPaWR.Allegato GetSelectedAttachment()
        {
            try
            {
                DocsPaWR.Allegato retValue = null;
                string versionId = "";

                if (DocumentManager.getSelectedAttachId() != null)
                    versionId = DocumentManager.getSelectedAttachId();

                if (!string.IsNullOrEmpty(versionId))
                {
                    if (DocumentManager.getSelectedRecord() != null)
                    {
                        Allegato attachment = (from attch in DocumentManager.getSelectedRecord().allegati where attch.versionId.Equals(versionId) select attch).FirstOrDefault();
                        return attachment;
                    }
                }
                return retValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static FileRequest getSelectedFile(Page page)
        {
            try
            {
                DocsPaWR.FileRequest fileReq = null;

                DocsPaWR.Allegato attach = FileManager.GetSelectedAttachment();
                if (attach != null)
                {
                    string selectedVersion = "";
                    if (HttpContext.Current.Session["SelectedVersion"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["SelectedVersion"].ToString())) selectedVersion = HttpContext.Current.Session["SelectedVersion"].ToString();
                    SchedaDocumento doc = DocumentManager.getDocumentDetails(page, attach.docNumber, attach.docNumber);
                    if (doc != null)
                    {
                        DocsPaWR.Documento[] ListaDocVersioni = DocumentManager.getDocumentDetails(page, attach.docNumber, attach.docNumber).documenti;
                        foreach (DocsPaWR.Documento item in ListaDocVersioni)
                        {
                            if (item.version == selectedVersion)
                            {
                                fileReq = (DocsPaWR.FileRequest)item;
                                break;
                            }
                        }
                    }
                }
                return fileReq;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void setSelectedReport(FileDocumento fileRep)
        {
            try
            {
                FileManager.SetSessionValue("FileManager.selectedReport", fileRep);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static FileDocumento getSelectedReport(Page page)
        {
            try
            {
                return (FileDocumento)page.Session["FileManager.selectedReport"];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public FileDocumento getReport(Page page)
        {
            try
            {
                response = page.Response;
                return getSelectedReport(page);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione per l'updload del file di un documento.
        /// </summary>
        /// <param name="page">La pagina richiedente</param>
        /// <param name="file"></param>
        /// <param name="cartaceo"></param>
        /// <param name="conversionePdfServer">Valore che indica se è richiesta la conversione PDF lato server</param>
        /// <param name="conversionePdfServerSincrona">Valore che indica se è richiesta la conversione PDF sincrona</param>
        public static string uploadPersonalFile(Page page, FileDocumento fileDoc, string fileName, string fileDescription)
        {
            // Variabile di appoggio utilizzata per onctenere il documento convertito in PDF
            // a seguito di una richiesta di converisone sincrona lato server
            FileDocumento convSyncResult = null;

            InfoUtente infoUtente = UserManager.GetInfoUser();

            string errorMessage = string.Empty;
            try
            {
                FileRequest fileReq = new FileRequest();
                // Il file è stato associato all'ultima versione del documento principale/allegato
                fileReq = (DocumentManager.getSelectedAttachId() != null) ?
                        FileManager.GetFileRequest(DocumentManager.getSelectedAttachId()) :
                            FileManager.GetFileRequest();

                //if (!docsPaWS.PutFileFromUploadManager(ref fileReq, fileDoc, fileName, fileDescription, infoUtente))//, out errorMessage))
                if (!docsPaWS.PutFileFromUploadManagerLight(ref fileReq, fileDoc, fileDescription, infoUtente))
                {
                    if (errorMessage != string.Empty)
                    {
                        aggiornaFileRequest(page, fileReq);
                        setSelectedFile(null);
                        return errorMessage;
                    }
                    else
                        return "ErrorAcquiredDocument";
                }
                aggiornaFileRequest(page, fileReq);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return string.Empty;
        }

        /// <summary>
        /// Funzione di cancellazione file personali nel repository personale.
        /// </summary>
        /// <param name="page">La pagina richiedente</param>
        /// <param name="fileDoc">File selezionato</param>
        /// <param name="fileName">Nome file su system</param>
        /// <param name="fileDescription">Identificativo univoco</param>
        public static string DeletePersonalFile(Page page, FileDocumento fileDoc, string fileName, string fileDescription)
        {
            // Variabile di appoggio utilizzata per onctenere il documento convertito in PDF
            // a seguito di una richiesta di converisone sincrona lato server
            FileDocumento convSyncResult = null;

            InfoUtente infoUtente = UserManager.GetInfoUser();

            string errorMessage = string.Empty;
            try
            {
                if (!docsPaWS.DeletePersonalFile(fileDoc, fileDescription, infoUtente))
                {
                    if (errorMessage != string.Empty)
                    {
                        return errorMessage;
                    }
                    else
                        return "ErrorDeletedDocument";
                }
                setSelectedFile(null);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                setSelectedFile(null);
                return null;
            }
            return string.Empty;
        }

        /// <summary>
        /// Funzione per l'updload del file di un documento.
        /// </summary>
        /// <param name="page">La pagina richiedente</param>
        /// <param name="file"></param>
        /// <param name="cartaceo"></param>
        /// <param name="conversionePdfServer">Valore che indica se è richiesta la conversione PDF lato server</param>
        /// <param name="conversionePdfServerSincrona">Valore che indica se è richiesta la conversione PDF sincrona</param>
        public static string uploadFile(Page page, FileDocumento fileDoc, bool cartaceo, bool conversionePdfServer, bool conversionePdfServerSincrona)
        {
            // Variabile di appoggio utilizzata per onctenere il documento convertito in PDF
            // a seguito di una richiesta di converisone sincrona lato server
            FileDocumento convSyncResult = null;

            string errorMessage = string.Empty;
            try
            {
                FileRequest fileReq = new FileRequest();
                // Il file è stato associato all'ultima versione del documento principale/allegato
                fileReq = (DocumentManager.getSelectedAttachId() != null) ?
                        FileManager.GetFileRequest(DocumentManager.getSelectedAttachId()) :
                            FileManager.GetFileRequest();
                if (fileReq != null)
                {
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    fileReq.cartaceo = cartaceo;
                    fileReq.fileName = System.IO.Path.GetFileName(fileDoc.fullName);
                    fileReq.fileSize = fileDoc.length.ToString();
                    fileReq.firmato = "";
                    bool isPdf = System.IO.Path.GetExtension(fileDoc.fullName).ToLower().EndsWith("pdf");
                    // Se è stata richiesta la conversione PDF lato server sincrona...
                    if (conversionePdfServer && conversionePdfServerSincrona && !isPdf)
                        // ...la prima versione del documento è direttamente il file convertito in PDF
                        // Richedo quindi la conversione in pdf del content dell'oggetto FileDocumento
                        convSyncResult = docsPaWS.GeneratePDFInSyncMod(fileDoc);

                    // Se la conversione PDF ha avuto successo...
                    if (convSyncResult != null)
                    {
                        // Sostituisco il fileDoc con convSyncResult
                        fileDoc = convSyncResult;
                        fileReq.fileName = fileDoc.name;
                    }

                    //Chiamo il metodo che verifica se è un form pdf da processare
                    processFormPdf(page, fileDoc, fileReq);

                    if (!docsPaWS.DocumentoPutFileNoException(ref fileReq, fileDoc, infoUtente, out errorMessage))
                    {
                        if (errorMessage != string.Empty)
                        {
                            aggiornaFileRequest(page, fileReq);
                            setSelectedFile(null);
                            return errorMessage;
                        }
                        else
                            return "ErrorAcquiredDocument";
                    }
                    aggiornaFileRequest(page, fileReq);

                    //Se abilitata la conversione lato server (asincrona) chiamo il webmethod che mette in coda il file da convertire
                    if (conversionePdfServer && !conversionePdfServerSincrona && !isPdf)
                    {
                        bool isAllegato = (fileReq.GetType() == typeof(DocsPaWR.Allegato));

                        DocsPaWR.SchedaDocumento schedaDocumentoCorrente = null;

                        if (isAllegato)
                        {
                            schedaDocumentoCorrente = CheckInOut.CheckInOutServices.CurrentSchedaDocumento;
                            /* APPLET_G
                              if (schedaDocumentoCorrente == null)
                                schedaDocumentoCorrente = CheckInOutApplet.CheckInOutServices.CurrentSchedaDocumento;
                             */
                        }
                        else
                            schedaDocumentoCorrente = DocumentManager.getSelectedRecord();

                        EnqueueServerPdfConversion(
                                                    UserManager.GetInfoUser(),
                                                    fileDoc.content,
                                                    fileDoc.name,
                                                    schedaDocumentoCorrente);
                    }

                    // Se è stata richiesta la conversione lato server ma questa non è andata
                    // a buon fine...
                    if (conversionePdfServer && conversionePdfServerSincrona && convSyncResult == null && !isPdf)
                    {
                        errorMessage = "ErrorConversionePdf";
                        return errorMessage;
                    }
                        //throw new Exception("Attenzione,<BR>L'acquisizione del documento è avvenuta con successo ma la procedura di conversione in pdf non è andata a buon fine.");
                    // ...sollevo un'eccezione in modo da avvisare l'utente che il documento è stato
                    // acquisito ma non si è riusciti a convertire in pdf in modalità sincrona
                    //throw new NttDataWAException(errorMessage3, "Class: FileManager\nMethod: uploadFile");
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return string.Empty;
        }


        public void uploadFile(Page page, bool addVersion, FileDocumento fileDoc)
        {
            try
            {
                uploadFile(page, fileDoc, addVersion);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static FileRequest uploadFile(Page page, FileDocumento fileDoc, bool addVersion)
        {
            FileRequest fileReq = null;

            try
            {
                //FileRequest fileReq = getSelectedFile();
                fileReq = (DocumentManager.getSelectedAttachId() != null) ?
                           FileManager.GetFileRequest(DocumentManager.getSelectedAttachId()) :
                               FileManager.GetFileRequest();

                fileReq.cartaceo = fileDoc.cartaceo;

                bool daInviare = true;
                if (addVersion)
                {
                    if (fileReq.fileName.Substring(fileReq.fileName.LastIndexOf(".") + 1).ToUpper().Equals("P7M"))
                        daInviare = false;
                    if (fileReq.GetType() == typeof(Allegato))
                    {
                        Allegato attach = new Allegato();
                        if (PermanentDisplaySignature)
                        {
                            attach.descrizione = "Impressa segnatura";
                            HttpContext.Current.Session.Remove("permanentDisplaySignature");
                        }
                        else
                            attach.descrizione = "File convertito";
                        attach.docNumber = fileReq.docNumber;
                        fileReq = DocumentManager.AddVersion(attach, daInviare);
                    }

                    else
                    {
                        Documento documento = new Documento();
                        if (PermanentDisplaySignature)
                        {
                            documento.descrizione = "Impressa segnatura";
                            HttpContext.Current.Session.Remove("permanentDisplaySignature");
                        }
                        else
                            documento.descrizione = "File convertito";
                        documento.docNumber = fileReq.docNumber;
                        fileReq = DocumentManager.AddVersion(documento, daInviare);
                    }

                }

                if (fileReq != null)
                {
                    string fileName = fileDoc.fullName;
                    chiudi = false;
                    InfoUtente infoUtente = UserManager.GetInfoUser();

                    string kbTotal = Convert.ToString(Math.Round((double)fileDoc.length / 1024, 2));
                    //bytesDone = 0;		
                    //int bytesToRead = 1024;

                    //startTime = System.DateTime.Now;

                    chiudi = true;

                    //Chiamo il metodo che verifica se è un form pdf da processare
                    processFormPdf(page, fileDoc, fileReq);

                    string nomeFile = fileReq.fileName;
                    fileReq = docsPaWS.DocumentoPutFile(fileReq, fileDoc, infoUtente);

                    if (fileReq == null)
                    {

                        throw new Exception("Attenzione,<BR>l'acquisizione file non effettuata.");
                    }
                    aggiornaFileRequest(page, fileReq, true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fileReq;
        }

        public static void uploadFile(Page page, HttpPostedFile file, bool cartaceo)
        {

            string errorMessage = string.Empty;
            try
            {
                FileRequest fileReq = getSelectedFile(page);
                if (fileReq != null)
                {
                    InfoUtente infoUtente = UserManager.GetInfoUser();
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
            //catch (System.Web.Services.Protocols.SoapException es)
            //{
            //    //ErrorManager.redirect(page, es);
            //    ErrorManager.redirect(page, es, "acquisizione");
            //}
            //catch (Exception e)
            //{
            //    ErrorManager.redirect(page, e, "acquisizione");
            //    //ErrorManager.redirect(page, e);
            //}
            //logger.Info("END");
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void uploadFileFromSchedaDocumento(Page page, HttpPostedFile file, SchedaDocumento schedaDocumento)
        {
            try
            {
                string errorMessage = string.Empty;
                if (file != null)
                {
                    FileRequest fileReq = null;
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    FileDocumento fileDoc = new FileDocumento();
                    bool cartaceo = false;
                    fileDoc.name = System.IO.Path.GetFileName(file.FileName);
                    fileDoc.fullName = fileDoc.name;
                    fileDoc.contentType = file.ContentType;
                    fileDoc.length = file.ContentLength;
                    fileDoc.content = new Byte[fileDoc.length];
                    fileDoc.cartaceo = cartaceo;
                    file.InputStream.Read(fileDoc.content, 0, fileDoc.length);
                    fileReq = schedaDocumento.documenti[0];
                    fileReq.cartaceo = cartaceo;
                    fileReq.fileName = System.IO.Path.GetFileName(file.FileName);
                    fileReq.fileSize = fileDoc.length.ToString();

                    if (!docsPaWS.DocumentoPutFileNoException(ref fileReq, fileDoc, infoUtente, out errorMessage))
                    {
                        string genericError = "Non è stato possibile acquisire il documento. <BR><BR>Ripetere l'operazione di acquisizione. Errore:";
                        if (errorMessage != string.Empty)
                        {
                            genericError += errorMessage;
                        }

                        throw new Exception(genericError);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            //logger.Info("END");
        }

        private static void processFormPdf(Page page, FileDocumento fileDocumento, FileRequest fileRequest)
        {
            try
            {
                SchedaDocumento schedaDocumento = DocumentManager.getSelectedRecord();

                if (schedaDocumento.template == null && System.IO.Path.GetExtension(fileDocumento.fullName).ToUpper() == ".PDF" && docsPaWS.FunzioneEsistente("PROCESS_FORM_PDF"))
                {
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    DocsPaWR.ProcessFormInput processFormInput = new ProcessFormInput();
                    processFormInput.schedaDocumentoInput = schedaDocumento;
                    processFormInput.fileDocumentoInput = fileDocumento;

                    docsPaWS.Timeout = System.Threading.Timeout.Infinite; ;
                    DocsPaWR.ProcessFormOutput processFormOutput = docsPaWS.ProcessFormPdf(infoUtente, processFormInput);

                    if (processFormOutput != null && processFormOutput.schedaDocumentoOutput != null)
                    {
                        DocumentManager.setSelectedRecord(processFormOutput.schedaDocumentoOutput);
                        if (processFormOutput.schedaDocumentoOutput.documenti != null && processFormOutput.schedaDocumentoOutput.documenti.Length >= 1)
                            fileRequest.firmato = ((DocsPaWR.FileRequest)processFormOutput.schedaDocumentoOutput.documenti[0]).firmato;
                        //SiteNavigation.CallContextStack.CurrentContext.ContextState["ProcessPdfLiveCycle"] = processFormOutput;
                    }
                }

                if (schedaDocumento.template == null && (System.IO.Path.GetExtension(fileDocumento.fullName).ToUpper() == ".TIF" || System.IO.Path.GetExtension(fileDocumento.fullName).ToUpper() == ".TIFF") && docsPaWS.FunzioneEsistente("PROCESS_FORM_PDF"))
                {
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    DocsPaWR.ProcessFormInput processFormInput = new ProcessFormInput();
                    processFormInput.schedaDocumentoInput = schedaDocumento;
                    processFormInput.fileDocumentoInput = fileDocumento;

                    docsPaWS.Timeout = System.Threading.Timeout.Infinite; ;
                    DocsPaWR.ProcessFormOutput processFormOutput = docsPaWS.ProcessBarcodeFormPdf(infoUtente, processFormInput);

                    if (processFormOutput != null && processFormOutput.schedaDocumentoOutput != null)
                    {
                        DocumentManager.setSelectedRecord(processFormOutput.schedaDocumentoOutput);
                        if (processFormOutput.schedaDocumentoOutput.documenti != null && processFormOutput.schedaDocumentoOutput.documenti.Length >= 1)
                            fileRequest.firmato = ((DocsPaWR.FileRequest)processFormOutput.schedaDocumentoOutput.documenti[0]).firmato;
                        //SiteNavigation.CallContextStack.CurrentContext.ContextState["ProcessPdfLiveCycle"] = processFormOutput;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void aggiornaFileRequest(Page page, FileRequest newFileReq, bool addVersion = false)
        {
            try
            {
                SchedaDocumento tabDocument = DocumentManager.getSelectedRecord();
                //Sono nel caso di acquisisci prima di salvare
                if (string.IsNullOrEmpty(newFileReq.docNumber))
                {
                    if (newFileReq.GetType() == typeof(Documento) && tabDocument.documenti != null && tabDocument.documenti.Count() > 0)
                    {
                        tabDocument.documenti[0] = newFileReq as Documento;
                        DocumentManager.setSelectedRecord(tabDocument);
                    }
                    return;
                }
                if (newFileReq.docNumber.Equals(tabDocument.docNumber))
                {
                    FileManager.setSelectedFile(newFileReq);
                    if (addVersion)
                    {
                        DocsPaWR.Documento documento = newFileReq as Documento;
                        Documento[] docs = new Documento[tabDocument.documenti.Length + 1];
                        tabDocument.documenti.CopyTo(docs, 1);
                        docs[0] = documento;
                        tabDocument.documenti = docs;
                        DocumentManager.setSelectedRecord(tabDocument);
                    }
                    else
                    {
                        DocsPaWR.Documento documento = newFileReq as Documento;
                        tabDocument.documenti[0] = documento;
                        DocumentManager.setSelectedRecord(tabDocument);
                        return;
                    }
                }

                else //Il file è stato aggiunto ad un allegato
                {
                    for (int i = 0; i < tabDocument.allegati.Length; i++)
                    {
                        if (tabDocument.allegati[i].docNumber != null && tabDocument.allegati[i].docNumber.Equals(newFileReq.docNumber))
                        {

                            Allegato a = new Allegato();
                            a.applicazione = newFileReq.applicazione;
                            a.daAggiornareFirmatari = newFileReq.daAggiornareFirmatari;
                            a.dataInserimento = newFileReq.dataInserimento;
                            a.descrizione = tabDocument.allegati[i].descrizione;
                            a.docNumber = newFileReq.docNumber;
                            a.docServerLoc = newFileReq.docServerLoc;
                            a.fileName = newFileReq.fileName;
                            a.fileSize = newFileReq.fileSize;
                            a.firmatari = newFileReq.firmatari;
                            a.firmato = newFileReq.firmato;
                            a.tipoFirma = newFileReq.tipoFirma;
                            a.idPeople = newFileReq.idPeople;
                            a.path = newFileReq.path;
                            a.subVersion = newFileReq.version;
                            a.version = newFileReq.version;
                            a.versionId = newFileReq.versionId;
                            a.versionLabel = tabDocument.allegati[i].versionLabel;
                            a.cartaceo = newFileReq.cartaceo;
                            a.repositoryContext = newFileReq.repositoryContext;
                            a.TypeAttachment = 1;
                            a.numeroPagine = (newFileReq.GetType() == typeof(DocsPaWR.Allegato)) ? (newFileReq as Allegato).numeroPagine : 0;
                            a.inLibroFirma = newFileReq.inLibroFirma;
                            // modifica necessaria per FILENET (A.B.)
                            if ((newFileReq.fNversionId != null) && (newFileReq.fNversionId != ""))
                                a.fNversionId = newFileReq.fNversionId;
                            tabDocument.allegati[i] = a;
                            DocumentManager.setSelectedRecord(tabDocument);
                            break;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);

            }
        }

        private static bool AreEqualsAttacments(Allegato att1, Allegato att2)
        {
            try
            {
                bool retVal = false;
                if (att1.docNumber == null && att2.docNumber == null)
                    retVal = att1.repositoryContext != null && att2.repositoryContext != null && att1.repositoryContext.Token.Equals(att2.repositoryContext.Token);
                else
                    retVal = att1.docNumber == att2.docNumber;

                return retVal;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public FileDocumento getInfoFile(Page page, FileRequest fileRequest)
        {
            FileDocumento fileDocument = null;

            try
            {
                if (fileRequest != null)
                {
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    fileDocument = docsPaWS.DocumentoGetInfoFile(fileRequest, infoUtente);
                }
            }
            catch (Exception e)
            {
                //string msg = string.Empty;
                //if (fileRequest.fileName != null && fileRequest.fileName.Equals(""))
                //    msg = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br> " + fileRequest.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";
                //else
                //    msg = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file <br><br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";

                //Exception custom = new Exception(msg);
                //ErrorManager.redirect(page, custom);
                UIManager.AdministrationManager.DiagnosticError(e);
            }

            return fileDocument;
        }

        public FileDocumento getInfoFile(Page page)
        {
            FileRequest fileReq = null;

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
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    fileDoc = docsPaWS.DocumentoGetInfoFile(fileReq, infoUtente);

                    if (fileDoc == null)
                    {
                        throw new Exception();
                    }
                }
                //this.delInstance();
            }
            //catch (Exception e)
            //{
            //    string msg = string.Empty;
            //    if (fileReq.fileName != null && fileReq.fileName.Equals(""))
            //        msg = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br> " + fileReq.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";
            //    else
            //        msg = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file <br><br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";

            //    Exception custom = new Exception(msg);
            //    ErrorManager.redirect(page, custom);

            //}
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fileDoc;
        }

        public FileDocumento getFile(Page page)
        {
            string msgErr = string.Empty;
            FileRequest fileReq = null;
            try
            {
                response = page.Response;
                //fileReq = getSelectedFile(page);

                if (UIManager.DocumentManager.getSelectedAttachId() != null)
                {
                    fileReq = FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId());
                }
                else
                {
                    fileReq = FileManager.GetFileRequest();
                }
                if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
                {
                    fileReq = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
                }

                if (fileReq == null && fileDoc != null)
                {
                    return fileDoc;
                }
                if (fileReq != null)
                {
                    InfoUtente infoUtente = UserManager.GetInfoUser();

                    //fine modifica
                    fileDoc = docsPaWS.DocumentoGetFileDoc(fileReq, infoUtente, out msgErr);

                    if (fileDoc == null)
                    {
                        throw new Exception();
                    }
                    //modifica

                    //fine modifica
                }
                //this.delInstance();
            }
            //catch (Exception e)
            //{

            //    if (msgErr == "")
            //        if (fileReq.fileName != null && fileReq.fileName.Equals(""))
            //            msgErr = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br> " + fileReq.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";
            //        else
            //            msgErr = "Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file <br><br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.";

            //    Exception custom = new Exception(msgErr);
            //    ErrorManager.redirect(page, custom);
            //}
            //logger.Info("END");
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fileDoc;
        }

        /// <summary>
        /// Metodo che permette la messa in coda di un file per la conversione pdf lato server
        /// </summary>
        /// <param name="page"></param>
        /// <param name="content"></param>
        /// <param name="filename"></param>
        public static void EnqueueServerPdfConversion(InfoUtente infoUtente, byte[] content, string filename, SchedaDocumento schDoc)
        {
            try
            {
                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.ASPOSE_PDF_CONVERSION.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.ASPOSE_PDF_CONVERSION.ToString()).Equals("1"))
                {
                    docsPaWS.AsposeServerPdfConversion(content, GetFileRequest(DocumentManager.getSelectedAttachId()), infoUtente);
                }
                else
                {
                    ObjServerPdfConversion objServerPdfConversion = new ObjServerPdfConversion();
                    objServerPdfConversion.content = content;
                    objServerPdfConversion.fileName = filename;
                    objServerPdfConversion.idProfile = schDoc.systemId;
                    objServerPdfConversion.docNumber = schDoc.docNumber;

                    if (!string.IsNullOrEmpty(objServerPdfConversion.idProfile) && !string.IsNullOrEmpty(objServerPdfConversion.docNumber))
                        docsPaWS.EnqueueServerPdfConversion(infoUtente, objServerPdfConversion);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
        /*
                /// <summary>
                /// NELLA CONFIGURAZIONE: set_DATA_VISTA_GRD=2, Toglie la trasmissione dalla TDL, ma non setta la DATA VISTA.
                /// </summary>
                /// <param name="infoutente"></param>
                /// <param name="idProfile"></param>
                /// <param name="docorFasc"></param>
                /// <returns></returns>
                public static bool setdatavistaSP_TV(DocsPaWR.InfoUtente infoutente, string idProfile, string docorFasc, String idRegistro, string idTrasm)
                {
                    try
                    {
                        return docsPaWS.SetDataVistaSP_TV(infoutente, idProfile, docorFasc, idRegistro, idTrasm);
                    }
                    catch (System.Exception ex)
                    {
                        UIManager.AdministrationManager.DiagnosticError(ex);
                        return false;
                    }
                }*/

        public static void setSelectedFileReport(Page page, DocsPaWR.FileDocumento fileDoc, string initialPath)
        {
            try
            {
                page.Session["FileManager.selectedReport"] = fileDoc;
                page.Session["EXPORT_FILE_SESSION"] = fileDoc;
                ScriptManager.RegisterStartupScript(page, page.GetType(), "Prints", "ajaxModalPopupPrints();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
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
        public FileDocumento GetFile(Page page,
                        string versionId,
                        string versionNumber,
                        string docNumber,
                        string path,
                        string fileName,
                        bool showAsPdfFormat)
        {
            //logger.Info("BEGIN");
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

            FileDocumento res = this.GetFile(page, showAsPdfFormat, fileRequest);
            //logger.Info("END");
            return res;
        }

        /// <summary>
        /// Funzione per il reperimento del file
        /// </summary>
        /// <param name="page">La pagina richiedente</param>
        /// <param name="convertPdfInline">True se il documento deve essre convertito in linea</param>
        /// <returns>L'oggetto con le informazioni ed il contenuto del file</returns>
        public FileDocumento GetFile(Page page, bool convertPdfInline, FileRequest fileRequest)
        {
            //logger.Info("BEGIN");
            FileRequest fr = null;
            string msgErr = string.Empty;

            try
            {
                response = page.Response;

                if (fileRequest == null)
                    fr = getSelectedFile(page);
                else
                    fr = fileRequest;

                if (fr != null)
                {
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    bool isConverted = false;

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
                ErrorManager.redirect(page, custom);

            }
            //logger.Info("END");
            return fileDoc;
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

        /// <summary>
        /// getSelectedFileReport: ritorna il file report
        /// </summary>
        /// <param name="page">Pagina da cui viene richiamato il metodo</param>
        /// <returns>file report</returns>
        public static object getSelectedFileReport(Page page)
        {
            return page.Session["FileManager.selectedReport"];
        }

        public static void RemoveFileReport(Page page)
        {
            page.Session.Remove("FileManager.selectedReport");
        }

        #endregion

        #region Gestione formati file supportati

        /// <summary>
        /// Reperimento dei tipi di file supportati da un'amministrazione
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static SupportedFileType[] GetSupportedFileTypes(int idAmministrazione)
        {
            try
            {
                return docsPaWS.GetSupportedFileTypes(idAmministrazione);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;

            }
        }

        public static bool IsEnabledSupportedFileTypes()
        {
            try
            {
                return docsPaWS.IsEnabledSupportedFileTypes();
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static int TolatFileSizeDocument(string idDocumento)
        {
            try
            {
                return docsPaWS.TotalFileSizeDocument(idDocumento);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return 0;
            }
        }
        #endregion
    }
}
