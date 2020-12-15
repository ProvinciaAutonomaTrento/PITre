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

namespace Amministrazione.Gestione_Organigramma
{
	/// <summary>
	/// Summary description for Sposta_UO.
	/// </summary>
	public class Sposta_UO : System.Web.UI.Page
	{
		#region WebControls e variabili
		protected System.Web.UI.WebControls.Label lbl_intest1;
		protected System.Web.UI.WebControls.Label lbl_intest2;
		protected System.Web.UI.WebControls.Label lbl_intest3;
		protected System.Web.UI.WebControls.Button btn_sposta;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_livelloUO_DaSpostare;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idCorrGlobUODaSpostare;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_descUODaSpostare;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_codUODaSpostare;		
		protected System.Web.UI.WebControls.Label lbl_uo;
		protected System.Web.UI.WebControls.TextBox txt_codice;
		protected System.Web.UI.WebControls.TextBox txt_descrizione;
		//-------------------------------------------------------------------
		private string _idAmm = string.Empty;
		private string _livelloUO_DaSpostare = string.Empty;		
		private string _idCorrGlobUODaSpostare = string.Empty;
		private string _codUODaSpostare = string.Empty;
		private string _descUODaSpostare = string.Empty;
		protected System.Web.UI.WebControls.TextBox txt_ricCod;
		protected System.Web.UI.WebControls.TextBox txt_ricDesc;
		protected System.Web.UI.WebControls.ImageButton btn_org;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModal;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idAmm;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idCorrGlobDest;				
		#endregion
	
		/// <summary>
		/// Page Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{		
			Response.Expires = -1;

			//----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
			if(Session.IsNewSession)
			{
				Response.Redirect("../Exit.aspx?FROM=EXPIRED");
			}
			
			AmmUtils.WebServiceLink ws2 =new AmmUtils.WebServiceLink();
			if(!ws2.CheckSession(Session.SessionID))
			{
				Response.Redirect("../Exit.aspx?FROM=ABORT");
			}
			// ---------------------------------------------------------------
			
			if (!IsPostBack)
				this.Inizialize();
			else
			{
				// gestione del valore di ritorno della modal Dialog (ricerca)
				if(this.hd_returnValueModal.Value != null && this.hd_returnValueModal.Value != string.Empty && this.hd_returnValueModal.Value != "undefined")
				{
					string[] appo = this.hd_returnValueModal.Value.Split('|');
					
					this.txt_ricCod.Text = appo[0];
					this.txt_ricDesc.Text = appo[1];
					this.hd_idCorrGlobDest.Value = appo[2];							
			
					this.hd_returnValueModal.Value = string.Empty;
				}
			}
		}

		private void Inizialize()
		{			
			//reperisce i dati dal chiamante
			this.GetDataQueryString();			

			//visualizza ed imposta dati sulla GUI
			this.lbl_uo.Text	= "<b>" + this._descUODaSpostare + "</b> (livello: "+ this._livelloUO_DaSpostare +")";								

			this.btn_org.Attributes.Add("onclick","ApriOrganigramma();");
	
			//riepilogo			
			this.txt_codice.Text = this.hd_codUODaSpostare.Value;
			this.txt_descrizione.Text = this.hd_descUODaSpostare.Value;
		}

		private void GetDataQueryString()
		{
			//reperisce i dati dal chiamante			
			this._idCorrGlobUODaSpostare = Request.QueryString["idCorrGlobUODaSpostare"].ToString();
			this.hd_idCorrGlobUODaSpostare.Value=this._idCorrGlobUODaSpostare;

			this._livelloUO_DaSpostare = Request.QueryString["livelloUO_DaSpostare"].ToString();
			this.hd_livelloUO_DaSpostare.Value = this._livelloUO_DaSpostare;

			this._descUODaSpostare = Server.UrlDecode(Request.QueryString["descUODaSpostare"].ToString());			
			this.hd_descUODaSpostare.Value = this._descUODaSpostare;

			this._codUODaSpostare = Request.QueryString["codUODaSpostare"].ToString();
			this.hd_codUODaSpostare.Value = this._codUODaSpostare;
			
			this._idAmm = Request.QueryString["idAmm"].ToString();
			this.hd_idAmm.Value = this._idAmm;
		}		

		private void executeJS(string key)
		{
			if(!this.Page.IsStartupScriptRegistered("theJS"))			
				this.Page.RegisterStartupScript("theJS", key);
		}

		private void SetFocus(System.Web.UI.Control ctrl, bool sel)
		{
			DocsPAWA.Utils.SetFocus(this,ctrl,sel);			
		}		

