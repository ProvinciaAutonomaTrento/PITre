using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;

namespace NttDataWA.Project.ImportExport
{
    public partial class ImportDocumentActiveX : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            if (!this.IsPostBack)
            {
                Session["Old_SelectedNodeIndex"] = Session["SelectedNodeIndex"];
                //if (Request.QueryString["codFasc"] != null && Request.QueryString["codFasc"] != "")
                //{
                string idFolder = "";
                string idFascicolo = "";


                if (UIManager.ProjectManager.getProjectInSession().folderSelezionato != null)
                {
                    DocsPaWR.Folder folder = UIManager.ProjectManager.getProjectInSession().folderSelezionato;
                    idFolder = folder.systemID;
                    NttDataWA.DocsPaWR.Fascicolo Fasc = NttDataWA.UIManager.ProjectManager.getProjectInSession();
                    if (Fasc != null)
                        codFasc.Value = Fasc.codice;
                }
                //}
            }

            if (!string.IsNullOrEmpty(this.hdMetaFileContent.Value))
            {
                this.massiveImportDocumenti.SetMetaFileContent(this.hdMetaFileContent.Value);

                this.hdMetaFileContent.Value = string.Empty;
            }
        }

        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            this.SetControlFocus();
        }

        private void InitializeComponent()
        {
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new EventHandler(this.Page_PreRender);
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
        }

        private void SetControlFocus()
        {
            if (this.txtFirstInvalidControlID.Value != string.Empty)
            {
                this.RegisterClientScript("SetControlFocus", "SetControlFocus('" + this.txtFirstInvalidControlID.Value + "');");

                this.txtFirstInvalidControlID.Value = string.Empty;
            }
        }



        private void InitializeClientScript()
        {
            this.btnBrowseForFolder.Attributes.Add("onClick", "PerformSelectFolder()");
            this.btnCancel.Attributes.Add("onClick", "ClosePage(false);");
        }
    }
}