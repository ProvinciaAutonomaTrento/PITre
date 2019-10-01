using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using System.Data;
using System.Text;
using NttDatalLibrary;
using System.Web.UI.HtmlControls;
using NttDataWA.UserControls;
using System.Collections;
using System.Text.RegularExpressions;


namespace NttDataWA.Search
{
    public partial class SearchDocumentAdvanced : System.Web.UI.Page
    {

        #region fields

        protected DocsPaWR.RagioneTrasmissione[] listaRagioni;
        protected Hashtable m_hashTableRagioneTrasmissione;

        private const string KEY_SCHEDA_RICERCA = "RicercaDocEstesa";
        private const string UP_DOCUMENT_BUTTONS = "upPnlButtons";
        private const string CLOSE_POPUP_ZOOM = "closeZoom";
        public SearchManager schedaRicerca = null;

        public static string componentType = Constans.TYPE_SMARTCLIENT;

        private const string TYPE_EXT = "esterni";
        private const string TYPE_PITRE = "SIMPLIFIEDINTEROPERABILITY";

        #endregion

        #region properties

        private SearchObject[] ListObjectNavigation
        {
            set
            {
                HttpContext.Current.Session["ListObjectNavigation"] = value;
            }
        }

        private void RemoveIsZoom()
        {
            HttpContext.Current.Session.Remove("isZoom");
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

        protected int MaxLenghtObject
        {
            get
            {
                int result = 2000;
                if (HttpContext.Current.Session["maxLenghtObject"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["maxLenghtObject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["maxLenghtObject"] = value;
            }
        }

        private bool ShowGridPersonalization
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["showGridPersonalization"] != null)
                {
                    return (bool)HttpContext.Current.Session["showGridPersonalization"];
                }
                return result;

            }
            set
            {
                HttpContext.Current.Session["showGridPersonalization"] = value;
            }
        }

        private bool IsEnabledProfilazioneAllegato
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["isEnabledProfilazioneAllegato"] != null)
                {
                    return (bool)HttpContext.Current.Session["isEnabledProfilazioneAllegato"];
                }
                return result;

            }
            set
            {
                HttpContext.Current.Session["isEnabledProfilazioneAllegato"] = value;
            }
        }

        // INTEGRAZIONE PITRE-PARER
        // Se true, è attivo l'invio in conservazione al sistema SACER
        // Se false, è attivo l'invio in conservazione al Centro Servizi
        private bool IsConservazioneSACER
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["isConservazioneSACER"] != null)
                {
                    return (bool)HttpContext.Current.Session["isConservazioneSACER"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["isConservazioneSACER"] = value;
            }
        }



        private List<EtichettaInfo> Labels
        {
            get
            {
                return (List<EtichettaInfo>)HttpContext.Current.Session["Labels"];

            }
            set
            {
                HttpContext.Current.Session["Labels"] = value;
            }
        }

        /// <summary>
        /// Number of result in page
        /// </summary>
        public int PageSize
        {
            get
            {
                int result = 20;
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

        /// <summary>
        /// Risultati restituiti dalla ricerca.
        /// </summary>
        public SearchObject[] Result
        {
            get
            {
                return HttpContext.Current.Session["result"] as SearchObject[];
            }
            set
            {
                HttpContext.Current.Session["result"] = value;
            }
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

        private int RecordCount
        {
            get
            {
                int toReturn = 0;
                if (HttpContext.Current.Session["recordCount"] != null) Int32.TryParse(HttpContext.Current.Session["recordCount"].ToString(), out toReturn);
                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["recordCount"] = value;
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

        /// <summary>
        /// Numero di pagine restituiti dalla ricerca
        /// </summary>
        public int PageCount
        {
            get
            {
                int toReturn = 1;

                if (HttpContext.Current.Session["PageCount"] != null)
                {
                    Int32.TryParse(
                        HttpContext.Current.Session["PageCount"].ToString(),
                        out toReturn);
                }

                return toReturn;
            }

            set
            {
                HttpContext.Current.Session["PageCount"] = value;
            }
        }

        private DataTable GrigliaResult
        {
            get
            {
                return (DataTable)HttpContext.Current.Session["GrigliaResult"];

            }
            set
            {
                HttpContext.Current.Session["GrigliaResult"] = value;
            }
        }

        private bool AllowConservazione
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["AllowConservazione"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["AllowConservazione"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["AllowConservazione"] = value;
            }
        }

        private bool AllowADL
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["AllowADL"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["AllowADL"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["AllowADL"] = value;
            }
        }

        private bool AllowADLRole
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["AllowADLRole"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["AllowADLRole"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["AllowADLRole"] = value;
            }
        }

        private bool EnableViewInfoProcessesStarted
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["EnableViewInfoProcessesStarted"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["EnableViewInfoProcessesStarted"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["EnableViewInfoProcessesStarted"] = value;
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

        /// <summary>
        /// Posizione celle per ordinamento
        /// </summary>
        public Dictionary<string, int> CellPosition
        {
            get
            {
                return HttpContext.Current.Session["cellPosition"] as Dictionary<string, int>;
            }
            set
            {
                HttpContext.Current.Session["cellPosition"] = value;
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

        private DocsPaWR.Templates Template
        {
            get
            {
                Templates result = null;
                if (HttpContext.Current.Session["template"] != null)
                {
                    result = HttpContext.Current.Session["template"] as Templates;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["template"] = value;
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

        private bool ActiveCodeDescriptionAdminSender
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["ActiveCodeDescriptionAdminSender"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["ActiveCodeDescriptionAdminSender"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ActiveCodeDescriptionAdminSender"] = value;
            }
        }

        private bool ShippingMethodRequired
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["ShippingMethodRequired"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["ShippingMethodRequired"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ShippingMethodRequired"] = value;
            }
        }

        private bool DisplayPecNotifications
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["DisplayPecNotifications"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["DisplayPecNotifications"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["DisplayPecNotifications"] = value;
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

        /// <summary>
        /// Abilita la ricerca degli allegati inserendo la descrizione del documento principale
        /// </summary>
        private bool EnabledSearchAttachByDescMainDoc
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["EnabledSearchAttachByDescMainDoc"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["EnabledSearchAttachByDescMainDoc"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["EnabledSearchAttachByDescMainDoc"] = value;
            }
        }

        private bool EnableTimestampDoc
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["EnableTimestampDoc"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["EnableTimestampDoc"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["EnableTimestampDoc"] = value;
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

        private bool IsAdl
        {
            get
            {
                return Request.QueryString["IsAdl"] != null ? true : false;
            }
        }

        private string SelectedRow
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["selectedRow"] != null)
                {
                    result = HttpContext.Current.Session["selectedRow"] as String;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["selectedRow"] = value;
            }
        }

        private string[] IdProfileList
        {
            get
            {
                string[] result = null;
                if (HttpContext.Current.Session["idProfileList"] != null)
                {
                    result = HttpContext.Current.Session["idProfileList"] as string[];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["idProfileList"] = value;
            }
        }

        private string[] CodeProfileList
        {
            get
            {
                string[] result = null;
                if (HttpContext.Current.Session["CodeProfileList"] != null)
                {
                    result = HttpContext.Current.Session["CodeProfileList"] as string[];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["CodeProfileList"] = value;
            }
        }

        protected Dictionary<String, String> ListCheck
        {
            get
            {
                Dictionary<String, String> result = null;
                if (HttpContext.Current.Session["listCheck"] != null)
                {
                    result = HttpContext.Current.Session["listCheck"] as Dictionary<String, String>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listCheck"] = value;
            }
        }

        protected Dictionary<String, FileToSign> ListToSign
        {
            get
            {
                Dictionary<String, FileToSign> result = null;
                if (HttpContext.Current.Session["listToSign"] != null)
                {
                    result = HttpContext.Current.Session["listToSign"] as Dictionary<String, FileToSign>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listToSign"] = value;
            }
        }

        protected bool CheckAll
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["checkAll"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["checkAll"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["checkAll"] = value;
            }
        }

        private bool SearchCorrespondentIntExtWithDisabled
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["searchCorrespondentIntExtWithDisabled"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["searchCorrespondentIntExtWithDisabled"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["searchCorrespondentIntExtWithDisabled"] = value;
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

        #endregion

        #region events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                schedaRicerca = (SearchManager)Session[SearchManager.SESSION_KEY];
                if (schedaRicerca == null)
                {
                    //Inizializzazione della scheda di ricerca per la gestione delle ricerche salvate
                    schedaRicerca = new SearchManager(KEY_SCHEDA_RICERCA, UserManager.GetUserInSession(), RoleManager.GetRoleInSession(), this);
                    Session[SearchManager.SESSION_KEY] = schedaRicerca;
                }
                schedaRicerca.Pagina = this;


                if (!UserManager.isRFEnabled())
                {
                    this.rl_visibilita.Items.Remove(this.rl_visibilita.Items.FindByValue("F"));
                }

                if (!this.IsPostBack)
                {
                    this.InitializePage();

                    if (this.IsAdl)
                    {
                        bool result = this.SearchDocumentFilters();
                        if (result)
                        {
                            this.SelectedRow = string.Empty;
                            this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                        }
                        this.UpnlNumerodocumenti.Update();
                        this.UpnlGrid.Update();
                    }

                    //Back
                    if (this.Request.QueryString["back"] != null && this.Request.QueryString["back"].Equals("1"))
                    {
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject obj = navigationList.Last();
                        if (!obj.CodePage.Equals(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED.ToString()) && !obj.CodePage.Equals(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED_ADL.ToString()))
                        {
                            obj = new Navigation.NavigationObject();
                            obj = navigationList.ElementAt(navigationList.Count - 2);
                        }
                        schedaRicerca.FiltriRicerca = obj.SearchFilters;
                        this.SearchFilters = obj.SearchFilters;
                        if (!string.IsNullOrEmpty(obj.NumPage))
                        {
                            this.SelectedPage = Int32.Parse(obj.NumPage);
                        }

                        this.BindFilterValues(schedaRicerca, this.ShowGridPersonalization);
                        DocumentManager.setFiltroRicFasc(this, schedaRicerca.FiltriRicerca);

                        if (!this.ShowGridPersonalization)
                        {
                            List<Field> visibleFields = GridManager.SelectedGrid.Fields.Where(x => x.Visible && x.GetType().Equals(typeof(Field))).ToList();
                            Field specialField = GridManager.SelectedGrid.Fields.Where(x => x.Visible && x.GetType().Equals(typeof(SpecialField)) && ((SpecialField)x).FieldType.Equals(SpecialFieldsEnum.Icons)).FirstOrDefault<Field>();

                            if (this.Template != null)
                            {
                                Session["templateRicerca"] = this.Template;
                            }
                        }

                        this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());

                        if (!string.IsNullOrEmpty(obj.OriginalObjectId))
                        {
                            string idProject = string.Empty;
                            foreach (GridViewRow grd in this.gridViewResult.Rows)
                            {
                                idProject = string.Empty;

                                if (GrigliaResult.Rows[grd.RowIndex]["IdProfile"] != null)
                                {
                                    idProject = GrigliaResult.Rows[grd.RowIndex]["IdProfile"].ToString();
                                }

                                if (idProject.Equals(obj.OriginalObjectId))
                                {
                                    this.gridViewResult.SelectRow(grd.RowIndex);
                                    this.SelectedRow = grd.RowIndex.ToString();
                                }
                            }
                        }

                        this.UpnlNumerodocumenti.Update();
                        this.UpnlGrid.Update();
                    }

                    this.txt_initNumProt_E.Focus();
                }

                else
                {
                    if (this.Result != null && this.Result.Length > 0)
                    {
                        // Visualizzazione dei risultati
                        this.SetCheckBox();

                        // Lista dei documenti risultato della ricerca
                        if (!this.SelectedPage.ToString().Equals(this.grid_pageindex.Value))
                        {
                            this.Result = null;
                            this.SelectedRow = string.Empty;
                            if (!string.IsNullOrEmpty(this.grid_pageindex.Value))
                            {
                                this.SelectedPage = int.Parse(this.grid_pageindex.Value);
                            }
                            this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                            this.BuildGridNavigator();
                            this.UpnlNumerodocumenti.Update();
                            this.UpnlGrid.Update();
                            this.upPnlGridIndexes.Update();

                            // riposiziono lo scroll del div in cima
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ScrollGridViewOnTop", "setFocusOnTop();", true);

                        }
                        else
                        {
                            this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
                        }
                    }
                    this.ReadRetValueFromPopup();
                }


                if (this.CustomDocuments)
                {
                    this.PnlTypeDocument.Controls.Clear();
                    if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
                    {
                        if (this.Template == null || !this.Template.SYSTEM_ID.ToString().Equals(this.DocumentDdlTypeDocument.SelectedValue))
                        {
                            this.Template = ProfilerDocManager.getTemplateById(this.DocumentDdlTypeDocument.SelectedItem.Value);
                        }
                        if (this.CustomDocuments)
                        {
                            this.PopulateProfiledDocument();
                        }
                    }
                }


                //rimuovo IsZoom alla chiusura della popup di zoom(da x o pulsante chiudi) quando richiamata da profilo, allegato, classifica
                if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_DOCUMENT_BUTTONS))
                {
                    if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_ZOOM)))
                    {
                        RemoveIsZoom();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('DocumentViewer','');", true);
                        return;
                    }
                }

                if (this.ShowGridPersonalization)
                {
                    this.EnableDisableSave();
                }

                this.RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DocumentImgSelectKeyword_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "SelectKeyword", "ajaxModalPopupSelectKeyword();", true);
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

                switch (addressBookCallFrom)
                {
                    case "CUSTOM":
                        if (atList != null && atList.Count > 0)
                        {
                            Corrispondente corr = null;
                            //Profiler document
                            UserControls.CorrespondentCustom userCorr = (UserControls.CorrespondentCustom)this.PnlTypeDocument.FindControl(this.IdCustomObjectCustomCorrespondent);

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
                            userCorr.TxtCodeCorrespondentCustom = corr.codiceRubrica;
                            userCorr.TxtDescriptionCorrespondentCustom = corr.descrizione;
                            userCorr.IdCorrespondentCustom = corr.systemId;
                            this.UpPnlTypeDocument.Update();
                        }
                        break;

                    case "T_S_R_S":
                        if (atList != null && atList.Count > 0)
                        {
                            NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                            Corrispondente tempCorrSingle;
                            if (!corrInSess.isRubricaComune)
                                tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                            else
                                tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                            this.txt_codMit_E.Text = tempCorrSingle.codiceRubrica;
                            this.txt_descrMit_E.Text = tempCorrSingle.descrizione;
                            this.IdRecipient.Value = tempCorrSingle.systemId;
                            this.UpProtocollo.Update();
                        }
                        break;

                    case "T_S_R_S_2":
                        if (atList != null && atList.Count > 0)
                        {
                            NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                            Corrispondente tempCorrSingle;
                            if (!corrInSess.isRubricaComune)
                                tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                            else
                                tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                            this.txt_codMittInter_C.Text = tempCorrSingle.codiceRubrica;
                            this.txt_descrMittInter_C.Text = tempCorrSingle.descrizione;
                            this.idMittItermedio.Value = tempCorrSingle.systemId;
                            this.UpMittInter.Update();
                        }
                        break;

                    case "T_S_R_S_3":
                        if (atList != null && atList.Count > 0)
                        {
                            NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                            Corrispondente tempCorrSingle;
                            if (!corrInSess.isRubricaComune)
                                tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                            else
                                tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                            this.txt_codUsrConsolidamento.Text = tempCorrSingle.codiceRubrica;
                            this.txt_descrUsrConsolidamento.Text = tempCorrSingle.descrizione;
                            this.idUsrConsolidamento.Value = tempCorrSingle.systemId;
                            this.UsrConsolidamentoTypeOfCorrespondent.Value = tempCorrSingle.tipoCorrispondente;
                            this.UpStatoConsolidamento.Update();
                        }
                        break;

                    case "F_X_X_S":
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
                            this.upPnlCreatore.Update();
                        }
                        break;
                    case "F_X_X_S_2":
                        if (atList != null && atList.Count > 0)
                        {
                            NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                            Corrispondente tempCorrSingle;
                            if (!corrInSess.isRubricaComune)
                                tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                            else
                                tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                            this.txtCodiceProprietario.Text = tempCorrSingle.codiceRubrica;
                            this.txtDescrizioneProprietario.Text = tempCorrSingle.descrizione;
                            this.idProprietario.Value = tempCorrSingle.systemId;
                            this.upPnlProprietario.Update();
                        }
                        break;
                    case "M_D_T_M":
                        if (atList != null && atList.Count > 0)
                        {
                            NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                            Corrispondente tempCorrSingle;
                            if (!corrInSess.isRubricaComune)
                                tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                            else
                                tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                            this.txtCodiceFirmatario.Text = tempCorrSingle.codiceRubrica;
                            this.txtDescrizioneFirmatario.Text = tempCorrSingle.descrizione;
                            this.idFirmatario.Value = tempCorrSingle.systemId;
                            this.UpPnlFirmatario.Update();
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

        protected void cancella_selezioneEsclusiva_Click(object sender, EventArgs e)
        {
            try
            {
                string idOggetto = (((CustomImageButton)sender).ID).Substring(1);
                ((RadioButtonList)this.PnlTypeDocument.FindControl(idOggetto)).SelectedIndex = -1;
                ((RadioButtonList)this.PnlTypeDocument.FindControl(idOggetto)).EnableViewState = true;
                for (int i = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
                {
                    if (((DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i]).SYSTEM_ID.ToString().Equals(idOggetto))
                    {
                        ((DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i]).VALORE_DATABASE = "";
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void txt_CodFascicolo_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                ProjectManager.removeFascicoloSelezionatoFascRapida(this);

                if (!string.IsNullOrEmpty(this.txt_CodFascicolo.Text))
                {
                    this.SearchProjectRegistro();
                }
                else
                {
                    this.txt_CodFascicolo.Text = string.Empty;
                    this.txt_DescFascicolo.Text = string.Empty;
                    this.IdProject.Value = string.Empty;
                    //Laura 25 Marzo
                    ProjectManager.setProjectInSessionForRicFasc(null);
                    ProjectManager.setProjectInSessionForRicFasc(String.Empty);
                }

                this.UpCodFasc.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtCodeObject_Click(object sender, EventArgs e)
        {
            try
            {
                List<DocsPaWR.Registro> registries = new List<Registro>();
                registries = UIManager.RegistryManager.GetListRegistriesAndRF(UIManager.RoleManager.GetRoleInSession().systemId, "1", string.Empty).ToList<DocsPaWR.Registro>();
                registries.Add(UIManager.RegistryManager.GetRegistryInSession());

                List<string> aL = new List<string>();
                if (registries != null)
                {
                    for (int i = 0; i < registries.Count; i++)
                    {
                        aL.Add(registries[i].systemId);
                    }
                }

                DocsPaWR.Oggetto[] listaObj = null;

                // E' inutile finire nel backend se la casella di testo è vuota (a parte il fatto che 
                // la funzione, in questo caso, restituisce tutto l'oggettario)
                if (!string.IsNullOrEmpty(this.TxtCodeObject.Text.Trim()))
                {
                    //In questo momento tralascio la descrizione oggetto che metto come stringa vuota
                    listaObj = DocumentManager.getListaOggettiByCod(aL.ToArray<string>(), string.Empty, this.TxtCodeObject.Text);
                }
                else
                {
                    listaObj = new DocsPaWR.Oggetto[] { 
                            new DocsPaWR.Oggetto()
                            {
                                descrizione = String.Empty,
                                codOggetto = String.Empty
                            }};
                }

                if (listaObj != null && listaObj.Length > 0)
                {
                    this.TxtObject.Text = listaObj[0].descrizione;
                    this.TxtCodeObject.Text = listaObj[0].codOggetto;
                }
                else
                {
                    this.TxtObject.Text = string.Empty;
                    this.TxtCodeObject.Text = string.Empty;
                }



                this.UpdPnlObject.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        private void rbl_Reg_C_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (this.rbl_Reg_C.SelectedItem.Value.Equals("R"))
                {
                    UserManager.removeListaIdRegistri(this);
                    //for (int h = 0; h < this.lb_reg_C.Items.Count; h++)
                    //    lb_reg_C.Items[h].Selected = false;
                    lb_reg_C.ClearSelection();
                }
                if (this.rbl_Reg_C.SelectedItem.Value.Equals("T"))
                {
                    UserManager.setListaIdRegistri(this, UserManager.getListaIdRegistriUtente(this));
                    for (int h = 0; h < this.lb_reg_C.Items.Count; h++)
                        lb_reg_C.Items[h].Selected = true;
                }
                if (this.rbl_Reg_C.SelectedItem.Value.Equals("M"))
                {
                    lb_reg_C.ClearSelection();
                    ArrayList idList = new ArrayList();
                    for (int h = 0; h < this.lb_reg_C.Items.Count; h++)
                    {
                        for (int i = 0; i < RoleManager.GetRoleInSession().registri.Length; i++)
                        {
                            if (RoleManager.GetRoleInSession().registri[i].codRegistro == lb_reg_C.Items[h].Text)
                            {
                                if (RoleManager.GetRoleInSession().registri[i] != null && !RoleManager.GetRoleInSession().registri[i].flag_pregresso)
                                {
                                    lb_reg_C.Items[h].Selected = true;
                                    idList.Add(lb_reg_C.Items[h].Value);
                                    break;
                                }
                            }
                        }

                    }

                    string[] id = new string[idList.Count];
                    for (int i = 0; i < idList.Count; i++)
                        id[i] = (string)idList[i];
                    UserManager.setListaIdRegistri(this, id);
                    //UserManager.setListaIdRegistri(this, UserManager.getListaIdRegistriUtente(this));
                }
                this.UpRegistro.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void lb_reg_C_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                this.rbl_Reg_C.ClearSelection();

                ArrayList idList = new ArrayList();
                for (int h = 0; h < this.lb_reg_C.Items.Count; h++)
                {
                    if (lb_reg_C.Items[h].Selected)
                        idList.Add(lb_reg_C.Items[h].Value);
                }
                string[] id = new string[idList.Count];
                for (int i = 0; i < idList.Count; i++)
                    id[i] = (string)idList[i];
                UserManager.setListaIdRegistri(this, id);
                this.UpRegistro.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DdlRapidSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.DdlRapidSearch.SelectedIndex == 0)
                {
                    this.SearchDocumentAdvancedEdit.Enabled = false;
                    this.SearchDocumentAdvancedRemove.Enabled = false;

                    if (GridManager.IsRoleEnabledToUseGrids())
                    {
                        GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);
                    }
                    return;
                }
                else
                {
                    this.SearchDocumentRemoveFilters_Click(null, null);
                }

                try
                {
                    string gridTempId = string.Empty;

                    schedaRicerca.Seleziona(Int32.Parse(this.DdlRapidSearch.SelectedValue), out gridTempId);

                    if (!string.IsNullOrEmpty(gridTempId) && GridManager.IsRoleEnabledToUseGrids())
                    {
                        schedaRicerca.gridId = gridTempId;
                        Grid tempGrid = GridManager.GetGridFromSearchId(schedaRicerca.gridId, GridTypeEnumeration.Document);
                        if (tempGrid != null)
                        {
                            GridManager.SelectedGrid = tempGrid;
                        }
                    }

                    try
                    {
                        if (this.DdlRapidSearch.SelectedIndex > 0)
                        {
                            Session.Add("itemUsedSearch", this.DdlRapidSearch.SelectedIndex.ToString());
                        }

                        this.BindFilterValues(schedaRicerca, true);
                        DocumentManager.setFiltroRicFasc(this, schedaRicerca.FiltriRicerca);

                        this.SearchDocumentAdvancedRemove.Enabled = true;
                        this.SearchDocumentAdvancedEdit.Enabled = true;
                        this.upPnlButtons.Update();

                        if (this.CustomDocuments)
                        {
                            this.PnlTypeDocument.Controls.Clear();
                            if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
                            {
                                if (this.Template == null || !this.Template.SYSTEM_ID.ToString().Equals(this.DocumentDdlTypeDocument.SelectedValue))
                                {
                                    this.Template = ProfilerDocManager.getTemplateById(this.DocumentDdlTypeDocument.SelectedItem.Value);
                                }
                                if (this.CustomDocuments)
                                {
                                    this.PopulateProfiledDocument();
                                    this.UpdatePanel2.Update();
                                    this.UpPnlTypeDocument.Update();
                                }
                            }
                        }

                        this.SearchDocumentAdvancedSearch_Click(null, null);

                    }
                    catch (Exception ex_)
                    {
                        string msg = utils.FormatJs(ex_.Message);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchProjectRemoveCriteria', 'error', '', '" + msg + "');", true);
                    }
                }
                catch (Exception ex)
                {
                    string msg = utils.FormatJs(ex.Message);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorCustom', 'error', '', '" + msg + "');", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void gridViewResult_ItemCreated(Object sender, GridViewRowEventArgs e)
        {
            try
            {
                //if (this.ShowGridPersonalization)
                //{
                //Posizione della freccetta nell'header
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    System.Web.UI.WebControls.Image arrow = new System.Web.UI.WebControls.Image();

                    arrow.BorderStyle = BorderStyle.None;

                    if (GridManager.SelectedGrid.OrderDirection == OrderDirectionEnum.Asc)
                    {
                        arrow.ImageUrl = "../Images/Icons/arrow_up.gif";
                    }
                    else
                    {
                        arrow.ImageUrl = "../Images/Icons/arrow_down.gif";
                    }

                    if (GridManager.SelectedGrid.FieldForOrder != null)
                    {
                        Field d = (Field)GridManager.SelectedGrid.Fields.Where(f => f.Visible && f.FieldId.Equals(GridManager.SelectedGrid.FieldForOrder.FieldId)).FirstOrDefault();
                        if (d != null)
                        {
                            try
                            {
                                int cell = this.CellPosition[d.FieldId];
                                e.Row.Cells[cell].Controls.Add(arrow);
                            }
                            catch
                            {
                                // il ruolo selezionato non ha tipologie
                            }
                        }
                    }
                }
                //}

                if (e.Row.RowType.Equals(DataControlRowType.DataRow) && this.GrigliaResult.Rows.Count > 0)
                {
                    string idProfile = this.GrigliaResult.Rows[e.Row.DataItemIndex]["idProfile"].ToString();
                    string codeProject = idProfile;
                    try { codeProject = this.Result.Where(y => y.SearchObjectID.Equals(idProfile)).FirstOrDefault().SearchObjectField.Where(x => x.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue; }
                    catch { }
                    if (string.IsNullOrEmpty(codeProject))
                        codeProject = idProfile;

                    CheckBox checkBox = e.Row.FindControl("checkDocumento") as CheckBox;
                    if (checkBox != null)
                    {
                        checkBox.CssClass = "pr" + idProfile;
                        checkBox.Attributes["onclick"] = "SetItemCheck(this, '" + idProfile + "_" + codeProject + "')";
                        if (this.ListCheck.ContainsKey(idProfile))
                            checkBox.Checked = true;
                        else
                            checkBox.Checked = false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchDocumentAdvancedSearch_Click(object o, EventArgs e)
        {
            // reset check list for massive ops
            this.ListCheck = new Dictionary<string, string>();
            this.SelectedPage = 1;
            this.HiddenItemsChecked.Value = string.Empty;
            this.HiddenItemsUnchecked.Value = string.Empty;
            this.HiddenItemsAll.Value = string.Empty;
            this.upPnlButtons.Update();

            try
            {
                bool result = this.SearchDocumentFilters();
                if (result)
                {
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    string searchIntervalYears = Utils.InitConfigurationKeys.GetValue("0", DBKeys.MAX_YEARS_SEARCHABLE.ToString());
                    bool maxYearsSearchable = (!this.IsAdl && string.IsNullOrEmpty(this.TxtYear.Text) && string.IsNullOrEmpty(this.txt_initIdDoc_C.Text)
                                                && string.IsNullOrEmpty(this.txt_initNumProt_E.Text) && string.IsNullOrEmpty(this.txt_initDataCreazione_E.Text)
                                                && string.IsNullOrEmpty(this.txt_initDataProt_E.Text) && string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue)
                                                && !string.IsNullOrEmpty(searchIntervalYears) && !searchIntervalYears.Equals("0"));
                    if (maxYearsSearchable && !this.PnlLblIntervalYears.Visible)
                    {
                        string date = (Convert.ToDateTime(DocumentManager.toDay()).AddYears(-Convert.ToInt32(searchIntervalYears))).ToShortDateString();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningSearchDocumentIntervalYears', 'warning', '', '" + date + "');} else {parent.ajaxDialogModal('WarningSearchDocumentIntervalYears', 'warning', '', '" + date + "');}", true);
                    }
                    this.CheckAll = false;
                }
                this.UpnlAzioniMassive.Update();
                this.UpnlNumerodocumenti.Update();
                this.UpnlGrid.Update();

                // riposiziono lo scroll del div in cima
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ScrollGridViewOnTop", "setFocusOnTop();", true);

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchDocumentAdvancedRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.DdlRapidSearch.SelectedIndex > 0)
                {
                    string id = this.DdlRapidSearch.SelectedValue;
                    DocsPaWR.SearchItem item = SearchManager.GetItemSearch(Int32.Parse(id));

                    DocsPaWR.Ruolo ruolo = null;
                    if (item.owner_idGruppo != 0)
                        ruolo = RoleManager.GetRoleInSession();

                    string msg = "Il criterio di ricerca con nome '" + this.DdlRapidSearch.SelectedItem.ToString() + "' verra' rimosso.<br />";
                    msg += (ruolo != null) ? "Attenzione! Il criterio di ricerca e' condiviso con il ruolo '" + ruolo.descrizione + "'.<br />" : "";
                    msg += "Confermi l'operazione?";
                    msg = utils.FormatJs(msg);

                    if (this.Session["itemUsedSearch"] != null)
                        Session.Remove("itemUsedSearch");

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('ErrorCustom', 'HiddenRemoveUsedSearch', '', '" + msg + "');", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchDocumentAdvancedEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.SearchDocumentFilters())
                {
                    // Impostazione del filtro utilizzato
                    schedaRicerca.FiltriRicerca = this.SearchFilters;
                    schedaRicerca.ProprietaNuovaRicerca = new SearchManager.NuovaRicerca();
                    if (this.DdlRapidSearch.SelectedIndex > 0)
                    {
                        string idRicercaSalvata = this.DdlRapidSearch.SelectedItem.Value.ToString();
                        Session["idRicercaSalvata"] = idRicercaSalvata;
                        Session["tipoRicercaSalvata"] = "D";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupModifySearch();", "ajaxModalPopupModifySearch();", true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchDocumentAdvancedSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.SearchDocumentFilters())
                {
                    // Impostazione del filtro utilizzato
                    schedaRicerca.FiltriRicerca = this.SearchFilters;
                    schedaRicerca.ProprietaNuovaRicerca = new SearchManager.NuovaRicerca();
                    Session["idRicercaSalvata"] = null;
                    Session["tipoRicercaSalvata"] = "D";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupSaveSearch();", "ajaxModalPopupSaveSearch();", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_idDocumento_C_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (this.ddl_idDocumento_C.SelectedIndex)
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
                    case 5: //Ieri
                        this.LtlADataCreazione.Visible = false;
                        this.txt_finedataCreazione_E.Visible = false;
                        this.txt_initDataCreazione_E.ReadOnly = true;
                        this.txt_initDataCreazione_E.Text = NttDataWA.Utils.dateformat.GetYesterday();
                        this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 6: //Ultimi 7 giorni
                        this.LtlADataCreazione.Visible = true;
                        this.txt_finedataCreazione_E.Visible = true;
                        this.txt_initDataCreazione_E.Text = NttDataWA.Utils.dateformat.GetLastSevenDay();
                        this.txt_finedataCreazione_E.Text = NttDataWA.Utils.dateformat.toDay();
                        this.txt_finedataCreazione_E.ReadOnly = true;
                        this.txt_initDataCreazione_E.ReadOnly = true;
                        this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataCreazione.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 7: //Ultimi 31 giorni
                        this.LtlADataCreazione.Visible = true;
                        this.txt_finedataCreazione_E.Visible = true;
                        this.txt_initDataCreazione_E.Text = NttDataWA.Utils.dateformat.GetLastThirtyOneDay();
                        this.txt_finedataCreazione_E.Text = NttDataWA.Utils.dateformat.toDay();
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

        protected void ddl_numProt_E_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Gabriele Melini 13-01-2015
                // INC000000519224
                // Conservo i valori contenuti nei campi data

                //this.txt_initNumProt_E.Text = string.Empty;
                //this.txt_initNumProt_E.Text = string.Empty;

                switch (this.ddl_numProt_E.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initNumProt_E.ReadOnly = false;
                        this.txt_fineNumProt_E.Visible = false;
                        this.LtlANumProto.Visible = false;
                        this.LtlDaNumProto.Visible = false;
                        this.txt_fineNumProt_E.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initNumProt_E.ReadOnly = false;
                        this.txt_fineNumProt_E.ReadOnly = false;
                        this.LtlANumProto.Visible = true;
                        this.LtlDaNumProto.Visible = true;
                        this.txt_fineNumProt_E.Visible = true;
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
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
                    case 5: //Ieri
                        this.LtlADataProto.Visible = false;
                        this.txt_fineDataProt_E.Visible = false;
                        this.txt_initDataProt_E.ReadOnly = true;
                        this.txt_initDataProt_E.Text = NttDataWA.Utils.dateformat.GetYesterday();
                        this.LtlDaDataProto.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDataProt_E.Text = string.Empty;
                        break;
                    case 6: //Ultimi 7 giorni
                        this.LtlADataProto.Visible = true;
                        this.txt_fineDataProt_E.Visible = true;
                        this.txt_initDataProt_E.Text = NttDataWA.Utils.dateformat.GetLastSevenDay();
                        this.txt_fineDataProt_E.Text = NttDataWA.Utils.dateformat.toDay();
                        this.txt_fineDataProt_E.ReadOnly = true;
                        this.txt_initDataProt_E.ReadOnly = true;
                        this.LtlDaDataProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 7: //Ultimi 31 iorni
                        this.LtlADataProto.Visible = true;
                        this.txt_fineDataProt_E.Visible = true;
                        this.txt_initDataProt_E.Text = NttDataWA.Utils.dateformat.GetLastThirtyOneDay();
                        this.txt_fineDataProt_E.Text = NttDataWA.Utils.dateformat.toDay();
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

        protected void ddl_dataScadenza_C_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Gabriele Melini 13-01-2015
                // INC000000519224
                // Conservo i valori contenuti nei campi data

                //this.txt_initDataScadenza_C.Text = string.Empty;
                //this.txt_fineDataScadenza_C.Text = string.Empty;

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dataScadenza_C.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initDataScadenza_C.ReadOnly = false;
                        this.txt_fineDataScadenza_C.Visible = false;
                        this.LtlADataScad.Visible = false;
                        this.LtlDaDataScad.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDataScadenza_C.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initDataScadenza_C.ReadOnly = false;
                        this.txt_fineDataScadenza_C.ReadOnly = false;
                        this.LtlADataScad.Visible = true;
                        this.LtlDaDataScad.Visible = true;
                        this.txt_fineDataScadenza_C.Visible = true;
                        this.LtlDaDataScad.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataScad.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADataScad.Visible = false;
                        this.txt_fineDataScadenza_C.Visible = false;
                        this.txt_initDataScadenza_C.ReadOnly = true;
                        this.txt_initDataScadenza_C.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDataScad.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDataScadenza_C.Text = string.Empty;
                        break;
                    case 3: //Settimana corrente
                        this.LtlADataScad.Visible = true;
                        this.txt_fineDataScadenza_C.Visible = true;
                        this.txt_initDataScadenza_C.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txt_fineDataScadenza_C.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txt_fineDataScadenza_C.ReadOnly = true;
                        this.txt_initDataScadenza_C.ReadOnly = true;
                        this.LtlDaDataScad.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataScad.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADataScad.Visible = true;
                        this.txt_fineDataScadenza_C.Visible = true;
                        this.txt_initDataScadenza_C.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txt_fineDataScadenza_C.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txt_fineDataScadenza_C.ReadOnly = true;
                        this.txt_initDataScadenza_C.ReadOnly = true;
                        this.LtlDaDataScad.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataScad.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
                this.UpDataScadProtMitt.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dataStampa_E_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Gabriele Melini 13-01-2015
                // INC000000519224
                // Conservo i valori contenuti nei campi data

                //this.txt_initDataStampa_E.Text = string.Empty;
                //this.txt_finedataStampa_E.Text = string.Empty;

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dataStampa_E.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initDataStampa_E.ReadOnly = false;
                        this.txt_finedataStampa_E.Visible = false;
                        this.LtlADataStampa.Visible = false;
                        this.LtlDaDataStampa.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_finedataStampa_E.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initDataStampa_E.ReadOnly = false;
                        this.txt_finedataStampa_E.ReadOnly = false;
                        this.LtlADataStampa.Visible = true;
                        this.LtlDaDataStampa.Visible = true;
                        this.txt_finedataStampa_E.Visible = true;
                        this.LtlDaDataStampa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataStampa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADataStampa.Visible = false;
                        this.txt_finedataStampa_E.Visible = false;
                        this.txt_initDataStampa_E.ReadOnly = true;
                        this.txt_initDataStampa_E.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDataStampa.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_finedataStampa_E.Text = string.Empty;
                        break;
                    case 3: //Settimana corrente
                        this.LtlADataStampa.Visible = true;
                        this.txt_finedataStampa_E.Visible = true;
                        this.txt_initDataStampa_E.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txt_finedataStampa_E.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txt_finedataStampa_E.ReadOnly = true;
                        this.txt_initDataStampa_E.ReadOnly = true;
                        this.LtlDaDataStampa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataStampa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADataStampa.Visible = true;
                        this.txt_finedataStampa_E.Visible = true;
                        this.txt_initDataStampa_E.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txt_finedataStampa_E.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txt_finedataStampa_E.ReadOnly = true;
                        this.txt_initDataStampa_E.ReadOnly = true;
                        this.LtlDaDataStampa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataStampa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
                this.UpDataStampa.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dataProtMitt_C_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    // Gabriele Melini 13-01-2015
                    // INC000000519224
                    // Conservo i valori contenuti nei campi data

                    //this.txt_initDataProtMitt_C.Text = string.Empty;
                    //this.txt_fineDataProtMitt_C.Text = string.Empty;
                }

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dataProtMitt_C.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initDataProtMitt_C.ReadOnly = false;
                        this.txt_fineDataProtMitt_C.Visible = false;
                        this.LtlADataProtMitt.Visible = false;
                        this.LtlDaDataProtMitt.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDataProtMitt_C.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initDataProtMitt_C.ReadOnly = false;
                        this.txt_fineDataProtMitt_C.ReadOnly = false;
                        this.LtlADataProtMitt.Visible = true;
                        this.LtlDaDataProtMitt.Visible = true;
                        this.txt_fineDataProtMitt_C.Visible = true;
                        this.LtlDaDataProtMitt.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataProtMitt.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADataProtMitt.Visible = false;
                        this.txt_fineDataProtMitt_C.Visible = false;
                        this.txt_initDataProtMitt_C.ReadOnly = true;
                        this.txt_initDataProtMitt_C.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDataProtMitt.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDataProtMitt_C.Text = string.Empty;
                        break;
                    case 3: //Settimana corrente
                        this.LtlADataProtMitt.Visible = true;
                        this.txt_fineDataProtMitt_C.Visible = true;
                        this.txt_initDataProtMitt_C.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txt_fineDataProtMitt_C.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txt_fineDataProtMitt_C.ReadOnly = true;
                        this.txt_initDataProtMitt_C.ReadOnly = true;
                        this.LtlDaDataProtMitt.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataProtMitt.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADataProtMitt.Visible = true;
                        this.txt_fineDataProtMitt_C.Visible = true;
                        this.txt_initDataProtMitt_C.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txt_fineDataProtMitt_C.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txt_fineDataProtMitt_C.ReadOnly = true;
                        this.txt_initDataProtMitt_C.ReadOnly = true;
                        this.LtlDaDataProtMitt.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataProtMitt.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
                this.UpDataProtMitt.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dataArrivo_C_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Gabriele Melini 13-01-2015
                // INC000000519224
                // Conservo i valori contenuti nei campi data

                //this.txt_initDataArrivo_C.Text = string.Empty;
                //this.txt_fineDataArrivo_C.Text = string.Empty;

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dataArrivo_C.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initDataArrivo_C.ReadOnly = false;
                        this.txt_fineDataArrivo_C.Visible = false;
                        this.LtlADataArrivo.Visible = false;
                        this.LtlDaDataArrivo.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDataArrivo_C.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initDataArrivo_C.ReadOnly = false;
                        this.txt_fineDataArrivo_C.ReadOnly = false;
                        this.LtlADataArrivo.Visible = true;
                        this.LtlDaDataArrivo.Visible = true;
                        this.txt_fineDataArrivo_C.Visible = true;
                        this.LtlDaDataArrivo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataArrivo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADataArrivo.Visible = false;
                        this.txt_fineDataArrivo_C.Visible = false;
                        this.txt_initDataArrivo_C.ReadOnly = true;
                        this.txt_initDataArrivo_C.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDataArrivo.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDataArrivo_C.Text = string.Empty;
                        break;
                    case 3: //Settimana corrente
                        this.LtlADataArrivo.Visible = true;
                        this.txt_fineDataArrivo_C.Visible = true;
                        this.txt_initDataArrivo_C.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txt_fineDataArrivo_C.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txt_fineDataArrivo_C.ReadOnly = true;
                        this.txt_initDataArrivo_C.ReadOnly = true;
                        this.LtlDaDataArrivo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataArrivo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADataArrivo.Visible = true;
                        this.txt_fineDataArrivo_C.Visible = true;
                        this.txt_initDataArrivo_C.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txt_fineDataArrivo_C.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txt_fineDataArrivo_C.ReadOnly = true;
                        this.txt_initDataArrivo_C.ReadOnly = true;
                        this.LtlDaDataArrivo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataArrivo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
                this.UpDataArrivo.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dataProtoEme_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Gabriele Melini 13-01-2015
                // INC000000519224
                // Conservo i valori contenuti nei campi data

                //this.txt_dataProtoEmeInizio.Text = string.Empty;
                //this.txt_dataProtoEmeFine.Text = string.Empty;

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dataProtoEme.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_dataProtoEmeInizio.ReadOnly = false;
                        this.txt_dataProtoEmeFine.Visible = false;
                        this.LtlADataSegDiEmerg.Visible = false;
                        this.LtlDaDataSegDiEmerg.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_dataProtoEmeFine.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_dataProtoEmeInizio.ReadOnly = false;
                        this.txt_dataProtoEmeFine.ReadOnly = false;
                        this.LtlADataSegDiEmerg.Visible = true;
                        this.LtlDaDataSegDiEmerg.Visible = true;
                        this.txt_dataProtoEmeFine.Visible = true;
                        this.LtlDaDataSegDiEmerg.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataSegDiEmerg.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADataSegDiEmerg.Visible = false;
                        this.txt_dataProtoEmeFine.Visible = false;
                        this.txt_dataProtoEmeInizio.ReadOnly = true;
                        this.txt_dataProtoEmeInizio.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDataSegDiEmerg.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_dataProtoEmeFine.Text = string.Empty;
                        break;
                    case 3: //Settimana corrente
                        this.LtlADataSegDiEmerg.Visible = true;
                        this.txt_dataProtoEmeFine.Visible = true;
                        this.txt_dataProtoEmeInizio.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txt_dataProtoEmeFine.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txt_dataProtoEmeFine.ReadOnly = true;
                        this.txt_dataProtoEmeInizio.ReadOnly = true;
                        this.LtlDaDataSegDiEmerg.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataSegDiEmerg.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADataSegDiEmerg.Visible = true;
                        this.txt_dataProtoEmeFine.Visible = true;
                        this.txt_dataProtoEmeInizio.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txt_dataProtoEmeFine.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txt_dataProtoEmeFine.ReadOnly = true;
                        this.txt_dataProtoEmeInizio.ReadOnly = true;
                        this.LtlDaDataSegDiEmerg.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataSegDiEmerg.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
                this.UpDataSegnaEme.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DdlIntervalloDataRepertorio_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string idOggetto = (((DropDownList)sender).ID).Replace("DdlIntervalloDataRepertorio_", "");
                DropDownList dlIntervalloDataRepertorio = (DropDownList)sender;
                CustomTextArea dataDa = (CustomTextArea)this.PnlTypeDocument.FindControl("TxtDataRepertorioDa_" + idOggetto);
                Label lblDataDa = (Label)this.PnlTypeDocument.FindControl("LblDataRepertorioDa_" + idOggetto);
                CustomTextArea dataA = (CustomTextArea)this.PnlTypeDocument.FindControl("TxtDataRepertorioA_" + idOggetto);
                Label lblDataA = (Label)this.PnlTypeDocument.FindControl("LblDataRepertorioA_" + idOggetto);
                string language = UIManager.UserManager.GetUserLanguage();
                switch (dlIntervalloDataRepertorio.SelectedIndex)
                {
                    case 0: //Valore singolo
                        dataDa.ReadOnly = false;
                        dataA.Visible = false;
                        dataA.Text = string.Empty;
                        lblDataA.Visible = false;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        dataDa.ReadOnly = false;
                        dataA.ReadOnly = false;
                        lblDataA.Visible = true;
                        lblDataDa.Visible = true;
                        dataA.Visible = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        lblDataA.Visible = false;
                        dataA.Visible = false;
                        dataA.Text = string.Empty;
                        dataDa.ReadOnly = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.toDay();
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 3: //Settimana corrente
                        lblDataA.Visible = true;
                        dataA.Visible = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        dataA.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        dataA.ReadOnly = true;
                        dataDa.ReadOnly = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        lblDataA.Visible = true;
                        dataA.Visible = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        dataA.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        dataA.ReadOnly = true;
                        dataDa.ReadOnly = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 5: //Ieri
                        lblDataA.Visible = false;
                        dataA.Visible = false;
                        dataA.Text = string.Empty;
                        dataDa.ReadOnly = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.GetYesterday();
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 6: //Ultimi 7 giorni
                        lblDataA.Visible = true;
                        dataA.Visible = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.GetLastSevenDay();
                        dataA.Text = NttDataWA.Utils.dateformat.toDay();
                        dataA.ReadOnly = true;
                        dataDa.ReadOnly = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 7: //Ultimi 31 giorni
                        lblDataA.Visible = true;
                        dataA.Visible = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.GetLastThirtyOneDay();
                        dataA.Text = NttDataWA.Utils.dateformat.toDay();
                        dataA.ReadOnly = true;
                        dataDa.ReadOnly = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
                OggettoCustom oggetto = (from o in this.Template.ELENCO_OGGETTI where o.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") select o).FirstOrDefault();
                if (oggetto != null)
                    this.controllaCampi(oggetto, oggetto.SYSTEM_ID.ToString());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_data_ricevute_pec_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Gabriele Melini 13-01-2015
                // INC000000519224
                // Conservo i valori contenuti nei campi data

                //this.Cal_Da_pec.Text = string.Empty;
                //this.Cal_A_pec.Text = string.Empty;

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_data_ricevute_pec.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.Cal_Da_pec.ReadOnly = false;
                        this.Cal_A_pec.Visible = false;
                        this.LtlADataRicevutaPEC.Visible = false;
                        this.LtlDaDataRicevutaPEC.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.Cal_A_pec.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.Cal_Da_pec.ReadOnly = false;
                        this.Cal_A_pec.ReadOnly = false;
                        this.LtlADataRicevutaPEC.Visible = true;
                        this.LtlDaDataRicevutaPEC.Visible = true;
                        this.Cal_A_pec.Visible = true;
                        this.LtlDaDataRicevutaPEC.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataRicevutaPEC.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADataRicevutaPEC.Visible = false;
                        this.Cal_A_pec.Visible = false;
                        this.Cal_Da_pec.ReadOnly = true;
                        this.Cal_Da_pec.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDataRicevutaPEC.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.Cal_A_pec.Text = string.Empty;
                        break;
                    case 3: //Settimana corrente
                        this.LtlADataRicevutaPEC.Visible = true;
                        this.Cal_A_pec.Visible = true;
                        this.Cal_Da_pec.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.Cal_A_pec.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.Cal_A_pec.ReadOnly = true;
                        this.Cal_Da_pec.ReadOnly = true;
                        this.LtlDaDataRicevutaPEC.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataRicevutaPEC.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADataRicevutaPEC.Visible = true;
                        this.Cal_A_pec.Visible = true;
                        this.Cal_Da_pec.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.Cal_A_pec.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.Cal_A_pec.ReadOnly = true;
                        this.Cal_Da_pec.ReadOnly = true;
                        this.LtlDaDataRicevutaPEC.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataRicevutaPEC.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
                this.UpDataRicevutePEC.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_data_ricevute_pitre_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Gabriele Melini 13-01-2015
                // INC000000519224
                // Conservo i valori contenuti nei campi data

                //this.Cal_Da_pitre.Text = string.Empty;
                //this.Cal_A_pitre.Text = string.Empty;

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_data_ricevute_pitre.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.Cal_Da_pitre.ReadOnly = false;
                        this.Cal_A_pitre.Visible = false;
                        this.LtlADataRicevuta.Visible = false;
                        this.LtlDaDataRicevuta.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.Cal_A_pitre.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.Cal_Da_pitre.ReadOnly = false;
                        this.Cal_A_pitre.ReadOnly = false;
                        this.LtlADataRicevuta.Visible = true;
                        this.LtlDaDataRicevuta.Visible = true;
                        this.Cal_A_pitre.Visible = true;
                        this.LtlDaDataRicevuta.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataRicevuta.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADataRicevuta.Visible = false;
                        this.Cal_A_pitre.Visible = false;
                        this.Cal_Da_pitre.ReadOnly = true;
                        this.Cal_Da_pitre.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDataRicevuta.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.Cal_A_pitre.Text = string.Empty;
                        break;
                    case 3: //Settimana corrente
                        this.LtlADataRicevuta.Visible = true;
                        this.Cal_A_pitre.Visible = true;
                        this.Cal_Da_pitre.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.Cal_A_pitre.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.Cal_A_pitre.ReadOnly = true;
                        this.Cal_Da_pitre.ReadOnly = true;
                        this.LtlDaDataRicevuta.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataRicevuta.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADataRicevuta.Visible = true;
                        this.Cal_A_pitre.Visible = true;
                        this.Cal_Da_pitre.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.Cal_A_pitre.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.Cal_A_pitre.ReadOnly = true;
                        this.Cal_Da_pitre.ReadOnly = true;
                        this.LtlDaDataRicevuta.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataRicevuta.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
                this.UpDataRicevute.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dataVers_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //this.txt_initDataVers.Text = string.Empty;
                //this.txt_fineDataVers.Text = string.Empty;

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_DataVers.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initDataVers.ReadOnly = false;
                        this.txt_fineDataVers.Visible = false;
                        this.LtlADataVers.Visible = false;
                        this.LtlDaDataVers.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDataVers.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initDataVers.ReadOnly = false;
                        this.txt_fineDataVers.ReadOnly = false;
                        this.LtlADataVers.Visible = true;
                        this.LtlDaDataVers.Visible = true;
                        this.txt_fineDataVers.Visible = true;
                        this.LtlDaDataVers.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataVers.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADataVers.Visible = false;
                        this.txt_fineDataVers.Visible = false;
                        this.txt_initDataVers.ReadOnly = true;
                        this.txt_initDataVers.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDataVers.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDataVers.Text = string.Empty;
                        break;
                    case 3: //Settimana corrente
                        this.LtlADataVers.Visible = true;
                        this.txt_fineDataVers.Visible = true;
                        this.txt_initDataVers.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txt_fineDataVers.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txt_initDataVers.ReadOnly = true;
                        this.txt_fineDataVers.ReadOnly = true;
                        this.LtlDaDataVers.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataVers.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADataVers.Visible = true;
                        this.txt_fineDataVers.Visible = true;
                        this.txt_initDataVers.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txt_fineDataVers.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txt_initDataVers.ReadOnly = true;
                        this.txt_fineDataVers.ReadOnly = true;
                        this.LtlDaDataVers.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataVers.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
                this.UpStatoConservazione.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_datePolicy_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_datePolicy.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initDatePolicy.ReadOnly = false;
                        this.txt_fineDatePolicy.Visible = false;
                        this.LtlADatePolicy.Visible = false;
                        this.LtlDaDatePolicy.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDatePolicy.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initDatePolicy.ReadOnly = false;
                        this.txt_fineDatePolicy.ReadOnly = false;
                        this.LtlADatePolicy.Visible = true;
                        this.LtlDaDatePolicy.Visible = true;
                        this.txt_fineDatePolicy.Visible = true;
                        this.LtlDaDatePolicy.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADatePolicy.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADatePolicy.Visible = false;
                        this.txt_fineDatePolicy.Visible = false;
                        this.txt_initDatePolicy.ReadOnly = true;
                        this.txt_initDatePolicy.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDatePolicy.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDatePolicy.Text = string.Empty;
                        break;
                    case 3: //Settimana corrente
                        this.LtlADatePolicy.Visible = true;
                        this.txt_fineDatePolicy.Visible = true;
                        this.txt_initDatePolicy.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txt_fineDatePolicy.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txt_initDatePolicy.ReadOnly = true;
                        this.txt_fineDatePolicy.ReadOnly = true;
                        this.LtlDaDatePolicy.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADatePolicy.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADatePolicy.Visible = true;
                        this.txt_fineDatePolicy.Visible = true;
                        this.txt_initDatePolicy.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txt_fineDatePolicy.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txt_initDatePolicy.ReadOnly = true;
                        this.txt_fineDatePolicy.ReadOnly = true;
                        this.LtlDaDatePolicy.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADatePolicy.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 5: //Ieri
                        this.LtlADatePolicy.Visible = false;
                        this.txt_fineDatePolicy.Visible = false;
                        this.txt_initDatePolicy.ReadOnly = true;
                        this.txt_initDatePolicy.Text = NttDataWA.Utils.dateformat.GetYesterday();
                        this.LtlDaDatePolicy.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                }
                this.UpStatoConservazione.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void cboDataConsolidamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.txtDataConsolidamento.Text = string.Empty;
                this.txtDataConsolidamentoFinale.Text = string.Empty;

                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.cboDataConsolidamento.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txtDataConsolidamento.ReadOnly = false;
                        this.txtDataConsolidamentoFinale.Visible = false;
                        this.LtlADataConsolidamento.Visible = false;
                        this.LtlDaDataConsolidamento.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txtDataConsolidamentoFinale.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txtDataConsolidamento.ReadOnly = false;
                        this.txtDataConsolidamentoFinale.ReadOnly = false;
                        this.LtlADataConsolidamento.Visible = true;
                        this.LtlDaDataConsolidamento.Visible = true;
                        this.txtDataConsolidamentoFinale.Visible = true;
                        this.LtlDaDataConsolidamento.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataConsolidamento.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADataConsolidamento.Visible = false;
                        this.txtDataConsolidamentoFinale.Visible = false;
                        this.txtDataConsolidamento.ReadOnly = true;
                        this.txtDataConsolidamento.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDataConsolidamento.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txtDataConsolidamentoFinale.Text = string.Empty;
                        break;
                    case 3: //Settimana corrente
                        this.LtlADataConsolidamento.Visible = true;
                        this.txtDataConsolidamentoFinale.Visible = true;
                        this.txtDataConsolidamento.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txtDataConsolidamentoFinale.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txtDataConsolidamentoFinale.ReadOnly = true;
                        this.txtDataConsolidamento.ReadOnly = true;
                        this.LtlDaDataConsolidamento.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataConsolidamento.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADataConsolidamento.Visible = true;
                        this.txtDataConsolidamentoFinale.Visible = true;
                        this.txtDataConsolidamento.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txtDataConsolidamentoFinale.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txtDataConsolidamentoFinale.ReadOnly = true;
                        this.txtDataConsolidamento.ReadOnly = true;
                        this.LtlDaDataConsolidamento.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataConsolidamento.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
                this.UpDataConsolidamento.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void chk_mitt_dest_storicizzati_Clik(object sender, EventArgs e)
        {
            string dataUser = RoleManager.GetRoleInSession().systemId;
            dataUser = dataUser + "-" + this.lb_reg_C.SelectedValue;
            string callType = string.Empty;

            if (this.chk_mitt_dest_storicizzati.Checked)
            {
                callType = "CALLTYPE_CORR_INT_EST_CON_DISABILITATI";
            }
            else
            {
                callType = "CALLTYPE_CORR_INT_EST";
            }
            this.RapidRecipient.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
            this.RapidMittInter.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
            this.RapidUsrConsolidamento.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;

            this.UpProtocollo.Update();
        }



        protected void DocumentImgRecipientAddressBookMittDest_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                //this.CallType = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
                if (this.chk_mitt_dest_storicizzati.Checked)
                {
                    this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                }
                else
                {
                    this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST;
                }
                //this.CallType = RubricaCallType.CALLTYPE_CORR_INT_EST;
                HttpContext.Current.Session["AddressBook.from"] = "T_S_R_S";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpProtocollo", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DocumentImgRecipientAddressBookMittInter_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                this.CallType = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
                HttpContext.Current.Session["AddressBook.from"] = "T_S_R_S_2";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpProtocollo", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void txt_codMit_E_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                CustomTextArea caller = sender as CustomTextArea;
                string codeAddressBook = this.txt_codMit_E.Text;

                if (!string.IsNullOrEmpty(codeAddressBook))
                {
                    this.SearchCorrespondent(codeAddressBook, caller.ID);
                }
                else
                {
                    this.txt_codMit_E.Text = string.Empty;
                    this.txt_descrMit_E.Text = string.Empty;
                    this.IdRecipient.Value = string.Empty;
                    this.UpProtocollo.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DataChanged(object sender, EventArgs e)
        {
            try
            {
                this.ddlNoteRF.Visible = false;

                //Se è presente il bottone di selezione esclusiva "RF" si verifica quanti sono gli
                //RF associati al ruolo dell'utente
                if (!string.IsNullOrEmpty(this.rl_visibilita.SelectedValue) && this.rl_visibilita.SelectedValue.Equals("F"))
                {
                    //DocsPaWR.Registro[] registriRf = UserManager.getListaRegistriWithRF(RoleManager.GetRoleInSession().systemId, "1", "");
                    DocsPaWR.Registro[] registriRf = RegistryManager.GetRFListInSession();
                    //Se un ruolo appartiene a più di un RF, allora selezionando dal menù il valore RF
                    //l'utente deve selezionare su quale degli RF creare la nota
                    if (registriRf != null && registriRf.Length > 0)
                    {
                        //Se l'inserimento della nota avviene durante la protocollazione 
                        //ed è impostato nella segnatura il codice del RF, la selezione del RF dal quale
                        //prendere il codice sarà mantenuta valida anche per l'eventuale inserimento delle note
                        //in questo caso non si deve presentare la popup di selezione del RF
                        if (this.ddlNoteRF != null)
                            this.LoadNoteRF(registriRf);

                    }
                }
                else
                {
                    this.ddlNoteRF.Items.Clear();
                    this.ddlNoteRF.Visible = false;
                }
                this.UpNote.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void LoadNoteRF(DocsPaWR.Registro[] listaRF)
        {

            this.ddlNoteRF.Items.Clear();
            if (listaRF != null && listaRF.Length > 0)
            {
                this.ddlNoteRF.Visible = true;

                if (listaRF.Length == 1)
                {
                    ListItem item = new ListItem();
                    item.Value = listaRF[0].systemId;
                    item.Text = listaRF[0].codRegistro;
                    this.ddlNoteRF.Items.Add(item);
                }
                else
                {
                    ListItem itemVuoto = new ListItem();
                    itemVuoto.Value = "";
                    itemVuoto.Text = Utils.Languages.GetLabelFromCode("DocumentNoteSelectAnRF", UIManager.UserManager.GetUserLanguage());
                    this.ddlNoteRF.Items.Add(itemVuoto);
                    foreach (DocsPaWR.Registro regis in listaRF)
                    {
                        ListItem item = new ListItem();
                        item.Value = regis.systemId;
                        item.Text = regis.codRegistro;
                        this.ddlNoteRF.Items.Add(item);
                    }
                }
            }
        }


        protected void rb_docSpediti_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.rb_docSpediti.SelectedItem.Value != "R")
                {
                    if (this.cbx_pec.Visible)
                        this.cbx_pec.Checked = true;
                    if (this.cbx_pitre.Visible)
                        this.cbx_pitre.Checked = true;
                }
                else
                {
                    if (this.cbx_pec.Visible)
                        this.cbx_pec.Checked = false;
                    if (this.cbx_pitre.Visible)
                        this.cbx_pitre.Checked = false;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void rb_docSpeditiEsito_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.rb_docSpeditiEsito.SelectedItem.Value != "R" && this.rb_docSpeditiEsito.SelectedItem.Value != "V")
                {
                    if (this.cbx_pec.Visible)
                    {
                        this.cbx_pec.Checked = false;
                        this.cbx_pec.Enabled = false;
                    }
                    if (this.cbx_pitre.Visible)
                    {
                        this.cbx_pitre.Checked = false;
                        this.cbx_pitre.Enabled = false;
                    }
                    if (this.rb_docSpediti.Visible)
                    {
                        this.rb_docSpediti.SelectedIndex = 5;
                        this.rb_docSpediti.Enabled = false;
                    }
                }
                else
                {
                    if (this.cbx_pec.Visible)
                    {
                        this.cbx_pec.Enabled = true;
                    }
                    if (this.cbx_pitre.Visible)
                    {
                        this.cbx_pitre.Enabled = true;
                    }
                    if (this.rb_docSpediti.Visible)
                    {
                        this.rb_docSpediti.Enabled = true;
                    }
                }
                this.UpDocSpediti.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void rbl_timestamp_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (this.rbl_timestamp.SelectedValue == "0")
                {
                    this.ddl_timestamp.Visible = true;
                }
                else
                {
                    this.ddl_timestamp.Visible = false;
                    this.ddl_timestamp.SelectedIndex = -1;
                    this.date_timestamp.Visible = false;
                    this.date_timestamp.Text = string.Empty;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_timestamp_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (this.ddl_timestamp.SelectedValue == "2")
                {
                    this.date_timestamp.Visible = true;
                }
                else
                {
                    this.date_timestamp.Visible = false;
                    this.date_timestamp.Text = string.Empty;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void lstFiltriConsolidamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.lstFiltriConsolidamento.SelectedValue == "0" && !this.lstFiltriConsolidamento.Items[1].Selected && !this.lstFiltriConsolidamento.Items[2].Selected)
                {
                    this.pnl_data_cons.Visible = false;
                    this.txtDataConsolidamento.Text = string.Empty;
                    this.txtDataConsolidamentoFinale.Text = string.Empty;
                    this.idUsrConsolidamento.Value = string.Empty;
                    this.UsrConsolidamentoTypeOfCorrespondent.Value = string.Empty;
                    this.txt_descrUsrConsolidamento.Text = string.Empty;
                    this.txt_codUsrConsolidamento.Text = string.Empty;
                    this.cboDataConsolidamento.SelectedIndex = 0;
                    this.txtDataConsolidamento.Enabled = true;
                    this.txtDataConsolidamentoFinale.Enabled = true;

                }
                else
                {
                    if (!this.lstFiltriConsolidamento.Items[0].Selected && !this.lstFiltriConsolidamento.Items[1].Selected && !this.lstFiltriConsolidamento.Items[2].Selected)
                    {
                        this.pnl_data_cons.Visible = false;
                        this.txtDataConsolidamento.Text = string.Empty;
                        this.txtDataConsolidamentoFinale.Text = string.Empty;
                        this.idUsrConsolidamento.Value = string.Empty;
                        this.UsrConsolidamentoTypeOfCorrespondent.Value = string.Empty;
                        this.txt_descrUsrConsolidamento.Text = string.Empty;
                        this.txt_codUsrConsolidamento.Text = string.Empty;
                        this.cboDataConsolidamento.SelectedIndex = 0;
                        this.txtDataConsolidamento.Enabled = true;
                        this.txtDataConsolidamentoFinale.Enabled = true;

                    }

                    else
                    {
                        this.pnl_data_cons.Visible = true;
                    }

                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ImgCreatoreAddressBook_Click(object sender, EventArgs e)
        {
            try
            {
                this.CallType = RubricaCallType.CALLTYPE_OWNER_AUTHOR;
                HttpContext.Current.Session["AddressBook.from"] = "F_X_X_S";
                HttpContext.Current.Session["AddressBook.EnableOnly"] = this.rblOwnerType.SelectedValue;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AddressBook", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ImgProprietarioAddressBook_Click(object sender, EventArgs e)
        {
            try
            {
                this.CallType = RubricaCallType.CALLTYPE_OWNER_AUTHOR;
                HttpContext.Current.Session["AddressBook.from"] = "F_X_X_S_2";
                HttpContext.Current.Session["AddressBook.EnableOnly"] = this.rblProprietarioType.SelectedValue;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AddressBook", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ImgFirmatarioAddressBook_Click(object sender, EventArgs e)
        {
            try
            {
                this.CallType = RubricaCallType.CALLTYPE_OWNER_AUTHOR;
                HttpContext.Current.Session["AddressBook.from"] = "M_D_T_M";
                HttpContext.Current.Session["AddressBook.EnableOnly"] = this.rblFirmatarioType.SelectedValue;
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
                        case "txtCodiceCreatore":
                            this.txtCodiceCreatore.Text = string.Empty;
                            this.txtDescrizioneCreatore.Text = string.Empty;
                            this.idCreatore.Value = string.Empty;
                            this.upPnlCreatore.Update();
                            break;
                        case "txtCodiceProprietario":
                            this.txtCodiceProprietario.Text = string.Empty;
                            this.txtDescrizioneProprietario.Text = string.Empty;
                            this.idProprietario.Value = string.Empty;
                            this.upPnlProprietario.Update();
                            break;
                        case "txt_codMittInter_C":
                            this.txt_codMittInter_C.Text = string.Empty;
                            this.txt_descrMittInter_C.Text = string.Empty;
                            this.idMittItermedio.Value = string.Empty;
                            this.UpMittInter.Update();
                            break;
                        case "txt_codMit_E":
                            this.txt_codMit_E.Text = string.Empty;
                            this.txt_descrMit_E.Text = string.Empty;
                            this.IdRecipient.Value = string.Empty;
                            this.UpProtocollo.Update();
                            break;
                        case "txt_codUsrConsolidamento":
                            this.txt_codUsrConsolidamento.Text = string.Empty;
                            this.txt_descrUsrConsolidamento.Text = string.Empty;
                            this.idUsrConsolidamento.Value = string.Empty;
                            this.UsrConsolidamentoTypeOfCorrespondent.Value = string.Empty;
                            this.UpStatoConsolidamento.Update();
                            break;
                        case "txtCodiceFirmatario":
                            this.txtCodiceFirmatario.Text = string.Empty;
                            this.txtDescrizioneFirmatario.Text = string.Empty;
                            this.idFirmatario.Value = string.Empty;
                            this.UpPnlFirmatario.Update();
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

        protected void DocumentImgAddressBookUsrConsolidamento_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                this.CallType = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
                HttpContext.Current.Session["AddressBook.from"] = "T_S_R_S_3";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpProtocollo", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchDocumentRemoveFilters_Click(object sender, EventArgs e)
        {
            try
            {
                this.opArr.Selected = true;
                this.opPart.Selected = true;
                this.opInt.Selected = true;
                this.opGrigio.Selected = true;
                this.opPredisposed.Selected = false;
                this.opAll.Selected = false;
                this.opPrints.Selected = false;
                this.opRepertorio.Selected = false;
                this.TxtCodeObject.Text = string.Empty;
                this.TxtObject.Text = string.Empty;
                this.TxtCodeObjectAttach.Text = string.Empty;
                this.TxtObjectAttach.Text = string.Empty;
                if (!this.IsAdl)
                {
                    this.TxtYear.Text = DateTime.Now.Year.ToString();
                }
                else
                {
                    this.TxtYear.Text = string.Empty;
                }
                this.DocumentDdlTypeDocument.SelectedIndex = 0;
                this.ddl_numRep.SelectedIndex = 0;
                this.txt_initNumRep.Text = string.Empty;
                this.txt_fineNumRep.Text = string.Empty;
                this.ddl_dataRepertorio.SelectedIndex = 0;
                this.txt_initDataRep.Text = string.Empty;
                this.txt_fineDataRep.Text = string.Empty;
                this.ddl_idDocumento_C.SelectedIndex = 0;
                this.txt_initIdDoc_C.Text = string.Empty;
                this.txt_fineIdDoc_C.Text = string.Empty;
                this.ddl_dataCreazione_E.SelectedIndex = 0;
                this.txt_initDataCreazione_E.Text = string.Empty;
                this.txt_finedataCreazione_E.Text = string.Empty;
                this.rblOwnerType.Items.FindByValue("R").Selected = true;
                this.idCreatore.Value = string.Empty;
                this.txtCodiceCreatore.Text = string.Empty;
                this.txtDescrizioneCreatore.Text = string.Empty;
                this.chkCreatoreExtendHistoricized.Checked = false;
                this.rblProprietarioType.Items.FindByValue("R").Selected = true;
                this.idProprietario.Value = string.Empty;
                this.txtCodiceProprietario.Text = string.Empty;
                this.txtDescrizioneProprietario.Text = string.Empty;
                this.ddl_numProt_E.SelectedIndex = 0;
                this.txt_initNumProt_E.Text = string.Empty;
                this.txt_fineNumProt_E.Text = string.Empty;
                this.ddl_dataProt_E.SelectedIndex = 0;
                this.txt_initDataProt_E.Text = string.Empty;
                this.txt_fineDataProt_E.Text = string.Empty;
                this.chk_mitt_dest_storicizzati.Checked = false;
                this.IdRecipient.Value = string.Empty;
                this.RecipientTypeOfCorrespondent.Value = string.Empty;
                this.txt_codMit_E.Text = string.Empty;
                this.txt_descrMit_E.Text = string.Empty;
                this.lb_reg_C.SelectedIndex = 0;
                this.rbl_Reg_C.SelectedIndex = 0;
                this.txt_numProtMitt_C.Text = string.Empty;
                this.ddl_dataScadenza_C.SelectedIndex = 0;
                this.txt_initDataScadenza_C.Text = string.Empty;
                this.txt_fineDataScadenza_C.Text = string.Empty;
                this.ddl_dataStampa_E.SelectedIndex = 0;
                this.txt_initDataStampa_E.Text = string.Empty;
                this.txt_finedataStampa_E.Text = string.Empty;
                this.ddl_spedizione.SelectedIndex = 0;
                this.cb_Conservato.Checked = false;
                this.cb_NonConservato.Checked = false;
                this.rb_annulla_C.Items.FindByValue("T").Selected = true;
                this.txt_segnatura.Text = string.Empty;
                this.idMittItermedio.Value = string.Empty;
                this.txt_codMittInter_C.Text = string.Empty;
                this.txt_descrMittInter_C.Text = string.Empty;
                this.IdProject.Value = string.Empty;
                this.txt_CodFascicolo.Text = string.Empty;
                this.txt_DescFascicolo.Text = string.Empty;
                this.cbxEstendiAFascicoli.Checked = false;
                this.ddl_dataProtMitt_C.SelectedIndex = 0;
                this.txt_initDataProtMitt_C.Text = string.Empty;
                this.txt_fineDataProtMitt_C.Text = string.Empty;
                this.ddl_dataArrivo_C.SelectedIndex = 0;
                this.txt_initDataArrivo_C.Text = string.Empty;
                this.txt_fineDataArrivo_C.Text = string.Empty;
                this.ListKeywords.Items.Clear();
                this.rl_visibilita.Items.FindByValue("Q").Selected = true;
                this.Txtnote.Text = string.Empty;
                this.cbl_docInCompl.SelectedIndex = -1;
                //this.rbl_timestamp.Items.FindByValue("2").Selected = true;
                //this.ddl_timestamp.Items.FindByValue("0").Selected = true;
                //this.date_timestamp.Text = string.Empty;
                this.rbl_timestamp.SelectedIndex = 2;
                this.ddl_timestamp.SelectedIndex = 0;
                this.date_timestamp.Text = string.Empty;
                this.cbx_Trasm.Checked = false;
                this.cbx_TrasmSenza.Checked = false;
                this.ddl_ragioneTrasm.SelectedIndex = -1;
                this.txt_protoEme.Text = string.Empty;
                this.ddl_dataProtoEme.SelectedIndex = -1;
                this.txt_dataProtoEmeInizio.Text = string.Empty;
                this.txt_dataProtoEmeFine.Text = string.Empty;
                this.rb_evidenza_C.Items.FindByValue("T").Selected = true;
                this.ddl_tipoFileAcquisiti.SelectedIndex = 0;
                this.chkFirmato.Checked = false;
                this.chkNonFirmato.Checked = false;
                this.chkFirmaElettronica.Checked = false;
                this.txtCodiceFirmatario.Text = string.Empty;
                this.txtDescrizioneFirmatario.Text = string.Empty;
                this.idFirmatario.Value = string.Empty;
                this.rblFirmatarioType.Items.FindByValue("R").Selected = true;
                this.ddl_op_versioni.SelectedIndex = 0;
                this.txt_versioni.Text = string.Empty;
                this.ddl_op_allegati.SelectedIndex = 0;
                this.txt_allegati.Text = string.Empty;
                //this.rblFiltriNumAllegati.Items.FindByValue("tutti").Selected = true;
                this.rblFiltriNumAllegati.SelectedIndex = 0;
                this.cbx_pec.Checked = false;
                this.cbx_pitre.Checked = false;
                this.rb_docSpediti.Items.FindByValue("R").Selected = true;
                this.rb_docSpeditiEsito.Items.FindByValue("R").Selected = true;
                this.rb_docSpediti.Enabled = true;
                this.cbx_pec.Enabled = true;
                this.cbx_pitre.Enabled = true;
                this.txt_dataSpedDa.Text = string.Empty;
                this.txt_dataSpedA.Text = string.Empty;
                this.ddl_ricevute_pitre.SelectedIndex = 0;
                this.ddl_data_ricevute_pitre.SelectedIndex = 0;
                this.Cal_Da_pitre.Text = string.Empty;
                this.Cal_A_pitre.Text = string.Empty;
                this.ddl_ricevute_pec.SelectedIndex = 0;
                this.ddl_data_ricevute_pec.SelectedIndex = 0;
                this.Cal_Da_pec.Text = string.Empty;
                this.Cal_A_pec.Text = string.Empty;
                this.lstFiltriConsolidamento.SelectedIndex = -1;
                this.cboDataConsolidamento.SelectedIndex = 0;
                this.txtDataConsolidamento.Text = string.Empty;
                this.txtDataConsolidamentoFinale.Text = string.Empty;
                this.idUsrConsolidamento.Value = string.Empty;
                this.UsrConsolidamentoTypeOfCorrespondent.Value = string.Empty;
                this.txt_codUsrConsolidamento.Text = string.Empty;
                this.txt_descrUsrConsolidamento.Text = string.Empty;
                this.txt_codDesc.Text = string.Empty;
                this.txt_numOggetto.Text = string.Empty;
                this.txt_commRef.Text = string.Empty;
                this.ddl_DataVers.SelectedIndex = 0;
                this.cbl_Conservazione.Items[0].Selected = true;
                this.cbl_Conservazione.Items[1].Selected = true;
                this.cbl_Conservazione.Items[2].Selected = true;
                this.cbl_Conservazione.Items[3].Selected = true;
                this.cbl_Conservazione.Items[4].Selected = true;
                this.cbl_Conservazione.Items[5].Selected = true;
                this.cbl_Conservazione.Items[6].Selected = true;
                this.cbl_Conservazione.Items[7].Selected = true;
                this.cbl_Conservazione.Items[8].Selected = true;
                this.cbl_Conservazione.Items[9].Selected = true;
                this.txt_initDataVers.Text = string.Empty;
                this.txt_fineDataVers.Text = string.Empty;
                this.TxtObjectAttach.Text = string.Empty;
                this.ddl_datePolicy.SelectedIndex = 0;
                this.txtCodPolicy.Text = string.Empty;
                this.txtCounterPolicy.Text = string.Empty;
                this.txt_initDatePolicy.Text = string.Empty;
                this.txt_fineDatePolicy.Text = string.Empty;
                this.pnlOjectAttach.Attributes.Add("style", "display:none");
                this.divDocumentLitObjectAttach.Attributes.Add("style", "display:none");
                this.divDocumentLitObject.Attributes.Add("style", "display:block");
                if (sender != null)
                {
                    this.DdlRapidSearch.SelectedIndex = -1;
                    this.DdlRapidSearch_SelectedIndexChanged(null, null);
                    this.SearchDocumentAdvancedEdit.Enabled = false;
                    this.SearchDocumentAdvancedRemove.Enabled = false;
                    this.upPnlButtons.Update();
                }

                this.DocumentDdlTypeDocument_OnSelectedIndexChanged(null, null);
                this.ddl_idDocumento_C_SelectedIndexChanged(null, null);
                this.ddl_dataCreazione_E_SelectedIndexChanged(null, null);
                this.ddl_numProt_E_SelectedIndexChanged(null, null);
                this.ddl_numRep_SelectedIndexChanged(null, null);
                this.ddl_dataRepertorio_SelectedIndexChanged(null, null);
                this.ddl_dataProt_E_SelectedIndexChanged(null, null);
                this.ddl_dataScadenza_C_SelectedIndexChanged(null, null);
                this.ddl_dataStampa_E_SelectedIndexChanged(null, null);
                this.ddl_dataProtMitt_C_SelectedIndexChanged(null, null);
                this.ddl_dataArrivo_C_SelectedIndexChanged(null, null);
                this.DataChanged(null, null);
                this.rbl_timestamp_SelectedIndexChanged(null, null);
                this.ddl_timestamp_SelectedIndexChanged(null, null);
                this.ddl_dataProtoEme_SelectedIndexChanged(null, null);
                this.ddl_data_ricevute_pitre_SelectedIndexChanged(null, null);
                this.ddl_data_ricevute_pec_SelectedIndexChanged(null, null);
                this.lstFiltriConsolidamento_SelectedIndexChanged(null, null);
                this.cboDataConsolidamento_SelectedIndexChanged(null, null);
                this.ddl_dataVers_SelectedIndexChanged(null, null);
                this.ddl_datePolicy_SelectedIndexChanged(null, null);

                this.PnlNeverTrasm.CssClass = "hidden";
                this.rbl_neverTrasm.SelectedIndex = 0;
                this.rb_allRoleTrasm.Selected = true;

                this.cb_neverSend.Checked = false;
                this.rbl_NeverSend.SelectedIndex = 0;
                this.PnlNeverSendFrom.CssClass = "hidden";
                
                this.UpPnlnNeverSend.Update();


                this.upPnlObject.Update();
                this.UpPnlRapidSearch.Update();
                this.UpPnlType.Update();
                this.UpdPnlObject.Update();
                this.UpPnlYear.Update();
                this.UpPnlTypeDocument.Update();
                this.UpDoc.Update();
                this.UpProtocollo.Update();
                this.UpRegistro.Update();
                this.UpPnlProtoMitt.Update();
                this.UpDataScadProtMitt.Update();
                this.UpDataStampa.Update();
                this.UpMezzoSped.Update();
                this.UpConservatoNon.Update();
                this.UpStatoDoc.Update();
                this.UpPnlSegnatura.Update();
                this.UpMittInter.Update();
                this.UpCodFasc.Update();
                this.UpDataProtMitt.Update();
                this.UpDataArrivo.Update();
                this.UpParolaChiave.Update();
                this.UpNote.Update();
                this.UpDocInColl.Update();
                this.UpPnlTimestamp.Update();
                this.UpTrasm.Update();
                this.UpSegnEmer.Update();
                this.UpEvidenza.Update();
                this.UpTipoFile.Update();
                this.UpVersioni.Update();
                this.UpAllegati.Update();
                this.UpDocSpeditiEsito.Update();
                this.UpDocSpediti.Update();
                this.UpRicPTRE.Update();
                this.UpRicPEC.Update();
                this.UpStatoConsolidamento.Update();
                this.UpPnlCodAmm.Update();
                this.UpPnlNumOggetto.Update();
                this.UpStatoConservazione.Update();
                this.UpPnlElectronicSignature.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void gridViewResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    string idProfile = GrigliaResult.Rows[e.Row.DataItemIndex]["IdProfile"].ToString();

                    string labelConservazione = "ProjectIconTemplateRemoveConservazione";
                    string labelAdl = "ProjectIconTemplateRemoveAdl";
                    string labelAdlRole = "ProjectIconTemplateRemoveAdlRole";
                    //imagini delle icone
                    if (bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["IsInStorageArea"].ToString()))
                    {
                        ((CustomImageButton)e.Row.FindControl("conservazione")).ImageUrl = "../Images/Icons/remove_preservation_grid.png";
                        ((CustomImageButton)e.Row.FindControl("conservazione")).OnMouseOutImage = "../Images/Icons/remove_preservation_grid.png";
                        ((CustomImageButton)e.Row.FindControl("conservazione")).OnMouseOverImage = "../Images/Icons/remove_preservation_grid_hover.png";
                        ((CustomImageButton)e.Row.FindControl("conservazione")).ImageUrlDisabled = "../Images/Icons/remove_preservation_grid_disabled.png";
                    }
                    else
                    {
                        labelConservazione = "ProjectIconTemplateConservazione";
                        ((CustomImageButton)e.Row.FindControl("conservazione")).ImageUrl = "../Images/Icons/add_preservation_grid.png";
                        ((CustomImageButton)e.Row.FindControl("conservazione")).OnMouseOutImage = "../Images/Icons/add_preservation_grid.png";
                        ((CustomImageButton)e.Row.FindControl("conservazione")).OnMouseOverImage = "../Images/Icons/add_preservation_grid_hover.png";
                        ((CustomImageButton)e.Row.FindControl("conservazione")).ImageUrlDisabled = "../Images/Icons/padd_preservation_grid_disabled.png";

                    }

                    labelAdlRole = "ProjectIconTemplateRemoveAdlRole";
                    if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
                    {
                        if (bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["IsInWorkingAreaRole"].ToString()))
                        {
                            ((CustomImageButton)e.Row.FindControl("adl")).Enabled = false;
                            ((CustomImageButton)e.Row.FindControl("adlrole")).ImageUrl = "../Images/Icons/adl1x.png";
                            ((CustomImageButton)e.Row.FindControl("adlrole")).OnMouseOutImage = "../Images/Icons/adl1x.png";
                            ((CustomImageButton)e.Row.FindControl("adlrole")).OnMouseOverImage = "../Images/Icons/adl1x_hover.png";
                            ((CustomImageButton)e.Row.FindControl("adlrole")).ImageUrlDisabled = "../Images/Icons/adl1x_disabled.png";
                        }
                        else
                        {
                            if (bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["IsInWorkingArea"].ToString()))
                                labelAdlRole = "ProjectIconTemplateAdlRole";
                            else
                                labelAdlRole = "ProjectIconTemplateAdlRoleInsert";
                            ((CustomImageButton)e.Row.FindControl("adl")).Enabled = true;
                            ((CustomImageButton)e.Row.FindControl("adlrole")).ImageUrl = "../Images/Icons/adl1.png";
                            ((CustomImageButton)e.Row.FindControl("adlrole")).OnMouseOutImage = "../Images/Icons/adl1.png";
                            ((CustomImageButton)e.Row.FindControl("adlrole")).OnMouseOverImage = "../Images/Icons/adl1_hover.png";
                            ((CustomImageButton)e.Row.FindControl("adlrole")).ImageUrlDisabled = "../Images/Icons/adl1_disabled.png";
                        }
                    }

                    if (bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["IsInWorkingArea"].ToString()))
                    {
                        ((CustomImageButton)e.Row.FindControl("adl")).ImageUrl = "../Images/Icons/adl2x.png";
                        ((CustomImageButton)e.Row.FindControl("adl")).OnMouseOutImage = "../Images/Icons/adl2x.png";
                        ((CustomImageButton)e.Row.FindControl("adl")).OnMouseOverImage = "../Images/Icons/adl2x_hover.png";
                        ((CustomImageButton)e.Row.FindControl("adl")).ImageUrlDisabled = "../Images/Icons/adl2x_disabled.png";
                    }
                    else
                    {
                        labelAdl = "ProjectIconTemplateAdl";
                        ((CustomImageButton)e.Row.FindControl("adl")).ImageUrl = "../Images/Icons/adl2.png";
                        ((CustomImageButton)e.Row.FindControl("adl")).OnMouseOutImage = "../Images/Icons/adl2.png";
                        ((CustomImageButton)e.Row.FindControl("adl")).OnMouseOverImage = "../Images/Icons/adl2_hover.png";
                        ((CustomImageButton)e.Row.FindControl("adl")).ImageUrlDisabled = "../Images/Icons/adl2_disabled.png";
                    }

                    string estensione = GrigliaResult.Rows[e.Row.RowIndex]["FileExtension"].ToString();
                    if (!string.IsNullOrEmpty(estensione))
                    {
                        string imgUrl = ResolveUrl(FileManager.getFileIcon(this, estensione));
                        ((CustomImageButton)e.Row.FindControl("estensionedoc")).Visible = true;
                        ((CustomImageButton)e.Row.FindControl("estensionedoc")).ImageUrl = imgUrl;
                        ((CustomImageButton)e.Row.FindControl("estensionedoc")).OnMouseOutImage = imgUrl;
                        ((CustomImageButton)e.Row.FindControl("estensionedoc")).OnMouseOverImage = imgUrl;
                    }
                    else
                        ((CustomImageButton)e.Row.FindControl("estensionedoc")).Visible = false;

                    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).Enabled = true;
                    //((CustomImageButton)e.Row.FindControl("conservazione")).Visible = this.AllowConservazione;
                    ((CustomImageButton)e.Row.FindControl("conservazione")).Visible = (this.AllowConservazione && !this.IsConservazioneSACER);
                    ((CustomImageButton)e.Row.FindControl("adl")).Visible = this.AllowADL;
                    ((CustomImageButton)e.Row.FindControl("adlrole")).Visible = this.AllowADLRole;
                    ((CustomImageButton)e.Row.FindControl("firmato")).Visible = bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["IsSigned"].ToString());
                    if (EnabledLibroFirma && EnableViewInfoProcessesStarted)
                    {
                        if (bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["InLibroFirma"].ToString()))
                            ((CustomImageButton)e.Row.FindControl("visualizzaProcessiFirmaAvviati")).Visible = true;

                        ((CustomImageButton)e.Row.FindControl("visualizzaStatoProcessiFirmaAvviati")).Visible = true;
                    }

                    //evento click
                    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).Click += new ImageClickEventHandler(ImageButton_Click);
                    ((CustomImageButton)e.Row.FindControl("visualizzaProcessiFirmaAvviati")).Click += new ImageClickEventHandler(ImageButton_Click);
                    ((CustomImageButton)e.Row.FindControl("visualizzaStatoProcessiFirmaAvviati")).Click += new ImageClickEventHandler(ImageButton_Click);
                    ((CustomImageButton)e.Row.FindControl("conservazione")).Click += new ImageClickEventHandler(ImageButton_Click);
                    ((CustomImageButton)e.Row.FindControl("adl")).Click += new ImageClickEventHandler(ImageButton_Click);
                    ((CustomImageButton)e.Row.FindControl("adlrole")).Click += new ImageClickEventHandler(ImageButton_Click);
                    ((CustomImageButton)e.Row.FindControl("firmato")).Click += new ImageClickEventHandler(ImageButton_Click);
                    ((CustomImageButton)e.Row.FindControl("estensionedoc")).Click += new ImageClickEventHandler(ImageButton_Click);
                    //tooltip
                    ((CustomImageButton)e.Row.FindControl("estensionedoc")).ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateEstensioneDoc", UIManager.UserManager.GetUserLanguage()) + " " + estensione;
                    ((CustomImageButton)e.Row.FindControl("conservazione")).ToolTip = Utils.Languages.GetLabelFromCode(labelConservazione, UIManager.UserManager.GetUserLanguage());
                    ((CustomImageButton)e.Row.FindControl("adl")).ToolTip = Utils.Languages.GetLabelFromCode(labelAdl, UIManager.UserManager.GetUserLanguage());
                    ((CustomImageButton)e.Row.FindControl("adlrole")).ToolTip = Utils.Languages.GetLabelFromCode(labelAdlRole, UIManager.UserManager.GetUserLanguage());
                    ((CustomImageButton)e.Row.FindControl("firmato")).ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateFirmato", UIManager.UserManager.GetUserLanguage());
                    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateVisualizzaDocumento", UIManager.UserManager.GetUserLanguage());
                    ((CustomImageButton)e.Row.FindControl("eliminadocumento")).Visible = false;
                    ((CustomImageButton)e.Row.FindControl("visualizzaProcessiFirmaAvviati")).ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgInfoProcessiAvviati", UIManager.UserManager.GetUserLanguage());
                    ((CustomImageButton)e.Row.FindControl("visualizzaStatoProcessiFirmaAvviati")).ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgProcessStateTooltip", UIManager.UserManager.GetUserLanguage());
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        //evento delle icone della griglia
        protected void ImageButton_Click(object sender, ImageClickEventArgs e)
        {
            CustomImageButton btnIm = (CustomImageButton)sender;
            GridViewRow row = (GridViewRow)btnIm.Parent.Parent;
            int rowIndex = row.RowIndex;
            string idProfile = GrigliaResult.Rows[rowIndex]["IdProfile"].ToString();
            SchedaDocumento schedaDocumento = UIManager.DocumentManager.getDocumentDetails(this, idProfile, idProfile);
            InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
            string language = UIManager.UserManager.GetUserLanguage();

            if (!string.IsNullOrEmpty(this.SelectedRow))
            {
                if (rowIndex != Int32.Parse(this.SelectedRow))
                {
                    this.SelectedRow = string.Empty;
                }
            }

            switch (btnIm.ID)
            {
                case "conservazione":
                    {
                        // Se il documento ha un file acquisito...
                        if (int.Parse(schedaDocumento.documenti[0].fileSize) > 0)
                        {
                            int resultIndex = Result.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.SearchObjectID.Equals(idProfile)).index;
                            int resultIndex2 = Result[resultIndex].SearchObjectField.Select((item2, j) => new { obj2 = item2, index2 = j }).First(item2 => item2.obj2.SearchObjectFieldID.Equals("IN_CONSERVAZIONE")).index2;

                            if (bool.Parse(this.GrigliaResult.Rows[rowIndex]["IsInStorageArea"].ToString()))
                            {
                                ProjectManager.RemoveDocumentFromStorageArea(schedaDocumento, infoUtente);
                                GrigliaResult.Rows[rowIndex]["IsInStorageArea"] = false;
                                Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "0";
                                btnIm.ImageUrl = "../Images/Icons/add_preservation_grid.png";
                                btnIm.OnMouseOutImage = "../Images/Icons/add_preservation_grid.png";
                                btnIm.OnMouseOverImage = "../Images/Icons/add_preservation_grid_hover.png";
                                btnIm.ImageUrlDisabled = "../Images/Icons/add_preservation_grid_disabled.png";
                                btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateConservazione", language);
                                btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateConservazione", language);
                            }
                            else
                            {
                                // MEV CS 1.5 - F03_01
                                #region OldCode
                                //ProjectManager.InsertDocumentInStorageArea(idProfile, schedaDocumento, infoUtente);
                                #endregion

                                #region NewCode
                                ProjectManager.InsertDocumentInStorageArea_WithConstraint(idProfile, schedaDocumento, infoUtente);
                                #endregion
                                // End MEV

                                GrigliaResult.Rows[rowIndex]["IsInStorageArea"] = true;
                                Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "1";
                                btnIm.ImageUrl = "../Images/Icons/remove_preservation_grid.png";
                                btnIm.OnMouseOutImage = "../Images/Icons/remove_preservation_grid.png";
                                btnIm.OnMouseOverImage = "../Images/Icons/remove_preservation_grid_hover.png";
                                btnIm.ImageUrlDisabled = "../Images/Icons/remove_preservation_grid_disabled.png";
                                btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveConservazione", language);
                                btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveConservazione", language);

                            }
                        }
                        else
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorProjectAddDocInConservazione', 'warning', '');} else {parent.ajaxDialogModal('ErrorProjectAddDocInConservazione', 'warning', '');}", true);
                        break;
                    }
                case "adl":
                    {
                        int resultIndex = Result.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.SearchObjectID.Equals(idProfile)).index;
                        int resultIndex2 = Result[resultIndex].SearchObjectField.Select((item2, j) => new { obj2 = item2, index2 = j }).First(item2 => item2.obj2.SearchObjectFieldID.Equals("IN_ADL")).index2;

                        if (bool.Parse(GrigliaResult.Rows[rowIndex]["IsInWorkingArea"].ToString()))
                        {
                            DocumentManager.eliminaDaAreaLavoro(this, idProfile, null);
                            GrigliaResult.Rows[rowIndex]["IsInWorkingArea"] = false;
                            Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "0";
                            btnIm.ImageUrl = "../Images/Icons/adl2.png";
                            btnIm.OnMouseOutImage = "../Images/Icons/adl2.png";
                            btnIm.OnMouseOverImage = "../Images/Icons/adl2_hover.png";
                            btnIm.ImageUrlDisabled = "../Images/Icons/adl2_disabled.png";
                            btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateAdl", language);
                            btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateAdl", language);

                            if (this.IsAdl && this.ListCheck.ContainsKey(idProfile))
                            {
                                this.ListCheck.Remove(idProfile);
                            }
                            //if (this.IsAdl)
                            //{
                            //    this.SelectedRow = string.Empty;
                            //    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                            //}
                        }
                        else
                        {
                            DocumentManager.addAreaLavoro(this, schedaDocumento);
                            GrigliaResult.Rows[rowIndex]["IsInWorkingArea"] = true;
                            Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "1";
                            btnIm.ImageUrl = "../Images/Icons/adl2x.png";
                            btnIm.OnMouseOutImage = "../Images/Icons/adl2x.png";
                            btnIm.OnMouseOverImage = "../Images/Icons/adl2x_hover.png";
                            btnIm.ImageUrlDisabled = "../Images/Icons/adl2x_disabled.png";
                            btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveAdl", language);
                            btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveAdl", language);
                        }
                        this.SelectedRow = string.Empty;
                        this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                        break;
                    }
                case "adlrole":
                    {
                        int resultIndex = Result.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.SearchObjectID.Equals(idProfile)).index;
                        int resultIndex2 = Result[resultIndex].SearchObjectField.Select((item2, j) => new { obj2 = item2, index2 = j }).First(item2 => item2.obj2.SearchObjectFieldID.Equals("IN_ADLROLE")).index2;

                        if (bool.Parse(GrigliaResult.Rows[rowIndex]["IsInWorkingAreaRole"].ToString()))
                        {
                            DocumentManager.eliminaDaAreaLavoroRole(this, idProfile, null);
                            GrigliaResult.Rows[rowIndex]["IsInWorkingAreaRole"] = false;
                            Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "0";
                            btnIm.ImageUrl = "../Images/Icons/adl1.png";
                            btnIm.OnMouseOutImage = "../Images/Icons/adl1.png";
                            btnIm.OnMouseOverImage = "../Images/Icons/adl1_hover.png";
                            btnIm.ImageUrlDisabled = "../Images/Icons/adl1_disabled.png";
                            btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateAdl", language);
                            btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateAdl", language);

                            if (this.IsAdl && this.ListCheck.ContainsKey(idProfile))
                            {
                                this.ListCheck.Remove(idProfile);
                            }
                            //if (this.IsAdl)
                            //{
                            //    this.SelectedRow = string.Empty;
                            //    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                            //}
                        }
                        else
                        {
                            DocumentManager.addAreaLavoroRole(this, schedaDocumento);
                            GrigliaResult.Rows[rowIndex]["IsInWorkingAreaRole"] = true;
                            Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "1";
                            btnIm.ImageUrl = "../Images/Icons/adl1x.png";
                            btnIm.OnMouseOutImage = "../Images/Icons/adl1x.png";
                            btnIm.OnMouseOverImage = "../Images/Icons/adl1x_hover.png";
                            btnIm.ImageUrlDisabled = "../Images/Icons/adl1x_disabled.png";
                            btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveAdl", language);
                            btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveAdl", language);
                        }
                        this.SelectedRow = string.Empty;
                        this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                        break;
                    }
                case "firmato":
                    {
                        UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                        FileManager.setSelectedFile(schedaDocumento.documenti[0]);
                        DocumentManager.removeSelectedNumberVersion();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDigitalSignDetails", "ajaxModalPopupDigitalSignDetails();", true);
                        break;
                    }
                case "visualizzaProcessiFirmaAvviati":
                    {
                        UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                        FileManager.setSelectedFile(schedaDocumento.documenti[0]);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupInfoSignatureProcessesStarted", "ajaxModalPopupInfoSignatureProcessesStarted();", true);
                        break;
                    }
                case "visualizzaStatoProcessiFirmaAvviati":
                    {
                        UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                        FileManager.setSelectedFile(schedaDocumento.documenti[0]);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDetailsLFAutomaticMode", "ajaxModalPopupDetailsLFAutomaticMode();", true);
                        break;
                    }
                case "visualizzadocumento":
                    {
                        HttpContext.Current.Session["isZoom"] = null;
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                        actualPage.IdObject = schedaDocumento.systemId;
                        actualPage.OriginalObjectId = schedaDocumento.systemId;
                        actualPage.NumPage = this.SelectedPage.ToString();
                        actualPage.SearchFilters = this.SearchFilters;
                        actualPage.PageSize = this.PageSize.ToString();
                        if (this.IsAdl)
                        {
                            actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED_ADL.ToString(), string.Empty);
                            actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED_ADL.ToString(), true, this.Page);
                            actualPage.CodePage = Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED_ADL.ToString();
                        }
                        else
                        {
                            actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED.ToString(), string.Empty);
                            actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED.ToString(), true, this.Page);
                            actualPage.CodePage = Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED.ToString();
                        }
                        actualPage.Page = "SEARCHDOCUMENTADVANCED.ASPX";
                        actualPage.DxTotalNumberElement = this.RecordCount.ToString();
                        actualPage.ViewResult = true;

                        if (this.PageCount == 0)
                        {
                            actualPage.DxTotalPageNumber = "1";
                        }
                        else
                        {
                            actualPage.DxTotalPageNumber = this.PageCount.ToString();
                        }

                        int indexElement = ((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1);
                        actualPage.DxPositionElement = indexElement.ToString();

                        navigationList.Add(actualPage);
                        Navigation.NavigationUtils.SetNavigationList(navigationList);

                        UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                        this.ListObjectNavigation = this.Result;
                        Response.Redirect("~/Document/Document.aspx");
                        break;
                    }
                case "estensionedoc":
                    {
                        this.IsZoom = true;
                        UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                        FileManager.setSelectedFile(schedaDocumento.documenti[0]);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDocumentViewer", "ajaxModalPopupDocumentViewer();", true);
                        NttDataWA.Popup.DocumentViewer.OpenDocumentViewer = true;
                        break;
                    }
            }

        }

        protected void DocumentDdlTypeDocument_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue) && this.DocumentDdlTypeDocument.SelectedValue != "0")
                {
                    if (this.CustomDocuments)
                    {
                        this.Template = ProfilerDocManager.getTemplateById(this.DocumentDdlTypeDocument.SelectedItem.Value);
                        if (this.Template != null)
                        {
                            if (!this.ShowGridPersonalization)
                            {
                                Session["templateRicerca"] = this.Template;
                            }

                            if (this.EnableStateDiagram)
                            {
                                this.DocumentDdlStateDiagram.ClearSelection();

                                //Verifico se esiste un diagramma di stato associato al tipo di documento
                                //Modifica per la visualizzazione solo degli stati per cui esistono documenti in essi
                                string idDiagramma = DiagrammiManager.getDiagrammaAssociato(this.DocumentDdlTypeDocument.SelectedValue).ToString();
                                if (!string.IsNullOrEmpty(idDiagramma) && !idDiagramma.Equals("0"))
                                {
                                    this.PnlStateDiagram.Visible = true;

                                    //Inizializzazione comboBox
                                    this.DocumentDdlStateDiagram.Items.Clear();
                                    ListItem itemEmpty = new ListItem();
                                    this.DocumentDdlStateDiagram.Items.Add(itemEmpty);

                                    DocsPaWR.Stato[] statiDg = DiagrammiManager.getStatiPerRicerca(idDiagramma, "D");
                                    foreach (Stato st in statiDg)
                                    {
                                        ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                                        this.DocumentDdlStateDiagram.Items.Add(item);
                                    }

                                    this.ddlStateCondition.Visible = true;
                                    this.PnlStateDiagram.Visible = true;
                                }
                                else
                                {
                                    this.ddlStateCondition.Visible = false;
                                    this.PnlStateDiagram.Visible = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    this.Template = null;
                    Session["templateRicerca"] = null;
                    this.PnlTypeDocument.Controls.Clear();
                    if (this.EnableStateDiagram)
                    {
                        this.DocumentDdlStateDiagram.ClearSelection();
                        this.PnlStateDiagram.Visible = false;
                        this.ddlStateCondition.Visible = false;
                    }
                }
                this.UpPnlTypeDocument.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void gridViewResult_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                Field d = new Field();
                string sortExpression = e.SortExpression.ToString();

                Templates templateTemp = Session["templateRicerca"] as Templates;

                OggettoCustom customObjectTemp = new OggettoCustom();

                if (templateTemp != null && !this.ShowGridPersonalization && sortExpression.Equals("CONTATORE"))
                {
                    customObjectTemp = templateTemp.ELENCO_OGGETTI.Where(
                         g => g.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && g.DA_VISUALIZZARE_RICERCA == "1").FirstOrDefault();

                    if (customObjectTemp != null)
                    {
                        d.AssociatedTemplateName = templateTemp.DESCRIZIONE;
                        d.CustomObjectId = customObjectTemp.SYSTEM_ID;
                        d.FieldId = customObjectTemp.SYSTEM_ID.ToString();
                        d.IsNumber = true;
                        d.Label = customObjectTemp.DESCRIZIONE;
                        d.OriginalLabel = customObjectTemp.DESCRIZIONE;
                        d.OracleDbColumnName = "to_number(getcontatoredocordinamento (a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "'))";
                        d.SqlServerDbColumnName = "@dbUser@.getContatoreDocOrdinamento(a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "')";
                    }
                }
                else
                {
                    d = (Field)GridManager.SelectedGrid.Fields.Where(f => f.Visible && f.FieldId.Equals(sortExpression)).FirstOrDefault();
                }

                if (d != null)
                {
                    if (GridManager.SelectedGrid.FieldForOrder != null && (GridManager.SelectedGrid.FieldForOrder.FieldId).Equals(d.FieldId))
                    {
                        if (GridManager.SelectedGrid.OrderDirection == OrderDirectionEnum.Asc)
                        {
                            GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Desc;
                        }
                        else
                        {
                            GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Asc;
                        }
                    }
                    else
                    {
                        if (GridManager.SelectedGrid.FieldForOrder == null && d.FieldId.Equals("D9"))
                        {
                            GridManager.SelectedGrid.FieldForOrder = d;
                            if (GridManager.SelectedGrid.OrderDirection == OrderDirectionEnum.Asc)
                            {
                                GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Desc;
                            }
                            else
                            {
                                GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Asc;
                            }
                        }
                        else
                        {
                            GridManager.SelectedGrid.FieldForOrder = d;
                            GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Asc;
                        }
                    }
                    GridManager.SelectedGrid.GridId = string.Empty;


                    this.SelectedPage = 1;

                    if (this.Result != null && this.Result.Length > 0)
                    {
                        this.SearchDocumentFilters();
                        this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    }
                    else
                    {
                        this.ShowGrid(GridManager.SelectedGrid, null, 0, 0, this.Labels.ToArray<EtichettaInfo>());
                    }

                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();

                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void gridViewResult_PreRender(object sender, EventArgs e)
        {
            try
            {
                CheckBox chkBxHeader = (CheckBox)this.gridViewResult.HeaderRow.FindControl("cb_selectall");
                if (chkBxHeader != null)
                {
                    chkBxHeader.Checked = this.CheckAll;
                }


                int cellsCount = 0;
                if (gridViewResult.Columns.Count > 0)
                    foreach (DataControlField td in gridViewResult.Columns)
                        if (td.Visible) cellsCount++;

                bool alternateRow = false;
                int indexCellIcons = -1;

                if (cellsCount > 0)
                {
                    for (int i = 1; i < gridViewResult.Rows.Count; i = i + 2)
                    {

                        gridViewResult.Rows[i].CssClass = "NormalRow";
                        if (alternateRow) gridViewResult.Rows[i].CssClass = "AltRow";
                        alternateRow = !alternateRow;

                        for (int j = 0; j < gridViewResult.Rows[i].Cells.Count; j++)
                        {
                            bool found = false;
                            foreach (Control c in gridViewResult.Rows[i].Cells[j].Controls)
                            {
                                if (c.ID == "visualizzadocumento")
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                                gridViewResult.Rows[i].Cells[j].Visible = false;
                            else
                            {
                                gridViewResult.Rows[i].Cells[j].ColumnSpan = cellsCount - 1;
                                gridViewResult.Rows[i].Cells[j].Attributes["style"] = "text-align: right;";
                                indexCellIcons = j;
                            }
                        }


                    }

                    alternateRow = false;
                    for (int i = 0; i < gridViewResult.Rows.Count; i = i + 2)
                    {
                        gridViewResult.Rows[i].CssClass = "NormalRow";
                        if (alternateRow) gridViewResult.Rows[i].CssClass = "AltRow";
                        alternateRow = !alternateRow;

                        for (int j = 0; j < gridViewResult.Rows[i].Cells.Count; j++)
                        {
                            bool found = false;
                            foreach (Control c in gridViewResult.Rows[i].Cells[j].Controls)
                            {
                                if (c.ID == "visualizzadocumento")
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (found)
                                gridViewResult.Rows[i].Cells[j].Visible = false;
                            else
                                gridViewResult.Rows[i].Cells[j].Attributes["style"] = "text-align: center;";
                        }


                    }
                    if (indexCellIcons > -1)
                        gridViewResult.HeaderRow.Cells[indexCellIcons].Visible = false;
                    for (int j = 0; j < gridViewResult.HeaderRow.Cells.Count; j++)
                        gridViewResult.HeaderRow.Cells[j].Attributes["style"] = "text-align: center;";

                    //MEV EMEA: per tipologie con multivalore con colore, vado a colorare la cella con il rispettivo colore
                    for (int i = 0; i < gridViewResult.Columns.Count; i++)
                    {
                        string textColumn = gridViewResult.Columns[i].HeaderText;
                        if (!string.IsNullOrEmpty(textColumn))
                        {
                            Field field = (from f in GridManager.SelectedGrid.Fields 
                                           where f.Label.Equals(textColumn) && !string.IsNullOrEmpty(f.AssociatedTemplateId) && f.Values != null && f.Values.Count() > 0
                                           select f).FirstOrDefault();
                            if (field != null)
                            {
                                string valueDoc = string.Empty;
                                for (int j = 0; j < gridViewResult.Rows.Count; j++)
                                {
                                    valueDoc = this.gridViewResult.Rows[j].Cells[i].Text;
                                    string htmlString = @valueDoc;
                                    htmlString = Regex.Replace(htmlString, @"<(.|\n)*?>", "");
                                    string color = (from value in field.Values
                                                    where htmlString.Equals(value.Value)
                                                    select value.ColorBG).FirstOrDefault();

                                    if (!string.IsNullOrEmpty(color))
                                    {
                                        string[] colorSplit = color.Split('^');
                                        this.gridViewResult.Rows[j].Cells[i].BackColor = System.Drawing.Color.FromArgb(Convert.ToInt16(colorSplit[0]), Convert.ToInt16(colorSplit[1]), Convert.ToInt16(colorSplit[2]));
                                    }
                                }
                            }
                        }
                    }
                    //FINE MEV EMEA
                }

                if (!string.IsNullOrEmpty(this.SelectedRow))
                {
                    for (int i = 0; i < gridViewResult.Rows.Count; i++)
                    {
                        if (this.gridViewResult.Rows[i].RowIndex == Int32.Parse(this.SelectedRow))
                        {
                            this.gridViewResult.Rows[i].Attributes.Remove("class");
                            this.gridViewResult.Rows[i].CssClass = "selectedrow";
                            this.gridViewResult.Rows[i - 1].Attributes.Remove("class");
                            this.gridViewResult.Rows[i - 1].CssClass = "selectedrow";
                        }
                    }

                }


                // grid width
                int fullWidth = 0;
                foreach (Field field in GridManager.SelectedGrid.Fields.Where(u => u.Visible).OrderBy(f => f.Position).ToList())
                    fullWidth += field.Width;
                this.gridViewResult.Attributes["style"] = "width: " + fullWidth + "px;";

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchDocumentDdlMassiveOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ListCheck != null && this.ListCheck.Count > 0)
            {
                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_ADL")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveAddAdlUser", "ajaxModalPopupMassiveAddAdlUser();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "REMOVE_MASSIVE_ADL")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveRemoveAdlUser", "ajaxModalPopupMassiveRemoveAdlUser();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_ADLR_DOC")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveAddAdlRole", "ajaxModalPopupMassiveAddAdlRole();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "REMOVE_MASSIVE_ADLR_DOC")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveRemoveAdlRole", "ajaxModalPopupMassiveRemoveAdlRole();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "DO_MASSIVE_CONS")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveConservation", "ajaxModalPopupMassiveConservation();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "DO_VERSAMENTO_PARER")
                {

                    // MEV Policy e responsabile conservazione
                    // 1) Se non è stato configurato un responsabile della conservazione si visualizza un messaggio bloccante
                    if (string.IsNullOrEmpty(UserManager.GetIdRuoloRespConservazione()))
                    {
                        string msg = "msgRespConsNotDefined";
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning');}", true);
                    }
                    else
                    {
                        // 2) Se è definita la chiave FE_MAX_DOC_VERSAMENTO devo controllare che il numero di documenti selezionati sia inferiore al limite
                        if (this.checkLimitDocVersamento())
                        {
                            string msg = "msgLimitVersReached";
                            string limit = Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_DOC_VERSAMENTO.ToString());
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '', '" + limit + "');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '', '" + limit + "');}", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveVersPARER", "ajaxModalPopupMassiveVersPARER();", true);
                        }
                    }
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVEXPORTDOC")
                {
                    DocumentManager.setFiltroRicFasc(this, this.SearchFilters);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ExportDati", "ajaxModalPopupExportDati();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_TRANSMISSION")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveTransmission", "ajaxModalPopupMassiveTransmission();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_CONVERSION")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveConversion", "ajaxModalPopupMassiveConversion();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_TIMESTAMP")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveTimestamp", "ajaxModalPopupMassiveTimestamp();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "DO_CONSOLIDAMENTO")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveConsolidation", "ajaxModalPopupMassiveConsolidation();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "DO_CONSOLIDAMENTO_METADATI")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveConsolidationMetadati", "ajaxModalPopupMassiveConsolidationMetadati();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_INOLTRA")
                {
                    DocumentManager.setSelectedRecord(null);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveForward", "ajaxModalPopupMassiveForward();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_CLASSIFICATION")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveCollate", "ajaxModalPopupMassiveCollate();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_REMOVE_VERSIONS")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveRemoveVersions", "ajaxModalPopupMassiveRemoveVersions();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_SIGN")
                {
                    componentType = UserManager.getComponentType(Request.UserAgent);
                    switch (componentType)
                    {
                        case (Constans.TYPE_ACTIVEX):
                        case (Constans.TYPE_SMARTCLIENT):
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveDigitalSignature", "ajaxModalPopupMassiveDigitalSignature();", true);
                            break;
                        case (Constans.TYPE_APPLET):
                            HttpContext.Current.Session["CommandType"] = null;
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveDigitalSignatureApplet", "ajaxModalPopupMassiveDigitalSignatureApplet();", true);
                            break;
                        default:
                            HttpContext.Current.Session["CommandType"] = null;
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveDigitalSignatureSockett", "ajaxModalPopupMassiveDigitalSignatureSocket();", true);
                            break;
                    }
                }
                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_SIGN_HSM")
                {
                    HttpContext.Current.Session["CommandType"] = null;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveSignatureHSM", "ajaxModalPopupMassiveSignatureHSM();", true);
                }
                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "DO_START_SIGNATURE_PROCESS")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveStartSignatureProcess", "ajaxModalPopupStartProcessSignature();", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorMassiveOperationNoItemSelected', 'warning', '');", true);
            }

            this.SearchDocumentDdlMassiveOperation.SelectedIndex = -1;
            this.UpnlAzioniMassive.Update();
        }

        protected void GridView_RowCommand(Object sender, GridViewCommandEventArgs e)
        {

            // If multiple ButtonField column fields are used, use the
            // CommandName property to determine which button was clicked.
            if (e.CommandName == "viewDetails")
            {


                int rowIndex = Convert.ToInt32(e.CommandArgument);
                string idProfile = GrigliaResult.Rows[rowIndex]["IdProfile"].ToString();
                SchedaDocumento schedaDocumento = UIManager.DocumentManager.getDocumentDetails(this, idProfile, idProfile);
                InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
                string language = UIManager.UserManager.GetUserLanguage();

                if (!string.IsNullOrEmpty(this.SelectedRow))
                {
                    if (rowIndex != Int32.Parse(this.SelectedRow))
                    {
                        this.SelectedRow = string.Empty;
                    }
                }

                HttpContext.Current.Session["isZoom"] = null;
                List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                actualPage.IdObject = schedaDocumento.systemId;
                actualPage.OriginalObjectId = schedaDocumento.systemId;
                actualPage.NumPage = this.SelectedPage.ToString();
                actualPage.SearchFilters = this.SearchFilters;
                actualPage.PageSize = this.PageSize.ToString();
                if (this.IsAdl)
                {
                    actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED_ADL.ToString(), string.Empty);
                    actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED_ADL.ToString(), true, this.Page);
                    actualPage.CodePage = Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED_ADL.ToString();
                }
                else
                {
                    actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED.ToString(), string.Empty);
                    actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED.ToString(), true, this.Page);
                    actualPage.CodePage = Navigation.NavigationUtils.NamePage.SEARCH_DOCUMENTS_ADVANCED.ToString();
                }
                actualPage.Page = "SEARCHDOCUMENTADVANCED.ASPX";
                actualPage.DxTotalNumberElement = this.RecordCount.ToString();
                actualPage.ViewResult = true;

                if (this.PageCount == 0)
                {
                    actualPage.DxTotalPageNumber = "1";
                }
                else
                {
                    actualPage.DxTotalPageNumber = this.PageCount.ToString();
                }

                int indexElement = ((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1);
                indexElement = indexElement + 1;
                actualPage.DxPositionElement = indexElement.ToString();

                navigationList.Add(actualPage);
                Navigation.NavigationUtils.SetNavigationList(navigationList);

                UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                this.ListObjectNavigation = this.Result;
                Response.Redirect("~/Document/Document.aspx");



            }

        }



        #endregion

        #region methods

        protected RubricaCallType GetCallType(string idControl)
        {
            RubricaCallType calltype = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
            if (idControl == "txt_codMit_E")
            {
                if (this.chk_mitt_dest_storicizzati.Checked)
                {
                    calltype = RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                }
                else
                {
                    calltype = RubricaCallType.CALLTYPE_CORR_INT_EST;
                }
            }

            if (idControl == "txtCodiceCreatore")
            {
                calltype = RubricaCallType.CALLTYPE_OWNER_AUTHOR;
            }

            if (idControl == "txtCodiceFirmatario")
            {
                calltype = RubricaCallType.CALLTYPE_OWNER_AUTHOR;
            }

            return calltype;
        }

        protected void PopulateProfiledDocument()
        {
            this.PnlTypeDocument.Controls.Clear();
            this.inserisciComponenti(false);
        }

        private void inserisciComponenti(bool readOnly)
        {
            List<AssDocFascRuoli> dirittiCampiRuolo = ProfilerDocManager.getDirittiCampiTipologiaDoc(RoleManager.GetRoleInSession().idGruppo, this.Template.SYSTEM_ID.ToString());

            for (int i = 0, index = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i];

                ProfilerDocManager.addNoRightsCustomObject(dirittiCampiRuolo, oggettoCustom);

                switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                {
                    case "CampoDiTesto":
                        this.inserisciCampoDiTesto(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "CasellaDiSelezione":
                        this.inserisciCasellaDiSelezione(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "MenuATendina":
                        this.inserisciMenuATendina(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "SelezioneEsclusiva":
                        this.inserisciSelezioneEsclusiva(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "Contatore":
                        this.inserisciContatore(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "Data":
                        this.inserisciData(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "Corrispondente":
                        SearchCorrespondentIntExtWithDisabled = true;
                        this.inserisciCorrispondente(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "Link":
                        //this.inserisciLink(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "ContatoreSottocontatore":
                        this.inserisciContatoreSottocontatore(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "Separatore":
                        this.inserisciCampoSeparatore(oggettoCustom);
                        break;
                    case "OggettoEsterno":
                        this.inserisciOggettoEsterno(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                }
            }
        }

        public void inserisciOggettoEsterno(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            Label etichetta = new Label();
            etichetta.EnableViewState = true;

            if ("SI".Equals(oggettoCustom.CAMPO_OBBLIGATORIO))
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE;
            }
            etichetta.CssClass = "weight";
            UserControls.IntegrationAdapter intAd = (UserControls.IntegrationAdapter)this.LoadControl("../UserControls/IntegrationAdapter.ascx");
            intAd.ID = oggettoCustom.SYSTEM_ID.ToString();
            intAd.View = UserControls.IntegrationAdapterView.INSERT_MODIFY;
            intAd.ManualInsertCssClass = "txt_textdata_counter_disabled_red";
            intAd.EnableViewState = true;
            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, intAd, oggettoCustom, this.Template, dirittiCampiRuolo);

            intAd.ConfigurationValue = oggettoCustom.CONFIG_OBJ_EST;
            IntegrationAdapterValue value = new IntegrationAdapterValue(oggettoCustom.CODICE_DB, oggettoCustom.VALORE_DATABASE, oggettoCustom.MANUAL_INSERT);
            intAd.Value = value;

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;

            if (etichetta.Visible)
            {
                HtmlGenericControl parDesc = new HtmlGenericControl("p");
                parDesc.Controls.Add(etichetta);
                parDesc.EnableViewState = true;
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (intAd.Visible)
            {
                divColValue.Controls.Add(intAd);
                divRowValue.Controls.Add(divColValue);
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }

        }

        private void inserisciCampoSeparatore(DocsPaWR.OggettoCustom oggettoCustom)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            Label etichettaCampoSeparatore = new Label();
            etichettaCampoSeparatore.CssClass = "weight";
            etichettaCampoSeparatore.EnableViewState = true;
            etichettaCampoSeparatore.Text = oggettoCustom.DESCRIZIONE.ToUpper();

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col_full_line";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaCampoSeparatore);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);

            if (etichettaCampoSeparatore.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }


        }

        private void inserisciContatoreSottocontatore(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            bool paneldll = false;

            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                return;

            Label etichettaContatoreSottocontatore = new Label();
            etichettaContatoreSottocontatore.EnableViewState = true;
            etichettaContatoreSottocontatore.Text = oggettoCustom.DESCRIZIONE;
            etichettaContatoreSottocontatore.CssClass = "weight";

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaContatoreSottocontatore);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);

            //Ricerca contatore
            TextBox contatoreDa = new TextBox();
            contatoreDa.ID = "da_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreDa.Text = oggettoCustom.VALORE_DATABASE;
            contatoreDa.Width = 40;
            contatoreDa.CssClass = "comp_profilazione_anteprima";

            TextBox contatoreA = new TextBox();
            contatoreA.ID = "a_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreA.Text = oggettoCustom.VALORE_DATABASE;
            contatoreA.Width = 40;
            contatoreA.CssClass = "comp_profilazione_anteprima";

            TextBox sottocontatoreDa = new TextBox();
            sottocontatoreDa.ID = "da_sottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            sottocontatoreDa.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
            sottocontatoreDa.Width = 40;
            sottocontatoreDa.CssClass = "comp_profilazione_anteprima";

            TextBox sottocontatoreA = new TextBox();
            sottocontatoreA.ID = "a_sottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            sottocontatoreA.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
            sottocontatoreA.Width = 40;
            sottocontatoreA.CssClass = "comp_profilazione_anteprima";

            //UserControls.Calendar dataSottocontatoreDa = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            //dataSottocontatoreDa.fromUrl = "../UserControls/DialogCalendar.aspx";
            //dataSottocontatoreDa.CSS = "testo_grigio";
            //dataSottocontatoreDa.ID = "da_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            //dataSottocontatoreDa.VisibleTimeMode = ProfilerDocManager.getVisibleTimeMode(oggettoCustom);

            //UserControls.Calendar dataSottocontatoreA = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            //dataSottocontatoreA.fromUrl = "../UserControls/DialogCalendar.aspx";
            //dataSottocontatoreA.CSS = "testo_grigio";
            //dataSottocontatoreA.ID = "a_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            //dataSottocontatoreA.VisibleTimeMode = ProfilazioneDocManager.getVisibleTimeMode(oggettoCustom);

            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }
            }

            if (!oggettoCustom.VALORE_SOTTOCONTATORE.Equals(""))
            {
                if (oggettoCustom.VALORE_SOTTOCONTATORE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_SOTTOCONTATORE.Split('@');
                    sottocontatoreDa.Text = contatori[0].ToString();
                    sottocontatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    sottocontatoreDa.Text = oggettoCustom.VALORE_SOTTOCONTATORE.ToString();
                    sottocontatoreA.Text = "";
                }
            }

            //if (!oggettoCustom.DATA_INSERIMENTO.Equals(""))
            //{
            //    if (oggettoCustom.DATA_INSERIMENTO.IndexOf("@") != -1)
            //    {
            //        string[] date = oggettoCustom.DATA_INSERIMENTO.Split('@');
            //        dataSottocontatoreDa.Text = date[0].ToString();
            //        dataSottocontatoreA.Text = date[1].ToString();
            //    }
            //    else
            //    {
            //        dataSottocontatoreDa.Text = oggettoCustom.DATA_INSERIMENTO.ToString();
            //        dataSottocontatoreA.Text = "";
            //    }
            //}

            Label etichettaContatoreDa = new Label();
            etichettaContatoreDa.Text = "<br/>&nbsp;&nbsp;da&nbsp;";
            etichettaContatoreDa.Font.Size = FontUnit.Point(8);
            etichettaContatoreDa.Font.Bold = true;
            etichettaContatoreDa.Font.Name = "Verdana";
            Label etichettaContatoreA = new Label();
            etichettaContatoreA.Text = "&nbsp;a&nbsp;";
            etichettaContatoreA.Font.Size = FontUnit.Point(8);
            etichettaContatoreA.Font.Bold = true;
            etichettaContatoreA.Font.Name = "Verdana";

            Label etichettaSottocontatoreDa = new Label();
            etichettaSottocontatoreDa.Text = "<br/>&nbsp;&nbsp;da&nbsp;";
            etichettaSottocontatoreDa.Font.Size = FontUnit.Point(8);
            etichettaSottocontatoreDa.Font.Bold = true;
            etichettaSottocontatoreDa.Font.Name = "Verdana";
            Label etichettaSottocontatoreA = new Label();
            etichettaSottocontatoreA.Text = "&nbsp;a&nbsp;";
            etichettaSottocontatoreA.Font.Size = FontUnit.Point(8);
            etichettaSottocontatoreA.Font.Bold = true;
            etichettaSottocontatoreA.Font.Name = "Verdana";

            Label etichettaDataSottocontatoreDa = new Label();
            etichettaDataSottocontatoreDa.Text = "<br/>&nbsp;&nbsp;da&nbsp;";
            etichettaDataSottocontatoreDa.Font.Size = FontUnit.Point(8);
            etichettaDataSottocontatoreDa.Font.Bold = true;
            etichettaDataSottocontatoreDa.Font.Name = "Verdana";
            Label etichettaDataSottocontatoreA = new Label();
            etichettaDataSottocontatoreA.Text = "&nbsp;a&nbsp;";
            etichettaDataSottocontatoreA.Font.Size = FontUnit.Point(8);
            etichettaDataSottocontatoreA.Font.Bold = true;
            etichettaDataSottocontatoreA.Font.Name = "Verdana";

            //TableRow row = new TableRow();
            //TableCell cell_1 = new TableCell();
            //cell_1.Controls.Add(etichettaContatoreSottocontatore);
            //row.Cells.Add(cell_1);

            //TableCell cell_2 = new TableCell();
            //


            //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
            //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
            //del contatore come da formato prescelto e in readOnly
            Label etichettaDDL = new Label();
            etichettaDDL.EnableViewState = true;
            DropDownList ddl = new DropDownList();
            ddl.EnableViewState = true;

            string language = UIManager.UserManager.GetUserLanguage();
            ddl.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));

            // aggiunto default vuoto
            ddl.Items.Add(new ListItem() { Text = "", Value = "" });

            Panel divRowDll = new Panel();
            divRowDll.CssClass = "row";
            divRowDll.EnableViewState = true;

            Panel divRowEtiDll = new Panel();
            divRowEtiDll.CssClass = "row";
            divRowEtiDll.EnableViewState = true;

            HtmlGenericControl parDll = new HtmlGenericControl("p");
            parDll.EnableViewState = true;

            if (string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }

                Ruolo ruoloUtente = RoleManager.GetRoleInSession();
                Registro[] registriRfVisibili = UIManager.RegistryManager.GetListRegistriesAndRF(ruoloUtente.systemId, string.Empty, string.Empty);

                Panel divColDllEti = new Panel();
                divColDllEti.CssClass = "col";
                divColDllEti.EnableViewState = true;

                Panel divColDll = new Panel();
                divColDll.CssClass = "col";
                divColDll.EnableViewState = true;

                switch (oggettoCustom.TIPO_CONTATORE)
                {
                    case "T":
                        break;
                    case "A":
                        paneldll = true;
                        etichettaDDL.Text = "&nbsp;AOO&nbsp;";
                        etichettaDDL.CssClass = "weight";
                        etichettaDDL.Width = 50;
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "chzn-select-deselect";
                        ddl.Width = 240;

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "0" && !((Registro)registriRfVisibili[i]).Sospeso)
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }
                    /**    if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Text = string.Empty;
                            it.Value = string.Empty;
                            ddl.Items.Add(it);
                            ddl.SelectedValue = string.Empty;
                        }
                        else
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF; */

                        parDll.Controls.Add(etichettaDDL);
                        divColDllEti.Controls.Add(parDll);
                        divRowEtiDll.Controls.Add(divColDllEti);

                        divColDll.Controls.Add(ddl);
                        divRowDll.Controls.Add(divColDll);
                        break;
                    case "R":
                        paneldll = true;
                        etichettaDDL.Text = "&nbsp;RF&nbsp;";
                        etichettaDDL.CssClass = "weight";
                        etichettaDDL.Width = 50;
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "chzn-select-deselect";
                        ddl.Width = 240;

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "1" && ((Registro)registriRfVisibili[i]).rfDisabled == "0")
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }

                    /**
                        if (ddl.Items.Count > 1)
                        {
                            ListItem emptyit = new ListItem();
                            emptyit.Value = string.Empty;
                            emptyit.Text = string.Empty;
                            ddl.Items.Add(emptyit);
                            ddl.SelectedValue = string.Empty;
                        }
                         else
                         {
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                         } */

                        parDll.Controls.Add(etichettaDDL);
                        divColDllEti.Controls.Add(parDll);
                        divRowEtiDll.Controls.Add(divColDllEti);

                        divColDll.Controls.Add(ddl);
                        divRowDll.Controls.Add(divColDll);
                        break;
                }
            }

            if (!oggettoCustom.VALORE_SOTTOCONTATORE.Equals(""))
            {
                if (oggettoCustom.VALORE_SOTTOCONTATORE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_SOTTOCONTATORE.Split('@');
                    sottocontatoreDa.Text = contatori[0].ToString();
                    sottocontatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    sottocontatoreDa.Text = oggettoCustom.VALORE_SOTTOCONTATORE.ToString();
                    sottocontatoreA.Text = "";
                }
            }

            //if (!oggettoCustom.DATA_INSERIMENTO.Equals(""))
            //{
            //    if (oggettoCustom.DATA_INSERIMENTO.IndexOf("@") != -1)
            //    {
            //        string[] date = oggettoCustom.DATA_INSERIMENTO.Split('@');
            //        dataSottocontatoreDa.Text = date[0].ToString();
            //        dataSottocontatoreA.Text = date[1].ToString();
            //    }
            //    else
            //    {
            //        dataSottocontatoreDa.Text = oggettoCustom.DATA_INSERIMENTO.ToString();
            //        dataSottocontatoreA.Text = "";
            //    }
            //}

            ////Imposto il contatore in funzione del formato
            //CustomTextArea contatore = new CustomTextArea();
            //CustomTextArea sottocontatore = new CustomTextArea();
            //CustomTextArea dataInserimentoSottocontatore = new CustomTextArea();
            //contatore.EnableViewState = true;
            //sottocontatore.EnableViewState = true;
            //dataInserimentoSottocontatore.EnableViewState = true;

            //contatore.ID = oggettoCustom.SYSTEM_ID.ToString();
            //sottocontatore.ID = oggettoCustom.SYSTEM_ID.ToString() + "_sottocontatore";
            //dataInserimentoSottocontatore.ID = oggettoCustom.SYSTEM_ID.ToString() + "_dataSottocontatore";
            //if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
            //{
            //    contatore.Text = oggettoCustom.FORMATO_CONTATORE;
            //    sottocontatore.Text = oggettoCustom.FORMATO_CONTATORE;

            //    if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE) && !string.IsNullOrEmpty(oggettoCustom.VALORE_SOTTOCONTATORE))
            //    {
            //        contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO);
            //        contatore.Text = contatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);

            //        sottocontatore.Text = sottocontatore.Text.Replace("ANNO", oggettoCustom.ANNO);
            //        sottocontatore.Text = sottocontatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_SOTTOCONTATORE);

            //        if (!string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) && oggettoCustom.ID_AOO_RF != "0")
            //        {
            //            Registro reg = UserManager.getRegistroBySistemId(this, oggettoCustom.ID_AOO_RF);
            //            if (reg != null)
            //            {
            //                contatore.Text = contatore.Text.Replace("RF", reg.codRegistro);
            //                contatore.Text = contatore.Text.Replace("AOO", reg.codRegistro);

            //                sottocontatore.Text = sottocontatore.Text.Replace("RF", reg.codRegistro);
            //                sottocontatore.Text = sottocontatore.Text.Replace("AOO", reg.codRegistro);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        contatore.Text = contatore.Text.Replace("ANNO", "");
            //        contatore.Text = contatore.Text.Replace("CONTATORE", "");
            //        contatore.Text = contatore.Text.Replace("RF", "");
            //        contatore.Text = contatore.Text.Replace("AOO", "");

            //        sottocontatore.Text = sottocontatore.Text.Replace("ANNO", "");
            //        sottocontatore.Text = sottocontatore.Text.Replace("CONTATORE", "");
            //        sottocontatore.Text = sottocontatore.Text.Replace("RF", "");
            //        sottocontatore.Text = sottocontatore.Text.Replace("AOO", "");
            //    }
            //    //}
            //}
            //else
            //{
            //    contatore.Text = oggettoCustom.VALORE_DATABASE;
            //    sottocontatore.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
            //}

            //Panel divRowCounter = new Panel();
            //divRowCounter.CssClass = "row";
            //divRowCounter.EnableViewState = true;

            //Panel divColCountCounter = new Panel();
            //divColCountCounter.CssClass = "col_full";
            //divColCountCounter.EnableViewState = true;
            //divColCountCounter.Controls.Add(contatore);
            //divColCountCounter.Controls.Add(sottocontatore);
            //divRowCounter.Controls.Add(divColCountCounter);

            //if (!string.IsNullOrEmpty(oggettoCustom.DATA_INSERIMENTO))
            //{
            //    dataInserimentoSottocontatore.Text = oggettoCustom.DATA_INSERIMENTO;

            //    Panel divColCountAbort = new Panel();
            //    divColCountAbort.CssClass = "col";
            //    divColCountAbort.EnableViewState = true;
            //    divColCountAbort.Controls.Add(dataInserimentoSottocontatore);
            //    divRowCounter.Controls.Add(divColCountAbort);
            //}

            //CheckBox cbContaDopo = new CheckBox();
            //cbContaDopo.EnableViewState = true;

            ////Verifico i diritti del ruolo sul campo
            //this.impostaDirittiRuoloContatoreSottocontatore(etichettaContatoreSottocontatore, contatore, sottocontatore, dataInserimentoSottocontatore, cbContaDopo, etichettaDDL, ddl, oggettoCustom, this.Template, dirittiCampiRuolo);

            //if (etichettaContatoreSottocontatore.Visible)
            //{
            //    this.PnlTypeDocument.Controls.Add(divRowDesc);
            //}

            //contatore.ReadOnly = true;
            //contatore.CssClass = "txt_input_half";
            //contatore.CssClassReadOnly = "txt_input_half_disabled";

            //sottocontatore.ReadOnly = true;
            //sottocontatore.CssClass = "txt_input_half";
            //sottocontatore.CssClassReadOnly = "txt_input_half_disabled";

            //dataInserimentoSottocontatore.ReadOnly = true;
            //dataInserimentoSottocontatore.CssClass = "txt_input_full";
            //dataInserimentoSottocontatore.CssClassReadOnly = "txt_input_full_disabled";
            //dataInserimentoSottocontatore.Visible = false;


            ////Inserisco il cb per il conta dopo
            //if (oggettoCustom.CONTA_DOPO == "1")
            //{
            //    cbContaDopo.ID = oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo";
            //    cbContaDopo.Checked = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
            //    cbContaDopo.ToolTip = "Attiva / Disattiva incremento del contatore al salvataggio dei dati.";

            //    Panel divColCountAfter = new Panel();
            //    divColCountAfter.CssClass = "col";
            //    divColCountAfter.EnableViewState = true;
            //    divColCountAfter.Controls.Add(cbContaDopo);
            //    divRowDll.Controls.Add(divColCountAfter);
            //}

            //if (paneldll)
            //{
            //    this.PnlTypeDocument.Controls.Add(divRowEtiDll);
            //    this.PnlTypeDocument.Controls.Add(divRowDll);
            //}

            //if (contatore.Visible || sottocontatore.Visible)
            //{
            //    this.PnlTypeDocument.Controls.Add(divRowCounter);
            //}
        }

        private void inserisciLink(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            UserControls.LinkDocFasc link = (UserControls.LinkDocFasc)this.LoadControl("../UserControls/LinkDocFasc.ascx");
            link.EnableViewState = true;
            link.ID = oggettoCustom.SYSTEM_ID.ToString();
            link.IsEsterno = (oggettoCustom.TIPO_LINK.Equals("ESTERNO"));
            link.IsFascicolo = ("FASCICOLO".Equals(oggettoCustom.TIPO_OBJ_LINK));

            link.TxtEtiLinkDocOrFasc = oggettoCustom.DESCRIZIONE;

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(link.TxtEtiLinkDocOrFasc, link, oggettoCustom, this.Template, dirittiCampiRuolo);

            link.Value = oggettoCustom.VALORE_DATABASE;

            if (link.Visible)
            {
                this.PnlTypeDocument.Controls.Add(link);
            }
        }

        private void HandleInternalDoc(string idDoc)
        {
            //InfoDocumento infoDoc = DocumentManager.GetInfoDocumento(idDoc, null, this);
            //if (infoDoc == null)
            //{
            //    Response.Write("<script language='javascript'>alert('Non si possiedono i diritti per la visualizzazione')</script>");
            //    return;
            //}
            //string errorMessage = "";
            //int result = DocumentManager.verificaACL("D", infoDoc.idProfile, UserManager.getInfoUtente(), out errorMessage);
            //if (result != 2)
            //{
            //    Response.Write("<script language='javascript'>alert('Non si possiedono i diritti per la visualizzazione')</script>");
            //}
            //else
            //{
            //    DocumentManager.setRisultatoRicerca(this, infoDoc);
            //    Response.Write("<script language='javascript'> top.principale.document.location = '../documento/gestionedoc.aspx?tab=protocollo&forceNewContext=true';</script>");
            //}
        }

        private void HandleInternalFasc(string idFasc)
        {
            //Fascicolo fasc = FascicoliManager.getFascicoloById(this, idFasc);
            //if (fasc == null)
            //{
            //    Response.Write("<script language='javascript'>alert('Non si possiedono i diritti per la visualizzazione')</script>");
            //    return;
            //}
            //string errorMessage = "";
            //int result = DocumentManager.verificaACL("F", fasc.systemID, UserManager.getInfoUtente(), out errorMessage);
            //if (result != 2)
            //{
            //    Response.Write("<script language='javascript'>alert('Non si possiedono i diritti per la visualizzazione')</script>");
            //}
            //else
            //{
            //    FascicoliManager.setFascicoloSelezionato(this, fasc);
            //    string newUrl = "../fascicolo/gestioneFasc.aspx?tab=documenti&forceNewContext=true";
            //    Response.Write("<script language='javascript'>top.principale.document.location='" + newUrl + "';</script>");
            //}
        }

        private void inserisciCorrispondente(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            DocsPaWR.StoricoProfilatiOldValue corrOldOb = new StoricoProfilatiOldValue();

            UserControls.CorrespondentCustom corrispondente = (UserControls.CorrespondentCustom)this.LoadControl("../UserControls/CorrespondentCustom.ascx");
            corrispondente.EnableViewState = true;

            corrispondente.TxtEtiCustomCorrespondent = oggettoCustom.DESCRIZIONE;

            corrispondente.TypeCorrespondentCustom = oggettoCustom.TIPO_RICERCA_CORR;
            corrispondente.ID = oggettoCustom.SYSTEM_ID.ToString();

            //Da amministrazione è stato impostato un ruolo di default per questo campo.
            if (!string.IsNullOrEmpty(oggettoCustom.ID_RUOLO_DEFAULT) && oggettoCustom.ID_RUOLO_DEFAULT != "0")
            {
                DocsPaWR.Ruolo ruolo = RoleManager.getRuoloById(oggettoCustom.ID_RUOLO_DEFAULT);
                if (ruolo != null)
                {
                    corrispondente.IdCorrespondentCustom = ruolo.systemId;
                    corrispondente.TxtCodeCorrespondentCustom = ruolo.codiceRubrica;
                    corrispondente.TxtDescriptionCorrespondentCustom = ruolo.descrizione;
                }
                oggettoCustom.ID_RUOLO_DEFAULT = "0";
            }

            //Il campo è valorizzato.
            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                DocsPaWR.Corrispondente corr_1 = AddressBookManager.getCorrispondenteBySystemIDDisabled(oggettoCustom.VALORE_DATABASE);
                if (corr_1 != null)
                {
                    corrispondente.IdCorrespondentCustom = corr_1.systemId;
                    corrispondente.TxtCodeCorrespondentCustom = corr_1.codiceRubrica.ToString();
                    corrispondente.TxtDescriptionCorrespondentCustom = corr_1.descrizione.ToString();
                    oggettoCustom.VALORE_DATABASE = corr_1.systemId;
                }
            }

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(corrispondente.TxtEtiCustomCorrespondent, corrispondente, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (corrispondente.Visible)
            {
                this.PnlTypeDocument.Controls.Add(corrispondente);
            }

        }

        private void inserisciData(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            //Per il momento questo tipo di campo è stato implementato con tre semplici textBox
            //Sarebbe opportuno creare un oggetto personalizzato, che espone le stesse funzionalità
            //della textBox, ma che mi permette di gestire la data con i tre campi separati.
            DocsPaWR.StoricoProfilatiOldValue dataOldOb = new StoricoProfilatiOldValue();
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            Label etichettaData = new Label();
            etichettaData.EnableViewState = true;


            etichettaData.Text = oggettoCustom.DESCRIZIONE;

            etichettaData.CssClass = "weight";

            UserControls.Calendar data = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            data.EnableViewState = true;
            data.ID = "da_" + oggettoCustom.SYSTEM_ID.ToString();
            data.VisibleTimeMode = ProfilerDocManager.getVisibleTimeMode(oggettoCustom);
            data.SetEnableTimeMode();

            UserControls.Calendar data2 = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            data2.EnableViewState = true;
            data2.ID = "a_" + oggettoCustom.SYSTEM_ID.ToString();
            data2.VisibleTimeMode = ProfilerDocManager.getVisibleTimeMode(oggettoCustom);
            data2.SetEnableTimeMode();

            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] date = oggettoCustom.VALORE_DATABASE.Split('@');
                    //dataDa.txt_Data.Text = date[0].ToString();
                    //dataA.txt_Data.Text = date[1].ToString();
                    data.Text = date[0].ToString();
                    data2.Text = date[1].ToString();
                }
                else
                {
                    //dataDa.txt_Data.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    //data.txt_Data.Text = "";
                    data.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    data2.Text = "";
                }
            }

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaData, data, oggettoCustom, this.Template, dirittiCampiRuolo);

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaData);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);

            if (etichettaData.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            Label etichettaDataFrom = new Label();
            etichettaDataFrom.EnableViewState = true;
            etichettaDataFrom.Text = "Da";

            HtmlGenericControl parDescFrom = new HtmlGenericControl("p");
            parDescFrom.Controls.Add(etichettaDataFrom);
            parDescFrom.EnableViewState = true;

            Panel divRowValueFrom = new Panel();
            divRowValueFrom.CssClass = "row";
            divRowValueFrom.EnableViewState = true;

            Panel divColValueFrom = new Panel();
            divColValueFrom.CssClass = "col";
            divColValueFrom.EnableViewState = true;

            divColValueFrom.Controls.Add(parDescFrom);
            divRowValueFrom.Controls.Add(divColValueFrom);

            if (data.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValueFrom);
            }

            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col";
            divColValue.EnableViewState = true;

            divColValue.Controls.Add(data);
            divRowValue.Controls.Add(divColValue);

            if (data.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }

            //////
            Label etichettaDataTo = new Label();
            etichettaDataTo.EnableViewState = true;
            etichettaDataTo.Text = "A";

            Panel divRowValueTo = new Panel();
            divRowValueTo.CssClass = "row";
            divRowValueTo.EnableViewState = true;

            Panel divColValueTo = new Panel();
            divColValueTo.CssClass = "col";
            divColValueTo.EnableViewState = true;

            HtmlGenericControl parDescTo = new HtmlGenericControl("p");
            parDescTo.Controls.Add(etichettaDataTo);
            parDescTo.EnableViewState = true;

            divColValueTo.Controls.Add(parDescTo);
            divRowValueTo.Controls.Add(divColValueTo);

            if (data.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValueTo);
            }

            Panel divRowValue2 = new Panel();
            divRowValue2.CssClass = "row";
            divRowValue2.EnableViewState = true;


            Panel divColValue2 = new Panel();
            divColValue2.CssClass = "col";
            divColValue2.EnableViewState = true;

            divColValue2.Controls.Add(data2);
            divRowValue2.Controls.Add(divColValue2);

            if (data.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValue2);
            }

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void inserisciContatore(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            bool paneldll = false;

            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            Label etichettaContatore = new Label();
            etichettaContatore.EnableViewState = true;


            etichettaContatore.Text = oggettoCustom.DESCRIZIONE;

            etichettaContatore.CssClass = "weight";

            CustomTextArea contatoreDa = new CustomTextArea();
            contatoreDa.EnableViewState = true;
            contatoreDa.ID = "da_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreDa.CssClass = "txt_textdata";

            CustomTextArea contatoreA = new CustomTextArea();
            contatoreA.EnableViewState = true;
            contatoreA.ID = "a_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreA.CssClass = "txt_textdata";

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaContatore);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);

            //Ruolo ruoloUtente = RoleManager.GetRoleInSession();
            //Registro[] registriRfVisibili = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, string.Empty, string.Empty);
            Registro[] registriRfVisibili = RegistryManager.GetRegAndRFListInSession();
            //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
            //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
            //del contatore come da formato prescelto e in readOnly
            Label etichettaDDL = new Label();
            etichettaDDL.EnableViewState = true;
            etichettaDDL.Width = 50;
            DropDownList ddl = new DropDownList();
            ddl.EnableViewState = true;

            string language = UIManager.UserManager.GetUserLanguage();
            ddl.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));

            //Emanuela 19-05-2014: aggiunto default vuoto
            ddl.Items.Add(new ListItem() { Text = "", Value = "" });

            Panel divRowDll = new Panel();
            divRowDll.CssClass = "row";
            divRowDll.EnableViewState = true;

            Panel divRowEtiDll = new Panel();
            divRowEtiDll.CssClass = "row";
            divRowEtiDll.EnableViewState = true;

            HtmlGenericControl parDll = new HtmlGenericControl("p");
            parDll.EnableViewState = true;

            Panel divColDllEti = new Panel();
            divColDllEti.CssClass = "col";
            divColDllEti.EnableViewState = true;

            Panel divColDll = new Panel();
            divColDll.CssClass = "col";
            divColDll.EnableViewState = true;


            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }
            }

            switch (oggettoCustom.TIPO_CONTATORE)
            {
                case "T":
                    break;
                case "A":
                    paneldll = true;
                    etichettaDDL.Text = "&nbsp;AOO&nbsp;";
                    etichettaDDL.CssClass = "weight";
                    ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                    ddl.CssClass = "chzn-select-deselect";
                    ddl.Width = 240;

                    //Distinguo se è un registro o un rf
                    for (int i = 0; i < registriRfVisibili.Length; i++)
                    {
                        ListItem item = new ListItem();
                        if (((Registro)registriRfVisibili[i]).chaRF == "0")
                        {
                            item.Value = ((Registro)registriRfVisibili[i]).systemId;
                            item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                            ddl.Items.Add(item);
                        }
                    }
                  //  ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                    parDll.Controls.Add(etichettaDDL);
                    divColDllEti.Controls.Add(parDll);
                    divRowEtiDll.Controls.Add(divColDllEti);

                    divColDll.Controls.Add(ddl);
                    divRowDll.Controls.Add(divColDll);
                    break;
                case "R":
                    paneldll = true;
                    etichettaDDL.Text = "&nbsp;RF&nbsp;";
                    etichettaDDL.CssClass = "weight";
                    ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                    ddl.CssClass = "chzn-select-deselect";
                    ddl.Width = 240;

                    //Distinguo se è un registro o un rf
                    for (int i = 0; i < registriRfVisibili.Length; i++)
                    {
                        ListItem item = new ListItem();
                        if (((Registro)registriRfVisibili[i]).chaRF == "1" && ((Registro)registriRfVisibili[i]).rfDisabled == "0")
                        {
                            item.Value = ((Registro)registriRfVisibili[i]).systemId;
                            item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                            ddl.Items.Add(item);
                        }
                    }
                    /*
                     * Emanuela 21-05-2014: commento per far si che come RF di default venga mostrato l'item vuoto
                    if (ddl.Items.Count == 1)
                        ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                    else
                        ddl.Items.Insert(0, new ListItem(""));
                    */

                    if (!string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) && !oggettoCustom.ID_AOO_RF.Equals("0"))
                        ddl.SelectedValue = oggettoCustom.ID_AOO_RF;

                    ddl.CssClass = "chzn-select-deselect";

                    parDll.Controls.Add(etichettaDDL);
                    divColDllEti.Controls.Add(parDll);
                    divRowEtiDll.Controls.Add(divColDllEti);

                    divColDll.Controls.Add(ddl);
                    divRowDll.Controls.Add(divColDll);

                    break;
            }

            if (etichettaContatore.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }
            }

            Label etichettaContatoreDa = new Label();
            etichettaContatoreDa.EnableViewState = true;
            etichettaContatoreDa.Text = "Da";

            //////
            Label etichettaContatoreA = new Label();
            etichettaContatoreA.EnableViewState = true;
            etichettaContatoreA.Text = "A";

            Panel divColValueTo = new Panel();
            divColValueTo.CssClass = "col";
            divColValueTo.EnableViewState = true;

            Panel divRowValueFrom = new Panel();
            divRowValueFrom.CssClass = "row";
            divRowValueFrom.EnableViewState = true;

            Panel divCol1 = new Panel();
            divCol1.CssClass = "col";
            divCol1.EnableViewState = true;

            Panel divCol2 = new Panel();
            divCol2.CssClass = "col";
            divCol2.EnableViewState = true;

            Panel divCol3 = new Panel();
            divCol3.CssClass = "col";
            divCol3.EnableViewState = true;

            Panel divCol4 = new Panel();
            divCol4.CssClass = "col";
            divCol4.EnableViewState = true;

            

            divCol1.Controls.Add(etichettaContatoreDa);
            divCol2.Controls.Add(contatoreDa);
            divCol3.Controls.Add(etichettaContatoreA);
            divCol4.Controls.Add(contatoreA);

            divRowValueFrom.Controls.Add(divCol1);
            divRowValueFrom.Controls.Add(divCol2);
            divRowValueFrom.Controls.Add(divCol3);
            divRowValueFrom.Controls.Add(divCol4);

            

            impostaDirittiRuoloContatore(etichettaContatore, contatoreDa, contatoreA, etichettaContatoreDa, etichettaContatoreA, etichettaDDL, ddl, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (paneldll)
            {
                this.PnlTypeDocument.Controls.Add(divRowEtiDll);
                this.PnlTypeDocument.Controls.Add(divRowDll);
            }

            if (contatoreDa.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValueFrom);
            }

            #region DATA REPERTORIAZIONE
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_ENABLE_DATA_REPERTORIO.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_ENABLE_DATA_REPERTORIO.ToString()).Equals("1"))
            {
                Label dataRepertorio = new Label();
                dataRepertorio.EnableViewState = true;
                if (oggettoCustom.REPERTORIO.Equals("1"))
                    dataRepertorio.Text = Utils.Languages.GetLabelFromCode("SearhDocumentDataRepertorio", language);
                else
                    dataRepertorio.Text = Utils.Languages.GetLabelFromCode("SearhDocumentDataInserimentoContatore", language);
                dataRepertorio.CssClass = "weight";
                Panel divEtichettaDataRepertorio = new Panel();
                divEtichettaDataRepertorio.CssClass = "row";
                divEtichettaDataRepertorio.EnableViewState = true;
                divEtichettaDataRepertorio.Controls.Add(dataRepertorio);

                Panel divFiltriDataRepertorio = new Panel();
                divFiltriDataRepertorio.CssClass = "row";
                divFiltriDataRepertorio.EnableViewState = true;

                Panel divFiltriDdlIntervalloDataRepertorio = new Panel();
                divFiltriDdlIntervalloDataRepertorio.CssClass = "col";
                divFiltriDdlIntervalloDataRepertorio.EnableViewState = true;
                DropDownList ddlIntervalloDataRepertorio = new DropDownList();
                ddlIntervalloDataRepertorio.EnableViewState = true;
                ddlIntervalloDataRepertorio.ID = "DdlIntervalloDataRepertorio_" + oggettoCustom.SYSTEM_ID.ToString();
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "0", Text = Utils.Languages.GetLabelFromCode("ddl_data0", language), Selected = true });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "1", Text = Utils.Languages.GetLabelFromCode("ddl_data1", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "2", Text = Utils.Languages.GetLabelFromCode("ddl_data2", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "3", Text = Utils.Languages.GetLabelFromCode("ddl_data3", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "4", Text = Utils.Languages.GetLabelFromCode("ddl_data4", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "5", Text = Utils.Languages.GetLabelFromCode("ddl_data5", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "6", Text = Utils.Languages.GetLabelFromCode("ddl_data6", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "7", Text = Utils.Languages.GetLabelFromCode("ddl_data7", language) });
                ddlIntervalloDataRepertorio.AutoPostBack = true;
                ddlIntervalloDataRepertorio.SelectedIndexChanged += DdlIntervalloDataRepertorio_SelectedIndexChanged;
                divFiltriDdlIntervalloDataRepertorio.Controls.Add(ddlIntervalloDataRepertorio);

                Panel divFiltriDataDa = new Panel();
                divFiltriDataDa.CssClass = "col";
                divFiltriDataDa.EnableViewState = true;
                Panel divFiltriLblDataDa = new Panel();
                divFiltriLblDataDa.CssClass = "col-no-margin-top";
                divFiltriLblDataDa.EnableViewState = true;
                Label lblDataDa = new Label();
                lblDataDa.ID = "LblDataRepertorioDa_" + oggettoCustom.SYSTEM_ID;
                lblDataDa.EnableViewState = true;
                lblDataDa.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                lblDataDa.CssClass = "weight";
                divFiltriLblDataDa.Controls.Add(lblDataDa);
                divFiltriDataDa.Controls.Add(divFiltriLblDataDa);
                CustomTextArea dataDa = new CustomTextArea();
                dataDa.EnableViewState = true;
                dataDa.ID = "TxtDataRepertorioDa_" + oggettoCustom.SYSTEM_ID.ToString();
                dataDa.CssClass = "txt_textdata datepicker";
                dataDa.CssClassReadOnly = "txt_textdata_disabled";
                dataDa.Style["width"] = "80px";
                divFiltriDataDa.Controls.Add(dataDa);

                Panel divFiltriDataA = new Panel();
                divFiltriDataA.CssClass = "col";
                divFiltriDataA.EnableViewState = true;
                Panel divFiltriLblDataA = new Panel();
                divFiltriLblDataA.CssClass = "col-no-margin-top";
                divFiltriLblDataA.EnableViewState = true;
                Label lblDataA = new Label();
                lblDataA.ID = "LblDataRepertorioA_" + oggettoCustom.SYSTEM_ID;
                lblDataA.EnableViewState = true;
                lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                lblDataA.CssClass = "weight";
                lblDataA.Visible = false;
                divFiltriLblDataA.Controls.Add(lblDataA);
                divFiltriDataA.Controls.Add(divFiltriLblDataA);
                CustomTextArea dataA = new CustomTextArea();
                dataA.EnableViewState = true;
                dataA.ID = "TxtDataRepertorioA_" + oggettoCustom.SYSTEM_ID.ToString();
                dataA.CssClass = "txt_textdata datepicker";
                dataA.CssClassReadOnly = "txt_textdata_disabled";
                dataA.Style["width"] = "80px";
                dataA.Visible = false;
                divFiltriDataA.Controls.Add(dataA);

                divFiltriDataRepertorio.Controls.Add(divFiltriDdlIntervalloDataRepertorio);
                divFiltriDataRepertorio.Controls.Add(divFiltriDataDa);
                divFiltriDataRepertorio.Controls.Add(divFiltriDataA);

                Panel divRowDataRepertorio = new Panel();
                divRowDataRepertorio.CssClass = "row";
                divRowDataRepertorio.EnableViewState = true;

                divRowDataRepertorio.Controls.Add(divEtichettaDataRepertorio);
                divRowDataRepertorio.Controls.Add(divFiltriDataRepertorio);

                if (contatoreDa.Visible)
                {
                    this.PnlTypeDocument.Controls.Add(divRowDataRepertorio);
                }

                #region BindFilterDataRepertorio

                if (!string.IsNullOrEmpty(oggettoCustom.DATA_INSERIMENTO))
                {
                    if (oggettoCustom.DATA_INSERIMENTO.IndexOf("@") != -1)
                    {
                        ddlIntervalloDataRepertorio.SelectedIndex = 1;
                        dataDa.ReadOnly = false;
                        dataA.ReadOnly = false;
                        lblDataA.Visible = true;
                        lblDataDa.Visible = true;
                        dataA.Visible = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        string[] dataInserimento = oggettoCustom.DATA_INSERIMENTO.Split('@');
                        dataDa.Text = dataInserimento[0].ToString();
                        dataA.Text = dataInserimento[1].ToString();
                    }
                    else
                    {
                        dataDa.Text = oggettoCustom.DATA_INSERIMENTO.ToString();
                        dataA.Text = "";
                    }
                }

                #endregion
            }
            #endregion
        }

        public void impostaDirittiRuoloContatore(Object etichettaContatore, Object contatoreDa, Object contatoreA, Object etichettaContatoreDa, Object etichettaContatoreA, Object etichettaDDL, Object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichettaContatore).Visible = false;

                        ((System.Web.UI.WebControls.Label)etichettaContatoreDa).Visible = false;
                        ((CustomTextArea)contatoreDa).Visible = false;
                        ((System.Web.UI.WebControls.Label)etichettaContatoreA).Visible = false;
                        ((CustomTextArea)contatoreA).Visible = false;

                        ((System.Web.UI.WebControls.Label)etichettaDDL).Visible = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Visible = false;
                    }
                }
            }
        }

        private void inserisciSelezioneEsclusiva(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            DocsPaWR.StoricoProfilatiOldValue selezEsclOldObj = new StoricoProfilatiOldValue();
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            Label etichettaSelezioneEsclusiva = new Label();
            etichettaSelezioneEsclusiva.EnableViewState = true;
            CustomImageButton cancella_selezioneEsclusiva = new CustomImageButton();
            string language = UIManager.UserManager.GetUserLanguage();
            cancella_selezioneEsclusiva.AlternateText = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Reset", language);
            cancella_selezioneEsclusiva.ToolTip = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Reset", language);
            cancella_selezioneEsclusiva.EnableViewState = true;


            etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE;


            cancella_selezioneEsclusiva.ID = "_" + oggettoCustom.SYSTEM_ID.ToString();
            cancella_selezioneEsclusiva.ImageUrl = "../Images/Icons/clean_field_custom.png";
            cancella_selezioneEsclusiva.OnMouseOutImage = "../Images/Icons/clean_field_custom.png";
            cancella_selezioneEsclusiva.OnMouseOverImage = "../Images/Icons/clean_field_custom_hover.png";
            cancella_selezioneEsclusiva.ImageUrlDisabled = "../Images/Icons/clean_field_custom_disabled.png";
            cancella_selezioneEsclusiva.CssClass = "clickable";
            cancella_selezioneEsclusiva.Click += cancella_selezioneEsclusiva_Click;
            etichettaSelezioneEsclusiva.CssClass = "weight";

            RadioButtonList selezioneEsclusiva = new RadioButtonList();
            selezioneEsclusiva.EnableViewState = true;
            selezioneEsclusiva.ID = oggettoCustom.SYSTEM_ID.ToString();
            //int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                //Valori disabilitati/abilitati
                if (valoreOggetto.ABILITATO == 1 || (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE))
                {
                    //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                    if (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE)
                        valoreOggetto.ABILITATO = 1;

                    selezioneEsclusiva.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                    //Valore di default
                    //if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                    //{
                    //    valoreDiDefault = i;
                    //}
                }
            }

            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                selezioneEsclusiva.RepeatDirection = RepeatDirection.Horizontal;
            }
            else
            {
                selezioneEsclusiva.RepeatDirection = RepeatDirection.Vertical;
            }
            //if (valoreDiDefault != -1)
            //{
            //    selezioneEsclusiva.SelectedIndex = valoreDiDefault;
            //}
            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                selezioneEsclusiva.SelectedIndex = impostaSelezioneEsclusiva(oggettoCustom.VALORE_DATABASE, selezioneEsclusiva);
            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            Panel divColImage = new Panel();
            divColImage.CssClass = "col-right-no-margin";
            divColImage.EnableViewState = true;

            divColImage.Controls.Add(cancella_selezioneEsclusiva);

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaSelezioneEsclusiva);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);

            divRowDesc.Controls.Add(divColDesc);
            divRowDesc.Controls.Add(divColImage);


            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;

            divColValue.Controls.Add(selezioneEsclusiva);
            divRowValue.Controls.Add(divColValue);



            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSelezioneEsclusiva(etichettaSelezioneEsclusiva, selezioneEsclusiva, cancella_selezioneEsclusiva, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (etichettaSelezioneEsclusiva.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (selezioneEsclusiva.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }
        }

        public void impostaDirittiRuoloSelezioneEsclusiva(Object etichetta, Object campo, Object button, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                        ((System.Web.UI.WebControls.RadioButtonList)campo).Visible = false;
                        //((System.Web.UI.HtmlControls.HtmlAnchor)button).Visible = false;
                        oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                    }
                }
            }
        }

        private void inserisciMenuATendina(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            DocsPaWR.StoricoProfilatiOldValue menuOldObj = new StoricoProfilatiOldValue();
            Label etichettaMenuATendina = new Label();
            etichettaMenuATendina.EnableViewState = true;
            etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE;

            etichettaMenuATendina.CssClass = "weight";

            int maxLenght = 0;
            DropDownList menuATendina = new DropDownList();
            menuATendina.EnableViewState = true;
            menuATendina.ID = oggettoCustom.SYSTEM_ID.ToString();
            //int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                //Valori disabilitati/abilitati
                //if (valoreOggetto.ABILITATO == 1 || (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE))
                //{
                //    //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                //    if (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE)
                //        valoreOggetto.ABILITATO = 1;

                menuATendina.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                //Valore di default
                //if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                //{
                //    valoreDiDefault = i;
                //}

                if (maxLenght < valoreOggetto.VALORE.Length)
                {
                    maxLenght = valoreOggetto.VALORE.Length;
                }
                //  }
            }
            menuATendina.CssClass = "chzn-select-deselect";
            string language = UIManager.UserManager.GetUserLanguage();
            menuATendina.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));
            menuATendina.Width = maxLenght + 250;

            //if (valoreDiDefault != -1)
            //{
            //    menuATendina.SelectedIndex = valoreDiDefault;
            //}
            //if (!(valoreDiDefault != -1 && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI")))
            //{
            menuATendina.Items.Insert(0, "");
            //}
            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                menuATendina.SelectedIndex = this.impostaSelezioneMenuATendina(oggettoCustom.VALORE_DATABASE, menuATendina);
            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaMenuATendina);
            parDesc.EnableViewState = true;



            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaMenuATendina, menuATendina, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (etichettaMenuATendina.Visible)
            {
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }


            if (menuATendina.Visible)
            {
                divColValue.Controls.Add(menuATendina);
                divRowValue.Controls.Add(divColValue);
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }
        }

        private void inserisciCampoDiTesto(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            Label etichettaCampoDiTesto = new Label();
            etichettaCampoDiTesto.EnableViewState = true;

            CustomTextArea txt_CampoDiTesto = new CustomTextArea();
            txt_CampoDiTesto.EnableViewState = true;

            if (oggettoCustom.MULTILINEA.Equals("SI"))
            {

                etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;

                etichettaCampoDiTesto.CssClass = "weight";

                txt_CampoDiTesto.CssClass = "txt_textarea";
                txt_CampoDiTesto.CssClassReadOnly = "txt_textarea_disabled";

                if (string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_LINEE))
                {
                    txt_CampoDiTesto.Height = 55;
                }
                else
                {
                    txt_CampoDiTesto.Rows = Convert.ToInt32(oggettoCustom.NUMERO_DI_LINEE);
                }

                if (string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_CARATTERI))
                {
                    txt_CampoDiTesto.MaxLength = 150;
                }
                else
                {
                    txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                }

                txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;
                txt_CampoDiTesto.TextMode = TextBoxMode.MultiLine;
            }
            else
            {

                etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;

                etichettaCampoDiTesto.CssClass = "weight";

                if (!string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_CARATTERI))
                {
                    //ATTENZIONE : La lunghezza della textBox non è speculare al numero massimo di
                    //caratteri che l'utente inserisce.
                    if (((Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI) * 6) <= 400))
                    {
                        txt_CampoDiTesto.Width = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI) * 6;
                    }
                    txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                }
                txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;
                txt_CampoDiTesto.CssClass = "txt_input_full";
                txt_CampoDiTesto.CssClassReadOnly = "txt_input_full_disabled";
                txt_CampoDiTesto.TextMode = TextBoxMode.SingleLine;


            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;


            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaCampoDiTesto, txt_CampoDiTesto, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (etichettaCampoDiTesto.Visible)
            {
                HtmlGenericControl parDesc = new HtmlGenericControl("p");
                parDesc.Controls.Add(etichettaCampoDiTesto);
                parDesc.EnableViewState = true;
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (txt_CampoDiTesto.Visible)
            {
                divColValue.Controls.Add(txt_CampoDiTesto);
                divRowValue.Controls.Add(divColValue);
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }
        }

        private void inserisciCasellaDiSelezione(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            DocsPaWR.StoricoProfilatiOldValue casellaSelOldObj = new StoricoProfilatiOldValue();
            Label etichettaCasellaSelezione = new Label();
            etichettaCasellaSelezione.EnableViewState = true;

            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE;
            }

            etichettaCasellaSelezione.Width = Unit.Percentage(100);
            etichettaCasellaSelezione.CssClass = "weight";

            CheckBoxList casellaSelezione = new CheckBoxList();
            casellaSelezione.EnableViewState = true;
            casellaSelezione.ID = oggettoCustom.SYSTEM_ID.ToString();
            //int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreElenco = ((ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                if (i < oggettoCustom.VALORI_SELEZIONATI.Length)
                {
                    string valoreSelezionato = (string)(oggettoCustom.VALORI_SELEZIONATI[i]);
                    if (valoreElenco.ABILITATO == 1 || (valoreElenco.ABILITATO == 0 && !string.IsNullOrEmpty(valoreSelezionato)))
                    {
                        //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                        if (valoreElenco.ABILITATO == 0 && !string.IsNullOrEmpty(valoreSelezionato))
                            valoreElenco.ABILITATO = 1;

                        casellaSelezione.Items.Add(new ListItem(valoreElenco.VALORE, valoreElenco.VALORE));
                        //Valore di default
                        //if (valoreElenco.VALORE_DI_DEFAULT.Equals("SI"))
                        //{
                        //    valoreDiDefault = i;
                        //}
                    }
                }
            }

            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                casellaSelezione.RepeatDirection = RepeatDirection.Horizontal;
            }
            else
            {
                casellaSelezione.RepeatDirection = RepeatDirection.Vertical;
            }
            //if (valoreDiDefault != -1)
            //{
            //    casellaSelezione.SelectedIndex = valoreDiDefault;
            //}

            if (oggettoCustom.VALORI_SELEZIONATI != null)
            {
                this.impostaSelezioneCaselleDiSelezione(oggettoCustom, casellaSelezione);
            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaCasellaSelezione);
            parDesc.EnableViewState = true;



            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColDesc.EnableViewState = true;



            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaCasellaSelezione, casellaSelezione, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (etichettaCasellaSelezione.Visible)
            {
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (casellaSelezione.Visible)
            {

                divColValue.Controls.Add(casellaSelezione);
                divRowValue.Controls.Add(divColValue);

                this.PnlTypeDocument.Controls.Add(divRowValue);
            }
        }

        private int impostaSelezioneEsclusiva(string valore, RadioButtonList rbl)
        {
            for (int i = 0; i < rbl.Items.Count; i++)
            {
                if (rbl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }

        private void impostaSelezioneCaselleDiSelezione(DocsPaWR.OggettoCustom objCustom, CheckBoxList cbl)
        {
            for (int i = 0; i < objCustom.VALORI_SELEZIONATI.Length; i++)
            {
                for (int j = 0; j < cbl.Items.Count; j++)
                {
                    if ((string)objCustom.VALORI_SELEZIONATI[i] == cbl.Items[j].Text)
                    {
                        cbl.Items[j].Selected = true;
                    }
                }
            }
        }

        private int impostaSelezioneMenuATendina(string valore, DropDownList ddl)
        {
            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }

        public void impostaDirittiRuoloSulCampo(Object etichetta, Object campo, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "CampoDiTesto":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((CustomTextArea)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "CasellaDiSelezione":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.CheckBoxList)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "MenuATendina":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.DropDownList)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "SelezioneEsclusiva":
                            //Per la selezione esclusiva è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                            break;
                        case "Contatore":
                            //Per il contatore è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                            break;
                        case "Data":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((UserControls.Calendar)campo).Visible = false;
                                ((UserControls.Calendar)campo).VisibleTimeMode = UserControls.Calendar.VisibleTimeModeEnum.Nothing;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Corrispondente":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((UserControls.CorrespondentCustom)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Link":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((UserControls.LinkDocFasc)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "OggettoEsterno":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((UserControls.IntegrationAdapter)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                    }
                }
            }
        }

        protected void EnableDisableSave()
        {
            if (GridManager.SelectedGrid != null && string.IsNullOrEmpty(GridManager.SelectedGrid.GridId))
            {
                this.projectImgSaveGrid.Enabled = true;
            }
            else
            {
                this.projectImgSaveGrid.Enabled = false;
            }
        }

        protected bool GetGridPersonalization()
        {
            return this.ShowGridPersonalization;
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OnlyNumbers", "OnlyNumbers();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CombineRowsHover", "CombineRowsHover();", true);
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            this.StartProcessSignature.Title = Utils.Languages.GetLabelFromCode("StartProcessSignature", language);
            this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.LtlDaDataProto.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.LtlDaDataScad.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.LtlDaDataStampa.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.LtlDaDataProtMitt.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.LtlDaDataArrivo.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.LtlDaDataSegDiEmerg.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.LtlDaDataSpedizione.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
            this.LtlDaDataRicevuta.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.LtlDaDataConsolidamento.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.LtlDaDataRicevutaPEC.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.LitObjectAttach.Text = Utils.Languages.GetLabelFromCode("SearchDocumentLitObjectAttach", language);
            this.Object.Title = Utils.Languages.GetLabelFromCode("TitleObjectPopup", language);
            this.ObjectFromMainDocument.Title = Utils.Languages.GetLabelFromCode("TitleObjectPopup", language);
            if (this.IsAdl)
            {
                this.LitSearchProject.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedTitleAdl", language);
                this.RblTypeAdl.Items[0].Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdlUser", language);
                this.RblTypeAdl.Items[1].Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdlRole", language);
            }
            else
            {
                this.LitSearchProject.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedTitle", language);
            }

            this.SearchDocumentAdvancedSearch.Text = Utils.Languages.GetLabelFromCode("SearchLabelButton", language);
            this.SearchDocumentAdvancedSave.Text = Utils.Languages.GetLabelFromCode("SearchLabelSearchButton", language);
            this.SearchDocumentAdvancedRemove.Text = Utils.Languages.GetLabelFromCode("SearchLabelRemoveButton", language);
            this.SearchDocumentRemoveFilters.Text = Utils.Languages.GetLabelFromCode("SearchLabelRemoveFiltersButton", language);
            //this.SearchDocumentLitRecord.Text = Utils.Languages.GetLabelFromCode("SearchDocumentLitRecord", language);
            this.DdlRapidSearch.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DdlRapidSearch", language));
            this.DocumentLitObject.Text = Utils.Languages.GetLabelFromCode("DocumentLitObject", language);
            this.DocumentLitObjectAttach.Text = Utils.Languages.GetLabelFromCode("DocumentLitObjectAttach", language);
            this.DocumentImgObjectary.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgObjectary", language);
            this.DocumentImgObjectary.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgObjectary", language);
            this.DocumentImgObjectaryAttach.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgObjectary", language);
            this.DocumentImgObjectaryAttach.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgObjectary", language);
            //this.DocumentLitObjectChAv.Text = Utils.Languages.GetLabelFromCode("DocumentLitObjectChAv", language);
            this.SearchDocumentLitRapidSearch.Text = Utils.Languages.GetLabelFromCode("SearchDocumentLitRapidSearch", language);
            this.SearchDocumentTypeDocument.Text = Utils.Languages.GetLabelFromCode("SearchDocumentTypeDocument", language);
            this.opPredisposed.Text = Utils.Languages.GetLabelFromCode("opOredisposed", language);
            this.opPrints.Text = Utils.Languages.GetLabelFromCode("opPrints", language);
            this.SearchDocumentDdlMassiveOperation.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("projectDdlAzioniMassive", language));
            this.projectImgEditGrid.ToolTip = Utils.Languages.GetLabelFromCode("projectImgEditGrid", language);
            this.projectImgPreferredGrids.ToolTip = Utils.Languages.GetLabelFromCode("projectImgPreferredGrids", language);
            this.projectImgSaveGrid.ToolTip = Utils.Languages.GetLabelFromCode("projectImgSaveGrid", language);
            this.projectImgEditGrid.AlternateText = Utils.Languages.GetLabelFromCode("projectImgEditGrid", language);
            this.projectImgPreferredGrids.AlternateText = Utils.Languages.GetLabelFromCode("projectImgPreferredGrids", language);
            this.projectImgSaveGrid.AlternateText = Utils.Languages.GetLabelFromCode("projectImgSaveGrid", language);
            this.projectLitNomeGriglia.Text = Utils.Languages.GetLabelFromCode("projectLitNomeGriglia", language);
            this.SearchDocumentAdvancedEdit.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedEdit", language);
            this.GridPersonalizationPreferred.Title = Utils.Languages.GetLabelFromCode("projectImgPreferredGrids", language);
            this.GrigliaPersonalizzata.Title = Utils.Languages.GetLabelFromCode("projectImgEditGrid", language);
            this.GrigliaPersonalizzataSave.Title = Utils.Languages.GetLabelFromCode("projectImgSaveGrid", language);
            this.DocumentViewer.Title = Utils.Languages.GetLabelFromCode("TitleDocumentViewerPopup", language);
            this.SearchDocumentYear.Text = Utils.Languages.GetLabelFromCode("SearchDocumentYear", language);
            this.SearchDocumentLitTypology.Text = Utils.Languages.GetLabelFromCode("SearchDocumentLitTypology", language);
            this.DocumentDdlTypeDocument.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DocumentDdlTypeDocument", language));
            this.ddlStateCondition.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));
            this.DocumentDdlStateDiagram.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DocumentDdlStateDiagram", language));
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
            this.LtlDaIdDoc.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
            this.LtlDaNumProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);

            this.LtlAIdDoc.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlANumProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlADataProto.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlADataScad.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlADataStampa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlADataProtMitt.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlADataArrivo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlADataSegDiEmerg.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlADataSpedizione.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlADataRicevuta.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlADataConsolidamento.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.LtlRegistro.Text = Utils.Languages.GetLabelFromCode("LtlRegistro", language);
            this.LtlProtMitt.Text = Utils.Languages.GetLabelFromCode("LtlProtMitt", language);
            this.LtlMezzoSpediz.Text = Utils.Languages.GetLabelFromCode("LtlMezzoSpediz", language);
            this.SearchDocumentLit.Text = Utils.Languages.GetLabelFromCode("SearchDocumentLit", language);
            this.cbx_pec.Text = Utils.Languages.GetLabelFromCode("SearchDocumentCbxPec", language);
            this.LtlIdDoc.Text = Utils.Languages.GetLabelFromCode("LtlIdDoc", language);
            this.LtlDataCreazione.Text = Utils.Languages.GetLabelFromCode("LtlDataCreazione", language);
            this.SearchProtocolloLit.Text = Utils.Languages.GetLabelFromCode("SearchProtocolloLit", language);
            this.LtlNumProto.Text = Utils.Languages.GetLabelFromCode("LtlNumProto", language);
            this.LtlDataProto.Text = Utils.Languages.GetLabelFromCode("LtlDataProto", language);
            this.LtlMitDest.Text = Utils.Languages.GetLabelFromCode("LtlMitDest", language);
            this.chk_mitt_dest_storicizzati.Text = Utils.Languages.GetLabelFromCode("chk_mitt_dest_storicizzati", language);
            this.LtlDataScad.Text = Utils.Languages.GetLabelFromCode("LtlDataScad", language);
            this.LtlDataStampa.Text = Utils.Languages.GetLabelFromCode("LtlDataStampa", language);
            this.cb_Conservato.Text = Utils.Languages.GetLabelFromCode("cb_Conservato", language);
            this.cb_NonConservato.Text = Utils.Languages.GetLabelFromCode("cb_NonConservato", language);
            this.LtlStatodelDoc.Text = Utils.Languages.GetLabelFromCode("LtlStatodelDoc", language);
            this.LtlSegnatura.Text = Utils.Languages.GetLabelFromCode("LtlSegnatura", language);
            this.LtlMittItermedio.Text = Utils.Languages.GetLabelFromCode("LtlMittItermedio", language);
            this.LtlCodFascGenProc.Text = Utils.Languages.GetLabelFromCode("LtlCodFascGenProc", language);
            this.LtlDataProtMitt.Text = Utils.Languages.GetLabelFromCode("LtlDataProtMitt", language);
            this.LtlDataArrivo.Text = Utils.Languages.GetLabelFromCode("LtlDataArrivo", language);
            this.LtlParolaChiave.Text = Utils.Languages.GetLabelFromCode("LtlParolaChiave", language);
            this.LtlNote.Text = Utils.Languages.GetLabelFromCode("LtlNote", language);
            //this.LitNoteObjectChAv.Text = Utils.Languages.GetLabelFromCode("LitNoteObjectChAv", language);
            this.LtlDocInCompletamento.Text = Utils.Languages.GetLabelFromCode("LtlDocInCompletamento", language);
            this.cbx_Trasm.Text = Utils.Languages.GetLabelFromCode("cbx_Trasm", language);
            this.cbx_TrasmSenza.Text = Utils.Languages.GetLabelFromCode("cbx_TrasmSenza", language);
            this.LtlRagione.Text = Utils.Languages.GetLabelFromCode("LtlRagione", language);
            this.LtlSegnatDiEmerg.Text = Utils.Languages.GetLabelFromCode("LtlSegnatDiEmerg", language);
            this.LtlDataSegDiEmerg.Text = Utils.Languages.GetLabelFromCode("LtlDataSegDiEmerg", language);
            this.LtlEvidenza.Text = Utils.Languages.GetLabelFromCode("LtlEvidenza", language);
            this.LtlTipoFileAcq.Text = Utils.Languages.GetLabelFromCode("LtlTipoFileAcq", language);
            this.chkFirmato.Text = Utils.Languages.GetLabelFromCode("chkFirmato", language);
            //this.chkFirmatarioExtendHistoricized.Text = Utils.Languages.GetLabelFromCode("SearchProjectCreatoreExtendHistoricized", language);
            this.chkNonFirmato.Text = Utils.Languages.GetLabelFromCode("chkNonFirmato", language);
            this.chkFirmaElettronica.Text = Utils.Languages.GetLabelFromCode("chkFirmaElettronica", language);
            this.LtlVersioni.Text = Utils.Languages.GetLabelFromCode("LtlVersioni", language);
            this.LtlNumVersDoc.Text = Utils.Languages.GetLabelFromCode("LtlNumVersDoc", language);
            this.LtlAllegati.Text = Utils.Languages.GetLabelFromCode("LtlAllegati", language);
            this.LtlNumAllegatiDoc.Text = Utils.Languages.GetLabelFromCode("LtlNumAllegatiDoc", language);
            this.LtlDocSpediti.Text = Utils.Languages.GetLabelFromCode("LtlDocSpediti", language);
            this.LtlDocSpeditiEsito.Text = Utils.Languages.GetLabelFromCode("LtlDocSpeditiEsito", language);
            this.LtlDataSpedizione.Text = Utils.Languages.GetLabelFromCode("LtlDataSpedizione", language);
            this.LtlRicPTRE.Text = Utils.Languages.GetLabelFromCode("LtlRicPTRE", language) + " " + SimplifiedInteroperabilityManager.SearchItemDescriprion;
            this.LtlRicPEC.Text = Utils.Languages.GetLabelFromCode("LtlRicPEC", language);
            this.LtlRicDi.Text = Utils.Languages.GetLabelFromCode("LtlRicDi", language);
            this.LtlRicDiPEC.Text = Utils.Languages.GetLabelFromCode("LtlRicDiPEC", language);
            this.LtlDataRicevuta.Text = Utils.Languages.GetLabelFromCode("LtlDataRicevuta", language);
            this.LtlDataRicevutaPec.Text = Utils.Languages.GetLabelFromCode("LtlDataRicevuta", language);
            this.LtlStatoConsolid.Text = Utils.Languages.GetLabelFromCode("LtlStatoConsolid", language);
            this.LtlDataConsolidamento.Text = Utils.Languages.GetLabelFromCode("LtlDataConsolidamento", language);
            this.cbl_docInCompl.Items[0].Text = Utils.Languages.GetLabelFromCode("cbl_docInCompl0", language);
            this.cbl_docInCompl.Items[1].Text = Utils.Languages.GetLabelFromCode("cbl_docInCompl1", language);
            this.cbl_docInCompl.Items[2].Text = Utils.Languages.GetLabelFromCode("cbl_docInCompl2", language);
            this.cbl_docInCompl.Items[3].Text = Utils.Languages.GetLabelFromCode("cbl_docInCompl3", language);
            this.ddl_idDocumento_C.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_idDocumento_C0", language);
            this.ddl_idDocumento_C.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_idDocumento_C1", language);
            this.ddl_dataCreazione_E.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_dataCreazione_E.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_dataCreazione_E.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_dataCreazione_E.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_dataCreazione_E.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.ddl_dataCreazione_E.Items[5].Text = Utils.Languages.GetLabelFromCode("ddl_data5", language);
            this.ddl_dataCreazione_E.Items[6].Text = Utils.Languages.GetLabelFromCode("ddl_data6", language);
            this.ddl_dataCreazione_E.Items[7].Text = Utils.Languages.GetLabelFromCode("ddl_data7", language);
            this.ddl_numProt_E.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E0", language);
            this.ddl_numProt_E.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E1", language);

            this.ddl_dataProt_E.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_dataProt_E.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_dataProt_E.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_dataProt_E.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_dataProt_E.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.ddl_dataProt_E.Items[5].Text = Utils.Languages.GetLabelFromCode("ddl_data5", language);
            this.ddl_dataProt_E.Items[6].Text = Utils.Languages.GetLabelFromCode("ddl_data6", language);
            this.ddl_dataProt_E.Items[7].Text = Utils.Languages.GetLabelFromCode("ddl_data7", language);
            this.ddl_dataScadenza_C.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_dataScadenza_C.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_dataScadenza_C.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_dataScadenza_C.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_dataScadenza_C.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.ddl_dataStampa_E.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_dataStampa_E.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_dataStampa_E.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_dataStampa_E.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_dataStampa_E.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.ddl_dataProtMitt_C.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_dataProtMitt_C.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_dataProtMitt_C.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_dataProtMitt_C.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_dataProtMitt_C.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.ddl_dataArrivo_C.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_dataArrivo_C.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_dataArrivo_C.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_dataArrivo_C.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_dataArrivo_C.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.ddl_dataProtoEme.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_dataProtoEme.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_dataProtoEme.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_dataProtoEme.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_dataProtoEme.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.ddl_data_ricevute_pitre.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_data_ricevute_pitre.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_data_ricevute_pitre.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_data_ricevute_pitre.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_data_ricevute_pitre.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.ddl_data_ricevute_pec.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_data_ricevute_pec.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_data_ricevute_pec.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_data_ricevute_pec.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_data_ricevute_pec.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.cboDataConsolidamento.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.cboDataConsolidamento.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.cboDataConsolidamento.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.cboDataConsolidamento.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.cboDataConsolidamento.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);

            this.rb_annulla_C.Items[0].Text = Utils.Languages.GetLabelFromCode("rb_annulla_C0", language);
            this.rb_annulla_C.Items[1].Text = Utils.Languages.GetLabelFromCode("rb_annulla_C1", language);
            this.rb_annulla_C.Items[2].Text = Utils.Languages.GetLabelFromCode("rb_annulla_C2", language);

            if (this.rl_visibilita.Items.Count >= 5)
            {
                this.rl_visibilita.Items[0].Text = Utils.Languages.GetLabelFromCode("rl_visibilita0", language);
                this.rl_visibilita.Items[1].Text = Utils.Languages.GetLabelFromCode("rl_visibilita1", language);
                this.rl_visibilita.Items[2].Text = Utils.Languages.GetLabelFromCode("rl_visibilita2", language);
                this.rl_visibilita.Items[3].Text = Utils.Languages.GetLabelFromCode("rl_visibilita3", language);
                this.rl_visibilita.Items[4].Text = Utils.Languages.GetLabelFromCode("rl_visibilita4", language);
            }
            else
            {
                this.rl_visibilita.Items[0].Text = Utils.Languages.GetLabelFromCode("rl_visibilita0", language);
                this.rl_visibilita.Items[1].Text = Utils.Languages.GetLabelFromCode("rl_visibilita1", language);
                this.rl_visibilita.Items[2].Text = Utils.Languages.GetLabelFromCode("rl_visibilita2", language);
                this.rl_visibilita.Items[3].Text = Utils.Languages.GetLabelFromCode("rl_visibilita4", language);
            }

            this.rb_evidenza_C.Items[0].Text = Utils.Languages.GetLabelFromCode("rb_evidenza_C0", language);
            this.rb_evidenza_C.Items[1].Text = Utils.Languages.GetLabelFromCode("rb_evidenza_C1", language);
            this.rb_evidenza_C.Items[2].Text = Utils.Languages.GetLabelFromCode("rb_evidenza_C2", language);

            this.optUO.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptUO", language);
            this.optPropUO.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptUO", language);
            this.optRole.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptRole", language);
            this.optPropRole.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptRole", language);
            this.optUser.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptUser", language);
            this.optPropUser.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptUser", language);
            this.chkCreatoreExtendHistoricized.Text = Utils.Languages.GetLabelFromCode("SearchProjectCreatoreExtendHistoricized", language);

            this.rblFiltriNumAllegati.Items[0].Text = Utils.Languages.GetLabelFromCode("rblFiltriNumAllegati0", language);
            this.rblFiltriNumAllegati.Items[1].Text = Utils.Languages.GetLabelFromCode("rblFiltriNumAllegati1", language);
            this.rblFiltriNumAllegati.Items[2].Text = Utils.Languages.GetLabelFromCode("rblFiltriNumAllegati2", language);
            this.rblFiltriNumAllegati.Items[3].Text = Utils.Languages.GetLabelFromCode("rblFiltriNumAllegati3", language);

            this.rb_docSpediti.Items[0].Text = Utils.Languages.GetLabelFromCode("rb_docSpediti_conferma", language);
            this.rb_docSpediti.Items[1].Text = Utils.Languages.GetLabelFromCode("rb_docSpediti_annullamento", language);
            this.rb_docSpediti.Items[2].Text = Utils.Languages.GetLabelFromCode("rb_docSpediti_eccezione", language);
            this.rb_docSpediti.Items[3].Text = Utils.Languages.GetLabelFromCode("rb_docSpediti1", language);
            this.rb_docSpediti.Items[4].Text = Utils.Languages.GetLabelFromCode("rb_docSpediti2", language);
            this.rb_docSpediti.Items[5].Text = Utils.Languages.GetLabelFromCode("rb_docSpediti3", language);

            this.rb_docSpeditiEsito.Items[0].Text = Utils.Languages.GetLabelFromCode("rb_docSpeditiEsito0", language);
            this.rb_docSpeditiEsito.Items[1].Text = Utils.Languages.GetLabelFromCode("rb_docSpeditiEsito1", language);
            this.rb_docSpeditiEsito.Items[2].Text = Utils.Languages.GetLabelFromCode("rb_docSpeditiEsito2", language);
            this.rb_docSpeditiEsito.Items[3].Text = Utils.Languages.GetLabelFromCode("rb_docSpediti3", language);

            this.ddl_ricevute_pitre.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_ricevute_pitre1", language);
            this.ddl_ricevute_pitre.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_ricevute_pitre2", language);

            this.ddl_ricevute_pec.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_ricevute_pec1", language);
            this.ddl_ricevute_pec.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_ricevute_pec2", language);
            this.ddl_ricevute_pec.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_ricevute_pec3", language);
            this.ddl_ricevute_pec.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_ricevute_pec4", language);
            this.ddl_ricevute_pec.Items[5].Text = Utils.Languages.GetLabelFromCode("ddl_ricevute_pec5", language);

            this.lstFiltriConsolidamento.Items[0].Text = Utils.Languages.GetLabelFromCode("lstFiltriConsolidamento0", language);
            this.lstFiltriConsolidamento.Items[1].Text = Utils.Languages.GetLabelFromCode("lstFiltriConsolidamento1", language);
            this.lstFiltriConsolidamento.Items[2].Text = Utils.Languages.GetLabelFromCode("lstFiltriConsolidamento2", language);

            this.litCreator.Text = Utils.Languages.GetLabelFromCode("LtlCreatore", language);
            this.litOwner.Text = Utils.Languages.GetLabelFromCode("LtlProprietario", language);
            this.ltlFirmatario.Text = Utils.Languages.GetLabelFromCode("ltlFirmatario", language);       
            this.optFirmatarioRole.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptRole", language);
            this.optFirmatarioUser.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptUser", language);

            this.ImgProprietarioAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgRecipientAddressBookAuthor", language);
            this.ImgCreatoreAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgRecipientAddressBookOwner", language);
            this.DocumentImgRecipientAddressBookMittDest.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgRecipientAddressBookMittDest", language);
            this.DocumentImgRecipientAddressBookMittInter.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgRecipientAddressBookMittInter", language);
            this.SearchProjectImg.ToolTip = Utils.Languages.GetLabelFromCode("SearchProjectImg", language);
            this.SearchProjectImg.AlternateText = Utils.Languages.GetLabelFromCode("SearchProjectImg", language);
            this.ImgSelectKeyword.ToolTip = Utils.Languages.GetLabelFromCode("ImgSelectKeyword", language);

            this.ddlStateCondition.Items[0].Text = Utils.Languages.GetLabelFromCode("ddlStateCondition0", language);
            this.ddlStateCondition.Items[1].Text = Utils.Languages.GetLabelFromCode("ddlStateCondition1", language);

            this.ddl_spedizione.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language));
            this.ddl_ragioneTrasm.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language));
            this.ddl_tipoFileAcquisiti.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language));
            this.ddl_ricevute_pitre.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language));
            this.ddl_ricevute_pec.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language));

            this.rbl_timestamp0.Text = Utils.Languages.GetLabelFromCode("rbl_timestamp0", language);
            this.rbl_timestamp1.Text = Utils.Languages.GetLabelFromCode("rbl_timestamp1", language);
            this.rbl_timestamp2.Text = Utils.Languages.GetLabelFromCode("rbl_timestamp2", language);
            this.ddl_timestamp1.Text = Utils.Languages.GetLabelFromCode("ddl_timestamp1", language);
            this.ddl_timestamp2.Text = Utils.Languages.GetLabelFromCode("ddl_timestamp2", language);

            this.litUsrConsolidamento.Text = Utils.Languages.GetLabelFromCode("LtlUsrConsolidamento", language);
            this.litCodAmm.Text = Utils.Languages.GetLabelFromCode("litCodAmm", language);
            this.litNumOggetto.Text = Utils.Languages.GetLabelFromCode("litNumOggetto", language);
            this.litCommRef.Text = Utils.Languages.GetLabelFromCode("litCommRef", language);

            this.litVisibility.Text = Utils.Languages.GetLabelFromCode("SearchProjectVisibility", language);
            this.optVisibility1.Text = Utils.Languages.GetLabelFromCode("SearchProjectVisibilityOpt1", language);
            this.optVisibility2.Text = Utils.Languages.GetLabelFromCode("SearchProjectVisibilityOpt2", language);
            this.optVisibility3.Text = Utils.Languages.GetLabelFromCode("SearchProjectVisibilityOpt3", language);

            this.SaveSearch.Title = Utils.Languages.GetLabelFromCode("SearchProjectSaveSearchTitle", language);
            this.ModifySearch.Title = Utils.Languages.GetLabelFromCode("SearchProjectModifySearchTitle", language);

            this.SearchProject.Title = Utils.Languages.GetLabelFromCode("SearchProjectTitleSearch", language);

            this.btnclassificationschema.AlternateText = Utils.Languages.GetLabelFromCode("btnclassificationschema", language);
            this.btnclassificationschema.ToolTip = Utils.Languages.GetLabelFromCode("btnclassificationschema", language);
            this.MassiveSignatureHSM.Title = Utils.Languages.GetLabelFromCode("MassiveSignatureHSMTitle", language);
            this.OpenTitolario.Title = Utils.Languages.GetLabelFromCode("TitleClassificationScheme", language);
            this.InfoSignatureProcessesStarted.Title = Utils.Languages.GetLabelFromCode("InfoSignatureProcessesStarted", language);
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

            this.MassiveReport.Title = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
            this.MassiveAddAdlUser.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveAddAdlUserTitle", language);
            this.MassiveRemoveAdlUser.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveAdlUserTitle", language);
            this.MassiveAddAdlRole.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveAddAdlRoleTitle", language);
            this.MassiveRemoveAdlRole.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveAdlRoleTitle", language);
            this.MassiveConservation.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConservationTitle", language);
            this.MassiveTransmission.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveTransmissionTitle", language);
            this.MassiveConversion.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConversionTitle", language);
            this.MassiveTimestamp.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveTimestampTitle", language);
            this.MassiveConsolidation.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConsolidationTitle", language);
            this.MassiveConsolidationMetadati.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConsolidationMetadatiTitle", language);
            this.MassiveForward.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveForwardTitle", language);
            this.MassiveCollate.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveCollateTitle", language);
            this.MassiveRemoveVersions.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveVersionsTitle", language);
            this.MassiveDigitalSignature.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveDigitalSignatureTitle", language);
            this.MassiveDigitalSignatureApplet.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveDigitalSignatureTitle", language);
            this.MassiveDigitalSignatureSocket.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveDigitalSignatureTitle", language);
            this.ExportDati.Title = Utils.Languages.GetLabelFromCode("SearchDocumentExportDatiTitle", language);
            this.OpenTitolarioMassive.Title = Utils.Languages.GetLabelFromCode("TitleClassificationScheme", language);
            this.SearchProjectMassive.Title = Utils.Languages.GetLabelFromCode("SearchProjectTitle", language);
            this.DigitalSignDetails.Title = Utils.Languages.GetLabelFromCode("DigitalSignDetailsTitle", language);
            this.MassiveVersPARER.Title = Utils.Languages.GetLabelFromCode("MassiveVersTitle", language);

            this.LtlStatoCons.Text = Utils.Languages.GetLabelFromCode("LtlStatoCons", language);
            this.LtlDataVers.Text = Utils.Languages.GetLabelFromCode("LtlDataVers", language);
            this.LtlDaDataVers.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.LtlPolicy.Text = Utils.Languages.GetLabelFromCode("LtlPolicy", language);
            this.LtlCodPolicy.Text = Utils.Languages.GetLabelFromCode("LtlCodPolicy", language);
            this.LtlCounterPolicy.Text = Utils.Languages.GetLabelFromCode("LtlCounterPolicy", language);
            this.ltlDatePolicy.Text = Utils.Languages.GetLabelFromCode("LtlDatePolicy", language);
            this.LtlDaDatePolicy.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            
            this.optConsNC.Text = Utils.Languages.GetLabelFromCode("optConsNC", language);
            this.optConsAtt.Text = Utils.Languages.GetLabelFromCode("optConsAtt", language);
            this.optConsVer.Text = Utils.Languages.GetLabelFromCode("optConsVer", language);
            this.optConsPre.Text = Utils.Languages.GetLabelFromCode("optConsPre", language);
            this.optConsRif.Text = Utils.Languages.GetLabelFromCode("optConsRif", language);
            this.optConsErr.Text = Utils.Languages.GetLabelFromCode("optConsErr", language);
            this.optConsTim.Text = Utils.Languages.GetLabelFromCode("optConsTim", language);
            this.optConsFld.Text = Utils.Languages.GetLabelFromCode("optConsFld", language);
            this.optConsBfw.Text = Utils.Languages.GetLabelFromCode("optConsBfw", language);
            this.optConsBfe.Text = Utils.Languages.GetLabelFromCode("optConsBfe", language);
            this.ddl_DataVers.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_DataVers.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_DataVers.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_DataVers.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_DataVers.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.ddl_datePolicy.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_datePolicy.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_datePolicy.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_datePolicy.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_datePolicy.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.ddl_datePolicy.Items[5].Text = Utils.Languages.GetLabelFromCode("ddl_data5", language);
            this.rb_allRole.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedAllUser", language);
            this.rb_roleUser.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedMyRole", language);
            this.rb_onlyUser.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedOnlyUser", language);
            this.LtlNeverSendFrom.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlNeverSendFrom", language);

            this.rb_allRoleTrasm.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedAllUser", language);
            this.rb_roleUserTrasm.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedMyRole", language);
            this.rb_onlyUserTrasm.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedOnlyUser", language);
            this.LtlNeverTrasmFrom.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlNeverTrasmFrom", language);
            this.cb_neverSend.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlNeverSend", language);
            this.DetailsLFAutomaticMode.Title = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeTitle", language);
            
            this.opRepertorio.Text = Utils.Languages.GetLabelFromCode("SearchDocumentRep", language);
            this.LtlNumRep.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedNumRep", language);
            this.LtlDaNumRep.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
            this.LtlANumRep.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
            this.ddl_numRep.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E0", language);
            this.ddl_numRep.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_numProt_E1", language);
            this.LtlDataRepertorio.Text = Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDataRepertorio", language);
            this.ddl_dataRepertorio.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_data0", language);
            this.ddl_dataRepertorio.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_data1", language);
            this.ddl_dataRepertorio.Items[2].Text = Utils.Languages.GetLabelFromCode("ddl_data2", language);
            this.ddl_dataRepertorio.Items[3].Text = Utils.Languages.GetLabelFromCode("ddl_data3", language);
            this.ddl_dataRepertorio.Items[4].Text = Utils.Languages.GetLabelFromCode("ddl_data4", language);
            this.LtlDaDataRep.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.cbx_pitre.Text = SimplifiedInteroperabilityManager.SearchItemDescriprion;
            this.rbOpIS.Text = SimplifiedInteroperabilityManager.SearchItemDescriprion;
            this.rblFiltriNumAllegatiOpIS.Text = SimplifiedInteroperabilityManager.SearchItemDescriprion;
            this.cbxEstendiAFascicoli.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedCbxEstendiAFascicolo", language);
        }

        protected void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.ObjectFromMainDocument.ReturnValue))
            {
                this.TxtObjectAttach.Text = this.ReturnValue.Split('#').First();
                if (this.ReturnValue.Split('#').Length > 1)
                    this.TxtCodeObjectAttach.Text = this.ReturnValue.Split('#').Last();
                this.UpdPnlObjectAttach.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ObjectFromMainDocument','');", true);
            }
            if (!string.IsNullOrEmpty(this.Object.ReturnValue))
            {
                this.TxtObject.Text = this.ReturnValue.Split('#').First();
                if (this.ReturnValue.Split('#').Length > 1)
                    this.TxtCodeObject.Text = this.ReturnValue.Split('#').Last();
                this.UpdPnlObject.Update();

                //TxtCodeProject_OnTextChanged(new object(), new EventArgs());
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('Object','');", true);
            }

            if (!string.IsNullOrEmpty(this.SelectKeyword.ReturnValue))
            {
                this.ListKeywords.Attributes.Clear();
                this.ListKeywords.Items.Clear();
                foreach (DocsPaWR.DocumentoParolaChiave key in (Session["ReturnValuePopup"] as DocsPaWR.DocumentoParolaChiave[]))
                    this.ListKeywords.Items.Add(new ListItem(key.descrizione, key.systemId));
                this.UpParolaChiave.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SelectKeyword','');", true);
            }

            if (!string.IsNullOrEmpty(this.SearchProject.ReturnValue))
            {
                if (this.ReturnValue.Split('#').Length > 1)
                {
                    this.txt_CodFascicolo.Text = this.ReturnValue.Split('#').First();
                    this.txt_DescFascicolo.Text = this.ReturnValue.Split('#').Last();
                    this.UpCodFasc.Update();
                    this.txt_CodFascicolo_OnTextChanged(new object(), new EventArgs());
                }
                else if (this.ReturnValue.Contains("//"))
                {
                    this.txt_CodFascicolo.Text = string.Empty;
                    this.txt_DescFascicolo.Text = string.Empty;
                    this.UpCodFasc.Update();
                    this.txt_CodFascicolo_OnTextChanged(new object(), new EventArgs());
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SearchProject','');", true);
            }

            if (!string.IsNullOrEmpty(this.HiddenRemoveUsedSearch.Value))
            {
                try
                {
                    schedaRicerca.Cancella(Int32.Parse(this.DdlRapidSearch.SelectedValue));
                    Session.Remove("itemUsedSearch");
                    this.DdlRapidSearch.SelectedIndex = 0;
                    this.SearchDocumentAdvancedEdit.Enabled = false;
                    this.SearchDocumentAdvancedRemove.Enabled = false;
                    this.PopulateDDLSavedSearches();
                    this.UpPnlRapidSearch.Update();

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('InfoSearchProjectRemoveSearch', 'info', '');", true);
                }
                catch (Exception ex)
                {
                    string msg = utils.FormatJs("Impossibile rimuovere i criteri di ricerca. Errore: " + ex.Message);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorCustom', 'error', '', '" + msg + "');", true);
                }

                this.HiddenRemoveUsedSearch.Value = string.Empty;
                this.upPnlButtons.Update();
            }

            if (!string.IsNullOrEmpty(this.SaveSearch.ReturnValue))
            {
                this.PopulateDDLSavedSearches();
                this.SearchDocumentAdvancedEdit.Enabled = true;
                this.SearchDocumentAdvancedRemove.Enabled = true;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SaveSearch','');", true);
            }

            if (!string.IsNullOrEmpty(this.ModifySearch.ReturnValue))
            {
                this.PopulateDDLSavedSearches();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ModifySearch','');", true);
            }

            if (!string.IsNullOrEmpty(this.GrigliaPersonalizzata.ReturnValue))
            {
                this.UpContainerProjectTab.Update();
                this.UpnlTabHeader.Update();

                bool result = this.SearchDocumentFilters();
                if (this.Result != null && this.Result.Length > 0)
                {
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                }
                else
                {
                    this.ShowGrid(GridManager.SelectedGrid, null, 0, 0, this.Labels.ToArray<EtichettaInfo>());
                }

                //if (this.Result != null && this.Result.Length > 0)
                //{
                //    this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
                //    this.SearchDocumentDdlMassiveOperation.Enabled = true;
                //}
                //else
                //{
                //    this.ShowGrid(GridManager.SelectedGrid, null, 0, 0, this.Labels.ToArray<EtichettaInfo>());
                //}
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('GrigliaPersonalizzata','');", true);
            }

            if (!string.IsNullOrEmpty(this.GrigliaPersonalizzataSave.ReturnValue))
            {
                this.UpContainerProjectTab.Update();
                this.UpnlGrid.Update();
                this.UpnlTabHeader.Update();
                // Se ci sono risultati, vengono visualizzati
                if (this.Result != null && this.Result.Length > 0)
                {
                    this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
                    this.SearchDocumentDdlMassiveOperation.Enabled = true;
                }
                else
                {
                    this.ShowGrid(GridManager.SelectedGrid, null, 0, 0, this.Labels.ToArray<EtichettaInfo>());
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('GrigliaPersonalizzataSave','');", true);

                string msg = "InfoSaveGrid";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'info');} else {parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'info');}  ", true);
            }

            if (!string.IsNullOrEmpty(this.GridPersonalizationPreferred.ReturnValue))
            {
                this.UpContainerProjectTab.Update();
                this.UpnlGrid.Update();
                this.UpnlTabHeader.Update();
                /* Emanuela 18/04/2014: Modifica per aggiornare la griglia con tutti i campi
                // Se ci sono risultati, vengono visualizzati
                if (this.Result != null && this.Result.Length > 0)
                {
                    this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
                    this.SearchDocumentDdlMassiveOperation.Enabled = true;
                }
                else
                {
                    this.ShowGrid(GridManager.SelectedGrid, null, 0, 0, this.Labels.ToArray<EtichettaInfo>());
                }
                 * */
                SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('GridPersonalizationPreferred','');", true);
            }

            if (!string.IsNullOrEmpty(this.OpenTitolario.ReturnValue))
            {

                if (this.ReturnValue.Split('#').Length > 1)
                {
                    this.txt_CodFascicolo.Text = this.ReturnValue.Split('#').First();
                    this.txt_DescFascicolo.Text = this.ReturnValue.Split('#').Last();
                    this.UpCodFasc.Update();
                    txt_CodFascicolo_OnTextChanged(new object(), new EventArgs());
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenTitolario','');", true);
            }

            if (!string.IsNullOrEmpty(this.OpenTitolarioMassive.ReturnValue))
            {
                if (this.ReturnValue.Split('#').Length > 1)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "txt_CodFascicolo", "$('#MassiveCollate_panel>#ifrm_MassiveCollate').contents().find('.txt_addressBookLeft').val('" + utils.FormatJs(this.ReturnValue.Split('#').First()) + "');", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "txt_DescFascicolo", "$('#MassiveCollate_panel>#ifrm_MassiveCollate').contents().find('#txt_DescFascicolo').val('" + utils.FormatJs(this.ReturnValue.Split('#').Last()) + "');", true);
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenTitolario','');", true);
            }

            if (!string.IsNullOrEmpty(this.SearchProjectMassive.ReturnValue))
            {
                if (this.ReturnValue.Split('#').Length > 1)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "txt_CodFascicolo", "$('#MassiveCollate_panel>#ifrm_MassiveCollate').contents().find('.txt_addressBookLeft').val('" + utils.FormatJs(this.ReturnValue.Split('#').First()) + "');", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "txt_DescFascicolo", "$('#MassiveCollate_panel>#ifrm_MassiveCollate').contents().find('#txt_DescFascicolo').val('" + utils.FormatJs(this.ReturnValue.Split('#').Last()) + "');", true);
                }
                else
                    if (this.ReturnValue.Contains("//"))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "txt_CodFascicolo", "$('#MassiveCollate_panel>#ifrm_MassiveCollate').contents().find('.txt_addressBookLeft').val('" + utils.FormatJs(this.ReturnValue) + "');", true);
                        Registro registro = RegistryManager.GetRegistryInSession();
                        string desc_fascicolo = string.Empty;
                        string cod_fascicolo = utils.FormatJs(this.ReturnValue);
                        string codClassifica = string.Empty;
                        Fascicolo project = null;

                        if (string.IsNullOrEmpty(cod_fascicolo))
                        {
                            this.txt_DescFascicolo.Text = string.Empty;
                            return;
                        }
                        if (cod_fascicolo.IndexOf("//") > -1)
                        {
                            #region FASCICOLAZIONE IN SOTTOFASCICOLI
                            string codice = string.Empty;
                            string descrizione = string.Empty;
                            DocsPaWR.Fascicolo SottoFascicolo = getFolderMassiveCollate(registro, ref codice, ref descrizione, cod_fascicolo);
                            if (SottoFascicolo != null)
                            {

                                if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
                                {
                                    desc_fascicolo = descrizione;
                                    project = SottoFascicolo;
                                    DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, SottoFascicolo.idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                                    ProjectManager.setProjectInSessionForRicFasc(gerClassifica[gerClassifica.Length - 1].codice);
                                    ProjectManager.setFascicoloSelezionatoFascRapida(this, SottoFascicolo);
                                }
                                else
                                {

                                    //string msg = @"Attenzione, sottofascicolo non presente.";
                                    string msg = "WarningDocumentSubFileNoFound";

                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                                    desc_fascicolo = string.Empty;
                                    project = null;
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
                                desc_fascicolo = string.Empty;
                                project = null;
                                ProjectManager.setProjectInSessionForRicFasc(null);
                                ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                            }

                            #endregion
                        }

                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "txt_DescFascicolo", "$('#MassiveCollate_panel>#ifrm_MassiveCollate').contents().find('#txt_DescFascicolo').val('" + desc_fascicolo + "');", true);
                    }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SearchProjectMassive','');", true);
            }

            //Laura 13 Marzo
            if (!string.IsNullOrEmpty(this.SearchProject.ReturnValue))
            {

                if (this.ReturnValue.Split('#').Length > 1)
                {
                    this.txt_CodFascicolo.Text = this.ReturnValue.Split('#').First();
                    this.txt_DescFascicolo.Text = this.ReturnValue.Split('#').Last();
                    this.UpCodFasc.Update();
                    txt_CodFascicolo_OnTextChanged(new object(), new EventArgs());
                }
                else
                    //Laura 19 Marzo
                    if (this.ReturnValue.Contains("//"))
                    {
                        this.txt_CodFascicolo.Text = this.ReturnValue.Split('#').First();
                        this.txt_DescFascicolo.Text = this.ReturnValue.Split('#').Last();
                        this.UpCodFasc.Update();
                        txt_CodFascicolo_OnTextChanged(new object(), new EventArgs());
                    }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SearchProject','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveAddAdlUser.ReturnValue))
            {
                if (this.MassiveAddAdlUser.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveAddAdlUser','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveRemoveAdlUser.ReturnValue))
            {
                if (this.MassiveRemoveAdlUser.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveRemoveAdlUser','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveAddAdlRole.ReturnValue))
            {
                if (this.MassiveAddAdlRole.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveAddAdlRole','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveRemoveAdlRole.ReturnValue))
            {
                if (this.MassiveRemoveAdlRole.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveRemoveAdlRole','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveConservation.ReturnValue))
            {
                if (this.MassiveConservation.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveConservation','');", true);
            }

            if (!string.IsNullOrEmpty(this.ExportDati.ReturnValue))
            {
                if (this.ExportDati.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ExportDati','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveTransmission.ReturnValue))
            {
                if (this.MassiveTransmission.ReturnValue == "true")
                {
                    this.ListCheck = new Dictionary<string, string>();
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveTransmission','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveTimestamp.ReturnValue))
            {
                if (this.MassiveTimestamp.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveTimestamp','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveConversion.ReturnValue))
            {
                if (this.MassiveConversion.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveConversion','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveConsolidation.ReturnValue))
            {
                if (this.MassiveConsolidation.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveConsolidation','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveConsolidationMetadati.ReturnValue))
            {
                if (this.MassiveConsolidationMetadati.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveConsolidationMetadati','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveForward.ReturnValue))
            {
                if (this.MassiveForward.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                if (DocumentManager.getSelectedRecord() != null)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "redirDocument", "disallowOp(''); $(location).attr('href','../Document/Document.aspx');", true);
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveForward','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveCollate.ReturnValue))
            {
                if (this.MassiveCollate.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveCollate','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveDigitalSignature.ReturnValue))
            {
                if (this.MassiveDigitalSignature.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                    MassiveOperationUtils.ItemsStatus = null;
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveDigitalSignature','');", true);
            }

            if (!string.IsNullOrEmpty(this.StartProcessSignature.ReturnValue))
            {
                this.Result = null;
                this.SelectedRow = string.Empty;
                this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                this.BuildGridNavigator();
                this.UpnlNumerodocumenti.Update();
                this.UpnlGrid.Update();
                this.upPnlGridIndexes.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('StartProcessSignature','');", true);
            }

            if (!string.IsNullOrEmpty(this.ChooseCorrespondent.ReturnValue))
            {
                switch (this.ChooseCorrespondent.ReturnValue)
                {
                    case "txtCodiceCreatore":
                        this.txtCodiceCreatore.Text = this.ChooseMultipleCorrespondent.codiceRubrica;
                        this.txtDescrizioneCreatore.Text = this.ChooseMultipleCorrespondent.descrizione;
                        this.idCreatore.Value = this.ChooseMultipleCorrespondent.systemId;
                        this.rblOwnerType.SelectedIndex = -1;
                        this.rblOwnerType.Items.FindByValue(this.ChooseMultipleCorrespondent.tipoCorrispondente).Selected = true;
                        this.upPnlCreatore.Update();
                        break;
                    case "txtCodiceProprietario":
                        this.txtCodiceProprietario.Text = this.ChooseMultipleCorrespondent.codiceRubrica;
                        this.txtDescrizioneProprietario.Text = this.ChooseMultipleCorrespondent.descrizione;
                        this.idProprietario.Value = this.ChooseMultipleCorrespondent.systemId;
                        this.rblProprietarioType.SelectedIndex = -1;
                        this.rblProprietarioType.Items.FindByValue(this.ChooseMultipleCorrespondent.tipoCorrispondente).Selected = true;
                        this.upPnlProprietario.Update();
                        break;
                    case "txt_codMittInter_C":
                        this.txt_codMittInter_C.Text = this.ChooseMultipleCorrespondent.codiceRubrica;
                        this.txt_descrMittInter_C.Text = this.ChooseMultipleCorrespondent.descrizione;
                        this.idMittItermedio.Value = this.ChooseMultipleCorrespondent.systemId;
                        this.UpMittInter.Update();
                        break;
                    case "txt_codMit_E":
                        this.txt_codMit_E.Text = this.ChooseMultipleCorrespondent.codiceRubrica;
                        this.txt_descrMit_E.Text = this.ChooseMultipleCorrespondent.descrizione;
                        this.IdRecipient.Value = this.ChooseMultipleCorrespondent.systemId;
                        this.UpProtocollo.Update();
                        break;
                    case "txt_codUsrConsolidamento":
                        this.txt_codUsrConsolidamento.Text = this.ChooseMultipleCorrespondent.codiceRubrica;
                        this.txt_descrUsrConsolidamento.Text = this.ChooseMultipleCorrespondent.descrizione;
                        this.idUsrConsolidamento.Value = this.ChooseMultipleCorrespondent.systemId;
                        this.UsrConsolidamentoTypeOfCorrespondent.Value = this.ChooseMultipleCorrespondent.tipoCorrispondente;
                        this.UpStatoConsolidamento.Update();
                        break;
                }


                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ChooseCorrespondent','');", true);
            }

        }

        protected void SetAjaxDescriptionProject()
        {
            string dataUser = RoleManager.GetRoleInSession().idGruppo;
            dataUser = dataUser + "-" + this.lb_reg_C.SelectedValue;
            if (UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione) != null)
            {
                RapidSenderDescriptionProject.ContextKey = dataUser + "-" + UserManager.GetUserInSession().idAmministrazione + "-" + UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione).ID + "-" + UserManager.GetUserInSession().idPeople + "-" + UIManager.UserManager.GetUserInSession().systemId;
            }
        }

        protected void SetAjaxAddressBook()
        {
            string dataUser = RoleManager.GetRoleInSession().systemId;
            dataUser = dataUser + "-" + this.lb_reg_C.SelectedValue;

            string callType = "CALLTYPE_OWNER_AUTHOR";
            this.RapidCreatore.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
            this.RapidProprietario.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
            this.RapidFirmatario.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;

            callType = "CALLTYPE_CORR_INT_EST_CON_DISABILITATI";
            this.RapidRecipient.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
            this.RapidUsrConsolidamento.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;

            callType = "CALLTYPE_CORR_INT_NO_UO";
            this.RapidMittInter.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
        }

        protected void SearchProjectRegistro()
        {
            Registro registro = RegistryManager.getRegistroBySistemId(this.lb_reg_C.SelectedValue);
            this.txt_DescFascicolo.Text = string.Empty;
            string codClassifica = string.Empty;

            if (string.IsNullOrEmpty(this.txt_CodFascicolo.Text))
            {
                this.txt_DescFascicolo.Text = string.Empty;
                return;
            }

            //FASCICOLAZIONE IN SOTTOFASCICOLI

            //if (this.txt_CodFascicolo.Text.IndexOf("//") > -1)
            //{
            //    #region FASCICOLAZIONE IN SOTTOFASCICOLI
            //    string codice = string.Empty;
            //    string descrizione = string.Empty;
            //    DocsPaWR.Fascicolo SottoFascicolo = this.getFolder(registro, ref codice, ref descrizione);
            //    if (SottoFascicolo != null)
            //    {

            //        if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
            //        {
            //            this.txt_DescFascicolo.Text = descrizione;
            //            this.txt_CodFascicolo.Text = codice;
            //            this.IdProject.Value = SottoFascicolo.systemID;
            //        }
            //        else
            //        {

            //            //string msg = @"Attenzione, sottofascicolo non presente.";
            //            string msg = "WarningDocumentSubFileNoFound";

            //            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
            //            this.txt_DescFascicolo.Text = string.Empty;
            //            this.txt_CodFascicolo.Text = string.Empty;
            //            this.IdProject.Value = string.Empty;
            //        }
            //    }
            //    else
            //    {
            //        Session["validCodeFasc"] = "false";

            //        //string msg = @"Attenzione, sottofascicolo non presente.";
            //        string msg = "WarningDocumentSubFileNoFound";

            //        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
            //        this.txt_DescFascicolo.Text = string.Empty;
            //        this.txt_CodFascicolo.Text = string.Empty;
            //        this.IdProject.Value = string.Empty;
            //    }

            //    #endregion
            //}
            //else
            //{
            DocsPaWR.Fascicolo[] listaFasc = getFascicoli(registro);

            if (listaFasc != null)
            {
                if (listaFasc.Length > 0)
                {
                    //caso 1: al codice digitato corrisponde un solo fascicolo
                    if (listaFasc.Length == 1)
                    {
                        this.IdProject.Value = listaFasc[0].systemID;
                        this.txt_DescFascicolo.Text = listaFasc[0].descrizione;
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
                    }
                    else
                    {
                        //caso 2: al codice digitato corrispondono piu fascicoli
                        codClassifica = this.txt_CodFascicolo.Text;
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
                        Fascicolo chiusoFasc = ProjectManager.getFascicoloDaCodice(this.Page, this.txt_CodFascicolo.Text);
                        if (chiusoFasc != null && !string.IsNullOrEmpty(chiusoFasc.stato) && chiusoFasc.stato.Equals("C"))
                        {
                            // 06/06/2018: INC000001089858  se il fascicolo è chiuso devo comunque consentire la ricerca
                            /*
                            string msg = "WarningDocumentFileNoOpen";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            */

                            this.IdProject.Value = chiusoFasc.systemID;
                            this.txt_DescFascicolo.Text = chiusoFasc.descrizione;
                            if (chiusoFasc.tipo.Equals("G"))
                            {
                                codClassifica = chiusoFasc.codice;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, chiusoFasc.idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }
                        }
                        else
                        {
                            //string msg = @"Attenzione, codice fascicolo non presente.";
                            string msg = "WarningDocumentCodFileNoFound";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                            this.txt_DescFascicolo.Text = string.Empty;
                            this.txt_CodFascicolo.Text = string.Empty;
                            this.IdProject.Value = string.Empty;
                        }
                    }
                }
            }
            //}
        }

        private DocsPaWR.Fascicolo getFolder(DocsPaWR.Registro registro, ref string codice, ref string descrizione)
        {
            DocsPaWR.Folder[] listaFolder = null;
            DocsPaWR.Fascicolo fasc = null;
            string separatore = "//";
            int posSep = this.txt_CodFascicolo.Text.IndexOf("//");
            if (this.txt_CodFascicolo.Text != string.Empty && posSep > -1)
            {

                string codiceFascicolo = this.txt_CodFascicolo.Text.Substring(0, posSep);
                string descrFolder = this.txt_CodFascicolo.Text.Substring(posSep + separatore.Length);

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

        private DocsPaWR.Fascicolo getFolderMassiveCollate(DocsPaWR.Registro registro, ref string codice, ref string descrizione, string codiceFasc)
        {
            DocsPaWR.Folder[] listaFolder = null;
            DocsPaWR.Fascicolo fasc = null;
            string separatore = "//";
            int posSep = codiceFasc.IndexOf("//");
            if (codiceFasc != string.Empty && posSep > -1)
            {

                string codiceFascicolo = codiceFasc.Substring(0, posSep);
                string descrFolder = codiceFasc.Substring(posSep + separatore.Length);

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

        private DocsPaWR.Fascicolo[] getFascicoli(DocsPaWR.Registro registro)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            if (!this.txt_CodFascicolo.Text.Equals(""))
            {
                string codiceFascicolo = this.txt_CodFascicolo.Text;
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

        private void InitializeComponent()
        {
            this.lb_reg_C.SelectedIndexChanged += new System.EventHandler(this.lb_reg_C_SelectedIndexChanged);
            this.rbl_Reg_C.SelectedIndexChanged += new System.EventHandler(this.rbl_Reg_C_SelectedIndexChanged);
            this.rb_docSpediti.SelectedIndexChanged += new EventHandler(rb_docSpediti_SelectedIndexChanged);
            this.rb_docSpeditiEsito.SelectedIndexChanged += new EventHandler(rb_docSpeditiEsito_SelectedIndexChanged);
        }


        private void InitializePage()
        {
            // Gabriele Melini 13-01-2015
            // INC000000519224
            // Imposto il tasto "Cerca" come tasto di default della pagina
            this.Form.DefaultButton = this.SearchDocumentAdvancedSearch.UniqueID;

            this.txt_initNumRep.ReadOnly = false;
            this.txt_fineNumRep.Visible = false;
            this.LtlDaNumRep.Visible = false;
            this.LtlANumRep.Visible = false;
            this.txt_initDataRep.ReadOnly = false;
            this.txt_fineDataRep.Visible = false;
            this.LtlADataRep.Visible = false;
            this.txt_initIdDoc_C.ReadOnly = false;
            this.txt_fineIdDoc_C.Visible = false;
            this.LtlAIdDoc.Visible = false;
            this.LtlDaIdDoc.Visible = false;
            this.txt_initDataCreazione_E.ReadOnly = false;
            this.txt_finedataCreazione_E.Visible = false;
            this.LtlADataCreazione.Visible = false;
            this.txt_initNumProt_E.ReadOnly = false;
            this.txt_fineNumProt_E.Visible = false;
            this.LtlANumProto.Visible = false;
            this.LtlDaNumProto.Visible = false;
            this.txt_fineNumProt_E.Text = string.Empty;
            this.txt_initDataProt_E.ReadOnly = false;
            this.txt_fineDataProt_E.Visible = false;
            this.LtlADataProto.Visible = false;
            this.txt_initDataScadenza_C.ReadOnly = false;
            this.txt_fineDataScadenza_C.Visible = false;
            this.LtlADataScad.Visible = false;
            this.txt_fineDataScadenza_C.Text = string.Empty;
            this.txt_initDataStampa_E.ReadOnly = false;
            this.txt_finedataStampa_E.Visible = false;
            this.LtlADataStampa.Visible = false;
            this.txt_finedataStampa_E.Text = string.Empty;
            this.txt_initDataProtMitt_C.ReadOnly = false;
            this.txt_fineDataProtMitt_C.Visible = false;
            this.LtlADataProtMitt.Visible = false;
            this.txt_fineDataProtMitt_C.Text = string.Empty;
            this.txt_initDataArrivo_C.ReadOnly = false;
            this.txt_fineDataArrivo_C.Visible = false;
            this.LtlADataArrivo.Visible = false;
            this.txt_fineDataArrivo_C.Text = string.Empty;
            this.txt_dataProtoEmeInizio.ReadOnly = false;
            this.txt_dataProtoEmeFine.Visible = false;
            this.LtlADataSegDiEmerg.Visible = false;
            this.txt_dataProtoEmeFine.Text = string.Empty;
            this.Cal_Da_pitre.ReadOnly = false;
            this.Cal_A_pitre.Visible = false;
            this.LtlADataRicevuta.Visible = false;
            this.Cal_A_pitre.Text = string.Empty;
            this.Cal_Da_pec.ReadOnly = false;
            this.Cal_A_pec.Visible = false;
            this.LtlADataRicevutaPEC.Visible = false;
            this.Cal_A_pec.Text = string.Empty;
            this.txtDataConsolidamento.ReadOnly = false;
            this.txtDataConsolidamentoFinale.Visible = false;
            this.LtlADataConsolidamento.Visible = false;
            this.txtDataConsolidamentoFinale.Text = string.Empty;
            this.txt_fineDataProt_E.Text = string.Empty;
            this.LtlADataVers.Visible = false;
            this.txt_fineDataVers.Visible = false;
            this.LtlADatePolicy.Visible = false;
            this.txt_fineDatePolicy.Visible = false;


            this.ShowGridPersonalization = UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION");
            this.ClearSessionProperties();
            this.InitializeLanguage();
            this.LoadKeys();
            this.PopulateDDLSavedSearches();
            this.LoadMassiveOperation();

            this.LoadTypeDocuments();
            this.VisibiltyRoleFunctions();
            this.ListaRegistri();
            this.CaricaComboMezzoSpedizione(this.ddl_spedizione);
            this.ListaRagioni();
            this.ComboTipoFileAcquisiti();
            this.InizializaDocSpediti();

            if (this.ShippingMethodRequired)
            {
                this.plcMezzoSped.Visible = true;
                this.UpMezzoSped.Update();
            }

            this.SetAjaxDescriptionProject();
            this.SetAjaxAddressBook();

            if (this.EnabledSearchAttachByDescMainDoc)
            {
                this.cbl_archDoc_E.Attributes.Add("onclick", "enableFieldPnlObjectAttach();");
            }
            else
            { 
                this.cbl_archDoc_E.Attributes.Add("onclick", "enableField();");          
            }

            this.ListCheck = new Dictionary<string, string>();
			this.ListToSign = new Dictionary<string, FileToSign>();

            if (this.IsAdl)
            {
                this.SaveSearch.Url = ResolveUrl("~/Popup/SaveSearch.aspx?IsAdl=true");
                this.ModifySearch.Url = ResolveUrl("~/Popup/SaveSearch.aspx?modify=true&IsAdl=true");
            }
            if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FILTRO_ALLEGATI_ESTERNI.ToString())) ||
                !Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FILTRO_ALLEGATI_ESTERNI.ToString()).Equals("1"))
            {
                ListItem itemExternal = (from item in rblFiltriAllegati.Items.Cast<ListItem>() where item.Value.Equals(TYPE_EXT) select item).FirstOrDefault();
                rblFiltriAllegati.Items.Remove(itemExternal);
            }
            if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.INTEROP_SERVICE_ACTIVE.ToString())) ||
                !Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.INTEROP_SERVICE_ACTIVE.ToString()).Equals("1"))
            {
                ListItem itemPitre = (from item in rblFiltriAllegati.Items.Cast<ListItem>() where item.Value.Equals(TYPE_PITRE) select item).FirstOrDefault();
                rblFiltriAllegati.Items.Remove(itemPitre);

                ListItem itemPitreNumAllegati = (from item in rblFiltriNumAllegati.Items.Cast<ListItem>() where item.Value.Equals(TYPE_PITRE) select item).FirstOrDefault();
                rblFiltriNumAllegati.Items.Remove(itemPitreNumAllegati);
            }

            Session["templateRicerca"] = null;
        }

        private void InizializaDocSpediti()
        {
            if (string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_NOTIFICHE_PEC)) ||
               !bool.Parse(ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_NOTIFICHE_PEC)))
            {
                this.p_ricevute_pec.Visible = false;
                this.cbx_pec.Visible = false;
            }
            else
            {
                this.p_ricevute_pec.Visible = true;
                this.cbx_pec.Visible = true;
            }
        }

        private void ComboTipoFileAcquisiti()
        {
            ArrayList tipoFile = new ArrayList();
            tipoFile = DocumentManager.getExtFileAcquisiti(this);
            //bool firmati = false;
            for (int i = 0; i < tipoFile.Count; i++)
            {
                if (!tipoFile[i].ToString().Contains("P7M"))
                {
                    ListItem item = new ListItem(tipoFile[i].ToString());
                    this.ddl_tipoFileAcquisiti.Items.Add(item);
                }
            }
        }

        private void ListaRagioni()
        {
            listaRagioni = TrasmManager.getListaRagioni(this, string.Empty, true);

            m_hashTableRagioneTrasmissione = new Hashtable();
            if (listaRagioni != null && listaRagioni.Length > 0)
            {
                string ragione = Utils.Languages.GetLabelFromCode("ddl_ragioneTrasmAny", UserManager.GetUserLanguage());
                ddl_ragioneTrasm.Items.Add(ragione);
                for (int i = 0; i < listaRagioni.Length; i++)
                {
                    m_hashTableRagioneTrasmissione.Add(i, listaRagioni[i]);

                    ListItem newItem = new ListItem(listaRagioni[i].descrizione, listaRagioni[i].systemId);
                    ddl_ragioneTrasm.Items.Add(newItem);
                }
                TrasmManager.setHashRagioneTrasmissione(this, m_hashTableRagioneTrasmissione);

                this.ddl_ragioneTrasm.SelectedIndex = 0;
            }
        }

        private void CaricaComboMezzoSpedizione(DropDownList ddl)
        {
            ddl.Items.Clear();
            ddl.Items.Add("");
            ArrayList listaMezzoSpedizione = new ArrayList();
            DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
            string idAmm = UserManager.GetUserInSession().idAmministrazione;
            DocsPaWR.MezzoSpedizione[] m_sped = ws.AmmListaMezzoSpedizione(idAmm, true);
            foreach (DocsPaWR.MezzoSpedizione m_spediz in m_sped)
            {
                ListItem li = new ListItem();
                li.Value = m_spediz.IDSystem;
                li.Text = m_spediz.Descrizione;
                ddl.Items.Add(li);
            }
        }

        private void ListaRegistri()
        {
            bool filtroAoo = false;
            DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistriNoFiltroAOO(out filtroAoo);

            //DocsPaWR.Registro[] registri = UserManager.getRuolo(this).registri;
            //string[] listaReg = new string[registri.Length];
            if (userRegistri != null && filtroAoo)
            {
                ListItem itemM = new ListItem(this.GetLabel("LbRegistroMine"), "M");
                rbl_Reg_C.Items.Add(itemM);
                itemM = new ListItem(this.GetLabel("LbRegistroAll"), "T");
                rbl_Reg_C.Items.Add(itemM);
                itemM = new ListItem(this.GetLabel("LbRegistroReset"), "R");
                rbl_Reg_C.Items.Add(itemM);
                lb_reg_C.Rows = 5;
            }
            else
            {
                userRegistri = RoleManager.GetRoleInSession().registri;
                ListItem itemM = new ListItem(this.GetLabel("LbRegistroAll"), "T");
                rbl_Reg_C.Items.Add(itemM);
                itemM = new ListItem(this.GetLabel("LbRegistroReset"), "R");
                rbl_Reg_C.Items.Add(itemM);
                //rbl_Reg_E.SelectedIndex = 1;
            }
            rbl_Reg_C.SelectedIndex = 0;
            string[] id = new string[userRegistri.Length];
            for (int i = 0; i < userRegistri.Length; i++)
            {
                lb_reg_C.Items.Add(userRegistri[i].codRegistro);
                lb_reg_C.Items[i].Value = userRegistri[i].systemId;
                string nomeRegCurrente = "UserReg" + i;
                // SELEZIONA TUTTI I REGISTRI PRESENTI per DEFAULT
                if (!filtroAoo)
                {
                    if (!userRegistri[i].flag_pregresso)
                        lb_reg_C.Items[i].Selected = true;
                    else
                        // tolgo la selezione "tutti"
                        rbl_Reg_C.SelectedIndex = -1;
                }
                else
                    if (rbl_Reg_C.SelectedItem.Value == "M")
                        for (int j = 0; j < RoleManager.GetRoleInSession().registri.Length; j++)
                        {
                            if (RoleManager.GetRoleInSession().registri[j].codRegistro == lb_reg_C.Items[i].Text)
                            {
                                if (!userRegistri[i].flag_pregresso)
                                {
                                    lb_reg_C.Items[i].Selected = true;
                                    break;
                                }
                            }
                        }

                id[i] = (string)userRegistri[i].systemId;
            }

            if (this.lb_reg_C.Items.Count == 1)
            {
                this.plcRegistro.Visible = false;
                this.UpRegistro.Update();
            }

            //UserManager.setListaIdRegistri(this, listaReg);
            //rbl_Reg_C.Items[0].Selected = true;
        }

        private void LoadTypeDocuments()
        {
            DocsPaWR.TipologiaAtto[] listaTipologiaAtto;
            if (this.CustomDocuments)
            {
                listaTipologiaAtto = DocumentManager.getTipoAttoPDInsRic(this, UserManager.GetInfoUser().idAmministrazione, RoleManager.GetRoleInSession().idGruppo, "1");
            }
            else
                listaTipologiaAtto = DocumentManager.getListaTipologiaAtto(this);

            this.DocumentDdlTypeDocument.Items.Clear();

            ListItem item = new ListItem(string.Empty, string.Empty);
            this.DocumentDdlTypeDocument.Items.Add(item);

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_RICERCA_NESSUNA_TIPOLOGIA"))
            {
                item = new ListItem(Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedNoTipology", UserManager.GetUserLanguage()), "0");
                this.DocumentDdlTypeDocument.Items.Add(item);
            }
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
        }

        protected void InitializePageSize()
        {
            string keyValue = Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_PAGING_ROW_DOC.ToString());
            int tempValue = 0;
            if (!string.IsNullOrEmpty(keyValue))
            {
                tempValue = Convert.ToInt32(keyValue);
                if (tempValue >= 20 || tempValue <= 50)
                {
                    this.PageSize = tempValue;
                }
            }
        }

        private void LoadMassiveOperation()
        {
            this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem("", ""));
            string title = string.Empty;
            string language = UIManager.UserManager.GetUserLanguage();

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_DOC_FIRMA") && UIManager.UserManager.IsAuthorizedFunctions("DO_DOC_FIRMA"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveDigitalSignatureTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_SIGN"));
            }
            if (UserManager.IsAuthorizedFunctions("FIRMA_HSM") && UIManager.UserManager.IsAuthorizedFunctions("FIRMA_HSM_MASSIVA"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveSignatureHSMTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_SIGN_HSM"));
            }
            if (UIManager.UserManager.IsAuthorizedFunctions("FASC_INS_DOC") && UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_CLASSIFICATION"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveCollateTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_CLASSIFICATION"));

            }
            if (UIManager.UserManager.IsAuthorizedFunctions("DO_TRA_TRASMETTI") && UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_TRANSMISSION"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveTransmissionTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_TRANSMISSION"));

            }
            if (UIManager.UserManager.IsAuthorizedFunctions("DO_TIMESTAMP") && UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_TIMESTAMP"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveTimestampTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_TIMESTAMP"));

            }

            if (UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_CONVERSION"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConversionTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_CONVERSION"));

            }
            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADD_ADL"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveAddAdlUserTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_ADL"));

                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveAdlUserTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "REMOVE_MASSIVE_ADL"));
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveAddAdlRoleTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_ADLR_DOC"));

                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveAdlRoleTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "REMOVE_MASSIVE_ADLR_DOC"));
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_INOLTRA"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveForwardTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_INOLTRA"));

            }


            title = Utils.Languages.GetLabelFromCode("SearchDocumentExportDatiTitle", language);
            this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVEXPORTDOC"));

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_CONS") && !this.IsConservazioneSACER)
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConservationTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "DO_MASSIVE_CONS"));
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_SACER_VERSAMENTO") && this.IsConservazioneSACER)
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConservationPARERTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "DO_VERSAMENTO_PARER"));
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_REMOVE_VERSIONS"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveRemoveVersionsTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_REMOVE_VERSIONS"));
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_CONSOLIDAMENTO"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConsolidationTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "DO_CONSOLIDAMENTO"));
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_CONSOLIDAMENTO_METADATI"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveConsolidationMetadatiTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "DO_CONSOLIDAMENTO_METADATI"));
            }

            if (this.EnabledLibroFirma && UIManager.UserManager.IsAuthorizedFunctions("DO_START_SIGNATURE_PROCESS"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveStartSignatureProcess", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "DO_START_SIGNATURE_PROCESS"));
            }
            // riordina alfabeticamente
            List<ListItem> listCopy = new List<ListItem>();
            foreach (ListItem item in this.SearchDocumentDdlMassiveOperation.Items)
                listCopy.Add(item);
            this.SearchDocumentDdlMassiveOperation.Items.Clear();
            foreach (ListItem item in listCopy.OrderBy(item => item.Text))
                this.SearchDocumentDdlMassiveOperation.Items.Add(item);
        }

        private void LoadKeys()
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.VISUALIZZA_OGGETTO_COMM_REF.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.VISUALIZZA_OGGETTO_COMM_REF.ToString()]))
            {
                this.plcNumOggetto.Visible = true;
                this.UpPnlNumOggetto.Update();
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.MEZZO_SPEDIZIONE.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.MEZZO_SPEDIZIONE.ToString()].Equals("1"))
            {
                this.ShippingMethodRequired = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.VISUALIZZA_NOTIFICHE_PEC.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.VISUALIZZA_NOTIFICHE_PEC.ToString()]))
            {
                this.DisplayPecNotifications = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE.ToString()]))
            {
                this.ActiveCodeDescriptionAdminSender = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ENABLE_TIMESTAMP_DOC.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ENABLE_TIMESTAMP_DOC.ToString()]))
            {
                this.EnableTimestampDoc = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_OGGETTO.ToString())))
            {
                this.MaxLenghtObject = int.Parse(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_OGGETTO.ToString()));
            }

            this.InitializePageSize();

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()].Equals("1"))
            {
                this.CustomDocuments = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()].Equals("1"))
            {
                this.EnableStateDiagram = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]))
            {
                this.SearchDocumentDdlMassiveOperation.Visible = false;
                this.SearchDocumentLitRapidSearch.Visible = false;
                this.DdlRapidSearch.Visible = false;
                this.cbl_archDoc_E.Visible = false;
                this.SearchDocumentTypeDocument.Visible = false;
                this.SearchDocumentAdvancedSave.Visible = false;
                this.SearchDocumentAdvancedEdit.Visible = false;
                this.SearchDocumentAdvancedRemove.Visible = false;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE.ToString()]))
            {
                this.p_cod_amm.Visible = false;
                this.UpPnlCodAmm.Update();
            }

            if (utils.GetAbilitazioneAtipicita())
            {
                this.plcVisibility.Visible = true;
                this.UpPnlVisibility.Update();
            }

            if (UIManager.DocumentManager.IsEnabledProfilazioneAllegati())
            {
                this.IsEnabledProfilazioneAllegato = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_KEY_WORDS.ToString())) && Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_KEY_WORDS.ToString()).Equals("1"))
            {
                this.PlcKeyWord.Visible = true;
            }

            if (UIManager.SimplifiedInteroperabilityManager.IsEnabledSimpInterop)
            {
                this.p_ricevute_pitre.Visible = true;
                this.cbx_pitre.Visible = true;
            }
            else
            {
                this.p_ricevute_pitre.Visible = false;
                this.cbx_pitre.Visible = false;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString())) && !Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString()).Equals("1"))
            {
                this.opInt.Attributes.CssStyle.Add("display", "none");
                this.opInt.Selected = false;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_WA_CONSERVAZIONE.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_WA_CONSERVAZIONE.ToString()).Equals("1"))
            {
                this.IsConservazioneSACER = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString()).Equals("1"))
            {
                this.PnlFirmaElettronica.Visible = true;
                this.EnabledLibroFirma = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_RIC_DESC_ALLEGATI.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_RIC_DESC_ALLEGATI.ToString()).Equals("1"))
            {
                this.EnabledSearchAttachByDescMainDoc = true;
            }
            if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_RIC_REPERTORIO.ToString())) || !Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_RIC_REPERTORIO.ToString()).Equals("1"))
            {
                ListItem rbRep = this.cbl_archDoc_E.Items.FindByValue("REP");
                cbl_archDoc_E.Items.Remove(rbRep);
            }
        }

        private void VisibiltyRoleFunctions()
        {
            if (UIManager.UserManager.IsAuthorizedFunctions("DO_CONS") && !(this.IsConservazioneSACER))
            {
                this.AllowConservazione = true;
            }

            //if (!(UIManager.UserManager.IsAuthorizedFunctions("DO_SACER_RECUPERO") || UIManager.UserManager.IsAuthorizedFunctions("DO_SACER_VERSAMENTO")) && !(this.IsConservazioneSACER))
            //{
            //    this.cbl_Conservazione.Visible = false;
            //    this.phStatoConservazione.Visible = false;
            //}

            // INTEGRAZIONE PITRE-PARER
            // Microfunzione per recupero informazioni stato conservazione
            if (this.IsConservazioneSACER)
            {
                this.phConservatoNon.Visible = false;
                if (!(UIManager.UserManager.IsAuthorizedFunctions("DO_SACER_RECUPERO") || UIManager.UserManager.IsAuthorizedFunctions("DO_SACER_VERSAMENTO")))
                {
                    this.cbl_Conservazione.Visible = false;
                    this.phStatoConservazione.Visible = false;
                }
            }
            else
            {
                this.cbl_Conservazione.Visible = false;
                this.phStatoConservazione.Visible = false;
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADD_ADL"))
            {
                this.AllowADL = true;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_CLA_TITOLARIO"))
            {
                this.btnclassificationschema.Visible = false;
            }

            if (!this.ShowGridPersonalization)
            {
                this.projectImgSaveGrid.Visible = false;
                this.projectImgEditGrid.Visible = false;
                this.projectImgPreferredGrids.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_PROT_OG_OGGETTARIO"))
            {
                this.DocumentImgObjectary.Visible = false;
            }


            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
            {
                this.AllowADLRole = true;
                if (this.IsAdl)
                {
                    this.PlcAdl.Visible = true;
                }
            }

            //Microfunzione che disabilita il protocollo e altri filtri nella ricerca 
            //documenti semplice e avanzata
            if (UIManager.UserManager.IsAuthorizedFunctions("DIS_PROTO_SEARCH_DOC"))
            {
                this.cbl_archDoc_E.Visible = false;
                this.SearchDocumentTypeDocument.Visible = false;
                this.phProtocol.Visible = false;
                this.plcRegistro.Visible = false;
                this.phProtoMitt.Visible = false;
                this.phDataScadProtMitt.Visible = false;
                this.phDataStampa.Visible = false;
                this.phConservatoNon.Visible = false;
                this.phStatoDoc.Visible = false;
                this.phSegnatura.Visible = false;
                this.phMittInter.Visible = false;
                this.phDataArrivo.Visible = false;
                this.rblFiltriNumAllegati.Visible = false;
                this.plcVisibility.Visible = false;
                this.p_cod_amm.Visible = false;
                this.p_ricevute_pec.Visible = false;
                this.p_ricevute_pitre.Visible = false;
                this.phDocSpediti.Visible = false;
                this.phEvidenza.Visible = false;
                this.phSegnEmer.Visible = false;
                this.phTimestamp.Visible = false;
                this.UpMezzoSped.Visible = false;

            }

            if (UserManager.IsAuthorizedFunctions("DO_STATE_SIGNATURE_PROCESS"))
            {
                this.EnableViewInfoProcessesStarted = true;
            }
            else
            {
                this.EnableViewInfoProcessesStarted = false;
            }

            if (UserManager.IsAuthorizedFunctions("DO_DOC_MAI_SPEDITI"))
            {
                this.PnlNeverSend.Visible = true;
            }

            if (!UserManager.IsAuthorizedFunctions("DO_DOC_MAI_TRASMESSI"))
            {
                this.PnlNeverTrasm.Visible = false;
            }

            if(UserManager.IsAuthorizedFunctions("DO_RIC_ESTENDI_NODI_FIGLI_E_FASC"))
            {
                this.PnlEstendiAFascicoli.Visible = true;
            }
        }

        private void ClearSessionProperties()
        {
            Session.Remove("itemUsedSearch");
            Session.Remove("idRicercaSalvata");

            this.Result = null;
            this.SelectedRow = string.Empty;
            this.SearchFilters = null;
            this.Template = null;
            this.RecordCount = 0;
            this.PageCount = 0;
            this.SelectedPage = 1;
            this.TypeDocument = "Search";

            this.Labels = DocumentManager.GetLettereProtocolli();
            this.CellPosition = new Dictionary<string, int>();
            this.IdProfileList = null;
            this.CodeProfileList = null;
            this.CheckAll = false;
            this.ListCheck = null;
			this.ListToSign = null;
            UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);

            // Caricamento della griglia se non ce n'è una già selezionata
            if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Document)
            {
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);
            }

            schedaRicerca = new SearchManager(KEY_SCHEDA_RICERCA, UserManager.GetUserInSession(), RoleManager.GetRoleInSession(), this);
            Session[SearchManager.SESSION_KEY] = schedaRicerca;

            this.SearchDocumentDdlMassiveOperation.Enabled = false;
            this.ShowGrid(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());

            if (!this.IsAdl)
            {
                this.TxtYear.Text = DateTime.Now.Year.ToString();
            }

            //questa property rimane in sessione quando dal tab allegati faccio back e torno nella ricerca; va rimossa
            HttpContext.Current.Session.Remove("selectedAttachmentId");
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('StartProcessSignature','');", true);
        }

        /// <summary>
        /// Questa funzione si occupa di ricercare i documenti e di visualizzare 
        /// i dati
        /// </summary>
        private void SearchDocumentsAndDisplayResult(FiltroRicerca[][] searchFilters, int selectedPage, Grid selectedGrid, EtichettaInfo[] labels)
        {
            // Numero di record restituiti dalla pagina
            int recordNumber = 0;

            // Risultati restituiti dalla ricerca
            SearchObject[] result;

            /* ABBATANGELI GIANLUIGI
             * il nuovo parametro outOfMaxRowSearchable è true se raggiunto il numero 
             * massimo di riche accettate in risposta ad una ricerca */
            bool outOfMaxRowSearchable;
            // Ricerca dei documenti
            result = this.SearchDocument(searchFilters, selectedPage, out recordNumber, out outOfMaxRowSearchable);
            string searchIntervalYears = Utils.InitConfigurationKeys.GetValue("0", DBKeys.MAX_YEARS_SEARCHABLE.ToString());
            bool maxYearsSearchable = (string.IsNullOrEmpty(this.TxtYear.Text) && string.IsNullOrEmpty(this.txt_initIdDoc_C.Text)
                                        && string.IsNullOrEmpty(this.txt_initNumProt_E.Text) && string.IsNullOrEmpty(this.txt_initDataCreazione_E.Text)
                                        && string.IsNullOrEmpty(this.txt_initDataProt_E.Text) && string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue)
                                        && !string.IsNullOrEmpty(searchIntervalYears) && !searchIntervalYears.Equals("0"));
            this.searchDocumentLblIntervalYears.Text = string.Empty;
            this.PnlLblIntervalYears.Visible = false;

            if (outOfMaxRowSearchable && maxYearsSearchable)
            {
                this.ClearGrid();
                string valoreChiaveDB = Utils.InitConfigurationKeys.GetValue("0", DBKeys.MAX_ROW_SEARCHABLE.ToString());
                if (valoreChiaveDB.Length == 0)
                {
                    valoreChiaveDB = Utils.InitConfigurationKeys.GetValue("0", DBKeys.MAX_ROW_SEARCHABLE.ToString());
                }
                string date = (Convert.ToDateTime(DocumentManager.toDay()).AddYears(-Convert.ToInt32(searchIntervalYears))).ToShortDateString();
                string msg = Utils.Languages.GetMessageFromCode("WarningSearchIntervalYearsRecordNumber", UIManager.UserManager.GetUserLanguage());
                msg = msg.Replace("@@", date).Replace("##", recordNumber.ToString()).Replace("@#", valoreChiaveDB);
                this.searchDocumentLblIntervalYears.Text = msg;
                this.PnlLblIntervalYears.Visible = true;
                return;
            }
            else if (outOfMaxRowSearchable)
            {
                this.ClearGrid();
                string valoreChiaveDB = Utils.InitConfigurationKeys.GetValue("0", DBKeys.MAX_ROW_SEARCHABLE.ToString());
                if (valoreChiaveDB.Length == 0)
                {
                    valoreChiaveDB = Utils.InitConfigurationKeys.GetValue("0", DBKeys.MAX_ROW_SEARCHABLE.ToString());
                }
                string msgDesc = Utils.Languages.GetLabelFromCode("LblSearchRecordNumber", UIManager.UserManager.GetUserLanguage());
                string msgCenter = Utils.Languages.GetLabelFromCode("LblSearchRecordNumber2", UIManager.UserManager.GetUserLanguage());
                string customError = recordNumber + " " + msgCenter + " " + valoreChiaveDB;
                string errFormt = Server.UrlEncode(customError);
                msgDesc = msgDesc.Replace("@@", customError);
                this.searchDocumentLblIntervalYears.Text = msgDesc;
                this.PnlLblIntervalYears.Visible = true;
                return;
            }
            // Se ci sono risultati, vengono visualizzati
            if (this.Result != null && this.Result.Length > 0)
            {
                this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
                this.SearchDocumentDdlMassiveOperation.Enabled = true;
            }
            else
            {
                this.ShowGrid(selectedGrid, null, 0, 0, labels);
                this.BuildGridNavigator();
                this.SearchDocumentDdlMassiveOperation.Enabled = false;
            }

        }

        private void ClearGrid()
        {
            this.Result = null;
            this.RecordCount = 0;
            this.PageCount = 0;
            this.searchDocumentLblIntervalYears.Text = string.Empty;
            this.PnlLblIntervalYears.Visible = false;
            this.ShowGrid(GridManager.SelectedGrid, null, 0, 0, this.Labels.ToArray<EtichettaInfo>());

            string gridType = this.GetLabel("projectLitGrigliaStandard");
            this.projectImgSaveGrid.Enabled = false;
            if (this.ShowGridPersonalization)
            {
                if (GridManager.SelectedGrid != null && string.IsNullOrEmpty(GridManager.SelectedGrid.GridId))
                {
                    gridType = "<span class=\"red\">" + this.GetLabel("projectLitGrigliaTemp") + "</span>";
                    if (this.gridViewResult.Rows.Count > 0) this.projectImgSaveGrid.Enabled = true;
                }
                else
                {
                    if (!(GridManager.SelectedGrid.GridId).Equals("-1"))
                    {
                        gridType = GridManager.SelectedGrid.GridName;
                        if (this.gridViewResult.Rows.Count > 0) projectImgSaveGrid.Enabled = true;
                    }
                }


                this.EnableDisableSave();
            }

            this.projectLblNumeroDocumenti.Text = this.GetLabel("projectLblNumeroDocumenti2");
            this.projectLblNumeroDocumenti.Text = this.projectLblNumeroDocumenti.Text.Replace("{0}", gridType);
            this.UpnlNumerodocumenti.Update();
            this.BuildGridNavigator();
            this.SearchDocumentDdlMassiveOperation.Enabled = false;
        }
        /// <summary>
        /// Funzione per la visualizzazione dei risutati della ricerca
        /// </summary>
        /// <param name="result">I risultati della ricerca</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        private void ShowGrid(Grid selectedGrid, SearchObject[] result, int recordNumber, int selectedPage, EtichettaInfo[] labels)
        {
            bool visibile = false;
            Templates templates = Session["templateRicerca"] as Templates;

            this.CellPosition.Clear();

            gridViewResult = this.HeaderGridView(selectedGrid,
              templates,
              this.ShowGridPersonalization, gridViewResult);

            DataTable dt = UIManager.GridManager.InitializeDataSet(selectedGrid,
                         Session["templateRicerca"] as Templates,
                         this.ShowGridPersonalization);


            if (this.Result != null && this.Result.Length > 0)
            {
                dt = this.FillDataSet(dt, this.Result, selectedGrid, labels, templates, this.ShowGridPersonalization);
                visibile = true;
            }

            // adding blank row eachone
            if (dt.Rows.Count == 1 && string.IsNullOrEmpty(dt.Rows[0]["idProfile"].ToString())) dt.Rows.RemoveAt(0);

            DataTable dt2 = dt;
            int dtRowsCount = dt.Rows.Count;
            int index = 1;
            if (dtRowsCount > 0)
            {
                for (int i = 0; i < dtRowsCount; i++)
                {
                    DataRow dr = dt2.NewRow();
                    dr.ItemArray = dt2.Rows[index - 1].ItemArray;
                    dt.Rows.InsertAt(dr, index);
                    index += 2;
                }
            }

            this.GrigliaResult = dt;
            this.gridViewResult.DataSource = dt;
            this.gridViewResult.DataBind();
            if (this.gridViewResult.Rows.Count > 0) this.gridViewResult.Rows[0].Visible = visibile;

            string gridType = this.GetLabel("projectLitGrigliaStandard");
            this.projectImgSaveGrid.Enabled = false;
            if (this.ShowGridPersonalization)
            {
                if (selectedGrid != null && string.IsNullOrEmpty(selectedGrid.GridId))
                {
                    gridType = "<span class=\"red\">" + this.GetLabel("projectLitGrigliaTemp") + "</span>";
                    if (this.gridViewResult.Rows.Count > 0) this.projectImgSaveGrid.Enabled = true;
                }
                else
                {
                    if (!(selectedGrid.GridId).Equals("-1"))
                    {
                        gridType = selectedGrid.GridName;
                        if (this.gridViewResult.Rows.Count > 0) projectImgSaveGrid.Enabled = true;
                    }
                }


                this.EnableDisableSave();
            }

            string valoreChiaveDB = Utils.InitConfigurationKeys.GetValue("0", DBKeys.MAX_ROW_SEARCHABLE.ToString());
            if (string.IsNullOrEmpty(valoreChiaveDB) || valoreChiaveDB.Equals("0") || recordNumber <= Convert.ToInt32(valoreChiaveDB))
            {
                this.projectLblNumeroDocumenti.Text = this.GetLabel("projectLblNumeroDocumenti");
                this.projectLblNumeroDocumenti.Text = this.projectLblNumeroDocumenti.Text.Replace("{0}", gridType);
                this.projectLblNumeroDocumenti.Text = this.projectLblNumeroDocumenti.Text.Replace("{1}", recordNumber.ToString());
            }
            else
            {
                this.projectLblNumeroDocumenti.Text = this.GetLabel("projectLblNumeroDocumenti2");
                this.projectLblNumeroDocumenti.Text = this.projectLblNumeroDocumenti.Text.Replace("{0}", gridType);
            }

            this.UpnlNumerodocumenti.Update();
            this.UpnlGrid.Update();
        }

        /// <summary>
        /// Funzione per l'inizializzazione del data set in base ai campi definiti nella 
        /// griglia
        /// </summary>
        /// <param name="selectedGrid">La griglia su cui basare la creazione del dataset</param>
        /// <returns></returns>
        public GridView HeaderGridView(Grid selectedGrid, Templates templateTemp, bool showGridPersonalization, GridView grid)
        {
            try
            {
                int position = 0;
                List<Field> fields = selectedGrid.Fields.Where(e => e.Visible).OrderBy(f => f.Position).ToList();
                OggettoCustom customObjectTemp = new OggettoCustom();

                if (templateTemp != null && !showGridPersonalization)
                {
                    customObjectTemp = templateTemp.ELENCO_OGGETTI.Where(
                         e => e.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && e.DA_VISUALIZZARE_RICERCA == "1").FirstOrDefault();

                    Field d = new Field();

                    if (customObjectTemp != null)
                    {
                        d.AssociatedTemplateName = templateTemp.DESCRIZIONE;
                        d.CustomObjectId = customObjectTemp.SYSTEM_ID;
                        d.FieldId = "CONTATORE";
                        d.IsNumber = true;
                        d.Label = customObjectTemp.DESCRIZIONE;
                        d.OriginalLabel = customObjectTemp.DESCRIZIONE;
                        d.OracleDbColumnName = "to_number(getcontatoredocordinamento (a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "'))";
                        d.SqlServerDbColumnName = "@dbUser@.getContatoreDocOrdinamento(a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "')";
                        fields.Insert(2, d);
                    }
                    else
                        fields.Remove(d);
                }

                grid.Columns.Clear();

                // Creazione delle colonne
                foreach (Field field in fields)
                {
                    BoundField column = null;
                    ButtonField columnHL = null;
                    TemplateField columnCKB = null;
                    if (field.OriginalLabel.ToUpper().Equals("DOCUMENTO"))
                    {
                        columnHL = GridManager.GetLinkColumn(field.Label,
                            field.FieldId,
                            field.Width);
                        columnHL.SortExpression = field.FieldId;
                    }
                    else
                    {

                        if (field is SpecialField)
                        {
                            switch (((SpecialField)field).FieldType)
                            {
                                case SpecialFieldsEnum.Icons:
                                    columnCKB = GridManager.GetBoundColumnIcon(field.Label, field.Width, field.FieldId);
                                    columnCKB.SortExpression = field.FieldId;
                                    break;
                                case SpecialFieldsEnum.CheckBox:
                                    {
                                        columnCKB = GridManager.GetBoundColumnCheckBox(field.Label, field.Width, field.FieldId);
                                        columnCKB.SortExpression = field.FieldId;
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (field.FieldId)
                            {
                                case "CONTATORE":
                                    {
                                        column = GridManager.GetBoundColumn(
                                            field.Label,
                                            field.OriginalLabel,
                                            100,
                                            field.FieldId);
                                        column.SortExpression = field.FieldId;
                                        break;
                                    }

                                default:
                                    {
                                        column = GridManager.GetBoundColumn(
                                         field.Label,
                                         field.OriginalLabel,
                                         field.Width,
                                         field.FieldId);
                                        column.SortExpression = field.FieldId;
                                        break;
                                    }
                            }
                        }
                    }



                    if (columnCKB != null)
                        grid.Columns.Add(columnCKB);
                    else
                        if (column != null)
                            grid.Columns.Add(column);
                        else
                            grid.Columns.Add(columnHL);



                    if (!this.CellPosition.ContainsKey(field.FieldId))
                    {
                        CellPosition.Add(field.FieldId, position);
                    }
                    // Aggiornamento della posizione
                    position += 1;
                }
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IdProfile", "IdProfile"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("FileExtension", "FileExtension"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsInStorageArea", "IsInStorageArea"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsInWorkingArea", "IsInWorkingArea"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsInWorkingAreaRole", "IsInWorkingAreaRole"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("ProtoType", "ProtoType"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsSigned", "IsSigned"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("InLibroFirma", "InLibroFirma"));
                // Altrimenti si procede con la creazione di una colonna normale

                return grid;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione per la compilazione del datagrid da associare al datagrid
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="result"></param>
        public DataTable FillDataSet(DataTable dataTable,
            SearchObject[] result, Grid selectedGrid,
            EtichettaInfo[] labels, Templates templates, bool showGridPersonalization)
        {
            try
            {
                List<Field> visibleFields = selectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field))).ToList();
                Field specialField = selectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(SpecialField)) && ((SpecialField)e).FieldType.Equals(SpecialFieldsEnum.Icons)).FirstOrDefault<Field>();

                Templates templateTemp = templates;

                OggettoCustom customObjectTemp = new OggettoCustom();

                if (templates != null && !showGridPersonalization)
                {
                    customObjectTemp = templateTemp.ELENCO_OGGETTI.Where(
                         e => e.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && e.DA_VISUALIZZARE_RICERCA == "1").FirstOrDefault();

                    Field d = new Field();

                    if (customObjectTemp != null)
                    {
                        d.AssociatedTemplateName = templateTemp.DESCRIZIONE;
                        d.CustomObjectId = customObjectTemp.SYSTEM_ID;
                        d.FieldId = "CONTATORE";
                        d.IsNumber = true;
                        d.Label = customObjectTemp.DESCRIZIONE;
                        d.OriginalLabel = customObjectTemp.DESCRIZIONE;
                        d.OracleDbColumnName = "to_number(getcontatoredocordinamento (a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "'))";
                        d.SqlServerDbColumnName = "@dbUser@.getContatoreDocOrdinamento(a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "')";
                        visibleFields.Insert(2, d);
                    }
                    else
                    {
                        visibleFields.Remove(d);
                    }
                }

                string documentDescriptionColor = string.Empty;
                // Individuazione del colore da assegnare alla descrizione del documento
                switch (new DocsPaWebService().getSegnAmm(UIManager.UserManager.GetInfoUser().idAmministrazione))
                {
                    case "0":
                        documentDescriptionColor = "Black";
                        break;
                    case "1":
                        documentDescriptionColor = "Blue";
                        break;
                    default:
                        documentDescriptionColor = "Red";
                        break;
                }

                dataTable.Rows.Remove(dataTable.Rows[0]);
                // Valore da assegnare ad un campo
                string value = string.Empty;
                // Per ogni risultato...
                // La riga da aggiungere al dataset

                DataRow dataRow = null;
                StringBuilder temp;
                foreach (SearchObject doc in result)
                {
                    // ...viene inizializzata una nuova riga
                    dataRow = dataTable.NewRow();

                    foreach (Field field in visibleFields)
                    {
                        string numeroDocumento = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;

                        switch (field.FieldId)
                        {
                            //SEGNATURA
                            case "D8":
                                value = "<span style=\"color:Red; font-weight:bold;\">" + doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue + "</span>";
                                break;
                            //REGISTRO
                            case "D2":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //TIPO
                            case "D3":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_DOCUMENTO_PRINCIPALE")).FirstOrDefault().SearchObjectFieldValue;
                                string tempVal = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                if (!string.IsNullOrEmpty(value))
                                    value = labels.Where(e => e.Codice == "ALL").FirstOrDefault().Descrizione;
                                else
                                    value = labels.Where(e => e.Codice == tempVal).FirstOrDefault().Descrizione;
                                break;
                            //OGGETTO
                            case "D4":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //MITTENTE / DESTINATARIO
                            case "D5":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //MITTENTE
                            case "D6":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //DESTINATARI
                            case "D7":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //DATA
                            case "D9":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            // ESITO PUBBLICAZIONE
                            case "D10":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //DATA ANNULLAMENTO
                            case "D11":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //DOCUMENTO
                            case "D1":
                                // Inizializzazione dello stringbuilder con l'apertura del tag Span in
                                // cui inserire l'identiifcativo del documento
                                string numeroProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue;
                                string dataProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
                                string dataApertura = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
                                string protTit = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("PROT_TIT")).FirstOrDefault().SearchObjectFieldValue;

                                temp = new StringBuilder("<span style=\"color:");
                                // Se il documento è un protocollo viene colorato in rosso altrimenti
                                // viene colorato in nero
                                temp.Append(String.IsNullOrEmpty(numeroProtocollo) ? "Black" : documentDescriptionColor);
                                // Il testo deve essere grassetto
                                temp.Append("; font-weight:bold;\">");

                                // Creazione dell'informazione sul documento
                                if (!String.IsNullOrEmpty(numeroProtocollo))
                                    temp.Append(numeroProtocollo + "<br />" + dataProtocollo);
                                else
                                    temp.Append(numeroDocumento + "<br />" + dataApertura);

                                if (!String.IsNullOrEmpty(protTit))
                                    temp.Append("<br />" + protTit);

                                // Chiusura del tag span
                                temp.Append("</span>");

                                value = temp.ToString();
                                break;
                            //NUMERO PROTOCOLLO
                            case "D12":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //AUTORE
                            case "D13":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //DATA ARCHIVIAZIONE
                            case "D14":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //PERSONALE
                            case "D15":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                                    value = "Si";
                                else
                                    value = "No";
                                break;
                            //PRIVATO
                            case "D16":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                                    value = "Si";
                                else
                                    value = "No";
                                break;
                            //TIPOLOGIA
                            case "U1":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //NOTE
                            case "D17":
                                string valoreChiave = string.Empty;
                                valoreChiave = Utils.InitConfigurationKeys.GetValue("0", "FE_IS_PRESENT_NOTE");

                                if (!string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("1"))
                                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ESISTE_NOTA")).FirstOrDefault().SearchObjectFieldValue;
                                else
                                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //CONTATORE
                            case "CONTATORE":
                                value = string.Empty;
                                try
                                {
                                    bool existsCounter = (doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault() != null ? true : false);
                                    if (existsCounter)
                                    {
                                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                    }
                                    //verifico se si tratta di un contatore di reertorio
                                    if (value.ToUpper().Equals("#CONTATORE_DI_REPERTORIO#"))
                                    {
                                        //reperisco la segnatura di repertorio
                                        string dNumber = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                                        value = DocumentManager.getSegnaturaRepertorio(dNumber, AdministrationManager.AmmGetInfoAmmCorrente(UserManager.GetInfoUser().idAmministrazione).Codice);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    UIManager.AdministrationManager.DiagnosticError(ex);
                                    return null;
                                }
                                break;
                            //COD. FASCICOLI
                            case "D18":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //Nome e cognome autore
                            case "D19":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //Ruolo autore
                            case "D20":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //Data arrivo
                            case "D21":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //Stato del documento
                            case "D22":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            case "IMPRONTA":
                                // IMPRONTA FILE
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //DATE INSERT IN ADL
                            case "DTA_ADL":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            // PEC 4 Requisito 3: ricerca documenti spediti
                            // Esito della spedizione
                            case "esito_spedizione":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            // PEC 4 Requisito 3: ricerca documenti spediti
                            // Conto delle ricevute
                            case "count_ric_interop":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            // INTEGRAZIONE PITRE-PARER
                            case "stato_conservazione":
                                string codStato = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                //string language = UIManager.UserManager.GetUserLanguage();
                                switch (codStato)
                                {
                                    case "N":
                                        value = this.GetLabel("optConsNC");
                                        break;
                                    case "V":
                                        value = "In attesa di versamento";
                                        break;
                                    case "W":
                                        value = "Versamento in corso";
                                        break;
                                    case "C":
                                        value = "Preso in carico";
                                        break;
                                    case "R":
                                        value = "Rifiutato";
                                        break;
                                    case "E":
                                        value = "Errore nell'invio";
                                        break;
                                    case "T":
                                        value = "Timeout nell'operazione";
                                        break;
                                    case "F":
                                        value = this.GetLabel("optConsFld");
                                        break;
                                    case "B":
                                        value = this.GetLabel("optConsBfw");
                                        break;
                                    case "K":
                                        value = this.GetLabel("optConsBfe");
                                        break;
                                }
                                break;
                            case "CODICE_POLICY":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            case "CONTATORE_POLICY":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            case "DATA_ESECUZIONE_POLICY":
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            // STATO TASK
                            case "CHA_TASK_STATUS":
                                string status = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                switch (status)
                                {
                                    case "IN_PROGRESS":
                                        value = this.GetLabel("TaskStatusInProgress");
                                        break;
                                    case "CLOSED":
                                        value = this.GetLabel("TaskStatusClosed");
                                        break;
                                    default:
                                        value = this.GetLabel("TaskStatusNA");
                                        break;
                                }
                                break;
                            //OGGETTI CUSTOM
                            default:
                                try
                                {
                                    if (!string.IsNullOrEmpty(doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue))
                                    {
                                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                        if (value.ToUpper().Equals("#CONTATORE_DI_REPERTORIO#"))
                                        {
                                            //reperisco la segnatura di repertorio
                                            string dNumber = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                                            value = DocumentManager.getSegnaturaRepertorio(dNumber, AdministrationManager.AmmGetInfoAmmCorrente(UserManager.GetInfoUser().idAmministrazione).Codice);
                                        }
                                    }
                                    else
                                    {
                                        value = "";
                                    }
                                }
                                catch (Exception e)
                                {
                                    value = "";
                                }
                                break;
                        }

                        // Valorizzazione del campo fieldName
                        // Se il documento è annullato, viene mostrato un testo barrato, altrimenti
                        // viene mostrato così com'è
                        string dataAnnullamento = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D11")).FirstOrDefault().SearchObjectFieldValue;
                        if (!String.IsNullOrEmpty(dataAnnullamento))
                            dataRow[field.FieldId] = String.Format("<span id=\"doc" + numeroDocumento + "\"  style=\"text-decoration: line-through; color: Red;\">{0}</span>", value);
                        else
                            dataRow[field.FieldId] = String.Format("<span id=\"doc" + numeroDocumento + "\">{0}</span>", value);
                        value = string.Empty;
                    }

                    string immagineAcquisita = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D23")).FirstOrDefault().SearchObjectFieldValue;
                    string inConservazione = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_CONSERVAZIONE")).FirstOrDefault().SearchObjectFieldValue;
                    string inAdl = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_ADL")).FirstOrDefault().SearchObjectFieldValue;
                    string inAdlRole = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_ADLROLE")).FirstOrDefault().SearchObjectFieldValue;
                    string isFirmato = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("CHA_FIRMATO")).FirstOrDefault().SearchObjectFieldValue;
                    string inLibroFirma = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_LIBROFIRMA")).FirstOrDefault().SearchObjectFieldValue;
                    string signType = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("CHA_TIPO_FIRMA")).FirstOrDefault().SearchObjectFieldValue;

                    dataRow["ProtoType"] = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D3")).FirstOrDefault().SearchObjectFieldValue;
                    dataRow["IdProfile"] = doc.SearchObjectID;
                    dataRow["FileExtension"] = !String.IsNullOrEmpty(immagineAcquisita) && immagineAcquisita != "0" ? immagineAcquisita : String.Empty;
                    dataRow["IsInStorageArea"] = !String.IsNullOrEmpty(inConservazione) && inConservazione != "0" ? true : false;
                    dataRow["IsInWorkingArea"] = !String.IsNullOrEmpty(inAdl) && inAdl != "0" ? true : false;
                    dataRow["IsInWorkingAreaRole"] = !String.IsNullOrEmpty(inAdlRole) && inAdlRole != "0" ? true : false;
                    dataRow["IsSigned"] = !String.IsNullOrEmpty(isFirmato) && isFirmato != "0" ? true : false;
                    dataRow["InLibroFirma"] = !String.IsNullOrEmpty(inLibroFirma) && inLibroFirma != "0" ? true : false;

                    if (this.ListToSign != null && this.ListCheck != null && this.ListCheck.ContainsKey(doc.SearchObjectID))
                    {
                        FileToSign file = null;
                        string fileExstention = !String.IsNullOrEmpty(immagineAcquisita) && immagineAcquisita != "0" ? immagineAcquisita : String.Empty;
                        if (!this.ListToSign.ContainsKey(doc.SearchObjectID))
                        {
                            file = new FileToSign(fileExstention, isFirmato, signType);
                            this.ListToSign.Add(doc.SearchObjectID, file);
                        }
                        else
                        {
                            file = this.ListToSign[doc.SearchObjectID];
                            file.signed = isFirmato;
                            file.fileExtension = fileExstention;
                            file.signType = signType;
                        }
                    }
                    // ...aggiunta della riga alla collezione delle righe
                    dataTable.Rows.Add(dataRow);
                }
                return dataTable;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        /// <summary>
        /// Funzione per la visualizzazione dei risutati della ricerca
        /// </summary>
        /// <param name="result">I risultati della ricerca</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        private void ShowResult(Grid selectedGrid, SearchObject[] result, int recordNumber, int selectedPage, EtichettaInfo[] labels)
        {
            this.ShowGrid(selectedGrid, this.Result, this.RecordCount, selectedPage, labels);

            this.grid_pageindex.Value = (this.PageCount - 1).ToString();
            this.grid_pageindex.Value = this.SelectedPage.ToString();
            this.gridViewResult.PageIndex = this.PageCount;
            this.gridViewResult.SelectedIndex = this.SelectedPage;
            this.BuildGridNavigator();

        }

        protected void BuildGridNavigator()
        {
            try
            {
                this.plcNavigator.Controls.Clear();

                int countPage = this.PageCount;

                //int val = this.RecordCount % this.PageSize;
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

        /// <summary>
        /// Funzione per la ricerca dei documenti
        /// </summary>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        private SearchObject[] SearchDocument(FiltroRicerca[][] searchFilters, int selectedPage, out int recordNumber, out bool outOfMaxRowSearchable)
        {
            // Documenti individuati dalla ricerca
            SearchObject[] documents;

            // Informazioni sull'utente
            InfoUtente userInfo;

            // Numero totale di pagine
            int pageNumbers;

            // Lista dei system id dei documenti restituiti dalla ricerca
            SearchResultInfo[] idProfiles = null;

            // Prelevamento delle informazioni sull'utente
            userInfo = UserManager.GetInfoUser();

            // Recupero dei campi della griglia impostati come visibili
            Field[] visibleArray = null;


            if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Document)
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);

            List<Field> visibleFields = GridManager.SelectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field))).ToList();
            Field specialField = GridManager.SelectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(SpecialField)) && ((SpecialField)e).FieldType.Equals(SpecialFieldsEnum.Icons)).FirstOrDefault<Field>();

            Templates templateTemp = Session["templateRicerca"] as Templates;

            OggettoCustom customObjectTemp = new OggettoCustom();

            if (templateTemp != null && !this.ShowGridPersonalization)
            {
                customObjectTemp = templateTemp.ELENCO_OGGETTI.Where(
                     e => e.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && e.DA_VISUALIZZARE_RICERCA == "1").FirstOrDefault();

                Field d = new Field();

                if (customObjectTemp != null)
                {
                    d.AssociatedTemplateName = templateTemp.DESCRIZIONE;
                    d.CustomObjectId = customObjectTemp.SYSTEM_ID;
                    d.FieldId = "CONTATORE";
                    d.IsNumber = true;
                    d.Label = customObjectTemp.DESCRIZIONE;
                    d.OriginalLabel = customObjectTemp.DESCRIZIONE;
                    d.OracleDbColumnName = "to_number(getcontatoredocordinamento (a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "'))";
                    d.SqlServerDbColumnName = "@dbUser@.getContatoreDocOrdinamento(a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "')";
                    visibleFields.Insert(2, d);
                }
                else
                {
                    visibleFields.Remove(d);
                }
            }

            //visibleFields = GridManager.SelectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field)) && e.CustomObjectId != 0).ToList();

            if (visibleFields != null && visibleFields.Count > 0)
            {
                visibleArray = visibleFields.ToArray();
            }

            documents = DocumentManager.getQueryInfoDocumentoPagingCustom(userInfo, this, searchFilters, selectedPage, out pageNumbers, out recordNumber, true, true, this.ShowGridPersonalization, this.PageSize, false, visibleArray, null, out idProfiles);

            /* ABBATANGELI GIANLUIGI
             * outOfMaxRowSearchable viene impostato a true se getQueryInfoDocumentoPagingCustom
             * restituisce pageNumbers = -2 (raggiunto il numero massimo di righe possibili come risultato di ricerca)*/
            outOfMaxRowSearchable = (pageNumbers == -2);

            this.RecordCount = recordNumber;
            //this.PageCount = pageNumbers;
            this.PageCount = (int)Math.Round(((double)recordNumber / (double)this.PageSize) + 0.49);
            this.Result = documents;

            //appoggio il risultato in sessione.
            if (idProfiles != null && idProfiles.Length > 0)
            {
                this.IdProfileList = new string[idProfiles.Length];
                this.CodeProfileList = new string[idProfiles.Length];
                for (int i = 0; i < idProfiles.Length; i++)
                {
                    this.IdProfileList[i] = idProfiles[i].Id;
                    this.CodeProfileList[i] = idProfiles[i].Id;
                }
            }

            return documents;
        }

        private bool SearchDocumentFilters()
        {
            //try
            //{
            DocsPaWR.FiltroRicerca[][] qV;
            DocsPaWR.FiltroRicerca[] fVList;
            DocsPaWR.FiltroRicerca fV1;
            //array contenitore degli array filtro di ricerca
            qV = new DocsPaWR.FiltroRicerca[1][];
            qV[0] = new DocsPaWR.FiltroRicerca[1];
            fVList = new DocsPaWR.FiltroRicerca[0];

            string valore = string.Empty;

            #region filtro Archivio (Arrivo, Partenza, Tutti)
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
            fV1.valore = "tipo";
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            #endregion
            #region filtro tipo
            if (this.cbl_archDoc_E.Items.FindByValue("A") != null)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
                if (this.cbl_archDoc_E.Items.FindByValue("A").Selected)
                    fV1.valore = "true";
                else
                    fV1.valore = "false";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.cbl_archDoc_E.Items.FindByValue("P") != null)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
                if (this.cbl_archDoc_E.Items.FindByValue("P").Selected)
                    fV1.valore = "true";
                else
                    //valore += "0^";
                    fV1.valore = "false";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.cbl_archDoc_E.Items.FindByValue("I") != null)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
                if (this.cbl_archDoc_E.Items.FindByValue("I").Selected)
                    fV1.valore = "true";
                else
                    fV1.valore = "false";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            #region filtro repertoriati

            #region DOC_REPERTORIATI
            if (this.cbl_archDoc_E.Items.FindByValue("REP") != null && this.cbl_archDoc_E.Items.FindByValue("REP").Selected)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DOC_REPERTORIATO.ToString();
                fV1.valore = this.cbl_archDoc_E.Items.FindByValue("REP").Selected.ToString();
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region NUMERO_REPERTORIO
            if (this.ddl_numRep.SelectedIndex == 0)
            {

                if (this.txt_initNumRep.Text != null && !this.txt_initNumRep.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUM_REPERTORIO.ToString();
                    fV1.valore = this.txt_initNumRep.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            else
            {
                if (!this.txt_initNumRep.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUM_REPERTORIO_DAL.ToString();
                    fV1.valore = this.txt_initNumRep.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.txt_fineNumRep.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUM_REPERTORIO_AL.ToString();
                    fV1.valore = this.txt_fineNumRep.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region DATA_REPERTORIO

            if (this.ddl_dataRepertorio.SelectedIndex == 2)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_REPERTORIO_TODAY.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataRepertorio.SelectedIndex == 3)
            {
                // siamo nel caso di Settimana corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_REPERTORIO_SC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataRepertorio.SelectedIndex == 4)
            {
                // siamo nel caso di Mese corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_REPERTORIO_MC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataRepertorio.SelectedIndex == 0)
            {//valore singolo carico DATA_PROTOCOLLO_MIT
                if (!string.IsNullOrEmpty(this.txt_initDataRep.Text))
                {
                    if (!utils.isDate(this.txt_initDataRep.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchProjectFilterDate', 'warning', '');", true);
                        return false;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_REPERTORIO_IL.ToString();
                    fV1.valore = this.txt_initDataRep.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            if (this.ddl_dataRepertorio.SelectedIndex == 1)
            {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
                if (!string.IsNullOrEmpty(txt_initDataRep.Text) &&
                  !string.IsNullOrEmpty(txt_fineDataRep.Text) &&
                  utils.verificaIntervalloDate(txt_initDataRep.Text, txt_fineDataRep.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDateArrivoInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDateArrivoInterval', 'warning', '');};", true);
                    return false;
                }
                if (!this.txt_initDataRep.Text.Equals(""))
                {
                    if (!utils.isDate(this.txt_initDataRep.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchProjectFilterDate', 'warning', '');", true);
                        return false;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_REPERTORIO_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.txt_initDataRep.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.txt_fineDataRep.Text.Equals(""))
                {
                    if (!utils.isDate(this.txt_fineDataRep.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchProjectFilterDate', 'warning', '');", true);
                        return false;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_REPERTORIO_PRECEDENTE_IL.ToString();
                    fV1.valore = this.txt_fineDataRep.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            #endregion
            #endregion

            #region filtro per Stampe Registro
            if (this.cbl_archDoc_E.Items.FindByValue("R") != null)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.STAMPA_REG.ToString();
                if (this.cbl_archDoc_E.Items.FindByValue("R").Selected)
                    if (this.cbl_archDoc_E.Items.FindByValue("A").Selected ||
                        this.cbl_archDoc_E.Items.FindByValue("P").Selected ||
                        this.cbl_archDoc_E.Items.FindByValue("G").Selected ||
                        this.cbl_archDoc_E.Items.FindByValue("Pr").Selected ||
                        this.cbl_archDoc_E.Items.FindByValue("ALL").Selected)
                    {
                        fV1.valore = "U^true";
                    }
                    else
                        fV1.valore = "true";
                else
                    fV1.valore = "false";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                if (this.ddl_dataStampa_E.SelectedIndex == 0)
                {//valore singolo carico DATA_STAMPA
                    if (!this.txt_initDataStampa_E.Text.Equals(""))
                    {

                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO.ToString();
                        fV1.valore = this.txt_initDataStampa_E.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dataStampa_E.SelectedIndex == 1)
                {//valore singolo carico DATA_STAMPA_DAL - DATA_STAMPA_AL
                    if (!string.IsNullOrEmpty(txt_initDataStampa_E.Text) &&
                        !string.IsNullOrEmpty(txt_finedataStampa_E.Text) &&
                        utils.verificaIntervalloDate(txt_initDataStampa_E.Text, txt_finedataStampa_E.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDataStampaInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDataStampaInterval', 'warning', '');};", true);
                        return false;
                    }
                    if (!this.txt_initDataStampa_E.Text.Equals(""))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_DAL.ToString();
                        fV1.valore = this.txt_initDataStampa_E.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.txt_finedataStampa_E.Text.Equals(""))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_AL.ToString();
                        fV1.valore = this.txt_finedataStampa_E.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                //// numero protocollo stampe registro
                //if (this.ddl_numProt_E.SelectedIndex == 0)
                //{//valore singolo carico NUM_PROTOCOLLO
                //    if (this.txt_initNumProt_E.Text != null && !this.txt_initNumProt_E.Text.Equals(""))
                //    {
                //        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //        fV1.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA.ToString();
                //        fV1.valore = this.txt_initNumProt_E.Text;
                //        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //    }
                //}
                //else
                //{//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                //    if (!this.txt_initNumProt_E.Text.Equals(""))
                //    {
                //        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //        fV1.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA_DAL.ToString();
                //        fV1.valore = this.txt_initNumProt_E.Text;
                //        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //    }
                //    if (!this.txt_fineNumProt_E.Text.Equals(""))
                //    {
                //        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //        fV1.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA_AL.ToString();
                //        fV1.valore = this.txt_fineNumProt_E.Text;
                //        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //    }
                //}

                if (!string.IsNullOrEmpty(this.TxtYear.Text))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriStampaRegistro.ANNO_PROTOCOLLO_STAMPA.ToString();
                    fV1.valore = this.TxtYear.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
            fV1.valore = this.cbl_archDoc_E.Items.FindByValue("G").Selected.ToString();
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

            if (this.IsEnabledProfilazioneAllegato && this.cbl_archDoc_E.Items.FindByValue("ALL").Selected)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ALLEGATO.ToString();
                fV1.valore = this.rblFiltriAllegati.SelectedValue.ToString();
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.cbl_archDoc_E.Items.FindByValue("Pr") != null)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                if (this.cbl_archDoc_E.Items.FindByValue("Pr").Selected)
                    //valore += "1";
                    fV1.valore = "true";
                else
                    //valore += "0";
                    fV1.valore = "false";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }



            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
            fV1.valore = "tipo";
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            #endregion
            #region filtro registro
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
            string registri = "";
            if (this.lb_reg_C.Items.Count > 0)
            {
                for (int i = 0; i < this.lb_reg_C.Items.Count; i++)
                {
                    if (this.lb_reg_C.Items[i].Selected)
                    {
                        if (!string.IsNullOrEmpty(registri)) registri += ",";
                        registri += this.lb_reg_C.Items[i].Value;
                    }

                }
            }
            if (!registri.Equals(""))
            {
                fV1.valore = registri;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region filtro docNumber
            if (this.ddl_idDocumento_C.SelectedIndex == 0)
            {
                if (this.txt_initIdDoc_C.Text != null && !this.txt_initIdDoc_C.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();
                    fV1.valore = this.txt_initIdDoc_C.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            else
            {
                if (this.txt_initIdDoc_C.Text != null && !this.txt_initIdDoc_C.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
                    fV1.valore = this.txt_initIdDoc_C.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.txt_fineIdDoc_C.Text != null && !this.txt_fineIdDoc_C.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
                    fV1.valore = this.txt_fineIdDoc_C.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            //#endregion
            //#region filtro registro
            //fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
            //fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
            //if (!isLimited) // se la ricerca non è limitata
            //{
            //    string registri = "";
            //    if (this.lb_reg_E.Items.Count > 0)
            //    {
            //        for (int i = 0; i < this.lb_reg_E.Items.Count; i++)
            //        {
            //            if (this.lb_reg_E.Items[i].Selected)
            //                registri += this.lb_reg_E.Items[i].Value + ",";
            //        }
            //    }
            //    if (!registri.Equals(""))
            //    {
            //        registri = registri.Substring(0, registri.Length - 1);
            //        fV1.valore = registri;
            //        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            //    }
            //    else
            //    {
            //        Response.Write("<script>alert('Selezionare almeno un registro');top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
            //        return false;
            //    }
            //}
            //else
            //{
            //    fV1.valore = ddl_registri.SelectedValue;
            //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            //}

            #endregion
            #region filtro numero protocollo
            if (this.ddl_numProt_E.SelectedIndex == 0)
            {//valore singolo carico NUM_PROTOCOLLO

                if (this.txt_initNumProt_E.Text != null && !this.txt_initNumProt_E.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();
                    fV1.valore = this.txt_initNumProt_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            else
            {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                if (!this.txt_initNumProt_E.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                    fV1.valore = this.txt_initNumProt_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.txt_fineNumProt_E.Text.Equals(""))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
                    fV1.valore = this.txt_fineNumProt_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region filtro Data Scadenza
            if (this.ddl_dataScadenza_C.SelectedIndex == 2)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCAD_TODAY.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataScadenza_C.SelectedIndex == 3)
            {
                // siamo nel caso di Settimana corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCAD_SC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataScadenza_C.SelectedIndex == 4)
            {
                // siamo nel caso di Mese corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCAD_MC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataScadenza_C.SelectedIndex == 0)
            {
                if (!this.txt_initDataScadenza_C.Text.Equals(""))
                {
                    if (!utils.isDate(this.txt_initDataScadenza_C.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchProjectFilterDate', 'warning', '');", true);
                        return false;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCADENZA_IL.ToString();
                    fV1.valore = this.txt_initDataScadenza_C.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            if (this.ddl_dataScadenza_C.SelectedIndex == 1)
            {
                if (!string.IsNullOrEmpty(txt_initDataScadenza_C.Text) &&
                    !string.IsNullOrEmpty(txt_fineDataScadenza_C.Text) &&
                    utils.verificaIntervalloDate(txt_initDataScadenza_C.Text, txt_fineDataScadenza_C.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDateExpireInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDateExpireInterval', 'warning', '');};", true);
                    return false;
                }
                if (!this.txt_initDataScadenza_C.Text.Equals(""))
                {
                    if (!utils.isDate(this.txt_initDataScadenza_C.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchProjectFilterDate', 'warning', '');", true);
                        return false;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCADENZA_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.txt_initDataScadenza_C.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.txt_fineDataScadenza_C.Text.Equals(""))
                {
                    if (!utils.isDate(this.txt_fineDataScadenza_C.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchProjectFilterDate', 'warning', '');", true);
                        return false;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCADENZA_PRECEDENTE_IL.ToString();
                    fV1.valore = this.txt_fineDataScadenza_C.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region filtro data protocollo
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
            if (this.ddl_dataProt_E.SelectedIndex == 5)
            {
                // siamo nel caso di Ieri
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROTO_YESTERDAY.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProt_E.SelectedIndex == 6)
            {
                // siamo nel caso di Ultimi 7 giorni
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROTO_LAST_SEVEN_DAYS.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProt_E.SelectedIndex == 7)
            {
                // siamo nel caso di Ultimi 31 giorni
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROTO_LAST_THIRTY_ONE_DAYS.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProt_E.SelectedIndex == 0)
            {//valore singolo carico DATA_PROTOCOLLO
                if (!this.txt_initDataProt_E.Text.Equals(""))
                {

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString();
                    fV1.valore = this.txt_initDataProt_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
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
                string maxIntervalDate = Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.MAX_INTERVAL_DATE_SEARCHABLE.ToString());
                if (string.IsNullOrEmpty(this.TxtYear.Text) && !string.IsNullOrEmpty(maxIntervalDate) && maxIntervalDate != "0"
                    && utils.GetYearsFromInterval(txt_initDataProt_E.Text, txt_fineDataProt_E.Text) >= Convert.ToInt32(maxIntervalDate))
                {
                    this.ClearGrid();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningSearchDocumentMaxIntervalRecordDate', 'warning', '', '" + maxIntervalDate + "');} else {parent.ajaxDialogModal('WarningSearchDocumentMaxIntervalRecordDate', 'warning', '', '" + maxIntervalDate + "');}", true);
                    return false;
                }
                if (!this.txt_initDataProt_E.Text.Equals(""))
                {

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.txt_initDataProt_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.txt_fineDataProt_E.Text.Equals(""))
                {

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
                    fV1.valore = this.txt_fineDataProt_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion

            #region filtro data creazione
            if (this.ddl_dataCreazione_E.SelectedIndex == 2)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_TODAY.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataCreazione_E.SelectedIndex == 3)
            {
                // siamo nel caso di Settimana corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_SC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataCreazione_E.SelectedIndex == 4)
            {
                // siamo nel caso di Mese corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_MC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataCreazione_E.SelectedIndex == 5)
            {
                // siamo nel caso di Ieri
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_YESTERDAY.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataCreazione_E.SelectedIndex == 6)
            {
                // siamo nel caso di Ultimi 7 giorni
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_LAST_SEVEN_DAYS.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataCreazione_E.SelectedIndex == 7)
            {
                // siamo nel caso di Ultimi 31 giorni
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_LAST_THIRTY_ONE_DAYS.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataCreazione_E.SelectedIndex == 0)

                if (this.ddl_dataCreazione_E.SelectedIndex == 0)
                { //valore singolo carico DATA_CREAZIONE
                    if (!this.txt_initDataCreazione_E.Text.Equals(""))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
                        fV1.valore = this.txt_initDataCreazione_E.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
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
                string maxIntervalDate = Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.MAX_INTERVAL_DATE_SEARCHABLE.ToString());
                if (string.IsNullOrEmpty(this.TxtYear.Text) && !string.IsNullOrEmpty(maxIntervalDate) && maxIntervalDate != "0"
                    && utils.GetYearsFromInterval(txt_initDataCreazione_E.Text, txt_finedataCreazione_E.Text) >= Convert.ToInt32(maxIntervalDate))
                {
                    this.ClearGrid();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningSearchDocumentMaxIntervalDate', 'warning', '', '" + maxIntervalDate + "');} else {parent.ajaxDialogModal('WarningSearchDocumentMaxIntervalDate', 'warning', '', '" + maxIntervalDate + "');}", true);
                    return false;
                }
                if (!this.txt_initDataCreazione_E.Text.Equals(""))
                {

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.txt_initDataCreazione_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.txt_finedataCreazione_E.Text.Equals(""))
                {

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                    fV1.valore = this.txt_finedataCreazione_E.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region filtro Data Stampa Registro
            //if (this.ddl_dataStampa_E.SelectedIndex == 2)
            //{
            //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
            //    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_STAMPA_TODAY.ToString();
            //    fV1.valore = "1";
            //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            //}
            //if (this.ddl_dataStampa_E.SelectedIndex == 3)
            //{
            //    // siamo nel caso di Settimana corrente
            //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
            //    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_STAMPA_SC.ToString();
            //    fV1.valore = "1";
            //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            //}
            //if (this.ddl_dataStampa_E.SelectedIndex == 4)
            //{
            //    // siamo nel caso di Mese corrente
            //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
            //    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_STAMPA_MC.ToString();
            //    fV1.valore = "1";
            //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            //}

            #endregion
            #region filtro oggetto

            if (!string.IsNullOrEmpty(this.TxtObject.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.OGGETTO.ToString();
                fV1.valore = utils.DO_AdattaString(this.TxtObject.Text);
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
             /*
             * EMANUELA 24/04/2015: AGGIUNTO NUOVO CAMPO OGGETTO DEL DOCUMENTO PRINCIPALE
             * */
            if (this.EnabledSearchAttachByDescMainDoc && !string.IsNullOrEmpty(this.HiddenDisplayPanel.Value))
            {
                if (!string.IsNullOrEmpty(this.TxtObjectAttach.Text))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.OGGETTO_DOCUMENTO_PRINCIPALE.ToString();
                    fV1.valore = utils.DO_AdattaString(this.TxtObjectAttach.Text);
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion

            #region filtro mitt/dest
            if (!string.IsNullOrEmpty(this.IdRecipient.Value))
            {
                if (!this.txt_descrMit_E.Text.Equals(""))
                {
                    if (!string.IsNullOrEmpty(this.txt_codMit_E.Text))
                    {
                        if (this.chk_mitt_dest_storicizzati.Checked)
                        {
                            // Ricerca i documenti per i mittenti / destinatari storicizzati
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString();
                            fV1.valore = this.txt_codMit_E.Text;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString();
                            fV1.valore = this.chk_mitt_dest_storicizzati.Checked.ToString();
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        else
                        {
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString();
                            fV1.valore = this.IdRecipient.Value;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                    }
                }
            }
            else
            {
                if (!this.txt_descrMit_E.Text.Equals(""))
                {
                    if (!string.IsNullOrEmpty(this.txt_codMit_E.Text))
                    {
                        if (this.chk_mitt_dest_storicizzati.Checked)
                        {
                            // Ricerca i documenti per i mittenti / destinatari storicizzati
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString();
                            fV1.valore = this.txt_codMit_E.Text;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString();
                            fV1.valore = this.chk_mitt_dest_storicizzati.Checked.ToString();
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        else
                        {
                            // Ricerca dell'id del corrispondente a partire dal codice
                            DocsPaWR.Corrispondente corrByCode = AddressBookManager.getCorrispondenteByCodRubrica(this.txt_codMit_E.Text, false);
                            if (corrByCode != null)
                            {
                                this.IdRecipient.Value = corrByCode.systemId;

                                fV1 = new DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString();
                                fV1.valore = this.IdRecipient.Value;
                                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                            else
                            {
                                fV1 = new DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST.ToString();
                                fV1.valore = this.txt_descrMit_E.Text;
                                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                        }
                    }
                    else
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST.ToString();
                        fV1.valore = this.txt_descrMit_E.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
            }
            #endregion
            #region filtro numero oggetto
            if (!this.txt_numOggetto.Text.Equals(""))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.NUM_OGGETTO.ToString();
                fV1.valore = this.txt_numOggetto.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region filtro commissione referente
            if (this.txt_commRef.Text != null && !this.txt_commRef.Text.Equals(""))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.COMMISSIONE_REF.ToString();
                fV1.valore = this.txt_commRef.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion

            #region Filtri Creatore

            if (!string.IsNullOrEmpty(this.idCreatore.Value))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = "ID_AUTHOR";
                fV1.valore = this.idCreatore.Value;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = "CORR_TYPE_AUTHOR";
                fV1.valore = this.rblOwnerType.SelectedValue;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = "EXTEND_TO_HISTORICIZED_AUTHOR";
                fV1.valore = this.chkCreatoreExtendHistoricized.Checked.ToString();
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

            }
            else if (!string.IsNullOrEmpty(this.txtDescrizioneCreatore.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = "DESC_AUTHOR";
                fV1.valore = this.txtDescrizioneCreatore.Text;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.CORR_TYPE_OWNER.ToString();
                fV1.valore = this.rblProprietarioType.SelectedValue;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
            }

            #endregion
            #region Filtri Proprietario

            if (!string.IsNullOrEmpty(this.idProprietario.Value))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_OWNER.ToString();
                fV1.valore = this.idProprietario.Value;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.CORR_TYPE_OWNER.ToString();
                fV1.valore = this.rblProprietarioType.SelectedValue;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                //fV1 = new DocsPaWR.FiltroRicerca();
                //fV1.argomento = "EXTEND_TO_HISTORICIZED_OWNER";
                //fV1.valore = this.chkCreatoreExtendHistoricized.Checked.ToString();
                //fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
            }
            else if (!string.IsNullOrEmpty(this.txtDescrizioneProprietario.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = "DESC_OWNER";
                fV1.valore = this.txtDescrizioneProprietario.Text;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.CORR_TYPE_OWNER.ToString();
                fV1.valore = this.rblProprietarioType.SelectedValue;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion

            #region filtro CODICE FASCICOLO
            if (!this.txt_CodFascicolo.Text.Equals(""))
            {
                if (this.cbxEstendiAFascicoli.Checked)
                {
                    fV1 = new NttDataWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.ESTENDI_A_NODI_FIGLI_E_FASCICOLI.ToString();
                    fV1.valore = this.txt_CodFascicolo.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                else
                {
                    #region costruzione condizione IN per valorizzare il filtro di ricerca IN_CHILD_RIC_ESTESA
                    ArrayList listaFascicoli = null;
                    if (ProjectManager.getFascicoloSelezionatoFascRapida(this) != null)
                    {
                        listaFascicoli = new ArrayList();
                        listaFascicoli.Add(ProjectManager.getFascicoloSelezionatoFascRapida(this));
                    }
                    else //da Cambiare perchè cerca in tutti i fascicoli indipentemente da quello selezionato !!!
                        listaFascicoli = new ArrayList(ProjectManager.getListaFascicoliDaCodice(this, this.txt_CodFascicolo.Text, UserManager.getRegistroSelezionato(this), "R"));

                    string inSubFolder = "IN (";
                    for (int k = 0; k < listaFascicoli.Count; k++)
                    {
                        DocsPaWR.Folder folder = ProjectManager.getFolder(this, (DocsPaWR.Fascicolo)listaFascicoli[k]);
                        inSubFolder += folder.systemID;
                        if (folder.childs != null && folder.childs.Length > 0)
                        {
                            for (int i = 0; i < folder.childs.Length; i++)
                            {
                                inSubFolder += ", " + folder.childs[i].systemID;
                                inSubFolder = this.getInStringChild(folder.childs[i], inSubFolder);
                            }
                        }
                        inSubFolder += ",";
                    }
                    inSubFolder = inSubFolder.Substring(0, inSubFolder.Length - 1) + ")";

                    #endregion

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.IN_CHILD_RIC_ESTESA.ToString();
                    fV1.valore = inSubFolder;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region filtro RICERCA IN AREA LAVORO
            if (this.IsAdl)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DOC_IN_ADL.ToString();
                if (this.RblTypeAdl.SelectedValue == "0")
                    fV1.valore = UserManager.GetUserInSession().idPeople.ToString() + "@" + RoleManager.GetRoleInSession().systemId.ToString();
                else
                    fV1.valore = "0@" + RoleManager.GetRoleInSession().systemId.ToString();
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region visualizza storico  ?da aggiustare? il valore da passare pu essere qualunque
            //				if (this.rbl_Rif_E.SelectedItem.Value.Equals("S"))
            //				{
            //					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
            //					fV1.argomento=DocsPaWR.FiltriDocumento.VIS_STORICO_MITT_DEST.ToString();
            //					fV1.valore=this.rbl_Rif_E.SelectedItem.Value;
            //					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
            //				}
            #endregion
            #region Anno Protocollo
            //Impostato controllo sul filtro anno: se anno è vuoto controllo che siano state inserite almeno una di questi filtri data creazione, data protocollo, id documento, numero protocollo
            /*if (string.IsNullOrEmpty(this.TxtYear.Text) && string.IsNullOrEmpty(this.txt_initIdDoc_C.Text)
                && string.IsNullOrEmpty(this.txt_initNumProt_E.Text) && string.IsNullOrEmpty(this.txt_initDataCreazione_E.Text)
                && string.IsNullOrEmpty(this.txt_initDataProt_E.Text))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchDocumentAdvancedYear', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchDocumentAdvancedYear', 'warning', '');};", true);
                return false;
            }
            else
            {*/
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
                fV1.valore = this.TxtYear.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            //}
            #endregion
            #region filtro numero protocollo mittente
            if (!string.IsNullOrEmpty(this.txt_numProtMitt_C.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PROTOCOLLO_MITTENTE.ToString();
                fV1.valore = utils.ReplaceApexes(this.txt_numProtMitt_C.Text);
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region MEZZO SPEDIZIONE
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.MEZZO_SPEDIZIONE.ToString();
            fV1.valore = this.ddl_spedizione.SelectedValue;
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            #endregion
            #region filtro Conservazione
            if (this.cb_Conservato.Checked)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.CONSERVAZIONE.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            else if (this.cb_NonConservato.Checked)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.CONSERVAZIONE.ToString();
                fV1.valore = "0";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region filtro Stato Doc  (Annullato, Non annullato, Tutti)
            if (!this.rb_annulla_C.SelectedItem.Value.Equals("T"))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ANNULLATO.ToString();
                fV1.valore = this.rb_annulla_C.SelectedItem.Value;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region filtro Segnatura

            if (!this.txt_segnatura.Text.Equals(string.Empty))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.SEGNATURA.ToString();
                fV1.valore = this.txt_segnatura.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            #endregion
            #region ID_MITTENTE_INTERMEDIO
            if (!string.IsNullOrEmpty(this.idMittItermedio.Value))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ID_MITTENTE_INTERMEDIO.ToString();
                fV1.valore = this.idMittItermedio.Value;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion ID_MITTENTE_INTERMEDIO
            #region filtro data protocollo mittente
            if (this.ddl_dataProtMitt_C.SelectedIndex == 2)
            {
                // siamo nel caso di Settimana corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_TODAY.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProtMitt_C.SelectedIndex == 3)
            {
                // siamo nel caso di Settimana corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_SC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProtMitt_C.SelectedIndex == 4)
            {
                // siamo nel caso di Mese corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_MC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProtMitt_C.SelectedIndex == 0)
            {//valore singolo carico DATA_PROTOCOLLO_MIT
                if (!string.IsNullOrEmpty(this.txt_initDataProtMitt_C.Text))
                {
                    if (!utils.isDate(this.txt_initDataProtMitt_C.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchProjectFilterDate', 'warning', '');", true);
                        return false;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_IL.ToString();
                    fV1.valore = this.txt_initDataProtMitt_C.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            if (this.ddl_dataProtMitt_C.SelectedIndex == 1)
            {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
                if (!string.IsNullOrEmpty(txt_initDataProtMitt_C.Text) &&
                   !string.IsNullOrEmpty(txt_fineDataProtMitt_C.Text) &&
                   utils.verificaIntervalloDate(txt_initDataProtMitt_C.Text, txt_fineDataProtMitt_C.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDateProtMittInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDateProtMittInterval', 'warning', '');};", true);
                    return false;
                }
                if (!this.txt_initDataProtMitt_C.Text.Equals(""))
                {
                    if (!utils.isDate(this.txt_initDataProtMitt_C.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchProjectFilterDate', 'warning', '');", true);
                        return false;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.txt_initDataProtMitt_C.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.txt_fineDataProtMitt_C.Equals(""))
                {
                    if (!utils.isDate(this.txt_fineDataProtMitt_C.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchProjectFilterDate', 'warning', '');", true);
                        return false;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_PRECEDENTE_IL.ToString();
                    fV1.valore = this.txt_fineDataProtMitt_C.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region filtro data arrivo
            if (this.ddl_dataArrivo_C.SelectedIndex == 2)
            {
                // siamo nel caso di Settimana corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_ARRIVO_TODAY.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataArrivo_C.SelectedIndex == 3)
            {
                // siamo nel caso di Settimana corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_ARRIVO_SC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataArrivo_C.SelectedIndex == 4)
            {
                // siamo nel caso di Mese corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_ARRIVO_MC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataArrivo_C.SelectedIndex == 0)
            {//valore singolo carico DATA_PROTOCOLLO_MIT
                if (!string.IsNullOrEmpty(this.txt_initDataArrivo_C.Text))
                {
                    if (!utils.isDate(this.txt_initDataArrivo_C.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchProjectFilterDate', 'warning', '');", true);
                        return false;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_ARRIVO_IL.ToString();
                    fV1.valore = this.txt_initDataArrivo_C.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            if (this.ddl_dataArrivo_C.SelectedIndex == 1)
            {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
                if (!string.IsNullOrEmpty(txt_initDataArrivo_C.Text) &&
                  !string.IsNullOrEmpty(txt_fineDataArrivo_C.Text) &&
                  utils.verificaIntervalloDate(txt_initDataArrivo_C.Text, txt_fineDataArrivo_C.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDateArrivoInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDateArrivoInterval', 'warning', '');};", true);
                    return false;
                }
                if (!this.txt_initDataArrivo_C.Text.Equals(""))
                {
                    if (!utils.isDate(this.txt_initDataArrivo_C.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchProjectFilterDate', 'warning', '');", true);
                        return false;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_ARRIVO_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.txt_initDataArrivo_C.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.txt_fineDataArrivo_C.Text.Equals(""))
                {
                    if (!utils.isDate(this.txt_fineDataArrivo_C.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchProjectFilterDate', 'warning', '');", true);
                        return false;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_ARRIVO_PRECEDENTE_IL.ToString();
                    fV1.valore = this.txt_fineDataArrivo_C.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region filtro parole chiave
            //creo tanti filtri quante sono le parole chiave (condizione di AND)
            for (int i = 0; i < this.ListKeywords.Items.Count; i++)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PAROLE_CHIAVE.ToString();
                fV1.valore = this.ListKeywords.Items[i].Value;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

            }
            #endregion
            #region filtro note
            if (!string.IsNullOrEmpty(this.Txtnote.Text))
            {
                // string rfsel = RegistryManager.GetListRegistriesAndRF(RoleManager.GetRoleInSession().systemId, "1", string.Empty)[0].systemId;
                string rfsel = this.ddlNoteRF.SelectedValue;
                if (string.IsNullOrEmpty(rfsel))
                {
                    rfsel = RoleManager.GetRoleInSession().registri[0].systemId;
                }
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.NOTE.ToString();
                fV1.valore = utils.ReplaceApexes(this.Txtnote.Text) + "@-@" + this.rl_visibilita.SelectedValue + "@-@" + rfsel;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region filtro su mancanza/presenza Immagine e fascicolazione
            if ((this.cbl_docInCompl.Items.FindByValue("C_Img").Selected && !this.cbl_docInCompl.Items.FindByValue("S_Img").Selected)
                ||
              (!this.cbl_docInCompl.Items.FindByValue("C_Img").Selected && this.cbl_docInCompl.Items.FindByValue("S_Img").Selected))
            {
                if (this.cbl_docInCompl.Items.FindByValue("C_Img").Selected)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.MANCANZA_IMMAGINE.ToString();
                    fV1.valore = "0";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.cbl_docInCompl.Items.FindByValue("S_Img").Selected)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.MANCANZA_IMMAGINE.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            if ((this.cbl_docInCompl.Items.FindByValue("C_Fasc").Selected && !this.cbl_docInCompl.Items.FindByValue("S_Fasc").Selected)
                ||
              (!this.cbl_docInCompl.Items.FindByValue("C_Fasc").Selected && this.cbl_docInCompl.Items.FindByValue("S_Fasc").Selected))
            {
                if (this.cbl_docInCompl.Items.FindByValue("C_Fasc").Selected)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.MANCANZA_FASCICOLAZIONE.ToString();
                    fV1.valore = "0";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.cbl_docInCompl.Items.FindByValue("S_Fasc").Selected)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.MANCANZA_FASCICOLAZIONE.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region filtro TRASMESSI_CON
            if (this.cbx_Trasm.Checked)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.TRASMESSI_CON.ToString();
                if (this.ddl_ragioneTrasm.SelectedIndex == 0)
                    fV1.valore = "Tutte";
                else
                    fV1.valore = this.ddl_ragioneTrasm.SelectedItem.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region filtro TRASMESSI_SENZA
            if (this.cbx_TrasmSenza.Checked)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.TRASMESSI_SENZA.ToString();
                if (this.ddl_ragioneTrasm.SelectedIndex == 0)
                    fV1.valore = "Tutte";
                else
                    fV1.valore = this.ddl_ragioneTrasm.SelectedItem.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                if (this.rbl_neverTrasm.SelectedValue.Equals("U"))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOC_MAI_TRASMESSI_DA_UTENTE.ToString();
                    fV1.valore = UserManager.GetInfoUser().idPeople;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.rbl_neverTrasm.SelectedValue.Equals("R"))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOC_MAI_TRASMESSI_DA_RUOLO.ToString();
                    fV1.valore = RoleManager.GetRoleInSession().systemId;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.rbl_neverTrasm.SelectedValue.Equals("T"))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOC_MAI_TRASMESSI_DA_RUOLO.ToString();
                    fV1.valore = "0";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region FILTRO MAI SPEDITI

            if (this.cb_neverSend.Checked)
            {
                if (this.rbl_NeverSend.SelectedValue.Equals("U"))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOC_MAI_SPEDITI_DA_UTENTE.ToString();
                    fV1.valore = UserManager.GetInfoUser().idPeople;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.rbl_NeverSend.SelectedValue.Equals("R"))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOC_MAI_SPEDITI_DA_RUOLO.ToString();
                    fV1.valore = RoleManager.GetRoleInSession().idGruppo;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.rbl_NeverSend.SelectedValue.Equals("T"))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOC_MAI_SPEDITI.ToString();
                    fV1.valore = "0";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            #endregion
            #region filtro Protocollo Emergenza

            //filtro DATA PROTO EMERGENZA
            if (this.txt_protoEme.Text != null && !this.txt_protoEme.Text.Equals(""))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTO_EMERGENZA.ToString();
                fV1.valore = this.txt_protoEme.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }


            if (this.ddl_dataProtoEme.SelectedIndex == 2)
            {
                // siamo nel caso di Settimana corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_TODAY.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProtoEme.SelectedIndex == 3)
            {
                // siamo nel caso di Settimana corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_SC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProtoEme.SelectedIndex == 4)
            {
                // siamo nel caso di Mese corrente
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_MC.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProtoEme.SelectedIndex == 0)
            {//valore singolo carico DATA_PROTOCOLLO_EMERGENZA
                if (!string.IsNullOrEmpty(this.txt_dataProtoEmeInizio.Text))
                {
                    if (!utils.isDate(this.txt_dataProtoEmeInizio.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchProjectFilterDate', 'warning', '');", true);
                        return false;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_IL.ToString();
                    fV1.valore = this.txt_dataProtoEmeInizio.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            if (this.ddl_dataProtoEme.SelectedIndex == 1)
            {//valore singolo carico DATA_PROTO_EMER_DAL - DATA_PROTO_EMER_AL
                if (!string.IsNullOrEmpty(txt_dataProtoEmeInizio.Text) &&
                  !string.IsNullOrEmpty(txt_dataProtoEmeFine.Text) &&
                  utils.verificaIntervalloDate(txt_dataProtoEmeInizio.Text, txt_dataProtoEmeFine.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDateProtoEmeInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDateProtoEmeInterval', 'warning', '');};", true);
                    return false;
                }
                if (!this.txt_dataProtoEmeInizio.Text.Equals(""))
                {
                    if (!utils.isDate(this.txt_dataProtoEmeInizio.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchProjectFilterDate', 'warning', '');", true);
                        return false;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.txt_dataProtoEmeInizio.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.txt_dataProtoEmeFine.Text.Equals(""))
                {
                    if (!utils.isDate(this.txt_dataProtoEmeFine.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSearchProjectFilterDate', 'warning', '');", true);
                        return false;
                    }
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_PRECEDENTE_IL.ToString();
                    fV1.valore = this.txt_dataProtoEmeFine.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region filtro Evidenza  (Si, No, Tutti)
            if (!this.rb_evidenza_C.SelectedItem.Value.Equals("T"))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.EVIDENZA.ToString();
                fV1.valore = this.rb_evidenza_C.SelectedItem.Value;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region filtro tipo file acquisito
            if (this.ddl_tipoFileAcquisiti.SelectedIndex > 0)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.TIPO_FILE_ACQUISITO.ToString();
                fV1.valore = this.ddl_tipoFileAcquisiti.SelectedItem.Value;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region filtro file firmati
            if (this.chkFirmato.Checked)
            {
                //cerco documenti firmati
                if (!this.chkNonFirmato.Checked)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.FIRMATO.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }//se sono entrambi selezionati cerco i documenti che abbiano un file acquisito, siano essi firmati o meno.
                else
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.FIRMATO.ToString();
                    fV1.valore = "2";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            else
            {
                //cerco i documenti non firmati
                if (this.chkNonFirmato.Checked)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.FIRMATO.ToString();
                    fV1.valore = "0";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region Firma Elettronica

            if (this.chkFirmaElettronica.Checked)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.FIRMA_ELETTRONICA.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                if (!string.IsNullOrEmpty(this.idFirmatario.Value))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = this.rblFirmatarioType.SelectedValue.Equals("P")  ? DocsPaWR.FiltriDocumento.ID_UTENTE_FIRMATARIO_ELETTRONICA.ToString() :
                       DocsPaWR.FiltriDocumento.ID_RUOLO_FIRMATARIO_ELETTRONICA.ToString();
                    fV1.valore = this.idFirmatario.Value;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                else if (!string.IsNullOrEmpty(this.txtDescrizioneFirmatario.Text))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DESC_FIRMATARIO_ELETTRONICA.ToString();
                    fV1.valore = this.txtDescrizioneFirmatario.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                else
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.ID_UTENTE_FIRMATARIO_ELETTRONICA.ToString();
                    fV1.valore = "0";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(this.idFirmatario.Value))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.FIRMA_ELETTRONICA.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = this.rblFirmatarioType.SelectedValue.Equals("P") ? DocsPaWR.FiltriDocumento.ID_UTENTE_FIRMATARIO_ELETTRONICA.ToString() :
                       DocsPaWR.FiltriDocumento.ID_RUOLO_FIRMATARIO_ELETTRONICA.ToString();
                    fV1.valore = this.idFirmatario.Value;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                else if (!string.IsNullOrEmpty(this.txtDescrizioneFirmatario.Text))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.FIRMA_ELETTRONICA.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DESC_FIRMATARIO_ELETTRONICA.ToString();
                    fV1.valore = this.txtDescrizioneFirmatario.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            #endregion
            #region filtro Versioni
            if (!string.IsNullOrEmpty(this.txt_versioni.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.NUMERO_VERSIONI.ToString();
                fV1.valore = this.ddl_op_versioni.SelectedValue + this.txt_versioni.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region filtro Allegati
            if (!string.IsNullOrEmpty(this.txt_allegati.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.NUMERO_ALLEGATI.ToString();
                fV1.valore = this.ddl_op_allegati.SelectedValue + this.txt_allegati.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = "NUMERO_ALLEGATI_TIPO";
                fV1.valore = this.rblFiltriNumAllegati.SelectedValue;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region filtri timestamp
            //Senza timestamp
            if (this.rbl_timestamp.SelectedValue == "1")
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.SENZA_TIMESTAMP.ToString();
                fV1.valore = "1";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            //Con timestamp
            if (this.rbl_timestamp.SelectedValue == "0" && this.ddl_timestamp.SelectedValue == "0" && string.IsNullOrEmpty(this.date_timestamp.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.CON_TIMESTAMP.ToString();
                fV1.valore = "0";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            //Timestamp scaduto
            if (this.rbl_timestamp.SelectedValue == "0" && this.ddl_timestamp.SelectedValue == "1" && string.IsNullOrEmpty(this.date_timestamp.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.TIMESTAMP_SCADUTO.ToString();
                fV1.valore = "0";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            //Timestamp scade prima di
            if (this.rbl_timestamp.SelectedValue == "0" && this.ddl_timestamp.SelectedValue == "2" && !string.IsNullOrEmpty(this.date_timestamp.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.TIMESTAMP_SCADE_PRIMA_DI.ToString();
                fV1.valore = this.date_timestamp.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion filtri timestamp
            #region filtro documenti spediti
            // PEC 4 Requisito 3: ricerca documenti spediti
            // punto ingresso frontend
            int appo = 0;
            string docSpediti = string.Empty;
            if (this.cbx_pec.Visible && this.cbx_pec.Checked)
                appo = 1;
            if (this.cbx_pitre.Visible && this.cbx_pitre.Checked)
                if (appo == 0)
                    appo = 2;
                else
                    appo = 3;

            switch (appo)
            {
                case 0:
                    break;

                case 1:
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOC_SPEDITI_TIPO.ToString();
                    fV1.valore = "PEC";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    if (this.rb_docSpediti.SelectedIndex == -1 || this.rb_docSpediti.SelectedIndex == 5)
                        docSpediti = rb_docSpediti.Items[4].Value;
                    break;

                case 2:
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOC_SPEDITI_TIPO.ToString();
                    fV1.valore = "PITRE";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    if (this.rb_docSpediti.SelectedIndex == -1 || this.rb_docSpediti.SelectedIndex == 5)
                        docSpediti = rb_docSpediti.Items[4].Value;
                    break;

                case 3:
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOC_SPEDITI_TIPO.ToString();
                    fV1.valore = "ALL";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    if (this.rb_docSpediti.SelectedIndex == -1 || this.rb_docSpediti.SelectedIndex == 5)
                        docSpediti = rb_docSpediti.Items[4].Value;
                    break;
            }
            if (!string.IsNullOrEmpty(txt_dataSpedDa.Text) &&
                  !string.IsNullOrEmpty(txt_dataSpedA.Text) &&
                  !utils.verificaIntervalloDate(txt_dataSpedA.Text, txt_dataSpedDa.Text))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDataSpedizioneInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDataSpedizioneInterval', 'warning', '');};", true);
                return false;
            }
            if (!string.IsNullOrEmpty(this.txt_dataSpedDa.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SPEDIZIONE_DA.ToString();
                fV1.valore = this.txt_dataSpedDa.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                if (this.rb_docSpediti.SelectedIndex == -1 || this.rb_docSpediti.SelectedIndex == 5)
                    docSpediti = rb_docSpediti.Items[4].Value;
                    //this.rb_docSpediti.SelectedIndex = 4;
            }
            if (!string.IsNullOrEmpty(this.txt_dataSpedA.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SPEDIZIONE_A.ToString();
                fV1.valore = this.txt_dataSpedA.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                if (this.rb_docSpediti.SelectedIndex == -1 || this.rb_docSpediti.SelectedIndex == 5)
                    docSpediti = rb_docSpediti.Items[4].Value;
                    //this.rb_docSpediti.SelectedIndex = 4;
            }
            if (this.rb_docSpediti.SelectedIndex != -1 && this.rb_docSpediti.SelectedIndex != 5)
                docSpediti = this.rb_docSpediti.SelectedValue;
            if ((!string.IsNullOrEmpty(docSpediti)))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DOC_SPEDITI.ToString();
                fV1.valore = docSpediti;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.rb_docSpeditiEsito.SelectedIndex != -1 && this.rb_docSpeditiEsito.SelectedIndex != 3)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DOC_SPEDITI_ESITO.ToString();
                fV1.valore = this.rb_docSpeditiEsito.SelectedValue;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region DATA_TIPO_NOTIFICA_TODAY
            if (!string.IsNullOrEmpty(Cal_Da_pec.Text) &&
                  !string.IsNullOrEmpty(Cal_A_pec.Text) &&
                  utils.verificaIntervalloDate(Cal_Da_pec.Text, Cal_A_pec.Text))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDataRicevutaInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDataRicevutaInterval', 'warning', '');};", true);
                return false;
            }
            if (!string.IsNullOrEmpty(this.Cal_Da_pec.Text) &&
                string.IsNullOrEmpty(this.Cal_A_pec.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_TODAY.ToString();
                fV1.valore = this.Cal_Da_pec.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region DATA_TIPO_NOTIFICA_TODAY_PITRE
            if (!string.IsNullOrEmpty(this.Cal_Da_pitre.Text) &&
                string.IsNullOrEmpty(this.Cal_A_pitre.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_TODAY_PITRE.ToString();
                fV1.valore = this.Cal_Da_pitre.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region DATA_TIPO_NOTIFICA_DA - DATA_TIPO_NOTIFICA_A
            if (!string.IsNullOrEmpty(this.Cal_Da_pec.Text) &&
                !string.IsNullOrEmpty(this.Cal_A_pec.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_DA.ToString();
                fV1.valore = this.Cal_Da_pec.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_A.ToString();
                fV1.valore = this.Cal_A_pec.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region DATA_TIPO_NOTIFICA_DA_PITRE - DATA_TIPO_NOTIFICA_A_PITRE
            if (!string.IsNullOrEmpty(this.Cal_Da_pitre.Text) &&
                !string.IsNullOrEmpty(this.Cal_A_pitre.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_DA_PITRE.ToString();
                fV1.valore = this.Cal_Da_pitre.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_A_PITRE.ToString();
                fV1.valore = this.Cal_A_pitre.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region DATA_TIPO_NOTIFICA_NESSUNA
            if (this.p_ricevute_pec.Visible)
            {
                if (this.ddl_ricevute_pec.SelectedIndex != 0)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_NESSUNA.ToString();
                    fV1.valore = ")";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region DATA_TIPO_NOTIFICA_NESSUNA_PITRE
            if (this.p_ricevute_pitre.Visible)
            {
                if (this.ddl_ricevute_pitre.SelectedIndex != 0)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_NESSUNA_PITRE.ToString();
                    fV1.valore = ")";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region CODICE_TIPO_NOTIFICA
            if (this.ddl_ricevute_pec.SelectedIndex != 0)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.CODICE_TIPO_NOTIFICA.ToString();
                fV1.valore = this.ddl_ricevute_pec.SelectedValue;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region CODICE_TIPO_NOTIFICA_PITRE
            if (this.p_ricevute_pitre.Visible && this.ddl_ricevute_pitre.SelectedIndex != 0)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.CODICE_TIPO_NOTIFICA_PITRE.ToString();
                fV1.valore = this.ddl_ricevute_pitre.SelectedValue;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region Filtro documenti consolidati
            DocsPaWR.FiltroRicerca filterItem = null;
            foreach (ListItem itm in this.lstFiltriConsolidamento.Items)
            {
                if (itm.Selected)
                {
                    if (filterItem == null)
                        filterItem = new DocsPaWR.FiltroRicerca { argomento = DocsPaWR.FiltriDocumento.STATO_CONSOLIDAMENTO.ToString() };

                    if (!string.IsNullOrEmpty(filterItem.valore))
                        filterItem.valore += "|";

                    filterItem.valore += itm.Value;
                }
            }
            if (filterItem != null)
                fVList = utils.addToArrayFiltroRicerca(fVList, filterItem);

            //Filtro per ricordarsi il tipo di data selezionata per il consolidamento
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_CONSOLIDAMENTO.ToString();
            fV1.valore = this.cboDataConsolidamento.SelectedValue;
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

            if (!string.IsNullOrEmpty(this.txtDataConsolidamento.Text) && utils.isDate(this.txtDataConsolidamento.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca { argomento = DocsPaWR.FiltriDocumento.DATA_CONSOLIDAMENTO_DA.ToString() };
                fV1.valore = this.txtDataConsolidamento.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (!string.IsNullOrEmpty(this.txtDataConsolidamentoFinale.Text) && utils.isDate(this.txtDataConsolidamentoFinale.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca { argomento = DocsPaWR.FiltriDocumento.DATA_CONSOLIDAMENTO_A.ToString() };
                fV1.valore = this.txtDataConsolidamentoFinale.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (!string.IsNullOrEmpty(this.idUsrConsolidamento.Value))
            {
                string filterName = string.Empty;

                DocsPaWR.Corrispondente temp = new DocsPaWR.Corrispondente();
                if (this.UsrConsolidamentoTypeOfCorrespondent.Value == "P" || (temp.tipoCorrispondente != null && temp.tipoCorrispondente.Equals("P")))
                {
                    filterName = DocsPaWR.FiltriDocumento.ID_UTENTE_CONSOLIDANTE.ToString();
                }
                else
                {
                    if (this.UsrConsolidamentoTypeOfCorrespondent.Value == "R" || (temp.tipoCorrispondente != null && temp.tipoCorrispondente.Equals("R")))
                    {
                        filterName = DocsPaWR.FiltriDocumento.ID_RUOLO_CONSOLIDANTE.ToString();
                    }
                }

                if (!string.IsNullOrEmpty(filterName))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = filterName;
                    fV1.valore = this.idUsrConsolidamento.Value;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion

            #region Visibilità Tipica / Atipica
            if (this.plcVisibility.Visible)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.VISIBILITA_T_A.ToString();
                fV1.valore = this.rblVisibility.SelectedValue;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion Visibilità Tipica / Atipica

            // Integrazione Pitre-PARER
            #region filtro Stato Conservazione
            DocsPaWR.FiltroRicerca fVStatoCons = null;
            foreach (ListItem item in cbl_Conservazione.Items)
            {
                if (item.Selected)
                {
                    if (fVStatoCons == null)
                        fVStatoCons = new DocsPaWR.FiltroRicerca() { argomento = DocsPaWR.FiltriDocumento.STATO_CONSERVAZIONE.ToString() };

                    fVStatoCons.valore += item.Value;
                }
            }
            if(fVStatoCons != null)
                fVList = utils.addToArrayFiltroRicerca(fVList, fVStatoCons);
            #endregion
            #region filtro Data Versamento
            if (this.cbl_Conservazione.Visible)
            {
                switch (ddl_DataVers.SelectedValue)
                {
                    // Valore singolo o oggi
                    // inserisco un solo valore nel filtro
                    case "0":
                    case "2":
                        if (!string.IsNullOrEmpty(this.txt_initDataVers.Text))
                        {
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.DATA_VERSAMENTO_IL.ToString();
                            fV1.valore = this.txt_initDataVers.Text;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        break;
                    // Intervallo, settimana corrente, mese corrente
                    // inserisco una coppia di valori nei filtri
                    case "1":
                    case "3":
                    case "4":
                        if (!string.IsNullOrEmpty(this.txt_initDataVers.Text) && !string.IsNullOrEmpty(this.txt_fineDataVers.Text) &&
                            utils.verificaIntervalloDate(this.txt_initDataVers.Text, this.txt_fineDataVers.Text))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDataVersamento', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDataVersamento', 'warning', '');};", true);
                            return false;
                        }
                        if (!string.IsNullOrEmpty(this.txt_initDataVers.Text))
                        {
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.DATA_VERSAMENTO_DA.ToString();
                            fV1.valore = this.txt_initDataVers.Text;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        if (!string.IsNullOrEmpty(this.txt_fineDataVers.Text))
                        {
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.DATA_VERSAMENTO_A.ToString();
                            fV1.valore = this.txt_fineDataVers.Text;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        break;
                }
            }
            #endregion
            // Policy
            #region filtro codice
            if (!string.IsNullOrEmpty(this.txtCodPolicy.Text))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.POLICY_CODICE.ToString();
                fV1.valore = this.txtCodPolicy.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region filtro numero esecuzione
            if (!string.IsNullOrEmpty(this.txtCounterPolicy.Text))
            {
                // verifico che sia valorizzato il filtro sul codice policy
                if (string.IsNullOrEmpty(this.txtCodPolicy.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterCounterPolicy', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterCounterPolicy', 'warning', '');};", true);
                    return false;
                }
                else
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.POLICY_NUM_ESECUZIONE.ToString();
                    fV1.valore = this.txtCounterPolicy.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion
            #region filtro data esecuzione
            switch (this.ddl_datePolicy.SelectedValue)
            {
                // Valore Singolo, Oggi
                case "0":
                case "2":
                    if (!string.IsNullOrEmpty(this.txt_initDatePolicy.Text))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_EXEC_POLICY_IL.ToString();
                        fV1.valore = this.txt_initDatePolicy.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    break;
                // Intervallo, Settimana Corrente, Mese Corrente
                case "1":
                case "3":
                case "4":
                    if (!string.IsNullOrEmpty(this.txt_initDatePolicy.Text) && !string.IsNullOrEmpty(this.txt_fineDatePolicy.Text) &&
                            utils.verificaIntervalloDate(this.txt_initDatePolicy.Text, this.txt_fineDatePolicy.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDataPolicy', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchDocumentAdvancedFilterDataPolicy', 'warning', '');};", true);
                        return false;
                    }
                    if (!string.IsNullOrEmpty(this.txt_initDatePolicy.Text))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_EXEC_POLICY_DA.ToString();
                        fV1.valore = this.txt_initDatePolicy.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!string.IsNullOrEmpty(this.txt_fineDatePolicy.Text))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_EXEC_POLICY_A.ToString();
                        fV1.valore = this.txt_fineDatePolicy.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    break;
                case "5":
                    // siamo nel caso di Ieri
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_EXEC_POLICY_YESTERDAY.ToString();
                    fV1.valore = this.txt_initDatePolicy.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    break;
            }
            #endregion

            //nuovo filtro per prendere solo i documenti protocollati

            #region filtro riferimento
            //DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            //if (wws.isEnableRiferimentiMittente() && !string.IsNullOrEmpty(txt_rif_mittente.Text))
            //{
            //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
            //    fV1.argomento = DocsPaWR.FiltriDocumento.RIFERIMENTO_MITTENTE.ToString();
            //    fV1.valore = txt_rif_mittente.Text;
            //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

            //}
            #endregion

            //ABBATANGELI GIANLUIGI - Filtro per nascondere doc di altre applicazioni
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.FILTRO_APPLICAZIONE.ToString()]))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.COD_EXT_APP.ToString();
                fV1.valore = (System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.FILTRO_APPLICAZIONE.ToString()]);
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            #region DA PROTOCOLLARE
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.DA_PROTOCOLLARE.ToString();
            fV1.valore = "0";  //corrisponde a 'false'
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            #endregion
            #region codice nome amministrazione
            if (!this.txt_codDesc.Text.Equals(""))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.CODICE_DESCRIZIONE_AMMINISTRAZIONE.ToString();
                fV1.valore = this.txt_codDesc.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion

            #region Filtro campi profilati
            if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
            {
                this.SaveTemplateDocument();
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.PROFILAZIONE_DINAMICA.ToString();
                fV1.template = this.Template;
                fV1.valore = "Profilazione Dinamica";
                if (this.Template != null && this.Template.ELENCO_OGGETTI != null && this.Template.ELENCO_OGGETTI.Length > 0)
                {
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString();
                fV1.valore = this.DocumentDdlTypeDocument.SelectedItem.Value;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            
            #region filtro DIAGRAMMI DI STATO
            if (this.DocumentDdlStateDiagram.Visible && this.DocumentDdlStateDiagram.SelectedIndex != 0)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DIAGRAMMA_STATO_DOC.ToString();
                fV1.nomeCampo = this.ddlStateCondition.SelectedValue;
                fV1.valore = this.DocumentDdlStateDiagram.SelectedValue;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion filtro DIAGRAMMI DI STATO

            #region FILTRO MAI SPEDITI
            /*
            if (this.cbx_mai_spediti.Checked)
            {
                //Con il filtro mai spediti è obbligatorio inserire data o intervallo di creazione
                if (string.IsNullOrEmpty(txt_initDataCreazione_E.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningCreationDateRequiredNeverSent', 'warning', '');} else {parent.ajaxDialogModal('WarningCreationDateRequiredNeverSent', 'warning', '');};", true);
                    return false;
                }

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DOC_NON_SPEDITI.ToString();
                fV1.valore = this.cbx_mai_spediti.Checked.ToString();
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            */
            #endregion

            #region Ordinamento
            List<FiltroRicerca> filterList = GridManager.GetOrderFilter();

            // Se la lista è valorizzata vengono aggiunti i filtri
            if (filterList != null)
            {
                foreach (FiltroRicerca filter in filterList)
                {
                    fVList = utils.addToArrayFiltroRicerca(fVList, filter);
                }
            }

            #endregion

            qV[0] = fVList;
            this.SearchFilters = qV;
            return true;
            //}
            //catch (System.Exception ex)
            //{
            //    ErrorManager.redirect(this, ex);
            //    return false;
            //}
        }

        private string getInStringChild(DocsPaWR.Folder folder, string inSubFolder)
        {
            if (folder.childs != null && folder.childs.Length > 0)
            {
                for (int i = 0; i < folder.childs.Length; i++)
                {
                    inSubFolder += ", " + folder.childs[i].systemID;
                    inSubFolder = getInStringChild(folder.childs[i], inSubFolder);
                }
            }
            return inSubFolder;
        }

        private void SaveTemplateDocument()
        {
            int result = 0;
            for (int i = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i];
                if (controllaCampi(oggettoCustom, oggettoCustom.SYSTEM_ID.ToString()))
                {
                    result++;
                }
                //if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("Data"))
                //{
                //    try
                //    {
                //        UserControls.Calendar dataDa = (UserControls.Calendar)panel_Contenuto.FindControl("da_" + oggettoCustom.SYSTEM_ID.ToString());
                //        //if (dataDa.txt_Data.Text != null && dataDa.txt_Data.Text != "")
                //        if (dataDa.Text != null && dataDa.Text != "")
                //        {
                //            //DateTime dataAppoggio = Convert.ToDateTime(dataDa.txt_Data.Text);
                //            DateTime dataAppoggio = Convert.ToDateTime(dataDa.Text);
                //        }
                //        UserControls.Calendar dataA = (UserControls.Calendar)panel_Contenuto.FindControl("a_" + oggettoCustom.SYSTEM_ID.ToString());
                //        //if (dataA.txt_Data.Text != null && dataA.txt_Data.Text != "")
                //        if (dataA.Text != null && dataA.Text != "")
                //        {
                //            //DateTime dataAppoggio = Convert.ToDateTime(dataA.txt_Data.Text);
                //            DateTime dataAppoggio = Convert.ToDateTime(dataA.Text);
                //        }
                //    }
                //    catch (Exception)
                //    {
                //        Label_Avviso.Text = "Inserire valori validi per il campo data !";
                //        Label_Avviso.Visible = true;
                //        return;
                //    }
                //}
            }
            //if (result == this.Template.ELENCO_OGGETTI.Length)
            //{
            //    Label_Avviso.Text = "Inserire almeno un criterio di ricerca !";
            //    Label_Avviso.Visible = true;
            //    return;
            //}
        }

        private bool controllaCampi(DocsPaWR.OggettoCustom oggettoCustom, string idOggetto)
        {
            //In questo metodo, oltre al controllo si salvano i valori dei campi inseriti 
            //dall'utente nel template in sessione. Solo successivamente, quanto verra' salvato 
            //il documento i suddetti valori verranno riportai nel Db vedi metodo "btn_salva_Click" della "docProfilo.aspx"

            //Label_Avviso.Visible = false;
            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
            {
                case "CampoDiTesto":
                    CustomTextArea textBox = (CustomTextArea)PnlTypeDocument.FindControl(idOggetto);
                    if (textBox != null)
                    {
                        if (string.IsNullOrEmpty(textBox.Text))
                        {
                            //SetFocus(textBox);
                            oggettoCustom.VALORE_DATABASE = textBox.Text;
                            return true;
                        }
                        oggettoCustom.VALORE_DATABASE = textBox.Text;
                    }
                    break;
                case "CasellaDiSelezione":
                    CheckBoxList checkBox = (CheckBoxList)PnlTypeDocument.FindControl(idOggetto);
                    if (checkBox != null)
                    {
                        if (checkBox.SelectedIndex == -1)
                        {
                            //SetFocus(checkBox);
                            for (int i = 0; i < oggettoCustom.VALORI_SELEZIONATI.Length; i++)
                                oggettoCustom.VALORI_SELEZIONATI[i] = null;

                            return true;
                        }

                        oggettoCustom.VALORI_SELEZIONATI = new string[checkBox.Items.Count];
                        oggettoCustom.VALORE_DATABASE = "";
                        for (int i = 0; i < checkBox.Items.Count; i++)
                        {
                            if (checkBox.Items[i].Selected)
                            {
                                oggettoCustom.VALORI_SELEZIONATI[i] = checkBox.Items[i].Text;
                            }
                        }
                    }
                    break;
                case "MenuATendina":
                    DropDownList dropDwonList = (DropDownList)PnlTypeDocument.FindControl(idOggetto);
                    if (dropDwonList != null)
                    {
                        if (dropDwonList.SelectedItem.Text.Equals(""))
                        {
                            //SetFocus(dropDwonList);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }
                        oggettoCustom.VALORE_DATABASE = dropDwonList.SelectedItem.Text;
                    }
                    break;
                case "SelezioneEsclusiva":
                    RadioButtonList radioButtonList = (RadioButtonList)PnlTypeDocument.FindControl(idOggetto);
                    if (radioButtonList != null)
                    {
                        if (oggettoCustom.VALORE_DATABASE == "-1" || radioButtonList.SelectedIndex == -1 || radioButtonList.SelectedValue == "-1")
                        {
                            //SetFocus(radioButtonList);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }
                        oggettoCustom.VALORE_DATABASE = radioButtonList.SelectedItem.Text;
                    }
                    break;
                case "Data":
                    UserControls.Calendar dataDa = (UserControls.Calendar)PnlTypeDocument.FindControl("da_" + oggettoCustom.SYSTEM_ID.ToString());
                    UserControls.Calendar dataA = (UserControls.Calendar)PnlTypeDocument.FindControl("a_" + oggettoCustom.SYSTEM_ID.ToString());

                    if (dataDa != null && dataA != null)
                    {
                        if (dataDa.Text.Equals("") && dataA.Text.Equals(""))
                        {
                            //SetFocus(dataDa.txt_Data);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }

                        if (dataDa.Text.Equals("") && dataA.Text != "")
                        {
                            //SetFocus(dataDa.txt_Data);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }

                        if (dataDa.Text != "" && dataA.Text != "")
                            //oggettoCustom.VALORE_DATABASE = dataDa.txt_Data.Text + "@" + dataA.txt_Data.Text;
                            oggettoCustom.VALORE_DATABASE = dataDa.Text + "@" + dataA.Text;

                        if (dataDa.Text != "" && dataA.Text == "")
                            //oggettoCustom.VALORE_DATABASE = dataDa.txt_Data.Text;
                            oggettoCustom.VALORE_DATABASE = dataDa.Text;
                    }

                    break;
                case "Contatore":
                    CustomTextArea contatoreDa = (CustomTextArea)PnlTypeDocument.FindControl("da_" + idOggetto);
                    CustomTextArea contatoreA = (CustomTextArea)PnlTypeDocument.FindControl("a_" + idOggetto);
                    //Controllo la valorizzazione di campi ed eventualmente notifico gli errori
                    
                    CustomTextArea dataRepertorioDa = (CustomTextArea)this.PnlTypeDocument.FindControl("TxtDataRepertorioDa_" + idOggetto);
                    CustomTextArea dataRepertorioA = (CustomTextArea)this.PnlTypeDocument.FindControl("TxtDataRepertorioA_" + idOggetto);


                     if (dataRepertorioDa != null && dataRepertorioA != null)
                     {
                         if (dataRepertorioDa.Text != "" && dataRepertorioA.Text != "")
                             oggettoCustom.DATA_INSERIMENTO = dataRepertorioDa.Text + "@" + dataRepertorioA.Text;

                         if (dataRepertorioDa.Text != "" && dataRepertorioA.Text == "")
                             oggettoCustom.DATA_INSERIMENTO = dataRepertorioDa.Text;
                     }
                    switch (oggettoCustom.TIPO_CONTATORE)
                    {
                        case "T":
                            if (contatoreDa != null && contatoreA != null && contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }
                            break;
                        case "A":
                            DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (contatoreDa != null && ddlAoo != null && contatoreDa.Text.Equals("") && ddlAoo.SelectedValue.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }

                            if (contatoreDa != null && contatoreA != null && contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                                oggettoCustom.VALORE_DATABASE = "";
                            break;
                        case "R":
                            DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (contatoreDa != null && ddlRf != null && contatoreDa.Text.Equals("") && ddlRf.SelectedValue.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                oggettoCustom.ID_AOO_RF = "";
                                return true;
                            }

                            if (contatoreDa != null && contatoreA != null && contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                                oggettoCustom.VALORE_DATABASE = "";
                            break;
                    }

                    if (contatoreDa != null && contatoreA != null && contatoreDa.Text.Equals("") && contatoreA.Text != "")
                    {
                        //SetFocus(contatoreDa);
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }

                    try
                    {
                        if (contatoreDa != null && contatoreDa.Text != null && contatoreDa.Text != "")
                            Convert.ToInt32(contatoreDa.Text);
                        if (contatoreA != null && contatoreA.Text != null && contatoreA.Text != "")
                            Convert.ToInt32(contatoreA.Text);
                    }
                    catch (Exception ex)
                    {
                        //SetFocus(contatoreDa);
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }


                    //I campi sono valorizzati correttamente procedo
                    if (contatoreDa != null && contatoreA != null)
                    {
                        if (contatoreDa.Text != "" && contatoreA.Text != "")
                            oggettoCustom.VALORE_DATABASE = contatoreDa.Text + "@" + contatoreA.Text;

                        if (contatoreDa.Text != "" && contatoreA.Text == "")
                            oggettoCustom.VALORE_DATABASE = contatoreDa.Text;
                    }

                    switch (oggettoCustom.TIPO_CONTATORE)
                    {
                        case "A":
                            DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (ddlAoo != null)
                                oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
                            break;
                        case "R":
                            DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (ddlRf != null)
                                oggettoCustom.ID_AOO_RF = ddlRf.SelectedValue;
                            break;
                    }
                    break;
                case "Corrispondente":
                    UserControls.CorrespondentCustom corr = (UserControls.CorrespondentCustom)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    DocsPaWR.Corrispondente corrispondente = new DocsPaWR.Corrispondente();

                    if (corr != null)
                    {
                        // 1 - Ambedue i campi del corrispondente non sono valorizzati
                        if (string.IsNullOrEmpty(corr.TxtCodeCorrespondentCustom) && string.IsNullOrEmpty(corr.TxtDescriptionCorrespondentCustom))
                        {
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                            return true;
                        }
                        // 2 - E' stato valorizzato solo il campo descrizione del corrispondente
                        if (string.IsNullOrEmpty(corr.TxtCodeCorrespondentCustom) && !string.IsNullOrEmpty(corr.TxtDescriptionCorrespondentCustom))
                        {
                            oggettoCustom.VALORE_DATABASE = corr.TxtDescriptionCorrespondentCustom;
                        }
                        // 3 - E' valorizzato il campo codice del corrispondente
                        if (!string.IsNullOrEmpty(corr.TxtCodeCorrespondentCustom))
                        {
                            //Cerco il corrispondente
                            if (!string.IsNullOrEmpty(corr.IdCorrespondentCustom))
                                corrispondente = UIManager.AddressBookManager.getCorrispondenteBySystemIDDisabled(corr.IdCorrespondentCustom);
                            else
                                corrispondente = UIManager.AddressBookManager.getCorrispondenteByCodRubrica(corr.TxtCodeCorrespondentCustom, false);

                            //corrispondente = UserManager.getCorrispondenteByCodRubrica(this, corr.CODICE_TEXT);
                            // 3.1 - Corrispondente trovato per codice
                            if (corrispondente != null)
                            {
                                oggettoCustom.VALORE_DATABASE = corrispondente.systemId;
                                oggettoCustom.ESTENDI_STORICIZZATI = corr.ChkStoryCustomCorrespondentCustom;
                            }
                            // 3.2 - Corrispondente non trovato per codice
                            else
                            {
                                // 3.2.1 - Campo descrizione non valorizzato
                                if (string.IsNullOrEmpty(corr.TxtDescriptionCorrespondentCustom))
                                {
                                    oggettoCustom.VALORE_DATABASE = string.Empty;
                                    return true;
                                }
                                // 3.2.2 - Campo descrizione valorizzato
                                else
                                    oggettoCustom.VALORE_DATABASE = corr.TxtDescriptionCorrespondentCustom;
                            }
                        }
                    }
                    break;
                case "ContatoreSottocontatore":
                    //TextBox contatoreSDa = (TextBox)PnlTypeDocument.FindControl("da_" + idOggetto);
                    //TextBox contatoreSA = (TextBox)PnlTypeDocument.FindControl("a_" + idOggetto);
                    //TextBox sottocontatoreDa = (TextBox)PnlTypeDocument.FindControl("da_sottocontatore_" + idOggetto);
                    //TextBox sottocontatoreA = (TextBox)PnlTypeDocument.FindControl("a_sottocontatore_" + idOggetto);
                    //UserControls.Calendar dataSottocontatoreDa = (UserControls.Calendar)PnlTypeDocument.FindControl("da_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString());
                    //UserControls.Calendar dataSottocontatoreA = (UserControls.Calendar)PnlTypeDocument.FindControl("a_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString());

                    ////Controllo la valorizzazione di campi ed eventualmente notifico gli errori
                    //switch (oggettoCustom.TIPO_CONTATORE)
                    //{
                    //    case "T":
                    //        if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals("") &&
                    //            sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals("") &&
                    //            dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals("")
                    //            )
                    //        {
                    //            //SetFocus(contatoreDa);
                    //            oggettoCustom.VALORE_DATABASE = "";
                    //            oggettoCustom.VALORE_SOTTOCONTATORE = "";
                    //            oggettoCustom.DATA_INSERIMENTO = "";
                    //            return true;
                    //        }
                    //        break;
                    //    case "A":
                    //        DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                    //        if (contatoreSDa.Text.Equals("") && sottocontatoreDa.Text.Equals("") && ddlAoo.SelectedValue.Equals(""))
                    //        {
                    //            //SetFocus(contatoreDa);
                    //            oggettoCustom.VALORE_DATABASE = "";
                    //            oggettoCustom.VALORE_SOTTOCONTATORE = "";
                    //            return true;
                    //        }

                    //        if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals(""))
                    //            oggettoCustom.VALORE_DATABASE = "";

                    //        if (sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals(""))
                    //            oggettoCustom.VALORE_SOTTOCONTATORE = "";

                    //        if (dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals(""))
                    //            oggettoCustom.DATA_INSERIMENTO = "";
                    //        break;
                    //    case "R":
                    //        DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                    //        if (contatoreSDa.Text.Equals("") && sottocontatoreDa.Text.Equals("") && ddlRf.SelectedValue.Equals(""))
                    //        {
                    //            //SetFocus(contatoreDa);
                    //            oggettoCustom.VALORE_DATABASE = "";
                    //            oggettoCustom.VALORE_SOTTOCONTATORE = "";
                    //            return true;
                    //        }

                    //        if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals(""))
                    //            oggettoCustom.VALORE_DATABASE = "";

                    //        if (sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals(""))
                    //            oggettoCustom.VALORE_SOTTOCONTATORE = "";

                    //        if (dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals(""))
                    //            oggettoCustom.DATA_INSERIMENTO = "";
                    //        break;
                    //}

                    //if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals("") &&
                    //    sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals("") &&
                    //    dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals("")
                    //    )
                    //{
                    //    //SetFocus(contatoreDa);
                    //    oggettoCustom.VALORE_DATABASE = "";
                    //    oggettoCustom.VALORE_SOTTOCONTATORE = "";
                    //    oggettoCustom.DATA_INSERIMENTO = "";
                    //    return true;
                    //}

                    //if (contatoreSDa.Text != null && contatoreSDa.Text != "")
                    //    Convert.ToInt32(contatoreSDa.Text);
                    //if (contatoreSA.Text != null && contatoreSA.Text != "")
                    //    Convert.ToInt32(contatoreSA.Text);
                    //if (sottocontatoreDa.Text != null && sottocontatoreDa.Text != "")
                    //    Convert.ToInt32(sottocontatoreDa.Text);
                    //if (sottocontatoreA.Text != null && sottocontatoreA.Text != "")
                    //    Convert.ToInt32(sottocontatoreA.Text);


                    ////I campi sono valorizzati correttamente procedo
                    //if (contatoreSDa.Text != "" && contatoreSA.Text != "")
                    //    oggettoCustom.VALORE_DATABASE = contatoreSDa.Text + "@" + contatoreSA.Text;

                    //if (contatoreSDa.Text != "" && contatoreSA.Text == "")
                    //    oggettoCustom.VALORE_DATABASE = contatoreSDa.Text;

                    //if (sottocontatoreDa.Text != "" && sottocontatoreA.Text != "")
                    //    oggettoCustom.VALORE_SOTTOCONTATORE = sottocontatoreDa.Text + "@" + sottocontatoreA.Text;

                    //if (sottocontatoreDa.Text != "" && sottocontatoreA.Text == "")
                    //    oggettoCustom.VALORE_SOTTOCONTATORE = sottocontatoreDa.Text;

                    //if (dataSottocontatoreDa.Text != "" && dataSottocontatoreA.Text != "")
                    //    oggettoCustom.DATA_INSERIMENTO = dataSottocontatoreDa.Text + "@" + dataSottocontatoreA.Text;

                    //if (dataSottocontatoreDa.Text != "" && dataSottocontatoreA.Text == "")
                    //    oggettoCustom.DATA_INSERIMENTO = dataSottocontatoreDa.Text;

                    //switch (oggettoCustom.TIPO_CONTATORE)
                    //{
                    //    case "A":
                    //        DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                    //        oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
                    //        break;
                    //    case "R":
                    //        DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                    //        oggettoCustom.ID_AOO_RF = ddlRf.SelectedValue;
                    //        break;
                    //}
                    break;


            }
            return false;
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
                    corr = null;
                    multiCorr = true;
                    this.FoundCorr = listaCorr;
                    this.IdCustomObjectCustomCorrespondent = idControl;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chooseCorrespondent", "ajaxModalPopupChooseCorrespondent();", true);
                }
            }

            if (corr == null)
            {
                switch (idControl)
                {
                    case "txtCodiceCreatore":
                        this.txtCodiceCreatore.Text = string.Empty;
                        this.txtDescrizioneCreatore.Text = string.Empty;
                        this.idCreatore.Value = string.Empty;
                        this.upPnlCreatore.Update();
                        break;
                    case "txtCodiceProprietario":
                        this.txtCodiceProprietario.Text = string.Empty;
                        this.txtDescrizioneProprietario.Text = string.Empty;
                        this.idProprietario.Value = string.Empty;
                        this.upPnlProprietario.Update();
                        break;
                    case "txt_codMittInter_C":
                        this.txt_codMittInter_C.Text = string.Empty;
                        this.txt_descrMittInter_C.Text = string.Empty;
                        this.idMittItermedio.Value = string.Empty;
                        this.UpMittInter.Update();
                        break;
                    case "txt_codMit_E":
                        this.txt_codMit_E.Text = string.Empty;
                        this.txt_descrMit_E.Text = string.Empty;
                        this.IdRecipient.Value = string.Empty;
                        this.RecipientTypeOfCorrespondent.Value = string.Empty;
                        this.UpProtocollo.Update();
                        break;
                    case "txt_codUsrConsolidamento":
                        this.txt_codUsrConsolidamento.Text = string.Empty;
                        this.txt_descrUsrConsolidamento.Text = string.Empty;
                        this.idUsrConsolidamento.Value = string.Empty;
                        this.UsrConsolidamentoTypeOfCorrespondent.Value = string.Empty;
                        this.UpStatoConsolidamento.Update();
                        break;
                    case "txtCodiceFirmatario":
                        this.txtCodiceFirmatario.Text = string.Empty;
                        this.txtDescrizioneFirmatario.Text = string.Empty;
                        this.idFirmatario.Value = string.Empty;
                        this.UpPnlFirmatario.Update();
                        break;
                }
                if (!multiCorr)
                {
                    string msg = "ErrorTransmissionCorrespondentNotFound";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                }
            }
            else
            {
                switch (idControl)
                {
                    case "txtCodiceCreatore":
                        this.txtCodiceCreatore.Text = corr.codiceRubrica;
                        this.txtDescrizioneCreatore.Text = corr.descrizione;
                        this.idCreatore.Value = corr.systemId;
                        this.rblOwnerType.SelectedIndex = -1;
                        this.rblOwnerType.Items.FindByValue(corr.tipoCorrispondente).Selected = true;
                        this.upPnlCreatore.Update();
                        break;
                    case "txtCodiceProprietario":
                        this.txtCodiceProprietario.Text = corr.codiceRubrica;
                        this.txtDescrizioneProprietario.Text = corr.descrizione;
                        this.idProprietario.Value = corr.systemId;
                        this.rblProprietarioType.SelectedIndex = -1;
                        this.rblProprietarioType.Items.FindByValue(corr.tipoCorrispondente).Selected = true;
                        this.upPnlProprietario.Update();
                        break;
                    case "txt_codMittInter_C":
                        this.txt_codMittInter_C.Text = corr.codiceRubrica;
                        this.txt_descrMittInter_C.Text = corr.descrizione;
                        this.idMittItermedio.Value = corr.systemId;
                        this.UpMittInter.Update();
                        break;
                    case "txt_codMit_E":
                        this.txt_codMit_E.Text = corr.codiceRubrica;
                        this.txt_descrMit_E.Text = corr.descrizione;
                        this.IdRecipient.Value = corr.systemId;
                        this.UpProtocollo.Update();
                        break;
                    case "txt_codUsrConsolidamento":
                        this.txt_codUsrConsolidamento.Text = corr.codiceRubrica;
                        this.txt_descrUsrConsolidamento.Text = corr.descrizione;
                        this.idUsrConsolidamento.Value = corr.systemId;
                        this.UsrConsolidamentoTypeOfCorrespondent.Value = corr.tipoCorrispondente;
                        this.UpStatoConsolidamento.Update();
                        break;
                    case "txtCodiceFirmatario":
                        this.txtCodiceFirmatario.Text = corr.codiceRubrica;
                        this.txtDescrizioneFirmatario.Text = corr.descrizione;
                        this.idFirmatario.Value = corr.systemId;
                        this.rblFirmatarioType.SelectedIndex = -1;
                        this.rblFirmatarioType.Items.FindByValue(corr.tipoCorrispondente).Selected = true;
                        this.chkFirmaElettronica.Checked = true;
                        this.UpPnlElectronicSignature.Update();
                        break;
                }
            }

            #region OLD CODE

            //RubricaCallType calltype = GetCallType(idControl);
            //Corrispondente corr = UIManager.AddressBookManager.getCorrispondenteRubrica(addressCode, calltype);

            //if (corr == null)
            //{
            //    switch (idControl)
            //    {
            //        case "txtCodiceCreatore":
            //            this.txtCodiceCreatore.Text = string.Empty;
            //            this.txtDescrizioneCreatore.Text = string.Empty;
            //            this.idCreatore.Value = string.Empty;
            //            this.upPnlCreatore.Update();
            //            break;
            //        case "txtCodiceProprietario":
            //            this.txtCodiceProprietario.Text = string.Empty;
            //            this.txtDescrizioneProprietario.Text = string.Empty;
            //            this.idProprietario.Value = string.Empty;
            //            this.upPnlProprietario.Update();
            //            break;
            //        case "txt_codMittInter_C":
            //            this.txt_codMittInter_C.Text = string.Empty;
            //            this.txt_descrMittInter_C.Text = string.Empty;
            //            this.idMittItermedio.Value = string.Empty;
            //            this.UpMittInter.Update();
            //            break;
            //        case "txt_codMit_E":
            //            this.txt_codMit_E.Text = string.Empty;
            //            this.txt_descrMit_E.Text = string.Empty;
            //            this.IdRecipient.Value = string.Empty;
            //            this.RecipientTypeOfCorrespondent.Value = string.Empty;
            //            this.UpProtocollo.Update();
            //            break;
            //        case "txt_codUsrConsolidamento":
            //            this.txt_codUsrConsolidamento.Text = string.Empty;
            //            this.txt_descrUsrConsolidamento.Text = string.Empty;
            //            this.idUsrConsolidamento.Value = string.Empty;
            //            this.UsrConsolidamentoTypeOfCorrespondent.Value = string.Empty;
            //            this.UpStatoConsolidamento.Update();
            //            break;
            //    }

            //    string msg = "ErrorTransmissionCorrespondentNotFound";
            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
            //}
            //else
            //{
            //    switch (idControl)
            //    {
            //        case "txtCodiceCreatore":
            //            this.txtCodiceCreatore.Text = corr.codiceRubrica;
            //            this.txtDescrizioneCreatore.Text = corr.descrizione;
            //            this.idCreatore.Value = corr.systemId;
            //            this.rblOwnerType.SelectedIndex = -1;
            //            this.rblOwnerType.Items.FindByValue(corr.tipoCorrispondente).Selected = true;
            //            this.upPnlCreatore.Update();
            //            break;
            //        case "txtCodiceProprietario":
            //            this.txtCodiceProprietario.Text = corr.codiceRubrica;
            //            this.txtDescrizioneProprietario.Text = corr.descrizione;
            //            this.idProprietario.Value = corr.systemId;
            //            this.rblProprietarioType.SelectedIndex = -1;
            //            this.rblProprietarioType.Items.FindByValue(corr.tipoCorrispondente).Selected = true;
            //            this.upPnlProprietario.Update();
            //            break;
            //        case "txt_codMittInter_C":
            //            this.txt_codMittInter_C.Text = corr.codiceRubrica;
            //            this.txt_descrMittInter_C.Text = corr.descrizione;
            //            this.idMittItermedio.Value = corr.systemId;
            //            this.UpMittInter.Update();
            //            break;
            //        case "txt_codMit_E":
            //            this.txt_codMit_E.Text = corr.codiceRubrica;
            //            this.txt_descrMit_E.Text = corr.descrizione;
            //            this.IdRecipient.Value = corr.systemId;
            //            this.UpProtocollo.Update();
            //            break;
            //        case "txt_codUsrConsolidamento":
            //            this.txt_codUsrConsolidamento.Text = corr.codiceRubrica;
            //            this.txt_descrUsrConsolidamento.Text = corr.descrizione;
            //            this.idUsrConsolidamento.Value = corr.systemId;
            //            this.UsrConsolidamentoTypeOfCorrespondent.Value = corr.tipoCorrispondente;
            //            this.UpStatoConsolidamento.Update();
            //            break;
            //    }
            //}

            #endregion

            this.UpProtocollo.Update();
        }

        public void PopulateDDLSavedSearches()
        {
            if (this.IsAdl)
            {
                schedaRicerca.ElencoRicercheADL("D", false, this.DdlRapidSearch, null);
            }
            else
            {
                schedaRicerca.ElencoRicerche("D", true, this.DdlRapidSearch);
            }

            if (Session["idRicercaSalvata"] != null)
            {
                Session["itemUsedSearch"] = this.DdlRapidSearch.Items.IndexOf(this.DdlRapidSearch.Items.FindByValue(Session["idRicercaSalvata"].ToString()));
                Session["idRicercaSalvata"] = null;
            }

            this.BindFilterValues(schedaRicerca, false);
        }

        /// <summary>
        /// Ripristino valori filtri di ricerca nei campi della UI
        /// </summary>
        /// <param name="schedaRicerca"></param>
        private void BindFilterValues(SearchManager schedaRicerca, bool grid)
        {
            if (schedaRicerca.FiltriRicerca != null && schedaRicerca.FiltriRicerca.Length > 0)
            {
                try
                {

                    if (this.Session["itemUsedSearch"] != null)
                    {
                        this.DdlRapidSearch.SelectedIndex = Convert.ToInt32(this.Session["itemUsedSearch"]);
                        this.UpPnlRapidSearch.Update();
                    }
                    else if (this.Session["idRicercaSalvata"] != null)
                    {
                        this.DdlRapidSearch.Items.FindByValue(this.Session["idRicercaSalvata"].ToString()).Selected = true;
                        this.UpPnlRapidSearch.Update();
                    }

                    try
                    {
                        foreach (DocsPaWR.FiltroRicerca item in schedaRicerca.FiltriRicerca[0])
                        {
                            #region PROTOCOLLO_ARRIVO
                            if (item.argomento == DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString())
                            {
                                this.cbl_archDoc_E.Items.FindByValue("A").Selected = Convert.ToBoolean(item.valore);
                            }
                            #endregion
                            #region PROTOCOLLO_PARTENZA
                            else if (item.argomento == DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString())
                            {
                                this.cbl_archDoc_E.Items.FindByValue("P").Selected = Convert.ToBoolean(item.valore);
                            }
                            #endregion
                            #region PROTOCOLLO_INTERNO

                            else if (item.argomento == DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString())
                            {
                                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString())) && !Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString()).Equals("1"))
                                {
                                    this.opInt.Attributes.CssStyle.Add("display", "none");
                                    this.opInt.Selected = false;
                                }
                                else if (this.cbl_archDoc_E.Items.FindByValue("I") != null)
                                    this.cbl_archDoc_E.Items.FindByValue("I").Selected = Convert.ToBoolean(item.valore);
                            }
                            #endregion
                            #region GRIGI
                            else if (item.argomento == DocsPaWR.FiltriDocumento.GRIGIO.ToString())
                            {
                                this.cbl_archDoc_E.Items.FindByValue("G").Selected = Convert.ToBoolean(item.valore);
                            }
                            #endregion
                            #region PREDISPOSTI
                            else if (item.argomento == DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString())
                            {
                                this.cbl_archDoc_E.Items.FindByValue("Pr").Selected = Convert.ToBoolean(item.valore);
                            }
                            #endregion
                            #region ALLEGATI
                            else if (item.argomento == DocsPaWR.FiltriDocumento.ALLEGATO.ToString())
                            {
                                this.cbl_archDoc_E.Items.FindByValue("ALL").Selected = true; //Convert.ToBoolean(aux.valore);
                                // INC000000589215
                                // Tipo allegati in ricerche salvate
                                if (item.valore != null)
                                {
                                    this.rblFiltriAllegati.SelectedValue = item.valore;

                                    if (this.EnabledSearchAttachByDescMainDoc)
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "enableField", "enableFieldPnlObjectAttach();", true);
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "enableField", "enableField();", true);
                                    }

                                }
                            }
                            #endregion
                            #region REGISTRO
                            else if (item.argomento == DocsPaWR.FiltriDocumento.REGISTRO.ToString())
                            {
                                char[] sep = { ',' };
                                string[] regs = item.valore.Split(sep);
                                foreach (ListItem li in this.lb_reg_C.Items)
                                    li.Selected = false;
                                foreach (string reg in regs)
                                {
                                    for (int i = 0; i < this.lb_reg_C.Items.Count; i++)
                                    {
                                        if (this.lb_reg_C.Items[i].Value == reg)
                                            this.lb_reg_C.Items[i].Selected = true;
                                    }
                                }
                            }
                            #endregion REGISTRO
                            #region REPERTORIATI
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DOC_REPERTORIATO.ToString())
                            {
                                this.cbl_archDoc_E.Items.FindByValue("REP").Selected = Convert.ToBoolean(item.valore);
                                if (this.cbl_archDoc_E.Items.FindByValue("REP").Selected)
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "enableFiltersRep", "enableFiltersRep();", true);
                                }
                            }
                            #endregion
                            #region NUMERO REPERTORIO
                            else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_REPERTORIO.ToString())
                            {
                                if (this.ddl_numRep.SelectedIndex != 0)
                                    this.ddl_numRep.SelectedIndex = 0;
                                this.ddl_numRep_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initNumRep.Text = item.valore;
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_REPERTORIO_DAL.ToString())
                            {
                                if (this.ddl_numRep.SelectedIndex != 1)
                                    this.ddl_numRep.SelectedIndex = 1;
                                this.ddl_numRep_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initNumRep.Text = item.valore;
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_REPERTORIO_AL.ToString())
                            {
                                if (this.ddl_numRep.SelectedIndex != 1)
                                    this.ddl_numRep.SelectedIndex = 1;
                                this.ddl_numRep_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_fineNumRep.Text = item.valore;
                            }
                            #endregion
                            #region DATA REPERTORIO
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_REPERTORIO_IL.ToString())
                            {
                                if (this.ddl_dataRepertorio.SelectedIndex != 0)
                                    ddl_dataRepertorio.SelectedIndex = 0;
                                ddl_dataRepertorio_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initDataRep.Visible = true;
                                this.txt_initDataRep.Visible = true;
                                this.txt_initDataRep.Text = item.valore;
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_REPERTORIO_SUCCESSIVA_AL.ToString())
                            {
                                if (ddl_dataRepertorio.SelectedIndex != 1)
                                    ddl_dataRepertorio.SelectedIndex = 1;
                                ddl_dataRepertorio_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initDataRep.Visible = true;
                                this.txt_initDataRep.Visible = true;
                                this.txt_initDataRep.Text = item.valore;
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_REPERTORIO_PRECEDENTE_IL.ToString())
                            {
                                if (this.ddl_dataRepertorio.SelectedIndex != 1)
                                    this.ddl_dataRepertorio.SelectedIndex = 1;
                                this.ddl_dataRepertorio_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_fineDataRep.Visible = true;
                                this.txt_fineDataRep.Visible = true;
                                this.txt_fineDataRep.Text = item.valore;
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_REPERTORIO_SC.ToString() && item.valore == "1")
                            {
                                this.ddl_dataRepertorio.SelectedIndex = 3;
                                this.txt_initDataRep.Text = DocumentManager.getFirstDayOfWeek();
                                this.txt_initDataRep.ReadOnly = true;
                                this.txt_fineDataRep.Visible = true;
                                this.txt_fineDataRep.Text = DocumentManager.getLastDayOfWeek();
                                this.txt_fineDataRep.Visible = true;
                                this.txt_fineDataRep.ReadOnly = true;
                                this.LtlADataRep.Visible = true;
                                this.LtlDaDataRep.Visible = true;
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_REPERTORIO_MC.ToString() && item.valore == "1")
                            {
                                this.ddl_dataRepertorio.SelectedIndex = 4;
                                this.txt_initDataRep.Text = DocumentManager.getFirstDayOfMonth();
                                this.txt_initDataRep.ReadOnly = true;
                                this.txt_fineDataRep.Visible = true;
                                this.txt_fineDataRep.Text = DocumentManager.getLastDayOfMonth();
                                this.txt_fineDataRep.Visible = true;
                                this.txt_fineDataRep.ReadOnly = true;
                                this.LtlADataRep.Visible = true;
                                this.LtlDaDataRep.Visible = true;
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_REPERTORIO_TODAY.ToString() && item.valore == "1")
                            {
                                this.ddl_dataRepertorio.SelectedIndex = 2;
                                this.txt_initDataRep.Visible = true;
                                this.txt_initDataRep.Text = DocumentManager.toDay();
                                this.txt_initDataRep.Visible = true;
                                this.txt_initDataRep.ReadOnly = true;
                                this.txt_fineDataRep.Visible = false;
                                this.txt_fineDataRep.Visible = false;
                                this.LtlADataRep.Visible = false;
                                this.LtlDaDataRep.Visible = false;
                            }
                            #endregion
                            #region NUM_PROTOCOLLO
                            else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString())
                            {
                                if (this.ddl_numProt_E.SelectedIndex != 0)
                                    this.ddl_numProt_E.SelectedIndex = 0;
                                this.ddl_numProt_E_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initNumProt_E.Text = item.valore;
                            }
                            #endregion NUM_PROTOCOLLO
                            #region NUM_PROTOCOLLO_DAL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString())
                            {
                                if (this.ddl_numProt_E.SelectedIndex != 1)
                                    this.ddl_numProt_E.SelectedIndex = 1;
                                this.ddl_numProt_E_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initNumProt_E.Text = item.valore;
                            }
                            #endregion NUM_PROTOCOLLO_DAL
                            #region NUM_PROTOCOLLO_AL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString())
                            {
                                if (this.ddl_numProt_E.SelectedIndex != 1)
                                    this.ddl_numProt_E.SelectedIndex = 1;
                                this.ddl_numProt_E_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_fineNumProt_E.Text = item.valore;
                            }
                            #endregion NUM_PROTOCOLLO_AL
                            #region ANNO_PROTOCOLLO
                            else if (item.argomento == DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString())
                            {
                                this.TxtYear.Text = item.valore;
                            }
                            #endregion ANNO_PROTOCOLLO
                            #region DATA_SCADENZA_IL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_SCADENZA_IL.ToString())
                            {
                                if (this.ddl_dataScadenza_C.SelectedIndex != 0)
                                    this.ddl_dataScadenza_C.SelectedIndex = 0;
                                this.ddl_dataScadenza_C_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initDataScadenza_C.Visible = true;
                                this.txt_initDataScadenza_C.Visible = true;
                                this.txt_initDataScadenza_C.Text = item.valore;
                            }
                            #endregion DATA_SCADENZA_IL
                            #region DATA_SCADENZA_SUCCESSIVA_AL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_SCADENZA_SUCCESSIVA_AL.ToString())
                            {
                                if (this.ddl_dataScadenza_C.SelectedIndex != 1)
                                    this.ddl_dataScadenza_C.SelectedIndex = 1;
                                this.ddl_dataScadenza_C_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initDataScadenza_C.Visible = true;
                                this.txt_initDataScadenza_C.Visible = true;
                                this.txt_initDataScadenza_C.Text = item.valore;
                            }
                            #endregion DATA_SCADENZA_SUCCESSIVA_AL
                            #region DATA_SCADENZA_PRECEDENTE_IL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_SCADENZA_PRECEDENTE_IL.ToString())
                            {
                                if (this.ddl_dataScadenza_C.SelectedIndex != 1)
                                    this.ddl_dataScadenza_C.SelectedIndex = 1;
                                this.ddl_dataScadenza_C_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_fineDataScadenza_C.Visible = true;
                                this.txt_fineDataScadenza_C.Visible = true;
                                this.LtlADataScad.Visible = true;
                                this.LtlDaDataScad.Visible = true;
                                this.txt_fineDataScadenza_C.Text = item.valore;
                            }
                            #endregion DATA_SCADENZA_PRECEDENTE_IL
                            #region DATA_SCAD_SC
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_SCAD_SC.ToString() && item.valore == "1")
                            {
                                this.ddl_dataScadenza_C.SelectedIndex = 3;
                                this.txt_initDataScadenza_C.Text = DocumentManager.getFirstDayOfWeek();
                                this.txt_initDataScadenza_C.Enabled = false;
                                this.txt_fineDataScadenza_C.Visible = true;
                                this.txt_fineDataScadenza_C.Text = DocumentManager.getLastDayOfWeek();
                                this.txt_fineDataScadenza_C.Visible = true;
                                this.txt_fineDataScadenza_C.Enabled = false;
                                this.LtlADataScad.Visible = true;
                                this.LtlDaDataScad.Visible = true;
                            }
                            #endregion
                            #region DATA_SCAD_MC
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_SCAD_MC.ToString() && item.valore == "1")
                            {
                                this.ddl_dataScadenza_C.SelectedIndex = 4;
                                this.txt_initDataScadenza_C.Text = DocumentManager.getFirstDayOfMonth();
                                this.txt_initDataScadenza_C.Enabled = false;
                                this.txt_fineDataScadenza_C.Visible = true;
                                this.txt_fineDataScadenza_C.Text = DocumentManager.getLastDayOfMonth();
                                this.txt_fineDataScadenza_C.Visible = true;
                                this.txt_fineDataScadenza_C.Enabled = false;
                                this.LtlADataScad.Visible = true;
                                this.LtlDaDataScad.Visible = true;
                            }
                            #endregion
                            #region DATA_SCAD_TODAY
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_SCAD_TODAY.ToString() && item.valore == "1")
                            {
                                this.ddl_dataScadenza_C.SelectedIndex = 2;
                                this.txt_initDataScadenza_C.Visible = true;
                                this.txt_initDataScadenza_C.Text = DocumentManager.toDay();
                                this.txt_initDataScadenza_C.Visible = true;
                                this.txt_initDataScadenza_C.Enabled = false;
                                this.txt_fineDataScadenza_C.Visible = false;
                                this.txt_fineDataScadenza_C.Visible = false;
                                this.LtlADataScad.Visible = false;
                                this.LtlDaDataScad.Visible = false;
                            }
                            #endregion
                            #region DATA_CREAZIONE_IL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString())
                            {
                                if (ddl_dataCreazione_E.SelectedIndex != 0)
                                    ddl_dataCreazione_E.SelectedIndex = 0;
                                ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initDataCreazione_E.Visible = true;
                                this.txt_initDataCreazione_E.Visible = true;
                                this.txt_initDataCreazione_E.Text = item.valore;
                            }
                            #endregion DATA_CREAZIONE_IL
                            #region DATA_CREAZIONE_SUCCESSIVA_AL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString())
                            {
                                if (ddl_dataCreazione_E.SelectedIndex != 1)
                                    ddl_dataCreazione_E.SelectedIndex = 1;
                                ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initDataCreazione_E.Visible = true;
                                this.txt_initDataCreazione_E.Visible = true;
                                this.txt_initDataCreazione_E.Text = item.valore;
                            }
                            #endregion DATA_CREAZIONE_SUCCESSIVA_AL
                            #region DATA_CREAZIONE_PRECEDENTE_IL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString())
                            {
                                if (this.ddl_dataCreazione_E.SelectedIndex != 1)
                                    this.ddl_dataCreazione_E.SelectedIndex = 1;
                                this.ddl_dataCreazione_E_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_finedataCreazione_E.Visible = true;
                                this.txt_finedataCreazione_E.Visible = true;
                                this.txt_finedataCreazione_E.Text = item.valore;
                            }
                            #endregion DATA_CREAZIONE_PRECEDENTE_IL
                            #region DATA_CREAZ_SC
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_SC.ToString() && item.valore == "1")
                            {
                                this.ddl_dataCreazione_E.SelectedIndex = 3;
                                this.txt_initDataCreazione_E.Text = DocumentManager.getFirstDayOfWeek();
                                this.txt_initDataCreazione_E.ReadOnly = true;
                                this.txt_finedataCreazione_E.Visible = true;
                                this.txt_finedataCreazione_E.Text = DocumentManager.getLastDayOfWeek();
                                this.txt_finedataCreazione_E.Visible = true;
                                this.txt_finedataCreazione_E.ReadOnly = true;
                                this.LtlADataCreazione.Visible = true;
                                this.LtlDaDataCreazione.Visible = true;
                            }
                            #endregion
                            #region DATA_CREAZ_MC
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_MC.ToString() && item.valore == "1")
                            {
                                this.ddl_dataCreazione_E.SelectedIndex = 4;
                                this.txt_initDataCreazione_E.Text = DocumentManager.getFirstDayOfMonth();
                                this.txt_initDataCreazione_E.ReadOnly = true;
                                this.txt_finedataCreazione_E.Visible = true;
                                this.txt_finedataCreazione_E.Text = DocumentManager.getLastDayOfMonth();
                                this.txt_finedataCreazione_E.Visible = true;
                                this.txt_finedataCreazione_E.ReadOnly = true;
                                this.LtlADataCreazione.Visible = true;
                                this.LtlDaDataCreazione.Visible = true;
                            }
                            #endregion
                            #region DATA_CREAZ_TODAY
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_TODAY.ToString() && item.valore == "1")
                            {
                                this.ddl_dataCreazione_E.SelectedIndex = 2;
                                this.txt_initDataCreazione_E.Visible = true;
                                this.txt_initDataCreazione_E.Text = DocumentManager.toDay();
                                this.txt_initDataCreazione_E.Visible = true;
                                this.txt_initDataCreazione_E.ReadOnly = true;
                                this.txt_finedataCreazione_E.Visible = false;
                                this.txt_finedataCreazione_E.Visible = false;
                                this.LtlADataCreazione.Visible = false;
                                this.LtlDaDataCreazione.Visible = false;
                            }
                            #endregion
                            #region DATA CREAZ_YESTERDAY
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_YESTERDAY.ToString() && item.valore == "1")
                            {
                                this.ddl_dataCreazione_E.SelectedIndex = 5;
                                this.txt_initDataCreazione_E.Visible = true;
                                this.txt_initDataCreazione_E.Text = NttDataWA.Utils.dateformat.GetYesterday();
                                this.txt_initDataCreazione_E.Visible = true;
                                this.txt_initDataCreazione_E.ReadOnly = true;
                                this.txt_finedataCreazione_E.Visible = false;
                                this.txt_finedataCreazione_E.Visible = false;
                                this.LtlADataCreazione.Visible = false;
                                this.LtlDaDataCreazione.Visible = false;
                            }
                            #endregion
                            #region DATA_CREAZ_LAST_SEVEN_DAYS
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_LAST_SEVEN_DAYS.ToString() && item.valore == "1")
                            {
                                this.ddl_dataCreazione_E.SelectedIndex = 6;
                                this.txt_initDataCreazione_E.Text = NttDataWA.Utils.dateformat.GetLastSevenDay();
                                this.txt_initDataCreazione_E.ReadOnly = true;
                                this.txt_finedataCreazione_E.Visible = true;
                                this.txt_finedataCreazione_E.Text = DocumentManager.toDay();
                                this.txt_finedataCreazione_E.Visible = true;
                                this.txt_finedataCreazione_E.ReadOnly = true;
                                this.LtlADataCreazione.Visible = true;
                                this.LtlDaDataCreazione.Visible = true;
                            }
                            #endregion
                            #region DATA_CREAZ_LAST_THIRTY_ONE_DAYS
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_LAST_THIRTY_ONE_DAYS.ToString() && item.valore == "1")
                            {
                                this.ddl_dataCreazione_E.SelectedIndex = 7;
                                this.txt_initDataCreazione_E.Text = NttDataWA.Utils.dateformat.GetLastThirtyOneDay();
                                this.txt_initDataCreazione_E.ReadOnly = true;
                                this.txt_finedataCreazione_E.Visible = true;
                                this.txt_finedataCreazione_E.Text = DocumentManager.toDay();
                                this.txt_finedataCreazione_E.Visible = true;
                                this.txt_finedataCreazione_E.ReadOnly = true;
                                this.LtlADataCreazione.Visible = true;
                                this.LtlDaDataCreazione.Visible = true;
                            }
                            #endregion
                            #region DATA_PROT_IL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString())
                            {
                                if (this.ddl_dataProt_E.SelectedIndex != 0)
                                    this.ddl_dataProt_E.SelectedIndex = 0;
                                this.ddl_dataProt_E_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initDataProt_E.Visible = true;
                                this.txt_initDataProt_E.Visible = true;
                                this.txt_initDataProt_E.Text = item.valore;
                            }
                            #endregion DATA_PROT_IL
                            #region DATA_PROT_SUCCESSIVA_AL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString())
                            {
                                if (this.ddl_dataProt_E.SelectedIndex != 1)
                                    this.ddl_dataProt_E.SelectedIndex = 1;
                                this.ddl_dataProt_E_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initDataProt_E.Visible = true;
                                this.txt_initDataProt_E.Visible = true;
                                this.txt_initDataProt_E.Text = item.valore;
                            }
                            #endregion DATA_PROT_SUCCESSIVA_AL
                            #region DATA_PROT_PRECEDENTE_IL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString())
                            {
                                if (ddl_dataProt_E.SelectedIndex != 1)
                                    ddl_dataProt_E.SelectedIndex = 1;
                                ddl_dataProt_E_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_fineDataProt_E.Visible = true;
                                this.txt_fineDataProt_E.Visible = true;
                                this.LtlADataProto.Visible = true;
                                this.LtlDaDataProto.Visible = true;
                                this.txt_fineDataProt_E.Text = item.valore;
                            }
                            #endregion DATA_PROT_PRECEDENTE_IL
                            #region DATA_PROT_SC
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_SC.ToString() && item.valore == "1")
                            {
                                this.ddl_dataProt_E.SelectedIndex = 3;
                                this.txt_initDataProt_E.Text = DocumentManager.getFirstDayOfWeek();
                                this.txt_initDataProt_E.ReadOnly = true;
                                this.txt_fineDataProt_E.Visible = true;
                                this.txt_fineDataProt_E.Text = DocumentManager.getLastDayOfWeek();
                                this.txt_fineDataProt_E.Visible = true;
                                this.txt_fineDataProt_E.ReadOnly = true;
                                this.LtlDaDataProto.Visible = true;
                                this.LtlADataProto.Visible = true;
                            }
                            #endregion
                            #region DATA_PROT_MC
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MC.ToString() && item.valore == "1")
                            {
                                this.ddl_dataProt_E.SelectedIndex = 4;
                                this.txt_initDataProt_E.Text = DocumentManager.getFirstDayOfMonth();
                                this.txt_initDataProt_E.ReadOnly = true;
                                this.txt_fineDataProt_E.Visible = true;
                                this.txt_fineDataProt_E.Text = DocumentManager.getLastDayOfMonth();
                                this.txt_fineDataProt_E.Visible = true;
                                this.txt_fineDataProt_E.ReadOnly = true;
                                this.LtlDaDataProto.Visible = true;
                                this.LtlADataProto.Visible = true;
                            }
                            #endregion
                            #region DATA_PROT_TODAY
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_TODAY.ToString() && item.valore == "1")
                            {
                                this.ddl_dataProt_E.SelectedIndex = 2;
                                this.txt_initDataProt_E.Visible = true;
                                this.txt_initDataProt_E.Text = DocumentManager.toDay();
                                this.txt_initDataProt_E.Visible = true;
                                this.txt_initDataProt_E.ReadOnly = true;
                                this.txt_fineDataProt_E.Visible = false;
                                this.txt_fineDataProt_E.Visible = false;
                                this.LtlDaDataProto.Visible = false;
                                this.LtlADataProto.Visible = false;
                            }
                            #endregion
                            #region DATA_PROTO_YESTERDAY
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROTO_YESTERDAY.ToString() && item.valore == "1")
                            {
                                this.ddl_dataProt_E.SelectedIndex = 5;
                                this.txt_initDataProt_E.Visible = true;
                                this.txt_initDataProt_E.Text = NttDataWA.Utils.dateformat.GetYesterday();
                                this.txt_initDataProt_E.Visible = true;
                                this.txt_initDataProt_E.ReadOnly = true;
                                this.txt_fineDataProt_E.Visible = false;
                                this.txt_fineDataProt_E.Visible = false;
                                this.LtlDaDataProto.Visible = false;
                                this.LtlADataProto.Visible = false;
                            }
                            #endregion
                            #region DATA_PROTO_LAST_SEVEN_DAYS
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROTO_LAST_SEVEN_DAYS.ToString() && item.valore == "1")
                            {
                                this.ddl_dataProt_E.SelectedIndex = 6;
                                this.txt_initDataProt_E.Text = NttDataWA.Utils.dateformat.GetLastSevenDay();
                                this.txt_initDataProt_E.ReadOnly = true;
                                this.txt_fineDataProt_E.Visible = true;
                                this.txt_fineDataProt_E.Text = DocumentManager.toDay();
                                this.txt_fineDataProt_E.Visible = true;
                                this.txt_fineDataProt_E.ReadOnly = true;
                                this.LtlDaDataProto.Visible = true;
                                this.LtlADataProto.Visible = true;
                            }
                            #endregion
                            #region DATA_PROTO_LAST_THIRTY_ONE_DAYS
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROTO_LAST_THIRTY_ONE_DAYS.ToString() && item.valore == "1")
                            {
                                this.ddl_dataProt_E.SelectedIndex = 7;
                                this.txt_initDataProt_E.Text = NttDataWA.Utils.dateformat.GetLastThirtyOneDay();
                                this.txt_initDataProt_E.ReadOnly = true;
                                this.txt_fineDataProt_E.Visible = true;
                                this.txt_fineDataProt_E.Text = DocumentManager.toDay();
                                this.txt_fineDataProt_E.Visible = true;
                                this.txt_fineDataProt_E.ReadOnly = true;
                                this.LtlDaDataProto.Visible = true;
                                this.LtlADataProto.Visible = true;
                            }
                            #endregion
                            #region DOCNUMBER
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER.ToString())
                            {
                                if (this.ddl_idDocumento_C.SelectedIndex != 0)
                                    this.ddl_idDocumento_C.SelectedIndex = 0;
                                this.ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initIdDoc_C.Text = item.valore;
                            }
                            #endregion DOCNUMBER
                            #region DOCNUMBER_DAL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString())
                            {
                                if (this.ddl_idDocumento_C.SelectedIndex != 1)
                                    this.ddl_idDocumento_C.SelectedIndex = 1;
                                this.ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initIdDoc_C.Text = item.valore;
                            }
                            #endregion DOCNUMBER_DAL
                            #region DOCNUMBER_AL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString())
                            {
                                if (this.ddl_idDocumento_C.SelectedIndex != 1)
                                    this.ddl_idDocumento_C.SelectedIndex = 1;
                                this.ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_fineIdDoc_C.Text = item.valore;
                            }
                            #endregion DOCNUMBER_AL
                            #region DATA_STAMPA_IL
                            else if (item.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO.ToString())
                            {
                                if (this.ddl_dataStampa_E.SelectedIndex != 0)
                                    this.ddl_dataStampa_E.SelectedIndex = 0;
                                this.ddl_dataStampa_E_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initDataStampa_E.Visible = true;
                                this.txt_initDataStampa_E.Text = item.valore;
                            }
                            #endregion DATA_STAMPA_IL
                            #region DATA_STAMPA_SUCCESSIVA_AL
                            else if (item.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_DAL.ToString())
                            {
                                if (this.ddl_dataStampa_E.SelectedIndex != 1)
                                    this.ddl_dataStampa_E.SelectedIndex = 1;
                                this.ddl_dataStampa_E_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initDataStampa_E.Visible = true;
                                this.txt_initDataStampa_E.Text = item.valore;
                            }
                            #endregion DATA_STAMPA_SUCCESSIVA_AL
                            #region DATA_STAMPA_PRECEDENTE_IL
                            else if (item.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_AL.ToString())
                            {
                                if (this.ddl_dataStampa_E.SelectedIndex != 1)
                                    this.ddl_dataStampa_E.SelectedIndex = 1;
                                this.ddl_dataStampa_E_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_finedataStampa_E.Visible = true;
                                this.LtlDaDataStampa.Visible = true;
                                this.LtlADataStampa.Visible = true;
                                this.txt_finedataStampa_E.Text = item.valore;
                            }
                            #endregion DATA_STAMPA_PRECEDENTE_IL
                            #region DATA_STAMPA_SC
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_STAMPA_SC.ToString() && item.valore == "1")
                            {
                                this.ddl_dataStampa_E.SelectedIndex = 3;
                                this.txt_initDataStampa_E.Text = DocumentManager.getFirstDayOfWeek();
                                this.txt_initDataStampa_E.Enabled = false;
                                this.txt_finedataStampa_E.Visible = true;
                                this.txt_finedataStampa_E.Text = DocumentManager.getLastDayOfWeek();
                                this.txt_finedataStampa_E.Visible = true;
                                this.txt_finedataStampa_E.Enabled = false;
                                this.LtlADataStampa.Visible = true;
                                this.LtlDaDataStampa.Visible = true;
                            }
                            #endregion
                            #region DATA_STAMPA_MC
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_STAMPA_MC.ToString() && item.valore == "1")
                            {
                                this.ddl_dataStampa_E.SelectedIndex = 4;
                                this.txt_initDataStampa_E.Text = DocumentManager.getFirstDayOfMonth();
                                this.txt_initDataStampa_E.Enabled = false;
                                this.txt_finedataStampa_E.Visible = true;
                                this.txt_finedataStampa_E.Text = DocumentManager.getLastDayOfMonth();
                                this.txt_finedataStampa_E.Visible = true;
                                this.txt_finedataStampa_E.Enabled = false;
                                this.LtlADataStampa.Visible = true;
                                this.LtlDaDataStampa.Visible = true;
                            }
                            #endregion
                            #region DATA_STAMPA_TODAY
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_STAMPA_TODAY.ToString() && item.valore == "1")
                            {
                                this.ddl_dataStampa_E.SelectedIndex = 2;
                                this.txt_initDataStampa_E.Visible = true;
                                this.txt_initDataStampa_E.Text = DocumentManager.toDay();
                                this.txt_initDataStampa_E.Visible = true;
                                this.txt_initDataStampa_E.Enabled = false;
                                this.txt_finedataStampa_E.Visible = false;
                                this.txt_finedataStampa_E.Visible = false;
                                this.LtlADataStampa.Visible = false;
                                this.LtlDaDataStampa.Visible = false;
                            }
                            #endregion
                            #region OGGETTO
                            else if (item.argomento == DocsPaWR.FiltriDocumento.OGGETTO.ToString())
                            {
                                this.TxtObject.Text = item.valore;
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.OGGETTO_DOCUMENTO_PRINCIPALE.ToString())
                            {
                                this.TxtObjectAttach.Text = item.valore;
                                this.pnlOjectAttach.Attributes.Add("style", "display:block");
                            }
                            #endregion OGGETTO
                            #region MITT_DEST
                            else if (item.argomento == DocsPaWR.FiltriDocumento.MITT_DEST.ToString())
                            {
                                txt_descrMit_E.Text = item.valore;
                            }
                            #endregion MITT_DEST
                            #region COD_MITT_DEST
                            else if (item.argomento == DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString())
                            {
                                /*
                                DocsPaWR.Corrispondente corr = AddressBookManager.getCorrispondenteByCodRubrica(item.valore, true);

                                DocsPaWR.Corrispondente corr = AddressBookManager.getCorrispondenteByCodRubrica(item.valore, true);
                                if (corr != null)
                                {
                                    txt_codMit_E.Text = corr.codiceRubrica;
                                    txt_descrMit_E.Text = corr.descrizione;
                                }
                                 * */
                                SearchCorrespondent(item.valore, "txt_codMit_E");
                            }
                            #endregion
                            #region MITT_DEST_STORICIZZATI
                            else if (item.argomento == DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString())
                            {
                                bool chkValue;
                                bool.TryParse(item.valore, out chkValue);
                                this.chk_mitt_dest_storicizzati.Checked = chkValue;
                            }
                            #endregion
                            #region ID_MITT_DEST
                            else if (item.argomento == DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString())
                            {
                                DocsPaWR.Corrispondente corr = AddressBookManager.GetCorrespondentBySystemId(item.valore);
                                if (corr != null)
                                {
                                    txt_codMit_E.Text = corr.codiceRubrica;
                                    txt_descrMit_E.Text = corr.descrizione;
                                }
                            }
                            #endregion ID_MITT_DEST
                            #region NUM_OGGETTO
                            else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_OGGETTO.ToString())
                            {
                                this.txt_numOggetto.Text = item.valore;
                            }
                            #endregion NUM_OGGETTO
                            #region FASCICOLO
                            else if (item.argomento == DocsPaWR.FiltriFascicolazione.IN_CHILD_RIC_ESTESA.ToString())
                            {
                                string val = item.valore.Trim();
                                val = val.Substring("IN".Length).Trim();
                                val = val.Substring("(".Length).Trim();
                                val = val.Substring(0, val.LastIndexOf(")")).Trim();
                                char[] sep = { ',' };
                                string[] ids = val.Split(sep);
                                if (ids != null && ids.Length > 0)
                                {
                                    DocsPaWR.Folder folder = ProjectManager.getFolder(this, ids[0].Trim());
                                    DocsPaWR.Fascicolo fasc = ProjectManager.getFascicoloById(this, folder.idFascicolo);
                                    if (fasc != null)
                                    {
                                        //ArrayList listaFascicoli = FascicoliManager.getFascicoloDaCodice3(this, fasc.codice);
                                        ArrayList listaFascicoli = new ArrayList(ProjectManager.getListaFascicoliDaCodice(this, fasc.codice, UserManager.getRegistroSelezionato(this), "R"));

                                        if (listaFascicoli != null)
                                        {
                                            //ProjectManager.setFascicoliSelezionati(this, listaFascicoli);
                                            this.txt_CodFascicolo.Text = fasc.codice;
                                            if (listaFascicoli.Count == 1)
                                            {
                                                this.txt_DescFascicolo.Text = fasc.descrizione;
                                            }
                                            else
                                            {
                                                if (((DocsPaWR.Fascicolo)listaFascicoli[0]).descrizione == ((DocsPaWR.Fascicolo)listaFascicoli[1]).descrizione)
                                                    this.txt_DescFascicolo.Text = ((DocsPaWR.Fascicolo)listaFascicoli[0]).descrizione;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.ESTENDI_A_NODI_FIGLI_E_FASCICOLI.ToString())
                            {
                                this.txt_CodFascicolo.Text = item.valore;
                                this.txt_CodFascicolo_OnTextChanged(null, null);
                                this.cbxEstendiAFascicoli.Checked = true;
                            }
                            #endregion FASCICOLO
                                #region CODICE_DESCRIZIONE_AMMINISTRAZIONE
                            else if (item.argomento == DocsPaWR.FiltriDocumento.CODICE_DESCRIZIONE_AMMINISTRAZIONE.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                    this.txt_codDesc.Text = item.valore;
                            }
                            #endregion
                            #region TRASMISSIONE CON
                            else if (item.argomento == DocsPaWR.FiltriDocumento.TRASMESSI_CON.ToString())
                            {
                                this.cbx_Trasm.Checked = true;
                                if (item.valore.Equals("Tutte"))
                                    this.ddl_ragioneTrasm.SelectedIndex = 0;
                                else
                                {
                                    // questo da errore.
                                    //this.ddl_ragioneTrasm.SelectedItem.Text = item.valore;
                                    // anche questo perché non trova l'item.
                                    //this.ddl_ragioneTrasm.Items.FindByValue(item.valore).Selected = true;

                                    // risoluzione
                                    this.ddl_ragioneTrasm.SelectedIndex = 1;
                                    this.ddl_ragioneTrasm.SelectedItem.Text = item.valore;

                                }
                            }
                            #endregion
                            #region TRASMISSIONE SENZA
                            else if (item.argomento == DocsPaWR.FiltriDocumento.TRASMESSI_SENZA.ToString())
                            {
                                this.cbx_TrasmSenza.Checked = true;
                                if (item.valore.Equals("Tutte"))
                                    this.ddl_ragioneTrasm.SelectedIndex = 0;
                                else
                                {
                                    //this.ddl_ragioneTrasm.SelectedItem.Text = item.valore;
                                    this.ddl_ragioneTrasm.SelectedIndex = 1;
                                    this.ddl_ragioneTrasm.SelectedItem.Text = item.valore;
                                }

                                this.PnlNeverTrasm.CssClass = "";
                            }
                            #endregion
                            #region ANNULLATO
                            else if (item.argomento == DocsPaWR.FiltriDocumento.ANNULLATO.ToString())
                            {
                                this.rb_annulla_C.SelectedValue = item.valore;
                            }
                            #endregion ANNULLATO
                            #region SEGNATURA
                            else if (item.argomento == DocsPaWR.FiltriDocumento.SEGNATURA.ToString())
                            {
                                this.txt_segnatura.Text = item.valore;
                            }
                            #endregion SEGNATURA
                            #region MITTENTE_INTERMEDIO
                            else if (item.argomento == DocsPaWR.FiltriDocumento.MITTENTE_INTERMEDIO.ToString())
                            {
                                this.txt_descrMittInter_C.Text = item.valore;
                            }
                            #endregion MITTENTE_INTERMEDIO
                            #region ID_MITTENTE_INTERMEDIO
                            else if (item.argomento == DocsPaWR.FiltriDocumento.ID_MITTENTE_INTERMEDIO.ToString())
                            {
                                DocsPaWR.Corrispondente corr = UserManager.getCorrispondentBySystemID(item.valore);
                                this.txt_codMittInter_C.Text = corr.codiceRubrica;
                                this.txt_descrMittInter_C.Text = corr.descrizione;
                            }
                            #endregion ID_MITTENTE_INTERMEDIO
                            #region PROTOCOLLO_MITTENTE
                            else if (item.argomento == DocsPaWR.FiltriDocumento.PROTOCOLLO_MITTENTE.ToString())
                            {
                                this.txt_numProtMitt_C.Text = item.valore;
                            }
                            #endregion PROTOCOLLO_MITTENTE
                            #region DATA_PROT_MITTENTE_IL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_IL.ToString())
                            {
                                if (this.ddl_dataProtMitt_C.SelectedIndex != 0)
                                    this.ddl_dataProtMitt_C.SelectedIndex = 0;
                                this.ddl_dataProtMitt_C_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initDataProtMitt_C.Text = item.valore;
                            }
                            #endregion DATA_PROT_MITTENTE_IL
                            #region DATA_PROT_MITTENTE_SUCCESSIVA_AL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_SUCCESSIVA_AL.ToString())
                            {
                                if (this.ddl_dataProtMitt_C.SelectedIndex != 1)
                                    this.ddl_dataProtMitt_C.SelectedIndex = 1;
                                this.ddl_dataProtMitt_C_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initDataProtMitt_C.Text = item.valore;
                            }
                            #endregion DATA_PROT_MITTENTE_SUCCESSIVA_AL
                            #region DATA_PROT_MITTENTE_PRECEDENTE_IL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_PRECEDENTE_IL.ToString())
                            {
                                if (this.ddl_dataProtMitt_C.SelectedIndex != 1)
                                    this.ddl_dataProtMitt_C.SelectedIndex = 1;
                                this.ddl_dataProtMitt_C_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_fineDataProtMitt_C.Text = item.valore;
                            }
                            #endregion DATA_PROT_MITTENTE_PRECEDENTE_IL
                            #region DATA_PROT_MITTENTE_SC
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_SC.ToString() && item.valore == "1")
                            {
                                this.ddl_dataProtMitt_C.SelectedIndex = 3;
                                this.txt_initDataProtMitt_C.Text = DocumentManager.getFirstDayOfWeek();
                                this.txt_initDataProtMitt_C.ReadOnly = true;
                                this.txt_fineDataProtMitt_C.Text = DocumentManager.getLastDayOfWeek();
                                this.txt_fineDataProtMitt_C.Visible = true;
                                this.txt_fineDataProtMitt_C.ReadOnly = true;
                                this.LtlADataProtMitt.Visible = true;
                                this.LtlDaDataProtMitt.Visible = true;
                            }
                            #endregion
                            #region DATA_PROT_MITTENTE_MC
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_MC.ToString() && item.valore == "1")
                            {
                                this.ddl_dataProtMitt_C.SelectedIndex = 4;
                                this.txt_initDataProtMitt_C.Text = DocumentManager.getFirstDayOfMonth();
                                this.txt_initDataProtMitt_C.ReadOnly = true;
                                this.txt_fineDataProtMitt_C.Text = DocumentManager.getLastDayOfMonth();
                                this.txt_fineDataProtMitt_C.Visible = true;
                                this.txt_fineDataProtMitt_C.ReadOnly = true;
                                this.LtlADataProtMitt.Visible = true;
                                this.LtlDaDataProtMitt.Visible = true;
                            }
                            #endregion
                            #region DATA_PROT_MITTENTE_TODAY
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_TODAY.ToString() && item.valore == "1")
                            {
                                this.ddl_dataProtMitt_C.SelectedIndex = 2;
                                this.txt_initDataProtMitt_C.Visible = true;
                                this.txt_initDataProtMitt_C.Text = DocumentManager.toDay();
                                this.txt_initDataProtMitt_C.Visible = true;
                                this.txt_initDataProtMitt_C.ReadOnly = true;
                                this.txt_fineDataProtMitt_C.Visible = false;
                                this.txt_fineDataProtMitt_C.Visible = false;
                                this.LtlADataProtMitt.Visible = false;
                                this.LtlDaDataProtMitt.Visible = false;
                            }
                            #endregion
                            #region NUM_PROTO_EMERGENZA
                            else if (item.argomento == DocsPaWR.FiltriDocumento.NUM_PROTO_EMERGENZA.ToString())
                            {
                                this.txt_protoEme.Text = item.valore;
                            }
                            #endregion NUM_PROTO_EMERGENZA
                            #region DATA_PROTO_EMERGENZA_IL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_IL.ToString())
                            {
                                this.txt_dataProtoEmeInizio.Text = item.valore;
                            }
                            #endregion DATA_PROTO_EMERGENZA_IL
                            #region DATA_PROTO_EMERGENZA_SUCCESSIVA_AL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_SUCCESSIVA_AL.ToString())
                            {
                                if (this.ddl_dataProtoEme.SelectedIndex != 1)
                                    this.ddl_dataProtoEme.SelectedIndex = 1;
                                this.ddl_dataProtoEme_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_dataProtoEmeInizio.Text = item.valore;
                            }
                            #endregion DATA_PROTO_EMERGENZA_SUCCESSIVA_AL
                            #region DATA_PROTO_EMERGENZA_PRECEDENTE_IL
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_PRECEDENTE_IL.ToString())
                            {
                                if (this.ddl_dataProtoEme.SelectedIndex != 1)
                                    this.ddl_dataProtoEme.SelectedIndex = 1;
                                this.ddl_dataProtoEme_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_dataProtoEmeFine.Text = item.valore;
                            }
                            #endregion DATA_ARRIVO_PRECEDENTE_IL
                            #region DATA_PROTO_EMERGENZA_SC
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_SC.ToString() && item.valore == "1")
                            {
                                this.ddl_dataProtoEme.SelectedIndex = 3;
                                this.txt_dataProtoEmeInizio.Text = DocumentManager.getFirstDayOfWeek();
                                this.txt_dataProtoEmeInizio.Enabled = false;
                                this.txt_dataProtoEmeFine.Text = DocumentManager.getLastDayOfWeek();
                                this.txt_dataProtoEmeFine.Visible = true;
                                this.txt_dataProtoEmeFine.Enabled = false;
                                this.LtlADataSegDiEmerg.Visible = true;
                                this.LtlDaDataSegDiEmerg.Visible = true;
                            }
                            #endregion
                            #region DATA_PROTO_EMERGENZA_MC
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_MC.ToString() && item.valore == "1")
                            {
                                this.ddl_dataProtoEme.SelectedIndex = 4;
                                this.txt_dataProtoEmeInizio.Text = DocumentManager.getFirstDayOfMonth();
                                this.txt_dataProtoEmeInizio.Enabled = false;
                                this.txt_dataProtoEmeFine.Text = DocumentManager.getLastDayOfMonth();
                                this.txt_dataProtoEmeFine.Visible = true;
                                this.txt_dataProtoEmeFine.Enabled = false;
                                this.LtlADataSegDiEmerg.Visible = true;
                                this.LtlDaDataSegDiEmerg.Visible = true;
                            }
                            #endregion
                            #region DATA_PROTO_EMERGENZA_TODAY
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_TODAY.ToString() && item.valore == "1")
                            {
                                this.ddl_dataProtoEme.SelectedIndex = 2;
                                this.txt_dataProtoEmeInizio.Visible = true;
                                this.txt_dataProtoEmeInizio.Text = DocumentManager.toDay();
                                this.txt_dataProtoEmeInizio.Visible = true;
                                this.txt_dataProtoEmeInizio.Enabled = false;
                                this.txt_dataProtoEmeFine.Visible = false;
                                this.txt_dataProtoEmeFine.Visible = false;
                                this.LtlADataSegDiEmerg.Visible = false;
                                this.LtlDaDataSegDiEmerg.Visible = false;
                            }
                            #endregion
                            #region EVIDENZA
                            else if (item.argomento == DocsPaWR.FiltriDocumento.EVIDENZA.ToString())
                            {
                                this.rb_evidenza_C.SelectedValue = item.valore;
                            }
                            #endregion EVIDENZA
                            #region MEZZO SPEDIZIONE
                            else if (item.argomento == DocsPaWR.FiltriDocumento.MEZZO_SPEDIZIONE.ToString())
                            {
                                this.ddl_spedizione.SelectedValue = item.valore;
                            }
                            #endregion
                            #region Mancanza Immagine
                            else if (item.argomento == DocsPaWR.FiltriDocumento.MANCANZA_IMMAGINE.ToString())
                            {
                                if (item.valore.Equals("1"))
                                    this.cbl_docInCompl.Items.FindByValue("S_Img").Selected = true;
                                if (item.valore.Equals("0"))
                                    this.cbl_docInCompl.Items.FindByValue("C_Img").Selected = true;
                            }
                            #endregion
                            #region Mancanza Fascicolazione

                            else if (item.argomento == DocsPaWR.FiltriDocumento.MANCANZA_FASCICOLAZIONE.ToString())
                            {
                                if (item.valore.Equals("1"))
                                    this.cbl_docInCompl.Items.FindByValue("S_Fasc").Selected = true;
                                if (item.valore.Equals("0"))
                                    this.cbl_docInCompl.Items.FindByValue("C_Fasc").Selected = true;
                            }
                            #endregion
                            #region CONSERVAZIONE
                            else if (item.argomento == DocsPaWR.FiltriDocumento.CONSERVAZIONE.ToString())
                            {
                                if (item.valore.Equals("1"))
                                {
                                    this.cb_Conservato.Checked = true;
                                    this.cb_NonConservato.Checked = false;
                                }
                                if (item.valore.Equals("0"))
                                {
                                    this.cb_NonConservato.Checked = true;
                                    this.cb_Conservato.Checked = false;
                                }
                            }
                            #endregion
                            #region STATO CONSERVAZIONE
                            else if (item.argomento == DocsPaWR.FiltriDocumento.STATO_CONSERVAZIONE.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    this.cbl_Conservazione.Items[0].Selected = item.valore.Contains("N");
                                    this.cbl_Conservazione.Items[1].Selected = item.valore.Contains("V");
                                    this.cbl_Conservazione.Items[2].Selected = item.valore.Contains("W");
                                    this.cbl_Conservazione.Items[3].Selected = item.valore.Contains("C");
                                    this.cbl_Conservazione.Items[4].Selected = item.valore.Contains("R");
                                    this.cbl_Conservazione.Items[5].Selected = item.valore.Contains("E");
                                    this.cbl_Conservazione.Items[6].Selected = item.valore.Contains("T");
                                    this.cbl_Conservazione.Items[7].Selected = item.valore.Contains("F");
                                    this.cbl_Conservazione.Items[8].Selected = item.valore.Contains("B");
                                    this.cbl_Conservazione.Items[9].Selected = item.valore.Contains("K");
                                }
                            }
                            #endregion
                            #region DATA DI VERSAMENTO
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_VERSAMENTO_IL.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    this.ddl_DataVers.SelectedIndex = 0;
                                    this.ddl_dataVers_SelectedIndexChanged(null, new System.EventArgs());
                                    this.txt_initDataVers.Text = item.valore;
                                }
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_VERSAMENTO_DA.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    this.ddl_DataVers.SelectedIndex = 1;
                                    this.ddl_dataVers_SelectedIndexChanged(null, new System.EventArgs());
                                    this.txt_initDataVers.Text = item.valore;
                                }
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_VERSAMENTO_A.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    if (!this.ddl_DataVers.SelectedIndex.Equals(1))
                                    {
                                        this.ddl_DataVers.SelectedIndex = 1;
                                    }
                                    this.ddl_dataVers_SelectedIndexChanged(null, new System.EventArgs());
                                    this.txt_fineDataVers.Text = item.valore;
                                }
                            }
                            #endregion
                            #region CODICE POLICY
                            else if (item.argomento == DocsPaWR.FiltriDocumento.POLICY_CODICE.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    this.txtCodPolicy.Text = item.valore;
                                }
                            }
                            #endregion
                            #region NUMERO ESECUZIONE POLICY
                            else if (item.argomento == DocsPaWR.FiltriDocumento.POLICY_NUM_ESECUZIONE.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    this.txtCounterPolicy.Text = item.valore;
                                }
                            }
                            #endregion
                            #region DATA ESECUZIONE POLICY
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_EXEC_POLICY_IL.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    this.ddl_datePolicy.SelectedIndex = 0;
                                    this.ddl_datePolicy_SelectedIndexChanged(null, new System.EventArgs());
                                    this.txt_initDatePolicy.Text = item.valore;
                                }
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_EXEC_POLICY_DA.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    this.ddl_datePolicy.SelectedIndex = 1;
                                    this.ddl_datePolicy_SelectedIndexChanged(null, new System.EventArgs());
                                    this.txt_initDatePolicy.Text = item.valore;
                                }
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_EXEC_POLICY_A.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    if (!this.ddl_datePolicy.SelectedIndex.Equals(1))
                                    {
                                        this.ddl_datePolicy.SelectedIndex = 1;
                                    }
                                    this.ddl_datePolicy_SelectedIndexChanged(null, new System.EventArgs());
                                    this.txt_fineDatePolicy.Text = item.valore;
                                }
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_EXEC_POLICY_YESTERDAY.ToString())
                            {
                                this.ddl_datePolicy.SelectedIndex = 5;
                                this.ddl_datePolicy_SelectedIndexChanged(null, new System.EventArgs());
                                this.txt_initDatePolicy.Text = NttDataWA.Utils.dateformat.GetYesterday();
                            }
                            #endregion
                            #region NUMERO_VERSIONI
                            else if (item.argomento == DocsPaWR.FiltriDocumento.NUMERO_VERSIONI.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    this.ddl_op_versioni.SelectedValue = item.valore.Substring(0, 1);
                                    this.txt_versioni.Text = item.valore.Substring(1);
                                }
                            }
                            #endregion
                            #region NUMERO_ALLEGATI
                            else if (item.argomento == DocsPaWR.FiltriDocumento.NUMERO_ALLEGATI.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    this.ddl_op_allegati.SelectedValue = item.valore.Substring(0, 1);
                                    this.txt_allegati.Text = item.valore.Substring(1);
                                }
                            }
                            #endregion
                            #region TIPO ALLEGATI
                            // INC000000589215
                            // aggiunto filtro tipo allegati in ricerche salvate
                            else if (item.argomento == "NUMERO_ALLEGATI_TIPO")
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    this.rblFiltriNumAllegati.SelectedValue = item.valore;
                                }
                            }
                            #endregion
                            #region Visibilità Tipica / Atipica
                            else if (item.argomento == DocsPaWR.FiltriDocumento.VISIBILITA_T_A.ToString())
                            {
                                this.rblVisibility.SelectedValue = item.valore;
                            }
                            #endregion Visibilità Tipica / Atipica
                            #region CODICE_TIPO_NOTIFICA
                            else if (item.argomento == DocsPaWR.FiltriDocumento.CODICE_TIPO_NOTIFICA.ToString())
                            {
                                for (int i = 0; i < this.ddl_ricevute_pec.Items.Count; i++)
                                {
                                    if (this.ddl_ricevute_pec.Items[i].Value.ToUpper().Equals(item.valore.ToUpper()))
                                    {
                                        this.ddl_ricevute_pec.SelectedIndex = i;
                                        break;
                                    }
                                }
                                this.ddl_data_ricevute_pec.SelectedIndex = 0;
                                this.ddl_data_ricevute_pec_SelectedIndexChanged(null, new System.EventArgs());
                            }
                            #endregion
                            #region CODICE_TIPO_NOTIFICA_PITRE
                            else if (item.argomento == DocsPaWR.FiltriDocumento.CODICE_TIPO_NOTIFICA_PITRE.ToString())
                            {
                                for (int i = 0; i < ddl_ricevute_pitre.Items.Count; i++)
                                {
                                    if (this.ddl_ricevute_pitre.Items[i].Value.ToUpper().Equals(item.valore.ToUpper()))
                                    {
                                        this.ddl_ricevute_pitre.SelectedIndex = i;
                                        break;
                                    }
                                }
                                this.ddl_data_ricevute_pitre.SelectedIndex = 0;
                                this.ddl_data_ricevute_pitre_SelectedIndexChanged(null, new System.EventArgs());
                            }
                            #endregion
                            #region DATA_TIPO_NOTIFICA_DA
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_DA.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    this.ddl_data_ricevute_pec.SelectedIndex = 0;
                                    this.ddl_data_ricevute_pec_SelectedIndexChanged(null, new System.EventArgs());
                                    this.Cal_Da_pec.Text = item.valore;
                                }
                            }
                            #endregion
                            #region DATA_TIPO_NOTIFICA_DA_PITRE
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_DA_PITRE.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    this.ddl_data_ricevute_pitre.SelectedIndex = 0;
                                    this.ddl_data_ricevute_pitre_SelectedIndexChanged(null, new System.EventArgs());
                                    this.Cal_Da_pitre.Text = item.valore;
                                }
                            }
                            #endregion
                            #region DATA_TIPO_NOTIFICA_A
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_A.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    this.ddl_data_ricevute_pec.SelectedIndex = 1;
                                    this.ddl_data_ricevute_pec_SelectedIndexChanged(null, new System.EventArgs());
                                    this.Cal_A_pec.Text = item.valore;
                                }
                            }
                            #endregion
                            #region DATA_TIPO_NOTIFICA_A_PITRE
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_A_PITRE.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    this.ddl_data_ricevute_pitre.SelectedIndex = 1;
                                    this.ddl_data_ricevute_pitre_SelectedIndexChanged(null, new System.EventArgs());
                                    this.Cal_A_pitre.Text = item.valore;
                                }
                            }
                            #endregion
                            #region DATA_TIPO_NOTIFICA_TODAY
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_TODAY.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    this.ddl_data_ricevute_pec.SelectedIndex = 2;
                                    this.ddl_data_ricevute_pec_SelectedIndexChanged(null, new System.EventArgs());
                                    this.Cal_Da_pec.Text = item.valore;
                                }
                            }
                            #endregion
                            #region DATA_TIPO_NOTIFICA_TODAY_PITRE
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_TODAY_PITRE.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                {
                                    ddl_data_ricevute_pitre.SelectedIndex = 2;
                                    ddl_data_ricevute_pitre_SelectedIndexChanged(null, new System.EventArgs());
                                    this.Cal_Da_pitre.Text = item.valore;
                                }
                            }
                            #endregion
                            #region DOC_SPEDITI
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DOC_SPEDITI_TIPO.ToString())
                            {
                                switch (item.valore)
                                {
                                    case "PEC":
                                        if (this.cbx_pec.Visible)
                                            this.cbx_pec.Checked = true;
                                        if (this.cbx_pitre.Visible)
                                            this.cbx_pitre.Checked = false;
                                        break;

                                    case "PITRE":
                                        if (this.cbx_pec.Visible)
                                            this.cbx_pec.Checked = false;
                                        if (this.cbx_pitre.Visible)
                                            this.cbx_pitre.Checked = true;
                                        break;

                                    case "ALL":
                                        if (this.cbx_pec.Visible)
                                            this.cbx_pec.Checked = true;
                                        if (this.cbx_pitre.Visible)
                                            this.cbx_pitre.Checked = true;
                                        break;
                                }
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DOC_SPEDITI.ToString())
                            {
                                if (item.valore == "1")
                                    this.rb_docSpediti.SelectedIndex = 0;
                                if (item.valore == "0")
                                    this.rb_docSpediti.SelectedIndex = 3;
                                if (item.valore == "T")
                                    this.rb_docSpediti.SelectedIndex = 4;
                                if (item.valore == "2")
                                    this.rb_docSpediti.SelectedIndex = 1;
                                if (item.valore == "3")
                                    this.rb_docSpediti.SelectedIndex = 2;
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DOC_SPEDITI_ESITO.ToString())
                            {
                                if (item.valore == "V")
                                    this.rb_docSpeditiEsito.SelectedIndex = 0;
                                if (item.valore == "A")
                                    this.rb_docSpeditiEsito.SelectedIndex = 1;
                                if (item.valore == "X")
                                    this.rb_docSpeditiEsito.SelectedIndex = 2;

                            }
                            #endregion
                            #region FILTRI TIMESTAMP
                            else if (item.argomento == DocsPaWR.FiltriDocumento.SENZA_TIMESTAMP.ToString())
                            {
                                this.rbl_timestamp.SelectedValue = "1";
                                this.ddl_timestamp.SelectedValue = "0";
                                this.date_timestamp.Text = string.Empty;
                                this.ddl_timestamp.Visible = false;
                                this.date_timestamp.Visible = false;
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.CON_TIMESTAMP.ToString())
                            {
                                this.rbl_timestamp.SelectedValue = "0";
                                this.ddl_timestamp.SelectedValue = "0";
                                this.date_timestamp.Text = string.Empty;
                                this.ddl_timestamp.Visible = true;
                                this.date_timestamp.Visible = false;
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.TIMESTAMP_SCADUTO.ToString())
                            {
                                this.rbl_timestamp.SelectedValue = "0";
                                this.ddl_timestamp.SelectedValue = "1";
                                this.date_timestamp.Text = string.Empty;
                                this.ddl_timestamp.Visible = true;
                                this.date_timestamp.Visible = false;
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.TIMESTAMP_SCADE_PRIMA_DI.ToString())
                            {
                                this.rbl_timestamp.SelectedValue = "0";
                                this.ddl_timestamp.SelectedValue = "2";
                                this.date_timestamp.Text = item.valore;
                                this.ddl_timestamp.Visible = true;
                                this.date_timestamp.Visible = true;
                            }
                            #endregion FILTRI TIMESTAMP
                            #region FILTRI CONSOLIDAMENTO
                            else if (item.argomento == DocsPaWR.FiltriDocumento.STATO_CONSOLIDAMENTO.ToString())
                            {
                                foreach (string itm in item.valore.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                                    if (itm == "0")
                                    {
                                        this.pnl_data_cons.Visible = false;
                                        this.lstFiltriConsolidamento.Items[0].Selected = true;
                                    }
                                    else
                                    {
                                        this.pnl_data_cons.Visible = true;
                                        if (itm == "1")
                                        {
                                            this.lstFiltriConsolidamento.Items[1].Selected = true;
                                        }
                                        else
                                        {
                                            this.lstFiltriConsolidamento.Items[2].Selected = true;
                                        }
                                    }

                                this.UpStatoConsolidamento.Update();
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_CONSOLIDAMENTO_DA.ToString())
                            {
                                this.txtDataConsolidamento.Text = item.valore;
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_CONSOLIDAMENTO_A.ToString())
                            {
                                this.txtDataConsolidamentoFinale.Text = item.valore;
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DATA_TIPO_CONSOLIDAMENTO.ToString())
                            {
                                this.cboDataConsolidamento.SelectedIndex = Int32.Parse(item.valore);
                                switch (item.valore)
                                {
                                    case "0":
                                        this.txtDataConsolidamento.Visible = true;
                                        this.txtDataConsolidamento.Visible = true;
                                        this.txtDataConsolidamento.Enabled = true;
                                        this.txtDataConsolidamentoFinale.Visible = false;
                                        this.txtDataConsolidamentoFinale.Visible = false;
                                        this.txtDataConsolidamentoFinale.Text = string.Empty;
                                        break;

                                    case "1":
                                        this.txtDataConsolidamento.Visible = true;
                                        this.txtDataConsolidamento.Visible = true;
                                        this.txtDataConsolidamento.Enabled = true;
                                        this.txtDataConsolidamentoFinale.Visible = true;
                                        this.txtDataConsolidamentoFinale.Visible = true;
                                        this.txtDataConsolidamentoFinale.Enabled = true;
                                        break;

                                    case "2":
                                        this.txtDataConsolidamento.Visible = true;
                                        this.txtDataConsolidamento.Visible = true;
                                        this.txtDataConsolidamento.Enabled = false;
                                        this.txtDataConsolidamentoFinale.Visible = false;
                                        this.txtDataConsolidamentoFinale.Visible = false;
                                        break;

                                    case "3":
                                        this.txtDataConsolidamento.Visible = true;
                                        this.txtDataConsolidamento.Enabled = false;
                                        this.txtDataConsolidamentoFinale.Visible = true;
                                        this.txtDataConsolidamentoFinale.Visible = true;
                                        this.txtDataConsolidamentoFinale.Enabled = false;
                                        break;

                                    case "4":
                                        this.txtDataConsolidamento.Visible = true;
                                        this.txtDataConsolidamento.Enabled = false;
                                        this.txtDataConsolidamentoFinale.Visible = true;
                                        this.txtDataConsolidamentoFinale.Visible = true;
                                        this.txtDataConsolidamentoFinale.Enabled = false;
                                        break;
                                }
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.ID_UTENTE_CONSOLIDANTE.ToString())
                            {
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.ID_RUOLO_CONSOLIDANTE.ToString())
                            {
                            }
                            #endregion
                            #region filtro PROFILED DOCUMENT
                            // Ripristino filtro "Tipologia fascicoli"
                            else if (item.argomento == DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString())
                            {
                                SearchCorrespondentIntExtWithDisabled = true;
                                this.DocumentDdlTypeDocument.SelectedValue = item.valore;
                                //Verifico se esiste un diagramma di stato associato al tipo di documento
                                //Modifica per la visualizzazione solo degli stati per cui esistono documenti in essi
                                string idDiagramma = DiagrammiManager.getDiagrammaAssociato(this.DocumentDdlTypeDocument.SelectedValue).ToString();

                                if (this.DocumentDdlTypeDocument.Items.FindByValue(item.valore) != null)
                                {
                                    if (!string.IsNullOrEmpty(idDiagramma) && !idDiagramma.Equals("0"))
                                    {
                                        this.PnlStateDiagram.Visible = true;

                                        //Inizializzazione comboBox
                                        this.DocumentDdlStateDiagram.Items.Clear();
                                        ListItem itemEmpty = new ListItem();
                                        this.DocumentDdlStateDiagram.Items.Add(itemEmpty);

                                        DocsPaWR.Stato[] statiDg = DiagrammiManager.getStatiPerRicerca(idDiagramma, "D");
                                        foreach (Stato st in statiDg)
                                        {
                                            ListItem itemList = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                                            this.DocumentDdlStateDiagram.Items.Add(itemList);
                                        }

                                        this.ddlStateCondition.Visible = true;
                                        this.PnlStateDiagram.Visible = true;
                                    }
                                    else
                                    {
                                        this.ddlStateCondition.Visible = false;
                                        this.PnlStateDiagram.Visible = false;
                                    }
                                }

                                //this.PnlSearchDocTipology.Attributes.Remove("class");
                                //this.PnlSearchDocTipology.Attributes.Add("class", "collapse shown");
                                this.UpPnlTypeDocument.Update();
                            }

                            // Ripristino filtro "Tipologia fascicoli"
                            else if (item.argomento == DocsPaWR.FiltriDocumento.PROFILAZIONE_DINAMICA.ToString())
                            {
                                this.Template = item.template;
                                if (!this.ShowGridPersonalization)
                                {
                                    Session["templateRicerca"] = this.Template;
                                }
                                this.UpPnlTypeDocument.Update();

                            }
                            #endregion filtro PROFILED DOCUMENT

                            #region filtro DIAGRAMMI DI STATO
                            // Ripristino filtro "Diagramma Stato"
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DIAGRAMMA_STATO_DOC.ToString())
                            {
                                this.ddlStateCondition.Visible = true;
                                this.PnlStateDiagram.Visible = true;
                                this.ddlStateCondition.SelectedValue = item.nomeCampo;
                                this.DocumentDdlStateDiagram.SelectedValue = item.valore;
                                this.UpPnlTypeDocument.Update();
                            }
                            #endregion filtro DIAGRAMMI DI STATO
                            #region filtro NOTE
                            // Ripristino filtro "Diagramma Stato"
                            else if (item.argomento == DocsPaWR.FiltriDocumento.NOTE.ToString())
                            {
                                // ...splitta la stringa in argomento...
                                string[] info = utils.splittaStringaRicercaNote(item.valore);

                                // ...la prima posizione dell'array contiene il testo da ricercare...
                                this.Txtnote.Text = info[0];

                                //DocsPaWR.Registro[] registriRf = UserManager.getListaRegistriWithRF(RoleManager.GetRoleInSession().systemId, "1", "");
                                DocsPaWR.Registro[] registriRf = RegistryManager.GetRFListInSession();
                                //Se un ruolo appartiene a più di un RF, allora selezionando dal menù il valore RF
                                //l'utente deve selezionare su quale degli RF creare la nota
                                if (registriRf != null && registriRf.Length > 0)
                                {
                                    this.ddlNoteRF.Visible = true;
                                    if (registriRf.Length == 1)
                                    {
                                        ListItem item2 = new ListItem();
                                        item2.Value = registriRf[0].systemId;
                                        item2.Text = registriRf[0].codRegistro;
                                        this.ddlNoteRF.Items.Add(item2);
                                    }
                                    else
                                    {
                                        ListItem itemVuoto = new ListItem();
                                        itemVuoto.Value = "";
                                        itemVuoto.Text = Utils.Languages.GetLabelFromCode("DocumentNoteSelectAnRF", UIManager.UserManager.GetUserLanguage());
                                        this.ddlNoteRF.Items.Add(itemVuoto);
                                        foreach (DocsPaWR.Registro regis in registriRf)
                                        {
                                            ListItem item2 = new ListItem();
                                            item2.Value = regis.systemId;
                                            item2.Text = regis.codRegistro;
                                            this.ddlNoteRF.Items.Add(item2);
                                        }
                                    }

                                }

                                // ...la seconda contiene la tipologia di ricerca
                                this.rl_visibilita.SelectedValue = (info[1])[0].ToString();

                                if (this.ddlNoteRF.Visible && info != null && info.Length > 2)
                                {
                                    this.ddlNoteRF.SelectedValue = info[2].ToString();
                                }

                                this.UpNote.Update();
                            }
                            #endregion filtro NOTE

                            #region FIRMATI
                            else if (item.argomento == DocsPaWR.FiltriDocumento.FIRMATO.ToString())
                            {
                                switch (item.valore)
                                {
                                    case "0":
                                        this.chkFirmato.Checked = false;
                                        this.chkNonFirmato.Checked = true;
                                        break;
                                    case "1":
                                        this.chkFirmato.Checked = true;
                                        this.chkNonFirmato.Checked = false;
                                        break;
                                    case "2":
                                        this.chkFirmato.Checked = true;
                                        this.chkNonFirmato.Checked = true;
                                        break;
                                }
                            }
                            #endregion

                            #region FIRMA ELETTRONICA

                            else if (item.argomento == DocsPaWR.FiltriDocumento.FIRMA_ELETTRONICA.ToString())
                            {
                                this.chkFirmaElettronica.Checked = item.valore.Equals("1");
                                this.UpPnlElectronicSignature.Update();
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.ID_UTENTE_FIRMATARIO_ELETTRONICA.ToString())
                            {
                                if (!item.valore.Equals("0"))
                                {
                                    DocsPaWR.Corrispondente corr = UserManager.getCorrispondentBySystemID(item.valore);
                                    this.txtCodiceFirmatario.Text = corr.codiceRubrica;
                                    this.txtDescrizioneFirmatario.Text = corr.descrizione;
                                    this.idFirmatario.Value = corr.systemId;
                                    this.rblFirmatarioType.SelectedValue = corr.tipoCorrispondente;
                                }
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.ID_RUOLO_FIRMATARIO_ELETTRONICA.ToString())
                            {
                                DocsPaWR.Corrispondente corr = UserManager.getCorrispondentBySystemID(item.valore);
                                this.txtCodiceFirmatario.Text = corr.codiceRubrica;
                                this.txtDescrizioneFirmatario.Text = corr.descrizione;
                                this.idFirmatario.Value = corr.systemId;
                                this.rblFirmatarioType.SelectedValue = corr.tipoCorrispondente;
                            }
                            else if (item.argomento == DocsPaWR.FiltriDocumento.DESC_FIRMATARIO_ELETTRONICA.ToString())
                            {
                                this.txtDescrizioneFirmatario.Text = item.valore;
                            }

                            #endregion
                            #region CREATORE
                            else if (item.argomento == "ID_AUTHOR")
                            {
                                DocsPaWR.Corrispondente corr = UserManager.getCorrispondentBySystemID(item.valore);
                                this.txtCodiceCreatore.Text = corr.codiceRubrica;
                                this.txtDescrizioneCreatore.Text = corr.descrizione;
                                this.idCreatore.Value = corr.systemId;
                            }
                            else if (item.argomento == "CORR_TYPE_AUTHOR")
                            {
                                this.rblOwnerType.SelectedValue = item.valore;
                            }
                            else if (item.argomento == "EXTEND_TO_HISTORICIZED_AUTHOR")
                            {
                                bool tempX1 = false;
                                if (Boolean.TryParse(item.valore, out tempX1))
                                {
                                    this.chkCreatoreExtendHistoricized.Checked = tempX1;
                                }
                                else this.chkCreatoreExtendHistoricized.Checked = false;
                            }
                            else if (item.argomento == "DESC_AUTHOR")
                            {
                                this.txtDescrizioneCreatore.Text = item.valore;
                            }
                            #endregion

                            #region PROPRIETARIO
                            else if (item.argomento == "ID_OWNER")
                            {
                                DocsPaWR.Corrispondente corr = UserManager.getCorrispondentBySystemID(item.valore);
                                this.txtCodiceProprietario.Text = corr.codiceRubrica;
                                this.txtDescrizioneProprietario.Text = corr.descrizione;
                                this.idProprietario.Value = corr.systemId;
                            }
                            else if (item.argomento == "CORR_TYPE_OWNER")
                            {
                                this.rblProprietarioType.SelectedValue = item.valore;
                            }
                            else if (item.argomento == "EXTEND_TO_HISTORICIZED_OWNER")
                            {
                                // non è presente la checkbox di proprietario storicizzato.
                            }
                            else if (item.argomento == "DESC_OWNER")
                            {
                                this.txtDescrizioneProprietario.Text = item.valore;
                            }
                            #endregion

                            #region MAI TRASMESSI DA

                            if(item.argomento == DocsPaWR.FiltriDocumento.DOC_MAI_TRASMESSI_DA_RUOLO.ToString())
                            {
                                if(!string.IsNullOrEmpty(item.valore))
                                    this.rb_roleUserTrasm.Selected = true;
                            }

                            if (item.argomento == DocsPaWR.FiltriDocumento.DOC_MAI_TRASMESSI_DA_UTENTE.ToString())
                            {
                                if (!string.IsNullOrEmpty(item.valore))
                                    this.rb_onlyUserTrasm.Selected = true;
                            }
                            #endregion

                            #region MAI SPEDITI

                            if (item.argomento == DocsPaWR.FiltriDocumento.DOC_MAI_SPEDITI.ToString())
                            {
                                this.cb_neverSend.Checked = true;
                                this.PnlNeverSendFrom.CssClass = "";
                            }

                            if (item.argomento == DocsPaWR.FiltriDocumento.DOC_MAI_SPEDITI_DA_RUOLO.ToString())
                            {
                                this.cb_neverSend.Checked = true;
                                this.rb_roleUser.Selected = true;
                                this.PnlNeverSendFrom.CssClass = "";
                            }

                            if (item.argomento == DocsPaWR.FiltriDocumento.DOC_MAI_SPEDITI_DA_UTENTE.ToString())
                            {
                                this.cb_neverSend.Checked = true;
                                this.rb_onlyUser.Selected = true;
                                this.PnlNeverSendFrom.CssClass = "";
                            }
                            #endregion
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception("I criteri di ricerca non sono piu\' validi.");
                    }
                }
                catch (System.Exception ex)
                {
                    ErrorManager.redirect(this, ex);
                }
            }
        }

        protected void addAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.gridViewResult.HeaderRow.FindControl("cb_selectall") != null)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                    if (this.IdProfileList != null)
                    {
                        bool value = ((CheckBox)this.gridViewResult.HeaderRow.FindControl("cb_selectall")).Checked;
                        for (int i = 0; i < this.IdProfileList.Length; i++)
                        {
                            if (value)
                            {

                                if (!this.ListCheck.ContainsKey(this.IdProfileList[i]))
                                {
                                    this.ListCheck.Add(this.IdProfileList[i], this.CodeProfileList[i]);
                                }
                            }
                            else
                            {
                                if (this.ListCheck.ContainsKey(this.IdProfileList[i]))
                                {
                                    this.ListCheck.Remove(this.IdProfileList[i]);
                                }
                            }
                        }

                        this.CheckAll = value;

                        foreach (GridViewRow dgItem in this.gridViewResult.Rows)
                        {
                            CheckBox checkBox = dgItem.FindControl("checkDocumento") as CheckBox;
                            checkBox.Checked = value;
                        }

                        if (this.CheckAll)
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "clearCheckboxes", "clearCheckboxes('true', '');", true);

                        if (this.CheckAll)
                            this.HiddenItemsAll.Value = "true";
                        else
                            this.HiddenItemsAll.Value = string.Empty;
                        this.upPnlButtons.Update();
                    }

                    this.UpnlGrid.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
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

                    foreach (string item in items)
                    {
                        string key = item.Split('_')[0];
                        string value = item.Split('_')[1].Replace("<span style=\"color:Black;\">", "").Replace("</span>", "");
                        if (!this.ListCheck.ContainsKey(key))
                            this.ListCheck.Add(key, value);
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
                        string value = item.Split('_')[1];
                        if (this.ListCheck.ContainsKey(key))
                            this.ListCheck.Remove(key);
                    }
                }


                if (string.IsNullOrEmpty(this.HiddenItemsAll.Value))
                {
                    string js = string.Empty;
                    foreach (KeyValuePair<string, string> d in this.ListCheck)
                    {
                        if (!string.IsNullOrEmpty(js)) js += ",";
                        js += d.Key;
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "clearCheckboxes", "clearCheckboxes('false', '" + js + "');", true);
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


        /// <summary>
        /// INTEGRAZIONE PITRE-PARER
        /// Verifica che il numero di documenti selezionati sia inferiore al limite definito nella chiave
        /// </summary>
        /// <returns>True se il numero di doc supera il limite</returns>
        private bool checkLimitDocVersamento()
        {
            bool result = false;

            // Controllo se la chiave è definita
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_DOC_VERSAMENTO.ToString())))
            {
                int limit;
                // Se non riesco ad estrarre il valore (es. chiave definita con valori non numerici) restituisco comunque false
                if (Int32.TryParse(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_DOC_VERSAMENTO.ToString()), out limit))
                {
                    if (this.ListCheck.Count > limit)
                        result = true;
                    else
                        result = false;
                }
                else
                    result = false;
            }
            else
            {
                result = false;
            }

            return result;
        }

        #endregion

        #region Filtro tipo repertorio

        protected void ddl_numRep_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (this.ddl_numRep.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initNumRep.ReadOnly = false;
                        this.txt_fineNumRep.Visible = false;
                        this.LtlANumRep.Visible = false;
                        this.LtlDaNumRep.Visible = false;
                        this.txt_fineNumRep.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initNumRep.ReadOnly = false;
                        this.txt_fineNumRep.ReadOnly = false;
                        this.LtlANumRep.Visible = true;
                        this.LtlDaNumRep.Visible = true;
                        this.txt_fineNumRep.Visible = true;
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dataRepertorio_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dataRepertorio.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.txt_initDataRep.ReadOnly = false;
                        this.txt_fineDataRep.Visible = false;
                        this.LtlADataRep.Visible = false;
                        this.LtlDaDataRep.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.txt_initDataRep.ReadOnly = false;
                        this.txt_fineDataRep.ReadOnly = false;
                        this.LtlADataRep.Visible = true;
                        this.LtlDaDataRep.Visible = true;
                        this.txt_fineDataRep.Visible = true;
                        this.LtlDaDataRep.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataRep.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        this.LtlADataRep.Visible = false;
                        this.txt_fineDataRep.Visible = false;
                        this.txt_initDataRep.ReadOnly = true;
                        this.txt_initDataRep.Text = NttDataWA.Utils.dateformat.toDay();
                        this.LtlDaDataRep.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.txt_fineDataRep.Text = string.Empty;
                        break;
                    case 3: //Settimana corrente
                        this.LtlADataRep.Visible = true;
                        this.txt_fineDataRep.Visible = true;
                        this.txt_initDataRep.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.txt_fineDataRep.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.txt_fineDataRep.ReadOnly = true;
                        this.txt_initDataRep.ReadOnly = true;
                        this.LtlDaDataRep.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataRep.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        this.LtlADataRep.Visible = true;
                        this.txt_fineDataRep.Visible = true;
                        this.txt_initDataRep.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.txt_fineDataRep.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.txt_fineDataRep.ReadOnly = true;
                        this.txt_initDataRep.ReadOnly = true;
                        this.LtlDaDataRep.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.LtlADataRep.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
        #endregion
    }
}
