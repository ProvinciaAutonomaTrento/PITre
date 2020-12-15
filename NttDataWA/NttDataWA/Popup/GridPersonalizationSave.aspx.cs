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
    public partial class GridPersonalizationSave : System.Web.UI.Page
    {

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

        private static Grid SelectedGrid
        {
            get
            {

                return (Grid)HttpContext.Current.Session["SelectedGrid"];

            }

            set
            {
                HttpContext.Current.Session["SelectedGrid"] = value;
            }
        }

        private string gridIdPreferred
        {
            get
            {
                return HttpContext.Current.Session["gridIdPreferred"] as string;
            }
            set
            {
                HttpContext.Current.Session["gridIdPreferred"] = value;
            }
        }

        private GridBaseInfo[] listGrid
        {
            get
            {
                return HttpContext.Current.Session["listGrid"] as GridBaseInfo[];
            }
            set
            {
                HttpContext.Current.Session["listGrid"] = value;
            }
        }



        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializateLabel();
                    this.InitializateValue();
                }

                this.RefreshScripts();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitializateValue()
        {
            this.listGrid = null;
            this.gridIdPreferred = string.Empty;
            this.ddl_ric_griglie.Items.Clear();
            InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
            if (SelectedGrid != null)
            {
                this.listGrid = GridManager.GetGridsBaseInfo(infoUtente, SelectedGrid.GridType, true);
            }
            else
            {
                this.listGrid = GridManager.GetGridsBaseInfo(infoUtente, GridTypeEnumeration.Document, true);
            }

            if (this.listGrid != null && this.listGrid.Length > 0)
            {
                int count = 0;
                foreach (GridBaseInfo gb in this.listGrid)
                {
                    if (!gb.IsSearchGrid && !gb.GridId.Equals("-1"))
                    {
                        ListItem it = new ListItem(gb.GridName, gb.GridId);
                        this.ddl_ric_griglie.Items.Add(it);
                        if (gb.IsPreferred)
                        {
                            this.gridIdPreferred = gb.GridId;
                        }
                        count++;
                    }
                }
                if (count > 0)
                {
                    this.pnl_scelta.Visible = false;
                    this.nuova_g.Visible = true;
                }
                else
                {
                    //Ho solo la griglia standard quindi non posso modificare nulla devo per forza crearne una nuova
                    this.pnl_scelta.Visible = false;
                    this.nuova_g.Visible = true;
                    this.rbl_save.Items[0].Selected = true;
                    this.rbl_save.Items[1].Enabled = false;
                }
            }
        }

        private void InitializateLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            rblVisibilita.Items[0].Text = Utils.Languages.GetLabelFromCode("GridPersonalizationSaveUtente", language);
            rblVisibilita.Items[1].Text = Utils.Languages.GetLabelFromCode("GridPersonalizationSaveRuolo", language);
            GridPersonalizationSaveBtnInserisci.Text = Utils.Languages.GetLabelFromCode("GridPersonalizationSaveSalva", language);
            GridPersonalizationSaveBtnClose.Text = Utils.Languages.GetLabelFromCode("GridPersonalizationSaveChiudi", language);
            l_GridName.Text = Utils.Languages.GetLabelFromCode("GridPersonalizationSaveAlterNameGrid", language);
            l_GridNew.Text = Utils.Languages.GetLabelFromCode("GridPersonalizationSaveNewNameGrid", language);
            l_GridModify.Text = Utils.Languages.GetLabelFromCode("GridPersonalizationSaveChooseGrid", language);
            rbl_save.Items[0].Text = Utils.Languages.GetLabelFromCode("GridPersonalizationSaveNewGrid", language);
            rbl_save.Items[0].Text=rbl_save.Items[0].Text.Replace("@user@", UserManager.GetUserInSession().descrizione);
            rbl_save.Items[1].Text = Utils.Languages.GetLabelFromCode("GridPersonalizationSaveAlterGrid", language).Replace("@role@", RoleManager.GetRoleInSession().descrizione);
            rbl_save.Items[1].Text = rbl_save.Items[1].Text.Replace("@role@", RoleManager.GetRoleInSession().descrizione);
            chk_pref.Text = Utils.Languages.GetLabelFromCode("GridPersonalizationSaveGrigliaPredefinita", language);
            l_RendiDisponibile.Text = Utils.Languages.GetLabelFromCode("GridPersonalizationSaveVIsibile", language);
            l_actions.Text = Utils.Languages.GetLabelFromCode("GridPersonalizationActionsAvailable", language);
        }

        private void RefreshScripts()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
        }

        private void removeSession()
        {
            HttpContext.Current.Session.Remove("listGrid");
            HttpContext.Current.Session.Remove("gridIdPreferred");
        }

        protected void GridPersonalizationSaveBtnClose_Click(object sender, EventArgs e)
        {
            try {
                removeSession();
                close(string.Empty);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void close(string parametroChiusura)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('GrigliaPersonalizzataSave', '" + parametroChiusura + "');} else {parent.closeAjaxModal('GrigliaPersonalizzataSave', '" + parametroChiusura + "');};", true);
        }

        protected void GridPersonalizationSaveBtnInserisci_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                InfoUtente infoUtente = UserManager.GetInfoUser();
                Grid TemporaryGrid = GridManager.CloneGrid(SelectedGrid);

                string nuovoNome = string.Empty;
                string visibility = this.rblVisibilita.SelectedItem.Value;

                //Nuova griglia
                if (this.rbl_save.SelectedItem.Value.Equals("new"))
                {
                    if (string.IsNullOrEmpty(txt_titolo.Text))
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('GridPersonalizationWarningNameGrid', 'warning', '');} else {parent.ajaxDialogModal('GridPersonalizationWarningNameGrid', 'warning', '');}", true);
                    else
                    {
                        TemporaryGrid.GridName = txt_titolo.Text;
                        nuovoNome = txt_titolo.Text;
                        GridManager.SaveNewGrid(TemporaryGrid, infoUtente, nuovoNome, visibility, this.chk_pref.Checked);
                        SelectedGrid = TemporaryGrid;
                        close("up");
                    }
                }
                //Modifica griglia
                else
                {
                    TemporaryGrid.GridId = this.ddl_ric_griglie.SelectedValue;
                    TemporaryGrid.GridName = this.ddl_ric_griglie.SelectedItem.Text;

                    if (string.IsNullOrEmpty(txt_name_mod.Text))
                         ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + Utils.Languages.GetLabelFromCode("GridPersonalizationWarningNameGrid", UIManager.UserManager.GetUserLanguage()).Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + Utils.Languages.GetLabelFromCode("GridPersonalizationWarningNameGrid", UIManager.UserManager.GetUserLanguage()).Replace("'", @"\'") + "', 'error', '');}", true);
                    else
                    {
                        TemporaryGrid.GridName = txt_name_mod.Text;
                        GridManager.ModifyGrid(TemporaryGrid, infoUtente, visibility, this.chk_pref.Checked);

                        if (this.chk_pref.Checked || (!string.IsNullOrEmpty(this.gridIdPreferred) && this.gridIdPreferred.Equals(TemporaryGrid.GridId)))
                            SelectedGrid = TemporaryGrid;
                        close("up");
                    }


                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ChangeSelectedGrid(object sender, EventArgs e)
        {
            try {
                List<GridBaseInfo> tempList = new List<GridBaseInfo>(this.listGrid);
                GridBaseInfo tempGrid = (GridBaseInfo)tempList.Where(g => g.GridId.Equals(this.ddl_ric_griglie.SelectedValue)).FirstOrDefault();
                if (tempGrid.UserGrid)
                {
                    this.rblVisibilita.Items[0].Selected = true;
                    this.rblVisibilita.Items[1].Selected = false;
                }
                else
                {
                    this.rblVisibilita.Items[0].Selected = false;
                    this.rblVisibilita.Items[1].Selected = true;
                
                    //this.bDeleteExtender.Enabled = true;
                }
                this.modify_g.Visible = true;
                this.txt_name_mod.Text = tempGrid.GridName;
                this.upChangeGridName.Update();
                this.pnl_visibility.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ChangeMode(object sender, EventArgs e)
        {
            try {
                RadioButtonList obj = (RadioButtonList)sender;
                this.rblVisibilita.Items[0].Selected = true;
                this.rblVisibilita.Items[1].Selected = false;
                this.pnl_scelta.Visible = false;
                this.nuova_g.Visible = true;
                this.modify_g.Visible = false;
                if (obj.SelectedItem.Value.Equals("mod"))
                {
                    this.pnl_scelta.Visible = true;
                    this.nuova_g.Visible = false;
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    this.ddl_ric_griglie.Items.Clear();
                    GridBaseInfo tempGrid = null;

                    if (this.listGrid != null)
                    {
                        foreach (GridBaseInfo gb in this.listGrid)
                        {
                            if (!gb.IsSearchGrid && !gb.GridId.Equals("-1"))
                            {
                                ListItem it = new ListItem(gb.GridName, gb.GridId);
                                this.ddl_ric_griglie.Items.Add(it);
                                if (gb.IsPreferred)
                                {
                                    tempGrid = gb;
                                    this.ddl_ric_griglie.SelectedValue = gb.GridId;
                                }
                            }
                        }
                    }
                    if (tempGrid != null && !tempGrid.UserGrid)
                        {
                            //this.bDeleteExtender.Enabled = true;
                        }
                    this.modify_g.Visible = true;
                    this.txt_name_mod.Text = this.ddl_ric_griglie.SelectedItem.Text;
                    this.pnl_visibility.Update();
                }
                this.upChangeGridName.Update();
                this.upChangeGrid.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

    }
}