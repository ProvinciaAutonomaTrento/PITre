using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDatalLibrary;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;
using log4net;

namespace NttDataWA.Project
{
    public partial class Transmissions : System.Web.UI.Page
    {
        #region fields
        private ArrayList listaExceptionTrasmissioni = new ArrayList();
        protected DocsPaWR.TrasmissioneSingola[] listTrasmSing;
        private ILog logger = LogManager.GetLogger(typeof(Transmissions));

        #endregion

        #region properties

        private int PageSize
        {
            get
            {
                int toReturn = 10;
                if (HttpContext.Current.Session["PageSize"] != null) Int32.TryParse(HttpContext.Current.Session["PageSize"].ToString(), out toReturn);
                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["PageSize"] = value;
            }
        }

        private int SelectedPage
        {
            get
            {
                int toReturn = 1;
                if (HttpContext.Current.Session["SelectedPage"] != null) Int32.TryParse(HttpContext.Current.Session["SelectedPage"].ToString(), out toReturn);
                if (toReturn < 1) toReturn = 1;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["SelectedPage"] = value;
            }

        }

        private int SelectedRowIndex
        {
            get
            {
                int toReturn = -1;
                if (HttpContext.Current.Session["SelectedRowIndex"] != null) Int32.TryParse(HttpContext.Current.Session["SelectedRowIndex"].ToString(), out toReturn);

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["SelectedRowIndex"] = value;
            }

        }

        private bool FilterReceived
        {
            get
            {
                bool toReturn = false;
                if (HttpContext.Current.Session["FilterReceived"] != null) toReturn = (bool)HttpContext.Current.Session["FilterReceived"];

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterReceived"] = value;
            }
        }

        private bool FilterTransmittedFromRole
        {
            get
            {
                bool toReturn = false;
                if (HttpContext.Current.Session["FilterTransmittedFromRole"] != null) toReturn = (bool)HttpContext.Current.Session["FilterTransmittedFromRole"];

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterTransmittedFromRole"] = value;
            }
        }

        private bool FilterActiveRF
        {
            get
            {
                bool toReturn = false;
                if (HttpContext.Current.Session["FilterActiveRF"] != null) toReturn = (bool)HttpContext.Current.Session["FilterActiveRF"];

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterActiveRF"] = value;
            }
        }

