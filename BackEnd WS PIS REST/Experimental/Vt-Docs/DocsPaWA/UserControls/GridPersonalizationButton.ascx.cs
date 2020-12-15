using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;
using System.ComponentModel;

namespace DocsPAWA.UserControls
{
    public partial class GridPersonalizationButton : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Initialize();
        }

        /// <summary>
        /// Funzione per l'inizializzazione del controllo
        /// </summary>
        private void Initialize()
        {
            // Associazione della funzione javascript per l'apertura della pagina
            // di personalizzazione della griglia
            this.btnCustomizeGrid.OnClientClick = String.Format(
                "OpenGrids('{0}','{1}','{2}','{3}');",
                this.GridType,
                String.Empty,
                String.Empty,
                String.IsNullOrEmpty(Request["ricADL"]) ? "" : "ricADL");

            //Associazione della funzione javascript per l'apertura della
            //pagina per IList ritorno dal default della griglia

            /* this.btnDefaultGrid.OnClientClick = String.Format("ReturnDefaultGrid('{0}', '{1}');", 
                 this.GridType,
                 String.IsNullOrEmpty(Request["ricADL"]) ? "" : "ricADL");*/

            // Associazione della funzione javascript per l'apertura della pagina
            // di personalizzazione della griglia
            this.btnPreferredGrid.OnClientClick = String.Format(
                "OpenPreferredGrids('{0}','{1}','{2}','{3}');",
                this.GridType,
                String.Empty,
                String.Empty,
                String.IsNullOrEmpty(Request["ricADL"]) ? "" : "ricADL");

            //Cancellare quando tutto finito e utilizzare enabled
            this.btnSalvaGrid.OnClientClick = String.Format(
               "OpenSaveGrid('{0}','{1}','{2}','{3}');",
               this.GridType,
               String.Empty,
               String.Empty,
               String.IsNullOrEmpty(Request["ricADL"]) ? "" : "ricADL");
            //


            // Visualizzazione del controllo in base alla configurazione
            this.ShowControls();
        }

        /// <summary>
        /// Visualizzazione del controllo in base alla configurazione
        /// </summary>
        private void ShowControls()
        {
            // Il ruolo associato all'utente
            Funzione[] functions;
            // Indica se bisogna visualizzare il bottone della persinalizzazione griglia
            bool showGridPersonalization = true;
            // Indica se i pulsanti relativi alle griglie devono essere abilitati
            bool enableGridPersonalization = true;

            // Reperimento della lista di funzioni associate al ruolo
            functions = UserManager.getRuolo(this.Page).funzioni;

            // Verifica delle autorizzazioni
            showGridPersonalization = functions.Where(e => e.codice.Equals("GRID_PERSONALIZATION")).Count() > 0;

            this.btnCustomizeGrid.Visible = showGridPersonalization;
            this.btnPreferredGrid.Visible = showGridPersonalization;
            this.btnSalvaGrid.Visible = showGridPersonalization;
            this.btnPreferredGrid.Visible = enableGridPersonalization;
            this.btnSalvaGrid.Visible = enableGridPersonalization;
            //  this.btnDefaultGrid.Visible = showGridPersonalization;

            /*enableGridPersonalization = String.IsNullOrEmpty(GridManager.SelectedGrid.RapidSearchId) || 
                 GridManager.SelectedGrid.RapidSearchId == "-1";*/

            this.btnCustomizeGrid.Enabled = enableGridPersonalization;
            // this.btnDefaultGrid.Enabled = enableGridPersonalization;
        }

        /// <summary>
        /// Tipo di griglia di ricerca in cui è inserita la bottoniera.
        /// Questo campo viene utilizzato per la gestione delle griglie custom
        /// </summary>
        public GridTypeEnumeration GridType { get; set; }

        /// <summary>
        /// URL alla pagina di persinalizzazione griglia
        /// </summary>
        protected string UrlToGridManagement
        {
            get
            {
                return Utils.getHttpFullPath() + "/Grids/GridPersonalization.aspx";
            }
        }

        protected string UrlToDefaultGrid
        {
            get
            {
                return Utils.getHttpFullPath() + "/Grids/GridDefault.aspx";
            }
        }

        protected string UrlPreferredGrid
        {
            get
            {
                return Utils.getHttpFullPath() + "/Grids/GridPreferred.aspx";
            }
        }

        protected string UrlSaveGrid
        {
            get
            {
                return Utils.getHttpFullPath() + "/Grids/GridSave.aspx";
            }
        }


        public void enableSave()
        {
            this.btnSalvaGrid.Enabled = true;
            this.btnSalvaGrid.OnClientClick = String.Format(
                "OpenSaveGrid('{0}','{1}','{2}','{3}');",
                this.GridType,
                String.Empty,
                String.Empty,
                String.IsNullOrEmpty(Request["ricADL"]) ? "" : "ricADL");
        }

        public void disableSave()
        {
            this.btnSalvaGrid.Enabled = false;
        }

    }
}

