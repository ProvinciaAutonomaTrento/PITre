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

namespace NttDataWA.Document
{
    public partial class Transmissions : System.Web.UI.Page
    {

        #region fields
        private ArrayList listaExceptionTrasmissioni = new ArrayList();
        private string PROTOCOL = "P";
        private string NOTPROTOCOL = "N";
        private string idProfile = "";
        private string docNumber = "";
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

        private bool EnabledLibroFirma
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["EnabledLibroFirma"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["EnabledLibroFirma"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["EnabledLibroFirma"] = value;
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

        /// <summary>
        /// Verifica se il documento è in cestino
        /// </summary>
        protected bool DocumentoInCestino
        {
            get
            {
                SchedaDocumento doc = DocumentManager.getSelectedRecord();
                return (!string.IsNullOrEmpty(doc.inCestino) && doc.inCestino == "1");
            }
        }

        protected bool DocumentoInArchivio
        {
            get
            {
                SchedaDocumento doc = DocumentManager.getSelectedRecord();
                return (!string.IsNullOrEmpty(doc.inArchivio) && doc.inArchivio == "1");
            }
        }

        /// <summary>
        /// Verifica se il protocollo è stato annullato
        /// </summary>
        protected bool ProtocolloAnnullato
        {
            get
            {
                bool retValue = false;
                SchedaDocumento doc = DocumentManager.getSelectedRecord();

                if (doc.protocollo != null)
                {
                    DocsPaWR.ProtocolloAnnullato protAnnull = doc.protocollo.protocolloAnnullato;

                    retValue = (protAnnull != null && !string.IsNullOrEmpty(protAnnull.dataAnnullamento));
                }

                return retValue;
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
                if (!DocumentManager.IsNewDocument() && DocumentManager.CheckRevocationAcl())
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}", true);
                    return;
                }

                if (!IsPostBack)
                {
                    this.InfoUser = UIManager.UserManager.GetInfoUser();
                    this.UserLog = UIManager.UserManager.GetUserInSession();
                    this.Role = UIManager.RoleManager.GetRoleInSession();
                    this.Registry = null;

                    SchedaDocumento doc = DocumentManager.getSelectedRecord();
                    if (doc != null)
                    {
                        litObject.Text = Server.HtmlEncode(doc.oggetto.descrizione);

                        if (doc != null && doc.registro != null)
                        {
                            this.Registry = doc.registro;
                        }

                        if (doc.tipoProto.Equals("A"))
                        {
                            this.container.Attributes.Add("class", "borderOrange");
                            this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabOrangeDxBorder");
                            this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabOrange");
                            this.HeaderDocument.TypeRecord = "A";
                        }
                        else
                        {
                            if (doc.tipoProto.Equals("P"))
                            {
                                this.container.Attributes.Add("class", "borderGreen");
                                this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabGreenDxBorder");
                                this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabGreen");
                                this.HeaderDocument.TypeRecord = "P";
                            }
                            else
                            {
                                if (doc.tipoProto.Equals("I"))
                                {
                                    this.container.Attributes.Add("class", "borderBlue");
                                    this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabBlueDxBorder");
                                    this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabBlue");
                                    this.HeaderDocument.TypeRecord = "I";
                                }
                                else
                                {
                                    this.container.Attributes.Add("class", "borderGrey");
                                    this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabGreyDxBorder");
                                    this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabGrey");
                                    this.HeaderDocument.TypeRecord = "N";
                                }
                            }
                        }
                    }
                    this.InitializePage();
                    this.VisibiltyRoleFunctions();
                }
                else
                {
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshExpand", "UpdateExpand();", true);
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
                        if (this.proceed_private.Value == "true") { this.BeginTransmissionIfAllowed(); return; }
                        if (this.proceed_ownership.Value == "true") { this.PerformActionTransmit(); return; }
                        if (this.final_state.Value == "true")
                        {
                            this.ChangeState();
                            return;
                        } 
                        
                        // reset hidden forms
                        this.proceed_personal.Value = "";
                        this.proceed_private.Value = "";
                    }
                    ReadRetValueFromPopup();
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
                    SchedaDocumento doc = DocumentManager.getSelectedRecord();
                    //Visualizzo il dettaglio del contributo
                    SchedaDocumento schedaDoc = UIManager.DocumentManager.getDocumentDetails(this.Page, this.idContributo.Value, this.idContributo.Value);
                    if (schedaDoc != null)
                    {
                        #region navigation
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                        actualPage.IdObject = doc.docNumber;
                        actualPage.OriginalObjectId = doc.docNumber;
                        actualPage.ViewResult = true;
                        actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT_TRANSMISSIONS.ToString(), string.Empty);
                        actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT_TRANSMISSIONS.ToString(), true, this.Page);
                        actualPage.CodePage = Navigation.NavigationUtils.NamePage.DOCUMENT_TRANSMISSIONS.ToString();

                        actualPage.Page = "TRANSMISSIONS.ASPX";
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
            if (!string.IsNullOrEmpty(this.SearchProject.ReturnValue))
            {
                if (this.ReturnValue.Split('#').Length > 1)
                {
                    this.TxtCodeProject.Text = this.ReturnValue.Split('#').First();
                    this.TxtDescriptionProject.Text = this.ReturnValue.Split('#').Last();
                    this.UpPnlProject.Update();
                    TxtCodeProject_OnTextChanged(new object(), new EventArgs());
                }
                else
                    //Laura 19 Marzo
                    if (this.ReturnValue.Contains("//"))
                    {
                        this.TxtCodeProject.Text = this.ReturnValue;
                        this.TxtDescriptionProject.Text = "";
                        this.UpPnlProject.Update();
                        TxtCodeProject_OnTextChanged(new object(), new EventArgs());
                    }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SearchProject','');", true);
            }
            if (!string.IsNullOrEmpty(this.OpenTitolario.ReturnValue))
            {

                if (this.ReturnValue.Split('#').Length > 1)
                {
                    this.TxtCodeProject.Text = this.ReturnValue.Split('#').First();
                    this.TxtDescriptionProject.Text = this.ReturnValue.Split('#').Last();
                    this.UpPnlProject.Update();
                    TxtCodeProject_OnTextChanged(new object(), new EventArgs());
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenTitolario','');", true);
            }
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
            Session["PrintTransm"] = "D"; //Sessione stampa trasmissione documenti 

            this.LoadKeys();
            this.InitializesLabel();
            this.LoadTransmissionModels();
            this.LoadReasons();
            this.InitializeAddressBooks();

            this.LoadRF();

            this.ResetFilters();
            this.grdList_Bind();
            this.EnableButtons();
            this.ControlAbortDocument();
        }

        private SchedaDocumento DocumentInWorking
        {
            get
            {
                SchedaDocumento result = null;
                if (HttpContext.Current.Session["document"] != null)
                {
                    result = HttpContext.Current.Session["document"] as SchedaDocumento;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["document"] = value;
            }
        }

        private void ChangeState()
        {

            DocsPaWR.Trasmissione trasmissione = TrasmManager.getSelectedTransmission();
            // da verifiarlo solo per i documenti
            DocsPaWR.InfoDocumento infoDocumento = trasmissione.infoDocumento;
            DocsPaWR.Stato statoSucc = DiagrammiManager.getStatoSuccessivoAutomatico(infoDocumento.docNumber);
            DocsPaWR.DiagrammaStato dg = DiagrammiManager.getDiagrammaById(Convert.ToString(statoSucc.ID_DIAGRAMMA));
            DiagrammiManager.salvaModificaStato(infoDocumento.docNumber, Convert.ToString(statoSucc.SYSTEM_ID), dg, UserManager.GetInfoUser().userId, UserManager.GetInfoUser(), "", this.Page);
            //ho messo il documento in stato finale dunque modifico anche l'accessRigths
            DocumentManager.RightsDocumentChanges(HMdiritti.HMdiritti_Read, this.DocumentInWorking.systemId);
            //inserisco lo stato successivo cambiato in salvamodificastato
            statoSucc = DiagrammiManager.GetStateDocument(infoDocumento.docNumber);
            string idTipoDoc = ProfilerDocManager.getIdTemplate(infoDocumento.docNumber);

            if (idTipoDoc != "")
            {

                ArrayList modelli = new ArrayList(DiagrammiManager.isStatoTrasmAuto(UserManager.GetInfoUser().idAmministrazione, Convert.ToString(statoSucc.SYSTEM_ID), idTipoDoc));
                InfoDocumento infoDoc = DocumentManager.getInfoDocumento(this.DocumentInWorking);
                for (int i = 0; i < modelli.Count; i++)
                {
                    DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                    if (mod.SINGLE == "1")
                    {
                        TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(statoSucc.SYSTEM_ID), infoDoc, this);
                    }
                    else
                    {
                        for (int k = 0; k < mod.MITTENTE.Length; k++)
                        {
                            if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.GetSelectedRole().systemId)
                            {
                                TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(statoSucc.SYSTEM_ID), infoDoc, this);
                                break;
                            }
                        }
                    }
                }

                this.EnableButtons();
                this.showTransmission();
            }
        }


        private void ControlAbortDocument()
        {
            SchedaDocumento doc = DocumentManager.getSelectedRecord();
            if (doc.tipoProto.ToUpper().Equals("A") || doc.tipoProto.ToUpper().Equals("P") || doc.tipoProto.ToUpper().Equals("I"))
            {
                if (doc != null && doc.tipoProto != null && doc.protocollo.protocolloAnnullato != null)
                {
                    this.DisableAllButtons();
                }
            }
        }

