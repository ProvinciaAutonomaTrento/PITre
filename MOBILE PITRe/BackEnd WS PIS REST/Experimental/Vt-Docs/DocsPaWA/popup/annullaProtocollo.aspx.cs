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
using DocsPAWA.DocsPaWR;
using DocsPAWA.CheckInOut;
using DocsPAWA.utils;

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for annullaProtocollo.
	/// </summary>
    public class annullaProtocollo : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.Label Label;
		protected System.Web.UI.WebControls.Label LabelCodice;
		protected System.Web.UI.WebControls.Label lbl_messageCheckOut;
		protected System.Web.UI.WebControls.Label lbl_messageOwnerCheckOut;
		protected System.Web.UI.HtmlControls.HtmlInputHidden deleteConfirmed;
		protected System.Web.UI.WebControls.TextBox txt_note_annullamento;
	
		private void Page_Load(object sender, System.EventArgs e)
		{	
			Response.Expires=-1;

			this.SetDefaultButton();

			if (!this.IsPostBack)
			{
				this.txt_note_annullamento.Attributes.Add("onChange","AbilitaBtn();");
				this.btn_ok.Attributes.Add("onClick","ConfirmCancelProtocollo();");

				DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato(this);

                string ownerUser;

				// Verifica se il documento è in stato checkout
                if (CheckInOutServices.IsCheckedOutDocumentWithUser(out ownerUser))
				{
					this.IsOwnerCheckedOut=(ownerUser == UserManager.getInfoUtente().userId);

					this.lbl_messageCheckOut.Visible=!this.IsOwnerCheckedOut;
					this.lbl_messageOwnerCheckOut.Visible=this.IsOwnerCheckedOut;
					
					this.LabelCodice.Visible=this.IsOwnerCheckedOut;
					this.txt_note_annullamento.Visible=this.IsOwnerCheckedOut;
					this.btn_ok.Visible=this.IsOwnerCheckedOut;
				}
				else
				{
					this.lbl_messageCheckOut.Visible=false;
					this.lbl_messageOwnerCheckOut.Visible=false;
				}
			}

			this.btn_ok.Enabled=(this.txt_note_annullamento.Text.Length>0);
		}

		/// <summary>
		/// 
		/// </summary>
		private bool IsOwnerCheckedOut
		{
			get
			{
				if (this.ViewState["IsOwnerCheckedOut"]==null)
					return false;
				else
					return Convert.ToBoolean(this.ViewState["IsOwnerCheckedOut"]);
			}
			set
			{
				this.ViewState["IsOwnerCheckedOut"]=value;
			}
		}

		
		/// <summary>
		/// Azione di annullamento del protocollo
		/// </summary>
		private void PerformActionAnnullaProtocollo()
		{
			DocsPaWR.SchedaDocumento schedaDoc=DocumentManager.annullaProtocollo(this,this.txt_note_annullamento.Text);
		}

		private void PerformActionNotificaAnnullamento()
		{
            DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
            DocsPAWA.DocsPaWR.Registro reg;
            //se il protocollo è in entrata ed è stato ricevuto per interoperabilità, allora invio una notifica di annullamento al mittente
            if (schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloEntrata))
            {
                if (!string.IsNullOrEmpty(schedaDocumento.documento_da_pec) || schedaDocumento.typeId == InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId)
                {
                    DocsPaWebService ws = new DocsPaWebService();
                    string idRegistro = (schedaDocumento.protocollo as DocsPAWA.DocsPaWR.ProtocolloEntrata).mittente.idRegistro; 
                    if (!string.IsNullOrEmpty(idRegistro))
                        reg =  ws.GetRegistroBySistemId(idRegistro);
                    else
                        reg = ws.GetRegistroBySistemId(schedaDocumento.registro.systemId);
                    DocumentManager.DocumentoInvioNotificaAnnulla(this, schedaDocumento.systemId, reg);
                }
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

		/// <summary>
		/// Impostazione pulsante di default
		/// </summary>
		public void SetDefaultButton()
		{
			Utils.DefaultButton(this, ref txt_note_annullamento, ref btn_ok);
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
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btn_ok_Click(object sender, System.EventArgs e)
		{
			if (this.deleteConfirmed.Value=="true")
			{
				this.PerformActionAnnullaProtocollo();
				this.PerformActionNotificaAnnullamento();
				
				this.deleteConfirmed.Value="false";

				this.RegisterClientScript("Close","ClosePage(true);");
			}
		}

		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{
			this.RegisterClientScript("Close","ClosePage(false);");
		}
	}
}