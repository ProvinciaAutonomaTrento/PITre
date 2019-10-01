using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DocsPAWA.CheckInOut;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for rimuoviVersione.
	/// </summary>
    public class rimuoviVersione : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Label lbl_message;
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.Button btn_annulla;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Label lbl_messageCheckOut;
		protected System.Web.UI.WebControls.Label lbl_messageOwnerCheckOut;
		protected System.Web.UI.WebControls.Label lbl_result;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires=-1;

			if (!this.IsPostBack)
			{
				this.btn_annulla.Attributes["onClick"]="ClosePage(false);";

				DocsPaWR.SchedaDocumento schedaDocumento=DocumentManager.getDocumentoSelezionato(this);

                string ownerUser;

				// Verifica se il documento è in stato checkout
                if (CheckInOutServices.IsCheckedOutDocumentWithUser(out ownerUser))
				{
                    bool isOwnerCheckedOut = (ownerUser.ToUpper() == UserManager.getInfoUtente().userId.ToUpper());

					this.lbl_messageOwnerCheckOut.Visible=isOwnerCheckedOut;
					this.lbl_messageCheckOut.Visible=!isOwnerCheckedOut;
					this.lbl_message.Visible=false;
					this.btn_ok.Visible=isOwnerCheckedOut;

					if (isOwnerCheckedOut)
						this.btn_ok.Attributes.Add("onClick","RemoveCheckOutVersion();");
				}
				else
				{
					this.lbl_messageOwnerCheckOut.Visible=false;
					this.lbl_messageCheckOut.Visible=false;
					this.lbl_message.Visible=true;
				}

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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion


		private void btn_ok_Click(object sender, System.EventArgs e)
		{
			try
			{
				int indexSel = Int32.Parse(Request.QueryString["versioneSelezionata"]);
				
				if (indexSel >= 0)
				{	
					DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
					
					//if(schedaDocumento.documenti[indexSel].version!="1")
                    if (schedaDocumento.documenti.Length > 1 && indexSel != 0)
					{
                        if (DocumentManager.rimuoviVersione(this, schedaDocumento.documenti[indexSel], schedaDocumento))
                        {
                            schedaDocumento.documenti = DocumentManager.rimuoviDaListaVersioni(schedaDocumento.documenti, indexSel);
                            DocumentManager.setDocumentoSelezionato(this, schedaDocumento);

                            //pulisco anche la parte di destra
                            FileManager.removeSelectedFile(this);
                        }
					}
				}

				this.RegisterClientScript("ClosePage","ClosePage(true);");
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}		
		}

		/// <summary>
		/// Registrazione script client
		/// </summary>
		/// <param name="scriptKey"></param>
		/// <param name="scriptValue"></param>
		private void RegisterClientScript(string scriptKey,string scriptValue)
		{
			if(!this.Page.IsStartupScriptRegistered(scriptKey))
			{
				string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
				this.Page.RegisterStartupScript(scriptKey, scriptString);
			}
		}
	}
}