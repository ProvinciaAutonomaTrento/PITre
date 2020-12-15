using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace SAAdminTool.AdminTool.Gestione_ProfDinamica
{
    public partial class ModelAnteprimaModelli : System.Web.UI.Page
    {
        /// <summary>
        /// Url della pagina di visualizzazione del documento (nel frame)
        /// </summary>
        protected StringBuilder _frameSrc = new StringBuilder(String.Empty);

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            this.ShowDocument();

        }

        public void ShowDocument()
        {
            // Impostazione titolo pagina
            

            // Preparazione della query string per la visualizzazione del documento
            this._frameSrc.AppendFormat("VisualizzatoreModelli.aspx");

            
            // Attach del source al frame
            this.iFrameVisUnificata.Attributes["src"] = String.Empty;
            this.iFrameVisUnificata.Attributes["src"] = this._frameSrc.ToString();
        }
    }
}