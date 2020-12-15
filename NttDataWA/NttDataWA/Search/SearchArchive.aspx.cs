using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using System.Data;
using NttDataWA.Utils;
using NttDatalLibrary;


namespace NttDataWA.Search
{
    public partial class SearchArchive : System.Web.UI.Page
    {
        #region Fields

        private ILog logger = LogManager.GetLogger(typeof(ObjectManager));
        private const string UP_DOCUMENT_BUTTONS = "upPnlButtons";
        private const string CLOSE_POPUP_ZOOM = "closeZoom";

        #endregion

        #region Properties

        private SearchObject[] ListObjectNavigation
        {
            set
            {
                HttpContext.Current.Session["ListObjectNavigation"] = value;
            }
        }

        /// <summary>
        /// Classificazione da utilizzare per la ricerca fascicoli
        /// </summary>
        public FascicolazioneClassificazione Classification
        {
            get
            {
                return HttpContext.Current.Session["classification"] as FascicolazioneClassificazione;
            }

            set
            {
                HttpContext.Current.Session["classification"] = value;
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

        public FiltroRicerca[][] SearchFilters
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

        public FiltroRicerca[][] SearchDocFilters
        {
            get
            {
                return (FiltroRicerca[][])HttpContext.Current.Session["SearchDocFilters"];
            }
            set
            {
                HttpContext.Current.Session["SearchDocFilters"] = value;
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

        /// <summary>
        /// Risultati restituiti dalla ricerca fascicoli
        /// </summary>
        public SearchObject[] SearchResult
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

        private Folder FascFolder
        {
            get
            {
                return HttpContext.Current.Session["FascFolder"] as DocsPaWR.Folder;
            }
            set
            {
                HttpContext.Current.Session["FascFolder"] = value;
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

        private bool ShowGridPersonalization
        {
            get
            {
                bool result = false;
                return result;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    this.InitializePage();
                }

                //rimuovo IsZoom alla chiusura della popup di zoom(da x o pulsante chiudi) quando richiamata da profilo, allegato, classifica
                if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_DOCUMENT_BUTTONS))
                {
                    if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_ZOOM)))
                    {
                        this.RemoveIsZoom();
                        this.UpnlAzioniMassive.Update();
                        this.UpnlTabHeader.Update();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('DocumentViewer','');", true);
                        return;
                    }
                }

                if (this.Result != null && this.Result.Length > 0 && this.RecordCount > 0)
                {
                    // Visualizzazione dei risultati

                    // Lista dei documenti risultato della ricerca
                    if (!this.SelectedPage.ToString().Equals(this.grid_pageindex.Value) && UIManager.ProjectManager.getProjectInSession() != null)
                    {
                        this.Result = null;
                        this.SelectedRow = string.Empty;
                        if (!string.IsNullOrEmpty(this.grid_pageindex.Value))
                        {
                            this.SelectedPage = int.Parse(this.grid_pageindex.Value);
                        }
                        this.SearchDocumentsAndDisplayResult(this.SearchDocFilters, SelectedPage, GridManager.GetStandardGridForUser(GridTypeEnumeration.Document, UserManager.GetInfoUser()), this.Labels.ToArray(), UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.GetStandardGridForUser(GridTypeEnumeration.Document, UserManager.GetInfoUser())), true);
                        this.BuildGridNavigator();
                        this.UpnlNumerodocumenti.Update();
                        this.UpnlGrid.Update();
                        this.upPnlGridIndexes.Update();
                    }
                    else
                    {
                        this.ShowResult(GridManager.GetStandardGridForUser(GridTypeEnumeration.Document, UserManager.GetInfoUser()), this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
                    }
                }

                this.RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddlRegistri_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DocsPaWR.Utente user = UIManager.UserManager.GetUserInSession();
                DocsPaWR.Ruolo role = UIManager.RoleManager.GetRoleInSession();
                this.PopulateDDLTitolario();
                DocsPaWR.OrgTitolario titolario = UIManager.ClassificationSchemeManager.GetTitolarioInSession();
                string IdRegistro = UIManager.RegistryManager.GetRegistryInSession().systemId;
                if (ddlRegistri.Visible)
                    IdRegistro = ddlRegistri.SelectedValue;
                UIManager.RegistryManager.GetRegistryInSession();
                this.CaricamentoTitolario(user.idAmministrazione, "0", role.idGruppo, IdRegistro, titolario.ID);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddlTitolario_SelectedIndexChanged(object sender, EventArgs e)
        {
            OrgTitolario titolario = ClassificationSchemeManager.getTitolario(this.ddlTitolario.SelectedValue);
            UIManager.ClassificationSchemeManager.SetTitolarioInSession(titolario);
            this.Result = null;
            this.PageCount = 1;
            this.BuildGridNavigator();
            this.InitSearch();
        }

        protected void ExpandeTreeView(object sender, TreeNodeEventArgs e)
        {
            try
            {
                string idregistro = UIManager.RegistryManager.GetRegistryInSession().systemId;
                if (this.ddlRegistri.Visible)
                    idregistro = ddlRegistri.SelectedValue;
                string idAmministrazione = UIManager.UserManager.GetUserInSession().idAmministrazione;
                string idgruppo = UIManager.RoleManager.GetRoleInSession().idGruppo;
                string idTitolario = UIManager.ClassificationSchemeManager.GetTitolarioInSession().ID;
                e.Node.ChildNodes.Clear();

                if (string.IsNullOrEmpty(((myTreeNode)e.Node).TIPO))
                    UIManager.ClassificationSchemeManager.LoadTreeViewChild(e.Node, idregistro, idAmministrazione, idgruppo, idTitolario);

                if (((myTreeNode)e.Node).ID != "T")
                    this.LoadProjectNodes(e.Node);

                this.HighlightNodesFound(e.Node);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        protected void TreeTitolario_SelectedNodeChanged(object sender, EventArgs e)
        {
            try
            {
                if (TreeTitolario != null && TreeTitolario.SelectedNode != null && ((myTreeNode)TreeTitolario.SelectedNode).CODICE != "T")
                {
                    this.SearchFilters = null;
                    this.SelectedPage = 1;
                    this.plcNavigator.Controls.Clear();


                    this.plcGoFascicolo.Visible = false;
                    if (((myTreeNode)TreeTitolario.SelectedNode).TIPO == "P" || ((myTreeNode)TreeTitolario.SelectedNode).TIPO == "S") this.plcGoFascicolo.Visible = true;
                    this.UpnlNumerodocumenti.Update();


                    // reset highlight of previous node if present
                    /*
                                        if (!string.IsNullOrEmpty(this.TreeTitolario_SelectedNode.Value) && this.TreeTitolario_SelectedNode.Value.Split('|')[2] == "1")
                                        {
                                            TreeNode prevNode = this.TreeTitolario.FindNode(this.TreeTitolario_SelectedNode.Value.Split('|')[0]);
                                            TreeNode prevNode2 = this.TreeTitolario.FindNode(this.TreeTitolario_SelectedNode.Value.Split('|')[0].Replace(this.TreeTitolario_SelectedNode.Value.Split('|')[1], "<span style=\"background: #115A8D; color: #fff;\">" + this.TreeTitolario_SelectedNode.Value.Split('|')[1] + "</span>"));
                                            if (prevNode != null) prevNode.Text = "<span style=\"background: #115A8D; color: #fff;\">" + this.TreeTitolario_SelectedNode.Value.Split('|')[1] + "</span>";
                                        }
                                        this.TreeTitolario_SelectedNode.Value = this.TreeTitolario.SelectedNode.ValuePath.Replace(this.TreeTitolario.SelectedNode.Text, utils.FWithoutHtml(this.TreeTitolario.SelectedNode.Text)) + "|" + utils.FWithoutHtml(this.TreeTitolario.SelectedNode.Text);
                                        if (this.TreeTitolario.SelectedNode.Text.IndexOf("<span ") >= 0)
                                        {
                                            this.TreeTitolario_SelectedNode.Value += "|1";
                                            this.TreeTitolario.SelectedNode.Text = utils.FWithoutHtml(this.TreeTitolario.SelectedNode.Text);
                                        }
                                        else
                                        {
                                            this.TreeTitolario_SelectedNode.Value += "|0";
                                        }
                                        this.upPnlButtons.Update();
                     */


                    Folder folder = new Folder();
                    if (string.IsNullOrEmpty(((myTreeNode)TreeTitolario.SelectedNode).TIPO) || ((myTreeNode)TreeTitolario.SelectedNode).TIPO == "P")
                    {
                        //Fascicolo project = UIManager.ProjectManager.GetProjectByCode(this.Registry, ((myTreeNode)TreeTitolario.SelectedNode).CODICE);
                        Fascicolo project = UIManager.ProjectManager.GetProjectByCodeRedAndClassScheme(this.Registry, ((myTreeNode)TreeTitolario.SelectedNode).CODICE, this.ddlTitolario.SelectedValue);
                        folder = ProjectManager.getFolder(this, project);
                        folder = ProjectManager.getFolder(this, folder);

                        project.folderSelezionato = folder;
                        UIManager.ProjectManager.setProjectInSession(project);
                    }
                    else
                    {
                        folder = ProjectManager.getFolder(this, ((myTreeNode)TreeTitolario.SelectedNode).IDRECORD);
                        folder = ProjectManager.getFolder(this, folder);

                        Fascicolo project = UIManager.ProjectManager.getProjectInSession();
                        if (project == null)
                        {
                            //project = UIManager.ProjectManager.GetProjectByCode(this.Registry, ((myTreeNode)TreeTitolario.SelectedNode).CODICE);
                            project = UIManager.ProjectManager.GetProjectByCodeRedAndClassScheme(this.Registry, ((myTreeNode)TreeTitolario.SelectedNode).CODICE, this.ddlTitolario.SelectedValue);
                        }
                        project.folderSelezionato = folder;
                        UIManager.ProjectManager.setProjectInSession(project);
                    }

                    this.SearchDocumentsAndDisplayResult(this.SearchDocFilters, SelectedPage, GridManager.GetStandardGridForUser(GridTypeEnumeration.Document, UserManager.GetInfoUser()), this.Labels.ToArray(), UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.GetStandardGridForUser(GridTypeEnumeration.Document, UserManager.GetInfoUser())), true);
                }
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

                    //imagini delle icone
                    ((CustomImageButton)e.Row.FindControl("conservazione")).Visible = false;
                    ((CustomImageButton)e.Row.FindControl("adlrole")).Visible = false;
                    ((CustomImageButton)e.Row.FindControl("adl")).Visible = false;
                    ((CustomImageButton)e.Row.FindControl("firmato")).Visible = false;

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

                    //evento click
                    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).Click += new ImageClickEventHandler(ImageButton_Click);
                    ((CustomImageButton)e.Row.FindControl("estensionedoc")).Click += new ImageClickEventHandler(ImageButton_Click);
                    //tooltip
                    ((CustomImageButton)e.Row.FindControl("estensionedoc")).ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateEstensioneDoc", UIManager.UserManager.GetUserLanguage()) + " " + estensione;
                    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateVisualizzaDocumento", UIManager.UserManager.GetUserLanguage());
                    ((CustomImageButton)e.Row.FindControl("eliminadocumento")).Visible = false;
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
                case "visualizzadocumento":
                    {
                        HttpContext.Current.Session["isZoom"] = null;
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject actualPage = new Navigation.NavigationObject();


                        //actualPage.NumPage = this.SelectedPage.ToString();
                        //actualPage.PageSize = this.PageSize.ToString();
                        //actualPage.IdObject = UIManager.ProjectManager.getProjectInSession().systemID;
                        //actualPage.SearchFilters = null;
                        //actualPage.Type = string.Empty;
                        //actualPage.folder = UIManager.ProjectManager.getProjectInSession().folderSelezionato;

                        //actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_ARCHIVE.ToString(), string.Empty);
                        //actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.SEARCH_ARCHIVE.ToString(), true);
                        //actualPage.CodePage = Navigation.NavigationUtils.NamePage.SEARCH_ARCHIVE.ToString();

                        //actualPage.Page = "SEARCHARCHIVE.ASPX";
                        //actualPage.DxTotalNumberElement = this.RecordCount.ToString();
                        //actualPage.ViewResult = true;

                        //if (this.PageCount == 0)
                        //{
                        //    actualPage.DxTotalPageNumber = "1";
                        //}
                        //else
                        //{
                        //    actualPage.DxTotalPageNumber = this.PageCount.ToString();
                        //}

                        //int indexElement = ((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1);
                        //actualPage.DxPositionElement = indexElement.ToString();

                        //navigationList.Add(actualPage);
                        //Navigation.NavigationUtils.SetNavigationList(navigationList);

                        actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_ARCHIVE.ToString(), string.Empty);
                        actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.SEARCH_ARCHIVE.ToString(), true, this.Page);
                        actualPage.CodePage = Navigation.NavigationUtils.NamePage.SEARCH_ARCHIVE.ToString();
                        actualPage.Page = "SEARCHARCHIVE.ASPX";
                        actualPage.IdObject = schedaDocumento.systemId;
                        actualPage.OriginalObjectId = schedaDocumento.systemId;
                        actualPage.NumPage = this.SelectedPage.ToString();
                        actualPage.PageSize = this.PageSize.ToString();
                        actualPage.DxTotalPageNumber = this.PageCount.ToString();
                        actualPage.DxTotalNumberElement = this.RecordCount.ToString();
                        actualPage.SearchFilters = this.SearchFilters;
                        actualPage.SearchFiltersOrder = UIManager.GridManager.GetFiltriOrderRicerca(GridManager.GetStandardGridForUser(GridTypeEnumeration.DocumentInProject, UIManager.UserManager.GetInfoUser()));
                        actualPage.ViewResult = true;
                        actualPage.folder = UIManager.ProjectManager.getProjectInSession().folderSelezionato;
                        actualPage.idProject = UIManager.ProjectManager.getProjectInSession().systemID;
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

        protected void gridViewResult_PreRender(object sender, EventArgs e)
        {
            try
            {
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

        protected void lbGoFascicolo_Click(object sender, EventArgs e)
        {

            //
            Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
            List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
            Navigation.NavigationObject obj = new Navigation.NavigationObject();
            obj.IdObject = fascicolo.systemID;
            obj.OriginalObjectId = fascicolo.systemID;
            obj.SearchFilters = this.SearchFilters;
            obj.ViewResult = false;
            obj.Type = string.Empty;
            obj.RegistryFilter = this.Registry;
            obj.Classification = this.Classification;
            obj.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_ARCHIVE.ToString(), string.Empty);
            obj.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.SEARCH_ARCHIVE.ToString(), true, this.Page);
            obj.CodePage = Navigation.NavigationUtils.NamePage.SEARCH_ARCHIVE.ToString();
            obj.Page = "SEARCHARCHIVE.ASPX";

            navigationList.Add(obj);
            Navigation.NavigationUtils.SetNavigationList(navigationList);

            Response.Redirect("../Project/Project.aspx");
        }

        protected void SearchProjectArchiveSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.TxtDescription.Text))
                {
                    this.HiddenIsSearching.Value = "true";
                    //this.cercaClassificazioneDaCodice(((myTreeNode)TreeTitolario.SelectedNode).CODICE);

                    int recordNumber = 0;
                    bool outOfMaxRowSearchable;

                    FiltroRicerca fV1 = new FiltroRicerca();
                    FiltroRicerca[][] qV = new FiltroRicerca[1][];
                    qV[0] = new FiltroRicerca[1];
                    FiltroRicerca[] fVList = new FiltroRicerca[0];

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_TITOLARIO.ToString();
                    fV1.valore = this.ddlTitolario.SelectedValue;
                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                    //fV1 = new DocsPaWR.FiltroRicerca();
                    //fV1.argomento = DocsPaWR.FiltriFascicolazione.CODICE_CLASSIFICA.ToString();
                    //fV1.valore = ((myTreeNode)TreeTitolario.SelectedNode).CODICE;
                    //fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new FiltroRicerca();
                    fV1.argomento = FiltriFascicolazione.TITOLO.ToString();
                    fV1.valore = this.TxtDescription.Text.ToString();
                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                    fV1.nomeCampo = null;

                    qV[0] = fVList;
                    this.SearchFilters = qV;

                    this.Classification = null;
                    this.SearchResult = this.SearchProject(this.SearchFilters, 1, out recordNumber, out outOfMaxRowSearchable);

                    this.TreeTitolario.Nodes.Clear();
                    this.InitializeObjectValue();

                    this.projectLblDocumentiFascicoliCount.Text = string.Empty;
                    this.projectLblNumeroDocumenti.Text = string.Empty;
                    this.plcGoFascicolo.Visible = false;
                    this.UpnlNumerodocumenti.Update();

                    this.gridViewResult.DataSource = new DataTable();
                    this.gridViewResult.DataBind();
                    this.UpnlGrid.Update();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningSearchArchiveDescription', 'warning', '');", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchProjectArchiveRemove_Click(object sender, EventArgs e)
        {
            this.HiddenIsSearching.Value = string.Empty;
            this.upPnlButtons.Update();

            this.TxtDescription.Text = string.Empty;
            this.SearchFilters = null;
            this.Classification = null;
            this.SearchResult = null;

            this.TreeTitolario.Nodes.Clear();
            this.InitializeObjectValue();

            this.projectLblDocumentiFascicoliCount.Text = string.Empty;
            this.projectLblNumeroDocumenti.Text = string.Empty;
            this.plcGoFascicolo.Visible = false;
            this.UpnlNumerodocumenti.Update();

            this.gridViewResult.DataSource = new DataTable();
            this.gridViewResult.DataBind();
            this.UpnlGrid.Update();
        }

        #endregion

        #region Methods

        private void InitSearch()
        {
            this.HiddenIsSearching.Value = "true";

            int recordNumber = 0;
            bool outOfMaxRowSearchable;

            //Back
            if (this.Request.QueryString["back"] != null && this.Request.QueryString["back"].Equals("1"))
            {
                List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                Navigation.NavigationObject obj = navigationList.Last();
                if (!obj.CodePage.Equals(Navigation.NavigationUtils.NamePage.SEARCH_ARCHIVE.ToString()))
                {
                    obj = new Navigation.NavigationObject();
                    obj = navigationList.ElementAt(navigationList.Count - 2);
                }
                //schedaRicerca.FiltriRicerca = obj.SearchFilters;
                //this.SearchFilters = obj.SearchFilters;
                //if (!string.IsNullOrEmpty(obj.NumPage))
                //{
                //    this.SelectedPage = Int32.Parse(obj.NumPage);
                //}

                //this.BindFilterValues(schedaRicerca, this.ShowGridPersonalization);
                //ProjectManager.setFiltroRicFasc(this, schedaRicerca.FiltriRicerca);

                //this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid);

                //if (!string.IsNullOrEmpty(obj.OriginalObjectId))
                //{
                //    string idProject = string.Empty;
                //    foreach (GridViewRow grd in this.gridViewResult.Rows)
                //    {
                //        idProject = string.Empty;

                //        if (GrigliaResult.Rows[grd.RowIndex]["IdProject"] != null)
                //        {
                //            idProject = GrigliaResult.Rows[grd.RowIndex]["IdProject"].ToString();
                //        }

                //        if (idProject.Equals(obj.OriginalObjectId))
                //        {
                //            this.gridViewResult.SelectRow(grd.RowIndex);
                //            this.SelectedRow = grd.RowIndex.ToString();
                //        }
                //    }
                //}
                this.SearchFilters = obj.SearchFilters;
                this.UpnlNumerodocumenti.Update();
                this.UpnlGrid.Update();
            }


            if (this.SearchFilters != null)
            {
                for (int i = 0; i < this.SearchFilters[0].Length; i++)
                {
                    if (this.SearchFilters[0][i].argomento == DocsPaWR.FiltriFascicolazione.TIPO_FASCICOLO.ToString())
                        this.SearchFilters[0][i].valore = string.Empty;
                }
            }
            if (this.SearchFilters != null)
                this.SearchResult = this.SearchProject(this.SearchFilters, 1, out recordNumber, out outOfMaxRowSearchable);

            this.TreeTitolario.Nodes.Clear();
            this.InitializeObjectValue();

            this.projectLblDocumentiFascicoliCount.Text = string.Empty;
            this.projectLblNumeroDocumenti.Text = string.Empty;
            this.plcGoFascicolo.Visible = false;
            this.UpnlNumerodocumenti.Update();

            //this.SearchDocumentsAndDisplayResult(this.SearchDocFilters, SelectedPage, GridManager.GetStandardGridForUser(GridTypeEnumeration.Document, UserManager.GetInfoUser()), this.Labels.ToArray(), UIManager.ProjectManager.getProjectInSession().folderSelezionato, UIManager.GridManager.GetFiltriOrderRicerca(GridManager.GetStandardGridForUser(GridTypeEnumeration.Document, UserManager.GetInfoUser())), true);
            this.ShowGrid(GridManager.GetStandardGridForUser(GridTypeEnumeration.Document, UserManager.GetInfoUser()), null, 0, 0, this.Labels.ToArray());
            this.UpnlGrid.Update();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.LitSearchProject.Text = Utils.Languages.GetLabelFromCode("SearchProjectsArchivioTitle", language);
            this.SearchProjectArchiveSearch.Text = Utils.Languages.GetLabelFromCode("SearchLabelButton", language);
            this.SearchProjectArchiveRemove.Text = Utils.Languages.GetLabelFromCode("SearchLabelRemoveFiltersButton", language);
            this.litRegistry.Text = Utils.Languages.GetLabelFromCode("SearchProjectRegistry", language);
            this.litDescription.Text = Utils.Languages.GetLabelFromCode("SearchProjectDescription", language);
            this.projectLitNomeGriglia.Text = Utils.Languages.GetLabelFromCode("projectLitNomeGriglia", language);
            this.DocumentViewer.Title = Utils.Languages.GetLabelFromCode("TitleDocumentViewerPopup", language);
            this.lbGoFascicolo.Text = Utils.Languages.GetLabelFromCode("SearchArchiveGoFascicolo", language);
            this.litTitolario.Text = Utils.Languages.GetLabelFromCode("SearchProjectTitolario", language)+": ";
        }

        private void InitializePage()
        {
            this.ClearSessionProperties();
            this.InitializeLanguage();
            this.PopulateDDLRegistry();
            this.PopulateDDLTitolario();
            this.InitSearch();
            //this.InitializeObjectValue();
            this.VisibiltyRoleFunctions();
        }

        protected void InitializeObjectValue()
        {
            DocsPaWR.Utente user = UIManager.UserManager.GetUserInSession();
            DocsPaWR.Ruolo role = UIManager.RoleManager.GetRoleInSession();
            DocsPaWR.OrgTitolario titolario = UIManager.ClassificationSchemeManager.GetTitolarioInSession();
            if (titolario == null)
            {
                titolario = UIManager.ClassificationSchemeManager.getTitolarioAttivo(user.idAmministrazione);
                UIManager.ClassificationSchemeManager.SetTitolarioInSession(titolario);
            }

            string IdRegistro = UIManager.RegistryManager.GetRegistryInSession().systemId;
            if (this.ddlRegistri.Visible)
                IdRegistro = this.ddlRegistri.SelectedValue;
            UIManager.RegistryManager.GetRegistryInSession();
            this.CaricamentoTitolario(user.idAmministrazione, "0", role.idGruppo, IdRegistro, titolario.ID);
        }

        private void VisibiltyRoleFunctions()
        {

        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CombineRowsHover", "CombineRowsHover();", true);
        }

        private void RemoveIsZoom()
        {
            HttpContext.Current.Session.Remove("isZoom");
        }

        private void ClearSessionProperties()
        {
            //this.Classification = null;
            this.Registry = null;
            this.SearchDocFilters = null;
            this.FascFolder = null;
            //this.SearchResult = null;
            this.Result = null;
            this.SelectedRow = string.Empty;
            this.RecordCount = 0;
            this.SelectedPage = 1;
            Session["templateRicerca"] = null;
            this.Labels = DocumentManager.GetLettereProtocolli();
            this.CellPosition = new Dictionary<string, int>();
            UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
            GridManager.SelectedGrid = GridManager.GetStandardGridForUser(GridTypeEnumeration.Document, UserManager.GetInfoUser());


        }

        protected void PopulateDDLRegistry()
        {
            DocsPaWR.Registro registro = UIManager.RegistryManager.GetRegistryInSession();
            if (registro == null)
            {
                registro = RoleManager.GetRoleInSession().registri[0];
            }
            foreach (DocsPaWR.Registro reg in RoleManager.GetRoleInSession().registri)
            {
                if (!reg.flag_pregresso)
                {
                    ListItem item = new ListItem();
                    item.Text = reg.codRegistro;
                    item.Value = reg.systemId;
                    if (reg.systemId == registro.systemId) item.Selected = true;
                    this.ddlRegistri.Items.Add(item);
                }
            }

            this.Registry = registro;

            if (this.ddlRegistri.Items.Count == 1)
            {
                this.plcRegistry.Visible = false;
                this.UpPnlRegistry.Update();
            }
        }

        private void PopulateDDLTitolario()
        {
            this.ddlTitolario.Items.Clear();
            ArrayList listaTitolari = ClassificationSchemeManager.getTitolariUtilizzabili();

            //Esistono dei titolari chiusi
            if (listaTitolari.Count > 1)
            {
                //Creo le voci della ddl dei titolari
                foreach (DocsPaWR.OrgTitolario titolario in listaTitolari)
                {
                    ListItem it = new ListItem(titolario.Descrizione, titolario.ID);
                    this.ddlTitolario.Items.Add(it);
                }

                //se la chiave di db è a 1, seleziono di default il titolario attivo
                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_CHECK_TITOLARIO_ATTIVO.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_CHECK_TITOLARIO_ATTIVO.ToString()).Equals("1"))
                {
                    foreach (DocsPaWR.OrgTitolario titolario in listaTitolari)
                    {

                        if (titolario.Stato == DocsPaWR.OrgStatiTitolarioEnum.Attivo)
                            this.ddlTitolario.SelectedValue = titolario.ID;
                    }
                }

                DocsPaWR.OrgTitolario titolarioX = UIManager.ClassificationSchemeManager.GetTitolarioInSession();
                if (titolarioX != null)
                {
                    ddlTitolario.SelectedValue = titolarioX.ID;
                }
            
            }
            else
            {
                DocsPaWR.OrgTitolario titolario = (DocsPaWR.OrgTitolario)listaTitolari[0];
                if (titolario.Stato != DocsPaWR.OrgStatiTitolarioEnum.InDefinizione)
                {
                    ListItem it = new ListItem(titolario.Descrizione, titolario.ID);
                    this.ddlTitolario.Items.Add(it);
                }
                this.ddlTitolario.Enabled = false;
                this.plcTitolario.Visible = false;
                this.UpPnlTitolario.Update();
            }
        }

