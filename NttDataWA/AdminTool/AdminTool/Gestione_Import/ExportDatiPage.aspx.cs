using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAAdminTool.exportDati;
using SAAdminTool.SiteNavigation;

namespace SAAdminTool.AdminTool.Gestione_Import
{
    public partial class ExportDatiPage : System.Web.UI.Page
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {

                if (this.FileDocumento != null && this.FileDocumento.content.Length > 0)
                {
                    Response.ContentType = this.FileDocumento.contentType;
                    Response.AddHeader("content-disposition", "inline;filename=exportDocspa.xls");
                    Response.AddHeader("content-lenght", this.FileDocumento.content.Length.ToString());
                    Response.BinaryWrite(this.FileDocumento.content);
                    this.FileDocumento = null;
                }
                else
                {
                    this.executeJS("<SCRIPT>alert('Impossibile generare il documento di esportazione dei dati.\nContattare l'amministratore di sistema.');self.close();</SCRIPT>");
                }
            }
            catch
            {
                this.executeJS("<SCRIPT>alert('Impossibile generare il documento di esportazione dei dati.\nContattare l'amministratore di sistema.');self.close();</SCRIPT>");
            }

        }

        private void executeJS(string key)
        {
            if (!this.Page.IsStartupScriptRegistered("theJS"))
                this.Page.RegisterStartupScript("theJS", key);
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion

        /// <summary>
        /// File Excel
        /// </summary>
        public DocsPaWR.FileDocumento FileDocumento
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["fileDocumento"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["fileDocumento"] as DocsPaWR.FileDocumento;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["fileDocumento"] = value;
            }
        }
    }
}