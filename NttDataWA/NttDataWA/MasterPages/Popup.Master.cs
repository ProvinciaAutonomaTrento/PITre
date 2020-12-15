using NttDataWA.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.MasterPages
{
    public partial class Popup : System.Web.UI.MasterPage
    {
        public String componentType = Constans.TYPE_APPLET;
        private String socketErrorIE = string.Empty;

        public String SocketErrorIE
        {
            get
            {
                return socketErrorIE;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializesPage();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InitializesPage()
        {
            this.InitializesLabel();
            this.InitializeCssText();
            this.InitSocket();
            
        }
        /// <summary>
        /// Set right or left Css
        /// </summary>
        /// <param name="languageDirection"></param>
        private void SetCssClass(string languageDirection)
        {
            string link = "~/Css/Left/popup.css";
            if (!string.IsNullOrEmpty(languageDirection) && languageDirection.Equals("rtl"))
            {
                link = "~/Css/Right/popup.css";
            }
            this.CssLayout.Attributes.Add("href", link);
        }

        protected void InitializesLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            string languageDirection = Utils.Languages.GetLanguageDirection(language);
            this.Html.Attributes.Add("dir", languageDirection);
            this.SetCssClass(languageDirection);
            //this.Loading.Text = Utils.Languages.GetLabelFromCode("Loading", string.Empty);

            this.litDialogCheck.Text = Utils.Languages.GetLabelFromCode("DialogCheckTitle", language);
            this.litDialogError.Text = Utils.Languages.GetLabelFromCode("DialogErrorTitle", language);
            this.litDialogInfo.Text = Utils.Languages.GetLabelFromCode("DialogInfoTitle", language);
            this.litDialogQuestion.Text = Utils.Languages.GetLabelFromCode("DialogQuestionTitle", language);
            this.litDialogWarning.Text = Utils.Languages.GetLabelFromCode("DialogWarningTitle", language);
            this.litConfirm.Text = Utils.Languages.GetLabelFromCode("DialogQuestionTitle", language);
            this.socketErrorIE = Utils.Languages.GetMessageFromCode("ErroreConnesioneSocketIE", language);
        }

        protected void InitializeCssText()
        {
            if (!string.IsNullOrEmpty(UIManager.CssManager.GetSizeText()))
            {
                if (UIManager.CssManager.GetSizeText().Equals(UIManager.CssManager.TextSize.NORMAL.ToString()))
                {
                    this.IdMasterBody.Attributes.Add("class", "body_normal");
                }
                else
                {
                    if (UIManager.CssManager.GetSizeText().Equals(UIManager.CssManager.TextSize.MEDIUM.ToString()))
                    {
                        this.IdMasterBody.Attributes.Add("class", "body_medium");
                    }
                    else
                    {
                        this.IdMasterBody.Attributes.Add("class", "body_high");
                    }
                }
            }
            else
            {
                this.IdMasterBody.Attributes.Add("class", "body_normal");
            }
        }

        protected void InitSocket()
        {
            this.componentType = UIManager.UserManager.getComponentType(Request.UserAgent);
            if (this.componentType == NttDataWA.Utils.Constans.TYPE_SOCKET)
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "script", "$(function () { CheckSocketIE(); });", true);
        }
       
    }
}