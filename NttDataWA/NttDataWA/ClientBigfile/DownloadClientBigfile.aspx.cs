using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.ClientBigfile
{
    public partial class DownloadClientBigfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                InitializeLanguage();
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            this.ClientBigfileOk.Text = Utils.Languages.GetLabelFromCode("ClientBigfileOk", language);
            this.ClientBigfileClose.Text = Utils.Languages.GetLabelFromCode("ClientBigfileClose", language);
            this.LtlDownloadFile.Text = Utils.Languages.GetLabelFromCode("ClientBigfileLtlDownloadFile", language);
        }

        protected void ClientBigfileOk_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = "FileUpLoadSetup.exe";
                System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearHeaders();
                response.ClearContent();
                response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                response.ContentType = "text/plain";
                response.TransmitFile(System.Web.HttpContext.Current.Server.MapPath(fileName));
                response.Flush();
                response.End();
                
            }
            catch (Exception ex)
            {

            }
        }

        protected void ClientBigfileClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ClientBigfile", "parent.closeAjaxModal('ClientBigfile', '');", true);
            }
            catch (Exception ex)
            {

            }
        }
    }
}