using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Xml;
using Microsoft.Web.UI.WebControls;
using DocsPaWR = DocsPAWA.DocsPaWR;
using System.Linq;

namespace Amministrazione.Gestione_Organigramma
{
	/// <summary>
	/// Summary description for sposta ruolo tra UO
	/// </summary>
	public class Sposta_Ruolo: System.Web.UI.Page
	{
		#region WebControls e variabili

		protected System.Web.UI.WebControls.Button btn_sposta;
		protected System.Web.UI.WebControls.Label lbl_ruolo;
		protected System.Web.UI.WebControls.TextBox txt_codNewRuolo;
		protected System.Web.UI.WebControls.TextBox txt_descNewRuolo;
		protected System.Web.UI.WebControls.Label lbl_intest1;
		protected System.Web.UI.WebControls.Label lbl_intest2;
		protected System.Web.UI.WebControls.Label lbl_intest3;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_tiporuolo;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idCorrGlobRuoloDaSpostare;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idGruppoDaSpostare;
		//-------------------------------------------------------------------
		private string _idAmm = string.Empty;
		private string _idGruppoDaSpostare = string.Empty;
		private string _idCorrGlobRuoloDaSpostare = string.Empty;		
		private string _descRuoloDaSpostare = string.Empty;
		private string _tipoRuoloDaSpostare = string.Empty;
		private string _listaUtentiConnessi = string.Empty;
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
					
					string[] appo2 = this.hd_tiporuolo.Value.Split('-');
					this.txt_codNewRuolo.Text = appo2[0].Trim() + " " + this.txt_ricCod.Text;
					this.txt_descNewRuolo.Text = appo2[1].Trim() + " " + this.txt_ricDesc.Text;		
			
