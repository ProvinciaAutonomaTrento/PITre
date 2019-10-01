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
    public partial class ProjectConfirmDeleteNode : System.Web.UI.Page
    {

        private DocsPaWR.Folder folder
        {
            get
            {
                return Session["remove_node_folder"] as DocsPaWR.Folder;
            }
            set {
                Session["remove_node_folder"] = value;
            }
        }

        private Fascicolo Fasc
        {
            get
            {
                return ProjectManager.getProjectInSession();
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitLanguage();
                
                    if (this.folder!=null)
                        this.message.Text += "<br /><strong>"+this.folder.descrizione+"</strong>";
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
            this.BtnDelete.Text = Utils.Languages.GetLabelFromCode("GenericBtnDelete", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.message.Text = Utils.Languages.GetLabelFromCode("ProjectConfirmDeleteSubset", language);
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
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "parent.closeAjaxModal('ConfirmDeleteNode', '" + returnValue + "', parent);", true);
        }

        protected void BtnDelete_Click(object sender, EventArgs e)
        {
            if (!ProjectManager.CheckRevocationAcl())
            {
                try
                {
                    string nFasc = "";

                    if (this.CheckIfRootFolder(this.folder))
                    {
                        if (Fasc.tipo.Equals("P"))
                        {
                            string msg = "WarningProjectImpossibleDeleteProcedural";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '', '" + utils.FormatJs(Fasc.codice) + "');", true);
                        }
                        if (Fasc.tipo.Equals("G"))
                        {
                            string msg = "WarningProjectImpossibleDeleteGeneral";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '', '" + utils.FormatJs(Fasc.codice) + "');", true);
                        } return;
                    }
                    if (this.folder != null)
                    {
                        /* Se il folder selezionato ha figli (doc o sottocartelle) su cui HO visibilità 
                         * non deve essere rimosso. Dopo l'avviso all'utente, la procedura termina */
                        if (this.folder.childs.Length > 0)
                        {
                            string msg = "WarningProjectImpossibleDeleteHasChildren";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        }
                        else
                        {
                            /* Se il folder selezionato ha figli (doc o sottocartelle) su cui NON HO 
                             * la visibilità non deve essere rimosso */
                            //CanRemoveFascicolo ritornerà un bool: true = posso rimuovere il folder, false altrimenti
                            if (!ProjectManager.CanRemoveFascicolo(this, this.folder.systemID, out nFasc))
                            {
                                if (nFasc.Equals("0") || nFasc.Equals(""))
                                {
                                    string msg = "WarningProjectImpossibleDeleteContainsDocuments";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                                }
                                else
                                {
                                    string msg = "WarningProjectImpossibleDeleteHasChildren";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                                }
                            }
                            else
                            {
                                ProjectManager.delFolder(this, this.folder);
                                this.CloseMask(true);
                            }
                        }
                    }

                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }

        private bool CheckIfRootFolder(DocsPaWR.Folder folder)
        {
            Fascicolo Fasc = ProjectManager.getProjectInSession();
            DocsPaWR.Folder rootFolder = ProjectManager.getFolder(this, Fasc);
            if (folder.systemID == rootFolder.systemID) return true;
            return false;
        }

    }
}