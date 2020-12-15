using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.MassiveOperation
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DocumentConsolidationSummary : CssPage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Expires = -1;

            if (!this.IsPostBack)
            {
                this.grdDocumentConsolidationSummary.DataSource = this.GetReportData();
                this.grdDocumentConsolidationSummary.DataBind();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportPdf_Click(object sender, EventArgs e)
        {
            DocsPAWA.utils.MassiveOperationReport report = this.GetReport();

            report.GenerateDataSetForExport(Server.MapPath("formatPdfExport.xml"),"Consolidazione documenti");

            this.Page.ClientScript.RegisterStartupScript(this.GetType(), 
                                "openReport",
                                "openReport();", 
                                true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected DocsPAWA.utils.MassiveOperationReport GetReport()
        {
            return SiteNavigation.CallContextStack.CurrentContext.ContextState["DocumentConsolidationSummary"] as DocsPAWA.utils.MassiveOperationReport;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected System.Data.DataSet GetReportData()
        {
            if (SiteNavigation.CallContextStack.CurrentContext.ContextState.ContainsKey("DocumentConsolidationSummary"))
            {
                DocsPAWA.utils.MassiveOperationReport report = GetReport();

                if (report != null)
                    return report.GetDataSet();
                else
                    return null;
            }
            else
                return null;
        }
    }
}