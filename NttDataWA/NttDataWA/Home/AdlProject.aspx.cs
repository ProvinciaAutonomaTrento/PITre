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


namespace NttDataWA.Home
{
    public partial class AdlProject : System.Web.UI.Page
    {

        public SearchManager schedaRicerca = null;
        private const string KEY_SCHEDA_RICERCA = "HomePrjAdl";

        #region Properties

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

        protected Grid SelectedGrid
        {
            get
            {
                return HttpContext.Current.Session["AdlProject.SelectedGrid"] as Grid;
            }
            set
            {
                HttpContext.Current.Session["AdlProject.SelectedGrid"] = value;
            }
        }

        #endregion

        #region Events

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

                    //Back
                    if (this.Request.QueryString["back"] != null && this.Request.QueryString["back"].Equals("1"))
                    {
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject obj = navigationList.Last();
                        if (!obj.CodePage.Equals(Navigation.NavigationUtils.NamePage.HOME_ADL_PROJECT.ToString()))
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

                        this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, this.SelectedGrid);

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

                        this.UpnlGrid.Update();
                    }
                    else
                    {
                        bool result = this.ricercaFascicoli();
                        if (result)
                        {
                            this.SelectedRow = string.Empty;
                            this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, this.SelectedGrid);
                        }
                    }


                    this.UpnlGrid.Update();

                    this.VisibiltyRoleFunctions();
                }
                else
                {
                    if (this.Result != null && this.Result.Length > 0)
                    {
                        // Lista dei documenti risultato della ricerca
                        if (!this.SelectedPage.ToString().Equals(this.grid_pageindex.Value))
                        {
                            this.Result = null;
                            this.SelectedRow = string.Empty;
                            if (!string.IsNullOrEmpty(this.grid_pageindex.Value))
                            {
                                this.SelectedPage = int.Parse(this.grid_pageindex.Value);
                            }
                            this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, this.SelectedGrid);
                            this.BuildGridNavigator();
                            this.UpnlGrid.Update();
                            this.upPnlGridIndexes.Update();
                        }
                        else
                        {
                            this.ShowResult(this.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage);
                        }
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

        protected void RblTypeAdl_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool result = this.ricercaFascicoli();
            if (result)
            {
                this.SelectedRow = string.Empty;
                this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, this.SelectedGrid);
            }
            this.UpnlGrid.Update();
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
                            labelAdlRole = "ProjectIconTemplateAdlRoleProject";
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


                    ((CustomImageButton)e.Row.FindControl("conservazione")).Visible = this.AllowConservazione;
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
                            btnIm.ToolTip = Utils.Languages.GetLabelFromCode("ProjectIconTemplateAdl", language);
                            btnIm.AlternateText = Utils.Languages.GetLabelFromCode("ProjectIconTemplateAdl", language);
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
                        this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, this.SelectedGrid);
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
                        this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, this.SelectedGrid);
                        break;
                    }
                case "visualizzadocumento":
                    {
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                        actualPage.IdObject = fascicolo.systemID;
                        actualPage.OriginalObjectId = fascicolo.systemID;
                        actualPage.NumPage = this.SelectedPage.ToString();
                        actualPage.SearchFilters = this.SearchFilters;
                        actualPage.PageSize = this.PageSize.ToString();
                        actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME_ADL_PROJECT.ToString(), string.Empty);
                        actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.HOME_ADL_PROJECT.ToString(), true, this.Page);
                        actualPage.CodePage = Navigation.NavigationUtils.NamePage.HOME_ADL_PROJECT.ToString();
                        actualPage.Page = "ADLPROJECT.ASPX";
                        actualPage.DxTotalNumberElement = this.RecordCount.ToString();
                        actualPage.ViewResult = true;

                        if (this.PagesCount == 0)
                        {
                            actualPage.DxTotalPageNumber = "1";
                        }
                        else
                        {
                            actualPage.DxTotalPageNumber = this.PagesCount.ToString();
                        }

                        int indexElement = ((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1);
                        actualPage.DxPositionElement = indexElement.ToString();

                        navigationList.Add(actualPage);

                        Navigation.NavigationUtils.SetNavigationList(navigationList);

                        UIManager.ProjectManager.setProjectInSession(fascicolo);
                        Response.Redirect("~/Project/project.aspx");
                        break;
                    }

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
                        this.ricercaFascicoli();
                        this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, this.SelectedGrid);
                    }
                    else
                    {
                        this.ShowGrid(this.SelectedGrid, null, 0, 0);
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
            //(this.Page.Master.FindControl("upPnlInfoUser") as UpdatePanel).Update();
            /*
            try
            {
                bool result = this.ricercaFascicoli();
                if (result)
                {
                    this.SelectedRow = string.Empty;
                    this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, this.SelectedGrid);
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
            this.ClearSessionProperties();
            this.InitializeLanguage();
            this.InitializePageSize();
            this.VisibiltyRoleFunctions();
            this.LoadRolesUser();
        }

        private void ClearSessionProperties()
        {
            Session.Remove("itemUsedSearch");
            Session.Remove("idRicercaSalvata");
            this.SelectedGrid = GridManager.GetStandardGridForUser(GridTypeEnumeration.Project, UIManager.UserManager.GetInfoUser());
            this.SelectedGrid.Fields.Where(e => e.FieldId == "U1").FirstOrDefault().Visible = true;
            this.SelectedGrid.Fields.Where(e => e.FieldId == "DTA_ADL").FirstOrDefault().Visible = true;
            this.SelectedGrid.Fields.Where(e => e.FieldId == "DTA_ADL").FirstOrDefault().Position = 0;
            this.SelectedGrid.Fields.Where(e => e.FieldId == "MOTIVO_ADL").FirstOrDefault().Visible = true;

            this.SelectedRow = string.Empty;
            this.Result = null;
            this.SearchFilters = null;
            this.Template = null;
            this.RecordCount = 0;
            this.PagesCount = 0;
            this.SelectedPage = 1;
            this.Registry = RoleManager.GetRoleInSession().registri[0];
            this.AllClassification = false;
            this.Classification = null;
            this.CellPosition = new Dictionary<string, int>();

            UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);

            // Caricamento della griglia se non ce n'è una già selezionata
            if (this.SelectedGrid == null || this.SelectedGrid.GridType != GridTypeEnumeration.Project)
            {
                this.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Project);
            }

            this.ShowGrid(this.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage);
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            this.headerHomeLblRole.Text = Utils.Languages.GetLabelFromCode("HeaderHomeLblRole", language);
            this.RblTypeAdl.Items[0].Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdlUser", language);
            this.RblTypeAdl.Items[1].Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdlRole", language);
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

        private void VisibiltyRoleFunctions()
        {
            this.ShowGridPersonalization = UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION");

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
            {
                this.AllowADLRole = true;
                this.PnlAdlRoleUserHome.Visible = true;
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_CONS"))
            {
                this.AllowConservazione = true;
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADD_ADL"))
            {
                this.AllowADL = true;
            }
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
            fV1.valore = this.GetTitolarioId();
            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

            #endregion
            #region  filtro sulla tipologia del fascicolo
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriFascicolazione.TIPO_FASCICOLO.ToString();
            fV1.valore = "P";
            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);
            #endregion
            #region Mostra tutti i fascicoli

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriFascicolazione.INCLUDI_FASCICOLI_FIGLI.ToString();
            fV1.valore = "S";
            fVList = GridManager.AddToArrayFiltroRicerca(fVList, fV1);

            #endregion
            #region filtro RICERCA IN AREA LAVORO
            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriFascicolazione.DOC_IN_FASC_ADL.ToString();
            if (this.RblTypeAdl.SelectedValue == "0")
                fV1.valore = UserManager.GetUserInSession().idPeople.ToString() + "@" + RoleManager.GetRoleInSession().systemId.ToString();
            else
                fV1.valore = "0@" + RoleManager.GetRoleInSession().systemId.ToString();
            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
            #endregion

            #region Ordinamento
            List<FiltroRicerca> filterList = GridManager.GetOrderFilter(this.SelectedGrid);

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

        private string GetTitolarioId()
        {
            ArrayList listaTitolari = ClassificationSchemeManager.getTitolariUtilizzabili();

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
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            valueTutti += titolario.ID + ",";
                            break;
                        case DocsPaWR.OrgStatiTitolarioEnum.Chiuso:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
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
                }

                return valueTutti;
            }
            else
            {
                DocsPaWR.OrgTitolario titolario = (DocsPaWR.OrgTitolario)listaTitolari[0];
                return titolario.ID;
            }
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CombineRowsHover", "CombineRowsHover();", true);
        }

        /// <summary>
        /// Funzione per la visualizzazione dei risutati della ricerca
        /// </summary>
        /// <param name="result">I risultati della ricerca</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        private void ShowResult(Grid selectedGrid, SearchObject[] result, int recordNumber, int selectedPage)
        {
            this.ShowGrid(selectedGrid, this.Result, this.RecordCount, selectedPage);
            this.gridViewResult.SelectedIndex = this.SelectedPage;
            this.BuildGridNavigator();
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
            }
            else
            {
                this.ShowGrid(selectedGrid, null, 0, 0);
                this.BuildGridNavigator();
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

            this.UpnlGrid.Update();
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

            if (this.SelectedGrid == null || this.SelectedGrid.GridType != GridTypeEnumeration.Project)
                this.SelectedGrid = GridManager.GetStandardGridForUser(GridTypeEnumeration.Project, UIManager.UserManager.GetInfoUser());



            visibleFieldsTemplate = this.SelectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field)) && e.CustomObjectId != 0).ToList();

            if (visibleFieldsTemplate != null && visibleFieldsTemplate.Count > 0)
            {
                visibleArray = visibleFieldsTemplate.ToArray();
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
            this.PagesCount = pageNumbers;
            this.Result = toReturn;

            // Restituzione della lista di fascicoli da visualizzare
            return toReturn;

        }

        protected void BuildGridNavigator()
        {
            try
            {
                this.plcNavigator.Controls.Clear();

                int countPage = this.PagesCount;

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
                            //MOTIVO INSERIMENTO IN ADL
                            case "MOTIVO_ADL":
                                if ((prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault() != null))
                                {
                                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
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

            // Inserimento di tutti i documenti nell'area di conservazione

            //Old Code
            #region OldCode
            /*
            foreach (String id in idProjects)
            {
                temp = this.InsertDocumentInStorageArea(id, objectId);

                if (!String.IsNullOrEmpty(temp))
                    toReturn.AppendLine(temp);
            }
            */
            #endregion
            //End OldCode

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

        protected bool GetGridPersonalization()
        {
            return this.ShowGridPersonalization;
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


                List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                actualPage.IdObject = fascicolo.systemID;
                actualPage.OriginalObjectId = fascicolo.systemID;
                actualPage.NumPage = this.SelectedPage.ToString();
                actualPage.SearchFilters = this.SearchFilters;
                actualPage.PageSize = this.PageSize.ToString();
                actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME_ADL_PROJECT.ToString(), string.Empty);
                actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.HOME_ADL_PROJECT.ToString(), true, this.Page);
                actualPage.CodePage = Navigation.NavigationUtils.NamePage.HOME_ADL_PROJECT.ToString();
                actualPage.Page = "ADLPROJECT.ASPX";
                actualPage.DxTotalNumberElement = this.RecordCount.ToString();
                actualPage.ViewResult = true;

                if (this.PagesCount == 0)
                {
                    actualPage.DxTotalPageNumber = "1";
                }
                else
                {
                    actualPage.DxTotalPageNumber = this.PagesCount.ToString();
                }

                int indexElement = (((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1)) + 1;
                actualPage.DxPositionElement = indexElement.ToString();

                navigationList.Add(actualPage);

                Navigation.NavigationUtils.SetNavigationList(navigationList);

                UIManager.ProjectManager.setProjectInSession(fascicolo);
                Response.Redirect("~/Project/project.aspx");

            }
        }

        #endregion

    }
}