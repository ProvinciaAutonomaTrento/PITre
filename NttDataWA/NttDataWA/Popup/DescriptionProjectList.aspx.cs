using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class DescriptionProjectList : System.Web.UI.Page
    {
        #region Properties

        /// <summary>
        /// numero di caratteri nella nota di testo
        /// </summary>
        private int MaxLenghtProject
        {
            get
            {
                int result = 2000;
                if (HttpContext.Current.Session["MaxLenghtProject"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["MaxLenghtProject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["MaxLenghtNote"] = value;
            }
        }

        private int SelectedPage
        {
            get
            {
                int toReturn = 1;
                if (HttpContext.Current.Session["selectedPageDescriptionProjectList"] != null) Int32.TryParse(HttpContext.Current.Session["selectedPageDescriptionProjectList"].ToString(), out toReturn);
                if (toReturn < 1) toReturn = 1;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["selectedPageDescriptionProjectList"] = value;
            }
        }

        private int RecordCount
        {
            get
            {
                int toReturn = 0;
                if (HttpContext.Current.Session["recordCountDescriptionProjectList"] != null) Int32.TryParse(HttpContext.Current.Session["recordCountDescriptionProjectList"].ToString(), out toReturn);
                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["recordCountDescriptionProjectList"] = value;
            }
        }


        public int PageCount
        {
            get
            {
                int toReturn = 1;
                if (HttpContext.Current.Session["PageCountDescriptionProjectList"] != null)
                {
                    Int32.TryParse(
                        HttpContext.Current.Session["PageCountDescriptionProjectList"].ToString(),
                        out toReturn);
                }
                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["PageCountDescriptionProjectList"] = value;
            }
        }

        public List<FiltroDescrizioniFascicolo> Filtro
        {
            get
            {
                return HttpContext.Current.Session["FiltroDescrizioniFascicolo"] as List<FiltroDescrizioniFascicolo>;
            }
            set
            {
                HttpContext.Current.Session["FiltroDescrizioniFascicolo"] = value;
            }
        }

        private DescrizioneFascicolo DescrizioneFascicoloSelezionata
        {
            set { HttpContext.Current.Session["DescrizioneFascicoloSelezionata"] = value; }
        }
        #endregion

        #region Constant

        private const string PANEL_GRID_INDEXES = "upPnlGridIndexes";
        private const string PANEL_GRID_PAGE_INDEX = "upPnlGridPageIndex";

        #endregion

        #region Standard method
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                InitializeLanguage();
                InitializeKey();
                InitializePage();
                EnableButton();
            }
            else
            {
                if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(PANEL_GRID_PAGE_INDEX))
                {
                    this.grid_rowindex.Value = string.Empty;
                    List<DescrizioneFascicolo> descriptionList = SearchDescriptionProject();
                    BindGridDescriptionProject(descriptionList);
                    EnableButton();
                    return;
                }
                if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(PANEL_GRID_INDEXES))
                {
                    this.HighlightSelectedRow();
                    this.BuildGridNavigator();
                    EnableButton();
                    this.TxtCodObject.Text = (GridDescriptionProject.Rows[this.GridDescriptionProject.SelectedIndex].FindControl("lblCodObject") as Label).Text;
                    this.TxtDescObject.Text = (GridDescriptionProject.Rows[this.GridDescriptionProject.SelectedIndex].FindControl("lblDescObject") as Label).Text;
                    this.UpdPnlCodeObject.Update();
                    this.UpdPnlGridDescriptionProject.Update();
                    return;
                }
            }
            RefreshScript();
        }

        private void InitializePage()
        {
            LoadRegistryRF();
        }

        private void InitializeKey()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_DESC_FASC.ToString())))
            {
                this.MaxLenghtProject = int.Parse(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_DESC_FASC.ToString()));
            }
            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_DESCRIZIONI_FASC_INS"))
            {
                this.ObjectBtnInsert.Visible = false;
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            ObjectLblRegistry.Text = Utils.Languages.GetLabelFromCode("ObjectLblRegistry", language);
            ObjectLblCodObject.Text = Utils.Languages.GetLabelFromCode("ProjectLblCodDescription", language);
            ObjectLblVoiceObject.Text = Utils.Languages.GetLabelFromCode("ProjectLblDescription", language);
            ObjectBtnSearch.Text = Utils.Languages.GetLabelFromCode("ObjectBtnSearch", language);
            ObjectBtnInsert.Text = Utils.Languages.GetLabelFromCode("ObjectBtnInsert", language);
            ObjectBtnUpdate.Text = Utils.Languages.GetLabelFromCode("ObjectBtnUpdate", language);
            ObjectBtnDelete.Text = Utils.Languages.GetLabelFromCode("ObjectBtnDelete", language);
            //ObjectLblFindRis.Text = Utils.Languages.GetLabelFromCode("ObjectLblFindRis", language);
            ObjectBtnChiudi.Text = Utils.Languages.GetLabelFromCode("ObjectBtnChiudi", language);
            this.DocumentLitObjectChAv.Text = Utils.Languages.GetLabelFromCode("DocumentLitObjectChAv", language);
            this.ObjectBtnInitialize.ToolTip = Utils.Languages.GetLabelFromCode("DocumentObjectBtnInitialize", language);
            this.ObjectBtnInitialize.AlternateText = Utils.Languages.GetLabelFromCode("DocumentObjectBtnInitialize", language);
        }

        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);

            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshprojectTxtDescrizione", "charsLeft('projectTxtDescrizione', '2000' , '" + this.projectLtrDescrizione.Text.Replace("'", "\'") + "');", true);
            //this.projectTxtDescrizione_chars.Attributes["rel"] = "projectTxtDescrizione_'2000'_" + this.projectLtrDescrizione.Text;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshprojectTxtDescrizione", "charsLeft('TxtDescObject', " + this.MaxLenghtProject + ", '" + this.DocumentLitObjectChAv.Text.Replace("'", "\'") + "');", true);
            this.TxtDescObject_chars.Attributes["rel"] = "TxtDescObject_'" + this.MaxLenghtProject + "'_" + this.DocumentLitObjectChAv.Text;
        }

        /// <summary>
        /// Load registry and RF
        /// </summary>
        private void LoadRegistryRF()
        {
            this.DdlRegRf.Items.Add(new ListItem() { Text = "TUTTI", Value = string.Empty, Selected = true });
            Registro[] ListRegistriesAndRF = RegistryManager.GetRegAndRFListInSession();
            //Registro[] ListRegistriesAndRF = RegistryManager.GetListRegistriesAndRF(UIManager.RoleManager.GetRoleInSession().systemId, string.Empty, string.Empty);
            var PairRegRF = (from reg in ListRegistriesAndRF
                                where reg.chaRF.Equals("0")
                                select (new
                                {
                                    IdReg = reg.systemId,
                                    CodReg = reg.codRegistro,
                                    ListItemRF =
                                        (from rf in ListRegistriesAndRF
                                        where rf.idAOOCollegata == reg.systemId
                                        select new ListItem() { Text = rf.codRegistro, Value = rf.systemId }).ToArray()
                                }));
            if (PairRegRF != null)
            {
                foreach (var RegAssRF in PairRegRF)
                {
                    this.DdlRegRf.Items.Add(new ListItem() { Text = RegAssRF.CodReg, Value = RegAssRF.IdReg });
                    if ((RegAssRF.ListItemRF as ListItem[]) != null && (RegAssRF.ListItemRF as ListItem[]).Length > 0)
                    {
                        foreach (ListItem li in (RegAssRF.ListItemRF as ListItem[]))
                            li.Attributes.Add("style", "margin-left:10px;");
                        this.DdlRegRf.Items.AddRange(RegAssRF.ListItemRF as ListItem[]);
                    }
                }
            }
            //this.ObjectRegistryList = ListRegistriesAndRF;
        }
        #endregion

        #region Event button
        protected void ObjectBtnSearch_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                this.SelectedPage = 0;
                this.grid_pageindex.Value = string.Empty;
                this.grid_rowindex.Value = string.Empty;

                this.Filtro = this.BindFilter();

                List<DescrizioneFascicolo> descriptionList = SearchDescriptionProject();
                BindGridDescriptionProject(descriptionList);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private List<DescrizioneFascicolo> SearchDescriptionProject()
        {
            List<DescrizioneFascicolo> descriptionList = new List<DescrizioneFascicolo>();

            int numPage = 1;
            int numTotPage = 0;
            int nRec = 0;

            if (!string.IsNullOrEmpty(this.grid_pageindex.Value))
            {
                numPage = int.Parse(this.grid_pageindex.Value);
            }
            this.SelectedPage = numPage;

            descriptionList = ProjectManager.GetListDescrizioniFascicolo(this.Filtro, numPage, this.GridDescriptionProject.PageSize, out numTotPage, out nRec);

            this.RecordCount = nRec;
            this.PageCount = (int)Math.Round(((double)nRec / (double)GridDescriptionProject.PageSize) + 0.49);

            return descriptionList;
        }

        private List<FiltroDescrizioniFascicolo> BindFilter()
        {
            List<FiltroDescrizioniFascicolo> filters = new List<FiltroDescrizioniFascicolo>();
            FiltroDescrizioniFascicolo filter;

            if(!string.IsNullOrEmpty(this.TxtCodObject.Text))
            {
                filter = new FiltroDescrizioniFascicolo();
                filter.Argomento = DocsPaWR.FiltriDescrizioniFascicoli.CODICE.ToString();
                filter.Valore = this.TxtCodObject.Text;
                filters.Add(filter);
            }

            if (!string.IsNullOrEmpty(this.TxtDescObject.Text))
            {
                filter = new FiltroDescrizioniFascicolo();
                filter.Argomento = DocsPaWR.FiltriDescrizioniFascicoli.DESCRIZIONE.ToString();
                filter.Valore = this.TxtDescObject.Text;
                filters.Add(filter);
            }

            #region REGISTRO

            string idRegIdRf = string.Empty;
            if (string.IsNullOrEmpty(this.DdlRegRf.SelectedValue))
            {
                Registro[] ListRegistriesAndRF = RegistryManager.GetRegAndRFListInSession();
                foreach (Registro reg in ListRegistriesAndRF)
                {
                    if (!string.IsNullOrEmpty(idRegIdRf))
                        idRegIdRf += "_";

                    idRegIdRf += reg.systemId;
                }
            }
            else
            {
                idRegIdRf = this.DdlRegRf.SelectedValue;
            }

            filter = new FiltroDescrizioniFascicolo();
            filter.Argomento = DocsPaWR.FiltriDescrizioniFascicoli.REGISTRO.ToString();
            filter.Valore = idRegIdRf;
            filters.Add(filter);

            #endregion

            return filters;
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                string error = string.Empty;
                if(string.IsNullOrEmpty(this.TxtDescObject.Text.Trim()))
                {
                    error = "WarningDescriptionProjectNotEmpty";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectLength", "parent.ajaxDialogModal('" + error + "', 'warning');", true);
                    return;
                }
                if (string.IsNullOrEmpty(this.DdlRegRf.SelectedItem.Value))
                {
                    error = "ErrorRegisterNotSelected";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorRegisterNotSelected", "parent.ajaxDialogModal('" + error + "', 'warning');", true);
                    return;
                }
                Registro reg = (from r in RegistryManager.GetRegAndRFListInSession()
                                where r.systemId.Equals(this.DdlRegRf.SelectedValue)
                                select r).FirstOrDefault();
                if (reg != null)
                {
                    if (reg.chaRF.Equals("0"))
                    {
                        error = "WarningDescriptionProjectInsertReg";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorRegisterNotSelected", "parent.ajaxDialogModal('" + error + "', 'warning');", true);
                        return;
                    }
                    DescrizioneFascicolo descFasc = new DescrizioneFascicolo();
                    descFasc.Descrizione = this.TxtDescObject.Text;
                    descFasc.Codice = this.TxtCodObject.Text;
                    descFasc.IdRegistro = reg.systemId;
                    descFasc.IdAmm = UserManager.GetInfoUser().idAmministrazione;
                    ResultDescrizioniFascicolo resultInsDescFasc = ResultDescrizioniFascicolo.OK;
                    bool result = ProjectManager.InsertDescrizioneFascicolo(descFasc, out resultInsDescFasc);
                    if (!result)
                    {
                        error = GetErrorMessage(resultInsDescFasc);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectLength", "parent.ajaxDialogModal('" + error + "', 'warning');", true);
                        return;
                    }
                    this.SelectedPage = 0;
                    this.grid_pageindex.Value = string.Empty;
                    this.grid_rowindex.Value = "0";
                    this.Filtro = this.BindFilter();
                    List<DescrizioneFascicolo> descriptionList  = this.SearchDescriptionProject();
                    BindGridDescriptionProject(descriptionList);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private string GetErrorMessage(ResultDescrizioniFascicolo resultDescFasc)
        {
            string error = string.Empty;

            switch (resultDescFasc)
            {
                case ResultDescrizioniFascicolo.DESCRIZIONE_PRESENTE:
                    error = "WarningInsertDescriptionProject";
                    break;
                default:
                    error = "ErrorInsertDescriptionProject";
                    break;
            }

            return error;
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                string error = string.Empty;
                if (string.IsNullOrEmpty(this.TxtDescObject.Text.Trim()))
                {
                    error = "WarningDescriptionProjectNotEmpty";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectLength", "parent.ajaxDialogModal('" + error + "', 'warning');", true);
                    return;
                }
                DescrizioneFascicolo descFasc = new DescrizioneFascicolo();
                descFasc.SystemId = (GridDescriptionProject.Rows[this.GridDescriptionProject.SelectedIndex].FindControl("lblSystemid") as Label).Text;
                descFasc.Codice = this.TxtCodObject.Text;
                descFasc.Descrizione = this.TxtDescObject.Text;
                descFasc.IdRegistro = (GridDescriptionProject.Rows[this.GridDescriptionProject.SelectedIndex].FindControl("lblIdRegistro") as Label).Text;
                descFasc.IdAmm = (GridDescriptionProject.Rows[this.GridDescriptionProject.SelectedIndex].FindControl("lblIdAmm") as Label).Text;

                ResultDescrizioniFascicolo resultUpdateDescFasc = ResultDescrizioniFascicolo.OK;
                if (!ProjectManager.AggiornaDescrizioneFascicolo(descFasc, out resultUpdateDescFasc))
                {
                    error = GetErrorMessage(resultUpdateDescFasc);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorObjectLength", "parent.ajaxDialogModal('" + error + "', 'warning');", true);
                    return;
                }

                (GridDescriptionProject.Rows[this.GridDescriptionProject.SelectedIndex].FindControl("lblCodObject") as Label).Text = descFasc.Codice;
                (GridDescriptionProject.Rows[this.GridDescriptionProject.SelectedIndex].FindControl("lblDescObject") as Label).Text = descFasc.Descrizione;
                this.UpdPnlGridDescriptionProject.Update();
                this.grid_rowindex.Value = string.Empty;
                this.HighlightSelectedRow();
                EnableButton();
                this.TxtCodObject.Text = string.Empty;
                this.TxtDescObject.Text = string.Empty;
                this.UpdPnlCodeObject.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                string systemId = (GridDescriptionProject.Rows[this.GridDescriptionProject.SelectedIndex].FindControl("lblSystemid") as Label).Text;
                if(!ProjectManager.EliminaDescrizioneFascicolo(systemId))
                {
                    return;
                }
                this.ResetPage();
                List<DescrizioneFascicolo> listDescFasc = this.SearchDescriptionProject();
                BindGridDescriptionProject(listDescFasc);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private void ResetPage()
        {
            this.SelectedPage = 0;
            this.grid_pageindex.Value = string.Empty;
            this.grid_rowindex.Value = string.Empty;
            this.HighlightSelectedRow();
            this.EnableButton();
            this.TxtCodObject.Text = string.Empty;
            this.TxtDescObject.Text = string.Empty;
            this.UpdPnlCodeObject.Update();
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                DescrizioneFascicolo descFasc = new DescrizioneFascicolo();
                descFasc.SystemId = (GridDescriptionProject.Rows[this.GridDescriptionProject.SelectedIndex].FindControl("lblSystemid") as Label).Text;
                descFasc.Codice = this.TxtCodObject.Text;
                descFasc.Descrizione = this.TxtDescObject.Text;

                this.DescrizioneFascicoloSelezionata = descFasc;
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('DescriptionProjectList','up');", true);

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void ObjectBtnChiudi_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('DescriptionProjectList','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
        protected void ObjectBtnInitialize_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                this.DdlRegRf.SelectedIndex = -1;
                this.TxtCodObject.Text = string.Empty;
                TxtDescObject.Text = string.Empty;
                this.UpdPnlRegistry.Update();
                this.UpdPnlCodeObject.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private void EnableButton()
        {
            bool enable = false;
            if(!string.IsNullOrEmpty(this.grid_rowindex.Value))
            {
                enable = true;
            }
            this.BtnOk.Enabled = enable;
            this.ObjectBtnDelete.Enabled = enable;
            this.ObjectBtnUpdate.Enabled = enable;
            this.UpPnlButtons.Update();
        }
        #endregion

        #region Gridview
        protected void GridDescriptionProject_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "'); __doPostBack('upPnlGridIndexes', ''); return false;";
            }
        }

        protected void BuildGridNavigator()
        {
            try
            {
                this.plcNavigator.Controls.Clear();

                int countPage = this.PageCount;

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
                        btn.Attributes["onclick"] = " $('#grid_pageindex').val(" + (startFrom - 1) + "); __doPostBack('upPnlGridPageIndex', ''); return false;";
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
                            btn.Attributes["onclick"] = " $('#grid_pageindex').val($(this).text()); __doPostBack('upPnlGridPageIndex', ''); return false;";
                            panel.Controls.Add(btn);
                        }
                    }

                    if (endTo < countPage)
                    {
                        LinkButton btn = new LinkButton();
                        btn.EnableViewState = true;
                        btn.Text = "...";
                        btn.Attributes["onclick"] = " $('#grid_pageindex').val(" + endTo + "); __doPostBack('upPnlGridPageIndex', ''); return false;";
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

        private void BindGridDescriptionProject(List<DescrizioneFascicolo> descriptionList)
        {
            this.GridDescriptionProject.DataSource = descriptionList;
            this.GridDescriptionProject.DataBind();
            this.BuildGridNavigator();
            this.HighlightSelectedRow();
            this.UpdPnlGridDescriptionProject.Update();
        }

        private void HighlightSelectedRow()
        {
            if (this.GridDescriptionProject.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(this.grid_rowindex.Value))
                {
                    this.GridDescriptionProject.SelectedIndex = Convert.ToInt32(this.grid_rowindex.Value);
                    GridViewRow gvRow = this.GridDescriptionProject.SelectedRow;
                    foreach (GridViewRow GVR in this.GridDescriptionProject.Rows)
                    {
                        if (GVR == gvRow)
                        {
                            GVR.CssClass += " selectedrow";
                        }
                        else
                        {
                            GVR.CssClass = GVR.CssClass.Replace(" selectedrow", "");
                        }
                    }
                }
                else
                {
                    this.GridDescriptionProject.SelectedIndex = 0;
                    foreach (GridViewRow GVR in this.GridDescriptionProject.Rows)
                    {
                        GVR.CssClass = GVR.CssClass.Replace(" selectedrow", "");
                    }
                }
            }
        }
        #endregion

    }
}