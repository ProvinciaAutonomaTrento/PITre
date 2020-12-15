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
using DocsPAWA.AdminTool.Gestione_Logs;
using DocsPAWA.UserControls;

namespace Amministrazione.Gestione_Logs
{
    public partial class EstrazioneLog : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.Label lbl_position;
        protected string codAmm;
        protected DataSet dataSet;
        protected AmmUtils.WebServiceLink ws;
        protected DocsPAWA.DocsPaWR.InfoUtenteAmministratore _datiAmministratore = new DocsPAWA.DocsPaWR.InfoUtenteAmministratore();
        protected string idAmministrazione;
        private DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
        protected int giorniIOP;
        protected string dataInitIOP;
        protected string dataFineIOP;
        protected string iop_finale;

        #region Init

        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.estrazioneLog_PreRender);
            // this.chk_ruolo.CheckedChanged += new System.EventHandler(this.chk_ruolo_SelectIndexChanged);
            // this.chk_rf.CheckedChanged += new System.EventHandler(this.chk_rf_SelectIndexChanged);
            // this.chk_aoo.CheckedChanged += new System.EventHandler(this.chk_aoo_SelectIndexChanged);
            // this.chk_uo.CheckedChanged += new System.EventHandler(this.chk_uo_SelectIndexChanged);
            this.rbl_ruoli.SelectedIndexChanged += new System.EventHandler(this.rbl_ruoli_SelectedIndexChanged);
            this.ddl_oggetto.SelectedIndexChanged += new System.EventHandler(this.ddl_oggetto_SelectedIndexChanged);
            this.txt_codRuolo.TextChanged += new System.EventHandler(this.txt_codRuolo_TextChanged);
            this.txt_codUo.TextChanged += new System.EventHandler(this.txt_codUo_TextChanged);
            this.eliminaDataDa.Click += new ImageClickEventHandler(cancelDataDA);
            this.eliminaDataA.Click += new ImageClickEventHandler(cancelDataA);
            this.ddl_periodo_ipa.SelectedIndexChanged += new System.EventHandler(this.ddl_periodo_ipa_SelectedIndexChanged);
            this.ddl_ampiezza_ipa.SelectedIndexChanged += new System.EventHandler(this.ddl_ampiezza_ipa_SelectedIndexChanged);
            this.ddl_iagg.SelectedIndexChanged += new System.EventHandler(this.ddl_iagg_SelectedIndexChanged);
            this.ddl_azione.SelectedIndexChanged += new System.EventHandler(this.ddl_azione_SelectedIndexChanged);
            this.ddl_rf.SelectedIndexChanged += new System.EventHandler(this.ddl_rf_SelectedIndexChanged);
            this.ddl_aoo.SelectedIndexChanged += new System.EventHandler(this.ddl_aoo_SelectedIndexChanged);
            this.btn_stampa.Click += new System.EventHandler(this.btn_stampa_Click);
            this.ddl_amministrazioni.SelectedIndexChanged += new System.EventHandler(this.ddl_amministrazioni_SelectedIndexChanged);
            this.chk_sottoposti.CheckedChanged += new System.EventHandler(this.chk_sottoposti_SelectIndexChanged);
            this.txt_numeroUnita.TextChanged += new System.EventHandler(this.txt_numeroUnita_TextChanged);
            this.ddl_tp.SelectedIndexChanged += new System.EventHandler(this.ddl_tp_SelectedIndexChanged);
        }

        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Page.Response.Expires = -1;

            this.codAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");

            ws = new AmmUtils.WebServiceLink();

            if (!IsPostBack)
            {

                this.lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");

                idAmministrazione = wws.getIdAmmByCod(codAmm);

                caricaDataSet();

                caricaComboRf();

                caricaComboRegistri();

                FillListTipiRuolo();

                ddl_azione.Enabled = false;

                setRadioButtonRuoli();

                DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();

                this._datiAmministratore = session.getUserAmmSession();

                DocsPAWA.DocsPaWR.Utente ut = new DocsPAWA.DocsPaWR.Utente();
                //ut.codiceAmm = codiceAmministrazione;
                ut.codiceAmm = codAmm;
                ut.idAmministrazione = idAmministrazione;
                ut.tipoIE = "I";
                ut.systemId = _datiAmministratore.idCorrGlobali;
                ut.idPeople = _datiAmministratore.idPeople;
                ut.userId = _datiAmministratore.userId;
                // ut.idRegistro = this.ddl_registri.SelectedValue.ToString();
                Session.Add("userData", ut);

                DocsPAWA.DocsPaWR.Ruolo rl = new DocsPAWA.DocsPaWR.Ruolo();
                // rl.codiceAmm = codiceAmministrazione;
                rl.idAmministrazione = idAmministrazione;
                rl.tipoIE = "I";
                // rl.idRegistro = this.ddl_registri.SelectedValue.ToString();

                rl.systemId = idAmministrazione;
                rl.uo = new DocsPAWA.DocsPaWR.UnitaOrganizzativa();
                // rl.uo.codiceRubrica = codiceAmministrazione;

                Session.Add("userRuolo", rl);

                DocsPAWA.DocsPaWR.Registro reg = new DocsPAWA.DocsPaWR.Registro();
                //reg.codAmministrazione = codiceAmministrazione;
                reg.idAmministrazione = codAmm;
                //reg.systemId = this.ddl_registri.SelectedValue.ToString();
                // reg.codRegistro = this.ddl_registri.SelectedItem.Text.ToString();
                Session.Add("userRegistro", reg);

                //RUBRICA RUOLO
                string s = "<SCRIPT language='javascript'>document.getElementById('" + this.txt_codRuolo.ID + "').focus()</SCRIPT>";
                if (!this.Page.ClientScript.IsStartupScriptRegistered("focus"))
                {

                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s);

                }

                //RUBRICA UO
                string f = "<SCRIPT language='javascript'>document.getElementById('" + this.txt_codUo.ID + "').focus()</SCRIPT>";
                if (!this.Page.ClientScript.IsStartupScriptRegistered("focus"))
                {
                    //this.RegisterStartupScript("focus", s);
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", f);
                }


                //CARICA DDL AMMINISTRAZIONI
                DocsPAWA.AdminTool.Manager.AmministrazioneManager amm = new DocsPAWA.AdminTool.Manager.AmministrazioneManager();
                amm.ListaAmministrazioni();

                if (amm.getListaAmministrazioni() != null && amm.getListaAmministrazioni().Count > 0)
                {
                    // gestione tipologie di utente amministratore
                    if (_datiAmministratore.tipoAmministratore.Equals("2"))
                    {
                        this.LoadDDLAmministrazioni(amm.getListaAmministrazioni(), _datiAmministratore.idAmministrazione);
                    }
                    else
                    {
                        this.LoadDDLAmministrazioni(amm.getListaAmministrazioni(), null);
                        DocsPAWA.DocsPaWR.InfoAmministrazione firstTime = (DocsPAWA.DocsPaWR.InfoAmministrazione)amm.getListaAmministrazioni()[0];
                    }
                }
                /////

            }

            string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(DocsPAWA.ConfigSettings.KeysENUM.RUBRICA_V2);

            if (use_new_rubrica != "1")
            {
                //NON PIU' UTILIZZATA
            }
            else
            {
                this.btn_Rubrica_Ruolo.Attributes.Add("onclick", "_ApriRubricaRicercaRuoli();");
                this.btn_Rubrica_Uo.Attributes.Add("onclick", "_ApriRubricaRicercaUO();");
            }

            if (IsPostBack)
            {
                abilitaCampi();
            }
        }

        private void estrazioneLog_PreRender(object sender, EventArgs e)
        {

            btn_Rubrica_Ruolo.Attributes["onMouseOver"] = "javascript:ImpostaCursore (1,'" + btn_Rubrica_Ruolo.ClientID + "');";
            btn_Rubrica_Ruolo.Attributes["onMouseOut"] = "javascript:ImpostaCursore (0,'" + btn_Rubrica_Ruolo.ClientID + "');";

            btn_Rubrica_Uo.Attributes["onMouseOver"] = "javascript:ImpostaCursore (1,'" + btn_Rubrica_Uo.ClientID + "');";
            btn_Rubrica_Uo.Attributes["onMouseOut"] = "javascript:ImpostaCursore (0,'" + btn_Rubrica_Uo.ClientID + "');";

            if (!this.Page.IsClientScriptBlockRegistered("imposta_cursore"))
            {
                this.Page.RegisterClientScriptBlock("imposta_cursore",
                    "<script language=\"javascript\">\n" +
                    "function ImpostaCursore (t, ctl)\n{\n" +
                    "document.getElementById(ctl).style.cursor = (t == 0) ? 'default' : 'hand';\n" +
                    "}\n</script>\n");
            }

            //carico il ruolo, se esiste
            DocsPAWA.DocsPaWR.Corrispondente corrRuolo = DocsPAWA.UserManager.getCorrispondenteSelezionatoRuoloAmministrazione(this);
            if (corrRuolo != null)
            {
                DocsPAWA.UserManager.removeCorrispondentiSelezionati(this);
                if (!corrRuolo.codiceRubrica.Equals(""))
                {
                    this.txt_codRuolo.Text = corrRuolo.codiceRubrica;
                    this.txt_descRuolo.Text = DocsPAWA.UserManager.getDecrizioneCorrispondenteSemplice(corrRuolo);
                    this.hd_systemIdCorrRuolo.Value = corrRuolo.systemId;
                    DocsPAWA.UserManager.setCorrispondenteSelezionatoRuoloAmministrazione(this.Page, corrRuolo);
                }
            }

            //carico la UO, se esiste
            DocsPAWA.DocsPaWR.Corrispondente corrUO = DocsPAWA.UserManager.getCorrispondenteSelezionatoUOAmministrazione(this);
            if (corrUO != null)
            {
                DocsPAWA.UserManager.removeCorrispondentiSelezionati(this);
                if (!corrUO.codiceRubrica.Equals(""))
                {
                    this.txt_codUo.Text = corrUO.codiceRubrica;
                    this.txt_descUo.Text = DocsPAWA.UserManager.getDecrizioneCorrispondenteSemplice(corrUO);
                    this.hd_systemIdCorrUO.Value = corrUO.systemId;
                    DocsPAWA.UserManager.setCorrispondenteSelezionatoUOAmministrazione(this.Page, corrUO);
                }
            }
        }


        /// <summary>
        /// carica Combobox Oggetto
        /// </summary>
        public void caricaComboOggetto()
        {
            if (ViewState["LOG"] != null)
            {
                dataSet = new DataSet();
                dataSet = (DataSet)ViewState["LOG"];

                DataTable dt = IniDataSetAppo(dataSet);

                foreach (DataRow riga in dataSet.Tables["LOG"].Rows)
                {
                    DataRow[] righe = dt.Select("oggetto='" + riga[1] + "'");
                    if (righe.Length == 0)
                    {
                        dt.ImportRow(riga);
                    }
                }

                DataView dv = dataSet.Tables["APPO"].DefaultView;
                dv.Sort = "oggetto asc";

                foreach (DataRow riga in dv.Table.Rows)
                {
                    ListItem item = new ListItem(riga[1].ToString(), riga[1].ToString());
                    if (!((this.DisableAmmGestCons() || this.IsConservazionePARER()) && (riga[1].ToString() == "CONSERVAZIONE" || riga[1].ToString() == "AREA_CONSERVAZIONE")))
                        ddl_oggetto.Items.Add(item);
                }
            }
        }

        //MEV CONS 1.3
        /// <summary>
        /// Funzione per la gestione dell'abilitazione/disabilitazione della visualizzazione
        /// dei log conservazione
        /// </summary>
        /// <returns></returns>
        protected bool DisableAmmGestCons()
        {
            bool result = false;

            string PGU_FE_DISABLE_AMM_GEST_CONS_Value = string.Empty;
            PGU_FE_DISABLE_AMM_GEST_CONS_Value = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "PGU_FE_DISABLE_AMM_GEST_CONS");
            result = ((PGU_FE_DISABLE_AMM_GEST_CONS_Value.Equals("0") || string.IsNullOrEmpty(PGU_FE_DISABLE_AMM_GEST_CONS_Value)) ? false : true);

            return result;

        }

        protected bool IsConservazionePARER()
        {
            bool result = false;

            string IS_CONSERVAZIONE_PARER = string.Empty;
            IS_CONSERVAZIONE_PARER = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_WA_CONSERVAZIONE");
            result = ((string.IsNullOrEmpty(IS_CONSERVAZIONE_PARER) || IS_CONSERVAZIONE_PARER.Equals("0")) ? false : true);

            return result;
        }

        /// <summary>
        /// Inizializza il dataset
        /// </summary>
        private DataTable IniDataSetAppo(DataSet dataSet)
        {
            dataSet.Tables.Add("APPO");

            DataColumn dc = new DataColumn("codice");
            dataSet.Tables["APPO"].Columns.Add(dc);

            dc = new DataColumn("oggetto");
            dataSet.Tables["APPO"].Columns.Add(dc);

            dc = new DataColumn("descrizione");
            dataSet.Tables["APPO"].Columns.Add(dc);

            return dataSet.Tables["APPO"];
        }

        /// <summary>
        /// onChange ddl_oggetto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddl_oggetto_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (ddl_oggetto.SelectedItem.Value != "null")
            {

                ddl_azione.DataSource = caricaAzioni(ddl_oggetto.SelectedItem.Value);
                ddl_azione.DataTextField = "descrizione";
                ddl_azione.DataValueField = "codice";
                ddl_azione.DataBind();
                ddl_azione.Enabled = true;
                descrizioneFinale();
            }
            else
            {
                ddl_azione.Items.Clear();
                ddl_azione.Enabled = false;
            }
        }

        public ICollection caricaAzioni(string filtro)
        {
            this.idAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

            DataTable dt = new DataTable();
            DataRow dr;

            dataSet = new DataSet();
            dataSet = (DataSet)ViewState["LOG"];

            dt.Columns.Add(new DataColumn("codice", typeof(string)));
            dt.Columns.Add(new DataColumn("oggetto", typeof(string)));
            dt.Columns.Add(new DataColumn("descrizione", typeof(string)));

            // aggiunge la prima riga con "*"
            dr = dt.NewRow();
            dr[0] = "null";		// campo codice
            dr[1] = filtro;		// campo oggetto
            dr[2] = "*";		// campo descrizione

            dt.Rows.Add(dr);

            foreach (DataRow riga in dataSet.Tables["LOG"].Rows)
            {
                // INTEGRAZIONE PITRE-PARER
                // filtro aggiuntivo per i log di invio in conservazione non associati agli oggetti CONSERVAZIONE ed AREA CONSERVAZIONE
                if (!(this.IsConservazionePARER() && (
                    riga[2].ToString().Equals("Invio in conservazione del documento") ||
                    riga[2].ToString().Equals("Documento inserito in conservazione") ||
                    riga[2].ToString().Equals("Fascicolo inserito in conservazione")
                    )))
                {
                    dr = dt.NewRow();
                    dr[0] = riga[0].ToString(); // campo codice
                    dr[1] = riga[1].ToString(); // campo oggetto
                    dr[2] = riga[2].ToString(); // campo descrizione

                    dt.Rows.Add(dr);
                }
            }

            DataView dv = dt.DefaultView;
            dv.RowFilter = "oggetto = '" + filtro + "'";
            dv.Sort = "descrizione ASC";

            return dv;
        }

        /// <summary>
        /// carica DataSet
        /// </summary>
        public void caricaDataSet()
        {
            try
            {
                IniDataSet();

                XmlDocument xmlDoc = new XmlDocument();
                DataRow row;

                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                string xmlStream = ws.GetXmlLog(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0"));

                if (xmlStream != null && xmlStream != "")
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlStream);

                    //XmlNode listaAzioni = doc.SelectSingleNode("AMMINISTRAZIONE");
                    XmlNode listaAzioni = doc.SelectSingleNode("NewDataSet");
                    if (listaAzioni.ChildNodes.Count > 0)
                    {
                        foreach (XmlNode azione in listaAzioni.ChildNodes)
                        {
                            //carica il dataset
                            row = dataSet.Tables["LOG"].NewRow();
                            row["codice"] = azione.ChildNodes[0].InnerText;
                            row["oggetto"] = azione.ChildNodes[2].InnerText;
                            row["descrizione"] = azione.ChildNodes[1].InnerText;

                            dataSet.Tables["LOG"].Rows.Add(row);
                        }

                        ViewState["LOG"] = dataSet;
                    }

                    caricaComboOggetto();
                    pnl_no_log.Visible = false;
                    pnl_tipo_evento.Visible = true;
                }
                else
                {
                    pnl_no_log.Visible = true;
                    pnl_tipo_evento.Visible = false;
                    lbl_errore.Text = "ATTENZIONE! nessun record dei log presente in tabella!";
                    //btn_cerca.Visible = false;
                }
            }
            catch
            {
                pnl_no_log.Visible = true;
                pnl_tipo_evento.Visible = false;
                lbl_errore.Text = "ATTENZIONE! errore nel caricamento del file XML dei log!";
                //btn_cerca.Visible = false;
            }
        }

        /// <summary>
        /// Inizializza il dataset
        /// </summary>
        private void IniDataSet()
        {
            dataSet = new DataSet();

            dataSet.Tables.Add("LOG");

            DataColumn dc = new DataColumn("codice");
            dataSet.Tables["LOG"].Columns.Add(dc);

            dc = new DataColumn("oggetto");
            dataSet.Tables["LOG"].Columns.Add(dc);

            dc = new DataColumn("descrizione");
            dataSet.Tables["LOG"].Columns.Add(dc);
        }

        private void caricaComboRf()
        {
            this.ddl_rf.Items.Clear();

            DocsPAWA.DocsPaWR.OrgRegistro[] listaTotale = null;
            //voglio la lista dei soli RF, quindi al webMethod passero come chaRF il valore 1 (solo RF)
            listaTotale = ws.AmmGetRegistri(codAmm, "1");

            if (listaTotale != null && listaTotale.Length > 0)
            {
                int y = 0;
                for (int i = 0; i < listaTotale.Length; i++)
                {
                    string testo = listaTotale[i].Codice;
                    this.ddl_rf.Items.Add(testo);
                    this.ddl_rf.Items[y].Value = listaTotale[i].IDRegistro;
                    y++;
                }

                DocsPAWA.DocsPaWR.Registro rf_reg = wws.GetRegistroBySistemId(ddl_rf.SelectedItem.Value);
                this.lbl_rf.Text = rf_reg.descrizione;
            }
        }

        private void caricaComboRegistri()
        {
            this.ddl_aoo.Items.Clear();

            DocsPAWA.DocsPaWR.OrgRegistro[] listaTotale = null;
            //voglio la lista dei soli RF, quindi al webMethod passero come chaRF il valore 1 (solo RF)
            listaTotale = ws.AmmGetRegistri(codAmm, "0");

            if (listaTotale != null && listaTotale.Length > 0)
            {
                int y = 0;
                for (int i = 0; i < listaTotale.Length; i++)
                {
                    string testo = listaTotale[i].Codice;
                    this.ddl_aoo.Items.Add(testo);
                    this.ddl_aoo.Items[y].Value = listaTotale[i].IDRegistro;
                    y++;
                }
            }
        }

        private void setRadioButtonRuoli()
        {
            ListItem itemM = new ListItem("Ruolo", "Ruolo");
            rbl_ruoli.Items.Add(itemM);
            itemM = new ListItem("Tipo Ruolo", "TP");
            rbl_ruoli.Items.Add(itemM);
            itemM = new ListItem("UO", "UO");
            rbl_ruoli.Items.Add(itemM);
            if (ws.IsEnabldRF(codAmm))
            {
                itemM = new ListItem("RF", "RF");
                rbl_ruoli.Items.Add(itemM);
            }
            itemM = new ListItem("AOO", "AOO");
            rbl_ruoli.Items.Add(itemM);
            itemM = new ListItem("Reset", "R");
            rbl_ruoli.Items.Add(itemM);
        }

        private void rbl_ruoli_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.rbl_ruoli.SelectedItem.Value.Equals("Ruolo"))
            {
                this.pnl_uo.Visible = false;
                this.pnl_rf.Visible = false;
                this.pnl_aoo.Visible = false;
                this.pnl_ruolo.Visible = true;
                this.pnl_tipo_ruolo.Visible = false;
            }

            if (this.rbl_ruoli.SelectedItem.Value.Equals("UO"))
            {
                this.pnl_uo.Visible = true;
                this.pnl_rf.Visible = false;
                this.pnl_aoo.Visible = false;
                this.pnl_ruolo.Visible = false;
                this.pnl_tipo_ruolo.Visible = false;
            }

            if (this.rbl_ruoli.SelectedItem.Value.Equals("RF"))
            {
                this.pnl_uo.Visible = false;
                this.pnl_rf.Visible = true;
                this.pnl_aoo.Visible = false;
                this.pnl_ruolo.Visible = false;
                this.pnl_tipo_ruolo.Visible = false;
            }

            if (this.rbl_ruoli.SelectedItem.Value.Equals("AOO"))
            {
                this.pnl_uo.Visible = false;
                this.pnl_rf.Visible = false;
                this.pnl_aoo.Visible = true;
                this.pnl_ruolo.Visible = false;
                this.pnl_tipo_ruolo.Visible = false;
            }

            if (this.rbl_ruoli.SelectedItem.Value.Equals("R"))
            {
                this.pnl_uo.Visible = false;
                this.pnl_rf.Visible = false;
                this.pnl_aoo.Visible = false;
                this.pnl_ruolo.Visible = false;
                this.pnl_tipo_ruolo.Visible = false;
            }

            if (this.rbl_ruoli.SelectedItem.Value.Equals("TP"))
            {
                this.pnl_uo.Visible = false;
                this.pnl_rf.Visible = false;
                this.pnl_aoo.Visible = false;
                this.pnl_ruolo.Visible = false;
                this.pnl_tipo_ruolo.Visible = true;
            }

            descrizioneFinale();
        }

        private void txt_codRuolo_TextChanged(object sender, System.EventArgs e)
        {
            setDescCorrRuolo(this.txt_codRuolo.Text);
        }

        private void txt_codUo_TextChanged(object sender, System.EventArgs e)
        {
            setDescCorrUO(this.txt_codUo.Text);
        }

        private void setDescCorrRuolo(string codiceRubrica)
        {
            DocsPAWA.DocsPaWR.Corrispondente corr = null;
            DocsPAWA.UserManager.removeCorrispondentiSelezionati(this);
            if (!codiceRubrica.Equals(""))
                corr = DocsPAWA.UserManager.getCorrispondente(this, codiceRubrica, false);

            if (corr == null)
            {
                corr_non_trovato("1");
            }
            else
            {
                if (!corr.tipoCorrispondente.Equals("R"))
                {
                    corr_non_trovato("2");
                }
                else
                {
                    this.txt_codRuolo.Text = "";
                    this.txt_descRuolo.Text = "";
                    this.hd_systemIdCorrRuolo.Value = "";

                    DocsPAWA.UserManager.setCorrispondenteSelezionatoRuoloAmministrazione(this.Page, corr);
                    this.txt_codRuolo.Text = codiceRubrica;
                    this.txt_descRuolo.Text = DocsPAWA.UserManager.getDecrizioneCorrispondenteSemplice(corr);
                    this.hd_systemIdCorrRuolo.Value = corr.systemId;
                    descrizioneFinale();
                }
            }
        }

        private void corr_non_trovato(string caso)
        {
            string s = null;
            this.txt_codRuolo.Text = "";
            this.txt_descRuolo.Text = "";
            this.hd_systemIdCorrRuolo.Value = "";
            descrizioneFinale();

            switch (caso)
            {
                case "1":
                    s = "Corrispondente non trovato";
                    break;

                case "2":
                    s = "Inserire soltanto Ruoli come corrispondenti";
                    break;

            }
            if (!IsStartupScriptRegistered("corr_non_trovato"))
                RegisterStartupScript("corr_non_trovato",
                    "<script language=\"javascript\">" +
                    "alert (\"" + s + "\");</script>");
        }

        private void setDescCorrUO(string codiceRubrica)
        {
            DocsPAWA.DocsPaWR.Corrispondente corr = null;
            DocsPAWA.UserManager.removeCorrispondentiSelezionati(this);
            if (!codiceRubrica.Equals(""))
                corr = DocsPAWA.UserManager.getCorrispondente(this, codiceRubrica, false);

            if (corr == null)
            {
                corr_UO_non_trovato("1");
            }
            else
            {
                if (!corr.tipoCorrispondente.Equals("U"))
                {
                    corr_UO_non_trovato("2");
                }
                else
                {
                    this.txt_codUo.Text = "";
                    this.txt_descUo.Text = "";
                    this.hd_systemIdCorrUO.Value = "";

                    DocsPAWA.UserManager.setCorrispondenteSelezionatoUOAmministrazione(this.Page, corr);
                    this.txt_codUo.Text = codiceRubrica;
                    this.txt_descUo.Text = DocsPAWA.UserManager.getDecrizioneCorrispondenteSemplice(corr);
                    this.hd_systemIdCorrUO.Value = corr.systemId;
                    descrizioneFinale();
                }
            }
        }

        private void corr_UO_non_trovato(string caso)
        {
            string s = null;
            this.txt_codUo.Text = "";
            this.txt_descUo.Text = "";
            this.hd_systemIdCorrUO.Value = "";
            descrizioneFinale();

            switch (caso)
            {
                case "1":
                    s = "Corrispondente non trovato";
                    break;

                case "2":
                    s = "Inserire soltanto UO come corrispondenti";
                    break;

            }
            if (!IsStartupScriptRegistered("corr_non_trovato"))
                RegisterStartupScript("corr_non_trovato",
                    "<script language=\"javascript\">" +
                    "alert (\"" + s + "\");</script>");
        }

        private void cancelDataA(object sender, System.EventArgs e)
        {
            this.GetCalendarControl("txt_data_a").txt_Data.Text = string.Empty;
            this.pnl_ipa.Visible = false;
            this.pnl_risultati_iop.Visible = false;
            this.pnl_periodo_ipa.Visible = false;
            this.pnl_risultati_iap.Visible = false;
            this.pnl_iagg.Visible = false;
            this.pnl_risultati_iagg.Visible = false;
            this.btn_stampa.Enabled = false;
            this.pnl_finale.Visible = false;
            this.lbl_numero.Visible = false;
            this.txt_numeroUnita.Visible = false;
        }

        private void cancelDataDA(object sender, System.EventArgs e)
        {
            this.GetCalendarControl("txt_data_da").txt_Data.Text = string.Empty;
            this.pnl_ipa.Visible = false;
            this.pnl_risultati_iop.Visible = false;
            this.pnl_periodo_ipa.Visible = false;
            this.pnl_risultati_iap.Visible = false;
            this.pnl_iagg.Visible = false;
            this.pnl_risultati_iagg.Visible = false;
            this.btn_stampa.Enabled = false;
            this.pnl_finale.Visible = false;
            this.lbl_numero.Visible = false;
            this.txt_numeroUnita.Visible = false;
        }

        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        private DocsPAWA.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (DocsPAWA.UserControls.Calendar)this.FindControl(controlId);
        }

        private void abilitaCampi()
        {
            bool contIpa = true;
            
            if (!string.IsNullOrEmpty(txt_data_da.Text) || !string.IsNullOrEmpty(txt_data_a.Text))
            {
                if (!DocsPAWA.Utils.isDate(this.GetCalendarControl("txt_data_da").txt_Data.Text) || !DocsPAWA.Utils.isDate(this.GetCalendarControl("txt_data_a").txt_Data.Text))
                {

                    if (!string.IsNullOrEmpty(txt_data_da.Text) && !DocsPAWA.Utils.isDate(this.GetCalendarControl("txt_data_da").txt_Data.Text))
                    {
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Seleziona_data_iop", "alert('Verificare il formato della data di inzio');", true);
                        txt_data_da.Text = "";
                        contIpa = false;
                    }

                    if (!string.IsNullOrEmpty(txt_data_a.Text) && !DocsPAWA.Utils.isDate(this.GetCalendarControl("txt_data_a").txt_Data.Text))
                    {
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Seleziona_data_iop", "alert('Verificare il formato della data di fine');", true);
                        txt_data_a.Text = "";
                        contIpa = false;
                    }
                }
            }

            if (!string.IsNullOrEmpty(txt_data_da.Text) && !string.IsNullOrEmpty(txt_data_a.Text) && contIpa)
            {
                if (!DocsPAWA.Utils.isDate(this.GetCalendarControl("txt_data_da").txt_Data.Text) || !DocsPAWA.Utils.isDate(this.GetCalendarControl("txt_data_a").txt_Data.Text))
                {
                    contIpa = false;
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Seleziona_data_iop", "alert('Verificare il formato della data');", true);

                    if (!DocsPAWA.Utils.isDate(this.GetCalendarControl("txt_data_da").txt_Data.Text))
                    {
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Seleziona_data_iop", "alert('Verificare il formato della data di inzio');", true);
                        txt_data_da.Text = "";
                    }

                    if (!DocsPAWA.Utils.isDate(this.GetCalendarControl("txt_data_aa").txt_Data.Text))
                    {
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Seleziona_data_iop", "alert('Verificare il formato della data di fine');", true);
                        txt_data_a.Text = "";
                    }
                }

                if (DocsPAWA.Utils.verificaIntervalloDate(this.GetCalendarControl("txt_data_da").txt_Data.Text, this.GetCalendarControl("txt_data_a").txt_Data.Text))
                {
                    contIpa = false;
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Seleziona_data_iop", "alert('La data fine deve essere maggiore della data di inzio');", true);
                }

                this.giorniIOP = DocsPAWA.Utils.getIntervalloDate(this.GetCalendarControl("txt_data_da").txt_Data.Text, this.GetCalendarControl("txt_data_a").txt_Data.Text);

                if (contIpa == true)
                {
                    //this.pnl_ipa.Visible = true;
                    this.pnl_iagg.Visible = true;
                    if (ViewState["dataInit"] != null)
                    {
                        this.dataInitIOP = ViewState["dataInit"] as string;
                    }

                    if (ViewState["dataFine"] != null)
                    {
                        this.dataFineIOP = ViewState["dataFine"] as string;
                    }

                    if (string.IsNullOrEmpty(this.dataInitIOP) || string.IsNullOrEmpty(this.dataFineIOP) || !this.dataInitIOP.Equals(this.GetCalendarControl("txt_data_da").txt_Data.Text) || !this.dataFineIOP.Equals(this.GetCalendarControl("txt_data_a").txt_Data.Text))
                    {
                        //this.ddl_periodo_ipa.Items.Clear();
                        
                        //this.caricaComboPeriodoIAP(this.giorniIOP);
                        this.caricaComboIAGG(this.giorniIOP.ToString());

                        //

                        this.pnl_risultati_iop.Visible = false;
                        this.pnl_periodo_ipa.Visible = false;
                        //this.pnl_iagg.Visible = false;
                        this.pnl_finale.Visible = false;
                        this.btn_stampa.Enabled = false;
                        this.lbl_numero.Visible = false;
                        this.txt_numeroUnita.Visible = false;
                    }

                    ViewState["dataInit"] = this.GetCalendarControl("txt_data_da").txt_Data.Text;
                    ViewState["dataFine"] = this.GetCalendarControl("txt_data_a").txt_Data.Text;

                    this.dataInitIOP = this.GetCalendarControl("txt_data_da").txt_Data.Text;
                    this.dataFineIOP = this.GetCalendarControl("txt_data_a").txt_Data.Text;
                    string mese_inizio = DocsPAWA.Utils.getMonthFromData(this.dataInitIOP);
                    string mese_inizio_normale = DocsPAWA.Utils.getMonthFromData(this.dataInitIOP);
                    mese_inizio = mese_inizio.Substring(0, 3);

                    this.dataInitIOP = this.dataInitIOP.Trim();

                    int inizio_giorno = this.dataInitIOP.IndexOf('/');
                    int fine_anno = this.dataInitIOP.LastIndexOf('/');

                    string giorno_inizio = this.dataInitIOP.Substring(0, inizio_giorno);
                    string anno_inizio = this.dataInitIOP.Substring(fine_anno + 1);

                    string mese_fine = DocsPAWA.Utils.getMonthFromData(this.dataFineIOP);
                    string mese_fine_normale = DocsPAWA.Utils.getMonthFromData(this.dataFineIOP);
                    mese_fine = mese_fine.Substring(0, 3);

                    string giorno_fine = this.dataFineIOP.Substring(0, inizio_giorno);
                    string anno_fine = this.dataFineIOP.Substring(fine_anno + 1);

                    this.lbl_quanto_iop.Text = "Intervallo di osservazione:  Dal " + giorno_inizio + " " + mese_inizio + " " + anno_inizio + " al " + giorno_fine + " " + mese_fine + " " + anno_fine + "";
                    this.pnl_risultati_iop.Visible = true;

                    this.iop_finale = "dal " + giorno_inizio + " " + mese_inizio_normale + " " + anno_inizio + " al " + giorno_fine + " " + mese_fine_normale + " " + anno_fine;
                }
                else
                {
                    this.pnl_ipa.Visible = false;
                    this.pnl_risultati_iop.Visible = false;
                    this.pnl_periodo_ipa.Visible = false;
                    this.pnl_iagg.Visible = false;
                    this.lbl_numero.Visible = false;
                    this.txt_numeroUnita.Visible = false;
                }
            }
            else
            {
                this.pnl_ipa.Visible = false;
                this.pnl_risultati_iop.Visible = false;
                this.pnl_periodo_ipa.Visible = false;
                this.pnl_iagg.Visible = false;
                this.lbl_numero.Visible = false;
                this.txt_numeroUnita.Visible = false;
            }
        }

        private string convertiLogica(int giorni)
        {
            string result = "";

            int anni = giorni / 365;

            int resto_anni = giorni % 365;

            int mesi = resto_anni / 30;

            int resto_mesi = resto_anni % 30;

            if (anni > 0)
            {
                string cont = " ";
                if (mesi > 0 || resto_mesi > 0)
                {
                    cont = ", ";
                }
                if (anni == 1)
                {
                    result += anni.ToString() + " anno" + cont;
                }
                else
                {
                    result += anni.ToString() + " anni" + cont;
                }
            }

            if (mesi > 0)
            {
                string cont = " ";
                if (resto_mesi > 0)
                {
                    cont = " e ";
                }
                if (mesi == 1)
                {
                    result += mesi.ToString() + " mese" + cont;
                }
                else
                {
                    result += mesi.ToString() + " mesi" + cont;
                }
            }

            if (resto_mesi > 0)
            {
                if (resto_mesi == 1)
                {
                    result += resto_mesi.ToString() + " giorno ";
                }
                else
                {
                    result += resto_mesi.ToString() + " giorni ";
                }
            }

            return result;
        }

        /// <summary>
        /// onChange ddl_periodo_ipa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddl_periodo_ipa_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!this.ddl_periodo_ipa.SelectedItem.Value.Equals("niente"))
            {
                this.pnl_periodo_ipa.Visible = true;
                caricaComboAmpiezzaIAP(this.ddl_periodo_ipa.SelectedItem.Value);
                this.pnl_risultati_iap.Visible = false;
                this.pnl_iagg.Visible = false;
                this.pnl_risultati_iagg.Visible = false;
                this.btn_stampa.Enabled = false;
                this.pnl_finale.Visible = false;
                this.txt_numeroUnita.Visible = false;
            }
            else
            {
                this.pnl_periodo_ipa.Visible = false;
                this.pnl_risultati_iap.Visible = false;
                this.pnl_iagg.Visible = false;
                this.pnl_risultati_iagg.Visible = false;
                this.btn_stampa.Enabled = false;
                this.pnl_finale.Visible = false;
                this.lbl_numero.Visible = false;
                this.txt_numeroUnita.Visible = false;
            }
        }

        private void caricaComboPeriodoIAP(int numero_giorni)
        {

            this.ddl_periodo_ipa.Items.Add(new ListItem("", "niente"));

            if (numero_giorni >= 365)
            {
                this.ddl_periodo_ipa.Items.Add(new ListItem("anno", "6"));
            }

            if (numero_giorni >= 120)
            {
                this.ddl_periodo_ipa.Items.Add(new ListItem("trimestre", "5"));
            }

            if (numero_giorni >= 30)
            {
                this.ddl_periodo_ipa.Items.Add(new ListItem("mese", "4"));
            }

            if (numero_giorni >= 7)
            {
                this.ddl_periodo_ipa.Items.Add(new ListItem("settimana", "3"));
            }

            if (numero_giorni >= 1)
            {
                this.ddl_periodo_ipa.Items.Add(new ListItem("giorno", "2"));
            }

            this.ddl_periodo_ipa.Items.Add(new ListItem("ora", "1"));

        }

        protected void caricaComboAmpiezzaIAP(string valoreGiorni)
        {
            this.ddl_ampiezza_ipa.Items.Clear();

            this.ddl_ampiezza_ipa.Items.Add(new ListItem("", "niente"));

            int numero_giorni = Convert.ToInt32(valoreGiorni);

            //Se il periodo dell'ipa è di un anno anche l'ampiezza sarà un anno

            if (this.ddl_periodo_ipa.SelectedItem.Value.Equals("6"))
            {
                this.ddl_ampiezza_ipa.Items.Add(new ListItem("anno", "6"));
            }
            else
            {

                if (numero_giorni >= 6)
                {
                    this.ddl_ampiezza_ipa.Items.Add(new ListItem("anno", "6"));
                }

                if (numero_giorni >= 5)
                {
                    this.ddl_ampiezza_ipa.Items.Add(new ListItem("trimestre", "5"));
                }

                if (numero_giorni >= 4)
                {
                    this.ddl_ampiezza_ipa.Items.Add(new ListItem("mese", "4"));
                }

                if (numero_giorni >= 3)
                {
                    this.ddl_ampiezza_ipa.Items.Add(new ListItem("settimana", "3"));
                }

                if (numero_giorni >= 2)
                {
                    this.ddl_ampiezza_ipa.Items.Add(new ListItem("giorno", "2"));
                }

                if (numero_giorni >= 1)
                {
                    this.ddl_ampiezza_ipa.Items.Add(new ListItem("ora", "1"));
                }
            }
        }

        /// <summary>
        /// onChange ddl_ampiezza_ipa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddl_ampiezza_ipa_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!this.ddl_ampiezza_ipa.SelectedItem.Value.Equals("niente"))
            {
                this.pnl_risultati_iap.Visible = true;
                string mese = DocsPAWA.Utils.getMonthFromData(this.dataInitIOP);
                mese = mese.Substring(0, 3);

                this.dataInitIOP = this.dataInitIOP.Trim();

                int inizio_giorno = this.dataInitIOP.IndexOf('/');
                int fine_anno = this.dataInitIOP.LastIndexOf('/');

                string giorno = this.dataInitIOP.Substring(0, inizio_giorno);
                string anno = this.dataInitIOP.Substring(fine_anno + 1);

                string risultato_IPA = "Ampiezza intervallo periodico di analisi: ";
                risultato_IPA += "periodicità " + ddl_periodo_ipa.SelectedItem.Text + ", ampiezza " + ddl_ampiezza_ipa.SelectedItem.Text + ", collocazione iniziale " + giorno + " " + mese + " " + anno + " " + "(" + DocsPAWA.Utils.getDayWeekFromData(this.dataInitIOP) + ")";
                this.lbl_quanto_iap.Text = risultato_IPA;

                this.pnl_iagg.Visible = true;

                caricaComboIAGG(this.ddl_ampiezza_ipa.SelectedItem.Value);


                this.pnl_risultati_iagg.Visible = false;
                this.btn_stampa.Enabled = false;
                this.pnl_finale.Visible = false;

                if (this.ddl_ampiezza_ipa.SelectedItem.Value != this.ddl_periodo_ipa.SelectedItem.Value)
                {
                    this.lbl_numero.Visible = true;
                    this.txt_numeroUnita.Visible = true;
                    this.txt_numeroUnita.Text = "1";
                }
                else
                {
                    this.lbl_numero.Visible = false;
                    this.txt_numeroUnita.Visible = false;
                }
            }
            else
            {
                this.pnl_risultati_iap.Visible = false;
                this.pnl_iagg.Visible = false;
                this.lbl_quanto_iap.Text = "";
                this.pnl_risultati_iagg.Visible = false;
                this.btn_stampa.Enabled = false;
                this.pnl_finale.Visible = false;
                this.lbl_numero.Visible = false;
                this.txt_numeroUnita.Visible = false;
            }
        }

        protected void caricaComboIAGG(string valoreGiorni)
        {
            this.ddl_iagg.Items.Clear();

            this.ddl_iagg.Items.Add(new ListItem("", "niente"));

            int numero_giorni = Convert.ToInt32(valoreGiorni);

          /*  if (numero_giorni > 6)
            {
                this.ddl_iagg.Items.Add(new ListItem("anno", "6"));
            }

            if (numero_giorni > 5)
            {
                this.ddl_iagg.Items.Add(new ListItem("trimestre", "5"));
            }

            if (numero_giorni > 4)
            {
                this.ddl_iagg.Items.Add(new ListItem("mese", "4"));
            }

            if (numero_giorni > 3)
            {
                this.ddl_iagg.Items.Add(new ListItem("settimana", "3"));
            }

            if (numero_giorni > 2)
            {
                this.ddl_iagg.Items.Add(new ListItem("giorno", "2"));
            }

            if (numero_giorni > 1)
            {
                this.ddl_iagg.Items.Add(new ListItem("ora", "1"));
            }

            this.ddl_iagg.Items.Add(new ListItem("minuto", "0"));
             * */

            if (numero_giorni >= 180)
            {
                this.ddl_iagg.Items.Add(new ListItem("mese", "4"));
            }

            if (numero_giorni < 180 && numero_giorni >= 28)
            {
                this.ddl_iagg.Items.Add(new ListItem("mese", "4"));
                this.ddl_iagg.Items.Add(new ListItem("settimana", "3"));
                this.ddl_iagg.Items.Add(new ListItem("giorno", "2"));
            }

            if (numero_giorni < 28 && numero_giorni >= 7)
            {
                this.ddl_iagg.Items.Add(new ListItem("settimana", "3"));
                this.ddl_iagg.Items.Add(new ListItem("giorno", "2"));
            }

            if (numero_giorni < 7 && numero_giorni > 1)
            {
                this.ddl_iagg.Items.Add(new ListItem("giorno", "2"));
                this.ddl_iagg.Items.Add(new ListItem("ora", "1"));
            }

            if (numero_giorni == 1 )
            {
                this.ddl_iagg.Items.Add(new ListItem("ora", "1"));
                this.ddl_iagg.Items.Add(new ListItem("minuto", "0"));
            }


        }

        /// <summary>
        /// onChange ddl_ampiezza_ipa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddl_iagg_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!this.ddl_iagg.SelectedItem.Value.Equals("niente"))
            {
                this.btn_stampa.Enabled = true;
                this.pnl_risultati_iagg.Visible = true;
                this.lbl_quanto_iag.Text = "Intervallo di aggregazione " + ddl_iagg.SelectedItem.Text;
                this.pnl_finale.Visible = true;
                descrizioneFinale();
            }
            else
            {
                this.btn_stampa.Enabled = false;
                this.pnl_risultati_iagg.Visible = false;
                this.pnl_finale.Visible = false;
            }
        }

        private void descrizioneFinale()
        {
            string finale = "Aggrega ";
            if (this.ddl_oggetto.SelectedItem.Value != "null")
            {
                if (this.ddl_azione.SelectedItem.Value != "null")
                {
                    finale += "tutti gli eventi di '" + (ddl_azione.SelectedItem.Text).ToLower() + "' ";
                }
                else
                {
                    finale += "tutti gli eventi di '" + (ddl_oggetto.SelectedItem.Text).ToLower() + "' ";
                }
            }
            else
            {
                finale += "tutti gli eventi ";
            }

            if (this.pnl_ipa.Visible == true && this.ddl_periodo_ipa.SelectedItem.Value != "")
            {
                finale += this.iop_finale + " con periodicità";

                if (ddl_periodo_ipa.SelectedItem.Value.Equals("6"))
                {
                    finale += " annuale";
                }
                if (ddl_periodo_ipa.SelectedItem.Value.Equals("5"))
                {
                    finale += " trimestrale";
                }
                if (ddl_periodo_ipa.SelectedItem.Value.Equals("4"))
                {
                    finale += " mensile";
                }
                if (ddl_periodo_ipa.SelectedItem.Value.Equals("3"))
                {
                    finale += " settimanale";
                }
                if (ddl_periodo_ipa.SelectedItem.Value.Equals("2"))
                {
                    finale += " giornaliera";
                }
                if (ddl_periodo_ipa.SelectedItem.Value.Equals("1"))
                {
                    finale += " oraria";
                }
            }

            if (this.pnl_iagg.Visible == true && this.ddl_iagg.SelectedItem.Value != "")
            {
                finale += " all'interno di finestre ";

                if (ddl_iagg.SelectedItem.Value.Equals("6"))
                {
                    finale += " di un anno";
                }
                if (ddl_iagg.SelectedItem.Value.Equals("5"))
                {
                    finale += " di un trimestre";
                }
                if (ddl_iagg.SelectedItem.Value.Equals("4"))
                {
                    finale += " di un mese";
                }
                if (ddl_iagg.SelectedItem.Value.Equals("3"))
                {
                    finale += " di una settimana";
                }
                if (ddl_iagg.SelectedItem.Value.Equals("2"))
                {
                    finale += " di un giorno";
                }
                if (ddl_iagg.SelectedItem.Value.Equals("1"))
                {
                    finale += " di un ora";
                }
                if (ddl_iagg.SelectedItem.Value.Equals("0"))
                {
                    finale += " di un minuto";
                }
            }

            if (this.pnl_periodo_ipa.Visible == true && ddl_ampiezza_ipa.SelectedItem.Value != "")
            {
                finale += " nell'arco";

                if (ddl_ampiezza_ipa.SelectedItem.Value.Equals("6"))
                {
                    if (this.txt_numeroUnita.Visible)
                    {
                        if (!string.IsNullOrEmpty(this.txt_numeroUnita.Text))
                        {
                            if (this.txt_numeroUnita.Text.Equals("1"))
                            {
                                finale += " di un anno";
                            }
                            else
                            {
                                finale += " di " + "" + txt_numeroUnita.Text + " anni";
                            }
                        }
                    }
                    else
                    {
                        finale += " dell'anno";
                    }
                }
                if (ddl_ampiezza_ipa.SelectedItem.Value.Equals("5"))
                {
                    if (this.txt_numeroUnita.Visible)
                    {
                        if (!string.IsNullOrEmpty(this.txt_numeroUnita.Text))
                        {
                            if (this.txt_numeroUnita.Text.Equals("1"))
                            {
                                finale += " di un trimestre";
                            }
                            else
                            {
                                finale += " di " + "" + txt_numeroUnita.Text + " trimestri";
                            }
                        }
                    }
                    else
                    {
                        finale += " di trimestri";
                    }
                }
                if (ddl_ampiezza_ipa.SelectedItem.Value.Equals("4"))
                {
                    if (this.txt_numeroUnita.Visible)
                    {
                        if (!string.IsNullOrEmpty(this.txt_numeroUnita.Text))
                        {
                            if (this.txt_numeroUnita.Text.Equals("1"))
                            {
                                finale += " di un mese";
                            }
                            else
                            {
                                finale += " di " + "" + txt_numeroUnita.Text + " mesi";
                            }
                        }
                    }
                    else
                    {
                        finale += " di mesi";
                    }
                }
                if (ddl_ampiezza_ipa.SelectedItem.Value.Equals("3"))
                {
                    if (this.txt_numeroUnita.Visible)
                    {
                        if (!string.IsNullOrEmpty(this.txt_numeroUnita.Text))
                        {
                            if (this.txt_numeroUnita.Text.Equals("1"))
                            {
                                finale += " di una settimana";
                            }
                            else
                            {
                                finale += " di " + "" + txt_numeroUnita.Text + " settimane";
                            }
                        }
                    }
                    else
                    {
                        finale += " di settimane";
                    }
                }
                if (ddl_ampiezza_ipa.SelectedItem.Value.Equals("2"))
                {
                    if (this.txt_numeroUnita.Visible)
                    {
                        if (!string.IsNullOrEmpty(this.txt_numeroUnita.Text))
                        {
                            if (this.txt_numeroUnita.Text.Equals("1"))
                            {
                                finale += " di un giorno";
                            }
                            else
                            {
                                finale += " di " + "" + txt_numeroUnita.Text + " giorni";
                            }
                        }
                    }
                    else
                    {
                        finale += " di giorni";
                    }
                }
                if (ddl_ampiezza_ipa.SelectedItem.Value.Equals("1"))
                {
                    if (this.txt_numeroUnita.Visible)
                    {
                        if (!string.IsNullOrEmpty(this.txt_numeroUnita.Text))
                        {
                            if (this.txt_numeroUnita.Text.Equals("1"))
                            {
                                finale += " di un ora";
                            }
                            else
                            {
                                finale += " di " + "" + txt_numeroUnita.Text + " ore";
                            }
                        }
                    }
                    else
                    {
                        finale += " di ore";
                    }
                }
            }

            //RUOLI
            if (this.pnl_ruolo.Visible == true && this.rbl_ruoli.SelectedItem.Value.Equals("Ruolo"))
            {
                if (!string.IsNullOrEmpty(this.txt_descRuolo.Text))
                {
                    finale += " effettuati dal ruolo: " + this.txt_descRuolo.Text;
                }
            }

            if (this.pnl_uo.Visible == true && this.rbl_ruoli.SelectedItem.Value.Equals("UO"))
            {
                if (!string.IsNullOrEmpty(this.txt_descUo.Text))
                {
                    finale += " effettuati da tutti i ruoli della UO: " + this.txt_descUo.Text;
                    if (this.chk_sottoposti.Checked)
                    {
                        finale += " e dai ruoli sottoposti";
                    }
                }
            }

            if (this.pnl_rf.Visible == true && this.rbl_ruoli.SelectedItem.Value.Equals("RF"))
            {
                if (this.ddl_rf.SelectedItem.Value != "null")
                {
                    finale += " effettuati da tutti i ruoli dell' RF: " + this.ddl_rf.SelectedItem.Text;
                }
            }

            if (this.pnl_aoo.Visible == true && this.rbl_ruoli.SelectedItem.Value.Equals("AOO"))
            {
                if (this.ddl_aoo.SelectedItem.Value != "null")
                {
                    finale += " effettuati da tutti i ruoli della AOO : " + this.ddl_aoo.SelectedItem.Text;
                }
            }

            if (this.pnl_tipo_ruolo.Visible == true && this.rbl_ruoli.SelectedItem.Value.Equals("TP"))
            {
                if (this.ddl_tp.SelectedItem.Value != "null")
                {
                    finale += " effettuati da tutti i ruoli di tipo : " + this.ddl_tp.SelectedItem.Text;
                }
            }

            finale += " nell'amministrazione " + this.ddl_amministrazioni.SelectedItem.Text;

            this.lbl_finale.Text = finale;
        }

        private void ddl_azione_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            descrizioneFinale();
        }

        private void ddl_rf_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            DocsPAWA.DocsPaWR.Registro rf_reg = wws.GetRegistroBySistemId(ddl_rf.SelectedItem.Value);
            this.lbl_rf.Text = rf_reg.descrizione;

            descrizioneFinale();
        }

        private void ddl_aoo_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            descrizioneFinale();
        }

        private void btn_stampa_Click(object sender, System.EventArgs e)
        {
            ArrayList filtri = new ArrayList();

            Session.Remove("filtri");

            if (controllaDati())
            {
                try
                {
                    DocsPAWA.DocsPaWR.FileDocumento fileRep = new DocsPAWA.DocsPaWR.FileDocumento();

                    #region filtri
                    //CARICA I FILTRI
                    ProspettiRiepilogativi.Frontend.Parametro _amministrazione = new ProspettiRiepilogativi.Frontend.Parametro("idAmministrazione", "idAmministrazione", this.ddl_amministrazioni.SelectedItem.Value);
                    ProspettiRiepilogativi.Frontend.utility.DO_UpdateParameters(filtri, _amministrazione);

                    ProspettiRiepilogativi.Frontend.Parametro _finale = new ProspettiRiepilogativi.Frontend.Parametro("descrizioneFinale", "descrizioneFinale", this.lbl_finale.Text);
                    ProspettiRiepilogativi.Frontend.utility.DO_UpdateParameters(filtri, _finale);

                    if (!string.IsNullOrEmpty(txt_data_da.Text))
                    {
                        ProspettiRiepilogativi.Frontend.Parametro _data_inizio_iop = new ProspettiRiepilogativi.Frontend.Parametro("data_inizio_iop", "data_inizio_iop", this.GetCalendarControl("txt_data_da").txt_Data.Text);
                        ProspettiRiepilogativi.Frontend.utility.DO_UpdateParameters(filtri, _data_inizio_iop);
                    }

                    if (!string.IsNullOrEmpty(txt_data_a.Text))
                    {
                        ProspettiRiepilogativi.Frontend.Parametro _data_fine_iop = new ProspettiRiepilogativi.Frontend.Parametro("data_fine_iop", "data_fine_iop", this.GetCalendarControl("txt_data_a").txt_Data.Text);
                        ProspettiRiepilogativi.Frontend.utility.DO_UpdateParameters(filtri, _data_fine_iop);
                    }

                 /*   if (!string.IsNullOrEmpty(this.ddl_periodo_ipa.SelectedItem.Value))
                    {
                        ProspettiRiepilogativi.Frontend.Parametro _periodo_ipa = new ProspettiRiepilogativi.Frontend.Parametro("periodo_ipa", "periodo_ipa", this.ddl_periodo_ipa.SelectedItem.Value);
                        ProspettiRiepilogativi.Frontend.utility.DO_UpdateParameters(filtri, _periodo_ipa);
                    }
                    */
                /*    if (!string.IsNullOrEmpty(this.ddl_ampiezza_ipa.SelectedItem.Value))
                    {
                        ProspettiRiepilogativi.Frontend.Parametro _ampiezza_ipa = new ProspettiRiepilogativi.Frontend.Parametro("ampiezza_ipa", "ampiezza_ipa", this.ddl_ampiezza_ipa.SelectedItem.Value);
                        ProspettiRiepilogativi.Frontend.utility.DO_UpdateParameters(filtri, _ampiezza_ipa);
                    }
*/
                  /*  if (this.txt_numeroUnita.Visible == true && !string.IsNullOrEmpty(this.txt_numeroUnita.Text))
                    {
                        ProspettiRiepilogativi.Frontend.Parametro _numero_ampiezza_ipa = new ProspettiRiepilogativi.Frontend.Parametro("numero_ampiezza_ipa", "numero_ampiezza_ipa", this.txt_numeroUnita.Text);
                        ProspettiRiepilogativi.Frontend.utility.DO_UpdateParameters(filtri, _numero_ampiezza_ipa);
                    }
*/
                    if (!string.IsNullOrEmpty(this.ddl_iagg.SelectedValue))
                    {
                        ProspettiRiepilogativi.Frontend.Parametro _iagg = new ProspettiRiepilogativi.Frontend.Parametro("iagg", "iagg", this.ddl_iagg.SelectedValue);
                        ProspettiRiepilogativi.Frontend.utility.DO_UpdateParameters(filtri, _iagg);
                    }

                    //RUOLI
                    if (this.pnl_ruolo.Visible == true && this.rbl_ruoli.SelectedItem.Value.Equals("Ruolo"))
                    {
                        if (!string.IsNullOrEmpty(this.hd_systemIdCorrRuolo.Value))
                        {
                            ProspettiRiepilogativi.Frontend.Parametro _ruolo = new ProspettiRiepilogativi.Frontend.Parametro("ruolo", "ruolo", this.hd_systemIdCorrRuolo.Value);
                            ProspettiRiepilogativi.Frontend.utility.DO_UpdateParameters(filtri, _ruolo);
                        }
                    }

                    if (this.pnl_uo.Visible == true && this.rbl_ruoli.SelectedItem.Value.Equals("UO"))
                    {
                        if (!string.IsNullOrEmpty(this.hd_systemIdCorrUO.Value))
                        {
                            ProspettiRiepilogativi.Frontend.Parametro _uo = new ProspettiRiepilogativi.Frontend.Parametro("uo", "uo", this.hd_systemIdCorrUO.Value);
                            ProspettiRiepilogativi.Frontend.utility.DO_UpdateParameters(filtri, _uo);

                            if (this.chk_sottoposti.Checked)
                            {
                                ProspettiRiepilogativi.Frontend.Parametro _sottoposti = new ProspettiRiepilogativi.Frontend.Parametro("sottoposti", "sottoposti", "true");
                                ProspettiRiepilogativi.Frontend.utility.DO_UpdateParameters(filtri, _sottoposti);
                            }
                        }
                    }

                    if (this.pnl_rf.Visible == true && this.rbl_ruoli.SelectedItem.Value.Equals("RF"))
                    {
                        if (!string.IsNullOrEmpty(this.ddl_rf.SelectedItem.Value))
                        {
                            ProspettiRiepilogativi.Frontend.Parametro _rf = new ProspettiRiepilogativi.Frontend.Parametro("rf", "rf", this.ddl_rf.SelectedItem.Value);
                            ProspettiRiepilogativi.Frontend.utility.DO_UpdateParameters(filtri, _rf);
                        }
                    }

                    if (this.pnl_aoo.Visible == true && this.rbl_ruoli.SelectedItem.Value.Equals("AOO"))
                    {
                        if (!string.IsNullOrEmpty(this.ddl_aoo.SelectedItem.Value))
                        {
                            ProspettiRiepilogativi.Frontend.Parametro _aoo = new ProspettiRiepilogativi.Frontend.Parametro("aoo", "aoo", this.ddl_aoo.SelectedItem.Value);
                            ProspettiRiepilogativi.Frontend.utility.DO_UpdateParameters(filtri, _aoo);
                        }
                    }

                    if (this.pnl_tipo_ruolo.Visible == true && this.rbl_ruoli.SelectedItem.Value.Equals("TP"))
                    {
                        if (!string.IsNullOrEmpty(this.ddl_tp.SelectedItem.Value))
                        {
                            ProspettiRiepilogativi.Frontend.Parametro _tipo_ruolo = new ProspettiRiepilogativi.Frontend.Parametro("tp", "tp", this.ddl_tp.SelectedItem.Value);
                            ProspettiRiepilogativi.Frontend.utility.DO_UpdateParameters(filtri, _tipo_ruolo);
                        }
                    }

                    if (this.ddl_oggetto.SelectedItem.Value != "null")
                    {

                        ProspettiRiepilogativi.Frontend.Parametro _evento = new ProspettiRiepilogativi.Frontend.Parametro("evento", "evento", this.ddl_oggetto.SelectedItem.Value);
                        ProspettiRiepilogativi.Frontend.utility.DO_UpdateParameters(filtri, _evento);

                        if (this.ddl_azione.SelectedItem.Value != "null")
                        {
                            ProspettiRiepilogativi.Frontend.Parametro _evento_secondario = new ProspettiRiepilogativi.Frontend.Parametro("evento_secondario", this.ddl_azione.SelectedItem.Text, this.ddl_azione.SelectedItem.Value);
                            ProspettiRiepilogativi.Frontend.utility.DO_UpdateParameters(filtri, _evento_secondario);
                        }
                    }
                    else
                    {
                        //MEV CONS 1.3
                        //filtro per nascondere i log conservazione
                        //se è attiva la chiave e non è selezionato nessun oggetto
                        if (this.DisableAmmGestCons())
                        {
                            ProspettiRiepilogativi.Frontend.Parametro _evento_secondario = new ProspettiRiepilogativi.Frontend.Parametro("evento_secondario", "PGU_FE_DISABLE_AMM_GEST_CONS", "PGU_FE_DISABLE_AMM_GEST_CONS");
                            ProspettiRiepilogativi.Frontend.utility.DO_UpdateParameters(filtri, _evento_secondario);
                        }
                    }

                    

                    ////
                    #endregion

                    Session["filtri"] = filtri;
                    filtri = (ArrayList)Session["filtri"];

                    DocsPAWA.DocsPaWR.InfoUtente infoUtente = new DocsPAWA.DocsPaWR.InfoUtente();

                    fileRep = ProspettiRiepilogativi.Frontend.Controller.DO_StampaLog(filtri, infoUtente);

                    if (fileRep != null)
                    {
                        Session["Estrazione"] = fileRep;
                        this.iframeVisUnificata.Attributes["src"] = "ReportFile.aspx";
                    }
                    else
                    {
                        this.ClientScript.RegisterStartupScript(this.GetType(), "no_dati", "alert('Non ci sono dati da esportare per questa selezione');", true);
                    }

                    //this.mdlPopupWait.Hide();
                }

                catch (Exception ex)
                {
                    DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
                }
            }
            else
            {
                //this.mdlPopupWait.Hide();
            }
        }

        public void ddl_amministrazioni_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            descrizioneFinale();
        }

        private void LoadDDLAmministrazioni(ArrayList listaAmministrazioni, string idAmmSelected)
        {
            this.ddl_amministrazioni.Items.Clear();

            foreach (DocsPAWA.DocsPaWR.InfoAmministrazione currAmm in listaAmministrazioni)
            {
                ListItem items = new ListItem(currAmm.Descrizione, currAmm.IDAmm);
                this.ddl_amministrazioni.Items.Add(items);
            }

            if (this.ddl_amministrazioni.Items.Count > 1)
                this.ddl_amministrazioni.SelectedIndex = this.ddl_amministrazioni.Items.IndexOf(this.ddl_amministrazioni.Items.FindByValue(idAmmSelected));
        }

        private void chk_sottoposti_SelectIndexChanged(object sender, System.EventArgs e)
        {
            descrizioneFinale();
        }

        private void ddl_tp_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            descrizioneFinale();
        }

        private void txt_numeroUnita_TextChanged(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txt_numeroUnita.Text))
            {
                int numeroDaVerficare;

                if ((Int32.TryParse(this.txt_numeroUnita.Text, out numeroDaVerficare)))
                {

                    if (!string.IsNullOrEmpty(this.ddl_periodo_ipa.SelectedValue) && !string.IsNullOrEmpty(this.ddl_ampiezza_ipa.SelectedValue))
                    {
                        int periodo = Convert.ToInt32(this.ddl_periodo_ipa.SelectedValue);
                        int ampiezza = Convert.ToInt32(this.ddl_ampiezza_ipa.SelectedValue);

                        if (periodo > ampiezza)
                        {
                            int soglia = 0;

                            if (ampiezza == 6)
                            {
                                soglia = this.giorniIOP / 12;
                            }

                            if (ampiezza == 5)
                            {
                                soglia = 4;
                            }
                            if (ampiezza == 4)
                            {
                                if (periodo == 5)
                                {
                                    soglia = 3;
                                }
                                else
                                {
                                    soglia = 12;
                                }
                            }
                            if (ampiezza == 3)
                            {
                                soglia = 4;
                            }
                            if (ampiezza == 2)
                            {
                                soglia = 31;
                            }
                            if (ampiezza == 1)
                            {
                                soglia = 24;
                            }

                            if (numeroDaVerficare <= soglia)
                            {
                                descrizioneFinale();
                            }
                            else
                            {
                                this.txt_numeroUnita.Text = "1";
                                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "numero_errato", "alert('Attenzione! Il numero di ampiezza deve essere minore rispetto unità di misura selezionata nel periodo');", true);
                            }
                        }
                        else
                        {
                            this.txt_numeroUnita.Text = "1";
                            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "numero_errato", "alert('Attenzione! Seleziona un ampiezza minore del periodo');", true);
                        }
                    }
                    else
                    {
                        this.txt_numeroUnita.Text = "1";
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "numero_errato", "alert('Attenzione! Prima seleziona un periodo e un ampiezza');", true);
                    }
                }
                else
                {
                    this.txt_numeroUnita.Text = "1";
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "numero_errato", "alert('Attenzione! Devi inserire un numero');", true);
                }
            }
        }


        public bool controllaDati()
        {

            bool contIpa = true;

            if (contIpa && (string.IsNullOrEmpty(txt_data_da.Text) || string.IsNullOrEmpty(txt_data_a.Text)))
            {
                contIpa = false;
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Seleziona_data_iop", "alert('Inserire la data inizio e fine dello IOP');", true);
            }

            if (contIpa && (!DocsPAWA.Utils.isDate(this.GetCalendarControl("txt_data_da").txt_Data.Text) || !DocsPAWA.Utils.isDate(this.GetCalendarControl("txt_data_a").txt_Data.Text)))
            {
                contIpa = false;
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Seleziona_data_iop", "alert('Verificare il formato della data dello IOP');", true);
            }

            if (contIpa && DocsPAWA.Utils.verificaIntervalloDate(this.GetCalendarControl("txt_data_da").txt_Data.Text, this.GetCalendarControl("txt_data_a").txt_Data.Text))
            {
                contIpa = false;
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Seleziona_data_iop", "alert('La data fine deve essere maggiore della data di inzio nello IOP');", true);
            }

            this.giorniIOP = DocsPAWA.Utils.getIntervalloDate(this.GetCalendarControl("txt_data_da").txt_Data.Text, this.GetCalendarControl("txt_data_a").txt_Data.Text);

         /*   if (contIpa && string.IsNullOrEmpty(this.ddl_periodo_ipa.SelectedItem.Value))
            {
                contIpa = false;
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Seleziona_data_iop", "alert('Devi selezionare il periodo per IPA');", true);
            }*/

          /*  if (contIpa && string.IsNullOrEmpty(this.ddl_ampiezza_ipa.SelectedItem.Value))
            {
                contIpa = false;
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Seleziona_data_iop", "alert('Devi selezionare un tipo di ampiezza per IPA');", true);
            }*/

            if (contIpa && string.IsNullOrEmpty(this.ddl_iagg.SelectedItem.Value))
            {
                contIpa = false;
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Seleziona_data_iop", "alert('Devi selezionare un tipo per lo IAGG');", true);
            }
          

    /*        if (contIpa && this.txt_numeroUnita.Visible)
            {
                contIpa = verificaNumeroUnita();
            }
*/

            return contIpa;
        }

        protected bool verificaNumeroUnita()
        {
            bool retvalue = true;

            if (!string.IsNullOrEmpty(this.txt_numeroUnita.Text))
            {
                int numeroDaVerficare;

                if ((Int32.TryParse(this.txt_numeroUnita.Text, out numeroDaVerficare)))
                {

                    if (!string.IsNullOrEmpty(this.ddl_periodo_ipa.SelectedValue) && !string.IsNullOrEmpty(this.ddl_ampiezza_ipa.SelectedValue))
                    {
                        int periodo = Convert.ToInt32(this.ddl_periodo_ipa.SelectedValue);
                        int ampiezza = Convert.ToInt32(this.ddl_ampiezza_ipa.SelectedValue);

                        if (periodo > ampiezza)
                        {
                            int soglia = 0;

                            if (ampiezza == 5)
                            {
                                soglia = 4;
                            }
                            if (ampiezza == 4)
                            {
                                if (periodo == 5)
                                {
                                    soglia = 3;
                                }
                                else
                                {
                                    soglia = 12;
                                }
                            }
                            if (ampiezza == 3)
                            {
                                soglia = 4;
                            }
                            if (ampiezza == 2)
                            {
                                soglia = 31;
                            }
                            if (ampiezza == 1)
                            {
                                soglia = 24;
                            }

                            if (numeroDaVerficare > soglia)
                            {
                                retvalue = false;
                                this.txt_numeroUnita.Text = "";
                                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "numero_errato", "alert('Attenzione! Il numero di ampiezza deve essere minore rispetto unità di misura selezionata nel periodo');", true);
                                return retvalue;
                            }
                        }
                        else
                        {
                            retvalue = false;
                            this.txt_numeroUnita.Text = "";
                            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "numero_errato", "alert('Attenzione! Seleziona un ampiezza minore del periodo');", true);
                            return retvalue;
                        }
                    }
                    else
                    {
                        retvalue = false;
                        this.txt_numeroUnita.Text = "";
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "numero_errato", "alert('Attenzione! Prima seleziona un periodo e un ampiezza');", true);
                        return retvalue;
                    }
                }
                else
                {
                    retvalue = false;
                    this.txt_numeroUnita.Text = "";
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "numero_errato", "alert('Attenzione! Devi inserire un numero');", true);
                    return retvalue;
                }
            }

            return retvalue;
        }

        /// <summary>
        /// Gestione caricamento lista tipi ruolo
        /// </summary>
        private void FillListTipiRuolo()
        {
            DocsPAWA.DocsPaWR.OrgTipoRuolo[] tipiRuolo = this.GetTipiRuolo();

            foreach (DocsPAWA.DocsPaWR.OrgTipoRuolo tipoRuolo in tipiRuolo)
            {
                ListItem items = new ListItem(tipoRuolo.Descrizione, tipoRuolo.IDTipoRuolo);
                this.ddl_tp.Items.Add(items);
            }
            tipiRuolo = null;
        }

        private DocsPAWA.DocsPaWR.OrgTipoRuolo[] GetTipiRuolo()
        {
            string codiceAmministrazione = this.codAmm;
            return this.GetTipiRuolo(codiceAmministrazione);
        }

        /// <summary>
        /// Reperimento dei tipi ruolo
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.OrgTipoRuolo[] GetTipiRuolo(string codiceAmministrazione)
        {
            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            return ws.AmmGetTipiRuolo(codiceAmministrazione);
        }



    }
}