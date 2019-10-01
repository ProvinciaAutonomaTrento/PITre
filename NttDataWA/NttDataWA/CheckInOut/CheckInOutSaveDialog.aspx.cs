using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using NttDataWA.DocsPaWR;
using System.Text;
using NttDataWA.UIManager;

namespace NttDataWA.CheckInOut
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
                this.InitializeLabel();

				// Caricamento tipi di file
				this.FetchFileTypes();

				// Ripristino dati salvati sul client relativamente a:
				// - percorso in cui è stato salvato l'ultima volta il file in checkout
				// - nome dell'ultimo file in checkout
				this.RegisterClientScript("RestoreClientData","RestoreClientData();");

				// Inizializzazione script client
				this.InitializeClientScript();

                NttDataWA.DocsPaWR.SchedaDocumento schedaDocumento = UIManager.DocumentManager.getSelectedRecord();

                string nomeOriginale = null;

                //if (schedaDocumento != null)
                //{
                //    FileDocumento fd = new FileManager().getInfoFile(this.Page);
                //    if (fd != null)
                //        nomeOriginale = fd.nomeOriginale;
                //}

                if (schedaDocumento != null)
                {

                    /*
                    FileRequest fileReq = null;
                    if (FileManager.GetSelectedAttachment() == null)
                        fileReq = UIManager.FileManager.getSelectedFile();
                    else
                    {
                        fileReq = FileManager.GetSelectedAttachment();
                    }
                    */

                    String selectedVersionId = null;

                    if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
                        selectedVersionId = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v.versionId).FirstOrDefault();

                    DocsPaWR.FileRequest fileReq = (UIManager.DocumentManager.getSelectedAttachId() != null) ?
                            UIManager.FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId()) :
                                UIManager.FileManager.GetFileRequest(selectedVersionId);

                    FileDocumento doc = FileManager.getInstance(schedaDocumento.systemId).getInfoFile(this.Page, fileReq);
                    if (doc != null && !string.IsNullOrEmpty(doc.nomeOriginale))
                    {
                       nomeOriginale = doc.nomeOriginale;
                    }
                    
                }


                if (UIManager.DocumentManager.getSelectedAttachId() == null && schedaDocumento != null && schedaDocumento.protocollo != null &&  schedaDocumento.protocollo.segnatura!= null && !string.IsNullOrEmpty(schedaDocumento.protocollo.segnatura))
                {
                    if (string.IsNullOrEmpty(nomeOriginale))
                    {
                        string nome_file_segnatura = schedaDocumento.protocollo.segnatura;
                        this.txtFileName.Text = UserManager.normalizeStringPropertyValue(nome_file_segnatura);
                    }
                    else
                        this.txtFileName.Text = cleanFileName(nomeOriginale);
                }
                else
                {
                    if (schedaDocumento != null && schedaDocumento.docNumber != null && !string.IsNullOrEmpty(schedaDocumento.docNumber))
                    {
                        if (string.IsNullOrEmpty(nomeOriginale))
                        {
                            this.txtFileName.Text = (UIManager.DocumentManager.getSelectedAttachId() != null ? UIManager.DocumentManager.getSelectedAttachId() : schedaDocumento.docNumber);
                        }
                        else
                        {
                             this.txtFileName.Text = cleanFileName(nomeOriginale);
                        }
                    }
                }

                if (this.modelType != null && (this.modelType.ToUpper().Equals("PPT") || this.modelType.ToUpper().Equals("PPTX")))
                {
                    ListItem pptItem = null;
                    foreach (ListItem item in this.cboFileTypes.Items)
                    {
                        if (item.Text.ToUpper() == "PPT")
                        {
                            pptItem = item;
                            break;
                        }
                    }
                    this.cboFileTypes.SelectedValue = pptItem.Value;
                    this.cboFileTypes.Enabled = false;
                }
			}
		}

        private string cleanFileName(string inputFile)
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

        private void InitializeLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.lblFolderPath.Text = Utils.Languages.GetLabelFromCode("CheckInOutSaveLocalFolderPath", language);
            this.lblFileName.Text = Utils.Languages.GetLabelFromCode("CheckInOutSaveLocalFileName", language);
            this.btnCancel.Text = Utils.Languages.GetLabelFromCode("CheckInOutUndoCloseButton", language);

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
                if (!string.IsNullOrEmpty(this.modelType) && this.modelType.ToUpper().Equals("PPT"))
                    return "Apri modello documento";
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
            String selectedVersionId = null;

            if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
                selectedVersionId = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v.versionId).FirstOrDefault();

            DocsPaWR.FileRequest fileInfo =(UIManager.DocumentManager.getSelectedAttachId() != null) ?
                    UIManager.FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId()) :
                        UIManager.FileManager.GetFileRequest(selectedVersionId);
            string fileName = fileInfo != null ? fileInfo.fileName : string.Empty;

            string currExtList = GetCurrentFileExtensionList(fileInfo);

            // Verifica se il file è già stato acquisito
            bool isFileAcquired = (!string.IsNullOrEmpty(fileName));

            //if (isFileAcquired)
            //{
            //    if (Request["fileType"] != null && Request["fileType"] != string.Empty)
            //    {
            //        // Vengono forniti i modelli eventualmente forniti in querystring
            //        retValue = Request["fileType"].Split('|');
            //    }
            //    else
            //    {
            //        string extension = (fileInfo.fileName.Split('.').Length > 1) ? (fileInfo.fileName.Split('.'))[fileInfo.fileName.Split('.').Length - 1] : string.Empty;
            //        retValue = new string[1] { extension };
            //    }
            //}
            //else
            //{	
            //    if (Request["fileType"]!=null && Request["fileType"]!=string.Empty)
            //    {
            //        // Vengono forniti i modelli eventualmente forniti in querystring
            //        retValue=Request["fileType"].Split('|');
            //    }
            //    else
            //    {
            //        // Vengono forniti i modelli disponibili nel sistema
            //        DocsPaWR.DocsPaWebService ws=new NttDataWA.DocsPaWR.DocsPaWebService();
            //        int idAdmin = Convert.ToInt32(UIManager.UserManager.GetInfoUser().idAmministrazione);
            //        retValue = ws.GetCheckOutDocumentModelTypes(idAdmin);
            //    }
            //}

            if (isFileAcquired)
            {
                // Se il file è già stato acquisito, viene proposta
                // l'estensione del file fornito in querystring
                if (string.IsNullOrEmpty(currExtList))
                {
                    string extension = (fileInfo.fileName.Split('.').Length > 1) ? (fileInfo.fileName.Split('.'))[fileInfo.fileName.Split('.').Length - 1] : string.Empty;
                    retValue = new string[1] { extension };
                }
                else
                {
                    retValue = currExtList.Split('|');
                }
            }
            else
            {
                if (Request["fileType"] != null && Request["fileType"] != string.Empty)
                {
                    // Vengono forniti i modelli eventualmente forniti in querystring
                    retValue = Request["fileType"].Split('|');
                }
                else
                {
                    // Vengono forniti i modelli disponibili nel sistema
                    DocsPaWR.DocsPaWebService ws = new NttDataWA.DocsPaWR.DocsPaWebService();













                    int idAdmin = Convert.ToInt32(UIManager.UserManager.GetInfoUser().idAmministrazione);
                    retValue = ws.GetCheckOutDocumentModelTypes(idAdmin);
                }
            }

            return retValue;
        }

        //GIANGI NEW
        private string GetCurrentFileExtensionList(DocsPaWR.FileRequest filerq)
        {
            string retValue = string.Empty;

            DocsPaWR.FileDocumento fileDocument = new FileManager().getInfoFile(this.Page, filerq);

            if (fileDocument != null)
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileDocument.fullName);

                retValue = fileInfo.Extension.Replace(".", string.Empty);

                if (retValue.ToUpper() == "P7M"
                    || retValue.ToUpper() == "TSD" /*||
                    retValue.ToUpper() == "M7M"*/
                    )
                {
                    retValue += "|" + getEstensioneIntoSignedFile(fileDocument.fullName).ToUpper();
                }
            }

            return retValue;
        }

        private string getEstensioneIntoSignedFile(string fullname)
        {
            string retValue = string.Empty;
            // Reperimento del nome del file con estensione
            string fileName = new System.IO.FileInfo(fullname).Name;

            string[] items = fileName.Split('.');

            // SKIZZO decommentare per icona giusta.
            for (int i = (items.Length - 1); i >= 0; i--)
            {
                if (!(items[i].ToUpper().EndsWith("P7M")
                    || items[i].ToUpper().EndsWith("TSD") ||
                    items[i].ToUpper().EndsWith("M7M")
                    ))
                {
                    retValue = items[i];
                    break;
                }
            }

            return retValue;
        }
        //END NEW

        /// <summary>
        /// Reperimento dell'estensione completa del file nel caso in cui sia P7M
        /// </summary>
        /// <returns></returns>
        protected string GetP7mFileExtensions()
        {
            string extensions = string.Empty;

            NttDataWA.DocsPaWR.SchedaDocumento schedaDocumento = UIManager.DocumentManager.getSelectedRecord();

            /*
            DocsPaWR.FileRequest fileInfo =(UIManager.DocumentManager.getSelectedAttachId() != null) ?
                    UIManager.FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId()) :
                        UIManager.FileManager.GetFileRequest();
               */

            String selectedVersionId = null;

            if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
                selectedVersionId = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v.versionId).FirstOrDefault();

            DocsPaWR.FileRequest fileInfo = (UIManager.DocumentManager.getSelectedAttachId() != null) ?
                    UIManager.FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId()) :
                        UIManager.FileManager.GetFileRequest(selectedVersionId);

            //DocsPaWR.FileDocumento fileInfo = SaveFileServices.GetFileInfo();
            FileDocumento doc = FileManager.getInstance(schedaDocumento.systemId).getInfoFile(this.Page, fileInfo);


            //string fileName = fileInfo.fileName;
            //string fileName = doc.nomeOriginale != null ? doc.nomeOriginale : string.Empty;

            //INC000001126833 APSS: problema con riproponi su documenti tipizzati con modelli
            string fileName = string.Empty;
            if (doc != null)
            {
                fileName = doc.nomeOriginale;
                if (string.IsNullOrEmpty(fileName))
                    fileName = doc.name;
            }
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
			Utils.utils.DefaultButton(this,ref this.txtFolderPath,ref this.btnOk);
			Utils.utils.DefaultButton(this,ref this.txtFileName,ref this.btnOk);
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
                StringBuilder link = new StringBuilder(Utils.utils.getHttpFullPath() +
                   "/CheckInOut/OpenDirectLink.aspx?groupId=" +
                   UIManager.UserManager.GetInfoUser().idGruppo +
                   "&docNumber=" +
                   UIManager.DocumentManager.getSelectedRecord().docNumber +
                   "&idProfile=" + UIManager.DocumentManager.getSelectedRecord().systemId +
                   "&from=file&numVersion="
                   );

                if (UIManager.FileManager.getSelectedFile() == null || String.IsNullOrEmpty(UIManager.FileManager.getSelectedFile().version))
                    link.Append("");
                else
                    link.Append(UIManager.FileManager.getSelectedFile().version);

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
                StringBuilder link = new StringBuilder(Utils.utils.getHttpFullPath() +
                   "/CheckInOut/OpenDirectLink.aspx?idAmministrazione=" + UIManager.UserManager.GetInfoUser().idAmministrazione +
                   "&idObj=" + UIManager.DocumentManager.getSelectedRecord().systemId +
                   "&tipoProto=" + UIManager.DocumentManager.getSelectedRecord().tipoProto + "&from=record");

                return link.ToString();
            }
        }

        /// <summary>
        /// Recupero del numero di documento
        /// </summary>
        protected string DocNumber
        {
            get
            {
                return UIManager.DocumentManager.getSelectedRecord().docNumber;
            }
        }

        /// <summary>
        /// Creazione del link path completo dell'icona
        /// </summary>
        protected string FileIcona
        {
            get
            {
                return Utils.utils.getHttpFullPath() + "/images/favicon.ico";
            }
        }

        public string modelType
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["templateModelType"] != null)
                {
                    result = HttpContext.Current.Session["templateModelType"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["templateModelType"] = value;
            }
        }

	}
}
