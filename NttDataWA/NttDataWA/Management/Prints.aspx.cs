using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;

namespace NttDataWA.Management
{
    public partial class Prints : System.Web.UI.Page
    {
        #region const
        private const string UP_PANEL_BUTTONS = "upPnlButtons";
        #endregion

        #region global variable
        protected DocsPaWR.RagioneTrasmissione[] listaRagioni;
        private DocsPaWR.Registro[] userRegistri;
        protected Hashtable m_hashTableRagioneTrasmissione;
        protected DocsPaWR.FiltroRicerca[][] qV;
        protected DocsPaWR.FiltroRicerca fV1;
        protected DocsPaWR.FiltroRicerca[] fVList;

        #endregion

        #region Property

        /// <summary>
        /// Registro selezionato nella griglia
        /// </summary>
        private Registro SelectedRegister
        {
            get
            {
                if (HttpContext.Current.Session["selectedRegister"] != null)
                    return (Registro)HttpContext.Current.Session["selectedRegister"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["selectedRegister"] = value;
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

        /// <summary>
        /// Id corrispondente all'oggetto InfoCheckMailbox 
        /// </summary>
        private string IdCheckMailbox
        {
            get
            {
                if (HttpContext.Current.Session["idCheckMailbox"] != null)
                    return (String)HttpContext.Current.Session["idCheckMailbox"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["idCheckMailbox"] = value;
            }
        }

        private bool AlreadyDownloaded
        {
            get
            {
                if (HttpContext.Current.Session["visualReportAlreadyDownloaded" + Session.SessionID] != null)
                    return (bool)HttpContext.Current.Session["visualReportAlreadyDownloaded" + Session.SessionID];
                else
                    return false;
            }
            set
            {
                HttpContext.Current.Session["visualReportAlreadyDownloaded" + Session.SessionID] = value;
            }
        }

        private Registro[] ListRegisters
        {
            get
            {
                if (HttpContext.Current.Session["listRegisters"] != null)
                    return (Registro[])HttpContext.Current.Session["listRegisters"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["listRegisters"] = value;
            }
        }


        /// <summary>
        ///  Contiene il file doc per la stampa registri
        /// </summary>
        private FileDocumento FileDocPrintRegister
        {
            get
            {
                return HttpContext.Current.Session["fileDocPrintRegister"] as FileDocumento;
            }
            set
            {
                HttpContext.Current.Session["fileDocPrintRegister"] = value;
            }
        }

        #endregion

        #region Standard Method

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ClearSession();
                    InitializeLanguage();
                    //userRegistri = UserManager.getListaRegistri(this);
                    userRegistri = RoleManager.GetRoleInSession().registri;
                    this.CaricaComboRegistri(ddl_registri);
                    //carica il ruolo scelto
                    DocsPaWR.Ruolo ruolo = UserManager.GetSelectedRole();
                    this.getAutorizzazioniReport(ruolo);
                    this.SetAjaxAddressBook();
                }
                else
                {
                    this.ReadRetValueFromPopup();
                }
                RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ClearSession()
        {
            HttpContext.Current.Session.Remove("listRegisters");
            HttpContext.Current.Session.Remove("selectedRegister");
            HttpContext.Current.Session["userRegistry"] = null;
        }


        protected void SetAjaxAddressBook()
        {
            string dataUser = RoleManager.GetRoleInSession().systemId;
            dataUser = dataUser + "-" + this.ddl_registri.SelectedValue;

            string callType = "CALLTYPE_STAMPA_REG_UO";
            this.RapidCreatore.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
        }


        protected void ReadRetValueFromPopup()
        {
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('visualReport','')", true);
            //return;  
        }

        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeIframe", "resizeIframe();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);

        }

        /// <summary>
        /// Language Manager
        /// </summary>
        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnPrint.Text = Utils.Languages.GetLabelFromCode("RegistersBtnPrint", language);
            //this.ViewPrintRegister.Title = Utils.Languages.GetLabelFromCode("ViewPrintRegisterTitle", language);
            //lblSelectReport.Text = Utils.Languages.GetLabelFromCode("lblSelectReportText", language);
            this.LitSearchDocumentFrom.Text = Utils.Languages.GetLabelFromCode("LitSearchDocumentFrom", language);
            this.LitSearchDocumentTo.Text = Utils.Languages.GetLabelFromCode("LitSearchDocumentTo", language);
            this.lblRegistro.Text = Utils.Languages.GetLabelFromCode("LtlRegistro", language);
            this.ManagementPrintsLabel.Text = Utils.Languages.GetLabelFromCode("ManagementPrintsLabel", language);
            lblOggettoTrasmesso.Text = Utils.Languages.GetLabelFromCode("lblOggettoTrasmesso", language);
            lblDataTrasmissione.Text = Utils.Languages.GetLabelFromCode("lblDataTrasmissione", language);
            lit_initdataProt_E.Text = Utils.Languages.GetLabelFromCode("LitSearchDocumentFrom", language);
            lit_finedataProt_E.Text = Utils.Languages.GetLabelFromCode("LitSearchDocumentTo", language);
            lit_initIdDoc_E.Text = Utils.Languages.GetLabelFromCode("LitSearchDocumentFrom", language);
            lit_fineIdDoc_E.Text = Utils.Languages.GetLabelFromCode("LitSearchDocumentTo", language);
            lblDAnumprot_B.Text = Utils.Languages.GetLabelFromCode("LitSearchDocumentFrom", language);
            lblAnumprot_B.Text = Utils.Languages.GetLabelFromCode("LitSearchDocumentTo", language);
            lit_initDataCreazioneG_E.Text = Utils.Languages.GetLabelFromCode("LitSearchDocumentFrom", language);
            lit_fineDataCreazioneG_E.Text = Utils.Languages.GetLabelFromCode("LitSearchDocumentTo", language);
            lit_initdataProt_B.Text = Utils.Languages.GetLabelFromCode("LitSearchDocumentFrom", language);
            lit_finedataProt_B.Text = Utils.Languages.GetLabelFromCode("LitSearchDocumentTo", language);
            lit_rep_av_da.Text = Utils.Languages.GetLabelFromCode("LitSearchDocumentFrom", language);
            lit_rep_av_a.Text = Utils.Languages.GetLabelFromCode("LitSearchDocumentTo", language);
            this.litUO.Text = Utils.Languages.GetLabelFromCode("litUO2", language);
            AddFilterLblReasonTransmission.Text = Utils.Languages.GetLabelFromCode("AddFilterLblReasonTransmission", language);
            LblAddFilterNumProtocol.Text = Utils.Languages.GetLabelFromCode("LblAddFilterNumProtocol", language);
            DocumentLitTypeDocument.Text = Utils.Languages.GetLabelFromCode("DocumentLitTypeDocument", language);
            DocumentLitTypeDocument1.Text = Utils.Languages.GetLabelFromCode("DocumentLitTypeDocument", language);
            LblAddFilterDateProtocol.Text = Utils.Languages.GetLabelFromCode("LblAddFilterDateProtocol", language);
            this.chk_uo.Text = Utils.Languages.GetLabelFromCode("chkUO", language);
            this.ImgProprietarioAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("PrintUOAddressBook", language);
            this.PrintsTitle.Text = Utils.Languages.GetLabelFromCode("PrintsTitle", language);
            this.lblDAnumprot_E.Text = Utils.Languages.GetLabelFromCode("LitSearchDocumentFrom", language);
            this.lblAnumprot_E.Text = Utils.Languages.GetLabelFromCode("LitSearchDocumentTo", language);
            this.LblAddDocNumDoc.Text = Utils.Languages.GetLabelFromCode("LblAddDocNumDoc", language);
            this.LblAddDocNumDoc1.Text = Utils.Languages.GetLabelFromCode("LblAddFilterNumProtocol", language);
            this.LblAddDocDtaDoc.Text = Utils.Languages.GetLabelFromCode("LblAddDocDtaDoc", language);
            this.LblAddDocDtaDoc1.Text = Utils.Languages.GetLabelFromCode("LblAddFilterDateProtocol", language);
            this.LitAnnoSearchProject.Text = Utils.Languages.GetLabelFromCode("LitAnnoSearchProject", language);
            TypeRole.Text = Utils.Languages.GetLabelFromCode("TypeRole", language);
            TransmissionLitReasonExtended.Text = Utils.Languages.GetLabelFromCode("TransmissionLitReasonExtended", language);
            ReferDate.Text = Utils.Languages.GetLabelFromCode("ReferDate", language);
            ddl_report.Items[0].Text = Utils.Languages.GetLabelFromCode("SearchProjectTitolario", language);
            ddl_report.Items[0].Value = "T";
            ddl_report.Items[1].Text = Utils.Languages.GetLabelFromCode("ExternCorrispondents", language);
            ddl_report.Items[1].Value = "E";
            ddl_report.Items[2].Text = Utils.Languages.GetLabelFromCode("TrasmissionsUO", language);
            ddl_report.Items[2].Value = "TR";
            ddl_report.Items[3].Text = Utils.Languages.GetLabelFromCode("DocumentsRegister", language);
            ddl_report.Items[3].Value = "DR";
            ddl_report.Items[4].Text = Utils.Languages.GetLabelFromCode("DocumentsNotProt", language);
            ddl_report.Items[4].Value = "DG";
            ddl_report.Items[5].Text = Utils.Languages.GetLabelFromCode("Buste", language);
            ddl_report.Items[5].Value = "B";
            ddl_report.Items[6].Text = Utils.Languages.GetLabelFromCode("FascetteFascicolo", language);
            ddl_report.Items[6].Value = "F";
            DDLOggettoTab1.Items[0].Text = Utils.Languages.GetLabelFromCode("DigitalSignDialogIdDoc", language);
            DDLOggettoTab1.Items[1].Text = Utils.Languages.GetLabelFromCode("Fascicolo", language);
            ddl_dataTrasm.Items[0].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            ddl_dataTrasm.Items[1].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            ddl_dataTrasm.Items[2].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            ddl_dataTrasm.Items[3].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            ddl_dataTrasm.Items[4].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            ddl_numProt_E.Items[0].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            ddl_numProt_E.Items[1].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            ddl_dataProt_E.Items[0].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            ddl_dataProt_E.Items[1].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            ddl_dataProt_E.Items[2].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            ddl_dataProt_E.Items[3].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            ddl_dataProt_E.Items[4].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            ddl_idDoc_E.Items[0].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            ddl_idDoc_E.Items[1].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            ddl_dataCreazioneG_E.Items[0].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            ddl_dataCreazioneG_E.Items[1].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            ddl_dataCreazioneG_E.Items[2].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            ddl_dataCreazioneG_E.Items[3].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            ddl_dataCreazioneG_E.Items[4].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            ddl_numProt_B.Items[0].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            ddl_numProt_B.Items[1].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            ddl_dataProt_B.Items[0].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            ddl_dataProt_B.Items[1].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            ddl_dataProt_B.Items[2].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            ddl_dataProt_B.Items[3].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            ddl_dataProt_B.Items[4].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            ddl_rep_av_data.Items[0].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            ddl_rep_av_data.Items[1].Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.LitPrintsManagementSelect.Text = Utils.Languages.GetLabelFromCode("LitPrintsManagementSelect", language);
            this.ddl_ragioni.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("AddFilterDdlReasonTransmission", language));
            this.ddl_tipoAttoDR.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DocumentDdlTypeDocument", language));
            this.ddl_tipoAttoDG.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DocumentDdlTypeDocument", language));
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("DocumentImgCustomCorrespondentAddressBookCustom", language);
            this.visualReport_iframe.Title = Utils.Languages.GetLabelFromCode("PrintsTitle", language);
            this.TransmissionsPrint.Title = Utils.Languages.GetLabelFromCode("PrintsTitle", language);
            this.ExportDati.Title = Utils.Languages.GetLabelFromCode("SearchDocumentExportDatiTitle", language);
        }

