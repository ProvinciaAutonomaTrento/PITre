using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class HistoryVers : System.Web.UI.Page
    {

        protected string status;
        protected bool isAttachment;

        protected void Page_Load(object sender, EventArgs e)
        {

            this.SetStatus();

            if (!IsPostBack)
            {
                this.InitializeLanguage();
                this.InitializePage();
            }
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            string labelNotFound = string.Empty;
            string labelCaption = string.Empty;
            this.HistoryVersBtnChiudi.Text = Utils.Languages.GetLabelFromCode("HistoryPreservedBtnChiudi", language);
            this.HistoryVersBtnRecupero.Text = Utils.Languages.GetLabelFromCode("HistoryVersBtnRecupero", language);
            this.HistoryVersBtnRecupero.ToolTip = Utils.Languages.GetLabelFromCode("HistoryVersBtnRecuperoTooltip", language);
            this.HistoryVersBtnRapporto.Text = Utils.Languages.GetLabelFromCode("HistoryVersBtnRapporto", language);
            this.HistoryVersBtnRapporto.ToolTip = Utils.Languages.GetLabelFromCode("HistoryVersBtnRapportoTooltip", language);

            this.HistoryVersLblCaption.Text = Utils.Languages.GetLabelFromCode("HistroyPreservedDocLblCaption", language);
            this.HistoryVersLblReport.Text = Utils.Languages.GetLabelFromCode("HistoryVersLblReport", language);
            this.HistoryVersLblRecover.Text = Utils.Languages.GetLabelFromCode("HistoryVersLblRecover", language);
        }

        private void InitializePage()
        {
            if (!this.status.Equals("C"))
            {
                this.HistoryVersBtnRecupero.Enabled = false;
            }
            if (this.status.Equals("C") || this.status.Equals("R"))
            {
                this.HistoryVersBtnRapporto.Enabled = true;
            }
            else
            {
                this.HistoryVersBtnRapporto.Enabled = false;
            }

            if (this.isAttachment)
            {
                this.HistoryVersBtnRapporto.Enabled = false;
                this.HistoryVersBtnRecupero.Enabled = false;
            }

            this.GetStatoConservazioneDoc();
        }

        private void SetStatus()
        {
            try
            {
                if (DocumentManager.getSelectedRecord().documentoPrincipale != null)
                {
                    this.status = UIManager.DocumentManager.getStatoConservazioneDoc(DocumentManager.getSelectedRecord().documentoPrincipale.docNumber);
                    this.isAttachment = true;
                }
                else
                {
                    this.status = UIManager.DocumentManager.getStatoConservazioneDoc(DocumentManager.getSelectedRecord().systemId);
                    this.isAttachment = false;
                }
            }
            catch (Exception ex)
            {
                this.status = UIManager.DocumentManager.getStatoConservazioneDoc(DocumentManager.getSelectedRecord().systemId);
                this.isAttachment = false;
            }

        }

        protected void HistoryVersBtnChiudi_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('HistoryVers','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void HistoryVersBtnRecupero_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            string result = DocumentManager.RecuperoStatoConservazione(DocumentManager.getSelectedRecord().systemId, UserManager.GetInfoUser());
            if (!string.IsNullOrEmpty(result))
            {
                this.PlcHistoryVersXml.Visible = true;
                this.HistoryVersLblRecover.Visible = true;
                this.HistoryVersLblReport.Visible = false;

                string xmlString = this.GetXML(result);

                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_URL_XSL_RAPPORTO_VERS.ToString())))
                {
                    //string urlXSL = Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_URL_XSL_RAPPORTO_VERS.ToString());
                    string urlXSL = "../xml/rapporto_versamento.xsl";
                    string decl = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                    string pi = string.Format("<?xml-stylesheet type=\"text/xsl\" href=\"{0}\"?>", urlXSL);
                    xmlString = xmlString.Replace(decl, decl + "\n" + pi);
                }

                FileDocumento fd = new FileDocumento();
                fd.content = System.Text.Encoding.UTF8.GetBytes(xmlString);
                fd.length = fd.content.Length;
                fd.name = "StatoConservazione.xml";
                fd.nomeOriginale = "StatoConservazione.xml";
                fd.estensioneFile = "xml";
                if (xmlString.Contains("text/xsl"))
                    fd.contentType = "text/xsl";
                else
                    fd.contentType = "text/xml";

                HttpContext.Current.Session["fileDoc"] = fd;
                this.frame.Attributes["src"] = "../Document/AttachmentViewer.aspx";
            }
            else
            {
                string msgDesc = "msgConsStatusError";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'error', '');}", true);
            }
            
            this.UpPnlHistoryVersXml.Update();
        }

        protected void HistoryVersBtnRapporto_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            string result = DocumentManager.GetRapportoVersamento(DocumentManager.getSelectedRecord().systemId, UserManager.GetInfoUser());
            if (!string.IsNullOrEmpty(result))
            {
                this.PlcHistoryVersXml.Visible = true;
                this.HistoryVersLblReport.Visible = true;
                this.HistoryVersLblRecover.Visible = false;

                string xmlString = this.GetXML(result);

                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_URL_XSL_RAPPORTO_VERS.ToString())))
                {
                    //string urlXSL = Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_URL_XSL_RAPPORTO_VERS.ToString());
                    string urlXSL = "../xml/rapporto_versamento.xsl";
                    string decl = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                    string pi = string.Format("<?xml-stylesheet type=\"text/xsl\" href=\"{0}\"?>", urlXSL);
                    xmlString = xmlString.Replace(decl, decl + "\n" + pi);
                }

                FileDocumento fd = new FileDocumento();
                fd.content = System.Text.Encoding.UTF8.GetBytes(xmlString);
                fd.length = fd.content.Length;
                fd.name = "RapportoVersamento.xml";
                fd.nomeOriginale = "RapportoVersamento.xml";
                fd.estensioneFile = "xml";
                if (xmlString.Contains("text/xsl"))
                    fd.contentType = "text/xsl";
                else
                    fd.contentType = "text/xml";

                HttpContext.Current.Session["fileDoc"] = fd;
                this.frame.Attributes["src"] = "../Document/AttachmentViewer.aspx";
            }
            else
            {
                string msgDesc = "msgConsRecoveryError";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'error', '');}", true);
            }

            this.UpPnlHistoryVersXml.Update();
        }

        private void GetStatoConservazioneDoc()
        {
            //string status = UIManager.DocumentManager.getStatoConservazioneDoc(DocumentManager.getSelectedRecord().systemId);
            string label = string.Empty;
            string language = UIManager.UserManager.GetUserLanguage();
            switch (this.status)
            {
                case "N":
                    label = Utils.Languages.GetLabelFromCode("optConsNC", language);
                    break;
                case "V":
                    label = Utils.Languages.GetLabelFromCode("optConsAtt", language);
                    break;
                case "W":
                    label = Utils.Languages.GetLabelFromCode("optConsVer", language);
                    break;
                case "C":
                    label = Utils.Languages.GetLabelFromCode("optConsPre", language);
                    break;
                case "R":
                    label = Utils.Languages.GetLabelFromCode("optConsRif", language);
                    break;
                case "E":
                    label = Utils.Languages.GetLabelFromCode("optConsErr", language);
                    break;
                case "F":
                    label = Utils.Languages.GetLabelFromCode("optConsFld", language);
                    break;
                case "T":
                    label = Utils.Languages.GetLabelFromCode("optConsTim", language);
                    break;
                case "B":
                    label = Utils.Languages.GetLabelFromCode("optConsBfw", language);
                    break;
                case "K":
                    label = Utils.Languages.GetLabelFromCode("optConsBfe", language);
                    break;

            }

            this.HistoryVersLblStatus.Text = label;

        }

        private string GetXML(string text)
        {
            XmlDocument xml = new XmlDocument();
            StringBuilder sb = new StringBuilder();

            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            xml.LoadXml(text);

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace,
                Encoding = new UTF8Encoding(false)
            };

            using (XmlWriter writer = XmlWriter.Create(ms, settings))
            {
                xml.Save(writer);
            }

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}