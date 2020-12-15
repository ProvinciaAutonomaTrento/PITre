using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.utils;
using log4net;

namespace DocsPAWA
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private ILog logger = LogManager.GetLogger(typeof(form));
        protected System.Web.UI.WebControls.Label lbl_msg;
        protected void Page_Load(object sender, EventArgs e)
        {
            bool rtn = false;
            try
            {
                string exMessage = "";


                rtn = Utils.checkSystem(out exMessage);
                if (!rtn)
                {
                    lbl_msg.Text = "False";
                    Response.StatusCode = 500;
                    logger.Error("CheckPageError: " + exMessage);
                }
                else
                {
                    lbl_msg.Text = "True";
                    Response.StatusCode = 200;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message+" "+ex.StackTrace);
            }
        }
    }
}
