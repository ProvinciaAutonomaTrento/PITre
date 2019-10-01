using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDatalLibrary;

namespace NttDataWA.Management
{
    public partial class InstaceDetails : System.Web.UI.Page
    {

        #region Properties

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

        public DocsPaWR.Corrispondente Richiedente
        {
            get
            {
                DocsPaWR.Corrispondente result = null;
                if (HttpContext.Current.Session["richiedente"] != null)
                {
                    result = HttpContext.Current.Session["richiedente"] as DocsPaWR.Corrispondente;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["richiedente"] = value;
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

        private bool OpenAddressBookFromFilter
        {
            get
            {
                if (HttpContext.Current.Session["OpenAddressBookFromFilter"] != null)
                    return (Boolean)HttpContext.Current.Session["OpenAddressBookFromFilter"];
                else
                    return false;
            }
            set
            {
                HttpContext.Current.Session["OpenAddressBookFromFilter"] = value;
            }
        }

        public DocsPaWR.Corrispondente corr
        {
            get
            {
                return Session["CorrespondentDetails_corr"] as DocsPaWR.Corrispondente;
            }
            set
            {
                Session["CorrespondentDetails_corr"] = value;
            }
        }

        private List<String> ListSelectedDocuments
        {
            get
            {
                List<string> result = null;
                if (HttpContext.Current.Session["ListSelectedDocuments"] != null)
                {
                    result = HttpContext.Current.Session["ListSelectedDocuments"] as List<String>;
                }
                return result;
            }

            set
            {
                HttpContext.Current.Session["ListSelectedDocuments"] = value;
            }
        }

        private bool CheckAll
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["checkAllDocuments"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["checkAllDocuments"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["checkAllDocuments"] = value;
            }
        }

        private bool IsZoom
        {
            get
            {
                if (HttpContext.Current.Session["isZoom"] != null)
                    return (bool)HttpContext.Current.Session["isZoom"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["isZoom"] = value;
            }
        }

        private Corrispondente CorrRichiedente
        {
            get
            {
                Corrispondente corr = null;
                if (HttpContext.Current.Session["corrRichiedente"] != null)
                {
                    corr = HttpContext.Current.Session["corrRichiedente"] as Corrispondente;
                }
                return corr;
            }
            set
            {
                HttpContext.Current.Session["corrRichiedente"] = value;
            }
        }

        private InstanceAccessDocument Document
        {
            get
            {
                InstanceAccessDocument doc = null;
                if (HttpContext.Current.Session["instanceAccessDocument"] != null)
                {
                    doc = HttpContext.Current.Session["instanceAccessDocument"] as InstanceAccessDocument;
                }
                return doc;
            }
            set
            {
                HttpContext.Current.Session["instanceAccessDocument"] = value;
            }
        }

        private FiltroRicerca[][] SearchFilters
        {
            get
            {
                return (FiltroRicerca[][])HttpContext.Current.Session["filtroDocumentInstanceAccess"];
            }
            set
            {
                HttpContext.Current.Session["filtroDocumentInstanceAccess"] = value;
            }
        }

        private List<InstanceAccessDocument> ListDocumentFiltered
        {
            get
            {
                List<InstanceAccessDocument> docs = null;
                if (HttpContext.Current.Session["ListDocumentFiltered"] != null)
                {
                    docs = HttpContext.Current.Session["ListDocumentFiltered"] as List<InstanceAccessDocument>;
                }
                return docs;
            }
            set
            {
                HttpContext.Current.Session["ListDocumentFiltered"] = value;
            }
        }

        private bool IsForwarded
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsForwarded"] != null) result = (bool)HttpContext.Current.Session["IsForwarded"];
                return result;

            }
            set
            {
                HttpContext.Current.Session["IsForwarded"] = value;
            }
        }

        private string SelectedRow
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["selectedRowInstanceDetails"] != null)
                {
                    result = HttpContext.Current.Session["selectedRowInstanceDetails"] as String;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["selectedRowInstanceDetails"] = value;
            }
        }

        private FileDocumento FileDoc
        {
            get
            {
                return HttpContext.Current.Session["fileDoc"] as FileDocumento;
            }
            set
            {
                HttpContext.Current.Session["fileDoc"] = value;
            }
        }

        private Templates TipologiaAttoIstanza
        {
            get
            {
                return HttpContext.Current.Session["TipologiaAttoIstanza"] as Templates;
            }
            set
            {
                HttpContext.Current.Session["TipologiaAttoIstanza"] = value;
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

        private bool EsisteDichiarazioneConformita
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["EsisteDichiarazioneConformita"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["EsisteDichiarazioneConformita"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["EsisteDichiarazioneConformita"] = value;
            }
        }

        /// <summary>
        /// Number of result in page
        /// </summary>
        public int PageSize
        {
            get
            {
                int result = 10;
                if (HttpContext.Current.Session["pageSizeDocument"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["pageSizeDocument"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["pageSizeDocument"] = value;
            }
        }

        private int SelectedPage
        {
            get
            {
                int toReturn = 1;
                if (HttpContext.Current.Session["selectedPage"] != null) Int32.TryParse(HttpContext.Current.Session["selectedPage"].ToString(), out toReturn);
                if (toReturn < 1) toReturn = 1;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["selectedPage"] = value;
            }
        }
        #endregion

        #region Const

        private const string CLOSE_POPUP_OBJECT = "closePopupObject";
        private const string CLOSE_POPUP_ADDRESS_BOOK = "closePopupAddressBook";
        private const string CLOSE_POPUP_TITOLARIO = "closePopupTitolario";
        private const string TUTTI = "0";
        private const int FILE_SIZE_LIMIT = 20;
        private const string UPDATE_PANEL_GRID_INDEXES = "upPnlGridIndexes";

        #endregion

        #region Standard Method

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializePage();
                this.InitializeLanguage();
            }
            else
            {
                ReadRetValueFromPopup();
            }

            this.RefreshScript();
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshprojectTxtDescrizione", "charsLeft('projectTxtDescrizione', '2000' , '" + this.projectLtrDescrizione.Text.Replace("'", "\'") + "');", true);
            this.projectTxtDescrizione_chars.Attributes["rel"] = "projectTxtDescrizione_'2000'_" + this.projectLtrDescrizione.Text;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CombineRowsHover", "CombineRowsHover();", true);
        }

        private void InitializePage()
        {
            this.ClearSession();
            this.TipologiaAttoIstanza = InstanceAccessManager.GetTemplate();
            this.TxtProtoRequest.ReadOnly = true;
            //this.divFrame.Visible = false;
            LoadMassiveOperation();
            //Back
            if (this.Request.QueryString["back"] != null && this.Request.QueryString["back"].Equals("1"))
            {
                List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                Navigation.NavigationObject obj = navigationList.Last();

                if (!string.IsNullOrEmpty(obj.IdInstance))
                {
                    DocsPaWR.InstanceAccess instance = InstanceAccessManager.GetInstanceAccessById(obj.IdInstance);
                    InstanceAccessManager.setInstanceAccessInSession(instance);
                }
            }
            else
            {
                this.SelectedRow = null;
            }

            if (Request.QueryString["t"] != null && Request.QueryString["t"].Equals("n"))
            {
                this.InstancelblDataChiusura.Visible = false;
                this.projectLblCodice.Visible = false;
                EnabledButtons(false);
                this.projectLblDataAperturaGenerata.Text = DateTime.Now.ToShortDateString();
            }
            else
            {
                DocsPaWR.InstanceAccess instanceAccess = InstanceAccessManager.GetInstanceAccessById(IdInstance);
                InstanceAccessManager.setInstanceAccessInSession(instanceAccess);
                PopulateFields();
                ListDocumentFiltered = new List<InstanceAccessDocument>();
                if (InstanceAccessManager.getInstanceAccessInSession() != null && InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS != null)
                {
                    ListDocumentFiltered.AddRange(InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS);
                }
                GridInstanceAccessDocuments_Bind();
                this.BuildGridNavigator();
            }
            string dataUser = RoleManager.GetRoleInSession().systemId;
            Registro reg = RegistryManager.GetRegistryInSession();
            if (reg == null)
            {
                reg = RoleManager.GetRoleInSession().registri[0];
            }
            dataUser = dataUser + "-" + reg.systemId;
            this.RapidRichiedente.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + "CALLTYPE_CORR_INT_EST_CON_DISABILITATI";
        }

        private void ClearSession()
        {
            HttpContext.Current.Session.Remove("ListDocumentFiltered");
            HttpContext.Current.Session.Remove("instanceAccessDocument");
            HttpContext.Current.Session.Remove("ListSelectedDocuments");
            HttpContext.Current.Session.Remove("checkAllDocuments");
            HttpContext.Current.Session.Remove("listDocumentsAccess");
            HttpContext.Current.Session.Remove("TipologiaAttoIstanza");
            HttpContext.Current.Session.Remove("fileDoc");
            HttpContext.Current.Session.Remove("pageSizeDocument");
            HttpContext.Current.Session.Remove("selectedPage");
            HttpContext.Current.Session.Remove("selectedRowInstanceDetails");
            HttpContext.Current.Session.Remove("EsisteDichiarazioneConformita");
            //questa property rimane in sessione quando dal tab allegati faccio back e torno nella ricerca; va rimossa
            HttpContext.Current.Session.Remove("selectedAttachmentId");
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
                return;
            }
            if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_OBJECT)))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_AddFilterDocInstanceAccess').contentWindow.closeObjectPopup();", true);
                return;
            }
            if (!string.IsNullOrEmpty(this.OpenTitolario.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenTitolario','');", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_AddFilterDocInstanceAccess').contentWindow.closeTitolarioPopup();", true);
                return;
            }
            if (!string.IsNullOrEmpty(this.SearchProject.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SearchProject','');", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_AddFilterDocInstanceAccess').contentWindow.closeSearchProjectPopup();", true);
                return;
            }
            if (!string.IsNullOrEmpty(this.AddressBook.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('AddressBook','');", true);
                if (OpenAddressBookFromFilter)
                {
                    HttpContext.Current.Session.Remove("OpenAddressBookFromFilter");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_AddFilterDocInstanceAccess').contentWindow.closeAddressBookPopup();", true);
                    return;
                }
                else
                {
                    this.btnAddressBookPostback_Click(null, null);
                }
                return;
            }

            if (!string.IsNullOrEmpty(this.SearchDocument.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SearchDocument','');", true);
                List<InstanceAccessDocument> listInstanceAccessDocument = BuildObjectInstanceAccessDocuments();
                HttpContext.Current.Session.Remove("listDocs");
                if (listInstanceAccessDocument != null && listInstanceAccessDocument.Count > 0)
                {
                    //Controllo che non vengano inseriti nell'istanza documenti la cui tipologia è la stessa del documento di dichiarazione di conformità;
                    //nel caso in cui viene inserito, controllo che ne sia solo uno e che nell'istanza non sia già presente un documento con tale tipologia:
                    //NELL'ISTANZA CI PUò ESSERE UN SOLO DOCUMENTO DI CONFORMITà
                    if (TipologiaAttoIstanza != null)
                    {
                        int countDocumentTipologiaAttoIstanza = (from d in listInstanceAccessDocument where d.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(this.TipologiaAttoIstanza.DESCRIZIONE) select d).ToList().Count;
                        DocsPaWR.InstanceAccess instance = InstanceAccessManager.getInstanceAccessInSession();
                        int countDocumentTipologiaAttoIstanzaInSession = (from d in instance.DOCUMENTS where d.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(this.TipologiaAttoIstanza.DESCRIZIONE) select d).ToList().Count;
                        if (countDocumentTipologiaAttoIstanza > 1 || (countDocumentTipologiaAttoIstanza > 0 && countDocumentTipologiaAttoIstanzaInSession > 0))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningDocumentsInInstance', 'warning', '','');", true);
                            return;
                        }
                    }
                    if (InstanceAccessManager.InsertInstanceAccessDocuments(listInstanceAccessDocument))
                    {
                        DocsPaWR.InstanceAccess instanceAccess = InstanceAccessManager.GetInstanceAccessById(InstanceAccessManager.getInstanceAccessInSession().ID_INSTANCE_ACCESS);
                        InstanceAccessManager.setInstanceAccessInSession(instanceAccess);
                        ApplyFilters();
                        GridInstanceAccessDocuments_Bind();
                        this.UpnlGridInstanceAccessDocuments.Update();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorInsertInstanceAccessDocument', 'error', '','');", true);
                    }
                }
                return;
            }

            if (!string.IsNullOrEmpty(this.SearchDocumentsInProject.ReturnValue))
            {
                //Leggi valore oggetti messi in sessione in  this.ListDocumentsAccess
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SearchDocumentsInProject','');", true);
                if (ListDocumentsAccess != null && ListDocumentsAccess.Count > 0)
                {
                    List<InstanceAccessDocument> instanceAccessDocInSession = InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS.ToList();
                    //Controllo che non vengano inseriti nell'istanza documenti la cui tipologia è la stessa del documento di dichiarazione di conformità;
                    //nel caso in cui viene inserito, controllo che ne sia solo uno e che nell'istanza non sia già presente un documento con tale tipologia:
                    //NELL'ISTANZA CI PUò ESSERE UN SOLO DOCUMENTO DI CONFORMITà
                    if (TipologiaAttoIstanza != null)
                    {
                        int countDocumentTipologiaAttoIstanza = (from d in ListDocumentsAccess where d.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(this.TipologiaAttoIstanza.DESCRIZIONE) select d).ToList().Count;
                        int countDocumentTipologiaAttoIstanzaInSession = (from d in instanceAccessDocInSession where d.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(this.TipologiaAttoIstanza.DESCRIZIONE) select d).ToList().Count;
                        if (countDocumentTipologiaAttoIstanza > 1 || (countDocumentTipologiaAttoIstanza > 0 && countDocumentTipologiaAttoIstanzaInSession > 0))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningDocumentsInInstance', 'warning', '','');", true);
                            return;
                        }
                    }
                    //Se nell'istanza sono stati già aggiunti documenti nel fascicolo selezionato non vengono inseriti
                    ListDocumentsAccess.RemoveAll(d => ((from d1 in InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS
                                                         where ((d.INFO_DOCUMENT.DOCNUMBER.Equals(d1.INFO_DOCUMENT.DOCNUMBER) &&
                                                         d1.INFO_PROJECT != null &&
                                                         d1.INFO_PROJECT.ID_PROJECT.Equals(d.INFO_PROJECT.ID_PROJECT)))
                                                         select d1).Count() > 0));
                    ListDocumentsAccess.ForEach(d => d.ID_INSTANCE_ACCESS = InstanceAccessManager.getInstanceAccessInSession().ID_INSTANCE_ACCESS);
                    if (ListDocumentsAccess.Count > 0)
                    {
                        if (InstanceAccessManager.InsertInstanceAccessDocuments(ListDocumentsAccess))
                        {
                            DocsPaWR.InstanceAccess instanceAccess = InstanceAccessManager.GetInstanceAccessById(InstanceAccessManager.getInstanceAccessInSession().ID_INSTANCE_ACCESS);
                            InstanceAccessManager.setInstanceAccessInSession(instanceAccess);
                            ApplyFilters();
                            GridInstanceAccessDocuments_Bind();
                            this.UpnlGridInstanceAccessDocuments.Update();
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorInsertInstanceAccessDocument', 'error', '','');", true);
                        }
                    }
                }
                HttpContext.Current.Session.Remove("listDocumentsAccess");
            }

