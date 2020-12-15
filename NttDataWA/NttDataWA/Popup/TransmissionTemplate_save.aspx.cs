using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class TransmissionTemplate_save : System.Web.UI.Page
    {

        #region Fields

        public int maxLength = 2000;

        #endregion

        #region Properties

        private DocsPaWR.ModelloTrasmissione Template
        {
            get
            {
                DocsPaWR.ModelloTrasmissione result = null;
                if (HttpContext.Current.Session["Transmission_template"] != null)
                {
                    result = HttpContext.Current.Session["Transmission_template"] as DocsPaWR.ModelloTrasmissione;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["Transmission_template"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializeLanguage();
                    this.rblShare.Items[0].Text = this.rblShare.Items[0].Text.Replace("@usr@", UserManager.GetUserInSession().descrizione);
                    this.rblShare.Items[1].Text = this.rblShare.Items[1].Text.Replace("@grp@", UserManager.GetSelectedRole().descrizione);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.TemplateBtnSave.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnSave", language);
            this.TemplateBtnClose.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnClose", language);
            this.TransmissionLitTemplateName.Text = Utils.Languages.GetLabelFromCode("TransmissionLitTemplateName", language);
            this.TransmissionLitCharsRemain.Text = Utils.Languages.GetLabelFromCode("TransmissionLitCharsRemain", language);
            this.TransmissionLitMakeAvailable.Text = Utils.Languages.GetLabelFromCode("TransmissionLitMakeAvailable", language);
            this.rblShare.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("TransmissionLitMakeAvailableOnlyMe", language), "usr"));
            this.rblShare.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("TransmissionLitMakeAvailableAllRole", language), "grp"));
            this.rblShare.Items[0].Selected = true;
        }

        protected void TemplateBtnSave_Click(object sender, EventArgs e)
        {
            //try {
                if (this.IsValid)
                {
                    if (this.SaveTemplate())
                    {
                        this.CloseMask();
                    }
                    else
                    {
                        RenderMessage("Errore nella creazione del modello");
                    }
                }
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        private bool SaveTemplate()
        {
            try
            {
                this.Template.NOME = this.txtTitle.Text;
                if (this.rblShare.Items[0].Selected)
                {
                    for (int k = 0; k < this.Template.MITTENTE.Length; k++)
                    {
                        this.Template.MITTENTE[k].ID_CORR_GLOBALI = 0;
                    }
                }
                else
                {
                    this.Template.ID_PEOPLE = "";
                }

                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                this.Template.CODICE = "MT_" + TransmissionModelsManager.GetTemplateSystemId();
                TransmissionModelsManager.SaveTemplate(this.Template, infoUtente);

                return true;
            }
            catch
            {
                return false;
            }
        }

        protected void TemplateBtnClose_Click(object sender, EventArgs e)
        {
            //try {
                this.CloseMask();
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        /// <summary>
        /// Close Mask
        /// </summary>
        /// <param name="versionId"></param>
        protected void CloseMask()
        {
            Response.Write("<html><body><script type=\"text/javascript\">if (parent.fra_main) {parent.fra_main.closeAjaxModal('TemplateSave');} else {parent.closeAjaxModal('TemplateSave');}</script></body></html>");
            Response.End();
        }

        /// <summary>
        /// Render Message
        /// </summary>
        /// <param name="message"></param>
        protected virtual void RenderMessage(string message)
        {
            rowMessage.InnerHtml = message;
            rowMessage.Visible = true;
        }

    }
}