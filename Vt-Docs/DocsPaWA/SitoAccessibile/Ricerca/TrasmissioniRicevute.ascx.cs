namespace DocsPAWA.SitoAccessibile.Ricerca
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Linq;
    using System.Collections;
    using SitoAccessibile.Paging;
    using DocsPAWA.DocsPaWR;
    using DocsPAWA.SitoAccessibile.Trasmissioni;
    using SitoAccessibile.Ricerca;
    using SitoAccessibile.Documenti.Trasmissioni;

    /// <summary>
    ///	User control per la gestione della visualizzazione dell'elenco 
    ///	delle trasmissioni ricevute dall'utente
    /// </summary>
    /// <remarks>
    /// Il controllo gestisce sia le trasmissioni visualizzate nel contesto
    /// della todolist, sia quelle nel constesto della ricerca
    /// </remarks>
    public class TrasmissioniRicevute : System.Web.UI.UserControl
    {
        protected System.Web.UI.HtmlControls.HtmlGenericControl pnlMessage;
        protected UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid grdTrasmissioniRicevute;

        /// <summary>
        /// Enumeration che identifica le colonne visualizzate nel datagrid
        /// </summary>
        private enum GridColumnsEnum
        {
            ID,
            IDProfile,
            DocNumber,
            IDFascicolo,
            CodiceRegistro,
            DataInvio,
            Mittente,
            Oggetto,
            Documento,
            Fascicolo,
            DataScadenza,
            DettagliTrasmissione
        }

        private void Page_Load(object sender, System.EventArgs e)
        {
            this.GetListPagingControl().OnPageChanged += new ListPagingNavigationControls.PageChangedDelegate(this.TrasmissioniRicevute_OnPageChanged);

            this.HideDetailsTrasmissione();

            // Impostazione visibilità controllo paginazione,
            // visibile se visibile lo usercontrol delle trasmissioni
            this.GetListPagingControl().Visible = this.Visible;
        }

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            // Impostazione visibilità campi
            this.SetFieldsVisibility();
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
            this.PreRender += new System.EventHandler(this.Page_PreRender);

        }
        #endregion

        #region Gestione eventi

        #region Gestione eventi paginazione

        /// <summary>
        /// Evento relativo al cambio pagina della griglia
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void TrasmissioniRicevute_OnPageChanged(object source, Paging.ListPagingNavigationControls.PageChangedEventArgs e)
        {
            this.FetchTrasmissioni(e.PagingContext);

            ((ListPagingNavigationControls)source).RefreshPaging(e.PagingContext);

            this.SetPanelMessageText(e.PagingContext);
        }

        #endregion

        #region Gestione eventi griglia

        private void grdTrasmissioniRicevute_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (e.CommandName.Equals("SHOW_DETAILS"))
            {
                // Visualizzazzione dettagli trasmissione
                this.ShowDetailsTrasmissione(e.Item.Cells[(int)GridColumnsEnum.ID].Text);
            }
            else if (e.CommandName.Equals("SHOW"))
            {
                this.SetCallerContext();

                if (this.TipoRicercaTrasmissione.Equals(TipiRicercaTrasmissioniEnum.Documenti))
                    // Visualizzazione del documento
                    this.ShowDocument(e.Item.Cells[(int)GridColumnsEnum.IDProfile].Text, e.Item.Cells[(int)GridColumnsEnum.DocNumber].Text);
                else
                    this.ShowFascicolo(e.Item.Cells[(int)GridColumnsEnum.IDFascicolo].Text, e.Item.Cells[(int)GridColumnsEnum.CodiceRegistro].Text);
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Reperimento filtri di ricerca
        /// </summary>
        /// <returns></returns>
        private FiltroRicerca[] GetFiltroRicerca()
        {
            if (this.TodoListSearch)
            {
                // Creazione parametri di filtro di ricerca per todolist
                ArrayList list = new ArrayList();

                FiltroRicerca filtroRicerca = new FiltroRicerca();
                filtroRicerca.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString();
                string tipoOggetto = string.Empty;
                if (this.TipoRicercaTrasmissione.Equals(TipiRicercaTrasmissioniEnum.Documenti))
                    tipoOggetto = "D";
                else
                    tipoOggetto = "F";
                filtroRicerca.valore = tipoOggetto;
                list.Add(filtroRicerca);

                filtroRicerca = new FiltroRicerca();
                filtroRicerca.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TODO_LIST.ToString();
                filtroRicerca.valore = "null";
                list.Add(filtroRicerca);

                filtroRicerca = new FiltroRicerca();
                filtroRicerca.argomento = DocsPaWR.FiltriTrasmissioneNascosti.NO_CERCA_INFERIORI.ToString();
                filtroRicerca.valore = "null";
                list.Add(filtroRicerca);

                return (FiltroRicerca[])list.ToArray(typeof(FiltroRicerca));
            }
            else
            {
                // Reperimento parametri di filtro di ricerca
                if (Ricerca.RicercaTrasmissioniHandler.CurrentFilter != null)
                    return Ricerca.RicercaTrasmissioniHandler.CurrentFilter.ToFiltriRicerca();
                else
                    return new FiltroRicerca[1];
            }
        }

        /// <summary>
        /// Tipologia di ricerca delle trasmissioni
        /// </summary>
        private TipiRicercaTrasmissioniEnum TipoRicercaTrasmissione
        {
            get
            {
                try
                {
                    return (TipiRicercaTrasmissioniEnum)Enum.Parse(typeof(TipiRicercaTrasmissioniEnum), this.ViewState["tipoRicercaTrasmissione"].ToString(), true);
                }
                catch
                {
                    return TipiRicercaTrasmissioniEnum.Documenti;
                }
            }
            set
            {
                this.ViewState["tipoRicercaTrasmissione"] = value.ToString();
            }
        }

        /// <summary>
        /// Verifica se è stata richiesta una ricerca per le trasmissioni
        /// della todolist o per una ricerca con filtri
        /// </summary>
        private bool TodoListSearch
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this.ViewState["todoListSearch"]);
                }
                catch
                {
                    return true;
                }
            }
            set
            {
                this.ViewState["todoListSearch"] = value;
            }
        }

        /// <summary>
        /// Caricamento dati trasmissioni
        /// </summary>
        /// <param name="trasmissioniDocumento">
        /// Se true, ricerca i documenti trasmessi,
        /// altrimenti i fascicoli
        /// </param>
        /// <param name="tipoRicerca">
        /// Tipologia di ricerca delle trasmissioni
        /// </param>
        public void Fetch(TipiRicercaTrasmissioniEnum tipoRicerca, bool todoListSearch)
        {
            this.TipoRicercaTrasmissione = tipoRicerca;
            this.TodoListSearch = todoListSearch;

            PagingContext pagingContext = new PagingContext(this.GetInitPageNumber());

            this.FetchTrasmissioni(pagingContext);

            this.GetListPagingControl().RefreshPaging(pagingContext);

            this.SetPanelMessageText(pagingContext);
        }

        /// <summary>
        /// Caricamento dati trasmissioni
        /// </summary>
        /// <param name="tipoRicerca">
        /// Tipologia di ricerca delle trasmissioni
        /// </param>
        /// 
        public void Fetch(TipiRicercaTrasmissioniEnum tipoRicerca)
        {
            this.Fetch(tipoRicerca, false);
        }

        /// <summary>
        /// Caricamento dati trasmissioni
        /// </summary>
        public void Fetch()
        {
            TipiRicercaTrasmissioniEnum tipoRicerca = TipiRicercaTrasmissioniEnum.Documenti;

            if (Ricerca.RicercaTrasmissioniHandler.CurrentFilter != null &&
                Ricerca.RicercaTrasmissioniHandler.CurrentFilter.TipoOggettoTrasmissione == TipiOggettiTrasmissioniEnum.Fascicoli)
                tipoRicerca = TipiRicercaTrasmissioniEnum.Fascicoli;

            this.Fetch(tipoRicerca);
        }

        /// <summary>
        /// Reperimento numero pagina inizialie
        /// </summary>
        /// <returns></returns>
        private int GetInitPageNumber()
        {
            int pageNumber = 1;

            string searchPage = Request.QueryString["searchPage"];

            if (searchPage != null && searchPage != string.Empty)
            {
                try
                {
                    pageNumber = Convert.ToInt32(searchPage);
                }
                catch
                {
                }
            }

            return pageNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        private DocsPaWR.infoToDoList[] CurrentTodoListPage
        {
            get
            {
                if (this.ViewState["CurrentTodoListPage"] != null)
                    return (DocsPaWR.infoToDoList[])this.ViewState["CurrentTodoListPage"];
                else
                    return null;
            }
            set
            {
                this.ViewState["CurrentTodoListPage"] = value;
            }
        }

        /// <summary>
        /// Caricamento dati trasmissioni ricevute
        /// </summary>
        private void FetchTrasmissioni(PagingContext pagingContext)
        {
            if (this.TodoListSearch)
            {
                RicercaTodoListHandler todolistHandler = new RicercaTodoListHandler();

                DocsPaWR.infoToDoList[] list = todolistHandler.GetMyTodoList(pagingContext, this.GetFiltroRicerca());

                this.CurrentTodoListPage = list;

                this.BindGridTrasmissioni(this.TodolistToDataset(list));
            }
            else
            {
                RicercaTrasmissioniHandler trasmissioniHandler = new RicercaTrasmissioniHandler();

                Trasmissione[] trasmissioni = trasmissioniHandler.GetTrasmissioni(TipiTrasmissioniEnum.Ricevute, pagingContext, this.GetFiltroRicerca());

                // Associazione dati al datagrid
                this.BindGridTrasmissioni(this.TrasmissioniRicevuteToDataset(trasmissioni));
            }

            this.GetListPagingControl().RefreshPaging(pagingContext);
        }

        /// <summary>
        /// Associazione dati al datagrid
        /// </summary>
        /// <param name="ds"></param>
        private void BindGridTrasmissioni(DataSet ds)
        {
            this.grdTrasmissioniRicevute.DataSource = ds;
            this.grdTrasmissioniRicevute.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        private void HideDetailsTrasmissione()
        {
            this.GetControDetailTrasmRicevute().Visible = false;
        }

        /// <summary>
        /// Visualizzazione dettagli trasmissione
        /// </summary>
        /// <param name="idTrasmissione"></param>
        private void ShowDetailsTrasmissione(string idTrasmissione)
        {
            DettTrasmRicevute details = this.GetControDetailTrasmRicevute();
            details.Visible = true;

            if (this.TodoListSearch)
            {
                infoToDoList item = this.GetInfoTodoList(idTrasmissione);

                SitoAccessibile.Ricerca.RicercaTodoListHandler handler = new SitoAccessibile.Ricerca.RicercaTodoListHandler();

                details.Initialize(handler.GetTrasmissione(item));
            }
            else
            {
                details.Initialize(idTrasmissione);
            }
        }

        /// <summary>
        /// Visualizzazione del documento
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        private void ShowDocument(string idProfile, string docNumber)
        {
            string url = EnvironmentContext.RootPath + "Documenti/DettagliDocumento.aspx?iddoc=" + idProfile + "&docnum=" + docNumber;

            Response.Redirect(url);
        }

        /// <summary>
        /// Visualizzazione del fascicolo
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="idRegistro"></param>
        private void ShowFascicolo(string idFascicolo, string idRegistro)
        {
            string url = EnvironmentContext.RootPath + "Fascicoli/Fascicolo.aspx?idFascicolo=" + idFascicolo + "&idRegistro=" + idRegistro;

            Response.Redirect(url);
        }

        /// <summary>
        /// Impostazione contesto chiamante
        /// </summary>
        private void SetCallerContext()
        {
            // Impostazione del contesto del chiamante in sessione
            CallerContext callerContext = CallerContext.NewContext(this.Page.Request.Url.AbsoluteUri);
            callerContext.PageNumber = this.GetListPagingControl().GetPagingContext().PageNumber;
        }

        /// <summary>
        /// Reperimento controllo dettagli trasmissioni ricevute
        /// </summary>
        /// <returns></returns>
        private DettTrasmRicevute GetControDetailTrasmRicevute()
        {
            return this.FindControl("DettTrasmRicevute") as DettTrasmRicevute;
        }

        /// <summary>
        /// Impostazione visibilita campi UI
        /// </summary>
        private void SetFieldsVisibility()
        {
            bool isVisible = (this.grdTrasmissioniRicevute.Items.Count > 0);

            this.grdTrasmissioniRicevute.Visible = isVisible;
            this.GetListPagingControl().Visible = isVisible;

            bool ricercaDocumenti = (this.TipoRicercaTrasmissione.Equals(TipiRicercaTrasmissioniEnum.Documenti));

            this.grdTrasmissioniRicevute.Columns[(int)GridColumnsEnum.Documento].Visible = ricercaDocumenti;
            this.grdTrasmissioniRicevute.Columns[(int)GridColumnsEnum.Oggetto].Visible = ricercaDocumenti;
            this.grdTrasmissioniRicevute.Columns[(int)GridColumnsEnum.Fascicolo].Visible = !ricercaDocumenti;

            foreach (DataGridItem item in this.grdTrasmissioniRicevute.Items)
            {
                Button btn = item.Cells[(int)GridColumnsEnum.DettagliTrasmissione].FindControl("btnShow") as Button;
                if (btn != null)
                {
                    if (ricercaDocumenti)
                        btn.Text = "Dettaglio documento";
                    else
                        btn.Text = "Dettaglio fascicolo";
                }
            }
        }

        /// <summary>
        /// Impostazione messaggio
        /// </summary>
        /// <param name="pagingContext"></param>
        private void SetPanelMessageText(PagingContext pagingContext)
        {
            if (pagingContext.RecordCount == 0)
                this.pnlMessage.InnerText = "Nessuna trasmissione trovata";
            else
                this.pnlMessage.InnerText = "Trovate " + pagingContext.RecordCount.ToString() + " trasmissioni";
        }

        /// <summary>
        /// Reperimento controllo navigazione
        /// </summary>
        /// <returns></returns>
        private ListPagingNavigationControls GetListPagingControl()
        {
            return this.FindControl("ListPagingNavigation") as ListPagingNavigationControls;
        }

        private string GetMittenteTrasmissione(DocsPAWA.DocsPaWR.Trasmissione trasmissione)
        {
            string retValue = string.Empty;

            if (trasmissione.utente != null)
                retValue = trasmissione.utente.descrizione;

            if (trasmissione.ruolo != null)
                retValue += Environment.NewLine + "<br />(" + trasmissione.ruolo.descrizione + ")";

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DataSet CreateDataset()
        {
            DataSet ds = new DataSet("DatasetTrasmissioniRicevute");

            DataTable dt = new DataTable("DatasetTrasmissioniRicevute");

            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("IDProfile", typeof(string));
            dt.Columns.Add("DocNumber", typeof(string));
            dt.Columns.Add("IDFascicolo", typeof(string));
            dt.Columns.Add("IDRegistro", typeof(string));
            dt.Columns.Add("DataInvio", typeof(string));
            dt.Columns.Add("Mittente", typeof(string));
            dt.Columns.Add("Ragione", typeof(string));
            dt.Columns.Add("DataScadenza", typeof(string));
            dt.Columns.Add("Documento", typeof(string));
            dt.Columns.Add("Fascicolo", typeof(string));
            dt.Columns.Add("Oggetto", typeof(string));
            dt.Columns.Add("MittenteDocumento", typeof(string));

            ds.Tables.Add(dt);

            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private DocsPaWR.infoToDoList GetInfoTodoList(string idTrasmissione)
        {
            return this.CurrentTodoListPage.Where(e => e.sysIdTrasm == idTrasmissione).First();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trasmissioniToDoList"></param>
        /// <returns></returns>
        private DataSet TodolistToDataset(DocsPAWA.DocsPaWR.infoToDoList[] trasmissioniToDoList)
        {
            DataSet ds = this.CreateDataset();

            DataTable dt = ds.Tables[0];

            foreach (DocsPAWA.DocsPaWR.infoToDoList info in trasmissioniToDoList)
            {
                DataRow row = dt.NewRow();
                row["ID"] = info.sysIdTrasm;
                row["Mittente"] = info.ruoloMittente;

                if (this.TipoRicercaTrasmissione.Equals(TipiRicercaTrasmissioniEnum.Documenti))
                {
                    // Caricamento dati colonne relative ai documenti
                    row["IDProfile"] = info.sysIdDoc;
                    row["DocNumber"] = info.sysIdDoc;
                    row["Documento"] = info.videoDocInfo;
                    row["Oggetto"] = info.oggetto;
                    row["MittenteDocumento"] = info.mittente;
                }
                else
                {
                    // Caricamento dati colonne relative ai fascicoli
                    row["IDFascicolo"] = info.sysIdFasc;
                    row["IDRegistro"] = "";
                    row["Fascicolo"] = info.oggetto;
                    row["MittenteDocumento"] = info.mittente;
                }

                row["DataInvio"] = info.dataInvio;
                row["Ragione"] = info.ragione;
                row["DataScadenza"] = info.dataScadenza;

                dt.Rows.Add(row);
            }

            return ds;
        }

        /// <summary>
        /// Creazione dataset contenente i dati delle trasm ricevute
        /// </summary>
        /// <param name="trasmissioni"></param>
        /// <returns></returns>
        private DataSet TrasmissioniRicevuteToDataset(DocsPAWA.DocsPaWR.Trasmissione[] trasmissioni)
        {
            DataSet ds = this.CreateDataset();

            DataTable dt = ds.Tables[0];

            foreach (DocsPAWA.DocsPaWR.Trasmissione trasm in trasmissioni)
            {
                DataRow row = dt.NewRow();
                row["ID"] = trasm.systemId;
                row["Mittente"] = this.GetMittenteTrasmissione(trasm);

                if (this.TipoRicercaTrasmissione.Equals(TipiRicercaTrasmissioniEnum.Documenti))
                {
                    // Caricamento dati colonne relative ai documenti
                    row["IDProfile"] = trasm.infoDocumento.idProfile;
                    row["DocNumber"] = trasm.infoDocumento.docNumber;
                    row["Documento"] = SitoAccessibile.Documenti.TipiDocumento.ToString(trasm.infoDocumento);
                    row["Oggetto"] = trasm.infoDocumento.oggetto;
                    row["MittenteDocumento"] = trasm.infoDocumento.mittDoc;
                }
                else
                {
                    // Caricamento dati colonne relative ai fascicoli
                    row["IDFascicolo"] = trasm.infoFascicolo.idFascicolo;
                    row["IDRegistro"] = trasm.infoFascicolo.idRegistro;
                    row["Fascicolo"] = trasm.infoFascicolo.codice + " - " + trasm.infoFascicolo.descrizione;
                }

                row["DataInvio"] = trasm.dataInvio;

                string ragione = string.Empty;
                string dataScadenza = string.Empty;

                if (trasm.trasmissioniSingole.Length > 0)
                {
                    DocsPaWR.TrasmissioneSingola trasmSingola = trasm.trasmissioniSingole[0];
                    ragione = trasmSingola.ragione.descrizione;
                    dataScadenza = trasmSingola.dataScadenza;
                }

                row["Ragione"] = ragione;
                row["DataScadenza"] = dataScadenza;

                dt.Rows.Add(row);
            }

            return ds;
        }
    }
}