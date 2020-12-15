using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDatalLibrary;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class VisibilityRemove : System.Web.UI.Page
    {

        #region Properties

        protected GridViewRow RowSelected
        {
            get
            {
                return HttpContext.Current.Session["RowSelected"] as GridViewRow;
            }
            set
            {
                HttpContext.Current.Session["RowSelected"] = value;
            }
        }

        protected DocumentoDiritto[] VisibilityList
        {
            get
            {
                DocumentoDiritto[] result = null;
                if (HttpContext.Current.Session["visibilityList"] != null)
                {
                    result = HttpContext.Current.Session["visibilityList"] as DocumentoDiritto[];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["visibilityList"] = value;
            }
        }

        private string isPersonOrGroup
        {
            get
            {
                return HttpContext.Current.Session["isPersonOrGroup"] as string;
            }
            set
            {
                HttpContext.Current.Session["isPersonOrGroup"] = value;
            }
        }

        protected SchedaDocumento documentWIP
        {
            get
            {
                return HttpContext.Current.Session["Visibility_document"] as SchedaDocumento;
            }
            set
            {
                HttpContext.Current.Session["Visibility_document"] = value;
            }
        }

        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitLanguage();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnOk.Text = Utils.Languages.GetLabelFromCode("GenericBtnOk", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.litNotes.Text = Utils.Languages.GetLabelFromCode("VisibilityNotes", language);
            if (this.TypeObject != "D")
                this.messager.Text = Utils.Languages.GetMessageFromCode("VisibilityRemoveWarningFasc", language);
            else
                this.messager.Text = Utils.Languages.GetMessageFromCode("VisibilityRemoveWarning", language);
        }

        protected void BtnOk_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                if (this.RemoveACL())
                    this.CloseMask(true);
                else
                    this.message.Text = Utils.Languages.GetMessageFromCode("VisibilityRemoveError", UIManager.UserManager.GetUserLanguage());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            try {
                this.CloseMask(false);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void CloseMask(bool withReturnValue)
        {
            string returnValue = "";
            if (withReturnValue) returnValue = "true";

            if (this.documentWIP==null)
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('VisibilityRemove', '" + returnValue + "');} else {parent.closeAjaxModal('VisibilityRemove', '" + returnValue + "');};", true);
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "parent.closeAjaxModal('VisibilityRemove', '" + returnValue + "', parent);", true);
        }

        protected bool RemoveACL()
        {
            // Verify if remove ACL at user or at role
            DocumentoDiritto[] ListDocDir = this.VisibilityList;
            DocsPaWR.DocumentoDiritto docDiritti = ListDocDir[this.RowSelected.DataItemIndex];
            docDiritti.personorgroup = this.isPersonOrGroup;
            docDiritti.note = this.txtNote.Text;

            return DocumentManager.RemoveACL(docDiritti, this.isPersonOrGroup, UserManager.GetInfoUser(), this.TypeObject);
        }

        public string TypeObject
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["typeObject"] != null)
                {
                    result = HttpContext.Current.Session["typeObject"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["typeObject"] = value;
            }
        }

    }
}