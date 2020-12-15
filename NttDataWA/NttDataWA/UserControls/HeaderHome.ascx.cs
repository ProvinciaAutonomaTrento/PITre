using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA;
using NttDataWA.DocsPaWR;

namespace NttDataWA.UserControls
{
    public partial class HeaderHome : System.Web.UI.UserControl
    {
        #region Property
        private string PageCaller
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["pageCaller"] != null)
                {
                    result = HttpContext.Current.Session["pageCaller"].ToString();
                }
                return result;
            }
        }

        private bool IsRoleChanged
        {
            set
            {
                HttpContext.Current.Session["IsRoleChanged"] = value;
            }
        }
        #endregion

        #region Const

        private const string NOTIFICATION_CENTER = "NOTIFICATION_CENTER";

        #endregion

        #region Standard Method
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    this.InitializePage();
                    this.InitializeLanguage();
                }
                RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitializePage()
        {
            LoadRolesUser();
        }

        private void InitializeLanguage()
        { 
            string language = UIManager.UserManager.GetUserLanguage();
            this.headerHomeLblRole.Text = Utils.Languages.GetLabelFromCode("HeaderHomeLblRole", language);
            this.ddlRolesUser.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("HeaderHomeChooseOtherRole", language));
        }


        private void LoadRolesUser()
        {
            try
            {
                ListItem item;
                Ruolo[] role = UIManager.UserManager.GetUserInSession().ruoli;
                foreach (Ruolo r in role)
                {
                    item = new ListItem();
                    item.Value = r.systemId;
                    item.Text = r.descrizione;
                    this.ddlRolesUser.Items.Add(item);
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
        }

        #endregion

        #region Event hendler

        protected void HomeDdlRoles_SelectedIndexChange(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.ddlRolesUser.SelectedValue))
                {
                    Ruolo[] roles = UIManager.UserManager.GetUserInSession().ruoli;
                    Ruolo role = (from r in roles where r.systemId.Equals(this.ddlRolesUser.SelectedValue) select r).FirstOrDefault();
                    UIManager.RoleManager.SetRoleInSession(role);
                    UIManager.RegistryManager.SetRegAndRFListInSession(UIManager.UserManager.getListaRegistriWithRF(role.systemId,"",""));
                    UIManager.RegistryManager.SetRFListInSession(UIManager.UserManager.getListaRegistriWithRF(role.systemId,"1",""));
                    (this.Page.Master.FindControl("upPnlInfoUser") as UpdatePanel).Update();
                    if (this.PageCaller.Equals(NOTIFICATION_CENTER))
                        IsRoleChanged = true;
                    (this.Parent.FindControl("UpdatePanelRepListNotify") as UpdatePanel).Update();
                }
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