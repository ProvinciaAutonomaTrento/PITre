using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using VTDocsMobile.VTDocsWSMobile;

namespace VTDocsMobile
{
    public partial class CheckPage : System.Web.UI.Page
    {

        VTDocsWSMobileClient ws = new VTDocsWSMobileClient();
        private ILog logger = LogManager.GetLogger(typeof(CheckPage));

        protected void Page_Load(object sender, EventArgs e)
        {
            bool rtn = false;
            try
            {
                string exMessage = string.Empty;
                rtn = ws.checkConnection(out exMessage);

                if (!rtn)
                {
                    this.lbl_msg.Text = "False";
                    Response.StatusCode = 500;
                    logger.Error("CheckPageError: " + exMessage);
                }
                else
                {
                    logger.Info("System Check OK");
                    this.lbl_msg.Text = "True";
                    Response.StatusCode = 200;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " " + ex.StackTrace);
            }        

        }


    }
}