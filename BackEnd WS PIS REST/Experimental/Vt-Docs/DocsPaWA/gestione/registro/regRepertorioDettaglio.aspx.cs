using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.gestione.registro
{
    public partial class regRepertorioDettaglio : System.Web.UI.Page
    {
        private string responsabile = string.Empty;
        private string dtaLastPrint = string.Empty;

        protected System.Web.UI.WebControls.Label lbl_dtaLastPrint;
        protected System.Web.UI.WebControls.Label lbl_Responsabile;

        protected void Page_Load(object sender, EventArgs e)
        {
           
            Response.Expires = -1;
            Utils.startUp(this);
            if (!this.IsPostBack)
            {
                if (!(string.IsNullOrEmpty(Request.QueryString["dtaLastPrint"])))
                    dtaLastPrint = Request.QueryString["dtaLastPrint"];
                if (!(string.IsNullOrEmpty(Request.QueryString["responsabile"])))
                    responsabile = Request.QueryString["responsabile"];
                setDettagli();
            }
        }

        protected void setDettagli()
        {
            lbl_Responsabile.Text = responsabile;
            lbl_dtaLastPrint.Text = dtaLastPrint;
            if (lbl_dtaLastPrint.Text.Equals("01/01/0001"))
                lbl_dtaLastPrint.Text = string.Empty;
        }
    }
}