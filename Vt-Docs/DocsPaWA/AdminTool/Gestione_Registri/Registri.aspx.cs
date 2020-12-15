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
using System.IO;
using System.Xml;
using Microsoft.Web.UI.WebControls;
using log4net;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;
using DocsPAWA.AdminTool.UserControl;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using DocsPAWA.SiteNavigation;
using System.Configuration;

namespace Amministrazione.Gestione_Registri
{
    /// <summary>
    /// Summary description for TipiFunzione.
    /// </summary>
    public class Registri : System.Web.UI.Page
    {
        private ILog logger = LogManager.GetLogger(typeof(Registri));
        #region WebControls e variabili
        protected System.Web.UI.HtmlControls.HtmlForm Form1;
        protected System.Web.UI.WebControls.Label lbl_position;
        protected System.Web.UI.WebControls.Button btn_nuova;
        protected System.Web.UI.WebControls.Panel pnl_info;
        protected System.Web.UI.WebControls.TextBox txt_codice;
        protected System.Web.UI.WebControls.TextBox txt_descrizione;
        // protected System.Web.UI.WebControls.TextBox txt_UtInteropNoMail;
        protected System.Web.UI.WebControls.TextBox txt_RuoloInteropNoMail;
        protected System.Web.UI.WebControls.Button btn_aggiungi;
        protected System.Web.UI.WebControls.DataGrid dg_Registri;
        protected System.Web.UI.WebControls.TextBox txtindirizzo;
        protected System.Web.UI.WebControls.TextBox txt_utente;
        protected System.Web.UI.WebControls.TextBox txt_password;
        protected System.Web.UI.WebControls.TextBox txt_conferma_pwd;
        protected System.Web.UI.WebControls.TextBox txt_pop;
        protected System.Web.UI.WebControls.TextBox txt_portapop;
        protected System.Web.UI.WebControls.TextBox txt_smtp;
        protected System.Web.UI.WebControls.TextBox txt_portasmtp;
        protected System.Web.UI.WebControls.Label lbl_cod;
        protected System.Web.UI.WebControls.Label lbl_tit;
        protected System.Web.UI.WebControls.ImageButton btn_chiudiPnlInfo;
        protected System.Web.UI.WebControls.Panel panel_InteropNoMail;
        protected System.Web.UI.WebControls.ImageButton btn_RubricaUtente;
        protected System.Web.UI.WebControls.ImageButton btn_RubricaRuolo;
        protected DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
        protected System.Web.UI.WebControls.TextBox txt_IdHiddenRuoloNomail;
        protected System.Web.UI.WebControls.TextBox txt_IdHiddenUtenteNomail;
        // protected System.Web.UI.WebControls.ImageButton img_delUtenteNoMail;
        protected System.Web.UI.WebControls.ImageButton img_delRuoloNoMail;
        protected System.Web.UI.WebControls.CheckBox ChkBoxPopSSl;
        protected System.Web.UI.WebControls.CheckBox ChkBoxsmtp;
        protected System.Web.UI.WebControls.CheckBox ChkBoxsmtpSTA;
        protected System.Web.UI.WebControls.Label lblTitle;
        protected System.Web.UI.WebControls.Panel panel_RuoloResp;
        //---------------------------------------------------------------------------------------------
        protected System.Web.UI.WebControls.CheckBox ChkAutomatico;
        protected System.Web.UI.WebControls.TextBox txt_userid_smtp;
        protected System.Web.UI.WebControls.TextBox txt_pwd_smtp;
        protected System.Web.UI.WebControls.TextBox txt_conferma_pwd_smtp;
        protected System.Web.UI.WebControls.TextBox txt_indirizzo;
        protected System.Web.UI.WebControls.DropDownList ddl_automatico;
        protected System.Web.UI.WebControls.DropDownList ddl_UtInteropNoMail;
        protected System.Web.UI.WebControls.DropDownList ddl_DirittoResp;
        protected System.Web.UI.WebControls.Label lblDirittoResp;
        protected System.Web.UI.WebControls.Label lbl_ruoloRespReg;
        protected System.Web.UI.WebControls.DropDownList ddl_ricevutaPec;
        //---------------------------------------------------------------------------------------------
        protected System.Web.UI.WebControls.TextBox txt_imap;
        protected System.Web.UI.WebControls.TextBox txt_portaImap;
        protected System.Web.UI.WebControls.TextBox txt_inbox;
        protected System.Web.UI.WebControls.TextBox txt_mailElaborate;
        protected System.Web.UI.WebControls.DropDownList ddl_posta;
        protected System.Web.UI.WebControls.CheckBox Chk_sslImap;
        protected System.Web.UI.WebControls.TextBox txt_mailNonElaborate;
        protected System.Web.UI.WebControls.CheckBox ChkMailPec;
        // Per gestione pendenti tramite PEC
        protected System.Web.UI.WebControls.CheckBox ChkMailRicevutePendenti;
        protected System.Web.UI.WebControls.Label ChkMailRicevutePendentiText;
        protected System.Web.UI.WebControls.Button btn_test;
        #endregion

        // Costanti che identificano i nomi delle colonne del datagrid
        private const string TABLE_COL_ID = "ID";
        private const string TABLE_COL_CODICE = "Codice";
        private const string TABLE_COL_DESCRIZIONE = "Descrizione";
        private const string TABLE_COL_EMAIL = "Email";
        private const string TABLE_COL_DISABILITATO = "Disabilitato";

        private const int GRID_COL_ID = 0;
        private const int GRID_COL_CODICE = 1;
        private const int GRID_COL_DESCRIZIONE = 2;
        private const int GRID_COL_EMAIL = 3;
        private const int GRID_COL_DISABILITATO = 4;
        private const int GRID_COL_DETAIL = 5;

        protected System.Web.UI.HtmlControls.HtmlInputHidden onDeleteRegistro;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtCommandPending;
        protected System.Web.UI.WebControls.Label lbl_msg;
        protected System.Web.UI.WebControls.Button btn_UOsmista;
        protected System.Web.UI.WebControls.Label lblDescAutoInterop;
        protected DocsPAWA.DocsPaWR.InfoUtenteAmministratore datiAmministratore;
        protected System.Web.UI.WebControls.TextBox ruoloRespReg;
        protected System.Web.UI.WebControls.TextBox codRuoloRespReg;
        protected System.Web.UI.WebControls.HiddenField txt_IdHiddenRuoloResp;
        private const int GRID_COL_DELETE = 6;
        protected System.Web.UI.WebControls.CheckBox ChkSospeso;

        protected System.Web.UI.WebControls.Panel pnl_ricevute;
        protected System.Web.UI.WebControls.Button btn_upload_ricevuta;
        protected System.Web.UI.WebControls.FileUpload fileupload_ricevuta;
        protected System.Web.UI.WebControls.Label lbl_ricevuta;
        protected System.Web.UI.WebControls.ImageButton btn_elimina_ricevuta;
        protected System.Web.UI.WebControls.ImageButton btn_RubricaRuoloResp;
        protected System.Web.UI.WebControls.ImageButton img_delRuoloResp;
        protected System.Web.UI.UpdatePanel UpdatePanelRegistri;
        protected System.Web.UI.UpdatePanel updPanelCasella;
        protected System.Web.UI.UpdatePanel updPanelSMTP;
        protected System.Web.UI.UpdatePanel UpdatePanel1PostaIngresso;
        protected System.Web.UI.UpdateProgress updProgressCasella;
        protected System.Web.UI.WebControls.ImageButton img_aggiungiCasella;
        protected System.Web.UI.WebControls.TextBox txt_note;
        protected System.Web.UI.WebControls.DropDownList ddl_caselle;
        protected System.Web.UI.WebControls.CheckBox cbx_casellaPrincipale;
        protected System.Web.UI.WebControls.ImageButton img_eliminaCasella;
        private System.Drawing.Color coloreBianco;
        private System.Drawing.Color coloreGrigio;
        protected bool ricevutaPdf;
        protected System.Web.UI.UpdatePanel pnl_pregresso;
        protected System.Web.UI.WebControls.CheckBox chk_pregresso;

        protected System.Web.UI.WebControls.CheckBox cbx_invioAuto;
        protected System.Web.UI.WebControls.TextBox txt_anno_reg_pre;

        protected System.Web.UI.WebControls.Label lbl_utRespStampa;
        protected System.Web.UI.WebControls.DropDownList ddl_user;

        protected System.Web.UI.WebControls.TextBox txt_msg_posta_in_uscita;
        protected System.Web.UI.WebControls.CheckBox cbx_sovrascrivi_messaggio;
        protected System.Web.UI.WebControls.Button btn_SbloccaCasella;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_ReturnValueProcessiFirmaRegistroRF;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_ReturnValueProcessiFirmaRegistro;

        #region Repertori
        /// <summary>
        /// Datagrid con le informaizoni sui registri di repertorio dell'amministrazione
        /// </summary>
        protected System.Web.UI.WebControls.DataGrid dgRepertori;

        /// <summary>
        /// DataSource per il datagrid e per il DetailsView
        /// </summary>
        protected ObjectDataSource RegistriRepertorioManagerDataSource;

        #endregion

        #region Interoperabilità semplificata

        /// <summary>
        /// Controllo con le impostazioni del registro relativamente all'interoperabilità 
        /// semplificata
        /// </summary>
        protected DocsPAWA.AdminTool.UserControl.InteroperabilitySettings isRegistrySettings;

        /// <summary>
        /// Titolo del pannello con le impostazioni relative all'IS
        /// </summary>
        protected System.Web.UI.HtmlControls.HtmlTableRow titoloIS;

        #endregion

        #region Page_Load

