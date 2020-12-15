using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using DocsPAWA;
using DocsPAWA.DocsPaWR;

namespace Amministrazione.Gestione_Utenti
{

    public class GestUtenti : System.Web.UI.Page
    {
        #region WebControls e variabili

        private int processiCoinvolti_U = 0;
        private int istazaProcessiCoinvolti_U = 0;
        private string msgLibroFirma;
        //
        // MEV CS 1.4 Esibizione - definizione checkBox cb_abilitatoEsibizione
        protected System.Web.UI.WebControls.CheckBox cb_abilitatoEsibizione;
        // End MEV CS 1.4 Esibizione
        //

        protected System.Web.UI.WebControls.Label lbl_avviso;
        protected System.Web.UI.WebControls.Panel pnl_info;
        protected System.Web.UI.WebControls.Label lbl_position;
        protected System.Web.UI.HtmlControls.HtmlForm Form1;
        protected System.Web.UI.WebControls.TextBox txt_nome;
        protected System.Web.UI.WebControls.TextBox txt_cognome;
        protected System.Web.UI.WebControls.TextBox txt_rubrica;
        protected System.Web.UI.WebControls.TextBox txt_userid;
        protected System.Web.UI.WebControls.TextBox txt_Matricola;
        protected System.Web.UI.WebControls.DataGrid dg_Utenti;
        protected System.Web.UI.WebControls.Label lbl_tit;
        protected System.Web.UI.WebControls.Label lbl_import;
        protected System.Web.UI.HtmlControls.HtmlGenericControl DivDGList;
        protected System.Web.UI.WebControls.Label lbl_msg;
        protected System.Web.UI.WebControls.ImageButton btn_chiudiPnlInfo;
        protected System.Web.UI.WebControls.TextBox txt_email;
        protected System.Web.UI.WebControls.TextBox txt_from_email;
        protected System.Web.UI.WebControls.DropDownList cb_amm;
        protected System.Web.UI.WebControls.CheckBox cb_abilitato;
        protected System.Web.UI.WebControls.CheckBox cb_abilitatoCentroServizi;
        protected System.Web.UI.WebControls.TextBox txt_dominio;
        protected System.Web.UI.WebControls.DropDownList ddl_notifica;
        protected System.Web.UI.WebControls.Button btn_UtConn;
        protected System.Web.UI.WebControls.TextBox txt_sede;
        protected System.Web.UI.WebControls.Button btn_importa;


        protected System.Web.UI.WebControls.CheckBox chkUserPasswordNeverExpire;
        protected System.Web.UI.HtmlControls.HtmlTableRow trIntegrazioneLdap;
        protected System.Web.UI.WebControls.Button btnLdapSync;
        protected System.Web.UI.WebControls.Label lblSyncronizeLdap;
        protected System.Web.UI.WebControls.CheckBox chkSyncronizeLdap;
        protected System.Web.UI.WebControls.CheckBox chkAutenticatoLdap;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hdIdSincronizzazioneLdap;

        //protected System.Web.UI.WebControls.CheckBox chkIsEnabledSmartClient;
        protected System.Web.UI.WebControls.RadioButton rdbIsEnabledActiveX;
        protected System.Web.UI.WebControls.RadioButton rdbIsEnabledSmartClient;
        protected System.Web.UI.WebControls.RadioButton rdbIsEnabledJavaApplet;
        protected System.Web.UI.WebControls.RadioButton rdbIsEnabledHTML5Socket;
        protected System.Web.UI.WebControls.RadioButton rdbDisableAll;

        protected System.Web.UI.WebControls.CheckBox chkSmartClientConvertPdfOnScan;

        //-----------------------------------------------------------------------
        private DataSet dataSet = new DataSet();
        public int UtAbilit = 0;
        protected System.Web.UI.WebControls.Button btn_newUt;
        protected System.Web.UI.HtmlControls.HtmlTable tblPulsanti;
        protected System.Web.UI.WebControls.Button btn_aggiungi;
        protected System.Web.UI.WebControls.Button btn_ruoli;
        protected System.Web.UI.WebControls.TextBox txt_ConfPassword;
        protected System.Web.UI.WebControls.TextBox txt_password;
        protected System.Web.UI.WebControls.DropDownList ddl_ricerca;
        protected System.Web.UI.WebControls.TextBox txt_ricerca;
        protected System.Web.UI.WebControls.Button btn_find;
        protected System.Web.UI.WebControls.ImageButton btn_vociMenu;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModal;
        public int UtDisabilit = 0;
        protected DocsPAWA.DocsPaWR.InfoUtenteAmministratore datiAmministratore;

        protected System.Web.UI.WebControls.Label lblClientModelProcessor;
        protected System.Web.UI.WebControls.DropDownList cboClientModelProcessor;
        protected System.Web.UI.WebControls.DropDownList ddlLabelPrinterType;

        protected System.Web.UI.HtmlControls.HtmlTableRow trMatricola;
        protected System.Web.UI.WebControls.CheckBox cbx_chiavi;
        protected Utilities.MessageBox msg_conferma;
        protected Utilities.MessageBox msg_modifica;

        #endregion

        #region Page Load / Inizialize

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_PreRender(object sender, EventArgs e)
        {
            string utente;

            for (int i = 0; i < this.dg_Utenti.Items.Count; i++)
            {
                if (this.dg_Utenti.Items[i].ItemIndex >= 0)
                {
                    // verifica se l'utente è disabilitato
                    if (this.dg_Utenti.Items[i].Cells[10].Text.Equals("Y"))
                    {
                        this.dg_Utenti.Items[i].ForeColor = System.Drawing.Color.Red;
                        this.UtDisabilit += 1;
                    }

                    utente = this.dg_Utenti.Items[i].Cells[4].Text + " " + this.dg_Utenti.Items[i].Cells[5].Text;
                    //controllo il tipo di utente che si vuole eliminare e gestisco il pulsante 'ELIMINA'

                    //verifica il tipo amministratore
                    string idTipoAmmUtente = dg_Utenti.Items[i].Cells[8].Text;
                    string tipoAmministratore = datiAmministratore.tipoAmministratore;
                    string alert = "";
                    switch (tipoAmministratore)
                    {
                        case "1":  //system administrator
                            break;
                        case "2": //super administrator
                            if (idTipoAmmUtente.Equals("1"))
                            {
                                //non può essere eliminato
                                alert = "Attenzione, un Amministratore di livello superiore non può essere eliminato con il profilo che si possiede.";
                            }
                            break;
                        case "3": //user administrator
                            if (idTipoAmmUtente.Equals("1") || idTipoAmmUtente.Equals("2") || idTipoAmmUtente.Equals("3"))
                            {
                                alert = "Attenzione, un Amministratore non può essere eliminato con il profilo che si possiede.";
                            }
                            break;
                    }
                    //
                    if (alert.Equals(""))
                        this.dg_Utenti.Items[i].Cells[16].Attributes.Add("onclick", "if (!window.confirm('Attenzione!\\nsei sicuro di voler eliminare\\n" + utente.Replace("'", "\\'") + "?')) {return false};");
                    else
                        this.dg_Utenti.Items[i].Cells[16].Attributes.Add("onclick", "alert('" + alert + "'); return false;");
                }
            }

            // solo il super amministratore può gestire le chiavi di configurazione
            if (this.cb_amm.SelectedValue.Equals("2"))
                this.cbx_chiavi.Visible = true;
            else
                this.cbx_chiavi.Visible = false;

            this.SetLdapControlsEnabled();
        }

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            //
            // Mev CS 1.4 - Esibizione
            // La checkBox cb_abilitatoEsibizione sarà visibile o meno in ragione della chiave di web.config VISUALIZZA_CB_ESIBIZIONE
            this.cb_abilitatoEsibizione.Visible = this.isCBEsibizioneVisible();
            // End Mev CS 1.4
            //

            // INTEGRAZIONE PITRE-PARER
            if (this.IsConservazionePARER())
            {
                this.cb_abilitatoCentroServizi.Visible = false;
                this.cb_abilitatoEsibizione.Visible = false;
            }

            Session["AdminBookmark"] = "Utenti";

            // Risoluzione problema valore campi password
            this.txt_password.Attributes.Add("value", this.txt_password.Text);
            this.txt_ConfPassword.Attributes.Add("value", this.txt_ConfPassword.Text);

            //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
            if (Session.IsNewSession)
            {
                Response.Redirect("../Exit.aspx?FROM=EXPIRED");
            }

            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            if (!ws.CheckSession(Session.SessionID))
            {
                Response.Redirect("../Exit.aspx?FROM=ABORT");
            }
            // ---------------------------------------------------------------

            btn_UtConn.Attributes.Add("onclick", "javascript: apriPopupUtConn();");

            this.SetDefaultBtn_Ricerca();

            //Viene impostato il focus sulla textbox userid
            //e viene impostata la funzione del tasto invio dul pulsante aggiungi
            if (pnl_info.Visible)
            {
                Utils.DefaultButton(this.Page, ref this.txt_userid, ref btn_aggiungi);
                Utils.DefaultButton(this.Page, ref this.txt_Matricola, ref btn_aggiungi);
                Utils.DefaultButton(this.Page, ref this.txt_dominio, ref btn_aggiungi);
                Utils.DefaultButton(this.Page, ref this.txt_password, ref btn_aggiungi);
                Utils.DefaultButton(this.Page, ref this.txt_ConfPassword, ref btn_aggiungi);
                Utils.DefaultButton(this.Page, ref this.txt_nome, ref btn_aggiungi);
                Utils.DefaultButton(this.Page, ref this.txt_cognome, ref btn_aggiungi);
                Utils.DefaultButton(this.Page, ref this.txt_rubrica, ref btn_aggiungi);
                Utils.DefaultButton(this.Page, ref this.txt_email, ref btn_aggiungi);
                Utils.DefaultButton(this.Page, ref this.txt_sede, ref btn_aggiungi);
                Utils.DefaultButton(this.Page, ref this.ddl_notifica, ref btn_aggiungi);
                Utils.DefaultButton(this.Page, ref this.cb_amm, ref btn_aggiungi);
                Utils.DefaultButton(this.Page, ref this.txt_from_email, ref btn_aggiungi);
            }

            DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
            datiAmministratore = session.getUserAmmSession();

            if (!IsPostBack)
            {
                try
                {
                    this.SetFocus(this.txt_ricerca);
                    lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");


                    this.LoadDDLAmministratori();

                    // Caricamento combo dei client word processor
                    this.FetchComboClientModelProcessors();

                    this.Inizialize(false, null, null);
                }
                catch (Exception ex)
                {
                    //this.lbl_avviso.Text = ex.Message.ToString();
                    this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
                    this.GUI("ResetAll");
                }
            }
            else
            {
                // gestione del valore di ritorno della modal Dialog 
                if (this.hd_returnValueModal.Value != null && this.hd_returnValueModal.Value != string.Empty && this.hd_returnValueModal.Value != "undefined" && this.hd_returnValueModal.Value.Equals("noAmmTitolario"))
                {
                    this.hd_returnValueModal.Value = "";
                    this.cb_amm.SelectedIndex = 0;
                    this.btn_vociMenu.Visible = false;
                    this.dg_Utenti.SelectedItem.Cells[8].Text = "0";
                    this.AlertJS("Attenzione,\\nconfermare le modifiche effettuate tramite il tasto 'Modifica' in basso a destra.");
                }
            }

            if (Session["ImportazioneAvvenuta"] != null)
            {
                Session.Remove("ImportazioneAvvenuta");
                this.Inizialize(false, null, null);
            }

            if (!IsPostBack)
            {
                DocsPaWebService wss = new DocsPaWebService();

                ddlLabelPrinterType.DataTextField = "Description";
                ddlLabelPrinterType.DataValueField = "Id";


                ddlLabelPrinterType.DataSource = wss.AmministrazioneGetDispositivoStampaEtichetta();
                ddlLabelPrinterType.DataBind();
                ddlLabelPrinterType.Items.Insert(0, new ListItem("", "null"));

                DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                rdbIsEnabledJavaApplet.Visible = wws.EnableJavaAppletOption();
                rdbIsEnabledHTML5Socket.Visible = wws.EnableHTML5SocketOption();
            }

            this.chkSmartClientConvertPdfOnScan.Enabled = (this.rdbIsEnabledSmartClient.Checked || this.rdbIsEnabledJavaApplet.Checked || this.rdbIsEnabledHTML5Socket.Checked);
        }

