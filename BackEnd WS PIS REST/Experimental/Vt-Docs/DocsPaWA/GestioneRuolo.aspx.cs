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

namespace DocsPAWA
{
    /// <summary>
    /// Summary description for GestioneRuolo.
    /// </summary>
    public class GestioneRuolo : System.Web.UI.Page
    {
        protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_sx;
        protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_dx;
        protected Utilities.MessageBox msg_delega;

        private void Page_Load(object sender, System.EventArgs e)
        {
            Utils.startUp(this);

            this.iFrame_sx.NavigateTo = "sceltaRuoloNew.aspx";

            if (!this.IsPostBack)
            {
                int numDelega = DelegheManager.VerificaDelega(this);


                string autoList = ConfigSettings.getKey("AUTO_TO_DO_LIST");

                if ((autoList != null) && (Utils.isNumeric(autoList)) && numDelega < 1)
                {
                    if (autoList.Equals("1"))
                        this.iFrame_dx.NavigateTo = "waitingpage.htm";
                    else
                        this.iFrame_dx.NavigateTo = "blank_page.htm";
                }
                else
                {
                    if ((numDelega > 0) && (Session["ESERCITADELEGA"] == null))
                    {
                        string messaggio = InitMessageXml.getInstance().getMessage("ESERCITA_DELEGA");
                        msg_delega.Confirm(messaggio);
                    }
                }
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
            this.msg_delega.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_delega_GetMessageBoxResponse);
        }
        #endregion

        private void msg_delega_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                string scriptString = "<SCRIPT>OpenDeleghe();</SCRIPT>";
                this.RegisterStartupScript("OpenDeleghe", scriptString);
                Session.Add("ESERCITADELEGA", true);
                Session.Add("APRIDELEGHE", true);
            }
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Cancel)
            {
                Session.Add("ESERCITADELEGA", false);
            }
        }
    }
}