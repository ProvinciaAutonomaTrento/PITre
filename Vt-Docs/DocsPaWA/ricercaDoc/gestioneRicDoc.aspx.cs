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
using DocsPAWA.utils;

namespace DocsPAWA.ricercaDoc
{
    /// <summary>
    /// Summary description for GestioneRicDoc.
    /// </summary>
    public class GestioneRicDoc : System.Web.UI.Page
    {
        protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_sx;
        protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_dx;

        private void Page_Load(object sender, System.EventArgs e)
        {
            Utils.startUp(this);

            // Rimozione dell'eventuale documento selezionato
            // o in lavorazione
            DocumentManager.removeDocumentoSelezionato(this);
            DocumentManager.removeDocumentoInLavorazione(this);

            string tab = Request.QueryString["tab"];
            string back = Request.QueryString["back"];
            string ricADL = Request.QueryString["ricADL"];


            if (!IsPostBack && (string.IsNullOrEmpty(back) || !back.Equals("true")))
            {
                if (GridManager.SelectedGrid != null)
                {
                    GridManager.SelectedGrid = null;
                }
            }

            // Impostazione contesto corrente
            this.SetContext(tab, back);

            string url = "tabGestioneRicDoc.aspx?tab=" + Request.QueryString["tab"];

            if (ricADL != null && ricADL == "1")
                url += "&ricADL=1";

            if (back != null && back.ToLower() == "true" &&
                DocumentManager.getFiltroRicDoc(this) != null)
            {
                // Gestione del tasto "back", viene effettuata
                // nuovamente la ricerca e viene visualizzata
                // la pagina di attesa
                this.iFrame_dx.NavigateTo = "../waitingpage.htm";

                url += "&back=true";

                string docIndex = Request.QueryString["docIndex"];

                if (docIndex != null && docIndex != string.Empty)
                    url += "&docIndex=" + docIndex;
            }
            else
            {
                if (tab.IndexOf("Grigia") < 0)
                {
                    this.iFrame_dx.NavigateTo = "../waitingpage.htm";
                }
            }

            this.iFrame_sx.NavigateTo = url;

            //perchè quando si push ricerca prima ricarica la pagina (evento javascript) poi
            //evento .net = cancella sessione ricerca, è la pagina appare con la vecchia ricerca
            //perchè è stata caricato prima del remove dalle session.
            //DocumentManager.removeFiltroRicDoc(this);
            DocumentManager.removeRisultatoRicerca(this);
            DocumentManager.removeDatagridDocumento(this);

            #region tasto 'back' su gestione fascicolo by massimo digregorio
            FascicoliManager.removeMemoriaRicFasc(this);
            #endregion tasto 'back' su gestione fascicolo by massimo digregorio
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

        /// <summary>
        /// Impostazione contesto corrente
        /// </summary>
        /// <param name="back"></param>
        /// <param name="tab"></param>
        public void SetContext(string tab, string back)
        {
            string url = DocsPAWA.Utils.getHttpFullPath() + "/ricercaDoc/gestioneRicDoc.aspx";

            if (tab != null && tab != string.Empty)
                url += "?tab=" + tab;

            if (back != null && back != string.Empty)
                url += "&back=" + back;

            string contextName = string.Empty;

            string ricAdl = this.Request.QueryString["ricADL"];
            if (!string.IsNullOrEmpty(ricAdl))
            {
                contextName = SiteNavigation.NavigationKeys.RICERCA_DOCUMENTI_ADL;
                url += "&ricADL=" + ricAdl;
            }
            else
            {
                contextName = SiteNavigation.NavigationKeys.RICERCA_DOCUMENTI;
            }

            SiteNavigation.CallContext newContext = new SiteNavigation.CallContext(contextName, url);
            newContext.ContextFrameName = "top.principale";

            if (SiteNavigation.CallContextStack.SetCurrentContext(newContext))
                SiteNavigation.NavigationContext.RefreshNavigation();
        }
    }
}