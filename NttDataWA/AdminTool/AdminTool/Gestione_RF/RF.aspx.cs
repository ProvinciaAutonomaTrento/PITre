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
using SAAdminTool;
using SAAdminTool.AdminTool.Manager;
using System.Configuration;

namespace Amministrazione.Gestione_RF
{
    /// <summary>
    /// Summary description for TipiFunzione.
    /// </summary>
    public partial class RF : System.Web.UI.Page
    {
        private ILog logger = LogManager.GetLogger(typeof(RF));
        #region WebControls e variabili
        protected System.Web.UI.HtmlControls.HtmlForm Form1;
        protected System.Web.UI.WebControls.Label lbl_position;
        protected System.Web.UI.WebControls.Button btn_nuova;

        protected System.Web.UI.WebControls.TextBox txt_codice;
        protected System.Web.UI.WebControls.TextBox txt_descrizione;
  
        protected System.Web.UI.WebControls.Button btn_aggiungi;
        protected System.Web.UI.WebControls.DataGrid dg_RF;
      

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
        protected SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();

       
        protected System.Web.UI.WebControls.CheckBox ChkBoxPopSSl;
        protected System.Web.UI.WebControls.CheckBox ChkBoxsmtp;
        protected System.Web.UI.WebControls.CheckBox ChkBoxsmtpSTA;

        //---------------------------------------------------------------------------------------------
        protected System.Web.UI.WebControls.CheckBox ChkDisabilitato;
        protected System.Web.UI.WebControls.TextBox txt_userid_smtp;
        protected System.Web.UI.WebControls.TextBox txt_pwd_smtp;
        protected System.Web.UI.WebControls.TextBox txt_conferma_pwd_smtp;
        protected System.Web.UI.WebControls.TextBox txt_indirizzo;
        protected System.Web.UI.WebControls.Panel pnl_info;
        //---------------------------------------------------------------------------------------------
        #endregion

        // Costanti che identificano i nomi delle colonne del datagrid
        private const string TABLE_COL_ID = "ID";
        private const string TABLE_COL_CODICE = "Codice";
        private const string TABLE_COL_DESCRIZIONE = "Descrizione";
        private const string TABLE_COL_EMAIL = "Email";
        private const string TABLE_COL_DISABLED = "Disabled";

        private const int GRID_COL_ID = 0;
        private const int GRID_COL_CODICE = 1;
        private const int GRID_COL_DESCRIZIONE = 2;
        private const int GRID_COL_EMAIL = 3;
        private const int GRID_COL_DETAIL = 4;
        private const int GRID_COL_DELETE = 5;
        private const int GRID_COL_DISABLED = 6;

       // protected System.Web.UI.WebControls.CheckBox cbx_inviomanuale;
       
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtCommandPending;
        protected System.Web.UI.WebControls.Label lbl_msg;

        // Controllo gestione RubricaComune
        protected SAAdminTool.AdminTool.UserControl.RubricaComune RubricaComune1;
       

        #region Page_Load

