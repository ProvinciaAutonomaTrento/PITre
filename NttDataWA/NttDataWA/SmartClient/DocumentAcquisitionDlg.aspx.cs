using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.SmartClient
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DocumentAcquisitionDlg : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Expires = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        protected DocsPaWR.SmartClientConfigurations SmartClientConfigurations
        {
            get
            {
                if (this.ViewState["SmartClientConfigurations"] == null)
                    this.ViewState["SmartClientConfigurations"] = SmartClient.Configurations.GetConfigurationsPerUser();
                return (DocsPaWR.SmartClientConfigurations) this.ViewState["SmartClientConfigurations"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string BaseUrl
        {
            get
            {
                return NttDataWA.Utils.utils.getHttpFullPath();
            }
        }

        protected string segnatura
        {
            get
            {
                string segn = "";
                if (!string.IsNullOrEmpty(Request.QueryString["segnatura"]))
                    return Request.QueryString["segnatura"].ToString();
                else
                    return segn;
               
            }
        }
    }
}