        /// <summary>
        /// Inizializzazione dati
        /// </summary>
        /// <param name="ricerca"></param>
        /// <param name="ricercaPer"></param>
        /// <param name="testoDaRicercare"></param>
        private void Inizialize(bool ricerca, string ricercaPer, string testoDaRicercare)
        {
            try
            {
                // Inizializzazione campi configurazione password
                //this.LoadPasswordConfigurations();

                string codAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");
                Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
                theManager.CurrentIDAmm(codAmm);

                if (theManager.getIDAmministrazione() != null && theManager.getIDAmministrazione() != string.Empty)
                {
                    this.GestListaCorrente(theManager, ricerca, ricercaPer, testoDaRicercare);
                }
                else
                {
                    this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
                    this.GUI("ResetAll");
                }

                theManager = null;
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theManager"></param>
        /// <param name="ricerca"></param>
        /// <param name="ricercaPer"></param>
        /// <param name="testoDaRicercare"></param>
        private void GestListaCorrente(Manager.UtentiManager theManager, bool ricerca, string ricercaPer, string testoDaRicercare)
        {
            if (ricerca)
            {
                theManager.ListaUtenti(theManager.getIDAmministrazione(), ricercaPer, testoDaRicercare);
            }
            else
            {
                theManager.ListaUtenti(theManager.getIDAmministrazione());
            }

            if (theManager.getListaUtenti() != null && theManager.getListaUtenti().Count > 0)
            {
                this.LoadUtenti(theManager.getListaUtenti());
            }
            else
            {
                if (ricerca)
                {
                    //this.RemoveSessionDataSetUtenti();
                    this.GUI("RicercaNull");
                }
                else
                {
                    this.lbl_avviso.Text = "Nessun utente presente sul database";
                    this.GUI("InsPrimoUtente");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctrl"></param>
        private void SetFocus(System.Web.UI.Control ctrl)
        {
            string s = "<SCRIPT language='javascript'>if (document.getElementById('" + ctrl.ID + "')) document.getElementById('" + ctrl.ID + "').focus(); </SCRIPT>";
            RegisterStartupScript("focus", s);
        }

        /// <summary>
        /// Imposta il tasto di default che permette la Post al server in merito alla sezione RICERCA
        /// </summary>
        private void SetDefaultBtn_Ricerca()
        {
            DocsPAWA.Utils.DefaultButton(this, ref this.txt_ricerca, ref this.btn_find);
        }

        /// <summary>
        /// Imposta il tasto di default che permette la Post al server in merito alla sezione INFO Utente (Inserimento/Modifica)
        /// </summary>
        private void SetDefaultBtn_InsMod()
        {
        }

        #endregion

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
            this.ddl_ricerca.SelectedIndexChanged += new System.EventHandler(this.ddl_ricerca_SelectedIndexChanged);
            this.btn_find.Click += new System.EventHandler(this.btn_find_Click);
            this.btn_newUt.Click += new System.EventHandler(this.btn_newUt_Click);
            this.dg_Utenti.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_Utenti_ItemCommand);
            this.dg_Utenti.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_Utenti_PageIndexChanged);
            this.btn_chiudiPnlInfo.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudiPnlInfo_Click);
            this.cb_amm.SelectedIndexChanged += new System.EventHandler(this.cb_amm_SelectedIndexChanged);
            this.btn_aggiungi.Click += new System.EventHandler(this.btn_aggiungi_Click);
            this.txt_userid.TextChanged += new System.EventHandler(this.txt_userid_TextChanged);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new EventHandler(this.Page_PreRender);
        }
        #endregion

        #region Interfaccia grafica

        private void GUI(string from)
        {
            switch (from)
            {
                case "RicercaNull":
                    this.lbl_tit.Text = "Nessun utente per questa ricerca!";		// titolo
                    this.btn_find.Visible = true;						// pulsante Cerca utenti
                    this.btn_UtConn.Visible = true;						// pulsante Utenti connessi
                    this.btn_newUt.Visible = true;						// pulsante Nuovo utente
                    this.dg_Utenti.Visible = false;						// datagrid
                    this.pnl_info.Visible = false;						// info
                    this.lbl_msg.Text = "";								// messaggi
                    this.lbl_avviso.Text = "";
                    break;
                case "ResetAll":
                    this.btn_find.Visible = false;						// pulsante Cerca utenti
                    this.btn_UtConn.Visible = false;					// pulsante Utenti connessi
                    this.btn_newUt.Visible = false;						// pulsante Nuovo utente
                    this.dg_Utenti.Visible = false;						// datagrid
                    this.pnl_info.Visible = false;						// info
                    //if (this.IsSupportedPasswordConfig())
                    //    this.tblRowPasswordMng.Visible = false;
                    this.SetClientModelControlsVisibility();
                    this.SetMatricolaControlsVisibility();
                    this.tblPulsanti.Visible = false;

                    this.rdbIsEnabledSmartClient.Checked = false;
                    this.rdbIsEnabledJavaApplet.Checked = false;
                    this.rdbIsEnabledHTML5Socket.Checked = false;
                    this.rdbDisableAll.Checked = true;

                    this.chkSmartClientConvertPdfOnScan.Checked = false;
                    this.chkSmartClientConvertPdfOnScan.Enabled = false;

                    break;
                case "InsPrimoUtente":
                    this.btn_find.Visible = false;						// pulsante Cerca utenti
                    this.btn_UtConn.Visible = true;					// pulsante Utenti connessi
                    this.btn_newUt.Visible = true;						// pulsante Nuovo utente
                    this.dg_Utenti.Visible = false;						// datagrid
                    this.pnl_info.Visible = true;						// info
                    this.btn_ruoli.Visible = false;						// pulsante visualizza ruoli
                    this.btn_aggiungi.Enabled = true;					// pulsante Aggiungi/Modifica	
                    this.btn_aggiungi.Text = "Aggiungi";
                    this.lbl_msg.Text = "";								// messaggi
                    this.lbl_avviso.Text = "";
                    this.chkUserPasswordNeverExpire.Visible = this.IsSupportedPasswordConfig();
                    this.hdIdSincronizzazioneLdap.Value = string.Empty;
                    this.SetClientModelControlsVisibility();
                    this.SetMatricolaControlsVisibility();
                    this.RipulisceTutto();
                    this.SetFocus(this.txt_userid);
                    break;
                case "ListaUtenti":
                    this.btn_find.Visible = true;						// pulsante Cerca utenti
                    this.btn_UtConn.Visible = true;						// pulsante Utenti connessi
                    this.btn_newUt.Visible = true;						// pulsante Nuovo utente
                    this.dg_Utenti.Visible = true;						// datagrid
                    this.dg_Utenti.SelectedIndex = -1;
                    this.pnl_info.Visible = false;						// info
                    this.lbl_msg.Text = "";								// messaggi
                    this.lbl_avviso.Text = "";
                    this.SetFocus(this.txt_ricerca);
                    break;
                case "DatiUtente":
                    this.btn_find.Visible = true;						// pulsante Cerca utenti
                    this.btn_UtConn.Visible = true;						// pulsante Utenti connessi
                    this.btn_newUt.Visible = true;						// pulsante Nuovo utente
                    this.dg_Utenti.Visible = true;						// datagrid
                    this.pnl_info.Visible = true;						// info
                    this.btn_ruoli.Visible = false;						// pulsante visualizza ruoli
                    this.btn_aggiungi.Enabled = true;					// pulsante Aggiungi/Modifica
                    this.btn_aggiungi.Text = "Modifica";
                    this.lbl_msg.Text = "";								// messaggi
                    this.lbl_avviso.Text = "";
                    this.chkUserPasswordNeverExpire.Visible = this.IsSupportedPasswordConfig();
                    this.SetClientModelControlsVisibility();
                    this.SetMatricolaControlsVisibility();
                    this.chkSmartClientConvertPdfOnScan.Enabled = (this.rdbIsEnabledSmartClient.Checked || this.rdbIsEnabledJavaApplet.Checked || this.rdbIsEnabledHTML5Socket.Checked);

                    this.SetFocus(this.txt_userid);
                    break;
                case "DatiUtenteConnesso":
                    this.btn_find.Visible = true;						// pulsante Cerca utenti
                    this.btn_UtConn.Visible = true;						// pulsante Utenti connessi
                    this.btn_newUt.Visible = true;						// pulsante Nuovo utente
                    this.dg_Utenti.Visible = true;						// datagrid
                    this.pnl_info.Visible = true;						// info
                    this.btn_ruoli.Visible = false;						// pulsante visualizza ruoli
                    this.btn_aggiungi.Enabled = false;					// pulsante Aggiungi/Modifica
                    this.btn_aggiungi.Text = "Modifica";
                    this.lbl_avviso.Text = "";							// messaggi
                    this.chkUserPasswordNeverExpire.Visible = this.IsSupportedPasswordConfig();
                    this.SetClientModelControlsVisibility();
                    this.SetMatricolaControlsVisibility();
                    this.chkSmartClientConvertPdfOnScan.Enabled = (this.rdbIsEnabledSmartClient.Checked || this.rdbIsEnabledJavaApplet.Checked || this.rdbIsEnabledHTML5Socket.Checked);
                    this.lbl_msg.Text = "Attenzione, l'utente è al momento connesso a DocsPA. Impossibile modificare i dati.";
                    break;
                case "InsNuovoUtente":
                    this.btn_find.Visible = true;						// pulsante Cerca utenti
                    this.btn_UtConn.Visible = true;						// pulsante Utenti connessi
                    this.btn_newUt.Visible = false;						// pulsante Nuovo utente
                    this.dg_Utenti.Visible = true;						// datagrid
                    this.pnl_info.Visible = true;						// info
                    this.btn_ruoli.Visible = false;						// pulsante visualizza ruoli
                    this.btn_aggiungi.Enabled = true;					// pulsante Aggiungi/Modifica
                    this.btn_aggiungi.Text = "Aggiungi";
                    this.lbl_msg.Text = "";								// messaggi
                    this.lbl_avviso.Text = "";
                    this.chkUserPasswordNeverExpire.Visible = this.IsSupportedPasswordConfig();
                    this.SetClientModelControlsVisibility();
                    this.SetMatricolaControlsVisibility();
                    this.hdIdSincronizzazioneLdap.Value = string.Empty;
                    this.RipulisceTutto();
                    string dominio = this.GetDominioAmministrazione();
                    if (!string.IsNullOrEmpty(dominio))
                        txt_dominio.Text = dominio;
                    this.SetFocus(this.txt_userid);

                    this.ddlLabelPrinterType.SelectedIndex = this.ddlLabelPrinterType.Items.IndexOf(this.ddlLabelPrinterType.Items.FindByValue("null"));
                    break;
            }
        }

        private void RipulisceTutto()
        {
            this.lbl_avviso.Text = "";
            this.lbl_msg.Text = "";

            this.lbl_tit.Text = "Lista Utenti";
            this.txt_ricerca.Text = "";

            this.txt_userid.Text = "";
            this.txt_Matricola.Text = "";

            this.txt_cognome.Text = "";
            this.txt_nome.Text = "";

            this.txt_password.Text = string.Empty;
            this.txt_ConfPassword.Text = string.Empty;
            this.txt_password.Attributes.Add("value", string.Empty);
            this.txt_ConfPassword.Attributes.Add("value", string.Empty);

            this.txt_rubrica.Text = "";
            this.txt_sede.Text = "";
            this.txt_email.Text = "";
            this.txt_from_email.Text = "";
            this.txt_dominio.Text = "";
            this.chkSyncronizeLdap.Checked = false;
            this.chkAutenticatoLdap.Checked = false;

            this.SetClientModelProcessor("0");

            this.rdbIsEnabledSmartClient.Checked = false;
            this.rdbIsEnabledJavaApplet.Checked = false;
            this.rdbIsEnabledHTML5Socket.Checked = false;
            this.rdbDisableAll.Checked = true;

            this.chkSmartClientConvertPdfOnScan.Checked = false;
            this.chkSmartClientConvertPdfOnScan.Enabled = false;

            this.cb_abilitato.Checked = false;
            this.cb_abilitatoCentroServizi.Checked = false;

            //
            // MEV CS 1.4 - Esibizione
            this.cb_abilitatoEsibizione.Checked = false;
            // End Mev CS 1.4 - Esibizione
            // 

            //gestione menu
            LoadDDLAmministratori();
            this.cb_amm.SelectedIndex = 0;
            this.btn_vociMenu.Visible = false;

            this.ddl_notifica.SelectedIndex = this.ddl_notifica.Items.IndexOf(this.ddl_notifica.Items.FindByValue("null"));

            this.dg_Utenti.SelectedIndex = -1;
            this.ddlLabelPrinterType.SelectedIndex = this.ddlLabelPrinterType.Items.IndexOf(this.ddlLabelPrinterType.Items.FindByValue("null"));
        }

        private void LoadDDLAmministratori()
        {
            this.cb_amm.Items.Clear();
            string tipoAmm = datiAmministratore.tipoAmministratore.ToString();
            ListItem items = new ListItem("No", "0");
            this.cb_amm.Items.Add(items);
            switch (tipoAmm)
            {
                case "1": // system administrator
                    items = new ListItem("Super Administrator", "2");
                    this.cb_amm.Items.Add(items);
                    items = new ListItem("User Administrator", "3");
                    this.cb_amm.Items.Add(items);
                    break;

                case "2": // super administrator
                    items = new ListItem("Super Administrator", "2");
                    this.cb_amm.Items.Add(items);
                    items = new ListItem("User Administrator", "3");
                    this.cb_amm.Items.Add(items);
                    break;
                case "3": // user  administrator
                default:
                    break;
            }
        }

        #endregion

        #region DataGrid utenti

        protected void dg_Utenti_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
                if (e.Item.Cells.Count > 0)
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        private void IniDataSet()
        {
            //DataSet contenente la lista degli utenti
            dataSet = new DataSet();
            dataSet.Tables.Add("Utente");

            DataColumn dc = new DataColumn("idCorrGlobale");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("idPeople");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("userid");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("password");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("cognome");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("nome");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("codice");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("codiceRubrica");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("amministratore");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("email");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("abilitato");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("dominio");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("sede");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("notifica");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("idAmministrazione");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("nessunaScadenzaPassword");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("fromEmail");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("SincronizzaLdap");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("AutenticatoLdap");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("IdSincronizzazioneLdap");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("IdClientSideModelProcessor");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("IsEnabledSmartClient", typeof(Boolean));
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("ApplyPdfConvertionOnScan", typeof(Boolean));
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("DispositivoStampa", typeof(Int32));
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("AbilitatoCentroServizi", typeof(Boolean));
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("Matricola");
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("AbilitatoChiaviConfig", typeof(Boolean));
            dataSet.Tables["Utente"].Columns.Add(dc);

            dc = new DataColumn("Cha_Tipo_Componenti");
            dataSet.Tables["Utente"].Columns.Add(dc);

            //
            // MEV CS 1.4 - Esibizione
            dc = new DataColumn("AbilitatoEsibizione", typeof(Boolean));
            dataSet.Tables["Utente"].Columns.Add(dc);
            // End MEV cs 1.4 - Esibizione
            //

        }

        /// <summary>
        /// Caricamento dati in griglia
        /// </summary>
        private void FillDataGrid(int requestedPage)
        {
            DataSet dataSet = this.GetSessionDataSetUtenti();
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            string codAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");

            DataView dv = dataSet.Tables["Utente"].DefaultView;
            dv.Sort = "cognome ASC";

            this.dg_Utenti.CurrentPageIndex = requestedPage;

            this.dg_Utenti.DataSource = dv;
            this.dg_Utenti.DataBind();

            int numUtenti = ws.NumeroUtentiAttivi(codAmm);
            this.lbl_tit.Text = "Lista Utenti - Totali: " + Convert.ToString(this.GetSessionDataSetUtenti().Tables["Utente"].Rows.Count);
            if (numUtenti != 0)
                this.lbl_tit.Text += " di cui " + numUtenti + " attivi";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listaUtenti"></param>
        private void LoadUtenti(ArrayList listaUtenti)
        {
            try
            {
                this.GUI("ListaUtenti");
                this.IniDataSet();

                DataRow row;
                foreach (DocsPAWA.DocsPaWR.OrgUtente utente in listaUtenti)
                {
                    row = dataSet.Tables["Utente"].NewRow();
                    row["idCorrGlobale"] = utente.IDCorrGlobale;
                    row["idPeople"] = utente.IDPeople;
                    row["userid"] = utente.UserId;
                    row["Matricola"] = utente.Matricola;
                    row["password"] = utente.Password;
                    row["nome"] = utente.Nome;
                    row["cognome"] = utente.Cognome;
                    row["codice"] = utente.Codice;
                    row["codiceRubrica"] = utente.CodiceRubrica;
                    row["amministratore"] = utente.Amministratore;
                    row["email"] = utente.Email;
                    row["fromEmail"] = utente.FromEmail;
                    row["abilitato"] = utente.Abilitato;
                    row["dominio"] = utente.Dominio;
                    row["sede"] = utente.Sede;
                    row["notifica"] = utente.NotificaTrasm;
                    row["idAmministrazione"] = utente.IDAmministrazione;
                    row["nessunaScadenzaPassword"] = utente.NessunaScadenzaPassword;
                    row["SincronizzaLdap"] = utente.SincronizzaLdap.ToString();
                    row["AutenticatoLdap"] = utente.AutenticatoInLdap.ToString();
                    row["IdSincronizzazioneLdap"] = utente.IdSincronizzazioneLdap;
                    row["IdClientSideModelProcessor"] = utente.IdClientSideModelProcessor;
                    row["IsEnabledSmartClient"] = utente.SmartClientConfigurations.IsEnabled;
                    row["ApplyPdfConvertionOnScan"] = utente.SmartClientConfigurations.ApplyPdfConvertionOnScan;
                    row["DispositivoStampa"] = utente.DispositivoStampa;
                    row["AbilitatoCentroServizi"] = utente.AbilitatoCentroServizi;
                    row["AbilitatoChiaviConfig"] = utente.AbilitatoChiaviConfigurazione;
                    row["Cha_Tipo_Componenti"] = utente.SmartClientConfigurations.ComponentsType;

                    //
                    // MEV CS 1.4 - Esibizione
                    row["AbilitatoEsibizione"] = utente.AbilitatoEsibizione;
                    // End MEV CS 1.4
                    //

                    dataSet.Tables["Utente"].Rows.Add(row);
                }

                // Impostazione dataset in sessione
                this.SetSessionDataSetUtenti(dataSet);

                // Caricamento dati griglia
                this.FillDataGrid(0);

                // Visualizza subito i dati se è l'unico utente nel datagrid
                if (listaUtenti.Count == 1)
                {
                    this.dg_Utenti.SelectedIndex = 0;
                    this.LoadDatiUtente(this.dg_Utenti.Items[0]);
                }
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// dg_Utenti ItemCommand
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void dg_Utenti_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.CommandName.Equals("Select"))
            {
                this.LoadDatiUtente(e);
            }
        }

        /// <summary>
        /// dg_Utenti PreRender
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dg_Utenti_PreRender(object sender, System.EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void dg_Utenti_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            try
            {
                this.FillDataGrid(e.NewPageIndex);

                this.GUI("ListaUtenti");
            }
            catch
            {

            }
        }

        /// <summary>
        /// Aggiornamento datagrid dopo le modifiche utente
        /// </summary>
        private void AggiornaDGDopoMod(DocsPAWA.DocsPaWR.OrgUtente utente)
        {
            try
            {
                DataSet ds = this.GetSessionDataSetUtenti();

                DataView dv = ds.Tables["Utente"].DefaultView;
                int currentPageIndex = 0;
                if (Session["currentPageIndex"] != null)
                {
                    currentPageIndex = (int)Session["currentPageIndex"];
                    Session.Remove("currentPageIndex");
                }
                else
                    if (this.dg_Utenti != null && this.dg_Utenti.SelectedItem != null)
                        currentPageIndex = dg_Utenti.CurrentPageIndex;

                //dv.RowFilter="userid = '" + utente.UserId + "'";
                if (this.dg_Utenti != null && this.dg_Utenti.SelectedItem != null)
                    dv.RowFilter = "userid = '" + this.dg_Utenti.SelectedItem.Cells[2].Text + "'";
                else
                {
                    if (Session["datagridUser"] != null)
                    {
                        DataGridItem dg_User = (DataGridItem)Session["datagridUser"];
                        dv.RowFilter = "userid = '" + dg_User.Cells[2].Text + "'";
                    }
                }

                if (dv.Count > 0)
                {
                    DataRow row = dv[0].Row;
                    row["idCorrGlobale"] = utente.IDCorrGlobale;
                    row["idPeople"] = utente.IDPeople;
                    row["userid"] = utente.UserId;
                    row["password"] = utente.Password;
                    row["nome"] = utente.Nome;
                    row["cognome"] = utente.Cognome;
                    row["codice"] = utente.Codice;
                    row["codiceRubrica"] = utente.CodiceRubrica;
                    row["amministratore"] = utente.Amministratore;
                    row["email"] = utente.Email;
                    row["fromEmail"] = utente.FromEmail;
                    row["abilitato"] = (utente.Abilitato.Equals("0")) ? "Y" : "N";
                    //Commentato per gestione nuovo formato dominio
                    //row["dominio"] = utente.Dominio + @"\" + utente.UserId;
                    row["dominio"] = utente.Dominio;
                    row["sede"] = utente.Sede;
                    row["notifica"] = utente.NotificaTrasm;
                    row["idAmministrazione"] = utente.IDAmministrazione;
                    row["nessunaScadenzaPassword"] = utente.NessunaScadenzaPassword;
                    row["SincronizzaLdap"] = utente.SincronizzaLdap.ToString();
                    row["IdSincronizzazioneLdap"] = utente.IdSincronizzazioneLdap;
                    row["AutenticatoLdap"] = utente.AutenticatoInLdap;
                    row["IdClientSideModelProcessor"] = utente.IdClientSideModelProcessor;
                    row["IsEnabledSmartClient"] = utente.SmartClientConfigurations.IsEnabled;
                    row["Cha_Tipo_Componenti"] = utente.SmartClientConfigurations.ComponentsType;
                    row["ApplyPdfConvertionOnScan"] = utente.SmartClientConfigurations.ApplyPdfConvertionOnScan;
                    if (utente.DispositivoStampa == null)
                        row["DispositivoStampa"] = DBNull.Value;
                    else
                        row["DispositivoStampa"] = utente.DispositivoStampa;
                    row["AbilitatoCentroServizi"] = utente.AbilitatoCentroServizi;

                    //
                    // MEV CS 1.4 - Esibizione
                    row["AbilitatoEsibizione"] = utente.AbilitatoEsibizione;
                    // End MEV CS 1.4
                    //

                    row["Matricola"] = utente.Matricola;
                }

                dv.RowFilter = string.Empty;

                this.FillDataGrid(currentPageIndex);

                this.dg_Utenti_PreRender(null, null);

                this.GUI("ListaUtenti");
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
                this.GUI("ResetAll");
            }
        }

        #endregion

        #region Tasti

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_aggiungi_Click(object sender, System.EventArgs e)
        {
            switch (this.btn_aggiungi.Text)
            {
                case "Aggiungi":
                    if (this.VerificaPassword("Aggiungi"))
                        this.InserimentoUtente();
                    break;
                case "Modifica":
                    this.ModificaUtente();
                    break;
            }
        }

        private void btn_newUt_Click(object sender, System.EventArgs e)
        {
            this.GUI("InsNuovoUtente");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_chiudiPnlInfo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.GUI("ListaUtenti");
        }

        #endregion

        #region Gestione dati utente

        protected void msg_conferma_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                Session.Add("ok_multiAmm", true);
                InserimentoUtente();
            }
            else
                Session.Remove("ok_multiAmm");
        }