            if (!string.IsNullOrEmpty(this.CorrespondentDetails.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('CorrespondentDetails','');", true);
                CorrRichiedente = Richiedente;
                HttpContext.Current.Session.Remove("CorrespondentDetails_corr");
                HttpContext.Current.Session.Remove("richiedente");
                return;
            }
            if (!string.IsNullOrEmpty(this.AddFilterDocInstanceAccess.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('AddFilterDocInstanceAccess','');", true);
                ApplyFilters();
                GridInstanceAccessDocuments_Bind();
                this.projectImgRemoveFilter.Enabled = true;
                UpContainerProjectTab.Update();
                UpnlTabHeader.Update();
                this.UpnlGridInstanceAccessDocuments.Update();
                return;
            }
            if (!string.IsNullOrEmpty(this.HiddenRemoveDichiarazione.Value))
            {
                SchedaDocumento schedaDoc = DocumentManager.getDocumentDetails(this, Document.DOCNUMBER, Document.DOCNUMBER);

                //Verifico se l'utente può rimuovere la dichiarazione: la dichiarazione può essere rimossa solo se l'utente è il creatore dell'istanza
                //e della dichiarazione.
                if (InstanceAccessManager.getInstanceAccessInSession().ID_PEOPLE_OWNER.Equals(UserManager.GetInfoUser().idPeople) &&
                    InstanceAccessManager.getInstanceAccessInSession().ID_GROUPS_OWNER.Equals(UserManager.GetInfoUser().idGruppo) &&
                    schedaDoc.creatoreDocumento.idPeople.Equals(UserManager.GetInfoUser().idPeople))
                {
                    this.RemoveDocument();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningRemoveDichiarazioneInstanceAccess', 'warning', '','');", true);
                }
                this.HiddenRemoveDichiarazione.Value = string.Empty;
                return;
            }
            if (!string.IsNullOrEmpty(this.HiddenRemoveDocuments.Value))
            {
                RemoveDocuments();
                this.HiddenRemoveDocuments.Value = string.Empty;
                return;
            }
            if (!string.IsNullOrEmpty(this.HiddenConfirmCreateDeclaration.Value))
            {
                CreateDeclaration();
                this.HiddenConfirmCreateDeclaration.Value = string.Empty;
                this.BuildGridNavigator();
                return;
            }
            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UPDATE_PANEL_GRID_INDEXES))
            {
                this.SelectedPage = int.Parse(this.grid_pageindex.Value);
                GridInstanceAccessDocuments_Bind();
                this.UpnlGridInstanceAccessDocuments.Update();
                return;
            }

            if (!string.IsNullOrEmpty(this.ViewDetailsInstanceAccessDocument.ReturnValue))
            {
                GridInstanceAccessDocuments_Bind();
                this.UpnlGridInstanceAccessDocuments.Update();
                this.BuildGridNavigator();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ViewDetailsInstanceAccessDocument','');", true);
            }