        private void SetDataInterval()
        {
            this.TxtDateTo.Text = UIManager.AdministrationManager.ToDay();
            this.TxtDateFrom.Text = UIManager.AdministrationManager.DayOnYearBeforeToday();
        }
        #endregion



        #region Event Handler
        private void getAutorizzazioniReport(DocsPaWR.Ruolo ruolo)
        {
            ListItem newItem;
            string[] arrFunzione = new string[3];

            for (int i = 1; i < 5; i++)
            {
                arrFunzione = this.datiFunzioneAvanzata(i);

                if (UserManager.ruoloIsAutorized(this, arrFunzione[0]))
                {
                    newItem = new ListItem();
                    newItem.Text = arrFunzione[1];
                    newItem.Value = arrFunzione[2];
                    this.ddl_report.Items.Add(newItem);
                }
            }
        }
        private string[] datiFunzioneAvanzata(int index)
        {
            string[] retValue = new string[3];

            switch (index)
            {
                case 1:
                    retValue[0] = "DO_TRASM_RUOLO_STAMPA";
                    retValue[1] = "Numero di trasmissioni effettuate da ruolo con dettaglio utente";
                    retValue[2] = "TX_R";
                    break;
                case 2:
                    retValue[0] = "DO_TRASM_PEND_STAMPA";
                    retValue[1] = "Numero di trasmissioni pendenti con dettaglio del ruolo";
                    retValue[2] = "TX_P";
                    break;
                case 3:
                    retValue[0] = "DO_PROT_REG_STAMPA";
                    retValue[1] = "Numero di documenti protocollati per registro";
                    retValue[2] = "PR_REG";
                    break;
                case 4:
                    retValue[0] = "DO_PROT_REG_RUOLO_STAMPA";
                    retValue[1] = "Numero di documenti protocollati per registro con dettaglio ruolo";
                    retValue[2] = "PR_REG_R";
                    break;
            }

            return retValue;
        }
        private void CaricaComboRegistri(DropDownList ddl)
        {
            if (userRegistri == null)
            {
                //userRegistri = UserManager.getListaRegistri(this);
                userRegistri = RoleManager.GetRoleInSession().registri;
            }
            string stato = string.Empty;
            for (int i = 0; i < userRegistri.Length; i++)
            {
                stato = UserManager.getStatoRegistro(userRegistri[i]);

                ddl.Items.Add(userRegistri[i].codRegistro);
                ddl.Items[i].Value = userRegistri[i].systemId;
            }

            //setto lo stato del registro
            if (userRegistri.Length > 0)
            {
                setStatoReg(userRegistri[0]);
            }

        }
        protected void BtnPrint_Click(object sender, EventArgs e)
        {
            string msgDesc = "";
            DocsPaWR.Registro registro = null;
            if (userRegistri == null)
            {
                //userRegistri = UserManager.getListaRegistri(this);
                userRegistri = RoleManager.GetRoleInSession().registri;
            }

            if (this.pnl_trasmUO.Visible)
            {
                //Controllo intervallo date trasmissione
                if (this.ddl_dataTrasm.SelectedIndex != 0)
                {
                    if (utils.isDate(this.TxtDateTo.Text) && utils.isDate(this.TxtDateFrom.Text) && utils.verificaIntervalloDate(this.TxtDateFrom.Text, this.TxtDateTo.Text))
                    {
                        msgDesc = "WarningPrintsDateGeneric";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                }
            }

            if (this.pnl_DocumentiRegistro.Visible)
            {
                //Controllo intervallo date protocollo
                if (this.ddl_dataProt_E.SelectedIndex != 0)
                {
                    if (utils.isDate(this.txt_initDataProt_E.Text) && utils.isDate(this.txt_fineDataProt_E.Text) && utils.verificaIntervalloDate(this.txt_initDataProt_E.Text, this.txt_fineDataProt_E.Text))
                    {
                        msgDesc = "WarningPrintsDateGeneric";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                }
            }

            if (this.pnl_DocumentiGrigi.Visible)
            {
                //Controllo intervallo date creazione
                if (this.ddl_dataCreazioneG_E.SelectedIndex != 0)
                {
                    if (utils.isDate(this.txt_initDataCreazioneG_E.Text) && utils.isDate(this.txt_fineDataCreazioneG_E.Text) && utils.verificaIntervalloDate(this.txt_initDataCreazioneG_E.Text, this.txt_fineDataCreazioneG_E.Text))
                    {
                        msgDesc = "WarningPrintsDateGeneric";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                }
            }

            if (this.pnl_StampaBuste.Visible)
            {
                //Controllo intervallo date protocollo
                if (this.ddl_dataProt_B.SelectedIndex != 0)
                {
                    if (utils.isDate(this.txt_initDataProt_B.Text) && utils.isDate(this.txt_fineDataProt_B.Text) && utils.verificaIntervalloDate(this.txt_initDataProt_B.Text, this.txt_fineDataProt_B.Text))
                    {
                        msgDesc = "WarningPrintsDateGeneric";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                }
            }

            if (this.pnl_reportAvanzati.Visible)
            {
                //Controllo intervallo date di riferimento
                if (this.ddl_rep_av_data.SelectedIndex != 0)
                {
                    if (utils.isDate(this.txt_rep_av_initData.Text) && utils.isDate(this.txt_rep_av_fineData.Text) && utils.verificaIntervalloDate(this.txt_rep_av_initData.Text, this.txt_rep_av_fineData.Text))
                    {
                        msgDesc = "WarningPrintsDateGeneric";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                }
            }

            DocsPaWR.FileDocumento fileRep = null;
            string registri = "";

            string tipoReport = this.ddl_report.SelectedValue;
            switch (tipoReport)
            {
                case "TX_R":
                case "TX_P":
                case "PR_REG":
                case "PR_REG_R":
                    this.impostaDatiRepAv();
                    return;
            }

            if (ddl_registri.SelectedItem.Text == "Tutti" && tipoReport == "E")
            {
                foreach (DocsPaWR.Registro reg in userRegistri)
                {
                    registri += reg.systemId + ",";
                }
                registri = registri.Substring(0, registri.Length - 1);
            }
            else
            {
                registro = userRegistri[ddl_registri.SelectedIndex];
                RegistryManager.SetRegistryInSession(registro);
            }
            InfoUtente infoUtente = UserManager.GetInfoUser();
            Ruolo ruolo = RoleManager.GetRoleInSession();

            switch (tipoReport)
            {
                case "T": fileRep = ProjectManager.reportTitolario(registro); break;
                case "F": fileRep = ProjectManager.reportFascette(this.txtInput.Text, registro); break;
                case "C": fileRep = UserManager.reportCorrispondenti(this, tipoReport, registro); break;
                case "I": fileRep = UserManager.reportCorrispondenti(this, tipoReport, registro); break;
                case "E":
                    registri = "";
                    bool store = false;

                    fileRep = AddressBookManager.ExportRubrica(infoUtente, store, registri);
                    break;
                case "TR":
                    if (!ricercaTrasmissioni())
                    {
                        msgDesc = "WarningPrintsCriteriGeneric";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                    fileRep = TrasmManager.getReportTrasmUO(this, fVList, UserManager.GetSelectedRole().uo.descrizione);
                    break;
                case "DR":
                    // Stampa documenti registro
                    if (!CheckRequiredFieldsStampaRegistro())
                    {
                        msgDesc = "WarningPrintsCriteriGeneric";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }

                    string rtn = IsValidControlsProtocollo();
                    if (rtn != "")
                    {
                        msgDesc = "WarningPrintsCriteriGeneric";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }

                    // Costruzione oggetto filtro
                    this.BuildFiltroDocumentiRegistro();

                    try
                    {

                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ExportDati", "ajaxModalPopupExportDati();", true);
                    }
                    catch (System.Exception ex)
                    {
                        UIManager.AdministrationManager.DiagnosticError(ex);
                        return;
                    }

                    infoUtente = null;
                    ruolo = null;

                    break;

                case "DG":
                    // Stampa documenti grigi
                    if (!CheckRequiredFieldsDocumentiGrigi())
                    {
                        msgDesc = "WarningPrintsCriteriGeneric";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }

                    string result = IsValidControlsDocumentiGrigi();
                    if (result != "")
                    {
                        msgDesc = "WarningPrintsCriteriGeneric";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }

                    // Costruzione oggetto filtro
                    this.BuildFiltroDocumentiGrigi();
                    //DocumentManager.setFiltroRicDoc(this, qV);

                    //ClientScript.RegisterStartupScript(this.GetType(), "stampaRisultati_DG", "StampaRisultatoRicerca();", true);
                    try
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ExportDati", "ajaxModalPopupExportDati();", true);
                    }
                    catch (System.Exception ex)
                    {
                        UIManager.AdministrationManager.DiagnosticError(ex);
                        return;
                    }
                    infoUtente = null;
                    ruolo = null;

                    break;

                case "B":
                    // Stampa Buste
                    if (!CheckRequiredFieldsStampaBuste())
                    {
                        msgDesc = "WarningPrintsCriteriGeneric";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }

                    if (!IsValidControlsStampaBuste())
                    {
                        msgDesc = "WarningPrintsCriteriGeneric";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }

                    //Costruzione oggetto filtro
                    this.BuildFiltroDocumentiStampaBuste();
                    fileRep = GestManager.GetReportBusteWithFilters(this, infoUtente, ruolo, registro, qV);

                    infoUtente = null;
                    ruolo = null;

                    break;
            }

            if (fileRep != null)
            {
                this.Session["FileManager.selectedReport"] = fileRep;


                if (fileRep != null)
                {
                    exportDatiSessionManager session = new exportDatiSessionManager();
                    session.SetSessionExportFile(fileRep);
                }

                //if (tipoReport.Equals("F"))
                //{
                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "stampaFascette", "StampaFascette();", true);

                //}
                //else
                //{
                    if (tipoReport.Equals("TR"))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "TransmissionsPrint", "ajaxModalPopupTransmissionsPrint();", true);
                    }
                    else
                    {
                        this.AlreadyDownloaded = false;        
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Prints", "ajaxModalPopupvisualReport_iframe();", true);
                    }

                //}
            }
            else
            {
                switch (tipoReport)
                {
                    //In questi due casi non fa nulla perchè è la popup di selezione campi che fa la ricerca
                    //ed eventualmente comunica che non sono stati trovati documenti
                    case "DG":
                    case "DR":
                        break;

                    default:
                        msgDesc = "WarningPrintsNoDatiNoReport";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        break;

                }
            }

        }
        private bool ricercaTrasmissioni()
        {
            bool res = true;
            try
            {
                //array contenitore degli array filtro di ricerca
                qV = new DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPaWR.FiltroRicerca[1];

                fVList = new DocsPaWR.FiltroRicerca[0];
                #region  filtro UO
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.ID_UO.ToString();
                fV1.valore = UserManager.GetSelectedRole().uo.systemId;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                res = checkData(fV1.argomento, fV1.valore);
                #endregion

                #region filtro "oggetto trasmesso"
                if (this.DDLOggettoTab1.SelectedIndex >= 0)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString();
                    fV1.valore = this.DDLOggettoTab1.SelectedItem.Value.ToString();
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region  filtro sulla ragione
                if (ddl_ragioni.SelectedIndex > 0)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.RAGIONE.ToString();
                    fV1.valore = ddl_ragioni.SelectedValue.ToString();
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region  filtro sulla data invio INIZIO
                if (!this.TxtDateFrom.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    if (this.ddl_dataTrasm.SelectedIndex.Equals(0))
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_IL.ToString();
                    else
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_SUCCESSIVA_AL.ToString();
                    fV1.valore = TxtDateFrom.Text.ToString();
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    res = checkData(fV1.argomento, fV1.valore);
                }
                #endregion

                #region  filtro sulla data invio FINE
                if (!this.TxtDateTo.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_PRECEDENTE_IL.ToString();
                    fV1.valore = TxtDateTo.Text.ToString();
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    res = checkData(fV1.argomento, fV1.valore);
                }
                #endregion

                qV[0] = fVList;
                this.SearchFilters = qV;
                return res;
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
                res = false;
                return res;
            }
        }
        private bool checkData(string argomento, string valore)
        {
            if (argomento.Equals("TRASMISSIONE_IL") || argomento.Equals("TRASMISSIONE_SUCCESSIVA_AL") || argomento.Equals("TRASMISSIONE_PRECEDENTE_IL"))
            {
                if (!utils.isDate(valore))
                    return false;
            }
            return true;
        }

        private void impostaDatiRepAv()
        {
            //try
            //{
            // verifica le date inserite
            //if (
            //        (this.ddl_rep_av_data.SelectedValue.Equals("0") &&  // è stato selezionato 'valore singolo'
            //         this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text.Trim() != "" &&      // ed il campo data non è vuoto
            //         Utils.isDate(this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text))       // ed è un formato data...
            //         ||                                                 //                              ... oppure
            //        (this.ddl_rep_av_data.SelectedValue.Equals("1") &&                                                  // è stato selezionato 'intervallo'
            //         ((this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text.Trim() != "" && Utils.isDate(this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text)) &&    // ed il campo data 'Da' non è vuoto ed è un formato data
            //          (this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Text.Trim() != "" && Utils.isDate(this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Text))       // ed il campo data 'A' non è vuoto ed è un formato data
            //         )
            //        )
            //    )
            //{
            string tipoReport = this.ddl_report.SelectedValue;

            Ruolo ruolo = UserManager.GetSelectedRole();
            ExportExcelClass objReport = new ExportExcelClass();
            ExportDataFilterExcel objFilter = new ExportDataFilterExcel();

            objFilter.tipologiaReport = tipoReport;
            //objFilter.idRegistro = this.ddl_registri.SelectedValue;
            objFilter.idAmministrazione = ruolo.idAmministrazione;

            // memorizza i filtri
            switch (tipoReport)
            {
                case "TX_R":
                case "TX_P":
                    objFilter.idRuolo = this.ddl_rep_av_ruolo.SelectedValue;
                    objFilter.idRagTrasm = this.ddl_rep_av_rag_trasm.SelectedValue;
                    //if (this.GetCalendarControl("txt_rep_av_initData").Visible)
                    //    objFilter.dataDa = this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text;
                    //if (this.GetCalendarControl("txt_rep_av_fineData").Visible)
                    //    objFilter.dataA = this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Text;
                    break;
                case "PR_REG":
                    //if (this.GetCalendarControl("txt_rep_av_initData").Visible)
                    //    objFilter.dataDa = this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text;
                    //if (this.GetCalendarControl("txt_rep_av_fineData").Visible)
                    //    objFilter.dataA = this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Text;
                    break;
                case "PR_REG_R":
                    objFilter.idRuolo = this.ddl_rep_av_ruolo.SelectedValue;
                    //if (this.GetCalendarControl("txt_rep_av_initData").Visible)
                    //    objFilter.dataDa = this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text;
                    //if (this.GetCalendarControl("txt_rep_av_fineData").Visible)
                    //    objFilter.dataA = this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Text;
                    break;
            }

            objReport.filtro = objFilter;

            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            ws.Timeout = System.Threading.Timeout.Infinite;
            objReport.file = ws.stampaReportAvanzatiXLS(objReport);

            if (objReport.file != null && objReport.file.length > 0)
            {
                // imposta la sessione
                //         gestione.report.stampaReportXLS.stampaReportXLS_Session sessioneStampaReportAv = new stampaReportXLS.stampaReportXLS_Session();
                //  sessioneStampaReportAv.SetSessionReportXLS(objReport);

                // chiama la pagina che visualizza il report in Excel                    

                //this.RegisterClientScript("Rep", "OpenFileXLS();");
            }
            //    else
            //        RegisterClientScript("RepNOFile", "alert('Nessun dato trovato!\\n\\nImpostare un diverso criterio di ricerca.');");
            //}
            //else
            //    RegisterClientScript("RepAlert", "alert('Attenzione! verificare la correttezza delle date inserite.');");
            //}
            //catch (System.Exception ex)
            //{
            //    RegisterClientScript("RepAlert", "alert('Attenzione! si è verificato un errore di sistema durante la generazione del report');");
            //    logger.Error("Generazione del file per la stampa report in ERRORE: " + ex.ToString());
            //}
        }

        private void CaricaRagioni()
        {
            this.ddl_ragioni.Items.Clear();

            listaRagioni = TrasmManager.getListaRagioni(this, String.Empty, false);

            if (listaRagioni != null && listaRagioni.Length > 0)
            {
                ddl_ragioni.Items.Add("");
                for (int i = 0; i < listaRagioni.Length; i++)
                {
                    ListItem newItem = new ListItem(listaRagioni[i].descrizione, listaRagioni[i].systemId);
                    ddl_ragioni.Items.Add(newItem);
                }
                this.ddl_ragioni.SelectedIndex = 0;
            }
        }
        private void CaricaComboTipologiaAtto(DropDownList ddl)
        {
            DocsPaWR.TipologiaAtto[] listaTipologiaAtto;
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1" && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null)
            {
                listaTipologiaAtto = DocumentManager.getTipoAttoPDInsRic(this, UserManager.GetInfoUser().idAmministrazione, UserManager.GetSelectedRole().idGruppo, "1");
            }
            else
            {
                listaTipologiaAtto = DocumentManager.getListaTipologiaAtto(this);
            }

            ddl.Items.Clear();
            //aggiunge una riga vuota alla combo
            ddl.Items.Add("");
            if (listaTipologiaAtto != null)
            {
                for (int i = 0; i < listaTipologiaAtto.Length; i++)
                {
                    ddl.Items.Add(listaTipologiaAtto[i].descrizione);
                    ddl.Items[i + 1].Value = listaTipologiaAtto[i].systemId;
                }
            }

            //btn_CampiPersonalizzatiDR.Visible = false;
            //btn_CampiPersonalizzatiDG.Visible = false;
        }
        private void toglieVociTuttiRegistri()
        {
            if (this.ddl_registri.Items.FindByText("Tutti") != null)
                this.ddl_registri.Items.RemoveAt(this.ddl_registri.Items.Count - 1);
        }
        protected void ddl_report_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //rimuove da sessione eventuale selezione uo per stampa registri uo.
                System.Web.HttpContext.Current.Session.Remove("corrStampaUo");

                string tipoReport = this.ddl_report.SelectedValue;
                switch (tipoReport)
                {
                    case "F": // Fascette Fascicolo
                        this.pnlInput.Visible = true;
                        this.pnl_DocumentiRegistro.Visible = false;
                        this.pnl_StampaBuste.Visible = false;
                        this.pnl_trasmUO.Visible = false;
                        this.pnl_reportAvanzati.Visible = false;
                        this.pnl_DocumentiGrigi.Visible = false;
                        this.TxtDateFrom.Visible = false;
                        this.TxtDateTo.Visible = false;
                        LitSearchDocumentFrom.Visible = false;
                        LitSearchDocumentTo.Visible = false;
                        this.toglieVociTuttiRegistri();
                        break;
                    case "TR": // Trasmissioni UO
                        this.pnlInput.Visible = false;
                        this.pnl_DocumentiRegistro.Visible = false;
                        this.pnl_StampaBuste.Visible = false;
                        this.pnl_trasmUO.Visible = true;
                        this.pnl_reportAvanzati.Visible = false;
                        this.pnl_DocumentiGrigi.Visible = false;
                        LitSearchDocumentFrom.Visible = false;
                        LitSearchDocumentTo.Visible = false;
                        this.TxtDateFrom.Text = "";
                        this.TxtDateTo.Text = DateTime.Now.ToShortDateString().ToString();
                        this.TxtDateTo.Visible = false;
                        this.ddl_dataTrasm.SelectedIndex = 0;
                        this.TxtDateFrom.Visible = true;
                        this.toglieVociTuttiRegistri();
                        this.CaricaRagioni();
                        break;

                    case "DR": // Documenti Registro
                        this.pnlInput.Visible = false;
                        this.pnl_trasmUO.Visible = false;
                        this.pnl_StampaBuste.Visible = false;
                        this.pnl_DocumentiRegistro.Visible = true;
                        this.pnl_reportAvanzati.Visible = false;
                        this.pnl_DocumentiGrigi.Visible = false;
                        this.txt_initDataProt_E.Text = DateTime.Now.ToShortDateString().ToString();
                        CaricaComboTipologiaAtto(ddl_tipoAttoDR);
                        // Aggiornamento visibilità campi filtro
                        this.RefreshFiltersCtlsNumProtocollo();
                        this.RefreshFiltersCtlsDataProtocollo();
                        break;
                    case "B": // Stampa Buste
                        this.pnlInput.Visible = false;
                        this.pnl_trasmUO.Visible = false;
                        this.pnl_DocumentiRegistro.Visible = false;
                        this.pnl_StampaBuste.Visible = true;
                        this.pnl_reportAvanzati.Visible = false;
                        this.pnl_DocumentiGrigi.Visible = false;
                        this.txt_initDataProt_B.Text = DateTime.Now.ToShortDateString().ToString();
                        // Aggiornamento visibilità campi filtro
                        this.RefreshFiltersCtlsNumProtocollo_B();
                        this.RefreshFiltersCtlsDataProtocollo_B();

                        break;
                    case "TX_R":
                    case "TX_P":
                    case "PR_REG":
                    case "PR_REG_R":

                        this.impostaHtmlVisibile(tipoReport);
                        this.CaricaRagioniPerRicerca();
                        this.CaricaListaRuoliRepAv();
                        break;
                    case "DG": // Documenti Registro
                        this.pnlInput.Visible = false;
                        this.pnl_trasmUO.Visible = false;
                        this.pnl_DocumentiRegistro.Visible = false;
                        this.pnl_StampaBuste.Visible = false;
                        this.pnl_reportAvanzati.Visible = false;
                        this.pnl_DocumentiGrigi.Visible = true;
                        this.txt_initDataCreazioneG_E.Text = DateTime.Now.ToShortDateString().ToString();
                        CaricaComboTipologiaAtto(ddl_tipoAttoDG);
                        // Aggiornamento visibilità campi filtro
                        this.RefreshFiltersCtlsIdDoc();
                        this.RefreshFiltersCtlsDataCreazioneG();

                        break;
                    case "E": //Corrispondenti esterni

                        this.pnlInput.Visible = false;
                        this.pnl_trasmUO.Visible = false;
                        this.pnl_DocumentiRegistro.Visible = false;
                        this.pnl_StampaBuste.Visible = false;
                        this.pnl_reportAvanzati.Visible = false;
                        this.pnl_DocumentiGrigi.Visible = false;
                        break;
                    default:
                        this.pnlInput.Visible = false;
                        this.pnl_trasmUO.Visible = false;
                        this.pnl_DocumentiRegistro.Visible = false;
                        this.pnl_StampaBuste.Visible = false;
                        this.pnl_reportAvanzati.Visible = false;
                        this.pnl_DocumentiGrigi.Visible = false;

                        break;
                }
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void RefreshFiltersCtlsDataCreazioneG()
        {
            // Aggiornamento visibilità controlli filtro per data creazione grigio           
            switch (this.ddl_dataCreazioneG_E.SelectedIndex)
            {
                case 0:
                    this.txt_initDataCreazioneG_E.Visible = true;
                    this.txt_initDataCreazioneG_E.Enabled = true;
                    this.txt_fineDataCreazioneG_E.Visible = false;
                    this.lit_fineDataCreazioneG_E.Visible = false;
                    this.lit_initDataCreazioneG_E.Visible = false;
                    break;

                case 1:
                    this.txt_initDataCreazioneG_E.Visible = true;
                    this.txt_initDataCreazioneG_E.Enabled = true;
                    this.txt_fineDataCreazioneG_E.Enabled = true;
                    this.txt_fineDataCreazioneG_E.Visible = true;
                    this.lit_fineDataCreazioneG_E.Visible = true;
                    this.lit_initDataCreazioneG_E.Visible = true;
                    break;

                case 2:
                    this.txt_initDataCreazioneG_E.Visible = true;
                    this.txt_initDataCreazioneG_E.Text = DocumentManager.toDay();
                    this.txt_initDataCreazioneG_E.Enabled = false;
                    this.txt_fineDataCreazioneG_E.Visible = false;
                    this.lit_fineDataCreazioneG_E.Visible = false;
                    this.lit_initDataCreazioneG_E.Visible = false;
                    break;

                case 3:
                    txt_initDataCreazioneG_E.Visible = true;
                    this.txt_initDataCreazioneG_E.Text = DocumentManager.getFirstDayOfWeek();
                    this.txt_initDataCreazioneG_E.Enabled = false;
                    this.txt_fineDataCreazioneG_E.Visible = true;
                    this.txt_fineDataCreazioneG_E.Text = DocumentManager.getLastDayOfWeek();
                    this.txt_fineDataCreazioneG_E.Visible = true;
                    txt_fineDataCreazioneG_E.Enabled = false;
                    this.lit_fineDataCreazioneG_E.Visible = false;
                    this.lit_initDataCreazioneG_E.Visible = false;
                    break;

                case 4:
                    this.txt_initDataCreazioneG_E.Visible = true;
                    this.txt_initDataCreazioneG_E.Text = DocumentManager.getFirstDayOfMonth();
                    this.txt_initDataCreazioneG_E.Enabled = false;
                    this.txt_fineDataCreazioneG_E.Visible = true;
                    this.txt_fineDataCreazioneG_E.Text = DocumentManager.getLastDayOfMonth();
                    this.txt_fineDataCreazioneG_E.Visible = true;
                    this.txt_fineDataCreazioneG_E.Enabled = false;
                    this.lit_fineDataCreazioneG_E.Visible = true;
                    this.lit_initDataCreazioneG_E.Visible = true;
                    break;
            }

        }

        private void RefreshFiltersCtlsIdDoc()
        {
            // Aggiornamento visibilità controlli filtro per id documento
            bool rangeFilter = (this.ddl_idDoc_E.SelectedIndex != 0);

            this.lit_initIdDoc_E.Visible = rangeFilter;
            this.lit_fineIdDoc_E.Visible = rangeFilter;
            this.txt_fineIdDoc_E.Visible = rangeFilter;

            this.txt_fineIdDoc_E.Text = "";
        }
        private DocsPaWR.OrgTipoRuolo[] getTipiRuolo()
        {
            //userRegistri = UserManager.getListaRegistri(this);
            userRegistri = RoleManager.GetRoleInSession().registri;

            string codiceAmministrazione = userRegistri[0].codAmministrazione;

            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            return ws.AmmGetTipiRuolo(codiceAmministrazione);
        }
        private void CaricaListaRuoliRepAv()
        {
            ListItem newItem;

            switch (this.ddl_report.SelectedValue)
            {
                case "TX_R":
                case "TX_P":
                case "PR_REG_R":
                    this.ddl_rep_av_ruolo.Items.Clear();

                    newItem = new ListItem("", "0");
                    this.ddl_rep_av_ruolo.Items.Add(newItem);

                    DocsPaWR.OrgTipoRuolo[] ruoli = this.getTipiRuolo();
                    foreach (DocsPaWR.OrgTipoRuolo ruolo in ruoli)
                    {
                        newItem = new ListItem(ruolo.Descrizione.ToUpper(), ruolo.IDTipoRuolo);
                        this.ddl_rep_av_ruolo.Items.Add(newItem);
                    }

                    this.ddl_rep_av_ruolo.SelectedIndex = 0;
                    break;
            }
        }
        private void CaricaRagioniPerRicerca()
        {
            ListItem newItem;
            switch (this.ddl_report.SelectedValue)
            {
                case "TX_R":
                case "TX_P":
                    this.ddl_rep_av_rag_trasm.Items.Clear();
                    newItem = new ListItem("", "0");
                    this.ddl_rep_av_rag_trasm.Items.Add(newItem);

                    listaRagioni = TrasmManager.getListaRagioni(this, String.Empty, true);
                    if (listaRagioni != null && listaRagioni.Length > 0)
                    {
                        for (int i = 0; i < listaRagioni.Length; i++)
                        {
                            newItem = new ListItem(listaRagioni[i].descrizione, listaRagioni[i].systemId);
                            this.ddl_rep_av_rag_trasm.Items.Add(newItem);
                        }
                        this.ddl_rep_av_rag_trasm.SelectedIndex = 0;
                    }
                    break;
            }
        }
        private void impostaHtmlVisibile(string tipo)
        {
            this.pnlInput.Visible = false;
            this.pnl_trasmUO.Visible = false;
            this.pnl_DocumentiRegistro.Visible = false;
            this.pnl_StampaBuste.Visible = false;
            this.pnl_DocumentiGrigi.Visible = false;
            this.pnl_reportAvanzati.Visible = true;

            this.td_rep_av_ruolo.Visible = true;
            this.td_rep_av_rag_trasm.Visible = true;
            this.td_rep_av_data.Visible = true;

            this.lit_rep_av_da.Visible = true;
            txt_rep_av_initData.Visible = true;
            txt_rep_av_initData.Text = "01/01/" + DateTime.Now.Year.ToString();

            this.lit_rep_av_a.Visible = true;
            this.txt_rep_av_fineData.Visible = true;
            this.txt_rep_av_fineData.Text = DateTime.Now.ToShortDateString().ToString(); ;

            this.ddl_rep_av_ruolo.Visible = true;
            this.ddl_rep_av_rag_trasm.Visible = true;

            switch (tipo)
            {
                case "PR_REG":
                    this.td_rep_av_rag_trasm.Visible = false;
                    this.td_rep_av_ruolo.Visible = false;
                    break;
                case "PR_REG_R":
                    this.td_rep_av_rag_trasm.Visible = false;
                    break;
            }
        }
        private void RefreshFiltersCtlsNumProtocollo()
        {
            // Aggiornamento visibilità controlli filtro per numero protocollo
            bool rangeFilter = (this.ddl_numProt_E.SelectedIndex != 0);

            this.lblDAnumprot_E.Visible = rangeFilter;
            this.lblAnumprot_E.Visible = rangeFilter;
            this.txt_fineNumProt_E.Visible = rangeFilter;
            this.txt_fineNumProt_E.Text = "";
        }
        private void RefreshFiltersCtlsDataProtocollo()
        {
            //this.txt_initDataProt_E.Text = string.Empty;
            //this.txt_fineDataProt_E.Text = string.Empty;

            // Aggiornamento visibilità controlli filtro per data protocollo          
            switch (this.ddl_dataProt_E.SelectedIndex)
            {
                case 0:
                    this.txt_initDataProt_E.Visible = true;
                    this.txt_fineDataProt_E.Visible = false;
                    this.lit_initdataProt_E.Visible = false;
                    this.lit_finedataProt_E.Visible = false;
                    break;

                case 1:
                    this.txt_initDataProt_E.Visible = true;
                    this.txt_fineDataProt_E.Visible = true;
                    this.lit_initdataProt_E.Visible = true;
                    this.lit_finedataProt_E.Visible = true;
                    this.txt_initDataProt_E.Enabled = true;
                    this.txt_fineDataProt_E.Enabled = true;
                    break;

                case 2:
                    this.txt_initDataProt_E.Visible = true;
                    this.txt_initDataProt_E.Text = DocumentManager.toDay();
                    this.txt_initDataProt_E.Enabled = false;
                    this.txt_fineDataProt_E.Visible = false;
                    this.lit_initdataProt_E.Visible = false;
                    this.lit_finedataProt_E.Visible = false;
                    break;

                case 3:
                    this.txt_initDataProt_E.Visible = true;
                    this.txt_initDataProt_E.Enabled = false;
                    txt_initDataProt_E.Text = DocumentManager.getFirstDayOfWeek();
                    txt_fineDataProt_E.Text = DocumentManager.getLastDayOfWeek();
                    this.txt_fineDataProt_E.Visible = true;
                    this.lit_initdataProt_E.Visible = true;
                    this.lit_finedataProt_E.Visible = true;
                    this.txt_fineDataProt_E.Enabled = false;
                    break;

                case 4:
                    txt_initDataProt_E.Visible = true;
                    txt_initDataProt_E.Text = DocumentManager.getFirstDayOfMonth();
                    txt_initDataProt_E.Enabled = false;
                    txt_fineDataProt_E.Visible = true;
                    txt_fineDataProt_E.Text = DocumentManager.getLastDayOfMonth();
                    txt_fineDataProt_E.Enabled = false;
                    this.lit_initdataProt_E.Visible = true;
                    this.lit_finedataProt_E.Visible = true;
                    break;
            }
        }


        private void RefreshFiltersCtlsNumProtocollo_B()
        {
            // Aggiornamento visibilità controlli filtro per numero protocollo
            bool rangeFilter = (this.ddl_numProt_B.SelectedIndex != 0);

            this.lblDAnumprot_B.Visible = rangeFilter;
            this.lblAnumprot_B.Visible = rangeFilter;
            this.txt_fineNumProt_B.Visible = rangeFilter;

            this.txt_fineNumProt_B.Text = "";
        }

        private void RefreshFiltersCtlsDataProtocollo_B()
        {
            // Aggiornamento visibilità controlli filtro per data protocollo
            bool rangeFilter = (this.ddl_dataProt_B.SelectedIndex != 0);

            this.lit_initdataProt_B.Visible = rangeFilter;
            this.lit_finedataProt_B.Visible = rangeFilter;
            this.txt_fineDataProt_B.Visible = rangeFilter;
            this.txt_fineDataProt_B.Visible = rangeFilter;
            if (rangeFilter)
            {
                this.txt_initDataProt_B.Text = "";
                this.txt_fineDataProt_B.Text = System.DateTime.Now.ToShortDateString();
            }
        }

        /// <summary>
        /// Verifica della presenza di almeno un dato
        /// immesso nei campi di filtro della stampa 
        /// dei registri
        /// </summary>
        /// <returns></returns>
        private bool CheckRequiredFieldsStampaRegistro()
        {
            return (this.txt_initNumProt_E.Text.Length > 0 ||
                    this.txt_fineNumProt_E.Text.Length > 0 ||
                     this.txt_initDataProt_E.Text.Length > 0 ||
                    this.txt_fineDataProt_E.Text.Length > 0 ||
                    this.ddl_tipoAttoDR.SelectedItem.Text.Length > 0);
        }

        /// <summary>
        /// Verifica della presenza di almeno un dato
        /// immesso nei campi di filtro dei
        /// documenti grigi
        /// </summary>
        /// <returns></returns>
        private bool CheckRequiredFieldsDocumentiGrigi()
        {
            return (this.txt_initIdDoc_E.Text.Length > 0 ||
                    this.txt_fineIdDoc_E.Text.Length > 0 ||
                this.txt_initDataCreazioneG_E.Text.Length > 0 ||
                this.txt_fineDataCreazioneG_E.Text.Length > 0 ||
                    this.ddl_tipoAttoDG.SelectedItem.Text.Length > 0);
        }

        /// <summary>
        /// Verifica della presenza di almeno un dato
        /// immesso nei campi di filtro della stampa 
        /// buste destinatari
        /// </summary>
        /// <returns></returns>
        private bool CheckRequiredFieldsStampaBuste()
        {
            return (this.txt_initNumProt_B.Text.Length > 0 ||
                this.txt_fineNumProt_B.Text.Length > 0 ||
                this.txt_initDataProt_B.Text.Length > 0 ||
                this.txt_fineDataProt_B.Text.Length > 0 ||
                this.txt_Anno_B.Text.Length > 0);

        }
        private void setTuttiRegistri()
        {
            string[] listaIDRegistri;
            string listaIDRegCombo = string.Empty;

            try
            {
                listaIDRegCombo = this.ddl_registri.SelectedValue;

                listaIDRegistri = listaIDRegCombo.Split('_');

                UserManager.setListaIdRegistri(this, listaIDRegistri);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
        private void setStatoReg(DocsPaWR.Registro reg)
        {
            // inserisco il registro selezionato in sessione			
            UserManager.setRegistroSelezionato(this, reg);
            RegistryManager.SetRegistryInSession(reg);
        }
        protected void ddl_registri_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //mette in sessione il registro selezionato
            if (ddl_registri.SelectedIndex != -1)
            {
                if (userRegistri == null)
                {
                    //userRegistri = UserManager.getListaRegistri(this);
                    userRegistri = RoleManager.GetRoleInSession().registri;
                }

                if (this.ddl_registri.Items[this.ddl_registri.SelectedIndex].Text.Equals("Tutti"))
                {
                    this.setTuttiRegistri();
                }
                else
                {
                    this.setStatoReg(userRegistri[ddl_registri.SelectedIndex]);
                }
            }
        }

        protected void ddl_rep_av_data_SelectedIndexChanged(object sender, EventArgs e)
        {
            // valore singolo
            if (this.ddl_rep_av_data.SelectedIndex == 0)
            {
                this.lit_rep_av_da.Visible = true;
                this.lit_rep_av_a.Visible = false;
                this.txt_rep_av_initData.Text = System.DateTime.Now.ToShortDateString();
                this.txt_rep_av_fineData.Visible = false;
                this.txt_rep_av_fineData.Visible = false;
            }
            else // intervallo di tempo
            {
                this.lit_rep_av_da.Visible = true;
                this.lit_rep_av_a.Visible = true;
                this.txt_rep_av_initData.Text = "01/01/" + System.DateTime.Now.Year.ToString();
                this.txt_rep_av_fineData.Visible = true;
                this.txt_rep_av_fineData.Text = System.DateTime.Now.ToShortDateString().ToString();
            }
        }
        protected void ddl_dataTrasm_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataTrasm.SelectedIndex)
            {
                case 0://valore singolo
                    this.TxtDateFrom.Visible = true;
                    LitSearchDocumentFrom.Visible = false;
                    this.TxtDateTo.Visible = false;
                    LitSearchDocumentTo.Visible = false;
                    this.TxtDateFrom.Enabled = true;
                    break;

                case 1://intervallo
                    this.TxtDateFrom.Visible = true;
                    LitSearchDocumentFrom.Visible = true;
                    this.TxtDateTo.Visible = true;
                    LitSearchDocumentTo.Visible = true;
                    this.TxtDateFrom.Enabled = true;
                    this.TxtDateTo.Enabled = true;
                    break;

                case 2://oggi
                    this.TxtDateFrom.Visible = true;
                    this.TxtDateFrom.Enabled = false;
                    LitSearchDocumentFrom.Visible = false;
                    LitSearchDocumentTo.Visible = false;
                    this.TxtDateFrom.Text = DocumentManager.toDay();
                    this.TxtDateTo.Visible = false;
                    break;

                case 3://settimana corrente
                    this.TxtDateFrom.Visible = true;
                    this.TxtDateFrom.Enabled = false;
                    this.TxtDateFrom.Text = DocumentManager.getFirstDayOfWeek();
                    this.TxtDateTo.Visible = true;
                    this.TxtDateTo.Enabled = false;
                    this.TxtDateTo.Text = DocumentManager.getLastDayOfWeek();
                    LitSearchDocumentFrom.Visible = true;
                    LitSearchDocumentTo.Visible = true;
                    break;

                case 4://mese corrente
                    this.TxtDateFrom.Visible = true;
                    this.TxtDateFrom.Enabled = false;
                    this.TxtDateFrom.Text = DocumentManager.getFirstDayOfMonth();
                    this.TxtDateTo.Visible = true;
                    this.TxtDateTo.Enabled = false;
                    this.TxtDateTo.Text = DocumentManager.getLastDayOfMonth();
                    LitSearchDocumentFrom.Visible = true;
                    LitSearchDocumentTo.Visible = true;
                    break;
            }
        }
        protected void ImgProprietarioAddressBook_Click(object sender, EventArgs e)
        {
            this.CallType = RubricaCallType.CALLTYPE_STAMPA_REG_UO;
            HttpContext.Current.Session["AddressBook.from"] = "STAMPAREPORTUO";
            HttpContext.Current.Session["AddressBook.EnableOnly"] = "P";
            HttpContext.Current.Session["AddDocInProject"] = true;
            // AddressBookChkCommonAddressBook
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AddressBook", "ajaxModalPopupAddressBook();", true);
        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
            string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();

            if (atList != null && atList.Count > 0)
            {
                NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                Corrispondente tempCorrSingle;
                if (!corrInSess.isRubricaComune)
                    tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                else
                    tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                this.txt_codUO.Text = tempCorrSingle.codiceRubrica;
                this.txt_descUO.Text = tempCorrSingle.descrizione;
                this.upPnlUO.Update();
            }

            HttpContext.Current.Session["AddressBook.At"] = null;
            HttpContext.Current.Session["AddressBook.Cc"] = null;
        }

        #endregion
        private void BuildFiltroDocumentiRegistro()
        {
            //array contenitore degli array filtro di ricerca
            qV = new FiltroRicerca[1][];
            fVList = new DocsPaWR.FiltroRicerca[0];
            FiltroRicerca filterItem = null;

            #region Composizione filtri per numero protocollo
            if (this.ddl_numProt_E.SelectedIndex == 0)
            {//valore singolo carico NUM_PROTOCOLLO
                filterItem = new FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();
                filterItem.valore = this.txt_initNumProt_E.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);

            }
            else
            {
                //valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                if (!this.txt_initNumProt_E.Text.Equals(""))
                {
                    filterItem = new FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                    filterItem.valore = this.txt_initNumProt_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
                }
                if (!this.txt_fineNumProt_E.Text.Equals(""))
                {
                    filterItem = new FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
                    filterItem.valore = this.txt_fineNumProt_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);

                }
            }
            #endregion

