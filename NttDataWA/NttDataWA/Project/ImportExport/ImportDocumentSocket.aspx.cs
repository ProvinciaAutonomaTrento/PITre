using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.Utils;

namespace NttDataWA.Project.ImportExport
{
    public partial class ImportDocumentSocket : System.Web.UI.Page
    {
        private static string idFolder = "";
        private static string idProject = "";
        private static string projectCode = "";

        private string language;

        protected void Page_Load(object sender, EventArgs e)
        {
            NttDataWA.DocsPaWR.Fascicolo prj = NttDataWA.UIManager.ProjectManager.getProjectInSession();

            if (prj!=null) 
            {
                idProject = prj.systemID;
                projectCode = prj.codice;

                DocsPaWR.Folder folder = prj.folderSelezionato;

                if (folder != null)
                    idFolder = folder.systemID;
                else
                    idFolder = idProject;
            }

            if (!IsPostBack)
            {
                this.initForm();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "InitializeCtrlScript", "setTempFolder();", true);
            }
        }

        private void initForm()
        {
            this.language = NttDataWA.UIManager.UserManager.GetUserLanguage();

            this.lblFolderPath.Text = Languages.GetLabelFromCode("ImportDocumentsLblFolderPath", language);
            this.CheckInOutConfirmButton.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalConfirmButton", language);
            this.CheckInOutCloseButton.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalCloseButton", language);
        }

        #region parameter
        
        public static string getProjectId
        {
            get
            {
                return idProject;
            }
        }

        public static string getProjectCode
        {
            get
            {
                return projectCode;
            }
        }

        public static string getFolderId
        {
            get
            {
                return idFolder;
            }
        }

        protected string getLastPath()
        {
            string strResult = "";
            if (Session["lastPath"] != null)
                strResult = doubleBackslash(Session["lastPath"].ToString());

            return strResult;
        }

        private string doubleBackslash(string strToTransform)
        {
            string strResult;

            strResult = strToTransform.Replace("\\", "\\\\");
            return strResult;
        }

        public static string httpFullPath
        {
            get
            {
                return utils.getHttpFullPath();
            }
        }

        public static string getTitolarioId
        {
            get
            {
                return UIManager.ProjectManager.getProjectInSession().idTitolario;
            }
        }

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
                case "EXISTS_FILE":
                    message = Utils.Languages.GetMessageFromCode("File_Exists", language);
                    break;
            }

            return message;
        }
        #endregion

        protected void ImportDocumentConfirmButton_Click(object sender, EventArgs e)
        {

            // viene eseguito dopo a fine esecuzione applet
            if (!string.IsNullOrEmpty(this.hdResult.Value))
            {
                // ...stampo il log...
                this.ImportProjectApplet.PrintLog(this.hdResult.Value);
                // ...e cancello il contneuto del campo nascosto
                this.hdResult.Value = string.Empty;
            }
            //ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "confirmAJM", "return confirmAction(); parent.closeAjaxModal('CheckOutDocument','up');", true);
            // ImportDocumentApplet
            //ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "confirmAJM", "parent.closeAjaxModal('ImportDocumentApplet','up');", true);
            
            // Nascondo il tasto di esecuzione import
            this.CheckInOutConfirmButton.Visible = false;
        }
        
    }


}