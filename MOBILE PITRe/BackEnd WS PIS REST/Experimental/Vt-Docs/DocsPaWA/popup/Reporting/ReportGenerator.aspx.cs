using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.popup
{
    public partial class ReportGenerator : System.Web.UI.Page
    {
        /// <summary>
        /// Lista dei report registrati nel sistema
        /// </summary>
        private ReportMetadata[] ReportRegistry
        {
            set
            {
                CallContextStack.CurrentContext.ContextState["ReportRegistry"] = value;
            }

            get
            {
                return CallContextStack.CurrentContext.ContextState["ReportRegistry"] as ReportMetadata[];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Inizializzazione dell'interfaccia se è valorizzata la request
            if (ReportingUtils.PrintRequest != null && !IsPostBack)
                this.InitializeControls();
        }

        /// <summary>
        /// Funzione per l'inizializzazione della pagina
        /// </summary>
        private void InitializeControls()
        {
            Response.Expires = -1;
            // Recupero anagrafica dei report
            try
            {
                this.ReportRegistry = ReportingUtils.GetReportRegistry(ReportingUtils.PrintRequest.ContextName);
                // DataBinding con la drop down list dei report
                this.ddlReport.DataSource = this.ReportRegistry;
                this.ddlReport.DataBind();
                this.ShowReportMetadata(this.ReportRegistry[0]);

                // Per default il sottotitolo viene impostato con la data di creazione
                this.tbwSubtitle.WatermarkText = String.Format("Report generato {0} alle {1}", DateTime.Now.ToString("dddd, dd MMMM yyyy"), DateTime.Now.ToString("HH:mm.ss"));
                if (!string.IsNullOrEmpty(this.Request.QueryString["readOnlySubTitle"]))
                    this.txtReportSubtitle.Enabled = false;
            }
            catch
            {
                this.messageBox.ShowMessage("Non è stato possibile reperire informazioni sui report registrati.");
            }
        }

        /// <summary>
        /// Esportazione del report
        /// </summary>
        protected void btnExport_Click(object sender, EventArgs e)
        {
            // Impostazione delle proprietà del report da produrre
            ReportingUtils.PrintRequest.ReportKey = this.ddlReport.SelectedValue;
            ReportingUtils.PrintRequest.ReportType = (ReportTypeEnum)Enum.Parse(typeof(ReportTypeEnum), this.ddlExportFormat.SelectedValue, true);
            ReportingUtils.PrintRequest.SubTitle = String.IsNullOrEmpty(this.txtReportSubtitle.Text) ? this.tbwSubtitle.WatermarkText : this.txtReportSubtitle.Text;
            ReportingUtils.PrintRequest.Title = this.txtReportTitle.Text;
            ReportingUtils.PrintRequest.ColumnsToExport = this.ReportRegistry.Where(r => r.ReportKey == this.ddlReport.SelectedValue).First().ExportableFields;

            FileDocumento temp = null;

            try
            {
                temp = ReportingUtils.GenerateReport(ReportingUtils.PrintRequest);
            }
            catch
            {
                this.messageBox.ShowMessage("Errore durante la generazione del report richiesto.");

            }

            if (temp != null)
            {
                CallContextStack.CurrentContext.ContextState["documentFile"] = temp;

                this.reportContent.Attributes["src"] = "ReportContent.aspx";
            }

        }

        /// <summary>
        /// Rimozione dal callcontext di tutte le variabile generate durante la creazione del report
        /// </summary>
        protected void btnClose_Click(object sender, EventArgs e)
        {
            // Rimozione del file prodotto
            CallContextStack.CurrentContext.ContextState.Remove("documentFile");

            // Pulizia registro report
            this.ReportRegistry = null;

        }

        private void ShowReportMetadata(ReportMetadata report)
        {
            this.dgFields.DataSource = report.ExportableFields;
            this.dgFields.DataBind();

            if (report.ExportableFields != null &&
                report.ExportableFields.Length > 0)
                this.pnlFieldsSelection.Visible = true;
            else
                this.pnlFieldsSelection.Visible = false;
        }

        protected void ddlReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReportMetadata selectedReport = this.ReportRegistry.Where(r => r.ReportKey == ((DropDownList)sender).SelectedValue).First();
            this.ShowReportMetadata(selectedReport);

        }

        protected void chkSelected_CheckedChanged(object sender, EventArgs e)
        {
            ReportMetadata selectedReport = this.ReportRegistry.Where(r => r.ReportKey == ddlReport.SelectedValue).First();
            HiddenField hf = ((TableCell)((CheckBox)sender).Parent).FindControl("hfFieldName") as HiddenField;

            selectedReport.ExportableFields.Where(
                f => f.OriginalName == hf.Value).First().Export = 
                    ((CheckBox)sender).Checked;
        }

    }
}
