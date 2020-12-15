using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MText;

namespace DocsPAWA.models.mtext
{
    public partial class ShowDocument : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Il query string deve contenere l'attributo fqn
            if(!String.IsNullOrEmpty(Request["fqn"]))
                this.ProcessShowFile(HttpUtility.UrlDecode(Request["fqn"]));

        }

        /// <summary>
        /// Funzione per la visualizzazione di un documento M/Text con un certo Full
        /// Qualified Name
        /// </summary>
        /// <param name="fqn">Full Qualified Name del documento da visualizzare</param>
        private void ProcessShowFile(string fqn)
        {
            // Reperimento istanza provider del modello e url per
            // l'apertura del documento M/Text
            MTextModelProvider mText = ModelProviderFactory<MTextModelProvider>.GetInstance();
            String documentUrl = String.Empty;
            try
            {
                documentUrl = mText.GetDocumentEditUrl(fqn);
            }
            catch (Exception e)
            {
                this.WriteResponse(fqn, e.Message);
                this.Response.StatusCode = 500;
                return;
            }

            // Apertura della pagina 
            HttpResponse r = Response;
            r.Write(fqn);
            r.Write("|");
            r.Write(documentUrl);

        }

        /// <summary>
        /// Funzione per la scrittura di un messaggio nella Response
        /// </summary>
        /// <param name="fqn"></param>
        /// <param name="message"></param>
        private void WriteResponse(String fqn, String message)
        { }
    }
}