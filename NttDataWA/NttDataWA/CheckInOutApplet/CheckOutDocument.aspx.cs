using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;

namespace NttDataWA.CheckInOutApplet
{
    public partial class CheckOutDocument : System.Web.UI.Page
    {
        private DocsPaWebService _wsInstance = null;
        private const string SESSION_KEY = "SupportedFilesController.FileTypeList";
        private const string SUPPORTED_FILE_TYPES_ENABLED_SESSION_KEY = "Configurations.SupportedFileTypesEnabled";

        private string language;

        private static string documentId;
        private static string documentNumber;
        private static string supportedFileFormats;
        private static string supportedFileFormatsMaxSize;
        private static string maxFileSizeAlertMode;
        private static string documentType;
        private static string tipoDocumento;
        private static string fileExtention;
        private static string createNewFile;


        private string componentType = Constans.TYPE_APPLET;
        private string file_name;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CheckInOutServices.InitializeContext();
                if (UIManager.DocumentManager.getSelectedAttachId() == null && CheckInOutServices.CurrentSchedaDocumento != null && CheckInOutServices.CurrentSchedaDocumento.protocollo != null && CheckInOutServices.CurrentSchedaDocumento.protocollo.segnatura != null && !string.IsNullOrEmpty(CheckInOutServices.CurrentSchedaDocumento.protocollo.segnatura))
                {
                    file_name = UIManager.UserManager.normalizeStringPropertyValue(CheckInOutServices.CurrentSchedaDocumento.protocollo.segnatura);
                }
                else
                {
                    if (CheckInOutServices.CurrentSchedaDocumento != null && CheckInOutServices.CurrentSchedaDocumento.docNumber != null && !string.IsNullOrEmpty(CheckInOutServices.CurrentSchedaDocumento.docNumber))
                    {
                        if (UIManager.DocumentManager.getSelectedAttachId() != null)
                            file_name = UIManager.DocumentManager.GetSelectedAttachment().docNumber;
                        else
                            file_name = CheckInOutServices.CurrentSchedaDocumento.docNumber;
                    }
                }
                this.SetParameters();
               
