using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDatalLibrary;
using System.Data;

namespace NttDataWA.Management
{
    public partial class SignatureProcesses : System.Web.UI.Page
    {
        #region Property

        private List<FiltroProcessoFirma> FiltersProcesses
        {
            get
            {
                return (List<FiltroProcessoFirma>)HttpContext.Current.Session["FiltroProcessoFirma"];
            }
            set
            {
                HttpContext.Current.Session["FiltroProcessoFirma"] = value;
            }
        }

        private List<ProcessoFirma> ListaProcessiDiFirma
        {
            get
            {
                if (HttpContext.Current.Session["ListaProcessiDiFirma"] != null)
                    return (List<ProcessoFirma>)HttpContext.Current.Session["ListaProcessiDiFirma"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ListaProcessiDiFirma"] = value;
            }
        }

        private ProcessoFirma ProcessoDiFirmaSelected
        {
            get
            {
                if (HttpContext.Current.Session["ProcessoDiFirmaSelected"] != null)
                    return (ProcessoFirma)HttpContext.Current.Session["ProcessoDiFirmaSelected"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ProcessoDiFirmaSelected"] = value;
            }
        }

        private PassoFirma PassoDiFirmaSelected
        {
            get
            {
                if (HttpContext.Current.Session["PassoDiFirmaSelected"] != null)
                    return (PassoFirma)HttpContext.Current.Session["PassoDiFirmaSelected"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["PassoDiFirmaSelected"] = value;
            }
        }

        private RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_PROTO_INT_DEST;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }
        }

        private string IdProcesso
        {
            set
            {
                HttpContext.Current.Session["IdProcesso"] = value;
            }
        }

        #endregion

        #region Constants
        private const string CLOSE_POPUP_ADDRESS_BOOK = "closePopupAddressBook";
        private const string ELETTRONICA = "SIGN_E";
        private const string DIGITALE = "SIGN_D";
        private const string SIGN = "F";
        private const string WAIT = "W";
        private const string EVENT = "E";
        private const string CHECK_INSERT_IN_LF = "INSERIMENTO_DOCUMENTO_LF";
        private const string CHECK_ERRORE_PASSO_AUTOMATICO = "ERRORE_PASSO_AUTOMATICO";
        private const string CHECK_CONCLUSIONE_PROCESSO_LF = "CONCLUSIONE_PROCESSO_LF";
        private const string CHECK_INTERROTTO_PROCESSO = "INTERROTTO_PROCESSO";
        private const char ROLE_DISABLED = 'R';
        private const char USER_DISABLED = 'U';
        private const char REGISTRO_DISABLED = 'F';
        private const string RUOLO = "R";
        private const string TIPO_RUOLO = "TR";
        #endregion

        #region Standard Method
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.InitializeLanguage();
                this.InitializaPage();
            }
            else
            {
                ReadRetValueFromPopup();
            }

            RefreshScript();
        }

