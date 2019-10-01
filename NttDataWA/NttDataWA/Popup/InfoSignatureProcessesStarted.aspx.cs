using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class InfoSignatureProcessesStarted : System.Web.UI.Page
    {
        #region Const

        private const string DOCUMENT = "D";
        private const string ATTACHMENT = "A";

        #endregion

        #region Standard method

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializeLanguage();
                this.InitializePage();
            }
        }

        private void InitializePage()
        {
            SchedaDocumento doc = UIManager.DocumentManager.getSelectedRecord();
            this.BindInfoProcessesStarted(doc);
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.InfoSignatureProcessesStartedClose.Text = Utils.Languages.GetLabelFromCode("InfoSignatureProcessesStartedClose", language);
            this.lblMessage.Text = Utils.Languages.GetLabelFromCode("InfoSignatureProcessesStarted", language);
        }


        private void BindInfoProcessesStarted(SchedaDocumento doc)
        {
            try
            {
                string msg = string.Empty;
                string language = UIManager.UserManager.GetUserLanguage();
                string docnumber = (doc.documentoPrincipale != null && !string.IsNullOrEmpty(doc.documentoPrincipale.docNumber)) ? doc.documentoPrincipale.docNumber : doc.docNumber;
                List<IstanzaProcessoDiFirma> list = UIManager.LibroFirmaManager.GetInfoProcessesStartedForDocument(docnumber);
                if (list != null && list.Count > 0)
                {
                    IstanzaProcessoDiFirma istanza = (from i in list where i.docAll.Equals(DOCUMENT) select i).FirstOrDefault();
                    msg = istanza != null ? "InfoSignatureProcessesStartedDetailsMainDocument" : "InfoSignatureProcessesStartedDetailsMainDocumentNoProcesses";
                    this.lblMessageDetailsMainDocument.Text = istanza == null ? Utils.Languages.GetLabelFromCode(msg, language) : Utils.Languages.GetLabelFromCode(msg, language) + istanza.Descrizione;


                    List<IstanzaProcessoDiFirma> istanze = (from i in list where i.docAll.Equals(ATTACHMENT) select i).ToList();
                    if (istanze != null && istanze.Count > 0)
                    {
                        string istanzeAttach = string.Empty;
                        this.lblMessageDetailsAttachments.Text = Utils.Languages.GetLabelFromCode("InfoSignatureProcessesStartedDetailsAttachments", language);
                        foreach (IstanzaProcessoDiFirma i in istanze)
                        {
                            istanzeAttach += "<li> " + Truncate(i.oggetto, 30) + " (" + i.Descrizione + ")</li>";
                        }

                        this.lblListAttach.Text = istanzeAttach;
                    }
                    else
                    {
                        this.lblMessageDetailsAttachments.Text = Utils.Languages.GetLabelFromCode("InfoSignatureProcessesStartedDetailsAttachmentsNoProcesses", language);
                    }
                }
                else
                {
                    this.lblMessageDetailsMainDocument.Text = Utils.Languages.GetLabelFromCode("InfoSignatureProcessesStartedDetailsMainDocumentNoProcesses", language);

                    if (doc.allegati != null && doc.allegati.Length > 0)
                    {
                        this.lblMessageDetailsAttachments.Text = Utils.Languages.GetLabelFromCode("InfoSignatureProcessesStartedDetailsAttachmentsNoProcesses", language);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        private string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }

        private string FormatCodiceAllegato(int indexAllegato)
        {
            return string.Format("A{0:0#}", indexAllegato);
        }

        #endregion

        #region Event button

        protected void InfoSignatureProcessesStartedClose_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "closeAJM", "parent.closeAjaxModal('InfoSignatureProcessesStarted','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion
    }
}