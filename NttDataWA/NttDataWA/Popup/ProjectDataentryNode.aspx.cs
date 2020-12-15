using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;


namespace NttDataWA.Popup
{
    public partial class ProjectDataentryNode : System.Web.UI.Page
    {
        private int maxLengthDescription = 1985;

        private DocsPaWR.Folder folder
        {
            get
            {
                return Session["dataentry_node_folder"] as DocsPaWR.Folder;
            }
            set
            {
                Session["dataentry_node_folder"] = value;
            }
        }

        private Fascicolo Fasc
        {
            get
            {
                return ProjectManager.getProjectInSession();
            }
        }

        private DocsPaWR.Fascicolo Project
        {
            get
            {
                Fascicolo result = null;
                if (HttpContext.Current.Session["searchproject"] != null)
                {
                    result = HttpContext.Current.Session["searchproject"] as Fascicolo;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["searchproject"] = value;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitLanguage();

                    if (Request.QueryString != null && Request.QueryString["t"] == "modify" && this.folder!=null)
                    {
                        this.FolderDescription.Text = this.folder.descrizione;
                    }
                }

                this.RefreshScripts();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void RefreshScripts()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshDescriptionChars", "charsLeft('FolderDescription', " + this.maxLengthDescription + ", '" + this.lbl_description.Text.Replace("'", "\'") + "');", true);
            this.FolderDescription_chars.Attributes["rel"] = "TxtNote_" + this.maxLengthDescription + "_" + this.lbl_description.Text;
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnSave.Text = Utils.Languages.GetLabelFromCode("GenericBtnSave", language);
            if (Request.QueryString != null && Request.QueryString["t"] == "modify") this.BtnSave.Text = Utils.Languages.GetLabelFromCode("GenericBtnModify", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnCancel", language);
            this.lbl_description.Text = Utils.Languages.GetLabelFromCode("ProjectFolderDescription", language);
            this.FolderDescriptionChars.Text = Utils.Languages.GetLabelFromCode("DocumentLitVisibleNotesChars", language);
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                this.CloseMask(false);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ClearSessionData()
        {
            this.folder = null;
        }

        protected void CloseMask(bool withReturnValue)
        {
            string returnValue = string.Empty;
            if (withReturnValue) returnValue = "true";
            this.ClearSessionData();

            string popupId = "CreateNode";
            if (Request.QueryString != null && Request.QueryString["t"] == "modify") popupId = "ModifyNode";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "parent.closeAjaxModal('"+popupId+"', '" + returnValue + "', parent);", true);
        }

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                if (!string.IsNullOrEmpty(this.FolderDescription.Text) && !string.IsNullOrEmpty(this.FolderDescription.Text.Trim()))
                {
                    if (Request.QueryString != null && Request.QueryString["t"] == "modify")
                    {
                        // MODIFICA
                        string descr = this.FolderDescription.Text;
                        if (descr.Length > this.maxLengthDescription)
                            descr = descr.Substring(0, maxLengthDescription - 1);
                        folder.descrizione = this.FolderDescription.Text;
                        ProjectManager.updateFolder(this, folder);
                        this.CloseMask(true);
                    }
                    else
                    {
                        // INSERIMENTO
                        DocsPaWR.ResultCreazioneFolder result;
                        if (!this.CreateNewFolder(folder, out result))
                        {
                            // Visualizzazione messaggio di errore
                            string errorMessage = string.Empty;
                            if (result == DocsPaWR.ResultCreazioneFolder.FOLDER_EXIST)
                            {
                                string msg = "WarningProjectDuplicateSubset";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning');", true);
                            }
                            else
                            {
                                string msg = "WarningProjectCreateError";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning');", true);
                            }
                        }
                        else
                        {
                            this.CloseMask(true);
                        }
                    }
                }
                else
                {
                    this.rowMessage.InnerHtml = this.GetLabel("ProjectDataentryFolderDescriptionObligatory");
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private bool CreateNewFolder(DocsPaWR.Folder folderSelected, out DocsPaWR.ResultCreazioneFolder result)
        {
            bool retValue = false;
            result = DocsPaWR.ResultCreazioneFolder.GENERIC_ERROR;

            try
            {
                if (folderSelected != null)
                {
                    DocsPaWR.Folder newFolder = new DocsPaWR.Folder();

                    newFolder.idFascicolo = folderSelected.idFascicolo;
                    newFolder.idParent = folderSelected.systemID;
                    newFolder.descrizione = this.FolderDescription.Text;

                    ProjectManager.newFolder(this, ref newFolder, UserManager.GetInfoUser(), RoleManager.GetRoleInSession(), out result);
                    retValue = (result == DocsPaWR.ResultCreazioneFolder.OK);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            return retValue;
        }

    }
}