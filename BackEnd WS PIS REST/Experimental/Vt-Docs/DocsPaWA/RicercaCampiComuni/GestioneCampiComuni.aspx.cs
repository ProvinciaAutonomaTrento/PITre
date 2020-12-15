using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.RicercaCampiComuni
{
    public partial class GestioneCampiComuni : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["Bookmark"] = "RicCampiComuni";
            // Impostazione contesto corrente
            this.SetContext();
        }

        public void SetContext()
        {
            bool forceInsert;
            bool.TryParse(this.Request.QueryString["forceInsertContext"], out forceInsert);

            string url = DocsPAWA.Utils.getHttpFullPath() + "/RicercaCampiComuni/GestioneCampiComuni.aspx";
            string contextName = SiteNavigation.NavigationKeys.RICERCA_CAMPI_COMUNI;
            
            SiteNavigation.CallContext newContext = new SiteNavigation.CallContext(contextName, url);
            newContext.ContextFrameName = "top.principale";

            if (SiteNavigation.CallContextStack.SetCurrentContext(newContext, forceInsert))
                SiteNavigation.NavigationContext.RefreshNavigation();
        }
    }
}