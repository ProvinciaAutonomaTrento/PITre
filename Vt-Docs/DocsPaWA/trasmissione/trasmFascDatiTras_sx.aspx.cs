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

//Andrea
using DocsPAWA.utils;
//End Andrea

namespace DocsPAWA.trasmissione
{
	/// <summary>
	/// Summary description for trasmDatiTrasm_sx.
	/// </summary>
	public class trasmFascDatiTras_sx : DocsPAWA.CssPage
	{

        //Andrea
        private string messError = "";
        private string MessError_DestPerCodice = "";
        private ArrayList listaExceptionTrasmissioni1 = new ArrayList();
        //End Andrea

		protected System.Web.UI.WebControls.Label lbl_ruolo;
		protected DocsPaWebCtrlLibrary.ImageButton btn_rubricaDest;
		protected DocsPaWebCtrlLibrary.ImageButton btn_rispostaA;
		protected System.Web.UI.WebControls.DropDownList ddl_ragioni;
		protected System.Web.UI.WebControls.TextBox txt_Note;

		//my var
		protected DocsPAWA.DocsPaWR.InfoFascicolo infoFasc;
		protected DocsPAWA.DocsPaWR.Fascicolo fasc;
		protected DocsPAWA.DocsPaWR.Trasmissione trasmissione;
		protected System.Web.UI.WebControls.TextBox txt_descrizione;
		protected DocsPAWA.DocsPaWR.RagioneTrasmissione[] listaRagioni;
		protected System.Web.UI.WebControls.Label lbl_dataDoc;
		protected System.Web.UI.WebControls.Label lbl_oggettoDoc;
		protected System.Web.UI.WebControls.Label lbl_titolo_doc;
        protected Utilities.MessageBox msg_SalvaCompTrasm;
        protected Utilities.MessageBox msg_SalvaTrasm;

		protected DocsPAWA.DocsPaWR.Utente utente;
		protected DocsPAWA.DocsPaWR.Ruolo ruolo;
		protected Hashtable m_hashTableRuoliSup;
		protected DocsPAWA.DocsPaWR.Ruolo[] listaRuoliSup;

		protected Hashtable m_hashTableUtenti;
		protected DocsPAWA.DocsPaWR.Corrispondente[] listaUtenti;
		protected System.Web.UI.WebControls.TextBox txt_utenteTrasm;
		protected DocsPaWebCtrlLibrary.ImageButton btn_modMit;
		protected System.Web.UI.WebControls.TextBox txt_ruoloTrasm;
		protected System.Web.UI.WebControls.Label lbl_ruol;
		protected System.Web.UI.WebControls.Label lbl_ut;
		protected System.Web.UI.WebControls.Label lbl_dataDocumento;
		protected System.Web.UI.WebControls.Label lbl_segnDocumento;
		protected DocsPaWebCtrlLibrary.ImageButton btn_SalvaTemplate;
		protected DocsPaWebCtrlLibrary.ImageButton btn_NuovaDaTempl;
		protected DocsPaWebCtrlLibrary.ImageButton btn_SalvaCompTrasm;
		protected DocsPaWebCtrlLibrary.ImageButton btn_SalvaTrasm;
        protected DocsPaWebCtrlLibrary.ImageButton btn_EliminaTrasmSalvata;
		protected System.Web.UI.WebControls.TextBox txt_oggettoDocumento;

        //protected Hashtable m_hashTableRagioneTrasmissione;
		protected System.Web.UI.WebControls.TextBox txt_codDest;
		protected System.Web.UI.WebControls.ImageButton ibtnMoveToA;

		private ArrayList trasm_strutture_vuote  = new ArrayList();
		protected string gerarchia_trasm;

        protected bool userAutorizedEditingACL;
        protected System.Web.UI.WebControls.CheckBox cbx_cediDiritti;
        protected System.Web.UI.WebControls.Panel pnl_cediDiritti;
        protected int numeroRuoliDestInTrasmissione = 0;
        protected int numeroUtentiConNotifica = 0;
        protected System.Web.UI.HtmlControls.HtmlInputHidden azione;

        protected System.Web.UI.HtmlControls.HtmlInputControl clTestoOgg;
        protected  int caratteriDisponibiliOgg = 2000;


        protected System.Web.UI.HtmlControls.HtmlInputControl clTestoDesc;
        protected  int caratteriDisponibiliDesc = 2000;

        protected System.Web.UI.HtmlControls.HtmlInputControl clTestoNote;
        protected  int caratteriDisponibiliNote = 250;
		
