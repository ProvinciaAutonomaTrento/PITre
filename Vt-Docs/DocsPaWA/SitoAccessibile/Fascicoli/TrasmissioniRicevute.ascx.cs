namespace DocsPAWA.SitoAccessibile.Fascicoli
{
    using System;
    using System.Collections.Generic;
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
    ///	Usercontrol contenente la lista delle trasmissioni ricevute da un fascicolo
    /// </summary>
    public class TrasmissioniRicevute : TrasmissioniFascicolo
    {
        protected System.Web.UI.HtmlControls.HtmlGenericControl pnlMessage;
        protected UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid grdTrasmissioniRicevute;

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
            this.grdTrasmissioniRicevute.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdTrasmissioniRicevute_ItemCommand);
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion

        /// <summary>
        /// Indici delle colonne del datagrid
        /// </summary>
        private const int COL_ID = 0;
        private const int COL_DATA_INVIO = 1;
        private const int COL_MITTENTE = 2;
        private const int COL_RUOLO = 3;
        private const int COL_RAGIONE = 4;
        private const int COL_DATA_SCADENZA = 5;
        private const int COL_DETAILS = 5;

        /// <summary>
        /// Creazione dataset contenente i dati delle trasm ricevute
        /// </summary>
        /// <param name="trasmissioni"></param>
        /// <returns></returns>
        protected override DataSet TrasmissioniToDataset(DocsPAWA.DocsPaWR.Trasmissione[] trasmissioni)
        {
            DataSet ds = new DataSet("DatasetTrasmissioniRicevute");
            DataTable dt = new DataTable("TableTrasmissioniRicevute");

            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("DataInvio", typeof(string));
            dt.Columns.Add("Mittente", typeof(string));
            dt.Columns.Add("Ruolo", typeof(string));
            dt.Columns.Add("Ragione", typeof(string));
            dt.Columns.Add("DataScadenza", typeof(string));

            if (trasmissioni != null)
            {
                foreach (DocsPAWA.DocsPaWR.Trasmissione trasm in trasmissioni)
                {
                    DataRow row = dt.NewRow();
                    row["ID"] = trasm.systemId;
                    row["DataInvio"] = trasm.dataInvio;
                    row["Mittente"] = trasm.utente.descrizione;
                    row["Ruolo"] = trasm.ruolo.descrizione;

                    if (trasm.trasmissioniSingole.Length > 0)
                    {
                        TrasmissioneSingola trasmSingola = trasm.trasmissioniSingole[0];
                        row["Ragione"] = trasmSingola.ragione.descrizione;
                        row["DataScadenza"] = trasmSingola.dataScadenza;
                    }

                    dt.Rows.Add(row);
                }
            }

            ds.Tables.Add(dt);

            return ds;
        }

        /// <summary>
        /// Reperimento controllo dettagli trasmissione ricevuta
        /// </summary>
        /// <returns></returns>
        protected override Control GetControlDetailsTrasmissione()
        {
            return this.FindControl("dettTrasmRicevute");
        }

        /// <summary>
        /// Reperimento controllo paginazione
        /// </summary>
        /// <returns></returns>
        protected override ListPagingNavigationControls GetControlPagingNavigation()
        {
            return this.FindControl("pagingTrasmRicevute") as ListPagingNavigationControls;
        }

        /// <summary>
        /// Reperimento controllo datagrid trasmissioni
        /// </summary>
        /// <returns></returns>
        protected override DataGrid GetControlDatagridTrasmissioni()
        {
            return this.grdTrasmissioniRicevute;
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
        /// Reperimento numero totale delle trasmissioni ricevute
        /// </summary>
        /// <returns></returns>
        protected override int GetItemsCount()
        {
            return this.GetControlPagingNavigation().GetPagingContext().RecordCount;
        }

        /// <summary>
        /// Reperimento trasmissioni
        /// </summary>
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        protected override Trasmissione[] GetTrasmissioni(PagingContext pagingContext)
        {
            FascicoloHandler fascicoloHandler = new FascicoloHandler();

            FiltroRicerca filter = new FiltroRicerca { argomento = "TAB_TRASMISSIONI", valore = bool.TrueString.ToUpper() };

            return fascicoloHandler.GetTrasmissioniFascicolo(
                                        TipiTrasmissioniEnum.Ricevute,
                                        new FiltroRicerca[1] { filter },
                                        this.GetFascicolo(),
                                        pagingContext);
        }

        ///// <summary>
        ///// Reperimento trasmissioni
        ///// </summary>
        ///// <returns></returns>
        //protected override Trasmissione[] GetTrasmissioni()
        //{
        //    FascicoloHandler fascicoloHandler=new FascicoloHandler();

        //    Paging.PagingContext pagingContext = this.GetControlPaging().GetPagingContext();

        //    FiltroRicerca filter = new FiltroRicerca { argomento = "TAB_TRASMISSIONI", valore = bool.TrueString.ToUpper() };

        //    Trasmissione[] retValue = fascicoloHandler.GetTrasmissioniFascicolo(
        //                                TipiTrasmissioniEnum.Ricevute,
        //                                new FiltroRicerca[1] { filter },
        //                                this.GetFascicolo(), 
        //                                pagingContext);

        //    this.GetControlPaging().RefreshPaging(pagingContext);

        //    return retValue;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTrasmissione"></param>
        /// <returns></returns>
        protected Trasmissione GetTrasmissione(string idTrasmissione)
        {
            FascicoloHandler fascicoloHandler = new FascicoloHandler();

            List<FiltroRicerca> filters = new List<FiltroRicerca>();
            filters.Add(new FiltroRicerca { argomento = "TAB_TRASMISSIONI", valore = bool.TrueString.ToUpper() });
            filters.Add(new FiltroRicerca { argomento = DocsPaWR.FiltriTrasmissioneNascosti.ID_TRASMISSIONE.ToString(), valore = idTrasmissione });

            Trasmissione[] retValue = fascicoloHandler.GetTrasmissioniFascicolo(
                                        TipiTrasmissioniEnum.Ricevute,
                                        filters.ToArray(),
                                        this.GetFascicolo(),
                                        new PagingContext(1));

            if (retValue != null && retValue.Length > 0)
                return retValue[0];
            else
                return null;
        }


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
            DettTrasmRicevute details = (DettTrasmRicevute)this.GetControlDetailsTrasmissione();
            details.Initialize(this.GetTrasmissione(idTrasmissione));
            details.Visible = true;
        }

        private void grdTrasmissioniRicevute_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (e.CommandName.Equals("SHOW_DETAILS"))
            {
                this.ShowDetailsTrasmissione(e.Item.Cells[COL_ID].Text);
            }
        }
    }
}