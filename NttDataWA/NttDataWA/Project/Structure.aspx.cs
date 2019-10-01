using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using NttDatalLibrary;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;


namespace NttDataWA.Project
{
    public partial class Structure : System.Web.UI.Page
    {

        #region Properties

        /// <summary>
        ///  document file to be show
        /// </summary>
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

        private FiltroRicerca[][] FiltroRicerca
        {
            get
            {
                return (FiltroRicerca[][])HttpContext.Current.Session["FiltroRicerca"];
            }
            set
            {
                HttpContext.Current.Session["FiltroRicerca"] = value;
            }
        }

        private SearchObject[] Result
        {
            get
            {
                return HttpContext.Current.Session["Result"] as SearchObject[];
            }
            set
            {
                HttpContext.Current.Session["Result"] = value;
            }
        }

        private SearchObject[] SearchResult
        {
            get
            {
                return HttpContext.Current.Session["SearchResult"] as SearchObject[];
            }
            set
            {
                HttpContext.Current.Session["SearchResult"] = value;
            }
        }

        private string JsOnFolders
        {
            get
            {
                return HttpContext.Current.Session["JsOnFolders"] as string;
            }
            set
            {
                HttpContext.Current.Session["JsOnFolders"] = value;
            }
        }

        private RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_RICERCA_ESTESA;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }
        }

        private int PageSizeStructure
        {
            get
            {
                if (HttpContext.Current.Session["PageSizeStructure"] != null)
                    return (int)HttpContext.Current.Session["PageSizeStructure"];
                else 
                    return 0;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializePage();
                }
                else
                {
                    this.setValuePopUp();
                }

                this.RefreshScripts();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void setValuePopUp()
        {
            if (!string.IsNullOrEmpty(this.SearchSubset.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "searchTree", "searchTree('" + utils.FormatJs(this.SearchSubset.ReturnValue) + "','');", true);
            }
        }

        protected void InitializePage()
        {
            Fascicolo fascicolo = null;
            HttpContext.Current.Session.Remove("PageSizeStructure");
            //Back
            if (this.Request.QueryString["back"] != null && this.Request.QueryString["back"].Equals("1"))
            {
                List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                Navigation.NavigationObject obj = navigationList.Last();
                if (!obj.CodePage.Equals(Navigation.NavigationUtils.NamePage.PROJECT_STRUCTURE.ToString()))
                {
                    obj = new Navigation.NavigationObject();
                    obj = navigationList.ElementAt(navigationList.Count - 2);
                }

                fascicolo = UIManager.ProjectManager.getFascicoloById(obj.idProject);

            }
            else
            {
                fascicolo = UIManager.ProjectManager.getProjectInSession();
            }

            fascicolo.folderSelezionato = ProjectManager.getFolder(this, fascicolo);
            fascicolo.template = fascicolo.template = ProfilerProjectManager.getTemplateFascDettagli(fascicolo.systemID);
            UIManager.ProjectManager.setProjectInSession(fascicolo);

            this.InitializeLabel();
            this.InitInitiallyOpenTree();
            this.InitializeAddressBooks();
        }

        private void InitInitiallyOpenTree()
        {
            Fascicolo Fasc = ProjectManager.getProjectInSession();
            DocsPaWR.Folder folder = ProjectManager.getFolder(this, Fasc);
            this.jstree_initially_open.Text = "\"root_" + folder.systemID + "\"";
            this.treenode_sel.Value = "root_" + folder.systemID;
        }

        private void InitializeLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.projectBtnSearch.Text = Utils.Languages.GetLabelFromCode("GenericBtnSearch", language);
            this.projectBtnRemoveFilters.Text = Utils.Languages.GetLabelFromCode("projectBtnRemoveFilters", language);
            this.litTreeExpandAll.Text = Utils.Languages.GetLabelFromCode("projectTreeExpandAll", language);
            this.litTreeCollapseAll.Text = Utils.Languages.GetLabelFromCode("projectTreeCollapseAll", language);
            this.lbl_dtCreation.Text = Utils.Languages.GetLabelFromCode("projectDateCreation", language);
            this.lbl_subject.Text = Utils.Languages.GetLabelFromCode("projectSubject", language);
            this.lbl_recipient.Text = Utils.Languages.GetLabelFromCode("projectRecipientSender", language);
            this.LnkViewDocument.Text = Utils.Languages.GetLabelFromCode("projectLnkViewDocument", language);
            this.ImgFolderSearch.ToolTip = Utils.Languages.GetLabelFromCode("projectImgFolderSearch", language);
            this.ImgFolderSearch.AlternateText = Utils.Languages.GetLabelFromCode("projectImgFolderSearch", language);
            this.litConfirmMoveFolder.Text = Utils.Languages.GetLabelFromCode("projectConfirmMoveFolder", language);
            this.litConfirmMoveDocuments.Text = Utils.Languages.GetLabelFromCode("projectConfirmMoveDocuments", language);
            this.litTreeAlertOperationNotAllowed.Text = Utils.Languages.GetLabelFromCode("ProjectTreeAlertOperationNotAllowed", language);
            this.litTreeLoading.Text = Utils.Languages.GetLabelFromCode("ProjectTreeLoading", language);
            this.SearchSubset.Title = Utils.Languages.GetLabelFromCode("ProjectSearchSubsetTitle", language);
            this.rblRecipientType.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("TransmissionRblRecipientTypeSender", language), "M"));
            this.rblRecipientType.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("TransmissionRblRecipientTypeRecipient", language), "D"));
            this.rblRecipientType.Items[0].Selected = true;
        }

        protected void InitializeAddressBooks()
        {
            this.SetAjaxAddressBook();
        }

        protected void SetAjaxAddressBook()
        {
            //if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.RUBRICAVELOCE_MINIMUMPREFIXLENGTH.ToString()]))
            //    this.RapidRecipient.MinimumPrefixLength = int.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.RUBRICAVELOCE_MINIMUMPREFIXLENGTH.ToString()]);

            string dataUser = UIManager.RoleManager.GetRoleInSession().systemId;

            Fascicolo prj = UIManager.ProjectManager.getProjectInSession();
            Registro reg = UIManager.RegistryManager.getRegistroBySistemId(prj.idRegistroNodoTit);
            if (reg == null)
            {
                reg = UIManager.RegistryManager.getRegistroBySistemId(prj.idRegistro);
            }
            if (reg == null)
            {
                reg = UIManager.RoleManager.GetRoleInSession().registri[0];
            }
            dataUser = dataUser + "-" + reg.systemId;

            string callType = "CALLTYPE_RICERCA_ESTESA";
            this.RapidRecipient.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
        }

        private void RefreshScripts()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "Tipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            try {
                CustomTextArea caller = sender as CustomTextArea;
                string codeAddressBook = this.TxtCodeRecipient.Text;

                if (!string.IsNullOrEmpty(codeAddressBook))
                {
                    this.SearchCorrespondent(codeAddressBook, caller.ID);
                }
                else
                {
                    this.TxtCodeRecipient.Text = string.Empty;
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

        protected RubricaCallType GetCallType(string idControl)
        {
            RubricaCallType calltype = DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_ESTESA;
            return calltype;
        }

        protected void SearchCorrespondent(string addressCode, string idControl)
        {
            try {
                RubricaCallType calltype = this.GetCallType(idControl);
                DocsPaWR.Corrispondente corr = UIManager.AddressBookManager.getCorrispondenteRubrica(addressCode, calltype);
                if (corr == null)
                {
                    this.TxtCodeRecipient.Text = string.Empty;
                    this.TxtDescriptionRecipient.Text = string.Empty;
                    this.IdRecipient.Value = string.Empty;

                    string msg = "ErrorTransmissionCorrespondentNotFound";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                }
                else
                {
                    this.TxtCodeRecipient.Text = corr.codiceRubrica;
                    this.TxtDescriptionRecipient.Text = corr.descrizione;
                    this.IdRecipient.Value = corr.systemId;
                }

                this.UpPnlRecipient.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnChangeSelectedDocument_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.treenode_sel.Value))
            {
                Session["IsInvokingFromProjectStructure"] = true;

                SchedaDocumento doc = DocumentManager.getDocumentDetails(this, this.treenode_sel.Value, this.treenode_sel.Value);

                DocumentManager.setSelectedRecord(doc);
                FileRequest req = FileManager.GetFileRequest();
                FileManager.setSelectedFile(req);

                this.ViewDocument.Page_Load(null, null);
                this.unPnlVD.Visible = true;
                this.unPnlViewDocument.Update();
            }
            else
            {
                DocumentManager.setSelectedRecord(null);
                this.unPnlVD.Visible = false;
                this.unPnlViewDocument.Update();
            }
        }

        protected void LnkViewDocument_Click(object sender, EventArgs e)
        {
                Session["IsInvokingFromProjectStructure"] = null;
                SchedaDocumento schedaDocumento = UIManager.DocumentManager.getSelectedRecord();
                Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
                //Session["isZoom"] = null;
                List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                Navigation.NavigationObject obj = navigationList.Last();
                Navigation.NavigationObject newObj = Navigation.NavigationUtils.CloneObject(obj);

                newObj.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT_STRUCTURE.ToString(), string.Empty);
                newObj.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT_STRUCTURE.ToString(), true,this.Page);
                newObj.CodePage = Navigation.NavigationUtils.NamePage.PROJECT_STRUCTURE.ToString();
                newObj.Page = "STRUCTURE.ASPX";
                newObj.IdObject = schedaDocumento.systemId;
                newObj.OriginalObjectId = schedaDocumento.systemId;
                newObj.folder = fascicolo.folderSelezionato;
                newObj.idProject = fascicolo.systemID;
                //int indexElement = ((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1);
                //newObj.DxPositionElement = indexElement.ToString();

                if (obj.NamePage.Equals(Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT_STRUCTURE.ToString(), string.Empty)) && !string.IsNullOrEmpty(obj.IdObject) && obj.IdObject.Equals(newObj.idProject))
                {
                    navigationList.Remove(obj);
                }
                navigationList.Add(newObj);
                Navigation.NavigationUtils.SetNavigationList(navigationList);

                Response.Redirect("../Document/document.aspx");
        }

        protected void DocumentImgSenderAddressBook_Click(object sender, EventArgs e)
        {
            try {
                this.CallType = RubricaCallType.CALLTYPE_RICERCA_ESTESA;
                HttpContext.Current.Session["AddressBook.from"] = "D_R_X_S";
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
            try {
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
                    this.TxtCodeRecipient.Text = tempCorrSingle.codiceRubrica;
                    this.TxtDescriptionRecipient.Text = tempCorrSingle.descrizione;
                    this.IdRecipient.Value = tempCorrSingle.systemId;
                    this.UpPnlRecipient.Update();
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

        protected void projectBtnSearch_Click(object sender, EventArgs e)
        {
            try {
                ArrayList filterItems = new ArrayList();
                this.AddFilterTipoDocumento(filterItems);
                this.AddFilterDataCreazioneDocumento(filterItems);
                this.AddFilterOggettoDocumento(filterItems);
                this.AddFilterMittDestDocumento(filterItems);

                DocsPaWR.FiltroRicerca[] initArray = new DocsPaWR.FiltroRicerca[filterItems.Count];
                filterItems.CopyTo(initArray);

                DocsPaWR.FiltroRicerca[][] searchFilters = new DocsPaWR.FiltroRicerca[1][];
                searchFilters[0] = initArray;

                Fascicolo Fasc = ProjectManager.getProjectInSession();
                DocsPaWR.Folder folder = ProjectManager.getFolder(this, Fasc);
                this.SearchResult = null;
                this.JsOnFolders = string.Empty;
                this.GetSubFolders(searchFilters, folder);

                if (this.JsOnFolders.IndexOf(";") > 0)
                {
                    string jsOnTemp = string.Empty;
                    foreach (string jsTemp in this.JsOnFolders.Split(';'))
                    {
                        int indexof = jsOnTemp.IndexOf(jsTemp.Trim());
                        if (jsOnTemp.IndexOf(jsTemp.Trim()) < 0)
                        {
                            if (jsOnTemp.Length > 0) jsOnTemp += ", ";
                            jsOnTemp += jsTemp.Trim();
                        }
                    }
                    this.JsOnFolders = jsOnTemp;
                }

                if (this.SearchResult != null)
                {
                    string jsOn = this.JsOnFolders;
                    foreach (SearchObject obj in this.SearchResult)
                    {
                        if (jsOn.IndexOf("doc_" + obj.SearchObjectField[0].SearchObjectFieldValue) < 0)
                        {
                            if (jsOn.Length > 0) jsOn += ", ";
                            jsOn += "{ \"id\": \"doc_" + obj.SearchObjectField[0].SearchObjectFieldValue + "\" , \"isResult\": \"true\" } ";
                        }
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "searchTreeDocuments", "searchTreeDocuments('[" + jsOn + "]')", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void projectBtnRemoveFilters_Click(object sender, EventArgs e)
        {
            try {
                this.txtDate.Text = string.Empty;
                this.txtSubject.Text = string.Empty;
                this.IdRecipient.Value = string.Empty;
                this.TxtCodeRecipient.Text = string.Empty;
                this.TxtDescriptionRecipient.Text = string.Empty;
                this.unPnlFilters.Update();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "resetTreeHighlight", "$('#tree').find('.jstree-search').removeClass('jstree-search');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void GetSubFolders(FiltroRicerca[][] searchFilters, Folder folder)
        {
            try {
                int recordNum = this.GetDocuments(searchFilters, folder);

                if (recordNum > 0)
                {
                    Fascicolo fascSelezionato = ProjectManager.getProjectInSession();
                    if (fascSelezionato.folderSelezionato.idParent != folder.systemID)
                    {
                        Folder tempFolder = folder;
                        while (fascSelezionato.folderSelezionato.idParent != tempFolder.idParent)
                        {
                            if (this.JsOnFolders.Length > 0)
                            {
                                this.JsOnFolders = "	{ \"id\": \"node_" + tempFolder.systemID + "\" , \"isResult\": \"false\" }; " + this.JsOnFolders;
                            }
                            else
                            {
                                this.JsOnFolders = "	{ \"id\": \"node_" + tempFolder.systemID + "\" , \"isResult\": \"false\" } " + this.JsOnFolders;
                            }
                            tempFolder = ProjectManager.getFolder(this, tempFolder.idParent);
                        }
                    }
                }

                for (int i = 0; i < folder.childs.Length; i++)
                    this.GetSubFolders(searchFilters, folder.childs[i]);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void AddFilterTipoDocumento(ArrayList filterItems)
        {
            try {
                DocsPaWR.FiltroRicerca filterItem = new DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                filterItem.valore = "T";
                filterItems.Add(filterItem);
                filterItem = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void AddFilterDataCreazioneDocumento(ArrayList filterItems)
        {
            try {
                if (this.txtDate.Text.Length > 0)
                {
                    DocsPaWR.FiltroRicerca filterItem = new DocsPaWR.FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
                    filterItem.valore = this.txtDate.Text;
                    filterItems.Add(filterItem);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void AddFilterOggettoDocumento(ArrayList filterItems)
        {
            try {
                if (this.txtSubject.Text.Length > 0)
                {
                    DocsPaWR.FiltroRicerca filterItem = new DocsPaWR.FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.OGGETTO.ToString();
                    filterItem.valore = this.txtSubject.Text;
                    filterItems.Add(filterItem);
                    filterItem = null;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void AddFilterMittDestDocumento(ArrayList filterItems)
        {
            try {
                if (this.IdRecipient.Value.Length > 0)
                {
                    DocsPaWR.FiltroRicerca filterItem = new DocsPaWR.FiltroRicerca();
                    if (this.rblRecipientType.SelectedValue=="M")
                        filterItem.argomento = DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString();
                    else
                        filterItem.argomento = DocsPaWR.FiltriDocumento.ID_DESTINATARIO.ToString();
                    filterItem.valore = this.IdRecipient.Value;
                    filterItems.Add(filterItem);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private int GetDocuments(FiltroRicerca[][] searchFilters, Folder folder)
        {
            try {
                bool showGridPersonalization = UIManager.GridManager.EnableCustomGrid(UIManager.RoleManager.GetRoleInSession());

                int recordNumber = 0;
                int selectedPage = 1;
                int pageNumbers = 0;

                Grid selectedGrid = GridManager.GetStandardGridForUser(GridTypeEnumeration.Document, UserManager.GetInfoUser());
                FiltroRicerca[][] orderFilters = UIManager.GridManager.GetFiltriOrderRicerca(selectedGrid);
                SearchObject[] result = this.SearchDocument(searchFilters, selectedPage, out pageNumbers, out recordNumber, folder, orderFilters, showGridPersonalization, selectedGrid);
                if (this.SearchResult == null)
                    this.SearchResult = result;
                else
                {
                    SearchObject[] temp = this.SearchResult;
                    int array1OriginalLength = temp.Length;
                    Array.Resize<SearchObject>(ref temp, array1OriginalLength + result.Length);
                    Array.Copy(result, 0, temp, array1OriginalLength, result.Length);
                    this.SearchResult = temp;
                }

                return recordNumber;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return 0;
            }
        }

        private SearchObject[] SearchDocument(FiltroRicerca[][] searchFilters,
            int selectedPage, out int pageNumbers, out  int recordNumber, Folder folder,
            FiltroRicerca[][] orderFilters, bool showGridPersonalization, Grid SelectedGrid)
        {
            try {
                // Documenti individuati dalla ricerca
                SearchObject[] toReturn;

                // Recupero dei campi della griglia impostati come visibili
                Field[] visibleArray = null;
                List<Field> visibleFieldsTemplate;

                // Lista dei system id dei documenti restituiti dalla ricerca
                SearchResultInfo[] idProfiles = null;

                visibleFieldsTemplate = SelectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field)) && e.CustomObjectId != 0).ToList();
                if (visibleFieldsTemplate != null && visibleFieldsTemplate.Count > 0)
                    visibleArray = visibleFieldsTemplate.ToArray();
                int pageSize = (PageSizeStructure > 0) ? PageSizeStructure : 20;
                toReturn = UIManager.ProjectManager.getListaDocumentiPagingCustom(
                      folder,
                      searchFilters,
                      selectedPage,
                      out pageNumbers,
                      out recordNumber,
                      !IsPostBack,
                      showGridPersonalization,
                      false,
                      visibleArray,
                      null,
                      pageSize,
                      orderFilters,
                      out idProfiles);

                // Restituzione della lista di documenti da visualizzare
                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                pageNumbers = 0;
                recordNumber = 0;
                return new SearchObject[1];
            }
        }

        protected string GetStateOfTheProject()
        {
            return ProjectManager.getProjectInSession().stato;
        }

    }
}
