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

namespace DocsPAWA.popup {
	/// <summary>
	/// Summary description for acquisisciDocumento.
	/// </summary>
	public class acquisisciDocumento : DocsPAWA.CssPage {
        protected System.Web.UI.HtmlControls.HtmlInputHidden cache;
		protected System.Web.UI.HtmlControls.HtmlInputFile uploadedFile;
		protected System.Web.UI.WebControls.CheckBox chk_ConvertiPDF;
        protected System.Web.UI.WebControls.CheckBox chkRecognizeText;
        protected System.Web.UI.WebControls.CheckBox chk_cartaceo;        
		protected System.Web.UI.WebControls.RadioButton optAcquisizioneScanner;
		protected System.Web.UI.WebControls.Label lblAcquisisciDaFile;
		protected System.Web.UI.WebControls.Panel pnlRecognizeText;
		protected System.Web.UI.HtmlControls.HtmlTable tblContainer;
		protected System.Web.UI.WebControls.Label Label3;
		protected System.Web.UI.WebControls.Button btnUpload;
		protected System.Web.UI.HtmlControls.HtmlInputButton btnInvia;
		protected System.Web.UI.WebControls.Label lblAcquisisciDaScanner;
		protected Utilities.MessageBox MessageBox1;
		protected System.Web.UI.WebControls.RadioButton optAcquisisciDaFile;

        protected System.Web.UI.WebControls.RadioButton optLocale;
        protected System.Web.UI.WebControls.RadioButton optCentrale;
        protected System.Web.UI.WebControls.RadioButton optSincrona;
        protected System.Web.UI.WebControls.RadioButton optAsincrona;

        private void Page_Load(object sender, System.EventArgs e)
        {
            Response.Expires = -1;

            // Verifica che il documento corrente non sia impostato in stato checkout
            // DocumentManager.getDocumentoSelezionato(this)
            string checkOutMessage;
            if (CheckInOut.CheckInOutServices.IsCheckedOutDocument(out checkOutMessage))
            {
                this.RegisterClientScript("checkOutMessage", string.Format("alert('{0}');", checkOutMessage));
                this.RegisterClientScript("closeDialog", "window.returnValue=false; window.close();");
            }
            else
            {
                if (!this.IsPostBack)
                {
                    this.SetDefaultButton();

                    // Inizializzazione script e handler eventi controlli javascript
                    this.InitializeJavaScriptCode();

                    //Cambio la descrizione della label se è attiva la conversione PDF lato server
                    //if (DocumentPdfConvertServer == "true")
                    //    chk_ConvertiPDF.Text = "Converti in PDF lato server";                    
                }
                else
                {
                    if (this.optAcquisisciDaFile.Checked &&
                        uploadedFile.PostedFile != null)
                    {
                        string scriptStr = "<script language='javascript'>";
                        scriptStr += "window.close();</script>";

                        // Se "Converti in PDF è flaggato...
                        if (chk_ConvertiPDF.Checked && System.IO.Path.GetExtension(uploadedFile.PostedFile.FileName).ToUpper() != ".PDF")
                        {
                            // ...se la conversione richiesta è centralizzata asincrona...
                            if (this.optCentrale.Checked)
                            {
                                // ...bisogna verificare se è richiesta conversione sincrona o asincrona
                                // Nel caso di conversione sincrona...
                                if (this.optSincrona.Checked)
                                    // ...si richiede conversione sincrona
                                    FileManager.uploadFile(this, uploadedFile.PostedFile, this.chk_cartaceo.Checked, true, true);
                                else
                                {
 //modifica
                        if (!cache.Value.Equals("true"))
                        //fine modfica  
                                    // ...altrimenti si richiede la conversione asincrona e si mette il documento in checkout
                                    FileManager.uploadFile(this, uploadedFile.PostedFile, this.chk_cartaceo.Checked, true, false);
                                    
                                    CheckInOut.CheckInOutServices.RefreshCheckOutStatus();
                                }

                            }
                            else
                            {
                                // ...altrimenti bisogna semplicemente uploadare il file senza eseguire 
                                // alcuna conversione
  //modifica
                        if (!cache.Value.Equals("true"))                    
                         //fine modfica                                  
                                FileManager.uploadFile(this, uploadedFile.PostedFile, this.chk_cartaceo.Checked, false, false);

                            }

                        }
                        else
                        {
                            // Se non si è richiesta conversione pdf bisogna seplicemente fare upload del file
  //modifica
                        if (!cache.Value.Equals("true"))                    
                         //fine modfica                           
                            FileManager.uploadFile(this, uploadedFile.PostedFile, this.chk_cartaceo.Checked, false, false);
                        }

                        //if (DocsPAWA.Utils.isEnableConversionePdfLatoServer() == "true" && chk_ConvertiPDF.Checked)
                        //{
                        //    //Upload del file
                        //    FileManager.uploadFile(this, uploadedFile.PostedFile, this.chk_cartaceo.Checked, true,false);
                        //    //Aggiornamento contesto checkInOut
                        //    CheckInOut.CheckInOutServices.refreshContext(DocumentManager.getDocumentoSelezionato(this),this);
                        //}
                        //else
                        //{
                        //    FileManager.uploadFile(this, uploadedFile.PostedFile, this.chk_cartaceo.Checked, false,false);
                        //}

                        Response.Write(scriptStr);
                    }
                }
            }
        }       