		private void btn_sposta_Click(object sender, System.EventArgs e)
		{
			DocsPAWA.DocsPaWR.OrgUO uoDaSpostare = new DocsPAWA.DocsPaWR.OrgUO();
			uoDaSpostare.IDCorrGlobale = this.hd_idCorrGlobUODaSpostare.Value;
			uoDaSpostare.Codice = this.txt_codice.Text;
			uoDaSpostare.CodiceRubrica = this.txt_codice.Text;
			uoDaSpostare.Descrizione = this.txt_descrizione.Text;
			uoDaSpostare.IDAmministrazione = this.hd_idAmm.Value;
			uoDaSpostare.Livello = this.hd_livelloUO_DaSpostare.Value;

			DocsPAWA.DocsPaWR.OrgUO uoPadre = new DocsPAWA.DocsPaWR.OrgUO();
			uoPadre.IDCorrGlobale = this.hd_idCorrGlobDest.Value;
			uoPadre.Codice = this.txt_ricCod.Text;
			uoPadre.CodiceRubrica = this.txt_ricCod.Text;
			uoPadre.Descrizione = this.txt_ricDesc.Text;
			uoPadre.IDAmministrazione = this.hd_idAmm.Value;			

			Manager.OrganigrammaManager manager = new Amministrazione.Manager.OrganigrammaManager();
			DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();

			//Se la Uo si sta spostando in una dei suio figli si deve bloccare l'operazione
			//e avvisare l'utente
			ArrayList uoFiglie = cercaInUoFiglie(uoDaSpostare);

			/*verifico se la systemId della UoPadre 
			(intesa come il nodo sotto il quale si vuole spostare la Uo 'uoDaSpostare')
			è presente tra i figli della 'uoDaSpostare', in tal caso blocchiamo tale operazione
			Attualmente non è gestito il caso di sposare un NODO ('uoPadre') sotto a un suo FIGLIO*/
			if(uoPadre.IDCorrGlobale!=uoDaSpostare.IDCorrGlobale)
			{
				if(!uoFiglie.Contains(uoPadre.IDCorrGlobale))
				{

					manager.SpostaUO(uoDaSpostare,uoPadre);
					esito = manager.getEsitoOperazione();

					if(esito.Codice.Equals(0))
					{									
						this.executeJS("<SCRIPT>alert('Operazione di spostamento UO eseguita correttamente.'); window.returnValue = 'Y'; self.close();</SCRIPT>");
					}
					else
						this.executeJS("<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'","\\'") + "');</SCRIPT>");
		
				}	
				else
				{
					//caso in cui si tenta si spostare UN PADRE SOTTO A UN FIGLIO
					this.executeJS("<SCRIPT>alert('Attenzione: non è possibile spostare un nodo padre sotto a un nodo figlio.'); window.returnValue = 'Y'; self.close();</SCRIPT>");
				}
			}
			else
			{
				//caso in cui si tenta si spostare un nodo sotto se stesso (senza questo controllo l'applicazione va in loop)
				this.executeJS("<SCRIPT>alert('Attenzione: si sta cercando di spostare una Uo sotto se stessa.'); window.returnValue = 'Y'; self.close();</SCRIPT>");
				
			}
		}

		private ArrayList cercaInUoFiglie(DocsPAWA.DocsPaWR.OrgUO uo)
		{
			//CALCOLARE I FIGLI DEL NODO DA SPOSTARE sysUoFrom
			ArrayList listaUo = null;
			Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
			listaUo = theManager.GetListaUoFiglie(uo);
			return listaUo;
			
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
			this.txt_ricCod.TextChanged += new System.EventHandler(this.txt_ricCod_TextChanged);
			this.btn_sposta.Click += new System.EventHandler(this.btn_sposta_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion		

		private void txt_ricCod_TextChanged(object sender, System.EventArgs e)
		{
			try
			{				
				Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
			
				theManager.RicercaInOrg("U",this.txt_ricCod.Text.Trim(),"",this.hd_idAmm.Value, false, true);

				if(theManager.getRisultatoRicerca()!=null && theManager.getRisultatoRicerca().Count.Equals(1))
				{							
					foreach(DocsPAWA.DocsPaWR.OrgRisultatoRicerca risultato in theManager.getRisultatoRicerca())
					{
						this.txt_ricDesc.Text = risultato.Descrizione;
						this.hd_idCorrGlobDest.Value = risultato.IDCorrGlob;	

						this.SetFocus(this.txt_codice,false);

						DocsPAWA.Utils.DefaultButton(this, ref txt_codice, ref btn_sposta);
						DocsPAWA.Utils.DefaultButton(this, ref txt_descrizione, ref btn_sposta);
					}
				}
				else
				{
					this.executeJS("<SCRIPT>alert('Nessuna UO trovata');</SCRIPT>");				
					this.SetFocus(this.txt_ricCod,true);
					this.txt_ricDesc.Text="";				
				}
				
			}
			catch
			{
				this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema');</SCRIPT>");
			}
		}
	}
}
