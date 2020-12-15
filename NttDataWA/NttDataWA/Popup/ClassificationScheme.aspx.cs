using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Web.UI.WebControls;
using NttDatalLibrary;
using log4net;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class ClassificationScheme : System.Web.UI.Page
    {
        private ILog logger = LogManager.GetLogger(typeof(ObjectManager));



        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializePage();

                    //if (Session["sottofascicolo"] != null)
                    //{
                    //    OpenTree();
                    //    Session["sottofascicolo"] = null;
                    //}
                }
                else
                {

                    if (!string.IsNullOrEmpty(grid_rowindex.Value) &&
                        int.Parse(grid_rowindex.Value) > -1)
                    {
                        GridSearchClassificationScheme_SelectedIndexChanging(new object(), new GridViewSelectEventArgs(int.Parse(grid_rowindex.Value)));
                    }
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
                    this.ClassificationSchemaBtnSearch.Focus();
                    this.UpPnlButtons.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "functionKeyPress", "<script>$(function() {$('.defaultAction').keypress(function(e) {if(e.which == 13) {e.preventDefault();$('#ClassificationSchemaBtnSearch').click();}});});</script>", false);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        //protected void OpenTree()
        //{
            //object Fascicolo = Session["sottofascicolo"];
            
            //DocsPaWR.Utente user = UIManager.UserManager.GetUserInSession();
            //DocsPaWR.Ruolo role = UIManager.RoleManager.GetRoleInSession();
            //DocsPaWR.OrgTitolario titolario = UIManager.ClassificationSchemeManager.GetTitolarioInSession();
            //string idregistro = UIManager.RegistryManager.GetRegistryInSession().systemId;
            //if (ddlRegistri.Visible)
            //    idregistro = ddlRegistri.SelectedValue;
            //TreeTitolario = UIManager.ClassificationSchemeManager.LoadTreeViewRicerca(TreeTitolario, user.idAmministrazione,
            // (GridSearchClassificationScheme.Rows[e.NewSelectedIndex].FindControl("lblId") as Label).Text,
            // (GridSearchClassificationScheme.Rows[e.NewSelectedIndex].FindControl("lblIdparent") as Label).Text,
            // int.Parse((GridSearchClassificationScheme.Rows[e.NewSelectedIndex].FindControl("lblLivello") as Label).Text),
            // role.idGruppo, idregistro, titolario.ID,
            // (GridSearchClassificationScheme.Rows[e.NewSelectedIndex].FindControl("lblDescrizione") as Label).Text);
            //grid_rowindex.Value = "-1";
            //ClassificationSchemaBtnOk.Enabled = true;
            //user = null;
            //role = null;
            //titolario = null;
        //}
        protected void InitializePage()
        {

            this.InitializeLabel();
            this.InitializeObjectValue();
            if (!UIManager.AdministrationManager.isEnableIndiceSistematico())
            {
                this.CustomImageIndiceSistematico.Visible = false;
            }
        }

        protected void InitializeObjectValue()
        {
            divIndiceSistematico.Visible = UIManager.ClassificationSchemeManager.isEnableIndiceSistematico();
            CustomImageIndiceSistematico.Visible = divIndiceSistematico.Visible;
            if (HttpContext.Current.Session["typeDoc"] != null && UIManager.DocumentManager.getSelectedRecord()!=null)
            {
                if ((!string.IsNullOrEmpty(HttpContext.Current.Session["typeDoc"].ToString()) &&
                HttpContext.Current.Session["typeDoc"].Equals("n"))
                ||
                (UIManager.DocumentManager.getSelectedRecord().tipoProto != null &&
                    UIManager.DocumentManager.getSelectedRecord().tipoProto.ToUpper().Equals("G"))
                )
                {
                    this.ddlRegistri.Visible = true;
                    this.ClassificationSchemelabelRegistro.Visible = true;

                }

                if (HttpContext.Current.Session["from"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["from"].ToString()) && HttpContext.Current.Session["from"].ToString().Equals("search"))
                {
                    this.ddlRegistri.Visible = true;
                    this.ClassificationSchemelabelRegistro.Visible = true;
                }


            }

            DocsPaWR.Utente user = UIManager.UserManager.GetUserInSession();
            DocsPaWR.Ruolo role = UIManager.RoleManager.GetRoleInSession();
            DocsPaWR.OrgTitolario titolario = UIManager.ClassificationSchemeManager.GetTitolarioInSession();
            if (titolario == null)
            {
                titolario = UIManager.ClassificationSchemeManager.getTitolarioAttivo(user.idAmministrazione);
                UIManager.ClassificationSchemeManager.SetTitolarioInSession(titolario);
            }
            DocsPaWR.Registro registro = UIManager.RegistryManager.GetRegistryInSession();
            if (registro == null)
            {
                registro = UIManager.RoleManager.GetRoleInSession().registri[0];
            }
            if (ddlRegistri.Visible)
            {
                DocsPaWR.Registro[] reg = UIManager.RegistryManager.GetRegistriesByRole(role.systemId);
                foreach (DocsPaWR.Registro r in reg)
                {
                    ListItem i = new ListItem();
                    i.Text = r.codRegistro;
                    i.Value = r.systemId;
                    if (registro.systemId == r.systemId)
                    {
                        //UIManager.RegistryManager.SetRegistryInSession(r);
                        i.Selected = true;
                    }
                    if (!r.flag_pregresso)
                    {
                        ddlRegistri.Items.Add(i);
                    }
                }
                reg = null;

            }


            string IdRegistro = registro.systemId;
            if (ddlRegistri.Visible)
                IdRegistro = ddlRegistri.SelectedValue;

            CaricamentoTitolario(user.idAmministrazione, "0", role.idGruppo, IdRegistro, titolario.ID);

            user = null;
            role = null;
            titolario = null;
            IdRegistro = null;
            registro = null;
            GridSearchClassificationScheme.Visible = false;
            divRisultati.Visible = false;
        }

        protected void InitializeLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.ClassificationSchemaBtnSearch.Text = Utils.Languages.GetLabelFromCode("ClassificationSchemaSearch", language);
            this.ClassificationSchemaBtnOk.Text = Utils.Languages.GetLabelFromCode("ClassificationSchemaOk", language);
            this.ClassificationSchemaBtnClose.Text = Utils.Languages.GetLabelFromCode("ClassificationSchemaClose", language);
            this.ClassificationSchemelabelCodice.Text = Utils.Languages.GetLabelFromCode("ClassificationSchemelabelCodice", language);
            this.ClassificationSchemelabelDescrizione.Text = Utils.Languages.GetLabelFromCode("ClassificationSchemelabelDescrizione", language);
            this.ClassificationSchemelabelIndiceSistematico.Text = Utils.Languages.GetLabelFromCode("ClassificationSchemelabelIndiceSistematico", language);
            this.ClassificationSchemelabelNote.Text = Utils.Languages.GetLabelFromCode("ClassificationSchemelabelNote", language);
            this.ClassificationSchemelabelRegistro.Text = Utils.Languages.GetLabelFromCode("ClassificationSchemeRegistro", language);
            this.CustomImageIndiceSistematico.ToolTip = Utils.Languages.GetLabelFromCode("ClassificationSchemelabelIndiceSistematico", language);
            this.CustomImageIndiceSistematico.ToolTip = Utils.Languages.GetLabelFromCode("CustomImageIndiceSistematico", language);
            this.ClassificationSchemaBtnSearch.Focus();
        }

        protected void ClassificationSchemaBtnOk_Click(object sender, EventArgs e)
        {
            //try {
                string returnValue = String.Empty;
                if (TreeTitolario.SelectedNode != null)
                {
                    returnValue = ((myTreeNode)TreeTitolario.SelectedNode).CODICE + "#" + ((myTreeNode)TreeTitolario.SelectedNode).DESCRIZIONE;
                    HttpContext.Current.Session["ReturnValuePopup"] = returnValue;
                }

                closePage("up");
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        protected void ClassificationSchemaBtnClose_Click(object sender, EventArgs e)
        {
            //try {
                closePage(string.Empty);
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        private void closePage(string _ParametroDiRitorno)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["massive"]))
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('OpenTitolarioMassive','" + _ParametroDiRitorno + "');", true);
            else
            {
                if (!string.IsNullOrEmpty(Request.QueryString["popup"]))
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('OpenTitolario','" + _ParametroDiRitorno + "', parent);", true);
                else
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('OpenTitolario','" + _ParametroDiRitorno + "');", true);
            }
            
        }


        protected void ExpandeTreeView(object sender, TreeNodeEventArgs e)
        {
            try
            {
                string idregistro = UIManager.RegistryManager.GetRegistryInSession().systemId;
                if (ddlRegistri.Visible)
                    idregistro = ddlRegistri.SelectedValue;
                string idAmministrazione = UIManager.UserManager.GetUserInSession().idAmministrazione;
                string idgruppo = UIManager.RoleManager.GetRoleInSession().idGruppo;
                string idTitolario = UIManager.ClassificationSchemeManager.GetTitolarioInSession().ID;
                e.Node.ChildNodes.Clear();
                UIManager.ClassificationSchemeManager.LoadTreeViewChild(e.Node, idregistro, idAmministrazione, idgruppo, idTitolario);

                idregistro = null;
                idAmministrazione = null;
                idgruppo = null;
                idTitolario = null;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }


        }

        protected void ClassificationSchemaBtnSearch_Click(object sender, EventArgs e)
        {
            //try
            //{
                GridSearchClassificationScheme.SelectedIndex = -1;
                if (ricerca())
                {
                    GridSearchClassificationScheme.Visible = true;
                    divRisultati.Visible = true;
                }
                else
                {
                    string msg ="ErrorClassificationSchemeNoSearch";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'info', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'info', '');}", true);
                    this.UpPnlButtons.Update();
                }
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        protected void gridSearchObject_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try {
                grid_rowindex.Value = "-1";
                GridSearchClassificationScheme.SelectedIndex = -1;
                GridSearchClassificationScheme.PageIndex = e.NewPageIndex;
                ricerca();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        private bool ricerca()
        {
            bool ricercaOk = true;
            DocsPaWR.Utente user = UIManager.UserManager.GetUserInSession();
            DocsPaWR.Ruolo role = UIManager.RoleManager.GetRoleInSession();
            DocsPaWR.OrgTitolario titolario = UIManager.ClassificationSchemeManager.GetTitolarioInSession();
            DocsPaWR.Registro registro = UIManager.RegistryManager.GetRegistryInSession();
            List<UIManager.RisultatiRicercaTitolario> result = null;
            if (string.IsNullOrEmpty(ClassificationSchemeTxtCodice.Text) &&
                string.IsNullOrEmpty(ClassificationSchemeTxtDescrizione.Text) &&
                string.IsNullOrEmpty(ClassificationSchemeTxtNote.Text) &&
                string.IsNullOrEmpty(ClassificationSchemeTxtIndiceSistematico.Text))
            {
                GridSearchClassificationScheme.DataSource = new List<UIManager.RisultatiRicercaTitolario>();
                GridSearchClassificationScheme.DataBind();
                ricercaOk = false;
            }
            else
            {
                result = UIManager.ClassificationSchemeManager.getRicercaTitolario(
                    ClassificationSchemeTxtCodice.Text,
                    ClassificationSchemeTxtDescrizione.Text,
                    ClassificationSchemeTxtNote.Text,
                    ClassificationSchemeTxtIndiceSistematico.Text,
                    user.idAmministrazione,
                    role.idGruppo,
                    registro.systemId,
                    titolario.ID);
                GridSearchClassificationScheme.DataSource = result;
                GridSearchClassificationScheme.DataBind();
                if (result.Count == 0)
                    ricercaOk = false;

            }

            user = null;
            role = null;
            titolario = null;
            registro = null;

            return ricercaOk;
        }

        protected void TreeTitolario_SelectedNodeChanged(object sender, EventArgs e)
        {
            try {
                if (TreeTitolario != null && TreeTitolario.SelectedNode != null && ((myTreeNode)TreeTitolario.SelectedNode).CODICE == "T")
                {
                    ClassificationSchemaBtnOk.Enabled = false;
                }
                else
                {
                    ClassificationSchemaBtnOk.Enabled = true;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        

        protected void GridSearchClassificationScheme_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            try {
                GridSearchClassificationScheme.SelectedIndex = e.NewSelectedIndex;
                DocsPaWR.Utente user = UIManager.UserManager.GetUserInSession();
                DocsPaWR.Ruolo role = UIManager.RoleManager.GetRoleInSession();
                DocsPaWR.OrgTitolario titolario = UIManager.ClassificationSchemeManager.GetTitolarioInSession();
                string idregistro = UIManager.RegistryManager.GetRegistryInSession().systemId;
                if (ddlRegistri.Visible)
                    idregistro = ddlRegistri.SelectedValue;
                TreeTitolario = UIManager.ClassificationSchemeManager.LoadTreeViewRicerca(TreeTitolario, user.idAmministrazione,
                 (GridSearchClassificationScheme.Rows[e.NewSelectedIndex].FindControl("lblId") as Label).Text,
                 (GridSearchClassificationScheme.Rows[e.NewSelectedIndex].FindControl("lblIdparent") as Label).Text,
                 int.Parse((GridSearchClassificationScheme.Rows[e.NewSelectedIndex].FindControl("lblLivello") as Label).Text),
                 role.idGruppo, idregistro, titolario.ID,
                 (GridSearchClassificationScheme.Rows[e.NewSelectedIndex].FindControl("lblDescrizione") as Label).Text);
                grid_rowindex.Value = "-1";
                ClassificationSchemaBtnOk.Enabled = true;
                user = null;
                role = null;
                titolario = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GridSearchClassificationScheme_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes["onclick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "'); __doPostBack('UpdatePanelGridSearchClassificaction', ''); return false;";
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddlRegistri_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                DocsPaWR.Utente user = UIManager.UserManager.GetUserInSession();
                DocsPaWR.Ruolo role = UIManager.RoleManager.GetRoleInSession();
                DocsPaWR.OrgTitolario titolario = UIManager.ClassificationSchemeManager.GetTitolarioInSession();
                string IdRegistro = UIManager.RegistryManager.GetRegistryInSession().systemId;
                if (ddlRegistri.Visible)
                    IdRegistro = ddlRegistri.SelectedValue;
                UIManager.RegistryManager.GetRegistryInSession();
                CaricamentoTitolario(user.idAmministrazione, "0", role.idGruppo, IdRegistro, titolario.ID);

                user = null;
                role = null;
                titolario = null;
                IdRegistro = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        private void CaricamentoTitolario(string _IdAmministrazione, string _IdParent, string _IdGruppo, string _IdRegistro, string _IdTitolario)
        {

            TreeTitolario = UIManager.ClassificationSchemeManager.loadTreeView(TreeTitolario, _IdAmministrazione, _IdParent, _IdGruppo, _IdRegistro, _IdTitolario);

        }

        protected void CustomImageIndiceSistematico_Click(object sender, ImageClickEventArgs e)
        {
            DocsPaWR.FileDocumento filedoc = UIManager.ClassificationSchemeManager.getIndiceSistematico(UIManager.ClassificationSchemeManager.GetTitolarioInSession().ID);
            if (filedoc != null && filedoc.content.Length > 0)
            {

                Response.ContentType = filedoc.contentType;
                Response.AddHeader("content-disposition", "inline;filename=" + filedoc.fullName);
                Response.AddHeader("content-lenght", filedoc.content.Length.ToString());
                Response.BinaryWrite(filedoc.content);

                Response.Write("<html><body><script type=\"text/javascript\">OpenFile();</script></body></html>");
                Response.End();
            }
        }


    }
}