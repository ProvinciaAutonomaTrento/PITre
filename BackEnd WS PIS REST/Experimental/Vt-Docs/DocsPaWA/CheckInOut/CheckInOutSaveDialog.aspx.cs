using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DocsPAWA.DocsPaWR;
using System.Text;

namespace DocsPAWA.CheckInOut
{
	/// <summary>
	/// Pagina utilizzata per acquisire dall'utente il percorso
	/// del file in cui bloccare il documento
	/// </summary>
	public class CheckInOutSaveDialog : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.TextBox txtFolderPath;
		protected System.Web.UI.WebControls.Label lblFolderPath;
		protected System.Web.UI.WebControls.Button btnBrowseForFolder;
		protected System.Web.UI.WebControls.TextBox txtFileName;
		protected System.Web.UI.WebControls.Label lblFileName;
		protected System.Web.UI.WebControls.Button btnOk;
		protected System.Web.UI.WebControls.Button btnCancel;
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.WebControls.DropDownList cboFileTypes;
		protected System.Web.UI.WebControls.Label lblFileType;
		protected System.Web.UI.HtmlControls.HtmlInputHidden txtFirstInvalidControlID;
		protected System.Web.UI.HtmlControls.HtmlTable tblContainer;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires=-1;

			// Impostazione pulsante di default
			this.SetDefaultButton();

