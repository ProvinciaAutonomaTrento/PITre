using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class MassiveTransmissionAccept : System.Web.UI.Page
    {
        #region Properties

        private int SelectedPage
        {
            get
            {
                int toReturn = 1;
                if (HttpContext.Current.Session["SelectedPageMassiveTransmissionAccept"] != null) Int32.TryParse(HttpContext.Current.Session["SelectedPageMassiveTransmissionAccept"].ToString(), out toReturn);
                if (toReturn < 1) toReturn = 1;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["SelectedPageMassiveTransmissionAccept"] = value;
            }

        }

        private int PageCount
        {
            get
            {
                int toReturn = 1;
                if (HttpContext.Current.Session["PageCountMassiveTransmissionAccept"] != null) Int32.TryParse(HttpContext.Current.Session["PageCountMassiveTransmissionAccept"].ToString(), out toReturn);
                if (toReturn < 1) toReturn = 1;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["PageCountMassiveTransmissionAccept"] = value;
            }

        }

        protected bool CheckAllTransmission
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["CheckAllTransmission"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["CheckAllTransmission"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["CheckAllTransmission"] = value;
            }
        }

        private List<string> IdTrasmissioneSingoleSelezionate
        {
            get
            {
                return (List<string>)HttpContext.Current.Session["IdTrasmissioneSingoleSelezionate"];
            }
            set
            {
                HttpContext.Current.Session["IdTrasmissioneSingoleSelezionate"] = value;
            }
        }

        private List<string> IdTrasmissioneSingole
        {
            get
            {
                return (List<string>)HttpContext.Current.Session["IdTrasmissioneSingole"];
            }
            set
            {
                HttpContext.Current.Session["IdTrasmissioneSingole"] = value;
            }
        }

        #endregion

        #region Constant

        private const string UP_PNL_GRID_INDEXES  = "upPnlGridIndexes";

        #endregion

        #region Standard Method
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
            }
            else
            {
                if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_PNL_GRID_INDEXES))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                    if (!this.SelectedPage.Equals(this.grid_pageindex.Value))
                    {
                        this.SelectedPage = Convert.ToInt32(this.grid_pageindex.Value);
                        this.GridTrasmissioniPendenti_Bind();
                        this.UpGridTrasmissioniPendenti.Update();
                    }
                }
            }

            RefreshScript();
        }

        private void ClearSession()
        {
            HttpContext.Current.Session.Remove("CheckAllTransmission");
            HttpContext.Current.Session.Remove("IdTrasmissioneSingole");
            HttpContext.Current.Session.Remove("IdTrasmissioneSingoleSelezionate");
            HttpContext.Current.Session.Remove("SelectedPageMassiveTransmissionAccept");
            HttpContext.Current.Session.Remove("PageCountMassiveTransmissionAccept");
        }

        private void InitializePage()
        {
            ClearSession();
            InitializeLanguage();
            GridTrasmissioniPendenti_Bind();
        }

        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.MassiveTransmissionAcceptBtnAccept.Text = Utils.Languages.GetLabelFromCode("MassiveTransmissionAcceptBtnAccept", language);
            this.MassiveTransmissionAcceptBtnCancel.Text = Utils.Languages.GetLabelFromCode("MassiveTransmissionAcceptBtnCancel", language);
            this.LblNoteAccettazione.Text = Utils.Languages.GetLabelFromCode("MassiveTransmissionAcceptLblNoteAccettazione", language);
            this.LitNoteAccettazione.Text = Utils.Languages.GetLabelFromCode("DocumentLitObjectChAv", language);
        }

        protected void RefreshScript()
        {         
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshNoteAccettazione", "charsLeft('txt_NoteAccettazione', " + this.txt_NoteAccettazione.MaxLength + ", '" + this.LitNoteAccettazione.Text.Replace("'", "\'") + "');", true);
            this.txt_NoteAccettazione_chars.Attributes["rel"] = "txt_NoteAccettazione_" + this.txt_NoteAccettazione.MaxLength + "_" + this.LitNoteAccettazione.Text;
        }
        #endregion

        #region Event Button

        protected void MassiveTransmissionAcceptBtnCancel_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('MassiveTransmissionAccept','up');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void MassiveTransmissionAcceptBtnAccept_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                SetCheckBox();
                if (this.IdTrasmissioneSingoleSelezionate == null || this.IdTrasmissioneSingoleSelezionate.Count == 0)
                {
                    string msg = "WarningMassiveTransmissionAccept";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }
                if(!TrasmManager.AcceptTransmissions(this.IdTrasmissioneSingoleSelezionate, this.txt_NoteAccettazione.Text))
                {
                    string msg = "ErrorMassiveTransmissionAccept";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                    return;
                }
                else
                {
                    this.GridTrasmissioniPendenti_Bind();
                    this.IdTrasmissioneSingoleSelezionate = null;
                    this.CheckAllTransmission = false;
                    this.txt_NoteAccettazione.Text = string.Empty;
                    this.UpGridTrasmissioniPendenti.Update();
                    this.UpNoteAccettazione.Update();
                    //Se non ci sono più trasmissioni da accettare chiudo il popup
                    if(IdTrasmissioneSingole == null || IdTrasmissioneSingole.Count == 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('MassiveTransmissionAccept','up');", true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                string msg = "ErrorMassiveTransmissionAccept";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        private void SetCheckBox()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.HiddenItemsChecked.Value))
                {
                    //salvo i check spuntati alla pagina cliccata in precedenza
                    string[] items = new string[1] { this.HiddenItemsChecked.Value };
                    if (this.HiddenItemsChecked.Value.IndexOf(",") > 0)
                        items = this.HiddenItemsChecked.Value.Split(',');
                    if (IdTrasmissioneSingoleSelezionate == null)
                        IdTrasmissioneSingoleSelezionate = new List<string>();
                    foreach (string item in items)
                    {
                        string key = item.Split('_')[0];
                        if (!this.IdTrasmissioneSingoleSelezionate.Contains(key))
                        {
                            this.IdTrasmissioneSingoleSelezionate.Add(key);
                        }
                    }
                }


                if (!string.IsNullOrEmpty(this.HiddenItemsUnchecked.Value))
                {
                    this.CheckAllTransmission = false;

                    // salvo i check non spuntati alla pagina cliccata in precedenza
                    string[] items = new string[1] { this.HiddenItemsUnchecked.Value };
                    if (this.HiddenItemsUnchecked.Value.IndexOf(",") > 0)
                        items = this.HiddenItemsUnchecked.Value.Split(',');

                    foreach (string item in items)
                    {
                        string key = item.Split('_')[0];
                        if (this.IdTrasmissioneSingoleSelezionate.Contains(key))
                            this.IdTrasmissioneSingoleSelezionate.Remove(key);
                    }
                }
                this.HiddenItemsChecked.Value = string.Empty;
                this.HiddenItemsUnchecked.Value = string.Empty;
                this.UpPnlButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #region GridView


        private List<InfoTrasmissione> LoadInfoTrasmissioni()
        {
            string idDocOrFasc = string.Empty;
            string docOrFasc = string.Empty;
            if (this.Request.QueryString["type"].Equals("d"))
            {
                docOrFasc = "D";
                idDocOrFasc = DocumentManager.getSelectedRecord().docNumber;
            }
            else
            {
                docOrFasc = "F";
                idDocOrFasc = UIManager.ProjectManager.getProjectInSession().systemID;
            }
            List<InfoTrasmissione> listInfoTrasmissioni = new List<InfoTrasmissione>();
            DocsPaWR.SearchPagingContext pagingContext = new DocsPaWR.SearchPagingContext();
            pagingContext.Page = this.SelectedPage;
            pagingContext.PageSize = this.GridTrasmissioniPendenti.PageSize;
            string[] idTrasmSingole = null;
            listInfoTrasmissioni = TrasmManager.GetTrasmissioniPendentiConWorkflow(idDocOrFasc, docOrFasc, RoleManager.GetRoleInSession().systemId, UserManager.GetUserInSession().idPeople, out idTrasmSingole, ref pagingContext);
            this.IdTrasmissioneSingole = idTrasmSingole.ToList();
            this.PageCount = pagingContext.PageCount;
            return listInfoTrasmissioni;
        }

        private void GridTrasmissioniPendenti_Bind()
        {
            List<InfoTrasmissione> listInfoTrasmissioni = LoadInfoTrasmissioni();
            this.GridTrasmissioniPendenti.DataSource = listInfoTrasmissioni;
            this.GridTrasmissioniPendenti.DataBind();
            BuildGridNavigator();
        }

        protected void GridTrasmissioniPendenti_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    InfoTrasmissione infoTrasmissione = e.Row.DataItem as InfoTrasmissione;
                    CheckBox checkBox = e.Row.FindControl("cbxSel") as CheckBox;
                    if (checkBox != null)
                    {
                        checkBox.Attributes["onclick"] = "SetItemCheck(this, '" + infoTrasmissione.idTrasmSingola + "')";
                        if (this.IdTrasmissioneSingoleSelezionate != null && this.IdTrasmissioneSingoleSelezionate.Contains(infoTrasmissione.idTrasmSingola))
                            checkBox.Checked = true;
                        else
                            checkBox.Checked = false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void GridTrasmissioniPendenti_PreRender(object sender, EventArgs e)
        {
            try
            {
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void GridTrasmissioniPendenti_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void GridTrasmissioniPendenti_ItemCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chkBxHeader = (CheckBox)this.GridTrasmissioniPendenti.HeaderRow.FindControl("cbxSelAll");
                    if (chkBxHeader != null)
                    {
                        chkBxHeader.Checked = this.CheckAllTransmission;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected string GetSenderName(string name, string delegateName)
        {
            if (string.IsNullOrEmpty(delegateName))
                return name;
            else
                return delegateName + " (" + Utils.Languages.GetLabelFromCode("DocumentNoteAuthorDelegatedBy", UserManager.GetUserLanguage()) + " " + name + ")";
        }

        private void BuildGridNavigator()
        {
            try
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
                        btn.Attributes["onclick"] = "disallowOp('Content2'); $('#grid_pageindex').val(" + (startFrom - 1) + "); __doPostBack('upPnlGridIndexes', ''); return false;";
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
                            btn.Attributes["onclick"] = "disallowOp('Content2'); $('#grid_pageindex').val($(this).text()); __doPostBack('upPnlGridIndexes', ''); return false;";
                            panel.Controls.Add(btn);
                        }
                    }

                    if (endTo < this.PageCount)
                    {
                        LinkButton btn = new LinkButton();
                        btn.EnableViewState = true;
                        btn.Text = "...";
                        btn.Attributes["onclick"] = "disallowOp('Content2'); $('#grid_pageindex').val(" + endTo + "); __doPostBack('upPnlGridIndexes', ''); return false;";
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

        protected void cbxSelAll_CheckedChanged(object sender, EventArgs e)
        {
            this.IdTrasmissioneSingoleSelezionate = new List<string>();
            CheckBox cbxSelAll = (CheckBox)GridTrasmissioniPendenti.HeaderRow.FindControl("cbxSelAll");
            foreach (GridViewRow row in GridTrasmissioniPendenti.Rows)
            {
                CheckBox cbxSel = (CheckBox)row.FindControl("cbxSel");
                if (cbxSel.Enabled)
                    cbxSel.Checked = cbxSelAll.Checked;
            }
            if (cbxSelAll.Checked)
            {
                foreach (string id in IdTrasmissioneSingole)
                {
                    this.IdTrasmissioneSingoleSelezionate.Add(id);
                }
            }
            this.CheckAllTransmission = cbxSelAll.Checked;
            BuildGridNavigator();
            this.upPnlGridIndexes.Update();
        }
        #endregion
    }
}