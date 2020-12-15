using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using Newtonsoft.Json;

namespace NttDataWA.Project.ImportExport
{
    public partial class getFileInProject : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string docId = Request.Params["ID"];
            string userInfoJSON = Request.Params["USERINFO"];

            InfoUtente userInfo = NttDataWA.UIManager.UserManager.GetInfoUser();

            if (!String.IsNullOrEmpty(userInfoJSON))
            {
                userInfo = JsonConvert.DeserializeObject<InfoUtente>(userInfoJSON);

                if (userInfo == null)
                    throw new Exception("User info not found");
            }

            DocsPaWebService webServices = new DocsPaWebService();

            ContentDocumento docContent = webServices.GetFileDocumento(userInfo, docId);

            Response.BinaryWrite(docContent.fileContent);
            Response.Flush();

        }
    }
}