                this.componentType = UIManager.UserManager.getComponentType(Request.UserAgent);
                this.initForm();
            }
        }

        private void initForm()
        {
            string confirmScript = "if (confirmAction()) parent.closeAjaxModal('CheckOutDocument','up');";
            string setTempFolderScript = "setTempFolder();";
            string selectFolderScript = "SelectFolder();";
            this.language = NttDataWA.UIManager.UserManager.GetUserLanguage();
            
            this.lblFolderPath.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalFolderPath", language);
            this.lblFileName.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalFileName", language);
            this.CheckInOutConfirmButton.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalConfirmButton", language);
            this.CheckInOutCloseButton.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalCloseButton", language);

             NttDataWA.DocsPaWR.SchedaDocumento schedaDocumento = UIManager.DocumentManager.getSelectedRecord();

                string nomeOriginale = null;

                if (schedaDocumento != null)
                {
                    FileRequest fileReq = null;
                    if (FileManager.GetSelectedAttachment() == null)
                        fileReq = UIManager.FileManager.getSelectedFile();
                    else
                    {
                        fileReq = FileManager.GetSelectedAttachment();
                    }
                    FileDocumento doc = FileManager.getInstance(schedaDocumento.systemId).getInfoFile(this.Page, fileReq);
                    if (doc != null && !string.IsNullOrEmpty(doc.nomeOriginale))
                    {
                       nomeOriginale = doc.nomeOriginale;
                    }
                }

                if (!string.IsNullOrEmpty(nomeOriginale))
                {
                    this.txtFileName.Text = cleanFileName(nomeOriginale);
                }
                else
                {
                    this.txtFileName.Text = file_name;
                }


                if(this.componentType.Equals(Constans.TYPE_SOCKET)){
                    confirmScript = "disallowOp('Content1'); confirmActionSocket(function(close){ reallowOp(); if(close)parent.closeAjaxModal('CheckOutDocument','up');})";
                    setTempFolderScript = "setTempFolderSocket();";
                    selectFolderScript = "SelectFolderSocket()";
                    this.pnlAppletTag.Visible = false;
                }
                this.CheckInOutConfirmButton.OnClientClick = confirmScript;
                this.btnBrowseForFolder.OnClientClick = selectFolderScript;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "InitializeCtrlScript", setTempFolderScript, true);
            //file_name;
        }

        private string cleanFileName(string inputFile)
        {
            string exts = GetP7mFileExtensions();
            if (!String.IsNullOrEmpty(exts))
                return inputFile.Replace(exts, "");
            else
                return Path.GetFileNameWithoutExtension(inputFile);
        }

        private void SetParameters()
        {
            SchedaDocumento selDoc = CheckInOutServices.CurrentSchedaDocumento; // CurrentSchedaDocumento();
            if (selDoc != null) 
            {
                DocumentId = selDoc.systemId;

                if (UIManager.DocumentManager.getSelectedAttachId() != null)
                {
                    DocumentNumber = UIManager.DocumentManager.GetSelectedAttachment().docNumber;
                }
                else 
                {
                    DocumentNumber = selDoc.docNumber;
                }

                FileExtention = LastAcquiredDocumentExtension(selDoc);
                CreateNewFile = (string.IsNullOrEmpty(FileExtention)?"true":"false");
                if (selDoc.tipoProto == "G")
                    TipoDocumento = "G";
                else 
                    TipoDocumento = "P";

                this.Fetch();
                this.FetchFileTypes();
            }
        }

        private void FetchFileTypes()
        {
            // Reperimento modelli di documento
            string[] types = GetDocumentModelTypes();

            this.cboFileTypes.Visible = (types.Length != 1);
            this.lblFileType.Visible = !this.cboFileTypes.Visible;

            if (this.cboFileTypes.Visible)
            {
                foreach (string item in types)
                    this.cboFileTypes.Items.Add(item);
            }

            else if (types.Length > 0)
                this.lblFileType.Text = types[0];

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
                if (pptItem != null)
                {
                    this.cboFileTypes.SelectedValue = pptItem.Value;
                }
                this.cboFileTypes.Enabled = false;
                
            }
        }

        /// <summary>
        /// Reperimento tipologie di modelli di documento disponibili
        /// </summary>
        /// <returns></returns>
        private string[] GetDocumentModelTypes()
        {
            string[] retValue = null;

            //DocsPaWR.FileRequest fileInfo = (UIManager.DocumentManager.getSelectedAttachId() != null) ?
            //        UIManager.FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId()) :
            //            UIManager.FileManager.GetFileRequest();
            DocsPaWR.FileRequest fileInfo = new FileRequest();

            if (UIManager.DocumentManager.getSelectedAttachId() != null)
            {
                fileInfo = UIManager.FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId());
            }
            else
            {
                fileInfo= (from doc in CheckInOutServices.CurrentSchedaDocumento.documenti where doc.version.Equals(DocumentManager.getSelectedNumberVersion()) select doc).FirstOrDefault();
            }

            string fileName = string.Empty;

            if (fileInfo!=null)
                fileName = fileInfo.fileName;

            // Verifica se il file è già stato acquisito
            bool isFileAcquired = (!string.IsNullOrEmpty(fileName));

            if (isFileAcquired)
            {
                // Se il file è già stato acquisito, viene proposta
                // l'estensione del file fornito in querystring

                string extension = (fileInfo.fileName.Split('.').Length > 1) ? (fileInfo.fileName.Split('.'))[fileInfo.fileName.Split('.').Length - 1] : string.Empty;
                retValue = new string[1] { extension };
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

        private void HydeExtension()
        {
            this.lblFileType.Visible = false;
            this.cboFileTypes.Visible = false;
        }


        public static int FileAcquireSizeMax
        {
            get
            {
                int maxFileSize = 0;

                if (!SupportedFileTypesEnabled)
                    Int32.TryParse(ConfigurationManager.AppSettings[NttDataWA.Utils.WebConfigKeys.FILE_ACQ_SIZE_MAX.ToString()], out maxFileSize);

                if (maxFileSize == 0)
                    maxFileSize = Int32.MaxValue;

                return maxFileSize;
            }
        }

        /// <summary>
        /// Reperimento tipologia del documento corrente
        /// </summary>
        protected string TipoDocumento
        {
            get
            {
                return tipoDocumento;
            }
            set
            {
                tipoDocumento = value;
            }
        }

        /// <summary>
        /// Reperimento metadati sui tipi di file supportati dall'amministrazione
        /// </summary>
        /// <returns></returns>
        private SupportedFileType[] GetSupportedFileTypes()
        {
            if (Session[SESSION_KEY] == null)
                Session.Add(SESSION_KEY, this.WSInstance.GetSupportedFileTypes(Convert.ToInt32(UserManager.GetInfoUser().idAmministrazione)));
            return Session[SESSION_KEY] as SupportedFileType[];
        }

        /// <summary>
        /// Reperimento instanza webservice
        /// </summary>
        private DocsPaWebService WSInstance
        {
            get
            {
                if (this._wsInstance == null)
                    this._wsInstance = new DocsPaWebService();
                return this._wsInstance;
            }
        }

        /// <summary>
        /// Verifica se risulta abilitata la gestione dei tipi file supportati
        /// </summary>
        public static bool SupportedFileTypesEnabled
        {
            get
            {
                if (HttpContext.Current.Session[SUPPORTED_FILE_TYPES_ENABLED_SESSION_KEY] == null)
                {
                    DocsPaWebService ws = new DocsPaWebService();
                    HttpContext.Current.Session.Add(SUPPORTED_FILE_TYPES_ENABLED_SESSION_KEY, ws.IsEnabledSupportedFileTypes());
                }

                return (bool)HttpContext.Current.Session[SUPPORTED_FILE_TYPES_ENABLED_SESSION_KEY];
            }
        }

        public static string CreateNewFile
        {
            get
            {
                return createNewFile;
            }
            set
            {
                createNewFile = value;
            }
        }

        public static string FileExtention
        {
            get
            {
                return fileExtention;
            }
            set
            {
                fileExtention = value;
            }
        }
        /// <summary>
        /// Reperimento nome computer
        /// </summary>
        public static string MachineName
        {
            get
            {
                string machineName = string.Empty;

                string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                if (ipAddress != null && ipAddress != string.Empty)
                {
                    try
                    {
                        System.Net.IPHostEntry entry = System.Net.Dns.GetHostByAddress(ipAddress);
                        machineName = entry.HostName;
                    }
                    catch
                    {
                        machineName = ipAddress;
                    }
                }

                return machineName;
            }
        }
        
        public static string httpFullPath
        {
            get
            {
                return utils.getHttpFullPath();
            }
        }
        
        /// <summary>
        /// Reperimento dell'estensione del file acquisito
        /// relativamente all'ultima versione del documento
        /// </summary>
        /// <returns></returns>
        protected string LastAcquiredDocumentExtension(SchedaDocumento currentScheda)
        {
            string retValue = string.Empty;

            if (currentScheda != null && currentScheda.documenti != null && currentScheda.documenti.Length > 0)
            {
                
                if (this.ViewState["LastAcquiredDocumentExtension"] == null || string.IsNullOrEmpty(this.ViewState["LastAcquiredDocumentExtension"].ToString()))
                {
                    if (CheckInOutServices.UserEnabled)
                    {
                        FileRequest lastDocument = new FileRequest();
                        // modifica per reperire le versioni antecedenti l'ultima
                        //if (DocumentManager.ListDocVersions != null)
                        //{
                        //    lastDocument = (from doc in DocumentManager.ListDocVersions where doc.version.Equals(DocumentManager.getSelectedNumberVersion()) select doc).FirstOrDefault();
                        //}
                        //else
                        //{
                        //    lastDocument = currentScheda.documenti[0];
                        //}

                        // provo con l'array già presente
                        lastDocument= (from doc in currentScheda.documenti where doc.version.Equals(DocumentManager.getSelectedNumberVersion()) select doc).FirstOrDefault();

                        // Reperimento ultimo file acquisito dal documento

                        //FileRequest lastDocument = currentScheda.documenti[0];

                        // Verifica se è stato acquisito un file
                        if (this.IsAcquired(lastDocument))
                        {
                            // Reperimento oggetto "FileDocumento" contenente
                            // il nome originale del file, indipendentemente 
                            // dal documentale utilizzato correntemente
                            FileManager fileManager = new FileManager();
                            DocsPaWR.FileDocumento fileDocument = fileManager.getInfoFile(this.Page, lastDocument);

                            string defaultFileName = fileDocument.name;

                            // In caso di file firmato, viene reperita l'estensione originaria del file
                            while (defaultFileName.ToLower().EndsWith(".p7m"))
                                defaultFileName = defaultFileName.Substring(0, defaultFileName.Length - 4);

                            FileInfo fileInfo = new FileInfo(defaultFileName);
                            retValue = fileInfo.Extension.Replace(".", "");
                        }
                    }

                    this.ViewState.Add("LastAcquiredDocumentExtension", retValue);
                }
                else
                {
                    retValue = this.ViewState["LastAcquiredDocumentExtension"].ToString();
                }
            }
            return retValue;
        }

        private void Fetch()
        {
            if (SupportedFileTypesEnabled)
            {
                SupportedFileType[] fileTypes = this.GetSupportedFileTypes();

                List<string> list = new List<string>();

                string fileFormats = string.Empty;
                string fileFormatsMaxSize = string.Empty;
                string maxFileSizeAlertMode = string.Empty;
                string documentType = string.Empty;

                foreach (SupportedFileType fileType in fileTypes)
                {
                    if (fileFormats != string.Empty)
                        fileFormats += "|";
                    fileFormats += fileType.FileExtension + ":" + fileType.FileTypeUsed.ToString();

                    if (fileFormatsMaxSize != string.Empty)
                        fileFormatsMaxSize += "|";
                    fileFormatsMaxSize += fileType.MaxFileSize;

                    if (maxFileSizeAlertMode != string.Empty)
                        maxFileSizeAlertMode += "|";
                    maxFileSizeAlertMode += fileType.MaxFileSizeAlertMode.ToString();

                    if (documentType != string.Empty)
                        documentType += "|";
                    documentType += fileType.DocumentType.ToString();
                }
           
                SupportedFileFormats = fileFormats;
                SupportedFileFormatsMaxSize = fileFormatsMaxSize;
                MaxFileSizeAlertMode = maxFileSizeAlertMode;
                DocumentType = documentType;
            }
        }

        /// <summary>
        /// Reperimento dell'estensione completa del file nel caso in cui sia P7M
        /// </summary>
        /// <returns></returns>
        protected string GetP7mFileExtensions()
        {
            string extensions = string.Empty;

            DocsPaWR.FileRequest fileInfo = (UIManager.DocumentManager.getSelectedAttachId() != null) ?
                    UIManager.FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId()) :
                        UIManager.FileManager.GetFileRequest();
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

        public static string DocumentId
        {
            get
            {
                return documentId;
            }
            set
            {
                documentId = value;
            }
        }

        public static string DocumentNumber
        {
            get
            {
                return documentNumber;
            }
            set
            {
                documentNumber = value;
            }
        }

        public static string SupportedFileFormats
        {
            get
            {
                return supportedFileFormats;
            }
            set
            {
                supportedFileFormats = value;
            }
        }

        public static string SupportedFileFormatsMaxSize
        {
            get
            {
                return supportedFileFormatsMaxSize;
            }
            set
            {
                supportedFileFormatsMaxSize = value;
            }
        }

        public static string MaxFileSizeAlertMode
        {
            get
            {
                return maxFileSizeAlertMode;
            }
            set
            {
                maxFileSizeAlertMode = value;
            }
        }

        public static string DocumentType
        {
            get
            {
                return documentType;
            }
            set
            {
                documentType = value;
            }
        }

        /// <summary>
        /// Messaggi di errore e avvisi
        /// </summary>
        protected string GetMessage(string messageType)
        {
            string message = "";

            switch (messageType)
            {
                case "NO_ACCESS":
                    message = Utils.Languages.GetMessageFromCode("Path_AccessDenied", language);
                    break;
                case "NOT_EXIST":
                    message = Utils.Languages.GetMessageFromCode("Path_Nonexistent", language);
                    break;
                case "NO_NAME":
                    message = Utils.Languages.GetMessageFromCode("File_InvalidName", language);
                    break;
                case "SAVE_ERROR":
                    message = Utils.Languages.GetMessageFromCode("File_SaveError", language);
                    break;
                case "DOWNLOAD_ERROR":
                    message = Utils.Languages.GetMessageFromCode("File_DownloadError", language);
                    break;
                case "SELECT_PATH":
                    message = Utils.Languages.GetMessageFromCode("Path_Select", language);
                    break;
                case "CREATE_PATH":
                    message = Utils.Languages.GetMessageFromCode("Path_CreateIt", language);
                    break;
            }

            return message;
        }
        /// <summary>
        /// Verifica se un documento è acquisito
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        private bool IsAcquired(FileRequest fileRequest)
        {
            return (fileRequest != null &&
                    fileRequest.fileName != null &&
                    fileRequest.fileName != string.Empty &&
                    fileRequest.fileSize != null &&
                    fileRequest.fileSize != "0");
        }

        protected void CheckInOutConfirmButton_Click(object sender, EventArgs e)
        {
            //ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "confirmAJM", "return confirmAction(); parent.closeAjaxModal('CheckOutDocument','up');", true);
        }

        protected void CheckInOutCloseButton_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "closeAJM", "parent.closeAjaxModal('CheckOutDocument','up');", true);
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