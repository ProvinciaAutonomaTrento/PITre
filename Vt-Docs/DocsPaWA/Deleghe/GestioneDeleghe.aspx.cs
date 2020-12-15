using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.Web.UI.WebControls;
using System.Globalization;
using DocsPAWA.DocsPaWR;
using System.Collections.Generic;


namespace DocsPAWA.Deleghe
{
    public partial class GestioneDeleghe : DocsPAWA.CssPage
    {
        private string HttpFullPath;
        protected Microsoft.Web.UI.WebControls.TreeView treeViewUO;
        protected System.Web.UI.WebControls.DropDownList chklst_ruoli;
        protected System.Web.UI.WebControls.DropDownList ddl_ora_decorrenza;
        protected System.Web.UI.WebControls.DropDownList ddl_ora_scadenza;
        protected System.Web.UI.WebControls.Panel pnl_nuovaDelega;
        protected System.Web.UI.WebControls.Label lbl_stato;
        protected Utilities.MessageBox msg_chiudi;
        protected Utilities.MessageBox msg_DelegaPermanente;
        private string MODELLI_DELEGA_RB_VALUE = "modelliDelega";
        private OrganigrammaHandler orgDelegheHandler;
        private OrganigrammaHandler orgModelliDelegheHandler;

        private Dictionary<StatoModelloDelega, string> StatiModelloDelega
        {
            get
            {
                Dictionary<StatoModelloDelega, string> res = new Dictionary<StatoModelloDelega, string>();
                res.Add(StatoModelloDelega.NON_VALIDO, "Non valido");
                res.Add(StatoModelloDelega.SCADUTO, "Scaduto");
                res.Add(StatoModelloDelega.VALIDO, "Valido");
                return res;
            }
        }
        private bool SHOW_MODELLI_DELEGA
        {
            get
            {
                string valoreChiave =utils.InitConfigurationKeys.GetValue("0","FE_MODELLI_DELEGA");
                return "1".Equals(valoreChiave);
            }
        }