        private void Page_Load(object sender, System.EventArgs e)
        {
            Session["AdminBookmark"] = "GestioneRF";
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

            this.RegisterScrollKeeper("DivDGList");

            // Inizializzazione hashtable businessrules
            this.InitializeBusinessRuleControls();

            if (!IsPostBack)
            {
                this.AddControlsClientAttribute();


                // Caricamento lista RAGGRUPPAMENTI FUNZIONALI 
                this.FillListRF();

                this.lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
            }

            if (this.CommandPending.Equals("DELETE"))
            {
                this.Delete(true);
                this.CommandPending = string.Empty;
            }

            if (this.CommandPending.Equals("MODIFY"))
            {
                this.Save();
                this.CommandPending = string.Empty;
            }

            //modifica
            switch (ddl_posta.SelectedValue)
            {
                case "":
                    {
                             
                        Chk_sslImap.Checked = false;
                        ChkBoxPopSSl.Checked = false;
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
            //fine modifca

            this.RubricaComune1.IdElemento = this.CurrentIDRF;
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
            this.dg_RF.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dg_RF_ItemCreated);
            this.dg_RF.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_RF_ItemCommand);
            this.dg_RF.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DeleteItemCommand);
            this.btn_aggiungi.Click += new System.EventHandler(this.btn_aggiungi_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.btn_chiudiPnlInfo.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudiPnlInfo_Click);
            this.msg_confirmModificaRF.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_confirmModificaRF_GetMessageBoxResponse);
            this.ddl_registri.SelectedIndexChanged += new EventHandler(ddl_registri_SelectedIndexChanged);
            this.btn_test.Click += new System.EventHandler(this.btn_test_Click);
            this.img_aggiungiCasella.Click += new ImageClickEventHandler(this.img_aggiungiCasella_Click);
            this.ddl_caselle.SelectedIndexChanged += new EventHandler(this.ddl_caselle_Changed);
            this.cbx_casellaPrincipale.CheckedChanged += new EventHandler(this.cbx_casellaPrincipale_CheckedChanged);
            this.ChkMailPec.CheckedChanged += new EventHandler(this.cbx_ChkMailPec_CheckedChanged);
            img_eliminaCasella.Click += new ImageClickEventHandler(this.img_eliminaCasella_Click);
            
        }

        void ddl_registri_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ddl_registri != null && this.ddl_registri.SelectedIndex > 0)
            {
                SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
                SAAdminTool.DocsPaWR.Registro reg = ws.GetRegistroBySistemId(this.ddl_registri.SelectedValue);
                if (reg.Sospeso)
                {
                    RegisterClientScript("alertRegistroSospeso", "alert('Il registro selezionato è sospeso!');");
                    this.ddl_registri.SelectedIndex = 0;
                }
            }

        }
        #endregion

        #region dg_RF

        private void RegisterScrollKeeper(string divID)
        {
            SAAdminTool.AdminTool.UserControl.ScrollKeeper scrollKeeper = new SAAdminTool.AdminTool.UserControl.ScrollKeeper();
            scrollKeeper.WebControl = divID;
            this.Form1.Controls.Add(scrollKeeper);
        }

        /// <summary>
        /// dg_RF_ItemCommand
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void dg_RF_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            string commandName = e.CommandName;
            pnl_info.Visible = true;		
            if (commandName.Equals("Select"))
            {
                //visibilià informazioni
                pnl_info.Visible = true;
                btn_aggiungi.Text = "Salva modifiche";
                txt_codice.Visible = false;
                lbl_cod.Visible = true;
                SetFocus(txt_descrizione);

                // Assegnazione valori campi UI
                SAAdminTool.DocsPaWR.OrgRegistro registro = this.GetRegistro(e.Item.Cells[GRID_COL_ID].Text);

                this.BindUI(registro);

                // Impostazione della descrizione dell'RF
                this.RubricaComune1.Descrizione = String.Format("{0} - {1}", registro.CodiceAmministrazione, registro.Descrizione);

                // In fase di modifica viene visualizzato il controllo della rubrica comune
                this.RubricaComune1.Visible = true;

                // Impostazione id registro correntemente selezionato
                this.CurrentIDRF = registro.IDRegistro;
                //salvo le informazioni sulle caselle associate all'RF corrente
                this.SetCaselleRegistro(this.CurrentIDRF);
                if (Caselle != null && Caselle.Length > 0) // aggiorno le note della casella di default
                    this.txt_note.Text = Caselle[0].Note;
            }
        }

        /// <summary>
        /// ID del registro correntemente selezionato
        /// </summary>
        private string CurrentIDRF
        {
            get
            {
                string retValue = this.ViewState["ID_RF"] as string;
                if (retValue == null)
                    retValue = string.Empty;
                return retValue;
            }
            set
            {
                this.ViewState["ID_RF"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dg_RF_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            e.Item.Cells[GRID_COL_DELETE].Attributes.Add("onclick", "if (!window.confirm('Sei sicuro di voler eliminare questo raggruppamento funzionale?')) {return false};");

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
            ViewState["CodAooColl"] = null;
            this.SetInsertMode();
            DeleteCaselleRegistro();
            ddl_caselle.Items.Clear();
            ClearData();
            ClearParametri();
        }

        private void btn_aggiungi_Click(object sender, System.EventArgs e)
        {
            //se true errore, quindi termino senza salvare
            if (CheckUpdParamCasella())
                return;
            if (ViewState["CodAooColl"] != null && ViewState["CodAooColl"].ToString() != ddl_registri.SelectedItem.Text)
            {
                string msg = "Attenzione, si è deciso di modificare la Aoo collegata, pertanto i ruoli\\nche non vedranno questo registro saranno disassociati automaticamente.\\n\\nSi desidera continuare?";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ShowValidationMessage", "<script>ShowContinueModifyRF('" + msg + "');</script>", false);
                return;
            }
            Save();
        }

        private void msg_confirmModificaRF_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                this.Save();
            }
        }
        #endregion

        #region pannelli

        private void btn_chiudiPnlInfo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            pnl_info.Visible = false;

            this.ClearData();

            dg_RF.SelectedIndex = -1;
        }
        #endregion

        #region Gestione accesso ai dati

        /// <summary>
        /// Gestione caricamento lista registri
        /// </summary>
        private void FillListRF()
        {
            SAAdminTool.DocsPaWR.OrgRegistro[] registri = this.GetRegistri_RF_Amministrazione("1");
            DataSet ds = this.ConvertToDataSet(registri);

            this.dg_RF.DataSource = ds;
            this.dg_RF.DataBind();

            registri = null;
        }

        /// <summary>
        /// Conversione array
        /// </summary>
        /// <param name="registri"></param>
        /// <returns></returns>
        private DataSet ConvertToDataSet(SAAdminTool.DocsPaWR.OrgRegistro[] registri)
        {
            DataSet ds = this.CreateGridDataSet();
            DataTable dt = ds.Tables["Registri"];

            foreach (SAAdminTool.DocsPaWR.OrgRegistro registro in registri)
            {
                DataRow row = dt.NewRow();

                row[TABLE_COL_ID] = registro.IDRegistro;
                row[TABLE_COL_CODICE] = registro.Codice;
                row[TABLE_COL_DESCRIZIONE] = registro.Descrizione;
                row[TABLE_COL_EMAIL] = registro.Mail.Email;
                row[TABLE_COL_DISABLED] = registro.rfDisabled;

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
            dt.Columns.Add(new DataColumn(TABLE_COL_DISABLED));
            ds.Tables.Add(dt);
            return ds;
        }

        /// <summary>
        /// Associazione dati registro ai campi della UI
        /// </summary>
        /// <param name="registro"></param>
        private void BindUI(SAAdminTool.DocsPaWR.OrgRegistro registro)
        {
            this.ClearData();

            btn_test.Visible = true;

            this.txt_codice.Text = registro.Codice;
            this.lbl_cod.Text = this.txt_codice.Text;
            this.txt_descrizione.Text = registro.Descrizione;

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

            this.cbx_invioAuto.Checked = (registro.invioRicevutaManuale == "0") ? true : false;

            if (registro.Mail.PortaSMTP > 0)
                this.txt_portasmtp.Text = registro.Mail.PortaSMTP.ToString();
           
            if(string.IsNullOrEmpty(registro.Mail.tipoPosta) ||
              (string.IsNullOrEmpty(registro.Mail.ServerPOP) &&
               string.IsNullOrEmpty(registro.Mail.serverImap) &&
               string.IsNullOrEmpty(registro.Mail.UserID)))
            {
                ddl_posta.Items[0].Text = "";
                ddl_posta.Items[1].Text = "POP";
                ddl_posta.Items[2].Text = "IMAP";
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

            this.txt_userid_smtp.Text = registro.Mail.UserSMTP;
            this.txt_pwd_smtp.Text = registro.Mail.PasswordSMTP;
            this.txt_conferma_pwd_smtp.Text = registro.Mail.PasswordSMTP;
            
            this.ChkDisabilitato.Checked = (registro.rfDisabled=="1");

            CaricaComboRegistri();
            this.ddl_registri.SelectedIndex = ddl_registri.Items.IndexOf(ddl_registri.Items.FindByValue(registro.idAOOCollegata));

            ViewState["CodAooColl"] = this.ddl_registri.SelectedItem.Text;

            // pulsante "Gestione ruoli in RF"
            this.btn_GestRuoliInUo.Attributes.Add("onclick", "ApriGestioneRuoliInRF('" + ddl_registri.SelectedItem.Value + "', '" + Server.UrlEncode(registro.Codice) + "', '" + Server.UrlEncode(ddl_registri.SelectedItem.Text) + "', '" + registro.rfDisabled + "', '" + registro.IDRegistro+ "', '" + registro.IDAmministrazione+ "');");
            
            if (registro.Mail.pecTipoRicevuta != string.Empty)
            {
                string tipoPec = registro.Mail.pecTipoRicevuta.Substring(0, 1);
                this.ddl_ricevutaPec.SelectedIndex = ddl_ricevutaPec.Items.IndexOf(ddl_ricevutaPec.Items.FindByValue(tipoPec));
            }

            this.RubricaComune1.IdElemento = registro.IDRegistro;
        }

        /// <summary>
        /// Aggiornamento registro dai dati dei campi della UI
        /// </summary>
        private void RefreshRFFromUI(ref SAAdminTool.DocsPaWR.OrgRegistro registro, bool insertMode)
        {
            if(!insertMode)
                registro = this.GetRegistro(this.CurrentIDRF);

            registro.IDRegistro = this.CurrentIDRF;
            registro.Codice = this.txt_codice.Text.Trim();
            registro.Descrizione = this.txt_descrizione.Text.Trim();

            string codiceAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");
            registro.CodiceAmministrazione = codiceAmministrazione;

            if(registro.Mail == null)
                registro.Mail = new SAAdminTool.DocsPaWR.MailRegistro();
            registro.invioRicevutaManuale = (this.cbx_invioAuto.Checked == true) ? "0" : "1";
            registro.AperturaAutomatica = false;
            registro.rfDisabled = "0";
            registro.Sospeso = false;
            if (this.ChkDisabilitato.Checked)
            {
                registro.rfDisabled = "1";
                registro.Sospeso = true;
            }
            
            registro.chaRF = "1";
            registro.idAOOCollegata = ddl_registri.SelectedValue;
        }

        /// <summary>
        /// Reperimento registri
        /// </summary>
        /// <returns></returns>
        public SAAdminTool.DocsPaWR.OrgRegistro[] GetRegistri_RF_Amministrazione(string chaRF)
        {
            string codiceAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");
            return this.GetRegistri_RF_Amministrazione(codiceAmministrazione, chaRF);
        }

        /// <summary>
        /// Reperimento RF
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public SAAdminTool.DocsPaWR.OrgRegistro[] GetRegistri_RF_Amministrazione(string codiceAmministrazione, string chaRF)
        {
            return ws.AmmGetRegistri(codiceAmministrazione, chaRF);
        }

        /// <summary>
        /// Reperimento registro
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        private SAAdminTool.DocsPaWR.OrgRegistro GetRegistro(string idRegistro)
        {
            return ws.AmmGetRegistro(idRegistro);
        }

        /// <summary>
        /// Salvataggio dati
        /// </summary>
        /// <returns></returns>
       /* private void Save()
        {
            SAAdminTool.DocsPaWR.OrgRegistro registro = new SAAdminTool.DocsPaWR.OrgRegistro();
            

            SAAdminTool.DocsPaWR.ValidationResultInfo result = null;

            bool insertMode = this.OnInsertMode();

            this.RefreshRFFromUI(ref registro, insertMode);
            
            if (insertMode)
                result = this.InsertRF(ref registro);
            else
                result = this.UpdateRF(ref registro);

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

                dg_RF.SelectedIndex = -1;
            }
            else
            {
                // Inserimento

                // Refresh lista registri
                this.FillListRF();

                // Predisposizione per un nuovo inserimento
                this.SetInsertMode();
            }
        }*/

        /// <summary>
        /// Salvataggio dati
        /// </summary>
        /// <returns></returns>
        private void Save()
        {
            SAAdminTool.DocsPaWR.OrgRegistro registro = new SAAdminTool.DocsPaWR.OrgRegistro();
            SAAdminTool.DocsPaWR.ValidationResultInfo result = null;
            bool insertMode = this.OnInsertMode();

            this.RefreshRFFromUI(ref registro, insertMode);
            if (ddl_caselle.SelectedItem != null)
                this.RefreshCasellaFromUI(ddl_caselle.SelectedItem.Value);
            this.SaveMailPrincInRegistro(ref registro);
            
            if (insertMode)
            {
                result = this.InsertRF(ref registro);
                if (result.Value && ((Caselle != null && Caselle.Length > 0) || ws.IsEnabledInteropInterna()))
                    result = SAAdminTool.utils.MultiCasellaManager.InsertMailRegistro(registro.IDRegistro, Caselle, ws.IsEnabledInteropInterna());
                if (result.Value)
                {
                    txt_indirizzo.Enabled = true;
                    img_aggiungiCasella.Enabled = true;
                    ddl_caselle.Enabled = true;
                }
            }
            else
            {
                result = this.UpdateRF(ref registro);
                if (result.Value && Caselle != null && Caselle.Length > 0)
                    result = SAAdminTool.utils.MultiCasellaManager.UpdateMailRegistro(registro.IDRegistro, Caselle);
                if (result.Value && this.GestioneRubricaComuneAbilitata)
                {
                    // verifico che l'RF sia stato già precedentemente inviato alla rubrica comune
                    SAAdminTool.AdminTool.Manager.SessionManager sessionMng = new SAAdminTool.AdminTool.Manager.SessionManager();
                    RubricaComune.Proxy.Elementi.ElementoRubrica elemento = SAAdminTool.RubricaComune.RubricaServices.GetElementoRubricaRF(sessionMng.getUserAmmSession(), this.CurrentIDRF);
                    if (elemento != null)
                    {
                        string msg = "alert('Attenzione, l\\'RF è già stato pubblicato in rubrica comune; è necessario ripetere la pubblicazione!');";
                        ScriptManager.RegisterStartupScript(UpdatePanelGridRF, UpdatePanelGridRF.GetType(), "msgPublication", msg, true);
                    }
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
                dg_RF.SelectedIndex = -1;
            }
            else
            {
                // Inserimento

                // Refresh lista registri
                this.FillListRF();

                // Predisposizione per un nuovo inserimento
                this.SetInsertMode();
                ddl_caselle.Items.Clear();
                this.ClearParametri();
            }

            this.RubricaComune1.IdElemento = this.CurrentIDRF;
        }

        /// <summary>
        /// Rimozione dati controlli UI
        /// </summary>
        private void ClearData()
        {
            this.CurrentIDRF = string.Empty;

            txt_codice.Text = string.Empty;
            lbl_cod.Text = string.Empty;
            txt_descrizione.Text = string.Empty;
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
            txt_note.Text = string.Empty;
            Chk_sslImap.Checked = false;
            ChkBoxPopSSl.Checked = false;
        }

        /// <summary>
        /// Predisposizione per l'inserimento di un nuovo registro
        /// </summary>
        private void SetInsertMode()
        {
            //visibilità informazioni
            pnl_info.Visible = true;
            btn_aggiungi.Text = "Crea RF";
       
            txt_codice.Visible = true;
            lbl_cod.Visible = false;

            // Rimozione dati controlli UI
            this.ClearData();

            ChkDisabilitato.Checked = false;
            SetFocus(txt_codice);

            dg_RF.SelectedIndex = -1;

            CaricaComboRegistri();

            // In modalità di inserimento non è possibile inviare l'RF alla rubrica comune
            this.RubricaComune1.Visible = false;
        
        }

        /// <summary>
        /// Aggiornamento elemento griglia corrente
        /// </summary>
        /// <param name="registro"></param>
        private void RefreshGridItem(SAAdminTool.DocsPaWR.OrgRegistro registro)
        {
            DataGridItem item = this.dg_RF.SelectedItem;

            if (item != null)
            {
                item.Cells[GRID_COL_DESCRIZIONE].Text = registro.Descrizione;
                item.Cells[GRID_COL_EMAIL].Text = registro.Mail.Email;
                if (registro.rfDisabled == "0")
                {
                    item.ForeColor = Color.Black;
                    item.Font.Bold = false;
                    item.Font.Strikeout = false;
                }
                else
                {
                    item.ForeColor = Color.Red;
                    item.Font.Bold = true;
                    item.Font.Strikeout = true;
                }
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
            SAAdminTool.DocsPaWR.OrgRegistro registro = new SAAdminTool.DocsPaWR.OrgRegistro();
            this.RefreshRFFromUI(ref registro, false);

            SAAdminTool.DocsPaWR.ValidationResultInfo result = null;

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
                    result = SAAdminTool.utils.MultiCasellaManager.DeleteMailRegistro(registro.IDRegistro, string.Empty);

                if (!result.Value)
                {
                    this.ShowValidationMessage(result);
                }
                else
                {
                    //this.txtCommandPending.Value=string.Empty;

                    this.FillListRF();

                    pnl_info.Visible = false;

                    this.ClearData();
                    this.ddl_caselle.Items.Clear();
                    this.ClearParametri();
                    dg_RF.SelectedIndex = -1;
                }
            }
            else
            {
                this.ShowValidationMessage(result);
            }
            pnl_info.Visible = false;
        }

        /// <summary>
        /// Handler cancellazione elemento griglia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteItemCommand(object sender, DataGridCommandEventArgs e)
        {
            this.dg_RF.SelectedIndex = e.Item.ItemIndex;
            string idRegistro = this.dg_RF.SelectedItem.Cells[GRID_COL_ID].Text;
            this.CurrentIDRF = idRegistro;

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
            
            this._businessRuleControls.Add("CODICE_RF", this.txt_codice);
            this._businessRuleControls.Add("DESCRIZIONE_RF", this.txt_descrizione);
            this._businessRuleControls.Add("AOO_COLLEGATA_RF", this.ddl_registri);
            this._businessRuleControls.Add("CONTAIN_OGGETTI", this.txt_descrizione);
            this._businessRuleControls.Add("CONTAIN_CORRISPONDENTI", this.txt_descrizione);
        }

        private Control GetBusinessRuleControl(string idBusinessRule)
        {
            return this._businessRuleControls[idBusinessRule] as Control;
        }

        /// <summary>
        /// Visualizzazione messaggi di validazione
        /// </summary>
        /// <param name="validationResult"></param>
        private void ShowValidationMessage(SAAdminTool.DocsPaWR.ValidationResultInfo validationResult)
        {
            // Visualizzazione delle regole di business non valide
            bool warningMessage;
            Control firstInvalidControl;

            string validationMessage = this.GetValidationMessage(validationResult, out firstInvalidControl, out warningMessage);

            this.RegisterClientScript("ShowValidationMessage", "ShowValidationMessage('" + validationMessage + "'," + warningMessage.ToString().ToLower() + ");");

            if (firstInvalidControl != null)
                this.SetFocus(firstInvalidControl);
        }

        /// <summary>
        /// Inserimento di un nuovo RF
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        private SAAdminTool.DocsPaWR.ValidationResultInfo InsertRF(ref SAAdminTool.DocsPaWR.OrgRegistro registro)
        {
            SAAdminTool.AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();
            return ws.AmmInsertRegistro(ref registro, sessionManager.getUserAmmSession());
        }

        /// <summary>
        /// Aggiornamento RF
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        private SAAdminTool.DocsPaWR.ValidationResultInfo UpdateRF(ref SAAdminTool.DocsPaWR.OrgRegistro registro)
        {
          
            return ws.AmmUpdateRegistro(ref registro);
        }

        /// <summary>
        /// Cancellazione registro
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        private SAAdminTool.DocsPaWR.ValidationResultInfo DeleteRegistro(ref SAAdminTool.DocsPaWR.OrgRegistro registro)
        {
            return ws.AmmDeleteRegistro(ref registro);
        }

        /// <summary>
        /// Verifica se un registro può essere cancellato
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        private SAAdminTool.DocsPaWR.ValidationResultInfo CanDeleteRegistro(SAAdminTool.DocsPaWR.OrgRegistro registro)
        {
            return ws.AmmCanDeleteRegistro(registro);
        }

        private string GetValidationMessage(SAAdminTool.DocsPaWR.ValidationResultInfo validationResult,
                                            out Control firstInvalidControl,
                                            out bool warningMessage)
        {
            string retValue = string.Empty;
            bool errorMessage = false;
            firstInvalidControl = null;

            foreach (SAAdminTool.DocsPaWR.BrokenRule rule in validationResult.BrokenRules)
            {
                if (!errorMessage && rule.Level == SAAdminTool.DocsPaWR.BrokenRuleLevelEnum.Error)
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
            return (btn_aggiungi.Text.Equals("Crea RF"));
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

        private void AlertJS(string msg)
        {
            if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
            {
                string scriptString = "<SCRIPT>alert('" + msg.Replace("'", "\\'") + "');</SCRIPT>";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alertJavaScript", scriptString, false);
            }
        }

        

        #endregion

        protected void brn_RubricaRuolo_Click(object sender, ImageClickEventArgs e)
        {
            RegisterClientScript("apriRubrica", "_ApriRubricaRuolo();");
        }
        #region commentato
        //protected void insertUtRuoloRegistroInSession(string idRegistro)
        //{
        //    if (Session["AMMDATASET"] == null)
        //    {
        //        RegisterStartupScript("NoModTrasm", "<script>alert('Attenzione selezionare un\\'amministrazione !'); document.location = '../Gestione_Homepage/Home.aspx';</script>");
        //        return;
        //    }
        //    string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
        //    string codiceAmministrazione = amministrazione[0];
        //    string idAmministrazione = ws.getIdAmmByCod(codiceAmministrazione);


        //    SAAdminTool.DocsPaWR.Utente ut = new SAAdminTool.DocsPaWR.Utente();
        //    ut.codiceAmm = codiceAmministrazione;
        //    ut.idAmministrazione = idAmministrazione;
        //    ut.tipoIE = "I";
        //    ut.idRegistro = idRegistro;
        //    Session.Add("userData", ut);

        //    SAAdminTool.DocsPaWR.Ruolo rl = new SAAdminTool.DocsPaWR.Ruolo();
        //    rl.codiceAmm = codiceAmministrazione;
        //    rl.idAmministrazione = idAmministrazione;
        //    rl.tipoIE = "I";
        //    rl.idRegistro = idRegistro;
        //    rl.systemId = idAmministrazione;
        //    rl.uo = new SAAdminTool.DocsPaWR.UnitaOrganizzativa();
        //    rl.uo.codiceRubrica = codiceAmministrazione;
        //    Session.Add("userRuolo", rl);

        //    SAAdminTool.DocsPaWR.Registro reg = new SAAdminTool.DocsPaWR.Registro();
        //    reg = ws.GetRegistroBySistemId(idRegistro);
        //    Session.Add("userRegistro", reg);
        //}
        #endregion
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

        public void CaricaComboRegistri()
        {
            try
            {
                this.ddl_registri.Items.Clear();

                SAAdminTool.DocsPaWR.OrgRegistro[] registri = this.GetRegistri_RF_Amministrazione("0");

                ListItem it = new ListItem("", "");
                this.ddl_registri.Items.Add(it);

                if (registri.Length > 0)
                {
                    foreach (SAAdminTool.DocsPaWR.OrgRegistro registro in registri)
                    {
                        ListItem item = new ListItem(registro.Codice, registro.IDRegistro);
                        this.ddl_registri.Items.Add(item);
                    }
                }

            }
            catch(Exception e)
            {
                logger.Debug("errore - caricdaComboRegistri: " + e.Message);
                this.ShowErrorMessage("Si è verificato un errore durante il reperimento dati dei registri.");
            }
        }

        private void ShowErrorMessage(string errorMessage)
        {
            this.RegisterClientScript("ErrorMessage", "alert('" + errorMessage.Replace("'", "\\'") + "')");
        }

        protected void dg_RF_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {


                string disabled = ((TableCell)e.Item.Cells[GRID_COL_DISABLED]).Text;

                if (disabled != null && disabled == "1")
                {

                    e.Item.ForeColor = Color.Red;
                    e.Item.Font.Bold = true;
                    e.Item.Font.Strikeout = true;
                }
            }
        }

        protected void btn_test_Click(object sender, EventArgs e)
        {
            SAAdminTool.DocsPaWR.OrgRegistro registro = this.GetRegistro(this.CurrentIDRF);


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

        /// <summary>
        /// Verifica se la rubrica comune è abilitata
        /// </summary>
        protected bool GestioneRubricaComuneAbilitata
        {
            get
            {
                SAAdminTool.AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();
                SAAdminTool.DocsPaWR.ConfigurazioniRubricaComune config = SAAdminTool.RubricaComune.Configurazioni.GetConfigurazioni(sessionManager.getUserAmmSession());
                return config.GestioneAbilitata;
            }
        }

        #region Multi casella

        /// <summary>
        /// Add casella/e di posta nella lista
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void img_aggiungiCasella_Click(Object sender, ImageClickEventArgs e)
        { 
            //verifico che l'indirizzo non sia vuoto
            if(string.IsNullOrEmpty(txt_indirizzo.Text))
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

            string [] listaCaselle = txt_indirizzo.Text.Split(';'); //split delle caselle immesse dall'utente

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
            if(hashT.Count > 0)
            {
                string [] listaCasellePresenti = new string[hashT.Count];
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
                foreach(string c in listaCasellePresenti)
                    listCasellePresenti.Add(c);
            }
            SAAdminTool.DocsPaWR.CasellaRegistro [] newCaselle = new SAAdminTool.DocsPaWR.CasellaRegistro[listaCaselle.Length - listCasellePresenti.Count];
            int indice = 0;

            //inserisco le nuove caselle nel DB
            for(int i=0; i<listaCaselle.Length;++i)
            {
                if (!listCasellePresenti.Contains(listaCaselle[i]))
                {
                    newCaselle[indice] = new SAAdminTool.DocsPaWR.CasellaRegistro();
                    newCaselle[indice].EmailRegistro = listaCaselle[i];
                    newCaselle[indice].RicevutaPEC = "-";
                    ++indice;
                }
            }
            if (newCaselle != null && newCaselle.Length > 0 && (!string.IsNullOrEmpty(this.CurrentIDRF)))
            {
                SAAdminTool.DocsPaWR.ValidationResultInfo res = SAAdminTool.utils.MultiCasellaManager.InsertMailRegistro(this.CurrentIDRF, newCaselle, false);
                //associo i diritti sulla nuova casella a tutti i ruoli dell'RF
                if (res.Value)
                {
                    Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                    //Cerco solo i ruoli della AOO COLLEGATA: idReg 
                    theManager.GetListaRuoliAOO(this.CurrentIDRF);
                    if (theManager.getListaRuoliAOO() != null && theManager.getListaRuoliAOO().Count > 0)
                    {
                        System.Collections.Generic.List<SAAdminTool.DocsPaWR.RightRuoloMailRegistro> rightRuoloMailRegistro = new System.Collections.Generic.List<SAAdminTool.DocsPaWR.RightRuoloMailRegistro>();
                        foreach (SAAdminTool.DocsPaWR.OrgRuolo ruolo in theManager.getListaRuoliAOO())
                        {
                            foreach (SAAdminTool.DocsPaWR.CasellaRegistro c in newCaselle)
                            {
                                //di default imposto la visibilità del ruolo sulla nuova mail a 0(nessuna diritto)
                                // l'utente modifica la visibilità da organigramma
                                SAAdminTool.DocsPaWR.RightRuoloMailRegistro right = new SAAdminTool.DocsPaWR.RightRuoloMailRegistro
                                {
                                    IdRegistro = this.CurrentIDRF,
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

                    //load informazioni sulle caselle di posta(le metto in sessione)
                    if (!OnInsertMode())
                        SetCaselleRegistro(CurrentIDRF);
                    //salvo in sessione i parametri sulla casella selezionata al momento del nuovo inserimento
                    if (!string.IsNullOrEmpty(ddl_caselle.SelectedValue))
                        RefreshCasellaFromUI(ddl_caselle.SelectedValue);
                    //rendo visibile nella ddl la prima delle nuove caselle inserite
                    ddl_caselle.SelectedValue = newCaselle[0].EmailRegistro;
                    PrevCasella = ddl_caselle.SelectedValue;
                    ClearParametri();
                    if (newCaselle.Length < Caselle.Length)
                    {
                        cbx_casellaPrincipale.Checked = false;
                        cbx_casellaPrincipale.Enabled = true;
                    }
                }
                if (res.Value == false)
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningCreazioneCaselle", "<script>alert('Errore nelle creazione delle caselle di posta!');</script>", false);
            }
            else if (newCaselle.Length > 0) //se siamo in creazione nuovo rf salvo semplicemente in sessione le niove caselle(per salvarle devo prima terminare la creazione dell'RF)
            {
                int countCaselleEsistenti = Caselle != null ? Caselle.Length : 0;
                SAAdminTool.DocsPaWR.CasellaRegistro[] c = new SAAdminTool.DocsPaWR.CasellaRegistro[newCaselle.Length + countCaselleEsistenti];
                int i = 0;
                if (Caselle != null)
                {
                    foreach (SAAdminTool.DocsPaWR.CasellaRegistro oldC in Caselle)
                    {
                        c[i] = oldC;
                        ++i;
                    }
                }
                foreach (SAAdminTool.DocsPaWR.CasellaRegistro newC in newCaselle)
                {
                    c[i] = newC;
                    ++i;
                }
                Caselle = c;
                //salvo in sessione i parametri sulla casella selezionata al momento del nuovo inserimento
                if (!string.IsNullOrEmpty(ddl_caselle.SelectedValue))
                    RefreshCasellaFromUI(ddl_caselle.SelectedValue);
                //rendo visibile nella ddl la prima delle nuove caselle inserite
                ddl_caselle.SelectedValue = newCaselle[0].EmailRegistro;
                ClearParametri();
                //imposto come ultima casella selezionata quella correntemente visualizzata in ddl
                PrevCasella = ddl_caselle.SelectedValue;
            }
            pnl_info.Style.Remove("background");
            pnl_info.Style.Remove("filter");
            pnl_info.Style.Remove("opacity");
            if (!SAAdminTool.utils.MultiCasellaManager.IsEnabledMultiMail(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3")))
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
            foreach (SAAdminTool.DocsPaWR.CasellaRegistro c in Caselle)
            {
                if (c.EmailRegistro.Equals(PrevCasella))
                {
                    if (CheckUpdParamCasella())
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
            foreach (SAAdminTool.DocsPaWR.CasellaRegistro c in Caselle)
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
            foreach (SAAdminTool.DocsPaWR.CasellaRegistro c in Caselle)
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

            //clean campo Descrizione Casella
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
                foreach (SAAdminTool.DocsPaWR.CasellaRegistro c in Caselle)
                {
                    if (!string.IsNullOrEmpty(c.Principale))
                    {
                        c.Principale = string.Empty;
                        break;
                    }
                }
                //imposto la casella correntemente selezionata come principale
                foreach (SAAdminTool.DocsPaWR.CasellaRegistro c in Caselle)
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
        private void RefreshCasella(SAAdminTool.DocsPaWR.CasellaRegistro casella)  
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
                ddl_posta.SelectedIndex = 0;
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
            Caselle = ws.AmmGetMailRegistro(idRegistro);
            if (Caselle != null)
            {
                //popola la ddl caselle di posta
                foreach (SAAdminTool.DocsPaWR.CasellaRegistro c in Caselle)
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
            if (!SAAdminTool.utils.MultiCasellaManager.IsEnabledMultiMail(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3")))
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
        /// Elimina le informazioni sulle caselle di posta associate all'RF corrente dalla sessione
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
        private SAAdminTool.DocsPaWR.CasellaRegistro[] Caselle
        {
            get{
                string key = "CASELLE_POSTA_REGISTRO";
                return  (SAAdminTool.DocsPaWR.CasellaRegistro[]) HttpContext.Current.Session[key];
            }
            set{
                string key = "CASELLE_POSTA_REGISTRO";
                if(HttpContext.Current.Session[key] == null)
                    HttpContext.Current.Session.Add(key,value);
                else
                    HttpContext.Current.Session[key] = value;
            }
        }

        /// <summary>
        /// Indirizzo mail selezionato precedentemente al change della ddl
        /// </summary>
        private object PrevCasella
        {
            get{
                string key = "CASELLA_PRECEDENTE";
                return HttpContext.Current.Session[key];
            } 
            set{
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
            //Verifico che i campi siano correttamente settati
            bool controlloPWD = false;
            bool controlloPWD_SMPT = false;
            bool controlloCodice = true;
            bool errore = false;

            switch (ddl_posta.SelectedValue)
            {

                case "POP":
                    {
                        if (txt_pop.Text.Equals(string.Empty))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ParamPop", "<script>alert('Attenzione, Non hai inserito il parametro per il POP');</script>", false);
                            this.SetFocus(this.txt_pop);
                            errore = true;
                        }
                        if (txt_portapop.Text.Equals(string.Empty))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ParamNumPorta", "<script>alert('Attenzione, Non hai inserito il numero di porta');</script>", false);
                            this.SetFocus(this.txt_portapop);
                            errore = true;
                        }

                        break;
                    }
                case "IMAP":
                    {
                        if (txt_imap.Text.Equals(string.Empty))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ParamImap", "<script>alert('Attenzione, Non hai inserito il parametro per l'' IMAP');</script>", false);
                            this.SetFocus(this.txt_imap);
                            errore = true;
                        }

                        if (txt_portaImap.Text.Equals(string.Empty))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ParamPortaImap", "<script>alert('Attenzione, Non hai inserito il numero di porta');</script>", false);
                            this.SetFocus(this.txt_portaImap);
                            errore = true;
                        }
                        if (txt_inbox.Text.Equals(string.Empty))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ParamIbox", "<script>alert('Attenzione, Non hai inserito la cartella per inbox della e-mail');</script>", false);
                            this.SetFocus(this.txt_inbox);
                            errore = true;
                        }
                        if (txt_mailElaborate.Text.Equals(string.Empty))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ParamCartella", "<script>alert('Attenzione, Non hai inserito la cartella per le mail elaborate');</script>", false);
                            this.SetFocus(this.txt_mailElaborate);
                            errore = true;
                        }
                        if (txt_mailNonElaborate.Text.Equals(string.Empty))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ParamCartellaNonElaborate", "<script>alert('Attenzione, Non hai inserito la cartella per le mail non elaborate');</script>", false);
                            this.SetFocus(this.txt_mailElaborate);
                            errore = true;
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
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ParamPsw", "<script>alert('Attenzione, la password e la conferma sono differenti');</script>", false);
                    this.SetFocus(this.txt_password);
                    errore = true;
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
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ParamPswSmtp", "<script>alert('Attenzione, la password SMTP e la conferma sono differenti');</script>", false);
                    this.SetFocus(this.txt_pwd_smtp);
                    errore = true;
                }
                else
                    controlloPWD_SMPT = true;
            }
            else
                controlloPWD_SMPT = true;

            if (OnInsertMode())
            {
                if (!this.txt_codice.Text.Contains("'"))
                    controlloCodice = true;
                else
                {
                    controlloCodice = false;
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ParamChrSpeciali", "<script>alert('Attenzione, non inserire caratteri speciali nel codice RF');</script>", false);
                }
            }

           /* if (controlloPWD && controlloPWD_SMPT && controlloCodice)
            {

                if (ViewState["CodAooColl"] != null && ViewState["CodAooColl"].ToString() != ddl_registri.SelectedItem.Text)
                {
                    string msg = "Attenzione, si è deciso di modificare la Aoo collegata, pertanto i ruoli\\nche non vedranno questo registro saranno disassociati automaticamente.\\n\\nSi desidera continuare?";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ShowValidationMessage", "<script>ShowContinueModifyRF('" + msg + "');</script>", false);
                    //msg_confirmModificaRF.Confirm(msg);
                }
            }*/
            return errore;
        }

        /// <summary>
        /// Aggiorna i parametri della casella di posta passata come parametro, in sessione
        /// </summary>
        /// <param name="registro"></param>
        private void RefreshCasellaFromUI(string indirizzoPrevCasella)
        {
            foreach (SAAdminTool.DocsPaWR.CasellaRegistro c in Caselle)
            {
                if (c.EmailRegistro.Equals(indirizzoPrevCasella))
                {
                    c.UserMail = this.txt_utente.Text.Trim();
                    if (!this.txt_password.Text.Trim().Equals(string.Empty))
                        c.PwdMail = this.txt_password.Text.Trim();
                    c.ServerSMTP = this.txt_smtp.Text.Trim();
                    c.ServerPOP = this.txt_pop.Text.Trim();
                    //modiifca
                    c.IboxIMAP = this.txt_inbox.Text.Trim();
                    c.BoxMailElaborate = this.txt_mailElaborate.Text.Trim();
                    c.ServerIMAP = this.txt_imap.Text.Trim();
                    c.TipoConnessione = this.ddl_posta.SelectedValue;
                    c.MailNonElaborate = this.txt_mailNonElaborate.Text.Trim();
                    c.ImapSSL = (this.Chk_sslImap.Checked == true) ? "1" : "0";
                    c.SoloMailPEC = (this.ChkMailPec.Checked == true) ? "1" : "0";
                    // Per gestione pendenti tramite PEC
                    c.MailRicevutePendenti = (this.ChkMailRicevutePendenti.Checked == true) ? "1" : "0";
                    try
                    {
                        if (!this.txt_portasmtp.Text.Equals(""))
                            c.PortaSMTP = Convert.ToInt32(this.txt_portasmtp.Text.Trim());
                        else
                            c.PortaSMTP = 0;

                        if (!this.txt_portapop.Text.Equals(""))
                            c.PortaPOP = Convert.ToInt32(this.txt_portapop.Text.Trim());
                        else
                            c.PortaPOP = 0;

                        if (!this.txt_portaImap.Text.Equals(""))
                            c.PortaIMAP = Convert.ToInt32(this.txt_portaImap.Text.Trim());
                        else
                            c.PortaIMAP = 0;
                    }
                    catch (Exception e)
                    {
                        logger.Debug("errore - refreshRFFromUI: " + e.Message);
                    }

                    c.UserSMTP = this.txt_userid_smtp.Text.Trim();

                    if (!this.txt_pwd_smtp.Text.Trim().Equals(string.Empty))
                        c.PwdSMTP = this.txt_pwd_smtp.Text.Trim();

                    c.PopSSL = (this.ChkBoxPopSSl.Checked == true) ? "1" : "0";
                    c.SmtpSSL = (this.ChkBoxsmtp.Checked == true) ? "1" : "0";
                    c.SmtpSta = (this.ChkBoxsmtpSTA.Checked == true) ? "1" : "0";
                    c.ImapSSL = (this.Chk_sslImap.Checked == true) ? "1" : "0";
                    c.RicevutaPEC = this.ddl_ricevutaPec.SelectedValue;
                    c.Note = this.txt_note.Text;

                    break;
                }
            }
        }

        /// <summary>
        /// Aggiornamento registro con i dati sulla mail principale
        /// /// </summary>
        private void SaveMailPrincInRegistro(ref SAAdminTool.DocsPaWR.OrgRegistro registro)
        {
            if (Caselle != null)
            {
                foreach (SAAdminTool.DocsPaWR.CasellaRegistro c in Caselle)
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
                        logger.Debug("errore - refreshRFFromUI: " + e.Message);
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
            ChkBoxsmtp.Checked = false;
            ddl_posta.Text = string.Empty;
            cbx_casellaPrincipale.Checked = false;
            cbx_casellaPrincipale.Enabled = true;
            ddl_ricevutaPec.ClearSelection();
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
                foreach (SAAdminTool.DocsPaWR.CasellaRegistro c in Caselle)
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
                            if(string.IsNullOrEmpty(CurrentIDRF))
                            {
                                    //1. elimino la mail dalla ddl
                                     ddl_caselle.Items.Remove(c.EmailRegistro);
                                    //2. set prevCasella
                                     PrevCasella = ddl_caselle.SelectedValue;
                                     //3. elimino la mail dalla sessione
                                     if (Caselle.Length - 1 > 0)
                                     {
                                         SAAdminTool.DocsPaWR.CasellaRegistro[] newListCaselle = new SAAdminTool.DocsPaWR.CasellaRegistro[Caselle.Length - 1];
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
                                SAAdminTool.DocsPaWR.CasellaRegistro [] listaCaselleInDB = ws.AmmGetMailRegistro(CurrentIDRF);
                                //se in db la casella che si sta eliminando è salvata come principale, allora avviso l'utente che prima dell'eliminazione
                                //deve salvare la nuova casella di posta principale
                                if(Caselle.Length > 1 && listaCaselleInDB[0] != null && listaCaselleInDB[0].EmailRegistro.Equals(c.EmailRegistro))                                 
                                {
                                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "warningElim2", "<script>alert('Prima di eliminare la casella è necessario salvare la nuova casella principale');</script>", false);
                                    return;
                                }
                                SAAdminTool.DocsPaWR.ValidationResultInfo result= ws.AmmDeleteMailRegistro(CurrentIDRF, c.EmailRegistro);
                                if (result.Value)
                                {
                                    //elimino i diritti sulla nuova casella per tutti i ruoli dell'RF
                                    Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                                    //Cerco solo i ruoli della AOO COLLEGATA: idReg 
                                    theManager.GetListaRuoliAOO(this.CurrentIDRF);
                                    if (theManager.getListaRuoliAOO() != null && theManager.getListaRuoliAOO().Count > 0)
                                    {
                                        foreach (SAAdminTool.DocsPaWR.OrgRuolo ruolo in theManager.getListaRuoliAOO())
                                        {
                                            ws.AmmDelRightMailRegistro(this.CurrentIDRF, ruolo.IDCorrGlobale, c.EmailRegistro);
                                        }
                                    } //end aggiornamento diritti ruoli
                                    SetCaselleRegistro(CurrentIDRF);
                                    if (Caselle.Length > 0)
                                    {
                                        RefreshCasella(Caselle[0]);
                                    }
                                    FillListRF();
                                }
                                else
                                {
                                    this.ShowValidationMessage(result);   
                                }
                            }
                            
                        }
                        break;
                    }
                }
            }
            if (!SAAdminTool.utils.MultiCasellaManager.IsEnabledMultiMail(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3")))
            {
                if (ddl_caselle.Items.Count < 1)
                {
                    this.txt_indirizzo.Enabled = true;
                    this.img_aggiungiCasella.Enabled = true;
                    this.ddl_caselle.Enabled = true;
                }
            }
        }
    #endregion
    }
}