        private void CaricamentoTitolario(string _IdAmministrazione, string _IdParent, string _IdGruppo, string _IdRegistro, string _IdTitolario)
        {
            this.TreeTitolario = UIManager.ClassificationSchemeManager.loadTreeView(TreeTitolario, _IdAmministrazione, _IdParent, _IdGruppo, _IdRegistro, _IdTitolario);
        }

        protected TreeNode LoadProjectNodes(TreeNode node)
        {
            int numFascProc = 0;

            myTreeNode TreeNodo = (myTreeNode)node;
            if (string.IsNullOrEmpty(TreeNodo.TIPO))
            {
                if (node.ChildNodes.Count > 0)
                {
                    foreach (TreeNode n in node.ChildNodes)
                    {
                        if (n.ChildNodes.Count == 0)
                        {
                            // load fascicoli procedimentali
                            numFascProc = this.GetFascProc(((myTreeNode)n).ID);
                            if (numFascProc > 0)
                            {
                                myTreeNode nodoFiglio = new myTreeNode();
                                nodoFiglio.Expanded = false;
                                nodoFiglio.TIPO = "P";
                                n.ChildNodes.Add(nodoFiglio);
                            }
                        }
                    }
                }

                numFascProc = this.GetFascProc(((myTreeNode)node).ID);
                foreach (SearchObject obj in this.Result)
                {
                    string id = obj.SearchObjectID;
                    string tipo = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P1")).FirstOrDefault().SearchObjectFieldValue;
                    string codice = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P3")).FirstOrDefault().SearchObjectFieldValue;
                    string description = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P4")).FirstOrDefault().SearchObjectFieldValue;

                    myTreeNode nodoFiglio = new myTreeNode();
                    nodoFiglio.ID = id;
                    nodoFiglio.IDRECORD = id;
                    nodoFiglio.Text = "<img src=\"../Images/Icons/project.png\" alt=\"\" style=\"float: left; margin: 0 5px 0 0; border: 0;\" /> " + codice + " - " + description;
                    nodoFiglio.TIPO = tipo;
                    nodoFiglio.CODICE = codice;

                    // load sottofascicoli
                    this.GetSottofascicoli(nodoFiglio, codice);

                    TreeNodo.ChildNodes.Add(nodoFiglio);
                }
            }
            else if (TreeNodo.TIPO == "P")
            {
                // load sottofascicoli
                Fascicolo fascicolo = UIManager.ProjectManager.GetProjectByCode(this.Registry, TreeNodo.CODICE);
                Folder folder = ProjectManager.getFolder(this, fascicolo);
                folder = ProjectManager.getFolder(this, folder);

                //Fascicolo fascicolo = ProjectManager.getFascicoloById(TreeNodo.ID);
                //Folder folder = ProjectManager.getFolder(this, fascicolo);
                this.GetSottofascicoli(node, TreeNodo.CODICE, folder);
            }
            else if (TreeNodo.TIPO == "S")
            {
                // load sottosottofascicoli
                Folder folder = ProjectManager.getFolder(this, TreeNodo.ID);
                folder = ProjectManager.getFolder(this, folder);
                this.GetSottofascicoli(TreeNodo, TreeNodo.CODICE, folder);
            }

            return node;
        }

