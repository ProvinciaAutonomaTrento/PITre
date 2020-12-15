using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Text;
using System.Linq;
using DocsPAWA.FirmaDigitale;
using DocsPAWA.utils;

namespace DocsPAWA.MassiveOperation
{
	/// <summary>
	/// Summary description for DialogFirmaDigitale.
	/// </summary>
	public class FirmaDigitaleMassiva : MassivePage
	{
		protected System.Web.UI.HtmlControls.HtmlSelect lstListaCertificati;
		protected System.Web.UI.WebControls.Label lblListaCertificati;
        protected System.Web.UI.WebControls.CheckBox chkConverti;
        protected System.Web.UI.WebControls.Panel pnlConversione;
        protected System.Web.UI.WebControls.RadioButton optLocale;
        protected System.Web.UI.WebControls.RadioButton optCentrale;
        protected System.Web.UI.WebControls.Label lblDocumentCount;

        protected override string PageName
        {
            get
            {
                return "Firma digitale massiva";
            }
        }

        protected override bool IsFasc
        {
            get { return false; }
        }

		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = -1;

            this.lblDocumentCount.Text = string.Format("{0} documenti da firmare", this.GetSelectedDocuments().Count);

            DocsPaWR.Ruolo ruoloCorrente = UserManager.getRuolo();

            DocsPaWR.Funzione funzioneFirma256 = ruoloCorrente.funzioni.FirstOrDefault(itm => itm.codice == "DO_DOC_FIRMA_256");

            System.Web.UI.UserControl ctrl = null;

            if (funzioneFirma256 != null)
            {
                // Attivazione usercontrol smartclient
                ctrl = (System.Web.UI.UserControl)this.LoadControl("~/SmartClient/DigitalSignWrapper.ascx");
            }
            else
            {
                // Attivazione usercontrol activex
                ctrl = (System.Web.UI.UserControl)this.LoadControl("~/ActivexWrappers/CapicomWrapper.ascx");
            }

            this.Form.Controls.Add(ctrl);

            if (!this.IsPostBack)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "init", "Initialize();", true);
                this.addJSOnConfermaButton("return ApplySign()");
                this.lblDocumentCount.Text = string.Format("{0} documenti da firmare", this.GetSelectedDocuments().Count);
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override bool btnConferma_Click(object sender, EventArgs e)
        {
            this.ShowReport();
            return true;
        }

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

            this.chkConverti.Attributes.Add("onClick", "OnClickConvertPDF();");

		}
		#endregion

        /// <summary>
        /// Visualizzazione dell'esito della firma applicata ad uno o più documenti
        /// </summary>
        protected void ShowReport()
        {
            List<FirmaDigitaleResultStatus> res= FirmaDigitaleResultManager.CurrentData;
            MassiveOperationReport report=new MassiveOperationReport();
            foreach (FirmaDigitaleResultStatus temp in res)
            {
                MassiveOperationTarget target = MassiveOperationUtils.getItem(temp.IdDocument);
                MassiveOperationReport.MassiveOperationResultEnum result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                if(!temp.Status) result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                report.AddReportRow(target.Codice, result, temp.StatusDescription);
            }
            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
            report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);
            this.generateReport(report,"Firma digitale massiva");
            FirmaDigitaleResultManager.ClearData();
        }

        /// <summary>
        /// Lettura delle configurazioni per verificare
        /// se la gestione di conversione in pdf di un file firmato è attiva o meno
        /// </summary>
        protected bool ConvertPdfOnSign
        {
            get
            {
                bool retValue = false;

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ENABLE_CONVERT_PDF_ON_SIGN"]))
                    bool.TryParse(ConfigurationManager.AppSettings["ENABLE_CONVERT_PDF_ON_SIGN"], out retValue);

                return retValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsEnabledConvPDFSincrona
        {
            get
            {
                return new DocsPAWA.DocsPaWR.DocsPaWebService().IsEnabledConversionePdfLatoServerSincrona();
            }
        }
            
        /// <summary>
        /// Reperimento estensione del file da firmare
        /// </summary>
        /// <returns></returns>
        protected string GetFileExtension()
        {
            DocsPAWA.DocsPaWR.FileRequest fileRequest = DocsPAWA.FileManager.getSelectedFile(this);

            if (fileRequest != null)
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileRequest.fileName);
                return fileInfo.Extension.ToLower();
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected List<MassiveOperationTarget> GetSelectedDocuments()
        {
            List<MassiveOperationTarget> selectedItems = DocsPAWA.utils.MassiveOperationUtils.GetSelectedItems();

            if (selectedItems.Count == 0)
            {
                DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato();

                if (schedaDocumento != null)
                {
                    string codice=(schedaDocumento.protocollo!=null) ? schedaDocumento.protocollo.numero : schedaDocumento.docNumber;
                    MassiveOperationTarget mot = new MassiveOperationTarget(schedaDocumento.systemId, codice);
                    mot.Checked = true;
                    selectedItems.Add(mot);
                }
            }

            return selectedItems;
        }

        /// <summary>
        /// Reperimento degli id dei documenti selezionati da firmare digitalmente
        /// </summary>
        /// <returns></returns>
        protected string GetSelectedDocumentsIds()
        {
            StringBuilder sb = new StringBuilder();

            List<MassiveOperationTarget> selectedItems = this.GetSelectedDocuments();

            foreach (MassiveOperationTarget temp in selectedItems)
            {
                if (sb.Length > 0)
                    sb.Append("|");
                sb.Append(temp.Id);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
		protected string TipoFirma
		{
            get
            {
                string tipoFirma = Request.QueryString["TipoFirma"];

                if (tipoFirma == null || tipoFirma == string.Empty)
                    tipoFirma = "sign";

                return tipoFirma;
            }
		}

		public string GetMaskTitle()
		{
			string retValue=string.Empty;

			if (this.TipoFirma=="cosign")
				retValue="Co-Firma documento";
			else
				retValue="Firma documento";

			return retValue;
		}

		#region Gestione javascript

		/// <summary>
		/// Registrazione script client
		/// </summary>
		/// <param name="scriptKey"></param>
		/// <param name="scriptValue"></param>
		private void RegisterClientScript(string scriptKey,string scriptValue)
		{
			if(!this.Page.IsStartupScriptRegistered(scriptKey))
			{
				string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
				this.Page.RegisterStartupScript(scriptKey, scriptString);
			}
		}

		#endregion
	}
}
