using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Project.ImportExport
{
    public partial class ExportDocumentActiveX : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Faccio scadere la response
            this.Response.Expires = -1;

            if (!this.IsPostBack)
            {
                Session["fromImport"] = null;
                // Prelevo l'id del fascicolo da esportare
                if (NttDataWA.UIManager.ProjectManager.getProjectInSession() != null)
                    prjToFs.IdFascicolo = NttDataWA.UIManager.ProjectManager.getProjectInSession().systemID;
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