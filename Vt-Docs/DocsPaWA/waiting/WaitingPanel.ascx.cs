using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace DocsPAWA.waiting
{
    public partial class WaitingPanel : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected string WaitLabelMessage
        {
            get
            {
                return this.lblWaitMessage.ClientID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string DivWaitMessageID
        {
            get
            {
                return this.waitingPanel.ClientID;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected string WaitingPanelId
        {
            get
            {
                return this.waitingPanel.ClientID;
            }
        }
    }
}