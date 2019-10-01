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
using DocsPaWR = DocsPAWA.DocsPaWR;
using System.Linq;

namespace Amministrazione.Gestione_Organigramma
{
	/// <summary>
	/// Summary description for Sposta_Utente.
	/// </summary>
	public class Sposta_Utente : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lbl_intest1;
		protected System.Web.UI.WebControls.Label lbl_utente;
		protected System.Web.UI.WebControls.Label lbl_intest2;
		protected System.Web.UI.WebControls.Button btn_sposta;
		protected System.Web.UI.WebControls.ImageButton btn_org;	
		protected System.Web.UI.WebControls.TextBox txt_ricCod;
		protected System.Web.UI.WebControls.TextBox txt_ricDesc;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idCorrGlobUtente;	
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_userid;			
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_countUtenti;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idCorrGlobGruppo;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idGruppo;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idPeople;	
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idAmm;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModal;	
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idGruppoDest;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idCorrGlobGruppoDest;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModalLF;
        //-------------------------------------------------------------------
        private string _idAmm = string.Empty;				
		private string _idCorrGlobUtente = string.Empty;		
		private string _nomeUtenteDaSpostare = string.Empty;								
		private string _cognomeUtenteDaSpostare = string.Empty;

        private InvalidaPassiCorrelatiDelegate invalidaPassiCorrelati;

        #region Costanti

        private const string UTENTE_COINVOLTO = "UC";
        private const string ULTIMO_UTENTE_COINVOLTO = "UURC"; //L'utente che si stà rimuovendo è l'ultimo ed è coinvolto
        private const string ULTIMO_UTENTE_NON_COINVOLTO = "UURNC"; //L'utente che si stà rimuovendo non è coinvolto in processi/istanze
        private const string ULTIMO_UTENTE_COINVOLTO_E_NON = "UURCNC"; //L'utente che si stà rimuovendo è l'ultimo e ci sono processi in cui è coinvolti e altri no

