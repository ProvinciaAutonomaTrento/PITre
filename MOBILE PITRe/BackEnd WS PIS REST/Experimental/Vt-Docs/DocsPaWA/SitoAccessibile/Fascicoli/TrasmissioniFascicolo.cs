using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SitoAccessibile.Documenti.Trasmissioni;
using DocsPAWA.SitoAccessibile.Paging;

namespace DocsPAWA.SitoAccessibile.Fascicoli
{
    /// <summary>
    /// Summary description for TrasmissioniFascicolo.
    /// </summary>
    public abstract class TrasmissioniFascicolo : UserControl
    {
        /// <summary>
        /// Fascicolo corrente
        /// </summary>
        protected DocsPAWA.DocsPaWR.Fascicolo Fascicolo
        {
            get
            {
                if (this.ViewState["Fascicolo"] != null)
                    return (DocsPAWA.DocsPaWR.Fascicolo)this.ViewState["Fascicolo"];
                else
                    return null;
            }
            set
            {
                this.ViewState["Fascicolo"] = value;
            }
        }

        public TrasmissioniFascicolo()
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            ListPagingNavigationControls ctl = this.GetControlPagingNavigation();
            ctl.OnPageChanged += new DocsPAWA.SitoAccessibile.Paging.ListPagingNavigationControls.PageChangedDelegate(this.GridPaging_OnPageChanged);

            this.HideDetailsTrasmissione();

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.SetFieldsVisibility();

            this.RefreshMessage();

            base.OnPreRender(e);
        }

        /// <summary>
        /// Reperimento controllo paginazione
        /// </summary>
        /// <returns></returns>
        protected abstract ListPagingNavigationControls GetControlPagingNavigation();

        /// <summary>
        /// Reperimento controllo dettagli trasmissione
        /// </summary>
        /// <returns></returns>
        protected abstract Control GetControlDetailsTrasmissione();

        /// <summary>
        /// Reperimento controllo datagrid
        /// </summary>
        /// <returns></returns>
        protected abstract DataGrid GetControlDatagridTrasmissioni();

        /// <summary>
        /// 
        /// </summary>
        private void HideDetailsTrasmissione()
        {
            this.GetControlDetailsTrasmissione().Visible = false;
        }

        /// <summary>
        /// Impostazione visibilità campi UI
        /// </summary>
        private void SetFieldsVisibility()
        {
            DataGrid dataGrid = this.GetControlDatagridTrasmissioni();
            dataGrid.Visible = (dataGrid.Items.Count > 0);
            //this.GetControlPagingNavigation().Visible=dataGrid.Visible;
        }

        /// <summary>
        /// Impostazione messaggio
        /// </summary>
        /// <param name="message"></param>
        protected abstract void RefreshMessage();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract int GetItemsCount();

        /// <summary>
        /// Reperimento messaggio esito trasmissioni
        /// </summary>
        /// <returns></returns>
        protected string GetMessageTrasmissioni()
        {
            string message = string.Empty;

            int recordCount = this.GetItemsCount();

            if (recordCount == 0)
                message = "Nessuna trasmissione trovata";
            else
                message = "Trovate " + recordCount.ToString() + " trasmissioni";

            return message;
        }

        /// <summary>
        /// Impostazione / reperimento idfascicolo
        /// </summary>
        protected string IDFascicolo
        {
            get
            {
                if (this.ViewState["IDFascicolo"] != null)
                    return this.ViewState["IDFascicolo"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["IDFascicolo"] = value;
            }
        }

        /// <summary>
        /// Impostazione / reperimento idregistro
        /// </summary>
        protected string IDRegistro
        {
            get
            {
                if (this.ViewState["IDRegistro"] != null)
                    return this.ViewState["IDRegistro"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["IDRegistro"] = value;
            }
        }

        /// <summary>
        /// Caricamento trasmissioni effettuate da un fascicolo
        /// </summary>
        /// <param name="fascicolo"></param>
        public void Fetch(DocsPAWA.DocsPaWR.Fascicolo fascicolo)
        {
            if (fascicolo == null)
                throw new ApplicationException("Parametro 'fascicolo' non fornito");

            this.Fascicolo = fascicolo;

            ListPagingNavigationControls ctl = this.GetControlPagingNavigation();

            // Reperimento contesto di paginazione
            PagingContext pagingContext = ctl.GetPagingContext();

            this.FetchTrasmissioni(pagingContext);

            ctl.RefreshPaging(pagingContext);


            //this.FetchTrasmissioni();
        }

        /// <summary>
        /// Caricamento trasmissioni effettuate da un fascicolo
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="idRegistro"></param>
        public void Fetch(string idFascicolo, string idRegistro)
        {
            if (idFascicolo == null)
                throw new ApplicationException("Parametro 'idFascicolo' non fornito");

            if (idRegistro == null)
                throw new ApplicationException("Parametro 'idRegistro' non fornito");

            this.IDFascicolo = idFascicolo;
            this.IDRegistro = idRegistro;

            FascicoloHandler fascicoloHandler = new FascicoloHandler();
            this.Fetch(this.LoadFascicolo());
        }

        /// <summary>
        /// Reperimento oggetto fascicolo
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Fascicolo LoadFascicolo()
        {
            FascicoloHandler fascicoloHandler = new FascicoloHandler();
            return fascicoloHandler.GetFascicolo(this.IDFascicolo, this.IDRegistro);
        }

        /// <summary>
        /// Restituzione oggetto fascicolo corrente
        /// </summary>
        /// <returns></returns>
        protected DocsPAWA.DocsPaWR.Fascicolo GetFascicolo()
        {
            return this.Fascicolo;
        }

        ///// <summary>
        ///// Caricamento trasmissioni effettuate dal fascicolo
        ///// </summary>
        //private void FetchTrasmissioni()
        //{
        //    Trasmissione[] trasmissioni=this.GetTrasmissioni();

        //    this.BindGrid(trasmissioni);
        //}

        private void FetchTrasmissioni(PagingContext pagingContext)
        {
            Trasmissione[] trasmissioni = this.GetTrasmissioni(pagingContext);

            this.BindGrid(trasmissioni);
        }

        /// <summary>
        /// Reperimento trasmissioni
        /// </summary>
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        protected abstract Trasmissione[] GetTrasmissioni(PagingContext pagingContext);
        //protected abstract Trasmissione[] GetTrasmissioni();

        /// <summary>
        /// Binding dati nel datagrid
        /// </summary>
        /// <param name="trasmissioni"></param>
        private void BindGrid(Trasmissione[] trasmissioni)
        {
            DataGrid dataGrid = this.GetControlDatagridTrasmissioni();
            dataGrid.DataSource = this.TrasmissioniToDataset(trasmissioni);
            dataGrid.DataBind();
        }

        /// <summary>
        /// Creazione dataset contenente i dati delle trasm effettuate
        /// </summary>
        /// <param name="trasmissioni"></param>
        /// <returns></returns>
        protected abstract DataSet TrasmissioniToDataset(DocsPAWA.DocsPaWR.Trasmissione[] trasmissioni);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTrasmissione"></param>
        protected abstract void ShowDetailsTrasmissione(string idTrasmissione);

        /// <summary>
        /// Evento relativo al cambio pagina della griglia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridPaging_OnPageChanged(Object sender, ListPagingNavigationControls.PageChangedEventArgs e)
        {
            this.FetchTrasmissioni(this.GetControlPagingNavigation().GetPagingContext());
        }
    }
}