			if (!this.IsPostBack)
			{
				// Caricamento tipi di file
				this.FetchFileTypes();

				// Ripristino dati salvati sul client relativamente a:
				// - percorso in cui è stato salvato l'ultima volta il file in checkout
				// - nome dell'ultimo file in checkout
				this.RegisterClientScript("RestoreClientData","RestoreClientData();");

				// Inizializzazione script client
				this.InitializeClientScript();
                string nomeOriginale = null;
                DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato();

                if (schedaDocumento != null)
                {
                    FileDocumento fd = new FileManager().getInfoFile(this.Page);
                    if (fd != null)
                        nomeOriginale = fd.nomeOriginale;
                }
                if (schedaDocumento != null && schedaDocumento.protocollo != null && schedaDocumento.protocollo.segnatura != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.segnatura))
                {
                    if (string.IsNullOrEmpty(nomeOriginale))
                    {
                        string nome_file_segnatura = schedaDocumento.protocollo.segnatura;
                        this.txtFileName.Text = DocsPAWA.UserManager.normalizeStringPropertyValue(nome_file_segnatura);
                    }
                    else
                        this.txtFileName.Text = cleanFileName(nomeOriginale);
                }
                else
                {
                    if (schedaDocumento != null && schedaDocumento.docNumber != null && !string.IsNullOrEmpty(schedaDocumento.docNumber))
                    {
                        if (string.IsNullOrEmpty ( nomeOriginale ))
                            this.txtFileName.Text = schedaDocumento.docNumber;
                        else
                            this.txtFileName.Text = cleanFileName(nomeOriginale);
                    }
                }
			}
		}

        private string cleanFileName (string inputFile)
        {
            string exts = GetP7mFileExtensions();
            if (!String.IsNullOrEmpty(exts))
                return inputFile.Replace(exts, "");
            else
                return Path.GetFileNameWithoutExtension(inputFile);
        }

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			this.SetControlFocus();
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
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new EventHandler(this.Page_PreRender);
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		private void SetControlFocus()
		{
			if (this.txtFirstInvalidControlID.Value!=string.Empty)
			{
				this.RegisterClientScript("SetControlFocus","SetControlFocus('" + this.txtFirstInvalidControlID.Value + "');");

				this.txtFirstInvalidControlID.Value=string.Empty;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        protected string DialogTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Request.QueryString["title"]))
                    return this.Request.QueryString["title"];
                else
                    return string.Empty;
            }
        }

		/// <summary>
		/// Caricamento tipi di file
		/// </summary>
		private void FetchFileTypes()
		{
			// Reperimento modelli di documento
			string[] types=GetDocumentModelTypes();
			
			this.cboFileTypes.Visible=(types.Length != 1);
			this.lblFileType.Visible=!this.cboFileTypes.Visible;
            
            if (this.cboFileTypes.Visible)
            {
                foreach (string item in types)
                    this.cboFileTypes.Items.Add(item);
            }

            else if (types.Length > 0)
                this.lblFileType.Text = types[0];
		}

		/// <summary>
		/// Reperimento tipologie di modelli di documento disponibili
		/// </summary>
		/// <returns></returns>
		private string[] GetDocumentModelTypes()
		{
			string[] retValue=null;

			// Verifica se il file è già stato acquisito
			bool isFileAcquired=(Request["fileName"]!=null && 
								 Request["fileName"]!=string.Empty);
				
			if (isFileAcquired)
			{
				// Se il file è già stato acquisito, viene proposta
				// l'estensione del file fornito in querystring
				FileInfo fileInfo=new FileInfo(Request["fileName"]);
				retValue=new string[1] { fileInfo.Extension.Replace(".","") };
			}
			else
			{	
				if (Request["fileType"]!=null && Request["fileType"]!=string.Empty)
				{
					// Vengono forniti i modelli eventualmente forniti in querystring
					retValue=Request["fileType"].Split('|');
				}
				else
				{
					// Vengono forniti i modelli disponibili nel sistema
					DocsPaWR.DocsPaWebService ws=new DocsPAWA.DocsPaWR.DocsPaWebService();
                    int idAdmin = Convert.ToInt32(UserManager.getUtente().idAmministrazione);
                    retValue = ws.GetCheckOutDocumentModelTypes(idAdmin);
				}
			}

			return retValue;
		}

        /// <summary>
        /// Reperimento dell'estensione completa del file nel caso in cui sia P7M
        /// </summary>
        /// <returns></returns>
        protected string GetP7mFileExtensions()
        {
            string extensions = string.Empty;

            DocsPaWR.FileRequest fileInfo = FileManager.getSelectedFile();
            //DocsPaWR.FileDocumento fileInfo = SaveFileServices.GetFileInfo();

            string fileName = fileInfo.fileName;

            if (Path.GetExtension(fileName).ToUpperInvariant() == ".P7M")
            {
                while (!string.IsNullOrEmpty(Path.GetExtension(fileName)))
                {
                    string ext = Path.GetExtension(fileName);

                    if (!string.IsNullOrEmpty(ext))
                        extensions = ext + extensions;

                    fileName = Path.GetFileNameWithoutExtension(fileName);
                }
            }

            return extensions;
        }

		/// <summary>
		/// Inizializzazione script client
		/// </summary>
		private void InitializeClientScript()
		{
			this.btnBrowseForFolder.Attributes.Add("onClick","PerformSelectFolder()");
			this.btnOk.Attributes.Add("onClick","ClosePage(true);");
			this.btnCancel.Attributes.Add("onClick","ClosePage(false);");
		}

		/// <summary>
		/// Impostazione pulsante di default
		/// </summary>
		public void SetDefaultButton()
		{
			Utils.DefaultButton(this,ref this.txtFolderPath,ref this.btnOk);
			Utils.DefaultButton(this,ref this.txtFileName,ref this.btnOk);
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
				this.Page.RegisterStartupScript(scriptKey, scriptString);
			}
		}

		private void btnOk_Click(object sender, System.EventArgs e)
		{
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
		}

        /// <summary>
        /// Creazione link per l'accesso diretto al documento dall'esterno
        /// </summary>
        protected string Link
        {
            get
            {
                StringBuilder link = new StringBuilder(Utils.getHttpFullPath() +
                   "/visualizzaLink.aspx?groupId=" +
                   UserManager.getInfoUtente().idGruppo +
                   "&docNumber=" +
                   DocumentManager.getDocumentoSelezionato().docNumber +
                   "&idProfile=" + DocumentManager.getDocumentoSelezionato().systemId +
                   "&numVersion="
                   );

                if (DocsPAWA.FileManager.getSelectedFile() == null || String.IsNullOrEmpty(FileManager.getSelectedFile().version))
                    link.Append("");
                else
                    link.Append(FileManager.getSelectedFile().version);

                return link.ToString();
            }
        }

        /// <summary>
        /// Creazione link per l'accesso diretto alla scheda documento dall'esterno
        /// </summary>
        protected string LinkSD
        {
            get
            {
                string link = String.Format("{0}/visualizzaOggetto.htm?idAmministrazione={1}&tipoOggetto=D&idObj={2}&tipoProto={3}",
                    Utils.getHttpFullPath(),
                    UserManager.getUtente().idAmministrazione,
                    DocumentManager.getDocumentoSelezionato().systemId,
                    DocumentManager.getDocumentoSelezionato().tipoProto);

                return link;
            }
        }

        /// <summary>
        /// Recupero del numero di documento
        /// </summary>
        protected string DocNumber
        {
            get
            {
                return DocumentManager.getDocumentoSelezionato().docNumber;
            }
        }

        /// <summary>
        /// Creazione del link path completo dell'icona
        /// </summary>
        protected string FileIcona
        {
            get
            {
                return Utils.getHttpFullPath() + "/images/favicon.ico";
            }
        }

	}
}
