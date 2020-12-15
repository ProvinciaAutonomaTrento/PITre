using NttDataWA.Project.ImportExport.Import;
using NttDataWA.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Project.ImportExport
{
    public partial class UpdateImportStatus : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string importStatus = "";
            string key = Request.QueryString["key"];
            string componentType = UIManager.UserManager.getComponentType(Request.UserAgent);

            if (!String.IsNullOrEmpty(componentType) && componentType.Equals(Constans.TYPE_SOCKET) && !String.IsNullOrEmpty(key))
            {
                key = decodeQueryString(key);
                importStatus = ImportDocManager.getImportStatus(key);
            }
            else
            {
                if (HttpContext.Current.Session["idFolderAdded"] != null)
                    importStatus = HttpContext.Current.Session["idFolderAdded"].ToString();

                if (HttpContext.Current.Session["ImportStatus"] != null)
                    importStatus += "|" + HttpContext.Current.Session["ImportStatus"].ToString();
                else
                    importStatus += "|";
            }
            Response.Write(importStatus);
        }

        private string decodeQueryString(string queryString)
        {
            return System.Web.HttpUtility.UrlDecode(queryString);
        }
    }
}