using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;

namespace NttDataWA.Management
{
    public partial class ReportAccessi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                this.InitializeLanguage();
                this.InitializePage();
            }
            else
            {
                if(!string.IsNullOrEmpty(this.HiddenConfirmExport.Value))
                {
                    this.HiddenConfirmExport.Value = string.Empty;
                    this.Export();

                }
                else if(!string.IsNullOrEmpty(this.HiddenConfirmPublish.Value))
                {
                    this.HiddenConfirmPublish.Value = string.Empty;
                    this.Publish();
                }
            }

            this.RefreshScript();
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            this.ReportAccessiTitle.Text = Utils.Languages.GetLabelFromCode("ReportAccessiTitle", language);
            this.lblFormato.Text = Utils.Languages.GetLabelFromCode("ReportAccessiLblFormato", language);
            this.xlsItem.Text = Utils.Languages.GetLabelFromCode("ReportAccessiXlsItem", language);
            this.odtItem.Text = Utils.Languages.GetLabelFromCode("ReportAccessiOdtItem", language);
            this.ReportAccessiLitData.Text = Utils.Languages.GetLabelFromCode("ReportAccessiLitData", language);
            this.LtlDaDataCreazione.Text = Utils.Languages.GetLabelFromCode("ReportAccessiLitDataDa", language);
            this.LtlADataCreazione.Text = Utils.Languages.GetLabelFromCode("ReportAccessiLitDataA", language);
            this.ddlDateItemSingleValue.Text = Utils.Languages.GetLabelFromCode("ReportAccessiItemSingleValue", language);
            this.ddlDateItemRange.Text = Utils.Languages.GetLabelFromCode("ReportAccessiItemRange", language);
            this.LtlStatoFascicolo.Text = Utils.Languages.GetLabelFromCode("ReportAccessiLitStatoFasc", language);
            this.itemAll.Text = Utils.Languages.GetLabelFromCode("ReportAccessiItemAll", language);
            this.itemOpen.Text = Utils.Languages.GetLabelFromCode("ReportAccessiItemOpen", language);
            this.itemClosed.Text = Utils.Languages.GetLabelFromCode("ReportAccessiItemClosed", language);
            this.BtnExport.Text = Utils.Languages.GetLabelFromCode("ReportAccessiBtnPreReport", language);
            this.BtnPublish.Text = Utils.Languages.GetLabelFromCode("ReportAccessiBtnPublish", language);
        }

        protected void InitializePage()
        {
            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_REGISTRO_ACCESSI_EXPORT"))
            {
                this.BtnExport.Visible = false;
            }
            if(!UIManager.UserManager.IsAuthorizedFunctions("DO_REGISTRO_ACCESSI_PUBLISH"))
            {
                this.BtnPublish.Visible = false;
            }
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OnlyNumbers", "OnlyNumbers();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeIframe", "resizeIframe();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        protected void ddl_dataCreazione_E_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(ddl_dataCreazione_E.SelectedIndex)
            {
                case 0:
                    this.LtlDaDataCreazione.Visible = false;
                    this.LtlADataCreazione.Visible = false;
                    this.txt_finedataCreazione_E.Visible = false;
                    this.txt_finedataCreazione_E.Text = string.Empty;
                    break;
                case 1:
                    this.LtlDaDataCreazione.Visible = true;
                    this.LtlADataCreazione.Visible = true;
                    this.txt_finedataCreazione_E.Visible = true;
                    this.txt_finedataCreazione_E.Text = string.Empty;
                    break;
            }
        }

        protected void BtnExport_Click(object sender, EventArgs e)
        {
            if(this.CheckIntervalloDate())
            {
                this.Export();
            }
            else
            {
                string msgConfirm = "RegistroAccessiWarning";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('" + msgConfirm + "', 'HiddenConfirmExport', '', '');", true);
            }
        }

        protected void BtnPublish_Click(object sender, EventArgs e)
        {
            if (this.CheckIntervalloDate())
            {
                this.Publish();
            }
            else
            {
                string msgConfirm = "RegistroAccessiWarning";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('" + msgConfirm + "', 'HiddenConfirmPublish', '', '');", true);
            }
        }

        protected bool CheckIntervalloDate()
        {
            bool result = true;

            if(string.IsNullOrEmpty(this.txt_initDataCreazione_E.Text) && string.IsNullOrEmpty(this.txt_finedataCreazione_E.Text))
            {
                result = false;
            }

            return result;
        }

        private void Export()
        {
            DocsPaWR.PrintReportRequest request = new DocsPaWR.PrintReportRequest();

            // Parametri requesti
            request.Title = "Registro degli Accessi - " + UserManager.getInfoAmmCorrente(UserManager.GetInfoUser().idAmministrazione).Descrizione;
            request.ContextName = "RegistroAccessiExport";
            request.ReportKey = "RegistroAccessiExport";
            request.SearchFilters = this.SetFilters();

            switch (this.ddlFormatoPreReport.SelectedValue)
            {
                case "XLS":
                    request.ReportType = DocsPaWR.ReportTypeEnum.Excel;
                    break;
                case "ODS":
                    request.ReportType = DocsPaWR.ReportTypeEnum.ODS;
                    break;
            }

            DocsPaWR.FileDocumento fileDoc = ReportingManager.GenerateReport(request);

            if (fileDoc != null && fileDoc.content != null && fileDoc.content.Length > 0)
            {
                this.UpPnlDocumentData.Visible = true;

                this.FileDoc = fileDoc;
                this.frame.Attributes["src"] = "../Document/AttachmentViewer.aspx";

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

        private void Publish()
        {
            DocsPaWR.RegistroAccessiReportRequest request = new DocsPaWR.RegistroAccessiReportRequest();

            request.filters = this.SetFilters();
            request.requestType = DocsPaWR.RequestType.PUBLISH;

            DocsPaWR.RegistroAccessiReportResponse response = RegistroAccessiManager.PubblicaRegistroAccessi(request);

            if (response != null && response.success && response.document != null && response.document.content != null)
            {
                this.UpPnlDocumentData.Visible = true;

                this.FileDoc = response.document;
                this.frame.Attributes["src"] = "../Document/AttachmentViewer.aspx";

                this.UpPnlDocumentData.Update();
                this.UpPnlContentDxSx.Update();
            }
            else
            {
                string msg = "RegistroAccessiPublishError";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                this.UpPnlDocumentData.Visible = false;
                this.FileDoc = null;
                this.UpPnlDocumentData.Update();
                this.UpPnlContentDxSx.Update();
            }
        }

        private DocsPaWR.FiltroRicerca[] SetFilters()
        {
            List<DocsPaWR.FiltroRicerca> filters = new List<DocsPaWR.FiltroRicerca>();

            #region Amministrazione
            filters.Add(new DocsPaWR.FiltroRicerca() { argomento = "id_amm", valore = UserManager.GetInfoUser().idAmministrazione });
            #endregion

            #region Tipologia (deve essere vuoto)
            filters.Add(new DocsPaWR.FiltroRicerca() { argomento = "tipologia", valore = string.Empty });
            #endregion

            #region Data creazione
            filters.Add(new DocsPaWR.FiltroRicerca() { argomento = "data_creazione", valore = this.ddl_dataCreazione_E.SelectedValue });
            filters.Add(new DocsPaWR.FiltroRicerca() { argomento = "data_creazione_da", valore = this.txt_initDataCreazione_E.Text });
            if(this.ddl_dataCreazione_E.SelectedValue.Equals("1"))
            {
                filters.Add(new DocsPaWR.FiltroRicerca() { argomento = "data_creazione_a", valore = this.txt_finedataCreazione_E.Text });
            }
            else
            {
                filters.Add(new DocsPaWR.FiltroRicerca() { argomento = "data_creazione_a", valore = string.Empty });
            }
            #endregion

            #region Stato del fascicolo
            filters.Add(new DocsPaWR.FiltroRicerca() { argomento = "stato", valore = this.ddl_statoFascicolo.SelectedValue });
            #endregion

            #region Formato export
            filters.Add(new DocsPaWR.FiltroRicerca() { argomento = "formato_export", valore = this.ddlFormatoPreReport.SelectedValue });
            #endregion

            return filters.ToArray();
        }

        private DocsPaWR.FileDocumento FileDoc
        {
            get
            {
                if (HttpContext.Current.Session["fileDoc"] != null)
                    return HttpContext.Current.Session["fileDoc"] as DocsPaWR.FileDocumento;
                else return null;
            }
            set
            {
                HttpContext.Current.Session["fileDoc"] = value;
            }
        }
    }
}