            #region Composizione filtri per data protocollo


            if (this.ddl_dataProt_E.SelectedIndex == 2)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_TODAY.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProt_E.SelectedIndex == 3)
            {
                // siamo nel caso di Settimana corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProt_E.SelectedIndex == 4)
            {
                // siamo nel caso di Mese corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProt_E.SelectedIndex == 0)
            {//valore singolo carico DATA_PROTOCOLLO
                if (!this.txt_initDataProt_E.Text.Equals(""))
                {
                    if (!utils.isDate(this.txt_initDataProt_E.Text))
                    {
                        return;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString();
                    fV1.valore = this.txt_initDataProt_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            if (this.ddl_dataProt_E.SelectedIndex == 1)
            {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
                if (!this.txt_initDataProt_E.Text.Equals(""))
                {
                    if (!utils.isDate(this.txt_initDataProt_E.Text))
                    {
                        return;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.txt_initDataProt_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.txt_fineDataProt_E.Text.Equals(""))
                {
                    if (!utils.isDate(this.txt_fineDataProt_E.Text))
                    {
                        return;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
                    fV1.valore = this.txt_fineDataProt_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            #endregion

            #region Composizione filtri per Unità Organizzativa
            if (!chk_uo.Checked) //ricerca protocolli effettuati solo dalla uo selzionata
            {
                if (!this.txt_descUO.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();

                    if (this.hd_systemIdUo != null && !this.hd_systemIdUo.Value.Equals(""))
                    {
                        fV1.argomento = DocsPaWR.FiltriDocumento.ID_UO_PROT.ToString();
                        fV1.valore = this.hd_systemIdUo.Value;
                    }


                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            else //ricerca protocolli effettuati dalla uo selzionata e dalle sue figlie
            {
                if (!this.txt_descUO.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();

                    if (this.hd_systemIdUo != null && !this.hd_systemIdUo.Value.Equals(""))
                    {
                        fV1.argomento = DocsPaWR.FiltriDocumento.ID_UO_PROT_GERARCHIA.ToString();
                        fV1.valore = this.hd_systemIdUo.Value;
                    }


                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                else
                {

                }
            }

            #endregion

            #region Filtro Tipologia Documento
            if (this.ddl_tipoAttoDR.SelectedIndex > 0)
            {
                filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString();
                filterItem.valore = this.ddl_tipoAttoDR.SelectedItem.Value;
                fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
            }
            #endregion

            #region PROTOCOLLO_ARRIVO
            filterItem = new DocsPaWR.FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
            filterItem.valore = "true";
            fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
            #endregion

            #region PROTOCOLLO_PARTENZA
            filterItem = new DocsPaWR.FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
            filterItem.valore = "true";
            fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
            #endregion

            #region PROTOCOLLO_INTERNO
            filterItem = new DocsPaWR.FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
            filterItem.valore = "true";
            fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
            #endregion

            #region Filtro PROFILAZIONE_DINAMICA
            if (Session["templateRicerca"] != null)
            {
                filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriFascicolazione.PROFILAZIONE_DINAMICA.ToString();
                filterItem.valore = "ProfilazioneDinamica";
                filterItem.template = (DocsPaWR.Templates)Session["templateRicerca"];
                fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
            }
            #endregion filtro PROFILAZIONE_DINAMICA

            #region Filtro ORDER
            filterItem = new NttDataWA.DocsPaWR.FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriStampaRegistro.ORDER_FILTER.ToString();
            filterItem.valore = String.Empty;
            fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
            #endregion filtro ORDER

            #region Filtro CONSERVAZIONE
            filterItem = new NttDataWA.DocsPaWR.FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriDocumento.STATO_CONSERVAZIONE.ToString();
            filterItem.valore = "NVWCRETF";
            fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
            #endregion

            #region Filtro No Secutiry
            if (this.ddl_tipoAttoDR.SelectedIndex > 0 && UserManager.ruoloIsAutorized(this, "STAMPA_REG_NO_SEC"))
            {
                filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriFascicolazione.REG_NO_SECURITY.ToString();
                filterItem.valore = "true";
                fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
            }
            #endregion


            qV[0] = fVList;
            this.SearchFilters = qV;
        }
        private void verificaCampiPersonalizzati(DropDownList ddl)
        {
            DocsPaWR.Templates template = new DocsPaWR.Templates();
            if (!ddl.SelectedValue.Equals(""))
            {
                template = (DocsPaWR.Templates)Session["templateRicerca"];
                if (Session["templateRicerca"] == null)
                {
                    template = ProfilerDocManager.getTemplatePerRicerca((UserManager.GetInfoUser()).idAmministrazione, ddl.SelectedItem.Text);
                    Session.Add("templateRicerca", template);
                }
                if (template != null && !(ddl.SelectedItem.Text.ToUpper()).Equals(template.DESCRIZIONE.ToUpper()))
                {
                    template = ProfilerDocManager.getTemplatePerRicerca((UserManager.GetInfoUser()).idAmministrazione, ddl.SelectedItem.Text);
                    Session.Add("templateRicerca", template);
                }
            }
            if (template != null && template.SYSTEM_ID == 0)
            {
                //btn_CampiPersonalizzatiDR.Visible = false;
                //btn_CampiPersonalizzatiDG.Visible = false;
            }
            else
            {
                if (template != null && template.ELENCO_OGGETTI.Length != 0)
                {
                    //btn_CampiPersonalizzatiDR.Visible = true;
                    //btn_CampiPersonalizzatiDG.Visible = true;
                }
                else
                {
                    //btn_CampiPersonalizzatiDR.Visible = false;
                    //btn_CampiPersonalizzatiDG.Visible = false;
                }
            }
        }
        private void btn_CampiPersonalizzatiDR_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //verificaCampiPersonalizzati(ddl_tipoAttoDR);
            //RegisterStartupScript("Apri", "<script>apriPopupAnteprima();</script>");
        }

        private void BuildFiltroDocumentiGrigi()
        {
            //array contenitore degli array filtro di ricerca
            qV = new FiltroRicerca[1][];
            fVList = new DocsPaWR.FiltroRicerca[0];
            FiltroRicerca filterItem = null;

            #region Documento Grigio
            //Imposto sempre il TIPO in questo caso "G"
            filterItem = new FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
            filterItem.valore = "G";
            fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
            #endregion

            #region Id Documento Grigio
            if (this.ddl_idDoc_E.SelectedIndex == 0)
            {
                //valore singolo carico DOCNUMBER
                filterItem = new FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();
                filterItem.valore = this.txt_initIdDoc_E.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);

            }
            else
            {
                //valore singolo carico DOCNUMBER_DAL - DOCNUMBER_AL
                if (!this.txt_initIdDoc_E.Text.Equals(""))
                {
                    filterItem = new FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
                    filterItem.valore = this.txt_initIdDoc_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
                }
                if (!this.txt_fineIdDoc_E.Text.Equals(""))
                {
                    filterItem = new FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
                    filterItem.valore = this.txt_fineIdDoc_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);

                }
            }
            #endregion

            #region Data Creazione Grigio
            if (this.ddl_dataCreazioneG_E.SelectedIndex == 0)
            {
                //valore singolo  DATA_CREAZIONE_IL
                if (this.txt_initDataCreazioneG_E.Text != null && !this.txt_initDataCreazioneG_E.Text.Equals(""))
                {
                    filterItem = new FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
                    filterItem.valore = this.txt_initDataCreazioneG_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
                }
            }
            else
            {
                // intervallo DATA_CREAZIONE_SUCCESSIVA_AL - DATA_CREAZIONE_PRECEDENTE_IL
                if (this.txt_initDataCreazioneG_E.Text != null && !this.txt_initDataCreazioneG_E.Text.Equals(""))
                {
                    filterItem = new FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                    filterItem.valore = this.txt_initDataCreazioneG_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
                }
                if (!this.txt_fineDataCreazioneG_E.Text.Equals(""))
                {
                    filterItem = new FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                    filterItem.valore = this.txt_fineDataCreazioneG_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
                }
            }
            #endregion

            #region Filtro Tipologia Documento
            if (this.ddl_tipoAttoDG.SelectedIndex > 0)
            {
                filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString();
                filterItem.valore = this.ddl_tipoAttoDG.SelectedItem.Value;
                fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
            }
            #endregion

            #region GRIGI
            filterItem = new DocsPaWR.FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
            filterItem.valore = "true";
            fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
            #endregion

            #region Filtro PROFILAZIONE_DINAMICA
            if (Session["templateRicerca"] != null)
            {
                filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriFascicolazione.PROFILAZIONE_DINAMICA.ToString();
                filterItem.valore = "ProfilazioneDinamica";
                filterItem.template = (DocsPaWR.Templates)Session["templateRicerca"];
                fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
            }
            #endregion filtro PROFILAZIONE_DINAMICA

            #region Filtro No Security
            if (this.ddl_tipoAttoDG.SelectedIndex > 0 && UserManager.ruoloIsAutorized(this, "STAMPA_REG_NO_SEC"))
            {
                filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriFascicolazione.REG_NO_SECURITY.ToString();
                filterItem.valore = "true";
                fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
            }
            #endregion


            qV[0] = fVList;
            this.SearchFilters = qV;
        }

        private void BuildFiltroDocumentiStampaBuste()
        {
            //array contenitore degli array filtro di ricerca
            qV = new FiltroRicerca[1][];
            fVList = new NttDataWA.DocsPaWR.FiltroRicerca[0];
            FiltroRicerca filterItem = null;

            #region Composizione filtri per numero protocollo

            if (!this.txt_initNumProt_B.Text.Equals(""))
            {
                filterItem = new FiltroRicerca();

                if (this.ddl_numProt_B.SelectedValue == "1")
                    filterItem.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();

                filterItem.valore = this.txt_initNumProt_B.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
                filterItem = null;
            }

            if (!this.txt_fineNumProt_B.Text.Equals(""))
            {
                filterItem = new FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
                filterItem.valore = this.txt_fineNumProt_B.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
                filterItem = null;
            }

            #endregion

            #region Composizione filtri per data protocollo

            if (!this.txt_initDataProt_B.Text.Equals(""))
            {
                filterItem = new FiltroRicerca();

                if (this.ddl_dataProt_B.SelectedValue == "1")
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString();

                filterItem.valore = this.txt_initDataProt_B.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
                filterItem = null;
            }

            if (!this.txt_fineDataProt_B.Text.Equals(""))
            {
                filterItem = new FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
                filterItem.valore = this.txt_fineDataProt_B.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
                filterItem = null;
            }

            if (!this.txt_Anno_B.Text.Equals(""))
            {
                filterItem = new FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
                filterItem.valore = this.txt_Anno_B.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);
                filterItem = null;
            }

            #endregion


            qV[0] = fVList;
            this.SearchFilters = qV;
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
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
                    switch (caller.ID)
                    {
                        case "txt_codUO":
                            this.txt_codUO.Text = string.Empty;
                            this.txt_descUO.Text = string.Empty;
                            this.hd_systemIdUo.Value = string.Empty;
                            this.upPnlUO.Update();
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

        protected void SearchCorrespondent(string addressCode, string idControl)
        {
            RubricaCallType calltype = GetCallType(idControl);
            Corrispondente corr = UIManager.AddressBookManager.getCorrispondenteRubrica(addressCode, calltype);
            if (corr == null)
            {
                switch (idControl)
                {
                    case "txt_codUO":
                        this.txt_codUO.Text = string.Empty;
                        this.txt_descUO.Text = string.Empty;
                        this.hd_systemIdUo.Value = string.Empty;
                        this.upPnlUO.Update();
                        break;
                }

                string msg = "ErrorTransmissionCorrespondentNotFound";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
            }
            else
            {
                switch (idControl)
                {
                    case "txt_codUO":
                        if (corr.tipoCorrispondente.Equals("U"))
                        {
                            this.txt_codUO.Text = corr.codiceRubrica;
                            this.txt_descUO.Text = corr.descrizione;
                            this.hd_systemIdUo.Value = corr.systemId;
                           
                        }
                        else
                        {
                            this.txt_codUO.Text = string.Empty;
                            this.txt_descUO.Text = string.Empty;
                            this.hd_systemIdUo.Value = string.Empty;
                        }
                        this.upPnlUO.Update();
                        break;

                }
            }

            this.upPnlUO.Update();
        }

        protected RubricaCallType GetCallType(string idControl)
        {
            RubricaCallType calltype = RubricaCallType.CALLTYPE_STAMPA_REG_UO;
            return calltype;
        }

        /// <summary>
        /// Validazione dati immessi nei controlli di filtro per data e numero documento
        /// </summary>
        /// <returns></returns>
        private string IsValidControlsDocumentiGrigi()
        {
            string retValue = string.Empty;
            try
            {
                int intValue;

                // Validazione id documento
                if (this.txt_initIdDoc_E.Text != "")
                    intValue = Convert.ToInt32(this.txt_initIdDoc_E.Text);

                if (ddl_idDoc_E.SelectedIndex != 0 && this.txt_fineIdDoc_E.Text != "")
                    intValue = Convert.ToInt32(this.txt_fineIdDoc_E.Text);

                // Validazione data creazione
                if (this.txt_initDataCreazioneG_E.Text != "")
                    if (!utils.isDate(this.txt_initDataCreazioneG_E.Text))
                    {
                        retValue = "Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa";
                    }

                if (ddl_dataCreazioneG_E.SelectedIndex != 0 && this.txt_fineDataCreazioneG_E.Text != "")
                    if (!utils.isDate(this.txt_fineDataCreazioneG_E.Text))
                    {
                        retValue = "Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa";
                    }
            }
            catch (Exception)
            {
                retValue = "Si è verificato un errore nella scelta dei criteri di ricerca!";
            }

            return retValue;
        }

        private string IsValidControlsProtocollo()
        {
            string retValue = string.Empty;

            // Validazione anno protocollazione
            try
            {
                int intValue;


                if (this.txt_initNumProt_E.Text != "")
                    intValue = Convert.ToInt32(this.txt_initNumProt_E.Text);

                if (ddl_numProt_E.SelectedIndex != 0 && this.txt_fineNumProt_E.Text != "")
                    intValue = Convert.ToInt32(this.txt_fineNumProt_E.Text);

                if (this.txt_initDataProt_E.Text != "")
                    if (!utils.isDate(this.txt_initDataProt_E.Text))
                    {
                        retValue = "Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa";
                    }

                if (ddl_dataProt_E.SelectedIndex != 0 && this.txt_fineDataProt_E.Text != "")
                    if (!utils.isDate(this.txt_fineDataProt_E.Text))
                    {
                        retValue = "Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa";
                    }
            }
            catch (Exception)
            {
                retValue = "Si è verificato un errore nella scelta dei criteri di ricerca!";
            }

            return retValue;
        }


        private bool IsValidControlsStampaBuste()
        {
            bool retValue = true;

            try
            {
                int intValue;
                DateTime dateValue;

                if (this.txt_initNumProt_B.Text != "")
                    intValue = Convert.ToInt32(this.txt_initNumProt_B.Text);

                if (ddl_numProt_B.SelectedIndex != 0 && this.txt_fineNumProt_B.Text != "")
                    intValue = Convert.ToInt32(this.txt_fineNumProt_B.Text);

                if (this.txt_initDataProt_B.Text != "")
                    dateValue = Convert.ToDateTime(this.txt_initDataProt_B.Text);

                if (ddl_dataProt_B.SelectedIndex != 0 && this.txt_fineDataProt_B.Text != "")
                    dateValue = Convert.ToDateTime(this.txt_fineDataProt_B.Text);

                if (txt_Anno_B.Text != "")
                    intValue = Convert.ToInt32(this.txt_Anno_B.Text);
            }
            catch (Exception)
            {
                retValue = false;
            }

            return retValue;
        }

        protected void ddl_numProt_E_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.RefreshFiltersCtlsNumProtocollo();
        }

        protected void ddl_dataProt_E_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.RefreshFiltersCtlsDataProtocollo();
        }

        protected void ddl_idDoc_E_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.RefreshFiltersCtlsIdDoc();
        }

        protected void ddl_dataCreazioneG_E_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.RefreshFiltersCtlsDataCreazioneG();
        }

        protected void ddl_tipoAttoDG_SelectedIndexChanged(object sender, EventArgs e)
        {
            verificaCampiPersonalizzati(ddl_tipoAttoDG);
            Session.Remove("templateRicerca");
        }

        protected void ddl_numProt_B_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.RefreshFiltersCtlsNumProtocollo_B();
        }

        protected void ddl_dataProt_B_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.RefreshFiltersCtlsDataProtocollo_B();
        }

        private FiltroRicerca[][] SearchFilters
        {
            get
            {
                return (FiltroRicerca[][])HttpContext.Current.Session["filtroRicerca"];
            }
            set
            {
                HttpContext.Current.Session["filtroRicerca"] = value;
            }
        }
    }
}