        private int GetFascProc(string code)
        {
            this.cercaClassificazioneDaCodice(code);

            int recordNumber = 0;
            bool outOfMaxRowSearchable;

            FiltroRicerca fV1 = new FiltroRicerca();
            FiltroRicerca[][] qV = new FiltroRicerca[1][];
            qV[0] = new FiltroRicerca[1];
            FiltroRicerca[] fVList = new FiltroRicerca[0];

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_TITOLARIO.ToString();
            fV1.valore = this.ddlTitolario.SelectedValue;
            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriFascicolazione.CODICE_CLASSIFICA.ToString();
            fV1.valore = code;
            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriFascicolazione.TIPO_FASCICOLO.ToString();
            fV1.valore = "P";
            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

            if (!string.IsNullOrEmpty(this.HiddenIsSearching.Value) && !string.IsNullOrEmpty(this.TxtDescription.Text))
            {
                fV1 = new FiltroRicerca();
                fV1.argomento = FiltriFascicolazione.TITOLO.ToString();
                fV1.valore = this.TxtDescription.Text.ToString();
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                fV1.nomeCampo = null;
            }

            qV[0] = fVList;
            this.SearchFilters = qV;

            this.Result = this.SearchProject(this.SearchFilters, 1, out recordNumber, out outOfMaxRowSearchable);

            return recordNumber;
        }

