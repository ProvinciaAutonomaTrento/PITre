using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using NttDataWA.Utils;
using System.Web.UI.WebControls;

namespace NttDataWA.Project.ImportExport
{
    public partial class ExportDocumentApplet : System.Web.UI.Page
    {
        private string language;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                Session["fromImport"] = null;
                this.initForm();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "InitializeCtrlScript", "setTempFolder();", true);
            }
        }

        private void initForm()
        {
            this.language = NttDataWA.UIManager.UserManager.GetUserLanguage();

            this.lblFolderPath.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalFolderPath", language);
            this.CheckInOutConfirmButton.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalConfirmButton", language);
            this.CheckInOutCloseButton.Text = Languages.GetLabelFromCode("CheckInOutSaveLocalCloseButton", language);
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
            }

            return message;
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

        public static string getProjectId
        {
            get
            {
                return NttDataWA.UIManager.ProjectManager.getProjectInSession().systemID;
            }
        }
        
        protected void ExportDocumentConfirmButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.hdResult.Value))
            {
                // ...stampo il log...
                this.exportProjectApplet.PrintLog(this.hdResult.Value);
                // ...e cancello il contneuto del campo nascosto
                this.hdResult.Value = string.Empty;
            }
            //ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "confirmAJM", "return confirmAction(); parent.closeAjaxModal('CheckOutDocument','up');", true);
        }
    }
}