            HttpContext.Current.Session.Remove("isZoom");
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.InstanceDetailsSave.Text = Utils.Languages.GetLabelFromCode("InstanceDetailsSave", language);
            this.InstanceDetailsCreate.Text = Utils.Languages.GetLabelFromCode("InstanceDetailsCreate", language);
            this.InstanceDownload.Text = Utils.Languages.GetLabelFromCode("InstanceDownload", language);
            this.InstanceForward.Text = Utils.Languages.GetLabelFromCode("InstanceForward", language);
            this.InstanceDdlMassiveOperation.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("InstanceDdlMassiveOperation", language));
            this.InstanceLblDescrizione.Text = Utils.Languages.GetLabelFromCode("InstanceLblDescrizione", language);
            this.projectLtrDescrizione.Text = Utils.Languages.GetLabelFromCode("DocumentLitVisibleNotesChars", language) + " ";
            this.InstanceLblDataApertura.Text = Utils.Languages.GetLabelFromCode("InstanceLblDataApertura", language);
            this.InstancelblDataChiusura.Text = Utils.Languages.GetLabelFromCode("InstancelblDataChiusura", language);
            this.litRichiedente.Text = Utils.Languages.GetLabelFromCode("InstanceLblAnswer", language);
            this.ImgRichiedenteAddressBook.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgAnswerAddressBook", language);
            this.ImgRichiedenteAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgAnswerAddressBook", language);
            this.chkRichiedenteExtendHistoricized.Text = Utils.Languages.GetLabelFromCode("SearchProjectCreatoreExtendHistoricized", language);
            this.lit_dtaRequest.Text = Utils.Languages.GetLabelFromCode("InstanceAccessLitRequestDate", language);
            this.NoteInstance.Text = Utils.Languages.GetLabelFromCode("InstanceAccessNote", language);
            this.litProtoRequest.Text = Utils.Languages.GetLabelFromCode("InstanceAccessProtoRequest", language);
            this.OpenAddDocCustom.Title = Utils.Languages.GetLabelFromCode("OpenAddDocTitleCustom", language);
            this.SearchDocument.Title = Utils.Languages.GetLabelFromCode("OpenAddDocInstance", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
            this.ChooseCorrespondent.Title = Utils.Languages.GetLabelFromCode("InstanceAccessChooseCorrespondent", language);
            this.AddFilterDocInstanceAccess.Title = Utils.Languages.GetLabelFromCode("InstanceAccessAddFilter", language);
            this.Object.Title = Utils.Languages.GetLabelFromCode("TitleObjectPopup", language);
            this.OpenTitolario.Title = Utils.Languages.GetLabelFromCode("TitleClassificationScheme", language);
            this.SearchProject.Title = Utils.Languages.GetLabelFromCode("SearchProjectTitleSearch", language);
            this.InstanceAccessCercaProto.ToolTip = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Cerca", language);
            this.LinkDocFascBtn_Reset.ToolTip = Utils.Languages.GetLabelFromCode("InstanceLinkDocFascBtn_Reset", language);
            this.projectImgAddDoc.ToolTip = Utils.Languages.GetLabelFromCode("InstanceImgAddDocInstance", language);
            this.projectImgAddDoc.AlternateText = Utils.Languages.GetLabelFromCode("InstanceImgAddDocInstance", language);
            this.InstanceImgAddPrj.ToolTip = Utils.Languages.GetLabelFromCode("InstanceImgAddPrj", language);
            this.InstanceImgAddPrj.AlternateText = Utils.Languages.GetLabelFromCode("InstanceImgAddPrj", language);
            this.SearchDocumentsInProject.Title = Utils.Languages.GetLabelFromCode("SearchDocumentsInProject", language);
            this.CorrespondentDetails.Title = Utils.Languages.GetLabelFromCode("InstanceDetailsCorrespondentDetails", language);
            this.ViewDetailsInstanceAccessDocument.Title = Utils.Languages.GetLabelFromCode("ViewDetailsInstanceAccessDocumentTitle", language);
            this.InstancePrepareDownload.Text = Utils.Languages.GetLabelFromCode("InstancePrepareDownload", language);
            this.lblElencoDocumenti.Text = Utils.Languages.GetLabelFromCode("InstanceDetailsElencoDocumenti", language);
            this.ImgRichiedenteDetails.ToolTip = Utils.Languages.GetLabelFromCode("InstanceDetailsDettaglioRichiedente", language);
            this.projectImgAddFilter.ToolTip = Utils.Languages.GetLabelFromCode("projectImgAddFilter", language);
            this.projectImgRemoveFilter.ToolTip = Utils.Languages.GetLabelFromCode("projectImgRemoveFilter", language);
            this.InstancePrepareDownloadUpdate.Text = Utils.Languages.GetLabelFromCode("InstancePrepareDownloadUpdate", language);
            this.DigitalSignDetails.Title = Utils.Languages.GetLabelFromCode("DigitalSignDetailsTitle", language);
            this.DocumentViewer.Title = Utils.Languages.GetLabelFromCode("TitleDocumentViewerPopup", language);
        }

        #endregion

        #region Event handler

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

        protected void ImgRichiedenteDetails_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.txtDescrizioneRichiedente.Text))
                {
                    string msg = "ErrorRichiedenteNotSelected";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                }
                else
                {
                    this.Richiedente = this.CorrRichiedente;
                    if ((this.CorrRichiedente == null && !string.IsNullOrEmpty(this.txtDescrizioneRichiedente.Text)) ||
                        (this.CorrRichiedente != null && !this.CorrRichiedente.descrizione.Equals(this.txtDescrizioneRichiedente.Text)))
                    {
                        Corrispondente corr = new DocsPaWR.Corrispondente();
                        corr.descrizione = this.txtDescrizioneRichiedente.Text;
                        corr.tipoCorrispondente = "O";
                        corr.idAmministrazione = UserManager.GetUserInSession().idAmministrazione;
                        this.Richiedente = corr;
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "CorrespondentDetails", "ajaxModalPopupCorrespondentDetails();", true);
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
                            CorrRichiedente = corr;
                            if (InstanceAccessManager.getInstanceAccessInSession() != null)
                                InstanceAccessManager.getInstanceAccessInSession().RICHIEDENTE = CorrRichiedente;
                            this.upPnlRichiedente.Update();
                        }
                    }
                    else
                    {
                        corr = null;
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
                    CorrRichiedente = tempCorrSingle;
                    if (InstanceAccessManager.getInstanceAccessInSession() != null)
                        InstanceAccessManager.getInstanceAccessInSession().RICHIEDENTE = CorrRichiedente;
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

        protected void InstanceDetailsSave_Click(object o, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.projectTxtDescrizione.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorInstanceAccessDescriptionNoEmpty', 'warning', '');", true);
                    return;
                }

                if(string.IsNullOrEmpty(this.txtDescrizioneRichiedente.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorInstanceAccessRichiedenteNoEmpty', 'warning', '');", true);
                    return;
                }

                if (!string.IsNullOrEmpty(this.dtaRequest_TxtFrom.Text) && !Utils.utils.verificaIntervalloDate(DateTime.Now.ToShortDateString(), this.dtaRequest_TxtFrom.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorInstanceAccessDate', 'warning', '');", true);
                    return;
                }

                bool aggiornaRichiedente = false;
                //CASO NUOVA ISTANZA
                if (InstanceAccessManager.getInstanceAccessInSession() == null || string.IsNullOrEmpty(InstanceAccessManager.getInstanceAccessInSession().ID_INSTANCE_ACCESS))
                {

                    DocsPaWR.InstanceAccess instance = new DocsPaWR.InstanceAccess();
                    instance.CREATION_DATE = DateTime.Now;
                    instance.DESCRIPTION = this.projectTxtDescrizione.Text;
                    instance.ID_PEOPLE_OWNER = UserManager.GetUserInSession().idPeople;
                    instance.ID_GROUPS_OWNER = RoleManager.GetRoleInSession().idGruppo;
                    instance.RICHIEDENTE = CorrRichiedente != null ? CorrRichiedente : new Corrispondente();
                    if (string.IsNullOrEmpty(this.txtDescrizioneRichiedente.Text))
                    {
                        instance.RICHIEDENTE = null;
                    }
                    else if (string.IsNullOrEmpty(this.txtCodiceRichiedente.Text) ||
                            (instance.RICHIEDENTE != null && !instance.RICHIEDENTE.descrizione.Equals(this.txtDescrizioneRichiedente.Text)))
                    {
                        instance.RICHIEDENTE.tipoCorrispondente = "O";
                        instance.RICHIEDENTE.systemId = string.Empty;
                        instance.RICHIEDENTE.descrizione = this.txtDescrizioneRichiedente.Text;
                        aggiornaRichiedente = true;
                    }
                    //else
                    //{
                    //    instance.RICHIEDENTE = CorrRichiedente;
                    //}
                    instance.REQUEST_DATE = Utils.utils.formatStringToDate(this.dtaRequest_TxtFrom.Text);
                    instance.ID_DOCUMENT_REQUEST = this.idProtoRequest.Value;
                    instance.NOTE = this.TxtNote.Text;
                    DocsPaWR.InstanceAccess instanceAccess = InstanceAccessManager.InsertInstanceAccess(instance);
                    if (instanceAccess != null)
                    {
                        InstanceAccessManager.setInstanceAccessInSession(instanceAccess);
                        this.IdInstance = instanceAccess.ID_INSTANCE_ACCESS;
                        if (aggiornaRichiedente)
                        {
                            if (instanceAccess.RICHIEDENTE != null)
                            {
                                Corrispondente corr = AddressBookManager.GetCorrespondentBySystemId(instanceAccess.RICHIEDENTE.systemId);
                                if (corr != null)
                                {
                                    this.txtCodiceRichiedente.Text = corr.codiceRubrica;
                                    this.txtDescrizioneRichiedente.Text = corr.descrizione;
                                    this.upPnlRichiedente.Update();
                                    corr.tipoCorrispondente = "O";
                                    instanceAccess.RICHIEDENTE = corr;
                                    CorrRichiedente = corr;
                                }
                            }
                            else
                            {
                                this.txtCodiceRichiedente.Text = string.Empty;
                                this.txtDescrizioneRichiedente.Text = string.Empty;
                                this.upPnlRichiedente.Update();
                            }
                        }
                        this.projectLblCodice.Visible = true;
                        this.projectLblCodiceGenerato.Text = instanceAccess.ID_INSTANCE_ACCESS;
                        this.projectImgAddDoc.Enabled = true;
                        EnabledButtons(true);
                        this.upPnlButtons.Update();
                        this.UpdatePanel1.Update();
                        this.UpnlButtonTop.Update();
                        this.InstanceTabs.RefreshTabs();
                        this.ListDocumentFiltered = new List<InstanceAccessDocument>();
                        GridInstanceAccessDocuments_Bind();
                        this.UpnlGridInstanceAccessDocuments.Update();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorInstanceAccessInsertInstance', 'error', '');", true);
                        return;
                    }
                }
                // CASO MODIFICA ISTANZA ESISTENTE
                else
                {
                    DocsPaWR.InstanceAccess instance = InstanceAccessManager.getInstanceAccessInSession();
                    instance.DESCRIPTION = this.projectTxtDescrizione.Text;
                    if (string.IsNullOrEmpty(this.txtDescrizioneRichiedente.Text))
                    {
                        instance.RICHIEDENTE = null;
                    }
                    else if (string.IsNullOrEmpty(this.txtCodiceRichiedente.Text) ||
                            (instance.RICHIEDENTE != null && !instance.RICHIEDENTE.descrizione.Equals(this.txtDescrizioneRichiedente.Text)))
                    {
                        instance.RICHIEDENTE = CorrRichiedente != null ? CorrRichiedente : new Corrispondente();
                        instance.RICHIEDENTE.tipoCorrispondente = "O";
                        instance.RICHIEDENTE.systemId = string.Empty;
                        instance.RICHIEDENTE.descrizione = this.txtDescrizioneRichiedente.Text;
                        aggiornaRichiedente = true;
                    }
                    else
                    {
                        instance.RICHIEDENTE = CorrRichiedente;
                    }
                    instance.REQUEST_DATE = Utils.utils.formatStringToDate(this.dtaRequest_TxtFrom.Text);
                    instance.ID_DOCUMENT_REQUEST = this.idProtoRequest.Value;
                    instance.NOTE = this.TxtNote.Text;
                    DocsPaWR.InstanceAccess instanceAccess = InstanceAccessManager.UpdateInstanceAccess(instance);
                    if (instanceAccess != null)
                    {
                        InstanceAccessManager.setInstanceAccessInSession(instanceAccess);
                        if (aggiornaRichiedente)
                        {
                            if (instanceAccess.RICHIEDENTE != null)
                            {
                                Corrispondente corr = AddressBookManager.GetCorrespondentBySystemId(instanceAccess.RICHIEDENTE.systemId);
                                if (corr != null)
                                {
                                    this.txtCodiceRichiedente.Text = corr.codiceRubrica;
                                    this.txtDescrizioneRichiedente.Text = corr.descrizione;
                                    this.upPnlRichiedente.Update();
                                    corr.tipoCorrispondente = "O";
                                    instanceAccess.RICHIEDENTE = corr;
                                    CorrRichiedente = corr;
                                }
                            }
                            else
                            {
                                this.txtCodiceRichiedente.Text = string.Empty;
                                this.txtDescrizioneRichiedente.Text = string.Empty;
                            }
                            this.upPnlRichiedente.Update();
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorInstanceAccessUpdateInstance', 'error', '');", true);
                        return;
                    }
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


        protected void InstanceDownload_Click(object o, EventArgs e)
        {
            FileDoc = new FileDocumento();
            try
            {
                string pathFileZip = Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "BE_INSTANCE_ACCESS_PATH")
                     + @"\" + AdministrationManager.AmmGetInfoAmmCorrente(UserManager.GetUserInSession().idAmministrazione).Codice + @"\" + InstanceAccessManager.getInstanceAccessInSession().ID_INSTANCE_ACCESS + ".ZIP";
                Uri proxURI = new Uri(pathFileZip);
                string lastArg = proxURI.Segments.LastOrDefault();
                WebRequest request = WebRequest.Create(proxURI.OriginalString);
                request.Method = "GET";
                WebResponse response = request.GetResponse();
                Stream content = response.GetResponseStream();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    while ((bytesRead = content.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, bytesRead);
                    }
                    FileDoc.content = memoryStream.ToArray();
                }
                response.Close();
                FileDoc.path = proxURI.AbsolutePath;
                if (lastArg.ToLower().EndsWith(".zip"))
                    FileDoc.contentType = "application/x-zip-compressed";
                FileDoc.name = lastArg;

                this.frame.Attributes["src"] = "../Document/AttachmentViewer.aspx?download=1";
                this.UpPanelFrame.Update();
            }
            catch (System.Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorInstanceDetailsDownload', 'error', '');", true);
                return;
            }
        }

        protected void InstanceDetailsCreate_Click(object o, EventArgs e)
        {
            try
            {
                DocsPaWR.InstanceAccess instanceAccess = InstanceAccessManager.getInstanceAccessInSession();
                if (this.TipologiaAttoIstanza == null || string.IsNullOrEmpty(TipologiaAttoIstanza.PATH_MODELLO_1))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningInstanceDetailsNotFoundTemplate', 'warning', '');", true);
                    return;
                }
                if ((from i in instanceAccess.DOCUMENTS
                     where i.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(this.TipologiaAttoIstanza.DESCRIZIONE)
                     select i).FirstOrDefault() != null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('ConfirmCreateDeclaration', 'HiddenConfirmCreateDeclaration', '');", true);
                    return;
                }
                else
                {
                    CreateDeclaration();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void InstancePrepareDownload_Click(object o, EventArgs e)
        {
            UIManager.InstanceAccessManager.AsyncCreateDownload(UIManager.InstanceAccessManager.getInstanceAccessInSession());
            DocsPaWR.InstanceAccess inst = UIManager.InstanceAccessManager.getInstanceAccessInSession();
            inst.STATE_DOWNLOAD_FORWARD = '1';
            UIManager.InstanceAccessManager.setInstanceAccessInSession(inst);
            RefreshInstanceStateDownload();
            this.upPnlButtons.Update();
        }

        protected void InstancePrepareDownloadUpdate_Click(object o, EventArgs e)
        {
            DocsPaWR.InstanceAccess instance = InstanceAccessManager.getInstanceAccessInSession();
            try
            {
                string stateDownload = InstanceAccessManager.GetStateDownloadInstanceAccess(instance.ID_INSTANCE_ACCESS);
                if (!string.IsNullOrEmpty(stateDownload) && !stateDownload.Equals("1"))
                {
                    instance.STATE_DOWNLOAD_FORWARD = Convert.ToChar(stateDownload);
                    InstanceAccessManager.setInstanceAccessInSession(instance);
                    RefreshInstanceStateDownload();
                    this.upPnlButtons.Update();
                    if (stateDownload.Equals("0"))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorPrepareDownloadInstance', 'error', '');", true);
                    }
                }
                this.upPnlButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InstanceForward_Click(object o, EventArgs e)
        {
            SchedaDocumento doc = null;
            try
            {
                DocsPaWR.InstanceAccess instance = InstanceAccessManager.getInstanceAccessInSession();
                int totalFileSizeInstance = 0;
                doc = InstanceAccessManager.ForwardsInstanceAccess(instance, out totalFileSizeInstance);
                //Converto in Mb e verifico che non superi il limite massimo consentito per la spedizione.
                //In caso lo superi, viene bloccata l'operazione di inoltro
                if (((double)totalFileSizeInstance / 1048576) > FILE_SIZE_LIMIT)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningForwardsInstanceAccessMaxFileSize', 'warning', '');", true);
                    return;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
            if (doc != null)
            {
                #region navigation
                List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                actualPage.IdInstance = InstanceAccessManager.getInstanceAccessInSession().ID_INSTANCE_ACCESS;
                actualPage.IdInstance = InstanceAccessManager.getInstanceAccessInSession().ID_INSTANCE_ACCESS;
                //actualPage.NumPage = this.gridInstanceAccessDocuments.PageIndex.ToString();
                //actualPage.PageSize = this.gridInstanceAccessDocuments.PageCount.ToString();

                actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_DETAILS.ToString(), string.Empty);
                actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_DETAILS.ToString(), true, this.Page);
                actualPage.CodePage = Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_DETAILS.ToString();

                actualPage.Page = "INSTANCEDETAILS.ASPX";
                navigationList.Add(actualPage);
                Navigation.NavigationUtils.SetNavigationList(navigationList);
                #endregion
                this.IsForwarded = true;
                DocumentManager.setSelectedRecord(doc);
                Response.Redirect("~/Document/Document.aspx");
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorForwardsInstanceAccess', 'error', '');", true);
            }
        }

        protected void InstanceDdlMassiveOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetCheckBox();
            if (this.ListSelectedDocuments != null && this.ListSelectedDocuments.Count > 0)
            {
                switch (InstanceDdlMassiveOperation.SelectedValue)
                {
                    case "MASSIVE_REMOVE_DOCUMENTS":
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmRemoveDocumentsInstanceDetails', 'HiddenRemoveDocuments', '');", true);
                        break;
                    case "MASSIVE_REQUEST_COPIA_CONFORME":
                        UpdateTypeRequestSelectedDocument(InstanceAccessManager.TipoRichiesta.COPIA_CONFORME);
                        break;
                    case "MASSIVE_REQUEST_AUTENTICA":
                        UpdateTypeRequestSelectedDocument(InstanceAccessManager.TipoRichiesta.COPIA_SEMPLICE);
                        break;
                    case "MASSIVE_REQUEST_ESTRATTO":
                        UpdateTypeRequestSelectedDocument(InstanceAccessManager.TipoRichiesta.ESTRATTO);
                        break;
                    case "MASSIVE_REQUEST_DUPLICATO":
                        UpdateTypeRequestSelectedDocument(InstanceAccessManager.TipoRichiesta.DUPLCATO);
                        break;
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorMassiveOperationNoItemSelected', 'warning', '');", true);
            }
            this.InstanceDdlMassiveOperation.SelectedIndex = -1;
            this.UpnlAzioniMassive.Update();
        }

        protected void projectImgRemoveFilter_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                this.projectImgRemoveFilter.Enabled = false;
                HttpContext.Current.Session.Remove("filtroDocumentInstanceAccess");
                ListDocumentFiltered = new List<InstanceAccessDocument>();
                ListDocumentFiltered.AddRange(InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS);
                GridInstanceAccessDocuments_Bind();
                this.UpnlGridInstanceAccessDocuments.Update();
                UpContainerProjectTab.Update();
                UpnlTabHeader.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #region Management Grid

        private void GridInstanceAccessDocuments_Bind()
        {
            this.EsisteDichiarazioneConformita = (this.TipologiaAttoIstanza != null &&
                    (from d in InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS where d.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(this.TipologiaAttoIstanza.DESCRIZIONE) select d).FirstOrDefault() != null);
            EnabledButtons(true);
            RefreshInstanceStateDownload();
            this.lblNumeroDocumenti.Text = Utils.Languages.GetLabelFromCode("InstanceDetailsNumeroDocumenti", UserManager.GetUserLanguage()).Replace("@@", ListDocumentFiltered.Count.ToString());
            TotalNumberFile();
            this.UpnlNumeroDocumenti.Update();
            this.UpnlButtonTop.Update();

            List<InstanceAccessDocument> listRes = new List<InstanceAccessDocument>();
            listRes.AddRange(ListDocumentFiltered);

            int dtRowsCount = ListDocumentFiltered.Count;
            int index = 1;
            if (dtRowsCount > 0)
            {
                for (int i = 0; i < dtRowsCount; i++)
                {
                    InstanceAccessDocument d = new InstanceAccessDocument();
                    d = ListDocumentFiltered[i];
                    listRes.Insert(index, d);
                    index += 2;
                }
            }
            PagedDataSource dsP = new PagedDataSource();
            dsP.DataSource = listRes;
            dsP.AllowPaging = true;
            if (listRes.Count > 0 && ((float)listRes.Count / this.PageSize) <= (SelectedPage - 1))
                SelectedPage = (listRes.Count % PageSize) > 0 ?
                    (listRes.Count / PageSize) + 1 : (listRes.Count / PageSize);
            else if (listRes == null || listRes.Count == 0)
                SelectedPage = 1;
            //SelectedPage -= 1;
            dsP.CurrentPageIndex = SelectedPage - 1;
            dsP.PageSize = PageSize;
            this.gridInstanceAccessDocuments.DataSource = dsP;
            this.gridInstanceAccessDocuments.DataBind();
            SetCheckBox();
            if (ListSelectedDocuments != null && ListSelectedDocuments.Count() > 0)
            {
                foreach (GridViewRow row in gridInstanceAccessDocuments.Rows)
                {
                    (row.FindControl("ChkSelectedDocument") as CheckBox).Checked = (from id in ListSelectedDocuments
                                                                                    where id.Equals((row.FindControl("lblIdInstanceDocumentId") as Label).Text)
                                                                                    select id).Count() > 0;
                }
            }
            if (CheckAll)
                (this.gridInstanceAccessDocuments.HeaderRow.FindControl("ChkSelectedAllDocuments") as CheckBox).Checked = true;
            BuildGridNavigator();
        }

        protected void GridInstanceAccessDocuments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.gridInstanceAccessDocuments.PageIndex = e.NewPageIndex;
                this.SelectedRow = null;
                GridInstanceAccessDocuments_Bind();
                this.UpnlGridInstanceAccessDocuments.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GridInstanceAccessDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    InstanceAccessDocument doc = e.Row.DataItem as InstanceAccessDocument;
                    if (!string.IsNullOrEmpty(doc.TYPE_REQUEST) && (this.TipologiaAttoIstanza == null || !doc.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(this.TipologiaAttoIstanza.DESCRIZIONE)))
                        (e.Row.FindControl("DdlRequest") as DropDownList).Items.FindByText(doc.TYPE_REQUEST).Selected = true;
                    CheckBox checkBox = e.Row.FindControl("ChkSelectedDocument") as CheckBox;
                    if (checkBox != null)
                    {
                        checkBox.Attributes["onclick"] = "SetProjectCheck(this, '" + doc.ID_INSTANCE_ACCESS_DOCUMENT + "')";
                    }
                    //(e.Row.FindControl("stateInstanceAccessDocument") as HtmlGenericControl).Attributes["class"] = SetStateInstanceDocument(doc);

                    if (EsisteDichiarazioneConformita && doc.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(this.TipologiaAttoIstanza.DESCRIZIONE))
                    {
                        (e.Row.FindControl("DdlRequest") as DropDownList).Visible = false;
                        (e.Row.FindControl("IndexImgDetailsDocument") as CustomImageButton).Visible = false;
                        (e.Row.FindControl("IndexImgDetailsDocument") as CustomImageButton).Visible = false;
                        (e.Row.FindControl("stateInstanceAccessDocument") as Image).Visible = false;
                        (e.Row.FindControl("imgStateInstanceAccessDocument") as Image).Visible = false;
                        (e.Row.FindControl("lblTotalNumberAttachment") as Label).Visible = false;
                        (e.Row.FindControl("btnSignatureDetails") as CustomImageButton).Visible = doc.INFO_DOCUMENT.IS_SIGNED;
                        string url = ResolveUrl(FileManager.getFileIcon(this.Page, doc.INFO_DOCUMENT.EXTENSION));
                        (e.Row.FindControl("btnTypeDoc") as CustomImageButton).Visible = true;
                        (e.Row.FindControl("btnTypeDoc") as CustomImageButton).ImageUrl = url;
                        (e.Row.FindControl("btnTypeDoc") as CustomImageButton).OnMouseOverImage = url;
                        (e.Row.FindControl("btnTypeDoc") as CustomImageButton).OnMouseOutImage = url;
                        (e.Row.FindControl("btnTypeDoc") as CustomImageButton).ImageUrlDisabled = url;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(doc.INFO_DOCUMENT.ID_DOCUMENTO_PRINCIPALE))
                        {
                            (e.Row.FindControl("stateInstanceAccessDocument") as Image).Visible = true;
                            (e.Row.FindControl("stateInstanceAccessDocument") as Image).ImageUrl = SetStateInstanceDocument(doc);
                            (e.Row.FindControl("stateInstanceAccessDocument") as Image).ToolTip = SetStateInstanceDocumentWar(doc);
                            (e.Row.FindControl("stateInstanceAccessDocument") as Image).AlternateText = SetStateInstanceDocumentWar(doc);
                        }

                        (e.Row.FindControl("imgStateInstanceAccessDocument") as Image).ImageUrl = StateInstanceDocPrincipale(doc);
                        (e.Row.FindControl("imgStateInstanceAccessDocument") as Image).ToolTip = SetIntanceDocumentTooltip(doc);
                        (e.Row.FindControl("imgStateInstanceAccessDocument") as Image).AlternateText = SetIntanceDocumentTooltip(doc);
                        if (this.EsisteDichiarazioneConformita)
                        {
                            (e.Row.FindControl("DdlRequest") as DropDownList).Enabled = false;
                            (e.Row.FindControl("ImgRemoveNotify") as CustomImageButton).Enabled = false;
                        }

                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BuildGridNavigator()
        {
            try
            {
                this.plcNavigator.Controls.Clear();

                int countPage = (int)Math.Round(((double)(this.ListDocumentFiltered.Count * 2) / (double)this.PageSize) + 0.49);

                int val = (this.ListDocumentFiltered.Count * 2) % this.PageSize;
                //if (val == 0)
                //{
                //    countPage = countPage - 1;
                //}

                if (countPage > 1)
                {
                    Panel panel = new Panel();
                    panel.EnableViewState = true;
                    panel.CssClass = "recordNavigator";

                    int startFrom = 1;
                    if (this.SelectedPage > 6) startFrom = this.SelectedPage - 5;

                    int endTo = 10;
                    if (this.SelectedPage > 6) endTo = this.SelectedPage + 5;
                    if (endTo > countPage) endTo = countPage;

                    if (startFrom > 1)
                    {
                        LinkButton btn = new LinkButton();
                        btn.EnableViewState = true;
                        btn.Text = "...";
                        btn.Attributes["onclick"] = " $('#grid_pageindex').val(" + (startFrom - 1) + "); __doPostBack('upPnlGridIndexes', ''); return false;";
                        panel.Controls.Add(btn);
                    }

                    for (int i = startFrom; i <= endTo; i++)
                    {
                        if (i == this.SelectedPage)
                        {
                            Literal lit = new Literal();
                            lit.Text = "<span>" + i.ToString() + "</span>";
                            panel.Controls.Add(lit);
                        }
                        else
                        {
                            LinkButton btn = new LinkButton();
                            btn.EnableViewState = true;
                            btn.Text = i.ToString();
                            btn.Attributes["onclick"] = " $('#grid_pageindex').val($(this).text()); __doPostBack('upPnlGridIndexes', ''); return false;";
                            panel.Controls.Add(btn);
                        }
                    }

                    if (endTo < countPage)
                    {
                        LinkButton btn = new LinkButton();
                        btn.EnableViewState = true;
                        btn.Text = "...";
                        btn.Attributes["onclick"] = " $('#grid_pageindex').val(" + endTo + "); __doPostBack('upPnlGridIndexes', ''); return false;";
                        panel.Controls.Add(btn);
                    }

                    this.plcNavigator.Controls.Add(panel);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void gridInstanceAccessDocuments_PreRender(object sender, EventArgs e)
        {
            try
            {
                gridInstanceAccessDocuments.Columns[0].Visible = false;

                // GridManager.SelectedGrid

                int cellsCount = 0;
                if (gridInstanceAccessDocuments.Columns.Count > 0)
                    foreach (DataControlField td in gridInstanceAccessDocuments.Columns)
                        if (td.Visible) cellsCount++;

                bool alternateRow = false;
                int indexCellIcons = -1;

                if (cellsCount > 0)
                {
                    for (int i = 1; i < gridInstanceAccessDocuments.Rows.Count; i = i + 2)
                    {

                        gridInstanceAccessDocuments.Rows[i].CssClass = "NormalRow";
                        if (alternateRow) gridInstanceAccessDocuments.Rows[i].CssClass = "AltRow";
                        alternateRow = !alternateRow;

                        for (int j = 0; j < gridInstanceAccessDocuments.Rows[i].Cells.Count; j++)
                        {
                            bool found = false;
                            foreach (Control c in gridInstanceAccessDocuments.Rows[i].Cells[j].Controls)
                            {
                                if (c.ID == "ImgViewDocument")
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                                gridInstanceAccessDocuments.Rows[i].Cells[j].Visible = false;
                            else
                            {
                                gridInstanceAccessDocuments.Rows[i].Cells[j].ColumnSpan = cellsCount - 1;
                                gridInstanceAccessDocuments.Rows[i].Cells[j].Attributes["style"] = "text-align: right;";
                                indexCellIcons = j;
                            }
                        }


                    }

                    alternateRow = false;
                    for (int i = 0; i < gridInstanceAccessDocuments.Rows.Count; i = i + 2)
                    {
                        gridInstanceAccessDocuments.Rows[i].CssClass = "NormalRow";
                        if (alternateRow) gridInstanceAccessDocuments.Rows[i].CssClass = "AltRow";
                        alternateRow = !alternateRow;

                        for (int j = 0; j < gridInstanceAccessDocuments.Rows[i].Cells.Count; j++)
                        {
                            bool found = false;
                            foreach (Control c in gridInstanceAccessDocuments.Rows[i].Cells[j].Controls)
                            {
                                if (c.ID == "ImgViewDocument")
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (found)
                                gridInstanceAccessDocuments.Rows[i].Cells[j].Visible = false;
                            else
                                gridInstanceAccessDocuments.Rows[i].Cells[j].Attributes["style"] = "text-align: center;";
                        }


                    }
                    if (indexCellIcons > -1)
                        gridInstanceAccessDocuments.HeaderRow.Cells[indexCellIcons].Visible = false;
                    if (gridInstanceAccessDocuments.HeaderRow != null)
                    {
                        for (int j = 0; j < gridInstanceAccessDocuments.HeaderRow.Cells.Count; j++)
                            gridInstanceAccessDocuments.HeaderRow.Cells[j].Attributes["style"] = "text-align: center;";
                    }
                }

                if (!string.IsNullOrEmpty(this.SelectedRow))
                {
                    for (int i = 0; i < gridInstanceAccessDocuments.Rows.Count; i++)
                    {
                        if (this.gridInstanceAccessDocuments.Rows[i].RowIndex == Int32.Parse(this.SelectedRow))
                        {
                            this.gridInstanceAccessDocuments.Rows[i].Attributes.Remove("class");
                            this.gridInstanceAccessDocuments.Rows[i].CssClass = "selectedrow";
                            this.gridInstanceAccessDocuments.Rows[i - 1].Attributes.Remove("class");
                            this.gridInstanceAccessDocuments.Rows[i - 1].CssClass = "selectedrow";
                        }
                    }

                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected string GetLabelDoc(InstanceAccessDocument document)
        {
            string result = string.Empty;
            result = !string.IsNullOrEmpty(document.INFO_DOCUMENT.NUMBER_PROTO) ? "<font color='#FF0000'>" + document.INFO_DOCUMENT.NUMBER_PROTO + "</font>" : document.INFO_DOCUMENT.DOCNUMBER;
            return result;
        }

        protected string GetLabelRegister(InstanceAccessDocument document)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(document.INFO_DOCUMENT.COUNTER_REPERTORY))
                result = document.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO;
            else
                result = document.INFO_DOCUMENT.REGISTER;
            return result;
        }

        protected string GetLabelTypeProto(InstanceAccessDocument document)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(document.INFO_DOCUMENT.ID_DOCUMENTO_PRINCIPALE))
                result = "ALL";
            else if (document.INFO_DOCUMENT.TYPE_PROTO.Equals("G"))
                result = "NP";
            else
                result = document.INFO_DOCUMENT.TYPE_PROTO;
            return result;
        }

        protected string GetLabelTotalNumberAttachment(InstanceAccessDocument document)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(document.INFO_DOCUMENT.ID_DOCUMENTO_PRINCIPALE))
            {
                if (document.ATTACHMENTS != null && document.ATTACHMENTS.Count() > 0)
                {
                    result = (from a in document.ATTACHMENTS where a.ENABLE select a).ToList().Count
                         + "/" + document.ATTACHMENTS.Count().ToString();
                }
                else
                {
                    result = "0/0";
                }
            }
            return result;
        }

        protected void GridInstanceAccessDocuments_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            string idInstanceAccessDocument = string.Empty;
            switch (e.CommandName)
            {
                case "viewLinkDocument":
                case "ViewDocument":
                    string docnumber = (((e.CommandSource as Control).Parent.Parent as GridViewRow).FindControl("lblIdInstanceDocnumber") as Label).Text;
                    #region navigation
                    List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                    Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                    actualPage.IdInstance = InstanceAccessManager.getInstanceAccessInSession().ID_INSTANCE_ACCESS;
                    actualPage.IdInstance = InstanceAccessManager.getInstanceAccessInSession().ID_INSTANCE_ACCESS;
                    //actualPage.NumPage = this.gridInstanceAccessDocuments.PageIndex.ToString();
                    //actualPage.PageSize = this.gridInstanceAccessDocuments.PageCount.ToString();

                    actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_DETAILS.ToString(), string.Empty);
                    actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_DETAILS.ToString(), true, this.Page);
                    actualPage.CodePage = Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_DETAILS.ToString();

                    actualPage.Page = "INSTANCEDETAILS.ASPX";
                    navigationList.Add(actualPage);
                    Navigation.NavigationUtils.SetNavigationList(navigationList);
                    #endregion

                    SelectedRow = (e.CommandSource as LinkButton) != null ? (((e.CommandSource as Control).Parent.Parent as GridViewRow).RowIndex + 1).ToString() :
                        ((e.CommandSource as Control).Parent.Parent as GridViewRow).RowIndex.ToString();
                    SchedaDocumento schedaDoc = UIManager.DocumentManager.getDocumentDetails(this.Page, docnumber, docnumber);
                    DocumentManager.setSelectedRecord(schedaDoc);
                    Response.Redirect("../Document/Document.aspx");
                    Response.End();
                    return;

                case "ViewDetailsDocument":
                    idInstanceAccessDocument = (((e.CommandSource as CustomImageButton).Parent.Parent as GridViewRow).FindControl("lblIdInstanceDocumentId") as Label).Text;
                    Document = (from d in InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS
                                where d.ID_INSTANCE_ACCESS_DOCUMENT.Equals(idInstanceAccessDocument)
                                select d).FirstOrDefault();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ViewDetailsInstanceAccessDocument", "ajaxModalPopupViewDetailsInstanceAccessDocument();", true);
                    break;
                case "RemoveDocument":
                    idInstanceAccessDocument = (((e.CommandSource as CustomImageButton).Parent.Parent as GridViewRow).FindControl("lblIdInstanceDocumentId") as Label).Text;
                    DocsPaWR.InstanceAccess instanceAccess = InstanceAccessManager.getInstanceAccessInSession();
                    Document = (from doc in instanceAccess.DOCUMENTS
                                                                     where doc.ID_INSTANCE_ACCESS_DOCUMENT.Equals(idInstanceAccessDocument)
                                                                     select doc).FirstOrDefault();
                    
                    //Nel caso di documento di dichiarazione di conformità il click sul cestino corrisponde ad una rimozione effettiva del documento
                    //e non solo all'interno dell'istanza.
                    if (Document.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(this.TipologiaAttoIstanza.DESCRIZIONE))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmRemoveDichiarazioneInstanceDetails', 'HiddenRemoveDichiarazione', '');", true);
                        return;
                    }
                    this.RemoveDocument();
                    break;
                case "SignatureDetails":
                    string docnumberDeclaration = (((e.CommandSource as Control).Parent.Parent as GridViewRow).FindControl("lblIdInstanceDocnumber") as Label).Text;
                    UIManager.DocumentManager.setSelectedRecord(UIManager.DocumentManager.getDocumentDetails(this.Page, docnumberDeclaration, docnumberDeclaration));
                    FileManager.setSelectedFile(UIManager.DocumentManager.getSelectedRecord().documenti[0]);
                    DocumentManager.removeSelectedNumberVersion();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDigitalSignDetails", "ajaxModalPopupDigitalSignDetails();", true);
                    break;
                case "ViewDocumentFile":
                    string docnumberDeclarationInstance = (((e.CommandSource as Control).Parent.Parent as GridViewRow).FindControl("lblIdInstanceDocnumber") as Label).Text;
                    this.IsZoom = true;
                    UIManager.DocumentManager.setSelectedRecord(UIManager.DocumentManager.getDocumentDetails(this.Page, docnumberDeclarationInstance, docnumberDeclarationInstance));
                    FileManager.setSelectedFile(UIManager.DocumentManager.getSelectedRecord().documenti[0]);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDocumentViewer", "ajaxModalPopupDocumentViewer();", true);
                    NttDataWA.Popup.DocumentViewer.OpenDocumentViewer = true;
                    break;
            }
        }

        private void RemoveDocument()
        {
            InstanceAccessDocument docToRemove = new InstanceAccessDocument();
            docToRemove = Document;
            DocsPaWR.InstanceAccess instanceAccess = InstanceAccessManager.getInstanceAccessInSession();
            if (InstanceAccessManager.RemoveInstanceAccessDocuments(new List<InstanceAccessDocument> { docToRemove }))
            {
                instanceAccess.DOCUMENTS = (instanceAccess.DOCUMENTS.Except(new InstanceAccessDocument[] { docToRemove })).ToArray();
                InstanceAccessManager.setInstanceAccessInSession(instanceAccess);
                ListDocumentFiltered.Remove(docToRemove);
                if (ListSelectedDocuments != null)
                    ListSelectedDocuments.Remove(docToRemove.ID_INSTANCE_ACCESS_DOCUMENT);
                this.GridInstanceAccessDocuments_Bind();
                this.UpnlGridInstanceAccessDocuments.Update();
                this.SelectedRow = null;
                Document = null;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorRemoveInstanceAccessDocument', 'error', '','');", true);
            }
        }

        /*
        protected void btnGridInstanceDetailsRow_Click(object sender, EventArgs e)
        {
            string docnumber = (this.gridInstanceAccessDocuments.Rows[Convert.ToInt32(this.grid_rowindex.Value)].FindControl("lblIdInstanceDocumentId") as Label).Text;
            try
            {
                Document = (from d in InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS
                            where d.ID_INSTANCE_ACCESS_DOCUMENT.Equals(docnumber)
                            select d).FirstOrDefault();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ViewDetailsInstanceAccessDocument", "ajaxModalPopupViewDetailsInstanceAccessDocument();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
        */

        protected void ChkSelectedAllDocuments_CheckedChanged(object sender, EventArgs e)
        {
            if ((gridInstanceAccessDocuments.HeaderRow.FindControl("ChkSelectedAllDocuments") as CheckBox).Checked)
            {
                ListSelectedDocuments = new List<string>();
                ListSelectedDocuments = (from d in InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS select d.ID_INSTANCE_ACCESS_DOCUMENT).ToList();
                foreach (GridViewRow row in gridInstanceAccessDocuments.Rows)
                {
                    (row.FindControl("ChkSelectedDocument") as CheckBox).Checked = true;
                }
                CheckAll = true;
                this.HiddenItemsAll.Value = "true";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "clearCheckboxes", "clearCheckboxes('true', '');", true);
            }
            else
            {
                ListSelectedDocuments = new List<string>();
                foreach (GridViewRow row in gridInstanceAccessDocuments.Rows)
                {
                    (row.FindControl("ChkSelectedDocument") as CheckBox).Checked = false;
                }
                CheckAll = false;
                this.HiddenItemsAll.Value = string.Empty;
            }
            this.upPnlButtons.Update();
            BuildGridNavigator();
            this.UpnlGridInstanceAccessDocuments.Update();
        }


        protected void DdlRequest_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DropDownList ddlRequest = sender as DropDownList;
                InstanceAccessDocument document = new InstanceAccessDocument();
                string idInstanceDocumentId = ((ddlRequest.NamingContainer as GridViewRow).FindControl("lblIdInstanceDocumentId") as Label).Text;
                foreach (InstanceAccessDocument doc in InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS)
                {
                    if (doc.ID_INSTANCE_ACCESS_DOCUMENT.Equals(idInstanceDocumentId))
                    {
                        string oldValue = doc.TYPE_REQUEST;
                        switch (ddlRequest.SelectedValue)
                        {
                            case "0":
                                doc.TYPE_REQUEST = InstanceAccessManager.TipoRichiesta.COPIA_SEMPLICE;
                                break;
                            case "1":
                                doc.TYPE_REQUEST = InstanceAccessManager.TipoRichiesta.COPIA_CONFORME;
                                break;
                            case "2":
                                doc.TYPE_REQUEST = InstanceAccessManager.TipoRichiesta.ESTRATTO;
                                break;
                            case "3":
                                doc.TYPE_REQUEST = InstanceAccessManager.TipoRichiesta.DUPLCATO;
                                break;
                        }
                        if (!InstanceAccessManager.UpdateInstanceAccessDocuments(new List<InstanceAccessDocument>() { doc }))
                        {
                            doc.TYPE_REQUEST = oldValue;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorInstanceAccessUpdateInstance', 'error', '');", true);
                        }
                    }
                }
                if ((from doc in InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS where doc.TYPE_REQUEST.Equals(InstanceAccessManager.TipoRichiesta.COPIA_CONFORME) select doc).FirstOrDefault() != null)
                {
                    this.InstanceDetailsCreate.Enabled = true;
                }
                else
                {
                    this.InstanceDetailsCreate.Enabled = false;
                }
                this.upPnlButtons.Update();
                this.BuildGridNavigator();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        /// <summary>
        /// Crea il documento di dichiarazione di conformità
        /// </summary>
        private void CreateDeclaration()
        {
            DocsPaWR.InstanceAccess instance = null;
            try
            {
                instance = InstanceAccessManager.CreateDeclarationDocument();
                if (instance != null)
                {
                    InstanceAccessManager.setInstanceAccessInSession(instance);
                    this.ListDocumentFiltered.Clear();
                    this.ListDocumentFiltered.AddRange(instance.DOCUMENTS.ToList());
                    this.projectImgRemoveFilter.Enabled = false;
                    SearchFilters = null;
                    SelectedPage = 1;
                    GridInstanceAccessDocuments_Bind();
                    this.UpnlGridInstanceAccessDocuments.Update();
                    this.UpnlButtonTop.Update();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorInstanceDetailsCreateDeclarationDocument', 'error', '');", true);
                    return;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// Imposta la visibilità e l'abilitazione dei bottoni in base allo stato di download dell'istanza
        /// </summary>
        private void RefreshInstanceStateDownload()
        {
            DocsPaWR.InstanceAccess instance = UIManager.InstanceAccessManager.getInstanceAccessInSession();
            if (instance.STATE_DOWNLOAD_FORWARD == '0')
            {
                this.InstanceDownload.Visible = false;
                this.InstancePrepareDownloadUpdate.Visible = false;
                this.InstancePrepareDownload.Enabled = true;
                //this.InstancePrepareDownload.Visible = true;
            }

            if (instance.STATE_DOWNLOAD_FORWARD == '1')
            {
                this.InstanceDownload.Visible = false;
                //this.InstancePrepareDownload.Visible = true;
                this.InstancePrepareDownload.Enabled = false;
                this.InstancePrepareDownloadUpdate.Visible = true;
            }

            if (instance.STATE_DOWNLOAD_FORWARD == '2')
            {
                this.InstanceDownload.Visible = true;
                //this.InstancePrepareDownload.Visible = true;
                this.InstancePrepareDownload.Enabled = true;
                this.InstancePrepareDownloadUpdate.Visible = false;
            }
            this.upPnlButtons.Update();
        }

        /// <summary>
        /// Imposta il tipo di richiesta dei documenti, la cui check è fleggata, con il valore fornito in input 
        /// </summary>
        /// <param name="typeRequest"></param>
        private void UpdateTypeRequestSelectedDocument(string typeRequest)
        {
            InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS.Where(d => ListSelectedDocuments.Contains(d.ID_INSTANCE_ACCESS_DOCUMENT)).ToList().ForEach(c => c.TYPE_REQUEST = typeRequest);
            if (InstanceAccessManager.UpdateInstanceAccessDocuments(InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS.ToList()))
            {
                ApplyFilters();
                GridInstanceAccessDocuments_Bind();
                this.UpnlGridInstanceAccessDocuments.Update();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorInstanceAccessUpdateInstanceDocument', 'error', '');", true);
            }
        }

        /// <summary>
        /// Popola i campi della maschera a sinistra
        /// </summary>
        private void PopulateFields()
        {
            DocsPaWR.InstanceAccess instance = InstanceAccessManager.getInstanceAccessInSession();
            if (instance != null)
            {
                this.projectLblCodiceGenerato.Text = instance.ID_INSTANCE_ACCESS;
                this.projectLblDataAperturaGenerata.Text = instance.CREATION_DATE.ToShortDateString();
                this.projectTxtDescrizione.Text = instance.DESCRIPTION;
                if (instance.RICHIEDENTE != null)
                {
                    this.idRichiedente.Value = instance.RICHIEDENTE.systemId;
                    Corrispondente corr = AddressBookManager.GetCorrespondentBySystemId(instance.RICHIEDENTE.systemId);
                    this.txtCodiceRichiedente.Text = corr.codiceRubrica;
                    this.txtDescrizioneRichiedente.Text = corr.descrizione;
                    CorrRichiedente = corr;
                }
                this.dtaRequest_TxtFrom.Text = instance.REQUEST_DATE.Equals(DateTime.MinValue) ? string.Empty : instance.REQUEST_DATE.ToShortDateString();

                if (!instance.CLOSE_DATE.Equals(DateTime.MinValue))
                {
                    this.projectlblDataChiusuraGenerata.Text = instance.CLOSE_DATE.ToShortDateString();
                }
                else
                {
                    this.InstancelblDataChiusura.Visible = false;
                }

                if (!string.IsNullOrEmpty(instance.ID_DOCUMENT_REQUEST))
                {
                    InfoDocumento infoDoc = DocumentManager.GetInfoDocumento(instance.ID_DOCUMENT_REQUEST, instance.ID_DOCUMENT_REQUEST, this.Page);
                    if (infoDoc != null && !string.IsNullOrEmpty(infoDoc.segnatura))
                    {
                        this.TxtProtoRequest.Text = infoDoc.segnatura + " " + CutValue(infoDoc.oggetto);
                    }
                    else
                    {
                        this.TxtProtoRequest.Text = infoDoc.idProfile + " " + CutValue(infoDoc.oggetto);
                    }
                    this.idProtoRequest.Value = infoDoc.docNumber;
                }
                this.TxtNote.Text = instance.NOTE;
                this.projectImgAddDoc.Enabled = true;
            }
        }

        private void EnabledButtons(bool enable)
        {
            this.InstanceDetailsCreate.Enabled = enable;
            this.projectImgAddDoc.Enabled = enable;
            this.projectImgAddFilter.Enabled = enable;
            this.lblElencoDocumenti.Visible = enable;
            this.InstanceImgAddPrj.Enabled = enable;
            this.InstanceDetailsSave.Enabled = true;
            
            if (ListDocumentFiltered == null || ListDocumentFiltered.Count == 0)
            {
                this.InstanceDdlMassiveOperation.Enabled = false;
            }
            else
            {
                this.InstanceDdlMassiveOperation.Enabled = enable;
            }

            DocsPaWR.InstanceAccess instanceAccess = InstanceAccessManager.getInstanceAccessInSession();
            if (instanceAccess != null && instanceAccess.DOCUMENTS != null && instanceAccess.DOCUMENTS.Count() > 0)
            {
                this.InstanceDetailsCreate.Visible = enable;
                if ((from doc in instanceAccess.DOCUMENTS where doc.TYPE_REQUEST.Equals(InstanceAccessManager.TipoRichiesta.COPIA_CONFORME) select doc).FirstOrDefault() != null)
                {
                    this.InstanceDetailsCreate.Enabled = true;
                }
                else
                {
                    this.InstanceDetailsCreate.Enabled = false;
                }
                if(this.EsisteDichiarazioneConformita)
                {
                    this.InstanceDetailsSave.Enabled = false;
                    this.projectImgAddDoc.Enabled = false;
                    this.InstanceImgAddPrj.Enabled = false;
                    this.InstanceDdlMassiveOperation.Enabled = false;
                }
                this.InstancePrepareDownload.Visible = enable;
                this.InstancePrepareDownload.Enabled = enable;
                this.InstanceForward.Visible = enable;
            }
            else
            {
                this.InstanceDownload.Visible = false;
                this.InstancePrepareDownload.Visible = false;
                this.InstancePrepareDownloadUpdate.Visible = false;
                this.InstanceForward.Visible = false;
                this.InstanceDetailsCreate.Visible = false;
            }
        }

        private List<InstanceAccessDocument> BuildObjectInstanceAccessDocuments()
        {
            List<InstanceAccessDocument> listInstanceAccessDocument = new List<InstanceAccessDocument>();
            if (ListDocs != null && ListDocs.Count > 0)
            {
                listInstanceAccessDocument = (from doc in ListDocs
                                              where !((from d in InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS
                                                       where (d.DOCNUMBER.Equals(doc.docNumber) && d.INFO_PROJECT == null)
                                                       select d.INFO_DOCUMENT).Count() > 0)
                                              select new InstanceAccessDocument
                                              {
                                                  ID_INSTANCE_ACCESS = InstanceAccessManager.getInstanceAccessInSession().ID_INSTANCE_ACCESS,
                                                  DOCNUMBER = doc.docNumber,
                                                  ATTACHMENTS = BuildObjectAttach(doc),
                                                  ENABLE = true,
                                                  TYPE_REQUEST = (doc.tipologiaAtto != null && this.TipologiaAttoIstanza != null &&
                                                        doc.tipologiaAtto.descrizione.Equals(this.TipologiaAttoIstanza.DESCRIZIONE)) ? string.Empty : InstanceAccessManager.TipoRichiesta.COPIA_SEMPLICE,
                                                  INFO_DOCUMENT = new InfoDocument()
                                                  {
                                                      DOCNUMBER = doc.docNumber,
                                                      DESCRIPTION_TIPOLOGIA_ATTO = doc.tipologiaAtto != null ? doc.tipologiaAtto.descrizione : string.Empty
                                                  }
                                              }).ToList();

            }
            return listInstanceAccessDocument;
        }

        private string CutValue(string value)
        {
            if (value.Length < 20) return value;
            int firstSpacePos = value.IndexOf(' ', 20);
            if (firstSpacePos == -1) firstSpacePos = 20;
            return value.Substring(0, firstSpacePos) + "...";
        }

        private InstanceAccessAttachments[] BuildObjectAttach(SchedaDocumento schDoc)
        {
            List<InstanceAccessAttachments> listInstanceAccessAttachments = new List<InstanceAccessAttachments>();
            if (schDoc.allegati != null && schDoc.allegati.Count() > 0)
            {
                foreach (Allegato all in schDoc.allegati)
                {
                    if (all.TypeAttachment == 1 || all.TypeAttachment == 4)
                    {
                        listInstanceAccessAttachments.Add(new InstanceAccessAttachments { ID_ATTACH = all.docNumber, ENABLE = true });
                    }
                    else
                    {
                        listInstanceAccessAttachments.Add(new InstanceAccessAttachments { ID_ATTACH = all.docNumber, ENABLE = false });
                    }
                }
            }
            return listInstanceAccessAttachments.ToArray();
        }

        /// <summary>
        /// Applica i filtri impostati dalla popup
        /// </summary>
        private void ApplyFilters()
        {
            List<InstanceAccessDocument> documents = new List<InstanceAccessDocument>();
            if (InstanceAccessManager.getInstanceAccessInSession() != null && InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS != null)
            {
                documents = InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS.ToList();
            }
            ListDocumentFiltered = documents;
            if (SearchFilters != null)
            {
                ListDocumentFiltered = new List<InstanceAccessDocument>();
                List<InstanceAccessDocument> listaAppoggio = new List<InstanceAccessDocument>();
                List<InstanceAccessDocument> listTypeRequestAppoggio = new List<InstanceAccessDocument>();
                bool aggiorna = false;
                foreach (DocsPaWR.FiltroRicerca item in SearchFilters[0])
                {
                    listaAppoggio.Clear();
                    #region filtro tipo documento
                    if (item.argomento == DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString() && Convert.ToBoolean(item.valore))
                    {
                        listaAppoggio.AddRange(from d in documents
                                               where d.INFO_DOCUMENT.TYPE_PROTO.Equals("A")
                                               select d);
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }
                    if (item.argomento == DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString() && Convert.ToBoolean(item.valore))
                    {
                        listaAppoggio.AddRange(from d in documents
                                               where d.INFO_DOCUMENT.TYPE_PROTO.Equals("P")
                                               select d);
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }
                    if (item.argomento == DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString() && Convert.ToBoolean(item.valore))
                    {
                        listaAppoggio.AddRange(from d in documents
                                               where d.INFO_DOCUMENT.TYPE_PROTO.Equals("I")
                                               select d);
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }
                    if (item.argomento == DocsPaWR.FiltriDocumento.GRIGIO.ToString() && Convert.ToBoolean(item.valore))
                    {
                        listaAppoggio.AddRange(from d in documents
                                               where d.INFO_DOCUMENT.TYPE_PROTO.Equals("G") && string.IsNullOrEmpty(d.INFO_DOCUMENT.ID_DOCUMENTO_PRINCIPALE)
                                               select d);
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }
                    if (item.argomento == DocsPaWR.FiltriDocumento.ALLEGATO.ToString())
                    {
                        if (item.valore.ToString().Equals(TUTTI))
                        {
                            listaAppoggio.AddRange(from d in documents
                                                   where !string.IsNullOrEmpty(d.INFO_DOCUMENT.ID_DOCUMENTO_PRINCIPALE)
                                                   select d);
                        }
                        else
                        {
                            listaAppoggio.AddRange(from d in documents
                                                   where !string.IsNullOrEmpty(d.INFO_DOCUMENT.ID_DOCUMENTO_PRINCIPALE) && d.INFO_DOCUMENT.TYPE_ATTACH.ToString().Equals(item.valore.ToString())
                                                   select d);
                        }
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }
                    if (item.argomento == DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString() && Convert.ToBoolean(item.valore))
                    {
                        listaAppoggio.AddRange(from d in documents
                                               where !d.INFO_DOCUMENT.TYPE_PROTO.Equals("G") && !d.INFO_DOCUMENT.TYPE_PROTO.Equals("ALL") && string.IsNullOrEmpty(d.INFO_DOCUMENT.NUMBER_PROTO)
                                               select d);
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }
                    #endregion
                    #region filtro oggetto
                    if (item.argomento == DocsPaWR.FiltriDocumento.OGGETTO.ToString())
                    {
                        listaAppoggio.AddRange(from d in ListDocumentFiltered
                                               where d.INFO_DOCUMENT.OBJECT.ToLower().Contains(item.valore.ToString().ToLower())
                                               select d);
                        ListDocumentFiltered.Clear();
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }

                    #endregion
                    #region filtro numero protocollo

                    if (item.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString())
                    {
                        listaAppoggio.AddRange(from d in ListDocumentFiltered
                                               where !string.IsNullOrEmpty(d.INFO_DOCUMENT.NUMBER_PROTO) && d.INFO_DOCUMENT.NUMBER_PROTO.Equals(item.valore.ToString())
                                               select d);
                        ListDocumentFiltered.Clear();
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }

                    if (item.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString())
                    {
                        listaAppoggio.AddRange(from d in ListDocumentFiltered
                                               where !string.IsNullOrEmpty(d.INFO_DOCUMENT.NUMBER_PROTO) && Convert.ToInt32(d.INFO_DOCUMENT.NUMBER_PROTO) >= Convert.ToInt32(item.valore)
                                               select d);
                        ListDocumentFiltered.Clear();
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }

                    if (item.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString())
                    {
                        listaAppoggio.AddRange(from d in ListDocumentFiltered
                                               where !string.IsNullOrEmpty(d.INFO_DOCUMENT.NUMBER_PROTO) && Convert.ToInt32(d.INFO_DOCUMENT.NUMBER_PROTO) <= Convert.ToInt32(item.valore)
                                               select d);
                        ListDocumentFiltered.Clear();
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }
                    #endregion
                    #region filtro mitt/dest
                    if (item.argomento == DocsPaWR.FiltriDocumento.MITT_DEST.ToString())
                    {
                        listaAppoggio.AddRange(from d in ListDocumentFiltered
                                               where d.INFO_DOCUMENT.MITT_DEST.Contains(item.valore)
                                               select d);
                        ListDocumentFiltered.Clear();
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }
                    #endregion
                    #region filtro registro
                    if (item.argomento == DocsPaWR.FiltriDocumento.REGISTRO.ToString())
                    {
                        listaAppoggio.AddRange(from d in ListDocumentFiltered
                                               where d.INFO_DOCUMENT.REGISTER.Equals(item.valore)
                                               select d);
                        ListDocumentFiltered.Clear();
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }
                    #endregion
                    #region filtro id documento

                    if (item.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER.ToString())
                    {
                        listaAppoggio.AddRange(from d in ListDocumentFiltered
                                               where !string.IsNullOrEmpty(d.INFO_DOCUMENT.DOCNUMBER) && d.INFO_DOCUMENT.DOCNUMBER.Equals(item.valore.ToString())
                                               select d);
                        ListDocumentFiltered.Clear();
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }

                    if (item.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString())
                    {
                        listaAppoggio.AddRange(from d in ListDocumentFiltered
                                               where !string.IsNullOrEmpty(d.INFO_DOCUMENT.DOCNUMBER) && Convert.ToInt32(d.INFO_DOCUMENT.DOCNUMBER) >= Convert.ToInt32(item.valore)
                                               select d);
                        ListDocumentFiltered.Clear();
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }

                    if (item.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString())
                    {
                        listaAppoggio.AddRange(from d in ListDocumentFiltered
                                               where !string.IsNullOrEmpty(d.INFO_DOCUMENT.DOCNUMBER) && Convert.ToInt32(d.INFO_DOCUMENT.DOCNUMBER) <= Convert.ToInt32(item.valore)
                                               select d);
                        ListDocumentFiltered.Clear();
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }
                    #endregion
                    #region filtro repertorio
                    if (item.argomento == "TIPOLOGIA_DOCUMENTO")
                    {
                        listaAppoggio.AddRange(from d in ListDocumentFiltered
                                               where d.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(item.valore)
                                               select d);
                        ListDocumentFiltered.Clear();
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }
                    if (item.argomento == "NUMERO_CONTATORE")
                    {
                        listaAppoggio.AddRange(from d in ListDocumentFiltered
                                               where d.INFO_DOCUMENT.COUNTER_REPERTORY.Equals(item.valore)
                                               select d);
                        ListDocumentFiltered.Clear();
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }
                    #endregion
                    #region filtro codice fascicolo
                    if (item.argomento == DocsPaWR.FiltriDocumento.CODICE_FASCICOLO.ToString())
                    {
                        listaAppoggio.AddRange(from d in ListDocumentFiltered
                                               where (d.INFO_PROJECT != null && (d.INFO_PROJECT.CODE_PROJECT.Equals(item.valore)))
                                               select d);
                        ListDocumentFiltered.Clear();
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }
                    if (item.argomento.Equals("DESCRIZIONE_FASCICOLO"))
                    {
                        listaAppoggio.AddRange(from d in ListDocumentFiltered
                                               where (d.INFO_PROJECT != null && d.INFO_PROJECT.DESCRIPTION_PROJECT.Equals(item.valore))
                                               select d);
                        ListDocumentFiltered.Clear();
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }
                    #endregion
                    #region filtro numero allegati

                    if (item.argomento == DocsPaWR.FiltriDocumento.NUMERO_ALLEGATI.ToString())
                    {
                        string operatore = item.valore.Substring(0, 1);
                        int numAttach = Convert.ToInt32(item.valore.Substring(1, item.valore.Length - 1));
                        switch (operatore)
                        {
                            case ">":
                                listaAppoggio.AddRange(from d in ListDocumentFiltered
                                                       where (d.ATTACHMENTS != null && d.ATTACHMENTS.Count() > numAttach)
                                                       select d);
                                break;
                            case "<":
                                listaAppoggio.AddRange(from d in ListDocumentFiltered
                                                       where (d.ATTACHMENTS != null && d.ATTACHMENTS.Count() < numAttach)
                                                       select d);
                                break;
                            case "=":
                                listaAppoggio.AddRange(from d in ListDocumentFiltered
                                                       where (d.ATTACHMENTS != null && d.ATTACHMENTS.Count() == numAttach)
                                                       select d);
                                break;
                        }

                        ListDocumentFiltered.Clear();
                        ListDocumentFiltered.AddRange(listaAppoggio);
                    }

                    #endregion
                    #region filtro tipo richiesta
                    if (item.argomento == InstanceAccessManager.TipoRichiesta.COPIA_CONFORME.ToString() && Convert.ToBoolean(item.valore))
                    {
                        aggiorna = true;
                        listTypeRequestAppoggio.AddRange(from d in ListDocumentFiltered
                                                         where (d.TYPE_REQUEST.Equals(InstanceAccessManager.TipoRichiesta.COPIA_CONFORME.ToString()))
                                                         select d);
                    }
                    if (item.argomento == InstanceAccessManager.TipoRichiesta.COPIA_SEMPLICE.ToString() && Convert.ToBoolean(item.valore))
                    {
                        aggiorna = true;
                        listTypeRequestAppoggio.AddRange(from d in ListDocumentFiltered
                                                         where (d.TYPE_REQUEST.Equals(InstanceAccessManager.TipoRichiesta.COPIA_SEMPLICE.ToString()))
                                                         select d);
                    }
                    if (item.argomento == InstanceAccessManager.TipoRichiesta.ESTRATTO.ToString() && Convert.ToBoolean(item.valore))
                    {
                        aggiorna = true;
                        listTypeRequestAppoggio.AddRange(from d in ListDocumentFiltered
                                                         where (d.TYPE_REQUEST.Equals(InstanceAccessManager.TipoRichiesta.ESTRATTO.ToString()))
                                                         select d);
                    }
                    if (item.argomento == InstanceAccessManager.TipoRichiesta.DUPLCATO.ToString() && Convert.ToBoolean(item.valore))
                    {
                        aggiorna = true;
                        listTypeRequestAppoggio.AddRange(from d in ListDocumentFiltered
                                                         where (d.TYPE_REQUEST.Equals(InstanceAccessManager.TipoRichiesta.DUPLCATO.ToString()))
                                                         select d);
                    }
                    #endregion

                }
                if (aggiorna)
                {
                    ListDocumentFiltered.Clear();
                    ListDocumentFiltered.AddRange(listTypeRequestAppoggio);
                }

            }
        }

        private void SetCheckBox()
        {
            try
            {
                bool checkAll = this.CheckAll;

                if (!string.IsNullOrEmpty(this.HiddenItemsChecked.Value))
                {
                    //salvo i check spuntati alla pagina cliccata in precedenza
                    string[] items = new string[1] { this.HiddenItemsChecked.Value };
                    if (this.HiddenItemsChecked.Value.IndexOf(",") > 0)
                        items = this.HiddenItemsChecked.Value.Split(',');
                    if (ListSelectedDocuments == null)
                        ListSelectedDocuments = new List<string>();
                    foreach (string item in items)
                    {
                        string key = item.Split('_')[0];
                        if (!this.ListSelectedDocuments.Contains(key))
                        {
                            this.ListSelectedDocuments.Add(key);
                        }
                    }
                }


                if (!string.IsNullOrEmpty(this.HiddenItemsUnchecked.Value))
                {
                    this.CheckAll = false;

                    // salvo i check non spuntati alla pagina cliccata in precedenza
                    string[] items = new string[1] { this.HiddenItemsUnchecked.Value };
                    if (this.HiddenItemsUnchecked.Value.IndexOf(",") > 0)
                        items = this.HiddenItemsUnchecked.Value.Split(',');

                    foreach (string item in items)
                    {
                        string key = item.Split('_')[0];
                        if (this.ListSelectedDocuments.Contains(key))
                            this.ListSelectedDocuments.Remove(key);
                    }
                }
                this.HiddenItemsChecked.Value = string.Empty;
                this.HiddenItemsUnchecked.Value = string.Empty;
                this.upPnlButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void LoadMassiveOperation()
        {
            this.InstanceDdlMassiveOperation.Items.Add(new ListItem("", ""));
            string title = string.Empty;
            string language = UIManager.UserManager.GetUserLanguage();

            title = Utils.Languages.GetLabelFromCode("InstanceMassiveRemoveDocumentsTitle", language);
            this.InstanceDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_REMOVE_DOCUMENTS"));

            title = Utils.Languages.GetLabelFromCode("InstanceMassiveRequestCopiaConformeTitle", language);
            this.InstanceDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_REQUEST_COPIA_CONFORME"));

            title = Utils.Languages.GetLabelFromCode("InstanceMassiveRequestAutenticaTitle", language);
            this.InstanceDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_REQUEST_AUTENTICA"));

            title = Utils.Languages.GetLabelFromCode("InstanceMassiveRequestEstrattoTitle", language);
            this.InstanceDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_REQUEST_ESTRATTO"));

            title = Utils.Languages.GetLabelFromCode("InstanceMassiveRequestDuplicatoTitle", language);
            this.InstanceDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_REQUEST_DUPLICATO"));
        }

        private void RemoveDocuments()
        {
            try
            {
                List<InstanceAccessDocument> listDocumentsToRemove = (from d in InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS.ToList()
                                                                      where (from s in ListSelectedDocuments
                                                                             where s.Equals(d.ID_INSTANCE_ACCESS_DOCUMENT)
                                                                             select s).FirstOrDefault() != null
                                                                      select d).ToList();
                if (InstanceAccessManager.RemoveInstanceAccessDocuments(listDocumentsToRemove))
                {
                    List<InstanceAccessDocument> documents = InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS.ToList();
                    InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS = documents.Except(listDocumentsToRemove).ToArray();
                    ListDocumentFiltered = ListDocumentFiltered.Except(listDocumentsToRemove).ToList();
                    ListSelectedDocuments.RemoveAll(s => listDocumentsToRemove.Find(d => d.ID_INSTANCE_ACCESS_DOCUMENT.Equals(s)) != null);
                    CheckAll = false;
                    (this.gridInstanceAccessDocuments.HeaderRow.FindControl("ChkSelectedAllDocuments") as CheckBox).Checked = false;
                    GridInstanceAccessDocuments_Bind();
                    this.UpnlGridInstanceAccessDocuments.Update();
                    this.SelectedRow = null;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorInstanceMassiveOperationRemoveDocuments', 'error', '');", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /*
        protected void ImgRemoveNotify_Click(object sender, ImageClickEventArgs e)
        {
            CustomImageButton btnIm = (CustomImageButton)sender;
            GridViewRow row = (GridViewRow)btnIm.Parent.Parent;

            string idInstanceAccessDocument = (row.FindControl("lblIdInstanceDocumentId") as Label).Text;
            DocsPaWR.InstanceAccess instanceAccess = InstanceAccessManager.getInstanceAccessInSession();
            Document = (from doc in instanceAccess.DOCUMENTS
                                                             where doc.ID_INSTANCE_ACCESS_DOCUMENT.Equals(idInstanceAccessDocument)
                                                             select doc).FirstOrDefault();
            //Nel caso di documento di dichiarazione di conformità il click sul cestino corrisponde ad una rimozione effettiva del documento
            //e non solo all'interno dell'istanza.
            if (Document.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(this.TipologiaAttoIstanza.DESCRIZIONE))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmRemoveDichiarazioneInstanceDetails', 'HiddenRemoveDichiarazione', '');", true);
                return;
            }
            this.RemoveDocument();
        }
        */
        private string SetStateInstanceDocument(InstanceAccessDocument doc)
        {
            string result = string.Empty;
            int numberDocEnabled = 0;
            int numberDocWithFile = 0;
            int numberoDocNotEnabledWithFile = 0;
            // if (doc.ENABLE)

            if (doc.ATTACHMENTS != null && doc.ATTACHMENTS.Count() > 0)
            {
                numberoDocNotEnabledWithFile = (from a in doc.ATTACHMENTS where a.ENABLE && string.IsNullOrEmpty(a.FILE_NAME) select a).ToList().Count();
                numberDocWithFile = (from a in doc.ATTACHMENTS where !string.IsNullOrEmpty(a.FILE_NAME) select a).ToList().Count();
                numberDocEnabled = (from a in doc.ATTACHMENTS where a.ENABLE && !string.IsNullOrEmpty(a.FILE_NAME) select a).ToList().Count();
                if (numberDocEnabled == 0)
                    result = @"~/Images/Icons/ico_error_att.png";
                else if(numberoDocNotEnabledWithFile>0)
                    result = @"~/Images/Icons/ico_warning_att.png";
                else if (numberDocEnabled == numberDocWithFile)
                    result = @"~/Images/Icons/ico_ok_att.png";
                else
                    result = @"~/Images/Icons/ico_warning_att.png";
            }
            else
            {
                result = @"~/Images/Icons/ico_no_present_att.png";
            }


            return result;
        }

        private string SetStateInstanceDocumentWar(InstanceAccessDocument doc)
        {
            string language = UIManager.UserManager.GetUserLanguage();


            string result = string.Empty;
            int numberDocEnabled = 0;
            int numberDocWithFile = 0;
            int numberoDocNotEnabledWithFile = 0;
            // if (doc.ENABLE)

            if (doc.ATTACHMENTS != null && doc.ATTACHMENTS.Count() > 0)
            {
                numberoDocNotEnabledWithFile = (from a in doc.ATTACHMENTS where a.ENABLE && string.IsNullOrEmpty(a.FILE_NAME) select a).ToList().Count();
                numberDocWithFile = (from a in doc.ATTACHMENTS where !string.IsNullOrEmpty(a.FILE_NAME) select a).ToList().Count();
                numberDocEnabled = (from a in doc.ATTACHMENTS where a.ENABLE && !string.IsNullOrEmpty(a.FILE_NAME) select a).ToList().Count();
                if (numberDocEnabled == 0)
                    result = Utils.Languages.GetLabelFromCode("IntanceDocumentTooltipImgAttErr", language);
                else if (numberoDocNotEnabledWithFile > 0)
                    result = Utils.Languages.GetLabelFromCode("IntanceDocumentTooltipImgWarErr", language);
                else if (numberDocEnabled == numberDocWithFile)
                    result = Utils.Languages.GetLabelFromCode("IntanceDocumentTooltipImgOkErr", language);
                else
                    result = Utils.Languages.GetLabelFromCode("IntanceDocumentTooltipImgWarErr", language);
            }
            else
            {
                result = Utils.Languages.GetLabelFromCode("IntanceDocumentTooltipImgNoPresent", language);
            }

            return result;
        }

        private string StateInstanceDocPrincipale(InstanceAccessDocument doc)
        {
            if (doc.INFO_DOCUMENT != null && !string.IsNullOrEmpty(doc.INFO_DOCUMENT.FILE_NAME))
            {
                if (doc.ENABLE)
                    return @"~/Images/Icons/ico_main.png";
                else
                    return @"~/Images/Icons/ico_no_main.png";
            }
            else
            {
                return @"~/Images/Icons/ico_no_main.png";
            }
        }

        private string SetIntanceDocumentTooltip(InstanceAccessDocument doc)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            if (doc.INFO_DOCUMENT != null && !string.IsNullOrEmpty(doc.INFO_DOCUMENT.FILE_NAME))
            {
                if (doc.ENABLE)
                    return Utils.Languages.GetLabelFromCode("IntanceDocumentTooltipImgMain", language);
                else
                    return Utils.Languages.GetLabelFromCode("IntanceDocumentTooltipImgNoMain", language);
            }
            else
            {
                return Utils.Languages.GetLabelFromCode("IntanceDocumentTooltipImgNoPresentMain", language);
            }
        }


        private void TotalNumberFile()
        {
            int num = 0;
            DocsPaWR.InstanceAccess instance = InstanceAccessManager.getInstanceAccessInSession();
            if (instance.DOCUMENTS != null)
            {
                foreach (InstanceAccessDocument doc in instance.DOCUMENTS)
                {
                    if (doc.ENABLE)
                        num++;
                    if(this.TipologiaAttoIstanza == null || !doc.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(this.TipologiaAttoIstanza.DESCRIZIONE))
                        num += (from a in doc.ATTACHMENTS where a.ENABLE select a).ToList().Count();
                }
                this.lblNumeroTotaleFile.Text = Utils.Languages.GetLabelFromCode("InstanceDetailsLblNumeroTotaleFile", UserManager.GetUserLanguage()).Replace("@@", num.ToString());
            }
        }

        /// <summary>
        /// Risultati restituiti dalla ricerca.
        /// </summary>
        public List<SchedaDocumento> ListDocs
        {
            get
            {
                return HttpContext.Current.Session["listDocs"] as List<SchedaDocumento>;
            }
            set
            {
                HttpContext.Current.Session["listDocs"] = value;
            }
        }

        public List<InstanceAccessDocument> ListDocumentsAccess
        {
            get
            {
                List<InstanceAccessDocument> result = new List<InstanceAccessDocument>();
                if (HttpContext.Current.Session["listDocumentsAccess"] != null)
                {
                    result = HttpContext.Current.Session["listDocumentsAccess"] as List<InstanceAccessDocument>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listDocumentsAccess"] = value;
            }
        }

    }
}