					this.hd_returnValueModal.Value = string.Empty;
				}
			}
		}

		private void Inizialize()
		{			
			//reperisce i dati dal chiamante
			this.GetDataQueryString();

			//verifica se tutti gli utenti del ruolo da spostare sono disconnessi dal sistema
			if(this.IsUsersConnected(this._idGruppoDaSpostare,this._idAmm))
			{
				string msg = "Attenzione,\\ni seguenti utenti del ruolo sono connessi al sistema DocsPA:\\n\\n"+this._listaUtentiConnessi+"\\n\\nImpossibile eseguire lo spostamento del ruolo!";
				this.executeJS("<SCRIPT>alert('"+msg+"'); self.close();</SCRIPT>");
			}

			//visualizza ed imposta dati sulla GUI
			this.lbl_ruolo.Text	= "<b>" + this._descRuoloDaSpostare + "</b>";
			this.hd_tiporuolo.Value = this._tipoRuoloDaSpostare;		
			
			this.btn_org.Attributes.Add("onclick","ApriOrganigramma();");			
		}

		private void GetDataQueryString()
		{
			//reperisce i dati dal chiamante			
			this._idCorrGlobRuoloDaSpostare = Request.QueryString["idCorrGlobRuoloDaSpostare"].ToString();
			this.hd_idCorrGlobRuoloDaSpostare.Value=this._idCorrGlobRuoloDaSpostare;

			this._idGruppoDaSpostare = Request.QueryString["idGruppoDaSpostare"].ToString();
			this.hd_idGruppoDaSpostare.Value = this._idGruppoDaSpostare;

			this._idAmm = Request.QueryString["idAmm"].ToString();
			this.hd_idAmm.Value = this._idAmm;
		
			this._descRuoloDaSpostare = Server.UrlDecode(Request.QueryString["descRuoloDaSpostare"].ToString());
			this._tipoRuoloDaSpostare = Server.UrlDecode(Request.QueryString["tipoRuoloDaSpostare"].ToString());
		}

		private bool IsUsersConnected(string idGruppoDaSpostare, string idAmm)
		{
			bool retValue = false;
			
			Manager.OrganigrammaManager manager = new Amministrazione.Manager.OrganigrammaManager();
			manager.ListaUtenti(idGruppoDaSpostare);
			ArrayList listaUtenti = manager.getListaUtenti();

			if(manager.getListaUtenti() != null && manager.getListaUtenti().Count > 0)
			{
				DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
				
				foreach(DocsPAWA.DocsPaWR.OrgUtente utente in manager.getListaUtenti())
				{
					manager.VerificaUtenteLoggato(utente.CodiceRubrica,idAmm);
					esito = manager.getEsitoOperazione();
					if(esito.Codice>0)
						_listaUtentiConnessi += "- "+utente.Cognome+" "+utente.Nome+"\\n";			
				}
			}

			if(!_listaUtentiConnessi.Equals(string.Empty))
				retValue = true;

			return retValue;
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

		#region Sposta ruolo tra UO

		/// <summary>
		/// Tasto Sposta ruolo
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_sposta_Click(object sender, System.EventArgs e)
		{
			try
			{
				if(this.txt_codNewRuolo.Text.Trim()!="" && this.txt_descNewRuolo.Text.Trim()!="")
				{
					DocsPAWA.DocsPaWR.OrgRuolo ruolo = new DocsPAWA.DocsPaWR.OrgRuolo();

					ruolo.IDCorrGlobale = this.hd_idCorrGlobRuoloDaSpostare.Value;
					ruolo.Codice = this.txt_codNewRuolo.Text;
					ruolo.CodiceRubrica = this.txt_codNewRuolo.Text;
					ruolo.Descrizione = this.txt_descNewRuolo.Text;
					ruolo.IDGruppo = this.hd_idGruppoDaSpostare.Value;
					ruolo.IDUo = this.hd_idCorrGlobDest.Value;
					ruolo.IDAmministrazione = this.hd_idAmm.Value;

					Manager.OrganigrammaManager manager = new Amministrazione.Manager.OrganigrammaManager();
					DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();

					manager.SpostaRuolo(ruolo);
					esito = manager.getEsitoOperazione();

					if(esito.Codice.Equals(0))
					{
                        InvalidaPassiCorrelati();

						string qs = "?idAmm="+ruolo.IDAmministrazione+"&idCorrGlobUO="+ruolo.IDUo+"&idCorrGlobRuolo="+ruolo.IDCorrGlobale+"&idGruppo="+ruolo.IDGruppo;
						Response.Redirect("Esito_Sposta_Ruolo.aspx" + qs);
					}
					else
						this.executeJS("<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'","\\'") + "');</SCRIPT>");
				}
			}
			catch
			{
				this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema');</SCRIPT>");
			}
		}

        private void InvalidaPassiCorrelati()
        {
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            wws.Timeout = System.Threading.Timeout.Infinite;
            List<DocsPaWR.ProcessoFirma> processiCoinvolti_R = (Session["processiCoinvolti_R"] != null && ((int)Session["processiCoinvolti_R"]) > 0 ? wws.GetProcessiDiFirmaByRuoloTitolare(this._idGruppoDaSpostare).ToList() : new List<DocsPaWR.ProcessoFirma>());
            List<DocsPaWR.IstanzaProcessoDiFirma> istazaProcessiCoinvolti_R = (Session["istazaProcessiCoinvolti_R"] != null && ((int)Session["istazaProcessiCoinvolti_R"]) > 0 ? wws.GetIstanzeProcessiDiFirmaByRuoloCoinvolto(this._idGruppoDaSpostare).ToList() : new List<DocsPaWR.IstanzaProcessoDiFirma>());

            if (processiCoinvolti_R.Count > 0)
            {
                List<string> idPassi = new List<string>();
                foreach (DocsPaWR.ProcessoFirma processo in processiCoinvolti_R)
                {
                    foreach (DocsPaWR.PassoFirma passo in processo.passi)
                    {
                        if (!idPassi.Contains(passo.idPasso))
                        {
                            idPassi.Add(passo.idPasso);
                        }
                    }
                }

                wws.TickPasso(idPassi.ToArray(), "R");
                Session["processiCoinvolti_R"] = null;
            }

            if (istazaProcessiCoinvolti_R.Count > 0)
            {
                DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                wws.TickIstanze(istazaProcessiCoinvolti_R.ToArray(), "R", sessionManager.getUserAmmSession());
                Session["istazaProcessiCoinvolti_R"] = null;
            }
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

						string[] appo2 = this.hd_tiporuolo.Value.Split('-');
						this.txt_codNewRuolo.Text = appo2[0].Trim() + " " + this.txt_ricCod.Text;
						this.txt_descNewRuolo.Text = appo2[1].Trim() + " " + this.txt_ricDesc.Text;	

						this.SetFocus(this.txt_codNewRuolo,false);

						DocsPAWA.Utils.DefaultButton(this, ref txt_codNewRuolo, ref btn_sposta);
						DocsPAWA.Utils.DefaultButton(this, ref txt_descNewRuolo, ref btn_sposta);
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
