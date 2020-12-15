using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;


namespace ConservazioneWA
{
	/// <summary>
	/// Summary description for CssPage.
	/// </summary>
    public class CssPage : System.Web.UI.Page
    {
        public string CssTheme;

        public CssPage()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        protected override void OnPreInit(EventArgs e)
        {
            string Tema = System.Configuration.ConfigurationManager.AppSettings["TEMA"];
            if (Tema == null || Tema.Equals(""))
                base.Theme = "TemaRosso";
            else
                base.Theme = Tema;

            base.OnPreInit(e);
        }

        protected override void OnInit(EventArgs e)
        {
            base.Page.MaintainScrollPositionOnPostBack = true;
            try
            {
                base.OnInit(e);
            }
            catch (Exception er)
            {
                string error = er.Message;
            }
        }
    }
}