        private void Page_Load(object sender, System.EventArgs e)
        {
            // Il titolo della sezione IS deve essere visualizzato solo se è attiva l'IS
            this.titoloIS.Visible = InteroperabilitaSemplificataManager.IsEnabledSimpInterop;

            if (CallContextStack.CurrentContext == null)
                CallContextStack.CurrentContext = new CallContext("GestioneRegistri");

            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            string idAmministrazione = ws.getIdAmmByCod(codiceAmministrazione);
            //DocsPAWA.DocsPaWR.InfoUtente info = new DocsPAWA.DocsPaWR.InfoUtente();
            //info =DocsPAWA.UserManager.getInfoUtente( //DocsPAWA.UserManager.getInfoUtente(this.Page);
            string valoreChiave = DocsPAWA.utils.InitConfigurationKeys.GetValue(idAmministrazione, "FE_RICEVUTA_PROTOCOLLO_PDF");
            if (valoreChiave != null)
            {
                if (valoreChiave.Equals("0")) ricevutaPdf = false;
                else ricevutaPdf = true;

            }

            Session["AdminBookmark"] = "Registri";

            //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
            if (Session.IsNewSession)
            {
                Response.Redirect("../Exit.aspx?FROM=EXPIRED");
            }

            AmmUtils.WebServiceLink wws = new AmmUtils.WebServiceLink();
            if (!wws.CheckSession(Session.SessionID))
            {
                Response.Redirect("../Exit.aspx?FROM=ABORT");
            }
            // ---------------------------------------------------------------

            if (!String.IsNullOrEmpty(this.hd_ReturnValueProcessiFirmaRegistro.Value)
                && this.hd_ReturnValueProcessiFirmaRegistro.Value != "undefined")
            {
                this.hd_ReturnValueProcessiFirmaRegistro.Value = string.Empty;
                Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                DocsPAWA.DocsPaWR.OrgRegistro registro = new DocsPAWA.DocsPaWR.OrgRegistro();
                this.RefreshRegistroFromUI(ref registro);
                DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                theManager.InvalidaProcessiFirmaByIdRegistroAndEmailRegistro(registro.IDRegistro, string.Empty, sessionManager.getUserAmmSession());
                UpdateRegistro(registro);
            }

            this.RegisterScrollKeeper("DivDGList");

            // Inizializzazione hashtable businessrules
            this.InitializeBusinessRuleControls();

            if (!IsPostBack)
            {
                this.AddControlsClientAttribute();

                // Caricamento lista registri
                this.FillListRegistri();

                this.lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
            }

            if (this.CommandPending.Equals("DELETE"))
            {
                this.Delete(true);
                this.CommandPending = string.Empty;
            }

            if (Session["selRuoloRespReg"] != null)
            {
                addRuoloRespReg((DocsPAWA.DocsPaWR.ElementoRubrica[])Session["selRuoloRespReg"]);
            }

            if (!String.IsNullOrEmpty(this.hd_ReturnValueProcessiFirmaRegistroRF.Value)
                && this.hd_ReturnValueProcessiFirmaRegistroRF.Value != "undefined")
            {
                this.hd_ReturnValueProcessiFirmaRegistroRF.Value = string.Empty;
                Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                //this.InvalidaProcessiFirmaRegistriCoinvolti();
                foreach (DocsPAWA.DocsPaWR.CasellaRegistro c in Caselle)
                {
                    if (c.EmailRegistro.Equals(ddl_caselle.SelectedValue))
                    {
                        DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                        theManager.InvalidaProcessiFirmaByIdRegistroAndEmailRegistro(c.IdRegistro, c.EmailRegistro, sessionManager.getUserAmmSession());
                        EliminaCasella(c);
                        break;
                    }
                }
            }

            if (ws.getValueInteropNoMail())
            {
                //Carico il ruolo interoperabilità nomail
                if (Session["selRuoloRegNoMail"] != null)
                {
                    addRuoloInteropNoMail((DocsPAWA.DocsPaWR.ElementoRubrica[])Session["selRuoloRegNoMail"]);
                }
                this.panel_InteropNoMail.Visible = true;
                this.panel_RuoloResp.Visible = true;


                // EM
                //Carico l'utente interoperabilità nomail
                //if (Session["selUtenteRegNoMail"] != null)
                //{
                //    addUtenteInteropNoMail((DocsPAWA.DocsPaWR.ElementoRubrica[])Session["selUtenteRegNoMail"]);
                //}
            }
            coloreBianco = System.Drawing.Color.FromName(txt_password.BackColor.Name);
            coloreGrigio = System.Drawing.Color.FromName(ruoloRespReg.BackColor.Name);
            //modifica
            switch (ddl_posta.SelectedValue)
            {
                case "":
                    {
                        Chk_sslImap.Checked = false;
                        ChkBoxPopSSl.Checked = false;
                        ChkMailPec.Checked = false;

                        // Per gestione pendenti tramite PEC
                        ChkMailRicevutePendenti.Checked = false;
                        ChkMailRicevutePendenti.Enabled = true;

                        txt_imap.Enabled = false;
                        txt_pop.Enabled = false;
                        txt_portaImap.Enabled = false;
                        txt_portapop.Enabled = false;
                        txt_inbox.Enabled = false;
                        txt_mailElaborate.Enabled = false;
                        Chk_sslImap.Enabled = false;
                        ChkBoxPopSSl.Enabled = false;
                        txt_mailNonElaborate.Enabled = false;
                        break;
                    }

                case "POP":
                    {

                        txt_imap.Enabled = false;
                        txt_pop.Enabled = !false;
                        txt_portaImap.Enabled = false;
                        txt_portapop.Enabled = !false;
                        txt_inbox.Enabled = false;
                        txt_mailElaborate.Enabled = false;
                        Chk_sslImap.Enabled = false;
                        ChkBoxPopSSl.Enabled = !false;
                        txt_mailNonElaborate.Enabled = false;
                        break;
                    }
                case "IMAP":
                    {

                        txt_imap.Enabled = !false;
                        txt_pop.Enabled = false;
                        txt_portaImap.Enabled = !false;
                        txt_portapop.Enabled = false;
                        txt_inbox.Enabled = !false;
                        txt_mailElaborate.Enabled = !false;
                        Chk_sslImap.Enabled = !false;
                        ChkBoxPopSSl.Enabled = false;
                        txt_mailNonElaborate.Enabled = !false;
                        break;
                    }

            }
            //fine modifica

            //Campo per import pregressi
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_IMPORT_PREGRESSI")) && DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_IMPORT_PREGRESSI").Equals("1"))
                {
                    pnl_pregresso.Visible = true;
                }
                else
                {
                    pnl_pregresso.Visible = false;
                }
            }

        }

        private void SetFocus(System.Web.UI.Control ctrl)
        {
            string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "focus", s, false);
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
            this.btn_nuova.Click += new System.EventHandler(this.btn_nuova_Click);
            this.dg_Registri.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dg_Registri_ItemCreated);
            this.dg_Registri.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_Registri_ItemCommand);
            this.dg_Registri.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DeleteItemCommand);
            this.btn_chiudiPnlInfo.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudiPnlInfo_Click);
            this.btn_aggiungi.Click += new System.EventHandler(this.btn_aggiungi_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.dg_Registri.PreRender += new System.EventHandler(dg_Registri_PreRender);
            this.btn_upload_ricevuta.Click += new System.EventHandler(this.btn_upload_ricevuta_Click);
            this.btn_elimina_ricevuta.Click += new System.Web.UI.ImageClickEventHandler(this.btn_elimina_ricevuta_Click);
            this.btn_test.Click += new System.EventHandler(this.btn_test_Click);

            // Associazione dell'evento ItemCommand per il datagrid dei registri di repertorio
            this.dgRepertori.ItemCommand += new DataGridCommandEventHandler(this.dgRepertori_ItemCommand);

            //multicasella
            this.img_aggiungiCasella.Click += new ImageClickEventHandler(this.img_aggiungiCasella_Click);
            this.ddl_caselle.SelectedIndexChanged += new EventHandler(this.ddl_caselle_Changed);
            this.cbx_casellaPrincipale.CheckedChanged += new EventHandler(this.cbx_casellaPrincipale_CheckedChanged);
            this.ChkMailPec.CheckedChanged += new EventHandler(this.cbx_ChkMailPec_CheckedChanged);
            this.img_eliminaCasella.Click += new ImageClickEventHandler(this.img_eliminaCasella_Click);
            this.btn_SbloccaCasella.Click += new EventHandler(this.btn_SbloccaCasella_Click);
        }

        void dg_Registri_PreRender(object sender, System.EventArgs e)
        {
            for (int i = 0; i < this.dg_Registri.Items.Count; i++)
            {
                string disab = ((TableCell)this.dg_Registri.Items[i].Cells[GRID_COL_DISABILITATO]).Text;
                if (disab.ToUpper().Equals("TRUE"))
                {
                    this.dg_Registri.Items[i].ForeColor = Color.Red;
                }
                //this.dg_Registri.Items[i].Cells[GRID_COL_STATO].HorizontalAlign = HorizontalAlign.Center;
            }
        }

        #endregion

        #region dg_Registri

        private void RegisterScrollKeeper(string divID)
        {
            DocsPAWA.AdminTool.UserControl.ScrollKeeper scrollKeeper = new DocsPAWA.AdminTool.UserControl.ScrollKeeper();
            scrollKeeper.WebControl = divID;
            this.Form1.Controls.Add(scrollKeeper);
        }

        /// <summary>
        /// dg_Registri_ItemCommand
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void dg_Registri_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            string commandName = e.CommandName;

            if (commandName.Equals("Select"))
            {
                //visibilià informazioni
                pnl_info.Visible = true;
                btn_aggiungi.Text = "Modifica";
                txt_codice.Visible = false;
                lbl_cod.Visible = true;
                SetFocus(txt_descrizione);

                this.lbl_ruoloRespReg.Visible = true;
                this.ruoloRespReg.Visible = true;
                this.btn_RubricaRuoloResp.Visible = true;
                this.img_delRuoloResp.Visible = true;
                //if(ruoloRespReg.ReadOnly)
                //    this.ddl_DirittoResp.Visible = false;
                //else
                this.ddl_DirittoResp.Visible = true;
                this.lblDirittoResp.Visible = true;

                // Assegnazione valori campi UI
                DocsPAWA.DocsPaWR.OrgRegistro registro = this.GetRegistro(e.Item.Cells[GRID_COL_ID].Text);

                //Andrea De Marco - Popolamento chk_pregresso e txt_anno_reg_pre in pnl_pregresso
                if (registro.flag_pregresso)
                {
                    this.chk_pregresso.Checked = true;
                    this.txt_anno_reg_pre.Text = registro.anno_pregresso.ToString();
                    this.pnl_pregresso.Update();
                    
                    //Disabilito gli elementi che non rientrano nelle funzionalità import pregressi
                    this.ChkAutomatico.Checked = false;
                    this.ChkAutomatico.Enabled = false;
                    this.ChkSospeso.Checked = false;
                    this.ChkSospeso.Enabled = false;
                    
                    //Quando seleziono un registro di pregresso i campi relativi al ruolo responsabile devono essere disabilitati.
                    this.ruoloRespReg.Enabled = false;
                    this.btn_RubricaRuoloResp.Enabled = false;
                    this.img_delRuoloResp.Enabled = false;
                    this.ddl_DirittoResp.Enabled = false;
                    this.ddl_user.Enabled = false;
                    
                    
                }
                else
                {
                    this.chk_pregresso.Checked = false;
                    this.txt_anno_reg_pre.Text = string.Empty;
                    this.pnl_pregresso.Update();

                    this.ChkAutomatico.Enabled = true;
                    this.ChkSospeso.Enabled = true;

                    //Quando seleziono un registro di pregresso i campi relativi al ruolo responsabile devono essere disabilitati.
                    this.ruoloRespReg.Enabled = true;
                    this.btn_RubricaRuoloResp.Enabled = true;
                    this.img_delRuoloResp.Enabled = true;
                    this.ddl_DirittoResp.Enabled = true;

                }
                //End Andrea De Marco

                // Impostazione dell'id del registro nel controllo dell'interoperabilità semplificata
                // per il caricamento delle impostazioni
                this.isRegistrySettings.RegistryId = registro.IDRegistro;

                //e.Item.Cells[GRID_COL_ID].Text);

                if (!registro.idRuoloResp.Equals(""))
                {
                    this.codRuoloRespReg.Text = registro.idRuoloResp;
                    DocsPAWA.DocsPaWR.Ruolo rr = ws.getRuoloById(registro.idRuoloResp);
                    this.ruoloRespReg.Text = rr.descrizione;

                    this.PopolaDdlUtenti();
                    this.ddl_user.Enabled = true;
                    if (!string.IsNullOrEmpty(registro.idUtenteResp))
                    {
                        this.ddl_user.SelectedValue = registro.idUtenteResp;
                    }
                }
                else
                {
                    this.codRuoloRespReg.Text = "";
                    this.ruoloRespReg.Text = "";
                }
                //Imposto i due campi descrizione dell'utente e del ruolo per l'interoperabilità senza mail
                lblDirittoResp.Text = "Diritto per Acquisizione";
                lblDirittoResp.ToolTip = "Settando questo diritto il Ruolo Responsabile del Registro eredita per acquisizione tutti i protocolli creati sul registro.";

                if (ws.getValueInteropNoMail())
                {
                    this.panel_InteropNoMail.Visible = true;
                    this.panel_RuoloResp.Visible = true;
                    lblTitle.Text = "Ruolo Responsabile";

                    if (registro.ID_RUOLO_AOO != "")
                    {
                        DocsPAWA.DocsPaWR.Ruolo rr = ws.getRuoloById(registro.ID_RUOLO_AOO);
                        if (rr != null)
                            registro.DESCR_RUOLO_AOO = rr.descrizione;

                        CaricaComboUtente(registro.ID_RUOLO_AOO);
                        if (registro.ID_PEOPLE_AOO != "")
                        {
                            ddl_UtInteropNoMail.ClearSelection();
                            if (this.ddl_UtInteropNoMail.Items.Count > 1)
                                this.ddl_UtInteropNoMail.SelectedIndex = this.ddl_UtInteropNoMail.Items.IndexOf(ddl_UtInteropNoMail.Items.FindByValue(registro.ID_PEOPLE_AOO));
                        }

                        registro.DESCR_PEOPLE_AOO = ddl_UtInteropNoMail.SelectedItem != null ? ddl_UtInteropNoMail.SelectedItem.Text : string.Empty;
                        this.ddl_DirittoResp.SelectedIndex = this.ddl_DirittoResp.Items.IndexOf(ddl_DirittoResp.Items.FindByValue(registro.Diritto_Ruolo_AOO));
                    }
                    Session.Remove("selRuoloRegNoMail");
                    Session.Remove("selUtenteRegNoMail");
                }
                else  // imposto il campo per il ruolo responsabile
                {
                    this.panel_InteropNoMail.Visible = false;
                    this.panel_RuoloResp.Visible = false;
                    lblTitle.Text = "Ruolo Responsabile Registro";

                    if (registro.ID_RUOLO_AOO != "")
                    {
                        DocsPAWA.DocsPaWR.Ruolo rr = ws.getRuoloById(registro.ID_RUOLO_AOO);
                        registro.DESCR_RUOLO_AOO = rr.descrizione;
                    }
                    this.ddl_DirittoResp.SelectedIndex = this.ddl_DirittoResp.Items.IndexOf(ddl_DirittoResp.Items.FindByValue(registro.Diritto_Ruolo_AOO));

                    Session.Remove("selRuoloRegNoMail");
                }

                lbl_ruoloRespReg.ToolTip = "Il responsabile del registro è il ruolo che eredita tutti i protocolli creati su quella AOO con il diritto specificato nella sezione Diritto di Acquisizione";

                insertUtRuoloRegistroInSession(registro.IDRegistro);

                this.BindUI(registro);
                // Impostazione id registro correntemente selezionato
                this.CurrentIDRegistro = registro.IDRegistro;
                //salvo le informazioni sulle caselle associate al Registro corrente
                this.SetCaselleRegistro(this.CurrentIDRegistro);
                if (Caselle != null && Caselle.Length > 0)
                {// aggiorno le note della casella di default
                    this.txt_note.Text = Caselle[0].Note;

                    //Aggiorno il messaggio
                    this.txt_msg_posta_in_uscita.Text = Caselle[0].MessageSendMail;
                    this.cbx_sovrascrivi_messaggio.Checked = Caselle[0].OverwriteMessageAmm;
                }

                if (System.Configuration.ConfigurationManager.AppSettings["PROTO_SEMPLIFICATO_ENABLED"] != null &&
                    System.Configuration.ConfigurationManager.AppSettings["PROTO_SEMPLIFICATO_ENABLED"].ToUpper() == "TRUE")
                {
                    this.btn_UOsmista.Visible = true;
                    this.btn_UOsmista.Attributes.Add("onclick", "apriUOSmistamento('" + registro.IDRegistro + "');");
                }
                else
                {
                    this.btn_UOsmista.Visible = false;
                }
                this.btn_test.Visible = true;
            }
        }

        private void CaricaComboUtente(string idRuolo)
        {
            DocsPAWA.DocsPaWR.Utente[] utenti = ws.GetListaUtentiByIdRuolo(idRuolo);
            if (utenti != null && utenti.Length > 0)
            {

                ddl_UtInteropNoMail.DataSource = utenti;
                ddl_UtInteropNoMail.DataTextField = "descrizione";
                ddl_UtInteropNoMail.DataValueField = "idPeople";
                ddl_UtInteropNoMail.DataBind();
                ListItem neutro = new ListItem("", "0");
                ddl_UtInteropNoMail.Items.Insert(0, neutro);
                ddl_UtInteropNoMail.SelectedIndex = 0;
                //registro.DESCR_PEOPLE_AOO = ddl_UtInteropNoMail.SelectedItem.Text;
            }

        }

        /// <summary>
        /// ID del registro correntemente selezionato
        /// </summary>
        private string CurrentIDRegistro
        {
            get
            {
                string retValue = this.ViewState["ID_REGISTRO"] as string;
                if ((retValue == null) && (retValue == ""))
                    retValue = string.Empty;
                return retValue;
            }
            set
            {
                this.ViewState["ID_REGISTRO"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dg_Registri_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            e.Item.Cells[GRID_COL_DELETE].Attributes.Add("onclick", "if (!window.confirm('Sei sicuro di voler eliminare questo registro?')) {return false};");
        }

        #endregion

        #region tasti gestione pannello INFO

        /// <summary>
        /// Tasto AGGIUNGI registro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_nuova_Click(object sender, System.EventArgs e)
        {
            // Predisposizione per l'inserimento di un nuovo registro
            this.SetInsertMode();
            ddl_caselle.Items.Clear();
            DeleteCaselleRegistro();
        }

        private void btn_aggiungi_Click(object sender, System.EventArgs e)
        {
            //se supero i controlli salvo le informazioni sul registro
            //Andrea De Marco - Aggiunti controlli per Import Pregressi
            if (CheckUpdParamCasella() && 
                (string.IsNullOrEmpty(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_IMPORT_PREGRESSI")) || !(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_IMPORT_PREGRESSI").Equals("1"))) ||
                ((!string.IsNullOrEmpty(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_IMPORT_PREGRESSI")) && DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_IMPORT_PREGRESSI").Equals("1") && ControlloPregressi())))
            {
                //Update del panel di pregresso
                Save();
                //Andrea De Marco - Refresh Panel Import Pregressi dopo Inserimento Registro
                this.pnl_pregresso.Update();
                //End Andrea De Marco
            }
        }

        #endregion

        #region pannelli

        private void btn_chiudiPnlInfo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            pnl_info.Visible = false;

            this.ClearData();

            dg_Registri.SelectedIndex = -1;
        }
        #endregion

        #region Gestione accesso ai dati

        /// <summary>
        /// Gestione caricamento lista registri
        /// </summary>
        private void FillListRegistri()
        {
            DocsPAWA.DocsPaWR.OrgRegistro[] registri = this.GetRegistriAmministrazione();
            DataSet ds = this.ConvertToDataSet(registri);

            this.dg_Registri.DataSource = ds;
            this.dg_Registri.DataBind();

            registri = null;
        }

        /// <summary>
        /// Conversione array
        /// </summary>
        /// <param name="registri"></param>
        /// <returns></returns>
        private DataSet ConvertToDataSet(DocsPAWA.DocsPaWR.OrgRegistro[] registri)
        {
            DataSet ds = this.CreateGridDataSet();
            DataTable dt = ds.Tables["Registri"];

            foreach (DocsPAWA.DocsPaWR.OrgRegistro registro in registri)
            {
                DataRow row = dt.NewRow();

                row[TABLE_COL_ID] = registro.IDRegistro;
                row[TABLE_COL_CODICE] = registro.Codice;
                row[TABLE_COL_DESCRIZIONE] = registro.Descrizione;
                row[TABLE_COL_EMAIL] = registro.Mail.Email;
                row[TABLE_COL_DISABILITATO] = registro.Sospeso.ToString();

                dt.Rows.Add(row);
            }

            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DataSet CreateGridDataSet()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Registri");

            dt.Columns.Add(new DataColumn(TABLE_COL_ID));
            dt.Columns.Add(new DataColumn(TABLE_COL_CODICE));
            dt.Columns.Add(new DataColumn(TABLE_COL_DESCRIZIONE));
            dt.Columns.Add(new DataColumn(TABLE_COL_EMAIL));
            dt.Columns.Add(new DataColumn(TABLE_COL_DISABILITATO));

            ds.Tables.Add(dt);
            return ds;
        }

        /// <summary>
        /// Associazione dati registro ai campi della UI
        /// </summary>
        /// <param name="registro"></param>
        private void BindUI(DocsPAWA.DocsPaWR.OrgRegistro registro)
        {
            this.ClearData();
            Session.Add("REG_SEL", registro);

            this.txt_codice.Text = registro.Codice;
            this.lbl_cod.Text = this.txt_codice.Text;
            this.txt_descrizione.Text = registro.Descrizione;
            //if (registro.Stato.Equals("S"))
            //    this.ChkSospeso.Checked = true;
            //else
            //    this.ChkSospeso.Checked = false;
            this.ChkSospeso.Checked = registro.Sospeso;
            //this.cbx_invioManuale.Checked = registro.invioRicevutaManuale;
            this.cbx_invioAuto.Checked = (registro.invioRicevutaManuale.Equals("0")) ? true : false;
            if (!registro.idRuoloResp.Equals(""))
            {
                this.codRuoloRespReg.Text = registro.idRuoloResp;
                DocsPAWA.DocsPaWR.Ruolo rr = ws.getRuoloById(registro.idRuoloResp);
                this.ruoloRespReg.Text = rr.descrizione;

                this.PopolaDdlUtenti();
                this.ddl_user.Enabled = true;
                if (!string.IsNullOrEmpty(registro.idUtenteResp))
                {
                    this.ddl_user.SelectedValue = registro.idUtenteResp;
                }

            }
            this.ddl_DirittoResp.SelectedIndex = this.ddl_DirittoResp.Items.IndexOf(ddl_DirittoResp.Items.FindByValue(registro.Diritto_Ruolo_AOO));


            //Interoperabilità no mail
            if (ws.getValueInteropNoMail())
            {
                lblTitle.Text = "Interoperabilita' tra AOO interne senza uso della posta elettronica";
                this.panel_InteropNoMail.Visible = true;
                this.panel_RuoloResp.Visible = true;
                //EM this.txt_UtInteropNoMail.Text = registro.DESCR_PEOPLE_AOO;
                ddl_UtInteropNoMail.ClearSelection();
                if (registro.DESCR_PEOPLE_AOO.ToString() != string.Empty)
                {
                    if (this.ddl_UtInteropNoMail.Items.Count > 1)
                        this.ddl_UtInteropNoMail.SelectedIndex = this.ddl_UtInteropNoMail.Items.IndexOf(ddl_UtInteropNoMail.Items.FindByText(registro.DESCR_PEOPLE_AOO));
                }

                this.txt_RuoloInteropNoMail.Text = registro.DESCR_RUOLO_AOO;
                this.txt_IdHiddenRuoloNomail.Text = registro.ID_RUOLO_AOO;
                this.txt_IdHiddenUtenteNomail.Text = registro.ID_PEOPLE_AOO;
                this.ddl_automatico.SelectedIndex = ddl_automatico.Items.IndexOf(ddl_automatico.Items.FindByValue(registro.autoInterop));
                this.lblDescAutoInterop.Visible = true;

                string testo = calcolaLabelCombo(registro.autoInterop);

                this.lblDescAutoInterop.Text = testo;
            }
            else
            {
                lblTitle.Text = "Ruolo Responsabile Registro";
                this.panel_InteropNoMail.Visible = false;
                this.panel_RuoloResp.Visible = false;
                this.txt_RuoloInteropNoMail.Text = registro.DESCR_RUOLO_AOO;
                this.txt_IdHiddenRuoloNomail.Text = registro.ID_RUOLO_AOO;
            }

            //this.txt_indirizzo.Text = registro.Mail.Email;
            this.txt_utente.Text = registro.Mail.UserID;
            this.txt_password.Text = registro.Mail.Password;
            this.txt_conferma_pwd.Text = registro.Mail.Password;
            this.txt_smtp.Text = registro.Mail.ServerSMTP;
            this.txt_pop.Text = registro.Mail.ServerPOP;
            this.ChkBoxPopSSl.Checked = (registro.Mail.POPssl == "1") ? true : false;
            this.ChkBoxsmtp.Checked = (registro.Mail.SMTPssl == "1") ? true : false;
            this.ChkBoxsmtpSTA.Checked = (registro.Mail.SMTPsslSTA == "1") ? true : false;
            this.Chk_sslImap.Checked = (registro.Mail.IMAPssl == "1") ? true : false;
            this.ChkMailPec.Checked = (registro.Mail.soloMailPec == "1") ? true : false;

            // Per gestione pendenti tramite PEC
            this.ChkMailRicevutePendenti.Checked = (registro.Mail.MailRicevutePendenti == "1") ? true : false;
            this.ChkMailRicevutePendenti.Enabled = !this.ChkMailPec.Checked;
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["PEC4_R1_TEMP"]) && ConfigurationManager.AppSettings["PEC4_R1_TEMP"].ToString() == "0")
            {
                this.ChkMailRicevutePendenti.Visible = false;
                this.ChkMailRicevutePendentiText.Visible = false;
            }

            if (String.IsNullOrEmpty(registro.Mail.tipoPosta)
                ||
              (string.IsNullOrEmpty(registro.Mail.ServerPOP) &&
               string.IsNullOrEmpty(registro.Mail.serverImap) &&
               string.IsNullOrEmpty(registro.Mail.UserID)))
            {
                ddl_posta.Items[0].Text = "";
                ddl_posta.Items[1].Text = "POP";
                ddl_posta.Items[2].Text = "IMAP";
                txt_password.Text = string.Empty;
                txt_conferma_pwd.Text = string.Empty;
            }
            else
                if (registro.Mail.tipoPosta.Equals("POP"))
                {
                    ddl_posta.Items[0].Text = "POP";
                    ddl_posta.Items[1].Text = "IMAP";
                    ddl_posta.Items[2].Text = "";

                    this.ddl_posta.SelectedIndex = ddl_posta.Items.IndexOf(ddl_posta.Items.FindByValue(registro.Mail.tipoPosta));
                    this.txt_pop.Text = registro.Mail.ServerPOP;
                    this.ChkBoxPopSSl.Checked = (registro.Mail.POPssl == "1") ? true : false;
                    if (registro.Mail.PortaPOP > 0)
                        this.txt_portapop.Text = registro.Mail.PortaPOP.ToString();

                    this.txt_imap.Text = registro.Mail.serverImap;
                    this.Chk_sslImap.Checked = (registro.Mail.IMAPssl == "1") ? true : false;
                    if (registro.Mail.portaIMAP > 0)
                        this.txt_portaImap.Text = registro.Mail.portaIMAP.ToString();
                    this.txt_inbox.Text = registro.Mail.inbox;
                    this.txt_mailElaborate.Text = registro.Mail.mailElaborate;
                    this.txt_mailNonElaborate.Text = registro.Mail.mailNonElaborate;
                    //abilitazioni componenti                
                    this.txt_imap.Enabled = !true;
                    this.Chk_sslImap.Enabled = !true;
                    this.txt_portaImap.Enabled = !true;
                    this.txt_inbox.Enabled = !true;
                    this.txt_mailElaborate.Enabled = !true;
                    this.txt_mailNonElaborate.Enabled = !true;
                    this.txt_pop.Enabled = true;
                    this.ChkBoxPopSSl.Enabled = true;
                    this.txt_portapop.Enabled = true;

                }
                else
                    if (registro.Mail.tipoPosta.Equals("IMAP"))
                    {
                        ddl_posta.Items[0].Text = "IMAP";
                        ddl_posta.Items[1].Text = "POP";
                        ddl_posta.Items[2].Text = "";

                        this.ddl_posta.SelectedIndex = ddl_posta.Items.IndexOf(ddl_posta.Items.FindByValue(registro.Mail.tipoPosta));
                        this.txt_imap.Text = registro.Mail.serverImap;
                        this.Chk_sslImap.Checked = (registro.Mail.IMAPssl == "1") ? true : false;
                        if (registro.Mail.portaIMAP > 0)
                            this.txt_portaImap.Text = registro.Mail.portaIMAP.ToString();
                        this.txt_inbox.Text = registro.Mail.inbox;
                        this.txt_mailElaborate.Text = registro.Mail.mailElaborate;
                        this.txt_mailNonElaborate.Text = registro.Mail.mailNonElaborate;
                        this.txt_pop.Text = registro.Mail.ServerPOP;
                        this.ChkBoxPopSSl.Checked = (registro.Mail.POPssl == "1") ? true : false;
                        if (registro.Mail.PortaPOP > 0)
                            this.txt_portapop.Text = registro.Mail.PortaPOP.ToString();
                        //abilitazioni componenti
                        this.txt_imap.Enabled = true;
                        this.Chk_sslImap.Enabled = true;
                        this.txt_portaImap.Enabled = true;
                        this.txt_inbox.Enabled = true;
                        this.txt_mailElaborate.Enabled = true;
                        this.txt_mailNonElaborate.Enabled = true;
                        this.txt_pop.Enabled = !true;
                        this.ChkBoxPopSSl.Enabled = !true;
                        this.txt_portapop.Enabled = !true;



                    }
            this.ChkMailPec.Checked = (registro.Mail.soloMailPec == "1") ? true : false;

            // Per gestione pendenti tramite PEC
            this.ChkMailRicevutePendenti.Checked = (registro.Mail.MailRicevutePendenti == "1") ? true : false;
            this.ChkMailRicevutePendenti.Enabled = !this.ChkMailPec.Checked;

            if (registro.Mail.PortaSMTP > 0)
                this.txt_portasmtp.Text = registro.Mail.PortaSMTP.ToString();

            if (registro.Mail.PortaPOP > 0)
                this.txt_portapop.Text = registro.Mail.PortaPOP.ToString();

            this.txt_userid_smtp.Text = registro.Mail.UserSMTP;
            this.txt_pwd_smtp.Text = registro.Mail.PasswordSMTP;
            this.txt_conferma_pwd_smtp.Text = registro.Mail.PasswordSMTP;
            this.ChkAutomatico.Checked = registro.AperturaAutomatica;

            // gestione stampa ricevute
            this.gestioneStampaRicevute(registro);
            if (registro.Mail.pecTipoRicevuta != string.Empty)
            {
                string tipoPec = registro.Mail.pecTipoRicevuta.Substring(0, 1);
                this.ddl_ricevutaPec.SelectedIndex = ddl_ricevutaPec.Items.IndexOf(ddl_ricevutaPec.Items.FindByValue(tipoPec));

            }

            //Se l'utente è super-amministratore abilito il pulsante sblocco casella
            DocsPAWA.DocsPaWR.InfoUtenteAmministratore _datiAmministratore = new DocsPAWA.DocsPaWR.InfoUtenteAmministratore();
            DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
            _datiAmministratore = session.getUserAmmSession();
            if (_datiAmministratore.tipoAmministratore.Equals("1"))
                this.btn_SbloccaCasella.Visible = true;
        }

        /// <summary>
        /// Aggiornamento registro dai dati dei campi della UI
        /// </summary>
        private void RefreshRegistroFromUI(ref DocsPAWA.DocsPaWR.OrgRegistro registro)
        {

            registro = (DocsPAWA.DocsPaWR.OrgRegistro)Session["REG_SEL"];
            if (registro == null)
                registro = new DocsPAWA.DocsPaWR.OrgRegistro();
            registro.IDRegistro = this.CurrentIDRegistro;
            // Assegnazione valori campi UI
            registro = this.GetRegistro(this.CurrentIDRegistro);
            registro.Codice = this.txt_codice.Text.Trim();
            registro.Descrizione = this.txt_descrizione.Text.Trim();
            string codiceAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");
            registro.CodiceAmministrazione = codiceAmministrazione;
            registro.idRuoloResp = this.codRuoloRespReg.Text;
            registro.invioRicevutaManuale = (this.cbx_invioAuto.Checked == true) ? "0" : "1";
            //fine modifica
            registro.AperturaAutomatica = this.ChkAutomatico.Checked;
            registro.Sospeso = this.ChkSospeso.Checked;
            //Interoperabilità no mail
            if (ws.getValueInteropNoMail())
            {
                registro.ID_PEOPLE_AOO = this.txt_IdHiddenUtenteNomail.Text;
                registro.autoInterop = this.ddl_automatico.SelectedItem.Value;
            }

            registro.ID_RUOLO_AOO = this.txt_IdHiddenRuoloNomail.Text;
            registro.Diritto_Ruolo_AOO = this.ddl_DirittoResp.SelectedItem.Value;

            registro.idUtenteResp = this.ddl_user.SelectedValue;

        }


        private void RefreshRegistroFromUI_Save(ref DocsPAWA.DocsPaWR.OrgRegistro registro)
        {
            registro.IDRegistro = this.CurrentIDRegistro;
            registro.Codice = this.txt_codice.Text.Trim();
            registro.Descrizione = this.txt_descrizione.Text.Trim();

            string codiceAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");
            registro.CodiceAmministrazione = codiceAmministrazione;
            registro.idRuoloResp = this.codRuoloRespReg.Text;
            registro.invioRicevutaManuale = (this.cbx_invioAuto.Checked == true) ? "0" : "1";
            registro.AperturaAutomatica = this.ChkAutomatico.Checked;
            registro.Sospeso = this.ChkSospeso.Checked;
            registro.chaRF = "0";
            //Interoperabilità no mail
            if (ws.getValueInteropNoMail())
            {
                registro.ID_PEOPLE_AOO = this.txt_IdHiddenUtenteNomail.Text;
                registro.autoInterop = this.ddl_automatico.SelectedItem.Value;
            }
            registro.ID_RUOLO_AOO = this.txt_IdHiddenRuoloNomail.Text;
            registro.Diritto_Ruolo_AOO = this.ddl_DirittoResp.SelectedItem.Value;
            if (registro.Mail == null)
                registro.Mail = new DocsPAWA.DocsPaWR.MailRegistro();
        }

        /// <summary>
        /// Reperimento registri
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.OrgRegistro[] GetRegistriAmministrazione()
        {
            string codiceAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");
            return this.GetRegistriAmministrazione(codiceAmministrazione, "0");
        }

        /// <summary>
        /// Reperimento registri
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.OrgRegistro[] GetRegistriAmministrazione(string codiceAmministrazione, string chaRF)
        {
            return ws.AmmGetRegistri(codiceAmministrazione, chaRF);
        }

        /// <summary>
        /// Reperimento registro
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.OrgRegistro GetRegistro(string idRegistro)
        {
            return ws.AmmGetRegistro(idRegistro);
        }

        /// <summary>
        /// Salvataggio dati
        /// </summary>
        /// <returns></returns>
        private void Save()
        {
            DocsPAWA.DocsPaWR.OrgRegistro registro = new DocsPAWA.DocsPaWR.OrgRegistro();
            bool insertMode = this.OnInsertMode();

            if (insertMode)
            {
                this.RefreshRegistroFromUI_Save(ref registro);

                //Andrea De Marco - Insert - Controllo che il flag chk_pregresso sia flaggato e il campo anno sia != null o vuoto
                if (ControlloPregressi() && this.chk_pregresso.Checked)
                {
                    ////Disabilito gli elementi che non rientrano nelle funzionalità import pregressi
                    //this.ChkAutomatico.Checked = false;
                    //this.ChkAutomatico.Enabled = false;
                    //this.ChkSospeso.Checked = false;
                    //this.ChkSospeso.Enabled = false;
                    //this.pnl_automatico_sospeso.Update();

                    registro.flag_pregresso = true;
                    registro.anno_pregresso = txt_anno_reg_pre.Text;

                }
                else
                {
                    registro.flag_pregresso = false;
                    registro.anno_pregresso = string.Empty;
                }
                //End Andrea De Marco
            }
            else
            {
                this.RefreshRegistroFromUI(ref registro);

                //Andrea De Marco - Update - Controllo che il flag chk_pregresso sia flaggato e il campo anno sia != null o vuoto
                if (ControlloPregressi() && this.chk_pregresso.Checked)
                {
                    ////Disabilito gli elementi che non rientrano nelle funzionalità import pregressi
                    //this.ChkAutomatico.Checked = false;
                    //this.ChkAutomatico.Enabled = false;
                    //this.ChkSospeso.Checked = false;
                    //this.ChkSospeso.Enabled = false;
                    //this.pnl_automatico_sospeso.Update();
                    
                    ////Quando seleziono un registro di pregresso i campi relativi al ruolo responsabile devono essere disabilitati.
                    //this.ruoloRespReg.Enabled = false;
                    //this.btn_RubricaRuoloResp.Enabled = false;
                    //this.img_delRuoloResp.Enabled = false;
                    //this.ddl_DirittoResp.Enabled = false;
                    //this.pnlRuoloResponsabile.Update();

                    registro.flag_pregresso = true;
                    registro.anno_pregresso = txt_anno_reg_pre.Text;
                }
                else
                {
                    registro.flag_pregresso = false;
                    registro.anno_pregresso = string.Empty;
                }
                //End Andrea De Marco
            }    
            if (ddl_caselle.SelectedItem != null)
                this.RefreshCasellaFromUI(ddl_caselle.SelectedItem.Value);
            this.SaveMailPrincInRegistro(ref registro);

            DocsPAWA.DocsPaWR.ValidationResultInfo result = null;

            if (insertMode)
            {
                result = this.InsertRegistro(ref registro);
                if (result.Value && ((Caselle != null && Caselle.Length > 0) || ws.IsEnabledInteropInterna()))
                result = DocsPAWA.utils.MultiCasellaManager.InsertMailRegistro(registro.IDRegistro, Caselle, ws.IsEnabledInteropInterna());
                if (result.Value)
                {
                    txt_indirizzo.Enabled = true;
                    img_aggiungiCasella.Enabled = true;
                    ddl_caselle.Enabled = true;
                }
            }
            else
            {
                Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                if (registro.Sospeso && !ws.AmmPredispostiInRegistro(ref registro))
                {
                    RegisterClientScript("alertPredInReg", "alert('Ci sono documenti predisposti alla protocollazione nel registro che si intende sospendere!');");
                    return;
                }
                else if(registro.Sospeso && theManager.ExistsPassiFirmaByIdRegistroAndEmailRegistro(registro.IDRegistro, string.Empty))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "AvvisoPresenzaProcessiFirmaRegistro", "<script>AvvisoPresenzaProcessiFirmaRegistro();</script>", false);
                    return;
                }
                else
                {
                    result = this.UpdateRegistro(ref registro);
                    if (result.Value && Caselle != null && Caselle.Length > 0)
                        result = DocsPAWA.utils.MultiCasellaManager.UpdateMailRegistro(registro.IDRegistro, Caselle);
                }
            }

            // Se il salvataggio è anadato a buon fine, vengono salvate le impostazioni relative all'IS
            if (result.Value && (!insertMode))
            {
                try
                {
                    this.isRegistrySettings.SaveSettings();
                }
                catch (Exception e)
                {
                    result.Value = false;
                    result.BrokenRules = new BrokenRule[] 
                        {
                            new BrokenRule() 
                            { 
                                ID = "CONTROLLO_IS",
                                Level = BrokenRuleLevelEnum.Error, 
                                Description = e.Message
                            }
                        };
                }
            }

            if (!result.Value)
            {
                this.ShowValidationMessage(result);
            }
            else if (!insertMode)
            {
                // Aggiornamento
                pnl_info.Visible = false;

                this.ClearData();

                // Aggiornamento elemento griglia corrente
                this.RefreshGridItem(registro);

                dg_Registri.SelectedIndex = -1;
            }
            else
            {
                // Inserimento

                // Refresh lista registri
                this.FillListRegistri();
                // Predisposizione per un nuovo inserimento
                this.SetInsertMode();
                ddl_caselle.Items.Clear();
                this.ClearParametri();
            }

        }

        private void UpdateRegistro(DocsPAWA.DocsPaWR.OrgRegistro registro)
        {
            DocsPAWA.DocsPaWR.ValidationResultInfo result = this.UpdateRegistro(ref registro);
            if (result.Value && Caselle != null && Caselle.Length > 0)
                result = DocsPAWA.utils.MultiCasellaManager.UpdateMailRegistro(registro.IDRegistro, Caselle);

            if (result.Value)
            {
                try
                {
                    this.isRegistrySettings.SaveSettings();
                }
                catch (Exception e)
                {
                    result.Value = false;
                    result.BrokenRules = new BrokenRule[]
                        {
                            new BrokenRule()
                            {
                                ID = "CONTROLLO_IS",
                                Level = BrokenRuleLevelEnum.Error,
                                Description = e.Message
                            }
                        };
                }
            }

            if (!result.Value)
            {
                this.ShowValidationMessage(result);
            }
            else 
            {
                // Aggiornamento
                pnl_info.Visible = false;

                this.ClearData();

                // Aggiornamento elemento griglia corrente
                this.RefreshGridItem(registro);

                dg_Registri.SelectedIndex = -1;
            }
        
        }

        /// <summary>
        /// Rimozione dati controlli UI
        /// </summary>
        private void ClearData()
        {
            this.CurrentIDRegistro = string.Empty;

            txt_codice.Text = string.Empty;
            lbl_cod.Text = string.Empty;
            txt_descrizione.Text = string.Empty;
            txt_indirizzo.Text = string.Empty;
            txt_utente.Text = string.Empty;
            txt_password.Text = string.Empty;
            txt_smtp.Text = string.Empty;
            txt_userid_smtp.Text = string.Empty;
            txt_pwd_smtp.Text = string.Empty;
            //pop
            txt_pop.Text = string.Empty;
            txt_portasmtp.Text = string.Empty;
            txt_portapop.Text = string.Empty;
            //imap
            txt_imap.Text = string.Empty;
            txt_mailNonElaborate.Text = string.Empty;
            txt_mailElaborate.Text = string.Empty;
            txt_portaImap.Text = string.Empty;
            txt_inbox.Text = string.Empty;
            txt_note.Text = string.Empty;
            ruoloRespReg.Text = string.Empty;
            codRuoloRespReg.Text = string.Empty;

            ddl_posta.Items[0].Text = "";
            ddl_posta.Items[1].Text = "POP";
            ddl_posta.Items[2].Text = "IMAP";

            Chk_sslImap.Checked = false;
            ChkBoxPopSSl.Checked = false;
            ChkMailPec.Checked = false;
            // Per gestione pendenti tramite PEC
            ChkMailRicevutePendenti.Checked = false;
            ChkMailRicevutePendenti.Enabled = true;
            this.txt_msg_posta_in_uscita.Text = string.Empty;
            this.cbx_sovrascrivi_messaggio.Checked = false;

            this.ddl_user.Items.Clear();



            Session.Remove("REG_SEL");
        }

        /// <summary>
        /// Predisposizione per l'inserimento di un nuovo registro
        /// </summary>
        private void SetInsertMode()
        {
            //visibilità informazioni
            pnl_info.Visible = true;
            btn_aggiungi.Text = "Aggiungi";
            this.btn_UOsmista.Visible = false;
            txt_codice.Visible = true;
            lbl_cod.Visible = false;

            // Rimozione dati controlli UI
            this.ClearData();

            ChkAutomatico.Checked = false;
            SetFocus(txt_codice);

            dg_Registri.SelectedIndex = -1;

            //Interoperablità no mail
            this.txt_IdHiddenRuoloNomail.Text = "";
            this.txt_IdHiddenUtenteNomail.Text = "";

            this.txt_RuoloInteropNoMail.Text = "";
            lblTitle.Text = "Interoperabilita' tra AOO interne senza uso della posta elettronica";
            lblDirittoResp.ToolTip = "Settando questo diritto il Ruolo Responsabile del Registro eredita per acquisizione tutti i protocolli creati sul registro.";
            lbl_ruoloRespReg.ToolTip = "Il responsabile del registro è il ruolo che eredita tutti i protocolli creati su quella AOO con il diritto specificato nella sezione Diritto di Acquisizione";
            if (!ws.getValueInteropNoMail())
            {
                this.panel_InteropNoMail.Visible = false;
                this.panel_RuoloResp.Visible = false;


                this.lbl_ruoloRespReg.Visible = false;
                this.ruoloRespReg.Visible = false;
                this.btn_RubricaRuoloResp.Visible = false;
                this.img_delRuoloResp.Visible = false;
                this.ddl_DirittoResp.Visible = false;
                this.lblDirittoResp.Visible = false;
            }
            else
            {
                this.panel_InteropNoMail.Visible = true;
                this.panel_RuoloResp.Visible = true;
            }
            this.ddl_UtInteropNoMail.Items.Clear();
            //this.ddl_UtInteropNoMail.ClearSelection();
            //}

            // gestione stampa ricevute
            this.VisualizzaGestStampaRicevuta(false);
            this.ddl_posta.Items[0].Text = "";
            this.ddl_posta.Items[1].Text = "POP";
            this.ddl_posta.Items[2].Text = "IMAP";

            //Andrea - Ripristino dei campi legati ad import pregressi
            this.chk_pregresso.Checked = false;
            this.txt_anno_reg_pre.ReadOnly = true;
            this.txt_anno_reg_pre.Text = string.Empty;
            this.ChkAutomatico.Checked = false;
            this.ChkAutomatico.Enabled = true;
            this.ChkSospeso.Checked = false;
            this.ChkSospeso.Enabled = true;
            //End Andrea
        }

        /// <summary>
        /// Aggiornamento elemento griglia corrente
        /// </summary>
        /// <param name="registro"></param>
        private void RefreshGridItem(DocsPAWA.DocsPaWR.OrgRegistro registro)
        {
            DataGridItem item = this.dg_Registri.SelectedItem;

            if (item != null)
            {
                item.Cells[GRID_COL_DESCRIZIONE].Text = registro.Descrizione;
                item.Cells[GRID_COL_EMAIL].Text = registro.Mail.Email;
                item.Cells[GRID_COL_DISABILITATO].Text = registro.Sospeso.ToString();
                if (registro.Sospeso.ToString().ToUpper().Equals("TRUE"))
                    item.ForeColor = Color.Red;
                else
                    item.ForeColor = Color.Gray;
            }
        }

        private string CommandPending
        {
            get
            {
                return this.txtCommandPending.Value;
            }
            set
            {
                this.txtCommandPending.Value = value;
            }
        }

        /// <summary>
        /// Cancellazione registro
        /// </summary>
        private void Delete(bool forceDelete)
        {
            DocsPAWA.DocsPaWR.OrgRegistro registro = new DocsPAWA.DocsPaWR.OrgRegistro();
            this.RefreshRegistroFromUI(ref registro);

            DocsPAWA.DocsPaWR.ValidationResultInfo result = null;

            bool canDelete = true;

            if (!forceDelete)
            {
                // Verifica se il registro può essere cancellato in 
                // base alle regole di business
                result = this.CanDeleteRegistro(registro);
                canDelete = (result.BrokenRules.Length == 0);
            }

            if (canDelete)
            {
                //this.txtCommandPending.Value="DELETE";

                result = this.DeleteRegistro(ref registro);
                if (result.Value)
                    result = DocsPAWA.utils.MultiCasellaManager.DeleteMailRegistro(registro.IDRegistro, string.Empty);
                if (!result.Value)
                {
                    this.ShowValidationMessage(result);
                }
                else
                {
                    //this.txtCommandPending.Value=string.Empty;

                    this.FillListRegistri();

                    pnl_info.Visible = false;

                    this.ClearData();
                    this.ddl_caselle.Items.Clear();
                    this.ClearParametri();
                    dg_Registri.SelectedIndex = -1;
                }
            }
            else
            {
                this.ShowValidationMessage(result);
            }
        }

        /// <summary>
        /// Handler cancellazione elemento griglia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteItemCommand(object sender, DataGridCommandEventArgs e)
        {
            this.dg_Registri.SelectedIndex = e.Item.ItemIndex;
            string idRegistro = this.dg_Registri.SelectedItem.Cells[GRID_COL_ID].Text;
            this.CurrentIDRegistro = idRegistro;

            this.Delete(false);
        }

        /// <summary>
        /// Hashtable businessrules
        /// </summary>
        private Hashtable _businessRuleControls = null;

        /// <summary>
        /// Inizializzazione hashtable per le businessrule:
        /// - Key:		ID della regola di business
        /// - Value:	Controllo della UI contenente il 
        ///				dato in conflitto con la regola di business
        /// </summary>
        private void InitializeBusinessRuleControls()
        {
            this._businessRuleControls = new Hashtable();

            this._businessRuleControls.Add("CODICE_REGISTRO", this.txt_codice);
            this._businessRuleControls.Add("DESCRIZIONE_REGISTRO", this.txt_descrizione);
            this._businessRuleControls.Add("MAIL_REGISTRO", this.txt_indirizzo);
            this._businessRuleControls.Add("CONTROLLO_IS", null);
        }

        private Control GetBusinessRuleControl(string idBusinessRule)
        {
            return this._businessRuleControls[idBusinessRule] as Control;
        }

        /// <summary>
        /// Visualizzazione messaggi di validazione
        /// </summary>
        /// <param name="validationResult"></param>
        private void ShowValidationMessage(DocsPAWA.DocsPaWR.ValidationResultInfo validationResult)
        {
            // Visualizzazione delle regole di business non valide
            bool warningMessage;
            Control firstInvalidControl;
            string validationMessage = this.GetValidationMessage(validationResult, out firstInvalidControl, out warningMessage);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ShowValidationMessage", "ShowValidationMessage('" + validationMessage + "'," + warningMessage.ToString().ToLower() + ");", true);
            if (firstInvalidControl != null)
                this.SetFocus(firstInvalidControl);
        }

        /// <summary>
        /// Inserimento di un nuovo registro
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.ValidationResultInfo InsertRegistro(ref DocsPAWA.DocsPaWR.OrgRegistro registro)
        {
            DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
            datiAmministratore = session.getUserAmmSession();
            
            return ws.AmmInsertRegistro(ref registro, datiAmministratore);
        }

        /// <summary>
        /// Aggiornamento registro
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.ValidationResultInfo UpdateRegistro(ref DocsPAWA.DocsPaWR.OrgRegistro registro)
        {
            return ws.AmmUpdateRegistro(ref registro);
        }

        /// <summary>
        /// Cancellazione registro
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.ValidationResultInfo DeleteRegistro(ref DocsPAWA.DocsPaWR.OrgRegistro registro)
        {
            return ws.AmmDeleteRegistro(ref registro);
        }

        /// <summary>
        /// Verifica se un registro può essere cancellato
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.ValidationResultInfo CanDeleteRegistro(DocsPAWA.DocsPaWR.OrgRegistro registro)
        {
            return ws.AmmCanDeleteRegistro(registro);
        }

        private string GetValidationMessage(DocsPAWA.DocsPaWR.ValidationResultInfo validationResult,
                                            out Control firstInvalidControl,
                                            out bool warningMessage)
        {
            string retValue = string.Empty;
            bool errorMessage = false;
            firstInvalidControl = null;

            foreach (DocsPAWA.DocsPaWR.BrokenRule rule in validationResult.BrokenRules)
            {
                if (!errorMessage && rule.Level == DocsPAWA.DocsPaWR.BrokenRuleLevelEnum.Error)
                    errorMessage = true;

                if (retValue != string.Empty)
                    retValue += "\\n";

                retValue += " - " + rule.Description;

                if (firstInvalidControl == null)
                    firstInvalidControl = this.GetBusinessRuleControl(rule.ID);
            }

            if (errorMessage)
                retValue = "Sono state riscontrate le seguenti anomalie:\\n\\n" + retValue;
            else
                retValue = "Attenzione:\\n\\n" + retValue;

            warningMessage = !errorMessage;

            return retValue.Replace("'", "\\'");
        }

        /// <summary>
        /// Verifica se si è in fase di inserimento registro
        /// </summary>
        /// <returns></returns>
        private bool OnInsertMode()
        {
            return (btn_aggiungi.Text.Equals("Aggiungi"));
        }


        #endregion

        #region Gestione javascript

        private void AddControlsClientAttribute()
        {
            this.txt_portapop.Attributes.Add("onkeypress", "ValidateNumericKey();");
            this.txt_portasmtp.Attributes.Add("onkeypress", "ValidateNumericKey();");
        }

        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), scriptKey, scriptString, false);
            }
        }

        private void AlertJS(string scriptKey, string msg)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>alert('" + msg.Replace("'", "\\'") + "');</SCRIPT>";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), scriptKey, scriptString, false);
            }
        }
        #endregion

        protected void brn_RubricaRuolo_Click(object sender, ImageClickEventArgs e)
        {
            RegisterClientScript("apriRubrica", "_ApriRubricaRuolo();");
        }

        protected void btn_RubricaRuoloResp_Click(object sender, ImageClickEventArgs e)
        {
            RegisterClientScript("apriRubricaResp", "_ApriRubricaRuoloResp();");
        }

        protected void insertUtRuoloRegistroInSession(string idRegistro)
        {
            if (Session["AMMDATASET"] == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "NoModTrasm", "<script>alert('Attenzione selezionare un\\'amministrazione !'); document.location = '../Gestione_Homepage/Home.aspx';</script>", false);
                return;
            }
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            string idAmministrazione = ws.getIdAmmByCod(codiceAmministrazione);

            DocsPAWA.DocsPaWR.Utente ut = new DocsPAWA.DocsPaWR.Utente();
            ut.codiceAmm = codiceAmministrazione;
            ut.idAmministrazione = idAmministrazione;
            ut.tipoIE = "I";
            ut.idRegistro = idRegistro;
            Session.Add("userData", ut);

            DocsPAWA.DocsPaWR.Ruolo rl = new DocsPAWA.DocsPaWR.Ruolo();
            rl.codiceAmm = codiceAmministrazione;
            rl.idAmministrazione = idAmministrazione;
            rl.tipoIE = "I";
            rl.idRegistro = idRegistro;
            rl.systemId = idAmministrazione;
            rl.uo = new DocsPAWA.DocsPaWR.UnitaOrganizzativa();
            rl.uo.codiceRubrica = codiceAmministrazione;
            Session.Add("userRuolo", rl);

            DocsPAWA.DocsPaWR.Registro reg = new DocsPAWA.DocsPaWR.Registro();
            reg = ws.GetRegistroBySistemId(idRegistro);
            Session.Add("userRegistro", reg);
        }

        protected void btn_RubricaUtente_Click(object sender, ImageClickEventArgs e)
        {
            RegisterClientScript("apriRubrica", "_ApriRubricaUtente();");
        }

        private void addRuoloInteropNoMail(DocsPAWA.DocsPaWR.ElementoRubrica[] selMittDaRubrica)
        {
            DocsPAWA.DocsPaWR.ElementoRubrica el = ((DocsPAWA.DocsPaWR.ElementoRubrica[])Session["selRuoloRegNoMail"])[0];
            DocsPAWA.DocsPaWR.Corrispondente corr = DocsPAWA.UserManager.getCorrispondenteByCodRubricaIE(this, el.codice, el.interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);

            this.txt_IdHiddenRuoloNomail.Text = corr.systemId;
            this.txt_RuoloInteropNoMail.Text = corr.descrizione;

            CaricaComboUtente(corr.systemId);
            Session.Remove("selRuoloRegNoMail");
        }

        private void addRuoloRespReg(DocsPAWA.DocsPaWR.ElementoRubrica[] selMittDaRubrica)
        {
            DocsPAWA.DocsPaWR.ElementoRubrica el = ((DocsPAWA.DocsPaWR.ElementoRubrica[])Session["selRuoloRespReg"])[0];
            DocsPAWA.DocsPaWR.Corrispondente corr = DocsPAWA.UserManager.getCorrispondenteByCodRubricaIE(this, el.codice, el.interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);

            this.codRuoloRespReg.Text = corr.systemId;
            this.ruoloRespReg.Text = corr.descrizione;

            // Popolamento ddl_user
            this.PopolaDdlUtenti();           

            Session.Remove("selRuoloRespReg");
        }
        #region commentato
        //private void addUtenteInteropNoMail(DocsPAWA.DocsPaWR.ElementoRubrica[] selMittDaRubrica)
        //{
        //    DocsPAWA.DocsPaWR.ElementoRubrica el = ((DocsPAWA.DocsPaWR.ElementoRubrica[]) Session["selUtenteRegNoMail"])[0];
        //    DocsPAWA.DocsPaWR.Corrispondente corr = DocsPAWA.UserManager.getCorrispondenteByCodRubricaIE(this, el.codice, el.interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);
        //    // EM this.txt_UtInteropNoMail.Text = corr.descrizione;


        //    DocsPAWA.DocsPaWR.Utente ut = (DocsPAWA.DocsPaWR.Utente) DocsPAWA.UserManager.getCorrispondenteByCodRubricaIE(this, el.codice, el.interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);
        //    ddl_UtInteropNoMail.ClearSelection();

        //    if (this.ddl_UtInteropNoMail.Items.Count > 1)
        //       this.ddl_UtInteropNoMail.SelectedIndex = this.ddl_UtInteropNoMail.Items.IndexOf(ddl_UtInteropNoMail.Items.FindByValue(ut.idPeople));
        //    //   this.ddl_amministrazioni.SelectedIndex = this.ddl_amministrazioni.Items.IndexOf(this.ddl_amministrazioni.Items.FindByValue(idAmmSelected));
        //    //ddl_UtInteropNoMail.Items.FindByValue(ut.idPeople).Selected = true;
        //    this.txt_IdHiddenUtenteNomail.Text = ut.idPeople;            
        //}

        //protected void img_delUtenteNoMail_Click(object sender, ImageClickEventArgs e)
        //{
        //    Session.Remove("selUtenteRegNoMail");
        //    this.txt_IdHiddenUtenteNomail.Text = "";
        //    this.ddl_UtInteropNoMail.SelectedIndex = 0;
        //    //this.txt_UtInteropNoMail.Text = "";
        //}
        #endregion
        protected void img_delRuoloNoMail_Click(object sender, ImageClickEventArgs e)
        {
            Session.Remove("selRuoloRegNoMail");
            this.txt_IdHiddenRuoloNomail.Text = "";

            this.txt_RuoloInteropNoMail.Text = "";
            this.ddl_UtInteropNoMail.Items.Clear();

        }

        protected void img_delRuoloResp_Click(object sender, ImageClickEventArgs e)
        {
            //Session.Remove("selRuoloRegNoMail");
            this.codRuoloRespReg.Text = "";

            this.ruoloRespReg.Text = "";
            //this.ddl_UtInteropNoMail.Items.Clear();
        }

        protected void ddl_automatico_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string testo = calcolaLabelCombo(ddl_automatico.SelectedValue);

            this.lblDescAutoInterop.Text = testo;
        }

        protected void ddl_UtInteropNoMail_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            txt_IdHiddenUtenteNomail.Text = ddl_UtInteropNoMail.SelectedItem.Value;
        }

        public string calcolaLabelCombo(string value)
        {
            string retString = "";

            switch (value)
            {
                case "0":
                    retString = "Protocollazione e trasmissione ai destinatari manuale";
                    break;
                case "1":
                    retString = "Protocollazione manuale e trasmissione automatica ai destinatari";
                    break;
                case "2":
                    retString = "Protocollazione e trasmissione ai destinatari automatica";
                    break;
                default:
                    retString = "";
                    break;

            }

            return retString;
        }

        #region Stampa ricevute

        private bool isEnabledStampaRicevute()
        {
            return true; // al momento è sempre usato...
        }

        private void VisualizzaGestStampaRicevuta(bool visibile)
        {
            this.pnl_ricevute.Visible = visibile;
        }

        private void UploadStampaRicevute()
        {

            try
            {
                if (this.fileupload_ricevuta.HasFile)
                {
                    string fileName = Server.HtmlEncode(fileupload_ricevuta.FileName);
                    string extension = System.IO.Path.GetExtension(fileName);
                    bool validFormat = false;
                    string validExt = "";
                    if (ricevutaPdf)
                    {
                        validExt = ".pdf";
                    }
                    else
                    {
                        validExt = ".rtf";
                    }
                    if (extension == validExt)
                    {
                        validFormat = true;
                    }
                    if (validFormat)
                    {
                        HttpPostedFile p = fileupload_ricevuta.PostedFile;
                        Stream fs = p.InputStream;
                        byte[] dati = new byte[fs.Length];
                        fs.Read(dati, 0, (int)fs.Length);
                        fs.Close();

                        DocsPAWA.DocsPaWR.OrgRegistro objRegistro = (DocsPAWA.DocsPaWR.OrgRegistro)Session["REG_SEL"];
                        if (objRegistro != null)
                        {
                            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

                            DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                            bool retValue = false;

                            bool modelPdf = (extension == ".pdf");


                            retValue = ws.SaveModelloStampaRicevuta(sessionManager.getUserAmmSession(), objRegistro, dati, modelPdf);

                            if (!retValue)
                                this.AlertJS("error1", "Si è verificato un errore nel salvataggio del modello di ricevuta");
                            else
                                this.impostaGUIRicevuta(true);
                        }
                    }
                    else
                    {
                        this.AlertJS("warning1", "Selezionare solo file " + validExt.ToUpper());
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("errore - UploadStampaRicevute: " + e.Message);
                this.AlertJS("error2", "Si è verificato un errore nel salvataggio del modello di ricevuta");
            }
        }

        private void EliminaStampaRicevute()
        {
            try
            {
                DocsPAWA.DocsPaWR.OrgRegistro objRegistro = (DocsPAWA.DocsPaWR.OrgRegistro)Session["REG_SEL"];
                if (objRegistro != null)
                {
                    DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                    bool retValue = ws.DeleteModelloStampaRicevuta(sessionManager.getUserAmmSession(), objRegistro);
                    if (!retValue)
                        this.AlertJS("error3", "Si è verificato un errore nella eliminazione del modello di ricevuta");
                    else
                        this.impostaGUIRicevuta(false);
                }
            }
            catch (Exception e)
            {
                logger.Debug("errore - EliminaStampaRicevute: " + e.Message);
                this.AlertJS("error4", "Si è verificato un errore nella eliminazione del modello di ricevuta");
            }
        }

        protected void btn_upload_ricevuta_Click(object sender, System.EventArgs e)
        {
            this.UploadStampaRicevute();
        }

        protected void btn_elimina_ricevuta_Click(object sender, System.EventArgs e)
        {
            this.EliminaStampaRicevute();
        }

        private void gestioneStampaRicevute(DocsPAWA.DocsPaWR.OrgRegistro registro)
        {
            this.VisualizzaGestStampaRicevuta(this.isEnabledStampaRicevute());
            if (this.isEnabledStampaRicevute())
                this.impostaGUIRicevuta(this.esisteModelloRicevuta(registro));
        }

        private bool esisteModelloRicevuta(DocsPAWA.DocsPaWR.OrgRegistro registro)
        {
            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
            return ws.ContainsModelloStampaRicevuta(sessionManager.getUserAmmSession(), registro);
        }

        private void impostaGUIRicevuta(bool esisteModello)
        {
            if (esisteModello)
            {
                this.lbl_ricevuta.Text = "Modello personalizzato presente.            Elimina ";
                this.btn_elimina_ricevuta.Visible = true;
                this.btn_elimina_ricevuta.Attributes.Add("onclick", "if (!window.confirm('Attenzione, questa operazione è indipendente dal tasto Modifica.\\n\\nSei sicuro di voler eliminare il modello personalizzato della ricevuta?')) {return false};");
            }
            else
            {
                this.lbl_ricevuta.Text = "Modello personalizzato non presente.";
                this.btn_elimina_ricevuta.Visible = false;
            }
        }

        #endregion

        protected void btn_test_Click(object sender, EventArgs e)
        {

            DocsPAWA.DocsPaWR.OrgRegistro registro = registro = (DocsPAWA.DocsPaWR.OrgRegistro)Session["REG_SEL"];
            if (registro != null)
            {
                string messaggioErroreSmtp = string.Empty;
                string messaggioErrore = string.Empty;
                //  this.AlertJS("PRIMA DI CHIUDERE LA PAGINA ATTENDERE IL MESSAGGIO DI CONFERMA DEL TEST!");
                bool smtp = ws.testConnessione(registro.Mail, "SMTP", out messaggioErroreSmtp);
                bool ingresso = ws.testConnessione(registro.Mail, this.ddl_posta.SelectedValue, out messaggioErrore);

                string messaggioDiConferma = "Connessione al server SMTP effettuata con Successo. E' stata inviata una mail di prova per la connessione. \n Connessione al server" + this.ddl_posta.SelectedValue + " Effettuata con Successo ";

                int width = 302;
                int heigth = 147;

                string paginaDaAprire = "avvisoConfermaConnessioneMail.aspx";

                if (!string.IsNullOrEmpty(messaggioErrore) ||
                    !string.IsNullOrEmpty(messaggioErroreSmtp))
                {
                    heigth = 427;
                    paginaDaAprire = "avvisoErroreConnessioneMail.aspx";
                    messaggioDiConferma = messaggioErroreSmtp + " \n" + messaggioErrore;
                }


                Session.Add("messaggiErrorePosta", messaggioDiConferma);
                string script = "<script>window.showModalDialog('../../popup/" + paginaDaAprire
                + "','','dialogWidth:" + width + "px;dialogHeight:" + heigth + "px;fullscreen:no;toolbar:no;status:no;resizable:no;scroll:auto;"
                + "center:yes;help:no;close:no');</script>";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "popupErroreMail", script, false);

            }

        }

        protected void btn_SbloccaCasella_Click(object sender, EventArgs e)
        {
            DocsPAWA.DocsPaWR.OrgRegistro registro = registro = (DocsPAWA.DocsPaWR.OrgRegistro)Session["REG_SEL"];
            if (registro != null && ddl_caselle.SelectedItem != null)
            {
                DocsPAWA.AdminTool.Manager.AmministrazioneManager admin = new DocsPAWA.AdminTool.Manager.AmministrazioneManager();
                if (!admin.SbloccoCasella(ddl_caselle.SelectedItem.Value, registro.IDRegistro))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningSbloccoCasella", "<script>alert('Attenzione! Si è verificato un errore durante la procedura di sblocco della casella.');</script>", false);
                    return;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningSbloccoCasella", "<script>alert('Sblocco della casella effettuato con successo.');</script>", false);
                    return;
                }
            }
        }

        /// <summary>
        /// Metodo associato all'evento ItemCommand del datagrid con la lista dei repertori. Visualizza
        /// la maschera con i dettagli del registro
        /// </summary>
        protected void dgRepertori_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Select":
                    // Apertura della pagina di modifica impostazioni
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "openSettings", DocsPAWA.popup.ModifyRepertorioSettings.GetOpenScript(((HiddenField)e.Item.FindControl("hfRegistryId")).Value), true);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Evento scatenato prima di effettuare la selezione dei dati da inserire nell'anagrafica repertori
        /// </summary>
        protected void RegistriRepertorioManagerDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            String[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            String codiceAmministrazione = amministrazione[0];
            String idAmministrazione = ws.getIdAmmByCod(codiceAmministrazione);
            e.InputParameters["administrationId"] = idAmministrazione;

        }


        #region Multi Casella
        /// <summary>
        /// Add casella/e di posta nella lista
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void img_aggiungiCasella_Click(Object sender, ImageClickEventArgs e)
        {
            //verifico che l'indirizzo non sia vuoto
            if (string.IsNullOrEmpty(txt_indirizzo.Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningInserisciIndirizzo", "<script>alert('Immettere una casella di posta elettronica valida!');</script>", false);
                return;
            }

            //verifico il formato dell'indirizzo mail
            string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
            if (!System.Text.RegularExpressions.Regex.Match(txt_indirizzo.Text, pattern).Success)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningInserisciIndirizzo", "<script>alert('Formato casella di posta non valido!');</script>", false);
                return;
            }

            string[] listaCaselle = txt_indirizzo.Text.Split(';'); //split delle caselle immesse dall'utente

            Hashtable hashT = new Hashtable();
            //verifico quali caselle inserire nella drop down list(scarto quelle già presenti).
            foreach (ListItem i in ddl_caselle.Items)
            {
                for (int index = 0; index < listaCaselle.Length; ++index)
                {
                    if (listaCaselle[index].Equals(i.Text))
                    {
                        hashT.Add(index, listaCaselle[index]);
                        break;
                    }
                }
            }
            int selectedIndexItem = ddl_caselle.SelectedIndex;

            //aggiorno la ddl
            for (int x = 0; x < listaCaselle.Length; ++x)
            {
                if (!hashT.ContainsKey(x))
                {
                    //se ho superato tutti i controlli aggiungo la mail alla drop down list
                    ddl_caselle.Items.Add(listaCaselle[x]);
                }
            }

            System.Collections.Generic.List<string> listCasellePresenti = new System.Collections.Generic.List<string>();
            //visualizzo un alert per le eventuali caselle non inserite in ddl
            if (hashT.Count > 0)
            {
                string[] listaCasellePresenti = new string[hashT.Count];
                hashT.Values.CopyTo(listaCasellePresenti, 0);
                string msg = string.Empty;
                if (hashT.Count > 1)
                {
                    msg = "Le caselle di posta: ";
                    for (int x = 0; x < listaCasellePresenti.Length; ++x)
                    {
                        msg += "\\n" + listaCasellePresenti[x];
                    }
                }
                else
                    msg = "La casella di posta " + listaCasellePresenti[0] + " è già presente!";

                if (hashT.Count > 1)
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningCasellaEsistente", "<script>alert('" + msg + " sono già presenti!');</script>", false);
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningCasellaEsistente", "<script>alert('" + msg + "');</script>", false);

                ddl_caselle.SelectedIndex = selectedIndexItem;
                foreach (string c in listaCasellePresenti)
                    listCasellePresenti.Add(c);
            }
            DocsPAWA.DocsPaWR.CasellaRegistro[] newCaselle = new DocsPAWA.DocsPaWR.CasellaRegistro[listaCaselle.Length - listCasellePresenti.Count];
            int indice = 0;

            //inserisco le nuove caselle nel DB
            for (int i = 0; i < listaCaselle.Length; ++i)
            {
                if (!listCasellePresenti.Contains(listaCaselle[i]))
                {
                    newCaselle[indice] = new DocsPAWA.DocsPaWR.CasellaRegistro();
                    newCaselle[indice].EmailRegistro = listaCaselle[i];
                    newCaselle[indice].RicevutaPEC = "-";
                    ++indice;
                }
            }
            if (newCaselle != null && newCaselle.Length > 0 && (!string.IsNullOrEmpty(this.CurrentIDRegistro)))
            {
                DocsPAWA.DocsPaWR.ValidationResultInfo res = MultiCasellaManager.InsertMailRegistro(this.CurrentIDRegistro, newCaselle, false);
                //associo i diritti sulla nuova casella a tutti i ruoli dell'RF
                if (res.Value)
                {
                    Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                    //Cerco solo i ruoli della AOO COLLEGATA: idReg 
                    theManager.GetListaRuoliAOO(this.CurrentIDRegistro);
                    if (theManager.getListaRuoliAOO() != null && theManager.getListaRuoliAOO().Count > 0)
                    {
                        System.Collections.Generic.List<DocsPAWA.DocsPaWR.RightRuoloMailRegistro> rightRuoloMailRegistro = new System.Collections.Generic.List<DocsPAWA.DocsPaWR.RightRuoloMailRegistro>();
                        foreach (DocsPAWA.DocsPaWR.OrgRuolo ruolo in theManager.getListaRuoliAOO())
                        {
                            foreach (DocsPAWA.DocsPaWR.CasellaRegistro c in newCaselle)
                            {
                                //di default imposto la visibilità del ruolo sulla nuova mail a 0(nessuna diritto)
                                // l'utente modifica la visibilità da organigramma
                                DocsPAWA.DocsPaWR.RightRuoloMailRegistro right = new DocsPAWA.DocsPaWR.RightRuoloMailRegistro
                                {
                                    IdRegistro = this.CurrentIDRegistro,
                                    idRuolo = ruolo.IDCorrGlobale,
                                    EmailRegistro = c.EmailRegistro,
                                    cha_consulta = "false",
                                    cha_notifica = "false",
                                    cha_spedisci = "false"
                                };
                                rightRuoloMailRegistro.Add(right);
                            }
                        }
                        res = ws.AmmInsRightMailRegistro(rightRuoloMailRegistro.ToArray());
                    }
                }
                if (res.Value == false)
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningCreazioneCaselle", "<script>alert('Errore nelle creazione delle caselle di posta!');</script>", false);
            }
            else if (newCaselle.Length > 0) //se siamo in creazione nuovo registro salvo semplicemente in sessione le nuove caselle(per salvarle devo prima terminare la creazione del registro)
            {
                int countCaselleEsistenti = Caselle != null ? Caselle.Length : 0;
                DocsPAWA.DocsPaWR.CasellaRegistro[] c = new DocsPAWA.DocsPaWR.CasellaRegistro[newCaselle.Length + countCaselleEsistenti];
                int i = 0;
                if (Caselle != null)
                {
                    foreach (DocsPAWA.DocsPaWR.CasellaRegistro oldC in Caselle)
                    {
                        c[i] = oldC;
                        ++i;
                    }
                }
                foreach (DocsPAWA.DocsPaWR.CasellaRegistro newC in newCaselle)
                {
                    c[i] = newC;
                    ++i;
                }
                Caselle = c;
                //imposto come ultima casella selezionata quella correntemente visualizzata in ddl
                PrevCasella = ddl_caselle.SelectedValue;
            }

            //load informazioni sulle caselle di posta(le metto in sessione)
            if (!OnInsertMode())
                SetCaselleRegistro(CurrentIDRegistro);
            pnl_info.Style.Remove("background");
            pnl_info.Style.Remove("filter");
            pnl_info.Style.Remove("opacity");
            this.txt_indirizzo.Text = string.Empty;

            if (!DocsPAWA.utils.MultiCasellaManager.IsEnabledMultiMail(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3")))
            {
                if (ddl_caselle.Items.Count > 0)
                {
                    this.txt_indirizzo.Enabled = false;
                    this.img_aggiungiCasella.Enabled = false;
                    this.ddl_caselle.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Gestisce il passaggio da una casella di posta ad un'altra
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddl_caselle_Changed(object sender, EventArgs e)
        {
            //Prima di switchare sulla nuova casella 
            //1. verifico che la precedente casella sia stata correttamente configurata
            foreach (DocsPAWA.DocsPaWR.CasellaRegistro c in Caselle)
            {
                if (c.EmailRegistro.Equals(PrevCasella))
                {
                    if (!CheckUpdParamCasella())
                    {
                        foreach (ListItem prev in ddl_caselle.Items)
                        {
                            if (prev.Value.Equals(PrevCasella))
                                prev.Selected = true;
                            else
                                prev.Selected = false;
                        }
                        return;
                    }
                    break;
                }
            }

            //2. se il punto 1!=true(parametri correttamente configurati) effettuo un refresh dei parametri associati alla vecchia voce di ddl selezionata
            foreach (DocsPAWA.DocsPaWR.CasellaRegistro c in Caselle)
            {
                if (c.EmailRegistro.Equals(ddl_caselle.SelectedItem.Value))
                {
                    //3. aggiorno in session le informazioni sulla casella precedente
                    RefreshCasellaFromUI(PrevCasella.ToString());
                    //4. aggiorno la UI con le informazioni sull'attuale casella
                    RefreshCasella(c);

                    break;
                }
            }

            //5. effettuo i controlli sul flag principale
            foreach (DocsPAWA.DocsPaWR.CasellaRegistro c in Caselle)
            {
                if (c.EmailRegistro.Equals(ddl_caselle.SelectedItem.Value))
                {
                    if (string.IsNullOrEmpty(c.Principale))
                    {
                        cbx_casellaPrincipale.Checked = false;
                        cbx_casellaPrincipale.Enabled = true;
                    }
                    else
                    {
                        cbx_casellaPrincipale.Checked = true;
                        cbx_casellaPrincipale.Enabled = false;
                    }
                    PrevCasella = c.EmailRegistro;
                    break;
                }
            }

            this.txt_indirizzo.Text = string.Empty;
        }

        /// <summary>
        /// Gestione del flag principale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbx_casellaPrincipale_CheckedChanged(Object sender, EventArgs e)
        {
            CheckBox cbx = sender as CheckBox;
            if (cbx.Checked)
            {
                cbx.Enabled = false;
                //la casella precedentemente configurata come primaria, adesso va impostata come secondaria
                foreach (DocsPAWA.DocsPaWR.CasellaRegistro c in Caselle)
                {
                    if (!string.IsNullOrEmpty(c.Principale))
                    {
                        c.Principale = string.Empty;
                        break;
                    }
                }
                //imposto la casella correntemente selezionata come principale
                foreach (DocsPAWA.DocsPaWR.CasellaRegistro c in Caselle)
                {
                    //la casella precedentemente configurata come primaria, adesso va impostata come secondaria
                    if (c.EmailRegistro.Equals(ddl_caselle.SelectedItem.Value))
                    {
                        c.Principale = "1";
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Se viene impostato "Protocolla se Pec" non si può impostare le mail ricevute come pendenti.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbx_ChkMailPec_CheckedChanged(Object sender, EventArgs e)
        {
            CheckBox cbx = sender as CheckBox;
            if (cbx.Checked)
            {
                this.ChkMailRicevutePendenti.Checked = false;
                this.ChkMailRicevutePendenti.Enabled = false;
            }
            else
            {
                this.ChkMailRicevutePendenti.Enabled = true;
            }
        }


        /// <summary>
        /// Aggiorna i parametri della casella di posta con le informazioni sull'attuale casella selezionata dalla ddl
        /// </summary>
        /// <param name="casella"> casella di posta</param>
        private void RefreshCasella(DocsPAWA.DocsPaWR.CasellaRegistro casella)
        {
            this.ClearParametri();
            //this.txt_indirizzo.Text = casella.EmailRegistro;
            this.txt_utente.Text = casella.UserMail;
            this.txt_password.Text = casella.PwdMail;
            this.txt_conferma_pwd.Text = casella.PwdMail;
            this.txt_smtp.Text = casella.ServerSMTP;
            this.txt_pop.Text = casella.ServerPOP;
            this.ChkBoxPopSSl.Checked = (casella.PopSSL == "1") ? true : false;
            this.ChkBoxsmtp.Checked = (casella.SmtpSSL == "1") ? true : false;
            this.ChkBoxsmtpSTA.Checked = (casella.SmtpSta == "1") ? true : false;
            this.Chk_sslImap.Checked = (casella.ImapSSL == "1") ? true : false;
            this.ChkMailPec.Checked = (casella.SoloMailPEC == "1") ? true : false;
            // Per gestione pendenti tramite PEC
            this.ChkMailRicevutePendenti.Checked = (casella.MailRicevutePendenti == "1") ? true : false;
            this.ChkMailRicevutePendenti.Enabled = !this.ChkMailPec.Checked;

            this.txt_note.Text = casella.Note;
            if (casella.PortaSMTP > 0)
                this.txt_portasmtp.Text = casella.PortaSMTP.ToString();

            if (string.IsNullOrEmpty(casella.TipoConnessione) ||
              (string.IsNullOrEmpty(casella.ServerPOP) &&
               string.IsNullOrEmpty(casella.ServerIMAP) &&
               string.IsNullOrEmpty(casella.UserMail)))
            {
                ddl_posta.Items[0].Text = "";
                ddl_posta.Items[1].Text = "POP";
                ddl_posta.Items[2].Text = "IMAP";
            }
            else
                if (casella.TipoConnessione.Equals("POP"))
                {
                    ddl_posta.Items[0].Text = "POP";
                    ddl_posta.Items[1].Text = "IMAP";
                    ddl_posta.Items[2].Text = "";
                    this.ddl_posta.SelectedIndex = ddl_posta.Items.IndexOf(ddl_posta.Items.FindByValue(casella.TipoConnessione));

                    this.txt_pop.Text = casella.ServerPOP;
                    this.ChkBoxPopSSl.Checked = (casella.PopSSL == "1") ? true : false;
                    if (casella.PortaPOP > 0)
                        this.txt_portapop.Text = casella.PortaPOP.ToString();

                    this.txt_imap.Text = casella.ServerIMAP;
                    this.Chk_sslImap.Checked = (casella.ImapSSL == "1") ? true : false;
                    if (casella.PortaIMAP > 0)
                        this.txt_portaImap.Text = casella.PortaIMAP.ToString();
                    this.txt_inbox.Text = casella.IboxIMAP;
                    this.txt_mailElaborate.Text = casella.BoxMailElaborate;
                    this.txt_mailNonElaborate.Text = casella.MailNonElaborate;

                    this.txt_imap.Enabled = !true;
                    this.Chk_sslImap.Enabled = !true;
                    this.txt_portaImap.Enabled = !true;
                    this.txt_inbox.Enabled = !true;
                    this.txt_mailElaborate.Enabled = !true;
                    this.txt_mailNonElaborate.Enabled = !true;
                    this.txt_pop.Enabled = true;
                    this.ChkBoxPopSSl.Enabled = true;
                    this.txt_portapop.Enabled = true;

                }
                else
                    if (casella.TipoConnessione.Equals("IMAP"))
                    {
                        ddl_posta.Items[0].Text = "IMAP";
                        ddl_posta.Items[1].Text = "POP";
                        ddl_posta.Items[2].Text = "";
                        this.ddl_posta.SelectedIndex = ddl_posta.Items.IndexOf(ddl_posta.Items.FindByValue(casella.TipoConnessione));

                        this.txt_imap.Text = casella.ServerIMAP;
                        this.Chk_sslImap.Checked = (casella.ImapSSL == "1") ? true : false;
                        if (casella.PortaIMAP > 0)
                            this.txt_portaImap.Text = casella.PortaIMAP.ToString();
                        this.txt_inbox.Text = casella.IboxIMAP;
                        this.txt_mailElaborate.Text = casella.BoxMailElaborate;
                        this.txt_mailNonElaborate.Text = casella.MailNonElaborate;

                        this.txt_pop.Text = casella.ServerPOP;
                        this.ChkBoxPopSSl.Checked = (casella.PopSSL == "1") ? true : false;
                        if (casella.PortaPOP > 0)
                            this.txt_portapop.Text = casella.PortaPOP.ToString();


                        this.txt_imap.Enabled = true;
                        this.Chk_sslImap.Enabled = true;
                        this.txt_portaImap.Enabled = true;
                        this.txt_inbox.Enabled = true;
                        this.txt_mailElaborate.Enabled = true;
                        this.txt_mailNonElaborate.Enabled = true;
                        this.txt_pop.Enabled = !true;
                        this.ChkBoxPopSSl.Enabled = !true;
                        this.txt_portapop.Enabled = !true;

                    }

            this.txt_userid_smtp.Text = casella.UserSMTP;
            this.txt_pwd_smtp.Text = casella.PwdSMTP;
            this.txt_conferma_pwd_smtp.Text = casella.PwdSMTP;

            if (casella.RicevutaPEC != string.Empty)
            {
                string tipoPec = casella.RicevutaPEC.Substring(0, 1);
                this.ddl_ricevutaPec.SelectedIndex = ddl_ricevutaPec.Items.IndexOf(ddl_ricevutaPec.Items.FindByValue(tipoPec));
            }
            this.txt_note.Text = casella.Note;
            this.txt_msg_posta_in_uscita.Text = casella.MessageSendMail;
            this.cbx_sovrascrivi_messaggio.Checked = casella.OverwriteMessageAmm;
        }

        /// <summary>
        /// Reperimento e salvataggio in sessione delle caselle di posta associate al registro corrente
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        private void SetCaselleRegistro(string idRegistro)
        {
            //al caricamento del nuovo RF elimino le caselle di posta precedentemente selezionate
            this.DeleteCaselleRegistro();
            //svuoto la drop down list
            ddl_caselle.Items.Clear();
            Caselle = DocsPAWA.utils.MultiCasellaManager.GetMailRegistro(idRegistro);
            if (Caselle != null)
            {
                //popola la ddl caselle di posta
                foreach (DocsPAWA.DocsPaWR.CasellaRegistro c in Caselle)
                {
                    if (!string.IsNullOrEmpty(c.Principale))
                    {
                        ddl_caselle.Items.Add(new ListItem { Value = c.EmailRegistro, Selected = true });
                        PrevCasella = c.EmailRegistro;
                    }
                    else
                        ddl_caselle.Items.Add(c.EmailRegistro);
                }
                //per la casella principale seleziono e poi disabilito il flag  principale
                if (Caselle.Length > 0)
                {
                    cbx_casellaPrincipale.Checked = true;
                    cbx_casellaPrincipale.Enabled = false;
                }
            }

            if (!DocsPAWA.utils.MultiCasellaManager.IsEnabledMultiMail(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3")))
            {
                if (ddl_caselle.Items.Count > 0)
                {
                    this.txt_indirizzo.Enabled = false;
                    this.img_aggiungiCasella.Enabled = false;
                    this.ddl_caselle.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Elimina le informazioni sulle caselle di posta associate al Registro corrente dalla sessione
        /// </summary>
        private void DeleteCaselleRegistro()
        {
            string key = "CASELLE_POSTA_REGISTRO";
            if (HttpContext.Current.Session[key] != null)
                HttpContext.Current.Session.Remove(key);
        }

        /// <summary>
        /// Variabile di sessione per le caselle di posta associate all'RF corrente
        /// </summary>
        private DocsPAWA.DocsPaWR.CasellaRegistro[] Caselle
        {
            get
            {
                string key = "CASELLE_POSTA_REGISTRO";
                return (DocsPAWA.DocsPaWR.CasellaRegistro[])HttpContext.Current.Session[key];
            }
            set
            {
                string key = "CASELLE_POSTA_REGISTRO";
                if (HttpContext.Current.Session[key] == null)
                    HttpContext.Current.Session.Add(key, value);
                else
                    HttpContext.Current.Session[key] = value;
            }
        }

        /// <summary>
        /// Indirizzo mail selezionato precedentemente al change della ddl
        /// </summary>
        private object PrevCasella
        {
            get
            {
                string key = "CASELLA_PRECEDENTE";
                return HttpContext.Current.Session[key];
            }
            set
            {
                string key = "CASELLA_PRECEDENTE";
                if (HttpContext.Current.Session[key] == null)
                    HttpContext.Current.Session.Add(key, value);
                else
                    HttpContext.Current.Session[key] = value;
            }
        }

        /// <summary>
        /// Verifica che i campi associati ai parametri della casella di posta correntemente selezionata siano corretti
        /// </summary>
        /// <param name="casella">Casella di posta correntamente selezionata</param>
        /// <returns></returns>
        private bool CheckUpdParamCasella()
        {
            bool controlloPWD = false;
            bool controlloPWD_SMPT = false;
            bool controlloUtente = false;

            //modifica
            switch (ddl_posta.SelectedValue)
            {

                case "POP":
                    {
                        if (txt_pop.Text.Equals(string.Empty))
                        {
                            this.AlertJS("warning2", "Attenzione, Non hai inserito il server POP");
                            this.SetFocus(this.txt_pop);
                            return false;
                        }
                        if (txt_portapop.Text.Equals(string.Empty))
                        {
                            this.AlertJS("warning3", "Attenzione, Non hai inserito il numero di porta");
                            this.SetFocus(this.txt_portapop);
                            return false;
                        }

                        break;
                    }
                case "IMAP":
                    {
                        if (txt_imap.Text.Equals(string.Empty))
                        {
                            this.AlertJS("warning4", "Attenzione, Non hai inserito il server IMAP");
                            this.SetFocus(this.txt_imap);
                            return false;
                        }

                        if (txt_portaImap.Text.Equals(string.Empty))
                        {
                            this.AlertJS("warning5", "Attenzione, Non hai inserito il numero di porta");
                            this.SetFocus(this.txt_portaImap);
                            return false;
                        }
                        if (txt_inbox.Text.Equals(string.Empty))
                        {
                            this.AlertJS("warning6", "Attenzione, Non hai inserito la cartella per inbox della e-mail");
                            this.SetFocus(this.txt_inbox);
                        }
                        if (txt_mailElaborate.Text.Equals(string.Empty))
                        {
                            this.AlertJS("warning7", "Attenzione, Non hai inserito la cartella per le mail elaborate");
                            this.SetFocus(this.txt_mailElaborate);
                        }
                        if (txt_mailNonElaborate.Text.Equals(string.Empty))
                        {
                            this.AlertJS("warning8", "Attenzione, Non hai inserito la cartella per le mail elaborate");
                            this.SetFocus(this.txt_mailElaborate);
                        }

                        break;
                    }
            }

            //fien modifica
            if (this.txt_password.Text.Trim() != string.Empty || this.txt_conferma_pwd.Text.Trim() != string.Empty)
            {
                // controllo dell'uguaglianza
                if (this.txt_password.Text.Trim() != this.txt_conferma_pwd.Text.Trim())
                {
                    this.AlertJS("warning9", "Attenzione, la password e la conferma sono differenti");
                    this.SetFocus(this.txt_password);
                }
                else
                    controlloPWD = true;
            }
            else
                controlloPWD = true;

            if (this.txt_pwd_smtp.Text.Trim() != string.Empty || this.txt_conferma_pwd_smtp.Text.Trim() != string.Empty)
            {
                // controllo dell'uguaglianza
                if (this.txt_pwd_smtp.Text.Trim() != this.txt_conferma_pwd_smtp.Text.Trim())
                {
                    this.AlertJS("warning10", "Attenzione, la password SMTP e la conferma sono differenti");
                    this.SetFocus(this.txt_pwd_smtp);
                }
                else
                    controlloPWD_SMPT = true;
            }
            else
                controlloPWD_SMPT = true;

            if (this.btn_aggiungi.Text.Equals("Aggiungi"))
            {
                if (ws.getValueInteropNoMail())
                {
                    if (!this.txt_IdHiddenUtenteNomail.Text.Equals(""))
                    {
                        if (this.ddl_UtInteropNoMail.SelectedItem != null && this.ddl_UtInteropNoMail.SelectedItem.Text.Trim() != string.Empty)
                            controlloUtente = true;
                        else
                            this.AlertJS("warning11", "Attenzione, selezionare un utente creatore");
                    }
                    else { controlloUtente = true; }
                    // this.AlertJS("Attenzione, selezionare un ruolo per l'interoperabilità tra AOO senza posta elettronica");
                }
                else
                {
                    controlloUtente = true;

                    if (ddl_DirittoResp.Visible)
                    {

                        if (this.ddl_DirittoResp.SelectedItem != null && this.ddl_DirittoResp.Text.Trim() != "0")
                            controlloUtente = true;
                        else
                        {
                            controlloUtente = false;
                            this.AlertJS("warning12", "Attenzione, selezionare un diritto");
                        }
                    }

                }
            }
            else
            {
                controlloUtente = true;
            }

            if (this.txt_codice.Text.Contains("'"))
            {
                controlloUtente = false;
                this.AlertJS("warning13", "Attenzione, non inserire caratteri speciali nel codice registro");
            }

            if (controlloPWD && controlloPWD_SMPT && controlloUtente)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Aggiorna i parametri della casella di posta passata come parametro, in sessione
        /// </summary>
        /// <param name="registro"></param>
        private void RefreshCasellaFromUI(string indirizzoPrevCasella)
        {
            bool insertMode = this.OnInsertMode();
            foreach (DocsPAWA.DocsPaWR.CasellaRegistro c in Caselle)
            {
                if (c.EmailRegistro.Equals(indirizzoPrevCasella))
                {
                    try
                    {
                        c.UserMail = this.txt_utente.Text.Trim();
                        if (insertMode)
                            c.PwdMail = this.txt_password.Text.Trim();
                        else
                        {
                            if (!this.txt_password.Text.Trim().Equals(""))
                                c.PwdMail = this.txt_password.Text.Trim();
                        }
                        c.ServerSMTP = this.txt_smtp.Text.Trim();
                        c.ServerPOP = this.txt_pop.Text.Trim();
                        c.IboxIMAP = this.txt_inbox.Text.Trim();
                        c.BoxMailElaborate = this.txt_mailElaborate.Text.Trim();
                        c.ServerIMAP = this.txt_imap.Text.Trim();
                        c.TipoConnessione = this.ddl_posta.SelectedValue;
                        c.MailNonElaborate = this.txt_mailNonElaborate.Text.Trim();
                        c.ImapSSL = (this.Chk_sslImap.Checked == true) ? "1" : "0";
                        c.SoloMailPEC = (this.ChkMailPec.Checked == true) ? "1" : "0";

                        // Per gestione pendenti tramite PEC
                        c.MailRicevutePendenti = (this.ChkMailRicevutePendenti.Checked == true) ? "1" : "0";


                        if (insertMode)
                            c.PortaSMTP = (!string.IsNullOrEmpty(this.txt_portasmtp.Text.Trim())) ? Convert.ToInt32(this.txt_portasmtp.Text.Trim()) : 0;
                        else
                        {
                            if (!this.txt_portasmtp.Text.Trim().Equals(""))
                                c.PortaSMTP = Convert.ToInt32(this.txt_portasmtp.Text.Trim());
                        }
                        if (insertMode)
                            c.PortaPOP = (!string.IsNullOrEmpty(this.txt_portapop.Text.Trim())) ? Convert.ToInt32(this.txt_portapop.Text.Trim()) : 0;
                        else
                        {
                            if (!this.txt_portapop.Text.Trim().Equals(""))
                                c.PortaPOP = Convert.ToInt32(this.txt_portapop.Text.Trim());
                        }
                        if (insertMode)
                            c.PortaIMAP = (!string.IsNullOrEmpty(this.txt_portaImap.Text.Trim())) ? Convert.ToInt32(this.txt_portaImap.Text.Trim()) : 0;
                        else
                        {
                            if (!this.txt_portaImap.Text.Trim().Equals(""))
                                c.PortaIMAP = Convert.ToInt32(this.txt_portaImap.Text.Trim());
                        }
                        c.UserSMTP = this.txt_userid_smtp.Text.Trim();
                        if (insertMode)
                            c.PwdSMTP = this.txt_pwd_smtp.Text.Trim();
                        else
                        {
                            if (!this.txt_pwd_smtp.Text.Trim().Equals(""))
                                c.PwdSMTP = this.txt_pwd_smtp.Text.Trim();
                        }
                        c.ImapSSL = (this.Chk_sslImap.Checked == true) ? "1" : "0";
                        c.PopSSL = (this.ChkBoxPopSSl.Checked == true) ? "1" : "0";
                        c.SmtpSSL = (this.ChkBoxsmtp.Checked == true) ? "1" : "0";
                        c.SmtpSta = (this.ChkBoxsmtpSTA.Checked == true) ? "1" : "0";
                        c.RicevutaPEC = this.ddl_ricevutaPec.SelectedValue;
                        c.Note = txt_note.Text;
                        c.MessageSendMail = txt_msg_posta_in_uscita.Text;
                        c.OverwriteMessageAmm = this.cbx_sovrascrivi_messaggio.Checked;
                    }
                    catch (Exception e)
                    {
                        logger.Debug("errore - refreshCasellaFromUI: " + e.Message);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Aggiornamento registro con i dati sulla mail principale
        /// /// </summary>
        private void SaveMailPrincInRegistro(ref DocsPAWA.DocsPaWR.OrgRegistro registro)
        {
            if (Caselle != null)
            {
                foreach (DocsPAWA.DocsPaWR.CasellaRegistro c in Caselle)
                {
                    if (!string.IsNullOrEmpty(c.Principale))
                    {
                        registro.Mail.Email = c.EmailRegistro;
                        registro.Mail.UserID = c.UserMail;
                        registro.Mail.Password = c.PwdMail;
                        registro.Mail.ServerSMTP = c.ServerSMTP;
                        registro.Mail.ServerPOP = c.ServerPOP;
                        registro.Mail.inbox = c.IboxIMAP;
                        registro.Mail.mailElaborate = c.BoxMailElaborate;
                        registro.Mail.serverImap = c.ServerIMAP;
                        registro.Mail.tipoPosta = c.TipoConnessione;
                        registro.Mail.mailNonElaborate = c.MailNonElaborate;
                        registro.Mail.IMAPssl = c.ImapSSL;
                        registro.Mail.soloMailPec = c.SoloMailPEC;
                        // Per gestione pendenti tramite PEC
                        registro.Mail.MailRicevutePendenti = c.MailRicevutePendenti;

                        try
                        {
                            registro.Mail.PortaSMTP = c.PortaSMTP;
                            registro.Mail.PortaPOP = c.PortaPOP;
                            registro.Mail.portaIMAP = c.PortaIMAP;
                        }
                        catch (Exception e)
                        {
                            logger.Debug("errore - refreshRFFromUI: " + e.Message);
                        }
                        registro.Mail.UserSMTP = c.UserSMTP;
                        registro.Mail.PasswordSMTP = c.PwdSMTP;
                        registro.Mail.POPssl = c.PopSSL;
                        registro.Mail.SMTPssl = c.SmtpSSL;
                        registro.Mail.SMTPsslSTA = c.SmtpSta;
                        registro.Mail.IMAPssl = c.ImapSSL;
                        registro.Mail.pecTipoRicevuta = c.RicevutaPEC;
                        return;
                    }
                }

                //se non è stata selezionata una casella principale, imposto la prima di default come principale
                if (Caselle.Length > 0 && (!string.IsNullOrEmpty(Caselle[0].EmailRegistro)))
                {
                    registro.Mail.Email = Caselle[0].EmailRegistro;
                    registro.Mail.UserID = Caselle[0].UserMail;
                    registro.Mail.Password = Caselle[0].PwdMail;
                    registro.Mail.ServerSMTP = Caselle[0].ServerSMTP;
                    registro.Mail.ServerPOP = Caselle[0].ServerPOP;
                    registro.Mail.inbox = Caselle[0].IboxIMAP;
                    registro.Mail.mailElaborate = Caselle[0].BoxMailElaborate;
                    registro.Mail.serverImap = Caselle[0].ServerIMAP;
                    registro.Mail.tipoPosta = Caselle[0].TipoConnessione;
                    registro.Mail.mailNonElaborate = Caselle[0].MailNonElaborate;
                    registro.Mail.IMAPssl = Caselle[0].ImapSSL;
                    registro.Mail.soloMailPec = Caselle[0].SoloMailPEC;
                    // Per gestione pendenti tramite PEC
                    registro.Mail.MailRicevutePendenti = Caselle[0].MailRicevutePendenti;
                    try
                    {
                        registro.Mail.PortaSMTP = Caselle[0].PortaSMTP;
                        registro.Mail.PortaPOP = Caselle[0].PortaPOP;
                        registro.Mail.portaIMAP = Caselle[0].PortaIMAP;
                    }
                    catch (Exception e)
                    {
                        logger.Debug("errore - SaveMailPrincInRegistro: " + e.Message);
                    }
                    registro.Mail.UserSMTP = Caselle[0].UserSMTP;
                    registro.Mail.PasswordSMTP = Caselle[0].PwdSMTP;
                    registro.Mail.POPssl = Caselle[0].PopSSL;
                    registro.Mail.SMTPssl = Caselle[0].SmtpSSL;
                    registro.Mail.SMTPsslSTA = Caselle[0].SmtpSta;
                    registro.Mail.IMAPssl = Caselle[0].ImapSSL;
                    registro.Mail.pecTipoRicevuta = Caselle[0].RicevutaPEC;
                }
            }
        }

        /// <summary>
        /// Rimozione dati controlli UI per i parametri di configurazione della mail
        /// </summary>
        private void ClearParametri()
        {
            txt_indirizzo.Text = string.Empty;
            txt_utente.Text = string.Empty;
            txt_password.Text = string.Empty;
            txt_smtp.Text = string.Empty;
            txt_pop.Text = string.Empty;
            txt_portasmtp.Text = string.Empty;
            txt_portapop.Text = string.Empty;
            txt_userid_smtp.Text = string.Empty;
            txt_pwd_smtp.Text = string.Empty;
            txt_inbox.Text = string.Empty;
            txt_mailElaborate.Text = string.Empty;
            txt_mailNonElaborate.Text = string.Empty;
            txt_imap.Text = string.Empty;
            Chk_sslImap.Checked = false;
            ChkBoxPopSSl.Checked = false;
            this.txt_portapop.Text = string.Empty;
            this.txt_portaImap.Text = string.Empty;
            txt_note.Text = string.Empty;
        }

        /// <summary>
        /// Gestione eliminazione mail
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void img_eliminaCasella_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imgDelete = sender as ImageButton;
            if (!string.IsNullOrEmpty(ddl_caselle.SelectedValue) && Caselle != null)
            {
                foreach (DocsPAWA.DocsPaWR.CasellaRegistro c in Caselle)
                {
                    if (c.EmailRegistro.Equals(ddl_caselle.SelectedValue))
                    {
                        if (c.Principale != null && c.Equals("1") && Caselle.Length > 1) //verifico se è possibile eliminare la mail
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningElim", "<script>alert('Impossibile eliminare la casella di posta principale');</script>", false);
                        }
                        else
                        {
                            //siamo in insert
                            if (string.IsNullOrEmpty(this.CurrentIDRegistro))
                            {
                                //1. elimino la mail dalla ddl
                                ddl_caselle.Items.Remove(c.EmailRegistro);
                                //2. set prevCasella
                                PrevCasella = ddl_caselle.SelectedValue;
                                //3. elimino la mail dalla sessione
                                if (Caselle.Length - 1 > 0)
                                {
                                    DocsPAWA.DocsPaWR.CasellaRegistro[] newListCaselle = new DocsPAWA.DocsPaWR.CasellaRegistro[Caselle.Length - 1];
                                    for (int i = 0, y = 0; i < Caselle.Length; ++i)
                                    {
                                        if (!Caselle[i].EmailRegistro.Equals(c.EmailRegistro))
                                        {
                                            newListCaselle[y] = Caselle[i];
                                            ++y;
                                        }
                                    }
                                    Caselle = newListCaselle;
                                    RefreshCasella(Caselle[0]);
                                }
                                else
                                {
                                    DeleteCaselleRegistro();
                                    ClearParametri();
                                }
                            }

                            //siamo in update
                            else
                            {
                                //elimino da db la casella di posta
                                DocsPAWA.DocsPaWR.CasellaRegistro[] listaCaselleInDB = DocsPAWA.utils.MultiCasellaManager.GetMailRegistro(this.CurrentIDRegistro);
                                //se in db la casella che si sta eliminando è salvata come principale, allora avviso l'utente che prima dell'eliminazione
                                //deve salvare la nuova casella di posta principale
                                if (Caselle.Length > 1 && listaCaselleInDB[0] != null && listaCaselleInDB[0].EmailRegistro.Equals(c.EmailRegistro))
                                {
                                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningElim2", "<script>alert('Prima di eliminare la casella è necessario salvare la nuova casella principale');</script>", false);
                                    return;
                                }
                                Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                                if (!theManager.ExistsPassiFirmaByIdRegistroAndEmailRegistro(c.IdRegistro, c.EmailRegistro))
                                    EliminaCasella(c);
                                else
                                {
                                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "AvvisoPresenzaProcessiFirmaEmail", "<script>AvvisoPresenzaProcessiFirmaEmail();</script>", false);
                                    return;
                                }
                            }

                        }
                        break;
                    }
                }
            }
            if (!DocsPAWA.utils.MultiCasellaManager.IsEnabledMultiMail(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3")))
            {
                if (ddl_caselle.Items.Count < 1)
                {
                    this.txt_indirizzo.Enabled = true;
                    this.img_aggiungiCasella.Enabled = true;
                    this.ddl_caselle.Enabled = true;
                }
            }
        }

        private void EliminaCasella(DocsPAWA.DocsPaWR.CasellaRegistro c)
        {
            DocsPAWA.DocsPaWR.ValidationResultInfo result = ws.AmmDeleteMailRegistro(this.CurrentIDRegistro, c.EmailRegistro);
            if (result.Value)
            {
                //elimino i diritti sulla nuova casella per tutti i ruoli dell'RF
                Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                //Cerco solo i ruoli della AOO COLLEGATA: idReg 
                theManager.GetListaRuoliAOO(this.CurrentIDRegistro);
                if (theManager.getListaRuoliAOO() != null && theManager.getListaRuoliAOO().Count > 0)
                {
                    foreach (DocsPAWA.DocsPaWR.OrgRuolo ruolo in theManager.getListaRuoliAOO())
                    {
                        ws.AmmDelRightMailRegistro(this.CurrentIDRegistro, ruolo.IDCorrGlobale, c.EmailRegistro);
                    }
                } //end aggiornamento diritti ruoli
                SetCaselleRegistro(this.CurrentIDRegistro);
                if (Caselle.Length > 0)
                {
                    RefreshCasella(Caselle[0]);
                }
                FillListRegistri();
            }
            else
            {
                this.ShowValidationMessage(result);
            }
        }
        #endregion

        protected void chkPregresso_Click(object sender, EventArgs e)
        {
            if (this.chk_pregresso.Checked)
            {
                this.txt_anno_reg_pre.ReadOnly = false;

                //Disabilito i campi che non sono di pregresso
                this.ChkAutomatico.Checked = false;
                this.ChkAutomatico.Enabled = false;
                this.ChkSospeso.Checked = false;
                this.ChkSospeso.Enabled = false;

                this.ruoloRespReg.Enabled = false;
                this.ruoloRespReg.Text = string.Empty;
                this.btn_RubricaRuoloResp.Enabled = false;
                this.img_delRuoloResp.Enabled = false;
                this.ddl_DirittoResp.Enabled = false;
            }
            else
            {
                this.txt_anno_reg_pre.ReadOnly = true;
                this.txt_anno_reg_pre.Text = string.Empty;
                this.ChkAutomatico.Enabled = true;
                this.ChkSospeso.Enabled = true;

                this.ruoloRespReg.Enabled = true;
                this.btn_RubricaRuoloResp.Enabled = true;
                this.img_delRuoloResp.Enabled = true;
                this.ddl_DirittoResp.Enabled = true;

            }
            pnl_pregresso.Update();
        }

        public bool ControlloPregressi()
        {
            bool retValue = false;

            if (this.chk_pregresso.Checked)
            {
                if (!string.IsNullOrEmpty(this.txt_anno_reg_pre.Text) && this.txt_anno_reg_pre.Text.Length == 4)
                {
                    retValue = true;
                }
                else
                {
                    this.txt_anno_reg_pre.Text = string.Empty;
                    RegisterClientScript("alertAnnoPregressi", "alert('AVVISO: Il campo Anno deve essere popolato con un anno valido (4 cifre)!');");
                    pnl_pregresso.Update();
                    retValue = false;
                }
            }
            else
            {
                retValue = true;
            }
            return retValue;
        }

        protected void PopolaDdlUtenti()
        {
            if (!string.IsNullOrEmpty(this.codRuoloRespReg.Text))
            {
                Ruolo r = DocsPAWA.UserManager.getRuoloById(this.codRuoloRespReg.Text, this);
                UserMinimalInfo[] userInRole = DocsPAWA.UserManager.GetUsersInRoleMinimalInfo(r.idGruppo);
                this.ddl_user.Items.Clear();
                foreach (UserMinimalInfo user in userInRole)
                {
                    ListItem item = new ListItem();
                    item.Text = user.Description;
                    item.Value = user.SystemId;
                    this.ddl_user.Items.Add(item);
                }
            }
        }



    }
}
