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
using DocsPAWA.utils;

namespace DocsPAWA.FirmaDigitale
{
	/// <summary>
	/// Summary description for DialogFirmaDigitale.
	/// </summary>
	public class DialogFirmaDigitale : CssPage
	{
		protected System.Web.UI.WebControls.Button btnApplicaFirma;
		protected System.Web.UI.WebControls.Button btnAnnulla;
		protected System.Web.UI.HtmlControls.HtmlSelect lstListaCertificati;
		protected System.Web.UI.WebControls.Label lblListaCertificati;
        protected System.Web.UI.WebControls.CheckBox chkConverti;
        protected System.Web.UI.WebControls.Panel pnlConversione;
        protected System.Web.UI.WebControls.RadioButton optLocale;
        protected System.Web.UI.WebControls.RadioButton optCentrale;
        protected System.Web.UI.WebControls.Label lblDocumentCount;
        protected System.Web.UI.WebControls.DataGrid grdResult;
        protected System.Web.UI.WebControls.Panel pnlFirmaHash;
        protected System.Web.UI.WebControls.CheckBox chkPades;

        
		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = -1;

            //if (!this.IsPostBack)
            //{
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

                chkPades.Visible = false;

                if (signHash)
                {
                    if (GetFileExtension().EndsWith("pdf"))
                        chkPades.Visible = true;
                }
 
                this.Form.Controls.Add(ctrl);
            }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnApplicaFirma_OnClick(object sender, EventArgs e)
        {
            this.ShowReport();
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
            this.grdResult.DataSource = FirmaDigitaleResultManager.CurrentData;
            this.grdResult.DataBind();

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

        protected bool signHash
        {
            get
            {
                //tutto in tryCatch
                try
                {

                    //provo globalmente
                    if (utils.InitConfigurationKeys.GetValue("0", "FE_SIGN_WITH_HASH").Equals("1"))
                        return true;

                    //altrimenti per singola amministrazione
                    string idamm = UserManager.getInfoUtente(this).idAmministrazione;
                    if (utils.InitConfigurationKeys.GetValue(idamm, "FE_SIGN_WITH_HASH").Equals("1"))
                        return true;
                }
                catch
                {

                }

                return false;
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
        /// Reperimento estensione del file da firmare
        /// </summary>
        /// <returns></returns>
        protected string GetSignType()
        {

            
            if (signHash)
            {
                return "1";
            }
            return "0";
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
                    string codice = (schedaDocumento.protocollo != null) ? schedaDocumento.protocollo.numero : schedaDocumento.docNumber;
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
