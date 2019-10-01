using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using System.Data;

namespace NttDataWA.Popup
{
    public partial class VisibilityHistoryRole : System.Web.UI.Page
    {
        #region Properties

        protected GridViewRow RowSelected
        {
            get
            {
                return HttpContext.Current.Session["RowSelected"] as GridViewRow;
            }
            set
            {
                HttpContext.Current.Session["RowSelected"] = value;
            }
        }

        protected DocumentoDiritto[] VisibilityList
        {
            get
            {
                DocumentoDiritto[] result = null;
                if (HttpContext.Current.Session["visibilityList"] != null)
                {
                    result = HttpContext.Current.Session["visibilityList"] as DocumentoDiritto[];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["visibilityList"] = value;
            }
        }

        private PrintReportRequest RequestPrintReport
        {
            get
            {
                if (HttpContext.Current.Session["requestPrintReport"] != null)
                    return (PrintReportRequest)HttpContext.Current.Session["requestPrintReport"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["requestPrintReport"] = value;
            }
        }

        private bool ReadOnlySubtitle
        {
            set
            {
                HttpContext.Current.Session["readOnlySubtitle"] = value;
            }
        }

        private RoleHistoryResponse response
        {
            get
            {
                if (HttpContext.Current.Session["RoleHistoryResponse"] != null)
                    return (RoleHistoryResponse)HttpContext.Current.Session["RoleHistoryResponse"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["RoleHistoryResponse"] = value;
            }
        }


        #endregion

        #region Remove Properties
        private void RemovePropertiesReportGenerator()
        {
            HttpContext.Current.Session.Remove("requestPrintReport");
            HttpContext.Current.Session.Remove("readOnlySubtitle");
        }

        #endregion 

        #region Const

        private const string UP_PANEL_BUTTONS = "UpPnlButtons";
        private const string CLOSE_REPORT_GENERATOR = "closeReportGenerator";

        #endregion

        #region Standard Method

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitializeLanguage();
                InitializeContent();
            }
            else
            {
                ReadRetValueFromPopup();
            }

        }

        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.VisibilityHistoryRoleBtnClose.Text = Utils.Languages.GetLabelFromCode("VisibilityHistoryRoleBtnClose", language);
            this.VisibilityHistoryRoleBtnExport.Text = Utils.Languages.GetLabelFromCode("VisibilityHistoryRoleBtnExport", language);
            this.litLegendaAzione.Text = Utils.Languages.GetLabelFromCode("VisibilityHistoryRoleLegendaAzione", language);
            this.litCreatingRole.Text = Utils.Languages.GetLabelFromCode("VisibilityHistoryRoleCreatingRole", language);
            this.litRoleChange.Text = Utils.Languages.GetLabelFromCode("VisibilityHistoryRoleChangeRole", language);
            this.litHistoricizingRole.Text = Utils.Languages.GetLabelFromCode("VisibilityHistoryRoleHistoricizingRole", language);
        }

        private void InitializeContent()
        {
            DocumentoDiritto docDiritti = this.VisibilityList[this.RowSelected.DataItemIndex];
            this.response = RoleManager.GetRoleHistory(new RoleHistoryRequest()
            {
                IdCorrGlobRole = (docDiritti.soggetto as Ruolo).systemId
            });
            GrdGridHistoryRole_Bind();
        }

        private void ReadRetValueFromPopup()
        {
            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_PANEL_BUTTONS))
            {
                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_REPORT_GENERATOR)))
                {
                    RemovePropertiesReportGenerator();
                    return;
                }
            }
        }

        #endregion

        #region Management Grid

        private void GrdGridHistoryRole_Bind()
        {
            this.GrdGridHistoryRole.DataSource = response.RoleHistoryItems;
            this.GrdGridHistoryRole.DataBind();
        }

        protected String GetRoleDescription(RoleHistoryItem roleHistoryItem)
        {
            if (!String.IsNullOrEmpty(Request["highlight"]) &&
                (roleHistoryItem.HistoryAction == AdmittedHistoryAction.S || roleHistoryItem.HistoryAction == AdmittedHistoryAction.C) &&
                roleHistoryItem.RoleCorrGlobId == Request["highlight"])
                return String.Format("<span style=\"color:Red\"><strong>{0}</strong></span>", roleHistoryItem.RoleDescription);
            else
                return roleHistoryItem.RoleDescription;

        }

        protected String FormatDate(DateTime dateTime)
        {
            //return dateTime.ToString("dddd, dd MMMM yyyy");
            return dateTime.ToString("dd/MM/yyyy");
        }

        protected void GrdGridHistoryRole_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.GrdGridHistoryRole.PageIndex = e.NewPageIndex;
                GrdGridHistoryRole_Bind();
                this.UpPanelGridHistoryRole.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #region Event

        protected void VisibilityHistoryRoleBtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('VisibilityHistoryRole','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void VisibilityHistoryRoleBtnExport_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                // Salvataggio della request nel call context 
                PrintReportRequest request =
                    new PrintReportObjectTransformationRequest()
                    {
                        ContextName = "GestioneRuolo",
                        ReportKey = "StoriaRuolo",
                        SearchFilters = null,
                        UserInfo = UserManager.GetInfoUser(),
                        DataObject = response.RoleHistoryItems
                    };

                //ReportingUtils.PrintRequest = request;
                ReadOnlySubtitle = true;
                RequestPrintReport = request;
                Session["visibleGrdFields"] = false;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ReportGenerator", "ajaxModalPopupReportGenerator();", true);
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