using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using DocsPAWA;

namespace DocsPAWA.ImportMassivoDoc
{
    public partial class importaDoc : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.DropDownList cboFileTypes;
        protected System.Web.UI.WebControls.Label lblFileType;
        
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
                if (Session["fascDocumenti.FolderSel"] != null)
                {
                    DocsPaWR.Folder folder = (DocsPaWR.Folder)Session["fascDocumenti.FolderSel"];
                    idFolder = folder.systemID;
                    DocsPAWA.DocsPaWR.Fascicolo Fasc = FascicoliManager.getFascicoloSelezionato(this);
                    if(Fasc!=null)
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
