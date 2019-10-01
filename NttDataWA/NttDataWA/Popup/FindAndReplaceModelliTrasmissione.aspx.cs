using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using System.Web;
using System.Collections.Generic;

namespace NttDataWA.popup
{
    public partial class FindAndReplaceModelliTrasmissione : System.Web.UI.Page
    {
        #region Varibili di CallContext

        /// <summary>
        /// Ruolo da sostituire
        /// </summary>
        private ElementoRubrica RoleToReplace
        {
            get
            {

                return HttpContext.Current.Session["RoleToReplace"] as ElementoRubrica;
            }

            set
            {
                HttpContext.Current.Session["RoleToReplace"] = value;
            }
        }

        /// <summary>
        /// Ruolo con cui sostituire
        /// </summary>
        private ElementoRubrica NewRole
        {
            get
            {
                return HttpContext.Current.Session["NewRole"] as ElementoRubrica;
            }

            set
            {
                HttpContext.Current.Session["NewRole"] = value;
            }
        }

        private ModelloTrasmissioneSearchResult[] ModelloTrasmissioneArray 
        {
            get
            {
                return HttpContext.Current.Session["ModelloTrasmissioneArray"] as ModelloTrasmissioneSearchResult[];
            }

            set
            {
                HttpContext.Current.Session["ModelloTrasmissioneArray"] = value;
            }
        }

