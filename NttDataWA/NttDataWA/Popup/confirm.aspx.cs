using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Popup
{
    public partial class confirm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //try {
                string language = UIManager.UserManager.GetUserLanguage();
                string hiddenToValorize = Request.QueryString["hidden"];
                string message = Utils.Languages.GetMessageFromCode(Request.QueryString["msg"], language);
                string input = Request.QueryString["input"];

                message = message.Replace("@@", input);

                message = "<img src=\"" + Page.ResolveClientUrl("~/Images/Common/messager_question.gif") + "\" alt=\"\" />" + message;
                this.msg.Text = message;

                this.DialogBtnOk.Text = Utils.Languages.GetLabelFromCode("GenericBtnOk", language);
                this.DialogBtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnCancel", language);

                this.DialogBtnOk.OnClientClick = "if (parent.fra_main) {parent.fra_main.$('#" + hiddenToValorize + "').val('true'); parent.fra_main.__doPostBack('" + hiddenToValorize + "', ''); parent.fra_main.$('#confirm_modal').dialog('close');} else {parent.$('#" + hiddenToValorize + "').val('true'); parent.__doPostBack('" + hiddenToValorize + "', ''); parent.$('#confirm_modal').dialog('close');}";
                this.DialogBtnClose.OnClientClick = "if (parent.fra_main) {parent.fra_main.$('#" + hiddenToValorize + "').val(''); parent.fra_main.__doPostBack('" + hiddenToValorize + "', ''); parent.fra_main.$('#confirm_modal').dialog('close');} else {parent.$('#" + hiddenToValorize + "').val(''); parent.__doPostBack('" + hiddenToValorize + "', ''); parent.$('#confirm_modal').dialog('close');}";
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }
    }
}