        protected void msg_modifica_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                //Session.Add("ok_multiAmm", true);
                ModificaUtente();
            }
            else
            {
                Session.Remove("ok_multiAmm");
                Session.Remove("coinvoltoInLF");
            }
        }

        /// <summary>
        /// INSERIMENTO
        /// </summary>
        private void InserimentoUtente()
        {
            bool isUserMultiAmm = false;
            bool continua = true;
            string pwdUser = string.Empty;
            try
            {
                if (this.VerificaCampiObbligatori())
                {
                    if (this.VerificaCampiInseriti())
                    {
                        Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
                        DocsPAWA.DocsPaWR.OrgUtente utente = new DocsPAWA.DocsPaWR.OrgUtente();
                        //MEV Multi-amministrazione Verifica se utente in multi amminstrazione
                        //<
                        isUserMultiAmm = (VerificaUtenteMultiAmministrazione(this.txt_userid.Text.Trim().ToUpper()));

                        if (Session["ok_multiAmm"] == null)
                        {
                            if (isUserMultiAmm)
                            {
                                //TODO SHOW ALERT MESSAGE....
                                continua = false;
                                msg_conferma.Confirm("E’ presente almeno un utente con stessa UserID in un’altra amministrazione, vuoi creare un utente multiammministrazione? Attenzione: la password verrà ereditata dall’utente già esistente.");
                            }
                        }
                        else
                            Session.Remove("ok_multiAmm");
                        //>

                        if (continua)
                        {
                            utente.IDCorrGlobale = null;
                            utente.IDPeople = null;
                            utente.UserId = this.txt_userid.Text.Trim().ToUpper();
                            utente.Matricola = this.txt_Matricola.Text.Trim();
                            //MEV Multi-amministrazione
                            //inizio
                            if (isUserMultiAmm)
                            {
                                pwdUser = GetPasswordUtenteMultiAmm(utente.UserId);
                            }
                            //else
                            utente.Password = this.txt_password.Text.Trim();
                            //fine
                            utente.Nome = this.txt_nome.Text.Trim();
                            utente.Cognome = this.txt_cognome.Text.Trim();
                            utente.CodiceRubrica = this.txt_rubrica.Text.Trim();
                            utente.Codice = this.txt_userid.Text.Trim();
                            utente.Amministratore = this.cb_amm.SelectedValue;
                            utente.Email = this.txt_email.Text.Trim();
                            utente.FromEmail = this.txt_from_email.Text.Trim();
                            utente.Abilitato = (this.cb_abilitato.Checked) ? "1" : "0";
                            utente.AbilitatoCentroServizi = this.cb_abilitatoCentroServizi.Checked;
                            //
                            // Mev CS 1.4 - Esibizione
                            if (this.cb_abilitatoEsibizione != null)
                                utente.AbilitatoEsibizione = this.cb_abilitatoEsibizione.Checked;
                            // End Mev CS 1.4 - Esibizione
                            //
                            utente.Dominio = this.txt_dominio.Text.Trim();
                            utente.Sede = this.txt_sede.Text.Trim();
                            utente.NotificaTrasm = this.ddl_notifica.SelectedValue;
                            theManager.CurrentIDAmm(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0"));
                            utente.IDAmministrazione = theManager.getIDAmministrazione();
                            utente.NessunaScadenzaPassword = this.chkUserPasswordNeverExpire.Checked;
                            utente.SincronizzaLdap = this.chkSyncronizeLdap.Checked;
                            utente.AutenticatoInLdap = this.chkAutenticatoLdap.Checked;
                            utente.IdSincronizzazioneLdap = this.hdIdSincronizzazioneLdap.Value;
                            utente.IdClientSideModelProcessor = this.GetSelectedProcessorId();
                            utente.SmartClientConfigurations = new DocsPAWA.DocsPaWR.SmartClientConfigurations();
                            //utente.SmartClientConfigurations.IsEnabled = this.rdbIsEnabledSmartClient.Checked;
                            utente.SmartClientConfigurations.ComponentsType = (this.rdbIsEnabledHTML5Socket.Checked ? "4" : this.rdbIsEnabledJavaApplet.Checked ? "3" : (this.rdbIsEnabledSmartClient.Checked ? "2" : (this.rdbIsEnabledActiveX.Checked ? "1" : "0")));
                            utente.SmartClientConfigurations.ApplyPdfConvertionOnScan = ((utente.SmartClientConfigurations.ComponentsType != "1" && utente.SmartClientConfigurations.ComponentsType != "0") && this.chkSmartClientConvertPdfOnScan.Checked);
                            if (this.ddlLabelPrinterType.SelectedValue.ToString().Equals("null"))
                                utente.DispositivoStampa = null;
                            else
                                utente.DispositivoStampa = Convert.ToInt32(this.ddlLabelPrinterType.SelectedValue.ToString());

                            string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                            theManager.InsUtente(utente, idAmm);

                            DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                            esito = theManager.getEsitoOperazione();

                            if (esito.Codice.Equals(0))
                            {
                                // nel caso di multi amministrazione effettuo l'update del record inserendo
                                // l'encrypted password e password_creation_date presa in precedenza
                                if (isUserMultiAmm)
                                    this.SetPasswordUtenteMultiAmm(this.txt_userid.Text.Trim().ToUpper(), pwdUser);

                                this.Inizialize(true, "var_cod_rubrica", this.txt_userid.Text.Trim());
                                this.GUI("InsNuovoUtente");
                            }
                            else
                            {
                                this.AlertJS("Attenzione, " + esito.Descrizione);
                            }

                            esito = null;
                            utente = null;
                            theManager = null;
                        }
                    }
                }
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// Verifica se l'utente appartiene già ad altra amministrazione
        /// </summary>
        private bool VerificaUtenteMultiAmministrazione(string userId)
        {
            string returnMsg = string.Empty;
            DocsPAWA.DocsPaWR.Amministrazione[] listaAmm = UserManager.getListaAmministrazioniByUser(this, userId, false, out returnMsg);
            return (listaAmm.Length > 0) ? true : false;
        }

        private bool VerificaUtenteMultiAmministrazioneMod(string userId)
        {
            string returnMsg = string.Empty;
            DocsPAWA.DocsPaWR.Amministrazione[] listaAmm = UserManager.getListaAmministrazioniByUser(this, userId, true, out returnMsg);
            return (listaAmm.Length > 1) ? true : false;
        }

        /// <summary>
        /// Preleva l'encrypted password dell'utente
        /// </summary>
        private string GetPasswordUtenteMultiAmm(string userId)
        {
            string password = string.Empty;
            password = UserManager.GetPasswordUtenteMultiAmm(this, userId);
            return password;
        }

        /// <summary>
        /// Update del record in people con l'inserimento dell'encrypted password
        /// </summary>
        private bool SetPasswordUtenteMultiAmm(string userId, string password)
        {
            bool resultValue = false;
            resultValue = UserManager.SetPasswordUtenteMultiAmm(this, userId, password);
            return resultValue;
        }

        /// <summary>
        /// Update del record in people con l'inserimento dell'encrypted password
        /// </summary>
        private bool ModificaPasswordUtenteMultiAmm(string userId, string idAmm)
        {
            bool resultValue = false;
            resultValue = UserManager.ModificaPasswordUtenteMultiAmm(this, userId, idAmm);
            return resultValue;
        }

        /// <summary>
        /// MODIFICA
        /// </summary>
        private void ModificaUtente()
        {
            bool isUserMultiAmm = false;
            bool continua = true;
            string pwdUser = string.Empty;
            try
            {
                //verifica la tipologia di utente da modificare
                string alert = "";
                //verifica il tipo amministratore
                string idTipoAmmUtente = this.cb_amm.SelectedValue;
                string tipoAmministratore = datiAmministratore.tipoAmministratore;
                switch (tipoAmministratore)
                {
                    case "1":  //system administrator
                        break;
                    case "2": //super administrator
                        if (idTipoAmmUtente.Equals("1"))
                        {
                            //non può essere eliminato
                            alert = "Attenzione, un Amministratore di livello superiore non può essere modificato con il profilo che si possiede.";
                        }
                        break;
                    case "3": //user administrator
                        if (idTipoAmmUtente.Equals("1") || idTipoAmmUtente.Equals("2") || idTipoAmmUtente.Equals("3"))
                        {
                            alert = "Attenzione, un Amministratore non può essere modificato con il profilo che si possiede.";
                        }
                        break;
                }

                //Andrea De Marco - Disabilita checkbox chiavi se non sono Super Administrator in Amministrazione 
                switch (idTipoAmmUtente)
                {
                    case "0": //NO Administrator
                        this.cbx_chiavi.Checked = false;
                        break;

                    case "2": //Super Administrator
                        break;

                    case "3": //User Administrator
                        this.cbx_chiavi.Checked = false;
                        break;
                }

                
                //verifice se responsabile AOO
                if (Session["ok_multiAmm"] == null)
                {
                    bool isRespAOO = this.IsResponsabileAOO(this.dg_Utenti.SelectedItem.Cells[1].Text, "");
                    if (isRespAOO && !this.cb_abilitato.Checked)
                    {
                        alert = "Attenzione, l'utente  risulta essere responsabile di una o più AOO.";
                    }
                }
                // INTEGRAZIONE PITRE-PARER
                // MEV Responsabile della conservazione
                // Controllo se si sta cercando di disabilitare il responsabile della conservazione
                bool isRespCons = this.checkRespConservazione();
                if (isRespCons && !this.cb_abilitato.Checked)
                {
                    alert = "Attenzione, l'utente è configurato come responsabile della conservazione.";
                }


                //
                if (!alert.Equals(""))
                {
                    this.AlertJS(alert);
                    return;
                }
                if (this.VerificaCampiObbligatori())
                {
                    if (this.VerificaCampiInseriti())
                    {
                        Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
                        DocsPAWA.DocsPaWR.OrgUtente utente = null;
                        isUserMultiAmm = (VerificaUtenteMultiAmministrazioneMod(this.txt_userid.Text.Trim().ToUpper()));

                        if (Session["ok_multiAmm"] != null)
                        {
                            utente = (DocsPAWA.DocsPaWR.OrgUtente)Session["ok_multiAmm"];
                            Session.Remove("ok_multiAmm");
                        }

                        if (utente == null)
                        {
                            utente = new OrgUtente();
                            utente.IDCorrGlobale = this.dg_Utenti.SelectedItem.Cells[0].Text;
                            utente.IDPeople = this.dg_Utenti.SelectedItem.Cells[1].Text;
                            utente.UserId = this.txt_userid.Text.Trim().ToUpper();
                            utente.Matricola = this.txt_Matricola.Text.Trim();
                            utente.Password = this.txt_password.Text.Trim();
                            utente.Nome = this.txt_nome.Text.Trim();
                            utente.Cognome = this.txt_cognome.Text.Trim();
                            utente.CodiceRubrica = this.txt_rubrica.Text.Trim();
                            utente.Codice = this.txt_userid.Text.Trim();
                            utente.Amministratore = this.cb_amm.SelectedValue;
                            utente.Email = this.txt_email.Text.Trim();
                            utente.FromEmail = this.txt_from_email.Text.Trim();
                            utente.Abilitato = (this.cb_abilitato.Checked) ? "1" : "0";
                            utente.Dominio = this.txt_dominio.Text.Trim();
                            utente.Sede = this.txt_sede.Text.Trim();
                            utente.NotificaTrasm = this.ddl_notifica.SelectedValue;
                            utente.IDAmministrazione = this.dg_Utenti.SelectedItem.Cells[14].Text;
                            utente.NessunaScadenzaPassword = this.chkUserPasswordNeverExpire.Checked;
                            utente.SincronizzaLdap = this.chkSyncronizeLdap.Checked;
                            utente.AutenticatoInLdap = this.chkAutenticatoLdap.Checked;
                            utente.IdSincronizzazioneLdap = this.hdIdSincronizzazioneLdap.Value;
                            utente.IdClientSideModelProcessor = this.GetSelectedProcessorId();
                            utente.SmartClientConfigurations = new DocsPAWA.DocsPaWR.SmartClientConfigurations();
                            utente.SmartClientConfigurations.ComponentsType = (this.rdbIsEnabledHTML5Socket.Checked ? "4" : this.rdbIsEnabledJavaApplet.Checked ? "3" : (this.rdbIsEnabledSmartClient.Checked ? "2" : (this.rdbIsEnabledActiveX.Checked ? "1" : "0")));
                            //utente.SmartClientConfigurations.IsEnabled = this.rdbIsEnabledSmartClient.Checked;
                            utente.SmartClientConfigurations.ApplyPdfConvertionOnScan = ((utente.SmartClientConfigurations.ComponentsType != "1" && utente.SmartClientConfigurations.ComponentsType != "0") && this.chkSmartClientConvertPdfOnScan.Checked);
                            if (this.ddlLabelPrinterType.SelectedValue.ToString().Equals("null"))
                                utente.DispositivoStampa = null;
                            else
                                utente.DispositivoStampa = Convert.ToInt32(this.ddlLabelPrinterType.SelectedValue.ToString());

                            utente.AbilitatoCentroServizi = this.cb_abilitatoCentroServizi.Checked;

                            //
                            // Mev CS 1.4 -Esibizione
                            if (this.cb_abilitatoEsibizione != null)
                                utente.AbilitatoEsibizione = this.cb_abilitatoEsibizione.Checked;
                            // End Mev CS 1.4 - Esibizione
                            //

                            utente.AbilitatoChiaviConfigurazione = this.cbx_chiavi.Checked;

                            if (Session["ok_multiAmm"] == null)
                            {
                                if (isUserMultiAmm)
                                {
                                    //TODO SHOW ALERT MESSAGE....
                                    continua = false;
                                    Session.Add("ok_multiAmm", utente);
                                    //Session.Add("abilitaDisabilitaUser", this.dg_Utenti.SelectedItem.Cells[10].Text.Replace("&nbsp;", ""));
                                    Session.Add("currentPageIndex", this.dg_Utenti.CurrentPageIndex);
                                    Session.Add("comboUser", this.cb_amm.SelectedValue);
                                    Session.Add("datagridUser", this.dg_Utenti.SelectedItem);
                                    msg_modifica.Confirm("E’ presente almeno un utente con stessa UserID in un’altra amministrazione, vuoi procedere con la modifica? Attenzione: l’eventuale modifica della password verrà ereditata dagli altri utenti.");
                                }
                            }
                            else
                                Session.Remove("ok_multiAmm");
                        }
                        if (Session["coinvoltoInLF"] == null && !this.cb_abilitato.Checked)
                        {
                            string stridUtente = this.dg_Utenti.SelectedItem.Cells[1].Text;
                            if (!string.IsNullOrEmpty(stridUtente))
                                VerificaProcessiCoinvolti(stridUtente);
                            if (this.dg_Utenti.SelectedItem.Cells[10].Text.Replace("&nbsp;", "") == "N")
                            {
                                alert = getCoinvoltoInLibroFirma();
                            }
                            if (!string.IsNullOrEmpty(alert))
                            {
                                continua = false;
                                Session.Add("coinvoltoInLF", utente);
                                msg_modifica.Confirm(alert);
                            }
                        }
                        else
                        {
                            Session.Remove("coinvoltoInLF");
                        }

                        if (continua)
                        {
                            theManager.ModUtente(utente,
                            AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3"));

                            DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                            esito = theManager.getEsitoOperazione();

                            if (esito.Codice.Equals(0))
                            {
                                if (isUserMultiAmm)
                                    this.ModificaPasswordUtenteMultiAmm(utente.UserId.ToUpper(), utente.IDAmministrazione);

                                if (this.GestAbilitaDisabilitaUtente(utente))
                                {
                                    //SABRINA if(this.EliminaRegistriAdmin())
                                    if (this.EliminaMenuAdmin())
                                        this.AggiornaDGDopoMod(utente);
                                }
                                if (Session["datagridUser"] != null)
                                    Session.Remove("datagridUser");
                            }
                            else
                            {
                                this.AlertJS("Attenzione, " + esito.Descrizione);
                            }

                            esito = null;
                            utente = null;
                            theManager = null;
                        }
                    }
                }
                //Andrea De Marco - Richiamo PerformActionFind per rendere visibile all'utente la modifica
                if(continua)
                    this.PerformActionFind();
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
                this.GUI("ResetAll");
            }
        }

        private void InvalidaPassiCorrelati()
        {
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            wws.Timeout = System.Threading.Timeout.Infinite;
            List<ProcessoFirma> processiCoinvolti_U = (Session["processiCoinvolti_U"] != null && ((int)Session["processiCoinvolti_U"]) >0 ? wws.GetProcessiDiFirmaByUtenteTitolare(this.dg_Utenti.SelectedItem.Cells[1].Text, string.Empty).ToList() : new List<ProcessoFirma>());
            List<IstanzaProcessoDiFirma> istazaProcessiCoinvolti_U = (Session["istazaProcessiCoinvolti_U"] != null && ((int)Session["istazaProcessiCoinvolti_U"]) > 0 ? wws.GetIstanzeProcessiDiFirmaByUtenteCoinvolto(this.dg_Utenti.SelectedItem.Cells[1].Text, string.Empty).ToList() : new List<IstanzaProcessoDiFirma>());

            if (processiCoinvolti_U.Count > 0)
            {
                List<string> idPassi = new List<string>();
                foreach (ProcessoFirma processo in processiCoinvolti_U)
                {
                    foreach (PassoFirma passo in processo.passi)
                    {
                        if (!idPassi.Contains(passo.idPasso))
                        {
                            idPassi.Add(passo.idPasso);
                        }
                    }
                }

                wws.TickPasso(idPassi.ToArray(), "U");
                Session["processiCoinvolti_U"] = null;
            }

            if (istazaProcessiCoinvolti_U.Count > 0)
            {
                DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                wws.TickIstanze(istazaProcessiCoinvolti_U.ToArray(), "U", sessionManager.getUserAmmSession());

                Session["istazaProcessiCoinvolti_U"] = null;
            }
        }

        /// <summary>
        /// ELIMINA
        /// </summary>
        private void EliminaUtente(DocsPAWA.DocsPaWR.OrgUtente utente)
        {
            try
            {
                // verifica se l'utente è connesso
                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.VerificaUtenteLoggato(utente.UserId, utente.IDAmministrazione);

                DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                esito = theManager.getEsitoOperazione();

                if (!esito.Codice.Equals(0))
                {
                    this.AlertJS("Attenzione, " + esito.Descrizione);
                }
                else
                {
                    // non è connesso, quindi procede alle verifiche...
                    //verifica se è responsabile AOO
                    bool isRespAoo = this.IsResponsabileAOO(utente.IDPeople, "");
                    if (isRespAoo)
                    {
                        string msg = "Attenzione,\\n\\nimpossibile eliminare l'utente " + utente.Cognome + " " + utente.Nome + "\\npoichè risulta essere responsabile di una o più AOO.";
                        msg += "\\n\\nAccedere alla sezione Registri e cambiare l'utente resposabile.";
                        this.AlertJS(msg);
                    }
                    else
                    {
                        // verifica se ha ruoli in organigramma
                        ArrayList ruoli = new ArrayList();
                        ruoli = this.ListaRuoliUtente(utente.IDPeople);
                        if (ruoli != null && ruoli.Count > 0)
                        {
                            // ha ruoli... finisce qui!
                            string msg = "Attenzione,\\n\\nimpossibile eliminare l'utente " + utente.Cognome + " " + utente.Nome + "\\npoichè risulta essere inserito in uno o più ruoli in organigramma.";
                            msg += "\\n\\nAccedere alla sezione Organigramma e disassociare l'utente dai ruoli.";
                            this.AlertJS(msg);
                        }
                        else
                        {
                            /* è possibile eliminare fisicamente l'utente solo se:							
                                1 - non ha creato documenti o fascicoli
                                2 - non è stato mittente o destinatario di documenti						
                                3 - non è mittente o destinatario di trasmissioni							
                            */

                            Manager.UtentiManager utManager = new Amministrazione.Manager.UtentiManager();
                            utManager.VerificaEliminazioneUtente(utente);

                            esito = utManager.getEsitoOperazione();
                            // possibili valori di ritorno:
                            // 0 - tutto ok! è possibile eliminare l'utente
                            // 1 - non è possibile eliminare l'utente (viene disabilitato)					
                            // 9 - errore generico

                            switch (esito.Codice)
                            {
                                case 0:
                                    this.EliminaUtenteDefin(utente);
                                    InvalidaPassiCorrelati();
                                    break;
                                case 1:
                                    this.DisabilitaUtDaEliminare(utente);
                                    InvalidaPassiCorrelati();
                                    break;
                                case 9:
                                    this.AlertJS("Attenzione, " + esito.Descrizione);
                                    break;
                            }

                            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                            DataSet dsListe = wws.isCorrInListaDistr(utente.IDCorrGlobale);
                            string messaggio = string.Empty;
                            if (dsListe != null)
                            {
                                if (dsListe.Tables.Count > 0)
                                {
                                    DataTable tab = dsListe.Tables[0];
                                    if (tab.Rows.Count > 0)
                                    {
                                        messaggio += "Attenzione, utente presente nelle seguenti liste di distribuzione\\n";
                                        for (int i = 0; i < tab.Rows.Count; i++)
                                        {
                                            messaggio += tab.Rows[i]["var_desc_corr"].ToString();
                                            if (!string.IsNullOrEmpty(tab.Rows[i]["prop"].ToString()))
                                                messaggio += " creata da " + tab.Rows[i]["prop"].ToString();
                                            else
                                                if (!string.IsNullOrEmpty(tab.Rows[i]["ruolo"].ToString()))
                                                    messaggio += " creata per il ruolo " + tab.Rows[i]["ruolo"].ToString();
                                            messaggio += "\\n";
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(messaggio))
                                    this.AlertJS(messaggio);
                                //ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + messaggio + "');", true);

                            }
                        }
                    }
                }
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
                this.GUI("ResetAll");
            }

        }

        private void EliminaUtenteDefin(DocsPAWA.DocsPaWR.OrgUtente utente)
        {
            try
            {
                Manager.UtentiManager utManager = new Amministrazione.Manager.UtentiManager();

                utManager.EliminaUtente(utente);

                DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                esito = utManager.getEsitoOperazione();

                if (!esito.Codice.Equals(0))
                {
                    this.AlertJS("Attenzione, " + esito.Descrizione);
                }
                else
                {
                    this.Inizialize(false, null, null);
                }
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
                this.GUI("ResetAll");
            }
        }

        /// <summary>
        /// Tasto Elimina utente
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public void deleteGrid(Object obj, DataGridCommandEventArgs args)
        {
            DocsPAWA.DocsPaWR.OrgUtente utente = new DocsPAWA.DocsPaWR.OrgUtente();

            utente.IDCorrGlobale = args.Item.Cells[0].Text;
            utente.IDPeople = args.Item.Cells[1].Text;
            utente.UserId = args.Item.Cells[2].Text;
            utente.Nome = args.Item.Cells[4].Text;
            utente.Cognome = args.Item.Cells[5].Text;
            utente.Amministratore = args.Item.Cells[8].Text;
            utente.IDAmministrazione = args.Item.Cells[14].Text;

            this.EliminaUtente(utente);
        }


        #region Disabilitazione / Abilitazione

        private bool GestAbilitaDisabilitaUtente(DocsPAWA.DocsPaWR.OrgUtente utente)
        {
            bool retValue = true;

            if (this.dg_Utenti != null && this.dg_Utenti.SelectedItem != null)
            {
                // se l'utente era abilitato e ora viene disabilitato...
                if (this.dg_Utenti.SelectedItem.Cells[10].Text.Replace("&nbsp;", "") == "N" && utente.Abilitato.Equals("0"))
                {
                    retValue = this.DisabilitaUtente(utente);
                }

                // se l'utente era disabilitato e ora viene abilitato...
                if (this.dg_Utenti.SelectedItem.Cells[10].Text.Replace("&nbsp;", "") == "Y" && utente.Abilitato.Equals("1"))
                {
                    retValue = this.AbilitaUtente(utente);
                }
            }
            else
            {
                if (Session["datagridUser"] != null)
                {
                    DataGridItem che = (DataGridItem)Session["datagridUser"];
                    if (che != null)
                    {
                        if (che.Cells[10].Text.Replace("&nbsp;", "") == "N" && utente.Abilitato.Equals("0"))
                        {
                            retValue = this.DisabilitaUtente(utente);
                        }

                        // se l'utente era disabilitato e ora viene abilitato...
                        if (che.Cells[10].Text.Replace("&nbsp;", "") == "Y" && utente.Abilitato.Equals("1"))
                        {
                            retValue = this.AbilitaUtente(utente);
                        }
                    }
                }
            }
            return retValue;
        }

        /// <summary>
        /// Disabilita Utente
        /// </summary>
        private bool DisabilitaUtente(DocsPAWA.DocsPaWR.OrgUtente utente)
        {
            bool retValue = false;

            try
            {
                Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                theManager.DisabilitaUtente(utente.IDPeople, idAmm);

                DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                esito = theManager.getEsitoOperazione();

                // possibili valori di ritorno:
                // 1 - errore generico					
                // 9 - tutto ok con messaggio all'utente

                if (esito.Codice.Equals(9))
                {
                    this.AlertJS("NOTA\\n" + esito.Descrizione);
                    retValue = true;
                    InvalidaPassiCorrelati();
                }
                else // uguale a "1" (errore)
                {
                    this.AlertJS("Attenzione, " + esito.Descrizione);
                    retValue = false;
                }
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
                this.GUI("ResetAll");
            }

            return retValue;
        }

        /// <summary>
        /// Abilita Utente
        /// </summary>
        private bool AbilitaUtente(DocsPAWA.DocsPaWR.OrgUtente utente)
        {
            bool retValue = false;

            try
            {
                Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                theManager.AbilitaUtente(utente.IDPeople, idAmm);

                DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                esito = theManager.getEsitoOperazione();

                if (esito.Codice.Equals(9))
                {
                    this.AlertJS("NOTA\\n" + esito.Descrizione);
                    retValue = true;
                }
                else // uguale a "1" (errore)
                {
                    this.AlertJS("Attenzione, " + esito.Descrizione);
                    retValue = false;
                }
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
                this.GUI("ResetAll");
            }

            return retValue;
        }

        /// <summary>
        /// Disabilita Utente dopo aver verificato che non è possibile eliminarlo
        /// </summary>
        private void DisabilitaUtDaEliminare(DocsPAWA.DocsPaWR.OrgUtente utente)
        {
            try
            {
                Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                theManager.DisabilitaUtente(utente.IDPeople, idAmm);

                DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                esito = theManager.getEsitoOperazione();

                // possibili valori di ritorno:
                // 1 - errore generico					
                // 9 - tutto ok con messaggio all'utente

                if (esito.Codice.Equals(9))
                {
                    string msg = "non è possibile eliminare l'utente poichè ha compiuto operazioni su DocsPA.\\n\\n";
                    msg += "Lo stesso è stato al momento disabilitato.";
                    this.AlertJS("NOTA\\n" + msg);
                    this.Inizialize(true, this.ddl_ricerca.SelectedValue, this.txt_ricerca.Text.Trim());
                }
                else // uguale a "1" (errore)
                {
                    this.AlertJS("Attenzione, " + esito.Descrizione);
                }
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
                this.GUI("ResetAll");
            }
        }
        #endregion

        /// <summary>
        /// Verifica se sono stati inseriti tutti i campi obbligatori
        /// </summary>
        /// <returns></returns>
        private bool VerificaCampiObbligatori()
        {
            bool retValue = true;

            if (
                this.txt_userid.Text.Trim().Equals("") ||
                this.txt_nome.Text.Trim().Equals("") ||
                this.txt_cognome.Text.Trim().Equals("") ||
                this.txt_rubrica.Text.Trim().Equals("")
                )
            {
                this.AlertJS("Attenzione, inserire tutti i campi obbligatori");
                retValue = false;
            }

            return retValue;
        }

        /// <summary>
        /// Verifica la correttezza dei campi inseriti
        /// </summary>
        /// <returns></returns>
        private bool VerificaCampiInseriti()
        {
            bool retValue = true;

            // controllo del campo DOMINIO alfanumerico
            if (this.txt_dominio.Text.Trim() != "" && !AmmUtils.UtilsXml.IsAlphaNumeric(this.txt_dominio.Text.Trim()))
            {
                this.AlertJS("Attenzione, inserire valori alfanumerici nel campo DOMINIO. Ammesso l'underscore e il punto.");
                this.SetFocus(this.txt_dominio);
                return false;
            }

            //Non serve +
            //if (!VerificaFormatoDominio())
            //{
            //    retValue = false;
            //}

            // controllo del campo codice rubrica alfanumerico
            if (!AmmUtils.UtilsXml.IsAlphaNumeric(this.txt_rubrica.Text.Trim()))
            {
                this.AlertJS("Attenzione, inserire valori alfanumerici nel campo COD. RUBRICA. Ammesso l'underscore e il punto");
                this.SetFocus(this.txt_rubrica);
                return false;
            }

            // controllo del campo EMAIL valido
            if (this.txt_email.Text.Trim() != "" && !AmmUtils.UtilsXml.IsValidEmail(this.txt_email.Text.Trim()))
            {
                this.AlertJS("Attenzione, inserire una email valida");
                this.SetFocus(this.txt_email);
                return false;
            }

            // controllo del campo FROMEMAIL valido
            if (this.txt_from_email.Text.Trim() != "" && !AmmUtils.UtilsXml.IsValidEmail(this.txt_from_email.Text.Trim()))
            {
                this.AlertJS("Attenzione, inserire una email valida");
                this.SetFocus(this.txt_from_email);
                return false;
            }

            // controllo della notifica
            /*if ((this.txt_email.Text.Trim().Equals("") || this.txt_from_email.Text.Trim().Equals("")) && this.ddl_notifica.SelectedItem.Value.ToString() != "null")
            {
                this.AlertJS("Attenzione, bisogna inserire una email valida per poter gestire la notifica trasmissione");
                this.SetFocus(this.txt_email);
                return false;
            }*/

            // controllo se l'utente è USER amministratore ma non ha voci di menù associate
            if (this.cb_amm.SelectedValue.Equals("3"))
            {
                if (!this.EsistonoMenuAssociati())
                {
                    this.AlertJS("Attenzione, l'utente è uno USER amministratore \\npertanto bisogna associare almeno una voce di menù a questo utente");

                    if (this.btn_aggiungi.Text.Equals("Modifica"))
                        this.ExecuteJS("ApriGestVociMenu('" + this.dg_Utenti.SelectedItem.Cells[0].Text + "','" + this.dg_Utenti.SelectedItem.Cells[14].Text + "');");

                    return false;
                }
            }

            return retValue;
        }

        private bool VerificaFormatoDominio()
        {
            bool retValue = true;
            if (this.txt_dominio.Text.Trim() != "")
            {
                //recupero la stringa del formato del dominio dell'amministrazione decisa in homepage
                Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
                string formatoAmm = theManager.GetFormatDominio(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3"));
                if (!string.IsNullOrEmpty(formatoAmm))
                {
                    //recupero i separatori ammessi dal webconfig
                    string separatori = "";
                    if (System.Configuration.ConfigurationManager.AppSettings["SEPARATORI DOMINIO"] != null)
                    {
                        separatori = System.Configuration.ConfigurationManager.AppSettings["SEPARATORI DOMINIO"];
                    }
                    if (separatori != "")
                    {
                        char sepDominio = ' ';
                        string[] sepList = separatori.Split(';');
                        string[] ArrayDominioAmm;
                        string[] ArrayDominioUtente;

                        foreach (string sep in sepList)
                        {
                            if (formatoAmm.Contains(sep))
                            {
                                sepDominio = Convert.ToChar(sep);

                            }
                        }

                        if (sepDominio != ' ')
                        {
                            ArrayDominioAmm = formatoAmm.Split(sepDominio);

                            if (!this.txt_dominio.Text.Contains(sepDominio.ToString()))
                            {
                                this.AlertJS("Attenzione, il formato del dominio deve essere: " + formatoAmm + ". Come da impostazione di home page.");
                                this.SetFocus(this.txt_dominio);
                                retValue = false;
                            }
                            else
                            {
                                ArrayDominioUtente = txt_dominio.Text.Trim().Split(sepDominio);
                                if (ArrayDominioAmm.Length != ArrayDominioUtente.Length || ArrayDominioUtente[0] == "" || ArrayDominioUtente[1] == "" || !AmmUtils.UtilsXml.IsAlphaNumeric(ArrayDominioUtente[0]) || !AmmUtils.UtilsXml.IsAlphaNumeric(ArrayDominioUtente[1]))
                                {
                                    this.AlertJS("Attenzione, il formato del dominio deve essere: " + formatoAmm + ". Come da impostazione di home page.");
                                    this.SetFocus(this.txt_dominio);
                                    retValue = false;
                                }
                            }
                        }
                    }
                }

            }
            return retValue;
        }


        private bool EsistonoRegistriAssociati()
        {
            Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
            return theManager.EsistonoRegistriAssociati(this.dg_Utenti.SelectedItem.Cells[0].Text);
        }

        private bool EsistonoMenuAssociati()
        {
            Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
            return theManager.EsistonoMenuAssociati(this.dg_Utenti.SelectedItem.Cells[0].Text);
        }

        /// <summary>
        /// gestione controllo password
        /// </summary>
        /// <returns></returns>
        private bool VerificaPassword(string azione)
        {
            bool retValue = true;

            switch (azione)
            {
                case "Aggiungi":
                    if (this.txt_password.Text.Trim() != string.Empty || this.txt_ConfPassword.Text.Trim() != string.Empty)
                    {
                        // controllo dell'uguaglianza
                        if (this.txt_password.Text.Trim() != this.txt_ConfPassword.Text.Trim())
                        {
                            this.AlertJS("Attenzione, la password e la conferma sono differenti");
                            this.SetFocus(this.txt_password);
                            return false;
                        }

                        // Il controllo è valido solo se la gestione password da amministrazione è abilitata o meno
                        if (!this.IsSupportedPasswordConfig())
                        {
                            // controllo del campo password alfanumerico
                            if (!AmmUtils.UtilsXml.IsAlphaNumeric(this.txt_password.Text.Trim()))
                            {
                                this.AlertJS("Attenzione, inserire valori alfanumerici nel campo PASSWORD. Ammesso l'underscore e il punto");
                                this.SetFocus(this.txt_password);
                                return false;
                            }
                        }

                        // controllo autenticazione e password						
                        if (this.txt_dominio.Text.Trim() != string.Empty && this.cb_amm.SelectedValue.Equals("0"))
                        {
                            this.AlertJS("Attenzione, l'autenticazione avviene su DOMINIO. Inutile specificare una PASSWORD");
                            this.txt_password.Text = "";
                            this.txt_ConfPassword.Text = "";
                            this.SetFocus(this.txt_password);
                            return false;
                        }
                    }
                    else
                    {
                        // mancato inserimento della password						
                        if (this.txt_dominio.Text.Trim().Equals(string.Empty) && this.cb_amm.SelectedValue.Equals("0"))
                        {
                            this.AlertJS("Attenzione, inserire il campo PASSWORD e conferma password");
                            this.SetFocus(this.txt_password);
                            return false;
                        }

                        // mancato inserimento della password di amministratore						
                        if (!this.txt_dominio.Text.Trim().Equals(string.Empty) && !this.cb_amm.SelectedValue.Equals("0"))
                        {
                            this.AlertJS("Attenzione, inserire il campo PASSWORD e conferma password per accedere come Amministratore");
                            this.SetFocus(this.txt_password);
                            return false;
                        }
                    }

                    break;

                case "Modifica":
                    if (this.txt_password.Text.Trim() != string.Empty || this.txt_ConfPassword.Text.Trim() != string.Empty)
                    {
                        // controllo dell'uguaglianza
                        if (this.txt_password.Text.Trim() != this.txt_ConfPassword.Text.Trim())
                        {
                            this.AlertJS("Attenzione, la password e la conferma sono differenti");
                            this.SetFocus(this.txt_password);
                            return false;
                        }

                        // Il controllo è valido solo se la gestione password da amministrazione è abilitata o meno
                        if (!this.IsSupportedPasswordConfig())
                        {
                            // controllo del campo password alfanumerico
                            if (!AmmUtils.UtilsXml.IsAlphaNumeric(this.txt_password.Text.Trim()))
                            {
                                this.AlertJS("Attenzione, inserire valori alfanumerici nel campo PASSWORD. Ammesso l'underscore e il punto");
                                this.SetFocus(this.txt_password);
                                return false;
                            }
                        }

                        // controllo autenticazione e password						
                        if (this.txt_dominio.Text.Trim() != string.Empty && this.cb_amm.SelectedValue.Equals("0"))
                        {
                            this.txt_password.Text = "";
                            this.txt_ConfPassword.Text = "";
                            this.SetFocus(this.txt_password);
                            return false;
                        }
                    }
                    else
                    {
                        //// controllo autenticazione e password
                        ////if(this.txt_dominio.Text.Trim() != string.Empty && !this.cb_amm.Checked)
                        //if (this.txt_dominio.Text.Trim() != string.Empty && 
                        //    this.cb_amm.SelectedValue.Equals("0"))
                        //{
                        //    this.txt_password.Text = string.Empty;
                        //    this.txt_ConfPassword.Text = string.Empty;
                        //}

                        // mancato inserimento della password						
                        if (this.txt_dominio.Text.Trim().Equals(string.Empty) &&
                                this.cb_amm.SelectedValue.Equals("0") &&
                                this.txt_password.Text.Equals(string.Empty) &&
                                this.txt_ConfPassword.Text.Equals(string.Empty))
                        {
                            this.AlertJS("Attenzione, inserire una nuova password per questo utente");
                            this.SetFocus(this.txt_password);
                            return false;
                        }

                        // mancato inserimento della password di amministratore (privo di autent.)						
                        if (!this.cb_amm.SelectedValue.Equals("0") &&
                            this.txt_password.Text.Equals(string.Empty) &&
                                this.txt_ConfPassword.Text.Equals(string.Empty))
                        {
                            this.AlertJS("Attenzione, inserire il campo PASSWORD e conferma password per accedere come Amministratore");
                            this.SetFocus(this.txt_password);
                            return false;
                        }
                    }
                    break;
            }
            return retValue;
        }

        //controlli tipo amministratore
        private void CheckTipoAmministratore(DataGridItem item)
        {
            //nuovi controlli per la voce amministratore
            string idTipoAmmUtente = item.Cells[8].Text;
            string tipoAmministratore = datiAmministratore.tipoAmministratore;

            this.LoadDDLAmministratori();
            this.cb_amm.Enabled = true;
            switch (tipoAmministratore)
            {
                case "1":  //system administrator
                    cb_amm.SelectedIndex = cb_amm.Items.IndexOf(cb_amm.Items.FindByValue(item.Cells[8].Text));
                    this.btn_vociMenu.Visible = false;
                    if (cb_amm.SelectedValue.Equals("3"))
                    {
                        this.btn_vociMenu.Visible = true;
                        this.btn_vociMenu.Attributes.Add("onclick", "ApriGestVociMenu('" + item.Cells[0].Text + "','" + item.Cells[14].Text + "');");
                    }
                    break;
                case "2": //super administrator
                    if (idTipoAmmUtente.Equals("1"))
                    {
                        //ripuliamo la combobox e inseriamo solo il valore 2
                        this.cb_amm.Items.Clear();
                        ListItem items = new ListItem("System Administrator", "1");
                        this.cb_amm.Items.Add(items);
                        this.cb_amm.Enabled = false;
                        //non renderei modificabile i dati dell'amministratore ?!
                        //if (this.btn_aggiungi.Text.Equals("Modifica"))
                        //    btn_aggiungi.Enabled = false;

                    }
                    else //caso 0,2,3 ok
                    {
                        cb_amm.SelectedIndex = cb_amm.Items.IndexOf(cb_amm.Items.FindByValue(item.Cells[8].Text));
                        this.btn_vociMenu.Visible = false;
                        if (cb_amm.SelectedValue.Equals("3"))
                        {
                            this.btn_vociMenu.Visible = true;
                            this.btn_vociMenu.Attributes.Add("onclick", "ApriGestVociMenu('" + item.Cells[0].Text + "','" + item.Cells[14].Text + "');");
                        }
                    }
                    break;
                case "3": //user administrator
                    if (idTipoAmmUtente.Equals("1"))
                    {
                        //ripuliamo la combobox e inseriamo solo il valore 1
                        this.cb_amm.Items.Clear();
                        ListItem items = new ListItem("System Administrator", "1");
                        this.cb_amm.Items.Add(items);
                        this.cb_amm.Enabled = false;
                        //non renderei modificabile i dati dell'amministratore ?!
                        //if (this.btn_aggiungi.Text.Equals("Modifica"))
                        //    btn_aggiungi.Enabled = false;
                    }
                    else
                        if (idTipoAmmUtente.Equals("2"))
                        {
                            //ripuliamo la combobox e inseriamo solo il valore 2
                            this.cb_amm.Items.Clear();
                            ListItem items = new ListItem("Super Administrator", "2");
                            this.cb_amm.Items.Add(items);
                            this.cb_amm.Enabled = false;
                            //non renderei modificabile i dati dell'amministratore ?!
                            //if (this.btn_aggiungi.Text.Equals("Modifica"))
                            //btn_aggiungi.Enabled = false;
                        }
                        else if (idTipoAmmUtente.Equals("3"))
                        {
                            //ripuliamo la combobox e inseriamo solo il valore 3
                            this.cb_amm.Items.Clear();
                            ListItem items = new ListItem("User Administrator", "3");
                            this.cb_amm.Items.Add(items);
                            this.cb_amm.Enabled = false;
                        }
                        else
                            cb_amm.SelectedIndex = cb_amm.Items.IndexOf(cb_amm.Items.FindByValue(item.Cells[8].Text));
                    //disabilito sempre il tasto
                    this.cb_amm.Enabled = false;
                    break;

            }

        }

        private void VerificaProcessiCoinvolti(string idUtenteCoinvolto)
        {
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();

            processiCoinvolti_U = wws.GetCountProcessiDiFirmaByUtenteTitolare(idUtenteCoinvolto, string.Empty);
            if (processiCoinvolti_U > 0)
            {
                Session["processiCoinvolti_U"] = processiCoinvolti_U;
                istazaProcessiCoinvolti_U = wws.GetCountIstanzeProcessiDiFirmaByUtenteCoinvolto(idUtenteCoinvolto, string.Empty);
                if (istazaProcessiCoinvolti_U > 0)
                {
                    Session["istazaProcessiCoinvolti_U"] = istazaProcessiCoinvolti_U;
                }
                else
                {
                    Session["istazaProcessiCoinvolti_U"] = null;
                }
            }
            else
            {
                Session["processiCoinvolti_U"] = null;
            }
        }

        /// <summary>
        /// Reperimento dell'ID dell'UO corrente
        /// </summary>
        /// <returns></returns>
        protected string getCoinvoltoInLibroFirma()
        {
            string retVal = string.Empty;

            string passivi = string.Empty;
            string attivi = string.Empty;

            if (processiCoinvolti_U > 0)
            {
                passivi = (
                    (processiCoinvolti_U > 1) ?
                    "Il soggetto in modifica è coinvolto in " + processiCoinvolti_U + " processi di firma. Proseguendo nella modifica, sarà necessario modificare i passi in cui esso è coinvolto per poter avviare tali processi." :
                    "Il soggetto in modifica è coinvolto in un processo di firma. Proseguendo nella modifica, sarà necessario modificare i passi in cui esso è coinvolto per poter avviare tali processi.");
            }

            if (istazaProcessiCoinvolti_U > 0)
            {
                attivi = (
                    (istazaProcessiCoinvolti_U > 1) ?
                    "l soggetto in modifica è coinvolto in " + istazaProcessiCoinvolti_U + " processi di firma avviati e non ancora conclusi. Proseguendo nella modifica, tutti i processi coivolti saranno interrotti." :
                    "l soggetto in modifica è coinvolto in un processo di firma avviato e non ancora concluso. Proseguendo nella modifica, il processo coinvolto sarà interrotto.");
            }

            retVal = (!string.IsNullOrEmpty(passivi) && !string.IsNullOrEmpty(attivi) ? passivi + "\\n\\r Inoltre i" + attivi : (
                !string.IsNullOrEmpty(passivi) ? passivi : (!string.IsNullOrEmpty(attivi) ? "I" + attivi : "")));
            if (!string.IsNullOrEmpty(retVal))
                retVal = retVal + "\\n\\r Si vuole procedere comunque? Importante avvisare il creatore dei processi coinvolti e il proponente.";

            return retVal;
        }

        protected bool checkRespConservazione()
        {
            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            string idUtenteRespCons = ws.GetIdUtenteRespConservazione(this.dg_Utenti.SelectedItem.Cells[14].Text);

            if (!string.IsNullOrEmpty(idUtenteRespCons) && idUtenteRespCons.Equals(this.dg_Utenti.SelectedItem.Cells[1].Text))
                return true;
            else
                return false;
            
        }

        /// <summary>
        /// Carica i dati utente a video
        /// </summary>
        /// <param name="item"></param>
        private void LoadDatiUtente(DataGridItem item)
        {
            string idUtenteSelezionato = item.Cells[1].Text; //item.Cells[2].Text.Replace("&nbsp;", "");
            //string msgLibroFirma=string.Empty;
            if (!string.IsNullOrEmpty(idUtenteSelezionato))
            {
                VerificaProcessiCoinvolti(idUtenteSelezionato);
                msgLibroFirma = getCoinvoltoInLibroFirma();
            }

            // userid ----------------------------------------------------------------
            this.txt_userid.Text = item.Cells[2].Text.Replace("&nbsp;", "");
            // cognome ---------------------------------------------------------------
            this.txt_cognome.Text = item.Cells[4].Text.Replace("&nbsp;", "");
            // nome ------------------------------------------------------------------
            this.txt_nome.Text = item.Cells[5].Text.Replace("&nbsp;", "");
            // cod. rubrica ---------------------------------------------------------------
            this.txt_rubrica.Text = item.Cells[7].Text.Replace("&nbsp;", "");
            // email -----------------------------------------------------------------
            this.txt_email.Text = item.Cells[9].Text.Replace("&nbsp;", "");
            // fromEmail -----------------------------------------------------------------
            this.txt_from_email.Text = item.Cells[18].Text.Replace("&nbsp;", "");

            if (this.IsSupportedPasswordConfig())
            {
                // Impostazione valore per il check "Nessuna scadenza password" per l'utente
                // sono se supportata la gestione scadenza password
                this.chkUserPasswordNeverExpire.Checked = item.Cells[17].Text.Replace("&nbsp;", "").ToLower().Equals("true");
            }

            // amministratore --------------------------------------------------------			
            //cb_amm.SelectedIndex = cb_amm.Items.IndexOf(cb_amm.Items.FindByValue(item.Cells[8].Text));
            //this.btn_vociMenu.Visible=false;			
            //if(cb_amm.SelectedValue.Equals("3"))
            //{					
            //    this.btn_vociMenu.Visible=true;
            //    this.btn_vociMenu.Attributes.Add("onclick","ApriGestVociMenu('" + item.Cells[0].Text + "','" + item.Cells[14].Text + "');");
            //}

            CheckTipoAmministratore(item);

            // abilitato -------------------------------------------------------------
            this.cb_abilitato.Checked = false;
            if (item.Cells[10].Text.Replace("&nbsp;", "") == "N")
                this.cb_abilitato.Checked = true;

            // abilitato centro servizi-----------------------------------------------
            this.cb_abilitatoCentroServizi.Checked = false;
            bool abilitatoCentroServizi;
            if (Boolean.TryParse(item.Cells[26].Text, out abilitatoCentroServizi))
                this.cb_abilitatoCentroServizi.Checked = abilitatoCentroServizi;

            //
            // MEV CS 1.4 - Esibizione
            // abilitato Esibizione-----------------------------------------------
            this.cb_abilitatoEsibizione.Checked = false;
            bool abilitatoEsibizione;
            if (Boolean.TryParse(item.Cells[30].Text, out abilitatoEsibizione))
                this.cb_abilitatoEsibizione.Checked = abilitatoEsibizione;
            // End MEV CS 1.4 - Esibizione
            //

            // abilitato centro servizi-----------------------------------------------
            this.cbx_chiavi.Checked = false;
            bool abilitatoChiaviConfigurazione;
            if (Boolean.TryParse(item.Cells[28].Text, out abilitatoChiaviConfigurazione))
                this.cbx_chiavi.Checked = abilitatoChiaviConfigurazione;

            // dominio ---------------------------------------------------------------
            this.txt_dominio.Text = item.Cells[11].Text.Replace("&nbsp;", "");
            //Commentato nuova gestione formato dominio
            //if (this.txt_dominio.Text != "")
            //    this.txt_dominio.Text = this.txt_dominio.Text.Substring(0, (this.txt_dominio.Text.IndexOf("\\", 0)));
            // sede ------------------------------------------------------------------
            this.txt_sede.Text = item.Cells[12].Text.Replace("&nbsp;", "");
            // notifica --------------------------------------------------------------
            if (item.Cells[13].Text.Replace("&nbsp;", "") == "")
                this.ddl_notifica.SelectedIndex = this.ddl_notifica.Items.IndexOf(this.ddl_notifica.Items.FindByValue("null"));
            else
                this.ddl_notifica.SelectedIndex = this.ddl_notifica.Items.IndexOf(this.ddl_notifica.Items.FindByValue(item.Cells[13].Text));

            this.GUI("DatiUtente");

            if (!this.VerificaUtConnesso(this.txt_userid.Text, item.Cells[14].Text))
            {
                ArrayList listaRuoli = this.ListaRuoliUtente(item.Cells[1].Text);

                if (listaRuoli != null && listaRuoli.Count > 0)
                {
                    this.btn_ruoli.Visible = true;
                    this.btn_ruoli.Attributes.Add("onclick", "ApriVisRuoli('" + item.Cells[1].Text + "','" + Server.UrlEncode(item.Cells[4].Text.Replace("'", "|@ap@|")) + " " + Server.UrlEncode(item.Cells[5].Text.Replace("'", "|@ap@|")) + "');");
                }
            }
            if (this.btn_vociMenu.Visible)
                this.btn_vociMenu.Attributes.Add("onclick", "ApriGestVociMenu('" + item.Cells[0].Text + "','" + item.Cells[14].Text + "');");

            this.chkSyncronizeLdap.Checked = (this.LdapIntegrationActivated && Convert.ToBoolean(item.Cells[19].Text));
            this.chkAutenticatoLdap.Checked = (this.LdapIntegrationActivated && Convert.ToBoolean(item.Cells[24].Text));
            this.hdIdSincronizzazioneLdap.Value = item.Cells[20].Text;

            this.SetClientModelProcessor(item.Cells[21].Text);

            // tipo componenti
            /*
            bool isEnabledSmartClient;
            if (Boolean.TryParse(item.Cells[22].Text, out isEnabledSmartClient))
                this.rdbIsEnabledSmartClient.Checked = isEnabledSmartClient;

            if (item.Cells[29].Text != "0" && item.Cells[29].Text != "1")
            {
                this.rdbIsEnabledJavaApplet.Checked = item.Cells[29].Text == "3";
            }
            else
            {
                this.rdbIsEnabledJavaApplet.Checked = false;
                this.rdbDisableAll.Checked = true;
            }           
             */

            this.SetComponentiClient(item.Cells[29].Text);
            
            // conversione PDF
            bool smartClientConvPdfOnScan;
            if (Boolean.TryParse(item.Cells[23].Text, out smartClientConvPdfOnScan))
                this.chkSmartClientConvertPdfOnScan.Checked = smartClientConvPdfOnScan;

            //dispositivoStampa
            //////////////////////////////////////////////

            if (item.Cells[25].Text.Replace("&nbsp;", "") == "")
                this.ddlLabelPrinterType.SelectedIndex = this.ddlLabelPrinterType.Items.IndexOf(this.ddlLabelPrinterType.Items.FindByValue("null"));
            else
                this.ddlLabelPrinterType.SelectedIndex = this.ddlLabelPrinterType.Items.IndexOf(this.ddlLabelPrinterType.Items.FindByValue(item.Cells[25].Text));
            ////////////////////////////////////////////////

            //this.GUI("DatiUtente");


            #region MEV CS 1.3
            //
            // Se la chiave di DB "PGU_FE_DISABLE_AMM_GEST_CONS" è attiva, le funzionalità di conservazione in amministrazione vengono disabilitate
            string PGU_FE_DISABLE_AMM_GEST_CONS_Value = string.Empty;
            PGU_FE_DISABLE_AMM_GEST_CONS_Value = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "PGU_FE_DISABLE_AMM_GEST_CONS");

            bool DisableAmmGestCons = false;
            DisableAmmGestCons = ((PGU_FE_DISABLE_AMM_GEST_CONS_Value.Equals("0") || string.IsNullOrEmpty(PGU_FE_DISABLE_AMM_GEST_CONS_Value)) ? false : true);
            if (DisableAmmGestCons)
                this.cb_abilitatoCentroServizi.Visible = false;
            #endregion

            //if (!string.IsNullOrEmpty(msgLibroFirma))
            //{
            //    this.Page.RegisterStartupScript("alertJavaScript", msgLibroFirma);
            //}
        }

        /// <summary>
        /// Carica i dati utente a video
        /// </summary>
        /// <param name="e"></param>
        private void LoadDatiUtente(DataGridCommandEventArgs e)
        {
            string idUtenteSelezionato = e.Item.Cells[1].Text;//e.Item.Cells[2].Text.Replace("&nbsp;", "");
            //string msgLibroFirma = string.Empty;
            if (!string.IsNullOrEmpty(idUtenteSelezionato))
            {
                VerificaProcessiCoinvolti(idUtenteSelezionato);
                msgLibroFirma = getCoinvoltoInLibroFirma();
            }

            // userid ----------------------------------------------------------------
            this.txt_userid.Text = e.Item.Cells[2].Text.Replace("&nbsp;", "");
            // cognome ---------------------------------------------------------------
            this.txt_cognome.Text = e.Item.Cells[4].Text.Replace("&nbsp;", "");
            // nome ------------------------------------------------------------------
            this.txt_nome.Text = e.Item.Cells[5].Text.Replace("&nbsp;", "");
            // cod. rubrica ---------------------------------------------------------------
            this.txt_rubrica.Text = e.Item.Cells[7].Text.Replace("&nbsp;", "");
            // email -----------------------------------------------------------------
            this.txt_email.Text = e.Item.Cells[9].Text.Replace("&nbsp;", "");
            // amministratore --------------------------------------------------------

            // fromEmail -----------------------------------------------------------------
            this.txt_from_email.Text = e.Item.Cells[18].Text.Replace("&nbsp;", "");

            if (this.IsSupportedPasswordConfig())
            {
                this.chkUserPasswordNeverExpire.Checked = e.Item.Cells[17].Text.Replace("&nbsp;", "").ToLower().Equals("true");
            }

            //cb_amm.SelectedIndex = cb_amm.Items.IndexOf(cb_amm.Items.FindByValue(e.Item.Cells[8].Text));
            //this.btn_vociMenu.Visible=false;			
            //if(cb_amm.SelectedValue.Equals("3"))
            //{					
            //    this.btn_vociMenu.Visible=true;
            //    this.btn_vociMenu.Attributes.Add("onclick","ApriGestVociMenu('" + e.Item.Cells[0].Text + "','" + e.Item.Cells[14].Text + "');");
            //}

            // abilitato -------------------------------------------------------------
            this.cb_abilitato.Checked = false;
            if (e.Item.Cells[10].Text.Replace("&nbsp;", "") == "N")
                this.cb_abilitato.Checked = true;

            // abilitato centro servizi ----------------------------------------------
            this.cb_abilitatoCentroServizi.Checked = false;
            bool abilitatoCentroServizi;
            if (Boolean.TryParse(e.Item.Cells[26].Text, out abilitatoCentroServizi))
                this.cb_abilitatoCentroServizi.Checked = abilitatoCentroServizi;

            //
            // Mev CS 1.4 - Esibizione
            // abilitato Esibizione ----------------------------------------------
            this.cb_abilitatoEsibizione.Checked = false;
            bool abilitatoEsibizione;
            if (Boolean.TryParse(e.Item.Cells[30].Text, out abilitatoEsibizione))
                this.cb_abilitatoEsibizione.Checked = abilitatoEsibizione;
            // End Mev CS 1.4 - Esibizione
            //

            // campo matricola ----------------------------------------------
            this.txt_Matricola.Text = e.Item.Cells[27].Text.Replace("&nbsp;", "");

            // abilitato chiavi configurazione ----------------------------------------------
            this.cbx_chiavi.Checked = false;
            bool abilitatoChiaviConfig;
            if (Boolean.TryParse(e.Item.Cells[28].Text, out abilitatoChiaviConfig))
                this.cbx_chiavi.Checked = abilitatoChiaviConfig;

            // dominio ---------------------------------------------------------------
            this.txt_dominio.Text = e.Item.Cells[11].Text.Replace("&nbsp;", "");
            //Commentato per nuova gestione
            //if (this.txt_dominio.Text != "")
            //    this.txt_dominio.Text = this.txt_dominio.Text.Substring(0, (this.txt_dominio.Text.IndexOf("\\", 0)));
            // sede ------------------------------------------------------------------
            this.txt_sede.Text = e.Item.Cells[12].Text.Replace("&nbsp;", "");
            // notifica --------------------------------------------------------------
            if (e.Item.Cells[13].Text.Replace("&nbsp;", "") == "")
                this.ddl_notifica.SelectedIndex = this.ddl_notifica.Items.IndexOf(this.ddl_notifica.Items.FindByValue("null"));
            else
                this.ddl_notifica.SelectedIndex = this.ddl_notifica.Items.IndexOf(this.ddl_notifica.Items.FindByValue(e.Item.Cells[13].Text));

            //sabrina
            CheckTipoAmministratore(e.Item);
            this.GUI("DatiUtente");
            if (!this.VerificaUtConnesso(this.txt_userid.Text, e.Item.Cells[14].Text))
            {
                ArrayList listaRuoli = this.ListaRuoliUtente(e.Item.Cells[1].Text);

                if (listaRuoli != null && listaRuoli.Count > 0)
                {
                    this.btn_ruoli.Visible = true;
                    this.btn_ruoli.Attributes.Add("onclick", "ApriVisRuoli('" + e.Item.Cells[1].Text + "','" + Server.UrlEncode(e.Item.Cells[4].Text.Replace("'", "|@ap@|")) + " " + Server.UrlEncode(e.Item.Cells[5].Text.Replace("'", "|@ap@|")) + "');");
                }
            }

            if (this.btn_vociMenu.Visible)
                this.btn_vociMenu.Attributes.Add("onclick", "ApriGestVociMenu('" + e.Item.Cells[0].Text + "','" + e.Item.Cells[14].Text + "');");

            this.chkSyncronizeLdap.Checked = (this.LdapIntegrationActivated && Convert.ToBoolean(e.Item.Cells[19].Text));
            this.chkAutenticatoLdap.Checked = (this.LdapIntegrationActivated && Convert.ToBoolean(e.Item.Cells[24].Text));
            this.hdIdSincronizzazioneLdap.Value = e.Item.Cells[20].Text;

            this.SetClientModelProcessor(e.Item.Cells[21].Text);

            /**bool isEnabledSmartClient;
            if (Boolean.TryParse(e.Item.Cells[22].Text, out isEnabledSmartClient))
                this.rdbIsEnabledSmartClient.Checked = isEnabledSmartClient;

            if (e.Item.Cells[29].Text != "0" && e.Item.Cells[29].Text != "1")
            {
                this.rdbIsEnabledJavaApplet.Checked = e.Item.Cells[29].Text == "3";
            }
            else
            {
                this.rdbIsEnabledJavaApplet.Checked = false;
                this.rdbDisableAll.Checked = true;
            }*/

            this.SetComponentiClient(e.Item.Cells[29].Text);

            bool smartClientConvPdfOnScan;
            if (Boolean.TryParse(e.Item.Cells[23].Text, out smartClientConvPdfOnScan))
                this.chkSmartClientConvertPdfOnScan.Checked = smartClientConvPdfOnScan;

            //dispositivoStampa
            //////////////////////////////////////////////

            if (e.Item.Cells[25].Text.Replace("&nbsp;", "") == "")
                this.ddlLabelPrinterType.SelectedIndex = this.ddlLabelPrinterType.Items.IndexOf(this.ddlLabelPrinterType.Items.FindByValue("null"));
            else
                this.ddlLabelPrinterType.SelectedIndex = this.ddlLabelPrinterType.Items.IndexOf(this.ddlLabelPrinterType.Items.FindByValue(e.Item.Cells[25].Text));
            ////////////////////////////////////////////////

            //this.GUI("DatiUtente");

            #region MEV CS 1.3
            //
            // Se la chiave di DB "PGU_FE_DISABLE_AMM_GEST_CONS" è attiva, le funzionalità di conservazione in amministrazione vengono disabilitate
            string PGU_FE_DISABLE_AMM_GEST_CONS_Value = string.Empty;

            PGU_FE_DISABLE_AMM_GEST_CONS_Value = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "PGU_FE_DISABLE_AMM_GEST_CONS");

            bool DisableAmmGestCons = false;
            DisableAmmGestCons = ((PGU_FE_DISABLE_AMM_GEST_CONS_Value.Equals("0") || string.IsNullOrEmpty(PGU_FE_DISABLE_AMM_GEST_CONS_Value)) ? false : true);
            if (DisableAmmGestCons)
                this.cb_abilitatoCentroServizi.Visible = false;
            #endregion

            //if (!string.IsNullOrEmpty(msgLibroFirma))
            //{
            //    this.Page.RegisterStartupScript("alertJavaScript", msgLibroFirma);
            //}
        }

        /// <summary>
        /// verifica utente connesso a docspa
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>bool</returns>
        private bool VerificaUtConnesso(string userId, string idAmm)
        {
            bool retValue = false;

            try
            {
                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.VerificaUtenteLoggato(userId, idAmm);

                DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                esito = theManager.getEsitoOperazione();

                if (!esito.Codice.Equals(0))
                {
                    this.GUI("DatiUtenteConnesso");
                    retValue = true;
                }
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
                this.GUI("ResetAll");
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        private ArrayList ListaRuoliUtente(string idPeople)
        {
            Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
            theManager.RuoliUtente(idPeople);

            return theManager.getRuoliUtente();
        }
        /// <summary>
        /// Verifica se l'utente è responsabile di una AOO
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        private bool IsResponsabileAOO(string idPeople, string idGruppo)
        {
            Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
            return theManager.VerificaSeRespAOO(idPeople, idGruppo);
        }
        #endregion

        #region Gestione della lista utenti in sessione

        private void SetSessionDataSetUtenti(DataSet dsUtenti)
        {
            Session.Remove("UTENTIDATASET");
            Session["UTENTIDATASET"] = dsUtenti;
        }

        private DataSet GetSessionDataSetUtenti()
        {
            return (DataSet)Session["UTENTIDATASET"];
        }

        private void RemoveSessionDataSetUtenti()
        {
            this.GetSessionDataSetUtenti().Dispose();
            Session.Remove("UTENTIDATASET");
        }

        #endregion

        #region Gestione JavaScript

        private void AlertJS(string msg)
        {
            if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
            {
                string scriptString = "<SCRIPT>alert('" + msg.Replace("'", "\\'") + "');</SCRIPT>";
                this.Page.RegisterStartupScript("alertJavaScript", scriptString);
            }
        }

        private void ExecuteJS(string key)
        {
            if (!this.Page.IsStartupScriptRegistered("theJavaScript"))
            {
                string scriptString = "<SCRIPT>" + key + "</SCRIPT>";
                this.Page.RegisterStartupScript("theJavaScript", scriptString);
            }
        }

        #endregion

        #region Ricerca utenti

        /// <summary>
        /// Azione di ricerca utenti con i filtri correntemente impostati
        /// </summary>
        private void PerformActionFind()
        {
            // se non si è inserito il filtro, allora imposta la tendina a TUTTI
            if (this.txt_ricerca.Text.Trim() == string.Empty)
            {
                this.ddl_ricerca.SelectedIndex = 0;
                this.txt_ricerca.ReadOnly = true;
            }

            this.Inizialize(true, this.ddl_ricerca.SelectedValue, this.txt_ricerca.Text.Trim());
        }

        /// <summary>
        /// Tasto Cerca utenti
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_find_Click(object sender, System.EventArgs e)
        {
            this.PerformActionFind();
        }

        /// <summary>
        /// Change DDL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddl_ricerca_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.ddl_ricerca.SelectedValue.ToString().Equals("*"))
            {
                this.txt_ricerca.ReadOnly = true;
                this.txt_ricerca.Text = "";
            }
            else
            {
                this.txt_ricerca.ReadOnly = false;
                this.SetFocus(this.txt_ricerca);
            }
        }

        #endregion

        #region Gestione Amministratore

        private void cb_amm_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // MODIFICA
            if (this.btn_aggiungi.Text.Equals("Modifica"))
            {
                if (this.cb_amm.SelectedValue.Equals(this.dg_Utenti.SelectedItem.Cells[8].Text))
                {
                    this.btn_vociMenu.Visible = false;
                    // return;   //sabrina: non sono sicura di questo comportamento!!!
                }

                if (this.cb_amm.SelectedValue.Equals("3"))
                {
                    this.btn_vociMenu.Visible = true;
                    this.btn_vociMenu.Attributes.Add("onclick", "ApriGestVociMenu('" + this.dg_Utenti.SelectedItem.Cells[0].Text + "','" + this.dg_Utenti.SelectedItem.Cells[14].Text + "');");
                    this.ExecuteJS("ApriGestVociMenu('" + this.dg_Utenti.SelectedItem.Cells[0].Text + "','" + this.dg_Utenti.SelectedItem.Cells[14].Text + "');");
                }
                else
                    this.btn_vociMenu.Visible = false;

                if ((this.cb_amm.SelectedValue.Equals("0") || this.cb_amm.SelectedValue.Equals("1")) && this.dg_Utenti.SelectedItem.Cells[8].Text.Equals("3"))
                    this.AlertJS("Attenzione,\\nconfermando questa modifica tramite il tasto in basso a destra,\\nsaranno eliminate le voci di menù.");


            }

            // INSERIMENTO
            if (this.btn_aggiungi.Text.Equals("Aggiungi"))
            {
                if (this.cb_amm.SelectedValue.Equals("3"))
                {
                    this.cb_amm.SelectedIndex = 0;
                    this.AlertJS("Attenzione,\\nla qualifica di USER amministratore può essere assegnata solo dopo aver salvato i dati del nuovo utente.\\n\\nCome procedere:\\n1 - inserire e salvare prima i dati di questo utente\\n2- accedere poi ai suoi dati in modalità 'Modifica'\\n3 - quindi impostare la qualifica di USER amministratore");
                }
            }

            if (this.cb_amm.SelectedValue.Equals("0"))
                this.cbx_chiavi.Checked = true;

            this.chkSyncronizeLdap.Checked = (this.LdapIntegrationActivated && this.cb_amm.SelectedValue == "0");
            this.chkAutenticatoLdap.Checked = (this.LdapIntegrationActivated && this.cb_amm.SelectedValue == "0");
        }

        private bool EliminaRegistriAdmin()
        {
            bool retValue = true;

            Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
            DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();

            if (!this.cb_amm.SelectedValue.Equals("3"))
            {
                theManager.EliminaRegistriUtente(this.dg_Utenti.SelectedItem.Cells[0].Text);
                esito = theManager.getEsitoOperazione();

                if (!esito.Codice.Equals(0))
                {
                    this.AlertJS("Attenzione, " + esito.Descrizione);
                    retValue = false;
                }
            }
            return retValue;
        }
        private bool EliminaMenuAdmin()
        {
            bool retValue = true;
            string eliminaMenuAdmin = string.Empty;
            string comboUser = string.Empty;
            if (this.dg_Utenti != null && this.dg_Utenti.SelectedItem != null)
                eliminaMenuAdmin = this.dg_Utenti.SelectedItem.Cells[0].Text;
            else
                if (Session["datagridUser"] != null)
                {
                    DataGridItem dg_user = (DataGridItem)Session["datagridUser"];
                    eliminaMenuAdmin = dg_user.Cells[0].Text;
                }

            if (Session["comboUser"] != null)
            {
                comboUser = (string)Session["comboUser"];
                Session.Remove("comboUser");
            }
            else
                if (this.cb_amm != null && this.cb_amm.SelectedValue != null)
                    comboUser = this.cb_amm.SelectedValue;

            Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
            DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();

            if (!comboUser.Equals("3"))
            {
                theManager.EliminaMenuUtente(eliminaMenuAdmin);
                esito = theManager.getEsitoOperazione();

                if (!esito.Codice.Equals(0))
                {
                    this.AlertJS("Attenzione, " + esito.Descrizione);
                    retValue = false;
                }
                else
                {
                    theManager.EliminaRegistriUtente(eliminaMenuAdmin);
                    esito = theManager.getEsitoOperazione();

                    if (!esito.Codice.Equals(0))
                    {
                        this.AlertJS("Attenzione, " + esito.Descrizione);
                        retValue = false;
                    }
                }
            }
            return retValue;
        }


        #endregion

        #region Eventi Controlli
        protected void txt_userid_TextChanged(object sender, EventArgs e)
        {
            if (txt_userid.Text != "")
            {
                txt_rubrica.Text = txt_userid.Text;
            }
        }
        #endregion Fine Eventi Controlli

        #region importazione utenti da foglio excel
        protected void btn_importa_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "importaUtenti", "ApriImportaUtenti();", true);
        }
        #endregion

        #region Gestione scadenza password

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected int GetIdAmministrazione()
        {
            return Convert.ToInt32(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3"));
        }

        protected string GetDominioAmministrazione()
        {
            return AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "2");
        }



        /// <summary>
        /// Verifica se, per l'amministrazione, la gestione scadenza password è abilitata o meno
        /// </summary>
        /// <returns></returns>
        protected bool IsSupportedPasswordConfig()
        {
            if (this.ViewState["IsSupportedPasswordConfig"] == null)
            {
                DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                this.ViewState["IsSupportedPasswordConfig"] = ws.AdminIsSupportedPasswordConfig();
            }

            return Convert.ToBoolean(this.ViewState["IsSupportedPasswordConfig"]);
        }



        #endregion

        #region Gestione sincronizzazione utenti da ldap

        /// <summary>
        /// Indica che l'integrazione con ldap è attivata o meno
        /// </summary>
        protected bool LdapIntegrationActivated
        {
            get
            {
                if (this.ViewState["LdapIntegrationActivated"] == null)
                {
                    DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

                    DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();

                    try
                    {
                        DocsPAWA.DocsPaWR.LdapConfig config = ws.GetLdapConfig(sessionManager.getUserAmmSession(), this.GetIdAmministrazione().ToString());

                        this.ViewState["LdapIntegrationActivated"] = (config != null && config.LdapIntegrationActive);
                    }
                    catch (Exception ex)
                    {
                        DocsPaUtils.Exceptions.SoapExceptionParser.ThrowOriginalException(ex);
                    }
                }

                return Convert.ToBoolean(this.ViewState["LdapIntegrationActivated"].ToString());
            }
        }

        /// <summary>
        /// Per non creare disallineamenti, vengono disabilitati i controlli (in modifica utente esistente)
        /// i cui valori provengono dalle utenze ldap
        /// </summary>
        protected void SetLdapControlsEnabled()
        {
            this.trIntegrazioneLdap.Visible = this.LdapIntegrationActivated;

            bool insertMode = (this.dg_Utenti.SelectedItem == null);

            bool enabled = true;

            if (this.LdapIntegrationActivated)
            {
                bool isAmministratore = false;

                if (!insertMode)
                {
                    // Gli utenti amministratori non sono soggetti alla sincronizzazione ldap
                    // NB: I dati dell'utente vengono reperiti dall'elemento correntemente selezionato nel datagrid
                    isAmministratore = (this.cb_amm.SelectedValue != "0");

                    enabled = (isAmministratore || !this.chkSyncronizeLdap.Checked);
                }
                else
                {
                    enabled = true;
                }

                this.lblSyncronizeLdap.Visible = true;
                this.chkSyncronizeLdap.Visible = true;
                this.chkSyncronizeLdap.Enabled = !isAmministratore;
                this.chkAutenticatoLdap.Visible = true;
                this.chkAutenticatoLdap.Enabled = !isAmministratore;
            }
            else
            {
                this.lblSyncronizeLdap.Visible = false;
                this.chkSyncronizeLdap.Visible = false;
                this.chkAutenticatoLdap.Visible = false;
            }

            this.txt_userid.Enabled = enabled;
            this.txt_Matricola.Enabled = enabled;
            this.txt_email.Enabled = enabled;
            this.txt_cognome.Enabled = enabled;
            this.txt_nome.Enabled = enabled;
            this.txt_sede.Enabled = enabled;
            this.txt_email.Enabled = enabled;
        }

        /// <summary>
        /// Handler pulsante si sincronizzazione utenti da ldap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSyncronizeLdap_Click(object sender, EventArgs e)
        {
            this.PerformActionFind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkAutenticatoLdap_OnCheckedChanged(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkAbilitato_OnCheckedChanged(object sender, EventArgs e)
        {
            /*string alert = string.Empty;
            if (!this.cb_abilitato.Checked)
            {
                string stridUtente = this.dg_Utenti.SelectedItem.Cells[1].Text;
                if (!string.IsNullOrEmpty(stridUtente))
                    VerificaProcessiCoinvolti(stridUtente);
                if (this.dg_Utenti.SelectedItem.Cells[10].Text.Replace("&nbsp;", "") == "N")
                {
                    alert = getCoinvoltoInLibroFirma();
                }

                if (!string.IsNullOrEmpty(alert)) this.AlertJS(alert);
            }
            return;
             * */
        }

        #endregion

        #region Gestione client model processor

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int GetSelectedProcessorId()
        {
            if (this.cboClientModelProcessor.SelectedItem != null)
                return Convert.ToInt32(this.cboClientModelProcessor.SelectedItem.Value);
            else
                return 0;
        }

        /// <summary>
        /// Caricamento dati relativi al model processor per l'amministrazione
        /// </summary>
        /// <param name="currAmm"></param>
        private void SetClientModelProcessor(string idClientModelProcessor)
        {
            if (this.IsEnabledClientModelsProcessor())
                this.cboClientModelProcessor.SelectedValue = (string.IsNullOrEmpty(idClientModelProcessor) ? "0" : idClientModelProcessor);
        }        

        /// <summary>
        /// Verifica se la creazione dei modelli è abilitata o meno
        /// </summary>
        /// <returns></returns>
        private bool IsEnabledClientModelsProcessor()
        {
            int result;
            Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["MODELLO_DOCUMENTO"], out result);
            return (result > 0);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetClientModelControlsVisibility()
        {
            this.cboClientModelProcessor.Visible = this.IsEnabledClientModelsProcessor();
            this.lblClientModelProcessor.Visible = this.cboClientModelProcessor.Visible;
        }

        /// <summary>
        /// Impostazione visibilità dei controlli legati alla matricola
        /// </summary>
        private void SetMatricolaControlsVisibility()
        {
            string configValue = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_GESTIONE_MATRICOLE");

            this.trMatricola.Visible = (!string.IsNullOrEmpty(configValue) && configValue == "1");
        }

        /// <summary>
        /// Caricamento combo dei word processors
        /// </summary>
        private void FetchComboClientModelProcessors()
        {
            if (this.IsEnabledClientModelsProcessor())
            {
                using (DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService())
                {
                    DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();

                    foreach (DocsPAWA.DocsPaWR.ModelProcessorInfo processor in ws.GetModelProcessors(sessionManager.getUserAmmSession()))
                    {
                        this.cboClientModelProcessor.Items.Add(new ListItem(processor.name, processor.id.ToString()));
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Imposta scelta tipo componenti
        /// </summary>
        /// <param name="tipoComponenti"></param>
        private void SetComponentiClient(string tipoComponenti)
        {
            switch (tipoComponenti)
            {
                // activeX
                case "1":
                    this.rdbDisableAll.Checked = false;
                    this.rdbIsEnabledActiveX.Checked = true;
                    this.rdbIsEnabledSmartClient.Checked = false;
                    this.rdbIsEnabledJavaApplet.Checked = false;
                    this.rdbIsEnabledHTML5Socket.Checked = false;
                    break;

                // smartclient
                case "2":
                    this.rdbDisableAll.Checked = false;
                    this.rdbIsEnabledActiveX.Checked = false;
                    this.rdbIsEnabledSmartClient.Checked = true;
                    this.rdbIsEnabledJavaApplet.Checked = false;
                    this.rdbIsEnabledHTML5Socket.Checked = false;
                    break;

                // applet
                case "3":
                    this.rdbDisableAll.Checked = false;
                    this.rdbIsEnabledActiveX.Checked = false;
                    this.rdbIsEnabledSmartClient.Checked = false;
                    this.rdbIsEnabledJavaApplet.Checked = true;
                    this.rdbIsEnabledHTML5Socket.Checked = false;
                    break;

                // 
                case "4":
                    this.rdbDisableAll.Checked = false;
                    this.rdbIsEnabledActiveX.Checked = false;
                    this.rdbIsEnabledSmartClient.Checked = false;
                    this.rdbIsEnabledJavaApplet.Checked = false;
                    this.rdbIsEnabledHTML5Socket.Checked = true;
                    break;

                // default amministrazione
                case "0":
                default:
                    this.rdbDisableAll.Checked = true;
                    this.rdbIsEnabledActiveX.Checked = false;
                    this.rdbIsEnabledSmartClient.Checked = false;
                    this.rdbIsEnabledJavaApplet.Checked = false;
                    this.rdbIsEnabledHTML5Socket.Checked = false;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkIsEnabledSmartClient_OnCheckedChanged(object sender, EventArgs e)
        {
            this.chkSmartClientConvertPdfOnScan.Enabled = (this.rdbIsEnabledSmartClient.Checked || this.rdbIsEnabledJavaApplet.Checked || this.rdbIsEnabledHTML5Socket.Checked);
            this.chkSmartClientConvertPdfOnScan.Checked = this.chkSmartClientConvertPdfOnScan.Checked && this.chkSmartClientConvertPdfOnScan.Enabled;
            
        }

        /// <summary>
        /// 
        /// </summary>
        protected string CurrentIdUser
        {
            get
            {
                if (this.dg_Utenti.SelectedItem != null)
                    return this.dg_Utenti.SelectedItem.Cells[1].Text;
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Verifica se la checkBox cb_abilitaEsibizione è visibile o meno
        /// Come impostazione di default se la chiave di web.config è assente o presente ma impostata a 1, la Checkbox sarà visibile
        /// Viceversa, se presente ma impostata a 0, non sarà visibile
        /// </summary>
        /// <returns></returns>
        protected bool isCBEsibizioneVisible()
        {
            // Default: checkBox visibile
            bool visible = true;
            string Visualizza_cb_esibizione = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["VISUALIZZA_CB_ESIBIZIONE"]))
                {
                    // Recupero il valore della chiave di web.config
                    Visualizza_cb_esibizione = System.Configuration.ConfigurationManager.AppSettings["VISUALIZZA_CB_ESIBIZIONE"].ToString();
                }

                // Solo se la chiave è presente e volutamente impostata a 0, allora la check viene nascosta.
                // In tutti gli altri casi è visibile
                if (!string.IsNullOrEmpty(Visualizza_cb_esibizione) && Visualizza_cb_esibizione.Equals("0")) 
                {
                    visible = false;
                }
            }
            catch (Exception e) 
            {
                visible = true;
            }
            
            return visible;
        }

        /// <summary>
        /// INTEGRAZIONE PITRE-PARER
        /// Determina se è attiva la conservazione PARER
        /// </summary>
        /// <returns></returns>
        protected bool IsConservazionePARER()
        {
            bool result = false;

            string IS_CONSERVAZIONE_PARER = string.Empty;
            IS_CONSERVAZIONE_PARER = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_WA_CONSERVAZIONE");
            result = ((string.IsNullOrEmpty(IS_CONSERVAZIONE_PARER) || IS_CONSERVAZIONE_PARER.Equals("0")) ? false : true);

            return result;
        }
    }
}
