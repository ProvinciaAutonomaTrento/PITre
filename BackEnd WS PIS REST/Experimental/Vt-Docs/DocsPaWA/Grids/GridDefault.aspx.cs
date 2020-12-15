using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;
using System.Data;
using System.Drawing;
using DocsPAWA.SiteNavigation;
using DocsPaUtils.LogsManagement;


namespace DocsPAWA.Grids
{
    public partial class GridDefault : CssPage
    {
        // Tipo di griglia da resettare
        private String gridType;

        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);

            // Se non si è in postback viene inizializzata la pagina
            if (!IsPostBack)
                this.SetTheme();
                
            // Lettura dell'identificativo della tipologia di ricerca
            // per cui ripristinare la griglia standard
            this.gridType = Request["gridType"];

            string result = string.Empty;
            if (Request.QueryString["tabRes"] != string.Empty && Request.QueryString["tabRes"] != null)
            {
                result = Request.QueryString["tabRes"].ToString();
            }
            this.hid_tab_est.Value = result;

        }

        private void SetTheme()
        {
            string Tema = string.Empty;
            string idAmm = string.Empty;
            if ((string)Session["AMMDATASET"] != null)
                idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            else
            {
                if (UserManager.getInfoUtente() != null)
                    idAmm = UserManager.getInfoUtente().idAmministrazione;
            }

            UserManager userM = new UserManager();
            Tema = userM.getCssAmministrazione(idAmm);
        }

        /// <summary>
        /// Al clicm su questo pulsante vengono salvate le impostazioni per la griglia corrente
        /// </summary>
        protected void btnDefaultGridSettings_Click(object sender, EventArgs e)
        {
            GridManager.SelectedGrid = GridManager.SelectedGrid = GridManager.getUserGrid(GridManager.SelectedGrid.GridType);

        }

    }

}
