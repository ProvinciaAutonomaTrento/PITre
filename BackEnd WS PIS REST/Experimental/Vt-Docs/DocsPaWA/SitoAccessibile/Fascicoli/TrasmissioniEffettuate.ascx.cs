namespace DocsPAWA.SitoAccessibile.Fascicoli
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using DocsPAWA.DocsPaWR;
    using DocsPAWA.SitoAccessibile.Trasmissioni;
    using DocsPAWA.SitoAccessibile.Documenti.Trasmissioni;
    using DocsPAWA.SitoAccessibile.Paging;

    /// <summary>
    ///	Usercontrol contenente la lista delle trasmissioni effettuate da un fascicolo
    /// </summary>
    public class TrasmissioniEffettuate : TrasmissioniFascicolo
    {
        protected UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid grdTrasmissioniEffettuate;

        private void Page_Load(object sender, System.EventArgs e)
        {
            //this.GetControlPaging().OnPageChanged += new ListPagingNavigationControls.PageChangedDelegate(this.OnPageChanged);

            //if (!this.IsPostBack)
            //{
            //    this.GetControlPaging().GetPagingContext().PageNumber = 1;
            //}
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
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grdTrasmissioniEffettuate.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdTrasmissioniEffettuate_ItemCommand);
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion


        protected System.Web.UI.HtmlControls.HtmlGenericControl pnlMessage;

        /// <summary>
        /// Indici delle colonne del datagrid
        /// </summary>
        private const int COL_ID = 0;
        private const int COL_DATA_INVIO = 1;
        private const int COL_UTENTE = 2;
        private const int COL_RUOLO = 3;
        private const int COL_DETAILS = 4;

        /// <summary>
        /// Creazione dataset contenente i dati delle trasm effettuate
        /// </summary>
        /// <param name="trasmissioni"></param>
        /// <returns></returns>
        protected override DataSet TrasmissioniToDataset(DocsPAWA.DocsPaWR.Trasmissione[] trasmissioni)
        {
            DataSet ds = new DataSet("DatasetTrasmissioniEffettuate");
            DataTable dt = new DataTable("TableTrasmissioniEffettuate");

            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("DataInvio", typeof(string));
            dt.Columns.Add("Utente", typeof(string));
            dt.Columns.Add("Ruolo", typeof(string));

            if (trasmissioni != null)
            {
                foreach (DocsPAWA.DocsPaWR.Trasmissione trasm in trasmissioni)
                {
                    DataRow row = dt.NewRow();
                    row["ID"] = trasm.systemId;
                    row["DataInvio"] = trasm.dataInvio;
                    row["Utente"] = trasm.utente.descrizione;
                    row["Ruolo"] = trasm.ruolo.descrizione;
                    dt.Rows.Add(row);
                }
            }

            ds.Tables.Add(dt);

            return ds;
        }

        /// <summary>
        /// Reperimento controllo paginazione
        /// </summary>
        /// <returns></returns>
        protected override ListPagingNavigationControls GetControlPagingNavigation()
        {
            return this.FindControl("pagingTrasmEffettuate") as ListPagingNavigationControls;
        }

        /// <summary>
        /// Reperimento controllo dettagli trasmissione
        /// </summary>
        /// <returns></returns>
        protected override Control GetControlDetailsTrasmissione()
        {
            return this.FindControl("dettTrasmEffettuate");
        }

        /// <summary>
        /// Reperimento controllo datagrid
        /// </summary>
        /// <returns></returns>
        protected override DataGrid GetControlDatagridTrasmissioni()
        {
            return this.grdTrasmissioniEffettuate;
        }

        /// <summary>
        /// Impostazione messaggio
        /// </summary>
        /// <param name="message"></param>
        protected override void RefreshMessage()
        {
            this.pnlMessage.InnerText = this.GetMessageTrasmissioni();
        }

        /// <summary>
        /// Reperimento numero totale delle trasmissioni effettuate
        /// </summary>
        /// <returns></returns>
        protected override int GetItemsCount()
        {
            return this.GetControlPagingNavigation().GetPagingContext().RecordCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        protected override Trasmissione[] GetTrasmissioni(PagingContext pagingContext)
        {
            FascicoloHandler fascicoloHandler = new FascicoloHandler();

            return fascicoloHandler.GetTrasmissioniFascicolo(TipiTrasmissioniEnum.Effettuate, null, this.GetFascicolo(), pagingContext);
        }

        ///// <summary>
        ///// Reperimento trasmissioni
        ///// </summary>
        ///// <returns></returns>
        //protected override Trasmissione[] GetTrasmissioni()
        //{
        //    FascicoloHandler fascicoloHandler=new FascicoloHandler();

        //    Paging.PagingContext pagingContext = this.GetControlPaging().GetPagingContext();

        //    //this.GetControlPaging().OnPageChanged += new ListPagingNavigationControls.PageChangedDelegate(OnPageChanged);

        //    Trasmissione[] retValue = fascicoloHandler.GetTrasmissioniFascicolo(TipiTrasmissioniEnum.Effettuate, null, this.GetFascicolo(), pagingContext);

        //    this.GetControlPaging().RefreshPaging(pagingContext);

        //    return retValue;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnPageChanged(object sender, Paging.ListPagingNavigationControls.PageChangedEventArgs e)
        {
            this.Fetch(this.IDFascicolo, this.IDRegistro);
        }

        /// <summary>
        /// Visualizzazione dettagli trasmissione
        /// </summary>
        /// <param name="idTrasmissione"></param>
        protected override void ShowDetailsTrasmissione(string idTrasmissione)
        {
            DettTrasmEffettuate details = (DettTrasmEffettuate)this.GetControlDetailsTrasmissione();
            details.Initialize(idTrasmissione);
            details.Visible = true;
        }

        private void grdTrasmissioniEffettuate_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (e.CommandName.Equals("SHOW_DETAILS"))
                this.ShowDetailsTrasmissione(e.Item.Cells[COL_ID].Text);
        }
    }
}