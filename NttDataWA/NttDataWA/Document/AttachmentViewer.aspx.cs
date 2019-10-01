using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using System.Globalization;


namespace NttDataWA.Document
{
    public partial class AttachmentViewer : System.Web.UI.Page
    {
        #region Costanti
        private const string EML = "eml";

        private const int CODE_ATTACH_USER = 1;
        private const int CODE_ATTACH_PEC = 2;
        private const int CODE_ATTACH_IS = 3;
        private const int CODE_ATTACH_EXT = 4;
        private const string PDF = "pdf";
        private const string TSD = "tsd";
        private const string M7M = "m7m";
        #endregion

        #region Property

        /// <summary>
        /// document file to be show
        /// </summary>
        private FileDocumento FileDoc
        {
            get
            {
                if (HttpContext.Current.Session["fileDoc"] != null)
                    return HttpContext.Current.Session["fileDoc"] as FileDocumento;
                else return null;
            }
        }

                /// <summary>
        ///  Contiene il file doc per la stampa registri
        /// </summary>
        private FileDocumento FileDocPrintRegister
        {
            get
            {
                return HttpContext.Current.Session["fileDocPrintRegister"] as FileDocumento;
            }
        }

        #region Posizione segnatura

        /// <summary>
        /// 
        /// </summary>
        private string PositionSignature
        {
            get
            {
                if (HttpContext.Current.Session["position"] != null)
                    return HttpContext.Current.Session["position"].ToString();
                else return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool TypeLabel
        {
            get
            {
                if (HttpContext.Current.Session["typeLabel"] != null)
                    return (bool)HttpContext.Current.Session["typeLabel"];
                else return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string RotationSignature
        {
            get
            {
                if (HttpContext.Current.Session["rotation"] != null)
                    return HttpContext.Current.Session["rotation"].ToString();
                else return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string CharacterSignature
        {
            get
            {
                if (HttpContext.Current.Session["character"] != null)
                    return HttpContext.Current.Session["character"].ToString();
                else return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string ColorSignature
        {
            get
            {
                if (HttpContext.Current.Session["color"] != null)
                    return HttpContext.Current.Session["color"].ToString();
                else return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string OrientationSignature
        {
            get
            {
                if (HttpContext.Current.Session["orientation"] != null)
                    return HttpContext.Current.Session["orientation"].ToString();
                else return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool NoTimbro
        {
            get
            {
                if (HttpContext.Current.Session["NoTimbro"] != null)
                    return (bool)HttpContext.Current.Session["NoTimbro"];
                else return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool PrintOnFirstPage
        {
            get
            {
                if (HttpContext.Current.Session["printOnFirstPage"] != null)
                    return (bool)HttpContext.Current.Session["printOnFirstPage"];
                else return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool PrintOnLastPage
        {
            get
            {
                if (HttpContext.Current.Session["printOnLastPage"] != null)
                    return (bool)HttpContext.Current.Session["printOnLastPage"];
                else return false;
            }
        }

        private TypePrintFormatSign FormatSignature
        {
            get
            {
                if (HttpContext.Current.Session["FormatSignature"] != null)
                    return (TypePrintFormatSign)HttpContext.Current.Session["FormatSignature"];
                else return TypePrintFormatSign.Sign_Extended;
            }
            set
            {
                HttpContext.Current.Session["FormatSignature"] = value;
            }
        }

        private bool ChangeSignature
        {
            get
            {
                if (HttpContext.Current.Session["ChangeSignature"] != null)
                    return (bool)HttpContext.Current.Session["ChangeSignature"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["ChangeSignature"] = value;
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Delete Property defined in ViewDocument 
        /// </summary>
        private void DeleteProperty()
        {
            HttpContext.Current.Session.Remove("fileDoc");
            HttpContext.Current.Session.Remove("showSignature");
            HttpContext.Current.Session.Remove("position");
            HttpContext.Current.Session.Remove("typeLabel");
            HttpContext.Current.Session.Remove("rotation");
            HttpContext.Current.Session.Remove("color");
            HttpContext.Current.Session.Remove("character");
            HttpContext.Current.Session.Remove("orientation");
        }

        string removeAllCryptExtensions(string fileName)
        {
            string retval =string.Empty;
            string[]  name= fileName.Split ('.');
            foreach (string s in name)
            {
                if (s.ToLower ().Contains ("p7m"))
                    continue;

                if (s.ToLower ().Contains ("tsd"))
                    continue;

                if (string.IsNullOrEmpty (retval))
                    retval = s;
                else
                    retval += "."+s;


            }
            return retval;
        }

        private void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
                
                Response.Expires = -1;
                if (FileDoc != null || FileDocPrintRegister != null)
                {
                    if (Request.QueryString["download"] != null && Request.QueryString["download"] == "1")
                    {
                        Response.ContentType = FileDoc.contentType;
                        Response.AddHeader("content-disposition", "attachment;filename=" + FileDoc.name);
                        Response.BinaryWrite(FileDoc.content);
                        Response.End();
                    }

                    if (Request.QueryString["versionDownload"] != null)
                    {
                        string versionId = Request.QueryString["versionDownload"].ToString();
                        FileRequest file = FileManager.GetFileRequest(versionId);
                        FileDocumento fileDocumento = SaveFileDocument(file);

                        //Anomalia CHROME: per nome del file contenenti la virgola va in eccezione
                        string nomeOriginale = (fileDocumento.nomeOriginale != null) ? fileDocumento.nomeOriginale.Replace(",", "") : fileDocumento.nomeOriginale;
                        fileDocumento.nomeOriginale = nomeOriginale;

                        Response.ContentType = fileDocumento.contentType;

                        string nomeFile = null;
                        nomeFile = fileDocumento.nomeOriginale;
                        // Se manca OFN,  ricavo il nome dal version number piu l'ext
                        // non uso il filedocumento.name in quanto non so se su dtcm ho questa info.
                        if (String.IsNullOrEmpty(nomeFile))
                        {
                            /*
                            string ext = System.IO.Path.GetExtension(fileDocumento.name).ToLowerInvariant(); ;
                            nomeFile = String.Format ("{0}{1}",versionId,ext);
                             */
                            nomeFile = fileDocumento.name;
                        }
                        string nomeSenzap7mOtsd = removeAllCryptExtensions(nomeFile);


                        //questo serve per l'inline converter se il content type è application/pdf ma l'ext non lo è allora ci sta un
                        //problema e la cosa deve essere corretta..
                        if ((fileDocumento.contentType.Equals("application/pdf")) &&
                            (System.IO.Path.GetExtension(nomeSenzap7mOtsd).ToLowerInvariant() != ".pdf"))
                            nomeSenzap7mOtsd += ".pdf";

                        Response.AddHeader("content-disposition", "attachment;filename=" + nomeSenzap7mOtsd);
                        Response.BinaryWrite(fileDocumento.content);
                        Response.End();
                    }

                    if (Request.QueryString["printRegister"] != null && Request.QueryString["printRegister"] == "t")
                    {
                        Response.ContentType = FileDocPrintRegister.contentType;
                        Response.AddHeader("content-disposition", "inline;filename=" + FileDocPrintRegister.name);
                        Response.AddHeader("content-length", FileDocPrintRegister.content.Length.ToString());
                        Response.BinaryWrite(FileDocPrintRegister.content);
                        Response.End();
                        HttpContext.Current.Session.Remove("fileDocPrintRegister");
                    }
                    else
                    {
                        /*
                        // Gabriele Melini 29-05-2014
                        // Visualizzazione FatturaPA
                        if (FileDoc.nomeOriginale != null && (FileDoc.nomeOriginale.ToUpper().EndsWith("XML")))
                        {
                            SchedaDocumento schedaDoc = DocumentManager.getSelectedRecord();
                            if (schedaDoc.tipologiaAtto != null && (schedaDoc.tipologiaAtto.descrizione.ToUpper().Equals("FATTURA ELETTRONICA")))
                            {
                                string urlXSL = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.FATTURAPA_XSL_URL.ToString()];
                                if (!string.IsNullOrEmpty(urlXSL))
                                {
                                    string xmlString = System.Text.Encoding.UTF8.GetString(FileDoc.content);
                                    string decl = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                                    string pi = string.Format("<?xml-stylesheet type=\"text/xsl\" href=\"{0}\"?>", urlXSL);
                                    xmlString = xmlString.Replace(decl, decl + "\n" + pi);
                                    FileDoc.content = System.Text.Encoding.UTF8.GetBytes(xmlString);
                                    FileDoc.length = (System.Text.Encoding.UTF8.GetBytes(xmlString)).Length;
                                    FileDoc.contentType = "text/FatturaPA";
                                }
                            }
                        }*/

                        if (!String.IsNullOrEmpty(FileDoc.fullName) && (FileDoc.fullName.ToUpper().EndsWith("XML")))
                        {
                            string urlXSL = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.FATTURAPA_XSL_URL.ToString()];
                            if (!string.IsNullOrEmpty(urlXSL))
                            {
                                //elaborazione per la fattura elettronica
                                byte[] xlstFatt = fatturazioneElettronicaInsertXlst(FileDoc.content);
                                if (xlstFatt != null) // se xlsFatt è null allora non è di tipo Fattura elettronica.
                                {
                                    FileDoc.content = xlstFatt;
                                    FileDoc.length = xlstFatt.Length;
                                    FileDoc.contentType = "text/FatturaPA";
                                }
                            }
                        }

                        //ABBATANGELI - LINEARIZZAZIONE
                        bool linearizze = false;

                        // fine modifica
                        switch (FileDoc.contentType)
                        {
                            //case "text/html":
                            case "text/xml":
                                if (Request.Browser.Browser.Trim().ToUpperInvariant().Equals("IE"))
                                    Response.ContentType = "image/jpeg";
                                else
                                    Response.ContentType = "text/plain";
                                break;
                            case "imagepng":
                                Response.ContentType = "image/png";
                                break;
                            case "application/rtf":
                            case "applicationmsword":
                                Response.ContentType = "application/msword";
                                break;
                            case "applicationvnd.ms-excel":
                                Response.ContentType = "application/vnd.ms-excel";
                                break;
                            case "text/FatturaPA":
                                Response.ContentType = "text/xml";
                                break;
                            // INTEGRAZIONE PITRE-PARER
                            case "text/xsl":
                                Response.ContentType = "text/xml";
                                break;
                            case "application/pdf":
                                //ABBATANGELI - LINEARIZZAZIONE
                                //if (CHIAVE DI CONFIGURAZIONE ABILITATA)
                                linearizze = true;
                                break;
                            default:
                                Response.ContentType = FileDoc.contentType;
                                break;
                        }

                        if (linearizze)
                        {
                            try
                            {
                                DocsPaWR.FileRequest fileRequest = null;

                                if (DocumentManager.getSelectedAttachId() != null) // ho aggiunto il file ad un allegato
                                {
                                    fileRequest = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId());
                                }
                                else // ho aggiunto il file al documento principale
                                {
                                    fileRequest = FileManager.getSelectedFile();
                                }
 
                                FileDocumento doc = FileDoc;

                                
                                DateTime dataFile = DateTime.MinValue;
                                DateTime.TryParseExact(fileRequest.dataAcquisizione, PDFDownloadHandler._httpDateFormats, null, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out dataFile);
                                string nomeOriginale = (doc.nomeOriginale != null) ? doc.nomeOriginale.Replace(",", "") : doc.nomeOriginale;
                                string name = fileRequest.versionId + nomeOriginale;

                                //INC000001202054 UNITN. il visualizzatore nativo di Chrome salva i pdf estratto dal file CADES come 
                                string nomeSenzap7mOtsd = removeAllCryptExtensions(name);

                                PDFDownloadHandler handler = new PDFDownloadHandler(doc.contentType, nomeSenzap7mOtsd, ChangeSignature, dataFile, FileDoc.content.Length, doc.content, Request, Response);
                                handler.ManageMultipartDownload();
                                this.ChangeSignature = false;
                            }
                            catch (System.Exception ex)
                            {
                                UIManager.AdministrationManager.DiagnosticError(ex);
                            }
                        }
                        else
                        {

                            Response.AddHeader("content-disposition", "inline;filename=" + FileDoc.name);
                            Response.AddHeader("content-length", FileDoc.content.Length.ToString());
                            if (FileDoc.contentType == "text/plain" /*|| FileDoc.contentType == "text/html" || FileDoc.contentType == "text/xml"*/)
                            {
                                string result = System.Text.Encoding.UTF8.GetString(FileDoc.content);
                                if (!result.Contains("\r\n"))
                                    result = result.Replace("\n", "\r\n");
                                Response.Write(result);
                            }
                            else
                                Response.BinaryWrite(FileDoc.content);
                            Response.End();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Modifica l'header del file XML della fatturazione elettronica con 
        /// l'intestazione per il xlst in modo da essere visualizzato in modo umano
        /// Faillace 23/6/2014
        /// </summary>
        /// <param name="xmlByteArray">xml in ingresso</param>
        /// <returns>xml in uscita, se non è di tipo fatturazione , restituisce null</returns>
        private byte[] fatturazioneElettronicaInsertXlst(byte[] xmlByteArray)
        {
            
            
            System.Xml.XmlDocument xd = new System.Xml.XmlDocument();
            //Predisposizione FIX xml 1.1 se escono anomalie decommenta il codice sotto e sostituisci la xd.Load con la xd.LoadXml nel try.
            
            System.IO.TextReader tr = new System.IO.StreamReader(new System.IO.MemoryStream(xmlByteArray));
            string fattura = tr.ReadToEnd();
            fattura = fattura.Replace("<?xml version=\"1.1", "<?xml version=\"1.0");  //FIX per l'xml 1.1 (che non viene processato da dotnet)
            bool isFattura = false;
            
            try
            {
                xd.LoadXml(fattura);
                //xd.Load(new System.IO.MemoryStream(xmlByteArray));
                //controllo se il namespace è tipo fattura elettronica 
                if (xd.DocumentElement.NamespaceURI.ToLower().Equals("http://www.fatturapa.gov.it/sdi/fatturapa/v1.0") ||
                    xd.DocumentElement.NamespaceURI.ToLower().Equals("http://www.fatturapa.gov.it/sdi/fatturapa/v1.1") ||
                    xd.DocumentElement.NamespaceURI.ToLower().Equals("http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2"))
                {
                    System.Xml.XmlNamespaceManager mgr = new System.Xml.XmlNamespaceManager(xd.NameTable);
                    //se lo è aggiungo il namespace e il prefix
                    mgr.AddNamespace(xd.DocumentElement.Prefix, xd.DocumentElement.NamespaceURI);
                    //faccio una ricerca xpat per trovare il nodo FatturaElettronica
                    // Old Code
                    //System.Xml.XmlNode xn = xd.SelectSingleNode("//p:FatturaElettronica", mgr);
                    // New Code
                    System.Xml.XmlNode xn = null;

                    if (!string.IsNullOrEmpty(xd.DocumentElement.Prefix))
                        xn = xd.SelectSingleNode("//" + xd.DocumentElement.Prefix + ":FatturaElettronica", mgr);
                    else
                    {
                        //xn = xd.SelectSingleNode("FatturaElettronica");
                        if (xd.DocumentElement != null &&
                            !string.IsNullOrEmpty(xd.DocumentElement.Name) &&
                            xd.DocumentElement.Name.Equals("FatturaElettronica"))
                            xn = xd.DocumentElement;
                        else
                            xn = xd.SelectSingleNode("FatturaElettronica");
                    }

                    if (xn != null)
                    {
                        string nsUri = xd.DocumentElement.NamespaceURI.ToLower();
                        //Se è una fattura elettronica creo l'intestazione xlst con in path il file di trasfomazione che sta nell'xml di DPA
                        System.Xml.XmlProcessingInstruction dpaProcessingInformation = xd.CreateProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"../xml/fatturapa_v1.0.xsl\"");
                        if (nsUri.EndsWith("v1.1"))
                            dpaProcessingInformation = xd.CreateProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"../xml/fatturapa_v1.1.xsl\"");
                        else if(nsUri.EndsWith("v1.2"))
                            dpaProcessingInformation = xd.CreateProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"../xml/fatturapa_v1.2.xsl\"");
                        
                        //Ricerco il un eventuale xlst preessitente nel file xml
                        System.Xml.XmlProcessingInstruction foundProcessingInformation = xd.SelectSingleNode("//processing-instruction(\"xml-stylesheet\")") as System.Xml.XmlProcessingInstruction;
                        if (foundProcessingInformation != null)
                        {
                            // se lo trova lo rimpazza con quello geenrato sopra
                            xd.ReplaceChild(dpaProcessingInformation, foundProcessingInformation);
                        }
                        else
                        {
                            //inserisce l'xlst in cima al documento prima del primo tag
                            xd.InsertBefore(dpaProcessingInformation, xn);
                        }
                        //preparo un memorystream per l'output 
                        System.IO.MemoryStream msOut = new System.IO.MemoryStream();
                        //salvo il risultato sul memorystream 
                        xd.Save(msOut);
                        isFattura = true;
                        //esco con il risultato
                        return msOut.ToArray();
                        
                    }
                }
                if(!isFattura && (xd.DocumentElement.NamespaceURI.ToLower().Equals("http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fattura/messaggi/v1.0")
                                || xd.DocumentElement.NamespaceURI.ToLower().Equals("http://www.fatturapa.gov.it/sdi/fatturapa/v1.0")
                                || xd.DocumentElement.NamespaceURI.ToLower().Equals("http://www.fatturapa.gov.it/sdi/messaggi/v1.0")))
                {
                    // Ricevute SDI
                    System.Xml.XmlNamespaceManager mgr = new System.Xml.XmlNamespaceManager(xd.NameTable);

                    if(xd.DocumentElement.NamespaceURI.ToLower().Equals("http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fattura/messaggi/v1.0"))
                    {
                        mgr.AddNamespace("ns3", "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fattura/messaggi/v1.0");
                    }
                    else if(xd.DocumentElement.NamespaceURI.ToLower().Equals("http://www.fatturapa.gov.it/sdi/fatturapa/v1.0"))
                    {
                        mgr.AddNamespace("ns3", "http://www.fatturapa.gov.it/sdi/fatturapa/v1.0");
                    }
                    else if(xd.DocumentElement.NamespaceURI.ToLower().Equals("http://www.fatturapa.gov.it/sdi/messaggi/v1.0"))
                    {
                        mgr.AddNamespace("ns3", "http://www.fatturapa.gov.it/sdi/messaggi/v1.0");
                    }
                    else
                    {
                        return null;
                    }

                    string urlXSL = string.Empty;

                    if(xd.SelectSingleNode("//ns3:RicevutaConsegna", mgr) != null)
                    {
                        urlXSL = "../xml/RC_v1.1.xsl";
                    }
                    else if(xd.SelectSingleNode("//ns3:NotificaMancataConsegna", mgr) != null || xd.SelectSingleNode("//ns3:RicevutaImpossibilitaRecapito", mgr) != null)
                    {
                        urlXSL = "../xml/MC_v1.1.xsl";
                    }
                    else if (xd.SelectSingleNode("//ns3:NotificaScarto", mgr) != null || xd.SelectSingleNode("//ns3:RicevutaScarto", mgr) != null)
                    {
                        urlXSL = "../xml/NS_v1.1.xsl";
                    }
                    else if (xd.SelectSingleNode("//ns3:NotificaEsito", mgr) != null)
                    {
                        urlXSL = "../xml/EC_v1.0.xsl";
                    }
                    else if (xd.SelectSingleNode("//ns3:NotificaDecorrenzaTermini", mgr) != null)
                    {
                        urlXSL = "../xml/DT_v1.0.xsl";
                    }
                    else if (xd.SelectSingleNode("//types:ScartoEsitoCommittente", mgr) != null)
                    {
                        urlXSL = "../xml/SE_v1.0.xsl";
                    }


                    string decl = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                    string decl8 = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                    string pi = "<?xml-stylesheet type=\"text/xsl\" href=\"" + urlXSL + "\"?>";

                    string previewXml = fattura.Replace(decl, decl + "\n" + pi);
                    previewXml = previewXml.Replace(decl8, decl8 + "\n" + pi);
                    xd.LoadXml(previewXml);
                    Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(xd.OuterXml);

                    return bytes;


                }
            }
            catch(Exception)
            {

            }

            return null;
        }


        private FileDocumento SaveFileDocument(FileRequest fileReq)
        {
            SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
            FileDocumento fileDoc = new FileDocumento();
            if (DocToSign == null)
            {

                //if (fileReq.fileName.EndsWith("." + EML))
                //{
                //    fileDoc = FileManager.getInstance(documentTab.systemId).GetFileAsEML(this.Page, fileReq);
                //}
                //else
                //{
                //fileDoc = FileManager.getInstance(documentTab.systemId).GetFile(this.Page, fileReq, ShowDocumentAsPdfFormat);
                // }

                //if required viewing with signature

                if (DocumentManager.getSelectedRecord() != null && !DocumentManager.IsNewDocument() && !DisableSignature(fileReq))
                {
                    if (IsPdf(fileReq))
                    {
                        fileDoc = DocumentWithSignature(fileReq);
                    }
                    else
                    {
                        fileDoc = FileManager.getInstance(documentTab.systemId).GetFile(this.Page, fileReq, ShowDocumentAsPdfFormat);
                    }
                }
                else
                {
                    fileDoc = FileManager.getInstance(documentTab.systemId).GetFile(this.Page, fileReq, ShowDocumentAsPdfFormat);
                }

            }
            else
            {
                fileDoc = DocToSign;
            }

            return fileDoc;
        }

        /// <summary>
        /// Disabilita la segnatura se il file non è di tipo pdf, ha impressa la segnatura e se è un allegato non di tipo utente
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        private bool DisableSignature(FileRequest file)
        {
            //Verifico se la versione del documento ha impressa la segnatura
            bool withSignature = DocumentManager.IsVersionWithSegnature(file.versionId);

            //Verifico se la versione è un pdf o un pdf con firma digitale, oppure se è un file convertibile in pdf
            // Gabriele Melini 24-02-2014
            // le stringhe con le estensioni sono definite minuscole
            // il ToLower evita schianti in caso di file con estensioni maiuscole (e.g. INC000000309701)
            bool isPdf = (
                //FileManager.getEstensioneIntoSignedFile(file.fileName).Equals(PDF) ||
                //FileManager.getEstensioneIntoSignedFile(file.fileName).Equals(TSD) ||
                //FileManager.getEstensioneIntoSignedFile(file.fileName).Equals(M7M) ||
                FileManager.getEstensioneIntoSignedFile(file.fileName).ToLowerInvariant().Equals(PDF) ||
                FileManager.getEstensioneIntoSignedFile(file.fileName).ToLowerInvariant().Equals(TSD) ||
                FileManager.getEstensioneIntoSignedFile(file.fileName).ToLowerInvariant().Equals(M7M) ||
                (ShowDocumentAsPdfFormat && PdfConverterInfo.CanConvertFileToPdf(file.fileName)) ||
                 (!string.IsNullOrEmpty(NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_RENDER_PDF")) && NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_RENDER_PDF").Equals("1")));

            bool isAllegatoUser = true;
            //Verifico se il file request è un allegato di tipo utente
            if (file.GetType() == typeof(Allegato))
                isAllegatoUser = (file as Allegato).TypeAttachment == CODE_ATTACH_USER;
            else
                //nel caso di dettaglio allegato verifico che sia di tipo utente
                if (file.GetType() == typeof(Documento) && this.Page.Request["typeAttachment"] != null)
                    isAllegatoUser = Convert.ToInt32(this.Page.Request["typeAttachment"]) == CODE_ATTACH_USER;

            // Controllo se l'allegato è esterno

            bool isAllegatoExt = true;
            //Verifico se il file request è un allegato di tipo utente
            if (file.GetType() == typeof(Allegato))
                isAllegatoExt = (file as Allegato).TypeAttachment == CODE_ATTACH_EXT;
            else
                //nel caso di dettaglio allegato verifico che sia di tipo utente
                if (file.GetType() == typeof(Documento) && this.Page.Request["typeAttachment"] != null)
                    isAllegatoExt = Convert.ToInt32(this.Page.Request["typeAttachment"]) == CODE_ATTACH_EXT;


            return !(!withSignature && isPdf && (isAllegatoUser || isAllegatoExt));
        }


        private bool IsPdf(FileRequest file)
        {
            // Gabriele Melini 24-02-2014
            // le stringhe con le estensioni sono definite minuscole
            // il ToLower evita schianti in caso di file con estensioni maiuscole
            return (
                //FileManager.getEstensioneIntoSignedFile(file.fileName).Equals(PDF) ||
                //FileManager.getEstensioneIntoSignedFile(file.fileName).Equals(TSD) ||
                //FileManager.getEstensioneIntoSignedFile(file.fileName).Equals(M7M) ||
                FileManager.getEstensioneIntoSignedFile(file.fileName).ToLowerInvariant().Equals(PDF) ||
                FileManager.getEstensioneIntoSignedFile(file.fileName).ToLowerInvariant().Equals(TSD) ||
                FileManager.getEstensioneIntoSignedFile(file.fileName).ToLowerInvariant().Equals(M7M) ||
                (ShowDocumentAsPdfFormat && PdfConverterInfo.CanConvertFileToPdf(file.fileName)));

        }

        /// <summary>
        /// Returns the document / attachment with signature in the case of pdf file
        /// </summary>
        /// <param name="fileReq"></param>
        /// <returns></returns>
        private FileDocumento DocumentWithSignature(FileRequest fileReq)
        {
            SchedaDocumento sch = null;
            if (FileManager.GetSelectedAttachment() == null)
                sch = DocumentManager.getSelectedRecord();
            else if (DocumentManager.getSelectedRecord().documentoPrincipale == null)
            {
                sch = DocumentManager.getDocumentListVersions(this.Page, fileReq.docNumber, fileReq.docNumber);
            }
            Response.Expires = -1;
            DocsPaWR.InfoUtente infoUser = UserManager.GetInfoUser();
            DocsPaWR.labelPdf label = new DocsPaWR.labelPdf();
            //load data in the object Label
            label.position = PositionSignature;
            label.tipoLabel = TypeLabel;
            label.label_rotation = RotationSignature;
            label.sel_font = CharacterSignature;
            label.sel_color = ColorSignature;
            label.orientamento = OrientationSignature;
            if (fileReq.firmato == "1" || NoTimbro)
                label.notimbro = NoTimbro;
            if (PrintOnFirstPage || PrintOnLastPage)
            {
                label.digitalSignInfo = new labelPdfDigitalSignInfo();
                label.digitalSignInfo.printOnFirstPage = PrintOnFirstPage;
                label.digitalSignInfo.printOnLastPage = PrintOnLastPage;
                if (fileReq.firmato == "1" || NoTimbro)
                    label.digitalSignInfo.printFormatSign = FormatSignature;
            }
            DocsPaWR.SchedaDocumento schedaCorrente = NttDataWA.UIManager.DocumentManager.getSelectedRecord();
            FileDocumento theDoc;
            if (!string.IsNullOrEmpty(NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_RENDER_PDF")) &&
                NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_RENDER_PDF").Equals("1") && !IsPdf(fileReq))
            {
                try
                {
                    theDoc = FileManager.getInstance(sch.systemId).DocumentoGetFileConSegnaturaUsingLC(this.Page, sch, label, fileReq);
                    if (theDoc == null)
                    {
                        if (fileReq.fileName.EndsWith("." + EML))
                        {
                            theDoc = FileManager.getInstance(sch.systemId).GetFileAsEML(this.Page, fileReq);
                        }
                        else
                        {
                            theDoc = FileManager.getInstance(sch.systemId).GetFile(this.Page, fileReq, ShowDocumentAsPdfFormat);
                            return theDoc;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    if (fileReq.fileName.EndsWith("." + EML))
                    {
                        theDoc = FileManager.getInstance(sch.systemId).GetFileAsEML(this.Page, fileReq);
                    }
                    else
                    {
                        theDoc = FileManager.getInstance(sch.systemId).GetFile(this.Page, fileReq, ShowDocumentAsPdfFormat);
                    }
                }
            }
            else
            {
                theDoc = FileManager.getInstance(sch.systemId).getFileConSegnatura(this.Page, sch, label, fileReq);
            }
            if (theDoc != null && theDoc.LabelPdf.default_position.Equals("pos_pers"))
                theDoc.LabelPdf.default_position = theDoc.LabelPdf.positions[4].PosX + "-" + theDoc.LabelPdf.positions[4].PosY;

            return theDoc;
        }

        /// <summary>
        /// 
        /// </summary>
        private bool ShowDocumentAsPdfFormat
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["SHOW_DOCUMENT_AS_PDF_FORMAT"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["SHOW_DOCUMENT_AS_PDF_FORMAT"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["SHOW_DOCUMENT_AS_PDF_FORMAT"] = value;
            }
        }


        /// <summary>
        /// Restituisce il file firmato
        /// </summary>
        private FileDocumento DocToSign
        {
            get
            {
                FileDocumento fileDoc = null;
                if (HttpContext.Current.Session["docToSign"] != null)
                    fileDoc = HttpContext.Current.Session["docToSign"] as FileDocumento;
                return fileDoc;
            }
        }
    }
}
