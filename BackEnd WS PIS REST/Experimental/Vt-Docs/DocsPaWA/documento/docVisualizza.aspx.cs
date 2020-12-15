using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
//using iTextSharp.text.pdf;

namespace DocsPAWA.documento
{
	/// <summary>
	/// Summary description for docVisualizza.
	/// </summary>
	public class docVisualizza : System.Web.UI.Page
	{
        private void Page_Load(object sender, System.EventArgs e)
        {
            Response.Expires = -1;
            DocsPaWR.FileDocumento theDoc = null;
            DocsPaWR.labelPdf label = new DocsPAWA.DocsPaWR.labelPdf();
            string id = string.Empty;
            string plusEtic = string.Empty;
            string PosLabelPdf = null;
            bool tipo = false;
            bool primaPag = false;
            bool ultimaPag = false;
            string rotazione = null;
            string carattere = string.Empty;
            string colore = string.Empty;
            string orientamento = null;
            //Mev Firma1 < diciarazione variabile 
            string notimbro = string.Empty;
            //>
            string versioneStampabile = string.Empty;

            // Valore booleano per indicare se il documento va visualizzato
            // nel suo formato originale
            bool visInOrig = false;

            // Se il query string contiene visInOrig impostato a 1
            // ed il ruolo è abilitato alla visualizzazione del file in
            // originale
            if (!string.IsNullOrEmpty(Request.QueryString["visInOrig"])
                && Request.QueryString["visInOrig"].ToString().Equals("1")
                && UserManager.ruoloIsAutorized(this, "DO_VIS_ORIG"))
                visInOrig = true;

            if (Session["docToSign"] == null)
            {
                id = Request["id"];
                plusEtic = Request["plusEtic"];
                PosLabelPdf = Request["pos"];
                tipo = System.Convert.ToBoolean(Request["tipo"]);
                rotazione = Request["rotazione"];
                carattere = Request["carattere"];
                colore = Request["colore"];
                orientamento = Request["orientamento"];
                //MEV Firma1 < set su variabile notimbro
                notimbro = Request["notimbro"];
                //>
                versioneStampabile = Request["versioneStampabile"];
                //carico i dati dentro l'oggetto Label
                label.position = PosLabelPdf;
                label.tipoLabel = tipo;
                label.label_rotation = rotazione;
                label.orientamento = orientamento;
                //Mev Firma1 <
                if (notimbro != null)
                    label.notimbro = (notimbro.ToUpper() == "TRUE") ? true : false;
                else
                    label.notimbro = false;
                //>
                label.sel_font = carattere;
                label.sel_color = colore;
                primaPag = System.Convert.ToBoolean(this.Session["printOnFirstPage"]);
                ultimaPag = System.Convert.ToBoolean(this.Session["printOnLastPage"]);
                if (primaPag || ultimaPag)
                {
                    label.digitalSignInfo = new DocsPaWR.labelPdfDigitalSignInfo();
                    label.digitalSignInfo.printOnFirstPage = primaPag;
                    label.digitalSignInfo.printOnLastPage = ultimaPag;
                }
                //Mev Firma1 <
                if (Session["printFormatSign"]!=null)
                     label.digitalSignInfo.printFormatSign = (DocsPaWR.TypePrintFormatSign)Session["printFormatSign"];
                //>


                // Garantisce che nella selezione del comando “Visualizza dati con segnatura” siano sempre e 
                // solo mostrati i dati identificativi del documento. 
                // Non vengono mantenute in sessione eventuali selezioni operate nella popup di posizionamento dati.
                if (Session["SHOWDOCWITHSEGNATURE"] != null) label.digitalSignInfo = null;
                //>

                    if (!(id != null && !id.Equals("")))
                        id = Session.SessionID;

                DocsPaWR.FileRequest fileRequestVisualizzatore = this.Session["VisualizzatoreUnificato.SelectedFileRequest"] as DocsPaWR.FileRequest;

                DocsPaWR.SchedaDocumento schedaCorrente = DocumentManager.getDocumentoSelezionato(Page);

                if (plusEtic != null && plusEtic != "" && schedaCorrente != null && fileRequestVisualizzatore != null && !string.IsNullOrEmpty(schedaCorrente.docNumber) && !string.IsNullOrEmpty(fileRequestVisualizzatore.docNumber) && schedaCorrente.docNumber.Equals(fileRequestVisualizzatore.docNumber))
                {
                    if (fileRequestVisualizzatore == null)
                        theDoc = FileManager.getInstance(id).getFileConSegnatura(this, schedaCorrente, label, null);
                    else
                    {
                        theDoc = FileManager.getInstance(id).getFileConSegnatura(this, schedaCorrente, label, fileRequestVisualizzatore);
                        this.Session.Remove("VisualizzatoreUnificato.SelectedFileRequest");
                    }

                    //aggiungo in session le info relative alle label
                    Session.Add("labelProperties", theDoc.LabelPdf);
                }
                else
                {
                    bool showAsPdfFormat = false;

                    // Se visInOrig è false...
                    if (!visInOrig)
                    {
                        try
                        {
                            // Verifica se convertire il file visualizzato in pdf
                            string config = ConfigurationManager.AppSettings["SHOW_DOCUMENT_AS_PDF_FORMAT"];

                            if (config != null && config != string.Empty)
                                showAsPdfFormat = Convert.ToBoolean(config);
                        }
                        catch { }
                    }

                    if (fileRequestVisualizzatore == null)
                        theDoc = FileManager.getInstance(id).getFile(this, showAsPdfFormat);
                    else
                    {
                        if (fileRequestVisualizzatore.fileName.ToUpper().EndsWith(".EML"))
                        {
                            if (!string.IsNullOrEmpty(versioneStampabile) && versioneStampabile.ToUpper().Equals("FALSE"))
                                theDoc = FileManager.getInstance(id).getFileAsEML(this, fileRequestVisualizzatore);
                            else
                                theDoc = FileManager.getInstance(id).getFile(this, fileRequestVisualizzatore, showAsPdfFormat);
                        }
                        else
                            theDoc = FileManager.getInstance(id).getFile(this, fileRequestVisualizzatore, showAsPdfFormat);
                        this.Session.Remove("VisualizzatoreUnificato.SelectedFileRequest");
                    }
                }
            }
            else
            {
                theDoc = (DocsPAWA.DocsPaWR.FileDocumento)Session["docToSign"];
            }

            if (theDoc != null && (!string.IsNullOrEmpty(theDoc.name)))
            {
                bool downloadAsAttatchment;
                bool.TryParse(this.Request.QueryString["downloadAsAttatchment"], out downloadAsAttatchment);

                if (theDoc.estensioneFile == null && theDoc.fullName.Contains("."))
                {
                    string extension = theDoc.fullName.Substring(theDoc.fullName.LastIndexOf(".") + 1);
                    theDoc.estensioneFile = extension;
                    if (theDoc.estensioneFile.ToUpper().Equals("XML")) 
                    {
                        //elaborazione per la fattura elettronica
                        byte[] xlstFatt =  fatturazioneElettronicaInsertXlst(theDoc.content);
                        if (xlstFatt == null)
                            theDoc.contentType = "image/jpeg";
                        else
                            theDoc.content = xlstFatt;
                    }
                }

                if (!string.IsNullOrEmpty(versioneStampabile) && versioneStampabile.ToUpper().Equals("FALSE") && theDoc.estensioneFile != null && theDoc.estensioneFile.ToUpper().Equals("EML"))
                {
                   // string emlContent = FileManager.getInstance(id).getEMLContent(this, theDoc.fullName);
                    Response.ContentType = theDoc.contentType;
                    Response.AddHeader("content-disposition", "inline;filename=" + theDoc.name);
                    Response.AddHeader("content-length", theDoc.content.Length.ToString());
                    Response.BinaryWrite(theDoc.content);
                    Response.End();
                }
                else
                {
                    if (!downloadAsAttatchment)
                    {
                        Response.ContentType = theDoc.contentType;
                        Response.AddHeader("content-disposition", "inline;filename=" + theDoc.name);
                        Response.AddHeader("content-length", theDoc.content.Length.ToString());
                        Response.BinaryWrite(theDoc.content);
                        Response.End();
                    }
                    else
                    {
                        // Download del documento come attatchment
                        Response.ContentType = theDoc.contentType;
                        Response.AddHeader("content-disposition", "attachment;filename=" + theDoc.name);
                        Response.BinaryWrite(theDoc.content);
                        Response.End();
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
            System.IO.TextReader tr = new System.IO.StreamReader(new System.IO.MemoryStream(xmlByteArray));
            string fattura = tr.ReadToEnd();
            fattura = fattura.Replace("<?xml version=\"1.1", "<?xml version=\"1.0"); //FIX per l'xml 1.1 (che non viene processato da dotnet)
            try
            {
                xd.LoadXml(fattura);
                //controllo se il namespace è tipo fattura elettronica 
                if (xd.DocumentElement.NamespaceURI.ToLower().Equals("http://www.fatturapa.gov.it/sdi/fatturapa/v1.0") ||
                    xd.DocumentElement.NamespaceURI.ToLower().Equals("http://www.fatturapa.gov.it/sdi/fatturapa/v1.1"))
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
                        //esco con il risultato
                        return msOut.ToArray();
                    }
                }
            }
                // il caricamento dell'xml ha dato errore (xml non formattato, tag non chiusi etc etc etc)
            catch 
            { }
            //l'eleaborazione è avvenuta in modo corretto, o non è una fattura  elettronica 
            //oppure non è un xml oppure si è verificato un errore
            return null;
        }

        #region old_method stampa etichetta
        /// <summary>
        /// aggiunge etichetta ad un file pdf
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        //		private  byte[] addEtic( byte[] content)
        //		{
        //			byte[] rtn=null;
        //			try
        //			{
        //				if(System.IO.File.Exists("C:\\temp\\risultato.pdf"))
        //					System.IO.File.Delete("C:\\temp\\risultato.pdf");
        //				
        //
        //				DocsPaWR.SchedaDocumento schedaCorrente=DocumentManager.getDocumentoSelezionato(Page);
        //				System.IO.MemoryStream ms=new System.IO.MemoryStream(content,true);//("C:\\temp\\"+UserManager.getUtente(Page).userId+".pdf",System.IO.FileAccess.ReadWrite);
        //				ms.Position=0;
        //				//iTextSharp.text.pdf.PdfReader prd =new PdfReader("c:\\temp\\tiff.pdf");
        //				iTextSharp.text.pdf.PdfReader prd =new PdfReader(ms);
        //				string path="C:\\temp\\risultato.pdf";
        //				System.IO.FileStream fs= new System.IO.FileStream(path,System.IO.FileMode.OpenOrCreate);
        //				iTextSharp.text.pdf.PdfStamper stamp=new PdfStamper(prd,fs);
        //				stamp.Writer.Open();
        //				PdfContentByte cb = stamp.GetOverContent(1);
        //				BaseFont bf = BaseFont.CreateFont(
        //					BaseFont.COURIER,
        //					BaseFont.CP1252,
        //					BaseFont.NOT_EMBEDDED);	
        //				cb.BeginText();
        //				cb.SetFontAndSize(bf, 14);
        //				//cb.SetTextMatrix(50,800);
        //				//cb.ShowTextAligned(1,"ANAS",26,820,0);
        //				cb.ShowTextAligned(1,schedaCorrente.protocollo.segnatura,110,806,0);
        //				//cb.ShowText(schedaCorrente.protocollo.segnatura);
        //				cb.EndText();
        //				stamp.Close();
        //				System.IO.FileStream fileStream = new System.IO.FileStream("c:\\temp\\risultato.pdf", System.IO.FileMode.Open);
        //				int fileLength = (int)fileStream.Length;
        //				rtn= new byte[fileLength];
        //				fileStream.Read(rtn, 0, fileLength);
        //				fileStream.Close();
        //				//int dim=(int)fs.Length;
        //				//rtn=new byte[fileLength];
        //				//fs.Read(rtn,0,dim);
        //				
        //				//fs.Close();
        //				return rtn;
        //			}
        //			catch(Exception ex)
        //			{
        //				ErrorManager.redirectToErrorPage(Page,ex);
        //				return null;
        //			} 
        //			
        //		}
        #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

		
		
	}
}
