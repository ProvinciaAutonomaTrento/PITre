using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class GridPersonalizationPreferred : System.Web.UI.Page
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

        private GridBaseInfo[] ListGrid
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

        private GridBaseInfo GridDelete
        {
            get
            {

                return (GridBaseInfo)HttpContext.Current.Session["GridDelete"];

            }

            set
            {
                HttpContext.Current.Session["GridDelete"] = value;
            }
        }



        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    InitializePage();
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.HiddenConfirmDelete.Value) && this.HiddenConfirmDelete.Value.Equals("true"))
                    {
                        this.HiddenConfirmDelete.Value = string.Empty;
                        if (GridManager.RemoveGrid(this.GridDelete, UserManager.GetInfoUser()))
                        {

                            if (this.GridDelete.IsPreferred)
                            {
                                SelectedGrid = GridManager.getUserGrid(SelectedGrid.GridType, UserManager.GetInfoUser(), RoleManager.GetRoleInSession());
                            }

                            this.ListGrid = GridManager.GetGridsBaseInfo(UserManager.GetInfoUser(), SelectedGrid.GridType, true);

                            this.GridPreferred.DataSource = this.ListGrid;
                            this.GridPreferred.DataBind();
                            this.UpnlGrid.Update();
                        }
                        this.UpHiddenFields.Update();
                    }

                    this.ReApplyScripts();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ReApplyScripts()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        private void InitializePage()
        {
            InitializeLabel();
            InitializeValue();
        }

        private void InitializeLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            GridPersonalizationPreferredBtnClose.Text= Utils.Languages.GetLabelFromCode("GridPersonalizationPreferredClose", language);
            GridPersonalizationPreferredBtnInserisci.Text = Utils.Languages.GetLabelFromCode("GridPersonalizationPreferredSave", language);
            }

        private void InitializeValue()
        {
            this.ListGrid = null;
            this.GridDelete = null;
            this.GetInitData();
            string result = string.Empty;
        }

        private void RemoveSession()
        {
            HttpContext.Current.Session.Remove("listGrid");

        }
        protected void GridPersonalizationPreferredBtnClose_Click(object sender, EventArgs e)
        {
            try {
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
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('GridPersonalizationPreferred', '" + parametroChiusura + "');} else {parent.closeAjaxModal('GridPersonalizationPreferred', '" + parametroChiusura + "');};", true);
        }
        protected void GridPersonalizationPreferredBtnInserisci_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
          
                InfoUtente infoUtente = UserManager.GetInfoUser();
                Ruolo ruolo = RoleManager.GetRoleInSession();
                string gridId = "-1";
            
                foreach(GridViewRow row in GridPreferred.Rows)
                    if (((RadioButton)row.FindControl("rbSelect")).Checked)
                    {
                        gridId = ((Label)row.FindControl("SYSTEM_ID")).Text;
                        if (gridId.Equals("-1"))
                             GridManager.RemovePreferredTypeGrid(infoUtente, SelectedGrid.GridType);
                        else
                           GridManager.AddPreferredGrid(gridId, infoUtente, SelectedGrid.GridType);
                        break;
                    }
                SelectedGrid = GridManager.getUserGrid(SelectedGrid.GridType, infoUtente, ruolo);
                close("up");
                //GridPersonalizationPreferredBtnClose_Click(new object(), new EventArgs());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

     
        protected void GetInitData()
        {
            InfoUtente infoUtente = UserManager.GetInfoUser();
            string language = UIManager.UserManager.GetUserLanguage();
            if (SelectedGrid != null)
            {
                this.ListGrid = GridManager.GetGridsBaseInfo(infoUtente, SelectedGrid.GridType, true);
            }
            else
            {
                this.ListGrid = GridManager.GetGridsBaseInfo(infoUtente, GridTypeEnumeration.Document, true);
            }
            // FIX GRIGLIA STANDARD
            if (language.Trim().ToUpper() != "ITALIAN")
            {
                if (this.ListGrid != null)
                {
                    this.ListGrid.Where(g => g.GridId == "-1").FirstOrDefault().GridName = Utils.Languages.GetLabelFromCode("projectLitGrigliaStandard", language);
                }
            }
            this.GridPreferred.DataSource = this.ListGrid;
            this.GridPreferred.DataBind();                        
            GridPreferred.HeaderRow.Cells[0].Text = Utils.Languages.GetLabelFromCode("GridPreferredHeader0", language);
            GridPreferred.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("GridPreferredHeader1", language);
            GridPreferred.HeaderRow.Cells[2].Text = Utils.Languages.GetLabelFromCode("GridPreferredHeader2", language);
            GridPreferred.HeaderRow.Cells[3].Text = Utils.Languages.GetLabelFromCode("GridPreferredHeader3", language);

        }

        protected string GetTooltipRemove()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode("DocumentBtnRemove", language);
        }

        protected String GetGridID(GridBaseInfo temp)
        {
            return temp.GridId;
        }

      
        protected bool GetGridPreferred(GridBaseInfo temp)
        {
            return temp.IsPreferred;
        }


        protected bool GetRoleGridID(GridBaseInfo temp)
        {
            bool result = false;
            if (temp.RoleGrid)
            {
                result = true;
            }
            return result;
        }

        protected String GetGridName(GridBaseInfo temp)
        {
            return temp.GridName;
        }

        protected String GetImageUserGridID(GridBaseInfo temp)
        {
            if (temp.UserGrid)
            {
                return "../Images/Icons/flag_ok.png";
            }
            else
            {
                return "../Images/Icons/no_check_grid.gif";
            }
        }

        protected String GetImageRoleGridID(GridBaseInfo temp)
        {
            if (temp.RoleGrid)
            {
                return "../Images/Icons/flag_ok.png";
            }
            else
            {
                return "../Images/Icons/no_check_grid.gif";
            }
        }

        protected bool GetDeleteGridID(GridBaseInfo temp)
        {
            bool result = true;
            if (temp.GridId.Equals("-1"))
            {
                result = false;
            }
            return result;
        }

     
        protected void DeleteGrid(object sender, EventArgs e)
        {
            try {
                this.GridDelete = null;
                InfoUtente infoutente = UserManager.GetInfoUser();
                Ruolo ruolo = RoleManager.GetRoleInSession();

                string clientID = ((CustomImageButton)sender).ClientID;
                clientID = clientID.Split('_').Last();

                string id = ((Label)GridPreferred.Rows[int.Parse(clientID)].FindControl("SYSTEM_ID")).Text;

                GridBaseInfo tempBase = new GridBaseInfo();
                foreach (GridBaseInfo gri in this.ListGrid)
                {
                    if (gri.GridId.Equals(id))
                    {
                        tempBase = gri;
                        break;
                    }
                }

                if (!tempBase.RoleGrid)
                {
                    if (GridManager.RemoveGrid(tempBase, infoutente))
                    {

                        if (tempBase.IsPreferred)
                        {
                            SelectedGrid = GridManager.getUserGrid(SelectedGrid.GridType, infoutente, ruolo);
                        }

                        this.ListGrid = GridManager.GetGridsBaseInfo(infoutente, SelectedGrid.GridType, true);

                        this.GridPreferred.DataSource = this.ListGrid;
                        //this.GridPreferred.CurrentPageIndex = 0;
                        this.GridPreferred.DataBind();
                        this.UpnlGrid.Update();
                    }
                }
                else
                {
                    string msg = "ConfirmDeleteRoleGrid";
                    string language = UIManager.UserManager.GetUserLanguage();
                    string msgTitleCnfirm = Utils.Languages.GetLabelFromCode("msgTitleCnfirm", language);
                    this.GridDelete = tempBase;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'HiddenConfirmDelete', '" + msgTitleCnfirm + "');", true);
                    this.UpHiddenFields.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GridPreferred_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try {
                this.GridPreferred.PageIndex = e.NewPageIndex;
                this.GridPreferred.DataSource = this.ListGrid;
                this.GridPreferred.DataBind();
                this.UpnlGrid.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GridPreferred_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try {
                if (e.Row.RowType == DataControlRowType.DataRow &&
              (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate))
                {
                    RadioButton rbSelect = (RadioButton)e.Row.FindControl("rbSelect");

                    rbSelect.Attributes["onclick"] = "javascript:RadioCheck(this);";
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


       
    }
}