        private bool cercaClassificazioneDaCodice(string codClassificazione)
        {
            bool res = false;
            DocsPaWR.Fascicolo[] listaFasc;
            if (!string.IsNullOrEmpty(codClassificazione))
            {
                listaFasc = this.getFascicolo(RegistryManager.getRegistroBySistemId(this.ddlRegistri.SelectedValue), codClassificazione);

                DocsPaWR.FascicolazioneClassificazione[] FascClass = ProjectManager.fascicolazioneGetTitolario2(this, codClassificazione, false, this.getIdTitolario(codClassificazione, listaFasc));
                if (FascClass != null && FascClass.Length != 0)
                {
                    this.Classification = FascClass[0];
                }
                else
                {
                    this.Classification = null;
                }
            }

            return res;
        }

        private string getIdTitolario(string codClassificazione, DocsPaWR.Fascicolo[] listaFasc)
        {
            if (!string.IsNullOrEmpty(codClassificazione))
            {
                //DocsPaWR.Fascicolo[] listaFasc = getFascicolo(UserManager.getRegistroSelezionato(this), codClassificazione);

                //In questo caso il metodo "GetFigliClassifica2" funzionerebbe male
                //per questo viene restituti l'idTitolario dell'unico fascicolo risolto
                if (this.ddlTitolario.SelectedItem != null && this.ddlTitolario.SelectedItem.Text == "Tutti i titolari" && listaFasc != null && listaFasc.Length == 1)
                {
                    DocsPaWR.Fascicolo fasc = (DocsPaWR.Fascicolo)listaFasc[0];
                    return fasc.idTitolario;
                }
            }

            //In tutti gli altri casi è sufficiente restituire il value degli item della
            //ddl_Titolario in quanto formati secondo le specifiche di uno o piu' titolari
            return this.ddlTitolario.SelectedValue;
        }