        private void LoadKeys()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_TIPO_RUOLO_LIBRO_FIRMA.ToString())) && Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_TIPO_RUOLO_LIBRO_FIRMA.ToString()) != "0")
            {
                this.PnlRoleOrTypeRole.Visible = true;
                this.LoadDdlTypeRole();
            }
        }

        public bool IsRoleCreateModelProcessEnabled()
        {
            bool result = false;
            result = UIManager.UserManager.IsAuthorizedFunctions("DO_CREATE_MODEL_PROCESS");
            return result;
        }

        private void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.HiddenRemoveSignatureProcess.Value))
            {
                this.RimuoviProcessoDiFirma();
                this.HiddenRemoveSignatureProcess.Value = string.Empty;
                return;
            }
            if (!string.IsNullOrEmpty(this.HiddenEventWithoutWait.Value))
            {
                this.HiddenEventWithoutWait.Value = string.Empty;
                this.AddStep();
                return;
            }
            if (!string.IsNullOrEmpty(this.HiddenRemoveStepSignatureProcess.Value))
            {
                this.RimuoviPassoProcessoDiFirma();
                this.HiddenRemoveStepSignatureProcess.Value = string.Empty;
                return;
            }
            if (!string.IsNullOrEmpty(this.AddNewProcess.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('AddNewProcess','');", true);
                this.CalcolaProssimoPasso(ProcessoDiFirmaSelected);
                this.ListaProcessiDiFirma.Add(ProcessoDiFirmaSelected);
                this.AddNodeToTop(ProcessoDiFirmaSelected).Select();
                this.txtNameSignatureProcesses.Text = ProcessoDiFirmaSelected.nome;
                this.TreeSignatureProcess.SelectedNode.Expand();
                this.UpdateContentPage();
                this.upPnlTreeSignatureProcess.Update();
            }
            if (!string.IsNullOrEmpty(this.CopySignatureProcess.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('CopySignatureProcess','');", true);
                if (FiltersProcesses != null)
                    ListaProcessiDiFirma = SignatureProcessesManager.GetProcessiDiFirmaByFilter(this.FiltersProcesses);
                else
                    LoadProcessiDiFirma();
                TreeviewProcesses_Bind();
                this.UpdateContentPage();
                this.upPnlTreeSignatureProcess.Update();
            }
            if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_ADDRESS_BOOK)))
            {
                string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();
                switch (addressBookCallFrom)
                {
                    case "VISIBILITY_SIGNATURE_PROCESS":
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_VisibilitySignatureProcess').contentWindow.closeAddressBookPopup();", true);
                        HttpContext.Current.Session["AddressBook.from"] = null;
                        break;
                    case "SIGNATURE_PROCESS":
                        btnAddressBookPostback();
                        HttpContext.Current.Session["AddressBook.from"] = null;
                        break;
                    case "FILTER_SIGNATURE_PROCESS_ROLE":
                    case "FILTER_SIGNATURE_PROCESS_USER":
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_AddFilterSignatureProcesses').contentWindow.closeAddressBookPopup();", true);
                        break;
                    case "FILTER_VISIBILITY_SIGNATURE_PROCESS":
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_VisibilitySignatureProcess').contentWindow.document.getElementById('ifrm_AddFilterVisibilitySignatureProcess').contentWindow.closeAddressBookPopup();", true);
                        HttpContext.Current.Session["AddressBook.from"] = null;
                        break;
                    case "COPIA_PROCESSO":
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_CopySignatureProcess').contentWindow.closeAddressBookPopup();", true);
                        HttpContext.Current.Session["AddressBook.from"] = null;
                        break;
                }
                return;
            }
            if (!string.IsNullOrEmpty(this.AddFilterSignatureProcesses.ReturnValue))
            {
                this.ProcessoDiFirmaSelected = null;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('AddFilterSignatureProcesses','');", true);
                ListaProcessiDiFirma = SignatureProcessesManager.GetProcessiDiFirmaByFilter(this.FiltersProcesses);
                TreeviewProcesses_Bind();
                this.upPnlTreeSignatureProcess.Update();
                UpdateContentPage();

                this.IndexImgRemoveFilter.Enabled = true;
                this.UpPnlBAction.Update();
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.ManagementSignatureProcesses.Text = Utils.Languages.GetLabelFromCode("ManagementSignatureProcesses", language);
            this.linkSignatureProcesses.Text = Utils.Languages.GetLabelFromCode("linkSignatureProcesses", language);
            this.SignatureProcessesBtnNew.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesBtnNew", language);
            this.SignatureProcessesBtnSave.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesBtnSave", language);
            this.SignatureProcessesBtnRemove.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesBtnRemove", language);
            this.lblNameSignatureProcesses.Text = Utils.Languages.GetLabelFromCode("lblNameSignatureProcesses", language);
            
            this.LitSignatureProcessesRole.Text = Utils.Languages.GetLabelFromCode("LitSignatureProcessesRole", language);

            this.ltlSignatureProcessesTypeSignature.Text = Utils.Languages.GetLabelFromCode("ltlSignatureProcessesTypeSignature", language);
            this.ltlSignatureProcessesTypeSignatureD.Text = Utils.Languages.GetLabelFromCode("ltlSignatureProcessesTypeSignatureD", language);
            this.ltlSignatureProcessesTypeSignatureE.Text = Utils.Languages.GetLabelFromCode("ltlSignatureProcessesTypeSignatureE", language);
            this.ltlOptionNotify.Text = Utils.Languages.GetLabelFromCode("ltlSignatureProcessesltlOptionNotify", language);
            this.ltlNotes.Text = Utils.Languages.GetLabelFromCode("ltlSignatureProcessesltlNotes", language);
            this.ltrNotes.Text = Utils.Languages.GetLabelFromCode("DocumentLitObjectChAv", language);
            this.ltlUtenteCoinvolto.Text = Utils.Languages.GetLabelFromCode("ltlUtenteCoinvolto", language);
            this.ddlUtenteCoinvolto.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("ddlUtenteCoinvolto", language));
            this.btnAddStep.ToolTip = Utils.Languages.GetLabelFromCode("SignatureProcessesBtnAddStep", language);
            this.ltlNr.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLtlNr", language);
            this.lblSectionDocument.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLblSectionDocument", language);
            this.btnAddStep.ToolTip = Utils.Languages.GetLabelFromCode("SignatureProcessesBtnAddStepToolTip", language);
            this.BtnDeleteStep.ToolTip = Utils.Languages.GetLabelFromCode("SignatureProcessesBtnDeleteStepToolTip", language);
            this.SignatureProcessesBtnVisibility.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesBtnVisibility", language);
            this.VisibilitySignatureProcess.Title = Utils.Languages.GetLabelFromCode("SignatureProcessesPopupVisibility", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddFilterAddressBookTitle", language);
            //this.optDigitale.Text = Utils.Languages.GetLabelFromCode("SignatureProcessOptDigitale", language);
            //this.optElettronica.Text = Utils.Languages.GetLabelFromCode("SignatureProcessOptElettronica", language);
            this.SignatureProcessesStatistics.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesStatistics", language);
            this.StatisticsSignatureProcess.Title = Utils.Languages.GetLabelFromCode("SignatureProcessesStatistics", language);
            this.LtlTypeStep.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesTypeStep", language);
            this.optSign.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesOptSign", language);
            this.optWait.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesOptWait", language);
            this.optEvent.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesOptEvent", language);
            this.LtlTypeEvent.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLtlTypeEvent", language);
            this.DdlTypeEvent.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SignatureProcessesDdlTypeEvent", language));
            this.MessangerWarning.Text = Utils.Languages.GetLabelFromCode("MessangerWarning", language);
            this.optRole.Text = Utils.Languages.GetLabelFromCode("SignatureProcessOptRole", language);
            this.optTypeRole.Text = Utils.Languages.GetLabelFromCode("SignatureProcessOptTypeRole", language);
            this.DdlTypeRole.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SignatureProcessesDdlTypeRole", language));
            this.LtlTypeRole.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLtlTypeRole", language);
            this.IndexImgAddFilter.ToolTip = Utils.Languages.GetLabelFromCode("IndexImgAddFilterTooltip", language);
            this.IndexImgAddFilter.AlternateText = Utils.Languages.GetLabelFromCode("IndexImgAddFilterTooltip", language);
            this.IndexImgRemoveFilter.ToolTip = Utils.Languages.GetLabelFromCode("IndexImgRemoveFilterTooltip", language);
            this.IndexImgRemoveFilter.AlternateText = Utils.Languages.GetLabelFromCode("IndexImgRemoveFilterTooltip", language);
            this.AddFilterSignatureProcesses.Title = Utils.Languages.GetLabelFromCode("AddFilterPopup", language);
            this.SignatureProcessesBtnDuplica.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesBtnDuplica", language);
            this.SignatureProcessesBtnDuplica.ToolTip = Utils.Languages.GetLabelFromCode("SignatureProcessesBtnDuplicaTooltip", language);
            this.SignatureProcessesBtnDuplica.ToolTip = Utils.Languages.GetLabelFromCode("SignatureProcessesBtnDuplicaTooltip", language);
            this.SignatureProcessesBtnCopia.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesBtnCopia", language);
            this.SignatureProcessesBtnCopia.ToolTip = Utils.Languages.GetLabelFromCode("SignatureProcessesBtnCopiaTooltip", language);
            this.CopySignatureProcess.Title = Utils.Languages.GetLabelFromCode("CopySignatureProcesso", language);
            this.AddNewProcess.Title = Utils.Languages.GetLabelFromCode("AddNewProcessDuplicaProcesso", language);
            this.ltlRegistroAOO.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLtlRegistroAOO", language);
            this.DdlRegistroAOO.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SignatureProcessesDdlRegistroAOO", language));
            this.ltlRegistroRF.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLtlRegistroRFSegnatura", language);
            this.ltlElencoCaselle.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLtlElencoCaselle", language);
            this.DdlRegistroRF.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SignatureProcessesDdlRegistroRF", language));
            this.DdlRegistroRF.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SignatureProcessesDdlRegistroRF", language));
            this.DdlElencoCaselle.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SignatureProcessesDdlElencoCaselle", language));
            if (!IsRoleCreateModelProcessEnabled())
            {
                this.ltlRegistroAOO.Text += "*";
                this.LitSignatureProcessesRole.Text += "*";
                this.ltlElencoCaselle.Text += "*";
                this.LblFacoltativo.Visible = false;
                this.cbxFacoltativo.Visible = false;
            }
            this.LtlTipologiaDocumento.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLtlTipologiaDocumento", language);
            this.LtlStatoDiagramma.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLtlStatoDiagramma", language);
            this.DdlTipologiaDocumento.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SignatureProcessesDdlTipologiaDocumento", language));
            this.DdlStatoDiagramma.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SignatureProcessesDdlStatoDiagramma", language));
            this.LblStatoDiagrammaInterruzione.Text = Utils.Languages.GetLabelFromCode("LblStatoDiagrammaInterruzione", language);
            this.DdlStatoDiagrammaInterruzione.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SignatureProcessesDdlStatoDiagramma", language));
        }

        private void InitializaPage()
        {
            try
            {
                ClearSession();
                LoadProcessiDiFirma();
                TreeviewProcesses_Bind();
                this.LoadEventNotification(); 
                this.LoadEventTypes();
                this.LoadKeys();
                this.Bind_RblSignature();
                this.UpdateContentPage();
                string dataUser = RoleManager.GetRoleInSession().systemId;

                Registro reg = RegistryManager.GetRegistryInSession();
                if (reg == null)
                {
                    reg = RoleManager.GetRoleInSession().registri[0];
                }
                dataUser = dataUser + "-" + reg.systemId;
                this.RapidRole.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + "CALLTYPE_IN_ONLY_ROLE";
            }
            catch (Exception ex)
            {
                string msg = "ErrorSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OnlyNumbers", "OnlyNumbers();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshprojectTxtDescrizione", "charsLeft('txtNotes', '2000' , '" + this.ltrNotes.Text.Replace("'", "\'") + "');", true);
            this.txtNotes_chars.Attributes["rel"] = "txtNotes_'2000'_" + this.ltrNotes.Text;
        }


        private void ClearSession()
        {
            HttpContext.Current.Session.Remove("ProcessoDiFirmaSelected");
            HttpContext.Current.Session.Remove("PassoDiFirmaSelected");
            HttpContext.Current.Session.Remove("ListaProcessiDiFirma");
            HttpContext.Current.Session.Remove("FiltroProcessoFirma");
        }
        #endregion

        #region Event Buttons

        protected void SignatureProcessesBtnNew_Click(object sender, EventArgs e)
        {
            try
            {
                this.ClearFieldsStep();
                this.ClearFieldsProcess();

                this.ProcessoDiFirmaSelected = new ProcessoFirma();
                if (this.TreeSignatureProcess.SelectedNode != null)
                {
                    this.TreeSignatureProcess.SelectedNode.Selected = false;
                }
                this.UpdateContentPage();
                this.upPnlTreeSignatureProcess.Update();
            }
            catch (Exception ex)
            {
                string msg = "ErrorSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        protected void DdlTypeEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.cbx_automatico.Visible = false;
                this.cbx_automatico.Checked = false;
                this.ResetCampiAutomatciPassoSelezionato();
                if (!string.IsNullOrEmpty(DdlTypeEvent.SelectedValue))
                {
                    if (SignatureProcessesManager.IsEventoAutomatico(DdlTypeEvent.SelectedValue))
                    {
                        Azione azione = (Azione)Enum.Parse(typeof(Azione), this.DdlTypeEvent.SelectedValue, true);
                        //Verifico se il ruolo è abilitato alla creazione del passo automatico
                        switch (azione)
                        {
                            case Azione.RECORD_PREDISPOSED:
                                this.cbx_automatico.Visible = UserManager.IsAuthorizedFunctions("CREA_PASSO_PROTO_AUTO");
                                break;
                            case Azione.DOCUMENTOSPEDISCI:
                                this.cbx_automatico.Visible = UserManager.IsAuthorizedFunctions("CREA_PASSO_SPEDIZIONE_AUTO");
                                break;
                            case Azione.DOCUMENTO_REPERTORIATO:
                                this.cbx_automatico.Visible = UserManager.IsAuthorizedFunctions("CREA_PASSO_REPERTORIAZIONE_AUTO");
                                break;
                            case Azione.DOC_CAMBIO_STATO:
                                this.cbx_automatico.Visible = UserManager.IsAuthorizedFunctions("CREA_PASSO_CAMBIO_STATO_AUTO");
                                break;
                        }
                    }
                }
                this.UpTypeEvent.Update();
            }
            catch(Exception ex)
            {

            }
        }

        private void AggiornaCampiAuotomaticPassoSelezionato()
        {
            this.PassoDiFirmaSelected.IdAOO = this.DdlRegistroAOO.SelectedValue;
            this.PassoDiFirmaSelected.IdRF = this.DdlRegistroRF.SelectedValue;
            this.PassoDiFirmaSelected.IdMailRegistro = this.DdlElencoCaselle.SelectedValue;
        }

        private void ResetCampiAutomatciPassoSelezionato()
        {
            this.VisibilitaCampiAutomatici(this.cbx_automatico.Checked, null);
        }

        protected void cbx_automatico_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if(this.cbx_automatico.Checked)
                {
                    Utente utenteAutomatico = UserManager.GetUtenteAutomatico();
                    if (utenteAutomatico == null || string.IsNullOrEmpty(utenteAutomatico.idPeople))
                    {
                        this.cbx_automatico.Checked = false;
                        string msg = "WarningUtenteAutomaticoNonConfigurato";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                }
                else
                {
                    //Se il passo non è automatico e ho selezionato il ruolo, popolo la combo degli utenti
                    if (this.PassoDiFirmaSelected.ruoloCoinvolto != null)
                      this.LoadDllUtenteCoinvolto(UIManager.UserManager.getUserInRoleByIdGruppo(this.PassoDiFirmaSelected.ruoloCoinvolto.idGruppo));
                }
                this.ResetCampiAutomatciPassoSelezionato();
            }
            catch (Exception ex)
            {

            }
        }
        
        protected void DdlRegistroAOO_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Azione codiceEvento = (Azione)Enum.Parse(typeof(Azione), this.DdlTypeEvent.SelectedValue, true);
                switch (codiceEvento)
                {
                    case Azione.RECORD_PREDISPOSED:
                    case Azione.DOCUMENTO_REPERTORIATO:
                        LoadRegistriRF();
                        break;
                    case Azione.DOCUMENTOSPEDISCI:
                        LoadRegistroRFSpedizione();
                        LoadElencoCaselleMittente();
                        break;
                }
                this.UpPnlRegistroRF.Update();
            }
            catch (Exception ex)
            {

            }
        }

        protected void DdlRegistroRF_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.PassoDiFirmaSelected.IdMailRegistro = string.Empty;
                Azione codiceEvento = (Azione)Enum.Parse(typeof(Azione), this.DdlTypeEvent.SelectedValue, true);
                if(codiceEvento.Equals(Azione.DOCUMENTOSPEDISCI))
                    LoadElencoCaselleMittente();
            }
            catch (Exception ex)
            {

            }
        }

        protected void DdlTipologiaDocumento_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.PassoDiFirmaSelected.IdTipologia = this.DdlTipologiaDocumento.SelectedValue;
                LoadStateDiagram();
            }
            catch (Exception ex)
            {

            }
        }

        protected void DdlStatoDiagramma_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if(!string.IsNullOrEmpty(this.DdlStatoDiagramma.SelectedValue))
                {
                    Stato stato = UIManager.DiagrammiManager.GetStatoById(this.DdlStatoDiagramma.SelectedValue);
                    if(stato != null && !string.IsNullOrEmpty(stato.ID_PROCESSO_FIRMA))
                    {
                        this.DdlStatoDiagramma.SelectedValue = string.Empty;
                        this.UpPnlStatoDiagramma.Update();
                        string msg = "WarningSignatureProcessStatoDiagrammaAssociatoAProcesso";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                    this.PassoDiFirmaSelected.IdStatoDiagramma = DdlStatoDiagramma.SelectedValue;
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Resistuisce l'elenco delle caselle associate al registro/rf per le quali il ruolo è abilitato in spedizione
        /// </summary>
        /// <returns>List CasellaRegistro</returns>
        private  List<CasellaRegistro> GetComboRegisterSend(string idRegistro, string idRuolo)
        {
            try
            {
                List<CasellaRegistro> listCaselle = new List<CasellaRegistro>();
                string casellaPrincipale = MultiBoxManager.GetMailPrincipaleRegistro(idRegistro);
                DataSet ds = MultiBoxManager.GetRightMailRegistro(idRegistro, idRuolo);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                        {
                            System.Text.StringBuilder formatMail = new System.Text.StringBuilder();
                            if (row["SPEDISCI"].ToString().Equals("1"))
                            {
                                listCaselle.Add(new CasellaRegistro
                                {
                                    Principale = row["EMAIL_REGISTRO"].ToString().Equals(casellaPrincipale) ? "1" : "0",
                                    EmailRegistro = row["EMAIL_REGISTRO"].ToString(),
                                    Note = row["VAR_NOTE"].ToString(),
                                    System_id = row["ID_MAIL_REGISTRI"].ToString()
                                });
                            }
                        }
                    }
                }
                return listCaselle;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        private void ResetCampiAutomatici()
        {
            this.PnlCampiPassoAutomatco.Visible = false;
            this.PnlElencoCaselle.Visible = false;
            this.DdlRegistroAOO.Items.Clear();
            this.DdlRegistroAOO.Enabled = false;
            this.DdlRegistroRF.Items.Clear();
            this.DdlRegistroRF.Enabled = false;
            this.DdlElencoCaselle.Items.Clear();
            this.DdlElencoCaselle.Enabled = false;
            this.PnlCampiCambioStatoAutomatico.Visible = false;
            this.DdlTipologiaDocumento.Items.Clear();
            this.DdlTipologiaDocumento.Enabled = false;
            this.DdlStatoDiagramma.Items.Clear();
            this.DdlStatoDiagramma.Enabled = false;
        }

        private void VisibilitaCampiAutomatici(bool visible, PassoFirma passo)
        {
            ResetCampiAutomatici();
            if (visible)
            {
                this.PnlCampiPassoAutomatco.Visible = true;
                this.PnlUenteCoinvolto.Visible = false;
                this.ddlUtenteCoinvolto.SelectedIndex = -1;
                this.ltlRegistroRF.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLtlRegistroRFSegnatura", UserManager.GetUserLanguage());
                Azione codiceEvento = (Azione)Enum.Parse(typeof(Azione), this.DdlTypeEvent.SelectedValue, true);
                switch (codiceEvento)
                {
                    case Azione.RECORD_PREDISPOSED:
                    case Azione.DOCUMENTO_REPERTORIATO:
                        LoadRegistriAOO();
                        if(passo != null)
                            this.DdlRegistroAOO.SelectedValue = passo.IdAOO;
                        LoadRegistriRF();
                        if(passo != null)
                            this.DdlRegistroRF.SelectedValue = passo.IdRF;
                        break;
                    case Azione.DOCUMENTOSPEDISCI:
                        this.ltlRegistroRF.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLtlRegistroRFSpedizione", UserManager.GetUserLanguage());
                        if (!IsRoleCreateModelProcessEnabled())
                            this.ltlRegistroRF.Text += "*";
                        this.PnlElencoCaselle.Visible = true;
                        LoadRegistriAOO();
                        if(passo != null)
                            this.DdlRegistroAOO.SelectedValue = passo.IdAOO;
                        LoadRegistroRFSpedizione();
                        if (passo != null)
                            this.DdlRegistroRF.SelectedValue = passo.IdRF;
                        LoadElencoCaselleMittente();
                        if(passo != null)
                            this.DdlElencoCaselle.SelectedValue = passo.IdMailRegistro;
                        break;
                    case Azione.DOC_CAMBIO_STATO:
                        this.PnlCampiPassoAutomatco.Visible = false;
                        this.PnlCampiCambioStatoAutomatico.Visible = true;
                        if (!IsRoleCreateModelProcessEnabled())
                        {
                            this.LtlTipologiaDocumento.Text += "*";
                            this.LtlStatoDiagramma.Text += "*";
                        }
                        LoadCustomDocuments();
                        if (passo != null)
                            this.DdlTipologiaDocumento.SelectedValue = passo.IdTipologia;
                        LoadStateDiagram();
                        if(passo != null)
                            this.DdlStatoDiagramma.SelectedValue = passo.IdStatoDiagramma;
                        break;
                }
            }
            else
            {
                this.PnlUenteCoinvolto.Visible = true;           
            }
            this.SetOpzioniNotifiche();
            this.UpdPnlUtenteCoinvolto.Update();
            this.UpPnlCampiPassoAutomatco.Update();
        }

        private void LoadRegistriAOO()
        {
            ListItem empty = new ListItem("", "");
            this.DdlRegistroAOO.Items.Add(empty);
            this.DdlRegistroAOO.SelectedIndex = -1;

            if (!string.IsNullOrEmpty(this.idRuolo.Value) && this.PassoDiFirmaSelected.ruoloCoinvolto != null)
            {
                this.DdlRegistroAOO.Enabled = true;
                Registro[] registriAOO = UIManager.RegistryManager.GetRegistriesByRole(this.PassoDiFirmaSelected.ruoloCoinvolto.systemId);
                foreach (DocsPaWR.Registro reg in registriAOO)
                {
                    if (!reg.flag_pregresso)
                    {
                        ListItem item = new ListItem();
                        item.Text = reg.codRegistro;
                        item.Value = reg.systemId;
                        this.DdlRegistroAOO.Items.Add(item);
                    }
                }
                if(this.DdlRegistroAOO.Items != null && this.DdlRegistroAOO.Items.Count == 2)
                {
                    this.DdlRegistroAOO.SelectedIndex = 1;
                }
                /*
                if (!string.IsNullOrEmpty(PassoDiFirmaSelected.IdAOO))
                {
                    this.DdlRegistroAOO.SelectedValue = PassoDiFirmaSelected.IdAOO;
                }
                */
                this.DdlRegistroAOO.Enabled = true;
            }
        }

        private void LoadRegistriRF()
        {
            this.DdlRegistroRF.Items.Clear();
            this.DdlRegistroRF.Enabled = false;
            ListItem empty = new ListItem("", "");
            this.DdlRegistroRF.Items.Add(empty);
            this.DdlRegistroRF.SelectedIndex = -1;

            string idAOO = this.DdlRegistroAOO.SelectedValue;
            if (!string.IsNullOrEmpty(idAOO))
            {
                Registro[] registriRfVisibili = UIManager.RegistryManager.GetListRegistriesAndRF(this.PassoDiFirmaSelected.ruoloCoinvolto.systemId, "1", idAOO);
                foreach (NttDataWA.DocsPaWR.Registro registro in registriRfVisibili)
                {
                    ListItem item = new ListItem(registro.codRegistro + " - " + registro.descrizione, registro.systemId);
                    this.DdlRegistroRF.Items.Add(item);
                }
                if (registriRfVisibili != null && registriRfVisibili.Length == 1)
                    this.DdlRegistroRF.SelectedIndex = 1;
                /*
                if (!string.IsNullOrEmpty(PassoDiFirmaSelected.IdRF))
                    this.DdlRegistroRF.SelectedValue = PassoDiFirmaSelected.IdRF;
                */
                this.DdlRegistroRF.Enabled = true;
            }
        }

        /// <summary>
        /// Seleziono solo gli Rf/registro visibili al ruolo abilitati alla spedizione
        /// </summary>
        private void LoadRegistroRFSpedizione()
        {
            this.DdlRegistroRF.Items.Clear();
            ListItem empty = new ListItem("", "");
            this.DdlRegistroRF.Items.Add(empty);
            this.DdlRegistroRF.SelectedIndex = -1;
            string idAOO = this.DdlRegistroAOO.SelectedValue;
            if (!string.IsNullOrEmpty(idAOO))
            {
                //prendo il registro corrente
                DataSet dsReg = MultiBoxManager.GetRightMailRegistro(idAOO, this.PassoDiFirmaSelected.ruoloCoinvolto.systemId);
                if (dsReg.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                {
                    foreach (DataRow row in dsReg.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                    {
                        if (row["SPEDISCI"].ToString().Equals("1"))
                        {
                            string codiceAoo = this.DdlRegistroAOO.SelectedItem.Text;
                            ListItem aoo = new ListItem(codiceAoo, idAOO);
                            this.DdlRegistroRF.Items.Add(aoo);
                            break;
                        }
                    }
                }
                NttDataWA.DocsPaWR.Registro[] rf = RegistryManager.GetListRegistriesAndRF(this.PassoDiFirmaSelected.ruoloCoinvolto.systemId, "1", idAOO);
                foreach (NttDataWA.DocsPaWR.Registro registro in rf)
                {
                    DataSet ds = MultiBoxManager.GetRightMailRegistro(registro.systemId, this.PassoDiFirmaSelected.ruoloCoinvolto.systemId);
                    if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                        {
                            if (row["SPEDISCI"].ToString().Equals("1"))
                            {
                                ListItem item = new ListItem(registro.codRegistro + " - " + registro.descrizione, registro.systemId);
                                this.DdlRegistroRF.Items.Add(item);
                                break;
                            }
                        }
                    }
                }
                if (this.DdlRegistroRF.Items.Count == 2)
                    this.DdlRegistroRF.SelectedIndex = 1;
                /*
                if (!string.IsNullOrEmpty(PassoDiFirmaSelected.IdRF))
                    this.DdlRegistroRF.SelectedValue = PassoDiFirmaSelected.IdRF;
                */
                this.DdlRegistroRF.Enabled = true;
            }
            else
            {
                this.DdlRegistroRF.Enabled = false;
            }     
        }

        private void LoadElencoCaselleMittente()
        {
            DdlElencoCaselle.Enabled = false;
            DdlElencoCaselle.Items.Clear();
            DdlElencoCaselle.Items.Add(new ListItem("", ""));
            string idRegistro = string.Empty;
            if (!string.IsNullOrEmpty(this.DdlRegistroRF.SelectedValue))
                idRegistro = this.DdlRegistroRF.SelectedValue;
            if (!string.IsNullOrEmpty(idRegistro))
            {
                List<DocsPaWR.CasellaRegistro> listCaselle = new List<DocsPaWR.CasellaRegistro>();
                listCaselle = GetComboRegisterSend(DdlRegistroRF.SelectedValue, this.PassoDiFirmaSelected.ruoloCoinvolto.systemId);
                if (listCaselle.Count > 0)
                {
                    DdlElencoCaselle.Enabled = true;
                    foreach (DocsPaWR.CasellaRegistro c in listCaselle)
                    {
                        System.Text.StringBuilder formatMail = new System.Text.StringBuilder();
                        if (c.Principale.Equals("1"))
                            formatMail.Append("* ");
                        formatMail.Append(c.EmailRegistro);
                        if (!string.IsNullOrEmpty(c.Note))
                        {
                            formatMail.Append(" - ");
                            formatMail.Append(c.Note);
                        }
                        DdlElencoCaselle.Items.Add(new ListItem(formatMail.ToString(), c.System_id));
                    }

                    foreach (ListItem i in DdlElencoCaselle.Items)
                    {
                        if (i.Text.Split(new string[] { "*" }, 2, System.StringSplitOptions.None).Length > 1)
                        {
                            DdlElencoCaselle.SelectedValue = i.Value;
                            break;
                        }
                    }
                    if (listCaselle.Count == 1)
                        DdlElencoCaselle.SelectedIndex = 1;

                    /*
                    //imposto la casella principale come selezionata
                    if (string.IsNullOrEmpty(this.PassoDiFirmaSelected.IdMailRegistro))
                    {
                        foreach (ListItem i in DdlElencoCaselle.Items)
                        {
                            if (i.Text.Split(new string[] { "*" }, 2, System.StringSplitOptions.None).Length > 1)
                            {
                                DdlElencoCaselle.SelectedValue = i.Value;
                                break;
                            }
                        }
                        if (listCaselle.Count == 1)
                            DdlElencoCaselle.SelectedIndex = 1;
                    }
                    else
                    {
                        DdlElencoCaselle.SelectedValue = PassoDiFirmaSelected.IdMailRegistro;
                    }
                    */
                }
            }
            else
            {
                this.DdlElencoCaselle.Items.Clear();
                this.DdlElencoCaselle.Enabled = false;
            }
            this.UpPnlElencoCaselle.Update();
        }

        protected void SignatureProcessesBtnSave_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(this.txtNameSignatureProcesses.Text))
            {
                msg = "WarningRequiredFieldNameProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                return;
            }

            this.ProcessoDiFirmaSelected.nome = this.txtNameSignatureProcesses.Text;
            try
            {
                //Creazione di un nuovo processo di firma
                if (string.IsNullOrEmpty(this.ProcessoDiFirmaSelected.idProcesso))
                {
                    ResultProcessoFirma result = ResultProcessoFirma.OK;
                    this.ProcessoDiFirmaSelected = SignatureProcessesManager.InsertProcessoDiFirma(this.ProcessoDiFirmaSelected, out result);
                    if (result.ToString().Equals(ResultProcessoFirma.OK.ToString()))
                    {
                        if (this.ProcessoDiFirmaSelected != null)
                        {
                            this.CalcolaProssimoPasso(ProcessoDiFirmaSelected);
                            this.ListaProcessiDiFirma.Add(ProcessoDiFirmaSelected);
                            this.AddNodeToTop(ProcessoDiFirmaSelected).Select();
                            this.TreeSignatureProcess.SelectedNode.Expand();
                            this.UpdateContentPage();
                        }
                        else
                        {
                            msg = "ErrorCreationProcess";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                            return;
                        }
                    }
                    else
                    {
                        switch (result)
                        {
                            case ResultProcessoFirma.EXISTING_PROCESS_NAME:
                                msg = "WarningSignatureProcessUniqueProcessName";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                                break;
                        }
                    }

                }
                else //salvataggio processo di firma già esistente
                {
                    //Se ho selezionato lo stato lo salvo
                    this.ProcessoDiFirmaSelected.IdStatoInterruzione = string.Empty;
                    if (!string.IsNullOrEmpty(this.DdlStatoDiagrammaInterruzione.SelectedValue))
                        this.ProcessoDiFirmaSelected.IdStatoInterruzione = this.DdlStatoDiagrammaInterruzione.SelectedValue;
                    ProcessoFirma processoAggiornato = UIManager.SignatureProcessesManager.AggiornaProcessoDiFirma(ProcessoDiFirmaSelected);
                    if (processoAggiornato != null)
                    {
                        //Aggiorno il processo in sessione
                        ProcessoFirma processo = (from pr in this.ListaProcessiDiFirma where pr.idProcesso.Equals(this.ProcessoDiFirmaSelected.idProcesso) select pr).FirstOrDefault();
                        processo = processoAggiornato;

                        //Aggiorno il Treeview
                        TreeNode nodoParent = this.TreeSignatureProcess.SelectedNode;
                        nodoParent.Text = processoAggiornato.nome;
                        nodoParent.Select();

                        /*
                        msg = "ConfirmProcessChange";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'check', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'check', '');}", true);
                        return;
                         * */
                    }
                    else
                    {
                        msg = "ErrorProcessChange";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                        return;
                    }
                }
                this.UpdateContentPage();
                this.upPnlTreeSignatureProcess.Update();
            }
            catch (Exception ex)
            {
                msg = "ErrorProcessChange";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        protected void SignatureProcessesBtnRemove_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmRemoveSignatureProcess', 'HiddenRemoveSignatureProcess', '','" + Utils.utils.FormatJs(this.ProcessoDiFirmaSelected.nome) + "');", true);
            return;
        }

        protected void SignatureProcessesStatistics_Click(object sender, EventArgs e)
        {
            try
            {
                this.IdProcesso = this.ProcessoDiFirmaSelected.idProcesso;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupStatisticsSignatureProcess", "ajaxModalPopupStatisticsSignatureProcess();", true);
                return;
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SignatureProcessesBtnVisibility_Click(object sender, EventArgs e)
        {
            try
            {
                if(this.ProcessoDiFirmaSelected != null && !string.IsNullOrEmpty(this.ProcessoDiFirmaSelected.idProcesso) 
                    && (this.ProcessoDiFirmaSelected.passi == null || this.ProcessoDiFirmaSelected.passi.Count() == 0))
                {
                    string msg = "WarningSignatureProcessesNoSteps";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupVisibilitySignatureProcess", "ajaxModalPopupVisibilitySignatureProcess();", true);
                return;
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void RblTypeStep_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.RblTypeStep.SelectedValue.Equals(SIGN))
                {
                    this.PnlTypeStep.Visible = true;
                    this.pnlSign.Visible = true;
                    this.rblTypeSignature.SelectedIndex = 0;
                    this.DdlTypeEvent.SelectedIndex = -1;
                    this.RblTypeSignature_SelectedIndexChanged(null, null);
                    this.PnlTypeEvent.Visible = false;
                }

                if (this.RblTypeStep.SelectedValue.Equals(WAIT))
                {
                    this.PnlTypeStep.Visible = false;
                    this.TxtCodeRole.Text = string.Empty;
                    this.TxtDescriptionRole.Text = string.Empty;
                    this.ddlUtenteCoinvolto.Items.Clear();
                    this.ddlUtenteCoinvolto.Enabled = false;
                    this.DdlTypeEvent.SelectedIndex = -1;
                    this.txtNotes.Text = string.Empty;
                }

                if (this.RblTypeStep.SelectedValue.Equals(EVENT))
                {
                    this.PnlTypeStep.Visible = true;
                    this.PnlTypeEvent.Visible = true;
                    this.pnlSign.Visible = false;
                }
                this.VisibilitaCampiAutomatici(false, null);
                this.UpTypeStep.Update();
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void SetOpzioniNotifiche()
        {
            if (this.RblTypeStep.SelectedValue.Equals(SIGN))
            {
                this.cbxOptionNotify.Items.FindByValue(CHECK_INSERT_IN_LF).Enabled = true;
                this.cbxOptionNotify.Items.FindByValue(CHECK_ERRORE_PASSO_AUTOMATICO).Attributes.Add("style", "display:none");
                this.cbxOptionNotify.Items.FindByValue(CHECK_ERRORE_PASSO_AUTOMATICO).Selected = false;
            }

            if (this.RblTypeStep.SelectedValue.Equals(WAIT))
            {
                this.cbxOptionNotify.Items.FindByValue(CHECK_INSERT_IN_LF).Attributes.Add("style", "display:none");
                this.cbxOptionNotify.Items.FindByValue(CHECK_ERRORE_PASSO_AUTOMATICO).Attributes.Add("style", "display:none");
                foreach (ListItem item in this.cbxOptionNotify.Items)
                {
                    item.Selected = false;
                }
            }

            if (this.RblTypeStep.SelectedValue.Equals(EVENT))
            {
                this.cbxOptionNotify.Items.FindByValue(CHECK_INSERT_IN_LF).Selected = false;
                this.cbxOptionNotify.Items.FindByValue(CHECK_INSERT_IN_LF).Enabled = false;
                this.cbxOptionNotify.Items.FindByValue(CHECK_ERRORE_PASSO_AUTOMATICO).Attributes.Add("style", "display:block");
                this.cbxOptionNotify.Items.FindByValue(CHECK_ERRORE_PASSO_AUTOMATICO).Selected = true;
                if (!this.cbx_automatico.Checked)
                {
                    this.cbxOptionNotify.Items.FindByValue(CHECK_ERRORE_PASSO_AUTOMATICO).Attributes.Add("style", "display:none");
                    this.cbxOptionNotify.Items.FindByValue(CHECK_ERRORE_PASSO_AUTOMATICO).Selected = false;                   
                }
                else
                {
                    this.cbxOptionNotify.Items.FindByValue(CHECK_CONCLUSIONE_PROCESSO_LF).Selected = true;
                    this.cbxOptionNotify.Items.FindByValue(CHECK_INTERROTTO_PROCESSO).Selected = true;
                }
            }
            this.UpdPnlNotififyOption.Update();
        }

        protected void RblTypeSignature_SelectedIndexChanged(object sender, EventArgs e)
        {
            setVisibilityTypeEvent();
            if (!string.IsNullOrEmpty(this.TxtCodeRole.Text))
            {
                if (!IsRuoloAbilitatoAlPassoSelezionato(this.PassoDiFirmaSelected.ruoloCoinvolto))
                {
                    string msg = "WarningRoleNotEnabledSign";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                }
            }
        }


        protected void RblRoleOrTypeRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.RblRoleOrTypeRole.SelectedValue.Equals(RUOLO))
            {
                this.DdlTypeRole.SelectedIndex = -1;
                this.PnlTypeRole.Attributes.Add("style", "display:none");
                this.PnlRole.Attributes.Add("style", "display:block");
                this.PnlUenteCoinvolto.Attributes.Add("style", "display:block");
            }
            else
            {
                this.PnlTypeRole.Attributes.Add("style", "display:block");
                this.PnlRole.Attributes.Add("style", "display:none");
                this.PnlUenteCoinvolto.Attributes.Add("style", "display:none");
                this.TxtCodeRole.Text = string.Empty;
                this.TxtDescriptionRole.Text = string.Empty;
                this.ddlUtenteCoinvolto.Items.Clear();
                this.ddlUtenteCoinvolto.Enabled = false;
            }
            this.UpdPnlRole.Update();
            this.UpdPnlTypeRole.Update();
            this.UpdPnlUtenteCoinvolto.Update();
        }

        protected void BtnAddressBook_Click(object sender, EventArgs e)
        {
            this.CallType = RubricaCallType.CALLTYPE_CORR_INT;
            HttpContext.Current.Session["AddressBook.from"] = "SIGNATURE_PROCESS";
            HttpContext.Current.Session["AddressBook.EnableOnly"] = "R";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAddressBook", "ajaxModalPopupAddressBook();", true);
        }

        protected void btnAddressBookPostback()
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                if (atList != null && atList.Count > 0)
                {
                    Corrispondente corr = null;
                    string idAmm = UIManager.UserManager.GetInfoUser().idAmministrazione;
                    foreach (NttDataWA.Popup.AddressBook.CorrespondentDetail addressBookCorrespondent in atList)
                    {

                        if (!addressBookCorrespondent.isRubricaComune)
                        {
                            corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(addressBookCorrespondent.SystemID);
                        }
                        else
                        {
                            corr = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(addressBookCorrespondent.CodiceRubrica);
                        }

                    }
                    Ruolo ruolo = RoleManager.GetRuolo(corr.systemId);
                    if (ruolo != null)
                    {
                        if (IsRuoloAbilitatoAlPassoSelezionato(ruolo) && !corr.disabledTrasm)
                        {
                            this.TxtCodeRole.Text = corr.codiceRubrica;
                            this.TxtDescriptionRole.Text = corr.descrizione;
                            this.idRuolo.Value = ruolo.idGruppo;
                            this.PassoDiFirmaSelected.ruoloCoinvolto = ruolo;
                            this.UpdPnlRole.Update();
                            this.LoadDllUtenteCoinvolto(UIManager.UserManager.getUserInRoleByIdGruppo(ruolo.idGruppo));
                        }
                        else
                        {
                            this.TxtCodeRole.Text = string.Empty;
                            this.TxtDescriptionRole.Text = string.Empty;
                            this.idRuolo.Value = string.Empty;
                            this.UpdPnlRole.Update();
                            string msg = "WarningRoleNotEnabledSign";
                            if (corr.disabledTrasm)
                                msg = "WarningRoleDisabledTrasm";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                    }
                }
                //Aggiorno i campi automatici se sono previsti
                this.ResetCampiAutomatciPassoSelezionato();

                HttpContext.Current.Session["AddressBook.At"] = null;
                HttpContext.Current.Session["AddressBook.Cc"] = null;
                HttpContext.Current.Session["AddressBook.type"] = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// Abilita il panel per la creazione di un nuovo passo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnAddStep_Click(object sender, EventArgs e)
        {
            try
            {
                this.ClearFieldsStep();
                this.CalcolaProssimoPasso(this.ProcessoDiFirmaSelected);
                this.PassoDiFirmaSelected = new PassoFirma();
                this.PassoDiFirmaSelected.Invalidated = '0';
                this.UpdateContentPage();
            }
            catch (Exception ex)
            {
                string msg = "ErrorSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        /// <summary>
        /// Effettua la rimozione del passo selezionato
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnDeleteStep_Click(object sender, EventArgs e)
        {
            //Se sto rimuovendo un passo di tipo cambio stato con stato selezionato, verifico che rimanga consistenza tra sta passo
            //cambio stato precedene e quello successivo qual'ora essi esistono. Non c'è consistenza blocco la cancellazione
            if (this.PassoDiFirmaSelected.Evento.CodiceAzione.Equals(Azione.DOC_CAMBIO_STATO.ToString())
                && !string.IsNullOrEmpty(this.PassoDiFirmaSelected.IdStatoDiagramma))
            {
                PassoFirma passoCambioStatoPrecedente = ProcessoDiFirmaSelected.passi.Where(p => p.numeroSequenza < PassoDiFirmaSelected.numeroSequenza && p.Evento.CodiceAzione.Equals(Azione.DOC_CAMBIO_STATO.ToString())).OrderByDescending(p => p.numeroSequenza).FirstOrDefault();
                PassoFirma passoCambioStatoSuccessivo = ProcessoDiFirmaSelected.passi.Where(p => p.numeroSequenza > PassoDiFirmaSelected.numeroSequenza && p.Evento.CodiceAzione.Equals(Azione.DOC_CAMBIO_STATO.ToString())).OrderBy(p => p.numeroSequenza).FirstOrDefault();
                if (passoCambioStatoPrecedente != null && !string.IsNullOrEmpty(passoCambioStatoPrecedente.IdStatoDiagramma)
                    && passoCambioStatoSuccessivo != null && !string.IsNullOrEmpty(passoCambioStatoSuccessivo.IdStatoDiagramma))
                {
                    //Verifico che lo stato selezionato nello stato precedente è padre di quello selezionato in quello successico
                    //Calcolo gli stati successi del passo che sto salvando e verifico che lo stato selezionato nel passo succesivo è tra questi
                    DiagrammaStato diagramma = DiagrammiManager.getDgByIdTipoDoc(passoCambioStatoSuccessivo.IdTipologia, UIManager.UserManager.GetInfoUser().idAmministrazione);
                    List<Stato> statiSuccessivi = new List<Stato>();
                    for (int i = 0; i < diagramma.PASSI.Length; i++)
                    {
                        DocsPaWR.Passo step = (DocsPaWR.Passo)diagramma.PASSI[i];
                        if (step.STATO_PADRE.SYSTEM_ID.ToString().Equals(passoCambioStatoPrecedente.IdStatoDiagramma))
                        {
                            for (int j = 0; j < step.SUCCESSIVI.Length; j++)
                            {
                                DocsPaWR.Stato st = (DocsPaWR.Stato)step.SUCCESSIVI[j];
                                if (DiagrammiManager.IsRuoloAssociatoStatoDia(diagramma.SYSTEM_ID.ToString(), this.PassoDiFirmaSelected.ruoloCoinvolto.idGruppo, st.SYSTEM_ID.ToString()))
                                {
                                    if (!st.STATO_SISTEMA)
                                        statiSuccessivi.Add(st);
                                }
                            }
                        }
                    }
                    bool existsInStatoSucc = false;
                    foreach (Stato s in statiSuccessivi)
                    {
                        if (s.SYSTEM_ID == Convert.ToInt32(passoCambioStatoSuccessivo.IdStatoDiagramma))
                        {
                            existsInStatoSucc = true;
                            break;
                        }
                    }
                    if (!existsInStatoSucc)
                    {
                        string msg = "WarningSignatureProcessesRimozioneStatoPassoSuccessivoNonConsistente";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                }
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmRemoveStepSignatureProcess', 'HiddenRemoveStepSignatureProcess', '','" + this.PassoDiFirmaSelected.numeroSequenza + "');", true);
            return;
        }

        /// <summary>
        /// Effettua la creazione/modifica del passo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnConfirmAddStep_Click(object sender, EventArgs e)
        {
            //Sto inserendo un nuvo passo
            string msg = string.Empty;
            try
            {
                msg = this.CheckFields();
                if (!string.IsNullOrEmpty(msg))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }
                else
                {                     
                    //Nel caso di passo di tipo evento, controllo che sia preceduto da un passo di tipo attesa, in caso contrario dò un messaggio di conferma
                    bool existWait = (from i in this.ProcessoDiFirmaSelected.passi
                                      where i.Evento.TipoEvento.Equals(LibroFirmaManager.TypeStep.WAIT)
                                      select i).FirstOrDefault() != null;
                    if (string.IsNullOrEmpty(this.PassoDiFirmaSelected.idPasso) && RblTypeStep.SelectedValue == LibroFirmaManager.TypeStep.EVENT && !existWait)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmEventWithoutWaitSignatureProcess', 'HiddenEventWithoutWait', '','" + this.PassoDiFirmaSelected.numeroSequenza + "');", true);
                        return;
                    }
                    this.AddStep();
                }
            }
            catch (Exception ex)
            {
                msg = "ErrorSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        private void AddStep()
        {
            string msg = string.Empty;
            int oldNumeroSequenza = this.PassoDiFirmaSelected.numeroSequenza;
            try
            {
                this.PopolaPassoDiFirma();
                if (string.IsNullOrEmpty(this.PassoDiFirmaSelected.idPasso))
                {
                    PassoFirma nuovoPasso = UIManager.SignatureProcessesManager.InserisciPassoDiFirma(this.PassoDiFirmaSelected);
                    if (nuovoPasso != null)
                    {
                        this.PassoDiFirmaSelected = nuovoPasso;
                        ProcessoFirma processo = UIManager.SignatureProcessesManager.GetProcessoDiFirma(nuovoPasso.idProcesso);
                        this.ProcessoDiFirmaSelected = processo;
                        //Aggiorno il Treeview
                        TreeNode parentNode = this.TreeSignatureProcess.SelectedNode;
                        this.UpdateNode(parentNode, processo);
                        //Aggiorno il processo in sessione
                        this.ListaProcessiDiFirma.Where(p => p.idProcesso.Equals(processo.idProcesso)).ToList().ForEach(f => f.passi = processo.passi);

                        this.PassoDiFirmaSelected = null;

                        this.CalcolaProssimoPasso(processo);
                    }
                    else
                    {
                        msg = "ErrorInsertStep";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                        return;
                    }
                }
                else //Sto aggiornando un passo esistente
                {
                    if (UIManager.SignatureProcessesManager.AggiornaPassoDiFirma(PassoDiFirmaSelected, oldNumeroSequenza))
                    {
                        ProcessoFirma processo = UIManager.SignatureProcessesManager.GetProcessoDiFirma(PassoDiFirmaSelected.idProcesso);
                        
                        //Aggiorno il treeview
                        TreeNode parentNode = this.TreeSignatureProcess.SelectedNode.Parent;
                        this.UpdateNode(parentNode, processo);
                        //Aggiorno il processo in sessione
                        this.ListaProcessiDiFirma.Where(p => p.idProcesso.Equals(processo.idProcesso)).ToList().ForEach(f => f.passi = processo.passi);                      
                        this.lblSectionDocument.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLblSectionDocument", UIManager.UserManager.GetUserLanguage()) + PassoDiFirmaSelected.numeroSequenza.ToString();
                        this.CalcolaProssimoPasso(processo);

                        msg = "ConfirmStepChange";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'check', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'check', '');}", true);
                    }
                    else
                    {
                        msg = "ErrorStepChange";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                        return;
                    }
                }
                this.UpdateContentPage();
            }
            catch (Exception ex)
            {
                msg = "ErrorSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }

        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.TxtCodeRole.Text))
                {
                    RubricaCallType calltype = RubricaCallType.CALLTYPE_PROTO_INT_MITT;
                    ElementoRubrica[] listaCorr = null;
                    Corrispondente corr = null;
                    UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
                    listaCorr = UIManager.AddressBookManager.getElementiRubricaMultipli(TxtCodeRole.Text, calltype, true);
                    if (listaCorr != null && (listaCorr.Count() == 1))
                    {
                        if (listaCorr.Count() == 1)
                        {
                            corr = UIManager.AddressBookManager.getCorrispondenteRubrica(this.TxtCodeRole.Text, calltype);
                        }
                        if (corr == null)
                        {
                            this.TxtCodeRole.Text = string.Empty;
                            this.TxtDescriptionRole.Text = string.Empty;
                            this.idRuolo.Value = string.Empty;
                            this.UpdPnlRole.Update();
                            string msg = "ErrorTransmissionCorrespondentNotFound";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        if (!corr.tipoCorrispondente.Equals("R"))
                        {
                            this.TxtCodeRole.Text = string.Empty;
                            this.TxtDescriptionRole.Text = string.Empty;
                            this.idRuolo.Value = string.Empty;
                            this.UpdPnlRole.Update();
                            string msg = "WarningCorrespondentAsRole";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        else
                        {
                            Ruolo ruolo = RoleManager.GetRuolo(corr.systemId);
                            if (IsRuoloAbilitatoAlPassoSelezionato(ruolo) && !corr.disabledTrasm)
                            {
                                this.TxtCodeRole.Text = corr.codiceRubrica;
                                this.TxtDescriptionRole.Text = corr.descrizione;
                                this.idRuolo.Value = ruolo.idGruppo;
                                this.PassoDiFirmaSelected.ruoloCoinvolto = ruolo;
                                this.UpdPnlRole.Update();
                                this.LoadDllUtenteCoinvolto(UIManager.UserManager.getUserInRoleByIdGruppo(ruolo.idGruppo));
                            }
                            else
                            {
                                this.TxtCodeRole.Text = string.Empty;
                                this.TxtDescriptionRole.Text = string.Empty;
                                this.idRuolo.Value = string.Empty;
                                this.UpdPnlRole.Update();
                                string msg = "WarningRoleNotEnabledSign";
                                if (corr.disabledTrasm)
                                    msg = "WarningRoleDisabledTrasm";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                        }
                    }
                    else
                    {
                        this.TxtCodeRole.Text = string.Empty;
                        this.TxtDescriptionRole.Text = string.Empty;
                        this.idRuolo.Value = string.Empty;
                        this.UpdPnlRole.Update();
                        string msg = "ErrorTransmissionCorrespondentNotFound";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                }
                else
                {
                    this.TxtCodeRole.Text = string.Empty;
                    this.TxtDescriptionRole.Text = string.Empty;
                    this.idRuolo.Value = string.Empty;
                    this.PassoDiFirmaSelected.ruoloCoinvolto = null;
                    this.LoadDllUtenteCoinvolto(null);
                    this.UpdPnlRole.Update();
                }
                //Aggiorno i campi automatici se sono previsti
                this.ResetCampiAutomatciPassoSelezionato();
            }
            catch (Exception ex)
            {
                string msg = "ErrorSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        protected void IndexImgRemoveFilter_Click(object sender, EventArgs e)
        {
            ListaProcessiDiFirma = SignatureProcessesManager.GetProcessiDiFirma();
            this.FiltersProcesses = null;
            this.ProcessoDiFirmaSelected = null;
            TreeviewProcesses_Bind();
            this.upPnlTreeSignatureProcess.Update();
            UpdateContentPage();

            this.IndexImgRemoveFilter.Enabled = false;
            this.UpPnlBAction.Update();
        }

        #endregion

        #region Treeview

        private void TreeviewProcesses_Bind()
        {
            this.TreeSignatureProcess.Nodes.Clear();
            TreeNode root = new TreeNode();
            root.Text = Utils.Languages.GetLabelFromCode("ManagementMonitoringProcessesTutti", UIManager.UserManager.GetUserLanguage());
            root.Value = "";
            this.TreeSignatureProcess.Nodes.Add(root);

            if (ListaProcessiDiFirma != null && ListaProcessiDiFirma.Count > 0)
            {
                foreach (ProcessoFirma p in ListaProcessiDiFirma)
                {
                    root.ChildNodes.Add(AddNode(p));
                }
                this.TreeSignatureProcess.DataBind();
            }

        }

        private TreeNode AddNode(ProcessoFirma p)
        {
            TreeNode root = new TreeNode();

            string infoModello = p.IsProcessModel ? Utils.Languages.GetLabelFromCode("ManagementSignatureProcessModel", UserManager.GetUserLanguage()) : string.Empty;
            root.Text = p.isInvalidated ? "<strike>" + p.nome + infoModello + "</strike>" : p.nome + " " + infoModello;
            root.Value = p.idProcesso;
            root.ToolTip = p.nome;
            foreach (PassoFirma passo in p.passi)
            {
                this.AddChildrenElements(passo, ref root);
            }
            if (ProcessoDiFirmaSelected != null && p.idProcesso.Equals(ProcessoDiFirmaSelected.idProcesso))
                root.Select();
            return root;
        }

        private TreeNode AddNodeToTop(ProcessoFirma p)
        {
            TreeNode root = new TreeNode();

            string infoModello = p.IsProcessModel ? Utils.Languages.GetLabelFromCode("ManagementSignatureProcessModel", UserManager.GetUserLanguage()) : string.Empty;
            root.Text = p.isInvalidated ? "<strike>" + p.nome + infoModello + "</strike>" : p.nome + " " + infoModello;
            root.Value = p.idProcesso;
            root.ToolTip = p.nome;
            foreach (PassoFirma passo in p.passi)
            {
                this.AddChildrenElements(passo, ref root);
            }

            this.TreeSignatureProcess.Nodes[0].ChildNodes.AddAt(0,root);
            return root;
        }

        private TreeNode AddChildrenElements(PassoFirma p, ref TreeNode root)
        {
            TreeNode nodeChild = new TreeNode();

            nodeChild.ImageUrl = LibroFirmaManager.GetIconEventType(p);
            nodeChild.Value = p.idPasso;
            nodeChild.Text = p.Invalidated != '0' ? "<strike>" + LibroFirmaManager.GetHolder(p) + "</strike>" : LibroFirmaManager.GetHolder(p);
            nodeChild.ToolTip = LibroFirmaManager.GetHolder(p);
            root.ChildNodes.Add(nodeChild);

            return nodeChild;
        }

        private void UpdateChildrenElement(PassoFirma p)
        {
            TreeNode nodeChild = this.TreeSignatureProcess.SelectedNode;

            nodeChild.ImageUrl = LibroFirmaManager.GetIconEventType(p);
            nodeChild.Value = p.idPasso;
            nodeChild.Text = LibroFirmaManager.GetHolder(p);
            nodeChild.ToolTip = LibroFirmaManager.GetHolder(p);

            this.upPnlTreeSignatureProcess.Update();
        }

        private void UpdateRootElement(ProcessoFirma p)
        {
            TreeNode root = this.TreeSignatureProcess.SelectedNode.Parent;

            string infoModello = p.IsProcessModel ? Utils.Languages.GetLabelFromCode("ManagementSignatureProcessModel", UserManager.GetUserLanguage()) : string.Empty;
            root.Text = p.isInvalidated ? "<strike>" + p.nome + infoModello + "</strike>" : p.nome + " " + infoModello;
            root.Value = p.idProcesso;
            root.ToolTip = p.nome;

            this.upPnlTreeSignatureProcess.Update();
        }

        /// <summary>
        /// Aggiorna nel treeview l'intero nodo, compreso nodi figli
        /// </summary>
        /// <param name="p"></param>
        private void UpdateNode(TreeNode node , ProcessoFirma p)
        {
            string infoModello = p.IsProcessModel ? Utils.Languages.GetLabelFromCode("ManagementSignatureProcessModel", UserManager.GetUserLanguage()) : string.Empty;
            node.Text = p.isInvalidated ? "<strike>" + p.nome + infoModello + "</strike>" : p.nome + " " + infoModello;
            node.Value = p.idProcesso;
            node.ToolTip = p.nome;
            node.ChildNodes.Clear();
            foreach (PassoFirma passo in p.passi)
            {
                this.AddChildrenElements(passo, ref node);
            }
            node.Select();
            this.upPnlTreeSignatureProcess.Update();
        }

        protected void TreeSignatureProcess_SelectedNodeChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            try
            {
                TreeNode node = this.TreeSignatureProcess.SelectedNode;
                if (!NessunNodoSelezionato())
                {
                    if (IsNodoProcesso())
                    {
                        this.ProcessoDiFirmaSelected = (from processo in this.ListaProcessiDiFirma where processo.idProcesso.Equals(node.Value) select processo).FirstOrDefault();
                        this.txtNameSignatureProcesses.Text = this.ProcessoDiFirmaSelected.nome;
                        this.PassoDiFirmaSelected = null;
                        this.CalcolaProssimoPasso(ProcessoDiFirmaSelected);
                    }
                    else
                    {
                        TreeNode nodeParent = node.Parent;
                        this.ClearFieldsStep();
                        ProcessoFirma processoAggiornato = SignatureProcessesManager.GetProcessoDiFirma(nodeParent.Value);
                        this.ListaProcessiDiFirma.Where(p => p.idProcesso.Equals(processoAggiornato.idProcesso)).ToList().ForEach(f => f.passi = processoAggiornato.passi);
                        this.ProcessoDiFirmaSelected = (from processo in this.ListaProcessiDiFirma where processo.idProcesso.Equals(nodeParent.Value) select processo).FirstOrDefault();

                        this.txtNameSignatureProcesses.Text = this.ProcessoDiFirmaSelected.nome;
                        this.PassoDiFirmaSelected = (from p in this.ProcessoDiFirmaSelected.passi where p.idPasso.Equals(node.Value) select p).FirstOrDefault();
                        this.PopolaCampiPasso(PassoDiFirmaSelected);
                    }
                    this.UpdateContentPage();
                }
                else //Ho selezionato il nodo radice tutti
                {
                    ClearFieldsProcess();
                    ClearFieldsStep();
                    this.ProcessoDiFirmaSelected = null;
                    UpdateContentPage();
                }
            }
            catch (Exception ex)
            {
                string msg = "ErrorSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        protected void TreeSignatureProcess_Collapsed(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
        }


        #endregion

        #region Utils

        /// <summary>
        /// Visualizzo il panel del tipo firma digitale solo se è selezionato tipo firma digitale
        /// </summary>
        private void setVisibilityTypeEvent()
        {
            if (this.rblTypeSignature.SelectedValue.Equals(DIGITALE))
            {
                this.plcTypeSignatureD.Visible = true;
                this.plcTypeSignatureE.Visible = false;
                this.cbx_automatico.Visible = false;
                this.cbx_automatico.Checked = false;
                this.rblTypeSignatureD.SelectedIndex = 0;
            }
            else
            {
                this.plcTypeSignatureD.Visible = false;
                this.plcTypeSignatureE.Visible = true;
                this.rblTypeSignatureE.SelectedIndex = 0;
            }
            this.UpdPnlTypeSignature.Update();
        }

        /// <summary>
        /// Carica gli eventi legati al libro firma
        /// </summary>
        private void LoadEventNotification()
        {
            List<AnagraficaEventi> listEventTypes = SignatureProcessesManager.GetEventNotification();
            if (listEventTypes != null && listEventTypes.Count > 0)
            {
                List<ListItem> listItem = new List<ListItem>();
                foreach (AnagraficaEventi evento in listEventTypes)
                {
                    listItem.Add(new ListItem()
                        {
                            Text = Utils.Languages.GetLabelFromCode(evento.gruppo, UserManager.GetUserLanguage()),
                            Value = evento.gruppo
                        });
                }

                this.cbxOptionNotify.Items.Clear();
                this.cbxOptionNotify.Items.AddRange(listItem.ToArray());
            }
        }

        private void LoadEventTypes()
        {
            this.DdlTypeEvent.Items.Clear();
            List<AnagraficaEventi> listEventTypes = SignatureProcessesManager.GetEventTypes(EVENT);
            if (listEventTypes != null && listEventTypes.Count > 0)
            {
                ListItem empty = new ListItem("", "");
                this.DdlTypeEvent.Items.Add(empty);
                this.DdlTypeEvent.SelectedIndex = -1;

                List<ListItem> listItem = new List<ListItem>();
                foreach (AnagraficaEventi evento in listEventTypes)
                {
                    listItem.Add(new ListItem()
                    {
                        Text = evento.descrizione,
                        Value = evento.codiceAzione
                    });
                }
                this.DdlTypeEvent.Items.AddRange(listItem.ToArray());
            }
        }



        /// <summary>
        /// Imposta la visibilità dei bottoni
        /// </summary>
        private void SetButtons()
        {
            this.SignatureProcessesBtnSave.Enabled = false;
            this.SignatureProcessesBtnDuplica.Enabled = false;
            this.SignatureProcessesStatistics.Enabled = false;
            this.SignatureProcessesBtnRemove.Enabled = false;
            //this.SignatureProcessesBtnVisibility.Enabled = false;
            this.SignatureProcessesBtnNew.Enabled = true;

            //I bottoni salva ed elimina e duplica sono applicabili solo per il processo, quindi sono abilitati solo se 
            //nel treeview è selezionato un processo
            TreeNode selectedNode = this.TreeSignatureProcess.SelectedNode;

            //Non ho selezionato alcun nodo
            if (NessunNodoSelezionato() && this.ProcessoDiFirmaSelected != null && string.IsNullOrEmpty(this.ProcessoDiFirmaSelected.idProcesso))
            {
                this.SignatureProcessesBtnNew.Enabled = false;
                this.SignatureProcessesBtnSave.Enabled = true;
            }

            //Ho selezionato un nodo di tipo PASSO
            if (IsNodoPasso()) 
            {
                this.BtnConfirmAddStep.ToolTip = Utils.Languages.GetLabelFromCode("SignatureProcessesBtnSaveStepToolTip", UIManager.UserManager.GetUserLanguage());
                this.BtnDeleteStep.Visible = true;
                this.PnlBtnAddStep.Visible = false;
            }

            //Ho selezionato un Nodo di tipo processo
            if (IsNodoProcesso()) 
            {
                //Se stiamo creando un passo di firma
                if (this.PassoDiFirmaSelected != null && string.IsNullOrEmpty(this.PassoDiFirmaSelected.idPasso))
                {
                    this.btnAddStep.Enabled = false;
                    this.BtnConfirmAddStep.ToolTip = Utils.Languages.GetLabelFromCode("SignatureProcessesBtnConfirmAddStepToolTip", UIManager.UserManager.GetUserLanguage());
                    this.BtnDeleteStep.Visible = false;
                }
                else
                {
                    this.btnAddStep.Enabled = true;
                    this.PnlBtnAddStep.Visible = true;
                    this.SignatureProcessesBtnSave.Enabled = true;
                    this.SignatureProcessesBtnDuplica.Enabled = true;
                    this.SignatureProcessesStatistics.Enabled = true;
                    this.SignatureProcessesBtnRemove.Enabled = true;

                    /*
                    this.SignatureProcessesBtnVisibility.Enabled = true;
                    if (this.ProcessoDiFirmaSelected.passi == null || this.ProcessoDiFirmaSelected.passi.Length < 1)
                    {
                        this.SignatureProcessesBtnVisibility.Enabled = false;
                    }
                    */
                }
            }
            this.UplAddStep.Update();
            this.UpPnlButtons.Update();
        }

        private void ClearFieldsStep()
        {
            this.PnlWarningRoleUserDisabled.Visible = false;
            this.txtNr.Text = string.Empty;
            this.idRuolo.Value = string.Empty;
            this.TxtCodeRole.Text = string.Empty;
            this.TxtDescriptionRole.Text = string.Empty;
            this.ddlUtenteCoinvolto.Items.Clear();
            this.ddlUtenteCoinvolto.Enabled = false;

            this.RblTypeStep.SelectedValue = SIGN;
            this.RblTypeStep_SelectedIndexChanged(null, null);
            this.DdlTypeEvent.SelectedIndex = -1;

            //per passi automatici
            this.cbx_automatico.Visible = false;
            this.cbx_automatico.Checked = false;
            this.ResetCampiAutomatici();

            this.cbxFacoltativo.Checked = false;

            this.rblTypeSignature.SelectedValue = DIGITALE;
            this.rblTypeSignatureD.SelectedIndex = 0;
            this.plcTypeSignatureD.Visible = true;
            this.plcTypeSignatureE.Visible = false;

            this.RblRoleOrTypeRole.SelectedIndex = 0;

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_TIPO_RUOLO_LIBRO_FIRMA.ToString()))
                && Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_TIPO_RUOLO_LIBRO_FIRMA.ToString()) != "0")
                this.DdlTypeRole.SelectedIndex = -1;

            this.PnlTypeRole.Attributes.Add("style", "display:none");
            this.PnlRole.Attributes.Add("style", "display:block");
            this.PnlUenteCoinvolto.Attributes.Add("style", "display:block");

            this.txtNotes.Text = string.Empty;

            this.PassoDiFirmaSelected = null;
            this.SetOpzioniNotifiche();
            this.cbxOptionNotify.ClearSelection();
            this.UpdPnlDetailsStep.Update();
        }

        private void ClearFieldsProcess()
        {
            this.ProcessoDiFirmaSelected = new ProcessoFirma();

            this.txtNameSignatureProcesses.Text = string.Empty;
            this.UpdPlnProcessName.Update();
        }

        /// <summary>
        /// Aggiorna l'intero contenuto della pagina
        /// </summary>
        private void UpdateContentPage()
        {
            this.pnlStep.Visible = true;
            this.PnlStatoDiagrammaInterruzione.Visible = false;
            if (NessunNodoSelezionato())//Se non c'è alcun nodo selezionato o ho selezionato il nodo radice non vedo nulla
            {
                this.pnlStep.Visible = false;
                this.pnlProcessName.Visible = false;
                if (this.ProcessoDiFirmaSelected != null)
                {
                    this.pnlProcessName.Visible = true;

                    //Se presente un passo di tipo cambio stato popolo la combo per selezionare uno stato per interruzione
                    LoadSatiDiagrammaInterruzione(this.ProcessoDiFirmaSelected);
                }
            }
            else if (IsNodoPasso()) //Se è selezionato il passo non vedo il nome del processo
            {
                this.pnlDetailsSteps.Visible = true;
                this.pnlProcessName.Visible = false;
            }
            else if (IsNodoProcesso()) //Selezionato un processo
            {
                this.pnlStep.Visible = true;
                this.pnlProcessName.Visible = true;

                if (this.PassoDiFirmaSelected != null && string.IsNullOrEmpty(this.PassoDiFirmaSelected.idPasso)) //Ho selezionato un processo e stò creando un nuovo passo
                {
                    this.pnlDetailsSteps.Visible = true;
                }
                else
                {
                    this.pnlProcessName.Visible = true;
                    this.pnlDetailsSteps.Visible = false;
                    
                    //Se presente un passo di tipo cambio stato popolo la combo per selezionare uno stato per interruzione
                    LoadSatiDiagrammaInterruzione(this.ProcessoDiFirmaSelected);
                }
            }
            this.SetButtons();
            this.UpPnlStatoDiagrammaInterruzione.Update();
            this.UpdPnlDetailsStep.Update();
            this.UplWarningRoleUserDisabled.Update();
            this.UpdPnlStep.Update();
            this.UplAddStep.Update();
            this.UpdPlnProcessName.Update();
        }

        private void LoadSatiDiagrammaInterruzione(ProcessoFirma processo)
        {
            this.DdlStatoDiagrammaInterruzione.Items.Clear();
            if(processo != null && processo.passi != null && processo.passi.Count() > 0)
            {
                //Verifico la presenta di un passo di tipo cambio stato con tipologia selezioanat
                PassoFirma passo = (from p in processo.passi
                                    where p.Evento.CodiceAzione.Equals(Azione.DOC_CAMBIO_STATO.ToString()) && !string.IsNullOrEmpty(p.IdTipologia)
                                    select p).FirstOrDefault();
                if(passo != null)
                {
                    ListItem empty = new ListItem("", "");
                    this.DdlStatoDiagrammaInterruzione.Items.Add(empty);
                    this.DdlStatoDiagrammaInterruzione.SelectedIndex = -1;
                    DiagrammaStato diagramma = DiagrammiManager.getDgByIdTipoDoc(passo.IdTipologia, UIManager.UserManager.GetInfoUser().idAmministrazione);
                    if (diagramma != null)
                    {
                        if (diagramma.STATI != null && diagramma.STATI.Count() > 0)
                        {
                            foreach (Stato s in diagramma.STATI)
                            {
                                ListItem item = new ListItem(s.DESCRIZIONE, s.SYSTEM_ID.ToString());
                                this.DdlStatoDiagrammaInterruzione.Items.Add(item);
                            }
                            if (!string.IsNullOrEmpty(processo.IdStatoInterruzione))
                                this.DdlStatoDiagrammaInterruzione.SelectedValue = processo.IdStatoInterruzione;
                        }
                    }
                    this.PnlStatoDiagrammaInterruzione.Visible = true;
                }
            }
            this.UpPnlStatoDiagrammaInterruzione.Update();
        }

        /// <summary>
        /// Ho selezionato il nodi radice 'TUTTI' oppure non ho selezionato nulla
        /// </summary>
        /// <returns></returns>
        private bool NessunNodoSelezionato()
        {
            TreeNode nodoSelezionato = this.TreeSignatureProcess.SelectedNode;
            return nodoSelezionato == null || string.IsNullOrEmpty(nodoSelezionato.Value);
        }

        /// <summary>
        /// Ho selezionato un nodo di tipo processo
        /// </summary>
        /// <returns></returns>
        private bool IsNodoProcesso()
        {
            TreeNode nodoSelezionato = this.TreeSignatureProcess.SelectedNode;
            return nodoSelezionato != null && nodoSelezionato.Parent != null && string.IsNullOrEmpty(nodoSelezionato.Parent.Value);
        }

        /// <summary>
        /// Ho selezionato un nodo di tipo passo
        /// </summary>
        /// <returns></returns>
        private bool IsNodoPasso()
        {
            TreeNode nodoSelezionato = this.TreeSignatureProcess.SelectedNode;
            return nodoSelezionato!= null && nodoSelezionato.Parent != null && !string.IsNullOrEmpty(nodoSelezionato.Parent.Value);
        }

        /// <summary>
        /// Estrazione dei processi di firma creati dall'utente
        /// </summary>
        private void LoadProcessiDiFirma()
        {
            ListaProcessiDiFirma = SignatureProcessesManager.GetProcessiDiFirma();
        }

        private void LoadDdlTypeRole()
        {
            List<DocsPaWR.TipoRuolo> listTypeRole = LibroFirmaManager.GetTypeRole();
            if (listTypeRole != null && listTypeRole.Count > 0)
            {
                ListItem empty = new ListItem("", "");
                this.DdlTypeRole.Items.Add(empty);
                this.DdlTypeRole.SelectedIndex = -1;

                for (int i = 0; i < listTypeRole.Count; i++)
                {
                    ListItem item = new ListItem(listTypeRole[i].descrizione, listTypeRole[i].systemId);
                    this.DdlTypeRole.Items.Add(item);
                }

                this.DdlTypeRole.Enabled = true;
            }
        }

        private void LoadDllUtenteCoinvolto(List<Utente> listUserInRole)
        {
            this.ddlUtenteCoinvolto.Items.Clear();

            if (listUserInRole != null && listUserInRole.Count > 0)
            {
                ListItem empty = new ListItem("", "");
                this.ddlUtenteCoinvolto.Items.Add(empty);
                this.ddlUtenteCoinvolto.SelectedIndex = -1;

                for (int i = 0; i < listUserInRole.Count; i++)
                {
                    ListItem item = new ListItem(listUserInRole[i].descrizione, listUserInRole[i].systemId);
                    this.ddlUtenteCoinvolto.Items.Add(item);
                }

                this.ddlUtenteCoinvolto.Enabled = true;
            }
            else
            {
                this.ddlUtenteCoinvolto.Enabled = false;
            }

            this.UpdPnlUtenteCoinvolto.Update();

        }

        private string CheckFields()
        {
            string errorMessage = string.Empty;
            bool isAuthorizedLibroFirma = true;
            int numPassi = string.IsNullOrEmpty(this.PassoDiFirmaSelected.idPasso) ? this.ProcessoDiFirmaSelected.passi.Length + 1 : this.ProcessoDiFirmaSelected.passi.Length;
            if (Convert.ToInt32(this.txtNr.Text) > numPassi || Convert.ToInt32(this.txtNr.Text) < 1)
            {
                errorMessage = "WarningSequenceNumberNoValid";
                return errorMessage;
            }
            //Se il ruolo non è abilitato alla creazione di modelli, deve inserire tutti i campi obbligatori
            if (RblTypeStep.SelectedValue != LibroFirmaManager.TypeStep.WAIT && !IsRoleCreateModelProcessEnabled())
            {
                errorMessage = ControlloCampiObbligatori();
                if (!string.IsNullOrEmpty(errorMessage))
                    return errorMessage;
            }

            string codEvento = string.Empty;
            bool ignoraOrdinePasso = false;
            if (this.RblTypeStep.SelectedValue.Equals(SIGN))
                codEvento = rblTypeSignature.SelectedValue.Equals(DIGITALE) ?  this.rblTypeSignatureD.SelectedValue : codEvento = this.rblTypeSignatureE.SelectedValue;
            else
                codEvento = this.DdlTypeEvent.SelectedValue;
            if (!string.IsNullOrEmpty(codEvento))
                ignoraOrdinePasso = SignatureProcessesManager.GetAnagraficaEventoByCodice(codEvento).IgnoraOrdine;

            switch (RblTypeStep.SelectedValue)
            {
                case LibroFirmaManager.TypeStep.SIGN:
                    if (this.RblRoleOrTypeRole.SelectedValue.Equals(RUOLO))
                    {                        
                        if (this.PassoDiFirmaSelected.ruoloCoinvolto != null && !string.IsNullOrEmpty(this.PassoDiFirmaSelected.ruoloCoinvolto.idGruppo))
                        {
                            if (this.PassoDiFirmaSelected.ruoloCoinvolto.disabledTrasm)
                            {
                                errorMessage = "WarningRoleDisabledTrasm";
                                return errorMessage;
                            }
                            isAuthorizedLibroFirma = UserManager.IsRoleAuthorizedFunctions("DO_LIBRO_FIRMA", this.PassoDiFirmaSelected.ruoloCoinvolto);
                            if (!isAuthorizedLibroFirma)
                            {
                                errorMessage = "WarningNotAuthorizedLibroFirma";
                                return errorMessage;
                            }
                        }
                    }
                    if (!ignoraOrdinePasso)
                    {
                        if (string.IsNullOrEmpty(this.PassoDiFirmaSelected.idPasso))
                        {
                            //Un passo di tipo firma non può essere inserito dopo un passo di tipo evento o di tipo attesa
                            bool canInsertStepSign = (from i in this.ProcessoDiFirmaSelected.passi
                                                      where i.numeroSequenza < Convert.ToInt32(this.txtNr.Text) && ((i.Evento.TipoEvento.Equals(LibroFirmaManager.TypeStep.EVENT) && !SignatureProcessesManager.GetAnagraficaEventoByCodice(i.Evento.CodiceAzione).IgnoraOrdine)
                                                      || (i.Evento.TipoEvento.Equals(LibroFirmaManager.TypeStep.WAIT)))
                                                      select i).FirstOrDefault() == null;
                            if (!canInsertStepSign)
                            {
                                errorMessage = "WarningNotStepSignAfterWaitOrEvent";
                                return errorMessage;
                            }

                            //Una passo di firma pades non può essere preceduto da un passo di firma CADES
                            if (rblTypeSignature.SelectedValue.Equals(DIGITALE) && rblTypeSignatureD.SelectedValue.Equals(LibroFirmaManager.TypeEvent.SIGN_PADES))
                            {
                                canInsertStepSign = (from i in this.ProcessoDiFirmaSelected.passi
                                                     where i.numeroSequenza < Convert.ToInt32(this.txtNr.Text) && i.Evento.CodiceAzione.Equals(LibroFirmaManager.TypeEvent.SIGN_CADES)
                                                     select i).FirstOrDefault() == null;
                                if (!canInsertStepSign)
                                {
                                    errorMessage = "WarningNotStepSignAfterCades";
                                    return errorMessage;
                                }
                            }
                        }
                        else
                        {
                            //Un passo di tipo firma non può essere inserito dopo un passo di tipo evento o di tipo attesa
                            bool canInsertStepSign = (from i in this.ProcessoDiFirmaSelected.passi
                                                      where i.numeroSequenza <= Convert.ToInt32(this.txtNr.Text) && i.numeroSequenza != this.PassoDiFirmaSelected.numeroSequenza
                                                         && ((i.Evento.TipoEvento.Equals(LibroFirmaManager.TypeStep.EVENT) && !SignatureProcessesManager.GetAnagraficaEventoByCodice(i.Evento.CodiceAzione).IgnoraOrdine) || (i.Evento.TipoEvento.Equals(LibroFirmaManager.TypeStep.WAIT)))
                                                      select i).FirstOrDefault() == null;
                            if (!canInsertStepSign)
                            {
                                errorMessage = "WarningNotStepSignAfterWaitOrEvent";
                                return errorMessage;
                            }

                            //Una passo di firma pades non può essere preceduto da un passo di firma CADES
                            if (rblTypeSignature.SelectedValue.Equals(DIGITALE) && rblTypeSignatureD.SelectedValue.Equals(LibroFirmaManager.TypeEvent.SIGN_PADES))
                            {
                                canInsertStepSign = (from i in this.ProcessoDiFirmaSelected.passi
                                                     where i.numeroSequenza <= Convert.ToInt32(this.txtNr.Text) && i.numeroSequenza != this.PassoDiFirmaSelected.numeroSequenza
                                                     && i.Evento.CodiceAzione.Equals(LibroFirmaManager.TypeEvent.SIGN_CADES)
                                                     select i).FirstOrDefault() == null;
                                if (!canInsertStepSign)
                                {
                                    errorMessage = "WarningNotStepSignAfterCades";
                                    return errorMessage;
                                }
                            }
                        }
                    }
                    if (rblTypeSignature.SelectedValue.Equals(DIGITALE))
                    {
                        Azione codiceEventoFirma = (Azione)Enum.Parse(typeof(Azione), rblTypeSignatureD.SelectedValue, true);
                        if (!CheckConsistenzaPassiCambioStato(codiceEventoFirma, out errorMessage))
                            return errorMessage;
                    }
                    break;
                case LibroFirmaManager.TypeStep.EVENT:
                    if (DdlTypeEvent.SelectedIndex == -1)
                    {
                        errorMessage = "WarningRequiredEventType";
                        return errorMessage;
                    }
                    if (!ignoraOrdinePasso)
                    {
                        //Un passo di tipo EVENTO non può essere inserito prima di un passo di attesa o di un passo di firma
                        bool canInsertStepEvent = (from i in this.ProcessoDiFirmaSelected.passi
                                                   where i.numeroSequenza >= Convert.ToInt32(this.txtNr.Text)
                                                      && (string.IsNullOrEmpty(this.PassoDiFirmaSelected.idPasso) || i.numeroSequenza != this.PassoDiFirmaSelected.numeroSequenza)
                                                      && ((i.Evento.TipoEvento.Equals(LibroFirmaManager.TypeStep.SIGN) && !SignatureProcessesManager.GetAnagraficaEventoByCodice(i.Evento.CodiceAzione).IgnoraOrdine) || (i.Evento.TipoEvento.Equals(LibroFirmaManager.TypeStep.WAIT)))
                                                   select i).FirstOrDefault() == null;

                        if (!canInsertStepEvent)
                        {
                            errorMessage = "WarningNotStepEvent";
                            return errorMessage;
                        }
                    }
                    Azione codiceEvento = (Azione)Enum.Parse(typeof(Azione), this.DdlTypeEvent.SelectedValue, true);
                    switch (codiceEvento)
                    {
                        case Azione.RECORD_PREDISPOSED:
                        case Azione.DOCUMENTO_REPERTORIATO:
                            //Un passo di protocollazione non può essere creato dopo un passo di spedizione
                            if (codiceEvento.Equals(Azione.RECORD_PREDISPOSED))
                            {
                                bool canInsertRecorPredisposed = (from i in this.ProcessoDiFirmaSelected.passi
                                                                    where i.Evento.CodiceAzione.Equals(Azione.DOCUMENTOSPEDISCI.ToString()) &&
                                                                    (i.numeroSequenza < Convert.ToInt32(this.txtNr.Text) ||
                                                                    (i.numeroSequenza == Convert.ToInt32(this.txtNr.Text) && !string.IsNullOrEmpty(this.PassoDiFirmaSelected.idPasso))) 
                                                                    select i).FirstOrDefault() == null;
                                if (!canInsertRecorPredisposed)
                                {
                                    errorMessage = "WarningPassoProtocolloDopoSpedizione";
                                    return errorMessage;
                                }

                                //Se esiste un passo di spedizione, il registro di AOO deve coincidere con quello selezionato per la spedizione
                                if (!string.IsNullOrEmpty(this.PassoDiFirmaSelected.IdAOO))
                                {
                                    PassoFirma passoSpedizione = (from p in this.ProcessoDiFirmaSelected.passi
                                                                       where p.Evento.CodiceAzione.Equals(Azione.DOCUMENTOSPEDISCI.ToString())
                                                                       select p).FirstOrDefault();
                                    if (passoSpedizione != null && !string.IsNullOrEmpty(passoSpedizione.IdAOO) && !passoSpedizione.IdAOO.Equals(PassoDiFirmaSelected.IdAOO))
                                    {
                                        errorMessage = "WarningSignatureProcessesRegistroErrato";
                                        return errorMessage;
                                    }
                                }
                            }
                            if (!CheckConsistenzaPassiCambioStato(codiceEvento, out errorMessage))
                                return errorMessage;
                            break;
                        case Azione.DOCUMENTOSPEDISCI:
                            //Un passo di spedizione non puo essere inserito prima di un passo di protocollazione.
                            bool canInsertSpedisci = (from i in this.ProcessoDiFirmaSelected.passi
                                                        where i.Evento.CodiceAzione.Equals(Azione.RECORD_PREDISPOSED.ToString()) && 
                                                        i.numeroSequenza >= Convert.ToInt32(this.txtNr.Text)                            
                                                       select i).FirstOrDefault() == null;
                            if (!canInsertSpedisci)
                            {
                                errorMessage = "WarningPassoProtocolloDopoSpedizione";
                                return errorMessage;
                            }

                            //Se esiste un passo di protocollazione, il registro di AOO deve coincidere con quello selezionato per la protocollazione
                            if (!string.IsNullOrEmpty(this.PassoDiFirmaSelected.IdAOO))
                            {
                                PassoFirma passoProtocollazione = (from p in this.ProcessoDiFirmaSelected.passi
                                                                   where p.Evento.CodiceAzione.Equals(Azione.RECORD_PREDISPOSED.ToString())
                                                                   select p).FirstOrDefault();
                                if (passoProtocollazione != null && !string.IsNullOrEmpty(passoProtocollazione.IdAOO) && !passoProtocollazione.IdAOO.Equals(PassoDiFirmaSelected.IdAOO))
                                {
                                    errorMessage = "WarningSignatureProcessesRegistroErrato";
                                    return errorMessage;
                                }
                            }
                            if (!CheckConsistenzaPassiCambioStato(codiceEvento, out errorMessage))
                                return errorMessage;
                            break;
                        case Azione.DOC_CAMBIO_STATO:
                            if (!CheckConsistenzaPassiCambioStato(codiceEvento, out errorMessage))
                                return errorMessage;
                            break;
                    }
                    break;
                case LibroFirmaManager.TypeStep.WAIT:

                    if (string.IsNullOrEmpty(this.PassoDiFirmaSelected.idPasso))
                    {
                        //Verifico, in caso di inserimento, se è già presente un passo di WAIT
                        bool existStepWait = (from i in this.ProcessoDiFirmaSelected.passi
                                              where i.Evento.TipoEvento.Equals(LibroFirmaManager.TypeStep.WAIT)
                                              select i).FirstOrDefault() != null;
                        if (existStepWait)
                        {
                            errorMessage = "WarningExistsStepWait";
                            return errorMessage;
                        }

                        //Verifico che il passo di WAIT non venga inserito prima di un passo di FIRMA( >=) o dopo un passo di EVENTO(<)
                        bool canInsertStepWait = (from i in this.ProcessoDiFirmaSelected.passi
                                                  where (i.numeroSequenza >= Convert.ToInt32(this.txtNr.Text) && i.Evento.TipoEvento.Equals(LibroFirmaManager.TypeStep.SIGN))
                                                    || (i.numeroSequenza < Convert.ToInt32(this.txtNr.Text) && i.Evento.TipoEvento.Equals(LibroFirmaManager.TypeStep.EVENT))
                                                  select i).FirstOrDefault() == null;
                        if (!canInsertStepWait)
                        {
                            errorMessage = "WarningInsertStepWait";
                            return errorMessage;
                        }
                    }//stò modificando il passo
                    else
                    {
                        //Verifico se è già presente un passo di WAIT
                        bool existStepWait = (from i in this.ProcessoDiFirmaSelected.passi
                                              where i.Evento.TipoEvento.Equals(LibroFirmaManager.TypeStep.WAIT) && i.numeroSequenza != this.PassoDiFirmaSelected.numeroSequenza
                                              select i).FirstOrDefault() != null;
                        if (existStepWait)
                        {
                            errorMessage = "WarningExistsStepWait";
                            return errorMessage;
                        }

                        //Verifico che il passo di WAIT non venga inserito prima di un passo di FIRMA( >=) o dopo un passo di EVENTO(<=)
                        bool canModifyStepWait = (from i in this.ProcessoDiFirmaSelected.passi
                                                  where i.numeroSequenza != this.PassoDiFirmaSelected.numeroSequenza &&
                                                      ((i.numeroSequenza >= Convert.ToInt32(this.txtNr.Text) && i.Evento.TipoEvento.Equals(LibroFirmaManager.TypeStep.SIGN))
                                                    || (i.numeroSequenza <= Convert.ToInt32(this.txtNr.Text) && i.Evento.TipoEvento.Equals(LibroFirmaManager.TypeStep.EVENT)))
                                                  select i).FirstOrDefault() == null;
                        if (!canModifyStepWait)
                        {
                            errorMessage = "WarningInsertStepWait";
                            return errorMessage;
                        }
                    }

                    break;
            }

            return errorMessage;
        }

        private bool CheckConsistenzaPassiCambioStato(Azione codiceEvento, out string error)
        {
            bool result = true;
            error = string.Empty;
            int numeroSequenza = Convert.ToInt32(this.txtNr.Text);
            switch (codiceEvento)
            {
                case Azione.DOC_CAMBIO_STATO:
                    if(this.cbx_automatico.Checked)
                    {
                        string idTipologiaSelezionata = this.DdlTipologiaDocumento.SelectedValue;
                        string idStatoDiagrammaSelezionato = this.DdlStatoDiagramma.SelectedValue;

                        if (!string.IsNullOrEmpty(idTipologiaSelezionata))
                        {
                            #region VERIFICA STESSA TIPOLOGIA
                            //Verifico se esiste un altro passo di tipo cambio stato con tipologia selezionata differente
                            bool existsPassoConTipologiaDiversa = (from p in ProcessoDiFirmaSelected.passi
                                                                   where p.Evento.CodiceAzione.Equals(Azione.DOC_CAMBIO_STATO.ToString())
                                                                   && !string.IsNullOrEmpty(p.IdTipologia) && !p.IdTipologia.Equals(idTipologiaSelezionata)
                                                                   select p).FirstOrDefault() != null;
                            if (existsPassoConTipologiaDiversa)
                            {
                                error = "WarningSignatureProcessesTipoDocErrata";
                                return false;
                            }
                            #endregion

                            #region VERIFICA PASSO STATO FINALE PRIMA DI PASSI CON MODIFICA AL DOC
                            //Uno stato finale non può essere seguito da un passo di qualsiasi tipo diverso da firma elettronica(es. protocollazione, firma digitale, spedizione...)
                            if (!string.IsNullOrEmpty(idStatoDiagrammaSelezionato) && !this.cbxFacoltativo.Checked)
                            {
                                Stato stato = UIManager.DiagrammiManager.GetStatoById(idStatoDiagrammaSelezionato);
                                if (stato.STATO_FINALE)
                                {
                                    bool existsPassoDiModifica = (from p in ProcessoDiFirmaSelected.passi
                                                                  where p.numeroSequenza >= numeroSequenza &&
                                                                  !p.Evento.CodiceAzione.Equals(Azione.DOC_VERIFIED.ToString()) &&
                                                                  !p.Evento.CodiceAzione.Equals(Azione.DOC_STEP_OVER.ToString()) &&
                                                                  !p.Evento.CodiceAzione.Equals(Azione.DOC_CAMBIO_STATO.ToString()) && !p.IsFacoltativo
                                                                  select p).FirstOrDefault() != null;
                                    if (existsPassoDiModifica)
                                    {
                                        error = "WarningSignatureProcessesStatoFinale";
                                        return false;
                                    }
                                }
                            }
                            #endregion

                            #region VERIFICA STATO CAMBIO PASSO SUCCESSIVO
                            if (!string.IsNullOrEmpty(idStatoDiagrammaSelezionato) && !this.cbxFacoltativo.Checked)
                            {
                                //Verifico se lo stato selezionato è uno stato padre per un passo di cambio stato successivo già salvato non facoltativo(questo può accadere quando sto salvando un passo già creato)
                                //In caso contrario, l'utente dovrà rimuovere lo stato del passo successivo e quindi tornarea modificare il passo.
                                PassoFirma passoSuccCS = ProcessoDiFirmaSelected.passi.Where(p => (p.numeroSequenza > numeroSequenza || (p.numeroSequenza == numeroSequenza && string.IsNullOrEmpty(PassoDiFirmaSelected.idPasso))) && !p.idPasso.Equals(this.PassoDiFirmaSelected.idPasso) && p.Evento.CodiceAzione.Equals(Azione.DOC_CAMBIO_STATO.ToString())).OrderBy(p => p.numeroSequenza).FirstOrDefault();

                                if (passoSuccCS != null && !string.IsNullOrEmpty(passoSuccCS.IdStatoDiagramma) && !passoSuccCS.IsFacoltativo)
                                {
                                    //Calcolo gli stati successi del passo che sto salvando e verifico che lo stato selezionato nel passo succesivo è tra questi
                                    DiagrammaStato diagramma = DiagrammiManager.getDgByIdTipoDoc(idTipologiaSelezionata, UIManager.UserManager.GetInfoUser().idAmministrazione);
                                    List<Stato> statiSuccessivi = new List<Stato>();
                                    for (int i = 0; i < diagramma.PASSI.Length; i++)
                                    {
                                        DocsPaWR.Passo step = (DocsPaWR.Passo)diagramma.PASSI[i];
                                        if (step.STATO_PADRE.SYSTEM_ID.ToString().Equals(idStatoDiagrammaSelezionato))
                                        {
                                            for (int j = 0; j < step.SUCCESSIVI.Length; j++)
                                            {
                                                DocsPaWR.Stato st = (DocsPaWR.Stato)step.SUCCESSIVI[j];
                                                if (DiagrammiManager.IsRuoloAssociatoStatoDia(diagramma.SYSTEM_ID.ToString(), this.PassoDiFirmaSelected.ruoloCoinvolto.idGruppo, st.SYSTEM_ID.ToString()))
                                                {
                                                    if (!st.STATO_SISTEMA)
                                                        statiSuccessivi.Add(st);
                                                }
                                            }
                                        }
                                    }
                                    bool existsInStatoSucc = false;
                                    foreach (Stato s in statiSuccessivi)
                                    {
                                        if (s.SYSTEM_ID == Convert.ToInt32(passoSuccCS.IdStatoDiagramma))
                                        {
                                            existsInStatoSucc = true;
                                            break;
                                        }
                                    }
                                    if (!existsInStatoSucc)
                                    {
                                        error = "WarningSignatureProcessesStatoPassoSuccessivoNonConsistente";
                                        return false;
                                    }
                                }
                            }
                            #endregion

                            #region VERIFICO CHE NON è PRESENTE PASSO PRECEDENTE CON CAMBIO STATO MANUALE 

                            PassoFirma passoPrecCS = ProcessoDiFirmaSelected.passi.Where(p => p.numeroSequenza < numeroSequenza && p.Evento.CodiceAzione.Equals(Azione.DOC_CAMBIO_STATO.ToString())).OrderByDescending(p => p.numeroSequenza).FirstOrDefault();
                            if (passoPrecCS != null && !passoPrecCS.IsAutomatico)
                            {
                                //Non può esserci un passo automatico di tipo CS dopo un passo manuale di tipo CS
                                error = "WarningSignatureProcessesPassoAutoDopoPassoManualeCS";
                                return false;
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        #region VERIFICA PASSO CAMBIO STATO MANUALE NO PRIMA DI PASSO CAMBIO STATO AUTOMATICO
                        PassoFirma passoSuccCS = ProcessoDiFirmaSelected.passi.Where(p => p.numeroSequenza > numeroSequenza && p.Evento.CodiceAzione.Equals(Azione.DOC_CAMBIO_STATO.ToString())).OrderBy(p => p.numeroSequenza).FirstOrDefault();
                        if (passoSuccCS != null && passoSuccCS.IsAutomatico)
                        {
                            //Non può esserci un passo automatico di tipo CS dopo un passo manuale di tipo CS
                            error = "WarningSignatureProcessesPassoAutoDopoPassoManualeCS";
                            return false;
                        }

                        #endregion
                    }

                    break;
                case Azione.DOC_SIGNATURE:
                case Azione.DOC_SIGNATURE_P:
                case Azione.RECORD_PREDISPOSED:
                case Azione.DOCUMENTO_REPERTORIATO:
                case Azione.DOCUMENTOSPEDISCI:
                    #region PASSI CON MODIFICHE AL DOCUMENTO DOPO DI PASSO AUTOMATICO CS CON STATO FINALE
                    //Se sto salvando/creando uno di questi passi verifico che non è presente un passo di tipo cambio con stato finale selezionato
                    if (!this.cbxFacoltativo.Checked)
                    {
                        List<PassoFirma> passiPrecDiTipoCS = ProcessoDiFirmaSelected.passi.Where(p => (p.numeroSequenza < numeroSequenza || (p.numeroSequenza == numeroSequenza && !string.IsNullOrEmpty(this.PassoDiFirmaSelected.idPasso))) && p.Evento.CodiceAzione.Equals(Azione.DOC_CAMBIO_STATO.ToString()) && !p.IsFacoltativo).ToList();
                        foreach (PassoFirma p in passiPrecDiTipoCS)
                        {
                            if (p.IsAutomatico && !string.IsNullOrEmpty(p.IdStatoDiagramma))
                            {
                                //Verifico se è un stato finale
                                Stato stato = UIManager.DiagrammiManager.GetStatoById(p.IdStatoDiagramma);
                                if (stato.STATO_FINALE)
                                {
                                    error = "WarningSignatureProcessesPassoEventoDopoStatoFinale";
                                    return false;
                                }
                            }
                        }
                    }
                    #endregion
                    break;
            }
            return result;
        }

        private string ControlloCampiObbligatori()
        {
            string errorMessage = string.Empty;

            if (this.RblRoleOrTypeRole.SelectedValue.Equals(RUOLO) && string.IsNullOrEmpty(this.idRuolo.Value))
            {
                errorMessage = "WarningRequiredFieldRole";
                return errorMessage;
            }
            else if (this.RblRoleOrTypeRole.SelectedValue.Equals(TIPO_RUOLO) && string.IsNullOrEmpty(this.DdlTypeRole.SelectedValue))
            {
                errorMessage = "WarningRequiredFieldTypeRole";
                return errorMessage;
            }
            //Se il passo è di tipo evento e automatico, controllo che sia stato inserito idAOO, idRF e nel caso di soedizione la casella
            if (RblTypeStep.SelectedValue.Equals(LibroFirmaManager.TypeStep.EVENT) && this.cbx_automatico.Checked)
            {
                if (string.IsNullOrEmpty(this.DdlRegistroAOO.SelectedValue))
                {
                    errorMessage = "WarningRequiredFieldRegistroRFSegnatura";
                    return errorMessage;
                }
                Azione codiceEvento = (Azione)Enum.Parse(typeof(Azione), this.DdlTypeEvent.SelectedValue, true);
                if(codiceEvento.Equals(Azione.DOCUMENTOSPEDISCI))
                {
                    if (string.IsNullOrEmpty(this.DdlRegistroRF.SelectedValue))
                    {
                        errorMessage = "WarningRequiredFieldRegistroRFMittente";
                        return errorMessage;
                    }
                    if (string.IsNullOrEmpty(this.DdlElencoCaselle.SelectedValue))
                    {
                        errorMessage = "WarningRequiredFieldCasellaMittente";
                        return errorMessage;
                    }
                }
                if (codiceEvento.Equals(Azione.DOC_CAMBIO_STATO))
                {
                    if (string.IsNullOrEmpty(this.DdlTipologiaDocumento.SelectedValue))
                    {
                        errorMessage = "WarningRequiredFieldTipologiaDocumento";
                        return errorMessage;
                    }
                    if (string.IsNullOrEmpty(this.DdlStatoDiagramma.SelectedValue))
                    {
                        errorMessage = "WarningRequiredFieldStatoDiagramma";
                        return errorMessage;
                    }
                }

            }
            return errorMessage;
        }

        private void PopolaPassoDiFirma()
        {
            PassoFirma passo = new PassoFirma();

            passo.idProcesso = ProcessoDiFirmaSelected.idProcesso;
            if (!string.IsNullOrEmpty(this.txtNr.Text))
            {
                passo.numeroSequenza = Convert.ToInt32(this.txtNr.Text);
            }
            else if (ProcessoDiFirmaSelected.passi != null)
            {
                passo.numeroSequenza = this.ProcessoDiFirmaSelected.passi.Length + 1;
            }
            else
            {
                passo.numeroSequenza = 1;
            }

            if (this.PassoDiFirmaSelected != null)
            {
                passo.idPasso = this.PassoDiFirmaSelected.idPasso;
                passo.Invalidated = PassoDiFirmaSelected.Invalidated;
            }

            passo.IsFacoltativo = this.cbxFacoltativo.Checked;
            ProcessoDiFirmaSelected.IsProcessModel = this.cbxFacoltativo.Checked;

            passo.Evento = new Evento();
            passo.Evento.TipoEvento = this.RblTypeStep.SelectedValue;

            #region TIPO FIRMA o TIPO EVENTO
            if (!RblTypeStep.SelectedValue.Equals(WAIT))
            {
                if (this.RblTypeStep.SelectedValue.Equals(SIGN))
                {
                    passo.Evento.Gruppo = this.rblTypeSignature.SelectedValue;
                    if (rblTypeSignature.SelectedValue.Equals(DIGITALE))
                    {
                        passo.Evento.CodiceAzione = this.rblTypeSignatureD.SelectedValue;
                    }
                    else
                    {
                        passo.Evento.CodiceAzione = this.rblTypeSignatureE.SelectedValue;
                    }
                }
                else
                {
                    passo.Evento.CodiceAzione = this.DdlTypeEvent.SelectedValue;
                    passo.Evento.Descrizione = this.DdlTypeEvent.SelectedItem.Text;
                    passo.IsAutomatico = this.cbx_automatico.Checked;
                    Azione codiceEvento = (Azione)Enum.Parse(typeof(Azione), this.DdlTypeEvent.SelectedValue, true);
                    if (passo.IsAutomatico)
                    {
                        switch (codiceEvento)
                        {
                            case Azione.RECORD_PREDISPOSED:
                            case Azione.DOCUMENTO_REPERTORIATO:
                                passo.IdAOO = this.DdlRegistroAOO.SelectedValue;
                                passo.IdRF = this.DdlRegistroRF.SelectedValue;
                                break;
                            case Azione.DOCUMENTOSPEDISCI:
                                passo.IdAOO = this.DdlRegistroAOO.SelectedValue;
                                passo.IdRF = this.DdlRegistroRF.SelectedValue;
                                passo.IdMailRegistro = this.DdlElencoCaselle.SelectedValue;
                                break;
                            case Azione.DOC_CAMBIO_STATO:
                                passo.IdTipologia = this.DdlTipologiaDocumento.SelectedValue;
                                passo.IdStatoDiagramma = this.DdlStatoDiagramma.SelectedValue;
                                break;
                        }
                    }
                }
                //Verifico se ho selezionato un ruolo o Tipo ruolo
                if (this.RblRoleOrTypeRole.SelectedValue.Equals(RUOLO))
                {
                    if (PassoDiFirmaSelected.ruoloCoinvolto != null && !string.IsNullOrEmpty(PassoDiFirmaSelected.ruoloCoinvolto.idGruppo))
                    {
                        if (this.PassoDiFirmaSelected != null)
                        {
                            passo.ruoloCoinvolto = PassoDiFirmaSelected.ruoloCoinvolto;
                        }

                        passo.utenteCoinvolto = new Utente();
                        passo.utenteCoinvolto.idPeople = this.ddlUtenteCoinvolto.SelectedValue;
                        passo.utenteCoinvolto.descrizione = this.ddlUtenteCoinvolto.SelectedItem != null ? this.ddlUtenteCoinvolto.SelectedItem.Text : string.Empty;
                    }
                }
                else
                {
                    passo.TpoRuoloCoinvolto = new TipoRuolo();
                    passo.TpoRuoloCoinvolto.systemId = this.DdlTypeRole.SelectedValue;
                    passo.TpoRuoloCoinvolto.descrizione = this.DdlTypeRole.SelectedItem.Text;
                }
                passo.note = this.txtNotes.Text;
                passo.idEventiDaNotificare = (from i in this.cbxOptionNotify.Items.Cast<ListItem>() where i.Selected select i.Value).ToArray();
            }
            #endregion

            #region TIPO ATTESA
            if (this.RblTypeStep.SelectedValue.Equals(LibroFirmaManager.TypeStep.WAIT))
            {
                List<AnagraficaEventi> eventWaiting = SignatureProcessesManager.GetEventTypes(WAIT);
                if (eventWaiting != null && eventWaiting.Count > 0)
                {
                    passo.Evento.CodiceAzione = eventWaiting[0].codiceAzione;
                    passo.Evento.Gruppo = eventWaiting[0].gruppo;
                }
            }

            #endregion

            this.PassoDiFirmaSelected = passo;
        }

        private void PopolaCampiPasso(PassoFirma p)
        {
            this.txtNr.Text = p.numeroSequenza.ToString();
            this.cbxFacoltativo.Checked = p.IsFacoltativo;
            string language = UIManager.UserManager.GetUserLanguage();
            this.lblSectionDocument.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLblSectionDocument", language) + this.txtNr.Text;
            this.RblTypeStep.SelectedValue = p.Evento.TipoEvento;
            this.RblTypeStep_SelectedIndexChanged(null, null);

            //Controllo se il passo è invalidato. Il passo può essere invalidato se
            // 1.Utente disabilitato o non più nel ruolo
            // 2.Ruolo modificato o disabilitato

            this.PnlWarningRoleUserDisabled.Visible = false;
            if(p.Invalidated == ROLE_DISABLED)
            {
                this.msgTextInvalidated.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesRoleDisabled", UserManager.GetUserLanguage());
                this.PnlWarningRoleUserDisabled.Visible = true;
            }               
            if(p.Invalidated == USER_DISABLED)
            {
                this.msgTextInvalidated.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesUserDisabled", UserManager.GetUserLanguage());
                this.PnlWarningRoleUserDisabled.Visible = true;
            }
            if (p.Invalidated == REGISTRO_DISABLED)
            {
                this.msgTextInvalidated.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesRegistroDisabled", UserManager.GetUserLanguage());
                this.PnlWarningRoleUserDisabled.Visible = true;
            }

            switch (p.Evento.TipoEvento)
            {
                case LibroFirmaManager.TypeStep.SIGN:
                    if (p.Evento.Gruppo.Equals(DIGITALE))
                    {
                        this.rblTypeSignature.SelectedValue = DIGITALE;
                        this.rblTypeSignatureD.SelectedValue = p.Evento.CodiceAzione;
                        this.plcTypeSignatureD.Visible = true;
                        this.plcTypeSignatureE.Visible = false;
                    }
                    else
                    {
                        this.rblTypeSignature.SelectedValue = ELETTRONICA;
                        this.rblTypeSignatureE.SelectedValue = p.Evento.CodiceAzione;
                        this.plcTypeSignatureD.Visible = false;
                        this.plcTypeSignatureE.Visible = true;
                    }
                    FillPanelDetailEvent_Sign(p);
                    break;
                case LibroFirmaManager.TypeStep.EVENT:
                    this.DdlTypeEvent.SelectedValue = p.Evento.CodiceAzione;
                    if (SignatureProcessesManager.IsEventoAutomatico(p.Evento.CodiceAzione))
                    {
                        Azione azione = (Azione)Enum.Parse(typeof(Azione), this.DdlTypeEvent.SelectedValue, true);
                        //Verifico se il ruolo è abilitato alla creazione del passo automatico
                        switch (azione)
                        {
                            case Azione.RECORD_PREDISPOSED:
                                this.cbx_automatico.Visible = UserManager.IsAuthorizedFunctions("CREA_PASSO_PROTO_AUTO");
                                break;
                            case Azione.DOCUMENTOSPEDISCI:
                                this.cbx_automatico.Visible = UserManager.IsAuthorizedFunctions("CREA_PASSO_SPEDIZIONE_AUTO");
                                break;
                            case Azione.DOCUMENTO_REPERTORIATO:
                                this.cbx_automatico.Visible = UserManager.IsAuthorizedFunctions("CREA_PASSO_REPERTORIAZIONE_AUTO");
                                break;
                            case Azione.DOC_CAMBIO_STATO:
                                this.cbx_automatico.Visible = UserManager.IsAuthorizedFunctions("CREA_PASSO_CAMBIO_STATO_AUTO");
                                break;
                        }
                    }
                    else
                    {
                        this.cbx_automatico.Visible = false;
                    }
                    this.cbx_automatico.Checked = p.IsAutomatico;
                    FillPanelDetailEvent_Sign(p);
                    break;
            }
        }

        private void FillPanelDetailEvent_Sign(PassoFirma p)
        {
            if (p.TpoRuoloCoinvolto != null && !string.IsNullOrEmpty(p.TpoRuoloCoinvolto.systemId))
            {
                this.DdlTypeRole.SelectedValue = p.TpoRuoloCoinvolto.systemId;
                this.RblRoleOrTypeRole.SelectedValue = TIPO_RUOLO;
                this.PnlTypeRole.Attributes.Add("style", "display:block");
                this.PnlRole.Attributes.Add("style", "display:none");
                this.PnlUenteCoinvolto.Attributes.Add("style", "display:none");
            }
            else
            {
                this.RblRoleOrTypeRole.SelectedValue = RUOLO;
                this.PnlRole.Attributes.Add("style", "display:block");
                this.PnlUenteCoinvolto.Attributes.Add("style", "display:block");
                this.PnlTypeRole.Attributes.Add("style", "display:none");
                if (p.Invalidated != USER_DISABLED && p.Invalidated != ROLE_DISABLED &&
                    p.ruoloCoinvolto != null && !string.IsNullOrEmpty(p.ruoloCoinvolto.idGruppo))
                {
                    this.TxtCodeRole.Text = p.ruoloCoinvolto.codiceRubrica;
                    this.TxtDescriptionRole.Text = p.ruoloCoinvolto.descrizione;
                    this.idRuolo.Value = p.ruoloCoinvolto.idGruppo;

                    this.LoadDllUtenteCoinvolto(UIManager.UserManager.getUserInRoleByIdGruppo(p.ruoloCoinvolto.idGruppo));
                    this.ddlUtenteCoinvolto.SelectedValue = p.utenteCoinvolto.idPeople;
                }
                else if (p.Invalidated == USER_DISABLED)
                {
                    this.TxtCodeRole.Text = p.ruoloCoinvolto.codiceRubrica;
                    this.TxtDescriptionRole.Text = p.ruoloCoinvolto.descrizione;
                    this.idRuolo.Value = p.ruoloCoinvolto.idGruppo;

                    this.LoadDllUtenteCoinvolto(UIManager.UserManager.getUserInRoleByIdGruppo(p.ruoloCoinvolto.idGruppo));
                }
                else
                {
                    this.TxtCodeRole.Text = string.Empty;
                    this.TxtDescriptionRole.Text = string.Empty;
                    this.idRuolo.Value = string.Empty;
                    this.ddlUtenteCoinvolto.ClearSelection();
                }
            }
            this.VisibilitaCampiAutomatici(p.IsAutomatico, p);
            this.txtNotes.Text = p.note;
            foreach (ListItem item in this.cbxOptionNotify.Items)
            {
                item.Selected = (from e in p.idEventiDaNotificare where e.Equals(item.Value) select e).FirstOrDefault() != null;
            }
        }

        private void RimuoviProcessoDiFirma()
        {
            string msg = string.Empty;
            if (UIManager.SignatureProcessesManager.RimuoviProcessoDiFirma(this.ProcessoDiFirmaSelected))
            {
                this.TreeSignatureProcess.Nodes[0].ChildNodes.Remove(this.TreeSignatureProcess.SelectedNode);
                this.upPnlTreeSignatureProcess.Update();
                this.ProcessoDiFirmaSelected = null;
                this.UpdateContentPage();
            }
            else
            {
                msg = "ErrorDeleteSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }
        
        private void RimuoviPassoProcessoDiFirma()
        {
            string msg = string.Empty;
            if (UIManager.SignatureProcessesManager.RimuoviPassoDiFirma(this.PassoDiFirmaSelected))
            {
                ProcessoFirma processo = UIManager.SignatureProcessesManager.GetProcessoDiFirma(PassoDiFirmaSelected.idProcesso);
                this.ProcessoDiFirmaSelected = processo;
                //Aggiorno il treeview
                TreeNode parentNode = this.TreeSignatureProcess.SelectedNode.Parent;
                this.UpdateNode(parentNode, processo);
                //Aggiorno il processo in sessione
                this.ListaProcessiDiFirma.Where(p => p.idProcesso.Equals(processo.idProcesso)).ToList().ForEach(f => f.passi = processo.passi);

                this.PassoDiFirmaSelected = null;
                this.CalcolaProssimoPasso(processo);
                this.UpdateContentPage();
            }
            else
            {
                msg = "ErrorDeleteStepSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        private void DecrementaNumeroSequenzaPassi(ref ProcessoFirma processo, PassoFirma passoRimosso)
        {
            if (passoRimosso.numeroSequenza < processo.passi.Length)
            {
                foreach (PassoFirma p in processo.passi)
                {
                    if (p.numeroSequenza > passoRimosso.numeroSequenza)
                        p.numeroSequenza -= 1;
                }
            }
        }


        private void IncrementaNumeroSequenzaPassi(ref ProcessoFirma processo, PassoFirma nuovoPasso)
        {
            if (nuovoPasso.numeroSequenza <= processo.passi.Length)
            {
                foreach (PassoFirma p in processo.passi)
                {
                    if (p.numeroSequenza >= nuovoPasso.numeroSequenza)
                        p.numeroSequenza += 1;
                }
            }
        }

        private void AggiornaNumeroSequenzaPassi(ref ProcessoFirma processo, PassoFirma passoAggiornato, int oldNumeroSequenza)
        {
            if (passoAggiornato.numeroSequenza > oldNumeroSequenza)
            {
                foreach (PassoFirma p in processo.passi)
                {
                    if (!p.idPasso.Equals(passoAggiornato.idPasso) && p.numeroSequenza > oldNumeroSequenza && p.numeroSequenza <= passoAggiornato.numeroSequenza)
                        p.numeroSequenza -= 1;
                }
            }
            else if (passoAggiornato.numeroSequenza < oldNumeroSequenza)
            {
                foreach (PassoFirma p in processo.passi)
                {
                    if (!p.idPasso.Equals(passoAggiornato.idPasso) && p.numeroSequenza >= passoAggiornato.numeroSequenza && p.numeroSequenza <= oldNumeroSequenza)
                        p.numeroSequenza += 1;
                }
            }
        }


        /// <summary>
        /// Costruisce i radio button per la firma
        /// </summary>
        private void Bind_RblSignature()
        {
            List<AnagraficaEventi> listSignatureEvent = SignatureProcessesManager.GetEventTypes(SIGN);

            #region TIPO FIRMA
            List<AnagraficaEventi> typeSignature = listSignatureEvent.GroupBy(e => e.gruppo).Select(g => g.First()).OrderBy(i => i.gruppo).ToList();
            if (typeSignature != null && typeSignature.Count > 0)
            {
                List<ListItem> listItem = new List<ListItem>();
                foreach (AnagraficaEventi evento in typeSignature)
                {
                    listItem.Add(new ListItem()
                    {
                        Text = Utils.Languages.GetLabelFromCode(evento.gruppo, UserManager.GetUserLanguage()),
                        Value = evento.gruppo
                    });
                }
                this.rblTypeSignature.Items.Clear();
                this.rblTypeSignature.Items.AddRange(listItem.ToArray());
            }

            #endregion

            #region TIPO FIRMA DIGITALE

            List<AnagraficaEventi> typeDigitalSignature = listSignatureEvent.Where(e => e.gruppo.Equals(DIGITALE)).OrderBy(g => g.idEvento).ToList(); ;
            if (typeDigitalSignature != null && typeDigitalSignature.Count > 0)
            {
                List<ListItem> listItem = new List<ListItem>();
                foreach (AnagraficaEventi evento in typeDigitalSignature)
                {
                    listItem.Add(new ListItem()
                    {
                        Text = evento.descrizione,
                        Value = evento.codiceAzione
                    });
                }
                this.rblTypeSignatureD.Items.Clear();
                this.rblTypeSignatureD.Items.AddRange(listItem.ToArray());
            }

            #endregion

            #region  TIPO FIRMA ELETTRONICA

            List<AnagraficaEventi> typeElectronicsSignature = listSignatureEvent.Where(e => e.gruppo.Equals(ELETTRONICA)).OrderBy(g => g.idEvento).ToList();
            if (typeElectronicsSignature != null && typeElectronicsSignature.Count > 0)
            {
                List<ListItem> listItem = new List<ListItem>();
                foreach (AnagraficaEventi evento in typeElectronicsSignature)
                {
                    listItem.Add(new ListItem()
                    {
                        Text = Utils.Languages.GetLabelFromCode(evento.codiceAzione, UserManager.GetUserLanguage()),
                        Value = evento.codiceAzione
                    });
                }
                this.rblTypeSignatureE.Items.Clear();
                this.rblTypeSignatureE.Items.AddRange(listItem.ToArray());
            }


            #endregion
        }

        private void CalcolaProssimoPasso(ProcessoFirma processo)
        {
            this.lblSectionDocument.Text = Utils.Languages.GetLabelFromCode("SignatureProcessesLblSectionDocument", UIManager.UserManager.GetUserLanguage()) +
                 (processo.passi.Length + 1).ToString();

            this.txtNr.Text = (processo.passi.Length + 1).ToString();

            this.pnlStep.Visible = true;
            this.UpdPnlStep.Update();
        }

        /// <summary>
        /// Verifica se il ruolo è abilitato ad effettuare la firma selezionata
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        private bool IsRuoloAbilitatoAlPassoSelezionato(Ruolo role)
        {
            bool isAuthorizedSign = true;
            if (this.RblTypeStep.SelectedValue.Equals(LibroFirmaManager.TypeStep.SIGN))
            {
                if (this.rblTypeSignature.SelectedValue.Equals(DIGITALE))
                {
                    isAuthorizedSign = UserManager.IsRoleAuthorizedFunctions("DO_DOC_FIRMA", role) || UserManager.IsRoleAuthorizedFunctions("FIRMA_HSM", role);
                }
                if (this.rblTypeSignature.SelectedValue.Equals(ELETTRONICA))
                {
                    if (this.rblTypeSignatureE.SelectedValue.Equals(LibroFirmaManager.TypeEvent.VERIFIED))
                    {
                        isAuthorizedSign = UserManager.IsRoleAuthorizedFunctions("DO_DOC_FIRMA_ELETTRONICA", role);
                    }
                    else if (this.rblTypeSignatureE.SelectedValue.Equals(LibroFirmaManager.TypeEvent.ADVANCEMENT_PROCESS))
                    {
                        isAuthorizedSign = UserManager.IsRoleAuthorizedFunctions("DO_DOC_AVANZAMENTO_ITER", role);
                    }
                }
            }
            if (this.RblTypeStep.SelectedValue.Equals(LibroFirmaManager.TypeStep.EVENT))
            {
                Azione codiceEvento = (Azione)Enum.Parse(typeof(Azione), this.DdlTypeEvent.SelectedValue, true);
                switch (codiceEvento)
                {
                    case Azione.RECORD_PREDISPOSED:
                        //Verifico che il ruolo selezionato ha la funzionalità di protocollazione abilitata
                        if (cbx_automatico.Checked)
                            isAuthorizedSign = UserManager.IsRoleAuthorizedFunctions("DO_PROT_PROTOCOLLA", role) && UserManager.IsRoleAuthorizedFunctions("DO_PROT_PROTOCOLLA_AUTOMATICO", role);
                        else
                            isAuthorizedSign = UserManager.IsRoleAuthorizedFunctions("DO_PROT_PROTOCOLLA", role);
                        break;
                    case Azione.DOCUMENTO_REPERTORIATO:
                        if(cbx_automatico.Checked)
                            isAuthorizedSign = UserManager.IsRoleAuthorizedFunctions("DO_REPERTORIAZIONE_ATOMATICA", role);
                        break;
                    case Azione.DOCUMENTOSPEDISCI:
                        //Verifico che il ruolo selezionato ha la funzionalità di protocollazione abilitata
                        if (cbx_automatico.Checked)
                            isAuthorizedSign = UserManager.IsRoleAuthorizedFunctions("DO_OUT_SPEDISCI", role) && UserManager.IsRoleAuthorizedFunctions("DO_OUT_SPEDISCI_AUTOMATICA", role);
                        else
                            isAuthorizedSign = UserManager.IsRoleAuthorizedFunctions("DO_OUT_SPEDISCI", role);
                        break;
                }
            }
            return isAuthorizedSign;
        }

        protected void LoadCustomDocuments()
        {
            ListItem empty = new ListItem("", "");
            this.DdlTipologiaDocumento.Items.Add(empty);
            this.DdlTipologiaDocumento.SelectedIndex = -1;
            if (!string.IsNullOrEmpty(this.idRuolo.Value) && this.PassoDiFirmaSelected.ruoloCoinvolto != null)
            {
                DocsPaWR.TipologiaAtto[] listCustomDocuments = listCustomDocuments = UIManager.DocumentManager.GetCustomDocumentsLiteWithStateDiagram(UIManager.UserManager.GetInfoUser().idAmministrazione, this.PassoDiFirmaSelected.ruoloCoinvolto.idGruppo, "2");

                if (listCustomDocuments != null)
                {
                    for (int i = 0; i < listCustomDocuments.Length; i++)
                    {
                        ListItem item = new ListItem(listCustomDocuments[i].descrizione, listCustomDocuments[i].systemId);
                        this.DdlTipologiaDocumento.Items.Add(item);
                    }
                    this.DdlTipologiaDocumento.Enabled = true;
                }

                //Se esiste un passo precedente di tipo cambio in cui è selezionata la tipologia seleziono di default la stessa tipologia
                string idTipologia = GetTipologiaSelezionataPassoCSPrecedente();
                if(!string.IsNullOrEmpty(idTipologia) && DdlTipologiaDocumento.Items.FindByValue(idTipologia) != null)
                {
                    this.DdlTipologiaDocumento.SelectedValue = idTipologia;
                }
            }
        }

        private string GetTipologiaSelezionataPassoCSPrecedente()
        {
            string idTipologiaSelezionata = string.Empty;

            //Seleziono il prima passo cambio stato automatico non facoltativo con tipologia selezionata
            PassoFirma passoPrec = ProcessoDiFirmaSelected.passi.Where(p => p.Evento.CodiceAzione.Equals(Azione.DOC_CAMBIO_STATO.ToString()) && !p.IsFacoltativo && p.IsAutomatico && !string.IsNullOrEmpty(p.IdTipologia)).FirstOrDefault();
            if (passoPrec != null)
                idTipologiaSelezionata = passoPrec.IdTipologia;

            return idTipologiaSelezionata;
        }

        protected void LoadStateDiagram()
        {
            this.DdlStatoDiagramma.Items.Clear();
            ListItem empty = new ListItem("", "");
            this.DdlStatoDiagramma.Items.Add(empty);
            this.DdlStatoDiagramma.SelectedIndex = -1;
            if (!string.IsNullOrEmpty(this.DdlTipologiaDocumento.SelectedItem.Value))
            {
                DiagrammaStato diagramma = DiagrammiManager.getDgByIdTipoDoc(this.DdlTipologiaDocumento.SelectedItem.Value, UIManager.UserManager.GetInfoUser().idAmministrazione);
                if (diagramma != null)
                {
                    List<Stato> stati = CalcolaStatiSuccessivi(diagramma, Convert.ToInt32(txtNr.Text));
                    if (stati != null && stati.Count() > 0)
                    {
                        foreach(Stato s in stati)
                        {
                            ListItem item = new ListItem(s.DESCRIZIONE, s.SYSTEM_ID.ToString());
                            this.DdlStatoDiagramma.Items.Add(item);
                        }
                    }
                    this.DdlStatoDiagramma.Enabled = true;
                }
            }
            else
            {
                this.DdlStatoDiagramma.Enabled = false;
            }
            this.UpPnlStatoDiagramma.Update();
        }

        private List<Stato> CalcolaStatiSuccessivi(DiagrammaStato diagramma, int numeroSequenzaPassoCorrente)
        {
            List<Stato> statiSuccessivi = new List<Stato>();

            if (numeroSequenzaPassoCorrente == 1)
            {
                //E' il primo passo quindi gli stati successivi sono tutti gli stati del diagramma
                if (diagramma.STATI != null && diagramma.STATI.Count() > 0)
                {
                    for (int i = 0; i < diagramma.STATI.Length; i++)
                    {
                        if (!diagramma.STATI[i].STATO_SISTEMA)
                        {
                            statiSuccessivi.Add(diagramma.STATI[i]);
                        }
                    }
                }
            }
            else
            {
                //verifico se esiste un passo precedente con cambio stato. Se esiste e non è facoltativo vedo lo stato selezionato ed in base a quello popolo la combo
                //con gli stati successivi
                PassoFirma passoPrec = ProcessoDiFirmaSelected.passi.Where(p => p.numeroSequenza < Convert.ToInt32(this.txtNr.Text) && p.Evento.CodiceAzione.Equals(Azione.DOC_CAMBIO_STATO.ToString())).OrderByDescending(p => p.numeroSequenza).FirstOrDefault();
                if(passoPrec != null && !passoPrec.IsFacoltativo && !string.IsNullOrEmpty(passoPrec.IdStatoDiagramma))
                {
                    for (int i = 0; i < diagramma.PASSI.Length; i++)
                    {
                        DocsPaWR.Passo step = (DocsPaWR.Passo)diagramma.PASSI[i];
                        if (step.STATO_PADRE.SYSTEM_ID.ToString().Equals(passoPrec.IdStatoDiagramma))
                        {
                            for (int j = 0; j < step.SUCCESSIVI.Length; j++)
                            {
                                DocsPaWR.Stato st = (DocsPaWR.Stato)step.SUCCESSIVI[j];
                                if (DiagrammiManager.IsRuoloAssociatoStatoDia(diagramma.SYSTEM_ID.ToString(), this.PassoDiFirmaSelected.ruoloCoinvolto.idGruppo, st.SYSTEM_ID.ToString()))
                                {
                                    if (!st.STATO_SISTEMA)
                                        statiSuccessivi.Add(st);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Non esiste o non è selezionato nessuno stato, quindi mostro tutti gli stati del diagramma
                    //E' il primo passo quindi gli stati successivi sono tutti quelli del diagramma
                    if (diagramma.STATI != null && diagramma.STATI.Count() > 0)
                    {
                        for (int i = 0; i < diagramma.STATI.Length; i++)
                        {
                            if (!diagramma.STATI[i].STATO_SISTEMA)
                            {
                                statiSuccessivi.Add(diagramma.STATI[i]);
                            }
                        }
                    }
                }
            }

            return statiSuccessivi;
        }

        #endregion
    }
}