		private void Page_Load(object sender, System.EventArgs e)
		{
            //gestione finestra attendere
            //const string onClickBtn = "javascript:_buttonClicked=this.id;";
            //btn_SalvaCompTrasm.Attributes["onClick"] = onClickBtn;
			
			Utils.startUp(this);
			
			//schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
			fasc = FascicoliManager.getFascicoloSelezionato(this);
			
			trasmissione = TrasmManager.getGestioneTrasmissione(this);
			utente = UserManager.getUtente(this);
			ruolo = UserManager.getRuolo(this);

			//se è null la trasmissione è nuova altrimenti è in modifica
			if (trasmissione == null)
			{
				trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();
				trasmissione.systemId = null;
				trasmissione.ruolo = ruolo;
				trasmissione.utente = utente;
				trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
				TrasmManager.setGestioneTrasmissione(this, trasmissione);
			} 
			else
				trasmissione.daAggiornare = true;

            // gestione cessione diritti
            this.checkIsAutorizedEditingACL();
            //this.esisteCessioneinTrasm(); // implementato per il ritorno della selezione del modello di trasmissione

			if(!IsPostBack)
			{

                DocsPAWA.DocsPaWR.InfoUtente info = new DocsPAWA.DocsPaWR.InfoUtente();
                info = UserManager.getInfoUtente(this.Page);


                string valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_NOTE");
                if (valoreChiave != null)
                    caratteriDisponibiliNote = int.Parse(valoreChiave);


                txt_Note.MaxLength = caratteriDisponibiliNote;
                clTestoNote.Value = caratteriDisponibiliNote.ToString();
                txt_Note.Attributes.Add("onKeyUp", "calcTesto(this,'" + caratteriDisponibiliNote.ToString() + " ','Descrizione Nota'," + clTestoNote.ClientID + ")");
                txt_Note.Attributes.Add("onchange", "calcTesto(this,'" + caratteriDisponibiliNote.ToString() + " ','Descrizione Nota'," + clTestoNote.ClientID + ")");

                valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_OGGETTO");
                if (valoreChiave != null)
                    caratteriDisponibiliOgg = int.Parse(valoreChiave);


                txt_Note.MaxLength = caratteriDisponibiliOgg;
                clTestoOgg.Value = caratteriDisponibiliOgg.ToString();
                txt_Note.Attributes.Add("onKeyUp", "calcTesto(this,'" + caratteriDisponibiliOgg.ToString() + " ','Descrizione Oggetto'," + clTestoOgg.ClientID + ")");
                txt_Note.Attributes.Add("onchange", "calcTesto(this,'" + caratteriDisponibiliOgg.ToString() + " ','Descrizione Oggetto'," + clTestoOgg.ClientID + ")");

                /*
                valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_DESC_TRASM");
                if (valoreChiave != null)
                    caratteriDisponibiliDesc = int.Parse(valoreChiave);


                txt_Note.MaxLength = caratteriDisponibiliDesc;
                clTestoDesc.Value = caratteriDisponibiliDesc.ToString();
                txt_Note.Attributes.Add("onKeyUp", "calcTesto(this,'" + caratteriDisponibiliDesc.ToString() + " ','Descrizione Trasmissione'," + clTestoDesc.ClientID + ")");
                txt_Note.Attributes.Add("onchange", "calcTesto(this,'" + caratteriDisponibiliDesc.ToString() + " ','Descrizione Trasmissione'," + clTestoDesc.ClientID + ")");
                */

                valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_DESC_TRASM");
                if (!string.IsNullOrEmpty(valoreChiave))
                {
                    caratteriDisponibiliNote = int.Parse(valoreChiave);
                }

                txt_Note.MaxLength = caratteriDisponibiliNote;
                clTestoNote.Value = caratteriDisponibiliNote.ToString();
                txt_Note.Attributes.Add("onKeyUp", "calcTesto(this,'" + caratteriDisponibiliNote.ToString() + " ','NOTE'," + clTestoNote.ClientID + ")");
                txt_Note.Attributes.Add("onchange", "calcTesto(this,'" + caratteriDisponibiliNote.ToString() + " ','NOTE'," + clTestoNote.ClientID + ")");


                if (!Request.QueryString["azione"].Equals(""))
                    this.azione.Value = Request.QueryString["azione"];
                else
                    this.azione.Value = "Nuova";

                if (this.azione.Value.Equals("Modifica"))
                {
                    this.btn_EliminaTrasmSalvata.Visible = true;
                    this.btn_EliminaTrasmSalvata.Attributes.Add("onclick", "if (!window.confirm('La trasmissione salvata sarà eliminata. Procedere?')) {return false};");
                    this.lbl_titolo_doc.Text = "Modifica trasmissione relativa al fascicolo:";
                }
                else
                    this.lbl_titolo_doc.Text = "Nuova Trasmissione relativa al fascicolo:";

				//carica il ruolo scelto e l'utente
				this.lbl_ruolo.Text = ruolo.descrizione;
				
				string funct = "ApriFinestraTrasmXRisp()"; 
				this.btn_rispostaA.Attributes.Add("onClick", funct);
				this.btn_rubricaDest.Enabled = false;
				this.btn_rispostaA.Enabled = false;

				//bottone modificaMitt
				this.btn_modMit.Attributes.Add("onClick", funct);

				//bottone per l'inserimento di un oggetto
				string page = "'../popup/modificaMittTrasm.aspx'";
				string titolo = "'Modifica_MittenteTrasm'";
				string param = "'width=420,height=200, scrollbar=no'";
				this.btn_modMit.Attributes.Add("onclick","ApriFinestraGen("+ page + ',' + titolo + ',' + param + ");");


				if (trasmissione != null)
					this.txt_Note.Text = trasmissione.noteGenerali;

			// inserisco i dati relativi al documento selezionato
				if (fasc!= null)
				{
					//this.lbl_oggettoDocumento.Text =fasc.descrizione;
					this.txt_oggettoDocumento.Text = fasc.descrizione;					
					if(fasc.stato!=null && fasc.stato.Equals("A"))
					{
						this.lbl_dataDoc.Text = "APERTO";	
						this.lbl_dataDocumento.Text = fasc.apertura;
						this.lbl_segnDocumento.Text = fasc.codice;
					}
					else
					{
						this.lbl_dataDoc.Text = "CHIUSO";
						this.lbl_dataDocumento.Text = fasc.chiusura;
						this.lbl_segnDocumento.Text = fasc.codice;
					}
				}

                // gestione cessione diritti - visualizzazione del check-box
                this.abilitaCbx_cediDiritti();

                caricaRagioniFasc(fasc);
			}

            //Andrea

            if (Session["MessError"] != null)
            {
                messError = Session["MessError"].ToString();
                //Response.Write("<script language=\"javascript\">alert('Trasmissioni avvenute con successo per tutti i destinatari ad eccezione delle seguenti: \\n" + messError + "');</script>");
                Response.Write("<script language=\"javascript\">alert('Trasmissioni con esito negativo: \\n" + messError + "\\n');</script>");
                Session.Remove("MessError");
            }

            //End Andrea
			
			DocsPAWA.Utils.DefaultButton (this, ref this.txt_codDest, ref this.ibtnMoveToA);
        }      

		private void trasmDatiTrasm_sx_PreRender(object sender, System.EventArgs e) 
		{
            this.clTestoDesc.Value = (this.caratteriDisponibiliDesc - txt_descrizione.Text.Length).ToString();
            this.clTestoOgg.Value = (this.caratteriDisponibiliOgg - txt_oggettoDocumento.Text.Length).ToString();
			//visualizzo le informazioni
			this.btn_NuovaDaTempl.Attributes.Add("onclick","ApriFinestraListaTemplate('F');");

			//visualizzo le informazioni
			trasmissione = TrasmManager.getGestioneTrasmissione(this);
			if (trasmissione != null)
			{
				this.txt_ruoloTrasm.Text = trasmissione.ruolo.descrizione;
				this.txt_utenteTrasm.Text = trasmissione.utente.descrizione;
			
			    //controllo se è stato cambiato il mittente della trasmissione
				if(!trasmissione.ruolo.systemId.Equals(ruolo.systemId))
					this.btn_SalvaTrasm.Enabled = false;
				else
					this.btn_SalvaTrasm.Enabled = true;

                this.btn_SalvaTrasm.Enabled = (trasmissione.trasmissioniSingole != null);
                this.btn_SalvaCompTrasm.Enabled = (trasmissione.trasmissioniSingole != null);
                this.btn_SalvaTemplate.Enabled = (trasmissione.trasmissioniSingole != null);  
			}
			
			// Inizializzazione condizionale link rubrica
			string obj_type = "F:";
			if (fasc != null && fasc.idClassificazione != null)
				obj_type += fasc.idClassificazione;

			string use_new_rubrica = DocsPAWA.ConfigSettings.getKey (ConfigSettings.KeysENUM.RUBRICA_V2);
			ImageButton btn_rubricaDest = (ImageButton) FindControl ("btn_rubricaDest");

			if (use_new_rubrica != "1") 
				btn_rubricaDest.Attributes["onClick"] = "ApriRubrica('trasm','trasm');";
			else
				btn_rubricaDest.Attributes["onClick"] = "ApriRubricaTrasm(ddl_ragioni.value,'" + obj_type + "');";

			btn_rubricaDest.Attributes["onMouseOver"] = "ChangeCursorT('hand','btn_rubricaDest');";

            DocsPaWR.RagioneTrasmissione rt = TrasmManager.getRagioneSel(this);
            if (rt != null)
            {
                this.gerarchia_trasm = rt.tipoDestinatario.ToString("g").Substring(0, 1);
            }

            //Andrea

            if (Session["MessError_DestPerCodice"] != null)
            {
                messError = Session["MessError_DestPerCodice"].ToString();
                //Response.Write("<script language=\"javascript\">alert('Trasmissioni avvenute con successo per tutti i destinatari ad eccezione delle seguenti: \\n" + messError + "');</script>");
                //Response.Write("<script language=\"javascript\">alert('Trasmissioni con esito negativo: \\n" + messError + "\\n');</script>");
                Response.Write("<script language=\"javascript\">alert('AVVISO: I seguenti destinatari saranno esclusi dalla Nuova Trasmissione: \\n" + messError + "\\n');</script>");
                Session.Remove("MessError_DestPerCodice");
            }

            //End Andrea

		}