        private string FilterRF
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterRF"] != null) toReturn = HttpContext.Current.Session["FilterRF"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterRF"] = value;
            }
        }

        private string FilterRecipientType
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterRecipientType"] != null) toReturn = HttpContext.Current.Session["FilterRecipientType"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterRecipientType"] = value;
            }
        }

        private string FilterRecipientTypeOfCorrespondent
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterRecipientTypeOfCorrespondent"] != null) toReturn = HttpContext.Current.Session["FilterRecipientTypeOfCorrespondent"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterRecipientTypeOfCorrespondent"] = value;
            }
        }

        private string FilterRecipient
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterRecipient"] != null) toReturn = HttpContext.Current.Session["FilterRecipient"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterRecipient"] = value;
            }
        }

        private string FilterReason
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterReason"] != null) toReturn = HttpContext.Current.Session["FilterReason"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterReason"] = value;
            }
        }

        private string FilterDate
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterDate"] != null) toReturn = HttpContext.Current.Session["FilterDate"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterDate"] = value;
            }
        }

        private bool FilterAccepted
        {
            get
            {
                bool toReturn = false;
                if (HttpContext.Current.Session["FilterAccepted"] != null) toReturn = (bool)HttpContext.Current.Session["FilterAccepted"];

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterAccepted"] = value;
            }
        }

        private bool FilterViewed
        {
            get
            {
                bool toReturn = false;
                if (HttpContext.Current.Session["FilterViewed"] != null) toReturn = (bool)HttpContext.Current.Session["FilterViewed"];

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterViewed"] = value;
            }
        }

        private bool FilterPending
        {
            get
            {
                bool toReturn = false;
                if (HttpContext.Current.Session["FilterPending"] != null) toReturn = (bool)HttpContext.Current.Session["FilterPending"];

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterPending"] = value;
            }
        }

        private bool FilterRefused
        {
            get
            {
                bool toReturn = false;
                if (HttpContext.Current.Session["FilterRefused"] != null) toReturn = (bool)HttpContext.Current.Session["FilterRefused"];

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterRefused"] = value;
            }
        }

        private bool ViewCodeTransmissionModels
        {
            get
            {
                bool result = false;
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.VISUALIZZA_CODICE_MODELLI_TRASM.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.VISUALIZZA_CODICE_MODELLI_TRASM.ToString()].Equals("1")) result = false;
                return result;
            }
        }

        private DocsPaWR.InfoUtente InfoUser
        {
            get
            {
                DocsPaWR.InfoUtente result = null;
                if (HttpContext.Current.Session["infoUser"] != null)
                {
                    result = HttpContext.Current.Session["infoUser"] as DocsPaWR.InfoUtente;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["infoUser"] = value;
            }
        }

        private DocsPaWR.Utente UserLog
        {
            get
            {
                DocsPaWR.Utente result = null;
                if (HttpContext.Current.Session["user"] != null)
                {
                    result = HttpContext.Current.Session["user"] as DocsPaWR.Utente;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["user"] = value;
            }
        }

        private DocsPaWR.Ruolo Role
        {
            get
            {
                DocsPaWR.Ruolo result = null;
                if (HttpContext.Current.Session["role"] != null)
                {
                    result = HttpContext.Current.Session["role"] as DocsPaWR.Ruolo;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["role"] = value;
            }
        }

        private DocsPaWR.Registro Registry
        {
            get
            {
                DocsPaWR.Registro result = null;
                if (HttpContext.Current.Session["registry"] != null)
                {
                    result = HttpContext.Current.Session["registry"] as DocsPaWR.Registro;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["registry"] = value;
            }
        }

        private DocsPaWR.ModelloTrasmissione Template
        {
            get
            {
                DocsPaWR.ModelloTrasmissione result = null;
                if (HttpContext.Current.Session["Transmission_template"] != null)
                {
                    result = HttpContext.Current.Session["Transmission_template"] as DocsPaWR.ModelloTrasmissione;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["Transmission_template"] = value;
            }
        }

        private int AjaxAddressBookMinPrefixLenght
        {
            get
            {
                int result = 3;
                if (HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"] = value;
            }
        }

        private bool EnableAjaxAddressBook
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableAjaxAddressBook"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableAjaxAddressBook"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableAjaxAddressBook"] = value;
            }
        }

        private bool ShowsUserLocation
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["ShowsUserLocation"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["ShowsUserLocation"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ShowsUserLocation"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Response.Buffer = true;
                Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
                Response.Expires = -1;
                Response.CacheControl = "no-cache";

                //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
                Fascicolo Prj = UIManager.ProjectManager.getProjectInSession();

                if ((Prj.systemID != null && !string.IsNullOrEmpty(Prj.systemID)) && ProjectManager.CheckRevocationAcl())
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()');}", true);
                    return;
                }

                if (!IsPostBack)
                {
                    this.InfoUser = UIManager.UserManager.GetInfoUser();
                    this.UserLog = UIManager.UserManager.GetUserInSession();
                    this.Role = UIManager.RoleManager.GetRoleInSession();
                    this.Registry = null;
                    this.InitializePage();
                    this.VisibiltyRoleFunctions();

                    //Modifica per bug: creando un nuovo fascicolo da ricerca fascicoli si ha accessrights a null
                    if (Prj != null && string.IsNullOrEmpty(Prj.accessRights))
                    {
                        Prj.accessRights = ProjectManager.GetAccessRightFascBySystemID(Prj.systemID);
                        ProjectManager.setProjectInSession(Prj);
                    }
                    //if (Prj == null || string.IsNullOrEmpty(Prj.systemID))
                    //    DisableAllButtons();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshExpand", "UpdateExpand();", true);
                    if (((ScriptManager)Master.FindControl("ScriptManager1")).IsInAsyncPostBack)
                    {
                        // detect action from async postback
                        switch (((ScriptManager)Master.FindControl("ScriptManager1")).AsyncPostBackSourceElementID)
                        {
                            case "upPnlGridList":
                                this.TransmissionsBtnSearch_Click(null, null);
                                return;
                        }

                        // detect action from popup confirm
                        if (this.proceed_personal.Value == "true") { this.BeginTransmissionIfAllowed(); return; }
                        if (this.proceed_private.Value == "true")
                        {
                            this.BeginTransmissionIfAllowed(); this.EnableButtons();
                            this.HeaderProject.RefreshHeader();
                            this.ProjectTabs.RefreshProjectabs();
                            this.UpContainerProjectTab.Update();
                            this.ReApplyScripts();
                            return;
                        }
                        if (this.proceed_ownership.Value == "true") { this.PerformActionTransmit(); return; }
                        if (!string.IsNullOrEmpty(this.final_state.Value))
                        {
                            this.ChangeState();
                            this.final_state.Value = string.Empty;
                        }
                        // reset hidden forms
                        this.proceed_personal.Value = "";
                        this.proceed_private.Value = "";
                    }

                    this.ReadRetValueFromPopup();
                }

                this.ReApplyScripts();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
            if (IsPostBack)
            {
                if (!string.IsNullOrEmpty(this.idContributo.Value))
                {
                    //Visualizzo il dettaglio del contributo
                    SchedaDocumento schedaDoc = UIManager.DocumentManager.getDocumentDetails(this.Page, this.idContributo.Value, this.idContributo.Value);
                    if (schedaDoc != null)
                    {
                        #region navigation
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                        actualPage.IdObject = schedaDoc.docNumber;
                        actualPage.OriginalObjectId = schedaDoc.docNumber;

                        actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT_TRANSMISSIONS.ToString(), string.Empty);
                        actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT_TRANSMISSIONS.ToString(), true, this.Page);
                        actualPage.CodePage = Navigation.NavigationUtils.NamePage.PROJECT_TRANSMISSIONS.ToString();

                        actualPage.Page = "TRANSMISSIONSP.ASPX";
                        navigationList.Add(actualPage);
                        Navigation.NavigationUtils.SetNavigationList(navigationList);

                        #endregion
                        DocumentManager.setSelectedRecord(schedaDoc);
                        Response.Redirect(this.ResolveUrl("~/Document/Document.aspx"));
                    }
                    else
                    {
                        string error = "RevocationAclIndex";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('" + error.Replace("'", @"\'") + "', 'warning', '','',null,null,'')", true);
                    }
                    return;
                }
            }
        }

        private void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.CompleteTask.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('CompleteTask','');", true);
                Task task = TaskSelected;
                TaskSelected = null;
                string note = UIManager.TaskManager.TagIdContributo.LABEL_ATTIVITA_CONCLUSA + System.DateTime.Now.ToString() + UIManager.TaskManager.TagIdContributo.LABEL_ATTIVITA_CONCLUSA_C;
                if (!string.IsNullOrEmpty(task.ID_PROFILE_REVIEW))
                    note += TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO + task.ID_PROFILE_REVIEW + TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO_C;
                if (!string.IsNullOrEmpty(this.NoteCompleteTask))
                    note += Utils.Languages.GetLabelFromCode("TaskNoteTrasm", UIManager.UserManager.GetUserLanguage()) + " " + this.NoteCompleteTask;
                note += TaskManager.TagIdContributo.LABEL_TEXT_WRAP;
                task.STATO_TASK.NOTE_LAVORAZIONE = this.NoteCompleteTask;
                if (UIManager.TaskManager.ChiudiLavorazioneTask(task, note))
                {
                    DocsPaWR.Trasmissione trasmissione = TrasmManager.getSelectedTransmission();
                    DocsPaWR.TrasmissioneSingola trasmSing = null;
                    DocsPaWR.TrasmissioneUtente trasmUtente = null;
                    trasmSing = (from s in trasmissione.trasmissioniSingole where s.systemId.Equals(task.ID_TRASM_SINGOLA) select s).FirstOrDefault();
                    trasmUtente = (from u in trasmSing.trasmissioneUtente where u.utente.idPeople.Equals(task.UTENTE_DESTINATARIO.idPeople) select u).FirstOrDefault();
                    trasmUtente.noteAccettazione = note + trasmUtente.noteAccettazione;
                    this.showTransmission();
                }
                else
                {
                    string msg = "ErrorCloseTask";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                }
            }
            if (!string.IsNullOrEmpty(this.ReopenTask.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ReopenTask','');", true);
                if (TaskManager.RiapriLavorazione(this.TaskSelected))
                {
                    this.showTransmission();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorTaskRiaperturaLavorazione', 'error', '');} else {parent.ajaxDialogModal('ErrorTaskRiaperturaLavorazione', 'error', '');}", true);
                }
                this.TaskSelected = null;
            }
            if (!string.IsNullOrEmpty(this.HiddenCancelTask.Value))
            {
                this.HiddenCancelTask.Value = string.Empty;
                DocsPaWR.Task task = this.TaskSelected;
                this.TaskSelected = null;
                if (UIManager.TaskManager.AnnullaTask(task))
                {
                    this.showTransmission();
                }
                else
                {
                    string msg = "ErrorBlockTask";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                }
                return;
            }
            if (!string.IsNullOrEmpty(this.HiddenRemoveTask.Value))
            {
                this.HiddenRemoveTask.Value = string.Empty;
                DocsPaWR.Task task = this.TaskSelected;
                this.TaskSelected = null;
                if (UIManager.TaskManager.ChiudiTask(task))
                {
                    this.showTransmission();
                }
                else
                {
                    string msg = "ErrorRemoveTask";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                }
                return;
            }
        }

        protected void VisibiltyRoleFunctions()
        {
            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_TRA_NUOVA"))
            {
                this.TransmissionsBtnTransmit.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_TRA_MODIFICA"))
            {
                this.TransmissionsBtnModify.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_TRA_TRASMETTI"))
            {
                this.TransmissionsBtnTransmit.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_TRA_STAMPA"))
            {
                this.TransmissionsBtnPrint.Visible = false;
            }

            this.panelButtons.Update();
        }

        protected void InitializePage()
        {
            Session["PrintTransm"] = "F"; //Sessione stampa trasmissione fascicoli

            Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
            this.litDescription.Text = Server.HtmlEncode(fascicolo.descrizione);
            if (fascicolo != null)
            {
                Registro reg = UIManager.RegistryManager.getRegistroBySistemId(fascicolo.idRegistro);
                if (reg == null)
                {
                    reg = UIManager.RegistryManager.getRegistroBySistemId(fascicolo.idRegistroNodoTit);
                }

                this.Registry = reg;
            }

            this.LoadKeys();
            this.InitializesLabel();

            this.LoadTransmissionModels();
            this.LoadReasons();
            this.InitializeAddressBooks();

            this.LoadRF();

            this.ResetFilters();
            this.grdList_Bind();
            this.EnableButtons();
        }


        /// <summary>
        /// Initializes application labels
        /// </summary>
        private void InitializesLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.SignatureA4.Title = Utils.Languages.GetLabelFromCode("PopupSignatureA4", language);
            this.litDescriptionText.Text = Utils.Languages.GetLabelFromCode("TransmissionLitDescription", language);
            this.chkReceived.Text = Utils.Languages.GetLabelFromCode("TransmissionChkReceived", language);
            this.chkTransmittedFromRole.Text = Utils.Languages.GetLabelFromCode("TransmissionChkTransmittedFromRole", language);
            this.chkTransmittedFromRF.Text = Utils.Languages.GetLabelFromCode("TransmissionChkTransmittedFromRF", language);
            this.ddlRF.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("TransmissionDdlRF", language);
            this.TransmissionLitSenderRecipient.Text = Utils.Languages.GetLabelFromCode("TransmissionLitSenderRecipient", language);
            this.rblRecipientType.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("TransmissionRblRecipientTypeSender", language), "M"));
            this.rblRecipientType.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("TransmissionRblRecipientTypeRecipient", language), "D"));
            this.rblRecipientType.Items[0].Selected = true;
            this.TransmissionLitReason.Text = Utils.Languages.GetLabelFromCode("TransmissionLitReason", language);
            this.TransmissionLitDate.Text = Utils.Languages.GetLabelFromCode("TransmissionLitDate", language);
            this.chkAccepted.Text = Utils.Languages.GetLabelFromCode("TransmissionChkAccepted", language);
            this.chkViewed.Text = Utils.Languages.GetLabelFromCode("TransmissionChkViewed", language);
            this.chkPending.Text = Utils.Languages.GetLabelFromCode("TransmissionChkPending", language);
            this.chkRefused.Text = Utils.Languages.GetLabelFromCode("TransmissionChkRefused", language);
            this.TransmissionLitRapidTransmission.Text = Utils.Languages.GetLabelFromCode("TransmissionLitRapidTransmission", language);
            this.DdlTransmissionsModel.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("TransmissionDdlTransmissionsModel", language);
            this.TransmissionLitNote.Text = Utils.Languages.GetLabelFromCode("TransmissionLitNote", language);
            this.TransmissionLitDetailsRecipient.Text = Utils.Languages.GetLabelFromCode("TransmissionLitDetailsRecipient", language);
            this.TransmissionNoteAccRej.Text = Utils.Languages.GetLabelFromCode("TransmissionNoteAccRej", language);
            this.TransmissionsBtnAdd.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnAdd", language);
            this.TransmissionsBtnModify.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnModify", language);
            this.TransmissionsBtnTransmit.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnTransmit", language);
            this.TransmissionsBtnPrint.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnPrint", language);
            this.TransmissionsBtnAccept.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnAccept", language);
            this.TransmissionsBtnAcceptADL.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnAcceptADL", language);
            this.TransmissionsBtnAcceptADLR.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnAcceptADLR", language);
            this.TransmissionsBtnReject.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnReject", language);
            this.TransmissionsBtnView.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnView", language);
            this.TransmissionsBtnViewADL.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnViewADL", language);
            this.TransmissionsBtnViewADLR.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnViewADLR", language);
            this.TransmissionsBtnSearch.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnSearch", language);
            this.TransmissionsBtnClear.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnClear", language);
            this.TrasmissionsImgAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("TrasmissionsImgAddressBook", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
            this.CompleteTask.Title = Utils.Languages.GetLabelFromCode("CompleteTaskTitle", language);
            this.ReopenTask.Title = Utils.Languages.GetLabelFromCode("ReopenTaskTitle", language);
        }

        private void ResetFilters()
        {
            this.SelectedRowIndex = -1;
            this.SelectedPage = 1;
            this.FilterAccepted = false;
            this.FilterActiveRF = false;
            this.FilterDate = "";
            this.FilterPending = false;
            this.FilterReason = "";
            this.FilterReceived = false;
            this.FilterRecipient = "";
            this.FilterRecipientType = "";
            this.FilterRecipientTypeOfCorrespondent = "";
            this.FilterRefused = false;
            this.FilterRF = "";
            this.FilterTransmittedFromRole = false;
            this.FilterViewed = false;
        }

        protected void LoadKeys()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_NEW_RUBRICA_VELOCE.ToString())) && (Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_NEW_RUBRICA_VELOCE.ToString()).Equals("1")))
            {
                this.EnableAjaxAddressBook = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, DBKeys.FE_SEDE_TRASM.ToString())) && (Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, DBKeys.FE_SEDE_TRASM.ToString()).Equals("1")))
            {
                this.ShowsUserLocation = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]))
            {
                this.AjaxAddressBookMinPrefixLenght = int.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]);
            }

            if (!UIManager.AdministrationManager.IsEnableRF(this.InfoUser.idAmministrazione))
            {
                this.chkTransmittedFromRF.Visible = false;
                this.ddlRF.Visible = false;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamicaFasc.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamicaFasc.ToString()].Equals("1"))
            {
                this.CustomDocuments = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()].Equals("1"))
            {
                this.EnableStateDiagram = true;
            }
        }

        protected void InitializeAddressBooks()
        {
            this.SetAjaxAddressBook();
        }

        protected void SetAjaxAddressBook()
        {
            if (this.Registry == null)
            {
                this.Registry = this.Role.registri[0];
            }
          //  this.RapidRecipient.MinimumPrefixLength = this.AjaxAddressBookMinPrefixLenght;

            string dataUser = this.Role.systemId;

            dataUser = dataUser + "-" + this.Registry.systemId;

            string callType = "CALLTYPE_CORR_INT_NO_UO"; // Destinatario su protocollo interno
            this.RapidRecipient.ContextKey = dataUser + "-" + this.UserLog.idAmministrazione + "-" + callType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool checkIfNewSearch()
        {
            if (
                    (this.FilterReceived != this.chkReceived.Checked)
                    || (this.FilterTransmittedFromRole != this.chkTransmittedFromRole.Checked)
                    || (this.FilterActiveRF != this.chkTransmittedFromRF.Checked)
                    || (this.FilterRF != this.ddlRF.SelectedValue)
                    || (this.FilterRecipientType != this.rblRecipientType.SelectedValue)
                    || (this.FilterRecipient != this.IdRecipient.Value)
                    || (this.FilterReason != this.ddlReason.SelectedValue)
                    || (this.FilterDate != this.txtDate.Text)
                    || (this.FilterAccepted != this.chkAccepted.Checked)
                    || (this.FilterViewed != this.chkViewed.Checked)
                    || (this.FilterPending != this.chkPending.Checked)
                    || (this.FilterRefused != this.chkRefused.Checked)
               ) return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void grdList_Bind()
        {
            try
            {
                // reset paging
                bool isNewSearch = this.checkIfNewSearch();
                this.TransmissionsBtnTransmit.Enabled = false;
                if (isNewSearch)
                {
                    this.SelectedPage = 1;
                    this.SelectedRowIndex = -1;
                    this.FilterReceived = this.chkReceived.Checked;
                    this.FilterTransmittedFromRole = this.chkTransmittedFromRole.Checked;
                    this.FilterRF = this.ddlRF.SelectedValue;
                    this.FilterRecipientType = this.rblRecipientType.SelectedValue;
                    this.FilterRecipientTypeOfCorrespondent = this.RecipientTypeOfCorrespondent.Value;
                    this.FilterRecipient = this.IdRecipient.Value;
                    this.FilterReason = this.ddlReason.SelectedValue;
                    this.FilterDate = this.txtDate.Text;
                    this.FilterAccepted = this.chkAccepted.Checked;
                    this.FilterViewed = this.chkViewed.Checked;
                    this.FilterPending = this.chkPending.Checked;
                    this.FilterRefused = this.chkRefused.Checked;
                    this.grid_pageindex.Value = this.SelectedPage.ToString();
                    this.grid_rowindex.Value = this.SelectedRowIndex.ToString();
                    this.FilterActiveRF = this.chkTransmittedFromRF.Checked;
                }
                else if (this.SelectedPage.ToString() != this.grid_pageindex.Value)
                {
                    this.SelectedPage = 1;
                    if (!string.IsNullOrEmpty(this.grid_pageindex.Value)) this.SelectedPage = int.Parse(this.grid_pageindex.Value);
                    this.SelectedRowIndex = 0;
                    this.grid_rowindex.Value = this.SelectedRowIndex.ToString();
                }
                else if (this.SelectedRowIndex.ToString() != this.grid_rowindex.Value)
                {
                    this.SelectedRowIndex = 0;
                    if (!string.IsNullOrEmpty(this.grid_rowindex.Value)) this.SelectedRowIndex = int.Parse(this.grid_rowindex.Value);
                }


                // Creazione oggetto paginazione
                DocsPaWR.SearchPagingContext pagingContext = new DocsPaWR.SearchPagingContext();
                pagingContext.Page = this.SelectedPage;
                pagingContext.PageSize = this.grdList.PageSize;

                // Reperimento trasmissioni per il fascicolo corrente
                DocsPaWR.InfoTrasmissione[] infoTrasmList = this.GetTrasmissioniProject(ref pagingContext);

                // rebuild navigator
                this.buildGridNavigator(pagingContext);

                // binding
                this.grdList.DataSource = infoTrasmList;
                this.grdList.DataBind();
                if (this.grdList.Rows.Count > 0 && this.SelectedRowIndex < 0)
                {
                    this.SelectedRowIndex = 0;
                    this.grid_rowindex.Value = "0";
                }
                this.grdList.SelectedIndex = this.SelectedRowIndex;
                this.HighlightSelectedRow();
                this.upPnlGridList.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagingContext"></param>
        protected void buildGridNavigator(DocsPaWR.SearchPagingContext pagingContext)
        {
            this.plcNavigator.Controls.Clear();

            if (pagingContext.PageCount > 1)
            {
                Panel panel = new Panel();
                panel.CssClass = "recordNavigator2";

                for (int i = 1; i < pagingContext.PageCount + 1; i++)
                {
                    if (i == this.SelectedPage)
                    {
                        Literal lit = new Literal();
                        lit.Text = "<span class=\"linkPageHome\">" + i.ToString() + "</span>";
                        panel.Controls.Add(lit);
                    }
                    else
                    {
                        LinkButton btn = new LinkButton();
                        btn.Text = i.ToString();
                        btn.CssClass = "linkPageHome";
                        btn.Attributes["onclick"] = "$('#grid_pageindex').val($(this).text()); __doPostBack('upPnlGridList', ''); return false;";
                        panel.Controls.Add(btn);
                    }
                }

                this.plcNavigator.Controls.Add(panel);
            }

        }

        /// <summary>
        /// Reperimento trasmissioni per il fascicolo corrente
        /// </summary>
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        private DocsPaWR.InfoTrasmissione[] GetTrasmissioniProject(ref DocsPaWR.SearchPagingContext pagingContext)
        {
            try
            {
                // creazione filtri
                DocsPaWR.FiltroRicerca[] filters = new DocsPaWR.FiltroRicerca[0];

                if (this.FilterReceived)
                {
                    DocsPaWR.FiltroRicerca f = new DocsPaWR.FiltroRicerca();
                    f.argomento = "MY_RECEIVED_TRANSMISSIONS";
                    f.valore = this.Role.systemId + "|" + this.UserLog.idPeople + "|" + this.UserLog.systemId;
                    filters = utils.addToArrayFiltroRicerca(filters, f);
                }

                if (this.FilterTransmittedFromRole)
                {
                    DocsPaWR.FiltroRicerca f = new DocsPaWR.FiltroRicerca();
                    f.argomento = "MITTENTE_RUOLO";
                    f.valore = this.Role.systemId;
                    filters = utils.addToArrayFiltroRicerca(filters, f);
                }

                if (this.FilterActiveRF && !string.IsNullOrEmpty(this.FilterRF))
                {
                    DocsPaWR.FiltroRicerca f = new DocsPaWR.FiltroRicerca();
                    f.argomento = "EFFETTUATE_RUOLI_IN_RF";
                    f.valore = this.FilterRF;
                    filters = utils.addToArrayFiltroRicerca(filters, f);
                }

                if (!string.IsNullOrEmpty(this.FilterRecipient))
                {
                    DocsPaWR.FiltroRicerca f = new DocsPaWR.FiltroRicerca();

                    if (this.FilterRecipientType == "M")
                    {
                        if (this.FilterRecipientTypeOfCorrespondent == "R")
                        {
                            f.argomento = "MITTENTE_RUOLO";
                        }
                        else if (this.FilterRecipientTypeOfCorrespondent == "U")
                        {
                            f.argomento = "MITTENTE_UTENTE";
                        }
                    }
                    else if (this.FilterRecipientType == "D")
                    {
                        if (this.FilterRecipientTypeOfCorrespondent == "R")
                        {
                            f.argomento = "DESTINATARIO_RUOLO";
                        }
                        else if (this.FilterRecipientTypeOfCorrespondent == "U")
                        {
                            f.argomento = "DESTINATARIO_UTENTE";
                        }
                    }

                    f.valore = this.FilterRecipient;
                    filters = utils.addToArrayFiltroRicerca(filters, f);
                }

                if (!string.IsNullOrEmpty(this.FilterReason))
                {
                    DocsPaWR.FiltroRicerca f = new DocsPaWR.FiltroRicerca();
                    f.argomento = "RAGIONE";
                    f.valore = this.FilterReason;
                    filters = utils.addToArrayFiltroRicerca(filters, f);
                }

                if (!string.IsNullOrEmpty(this.FilterDate))
                {
                    DocsPaWR.FiltroRicerca f = new DocsPaWR.FiltroRicerca();
                    f.argomento = "DTA_INVIO";
                    f.valore = this.FilterDate;
                    filters = utils.addToArrayFiltroRicerca(filters, f);
                }

                if (this.FilterAccepted)
                {
                    DocsPaWR.FiltroRicerca f = new DocsPaWR.FiltroRicerca();
                    f.argomento = "DATA_ACCETTAZIONE_DA";
                    f.valore = this.FilterAccepted.ToString();
                    filters = utils.addToArrayFiltroRicerca(filters, f);
                }

                if (this.FilterViewed)
                {
                    DocsPaWR.FiltroRicerca f = new DocsPaWR.FiltroRicerca();
                    f.argomento = "VISTE";
                    f.valore = this.FilterViewed.ToString();
                    filters = utils.addToArrayFiltroRicerca(filters, f);
                }

                if (this.FilterPending)
                {
                    DocsPaWR.FiltroRicerca f = new DocsPaWR.FiltroRicerca();
                    f.argomento = "PENDENTI";
                    f.valore = this.FilterPending.ToString();
                    filters = utils.addToArrayFiltroRicerca(filters, f);
                }

                if (this.FilterRefused)
                {
                    DocsPaWR.FiltroRicerca f = new DocsPaWR.FiltroRicerca();
                    f.argomento = "DATA_RIFIUTO_DA";
                    f.valore = this.FilterRefused.ToString();
                    filters = utils.addToArrayFiltroRicerca(filters, f);
                }

                Fascicolo Prj = UIManager.ProjectManager.getProjectInSession();
                return TrasmManager.GetInfoTrasmissioniFiltered(Prj.systemID, "F", filters, ref pagingContext);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes["onclick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "'); __doPostBack('upPnlGridList', ''); return false;";
                }
                else if (e.Row.RowType == DataControlRowType.Header)
                {
                    e.Row.Cells[0].Text = Utils.Languages.GetLabelFromCode("TransmissionDateSend", string.Empty);
                    e.Row.Cells[1].Text = Utils.Languages.GetLabelFromCode("TransmissionUserRoleSender", string.Empty);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.SelectedRowIndex = this.grdList.SelectedIndex;
                this.HighlightSelectedRow();
                this.EnableButtons();
                this.panelButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void HighlightSelectedRow()
        {
            if (this.grdList.Rows.Count > 0 && this.grdList.SelectedRow != null)
            {
                GridViewRow gvRow = this.grdList.SelectedRow;
                foreach (GridViewRow GVR in this.grdList.Rows)
                {
                    if (GVR == gvRow)
                    {
                        GVR.CssClass += " selectedrow";
                        TrasmManager.setSelectedTransmissionId(((HiddenField)GVR.Cells[1].FindControl("trasmId")).Value);
                        TrasmManager.setSelectedTransmission(TrasmManager.GetTransmission(this, "F"));
                        this.showTransmission();
                    }
                    else
                    {
                        GVR.CssClass = GVR.CssClass.Replace(" selectedrow", "");
                    }
                }
            }

            // reset Dropdownlist Transmission Models 
            if (this.SelectedRowIndex >= 0)
            {
                this.DdlTransmissionsModel.SelectedIndex = -1;
                this.upPnlTransmissionsModel.Update();
            }
            this.ReApplyScripts();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void showTransmission()
        {
            try
            {
                Trasmissione trasm = TrasmManager.getSelectedTransmission();

                if (trasm != null)
                {
                    this.trasmNote.Text = trasm.noteGenerali;
                    if (string.IsNullOrEmpty(trasm.noteGenerali)) this.trasmNote.Text = "&nbsp;";

                    listTrasmSing = trasm.trasmissioniSingole;
                    if (listTrasmSing != null)
                    {
                        for (int i = 0; i < listTrasmSing.Length; i++)
                        {
                            DocsPaWR.TrasmissioneSingola trasmSing = (DocsPaWR.TrasmissioneSingola)listTrasmSing[i];
                            this.BuildSingleTransmissionsTables(trasm, trasmSing);
                        }
                    }
                }

                this.plcTransmission.Visible = true;
                this.UpPnlTransmission.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void hideTransmission()
        {
            this.plcTransmission.Visible = false;
            this.UpPnlTransmission.Update();
        }

        protected void BuildSingleTransmissionsTables(Trasmissione trasm, TrasmissioneSingola trasmSing)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();

                Table table = this.GetTransmissionTable(trasmSing.systemId);

                // DRAW TABLE
                ((TableCell)GetControlById(table, "trasmDetailsRecipient" + trasmSing.systemId)).Text = "<strong>" + formatBlankValue(trasmSing.corrispondenteInterno.descrizione) + "</strong>";
                ((TableCell)GetControlById(table, "trasmDetailsReason" + trasmSing.systemId)).Text = formatBlankValue(trasmSing.ragione.descrizione);

                switch (trasmSing.tipoTrasm)
                {
                    case "T":
                        ((TableCell)GetControlById(table, "trasmDetailsType" + trasmSing.systemId)).Text = Utils.Languages.GetLabelFromCode("TransmissionDdlTypeMulti", language).ToUpper();
                        break;
                    case "S":
                        ((TableCell)GetControlById(table, "trasmDetailsType" + trasmSing.systemId)).Text = Utils.Languages.GetLabelFromCode("TransmissionDdlTypeSingle", language).ToUpper();
                        break;
                    default:
                        ((TableCell)GetControlById(table, "trasmDetailsType" + trasmSing.systemId)).Text = formatBlankValue(null);
                        break;
                }

                if (this.CheckTrasmEffettuataDaUtenteCorrente(trasm))
                {
                    ((TableCell)GetControlById(table, "trasmDetailsNote" + trasmSing.systemId)).Text = this.formatBlankValue(trasmSing.noteSingole);
                }
                else
                {
                    ((TableCell)GetControlById(table, "trasmDetailsNote" + trasmSing.systemId)).Text = new string('-', 15);
                }

                ((TableCell)GetControlById(table, "trasmDetailsExpire" + trasmSing.systemId)).Text = this.formatBlankValue(trasmSing.dataScadenza);


                // DRAW USERS
                if (trasmSing.trasmissioneUtente != null)
                {
                    TrasmissioneUtente[] tu = trasmSing.trasmissioneUtente.OrderBy(e => e.utente.descrizione).ToArray();

                    for (int i = 0; i < tu.Length; i++)
                    {
                        DocsPaWR.TrasmissioneUtente TrasmUt = (DocsPaWR.TrasmissioneUtente)tu[i];
                        TableRow row = this.GetDetailsRow(TrasmUt.systemId);

                        string userDetails = TrasmUt.utente.descrizione;
                        if (this.ShowsUserLocation && !string.IsNullOrEmpty(TrasmUt.utente.sede)) userDetails += " (" + TrasmUt.utente.sede + ")";
                        ((TableCell)GetControlById(row, "trasmDetailsUser" + TrasmUt.systemId)).Text = this.formatBlankValue(userDetails);

                        if (!string.IsNullOrEmpty(TrasmUt.dataVista) && (string.IsNullOrEmpty(TrasmUt.cha_vista_delegato) || TrasmUt.cha_vista_delegato.Equals("0")))
                        {
                            ((TableCell)GetControlById(row, "trasmDetailsViewed" + TrasmUt.systemId)).Text = this.formatBlankValue(TrasmUt.dataVista);
                        }
                        else
                        {
                            ((TableCell)GetControlById(row, "trasmDetailsViewed" + TrasmUt.systemId)).Text = this.formatBlankValue(null);
                        }

                        if (!string.IsNullOrEmpty(TrasmUt.dataAccettata) && (string.IsNullOrEmpty(TrasmUt.cha_accettata_delegato) || TrasmUt.cha_accettata_delegato.Equals("0")))
                        {
                            ((TableCell)GetControlById(row, "trasmDetailsAccepted" + TrasmUt.systemId)).Text = this.formatBlankValue(TrasmUt.dataAccettata);
                        }
                        else
                        {
                            ((TableCell)GetControlById(row, "trasmDetailsAccepted" + TrasmUt.systemId)).Text = this.formatBlankValue(null);
                        }

                        if (!string.IsNullOrEmpty(TrasmUt.dataRifiutata) && (string.IsNullOrEmpty(TrasmUt.cha_rifiutata_delegato) || TrasmUt.cha_rifiutata_delegato.Equals("0")))
                        {
                            ((TableCell)GetControlById(row, "trasmDetailsRif" + TrasmUt.systemId)).Text = this.formatBlankValue(TrasmUt.dataRifiutata);
                        }
                        else
                        {
                            ((TableCell)GetControlById(row, "trasmDetailsRif" + TrasmUt.systemId)).Text = this.formatBlankValue(null);
                        }

                        if (!string.IsNullOrEmpty(TrasmUt.dataRimossaTDL))
                        {
                            ((TableCell)GetControlById(row, "trasmDetailsRemoved" + TrasmUt.systemId)).Text = this.formatBlankValue(TrasmUt.dataRimossaTDL);
                        }
                        else
                        {
                            ((TableCell)GetControlById(row, "trasmDetailsRemoved" + TrasmUt.systemId)).Text = this.formatBlankValue(null);
                        }

                        // viene verificato se l'utente corrente
                        // può visualizzare i dettagli (note rifiuto e note accettazione)
                        // della trasmissione singola:
                        // - ha pieni diritti di visualizzazione
                        //   se è l'utente che ha creato la trasmissione;
                        // - altrimenti viene verificato se l'utente corrente è lo stesso
                        //   che ha ricevuto la trasmissione (e quindi l'ha accettata)
                        if (CheckTrasmEffettuataDaUtenteCorrente(trasm) || CheckTrasmUtenteCorrente(TrasmUt))
                        {
                            if (!string.IsNullOrEmpty(TrasmUt.dataAccettata) && (string.IsNullOrEmpty(TrasmUt.cha_accettata_delegato) || TrasmUt.cha_accettata_delegato.Equals("0")))
                            {
                                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.ENABLE_TASK.ToString()))
                                    && Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.ENABLE_TASK.ToString()).Equals("1")
                                    && TrasmUt.noteAccettazione.Contains(TaskManager.TagIdContributo.LABEL_ATTIVITA_CONCLUSA))
                                {
                                    ((TableCell)GetControlById(row, "trasmDetailsInfo" + TrasmUt.systemId)).Controls.Add(BuildNoteTask(TrasmUt.noteAccettazione, TrasmUt.systemId));
                                }
                                else
                                {
                                    ((TableCell)GetControlById(row, "trasmDetailsInfo" + TrasmUt.systemId)).Text = formatBlankValue(TrasmUt.noteAccettazione);
                                }
                            }
                            else
                                if (!string.IsNullOrEmpty(TrasmUt.dataRifiutata) && (string.IsNullOrEmpty(TrasmUt.cha_rifiutata_delegato) || TrasmUt.cha_rifiutata_delegato.Equals("0")))
                                {
                                    ((TableCell)GetControlById(row, "trasmDetailsInfo" + TrasmUt.systemId)).Text = formatBlankValue(TrasmUt.noteRifiuto);
                                }
                                else
                                {
                                    ((TableCell)GetControlById(row, "trasmDetailsInfo" + TrasmUt.systemId)).Text = formatBlankValue(null);
                                }
                        }
                        else
                        {
                            ((TableCell)GetControlById(row, "trasmDetailsInfo" + TrasmUt.systemId)).Text = new string('-', 15);
                        }

                        table.Controls.Add(row);


                        // RIGA IN CASO DI DELEGA
                        if (!string.IsNullOrEmpty(TrasmUt.idPeopleDelegato) && (TrasmUt.cha_accettata_delegato == "1" || TrasmUt.cha_rifiutata_delegato == "1" || TrasmUt.cha_vista_delegato == "1") && (!string.IsNullOrEmpty(TrasmUt.dataVista) || !string.IsNullOrEmpty(TrasmUt.dataAccettata) || !string.IsNullOrEmpty(TrasmUt.dataRifiutata)))
                        {
                            string idPeopleDelegatoTrasm = TrasmUt.systemId.ToString() + TrasmUt.idPeopleDelegato;
                            TableRow row2 = GetDetailsRow(idPeopleDelegatoTrasm);

                            string del = Utils.Languages.GetLabelFromCode("TransmissionDelegatedBy", language);

                            ((TableCell)GetControlById(row2, "trasmDetailsUser" + idPeopleDelegatoTrasm)).Text = formatBlankValue(TrasmUt.idPeopleDelegato + "<br>(" + del + " " + TrasmUt.utente.descrizione + ")");

                            if (!string.IsNullOrEmpty(TrasmUt.dataVista) && TrasmUt.cha_vista_delegato.Equals("1"))
                            {
                                ((TableCell)GetControlById(row2, "trasmDetailsViewed" + idPeopleDelegatoTrasm)).Text = formatBlankValue(TrasmUt.dataVista);
                            }
                            else
                            {
                                ((TableCell)GetControlById(row2, "trasmDetailsViewed" + idPeopleDelegatoTrasm)).Text = formatBlankValue(null);
                            }

                            if (TrasmUt.cha_accettata_delegato.Equals("1"))
                            {
                                ((TableCell)GetControlById(row2, "trasmDetailsAccepted" + idPeopleDelegatoTrasm)).Text = formatBlankValue(TrasmUt.dataAccettata);
                            }
                            else
                            {
                                ((TableCell)GetControlById(row2, "trasmDetailsAccepted" + idPeopleDelegatoTrasm)).Text = formatBlankValue(null);
                            }

                            if (TrasmUt.cha_rifiutata_delegato.Equals("1"))
                            {
                                ((TableCell)GetControlById(row2, "trasmDetailsRif" + idPeopleDelegatoTrasm)).Text = formatBlankValue(TrasmUt.dataRifiutata);
                            }
                            else
                            {
                                ((TableCell)GetControlById(row2, "trasmDetailsRif" + idPeopleDelegatoTrasm)).Text = formatBlankValue(null);
                            }
                            if (CheckTrasmEffettuataDaUtenteCorrente(trasm) || CheckTrasmUtenteCorrente(TrasmUt))
                            {
                                if (!string.IsNullOrEmpty(TrasmUt.dataAccettata) && TrasmUt.cha_accettata_delegato.Equals("1"))
                                {
                                    //Verifico se per questa trasmissione era stato avviato un task
                                    if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.ENABLE_TASK.ToString())) 
                                        && Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.ENABLE_TASK.ToString()).Equals("1")
                                        && TrasmUt.noteAccettazione.Contains(TaskManager.TagIdContributo.LABEL_ATTIVITA_CONCLUSA))
                                    {
                                        ((TableCell)GetControlById(row2, "trasmDetailsInfo" + idPeopleDelegatoTrasm)).Controls.Add(BuildNoteTask(TrasmUt.noteAccettazione, TrasmUt.systemId));
                                    }
                                    else
                                    {
                                        ((TableCell)GetControlById(row2, "trasmDetailsInfo" + idPeopleDelegatoTrasm)).Text = formatBlankValue(TrasmUt.noteAccettazione);
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(TrasmUt.dataRifiutata) && TrasmUt.cha_rifiutata_delegato.Equals("1"))
                                    {
                                        ((TableCell)GetControlById(row2, "trasmDetailsInfo" + idPeopleDelegatoTrasm)).Text = formatBlankValue(TrasmUt.noteRifiuto);
                                    }
                                    else
                                    {
                                        ((TableCell)GetControlById(row2, "trasmDetailsInfo" + idPeopleDelegatoTrasm)).Text = formatBlankValue(null);
                                    }
                                }
                            }
                            else
                            {
                                ((TableCell)GetControlById(row2, "trasmDetailsInfo" + idPeopleDelegatoTrasm)).Text = new string('-', 15);
                            }
                            table.Controls.Add(row2);
                        }
                    }
                }
                else
                {
                    table.Controls.Remove(((TableRow)GetControlById(table, "row_users" + trasmSing.systemId)));
                }
                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.ENABLE_TASK.ToString()))
                    && Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.ENABLE_TASK.ToString()).Equals("1")
                    && trasmSing.ragione.isTipoTask)
                {
                    Task task = UIManager.TaskManager.GetTaskByTrasmSingola(trasmSing.systemId);
                    if (task != null && !string.IsNullOrEmpty(task.ID_TASK))
                    {
                        //Verifico se il ruolo/utente è coinvolto nel task come destinatario o mittente
                        bool isCoinvolto = false;
                        if(task.UTENTE_DESTINATARIO.idPeople.Equals(UserManager.GetUserInSession().idPeople))
                        {
                            if (task.RUOLO_DESTINATARIO != null && !string.IsNullOrEmpty(task.RUOLO_DESTINATARIO.idGruppo))
                            {
                                if (task.RUOLO_DESTINATARIO.idGruppo.Equals(RoleManager.GetRoleInSession().idGruppo))
                                    isCoinvolto = true;
                            }
                            else
                                isCoinvolto = true;
                        }

                        if (task.UTENTE_MITTENTE.idPeople.Equals(UserManager.GetUserInSession().idPeople) && task.RUOLO_MITTENTE.idGruppo.Equals(RoleManager.GetRoleInSession().idGruppo))
                        {
                                isCoinvolto = true;
                        }

                        if (isCoinvolto)
                        {
                            //INSERISCO UNA RIGA PER I PULSANTI DI AZIONE DEL TASK
                            TableRow rowHeaderButtonTask = GetRowHeaderButtonTask(trasmSing.systemId);
                            table.Controls.Add(rowHeaderButtonTask);

                            TableRow row = GetRowButtonTask(trasmSing.systemId);
                            ((TableCell)GetControlById(row, "trasmButtonsTask" + trasmSing.systemId)).Controls.Add(EnableButtontTask(task));
                            table.Controls.Add(row);
                        }
                    }
                }

                // ADD TABLE
                this.plcTransmissions.Controls.Add(table);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        #region TASK

        private TableRow GetRowHeaderButtonTask(string id)
        {
            TableRow row = new TableRow();
            row.ID = "row_button_task" + id;
            row.CssClass = "header2";

            TableCell cell1 = new TableCell();
            cell1.ColumnSpan = 7;
            cell1.CssClass = "first";
            cell1.Text = Utils.Languages.GetLabelFromCode("TransmissionLitAzioniTask", UserManager.GetUserLanguage());
            row.Controls.Add(cell1);

            return row;
        }

        private TableRow GetRowButtonTask(string id)
        {
            TableRow row = new TableRow();
            row.CssClass = "users";

            TableCell cellButtonsTask = new TableCell();
            cellButtonsTask.ColumnSpan = 7;
            cellButtonsTask.ID = "trasmButtonsTask" + id;
            cellButtonsTask.CssClass = "first";
            row.Cells.Add(cellButtonsTask);

            return row;
        }

        private System.Web.UI.HtmlControls.HtmlGenericControl EnableButtontTask(Task task)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            System.Web.UI.HtmlControls.HtmlGenericControl divContent = new System.Web.UI.HtmlControls.HtmlGenericControl();
            divContent.Attributes.Add("class", "colMassiveOperationDx");

            bool isTaskReceived = false;
            bool isTaskSender = false;
            if (task.UTENTE_MITTENTE.idPeople.Equals(UserManager.GetUserInSession().idPeople) && task.RUOLO_MITTENTE.idGruppo.Equals(RoleManager.GetRoleInSession().idGruppo))
            {
                isTaskSender = true;
            }

            if (task.UTENTE_DESTINATARIO.idPeople.Equals(UserManager.GetUserInSession().idPeople))
            {
                if (task.RUOLO_DESTINATARIO != null && !string.IsNullOrEmpty(task.RUOLO_DESTINATARIO.idGruppo))
                {
                    if (task.RUOLO_DESTINATARIO.idGruppo.Equals(RoleManager.GetRoleInSession().idGruppo))
                        isTaskReceived = true;
                }
                else
                    isTaskReceived = true;
            }

            if (task.STATO_TASK.STATO == StatoAvanzamento.Aperto || task.STATO_TASK.STATO == StatoAvanzamento.Riaperto)
            {
                if (!string.IsNullOrEmpty(task.ID_PROFILE_REVIEW))
                {
                    CustomImageButton imgViewContributo = new CustomImageButton();
                    imgViewContributo.ID = "imgViewContributo_" + task.ID_TASK;
                    imgViewContributo.ImageUrl = "../Images/Icons/view_details_review.png";
                    imgViewContributo.OnMouseOverImage = "../Images/Icons/view_details_review_hover.png";
                    imgViewContributo.OnMouseOutImage = "../Images/Icons/view_details_review.png";
                    imgViewContributo.ImageUrlDisabled = "../Images/Icons/view_details_review_disabled.png";
                    imgViewContributo.ToolTip = Utils.Languages.GetLabelFromCode("TaskViewContributoTooltip", language);
                    imgViewContributo.CssClass = "clickableLeft";
                    //e.Row.Cells[4].Attributes["onclick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "'); $('#BtnSelect').click();";
                    imgViewContributo.Attributes["onClick"] = "$('#idTrasmSingola').val('" + task.ID_TRASM_SINGOLA + "');$('#btnImgViewContributo').click(); return false";
                    divContent.Controls.Add(imgViewContributo);
                }
                if (isTaskReceived)
                {
                    if (string.IsNullOrEmpty(task.ID_PROFILE_REVIEW))
                    {
                        CustomImageButton imgCreaContributo = new CustomImageButton();
                        imgCreaContributo.ID = "imgCreaContributo_" + task.ID_TASK;
                        imgCreaContributo.ImageUrl = "../Images/Icons/create_doc_in_project.png";
                        imgCreaContributo.OnMouseOverImage = "../Images/Icons/create_doc_in_project_hover.png";
                        imgCreaContributo.OnMouseOutImage = "../Images/Icons/create_doc_in_project.png";
                        imgCreaContributo.ImageUrlDisabled = "../Images/Icons/create_doc_in_project_disabled.png";
                        imgCreaContributo.ToolTip = Utils.Languages.GetLabelFromCode("TaskCreaContributoTooltip", language);
                        imgCreaContributo.CssClass = "clickableLeft";
                        imgCreaContributo.Attributes["onClick"] = "$('#idTrasmSingola').val('" + task.ID_TRASM_SINGOLA + "');$('#btnImgCreaContributo').click();return false;";
                        divContent.Controls.Add(imgCreaContributo);
                    }

                    CustomImageButton imgCloseTask = new CustomImageButton();
                    imgCloseTask.ID = "imgCloseTask_" + task.ID_TASK;
                    imgCloseTask.ImageUrl = "../Images/Icons/close_task.png";
                    imgCloseTask.OnMouseOverImage = "../Images/Icons/close_task_hover.png";
                    imgCloseTask.OnMouseOutImage = "../Images/Icons/close_task.png";
                    imgCloseTask.ImageUrlDisabled = "../Images/Icons/close_task_disabled.png";
                    imgCloseTask.ToolTip = Utils.Languages.GetLabelFromCode("TaskCloseTaskTooltip", language);
                    imgCloseTask.CssClass = "clickableLeft";
                    imgCloseTask.Attributes["onClick"] = "$('#idTrasmSingola').val('" + task.ID_TRASM_SINGOLA + "');$('#btnImgCloseTask').click(); return false";
                    divContent.Controls.Add(imgCloseTask);
                }
                if(isTaskSender)
                {
                    CustomImageButton imgBlockTask = new CustomImageButton();
                    imgBlockTask.ID = "imgBlockTask_" + task.ID_TASK;
                    imgBlockTask.ImageUrl = "../Images/Icons/block_task.png";
                    imgBlockTask.OnMouseOverImage = "../Images/Icons/block_task_hover.png";
                    imgBlockTask.OnMouseOutImage = "../Images/Icons/block_task.png";
                    imgBlockTask.ImageUrlDisabled = "../Images/Icons/block_task_disabled.png";
                    imgBlockTask.ToolTip = Utils.Languages.GetLabelFromCode("TaskCancelTaskTooltip", language);
                    imgBlockTask.CssClass = "clickableLeft";
                    imgBlockTask.Attributes["onClick"] = "$('#idTrasmSingola').val('" + task.ID_TRASM_SINGOLA + "');$('#btnImgBlockTask').click();return false;";
                    divContent.Controls.Add(imgBlockTask);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(task.ID_PROFILE_REVIEW))
                {
                    CustomImageButton imgViewContributo = new CustomImageButton();
                    imgViewContributo.ID = "imgViewContributo_" + task.ID_TASK;
                    imgViewContributo.ImageUrl = "../Images/Icons/view_details_review.png";
                    imgViewContributo.OnMouseOverImage = "../Images/Icons/view_details_review_hover.png";
                    imgViewContributo.OnMouseOutImage = "../Images/Icons/view_details_review.png";
                    imgViewContributo.ImageUrlDisabled = "../Images/Icons/view_details_review_disabled.png";
                    imgViewContributo.ToolTip = Utils.Languages.GetLabelFromCode("TaskViewContributoTooltip", language);
                    imgViewContributo.CssClass = "clickableLeft";
                    imgViewContributo.Attributes["onClick"] = "$('#idTrasmSingola').val('" + task.ID_TRASM_SINGOLA + "');$('#btnImgViewContributo').click(); return false";
                    divContent.Controls.Add(imgViewContributo);
                }
                if (isTaskSender)//se non sono il destinatario del task
                {
                    CustomImageButton imgRiapriLavorazione = new CustomImageButton();
                    imgRiapriLavorazione.ID = "imgRiapriLavorazione_" + task.ID_TASK;
                    imgRiapriLavorazione.ImageUrl = "../Images/Icons/Reopen_task.png";
                    imgRiapriLavorazione.OnMouseOverImage = "../Images/Icons/Reopen_task_hover.png";
                    imgRiapriLavorazione.OnMouseOutImage = "../Images/Icons/Reopen_task.png";
                    imgRiapriLavorazione.ImageUrlDisabled = "../Images/Icons/Reopen_task_disabled.png";
                    imgRiapriLavorazione.ToolTip = Utils.Languages.GetLabelFromCode("TaskRiapriLavorazioneTooltip", language);
                    imgRiapriLavorazione.CssClass = "clickableLeft";
                    imgRiapriLavorazione.Attributes["onClick"] = "$('#idTrasmSingola').val('" + task.ID_TRASM_SINGOLA + "');$('#btnImgRiapriLavorazione').click();return false;";
                    divContent.Controls.Add(imgRiapriLavorazione);

                    CustomImageButton imgRemoveTask = new CustomImageButton();
                    imgRemoveTask.ID = "imgRemoveTask_" + task.ID_TASK;
                    imgRemoveTask.ImageUrl = "../Images/Icons/chiudiTask.png";
                    imgRemoveTask.OnMouseOverImage = "../Images/Icons/chiudiTask_hover.png";
                    imgRemoveTask.OnMouseOutImage = "../Images/Icons/chiudiTask.png";
                    imgRemoveTask.ImageUrlDisabled = "../Images/Icons/chiudiTask_disabled.png";
                    imgRemoveTask.ToolTip = Utils.Languages.GetLabelFromCode("TaskRemoveTaskTooltip", language);
                    imgRemoveTask.CssClass = "clickableLeft";
                    imgRemoveTask.Attributes["onClick"] = "$('#idTrasmSingola').val('" + task.ID_TRASM_SINGOLA + "');$('#btnImgRemoveTask').click();return false;";
                    divContent.Controls.Add(imgRemoveTask);
                }
            }
            return divContent;
        }

        protected void btnImgViewContributo_Click(object sender, EventArgs e)
        {
            try
            {
                string idTrasmSingola = this.idTrasmSingola.Value;
                this.idTrasmSingola.Value = string.Empty;
                Task task = UIManager.TaskManager.GetTaskByTrasmSingola(idTrasmSingola);
                SchedaDocumento schedaDocumento = UIManager.DocumentManager.getDocumentDetails(this.Page, task.ID_PROFILE_REVIEW, task.ID_PROFILE_REVIEW);
                #region navigation
                List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                actualPage.IdObject = schedaDocumento.docNumber;
                actualPage.OriginalObjectId = schedaDocumento.docNumber;

                actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT_TRANSMISSIONS.ToString(), string.Empty);
                actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT_TRANSMISSIONS.ToString(), true, this.Page);
                actualPage.CodePage = Navigation.NavigationUtils.NamePage.PROJECT_TRANSMISSIONS.ToString();

                actualPage.Page = "TRANSMISSIONSP.ASPX";
                navigationList.Add(actualPage);
                Navigation.NavigationUtils.SetNavigationList(navigationList);

                #endregion
                DocumentManager.setSelectedRecord(schedaDocumento);
                Response.Redirect("../Document/Document.aspx", false);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnImgCreaContributo_Click(object sender, EventArgs e)
        {
            try
            {
                string idTrasmSingola = this.idTrasmSingola.Value;
                this.idTrasmSingola.Value = string.Empty;
                Task task = UIManager.TaskManager.GetTaskByTrasmSingola(idTrasmSingola);

                string idTask = task.ID_TASK;
                string idObject = !string.IsNullOrEmpty(task.ID_PROFILE) ? task.ID_PROFILE : task.ID_PROJECT;
                string error = this.CreaContributo(task);
                if (string.IsNullOrEmpty(error))
                {
                    #region navigation
                    List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                    Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                    actualPage.IdObject = ProjectManager.getProjectInSession().systemID;
                    actualPage.OriginalObjectId = ProjectManager.getProjectInSession().systemID;

                    actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT_TRANSMISSIONS.ToString(), string.Empty);
                    actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT_TRANSMISSIONS.ToString(), true, this.Page);
                    actualPage.CodePage = Navigation.NavigationUtils.NamePage.PROJECT_TRANSMISSIONS.ToString();

                    actualPage.Page = "TRANSMISSIONSP.ASPX";
                    navigationList.Add(actualPage);
                    Navigation.NavigationUtils.SetNavigationList(navigationList);
                    #endregion
                    TaskSelected = task;
                    Response.Redirect("~/Document/Document.aspx", false);
                }
                else if (error == "AnswerChooseRecipient")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "AnswerChooseRecipient", "ajaxModalPopupAnswerChooseRecipient();", true);
                }
                else
                {
                    TaskSelected = null;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('" + error.Replace("'", @"\'") + "', 'warning', '','',null,null,'')", true);
                }
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
        #region CREA CONTRIBUTO
        private string CreaContributo(Task task)
        {
            try
            {
                string msg = string.Empty;
                string idProject = task.ID_PROJECT;
                string idProfile = task.ID_PROFILE;
                Templates templateToMerge = null;
                SchedaDocumento document = UIManager.DocumentManager.NewSchedaDocumento();
                document.oggetto = new Oggetto();
                document.tipoProto = "G";
                string idTipoAtto = task.ID_TIPO_ATTO;
                if (!string.IsNullOrEmpty(idTipoAtto))
                {
                    //Devo aggiungere controlli di visibilità della tipologia
                    document.template = UIManager.DocumentManager.getTemplateById(idTipoAtto, UserManager.GetInfoUser());
                    document.oggetto.descrizione = document.template.DESCRIZIONE + " - ";
                    templateToMerge = !string.IsNullOrEmpty(idProfile) ? ProfilerDocManager.getTemplateDettagli(idProfile) : UIManager.ProfilerProjectManager.getTemplateFascDettagli(idProject);
                    if (templateToMerge != null)
                    {
                        document.template = MappingTemplates(templateToMerge, document.template);
                    }
                }

                if (!string.IsNullOrEmpty(idProject))
                {
                    Fascicolo fascicolo = UIManager.ProjectManager.getFascicoloById(idProject);
                    UIManager.ProjectManager.setProjectInSession(fascicolo);
                    if (fascicolo == null || ProjectManager.CheckRevocationAcl())
                    {
                        ProjectManager.setProjectInSession(null);
                        msg = "RevocationAclIndex";
                        return msg;
                    }
                    document.oggetto.descrizione += fascicolo.descrizione;
                    HttpContext.Current.Session["DocumentAnswerFromProject"] = true;
                }
                else
                {
                    SchedaDocumento schedaDocDiPartenza = DocumentManager.getDocumentDetails(this, idProfile, idProfile);
                    document.oggetto.descrizione += schedaDocDiPartenza.oggetto.descrizione;
                    DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(schedaDocDiPartenza);
                    document.rispostaDocumento = infoDoc;
                    switch (schedaDocDiPartenza.tipoProto)
                    {
                        case "A":
                            document.tipoProto = "P";
                            document.protocollo = new DocsPaWR.ProtocolloUscita();
                            document.registro = schedaDocDiPartenza.registro;
                            ((DocsPaWR.ProtocolloUscita)document.protocollo).destinatari = new DocsPaWR.Corrispondente[1];
                            ((DocsPaWR.ProtocolloUscita)document.protocollo).destinatari[0] = new DocsPaWR.Corrispondente();
                            ((DocsPaWR.ProtocolloUscita)document.protocollo).destinatari[0] = ((DocsPaWR.ProtocolloEntrata)schedaDocDiPartenza.protocollo).mittente;
                            if (EnableSenderDefault())
                            {
                                DocsPaWR.Corrispondente corr = RoleManager.GetRoleInSession().uo;
                                ((DocsPaWR.ProtocolloUscita)document.protocollo).mittente = corr;
                            }
                            break;
                        case "P":
                            document.tipoProto = "A";
                            document.protocollo = new DocsPaWR.ProtocolloEntrata();
                            document.registro = schedaDocDiPartenza.registro;
                            if (EnableSenderDefault())
                            {
                                if (((DocsPaWR.ProtocolloUscita)schedaDocDiPartenza.protocollo).destinatari.Count() > 1)
                                {
                                    this.SchedaDocContributo = document;
                                    UIManager.DocumentManager.setSelectedRecord(schedaDocDiPartenza);
                                    msg = "AnswerChooseRecipient";
                                    return msg;
                                }
                                else
                                    ((DocsPaWR.ProtocolloEntrata)document.protocollo).mittente = ((DocsPaWR.ProtocolloUscita)schedaDocDiPartenza.protocollo).destinatari[0];
                            }
                            break;
                        case "I":
                            document.tipoProto = "I";
                            document.protocollo = new DocsPaWR.ProtocolloInterno();
                            document.registro = schedaDocDiPartenza.registro;
                            if (EnableSenderDefault())
                            {
                                ((DocsPaWR.ProtocolloInterno)document.protocollo).mittente = ((DocsPaWR.ProtocolloInterno)schedaDocDiPartenza.protocollo).mittente;
                            }
                            ((DocsPaWR.ProtocolloInterno)document.protocollo).destinatari = ((DocsPaWR.ProtocolloInterno)schedaDocDiPartenza.protocollo).destinatari;
                            break;
                    }
                }
                UIManager.DocumentManager.setSelectedRecord(document);
                return msg;

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return ex.Message;
            }

        }

        private SchedaDocumento SchedaDocContributo
        {
            get
            {
                SchedaDocumento result = null;
                if (HttpContext.Current.Session["SchedaDocContributo"] != null)
                {
                    result = HttpContext.Current.Session["SchedaDocContributo"] as SchedaDocumento;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["SchedaDocContributo"] = value;
            }
        }

        private DocsPaWR.Task TaskSelected
        {
            get
            {
                if (HttpContext.Current.Session["Task"] != null)
                {
                    return HttpContext.Current.Session["Task"] as DocsPaWR.Task;
                }
                return null;
            }
            set
            {
                HttpContext.Current.Session["Task"] = value;
            }
        }

        private bool EnableSenderDefault()
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.MITTENTE_DEFAULT.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.MITTENTE_DEFAULT.ToString()].Equals("1"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Templates MappingTemplates(Templates templateOrigin, Templates templateDest)
        {
            DocsPaWR.OggettoCustom oggettoCustomTemp;
            for (int i = 0; i < templateDest.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)templateDest.ELENCO_OGGETTI[i];
                oggettoCustomTemp = (from ogg in templateOrigin.ELENCO_OGGETTI
                                     where ogg.TIPO.DESCRIZIONE_TIPO.Equals(oggettoCustom.TIPO.DESCRIZIONE_TIPO) && ogg.DESCRIZIONE.Equals(oggettoCustom.DESCRIZIONE)
                                     select ogg).FirstOrDefault();
                if (oggettoCustomTemp != null)
                {
                    oggettoCustom.VALORE_DATABASE = oggettoCustomTemp.VALORE_DATABASE;
                    oggettoCustom.VALORI_SELEZIONATI = oggettoCustomTemp.VALORI_SELEZIONATI;
                }
            }
            return templateDest;
        }

        #endregion
        protected void btnImgCloseTask_Click(object sender, EventArgs e)
        {
            try
            {
                //Completa Lavorazione
                string idTrasmSingola = this.idTrasmSingola.Value;
                this.idTrasmSingola.Value = string.Empty;
                Task task = UIManager.TaskManager.GetTaskByTrasmSingola(idTrasmSingola);

                bool requiredReview = string.IsNullOrEmpty(task.ID_PROFILE_REVIEW) && task.CONTRIBUTO_OBBLIGATORIO.Equals("1");
                if (requiredReview)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('WarningTaskRequiredReview', 'warning', '','',null,null,'')", true);
                    return;
                }
                this.TaskSelected = task;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CompleteTask", "ajaxModalPopupCompleteTask();", true);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private string NoteCompleteTask
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["NoteCompleteTask"] != null)
                {
                    result = HttpContext.Current.Session["NoteCompleteTask"] as String;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["NoteCompleteTask"] = value;
            }
        }

        protected void btnImgBlockTask_Click(object sender, EventArgs e)
        {
            try
            {
                string idTrasmSingola = this.idTrasmSingola.Value;
                this.idTrasmSingola.Value = string.Empty;
                this.TaskSelected = UIManager.TaskManager.GetTaskByTrasmSingola(idTrasmSingola);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmCancelTask', 'HiddenCancelTask', '');", true);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
        protected void btnImgRiapriLavorazione_Click(object sender, EventArgs e)
        {
            try
            {
                string idTrasmSingola = this.idTrasmSingola.Value;
                this.idTrasmSingola.Value = string.Empty;
                this.TaskSelected = UIManager.TaskManager.GetTaskByTrasmSingola(idTrasmSingola);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ReopenTask", "ajaxModalPopupReopenTask();", true);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
        protected void btnImgRemoveTask_Click(object sender, EventArgs e)
        {
            try
            {
                string idTrasmSingola = this.idTrasmSingola.Value;
                this.idTrasmSingola.Value = string.Empty;
                this.TaskSelected = UIManager.TaskManager.GetTaskByTrasmSingola(idTrasmSingola);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmRemoveTask', 'HiddenRemoveTask', '');", true);

            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
        #endregion

        private System.Web.UI.HtmlControls.HtmlGenericControl BuildNoteTask(string noteAccettazione, string idTrasmUtente)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            noteAccettazione = noteAccettazione.Replace(TaskManager.TagIdContributo.LABEL_ATTIVITA_CONCLUSA, Utils.Languages.GetLabelFromCode("TaskNote", language));
            noteAccettazione = noteAccettazione.Replace(TaskManager.TagIdContributo.LABEL_ATTIVITA_CONCLUSA_C, "");
            string idContributo = string.Empty;
            int indexFrom;
            int indexTo;
            Panel pnlContentNote = new Panel();
            pnlContentNote.ID = "pnlContentNote_" + idTrasmUtente;
            pnlContentNote.Attributes.Add("class", "fieldNotesAcc");

            string[] splitNote = noteAccettazione.Split(new string[] { TaskManager.TagIdContributo.LABEL_TEXT_WRAP }, StringSplitOptions.None);
            string notaElaborata = string.Empty;
            foreach (string nota in splitNote)
            {
                idContributo = string.Empty;
                if (!string.IsNullOrEmpty(nota))
                {
                    if (nota.Contains(TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO))
                    {
                        indexFrom = nota.IndexOf(TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO) + TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO.Length;
                        indexTo = nota.IndexOf(TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO_C) - (nota.IndexOf(TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO) + TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO.Length);
                        idContributo = nota.Substring(indexFrom, indexTo);
                        notaElaborata = nota.Replace(TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO, Utils.Languages.GetLabelFromCode("TaskIdContributo", language)).Replace(TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO_C, "");
                    }
                    Label note1 = new Label();
                    Label note2 = new Label();
                    if (!string.IsNullOrEmpty(idContributo))
                    {
                        string[] split = notaElaborata.Split(new string[] { idContributo }, StringSplitOptions.None);
                        note1.Text = formatBlankValue(split[0]);
                        note2.Text = formatBlankValue(split[1]) + "<br /><br />";
                    }
                    else
                    {
                        notaElaborata = nota;
                        note1.Text = notaElaborata + "<br /><br />";
                    }

                    //Creo un LinkButton all'idContributo
                    LinkButton lnkIdContributo = new LinkButton();
                    lnkIdContributo.ID = idContributo;
                    lnkIdContributo.Text = idContributo;
                    lnkIdContributo.Attributes["onClick"] = "$('#idContributo').val('" + lnkIdContributo.ID + "');__doPostBack('panelButtons');return false;";
                    lnkIdContributo.ToolTip = Utils.Languages.GetLabelFromCode("IndexDetailsDocTooltip", UIManager.UserManager.GetUserLanguage());
                    lnkIdContributo.CssClass = "clickableLeft";

                    pnlContentNote.Controls.Add(note1);
                    pnlContentNote.Controls.Add(lnkIdContributo);
                    pnlContentNote.Controls.Add(note2);
                }
            }

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshExpand", "UpdateExpand();", true);
            System.Web.UI.HtmlControls.HtmlGenericControl divContent = new System.Web.UI.HtmlControls.HtmlGenericControl();
            divContent.Attributes.Add("class", "contentNoteTask");

            System.Web.UI.HtmlControls.HtmlGenericControl fieldset = new System.Web.UI.HtmlControls.HtmlGenericControl("fieldset");

            System.Web.UI.HtmlControls.HtmlGenericControl h2 = new System.Web.UI.HtmlControls.HtmlGenericControl("h2");
            h2.Attributes.Add("class", "expand");

            System.Web.UI.HtmlControls.HtmlGenericControl div2 = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            div2.ID = "divContentNoteAcc" + idTrasmUtente;
            div2.Attributes.Add("class", "collapse");

            h2.InnerHtml = Utils.Languages.GetLabelFromCode("TaskExtendNotes", language);
            //h2.Controls.Add(pnlContentNote1);
            div2.Controls.Add(pnlContentNote);
            fieldset.Controls.Add(h2);
            fieldset.Controls.Add(div2);
            divContent.Controls.Add(fieldset);
            return divContent;
        }

        public Control GetControlById(Control owner, string controlID)
        {
            Control myControl = null;
            // cycle all controls
            if (owner.Controls.Count > 0)
            {
                foreach (Control c in owner.Controls)
                {
                    myControl = GetControlById(c, controlID);
                    if (myControl != null) return myControl;
                }
            }
            if (controlID.Equals(owner.ID)) return owner;
            return null;
        }

        public Table GetTransmissionTable(string id)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();

                Table tbl = new Table();
                tbl.CssClass = "tbl_rounded";


                {// header
                    TableRow row = new TableRow();

                    TableCell cell1 = new TableCell();
                    cell1.ColumnSpan = 7;
                    cell1.CssClass = "th first";
                    cell1.Text = Utils.Languages.GetLabelFromCode("TransmissionLitDetailsRecipient", language);
                    row.Controls.Add(cell1);

                    tbl.Controls.Add(row);
                }

                {// recipient header
                    TableRow row = new TableRow();
                    row.CssClass = "header";

                    TableCell cell1 = new TableCell();
                    cell1.ColumnSpan = 2;
                    cell1.CssClass = "first";
                    cell1.Text = Utils.Languages.GetLabelFromCode("TransmissionLitDescription", language);
                    row.Controls.Add(cell1);

                    TableCell cell2 = new TableCell();
                    cell2.CssClass = "center trasmDetailReason";
                    cell2.Text = Utils.Languages.GetLabelFromCode("TransmissionLitReason", language);
                    row.Controls.Add(cell2);

                    TableCell cell3 = new TableCell();
                    cell3.CssClass = "center trasmDetailType";
                    cell3.Text = Utils.Languages.GetLabelFromCode("TransmissionLitType", language);
                    cell3.ColumnSpan = 2;
                    row.Controls.Add(cell3);

                    TableCell cell4 = new TableCell();
                    cell4.CssClass = "trasmDetailNote";
                    cell4.Text = Utils.Languages.GetLabelFromCode("TransmissionLitNoteShort", language);
                    row.Controls.Add(cell4);

                    TableCell cell5 = new TableCell();
                    cell5.CssClass = "center trasmDetailDate";
                    cell5.Text = Utils.Languages.GetLabelFromCode("TransmissionLitExpire", language);
                    row.Controls.Add(cell5);

                    tbl.Controls.Add(row);
                }

                {// recipient data
                    TableRow row = new TableRow();

                    TableCell cell1 = new TableCell();
                    cell1.ID = "trasmDetailsRecipient" + id;
                    cell1.ColumnSpan = 2;
                    cell1.CssClass = "first";
                    cell1.Text = Utils.Languages.GetLabelFromCode("TransmissionLitDetailsRecipient", language);
                    row.Controls.Add(cell1);

                    TableCell cell2 = new TableCell();
                    cell2.ID = "trasmDetailsReason" + id;
                    cell2.CssClass = "center";
                    cell2.Text = Utils.Languages.GetLabelFromCode("TransmissionLitReason", language);
                    row.Controls.Add(cell2);

                    TableCell cell3 = new TableCell();
                    cell3.ID = "trasmDetailsType" + id;
                    cell3.CssClass = "center";
                    cell3.ColumnSpan = 2;
                    cell3.Text = Utils.Languages.GetLabelFromCode("TransmissionLitType", language);
                    row.Controls.Add(cell3);

                    TableCell cell4 = new TableCell();
                    cell4.ID = "trasmDetailsNote" + id;
                    cell4.Text = Utils.Languages.GetLabelFromCode("TransmissionLitNoteShort", language);
                    row.Controls.Add(cell4);

                    TableCell cell5 = new TableCell();
                    cell5.ID = "trasmDetailsExpire" + id;
                    cell5.CssClass = "center";
                    cell5.Text = Utils.Languages.GetLabelFromCode("TransmissionLitExpire", language);
                    row.Controls.Add(cell5);

                    tbl.Controls.Add(row);
                }

                {// users header
                    TableRow row = new TableRow();
                    row.ID = "row_users" + id;
                    row.CssClass = "header2";

                    TableCell cell1 = new TableCell();
                    cell1.CssClass = "first";
                    cell1.Text = Utils.Languages.GetLabelFromCode("TransmissionLitUser", language);
                    row.Controls.Add(cell1);

                    TableCell cell2 = new TableCell();
                    cell2.CssClass = "center trasmDetailDate";
                    cell2.Text = Utils.Languages.GetLabelFromCode("TransmissionLitViewedOn", language);
                    row.Controls.Add(cell2);

                    TableCell cell3 = new TableCell();
                    cell3.CssClass = "center trasmDetailDate";
                    cell3.Text = Utils.Languages.GetLabelFromCode("TransmissionLitAcceptedOn", language);
                    row.Controls.Add(cell3);

                    TableCell cell4 = new TableCell();
                    cell4.CssClass = "center trasmDetailDate";
                    cell4.Text = Utils.Languages.GetLabelFromCode("TransmissionLitRejectedOn", language);
                    row.Controls.Add(cell4);

                    TableCell cell5 = new TableCell();
                    cell5.CssClass = "center trasmDetailDate";
                    cell5.Text = Utils.Languages.GetLabelFromCode("TransmissionLitRemoved", language);
                    row.Controls.Add(cell5);

                    TableCell cell6 = new TableCell();
                    cell6.ColumnSpan = 2;
                    cell6.CssClass = "center";
                    cell6.Text = Utils.Languages.GetLabelFromCode("TransmissionLitInfoAccRej", language);
                    row.Controls.Add(cell6);

                    tbl.Controls.Add(row);
                }

                return tbl;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public TableRow GetDetailsRow(string id)
        {
            TableRow row = new TableRow();
            row.CssClass = "users";

            TableCell cellUser = new TableCell();
            cellUser.ID = "trasmDetailsUser" + id;
            cellUser.CssClass = "first";
            row.Cells.Add(cellUser);

            TableCell cellViewed = new TableCell();
            cellViewed.ID = "trasmDetailsViewed" + id;
            cellViewed.CssClass = "center";
            row.Cells.Add(cellViewed);

            TableCell cellAccepted = new TableCell();
            cellAccepted.ID = "trasmDetailsAccepted" + id;
            cellAccepted.CssClass = "center";
            row.Cells.Add(cellAccepted);

            TableCell cellRif = new TableCell();
            cellRif.ID = "trasmDetailsRif" + id;
            cellRif.CssClass = "center";
            row.Cells.Add(cellRif);

            TableCell cellRemoved = new TableCell();
            cellRemoved.ID = "trasmDetailsRemoved" + id;
            cellRemoved.CssClass = "center";
            row.Cells.Add(cellRemoved);

            TableCell cellInfo = new TableCell();
            cellInfo.ID = "trasmDetailsInfo" + id;
            cellInfo.ColumnSpan = 2;
            row.Cells.Add(cellInfo);

            return row;
        }

        private string formatBlankValue(string valore)
        {
            string retValue = "&nbsp;";

            if (valore != null && valore != "")
            {
                retValue = valore;
            }

            return retValue;
        }

        /// <summary>
        /// verifica se la trasmissione è stata effettuata 
        /// dall'utente correntemente connesso
        /// </summary>
        /// <returns></returns>
        private bool CheckTrasmEffettuataDaUtenteCorrente(Trasmissione trasm)
        {
            bool retValue = false;

            if (trasm != null && trasm.utente != null) retValue = (trasm.utente.idPeople.Equals(UserManager.GetInfoUser().idPeople));

            return retValue;
        }

        /// <summary>
        /// verifica se l'utente relativo ad una trasmissione utente sia lo
        /// stesso soggetto correntemente connesso all'applicazione
        /// </summary>
        /// <param name="trasmUtente"></param>
        private bool CheckTrasmUtenteCorrente(DocsPaWR.TrasmissioneUtente trasmUtente)
        {
            bool retValue = false;

            if (trasmUtente.utente != null)
            {
                retValue = (trasmUtente.utente.idPeople.Equals(UserManager.GetInfoUser().idPeople));
            }

            return retValue;
        }

        protected void TrasmissionsImgAddressBook_Click(object sender, EventArgs e)
        {
            try
            {
                this.CallType = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
                HttpContext.Current.Session["AddressBook.from"] = "T_S_R_S";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpPnlSender", "ajaxModalPopupAddressBook();", true);
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
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                if (atList != null && atList.Count > 0)
                {
                    Corrispondente tempCorr = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                    this.TxtCodeRecipientTransmission.Text = tempCorr.codiceRubrica;
                    this.TxtDescriptionRecipient.Text = tempCorr.descrizione;

                    if (tempCorr.GetType() == typeof(DocsPaWR.Utente))
                    {
                        this.IdRecipient.Value = tempCorr.codiceRubrica + "|" + tempCorr.idAmministrazione;
                        this.RecipientTypeOfCorrespondent.Value = "U";
                    }
                    else if (tempCorr.GetType() == typeof(DocsPaWR.Ruolo))
                    {
                        this.IdRecipient.Value = tempCorr.systemId;
                        this.RecipientTypeOfCorrespondent.Value = "R";
                    }

                    this.UpPnlRecipient.Update();
                }

                HttpContext.Current.Session["AddressBook.At"] = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void TransmissionsBtnTransmit_Click(object sender, EventArgs e)
        {
            if (ProjectManager.getProjectInSession().privato == "1")
            {
                string msg = "ConfirmTransmissionPrivateProject";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) { parent.fra_main.ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'proceed_private', '" + utils.FormatJs(this.GetLabel("TransmissionConfirm")) + "');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'proceed_private', '" + utils.FormatJs(this.GetLabel("TransmissionConfirm")) + "');}", true);
            }
            else
            {
                this.BeginTransmissionIfAllowed();

                this.EnableButtons();
                this.HeaderProject.RefreshHeader();
                this.ProjectTabs.RefreshProjectabs();
                this.UpContainerProjectTab.Update();
            }
            //    logger.Info("END");
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }


        private string GetLabel(string labelId)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(labelId, language);
        }

        private void BeginTransmissionIfAllowed()
        {
            try
            {
                string valoreChiaveCediDiritti = InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_CEDI_DIRITTI_IN_RUOLO.ToString());

                if (!string.IsNullOrEmpty(valoreChiaveCediDiritti) && valoreChiaveCediDiritti.Equals("1"))
                {
                    string accessRights = string.Empty;
                    string idGruppoTrasm = string.Empty;
                    string tipoDiritto = string.Empty;
                    string IDObject = string.Empty;

                    bool isPersonOwner = false;

                    Fascicolo prj = UIManager.ProjectManager.getProjectInSession();
                    IDObject = prj.systemID;

                    TrasmManager.SelectSecurity(IDObject, UserManager.GetInfoUser().idPeople, "= 0", out accessRights, out idGruppoTrasm, out tipoDiritto);

                    isPersonOwner = (accessRights.Equals("0"));
                    if (!isPersonOwner)
                    {
                        string idProprietario = GetAnagUtenteProprietario();
                        Utente _utproprietario = UserManager.GetUtenteByIdPeople(idProprietario);
                        string msg = "ConfirmTransmissionProceedOwnership";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'proceed_ownership', '');} else {parent.ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'proceed_ownership', '');}", true);
                    }
                    else
                    {
                        this.PerformActionTransmit();
                    }
                }
                else
                {
                    this.PerformActionTransmit();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private void PerformActionTransmit()
        {
            bool isModel = (this.DdlTransmissionsModel.SelectedValue != "");

            if (isModel)
            {
                this.TransmitModel();
            }
            else
            {
                this.PerformActionTransmitProject();
            }
        }

        /// <summary>
        /// Get dell'idProprietario del fascicolo
        /// </summary>
        /// <returns></returns>
        private string GetAnagUtenteProprietario()
        {
            try
            {
                FascicoloDiritto[] listaVisibilita = null;
                Fascicolo prj = ProjectManager.getProjectInSession();
                string idProprietario = string.Empty;
                InfoFascicolo infoFasc = new InfoFascicolo();
                string rootFolder = ProjectManager.GetRootFolderFasc(prj);

                listaVisibilita = ProjectManager.getListaVisibilitaSemplificata(infoFasc, true, rootFolder);

                if (listaVisibilita != null && listaVisibilita.Length > 0)
                {
                    for (int i = 0; i < listaVisibilita.Length; i++)
                    {
                        if (listaVisibilita[i].accessRights == 0)
                        {
                            return idProprietario = listaVisibilita[i].personorgroup;
                        }
                    }
                }
                return "";
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        private void TransmitModel()
        {
            ModelloTrasmissione modello = TrasmManager.GetTemplateById(this.Registry.idAmministrazione, this.DdlTransmissionsModel.SelectedValue);

            if (modello != null && modello.SYSTEM_ID != 0)
            {
                this.PerformActionTransmitModel();
            }
        }

        /// <summary>
        /// Azione di trasmissione del documento corrente
        /// </summary>
        private void PerformActionTransmitProject()
        {
            try
            {
                DocsPaWR.Trasmissione trasmEff = TrasmManager.getSelectedTransmission();
                if (trasmEff != null && trasmEff.utente != null && string.IsNullOrEmpty(trasmEff.utente.idAmministrazione)) trasmEff.utente.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;

                if (trasmEff != null)
                {
                    trasmEff = ExtendVisibility(trasmEff);

                    if (trasmEff.trasmissioniSingole[0].ragione.prevedeCessione == "R")
                    {

                        trasmEff.cessione = new CessioneDocumento();
                        trasmEff.cessione.docCeduto = true;
                        trasmEff.cessione.idPeople = UserManager.GetInfoUser().idPeople;
                        trasmEff.cessione.idRuolo = UserManager.GetInfoUser().idGruppo;
                        trasmEff.cessione.idPeopleNewPropr = trasmEff.trasmissioniSingole[0].trasmissioneUtente[0].utente.idPeople;
                        trasmEff.cessione.idRuoloNewPropr = ((DocsPaWR.Ruolo)trasmEff.trasmissioniSingole[0].corrispondenteInterno).idGruppo;
                        trasmEff.cessione.userId = UserManager.GetInfoUser().userId;
                    }

                    if (string.IsNullOrEmpty(trasmEff.dataInvio)) trasmEff = TrasmManager.executeTrasm(this, trasmEff);

                    if (trasmEff != null && trasmEff.ErrorSendingEmails)
                    {
                        string msg = "ErrorTransmissionSendingEmails";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning', 'Trasmissione');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning', 'Trasmissione');}", true);
                    }
                }

                TrasmManager.PerformAutomaticStateChange(trasmEff);

                //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
                if (ProjectManager.CheckRevocationAcl())
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()');}", true);
                    return;
                }

                this.ResetHiddenFields();

                this.grdList_Bind();
                this.upPnlGridList.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private Trasmissione BuildTransmissionFromModel_new()
        {
            try
            {
                Trasmissione transmission = new Trasmissione();

                if (!string.IsNullOrEmpty(this.DdlTransmissionsModel.SelectedValue))
                {
                    this.Template = TransmissionModelsManager.GetTemplateById(UserManager.GetInfoUser().idAmministrazione, this.DdlTransmissionsModel.SelectedValue);

                    if (this.Template != null)
                        transmission.NO_NOTIFY = this.Template.NO_NOTIFY;

                    // gestione della cessione diritti
                    if (this.Template.CEDE_DIRITTI != null && this.Template.CEDE_DIRITTI.Equals("1"))
                    {
                        DocsPaWR.CessioneDocumento objCessione = new DocsPaWR.CessioneDocumento();

                        objCessione.docCeduto = true;
                        objCessione.idPeople = UserManager.GetInfoUser().idPeople;
                        objCessione.idRuolo = UserManager.GetInfoUser().idGruppo;
                        objCessione.idPeopleNewPropr = this.Template.ID_PEOPLE_NEW_OWNER;
                        objCessione.idRuoloNewPropr = this.Template.ID_GROUP_NEW_OWNER;
                        objCessione.userId = UserManager.GetInfoUser().userId;

                        transmission.cessione = objCessione;
                    }

                    //Parametri della trasmissione
                    transmission.noteGenerali = this.Template.VAR_NOTE_GENERALI;
                    transmission.tipoOggetto = DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
                    transmission.infoFascicolo = ProjectManager.getInfoFascicoloDaFascicolo(UIManager.ProjectManager.getProjectInSession());

                    //Parametri delle trasmissioni singole
                    for (int i = 0; i < this.Template.RAGIONI_DESTINATARI.Length; i++)
                    {
                        DocsPaWR.RagioneDest ragDest = (DocsPaWR.RagioneDest)this.Template.RAGIONI_DESTINATARI[i];
                        ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                        for (int j = 0; j < destinatari.Count; j++)
                        {

                            DocsPaWR.MittDest mittDest = (DocsPaWR.MittDest)destinatari[j];
                            DocsPaWR.RagioneTrasmissione ragione = TrasmManager.getReasonById(mittDest.ID_RAGIONE.ToString());
                            DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                            if (corr != null) //corr nullo se non esiste o se è stato disabilitato                           

                                //Andrea - try - catch
                                try
                                {
                                    transmission = addTrasmissioneSingola(transmission, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, mittDest);
                                }
                                catch (ExceptionTrasmissioni e)
                                {
                                    //Aggiungo l'errore alla lista
                                    listaExceptionTrasmissioni.Add(e.Messaggio);
                                }
                            //End Andrea
                        }

                    }

                    if (this.Template.CEDE_DIRITTI != null && this.Template.CEDE_DIRITTI.Equals("1"))
                        transmission.trasmissioniSingole[0].ragione.cessioneImpostata = true;
                }

                return transmission;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        private Trasmissione BuildTransmissionFromModel()
        {

            logger.Info("BEGIN");

            DocsPaWR.Trasmissione trasmissione = new DocsPaWR.Trasmissione();

            try
            {
                DocsPaWR.ModelloTrasmissione template = TransmissionModelsManager.GetTemplateById(UserManager.GetInfoUser().idAmministrazione, this.DdlTransmissionsModel.SelectedValue);
                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

                if (template != null)
                    trasmissione.NO_NOTIFY = template.NO_NOTIFY;

                // gestione della cessione diritti
                if (template.CEDE_DIRITTI != null && template.CEDE_DIRITTI.Equals("1"))
                {
                    DocsPaWR.CessioneDocumento objCessione = new DocsPaWR.CessioneDocumento();

                    objCessione.docCeduto = true;
                    objCessione.idPeople = UserManager.GetInfoUser().idPeople;
                    objCessione.idRuolo = UserManager.GetInfoUser().idGruppo;
                    objCessione.idPeopleNewPropr = template.ID_PEOPLE_NEW_OWNER;
                    objCessione.idRuoloNewPropr = template.ID_GROUP_NEW_OWNER;
                    objCessione.userId = UserManager.GetInfoUser().userId;

                    if (!string.IsNullOrEmpty(template.MANTIENI_SCRITTURA) && !string.IsNullOrEmpty(template.MANTIENI_LETTURA))
                        if (Convert.ToBoolean(int.Parse(template.MANTIENI_SCRITTURA)))
                        {
                            trasmissione.mantieniLettura = true;
                            trasmissione.mantieniScrittura = true;
                        }
                        else
                        {
                            trasmissione.mantieniScrittura = false;
                            if (Convert.ToBoolean(int.Parse(template.MANTIENI_LETTURA)))
                                trasmissione.mantieniLettura = true;
                            else
                                trasmissione.mantieniLettura = false;
                        }

                    trasmissione.cessione = objCessione;
                }

                //Parametri della trasmissione
                trasmissione.noteGenerali = template.VAR_NOTE_GENERALI;
                trasmissione.tipoOggetto = DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
                trasmissione.infoFascicolo = ProjectManager.getInfoFascicoloDaFascicolo(UIManager.ProjectManager.getProjectInSession());

                //Parametri delle trasmissioni singole
                for (int i = 0; i < template.RAGIONI_DESTINATARI.Length; i++)
                {
                    DocsPaWR.RagioneDest ragDest = (DocsPaWR.RagioneDest)template.RAGIONI_DESTINATARI[i];
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {

                        DocsPaWR.MittDest mittDest = (DocsPaWR.MittDest)destinatari[j];
                        DocsPaWR.RagioneTrasmissione ragione = TrasmManager.getReasonById(mittDest.ID_RAGIONE.ToString());
                        DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                        if (corr != null) //corr nullo se non esiste o se è stato disabilitato                           

                            //Andrea - try - catch
                            try
                            {
                                trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, mittDest);
                            }
                            catch (ExceptionTrasmissioni e)
                            {
                                //Aggiungo l'errore alla lista
                                listaExceptionTrasmissioni.Add(e.Messaggio);
                            }
                        //End Andrea
                    }

                }

                if (template.CEDE_DIRITTI != null && template.CEDE_DIRITTI.Equals("1"))
                    trasmissione.trasmissioniSingole[0].ragione.cessioneImpostata = true;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }

            //logger.Info("END");

            return trasmissione;
        }

        private void PerformActionTransmitModel()
        {
            Trasmissione trasmEff = this.BuildTransmissionFromModel();
            if (trasmEff != null && trasmEff.utente == null) trasmEff.utente = UserManager.GetUserInSession();
            if (trasmEff != null && trasmEff.ruolo == null) trasmEff.ruolo = UserManager.GetSelectedRole();

            this.Template = TrasmManager.getModelloTrasmNuovo(trasmEff, this.Registry.systemId);

            if (!this.notificheUtImpostate(this.Template))
            {
                this.DdlTransmissionsModel.SelectedIndex = -1;
                this.upPnlTransmissionsModel.Update();

                string msg = "ErrorTransmissionRecipientsNotify";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', 'Trasmissione modello');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', 'Trasmissione modello');}", true);

                return;
            }


            Trasmissione trasmissione = new DocsPaWR.Trasmissione();
            if (this.Template != null)
                trasmissione.NO_NOTIFY = trasmEff.NO_NOTIFY;

            //Parametri della trasmissione
            trasmissione.noteGenerali = this.Template.VAR_NOTE_GENERALI;
            trasmissione.tipoOggetto = DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
            trasmissione.infoFascicolo = ProjectManager.getInfoFascicoloDaFascicolo(UIManager.ProjectManager.getProjectInSession());
            trasmissione.utente = UserManager.GetUserInSession();
            trasmissione.ruolo = UserManager.GetSelectedRole();


            // gestione della cessione diritti
            if (this.Template.CEDE_DIRITTI != null && this.Template.CEDE_DIRITTI.Equals("1"))
            {
                DocsPaWR.CessioneDocumento objCessione = new DocsPaWR.CessioneDocumento();

                objCessione.docCeduto = true;

                //*******************************************************************************************
                // MODIFICA IACOZZILLI GIORDANO 30/07/2012
                // Modifica inerente la cessione dei diritti di un doc da parte di un utente non proprietario ma 
                // nel ruolo del proprietario, in quel caso non posso valorizzare l'IDPEOPLE  con il corrente perchè
                // il proprietario può essere un altro utente del mio ruolo, quindi andrei a generare un errore nella security,
                // devo quindi controllare che nell'idpeople venga inserito l'id corretto del proprietario.
                string valoreChiaveCediDiritti = InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_CEDI_DIRITTI_IN_RUOLO.ToString());
                if (!string.IsNullOrEmpty(valoreChiaveCediDiritti) && valoreChiaveCediDiritti.Equals("1"))
                {
                    //Devo istanziare una classe utente.
                    string idProprietario = string.Empty;
                    idProprietario = GetAnagUtenteProprietario();
                    Utente _utproprietario = UserManager.GetUtenteByIdPeople(idProprietario);

                    objCessione.idPeople = idProprietario;
                    objCessione.idRuolo = UserManager.GetInfoUser().idGruppo;
                    objCessione.userId = _utproprietario.cognome + " " + _utproprietario.nome;


                    if (objCessione.idPeople == null || objCessione.idPeople == "")
                        objCessione.idPeople = idProprietario;

                    if (objCessione.idRuolo == null || objCessione.idRuolo == "")
                        objCessione.idRuolo = UserManager.GetInfoUser().idGruppo;

                    if (objCessione.userId == null || objCessione.userId == "")
                        objCessione.userId = _utproprietario.cognome + " " + _utproprietario.nome;
                }
                else
                {
                    //OLD CODE:
                    objCessione.idPeople = UserManager.GetInfoUser().idPeople;
                    objCessione.idRuolo = UserManager.GetInfoUser().idGruppo;
                    objCessione.userId = UserManager.GetInfoUser().userId;
                }
                //*******************************************************************************************
                // FINE MODIFICA
                //********************************************************************************************
                if (this.Template.ID_PEOPLE_NEW_OWNER != null && this.Template.ID_PEOPLE_NEW_OWNER != "")
                    objCessione.idPeopleNewPropr = this.Template.ID_PEOPLE_NEW_OWNER;
                if (this.Template.ID_GROUP_NEW_OWNER != null && this.Template.ID_GROUP_NEW_OWNER != "")
                    objCessione.idRuoloNewPropr = this.Template.ID_GROUP_NEW_OWNER;

                trasmissione.cessione = objCessione;
                // Se per il modello creato è prevista l'opzione di mantenimento dei diritti di lettura
                if (trasmEff.cessione == null)
                    if (!string.IsNullOrEmpty(this.Template.MANTIENI_SCRITTURA) && this.Template.MANTIENI_SCRITTURA == "1")
                    {
                        trasmissione.mantieniScrittura = true;
                        trasmissione.mantieniLettura = true;
                    }
                    else
                    {
                        trasmissione.mantieniScrittura = false;
                        // Se per il modello creato è prevista l'opzione di mantenimento dei diritti di lettura
                        if (!string.IsNullOrEmpty(this.Template.MANTIENI_LETTURA) && this.Template.MANTIENI_LETTURA == "1")
                            trasmissione.mantieniLettura = true;
                        else
                            trasmissione.mantieniLettura = false;
                    }
                else
                {
                    trasmissione.cessione = trasmEff.cessione;
                    trasmissione.mantieniLettura = trasmEff.mantieniLettura;
                    trasmissione.mantieniScrittura = trasmEff.mantieniScrittura;
                }
                // End Mev
            }

            bool eredita = false;
            //Parametri delle trasmissioni singole

            for (int i = 0; i < this.Template.RAGIONI_DESTINATARI.Length; i++)
            {
                DocsPaWR.RagioneDest ragDest = (DocsPaWR.RagioneDest)this.Template.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPaWR.MittDest mittDest = (DocsPaWR.MittDest)destinatari[j];
                    DocsPaWR.Corrispondente corr = new Corrispondente();
                    if (mittDest.CHA_TIPO_MITT_DEST == "D")
                    {
                        corr = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    }
                    else
                    {
                        corr = TrasmManager.getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, null, ProjectManager.getProjectInSession(), this);
                    }

                    if (corr != null)
                    {
                        DocsPaWR.RagioneTrasmissione ragione = TrasmManager.getRagioneById(mittDest.ID_RAGIONE.ToString());

                        try
                        {
                            trasmissione = this.addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, mittDest.NASCONDI_VERSIONI_PRECEDENTI);
                        }
                        catch (ExceptionTrasmissioni e)
                        {
                            listaExceptionTrasmissioni.Add(e.Messaggio);
                        }

                        if (ragione.eredita == "1")
                            eredita = true;
                    }
                }
            }


            DocsPaWR.Trasmissione t_rs = null;
            if (trasmissione.trasmissioniSingole != null && trasmissione.trasmissioniSingole.Length > 0)
            {
                trasmissione = this.impostaNotificheUtentiDaModello(trasmissione);
                ExtendVisibility(trasmissione);

                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                if (infoUtente.delegato != null)
                    trasmissione.delegato = ((DocsPaWR.InfoUtente)(infoUtente.delegato)).idPeople;

                t_rs = TrasmManager.saveExecuteTrasm(this, trasmissione, infoUtente);
            }


            if (t_rs != null && t_rs.ErrorSendingEmails)
            {
                string msg = "ErrorTransmissionSendingEmails";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
            }
            //if (t_rs != null && !t_rs.dirittiCeduti && this.Template.CEDE_DIRITTI.Equals("1"))
            //{
            //    string msg = "ErrorTransmissionSaleOfRights";
            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
            //    return;
            //}


            if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
            {
                Fascicolo prj = UIManager.ProjectManager.getProjectInSession();

                DocsPaWR.Stato stato = ProjectManager.getStatoFasc(prj);
                bool trasmWF = false;
                if (trasmissione != null && trasmissione.trasmissioniSingole != null &&
                    trasmissione.trasmissioniSingole.Length > 0)
                {
                    for (int j = 0; j < trasmissione.trasmissioniSingole.Length; j++)
                    {
                        DocsPaWR.TrasmissioneSingola trasmSing = (DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[j];
                        if (trasmSing.ragione.tipo == "W")
                            trasmWF = true;
                    }
                }
                if (stato != null && trasmWF)
                    TrasmManager.salvaStoricoTrasmDiagrammi(trasmissione.systemId, prj.systemID, Convert.ToString(stato.SYSTEM_ID));
            }




            //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
            if (ProjectManager.CheckRevocationAcl())
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()');}", true);
                return;
            }
            else
            {
                if (trasmissione.cessione != null && trasmissione.cessione.docCeduto)
                    if (trasmissione.mantieniLettura)
                        ProjectManager.setProjectInSession(ProjectManager.GetProjectByCode(RegistryManager.getRegistroBySistemId(ProjectManager.getProjectInSession().idRegistro), ProjectManager.getProjectInSession().codice));
            }


            this.DdlTransmissionsModel.SelectedIndex = -1;
            this.upPnlTransmissionsModel.Update();
            this.ResetHiddenFields();
            this.SelectedRowIndex = 0;
            this.grid_rowindex.Value = "0";
            this.grdList_Bind();
            this.upPnlGridList.Update();
        }

        private void ResetHiddenFields()
        {
            this.extend_visibility.Value = string.Empty;
            this.proceed_personal.Value = string.Empty;
            this.proceed_private.Value = string.Empty;
            this.proceed_ownership.Value = string.Empty;
        }

        private DocsPaWR.Trasmissione impostaNotificheUtentiDaModello(DocsPaWR.Trasmissione objTrasm)
        {
            if (objTrasm.trasmissioniSingole != null && objTrasm.trasmissioniSingole.Length > 0)
            {
                for (int cts = 0; cts < objTrasm.trasmissioniSingole.Length; cts++)
                {
                    if (objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length > 0)
                    {
                        for (int ctu = 0; ctu < objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length; ctu++)
                        {
                            objTrasm.trasmissioniSingole[cts].trasmissioneUtente[ctu].daNotificare = this.daNotificareSuModello(objTrasm.trasmissioniSingole[cts].trasmissioneUtente[ctu].utente.idPeople, objTrasm.trasmissioniSingole[cts].corrispondenteInterno.systemId);
                        }
                    }
                }
            }

            return objTrasm;
        }

        private bool daNotificareSuModello(string currentIDPeople, string currentIDCorrGlobRuolo)
        {
            bool retValue = true;

            for (int i = 0; i < this.Template.RAGIONI_DESTINATARI.Length; i++)
            {
                DocsPaWR.RagioneDest ragDest = (DocsPaWR.RagioneDest)this.Template.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPaWR.MittDest mittDest = (DocsPaWR.MittDest)destinatari[j];
                    if (mittDest.ID_CORR_GLOBALI.Equals(Convert.ToInt32(currentIDCorrGlobRuolo)))
                    {
                        if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length > 0)
                        {
                            for (int cut = 0; cut < mittDest.UTENTI_NOTIFICA.Length; cut++)
                            {
                                if (mittDest.UTENTI_NOTIFICA[cut].ID_PEOPLE.Equals(currentIDPeople))
                                {
                                    if (mittDest.UTENTI_NOTIFICA[cut].FLAG_NOTIFICA.Equals("1"))
                                        retValue = true;
                                    else
                                        retValue = false;

                                    return retValue;
                                }
                            }
                        }
                    }
                }
            }
            return retValue;
        }

        /// <summary>
        /// Check if visibility is extended: seems valued only for models 
        /// </summary>
        /// <param name="trasmEff"></param>
        /// <returns></returns>
        private Trasmissione ExtendVisibility(Trasmissione trasmEff)
        {
            if (this.extend_visibility.Value == "false")
            {
                TrasmissioneSingola[] appoTrasmSingole = new TrasmissioneSingola[trasmEff.trasmissioniSingole.Length];

                for (int i = 0; i < trasmEff.trasmissioniSingole.Length; i++)
                {
                    TrasmissioneSingola trasmSing = new TrasmissioneSingola();
                    trasmSing = trasmEff.trasmissioniSingole[i];
                    trasmSing.ragione.eredita = "0";
                    appoTrasmSingole[i] = trasmSing;
                }

                trasmEff.trasmissioniSingole = appoTrasmSingole;
            }
            return trasmEff;
        }

        private Registro[] GetRF()
        {
            string idRuolo = UserManager.GetSelectedRole().systemId;
            string all = "1";
            string idAooColl = string.Empty;

            return RegistryManager.GetListRegistriesAndRF(idRuolo, all, idAooColl);
        }

        private void LoadRF()
        {
            try
            {
                Registro[] rf = GetRF();

                for (int i = 0; i < rf.Length; i++)
                {
                    ListItem li = new ListItem(rf[i].codRegistro, rf[i].systemId);
                    this.ddlRF.Items.Add(li);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private void LoadReasons()
        {
            try
            {
                Fascicolo prj = ProjectManager.getProjectInSession();
                RagioneTrasmissione[] listaRagioni = TrasmManager.getListaRagioni(this, prj.accessRights, false);

                if (listaRagioni != null && listaRagioni.Length > 0)
                {
                    for (int i = 0; i < listaRagioni.Length; i++)
                    {
                        ListItem newItem = new ListItem(listaRagioni[i].descrizione, listaRagioni[i].systemId);
                        this.ddlReason.Items.Add(newItem);
                    }
                }

                this.ddlReason.Items.Insert(0, new ListItem());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        //private ArrayList GetTransmissionModels()
        //{
        //    try
        //    {
        //        string idTypeDoc = string.Empty;
        //        string idDiagram = string.Empty;
        //        string idState = string.Empty;
        //        string idFasc = string.Empty;
        //        string accessRights = string.Empty;
        //        Fascicolo prj = UIManager.ProjectManager.getProjectInSession();
        //        Registro reg = UIManager.RegistryManager.getRegistroBySistemId(prj.idRegistroNodoTit);
        //        accessRights = prj.accessRights;
        //        Registro[] listReg = new Registro[1];

        //        listReg[0] = reg;
        //        this.Registry = reg;

        //        return new ArrayList(UIManager.TransmissionModelsManager.GetTransmissionModelsLite(this.UserLog.idAmministrazione, listReg, this.UserLog.idPeople, this.InfoUser.idCorrGlobali, idTypeDoc, idDiagram, idState, "F", idFasc, this.Role.idGruppo, false, accessRights));
        //    }
        //    catch (System.Exception ex)
        //    {
        //        UIManager.AdministrationManager.DiagnosticError(ex);
        //        return null;
        //    }
        //}

        private void LoadTransmissionModels()
        {

            string idDiagram = string.Empty;
            string idState = string.Empty;
            string idFasc = string.Empty;
            string accessRights = string.Empty;
            string RegTit = string.Empty;
            string idTipoFasc = string.Empty;
            //bool AllReg = false;

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID))
            {
                accessRights = ProjectManager.getProjectInSession().accessRights;
                idFasc = ProjectManager.getProjectInSession().systemID;
                //RegTit = UIManager.RegistryManager.getRegTitolarioById(ProjectManager.getProjectInSession().idClassificazione);

                //if (!string.IsNullOrEmpty(RegTit))
                //{
                //    AllReg = true;
                //}
            }



            if (this.CustomDocuments && ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().template != null && ProjectManager.getProjectInSession().template.SYSTEM_ID != 0)
            {
                if (this.EnableStateDiagram)
                {
                    DiagrammaStato dia = DiagrammiManager.getDgByIdTipoFasc(ProjectManager.getProjectInSession().template.SYSTEM_ID.ToString(), this.InfoUser.idAmministrazione);
                    if (dia != null)
                    {
                        idDiagram = dia.SYSTEM_ID.ToString();
                        idTipoFasc = ProjectManager.getProjectInSession().template.SYSTEM_ID.ToString();
                        DocsPaWR.Stato stato = DiagrammiManager.getStatoFasc(ProjectManager.getProjectInSession().systemID);
                        if (stato != null)
                            idState = stato.SYSTEM_ID.ToString();
                    }
                }
            }

            Registro[] listReg = null;
            if (this.Registry != null)
            {
                listReg = new Registro[1];
                listReg[0] = this.Registry;
            }

            if (listReg == null)
            {
                listReg = this.Role.registri;
            }

            ArrayList idModelli = new ArrayList(UIManager.TransmissionModelsManager.GetTransmissionModelsLiteFasc(this.UserLog.idAmministrazione, listReg, this.UserLog.idPeople, this.InfoUser.idCorrGlobali, idTipoFasc, idDiagram, idState, "F", idFasc, this.Role.idGruppo, true, accessRights));

            ModelloTrasmissione[] modTrasm = idModelli.Cast<ModelloTrasmissione>().ToArray();
            modTrasm = (from mod in modTrasm orderby mod.NOME ascending select mod).ToArray<ModelloTrasmissione>();
            idModelli = ArrayList.Adapter(modTrasm.ToList());

            System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem("", "");
            this.DdlTransmissionsModel.Items.Add(li);

            for (int i = 0; i < idModelli.Count; i++)
            {
                DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)idModelli[i];
                li = new System.Web.UI.WebControls.ListItem();
                li.Value = mod.SYSTEM_ID.ToString();
                li.Text = mod.NOME;
                if (this.ViewCodeTransmissionModels)
                {
                    li.Text += " (" + mod.CODICE + ")";
                }
                this.DdlTransmissionsModel.Items.Add(li);
            }

        }

        private bool notificheUtImpostate(DocsPaWR.ModelloTrasmissione modello)
        {
            bool retValue = true;
            bool flag = false;

            foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
            {
                foreach (DocsPaWR.MittDest mittDest in ragDest.DESTINATARI)
                {
                    if (!mittDest.CHA_TIPO_URP.Equals("U"))
                    {
                        // ritorna FALSE se anche un solo destinatario del modello non ha UTENTI_NOTIFICA
                        if (mittDest.UTENTI_NOTIFICA == null)
                            return false;

                        if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length > 0)
                        {
                            flag = false;

                            foreach (DocsPaWR.UtentiConNotificaTrasm utNot in mittDest.UTENTI_NOTIFICA)
                            {
                                if (utNot.FLAG_NOTIFICA.Equals("1"))
                                    flag = true;
                            }

                            // ritorna FALSE se anche un solo destinatario ha tutti gli utenti con le notifiche non impostate
                            if (!flag)
                                return false;
                        }
                    }
                }
            }

            return retValue;
        }

        private DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPaWR.Trasmissione trasmissione, DocsPaWR.Corrispondente corr, DocsPaWR.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, bool nascondiVersioniPrecedenti)
        {
            try
            {
                if (trasmissione.trasmissioniSingole != null)
                {
                    // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                    for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                    {
                        DocsPaWR.TrasmissioneSingola ts = (DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                        if (ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                        {
                            if (ts.daEliminare)
                            {
                                ((DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                                return trasmissione;
                            }
                            else
                                return trasmissione;
                        }
                    }
                }

                // Aggiungo la trasmissione singola
                DocsPaWR.TrasmissioneSingola trasmissioneSingola = new DocsPaWR.TrasmissioneSingola();
                trasmissioneSingola.tipoTrasm = tipoTrasm;
                trasmissioneSingola.corrispondenteInterno = corr;
                trasmissioneSingola.ragione = ragione;
                trasmissioneSingola.noteSingole = note;

                if (trasmissione.tipoOggetto == TrasmissioneTipoOggetto.FASCICOLO)
                    trasmissioneSingola.hideDocumentPreviousVersions = nascondiVersioniPrecedenti;
                else
                    trasmissioneSingola.hideDocumentPreviousVersions = false;

                // Imposto la data di scadenza
                if (scadenza > 0)
                {
                    System.DateTime data = System.DateTime.Now.AddDays(scadenza);
                    trasmissioneSingola.dataScadenza = Utils.dateformat.formatDataDocsPa(data);
                }

                // Aggiungo la lista di trasmissioniUtente
                if (corr is DocsPaWR.Ruolo)
                {
                    trasmissioneSingola.tipoDest = DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
                    DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr);
                    /*
                     * Andrea
                     */
                    if (listaUtenti == null || listaUtenti.Length == 0)
                    {
                        trasmissioneSingola = null;
                        throw new ExceptionTrasmissioni("Non è presente alcun utente per la Trasmissione al ruolo: "
                                                        + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                        + ".");
                    }
                    // End Andrea
                    else
                    {
                        //ciclo per utenti se dest è gruppo o ruolo
                        for (int i = 0; i < listaUtenti.Length; i++)
                        {
                            DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPaWR.TrasmissioneUtente();
                            trasmissioneUtente.utente = (DocsPaWR.Utente)listaUtenti[i];
                            trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                        }
                    }
                }

                if (corr is DocsPaWR.Utente)
                {
                    trasmissioneSingola.tipoDest = DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
                    DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPaWR.TrasmissioneUtente();
                    trasmissioneUtente.utente = (DocsPaWR.Utente)corr;
                    /*
                     * Andrea
                     */
                    if (trasmissioneUtente.utente == null)
                    {
                        throw new ExceptionTrasmissioni("L utente: " + corr.codiceCorrispondente + " (" + corr.descrizione + ") è inesistente.");
                    }
                    //End Andrea
                    else
                        trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                }

                if (corr is DocsPaWR.UnitaOrganizzativa)
                {
                    DocsPaWR.UnitaOrganizzativa theUo = (DocsPaWR.UnitaOrganizzativa)corr;
                    DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = new AddressbookQueryCorrispondenteAutorizzato();
                    qca.ragione = trasmissioneSingola.ragione;
                    qca.ruolo = UserManager.GetSelectedRole();
                    qca.queryCorrispondente = new AddressbookQueryCorrispondente();
                    qca.queryCorrispondente.fineValidita = true;

                    DocsPaWR.Ruolo[] ruoli = UserManager.getRuoliRiferimentoAutorizzati(this, qca, theUo);
                    /*
                     * Andrea
                     */
                    if (ruoli == null || ruoli.Length == 0)
                    {
                        throw new ExceptionTrasmissioni("Manca un ruolo di riferimento per la UO: "
                                                        + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                        + ".");
                    }
                    //End Andrea
                    else
                    {
                        foreach (DocsPaWR.Ruolo r in ruoli)
                            trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm, scadenza, nascondiVersioniPrecedenti);
                    }
                    return trasmissione;
                }

                if (trasmissioneSingola != null)
                    trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);

                return trasmissione;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        private DocsPaWR.Corrispondente[] queryUtenti(DocsPaWR.Corrispondente corr)
        {

            //costruzione oggetto queryCorrispondente
            DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPaWR.AddressbookQueryCorrispondente();

            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;

            qco.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
            qco.fineValidita = true;

            //corrispondenti interni
            qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;

            return UserManager.getListaCorrispondenti(this, qco);
        }

        private void ReApplyScripts()
        {
            this.ReApplyChosenScript();
            this.ReApplyDatePickerScript();
            this.ReApplyTipsy();

        }

        private void ReApplyTipsy()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        private void ReApplyChosenScript()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen_deselect", "$('.chzn-select-deselect').chosen({ allow_single_deselect: true, no_results_text: 'Nessun risultato trovato' });", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen", "$('.chzn-select').chosen({ no_results_text: 'Nessun risultato trovato' });", true);
        }

        private void ReApplyDatePickerScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        protected void TransmissionsBtnAccept_Click(object sender, EventArgs e)
        {
            this.acceptTransmission();

            //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
            if (ProjectManager.CheckRevocationAcl())
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()');}", true);
                return;
            }
        }

        private void acceptTransmission()
        {
            try
            {
                logger.Info("BEGIN");
                DocsPaWR.Trasmissione trasmissione = TrasmManager.getSelectedTransmission();

                Fascicolo prj = ProjectManager.getProjectInSession();

                //Effettuo l'accettazione della trasmissione
                bool result = accettaRifiuta(DocsPaWR.TrasmissioneTipoRisposta.ACCETTAZIONE);

                if (result)
                {
                    prj = UIManager.ProjectManager.getFascicoloById(prj.systemID);
                    prj.template = ProfilerProjectManager.getTemplateFascDettagli(prj.systemID);
                    ProjectManager.setProjectInSession(prj);
                }

                //Verifico l'abilitazione dei diagrammi di stato
                if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                {
                    // DocsPaWR.Trasmissione trasmissione = (NttDataWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this);
                    if (trasmissione.tipoOggetto.ToString() == "FASCICOLO")
                    {
                        InfoFascicolo infoFascicolo = trasmissione.infoFascicolo;

                        //E' importante che l'accettazione della trasmiossione corrente sia fatta prima di questo tipo di verifica
                        if (DiagrammiManager.isUltimaDaAccettare(trasmissione.systemId, this.Page))
                        {

                            DocsPaWR.Stato statoSucc = DiagrammiManager.getStatoSuccessivoAutomaticoFasc(infoFascicolo.idFascicolo);
                            DocsPaWR.Stato statoCorr = DiagrammiManager.getStatoFasc(infoFascicolo.idFascicolo);
                            string idTemplate = string.Empty;
                            if (prj.template != null)
                            {
                                idTemplate = prj.template.SYSTEM_ID.ToString();
                            }
                            //Se il fascicolo è di una tipologia sospesa non viene fatta nessuna considerazione su un eventuale passaggio di stato automatico
                            if (!string.IsNullOrEmpty(idTemplate))
                            {
                                Templates tipoFascicolo = ProfilerDocManager.getTemplate(infoFascicolo.idFascicolo);
                                if (tipoFascicolo != null && tipoFascicolo.IN_ESERCIZIO.ToUpper().Equals("NO"))
                                {
                                    string msg = "ErrorTransmissionTypeSuspended";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                                    return;
                                }
                            }

                            if (statoSucc != null)
                            {
                                if (statoSucc.STATO_FINALE)
                                {
                                    string msg2 = "ConfirmTransmissionFinalStateFasc";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msg2.Replace("'", @"\'") + "', 'final_state', '');} else {parent.ajaxConfirmModal('" + msg2.Replace("'", @"\'") + "', 'final_state', '');}", true);
                                    return;
                                }
                                    //Cambio stato
                                DocsPaWR.DiagrammaStato dg = DiagrammiManager.getDiagrammaById(Convert.ToString(statoSucc.ID_DIAGRAMMA));
                                DiagrammiManager.salvaModificaStatoFasc(prj.systemID, Convert.ToString(statoSucc.SYSTEM_ID), dg, UserManager.GetInfoUser().userId, UserManager.GetInfoUser(), string.Empty);

                                //Cancellazione storico trasmissioni
                                DiagrammiManager.deleteStoricoTrasmDiagrammi(prj.systemID, Convert.ToString(statoCorr.SYSTEM_ID));

                                //Verifico se il nuovo stato ha delle trasmissioni automatiche
                                DocsPaWR.Stato stato = ProjectManager.getStatoFasc(prj);
                                string idTipoFasc = string.Empty;
                                if (prj.template != null)
                                {
                                    idTipoFasc = prj.template.SYSTEM_ID.ToString();
                                }
                                if (idTipoFasc != "")
                                {
                                    ArrayList modelli = new ArrayList(DiagrammiManager.isStatoTrasmAutoFasc(UserManager.GetInfoUser().idAmministrazione, Convert.ToString(stato.SYSTEM_ID), idTipoFasc));
                                    for (int i = 0; i < modelli.Count; i++)
                                    {
                                        DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];

                                        if (mod.SINGLE == "1")
                                        {
                                            //DocsPAWA.TrasmManager.effettuaTrasmissioneFascDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, Fasc, this);
                                            TrasmManager.effettuaTrasmissioneFascDaModello(mod, Convert.ToString(stato.SYSTEM_ID), prj, this);
                                            Response.Redirect("../Project/TransmissionsP.aspx", false);
                                            //TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), trasmissione.infoDocumento, this);
                                        }
                                        else
                                        {
                                            for (int k = 0; k < mod.MITTENTE.Length; k++)
                                            {
                                                if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.GetSelectedRole().systemId)
                                                {
                                                    //TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), trasmissione.infoDocumento, this);
                                                    TrasmManager.effettuaTrasmissioneFascDaModello(mod, Convert.ToString(stato.SYSTEM_ID), prj, this);
                                                    Response.Redirect("../Project/TransmissionsP.aspx", false);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                this.EnableButtons();
                this.showTransmission();

                logger.Info("END");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

        }

        private void ChangeState()
        {
            Fascicolo prj = ProjectManager.getProjectInSession();
            DocsPaWR.Trasmissione trasmissione = TrasmManager.getSelectedTransmission();
            // da verifiarlo solo per i documenti
            DocsPaWR.InfoFascicolo infoFascicolo = trasmissione.infoFascicolo;
            DocsPaWR.Stato statoSucc = DiagrammiManager.getStatoSuccessivoAutomaticoFasc(infoFascicolo.idFascicolo);
            DocsPaWR.DiagrammaStato dg = DiagrammiManager.getDiagrammaById(Convert.ToString(statoSucc.ID_DIAGRAMMA));
            DiagrammiManager.salvaModificaStatoFasc(prj.systemID, Convert.ToString(statoSucc.SYSTEM_ID), dg, UserManager.GetInfoUser().userId, UserManager.GetInfoUser(), string.Empty);
            this.chiudiFascicolo(prj, UIManager.UserManager.GetInfoUser(), UIManager.RoleManager.GetRoleInSession());

            //Verifico se il nuovo stato ha delle trasmissioni automatiche
            DocsPaWR.Stato stato = ProjectManager.getStatoFasc(prj);
            string idTipoFasc = string.Empty;
            if (prj.template != null)
            {
                idTipoFasc = prj.template.SYSTEM_ID.ToString();
            }
            if (idTipoFasc != "")
            {
                ArrayList modelli = new ArrayList(DiagrammiManager.isStatoTrasmAutoFasc(UserManager.GetInfoUser().idAmministrazione, Convert.ToString(stato.SYSTEM_ID), idTipoFasc));
                for (int i = 0; i < modelli.Count; i++)
                {
                    DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];

                    if (mod.SINGLE == "1")
                    {
                        //DocsPAWA.TrasmManager.effettuaTrasmissioneFascDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, Fasc, this);
                        TrasmManager.effettuaTrasmissioneFascDaModello(mod, Convert.ToString(stato.SYSTEM_ID), prj, this);
                        //TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), trasmissione.infoDocumento, this);
                    }
                    else
                    {
                        for (int k = 0; k < mod.MITTENTE.Length; k++)
                        {
                            if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.GetSelectedRole().systemId)
                            {
                                //TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), trasmissione.infoDocumento, this);
                                TrasmManager.effettuaTrasmissioneFascDaModello(mod, Convert.ToString(stato.SYSTEM_ID), prj, this);
                                Response.Redirect("../Project/TransmissionsP.aspx", false);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// consente la chiusura di un fascicolo
        /// </summary>
        private void chiudiFascicolo(Fascicolo fascicolo, InfoUtente infoutente, Ruolo ruolo)
        {
            string msg = string.Empty;
            fascicolo.chiusura = DateTime.Now.ToShortDateString();
            fascicolo.stato = "C";
            if (fascicolo.chiudeFascicolo == null)
            {
                fascicolo.chiudeFascicolo = new DocsPaWR.ChiudeFascicolo();
            }
            fascicolo.chiudeFascicolo.idPeople = infoutente.idPeople;
            fascicolo.chiudeFascicolo.idCorrGlob_Ruolo = ruolo.systemId;
            fascicolo.chiudeFascicolo.idCorrGlob_UO = ruolo.uo.systemId;
            fascicolo = UIManager.ProjectManager.setFascicolo(fascicolo, infoutente);
            if (fascicolo != null)
            {
                (this.HeaderProject.FindControl("projectLblStatoGenerato") as Label).Text = Utils.Languages.GetLabelFromCode("prjectStatoRegistroChiuso", UserManager.GetUserLanguage());
                (this.HeaderProject.FindControl("projectLblStatoGenerato") as Label).CssClass = "close";
                UIManager.ProjectManager.setProjectInSession(fascicolo);
                this.UpUserControlHeaderProject.Update();
            }
            else
            {
                msg = "ErroreProjectChiusuraFascicolo";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error');}", true);
            }
        }

        private bool accettaRifiuta(DocsPaWR.TrasmissioneTipoRisposta tipoRisp)
        {

            logger.Info("BEGIN");
            bool rtn = true;
            string errore = string.Empty;
            DocsPaWR.Trasmissione trasmissione = TrasmManager.getSelectedTransmission();
            DocsPaWR.TrasmissioneSingola trasmSing = null;
            DocsPaWR.TrasmissioneUtente trasmUtente = null;

            if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
            {
                if (string.IsNullOrEmpty(this.txt_noteAccRif.Text.Trim()))
                {
                    string msg = "ErrorTransmissionNoteReject";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', 'Accettazione');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', 'Accettazione');}", true);

                    return false;
                }
            }

            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
            System.Collections.Generic.List<DocsPaWR.TrasmissioneSingola> list = new System.Collections.Generic.List<TrasmissioneSingola>(trasmissione.trasmissioniSingole);
            List<DocsPaWR.TrasmissioneSingola> trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.UTENTE).ToList();
            DocsPaWR.Utente utenteCorrente = (DocsPaWR.Utente)UserManager.getCorrispondenteByIdPeople(this, infoUtente.idPeople, AddressbookTipoUtente.INTERNO);
            trasmSing = trasmSingoleUtente.Where(e => ((DocsPaWR.Utente)e.corrispondenteInterno).idPeople == infoUtente.idPeople).FirstOrDefault();

            if (trasmSing == null)
            {
                // Se non è stata trovata la trasmissione come destinatario ad utente, 
                // cerca quella con destinatario ruolo corrente dell'utente
                trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.RUOLO).ToList();
                trasmSing = TrasmManager.RoleTransmissionWithHistoricized(trasmSingoleUtente, infoUtente.idCorrGlobali);
            }

            if (trasmSing != null)
            {

                //MODIFICA PRE PROBLEMA ACCETTAZIONE TRASMISSIONE INVIATA SIA A RUOLO CHE UTENTE
                DocsPaWR.TrasmissioneSingola[] listaTrasmSing;
                //DocsPaWR.TrasmissioneUtente[] listaTrasmUtente;

                listaTrasmSing = trasmissione.trasmissioniSingole;

                //foreach (TrasmissioneSingola sing in trasmissione.trasmissioniSingole)
                //{

                trasmUtente = trasmSing.trasmissioneUtente.Where(e => ((DocsPaWR.Utente)e.utente).idPeople == infoUtente.idPeople).FirstOrDefault();

                //note acc/rif
                if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
                    trasmUtente.noteRifiuto = this.txt_noteAccRif.Text;
                else
                    trasmUtente.noteAccettazione = this.txt_noteAccRif.Text;

                //data Accettazione /Rifiuto
                if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
                    trasmUtente.dataRifiutata = dateformat.getDataOdiernaDocspa(); //getDataCorrente();
                else
                    trasmUtente.dataAccettata = dateformat.getDataOdiernaDocspa(); //getDataCorrente();

                //tipoRisposta
                trasmUtente.tipoRisposta = tipoRisp;

                if (!TrasmManager.executeAccRif(this, trasmUtente, trasmissione.systemId, null, out errore))
                {
                    rtn = false;
                    trasmUtente.dataRifiutata = null;
                    trasmUtente.dataAccettata = null;

                    string msg = "ErrorCustom";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '', '" + errore.Replace("'", @"\'") + "');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '', '" + errore.Replace("'", @"\'") + "');}", true);
                }
                else if (infoUtente.delegato != null)
                {
                    if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
                    {
                        trasmUtente.cha_rifiutata_delegato = "1";
                    }
                    else
                    {
                        trasmUtente.cha_accettata_delegato = "1";
                    }
                }
                //}

                ////NEL CASO DI TRASMISSIONE A RUOLO ACCETTO TUTTE LE TRASMISSIONE AD UTENTE
                //if (trasmSing.tipoDest == TrasmissioneTipoDestinatario.RUOLO && listaTrasmSing.Length > 1)
                //{
                //    DocsPaWR.TrasmissioneSingola trasmSingTemp;
                //    for (int i = 1; i < listaTrasmSing.Length; i++)
                //    {
                //        trasmSingTemp = (DocsPaWR.TrasmissioneSingola)listaTrasmSing[i];
                //        listaTrasmUtente = trasmSingTemp.trasmissioneUtente;
                //        if (listaTrasmUtente.Length > 0 && trasmSingTemp.tipoDest == TrasmissioneTipoDestinatario.UTENTE)
                //        {
                //            trasmUtente = trasmSingTemp.trasmissioneUtente.Where(e => ((DocsPaWR.Utente)e.utente).idPeople == infoUtente.idPeople).FirstOrDefault();

                //            //note acc/rif
                //            if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
                //                trasmUtente.noteRifiuto = this.txt_noteAccRif.Text;
                //            else
                //                trasmUtente.noteAccettazione = this.txt_noteAccRif.Text;

                //            //data Accettazione /Rifiuto
                //            if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
                //                trasmUtente.dataRifiutata = dateformat.getDataOdiernaDocspa(); //getDataCorrente();
                //            else
                //                trasmUtente.dataAccettata = dateformat.getDataOdiernaDocspa(); //getDataCorrente();

                //            //tipoRisposta
                //            trasmUtente.tipoRisposta = tipoRisp;

                //            TrasmManager.executeAccRif(this, trasmUtente, trasmissione.systemId, out errore);
                //        }
                //    }
                //}
            }

            logger.Info("END");
            return rtn;


        }


        protected void TransmissionsBtnAcceptADL_Click(object sender, System.EventArgs e)
        {
            this.acceptTransmission();

            Fascicolo prj = ProjectManager.getProjectInSession();
            ProjectManager.addFascicoloInAreaDiLavoro(prj, UserManager.GetInfoUser());


            //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
            if (ProjectManager.CheckRevocationAcl())
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()');}", true);
                return;
            }
        }

        protected void TransmissionsBtnAcceptADLR_Click(object sender, System.EventArgs e)
        {
            this.acceptTransmission();

            Fascicolo prj = ProjectManager.getProjectInSession();
            ProjectManager.addFascicoloInAreaDiLavoroRole(prj, UserManager.GetInfoUser());


            //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
            if (ProjectManager.CheckRevocationAcl())
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()');}", true);
                return;
            }
        }

        protected void TransmissionsBtnReject_Click(object sender, System.EventArgs e)
        {
            logger.Info("BEGIN");

            bool result = accettaRifiuta(DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO);

            if (result)
            {

                this.EnableButtons();
                this.showTransmission();

                Fascicolo prj = ProjectManager.getProjectInSession();
                Fascicolo newPrj = ProjectManager.getFascicoloById(prj.systemID);
                if (newPrj == null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()');}", true);
                }
            }

            logger.Info("END");
        }

        protected void TransmissionsBtnView_Click(object sender, System.EventArgs e)
        {
            InfoFascicolo infofascicolo = TrasmManager.getSelectedTransmission().infoFascicolo;
            TrasmManager.setdatavistaSP_TV(UserManager.GetInfoUser(), infofascicolo.idFascicolo, "F", infofascicolo.idRegistro, TrasmManager.getSelectedTransmission().systemId);


            //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
            if (ProjectManager.CheckRevocationAcl())
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()');}", true);
                return;
            }

            this.TransmissionsBtnView.Visible = false;
            this.TransmissionsBtnViewADL.Visible = false;
            this.TransmissionsBtnViewADLR.Visible = false;

            this.EnableButtons();
            this.showTransmission();
        }

        protected void TransmissionsBtnViewADL_Click(object sender, System.EventArgs e)
        {
            InfoFascicolo infofascicolo = TrasmManager.getSelectedTransmission().infoFascicolo;
            Fascicolo prj = ProjectManager.getProjectInSession();
            InfoUtente infoutente = UIManager.UserManager.GetInfoUser();


            TrasmManager.setdatavistaSP_TV(infoutente, infofascicolo.idFascicolo, "F", infofascicolo.idRegistro, TrasmManager.getSelectedTransmission().systemId);



            ProjectManager.addFascicoloInAreaDiLavoro(prj, infoutente);


            //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
            if (ProjectManager.CheckRevocationAcl())
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()');}", true);
                return;
            }

            this.TransmissionsBtnView.Visible = false;
            this.TransmissionsBtnViewADL.Visible = false;
            this.TransmissionsBtnViewADLR.Visible = false;

            this.EnableButtons();
            this.showTransmission();
        }

        protected void TransmissionsBtnViewADLR_Click(object sender, System.EventArgs e)
        {
            InfoFascicolo infofascicolo = TrasmManager.getSelectedTransmission().infoFascicolo;
            Fascicolo prj = ProjectManager.getProjectInSession();
            InfoUtente infoutente = UIManager.UserManager.GetInfoUser();


            TrasmManager.setdatavistaSP_TV(infoutente, infofascicolo.idFascicolo, "F", infofascicolo.idRegistro, TrasmManager.getSelectedTransmission().systemId);



            ProjectManager.addFascicoloInAreaDiLavoroRole(prj, infoutente);


            //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
            if (ProjectManager.CheckRevocationAcl())
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAclProject', 'warning', '','',null,null,'RedirectHome()');}", true);
                return;
            }

            this.TransmissionsBtnView.Visible = false;
            this.TransmissionsBtnViewADL.Visible = false;
            this.TransmissionsBtnViewADLR.Visible = false;

            this.EnableButtons();
            this.showTransmission();
        }

        private void EnableButtonsTrasmReceived(Trasmissione trasm)
        {
            bool isInToDoList = false;
            bool isToUserOrRole = true;
            DocsPaWR.TrasmissioneSingola trasmSing = null;
            DocsPaWR.TrasmissioneUtente trasmUtente = null;

            this.TransmissionsBtnAccept.Visible = false;
            this.TransmissionsBtnAcceptADL.Visible = false;
            this.TransmissionsBtnAcceptADLR.Visible = false;
            this.TransmissionsBtnReject.Visible = false;

            this.imgSepFooter.Visible = false;

            this.TransmissionsBtnView.Visible = false;
            this.TransmissionsBtnViewADL.Visible = false;
            this.TransmissionsBtnViewADLR.Visible = false;

            this.upPnlNoteAccRif.Visible = false;
            this.upPnlNoteAccRif.Update();

            listTrasmSing = trasm.trasmissioniSingole;
            if (listTrasmSing != null && listTrasmSing.Length > 0 && !string.IsNullOrEmpty(trasm.dataInvio))
            {
                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                //Vedo se presente la trasmissione all'utente
                List<DocsPaWR.TrasmissioneSingola> list = new List<TrasmissioneSingola>(trasm.trasmissioniSingole);
                List<DocsPaWR.TrasmissioneSingola> trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.UTENTE).ToList();
                DocsPaWR.Utente utenteCorrente = (DocsPaWR.Utente)UserManager.getCorrispondenteByIdPeople(this, infoUtente.idPeople, AddressbookTipoUtente.INTERNO);
                trasmSing = trasmSingoleUtente.Where(e => ((DocsPaWR.Utente)e.corrispondenteInterno).idPeople == infoUtente.idPeople).FirstOrDefault();

                if (trasmSing == null)
                {
                    // Se non è stata trovata la trasmissione come destinatario ad utente, 
                    // cerca quella con destinatario ruolo corrente dell'utente
                    trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.RUOLO).ToList();
                    trasmSing = TrasmManager.RoleTransmissionWithHistoricized(trasmSingoleUtente, infoUtente.idCorrGlobali);
                }

                if (trasmSing != null)
                {
                    trasmUtente = (DocsPaWR.TrasmissioneUtente)trasmSing.trasmissioneUtente.Where(e => ((DocsPaWR.Utente)e.utente).idPeople == infoUtente.idPeople).FirstOrDefault();
                    if (trasmUtente != null && trasmSing.ragione.tipo == "W" && string.IsNullOrEmpty(trasmUtente.dataRifiutata) && string.IsNullOrEmpty(trasmUtente.dataAccettata))
                    {
                        bool value = TrasmManager.checkTrasm_UNO_TUTTI_AccettataRifiutata(trasmSing);
                        value = value && isToUserOrRole;
                        this.imgSepFooter.Visible = value;
                        this.TransmissionsBtnAccept.Visible = value;
                        this.TransmissionsBtnAcceptADL.Visible = value;

                        if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
                        {
                            this.TransmissionsBtnAcceptADLR.Visible = value;
                        }

                        if (ProjectManager.isFascInADLRole(trasm.infoFascicolo.idFascicolo, this) == 1)
                            this.TransmissionsBtnAcceptADL.Visible = false;

                        
                        this.TransmissionsBtnReject.Visible = value;

                        this.upPnlNoteAccRif.Visible = value;
                        this.upPnlNoteAccRif.Update();
                    }
                    else
                    {
                        if (trasmSing.ragione.tipo != "W")
                        {
                            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[WebConfigKeys.SET_DATA_VISTA_GRD.ToString()])
                                && ConfigurationManager.AppSettings[WebConfigKeys.SET_DATA_VISTA_GRD.ToString()] == "2"
                                )
                            {
                                this.imgSepFooter.Visible = true;
                                this.TransmissionsBtnView.Visible = true;
                                this.TransmissionsBtnViewADL.Visible = true;
                                this.TransmissionsBtnViewADLR.Visible = true;
                            }

                            // PALUMBO: inserita variabile isInToDoList per eliminare visualizzazione tasto Visto in caso 
                            // di trasmissione per IS e già vista incident: INC000000103503  
                            if (trasmUtente != null && TrasmManager.getIfDocOrFascIsInToDoList(infoUtente, trasmUtente.systemId) && isToUserOrRole && !string.IsNullOrEmpty(ConfigurationManager.AppSettings[WebConfigKeys.SET_DATA_VISTA_GRD.ToString()])
                               && ConfigurationManager.AppSettings[WebConfigKeys.SET_DATA_VISTA_GRD.ToString()] == "2")
                            {
                                isInToDoList = true;
                                this.imgSepFooter.Visible = true;
                                this.TransmissionsBtnView.Visible = true;
                                this.TransmissionsBtnViewADL.Visible = true;
                                this.TransmissionsBtnViewADLR.Visible = true;
                            }
                            else
                            {
                                this.imgSepFooter.Visible = false;
                                this.TransmissionsBtnView.Visible = false;
                                this.TransmissionsBtnViewADL.Visible = false;
                                this.TransmissionsBtnViewADLR.Visible = false;
                            }

                        }
                        else
                        {
                            // hide note acc/rif
                            this.upPnlNoteAccRif.Visible = false;
                            this.upPnlNoteAccRif.Update();
                        }
                    }
                }
                else
                {
                    // hide note acc/rif
                    this.upPnlNoteAccRif.Visible = false;
                    this.upPnlNoteAccRif.Update();
                }
            }
        }

        protected virtual void EnableButtons()
        {
            this.DisableAllButtons();

            Fascicolo fascicolo = ProjectManager.getProjectInSession();
            Trasmissione trasm = TrasmManager.getSelectedTransmission();

            if (fascicolo != null && fascicolo.systemID != null)
            {
                this.TransmissionsBtnAdd.Enabled = true; // If the fascicolo is not in the basket insertion is always allowed

                if (this.grdList.Rows.Count > 0)
                {
                    this.TransmissionsBtnPrint.Visible = true; // print is enabled always if there are transmissions

                    // checks on transm selected
                    if (this.SelectedRowIndex >= 0)
                    {
                        if ((trasm != null && trasm.systemId != null) && (trasm.dataInvio == null || trasm.dataInvio == "") && trasm.ruolo.idGruppo.Equals(RoleManager.GetRoleInSession().idGruppo))
                        {
                            this.TransmissionsBtnModify.Enabled = true; // modify is enabled only if not yet transmitted
                            this.TransmissionsBtnTransmit.Enabled = true; // transmission is enabled only if not yet transmitted
                        }

                        // check visibility for buttons accept, reject and view
                        this.EnableButtonsTrasmReceived(trasm);
                    }
                }

                // checks on templates selected
                if (this.DdlTransmissionsModel.SelectedValue.Trim() != "")
                {
                    this.TransmissionsBtnTransmit.Enabled = true; // transmission is enabled if template selected
                }
                //disabilitazione dei bottoni in base all'autorizzazione di HM sul fascicolo
                if (!UserManager.EnableNewTransmissionProject())
                {
                    // Bottoni che devono essere disabilitati in caso di diritti di sola lettura
                    this.DisableAllButtons(false);
                }
            }

            this.panelButtons.Update();
        }

        private void DisableAllButtons()
        {
            bool disablePrintToo = true;
            this.DisableAllButtons(disablePrintToo);
        }

        /// <summary>
        /// Disable all buttons
        /// </summary>
        private void DisableAllButtons(bool disablePrintToo)
        {
            this.TransmissionsBtnAdd.Enabled = false;
            this.TransmissionsBtnModify.Enabled = false;
            this.TransmissionsBtnTransmit.Enabled = false;

            if (disablePrintToo)
            {
                this.TransmissionsBtnPrint.Visible = false;
                this.imgSepFooter.Visible = false;
                this.TransmissionsBtnAccept.Visible = false;
                this.TransmissionsBtnAcceptADL.Visible = false;
                this.TransmissionsBtnAcceptADLR.Visible = false;
                this.TransmissionsBtnReject.Visible = false;
                this.TransmissionsBtnView.Visible = false;
                this.TransmissionsBtnViewADL.Visible = false;
                this.TransmissionsBtnViewADLR.Visible = false;
            }
        }

        protected void TransmissionsBtnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Transmission/dataentry_project.aspx?t=" + Request.QueryString["t"] + "&idProfile=" + Request.QueryString["idProfile"]);
        }

        protected void TransmissionsBtnModify_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Transmission/dataentry_project.aspx?t=" + Request.QueryString["t"] + "&idProfile=" + Request.QueryString["idProfile"] + "&idTransmission=" + TrasmManager.getSelectedTransmission().systemId);
        }

        private DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPaWR.Trasmissione trasmissione, DocsPaWR.Corrispondente corr, DocsPaWR.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, DocsPaWR.MittDest mittDest)
        {
            try
            {
                if (trasmissione.trasmissioniSingole != null)
                {
                    // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                    for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                    {
                        DocsPaWR.TrasmissioneSingola ts = (DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];

                        if (corr != null)
                            if (ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                            {
                                if (ts.daEliminare)
                                {
                                    ((DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                                    return trasmissione;
                                }
                                else
                                    return trasmissione;
                            }
                    }
                }

                // Aggiungo la trasmissione singola
                DocsPaWR.TrasmissioneSingola trasmissioneSingola = new DocsPaWR.TrasmissioneSingola();
                trasmissioneSingola.tipoTrasm = tipoTrasm;
                trasmissioneSingola.corrispondenteInterno = corr;
                trasmissioneSingola.ragione = ragione;
                trasmissioneSingola.noteSingole = note;

                //Imposto la data di scadenza
                if (scadenza > 0)
                {
                    //string dataScadenza = "";
                    System.DateTime data = System.DateTime.Now.AddDays(scadenza);
                    trasmissioneSingola.dataScadenza = dateformat.formatDataDocsPa(data);
                }

                // Aggiungo la lista di trasmissioniUtente
                if (corr is DocsPaWR.Ruolo)
                {
                    trasmissioneSingola.tipoDest = DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
                    DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr);

                    //Andrea
                    if (listaUtenti.Length == 0)
                    {
                        trasmissioneSingola = null;
                        throw new ExceptionTrasmissioni("Non è presente alcun utente per la Trasmissione al ruolo: "
                                                        + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                        + ".");
                    }
                    else
                    {
                        //ciclo per utenti se dest è gruppo o ruolo
                        for (int i = 0; i < listaUtenti.Length; i++)
                        {
                            DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPaWR.TrasmissioneUtente();
                            trasmissioneUtente.utente = (DocsPaWR.Utente)listaUtenti[i];
                            trasmissioneUtente.daNotificare = this.selezionaNotificaDaModello(mittDest, trasmissioneUtente.utente); //TrasmManager.getTxRuoloUtentiChecked();
                            trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                        }
                    }
                }

                if (corr is DocsPaWR.Utente)
                {
                    trasmissioneSingola.tipoDest = DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
                    DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPaWR.TrasmissioneUtente();
                    trasmissioneUtente.utente = (DocsPaWR.Utente)corr;
                    trasmissioneUtente.daNotificare = this.selezionaNotificaDaModello(mittDest, trasmissioneUtente.utente);

                    //Andrea
                    if (trasmissioneUtente.utente == null)
                    {
                        throw new ExceptionTrasmissioni("L utente: " + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                        + " è inesistente.");
                    }
                    //End Andrea
                    else
                        trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                }

                if (corr is DocsPaWR.UnitaOrganizzativa)
                {
                    DocsPaWR.UnitaOrganizzativa theUo = (DocsPaWR.UnitaOrganizzativa)corr;
                    DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = new DocsPaWR.AddressbookQueryCorrispondenteAutorizzato();
                    qca.ragione = trasmissioneSingola.ragione;
                    qca.ruolo = UserManager.GetSelectedRole();

                    //DocsPaWR.Ruolo[] ruoli = UserManager.getListaRuoliInUO(this, theUo, UserManager.getInfoUtente());
                    DocsPaWR.Ruolo[] ruoli = UserManager.getRuoliRiferimentoAutorizzati(this, qca, theUo);

                    //Andrea
                    if (ruoli.Length == 0)
                    {
                        throw new ExceptionTrasmissioni("Manca un ruolo di riferimento per la UO: "
                                                            + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                            + ".");
                    }
                    //End Andrea
                    else
                    {
                        foreach (DocsPaWR.Ruolo r in ruoli)
                            if (r != null && r.systemId != null)
                                trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm, scadenza, mittDest);
                    }
                    return trasmissione;
                }

                if (trasmissioneSingola != null)
                    trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);


                return trasmissione;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        private bool selezionaNotificaDaModello(DocsPaWR.MittDest mittDest, DocsPaWR.Utente utente)
        {
            bool retValue = false;

            try
            {
                if (!mittDest.CHA_TIPO_URP.Equals("U"))
                {
                    if (mittDest.UTENTI_NOTIFICA != null)
                    {
                        foreach (DocsPaWR.UtentiConNotificaTrasm utNot in mittDest.UTENTI_NOTIFICA)
                        {
                            if (utente.idPeople.Equals(utNot.ID_PEOPLE))
                                return Convert.ToBoolean(utNot.FLAG_NOTIFICA.Replace("1", "true").Replace("0", "false"));
                        }
                    }
                    else
                    {
                        retValue = TrasmManager.getTxRuoloUtentiChecked();
                    }
                }
                else
                {
                    retValue = TrasmManager.getTxRuoloUtentiChecked();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }

            return retValue;
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                CustomTextArea caller = sender as CustomTextArea;
                string codeAddressBook = this.TxtCodeRecipientTransmission.Text;

                if (!string.IsNullOrEmpty(codeAddressBook))
                {
                    this.SearchCorrespondent(codeAddressBook, caller.ID);
                }
                else
                {
                    this.TxtCodeRecipientTransmission.Text = string.Empty;
                    this.TxtDescriptionRecipient.Text = string.Empty;
                    this.IdRecipient.Value = string.Empty;
                    this.UpPnlRecipient.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }
        }

        protected RubricaCallType GetCallType(string idControl)
        {
            RubricaCallType calltype = DocsPaWR.RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
            return calltype;
        }

        protected void SearchCorrespondent(string addressCode, string idControl)
        {
            try
            {
                RubricaCallType calltype = this.GetCallType(idControl);
                DocsPaWR.Corrispondente corr = UIManager.AddressBookManager.getCorrispondenteRubrica(addressCode, calltype);
                if (corr == null)
                {
                    this.TxtCodeRecipientTransmission.Text = string.Empty;
                    this.TxtDescriptionRecipient.Text = string.Empty;
                    this.IdRecipient.Value = string.Empty;
                    this.RecipientTypeOfCorrespondent.Value = string.Empty;
                    this.RecipientTypeOfCorrespondent.Value = string.Empty;

                    string msg = "ErrorTransmissionCorrespondentNotFound";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                }
                else
                {
                    this.TxtCodeRecipientTransmission.Text = corr.codiceRubrica;
                    this.TxtDescriptionRecipient.Text = corr.descrizione;

                    if (corr.GetType() == typeof(DocsPaWR.Utente))
                    {
                        this.IdRecipient.Value = corr.codiceRubrica + "|" + corr.idAmministrazione;
                        this.RecipientTypeOfCorrespondent.Value = "U";
                    }
                    else if (corr.GetType() == typeof(DocsPaWR.Ruolo))
                    {
                        this.IdRecipient.Value = corr.systemId;
                        this.RecipientTypeOfCorrespondent.Value = "R";
                    }
                    else if (corr.GetType() == typeof(DocsPaWR.UnitaOrganizzativa))
                    {
                        this.TxtCodeRecipientTransmission.Text = string.Empty;
                        this.TxtDescriptionRecipient.Text = string.Empty;
                        this.IdRecipient.Value = string.Empty;
                        this.RecipientTypeOfCorrespondent.Value = string.Empty;

                        string msg = "ErrorTransmissionCorrespondentNotFound";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                }

                this.UpPnlRecipient.Update();

                //this.hideTransmission();
                //this.grdList_Bind();
                //this.EnableButtons();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void TransmissionsBtnClear_Click(object sender, EventArgs e)
        {
            try
            {
                this.chkReceived.Checked = false;
                this.chkTransmittedFromRole.Checked = false;
                this.chkTransmittedFromRF.Checked = false;
                if (this.ddlRF.SelectedIndex >= 0)
                {
                    this.ddlRF.SelectedIndex = 0;
                }
                this.rblRecipientType.SelectedIndex = 0;
                this.IdRecipient.Value = string.Empty;
                this.RecipientTypeOfCorrespondent.Value = string.Empty;
                this.TxtCodeRecipientTransmission.Text = string.Empty;
                this.TxtDescriptionRecipient.Text = string.Empty;
                this.ddlReason.SelectedIndex = -1;
                this.txtDate.Text = string.Empty;
                this.chkAccepted.Checked = false;
                this.chkViewed.Checked = false;
                this.chkPending.Checked = false;
                this.DdlTransmissionsModel.SelectedIndex = -1;
                this.chkRefused.Checked = false;
                this.TransmissionsBtnTransmit.Enabled = false;
                this.TransmissionsBtnModify.Enabled = false;
                this.UpContainer.Update();
                this.TransmissionsBtnSearch_Click(null, null);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TransmissionsBtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                this.hideTransmission();
                this.grdList_Bind();
                this.ReApplyScripts();
                this.EnableButtons();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DdlTransmissionsModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.DdlTransmissionsModel.SelectedItem.ToString()) || this.DdlTransmissionsModel.SelectedItem.Value == null)
                { this.TransmissionsBtnTransmit.Enabled = false; }
                else
                { this.TransmissionsBtnTransmit.Enabled = true; }

                this.ReApplyScripts();
                this.EnableButtons();
                this.upPnlTransmissionsModel.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private bool CustomDocuments
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["customDocuments"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["customDocuments"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["customDocuments"] = value;
            }
        }

        private bool EnableStateDiagram
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableStateDiagram"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableStateDiagram"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableStateDiagram"] = value;
            }
        }

        public string GetSenderName(string name, string delegateName)
        {
            if (string.IsNullOrEmpty(delegateName))
                return name;
            else
                return delegateName + " (" + this.GetLabel("DocumentNoteAuthorDelegatedBy") + " " + name + ")";
        }

    }
}
