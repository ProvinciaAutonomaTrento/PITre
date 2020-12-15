using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDatalLibrary;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class AddFilterNotificationCenter : System.Web.UI.Page
    {
        #region Property

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

        private string TypeDocument
        {
            get
            {
                return HttpContext.Current.Session["typeDoc"].ToString();

            }
            set
            {
                if (value != null)
                    HttpContext.Current.Session["typeDoc"] = value;
                else if (!string.IsNullOrEmpty(Request.QueryString["t"]))
                    HttpContext.Current.Session["typeDoc"] = Request.QueryString["t"];
                else
                    HttpContext.Current.Session["typeDoc"] = string.Empty;
            }
        }

        public string IdCustomObjectCustomCorrespondent
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["idCustomObjectCustomCorrespondent"] != null)
                {
                    result = HttpContext.Current.Session["idCustomObjectCustomCorrespondent"] as string;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["idCustomObjectCustomCorrespondent"] = value;
            }
        }

        public NotificationsFilters Filters
        {
            get
            {
                if (HttpContext.Current.Session["Filters"] != null)
                {
                    return HttpContext.Current.Session["Filters"] as NotificationsFilters;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                HttpContext.Current.Session["Filters"] = value;
            }
        }

        private string ReturnValue
        {
            get
            {
                if ((HttpContext.Current.Session["ReturnValuePopup"]) != null)
                    return HttpContext.Current.Session["ReturnValuePopup"].ToString();
                else
                    return string.Empty;
            }
        }

        #endregion

        #region Const

        private const string UPDATE_PANEL_BUTTONS  = "UpPnlButtons";
        private const string CLOSE_POPUP_OBJECT = "closePopupObject";
        private const string INTERVAL = "1";

        #endregion

        #region Standard Method

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ClearSession();
                InitializeLanguage();
                InitializePage();       
            }
            ReadRetValueFromPopup();
            RefreshScript();
        }



        private void ClearSession()
        {
            this.TypeDocument = "Search";
            UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
        }

        private void ReadRetValueFromPopup()
        {

        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.AddFilterBtnConfirm.Text = Utils.Languages.GetLabelFromCode("AddFilterBtnConfirm", language);
            this.AddFilterBtnCancel.Text = Utils.Languages.GetLabelFromCode("AddFilterBtnCancel", language);
            this.rbDocumentTypeInbound.Text = Utils.Languages.GetLabelFromCode("AddFilterRbDocumentTypeInbound", language);
            this.rbDocumentTypeOutbound.Text = Utils.Languages.GetLabelFromCode("AddFilterRbDocumentTypeOutbound", language);
            this.rbDocumentTypeInternal.Text = Utils.Languages.GetLabelFromCode("AddFilterRbDocumentTypeInternal", language);
            this.rbDocumentTypeDocument.Text = Utils.Languages.GetLabelFromCode("AddFilterRbDocumentTypeDocument", language);
            this.rbDocumentTypePrepared.Text = Utils.Languages.GetLabelFromCode("AddFilterRbDocumentTypePrepared", language);
            this.rbDocumentTypeAll.Text = Utils.Languages.GetLabelFromCode("AddFilterRbDocumentTypeAll", language);
            this.cbDocumentAcquired.Text = Utils.Languages.GetLabelFromCode("AddFilterCbDocumentAcquired", language);
            this.cbDocumentSigned.Text = Utils.Languages.GetLabelFromCode("AddFilterCbDocumentSigned", language);
            this.cbDocumentNotSigned.Text = Utils.Languages.GetLabelFromCode("AddFilterCbDocumentNotSigned", language);
            this.lblWaitingAcceptance.Text = Utils.Languages.GetLabelFromCode("AddFilterLblWaitingAcceptance", language);
            this.ddlTypeFileAcquired.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("AddFilterDdlTypeFileAcquired", language));
            this.lblDateEvent.Text = Utils.Languages.GetLabelFromCode("AddFilterLblDateTrasmission", language);
            this.ddlItemValueSingle.Text = Utils.Languages.GetLabelFromCode("AddFilterDdlItemValueSingle", language);
            this.ddlItemInterval.Text = Utils.Languages.GetLabelFromCode("AddFilterDdlItemInterval", language);
            this.lblExpirationDate.Text = Utils.Languages.GetLabelFromCode("AddFilterLblExpirationDate", language);
            this.ddlExpirationDateItemValueSingle.Text = Utils.Languages.GetLabelFromCode("AddFilterDdlItemValueSingle", language);
            this.ddlExpirationDateItemInterval.Text = Utils.Languages.GetLabelFromCode("AddFilterDdlItemInterval", language);
            this.lblSenderTransmission.Text = Utils.Languages.GetLabelFromCode("AddFilterLblSenderTransmission", language);
            this.optUO.Text = Utils.Languages.GetLabelFromCode("AddFilterRbOptUO", language);
            this.optRole.Text = Utils.Languages.GetLabelFromCode("AddFilterRbOptRole", language);
            this.optUser.Text = Utils.Languages.GetLabelFromCode("AddFilterRbOptUser", language);
            this.lblObject.Text = Utils.Languages.GetLabelFromCode("AddFilterLblObject", language);
            this.lblTypeFileAcquired.Text = Utils.Languages.GetLabelFromCode("AddFilterLblTypeFileAcquired", language);
            this.ltlDateEventFrom.Text = Utils.Languages.GetLabelFromCode("AddFilterLtlDateFrom", language);
            this.ltlDateEventTo.Text = Utils.Languages.GetLabelFromCode("AddFilterLtlDateTo", language);
            this.ltlExpirationDateFrom.Text = Utils.Languages.GetLabelFromCode("AddFilterLtlDateFrom", language);
            this.ltlExpirationDateTo.Text = Utils.Languages.GetLabelFromCode("AddFilterLtlDateTo", language);
            this.ImageButtonAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("AddFilterButtonAddressBookTooltip", language);
            this.ImageButtonObjectary.ToolTip = Utils.Languages.GetLabelFromCode("AddFilterButtonObjectaryTooltip", language);
            this.lblSectionDocument.Text = Utils.Languages.GetLabelFromCode("AddFilterLblSectionDocument", language);
            this.lblNotes.Text = Utils.Languages.GetLabelFromCode("AddFilterLblNotes", language);
        }

        protected void SetAjaxAddressBook()
        {
            string dataUser = RoleManager.GetRoleInSession().systemId;
            dataUser = dataUser + "-" + RegistryManager.GetRegistryInSession().systemId;

            string callType = "CALLTYPE_OWNER_AUTHOR";
            this.RapidCreatore.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
        }

        private void InitializePage()
        {
            SetAjaxAddressBook();
            LoadTypeFileAcquired();
            if (UserManager.IsAuthorizedFunctions("HOME_NOTES"))
            {
                this.PlaceHolderNotes.Visible = true;
            }

            if (!InternalRecordEnable())
            {
                this.rblFilterDocumentType.Items.RemoveAt(2);
            }

            if (Filters != null)
            {
                PopolateFields();
            }
        }

        public bool InternalRecordEnable()
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString())) && Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString()).Equals("1"))
            {
                retVal = true;
            }
            else
            {
                retVal = false;
            }
            
            return retVal;
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        //Caricamento tipo file acquisito
        private void LoadTypeFileAcquired()
        {
            ArrayList typeFile = new ArrayList();
            typeFile.Add("");
            typeFile.AddRange(DocumentManager.getExtFileAcquisiti(this.Page));
            this.ddlTypeFileAcquired.DataSource = (from type in typeFile.ToArray() where !type.ToString().Contains("P7M") select type.ToString()).ToList();
            this.ddlTypeFileAcquired.DataBind();
        }

        #endregion

        #region Event hendler buttom

        protected void AddFilterBtnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.PlaceHolderDateEventTo.Visible)
                { 
                    if((!string.IsNullOrEmpty(this.txt_dateEventFrom.Text) && string.IsNullOrEmpty(this.txt_dateEventTo.Text)) ||
                        (string.IsNullOrEmpty(this.txt_dateEventFrom.Text) && !string.IsNullOrEmpty(this.txt_dateEventTo.Text)) ||
                         Utils.utils.verificaIntervalloDate(this.txt_dateEventFrom.Text, this.txt_dateEventTo.Text)
                        )
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningIntervalDateEvent', 'warning', '','',null,null,'')", true);
                        return;
                    }
                }
                if (this.PlaceHolderExpirationDateTo.Visible)
                {
                    if ((!string.IsNullOrEmpty(this.txt_expirationDateFrom.Text) && string.IsNullOrEmpty(this.txt_expirationDateTo.Text)) ||
                        (string.IsNullOrEmpty(this.txt_expirationDateFrom.Text) && !string.IsNullOrEmpty(this.txt_expirationDateTo.Text)) ||
                         Utils.utils.verificaIntervalloDate(this.txt_expirationDateFrom.Text, this.txt_expirationDateTo.Text)
                        )
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningIntervalDateExpire', 'warning', '','',null,null,'')", true);
                        return;
                    }
                }
                BuildFiltersSearchNotifications();
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('AddFilterNotificationCenter','up');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DdlDateEvent_SelectedIndexChanged(object sender, EventArgs e)
        { 
            try
            {
                switch (this.ddl_dateEvent.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.ltlDateEventFrom.Visible = false;
                        this.PlaceHolderDateEventTo.Visible = false;
                        break;
                    case 1: //Intervallo
                        this.ltlDateEventFrom.Visible = true;
                        this.PlaceHolderDateEventTo.Visible = true;
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ImageButtonAddressBook_Click(object sender, EventArgs e)
        {
            try
            {
                this.CallType = RubricaCallType.CALLTYPE_OWNER_AUTHOR;
                HttpContext.Current.Session["AddressBook.from"] = "F_X_X_S";
                HttpContext.Current.Session["AddressBook.EnableOnly"] = this.rblOwnerType.SelectedValue;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAddressBook", "parent.ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                CustomTextArea caller = sender as CustomTextArea;
                string codeAddressBook = caller.Text;

                if (!string.IsNullOrEmpty(codeAddressBook))
                {
                    this.SearchCorrespondent(codeAddressBook, caller.ID);
                }
                else
                {      
                    this.txtCodiceCreatore.Text = string.Empty;
                    this.txtDescrizioneCreatore.Text = string.Empty;
                    this.idCreatore.Value = string.Empty;
                    this.upPnlAuthor.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

      
        protected void btnObjectPostback_Click(object sender, EventArgs e)
        {
            this.TxtObject.Text = this.ReturnValue.Split('#').First();
            if (this.ReturnValue.Split('#').Length > 1)
                this.TxtCodeObject.Text = this.ReturnValue.Split('#').Last();
            this.UpdPnlObject.Update();
        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> ccList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.Cc"];
                if (atList != null && atList.Count > 0)
                {
                    NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                    Corrispondente tempCorrSingle;
                    if (!corrInSess.isRubricaComune)
                        tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                    else
                        tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                    this.txtCodiceCreatore.Text = tempCorrSingle.codiceRubrica;
                    this.txtDescrizioneCreatore.Text = tempCorrSingle.descrizione;
                    this.idCreatore.Value = tempCorrSingle.systemId;
                    this.upPnlAuthor.Update();
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

        protected void DdlExpirationDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (this.ddl_expirationDate.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.ltlExpirationDateFrom.Visible = false;
                        this.PlaceHolderExpirationDateTo.Visible = false;
                        break;
                    case 1: //Intervallo
                        this.ltlExpirationDateFrom.Visible = true;
                        this.PlaceHolderExpirationDateTo.Visible = true;
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void AddFilterBtnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('AddFilterNotificationCenter','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #region methods

        private void BuildFiltersSearchNotifications()
        {
            Filters = new NotificationsFilters();
            Filters.TYPE_DOCUMENT = this.rblFilterDocumentType.SelectedValue.Equals("ALL") ? string.Empty : this.rblFilterDocumentType.SelectedValue;
            Filters.DOCUMENT_ACQUIRED = this.cbDocumentAcquired.Selected;
            Filters.DOCUMENT_SIGNED = this.cbDocumentSigned.Selected;
            Filters.DOCUMENT_UNSIGNED = this.cbDocumentNotSigned.Selected;
            Filters.PENDING = this.cbWaitingAcceptance.Checked;
            Filters.DATE_EVENT_FROM = this.txt_dateEventFrom.Text;
            Filters.DATE_EVENT_TO = this.PlaceHolderDateEventTo.Visible ? this.txt_dateEventTo.Text : "";
            Filters.DATE_EXPIRE_FROM = this.txt_expirationDateFrom.Text;
            Filters.DATE_EXPIRE_TO = this.PlaceHolderExpirationDateTo.Visible ? this.txt_expirationDateTo.Text : "";
            Filters.AUTHOR_DESCRIPTION = this.txtDescrizioneCreatore.Text;
            Filters.AUTHOR_SYSTEM_ID = this.txtCodiceCreatore.Text;
            Filters.AUTHOR_TYPE = this.rblOwnerType.SelectedValue;
            Filters.OBJECT = this.TxtObject.Text;
            Filters.TYPE_FILE_ACQUIRED = this.ddlTypeFileAcquired.SelectedValue;
            Filters.NOTES = this.txt_notes.Text;
        }

        private RubricaCallType GetCallType(string idControl)
        {
            RubricaCallType calltype = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
            return calltype;
        }

        private void SearchCorrespondent(string addressCode, string idControl)
        {
            RubricaCallType calltype = GetCallType(idControl);
            Corrispondente corr = UIManager.AddressBookManager.getCorrispondenteRubrica(addressCode, calltype);
            if (corr == null)
            {
                this.txtCodiceCreatore.Text = string.Empty;
                this.txtDescrizioneCreatore.Text = string.Empty;
                this.idCreatore.Value = string.Empty;
                this.upPnlAuthor.Update();
                string msg = "ErrorTransmissionCorrespondentNotFound";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');", true);
            }
            else
            {
                this.txtCodiceCreatore.Text = corr.codiceRubrica;
                this.txtDescrizioneCreatore.Text = corr.descrizione;
                this.idCreatore.Value = corr.systemId;
                this.rblOwnerType.SelectedIndex = -1;
                this.rblOwnerType.Items.FindByValue(corr.tipoCorrispondente).Selected = true;
                this.upPnlAuthor.Update();
            }
        }

        /// <summary>
        /// Popola i campi della maschera sulla base di un filtro precedentemente applicato
        /// </summary>
        private void PopolateFields()
        {
            this.rblFilterDocumentType.SelectedValue = Filters.TYPE_DOCUMENT;

            this.cbDocumentAcquired.Selected = Filters.DOCUMENT_ACQUIRED;

            this.cbDocumentSigned.Selected = Filters.DOCUMENT_SIGNED;

            this.cbDocumentNotSigned.Selected = Filters.DOCUMENT_UNSIGNED;

            this.cbWaitingAcceptance.Checked = Filters.PENDING;

            this.txt_dateEventFrom.Text = Filters.DATE_EVENT_FROM;
            if (!string.IsNullOrEmpty(Filters.DATE_EVENT_TO))
            {
                this.ltlDateEventFrom.Visible = true;
                this.PlaceHolderDateEventTo.Visible = true;
                this.txt_dateEventTo.Text = Filters.DATE_EVENT_TO;
                this.ddl_dateEvent.SelectedValue = INTERVAL;
            }

            this.txt_expirationDateFrom.Text = Filters.DATE_EXPIRE_FROM;
            if (!string.IsNullOrEmpty(Filters.DATE_EXPIRE_TO))
            {
                this.ltlExpirationDateFrom.Visible = true;
                this.PlaceHolderExpirationDateTo.Visible = true;
                this.txt_expirationDateTo.Text = Filters.DATE_EXPIRE_TO;
                this.ddl_expirationDate.SelectedValue = INTERVAL;
            }

            if (!string.IsNullOrEmpty(Filters.AUTHOR_DESCRIPTION))
            {
                txtCodiceCreatore.Text = Filters.AUTHOR_SYSTEM_ID;
                txtDescrizioneCreatore.Text = Filters.AUTHOR_DESCRIPTION;
                this.rblOwnerType.SelectedIndex = -1;
                this.rblOwnerType.Items.FindByValue(Filters.AUTHOR_TYPE).Selected = true;
            }
            this.TxtObject.Text = Filters.OBJECT;
            this.ddlTypeFileAcquired.SelectedValue = Filters.TYPE_FILE_ACQUIRED;
            this.txt_notes.Text = Filters.NOTES;
        }

        #endregion
    }
}
