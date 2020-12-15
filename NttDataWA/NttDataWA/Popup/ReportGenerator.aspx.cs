using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using System.Collections;

namespace NttDataWA.Popup
{
    public partial class ReportGenerator : System.Web.UI.Page
    {
        #region Property

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

        /// <summary>
        /// Lista dei report registrati nel sistema
        /// </summary>
        private ReportMetadata[] ReportRegistry
        {
            set
            {
                HttpContext.Current.Session["ReportRegistry"] = value;
            }

            get
            {
                if (HttpContext.Current.Session["ReportRegistry"] != null)
                    return (ReportMetadata[])HttpContext.Current.Session["ReportRegistry"];
                else
                    return null;
            }
        }

        private bool ReadOnlySubtitle
        {
            get
            {
                if (HttpContext.Current.Session["readOnlySubtitle"] != null)
                    return (Boolean)HttpContext.Current.Session["readOnlySubtitle"];
                else
                    return false;
            }
        }


        private bool VisibleGrdFields
        {
            get
            {
                if (HttpContext.Current.Session["visibleGrdFields"] != null)
                    return (Boolean)HttpContext.Current.Session["visibleGrdFields"];
                else
                    return false;
            }
        }


        private bool ActiveExportFormatPDF
        {
            get
            {
                if (HttpContext.Current.Session["ActiveExportFormatPDF"] != null)
                    return (Boolean)HttpContext.Current.Session["ActiveExportFormatPDF"];
                else
                    return true;
            }
        }

        private bool ActiveExportFormatExcel
        {
            get
            {
                if (HttpContext.Current.Session["ActiveExportFormatExcel"] != null)
                    return (Boolean)HttpContext.Current.Session["ActiveExportFormatExcel"];
                else
                    return true;
            }
        }

        private bool ActiveExportFormatODS
        {
            get
            {
                if (HttpContext.Current.Session["ActiveExportFormatODS"] != null)
                    return (Boolean)HttpContext.Current.Session["ActiveExportFormatODS"];
                else
                    return true;
            }
        }

        /// <summary>
        ///  
        /// </summary>
        private FileDocumento FileDocPrintReport
        {
            set
            {
                HttpContext.Current.Session["fileDocPrintReport"] = value;
            }
        }


        #endregion

        #region Standard Method
        

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    InitializeLanguage();
                    if (RequestPrintReport != null)
                        this.InitializeControls();
                }
                RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.ReportGeneratorExport.Text = Utils.Languages.GetLabelFromCode("ReportGeneratorExportBtn", language);
            this.ReportGeneratorClose.Text = Utils.Languages.GetLabelFromCode("ReportGeneratorClose", language);
            this.lblExportFormat.Text = Utils.Languages.GetLabelFromCode("ReportGeneratorLblExportFormat", language);
            this.lblReportGenerator.Text = Utils.Languages.GetLabelFromCode("ReportGeneratorLblReportGenerator", language);
            this.lblReportSubtitle.Text = Utils.Languages.GetLabelFromCode("ReportGeneratorLblReportSubtitle", language);
            this.tbwTitle.WatermarkText = Utils.Languages.GetLabelFromCode("ReportGeneratorTbwTitle", language);
            this.lblTitle.Text = Utils.Languages.GetLabelFromCode("ReportGeneratorLblTitle", language);
            this.lblFilterExport.Text = Utils.Languages.GetLabelFromCode("ReportGeneratorLblFilterExport", language);
            // Per default il sottotitolo viene impostato con la data di creazione
            string txtSubtitle = Utils.Languages.GetLabelFromCode("ReportGeneratorTbwSubtitle", language);
            if (RequestPrintReport.ContextName.Equals("CasellaIstituzionale"))
            {
                this.tbwSubtitle.WatermarkText = RequestPrintReport.SubTitle;
            }
            else
            {
                this.tbwSubtitle.WatermarkText = String.Format(txtSubtitle, DateTime.Now.ToString("dddd, dd MMMM yyyy"), DateTime.Now.ToString("HH:mm.ss"));
            }
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
                this.ReportRegistry = CheckMailboxManager.GetReportRegistry(RequestPrintReport.ContextName);
                // DataBinding con la drop down list dei report
                this.DdlReportGenerator.DataSource = this.ReportRegistry;
                this.DdlReportGenerator.DataBind();
                this.ShowReportMetadata(this.ReportRegistry[0]);
                if (ReadOnlySubtitle)
                    this.txtReportSubtitle.Enabled = false;

