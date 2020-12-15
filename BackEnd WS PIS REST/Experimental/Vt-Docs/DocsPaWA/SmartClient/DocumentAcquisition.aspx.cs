using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Threading;
using System.Configuration;

namespace DocsPAWA.SmartClient
{
	public class DocumentAcquisition : System.Web.UI.Page 
	{
		protected System.Web.UI.HtmlControls.HtmlInputFile uploadedFile;
		protected System.Web.UI.WebControls.CheckBox chk_ConvertiPDF;
		protected System.Web.UI.WebControls.CheckBox chkRecognizeText;
        protected System.Web.UI.WebControls.CheckBox chk_cartaceo;
		protected System.Web.UI.HtmlControls.HtmlInputHidden txtPDFConvert;
		protected System.Web.UI.HtmlControls.HtmlInputHidden txtPDFConvertEnabled;
		protected System.Web.UI.WebControls.RadioButton optAcquisizioneScanner;
		protected System.Web.UI.WebControls.Label lblAcquisisciDaFile;
		protected System.Web.UI.WebControls.Panel pnlRecognizeText;
		protected System.Web.UI.HtmlControls.HtmlTable tblContainer;
		protected System.Web.UI.WebControls.Label Label3;
		protected System.Web.UI.WebControls.Button btnUpload;
		protected System.Web.UI.HtmlControls.HtmlInputButton btnInvia;
		protected System.Web.UI.WebControls.Label lblAcquisisciDaScanner;
		protected System.Web.UI.WebControls.RadioButton optAcquisisciDaFile;

		public string path 
		{
			get 
			{
				return Utils.getHttpFullPath(this);
			}
		}

		private void Page_Load(object sender, System.EventArgs e) 
		{
			if (!this.IsPostBack) 
			{
				Response.Expires=-1;

				this.SetDefaultButton();

				// Lettura valori di configurazione
				this.ReadConfigurationData();

				// Inizializzazione script e handler eventi controlli javascript
				this.InitializeJavaScriptCode();
			} 
			else 
			{
				if (this.optAcquisisciDaFile.Checked && uploadedFile.PostedFile!=null)
				{
					string scriptStr = "<script language='javascript'>window.close();</script>";
					FileManager.uploadFile(this,uploadedFile.PostedFile);
					Response.Write(scriptStr);
				}
			}
		}

		/// <summary>
		/// Gestione impostazione pulsante di protocollazione come pulsante di default.
		/// E' necessario impostare un handler di evento javascript per ogni controllo 
		/// editabile sul quale ci si può posizionare.
		/// </summary>
		private void SetDefaultButton()
		{
			DocsPAWA.Utils.DefaultButton(this,ref this.optAcquisizioneScanner,ref this.btnInvia);
			DocsPAWA.Utils.DefaultButton(this,ref this.optAcquisisciDaFile,ref this.btnInvia);
			DocsPAWA.Utils.DefaultButton(this,ref this.chkRecognizeText,ref this.btnInvia);
			DocsPAWA.Utils.DefaultButton(this,ref this.chk_ConvertiPDF,ref this.btnInvia);

			//			DocsPAWA.Utils.DefaultButton(this,ref this.chkRecognizeText,ref this.btnUpload);
			//			DocsPAWA.Utils.DefaultButton(this,ref this.chk_ConvertiPDF,ref this.btnUpload);
		}

		/// <summary>
		/// Lettura valori di configurazione
		/// </summary>
		private void ReadConfigurationData()
		{
			this.txtPDFConvert.Value="false";
			this.txtPDFConvertEnabled.Value="false";

			try 
			{
				this.txtPDFConvert.Value=ConfigSettings.getKey(ConfigSettings.KeysENUM.DOCUMENT_PDF_CONVERT).ToLower();					
				this.txtPDFConvertEnabled.Value=ConfigSettings.getKey(ConfigSettings.KeysENUM.DOCUMENT_PDF_CONVERT_ENABLED).ToLower();
			} 
			catch
			{
			}
		}

		/// <summary>
		/// Inizializzazione script e handler eventi controlli javascript
		/// </summary>
		private void InitializeJavaScriptCode()
		{
			//			if (this.txtAdobeAcrobatIntegration.Value=="true")
			//				// Solo se integrazione con SDK di acrobat attivata
			
			this.chk_ConvertiPDF.Attributes.Add("onClick","chkConvertiPDF_onClick();");

			// Associazione handler eventi javascript
			this.optAcquisizioneScanner.Attributes.Add("onClick","OnClickRadioAcquisizioneScanner();");
			this.optAcquisisciDaFile.Attributes.Add("onClick","OnClickRadioAcquisizioneDaFile();");
				
			// Impostazione valore di default check converti pdf
			this.RegisterClientScript("SetDefaultValueCheckConvertiPDF","SetDefaultValueCheckConvertiPDF();");
			// Impostazione abilitazione/disabilitazione di default check converti pdf
			this.RegisterClientScript("SetDefaultEnabledCheckConvertiPDF","SetDefaultEnabledCheckConvertiPDF();");
			// Impostazione visibilità check interpretazione testo con ocr
			//this.RegisterClientScript("SetVisibilityCheckRecognize","SetVisibilityCheckRecognize(" + this.txtAdobeAcrobatIntegration.Value + ");");
			// Abilitazione / disabilitazione check riconoscimento ocr
			this.RegisterClientScript("EnableCheckRecognizeText","EnableCheckRecognizeText();");
			// Impostazione del radio button selezionato per default
			this.RegisterClientScript("SetDefaultRadioButton","SetDefaultRadioButton();");
			// Impostazione del focus su un controllo "optAcquisisciDaFile"
			this.RegisterClientScript("SetControlFocus","SetControlFocus('" + this.optAcquisizioneScanner.ClientID + "');");
		}

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
				//this.Page.RegisterStartupScript(scriptKey, scriptString);
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), scriptKey, scriptString);
			}
		}

        /// <summary>
        /// Reperimento dalle configurazioni dimensione massima di un file acquisibile
        /// </summary>
        protected int FileSizeMax
        {
            get
            {
                int fileSizeMax = Int32.MaxValue;

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["FILE_ACQ_SIZE_MAX"]))
                {
                    if (!int.TryParse(ConfigurationManager.AppSettings["FILE_ACQ_SIZE_MAX"], out fileSizeMax))
                        throw new ApplicationException("Valore impostato per la chiave del web.config 'FILE_ACQ_SIZE_MAX' non valido");
                }

                return fileSizeMax;
            }
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
			this.ID = "DocumentAcquisition";
			this.Load += new System.EventHandler(this.Page_Load);

		}

		#endregion
	}
}
