using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.utils;

namespace DocsPAWA.MassiveOperation
{
    public partial class MassiveMasterPage : System.Web.UI.MasterPage
    {
        public delegate bool btnConfermaClick(object sender, EventArgs e);
        public string pageName;
        public string idLabel;
        public btnConfermaClick confermaDelegate;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.appTitleProvider.PageName = pageName;
            this.grdReport.Columns[0].HeaderText = idLabel;
        }

        protected void btnConfermaMP_Click(object sender, EventArgs e)
        {
            this.mdlPopupWait.Hide();
            bool res=confermaDelegate(sender, e);
            if (res)
            {
                this.Form.Visible = false;
                this.btnConferma.Visible = false;
                this.btnChiudi.Text = "Chiudi";
                this.btnChiudi.ToolTip = "Chiudi";
                this.btnExportReport.Enabled = true;
                ScriptManager.RegisterStartupScript(this.btnChiudi, this.GetType(), "resize", "resize();", true);
            }
        }

        public void showMessage(string message)
        {
            this.ltlMessage.Text = message;
            this.upMessage.Update();
            this.mpeMessage.Show();
        }

        public void generateReport(MassiveOperationReport report,string titolo,bool isFasc)
        {
            this.grdReport.DataSource = report.GetDataSet();
            this.grdReport.DataBind();
            this.pnlReport.Visible = true;
            string template = (isFasc) ? "formatPdfExport_fasc.xml" : "formatPdfExport.xml";
            report.GenerateDataSetForExport(Server.MapPath(template),titolo);
        }

        public void addJSOnConfermaButton(string js)
        {
            this.btnConferma.OnClientClick = js;
        }

        public void addJSOnChiudiButton(string js)
        {
            this.btnChiudi.OnClientClick = js;
        }
    }
}