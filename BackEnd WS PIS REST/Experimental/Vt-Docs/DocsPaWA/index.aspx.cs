using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Services;

namespace DocsPAWA
{
	/// <summary>
	/// Summary description for docsPA.
	/// </summary>
	public class index : System.Web.UI.Page
	{

		protected DocsPAWA.DocsPaWR.Ruolo userRuolo;
		protected DocsPAWA.DocsPaWR.InfoUtente Safe;
		protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
		protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
		protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
		protected DocsPaWebCtrlLibrary.IFrameWebControl superiore;
		protected DocsPaWebCtrlLibrary.IFrameWebControl principale;
		protected DocsPaWebCtrlLibrary.IFrameWebControl inferiore;
		protected DocsPAWA.DocsPaWR.InfoDocumento[] ListaDoc;

        [WebMethod]
        public static void AbandonSession()
        {
            UserManager.logoff(HttpContext.Current.Session);
            HttpContext.Current.Session.Abandon();
            //HttpContext.Current.Session.Abandon();
        }

		private void Page_Load(object sender, System.EventArgs e)
		{
            // Pagina a cui redirigere il contenuto del frame principale
            string newURL = "GestioneRuolo.aspx";

            this.superiore.NavigateTo = "testata320.aspx";
            this.inferiore.NavigateTo = "bottom.aspx";

            string tipoOggetto = Request.QueryString["tipoOggetto"];
            string idOggetto = Request.QueryString["idObj"];
            if (tipoOggetto != null && idOggetto != null)
            {
                DocsPaWR.InfoDocumento infoDoc = DocumentManager.getRisultatoRicerca(this);

                if (infoDoc != null)
                {
                    string nomeTab;
                    if (infoDoc.numProt != null && infoDoc.numProt.Length > 0)
                        nomeTab = "protocollo";
                    else
                        nomeTab = "profilo";

                    this.principale.NavigateTo = "documento/gestionedoc.aspx?tab" + nomeTab;
                }

            }

            #region Gestione VisualizzaOggetto

            // Tab a cui redirigere la richiesta, tipo di oggetto da visualizzare
            string tab, objType;

            tab = Request["tab"];
            objType = Request["objType"];

            // Se sono stati reperiti i dati dalla query string...
            if (!String.IsNullOrEmpty(tab) &&
                !String.IsNullOrEmpty(objType))
            {
                // ...visualizzazione dell'oggetto specificato
                switch (objType)
                {
                    case "D":
                        if (DocumentManager.getRisultatoRicerca(this) != null)
                            newURL = String.Format("{0}/documento/gestioneDoc.aspx?tab={1}",
                                Utils.getHttpFullPath(),
                                tab);
                        else
                            ClientScript.RegisterClientScriptBlock(
                                this.GetType(),
                                "Alert",
                                "alert('Non è stato possibile reperire le informazioni sul documento da visualizzare.');",
                                true);
                        break;
                    case "F":
                        if (FascicoliManager.getFascicoloSelezionato(this) != null)
                            newURL = String.Format("{0}/fascicolo/gestioneFasc.aspx?tab=documenti",
                                Utils.getHttpFullPath());
                        else
                            ClientScript.RegisterClientScriptBlock(
                                this.GetType(),
                                "Alert",
                                "alert('Non è stato possibile reperire le informazioni sul fascicolo da visualizzare.');",
                                true);
                        break;

                }

            }

            #endregion

            // Impostazione dell'url di redirezionamento per il frame principale
            this.principale.NavigateTo = newURL;
            
            //controllo che sia attivo il disservizio
            if (!IsPostBack)
            {
                string valoreChiave;
                valoreChiave= DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_GESTIONE_DISSERVIZIO");
                if (!string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("1"))
                {
                    DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    DocsPaWR.Utente utente = (DocsPAWA.DocsPaWR.Utente)this.Session["userData"];
                    DocsPAWA.DocsPaWR.Disservizio disservizio = wws.getInfoDisservizio();
                    //Se attivo faccio la redirect alla pagina di cortesia.
                    if (disservizio != null && !string.IsNullOrEmpty(disservizio.stato) && disservizio.stato.ToUpper().Equals("ATTIVO"))
                    {
                        Response.Redirect("disservizio.aspx");
                    }
                }
            }

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

       

        
	}
}
