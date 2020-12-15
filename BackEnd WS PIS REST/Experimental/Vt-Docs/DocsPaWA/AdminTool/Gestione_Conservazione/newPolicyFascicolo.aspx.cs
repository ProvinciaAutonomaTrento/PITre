using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SiteNavigation;
using System.Collections;

namespace DocsPAWA.AdminTool.Gestione_Conservazione
{
    public partial class newPolicyFascicolo : System.Web.UI.Page
    {
        private DocsPaWR.DocsPaWebService _wsInstance = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);
            if (!IsPostBack)
            {

                GetTypeProject();
                GetAOO();
                this.TemplateProf = null;
                GetRf();
                GetTitolariUtilizzabili();
                this.btnApriRubrica.Attributes.Add("onmouseover", "this.src='../../images/proto/rubrica_hover.gif'");
                this.btnApriRubrica.Attributes.Add("onmouseout", "this.src='../../images/proto/rubrica.gif'");
                this.btnApriRubrica.OnClientClick = String.Format("_ApriRubricaRicercaRuoliSottoposti();");
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

                if (Request.QueryString["id"] != null && !string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    ViewDetailsPolicy();
                    this.titlePage.Text = "Dettaglio della Policy";
                }

                

            }

            Corrispondente tempCorr = UserManager.getCorrispondenteSelezionatoRuoloSottoposto(this.Page);
            if (tempCorr != null)
            {
                txtCodRuolo.Text = tempCorr.codiceRubrica;
                txtDescRuolo.Text = tempCorr.descrizione;
                id_corr.Value = tempCorr.systemId;
                UserManager.removeCorrispondentiSelezionati(this.Page);
            }
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
                this.chk_includiSottoNodi.Enabled = true;
               
            }
        }

        protected void GetTypeProject()
        {
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
            {
                DocsPaWR.TemplateLite[] templateList;
                this.pnl_profilazione.Visible = true;
                templateList = this.WsInstance.GetTypeDocumentsWithDiagramByIdAmm(IdAmministrazione, "F");
                if (templateList != null && templateList.Length > 0)
                {
                    ddl_type_documents.Items.Clear();
                    ddl_type_documents.Items.Add("");
                    this.Template = new Dictionary<string, TemplateLite>();
                    for (int i = 0; i < templateList.Length; i++)
                    {
                        ddl_type_documents.Items.Add(templateList[i].name);
                        ddl_type_documents.Items[i + 1].Value = templateList[i].system_id;
                        if (!this.Template.ContainsKey(templateList[i].system_id))
                        {
                            this.Template.Add(templateList[i].system_id, templateList[i]);
                        }
                    }
                }
            }
            else
            {
                this.pnl_profilazione.Visible = false;
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
                this.chk_includiSottoNodi.Enabled = true;
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

            if (!string.IsNullOrEmpty(this.Policy.idUoCreatore) && !(this.Policy.idUoCreatore.Equals("-1")))
            {
                Corrispondente corrTemp = UserManager.getCorrispondenteBySystemIDDisabled(this.Page, this.Policy.idUoCreatore);
                this.txtCodRuolo.Text = corrTemp.codiceRubrica;
                this.txtDescRuolo.Text = corrTemp.descrizione;
                this.id_corr.Value = corrTemp.systemId;
            }

            this.chkDigitale.Checked = this.Policy.soloDigitali;
            this.chkFirmati.Checked = this.Policy.soloFirmati;

            this.chk_sottoposti.Checked = this.Policy.uoSottoposte;

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

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save('Y');", true);
                }
                else
                {
                    //Inserimento di una nuova policy
                    policy.system_id = Request.QueryString["id"].ToString();
                    bool result = WsInstance.ModifyPolicyConservazione(policy);

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
                    if (string.IsNullOrEmpty(this.txtCodFascicolo.Text) || string.IsNullOrEmpty(txtDescFascicolo.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_project", "alert('Inserire un codice di classifica');", true);
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
            result.automatico = false;
            result.classificazione = this.id_Fasc.Value;
            result.idAmministrazione = IdAmministrazione.ToString();
            result.idRf = this.ddl_rf.SelectedValue;
            result.idAOO = this.ddl_aoo.SelectedValue;
            if (this.ddl_state_document.Enabled)
            {
                result.idStatoDiagramma = this.ddl_state_document.SelectedValue;
            }
            result.idTemplate = this.ddl_type_documents.SelectedValue;
            result.nome = this.txt_nome.Text;
            result.tipo = "F";

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
            if ((!string.IsNullOrEmpty(this.txt_nome.Text)) && ((!string.IsNullOrEmpty(this.txtCodFascicolo.Text) || !string.IsNullOrEmpty(this.id_corr.Value) || !string.IsNullOrEmpty(this.ddl_type_documents.SelectedValue))))
            {
                result = true;
            }
            if (string.IsNullOrEmpty(this.txtCodFascicolo.Text) || string.IsNullOrEmpty(txtDescFascicolo.Text))
            {
                result = false;
            }

            return result;
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
                    this.chk_includiSottoNodi.Enabled = false;
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


        protected DocsPaWR.DocsPaWebService WsInstance
        {
            get
            {
                if (this._wsInstance == null)
                {
                    this._wsInstance = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    this._wsInstance.Timeout = System.Threading.Timeout.Infinite;
                }
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

        protected string UrlChooseProject
        {
            get
            {
                return "ChooseProject.aspx";
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

        protected string UrlCampiProfilati
        {
            get
            {
                if (this.Policy != null && !string.IsNullOrEmpty(this.Policy.system_id))
                {
                    return "CampiProfilati.aspx?type=F&idPolicy=" + this.Policy.system_id;
                }
                else
                {
                    return "CampiProfilati.aspx?type=F";
                }
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