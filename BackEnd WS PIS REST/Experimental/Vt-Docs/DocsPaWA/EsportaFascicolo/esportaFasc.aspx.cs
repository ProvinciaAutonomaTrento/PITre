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

namespace DocsPAWA.EsportaFascicolo
{
    public partial class esportaFasc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Faccio scadere la response
            this.Response.Expires = -1;
            
            if (!this.IsPostBack)
            {
                // Prelevo l'id del fascicolo da esportare
                prjToFs.IdFascicolo = this.Request.QueryString["idFasc"];
            }

            // Se il campo nascosto hdResult contiene del testo...
            if (!string.IsNullOrEmpty(this.hdResult.Value))
            {
                // ...stampo il log...
                this.exportProject.PrintLog(this.hdResult.Value);
                // ...e cancello il contneuto del campo nascosto
                this.hdResult.Value = string.Empty;
            }
        }
    }
}