        #region Reperimento configurazioni

        /// <summary>
        /// 
        /// </summary>
        protected string DocumentPdfConvert
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DOCUMENT_PDF_CONVERT"]))
                    return ConfigurationManager.AppSettings["DOCUMENT_PDF_CONVERT"].ToLower();
                else
                    return "false";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string DocumentPdfConvertEnabled
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DOCUMENT_PDF_CONVERT_ENABLED"]))
                    return ConfigurationManager.AppSettings["DOCUMENT_PDF_CONVERT_ENABLED"].ToLower();
                else
                    return "false";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string DocumentPdfConvertServer
        {
            get
            {
                if (DocsPAWA.Utils.isEnableConversionePdfLatoServer() == "true")
                {
                    //Controllo se l'acquisizione viene richiesta dal tab allegati 
                    //In questo caso deve essere disabilitata la possibilità di conversione pdf lato server
                    DocsPAWA.DocsPaWR.FileRequest fReq = FileManager.getSelectedFile(this);
                    if (fReq != null) // && !fReq.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Allegato)))
                        return "true";
                    else
                        return "false";
                }
                else
                    return "false";
            }
        }

        /// <summary>
        /// Proprietà in sola lettura per verificare se è attiva la converisone sincrona lato server
        /// </summary>
        protected string DocumentSyncPdfConvertServer
        {
            get
            {
                if (DocsPAWA.Utils.IsEbabledConversionePdfLatoServerSincrona() == "true")
                {
                    //Controllo se l'acquisizione viene richiesta dal tab allegati 
                    //In questo caso deve essere disabilitata la possibilità di conversione pdf lato server
                    DocsPAWA.DocsPaWR.FileRequest fReq = FileManager.getSelectedFile(this);
                    if (fReq != null) /*&& !fReq.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Allegato)))*/
                        return "true";
                    else
                        return "false";
                }
                else
                    return "false";
            }
        }

        /// <summary>
        /// Proprietà in sola lettura utilizzata per verificare se il documento/protocollo
        /// non è ancora stato salvato. Questa proprietà è necessaria in quanto se il documento
        /// non è stato ancora salvato, non deve essere possibile richiedere la conversione lato
        /// server asincrona
        /// </summary>
        protected string IsDocumentSaved
        {
            get
            {
                return (!String.IsNullOrEmpty(DocumentManager.getDocumentoSelezionato(this).systemId)).ToString().ToLower();
            }
        }
        
        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected bool IsActiveSmartClient
        {
            get
            {
                return SmartClient.Configurations.IsActive();
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
		}

        /// <summary>
        /// Indica se la stampa della seguatura di protocollo su scanner REI è abilitata o meno
        /// </summary>
        protected bool StampaSegnaturaAbilitata
        {
            get
            {
                DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoInLavorazione();

                if (schedaDocumento.protocollo == null)
                    return false;
                else if (schedaDocumento.documentoPrincipale != null)
                    // Acquisizione dal dettaglio dell'allegato
                    return false;
                else
                {
                    // Acquisizione dal tab "Allegati" del documento principale
                    DocsPaWR.FileRequest fileRequest = FileManager.getSelectedFile();

                    // Consente la stampa della segnatura solamente se il documento non è un allegato
                    return ((fileRequest as DocsPaWR.Allegato) == null);
                }
            }
        }

        /// <summary>
        /// Reperimento della segnatura di protocollo, necessaria per la stampa diretta su foglio tramite scanner REI
        /// </summary>
        protected string SegnaturaProtocollo
        {
            get
            {
                if (this.StampaSegnaturaAbilitata)
                {
                    DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoInLavorazione();

                    if (schedaDocumento != null && schedaDocumento.protocollo != null)
                        return schedaDocumento.protocollo.segnatura;
                    else
                        return string.Empty;
                }
                else
                    return string.Empty;
            }
        }



		/// <summary>
		/// Inizializzazione script e handler eventi controlli javascript
		/// </summary>
		private void InitializeJavaScriptCode()
		{
            // Associazione handler eventi javascript
			this.optAcquisizioneScanner.Attributes.Add("onClick","OnClickRadioAcquisizioneScanner();");
			this.optAcquisisciDaFile.Attributes.Add("onClick","OnClickRadioAcquisizioneDaFile();");
            this.chk_ConvertiPDF.Attributes.Add("onClick", "OnClickCheckConvertPdf();");
            this.optLocale.Attributes.Add("onClick", "OnClickLocaleCentralizzata();");
            this.optCentrale.Attributes.Add("onClick", "OnClickLocaleCentralizzata();");

            // Associazione valori per i check convertPdf e recognizeText
            this.RegisterClientScript("BindCheckConvertPdf", "BindCheckConvertPdf()");
            //this.RegisterClientScript("BindCheckRecognizeText", "BindCheckRecognizeText();");
            
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
			if(!this.Page.ClientScript.IsStartupScriptRegistered(this.GetType(), scriptKey))
			{
				string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), scriptKey, scriptString);
			}
		}
		
		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e) {
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
		}
		#endregion				
	}
}
