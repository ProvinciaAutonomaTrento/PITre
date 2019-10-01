using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;

namespace NttDataWA.Management
{
    public partial class Proceedings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializePage();
            }

            this.RefreshScripts();
        }

        protected void ProceedingsBtnPrint_Click(object sender, EventArgs e)
        {
            DocsPaWR.ReportProcedimentoRequest request = this.CreaRequest();

            DocsPaWR.ReportProcedimentoResponse response = ProceedingsManager.GetProcedimentiReport(request);

            if (response != null && response.Success)
            {
                if (response.Doc != null && response.Doc.content != null)
                {
                    this.UpPnlDocumentData.Visible = true;
                    FileManager.setSelectedFileReport(this, response.Doc, "../popup");
                    this.frame.Attributes["src"] = "../Summaries/PDFViewer.aspx";

                    this.UpPnlDocumentData.Update();
                    this.UpPnlContentDxSx.Update();
                }
                else
                {
                    string msg = "ProceedingsReportNoDataFound";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                    this.UpPnlDocumentData.Visible = false;
                    this.UpPnlDocumentData.Update();
                    this.UpPnlContentDxSx.Update();
                }
            }
            else
            {
                string msg = "ProceedingsReportError";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                this.UpPnlDocumentData.Visible = false;
                this.UpPnlDocumentData.Update();
                this.UpPnlContentDxSx.Update();
            }


            
        }

        private void InitializePage()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            this.LitProceedings.Text = Utils.Languages.GetLabelFromCode("ProceedingsTitle", language);
            this.lblReport.Text = Utils.Languages.GetLabelFromCode("ProceedingsReportType", language);
            
            this.ProceedingsBtnPrint.Text = Utils.Languages.GetLabelFromCode("ProceedingsBtnPrint", language);
            this.lblProceedingType.Text = Utils.Languages.GetLabelFromCode("ProceedingsLblType", language);
            this.lblProceedingsYear.Text = Utils.Languages.GetLabelFromCode("ProceedingsLblYear", language);

            this.ddl_reportType.Items.Clear();
            ddl_reportType.Items.Add(new ListItem() { Value = "reportProcedimentoSingolo", Text = Utils.Languages.GetLabelFromCode("ProceedingsLblSingle", language) });

            this.PopolaDdl();

        }

        private void RefreshScripts()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeIframe", "resizeIframe();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void PopolaDdl()
        {
            List<string> listaProcedimenti = UIManager.ProceedingsManager.GetTipiProcedimentoAmministrazione();

            this.ddl_proceeding_type.Items.Clear();
            foreach (string type in listaProcedimenti)
            {
                this.ddl_proceeding_type.Items.Add(new ListItem() { Text = type.Split('_')[0], Value = type.Split('_')[1] });
            }
        }

        private DocsPaWR.ReportProcedimentoRequest CreaRequest()
        {
            DocsPaWR.ReportProcedimentoRequest request = new DocsPaWR.ReportProcedimentoRequest();
            request.IdAmm = UserManager.GetInfoUser().idAmministrazione;
            request.IdProcedimento = this.ddl_proceeding_type.SelectedValue;
            request.Anno = this.txt_anno.Text;

            return request;
        }
    }
}