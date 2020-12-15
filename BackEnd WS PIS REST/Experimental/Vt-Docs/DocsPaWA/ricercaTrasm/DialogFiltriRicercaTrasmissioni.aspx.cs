using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace DocsPAWA.ricercaTrasm
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DialogFiltriRicercaTrasmissioni : DocsPAWA.CssPage
    {
        /// <summary>
        /// 
        /// </summary>
        public const string CURRENT_FILTERS_SESSION_KEY = "DialogFiltriRicercaTrasmissioni.CurrentFilters";


        /// <summary>
        /// Rimozione filtri in sessione
        /// </summary>
        public static void RemoveCurrentFilters()
        {
            if (HttpContext.Current.Session[ricercaTrasm.DialogFiltriRicercaTrasmissioni.CURRENT_FILTERS_SESSION_KEY] != null)
                HttpContext.Current.Session.Remove(ricercaTrasm.DialogFiltriRicercaTrasmissioni.CURRENT_FILTERS_SESSION_KEY);

            FiltriRicercaTrasmissioni.RemoveCurrentFilters();
        }

        /// <summary>
        /// Impostazione / reperimento da sessione del filtro correntemente impostato sulle trasmissioni
        /// </summary>
        public static DocsPaWR.FiltroRicerca[] CurrentFilters
        {
            get
            {
                return HttpContext.Current.Session[CURRENT_FILTERS_SESSION_KEY] as DocsPaWR.FiltroRicerca[];
            }
            set
            {
                if (value == null)
                {
                    HttpContext.Current.Session.Remove(CURRENT_FILTERS_SESSION_KEY);
                    HttpContext.Current.Session.Remove(FiltriRicercaTrasmissioni.CURRENT_UI_FILTERS_SESSION_KEY);
                }
                else
                {
                    HttpContext.Current.Session[CURRENT_FILTERS_SESSION_KEY] = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                // Ripristiono filtri immessi nei campi UI
                this.InnerFiltersControl.RestoreCurrentFilters();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnOK_Click(object sender, EventArgs e)
        {
            this.PerformActionConfirm();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.PerformActionCancel();
        }

        /// <summary>
        /// Azione di conferma per l'applicazione del filtro
        /// </summary>
        private void PerformActionConfirm()
        {
            bool ok = true;
            if (this.InnerFiltersControl != null)
            {
                // Reperimento filtri impostati dall'utente
                DocsPaWR.FiltroRicerca[] filters = this.InnerFiltersControl.GetFilters();

                if (filters != null)
                {
                    int countValoriOk = 0;
                    foreach (DocsPaWR.FiltroRicerca filtro in filters)
                    {
                        if (filtro.argomento == DocsPaWR.FiltriTrasmissioneNascosti.ELEMENTI_NON_VISTI.ToString() && !string.IsNullOrEmpty(filtro.valore))
                            countValoriOk++;
                        if (filtro.argomento == DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString() && !string.IsNullOrEmpty(filtro.valore))
                            countValoriOk++;
                        if (filtro.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DOCUMENTI_ACQUISITI.ToString() && !string.IsNullOrEmpty(filtro.valore))
                            countValoriOk++;
                        if (filtro.argomento == DocsPaWR.FiltriTrasmissioneNascosti.DOCUMENTI_FIRMATI.ToString() && !string.IsNullOrEmpty(filtro.valore))
                            countValoriOk++;
                    }
                    // Impostazione filtri impostati in sessione
                    CurrentFilters = filters;

                    // Persistenza filtri immessi nei campi UI per successivo ripristino
                    this.InnerFiltersControl.PersistCurrentFilters();

                    if (countValoriOk == 0)
                    {
                        string messaggio = "Attenzione! Selezionare un criterio di ricerca";
                        Response.Write("<script>alert(\"" + messaggio + "\");</script>");
                        ok = false;
                        return;
                    }
                }
                else
                    ok = false;
            }

            if(ok)
                this.RegisterClientScript("Accept", "CloseWindow(true);");
        }

        /// <summary>
        /// Azione di cancellazione applicazione del filtro
        /// </summary>
        private void PerformActionCancel()
        {
            this.RegisterClientScript("Cancel", "CloseWindow(false);");
        }

        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.ClientScript.IsStartupScriptRegistered(this.GetType(), scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), scriptKey, scriptString);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private FiltriRicercaTrasmissioni InnerFiltersControl
        {
            get
            {
                return this.FindControl("filtriRicercaTrasmissioni") as FiltriRicercaTrasmissioni;
            }
        }
    }
}