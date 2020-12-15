using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace ConservazioneWA
{
    public partial class SessionAborted : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            string result = Request.QueryString["result"];

            if (result == DocsPaWR.ValidationResult.SESSION_EXPIRED.ToString())
            {
                this.errorLabel.Text = "La sessione e' scaduta.";
            }
            else if (result == DocsPaWR.ValidationResult.SESSION_DROPPED.ToString())
            {
                this.errorLabel.Text = "L'utente si e' connesso da un'altra postazione.";
            }
            else if (result == DocsPaWR.ValidationResult.APPLICATION_ERROR.ToString())
            {
                this.errorLabel.Text = "Errore imprevisto dell'applicazione.";
            }

            this.errorLabel.Visible = true;	
        }
    }
}
