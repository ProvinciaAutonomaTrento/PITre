using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using System.Data;
using NttDataWA.Utils;
using NttDatalLibrary;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Collections;
using System.IO;

namespace NttDataWA.Search
{
    public partial class SearchProjects : System.Web.UI.Page
    {

        #region Fields

        private const string KEY_SCHEDA_RICERCA = "RicercaFascicoli";
        public SearchManager schedaRicerca = null;

        #endregion

        #region Properties

        protected int MaxLenghtProject
        {
            get
            {
                int result = 2000;
                if (HttpContext.Current.Session["maxLenghtProject"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["maxLenghtProject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["maxLenghtProject"] = value;
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
        public int PagesCount
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

        /// <summary>
        /// True  se biusogna ricercare anche fra i figli del fascicolo
        /// </summary>
        public bool AllClassification
        {
            get
            {
                // Valore da restituire
                bool toReturn = false;

                if (HttpContext.Current.Session["allClassification"] != null)
                    Boolean.TryParse(
                        HttpContext.Current.Session["allClassification"].ToString(),
                        out toReturn);

                return toReturn;
            }

            set
            {
                HttpContext.Current.Session["allClassification"] = value;
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

        private DocsPaWR.Fascicolo Project
        {
            get
            {
                Fascicolo result = null;
                if (HttpContext.Current.Session["searchproject"] != null)
                {
                    result = HttpContext.Current.Session["searchproject"] as Fascicolo;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["searchproject"] = value;
            }
        }

        /// <summary>
        /// valore di ritorno della popup del titolario
        /// </summary>
        private string ReturnValue
        {
            get
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["ReturnValuePopup"].ToString()))
                    return HttpContext.Current.Session["ReturnValuePopup"].ToString();
                else
                    return string.Empty;
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

        private DocsPaWR.Ruolo Role
        {
            get
            {
                return UIManager.RoleManager.GetRoleInSession();
            }
            set
            {
                HttpContext.Current.Session["role"] = value;
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

        private bool IsAdl
        {
            get
            {
                return Request.QueryString["IsAdl"] != null ? true : false;
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


                if (!this.IsPostBack)
                {
                    this.InitializePage();

                    if (this.IsAdl)
                    {
                        bool result = this.ricercaFascicoli();
                        if (result)
                        {
                            this.SelectedRow = string.Empty;
                            this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid);
                        }
                        this.UpnlNumerodocumenti.Update();
                        this.UpnlGrid.Update();
                    }

                    //Back
                    if (this.Request.QueryString["back"] != null && this.Request.QueryString["back"].Equals("1"))
                    {
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject obj = navigationList.Last();
                        if (!obj.CodePage.Equals(Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString()))
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
                        ProjectManager.setFiltroRicFasc(this, schedaRicerca.FiltriRicerca);

                        if (!this.ShowGridPersonalization)
                        {
                            List<Field> visibleFields = GridManager.SelectedGrid.Fields.Where(x => x.Visible && x.GetType().Equals(typeof(Field))).ToList();
                            Field specialField = GridManager.SelectedGrid.Fields.Where(x => x.Visible && x.GetType().Equals(typeof(SpecialField)) && ((SpecialField)x).FieldType.Equals(SpecialFieldsEnum.Icons)).FirstOrDefault<Field>();

                            if (this.Template != null)
                            {
                                Session["templateRicerca"] = this.Template;
                            }
                        }

                        this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid);

                        if (!string.IsNullOrEmpty(obj.OriginalObjectId))
                        {
                            string idProject = string.Empty;
                            foreach (GridViewRow grd in this.gridViewResult.Rows)
                            {
                                idProject = string.Empty;

                                if (GrigliaResult.Rows[grd.RowIndex]["IdProject"] != null)
                                {
                                    idProject = GrigliaResult.Rows[grd.RowIndex]["IdProject"].ToString();
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

                    this.VisibiltyRoleFunctions();
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
                            this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid);
                            this.BuildGridNavigator();
                            this.UpnlNumerodocumenti.Update();
                            this.UpnlGrid.Update();
                            this.upPnlGridIndexes.Update();

                            // riposiziono lo scroll del div in cima
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ScrollGridViewOnTop", "setFocusOnTop();", true);
                        }
                        else
                        {
                            this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage);
                        }
                    }

                    this.setValuePopUp();
                }


                if (this.CustomDocuments)
                {
                    this.PnlTypeDocument.Controls.Clear();
                    if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
                    {
                        if (this.Template == null || !this.Template.SYSTEM_ID.ToString().Equals(this.DocumentDdlTypeDocument.SelectedValue))
                        {
                            this.Template = ProfilerProjectManager.getTemplateFascById(this.DocumentDdlTypeDocument.SelectedItem.Value);
                        }
                        if (this.CustomDocuments)
                        {
                            this.PopulateProfiledDocument();
                        }
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

        private void VisibiltyRoleFunctions()
        {
            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_CLA_TITOLARIO"))
            {
                this.btnclassificationschema.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_CONS"))
            {
                this.PlcPreservation.Visible = false;
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
            {
                this.AllowADLRole = true;
                if (this.IsAdl)
                {
                    this.PlcAdl.Visible = true;
                }
            }

            if (!this.ShowGridPersonalization)
            {
                this.projectImgSaveGrid.Visible = false;
                this.projectImgEditGrid.Visible = false;
                this.projectImgPreferredGrids.Visible = false;
            }

            if (IsConservazioneSACER)
            {
                this.phConservation.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("FASC_NUOVO"))
            {
                this.SearchProjectNewProject.Visible = false;
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

                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    string idProject = this.GrigliaResult.Rows[e.Row.DataItemIndex]["IdProject"].ToString();
                    string codeProject = string.Empty;//this.GrigliaResult.Rows[e.Row.DataItemIndex]["P3"].ToString();

                    CheckBox checkBox = e.Row.FindControl("checkDocumento") as CheckBox;
                    if (checkBox != null)
                    {
                        checkBox.CssClass = "pr" + idProject;
                        checkBox.Attributes["onclick"] = "SetProjectCheck(this, '" + idProject + "_" + codeProject + "')";
                        if (this.ListCheck.ContainsKey(idProject))
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
                            this.HiddenProjectsAll.Value = "true";
                        else
                            this.HiddenProjectsAll.Value = string.Empty;
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
                        d.OracleDbColumnName = "to_number(getContatoreFascContatore (a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "'))";
                        d.SqlServerDbColumnName = "@dbUser@.getContatoreFascContatore(a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "')";
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
                        this.ricercaFascicoli();
                        this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid);
                    }
                    else
                    {
                        this.ShowGrid(GridManager.SelectedGrid, null, 0, 0);
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

        protected void SearchProjectRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ddlSavedSearches.SelectedIndex > 0)
                {
                    string id = this.ddlSavedSearches.SelectedValue;
                    DocsPaWR.SearchItem item = SearchManager.GetItemSearch(Int32.Parse(id));

                    DocsPaWR.Ruolo ruolo = null;
                    if (item.owner_idGruppo != 0)
                        ruolo = RoleManager.GetRoleInSession();

                    string msg = "Il criterio di ricerca con nome '" + this.ddlSavedSearches.SelectedItem.ToString() + "' verra' rimosso.<br />";
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

        protected void SearchProjectRemoveFilters_Click(object sender, EventArgs e)
        {
            try
            {
                this.Classification = null;
                this.DdlRegistries.SelectedIndex = 0;
                this.ddlTitolario.SelectedIndex = 0;
                this.IdProject.Value = string.Empty;
                this.TxtCodeProject.Text = string.Empty;
                this.TxtDescriptionProject.Text = string.Empty;
                this.TxtDescrizione.Text = string.Empty;
                this.TxtNumProject.Text = string.Empty;
                this.TxtYear.Text = string.Empty;
                this.ddlStatus.SelectedIndex = 0;
                this.ddl_dtaOpen.SelectedIndex = 0;
                this.dtaOpen_TxtFrom.Text = string.Empty;
                this.dtaOpen_TxtTo.Text = string.Empty;
                this.ddl_dtaClose.SelectedIndex = 0;
                this.dtaClose_TxtFrom.Text = string.Empty;
                this.dtaClose_TxtTo.Text = string.Empty;
                this.ddl_dtaCreate.SelectedIndex = 0;
                this.dtaCreate_TxtFrom.Text = string.Empty;
                this.dtaCreate_TxtTo.Text = string.Empty;
                this.ddl_dtaExpire.SelectedIndex = 0;
                this.dtaExpire_TxtFrom.Text = string.Empty;
                this.dtaExpire_TxtTo.Text = string.Empty;
                this.ddl_dtaCollocation.SelectedIndex = 0;
                this.dtaCollocation_TxtFrom.Text = string.Empty;
                this.dtaCollocation_TxtTo.Text = string.Empty;
                this.idCollocationAddr.Value = string.Empty;
                this.txtCodiceCollocazione.Text = string.Empty;
                this.txtDescrizioneCollocazione.Text = string.Empty;
                this.txt_subProject.Text = string.Empty;
                this.TxtNoteProject.Text = string.Empty;
                this.rblFilterNote.Items.FindByValue("Q").Selected = true;
                this.rbViewAllYes.Checked = true;
                this.rbViewAllNo.Checked = false;
                this.chkConservation.Checked = false;
                this.chkConservationNo.Checked = false;
                this.rblOwnerType.Items.FindByValue("R").Selected = true;
                this.idCreatore.Value = string.Empty;
                this.txtCodiceCreatore.Text = string.Empty;
                this.txtDescrizioneCreatore.Text = string.Empty;
                this.chkCreatoreExtendHistoricized.Checked = false;
                this.rblProprietarioType.Items.FindByValue("R").Selected = true;
                this.idProprietario.Value = string.Empty;
                this.txtCodiceProprietario.Text = string.Empty;
                this.txtDescrizioneProprietario.Text = string.Empty;
                this.Classification = null;

                //this.PopulateDDLSavedSearches();
                //this.PopulateDDLTitolario();

                if (sender != null)
                {
                    this.ddlSavedSearches.SelectedIndex = 0;
                    this.ddlSavedSearches_SelectedIndexChanged(null, null);
                    this.SearchProjectEdit.Enabled = false;
                    this.SearchProjectRemove.Enabled = false;
                    this.upPnlButtons.Update();
                }

                this.ddl_dtaOpen_SelectedIndexChanged(null, null);
                this.ddl_dtaClose_SelectedIndexChanged(null, null);
                this.ddl_dtaCreate_SelectedIndexChanged(null, null);
                this.ddl_dtaExpire_SelectedIndexChanged(null, null);
                this.ddl_dtaCollocation_SelectedIndexChanged(null, null);

                this.UpPnlSavedSearches.Update();
                this.UpPnlRegistry.Update();
                this.UpPnlTitolario.Update();
                this.UpPnlProject.Update();
                this.UpPnlDescription.Update();
                this.UpPnlGeneric.Update();
                this.upPnlIntervals.Update();
                this.upPnlCreatore.Update();
                this.upPnlProprietario.Update();
                this.UpPnlNote.Update();
                this.UpPnlDocType.Update();
                this.upPnlCollocationAddr.Update();
                this.upPnlCollocation.Update();
                this.UpPnlConservation.Update();
                this.UpPnlViewAll.Update();
                this.Template = null;
                this.PnlTypeDocument.Controls.Clear();
                this.DocumentDdlTypeDocument.SelectedIndex = -1;
                if (this.EnableStateDiagram)
                {
                    this.DocumentDdlStateDiagram.ClearSelection();
                    this.PnlStateDiagram.Visible = false;
                    this.ddlStateCondition.Visible = false;
                }
                this.UpPnlTypeDocument.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchProjectSearch_Click(object sender, EventArgs e)
        {
            try
            {
                this.ListCheck = new Dictionary<string, string>();
                bool result = this.ricercaFascicoli();
                if (result)
                {
                    this.SelectedRow = string.Empty;
                    this.SelectedPage = 1;
                    this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid);
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

        protected void gridViewResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    string idProject = GrigliaResult.Rows[e.Row.DataItemIndex]["IdProject"].ToString();

                    string labelConservazione = "ProjectIconTemplateRemoveConservazione";
                    string labelAdl = "ProjectIconTemplateRemoveAdlProject";
                    string labelAdlRole = "ProjectIconTemplateRemoveAdlRoleProject";

                    //imagini delle icone

                    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).ImageUrl = "../Images/Icons/ricerca-fasc-1.png";
                    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).OnMouseOutImage = "../Images/Icons/ricerca-fasc-1.png";
                    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).OnMouseOverImage = "../Images/Icons/ricerca-fasc-1_hover.png";
                    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).ImageUrlDisabled = "../Images/Icons/ricerca-fasc-1_disabled.png";

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
                        ((CustomImageButton)e.Row.FindControl("conservazione")).ImageUrlDisabled = "../Images/Icons/add_preservation_grid_disabled.png";

                    }

                    labelAdlRole = "ProjectIconTemplateRemoveAdlRoleProject";
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
                                labelAdlRole = "ProjectIconTemplateAdlRoleProject";
                            else
                                labelAdlRole = "ProjectIconTemplateAdlRoleProjectInsert";
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
                        labelAdl = "ProjectIconTemplateAdlProject";
                        ((CustomImageButton)e.Row.FindControl("adl")).ImageUrl = "../Images/Icons/adl2.png";
                        ((CustomImageButton)e.Row.FindControl("adl")).OnMouseOutImage = "../Images/Icons/adl2.png";
                        ((CustomImageButton)e.Row.FindControl("adl")).OnMouseOverImage = "../Images/Icons/adl2_hover.png";
                        ((CustomImageButton)e.Row.FindControl("adl")).ImageUrlDisabled = "../Images/Icons/adl2_disabled.png";
                    }


                    ((CustomImageButton)e.Row.FindControl("conservazione")).Visible = this.AllowConservazione && !this.IsConservazioneSACER;
                    ((CustomImageButton)e.Row.FindControl("adl")).Visible = this.AllowADL;
                    ((CustomImageButton)e.Row.FindControl("adlrole")).Visible = this.AllowADLRole;

                    //evento click
                    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).Click += new ImageClickEventHandler(ImageButton_Click);
                    ((CustomImageButton)e.Row.FindControl("conservazione")).Click += new ImageClickEventHandler(ImageButton_Click);
                    ((CustomImageButton)e.Row.FindControl("adl")).Click += new ImageClickEventHandler(ImageButton_Click);
                    ((CustomImageButton)e.Row.FindControl("adlrole")).Click += new ImageClickEventHandler(ImageButton_Click);

                    //tooltip
                    ((CustomImageButton)e.Row.FindControl("conservazione")).ToolTip = Utils.Languages.GetLabelFromCode(labelConservazione, UIManager.UserManager.GetUserLanguage());
                    ((CustomImageButton)e.Row.FindControl("adl")).ToolTip = Utils.Languages.GetLabelFromCode(labelAdl, UIManager.UserManager.GetUserLanguage());
                    ((CustomImageButton)e.Row.FindControl("adlRole")).ToolTip = Utils.Languages.GetLabelFromCode(labelAdlRole, UIManager.UserManager.GetUserLanguage());
                    ((CustomImageButton)e.Row.FindControl("visualizzadocumento")).ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateVisualizzaFascicolo", UIManager.UserManager.GetUserLanguage());

                    ((CustomImageButton)e.Row.FindControl("firmato")).Visible = false;
                    ((CustomImageButton)e.Row.FindControl("estensionedoc")).Visible = false;
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
            string language = UIManager.UserManager.GetUserLanguage();
            string idProject = GrigliaResult.Rows[rowIndex]["IdProject"].ToString();
            Fascicolo fascicolo = UIManager.ProjectManager.getFascicoloById(idProject);
            fascicolo.template = ProfilerProjectManager.getTemplateFascDettagli(fascicolo.systemID);
            InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();

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
                        string result = string.Empty;
                        int resultIndex = Result.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.SearchObjectID.Equals(idProject)).index;
                        int resultIndex2 = Result[resultIndex].SearchObjectField.Select((item2, j) => new { obj2 = item2, index2 = j }).First(item2 => item2.obj2.SearchObjectFieldID.Equals("P13")).index2;

                        if (bool.Parse(this.GrigliaResult.Rows[rowIndex]["IsInStorageArea"].ToString()))
                        {
                            result = this.RemoveProjectFromStorageArea(fascicolo.systemID);
                            GrigliaResult.Rows[rowIndex]["IsInStorageArea"] = false;
                            Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "0";
                            btnIm.ImageUrl = "../Images/Icons/add_preservation_grid.png";
                            btnIm.OnMouseOutImage = "../Images/Icons/add_preservation_grid.png";
                            btnIm.OnMouseOverImage = "../Images/Icons/add_preservation_grid_hover.png";
                            btnIm.ImageUrlDisabled = "../Images/Icons/add_preservation_grid_disabled.png";
                            btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateConservazione", language);
                            btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateConservazione", language);


                            if (!string.IsNullOrEmpty(result))
                            {
                                string msgDesc = "WarningDocumentCustom";
                                string errFormt = Server.UrlEncode(result);
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');}; ", true);
                            }
                        }
                        else
                        {
                            result = this.InsertProjectInStorageArea(fascicolo.systemID);

                            if (!string.IsNullOrEmpty(result))
                            {
                                string msgDesc = "WarningDocumentCustom";
                                string errFormt = Server.UrlEncode(result);
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');}; ", true);
                            }
                            else
                            {
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


                        break;
                    }
                case "adl":
                    {
                        int resultIndex = Result.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.SearchObjectID.Equals(idProject)).index;
                        int resultIndex2 = Result[resultIndex].SearchObjectField.Select((item2, j) => new { obj2 = item2, index2 = j }).First(item2 => item2.obj2.SearchObjectFieldID.Equals("IN_ADL")).index2;

                        if (bool.Parse(GrigliaResult.Rows[rowIndex]["IsInWorkingArea"].ToString()))
                        {
                            ProjectManager.eliminaFascicoloDaAreaDiLavoro(fascicolo, infoUtente);
                            GrigliaResult.Rows[rowIndex]["IsInWorkingArea"] = false;
                            Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "0";
                            btnIm.ImageUrl = "../Images/Icons/adl2.png";
                            btnIm.OnMouseOutImage = "../Images/Icons/adl2.png";
                            btnIm.OnMouseOverImage = "../Images/Icons/adl2_hover.png";
                            btnIm.ImageUrlDisabled = "../Images/Icons/adl2_disabled.png";
                            btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateAdlProject", language);
                            btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateAdlProject", language);

                            //if (this.IsAdl)
                            //{
                            //    this.SelectedRow = string.Empty;
                            //    this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid);
                            //}
                        }
                        else
                        {
                            ProjectManager.addFascicoloInAreaDiLavoro(fascicolo, infoUtente);
                            GrigliaResult.Rows[rowIndex]["IsInWorkingArea"] = true;
                            Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "1";
                            btnIm.ImageUrl = "../Images/Icons/adl2x.png";
                            btnIm.OnMouseOutImage = "../Images/Icons/adl2x.png";
                            btnIm.OnMouseOverImage = "../Images/Icons/adl2x_hover.png";
                            btnIm.ImageUrlDisabled = "../Images/Icons/adl2x_disabled.png";
                            btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveAdlProject", language);
                            btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveAdlProject", language);
                        }
                        this.SelectedRow = string.Empty;
                        this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid);
                        break;
                    }
                case "adlrole":
                    {
                        int resultIndex = Result.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.SearchObjectID.Equals(idProject)).index;
                        int resultIndex2 = Result[resultIndex].SearchObjectField.Select((item2, j) => new { obj2 = item2, index2 = j }).First(item2 => item2.obj2.SearchObjectFieldID.Equals("IN_ADLROLE")).index2;

                        if (bool.Parse(GrigliaResult.Rows[rowIndex]["IsInWorkingAreaRole"].ToString()))
                        {
                            ProjectManager.eliminaFascicoloDaAreaDiLavoroRole(fascicolo, infoUtente);
                            GrigliaResult.Rows[rowIndex]["IsInWorkingAreaRole"] = false;
                            Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "0";
                            btnIm.ImageUrl = "../Images/Icons/adl1.png";
                            btnIm.OnMouseOutImage = "../Images/Icons/adl1.png";
                            btnIm.OnMouseOverImage = "../Images/Icons/adl1_hover.png";
                            btnIm.ImageUrlDisabled = "../Images/Icons/adl1_disabled.png";
                            btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateAdlProject", language);
                            btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateAdlProject", language);
                            //if (this.IsAdl)
                            //{
                            //    this.SelectedRow = string.Empty;
                            //    this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels.ToArray<EtichettaInfo>());
                            //}
                        }
                        else
                        {
                            ProjectManager.addFascicoloInAreaDiLavoroRole(fascicolo, infoUtente);
                            GrigliaResult.Rows[rowIndex]["IsInWorkingAreaRole"] = true;
                            Result[resultIndex].SearchObjectField[resultIndex2].SearchObjectFieldValue = "1";
                            btnIm.ImageUrl = "../Images/Icons/adl1x.png";
                            btnIm.OnMouseOutImage = "../Images/Icons/adl1x.png";
                            btnIm.OnMouseOverImage = "../Images/Icons/adl1x_hover.png";
                            btnIm.ImageUrlDisabled = "../Images/Icons/adl1x_disabled.png";
                            btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveAdlProject", language);
                            btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateRemoveAdlProject", language);
                        }
                        this.SelectedRow = string.Empty;
                        this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid);
                        break;
                    }
                case "visualizzadocumento":
                    {
                        //List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        //Navigation.NavigationObject obj = navigationList.Last();
                        //navigationList.Remove(obj);
                        //obj.IdObject = fascicolo.systemID;
                        //obj.OriginalObjectId = fascicolo.systemID;
                        //obj.NumPage = this.SelectedPage.ToString();
                        //obj.DxTotalPageNumber = this.PagesCount.ToString();
                        //obj.DxTotalNumberElement = this.RecordCount.ToString();
                        //obj.NumPage = this.SelectedPage.ToString();
                        //obj.ViewResult = true;
                        //obj.SearchFilters = this.SearchFilters;
                        //obj.Type = string.Empty;
                        //obj.RegistryFilter = this.Registry;
                        //obj.Classification = this.Classification;
                        //obj.PageSize = this.PageSize.ToString();
                        //obj.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString(), string.Empty);
                        //obj.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString(), true);
                        //obj.CodePage = Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString();
                        //obj.Page = "SEARCHPROJECT.ASPX";
                        //int indexElement = ((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1);
                        //obj.DxPositionElement = indexElement.ToString();
                        //navigationList.Add(obj);
                        //Navigation.NavigationUtils.SetNavigationList(navigationList);
                        //UIManager.ProjectManager.setProjectInSession(fascicolo);
                        //Response.Redirect("~/Project/project.aspx");
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject obj = new Navigation.NavigationObject();
                        obj.IdObject = fascicolo.systemID;
                        obj.OriginalObjectId = fascicolo.systemID;
                        obj.SearchFilters = this.SearchFilters;
                        obj.NumPage = this.SelectedPage.ToString();
                        obj.DxTotalPageNumber = this.PagesCount.ToString();
                        obj.DxTotalNumberElement = this.RecordCount.ToString();
                        obj.ViewResult = true;
                        obj.Type = string.Empty;
                        obj.RegistryFilter = this.Registry;
                        obj.Classification = this.Classification;
                        obj.PageSize = this.PageSize.ToString();
                        obj.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString(), string.Empty);
                        obj.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString(), true, this.Page);
                        obj.CodePage = Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString();
                        obj.Page = "SEARCHPROJECT.ASPX";

                        int indexElement = ((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1);
                        obj.DxPositionElement = indexElement.ToString();
                        navigationList.Add(obj);
                        Navigation.NavigationUtils.SetNavigationList(navigationList);
                        UIManager.ProjectManager.setProjectInSession(fascicolo);
                        Response.Redirect("~/Project/project.aspx");
                        break;
                    }

            }
        }

        /// <summary>
        /// Funzione per la rimozione dall'area di conservazione
        /// di documenti contenuti in un fascicolo
        /// </summary>
        /// <param name="objectId">Id del fascicolo di cui rimuovere i documenti</param>
        /// <returns>Eventuale messaggio da mostrare all'utente</returns>
        private String RemoveProjectFromStorageArea(String objectId)
        {

            // Lista dei system id dei documenti contenuti nel fasciolo
            string[] idProjects = ProjectManager.getIdDocumentiFromFascicolo(objectId);

            // Risultato da restituire
            StringBuilder toReturn = new StringBuilder();

            // Risultato di un inserimento
            String temp;

            // Inserimento di tutti i documenti nell'area di conservazione
            foreach (String id in idProjects)
            {
                temp = this.RemoveDocumentFromStorageArea(id);

                //if (!String.IsNullOrEmpty(temp))
                //{
                //    temp = temp + "<br/>";
                //    toReturn.AppendLine(temp);
                //}
            }

            // Restituzione del risultato
            return toReturn.ToString();
        }


        /// <summary>
        /// Funzione per l'inserimento di documenti presenti in un fascicolo nell'area di
        /// conservazione
        /// </summary>
        /// <param name="objectId">Id del fascicolo di cui inserire i documenti nell'area di conservazione</param>
        /// <rereturns>Eventuale messaggio da mostrare all'utente</rereturns>
        private String InsertProjectInStorageArea(String objectId)
        {
            // Lista dei system id dei documenti contenuti nel fasciolo
            string[] idProjects = ProjectManager.getIdDocumentiFromFascicolo(objectId);

            // Risultato da restituire
            StringBuilder toReturn = new StringBuilder();

            // Risultato di un inserimento
            String temp;

            // Se il fascicolo non contiene documenti, viene preparato un messaggio
            // apposito
            if (idProjects.Length == 0)
            {
                toReturn = new StringBuilder(Utils.Languages.GetMessageFromCode("WarningPreservetionPrjEmpty", UIManager.UserManager.GetUserLanguage()));
            }

            #region OldCode
            // Inserimento di tutti i documenti nell'area di conservazione
            /*
            foreach (String id in idProjects)
            {
                temp = this.InsertDocumentInStorageArea(id, objectId);

                if (!String.IsNullOrEmpty(temp))
                    toReturn.AppendLine(temp);
            }
            */
            #endregion

            // Mev CS 1.5 - F03_01
            // New Code
            #region NewCode
            // Indice dell'elemento lavorato
            int indexOfCurrentDoc = 0;

            // Dimensione complessiva raggiunta
            int currentDimOfDocs = 0;

            // Get valori limiti per le istanze di conservazione
            int DimMaxInIstanza = 0;
            int numMaxDocInIstanza = 0;
            int TolleranzaPercentuale = 0;
            try
            {
                InfoUtente infoUt = UserManager.GetInfoUser();
                DimMaxInIstanza = DocumentManager.getDimensioneMassimaIstanze(infoUt.idAmministrazione);
                numMaxDocInIstanza = DocumentManager.getNumeroDocMassimoIstanze(infoUt.idAmministrazione);
                TolleranzaPercentuale = DocumentManager.getPercentualeTolleranzaDinesioneIstanze(infoUt.idAmministrazione);
            }
            catch (Exception ex)
            {
            }

            foreach (String id in idProjects)
            {
                temp = this.InsertDocumentInStorageArea_WithConstraint(id,
                    objectId,
                    ref indexOfCurrentDoc,
                    ref currentDimOfDocs,
                    DimMaxInIstanza,
                    numMaxDocInIstanza,
                    TolleranzaPercentuale);

                if (!String.IsNullOrEmpty(temp))
                    toReturn.AppendLine(temp);
            }
            #endregion
            //End New Code

            // Restituzione del risultato
            return toReturn.ToString();

        }

        /// <summary>
        /// Funzione per la rimozione del documento dall'area di conservazione
        /// </summary>
        private String RemoveDocumentFromStorageArea(String objectId)
        {
            // Eventuale messaggio da mostrare
            String toReturn = String.Empty;

            SchedaDocumento selectedDocument = DocumentManager.getDocumentDetails(
                this.Page,
                objectId,
                String.Empty);

            Fascicolo selectedProject;

            // Se il documento può essere rimosso dall'area di conservazione
            if ((DocumentManager.canDeleteAreaConservazione(
                selectedDocument.systemId,
                UserManager.GetInfoUser().idPeople,
                UserManager.GetInfoUser().idGruppo)))
                // Viene mostrato un messaggio all'utente
                toReturn = Utils.Languages.GetMessageFromCode("WarningRemoveDocumentPreservation", UIManager.UserManager.GetUserLanguage());
            else
            {
                // Reperimento del fascicolo selezionato
                selectedProject = ProjectManager.getProjectInSession();

                DocumentManager.eliminaDaAreaConservazione(
                    selectedDocument.systemId,
                    selectedProject,
                    null,
                    false,
                    String.Empty);

            }

            return toReturn;

        }

        /// <summary>
        /// Funzione per la procedura di inserimento del documento in area di conservazione
        /// </summary>
        private String InsertDocumentInStorageArea(String documentID, String projectId)
        {
            // Eventuale messaggio da restituire
            String toReturn = String.Empty;

            // Dettaglio del documento selezionato
            SchedaDocumento selectedDocument = DocumentManager.getDocumentDetails(
                this.Page,
                documentID,
                String.Empty);

            // Il fascicolo selezionato
            if (String.IsNullOrEmpty(projectId))
            {
                Fascicolo f = ProjectManager.getProjectInSession();
                if (f != null && String.IsNullOrEmpty(f.systemID))
                    projectId = string.Empty;
            }

            // Identificativo dell'istanza di certificazione
            int conservationAreaId;

            // Se il documento ha un file acquisito...
            if (Convert.ToInt32(selectedDocument.documenti[0].fileSize) > 0)
            {
                //// Se si sta creando la prima istanza di conservazione...
                //if (DocumentManager.isPrimaConservazione(
                //    UserManager.GetInfoUser().idPeople,
                //    UserManager.GetInfoUser().idGruppo) == 1)
                //    // Viene visualizzato un messaggio all'utente
                //    toReturn = "E' stata creata una nuova istanza di conservazione.";

                string tipo = string.Empty;
                if (!string.IsNullOrEmpty(projectId))
                {
                    tipo = "F";
                }
                else
                {
                    tipo = "D";
                }
                conservationAreaId = DocumentManager.addAreaConservazione(
                    documentID,
                    projectId,
                    selectedDocument.docNumber,
                    UserManager.GetInfoUser(),
                    tipo);

                // Se l'identificativo non è -1 vengono aggiornati i dati sulla conservazione
                if (conservationAreaId.ToString() != "-1")
                {
                    int size_xml = DocumentManager.getItemSize(
                        selectedDocument,
                        conservationAreaId.ToString());

                    int doc_size = Convert.ToInt32(selectedDocument.documenti[0].fileSize);

                    int numeroAllegati = selectedDocument.allegati.Length;
                    string fileName = selectedDocument.documenti[0].fileName;
                    string tipoFile = Path.GetExtension(fileName);
                    int size_allegati = 0;
                    for (int i = 0; i < selectedDocument.allegati.Length; i++)
                    {
                        size_allegati = size_allegati + Convert.ToInt32(selectedDocument.allegati[i].fileSize);
                    }
                    int total_size = size_allegati + doc_size + size_xml;

                    DocumentManager.insertSizeInItemCons(conservationAreaId.ToString(), total_size);

                    DocumentManager.updateItemsConservazione(
                        tipoFile,
                        Convert.ToString(numeroAllegati),
                        conservationAreaId.ToString());
                }

            }
            return toReturn;
        }

        /// <summary>
        /// Funzione per la procedura di inserimento del documento in area di conservazione rispettando i vincoli di dimesione delle istanze
        /// </summary>
        private String InsertDocumentInStorageArea_WithConstraint(String documentID,
            String projectId,
            ref int indexOfCurrentDoc,
            ref int currentDimOfDocs,
            int DimMaxInIstanza,
            int numMaxDocInIstanza,
            int percentualeTolleranza
            )
        {
            // Eventuale messaggio da restituire
            String toReturn = String.Empty;

            // Dettaglio del documento selezionato
            SchedaDocumento selectedDocument = DocumentManager.getDocumentDetails(
                this.Page,
                documentID,
                String.Empty);

            // Il fascicolo selezionato
            if (String.IsNullOrEmpty(projectId))
            {
                Fascicolo f = ProjectManager.getProjectInSession();
                if (f != null && String.IsNullOrEmpty(f.systemID))
                    projectId = string.Empty;
            }

            // Identificativo dell'istanza di certificazione
            int conservationAreaId;

            // Se il documento ha un file acquisito...
            if (Convert.ToInt32(selectedDocument.documenti[0].fileSize) > 0)
            {
                string tipo = string.Empty;
                if (!string.IsNullOrEmpty(projectId))
                {
                    tipo = "F";
                }
                else
                {
                    tipo = "D";
                }

                // Controllo Rispetto dei Vincoli dell'istanza
                #region Vincoli Istanza di Conservazione
                // Variabili di controllo per violazione dei vincoli sulle istanze
                bool numDocIstanzaViolato = false;
                bool dimIstanzaViolato = false;
                int TotalSelectedDocumentSize = 0;

                TotalSelectedDocumentSize = DocumentManager.GetTotalDocumentSize(selectedDocument);
                // Dimensione documenti raggiunta
                currentDimOfDocs = TotalSelectedDocumentSize + currentDimOfDocs;
                // Numero di documenti raggiunti
                indexOfCurrentDoc = indexOfCurrentDoc + 1;

                numDocIstanzaViolato = DocumentManager.isVincoloNumeroDocumentiIstanzaViolato(indexOfCurrentDoc, numMaxDocInIstanza);
                dimIstanzaViolato = DocumentManager.isVincoloDimensioneIstanzaViolato(currentDimOfDocs, DimMaxInIstanza, percentualeTolleranza);

                double DimensioneMassimaConsentitaPerIstanza = 0;
                DimensioneMassimaConsentitaPerIstanza = DimMaxInIstanza - ((DimMaxInIstanza * percentualeTolleranza) / 100);

                int DimMaxConsentita = 0;
                DimMaxConsentita = Convert.ToInt32(DimensioneMassimaConsentitaPerIstanza);

                if (numDocIstanzaViolato || dimIstanzaViolato)
                {
                    // Azzero le due variabili
                    currentDimOfDocs = 0;
                    indexOfCurrentDoc = 0;
                }
                #endregion

                // Invio in conservazione nel rispetto dei vincoli
                conservationAreaId = DocumentManager.addAreaConservazione_WithConstraint(
                    documentID,
                    projectId,
                    selectedDocument.docNumber,
                    UserManager.GetInfoUser(),
                    tipo,
                    numDocIstanzaViolato,
                    dimIstanzaViolato,
                    DimMaxConsentita,
                    numMaxDocInIstanza,
                    TotalSelectedDocumentSize
                    );

                // Se l'identificativo non è -1 vengono aggiornati i dati sulla conservazione
                if (conservationAreaId.ToString() != "-1")
                {
                    int size_xml = DocumentManager.getItemSize(
                        selectedDocument,
                        conservationAreaId.ToString());

                    int doc_size = Convert.ToInt32(selectedDocument.documenti[0].fileSize);

                    int numeroAllegati = selectedDocument.allegati.Length;
                    string fileName = selectedDocument.documenti[0].fileName;
                    string tipoFile = Path.GetExtension(fileName);
                    int size_allegati = 0;
                    for (int i = 0; i < selectedDocument.allegati.Length; i++)
                    {
                        size_allegati = size_allegati + Convert.ToInt32(selectedDocument.allegati[i].fileSize);
                    }
                    int total_size = size_allegati + doc_size + size_xml;

                    DocumentManager.insertSizeInItemCons(conservationAreaId.ToString(), total_size);

                    DocumentManager.updateItemsConservazione(
                        tipoFile,
                        Convert.ToString(numeroAllegati),
                        conservationAreaId.ToString());
                }

            }
            return toReturn;
        }

        protected void DocumentDdlTypeDocument_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
                {
                    if (this.CustomDocuments)
                    {
                        this.Template = ProfilerProjectManager.getTemplateFascById(this.DocumentDdlTypeDocument.SelectedItem.Value);
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
                                string idDiagramma = DiagrammiManager.getDiagrammaAssociatoFasc(this.DocumentDdlTypeDocument.SelectedValue).ToString();
                                if (!string.IsNullOrEmpty(idDiagramma) && !idDiagramma.Equals("0"))
                                {
                                    this.PnlStateDiagram.Visible = true;

                                    //Inizializzazione comboBox
                                    this.DocumentDdlStateDiagram.Items.Clear();
                                    ListItem itemEmpty = new ListItem();
                                    this.DocumentDdlStateDiagram.Items.Add(itemEmpty);

                                    DocsPaWR.Stato[] statiDg = DiagrammiManager.getStatiPerRicerca(idDiagramma, "F");
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

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try
            {
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();

                switch (addressBookCallFrom)
                {
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
                    case "F_X_X_S_4":
                        if (atList != null && atList.Count > 0)
                        {
                            NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                            Corrispondente tempCorrSingle;
                            if (!corrInSess.isRubricaComune)
                                tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                            else
                                tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                            this.txtCodiceCollocazione.Text = tempCorrSingle.codiceRubrica;
                            this.txtDescrizioneCollocazione.Text = tempCorrSingle.descrizione;
                            this.idCollocationAddr.Value = tempCorrSingle.systemId;
                            this.upPnlCollocationAddr.Update();
                        }
                        break;

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

        protected void DdlRegistries_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ListItem itemSelect = (sender as DropDownList).SelectedItem;
                Registro RegSel = (from reg in this.Role.registri
                                   where reg.systemId.Equals(itemSelect.Value) &&
                                       reg.codRegistro.Equals(itemSelect.Text.Trim())
                                   select reg).FirstOrDefault();
                UIManager.RegistryManager.SetRegistryInSession(RegSel);
                this.Registry = RegSel;
                this.Project = null;
                this.IdProject.Value = string.Empty;
                this.TxtCodeProject.Text = string.Empty;
                this.TxtDescriptionProject.Text = string.Empty;
                this.UpPnlProject.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddlSavedSearches_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ddlSavedSearches.SelectedIndex == 0)
                {
                    this.SearchProjectEdit.Enabled = false;
                    this.SearchProjectRemove.Enabled = false;

                    if (GridManager.IsRoleEnabledToUseGrids())
                    {
                        GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Project);
                    }
                    return;
                }
                else
                {
                    this.SearchProjectRemoveFilters_Click(null, null);
                }

                try
                {
                    string gridTempId = string.Empty;

                    schedaRicerca.Seleziona(Int32.Parse(this.ddlSavedSearches.SelectedValue), out gridTempId);

                    if (!string.IsNullOrEmpty(gridTempId) && GridManager.IsRoleEnabledToUseGrids())
                    {
                        schedaRicerca.gridId = gridTempId;
                        Grid tempGrid = GridManager.GetGridFromSearchId(schedaRicerca.gridId, GridTypeEnumeration.Project);
                        if (tempGrid != null)
                        {
                            GridManager.SelectedGrid = tempGrid;
                        }
                    }

                    try
                    {
                        if (this.ddlSavedSearches.SelectedIndex > 0)
                        {
                            Session.Add("itemUsedSearch", this.ddlSavedSearches.SelectedIndex.ToString());
                        }

                        this.BindFilterValues(schedaRicerca, true);
                        ProjectManager.setFiltroRicFasc(this, schedaRicerca.FiltriRicerca);

                        this.SearchProjectRemove.Enabled = true;
                        this.SearchProjectEdit.Enabled = true;
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

                        this.SearchProjectSearch_Click(null, null);
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

        protected void TxtCodeProject_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.TxtCodeProject.Text))
                {

                    string codClassificazione = this.TxtCodeProject.Text.ToString();

                    this.cercaClassificazioneDaCodice(codClassificazione);
                }
                else
                {
                    this.TxtCodeProject.Text = string.Empty;
                    this.TxtDescriptionProject.Text = string.Empty;
                    this.IdProject.Value = string.Empty;
                    this.Project = null;
                    this.Classification = null;
                }

                this.UpPnlProject.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private bool cercaClassificazioneDaCodice(string codClassificazione)
        {
            bool res = false;
            DocsPaWR.Fascicolo[] listaFasc;
            if (!string.IsNullOrEmpty(codClassificazione))
            {
                listaFasc = this.getFascicolo(this.Registry, codClassificazione);

                DocsPaWR.FascicolazioneClassificazione[] FascClass = ProjectManager.fascicolazioneGetTitolario2(this, codClassificazione, false, getIdTitolario(codClassificazione, listaFasc));
                if (FascClass != null && FascClass.Length != 0)
                {
                    this.Classification = FascClass[0];
                    this.TxtCodeProject.Text = this.Classification.codice;
                    this.TxtDescriptionProject.Text = this.Classification.descrizione;
                    this.IdProject.Value = this.Classification.systemID;
                }
                else
                {
                    this.Classification = null;
                    this.TxtCodeProject.Text = string.Empty;
                    this.TxtDescriptionProject.Text = string.Empty;
                    this.IdProject.Value = string.Empty;
                }
            }

            return res;
        }

        private DocsPaWR.Fascicolo[] getFascicolo(DocsPaWR.Registro registro, string codClassificazione)
        {
            DocsPaWR.Fascicolo[] listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codClassificazione, registro, "R");



            return listaFasc;
        }

        private string getIdTitolario(string codClassificazione, DocsPaWR.Fascicolo[] listaFasc)
        {
            if (!string.IsNullOrEmpty(codClassificazione))
            {
                //DocsPaWR.Fascicolo[] listaFasc = getFascicolo(UserManager.getRegistroSelezionato(this), codClassificazione);

                //In questo caso il metodo "GetFigliClassifica2" funzionerebbe male
                //per questo viene restituti l'idTitolario dell'unico fascicolo risolto
                if (ddlTitolario.SelectedItem != null && ddlTitolario.SelectedItem.Text == "Tutti i titolari")
                {
                    if (listaFasc != null && listaFasc.Length == 1)
                    {
                        DocsPaWR.Fascicolo fasc = (DocsPaWR.Fascicolo)listaFasc[0];
                        return fasc.idTitolario;
                    }
                    else
                    {
                        return UIManager.ClassificationSchemeManager.getTitolarioAttivo(UserManager.GetInfoUser().idAmministrazione).ID;
                    }

                }
            }

            //In tutti gli altri casi è sufficiente restituire il value degli item della
            //ddl_Titolario in quanto formati secondo le specifiche di uno o piu' titolari
            return ddlTitolario.SelectedValue;
        }

        protected void ddl_dtaOpen_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dtaOpen.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.dtaOpen_TxtFrom.ReadOnly = false;
                        this.dtaOpen_TxtTo.Visible = false;
                        this.lbl_dtaOpenTo.Visible = false;
                        this.lbl_dtaOpenFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.dtaOpen_TxtFrom.ReadOnly = false;
                        this.dtaOpen_TxtTo.ReadOnly = false;
                        this.lbl_dtaOpenTo.Visible = true;
                        this.lbl_dtaOpenFrom.Visible = true;
                        this.dtaOpen_TxtTo.Visible = true;
                        this.lbl_dtaOpenFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaOpenTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 2: //Oggi
                        this.lbl_dtaOpenTo.Visible = false;
                        this.dtaOpen_TxtTo.Visible = false;
                        this.dtaOpen_TxtFrom.ReadOnly = true;
                        this.dtaOpen_TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                        this.lbl_dtaOpenFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaOpenTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 3: //Settimana corrente
                        this.lbl_dtaOpenTo.Visible = true;
                        this.dtaOpen_TxtTo.Visible = true;
                        this.dtaOpen_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.dtaOpen_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.dtaOpen_TxtTo.ReadOnly = true;
                        this.dtaOpen_TxtFrom.ReadOnly = true;
                        this.lbl_dtaOpenFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaOpenTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 4: //Mese corrente
                        this.lbl_dtaOpenTo.Visible = true;
                        this.dtaOpen_TxtTo.Visible = true;
                        this.dtaOpen_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.dtaOpen_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.dtaOpen_TxtTo.ReadOnly = true;
                        this.dtaOpen_TxtFrom.ReadOnly = true;
                        this.lbl_dtaOpenFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaOpenTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                }

                this.upPnlIntervals.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

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

                this.upPnlIntervals.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dtaCreate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dtaCreate.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.dtaCreate_TxtFrom.ReadOnly = false;
                        this.dtaCreate_TxtTo.Visible = false;
                        this.lbl_dtaCreateTo.Visible = false;
                        this.lbl_dtaCreateFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.dtaCreate_TxtFrom.ReadOnly = false;
                        this.dtaCreate_TxtTo.ReadOnly = false;
                        this.lbl_dtaCreateTo.Visible = true;
                        this.lbl_dtaCreateFrom.Visible = true;
                        this.dtaCreate_TxtTo.Visible = true;
                        this.lbl_dtaCreateFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaCreateTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 2: //Oggi
                        this.lbl_dtaCreateTo.Visible = false;
                        this.dtaCreate_TxtTo.Visible = false;
                        this.dtaCreate_TxtFrom.ReadOnly = true;
                        this.dtaCreate_TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                        break;
                    case 3: //Settimana corrente
                        this.lbl_dtaCreateTo.Visible = true;
                        this.dtaCreate_TxtTo.Visible = true;
                        this.dtaCreate_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.dtaCreate_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.dtaCreate_TxtTo.ReadOnly = true;
                        this.dtaCreate_TxtFrom.ReadOnly = true;
                        this.lbl_dtaCreateFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaCreateTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 4: //Mese corrente
                        this.lbl_dtaCreateTo.Visible = true;
                        this.dtaCreate_TxtTo.Visible = true;
                        this.dtaCreate_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.dtaCreate_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.dtaCreate_TxtTo.ReadOnly = true;
                        this.dtaCreate_TxtFrom.ReadOnly = true;
                        this.lbl_dtaCreateFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaCreateTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 5: //Ieri
                        this.lbl_dtaCreateTo.Visible = false;
                        this.dtaCreate_TxtTo.Visible = false;
                        this.dtaCreate_TxtFrom.ReadOnly = true;
                        this.dtaCreate_TxtFrom.Text = NttDataWA.Utils.dateformat.GetYesterday();
                        this.lbl_dtaCreateFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        this.dtaCreate_TxtTo.Text = string.Empty;
                        break;
                    case 6: //Ultimi 7 giorni
                        this.lbl_dtaCreateTo.Visible = true;
                        this.dtaCreate_TxtTo.Visible = true;
                        this.dtaCreate_TxtFrom.Text = NttDataWA.Utils.dateformat.GetLastSevenDay();
                        this.dtaCreate_TxtTo.Text = NttDataWA.Utils.dateformat.toDay();
                        this.dtaCreate_TxtTo.ReadOnly = true;
                        this.dtaCreate_TxtFrom.ReadOnly = true;
                        this.lbl_dtaCreateFrom.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.lbl_dtaCreateTo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 7: //Ultimi 31 iorni
                        this.lbl_dtaCreateTo.Visible = true;
                        this.dtaCreate_TxtTo.Visible = true;
                        this.dtaCreate_TxtFrom.Text = NttDataWA.Utils.dateformat.GetLastThirtyOneDay();
                        this.dtaCreate_TxtTo.Text = NttDataWA.Utils.dateformat.toDay();
                        this.dtaCreate_TxtTo.ReadOnly = true;
                        this.dtaCreate_TxtFrom.ReadOnly = true;
                        this.lbl_dtaCreateFrom.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        this.lbl_dtaCreateTo.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }

                this.upPnlIntervals.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dtaExpire_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dtaExpire.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.dtaExpire_TxtFrom.ReadOnly = false;
                        this.dtaExpire_TxtTo.Visible = false;
                        this.lbl_dtaExpireTo.Visible = false;
                        this.lbl_dtaExpireFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.dtaExpire_TxtFrom.ReadOnly = false;
                        this.dtaExpire_TxtTo.ReadOnly = false;
                        this.lbl_dtaExpireTo.Visible = true;
                        this.lbl_dtaExpireFrom.Visible = true;
                        this.dtaExpire_TxtTo.Visible = true;
                        this.lbl_dtaExpireFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaExpireTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 2: //Oggi
                        this.lbl_dtaExpireTo.Visible = false;
                        this.dtaExpire_TxtTo.Visible = false;
                        this.dtaExpire_TxtFrom.ReadOnly = true;
                        this.dtaExpire_TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                        break;
                    case 3: //Settimana corrente
                        this.lbl_dtaExpireTo.Visible = true;
                        this.dtaExpire_TxtTo.Visible = true;
                        this.dtaExpire_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.dtaExpire_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.dtaExpire_TxtTo.ReadOnly = true;
                        this.dtaExpire_TxtFrom.ReadOnly = true;
                        this.lbl_dtaExpireFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaExpireTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 4: //Mese corrente
                        this.lbl_dtaExpireTo.Visible = true;
                        this.dtaExpire_TxtTo.Visible = true;
                        this.dtaExpire_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.dtaExpire_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.dtaExpire_TxtTo.ReadOnly = true;
                        this.dtaExpire_TxtFrom.ReadOnly = true;
                        this.lbl_dtaExpireFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaExpireTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                }

                this.upPnlIntervals.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dtaCollocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dtaCollocation.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.dtaCollocation_TxtFrom.ReadOnly = false;
                        this.dtaCollocation_TxtTo.Visible = false;
                        this.lbl_dtaCollocationTo.Visible = false;
                        this.lbl_dtaCollocationFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.dtaCollocation_TxtFrom.ReadOnly = false;
                        this.dtaCollocation_TxtTo.ReadOnly = false;
                        this.lbl_dtaCollocationTo.Visible = true;
                        this.lbl_dtaCollocationFrom.Visible = true;
                        this.dtaCollocation_TxtTo.Visible = true;
                        this.lbl_dtaCollocationFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaCollocationTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 2: //Oggi
                        this.lbl_dtaCollocationTo.Visible = false;
                        this.dtaCollocation_TxtTo.Visible = false;
                        this.dtaCollocation_TxtFrom.ReadOnly = true;
                        this.dtaCollocation_TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                        break;
                    case 3: //Settimana corrente
                        this.lbl_dtaCollocationTo.Visible = true;
                        this.dtaCollocation_TxtTo.Visible = true;
                        this.dtaCollocation_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.dtaCollocation_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.dtaCollocation_TxtTo.ReadOnly = true;
                        this.dtaCollocation_TxtFrom.ReadOnly = true;
                        this.lbl_dtaCollocationFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaCollocationTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 4: //Mese corrente
                        this.lbl_dtaCollocationTo.Visible = true;
                        this.dtaCollocation_TxtTo.Visible = true;
                        this.dtaCollocation_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.dtaCollocation_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.dtaCollocation_TxtTo.ReadOnly = true;
                        this.dtaCollocation_TxtFrom.ReadOnly = true;
                        this.lbl_dtaCollocationFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaCollocationTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                }

                this.upPnlCollocation.Update();
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

        protected void ImgCollocazioneAddressBook_Click(object sender, EventArgs e)
        {
            try
            {
                this.CallType = RubricaCallType.CALLTYPE_GESTFASC_LOCFISICA;
                HttpContext.Current.Session["AddressBook.from"] = "F_X_X_S_4";
                HttpContext.Current.Session["AddressBook.EnableOnly"] = "U";
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
                        case "txtCodiceCollocazione":
                            this.txtCodiceCollocazione.Text = string.Empty;
                            this.txtDescrizioneCollocazione.Text = string.Empty;
                            this.idCollocationAddr.Value = string.Empty;
                            this.upPnlCollocationAddr.Update();
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

        protected void chkConservation_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox check = (CheckBox)sender;
                if (check.Checked)
                {
                    if (check.ID == "chkConservation")
                    {
                        this.chkConservationNo.Checked = false;
                    }
                    else
                    {
                        this.chkConservation.Checked = false;
                    }

                    this.UpPnlConservation.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void rbViewAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                RadioButton radio = (RadioButton)sender;
                if (radio.ID == "rbViewAllYes")
                {
                    this.rbViewAllNo.Checked = false;
                }
                else
                {
                    this.rbViewAllYes.Checked = false;
                }

                this.UpPnlViewAll.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchProjectSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ricercaFascicoli())
                {
                    // Impostazione del filtro utilizzato
                    schedaRicerca.FiltriRicerca = this.SearchFilters;
                    schedaRicerca.ProprietaNuovaRicerca = new SearchManager.NuovaRicerca();
                    Session["idRicercaSalvata"] = null;
                    Session["tipoRicercaSalvata"] = "F";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupSaveSearch();", "ajaxModalPopupSaveSearch();", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchProjectEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ricercaFascicoli())
                {
                    // Impostazione del filtro utilizzato
                    schedaRicerca.FiltriRicerca = this.SearchFilters;
                    schedaRicerca.ProprietaNuovaRicerca = new SearchManager.NuovaRicerca();
                    if (this.ddlSavedSearches.SelectedIndex > 0)
                    {
                        string idRicercaSalvata = this.ddlSavedSearches.SelectedItem.Value.ToString();
                        Session["idRicercaSalvata"] = idRicercaSalvata;
                        Session["tipoRicercaSalvata"] = "F";
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

        protected void SearchDocumentDdlMassiveOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ListCheck != null && this.ListCheck.Count > 0)
            {
                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_ADL_PRJ")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveAddAdlUser", "ajaxModalPopupMassiveAddAdlUser();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "REMOVE_MASSIVE_ADL_PRJ")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveRemoveAdlUser", "ajaxModalPopupMassiveRemoveAdlUser();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_ADLR_PRJ")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveAddAdlRole", "ajaxModalPopupMassiveAddAdlRole();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "REMOVE_MASSIVE_ADLR_PRJ")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveRemoveAdlRole", "ajaxModalPopupMassiveRemoveAdlRole();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "DO_CONS")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveConservation", "ajaxModalPopupMassiveConservation();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVEXPORTPRJ")
                {
                    ProjectManager.setFiltroRicFasc(this, this.SearchFilters);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ExportDati", "ajaxModalPopupExportDati();", true);
                }

                if (this.SearchDocumentDdlMassiveOperation.SelectedValue == "MASSIVE_TRANSMISSION")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveTransmission", "ajaxModalPopupMassiveTransmission();", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorMassiveOperationNoItemSelected', 'warning', '');", true);
            }

            this.SearchDocumentDdlMassiveOperation.SelectedIndex = -1;
            this.UpnlAzioniMassive.Update();
        }

        protected void SearchProjectNewProject_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Project/Project.aspx?t=p");
        }

        #endregion

        #region methods

        private void setValuePopUp()
        {
            if (!string.IsNullOrEmpty(this.OpenTitolario.ReturnValue))
            {
                if (this.ReturnValue.Split('#').Length > 1)
                {
                    this.TxtCodeProject.Text = this.ReturnValue.Split('#').First();
                    this.TxtDescriptionProject.Text = this.ReturnValue.Split('#').Last();
                    this.UpPnlProject.Update();
                    this.TxtCodeProject_OnTextChanged(new object(), new EventArgs());
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenTitolario','');", true);
            }

            if (!string.IsNullOrEmpty(this.HiddenRemoveUsedSearch.Value))
            {
                try
                {
                    schedaRicerca.Cancella(Int32.Parse(this.ddlSavedSearches.SelectedValue));
                    Session.Remove("itemUsedSearch");
                    this.ddlSavedSearches.SelectedIndex = 0;
                    this.SearchProjectEdit.Enabled = false;
                    this.SearchProjectRemove.Enabled = false;
                    this.PopulateDDLSavedSearches();
                    this.UpPnlSavedSearches.Update();

                    ProjectManager.removeFiltroRicFasc(this);
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
                this.SearchProjectRemove.Enabled = true;
                this.SearchProjectEdit.Enabled = true;
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
                // Se ci sono risultati, vengono visualizzati
                if (this.Result != null && this.Result.Length > 0)
                {
                    this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage);

                }
                else
                {
                    this.ShowGrid(GridManager.SelectedGrid, null, 0, 0);
                }
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
                    this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage);

                }
                else
                {
                    this.ShowGrid(GridManager.SelectedGrid, null, 0, 0);
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
                // Se ci sono risultati, vengono visualizzati
                if (this.Result != null && this.Result.Length > 0)
                {
                    this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage);

                }
                else
                {
                    this.ShowGrid(GridManager.SelectedGrid, null, 0, 0);
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('GridPersonalizationPreferred','');", true);
            }

            if (!string.IsNullOrEmpty(this.MassiveAddAdlUser.ReturnValue))
            {
                if (this.MassiveAddAdlUser.ReturnValue == "true")
                {
                    this.Result = null;
                    this.SelectedRow = string.Empty;
                    this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid);
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
                    this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid);
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
                    this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid);
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
                    this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid);
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
                    this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid);
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
                    this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid);
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
                    this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid);
                    this.BuildGridNavigator();
                    this.UpnlNumerodocumenti.Update();
                    this.UpnlGrid.Update();
                    this.upPnlGridIndexes.Update();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveTransmission','');", true);
            }
        }

        /// <summary>
        /// Funzione per la visualizzazione dei risutati della ricerca
        /// </summary>
        /// <param name="result">I risultati della ricerca</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        private void ShowGrid(Grid selectedGrid, SearchObject[] result, int recordNumber, int selectedPage)
        {


            bool visibile = false;
            Templates templates = Session["templateRicerca"] as Templates;
            gridViewResult = this.HeaderGridView(selectedGrid,
              templates,
              this.ShowGridPersonalization, gridViewResult);

            DataTable dt = this.InitializeDataSet(selectedGrid,
                         Session["templateRicerca"] as Templates,
                         this.ShowGridPersonalization);


            if (result != null && result.Length > 0)
            {
                dt = this.FillDataSet(dt, result, selectedGrid, templates, this.ShowGridPersonalization);
                visibile = true;
            }

            // adding blank row eachone
            if (dt.Rows.Count == 1 && string.IsNullOrEmpty(dt.Rows[0]["IdProject"].ToString())) dt.Rows.RemoveAt(0);

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
            //projectImgSaveGrid.Enabled = false;
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

                this.EnableDisableSave();
            }

            this.projectLblNumeroDocumenti.Text = this.GetLabel("projectLblNumeroFascicoli");
            this.projectLblNumeroDocumenti.Text = this.projectLblNumeroDocumenti.Text.Replace("{0}", gridType);
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
        public DataTable InitializeDataSet(Grid selectedGrid, Templates templateTemp, bool showGridPersonalization)
        {
            try
            {
                // Il dataset da restituire
                DataSet toReturn;

                // La tabella da aggiungere al dataset
                DataTable dataTable;

                // La colonna da aggiungere alla tabella
                DataColumn dataColumn;

                // Inizializzazione del dataset
                toReturn = new DataSet();
                dataTable = new DataTable();
                toReturn.Tables.Add(dataTable);

                List<Field> fields = selectedGrid.Fields.Where(e => e.Visible).OrderBy(f => f.Position).ToList();

                dataTable.Columns.Add("IdProject", typeof(String));
                dataTable.Columns.Add("IsInStorageArea", typeof(Boolean));
                dataTable.Columns.Add("IsInWorkingArea", typeof(Boolean));
                dataTable.Columns.Add("IsInWorkingAreaRole", typeof(Boolean));

                // Creazione delle colonne
                foreach (Field field in fields)
                {
                    dataColumn = new DataColumn();
                    dataColumn.DataType = typeof(string);
                    dataColumn.ColumnName = field.FieldId;
                    dataTable.Columns.Add(dataColumn);
                }

                OggettoCustom customObjectTemp = new OggettoCustom();

                if (templateTemp != null && !showGridPersonalization)
                {
                    customObjectTemp = templateTemp.ELENCO_OGGETTI.Where(
                         e => e.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && e.DA_VISUALIZZARE_RICERCA == "1").FirstOrDefault();
                    if (customObjectTemp != null)
                    {
                        dataTable.Columns.Add("CONTATORE", typeof(String));
                    }
                }


                DataRow drow = dataTable.NewRow();
                dataTable.Rows.Add(drow);
                return dataTable;
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
            Templates templates, bool showGridPersonalization)
        {
            //try
            //{
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
                    d.OracleDbColumnName = "to_number(getContatoreFascContatore (a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "'))";
                    d.SqlServerDbColumnName = "@dbUser@.getContatoreFascContatore(a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "')";
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
            // Per ogni risultato...
            foreach (SearchObject prj in result)
            {
                // ...viene inizializzata una nuova riga
                dataRow = dataTable.NewRow();

                if (prj.SearchObjectID != null)
                {
                    // ...impostazione dell'id project
                    dataRow["IdProject"] = prj.SearchObjectID;

                    foreach (Field field in visibleFields)
                    {
                        switch (field.FieldId)
                        {
                            //APERTURA
                            case "P5":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //CARTACEO
                            case "P11":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                                {
                                    value = "Si";
                                }
                                else
                                {
                                    value = "No";
                                }
                                break;
                            //CHIUSURA
                            case "P6":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //CODICE
                            case "P3":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                temp = new StringBuilder("<span style=\"color:Black;font-weight: bold;\">");
                                // viene colorato in nero il link
                                temp.Append(value);
                                temp.Append("</span>");

                                value = temp.ToString();
                                break;
                            //CODICE CLASSIFICA
                            case "P2":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //AOO
                            case "P7":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //DESCRIZIONE
                            case "P4":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            // IN ARCHIVIO
                            case "P12":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                                {
                                    value = "Si";
                                }
                                else
                                {
                                    value = "No";
                                }
                                break;
                            //IN CONSERVAZIONE
                            case "P13":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                                {
                                    value = "Si";
                                }
                                else
                                {
                                    value = "No";
                                }
                                break;
                            //NOTE
                            case "P8":
                                string valoreChiave;

                                valoreChiave = Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_IS_PRESENT_NOTE.ToString());

                                if (!string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("1"))
                                {
                                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ESISTE_NOTA")).FirstOrDefault().SearchObjectFieldValue;
                                }
                                else
                                {
                                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                }
                                break;
                            // NUMERO
                            case "P14":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //NUMERO MESI IN CONSERVAZIONE
                            case "P15":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            // PRIVATO
                            case "P9":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                                {
                                    value = "Si";
                                }
                                else
                                {
                                    value = "No";
                                }
                                break;
                            // STATO
                            case "P16":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            // TIPO
                            case "P1":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            // TIPOLOGIA
                            case "U1":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //TITOLARIO
                            case "P10":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //Nome e cognome autore
                            case "P17":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //desc ruolo autore
                            case "P18":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //desc uo autore
                            case "P19":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //Data creazione
                            case "P20":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //Collocazione fisica
                            case "P22":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //CONTATORE
                            case "CONTATORE":
                                try
                                {
                                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                }
                                catch (Exception e)
                                {
                                    value = string.Empty;
                                }
                                break;
                            case "P23":
                                // Atipicità
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            case "COD_EXT_APP":
                                // CODICE ATTIVITA ESTERNA
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //DATE INSERT IN ADL
                            case "DTA_ADL":
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                break;
                            //OGGETTI CUSTOM
                            default:
                                try
                                {
                                    if (!string.IsNullOrEmpty(prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue))
                                    {
                                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
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
                        dataRow[field.FieldId] = value;

                    }

                    string inConservazione = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P13")).FirstOrDefault().SearchObjectFieldValue;
                    string inAdl = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_ADL")).FirstOrDefault().SearchObjectFieldValue;
                    string inAdlRole = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_ADLROLE")).FirstOrDefault().SearchObjectFieldValue;

                    // Impostazione dei campi IsInStorageArea, IsInWorkingArea e NavigateUrl
                    dataRow["IsInStorageArea"] = !String.IsNullOrEmpty(inConservazione) && inConservazione != "0" ? true : false;
                    dataRow["IsInWorkingArea"] = !String.IsNullOrEmpty(inAdl) && inAdl != "0" ? true : false;
                    dataRow["IsInWorkingAreaRole"] = !String.IsNullOrEmpty(inAdlRole) && inAdlRole != "0" ? true : false;
                    //dataRow["NavigateUrl"] = this.GetUrlForProjectDetails(UserManager.getInfoUtente(), prj.SearchObjectID);
                }

                // ...aggiunta della riga alla collezione delle righe
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return null;
            //}
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
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
                        d.OracleDbColumnName = "to_number(getContatoreFascContatore (a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "'))";
                        d.SqlServerDbColumnName = "@dbUser@.getContatoreFascContatore(a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "')";
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
                    if (field.OriginalLabel.ToUpper().Equals("CODICE"))
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
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IdProject", "IdProject"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsInStorageArea", "IsInStorageArea"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsInWorkingArea", "IsInWorkingArea"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsInWorkingAreaRole", "IsInWorkingAreaRole"));



                return grid;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
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

        protected void PopulateProfiledDocument()
        {
            this.PnlTypeDocument.Controls.Clear();
            this.inserisciComponenti(false);
        }

        private void inserisciComponenti(bool readOnly)
        {
            ArrayList dirittiCampiRuolo = ProfilerProjectManager.getDirittiCampiTipologiaFasc(UIManager.RoleManager.GetRoleInSession().idGruppo, this.Template.SYSTEM_ID.ToString());

            for (int i = 0, index = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i];

                ProfilerProjectManager.addNoRightsCustomObject(dirittiCampiRuolo, oggettoCustom);

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
                }
            }
        }

        protected bool GetGridPersonalization()
        {
            return this.ShowGridPersonalization;
        }

        public bool ricercaFascicoli()
        {

            //array contenitore degli array filtro di ricerca

            FiltroRicerca fV1 = new FiltroRicerca();
            FiltroRicerca[][] qV = new FiltroRicerca[1][];
            qV[0] = new FiltroRicerca[1];
            FiltroRicerca[] fVList = new FiltroRicerca[0];

            #region Filtro diTitolario

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_TITOLARIO.ToString();
            fV1.valore = this.ddlTitolario.SelectedValue;
            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

            #endregion
            #region Filtro su codice classificazione fascicolo
            if (this.TxtCodeProject.Text != string.Empty)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.CODICE_CLASSIFICA.ToString();
                fV1.valore = this.TxtCodeProject.Text;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region descrizione
            if (!string.IsNullOrEmpty(this.TxtDescrizione.Text))
            {
                fV1 = new FiltroRicerca();
                fV1.argomento = FiltriFascicolazione.TITOLO.ToString();
                fV1.valore = this.TxtDescrizione.Text.Trim();
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                fV1.nomeCampo = null;
            }
            #endregion
            #region numero fascicolo
            if (!this.TxtNumProject.Text.Equals(""))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.NUMERO_FASCICOLO.ToString();
                fV1.valore = this.TxtNumProject.Text.ToString();
                if (utils.isNumeric(fV1.valore))
                {
                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterNum', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterNum', 'error', '');}; parent.$('" + this.TxtNumProject.ClientID + "').focus();", true);
                    return false;
                }
            }
            #endregion
            #region anno fascicolo
            if (!string.IsNullOrEmpty(TxtYear.Text))
            {
                fV1 = new FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.ANNO_FASCICOLO.ToString();
                fV1.valore = TxtYear.Text;
                if (utils.isNumeric(fV1.valore))
                {
                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterYear', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterYear', 'error', '');}; parent.$('" + this.TxtYear.ClientID + "').focus();", true);
                    return false;
                }
            }
            #endregion
            #region  filtro sullo stato del fascicolo
            if (ddlStatus.SelectedIndex > 0)
            {
                fV1 = new FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.STATO.ToString();
                fV1.valore = ddlStatus.SelectedItem.Value;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                fV1 = null;
            }
            #endregion
            #region  filtro sulla tipologia del fascicolo
            if (this.ddl_typeProject.SelectedIndex > 0)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.TIPO_FASCICOLO.ToString();
                fV1.valore = this.ddl_typeProject.SelectedItem.Value;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region  filtro sulla data di apertura fascicolo
            if (!string.IsNullOrEmpty(dtaOpen_TxtFrom.Text) && dtaOpen_TxtFrom.Visible)
            {
                switch (this.ddl_dtaOpen.SelectedIndex)
                {
                    case 0:
                        {
                            if (!string.IsNullOrEmpty(dtaOpen_TxtFrom.Text))
                            {
                                if (!utils.isDate(dtaOpen_TxtFrom.Text))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                                    return false;
                                }
                                else
                                {
                                    fV1 = new FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_IL.ToString();
                                    fV1.valore = dtaOpen_TxtFrom.Text;
                                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                        }
                        break;
                    case 1:
                        {
                            if (!string.IsNullOrEmpty(dtaOpen_TxtFrom.Text) &&
                                !string.IsNullOrEmpty(dtaOpen_TxtTo.Text) &&
                                utils.verificaIntervalloDate(dtaOpen_TxtFrom.Text, dtaOpen_TxtTo.Text))
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDateOpenInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDateOpenInterval', 'warning', '');};", true);
                                return false;
                            }
                            if (!string.IsNullOrEmpty(dtaOpen_TxtFrom.Text))
                            {
                                if (!utils.isDate(dtaOpen_TxtFrom.Text))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                                    return false;
                                }
                                else
                                {
                                    fV1 = new FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_SUCCESSIVA_AL.ToString();
                                    fV1.valore = dtaOpen_TxtFrom.Text;
                                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                            if (!string.IsNullOrEmpty(dtaOpen_TxtTo.Text))
                            {
                                if (!utils.isDate(dtaOpen_TxtTo.Text))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                                    return false;
                                }
                                else
                                {
                                    fV1 = new FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_PRECEDENTE_IL.ToString();
                                    fV1.valore = dtaOpen_TxtTo.Text;
                                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                            break;
                        }
                    case 2:
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_IL.ToString();
                            fV1.valore = dtaOpen_TxtFrom.Text;
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                    case 3:
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_SC.ToString();
                            fV1.valore = "1";
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                    case 4:
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.APERTURA_MC.ToString();
                            fV1.valore = "1";
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                }
            }
            #endregion
            #region  filtro sulla data chiusura di un fascicolo
            if (!string.IsNullOrEmpty(dtaClose_TxtFrom.Text) && dtaClose_TxtFrom.Visible)
            {
                switch (this.ddl_dtaClose.SelectedIndex)
                {
                    case 0:
                        {
                            if (!string.IsNullOrEmpty(dtaClose_TxtFrom.Text))
                            {
                                if (!utils.isDate(dtaClose_TxtFrom.Text))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                                    return false;
                                }
                                else
                                {
                                    fV1 = new FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_IL.ToString();
                                    fV1.valore = dtaClose_TxtFrom.Text;
                                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                        }
                        break;
                    case 1:
                        {
                            if (!string.IsNullOrEmpty(dtaClose_TxtFrom.Text) &&
                                !string.IsNullOrEmpty(dtaClose_TxtTo.Text) &&
                                utils.verificaIntervalloDate(dtaClose_TxtFrom.Text, dtaClose_TxtTo.Text))
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDateCloseInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDateCloseInterval', 'warning', '');};", true);
                                return false;
                            }
                            if (!string.IsNullOrEmpty(dtaClose_TxtFrom.Text))
                            {
                                if (!utils.isDate(dtaClose_TxtFrom.Text))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                                    return false;
                                }
                                else
                                {
                                    fV1 = new FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_SUCCESSIVA_AL.ToString();
                                    fV1.valore = dtaClose_TxtFrom.Text;
                                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                            if (!string.IsNullOrEmpty(dtaClose_TxtTo.Text))
                            {
                                if (!utils.isDate(dtaClose_TxtTo.Text))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                                    return false;
                                }
                                else
                                {
                                    fV1 = new FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_PRECEDENTE_IL.ToString();
                                    fV1.valore = dtaClose_TxtTo.Text;
                                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                            break;
                        }
                    case 2:
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_IL.ToString();
                            fV1.valore = dtaClose_TxtFrom.Text;
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                    case 3:
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_SC.ToString();
                            fV1.valore = "1";
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                    case 4:
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.CHIUSURA_MC.ToString();
                            fV1.valore = "1";
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                }
            }
            #endregion
            #region  filtro sulla data creazione di un fascicolo
            if (!string.IsNullOrEmpty(dtaCreate_TxtFrom.Text) && dtaCreate_TxtFrom.Visible)
            {
                switch (this.ddl_dtaCreate.SelectedIndex)
                {
                    case 0:
                        {
                            if (!string.IsNullOrEmpty(dtaCreate_TxtFrom.Text))
                            {
                                if (!utils.isDate(dtaCreate_TxtFrom.Text))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                                    return false;
                                }
                                else
                                {
                                    fV1 = new FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_IL.ToString();
                                    fV1.valore = dtaCreate_TxtFrom.Text;
                                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                        }
                        break;
                    case 1:
                        {
                            if (!string.IsNullOrEmpty(dtaCreate_TxtFrom.Text) &&
                                !string.IsNullOrEmpty(dtaCreate_TxtTo.Text) &&
                                utils.verificaIntervalloDate(dtaCreate_TxtFrom.Text, dtaCreate_TxtTo.Text))
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDateCreateInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDateCreateInterval', 'warning', '');};", true);
                                return false;
                            }
                            if (!string.IsNullOrEmpty(dtaCreate_TxtFrom.Text))
                            {
                                if (!utils.isDate(dtaCreate_TxtFrom.Text))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                                    return false;
                                }
                                else
                                {
                                    fV1 = new FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_SUCCESSIVA_AL.ToString();
                                    fV1.valore = dtaCreate_TxtFrom.Text;
                                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                            if (!string.IsNullOrEmpty(dtaCreate_TxtTo.Text))
                            {
                                if (!utils.isDate(dtaCreate_TxtTo.Text))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                                    return false;
                                }
                                else
                                {
                                    fV1 = new FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_PRECEDENTE_IL.ToString();
                                    fV1.valore = dtaCreate_TxtTo.Text;
                                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                            break;
                        }
                    case 2:
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_IL.ToString();
                            fV1.valore = dtaCreate_TxtFrom.Text;
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                    case 3:
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_SC.ToString();
                            fV1.valore = "1";
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                    case 4:
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_MC.ToString();
                            fV1.valore = "1";
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                    case 5:
                        {
                            // siamo nel caso di Ieri
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_IERI.ToString();
                            fV1.valore = "1";
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                    case 6:
                        {
                            // siamo nel caso di Ultimi 7 giorni
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_ULTIMI_SETTE_GIORNI.ToString();
                            fV1.valore = "1";
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                    case 7:
                        {
                            // siamo nel caso di Ultimi 31 giorni
                            fV1 = new DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.CREAZIONE_ULTMI_TRENTUNO_GIORNI.ToString();
                            fV1.valore = "1";
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                }
            }
            #endregion
            #region  filtro sulla data scadenza di un fascicolo
            if (!string.IsNullOrEmpty(dtaExpire_TxtFrom.Text) && dtaExpire_TxtFrom.Visible)
            {
                switch (this.ddl_dtaExpire.SelectedIndex)
                {
                    case 0:
                        {
                            if (!string.IsNullOrEmpty(dtaExpire_TxtFrom.Text))
                            {
                                if (!utils.isDate(dtaExpire_TxtFrom.Text))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                                    return false;
                                }
                                else
                                {
                                    fV1 = new FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_IL.ToString();
                                    fV1.valore = dtaExpire_TxtFrom.Text;
                                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                        }
                        break;
                    case 1:
                        {
                            if (!string.IsNullOrEmpty(dtaExpire_TxtFrom.Text) &&
                                !string.IsNullOrEmpty(dtaExpire_TxtTo.Text) &&
                                utils.verificaIntervalloDate(dtaExpire_TxtFrom.Text, dtaExpire_TxtTo.Text))
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDateExpireInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDateExpireInterval', 'warning', '');};", true);
                                return false;
                            }
                            if (!string.IsNullOrEmpty(dtaExpire_TxtFrom.Text))
                            {
                                if (!utils.isDate(dtaExpire_TxtFrom.Text))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                                    return false;
                                }
                                else
                                {
                                    fV1 = new FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_SUCCESSIVA_AL.ToString();
                                    fV1.valore = dtaExpire_TxtFrom.Text;
                                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                            if (!string.IsNullOrEmpty(dtaExpire_TxtTo.Text))
                            {
                                if (!utils.isDate(dtaExpire_TxtTo.Text))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                                    return false;
                                }
                                else
                                {
                                    fV1 = new FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_PRECEDENTE_IL.ToString();
                                    fV1.valore = dtaExpire_TxtTo.Text;
                                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                            break;
                        }
                    case 2:
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_IL.ToString();
                            fV1.valore = dtaExpire_TxtFrom.Text;
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                    case 3:
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_SC.ToString();
                            fV1.valore = "1";
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                    case 4:
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.SCADENZA_MC.ToString();
                            fV1.valore = "1";
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                }
            }
            #endregion
            #region  filtro sulla data locazione fisica di un fascicolo
            if (!string.IsNullOrEmpty(dtaCollocation_TxtFrom.Text) && dtaCollocation_TxtFrom.Visible)
            {
                switch (this.ddl_dtaCollocation.SelectedIndex)
                {
                    case 0:
                        {
                            if (!string.IsNullOrEmpty(dtaCollocation_TxtFrom.Text))
                            {
                                if (!utils.isDate(dtaCollocation_TxtFrom.Text))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                                    return false;
                                }
                                else
                                {
                                    fV1 = new FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_IL.ToString();
                                    fV1.valore = dtaCollocation_TxtFrom.Text;
                                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                        }
                        break;
                    case 1:
                        {
                            if (!string.IsNullOrEmpty(dtaCollocation_TxtFrom.Text) &&
                                !string.IsNullOrEmpty(dtaCollocation_TxtTo.Text) &&
                                utils.verificaIntervalloDate(dtaCollocation_TxtFrom.Text, dtaCollocation_TxtTo.Text))
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDateCollocationInterval', 'warning', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDateCollocationInterval', 'warning', '');};", true);
                                return false;
                            }
                            if (!string.IsNullOrEmpty(dtaCollocation_TxtFrom.Text))
                            {
                                if (!utils.isDate(dtaCollocation_TxtFrom.Text))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                                    return false;
                                }
                                else
                                {
                                    fV1 = new FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_SUCCESSIVA_AL.ToString();
                                    fV1.valore = dtaCollocation_TxtFrom.Text;
                                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                            if (!string.IsNullOrEmpty(dtaCollocation_TxtTo.Text))
                            {
                                if (!utils.isDate(dtaCollocation_TxtTo.Text))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');} else {parent.ajaxDialogModal('ErrorSearchProjectFilterDate', 'error', '');};", true);
                                    return false;
                                }
                                else
                                {
                                    fV1 = new FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_PRECEDENTE_IL.ToString();
                                    fV1.valore = dtaCollocation_TxtTo.Text;
                                    fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                            break;
                        }
                    case 2:
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_IL.ToString();
                            fV1.valore = dtaCollocation_TxtFrom.Text;
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                    case 3:
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_SC.ToString();
                            fV1.valore = "1";
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                    case 4:
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriFascicolazione.DATA_LF_MC.ToString();
                            fV1.valore = "1";
                            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
                            fV1 = null;
                            break;
                        }
                }
            }
            #endregion
            #region descrizione Locazione Fisica
            if (this.idCollocationAddr != null && this.idCollocationAddr.Value != "")
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_UO_LF.ToString();
                string IdUoLF = this.idCollocationAddr.Value;
                fV1.valore = IdUoLF;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region FILTRO SUI SOTTOFASCICOLI
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriFascicolazione.SOTTOFASCICOLO.ToString();
            fV1.valore = this.txt_subProject.Text;
            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
            #endregion
            #region Note Fascicolo
            if (!this.TxtNoteProject.Text.Equals(""))
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.VAR_NOTE.ToString();
                fV1.valore = this.TxtNoteProject.Text.ToString() + "@-@" + this.rblFilterNote.SelectedValue + "@-@" + this.ddlNoteRF.SelectedValue;
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion
            #region Mostra tutti i fascicoli

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriFascicolazione.INCLUDI_FASCICOLI_FIGLI.ToString();
            if (this.rbViewAllYes.Checked)
            {
                fV1.valore = "S";
                this.AllClassification = true;
            }
            else
            {
                fV1.valore = "N";
                this.AllClassification = false;
            }
            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

            #endregion
            #region filtro CONSERVAZIONE
            if (this.chkConservation.Checked)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.CONSERVAZIONE.ToString();
                fV1.valore = "1";
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.chkConservationNo.Checked)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.CONSERVAZIONE.ToString();
                fV1.valore = "0";
                fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
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

            #region filtro RICERCA IN AREA LAVORO
            if (this.IsAdl)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.DOC_IN_FASC_ADL.ToString();
                if (this.RblTypeAdl.SelectedValue == "0")
                    fV1.valore = UserManager.GetUserInSession().idPeople.ToString() + "@" + RoleManager.GetRoleInSession().systemId.ToString();
                else
                    fV1.valore = "0@" + RoleManager.GetRoleInSession().systemId.ToString();
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion

            #region Filtro campi profilati
            if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
            {
                this.SaveTemplateProject();
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.PROFILAZIONE_DINAMICA.ToString();
                fV1.template = this.Template;
                fV1.valore = "Profilazione Dinamica";

                if (this.Template != null && this.Template.ELENCO_OGGETTI != null && this.Template.ELENCO_OGGETTI.Length > 0)
                {
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.TIPOLOGIA_FASCICOLO.ToString();
                fV1.valore = this.DocumentDdlTypeDocument.SelectedItem.Value;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion

            #region filtro DIAGRAMMI DI STATO
            if (this.DocumentDdlStateDiagram.Visible && this.DocumentDdlStateDiagram.SelectedIndex != 0)
            {
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.DIAGRAMMA_STATO_FASC.ToString();
                fV1.nomeCampo = this.ddlStateCondition.SelectedValue;
                fV1.valore = this.DocumentDdlStateDiagram.SelectedValue;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion filtro DIAGRAMMI DI STATO

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
        }

        protected void RblTypeNote_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.ddlNoteRF.Visible = false;

                //Se è presente il bottone di selezione esclusiva "RF" si verifica quanti sono gli
                //RF associati al ruolo dell'utente
                if (!string.IsNullOrEmpty(this.rblFilterNote.SelectedValue) && this.rblFilterNote.SelectedValue.Equals("F"))
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
                this.UpPnlNote.Update();
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

        private void SaveTemplateProject()
        {
            int result = 0;
            if (this.Template != null && this.Template.ELENCO_OGGETTI != null && this.Template.ELENCO_OGGETTI.Length > 0)
            {
                for (int i = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
                {
                    DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i];

                    if (this.controllaCampi(oggettoCustom, oggettoCustom.SYSTEM_ID.ToString()))
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
                    if (dataDa == null)
                    {
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }
                    else
                    {
                        if (dataDa.Text.Equals("") && (dataA == null || dataA.Text.Equals("")))
                        {
                            //SetFocus(dataDa.txt_Data);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }
                        else if (dataDa.Text.Equals("") && dataA.Text != "")
                        {
                            //SetFocus(dataDa.txt_Data);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }
                        else if (dataDa.Text != "" && (dataA == null || dataA.Text == ""))
                        {
                            //oggettoCustom.VALORE_DATABASE = dataDa.txt_Data.Text;
                            oggettoCustom.VALORE_DATABASE = dataDa.Text;
                        }
                        else if (dataDa.Text != "" && dataA.Text != "")
                        {
                            //oggettoCustom.VALORE_DATABASE = dataDa.txt_Data.Text + "@" + dataA.txt_Data.Text;
                            oggettoCustom.VALORE_DATABASE = dataDa.Text + "@" + dataA.Text;
                        }
                    }
                    break;
                case "Contatore":
                    CustomTextArea contatoreDa = (CustomTextArea)PnlTypeDocument.FindControl("da_" + idOggetto);
                    CustomTextArea contatoreA = (CustomTextArea)PnlTypeDocument.FindControl("a_" + idOggetto);
                    //Controllo la valorizzazione di campi ed eventualmente notifico gli errori
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
                    if (contatoreDa != null && contatoreA != null && contatoreDa.Text != "" && contatoreA.Text != "")
                        oggettoCustom.VALORE_DATABASE = contatoreDa.Text + "@" + contatoreA.Text;

                    if (contatoreDa != null && contatoreA != null && contatoreDa.Text != "" && contatoreA.Text == "")
                        oggettoCustom.VALORE_DATABASE = contatoreDa.Text;

                    switch (oggettoCustom.TIPO_CONTATORE)
                    {
                        case "A":
                            DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
                            break;
                        case "R":
                            DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
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

        private void inserisciContatoreSottocontatore(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, ArrayList dirittiCampiRuolo)
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
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Text = string.Empty;
                            it.Value = string.Empty;
                            ddl.Items.Add(it);
                            ddl.SelectedValue = string.Empty;
                        }
                        else
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;

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
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Value = string.Empty;
                            it.Text = string.Empty;
                            ddl.Items.Add(it);
                            ddl.SelectedValue = string.Empty;
                        }
                        else
                        {
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                        }

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

        private void inserisciLink(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, ArrayList dirittiCampiRuolo)
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

        private void inserisciCorrispondente(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
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

        private void inserisciData(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
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

            //Verifico i diritti del ruolo sul campo
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

        private void inserisciContatore(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, ArrayList dirittiCampiRuolo)
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

            ddl.Items.Add(new ListItem() { Text="", Value="" });

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
                    ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
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
        }

        public void impostaDirittiRuoloContatore(Object etichettaContatore, Object contatoreDa, Object contatoreA, Object etichettaContatoreDa, Object etichettaContatoreA, Object etichettaDDL, Object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, ArrayList dirittiCampiRuolo)
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

        private void inserisciSelezioneEsclusiva(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
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

        public void impostaDirittiRuoloSelezioneEsclusiva(Object etichetta, Object campo, Object button, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, ArrayList dirittiCampiRuolo)
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

        private void inserisciMenuATendina(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
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
               // }
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

        private void inserisciCampoDiTesto(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
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

        private void inserisciCasellaDiSelezione(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            DocsPaWR.StoricoProfilatiOldValue casellaSelOldObj = new StoricoProfilatiOldValue();
            Label etichettaCasellaSelezione = new Label();
            etichettaCasellaSelezione.EnableViewState = true;

            //if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            //{
            //    etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE + " *";
            //}
            //else
            //{
                etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE;
            //}

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

        public void impostaDirittiRuoloSulCampo(Object etichetta, Object campo, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, ArrayList dirittiCampiRuolo)
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
                            else if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM != "0")
                            {
                                ((UserControls.CorrespondentCustom)campo).CODICE_READ_ONLY = false;
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

        /// <summary>
        /// Questa funzione si occupa di ricercare i documenti e di visualizzare 
        /// i dati
        /// </summary>
        private void SearchProjectsAndDisplayResult(FiltroRicerca[][] searchFilters, int selectedPage, Grid selectedGrid)
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
            result = this.SearchProject(searchFilters, this.SelectedPage, out recordNumber, out outOfMaxRowSearchable);

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
                this.ShowResult(selectedGrid, this.Result, this.RecordCount, this.SelectedPage);
                this.SearchDocumentDdlMassiveOperation.Enabled = true;
            }
            else
            {
                this.ShowGrid(selectedGrid, null, 0, 0);
                this.BuildGridNavigator();
                this.SearchDocumentDdlMassiveOperation.Enabled = false;
            }
            this.UpnlAzioniMassive.Update();

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

            if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Project)
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Project);

            List<Field> visibleFields = GridManager.SelectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field)) && e.CustomObjectId != 0).ToList();
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
                    d.OracleDbColumnName = "to_number(getContatoreFascContatore (a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "'))";
                    d.SqlServerDbColumnName = "@dbUser@.getContatoreFascContatore(a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "')";
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

            // Caricamento dei fascicoli
            toReturn = ProjectManager.getListaFascicoliPagingCustom(this.Classification, this.Registry, this.SearchFilters[0], this.AllClassification, this.SelectedPage, out pageNumbers, out recordNumber, this.PageSize, true, out idProjects, null, this.ShowGridPersonalization, false, visibleArray, null, true);
            /* ABBATANGELI GIANLUIGI
             * outOfMaxRowSearchable viene impostato a true se getQueryInfoDocumentoPagingCustom
             * restituisce pageNumbers = -2 (raggiunto il numero massimo di righe possibili come risultato di ricerca)*/
            outOfMaxRowSearchable = (pageNumbers == -2);
            //if (this.Request.QueryString["newFasc"] != null && this.Request.QueryString["newFasc"].Equals("1"))
            //{
            //    recordNumber = 1;
            //}
            // Memorizzazione del numero di risultati restituiti dalla ricerca, del numero di pagina e dei risultati
            this.RecordCount = recordNumber;
            //this.PagesCount = pageNumbers;
            this.PagesCount = (int)Math.Round(((double)recordNumber / (double)this.PageSize) + 0.49);
            this.Result = toReturn;

            //appoggio il risultato in sessione.
            if (idProjects != null && idProjects.Length > 0)
            {
                this.IdProfileList = new string[idProjects.Length];
                this.CodeProfileList = new string[idProjects.Length];
                for (int i = 0; i < idProjects.Length; i++)
                {
                    this.IdProfileList[i] = idProjects[i].Id;
                    this.CodeProfileList[i] = idProjects[i].Codice;
                }
            }

            // Restituzione della lista di fascicoli da visualizzare
            return toReturn;

        }

        /// <summary>
        /// Funzione per la visualizzazione dei risutati della ricerca
        /// </summary>
        /// <param name="result">I risultati della ricerca</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        private void ShowResult(Grid selectedGrid, SearchObject[] result, int recordNumber, int selectedPage)
        {
            this.ShowGrid(selectedGrid, this.Result, this.RecordCount, selectedPage);
            this.grid_pageindex.Value = this.SelectedPage.ToString();
            this.gridViewResult.SelectedIndex = this.SelectedPage;
            this.BuildGridNavigator();
        }

        protected void BuildGridNavigator()
        {
            try
            {
                this.plcNavigator.Controls.Clear();

                int countPage = this.PagesCount;

                int val = this.RecordCount % this.PageSize;
                //if (val == 0)
                //{
                //    countPage = countPage;
                //}

                if (countPage >= 1)
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

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CombineRowsHover", "CombineRowsHover();", true);
        }

        private void InitializePage()
        {
            this.ShowGridPersonalization = UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION");
            this.ClearSessionProperties();
            this.InitializeLanguage();
            this.LoadKeys();
            this.PopulateDDLRegistry(this.Role);
            this.PopulateDDLSavedSearches();
            this.PopulateDDLTitolario();
            this.PopulateRapidSearch();
            this.LoadMassiveOperation();

            this.LoadTypeProjects();
            this.VisibiltyRoleFunctions();
            this.SetAjaxAddressBook();
            this.InitializeAjaxTitolario();

            this.ListCheck = new Dictionary<string, string>();

            if (this.IsAdl)
            {
                this.SaveSearch.Url = ResolveUrl("~/Popup/SaveSearch.aspx?IsAdl=true");
                this.ModifySearch.Url = ResolveUrl("~/Popup/SaveSearch.aspx?modify=true&IsAdl=true");
            }

            Session["templateRicerca"] = null;
        }

        public void PopulateDDLSavedSearches()
        {


            if (this.IsAdl)
            {
                schedaRicerca.ElencoRicercheADL("F", false, this.ddlSavedSearches, null);
            }
            else
            {
                schedaRicerca.ElencoRicerche("F", this.ddlSavedSearches);
            }

            if (Session["idRicercaSalvata"] != null)
            {
                Session["itemUsedSearch"] = this.ddlSavedSearches.Items.IndexOf(this.ddlSavedSearches.Items.FindByValue(Session["idRicercaSalvata"].ToString()));
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
                        this.ddlSavedSearches.SelectedIndex = Convert.ToInt32(this.Session["itemUsedSearch"]);
                        this.UpPnlSavedSearches.Update();
                    }
                    else if (this.Session["idRicercaSalvata"] != null)
                    {
                        this.ddlSavedSearches.Items.FindByValue(this.Session["idRicercaSalvata"].ToString()).Selected = true;
                        this.UpPnlSavedSearches.Update();
                    }

                    try
                    {
                        DocsPaWR.FiltroRicerca[] filterItems = schedaRicerca.FiltriRicerca[0];
                        //foreach (DocsPaWR.FiltroRicerca item in filterItems)
                        //{

                        //}

                        foreach (DocsPaWR.FiltroRicerca item in filterItems)
                        {

                            this.RestoreFiltersIdTitolario(item);

                            // Ripristino filtri su classificazione
                            this.RestoreFiltersClassificazione(item);

                            // Filtri data apertura fascicolo
                            this.RestoreFiltersDataApertura(item);

                            // Filtri data chiusura fascicolo
                            this.RestoreFiltersDataChiusura(item);

                            // Filtri data creazione fascicolo
                            this.RestoreFiltersDataCreazione(item);

                            if (item.argomento == DocsPaWR.FiltriFascicolazione.STATO.ToString())
                            {
                                this.ddlStatus.SelectedValue = item.valore;
                                this.UpPnlGeneric.Update();
                            }

                            if (item.argomento == DocsPaWR.FiltriFascicolazione.NUMERO_FASCICOLO.ToString())
                            {
                                this.TxtNumProject.Text = item.valore;
                                this.UpPnlGeneric.Update();
                            }

                            if (item.argomento == DocsPaWR.FiltriFascicolazione.TITOLO.ToString())
                            {
                                this.TxtDescrizione.Text = item.valore;
                                this.UpPnlDescription.Update();
                            }

                            if (item.argomento == DocsPaWR.FiltriFascicolazione.ANNO_FASCICOLO.ToString())
                            {
                                this.TxtYear.Text = item.valore;
                                this.UpPnlGeneric.Update();
                            }

                            // Ripristino valori per il box ricerca note
                            // Se l'argomento del filtro è note...
                            if (item.argomento == DocsPaWR.FiltriFascicolazione.VAR_NOTE.ToString())
                            {
                                // ...splitta la stringa in argomento...
                                string[] info = utils.splittaStringaRicercaNote(item.valore);

                                // ...la prima posizione dell'array contiene il testo da ricercare...
                                this.TxtNoteProject.Text = info[0];

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
                                this.rblFilterNote.SelectedValue = (info[1])[0].ToString();

                                if (this.ddlNoteRF.Visible && info != null && info.Length > 2)
                                {
                                    this.ddlNoteRF.SelectedValue = info[2].ToString();
                                }

                                this.UpPnlNote.Update();
                            }

                            //Filtri sottofascicolo
                            if (item.argomento == DocsPaWR.FiltriFascicolazione.SOTTOFASCICOLO.ToString())
                            {
                                this.txt_subProject.Text = item.valore;
                                this.UpPnlGeneric.Update();
                            }

                            //Filtri data scadenza
                            this.RestoreFiltersDataScadenza(item);

                            // Filtri data collocazione fascicolo
                            this.RestoreFiltersDataCollocazione(item);

                            // Ripristino filtri locazione fisica
                            this.RestoreFiltersLocazioneFisica(item);

                            //Ripristino filtro creatore
                            this.RestoreFiltersOwner(item);

                            this.RestoreFiltersAuthor(item);

                            if (this.CustomDocuments)
                            {
                                this.RestoreFiltersTypeProject(item);
                            }

                            if (this.EnableStateDiagram)
                            {
                                this.RestoreFiltersTypeDiagram(item);
                            }

                            // Ripristino filtro "Mostra tutti i fascicoli"
                            if (item.argomento == DocsPaWR.FiltriFascicolazione.INCLUDI_FASCICOLI_FIGLI.ToString())
                            {
                                if (item.valore == "S")
                                {
                                    this.rbViewAllYes.Checked = true;
                                    this.rbViewAllNo.Checked = false;
                                }
                                else
                                {
                                    this.rbViewAllNo.Checked = true;
                                    this.rbViewAllYes.Checked = false;
                                }

                                this.UpPnlViewAll.Update();
                            }


                            //impostaAbilitazioneNuovoFascNuovoTit();

                            if (item.argomento == DocsPaWR.FiltriDocumento.VISIBILITA_T_A.ToString())
                            {
                                this.rblVisibility.SelectedValue = item.valore;
                                this.UpPnlVisibility.Update();
                            }


                            // Ripristino filtro su Conservazione
                            if (item.argomento == DocsPaWR.FiltriFascicolazione.CONSERVAZIONE.ToString())
                            {
                                if (item.valore.Equals("1"))
                                {
                                    this.chkConservation.Checked = true;
                                    this.chkConservationNo.Checked = false;
                                }
                                if (item.valore.Equals("0"))
                                {
                                    this.chkConservation.Checked = false;
                                    this.chkConservationNo.Checked = true;
                                }

                                this.UpPnlConservation.Update();
                            }

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

        private void RestoreFiltersTypeDiagram(DocsPaWR.FiltroRicerca item)
        {
            // Ripristino filtro "Diagramma Stato"
            if (item.argomento == DocsPaWR.FiltriFascicolazione.DIAGRAMMA_STATO_FASC.ToString())
            {
                this.ddlStateCondition.Visible = true;
                this.PnlStateDiagram.Visible = true;
                this.ddlStateCondition.SelectedValue = item.nomeCampo;
                this.DocumentDdlStateDiagram.SelectedValue = item.valore;
                this.UpPnlTypeDocument.Update();
            }
        }

        private void RestoreFiltersTypeProject(DocsPaWR.FiltroRicerca item)
        {
            SearchCorrespondentIntExtWithDisabled = true;
            // Ripristino filtro "Tipologia fascicoli"
            if (item.argomento == DocsPaWR.FiltriFascicolazione.TIPOLOGIA_FASCICOLO.ToString())
            {
                this.DocumentDdlTypeDocument.SelectedValue = item.valore;
                //Verifico se esiste un diagramma di stato associato al tipo di documento
                //Modifica per la visualizzazione solo degli stati per cui esistono documenti in essi
                string idDiagramma = DiagrammiManager.getDiagrammaAssociatoFasc(this.DocumentDdlTypeDocument.SelectedValue).ToString();

                if (this.DocumentDdlTypeDocument.Items.FindByValue(item.valore) != null)
                {
                    if (!string.IsNullOrEmpty(idDiagramma) && !idDiagramma.Equals("0"))
                    {
                        this.PnlStateDiagram.Visible = true;

                        //Inizializzazione comboBox
                        this.DocumentDdlStateDiagram.Items.Clear();
                        ListItem itemEmpty = new ListItem();
                        this.DocumentDdlStateDiagram.Items.Add(itemEmpty);

                        DocsPaWR.Stato[] statiDg = DiagrammiManager.getStatiPerRicerca(idDiagramma, "F");
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
            }

            // Ripristino filtro "Tipologia fascicoli"
            if (item.argomento == DocsPaWR.FiltriFascicolazione.PROFILAZIONE_DINAMICA.ToString())
            {
                this.Template = item.template;
                if (!this.ShowGridPersonalization)
                {
                    Session["templateRicerca"] = this.Template;
                }
                this.UpPnlTypeDocument.Update();
            }
        }

        /// <summary>
        /// Ripristino filtro idTitolario
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>        
        private bool RestoreFiltersIdTitolario(DocsPaWR.FiltroRicerca item)
        {
            if (item.argomento == DocsPaWR.FiltriFascicolazione.ID_TITOLARIO.ToString())
            {
                try
                {
                    this.ddlTitolario.SelectedValue = item.valore;
                    this.UpPnlTitolario.Update();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return false;
        }

        private void RestoreFiltersOwner(DocsPaWR.FiltroRicerca item)
        {
            if (item.argomento == DocsPaWR.FiltriFascicolazione.ID_UO_CREATORE.ToString())
            {
                this.PnlCreator.Attributes.Remove("class");
                this.PnlCreator.Attributes.Add("class", "collapse shown");
                this.idCreatore.Value = item.valore;
                Corrispondente corr = UIManager.AddressBookManager.getCorrispondenteBySystemIDDisabled(item.valore);
                if (corr != null && !string.IsNullOrEmpty(corr.systemId))
                {
                    this.txtCodiceCreatore.Text = corr.codiceRubrica;
                    this.txtDescrizioneCreatore.Text = corr.descrizione;
                }
            }

            if (item.argomento == DocsPaWR.FiltriFascicolazione.CORR_TYPE_OWNER.ToString())
            {
                this.rblOwnerType.SelectedValue = item.valore;
            }


            if (item.argomento == "EXTEND_TO_HISTORICIZED_OWNER")
            {
                this.chkCreatoreExtendHistoricized.Checked = bool.Parse(item.valore);
            }
        }

        private void RestoreFiltersAuthor(DocsPaWR.FiltroRicerca item)
        {
            if (item.argomento == "ID_AUTHOR")
            {
                this.PnlCreator.Attributes.Remove("class");
                this.PnlCreator.Attributes.Add("class", "collapse shown");
                this.idProprietario.Value = item.valore;
                Corrispondente corr = UIManager.AddressBookManager.getCorrispondenteBySystemIDDisabled(item.valore);
                if (corr != null && !string.IsNullOrEmpty(corr.systemId))
                {
                    this.txtCodiceProprietario.Text = corr.codiceRubrica;
                    this.txtDescrizioneProprietario.Text = corr.descrizione;
                }
            }

            if (item.argomento == "CORR_TYPE_AUTHOR")
            {
                this.rblProprietarioType.SelectedValue = item.valore;
            }

        }

        /// <summary>
        /// Ripristino filtri su classificazione
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RestoreFiltersClassificazione(DocsPaWR.FiltroRicerca item)
        {
            if (item.argomento == DocsPaWR.FiltriFascicolazione.CODICE_CLASSIFICA.ToString())
            {
                if (!string.IsNullOrEmpty(item.valore))
                {
                    this.cercaClassificazioneDaCodice(item.valore);
                }
            }

            return false;
        } // TO DO

        /// <summary>
        /// Ripristino filtri data apertura
        /// </summary>
        /// <param name="filterItems"></param>
        /// <returns></returns>
        private bool RestoreFiltersDataApertura(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriFascicolazione.APERTURA_SUCCESSIVA_AL.ToString())
            {
                this.ddl_dtaOpen.SelectedValue = "1";
                this.ddl_dtaOpen_SelectedIndexChanged(null, null);
                this.dtaOpen_TxtFrom.Text = item.valore;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.APERTURA_PRECEDENTE_IL.ToString())
            {
                this.dtaOpen_TxtTo.Text = item.valore;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.APERTURA_IL.ToString())
            {
                this.ddl_dtaOpen.SelectedValue = "0";
                this.ddl_dtaOpen_SelectedIndexChanged(null, null);
                this.dtaOpen_TxtFrom.Text = item.valore;
                retValue = true;
            }
            #region APERTURA_SC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.APERTURA_SC.ToString() && item.valore == "1")
            {
                this.ddl_dtaOpen.SelectedValue = "3";
                this.ddl_dtaOpen_SelectedIndexChanged(null, null);
                this.dtaOpen_TxtFrom.Text = DocumentManager.getFirstDayOfWeek();
                this.dtaOpen_TxtTo.Text = DocumentManager.getLastDayOfWeek();
                retValue = true;
            }
            #endregion
            #region APERTURA_MC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.APERTURA_MC.ToString() && item.valore == "1")
            {
                this.ddl_dtaOpen.SelectedValue = "4";
                this.ddl_dtaOpen_SelectedIndexChanged(null, null);
                this.dtaOpen_TxtFrom.Text = DocumentManager.getFirstDayOfMonth();
                this.dtaOpen_TxtTo.Text = DocumentManager.getLastDayOfMonth();
                retValue = true;
            }
            #endregion
            #region APERTURA_TODAY
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.APERTURA_TODAY.ToString() && item.valore == "1")
            {
                this.ddl_dtaOpen.SelectedValue = "2";
                this.ddl_dtaOpen_SelectedIndexChanged(null, null);
                this.dtaOpen_TxtTo.Text = DocumentManager.toDay();
            }
            #endregion

            this.upPnlIntervals.Update();

            return retValue;
        }

        /// <summary>
        /// Ripristino filtri data chiusura
        /// </summary>
        /// <param name="filterItems"></param>
        /// <returns></returns>
        private bool RestoreFiltersDataChiusura(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriFascicolazione.CHIUSURA_SUCCESSIVA_AL.ToString())
            {
                this.ddl_dtaClose.SelectedValue = "1";
                this.ddl_dtaClose_SelectedIndexChanged(null, null);
                this.dtaClose_TxtFrom.Text = item.valore;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CHIUSURA_PRECEDENTE_IL.ToString())
            {
                this.dtaClose_TxtTo.Text = item.valore;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CHIUSURA_IL.ToString())
            {
                this.ddl_dtaClose.SelectedValue = "0";
                this.ddl_dtaClose_SelectedIndexChanged(null, null);
                this.dtaClose_TxtFrom.Text = item.valore;
                retValue = true;
            }
            #region CHIUSURA_SC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CHIUSURA_SC.ToString() && item.valore == "1")
            {
                this.ddl_dtaClose.SelectedValue = "3";
                this.ddl_dtaClose_SelectedIndexChanged(null, null);
                this.dtaClose_TxtFrom.Text = DocumentManager.getFirstDayOfWeek();
                this.dtaClose_TxtTo.Text = DocumentManager.getLastDayOfWeek();
                retValue = true;
            }
            #endregion
            #region CHIUSURA_MC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CHIUSURA_MC.ToString() && item.valore == "1")
            {
                this.ddl_dtaClose.SelectedValue = "4";
                this.ddl_dtaClose_SelectedIndexChanged(null, null);
                this.dtaClose_TxtFrom.Text = DocumentManager.getFirstDayOfMonth();
                this.dtaClose_TxtTo.Text = DocumentManager.getLastDayOfMonth();
                retValue = true;
            }
            #endregion
            #region CHIUSURA_TODAY
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CHIUSURA_TODAY.ToString() && item.valore == "1")
            {
                this.ddl_dtaClose.SelectedValue = "2";
                this.ddl_dtaClose_SelectedIndexChanged(null, null);
                this.dtaClose_TxtTo.Text = DocumentManager.toDay();
            }
            #endregion

            this.upPnlIntervals.Update();

            return retValue;
        }

        /// <summary>
        /// Ripristino filtri data creazione
        /// </summary>
        /// <param name="filterItems"></param>
        /// <returns></returns>
        private bool RestoreFiltersDataCreazione(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriFascicolazione.CREAZIONE_SUCCESSIVA_AL.ToString())
            {
                this.ddl_dtaCreate.SelectedValue = "1";
                this.ddl_dtaCreate_SelectedIndexChanged(null, null);
                this.dtaCreate_TxtFrom.Text = item.valore;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CREAZIONE_PRECEDENTE_IL.ToString())
            {
                this.dtaCreate_TxtTo.Text = item.valore;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CREAZIONE_IL.ToString())
            {
                this.ddl_dtaCreate.SelectedValue = "0";
                this.ddl_dtaCreate_SelectedIndexChanged(null, null);
                this.dtaCreate_TxtFrom.Text = item.valore;
                retValue = true;
            }
            #region CREAZIONE_SC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CREAZIONE_SC.ToString() && item.valore == "1")
            {
                this.ddl_dtaCreate.SelectedValue = "3";
                this.ddl_dtaCreate_SelectedIndexChanged(null, null);
                this.dtaCreate_TxtFrom.Text = DocumentManager.getFirstDayOfWeek();
                this.dtaCreate_TxtTo.Text = DocumentManager.getLastDayOfWeek();
                retValue = true;
            }
            #endregion
            #region CREAZIONE_MC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CREAZIONE_MC.ToString() && item.valore == "1")
            {
                this.ddl_dtaCreate.SelectedValue = "4";
                this.ddl_dtaCreate_SelectedIndexChanged(null, null);
                this.dtaCreate_TxtFrom.Text = DocumentManager.getFirstDayOfMonth();
                this.dtaCreate_TxtTo.Text = DocumentManager.getLastDayOfMonth();
                retValue = true;
            }
            #endregion
            #region CREAZIONE_TODAY
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CREAZIONE_TODAY.ToString() && item.valore == "1")
            {
                this.ddl_dtaCreate.SelectedValue = "2";
                this.ddl_dtaCreate_SelectedIndexChanged(null, null);
                this.dtaCreate_TxtTo.Text = DocumentManager.toDay();
            }
            #endregion
            #region CREAZIONE_IERI
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CREAZIONE_IERI.ToString() && item.valore == "1")
            {
                this.ddl_dtaCreate.SelectedIndex = 5;
                this.ddl_dtaCreate_SelectedIndexChanged(null, null);
                this.dtaCreate_TxtFrom.Text = NttDataWA.Utils.dateformat.GetYesterday();
                retValue = true;
            }
            #endregion
            #region CREAZIONE_ULTIMI_SETTE_GIORNI
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CREAZIONE_ULTIMI_SETTE_GIORNI.ToString() && item.valore == "1")
            {
                this.ddl_dtaCreate.SelectedIndex = 6;
                this.ddl_dtaCreate_SelectedIndexChanged(null, null);
                this.dtaCreate_TxtFrom.Text = NttDataWA.Utils.dateformat.GetLastSevenDay();
                this.dtaCreate_TxtTo.Text = DocumentManager.toDay();
                retValue = true;
            }
            #endregion
            #region CREAZIONE_ULTMI_TRENTUNO_GIORNI
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.CREAZIONE_ULTMI_TRENTUNO_GIORNI.ToString() && item.valore == "1")
            {
                this.ddl_dtaCreate.SelectedIndex = 7;
                this.ddl_dtaCreate_SelectedIndexChanged(null, null);
                this.dtaCreate_TxtFrom.Text = NttDataWA.Utils.dateformat.GetLastThirtyOneDay();
                this.dtaCreate_TxtTo.Text = DocumentManager.toDay();
                retValue = true;
            }
            #endregion
            this.upPnlIntervals.Update();

            return retValue;
        }

        /// <summary>
        /// Ripristino filtri data scadenza
        /// </summary>
        /// <param name="filterItems"></param>
        /// <returns></returns>
        private bool RestoreFiltersDataScadenza(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriFascicolazione.SCADENZA_SUCCESSIVA_AL.ToString())
            {
                this.ddl_dtaExpire.SelectedValue = "1";
                this.ddl_dtaExpire_SelectedIndexChanged(null, null);
                this.dtaExpire_TxtFrom.Text = item.valore;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.SCADENZA_PRECEDENTE_IL.ToString())
            {
                this.dtaExpire_TxtTo.Text = item.valore;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.SCADENZA_IL.ToString())
            {
                this.ddl_dtaExpire.SelectedValue = "0";
                this.ddl_dtaExpire_SelectedIndexChanged(null, null);
                this.dtaExpire_TxtFrom.Text = item.valore;
                retValue = true;
            }
            #region SCADENZA_SC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.SCADENZA_SC.ToString() && item.valore == "1")
            {
                this.ddl_dtaExpire.SelectedValue = "3";
                this.ddl_dtaExpire_SelectedIndexChanged(null, null);
                this.dtaExpire_TxtFrom.Text = DocumentManager.getFirstDayOfWeek();
                this.dtaExpire_TxtTo.Text = DocumentManager.getLastDayOfWeek();
                retValue = true;
            }
            #endregion
            #region SCADENZA_MC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.SCADENZA_MC.ToString() && item.valore == "1")
            {
                this.ddl_dtaExpire.SelectedValue = "4";
                this.ddl_dtaExpire_SelectedIndexChanged(null, null);
                this.dtaExpire_TxtFrom.Text = DocumentManager.getFirstDayOfMonth();
                this.dtaExpire_TxtTo.Text = DocumentManager.getLastDayOfMonth();
                retValue = true;
            }
            #endregion
            #region SCADENZA_TODAY
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.SCADENZA_TODAY.ToString() && item.valore == "1")
            {
                this.ddl_dtaExpire.SelectedValue = "2";
                this.ddl_dtaExpire_SelectedIndexChanged(null, null);
                this.dtaExpire_TxtTo.Text = DocumentManager.toDay();
            }
            #endregion

            this.upPnlIntervals.Update();

            return retValue;
        }

        /// <summary>
        /// Ripristino filtri data collocazione
        /// </summary>
        /// <param name="filterItems"></param>
        /// <returns></returns>
        private bool RestoreFiltersDataCollocazione(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriFascicolazione.DATA_LF_SUCCESSIVA_AL.ToString())
            {
                this.PnlCollocation.Attributes.Remove("class");
                this.PnlCollocation.Attributes.Add("class", "collapse shown");
                this.ddl_dtaCollocation.SelectedValue = "1";
                this.ddl_dtaCollocation_SelectedIndexChanged(null, null);
                this.dtaCollocation_TxtFrom.Text = item.valore;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.DATA_LF_PRECEDENTE_IL.ToString())
            {
                this.PnlCollocation.Attributes.Remove("class");
                this.PnlCollocation.Attributes.Add("class", "collapse shown");
                this.dtaCollocation_TxtFrom.Text = item.valore;
                retValue = true;
            }
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.DATA_LF_IL.ToString())
            {
                this.PnlCollocation.Attributes.Remove("class");
                this.PnlCollocation.Attributes.Add("class", "collapse shown");
                this.ddl_dtaCollocation.SelectedValue = "0";
                this.ddl_dtaCollocation_SelectedIndexChanged(null, null);
                this.dtaCollocation_TxtFrom.Text = item.valore;
                retValue = true;
            }
            #region DATA_LF_SC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.DATA_LF_SC.ToString() && item.valore == "1")
            {
                this.PnlCollocation.Attributes.Remove("class");
                this.PnlCollocation.Attributes.Add("class", "collapse shown");
                this.ddl_dtaCollocation.SelectedValue = "3";
                this.ddl_dtaCollocation_SelectedIndexChanged(null, null);
                this.dtaCollocation_TxtFrom.Text = DocumentManager.getFirstDayOfWeek();
                this.dtaCollocation_TxtTo.Text = DocumentManager.getLastDayOfWeek();
                retValue = true;
            }
            #endregion
            #region DATA_LF_MC
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.DATA_LF_MC.ToString() && item.valore == "1")
            {
                this.PnlCollocation.Attributes.Remove("class");
                this.PnlCollocation.Attributes.Add("class", "collapse shown");
                this.ddl_dtaCollocation.SelectedValue = "4";
                this.ddl_dtaCollocation_SelectedIndexChanged(null, null);
                this.dtaCollocation_TxtFrom.Text = DocumentManager.getFirstDayOfMonth();
                this.dtaCollocation_TxtTo.Text = DocumentManager.getLastDayOfMonth();
                retValue = true;
            }
            #endregion
            #region DATA_LF_TODAY
            else if (item.argomento == DocsPaWR.FiltriFascicolazione.DATA_LF_TODAY.ToString() && item.valore == "1")
            {
                this.PnlCollocation.Attributes.Remove("class");
                this.PnlCollocation.Attributes.Add("class", "collapse shown");
                this.ddl_dtaCollocation.SelectedValue = "2";
                this.ddl_dtaCollocation_SelectedIndexChanged(null, null);
                this.dtaCollocation_TxtTo.Text = DocumentManager.toDay();
            }
            #endregion

            this.upPnlCollocation.Update();

            return retValue;
        }

        protected void GridView_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "viewDetails")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                string language = UIManager.UserManager.GetUserLanguage();
                string idProject = GrigliaResult.Rows[rowIndex]["IdProject"].ToString();
                Fascicolo fascicolo = UIManager.ProjectManager.getFascicoloById(idProject);
                fascicolo.template = ProfilerProjectManager.getTemplateFascDettagli(fascicolo.systemID);
                InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();

                if (!string.IsNullOrEmpty(this.SelectedRow))
                {
                    if (rowIndex != Int32.Parse(this.SelectedRow))
                    {
                        this.SelectedRow = string.Empty;
                    }
                }


                //List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                //Navigation.NavigationObject obj = navigationList.Last();
                //navigationList.Remove(obj);
                //obj.IdObject = fascicolo.systemID;
                //obj.OriginalObjectId = fascicolo.systemID;
                //obj.NumPage = this.SelectedPage.ToString();
                //obj.DxTotalPageNumber = this.PagesCount.ToString();
                //obj.DxTotalNumberElement = this.RecordCount.ToString();
                //obj.NumPage = this.SelectedPage.ToString();
                //obj.ViewResult = true;
                //obj.SearchFilters = this.SearchFilters;
                //obj.Type = string.Empty;
                //obj.RegistryFilter = this.Registry;
                //obj.Classification = this.Classification;
                //obj.PageSize = this.PageSize.ToString();
                //obj.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString(), string.Empty);
                //obj.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString(), true);
                //obj.CodePage = Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString();
                //obj.Page = "SEARCHPROJECT.ASPX";
                //int indexElement = ((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1);
                //obj.DxPositionElement = indexElement.ToString();
                //navigationList.Add(obj);
                //Navigation.NavigationUtils.SetNavigationList(navigationList);
                //UIManager.ProjectManager.setProjectInSession(fascicolo);
                //Response.Redirect("~/Project/project.aspx");
                List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                Navigation.NavigationObject obj = new Navigation.NavigationObject();
                obj.IdObject = fascicolo.systemID;
                obj.OriginalObjectId = fascicolo.systemID;
                obj.SearchFilters = this.SearchFilters;
                obj.NumPage = this.SelectedPage.ToString();
                obj.DxTotalPageNumber = this.PagesCount.ToString();
                obj.DxTotalNumberElement = this.RecordCount.ToString();
                obj.ViewResult = true;
                obj.Type = string.Empty;
                obj.RegistryFilter = this.Registry;
                obj.Classification = this.Classification;
                obj.PageSize = this.PageSize.ToString();
                obj.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString(), string.Empty);
                obj.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString(), true, this.Page);
                obj.CodePage = Navigation.NavigationUtils.NamePage.SEARCH_PROJECTS.ToString();
                obj.Page = "SEARCHPROJECT.ASPX";

                int indexElement = (((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1))+1;
                obj.DxPositionElement = indexElement.ToString();
                navigationList.Add(obj);
                Navigation.NavigationUtils.SetNavigationList(navigationList);
                UIManager.ProjectManager.setProjectInSession(fascicolo);
                Response.Redirect("~/Project/project.aspx");

            }
        }

        /// <summary>
        /// Ripristino filtri locazione fisica
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool RestoreFiltersLocazioneFisica(DocsPaWR.FiltroRicerca item)
        {
            bool retValue = false;

            if (item.argomento == DocsPaWR.FiltriFascicolazione.ID_UO_LF.ToString())
            {
                this.PnlCollocation.Attributes.Remove("class");
                this.PnlCollocation.Attributes.Add("class", "collapse shown");
                // Reperimento dati corrispondente da systemID
                DocsPaWR.Corrispondente corr = UserManager.getCorrispondentBySystemID(item.valore);
                if (corr != null)
                {
                    this.idCollocationAddr.Value = item.valore;
                    this.txtCodiceCollocazione.Text = corr.codiceRubrica;
                    this.txtDescrizioneCollocazione.Text = corr.descrizione;
                    retValue = true;
                }
            }

            this.upPnlCollocation.Update();

            return retValue;
        }

        private void PopulateDDLTitolario()
        {
            this.ddlTitolario.Items.Clear();
            ArrayList listaTitolari = ClassificationSchemeManager.getTitolariUtilizzabili();

            string language = UIManager.UserManager.GetUserLanguage();

            //Esistono dei titolari chiusi
            if (listaTitolari.Count > 1)
            {
                //Creo le voci della ddl dei titolari
                string valueTutti = string.Empty;
                foreach (DocsPaWR.OrgTitolario titolario in listaTitolari)
                {
                    ListItem it = null;
                    switch (titolario.Stato)
                    {
                        case DocsPaWR.OrgStatiTitolarioEnum.Attivo:
                            string descrizione = Utils.Languages.GetLabelFromCode("ProjectLblTitolarioAttivo", language).Replace("@@", titolario.DescrizioneLite);
                            it = new ListItem(descrizione, titolario.ID);
                            this.ddlTitolario.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                        case DocsPaWR.OrgStatiTitolarioEnum.Chiuso:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            this.ddlTitolario.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                    }
                }
                //Imposto la voce tutti i titolari
                valueTutti = valueTutti.Substring(0, valueTutti.Length - 1);
                if (valueTutti != string.Empty)
                {
                    if (valueTutti.IndexOf(',') == -1)
                        valueTutti = valueTutti + "," + valueTutti;

                    ListItem it = new ListItem("Tutti i titolari", valueTutti);
                    this.ddlTitolario.Items.Insert(0, it);
                }

                //se la chiave di db è a 1, seleziono di default il titolario attivo
                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_CHECK_TITOLARIO_ATTIVO.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_CHECK_TITOLARIO_ATTIVO.ToString()).Equals("1"))
                {
                    int indexTitAtt = 0;
                    foreach (DocsPaWR.OrgTitolario titolario in listaTitolari)
                    {
                        if (titolario.Stato == DocsPaWR.OrgStatiTitolarioEnum.Attivo)
                        {
                            this.ddlTitolario.SelectedIndex = ++indexTitAtt;
                            break;
                        }
                        indexTitAtt++;
                    }
                }
            }
            else
            {
                DocsPaWR.OrgTitolario titolario = (DocsPaWR.OrgTitolario)listaTitolari[0];
                if (titolario.Stato != DocsPaWR.OrgStatiTitolarioEnum.InDefinizione)
                {
                    string descrizione = Utils.Languages.GetLabelFromCode("ProjectLblTitolarioAttivo", language).Replace("@@", titolario.DescrizioneLite);
                    ListItem it = new ListItem(descrizione, titolario.ID);
                    this.ddlTitolario.Items.Add(it);
                }
                this.ddlTitolario.Enabled = false;
                this.plcTitolario.Visible = false;
                this.UpPnlTitolario.Update();
            }
        }

        protected void SetAjaxAddressBook()
        {
            string dataUser = this.Role.systemId;
            if (this.Registry == null)
            {
                this.Registry = RegistryManager.GetRegistryInSession();
            }
            dataUser = dataUser + "-" + this.Registry.systemId;

            string callType = "CALLTYPE_OWNER_AUTHOR";

            this.RapidCreatore.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
            this.RapidProprietario.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;

            callType = "CALLTYPE_GESTFASC_LOCFISICA";
            this.RapidCollocazione.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
        }

        private void InitializeAjaxTitolario()
        {
            string dataUser = UIManager.RoleManager.GetRoleInSession().idGruppo;
            dataUser = dataUser + "-" + RegistryManager.GetRegistryInSession().systemId;
            if (UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione) != null)
            {
                this.RapidSenderDescriptionProject.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione).ID + "-" + UIManager.UserManager.GetUserInSession().idPeople + "-" + UIManager.UserManager.GetUserInSession().systemId;
            }
        }

        private void ClearSessionProperties()
        {
            Session.Remove("itemUsedSearch");
            Session.Remove("idRicercaSalvata");

            this.SelectedRow = string.Empty;
            this.Result = null;
            this.SearchFilters = null;
            this.Template = null;
            this.RecordCount = 0;
            this.PagesCount = 0;
            this.SelectedPage = 1;
            this.Registry = RoleManager.GetRoleInSession().registri[0];
            this.Classification = null;
            this.AllClassification = false;
            this.CellPosition = new Dictionary<string, int>();
            this.IdProfileList = null;
            this.CodeProfileList = null;
            this.CheckAll = false;
            this.ListCheck = null;

            //Inizializzazione della scheda di ricerca per la gestione delle ricerche salvate
            schedaRicerca = new SearchManager(KEY_SCHEDA_RICERCA, UserManager.GetUserInSession(), RoleManager.GetRoleInSession(), this);
            Session[SearchManager.SESSION_KEY] = schedaRicerca;

            UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);

            // Caricamento della griglia se non ce n'è una già selezionata
            if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Project)
            {
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Project);
            }

            this.SearchDocumentDdlMassiveOperation.Enabled = false;
            this.ShowGrid(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage);
        }

        private void PopulateRapidSearch()
        {

        }

        private void LoadMassiveOperation()
        {

            this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem("", ""));
            string title = string.Empty;
            string language = UIManager.UserManager.GetUserLanguage();


            if (UIManager.UserManager.IsAuthorizedFunctions("DO_TRA_TRASMETTI") && UIManager.UserManager.IsAuthorizedFunctions("MASSIVE_TRANSMISSION"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchProjectMassiveTransmissionTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_TRANSMISSION"));
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("FASC_ADD_ADL"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchProjectMassiveAddAdlUserTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_ADL_PRJ"));

                title = Utils.Languages.GetLabelFromCode("SearchProjectMassiveRemoveAdlUserTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "REMOVE_MASSIVE_ADL_PRJ"));
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchProjectMassiveAddAdlRoleTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_ADLR_PRJ"));

                title = Utils.Languages.GetLabelFromCode("SearchProjectMassiveRemoveAdlRoleTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "REMOVE_MASSIVE_ADLR_PRJ"));
            }

            title = Utils.Languages.GetLabelFromCode("SearchProjectExportDatiTitle", language);
            this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVEXPORTPRJ"));

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_CONS"))
            {
                title = Utils.Languages.GetLabelFromCode("SearchProjectMassiveConservationTitle", language);
                this.SearchDocumentDdlMassiveOperation.Items.Add(new ListItem(title, "DO_CONS"));
            }


            // riordina alfabeticamente
            List<ListItem> listCopy = new List<ListItem>();
            foreach (ListItem item in this.SearchDocumentDdlMassiveOperation.Items)
                listCopy.Add(item);
            this.SearchDocumentDdlMassiveOperation.Items.Clear();
            foreach (ListItem item in listCopy.OrderBy(item => item.Text))
                this.SearchDocumentDdlMassiveOperation.Items.Add(item);
        }

        private void LoadTypeProjects()
        {
            ArrayList listaTipiFasc = new ArrayList(ProfilerProjectManager.getTipoFascFromRuolo(UserManager.GetInfoUser().idAmministrazione, RoleManager.GetRoleInSession().idGruppo, "1"));
            ListItem item = new ListItem();
            item.Value = "";
            item.Text = "";
            if (this.DocumentDdlTypeDocument.Items.Count == 0)
            {
                this.DocumentDdlTypeDocument.Items.Add(item);
            }
            for (int i = 0; i < listaTipiFasc.Count; i++)
            {
                DocsPaWR.Templates templates = (DocsPaWR.Templates)listaTipiFasc[i];
                ListItem item_1 = new ListItem();
                item_1.Value = templates.SYSTEM_ID.ToString();
                item_1.Text = templates.DESCRIZIONE;

                //Christian - Ticket OC0000001490459 - Ricerca fascicoli: ripristino tipologia successivo a ordinamento tramite griglia.
                if (this.DocumentDdlTypeDocument.Items.FindByValue(templates.SYSTEM_ID.ToString()) == null)
                {
                    if (templates.IPER_FASC_DOC == "1")
                        this.DocumentDdlTypeDocument.Items.Insert(1, item_1);
                    else
                        this.DocumentDdlTypeDocument.Items.Add(item_1);
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

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.LitSearchProject.Text = Utils.Languages.GetLabelFromCode("SearchProjectsTitle", language);
            if (this.IsAdl)
            {
                this.LitSearchProject.Text = Utils.Languages.GetLabelFromCode("SearchProjectsTitleAdl", language);
                this.RblTypeAdl.Items[0].Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdlUser", language);
                this.RblTypeAdl.Items[1].Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdlRole", language);
            }
            else
            {
                this.LitSearchProject.Text = Utils.Languages.GetLabelFromCode("SearchProjectsTitle", language);
            }
            this.SearchProjectSearch.Text = Utils.Languages.GetLabelFromCode("SearchLabelButton", language);
            this.SearchProjectSave.Text = Utils.Languages.GetLabelFromCode("SearchLabelSearchButton", language);
            this.SearchProjectRemove.Text = Utils.Languages.GetLabelFromCode("SearchLabelRemoveButton", language);
            this.SearchProjectRemoveFilters.Text = Utils.Languages.GetLabelFromCode("SearchLabelRemoveFiltersButton", language);
            this.SearchDocumentDdlMassiveOperation.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("projectDdlAzioniMassive", language));
            this.projectImgEditGrid.ToolTip = Utils.Languages.GetLabelFromCode("projectImgEditGrid", language);
            this.projectImgPreferredGrids.ToolTip = Utils.Languages.GetLabelFromCode("projectImgPreferredGrids", language);
            this.projectImgSaveGrid.ToolTip = Utils.Languages.GetLabelFromCode("projectImgSaveGrid", language);
            this.projectImgEditGrid.AlternateText = Utils.Languages.GetLabelFromCode("projectImgEditGrid", language);
            this.projectImgPreferredGrids.AlternateText = Utils.Languages.GetLabelFromCode("projectImgPreferredGrids", language);
            this.projectImgSaveGrid.AlternateText = Utils.Languages.GetLabelFromCode("projectImgSaveGrid", language);
            this.ProjectLitObject.Text = Utils.Languages.GetLabelFromCode("projectLblDescrizione", language);
            this.SearchProjectLitNomeGriglia.Text = Utils.Languages.GetLabelFromCode("SearchProjectLitNomeGriglia", language);
            this.SearchDocumentLitTypology.Text = Utils.Languages.GetLabelFromCode("SearchProjectLitTypology", language);
            this.DocumentDdlTypeDocument.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("ProjectDdlTypeProject", language));
            this.ddlStateCondition.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));
            this.DocumentDdlStateDiagram.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DocumentDdlStateDiagram", language));
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
            this.SearchProjectEdit.Text = Utils.Languages.GetLabelFromCode("SearchProjectEdit", language);
            this.lit_dtaOpen.Text = Utils.Languages.GetLabelFromCode("SearchProjectDtaOpen", language);
            this.lit_dtaClose.Text = Utils.Languages.GetLabelFromCode("SearchProjectDtaClose", language);
            this.lit_dtaCreate.Text = Utils.Languages.GetLabelFromCode("SearchProjectDtaCreate", language);
            this.lit_dtaExpire.Text = Utils.Languages.GetLabelFromCode("SearchProjectDtaExpire", language);
            this.lit_dtaCollocation.Text = Utils.Languages.GetLabelFromCode("SearchProjectDtaCollocation", language);
            this.dtaOpen_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaOpen_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.dtaOpen_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.dtaOpen_opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.dtaOpen_opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            this.dtaClose_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaClose_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.dtaClose_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.dtaClose_opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.dtaClose_opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            this.dtaCreate_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaCreate_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.dtaCreate_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.dtaCreate_opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.dtaCreate_opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            this.dtaCreate_opt5.Text = Utils.Languages.GetLabelFromCode("ddl_data5", language);
            this.dtaCreate_opt6.Text = Utils.Languages.GetLabelFromCode("ddl_data6", language);
            this.dtaCreate_opt7.Text = Utils.Languages.GetLabelFromCode("ddl_data7", language);
            this.dtaExpire_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaExpire_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.dtaExpire_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.dtaExpire_opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.dtaExpire_opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            this.dtaCollocation_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaCollocation_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.dtaCollocation_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.dtaCollocation_opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.dtaCollocation_opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            this.litSavedSearches.Text = Utils.Languages.GetLabelFromCode("SearchProjectSavedSearches", language);
            this.litRegistry.Text = Utils.Languages.GetLabelFromCode("SearchProjectRegistry", language);
            this.litTitolario.Text = Utils.Languages.GetLabelFromCode("SearchProjectTitolario", language);
            this.litCodeProject.Text = Utils.Languages.GetLabelFromCode("SearchProjectCode", language);
            this.litStatus.Text = Utils.Languages.GetLabelFromCode("SearchProjectStatus", language);
            this.opt_statusA.Text = Utils.Languages.GetLabelFromCode("SearchProjectStatusOptA", language);
            this.opt_statusC.Text = Utils.Languages.GetLabelFromCode("SearchProjectStatusOptC", language);
            this.DdlRegistries.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language);
            this.ddlStatus.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language);
            this.ddl_typeProject.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language);
            this.litType.Text = Utils.Languages.GetLabelFromCode("SearchProjectType", language);
            this.opt_typeG.Text = Utils.Languages.GetLabelFromCode("SearchProjectTypeOptG", language);
            this.opt_typeP.Text = Utils.Languages.GetLabelFromCode("SearchProjectTypeOptP", language);
            this.litNum.Text = Utils.Languages.GetLabelFromCode("SearchProjectNum", language);
            this.litYear.Text = Utils.Languages.GetLabelFromCode("SearchProjectYear", language);
            this.litSubset.Text = Utils.Languages.GetLabelFromCode("SearchProjectSubset", language);
            this.litOwnerAuthor.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerAuthor", language);
            this.optUO.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptUO", language);
            this.optPropUO.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptUO", language);
            this.optRole.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptRole", language);
            this.optPropRole.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptRole", language);
            this.optUser.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptUser", language);
            this.optPropUser.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptUser", language);
            this.litCreator.Text = Utils.Languages.GetLabelFromCode("SearchProjectCreator", language);
            this.litOwner.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwner", language);
            this.litNotes.Text = Utils.Languages.GetLabelFromCode("SearchProjectNotes", language);
            this.optNoteAny.Text = Utils.Languages.GetLabelFromCode("SearchProjectNotesOptAny", language);
            this.optNoteAll.Text = Utils.Languages.GetLabelFromCode("SearchProjectNotesOptAll", language);
            this.optNoteRole.Text = Utils.Languages.GetLabelFromCode("SearchProjectNotesOptRole", language);
            this.optNoteRF.Text = Utils.Languages.GetLabelFromCode("SearchProjectNotesOptRF", language);
            this.optNotePersonal.Text = Utils.Languages.GetLabelFromCode("SearchProjectNotesOptPersonal", language);
            this.litIntervals.Text = Utils.Languages.GetLabelFromCode("SearchProjectIntervals", language);
            this.opt_StateConditionEquals.Text = Utils.Languages.GetLabelFromCode("SearchProjectStateConditionEquals", language);
            this.opt_StateConditionUnequals.Text = Utils.Languages.GetLabelFromCode("SearchProjectStateConditionUnequals", language);
            this.litCollocation.Text = Utils.Languages.GetLabelFromCode("SearchProjectCollocation", language);
            this.litCollocationAddr.Text = Utils.Languages.GetLabelFromCode("SearchProjectCollocationAddr", language);
            this.chkConservation.Text = Utils.Languages.GetLabelFromCode("SearchProjectConservationYes", language);
            this.chkConservationNo.Text = Utils.Languages.GetLabelFromCode("SearchProjectConservationNo", language);
            this.litViewAll.Text = Utils.Languages.GetLabelFromCode("SearchProjectViewAll", language);
            this.rbViewAllYes.Text = Utils.Languages.GetLabelFromCode("SearchProjectViewAllYes", language);
            this.rbViewAllNo.Text = Utils.Languages.GetLabelFromCode("SearchProjectViewAllNo", language);
            this.chkCreatoreExtendHistoricized.Text = Utils.Languages.GetLabelFromCode("SearchProjectCreatoreExtendHistoricized", language);
            this.litVisibility.Text = Utils.Languages.GetLabelFromCode("SearchProjectVisibility", language);
            this.optVisibility1.Text = Utils.Languages.GetLabelFromCode("SearchProjectVisibilityOpt1", language);
            this.optVisibility2.Text = Utils.Languages.GetLabelFromCode("SearchProjectVisibilityOpt2", language);
            this.optVisibility3.Text = Utils.Languages.GetLabelFromCode("SearchProjectVisibilityOpt3", language);
            this.SaveSearch.Title = Utils.Languages.GetLabelFromCode("SearchProjectSaveSearchTitle", language);
            this.ModifySearch.Title = Utils.Languages.GetLabelFromCode("SearchProjectModifySearchTitle", language);
            this.OpenTitolario.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("TitleClassificationScheme", language));
            this.ddlSavedSearches.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("SelectRapidSearch", language);
            this.lbl_dtaCloseFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.lbl_dtaCollocationFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.lbl_dtaCreateFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.lbl_dtaExpireFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.lbl_dtaOpenFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.ImgCreatoreAddressBook.AlternateText = Utils.Languages.GetLabelFromCode("ImgCreatoreAddressBook", language);
            this.ImgCreatoreAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("ImgCreatoreAddressBook", language);
            this.ImgProprietarioAddressBook.AlternateText = Utils.Languages.GetLabelFromCode("ImgProprietarioAddressBook", language);
            this.ImgProprietarioAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("ImgProprietarioAddressBook", language);
            this.ImgCollocazioneAddressBook.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgCollocationAddressBook", language);
            this.ImgCollocazioneAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgCollocationAddressBook", language);
            this.SearchProjectNewProject.Text = Utils.Languages.GetLabelFromCode("SearchProjectNewProject", language);
            this.MassiveReport.Title = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
            this.MassiveAddAdlUser.Title = Utils.Languages.GetLabelFromCode("SearchProjectMassiveAddAdlUserTitle", language);
            this.MassiveRemoveAdlUser.Title = Utils.Languages.GetLabelFromCode("SearchProjectMassiveRemoveAdlUserTitle", language);
            this.MassiveAddAdlRole.Title = Utils.Languages.GetLabelFromCode("SearchProjectMassiveAddAdlRoleTitle", language);
            this.MassiveRemoveAdlRole.Title = Utils.Languages.GetLabelFromCode("SearchProjectMassiveRemoveAdlRoleTitle", language);
            this.MassiveConservation.Title = Utils.Languages.GetLabelFromCode("SearchProjectMassiveConservationTitle", language);
            this.MassiveTransmission.Title = Utils.Languages.GetLabelFromCode("SearchProjectMassiveTransmissionTitle", language);
            this.ExportDati.Title = Utils.Languages.GetLabelFromCode("SearchProjectExportDatiTitle", language);
            this.GridPersonalizationPreferred.Title = Utils.Languages.GetLabelFromCode("projectImgPreferredGrids", language);
            this.GrigliaPersonalizzata.Title = Utils.Languages.GetLabelFromCode("projectImgEditGrid", language);
            this.GrigliaPersonalizzataSave.Title = Utils.Languages.GetLabelFromCode("projectImgSaveGrid", language);
        }

        private void LoadKeys()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_DESC_FASC.ToString())))
            {
                this.MaxLenghtProject = int.Parse(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_DESC_FASC.ToString()));
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_CONS"))
            {
                this.AllowConservazione = true;
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADD_ADL"))
            {
                this.AllowADL = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.CERCA_SOTTOFASCICOLI.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.CERCA_SOTTOFASCICOLI.ToString()].Equals("1"))
            {
                this.PlcSubProject.Visible = true;
            }
            else
            {
                this.PlcSubProject.Visible = false;
            }

            this.InitializePageSize();

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()]) && !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamicaFasc.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamicaFasc.ToString()].Equals("1"))
            {
                this.CustomDocuments = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()].Equals("1"))
            {
                this.EnableStateDiagram = true;
            }

            // INTREGRAZIONE PITRE-PARER
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_WA_CONSERVAZIONE.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_WA_CONSERVAZIONE.ToString()).Equals("1"))
            {
                this.IsConservazioneSACER = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]))
            {
                this.SearchDocumentDdlMassiveOperation.Visible = false;
                //this.SearchProjectLitRapidSearch.Visible = false;
                //this.SearchDocumentDeleteRapidSearch.Visible = false;
                //this.DdlRapidSearch.Visible = false;
                this.SearchProjectSave.Visible = false;
                this.SearchProjectEdit.Visible = false;
                this.SearchProjectRemove.Visible = false;
            }

            if (utils.GetAbilitazioneAtipicita())
            {
                this.plcVisibility.Visible = true;
                this.UpPnlVisibility.Update();
            }
        }

        protected void InitializePageSize()
        {
            string keyValue = Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_PAGING_ROW_PROJECT.ToString());
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

        protected void PopulateDDLRegistry(DocsPaWR.Ruolo role)
        {
            foreach (DocsPaWR.Registro reg in role.registri)
            {
                if (!reg.flag_pregresso)
                {
                    ListItem item = new ListItem();
                    item.Text = reg.codRegistro;
                    item.Value = reg.systemId;
                    this.DdlRegistries.Items.Add(item);
                }
            }

            if (this.DdlRegistries.Items.Count == 1)
            {
                this.plcRegistry.Visible = false;
                this.UpPnlRegistry.Update();
            }
        }

        protected void SearchProjectNoRegistro()
        {

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
                        TxtCodeProject.Text = codice;
                        this.Project = SottoFascicolo;

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

                DocsPaWR.Fascicolo[] listaFasc = getFascicolo(this.Registry);
                string codClassifica = string.Empty;
                if (listaFasc != null)
                {
                    if (listaFasc.Length > 0)
                    {
                        //caso 1: al codice digitato corrisponde un solo fascicolo
                        if (listaFasc.Length == 1)
                        {
                            this.TxtDescriptionProject.Text = listaFasc[0].descrizione;
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
            this.TxtDescriptionProject.Text = string.Empty;
            string codClassifica = string.Empty;

            if (string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                this.TxtDescriptionProject.Text = string.Empty;
                this.Project = null;
                return;
            }

            //FASCICOLAZIONE IN SOTTOFASCICOLI

            if (this.TxtCodeProject.Text.IndexOf("//") > -1)
            {
                #region FASCICOLAZIONE IN SOTTOFASCICOLI
                string codice = string.Empty;
                string descrizione = string.Empty;
                DocsPaWR.Fascicolo SottoFascicolo = getFolder(this.Registry, ref codice, ref descrizione);
                if (SottoFascicolo != null)
                {

                    if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
                    {
                        TxtDescriptionProject.Text = descrizione;
                        TxtCodeProject.Text = codice;
                        this.Project = SottoFascicolo;
                    }
                    else
                    {

                        //string msg = @"Attenzione, sottofascicolo non presente.";
                        string msg = "WarningDocumentSubFileNoFound";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        this.TxtDescriptionProject.Text = string.Empty;
                        this.TxtCodeProject.Text = string.Empty;
                        this.Project = null;
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
                }

                #endregion
            }
            else
            {
                DocsPaWR.Fascicolo[] listaFasc = getFascicoli(this.Registry);

                if (listaFasc != null)
                {
                    if (listaFasc.Length > 0)
                    {
                        //caso 1: al codice digitato corrisponde un solo fascicolo
                        if (listaFasc.Length == 1)
                        {
                            this.Project = listaFasc[0];
                            this.TxtDescriptionProject.Text = listaFasc[0].descrizione;
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
                            //ProjectManager.setFascicoloSelezionatoFascRapida(this, listaFasc[0]);
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

        protected void SearchCorrespondent(string addressCode, string idControl)
        {
            DocsPaWR.Corrispondente corr = UIManager.AddressBookManager.getCorrispondenteRubrica(addressCode, this.CallType);
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
                    case "txtCodiceCollocazione":
                        this.txtCodiceCollocazione.Text = string.Empty;
                        this.txtDescrizioneCollocazione.Text = string.Empty;
                        this.idCollocationAddr.Value = string.Empty;
                        this.upPnlCollocationAddr.Update();
                        break;
                }

                string msg = "ErrorTransmissionCorrespondentNotFound";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
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
                    case "txtCodiceCollocazione":
                        this.txtCodiceCollocazione.Text = corr.codiceRubrica;
                        this.txtDescrizioneCollocazione.Text = corr.descrizione;
                        this.idCollocationAddr.Value = corr.systemId;
                        this.upPnlCollocationAddr.Update();
                        break;
                }
            }
        }

        private void SetCheckBox()
        {
            try
            {
                bool checkAll = this.CheckAll;

                if (!string.IsNullOrEmpty(this.HiddenProjectsChecked.Value))
                {
                    //salvo i check spuntati alla pagina cliccata in precedenza
                    string[] items = new string[1] { this.HiddenProjectsChecked.Value };
                    if (this.HiddenProjectsChecked.Value.IndexOf(",") > 0)
                        items = this.HiddenProjectsChecked.Value.Split(',');

                    foreach (string item in items)
                    {
                        string key = item.Split('_')[0];
                        string value = item.Split('_')[1].Replace("<span style=\"color:Black;\">", "").Replace("</span>", "");
                        if (!this.ListCheck.ContainsKey(key))
                            this.ListCheck.Add(key, value);
                    }
                }


                if (!string.IsNullOrEmpty(this.HiddenProjectsUnchecked.Value))
                {
                    this.CheckAll = false;

                    // salvo i check non spuntati alla pagina cliccata in precedenza
                    string[] items = new string[1] { this.HiddenProjectsUnchecked.Value };
                    if (this.HiddenProjectsUnchecked.Value.IndexOf(",") > 0)
                        items = this.HiddenProjectsUnchecked.Value.Split(',');

                    foreach (string item in items)
                    {
                        string key = item.Split('_')[0];
                        string value = item.Split('_')[1];
                        if (this.ListCheck.ContainsKey(key))
                            this.ListCheck.Remove(key);
                    }
                }


                if (string.IsNullOrEmpty(this.HiddenProjectsAll.Value))
                {
                    string js = string.Empty;
                    foreach (KeyValuePair<string, string> d in this.ListCheck)
                    {
                        if (!string.IsNullOrEmpty(js)) js += ",";
                        js += d.Key;
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "clearCheckboxes", "clearCheckboxes('false', '" + js + "');", true);
                }

                this.HiddenProjectsChecked.Value = string.Empty;
                this.HiddenProjectsUnchecked.Value = string.Empty;
                this.upPnlButtons.Update();
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