        public RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_PROTO_IN;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }

        }


        #endregion

        #region Property

        private PrintReportRequest RequestPrintReport
        {
            get
            {
                if (HttpContext.Current.Session["requestPrintReport"] != null)
                    return (PrintReportRequest)HttpContext.Current.Session["requestPrintReport"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["requestPrintReport"] = value;
            }
        }


        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            // Inizializzazione drop down dei registri
            if (!IsPostBack)
            {
                InitializeLanguage();
                this.Initialize();
            }

            if (Session["rubricaFind"] != null)
                this.AnalyzeRoleResult();

            this.RefreshScript();
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
                Registro[] registri = UserManager.GetRegistriByRuolo(this, UserManager.GetSelectedRole().systemId);

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
                string msg = "WarningModelliNoCorrispondente";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);                
                this.txtFindCodice.Text = String.Empty;
            }
        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try
            {
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> ccList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.Cc"];
                string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();

                switch (addressBookCallFrom)
                {
                    case "M_D_F_R":
                        if (atList != null && atList.Count > 0)
                        {
                            NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                            Corrispondente tempCorrSingle;
                            if (!corrInSess.isRubricaComune)
                                tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                            else
                                tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                            this.txtFindCodice.Text = tempCorrSingle.codiceRubrica;
                            this.txtFindDescrizione.Text = tempCorrSingle.descrizione;

                            this.imgSrcFind_Click(null, null);
                            
                        }
                        break;

                    case "M_D_T_R":
                          if (atList != null && atList.Count > 0)
                        {
                            NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                            Corrispondente tempCorrSingle;
                            if (!corrInSess.isRubricaComune)
                                tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                            else
                                tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                            this.txtReplaceCodice.Text = tempCorrSingle.codiceRubrica;
                            this.txtReplaceDescrizione.Text = tempCorrSingle.descrizione;

                          
                            this.imgSrcReplace_Click(null, null);
                        }
                        break;


                }
                HttpContext.Current.Session["AddressBook.At"] = null;
                HttpContext.Current.Session["AddressBook.Cc"] = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void imgRubricaFind_Click(object sender, ImageClickEventArgs e)
        {
            // Registrazione script per l'apertura della rubrica e salvataggio in sessione di un valore che
            // consenta di capire che alla chiusura della finestra della rubrica deve essere compilato il
            // codice e la descrizione per il ruolo da sostituire
            Session["rubricaFind"] = this.txtFindCodice.ID;

            

            Registro reg = (NttDataWA.DocsPaWR.Registro)Session["userRegistro"];
            if (reg == null)
                reg = new Registro();

            reg.systemId = this.ddlRegistry.SelectedValue.ToString();
            reg.codRegistro = this.ddlRegistry.SelectedItem.Text.ToString();
            Session.Add("userRegistro", reg);
            this.CallType = RubricaCallType.CALLTYPE_FIND_ROLE;
            HttpContext.Current.Session["AddressBook.from"] = "M_D_F_R";
            HttpContext.Current.Session["AddressBook.EnableOnly"] = "R";
            HttpContext.Current.Session["fromFindModelli"] = true;
            
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AddressBook", "ajaxModalPopupAddressBook();", true);

            
            
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
                string msg = "WarningModelliNoCorrispondente";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                
                this.txtReplaceCodice.Text = String.Empty;
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.btnExport.Text = Utils.Languages.GetLabelFromCode("ReportGeneratorExportBtn", language);
            this.btnClose.Text = Utils.Languages.GetLabelFromCode("ReportGeneratorClose", language);
            this.lblRuoloFind.Text = Utils.Languages.GetLabelFromCode("ModelliLblRuoloFind", language);
            this.lblRuoloReplace.Text = Utils.Languages.GetLabelFromCode("ModelliLblRuoloReplace", language);
            //this.lgnTitle.InnerText = Utils.Languages.GetLabelFromCode("ModelliLgnTitle", language);
            this.lblRegistroFind.Text = Utils.Languages.GetLabelFromCode("ModelliLblRegistroFind", language);
            this.chkCopyNotes.Text = Utils.Languages.GetLabelFromCode("ModelliChkCopyNotes", language);
            this.ltlNoModels.Text = Utils.Languages.GetLabelFromCode("ModelliLtlNoModels", language);
            this.imgSrcFind.ToolTip = Utils.Languages.GetLabelFromCode("imgSrcDestinatari", language);
            this.imgSrcFind.AlternateText = Utils.Languages.GetLabelFromCode("imgSrcDestinatari", language);
            this.imgSrcReplace.AlternateText = Utils.Languages.GetLabelFromCode("imgSrcDestinatari", language);
            this.imgSrcReplace.ToolTip = Utils.Languages.GetLabelFromCode("imgSrcDestinatari", language);
            this.imgRubricaFind.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgCustomCorrespondentAddressBookCustom", language);
            this.imgRubricaFind.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgCustomCorrespondentAddressBookCustom", language);
            this.imgRubricaReplace.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgCustomCorrespondentAddressBookCustom", language);
            this.imgRubricaReplace.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgCustomCorrespondentAddressBookCustom", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
            this.ReportGenerator.Title = Utils.Languages.GetLabelFromCode("ReportGeneratorLblReportGenerator", language);
        }

        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        

        

        protected void imgRubricaReplace_Click(object sender, ImageClickEventArgs e)
        {
            Registro reg = (NttDataWA.DocsPaWR.Registro)Session["userRegistro"];
            if (reg == null)
                reg = new Registro();

            

            reg.systemId = this.ddlRegistry.SelectedValue.ToString();
            reg.codRegistro = this.ddlRegistry.SelectedItem.Text.ToString();
            Session.Add("userRegistro", reg);

            // Registrazione script per l'apertura della rubrica e salvataggio in sessione di un valore che
            // consenta di capire che alla chiusura della finestra della rubrica deve essere compilato il
            // codice e la descrizione per il ruolo con cui sostiuire
            
            this.CallType = RubricaCallType.CALLTYPE_REPLACE_ROLE;
            HttpContext.Current.Session["AddressBook.from"] = "M_D_T_R";
            HttpContext.Current.Session["AddressBook.EnableOnly"] = "R";
            HttpContext.Current.Session["fromFindModelli"] = true;
            
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AddressBook", "ajaxModalPopupAddressBook();", true);
            Session["rubricaFind"] = this.txtReplaceCodice.ID;
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
                qco.tipoIE = NttDataWA.DocsPaWR.AddressbookTipoUtente.INTERNO;
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
            

            switch (Session["rubricaFind"].ToString())
            {

                case "txtFindCodice":
                    {
                        try
                        {
                            List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                            string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();

                            switch (addressBookCallFrom)
                            {
                                case "M_D_T_F":
                                    if (atList != null && atList.Count > 0)
                                    {
                                        NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                                        Corrispondente tempCorrSingle;
                                        if (!corrInSess.isRubricaComune)
                                            tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                                        else
                                            tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                                        this.txtFindCodice.Text = tempCorrSingle.codiceRubrica;
                                        this.txtFindDescrizione.Text = tempCorrSingle.descrizione;
                                        ElementoRubrica[] elems = this.ResolveCode(this.txtFindCodice.Text, true, RubricaCallType.CALLTYPE_FIND_ROLE);
                                        if (elems != null && elems.Length > 0)
                                        {
                                            this.RoleToReplace = elems[0];
                                        }
                                        HttpContext.Current.Session["AddressBook.At"] = null;
                                        HttpContext.Current.Session["AddressBook.Cc"] = null;
                                        this.Page_Load(null, null);
                                        
                                        
                                    }
                                    break;                                
                            }                            
                        }
                        catch (System.Exception ex)
                        {
                            UIManager.AdministrationManager.DiagnosticError(ex);
                            return;
                        }
                    }
                    break;

                case "txtReplaceCodice":
                    {
                        try
                        {
                            List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                            string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();

                            switch (addressBookCallFrom)
                            {                                
                                case "M_D_T_R":
                                    if (atList != null && atList.Count > 0)
                                    {
                                        NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                                        Corrispondente tempCorrSingle;
                                        if (!corrInSess.isRubricaComune)
                                            tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                                        else
                                            tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                                        this.txtReplaceCodice.Text = tempCorrSingle.codiceRubrica;
                                        this.txtReplaceDescrizione.Text = tempCorrSingle.descrizione;
                                        ElementoRubrica[] elems = this.ResolveCode(this.txtReplaceCodice.Text, true, RubricaCallType.CALLTYPE_REPLACE_ROLE);
                                        if (elems != null && elems.Length > 0)
                                        {
                                            this.NewRole = elems[0];
                                        }
                                        HttpContext.Current.Session["AddressBook.At"] = null;
                                        HttpContext.Current.Session["AddressBook.Cc"] = null;
                                        this.Page_Load(null, null);                                                                                
                                    }
                                    break;                                
                            }                            
                        }
                        catch (System.Exception ex)
                        {
                            UIManager.AdministrationManager.DiagnosticError(ex);
                            return;
                        }
                        
                    }
                    break;
                    
            }

            // Pulizia sessione
            Session.Remove("rubricaFind");            

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

                string msg = "WarningRuoloSostituto";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                
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
                    string msg = "ErrorModelliOperationNotValid";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);                    
                    
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

            userInfo = UserManager.GetInfoUser();

            // Se userInfo è nullo, si potrebbe trattare di amministratore
            if (userInfo == null)
            {
                SessionManager sm = new SessionManager();
                userInfo = sm.getUserAmmSession();
                string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];
                userInfo.idAmministrazione = AdministrationManager.AmmGetInfoAmmCorrente(codiceAmministrazione).Codice;
            }

            return userInfo;
        }

        

        private PrintReportObjectTransformationRequest RequestPrintReportFind
        {
            get
            {
                if (HttpContext.Current.Session["requestPrintReportFind"] != null)
                    return (PrintReportObjectTransformationRequest)HttpContext.Current.Session["requestPrintReportFind"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["requestPrintReportFind"] = value;
            }
        }

        private bool ReadOnlySubtitle
        {
            set
            {
                HttpContext.Current.Session["readOnlySubtitle"] = value;
            }
        }
       

        protected void btnExport_Click(object sender, EventArgs e)
        {
            InfoUtente userInfo = UserManager.GetInfoUser();

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
                // Salvataggio della request nel call context 
                //PrintReportRequestDataset request =
                //    new PrintReportRequestDataset()
                //    {
                //        ContextName = "FindAndReplace",
                //        SearchFilters = null,
                //        UserInfo = userInfo,
                //        AdditionalInformation = String.Format("Ruolo da sostituire: {0} ({1})\nRuolo da utilizzare per la sostituzione: {2} ({3})",
                //            this.txtFindDescrizione.Text, this.txtFindCodice.Text,
                //            this.txtReplaceDescrizione.Text, this.txtReplaceCodice.Text)
                //    };

                ReadOnlySubtitle = true;
                RequestPrintReport = request;
                //RequestPrintReport = request as PrintReportRequestDataset;
                HttpContext.Current.Session["fromFindReplace"] = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ReportGenerator", "ajaxModalPopupReportGenerator();", true);
            }
 
        }

        protected void btnClose_Click(object sender, EventArgs e)
        { 
            // Pulizia della sessione
            this.RoleToReplace = null;
            this.NewRole = null;
            this.ModelloTrasmissioneArray = null;
            ModelliTrasmManager.SearchFilters = null;
            ScriptManager.RegisterClientScriptBlock(this.upButtons, this.upButtons.GetType(), "closeAJM", "parent.closeAjaxModal('FindAndReplace','up');", true);
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
