using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.AdminTool.Manager;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SiteNavigation;
using DocsPAWA.utils;

namespace DocsPAWA.popup
{
    public partial class FindAndReplaceModelliTrasmissione : CssPage
    {
        #region Varibili di CallContext

        /// <summary>
        /// Ruolo da sostituire
        /// </summary>
        private ElementoRubrica RoleToReplace
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["RoleToReplace"] as ElementoRubrica;
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["RoleToReplace"] = value;
            }
        }

        /// <summary>
        /// Ruolo con cui sostituire
        /// </summary>
        private ElementoRubrica NewRole
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["NewRole"] as ElementoRubrica;
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["NewRole"] = value;
            }
        }

        private ModelloTrasmissioneSearchResult[] ModelloTrasmissioneArray 
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["ModelloTrasmissioneArray"] as ModelloTrasmissioneSearchResult[];
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["ModelloTrasmissioneArray"] = value;
            }
        }


        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            // Inizializzazione drop down dei registri
            if (!IsPostBack)
                this.Initialize();

            if (Session["CodeTextBox"] != null)
                this.AnalyzeRoleResult();

        }

        /// <summary>
        /// Metodo per l'inizializzazione della finestra
        /// </summary>
        private void Initialize()
        {
            // Registrazione eventi onchange javascript per le text box dei codici
            this.txtFindCodice.Attributes["onchange"] = String.Format("javascript:clearCorrData(this, '{0}');", this.txtFindDescrizione.ClientID);
            this.txtReplaceCodice.Attributes["onchange"] = String.Format("javascript:clearCorrData(this, '{0}');", this.txtReplaceDescrizione.ClientID);

            // Se ci si trova in amministrazione, vengono caricati tutti i registri dell'amministrazione
            if (Session["AMMDATASET"] != null)
            {
                string[] amm = ((string)Session["AMMDATASET"]).Split('@');
                string codAmm = amm[0];

                OrgRegistro[] registri = UserManager.getRegistriByCodAmm(codAmm, "0");

                foreach (var reg in registri)
                    this.ddlRegistry.Items.Add(new ListItem(reg.Descrizione, reg.IDRegistro));
                
            }
            else
            {
                // Vengono caricati i soli registri ed RF per il ruolo dell'utente
                Registro[] registri = UserManager.GetRegistriByRuolo(this, UserManager.getRuolo().systemId);

                foreach (var reg in registri)
                    this.ddlRegistry.Items.Add(new ListItem(reg.descrizione, reg.systemId));
                
            }

            this.ddlRegistry.SelectedIndex = 0;

        }

        #region Gestione pulsanti soluzione codice e ricerca da rubrica per la sezione dei filtri di ricerca

        protected void imgSrcFind_Click(object sender, ImageClickEventArgs e)
        {
            // Ricerca dei corrispondenti con il codice inserito. Se ne vengono restituiti più di uno
            // viene preso il primo
            ElementoRubrica[] elems = this.ResolveCode(this.txtFindCodice.Text, true, RubricaCallType.CALLTYPE_FIND_ROLE);
            if (elems != null && elems.Length > 0)
            {
                this.txtFindCodice.Text = elems[0].codice;
                this.txtFindDescrizione.Text = elems[0].descrizione;

                this.RoleToReplace = elems[0];
            }
            else
            {
                this.MessageBox.ShowMessage("Corrispondente non trovato.");
                this.txtFindCodice.Text = String.Empty;
            }
        }

        protected void imgRubricaFind_Click(object sender, ImageClickEventArgs e)
        {
            // Registrazione script per l'apertura della rubrica e salvataggio in sessione di un valore che
            // consenta di capire che alla chiusura della finestra della rubrica deve essere compilato il
            // codice e la descrizione per il ruolo da sostituire
            Session["CodeTextBox"] = this.txtFindCodice.ID;

            Registro reg = (DocsPAWA.DocsPaWR.Registro)Session["userRegistro"];
            if (reg == null)
                reg = new Registro();

            reg.systemId = this.ddlRegistry.SelectedValue.ToString();
            reg.codRegistro = this.ddlRegistry.SelectedItem.Text.ToString();
            Session.Add("userRegistro", reg);

            ScriptManager.RegisterStartupScript(this, this.GetType(), "openAB", "ApriRubricaFind();", true);
            
        }

        protected void imgSrcReplace_Click(object sender, ImageClickEventArgs e)
        {
            // Ricerca dei corrispondenti con il codice inserito. Se ne vengono restituiti più di uno
            // viene preso il primo
            ElementoRubrica[] elems = this.ResolveCode(this.txtReplaceCodice.Text, false, RubricaCallType.CALLTYPE_REPLACE_ROLE);
            if (elems != null && elems.Length > 0)
            {
                this.txtReplaceCodice.Text = elems[0].codice;
                this.txtReplaceDescrizione.Text = elems[0].descrizione;
                this.NewRole = elems[0];
            }
            else
            {
                this.MessageBox.ShowMessage("Corrispondente non trovato.");
                this.txtReplaceCodice.Text = String.Empty;
            }
        }

        protected void imgRubricaReplace_Click(object sender, ImageClickEventArgs e)
        {
            Registro reg = (DocsPAWA.DocsPaWR.Registro)Session["userRegistro"];
            if (reg == null)
                reg = new Registro();

            reg.systemId = this.ddlRegistry.SelectedValue.ToString();
            reg.codRegistro = this.ddlRegistry.SelectedItem.Text.ToString();
            Session.Add("userRegistro", reg);

            // Registrazione script per l'apertura della rubrica e salvataggio in sessione di un valore che
            // consenta di capire che alla chiusura della finestra della rubrica deve essere compilato il
            // codice e la descrizione per il ruolo con cui sostiuire
            ScriptManager.RegisterStartupScript(this, this.GetType(), "abForRole", "ApriRubricaReplace();", true);
            Session["CodeTextBox"] = this.txtReplaceCodice.ID;
        }

        /// <summary>
        /// Funzione per la risoluzione di un codice corrispondente
        /// </summary>
        /// <param name="code">Codice da risolvere</param>
        /// <param name="searchOnlyRoles">True se bisogna ricercare solo nei ruoli</param>
        /// <param name="callType">Call type da utilizzare</param>
        /// <returns>Array di oggetti con le informazioni sul corrispondenti trovati</returns>
        private ElementoRubrica[] ResolveCode(String code, bool searchOnlyRoles, RubricaCallType callType)
        {
            ElementoRubrica[] corrSearch = null;
            if (!String.IsNullOrEmpty(code))
            {
                ParametriRicercaRubrica qco = new ParametriRicercaRubrica();
                UserManager.setQueryRubricaCaller(ref qco);
                qco.codice = code.Trim();
                qco.tipoIE = DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO;
                qco.calltype = callType;
                qco.caller.IdRegistro = this.ddlRegistry.SelectedValue;

                //cerco su tutti i tipi utente:
                if (ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] != null &&
                    ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] == "1")
                    qco.doListe = true;
                qco.doRuoli = true;
                if (searchOnlyRoles)
                {
                    qco.doUtenti = false;
                    qco.doUo = false;
                }
                else
                {
                    qco.doUtenti = true;
                    qco.doUo = true;
                }

                qco.queryCodiceEsatta = true;

                corrSearch = UserManager.getElementiRubrica(this.Page, qco);

                
                // Se il calltype è relativo a ruolo da utilizzare per la sostituzione,
                // vengono esclusi i corrispondenti inibiti
                if (callType == RubricaCallType.CALLTYPE_REPLACE_ROLE && corrSearch != null && corrSearch.Length == 1 && corrSearch[0].disabledTrasm)
                    corrSearch = new ElementoRubrica[0];

            }

            return corrSearch;

        }

        /// <summary>
        /// Funzione per l'interpretazione dei risultati di ricerca ruolo da rubrica
        /// </summary>
        private void AnalyzeRoleResult()
        {
            // Prelvamento dei risultati della ricerca
            ElementoRubrica[] elems = Session["selMittDaRubrica"] as ElementoRubrica[];

            switch (Session["CodeTextBox"].ToString())
            {

                case "txtFindCodice":     // Variabile di sessione impostata: selMittDaRubrica
                    // Se l'elemento in sessione è un array di ElementoRubrica
                    if (Session["selMittDaRubrica"] != null && Session["selMittDaRubrica"] is ElementoRubrica[])
                    {
                        // Compilazione dei campi codice e descrizione del corrispondente da cercare (se ne è stato selezionato
                        // più di uno, viene preso il primo)
                        this.txtFindCodice.Text = elems[0].codice;
                        this.txtFindDescrizione.Text = elems[0].descrizione;
                        this.RoleToReplace = elems[0];

                    }
                    break;
                case "txtReplaceCodice":    // Variabile di sessione impostata: selMittDaRubrica
                    // Se l'elemento in sessione è un array di ElementoRubrica
                    if (Session["selMittDaRubrica"] != null && Session["selMittDaRubrica"] is ElementoRubrica[])
                    {
                        // Compilazione dei campi codice e descrizione del corrispondente con cui sostituire (se ne è stato selezionato
                        // più di uno, viene preso il primo)
                        this.txtReplaceCodice.Text = elems[0].codice;
                        this.txtReplaceDescrizione.Text = elems[0].descrizione;
                        this.NewRole = elems[0];

                    }

                    break;
            }

            // Pulizia sessione
            Session.Remove("CodeTextBox");
            Session.Remove("selMittDaRubrica");

        }

        #endregion

        protected void wzWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            // Se lo step è il primo, ovvero find, viene avviata la ricerca dei modelli di trasmissione che
            // soddifano i criteri richiesti
            if (this.CheckInput())
                this.ExecuteAction(e.CurrentStepIndex);
            else
            {
                e.Cancel = true;
                this.MessageBox.ShowMessage("Prima di continuare è necessario specificare un ruolo da sostituire ed un ruolo da utilizzare per la sostituzione.");
            }

            // Se non ci sono modelli, viene impedito il passaggio all'azione successiva
            if(this.ModelloTrasmissioneArray != null && (this.ModelloTrasmissioneArray == null || this.ModelloTrasmissioneArray.Length == 0))
            {
                this.ltlNoModels.Visible = true;
                e.Cancel = true;
            }

        }

        protected void wzWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            FindAndReplaceResponse response = null;

            if (this.CheckInput())
                // Esecuzione dell'azione
                response = ModelliTrasmManager.FindAndReplaceRolesInModelliTrasmissione(
                    this.RoleToReplace, 
                    this.NewRole, 
                    FindAndReplaceEnum.Replace, 
                    ModelliTrasmManager.SearchFilters, 
                    this.GetUser(), 
                    Session["AMMDATASET"] != null,
                    this.ModelloTrasmissioneArray, this.chkCopyNotes.Checked);

            this.ModelloTrasmissioneArray = response.Models;

            if (response.Models != null && response.Models.Length == 0)
                this.dgResult.DataSource = null;
            else
                this.dgResult.DataSource = response.Models;
            this.dgResult.DataBind();
        }

        protected void wzWizard_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
        {
            this.RoleToReplace = null;
            this.NewRole = null;
            this.txtFindCodice.Text = String.Empty;
            this.txtFindDescrizione.Text = String.Empty;
            this.txtReplaceCodice.Text = String.Empty;
            this.txtReplaceDescrizione.Text = String.Empty;
            this.dgResult.DataSource = null;
            this.dgResult.DataBind();
        }

        /// <summary>
        /// Metodo per la verificare che i prerequisiti siano rispettati. In particolare, devono essere stati 
        /// impostati i ruoli e devono essere presenti dei filtri nel call context
        /// </summary>
        /// <returns>True se si può proseguire con la ricerca</returns>
        private bool CheckInput()
        {
            return this.NewRole != null && this.RoleToReplace != null &&
                ModelliTrasmManager.SearchFilters != null;
        }

        /// <summary>
        /// Metodo per l'attuazione dell'operazione richiesta
        /// </summary>
        /// <param name="currentIndex">Indice della pagina attuale del wizard. La prima pagina corrisponde a ricerca mentre la seconda a sostituzione</param>
        private void ExecuteAction(int currentIndex)
        {
            FindAndReplaceResponse response = null;

            switch (currentIndex)
            {
                case 0:
                    response = ModelliTrasmManager.FindAndReplaceRolesInModelliTrasmissione(this.RoleToReplace, this.NewRole, FindAndReplaceEnum.Find, ModelliTrasmManager.SearchFilters, this.GetUser(), Session["AMMDATASET"] != null, null, false);
                    this.ModelloTrasmissioneArray = response.Models;
                    break;
                default:
                    this.MessageBox.ShowMessage("Operazione non valida");
                    break;
            }

            if (response.Models != null && response.Models.Length == 0)
                this.dgResult.DataSource = null;
            else
                this.dgResult.DataSource = response.Models;
            this.dgResult.DataBind();

        }

        /// <summary>
        /// Metodo per la generazione delle informaizoni sull'utente
        /// </summary>
        /// <returns>Informazioni sull'utente</returns>
        private InfoUtente GetUser()
        {
            InfoUtente userInfo = null;

            userInfo = UserManager.getInfoUtente();

            // Se userInfo è nullo, si potrebbe trattare di amministratore
            if (userInfo == null)
            {
                SessionManager sm = new SessionManager();
                userInfo = sm.getUserAmmSession();
                string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];
                userInfo.idAmministrazione = new DocsPaWebService().getIdAmmByCod(codiceAmministrazione);
            }

            return userInfo;
        }

        /// <summary>
        /// Url completo della pagina
        /// </summary>
        public static String ScriptToOpenPage
        {
            get
            {
               
                return String.Format(
                    //"window.showModelessDialog('{0}/popup/FindAndReplaceModelliTrasmissione.aspx', '', 'dialogHeight: 400px; dialogWidth:755px; resizable: no;status:no;scroll:yes;help:no;close:no;center:yes;');",
                    "window.showModalDialog('{0}/popup/FindAndReplaceModelliTrasmissione.aspx', '', 'dialogHeight: 400px; dialogWidth:755px; resizable: no;status:no;scroll:yes;help:no;close:no;center:yes;'); document.forms[0].submit();",
                        Utils.getHttpFullPath());
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            InfoUtente userInfo = UserManager.getInfoUtente();

            // Inizializzazione del call context
            if (CallContextStack.CurrentContext == null)
                CallContextStack.CurrentContext = new CallContext("ReportingContext");

            // Salvataggio della request nel call context
            if (this.ModelloTrasmissioneArray != null)
            {
                PrintReportObjectTransformationRequest request =
                    new PrintReportObjectTransformationRequest()
                    {
                        ContextName = "FindAndReplace",
                        SearchFilters = null,
                        UserInfo = userInfo,
                        DataObject = this.ModelloTrasmissioneArray,
                        AdditionalInformation = String.Format("Ruolo da sostituire: {0} ({1})\nRuolo da utilizzare per la sostituzione: {2} ({3})", 
                            this.txtFindDescrizione.Text, this.txtFindCodice.Text,
                            this.txtReplaceDescrizione.Text, this.txtReplaceCodice.Text)
                    };

                ReportingUtils.PrintRequest = request;

                ScriptManager.RegisterStartupScript(this, this.GetType(), "openReport", ReportingUtils.GetOpenReportPageScript(false), true);
            }
 
        }

        protected void btnClose_Click(object sender, EventArgs e)
        { 
            // Pulizia della sessione
            this.RoleToReplace = null;
            this.NewRole = null;
            this.ModelloTrasmissioneArray = null;
            ModelliTrasmManager.SearchFilters = null;
        }

        protected void SideBarList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            WizardStep dataitem = e.Item.DataItem as WizardStep;
            LinkButton lnkBtn = e.Item.FindControl("SideBarButton") as LinkButton;
            if (dataitem != null)
                lnkBtn.Enabled = (e.Item.ItemIndex <= wzWizard.ActiveStepIndex);
        }
        
    }
}
