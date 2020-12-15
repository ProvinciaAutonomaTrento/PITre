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

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for scegliUoUtente.
	/// </summary>
    public class scegliUoUtente : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Label lbl_cod_rubr;
		protected System.Web.UI.WebControls.ListBox ListaUoUtente;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected System.Web.UI.WebControls.Button btn_annulla;
		protected string querystring;
		protected DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc; 
	
		private void Page_Load(object sender, System.EventArgs e)
		{
            Response.Expires = -1;
			// Put user code to initialize the page here
			Session["isLoaded_ScegliUoUtente"] = true; //true = la pagine ScegliUoUtente è caricata
			Session["retValue_ScegliUoUtente"] = false;//false = il valore ritornato è false finchè non viene premuto annulla
			querystring = Request.QueryString["win"];
			if(!IsPostBack)
			{
				this.lbl_cod_rubr.Text = Request.QueryString["rubr"].ToString();
				this.lbl_cod_rubr.Visible = true;
				/* si prende la scheda documento dalla sessione per popolare la
				* lista di Uo relative all'utente in questione */
				//DocsPaWR.SchedaDocumento schedaDoc= Session["tabDoc.schedaDocumento"] as DocsPAWA.DocsPaWR.SchedaDocumento;
				schedaDoc = DocumentManager.getDocumentoInLavorazione(this);
//				DocsPaWR.SchedaDocumento schedaDoc1 = DocumentManager.getDocumentoSelezionato(this);;
				DocsPaWR.Ruolo[] ruoli = null;
				
				if (schedaDoc.tipoProto.Equals("A"))
				{
					ruoli = ((DocsPAWA.DocsPaWR.Utente)(((DocsPAWA.DocsPaWR.ProtocolloEntrata)(schedaDoc.protocollo)).mittente)).ruoli;
				}
				if (schedaDoc.tipoProto.Equals("P"))
				{
					ruoli = ((DocsPAWA.DocsPaWR.Utente)(((DocsPAWA.DocsPaWR.ProtocolloUscita)(schedaDoc.protocollo)).mittente)).ruoli;
				}
				if (schedaDoc.tipoProto.Equals("I"))
				{
					ruoli = ((DocsPAWA.DocsPaWR.Utente)(((DocsPAWA.DocsPaWR.ProtocolloInterno)(schedaDoc.protocollo)).mittente)).ruoli;
				}
							
				if (schedaDoc != null && ruoli != null)
				{
					for(int i= 0; i< ruoli.Length ; i++) 
					{
						
						ListaUoUtente.Items.Add(ruoli[i].uo.codiceRubrica + " - "+ ruoli[i].uo.descrizione);	
						ListaUoUtente.Items[i].Value=ruoli[i].uo.systemId;
					
					}
				}

				Session.Add("scegliUOUtente.ruoli", ruoli);
				//se vengo da protocollo(mi serve per gestire la chiusura premendo la X)
				
			}

//			if(querystring!=null && querystring=="protocollo")
//			{
//				//registro la funzione che fa il submit del tab protocollo
//				if(!IsStartupScriptRegistered("onunload"))
//					Page.RegisterStartupScript("onunload","<SCRIPT>alert('funzione caricata');function on_unload(){};}</SCRIPT>");
//			}
			//				//se vengo da rubrica
//			if(querystring!=null && querystring=="rubrica")
//			{
//				//registro la funzione che fa il submit del tab protocollo se vengo da rubrica
//				if(!IsStartupScriptRegistered("onunload"))
//					Page.RegisterStartupScript("onunload","<SCRIPT>function on_unload(){ if(window.opener!=null && window.opener.document!=null ){window.opener.parent.opener.document.forms[0].submit();window.parent.opener.parent.close();}}</SCRIPT>");
//				
//			}
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
			this.ListaUoUtente.SelectedIndexChanged += new System.EventHandler(this.ListaUoUtente_SelectedIndexChanged);
			this.btn_annulla.Click += new System.EventHandler(this.btn_annulla_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btn_annulla_Click(object sender, System.EventArgs e)
		{
			/*true = il pulsante annulla è stato premuto*/
			Session["retValue_ScegliUoUtente"] = false;

//			if(querystring!=null && querystring=="protocollo")
//			{
//				//registro la funzione che fa il submit del tab protocollo
//				if(!IsStartupScriptRegistered("onunload"))
//					Page.RegisterStartupScript("onunload","<SCRIPT>function on_unload(){};}</SCRIPT>");
//			}

			//se vengo da rubrica
			if(querystring!=null && querystring=="rubrica")
			{
				//registro la funzione che fa il submit del tab protocollo se vengo da rubrica
				Page.RegisterStartupScript("onunload","<SCRIPT>if(window.opener!=null) {window.opener.parent.opener.document.forms[0].submit();window.parent.opener.parent.close();}</SCRIPT>");
				//Response.Write("<script>window.close();</script>");	
			}
			else
			{

				Response.Write("<script>window.returnValue = false;</script>");	
			}
			Response.Write("<script>window.close();</script>");	
		}


		private void ListaUoUtente_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (ListaUoUtente.SelectedIndex > -1)
			{
				/*true = un elemento della lista è stato premuto */
				Session["retValue_ScegliUoUtente"] = true;
				schedaDoc = DocumentManager.getDocumentoInLavorazione(this);
				DocsPaWR.Corrispondente[] corrRuolo = (DocsPAWA.DocsPaWR.Corrispondente[]) Session["scegliUOUtente.ruoli"];	 
				DocsPaWR.Corrispondente corr = (DocsPAWA.DocsPaWR.Corrispondente)corrRuolo[ListaUoUtente.SelectedIndex];	
				if (schedaDoc.tipoProto.Equals("A"))
				{
					((DocsPAWA.DocsPaWR.ProtocolloEntrata)(schedaDoc.protocollo)).ufficioReferente = ((DocsPAWA.DocsPaWR.Ruolo)corr).uo;
				}
				if (schedaDoc.tipoProto.Equals("P"))
				{
					((DocsPAWA.DocsPaWR.ProtocolloUscita)(schedaDoc.protocollo)).ufficioReferente = ((DocsPAWA.DocsPaWR.Ruolo)corr).uo;
				}
				if (schedaDoc.tipoProto.Equals("I"))
				{
					((DocsPAWA.DocsPaWR.ProtocolloInterno)(schedaDoc.protocollo)).ufficioReferente = ((DocsPAWA.DocsPaWR.Ruolo)corr).uo;
				}
				DocumentManager.setDocumentoInLavorazione(this,schedaDoc);
					
				//Caso in cui vengo dalla RUBRICA VECCHIA (il parametro 'win' in querystring è valorizzato con 'rubrica')
				if(querystring!=null && querystring.Equals("rubrica"))
				{
					Page.RegisterStartupScript("onunload","<SCRIPT>if(window.opener!=null) {window.opener.parent.opener.document.forms[0].submit();window.parent.opener.parent.close();}</SCRIPT>");
				}
				else
				{
					Response.Write("<script>window.returnValue = true;</script>");	
				}
				Session.Remove("scegliUOUtente.ruoli");
				Response.Write("<script>window.close();</script>");
			}
		}

	}
}