		private void caricaRagioniFasc(DocsPAWA.DocsPaWR.Fascicolo fasc)
		{
			if (fasc != null)
			{
				listaRagioni = TrasmManager.getListaRagioniFasc(this,fasc);
								
                //if (!Page.IsPostBack)
                //{
                //    m_hashTableRagioneTrasmissione=new Hashtable();
					if (listaRagioni!=null && listaRagioni.Length>0)
					{
						for (int i=0; i<listaRagioni.Length; i++)
						{
                            if(listaRagioni[i].descrizione.Equals("RISPOSTA"))
							{
								if (checkRagione(trasmissione))
									continue;
							}
                            
                            // se la ragione prevede cessione ('W' o 'R') e l'utente non è abilitato alla cessione,
                            // allora non aggiunge ragioni alla DDL
                            if (!listaRagioni[i].prevedeCessione.Equals("N") && !userAutorizedEditingACL)
                            {
                                continue;
                            }
                            else
                            {
                                ListItem newItem = new ListItem(listaRagioni[i].descrizione, listaRagioni[i].systemId);
                                ddl_ragioni.Items.Add(newItem);
                                //m_hashTableRagioneTrasmissione.Add(i, listaRagioni[i]);
                            }
						}

                        //TrasmManager.setHashRagioneTrasmissione(this,m_hashTableRagioneTrasmissione);
	
						this.ddl_ragioni.SelectedIndex = 0;
                        //this.txt_descrizione.Text=listaRagioni[0].note;
                        //TrasmManager.setRagioneSel(this, listaRagioni[0]);
                        //if (!listaRagioni[0].descrizione.Equals("RISPOSTA"))
                        //{
                        //    this.btn_rispostaA.Enabled = false;
                        //    this.btn_rubricaDest.Enabled = true;
                        //}
                        //else
                        //{
                        //    this.btn_rispostaA.Enabled = true;
                        //    this.btn_rubricaDest.Enabled = false;
                        //}

                        //// gestione cessione diritti
                        //DocsPaWR.RagioneTrasmissione ragtr = TrasmManager.getRagioneSel(this);
                        //this.gestioneCbxCessione(ragtr);

                        this.txt_codDest.Enabled = false;
                        this.ibtnMoveToA.Enabled = false;
                        this.btn_rubricaDest.Enabled = false;
                        this.btn_rispostaA.Enabled = false;
                        this.txt_descrizione.Text = "";

                        if (userAutorizedEditingACL)
                        {
                            this.cbx_cediDiritti.Checked = false;
                            this.cbx_cediDiritti.Enabled = false;
                        }
					}
                //}
                //else
                //{
                //    m_hashTableRagioneTrasmissione=TrasmManager.getHashRagioneTrasmissione(this);
                //}

				DocsPaWR.RagioneTrasmissione rt = TrasmManager.getRagioneSel(this);
                if (rt != null)
                {
                    this.gerarchia_trasm = rt.tipoDestinatario.ToString("g").Substring(0, 1);                    
                }
			}
		}
		

