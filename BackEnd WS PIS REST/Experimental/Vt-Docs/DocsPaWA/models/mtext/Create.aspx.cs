using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MText;
using DocsPAWA.utils;

namespace DocsPAWA.models.mtext
{
	public partial class Create : System.Web.UI.Page
	{
        private String documentNumber;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Controlla parametri query string
            if (Request.QueryString["idDocument"] != null &&
                Request.QueryString["documentNumber"] != null)
            {
                documentNumber = Request.QueryString["documentNumber"];
                ProcessRequest();
            }

        }

        public void ProcessRequest()
        {
            // Ottieni istanza MTEXT
            MTextModelProvider mtext = ModelProviderFactory<MTextModelProvider>.GetInstance();

            // Carica scheda documento
            DocsPaWR.SchedaDocumento schedaDocumentoSelezionata = CheckInOut.CheckInOutServices.CurrentSchedaDocumento;

            // Carica modello
            String databinding = schedaDocumentoSelezionata.template.PATH_MODELLO_1;

            // Elabora FQN
            String fqn = MTextUtils.Id2FullQualifiedName(documentNumber);

            // Crea documento
            try
            {
                fqn = mtext.CreateDocument(fqn, MTextUtils.CustomObject2Dictionary(schedaDocumentoSelezionata.template), databinding);
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                this.WriteResponse(fqn, e.Message);
                return;

            }

            // Ottieni URL per EDIT
            String url = mtext.GetDocumentEditUrl(fqn);

            // Restituisci stringa fqn|editURL
            this.WriteResponse(fqn, url);

        }

        /// <summary>
        /// Funzione per la scrittura della repsonse
        /// </summary>
        /// <param name="fqn">Full Qualified Name del documento</param>
        /// <param name="message">Messaggio da scrivere</param>
        public void WriteResponse(String fqn, String message)
        {
            HttpResponse r = Response;
            r.Write(fqn);
            r.Write("|");
            r.Write(message);
        }
	}
}