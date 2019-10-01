using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.CheckInOutApplet
{
    public partial class CheckInOutSaveLocal : System.Web.UI.Page
    {
        private const string OPT_FILE_SYSTEM = "FS";
        private const string OPT_PACKAGE = "PKG";
        private const string OPT_CLIP_BOARD = "CL";
        private const string OPT_CLIP_BOARD_SD = "CLSD";
        private const string OPT_URL = "URL";
        private const string OPT_URL_SD = "URLSD";

        private string language;
        private string file_name;
        public static string httpFullPath;

        private string componentType = Constans.TYPE_APPLET;

        private NttDataWA.DocsPaWR.SchedaDocumento schedaDocumento;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            httpFullPath = utils.getHttpFullPath();

            if (!this.IsPostBack)
            {
                this.FetchFileTypes();

                this.schedaDocumento = UIManager.DocumentManager.getSelectedRecord();

                if(this.schedaDocumento.documentoPrincipale != null)
                {
                    rblListSavingOption.Items.RemoveAt(1);
                }






                if (UIManager.DocumentManager.getSelectedAttachId() == null && this.schedaDocumento != null && this.schedaDocumento.protocollo != null && !string.IsNullOrEmpty(this.schedaDocumento.protocollo.segnatura))
                {
                    file_name = UIManager.UserManager.normalizeStringPropertyValue(this.schedaDocumento.protocollo.segnatura);
                }
                else
                {
                    if (this.schedaDocumento != null && this.schedaDocumento.docNumber != null && !string.IsNullOrEmpty(this.schedaDocumento.docNumber))
                    {
                        String selectedVersionId = null;

                        if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
                            selectedVersionId = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v.versionId).FirstOrDefault();

                        DocsPaWR.FileRequest fileReq = (UIManager.DocumentManager.getSelectedAttachId() != null) ?
                                UIManager.FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId()) :
                                    UIManager.FileManager.GetFileRequest(selectedVersionId);

                        FileDocumento doc = FileManager.getInstance(this.schedaDocumento.systemId).getInfoFile(this.Page, fileReq);
                        if (doc != null && !string.IsNullOrEmpty(doc.nomeOriginale))
                        {
                            file_name = cleanFileName(doc.nomeOriginale);
                        }
                        else
                        {
                            file_name = this.schedaDocumento.docNumber;
                        }
                        //file_name = (UIManager.DocumentManager.getSelectedAttachId() != null ? UIManager.DocumentManager.getSelectedAttachId() : this.schedaDocumento.docNumber);
                    }
                }

                this.initForm();
            }
            
        }

        private void initForm()
        {
            string selectFolderScript = "SelectFolder();";
            string setTempFolderScript = "setTempFolder();";
            string confirmScript = "if (confirmAction()) ShowSuccess();";

            this.language = NttDataWA.UIManager.UserManager.GetUserLanguage();
            this.lblSavingOption.Text = this.Request.QueryString["title"]; //Languages.GetLabelFromCode("DigitalSignDialogCertificateList", language);
            this.optFileSystem.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalFileSystem", language);
            this.optPackage.Text = Languages.GetLabelFromCode("CheckInOutSavePackage", language);
            this.optClipboard.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalClipboard", language);
            this.optClipboardSD.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalClipboardSD", language);
            this.optSaveUrl.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalUrl", language);
            this.optSaveUrlSD.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalUrlSD", language);

            this.lblFolderPath.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalFolderPath", language);
            this.lblFileName.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalFileName", language);
            this.CheckInOutConfirmButton.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalConfirmButton", language);
            this.CheckInOutCloseButton.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalCloseButton", language);
            this.pnlAppletTag.Visible = true; ;


            componentType = UserManager.getComponentType(Request.UserAgent);
            if (componentType == Constans.TYPE_SOCKET)
            {
                selectFolderScript = "SelectFolderSocket();";
                setTempFolderScript = "setTempFolderSocket();";
                confirmScript = "confirmActionSocket()";
                this.pnlAppletTag.Visible = false;
            }
            this.CheckInOutConfirmButton.OnClientClick = confirmScript;
            this.btnBrowseForFolder.OnClientClick = selectFolderScript;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "InitializeCtrlScript", setTempFolderScript, true);
            



            this.txtFileName.Text = file_name;
        }

        private string cleanFileName(string inputFile)
        {
            string exts = GetP7mFileExtensions();
            if (!String.IsNullOrEmpty(exts))
                return inputFile.Replace(exts, "");
            else
                return Path.GetFileNameWithoutExtension(inputFile);
        }

        private void FetchFileTypes(bool package = false)
        {
            // Reperimento modelli di documento
            if (!package)
            {
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
            }
            else
            {
                this.cboFileTypes.Visible = false;
                this.lblFileType.Visible = package;
                this.lblFileType.Text = "zip";
            }
            
        }

        private void HydeExtension()
        {
            this.lblFileType.Visible = false;
            this.cboFileTypes.Visible = false;
        }

        /// <summary>
        /// Reperimento tipologie di modelli di documento disponibili
        /// </summary>
        /// <returns></returns>
        private string[] GetDocumentModelTypes()
        {
            string[] retValue = null;
            String selectedVersionId = null;

            if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
                selectedVersionId = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v.versionId).FirstOrDefault();

            DocsPaWR.FileRequest fileInfo = (UIManager.DocumentManager.getSelectedAttachId() != null) ?
                    UIManager.FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId()) :
                        UIManager.FileManager.GetFileRequest(selectedVersionId);

            string fileName = fileInfo.fileName;

            string currExtList = GetCurrentFileExtensionList(fileInfo);

            // Verifica se il file è già stato acquisito
            bool isFileAcquired = (!string.IsNullOrEmpty(fileName));

            if (isFileAcquired)
            {
                // Se il file è già stato acquisito, viene proposta
                // l'estensione del file fornito in querystring
                if (string.IsNullOrEmpty(currExtList))
                {
                    string extension = (fileInfo.fileName.Split('.').Length > 1) ? (fileInfo.fileName.Split('.'))[fileInfo.fileName.Split('.').Length - 1] : string.Empty;
                    retValue = new string[1] { extension };
                }
                else {
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


            String selectedVersionId = null;

            if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
                selectedVersionId = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v.versionId).FirstOrDefault();

            DocsPaWR.FileRequest fileInfo = (UIManager.DocumentManager.getSelectedAttachId() != null) ?
                    UIManager.FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId()) :
                        UIManager.FileManager.GetFileRequest(selectedVersionId);

            /*
            DocsPaWR.FileRequest fileInfo = (UIManager.DocumentManager.getSelectedAttachId() != null) ?
                    UIManager.FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId()) :
                        UIManager.FileManager.GetFileRequest();
             * */
            //DocsPaWR.FileDocumento fileInfo = SaveFileServices.GetFileInfo();

            FileDocumento doc = FileManager.getInstance(this.schedaDocumento.systemId).getInfoFile(this.Page, fileInfo);

            //string fileName = fileInfo.fileName;
            //string fileName = doc.nomeOriginale;
            string fileName = doc.nomeOriginale;
            if (string.IsNullOrEmpty(fileName))
                fileName = doc.name;

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


        protected void rblSavingOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (this.rblListSavingOption.SelectedValue)
                {
                    case OPT_FILE_SYSTEM:
                        this.txtFolderPath.Enabled = true;
                        this.btnBrowseForFolder.Enabled = true;
                        this.txtFileName.Enabled = true;
                        this.hdnOptSelected.Value = OPT_FILE_SYSTEM;
                        this.FetchFileTypes();
                    break;
                    case OPT_PACKAGE:
                        this.txtFolderPath.Enabled = true;
                        this.btnBrowseForFolder.Enabled = true;
                        this.txtFileName.Enabled = true;
                        this.hdnOptSelected.Value = OPT_PACKAGE;
                        this.FetchFileTypes(true);

                        break;
                    case OPT_CLIP_BOARD:
                        this.txtFolderPath.Enabled = false;
                        this.btnBrowseForFolder.Enabled = false;
                        this.txtFileName.Enabled = false;
                        this.hdnOptSelected.Value = OPT_CLIP_BOARD;
                        this.HydeExtension();
                        break;
                    case OPT_CLIP_BOARD_SD:
                        this.txtFolderPath.Enabled = false;
                        this.btnBrowseForFolder.Enabled = false;
                        this.txtFileName.Enabled = false;
                        this.hdnOptSelected.Value = OPT_CLIP_BOARD_SD;
                    break;
                    case OPT_URL:
                        this.txtFolderPath.Enabled = true;
                        this.btnBrowseForFolder.Enabled = true;
                        this.txtFileName.Enabled = true;
                        this.hdnOptSelected.Value = OPT_URL;
                        this.HydeExtension();
                    break;
                    case OPT_URL_SD:
                        this.txtFolderPath.Enabled = true;
                        this.btnBrowseForFolder.Enabled = true;
                        this.txtFileName.Enabled = true;
                        this.hdnOptSelected.Value = OPT_URL_SD;
                        this.HydeExtension();
                    break;
                }

                this.udpFileSystem.Update();
            }
            catch (System.Exception)
            {
                
            }
        }

        /// <summary>
        /// Creazione link per l'accesso diretto al documento dall'esterno
        /// </summary>
        protected string Link
        {
            get
            {
                StringBuilder link = new StringBuilder(httpFullPath +
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
                StringBuilder link = new StringBuilder(httpFullPath +
                   "/CheckInOut/OpenDirectLink.aspx?idAmministrazione=" + UIManager.UserManager.GetInfoUser().idAmministrazione +
                   "&idObj=" + UIManager.DocumentManager.getSelectedRecord().systemId +
                   "&tipoProto=" + UIManager.DocumentManager.getSelectedRecord().tipoProto + "&from=record");

                return link.ToString();
            }
        }

        /// <summary>
        /// Creazione del link path completo dell'icona
        /// </summary>
        protected string FileIcona
        {
            get
            {
                return httpFullPath + "/images/Icons/favicon.ico";
            }
        }

        /// <summary>
        /// Messaggi di errore e avvisi
        /// </summary>
        protected string GetMessage(string messageType)
        {
            string message="";

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

        protected void CheckInOutConfirmButton_Click(object sender, EventArgs e)
        {
            string script =  "return confirmAction(); parent.closeAjaxModal('SaveDialog','up');";
            if(this.componentType == Constans.TYPE_SOCKET)
                script = "return confirmActionSocket(); parent.closeAjaxModal('SaveDialog','up');";
            ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "confirmAJM", script, true);
        }

        protected void CheckInOutCloseButton_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "closeAJM", "parent.closeAjaxModal('SaveDialog','up');", true);
        }


        private void _updateFilName()
        {
            try
            {
                if(this.rblListSavingOption.SelectedValue != OPT_PACKAGE)
                {
                    this.txtFileName.Text = file_name;
                }
                else
                {
                    this.txtFileName.Text = Path.ChangeExtension(file_name, "zip");
                }
                
            }
            catch (Exception)
            {

            }
        }


    }
}