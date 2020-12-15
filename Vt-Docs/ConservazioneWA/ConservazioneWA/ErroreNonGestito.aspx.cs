using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ConservazioneWA
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ErroreNonGestito : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.txtUnhandledErrorMessage.Text = this.Request.QueryString["Message"];
        }
    }
}