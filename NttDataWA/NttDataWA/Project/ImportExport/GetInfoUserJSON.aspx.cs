using Newtonsoft.Json;
using NttDataWA.DocsPaWR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Project.ImportExport
{
    public partial class GetInfoUserJSON : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InfoUtente userInfo = NttDataWA.UIManager.UserManager.GetInfoUser();
            string userInfoJSON = JsonConvert.SerializeObject(userInfo);
            Response.Write(userInfoJSON);
        }
    }
}