        private int NUM_OPZIONI_RB
        {
            get
            {
                if (SHOW_MODELLI_DELEGA)
                {
                    return 3;
                }
                else
                {
                    return 2;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["Bookmark"] = "GestioneDeleghe";

            //Immagini per il tree
            HttpFullPath = DocsPAWA.Utils.getHttpFullPath(this);
            this.treeViewUO.SystemImagesPath = HttpFullPath + "/AdminTool/Images/treeimages/";

            setDefaultButton();

            if (!Page.IsPostBack)
            {
                btn_modifica.Visible = false;
                btn_nuova.Visible = false;
                btn_revoca.Visible = false;
                this.mv_deleghe.SetActiveView(this.deleghe_view);
                DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();

                //Caso classico, l'utente non sta esercitando una delega
                if (infoUtente.delegato == null)
                {
                    ////Carica le opzioni di delega: attive, assegnate. 
                    ////La prima è sempre visibile, l'opzione deleghe assegnate è visibile solo
                    ////se l'utente ha attiva la funzionalità "DIRITTO_DELEGA" 
                    if (UserManager.ruoloIsAutorized(this, "DIRITTO_DELEGA") && rb_opzioni.Items.Count < NUM_OPZIONI_RB)
                    {
                        ListItem item = new ListItem();
                        item.Text = "Deleghe assegnate";
                        item.Value = "assegnate";
                        rb_opzioni.Items.Add(item);
                        if (SHOW_MODELLI_DELEGA)
                        {
                            ListItem item1 = new ListItem();
                            item1.Text = "Modelli delega";
                            item1.Value = MODELLI_DELEGA_RB_VALUE;
                            rb_opzioni.Items.Add(item1);
                        }
                    }
                    rb_opzioni.SelectedIndex = 0;
                    InitializeRicercaDeleghe("ricevute");
                    //Recupera l'elenco delle deleghe attive
                    fillDeleghe("ricevute", "A");
                    btn_esercita.Visible = true;
                    //btn_modifica.Visible = false;
                    //btn_nuova.Visible = false;
                    //btn_revoca.Visible = false;
                }
                else
                {
                    //Caso in cui l'utente sta esercitando una delega. L'utente delegato in esercizio
                    //non può a sua volta assegnare deleghe, può solo vedere l'elenco 
                    //delle deleghe attive e scegliere di esercitare o meno una delega
                    rb_opzioni.Visible = false;
                    lbl_stato.Visible = false;
                    ddl_stato.Visible = false;
                    btn_cerca_deleghe.Visible = false;
                    fillDeleghe("esercizio", "");
                    //btn_modifica.Visible = false;
                    //btn_nuova.Visible = false;
                    //btn_revoca.Visible = false;
                }
            }
            else
            {
                orgDelegheHandler.Load();
                orgModelliDelegheHandler.Load();
            }
            //gestione scroll del datagrid
            AdminTool.UserControl.ScrollKeeper skDgTemplate = new AdminTool.UserControl.ScrollKeeper();
            skDgTemplate.WebControl = "DivDGList";
            this.deleghe.Controls.Add(skDgTemplate);
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
            this.orgDelegheHandler = new OrganigrammaHandler(this.treeViewUO, this.lbl_messaggio, this.lbl_avviso, this.ddl_ricTipo, this.hd_returnValueModal, this.btn_find, this.btn_conferma, this.txt_ricDesc,"ApriRisultRic",this);
            this.orgModelliDelegheHandler = new OrganigrammaHandler(this.treeViewUO_modelloDelega, this.lbl_messaggio_modelloDelega, this.lbl_avviso_modelloDelega, this.ddl_ricTipo_modelloDelega, this.hd_returnValueModal_modelloDelega, this.btn_find_modelloDelega, this.btn_conferma_modelloDelega, this.txt_ricDesc_modelloDelega, "ApriRisultRicMD", this);
            //this.ddl_stato.SelectedIndexChanged += new EventHandler(this.ddl_stato_SelectedIndexChanged);
            this.btn_cerca_deleghe.Click += new EventHandler(this.btn_cerca_deleghe_Click);
            this.ddl_modelloDelega.SelectedIndexChanged += new EventHandler(this.ddl_modelloDelega_SelectedIndexChanged);
            this.rb_opzioni.SelectedIndexChanged += new System.EventHandler(this.rb_opzioni_SelectedIndexChanged);
            this.btn_nuova.Click += new EventHandler(btn_nuova_Click);
            this.btn_esercita.Click += new EventHandler(btn_esercita_Click);
            this.btn_chiudi.Click += new EventHandler(btn_chiudi_Click);
            this.btn_revoca.Click += new EventHandler(btn_revoca_Click);
            this.btn_modifica.Click += new EventHandler(btn_modifica_Click);
            this.msg_revoca.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_revoca_GetMessageBoxResponse);
            this.msg_conferma.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_conferma_GetMessageBoxResponse);
            this.btn_conferma.Click += new EventHandler(btn_conferma_Click);
            this.btn_chiudiPnl.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudiPnl_Click);
            this.msg_chiudi.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_chiudi_GetMessageBoxResponse);
            this.msg_DelegaPermanente.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_DelegaPermanente_GetMessageBoxResponse);
            this.chklst_ruoli.SelectedIndexChanged += new System.EventHandler(this.chklst_ruoli_SelectedIndexChanged);
            this.ddl_ricTipo.SelectedIndexChanged += new System.EventHandler(this.ddl_ricTipo_SelectedIndexChanged);
            this.orgDelegheHandler.Init();
            this.orgModelliDelegheHandler.Init();
            this.Load += new System.EventHandler(this.Page_Load);
            this.btn_nuovo_modelloDelega.Click += new EventHandler(btn_nuovo_modelloDelega_Click);
            this.btn_modifica_modelloDelega.Click += new EventHandler(btn_modifica_modelloDelega_Click);
            this.btn_cancella_modelloDelega.Click += new EventHandler(btn_cancella_modelloDelega_Click);
            this.ddl_ricTipo_modelloDelega.SelectedIndexChanged += new System.EventHandler(this.ddl_ricTipo_modelloDelega_SelectedIndexChanged);
            this.btn_chiudiPnl_modelloDelega.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudiPnlMD_Click);
            this.btn_conferma_modelloDelega.Click += new EventHandler(btn_conferma_modelloDelega_Click);
            this.btn_ric_cerca_modelloDelega.Click += new EventHandler(btn_cerca_modelloDelega_Click);
            this.ddl_ruolo_delegante.PreRender += new EventHandler(this.fillToolTipRuoli);
        }
        #endregion

        #region rb_opzioni & ddl_stato
        private void ddl_stato_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                this.pnl_nuovaDelega.Visible = false;
                fillDeleghe(rb_opzioni.SelectedValue, ddl_stato.SelectedValue);
                visualizza_pulsanti();
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }

        private void rb_opzioni_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                this.pnl_nuovaDelega.Visible = false;
                ddl_stato.SelectedIndex = 0; //Nel cambio tra deleghe ricevute/assegnate lo stato viene impostato sempre attivo
                if (MODELLI_DELEGA_RB_VALUE.Equals(rb_opzioni.SelectedValue))
                {
                    InitTabModelliDelega();
                }
                else
                {
                    this.mv_deleghe.SetActiveView(this.deleghe_view);
                    InitializeRicercaDeleghe(rb_opzioni.SelectedValue);
                    fillDeleghe(rb_opzioni.SelectedValue, ddl_stato.SelectedValue);
                }
                visualizza_pulsanti();
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }
        #endregion

        #region ricerca deleghe
        private void InitializeRicercaDeleghe(string tipoDelega)
        {
            if ("assegnate".Equals(tipoDelega))
            {
                this.td_nome_delegato.Visible = true;
                this.td_nome_delegante.Visible = false;
                this.td_ruolo_delegante.Visible = true;
            }
            else
            {
                this.td_nome_delegato.Visible = false;
                this.td_nome_delegante.Visible = true;
                fillComboRuoli(ddl_ruolo_delegante);
                this.td_ruolo_delegante.Visible = false;
            }
        }

        private void btn_cerca_deleghe_Click(object sender, EventArgs e)
        {
            try
            {
                this.pnl_nuovaDelega.Visible = false;
                SearchDelegaInfo searchInfo = new SearchDelegaInfo();
                searchInfo.TipoDelega = this.rb_opzioni.SelectedValue;
                searchInfo.StatoDelega = this.ddl_stato.SelectedValue;
                if ("assegnate".Equals(searchInfo.TipoDelega))
                {
                    searchInfo.NomeDelegato = this.txt_nome_delegato.Text;
                    searchInfo.IdRuoloDelegante = this.ddl_ruolo_delegante.SelectedValue;
                }
                else
                {
                    searchInfo.NomeDelegante = this.txt_nome_delegante.Text;
                }
                searchDeleghe(searchInfo);
                visualizza_pulsanti();
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }
        #endregion

        #region gestione pulsanti

        private void visualizza_pulsanti()
        {
            this.dgDeleghe.Columns[0].Visible = true;
            //DELEGHE ASSEGNATE
            if (rb_opzioni.SelectedIndex == 1)
            {
                btn_nuova.Visible = true;
                btn_nuova.Enabled = true;
                btn_revoca.Visible = true;
                btn_modifica.Visible = true;
                btn_esercita.Visible = false;
                if (this.dgDeleghe.Items.Count == 1)
                {
                    CheckBox chkBox = dgDeleghe.Items[0].Cells[0].FindControl("cbSel") as CheckBox;
                    chkBox.Enabled = true;
                    chkBox.Checked = true;
                    btn_revoca.Enabled = true;
                    btn_modifica.Enabled = true;
                }
                else
                {
                    btn_revoca.Enabled = false;
                    btn_modifica.Enabled = false;
                }

                //Deleghe assegnate scadute
                //if (ddl_stato.SelectedValue.Equals("S"))
                //{
                //    this.dgDeleghe.Columns[0].Visible = false;
                //    btn_revoca.Enabled = false;
                //    btn_modifica.Enabled = false;
                //}
            }
            //DELEGHE RICEVUTE
            else
            {
                btn_esercita.Visible = true;
                btn_nuova.Visible = false;
                btn_revoca.Visible = false;
                btn_modifica.Visible = false;
                //Deleghe ricevute scadute, impostate, tutte (non attive)
                if (!ddl_stato.SelectedValue.Equals("A"))
                {
                    this.dgDeleghe.Columns[0].Visible = false;
                    btn_esercita.Enabled = false;
                }
                else
                {
                    if (this.dgDeleghe.Items.Count == 1)
                    {
                        RadioButton rdb = dgDeleghe.Items[0].Cells[0].FindControl("rbSel") as RadioButton;
                        rdb.Enabled = true;
                        rdb.Checked = true;
                        btn_esercita.Enabled = true;
                    }
                    else
                        btn_esercita.Enabled = false;
                    //this.dgDeleghe.Columns[0].Visible = true;
                }

            }
        }

        private void disabilita_pulsanti()
        {
            btn_nuova.Enabled = false;
            btn_revoca.Enabled = false;
            btn_modifica.Enabled = false;
            btn_esercita.Enabled = false;
        }

        private void btn_nuova_Click(object sender, EventArgs e)
        {
            //inizializza la grafica per il pannello nuova delega
            disabilita_pulsanti();
            disabilitaCheckDataGrid();
            if (SHOW_MODELLI_DELEGA)
            {
                loadComboModelliDelega();
            }
            ddl_ricTipo.SelectedIndex = 0;
            ddl_ricTipo_SelectedIndexChanged(null, null);
            this.orgDelegheHandler.InizializeTree();
            lbl_messaggio.Visible = false;
            lbl_operazione.Text = "Nuova delega";
            this.pnl_nuovaDelega.Visible = true;
            this.ddl_ricTipo.SelectedIndex = 0;
            this.txt_ricCod.Text = "";
            this.txt_ricDesc.Text = "";
            btn_conferma.ToolTip = "Conferma creazione nuova delega";
            setInfoRuolo();
            chklst_ruoli.SelectedIndex = 0;
            this.treeViewUO.AutoPostBack = true;
            this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text = DateTime.Now.Date.ToShortDateString().ToString();
            this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text = "";
            ddl_ora_decorrenza.SelectedIndex = DateTime.Now.Hour;
            ddl_ora_scadenza.SelectedIndex = 0;
            txt_dta_decorrenza.EnableBtnCal = true;

        }

        private void btn_esercita_Click(object sender, EventArgs e)
        {
            if (btn_esercita.Text == "Esercita")
            {
                EsercitaDelega();
            }
            if (btn_esercita.Text == "Dismetti")
            {
                DismettiDelega();
            }
        }

        private void EsercitaDelega()
        {
            try
            {
                int posizione = -1;
                DataGridItem item = null;

                //Ricerca delle delega selezionata che si vuole esercitare
                for (int i = 0; i < dgDeleghe.Items.Count; i++)
                {
                    RadioButton rbSelection = dgDeleghe.Items[i].Cells[0].FindControl("rbSel") as RadioButton;
                    if (rbSelection.Checked)
                        item = this.dgDeleghe.Items[i];
                }

                if (item != null)
                {
                    DocsPAWA.DocsPaWR.InfoUtente infoUtDelegato = UserManager.getInfoUtente();
                    //Imposto nuovo utente (delegante) per esercitare la delega
                    DocsPaWR.UserLogin userLogin = new DocsPAWA.DocsPaWR.UserLogin();
                    DocsPaWR.Utente utente = DelegheManager.getUtenteById(this, item.Cells[7].Text);
                    userLogin.UserName = utente.userId;
                    userLogin.Password = "";
                    userLogin.IdAmministrazione = UserManager.getInfoUtente().idAmministrazione;
                    userLogin.IPAddress = this.Request.UserHostAddress;

                    DocsPaWR.LoginResult loginResult;
                    utente = DelegheManager.EsercitaDelega(this, userLogin, item.Cells[11].Text, item.Cells[9].Text, out loginResult);
                    switch (loginResult)
                    {
                        case DocsPAWA.DocsPaWR.LoginResult.OK:
                            if (utente != null)
                            {
                                utente.urlWA = Utils.getHttpFullPath(this);
                                //Memorizzo le info sul delegato che serviranno nel momento in cui
                                //si dismette la delega
                                UserManager.setDelegato(infoUtDelegato);
                                DocsPaWR.Utente utenteDelegato = UserManager.getUtente();
                                DocsPaWR.Ruolo ruoloDelegato = UserManager.getRuolo();
                                UserManager.setUtenteDelegato(this, utenteDelegato);
                                UserManager.setRuoloDelegato(ruoloDelegato);

                                //Nuovo utente (delegante)
                                UserManager.setRuolo(this, utente.ruoli[0]);
                                UserManager.setUtente(this, utente);
                                Session["ESERCITADELEGA"] = true;
                                string script = "<script>window.open('../GestioneRuolo.aspx','principale','width=410,height=300,scrollbars=NO');window.close();</script>";
                                this.Response.Write(script);
                            }
                            break;
                        case DocsPAWA.DocsPaWR.LoginResult.USER_ALREADY_LOGGED_IN:
                            Response.Write("<script>alert('Impossibile esercitare la delega.\\nL\\' utente delegante ha già una connessione aperta.')</script>");
                            break;
                        case DocsPAWA.DocsPaWR.LoginResult.NO_RUOLI:
                            Response.Write("<script>alert('L\\'utente non può accedere al sistema perché l\\'utente delegante \\nnon è associato a nessun ruolo.')</script>");
                            break;
                        default: 
                            // Application Error
                            Response.Write("<script>alert('Impossibile esercitare la delega. Errore nella procedura.')</script>");
					        break;
                    }
                }
                else
                    Response.Write("<script>alert('Nessuna delega selezionata.')</script>");

            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void DismettiDelega()
        {
            if (DelegheManager.DismettiDelega())
            {
                DocsPaWR.Utente utenteDelegato = UserManager.getUtenteDelegato();
                DocsPaWR.Ruolo ruoloDelegato = UserManager.getRuoloDelegato();

                //Ripristino il vecchio infoUtente (del delegato)
                UserManager.setRuolo(this, utenteDelegato.ruoli[0]);
                UserManager.setUtente(this, utenteDelegato);
                Session.Remove("userDelegato");
                Session.Remove("userDataDelegato");
                Session.Remove("userRuoloDelegato");
                Session["ESERCITADELEGA"] = false;
                string script = "<script>window.open('../GestioneRuolo.aspx','principale','width=410,height=300,scrollbars=NO');window.close();</script>";
                this.Response.Write(script);
            }
        }

        private void btn_chiudi_Click(object sender, EventArgs e)
        {
            RegisterStartupScript("ChiudiNuovaDelega", "<script>window.close()</script>");
        }

        private void btn_revoca_Click(object sender, EventArgs e)
        {
            //messaggio di conferma a seguito del quale il sistema revoca immediatamente le deleghe selezionate
            string messaggio = InitMessageXml.getInstance().getMessage("DELEGA_REVOCA");
            msg_revoca.Confirm(messaggio);
        }

        private void btn_modifica_Click(object sender, EventArgs e)
        {
            this.SetFocus(this.btn_conferma);
            //recupero le informazioni sulla delega che si vuole modificare
            int posizione = verificaSelezioneDeleghe();

            if (posizione >= 0)
            {
                //imposto interfaccia grafica
                disabilita_pulsanti();
                lbl_messaggio.Visible = false;
                lbl_operazione.Text = "Modifica delega";
                btn_conferma.ToolTip = "Conferma modifica delega";
                this.pnl_nuovaDelega.Visible = true;
                this.ddl_ricTipo.SelectedIndex = 0;
                this.txt_ricCod.Text = "";
                this.txt_ricDesc.Text = "";
                //Recupera i ruoli del delegante
                setInfoRuolo();

                DocsPAWA.DocsPaWR.InfoDelega delega = (DocsPAWA.DocsPaWR.InfoDelega)this.ListaDeleghe[posizione];

                //imposto la lista dei ruoli sul ruolo del delegante scelto per la delega
                foreach (ListItem item in chklst_ruoli.Items)
                {
                    if (item.Text == delega.cod_ruolo_delegante)
                    {
                        item.Selected = true;
                        break;
                    }
                }

                //impostazione del tree
                this.orgDelegheHandler.InizializeTree();
                this.treeViewUO.AutoPostBack = true;
                this.orgDelegheHandler.RicercaNodo(delega.id_people_corr_globali + "_" + delega.id_ruolo_delegato, "PC");

                //impostazione data decorrenza 
                this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text = (Convert.ToDateTime(delega.dataDecorrenza).ToShortDateString()).ToString();
                string oraDecorrenza = Convert.ToDateTime(delega.dataDecorrenza).ToShortTimeString();
                if (!string.IsNullOrEmpty(oraDecorrenza))
                {
                    oraDecorrenza = oraDecorrenza.Substring(0, 2);
                    if (oraDecorrenza.EndsWith("."))
                        oraDecorrenza = "0" + oraDecorrenza.Substring(0, 1);
                    ddl_ora_decorrenza.SelectedIndex = Convert.ToInt32(oraDecorrenza);
                }
                else
                    ddl_ora_decorrenza.SelectedIndex = 0;

                //impostazione data scadenza
                if (string.IsNullOrEmpty(delega.dataScadenza))
                {
                    this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text = string.Empty;
                    ddl_ora_scadenza.SelectedIndex = 0;
                }
                else
                {
                    this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text = (Convert.ToDateTime(delega.dataScadenza).ToShortDateString()).ToString();
                    string oraScadenza = "";

                    oraScadenza = Convert.ToDateTime(delega.dataScadenza).ToShortTimeString();

                    if (!string.IsNullOrEmpty(oraScadenza))
                    {
                        oraScadenza = oraScadenza.Substring(0, 2);
                        if (oraScadenza.EndsWith("."))
                            oraScadenza = "0" + oraScadenza.Substring(0, 1);
                        ddl_ora_scadenza.SelectedIndex = Convert.ToInt32(oraScadenza) + 1;
                    }
                    else
                        ddl_ora_scadenza.SelectedIndex = 0;
                }
                //delega attiva (non è possibile modificare la data di decorrenza di una delega attiva)
                if (Convert.ToDateTime(delega.dataDecorrenza) < DateTime.Now && (string.IsNullOrEmpty(delega.dataScadenza) || Convert.ToDateTime(delega.dataScadenza) > DateTime.Now))
                {
                    txt_dta_decorrenza.EnableBtnCal = false;
                }

            }
            else
            {
                Response.Write("<script>alert('Nessuna delega selezionata!');</script>");
                return;
            }
        }

        private void Revoca()
        {
            //Crea la lista delle deleghe da revocare, dopo averle revocate si ricarica
            //l'elenco delle deleghe assegnate
            ArrayList listaDeleghe = new ArrayList();
            for (int i = 0; i < dgDeleghe.Items.Count; i++)
            {
                CheckBox chkSelection = dgDeleghe.Items[i].Cells[0].FindControl("cbSel") as CheckBox;
                if (chkSelection != null && chkSelection.Checked)
                {
                    DocsPAWA.DocsPaWR.InfoDelega delega = (DocsPAWA.DocsPaWR.InfoDelega)this.ListaDeleghe[i];
                    listaDeleghe.Add(delega);
                }
            }
            string msg = string.Empty;
            if (DelegheManager.RevocaDelega(this, (DocsPAWA.DocsPaWR.InfoDelega[])listaDeleghe.ToArray(typeof(DocsPAWA.DocsPaWR.InfoDelega)), out msg))
            {
                fillDeleghe("assegnate", ddl_stato.SelectedValue);
            }
            else
            {
                string messaggio = string.Empty;
                if (string.IsNullOrEmpty(msg))
                    messaggio = "Impossibile revocare le deleghe selezionate";
                else
                    messaggio = msg;
                disabilitaCheckDataGrid();
                Response.Write("<script>alert('" + messaggio + "');</script>");
                return;
            }

        }
        #endregion

        #region DATAGRID
        protected void BindGridAndSelect(string tipoDelega, string stato)
        {
            DataGridItem item;
            string statoDelega;
            DocsPaWR.InfoDelega[] data = this.ListaDeleghe;
            this.dgDeleghe.DataSource = data;
            this.dgDeleghe.DataBind();
            if (data.Length > 0)
            {
                this.dgDeleghe.Visible = true;

                this.lbl_messaggio.Visible = false;
                //DELEGHE ASSEGNATE
                if (tipoDelega.Equals("assegnate"))
                {
                    this.dgDeleghe.Columns[8].Visible = false;
                    this.dgDeleghe.Columns[6].Visible = false;
                    this.dgDeleghe.Columns[4].Visible = true;
                    this.dgDeleghe.Columns[10].Visible = true;
                }
                //DELEGHE RICEVUTE
                if (tipoDelega.Equals("ricevute"))
                {
                    this.dgDeleghe.Columns[4].Visible = false;
                    this.dgDeleghe.Columns[6].Visible = false;
                    this.dgDeleghe.Columns[8].Visible = true;
                    this.dgDeleghe.Columns[10].Visible = true;
                }

                #region grafica elementi datagrid
                for (int i = 0; i < data.Length; i++)
                {
                    CheckBox chkBox = this.dgDeleghe.Items[i].Cells[0].FindControl("cbSel") as CheckBox;
                    chkBox.Visible = false;
                    RadioButton rdButton = this.dgDeleghe.Items[i].Cells[0].FindControl("rbSel") as RadioButton;
                    rdButton.Visible = false;
                    if (tipoDelega.Equals("assegnate"))
                        chkBox.Visible = true;
                    else
                        rdButton.Visible = true;
                    this.dgDeleghe.Items[i].Cells[4].Text = data[i].cod_utente_delegato + "<br>(" + data[i].cod_ruolo_delegato + ")";
                    //data decorrenza
                    this.dgDeleghe.Items[i].Cells[1].Text = Convert.ToDateTime(data[i].dataDecorrenza).ToShortDateString()
                                                            + "<br>" + Convert.ToDateTime(data[i].dataDecorrenza).ToShortTimeString();
                    //data scadenza 
                    if (!string.IsNullOrEmpty(data[i].dataScadenza))
                    {
                        if (Convert.ToDateTime(data[i].dataScadenza) > DateTime.Now)
                            this.dgDeleghe.Items[i].Font.Bold = true;

                        this.dgDeleghe.Items[i].Cells[2].Text = Convert.ToDateTime(data[i].dataScadenza).ToShortDateString()
                                                                + "<br>" + Convert.ToDateTime(data[i].dataScadenza).ToShortTimeString();
                    }
                    else
                        this.dgDeleghe.Items[i].Font.Bold = true;

                    if (data[i].utDelegatoDismesso.Equals("1"))
                    {
                        this.dgDeleghe.Items[i].Font.Strikeout = true;
                        this.dgDeleghe.Items[i].ToolTip = "Utente delegato dismesso";
                    }

                    //if (tipoDelega.Equals("assegnate"))
                    //{
                        statoDelega = "I";
                        if (Convert.ToDateTime(data[i].dataDecorrenza) < DateTime.Now && (string.IsNullOrEmpty(data[i].dataScadenza) || Convert.ToDateTime(data[i].dataScadenza) > DateTime.Now))
                        {
                            statoDelega = "A";
                        }
                        if (!string.IsNullOrEmpty(data[i].dataScadenza) && Convert.ToDateTime(data[i].dataScadenza) < DateTime.Now)
                        {
                            statoDelega = "S";
                        }
                        this.dgDeleghe.Items[i].Cells[15].Text = statoDelega;
                    //}
                }
                #endregion
                if (data.Length == 1)
                {
                    if (tipoDelega.Equals("assegnate"))
                    {
                        CheckBox chkBox2 = this.dgDeleghe.Items[0].Cells[0].FindControl("cbSel") as CheckBox;
                        chkBox2.Checked = true;
                        btn_modifica.Enabled = true;
                        btn_revoca.Enabled = true;
                    }
                    if (tipoDelega.Equals("ricevute"))
                    {
                        if (!stato.Equals("A"))
                        {
                            this.dgDeleghe.Columns[0].Visible = false;
                            btn_esercita.Enabled = false;
                        }
                        else
                        {
                            RadioButton rdb = this.dgDeleghe.Items[0].Cells[0].FindControl("rbSel") as RadioButton;
                            rdb.Checked = true;
                            this.dgDeleghe.Columns[0].Visible = true;
                            btn_esercita.Enabled = true;
                        }

                    }
                    if (tipoDelega.Equals("esercizio"))
                    {
                        this.dgDeleghe.Columns[4].Visible = false;
                        this.dgDeleghe.Columns[6].Visible = false;
                        this.dgDeleghe.Columns[8].Visible = true;
                        this.dgDeleghe.Columns[10].Visible = true;

                        this.btn_esercita.Text = "Dismetti";
                        btn_esercita.Enabled = true;
                        RadioButton rb = dgDeleghe.Items[0].Cells[0].FindControl("rbSel") as RadioButton;
                        rb.Checked = true;

                        this.btn_nuova.Visible = false;
                        this.btn_modifica.Visible = false;
                        this.btn_revoca.Visible = false;
                    }
                }
                else
                {
                    btn_modifica.Enabled = false;
                    btn_revoca.Enabled = false;
                    btn_esercita.Enabled = false;
                }
            }
            else
            {
                this.dgDeleghe.Visible = false;
                this.lbl_messaggio.Visible = true;
                if (tipoDelega == "ricevute")
                    this.lbl_messaggio.Text = "Nessuna delega ricevuta";
                if (tipoDelega == "assegnate")
                    this.lbl_messaggio.Text = "Nessuna delega assegnata";
                if (tipoDelega == "esercizio")
                    this.lbl_messaggio.Text = "Deleghe attive";
                btn_esercita.Enabled = false;
                btn_modifica.Enabled = false;
                btn_revoca.Enabled = false;

            }
            btn_nuova.Enabled = true;
        }

        protected void dgDeleghe_PageIndexChanged(object sender, EventArgs e)
        {
            DataGridPageChangedEventArgs ev = (DataGridPageChangedEventArgs)e;
            SearchDelegaInfo searchInfo = this.SearchDelegheMemento;
            SearchPagingContext pagingContext = new SearchPagingContext();
            pagingContext.PageSize = this.dgDeleghe.PageSize;
            pagingContext.Page = ev.NewPageIndex + 1;
            this.dgDeleghe.CurrentPageIndex = ev.NewPageIndex;
            this.ListaDeleghe = DelegheManager.SearchDeleghe(this, searchInfo, ref pagingContext);
            this.dgDeleghe.VirtualItemCount = pagingContext.RecordCount;
            this.BindGridAndSelect(searchInfo.TipoDelega, searchInfo.StatoDelega);
            //riempimento del form di ricerca a partire da quanto è nel memento
            this.ddl_stato.SelectedValue = this.SearchDelegheMemento.StatoDelega;
            this.txt_nome_delegante.Text = this.SearchDelegheMemento.NomeDelegante;
            this.txt_nome_delegato.Text = this.SearchDelegheMemento.NomeDelegato;
            this.ddl_ruolo_delegante.SelectedValue = this.SearchDelegheMemento.IdRuoloDelegante;
        }

        protected DocsPaWR.InfoDelega[] ListaDeleghe
        {
            get
            {
                if (this.ViewState["ListaDeleghe"] != null)
                    return (DocsPaWR.InfoDelega[])this.ViewState["ListaDeleghe"];
                else
                    return new DocsPaWR.InfoDelega[0];
            }
            set
            {
                this.ViewState["ListaDeleghe"] = value;
            }
        }

        protected DocsPaWR.ModelloDelega[] ListaModelliDelega
        {
            get
            {
                if (this.ViewState["ListaModelliDelega"] != null)
                    return (DocsPaWR.ModelloDelega[])this.ViewState["ListaModelliDelega"];
                else
                    return new DocsPaWR.ModelloDelega[0];
            }
            set
            {
                this.ViewState["ListaModelliDelega"] = value;
            }
        }

        protected SearchDelegaInfo SearchDelegheMemento
        {
            get
            {
                if (this.ViewState["SearchDelegheMemento"] != null)
                    return (SearchDelegaInfo)this.ViewState["SearchDelegheMemento"];
                else
                    return null;
            }
            set
            {
                this.ViewState["SearchDelegheMemento"] = value;
            }
        }

        protected SearchModelloDelegaInfo SearchModelliDelegaMemento
        {
            get
            {
                if (this.ViewState["SearchModelliDelegaMemento"] != null)
                    return (SearchModelloDelegaInfo)this.ViewState["SearchModelliDelegaMemento"];
                else
                    return null;
            }
            set
            {
                this.ViewState["SearchModelliDelegaMemento"] = value;
            }
        }

        //Controllo se sono state selezionate alcune deleghe dall'elenco
        //Nel caso in cui sia stata selezionata solo una delega memorizzo la sua posizione
        //nell'elenco
        private int verificaSelezioneDeleghe()
        {
            int posizione = -1;
            int numDelegheSel = 0;

            for (int i = 0; i < dgDeleghe.Items.Count; i++)
            {
                DataGridItem item = dgDeleghe.Items[i];
                if (rb_opzioni.SelectedIndex != 1)
                {
                    RadioButton rbSelection = item.Cells[0].FindControl("rbSel") as RadioButton;
                    if (rbSelection != null && rbSelection.Checked)
                    {
                        posizione = i;
                        numDelegheSel++;
                    }
                }
                else
                {
                    CheckBox chkSelection = item.Cells[0].FindControl("cbSel") as CheckBox;
                    if (chkSelection != null && chkSelection.Checked)
                    {
                        posizione = i;
                        numDelegheSel++;
                    }
                }
            }
            if (numDelegheSel != 1)
                posizione = -1;
            else
                ViewState.Add("posizione", posizione);
            return posizione;
        }

        protected void cbSel_CheckedChanged(object sender, EventArgs e)
        {
            pnl_nuovaDelega.Visible = false;
            //Se è stata selezionata almeno una delega non ancora scaduta sono attivi 
            //i pulsanti esercita e revoca, il pulsante modifica è invece attivo solo se si seleziona 
            //una e una sola delega attiva
            btn_modifica.Enabled = false;
            btn_revoca.Enabled = false;
            int elemSelModificabili = 0;
            int elemSelezionati = 0;
            for (int i = 0; i < dgDeleghe.Items.Count; i++)
            {
                DataGridItem item = dgDeleghe.Items[i];
                CheckBox chkSelection = item.Cells[0].FindControl("cbSel") as CheckBox;
                if (chkSelection != null && chkSelection.Checked)
                {
                    elemSelezionati++;
                    if (string.IsNullOrEmpty(ListaDeleghe[i].dataScadenza))
                        elemSelModificabili++;
                    else
                        if ((Convert.ToDateTime(ListaDeleghe[i].dataScadenza)).CompareTo(DateTime.Now) > 0)
                            elemSelModificabili++;
                }
                this.dgDeleghe.Items[i].Cells[15].Text = item.Cells[15].Text;
            }
            if (elemSelModificabili > 0)
            {
                btn_revoca.Enabled = true;
            }
            if (elemSelezionati == 1)
            {
                btn_modifica.Enabled = true;
            }
        }


        protected void rbSel_CheckedChanged(object sender, EventArgs e)
        {
            pnl_nuovaDelega.Visible = false;
            btn_esercita.Enabled = true;
        }


        private void disabilitaCheckDataGrid()
        {
            foreach (DataGridItem item in this.dgDeleghe.Items)
            {
                CheckBox chkSelection = item.Cells[0].FindControl("cbSel") as CheckBox;
                if (chkSelection != null && chkSelection.Checked)
                {
                    chkSelection.Checked = false;
                }
            }
        }
        #endregion

        #region MessageBox
        private void msg_revoca_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                Revoca();
            }
        }

        private void msg_DelegaPermanente_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                DocsPAWA.DocsPaWR.InfoDelega delega = (DocsPAWA.DocsPaWR.InfoDelega)(ViewState["delega"]);

                if (DelegheManager.CreaNuovaDelega(this, delega))
                {
                    pnl_nuovaDelega.Visible = false;
                    fillDeleghe("assegnate", ddl_stato.SelectedValue);
                }
            }
        }

        private void msg_conferma_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                Modifica();
            }
        }

        private void msg_chiudi_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Cancel)
            {
                SetFocus(btn_nuova);
                disabilitaCheckDataGrid();
                pnl_nuovaDelega.Visible = false;
                visualizza_pulsanti();
                if (dgDeleghe.Items.Count == 0)
                    lbl_messaggio.Visible = true;
            }
        }
        #endregion

        #region NUOVA E MODIFICA DELEGA
        //carica nella chklst tutti i ruoli del delegante
        private void setInfoRuolo()
        {
            chklst_ruoli.Items.Clear();
            ListItem item;
            DocsPaWR.Utente userHome = UserManager.getUtente(this);
            if (userHome != null)
            {
                if (userHome.ruoli != null)
                {
                    if (userHome.ruoli.Length > 1)
                    {
                        //Inserimento voce "Tutti"
                        item = new ListItem("TUTTI");
                        this.chklst_ruoli.Items.Add(item);
                    }
                    for (int i = 0; i < userHome.ruoli.Length; i++)
                    {
                        item = new ListItem(((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[i]).descrizione.ToString(), ((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[i]).systemId.ToString() + "_" + ((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[i]).uo.systemId);
                        this.chklst_ruoli.Items.Add(item);
                    }
                }
            }
        }

        private void chklst_ruoli_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (lbl_operazione.Text != "Modifica delega")
                this.orgDelegheHandler.RicercaNodo(chklst_ruoli.SelectedValue, "");
        }

        private DocsPAWA.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (DocsPAWA.UserControls.Calendar)this.FindControl(controlId);
        }

        private void btn_conferma_Click(object sender, EventArgs e)
        {
            if (lbl_operazione.Text == "Nuova delega")
            {
                confermaNuova();
            }
            if (lbl_operazione.Text == "Modifica delega")
            {
                confermaModifica();
            }
        }

        private void confermaNuova()
        {
            try
            {


                //Costruzione della data di scadenza nel formato "dd/MM/yyyy HH.mm.ss"
                string dta_scadenza = this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text;
                if (!string.IsNullOrEmpty(dta_scadenza) && dta_scadenza.Length <= 10)
                {
                    if (!string.IsNullOrEmpty(ddl_ora_scadenza.SelectedValue))
                    {
                        string ora = ddl_ora_scadenza.SelectedValue;
                        if (ddl_ora_scadenza.SelectedValue.Length == 1)
                            ora = "0" + ddl_ora_scadenza.SelectedValue;
                        dta_scadenza = dta_scadenza + " " + ora + ".00.00";
                    }
                    else
                        dta_scadenza = dta_scadenza + " " + System.DateTime.Now.Hour + ".00.00";
                }

                //Costruzione della data di decorrenza nel formato "dd/MM/yyyy HH.mm.ss"
                string dta_decorrenza = this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text;
                if (!string.IsNullOrEmpty(dta_decorrenza) && dta_decorrenza.Length <= 10)
                {
                    if (!string.IsNullOrEmpty(ddl_ora_decorrenza.SelectedValue))
                    {
                        string ora = ddl_ora_decorrenza.SelectedValue;
                        if (ddl_ora_decorrenza.SelectedValue.Length == 1)
                            ora = "0" + ddl_ora_decorrenza.SelectedValue;
                        dta_decorrenza = dta_decorrenza + " " + ora + ".00.00";
                    }
                    else
                        dta_decorrenza = dta_decorrenza + " " + System.DateTime.Now.Hour + ".00.00";
                }

                if (System.Globalization.DateTimeFormatInfo.CurrentInfo.TimeSeparator.Contains( ":"))
                {
                    dta_decorrenza = dta_decorrenza.Replace('.', ':');
                    dta_scadenza = dta_scadenza.Replace('.', ':');
                }

                //Verifica correttezza delle date
                //Data di scadenza nulla o vuota: caso delega permanente --> nessun controllo sulla data 
                //di scandenza, mentre invece si deve controllare che la data di decorrenza sia nel formato
                //corretto
                if (string.IsNullOrEmpty(dta_scadenza))
                {
                    if (!Utils.isDate(dta_decorrenza))
                    {
                        Response.Write("<script>alert('Verificare formato data decorrenza!');</script>");
                        return;
                    }
                }
                else
                //non si sta costruendo una delega permanete, quindi occorre controllare anche la data di scadenza
                    if (!Utils.isDate(dta_decorrenza) //la data di decorrenza è in formato corretto
                        || !Utils.isDate(dta_scadenza) // la data di scadenza è in formato corretto
                        || !Utils.verificaIntervalloDate(dta_scadenza, dta_decorrenza)) //verifica che la data di scadenza sia maggiore di quella di decorrenza
                    {
                        Response.Write("<script>alert('Verificare intervallo date!');</script>");
                        return;
                    }

                //Verifica che la data di decorrenza sia odierna
                //
                //if (Utils.verificaIntervalloDate(dta_scadenza, System.DateTime.Now.ToShortDateString()))
                if (!Utils.verificaIntervalloDate(Convert.ToDateTime(dta_decorrenza).ToShortDateString(), System.DateTime.Now.ToShortDateString()))
                {
                    Response.Write("<script>alert('La data di decorrenza non deve essere inferiore alla data corrente!');</script>");
                    return;
                }

                //Verifica che sia stato selezionato un delegato
                OrganigrammaTreeNode TreeNodo;
                TreeNodo = (OrganigrammaTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
                if (TreeNodo.getTipoNodo() != "P")
                {
                    Response.Write("<script>alert('Selezionare il delegato!');</script>");
                    return;
                }
                else
                    if (TreeNodo.getIDPeople() == UserManager.getInfoUtente().idPeople)
                    {
                        Response.Write("<script>alert('Attenzione il delegante e il delegato non possono essere la stessa persona!');</script>");
                        return;
                    }

                //Se tutto è a posto (date, delegato, delegante) si può creare la delega
                CreaDelega();
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        //Creazione di una nuova delega
        private void CreaDelega()
        {
            OrganigrammaTreeNode TreeNodo;
            TreeNodo = (OrganigrammaTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
            DocsPAWA.DocsPaWR.InfoDelega delega = new DocsPAWA.DocsPaWR.InfoDelega();
            if (chklst_ruoli.Items.Count > 1 && chklst_ruoli.SelectedIndex == 0)
            {
                delega.id_ruolo_delegante = "0";
                delega.cod_ruolo_delegante = "TUTTI";
            }
            else
            {
                string[] valoriRuolo = chklst_ruoli.SelectedValue.ToString().Split('_');
                delega.id_ruolo_delegante = valoriRuolo[0];
                delega.cod_ruolo_delegante = chklst_ruoli.SelectedItem.Text;
            }

            delega.id_utente_delegante = UserManager.getInfoUtente().idPeople;
            DocsPaWR.Corrispondente corr = new DocsPaWR.Corrispondente();
            corr = UserManager.getCorrispondenteByIdPeople(this, UserManager.getInfoUtente().idPeople, DocsPaWR.AddressbookTipoUtente.INTERNO);
            delega.cod_utente_delegante = corr.codiceRubrica;
            delega.id_ruolo_delegato = TreeNodo.Parent.ToString();
            delega.id_uo_delegato = ((TreeBase)(TreeNodo.Parent)).Parent.ToString();
            string ruoloDelegato = ((OrganigrammaTreeNode)((TreeBase)(TreeNodo.Parent))).Text;
            string[] codRuolo = ruoloDelegato.Split('-');
            delega.cod_ruolo_delegato = codRuolo[1].Substring(1, codRuolo[1].Length - 1);
            delega.id_utente_delegato = TreeNodo.getIDPeople();
            delega.cod_utente_delegato = TreeNodo.getCodiceRubrica();
            string ora;
            if (ddl_ora_decorrenza.SelectedIndex == 0)
                ora = System.DateTime.Now.Hour.ToString();
            else
                ora = ddl_ora_decorrenza.SelectedValue;
            if (ora.Length == 1)
                ora = "0" + ddl_ora_decorrenza.SelectedValue;
            if (Convert.ToDateTime(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text).Date.ToShortDateString().Equals(DateTime.Now.ToShortDateString())
                 && ora.Equals(DateTime.Now.Hour.ToString()))
                delega.dataDecorrenza = Convert.ToDateTime(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text).Date.ToShortDateString() + " " + ora + "." + (DateTime.Now.Minute).ToString() + ".00";
            else
                delega.dataDecorrenza = Convert.ToDateTime(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text).Date.ToShortDateString() + " " + ora + ".00.00";

            if (ddl_ora_scadenza.SelectedIndex == 0)
                ora = System.DateTime.Now.Hour.ToString();
            else
                ora = ddl_ora_scadenza.SelectedValue;
            if (ora.Length == 1)
                ora = "0" + ddl_ora_scadenza.SelectedValue;
            if (!string.IsNullOrEmpty(this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text))
                delega.dataScadenza = Convert.ToDateTime(this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text).Date.ToShortDateString() + " " + ora + ".00.00";
            else
                delega.dataScadenza = string.Empty;


            if (System.Globalization.DateTimeFormatInfo.CurrentInfo.TimeSeparator.Contains(":"))
            {
                delega.dataDecorrenza = delega.dataDecorrenza.Replace('.', ':');
                delega.dataScadenza = delega.dataScadenza.Replace('.', ':');
            }

            //Verifica che non sia stata già creata una delega nello stesso periodo (univocità dell'assegnazione di responsabilità)
            if (DelegheManager.VerificaUnicaDelega(this, delega))
            {
                if (string.IsNullOrEmpty(this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text))
                {
                    ViewState.Add("delega", delega);
                    string messaggio = InitMessageXml.getInstance().getMessage("DELEGA_PERMANENTE");
                    msg_DelegaPermanente.Confirm(messaggio);
                }
                else
                {
                    if (DelegheManager.CreaNuovaDelega(this, delega))
                    {
                        pnl_nuovaDelega.Visible = false;
                        fillDeleghe("assegnate", ddl_stato.SelectedValue);
                        btn_nuova.Enabled = true;
                    }
                    else
                    {
                        Response.Write("<script>alert('Attenzione, impossibile creare la delega. Riprovare più tardi.');</script>");
                        return;
                    }
                }
            }
            else
            {
                Response.Write("<script>alert('Attenzione, impossibile delegare lo stesso ruolo per periodi temporali sovrapposti.');</script>");
                return;
            }

        }

        private void confermaModifica()
        {
            try
            {
                //Costruzione della data di scadenza nel formato "dd/MM/yyyy HH.mm.ss"
                string dta_scadenza = this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text;
                if (!string.IsNullOrEmpty(dta_scadenza) && dta_scadenza.Length <= 10)
                {
                    if (!string.IsNullOrEmpty(ddl_ora_scadenza.SelectedValue))
                    {
                        string ora = ddl_ora_scadenza.SelectedValue;
                        if (ddl_ora_scadenza.SelectedValue.Length == 1)
                            ora = "0" + ddl_ora_scadenza.SelectedValue;
                        dta_scadenza = dta_scadenza + " " + ora + ".00.00";
                    }
                    else
                        dta_scadenza = dta_scadenza + " " + System.DateTime.Now.Hour + ".00.00";
                }

                //Costruzione della data di decorrenza nel formato "dd/MM/yyyy HH.mm.ss"
                string dta_decorrenza = this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text;
                if (!string.IsNullOrEmpty(dta_decorrenza) && dta_decorrenza.Length <= 10)
                {
                    if (!string.IsNullOrEmpty(ddl_ora_decorrenza.SelectedValue))
                    {
                        string ora = ddl_ora_decorrenza.SelectedValue;
                        if (ddl_ora_decorrenza.SelectedValue.Length == 1)
                            ora = "0" + ddl_ora_decorrenza.SelectedValue;
                        dta_decorrenza = dta_decorrenza + " " + ora + ".00.00";
                    }
                    else
                        dta_decorrenza = dta_decorrenza + " " + System.DateTime.Now.Hour + ".00.00";
                }

                //Verifica Date
                if (string.IsNullOrEmpty(dta_scadenza))
                {
                    if (!Utils.isDate(dta_decorrenza))
                    {
                        Response.Write("<script>alert('Verificare formato data decorrenza!');</script>");
                        return;
                    }
                }
                else
                    if (!Utils.isDate(dta_decorrenza)
                        || !Utils.isDate(dta_scadenza)
                        || !Utils.verificaIntervalloDate(dta_scadenza, dta_decorrenza)
         
                        )
                    {
                        Response.Write("<script>alert('Verificare intervallo date!');</script>");
                        return;
                    }

                OrganigrammaTreeNode TreeNodo;
                TreeNodo = (OrganigrammaTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
                if (TreeNodo.getTipoNodo() != "P")
                {
                    Response.Write("<script>alert('Selezionare il delegato!');</script>");
                    return;
                }
                else
                    if (TreeNodo.getIDPeople() == UserManager.getInfoUtente().idPeople)
                    {
                        Response.Write("<script>alert('Attenzione il delegante e il delegato non possono essere la stessa persona!');</script>");
                        return;
                    }

                if (string.IsNullOrEmpty(txt_dta_scadenza.txt_Data.Text))
                {
                    string messaggio = InitMessageXml.getInstance().getMessage("DELEGA_PERMANENTE");
                    msg_conferma.Confirm(messaggio);
                }
                else
                    Modifica();
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void Modifica()
        {
            string[] valoriRuolo = chklst_ruoli.SelectedValue.ToString().Split('_');
            string idUtenteOld = "";
            string idRuoloOld = "";
            string idRuoloDeleganteOld = "";
            string tipoDelega = "";
            string dataScadenzaOld = "";
            string dataDecorrenzaOld = "";

            //recupero le informazioni sulla delega che si vuole modificare
            int posizione = verificaSelezioneDeleghe();
            DocsPAWA.DocsPaWR.InfoDelega delega = (DocsPAWA.DocsPaWR.InfoDelega)this.ListaDeleghe[posizione];
            OrganigrammaTreeNode TreeNodo;
            TreeNodo = (OrganigrammaTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
            if (delega.id_utente_delegato != TreeNodo.getIDPeople())
            {
                idUtenteOld = delega.id_utente_delegato;
                delega.id_utente_delegato = TreeNodo.getIDPeople();
                delega.cod_utente_delegato = TreeNodo.getCodiceRubrica();
            }
            if (delega.id_ruolo_delegato != TreeNodo.Parent.ToString())
            {
                idRuoloOld = delega.id_ruolo_delegato;
                delega.id_ruolo_delegato = TreeNodo.Parent.ToString();
                string ruoloDelegato = ((OrganigrammaTreeNode)((TreeBase)(TreeNodo.Parent))).Text;
                string[] codRuolo = ruoloDelegato.Split('-');
                delega.cod_ruolo_delegato = codRuolo[1];
            }

            idRuoloDeleganteOld = delega.id_ruolo_delegante;
            delega.id_ruolo_delegante = valoriRuolo[0];
            if (valoriRuolo[0].Equals("TUTTI"))
                delega.id_ruolo_delegante = "0";

            if (idRuoloDeleganteOld.Equals(delega.id_ruolo_delegante))
                idRuoloDeleganteOld = "";

            delega.cod_ruolo_delegante = chklst_ruoli.SelectedItem.Text;
            tipoDelega = "I";
            if (Convert.ToDateTime(delega.dataDecorrenza) < DateTime.Now && (string.IsNullOrEmpty(delega.dataScadenza) || Convert.ToDateTime(delega.dataScadenza) > DateTime.Now))
            {
                tipoDelega = "A";
            }
            if (!string.IsNullOrEmpty(delega.dataScadenza) && Convert.ToDateTime(delega.dataScadenza) < DateTime.Now)
            {
                tipoDelega = "S";
            }

            dataScadenzaOld = delega.dataScadenza;
            dataDecorrenzaOld = delega.dataDecorrenza;
            string ora;
            if (ddl_ora_decorrenza.SelectedIndex == 0)
                ora = System.DateTime.Now.Hour.ToString();
            else
                ora = ddl_ora_decorrenza.SelectedValue;
            if (ora.Length == 1)
                ora = "0" + ddl_ora_decorrenza.SelectedValue;


           // delega.dataDecorrenza = Convert.ToDateTime(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text).Date.ToShortDateString() + " " + ora + "." + (DateTime.Now.Minute).ToString() + "." + DateTime.Now.Second.ToString();
            if (Convert.ToDateTime(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text).Date.ToShortDateString().Equals(DateTime.Now.ToShortDateString())
                  && ora.Equals(DateTime.Now.Hour.ToString()))
                delega.dataDecorrenza = Convert.ToDateTime(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text).Date.ToShortDateString() + " " + ora + "." + (DateTime.Now.Minute).ToString() + ".00";
            else
                delega.dataDecorrenza = Convert.ToDateTime(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text).Date.ToShortDateString() + " " + ora + ".00.00";

            
            if (ddl_ora_scadenza.SelectedIndex == 0)
                ora = System.DateTime.Now.Hour.ToString();
            else
                ora = ddl_ora_scadenza.SelectedValue;
            if (ora.Length == 1)
                ora = "0" + ddl_ora_scadenza.SelectedValue;
            if (!string.IsNullOrEmpty(this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text))
                delega.dataScadenza = Convert.ToDateTime(this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text).Date.ToShortDateString() + " " + ora + ".00.00";
            else
                delega.dataScadenza = string.Empty;

            if (DelegheManager.ModificaDelega(this, delega, tipoDelega, idRuoloOld, idUtenteOld, dataScadenzaOld, dataDecorrenzaOld, idRuoloDeleganteOld))
            {
                //chiudo il pannello e carico il datagrid delle deleghe assegnate
                pnl_nuovaDelega.Visible = false;
                fillDeleghe("assegnate", ddl_stato.SelectedValue);
                btn_nuova.Enabled = true;
            }
            else
            {
                Response.Write("<script>alert('Attenzione, impossibile modificare la delega selezionata!');</script>");
                return;
            }

        }

        private void btn_chiudiPnl_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            OrganigrammaTreeNode TreeNodo;
            TreeNodo = (OrganigrammaTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
            if (TreeNodo.getTipoNodo() == "P" && Utils.isDate(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text))
            {
                string messaggio = InitMessageXml.getInstance().getMessage("DELEGA_CHIUDINUOVA");
                msg_chiudi.Confirm(messaggio);
            }
            else
            {
                SetFocus(btn_nuova);
                disabilitaCheckDataGrid();
                pnl_nuovaDelega.Visible = false;
                visualizza_pulsanti();
                if (dgDeleghe.Items.Count == 0)
                    lbl_messaggio.Visible = true;
            }
        }

        #region Tipologia di ricerca
        /// <summary>
        /// DDL per impostare la tipologia della ricerca in organigramma
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddl_ricTipo_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.restoreSearchDefault();
            switch (this.ddl_ricTipo.SelectedValue)
            {
                case "U":
                    this.td_descRicerca.InnerHtml = "Nome UO:";
                    break;
                case "R":
                    this.td_descRicerca.InnerHtml = "Nome ruolo:";
                    break;
                case "PN":
                    this.td_descRicerca.InnerHtml = "Nome utente:";
                    break;
                case "PC":
                    this.td_descRicerca.InnerHtml = "Cognome utente:";
                    break;
            }
        }

        /// <summary>
        /// re-imposta la GUI di default per la tipologia di ricerca
        /// </summary>
        private void restoreSearchDefault()
        {
            this.txt_ricCod.Visible = true;
            this.txt_ricCod.Text = string.Empty;
            this.td_descRicerca.Visible = true;
            this.txt_ricDesc.Visible = true;
            this.txt_ricDesc.Text = string.Empty;
            this.btn_find.Visible = true;
            SetFocus(btn_find);
        }

        private void fillDeleghe(string tipoDelega, string statoDelega)
        {
            SearchDelegaInfo searchInfo = new SearchDelegaInfo();
            searchInfo.StatoDelega = statoDelega;
            searchInfo.TipoDelega = tipoDelega;
            searchDeleghe(searchInfo);
        }

        private void searchDeleghe(SearchDelegaInfo searchInfo)
        {
            this.SearchDelegheMemento = searchInfo;
            SearchPagingContext pagingContext = new SearchPagingContext();
            pagingContext.PageSize = this.dgDeleghe.PageSize;
            pagingContext.Page = 1;
            this.ListaDeleghe = DelegheManager.SearchDeleghe(this, searchInfo, ref pagingContext);
            this.dgDeleghe.VirtualItemCount = pagingContext.RecordCount;
            this.BindGridAndSelect(searchInfo.TipoDelega, searchInfo.StatoDelega);
        }

        #endregion
        #endregion

        #region gestione modelli delega

        private void InitTabModelliDelega()
        {
            this.pnl_nuovoModelloDelega.Visible = false;
            this.mv_deleghe.SetActiveView(this.modelli_deleghe_view);
            this.fillComboRuoli(this.ric_ruoloDel_modelloDelega);
            this.ric_statoMod_modelloDelega.Items.Clear();
            this.ric_statoMod_modelloDelega.Items.Add(new ListItem("TUTTI", ""));
            this.ric_statoMod_modelloDelega.Items.Add(new ListItem(StatiModelloDelega[StatoModelloDelega.NON_VALIDO], "" + StatoModelloDelega.NON_VALIDO));
            this.ric_statoMod_modelloDelega.Items.Add(new ListItem(StatiModelloDelega[StatoModelloDelega.SCADUTO], "" + StatoModelloDelega.SCADUTO));
            this.ric_statoMod_modelloDelega.Items.Add(new ListItem(StatiModelloDelega[StatoModelloDelega.VALIDO], "" + StatoModelloDelega.VALIDO));
            SearchPagingContext pagingContext = new SearchPagingContext();
            pagingContext.PageSize = this.modelliDelegheGrid.PageSize;
            pagingContext.Page = 1;
            SearchModelloDelegaInfo searchInfo=new SearchModelloDelegaInfo();
            searchInfo.StatoModelloDelegaSpec = false;
            this.SearchModelliDelegaMemento= searchInfo;
            this.ListaModelliDelega = DelegheManager.GetModelliDelega(this, this.SearchModelliDelegaMemento, ref pagingContext);
            this.modelliDelegheGrid.VirtualItemCount = pagingContext.RecordCount;
            FillModelliDelega();
            this.btn_cancella_modelloDelega.Enabled = false;
            this.btn_modifica_modelloDelega.Enabled = false;
            this.btn_nuovo_modelloDelega.Enabled = true;
            this.ric_dataInizio_modelloDelega.Text = "";
            this.ric_nomeDel_modelloRicerca.Text = "";
            this.ric_ruoloDel_modelloDelega.SelectedValue = "";
            this.ric_nomeMod_modelloDelega.Text = "";
        }

        private void FillModelliDelega()
        {
            DataGridItem item;
            DocsPaWR.ModelloDelega[] data = this.ListaModelliDelega;
            this.modelliDelegheGrid.DataSource = data;
            this.modelliDelegheGrid.DataBind();
            if (data != null && data.Length > 0)
            {
                this.modelliDelegheGrid.Visible = true;
                for (int i = 0; i < data.Length; i++)
                {
                    this.lbl_noModelloDelega.Visible = false;
                    this.modelliDelegheGrid.Items[i].Cells[1].Text = data[i].Nome;
                    this.modelliDelegheGrid.Items[i].Cells[2].Text = StatiModelloDelega[data[i].Stato];
                    this.modelliDelegheGrid.Items[i].Cells[3].Text = data[i].DescrUtenteDelegato;
                    if (!string.IsNullOrEmpty(data[i].IdRuoloDelegante))
                    {
                        this.modelliDelegheGrid.Items[i].Cells[4].Text = data[i].DescrRuoloDelegante;
                    }
                    else
                    {
                        this.modelliDelegheGrid.Items[i].Cells[4].Text = "TUTTI";
                    }
                    if (data[i].DataInizio.CompareTo(DateTime.MinValue) > 0)
                    {
                        this.modelliDelegheGrid.Items[i].Cells[5].Text = data[i].DataInizio.ToShortDateString()
                                                                + "<br>" + data[i].DataInizio.ToShortTimeString();
                    }
                    else
                    {
                        this.modelliDelegheGrid.Items[i].Cells[5].Text = "-";
                    }
                    if (data[i].Intervallo > 0)
                    {
                        this.modelliDelegheGrid.Items[i].Cells[6].Text = "" + data[i].Intervallo;
                    }
                    else
                    {
                        this.modelliDelegheGrid.Items[i].Cells[6].Text = "-";
                    }
                    if (data[i].DataFine.CompareTo(DateTime.MinValue) > 0)
                    {
                        this.modelliDelegheGrid.Items[i].Cells[7].Text = data[i].DataFine.ToShortDateString()
                                                                + "<br>" + data[i].DataFine.ToShortTimeString();
                    }
                    else
                    {
                        this.modelliDelegheGrid.Items[i].Cells[7].Text = "-";
                    }
                }
            }
            else
            {
                this.modelliDelegheGrid.Visible = false;
                this.lbl_noModelloDelega.Visible = true;
            }
        }

        private void btn_nuovo_modelloDelega_Click(object sender,EventArgs e){
            clearMDPanel();
            this.lbl_titoloPnl_ModelloDelega.Text = "NUOVO MODELLO DELEGA";
            this.pnl_nuovoModelloDelega.Visible = true;
            this.btn_nuovo_modelloDelega.Enabled = false;
        }

        private void btn_modifica_modelloDelega_Click(object sender, EventArgs e)
        {
            clearMDPanel();
            this.lbl_titoloPnl_ModelloDelega.Text = "MODIFICA MODELLO DELEGA";
            this.pnl_nuovoModelloDelega.Visible = true;
            ModelloDelega md=this.getCheckedMD()[0];
            this.txt_nome_modelloDelega.Text = md.Nome;
            this.hd_id_modelloDelega.Value = md.Id;
            if (!string.IsNullOrEmpty(md.IdRuoloDelegante))
            {
                this.chklist_ruolo_modelloDelega.SelectedValue = md.IdRuoloDelegante;
            }
            this.orgModelliDelegheHandler.RicercaUtente(md.IdUtenteDelegato, md.IdRuoloDelegato);
            if(md.DataInizio.CompareTo(DateTime.MinValue)>0){
                string val=""+md.DataInizio.Hour;
                if(val.Length==1) val="0"+val;
                this.ddl_ora_inizio_modelloDelega.SelectedValue = val;
                this.txt_data_inizio_modelloDelega.Text = md.DataInizio.ToString("dd/MM/yyyy");
            }
            if (md.DataFine.CompareTo(DateTime.MinValue) > 0)
            {
                string val = "" + md.DataFine.Hour;
                if (val.Length == 1) val = "0" + val;
                this.ddl_ora_fine_modelloDelega.SelectedValue = val;
                this.txt_data_fine_modelloDelega.Text = md.DataFine.ToString("dd/MM/yyyy");
            }
            if (md.Intervallo > 0)
            {
                this.txt_intervallo_modelloDelega.Text = "" + md.Intervallo;
            }
        }

        private void btn_cancella_modelloDelega_Click(object sender, EventArgs e)
        {
            List<ModelloDelega> checkedMD=getCheckedMD();
            List<string> ids = new List<string>();
            foreach(ModelloDelega temp in checkedMD){
                ids.Add(temp.Id);
            }
            bool res=DelegheManager.CancellaModelliDelega(ids, this);
            if (!res)
            {
                Response.Write("<script>alert('I modelli delega non sono stati cancellati');</script>");
                return;
            }
            this.InitTabModelliDelega();
        }

        private void ddl_ricTipo_modelloDelega_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.restoreSearchMDDefault();
            switch (this.ddl_ricTipo_modelloDelega.SelectedValue)
            {
                case "U":
                    this.td_descRicerca_modelloDelega.InnerHtml = "Nome UO:";
                    break;
                case "R":
                    this.td_descRicerca_modelloDelega.InnerHtml = "Nome ruolo:";
                    break;
                case "PN":
                    this.td_descRicerca_modelloDelega.InnerHtml = "Nome utente:";
                    break;
                case "PC":
                    this.td_descRicerca_modelloDelega.InnerHtml = "Cognome utente:";
                    break;
            }
        }

        private void restoreSearchMDDefault()
        {
            this.txt_ricCod_modelloDelega.Visible = true;
            this.txt_ricCod_modelloDelega.Text = string.Empty;
            this.td_descRicerca_modelloDelega.Visible = true;
            this.txt_ricDesc_modelloDelega.Visible = true;
            this.txt_ricDesc_modelloDelega.Text = string.Empty;
            this.btn_find_modelloDelega.Visible = true;
            SetFocus(btn_find_modelloDelega);
        }

        private void btn_chiudiPnlMD_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
                SetFocus(btn_nuovo_modelloDelega);
                this.pnl_nuovoModelloDelega.Visible = false;
                this.btn_nuovo_modelloDelega.Enabled = true;
        }

        private void btn_conferma_modelloDelega_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dataInizio = buildDateTime(this.GetCalendarControl("txt_data_inizio_modelloDelega").txt_Data.Text, this.ddl_ora_inizio_modelloDelega.Text);
                DateTime dataFine = buildDateTime(this.GetCalendarControl("txt_data_fine_modelloDelega").txt_Data.Text, this.ddl_ora_fine_modelloDelega.Text);
                int intervallo = 0;
                if (!string.IsNullOrEmpty(this.txt_intervallo_modelloDelega.Text))
                {
                    bool resParse = int.TryParse(this.txt_intervallo_modelloDelega.Text, out intervallo);
                    if (!resParse)
                    {
                        Response.Write("<script>alert('Il valore di intervallo deve essere un numero');</script>");
                        return;
                    }
                }
                if(intervallo > 0 && DateTime.MinValue.CompareTo(dataFine)<0)
                {
                    Response.Write("<script>alert('Se si specifica intervallo non va specificata data fine');</script>");
                    return;
                }
                if (intervallo == 0 && DateTime.MinValue.CompareTo(dataFine) == 0)
                {
                    Response.Write("<script>alert('Bisogna specificare intervallo o data fine');</script>");
                    return;
                }
                if (dataFine.CompareTo(DateTime.MinValue)>0 && dataFine.CompareTo(dataInizio) < 0)
                {
                    Response.Write("<script>alert('Data fine deve essere successiva a data inizio');</script>");
                    return;
                }
                //Verifica che sia stato selezionato un delegato
                OrganigrammaTreeNode TreeNodo = this.orgModelliDelegheHandler.getNodeSelected();
                if (TreeNodo.getTipoNodo() != "P")
                {
                    Response.Write("<script>alert('Selezionare il delegato');</script>");
                    return;
                }
                else
                {
                    if (TreeNodo.getIDPeople() == UserManager.getInfoUtente().idPeople)
                    {
                        Response.Write("<script>alert('Il delegante e il delegato non possono essere la stessa persona');</script>");
                        return;
                    }
                }
                if (string.IsNullOrEmpty(this.txt_nome_modelloDelega.Text))
                {
                    Response.Write("<script>alert('Specificare il nome');</script>");
                    return;
                }
                ModelloDelega md = new ModelloDelega();
                md.Id = this.hd_id_modelloDelega.Value;
                md.DataFine = dataFine;
                md.DataInizio = dataInizio;
                md.IdRuoloDelegante = this.chklist_ruolo_modelloDelega.Text;
                md.IdRuoloDelegato = TreeNodo.Parent.ToString();
                md.IdUtenteDelegante = UserManager.getInfoUtente().idPeople;
                md.IdUtenteDelegato = TreeNodo.getIDCorrGlobale();
                md.Intervallo = intervallo;
                md.Nome = this.txt_nome_modelloDelega.Text;
                if (string.IsNullOrEmpty(md.Id))
                {
                    bool response = DelegheManager.CreaNuovoModelloDelega(md, this);
                    if (!response)
                    {
                        Response.Write("<script>alert('Il modello delega non è stato creato');</script>");
                        return;
                    }
                }
                else
                {
                    bool response = DelegheManager.ModificaModelloDelega(md, this);
                    if (!response)
                    {
                        Response.Write("<script>alert('Il modello delega non è stato modificato');</script>");
                        return;
                    }
                }
                this.btn_nuovo_modelloDelega.Enabled = true;
                InitTabModelliDelega();
            }
            catch(Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        protected void btn_cerca_modelloDelega_Click(object sender, EventArgs e)
        {
            SearchModelloDelegaInfo searchInfo = new SearchModelloDelegaInfo();
            searchInfo.IdRuoloDelegante = this.ric_ruoloDel_modelloDelega.SelectedValue;
            searchInfo.NomeDelegato = this.ric_nomeDel_modelloRicerca.Text;
            searchInfo.Nome = this.ric_nomeMod_modelloDelega.Text;
            searchInfo.DataInizio = this.buildDateTime(this.ric_dataInizio_modelloDelega.Text, null);
            if (!string.IsNullOrEmpty(this.ric_statoMod_modelloDelega.SelectedValue))
            {
                searchInfo.StatoModelloDelegaSpec = true;
                searchInfo.StatoModelloDelega=(StatoModelloDelega) Enum.Parse(typeof(StatoModelloDelega), this.ric_statoMod_modelloDelega.SelectedValue);
            }
            this.SearchModelliDelegaMemento = searchInfo;
            SearchPagingContext pagingContext = new SearchPagingContext();
            pagingContext.PageSize = this.modelliDelegheGrid.PageSize;
            pagingContext.Page = 1;
            this.ListaModelliDelega = DelegheManager.GetModelliDelega(this, searchInfo, ref pagingContext);
            this.modelliDelegheGrid.VirtualItemCount = pagingContext.RecordCount;
            this.modelliDelegheGrid.CurrentPageIndex = 0;
            FillModelliDelega();
        }

        protected void cbSelMD_CheckedChanged(object sender, EventArgs e)
        {
            this.pnl_nuovoModelloDelega.Visible = false;
            this.btn_modifica_modelloDelega.Enabled = false;
            this.btn_cancella_modelloDelega.Enabled = false;
            List<ModelloDelega> checkedMD = getCheckedMD();
            if (checkedMD.Count == 1)
            {
                this.btn_modifica_modelloDelega.Enabled = true;
            }
            if (checkedMD.Count > 0)
            {
                this.btn_cancella_modelloDelega.Enabled = true;
            }
        }

        private List<ModelloDelega> getCheckedMD()
        {
            List<ModelloDelega> res=new List<ModelloDelega>();
            for (int i = 0; i < this.modelliDelegheGrid.Items.Count; i++)
            {
                DataGridItem item = modelliDelegheGrid.Items[i];
                CheckBox chkSelection = item.Cells[0].FindControl("cbSelMD") as CheckBox;
                if (chkSelection != null && chkSelection.Checked)
                {
                    res.Add(this.ListaModelliDelega[i]);
                }
            }
            return res;
        }

        private void clearMDPanel()
        {
            this.txt_data_fine_modelloDelega.Text = "";
            this.txt_data_inizio_modelloDelega.Text = "";
            this.txt_intervallo_modelloDelega.Text = "";
            this.ddl_ora_fine_modelloDelega.SelectedIndex = 0;
            this.ddl_ora_inizio_modelloDelega.SelectedIndex = 0;
            this.ddl_ricTipo_modelloDelega.SelectedIndex = 0;
            this.txt_nome_modelloDelega.Text = "";
            this.txt_ricCod_modelloDelega.Text = "";
            this.txt_ricDesc_modelloDelega.Text = "";
            this.hd_id_modelloDelega.Value = "";
            this.chklist_ruolo_modelloDelega.Items.Clear();
            this.orgModelliDelegheHandler.InizializeTree();
            this.fillComboRuoli(this.chklist_ruolo_modelloDelega);
        }

        private void fillComboRuoli(DropDownList ddl)
        {
            ddl.Items.Clear();
            DocsPaWR.Utente userHome = UserManager.getUtente(this);
            if (userHome != null)
            {
                if (userHome.ruoli != null)
                {
                    ListItem item = new ListItem("TUTTI", "");
                    ddl.Items.Add(item);
                    for (int i = 0; i < userHome.ruoli.Length; i++)
                    {
                        ListItem temp = new ListItem(((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[i]).descrizione.ToString(), ((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[i]).systemId.ToString());
                        ddl.Items.Add(temp);
                    }
                }
            }
        }

        private DateTime buildDateTime(string date, string hour)
        {
            if (string.IsNullOrEmpty(date)) return DateTime.MinValue;
            string temp = "";
            if (!string.IsNullOrEmpty(hour))
            {
                temp = date + " " + hour+":00";
            }
            else
            {
                temp = date + " 00:00";
            }
            string[] formats = { "dd/MM/yyyy HH:mm", "dd/M/yyyy HH:mm" };
            return DateTime.ParseExact(temp, formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
        }

        private void loadComboModelliDelega()
        {
            //caricamento dei modelli di delega validi
            tr_ddl_modelloDelega.Visible = true;
            ModelloDelega[] modelliValidi = DelegheManager.GetModelliDelegaValidi(this);
            if (modelliValidi.Length == 0)
            {
                this.ddl_modelloDelega.Enabled = false;
            }
            else
            {
                this.ddl_modelloDelega.Enabled = true;
                this.ddl_modelloDelega.Items.Clear();
                this.ddl_modelloDelega.Items.Add(new ListItem("", ""));
                foreach (ModelloDelega md in modelliValidi)
                {
                    this.ddl_modelloDelega.Items.Add(new ListItem(md.Nome, md.Id));
                }
            }
        }

        public void ddl_modelloDelega_SelectedIndexChanged(object sender, EventArgs e)
        {
            string idModello = this.ddl_modelloDelega.SelectedValue;
            if(!string.IsNullOrEmpty(idModello)){
                ModelloDelega md=DelegheManager.GetModelloDelegaById(idModello,this);
                if(!string.IsNullOrEmpty(md.IdRuoloDelegante)){
                    DocsPaWR.Utente userHome = UserManager.getUtente(this);
                    foreach (Ruolo temp in userHome.ruoli)
                    {
                        if(temp.systemId.Equals(md.IdRuoloDelegante)) this.chklst_ruoli.SelectedValue = temp.systemId.ToString() + "_" + temp.uo.systemId;
                    }
                }
                string oraInizio = ""+md.DataInizioDelega.Hour;
                if (oraInizio.Length == 1) oraInizio = "0" + oraInizio;
                ddl_ora_decorrenza.SelectedValue = oraInizio;
                this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text = md.DataInizioDelega.Date.ToShortDateString().ToString();
                string oraFine = "" + md.DataFineDelega.Hour;
                if (oraFine.Length == 1) oraFine = "0" + oraFine;
                ddl_ora_scadenza.SelectedValue = oraFine;
                this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text = md.DataFineDelega.Date.ToShortDateString().ToString();
                this.orgDelegheHandler.RicercaUtente(md.IdUtenteDelegato, md.IdRuoloDelegato);
            }
        }

        public void mdGrid_PageIndexChanged(object sender, EventArgs e)
        {
            DataGridPageChangedEventArgs ev=(DataGridPageChangedEventArgs) e;
            SearchModelloDelegaInfo searchInfo = this.SearchModelliDelegaMemento;
            SearchPagingContext pagingContext = new SearchPagingContext();
            pagingContext.PageSize = this.modelliDelegheGrid.PageSize;
            pagingContext.Page = ev.NewPageIndex+1;
            this.modelliDelegheGrid.CurrentPageIndex = ev.NewPageIndex;
            this.ListaModelliDelega = DelegheManager.GetModelliDelega(this, searchInfo, ref pagingContext);
            this.modelliDelegheGrid.VirtualItemCount = pagingContext.RecordCount;
            FillModelliDelega();
            if (searchInfo != null)
            {
                this.ric_nomeDel_modelloRicerca.Text = searchInfo.NomeDelegato;
                this.ric_nomeMod_modelloDelega.Text = searchInfo.Nome;
                this.ric_ruoloDel_modelloDelega.SelectedValue = searchInfo.IdRuoloDelegante;
                if (searchInfo.DataInizio.CompareTo(DateTime.MinValue) > 0)
                {
                    this.ric_dataInizio_modelloDelega.Text = searchInfo.DataInizio.ToString("dd/MM/yyyy");
                }
                else
                {
                    this.ric_dataInizio_modelloDelega.Text = "";
                }
                if (searchInfo.StatoModelloDelegaSpec)
                {
                    this.ric_statoMod_modelloDelega.SelectedValue = searchInfo.StatoModelloDelega.ToString();
                }
                else
                {
                    this.ric_statoMod_modelloDelega.SelectedValue = "";
                }
            }
        }
        #endregion

        private void setDefaultButton()
        {
            DocsPAWA.Utils.DefaultButton(this, ref txt_ricDesc, ref btn_conferma);
            DocsPAWA.Utils.DefaultButton(this, ref txt_ricCod, ref btn_conferma);
            //DocsPAWA.Utils.DefaultButton(this, ref treeViewUO, ref btn_conferma);
        }

        private void fillToolTipRuoli(object sender, EventArgs e)
        {
            foreach(ListItem item in ddl_ruolo_delegante.Items)
            {
                  item.Attributes.Add("title", item.Text);
            }
        }

    }
}
