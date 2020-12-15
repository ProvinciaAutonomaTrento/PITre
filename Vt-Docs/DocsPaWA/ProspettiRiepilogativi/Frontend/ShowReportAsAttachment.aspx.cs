using System;

namespace DocsPAWA.ProspettiRiepilogativi.Frontend
{
    public partial class ShowReportAsAttachment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.ifReport.Attributes["src"] = "ProspettiRiepilogativi_RF.aspx?showAsAttachment=1";

        }
    }
}