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
    public partial class newPolicyDocumento : System.Web.UI.Page
    {

        private DocsPaWR.DocsPaWebService _wsInstance = null;
        protected DocsPAWA.DocsPaWR.EtichettaInfo[] etichette;

        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);
            if (!IsPostBack)
            {
                GestioneGrafica();
                ClearSession();
                GetTypeDocument();
                GetRf();
                GetLettereProtocolli();
                PopulateTypeDocuments();
                GetAOO();
                GetTitolariUtilizzabili();
                this.TemplateProf = null;
                if (Request.QueryString["id"] != null && !string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    ViewDetailsPolicy();
                    this.titlePage.Text = "Dettaglio della Policy";
                }
            }
            UpdateFormatDocument();

            Corrispondente tempCorr = UserManager.getCorrispondenteSelezionatoRuoloSottoposto(this.Page);
            if (tempCorr != null)
            {
                txtCodRuolo.Text = tempCorr.codiceRubrica;
                txtDescRuolo.Text = tempCorr.descrizione;
                id_corr.Value = tempCorr.systemId;
                UserManager.removeCorrispondentiSelezionati(this.Page);
            }
        }

        protected void ViewDetailsPolicy()
        {
            string idPolicy = Request.QueryString["id"].ToString();
            this.Policy = _wsInstance.GetPolicyById(idPolicy);
            this.txt_nome.Text = this.Policy.nome;
            if (!string.IsNullOrEmpty(this.Policy.idTemplate) && !(this.Policy.idTemplate.Equals("-1")))
            {
                this.ddl_type_documents.SelectedValue = this.Policy.idTemplate;
                updateStateDiagram();
                this.btnCampiProfilati.Enabled = true;
                this.btnCampiProfilati.OnClientClick = String.Format("OpenCampiProfilati('" + this.Policy.idTemplate + "');");
                if (this.Policy.template != null)
                {
                    this.TemplateProf = this.Policy.template;
                }
            }
            if (!string.IsNullOrEmpty(this.Policy.idStatoDiagramma) && !(this.Policy.idStatoDiagramma.Equals("-1")))
            {
                this.ddl_state_document.SelectedValue = this.Policy.idStatoDiagramma;
            }
            if (!string.IsNullOrEmpty(this.Policy.idAOO) && !(this.Policy.idAOO.Equals("-1")))
            {
                this.ddl_aoo.SelectedValue = this.Policy.idAOO;
            }
            if (!string.IsNullOrEmpty(this.Policy.idRf) && !(this.Policy.idRf.Equals("-1")))
            {
                this.ddl_rf.SelectedValue = this.Policy.idRf;
            }
            if (!string.IsNullOrEmpty(this.Policy.classificazione) && !(this.Policy.classificazione.Equals("-1")))
            {
                Fascicolo tempF = FascicoliManager.getFascicoloByIdNoSecurity(this.Policy.classificazione);
                this.txtCodFascicolo.Text = tempF.codice;
                this.txtDescFascicolo.Text = tempF.descrizione;
                this.id_Fasc.Value = tempF.systemID;
                this.chk_tipo_class.Enabled = true;
                this.chk_includiSottoNodi.Enabled = true;
                if (!string.IsNullOrEmpty(this.Policy.tipoClassificazione))
                {
                    this.chk_tipo_class.SelectedValue = this.Policy.tipoClassificazione;
                }
                if (this.Policy.includiSottoNodi)
                {
                    this.chk_includiSottoNodi.Checked = true;
                }
                if (!string.IsNullOrEmpty(tempF.idTitolario))
                {
                    ListItem temp = new ListItem();
                    temp.Value = tempF.idTitolario;
                    if (this.ddl_titolari.Items.FindByValue(temp.Value) != null)
                    {
                        this.ddl_titolari.SelectedValue = tempF.idTitolario;
                    }
                }
            }
            this.chk_Arr.Selected = this.Policy.arrivo;
            this.chk_Grigio.Selected = this.Policy.grigio;
            this.chk_Int.Selected = this.Policy.interno;
            this.chk_Part.Selected = this.Policy.partenza;
            this.lbl_dataCreazioneDa.txt_Data.Text = this.Policy.dataCreazioneDa;

            this.chkDigitale.Checked = this.Policy.soloDigitali;
            this.chkFirmati.Checked = this.Policy.soloFirmati;

            if (!string.IsNullOrEmpty(this.Policy.tipoDataCreazione))
            {
                this.ddl_dataCreazione_E.SelectedValue = this.Policy.tipoDataCreazione;

                if (this.Policy.tipoDataCreazione.Equals("2") || this.Policy.tipoDataCreazione.Equals("3") || this.Policy.tipoDataCreazione.Equals("4") || this.Policy.tipoDataCreazione.Equals("5"))
                {
                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneAP.Visible = false;
                }
                else
                {
                    if (this.Policy.tipoDataCreazione.Equals("0"))
                    {
                        this.lblDa.Visible = true;
                        this.lbl_dataCreazioneDa.Visible = true;
                        this.lbl_dataCreazioneDa.txt_Data.Text = this.Policy.dataCreazioneDa;
                    }
                    if (this.Policy.tipoDataCreazione.Equals("1"))
                    {
                        this.lblDa.Visible = true;
                        this.lbl_dataCreazioneDa.Visible = true;
                        this.lbl_dataCreazioneDa.txt_Data.Text = this.Policy.dataCreazioneDa;
                        if (!string.IsNullOrEmpty(this.Policy.dataCreazioneA))
                        {
                            this.lblA.Visible = true;
                            this.lbl_dataCreazioneA.Visible = true;
                            this.lbl_dataCreazioneA.txt_Data.Text = this.Policy.dataCreazioneA;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(this.Policy.tipoDataProtocollazione))
            {
                this.ddl_dataProt_E.SelectedValue = this.Policy.tipoDataProtocollazione;

                if (this.Policy.tipoDataProtocollazione.Equals("2") || this.Policy.tipoDataProtocollazione.Equals("3") || this.Policy.tipoDataProtocollazione.Equals("4") || this.Policy.tipoDataProtocollazione.Equals("5"))
                {
                    this.lblDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Visible = false;
                    this.lblAP.Visible = false;
                    this.lbl_dataCreazioneAP.Visible = false;
                }
                else
                {
                    if (this.Policy.tipoDataProtocollazione.Equals("0"))
                    {
                        this.lblDaP.Visible = true;
                        this.lbl_dataCreazioneDaP.Visible = true;
                        this.lbl_dataCreazioneDaP.txt_Data.Text = this.Policy.dataProtocollazioneDa;
                    }
                    if (this.Policy.tipoDataProtocollazione.Equals("1"))
                    {
                        this.lblDaP.Visible = true;
                        this.lbl_dataCreazioneDaP.Visible = true;
                        this.lbl_dataCreazioneDaP.txt_Data.Text = this.Policy.dataProtocollazioneDa;
                        if (!string.IsNullOrEmpty(this.Policy.dataProtocollazioneA))
                        {
                            this.lblAP.Visible = true;
                            this.lbl_dataCreazioneAP.Visible = true;
                            this.lbl_dataCreazioneAP.txt_Data.Text = this.Policy.dataProtocollazioneA;
                        }
                    }
                }
            }

            if (this.Policy.FormatiDocumento != null && this.Policy.FormatiDocumento.Length > 0)
            {
                this.SelectedFile = new Dictionary<string, string>();
                string formatDocumet = string.Empty;
                for (int i = 0; i < this.Policy.FormatiDocumento.Length; i++)
                {
                    formatDocumet = formatDocumet + this.Policy.FormatiDocumento.ElementAt(i).FileExtension;
                    this.SelectedFile.Add(this.Policy.FormatiDocumento.ElementAt(i).SystemId.ToString(), this.Policy.FormatiDocumento.ElementAt(i).FileExtension);
                    if (i < this.Policy.FormatiDocumento.Length - 1)
                    {
                        formatDocumet = formatDocumet + " | ";
                    }
                }
                this.lbl_documents_format.Text = formatDocumet;
            }

            if (!string.IsNullOrEmpty(this.Policy.idUoCreatore) && !(this.Policy.idUoCreatore.Equals("-1")))
            {
                Corrispondente corrTemp = UserManager.getCorrispondenteBySystemIDDisabled(this.Page, this.Policy.idUoCreatore);
                this.txtCodRuolo.Text = corrTemp.codiceRubrica;
                this.txtDescRuolo.Text = corrTemp.descrizione;
                this.id_corr.Value = corrTemp.systemId;
            }

            this.chk_sottoposti.Checked = this.Policy.uoSottoposte;
        }

        public void Page_Prerender(object sender, EventArgs e)
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
                this.chk_includiSottoNodi.Enabled = true;
            }
        }

        protected void GestioneGrafica()
        {
            this.btnApriRubrica.Attributes.Add("onmouseover", "this.src='../../images/proto/rubrica_hover.gif'");
            this.btnApriRubrica.Attributes.Add("onmouseout", "this.src='../../images/proto/rubrica.gif'");
            this.btnApriRubrica.OnClientClick = String.Format("_ApriRubricaRicercaRuoliSottoposti();");
            this.btn_img_doc.Attributes.Add("onmouseover", "this.src='../Images/icon_admin_up.gif'");
            this.btn_img_doc.Attributes.Add("onmouseout", "this.src='../Images/icon_admin.gif'");
            this.btn_img_doc.OnClientClick = String.Format("OpenDocumentFormat();");
            this.lblA.Visible = false;
            this.lbl_dataCreazioneA.Visible = false;
            this.lblAP.Visible = false;
            this.lbl_dataCreazioneAP.Visible = false;
            DocsPaWR.Utente ut = new DocsPAWA.DocsPaWR.Utente();
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            string idAmministrazione = IdAmministrazione.ToString();

            ut.codiceAmm = codiceAmministrazione;
            ut.idAmministrazione = idAmministrazione;
            ut.tipoIE = "I";
            Session.Add("userData", ut);

            DocsPaWR.Ruolo rl = new DocsPAWA.DocsPaWR.Ruolo();
            rl.codiceAmm = codiceAmministrazione;
            rl.idAmministrazione = idAmministrazione;
            rl.tipoIE = "I";

            rl.systemId = idAmministrazione;
            rl.uo = new DocsPAWA.DocsPaWR.UnitaOrganizzativa();
            rl.uo.codiceRubrica = codiceAmministrazione;

            Session.Add("userRuolo", rl);

            DocsPaWR.Registro reg = new DocsPAWA.DocsPaWR.Registro();
            reg.codAmministrazione = codiceAmministrazione;
            reg.idAmministrazione = idAmministrazione;
            Session.Add("userRegistro", reg);
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

        protected string UrlChooseProject
        {
            get
            {
                return "ChooseProject.aspx";
            }

        }

        protected string UrlCampiProfilati
        {
            get
            {
                if (this.Policy != null && !string.IsNullOrEmpty(this.Policy.system_id))
                {
                    return "CampiProfilati.aspx?type=D&idPolicy=" + this.Policy.system_id;
                }
                else
                {
                    return "CampiProfilati.aspx?type=D";
                }
            }

        }

        protected void ClearSession()
        {
            if (Request.QueryString["s"] != null && Request.QueryString["s"].ToString().Equals("new"))
            {
                this.SelectedFile = null;
            }
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

        protected void GetTypeDocument()
        {
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
            {
                DocsPaWR.TemplateLite[] templateList;
                this.pnl_profilazione.Visible = true;
                templateList = this.WsInstance.GetTypeDocumentsWithDiagramByIdAmm(IdAmministrazione, "D");
                if (templateList != null && templateList.Length > 0)
                {
                    ddl_type_documents.Items.Clear();
                    ddl_type_documents.Items.Add("");
                    this.Template = new Dictionary<string, TemplateLite>();
                    for (int i = 0; i < templateList.Length; i++)
                    {
                        ddl_type_documents.Items.Add(templateList[i].name);
                        ddl_type_documents.Items[i + 1].Value = templateList[i].system_id;
                        this.Template[templateList[i].system_id] = templateList[i];
                    }
                }
            }
            else
            {
                this.pnl_profilazione.Visible = false;
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
                ddl_aoo.Items.Clear();
                ddl_aoo.Items.Add("");
                for (int i = 0; i < reg.Length; i++)
                {
                    string descrizione = "[" + reg[i].codice + "]" + " - " + reg[i].descrizione;
                    ddl_aoo.Items.Add(descrizione);
                    ddl_aoo.Items[i + 1].Value = reg[i].systemId;
                }
            }
        }

        protected void ChangeTypeDocument(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddl_type_documents.SelectedValue))
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
                    }
                }
                else
                {
                    ddl_state_document.Items.Clear();
                    ddl_state_document.Enabled = false;
                }
                this.btnCampiProfilati.OnClientClick = String.Format("OpenCampiProfilati('" + ddl_type_documents.SelectedValue + "');");
                this.btnCampiProfilati.Enabled = true;
                this.TemplateProf = null;
            }
            else
            {
                ddl_state_document.Items.Clear();
                ddl_state_document.Enabled = false;
                this.btnCampiProfilati.Enabled = false;
            }
            this.upStateTypeDocument.Update();
        }

        protected void PopulateTypeDocuments()
        {

        }
        /// <summary>
        /// Evento generato al cambio del testo nella casella del codice fascicolo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                this.chk_includiSottoNodi.Enabled = false;
            }
            this.upClassificationPanel.Update();
        }

        protected void SearchProject()
        {

            // Lista dei fascicoli restituiti dalla ricerca
            Fascicolo[] projectList;

            projectList = FascicoliManager.GetFascicoloDaCodiceNoSecurity(this.txtCodFascicolo.Text, this.IdAmministrazione.ToString(), this.ddl_titolari.SelectedValue, true);

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
                    this.chk_includiSottoNodi.Enabled = true;
                }
                else
                {
                    this.FascicoliSelezionati = new Fascicolo[projectList.Length];
                    this.FascicoliSelezionati = projectList;

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_project", "OpenSceltaFascicoli();", true);
                    txtCodFascicolo.Text = string.Empty;
                    txtDescFascicolo.Text = string.Empty;
                    this.id_Fasc.Value = string.Empty;
                    this.chk_tipo_class.Enabled = false;
                    this.rbl_1.Selected = false;
                    this.rbl_2.Selected = false;
                    this.rbl_3.Selected = false;
                    this.chk_includiSottoNodi.Enabled = false;
                }
            }

        }

        private void GetLettereProtocolli()
        {

            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPAWA.DocsPaWR.InfoUtente user = new InfoUtente();
            this.etichette = wws.getEtichetteDocumenti(user, IdAmministrazione.ToString());
            this.chk_Arr.Text = etichette[0].Etichetta; //Valore A
            this.chk_Part.Text = etichette[1].Etichetta; //Valore P
            this.chk_Int.Text = etichette[2].Etichetta;
            this.chk_Grigio.Text = etichette[3].Etichetta;

            if (!wws.IsInternalProtocolEnabled(IdAmministrazione.ToString()))
                this.chkList.Items.Remove(this.chkList.Items[2]);

        }

        protected void BtnSaveDocument_Click(object sender, EventArgs e)
        {
            bool insert = CheckValues();
            if (insert)
            {
                Policy policy = PopulatePolicy();
                if (Request.QueryString["s"] != null && Request.QueryString["s"].ToString().Equals("new"))
                {
                    //Inserimento di una nuova policy
                    bool result = WsInstance.InserisciPolicyConservazione(policy);

                    // svuoto la cache per ricaricare la lista aggiornata
                    try
                    {
                        bool svuotaCache = WsInstance.SvuotaCachePolicy(this.IdAmministrazione.ToString(), "D");
                    }
                    catch (Exception ex)
                    {

                    }

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save('Y');", true);
                }
                else
                {
                    //Inserimento di una nuova policy
                    policy.system_id = Request.QueryString["id"].ToString();
                    bool result = WsInstance.ModifyPolicyConservazione(policy);

                    // svuoto la cache per ricaricare la lista aggiornata nel dettaglio
                    try
                    {
                        bool svuotaCache = WsInstance.SvuotaCachePolicy(this.IdAmministrazione.ToString(), "D");
                    }
                    catch (Exception ex)
                    {

                    }

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save('Y');", true);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(this.txt_nome.Text))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_project", "alert('Inserire un nome per la Policy');", true);
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.txtCodFascicolo.Text) && !string.IsNullOrEmpty(this.txtDescFascicolo.Text) && string.IsNullOrEmpty(this.chk_tipo_class.SelectedValue))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_project", "alert('Seleziona un tipo di ricerca per la classificazione');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_project", "alert('Selezionare almeno un criterio per la creazione della Policy');", true);
                    }
                }
            }
        }

        protected Policy PopulatePolicy()
        {
            Policy result = new Policy();
            result.arrivo = this.chk_Arr.Selected;
            result.automatico = false;
            result.classificazione = this.id_Fasc.Value;
            result.tipoClassificazione = this.chk_tipo_class.SelectedValue;
            result.idRf = this.ddl_rf.SelectedValue;
            result.idAOO = this.ddl_aoo.SelectedValue;
            result.idAmministrazione = IdAmministrazione.ToString();
            if (this.ddl_state_document.Enabled)
            {
                result.idStatoDiagramma = this.ddl_state_document.SelectedValue;
            }
            result.idTemplate = this.ddl_type_documents.SelectedValue;
            result.interno = this.chk_Int.Selected;
            result.nome = this.txt_nome.Text;
            result.partenza = this.chk_Part.Selected;
            result.grigio = this.chk_Grigio.Selected;
            result.tipo = "D";
            if (this.SelectedFile != null && this.SelectedFile.Count > 0)
            {
                result.FormatiDocumento = new SupportedFileType[this.SelectedFile.Count];
                for (int i = 0; i < this.SelectedFile.Count; i++)
                {
                    SupportedFileType tempSupp = new SupportedFileType();
                    tempSupp.FileExtension = this.SelectedFile.ElementAt(i).Value;
                    tempSupp.SystemId = Convert.ToInt32(this.SelectedFile.ElementAt(i).Key);
                    result.FormatiDocumento[i] = tempSupp;
                }
            }

            result.tipoDataCreazione = this.ddl_dataCreazione_E.SelectedValue;
            result.dataCreazioneDa = this.lbl_dataCreazioneDa.Text;
            if (this.lbl_dataCreazioneA != null && !string.IsNullOrEmpty(this.lbl_dataCreazioneA.Text))
            {
                result.dataCreazioneA = this.lbl_dataCreazioneA.Text;
            }

            result.tipoDataProtocollazione = this.ddl_dataProt_E.SelectedValue;
            result.dataProtocollazioneDa = this.lbl_dataCreazioneDaP.Text;
            if (this.lbl_dataCreazioneAP != null && !string.IsNullOrEmpty(this.lbl_dataCreazioneAP.Text))
            {
                result.dataProtocollazioneA = this.lbl_dataCreazioneAP.Text;
            }

            result.idUoCreatore = this.id_corr.Value;
            result.uoSottoposte = this.chk_sottoposti.Checked;
            result.includiSottoNodi = this.chk_includiSottoNodi.Checked;
            result.soloDigitali = this.chkDigitale.Checked;
            result.soloFirmati = this.chkFirmati.Checked;

            if (this.TemplateProf != null)
            {
                result.template = this.TemplateProf;
            }

            return result;
        }

        protected bool CheckValues()
        {
            bool result = false;
            if ((!string.IsNullOrEmpty(this.txt_nome.Text)) && ((!string.IsNullOrEmpty(this.txtCodFascicolo.Text) || !string.IsNullOrEmpty(this.ddl_type_documents.SelectedValue)) || !string.IsNullOrEmpty(ddl_rf.SelectedValue) || this.chk_Arr.Selected || this.chk_Grigio.Selected || this.chk_Part.Selected || this.chk_Int.Selected || this.ddl_dataCreazione_E.SelectedValue.Equals("2") || this.ddl_dataCreazione_E.SelectedValue.Equals("3") || this.ddl_dataCreazione_E.SelectedValue.Equals("4") || this.ddl_dataCreazione_E.SelectedValue.Equals("5") || this.ddl_dataProt_E.SelectedValue.Equals("2") || this.ddl_dataProt_E.SelectedValue.Equals("3") || this.ddl_dataProt_E.SelectedValue.Equals("4") || this.ddl_dataProt_E.SelectedValue.Equals("5") || (this.ddl_dataCreazione_E.SelectedValue.Equals("0") && !string.IsNullOrEmpty(this.lbl_dataCreazioneDa.txt_Data.Text)) || (this.ddl_dataProt_E.SelectedValue.Equals("0") && !string.IsNullOrEmpty(this.lbl_dataCreazioneDaP.txt_Data.Text)) || (this.ddl_dataCreazione_E.SelectedValue.Equals("1") && !string.IsNullOrEmpty(this.lbl_dataCreazioneDa.txt_Data.Text)) || (this.ddl_dataProt_E.SelectedValue.Equals("1") && !string.IsNullOrEmpty(this.lbl_dataCreazioneDaP.txt_Data.Text)) || !string.IsNullOrEmpty(this.id_corr.Value) || (this.SelectedFile != null && this.SelectedFile.Count > 0)))
            {
                result = true;
            }
            if (!string.IsNullOrEmpty(this.txtCodFascicolo.Text) && !string.IsNullOrEmpty(this.txtDescFascicolo.Text) && string.IsNullOrEmpty(this.chk_tipo_class.SelectedValue))
            {
                result = false;
            }

            return result;
        }

        protected void ddl_dataCreazione_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataCreazione_E.SelectedIndex)
            {
                case 0:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = true;
                    this.lbl_dataCreazioneDa.Visible = true;
                    break;

                case 1:
                    this.lblA.Visible = true;
                    this.lbl_dataCreazioneA.Visible = true;

                    this.lblDa.Visible = true;
                    this.lbl_dataCreazioneDa.Visible = true;
                    break;

                case 2:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;
                    break;

                case 3:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;
                    break;

                case 4:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;
                    break;

                case 5:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;
                    break;
            }

            this.upCreationDate.Update();
        }

        protected void ddl_dataProt_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataProt_E.SelectedIndex)
            {
                case 0:
                    this.lblAP.Visible = false;
                    this.lbl_dataCreazioneAP.Visible = false;
                    this.lbl_dataCreazioneAP.txt_Data.Text = string.Empty;

                    this.lblDaP.Visible = true;
                    this.lbl_dataCreazioneDaP.Visible = true;
                    break;

                case 1:
                    this.lblAP.Visible = true;
                    this.lbl_dataCreazioneAP.Visible = true;


                    this.lblDaP.Visible = true;
                    this.lbl_dataCreazioneDaP.Visible = true;
                    break;

                case 2:
                    this.lblAP.Visible = false;
                    this.lbl_dataCreazioneAP.Visible = false;
                    this.lbl_dataCreazioneAP.Text = string.Empty;

                    this.lblDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Text = string.Empty;
                    break;

                case 3:
                    this.lblAP.Visible = false;
                    this.lbl_dataCreazioneAP.Visible = false;
                    this.lbl_dataCreazioneAP.Text = string.Empty;

                    this.lblDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Text = string.Empty;
                    break;

                case 4:
                    this.lblAP.Visible = false;
                    this.lbl_dataCreazioneAP.Visible = false;
                    this.lbl_dataCreazioneAP.Text = string.Empty;

                    this.lblDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Text = string.Empty;
                    break;

                case 5:
                    this.lblAP.Visible = false;
                    this.lbl_dataCreazioneAP.Visible = false;
                    this.lbl_dataCreazioneAP.Text = string.Empty;

                    this.lblDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Text = string.Empty;
                    break;
            }

            this.upProtDate.Update();
        }

        protected void updateStateDiagram()
        {
            if (!string.IsNullOrEmpty(ddl_type_documents.SelectedValue))
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
                    }
                }
                else
                {
                    ddl_state_document.Items.Clear();
                    ddl_state_document.Enabled = false;
                }

            }
            else
            {
                ddl_state_document.Items.Clear();
                ddl_state_document.Enabled = false;
            }
        }

        /// <summary>
        /// Evento generato al cambio del testo nella casella del codice rubrica
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtCodRuolo_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCodRuolo.Text))
            {
                setDescCorr(txtCodRuolo.Text);
            }
            else
            {
                txtCodRuolo.Text = string.Empty;
                txtDescRuolo.Text = string.Empty;
                id_corr.Value = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Inserire un codice da cercare in rubrica');", true);
            }
            this.upAddress.Update();
        }

        protected void setDescCorr(string codRubrica)
        {
            DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this.Page, codRubrica, AddressbookTipoUtente.INTERNO);
            if (corr == null)
            {
                txtCodRuolo.Text = string.Empty;
                txtDescRuolo.Text = string.Empty;
                id_corr.Value = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_found", "alert('Corrispondente non trovato');", true);
            }
            else
            {
                if (corr != null && !corr.tipoCorrispondente.Equals("U"))
                {
                    txtCodRuolo.Text = string.Empty;
                    txtDescRuolo.Text = string.Empty;
                    id_corr.Value = string.Empty;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_found", "alert('Inserire soltanto UO');", true);
                }
                else
                {
                    txtCodRuolo.Text = corr.codiceRubrica;
                    txtDescRuolo.Text = corr.descrizione;
                    id_corr.Value = corr.systemId;
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

        protected void ViewCampiProlilati(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// TemporarySelectedFile
        /// </summary>
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

        /// <summary>
        /// Policy selezionata
        /// </summary>
        protected Policy Policy
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["policy"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["policy"] as Policy;
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
                CallContextStack.CurrentContext.ContextState["policy"] = value;
            }
        }


        /// <summary>
        /// Template selezionato
        /// </summary>
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
    }
}