        private DocsPaWR.Fascicolo[] getFascicolo(DocsPaWR.Registro registro, string codClassificazione)
        {
            DocsPaWR.Fascicolo[] listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codClassificazione, registro, "R");
            return listaFasc;
        }

        /// <summary>
        /// Funzione per la ricerca dei fascicoli
        /// </summary>
        /// <param name="searchFilters">Filtri di ricerca</param>
        /// <param name="classification">Oggetto classificazione da utilizzare per la ricerca dei fascicoli</param>
        /// <param name="registry">Registro selezionato</param>
        /// <param name="allClassification">True se bisogna ricercare anche nei sottofascicoli</param>
        /// <param name="selectedPage">Pagina da visualizzare</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        /// <returns>Lista delle informazioni sui fascicoli restituiti dalla ricerca</returns>
        private SearchObject[] SearchProject(FiltroRicerca[][] searchFilters, int selectedPage, out int recordNumber, out bool outOfMaxRowSearchable)
        {
            // Fascicoli individuati dalla ricerca
            SearchObject[] toReturn;

            // Informazioni sull'utente
            InfoUtente userInfo;

            // Numero totale di pagine
            int pageNumbers;

            // Lista dei system id dei fascicoli restituiti dalla ricerca
            SearchResultInfo[] idProjects;

            // Prelevamento delle informazioni sull'utente
            userInfo = UserManager.GetInfoUser();

            // Recupero dei campi della griglia impostati come visibili
            Field[] visibleArray = null;
            List<Field> visibleFieldsTemplate;

            if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Project)
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Project);

            visibleFieldsTemplate = GridManager.SelectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field)) && e.CustomObjectId != 0).ToList();

            if (visibleFieldsTemplate != null && visibleFieldsTemplate.Count > 0)
            {
                visibleArray = visibleFieldsTemplate.ToArray();
            }

            // Caricamento dei fascicoli
            bool allClassification = false;
            bool showGridPersonalization = false;
            bool export = true;
            int pageSize = 10;
            if (this.SearchFilters != null)
                toReturn = ProjectManager.getListaFascicoliPagingCustom(this.Classification, this.Registry, this.SearchFilters[0], allClassification, 1, out pageNumbers, out recordNumber, pageSize, false, out idProjects, null, showGridPersonalization, export, visibleArray, null, true);
            else
                toReturn = ProjectManager.getListaFascicoliPagingCustom(this.Classification, this.Registry, null, allClassification, 1, out pageNumbers, out recordNumber, pageSize, false, out idProjects, null, showGridPersonalization, export, visibleArray, null, true);
            /* ABBATANGELI GIANLUIGI
             * outOfMaxRowSearchable viene impostato a true se getQueryInfoDocumentoPagingCustom
             * restituisce pageNumbers = -2 (raggiunto il numero massimo di righe possibili come risultato di ricerca)*/
            outOfMaxRowSearchable = (pageNumbers == -2);

            // Memorizzazione del numero di risultati restituiti dalla ricerca, del numero di pagina e dei risultati
            //this.RecordCount = recordNumber;
            //this.PagesCount = pageNumbers;
            //this.Result = toReturn;

            // Restituzione della lista di fascicoli da visualizzare
            return toReturn;
        }

        private int GetSottofascicoli(TreeNode node, string fascCode)
        {
            Fascicolo fascicolo = UIManager.ProjectManager.GetProjectByCode(this.Registry, fascCode);
            Folder folder = ProjectManager.getFolder(this, fascicolo);
            if (folder != null && !string.IsNullOrEmpty(folder.systemID))
            {
                folder = ProjectManager.getFolder(this, folder);

                foreach (Folder c in folder.childs)
                {
                    myTreeNode nodoSottofasc = new myTreeNode();
                    nodoSottofasc.Expanded = false;
                    nodoSottofasc.TIPO = "S";
                    node.ChildNodes.Add(nodoSottofasc);
                }

                return folder.childs.Length;
            }
            else
            {
                return this.GetSottofascicoli(node, fascCode, folder);
            }
        }

        private int GetSottofascicoli(TreeNode node, string fascCode, Folder folder)
        {
            myTreeNode TreeNodo = (myTreeNode)node;
            string html_tag = "<img src=\"../Images/Icons/folder.png\" alt=\"\" style=\"float: left; margin: 0 5px 0 0; border: 0;\" />";

            //ABBATANGELI - DUPLICA IL LIVELLO NEL TAB ARCHIVIO
            //string valorechiave = InitConfigurationKeys.GetValue("0", "FE_PROJECT_LEVEL");
            string valorechiave = "0";

            bool keyvalue = !string.IsNullOrEmpty(valorechiave) && valorechiave.Equals("1");

            foreach (Folder f in folder.childs)
            {
                myTreeNode nodoFiglio = new myTreeNode();
                nodoFiglio.ID = f.systemID;
                nodoFiglio.IDRECORD = f.systemID;
                nodoFiglio.CODICE = fascCode;
                nodoFiglio.Text = keyvalue ? html_tag + GetFolderDescription(f) : html_tag + f.descrizione;
                nodoFiglio.TIPO = "S";

                foreach (Folder c in f.childs)
                {
                    myTreeNode nodoSottofasc = new myTreeNode();
                    nodoSottofasc.Expanded = false;
                    nodoSottofasc.TIPO = "S";
                    nodoFiglio.ChildNodes.Add(nodoSottofasc);
                }

                TreeNodo.ChildNodes.Add(nodoFiglio);
            }

            return folder.childs.Length;
        }

        private string GetFolderDescription(Folder f)
        {
            string temp = string.Empty;
            int folder_length = (f.codicelivello.Length / 4);
            for (int i = 1; i < folder_length ; i++)
                temp += string.Format("{0}.", Convert.ToInt32(f.codicelivello.Substring(i * 4, 4)));

            return temp.Substring(0, temp.Length - 1) + " - " + f.descrizione;
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        protected void BuildGridNavigator()
        {
            this.plcNavigator.Controls.Clear();

            if (this.PageCount > 1)
            {
                Panel panel = new Panel();
                panel.EnableViewState = true;
                panel.CssClass = "recordNavigator";

                int startFrom = 1;
                if (this.SelectedPage > 6) startFrom = this.SelectedPage - 5;

                int endTo = 10;
                if (this.SelectedPage > 6) endTo = this.SelectedPage + 5;
                if (endTo > this.PageCount) endTo = this.PageCount;

                if (startFrom > 1)
                {
                    LinkButton btn = new LinkButton();
                    btn.EnableViewState = true;
                    btn.Text = "...";
                    btn.Attributes["onclick"] = " $('#grid_pageindex').val(" + (startFrom - 1) + "); __doPostBack('upPnlButtons', ''); return false;";
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
                        btn.Attributes["onclick"] = " $('#grid_pageindex').val($(this).text()); __doPostBack('upPnlButtons', ''); return false;";
                        panel.Controls.Add(btn);
                    }
                }

                if (endTo < this.PageCount)
                {
                    LinkButton btn = new LinkButton();
                    btn.EnableViewState = true;
                    btn.Text = "...";
                    btn.Attributes["onclick"] = " $('#grid_pageindex').val(" + endTo + "); __doPostBack('upPnlButtons', ''); return false;";
                    panel.Controls.Add(btn);
                }

                this.plcNavigator.Controls.Add(panel);
            }
            else
            {
                this.SelectedPage = -1;
            }
        }

        /// <summary>
        /// Questa funzione si occupa di ricercare i documenti e di visualizzare 
        /// i dati
        /// </summary>
        private void SearchDocumentsAndDisplayResult(FiltroRicerca[][] searchFilters, int selectedPage, Grid selectedGrid, EtichettaInfo[] labels, Folder folder,
            FiltroRicerca[][] orderFilters, bool ricerca)
        {
            // Numero di record restituiti dalla pagina
            int recordNumber = 0;

            // Risultati restituiti dalla ricerca
            SearchObject[] result;

            // Ricerca dei documenti
            result = this.SearchDocument(searchFilters, this.SelectedPage, out recordNumber, folder, orderFilters, selectedGrid);

            // Se ci sono risultati, vengono visualizzati
            if (this.Result != null && this.Result.Length > 0)
            {
                this.ShowResult(selectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
            }
            else
            {
                this.ShowGrid(selectedGrid, null, 0, 0, labels);
            }
        }

        /// <summary>
        /// Funzione per la ricerca dei documenti
        /// </summary>
        /// <param name="searchFilters">Filtri da utilizzare per la ricerca</param>
        /// <param name="selectedPage">Pagina da caricare</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        /// <param name="folder">Folder in cui ricercare</param>
        /// <returns>Lista delle informazioni sui documenti restituiti dalla ricerca</returns>
        private SearchObject[] SearchDocument(FiltroRicerca[][] searchFilters,
            int selectedPage, out  int recordNumber, Folder folder,
            FiltroRicerca[][] orderFilters, Grid SelectedGrid)
        {
            // Documenti individuati dalla ricerca
            SearchObject[] toReturn;

            // Recupero dei campi della griglia impostati come visibili
            Field[] visibleArray = null;
            List<Field> visibleFieldsTemplate;

            int pageNumbers = 0;

            // Lista dei system id dei documenti restituiti dalla ricerca
            SearchResultInfo[] idProfiles;

            visibleFieldsTemplate = SelectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field)) && e.CustomObjectId != 0).ToList();

            if (visibleFieldsTemplate != null && visibleFieldsTemplate.Count > 0)
            {
                visibleArray = visibleFieldsTemplate.ToArray();
            }

            toReturn = UIManager.ProjectManager.getListaDocumentiPagingCustom(
                    folder,
                    searchFilters,
                    selectedPage,
                    out pageNumbers,
                    out recordNumber,
                    true,
                    this.ShowGridPersonalization,
                    false,
                    visibleArray,
                    null,
                    this.PageSize,
                    orderFilters,
                    out idProfiles);

            this.RecordCount = recordNumber;
            //this.PageCount = pageNumbers == 0 ? 1 : pageNumbers;

            this.PageCount = (int)Math.Round(((double)recordNumber / (double)this.PageSize) + 0.49);

            this.Result = toReturn;

            this.UpnlAzioniMassive.Update();
            // Restituzione della lista di documenti da visualizzare
            return toReturn;
        }

        /// <summary>
        /// Funzione per la visualizzazione dei risutati della ricerca
        /// </summary>
        /// <param name="result">I risultati della ricerca</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        private void ShowResult(Grid selectedGrid, SearchObject[] result, int recordNumber, int selectedPage, EtichettaInfo[] labels)
        {
            this.ShowGrid(selectedGrid, this.Result, this.RecordCount, selectedPage, labels);
            this.gridViewResult.SelectedIndex = this.SelectedPage;
            this.BuildGridNavigator();

        }

        private void ShowGrid(Grid selectedGrid, SearchObject[] result, int recordNumber, int selectedPage, EtichettaInfo[] labels)
        {
            bool visibile = false;
            gridViewResult = this.HeaderGridView(selectedGrid,
              this.ShowGridPersonalization, gridViewResult);

            DataTable dt = UIManager.GridManager.InitializeDataSet(selectedGrid,
                         null,
                         this.ShowGridPersonalization);


            if (result != null && result.Length > 0)
            {
                dt = this.FillDataSet(dt, result, selectedGrid, labels, this.ShowGridPersonalization);
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

            GrigliaResult = dt;
            gridViewResult.DataSource = dt;
            gridViewResult.DataBind();
            if (gridViewResult.Rows.Count > 0) gridViewResult.Rows[0].Visible = visibile;

            string gridType = this.GetLabel("projectLitGrigliaStandard");
            if (this.ShowGridPersonalization)
            {
                if (selectedGrid != null && string.IsNullOrEmpty(selectedGrid.GridId))
                {
                    gridType = "<span class=\"red\">" + this.GetLabel("projectLitGrigliaTemp") + "</span>";
                }
                else
                {
                    if (!(selectedGrid.GridId).Equals("-1"))
                    {
                        gridType = selectedGrid.GridName;
                    }
                }
            }

            this.projectLblNumeroDocumenti.Text = this.GetLabel("projectLblNumeroDocumentiSimple");
            this.projectLblNumeroDocumenti.Text = this.projectLblNumeroDocumenti.Text.Replace("{1}", recordNumber.ToString());

            this.UpnlNumerodocumenti.Update();
            this.UpnlGrid.Update();
        }

        /// <summary>
        /// Funzione per l'inizializzazione del data set in base ai campi definiti nella 
        /// griglia
        /// </summary>
        /// <param name="selectedGrid">La griglia su cui basare la creazione del dataset</param>
        /// <returns></returns>
        public GridView HeaderGridView(Grid selectedGrid, bool showGridPersonalization, GridView grid)
        {
            try
            {
                int position = 0;
                List<Field> fields = selectedGrid.Fields.Where(e => e.Visible).OrderBy(f => f.Position).ToList();

                grid.Columns.Clear();

                // Creazione delle colonne
                foreach (Field field in fields)
                {
                    BoundField column = null;
                    ButtonField columnHL = null;
                    TemplateField columnCKB = null;
                    if (field.OriginalLabel.ToUpper().Equals("DOCUMENTO"))
                        columnHL = GridManager.GetLinkColumn(field.Label,
                            field.FieldId,
                            field.Width);
                    else
                    {

                        if (field is SpecialField)
                        {
                            switch (((SpecialField)field).FieldType)
                            {
                                case SpecialFieldsEnum.Icons:
                                    columnCKB = GridManager.GetBoundColumnIcon(field.Label, field.Width, field.FieldId);
                                    break;
                                //case SpecialFieldsEnum.CheckBox:
                                //    {
                                //        columnCKB = GridManager.GetBoundColumnCheckBox(field.Label, field.Width, field.FieldId);
                                //        break;
                                //    }
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
                                        break;
                                    }

                                default:
                                    {
                                        column = GridManager.GetBoundColumn(
                                         field.Label,
                                         field.OriginalLabel,
                                         field.Width,
                                         field.FieldId);
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
                        {
                            if (columnHL != null)
                            {
                                grid.Columns.Add(columnHL);
                            }
                        }



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
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("ProtoType", "ProtoType"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsSigned", "IsSigned"));
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
            EtichettaInfo[] labels, bool showGridPersonalization)
        {
            try
            {
                List<Field> visibleFields = selectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field))).ToList();
                Field specialField = selectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(SpecialField)) && ((SpecialField)e).FieldType.Equals(SpecialFieldsEnum.Icons)).FirstOrDefault<Field>();

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
                System.Text.StringBuilder temp;
                foreach (SearchObject doc in result)
                {
                    // ...viene inizializzata una nuova riga
                    dataRow = dataTable.NewRow();

                    foreach (Field field in visibleFields)
                    {
                        string numeroDocumento = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                        string numeroProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue;
                        string dataProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;

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
                                string dataApertura = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
                                string protTit = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("PROT_TIT")).FirstOrDefault().SearchObjectFieldValue;

                                temp = new System.Text.StringBuilder("<span style=\"color:");
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
                                //valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_IS_PRESENT_NOTE");

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
                                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
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
                            //OGGETTI CUSTOM
                            default:
                                SearchObjectField tempField = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault();
                                if (tempField != null)
                                {
                                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        // se l'ggetto custom è un contatore di repertorio visualizzo la segnatura di repertorio
                                        if (value.ToUpper().Equals("#CONTATORE_DI_REPERTORIO#"))
                                        {
                                            string idDoc = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                                            //value = DocumentManager.getSegnaturaRepertorio(this.Page, idDoc, UserManager.getInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione).Codice);
                                        }
                                    }
                                }
                                else
                                {
                                    value = string.Empty;
                                }
                                break;
                        }

                        // Valorizzazione del campo fieldName
                        // Se il documento è annullato, viene mostrato un testo barrato, altrimenti
                        // viene mostrato così com'è
                        string dataAnnullamento = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D11")).FirstOrDefault().SearchObjectFieldValue;

                        string itemTitle = numeroDocumento + " / " + dataProtocollo;
                        if (!String.IsNullOrEmpty(numeroProtocollo))
                            itemTitle = numeroProtocollo + " / " + dataProtocollo;

                        if (!String.IsNullOrEmpty(dataAnnullamento))
                            dataRow[field.FieldId] = String.Format("<span id=\"doc" + numeroDocumento + "\" class=\"jstree-draggable2\" title=\"" + itemTitle.Replace("\"", "&quot;") + "\" style=\"text-decoration: line-through; color: Red;\">{0}</span>", value);
                        else
                            dataRow[field.FieldId] = String.Format("<span id=\"doc" + numeroDocumento + "\" class=\"jstree-draggable2\" title=\"" + itemTitle.Replace("\"", "&quot;") + "\">{0}</span>", value);
                        value = string.Empty;
                    }

                    string immagineAcquisita = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D23")).FirstOrDefault().SearchObjectFieldValue;
                    string inConservazione = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_CONSERVAZIONE")).FirstOrDefault().SearchObjectFieldValue;
                    string inAdl = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_ADL")).FirstOrDefault().SearchObjectFieldValue;
                    string isFirmato = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("CHA_FIRMATO")).FirstOrDefault().SearchObjectFieldValue;

                    dataRow["ProtoType"] = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D3")).FirstOrDefault().SearchObjectFieldValue;
                    dataRow["IdProfile"] = doc.SearchObjectID;
                    dataRow["FileExtension"] = !String.IsNullOrEmpty(immagineAcquisita) && immagineAcquisita != "0" ? immagineAcquisita : String.Empty;
                    dataRow["IsInStorageArea"] = !String.IsNullOrEmpty(inConservazione) && inConservazione != "0" ? true : false;
                    dataRow["IsInWorkingArea"] = !String.IsNullOrEmpty(inAdl) && inAdl != "0" ? true : false;
                    dataRow["IsSigned"] = !String.IsNullOrEmpty(isFirmato) && isFirmato != "0" ? true : false;

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

        private void HighlightNodesFound(TreeNode node)
        {
            foreach (TreeNode n in node.ChildNodes)
            {
                myTreeNode _node = (myTreeNode)n;
                int livello = 0;
                if (!string.IsNullOrEmpty(_node.LIVELLO)) livello = int.Parse(_node.LIVELLO);

                if (SearchResult == null)
                    continue;

                foreach (SearchObject obj in this.SearchResult)
                {
                    string id = obj.SearchObjectID;
                    string tipo = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P1")).FirstOrDefault().SearchObjectFieldValue;
                    string codice = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P3")).FirstOrDefault().SearchObjectFieldValue;
                    int level = codice.Count(e => e == '.') + 1;

                    if ((livello == 0 || level >= livello) && (codice == _node.CODICE || codice.StartsWith(_node.CODICE + ".") || codice.StartsWith(_node.CODICE + "-")) && _node.Text.IndexOf("<span") < 0)
                    {
                        _node.Text = "<span style=\"background: #115A8D; color: #fff;\">" + _node.Text + "</span>";
                    }
                }
            }
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

                actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_ARCHIVE.ToString(), string.Empty);
                actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.SEARCH_ARCHIVE.ToString(), true, this.Page);
                actualPage.CodePage = Navigation.NavigationUtils.NamePage.SEARCH_ARCHIVE.ToString();
                actualPage.Page = "SEARCHARCHIVE.ASPX";
                actualPage.IdObject = schedaDocumento.systemId;
                actualPage.OriginalObjectId = schedaDocumento.systemId;
                actualPage.NumPage = this.SelectedPage.ToString();
                actualPage.PageSize = this.PageSize.ToString();
                actualPage.DxTotalPageNumber = this.PageCount.ToString();
                actualPage.DxTotalNumberElement = this.RecordCount.ToString();
                actualPage.SearchFilters = this.SearchFilters;
                actualPage.SearchFiltersOrder = UIManager.GridManager.GetFiltriOrderRicerca(GridManager.GetStandardGridForUser(GridTypeEnumeration.DocumentInProject, UIManager.UserManager.GetInfoUser()));
                actualPage.ViewResult = true;
                actualPage.folder = UIManager.ProjectManager.getProjectInSession().folderSelezionato;
                actualPage.idProject = UIManager.ProjectManager.getProjectInSession().systemID;
                int indexElement = ((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1) +1;
                actualPage.DxPositionElement = indexElement.ToString();
                navigationList.Add(actualPage);
                Navigation.NavigationUtils.SetNavigationList(navigationList);
                UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                this.ListObjectNavigation = this.Result;
                Response.Redirect("~/Document/Document.aspx");
            }
        }

        #endregion

    }
}