		private bool checkRagione(DocsPAWA.DocsPaWR.Trasmissione trasm)
		{

			if(trasm!= null)
			{
				if(trasm.trasmissioniSingole != null)
				{

					for (int i=0; i< trasm.trasmissioniSingole.Length; i++)
					{
						if (trasm.trasmissioniSingole[i].ragione.descrizione.Equals("RISPOSTA"))
							return true;
					}
				}
			}
			
			return false;
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
			this.ddl_ragioni.SelectedIndexChanged += new System.EventHandler(this.ddl_ragioni_SelectedIndexChanged);
			this.ibtnMoveToA.Click += new System.Web.UI.ImageClickEventHandler(this.ibtnMoveToA_Click);
			this.txt_Note.TextChanged += new System.EventHandler(this.txt_Note_TextChanged);
			this.btn_SalvaTemplate.Click += new System.Web.UI.ImageClickEventHandler(this.btn_SalvaTemplate_Click);
			this.btn_SalvaTrasm.Click += new System.Web.UI.ImageClickEventHandler(this.btn_SalvaTrasm_Click);
			this.btn_SalvaCompTrasm.Click += new System.Web.UI.ImageClickEventHandler(this.btn_SalvaCompTrasm_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.trasmDatiTrasm_sx_PreRender);
            this.msg_SalvaCompTrasm.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_SalvaCompTrasm_GetMessageBoxResponse);
            this.msg_SalvaTrasm.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_SalvaTrasm_GetMessageBoxResponse);
            this.btn_EliminaTrasmSalvata.Click += new System.Web.UI.ImageClickEventHandler(this.btn_EliminaTrasmSalvata_Click);
        }
		#endregion

		private void caricaValoriPerRagione(int indexRagione)
		{
            //if (m_hashTableRagioneTrasmissione!=null)
            //{
            //    DocsPaWR.RagioneTrasmissione ragione=(DocsPAWA.DocsPaWR.RagioneTrasmissione)m_hashTableRagioneTrasmissione[indexRagione];
            //    if (ragione!=null)
            //        this.txt_descrizione.Text = ragione.note;
            //}
		}

		private void ddl_ragioni_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            try
            {
                //int indexRagione = this.ddl_ragioni.SelectedIndex;
                //DocsPaWR.RagioneTrasmissione ragione = listaRagioni[indexRagione];

                if (this.ddl_ragioni.SelectedIndex.Equals(0))
                {
                    this.txt_codDest.Enabled = false;
                    this.ibtnMoveToA.Enabled = false;
                    this.btn_rubricaDest.Enabled = false;
                    this.btn_rispostaA.Enabled = false;
                    this.txt_descrizione.Text = "";
                    if (userAutorizedEditingACL)
                    {
                        this.cbx_cediDiritti.Checked = false;
                        this.cbx_cediDiritti.Enabled = false;
                    }
                    return;
                }

                DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
                DocsPaWR.RagioneTrasmissione ragione = ws.getRagioneById(this.ddl_ragioni.SelectedValue);

                if (ragione != null)
                {
                    //Controllo il tipo di Risposta: se è in RISPOSTA abilito il bottone - In Risposta a -
                    if (ragione.tipoRisposta != null && ragione.tipoRisposta.Equals("R"))
                    {
                        this.btn_rispostaA.Enabled = true;
                        this.btn_rubricaDest.Enabled = false;
                    }
                    else
                    {
                        this.btn_rispostaA.Enabled = false;
                        this.btn_rubricaDest.Enabled = true;
                    }

                    //caricaValoriPerRagione(indexRagione);
                    this.txt_codDest.Enabled = true;
                    this.ibtnMoveToA.Enabled = true;
                    this.txt_descrizione.Text = ragione.note;

                    // mette in sessione la ragione selezionata
                    TrasmManager.setRagioneSel(this, ragione);

                    //Se modifico la ragione devo invalidare i valori scelti per i corrispondenti
                    gerarchia_trasm = ragione.tipoDestinatario.ToString("g").Substring(0, 1);

                    this.txt_codDest.Text = "";

                    // GESTIONE CESSIONE DIRITTI
                    this.gestioneCbxCessione(ragione);

                    if (userAutorizedEditingACL)
                        this.cbx_cediDiritti_CheckedChanged(null, null);
                }
            }
            catch
            {
                return;
            }  
		}

		private void txt_Note_TextChanged(object sender, System.EventArgs e)
		{
			DocsPaWR.Trasmissione trasmissione;
			trasmissione = TrasmManager.getGestioneTrasmissione(this);
			trasmissione.noteGenerali = txt_Note.Text;
			TrasmManager.setGestioneTrasmissione(this, trasmissione);
		}

		/// <summary>
		/// Ripristino del contesto chiamante
		/// </summary>
		private void RestoreCallerContext()
		{
			// Ripristino contesto precedente
			SiteNavigation.CallContextStack.RestoreCaller();

			SiteNavigation.NavigationContext.RefreshNavigation();
		}

		private void salva(string parameter)
		{
			string funct;			
			funct = "top.principale.iFrame_dx.document.forms[0].flag_save_fasc.value='" + parameter +"'; ";
			funct += "top.principale.iFrame_dx.document.forms[0].submit(); ";
			Response.Write("<script language='javascript'> "+ funct + "</script>");
		}
		
		private void btn_SalvaTrasm_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            if (fasc.privato != null && fasc.privato == "1")
            {
                string messaggio = InitMessageXml.getInstance().getMessage("trasmFascPrivato");
                msg_SalvaTrasm.Confirm(messaggio);
            }
            else
            {
                this.RestoreCallerContext();
                salva("S");
            }
		}

		private void LinkButton1_Click(object sender, System.EventArgs e)
		{
			//Response.Write("<script language='javascript'>window.open('../documento/gestionedoc.aspx?tab=trasmissioni','principale');</script>");
			Response.Write("<script language='javascript'>top.principale.document.location='../documento/gestionedoc.aspx?tab=trasmissioni';</script>");
		}

