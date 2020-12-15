using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Popup
{
    public partial class dialog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                string language = UIManager.UserManager.GetUserLanguage();
                string type = Request.QueryString["type"];
                string message = Utils.Languages.GetMessageFromCode(Request.QueryString["msg"], language);
                string input = Request.QueryString["input"];

                message = message.Replace("@@", input);

                switch (type)
                {
                    case "check":
                        message = "<img src=\"" + Page.ResolveClientUrl("~/Images/Common/messager_check.png") + "\" alt=\"\" />" + message;
                        break;
                    case "error":
                        message = "<img src=\"" + Page.ResolveClientUrl("~/Images/Common/messager_error.png") + "\" alt=\"\" />" + message;
                        break;
                    case "info":
                        message = "<img src=\"" + Page.ResolveClientUrl("~/Images/Common/messager_info.png") + "\" alt=\"\" />" + message;
                        break;
                    case "question":
                        message = "<img src=\"" + Page.ResolveClientUrl("~/Images/Common/messager_question.gif") + "\" alt=\"\" />" + message;
                        break;
                    case "warning":
                        message = "<img src=\"" + Page.ResolveClientUrl("~/Images/Common/messager_warning.png") + "\" alt=\"\" />" + message;
                        break;
                }

                this.msg.Text = message;
                this.DialogBtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
    }
}