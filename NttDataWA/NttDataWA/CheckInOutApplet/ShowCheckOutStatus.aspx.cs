using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.CheckInOutApplet
{
    public partial class ShowCheckOutStatus : System.Web.UI.Page
    {
        private string language;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.initLabel();
                this.loadCheckOutData();
            }
        }

        private void initLabel()
        {
            this.language = NttDataWA.UIManager.UserManager.GetUserLanguage();

            this.lblUser.Text = Utils.Languages.GetLabelFromCode("CheckInOutStatusUser", language);
            this.lblRole.Text = Utils.Languages.GetLabelFromCode("CheckInOutStatusRole", language);
            this.lblData.Text = Utils.Languages.GetLabelFromCode("CheckInOutStatusData", language);
            this.lblPath.Text = Utils.Languages.GetLabelFromCode("CheckInOutStatusPath", language);
            this.lblComputer.Text = Utils.Languages.GetLabelFromCode("CheckInOutStatusComputer", language);
        }

        private void loadCheckOutData()
        {
            string strUser = string.Empty;
            string strRole = string.Empty;
            string strData = string.Empty;
            string strPath = string.Empty;
            string strComputer = string.Empty;

            CheckInOutServices.InitializeContext();

            NttDataWA.DocsPaWR.SchedaDocumento schedaDoc = CheckInOutServices.CurrentSchedaDocumento;
            DocsPaWR.CheckOutStatus tempStatus = null;
            
            if (UIManager.DocumentManager.getSelectedAttachId() == null)
            {
                 if (schedaDoc != null)
                    tempStatus = schedaDoc.checkOutStatus;
            }
            else
            {
                if (UIManager.DocumentManager.GetSelectedAttachment() != null)
                    tempStatus = UIManager.DocumentManager.GetCheckOutDocumentStatus(UIManager.DocumentManager.GetSelectedAttachment().docNumber);
            }


            if (tempStatus != null)
            {
                string temp_strUser = tempStatus.UserName;
                strRole = tempStatus.RoleName;
                strData = tempStatus.CheckOutDate.ToString();
                strPath = tempStatus.DocumentLocation;
                strComputer = tempStatus.MachineName;

                if (!string.IsNullOrEmpty(temp_strUser))
                    strUser = temp_strUser + " [" + UIManager.AddressBookManager.getCorrispondenteByCodRubrica(temp_strUser, true).descrizione + "]";
            }

            this.lblUserNAme.Text = strUser;
            this.lblRoleType.Text = strRole;
            this.lblDataCheckOut.Text = strData;
            this.lblLocalFilePath.Text = strPath;
            this.lblComputerName.Text = strComputer;
        }

        protected void CheckInOutCloseButton_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "closeAJM", "parent.closeAjaxModal('ShowCheckOutStatus','');", true);
        }
    }
}