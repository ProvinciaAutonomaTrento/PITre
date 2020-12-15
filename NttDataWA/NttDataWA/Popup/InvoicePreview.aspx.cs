using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class InvoicePreview : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                this.InitializeLanguage();

                this.InitializePage();
            }
        }

        protected void InitializePage()
        {
            MemoryStream ms = null;
            StreamReader streamReader = null;

            try
            {

                DocsPaWR.FileRequest fileReq = (DocsPaWR.FileRequest)DocumentManager.getSelectedRecord().documenti[0];
                DocsPaWR.FileDocumento fileDoc = DocumentManager.DocumentoGetFile(fileReq);

                string xmlString = GetFatturaFromBytes(fileDoc.name, fileDoc.content);

                XmlDocument xml = this.GenerateXMLDocumentFromString(xmlString);

                System.Xml.Xsl.XslCompiledTransform xslt = new System.Xml.Xsl.XslCompiledTransform();
                xslt.Load(System.IO.Path.Combine(Server.MapPath("~/"), "xml\\fatturapa_v1.2.xsl"));

                ms = new MemoryStream();

                System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(ms, new System.Text.UTF8Encoding());
                xslt.Transform(xml, null, writer);
                streamReader = new StreamReader(ms);

                DocsPaWR.FileDocumento pdfInvoice = ImportInvoiceManager.getInvoicePreviewPdf(ms.ToArray());

                if (pdfInvoice != null && pdfInvoice.content != null)
                {
                    pdfInvoice.name = fileDoc.name + ".pdf";
                    pdfInvoice.fullName = fileDoc.fullName + ".pdf";
                    pdfInvoice.estensioneFile = "pdf";
                    this.UpPnlDocumentData.Visible = true;
                    FileManager.setSelectedFileReport(this, pdfInvoice, "../popup");
                    //this.frame.Attributes["src"] = "../Summaries/PDFViewer.aspx";
                    this.frame.Attributes["src"] = "InvoicePDFViewer.aspx";
                    this.UpPnlDocumentData.Update();
                }
                else
                {
                    string msg = "InvoicePreviewError";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);

                    this.UpPnlDocumentData.Visible = false;
                    this.UpPnlDocumentData.Update();
                }

            }
            catch (Exception)
            {
                string msg = "InvoicePreviewError";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);

                this.UpPnlDocumentData.Visible = false;
                this.UpPnlDocumentData.Update();
            }
            finally
            {
                ms.Dispose();
                streamReader.Dispose();
            }
        }

        protected void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();

            this.InvoicePreviewBtnClose.Text = Utils.Languages.GetLabelFromCode("InvoicePreviewBtnClose", language);
        }

        protected void InvoicePreviewBtnClose_Click(object sender, EventArgs e)
        {
            this.frame.Visible = false;
            this.frame.Attributes["src"] = null;
            Session["FileManager.selectedReport"] = null;
            Session["EXPORT_FILE_SESSION"] = null;
            this.UpPnlDocumentData.Visible = false;
            this.UpPnlDocumentData.Update();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('InvoicePreview', '');} else {parent.closeAjaxModal('InvoicePreview', '');};", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected string GetFatturaFromBytes(string nomefile, byte[] content)
        {
            string retval = null;
            string xmlFattura = "";
            if (nomefile.ToUpper().EndsWith("XML") || nomefile.ToUpper().EndsWith("XML.P7M"))
            {
                //xmlFattura = System.Text.Encoding.UTF8.GetString(content);
                xmlFattura = System.Text.Encoding.Unicode.GetString(content);

                if (xmlFattura.ToLower().Contains("http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/"))
                {
                    retval = xmlFattura;
                }
            }
            return retval;
        }

        protected XmlDocument GenerateXMLDocumentFromString(string xml)
        {
            XmlDocument _xmlDoc = new XmlDocument();


            try
            {
                _xmlDoc.LoadXml(xml);
            }
            catch (Exception ex)
            {
                try
                {
                    _xmlDoc.LoadXml(xml.Trim());
                }
                catch (Exception x2)
                {
                    try
                    {
                        _xmlDoc.LoadXml(_removeUtf8ByteOrderMark(xml));
                    }
                    catch (Exception)
                    {
                        _xmlDoc = null;
                    }
                }
            }
            return _xmlDoc;
        }

        protected string _removeUtf8ByteOrderMark(string xml)
        {
            try
            {
                string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                if (xml.StartsWith(_byteOrderMarkUtf8))
                {
                    xml = xml.Remove(0, _byteOrderMarkUtf8.Length);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return xml.Trim();
        }

        protected void setSelectedFileReport(DocsPaWR.FileDocumento fileDoc)
        {
            Session["FileManager.selectedReport"] = fileDoc;
            Session["EXPORT_FILE_SESSION"] = fileDoc;
        }
    }
}