using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;
using NttDataWA.CheckInOut;


namespace NttDataWA.Popup
{
    public partial class RemoveProfile : System.Web.UI.Page
    {

        protected string result;


        private SchedaDocumento DocumentInWorking
        {
            get
            {
                SchedaDocumento result = null;
                if (HttpContext.Current.Session["document"] != null)
                {
                    result = HttpContext.Current.Session["document"] as SchedaDocumento;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["document"] = value;
            }
        }

        private bool IsOwnerCheckedOut
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsOwnerCheckedOut"] != null)
                    result = (bool)HttpContext.Current.Session["IsOwnerCheckedOut"];

                return result;
            }
            set
            {
                HttpContext.Current.Session["IsOwnerCheckedOut"] = value;
            }
        }

        private int MaxLenghtNote
        {
            get
            {
                return 256;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                this.TxtNote.Focus();

                if (!IsPostBack)
                {
                    this.InitPage();

                    string ownerUser;
                    bool isCheckedOut = CheckInOutServices.IsCheckedOutDocumentWithUser(out ownerUser);

                    // Verifica se il documento è in stato checkout
                    if (isCheckedOut)
                    {
                        this.IsOwnerCheckedOut = (ownerUser.ToUpper() == UserManager.GetInfoUser().userId.ToUpper());

                        this.plcMessageCheckOut.Visible = true;
                        this.lbl_messageCheckOut.Visible = !this.IsOwnerCheckedOut;
                        this.lbl_messageOwnerCheckOut.Visible = this.IsOwnerCheckedOut;

                        this.BtnOk.Visible = this.IsOwnerCheckedOut;
                        this.plcNote.Visible = false;
                        this.lbl_result.Text = string.Empty;
                    }
                    else
                    {
                        this.plcMessageCheckOut.Visible = false;

                        result = DocumentManager.verificaDirittiCestinaDocumento(this, this.DocumentInWorking);

                        bool visibility = (result == "Del");
                        this.plcNote.Visible = visibility;
                        this.BtnOk.Visible = visibility;
                        this.plcMessageConfirm.Visible = visibility;

                        if (!visibility) {
                            this.plcMessageResult.Visible = true;
                            this.lbl_result.Text = result;
                        }
                    }
                }

                this.RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitPage()
        {
            this.InitLanguage();
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnOk.Text = Utils.Languages.GetLabelFromCode("GenericBtnOk", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.lbl_messageCheckOut.Text = Utils.Languages.GetLabelFromCode("RemoveProfileCheckout", language);
            this.lbl_messageOwnerCheckOut.Text = Utils.Languages.GetLabelFromCode("RemoveProfileOwnerCheckout", language);
            this.lbl_mess_conf_rimuovi.Text = Utils.Languages.GetLabelFromCode("RemoveProfileConfirm", language);
            this.litNotesChars.Text = Utils.Languages.GetLabelFromCode("DocumentLitVisibleNotesChars", language);
        }

        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshNoteChars", "charsLeft('TxtNote', " + this.MaxLenghtNote + ", '" + this.litNotesChars.Text.Replace("'", "\'") + "');", true);
            this.TxtNote_chars.Attributes["rel"] = "TxtNote_" + this.MaxLenghtNote + "_" + this.litNotesChars.Text;
        }

        protected void BtnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.checkDati())
                {
                    string errorMsg = string.Empty;
                    bool cestina = DocumentManager.CestinaDocumento(this, this.DocumentInWorking, "P", TxtNote.Text, out errorMsg);

                    if (cestina)
                    {
                        this.DocumentInWorking.inCestino = "1";
                        this.CloseMask(cestina);
                    }
                    else
                    {
                        this.lbl_result.Text = errorMsg;
                        this.plcMessageConfirm.Visible = false;
                        this.plcNote.Visible = false;
                        this.BtnOk.Visible = false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
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
            string retValue = string.Empty;
            if (withReturnValue) retValue = "true";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('RemoveProfile', '"+retValue+"');} else {parent.closeAjaxModal('RemoveProfile', '"+retValue+"');};", true);
        }

        private bool checkDati()
        {
            if (this.TxtNote.Text.Length==0)
            {
                string msgDesc = "WarningDocumentRemoveNoteObligatory";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);

                return false;
            }
            else if (this.TxtNote.Text.Length > this.MaxLenghtNote)
            {
                string msgDesc = "WarningDocumentRemoveNoteMaxLength";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);

                return false;
            }
            return true;
        }

    }
}