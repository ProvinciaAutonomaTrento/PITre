using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using DocsPAWA.SiteNavigation;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;
using System.Text;
using System.Collections;

namespace DocsPAWA.AdminTool.Gestione_Conservazione
{
    public partial class newPolicyStampePARER : System.Web.UI.Page
    {

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);
            if (!this.IsPostBack)
            {
                this.Initialize();
                this.SaveUserInSession();
                this.GetRepertori();
                this.GetAOO();
                this.GetRF();
                this.PopulateDdlDays(false);

                if (Request.QueryString["id"] != null && !string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    this.LoadPolicy();
                    this.ddl_dataStampa_E_SelectedIndexChanged(null, null);
                    this.titlePage.Text = "Dettaglio della Policy";
                    this.txtCodPolicy.Enabled = false;
                }

                this.rblRegType_SelectedIndexChanged(null, null);
            }
        }

        protected void BtnSavePolicy_Click(object sender, EventArgs e)
        {
            // Controllo e validazione campi
            bool verifica = this.VerificaCampi(true);

            if (verifica)
            {
                DocsPAWA.DocsPaWR.PolicyPARER policy = this.PopolaOggettoPolicy();

                if (Request.QueryString["s"] != null && Request.QueryString["s"].ToString().Equals("new"))
                {
                    // inserisco una nuova policy
                    if (this.WsInstance.InsertNewPolicyPARER(policy, this.GetInfoUser()))
                    {
                        if (Session["refreshGrid"] != null)
                            Session.Add("refreshGrid", "1");
                        else
                            Session["refreshGrid"] = "1";

                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save('Y');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "insert_error", "alert('Errore nell\\' inserimento della policy');", true);
                    }
                }
                else
                {
                    if (Request.QueryString["id"] != null && !string.IsNullOrEmpty(Request.QueryString["id"]))
                    {
                        policy.id = Request.QueryString["id"];
                    }
                    if (this.WsInstance.UpdatePolicyPARER(policy, this.GetInfoUser()))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save('Y');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "update_error", "alert('Errore nel salvataggio della policy');", true);
                    }
                }
            }
        }

        protected void BtnExecTest_Click(object sender, EventArgs e)
        {
            if (this.VerificaCampi(false))
            {
                DocsPAWA.DocsPaWR.PolicyPARER policy = this.PopolaOggettoPolicy();

                string result = this.WsInstance.GetCountDocumentiFromPolicy(policy, this.IdAmministrazione.ToString());
                if (!string.IsNullOrEmpty(result))
                {
                    string msg = string.Format("Sono stati ottenuti {0} risultati utilizzando i parametri di configurazione impostati", result);
                    if (!string.IsNullOrEmpty(this.LimiteDocVersamento))
                    {
                        int limite = 0;
                        if (Int32.TryParse(this.LimiteDocVersamento, out limite))
                        {
                            if (Convert.ToInt32(result) > limite)
                                msg = msg + "\\n" + "ATTENZIONE: il limite massimo di documenti versabili per l\\'amministrazione è " + limite;
                        }
                    }
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test_result", "alert('" + msg + "');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test_error", "alert('Attenzione, si è verificato un errore');", true);
                }
            }
        }

        protected void rblRegType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.rblRegType.SelectedValue.Equals("R"))
            {
                this.ddl_rep.Enabled = false;
                this.ddl_rep.SelectedIndex = 0;
            }
            if (this.rblRegType.SelectedValue.Equals("C"))
            {
                this.ddl_rep.Enabled = true;
            }
            this.UpdatePanelSelCrit.Update();
        }

        protected void ddl_dataStampa_E_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.ddl_dataStampa_E.SelectedIndex)
            {
                // Valore singolo
                case 0:
                    this.lblA.Visible = false;
                    this.lbl_dataStampaA.Visible = false;
                    this.lbl_dataStampaA.Text = string.Empty;

                    this.lblDa.Visible = true;
                    this.lblDa.Text = "Il";
                    this.lbl_dataStampaDa.Visible = true;

                    this.lblDaysPr.Visible = false;
                    this.txt_days_pr.Visible = false;
                    break;

                // Intervallo
                case 1:
                    this.lblA.Visible = true;
                    this.lbl_dataStampaA.Visible = true;

                    this.lblDa.Visible = true;
                    this.lblDa.Text = "Da";
                    this.lbl_dataStampaDa.Visible = true;

                    this.lblDaysPr.Visible = false;
                    this.txt_days_pr.Visible = false;
                    break;

                // Oggi, ieri
                case 2:
                case 6:
                    this.lblA.Visible = false;
                    this.lbl_dataStampaA.Visible = false;
                    this.lbl_dataStampaA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataStampaDa.Visible = false;
                    this.lbl_dataStampaDa.Text = string.Empty;

                    this.lblDaysPr.Visible = false;
                    this.txt_days_pr.Visible = false;
                    break;

                // Settimana corrente o precedente
                case 3:
                case 7:
                    this.lblA.Visible = false;
                    this.lbl_dataStampaA.Visible = false;
                    this.lbl_dataStampaA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataStampaDa.Visible = false;
                    this.lbl_dataStampaDa.Text = string.Empty;

                    this.lblDaysPr.Visible = false;
                    this.txt_days_pr.Visible = false;
                    break;

                // Mese corrente o precedente
                case 4:
                case 8:
                    this.lblA.Visible = false;
                    this.lbl_dataStampaA.Visible = false;
                    this.lbl_dataStampaA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataStampaDa.Visible = false;
                    this.lbl_dataStampaDa.Text = string.Empty;

                    this.lblDaysPr.Visible = false;
                    this.txt_days_pr.Visible = false;
                    break;

                // Anno corrente o precedente
                case 5:
                case 9:
                    this.lblA.Visible = false;
                    this.lbl_dataStampaA.Visible = false;
                    this.lbl_dataStampaA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataStampaDa.Visible = false;
                    this.lbl_dataStampaDa.Text = string.Empty;

                    this.lblDaysPr.Visible = false;
                    this.txt_days_pr.Visible = false;
                    break;

                // Numero giorni prima
                case 10:
                    this.lblA.Visible = false;
                    this.lbl_dataStampaA.Visible = false;
                    this.lbl_dataStampaA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataStampaDa.Visible = false;
                    this.lbl_dataStampaDa.Text = string.Empty;

                    this.lblDaysPr.Visible = true;
                    this.txt_days_pr.Visible = true;
                    break;
            }
        }

        protected void Periodicity_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            if (chk.Checked)
            {
                switch (chk.ID)
                {
                    case "chk_exec_daily":
                        this.chk_exec_daily.Checked = true;
                        this.chk_exec_weekly.Checked = false;
                        this.chk_exec_monthly.Checked = false;
                        this.chk_exec_yearly.Checked = false;
                        this.chk_exec_once.Checked = false;
                        this.ddl_weekday.Enabled = false;
                        this.ddl_day_month.Enabled = false;
                        this.ddl_day_year.Enabled = false;
                        this.ddl_month_year.Enabled = false;
                        this.lbl_cal_once.txt_Data.Enabled = false;
                        this.lbl_cal_once.EnableBtnCal = false;
                        break;

                    case "chk_exec_weekly":
                        this.chk_exec_daily.Checked = false;
                        this.chk_exec_weekly.Checked = true;
                        this.chk_exec_monthly.Checked = false;
                        this.chk_exec_yearly.Checked = false;
                        this.chk_exec_once.Checked = false;
                        this.ddl_weekday.Enabled = true;
                        this.ddl_day_month.Enabled = false;
                        this.ddl_day_year.Enabled = false;
                        this.ddl_month_year.Enabled = false;
                        this.lbl_cal_once.txt_Data.Enabled = false;
                        this.lbl_cal_once.EnableBtnCal = false;
                        break;

                    case "chk_exec_monthly":
                        this.chk_exec_daily.Checked = false;
                        this.chk_exec_weekly.Checked = false;
                        this.chk_exec_monthly.Checked = true;
                        this.chk_exec_yearly.Checked = false;
                        this.chk_exec_once.Checked = false;
                        this.ddl_weekday.Enabled = false;
                        this.ddl_day_month.Enabled = true;
                        this.ddl_day_year.Enabled = false;
                        this.ddl_month_year.Enabled = false;
                        this.lbl_cal_once.txt_Data.Enabled = false;
                        this.lbl_cal_once.EnableBtnCal = false;
                        break;

                    case "chk_exec_yearly":
                        this.chk_exec_daily.Checked = false;
                        this.chk_exec_weekly.Checked = false;
                        this.chk_exec_monthly.Checked = false;
                        this.chk_exec_yearly.Checked = true;
                        this.chk_exec_once.Checked = false;
                        this.ddl_weekday.Enabled = false;
                        this.ddl_day_month.Enabled = false;
                        this.ddl_day_year.Enabled = true;
                        this.ddl_month_year.Enabled = true;
                        this.lbl_cal_once.txt_Data.Enabled = false;
                        this.lbl_cal_once.EnableBtnCal = false;
                        break;

                    case "chk_exec_once":
                        this.chk_exec_daily.Checked = false;
                        this.chk_exec_weekly.Checked = false;
                        this.chk_exec_monthly.Checked = false;
                        this.chk_exec_yearly.Checked = false;
                        this.chk_exec_once.Checked = true;
                        this.ddl_weekday.Enabled = false;
                        this.ddl_day_month.Enabled = false;
                        this.ddl_day_year.Enabled = false;
                        this.ddl_month_year.Enabled = false;
                        this.lbl_cal_once.txt_Data.Enabled = true;
                        this.lbl_cal_once.EnableBtnCal = true;
                        break;

                }
            }
            else
            {
                this.ddl_weekday.Enabled = false;
                this.ddl_day_month.Enabled = false;
                this.ddl_day_year.Enabled = false;
                this.ddl_month_year.Enabled = false;
                this.lbl_cal_once.txt_Data.Enabled = false;
                this.lbl_cal_once.EnableBtnCal = false;
            }
        }

        protected void ddl_month_year_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PopulateDdlDays(true);
        }

        protected void txtCodRuoloResp_TextChanged(object sender, EventArgs e)
        {

        }

        protected void btnRubricaRuoloResp_Click(object sender, EventArgs e)
        {
            // Se è stato selezionato un corrispondente, ne vengono visualizzate le informazioni
            if (Session["RuoloResponsabile"] != null && Session["RuoloResponsabile"] is ElementoRubrica[])
            {
                ElementoRubrica el = ((ElementoRubrica[])Session["RuoloResponsabile"])[0];

                // Recupero del dettaglio del corrispondente
                Ruolo corr = UserManager.getCorrispondenteByCodRubricaIE(
                    this.Page,
                    el.codice,
                    DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO) as Ruolo;

                this.txtCodRuoloResp.Text = corr.codiceRubrica;
                this.txtDescRuoloResp.Text = corr.descrizione;
                this.id_corr_resp.Value = corr.idGruppo;

                Session.Remove("RuoloResponsabile");

                this.UpdatePanelRoleResp.Update();
            }
        }

        #endregion

        #region Methods

        protected void Initialize()
        {
            this.btnApriRubricaRole.Attributes.Add("onmouseover", "this.src='../../images/proto/rubrica_hover.gif'");
            this.btnApriRubricaRole.Attributes.Add("onmouseout", "this.src='../../images/proto/rubrica.gif'");
            this.btnApriRubricaRole.OnClientClick = String.Format("OpenAddressBook();");

            this.LimiteDocVersamento = DocsPAWA.utils.InitConfigurationKeys.GetValue(this.IdAmministrazione.ToString(), "FE_MAX_DOC_VERSAMENTO");

            this.lblA.Visible = false;
            this.lbl_dataStampaA.Visible = false;

            this.lbl_cal_once.txt_Data.Enabled = false;
        }

        private void SaveUserInSession()
        {
            DocsPaWebService ws = new DocsPaWebService();
            ws.Timeout = System.Threading.Timeout.Infinite;
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            string idAmministrazione = ws.getIdAmmByCod(codiceAmministrazione);

            DocsPAWA.DocsPaWR.Utente ut = new DocsPAWA.DocsPaWR.Utente();
            ut.codiceAmm = codiceAmministrazione;
            ut.idAmministrazione = idAmministrazione;
            ut.tipoIE = "I";

            //ut.idRegistro = idRegistro;

            Session.Add("userData", ut);

            DocsPAWA.DocsPaWR.Ruolo rl = new DocsPAWA.DocsPaWR.Ruolo();
            rl.codiceAmm = codiceAmministrazione;
            rl.idAmministrazione = idAmministrazione;
            rl.tipoIE = "I";

            //rl.idRegistro = idRegistro;

            rl.systemId = idAmministrazione;
            rl.uo = new DocsPAWA.DocsPaWR.UnitaOrganizzativa();
            rl.uo.codiceRubrica = codiceAmministrazione;
            Session.Add("userRuolo", rl);

            DocsPAWA.DocsPaWR.Registro reg = new DocsPAWA.DocsPaWR.Registro();

            //reg = ws.GetRegistroBySistemId(idRegistro);

            Session.Add("userRegistro", reg);
        }

        protected void GetRepertori()
        {
            RegistroRepertorio[] rep = RegistriRepertorioUtils.GetRegisteredRegistries(this.IdAmministrazione.ToString());
            if (rep != null && rep.Length > 0)
            {
                ddl_rep.Items.Clear();
                ddl_rep.Items.Add("");
                for (int i = 0; i < rep.Length; i++)
                {
                    string descrizione = rep[i].TipologyDescription;
                    ddl_rep.Items.Add(descrizione);
                    ddl_rep.Items[i + 1].Value = rep[i].CounterId;
                }
            }
        }

        protected void GetAOO()
        {
            Registro[] reg = WsInstance.GetRfByIdAmm(IdAmministrazione, "0");
            if (reg != null && reg.Length > 0)
            {
                ddl_reg_aoo.Items.Clear();
                ddl_reg_aoo.Items.Add("");
                for (int i = 0; i < reg.Length; i++)
                {
                    string descrizione = "[" + reg[i].codice + "]" + " - " + reg[i].descrizione;
                    ddl_reg_aoo.Items.Add(descrizione);
                    ddl_reg_aoo.Items[i + 1].Value = reg[i].systemId;
                }
            }
        }

        protected void GetRF()
        {
            Registro[] rf = WsInstance.GetRfByIdAmm(IdAmministrazione, "1");
            if (rf != null && rf.Length > 0)
            {
                ddl_rf.Items.Clear();
                ddl_rf.Items.Add("");
                for (int i = 0; i < rf.Length; i++)
                {
                    string descrizione = "[" + rf[i].codice + "]" + " - " + rf[i].descrizione;
                    ddl_rf.Items.Add(descrizione);
                    ddl_rf.Items[i + 1].Value = rf[i].systemId;
                }
            }
        }

        protected bool VerificaCampi(bool save)
        {
            // Campi da verificare solo in caso di salvataggio policy
            if (save)
            {
                // codice
                if (string.IsNullOrEmpty(this.txtCodPolicy.Text))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_cod_policy", "alert('Il campo \"Codice\" è obbligatorio');", true);
                    return false;
                }
                // lunghezza codice
                if (this.txtCodPolicy.Text.Length > 16)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "long_cod_policy", "alert('Il codice non può superare i 16 caratteri.');", true);
                    return false;
                }
                // univocità codice
                if (this.txtCodPolicy.Enabled)
                {
                    if (this.listaPolicy != null && this.listaPolicy.Length > 0)
                    {
                        foreach (PolicyPARER p in this.listaPolicy)
                        {
                            if (txtCodPolicy.Text.ToUpper().Equals(p.codice.ToUpper()))
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "exist_cod_policy", "alert('Codice policy già presente');", true);
                                return false;
                            }
                        }
                    }
                }
                // descrizione
                if (string.IsNullOrEmpty(this.txtDescrPolicy.Text))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_descr_policy", "alert('Il campo \"Descrizione\" è obbligatorio');", true);
                    return false;
                }
                // periodicità
                if (!(this.chk_exec_daily.Checked || this.chk_exec_weekly.Checked || this.chk_exec_monthly.Checked || this.chk_exec_yearly.Checked || this.chk_exec_once.Checked))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_descr_policy", "alert('E\\' necessario impostare una periodicità');", true);
                    return false;
                }
                // periodicità una tantum
                if (this.chk_exec_once.Checked && string.IsNullOrEmpty(this.lbl_cal_once.Text))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_descr_policy", "alert('E\\' necessario impostare una data per l'esecuzione una tantum');", true);
                    return false;
                }
            }

            // Campi da verificare sia in caso di salvataggio che di test esecuzione

            // anno stampa
            if (!string.IsNullOrEmpty(this.txtYear.Text))
            {
                int anno;
                if (!Int32.TryParse(this.txtYear.Text, out anno) || anno < 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_descr_policy", "alert('Il campo \"Anno di stampa\" contiene un valore non valido');", true);
                    return false;
                }
            }

            // controllo filtro data stampa intervallo
            if (this.ddl_dataStampa_E.SelectedValue.Equals("R") && !string.IsNullOrEmpty(this.lbl_dataStampaDa.Text) && !string.IsNullOrEmpty(this.lbl_dataStampaA.Text) && Utils.verificaIntervalloDate(this.lbl_dataStampaDa.Text, this.lbl_dataStampaA.Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "err_data_st", "alert('Intervallo data stampa non valido');", true);
                return false;
            }

            return true;
        }

        protected DocsPAWA.DocsPaWR.PolicyPARER PopolaOggettoPolicy()
        {
            DocsPAWA.DocsPaWR.PolicyPARER policy = new PolicyPARER();

            policy.codice = this.txtCodPolicy.Text;
            policy.descrizione = this.txtDescrPolicy.Text;
            policy.idAmm = this.IdAmministrazione.ToString();
            policy.tipo = "S";

            policy.statoVersamento = this.rbl_stato_versamento.SelectedValue;

            policy.tipoRegistroStampa = this.rblRegType.SelectedValue;
            policy.idRepertorio = this.ddl_rep.SelectedValue;
            policy.idRegistro = this.ddl_reg_aoo.SelectedValue;
            policy.idRF = this.ddl_rf.SelectedValue;
            policy.annoStampa = this.txtYear.Text;
            policy.filtroDataStampa = this.ddl_dataStampa_E.SelectedValue;
            if (this.ddl_dataStampa_E.SelectedValue.Equals("R"))
            {
                policy.dataStampaDa = this.lbl_dataStampaDa.Text;
                policy.dataStampaA = this.lbl_dataStampaA.Text;
            }
            else if (this.ddl_dataStampa_E.SelectedValue.Equals("S"))
            {
                policy.dataStampaDa = this.lbl_dataStampaDa.Text;
                policy.dataStampaA = string.Empty;
            }
            else
            {
                policy.dataStampaDa = string.Empty;
                policy.dataStampaA = string.Empty;
            }
            if (this.ddl_dataStampa_E.SelectedValue.Equals("P"))
                policy.numGiorniStampa = this.txt_days_pr.Text;
            else
                policy.numGiorniStampa = string.Empty;

            if (this.chk_exec_daily.Checked)
                policy.periodicita = "D";
            else if (this.chk_exec_weekly.Checked)
            {
                policy.periodicita = "W";
                policy.giornoEsecuzione = this.ddl_weekday.SelectedValue;
            }
            else if (this.chk_exec_monthly.Checked)
            {
                policy.periodicita = "M";
                policy.giornoEsecuzione = this.ddl_day_month.SelectedValue;
            }
            else if (this.chk_exec_yearly.Checked)
            {
                policy.periodicita = "Y";
                policy.giornoEsecuzione = this.ddl_day_year.SelectedValue;
                policy.meseEsecuzione = this.ddl_month_year.SelectedValue;
            }
            else if (this.chk_exec_once.Checked)
            {
                policy.periodicita = "O";
                policy.dataEsecuzione = this.lbl_cal_once.Text;
            }

            policy.idGruppoRuoloResp = this.id_corr_resp.Value;


            return policy;
        }

        protected void LoadPolicy()
        {
            string idPolicy = Request.QueryString["id"].ToString();
            PolicyPARER policy = this.WsInstance.GetPolicyPARERById(idPolicy);

            if (policy != null)
            {
                this.txtCodPolicy.Text = policy.codice;
                this.txtDescrPolicy.Text = policy.descrizione;

                // RUOLO RESPONSABILE
                if (!string.IsNullOrEmpty(policy.idGruppoRuoloResp))
                {
                    Ruolo ruoloResp = UserManager.getRuoloByIdGruppo(policy.idGruppoRuoloResp, this.Page);
                    if (ruoloResp != null)
                    {
                        this.txtCodRuoloResp.Text = ruoloResp.codiceRubrica;
                        this.txtDescRuoloResp.Text = ruoloResp.descrizione;
                        this.id_corr_resp.Value = policy.idGruppoRuoloResp;
                    }
                }

                // STATO CONSERVAZIONE
                this.rbl_stato_versamento.SelectedValue = policy.statoVersamento;

                // TIPO REGISTRO
                if (!string.IsNullOrEmpty(policy.tipoRegistroStampa))
                    this.rblRegType.SelectedValue = policy.tipoRegistroStampa;

                // REPERTORI, AOO, RF
                if (!string.IsNullOrEmpty(policy.idRepertorio))
                    this.ddl_rep.SelectedValue = policy.idRepertorio;
                if (!string.IsNullOrEmpty(policy.idRegistro))
                    this.ddl_reg_aoo.SelectedValue = policy.idRegistro;
                if (!string.IsNullOrEmpty(policy.idRF))
                    this.ddl_rf.SelectedValue = policy.idRF;

                // ANNO E DATA STAMPA
                this.txtYear.Text = policy.annoStampa;
                if (!string.IsNullOrEmpty(policy.filtroDataStampa))
                {
                    this.ddl_dataStampa_E.SelectedValue = policy.filtroDataStampa;
                    if (policy.filtroDataStampa.Equals("R"))
                    {
                        this.lbl_dataStampaDa.Visible = true;
                        this.lbl_dataStampaA.Visible = true;
                        this.lblDa.Text = "Da";
                        this.lblDa.Visible = true;
                        this.lblA.Visible = true;
                    }
                    else if (policy.filtroDataStampa.Equals("S"))
                    {
                        this.lbl_dataStampaDa.Visible = true;
                        this.lbl_dataStampaA.Visible = false;
                        this.lblDa.Text = "Il";
                        this.lblDa.Visible = true;
                        this.lblA.Visible = false;
                    }
                    else
                    {
                        this.lbl_dataStampaDa.Visible = false;
                        this.lbl_dataStampaA.Visible = false;
                        this.lblDa.Visible = false;
                        this.lblA.Visible = false;
                    }
                    if (!string.IsNullOrEmpty(policy.numGiorniStampa))
                        this.txt_days_pr.Text = policy.numGiorniStampa;
                }
                if (!string.IsNullOrEmpty(policy.dataStampaDa))
                    this.lbl_dataStampaDa.Text = policy.dataStampaDa;
                if (!string.IsNullOrEmpty(policy.dataStampaA))
                    this.lbl_dataStampaA.Text = policy.dataStampaA;

                // PERIODICITA'
                switch (policy.periodicita)
                {
                    case "D":
                        this.chk_exec_daily.Checked = true;
                        this.Periodicity_CheckedChanged(this.chk_exec_daily, null);
                        break;
                    case "W":
                        this.chk_exec_weekly.Checked = true;
                        this.Periodicity_CheckedChanged(this.chk_exec_weekly, null);
                        this.ddl_weekday.SelectedValue = policy.giornoEsecuzione;
                        break;
                    case "M":
                        this.chk_exec_monthly.Checked = true;
                        this.Periodicity_CheckedChanged(this.chk_exec_monthly, null);
                        this.ddl_day_month.SelectedValue = policy.giornoEsecuzione;
                        break;
                    case "Y":
                        this.chk_exec_yearly.Checked = true;
                        this.Periodicity_CheckedChanged(this.chk_exec_yearly, null);
                        this.ddl_day_year.SelectedValue = policy.giornoEsecuzione;
                        this.ddl_month_year.SelectedValue = policy.meseEsecuzione;
                        break;
                    case "O":
                        this.chk_exec_once.Checked = true;
                        this.Periodicity_CheckedChanged(this.chk_exec_once, null);
                        this.lbl_cal_once.Text = policy.dataEsecuzione;
                        break;
                }

                // Se la policy è attiva il tasto "salva" deve essere disabilitato
                if (!string.IsNullOrEmpty(policy.isAttiva) && policy.isAttiva.Equals("1"))
                {
                    this.btn_salva.Enabled = false;
                    this.btn_salva.ToolTip = "La policy è attualmente attiva e pertanto non può essere modificata";
                }
                else
                {
                    this.btn_salva.Enabled = true;
                }

            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "getPolicyError", "getPolicyError();", true);
            }
        }

        protected void PopulateDdlDays(bool update)
        {
            if (!update)
            {
                // Primo caricamento
                for (int i = 1; i <= 31; i++)
                {
                    this.ddl_day_month.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    this.ddl_day_year.Items.Add(new ListItem(i.ToString(), i.ToString()));                    
                }
                this.ddl_day_month.Items.FindByValue("31").Text = "Fine mese";
            }
            else
            {
                // Aggiornamento in seguito a selezione mese
                int currValue = Convert.ToInt32(ddl_day_year.SelectedValue);
                ddl_day_year.Items.Clear();
                int days = 31;
                int month = Convert.ToInt32(ddl_month_year.SelectedValue);

                if (month.Equals(4) || month.Equals(6) || month.Equals(9) || month.Equals(11))
                    days = 30;
                if (month.Equals(2))
                    days = 28;

                for (int i = 1; i <= days; i++)
                {
                    ListItem item = new ListItem();
                    item.Value = i.ToString();
                    item.Text = i.ToString();
                    ddl_day_year.Items.Add(item);
                }
                if (currValue > ddl_day_year.Items.Count)
                    ddl_day_year.SelectedIndex = ddl_day_year.Items.Count - 1;
                else
                    ddl_day_year.SelectedIndex = currValue - 1;
            }
        }

        protected InfoUtente GetInfoUser()
        {
            DocsPAWA.AdminTool.Manager.SessionManager manager = new Manager.SessionManager();
            InfoUtente result = manager.getUserAmmSession();
            result.idAmministrazione = this.IdAmministrazione.ToString();
            return result;
        }

        #endregion

        #region Properties

        private DocsPaWR.DocsPaWebService _wsInstance = null;

        protected DocsPaWR.DocsPaWebService WsInstance
        {
            get
            {
                if (this._wsInstance == null)
                    this._wsInstance = new DocsPAWA.DocsPaWR.DocsPaWebService();
                return this._wsInstance;
            }
        }

        /// <summary>
        /// Reperimento idamministrazione corrente
        /// </summary>
        protected int IdAmministrazione
        {
            get
            {
                string idAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                return Convert.ToInt32(idAmministrazione);
            }
        }

        protected string LimiteDocVersamento
        {
            get
            {
                return HttpContext.Current.Session["limiteDocVersamento"] as string;
            }
            set
            {
                HttpContext.Current.Session["limiteDocVersamento"] = value;
            }
        }

        protected PolicyPARER[] listaPolicy
        {
            get
            {
                return HttpContext.Current.Session["listaPolicy"] as PolicyPARER[];
            }
        } 

        #endregion

    }
}