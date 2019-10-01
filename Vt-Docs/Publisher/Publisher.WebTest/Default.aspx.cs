using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Publisher.WebTest
{
    public partial class _Default : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPublishers_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Publishers.aspx?caller=Default.aspx");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubscribers_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Subscribers.aspx?caller=Default.aspx");
        }
    }
}