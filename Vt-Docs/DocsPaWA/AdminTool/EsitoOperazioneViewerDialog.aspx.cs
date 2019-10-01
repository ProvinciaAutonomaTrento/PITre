using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.AdminTool
{
    /// <summary>
    /// 
    /// </summary>
    public static class EsitoOperazioneViewerSessionManager
    {
        /// <summary>
        /// 
        /// </summary>
        private const string SESSION_KEY = "EsitoOperazioneVieverSessionManager.Instance";

        /// <summary>
        /// 
        /// </summary>
        public static DocsPaWR.EsitoOperazione[] Items
        {
            get
            {
                DocsPaWR.EsitoOperazione[] items = HttpContext.Current.Session[SESSION_KEY] as DocsPaWR.EsitoOperazione[];

                if (items == null)
                    return new DocsPaWR.EsitoOperazione[0];
                else
                    return items;
            }
            set
            {
                HttpContext.Current.Session[SESSION_KEY] = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class EsitoOperazioneViewerDialog : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Expires = -1;

            this.SetMaskTitle(Server.HtmlDecode(this.Request.QueryString["maskTitle"]));

            this.viewer.Fetch(EsitoOperazioneViewerSessionManager.Items);
            
            EsitoOperazioneViewerSessionManager.Items = null;
        }

        /// <summary>
        /// Impostazione intestazione della maschera
        /// </summary>
        /// <param name="title"></param>
        protected void SetMaskTitle(string title)
        {
            this.lblTitle.Text = title;
        }
    }
}
