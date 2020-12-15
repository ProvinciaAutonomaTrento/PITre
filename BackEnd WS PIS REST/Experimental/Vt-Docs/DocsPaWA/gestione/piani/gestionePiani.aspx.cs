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

namespace DocsPAWA.gestione.piani
{
    public partial class gestionePiani : System.Web.UI.Page
    {
        protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_sx;
        protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_dx;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Put user code to initialize the page here
            Utils.startUp(this);

            if (!this.IsPostBack)
                // Impostazione contesto corrente
                this.SetContext();
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

        }
        #endregion

        /// <summary>
        /// Impostazione contesto corrente
        /// </summary>
        private void SetContext()
        {
            string url = DocsPAWA.Utils.getHttpFullPath() + "/gestione/registro/gestioneReg.aspx";

            SiteNavigation.CallContext newContext = new SiteNavigation.CallContext(SiteNavigation.NavigationKeys.GESTIONE_REGISTRI, url);
            newContext.ContextFrameName = "top.principale";


            if (SiteNavigation.CallContextStack.SetCurrentContext(newContext))
                SiteNavigation.NavigationContext.RefreshNavigation();
        }
    }
}
