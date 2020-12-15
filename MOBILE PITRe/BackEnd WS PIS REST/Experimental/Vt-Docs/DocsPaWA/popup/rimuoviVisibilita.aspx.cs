using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace DocsPAWA.popup
{
    public partial class rimuoviVisibilita : DocsPAWA.CssPage
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            Response.Expires = -1;

            if (!this.IsPostBack)
            {
                this.btn_annulla.Attributes["onClick"] = "ClosePage(false);";

            }
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
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            this.btn_annulla.Click += new System.EventHandler(this.btn_annulla_Click);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        private void btn_ok_Click(object sender, System.EventArgs e)
        {
            RimuoviACL();
        }

        private void btn_annulla_Click(object sender, System.EventArgs e)
        {
            string scriptString = "<SCRIPT>window.close(); ApriFinestraVisibilita(); </SCRIPT>";
            this.RegisterStartupScript("apriModalDialogVisibilita", scriptString);
            
        }
        
        //Metodo per la rimozione del ACL
        private void RimuoviACL()
        {
            try
            {
                DocsPAWA.DocsPaWR.DocumentoDiritto docDiritto = (DocsPAWA.DocsPaWR.DocumentoDiritto)Session["DOCDIRITTO"];
                if (docDiritto != null)
                {
                    docDiritto.note = txt_note.Text;
                    bool result = DocumentManager.editingACL(docDiritto, docDiritto.personorgroup, UserManager.getInfoUtente(this));
                    if (result)
                    {

                        Session.Remove("DOCDIRITTO");
                        //Page.RegisterStartupScript("Chiudi", "<script>ClosePage(false);</script>");
                        //Response.Write("<script>window.opener.parent.location.href='visibilitaDocumento.aspx'; window.close();</script>");
                  
                        //Response.Write("<script>window.location.href = 'visibilitaDocumento.aspx'; window.close();</script>");
                        string scriptString = "<SCRIPT>window.close(); ApriFinestraVisibilita(); </SCRIPT>";
                        this.RegisterStartupScript("apriModalDialogVisibilita", scriptString);
            
                    }
                    else
                    {
                        lbl_result.Text = "Attenzione operazione non riuscita";
                    }
                }
                else
                {
                    lbl_result.Text = "Attenzione operazione non riuscita";
                }
                
                
               

            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }
       
    }
}