//		private void imgDoc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
//		{
//			//Response.Write("<script language='javascript'>window.open('../fascicolo/gestioneFasc.aspx?tab=trasmissioni','principale');</script>");
//			Response.Write("<script language='javascript'>top.principale.document.location='../fascicolo/gestioneFasc.aspx?tab=trasmissioni';</script>");
//
//		}

        private void btn_SalvaCompTrasm_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (this.fasc != null && this.fasc.privato == "1")
            {
                string messaggio = InitMessageXml.getInstance().getMessage("trasmFascPrivato");
                msg_SalvaCompTrasm.Confirm(messaggio);
            }
            else
            {
                // gestione cessione diritti - inizializzazione dati oggetto
                if (this.setObjCessioneDiritti())
                    salva("ST");
            }
        }

		private void btn_SalvaTemplate_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            // gestione cessione diritti - inizializzazione dati oggetto
            if (this.setObjCessioneDiritti())
                salva("STempl");
		}

        private void msg_SalvaTrasm_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                this.RestoreCallerContext();
                salva("S");
            }
        }

        private void msg_SalvaCompTrasm_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                this.RestoreCallerContext();
                //riapre la finestra attendere prego
                //ClientScript.RegisterStartupScript(this.GetType(), "refreshFascProgressMsg", "beforeSubmit(); ", true);

                // gestione cessione diritti - inizializzazione dati oggetto
                if (this.setObjCessioneDiritti())
                    salva("ST");
            }
            //if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Cancel)
            //{
            //    //chiude la finestra attendere prego
            //    ClientScript.RegisterStartupScript(this.GetType(), "chiusuraProgressMsgTrasmFascDatiTrasm", "hideWorkingInProgress('trasmFascDatiTrasm');", true);
            
            //}
        }

		#region Selezione del destinatario
		/// <summary>
		/// Seleziona destinatari per codice
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ibtnMoveToA_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{	
			try
			{
				if(this.txt_codDest.Text.Trim()!="" || this.txt_codDest.Text.Trim()!=string.Empty)
				{
					DocsPaWR.ParametriRicercaRubrica qco = new DocsPAWA.DocsPaWR.ParametriRicercaRubrica();
					DocsPAWA.UserManager.setQueryRubricaCaller (ref qco);
					qco.codice = this.txt_codDest.Text.Trim();
					qco.tipoIE = DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO;			
					//cerco su tutti i tipi utente:
					if(System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] != null && System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] == "1")
						qco.doListe=true;
					qco.doRuoli=true;
					qco.doUtenti=true;
					qco.doUo=true;
					//query per codice  esatta, no like.
					qco.queryCodiceEsatta=true;
					DocsPaWR.RagioneTrasmissione rt = TrasmManager.getRagioneSel(this);
					this.gerarchia_trasm = rt.tipoDestinatario.ToString("g").Substring(0,1);
					
					switch(this.gerarchia_trasm)
					{
						case "T":
							qco.calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_TRASM_ALL;							
							break;					
						case "I":
							qco.calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_TRASM_INF;							
							break;					
						case "S":
							qco.calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_TRASM_SUP;							
							break;							
						case "P":
							qco.calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_TRASM_PARILIVELLO;
							break;		
					}

					string objtype = string.Empty;					
					if (fasc != null)
					{												
						qco.ObjectType = "F:" + fasc.idClassificazione;
				
						DocsPaWR.ElementoRubrica[] corrSearch = UserManager.getElementiRubrica(this.Page, qco);
                        if (corrSearch != null && corrSearch.Length > 0) 
						{
                            if (corrSearch[0].tipo != "R" || (corrSearch[0].tipo == "R" && !corrSearch[0].disabledTrasm))
                            {
                                this.ImpostaDestinatario(corrSearch, qco);
                                this.txt_codDest.Text = "";
                                this.ddl_ragioni.SelectedIndex = 0;
                                this.ddl_ragioni_SelectedIndexChanged(null, null);
                            }
                            else
                            {
                                RegisterStartupScript("noresults", "<script language=\"javascript\">alert(\"Il Ruolo selezionato risulta disabilitato alla ricezione delle trasmissioni\");document.trasmDatiTrasm_sx.txt_codDest.select();</script>");
                            }
						}
						else
						{
							RegisterStartupScript ("noresults", "<script language=\"javascript\">alert(\"Nessun destinatario presente con il codice inserito\");document.trasmFascDatiTras_sx.txt_codDest.select();</script>");							
						}	
						
						corrSearch = null;
					}		

					qco = null;
					rt = null;						
				}
			}
			catch
			{
				RegisterStartupScript ("ErrResults", "<script language=\"javascript\">alert(\"Attenzione! si è verificato un errore nella ricerca del destinatario\");</script>");
			}
		}		

		/// <summary>
		/// Imposta Destinatario
		/// </summary>
		/// <param name="corrSearch"></param>
		/// <param name="qco"></param>
		private void ImpostaDestinatario(DocsPAWA.DocsPaWR.ElementoRubrica[] corrSearch, DocsPAWA.DocsPaWR.ParametriRicercaRubrica prr)
		{					
			string t_avviso = string.Empty;
			DocsPaWR.Corrispondente corr;

			
			DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoInLavorazione(this.Page);						
				
			DocsPaWR.Trasmissione trasmissione = TrasmManager.getGestioneTrasmissione(this.Page);
										
			// verifica liste di distribuzione
			if(corrSearch[0].tipo.Equals("L"))
			{
				if(System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] != null && System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] == "1")
				{
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
					ArrayList listaCorr = UserManager.getCorrispondentiByCodLista(this.Page,prr.codice,idAmm);
					if(listaCorr!=null && listaCorr.Count > 0)
					{
						DocsPaWR.Trasmissione trasmissioni = TrasmManager.getGestioneTrasmissione(this.Page);
						ElementoRubrica[] ers = new ElementoRubrica[listaCorr.Count];

						for(int i=0; i<listaCorr.Count; i++)
						{
							DocsPaWR.ElementoRubrica er_1 = new DocsPAWA.DocsPaWR.ElementoRubrica();
							DocsPaWR.Corrispondente c = (DocsPAWA.DocsPaWR.Corrispondente) listaCorr[i];
							er_1 = UserManager.getElementoRubrica(this,c.codiceRubrica);
                            if (!er_1.disabledTrasm)
                                ers[i] = er_1;   							
						}
						int coutStartErs = ers.Length;
						//Andrea
                        
                        ////Si filtrano i corrispondenti della lista per verificate la loro autorizzazione per risoluzione del bug 2285
                        //ElementoRubrica[] ers_1 = UserManager.filtra_trasmissioniPerListe(this,prr,ers);

                        //for(int i=0; i<listaCorr.Count; i++)
                        //{
                        //    DocsPaWR.Corrispondente c = (DocsPAWA.DocsPaWR.Corrispondente) listaCorr[i];
                        //    for(int j=0; j<ers_1.Length; j++)
                        //    {
                        //        if(c.codiceRubrica == ers_1[j].codice)
                        //        {
                        //            trasmissioni = addTrasmissioneSingola(trasmissione, c);
                        //            break;
                        //        }
                        //    }
                        //}
                        //if(trasmissione.trasmissioniSingole == null || trasmissioni.trasmissioniSingole.Length != listaCorr.Count)
                        //{
                        //    t_avviso = String.Format ("alert (\"AVVISO: Nella lista ci sono corrispondenti ai quali non è possibile trasmettere !\");");														
                        //}

                        //Andrea
                        for (int i = 0; i < listaCorr.Count; i++)
                        {
                            DocsPaWR.Corrispondente c = (DocsPAWA.DocsPaWR.Corrispondente)listaCorr[i];
                
                                trasmissioni = addTrasmissioneSingola(trasmissione, c);

                        }

                        //Andrea
                        foreach (string s in listaExceptionTrasmissioni1)
                        {
                            messError = messError + s + "\\n";
                        }

                        if (listaExceptionTrasmissioni1 != null && listaExceptionTrasmissioni1.Count != 0)
                        {
                            listaExceptionTrasmissioni1 = new ArrayList();
                        }

                        if (messError != "")
                        {
                            Session.Add("MessError_DestPerCodice", messError);
                        }
                        //End Andrea

						TrasmManager.setGestioneTrasmissione(this.Page, trasmissioni);
					}
				}
				else
				{
					RegisterStartupScript ("checkListe", "<script language=\"javascript\">alert(\"Attenzione! le liste di distribuzione non sono state attivate in questo sistema\");</script>");
					return;
				}
			}
			else
			{
				DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();
				qco.codiceRubrica = prr.codice;
				qco.getChildren = false;
				qco.idAmministrazione= UserManager.getInfoUtente(this.Page).idAmministrazione;
				qco.tipoUtente = DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO;
                qco.fineValidita = true;

				corr = UserManager.getListaCorrispondenti(this.Page, qco)[0];
		
				trasmissione = this.addTrasmissioneSingola(trasmissione, corr);

                //Andrea
                foreach (string s in listaExceptionTrasmissioni1)
                {
                    messError = messError + s + "\\n";
                }

                if (listaExceptionTrasmissioni1 != null && listaExceptionTrasmissioni1.Count != 0)
                {
                    listaExceptionTrasmissioni1 = new ArrayList();
                }

                if (messError != "")
                {
                    Session.Add("MessError_DestPerCodice", messError);
                }
                //End Andrea
			}

			TrasmManager.setGestioneTrasmissione(this.Page, trasmissione);

			
			HttpContext.Current.Response.Write("<script>top.principale.iFrame_dx.document.forms[0].submit();" + t_avviso + "</script>");
							
		}

		/// <summary>
		/// Aggiunge Trasmissione Singola
		/// </summary>
		/// <param name="trasmissione"></param>
		/// <param name="corr"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPAWA.DocsPaWR.Trasmissione trasmissione, DocsPAWA.DocsPaWR.Corrispondente corr) 
		{
			if (trasmissione.trasmissioniSingole != null)
			{
				// controllo se esiste la trasmissione singola associata a corrispondente selezionato
				for(int i = 0; i < trasmissione.trasmissioniSingole.Length; i++) 
				{
					DocsPaWR.TrasmissioneSingola ts = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
					if (ts.corrispondenteInterno.systemId.Equals(corr.systemId)) 
					{
						if(ts.daEliminare) 
						{
							((DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
							return trasmissione;
						}
						else
							return trasmissione;
					}
				}			
			}

            // prima di aggiungere la trasm.ne singola verifico le ragioni cessione
            if (this.esisteRagTrasmCessioneInTrasm(trasmissione))
            {
                this.ddl_ragioni.SelectedIndex = 0;
                this.ddl_ragioni_SelectedIndexChanged(null, null);
                return trasmissione;
            }

			// Aggiungo la trasmissione singola
			DocsPaWR.TrasmissioneSingola trasmissioneSingola = new DocsPAWA.DocsPaWR.TrasmissioneSingola();
			trasmissioneSingola.tipoTrasm = "S";
			trasmissioneSingola.corrispondenteInterno = corr;
			trasmissioneSingola.ragione = TrasmManager.getRagioneSel(this.Page);

            // Imposta la cessione sulla ragione
            if (userAutorizedEditingACL)
                trasmissioneSingola.ragione.cessioneImpostata = this.cbx_cediDiritti.Checked;

			// Aggiungo la lista di trasmissioniUtente
			if(corr is DocsPAWA.DocsPaWR.Ruolo) 
			{
				trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
				DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr);
                if (listaUtenti.Length == 0)
                //Andrea
                {
                    trasmissioneSingola = null;
                    listaExceptionTrasmissioni1.Add("Non è presente alcun utente per la Trasmissione al ruolo: "
                                                    + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + ".");
                }
                //End Andrea
                else
                {
                    //ciclo per utenti se dest è gruppo o ruolo
                    for (int i = 0; i < listaUtenti.Length; i++)
                    {
                        DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                        trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente)listaUtenti[i];
                        //trasmissioneUtente.daNotificare = true;
                        trasmissioneUtente.daNotificare = TrasmManager.getTxRuoloUtentiChecked();
                        trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                    }
                }
			}

			if (corr is DocsPAWA.DocsPaWR.Utente) 
			{
				trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
				DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
				trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente) corr;
                //Andrea - if - else
                if (trasmissioneUtente.utente == null)
                {
                    listaExceptionTrasmissioni1.Add("L utente: " + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + " è inesistente.");
                }
                //End Andrea
                else
                {
                    //trasmissioneUtente.daNotificare = true;
                    trasmissioneUtente.daNotificare = TrasmManager.getTxRuoloUtentiChecked();
                    trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                }
            }

			if (corr is DocsPAWA.DocsPaWR.UnitaOrganizzativa) 
			{
				DocsPaWR.UnitaOrganizzativa theUo = (DocsPAWA.DocsPaWR.UnitaOrganizzativa) corr;
				DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato();
				qca.ragione = trasmissioneSingola.ragione;
				qca.ruolo = UserManager.getRuolo();
                qca.queryCorrispondente = new AddressbookQueryCorrispondente();
                qca.queryCorrispondente.fineValidita = true;
				
				DocsPaWR.Ruolo[] ruoli = UserManager.getRuoliRiferimentoAutorizzati (this.Page, qca, theUo);

                //Andrea
                if (ruoli==null || ruoli.Length == 0)
                {
                    listaExceptionTrasmissioni1.Add("Manca un ruolo di riferimento per la UO: "
                                                        + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                        + ".");
                }
                //End Andrea
                else
                {

                    foreach (DocsPAWA.DocsPaWR.Ruolo r in ruoli)
                        trasmissione = addTrasmissioneSingola(trasmissione, r);
                }
				return trasmissione;
			}

			if (trasmissioneSingola != null)
				trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);
			else
			{
				// In questo caso questa trasmissione non può avvenire perché la struttura non ha utenti
				trasm_strutture_vuote.Add (String.Format ("{0} ({1})", corr.descrizione, corr.codiceRubrica));
			}
			return trasmissione;
		}

		/// <summary>
		/// query Utenti
		/// </summary>
		/// <param name="corr"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.Corrispondente[] queryUtenti(DocsPAWA.DocsPaWR.Corrispondente corr) 
		{
			
			//costruzione oggetto queryCorrispondente
			DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();

			qco.codiceRubrica = corr.codiceRubrica;
			qco.getChildren = true;
		
			qco.idAmministrazione= UserManager.getInfoUtente (this.Page).idAmministrazione;
            qco.fineValidita = true;
			
			//corrispondenti interni
			qco.tipoUtente=DocsPaWR.AddressbookTipoUtente.INTERNO;			
			
			return UserManager.getListaCorrispondenti(this.Page, qco);
		}
		#endregion

		private void SetFocus(System.Web.UI.Control ctrl)
		{
			string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
			RegisterStartupScript("focus", s);
		}

        #region Cessione dei diritti
        /// <summary>
        /// GESTIONE CESSIONE DIRITTI:
        /// abilita la visualizzazione del check-box "Cedi i diritti"
        /// </summary>
        private void abilitaCbx_cediDiritti()
        {
            this.pnl_cediDiritti.Visible = userAutorizedEditingACL;
        }

        /// <summary>
        /// GESTIONE CESSIONE DIRITTI:
        /// verifica se l'utente è abilitato alla funzione ABILITA_CEDI_DIRITTI_FASC
        /// </summary>
        private void checkIsAutorizedEditingACL()
        {
           userAutorizedEditingACL = UserManager.ruoloIsAutorized(this, "ABILITA_CEDI_DIRITTI_FASC");
        }

        /// <summary>
        /// GESTIONE CESSIONI DIRITTI
        /// imposta l'oggetto CESSIONE
        /// </summary>
        private bool setObjCessioneDiritti()
        {
            bool retValue = true;

            if (trasmissione != null && trasmissione.cessione == null)
                trasmissione.cessione = new DocsPAWA.DocsPaWR.CessioneDocumento();

            if (this.userAutorizedEditingACL)  // se l'utente è autorizzato alla cessione
            {
                if (this.isCessioneinTrasm())  // se esiste cessione impostata
                {         
                    retValue = this.isOwner(); // verifica l'eventuale proprietà del fascicolo

                    if (retValue)
                    {
                        trasmissione.salvataConCessione = true;
                        trasmissione.cessione.docCeduto = true;

                        //*******************************************************************************************
                        // MODIFICA IACOZZILLI GIORDANO 30/07/2012
                        // Modifica inerente la cessione dei diritti di un doc da parte di un utente non proprietario ma 
                        // nel ruolo del proprietario, in quel caso non posso valorizzare l'IDPEOPLE  con il corrente perchè
                        // il proprietario può essere un altro utente del mio ruolo, quindi andrei a generare un errore nella security,
                        // devo quindi controllare che nell'idpeople venga inserito l'id corretto del proprietario.
                        string valoreChiaveCediDiritti = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_CEDI_DIRITTI_IN_RUOLO");
                        if (!string.IsNullOrEmpty(valoreChiaveCediDiritti) && valoreChiaveCediDiritti.Equals("1"))
                        {
                            //Devo istanziare una classe utente.
                            string idProprietario = string.Empty;
                            idProprietario = GetAnagUtenteProprietario();
                            Utente _utproprietario = UserManager.GetUtenteByIdPeople(idProprietario);

                            trasmissione.cessione.idPeople = idProprietario;
                            trasmissione.cessione.idRuolo = UserManager.getInfoUtente(this).idGruppo;
                            trasmissione.cessione.userId = _utproprietario.cognome + " " + _utproprietario.nome;


                            if (trasmissione.cessione.idPeople == null || trasmissione.cessione.idPeople == "")
                                trasmissione.cessione.idPeople = idProprietario;

                            if (trasmissione.cessione.idRuolo == null || trasmissione.cessione.idRuolo == "")
                                trasmissione.cessione.idRuolo = UserManager.getInfoUtente(this).idGruppo;

                            if (trasmissione.cessione.userId == null || trasmissione.cessione.userId == "")
                                trasmissione.cessione.userId = _utproprietario.cognome + " " + _utproprietario.nome;
                        }
                        else
                        {
                            //OLD CODE:
                            if (trasmissione.cessione.idPeople == null || trasmissione.cessione.idPeople == "")
                                trasmissione.cessione.idPeople = UserManager.getInfoUtente(this).idPeople;

                            if (trasmissione.cessione.idRuolo == null || trasmissione.cessione.idRuolo == "")
                                trasmissione.cessione.idRuolo = UserManager.getInfoUtente(this).idGruppo;

                            if (trasmissione.cessione.userId == null || trasmissione.cessione.userId == "")
                                trasmissione.cessione.userId = UserManager.getInfoUtente(this).userId;
                        }
                        //*******************************************************************************************
                        // FINE MODIFICA
                        //********************************************************************************************
                        TrasmManager.removeGestioneTrasmissione(this);
                        TrasmManager.setGestioneTrasmissione(this, trasmissione);
                    }
                }
            }
            return retValue;
        }

        /// <summary>
        /// Iacozzilli: Faccio la Get dell'idProprietario del doc!
        /// </summary>
        /// <returns></returns>
        private string GetAnagUtenteProprietario()
        {
            DocumentoDiritto[] listaVisibilita = null;
            DocsPaWR.SchedaDocumento sd = DocumentManager.getDocumentoSelezionato(this);
            string idProprietario = string.Empty;
            listaVisibilita = DocumentManager.getListaVisibilitaSemplificata(this, sd.docNumber, true);
            if (listaVisibilita != null && listaVisibilita.Length > 0)
            {
                for (int i = 0; i < listaVisibilita.Length; i++)
                {
                    if (listaVisibilita[i].accessRights == 0)
                    {
                        return idProprietario = listaVisibilita[i].personorgroup;
                    }
                }
            }

            return "";
        }
        /// <summary>
        /// GESTIONE CESSIONI DIRITTI
        /// verifica se esiste l'oggetto cessione nella trasmissione o se la trasmissione è stata salvata con cessione:
        /// in tal caso imposta la spunta della check-box
        /// </summary>
        private void esisteCessioneinTrasm()
        {
            if (this.azione.Value.Equals(""))
            {
                // gestione con modelli di trasmissione
                if (trasmissione != null &&
                    ((trasmissione.cessione != null && trasmissione.cessione.docCeduto) || trasmissione.salvataConCessione))
                {
                    if (this.cbx_cediDiritti != null)
                        this.cbx_cediDiritti.Checked = true;
                }
            }
            else
            {
                // gestione senza modelli di trasmissione
                impostaCessioneInObjTrasmissione();
            }           
        }

        /// <summary>
        /// Verifica se esistono RUOLI tra i destinatari della trasmissione 
        /// ed imposta quanti sono
        /// </summary>
        private void verificaRuoliDestInTrasmissione()
        {
            foreach (DocsPAWA.DocsPaWR.TrasmissioneSingola trasm in trasmissione.trasmissioniSingole)
            {
                if (trasm.tipoDest.ToString().ToUpper().Equals("RUOLO"))
                    this.numeroRuoliDestInTrasmissione++;
            }
        }

        /// <summary>
        /// Verifica se l'utente è il PROPRIETARIO dell'oggetto
        /// </summary>
        /// <returns>True, False</returns>
        private bool utenteProprietario(string docNumber)
        {
            utils.CessioneDiritti obj = new DocsPAWA.utils.CessioneDiritti();
            return obj.UtenteProprietario(docNumber);
        }

        /// <summary>
        /// Invia un messaggio a video che avvisa l'utente che tra i destinatari della trasmissioni
        /// non ci sono ruoli
        /// </summary>
        private void inviaMsgNoRuoli()
        {
            utils.CessioneDiritti obj = new DocsPAWA.utils.CessioneDiritti();
            if (!ClientScript.IsStartupScriptRegistered("OpenMsg"))
                ClientScript.RegisterStartupScript(this.GetType(), "OpenMsg", obj.InviaMsgNoRuoli());
        }

        /// <summary>
        /// Verifica se ci sono utenti con notifica e quanti sono
        /// </summary>
        private void utentiConNotifica()
        {
            foreach (DocsPAWA.DocsPaWR.TrasmissioneSingola trasm in trasmissione.trasmissioniSingole)
                foreach (DocsPAWA.DocsPaWR.TrasmissioneUtente trasmUt in trasm.trasmissioneUtente)
                    if (trasmUt.daNotificare)
                        this.numeroUtentiConNotifica++;
        }

        /// <summary>
        /// Imposta la proprietà "SalvaConCessione" nell'oggetto Trasmissione corrente
        /// </summary>
        private void impostaCessioneInObjTrasmissione()
        {
            if (this.cbx_cediDiritti != null && this.cbx_cediDiritti.Checked)
            {
                if (!trasmissione.salvataConCessione)
                    trasmissione.salvataConCessione = true;
            }
            else
            {
                if (trasmissione.salvataConCessione)
                    trasmissione.salvataConCessione = false;
            }
        }

        /// <summary>
        /// Imposta lo stato della combobox della cessione dei diritti
        /// </summary>
        /// <param name="ragione">oggetto Ragione</param>
        private void gestioneCbxCessione(DocsPaWR.RagioneTrasmissione ragione)
        {
            if (userAutorizedEditingACL)
            {
                switch (ragione.prevedeCessione)
                {
                    case "N":
                        this.cbx_cediDiritti.Checked = false;
                        this.cbx_cediDiritti.Enabled = false;
                        break;
                    case "W":
                        this.cbx_cediDiritti.Checked = false;
                        this.cbx_cediDiritti.Enabled = true;
                        break;
                    case "R":
                        this.cbx_cediDiritti.Checked = true;
                        this.cbx_cediDiritti.Enabled = false;
                        break;
                    default:
                        this.cbx_cediDiritti.Checked = false;
                        this.cbx_cediDiritti.Enabled = false;
                        break;
                }
            }
        }

        private bool esisteRagTrasmCessioneInTrasm(DocsPAWA.DocsPaWR.Trasmissione trasmissione)
        {
            bool retValue = false;

            // verifica solo se le trasm.ni singole sono più di una
            if (trasmissione.trasmissioniSingole != null && trasmissione.trasmissioniSingole.Length > 0)
            {
                // conta quante trasm.ni singole hanno la ragione con la cessione impostata
                int trasmConCessione = 0;
                foreach (DocsPaWR.TrasmissioneSingola trasmS in trasmissione.trasmissioniSingole)
                    if (trasmS.ragione.cessioneImpostata)
                        trasmConCessione++;

                // è possibile inserire solo una ragione se questa ha la cessione, quindi...
                DocsPaWR.RagioneTrasmissione ragioneAttuale = TrasmManager.getRagioneSel(this);
                if (trasmConCessione > 0) //sono state già inserite ragioni con cessione 
                {
                    // non si può inserire nient'altro, avvisa...
                    RegisterStartupScript("undoTrasm", "<script language=\"javascript\">alert(\"Attenzione! poichè la trasmissione creata prevede la cessione dei diritti, non è possibile inserire alto\");</script>");
                    retValue = true;
                }
                else
                {
                    // non esistono ragioni con cessione, se l'attuale è con cessione avvisa che non si può...
                    if (this.userAutorizedEditingACL)
                    {
                        if (!ragioneAttuale.prevedeCessione.Equals("N") && this.cbx_cediDiritti.Checked)
                        {
                            RegisterStartupScript("undoTrasm", "<script language=\"javascript\">alert(\"Attenzione! non è possibile inserire ragioni di trasmissione con cessione insieme a ragioni senza cessione\");</script>");
                            retValue = true;
                        }
                    }
                }
            }

            return retValue;
        }

        private bool isCessioneinTrasm()
        {
            bool retValue = false;

            if (trasmissione.trasmissioniSingole != null && trasmissione.trasmissioniSingole.Length > 0)
            {
                foreach (DocsPaWR.TrasmissioneSingola trasmS in trasmissione.trasmissioniSingole)
                    if (trasmS.ragione.cessioneImpostata)
                        return true;
            }

            return retValue;
        }

        #endregion 

        protected void btn_EliminaTrasmSalvata_Click(object sender, ImageClickEventArgs e)
        {
            salva("E");
        }

        protected void cbx_cediDiritti_CheckedChanged(object sender, EventArgs e)
        {
            DocsPaWR.RagioneTrasmissione ragione = TrasmManager.getRagioneSel(this);
            ragione.cessioneImpostata = this.cbx_cediDiritti.Checked;

            //
            // Mev Cessione Diritti - Mantieni Scrittura
            if (ragione.cessioneImpostata && !string.IsNullOrEmpty(ragione.mantieniScrittura) && ragione.mantieniScrittura == "1")
            {
                trasmissione = TrasmManager.getGestioneTrasmissione(this);
                trasmissione.mantieniScrittura = true;
                TrasmManager.setGestioneTrasmissione(this, trasmissione);
            }
            //
            // End MEV Cessione Diritti

            if (ragione.cessioneImpostata && !string.IsNullOrEmpty(ragione.mantieniLettura) && ragione.mantieniLettura == "1")
            {
                trasmissione = TrasmManager.getGestioneTrasmissione(this);
                trasmissione.mantieniLettura = true;
                TrasmManager.setGestioneTrasmissione(this, trasmissione);
            }

            TrasmManager.setRagioneSel(this, ragione);
            gerarchia_trasm = ragione.tipoDestinatario.ToString("g").Substring(0, 1);
        }

        private bool isOwner()
        {
            bool retValue = true;
            string accessRights = string.Empty;
            string idGruppoTrasm = string.Empty;
            string tipoDiritto = string.Empty;
            string jscript = string.Empty;
            string IDObject = string.Empty;

            bool isPersonOwner = false;
            bool isGroupOwner = false;

            DocsPaWebService ws = new DocsPaWebService();

            try
            {
                IDObject = fasc.systemID;

                // verifica se è proprietario come UTENTE...
                ws.SelectSecurity(IDObject, UserManager.getInfoUtente(this).idPeople, "= 0", out accessRights, out idGruppoTrasm, out tipoDiritto);
                isPersonOwner = (accessRights.Equals("0"));

                // verifica se è proprietario come RUOLO...
                ws.SelectSecurity(IDObject, UserManager.getInfoUtente(this).idGruppo, "= 255", out accessRights, out idGruppoTrasm, out tipoDiritto);
                isGroupOwner = (accessRights.Equals("255"));

                // se non è il proprietario come utente e ruolo, ritorna true e cederà i diritti acquisiti...
                if (!isPersonOwner && !isGroupOwner)
                    return true;

                // altrimenti verifica la proprietà solo come ruolo...
                if (!isPersonOwner && isGroupOwner)
                {
                    jscript = "<script language='javascript'>alert('Attenzione, impossibile cedere i diritti di proprietà se non si è il creatore del fascicolo!');</script>";

                    if (!ClientScript.IsStartupScriptRegistered("scriptCessione1"))
                        ClientScript.RegisterStartupScript(this.GetType(), "scriptCessione1", jscript);

                    return false;
                }

                // oppure verifica la proprietà solo come utente (caso di fasc personale)

                // DA FARE !!!!!!!!!!!!!!!!!!!!!!!!
            }
            catch
            {
                jscript = "<script language='javascript'>alert('Attenzione, errore nella funzionalità di trasmissione!');</script>";

                if (!ClientScript.IsStartupScriptRegistered("scriptCessione"))
                    ClientScript.RegisterStartupScript(this.GetType(), "scriptCessione", jscript);
                retValue = false;
            }

            return retValue;
        }
	}
}
