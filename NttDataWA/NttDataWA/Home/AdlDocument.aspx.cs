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


namespace NttDataWA.Home
{
    public partial class AdlDocument : System.Web.UI.Page
    {

        #region Fields

        private const string UP_DOCUMENT_BUTTONS = "upPnlButtons";
        private const string CLOSE_POPUP_ZOOM = "closeZoom";
        public SearchManager schedaRicerca = null;
        private const string KEY_SCHEDA_RICERCA = "HomeDocAdl";
        private const string POPUP_DRAG_AND_DROP = "POPUP_DRAG_AND_DROP";
        #endregion

        #region Properties

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
                return true;
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

        protected Grid SelectedGrid
        {
            get
            {
                return HttpContext.Current.Session["AdlDocument.SelectedGrid"] as Grid;
            }
            set
            {
                HttpContext.Current.Session["AdlDocument.SelectedGrid"] = value;
            }
        }

        private bool OpenDocumentViewer
        {
            get
            {
                if (HttpContext.Current.Session["OpenDocumentViewer"] != null)
                    return (bool)HttpContext.Current.Session["OpenDocumentViewer"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["OpenDocumentViewer"] = value;
            }
        }


        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            schedaRicerca = (SearchManager)Session[SearchManager.SESSION_KEY];
            if (schedaRicerca == null)
            {
                //Inizializzazione della scheda di ricerca per la gestione delle ricerche salvate
                schedaRicerca = new SearchManager(KEY_SCHEDA_RICERCA, UserManager.GetUserInSession(), RoleManager.GetRoleInSession(), this);
                Session[SearchManager.SESSION_KEY] = schedaRicerca;
            }
            schedaRicerca.Pagina = this;

            if (!this.IsPostBack)
            {
                this.InitializePage();

                //Back
                if (this.Request.QueryString["back"] != null && this.Request.QueryString["back"].Equals("1"))
                {
                    List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                    Navigation.NavigationObject obj = navigationList.Last();
                    if (!obj.CodePage.Equals(Navigation.NavigationUtils.NamePage.HOME_ADL_DOCUMENT.ToString()))
                    {
                        obj = new Navigation.NavigationObject();
                        obj = navigationList.ElementAt(navigationList.Count - 2);
                    }

                    this.SearchFilters = obj.SearchFilters;
                    if (!string.IsNullOrEmpty(obj.NumPage))
                    {
                        this.SelectedPage = Int32.Parse(obj.NumPage);
                    }

                    schedaRicerca.FiltriRicerca = obj.SearchFilters;

                    this.BindFilterValues(schedaRicerca);

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

                    this.UpnlGrid.Update();
                }
                else
                {
                    bool result = this.SearchDocumentFilters();
                    if (result)
                    {
                        this.SelectedRow = string.Empty;
                        this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, this.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    }
                }


                this.UpnlGrid.Update();
            }
            else
            {
                if (this.Result != null && this.Result.Length > 0)
                {
                    // Visualizzazione dei risultati

                    // Lista dei documenti risultato della ricerca
                    if (!this.SelectedPage.ToString().Equals(this.grid_pageindex.Value))
                    {
                        this.Result = null;
                        this.SelectedRow = string.Empty;
                        if (!string.IsNullOrEmpty(this.grid_pageindex.Value))
                        {
                            this.SelectedPage = int.Parse(this.grid_pageindex.Value);
                        }
                        this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, this.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                        this.BuildGridNavigator();
                        this.UpnlGrid.Update();
                        this.upPnlGridIndexes.Update();
                    }
                    else
                    {
                        this.ShowResult(this.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
                    }
                }

                //rimuovo IsZoom alla chiusura della popup di zoom(da x o pulsante chiudi) quando richiamata da profilo, allegato, classifica
                if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_DOCUMENT_BUTTONS))
                {
                    if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_ZOOM)))
                    {
                        this.RemoveIsZoom();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('DocumentViewer','');", true);
                        return;
                    }
                }

                if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_DOCUMENT_BUTTONS))
                {
                    if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(POPUP_DRAG_AND_DROP)))
                    {
                        DragAndDropManager.ClearReport();

                        return;
                    }
                }

            }

            this.RefreshScript();
        }

        /// <summary>
        /// Ripristino valori filtri di ricerca nei campi della UI
        /// </summary>
        /// <param name="schedaRicerca"></param>
        private void BindFilterValues(SearchManager schedaRicerca)
        {
            if (schedaRicerca.FiltriRicerca != null && schedaRicerca.FiltriRicerca.Length > 0)
            {
                try
                {
                    foreach (DocsPaWR.FiltroRicerca item in schedaRicerca.FiltriRicerca[0])
                    {
                        if (item.argomento == DocsPaWR.FiltriDocumento.DOC_IN_ADL.ToString())
                        {
                            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
                            {
                                if (item.valore.StartsWith("0"))
                                {
                                    this.RblTypeAdl.SelectedValue = "1";
                                }
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    ErrorManager.redirect(this, ex);
                }
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
                            labelAdlRole = "ProjectIconTemplateAdlRole";
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
                        string imgUrl = FileManager.getFileIcon(this, estensione);
                        ((CustomImageButton)e.Row.FindControl("estensionedoc")).Visible = true;
                        ((CustomImageButton)e.Row.FindControl("estensionedoc")).ImageUrl = imgUrl;
                        ((CustomImageButton)e.Row.FindControl("estensionedoc")).OnMouseOutImage = imgUrl;
                        ((CustomImageButton)e.Row.FindControl("estensionedoc")).OnMouseOverImage = imgUrl;
                    }
                    else
                        ((CustomImageButton)e.Row.FindControl("estensionedoc")).Visible = false;

                    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).Enabled = true;
                    ((CustomImageButton)e.Row.FindControl("conservazione")).Visible = this.AllowConservazione;
                    ((CustomImageButton)e.Row.FindControl("adl")).Visible = this.AllowADL;
                    ((CustomImageButton)e.Row.FindControl("adlrole")).Visible = this.AllowADLRole;
                    ((CustomImageButton)e.Row.FindControl("firmato")).Visible = bool.Parse(GrigliaResult.Rows[e.Row.RowIndex]["IsSigned"].ToString());

                    //evento click
                    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).Click += new ImageClickEventHandler(ImageButton_Click);
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
                }
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
                string sortExpression = e.SortExpression.ToString();

                Field d = (Field)this.SelectedGrid.Fields.Where(f => f.Visible && f.FieldId.Equals(sortExpression)).FirstOrDefault();
                if (d != null)
                {
                    if (this.SelectedGrid.FieldForOrder != null && (this.SelectedGrid.FieldForOrder.FieldId).Equals(d.FieldId))
                    {
                        if (this.SelectedGrid.OrderDirection == OrderDirectionEnum.Asc)
                        {
                            this.SelectedGrid.OrderDirection = OrderDirectionEnum.Desc;
                        }
                        else
                        {
                            this.SelectedGrid.OrderDirection = OrderDirectionEnum.Asc;
                        }
                    }
                    else
                    {
                        if (this.SelectedGrid.FieldForOrder == null && d.FieldId.Equals("D9"))
                        {
                            this.SelectedGrid.FieldForOrder = d;
                            if (this.SelectedGrid.OrderDirection == OrderDirectionEnum.Asc)
                            {
                                this.SelectedGrid.OrderDirection = OrderDirectionEnum.Desc;
                            }
                            else
                            {
                                this.SelectedGrid.OrderDirection = OrderDirectionEnum.Asc;
                            }
                        }
                        else
                        {
                            this.SelectedGrid.FieldForOrder = d;
                            this.SelectedGrid.OrderDirection = OrderDirectionEnum.Asc;
                        }
                    }
                    this.SelectedGrid.GridId = string.Empty;


                    this.SelectedPage = 1;

                    if (this.Result != null && this.Result.Length > 0)
                    {
                        this.SearchDocumentFilters();
                        this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, this.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                    }
                    else
                    {
                        this.ShowGrid(this.SelectedGrid, null, 0, 0, this.Labels.ToArray<EtichettaInfo>());
                    }

                    this.BuildGridNavigator();
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
                gridViewResult.Columns[0].Visible = false;

                // GridManager.SelectedGrid

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
                foreach (Field field in this.SelectedGrid.Fields.Where(u => u.Visible).OrderBy(f => f.Position).ToList())
                    fullWidth += field.Width;
                this.gridViewResult.Attributes["style"] = "width: " + fullWidth + "px;";
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
                if (this.ShowGridPersonalization)
                {
                    //Posizione della freccetta nell'header
                    if (e.Row.RowType == DataControlRowType.Header)
                    {
                        System.Web.UI.WebControls.Image arrow = new System.Web.UI.WebControls.Image();

                        arrow.BorderStyle = BorderStyle.None;

                        if (this.SelectedGrid.OrderDirection == OrderDirectionEnum.Asc)
                        {
                            arrow.ImageUrl = "../Images/Icons/arrow_up.gif";
                        }
                        else
                        {
                            arrow.ImageUrl = "../Images/Icons/arrow_down.gif";
                        }

                        if (this.SelectedGrid.FieldForOrder != null)
                        {
                            Field d = (Field)this.SelectedGrid.Fields.Where(f => f.Visible && f.FieldId.Equals(this.SelectedGrid.FieldForOrder.FieldId)).FirstOrDefault();
                            if (d != null)
                            {
                                int cell = this.CellPosition[d.FieldId];
                                e.Row.Cells[cell].Controls.Add(arrow);
                            }
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

        /// <summary>
        /// evento delle icone della griglia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                                // Mev CS 1.5 - F03_01
                                #region oldCode
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
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorProjectAddDocInConservazione', 'error', '');} else {parent.ajaxDialogModal('ErrorProjectAddDocInConservazione', 'error', '');}", true);
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
                            if (this.IsAdl)
                            {
                                this.SelectedRow = string.Empty;
                                this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, this.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                            }
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
                        this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, this.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                        break;
                    }
                case "firmato":
                    {
                        UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                        FileManager.setSelectedFile(schedaDocumento.documenti[0]);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDigitalSignDetails", "ajaxModalPopupDigitalSignDetails();", true);
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
                        actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME_ADL_DOCUMENT.ToString(), string.Empty);
                        actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.HOME_ADL_DOCUMENT.ToString(), true, this.Page);
                        actualPage.CodePage = Navigation.NavigationUtils.NamePage.HOME_ADL_DOCUMENT.ToString();
                        actualPage.Page = "ADLDOCUMENT.ASPX";
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
                        OpenDocumentViewer = true;
                        break;
                    }
            }

        }

        protected void RblTypeAdl_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool result = this.SearchDocumentFilters();
            if (result)
            {
                this.SelectedPage = 1;
                this.SelectedRow = string.Empty;
                this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, this.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
            }
            this.UpnlGrid.Update();
        }

        public void HomeDdlRoles_SelectedIndexChange(object sender, EventArgs e)
        {

            Ruolo[] roles = UIManager.UserManager.GetUserInSession().ruoli;
            Ruolo role = (from r in roles where r.systemId.Equals(this.ddlRolesUser.SelectedValue) select r).FirstOrDefault();
            UIManager.RoleManager.SetRoleInSession(role);
            UIManager.RegistryManager.SetRegAndRFListInSession(UIManager.UserManager.getListaRegistriWithRF(role.systemId, "", ""));
            UIManager.RegistryManager.SetRFListInSession(UIManager.UserManager.getListaRegistriWithRF(role.systemId, "1", ""));
            //if (!UIManager.UserManager.IsAuthorizedFunctions("DO_LIBRO_FIRMA"))
            //{
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningNoAuthorizationLibroFirma', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('WarningNoAuthorizationLibroFirma', 'warning', '','',null,null,'RedirectHome()');}", true);       
                Response.Redirect("~/Index.aspx");
                return;
            //}
            /*
            try
            {
                (this.Page.Master.FindControl("upPnlInfoUser") as UpdatePanel).Update();

                bool result = this.SearchDocumentFilters();
                if (result)
                {
                    this.SelectedRow = string.Empty;
                    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, this.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                }
                this.UpnlGrid.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
             * */
        }

        #endregion

        #region Methods

        private void InitializePage()
        {
            this.ddlRolesUser.SelectedValue = RoleManager.GetRoleInSession().systemId;
            this.SelectedGrid = null;
            this.InitializeLanguage();
            this.ClearSessionProperties();
            this.InitializePageSize();
            this.LoadRolesUser();
            this.VisibiltyRoleFunctions();
            this.InitDragAndDropReport();
        }

        private void InitDragAndDropReport()
        {
            this.UploadLiveuploads.Visible = false;
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_ENABLE_DRAG_AND_DROP.ToString())) && Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_ENABLE_DRAG_AND_DROP.ToString()).Equals("1"))
            {
                this.UploadLiveuploads.Visible = true;
                if (DragAndDropManager.Report != null)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "MassiveReportDragAndDrop", " ajaxModalPopupMassiveReportDragAndDrop();", true);
            }
        }

        private void VisibiltyRoleFunctions()
        {
            this.ShowGridPersonalization = UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION");

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_CONS"))
            {
                this.AllowConservazione = true;
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADD_ADL"))
            {
                this.AllowADL = true;
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
            {
                this.AllowADLRole = true;
                this.PnlAdlRoleUserHome.Visible = true;
            }
        }

        private void ClearSessionProperties()
        {
            Session.Remove("itemUsedSearch");
            Session.Remove("idRicercaSalvata");
            //questa property rimane in sessione quando dal tab allegati faccio back e torno nella ricerca; va rimossa
            HttpContext.Current.Session.Remove("selectedAttachmentId");
            HttpContext.Current.Session.Remove("listDocVersions");

            this.SelectedGrid = GridManager.GetStandardGridForUser(GridTypeEnumeration.Document, UIManager.UserManager.GetInfoUser());
            this.SelectedGrid.Fields.Where(e => e.FieldId == "U1").FirstOrDefault().Visible = true;
            this.SelectedGrid.Fields.Where(e => e.FieldId == "DTA_ADL").FirstOrDefault().Visible = true;
            this.SelectedGrid.Fields.Where(e => e.FieldId == "DTA_ADL").FirstOrDefault().Position = 0;
            this.SelectedGrid.Fields.Where(e => e.FieldId == "MOTIVO_ADL").FirstOrDefault().Visible = true;

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
            UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);

            // Caricamento della griglia se non ce n'è una già selezionata
            if (this.SelectedGrid == null || this.SelectedGrid.GridType != GridTypeEnumeration.Document)
            {
                this.SelectedGrid = GridManager.GetStandardGridForUser(GridTypeEnumeration.Document, UIManager.UserManager.GetInfoUser());
            }

            this.ShowGrid(this.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            this.headerHomeLblRole.Text = Utils.Languages.GetLabelFromCode("HeaderHomeLblRole", language);
            this.RblTypeAdl.Items[0].Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdlUser", language);
            this.RblTypeAdl.Items[1].Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdlRole", language);
            this.DocumentViewer.Title = Utils.Languages.GetLabelFromCode("TitleDocumentViewerPopup", language);
            this.DigitalSignDetails.Title = Utils.Languages.GetLabelFromCode("DigitalSignDetailsTitle", language);
            this.MassiveReportDragAndDrop.Title = Utils.Languages.GetLabelFromCode("MassiveReportDragAndDropTitle", language);
        }

        protected void InitializePageSize()
        {
            //string keyValue = Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_PAGING_ROW_DOC.ToString());
            //int tempValue = 0;
            //if (!string.IsNullOrEmpty(keyValue))
            //{
            //    tempValue = Convert.ToInt32(keyValue);
            //    if (tempValue >= 20 || tempValue <= 50)
            //    {
            //        this.PageSize = tempValue;
            //    }
            //}

            this.PageSize = 10;
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

        private void LoadRolesUser()
        {
            try
            {
                ListItem item;
                Ruolo[] role = UIManager.UserManager.GetUserInSession().ruoli;
                string roleActive = RoleManager.GetRoleInSession().systemId;
                foreach (Ruolo r in role)
                {
                    item = new ListItem();
                    item.Value = r.systemId;
                    item.Text = r.descrizione;
                    if (r.systemId == roleActive) item.Selected = true;
                    this.ddlRolesUser.Items.Add(item);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
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
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
            fV1.valore = "true";
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
            fV1.valore = "true";
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
            fV1.valore = "true";
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

            #region filtro per Stampe Registro
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.STAMPA_REG.ToString();
            fV1.valore = "false";
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            #endregion

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
            fV1.valore = "true";
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
            fV1.valore = "true";
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
            fV1.valore = "tipo";
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            #endregion
            #region adl
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.DOC_IN_ADL.ToString();
            if (this.RblTypeAdl.SelectedValue == "0")
                fV1.valore = UserManager.GetUserInSession().idPeople.ToString() + "@" + RoleManager.GetRoleInSession().systemId.ToString();
            else
                fV1.valore = "0@" + RoleManager.GetRoleInSession().systemId.ToString();
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            #endregion
            #region filtro registro
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();

            string registri = "";
            DocsPaWR.Registro registro = UIManager.RegistryManager.GetRegistryInSession();
            if (registro == null)
            {
                registro = RoleManager.GetRoleInSession().registri[0];
            }
            foreach (DocsPaWR.Registro reg in RoleManager.GetRoleInSession().registri)
            {
                if (!reg.flag_pregresso)
                {
                    if (!string.IsNullOrEmpty(registri)) registri += ",";
                    registri += reg.systemId;
                }
            }
            if (!registri.Equals(""))
            {
                fV1.valore = registri;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region DA PROTOCOLLARE
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.DA_PROTOCOLLARE.ToString();
            fV1.valore = "0";  //corrisponde a 'false'
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            #endregion
            #region Filtro documenti consolidati
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_CONSOLIDAMENTO.ToString();
            fV1.valore = "0";
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            #endregion

            #region Ordinamento
            List<FiltroRicerca> filterList = this.GetOrderFilterDateADL(this.SelectedGrid);

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
        }

        public List<FiltroRicerca> GetOrderFilterDateADL(Grid SelGrid)
        {
            // Lista dei fitri da restituire
            List<FiltroRicerca> toReturn = new List<FiltroRicerca>();

            // Campo da utilizzare per l'ordinamento
            Field field = null;

            field = SelGrid.Fields.Where(e => e.FieldId == "DTA_ADL").FirstOrDefault();

            // Se il campo è valorizzato, vengono creati i filtri
            if (field != null)
            {
                // Se il campo è standard, vengono creati i due filtri di ricerca per SQL e per ORACLE
                if (field.CustomObjectId == 0)
                {
                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.ORACLE_FIELD_FOR_ORDER.ToString(),
                        valore = field.OracleDbColumnName,
                        nomeCampo = field.FieldId,
                    });

                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.SQL_FIELD_FOR_ORDER.ToString(),
                        valore = field.SqlServerDbColumnName,
                        nomeCampo = field.FieldId
                    });
                }
                else
                {
                    // Creazione di un filtro per la profilazione
                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.PROFILATION_FIELD_FOR_ORDER.ToString(),
                        valore = field.CustomObjectId.ToString(),
                        nomeCampo = field.FieldId
                    });
                }
            }


            string tempValue = string.Empty;

            if (field != null)
            {
                tempValue = field.FieldId;
            }

            // Aggiunta del filtro con indicazioni sulla direzione dell'ordinamento
            toReturn.Add(new FiltroRicerca()
            {
                argomento = FiltriDocumento.ORDER_DIRECTION.ToString(),
                nomeCampo = tempValue,
                valore = SelGrid.OrderDirection.ToString()
            });


            // Restituzione dei filtri creati
            return toReturn;
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

            if (outOfMaxRowSearchable)
            {
                string valoreChiaveDB = Utils.InitConfigurationKeys.GetValue("0", DBKeys.MAX_ROW_SEARCHABLE.ToString());
                if (valoreChiaveDB.Length == 0)
                {
                    valoreChiaveDB = Utils.InitConfigurationKeys.GetValue("0", DBKeys.MAX_ROW_SEARCHABLE.ToString());
                }
                string msgDesc = "WarningSearchRecordNumber";
                string msgCenter = Utils.Languages.GetMessageFromCode("WarningSearchRecordNumber2", UIManager.UserManager.GetUserLanguage());
                string customError = recordNumber + " " + msgCenter + " " + valoreChiaveDB;
                string errFormt = Server.UrlEncode(customError);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(customError) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(customError) + "');}; ", true);
                recordNumber = 0;
                return;
            }

            // Se ci sono risultati, vengono visualizzati
            if (this.Result != null && this.Result.Length > 0)
            {
                this.ShowResult(this.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels.ToArray<EtichettaInfo>());
            }
            else
            {
                this.ShowGrid(selectedGrid, null, 0, 0, labels);
                this.BuildGridNavigator();
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
            List<Field> visibleFieldsTemplate;

            if (this.SelectedGrid == null || this.SelectedGrid.GridType != GridTypeEnumeration.Document)
                this.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);



            visibleFieldsTemplate = this.SelectedGrid.Fields.Where(e => (e.Visible && e.GetType().Equals(typeof(Field)) && e.CustomObjectId != 0)).ToList();
            if (visibleFieldsTemplate != null && visibleFieldsTemplate.Count > 0)
            {
                visibleArray = visibleFieldsTemplate.ToArray();
            }

            documents = DocumentManager.getQueryInfoDocumentoPagingCustom(userInfo, this, searchFilters, selectedPage, out pageNumbers, out recordNumber, true, !IsPostBack, this.ShowGridPersonalization, this.PageSize, false, visibleArray, null, out idProfiles);

            /* ABBATANGELI GIANLUIGI
             * outOfMaxRowSearchable viene impostato a true se getQueryInfoDocumentoPagingCustom
             * restituisce pageNumbers = -2 (raggiunto il numero massimo di righe possibili come risultato di ricerca)*/
            outOfMaxRowSearchable = (pageNumbers == -2);

            this.RecordCount = recordNumber;
            this.PageCount = pageNumbers + 1;
            this.Result = documents;


            return documents;
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
                                        // value = DocumentManager.getSegnaturaRepertorio(this.Page, dNumber, UserManager.getInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione).Codice);
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
                            //MOTIVO INSERIMENTO IN ADL
                            case "MOTIVO_ADL":
                                //value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                if ((doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault() != null))
                                {
                                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                }
                                else
                                {
                                    value = "";
                                }
                                break;
                            //OGGETTI CUSTOM
                            default:
                                try
                                {
                                    if (!string.IsNullOrEmpty(doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue))
                                    {
                                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
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

                    dataRow["ProtoType"] = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D3")).FirstOrDefault().SearchObjectFieldValue;
                    dataRow["IdProfile"] = doc.SearchObjectID;
                    dataRow["FileExtension"] = !String.IsNullOrEmpty(immagineAcquisita) && immagineAcquisita != "0" ? immagineAcquisita : String.Empty;
                    dataRow["IsInStorageArea"] = !String.IsNullOrEmpty(inConservazione) && inConservazione != "0" ? true : false;
                    dataRow["IsInWorkingArea"] = !String.IsNullOrEmpty(inAdl) && inAdl != "0" ? true : false;
                    dataRow["IsInWorkingAreaRole"] = !String.IsNullOrEmpty(inAdlRole) && inAdlRole != "0" ? true : false;
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

        protected void BuildGridNavigator()
        {
            try
            {
                this.plcNavigator.Controls.Clear();

                int countPage = this.PageCount;

                int val = this.RecordCount % this.PageSize;
                if (val == 0)
                {
                    countPage = countPage - 1;
                }

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

        protected void GridView_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
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
                actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME_ADL_DOCUMENT.ToString(), string.Empty);
                actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.HOME_ADL_DOCUMENT.ToString(), true, this.Page);
                actualPage.CodePage = Navigation.NavigationUtils.NamePage.HOME_ADL_DOCUMENT.ToString();
                actualPage.Page = "ADLDOCUMENT.ASPX";
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

                int indexElement = (((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1)) + 1;
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