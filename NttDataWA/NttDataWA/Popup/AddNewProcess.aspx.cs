using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class AddNewProcess : System.Web.UI.Page
    {
        #region Properties

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

        public RubricaCallType CallType
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

        #endregion

        #region CONSTANT

        private const string COPIA = "C";

        #endregion

        #region Standard Method
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
            }

            RefreshScript();
        }

        private void InitializePage()
        {
            InitializeLanguage();
            string tipoOperazione = string.Empty;
            if(Request.QueryString["tipooperazione"] != null)
                tipoOperazione = Request.QueryString["tipooperazione"].ToString();
            if (tipoOperazione.ToUpper().Equals(COPIA))
            {
                this.PnlCopiaProcesso.Visible = true;
                this.PnlDuplicaProcesso.Visible = false;
                SetAjaxAddressBook();
            }
            else
            {
                this.AddNewProcessSave.Enabled = true;
                this.PnlCopiaProcesso.Visible = false;
                this.PnlDuplicaProcesso.Visible = true;
                this.txt_processName.Text = this.ProcessoDiFirmaSelected.nome;
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.AddNewProcessClose.Text = Utils.Languages.GetLabelFromCode("AddNewProcessClose", language);
            this.AddNewProcessSave.Text = Utils.Languages.GetLabelFromCode("AddNewProcessSave", language);
            this.ProcessName.Text = Utils.Languages.GetLabelFromCode("AddNewProcessName", language);
            this.cbxCopiaVisibilita.Text = Utils.Languages.GetLabelFromCode("AddNewProcessCbxCopiaVisibilita", language);
            this.cbxMantieni.Text = Utils.Languages.GetLabelFromCode("AddNewProcessCbxMantieni", language);
            this.ltlRuolo.Text = Utils.Languages.GetLabelFromCode("AddNewProcessRuolo", language);
            this.ltlUtente.Text = Utils.Languages.GetLabelFromCode("AddNewProcessUtente", language);
            this.ddlUtente.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("AddNewProcessddlUtente", language));
            string tipoOperazione = string.Empty;
            if (Request.QueryString["tipooperazione"] != null)
                tipoOperazione = Request.QueryString["tipooperazione"].ToString();
            if (tipoOperazione.ToUpper().Equals(COPIA))
            {
                this.AddNewProcessSave.Text = Utils.Languages.GetLabelFromCode("AddNewProcessCopia", language);
                this.AddNewProcessClose.Text = Utils.Languages.GetLabelFromCode("AddNewProcessChiudi", language);
            }
            this.LtlProcessiNonCopiati.Text = Utils.Languages.GetLabelFromCode("AddNewProcessLtlProcessiNonCopiati", language);
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen_deselect", "$('.chzn-select-deselect').chosen({ allow_single_deselect: true, no_results_text: 'Nessun risultato trovato' });", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen", "$('.chzn-select').chosen({ no_results_text: 'Nessun risultato trovato' });", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        protected void SetAjaxAddressBook()
        {
            string dataUser = UIManager.RoleManager.GetRoleInSession().systemId;
            Registro reg = RegistryManager.GetRegistryInSession();
            if (reg == null)
            {
                reg = RoleManager.GetRoleInSession().registri[0];
            }
            dataUser = dataUser + "-" + reg.systemId;

            string callType = "CALLTYPE_IN_ONLY_ROLE";
            this.RapidCorr.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
        }

        #endregion

        #region Event Buttons

        protected void AddNewProcessSave_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                string msg = string.Empty;
                string tipoOperazione = string.Empty;
                if (Request.QueryString["tipooperazione"] != null)
                    tipoOperazione = Request.QueryString["tipooperazione"].ToString();
                //Copia dei processi in altro ruolo
                if (tipoOperazione.ToUpper().Equals(COPIA))
                {
                    if (!string.IsNullOrEmpty(this.idRuolo.Value))
                    {
                        //Verifico che il ruolo ha la microfunzione per la creazione dei processi
                        Ruolo ruolo = RoleManager.getRuoloByIdGruppo(this.idRuolo.Value);
                        if(!UserManager.IsRoleAuthorizedFunctions("GEST_PROCESSI_DI_FIRMA", ruolo))
                        {
                            msg = "WarningRuoloNonAbilitatoAGestioneProcessi";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return;
                        }

                        List<ProcessoFirma> listaProcessiDaCopiare = null;
                        if (ProcessoDiFirmaSelected != null && !string.IsNullOrEmpty(ProcessoDiFirmaSelected.idProcesso))
                        {
                            listaProcessiDaCopiare = new List<ProcessoFirma>();
                            listaProcessiDaCopiare.Add(ProcessoDiFirmaSelected);
                        }
                        else
                        {
                            listaProcessiDaCopiare = ListaProcessiDiFirma;
                        }
                        string idPeople = this.ddlUtente.SelectedValue;
                        List<CopiaProcessiFirmaResult> resultCopiaProcessi = SignatureProcessesManager.CopiaProcessiFirma(listaProcessiDaCopiare, this.cbxCopiaVisibilita.Checked, this.cbxMantieni.Checked, idRuolo.Value, idPeople);
                        List<CopiaProcessiFirmaResult> processiNonCopiati = (from r in resultCopiaProcessi
                                                                             where r.esito != ResultProcessoFirma.OK
                                                                             select r).ToList();
                        if(processiNonCopiati == null || processiNonCopiati.Count == 0)
                        {
                            this.PnlListaProcessiNonCopiati.Visible = false;
                            msg = "SuccessAddNewProcessCopia";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'check', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeAJM", "parent.closeAjaxModal('CopySignatureProcess','up');", true);
                        }
                        else
                        {
                            GeneraReport(processiNonCopiati);
                            msg = "WarningAddNewProcessErroreCopia";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        this.UpnlListaProcessiNonCopiati.Update();
                        return;
                    }
                    else
                    {
                        msg = "WarningRequiredFieldRuoloDestinatario";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                }
                else //Dupplicazione del processo di firma nel ruolo corrente
                {
                    ResultProcessoFirma result = ResultProcessoFirma.OK;
                    if (string.IsNullOrEmpty(this.txt_processName.Text))
                    {
                        msg = "WarningRequiredFieldNameProcess";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                    ProcessoFirma processo = SignatureProcessesManager.DuplicaProcessoFirma(ProcessoDiFirmaSelected, this.txt_processName.Text, this.cbxCopiaVisibilita.Checked, out result);
                    switch (result)
                    {
                        case ResultProcessoFirma.OK:
                            ProcessoDiFirmaSelected = processo;
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeAJM", "parent.closeAjaxModal('AddNewProcess','up');", true);
                            break;
                        case ResultProcessoFirma.KO:
                            msg = "ErrorCreationProcess";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                            break;
                        case ResultProcessoFirma.EXISTING_PROCESS_NAME:
                            msg = "WarningSignatureProcessUniqueProcessName";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "ErrorSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        private void GeneraReport(List<CopiaProcessiFirmaResult> resultCopia)
        {
            MassiveOperationReport report = new MassiveOperationReport();
            MassiveOperationReport.MassiveOperationResultEnum result;
            string nome;
            string details;
            string language = UIManager.UserManager.GetUserLanguage();

            foreach (CopiaProcessiFirmaResult copia in resultCopia)
            {
                result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                nome = copia.nomeProcesso;
                details = Utils.Languages.GetLabelFromCode(copia.esito.ToString(), language); 
                report.AddReportRow(
                    nome,
                    result,
                    details);
            }

            this.grdReport.DataSource = report.GetDataSet();
            this.grdReport.DataBind();
            this.PnlListaProcessiNonCopiati.Visible = true;
        }

        protected void AddNewProcessClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                string tipoOperazione = string.Empty;
                if (Request.QueryString["tipooperazione"] != null)
                    tipoOperazione = Request.QueryString["tipooperazione"].ToString();
                if (tipoOperazione.ToUpper().Equals(COPIA))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeAJM", "parent.closeAjaxModal('CopySignatureProcess','');", true);
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeAJM", "parent.closeAjaxModal('AddNewProcess','');", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnAddressBook_Click(object sender, EventArgs e)
        {
            this.CallType = RubricaCallType.CALLTYPE_CORR_INT;
            HttpContext.Current.Session["AddressBook.from"] = "COPIA_PROCESSO";
            HttpContext.Current.Session["AddressBook.EnableOnly"] = "R";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAddressBook", "parent.ajaxModalPopupAddressBook();", true);
        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
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
                        this.txtCodiceRuolo.Text = corr.codiceRubrica;
                        this.txtDescrizioneRuolo.Text = corr.descrizione;
                        this.idRuolo.Value = ruolo.idGruppo;
                        this.LoadDllUtenteCoinvolto(UIManager.UserManager.getUserInRoleByIdGruppo(ruolo.idGruppo));
                        this.AddNewProcessSave.Enabled = true;
                        this.UpdPnlRuolo.Update();
                        this.UpPnlButtons.Update();
                    }

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

        private void LoadDllUtenteCoinvolto(List<Utente> listUserInRole)
        {
            this.ddlUtente.Items.Clear();

            if (listUserInRole != null && listUserInRole.Count > 0)
            {
                ListItem empty = new ListItem("", "");
                this.ddlUtente.Items.Add(empty);
                this.ddlUtente.SelectedIndex = -1;

                for (int i = 0; i < listUserInRole.Count; i++)
                {
                    ListItem item = new ListItem(listUserInRole[i].descrizione, listUserInRole[i].systemId);
                    this.ddlUtente.Items.Add(item);
                }

                this.ddlUtente.Enabled = true;
            }
            else
            {
                this.ddlUtente.Enabled = false;
            }        
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            try
            {
                this.AddNewProcessSave.Enabled = false;
                string codice = txtCodiceRuolo.Text;
                this.txtCodiceRuolo.Text = string.Empty;
                this.txtDescrizioneRuolo.Text = string.Empty;
                this.ddlUtente.Items.Clear();
                this.ddlUtente.Enabled = false;
                if (!string.IsNullOrEmpty(codice))
                {
                    RubricaCallType calltype = RubricaCallType.CALLTYPE_PROTO_INT_MITT;
                    ElementoRubrica[] listaCorr = null;
                    Corrispondente corr = null;
                    UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
                    listaCorr = UIManager.AddressBookManager.getElementiRubricaMultipli(codice, calltype, true);
                    if (listaCorr != null && (listaCorr.Count() == 1))
                    {
                        if (listaCorr.Count() == 1)
                        {
                            corr = UIManager.AddressBookManager.getCorrispondenteRubrica(codice, calltype);
                        }
                        if (corr == null)
                        {
                            string msg = "ErrorTransmissionCorrespondentNotFound";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        if (!corr.tipoCorrispondente.Equals("R"))
                        {
                            string msg = "WarningCorrespondentAsRole";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        else
                        {
                            this.txtCodiceRuolo.Text = corr.codiceRubrica;
                            this.txtDescrizioneRuolo.Text = corr.descrizione;
                            this.idRuolo.Value = ((DocsPaWR.Ruolo)corr).idGruppo;
                            this.LoadDllUtenteCoinvolto(UIManager.UserManager.getUserInRoleByIdGruppo(this.idRuolo.Value));
                            this.AddNewProcessSave.Enabled = true;
                            this.UpPnlButtons.Update();
                        }
                    }
                    else
                    {
                        string msg = "ErrorTransmissionCorrespondentNotFound";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                }
                this.UpdPnlRuolo.Update();
            }
            catch (Exception ex)
            {
                string msg = "ErrorSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        #endregion
    }
}