        /// <summary>
        /// Initializes application labels
        /// </summary>
        private void InitializesLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.SignatureA4.Title = Utils.Languages.GetLabelFromCode("PopupSignatureA4", language);
            this.TransmissionLitSubject.Text = Utils.Languages.GetLabelFromCode("TransmissionLitSubject", language);
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
            this.TransmissionsBtnAcquireRights.Text = Utils.Languages.GetLabelFromCode("ViewDetailNotifyBtnAcquireRights", language);
            this.TransmissionsBtnAcquireRights.ToolTip = Utils.Languages.GetLabelFromCode("ViewDetailNotifyBtnAcquireRightsTooltip", language);
            this.TrasmissionsImgAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("TrasmissionsImgAddressBook", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
            this.TransmissionsBtnAcceptLF.Text = Languages.GetLabelFromCode("ViewDetailNotifyBtnAcceptLF", language);
            this.TransmissionsBtnAcceptLF.ToolTip = Languages.GetLabelFromCode("ViewDetailNotifyBtnAcceptLFTooltip", language);
            this.PrintLabel.Title = Utils.Languages.GetLabelFromCode("PrintLabelPopUpTitle", language);
            this.ddlReason.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("TransmissionDdlReason", language);
            this.chkCreatoreExtendHistoricized.Text = Utils.Languages.GetLabelFromCode("SearchProjectCreatoreExtendHistoricized", language);
            this.SearchProject.Title = Utils.Languages.GetLabelFromCode("SearchProjectTitle", language);
            this.OpenTitolario.Title = Utils.Languages.GetLabelFromCode("TitleClassificationScheme", language);
            this.DocumentLitClassificationRapidTrasm.Text = Languages.GetLabelFromCode("DocumentLitClassificationRapidTrasm", language);
            this.DocumentImgSearchProjects.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgSearchProjects", language);
            this.DocumentImgSearchProjects.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgSearchProjects", language);
            this.btnclassificationschema.AlternateText = Utils.Languages.GetLabelFromCode("btnclassificationschema", language);
            this.btnclassificationschema.ToolTip = Utils.Languages.GetLabelFromCode("btnclassificationschema", language);
            this.ImgAddProjects.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgAddProjects", language);
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

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()].Equals("1"))
            {
                this.CustomDocuments = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()].Equals("1"))
            {
                this.EnableStateDiagram = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString()).Equals("1"))
            {
                this.EnabledLibroFirma = true;
            }
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_BLOCCA_CLASS.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_BLOCCA_CLASS.ToString()).Equals("1"))
            {
                this.EnableBlockClassification = true;
            }
        }

        protected void InitializeAddressBooks()
        {
            this.SetAjaxAddressBook();
        }

        protected void SetAjaxAddressBook()
        {
            this.RapidRecipient.MinimumPrefixLength = this.AjaxAddressBookMinPrefixLenght;

            string dataUser = this.Role.systemId;
            if (this.Registry == null)
            {
                this.Registry = RegistryManager.GetRegistryInSession();
            }
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
            // reset paging
            bool isNewSearch = this.checkIfNewSearch();
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
                this.FilterActiveRF = this.chkTransmittedFromRF.Checked;
                this.FilterRF = ddlRF.SelectedValue;
                this.grid_pageindex.Value = this.SelectedPage.ToString();
                this.grid_rowindex.Value = this.SelectedRowIndex.ToString();
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

            // Reperimento trasmissioni per il documento corrente
            DocsPaWR.InfoTrasmissione[] infoTrasmList = this.GetTrasmissioniDocumento(ref pagingContext);

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
        /// Reperimento trasmissioni per il documento corrente
        /// </summary>
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        private DocsPaWR.InfoTrasmissione[] GetTrasmissioniDocumento(ref DocsPaWR.SearchPagingContext pagingContext)
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

                // Aggiunta filtro per estensione ricerca a storicizzati
                f = new DocsPaWR.FiltroRicerca();
                f.argomento = FiltriTrasmissioneNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString();
                f.valore = this.chkCreatoreExtendHistoricized.Checked.ToString();
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


            SchedaDocumento doc = DocumentManager.getSelectedRecord();
            return TrasmManager.GetInfoTrasmissioniFiltered(doc.systemId, "D", filters, ref pagingContext);
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
                this.ControlAbortDocument();
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
                        TrasmManager.setSelectedTransmission(TrasmManager.GetTransmission(this, "D"));
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

        protected void hideTransmission()
        {
            this.plcTransmission.Visible = false;
            this.UpPnlTransmission.Update();
        }

        protected void BuildSingleTransmissionsTables(Trasmissione trasm, TrasmissioneSingola trasmSing)
        {
            Table table = this.GetTransmissionTable(trasmSing.systemId);



            // DRAW TABLE
            ((TableCell)GetControlById(table, "trasmDetailsRecipient" + trasmSing.systemId)).Text = "<strong>" + formatBlankValue(trasmSing.corrispondenteInterno.descrizione) + "</strong>";
            ((TableCell)GetControlById(table, "trasmDetailsReason" + trasmSing.systemId)).Text = formatBlankValue(trasmSing.ragione.descrizione);

            string language = UIManager.UserManager.GetUserLanguage();
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

            if (this.IsSingleNotesVisible(trasm, trasmSing))
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

                    if (!string.IsNullOrEmpty(TrasmUt.dataRimossaTDL) && (string.IsNullOrEmpty(TrasmUt.cha_rimossa_delegato) || TrasmUt.cha_rimossa_delegato.Equals("0")))
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
                    // INC000000511165
                    // Uniformato il comportamento a quello del centro notifiche
                    // LA note di accettazione/rifiuto è visibile a tutti gli utenti del ruolo a cui appartiene l'utente MITTENTE della trasmissione
                    //if (CheckTrasmEffettuataDaUtenteCorrente(trasm) || CheckTrasmUtenteCorrente(TrasmUt))
                    if (CheckTrasmEffettuataDaUtenteInRuoloCorrente(trasm) || CheckTrasmUtenteCorrente(TrasmUt))
                    {
                        if (!string.IsNullOrEmpty(TrasmUt.dataAccettata) && (string.IsNullOrEmpty(TrasmUt.cha_accettata_delegato) || TrasmUt.cha_accettata_delegato.Equals("0")))
                        {
                            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.ENABLE_TASK.ToString())) && 
                                Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.ENABLE_TASK.ToString()).Equals("1")
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
                        if (!string.IsNullOrEmpty(TrasmUt.dataRifiutata) || !string.IsNullOrEmpty(TrasmUt.dataAccettata))
                        {
                            ((TableCell)GetControlById(row, "trasmDetailsInfo" + TrasmUt.systemId)).Text = new string('-', 15);
                        }
                        else
                        {
                            ((TableCell)GetControlById(row, "trasmDetailsInfo" + TrasmUt.systemId)).Text = formatBlankValue(null);
                        }
                        //((TableCell)GetControlById(row, "trasmDetailsInfo" + TrasmUt.systemId)).Text = new string('-', 15);
                    }

                    table.Controls.Add(row);


                    // RIGA IN CASO DI DELEGA
                    string del = Utils.Languages.GetLabelFromCode("TransmissionDelegatedBy", language);
                    if (!string.IsNullOrEmpty(TrasmUt.idPeopleDelegato) && (TrasmUt.cha_accettata_delegato == "1" || TrasmUt.cha_rifiutata_delegato == "1" || TrasmUt.cha_vista_delegato == "1" ||  TrasmUt.cha_rimossa_delegato == "1") && (!string.IsNullOrEmpty(TrasmUt.dataVista) || !string.IsNullOrEmpty(TrasmUt.dataAccettata) || !string.IsNullOrEmpty(TrasmUt.dataRifiutata) || !string.IsNullOrEmpty(TrasmUt.dataRimossaTDL)))
                    {
                        string idPeopleDelegatoTrasm = TrasmUt.systemId.ToString() + TrasmUt.idPeopleDelegato;
                        TableRow row2 = GetDetailsRow(idPeopleDelegatoTrasm);
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
                        if (TrasmUt.cha_rimossa_delegato.Equals("1"))
                        {
                            ((TableCell)GetControlById(row2, "trasmDetailsRemoved" + idPeopleDelegatoTrasm)).Text = this.formatBlankValue(TrasmUt.dataRimossaTDL);
                        }
                        else
                        {
                            ((TableCell)GetControlById(row2, "trasmDetailsRemoved" + idPeopleDelegatoTrasm)).Text = this.formatBlankValue(null);
                        }
                        if (CheckTrasmEffettuataDaUtenteInRuoloCorrente(trasm) || CheckTrasmUtenteCorrente(TrasmUt))
                        {
                            if (!string.IsNullOrEmpty(TrasmUt.dataAccettata) && TrasmUt.cha_accettata_delegato.Equals("1"))
                            {
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
                    if (task.UTENTE_DESTINATARIO.idPeople.Equals(UserManager.GetUserInSession().idPeople))
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
                if (isTaskSender)
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
                SchedaDocumento doc = DocumentManager.getSelectedRecord();
                #region navigation
                List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                actualPage.IdObject = doc.docNumber;
                actualPage.OriginalObjectId = doc.docNumber;

                actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT_TRANSMISSIONS.ToString(), string.Empty);
                actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT_TRANSMISSIONS.ToString(), true, this.Page);
                actualPage.CodePage = Navigation.NavigationUtils.NamePage.DOCUMENT_TRANSMISSIONS.ToString();

                actualPage.Page = "TRANSMISSIONS.ASPX";
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
                SchedaDocumento doc = DocumentManager.getSelectedRecord();
                string idTask = task.ID_TASK;
                string idObject = !string.IsNullOrEmpty(task.ID_PROFILE) ? task.ID_PROFILE : task.ID_PROJECT;
                string error = this.CreaContributo(task);
                if (string.IsNullOrEmpty(error))
                {
                    #region navigation
                    List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                    Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                    actualPage.IdObject = doc.systemId;
                    actualPage.OriginalObjectId = doc.systemId;

                    actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT_TRANSMISSIONS.ToString(), string.Empty);
                    actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT_TRANSMISSIONS.ToString(), true, this.Page);
                    actualPage.CodePage = Navigation.NavigationUtils.NamePage.DOCUMENT_TRANSMISSIONS.ToString();

                    actualPage.Page = "TRANSMISSIONS.ASPX";
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
        /// Verifica se la trasmissione è stata effettuata
        /// da un utente del ruolo dell'utente attualmente connesso
        /// </summary>
        /// <param name="trasm"></param>
        /// <returns></returns>
        private bool CheckTrasmEffettuataDaUtenteInRuoloCorrente(Trasmissione trasm)
        {
            bool retValue = false;

            if (trasm != null && trasm.ruolo != null && trasm.ruolo.idGruppo != null)
                retValue = (trasm.ruolo.idGruppo.Equals(UserManager.GetInfoUser().idGruppo));
            else if (trasm != null && trasm.ruolo != null && trasm.ruolo.codice != null)
                retValue = (trasm.ruolo.codice.Equals(this.Role.codice));

            return retValue;
        }

        /// <summary>
        ///Verifica se le note singole devono essere visualizzate.
        ///Rules:
        ///Le note singole sono visibili se:
        /// -   Il ruolo corrente è il mittente della trasmissione;
        /// -   Il ruolo corrente è il destinatario della tramissione;
        /// -   La trasmissione è ad utente e
        ///
        /// </summary>
        /// <returns></returns>
        private bool IsSingleNotesVisible(Trasmissione trasm, TrasmissioneSingola trasmSing)
        {
            bool visible = false;
            if (trasm != null &&
                trasm.ruolo != null &&
                !string.IsNullOrEmpty(trasm.ruolo.systemId) &&
                trasm.ruolo.systemId.Equals(RoleManager.GetRoleInSession().systemId))
                visible = true;
            else if (trasmSing != null &&
                trasmSing.tipoDest != null &&
                trasmSing.tipoDest == TrasmissioneTipoDestinatario.RUOLO &&
                !string.IsNullOrEmpty(trasmSing.corrispondenteInterno.systemId) &&
                trasmSing.corrispondenteInterno.systemId.Equals(RoleManager.GetRoleInSession().systemId))
                visible = true;
            else if (trasmSing != null &&
                trasmSing.tipoDest != null &&
                trasmSing.tipoDest == TrasmissioneTipoDestinatario.UTENTE &&
                (trasmSing.corrispondenteInterno as Utente) != null &&
                (trasmSing.corrispondenteInterno as Utente).idPeople.Equals(UserManager.GetInfoUser().idPeople))
                visible = true;
            return visible;
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
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpPnlRecipient", "ajaxModalPopupAddressBook();", true);
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
                return;
            }
        }

        protected void TransmissionsBtnTransmit_Click(object sender, EventArgs e)
        {

            if (!DocumentManager.CheckRevocationAcl())
            {
                SchedaDocumento doc = DocumentManager.getSelectedRecord();


                bool docFattEle = false;
                
                if(DocumentManager.getSelectedRecord()!=null && DocumentManager.getSelectedRecord().tipologiaAtto!=null
                    && !String.IsNullOrEmpty(DocumentManager.getSelectedRecord().tipologiaAtto.descrizione)
                    )
                       docFattEle= DocumentManager.getSelectedRecord().tipologiaAtto.descrizione.ToUpper().Equals("FATTURA ELETTRONICA");

                if (doc.personale == "1" && !docFattEle)
                {
                    string msg = "ConfirmTransmissionPersonalDocument";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'proceed_personal', '');} else {parent.ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'proceed_personal', '');}", true);
                }
                else if (doc.privato == "1"  && !docFattEle)
                {
                    string msg = "ConfirmTransmissionPrivateDocument";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) { parent.fra_main.ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'proceed_private', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'proceed_private', '');}", true);
                }
                else
                {
                    this.BeginTransmissionIfAllowed();
                }
            }

            this.EnableButtons();
            this.DocumentTabs.RefreshLayoutTab();
            this.UpContainerDocumentTab.Update();


        }

        private void BeginTransmissionIfAllowed()
        {

            string valoreChiaveCediDiritti = InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_CEDI_DIRITTI_IN_RUOLO.ToString());
            DocsPaWR.Trasmissione trasmEff = TrasmManager.getSelectedTransmission();
            if (!string.IsNullOrEmpty(valoreChiaveCediDiritti) && valoreChiaveCediDiritti.Equals("1") && this.esisteRagTrasmCessioneInTrasm(trasmEff))
            {
                string accessRights = string.Empty;
                string idGruppoTrasm = string.Empty;
                string tipoDiritto = string.Empty;
                string IDObject = string.Empty;

                bool isPersonOwner = false;
                DocsPaWR.SchedaDocumento sd = DocumentManager.getSelectedRecord();
                IDObject = sd.systemId;

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

        private bool esisteRagTrasmCessioneInTrasm(DocsPaWR.Trasmissione trasmissione)
        {
            bool retValue = false;

            // verifica solo se le trasm.ni singole sono più di una
            if (trasmissione.trasmissioniSingole != null && trasmissione.trasmissioniSingole.Length > 0)
            {
                // conta quante trasm.ni singole hanno la ragione con la cessione impostata
                int trasmConCessione = 0;
                foreach (DocsPaWR.TrasmissioneSingola trasmS in trasmissione.trasmissioniSingole)
                    if (trasmS.ragione.cessioneImpostata)
                        trasmConCessione++;


                if (trasmConCessione > 0) //sono state già inserite ragioni con cessione 
                {
                    retValue = true;
                }
            }

            return retValue;
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
                this.PerformActionTransmitDocument();
            }
        }

        /// <summary>
        /// Iacozzilli: Faccio la Get dell'idProprietario del doc!
        /// </summary>
        /// <returns></returns>
        private string GetAnagUtenteProprietario()
        {
            DocumentoDiritto[] listaVisibilita = null;
            DocsPaWR.SchedaDocumento sd = DocumentManager.getSelectedRecord();
            string idProprietario = string.Empty;
            listaVisibilita = DocumentManager.getListaVisibilitaSemplificata(this, sd.docNumber, true);
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

        private void TransmitModel()
        {
            ModelloTrasmissione modello = TrasmManager.GetTemplateById(this.Registry.idAmministrazione, this.DdlTransmissionsModel.SelectedValue);

            if (modello != null && modello.SYSTEM_ID != 0)
            {
                //SchedaDocumento doc = DocumentManager.getSelectedRecord();

                //if (doc.privato == "1")
                //{
                //    if (string.IsNullOrEmpty(this.extend_visibility.Value))
                //    {
                //        if (TrasmManager.GetModelInheritsVisibility(modello.SYSTEM_ID.ToString()))
                //        {
                //            string msg = "ConfirmTransmissionBlockHierarchy";
                //            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'extend_visibility', '');} else {parent.ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'extend_visibility', '');}", true);
                //        }
                //    }
                //}
                //else
                //{
                //    this.PerformActionTransmitModel();
                //}

                this.PerformActionTransmitModel();
            }
        }

        /// <summary>
        /// Azione di trasmissione del documento corrente
        /// </summary>
        private void PerformActionTransmitDocument()
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

            TrasmManager.PerformAutomaticStateChangeDoc(trasmEff);

            // re-get document to avoid rights checks
            //SchedaDocumento tempDoc = DocumentManager.getSelectedRecord();
            //DocumentManager.setSelectedRecord(DocumentManager.getDocumentDetails(this, tempDoc.docNumber, tempDoc.docNumber));

            //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
            if (DocumentManager.CheckRevocationAcl())
            {
                DocumentManager.setSelectedRecord(null);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}", true);
                return;
            }

            this.ResetHiddenFields();

            this.grdList_Bind();
            this.upPnlGridList.Update();
        }

        private Trasmissione BuildTransmissionFromModel_new()
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
                DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getSelectedRecord();

                transmission.tipoOggetto = DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
                transmission.infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);

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

        private Trasmissione BuildTransmissionFromModel()
        {
            logger.Info("BEGIN");

            DocsPaWR.Trasmissione trasmissione = new DocsPaWR.Trasmissione();

            try
            {
                DocsPaWR.ModelloTrasmissione template = TransmissionModelsManager.GetTemplateById(UserManager.GetInfoUser().idAmministrazione, this.DdlTransmissionsModel.SelectedValue);
                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(DocumentManager.getSelectedRecord());


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
                DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getSelectedRecord();

                trasmissione.tipoOggetto = DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
                trasmissione.infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);

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
            trasmissione.tipoOggetto = DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
            trasmissione.infoDocumento = DocumentManager.getInfoDocumento(DocumentManager.getSelectedRecord());
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


                // Mev Cessione Diritti - mantieni scrittura
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
                        corr = TrasmManager.getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, DocumentManager.getSelectedRecord(), null, this);
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
                SchedaDocumento doc = DocumentManager.getSelectedRecord();

                DocsPaWR.Stato stato = DiagrammiManager.GetStateDocument(doc.systemId);
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
                    TrasmManager.salvaStoricoTrasmDiagrammi(trasmissione.systemId, doc.docNumber, Convert.ToString(stato.SYSTEM_ID));
            }


            // re-get document to avoid rights checks
            //SchedaDocumento tempDoc = DocumentManager.getSelectedRecord();
            //DocumentManager.setSelectedRecord(DocumentManager.getDocumentDetails(this, tempDoc.docNumber, tempDoc.docNumber));

            //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
            if (DocumentManager.CheckRevocationAcl())
            {
                DocumentManager.setSelectedRecord(null);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}", true);
                return;
            }
            else
            {
                if (trasmissione.cessione != null && trasmissione.cessione.docCeduto)
                    if (trasmissione.mantieniLettura)
                        DocumentManager.setSelectedRecord(DocumentManager.getDocumentDetails(this, DocumentManager.getSelectedRecord().docNumber, DocumentManager.getSelectedRecord().docNumber));
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
            this.extend_visibility.Value = "";
            this.proceed_personal.Value = "";
            this.proceed_private.Value = "";
            this.proceed_ownership.Value = "";
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
            Registro[] rf = GetRF();

            for (int i = 0; i < rf.Length; i++)
            {
                ListItem li = new ListItem(rf[i].codRegistro, rf[i].systemId);
                this.ddlRF.Items.Add(li);
            }
        }

        private void LoadReasons()
        {
            SchedaDocumento doc = DocumentManager.getSelectedRecord();
            RagioneTrasmissione[] listaRagioni = TrasmManager.getListaRagioni(this, doc.accessRights, false);

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

        //private ArrayList GetTransmissionModels()
        //{
        //    string idTypeDoc = string.Empty;
        //    string idDiagram = string.Empty;
        //    string idState = string.Empty;
        //    string idDocument = string.Empty;
        //    string accessRights = string.Empty;

        //    /*  if (this.CustomDocuments)
        //      {
        //          if (schedaDocumento.tipologiaAtto != null)
        //          {
        //              //DocsPaWR.Templates template = (Templates)Session["template"];
        //              //if (template == null)
        //              //    template = wws.getTemplate(idAmm, schedaDocumento.tipologiaAtto.descrizione, schedaDocumento.docNumber);

        //              //if (template != null)
        //              //{
        //              //    idTipoDoc = template.SYSTEM_ID.ToString();
        //              if (schedaDocumento.template != null && schedaDocumento.template.SYSTEM_ID.ToString() != "" && !isRiproposto)
        //              {
        //                  idTipoDoc = schedaDocumento.template.SYSTEM_ID.ToString();

        //                  if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
        //                  {
        //                      DocsPaWR.DiagrammaStato dg = NttDataWA.DiagrammiManager.getDgByIdTipoDoc(schedaDocumento.tipologiaAtto.systemId, idAmm, this);
        //                      if (dg != null)
        //                      {
        //                          idDiagramma = dg.SYSTEM_ID.ToString();
        //                          DocsPaWR.Stato stato = NttDataWA.DiagrammiManager.getStatoDoc(schedaDocumento.docNumber, this);
        //                          if (stato != null)
        //                              idStato = stato.SYSTEM_ID.ToString();
        //                      }
        //                  }
        //              }
        //          }
        //      }*/
        //    //
        //    Registro[] listReg = null;
        //    if (this.Registry != null)
        //    {
        //        listReg = new Registro[1];
        //        listReg[0] = this.Registry;
        //    }

        //    if (listReg == null)
        //    {
        //        listReg = this.Role.registri;
        //    }
        //    return new ArrayList(UIManager.TransmissionModelsManager.GetTransmissionModelsLite(this.UserLog.idAmministrazione, listReg, this.UserLog.idPeople, this.InfoUser.idCorrGlobali, idTypeDoc, idDiagram, idState, "D", idDocument, this.Role.idGruppo, false, accessRights));
        //}

        private void LoadTransmissionModels()
        {

            string idTypeDoc = string.Empty;
            string idDiagram = string.Empty;
            string idState = string.Empty;
            string idDocument = string.Empty;
            string accessRights = string.Empty;

            if (DocumentManager.getSelectedRecord() != null && !string.IsNullOrEmpty(DocumentManager.getSelectedRecord().systemId))
            {
                accessRights = DocumentManager.getSelectedRecord().accessRights;
                idDocument = DocumentManager.getSelectedRecord().systemId;
            }

            if (this.CustomDocuments && DocumentManager.getSelectedRecord() != null && !string.IsNullOrEmpty(DocumentManager.getSelectedRecord().systemId) && DocumentManager.getSelectedRecord().template != null)
            {
                if (this.EnableStateDiagram)
                {
                    DiagrammaStato dia = DiagrammiManager.getDgByIdTipoDoc(idTypeDoc, this.InfoUser.idAmministrazione);
                    if (dia != null)
                    {
                        idDiagram = dia.SYSTEM_ID.ToString();
                        DocsPaWR.Stato stato = DiagrammiManager.GetStateDocument(DocumentManager.getSelectedRecord().docNumber);
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

            ArrayList idModelli = new ArrayList(UIManager.TransmissionModelsManager.GetTransmissionModelsLite(this.UserLog.idAmministrazione, listReg, this.UserLog.idPeople, this.InfoUser.idCorrGlobali, idTypeDoc, idDiagram, idState, "D", idDocument, this.Role.idGruppo, false, accessRights));

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

            if (trasmissione.tipoOggetto == TrasmissioneTipoOggetto.DOCUMENTO)
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
            this.SetAjaxDescriptionProject();
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

            // re-get document to avoid rights checks
            //SchedaDocumento tempDoc = DocumentManager.getSelectedRecord();
            //DocumentManager.setSelectedRecord(DocumentManager.getDocumentDetails(this, tempDoc.docNumber, tempDoc.docNumber));

            //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
            if (DocumentManager.CheckRevocationAcl())
            {
                DocumentManager.setSelectedRecord(null);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}", true);
                return;
            }
        }

        protected void TransmissionsBtnAcceptLF_Click(object sender, System.EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            InfoUtente infoUtente = UserManager.GetInfoUser();
            this.acceptTransmission();
            DocsPaWR.Trasmissione trasmissione = TrasmManager.getSelectedTransmission();
            if (trasmissione.infoDocumento != null)
            {
                DocumentManager.addWorkArea(this, trasmissione.infoDocumento);

                System.Collections.Generic.List<DocsPaWR.TrasmissioneSingola> list = new System.Collections.Generic.List<TrasmissioneSingola>(trasmissione.trasmissioniSingole);
                List<DocsPaWR.TrasmissioneSingola> trasmSingoleUtente = list.Where(i => i.tipoDest == TrasmissioneTipoDestinatario.UTENTE).ToList();
                TrasmissioneSingola trasmSing = trasmSingoleUtente.Where(i => ((DocsPaWR.Utente)i.corrispondenteInterno).idPeople == infoUtente.idPeople).FirstOrDefault();

                if (trasmSing == null)
                {
                    // Se non è stata trovata la trasmissione come destinatario ad utente, 
                    // cerca quella con destinatario ruolo corrente dell'utente
                    trasmSingoleUtente = list.Where(i => i.tipoDest == TrasmissioneTipoDestinatario.RUOLO).ToList();
                    trasmSing = TrasmManager.RoleTransmissionWithHistoricized(trasmSingoleUtente, infoUtente.idCorrGlobali);
                }

                LibroFirmaManager.InserimentoInLibroFirma(this.Page, trasmissione, trasmSing.systemId);
            }
        }

        protected void TransmissionBtnAcquireRights_Click(object sender, EventArgs e)
        {
            this.AcquireRights();
        }

        private void acceptTransmission()
        {
            logger.Info("BEGIN");
            DocsPaWR.Trasmissione trasmissione = TrasmManager.getSelectedTransmission();

            //Effettuo l'accettazione della trasmissione
            bool result = accettaRifiuta(DocsPaWR.TrasmissioneTipoRisposta.ACCETTAZIONE);
            if (result)
            {
                SchedaDocumento doc = DocumentManager.getSelectedRecord();
                doc = DocumentManager.getDocumentDetails(this.Page, doc.systemId, doc.docNumber);
                DocumentManager.setSelectedRecord(doc);
            }

            //Verifico l'abilitazione dei diagrammi di stato
            if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
            {
                // DocsPaWR.Trasmissione trasmissione = (NttDataWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this);
                if (trasmissione.tipoOggetto.ToString() == "DOCUMENTO")
                {
                    DocsPaWR.InfoDocumento infoDocumento = trasmissione.infoDocumento;

                    //E' importante che l'accettazione della trasmiossione corrente sia fatta prima di questo tipo di verifica
                    if (DiagrammiManager.isUltimaDaAccettare(trasmissione.systemId, this))
                    {

                        DocsPaWR.Stato statoSucc = DiagrammiManager.getStatoSuccessivoAutomatico(infoDocumento.docNumber);
                        DocsPaWR.Stato statoCorr = DiagrammiManager.GetStateDocument(infoDocumento.docNumber);

                        //Se il documento è di una tipologia sospesa non viene fatta nessuna considerazione su un eventuale passaggio di stato automatico
                        if (!string.IsNullOrEmpty(infoDocumento.idTipoAtto))
                        {
                            Templates tipoDocumento = ProfilerDocManager.getTemplate(infoDocumento.docNumber);
                            if (tipoDocumento != null && tipoDocumento.IN_ESERCIZIO.ToUpper().Equals("NO"))
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
                                //Controllo se non è bloccato il documento principale o un suo allegato
                                /* APPLET_G */
                                if (CheckInOut.CheckInOutServices.IsCheckedOutDocument(infoDocumento.idProfile, infoDocumento.docNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord()) 
                                    || CheckInOutApplet.CheckInOutServices.IsCheckedOutDocument(infoDocumento.idProfile, infoDocumento.docNumber, UserManager.GetInfoUser(), true))
                                {
                                    string msg = "ErrorTransmissionDocumentOrAttachmentsBlocked";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                                    return;
                                }

                                string msg2 = "ConfirmTransmissionFinalState";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msg2.Replace("'", @"\'") + "', 'final_state', '');} else {parent.ajaxConfirmModal('" + msg2.Replace("'", @"\'") + "', 'final_state', '');}", true);
                                return;
                            }
                            if (DiagrammiManager.isStatoAuto(statoSucc.SYSTEM_ID.ToString(), statoSucc.ID_DIAGRAMMA.ToString()))
                            {
                                //Controllo se non è bloccato il documento principale o un suo allegato
                                /* APPLET_G */
                                if (CheckInOut.CheckInOutServices.IsCheckedOutDocument(infoDocumento.idProfile, infoDocumento.docNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord())
                                    || CheckInOutApplet.CheckInOutServices.IsCheckedOutDocument(infoDocumento.idProfile, infoDocumento.docNumber, UserManager.GetInfoUser(), true))
                                {
                                    string msg = "ErrorTransmissionDocumentOrAttachmentsBlocked";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                                    return;
                                }
                            }
                            //Cambio stato
                            DocsPaWR.DiagrammaStato dg = DiagrammiManager.getDiagrammaById(Convert.ToString(statoSucc.ID_DIAGRAMMA));
                            DiagrammiManager.salvaModificaStato(infoDocumento.docNumber, Convert.ToString(statoSucc.SYSTEM_ID), dg, UserManager.GetInfoUser().userId, UserManager.GetInfoUser(), "", this.Page);

                            //Cancellazione storico trasmissioni
                            DiagrammiManager.deleteStoricoTrasmDiagrammi(infoDocumento.docNumber, Convert.ToString(statoCorr.SYSTEM_ID));

                            //Verifico se il nuovo stato ha delle trasmissioni automatiche
                            DocsPaWR.Stato stato = DiagrammiManager.GetStateDocument(infoDocumento.docNumber);
                            string idTipoDoc = ProfilerDocManager.getIdTemplate(infoDocumento.docNumber);
                            if (idTipoDoc != "")
                            {
                                ArrayList modelli = new ArrayList(DiagrammiManager.isStatoTrasmAuto(UserManager.GetInfoUser().idAmministrazione, Convert.ToString(stato.SYSTEM_ID), idTipoDoc));
                                for (int i = 0; i < modelli.Count; i++)
                                {
                                    DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                                    if (mod.SINGLE == "1")
                                    {
                                        TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), trasmissione.infoDocumento, this);
                                    }
                                    else
                                    {
                                        for (int k = 0; k < mod.MITTENTE.Length; k++)
                                        {
                                            if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.GetSelectedRole().systemId)
                                            {
                                                TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), trasmissione.infoDocumento, this);
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
            this.DocumentTabs.RefreshLayoutTab();
            this.UpContainerDocumentTab.Update();
            logger.Info("END");
        }

        private void AcquireRights()
        {
            DocsPaWR.Trasmissione trasmissione = TrasmManager.getSelectedTransmission();

            //Effettuo l'accettazione della trasmissione
            bool result = accettaRifiuta(DocsPaWR.TrasmissioneTipoRisposta.ACCETTAZIONE);

            if (result)
            {
                SchedaDocumento doc = DocumentManager.getSelectedRecord();
                doc = DocumentManager.getDocumentDetails(this.Page, doc.systemId, doc.docNumber);
                DocumentManager.setSelectedRecord(doc);
            }

            //Verifico l'abilitazione dei diagrammi di stato
            if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
            {
                // DocsPaWR.Trasmissione trasmissione = (NttDataWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this);
                if (trasmissione.tipoOggetto.ToString() == "DOCUMENTO")
                {
                    DocsPaWR.InfoDocumento infoDocumento = trasmissione.infoDocumento;

                    //E' importante che l'accettazione della trasmiossione corrente sia fatta prima di questo tipo di verifica
                    if (DiagrammiManager.isUltimaDaAccettare(trasmissione.systemId, this))
                    {

                        DocsPaWR.Stato statoSucc = DiagrammiManager.getStatoSuccessivoAutomatico(infoDocumento.docNumber);
                        DocsPaWR.Stato statoCorr = DiagrammiManager.GetStateDocument(infoDocumento.docNumber);

                        //Se il documento è di una tipologia sospesa non viene fatta nessuna considerazione su un eventuale passaggio di stato automatico
                        if (!string.IsNullOrEmpty(infoDocumento.idTipoAtto))
                        {
                            Templates tipoDocumento = ProfilerDocManager.getTemplate(infoDocumento.docNumber);
                            if (tipoDocumento != null && tipoDocumento.IN_ESERCIZIO.ToUpper().Equals("NO"))
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
                                //Controllo se non è bloccato il documento principale o un suo allegato
                                /* APPLET_G */
                                if (CheckInOut.CheckInOutServices.IsCheckedOutDocument(infoDocumento.idProfile, infoDocumento.docNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord())
                                    || CheckInOutApplet.CheckInOutServices.IsCheckedOutDocument(infoDocumento.idProfile, infoDocumento.docNumber, UserManager.GetInfoUser(), true))
                                {
                                    string msg = "ErrorTransmissionDocumentOrAttachmentsBlocked";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                                    return;
                                }

                                string msg2 = "ConfirmTransmissionFinalState";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msg2.Replace("'", @"\'") + "', 'final_state', '');} else {parent.ajaxConfirmModal('" + msg2.Replace("'", @"\'") + "', 'final_state', '');}", true);
                                return;
                            }
                            else
                            {
                                //Cambio stato
                                DocsPaWR.DiagrammaStato dg = DiagrammiManager.getDiagrammaById(Convert.ToString(statoSucc.ID_DIAGRAMMA));
                                DiagrammiManager.salvaModificaStato(infoDocumento.docNumber, Convert.ToString(statoSucc.SYSTEM_ID), dg, UserManager.GetInfoUser().userId, UserManager.GetInfoUser(), "", this.Page);

                                //Cancellazione storico trasmissioni
                                DiagrammiManager.deleteStoricoTrasmDiagrammi(infoDocumento.docNumber, Convert.ToString(statoCorr.SYSTEM_ID));

                                //Verifico se il nuovo stato ha delle trasmissioni automatiche
                                DocsPaWR.Stato stato = DiagrammiManager.GetStateDocument(infoDocumento.docNumber);
                                string idTipoDoc = ProfilerDocManager.getIdTemplate(infoDocumento.docNumber);
                                if (idTipoDoc != "")
                                {
                                    ArrayList modelli = new ArrayList(DiagrammiManager.isStatoTrasmAuto(UserManager.GetInfoUser().idAmministrazione, Convert.ToString(stato.SYSTEM_ID), idTipoDoc));
                                    for (int i = 0; i < modelli.Count; i++)
                                    {
                                        DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                                        if (mod.SINGLE == "1")
                                        {
                                            TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), trasmissione.infoDocumento, this);
                                        }
                                        else
                                        {
                                            for (int k = 0; k < mod.MITTENTE.Length; k++)
                                            {
                                                if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.GetSelectedRole().systemId)
                                                {
                                                    TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), trasmissione.infoDocumento, this);
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
            }
            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
            string prova = trasmissione.ruolo.idGruppo;
            string idOggetto = "";
            if (trasmissione.infoFascicolo != null && !string.IsNullOrEmpty(trasmissione.infoFascicolo.idFascicolo))
            {
                idOggetto = trasmissione.infoFascicolo.idFascicolo;
            }
            else
            {
                idOggetto = trasmissione.infoDocumento.idProfile;
            }
            bool eseguito = TrasmManager.AcquireRightsFromExtSys(idOggetto, infoUtente.idGruppo, infoUtente.idPeople, trasmissione.ruolo.codice, trasmissione.utente.idPeople);
            
            this.EnableButtons();
            this.showTransmission();
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
            //Vedo se presente la trasmissione a ruolo
            List<DocsPaWR.TrasmissioneSingola> list = new List<TrasmissioneSingola>(TrasmManager.getSelectedTransmission().trasmissioniSingole);

            //Emanuela: modifica per accettazione di trasmissioni a ruolo ed utente, seleziono per defaul prima quella di tipo ruolo per far si
            //che funzioni la gestione di accettazioni di entrambi le trasm
            List<DocsPaWR.TrasmissioneSingola> trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.RUOLO).ToList();
            trasmSing = TrasmManager.RoleTransmissionWithHistoricized(trasmSingoleUtente, infoUtente.idCorrGlobali);
            bool trasmRoleWorked = true;

            //Controllo che la trasmissione a ruolo non sia stata elaborata, altrimenti vado a selezionare la seconda trasm singola
            if (trasmSing != null)
            {
                trasmUtente = (DocsPaWR.TrasmissioneUtente)trasmSing.trasmissioneUtente.Where(e => ((DocsPaWR.Utente)e.utente).idPeople == infoUtente.idPeople).FirstOrDefault();
                if (trasmUtente != null && trasmSing.ragione.tipo == "W" && string.IsNullOrEmpty(trasmUtente.dataRifiutata) && string.IsNullOrEmpty(trasmUtente.dataAccettata))
                {
                    trasmRoleWorked = !TrasmManager.checkTrasm_UNO_TUTTI_AccettataRifiutata(trasmSing);
                }
                if (trasmUtente != null && trasmSing.ragione.tipo != "W" && TrasmManager.getIfDocOrFascIsInToDoList(infoUtente, trasmUtente.systemId))
                {
                    trasmRoleWorked = false;
                }
            }

            if (trasmSing == null || trasmRoleWorked)
            {
                // Se non è stata trovata la trasmissione come destinatario a ruolo, 
                // cerca quella con destinatario l'utente corrente

                trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.UTENTE).ToList();
                if (trasmSingoleUtente != null)
                {
                    DocsPaWR.Utente utenteCorrente = (DocsPaWR.Utente)UserManager.getCorrispondenteByIdPeople(this, infoUtente.idPeople, AddressbookTipoUtente.INTERNO);
                    TrasmissioneSingola s = trasmSingoleUtente.Where(e => ((DocsPaWR.Utente)e.corrispondenteInterno).idPeople == infoUtente.idPeople).FirstOrDefault();
                    if (s != null)
                        trasmSing = s;
                }
            }

            if (trasmSing != null)
            {
                Fascicolo fascicolo = null;
                //Nel caso di ragione con classificazione obbligatoria verifico che sia stato inserito il fascicolo
                if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.ACCETTAZIONE && trasmSing.ragione.fascicolazioneObbligatoria)
                {
                    if (string.IsNullOrEmpty(this.TxtCodeProject.Text))
                    {
                        //RegisterStartupScript("alert", "<script>alert('La fascicolazione rapida è obbligatoria !');</script>");
                        errore = "WarningDocumentRequestProject";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + errore.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + errore.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return false;
                    }
                    DocsPaWR.Fascicolo fasc = this.Project;
                    if (fasc != null && fasc.systemID != null && this.EnableBlockClassification)
                    {
                        if (fasc.tipo.Equals("G") && fasc.isFascConsentita != null && fasc.isFascConsentita == "0")
                        {
                            string msgDesc = fasc.isFascicolazioneConsentita ? "WarningDocumentNoDocumentInsert" : "WarningDocumentNoDocumentInsertClassification";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return false;
                        }
                        if (fasc.tipo.Equals("P") && !fasc.isFascicolazioneConsentita)
                        {
                            string msgDesc = "WarningDocumentNoDocumentInsertFolder";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return false;
                        }
                    }
                    fascicolo = fasc;
                }
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
                if (!TrasmManager.executeAccRif(this, trasmUtente, trasmissione.systemId, fascicolo, out errore))
                {
                    rtn = false;
                    trasmUtente.dataRifiutata = null;
                    trasmUtente.dataAccettata = null;

                    string msg = "ErrorCustom";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '', '" + errore.Replace("'", @"\'") + "');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '', '" + errore.Replace("'", @"\'") + "');}", true);
                }
                else 
                {
                    if (infoUtente.delegato != null)
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

                    if (trasmSing.tipoDest == TrasmissioneTipoDestinatario.RUOLO) //Se ho accettato una trasm a ruolo vado ad aggiornare eventuali trasm per competenza ad utente
                    {
                        List<TrasmissioneSingola> trasmSingoleUtente1 = listaTrasmSing.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.UTENTE && e.ragione.systemId.Equals(trasmSing.ragione.systemId)).ToList();
                        if (trasmSingoleUtente != null)
                        {
                            DocsPaWR.Utente utenteCorrente = (DocsPaWR.Utente)UserManager.getCorrispondenteByIdPeople(this, infoUtente.idPeople, AddressbookTipoUtente.INTERNO);
                            TrasmissioneSingola s = trasmSingoleUtente1.Where(e => ((DocsPaWR.Utente)e.corrispondenteInterno).idPeople == infoUtente.idPeople).FirstOrDefault();
                            if (s != null)
                            {
                                TrasmissioneUtente trasmUtente2 = s.trasmissioneUtente.Where(e => ((DocsPaWR.Utente)e.utente).idPeople == infoUtente.idPeople).FirstOrDefault();
                                trasmUtente2.dataRifiutata = trasmUtente.dataRifiutata;
                                trasmUtente2.dataAccettata = trasmUtente.dataAccettata;
                                trasmUtente2.cha_accettata_delegato = trasmUtente.cha_accettata_delegato;
                                trasmUtente2.cha_rifiutata_delegato = trasmUtente.cha_rifiutata_delegato;
                            }
                        }
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

            DocsPaWR.InfoDocumento infoDocumento = TrasmManager.getSelectedTransmission().infoDocumento;
            DocumentManager.addWorkArea(this, infoDocumento);

            // re-get document to avoid rights checks
            //SchedaDocumento tempDoc = DocumentManager.getSelectedRecord();
            //DocumentManager.setSelectedRecord(DocumentManager.getDocumentDetails(this, tempDoc.docNumber, tempDoc.docNumber));

            //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
            if (DocumentManager.CheckRevocationAcl())
            {
                DocumentManager.setSelectedRecord(null);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}", true);
                return;
            }
        }

        protected void TransmissionsBtnAcceptADLR_Click(object sender, System.EventArgs e)
        {
            this.acceptTransmission();

            DocsPaWR.InfoDocumento infoDocumento = TrasmManager.getSelectedTransmission().infoDocumento;
            DocumentManager.addAreaLavoroRole(this, DocumentManager.getSelectedRecord());

            // re-get document to avoid rights checks
            //SchedaDocumento tempDoc = DocumentManager.getSelectedRecord();
            //DocumentManager.setSelectedRecord(DocumentManager.getDocumentDetails(this, tempDoc.docNumber, tempDoc.docNumber));

            //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
            if (DocumentManager.CheckRevocationAcl())
            {
                DocumentManager.setSelectedRecord(null);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}", true);
                return;
            }
        }

        protected void TransmissionsBtnReject_Click(object sender, System.EventArgs e)
        {
            logger.Info("BEGIN");

            bool result = accettaRifiuta(DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO);

            if (result)
            {
                //Cancello i riferimenti alle tramissioni da controllare per quanto riguarda
                //il passaggio di stato automatico
                if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                {
                    DocsPaWR.InfoDocumento infoDocumento = TrasmManager.getSelectedTransmission().infoDocumento;
                    string docNumber = infoDocumento.docNumber;
                    DocsPaWR.Stato stato = DiagrammiManager.GetStateDocument(docNumber);
                    if (stato != null)
                    {
                        string idStato = Convert.ToString(stato.SYSTEM_ID);
                        DiagrammiManager.deleteStoricoTrasmDiagrammi(docNumber, idStato);
                    }
                }

                this.EnableButtons();
                this.showTransmission();

                SchedaDocumento newDoc = DocumentManager.getDocumentDetails(this.Page, TrasmManager.getSelectedTransmission().infoDocumento.idProfile, TrasmManager.getSelectedTransmission().infoDocumento.docNumber);

                if (newDoc == null)
                {
                    DocumentManager.setSelectedRecord(null);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "disallow", "disallowOp('Content2');", true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}", true);
                }
            }

            //logger.Info("END");
        }

        protected void TransmissionsBtnView_Click(object sender, System.EventArgs e)
        {
            DocsPaWR.InfoDocumento infoDocumento = TrasmManager.getSelectedTransmission().infoDocumento;
            TrasmManager.setdatavistaSP_TV(UserManager.GetInfoUser(), infoDocumento.docNumber, "D", infoDocumento.idRegistro, TrasmManager.getSelectedTransmission().systemId);

            // re-get document to avoid rights checks
            //SchedaDocumento tempDoc = DocumentManager.getSelectedRecord();
            //DocumentManager.setSelectedRecord(DocumentManager.getDocumentDetails(this, tempDoc.docNumber, tempDoc.docNumber));

            //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
            if (DocumentManager.CheckRevocationAcl())
            {
                DocumentManager.setSelectedRecord(null);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}", true);
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
            DocsPaWR.InfoDocumento infoDocumento = TrasmManager.getSelectedTransmission().infoDocumento;
            TrasmManager.setdatavistaSP_TV(UserManager.GetInfoUser(), infoDocumento.docNumber, "D", infoDocumento.idRegistro, TrasmManager.getSelectedTransmission().systemId);
            DocumentManager.addWorkArea(this.Page, infoDocumento);

            // re-get document to avoid rights checks
            //SchedaDocumento tempDoc = DocumentManager.getSelectedRecord();
            //DocumentManager.setSelectedRecord(DocumentManager.getDocumentDetails(this, tempDoc.docNumber, tempDoc.docNumber));

            //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
            if (DocumentManager.CheckRevocationAcl())
            {
                DocumentManager.setSelectedRecord(null);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}", true);
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
            DocsPaWR.InfoDocumento infoDocumento = TrasmManager.getSelectedTransmission().infoDocumento;
            TrasmManager.setdatavistaSP_TV(UserManager.GetInfoUser(), infoDocumento.docNumber, "D", infoDocumento.idRegistro, TrasmManager.getSelectedTransmission().systemId);
            DocumentManager.addWorkAreaRole(this.Page, infoDocumento);

            // re-get document to avoid rights checks
            //SchedaDocumento tempDoc = DocumentManager.getSelectedRecord();
            //DocumentManager.setSelectedRecord(DocumentManager.getDocumentDetails(this, tempDoc.docNumber, tempDoc.docNumber));

            //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
            if (DocumentManager.CheckRevocationAcl())
            {
                DocumentManager.setSelectedRecord(null);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}", true);
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
            this.TransmissionsBtnAcceptLF.Visible = false;
            this.TransmissionsBtnReject.Visible = false;

            this.imgSepFooter.Visible = false;

            this.TransmissionsBtnView.Visible = false;
            this.TransmissionsBtnViewADL.Visible = false;
            this.TransmissionsBtnViewADLR.Visible = false;
            this.TransmissionsBtnAcquireRights.Visible = false;
            

            this.upPnlNoteAccRif.Visible = false;
            this.upPnlNoteAccRif.Update();

            this.pnlFascRequired.Visible = false;
            this.upPnlFascRequired.Update();

            // Gabriele Melini 08-01-2015
            // INC000000520003
            // Controllo su trasmissioniSingole = null
            if (trasm.trasmissioniSingole != null)
                listTrasmSing = trasm.trasmissioniSingole;
            else
                listTrasmSing = null;

            if (listTrasmSing != null && listTrasmSing.Length > 0 && !string.IsNullOrEmpty(trasm.dataInvio))
            {
                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                //Vedo se presente la trasmissione a ruolo
                List<DocsPaWR.TrasmissioneSingola> list = new List<TrasmissioneSingola>(TrasmManager.getSelectedTransmission().trasmissioniSingole);

                //Emanuela: modifica per accettazione di trasmissioni a ruolo ed utente, seleziono per defaul prima quella di tipo ruolo per far si
                //che funzioni la gestione di accettazioni di entrambi le trasm
                List<DocsPaWR.TrasmissioneSingola> trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.RUOLO).ToList();
                trasmSing = TrasmManager.RoleTransmissionWithHistoricized(trasmSingoleUtente, infoUtente.idCorrGlobali);
                bool trasmRoleWorked = true;

                if (trasmSing != null)
                {
                    trasmUtente = (DocsPaWR.TrasmissioneUtente)trasmSing.trasmissioneUtente.Where(e => ((DocsPaWR.Utente)e.utente).idPeople == infoUtente.idPeople).FirstOrDefault();
                    if (trasmUtente != null && trasmSing.ragione.tipo == "W" && string.IsNullOrEmpty(trasmUtente.dataRifiutata) && string.IsNullOrEmpty(trasmUtente.dataAccettata))
                    {
                        trasmRoleWorked = !TrasmManager.checkTrasm_UNO_TUTTI_AccettataRifiutata(trasmSing);
                    }
                    if (trasmUtente != null && trasmSing.ragione.tipo != "W" && TrasmManager.getIfDocOrFascIsInToDoList(infoUtente, trasmUtente.systemId))
                    {
                        trasmRoleWorked = false;
                    }
                }

                if (trasmSing == null || trasmRoleWorked)
                {
                    // Se non è stata trovata la trasmissione come destinatario a ruolo, 
                    // cerca quella con destinatario l'utente corrente

                    trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.UTENTE).ToList();
                    if (trasmSingoleUtente != null)
                    {
                        DocsPaWR.Utente utenteCorrente = (DocsPaWR.Utente)UserManager.getCorrispondenteByIdPeople(this, infoUtente.idPeople, AddressbookTipoUtente.INTERNO);
                        TrasmissioneSingola s = trasmSingoleUtente.Where(e => ((DocsPaWR.Utente)e.corrispondenteInterno).idPeople == infoUtente.idPeople).FirstOrDefault();
                        if (s != null)
                            trasmSing = s;
                    }
                }

                if (trasmSing != null)
                {
                    DocsPaWR.SistemaEsterno sysext = TrasmManager.getSistemaEsterno(infoUtente.idAmministrazione, trasm.ruolo.codice);
                    if (sysext == null)
                    {
                        trasmUtente = (DocsPaWR.TrasmissioneUtente)trasmSing.trasmissioneUtente.Where(e => ((DocsPaWR.Utente)e.utente).idPeople == infoUtente.idPeople).FirstOrDefault();
                        if (trasmUtente != null && trasmSing.ragione.tipo == "W" && string.IsNullOrEmpty(trasmUtente.dataRifiutata) && string.IsNullOrEmpty(trasmUtente.dataAccettata))
                        {
                            bool value = TrasmManager.checkTrasm_UNO_TUTTI_AccettataRifiutata(trasmSing);
                            value = value && isToUserOrRole;
                            this.imgSepFooter.Visible = value;
                            this.TransmissionsBtnAccept.Visible = value;
                            this.TransmissionsBtnAcceptADL.Visible = value;
                            this.TransmissionsBtnReject.Visible = value;
                            if (trasm.infoDocumento != null && this.EnabledLibroFirma && LibroFirmaManager.CanInsertInLibroFirma(trasmSing, trasm.infoDocumento.docNumber))
                            {
                                this.TransmissionsBtnAcceptLF.Visible = value;
                            }

                            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
                            {
                                this.TransmissionsBtnAcceptADLR.Visible = value;
                            }

                            if (DocumentManager.isDocInADLRole(trasm.infoDocumento.idProfile, this) == 1)
                                this.TransmissionsBtnAcceptADL.Visible = false;

                            this.upPnlNoteAccRif.Visible = value;
                            this.upPnlNoteAccRif.Update();

                            if (trasmSing.ragione.fascicolazioneObbligatoria)
                            {
                                this.pnlFascRequired.Visible = true;
                            }
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

                                // PALUMBO: condizione valida per APSS
                                if (trasmUtente != null && SimplifiedInteroperabilityManager.IsDocumentReceivedWithIS(trasm.infoDocumento.idProfile) &&
                                    ConfigurationManager.AppSettings[WebConfigKeys.SET_DATA_VISTA_GRD.ToString()] == "1")
                                {
                                    if (TrasmManager.getIfDocOrFascIsInToDoList(infoUtente, trasmUtente.systemId))
                                        isInToDoList = true;
                                }

                                // Se il documento è stato ricevuto per interoperabilità semplificata, il tasto visto deve essere
                                // visualizzato solo nel caso in cui il documento è già stato protocollato
                                // Il pulsante deve essere visualizzato indipendentemente dal fatto che sia attivo o meno il
                                // set_grd_datavista
                                // PALUMBO: inserita variabile isInToDoList per eliminare visualizzazione tasto Visto in caso 
                                // di trasmissione per IS e già vista incident: INC000000103503 
                                if (
                                    trasm.tipoOggetto == TrasmissioneTipoOggetto.DOCUMENTO
                                    && SimplifiedInteroperabilityManager.IsDocumentReceivedWithIS(trasm.infoDocumento.idProfile)
                                    && isInToDoList)
                                {
                                    bool value = !String.IsNullOrEmpty(trasm.infoDocumento.segnatura) || (String.IsNullOrEmpty(trasm.infoDocumento.segnatura) && trasm.infoDocumento.tipoProto == "G");
                                    this.imgSepFooter.Visible = value;
                                    this.TransmissionsBtnView.Visible = value;
                                    this.TransmissionsBtnViewADL.Visible = value;
                                    this.TransmissionsBtnViewADLR.Visible = value;
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
                        trasmUtente = (DocsPaWR.TrasmissioneUtente)trasmSing.trasmissioneUtente.Where(e => ((DocsPaWR.Utente)e.utente).idPeople == infoUtente.idPeople).FirstOrDefault();
                        //if (trasmUtente != null && trasmSing.ragione.tipo == "W" && string.IsNullOrEmpty(trasmUtente.dataRifiutata) && string.IsNullOrEmpty(trasmUtente.dataAccettata))
                        if (trasmUtente != null && string.IsNullOrEmpty(trasmUtente.dataRifiutata) && string.IsNullOrEmpty(trasmUtente.dataAccettata))
                        {
                            trasmSing.tipoTrasm = "S";
                            bool value = TrasmManager.checkTrasm_UNO_TUTTI_AccettataRifiutata(trasmSing);
                            value = value && isToUserOrRole;
                            this.TransmissionsBtnAcquireRights.Visible = value;
                            this.upPnlNoteAccRif.Visible = value;
                            this.upPnlNoteAccRif.Update();
                        }
                        else
                        {
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

            SchedaDocumento doc = DocumentManager.getSelectedRecord();
            Trasmissione trasm = TrasmManager.getSelectedTransmission();

            if ((doc != null && doc.systemId != null) && !this.DocumentoInCestino && !this.DocumentoInArchivio)
            {
                this.TransmissionsBtnAdd.Enabled = true; // If the document is not in the basket insertion is always allowed
                this.DdlTransmissionsModel.Enabled = true;

                if (this.grdList.Rows.Count > 0)
                {
                    this.TransmissionsBtnPrint.Visible = true; // print is enabled always if there are transmissions

                    // checks on transm selected
                    if (this.SelectedRowIndex >= 0)
                    {
                        if ((trasm != null && trasm.systemId != null) && (trasm.dataInvio == null || trasm.dataInvio == "") && trasm.ruolo!= null && trasm.ruolo.idGruppo != null && trasm.ruolo.idGruppo.Equals(RoleManager.GetRoleInSession().idGruppo))
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

                //disabilitazione dei bottoni in base all'autorizzazione di HM sul documento
                if (!UserManager.EnableNewTransmission(null))
                {
                    // Bottoni che devono essere disabilitati in caso di diritti di sola lettura
                    this.DisableAllButtons(false);
                }
            }
            this.upPnlTransmissionsModel.Update();
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
            this.DdlTransmissionsModel.Enabled = false;

            if (disablePrintToo)
            {
                this.TransmissionsBtnPrint.Visible = false;
                this.imgSepFooter.Visible = false;
                this.TransmissionsBtnAccept.Visible = false;
                this.TransmissionsBtnAcceptADL.Visible = false;
                this.TransmissionsBtnAcceptADLR.Visible = false;
                this.TransmissionsBtnAcceptLF.Visible = false;
                this.TransmissionsBtnReject.Visible = false;
                this.TransmissionsBtnView.Visible = false;
                this.TransmissionsBtnViewADL.Visible = false;
                this.TransmissionsBtnViewADLR.Visible = false;
                this.TransmissionsBtnAcquireRights.Visible = false;
            }
        }

        protected void TransmissionsBtnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Transmission/dataentry_document.aspx?t=" + Request.QueryString["t"] + "&idProfile=" + Request.QueryString["idProfile"]);
        }

        protected void TransmissionsBtnModify_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Transmission/dataentry_document.aspx?t=" + Request.QueryString["t"] + "&idProfile=" + Request.QueryString["idProfile"] + "&idTransmission=" + TrasmManager.getSelectedTransmission().systemId);
        }

        private DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPaWR.Trasmissione trasmissione, DocsPaWR.Corrispondente corr, DocsPaWR.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, DocsPaWR.MittDest mittDest)
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
            trasmissioneSingola.hideDocumentPreviousVersions = mittDest.NASCONDI_VERSIONI_PRECEDENTI;

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
                qca.queryCorrispondente = new AddressbookQueryCorrispondente();
                qca.queryCorrispondente.fineValidita = true;

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
            catch
            {
                retValue = false;
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
                this.chkCreatoreExtendHistoricized.Checked = false;
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
                this.ControlAbortDocument();
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

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        #region FASCICOLAZIONE OBBLIGATORIA

        protected void btnSearchProject_Click(object sender, EventArgs e)
        {
            if (this.ReturnValue.Split('#').Length > 1)
            {
                this.TxtCodeProject.Text = this.ReturnValue.Split('#').First();
                this.TxtDescriptionProject.Text = this.ReturnValue.Split('#').Last();
                this.UpPnlProject.Update();
                TxtCodeProject_OnTextChanged(new object(), new EventArgs());
            }
            else
                //Laura 19 Marzo
                if (this.ReturnValue.Contains("//"))
                {
                    this.TxtCodeProject.Text = this.ReturnValue;
                    this.TxtDescriptionProject.Text = "";
                    this.UpPnlProject.Update();
                    TxtCodeProject_OnTextChanged(new object(), new EventArgs());
                }
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SearchProject','');", true);
        }

        protected void btnTitolarioPostback_Click(object sender, EventArgs e)
        {
            if (this.ReturnValue.Split('#').Length > 1)
            {
                this.TxtCodeProject.Text = this.ReturnValue.Split('#').First();
                this.TxtDescriptionProject.Text = this.ReturnValue.Split('#').Last();
                this.UpPnlProject.Update();
                TxtCodeProject_OnTextChanged(new object(), new EventArgs());
            }
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenTitolario','');", true);
        }

        protected void DocumentImgSearchProjects_Click(object sender, EventArgs e)
        {
            RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "upPnlFascRequired", "ajaxModalPopupSearchProject();", true);
        }

        protected void DocumentImgOpenTitolario_Click(object sender, EventArgs e)
        {
            RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "upPnlFascRequired", "ajaxModalPopupOpenTitolario();", true);
        }

        protected void ImgAddProjects_Click(object sender, EventArgs e)
        {
            try
            {
                #region navigation
                SchedaDocumento doc = DocumentManager.getSelectedRecord();
                List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                actualPage.IdObject = doc.docNumber;
                actualPage.OriginalObjectId = doc.docNumber;
                actualPage.ViewResult = true;
                actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT_TRANSMISSIONS.ToString(), string.Empty);
                actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT_TRANSMISSIONS.ToString(), true, this.Page);
                actualPage.CodePage = Navigation.NavigationUtils.NamePage.DOCUMENT_TRANSMISSIONS.ToString();

                actualPage.Page = "TRANSMISSIONS.ASPX";
                navigationList.Add(actualPage);
                Navigation.NavigationUtils.SetNavigationList(navigationList);

                #endregion
                Response.Redirect(this.ResolveUrl("~/Project/Project.aspx?t=c"), false);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtCodeProject_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                Registro registro = RoleManager.GetRoleInSession().registri[0];
                ProjectManager.removeFascicoloSelezionatoFascRapida(this);

                if (!string.IsNullOrEmpty(this.TxtCodeProject.Text))
                {
                    SchedaDocumento documentoSelezionato = UIManager.DocumentManager.getSelectedRecord();

                    if (registro == null)
                    {
                        this.SearchProjectNoRegistro();
                    }
                    else
                    {
                        this.SearchProjectRegistro();
                    }
                    this.cercaClassificazioneDaCodice(this.TxtCodeProject.Text);
                }
                else
                {
                    this.TxtCodeProject.Text = string.Empty;
                    this.TxtDescriptionProject.Text = string.Empty;
                    this.IdProject.Value = string.Empty;
                    this.Project = null;
                    //Laura 25 Marzo
                    ProjectManager.setProjectInSessionForRicFasc(null);
                    ProjectManager.setProjectInSessionForRicFasc(String.Empty);
                }

                this.UpPnlProject.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchProjectNoRegistro()
        {
            Registro registro = RoleManager.GetRoleInSession().registri[0];
            this.TxtDescriptionProject.Text = string.Empty;
            if (string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                this.TxtDescriptionProject.Text = string.Empty;
                return;
            }
            //su DocProfilo devo cercare senza condizione sul registro.
            //Basta che il fascicolo sia visibile al ruolo loggato

            if (this.TxtCodeProject.Text.IndexOf("//") > -1)
            {

                string codice = string.Empty;
                string descrizione = string.Empty;

                DocsPaWR.Fascicolo SottoFascicolo = getFolder(null, ref codice, ref descrizione);
                if (SottoFascicolo != null)
                {
                    if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
                    {
                        TxtDescriptionProject.Text = descrizione;
                        this.TxtDescriptionProject.Focus();
                        TxtCodeProject.Text = codice;
                        this.Project = SottoFascicolo;
                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, SottoFascicolo.idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                        // DocsPaWR.Fascicolo fascForRicFasc = ProjectManager.getFascicoloById(gerClassifica[gerClassifica.Length - 1].systemId);
                        ProjectManager.setProjectInSessionForRicFasc(gerClassifica[gerClassifica.Length - 1].codice);
                        ProjectManager.setFascicoloSelezionatoFascRapida(this, SottoFascicolo);

                    }
                    else
                    {
                        //string msg = @"Attenzione, sottofascicolo non presente.";
                        string msg = "WarningDocumentSubFileNoFound";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                        this.TxtDescriptionProject.Text = string.Empty;
                        this.TxtCodeProject.Text = string.Empty;

                    }
                }
                else
                {
                    //string msg = @"Attenzione, sottofascicolo non presente.";
                    string msg = "WarningDocumentSubFileNoFound";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    this.TxtDescriptionProject.Text = string.Empty;
                    this.TxtCodeProject.Text = string.Empty;
                }
            }
            else
            {

                DocsPaWR.Fascicolo[] listaFasc = getFascicolo(registro);
                string codClassifica = string.Empty;
                if (listaFasc != null)
                {
                    if (listaFasc.Length > 0)
                    {
                        //caso 1: al codice digitato corrisponde un solo fascicolo
                        if (listaFasc.Length == 1)
                        {
                            this.TxtDescriptionProject.Text = listaFasc[0].descrizione;
                            this.TxtDescriptionProject.Focus();
                            //metto il fascicolo in sessione
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                codClassifica = listaFasc[0].codice;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);

                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }
                            this.Project = listaFasc[0];
                            ProjectManager.setProjectInSessionForRicFasc(codClassifica);
                            ProjectManager.setFascicoloSelezionatoFascRapida(this, listaFasc[0]);
                            //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                            //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'N');");
                        }
                        else
                        {
                            codClassifica = this.TxtCodeProject.Text;
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                //codClassifica = codClassifica;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }
                            //Page.RegisterStartupScript("openListaFasc","<SCRIPT>ApriSceltaFascicolo();</SCRIPT>");
                            //Session.Add("hasRegistriNodi",hasRegistriNodi);

                            //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                            //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'Y');");

                            //Da Fare
                            //RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli2('" + codClassifica + "', 'Y')</script>");                            

                            return;
                        }
                    }
                    else
                    {
                        //caso 0: al codice digitato non corrisponde alcun fascicolo
                        if (listaFasc.Length == 0)
                        {
                            //Provo il caso in cui il fascicolo è chiuso
                            Fascicolo chiusoFasc = ProjectManager.getFascicoloDaCodice(this.Page, this.TxtCodeProject.Text);
                            if (chiusoFasc != null && !string.IsNullOrEmpty(chiusoFasc.stato) && chiusoFasc.stato.Equals("C"))
                            {
                                //string msg = @"Attenzione, il fascicolo scelto è chiuso. Pertanto il documento non può essere inserito nel fascicolo selezionato.";
                                string msg = "WarningDocumentFileNoOpen";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            else
                            {
                                //string msg = @"Attenzione, codice fascicolo non presente.";
                                string msg = "WarningDocumentCodFileNoFound";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                            }

                            this.TxtDescriptionProject.Text = string.Empty;
                            this.TxtCodeProject.Text = string.Empty;
                        }
                        //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                        //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', '');");
                    }
                }
            }
        }

        protected void SearchProjectRegistro()
        {
            Registro registro = RoleManager.GetRoleInSession().registri[0];
            this.TxtDescriptionProject.Text = string.Empty;
            string codClassifica = string.Empty;

            if (string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                this.TxtDescriptionProject.Text = string.Empty;
                this.Project = null;
                //Laura 25 Marzo
                ProjectManager.setProjectInSessionForRicFasc(null);
                ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                return;
            }

            //FASCICOLAZIONE IN SOTTOFASCICOLI

            if (this.TxtCodeProject.Text.IndexOf("//") > -1)
            {
                #region FASCICOLAZIONE IN SOTTOFASCICOLI
                string codice = string.Empty;
                string descrizione = string.Empty;
                DocsPaWR.Fascicolo SottoFascicolo = getFolder(registro, ref codice, ref descrizione);
                if (SottoFascicolo != null)
                {

                    if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
                    {
                        TxtDescriptionProject.Text = descrizione;
                        this.TxtDescriptionProject.Focus();
                        TxtCodeProject.Text = codice;
                        this.Project = SottoFascicolo;
                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, SottoFascicolo.idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                        ProjectManager.setProjectInSessionForRicFasc(gerClassifica[gerClassifica.Length - 1].codice);
                        ProjectManager.setFascicoloSelezionatoFascRapida(this, SottoFascicolo);
                    }
                    else
                    {

                        //string msg = @"Attenzione, sottofascicolo non presente.";
                        string msg = "WarningDocumentSubFileNoFound";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        this.TxtDescriptionProject.Text = string.Empty;
                        this.TxtCodeProject.Text = string.Empty;
                        this.Project = null;
                        ProjectManager.setProjectInSessionForRicFasc(null);
                        ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                    }
                }
                else
                {
                    Session["validCodeFasc"] = "false";

                    //string msg = @"Attenzione, sottofascicolo non presente.";
                    string msg = "WarningDocumentSubFileNoFound";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    this.TxtDescriptionProject.Text = string.Empty;
                    this.TxtCodeProject.Text = string.Empty;
                    this.Project = null;
                    ProjectManager.setProjectInSessionForRicFasc(null);
                    ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                }

                #endregion
            }
            else
            {
                DocsPaWR.Fascicolo[] listaFasc = getFascicoli(registro);

                if (listaFasc != null)
                {
                    if (listaFasc.Length > 0)
                    {
                        //caso 1: al codice digitato corrisponde un solo fascicolo
                        if (listaFasc.Length == 1)
                        {
                            this.Project = listaFasc[0];
                            this.TxtDescriptionProject.Text = listaFasc[0].descrizione;
                            this.TxtDescriptionProject.Focus();
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                codClassifica = listaFasc[0].codice;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }
                            ProjectManager.setProjectInSessionForRicFasc(codClassifica);
                            ProjectManager.setFascicoloSelezionatoFascRapida(this, listaFasc[0]);
                        }
                        else
                        {
                            //caso 2: al codice digitato corrispondono piu fascicoli
                            codClassifica = this.TxtCodeProject.Text;
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                //codClassifica = codClassifica;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }

                            ////Da Fare
                            //RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codClassifica + "', 'Y')</script>");
                            return;
                        }
                    }
                    else
                    {
                        //caso 0: al codice digitato non corrisponde alcun fascicolo
                        if (listaFasc.Length == 0)
                        {
                            //Provo il caso in cui il fascicolo è chiuso
                            Fascicolo chiusoFasc = ProjectManager.getFascicoloDaCodice(this.Page, this.TxtCodeProject.Text);
                            if (chiusoFasc != null && !string.IsNullOrEmpty(chiusoFasc.stato) && chiusoFasc.stato.Equals("C"))
                            {
                                //string msg = @"Attenzione, il fascicolo scelto è chiuso. Pertanto il documento non può essere inserito nel fascicolo selezionato.";
                                string msg = "WarningDocumentFileNoOpen";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            else
                            {
                                //string msg = @"Attenzione, codice fascicolo non presente.";
                                string msg = "WarningDocumentCodFileNoFound";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            this.TxtDescriptionProject.Text = string.Empty;
                            this.TxtCodeProject.Text = string.Empty;
                            this.Project = null;
                            ProjectManager.setProjectInSessionForRicFasc(null);
                            ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                        }
                    }
                }
            }
        }

        private DocsPaWR.Fascicolo[] getFascicolo(DocsPaWR.Registro registro)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            if (!string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                string codiceFascicolo = TxtCodeProject.Text;
                listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "I");
            }
            if (listaFasc != null)
            {
                return listaFasc;
            }
            else
            {
                return null;
            }
        }

        private DocsPaWR.Fascicolo[] getFascicoli(DocsPaWR.Registro registro)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            if (!this.TxtCodeProject.Text.Equals(""))
            {
                string codiceFascicolo = TxtCodeProject.Text;
                listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "I");
            }
            if (listaFasc != null)
            {
                return listaFasc;
            }
            else
            {
                return null;
            }
        }

        private DocsPaWR.Fascicolo getFolder(DocsPaWR.Registro registro, ref string codice, ref string descrizione)
        {
            DocsPaWR.Folder[] listaFolder = null;
            DocsPaWR.Fascicolo fasc = null;
            string separatore = "//";
            int posSep = this.TxtCodeProject.Text.IndexOf("//");
            if (this.TxtCodeProject.Text != string.Empty && posSep > -1)
            {

                string codiceFascicolo = TxtCodeProject.Text.Substring(0, posSep);
                string descrFolder = TxtCodeProject.Text.Substring(posSep + separatore.Length);

                listaFolder = ProjectManager.getListaFolderDaCodiceFascicolo(this, codiceFascicolo, descrFolder, registro);
                if (listaFolder != null && listaFolder.Length > 0)
                {
                    //calcolo fascicolazionerapida
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    fasc = ProjectManager.getFascicoloById(listaFolder[0].idFascicolo, infoUtente);

                    if (fasc != null)
                    {
                        //folder selezionato è l'ultimo
                        fasc.folderSelezionato = listaFolder[listaFolder.Length - 1];
                    }
                    codice = fasc.codice + separatore;
                    descrizione = fasc.descrizione + separatore;
                    for (int i = 0; i < listaFolder.Length; i++)
                    {
                        codice += listaFolder[i].descrizione + "/";
                        descrizione += listaFolder[i].descrizione + "/";
                    }
                    codice = codice.Substring(0, codice.Length - 1);
                    descrizione = descrizione.Substring(0, descrizione.Length - 1);

                }
            }
            if (fasc != null)
            {

                return fasc;

            }
            else
            {
                return null;
            }
        }

        private void FascicolazioneRapida(string idProfile)
        {
            DocsPaWR.Fascicolo fasc;
            string returnMsg = string.Empty;
            string retMes2 = string.Empty;
            fasc = this.Project;
            if (fasc != null)
            {
                if (fasc.stato == "C")
                {
                    returnMsg += "WarningDocumentProjectClosed";
                }
                else
                {
                    if (fasc != null && fasc.systemID != null && this.EnableBlockClassification)
                    {

                        if (fasc.tipo.Equals("G") && fasc.isFascConsentita != null && fasc.isFascConsentita == "0")
                        {
                            returnMsg = fasc.isFascicolazioneConsentita ? "WarningDocumentNoDocumentInsert" : "WarningDocumentNoDocumentInsertClassification";
                        }
                        if (fasc.tipo.Equals("P") && !fasc.isFascicolazioneConsentita)
                        {
                            returnMsg = "WarningDocumentNoDocumentInsertFolder";
                        }
                    }
                    if (!string.IsNullOrEmpty(returnMsg))
                    {
                        int risultato = DocumentManager.fascicolaRapida(idProfile, idProfile, string.Empty, fasc, true);
                        switch (risultato)
                        {
                            case 1:
                                //returnMsg += "WarningDocumentNoClassificated";
                                returnMsg += "WarningDocumentDocumentFound";
                                break;
                            case 2:
                                returnMsg += "WarningDocumentNoClassificatedSelect";
                                retMes2 = fasc.descrizione;
                                break;
                        }

                    }
                    if (!string.IsNullOrEmpty(returnMsg))
                    {
                        if (!string.IsNullOrEmpty(retMes2))
                        {
                            string language = UIManager.UserManager.GetUserLanguage();
                            returnMsg = Utils.Languages.GetMessageFromCode("WarningDocumentNoClassificatedSelect", language);
                            returnMsg = returnMsg + " " + retMes2;
                            string msgDesc = "WarningDocumentCustom";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(returnMsg) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(returnMsg) + "');};", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + returnMsg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + returnMsg.Replace("'", @"\'") + "', 'warning', '');};", true);
                        }
                    }
                }

            }
        }

        private bool cercaClassificazioneDaCodice(string codClassificazione)
        {
            bool res = false;
            DocsPaWR.Fascicolo[] listaFasc;
            if (!string.IsNullOrEmpty(codClassificazione))
            {
                listaFasc = this.getFascicolo(RoleManager.GetRoleInSession().registri[0], codClassificazione);

                DocsPaWR.FascicolazioneClassificazione[] FascClass = ProjectManager.fascicolazioneGetTitolario2(this, codClassificazione, false, this.getIdTitolario(codClassificazione, listaFasc));
                if (FascClass != null && FascClass.Length != 0)
                {
                    HttpContext.Current.Session["classification"] = FascClass[0];
                }
                else
                {
                    HttpContext.Current.Session["classification"] = null;
                }
            }

            return res;
        }

        private DocsPaWR.Fascicolo[] getFascicolo(DocsPaWR.Registro registro, string codClassificazione)
        {
            DocsPaWR.Fascicolo[] listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codClassificazione, registro, "I");
            return listaFasc;
        }

        private string getIdTitolario(string codClassificazione, DocsPaWR.Fascicolo[] listaFasc)
        {
            if (listaFasc != null && listaFasc.Length > 0)
            {
                DocsPaWR.Fascicolo fasc = (DocsPaWR.Fascicolo)listaFasc[0];
                return fasc.idTitolario;
            }
            else
            {
                return null;
            }
        }
           

        protected void SetAjaxDescriptionProject()
        {
            Ruolo ruolo = RoleManager.GetRoleInSession();
            string dataUser = ruolo.idGruppo;
            dataUser = dataUser + "-" + ruolo.registri[0].systemId;
            if (UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione) != null)
            {
                RapidSenderDescriptionProject.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione).ID + "-" + UIManager.UserManager.GetUserInSession().idPeople + "-" + UIManager.UserManager.GetUserInSession().systemId;
            }
        }

        private DocsPaWR.Fascicolo Project
        {
            get
            {
                Fascicolo result = null;
                if (HttpContext.Current.Session["project"] != null)
                {
                    result = HttpContext.Current.Session["project"] as Fascicolo;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["project"] = value;
            }
        }

        private string ReturnValue
        {
            get
            {
                //Laura 19 Marzo
                if ((HttpContext.Current.Session["ReturnValuePopup"]) != null)
                    return HttpContext.Current.Session["ReturnValuePopup"].ToString();
                else
                    return string.Empty;
            }
        }

        private bool RapidClassificationRequiredByTypeDoc
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["RapidClassificationRequiredByTypeDoc"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["RapidClassificationRequiredByTypeDoc"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["RapidClassificationRequiredByTypeDoc"] = value;
            }

        }

        private bool EnableBlockClassification
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableBlockClassification"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableBlockClassification"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableBlockClassification"] = value;
            }
        }
        #endregion
    }
}
