using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using DocsPAWA.SiteNavigation;
using DocsPAWA.DocsPaWR;
using System.Text;
using System.Collections;

namespace DocsPAWA.AdminTool.Gestione_Conservazione
{
    public partial class newPolicyDocPARER : System.Web.UI.Page
    {

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);
            if (!IsPostBack)
            {
                this.Initialize();
                this.ClearSession();
                this.SaveUserInSession();
                this.GetTypeDocument();
                this.GetRf();
                this.GetAOO();
                this.GetTitolariUtilizzabili();
                this.PopulateDdlDays(false);

                if (Request.QueryString["id"] != null && !string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    this.LoadPolicy();
                    this.ddl_dataCreazione_E_SelectedIndexChanged(null, null);
                    this.ddl_dataProt_E_SelectedIndexChanged(null, null);
                    this.titlePage.Text = "Dettaglio della Policy";
                    this.txtCodPolicy.Enabled = false;
                }

                this.GestioneCampiTipoProto(null, null);
            }
            this.UpdateFormatDocument();
            this.UpdateCampiUO();
            this.UpdateSelectedTemplate();
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (this.FascicoliSelezionati != null && this.FascicoliSelezionati.Length == 1 && !string.IsNullOrEmpty(is_fasc.Value))
            {
                txtCodFascicolo.Text = this.FascicoliSelezionati[0].codice;
                txtDescFascicolo.Text = this.FascicoliSelezionati[0].descrizione;
                this.id_Fasc.Value = this.is_fasc.Value;
                this.is_fasc.Value = string.Empty;

                if (!string.IsNullOrEmpty(this.FascicoliSelezionati[0].idTitolario))
                {
                    ListItem temp = new ListItem();
                    temp.Value = this.FascicoliSelezionati[0].idTitolario;
                    if (this.ddl_titolari.Items.FindByValue(temp.Value) != null)
                    {
                        this.ddl_titolari.SelectedValue = this.FascicoliSelezionati[0].idTitolario;
                    }
                }
                this.FascicoliSelezionati = null;
                this.chk_tipo_class.Enabled = true;
                this.rbl_1.Selected = false;
                this.rbl_2.Selected = false;
                this.rbl_3.Selected = false;
            }
        }

        protected void BtnSavePolicy_Click(object sender, EventArgs e)
        {
            // Controllo e validazione campi
            bool verifica = this.verificaCampi(true);

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
            if (this.verificaCampi(false))
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

        protected void GestioneCampiTipoProto(object sender, EventArgs e)
        {
            if (this.chk_tipo.Items[0].Selected || this.chk_tipo.Items[1].Selected || this.chk_tipo.Items[2].Selected)
            {
                // Almeno un tipo doc protocollato selezionato
                this.ddl_dataProt_E.Enabled = true;
                this.lbl_dataCreazioneDaP.btn_Cal.Enabled = true;
                this.lbl_dataCreazioneDaP.ReadOnly = false;
                this.lbl_dataCreazioneDaP.txt_Data.Enabled = true;

                this.lbl_dataCreazioneAP.ReadOnly = false;

            }
            else
            {
                // Solo documenti non protocollati
                this.ddl_dataProt_E.Enabled = false;
                this.lbl_dataCreazioneDaP.btn_Cal.Enabled = false;
                this.lbl_dataCreazioneDaP.ReadOnly = true;
                this.lbl_dataCreazioneDaP.txt_Data.Enabled = false;

                this.lbl_dataCreazioneAP.ReadOnly = true;

            }

            this.UpdatePanelSelCrit.Update();
        }

        protected void ChangeTypeDocument(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddl_type_documents.SelectedValue) && !(ddl_type_documents.SelectedValue.Equals("0") || ddl_type_documents.SelectedValue.Equals("-1")))
            {
                TemplateLite template = this.Template[ddl_type_documents.SelectedValue];
                if (!string.IsNullOrEmpty(template.idDiagram))
                {
                    DiagrammaStato diagramma = WsInstance.getDiagrammaById(template.idDiagram);
                    if (diagramma != null && diagramma.STATI.Length > 0)
                    {
                        ddl_state_document.Items.Clear();
                        ddl_state_document.Items.Add("");
                        ddl_state_document.Enabled = true;
                        ddl_state_document_op.Enabled = true;
                        for (int i = 0; i < diagramma.STATI.Length; i++)
                        {
                            ddl_state_document.Items.Add(diagramma.STATI[i].DESCRIZIONE);
                            ddl_state_document.Items[i + 1].Value = (diagramma.STATI[i].SYSTEM_ID).ToString();
                        }
                    }
                    else
                    {
                        ddl_state_document.Items.Clear();
                        ddl_state_document.Enabled = false;
                        ddl_state_document_op.Enabled = false;
                    }
                }
                else
                {
                    ddl_state_document.Items.Clear();
                    ddl_state_document.Enabled = false;
                    ddl_state_document_op.Enabled = false;
                }
                this.btnCampiProfilati.OnClientClick = String.Format("OpenCampiProfilati('" + ddl_type_documents.SelectedValue + "');");
                this.btnCampiProfilati.Enabled = true;
                this.TemplateProf = null;
            }
            else
            {
                ddl_state_document.Items.Clear();
                ddl_state_document.Enabled = false;
                ddl_state_document_op.Enabled = false;
                this.btnCampiProfilati.Enabled = false;
                this.TemplateProf = null;
            }
            this.UpdatePanelSelCrit.Update();
        }

        protected void txtCodFascicolo_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtCodFascicolo.Text))
            {
                this.SearchProject();
            }
            else
            {
                this.FascicoliSelezionati = null;
                txtCodFascicolo.Text = string.Empty;
                txtDescFascicolo.Text = string.Empty;
                this.id_Fasc.Value = string.Empty;
                this.is_fasc.Value = string.Empty;
                this.chk_tipo_class.Enabled = false;
                this.rbl_1.Selected = false;
                this.rbl_2.Selected = false;
                this.rbl_3.Selected = false;
                //this.chk_includiSottoNodi.Enabled = false;
            }

            this.UpdatePanelSelCrit.Update();
        }

        protected void txtCodRuoloResp_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCodRuoloResp.Text))
            {
                setDescCorr(txtCodRuoloResp.Text);
            }
            else
            {
                txtCodRuoloResp.Text = string.Empty;
                txtDescRuoloResp.Text = string.Empty;
                id_corr_resp.Value = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole_resp", "alert('Inserire un codice da cercare in rubrica');", true);
            }

            this.UpdatePanelRoleResp.Update();
        }

        protected void txtCodUO_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCodUO.Text))
            {
                setDescCorrUO(txtCodUO.Text);
            }
            else
            {
                txtCodUO.Text = string.Empty;
                txtDescUO.Text = string.Empty;
                id_corr_uo.Value = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_uo_code", "alert('Inserire un codice da cercare in rubrica');", true);
            }
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

        protected void btnRubricaUO_Click(object sender, EventArgs e)
        {
            // Se è stato selezionato un corrispondente, ne vengono visualizzate le informazioni
            if (Session["rubrica.corrispondenteSelezionatoRuoloSottoposto"] != null && Session["rubrica.corrispondenteSelezionatoRuoloSottoposto"] is ElementoRubrica[])
            {
                ElementoRubrica el = ((ElementoRubrica[])Session["rubrica.corrispondenteSelezionatoRuoloSottoposto"])[0];

                // Recupero del dettaglio del corrispondente
                Ruolo corr = UserManager.getCorrispondenteByCodRubricaIE(
                    this.Page,
                    el.codice,
                    DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO) as Ruolo;

                this.txtCodUO.Text = corr.codiceRubrica;
                this.txtDescUO.Text = corr.descrizione;
                this.id_corr_uo.Value = corr.systemId;

                Session.Remove("rubrica.corrispondenteSelezionatoRuoloSottoposto");

                this.UpdatePanelSelCrit.Update();
            }
        }

        protected void ddl_reg_aoo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.FascicoliSelezionati = null;
            txtCodFascicolo.Text = string.Empty;
            txtDescFascicolo.Text = string.Empty;
            this.id_Fasc.Value = string.Empty;
            this.is_fasc.Value = string.Empty;
            this.chk_tipo_class.Enabled = false;
            this.rbl_1.Selected = false;
            this.rbl_2.Selected = false;
            this.rbl_3.Selected = false;
        }

        protected void ddl_dataCreazione_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataCreazione_E.SelectedIndex)
            {
                // Valore singolo
                case 0:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = true;
                    this.lbl_dataCreazioneDa.Visible = true;
                    this.lblDa.Text = "Il";

                    this.lblDaysCr.Visible = false;
                    this.txt_days_cr.Visible = false;
                    break;

                // Intervallo
                case 1:
                    this.lblA.Visible = true;
                    this.lbl_dataCreazioneA.Visible = true;

                    this.lblDa.Visible = true;
                    this.lbl_dataCreazioneDa.Visible = true;
                    this.lblDa.Text = "Da";

                    this.lblDaysCr.Visible = false;
                    this.txt_days_cr.Visible = false;
                    break;

                // Oggi, ieri
                case 2:
                case 6:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;

                    this.lblDaysCr.Visible = false;
                    this.txt_days_cr.Visible = false;
                    break;

                // Settimana corrente o precedente
                case 3:
                case 7:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;

                    this.lblDaysCr.Visible = false;
                    this.txt_days_cr.Visible = false;
                    break;

                // Mese corrente o precedente
                case 4:
                case 8:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;

                    this.lblDaysCr.Visible = false;
                    this.txt_days_cr.Visible = false;
                    break;

                // Anno corrente o precedente
                case 5:
                case 9:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;

                    this.lblDaysCr.Visible = false;
                    this.txt_days_cr.Visible = false;
                    break;

                // n giorni fa
                case 10:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;

                    this.lblDaysCr.Visible = true;
                    this.txt_days_cr.Visible = true;
                    break;
            }

            this.UpdatePanelSelCrit.Update();
        }

        protected void ddl_dataProt_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataProt_E.SelectedIndex)
            {
                // Valore singolo
                case 0:
                    this.lblAP.Visible = false;
                    this.lbl_dataCreazioneAP.Visible = false;
                    this.lbl_dataCreazioneAP.txt_Data.Text = string.Empty;

                    this.lblDaP.Visible = true;
                    this.lbl_dataCreazioneDaP.Visible = true;
                    this.lblDa.Text = "Il";

                    this.lblDaysPr.Visible = false;
                    this.txt_days_pr.Visible = false;
                    break;

                // Intervallo
                case 1:
                    this.lblAP.Visible = true;
                    this.lbl_dataCreazioneAP.Visible = true;


                    this.lblDaP.Visible = true;
                    this.lbl_dataCreazioneDaP.Visible = true;
                    this.lblDa.Text = "Da";

                    this.lblDaysPr.Visible = false;
                    this.txt_days_pr.Visible = false;
                    break;

                // Oggi, ieri
                case 2:
                case 6:
                    this.lblAP.Visible = false;
                    this.lbl_dataCreazioneAP.Visible = false;
                    this.lbl_dataCreazioneAP.Text = string.Empty;

                    this.lblDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Text = string.Empty;

                    this.lblDaysPr.Visible = false;
                    this.txt_days_pr.Visible = false;
                    break;

                // Settimana corrente o precedente
                case 3:
                case 7:
                    this.lblAP.Visible = false;
                    this.lbl_dataCreazioneAP.Visible = false;
                    this.lbl_dataCreazioneAP.Text = string.Empty;

                    this.lblDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Text = string.Empty;

                    this.lblDaysPr.Visible = false;
                    this.txt_days_pr.Visible = false;
                    break;

                // Mese corrente o precedente
                case 4:
                case 8:
                    this.lblAP.Visible = false;
                    this.lbl_dataCreazioneAP.Visible = false;
                    this.lbl_dataCreazioneAP.Text = string.Empty;

                    this.lblDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Text = string.Empty;

                    this.lblDaysPr.Visible = false;
                    this.txt_days_pr.Visible = false;
                    break;

                // Anno corrente o precedente
                case 5:
                case 9:
                    this.lblAP.Visible = false;
                    this.lbl_dataCreazioneAP.Visible = false;
                    this.lbl_dataCreazioneAP.Text = string.Empty;

                    this.lblDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Text = string.Empty;

                    this.lblDaysPr.Visible = false;
                    this.txt_days_pr.Visible = false;
                    break;

                // Numero giorni prima
                case 10:
                    this.lblAP.Visible = false;
                    this.lbl_dataCreazioneAP.Visible = false;
                    this.lbl_dataCreazioneAP.Text = string.Empty;

                    this.lblDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Text = string.Empty;

                    this.lblDaysPr.Visible = true;
                    this.txt_days_pr.Visible = true;
                    break;
            }

            this.UpdatePanelSelCrit.Update();
        }

        protected void ddl_month_year_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PopulateDdlDays(true);
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

        #endregion

        #region Methods

        protected void Initialize()
        {

            this.btnApriRubricaUO.Attributes.Add("onmouseover", "this.src='../../images/proto/rubrica_hover.gif");
            this.btnApriRubricaUO.Attributes.Add("onmouseout", "this.src='../../images/proto/rubrica.gif'");
            this.btnApriRubricaUO.OnClientClick = String.Format("_ApriRubricaRicercaRuoliSottoposti();");
            this.btnApriRubricaRole.Attributes.Add("onmouseover", "this.src='../../images/proto/rubrica_hover.gif'");
            this.btnApriRubricaRole.Attributes.Add("onmouseout", "this.src='../../images/proto/rubrica.gif'");
            this.btnApriRubricaRole.OnClientClick = String.Format("OpenAddressBook();");
            this.btn_img_doc.Attributes.Add("onmouseover", "this.src='../Images/icon_admin_up.gif'");
            this.btn_img_doc.Attributes.Add("onmouseout", "this.src='../Images/icon_admin.gif'");
            this.btn_img_doc.OnClientClick = String.Format("OpenDocumentFormat();");
            this.LimiteDocVersamento = DocsPAWA.utils.InitConfigurationKeys.GetValue(this.IdAmministrazione.ToString(), "FE_MAX_DOC_VERSAMENTO");

            this.lblA.Visible = false;
            this.lbl_dataCreazioneA.Visible = false;
            this.lblAP.Visible = false;
            this.lbl_dataCreazioneAP.Visible = false;

            this.lbl_cal_once.txt_Data.Enabled = false;

            //this.GestioneCampiTipoProto(null, null);
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

                this.chk_tipo.Items[0].Selected = policy.arrivo;
                this.chk_tipo.Items[1].Selected = policy.partenza;
                this.chk_tipo.Items[2].Selected = policy.interno;
                this.chk_tipo.Items[3].Selected = policy.grigio;

                // TIPOLOGIE
                if (!string.IsNullOrEmpty(policy.idTemplate))
                {
                    this.ddl_type_documents.SelectedValue = policy.idTemplate;
                    this.updateStateDiagram();
                    
                    this.btnCampiProfilati.Enabled = true;
                    this.btnCampiProfilati.OnClientClick = String.Format("OpenCampiProfilati('" + policy.idTemplate + "');");
                    if (policy.template != null)
                        this.TemplateProf = policy.template;

                    if (!string.IsNullOrEmpty(policy.idStato))
                        this.ddl_state_document.SelectedValue = policy.idStato;
                    if (!string.IsNullOrEmpty(policy.operatoreStato))
                        this.ddl_state_document_op.SelectedValue = policy.operatoreStato;
                }

                // REGISTRO, RF, UO
                if (!string.IsNullOrEmpty(policy.idRegistro))
                    this.ddl_reg_aoo.SelectedValue = policy.idRegistro;
                if (!string.IsNullOrEmpty(policy.idRF))
                    this.ddl_rf.SelectedValue = policy.idRF;
                if (!string.IsNullOrEmpty(policy.idUO))
                {
                    Corrispondente corr = UserManager.getCorrispondenteBySystemIDDisabled(this.Page, policy.idUO);
                    this.txtCodUO.Text = corr.codiceRubrica;
                    this.txtDescUO.Text = corr.descrizione;
                    this.id_corr_uo.Value = corr.systemId;
                }
                this.chk_uo_sottoposte.Checked = (!string.IsNullOrEmpty(policy.UOsottoposte) && policy.UOsottoposte.Equals("1"));

                // CLASSIFICAZIONE
                if(!string.IsNullOrEmpty(policy.idTitolario) && policy.idTitolario != "0")
                    this.ddl_titolari.SelectedValue = policy.idTitolario;
                else if (!string.IsNullOrEmpty(policy.idTitolario) && policy.idTitolario.Equals("0"))
                {
                    try
                    {
                        this.ddl_titolari.SelectedValue = this.ddl_titolari.Items.FindByText("Tutti i titolari").Value;                       
                    }
                    catch (Exception ex)
                    {
                        this.ddl_titolari.SelectedIndex = 0;
                    }
                }
                
                if (!string.IsNullOrEmpty(policy.idFascicolo))
                {
                    Fascicolo f = FascicoliManager.getFascicoloByIdNoSecurity(policy.idFascicolo);
                    this.txtCodFascicolo.Text = f.codice;
                    this.txtDescFascicolo.Text = f.descrizione;
                    this.id_Fasc.Value = f.systemID;
                    this.chk_tipo_class.Enabled = true;
                    if (!string.IsNullOrEmpty(policy.tipoClassificazione))
                        this.chk_tipo_class.SelectedValue = policy.tipoClassificazione;
                }


                this.chk_doc_digitali.Checked = policy.digitali.Equals("1");
                if (!string.IsNullOrEmpty(policy.firmati))
                    this.chk_firma.SelectedValue = policy.firmati;
                if (!string.IsNullOrEmpty(policy.marcati))
                    this.chk_timestamp.SelectedValue = policy.marcati;

                if (!string.IsNullOrEmpty(policy.scadenzaMarca))
                    this.ddl_timestamp_expiry.SelectedValue = policy.scadenzaMarca;

                if (!string.IsNullOrEmpty(policy.filtroDataCreazione))
                    this.ddl_dataCreazione_E.SelectedValue = policy.filtroDataCreazione;
                if (!string.IsNullOrEmpty(policy.dataCreazioneDa))
                    this.lbl_dataCreazioneDa.Text = policy.dataCreazioneDa;
                if (!string.IsNullOrEmpty(policy.dataCreazioneA))
                    this.lbl_dataCreazioneA.Text = policy.dataCreazioneA;
                if (!string.IsNullOrEmpty(policy.numGiorniCreazione))
                    this.txt_days_cr.Text = policy.numGiorniCreazione;

                if (!string.IsNullOrEmpty(policy.filtrodataProtocollazione))
                    this.ddl_dataProt_E.SelectedValue = policy.filtrodataProtocollazione;
                if (!string.IsNullOrEmpty(policy.dataProtocollazioneDa))
                    this.lbl_dataCreazioneDaP.Text = policy.dataProtocollazioneDa;
                if (!string.IsNullOrEmpty(policy.dataProtocollazioneA))
                    this.lbl_dataCreazioneAP.Text = policy.dataProtocollazioneA;
                if (!string.IsNullOrEmpty(policy.numGiorniProtocollazione))
                    this.txt_days_pr.Text = policy.numGiorniProtocollazione;

                if (policy.arrivo || policy.partenza || policy.interno)
                {
                    this.ddl_dataProt_E.Enabled = true;
                    this.lbl_dataCreazioneDaP.ReadOnly = false;
                    this.lbl_dataCreazioneAP.ReadOnly = false;
                }

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

                // FORMATI DOCUMENTO
                if (policy.FormatiDocumento != null && policy.FormatiDocumento.Length>0)
                {
                    this.SelectedFile = new Dictionary<string, string>();
                    string formati = string.Empty;
                    for (int i = 0; i < policy.FormatiDocumento.Length; i++)
                    {
                        formati = formati + policy.FormatiDocumento.ElementAt(i).FileExtension;
                        this.SelectedFile.Add(policy.FormatiDocumento.ElementAt(i).SystemId.ToString(), policy.FormatiDocumento.ElementAt(i).FileExtension);
                        if (i < policy.FormatiDocumento.Length)
                        {
                            formati = formati + " | ";
                        }
                    }

                    this.lbl_documents_format.Text = formati;
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

        protected void GetTypeDocument()
        {
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
            {
                this.rowTypeDocument.Visible = true;
                DocsPaWR.TemplateLite[] templateList;
                //this.pnl_profilazione.Visible = true;
                templateList = this.WsInstance.GetTypeDocumentsWithDiagramByIdAmm(IdAmministrazione, "D");
                if (templateList != null && templateList.Length > 0)
                {
                    ddl_type_documents.Items.Clear();
                    // Nessun valore - selezione di tutti i documenti
                    ddl_type_documents.Items.Add("");
                    ddl_type_documents.Items[0].Value = "0";
                    // Nessuna tipologia
                    ddl_type_documents.Items.Add("Nessuna tipologia");
                    ddl_type_documents.Items[1].Value = "-1";
                    this.Template = new Dictionary<string, TemplateLite>();
                    for (int i = 0; i < templateList.Length; i++)
                    {
                        ddl_type_documents.Items.Add(templateList[i].name);
                        ddl_type_documents.Items[i + 2].Value = templateList[i].system_id;
                        this.Template[templateList[i].system_id] = templateList[i];
                    }
                }
            }
            else
            {
                this.rowTypeDocument.Visible = false;
            }
        }

        protected void GetRf()
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

        protected void GetTitolariUtilizzabili()
        {
            ddl_titolari.Items.Clear();

            ArrayList listaTitolari = new ArrayList(this.WsInstance.getTitolariUtilizzabili(IdAmministrazione.ToString()));

            string titolarioAttivo = string.Empty;

            //Esistono dei titolari chiusi
            if (listaTitolari.Count > 1)
            {
                //Creo le voci della ddl dei titolari
                string valueTutti = string.Empty;

                foreach (OrgTitolario titolario in listaTitolari)
                {
                    ListItem it = null;
                    switch (titolario.Stato)
                    {
                        case OrgStatiTitolarioEnum.Attivo:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            ddl_titolari.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            titolarioAttivo = titolario.ID;
                            break;
                        case OrgStatiTitolarioEnum.Chiuso:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            ddl_titolari.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                    }
                }
                //Imposto la voce tutti i titolari
                valueTutti = valueTutti.Substring(0, valueTutti.Length - 1);
                if (valueTutti != string.Empty)
                {
                    if (valueTutti.IndexOf(',') == -1)
                        valueTutti = valueTutti + "," + valueTutti;

                    ListItem it = new ListItem("Tutti i titolari", valueTutti);
                    ddl_titolari.Items.Insert(0, it);
                }

            }
            //Non esistono titolario chiusi
            if (listaTitolari.Count == 1)
            {
                OrgTitolario titolario = (OrgTitolario)listaTitolari[0];
                if (titolario.Stato != OrgStatiTitolarioEnum.InDefinizione)
                {
                    ListItem it = new ListItem(titolario.Descrizione, titolario.ID);
                    ddl_titolari.Items.Add(it);
                }
                ddl_titolari.Enabled = false;
            }

            if (!string.IsNullOrEmpty(titolarioAttivo))
            {
                this.ddl_titolari.SelectedValue = titolarioAttivo;
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

        protected void SearchProject()
        {
            // Lista dei fascicoli restituiti dalla ricerca
            Fascicolo[] projectList;

            //projectList = FascicoliManager.GetFascicoloDaCodiceNoSecurity(this.txtCodFascicolo.Text, this.IdAmministrazione.ToString(), this.ddl_titolari.SelectedValue, true);
            string idRegistro = this.ddl_reg_aoo.SelectedValue;
            projectList = this.WsInstance.GetFascicoloDaCodiceNoSecurityConservazione(this.txtCodFascicolo.Text, this.IdAmministrazione.ToString(), this.ddl_titolari.SelectedValue, true, false, idRegistro);

            if (projectList == null || projectList.Length == 0)
            {
                this.FascicoliSelezionati = null;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_project", "alert('Nessun fascicolo trovato con questo codice');", true);
            }
            else
            {
                this.FascicoliSelezionati = null;
                if (projectList.Length == 1)
                {
                    txtCodFascicolo.Text = projectList[0].codice;
                    txtDescFascicolo.Text = projectList[0].descrizione;
                    this.id_Fasc.Value = projectList[0].systemID;
                    if (!string.IsNullOrEmpty(projectList[0].idTitolario))
                    {
                        ListItem temp = new ListItem();
                        temp.Value = projectList[0].idTitolario;
                        if (this.ddl_titolari.Items.FindByValue(temp.Value) != null)
                        {
                            this.ddl_titolari.SelectedValue = projectList[0].idTitolario;
                        }
                    }
                    this.chk_tipo_class.Enabled = true;
                    this.rbl_1.Selected = false;
                    this.rbl_2.Selected = false;
                    this.rbl_3.Selected = false;
                    //this.chk_includiSottoNodi.Enabled = true;
                }
                else
                {
                    this.FascicoliSelezionati = new Fascicolo[projectList.Length];
                    this.FascicoliSelezionati = projectList;

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_project", "OpenSceltaFascicoli();", true);
                    if (FascicoliSelezionati != null && FascicoliSelezionati.Length > 0 && FascicoliSelezionati[0] != null)
                    {
                        //txtCodFascicolo.Text = FascicoliSelezionati[0].codice;
                        //txtDescFascicolo.Text = FascicoliSelezionati[0].descrizione;
                    }
                    else
                    {
                        txtCodFascicolo.Text = string.Empty;
                        txtDescFascicolo.Text = string.Empty;
                        //this.chk_includiSottoNodi.Enabled = false;
                    }
                    this.id_Fasc.Value = string.Empty;
                    this.chk_tipo_class.Enabled = false;
                    this.rbl_1.Selected = false;
                    this.rbl_2.Selected = false;
                    this.rbl_3.Selected = false;
                }
            }
        }

        protected void setDescCorr(string codRubrica)
        {
            DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this.Page, codRubrica, AddressbookTipoUtente.INTERNO);

            if (corr == null)
            {
                txtCodRuoloResp.Text = string.Empty;
                txtDescRuoloResp.Text = string.Empty;
                id_corr_resp.Value = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_found", "alert('Corrispondente non trovato');", true);
            }
            else
            {
                if (!(corr.tipoCorrispondente.Equals("R")))
                {
                    txtCodRuoloResp.Text = string.Empty;
                    txtDescRuoloResp.Text = string.Empty;
                    id_corr_resp.Value = string.Empty;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_found", "alert('Inserire soltanto ruoli');", true);
                }
                else
                {
                    txtCodRuoloResp.Text = corr.codiceRubrica;
                    txtDescRuoloResp.Text = corr.descrizione;
                    id_corr_resp.Value = UserManager.getRuoloById(corr.systemId, this.Page).idGruppo;
                }
            }
        }

        protected void setDescCorrUO(string codRubrica)
        {
            DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this.Page, codRubrica, AddressbookTipoUtente.INTERNO);

            if (corr == null)
            {
                txtCodUO.Text = string.Empty;
                txtDescUO.Text = string.Empty;
                id_corr_uo.Value = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_found", "alert('Corrispondente non trovato');", true);
            }
            else
            {
                if (!(corr.tipoCorrispondente.Equals("U")))
                {
                    txtCodUO.Text = string.Empty;
                    txtDescUO.Text = string.Empty;
                    id_corr_uo.Value = string.Empty;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_found", "alert('Inserire soltanto UO');", true);
                }
                else
                {
                    txtCodUO.Text = corr.codiceRubrica;
                    txtDescUO.Text = corr.descrizione;
                    id_corr_uo.Value = corr.systemId;
                }
            }
        }

        private bool verificaCampi(bool save)
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
                if (this.txtCodPolicy.Enabled && this.listaPolicy != null)
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
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_descr_policy", "alert('E\\' necessario impostare una data per l\\'esecuzione una tantum');", true);
                    return false;
                }
            }

            // Campi da verificare sia in caso di salvataggio che di test esecuzione

            // controllo tipi documento
            if (!(this.chk_tipo.Items[0].Selected || this.chk_tipo.Items[1].Selected || this.chk_tipo.Items[2].Selected || this.chk_tipo.Items[3].Selected))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_tipo_proto", "alert('Selezionare almeno un tipo documento');", true);
                return false;
            }

            // controllo classificazione
            if (!string.IsNullOrEmpty(this.txtCodFascicolo.Text) && !string.IsNullOrEmpty(this.txtDescFascicolo.Text) && string.IsNullOrEmpty(this.chk_tipo_class.SelectedValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_tipo_class", "alert('E\\' necessario selezionare un valore per il tipo di classificazione');", true);
                return false;
            }

            // controllo file firmati
            if (!(this.chk_firma.Items[0].Selected || this.chk_firma.Items[1].Selected))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_chk_firma", "alert('Selezionare almeno un valore per il filtro \"Firme digitali\"');", true);
                return false;
            }

            // controllo file marcati
            if (!(this.chk_timestamp.Items[0].Selected || this.chk_timestamp.Items[1].Selected))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_chk_firma", "alert('Selezionare almeno un valore per il filtro \"Timestamp\"');", true);
                return false;
            }

            // controllo filtro data creazione intervallo
            if (this.ddl_dataCreazione_E.SelectedValue.Equals("R") && !string.IsNullOrEmpty(this.lbl_dataCreazioneDa.Text) && !string.IsNullOrEmpty(this.lbl_dataCreazioneA.Text) && Utils.verificaIntervalloDate(this.lbl_dataCreazioneDa.Text, this.lbl_dataCreazioneA.Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "err_data_cr", "alert('Intervallo data creazione non valido');", true);
                return false;
            }

            // controllo filtro data creazione n giorni prima
            if (this.ddl_dataCreazione_E.SelectedValue.Equals("P") && string.IsNullOrEmpty(this.txt_days_cr.Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_num_giorni_c", "alert('Inserire un valore nel campo \"Data creazione > Numero giorni\"');", true);
                return false;
            }

            // controllo su valore numerico campo n giorni prima
            if (!string.IsNullOrEmpty(this.txt_days_cr.Text))
            {
                int anno;
                if (!Int32.TryParse(this.txt_days_cr.Text, out anno) || anno < 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "n_giorni_c_not_valid", "alert('Il campo \"Data creazione > Numero giorni\" contiene un valore non valido');", true);
                    return false;
                }
            }

            // controllo filtro data protocollazione intervallo
            if (this.ddl_dataProt_E.SelectedValue.Equals("R") && !string.IsNullOrEmpty(this.lbl_dataCreazioneDaP.Text) && !string.IsNullOrEmpty(this.lbl_dataCreazioneAP.Text) && Utils.verificaIntervalloDate(this.lbl_dataCreazioneDaP.Text, this.lbl_dataCreazioneAP.Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "err_data_prot", "alert('Intervallo data protocollazione non valido');", true);
                return false;
            }

            // controllo filtro data protocollazione n giorni prima
            if (this.ddl_dataProt_E.SelectedValue.Equals("P") && string.IsNullOrEmpty(this.txt_days_pr.Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_num_giorni_p", "alert('Inserire un valore nel campo \"Data protocollazione > Numero giorni\"');", true);
                return false;
            }

            // controllo su valore numerico campo n giorni prima
            if (!string.IsNullOrEmpty(this.txt_days_pr.Text))
            {
                int anno;
                if (!Int32.TryParse(this.txt_days_pr.Text, out anno) || anno < 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "n_giorni_p_not_valid", "alert('Il campo \"Data protocollazione > Numero giorni\" contiene un valore non valido');", true);
                    return false;
                }
            }

            

            return true;
        }

        protected void UpdateFormatDocument()
        {

            string formatDocumet = string.Empty;
            if (this.SelectedFile != null && this.SelectedFile.Count > 0)
            {
                for (int i = 0; i < this.SelectedFile.Count; i++)
                {
                    formatDocumet = formatDocumet + this.SelectedFile.ElementAt(i).Value;
                    if (i < this.SelectedFile.Count - 1)
                    {
                        formatDocumet = formatDocumet + " | ";
                    }
                }
            }
            this.lbl_documents_format.Text = formatDocumet;
        }

        protected void updateStateDiagram()
        {
            if (!string.IsNullOrEmpty(ddl_type_documents.SelectedValue) && !(ddl_type_documents.SelectedValue.Equals("0") || ddl_type_documents.SelectedValue.Equals("-1")))
            {
                TemplateLite template = this.Template[ddl_type_documents.SelectedValue];
                if (!string.IsNullOrEmpty(template.idDiagram))
                {
                    DiagrammaStato diagramma = WsInstance.getDiagrammaById(template.idDiagram);
                    if (diagramma != null && diagramma.STATI.Length > 0)
                    {
                        ddl_state_document.Items.Clear();
                        ddl_state_document.Items.Add("");
                        ddl_state_document.Enabled = true;
                        ddl_state_document_op.Enabled = true;
                        for (int i = 0; i < diagramma.STATI.Length; i++)
                        {
                            ddl_state_document.Items.Add(diagramma.STATI[i].DESCRIZIONE);
                            ddl_state_document.Items[i + 1].Value = (diagramma.STATI[i].SYSTEM_ID).ToString();
                        }
                    }
                    else
                    {
                        ddl_state_document.Items.Clear();
                        ddl_state_document.Enabled = false;
                        ddl_state_document_op.Enabled = false;
                    }
                }
                else
                {
                    ddl_state_document.Items.Clear();
                    ddl_state_document.Enabled = false;
                    ddl_state_document_op.Enabled = false;
                }
            }
            else
            {
                ddl_state_document.Items.Clear();
                ddl_state_document.Enabled = false;
                ddl_state_document_op.Enabled = false;
            }
        }

        protected void UpdateCampiUO()
        {
            if (Session["rubrica.corrispondenteSelezionatoRuoloSottoposto"] != null) // && Session["rubrica.corrispondenteSelezionatoRuoloSottoposto"] is ElementoRubrica[])
            {
                DocsPaWR.Corrispondente corr = (DocsPaWR.Corrispondente)Session["rubrica.corrispondenteSelezionatoRuoloSottoposto"];

                if (corr.tipoCorrispondente.Equals("U"))
                {
                    this.txtCodUO.Text = corr.codiceRubrica;
                    this.txtDescUO.Text = corr.descrizione;
                    this.id_corr_uo.Value = corr.systemId;

                    Session.Remove("rubrica.corrispondenteSelezionatoRuoloSottoposto");
                }

                this.UpdatePanelSelCrit.Update();
            }
        }

        protected void UpdateSelectedTemplate()
        {
            if (!string.IsNullOrEmpty(ddl_type_documents.SelectedValue) && !(ddl_type_documents.SelectedValue.Equals("0") || ddl_type_documents.SelectedValue.Equals("-1")))
            {
                this.btnCampiProfilati.OnClientClick = String.Format("OpenCampiProfilati('" + ddl_type_documents.SelectedValue + "');");
                this.btnCampiProfilati.Enabled = true;
                this.TemplateMod = null;
            }
        }

        private DocsPAWA.DocsPaWR.PolicyPARER PopolaOggettoPolicy()
        {
            DocsPAWA.DocsPaWR.PolicyPARER policy = new PolicyPARER();

            policy.codice = this.txtCodPolicy.Text;
            policy.descrizione = this.txtDescrPolicy.Text;
            policy.idAmm = this.IdAmministrazione.ToString();
            policy.tipo = "D";

            policy.statoVersamento = this.rbl_stato_versamento.SelectedValue;

            policy.arrivo = this.chk_tipo.Items[0].Selected;
            policy.partenza = this.chk_tipo.Items[1].Selected;
            policy.interno = this.chk_tipo.Items[2].Selected;
            policy.grigio = this.chk_tipo.Items[3].Selected;

            policy.idTemplate = this.ddl_type_documents.SelectedValue;
            if (this.ddl_state_document.Enabled)
            {
                policy.idStato = this.ddl_state_document.SelectedValue;
                policy.operatoreStato = this.ddl_state_document_op.SelectedValue;
            }
            policy.idRegistro = this.ddl_reg_aoo.SelectedValue;
            policy.idRF = this.ddl_rf.SelectedValue;
            policy.idUO = this.id_corr_uo.Value;
            policy.UOsottoposte = this.chk_uo_sottoposte.Checked ? "1" : "0";
            policy.idTitolario = this.ddl_titolari.SelectedValue;
            if (!policy.idTitolario.IndexOf(",").Equals(-1))
                policy.idTitolario = "0";

            policy.tipoClassificazione = this.chk_tipo_class.SelectedValue != null ? this.chk_tipo_class.SelectedValue : string.Empty;
            policy.idFascicolo = this.id_Fasc.Value;

            if (this.ddl_state_document.Enabled)
                policy.idStato = this.ddl_state_document.SelectedValue;

            if (this.SelectedFile != null && this.SelectedFile.Count > 0)
            {
                policy.FormatiDocumento = new SupportedFileType[this.SelectedFile.Count];
                for (int i = 0; i < this.SelectedFile.Count; i++)
                {
                    SupportedFileType tempSupp = new SupportedFileType();
                    tempSupp.FileExtension = this.SelectedFile.ElementAt(i).Value;
                    tempSupp.SystemId = Convert.ToInt32(this.SelectedFile.ElementAt(i).Key);
                    policy.FormatiDocumento[i] = tempSupp;
                }
            }

            policy.digitali = this.chk_doc_digitali.Checked ? "1" : "0";
            if (this.chk_firma.Items[0].Selected && !this.chk_firma.Items[1].Selected)
                policy.firmati = "1";
            else if (this.chk_firma.Items[1].Selected && !this.chk_firma.Items[0].Selected)
                policy.firmati = "0";
            else
                policy.firmati = string.Empty;

            if (this.chk_timestamp.Items[0].Selected && !this.chk_timestamp.Items[1].Selected)
                policy.marcati = "1";
            else if (this.chk_timestamp.Items[1].Selected && !this.chk_timestamp.Items[0].Selected)
                policy.marcati = "0";
            else
                policy.marcati = string.Empty;

            policy.scadenzaMarca = this.ddl_timestamp_expiry.SelectedValue;

            policy.filtroDataCreazione = this.ddl_dataCreazione_E.SelectedValue;
            if (this.ddl_dataCreazione_E.SelectedValue.Equals("R"))
            {
                policy.dataCreazioneDa = this.lbl_dataCreazioneDa.Text;
                policy.dataCreazioneA = this.lbl_dataCreazioneA.Text;
            }
            else if (this.ddl_dataCreazione_E.SelectedValue.Equals("S"))
            {
                policy.dataCreazioneDa = this.lbl_dataCreazioneDa.Text;
            }
            else
            {
                policy.dataCreazioneDa = string.Empty;
                policy.dataCreazioneA = string.Empty;
            }
            if (this.ddl_dataCreazione_E.SelectedValue.Equals("P"))
                policy.numGiorniCreazione = this.txt_days_cr.Text;
            else
                policy.numGiorniCreazione = string.Empty;

            policy.filtrodataProtocollazione = this.ddl_dataProt_E.SelectedValue;
            if (this.ddl_dataProt_E.SelectedValue.Equals("R"))
            {
                policy.dataProtocollazioneDa = this.lbl_dataCreazioneDaP.Text;
                policy.dataProtocollazioneA = this.lbl_dataCreazioneAP.Text;
            }
            else if (this.ddl_dataProt_E.SelectedValue.Equals("S"))
            {
                policy.dataProtocollazioneDa = this.lbl_dataCreazioneDaP.Text;
            }
            else
            {
                policy.dataProtocollazioneDa = string.Empty;
                policy.dataProtocollazioneA = string.Empty;
            }
            if (this.ddl_dataProt_E.SelectedValue.Equals("P"))
                policy.numGiorniProtocollazione = this.txt_days_pr.Text;
            else
                policy.numGiorniProtocollazione = string.Empty;

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

            if (this.TemplateProf != null)
            {
                policy.template = this.TemplateProf;
            }

            return policy;
        }

        protected void ClearSession()
        {
            //if (Request.QueryString["s"] != null && Request.QueryString["s"].ToString().Equals("new"))
            //{
            //    this.SelectedFile = null;
            //}
            this.SelectedFile = null;
            this.Template = null;
            this.TemplateProf = null;
            this.TemplateMod = null;
            this.FascicoliSelezionati = null;
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

        /// <summary>
        /// Risultati restituiti selezionati
        /// </summary>
        public Dictionary<String, TemplateLite> Template
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["Template"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["Template"] as Dictionary<String, TemplateLite>;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["Template"] = value;
            }
        }

        protected Templates TemplateProf
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["TemplateProf"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["TemplateProf"] as Templates;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["TemplateProf"] = value;
            }
        }

        protected Templates TemplateMod
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["TemplateMod"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["TemplateMod"] as Templates;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["TemplateMod"] = value;
            }
        }

        /// <summary>
        /// Fascicoli selezionati
        /// </summary>
        public Fascicolo[] FascicoliSelezionati
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["FascicoliSelezionati"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["FascicoliSelezionati"] as Fascicolo[];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["FascicoliSelezionati"] = value;
            }
        }

        protected string UrlDocumentFormat
        {
            get
            {
                if (Request.QueryString["s"] != null && Request.QueryString["s"].ToString().Equals("new"))
                {
                    return "newDocumentFormat.aspx?s=new";
                }
                else
                {
                    return "newDocumentFormat.aspx?s=mod";
                }
            }

        }

        protected string UrlCampiProfilati
        {
            get
            {
                if (Request.QueryString["id"] != null && !string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    return "CampiProfilati.aspx?type=D&idPolicy=" + Request.QueryString["id"].ToString();
                }
                else
                {
                    return "CampiProfilati.aspx?type=D";
                }
            }

        }

        public Dictionary<String, String> SelectedFile
        {
            get
            {
                return HttpContext.Current.Session["SelectedFile"] as Dictionary<String, String>;
            }
            set
            {
                HttpContext.Current.Session["SelectedFile"] = value;
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