        #endregion

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
					this.hd_idCorrGlobGruppoDest.Value = appo[2];
					this.hd_idGruppoDest.Value = appo[3];					
				}
                if (this.hd_returnValueModalLF.Value != null && this.hd_returnValueModalLF.Value != string.Empty && this.hd_returnValueModalLF.Value != "undefined")
                {
                    this.GestRitornoAvvisoLF(this.hd_returnValueModalLF.Value);
                }
            }
		}

		private void Inizialize()
		{			
			//reperisce i dati dal chiamante
			this.GetDataQueryString();			

			//visualizza ed imposta dati sulla GUI
			this.lbl_utente.Text = "<b>" + Server.UrlDecode(Request.QueryString["cognomeUtente"].ToString()) + " " + Server.UrlDecode(Request.QueryString["nomeUtente"].ToString()) + "</b>";								

			this.btn_org.Attributes.Add("onclick","ApriOrganigramma();");
		}

		private void GetDataQueryString()
		{
			//reperisce i dati dal chiamante			
			this._idCorrGlobUtente = Request.QueryString["idCorrGlobUtente"].ToString();
			this.hd_idCorrGlobUtente.Value=this._idCorrGlobUtente;

			this.hd_userid.Value=Request.QueryString["userid"].ToString();

			this.hd_countUtenti.Value=Request.QueryString["countUtenti"].ToString();

			this.hd_idCorrGlobGruppo.Value=Request.QueryString["idCorrGlobGruppo"].ToString();

			this.hd_idGruppo.Value=Request.QueryString["idGruppo"].ToString();

			this._idAmm = Request.QueryString["idAmm"].ToString();
			this.hd_idAmm.Value = this._idAmm;
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

		private void btn_sposta_Click(object sender, System.EventArgs e)
		{
			try
			{
				if(this.txt_ricCod.Text.Trim()!="" && this.txt_ricDesc.Text.Trim()!="")
				{
					string idPeople = this.DatiUtente(this.hd_idCorrGlobUtente.Value).IDPeople;
					this.hd_idPeople.Value = idPeople;

					if(this.hd_idAmm.Value!="" && 
						this.hd_userid.Value!="" &&
						this.hd_countUtenti.Value!="" && 
						this.hd_idPeople.Value!="" && 
						this.hd_idGruppo.Value!="" && 
						this.hd_idCorrGlobGruppo.Value!="" &&
						this.hd_idGruppoDest.Value!="" &&
						this.hd_idCorrGlobGruppoDest.Value!="")
					{

                        // verifica che l'utente non sia connesso a docspa
                        if (this.VerificaUtenteLoggato(this.hd_userid.Value, this.hd_idAmm.Value))
                        {
                                // verifica che non sia l'unico del ruolo
                                if (Convert.ToInt16(this.hd_countUtenti.Value) > 1)
                                {
                                    // NON è l'unico utente nel ruolo quindi lo elimina dal ruolo
                                    if (this.EliminaUtenteInRuolo(this.hd_idPeople.Value, this.hd_idGruppo.Value))
                                    {
                                        // ripulisce l'AREA DI LAVORO
                                        if (this.EliminaADLUtente(this.hd_idPeople.Value, this.hd_idCorrGlobGruppo.Value))
                                        {
                                            // inserisce l'utente nel nuovo ruolo
                                            if (this.InserimentoUtente(this.hd_idPeople.Value, this.hd_idGruppoDest.Value))
                                            {
                                                // inserisce trasm utente
                                                if (this.InsTrasmUtente(this.hd_idPeople.Value, this.hd_idCorrGlobGruppoDest.Value))
                                                {
                                                    DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                                                    AsyncCallback callback = new AsyncCallback(CallBack);
                                                    invalidaPassiCorrelati = new InvalidaPassiCorrelatiDelegate(InvalidaPassiCorrelati);
                                                    invalidaPassiCorrelati.BeginInvoke(this.hd_idPeople.Value, this.hd_idGruppo.Value, sessionManager.getUserAmmSession(), callback, null);

                                                    string returnValue = this.hd_idCorrGlobUtente.Value + "_" + this.hd_idCorrGlobGruppoDest.Value;
                                                    this.executeJS("<SCRIPT>alert('Operazione di spostamento utente eseguita correttamente.'); window.returnValue = '" + returnValue + "'; self.close();</SCRIPT>");
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // è l'unico utente del ruolo...
                                    // verifica che il ruolo non abbia trasmissioni con work-flow
                                    if (this.RuoloConTrasmissioni(this.hd_idCorrGlobGruppo.Value))
                                    {
                                        // ruolo con trasmissioni... avvisa l'amministratore di seguire un'altra procedura
                                        string msg = "Attenzione,\\n\\ncon lo spostamento di " + this.lbl_utente.Text.Replace("<b>", "").Replace("</b>", "") + " il ruolo rimane senza utenti.\\n\\nTuttavia il ruolo presenta trasmissioni che necessitano ACCETTAZIONE,\\npertanto non è possibile lasciare il ruolo privo di utenti.\\n\\n\\nProcedere come segue:\\nselezionare il ruolo nel quale ora è inserito " + this.lbl_utente.Text.Replace("<b>", "").Replace("</b>", "") + ",\\neliminarlo da questo ruolo tramite il tasto \\'Gestione utenti\\',\\nquindi inserirlo nel nuovo ruolo di destinazione utilizzando sempre il tasto indicato.";
                                        this.executeJS("<SCRIPT>alert('" + msg + "'); window.returnValue = 'N'; self.close();</SCRIPT>");
                                    }
                                    else
                                    {
                                        // elimina utente dal ruolo
                                        if (this.EliminaUtenteInRuolo(this.hd_idPeople.Value, this.hd_idGruppo.Value))
                                        {
                                            // ripulisce l'AREA DI LAVORO
                                            if (this.EliminaADLUtente(this.hd_idPeople.Value, this.hd_idCorrGlobGruppo.Value))
                                            {
                                                // inserisce l'utente nel nuovo ruolo
                                                if (this.InserimentoUtente(this.hd_idPeople.Value, this.hd_idGruppoDest.Value))
                                                {
                                                    // inserisce trasm utente
                                                    if (this.InsTrasmUtente(this.hd_idPeople.Value, this.hd_idCorrGlobGruppoDest.Value))
                                                    {
                                                        DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                                                        AsyncCallback callback = new AsyncCallback(CallBack);
                                                        invalidaPassiCorrelati = new InvalidaPassiCorrelatiDelegate(InvalidaPassiCorrelati);
                                                        invalidaPassiCorrelati.BeginInvoke(this.hd_idPeople.Value, this.hd_idGruppo.Value, sessionManager.getUserAmmSession(), callback, null);


                                                        string returnValue = this.hd_idCorrGlobUtente.Value + "_" + this.hd_idCorrGlobGruppoDest.Value;
                                                        this.executeJS("<SCRIPT>alert('Operazione di spostamento utente eseguita correttamente.'); window.returnValue = '" + returnValue + "'; self.close();</SCRIPT>");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
						}
					}
					else
					{
						this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema: dati insufficienti');</SCRIPT>");
					}
				}
			}
			catch
			{
				this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema');</SCRIPT>");
			}
		}

        private void InvalidaPassiCorrelati(string idPeople, string idRuolo, DocsPAWA.DocsPaWR.InfoUtenteAmministratore infoAmm)
        {
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            wws.Timeout = System.Threading.Timeout.Infinite;
            string tipoTick = "U";
            if (Convert.ToInt16(this.hd_countUtenti.Value) == 1)
            {
                idPeople = string.Empty;
                tipoTick = "R";
            }
            wws.InvalidaPassiCorrelatiTitolare(idRuolo, idPeople, tipoTick, infoAmm);
        }

        public delegate void InvalidaPassiCorrelatiDelegate(string idPeople, string idGruppo, DocsPAWA.DocsPaWR.InfoUtenteAmministratore infoAmm);


        private void CallBack(IAsyncResult result)
        {
            invalidaPassiCorrelati.EndInvoke(result);
        }
        private DocsPAWA.DocsPaWR.OrgUtente DatiUtente(string idCorrGlob)
		{			
			Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
			theManager.DatiUtente(idCorrGlob);
			return theManager.getDatiUtente();			
		}

		private bool VerificaUtenteLoggato(string userId, string idAmm)
		{
			bool result = false;
			
			Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
			theManager.VerificaUtenteLoggato(userId,idAmm);	

			DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
			esito = theManager.getEsitoOperazione();

			if(esito.Codice.Equals(0))
			{
				result = true;
			}
			else
			{
				this.executeJS("<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'","''") + "');</SCRIPT>");					
			}
			esito = null;					

			return result;
		}

        private bool VerificaPresenzaProcessiFirma(string idPeople, string idGruppo)
        {
            bool result = false;
            //Verifico che non ci siano processi attivi
            string idUtente;
            string idRuolo = Request.QueryString["idGruppo"].ToString();
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            int countProcessiCoinvolti = 0;
            int countIstazaProcessiCoinvolti = 0;
            string tipoTitolare = string.Empty;
            // verifica che non sia l'unico del ruolo
            idUtente = idPeople;
            countProcessiCoinvolti = ws.GetCountProcessiDiFirmaByTitolare(idRuolo, idUtente);
            countIstazaProcessiCoinvolti = ws.GetCountIstanzaProcessiDiFirmaByTitolare(idRuolo, idUtente);

            if (countProcessiCoinvolti > 0 || countIstazaProcessiCoinvolti > 0)
            {
                string msg = "Attenzione, poichè l\\'utente in modifica è coinvolto in processi di firma o processi di firma avviati, per effettuare lo spostamento è consigliato procedere come segue:\\n\\nutilizzare il tasto \\'Gestione utenti\\' presente nel dettaglio del ruolo per procedere con l\\'interruzione o la sostituzione dell\\'utente nei processi coinvolti.";
                this.executeJS("<SCRIPT>alert('" + msg + "'); window.returnValue = 'N'; self.close();</SCRIPT>");
                return true;
            }
            return result;
        }

        private void GestRitornoAvvisoLF(string valore)
        {
            try
            {
                switch (valore)
                {
                    case "Y":
                        // verifica che non sia l'unico del ruolo
                        if (Convert.ToInt16(this.hd_countUtenti.Value) > 1)
                        {
                            // NON è l'unico utente nel ruolo quindi lo elimina dal ruolo
                            if (this.EliminaUtenteInRuolo(this.hd_idPeople.Value, this.hd_idGruppo.Value))
                            {
                                // ripulisce l'AREA DI LAVORO
                                if (this.EliminaADLUtente(this.hd_idPeople.Value, this.hd_idCorrGlobGruppo.Value))
                                {
                                    // inserisce l'utente nel nuovo ruolo
                                    if (this.InserimentoUtente(this.hd_idPeople.Value, this.hd_idGruppoDest.Value))
                                    {
                                        // inserisce trasm utente
                                        if (this.InsTrasmUtente(this.hd_idPeople.Value, this.hd_idCorrGlobGruppoDest.Value))
                                        {
                                            DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                                            AsyncCallback callback = new AsyncCallback(CallBack);
                                            invalidaPassiCorrelati = new InvalidaPassiCorrelatiDelegate(InvalidaPassiCorrelati);
                                            invalidaPassiCorrelati.BeginInvoke(this.hd_idPeople.Value, this.hd_idGruppo.Value, sessionManager.getUserAmmSession(), callback, null);

                                            string returnValue = this.hd_idCorrGlobUtente.Value + "_" + this.hd_idCorrGlobGruppoDest.Value;
                                            this.executeJS("<SCRIPT>alert('Operazione di spostamento utente eseguita correttamente.'); window.returnValue = '" + returnValue + "'; self.close();</SCRIPT>");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // è l'unico utente del ruolo...
                            // verifica che il ruolo non abbia trasmissioni con work-flow
                            if (this.RuoloConTrasmissioni(this.hd_idCorrGlobGruppo.Value))
                            {
                                // ruolo con trasmissioni... avvisa l'amministratore di seguire un'altra procedura
                                string msg = "Attenzione,\\n\\ncon lo spostamento di " + this.lbl_utente.Text.Replace("<b>", "").Replace("</b>", "") + " il ruolo rimane senza utenti.\\n\\nTuttavia il ruolo presenta trasmissioni che necessitano ACCETTAZIONE,\\npertanto non è possibile lasciare il ruolo privo di utenti.\\n\\n\\nProcedere come segue:\\nselezionare il ruolo nel quale ora è inserito " + this.lbl_utente.Text.Replace("<b>", "").Replace("</b>", "") + ",\\neliminarlo da questo ruolo tramite il tasto \\'Gestione utenti\\',\\nquindi inserirlo nel nuovo ruolo di destinazione utilizzando sempre il tasto indicato.";
                                this.executeJS("<SCRIPT>alert('" + msg + "'); window.returnValue = 'N'; self.close();</SCRIPT>");
                            }
                            else
                            {
                                // elimina utente dal ruolo
                                if (this.EliminaUtenteInRuolo(this.hd_idPeople.Value, this.hd_idGruppo.Value))
                                {
                                    // ripulisce l'AREA DI LAVORO
                                    if (this.EliminaADLUtente(this.hd_idPeople.Value, this.hd_idCorrGlobGruppo.Value))
                                    {
                                        // inserisce l'utente nel nuovo ruolo
                                        if (this.InserimentoUtente(this.hd_idPeople.Value, this.hd_idGruppoDest.Value))
                                        {
                                            // inserisce trasm utente
                                            if (this.InsTrasmUtente(this.hd_idPeople.Value, this.hd_idCorrGlobGruppoDest.Value))
                                            {
                                                DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                                                AsyncCallback callback = new AsyncCallback(CallBack);
                                                invalidaPassiCorrelati = new InvalidaPassiCorrelatiDelegate(InvalidaPassiCorrelati);
                                                invalidaPassiCorrelati.BeginInvoke(this.hd_idPeople.Value, this.hd_idGruppo.Value, sessionManager.getUserAmmSession(), callback, null);

                                                this.executeJS("<SCRIPT>alert('Operazione di spostamento utente eseguita correttamente.'); window.returnValue = 'Y'; self.close();</SCRIPT>");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case "N":
                        break;
                }
                this.hd_returnValueModalLF.Value = "";
            }
            catch
            {
            }
        }

        private bool EliminaUtenteInRuolo(string idPeople, string idGruppo)
		{
			bool result = false;
            string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
			
			Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
			theManager.EliminaUtenteInRuolo(idPeople,idGruppo, idAmm);	

			DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
			esito = theManager.getEsitoOperazione();

			if(esito.Codice.Equals(0))
			{
				result = true;
			}
			else
			{
				this.executeJS("<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'","''") + "');</SCRIPT>");				
			}
			esito = null;
			
			return result;
		}

		private bool EliminaADLUtente(string idPeople, string idCorrGlobGruppo)
		{
			bool result = false;
			
			Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
			theManager.EliminaADLUtente(idPeople,idCorrGlobGruppo);	

			DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
			esito = theManager.getEsitoOperazione();

			if(esito.Codice.Equals(0))
			{
				result = true;
			}
			else
			{
				this.executeJS("<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'","''") + "');</SCRIPT>");				
			}
			esito = null;
			
			return result;
		}

		private bool RuoloConTrasmissioni(string idCorrGlobRuolo)
		{
			bool result = true;			
			
			Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
			theManager.VerificaTrasmRuolo(idCorrGlobRuolo);	

			DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
			esito = theManager.getEsitoOperazione();

			if(esito.Codice.Equals(0))
			{
				result = false;
			}
		
			esito = null;
			
			return result;
		}
		
		private bool RifiutaTrasmConWF(string idCorrGlobRuolo, string idGruppo)
		{
			bool result = false;

			Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
			theManager.RifiutaTrasmConWF(idCorrGlobRuolo, idGruppo);	

			DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
			esito = theManager.getEsitoOperazione();

			if(esito.Codice.Equals(0))
			{
				result = true;
			}
			else
			{
				this.executeJS("<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'","''") + "');</SCRIPT>");				
			}
			esito = null;
			
			return result;
		}

		private bool InserimentoUtente(string idPeople, string idGruppo)
		{
			bool result = false;
            string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

			Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
			theManager.InsUtenteInRuolo(idPeople, idGruppo,idAmm,"spostaUser");	

			DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
			esito = theManager.getEsitoOperazione();

			if(esito.Codice.Equals(0))
			{
				result = true;
			}
			else
			{
				this.executeJS("<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'","''") + "');</SCRIPT>");				
			}
			esito = null;

			return result;
		}

		private bool InsTrasmUtente(string idPeople, string idCorrGlobRuolo)
		{
			bool result = false;
			
			Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
			theManager.InsTrasmUtente(idPeople, idCorrGlobRuolo);	

			DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
			esito = theManager.getEsitoOperazione();

			if(esito.Codice.Equals(0))
			{
				result = true;
			}
			else
			{
				this.executeJS("<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'","''") + "');</SCRIPT>");				
			}
			esito = null;

			return result;
		}

		private void SetFocus(System.Web.UI.Control ctrl, bool sel)
		{
			DocsPAWA.Utils.SetFocus(this,ctrl,sel);			
		}

		private void executeJS(string key)
		{
			if(!this.Page.IsStartupScriptRegistered("theJS"))			
				this.Page.RegisterStartupScript("theJS", key);
		}

		private void txt_ricCod_TextChanged(object sender, System.EventArgs e)
		{		
			try
			{
				if(this.txt_ricCod.Text.Trim()!="" && this.txt_ricCod.Text.Trim().Length>0)
				{
					Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
				
					theManager.RicercaInOrg("R",this.txt_ricCod.Text.Trim(),"",this.hd_idAmm.Value, false, true);

					if(theManager.getRisultatoRicerca()!=null && theManager.getRisultatoRicerca().Count.Equals(1))
					{					
						foreach(DocsPAWA.DocsPaWR.OrgRisultatoRicerca risultato in theManager.getRisultatoRicerca())
						{
							this.txt_ricDesc.Text = risultato.Descrizione;
							this.hd_idCorrGlobGruppoDest.Value = risultato.IDCorrGlob;
							this.hd_idGruppoDest.Value = risultato.IDGruppo;							
						}
					}
					else
					{
						this.executeJS("<SCRIPT>alert('Nessun ruolo trovato');document.Form1.txt_ricDesc.select();</SCRIPT>");				
						this.SetFocus(this.txt_ricCod,true);
						this.txt_ricDesc.Text="";				
					}
				}
			}
			catch
			{

			}
		}
    }
}
