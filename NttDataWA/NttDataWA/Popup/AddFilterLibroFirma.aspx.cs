using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class AddFilterLibroFirma : System.Web.UI.Page
    {

        #region Properties

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

        private List<FiltroElementiLibroFirma> FiltersElement
        {
            get
            {
                return (List<FiltroElementiLibroFirma>)HttpContext.Current.Session["FiltersElement"];
            }
            set
            {
                HttpContext.Current.Session["FiltersElement"] = value;
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

        private List<ElementoInLibroFirma> ListaElementiLibroFirma
        {
            get
            {
                if (HttpContext.Current.Session["ListaElementiLibroFirma"] != null)
                    return (List<ElementoInLibroFirma>)HttpContext.Current.Session["ListaElementiLibroFirma"];
                else
                    return null;
            }
        }

        private List<ElementoInLibroFirma> ListaElementiFiltrati
        {
            get
            {
                List<ElementoInLibroFirma> result = null;
                if (HttpContext.Current.Session["ListaElementiFiltrati"] != null)
                {
                    result = HttpContext.Current.Session["ListaElementiFiltrati"] as List<ElementoInLibroFirma>;
                }
                return result;
            }

            set
            {
                HttpContext.Current.Session["ListaElementiFiltrati"] = value;
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

        #endregion

        #region Const

        private const string UPDATE_PANEL_BUTTONS = "UpPnlButtons";
        private const string CLOSE_POPUP_OBJECT = "closePopupObject";

        #endregion

        #region Standard method
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.InitializeLanguage();
                this.ClearSession();
                this.InitializePage();
            }
            RefreshScript();
        }

        private void ClearSession()
        {
            HttpContext.Current.Session.Remove("ReturnValuePopup");
            this.TypeDocument = "Search";
            UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
        }

        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.AddFilterBtnConfirm.Text = Utils.Languages.GetLabelFromCode("AddFilterLbroFirmaConfirm", language);
            this.AddFilterBtnCancel.Text = Utils.Languages.GetLabelFromCode("AddFilterLibroFirmaCancel", language);
            this.LtlTypeDocument.Text = Utils.Languages.GetLabelFromCode("SearchDocumentTypeDocument", language);
            this.DocumentLitObject.Text = Utils.Languages.GetLabelFromCode("DocumentLitObject", language);
            this.LtlIdProto.Text = Utils.Languages.GetLabelFromCode("AddFilterInstanceAccessLtIdProto", language);
            this.ddl_idProto.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E0", language);
            this.ddl_idProto.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E1", language);
            this.LtlDaIdProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
            this.LtlAIdProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlDataProto.Text = Utils.Languages.GetLabelFromCode("LtlDataProto", language);
            this.litDest.Text = Utils.Languages.GetLabelFromCode("litDest", language);
            this.LtlDaDataProto.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.LtlADataProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.chkDestExtendHistoricized.Text = Utils.Languages.GetLabelFromCode("SearchProjectCreatoreExtendHistoricized", language);
            this.ddl_dataProt_E.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_dataProt_E.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_dataProt_E.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_dataProt_E.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_dataProt_E.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.LtlIdDoc.Text = Utils.Languages.GetLabelFromCode("LtlIdDoc", language);
            this.ddl_idDoc.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E0", language);
            this.ddl_idDoc.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E1", language);
            this.LtlDaIdDoc.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
            this.LtlAIdDoc.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlDataCreazione.Text = Utils.Languages.GetLabelFromCode("LtlDataCreazione", language);
            this.ddl_dataCreazione_E.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_dataCreazione_E.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_dataCreazione_E.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_dataCreazione_E.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_dataCreazione_E.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.SearchDocumentLitTypology.Text = Utils.Languages.GetLabelFromCode("SearchDocumentLitTypology", language);
            this.DocumentDdlTypeDocument.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DocumentDdlTypeDocument", language));
            this.LtlTypeSignature.Text = Utils.Languages.GetLabelFromCode("LtlTypeSignature", language);
            this.opDC.Text = Utils.Languages.GetLabelFromCode("cbxTypeSignatureDC", language);
            this.opDP.Text = Utils.Languages.GetLabelFromCode("cbxTypeSignatureDP", language);
            this.opES.Text = Utils.Languages.GetLabelFromCode("cbxTypeSignatureES", language);
            this.opEA.Text = Utils.Languages.GetLabelFromCode("cbxTypeSignatureEA", language);
            this.LtlMode.Text = Utils.Languages.GetLabelFromCode("AddFilterLibroFirmaLtlMode", language);
            this.CbxModeOpA.Text = Utils.Languages.GetLabelFromCode("AddFilterLibroFirmaCbxModeOpA", language);
            this.CbxModeOpM.Text = Utils.Languages.GetLabelFromCode("AddFilterLibroFirmaCbxModeOpM", language);
            this.cbxOpP.Text = Utils.Languages.GetLabelFromCode("cbxStateProposed", language);
            this.cbxOpF.Text = Utils.Languages.GetLabelFromCode("cbxStateToSign", language);
            this.cbxOpE.Text = Utils.Languages.GetLabelFromCode("cbxStateWithError", language);
            this.cbxOpSE.Text = Utils.Languages.GetLabelFromCode("cbxStateWithoutError", language);
            this.cbxOpR.Text = Utils.Languages.GetLabelFromCode("cbxStateToReject", language);
            this.LtlState.Text = Utils.Languages.GetLabelFromCode("LtlStateAddFilterLibroFirma", language);
            this.LtlErrorState.Text = Utils.Languages.GetLabelFromCode("LtlErrorStateAddFilterLibroFirma", language);
            this.ltlNote.Text = Utils.Languages.GetLabelFromCode("AddFilterLibroFirmaLtlNote", language);
            this.ltlDataInserimento.Text = Utils.Languages.GetLabelFromCode("AddFilterLibroFirmaLtlDataInserimento", language);
            this.ddl_DataInserimento.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_DataInserimento.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_DataInserimento.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_DataInserimento.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_DataInserimento.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.ltlDaDataInserimento.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.ltlADataInserimento.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.SearchDocumentLit.Text = Utils.Languages.GetLabelFromCode("SearchDocumentLit", language);
            this.protocolloLit.Text = Utils.Languages.GetLabelFromCode("AddFilterLibroFirmaProtocollo", language);
            this.ltlProponente.Text = Utils.Languages.GetLabelFromCode("AddFilterLibroFirmaLtlProponente", language);
            this.chkProponenteExtendHistoricized.Text = Utils.Languages.GetLabelFromCode("SearchProjectCreatoreExtendHistoricized", language);
            if ((DocumentManager.GetDescriptionLabel("A")).Length > 3)
            {
                this.opArr.Text = ((DocumentManager.GetDescriptionLabel("A")).Substring(0, 3)) + "."; //Valore A
            }
            else
            {
                this.opArr.Text = DocumentManager.GetDescriptionLabel("A");
            }

            if ((DocumentManager.GetDescriptionLabel("P")).Length > 3)
            {
                //CASO PER INFORMATICA TRENTINA PER LASCIARE 4 CARATTERI (Part.)
                if (DocumentManager.GetDescriptionLabel("P").Equals("Partenza"))
                {
                    this.opPart.Text = "Part.";
                }
                else
                {
                    this.opPart.Text = ((DocumentManager.GetDescriptionLabel("P")).Substring(0, 3)) + "."; //Valore P
                }
            }
            else
            {
                this.opPart.Text = DocumentManager.GetDescriptionLabel("P");
            }

            if (DocumentManager.GetDescriptionLabel("I").Length > 3)
            {
                this.opInt.Text = ((DocumentManager.GetDescriptionLabel("I")).Substring(0, 3)) + ".";//Valore I
            }
            else
            {
                this.opInt.Text = DocumentManager.GetDescriptionLabel("I");
            }
            if (DocumentManager.GetDescriptionLabel("G").Length > 3)
            {
                this.opGrigio.Text = (DocumentManager.GetDescriptionLabel("G").Substring(0, 3)) + ".";//Valore G
            }
            else
            {
                this.opGrigio.Text = DocumentManager.GetDescriptionLabel("G");
            }
            if (DocumentManager.GetDescriptionLabel("ALL").Length > 3)
            {
                this.opAll.Text = (DocumentManager.GetDescriptionLabel("ALL").Substring(0, 3)) + ".";//Valore ALL
            }
            else
            {
                this.opAll.Text = DocumentManager.GetDescriptionLabel("ALL");
            }
        }

        private void InitializePage()
        {
            this.txt_initIdProto.ReadOnly = false;
            this.txt_fineIdProto.Visible = false;
            this.LtlAIdProto.Visible = false;
            this.LtlDaIdProto.Visible = false;
            this.txt_initDataProt_E.ReadOnly = false;
            this.txt_fineDataProt_E.Visible = false;
            this.LtlADataProto.Visible = false;
            this.txt_initIdDoc.ReadOnly = false;
            this.txt_fineIdDoc.Visible = false;
            this.LtlAIdDoc.Visible = false;
            this.LtlDaIdDoc.Visible = false;
            this.txt_initDataCreazione_E.ReadOnly = false;
            this.txt_finedataCreazione_E.Visible = false;
            this.LtlADataCreazione.Visible = false;
            this.txt_Da_dataInserimento.ReadOnly = false;
            this.txt_A_dataInserimento.Visible = false;
            this.ltlADataInserimento.Visible = false;

            this.LoadTypeDocuments();

            if (this.FiltersElement != null)
            {
                BindFilterValues();
            }
            this.SetAjaxAddressBook();
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
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

            string callType = "CALLTYPE_CORR_INT_EST";
            this.RapidDest.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;

            callType = "CALLTYPE_PROTO_IN";
            this.RapidProponente.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
        }

        #endregion

        #region Event button

        protected void AddFilterBtnConfirm_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                if (this.BindFilters())
                {
                    ApplyFilters();
                    ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('AddFilterLibroFirma','up');", true);
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
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('AddFilterLibroFirma','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_idProto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (this.ddl_idProto.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initIdProto.ReadOnly = false;
                        this.txt_fineIdProto.Visible = false;
                        this.LtlAIdProto.Visible = false;
                        this.LtlDaIdProto.Visible = false;
                        this.txt_fineIdProto.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initIdProto.ReadOnly = false;
                        this.txt_fineIdProto.ReadOnly = false;
                        this.LtlAIdProto.Visible = true;
                        this.LtlDaIdProto.Visible = true;
                        this.txt_fineIdProto.Visible = true;
                        break;
                }
                this.UpPnlIdProto.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void ImgDestAddressBook_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.chkDestExtendHistoricized.Checked)
                {
                    this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                }
                else
                {
                    this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST;
                }
                HttpContext.Current.Session["AddressBook.from"] = "F_X_X_S";
                //OpenAddressBookFromFilter = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "AddressBook", "parent.ajaxModalPopupAddressBook();", true);
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
                    RubricaCallType calltype = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
                    if (this.chkDestExtendHistoricized.Checked)
                    {
                        calltype = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                    }
                    else
                    {
                        calltype = RubricaCallType.CALLTYPE_CORR_INT_EST;
                    }
                    Corrispondente corr = null;
                    corr = UIManager.AddressBookManager.getCorrispondenteRubrica(codeAddressBook, calltype);
                    
                    if (corr == null)
                    {
                        switch (caller.ID)
                        { 
                            case "txtCodiceDest":
                                 this.txtCodiceDest.Text = string.Empty;
                                 this.txtDescrizioneDest.Text = string.Empty;
                                 this.idDest.Value = string.Empty;
                                 this.upPnlDest.Update();
                                 break;
                            case "txtCodiceProponente":
                                 this.txtCodiceProponente.Text = string.Empty;
                                 this.txtDescrizioneProponente.Text = string.Empty;
                                 this.idProponente.Value = string.Empty;
                                 this.UpdPnlProponente.Update();
                                 break;
                        }
                       
                        string msg = "ErrorTransmissionCorrespondentNotFound";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                    else
                    {
                        switch (caller.ID)
                        {
                            case "txtCodiceDest":
                                this.txtCodiceDest.Text = corr.codiceRubrica;
                                this.txtDescrizioneDest.Text = corr.descrizione;
                                this.idDest.Value = corr.systemId;
                                this.upPnlDest.Update();
                                break;
                            case "txtCodiceProponente":
                                this.txtCodiceProponente.Text = corr.codiceRubrica;
                                this.txtDescrizioneProponente.Text = corr.descrizione;
                                this.idProponente.Value = corr.systemId;
                                this.UpdPnlProponente.Update();
                                break;
                        }
                    }
                }
                else
                {
                    switch (caller.ID)
                    {
                        case "txtCodiceDest":
                            this.txtCodiceDest.Text = string.Empty;
                            this.txtDescrizioneDest.Text = string.Empty;
                            this.idDest.Value = string.Empty;
                            this.upPnlDest.Update();
                            break;
                        case "txtCodiceProponente":
                            this.txtCodiceProponente.Text = string.Empty;
                            this.txtDescrizioneProponente.Text = string.Empty;
                            this.idProponente.Value = string.Empty;
                            this.UpdPnlProponente.Update();
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void chkDestExtendHistoricized_Click(object sender, EventArgs e)
        {
            string dataUser = RoleManager.GetRoleInSession().systemId;
            Registro reg = RegistryManager.GetRegistryInSession();
            if (reg == null)
            {
                reg = RoleManager.GetRoleInSession().registri[0];
            }
            dataUser = dataUser + "-" + reg.systemId;
            string callType = string.Empty;

            if (this.chkDestExtendHistoricized.Checked)
            {
                callType = "CALLTYPE_CORR_INT_EST_CON_DISABILITATI";
            }
            else
            {
                callType = "CALLTYPE_CORR_INT_EST";
            }
            this.RapidDest.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;

            this.upPnlDest.Update();
        }

        protected void ddl_dataProt_E_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    // Gabriele Melini 13-01-2015
                    // INC000000519224
                    // Conservo i valori contenuti nei campi data
                    //this.txt_initDataProt_E.Text = string.Empty;
                    //this.txt_fineDataProt_E.Text = string.Empty;
                }

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dataProt_E.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initDataProt_E.ReadOnly = false;
                        this.txt_fineDataProt_E.Visible = false;
                        this.LtlADataProto.Visible = false;
                        this.LtlDaDataProto.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.txt_initDataProt_E.ReadOnly = false;
                        this.txt_fineDataProt_E.ReadOnly = false;
                        this.LtlADataProto.Visible = true;
                        this.LtlDaDataProto.Visible = true;
                        this.txt_fineDataProt_E.Visible = true;
                        this.LtlDaDataProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADataProto.Visible = false;
                        this.txt_fineDataProt_E.Visible = false;
                        this.txt_initDataProt_E.ReadOnly = true;
                        this.txt_initDataProt_E.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDataProto.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDataProt_E.Text = string.Empty;
                        break;
                    case 3: //Settimana corrente
                        this.LtlADataProto.Visible = true;
                        this.txt_fineDataProt_E.Visible = true;
                        this.txt_initDataProt_E.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txt_fineDataProt_E.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txt_fineDataProt_E.ReadOnly = true;
                        this.txt_initDataProt_E.ReadOnly = true;
                        this.LtlDaDataProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADataProto.Visible = true;
                        this.txt_fineDataProt_E.Visible = true;
                        this.txt_initDataProt_E.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txt_fineDataProt_E.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txt_fineDataProt_E.ReadOnly = true;
                        this.txt_initDataProt_E.ReadOnly = true;
                        this.LtlDaDataProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_idDoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (this.ddl_idDoc.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initIdDoc.ReadOnly = false;
                        this.txt_fineIdDoc.Visible = false;
                        this.LtlAIdDoc.Visible = false;
                        this.LtlDaIdDoc.Visible = false;
                        this.txt_fineIdDoc.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initIdDoc.ReadOnly = false;
                        this.txt_fineIdDoc.ReadOnly = false;
                        this.LtlAIdDoc.Visible = true;
                        this.LtlDaIdDoc.Visible = true;
                        this.txt_fineIdDoc.Visible = true;
                        break;
                }
                this.UpPnlIdDoc.Update();
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
                if (sender != null)
                {
                    // Gabriele Melini 13-01-2015
                    // INC000000519224
                    // Conservo i valori contenuti nei campi data
                    //this.txt_initDataCreazione_E.Text = string.Empty;
                    //this.txt_finedataCreazione_E.Text = string.Empty;
                }

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
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_DataInserimento_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_DataInserimento.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_Da_dataInserimento.ReadOnly = false;
                        this.txt_A_dataInserimento.Visible = false;
                        this.ltlADataInserimento.Visible = false;
                        this.ltlDaDataInserimento.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.txt_Da_dataInserimento.ReadOnly = false;
                        this.txt_A_dataInserimento.ReadOnly = false;
                        this.ltlADataInserimento.Visible = true;
                        this.ltlDaDataInserimento.Visible = true;
                        this.txt_A_dataInserimento.Visible = true;
                        this.ltlDaDataInserimento.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.ltlADataInserimento.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.ltlADataInserimento.Visible = false;
                        this.txt_A_dataInserimento.Visible = false;
                        this.txt_Da_dataInserimento.ReadOnly = true;
                        this.txt_Da_dataInserimento.Text = NttDataWA.Utils.dateformat.toDay();
                        this.ltlDaDataInserimento.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_A_dataInserimento.Text = string.Empty;
                        break;
                    case 3: //Settimana corrente
                        this.ltlADataInserimento.Visible = true;
                        this.txt_A_dataInserimento.Visible = true;
                        this.txt_Da_dataInserimento.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txt_A_dataInserimento.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txt_A_dataInserimento.ReadOnly = true;
                        this.txt_Da_dataInserimento.ReadOnly = true;
                        this.ltlDaDataInserimento.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.ltlADataInserimento.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.ltlADataInserimento.Visible = true;
                        this.txt_A_dataInserimento.Visible = true;
                        this.txt_Da_dataInserimento.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txt_A_dataInserimento.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txt_A_dataInserimento.ReadOnly = true;
                        this.txt_Da_dataInserimento.ReadOnly = true;
                        this.ltlDaDataInserimento.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.ltlADataInserimento.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
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


        protected void ImgAddressBookProponente_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (this.chkDestExtendHistoricized.Checked)
                {
                    this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                }
                else
                {
                    this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST;
                }
                HttpContext.Current.Session["AddressBook.from"] = "F_X_X_S_4";
                //OpenAddressBookFromFilter = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "AddressBook", "parent.ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> ccList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.Cc"];
                string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();

                switch (addressBookCallFrom)
                {
                    case "F_X_X_S":
                        {
                            if (atList != null && atList.Count > 0)
                            {
                                NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                                Corrispondente tempCorrSingle;
                                if (!corrInSess.isRubricaComune)
                                    tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                                else
                                    tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                                this.txtCodiceDest.Text = tempCorrSingle.codiceRubrica;
                                this.txtDescrizioneDest.Text = tempCorrSingle.descrizione;
                                this.idDest.Value = tempCorrSingle.systemId;
                                this.upPnlDest.Update();
                            }
                        }
                        break;
                    case "F_X_X_S_4":
                        {
                            if (atList != null && atList.Count > 0)
                            {
                                NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                                Corrispondente tempCorrSingle;
                                if (!corrInSess.isRubricaComune)
                                    tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                                else
                                    tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                                this.txtCodiceProponente.Text = tempCorrSingle.codiceRubrica;
                                this.txtDescrizioneProponente.Text = tempCorrSingle.descrizione;
                                this.idProponente.Value = tempCorrSingle.systemId;
                                this.UpdPnlProponente.Update();
                            }
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

        private void LoadTypeDocuments()
        {
            DocsPaWR.TipologiaAtto[] listaTipologiaAtto;
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()].Equals("1"))
            {
                listaTipologiaAtto = DocumentManager.getTipoAttoPDInsRic(this, UserManager.GetInfoUser().idAmministrazione, RoleManager.GetRoleInSession().idGruppo, "1");
            }
            else
            {
                listaTipologiaAtto = DocumentManager.getListaTipologiaAtto(this);
            }

            this.DocumentDdlTypeDocument.Items.Clear();

            ListItem item = new ListItem(string.Empty, string.Empty);
            this.DocumentDdlTypeDocument.Items.Add(item);

            if (listaTipologiaAtto != null)
            {
                for (int i = 0; i < listaTipologiaAtto.Length; i++)
                {
                    item = new ListItem();
                    item.Text = listaTipologiaAtto[i].descrizione;
                    item.Value = listaTipologiaAtto[i].systemId;
                    this.DocumentDdlTypeDocument.Items.Add(item);
                }
            }
            this.DocumentDdlTypeDocument.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DocumentDdlTypeDocument", UserManager.GetUserLanguage()));
        }
        /*
        protected void ddl_idDoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (this.ddl_idDoc.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initIdDoc.ReadOnly = false;
                        this.txt_fineIdDoc.Visible = false;
                        this.LtlAIdDoc.Visible = false;
                        this.LtlDaIdDoc.Visible = false;
                        this.txt_fineIdDoc.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initIdDoc.ReadOnly = false;
                        this.txt_fineIdDoc.ReadOnly = false;
                        this.LtlAIdDoc.Visible = true;
                        this.LtlDaIdDoc.Visible = true;
                        this.txt_fineIdDoc.Visible = true;
                        break;
                }
                this.UpPnlIdDoc.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
         * */

        #endregion

        #region Filtri

        private bool BindFilters()
        {
            List<FiltroElementiLibroFirma> elementsFilter = new List<FiltroElementiLibroFirma>();
            FiltroElementiLibroFirma filter;

            #region TIPO
            if (this.cbl_archDoc_E.Items.FindByValue("A") != null)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
                if (this.cbl_archDoc_E.Items.FindByValue("A").Selected)
                    filter.Valore = "true";
                else
                    filter.Valore = "false";
                elementsFilter.Add(filter);
            }

            if (this.cbl_archDoc_E.Items.FindByValue("P") != null)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
                if (this.cbl_archDoc_E.Items.FindByValue("P").Selected)
                    filter.Valore = "true";
                else
                    //valore += "0^";
                    filter.Valore = "false";
                elementsFilter.Add(filter);
            }

            if (this.cbl_archDoc_E.Items.FindByValue("I") != null)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
                if (this.cbl_archDoc_E.Items.FindByValue("I").Selected)
                    filter.Valore = "true";
                else
                    filter.Valore = "false";
                elementsFilter.Add(filter);
            }

            filter = new DocsPaWR.FiltroElementiLibroFirma();
            filter.Argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
            filter.Valore = this.cbl_archDoc_E.Items.FindByValue("G").Selected.ToString();
            elementsFilter.Add(filter);

            if (this.cbl_archDoc_E.Items.FindByValue("ALL").Selected)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriDocumento.ALLEGATO.ToString();
                filter.Valore = "true";
                elementsFilter.Add(filter);
            }

            if (this.cbl_archDoc_E.Items.FindByValue("Pr") != null)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                if (this.cbl_archDoc_E.Items.FindByValue("Pr").Selected)
                    //valore += "1";
                    filter.Valore = "true";
                else
                    //valore += "0";
                    filter.Valore = "false";
                elementsFilter.Add(filter);
            }

            filter = new DocsPaWR.FiltroElementiLibroFirma();
            filter.Argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
            filter.Valore = "tipo";
            elementsFilter.Add(filter);
            #endregion
            #region OGGETTO
            if (!string.IsNullOrEmpty(this.TxtObject.Text))
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriDocumento.OGGETTO.ToString();
                filter.Valore = utils.DO_AdattaString(this.TxtObject.Text);
                elementsFilter.Add(filter);
            }
            #endregion
            #region NUMERO_PROTOCOLLO
            if (this.ddl_idProto.SelectedIndex == 0)
            {//valore singolo carico NUM_PROTOCOLLO

                if (this.txt_initIdProto.Text != null && !this.txt_initIdProto.Text.Equals(""))
                {
                    filter = new DocsPaWR.FiltroElementiLibroFirma();
                    filter.Argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();
                    filter.Valore = this.txt_initIdProto.Text;
                    elementsFilter.Add(filter);
                }
            }
            else
            {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                if (!this.txt_initIdProto.Text.Equals(""))
                {
                    filter = new DocsPaWR.FiltroElementiLibroFirma();
                    filter.Argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                    filter.Valore = this.txt_initIdProto.Text;
                    elementsFilter.Add(filter);
                }
                if (!this.txt_fineIdProto.Text.Equals(""))
                {
                    filter = new DocsPaWR.FiltroElementiLibroFirma();
                    filter.Argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
                    filter.Valore = this.txt_fineIdProto.Text;
                    elementsFilter.Add(filter);
                }
            }
            #endregion
            #region DATA_PROTOCOLLO
            if (this.ddl_dataProt_E.SelectedIndex == 2)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriDocumento.DATA_PROT_TODAY.ToString();
                filter.Valore = "1";
                elementsFilter.Add(filter);
            }
            if (this.ddl_dataProt_E.SelectedIndex == 3)
            {
                // siamo nel caso di Settimana corrente
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SC.ToString();
                filter.Valore = "1";
                elementsFilter.Add(filter);
            }
            if (this.ddl_dataProt_E.SelectedIndex == 4)
            {
                // siamo nel caso di Mese corrente
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MC.ToString();
                filter.Valore = "1";
                elementsFilter.Add(filter);
            }
            if (this.ddl_dataProt_E.SelectedIndex == 0)
            {//valore singolo carico DATA_PROTOCOLLO
                if (!this.txt_initDataProt_E.Text.Equals(""))
                {

                    filter = new DocsPaWR.FiltroElementiLibroFirma();
                    filter.Argomento = DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString();
                    filter.Valore = this.txt_initDataProt_E.Text;
                    elementsFilter.Add(filter);
                }
            }
            if (this.ddl_dataProt_E.SelectedIndex == 1)
            {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
                if (!string.IsNullOrEmpty(txt_initDataProt_E.Text) &&
                    !string.IsNullOrEmpty(txt_fineDataProt_E.Text) &&
                    utils.verificaIntervalloDate(txt_initDataProt_E.Text, txt_fineDataProt_E.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDateProtInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDateProtInterval', 'warning', '');};", true);
                    return false;
                }
                if (!this.txt_initDataProt_E.Text.Equals(""))
                {

                    filter = new DocsPaWR.FiltroElementiLibroFirma();
                    filter.Argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
                    filter.Valore = this.txt_initDataProt_E.Text;
                    elementsFilter.Add(filter);
                }
                if (!this.txt_fineDataProt_E.Text.Equals(""))
                {

                    filter = new DocsPaWR.FiltroElementiLibroFirma();
                    filter.Argomento = DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
                    filter.Valore = this.txt_fineDataProt_E.Text;
                    elementsFilter.Add(filter);
                }
            }
            #endregion
            #region DESTINATARIO
            if (!string.IsNullOrEmpty(this.idDest.Value))
            {
                if (!this.txtDescrizioneDest.Text.Equals(""))
                {
                    if (!string.IsNullOrEmpty(this.txtCodiceDest.Text))
                    {
                        if (this.chkDestExtendHistoricized.Checked)
                        {
                            // Ricerca i documenti per i mittenti / destinatari storicizzati
                            filter = new DocsPaWR.FiltroElementiLibroFirma();
                            filter.Argomento = DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString();
                            filter.Valore = this.txtCodiceDest.Text;
                            elementsFilter.Add(filter);

                            filter = new DocsPaWR.FiltroElementiLibroFirma();
                            filter.Argomento = DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString();
                            filter.Valore = this.chkDestExtendHistoricized.Checked.ToString();
                            elementsFilter.Add(filter);
                        }
                        else
                        {
                            filter = new DocsPaWR.FiltroElementiLibroFirma();
                            filter.Argomento = DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString();
                            filter.Valore = this.idDest.Value;
                            elementsFilter.Add(filter);
                        }
                    }
                }
            }
            else
            {
                if (!this.txtDescrizioneDest.Text.Equals(""))
                {
                    if (!string.IsNullOrEmpty(this.txtCodiceDest.Text))
                    {
                        if (this.chkDestExtendHistoricized.Checked)
                        {
                            // Ricerca i documenti per i mittenti / destinatari storicizzati
                            filter = new DocsPaWR.FiltroElementiLibroFirma();
                            filter.Argomento = DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString();
                            filter.Valore = this.txtCodiceDest.Text;
                            elementsFilter.Add(filter);

                            filter = new DocsPaWR.FiltroElementiLibroFirma();
                            filter.Argomento = DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString();
                            filter.Valore = this.chkDestExtendHistoricized.Checked.ToString();
                            elementsFilter.Add(filter);
                        }
                        else
                        {
                            // Ricerca dell'id del corrispondente a partire dal codice
                            DocsPaWR.Corrispondente corrByCode = AddressBookManager.getCorrispondenteByCodRubrica(this.txtCodiceDest.Text, false);
                            if (corrByCode != null)
                            {
                                this.idDest.Value = corrByCode.systemId;

                                filter = new DocsPaWR.FiltroElementiLibroFirma();
                                filter.Argomento = DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString();
                                filter.Valore = this.idDest.Value;
                                elementsFilter.Add(filter);
                            }
                            else
                            {
                                filter = new DocsPaWR.FiltroElementiLibroFirma();
                                filter.Argomento = DocsPaWR.FiltriDocumento.MITT_DEST.ToString();
                                filter.Valore = this.txtDescrizioneDest.Text;
                                elementsFilter.Add(filter);
                            }
                        }
                    }
                    else
                    {
                        filter = new DocsPaWR.FiltroElementiLibroFirma();
                        filter.Argomento = DocsPaWR.FiltriDocumento.MITT_DEST.ToString();
                        filter.Valore = this.txtDescrizioneDest.Text;
                        elementsFilter.Add(filter);
                    }
                }
            }
            #endregion

            #region PROPONENTE
            if (!string.IsNullOrEmpty(this.idProponente.Value))
            {
                if (!this.txtDescrizioneProponente.Text.Equals(""))
                {
                    if (!string.IsNullOrEmpty(this.txtCodiceProponente.Text))
                    {
                        if (this.chkProponenteExtendHistoricized.Checked)
                        {
                            // Ricerca i documenti per i mittenti / destinatari storicizzati
                            filter = new DocsPaWR.FiltroElementiLibroFirma();
                            filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.CODICE_PROPONENTE.ToString();
                            filter.Valore = this.txtCodiceProponente.Text;
                            elementsFilter.Add(filter);

                            filter = new DocsPaWR.FiltroElementiLibroFirma();
                            filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.PROPONENTE_STORICIZZATO.ToString();
                            filter.Valore = this.chkProponenteExtendHistoricized.Checked.ToString();
                            elementsFilter.Add(filter);
                        }
                        else
                        {
                            filter = new DocsPaWR.FiltroElementiLibroFirma();
                            filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.ID_PROPONENTE.ToString();
                            filter.Valore = this.idProponente.Value;
                            elementsFilter.Add(filter);
                        }
                    }
                }
            }
            else
            {
                if (!this.txtDescrizioneProponente.Text.Equals(""))
                {
                    if (!string.IsNullOrEmpty(this.txtCodiceProponente.Text))
                    {
                        if (this.chkProponenteExtendHistoricized.Checked)
                        {
                            // Ricerca i documenti per i mittenti / destinatari storicizzati
                            filter = new DocsPaWR.FiltroElementiLibroFirma();
                            filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.CODICE_PROPONENTE.ToString();
                            filter.Valore = this.txtCodiceProponente.Text;
                            elementsFilter.Add(filter);

                            filter = new DocsPaWR.FiltroElementiLibroFirma();
                            filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.PROPONENTE_STORICIZZATO.ToString();
                            filter.Valore = this.chkProponenteExtendHistoricized.Checked.ToString();
                            elementsFilter.Add(filter);
                        }
                        else
                        {
                            // Ricerca dell'id del corrispondente a partire dal codice
                            DocsPaWR.Corrispondente corrByCode = AddressBookManager.getCorrispondenteByCodRubrica(this.txtCodiceProponente.Text, false);
                            if (corrByCode != null)
                            {
                                this.idProponente.Value = corrByCode.systemId;

                                filter = new DocsPaWR.FiltroElementiLibroFirma();
                                filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.ID_PROPONENTE.ToString();
                                filter.Valore = this.idProponente.Value;
                                elementsFilter.Add(filter);
                            }
                            else
                            {
                                filter = new DocsPaWR.FiltroElementiLibroFirma();
                                filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.DESCRIZIONE_PROPONENTE.ToString();
                                filter.Valore = this.txtDescrizioneProponente.Text;
                                elementsFilter.Add(filter);
                            }
                        }
                    }
                    else
                    {
                        filter = new DocsPaWR.FiltroElementiLibroFirma();
                        filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.DESCRIZIONE_PROPONENTE.ToString();
                        filter.Valore = this.txtDescrizioneProponente.Text;
                        elementsFilter.Add(filter);
                    }
                }
            }
            #endregion

            #region DOCNUMBER
            if (this.ddl_idDoc.SelectedIndex == 0)
            {
                if (this.txt_initIdDoc.Text != null && !this.txt_initIdDoc.Text.Equals(""))
                {
                    filter = new DocsPaWR.FiltroElementiLibroFirma();
                    filter.Argomento = DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();
                    filter.Valore = this.txt_initIdDoc.Text;
                    elementsFilter.Add(filter);
                }
            }
            else
            {
                if (this.txt_initIdDoc.Text != null && !this.txt_initIdDoc.Text.Equals(""))
                {
                    filter = new DocsPaWR.FiltroElementiLibroFirma();
                    filter.Argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
                    filter.Valore = this.txt_initIdDoc.Text;
                    elementsFilter.Add(filter);
                }
                if (this.txt_fineIdDoc.Text != null && !this.txt_fineIdDoc.Text.Equals(""))
                {
                    filter = new DocsPaWR.FiltroElementiLibroFirma();
                    filter.Argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
                    filter.Valore = this.txt_fineIdDoc.Text;
                    elementsFilter.Add(filter);
                }
            }
            #endregion
            #region DATA_CREAZIONE
            if (this.ddl_dataCreazione_E.SelectedIndex == 2)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_TODAY.ToString();
                filter.Valore = "1";
                elementsFilter.Add(filter);
            }
            if (this.ddl_dataCreazione_E.SelectedIndex == 3)
            {
                // siamo nel caso di Settimana corrente
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_SC.ToString();
                filter.Valore = "1";
                elementsFilter.Add(filter);
            }
            if (this.ddl_dataCreazione_E.SelectedIndex == 4)
            {
                // siamo nel caso di Mese corrente
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_MC.ToString();
                filter.Valore = "1";
                elementsFilter.Add(filter);
            }
            if (this.ddl_dataCreazione_E.SelectedIndex == 0)

                if (this.ddl_dataCreazione_E.SelectedIndex == 0)
                { //valore singolo carico DATA_CREAZIONE
                    if (!this.txt_initDataCreazione_E.Text.Equals(""))
                    {
                        filter = new DocsPaWR.FiltroElementiLibroFirma();
                        filter.Argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
                        filter.Valore = this.txt_initDataCreazione_E.Text;
                        elementsFilter.Add(filter);
                    }
                }

            if (this.ddl_dataCreazione_E.SelectedIndex == 1)
            {//valore singolo carico DATA_CREAZIONE_DAL - DATA_CREAZIONE_AL
                if (!string.IsNullOrEmpty(txt_initDataCreazione_E.Text) &&
                   !string.IsNullOrEmpty(txt_finedataCreazione_E.Text) &&
                   utils.verificaIntervalloDate(txt_initDataCreazione_E.Text, txt_finedataCreazione_E.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDateCreateInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDateCreateInterval', 'warning', '');};", true);
                    return false;
                }
                if (!this.txt_initDataCreazione_E.Text.Equals(""))
                {

                    filter = new DocsPaWR.FiltroElementiLibroFirma();
                    filter.Argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                    filter.Valore = this.txt_initDataCreazione_E.Text;
                    elementsFilter.Add(filter);
                }
                if (!this.txt_finedataCreazione_E.Text.Equals(""))
                {

                    filter = new DocsPaWR.FiltroElementiLibroFirma();
                    filter.Argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                    filter.Valore = this.txt_finedataCreazione_E.Text;
                    elementsFilter.Add(filter);
                }
            }
            #endregion
            #region TIPOLOGIA

            if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString();
                filter.Valore = this.DocumentDdlTypeDocument.SelectedItem.Text;
                elementsFilter.Add(filter);
            }
            #endregion
            #region TIPO_FIRMA_RICHIESTA

            if (this.cbxTypeSignature.Items.FindByValue("DC") != null)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.FIRMA_DIGITALE_CADES.ToString();
                if (this.cbxTypeSignature.Items.FindByValue("DC").Selected)
                    filter.Valore = "true";
                else
                    filter.Valore = "false";
                elementsFilter.Add(filter);
            }

            if (this.cbxTypeSignature.Items.FindByValue("DP") != null)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.FIRMA_DIGITALE_PADES.ToString();
                if (this.cbxTypeSignature.Items.FindByValue("DP").Selected)
                    filter.Valore = "true";
                else
                    filter.Valore = "false";
                elementsFilter.Add(filter);
            }

            if (this.cbxTypeSignature.Items.FindByValue("ES") != null)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.FIRMA_ELETTRONICA_SOTTOSCRIZIONE.ToString();
                if (this.cbxTypeSignature.Items.FindByValue("ES").Selected)
                    filter.Valore = "true";
                else
                    filter.Valore = "false";
                elementsFilter.Add(filter);
            }
            if (this.cbxTypeSignature.Items.FindByValue("EA") != null)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.FIRMA_ELETTRONICA_AVANZAMENTO.ToString();
                if (this.cbxTypeSignature.Items.FindByValue("EA").Selected)
                    filter.Valore = "true";
                else
                    filter.Valore = "false";
                elementsFilter.Add(filter);
            }

            #endregion
            #region  MODALITA

            if (this.CbxMode.Items.FindByValue("A") != null)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.MODALITA_AUTOMATICA.ToString();
                if (this.CbxMode.Items.FindByValue("A").Selected)
                    filter.Valore = "true";
                else
                    filter.Valore = "false";
                elementsFilter.Add(filter);
            }
            if (this.CbxMode.Items.FindByValue("M") != null)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.MODALITA_MANUALE.ToString();
                if (this.CbxMode.Items.FindByValue("M").Selected)
                    filter.Valore = "true";
                else
                    filter.Valore = "false";
                elementsFilter.Add(filter);
            }
            #endregion
            #region STATO

            if (this.CbxState.Items.FindByValue("PROPOSTO") != null)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.PROPOSTO.ToString();
                if (this.CbxState.Items.FindByValue("PROPOSTO").Selected)
                    filter.Valore = "true";
                else
                    filter.Valore = "false";
                elementsFilter.Add(filter);
            }

            if (this.CbxState.Items.FindByValue("DA_FIRMARE") != null)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.DA_FIRMARE.ToString();
                if (this.CbxState.Items.FindByValue("DA_FIRMARE").Selected)
                    filter.Valore = "true";
                else
                    //valore += "0^";
                    filter.Valore = "false";
                elementsFilter.Add(filter);
            }

            if (this.CbxState.Items.FindByValue("DA_RESPINGERE") != null)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.DA_RESPINGERE.ToString();
                if (this.CbxState.Items.FindByValue("DA_RESPINGERE").Selected)
                    filter.Valore = "true";
                else
                    filter.Valore = "false";
                elementsFilter.Add(filter);
            }


            #endregion
            #region STATO CON_ERRORI/SENZA_ERRORI

            if (this.CbxErrorState.Items.FindByValue("CON_ERRORI") != null)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.CON_ERRORI.ToString();
                if (this.CbxErrorState.Items.FindByValue("CON_ERRORI").Selected)
                    filter.Valore = "true";
                else
                    //valore += "0^";
                    filter.Valore = "false";
                elementsFilter.Add(filter);
            }

            if (this.CbxErrorState.Items.FindByValue("SENZA_ERRORI") != null)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.SENZA_ERRORI.ToString();
                if (this.CbxErrorState.Items.FindByValue("SENZA_ERRORI").Selected)
                    filter.Valore = "true";
                else
                    //valore += "0^";
                    filter.Valore = "false";
                elementsFilter.Add(filter);
            }


            #endregion
            #region NOTE
            if (!string.IsNullOrEmpty(this.TxtNote.Text))
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.NOTE.ToString();
                filter.Valore = utils.DO_AdattaString(this.TxtNote.Text);
                elementsFilter.Add(filter);
            }
            #endregion
            #region DATA_INSERIMENTO
            if (this.ddl_DataInserimento.SelectedIndex == 2)
            {
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_TODAY.ToString();
                filter.Valore = "1";
                elementsFilter.Add(filter);
            }
            if (this.ddl_DataInserimento.SelectedIndex == 3)
            {
                // siamo nel caso di Settimana corrente
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_SC.ToString();
                filter.Valore = "1";
                elementsFilter.Add(filter);
            }
            if (this.ddl_DataInserimento.SelectedIndex == 4)
            {
                // siamo nel caso di Mese corrente
                filter = new DocsPaWR.FiltroElementiLibroFirma();
                filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_MC.ToString();
                filter.Valore = "1";
                elementsFilter.Add(filter);
            }
            if (this.ddl_DataInserimento.SelectedIndex == 0)

                if (this.ddl_DataInserimento.SelectedIndex == 0)
                {
                    if (!this.txt_Da_dataInserimento.Text.Equals(""))
                    {
                        filter = new DocsPaWR.FiltroElementiLibroFirma();
                        filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_IL.ToString();
                        filter.Valore = this.txt_Da_dataInserimento.Text;
                        elementsFilter.Add(filter);
                    }
                }

            if (this.ddl_DataInserimento.SelectedIndex == 1)
            {
                if (!string.IsNullOrEmpty(txt_Da_dataInserimento.Text) &&
                   !string.IsNullOrEmpty(txt_A_dataInserimento.Text) &&
                   utils.verificaIntervalloDate(txt_Da_dataInserimento.Text, txt_A_dataInserimento.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDateCreateInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDateCreateInterval', 'warning', '');};", true);
                    return false;
                }
                if (!this.txt_Da_dataInserimento.Text.Equals(""))
                {

                    filter = new DocsPaWR.FiltroElementiLibroFirma();
                    filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_SUCCESSIVA_AL.ToString();
                    filter.Valore = this.txt_Da_dataInserimento.Text;
                    elementsFilter.Add(filter);
                }
                if (!this.txt_A_dataInserimento.Text.Equals(""))
                {

                    filter = new DocsPaWR.FiltroElementiLibroFirma();
                    filter.Argomento = DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_PRECEDENTE_IL.ToString();
                    filter.Valore = this.txt_A_dataInserimento.Text;
                    elementsFilter.Add(filter);
                }
            }

            #endregion

            #region MODALITA
            #endregion

            this.FiltersElement = elementsFilter;
            return true;
        }

        private void BindFilterValues()
        {
            try
            {
                foreach (DocsPaWR.FiltroElementiLibroFirma item in this.FiltersElement)
                {
                    #region TIPO
                    #region PROTOCOLLO_ARRIVO
                    if (item.Argomento == DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString())
                    {
                        this.cbl_archDoc_E.Items.FindByValue("A").Selected = Convert.ToBoolean(item.Valore);
                    }
                    #endregion
                    #region PROTOCOLLO_PARTENZA
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString())
                    {
                        this.cbl_archDoc_E.Items.FindByValue("P").Selected = Convert.ToBoolean(item.Valore);
                    }
                    #endregion
                    #region PROTOCOLLO_INTERNO

                    else if (item.Argomento == DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString())
                    {
                        if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString())) && !Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString()).Equals("1"))
                        {
                            this.opInt.Attributes.CssStyle.Add("display", "none");
                            this.opInt.Selected = false;
                        }
                        else if (this.cbl_archDoc_E.Items.FindByValue("I") != null)
                            this.cbl_archDoc_E.Items.FindByValue("I").Selected = Convert.ToBoolean(item.Valore);
                    }
                    #endregion
                    #region GRIGI
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.GRIGIO.ToString())
                    {
                        this.cbl_archDoc_E.Items.FindByValue("G").Selected = Convert.ToBoolean(item.Valore);
                    }
                    #endregion
                    #region PREDISPOSTI
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString())
                    {
                        this.cbl_archDoc_E.Items.FindByValue("Pr").Selected = Convert.ToBoolean(item.Valore);
                    }
                    #endregion
                    #region ALLEGATI
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.ALLEGATO.ToString())
                    {
                        this.cbl_archDoc_E.Items.FindByValue("ALL").Selected = true; //Convert.ToBoolean(aux.valore);
                    }
                    #endregion
                    #endregion
                    #region OGGETTO
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.OGGETTO.ToString())
                    {
                        this.TxtObject.Text = item.Valore;
                    }
                    #endregion OGGETTO
                    #region NUMERO_PROTOCOLLO

                    #region NUM_PROTOCOLLO
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString())
                    {
                        if (this.ddl_idProto.SelectedIndex != 0)
                            this.ddl_idProto.SelectedIndex = 0;
                        this.ddl_idProto_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_initIdProto.Text = item.Valore;
                    }
                    #endregion NUM_PROTOCOLLO
                    #region NUM_PROTOCOLLO_DAL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString())
                    {
                        if (this.ddl_idProto.SelectedIndex != 1)
                            this.ddl_idProto.SelectedIndex = 1;
                        this.ddl_idProto_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_initIdProto.Text = item.Valore;
                    }
                    #endregion NUM_PROTOCOLLO_DAL
                    #region NUM_PROTOCOLLO_AL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString())
                    {
                        if (this.ddl_idProto.SelectedIndex != 1)
                            this.ddl_idProto.SelectedIndex = 1;
                        this.ddl_idProto_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_fineIdProto.Text = item.Valore;
                    }
                    #endregion NUM_PROTOCOLLO_AL

                    #endregion
                    #region DATA_PROTOCOLLO

                    #region DATA_PROT_IL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString())
                    {
                        if (this.ddl_dataProt_E.SelectedIndex != 0)
                            this.ddl_dataProt_E.SelectedIndex = 0;
                        this.ddl_dataProt_E_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_initDataProt_E.Text = item.Valore;
                    }
                    #endregion DATA_PROT_IL
                    #region DATA_PROT_SUCCESSIVA_AL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString())
                    {
                        if (this.ddl_dataProt_E.SelectedIndex != 1)
                            this.ddl_dataProt_E.SelectedIndex = 1;
                        this.ddl_dataProt_E_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_initDataProt_E.Text = item.Valore;
                    }
                    #endregion DATA_PROT_SUCCESSIVA_AL
                    #region DATA_PROT_PRECEDENTE_IL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString())
                    {
                        if (ddl_dataProt_E.SelectedIndex != 1)
                            ddl_dataProt_E.SelectedIndex = 1;
                        ddl_dataProt_E_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_fineDataProt_E.Text = item.Valore;
                    }
                    #endregion DATA_PROT_PRECEDENTE_IL
                    #region DATA_PROT_SC
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_SC.ToString() && item.Valore == "1")
                    {
                        this.ddl_dataProt_E.SelectedIndex = 3;
                        this.ddl_dataProt_E_SelectedIndexChanged(null, new System.EventArgs());
                    }
                    #endregion
                    #region DATA_PROT_MC
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MC.ToString() && item.Valore == "1")
                    {
                        this.ddl_dataProt_E.SelectedIndex = 4;
                        this.ddl_dataProt_E_SelectedIndexChanged(null, new System.EventArgs());
                    }
                    #endregion
                    #region DATA_PROT_TODAY
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_TODAY.ToString() && item.Valore == "1")
                    {
                        this.ddl_dataProt_E.SelectedIndex = 2;
                        this.ddl_dataProt_E_SelectedIndexChanged(null, new System.EventArgs());
                    }
                    #endregion
                    #endregion
                    #region DESTINATARIO

                    #region DEST
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.MITT_DEST.ToString())
                    {
                        txtDescrizioneDest.Text = item.Valore;
                    }
                    #endregion DEST
                    #region COD_T_DEST
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString())
                    {
                        SearchCorrespondent(item.Valore, "txtCodiceDest");
                    }
                    #endregion

                    #region DEST_STORICIZZATI
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString())
                    {
                        bool chkValue;
                        bool.TryParse(item.Valore, out chkValue);
                        this.chkDestExtendHistoricized.Checked = chkValue;
                    }
                    #endregion
                    #region ID_DEST
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString())
                    {
                        DocsPaWR.Corrispondente corr = AddressBookManager.GetCorrespondentBySystemId(item.Valore);
                        if (corr != null)
                        {
                            txtCodiceDest.Text = corr.codiceRubrica;
                            txtDescrizioneDest.Text = corr.descrizione;
                        }
                    }
                    #endregion ID_MITT_DEST
                    #endregion

                    #region PROPONENTE

                    #region PROPONENTE
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DESCRIZIONE_PROPONENTE.ToString())
                    {
                        this.txtDescrizioneProponente.Text = item.Valore;
                    }
                    #endregion PROPONENTE
                    #region COD_T_PROPONENTE
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.CODICE_PROPONENTE.ToString())
                    {
                        SearchCorrespondent(item.Valore, "txtCodiceProponente");
                    }
                    #endregion

                    #region PROPONENTE_STORICIZZATI
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.PROPONENTE_STORICIZZATO.ToString())
                    {
                        bool chkValue;
                        bool.TryParse(item.Valore, out chkValue);
                        this.chkProponenteExtendHistoricized.Checked = chkValue;
                    }
                    #endregion
                    #region ID_PROPONENTE
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.ID_PROPONENTE.ToString())
                    {
                        DocsPaWR.Corrispondente corr = AddressBookManager.GetCorrespondentBySystemId(item.Valore);
                        if (corr != null)
                        {
                            this.txtCodiceProponente.Text = corr.codiceRubrica;
                            this.txtDescrizioneProponente.Text = corr.descrizione;
                        }
                        this.idProponente.Value = item.Valore;
                    }
                    #endregion ID_MITT_PROPONENTE
                    #endregion

                    #region DOCNUMBER

                    #region DOCNUMBER
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DOCNUMBER.ToString())
                    {
                        if (this.ddl_idDoc.SelectedIndex != 0)
                            this.ddl_idDoc.SelectedIndex = 0;
                        this.ddl_idDoc_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_initIdDoc.Text = item.Valore;
                    }
                    #endregion DOCNUMBER
                    #region DOCNUMBER_DAL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString())
                    {
                        if (this.ddl_idDoc.SelectedIndex != 1)
                            this.ddl_idDoc.SelectedIndex = 1;
                        this.ddl_idDoc_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_initIdDoc.Text = item.Valore;
                    }
                    #endregion DOCNUMBER_DAL
                    #region DOCNUMBER_AL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString())
                    {
                        if (this.ddl_idDoc.SelectedIndex != 1)
                            this.ddl_idDoc.SelectedIndex = 1;
                        this.ddl_idDoc_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_fineIdDoc.Text = item.Valore;
                    }
                    #endregion DOCNUMBER_AL

                    #endregion
                    #region DATA_CREAZIONE

                    #region DATA_CREAZIONE_IL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString())
                    {
                        if (ddl_dataCreazione_E.SelectedIndex != 0)
                            ddl_dataCreazione_E.SelectedIndex = 0;
                        ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_initDataCreazione_E.Text = item.Valore;
                    }
                    #endregion DATA_CREAZIONE_IL
                    #region DATA_CREAZIONE_SUCCESSIVA_AL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString())
                    {
                        if (ddl_dataCreazione_E.SelectedIndex != 1)
                            ddl_dataCreazione_E.SelectedIndex = 1;
                        ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_initDataCreazione_E.Text = item.Valore;
                    }
                    #endregion DATA_CREAZIONE_SUCCESSIVA_AL
                    #region DATA_CREAZIONE_PRECEDENTE_IL
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString())
                    {
                        if (this.ddl_dataCreazione_E.SelectedIndex != 1)
                            this.ddl_dataCreazione_E.SelectedIndex = 1;
                        this.ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_finedataCreazione_E.Text = item.Valore;
                    }
                    #endregion DATA_CREAZIONE_PRECEDENTE_IL
                    #region DATA_CREAZ_SC
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_SC.ToString() && item.Valore == "1")
                    {
                        this.ddl_dataCreazione_E.SelectedIndex = 3;
                        this.ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                    }
                    #endregion
                    #region DATA_CREAZ_MC
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_MC.ToString() && item.Valore == "1")
                    {
                        this.ddl_dataCreazione_E.SelectedIndex = 4;
                        this.ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                    }
                    #endregion
                    #region DATA_CREAZ_TODAY
                    else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_TODAY.ToString() && item.Valore == "1")
                    {
                        this.ddl_dataCreazione_E.SelectedIndex = 2;
                        this.ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                    }
                    #endregion
                    #endregion

                    #region TIPOLOGIA

                    else if (item.Argomento == DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString())
                    {
                        this.DocumentDdlTypeDocument.Items.FindByText(item.Valore).Selected = true;
                    }
                    #endregion

                    #region TIPO_FIRMA

                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.FIRMA_DIGITALE_CADES.ToString())
                    {
                        this.cbxTypeSignature.Items.FindByValue("DC").Selected = Convert.ToBoolean(item.Valore);
                    }
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.FIRMA_DIGITALE_PADES.ToString())
                    {
                        this.cbxTypeSignature.Items.FindByValue("DP").Selected = Convert.ToBoolean(item.Valore);
                    }
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.FIRMA_ELETTRONICA_SOTTOSCRIZIONE.ToString())
                    {
                        this.cbxTypeSignature.Items.FindByValue("ES").Selected = Convert.ToBoolean(item.Valore);
                    }
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.FIRMA_ELETTRONICA_AVANZAMENTO.ToString())
                    {
                        this.cbxTypeSignature.Items.FindByValue("EA").Selected = Convert.ToBoolean(item.Valore);
                    }
                    #endregion

                    #region MODALITA

                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.MODALITA_AUTOMATICA.ToString())
                    {
                        this.CbxMode.Items.FindByValue("A").Selected = Convert.ToBoolean(item.Valore);
                    }

                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.MODALITA_MANUALE.ToString())
                    {
                        this.CbxMode.Items.FindByValue("M").Selected = Convert.ToBoolean(item.Valore);
                    }
                    #endregion

                    #region STATO
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.PROPOSTO.ToString())
                    {
                        this.CbxState.Items.FindByValue("PROPOSTO").Selected = Convert.ToBoolean(item.Valore);
                    }
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DA_FIRMARE.ToString())
                    {
                        this.CbxState.Items.FindByValue("DA_FIRMARE").Selected = Convert.ToBoolean(item.Valore);
                    }
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DA_RESPINGERE.ToString())
                    {
                        this.CbxState.Items.FindByValue("DA_RESPINGERE").Selected = Convert.ToBoolean(item.Valore);
                    }
                    #endregion

                    #region CON_ERRORI/SENZA_ERRORI
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.CON_ERRORI.ToString())
                    {
                        this.CbxErrorState.Items.FindByValue("CON_ERRORI").Selected = Convert.ToBoolean(item.Valore);
                    }
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.SENZA_ERRORI.ToString())
                    {
                        this.CbxErrorState.Items.FindByValue("SENZA_ERRORI").Selected = Convert.ToBoolean(item.Valore);
                    }
                    #endregion
                    #region NOTE

                    else if (item.Argomento == DocsPaWR.FiltriDocumento.NOTE.ToString())
                    {
                        this.TxtNote.Text = item.Valore;
                    }

                    #endregion

                    #region DATA_INSERIMENTO

                    #region DATA_INSERIMENTO_IL
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_IL.ToString())
                    {
                        if (this.ddl_DataInserimento.SelectedIndex != 0)
                            this.ddl_DataInserimento.SelectedIndex = 0;
                        this.ddl_DataInserimento_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_Da_dataInserimento.Text = item.Valore;
                    }
                    #endregion DATA_INSERIMENTO_IL
                    #region DATA_INSERIMENTO_SUCCESSIVA_AL
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_SUCCESSIVA_AL.ToString())
                    {
                        if (this.ddl_DataInserimento.SelectedIndex != 1)
                            this.ddl_DataInserimento.SelectedIndex = 1;
                        this.ddl_DataInserimento_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_Da_dataInserimento.Text = item.Valore;
                    }
                    #endregion DATA_INSERIMENTO_SUCCESSIVA_AL
                    #region DATA_INSERIMENTO_PRECEDENTE_IL
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_PRECEDENTE_IL.ToString())
                    {
                        if (ddl_DataInserimento.SelectedIndex != 1)
                            ddl_DataInserimento.SelectedIndex = 1;
                        ddl_DataInserimento_SelectedIndexChanged(null, new System.EventArgs());
                        this.txt_A_dataInserimento.Text = item.Valore;
                    }
                    #endregion DATA_INSERIMENTO_PRECEDENTE_IL
                    #region DATA_INSERIMENTO_SC
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_SC.ToString() && item.Valore == "1")
                    {
                        this.ddl_DataInserimento.SelectedIndex = 3;
                        ddl_DataInserimento_SelectedIndexChanged(null, new System.EventArgs());
                    }
                    #endregion
                    #region DATA_INSERIMENTO_MC
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_MC.ToString() && item.Valore == "1")
                    {
                        this.ddl_DataInserimento.SelectedIndex = 4;
                        ddl_DataInserimento_SelectedIndexChanged(null, new System.EventArgs());
                    }
                    #endregion
                    #region DATA_INSERIMENTO_TODAY
                    else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_TODAY.ToString() && item.Valore == "1")
                    {
                        this.ddl_DataInserimento.SelectedIndex = 2;
                        ddl_DataInserimento_SelectedIndexChanged(null, new System.EventArgs());
                    }
                    #endregion
                    #endregion
                }
            }
            catch (Exception e)
            { 
            
            }
        }

        protected RubricaCallType GetCallType(string idControl)
        {
            RubricaCallType calltype = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
            if (idControl == "txtCodiceDest")
            {
                if (this.chkDestExtendHistoricized.Checked)
                {
                    calltype = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                }
                else
                {
                    calltype = RubricaCallType.CALLTYPE_CORR_INT_EST;
                }
            }
            return calltype;
        }

        protected void SearchCorrespondent(string addressCode, string idControl)
        {

            RubricaCallType calltype = GetCallType(idControl);
            Corrispondente corr = null;
            ElementoRubrica[] listaCorr = UIManager.AddressBookManager.getElementiRubricaMultipli(addressCode, calltype, true);
            bool multiCorr = false;

            if (listaCorr != null && listaCorr.Length > 0)
            {
                if (listaCorr.Length == 1)
                {
                    if (!string.IsNullOrEmpty(listaCorr[0].systemId))
                    {
                        corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(listaCorr[0].systemId);
                    }
                    else
                    {
                        corr = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(listaCorr[0].codice);
                    }
                }
                else
                {
                    //corr = null;
                    //multiCorr = true;
                    //this.FoundCorr = listaCorr;
                    //this.IdCustomObjectCustomCorrespondent = idControl;
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chooseCorrespondent", "ajaxModalPopupChooseCorrespondent();", true);
                }
            }

            if (corr == null)
            {
                switch (idControl)
                {
                    case "txtCodiceDest":
                        this.txtCodiceDest.Text = string.Empty;
                        this.txtDescrizioneDest.Text = string.Empty;
                        this.idDest.Value = string.Empty;
                        break;
                    case "txtCodiceProponente":
                        this.txtCodiceProponente.Text = string.Empty;
                        this.txtDescrizioneProponente.Text = string.Empty;
                        this.idProponente.Value = string.Empty;
                        break;
                }
            }
            else
            {
                switch (idControl)
                {
                    case "txtCodiceDest":
                        this.txtCodiceDest.Text = corr.codiceRubrica;
                        this.txtDescrizioneDest.Text = corr.descrizione;
                        this.idDest.Value = corr.systemId;

                        break;
                    case "txtCodiceProponente":
                        this.txtCodiceProponente.Text = corr.codiceRubrica;
                        this.txtDescrizioneProponente.Text = corr.descrizione;
                        this.idProponente.Value = corr.systemId;
                        break;
                }
            }

            this.upPnlDest.Update();
        }

        private void ApplyFilters()
        {
            List<ElementoInLibroFirma> tmpListElemetsFilter = new List<ElementoInLibroFirma>();
            ListaElementiFiltrati.Clear();
            foreach (DocsPaWR.FiltroElementiLibroFirma item in this.FiltersElement)
            {
                #region TIPO
                #region PROTOCOLLO_ARRIVO
                if (item.Argomento == DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString())
                {
                    if(Convert.ToBoolean(item.Valore))
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiLibroFirma
                                                 where e.InfoDocumento.TipoProto.Equals("A") && !string.IsNullOrEmpty(e.InfoDocumento.NumProto)
                                                 select e).ToList());
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                }
                #endregion
                #region PROTOCOLLO_PARTENZA
                else if (item.Argomento == DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString())
                {
                    if (Convert.ToBoolean(item.Valore))
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiLibroFirma
                                                 where e.InfoDocumento.TipoProto.Equals("P") && !string.IsNullOrEmpty(e.InfoDocumento.NumProto)
                                                 select e).ToList());
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                }
                #endregion
                #region PROTOCOLLO_INTERNO

                else if (item.Argomento == DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString())
                {
                    if (Convert.ToBoolean(item.Valore))
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiLibroFirma
                                                 where e.InfoDocumento.TipoProto.Equals("I") && !string.IsNullOrEmpty(e.InfoDocumento.NumProto)
                                                 select e).ToList());
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                }
                #endregion
                #region GRIGI
                else if (item.Argomento == DocsPaWR.FiltriDocumento.GRIGIO.ToString())
                {
                    if (Convert.ToBoolean(item.Valore))
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiLibroFirma 
                                                 where e.InfoDocumento.TipoProto.Equals("G") && string.IsNullOrEmpty(e.InfoDocumento.IdDocumentoPrincipale) select e).ToList());
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                }
                #endregion
                #region PREDISPOSTI
                else if (item.Argomento == DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString())
                {
                    if (Convert.ToBoolean(item.Valore))
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiLibroFirma
                                                 where !e.InfoDocumento.TipoProto.Equals("G") && string.IsNullOrEmpty(e.InfoDocumento.NumProto) select e).ToList());
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                }
                #endregion
                #region ALLEGATI
                else if (item.Argomento == DocsPaWR.FiltriDocumento.ALLEGATO.ToString())
                {
                    if (Convert.ToBoolean(item.Valore))
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiLibroFirma where !string.IsNullOrEmpty(e.InfoDocumento.IdDocumentoPrincipale) select e).ToList());
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                }
                #endregion
                #endregion
                #region OGGETTO
                else if (item.Argomento == DocsPaWR.FiltriDocumento.OGGETTO.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati where e.InfoDocumento.Oggetto.ToUpper().Trim().Contains(item.Valore.ToUpper().Trim()) select e).ToList());
                    this.ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion OGGETTO
                #region NUMERO_PROTOCOLLO

                #region NUM_PROTOCOLLO
                else if (item.Argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati 
                                                   where !string.IsNullOrEmpty(e.InfoDocumento.NumProto) && Convert.ToInt32(e.InfoDocumento.NumProto) == Convert.ToInt32(item.Valore) select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion NUM_PROTOCOLLO
                #region NUM_PROTOCOLLO_DAL
                else if (item.Argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiLibroFirma
                                                   where !string.IsNullOrEmpty(e.InfoDocumento.NumProto) && Convert.ToInt32(e.InfoDocumento.NumProto) >= Convert.ToInt32(item.Valore)
                                                   select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion NUM_PROTOCOLLO_DAL
                #region NUM_PROTOCOLLO_AL
                else if (item.Argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiLibroFirma
                                                   where !string.IsNullOrEmpty(e.InfoDocumento.NumProto) && Convert.ToInt32(e.InfoDocumento.NumProto) <= Convert.ToInt32(item.Valore)
                                                   select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion NUM_PROTOCOLLO_AL

                #endregion
                #region DATA_PROTOCOLLO

                #region DATA_PROT_IL
                else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where !string.IsNullOrEmpty(e.InfoDocumento.DataProtocollo) && Utils.dateformat.ConvertToDate(e.InfoDocumento.DataProtocollo).ToShortDateString().Equals(item.Valore)
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion DATA_PROT_IL
                #region DATA_PROT_SUCCESSIVA_AL
                else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where !string.IsNullOrEmpty(e.InfoDocumento.DataProtocollo) && Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.InfoDocumento.DataProtocollo).ToShortDateString(), item.Valore)    
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion DATA_PROT_SUCCESSIVA_AL
                #region DATA_PROT_PRECEDENTE_IL
                else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where !string.IsNullOrEmpty(e.InfoDocumento.DataProtocollo) && Utils.utils.verificaIntervalloDateSenzaOra(item.Valore, Utils.dateformat.ConvertToDate(e.InfoDocumento.DataProtocollo).ToShortDateString())
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion DATA_PROT_PRECEDENTE_IL
                #region DATA_PROT_SC
                else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_SC.ToString() && item.Valore == "1")
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where !string.IsNullOrEmpty(e.InfoDocumento.DataProtocollo)
                                                && Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.InfoDocumento.DataProtocollo).ToShortDateString(), NttDataWA.Utils.dateformat.getFirstDayOfWeek())
                                                && Utils.utils.verificaIntervalloDateSenzaOra(NttDataWA.Utils.dateformat.getLastDayOfWeek(), Utils.dateformat.ConvertToDate(e.InfoDocumento.DataProtocollo).ToShortDateString())
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion
                #region DATA_PROT_MC
                else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MC.ToString() && item.Valore == "1")
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where !string.IsNullOrEmpty(e.InfoDocumento.DataProtocollo)
                                                && Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.InfoDocumento.DataProtocollo).ToShortDateString(), NttDataWA.Utils.dateformat.getFirstDayOfMonth())
                                                && Utils.utils.verificaIntervalloDateSenzaOra(NttDataWA.Utils.dateformat.getLastDayOfMonth(), Utils.dateformat.ConvertToDate(e.InfoDocumento.DataProtocollo).ToShortDateString())
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion
                #region DATA_PROT_TODAY
                else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_PROT_TODAY.ToString() && item.Valore == "1")
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where !string.IsNullOrEmpty(e.InfoDocumento.DataProtocollo) && Utils.dateformat.ConvertToDate(e.InfoDocumento.DataProtocollo).ToShortDateString().Equals(NttDataWA.Utils.dateformat.getDataOdiernaDocspa())
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion
                #endregion
                #region DESTINATARIO

                else if (item.Argomento == DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString())
                {
                    List<string> idElements = LibroFirmaManager.GetElementiInLibroFirmaByDestinatario( new Corrispondente() { codiceRubrica = item.Valore });
                    if (idElements != null)
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where idElements.Contains(e.IdElemento)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                }
                else if (item.Argomento == DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString())
                {
                    List<string> idElements = LibroFirmaManager.GetElementiInLibroFirmaByDestinatario(new Corrispondente() { systemId = item.Valore });
                    if (idElements != null)
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where idElements.Contains(e.IdElemento)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                }
                else if (item.Argomento == DocsPaWR.FiltriDocumento.MITT_DEST.ToString())
                {
                    List<string> idElements = LibroFirmaManager.GetElementiInLibroFirmaByDestinatario(new Corrispondente() { descrizione = item.Valore });
                    if (idElements != null)
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where idElements.Contains(e.IdElemento)
                                                 select e).ToList());
                        ListaElementiFiltrati.Clear();
                        this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                    }
                }
                #endregion
                #region PROPONENTE

                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.CODICE_PROPONENTE.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                where e.UtenteProponente.userId.Equals(item.Valore) || e.RuoloProponente.codiceRubrica.Equals(item.Valore)
                                                select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.ID_PROPONENTE.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                where e.UtenteProponente.systemId.Equals(item.Valore) || e.RuoloProponente.systemId.Equals(item.Valore)
                                                select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DESCRIZIONE_PROPONENTE.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                where e.UtenteProponente.descrizione.ToUpper().Contains(item.Valore.ToUpper()) || e.RuoloProponente.descrizione.ToUpper().Contains(item.Valore.ToUpper())
                                                select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }

                #endregion
                #region DOCNUMBER

                #region DOCNUMBER
                else if (item.Argomento == DocsPaWR.FiltriDocumento.DOCNUMBER.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where e.InfoDocumento.Docnumber.Equals(item.Valore)
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion DOCNUMBER
                #region DOCNUMBER_DAL
                else if (item.Argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where Convert.ToInt32(e.InfoDocumento.Docnumber) >= Convert.ToInt32(item.Valore)
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion DOCNUMBER_DAL
                #region DOCNUMBER_AL
                else if (item.Argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where Convert.ToInt32(e.InfoDocumento.Docnumber) <= Convert.ToInt32(item.Valore)
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion DOCNUMBER_AL

                #endregion
                #region DATA_CREAZIONE

                #region DATA_CREAZIONE_IL
                else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where !string.IsNullOrEmpty(e.InfoDocumento.DataCreazione) && Utils.dateformat.ConvertToDate(e.InfoDocumento.DataCreazione).ToShortDateString().Equals(item.Valore)
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion DATA_CREAZIONE_IL
                #region DATA_CREAZIONE_SUCCESSIVA_AL
                else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where !string.IsNullOrEmpty(e.InfoDocumento.DataCreazione) && Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.InfoDocumento.DataCreazione).ToShortDateString(), item.Valore)
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion DATA_CREAZIONE_SUCCESSIVA_AL
                #region DATA_CREAZIONE_PRECEDENTE_IL
                else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where !string.IsNullOrEmpty(e.InfoDocumento.DataCreazione) && Utils.utils.verificaIntervalloDateSenzaOra(item.Valore, Utils.dateformat.ConvertToDate(e.InfoDocumento.DataCreazione).ToShortDateString())
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion DATA_CREAZIONE_PRECEDENTE_IL
                #region DATA_CREAZ_SC
                else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_SC.ToString() && item.Valore == "1")
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where !string.IsNullOrEmpty(e.InfoDocumento.DataCreazione)
                                                && Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.InfoDocumento.DataCreazione).ToShortDateString(), NttDataWA.Utils.dateformat.getFirstDayOfWeek())
                                                && Utils.utils.verificaIntervalloDateSenzaOra(NttDataWA.Utils.dateformat.getLastDayOfWeek(), Utils.dateformat.ConvertToDate(e.InfoDocumento.DataCreazione).ToShortDateString())
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion
                #region DATA_CREAZ_MC
                else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_MC.ToString() && item.Valore == "1")
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where !string.IsNullOrEmpty(e.InfoDocumento.DataCreazione)
                                                && Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.InfoDocumento.DataCreazione).ToShortDateString(), NttDataWA.Utils.dateformat.getFirstDayOfMonth())
                                                && Utils.utils.verificaIntervalloDateSenzaOra(NttDataWA.Utils.dateformat.getLastDayOfMonth(), Utils.dateformat.ConvertToDate(e.InfoDocumento.DataCreazione).ToShortDateString())
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion
                #region DATA_CREAZ_TODAY
                else if (item.Argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_TODAY.ToString() && item.Valore == "1")
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where !string.IsNullOrEmpty(e.InfoDocumento.DataCreazione) && Utils.dateformat.ConvertToDate(e.InfoDocumento.DataCreazione).ToShortDateString().Equals(NttDataWA.Utils.dateformat.getDataOdiernaDocspa())
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion
                #endregion
                #region TIPOLOGIA

                else if (item.Argomento == DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where e.InfoDocumento.TipologiaDocumento.Equals(item.Valore)
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion
                #region TIPO FIRMA
                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.FIRMA_DIGITALE_CADES.ToString())
                {
                    tmpListElemetsFilter.Clear();
                    if (Convert.ToBoolean(item.Valore))
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where e.TipoFirma.Equals(LibroFirmaManager.TypeEvent.SIGN_CADES)
                                                 select e).ToList());
                    }
                }
                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.FIRMA_DIGITALE_PADES.ToString())
                {
                    if (Convert.ToBoolean(item.Valore))
                    {
                        tmpListElemetsFilter.AddRange((from e in this.ListaElementiFiltrati
                                                       where e.TipoFirma.Equals(LibroFirmaManager.TypeEvent.SIGN_PADES)
                                                 select e).ToList());
                    }
                }

                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.FIRMA_ELETTRONICA_SOTTOSCRIZIONE.ToString())
                {
                    if (Convert.ToBoolean(item.Valore))
                    {
                        tmpListElemetsFilter.AddRange((from e in this.ListaElementiFiltrati
                                                       where e.TipoFirma.Equals(LibroFirmaManager.TypeEvent.VERIFIED)
                                                         select e).ToList());
                    }
                }
                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.FIRMA_ELETTRONICA_AVANZAMENTO.ToString())
                {
                    if (Convert.ToBoolean(item.Valore))
                    {
                        tmpListElemetsFilter.AddRange((from e in this.ListaElementiFiltrati
                                                       where e.TipoFirma.Equals(LibroFirmaManager.TypeEvent.ADVANCEMENT_PROCESS)
                                                       select e).ToList());
                    }
                    this.ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion
                #region MODALITA

                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.MODALITA_AUTOMATICA.ToString())
                {
                    tmpListElemetsFilter.Clear();
                    if (Convert.ToBoolean(item.Valore))
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where e.Modalita.Equals("A")
                                                 select e).ToList());
                    }
                }
                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.MODALITA_MANUALE.ToString())
                {
                    if (Convert.ToBoolean(item.Valore))
                    {
                        tmpListElemetsFilter.AddRange((from e in this.ListaElementiFiltrati
                                                       where e.Modalita.Equals("M")
                                                       select e).ToList());
                    }
                    this.ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }

                #endregion
                #region STATO
                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.PROPOSTO.ToString())
                {
                    tmpListElemetsFilter.Clear();
                    if (Convert.ToBoolean(item.Valore))
                    {
                        tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                                 where e.StatoFirma.Equals (TipoStatoElemento.PROPOSTO)
                                                 select e).ToList());
                    }
                }
                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DA_FIRMARE.ToString())
                {
                    if (Convert.ToBoolean(item.Valore))
                    {
                        tmpListElemetsFilter.AddRange((from e in this.ListaElementiFiltrati
                                                       where e.StatoFirma.Equals(TipoStatoElemento.DA_FIRMARE)
                                                 select e).ToList());
                    }
                }

                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DA_RESPINGERE.ToString())
                {
                    if (Convert.ToBoolean(item.Valore))
                    {
                        tmpListElemetsFilter.AddRange((from e in this.ListaElementiFiltrati
                                                       where e.StatoFirma.Equals(TipoStatoElemento.DA_RESPINGERE)
                                                 select e).ToList());
                    }
                    this.ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion
                #region STATO CON_ERRORI/SENZA ERRORI

                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.CON_ERRORI.ToString())
                {
                    tmpListElemetsFilter.Clear();
                    if (Convert.ToBoolean(item.Valore))
                    {
                        tmpListElemetsFilter.AddRange((from e in this.ListaElementiFiltrati
                                                       where !string.IsNullOrEmpty(e.ErroreFirma)
                                                       select e).ToList());
                    }
                }
                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.SENZA_ERRORI.ToString())
                {
                    if (Convert.ToBoolean(item.Valore))
                    {
                        tmpListElemetsFilter.AddRange((from e in this.ListaElementiFiltrati
                                                       where string.IsNullOrEmpty(e.ErroreFirma)
                                                       select e).ToList());
                    }
                    this.ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                
                #endregion
                #region NOTE

                else if (item.Argomento == DocsPaWR.FiltriDocumento.NOTE.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where e.Note.ToUpper().Trim().Contains(item.Valore.ToUpper().Trim())
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }

                #endregion
                #region DATA_INSERIMENTO

                #region DATA_INSERIMENTO_IL
                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_IL.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where Utils.dateformat.ConvertToDate(e.DataInserimento).ToShortDateString().Equals(item.Valore)
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion DATA_INSERIMENTO_IL
                #region DATA_INSERIMENTO_SUCCESSIVA_AL
                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_SUCCESSIVA_AL.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.DataInserimento).ToShortDateString(), item.Valore)
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion DATA_INSERIMENTO_SUCCESSIVA_AL
                #region DATA_INSERIMENTO_PRECEDENTE_IL
                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_PRECEDENTE_IL.ToString())
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where Utils.utils.verificaIntervalloDateSenzaOra(item.Valore, Utils.dateformat.ConvertToDate(e.DataInserimento).ToShortDateString())
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion DATA_INSERIMENTO_PRECEDENTE_IL
                #region DATA_INSERIMENTO_SC
                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_SC.ToString() && item.Valore == "1")
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.DataInserimento).ToShortDateString(), NttDataWA.Utils.dateformat.getFirstDayOfWeek())
                                                && Utils.utils.verificaIntervalloDateSenzaOra(NttDataWA.Utils.dateformat.getLastDayOfWeek(), Utils.dateformat.ConvertToDate(e.DataInserimento).ToShortDateString())
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion
                #region DATA_INSERIMENTO_MC
                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_MC.ToString() && item.Valore == "1")
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(e.DataInserimento).ToShortDateString(), NttDataWA.Utils.dateformat.getFirstDayOfMonth())
                                                && Utils.utils.verificaIntervalloDateSenzaOra(NttDataWA.Utils.dateformat.getLastDayOfMonth(), Utils.dateformat.ConvertToDate(e.DataInserimento).ToShortDateString())
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion
                #region DATA_INSERIMENTO_TODAY
                else if (item.Argomento == DocsPaWR.FiltriElementoLibroFirma.DATA_INSERIMENTO_TODAY.ToString() && item.Valore == "1")
                {
                    tmpListElemetsFilter = ((from e in this.ListaElementiFiltrati
                                             where Utils.dateformat.ConvertToDate(e.DataInserimento).ToShortDateString().Equals(NttDataWA.Utils.dateformat.getDataOdiernaDocspa())
                                             select e).ToList());
                    ListaElementiFiltrati.Clear();
                    this.ListaElementiFiltrati.AddRange(tmpListElemetsFilter);
                }
                #endregion
                #endregion
            }
        }
        #endregion

    }
}