                // visibilità GrdFields
                if (VisibleGrdFields != null)
                {
                    if (GrdFields != null && GrdFields.Rows.Count > 0)
                    {
                        this.GrdFields.Visible = VisibleGrdFields;
                        this.lblFilterExport.Visible = VisibleGrdFields;
                    }
                    else
                    {
                        this.pnlFieldsSelection.Visible = false;
                    }
                }
              
                //Visibilità selezione formati export
                //PDF
                if (!ActiveExportFormatPDF) ddlExportFormat.Items.RemoveAt(0);
                //EXCEL
                if (!ActiveExportFormatExcel)  ddlExportFormat.Items.RemoveAt(1);
                //ODS  
                if (!ActiveExportFormatODS) ddlExportFormat.Items.RemoveAt(2);
            }
            catch
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('ErrorReportGeneratorExtractInfo', 'error', '');", true);
                return;
            }
        }

        private void DeleteProperty()
        {
            HttpContext.Current.Session.Remove("fileDocPrintReport");
            if (HttpContext.Current.Session["ActiveExportFormatPDF"] != null) HttpContext.Current.Session.Remove("ActiveExportFormatPDF");
            if (HttpContext.Current.Session["ActiveExportFormatExcel"] != null) HttpContext.Current.Session.Remove("ActiveExportFormatExcel");
            if (HttpContext.Current.Session["ActiveExportFormatODS"] != null) HttpContext.Current.Session.Remove("ActiveExportFormatODS");
        }

        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeIframe", "resizeIframe();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
        }

        #endregion

        #region Grid manager

        private void ShowReportMetadata(ReportMetadata report)
        {
            this.GrdFields.DataSource = report.ExportableFields;
            this.GrdFields.DataBind();

            if (report.ExportableFields != null &&
                report.ExportableFields.Length > 0)
                this.pnlFieldsSelection.Visible = true;
            else
                this.pnlFieldsSelection.Visible = false;
        }

        #endregion

        #region Event Handler

        protected void ReportGeneratorExport_Click(object sender, EventArgs e)
        {
            try {
                // Impostazione delle proprietà del report da produrre
                RequestPrintReport.ReportKey = this.DdlReportGenerator.SelectedValue;
                RequestPrintReport.ReportType = (ReportTypeEnum)Enum.Parse(typeof(ReportTypeEnum), this.ddlExportFormat.SelectedValue, true);
                RequestPrintReport.SubTitle = String.IsNullOrEmpty(this.txtReportSubtitle.Text) ? this.tbwSubtitle.WatermarkText : this.txtReportSubtitle.Text;
                RequestPrintReport.Title = String.IsNullOrEmpty(this.txtReportTitle.Text) ? this.tbwTitle.WatermarkText : this.txtReportTitle.Text;
                RequestPrintReport.ColumnsToExport = this.ReportRegistry.Where(r => r.ReportKey == this.DdlReportGenerator.SelectedValue).First().ExportableFields;

                FileDocumento temp = null;

                try
                {
                    temp = CheckMailboxManager.GenerateReport(RequestPrintReport);
                }
                catch
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('ErrorReportGenerator', 'error', '');", true);
                    return;
                }

                if (temp != null)
                {
                    FileDocPrintReport = temp;

                    this.reportContent.Attributes["src"] = "../Popup/ReportContent.aspx";
                    this.upPanelFrame.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ReportGeneratorClose_Click(object sender, EventArgs e)
        {
            try {
                DeleteProperty();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "parent.closeAjaxModal('ReportGenerator', 'close', parent);", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

       
        protected void ddlReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                ReportMetadata selectedReport = this.ReportRegistry.Where(r => r.ReportKey == ((DropDownList)sender).SelectedValue).First();
                this.ShowReportMetadata(selectedReport);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void chkSelected_CheckedChanged(object sender, EventArgs e)
        {
            try {
                ReportMetadata selectedReport = this.ReportRegistry.Where(r => r.ReportKey == DdlReportGenerator.SelectedValue).First();
                HiddenField hf = ((TableCell)((CheckBox)sender).Parent).FindControl("hfFieldName") as HiddenField;

                selectedReport.ExportableFields.Where(
                    f => f.OriginalName == hf.Value).First().Export =
                        ((CheckBox)sender).Checked;
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