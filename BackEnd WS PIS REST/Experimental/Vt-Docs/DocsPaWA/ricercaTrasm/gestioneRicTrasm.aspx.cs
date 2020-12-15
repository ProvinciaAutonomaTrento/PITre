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

namespace DocsPAWA.ricercaTrasm
{
    /// <summary>
    /// Summary description for gestioneRicTrasm.
    /// </summary>
    public class gestioneRicTrasm : System.Web.UI.Page
    {
        protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_sx;
        protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_dx;

        protected string versoTrasm;

        private string getQueryString()
        {
            string queryString = "";
            string conc = "";
            if (Request.QueryString["tab"] != null)
            {
                queryString += "tab=" + (string)Request.QueryString["tab"];
                conc = "&";
            }
            else
            {
                queryString += "tab=completa";
                conc = "&";
            }

            if (Request.QueryString["verso"] != null)
            {
                queryString = queryString + conc + "verso=" + (string)Request.QueryString["verso"];
                
                //if (Request.QueryString["oneTime"] != null)
                //    queryString += "&oneTime=" + Request.QueryString["oneTime"];
                if (Session["oneTimeRicTrasm"] != null)
                {
                    queryString += "&oneTime=1";
                    Session.Remove("oneTimeRicTrasm");
                }
            }
            else
            {
                queryString = queryString + conc + "verso=R";
            }

            if (Request.QueryString["back"] != null && Request.QueryString["back"].ToLower() == "true")
            {
                if (TrasmManager.getMemoriaFiltriRicTrasm(this) != null)
                {
                    queryString += conc + "back=" + Request.QueryString["back"].ToString();

                    // Gestione del tasto "back", viene effettuata
                    // nuovamente la ricerca e viene visualizzata
                    // la pagina di attesa
                    this.iFrame_dx.NavigateTo = "../waitingpage.htm";
                }
            }

            if (Request.QueryString["docIndex"] != null && Request.QueryString["docIndex"] != string.Empty)
            {
                queryString = queryString + conc + "docIndex=" + Request.QueryString["docIndex"];
            }

            return queryString;
        }

        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                Utils.startUp(this);
                string pageToView = "";
                string queryString = getQueryString();

                Session["Bookmark"] = "RicercaTrasm";

                // Rimozione dell'eventuale documento selezionato
                // o in lavorazione
                DocumentManager.removeDocumentoSelezionato(this);
                DocumentManager.removeDocumentoInLavorazione(this);

                this.SetContext(queryString);

                this.iFrame_sx.NavigateTo = "tabGestioneRicTrasm.aspx?" + queryString;

                versoTrasm = Request.QueryString["versoTrasm"];

                if (versoTrasm != null)
                {
                    //imposto la pagina da visualizzare nel frame di destra
                    //in base al valore passatomi dipendente dallo stato 
                    //del radio button
                    pageToView = "../resultBlankPage.aspx";

                    //aggiorno lo stato del frame di destra se sono riuscito
                    //a far corrispondere una pagina allo stato richiesto
                    if (pageToView != "")
                    {
                        this.iFrame_dx.NavigateTo = pageToView;
                    }
                }
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
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

        /// <summary>
        /// Impostazione contesto corrente
        /// </summary>
        private void SetContext(string queryString)
        {
            string url = DocsPAWA.Utils.getHttpFullPath() + "/ricercaTrasm/gestioneRicTrasm.aspx?" + queryString;

            SiteNavigation.CallContext newContext = new SiteNavigation.CallContext(SiteNavigation.NavigationKeys.RICERCA_TRASMISSIONI, url);
            newContext.ContextFrameName = "top.principale";

            if (SiteNavigation.CallContextStack.SetCurrentContext(newContext))
                SiteNavigation.NavigationContext.RefreshNavigation();
        }
    }
}