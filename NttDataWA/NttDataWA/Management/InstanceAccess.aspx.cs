using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.Management
{
    public partial class InstanceAccess : System.Web.UI.Page
    {

        #region Properties

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

        private List<NttDataWA.DocsPaWR.InstanceAccess> ListInstanceAccess
        {
            get
            {
                if (HttpContext.Current.Session["ListInstanceAccess"] != null)
                    return (List<NttDataWA.DocsPaWR.InstanceAccess>)HttpContext.Current.Session["ListInstanceAccess"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ListInstanceAccess"] = value;
            }
        }

        private List<NttDataWA.DocsPaWR.InstanceAccess> ListInstanceAccessFiltered
        {
            get
            {
                if (HttpContext.Current.Session["ListInstanceAccessFiltered"] != null)
                    return (List<NttDataWA.DocsPaWR.InstanceAccess>)HttpContext.Current.Session["ListInstanceAccessFiltered"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ListInstanceAccessFiltered"] = value;
            }
        }


        public string TypeChooseCorrespondent
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["typeChooseCorrespondent"] != null)
                {
                    result = HttpContext.Current.Session["typeChooseCorrespondent"] as string;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["typeChooseCorrespondent"] = value;
            }
        }

        public DocsPaWR.ElementoRubrica[] FoundCorr
        {
            get
            {
                DocsPaWR.ElementoRubrica[] result = null;
                if (HttpContext.Current.Session["foundCorr"] != null)
                {
                    result = HttpContext.Current.Session["foundCorr"] as DocsPaWR.ElementoRubrica[];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["foundCorr"] = value;
            }
        }

        public string TypeRecord
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["typeRecord"] != null)
                {
                    result = HttpContext.Current.Session["typeRecord"] as string;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["typeRecord"] = value;
            }
        }

        public Corrispondente ChooseMultipleCorrespondent
        {
            get
            {
                Corrispondente result = null;
                if (HttpContext.Current.Session["chooseMultipleCorrespondent"] != null)
                {
                    result = HttpContext.Current.Session["chooseMultipleCorrespondent"] as Corrispondente;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["chooseMultipleCorrespondent"] = value;
            }
        }

        private string IdInstance
        {
            get
            {
                string idInstance = null;
                if (HttpContext.Current.Session["IdInstance"] != null)
                {
                    idInstance = HttpContext.Current.Session["IdInstance"] as string;
                }
                return idInstance;
            }
            set
            {
                HttpContext.Current.Session["IdInstance"] = value;
            }
        }


        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializePage();
                this.InitializeLanguage();
                LoadListInstanceAccess();
                //Back
                if (this.Request.QueryString["back"] != null && this.Request.QueryString["back"].Equals("1"))
                {
                    List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                    Navigation.NavigationObject obj = navigationList.Last();
                    this.gridInstanceAccess.PageIndex = Convert.ToInt32(obj.NumPage);
                    if (!string.IsNullOrEmpty(obj.IdInstance))
                        this.IdInstance = obj.IdInstance;
                }
                GridInstanceAccess_Bind();
            }
            else
            {
                ReadRetValueFromPopup();
            }

            this.RefreshScript();
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.ChooseCorrespondent.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ChooseCorrespondent','');", true);
                this.txtDescrizioneRichiedente.Text = this.ChooseMultipleCorrespondent.descrizione;
                this.idRichiedente.Value = this.ChooseMultipleCorrespondent.systemId;
                HttpContext.Current.Session.Remove("chooseMultipleCorrespondent");
                this.upPnlRichiedente.Update();
            }
        }

        private void InitializePage()
        {
            this.txt_initIdDoc_C.ReadOnly = false;
            this.txt_fineIdDoc_C.Visible = false;
            this.LtlAIdDoc.Visible = false;
            this.LtlDaIdDoc.Visible = false;
            this.txt_initDataCreazione_E.ReadOnly = false;
            this.txt_finedataCreazione_E.Visible = false;
            this.LtlADataCreazione.Visible = false;
            this.ClearSessionProperties();
            this.TxtProtoRequest.ReadOnly = true;

            string dataUser = RoleManager.GetRoleInSession().systemId;
            Registro reg = RegistryManager.GetRegistryInSession();
            if (reg == null)
            {
                reg = RoleManager.GetRoleInSession().registri[0];
            }
            dataUser = dataUser + "-" + reg.systemId;
            this.RapidRichiedente.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + "CALLTYPE_CORR_INT_EST_CON_DISABILITATI";
        }

        private void ClearSessionProperties()
        {
            HttpContext.Current.Session.Remove("ListInstanceAccess");
            HttpContext.Current.Session.Remove("ListInstanceAccessFiltered");
            HttpContext.Current.Session.Remove("Instance");
            HttpContext.Current.Session.Remove("IdInstance");
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.LblInstanceAccessTitlePage.Text = Utils.Languages.GetLabelFromCode("LblInstanceAccessTitlePage", language);
            this.InstanceAccessSearch.Text = Utils.Languages.GetLabelFromCode("InstanceAccessSearch", language);
            this.InstanceAccessNew.Text = Utils.Languages.GetLabelFromCode("InstanceAccessNew", language);
            this.ddl_idInstance.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_idDocumento_C0", language);
            this.ddl_idInstance.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_idDocumento_C1", language);
            this.ddl_dataCreazione_E.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_dataCreazione_E.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_dataCreazione_E.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_dataCreazione_E.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_dataCreazione_E.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.LtlIdDoc.Text = Utils.Languages.GetLabelFromCode("LtlidInstance", language);
            this.LtlDataCreazione.Text = Utils.Languages.GetLabelFromCode("LtlDataCreazione", language);
            this.LtlDaIdDoc.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
            this.LtlAIdDoc.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.InstanceRemoveFilter.Text = Utils.Languages.GetLabelFromCode("SearchLabelRemoveFiltersButton", language);
            this.DescriptionInstance.Text = Utils.Languages.GetLabelFromCode("DescriptionInstance", language);
            //this.lit_dtaClose.Text = Utils.Languages.GetLabelFromCode("InstanceAccessDtaClose", language);
            //this.lbl_dtaCloseFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            //this.dtaClose_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            //this.dtaClose_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            //this.dtaClose_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            //this.dtaClose_opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            //this.dtaClose_opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            this.litRichiedente.Text = Utils.Languages.GetLabelFromCode("InstanceAccessLitRichiedente", language);
            this.chkRichiedenteExtendHistoricized.Text = Utils.Languages.GetLabelFromCode("SearchProjectCreatoreExtendHistoricized", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
            this.lit_dtaRequest.Text = Utils.Languages.GetLabelFromCode("InstanceAccessLitRequestDate", language);
            this.dtaRequest_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaRequest_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.dtaRequest_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.dtaRequest_opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.dtaRequest_opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            this.lbl_dtaRequestFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.lbl_dtaRequestTo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.NoteInstance.Text = Utils.Languages.GetLabelFromCode("InstanceAccessNote", language);
            this.litProtoRequest.Text = Utils.Languages.GetLabelFromCode("InstanceAccessProtoRequest", language);
            this.OpenAddDocCustom.Title = Utils.Languages.GetLabelFromCode("OpenAddDocTitleCustom", language);
            this.lblElencoIstanze.Text = Utils.Languages.GetLabelFromCode("InstanceAccessElencoIstanze", language);
            this.litEstremiProcedimento.Text = Utils.Languages.GetLabelFromCode("InstanceAccessLitEstremiProcedimento", language);
            this.ImgRichiedenteAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgAnswerAddressBook", language);
            this.InstanceAccessCercaProto.ToolTip = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Cerca", language);
            this.InstanceAccessResetProtoRequest.ToolTip = Utils.Languages.GetLabelFromCode("InstanceLinkDocFascBtn_Reset", language);
            this.ChooseCorrespondent.Title = Utils.Languages.GetLabelFromCode("InstanceAccessChooseCorrespondent", language);
            
        }

        protected void InstanceAccessSearch_Click(object o, EventArgs e)
        {
            try
            {
                if (SearchInstances())
                {
                    this.gridInstanceAccess.PageIndex = 0;
                    GridInstanceAccess_Bind();
                    this.UpnlGridInstanceAccess.Update();
                }
                else
                {
                    return;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void InstanceAccessNew_Click(object o, EventArgs e)
        {
            List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
            Navigation.NavigationObject pre = navigationList.Last();
            Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
            if (pre.CodePage != Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_ACCESS.ToString())
            {
                actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_ACCESS.ToString(), string.Empty);
                actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_ACCESS.ToString(), true,this.Page);
                actualPage.CodePage = Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_ACCESS.ToString();
                actualPage.Page = "INSTANCEACCESS.ASPX";
                navigationList.Add(actualPage);
                Navigation.NavigationUtils.SetNavigationList(navigationList);
            }

            Response.Redirect("../Management/InstanceDetails.aspx?t=n");

        }

        protected void InstanceRemoveFilter_Click(object o, EventArgs e)
        {
            try
            {

                ListInstanceAccessFiltered = new List<DocsPaWR.InstanceAccess>();
                ListInstanceAccessFiltered.AddRange(ListInstanceAccess);
                this.ddl_idInstance.SelectedIndex = 0;
                this.txt_initIdDoc_C.ReadOnly = false;
                this.txt_fineIdDoc_C.Visible = false;
                this.LtlAIdDoc.Visible = false;
                this.LtlDaIdDoc.Visible = false;
                this.txt_fineIdDoc_C.Text = string.Empty;
                this.txt_initIdDoc_C.Text = string.Empty;
                this.txt_fineIdDoc_C.Text = string.Empty;
                this.TxtObject.Text = string.Empty;
                this.ddl_dataCreazione_E.SelectedIndex = 0;
                this.txt_initDataCreazione_E.ReadOnly = false;
                this.txt_finedataCreazione_E.Visible = false;
                this.LtlADataCreazione.Visible = false;
                this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", UserManager.GetUserLanguage());
                this.txt_initDataCreazione_E.Text = string.Empty;
                this.txt_finedataCreazione_E.Text = string.Empty;
                //this.ddl_dtaClose.SelectedIndex = 0;
                //this.dtaClose_TxtFrom.Text = string.Empty;
                //this.dtaClose_TxtTo.Text = string.Empty;
                this.txtCodiceRichiedente.Text = string.Empty;
                this.txtDescrizioneRichiedente.Text = string.Empty;
                this.idRichiedente.Value = string.Empty;
                this.ddl_dtaRequest.SelectedIndex = 0;
                this.dtaRequest_TxtFrom.ReadOnly = false;
                this.dtaRequest_TxtTo.Visible = false;
                this.lbl_dtaRequestTo.Visible = false;
                this.lbl_dtaRequestFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", UserManager.GetUserLanguage());
                this.dtaRequest_TxtFrom.Text = string.Empty;
                this.dtaRequest_TxtTo.Text = string.Empty;
                this.TxtProtoRequest.Text = string.Empty;
                this.idProtoRequest.Value = string.Empty;
                this.TxtNote.Text = string.Empty;
                this.chkRichiedenteExtendHistoricized.Checked = true;
                this.gridInstanceAccess.PageIndex = 0;
                GridInstanceAccess_Bind();
                this.UpPnlIdInstance.Update();
                this.UpPnlDescription.Update();
                this.UpdateCreationDate.Update();
                //this.UpdateCloseDatePanel.Update();
                this.upPnlRichiedente.Update();
                this.UpdateRequestDatePanel.Update();
                this.UpdPnlProtoRequest.Update();
                this.UpdPnlNote.Update();
                this.UpnlGridInstanceAccess.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ImgRichiedenteAddressBook_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.chkRichiedenteExtendHistoricized.Checked)
                {
                    this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                }
                else
                {
                    this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST;
                }
                HttpContext.Current.Session["AddressBook.from"] = "F_X_X_S";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AddressBook", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.txtCodiceRichiedente.Text))
                {
                    RubricaCallType calltype = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
                    if (this.chkRichiedenteExtendHistoricized.Checked)
                    {
                        calltype = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                    }
                    else
                    {
                        calltype = RubricaCallType.CALLTYPE_CORR_INT_EST;
                    }
                    ElementoRubrica[] listaCorr = null;
                    Corrispondente corr = null;
                    bool multiCorr = false;
                    UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
                    listaCorr = UIManager.AddressBookManager.getElementiRubricaMultipli(txtCodiceRichiedente.Text, calltype, true);
                    if (listaCorr != null && (listaCorr.Count() == 1 || listaCorr.Count() == 0))
                    {
                        if (listaCorr.Count() == 1)
                        {
                            corr = UIManager.AddressBookManager.getCorrispondenteRubrica(this.txtCodiceRichiedente.Text, calltype);
                        }
                        if (corr == null)
                        {
                            this.txtCodiceRichiedente.Text = string.Empty;
                            this.txtDescrizioneRichiedente.Text = string.Empty;
                            this.idRichiedente.Value = string.Empty;
                            this.upPnlRichiedente.Update();
                            string msg = "ErrorTransmissionCorrespondentNotFound";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        else
                        {
                            this.txtCodiceRichiedente.Text = corr.codiceRubrica;
                            this.txtDescrizioneRichiedente.Text = corr.descrizione;
                            this.idRichiedente.Value = corr.systemId;
                            this.upPnlRichiedente.Update();
                        }
                    }
                    else
                    {
                        corr = null;
                        multiCorr = true;
                        this.FoundCorr = listaCorr;
                        this.TypeChooseCorrespondent = "Sender";
                        this.TypeRecord = "A";
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chooseCorrespondent", "ajaxModalPopupChooseCorrespondent();", true);
                    }
                }
                else
                {
                    this.txtCodiceRichiedente.Text = string.Empty;
                    this.txtDescrizioneRichiedente.Text = string.Empty;
                    this.idRichiedente.Value = string.Empty;
                    this.upPnlRichiedente.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void chkRichiedenteExtendHistoricized_Click(object sender, EventArgs e)
        {
            string dataUser = RoleManager.GetRoleInSession().systemId;
            Registro reg = RegistryManager.GetRegistryInSession();
            if (reg == null)
            {
                reg = RoleManager.GetRoleInSession().registri[0];
            }
            dataUser = dataUser + "-" + reg.systemId;
            string callType = string.Empty;

            if (this.chkRichiedenteExtendHistoricized.Checked)
            {
                callType = "CALLTYPE_CORR_INT_EST_CON_DISABILITATI";
            }
            else
            {
                callType = "CALLTYPE_CORR_INT_EST";
            }
            this.RapidRichiedente.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;

            this.upPnlRichiedente.Update();
        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try
            {
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> ccList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.Cc"];
                string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();
               
                if (atList != null && atList.Count > 0)
                {
                    NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                    Corrispondente tempCorrSingle;
                    if (!corrInSess.isRubricaComune)
                        tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                    else
                        tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                    this.txtCodiceRichiedente.Text = tempCorrSingle.codiceRubrica;
                    this.txtDescrizioneRichiedente.Text = tempCorrSingle.descrizione;
                    this.idRichiedente.Value = tempCorrSingle.systemId;
                    this.upPnlRichiedente.Update();
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
        /*
        protected void ddl_dtaClose_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dtaClose.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.dtaClose_TxtFrom.ReadOnly = false;
                        this.dtaClose_TxtTo.Visible = false;
                        this.lbl_dtaCloseTo.Visible = false;
                        this.lbl_dtaCloseFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.dtaClose_TxtFrom.ReadOnly = false;
                        this.dtaClose_TxtTo.ReadOnly = false;
                        this.lbl_dtaCloseTo.Visible = true;
                        this.lbl_dtaCloseFrom.Visible = true;
                        this.dtaClose_TxtTo.Visible = true;
                        this.lbl_dtaCloseFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaCloseTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 2: //Oggi
                        this.lbl_dtaCloseTo.Visible = false;
                        this.dtaClose_TxtTo.Visible = false;
                        this.dtaClose_TxtFrom.ReadOnly = true;
                        this.dtaClose_TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                        break;
                    case 3: //Settimana corrente
                        this.lbl_dtaCloseTo.Visible = true;
                        this.dtaClose_TxtTo.Visible = true;
                        this.dtaClose_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.dtaClose_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.dtaClose_TxtTo.ReadOnly = true;
                        this.dtaClose_TxtFrom.ReadOnly = true;
                        this.lbl_dtaCloseFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaCloseTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 4: //Mese corrente
                        this.lbl_dtaCloseTo.Visible = true;
                        this.dtaClose_TxtTo.Visible = true;
                        this.dtaClose_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.dtaClose_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.dtaClose_TxtTo.ReadOnly = true;
                        this.dtaClose_TxtFrom.ReadOnly = true;
                        this.lbl_dtaCloseFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaCloseTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                }

                this.UpdateCloseDatePanel.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        */
        protected void ddl_idInstance_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (this.ddl_idInstance.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initIdDoc_C.ReadOnly = false;
                        this.txt_fineIdDoc_C.Visible = false;
                        this.LtlAIdDoc.Visible = false;
                        this.LtlDaIdDoc.Visible = false;
                        this.txt_fineIdDoc_C.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initIdDoc_C.ReadOnly = false;
                        this.txt_fineIdDoc_C.ReadOnly = false;
                        this.LtlAIdDoc.Visible = true;
                        this.LtlDaIdDoc.Visible = true;
                        this.txt_fineIdDoc_C.Visible = true;
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dataCreazione_E_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.txt_initDataCreazione_E.Text = string.Empty;
                this.txt_finedataCreazione_E.Text = string.Empty;

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dataCreazione_E.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initDataCreazione_E.ReadOnly = false;
                        this.txt_finedataCreazione_E.Visible = false;
                        this.LtlADataCreazione.Visible = false;
                        this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.txt_initDataCreazione_E.ReadOnly = false;
                        this.txt_finedataCreazione_E.ReadOnly = false;
                        this.LtlADataCreazione.Visible = true;
                        this.LtlDaDataCreazione.Visible = true;
                        this.txt_finedataCreazione_E.Visible = true;
                        this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataCreazione.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADataCreazione.Visible = false;
                        this.txt_finedataCreazione_E.Visible = false;
                        this.txt_initDataCreazione_E.ReadOnly = true;
                        this.txt_initDataCreazione_E.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 3: //Settimana corrente
                        this.LtlADataCreazione.Visible = true;
                        this.txt_finedataCreazione_E.Visible = true;
                        this.txt_initDataCreazione_E.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txt_finedataCreazione_E.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txt_finedataCreazione_E.ReadOnly = true;
                        this.txt_initDataCreazione_E.ReadOnly = true;
                        this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataCreazione.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADataCreazione.Visible = true;
                        this.txt_finedataCreazione_E.Visible = true;
                        this.txt_initDataCreazione_E.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txt_finedataCreazione_E.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txt_finedataCreazione_E.ReadOnly = true;
                        this.txt_initDataCreazione_E.ReadOnly = true;
                        this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataCreazione.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
                this.UpdateCreationDate.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dtaRequest_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.dtaRequest_TxtFrom.Text = string.Empty;
                this.dtaRequest_TxtTo.Text = string.Empty;

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dtaRequest.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.dtaRequest_TxtFrom.ReadOnly = false;
                        this.dtaRequest_TxtTo.Visible = false;
                        this.lbl_dtaRequestTo.Visible = false;
                        this.lbl_dtaRequestFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.dtaRequest_TxtFrom.ReadOnly = false;
                        this.dtaRequest_TxtTo.ReadOnly = false;
                        this.lbl_dtaRequestTo.Visible = true;
                        this.lbl_dtaRequestFrom.Visible = true;
                        this.dtaRequest_TxtTo.Visible = true;
                        this.lbl_dtaRequestFrom.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.lbl_dtaRequestTo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.lbl_dtaRequestTo.Visible = false;
                        this.dtaRequest_TxtTo.Visible = false;
                        this.dtaRequest_TxtFrom.ReadOnly = true;
                        this.dtaRequest_TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                        this.lbl_dtaRequestFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 3: //Settimana corrente
                        this.lbl_dtaRequestTo.Visible = true;
                        this.dtaRequest_TxtTo.Visible = true;
                        this.dtaRequest_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.dtaRequest_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.dtaRequest_TxtTo.ReadOnly = true;
                        this.dtaRequest_TxtFrom.ReadOnly = true;
                        this.lbl_dtaRequestFrom.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.lbl_dtaRequestTo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.lbl_dtaRequestTo.Visible = true;
                        this.dtaRequest_TxtTo.Visible = true;
                        this.dtaRequest_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.dtaRequest_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.dtaRequest_TxtTo.ReadOnly = true;
                        this.dtaRequest_TxtFrom.ReadOnly = true;
                        this.lbl_dtaRequestFrom.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.lbl_dtaRequestTo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
                this.UpdateRequestDatePanel.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InstanceAccessCercaProto_Click(object sender, EventArgs e)
        {
            try
            {
                HttpContext.Current.Session["LinkCustom.type"] = this.ID;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenAddDocCustom", "ajaxModalPopupOpenAddDocCustom();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnAddDocPostback_Click(object sender, EventArgs e)
        {
            try
            {
                if (HttpContext.Current.Session["LinkCustom.return"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["LinkCustom.return"].ToString()))
                {
                    InfoDocumento infoDoc = DocumentManager.GetInfoDocumento(HttpContext.Current.Session["LinkCustom.return"].ToString(), HttpContext.Current.Session["LinkCustom.return"].ToString(), this.Page);
                    if (infoDoc != null && !string.IsNullOrEmpty(infoDoc.segnatura))
                    {
                        this.TxtProtoRequest.Text = infoDoc.segnatura + " " + CutValue(infoDoc.oggetto);
                    }
                    else
                    {
                        this.TxtProtoRequest.Text = infoDoc.idProfile + " " + CutValue(infoDoc.oggetto);
                    }
                    this.idProtoRequest.Value = infoDoc.docNumber;
                    HttpContext.Current.Session["LinkCustom.return"] = null;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenAddDocCustom','');", true);
                    this.UpdPnlProtoRequest.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InstanceAccessResetProtoRequest_Click(object sender, EventArgs e)
        {
            this.TxtProtoRequest.Text = string.Empty;
            this.idProtoRequest.Value = string.Empty;
            this.UpdPnlProtoRequest.Update();
        }

        private string CutValue(string value)
        {
            if (value.Length < 20) return value;
            int firstSpacePos = value.IndexOf(' ', 20);
            if (firstSpacePos == -1) firstSpacePos = 20;
            return value.Substring(0, firstSpacePos) + "...";
        }

        private void LoadListInstanceAccess()
        {
            ListInstanceAccess = InstanceAccessManager.GetInstanceAccess(UserManager.GetUserInSession().idPeople, RoleManager.GetRoleInSession().idGruppo);
            ListInstanceAccessFiltered = new List<DocsPaWR.InstanceAccess>();
            ListInstanceAccessFiltered.AddRange(ListInstanceAccess);
        }

        #region Management Grid

        private void GridInstanceAccess_Bind()
        {
            this.lblNumeroIstanze.Text = Utils.Languages.GetLabelFromCode("InstanceAccessNumeroIstanze", UserManager.GetUserLanguage()).Replace("@@", ListInstanceAccessFiltered.Count.ToString());
            this.UpnlNumeroIstanze.Update();
            this.gridInstanceAccess.DataSource = ListInstanceAccessFiltered;
            this.gridInstanceAccess.DataBind();
        }

        protected void GridInstanceAccess_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.gridInstanceAccess.PageIndex = e.NewPageIndex;
                GridInstanceAccess_Bind();
                this.UpnlGridInstanceAccess.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GridInstanceAccess_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes["onClick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "');$('#btnGridInstanceAccessRow').click();return false;";
                    if (this.IdInstance != null && (e.Row.FindControl("lblIdInstanceAccess") as Label).Text.Equals(this.IdInstance))
                    {
                        e.Row.Attributes.Remove("class");
                        e.Row.CssClass = "selectedrow";
                        HttpContext.Current.Session.Remove("IdInstance");
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected string GetLabelCreationDate(NttDataWA.DocsPaWR.InstanceAccess instance)
        {
            if (instance.CREATION_DATE.Equals(DateTime.MinValue))
                return "";
            else
                return instance.CREATION_DATE.ToShortDateString().ToString();
        }

        protected string GetLabelCloseDate(NttDataWA.DocsPaWR.InstanceAccess instance)
        {
            if (instance.CLOSE_DATE.Equals(DateTime.MinValue))
                return "";
            else
                return instance.CLOSE_DATE.ToShortDateString().ToString();
        }

        protected string GetLabelRequestDate(NttDataWA.DocsPaWR.InstanceAccess instance)
        {
            if (instance.REQUEST_DATE.Equals(DateTime.MinValue))
                return "";
            else
                return instance.REQUEST_DATE.ToShortDateString().ToString();
        }

        protected void btnGridInstanceAccessRow_Click(object sender, EventArgs e)
        {
            this.IdInstance = (this.gridInstanceAccess.Rows[Convert.ToInt32(this.grid_rowindex.Value)].FindControl("lblIdInstanceAccess") as Label).Text;
            //try
            //{
            //    DocsPaWR.InstanceAccess instance = InstanceAccessManager.GetInstanceAccessById(idInstance);
            //    InstanceAccessManager.setInstanceAccessInSession(instance);
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
            List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
            Navigation.NavigationObject pre = navigationList.Last();
            Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
            if (pre.CodePage == Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_ACCESS.ToString())
            {
                navigationList.RemoveAll(n => n.CodePage.Equals(Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_ACCESS.ToString()));
            }
            actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_ACCESS.ToString(), string.Empty);
            actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_ACCESS.ToString(), true, this.Page);
            actualPage.CodePage = Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_ACCESS.ToString();
            actualPage.Page = "INSTANCEACCESS.ASPX";
            navigationList.Add(actualPage);
            actualPage.IdInstance = this.IdInstance;
            actualPage.NumPage = this.gridInstanceAccess.PageIndex.ToString();
            Navigation.NavigationUtils.SetNavigationList(navigationList);
            Response.Redirect("../Management/InstanceDetails.aspx?t=d");
        }

        #endregion

        #region SearchInstances

        public bool SearchInstances()
        {
            ListInstanceAccessFiltered = new List<DocsPaWR.InstanceAccess>();
            ListInstanceAccessFiltered.AddRange(ListInstanceAccess);
            List<DocsPaWR.InstanceAccess> listInstanceAccessSupport = new List<DocsPaWR.InstanceAccess>();
            #region ID INSTANCE

            if (ddl_idInstance.SelectedValue.Equals("0") && !string.IsNullOrEmpty(this.txt_initIdDoc_C.Text))
            {
                listInstanceAccessSupport = (from instance in ListInstanceAccessFiltered
                                             where instance.ID_INSTANCE_ACCESS.Equals(this.txt_initIdDoc_C.Text) 
                                             select instance).ToList();
                ListInstanceAccessFiltered.Clear();
                ListInstanceAccessFiltered.AddRange(listInstanceAccessSupport);
            }

            else if (ddl_idInstance.SelectedValue.Equals("1"))
            {
                if (!string.IsNullOrEmpty(this.txt_initIdDoc_C.Text) && !string.IsNullOrEmpty(this.txt_fineIdDoc_C.Text))
                {
                    if (Convert.ToInt32(this.txt_fineIdDoc_C.Text) >= Convert.ToInt32(this.txt_initIdDoc_C.Text))
                    {
                        listInstanceAccessSupport = (from instance in ListInstanceAccess
                                                     where Convert.ToInt32(instance.ID_INSTANCE_ACCESS) >= Convert.ToInt32(this.txt_initIdDoc_C.Text)
                                                     && Convert.ToInt32(instance.ID_INSTANCE_ACCESS) <= Convert.ToInt32(this.txt_fineIdDoc_C.Text)
                                                     select instance).ToList();
                        ListInstanceAccessFiltered.Clear();
                        ListInstanceAccessFiltered.AddRange(listInstanceAccessSupport);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorInstanceAccessIntervalInstanceId', 'warning', '');", true);
                        return false;
                    }
                }
            }
            #endregion

            #region DESCRIZIONE

            if (!string.IsNullOrEmpty(this.TxtObject.Text))
            {
                listInstanceAccessSupport = (from instance in ListInstanceAccessFiltered
                                             where instance.DESCRIPTION.ToLower().Contains(this.TxtObject.Text.ToLower())
                                             select instance).ToList();
                ListInstanceAccessFiltered.Clear();
                ListInstanceAccessFiltered.AddRange(listInstanceAccessSupport);
            }

            #endregion

            #region DATA CREAZIONE

            if (ddl_dataCreazione_E.SelectedValue.Equals("0") && !string.IsNullOrEmpty(this.txt_initDataCreazione_E.Text))
            {
                listInstanceAccessSupport = (from instance in ListInstanceAccessFiltered
                                             where Utils.utils.formatDataDocsPa(instance.CREATION_DATE).Equals(this.txt_initDataCreazione_E.Text)
                                             select instance).ToList();
                ListInstanceAccessFiltered.Clear();
                ListInstanceAccessFiltered.AddRange(listInstanceAccessSupport);
            }
            else if (!string.IsNullOrEmpty(this.txt_initDataCreazione_E.Text) && !string.IsNullOrEmpty(this.txt_finedataCreazione_E.Text))
            {
                if (!Utils.utils.verificaIntervalloDate(txt_initDataCreazione_E.Text, txt_finedataCreazione_E.Text))
                {
                    listInstanceAccessSupport = (from instance in ListInstanceAccessFiltered
                                                    where Utils.utils.verificaIntervalloDateSenzaOra(Utils.utils.formatDataDocsPa(instance.CREATION_DATE), this.txt_initDataCreazione_E.Text)
                                                    && Utils.utils.verificaIntervalloDateSenzaOra(this.txt_finedataCreazione_E.Text, Utils.utils.formatDataDocsPa(instance.CREATION_DATE))
                                                    select instance).ToList();
                    ListInstanceAccessFiltered.Clear();
                    ListInstanceAccessFiltered.AddRange(listInstanceAccessSupport);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorInstanceAccessIntervalCreationDate', 'warning', '');", true);
                    return false;
                }
            }
            #endregion

            #region DATA CHIUSURA
            /*
            if (this.ddl_dtaClose.SelectedValue.Equals("0") && !string.IsNullOrEmpty(this.dtaClose_TxtFrom.Text))
            {
                listInstanceAccessSupport = (from instance in ListInstanceAccessFiltered
                                             where Utils.utils.formatDataDocsPa(instance.CLOSE_DATE).Equals(this.dtaClose_TxtFrom.Text)
                                             select instance).ToList();
                ListInstanceAccessFiltered.Clear();
                ListInstanceAccessFiltered.AddRange(listInstanceAccessSupport);
            }
            else if (!string.IsNullOrEmpty(this.dtaClose_TxtFrom.Text) && !string.IsNullOrEmpty(this.dtaClose_TxtTo.Text))
            {
                if (!Utils.utils.verificaIntervalloDate(dtaClose_TxtFrom.Text, dtaClose_TxtTo.Text))
                {
                    listInstanceAccessSupport = (from instance in ListInstanceAccessFiltered
                                                 where !instance.CLOSE_DATE.Equals(DateTime.MinValue) && Utils.utils.verificaIntervalloDateSenzaOra(Utils.utils.formatDataDocsPa(instance.CLOSE_DATE), this.dtaClose_TxtFrom.Text)
                                                 && Utils.utils.verificaIntervalloDateSenzaOra(this.dtaClose_TxtTo.Text, Utils.utils.formatDataDocsPa(instance.CLOSE_DATE))
                                                 select instance).ToList();
                    ListInstanceAccessFiltered.Clear();
                    ListInstanceAccessFiltered.AddRange(listInstanceAccessSupport);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorInstanceAccessIntervalCloseDate', 'warning', '');", true);
                    return false;
                }
            }
            */
            #endregion

            #region RICHIEDENTE

            if (!string.IsNullOrEmpty(this.txtCodiceRichiedente.Text))
            {
                listInstanceAccessSupport = (from instance in ListInstanceAccessFiltered
                                             where instance.RICHIEDENTE != null && instance.RICHIEDENTE.systemId.Equals(this.idRichiedente.Value)
                                             select instance).ToList();

                ListInstanceAccessFiltered.Clear();
                ListInstanceAccessFiltered.AddRange(listInstanceAccessSupport);
            }
            else if (!string.IsNullOrEmpty(this.txtDescrizioneRichiedente.Text))
            {
                listInstanceAccessSupport = (from instance in ListInstanceAccessFiltered
                                             where instance.RICHIEDENTE != null && instance.RICHIEDENTE.descrizione.ToLower().Contains(this.txtDescrizioneRichiedente.Text.ToLower())
                                             select instance).ToList();

                ListInstanceAccessFiltered.Clear();
                ListInstanceAccessFiltered.AddRange(listInstanceAccessSupport);
            }

            #endregion

            #region DATA RICHIESTA

            if (ddl_dtaRequest.SelectedValue.Equals("0") && !string.IsNullOrEmpty(this.dtaRequest_TxtFrom.Text))
            {
                listInstanceAccessSupport = (from instance in ListInstanceAccessFiltered
                                             where Utils.utils.formatDataDocsPa(instance.REQUEST_DATE).Equals(this.dtaRequest_TxtFrom.Text)
                                             select instance).ToList();
                ListInstanceAccessFiltered.Clear();
                ListInstanceAccessFiltered.AddRange(listInstanceAccessSupport);
            }
            else if (!string.IsNullOrEmpty(this.dtaRequest_TxtFrom.Text) && !string.IsNullOrEmpty(this.dtaRequest_TxtTo.Text))
            {
                if (!Utils.utils.verificaIntervalloDate(dtaRequest_TxtFrom.Text, dtaRequest_TxtTo.Text))
                {
                    listInstanceAccessSupport = (from instance in ListInstanceAccessFiltered
                                                 where !instance.REQUEST_DATE.Equals(DateTime.MinValue) && Utils.utils.verificaIntervalloDateSenzaOra(Utils.utils.formatDataDocsPa(instance.REQUEST_DATE), this.dtaRequest_TxtFrom.Text)
                                                 && Utils.utils.verificaIntervalloDateSenzaOra(this.dtaRequest_TxtTo.Text, Utils.utils.formatDataDocsPa(instance.REQUEST_DATE))
                                                 select instance).ToList();
                    ListInstanceAccessFiltered.Clear();
                    ListInstanceAccessFiltered.AddRange(listInstanceAccessSupport);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorInstanceAccessIntervalRequestDate', 'warning', '');", true);
                    return false;
                }
            }

            #endregion

            #region PROTOCOLLO RICHIESTA

            if (!string.IsNullOrEmpty(this.TxtProtoRequest.Text))
            {
                listInstanceAccessSupport = (from instance in ListInstanceAccessFiltered
                                             where instance.ID_DOCUMENT_REQUEST.Equals(this.idProtoRequest.Value)
                                             select instance).ToList();

                ListInstanceAccessFiltered.Clear();
                ListInstanceAccessFiltered.AddRange(listInstanceAccessSupport);
            }

            #endregion
            #region NOTE

            if (!string.IsNullOrEmpty(this.TxtNote.Text))
            {
                listInstanceAccessSupport = (from instance in ListInstanceAccessFiltered
                                             where instance.NOTE.ToLower().Contains(this.TxtNote.Text.ToLower())
                                             select instance).ToList();
                ListInstanceAccessFiltered.Clear();
                ListInstanceAccessFiltered.AddRange(listInstanceAccessSupport);
            }

            #endregion
            return true;
        }



        #endregion
    }
}
