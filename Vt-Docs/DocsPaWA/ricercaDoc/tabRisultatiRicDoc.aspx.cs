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

using System.Xml;
using System.Xml.Serialization;
using System.IO;
using DocsPAWA.UserControls;
using DocsPAWA.DocsPaWR;
using log4net;

namespace DocsPAWA.ricercaDoc
{
    /// <summary>
    /// Summary description for RicercaDoc_cn.
    /// </summary>
    public class tabRisulatiRicDoc : DocsPAWA.CssPage
    {
        private ILog logger = LogManager.GetLogger(typeof(tabRisulatiRicDoc));
        protected System.Web.UI.WebControls.DataGrid DataGrid1;
        protected DocsPAWA.DocsPaWR.Ruolo userRuolo;
        protected DocsPAWA.DocsPaWR.InfoUtente Safe;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] ListaFiltri;
        protected DocsPAWA.DocsPaWR.InfoDocumento InfoCurr;
        protected DocsPAWA.DocsPaWR.SchedaDocumento schedaSel;
        protected DocsPAWA.DocsPaWR.InfoDocumento[] ListaDoc;
        //protected DocsPAWA.DocsPaWR.InfoDocumento[] ListaDocProt;
        //protected DocsPAWA.DocsPaWR.InfoDocumento[] ListaDocGrigia;
        protected System.Web.UI.WebControls.TableRow tr_Doc2DT;
        protected System.Web.UI.WebControls.TableRow tr_Doc6DT;

        protected DocsPAWA.DocsPaWR.Utente userHome;
        protected ArrayList Rows;
        protected DataTable datatable;
        protected DocsPAWA.dataSet.DataSetRDoc dataSetRDoc1;
        protected DataView dv;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd1;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd2;
        protected System.Web.UI.WebControls.Button btn_hide;
        // Spostato in bottoniera operazioni massive
        //protected System.Web.UI.WebControls.ImageButton insertAllADL;
        protected System.Web.UI.WebControls.ImageButton insertAllCons;
        protected System.Collections.Hashtable listInArea;
        protected int currentPage = 1;
        // Spostato nella barra delle azioni massive.
        //protected System.Web.UI.WebControls.ImageButton btn_stampa;
        protected System.Web.UI.WebControls.Label titolo;
        protected System.Web.UI.WebControls.Label msgADL;
        protected System.Web.UI.HtmlControls.HtmlTableRow trHeader;
        protected System.Web.UI.HtmlControls.HtmlTableRow tr1;
        protected System.Web.UI.HtmlControls.HtmlTableCell Td2;
        protected System.Web.UI.HtmlControls.HtmlTableRow trBody;
        protected int numTotPage;
        protected int nRec = 0;
        protected static DocsPAWA.DocsPaWR.DocsPaWebService wss = ProxyManager.getWS();

        protected DocsPAWA.DocsPaWR.EtichettaInfo[] etichette;
        protected string arrivo;
        protected string partenza;
        protected string interno;
        protected string grigio;
        protected string allegato;

        #region Bottoniera Azioni massive

        // Bottoniera
        protected MassiveOperationButtons moButtons;
        // Checkbox per la selezione del singolo item
        protected CheckBox chkSelected;
        // Campo nascosto con l'id del documento
        protected HiddenField hfIdProfile;

        #endregion 

        private void Page_Load(object sender, System.EventArgs e)
        {
            // Se non si è in postback viene inizializzata la bottoniera
            if (!IsPostBack)
                this.moButtons.InitializeOrUpdateUserControl(new SearchResultInfo[] { });

            try
            {
                Utils.startUp(this);

                // La lista dei system id dei documenti restituiti dalla ricerca
                SearchResultInfo[] idProfile = new SearchResultInfo[0];

                if (!Page.IsPostBack)
                {
                    // Spostato in bottoniera operazioni massive
                    //this.btn_stampa.Attributes.Add("onclick", "StampaRisultatoRicerca();");

                    this.AttatchGridPagingWaitControl();

                    // Impostazione visibilità colonna Registro
                    this.SetVisibilityColumnRegistro();

                    currentPage = this.GetCurrentPageOnContext();

                    SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
                    if (!currentContext.IsBack && this.Request.QueryString["from"] != null)
                        currentContext.QueryStringParameters["from"] = this.Request.QueryString["from"];
                }

                getLettereProtocolli();

                // nuova ADL
                if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                {
                    //setting delle prop. video
                    // Spostato in bottoniera operazioni massive
                    //insertAllADL.ToolTip = "Elimina tutti i documenti trovati in 'Area di lavoro'";
                    //insertAllADL.ImageUrl = "../images/proto/cancella.gif";
                }



                //rimuovere la variabile di sessione
                DocumentManager.removeDocumentoSelezionato(this);

                userRuolo = UserManager.getRuolo(this);

                this.ListaFiltri = DocumentManager.getFiltroRicDoc(this);

                this.SetCurrentFiltersOnContext(this.ListaFiltri);

                //gestione tasto back
                if (this.ListaFiltri != null)
                {
                    Safe = new DocsPAWA.DocsPaWR.InfoUtente();
                    Safe = UserManager.getInfoUtente(this);

                    if (DocumentManager.getDatagridDocumento(this) == null)
                    {
                        if (Request.QueryString["nuovaRic"] == null)
                        {
                            ListaDoc = DocumentManager.getQueryInfoDocumentoPaging(Safe.idGruppo, Safe.idPeople, this, this.ListaFiltri, currentPage, out numTotPage, out nRec, false, false, true, !IsPostBack, out idProfile);
                            this.moButtons.InitializeOrUpdateUserControl(idProfile);
                        }
                        else
                        {//parametro di sessione per ricaricaricare il datagrid dopo che è stato eliminato un documento dall'area di lavoro
                            if (Session["aggiornaDatagrid"] == "true")
                                ListaDoc = DocumentManager.getQueryInfoDocumentoPaging(Safe.idGruppo, Safe.idPeople, this, this.ListaFiltri, currentPage, out numTotPage, out nRec, false, false, true, !IsPostBack, out idProfile);
                        }
                        this.DataGrid1.VirtualItemCount = nRec;

                        this.DataGrid1.CurrentPageIndex = currentPage - 1;

                        DocumentManager.setListaDocProt(this, ListaDoc);
                    }
                    else
                    {
                        ListaDoc = DocumentManager.getListaDocProt(this);
                    }

                    if (ListaDoc != null && ListaDoc.Length > 0)
                    {
                        if (!this.IsPostBack)
                            this.CaricaDatagridProt(this.DataGrid1, ListaDoc);

                        if (DocumentManager.getDatagridDocumento(this) != null)
                        {
                            dv = ((DataTable)DocumentManager.getDatagridDocumento(this)).DefaultView;
                            if (Session["listInArea"] != null)
                                listInArea = (Hashtable)Session["listInArea"];
                        }
                        else
                        {
                            //non so' se funziona potrebbe essere necessaria 
                            dv = this.dataSetRDoc1.Tables[0].DefaultView;
                        }
                        // gestione colonna visualizza unificata
                        //if ((System.Configuration.ConfigurationManager.AppSettings["GRD_VIS_UNIFICATA"] != null) && (System.Configuration.ConfigurationManager.AppSettings["GRD_VIS_UNIFICATA"] == "0"))
                        //{
                        //    this.DataGrid1.Columns[10].Visible = false;
                        //}

                    }

                }

            }
            catch (Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        #region Gestione call context



        /// <summary>
        /// Impostazione filtri correnti nella sessione del contesto corrente di ricerca
        /// </summary>
        /// <param name="filters"></param>
        private void SetCurrentFiltersOnContext(DocsPAWA.DocsPaWR.FiltroRicerca[][] filters)
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext != null)
            {
                // Impostazione, nell'oggetto CallContext corrente,
                // della lista dei filtri di ricerca
                currentContext.SessionState[SchedaRicerca.SESSION_KEY] = Session[SchedaRicerca.SESSION_KEY];
                currentContext.SessionState["ricDoc.listaFiltri"] = this.ListaFiltri;
            }
        }

        /// <summary>
        /// Reperimento numero pagina corrente dal contesto di ricerca
        /// </summary>
        /// <returns></returns>
        private int GetCurrentPageOnContext()
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext != null)
                return currentContext.PageNumber;
            else
                return 1;
        }

        /// <summary>
        /// Impostazione numero pagina corrente del contesto di ricerca
        /// </summary>
        /// <param name="currentPage"></param>
        private void SetCurrentPageOnContext(int currentPage)
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext != null)
                currentContext.PageNumber = currentPage;
        }

        /// <summary>
        /// Impostazione dell'indice del fascicolo
        /// selezionato nel contesto di ricerca
        /// </summary>
        /// <param name="docIndex"></param>
        private void SetCurrentDocumentIndexOnContext(int docIndex)
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext != null)
                currentContext.QueryStringParameters["docIndex"] = docIndex.ToString();
        }

        /// <summary>
        /// Ripristino del documento selezionato precedentemente
        /// </summary>
        /// <param name="dataGrid"></param>
        private void SetDocumentIndexFromQueryString(DataGrid dataGrid)
        {
            if (!this.IsPostBack)
            {
                string param = this.Request.QueryString["docIndex"];

                if (param != null && param != string.Empty)
                {
                    int documentIndex = -1;
                    try
                    {
                        documentIndex = Int32.Parse(param);
                    }
                    catch
                    {
                    }

                    dataGrid.SelectedIndex = documentIndex;
                }
            }
        }

        #endregion

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
            this.dataSetRDoc1 = new DocsPAWA.dataSet.DataSetRDoc();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetRDoc1)).BeginInit();
            // Spostato in bottoniera operazioni massive
            //this.insertAllADL.Click += new System.Web.UI.ImageClickEventHandler(this.insertAllADL_Click);
            this.insertAllCons.Click += new System.Web.UI.ImageClickEventHandler(this.insertAllCons_Click);
            this.DataGrid1.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
            this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_PageIndexChanged);
            this.DataGrid1.PreRender += new System.EventHandler(this.Datagrid1_PreRender);
            this.DataGrid1.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.Sorting);
            this.DataGrid1.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGrid1_ItemDataBound);
            this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
            this.hd1.ServerChange += new System.EventHandler(this.hd1_ServerChange);
            // 
            // dataSetRDoc1
            // 
            this.dataSetRDoc1.DataSetName = "DataSetRDoc";
            this.dataSetRDoc1.Locale = new System.Globalization.CultureInfo("en-US");
            this.Load += new System.EventHandler(this.Page_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataSetRDoc1)).EndInit();

        }
        #endregion

        #region Class per il DataGrid
        private void CaricaDatagridProt(DataGrid dg, DocsPaWR.InfoDocumento[] listaDoc)
        {
            try
            {
                InfoCurr = new DocsPAWA.DocsPaWR.InfoDocumento();
                Rows = new ArrayList();
                //ArrayList iconeRicerca = new ArrayList();

                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateRicerca"];
                DocsPAWA.DocsPaWR.OggettoCustom og1 = null;
                if (template != null)
                {
                    foreach (DocsPAWA.DocsPaWR.OggettoCustom og in template.ELENCO_OGGETTI)
                    {
                        if (og.DA_VISUALIZZARE_RICERCA.Equals("1"))
                        {
                            og1 = og;
                            break;
                        }
                    }
                }
                if (listaDoc != null)
                {
                    if (listaDoc.Length > 0)
                    {
                        string descrDoc = string.Empty;
                        bool visualizzaColonnaEsitoPubblicazione = isVisibleColEsitoPubblicazione();
                        for (int i = 0; i < listaDoc.Length; i++)
                        {
                            InfoCurr = (DocsPAWA.DocsPaWR.InfoDocumento)listaDoc[i];

                            //il campo mittDest è un array list di possibili mitt/dest lo scorro tutto e concat in una singola string con separatore ="[spazio]-[spazio]"
                            string MittDest = "";
                            int numProt = new Int32();
                            //							if(InfoCurr!=null && InfoCurr.numProt!=null ) 

                            if (InfoCurr.mittDest != null && InfoCurr.mittDest.Length > 0)
                            {
                                for (int g = 0; g < InfoCurr.mittDest.Length; g++)
                                    MittDest += InfoCurr.mittDest[g] + " - ";
                                if (InfoCurr.mittDest.Length > 0)
                                    MittDest = MittDest.Substring(0, MittDest.Length - 3);
                            }
                            else
                                MittDest += "";

                            string data = InfoCurr.dataApertura;

                            if (InfoCurr.numProt != null && !InfoCurr.numProt.Equals(""))
                            {
                                numProt = Int32.Parse(InfoCurr.numProt);
                                descrDoc = numProt.ToString();
                            }
                            else
                            {
                                descrDoc = InfoCurr.docNumber;
                            }

                            descrDoc += "<br>" + data;

                            //Protocollo titolario
                            if (!string.IsNullOrEmpty(InfoCurr.protocolloTitolario))
                                descrDoc += "<br>---------<br>" + InfoCurr.protocolloTitolario.ToString();

                            //MODIFICA DEL 20/05/2009
                            //this.dataSetRDoc1.element1.Addelement1Row(descrDoc, InfoCurr.idProfile, numProt, InfoCurr.segnatura, data, InfoCurr.codRegistro, this.GetTipo(InfoCurr), InfoCurr.oggetto, i, MittDest, InfoCurr.dataAnnullamento, InfoCurr.acquisitaImmagine, InfoCurr.inADL, InfoCurr.inConservazione, InfoCurr.cha_firmato, InfoCurr.docNumber);
                            if ((InfoCurr.contatore == null) || (InfoCurr.contatore.Equals("")))
                                DataGrid1.Columns[2].Visible = false;

                            if (og1 != null)
                            {
                                this.DataGrid1.Columns[2].HeaderText = og1.DESCRIZIONE;

                                this.dataSetRDoc1.element1.Addelement1Row(descrDoc, InfoCurr.idProfile, numProt, InfoCurr.segnatura, data, InfoCurr.codRegistro, this.GetTipo(InfoCurr), InfoCurr.oggetto, i, MittDest, InfoCurr.dataAnnullamento, InfoCurr.acquisitaImmagine, InfoCurr.inADL, InfoCurr.inConservazione, InfoCurr.cha_firmato, InfoCurr.docNumber, this.inserisciContatore(og1, InfoCurr.contatore), this.esitoPubblicazione(InfoCurr.idProfile, visualizzaColonnaEsitoPubblicazione));
                            }
                            else
                                this.dataSetRDoc1.element1.Addelement1Row(descrDoc, InfoCurr.idProfile, numProt, InfoCurr.segnatura, data, InfoCurr.codRegistro, this.GetTipo(InfoCurr), InfoCurr.oggetto, i, MittDest, InfoCurr.dataAnnullamento, InfoCurr.acquisitaImmagine, InfoCurr.inADL, InfoCurr.inConservazione, InfoCurr.cha_firmato, InfoCurr.docNumber, InfoCurr.contatore, this.esitoPubblicazione(InfoCurr.idProfile, visualizzaColonnaEsitoPubblicazione));
                            //MODIFICA DEL 20/05/2009
                        }

                        //Header protocollo titolario
                        DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                        string protocolloTitolario = wws.isEnableProtocolloTitolario();
                        if (!string.IsNullOrEmpty(protocolloTitolario) && !IsPostBack)
                        {
                            this.DataGrid1.Columns[1].HeaderText += "<br>---------<br>" + protocolloTitolario;
                        }

                        datatable = this.dataSetRDoc1.Tables[0];
                        DocumentManager.setDatagridDocumento(this, datatable);
                        DataGrid1.DataSource = datatable;
                        DataGrid1.DataBind();
                    }
                    else
                    {
                        DataGrid1.Visible = false;
                    }
                }

                else
                {

                    DataGrid1.Visible = false;
                }

            }
            catch (Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <returns></returns>
        protected string GetTipo(DocsPaWR.InfoDocumento infoDocumento)
        {
            if (infoDocumento.allegato)
            {
                //  return "ALL";
                return this.allegato;
            }
            else
            {
                if ((infoDocumento.tipoProto == "G") || (infoDocumento.tipoProto == "R"))
                {
                    //return "NP";
                    return this.grigio;
                }
                else
                {
                    if (infoDocumento.tipoProto == "A")
                    {
                        return this.arrivo;
                    }
                    else
                    {
                        if (infoDocumento.tipoProto == "I")
                        {
                            return this.interno;
                        }
                        else
                        {
                            return this.partenza;
                        }
                    }
                }
            }
        }

        public string getNumProt(int numprot)
        {
            string rtn = " ";
            try
            {
                if (numprot == 0)
                    return rtn = "";
                else
                    return rtn = numprot.ToString();
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
                return rtn;
            }
        }

        private void DataGrad1_OnPageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            // Aggiornamento dello stato di checking memorizzato dalla bottoniera
            this.moButtons.UpdateSavedCheckingStatus();

            this.DataGrid1.SelectedIndex = -1;

            DataGrid1.CurrentPageIndex = e.NewPageIndex;
            //---modificato da marco 15/01/2004
            string fieldSort;
            string direction;

            //DataGrid1.DataSource=DocumentManager.getDatagridDocumento(this);
            if (Session["fieldSorting"] == null)
            {
                DataGrid1.DataSource = DocumentManager.getDatagridDocumento(this);
            }
            else
            {
                fieldSort = (string)Session["fieldSorting"];
                if (Session["direction"] == null)
                {
                    direction = "DESC";
                }
                else
                {
                    direction = (string)Session["direction"];
                }

                this.dv = DocumentManager.getDatagridDocumento(this).DefaultView;
                dv.Sort = fieldSort + " " + direction;
                DataGrid1.DataSource = dv;
            }

            // Impostazione dello stato di flagging delle checkbox nella griglia in base
            // al valore salvato dalla bottoniera
            this.moButtons.UpdateItemCheckingStatus();

            DataGrid1.DataBind();
        }

        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }

        private void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {                
                //modifica                
                string key = ((Label)this.DataGrid1.Items[this.DataGrid1.SelectedIndex].Cells[12].Controls[1]).Text;
                //fine modifica
                string errorMessage = string.Empty;
                int documentIndex = Int32.Parse(key);

                // Lista dei system id degli elementi restituiti dalla ricerca
                SearchResultInfo[] idProfileList = new SearchResultInfo[0];

                // Impostazione documento correntemente selezionato nel contesto
                this.SetCurrentDocumentIndexOnContext(documentIndex);

                //Errore Randm Pilati: PR37159 segnalato su 3.11.3
                if (ListaDoc == null) //dal log della PAT sembra succedere che qui l'oggetto è null se è così rifaccio la ricerca
                {
                    logger.Debug("ripeto ricerca ADL, perchè l'oggetto ListaDoc è null");
                    int pagina = this.DataGrid1.CurrentPageIndex + 1;

                    // Ricerca degli elementi
                    ListaDoc = DocumentManager.getQueryInfoDocumentoPaging(Safe.idGruppo, Safe.idPeople, this, this.ListaFiltri, pagina, out numTotPage, out nRec, false, false, true, false, out idProfileList);

                }

                DocsPaWR.InfoDocumento doc = ListaDoc[documentIndex];

                //modifca del 14/05/2009
                if (doc.tipoProto.ToUpper().Equals("R"))//prima era R
                {

                    // Verifica se l'utente ha i diritti per accedere al documento
                    int retValue = DocumentManager.verificaACL("D", ((DocsPAWA.DocsPaWR.InfoDocumento)ListaDoc[Int32.Parse(key)]).idProfile, UserManager.getInfoUtente(), out errorMessage);
                    if (retValue == 0 || retValue == 1)
                    {
                        string script = ("<script>alert('" + errorMessage + "');</script>");
                        Response.Write(script);
                    }
                    else
                    {
                        DocumentManager.setRisultatoRicerca(this, (DocsPAWA.DocsPaWR.InfoDocumento)ListaDoc[Int32.Parse(key)]);
                        Session.Remove("listInArea");
                        DocumentManager.removeListaDocProt(this);//Session.Remove("tabRicDoc.Listadoc");

                        //rimuovo l'eventuale fascicolo selezionato per la ricerca, altrimenti
                        //si vede nel campo fasc rapida di profilo/protocollo
                        FascicoliManager.removeFascicoloSelezionatoFascRapida();

                        Response.Write("<script language='javascript'> top.principale.document.location = '../documento/gestionedoc.aspx?tab=profilo';</script>");
                    }
                    // this.RegisterClientScript("DocumentoTipoStampaRegistro",
                    //     "alert('Tipologia documento non visualizzabile');");
                    //fine modifica del 14/05/2009
                }
                else
                {

                    // Verifica se l'utente ha i diritti per accedere al documento
                    int retValue = DocumentManager.verificaACL("D", ((DocsPAWA.DocsPaWR.InfoDocumento)ListaDoc[Int32.Parse(key)]).idProfile, UserManager.getInfoUtente(), out errorMessage);
                    if (retValue == 0 || retValue == 1)
                    {
                        string script = ("<script>alert('" + errorMessage + "');</script>");
                        Response.Write(script);
                    }
                    else
                    {
                        DocumentManager.setRisultatoRicerca(this, (DocsPAWA.DocsPaWR.InfoDocumento)ListaDoc[Int32.Parse(key)]);
                        //	DocumentManager.removeFiltroRicDoc(this);

                        Session.Remove("listInArea");
                        DocumentManager.removeListaDocProt(this);//Session.Remove("tabRicDoc.Listadoc");

                        //rimuovo l'eventuale fascicolo selezionato per la ricerca, altrimenti
                        //si vede nel campo fasc rapida di profilo/protocollo
                        FascicoliManager.removeFascicoloSelezionatoFascRapida();

                        //Response.Write("<script language='javascript'>var k=window.open('../documento/gestionedoc.aspx?tab=protocollo','principale','');</script>");
                        Response.Write("<script language='javascript'> top.principale.document.location = '../documento/gestionedoc.aspx?tab=protocollo';</script>");
                    }
                }

                //Inserisco in sessione l'oggetto per lo scorrimento continuo della lista
                //UserControls.ScrollElementsList.ScrollManager.setInSessionNewObjScrollElementsList(DataGrid1.VirtualItemCount, DataGrid1.PageCount, DataGrid1.PageSize, documentIndex, DataGrid1.CurrentPageIndex, new ArrayList(ListaDoc), UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_DOCUMENTI);
                UserControls.ScrollElementsList.ScrollManager.setInContextNewObjScrollElementsList(DataGrid1.VirtualItemCount, DataGrid1.PageCount, DataGrid1.PageSize, documentIndex, DataGrid1.CurrentPageIndex, new ArrayList(ListaDoc), UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_DOCUMENTI);

            }
            catch (Exception ex)
            {
                Exception eApp = null;
                var err = "";
                string PagChiamante = "RicercaDoc";
                if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                    PagChiamante = "RicercaADLDoc";
                err = string.Format("{0} \r\n {1} \r\n {2}", "Pagina chiamante: " + PagChiamante, "Errore: " + ex.Message.ToString(), "StackTrace: " + ex.StackTrace.ToString());
                eApp = new Exception(err);

                //ErrorManager.OpenErrorPage(this, err,PagChiamante);
                ErrorManager.redirectToErrorPage(this, eApp);
            }
        }


        private string DirectionSorting
        {
            get
            {
                string retValue;
                if (ViewState["directionSorting"] == null)
                {
                    ViewState["directionSorting"] = "ASC";
                }

                retValue = (string)ViewState["directionSorting"];
                return retValue;
            }
            set
            {
                ViewState["directionSorting"] = value;
            }
        }

        private void ChangeDirectionSorting(string oldDirection)
        {
            string newValue;
            if (oldDirection != null && oldDirection.Equals("ASC"))
            {
                newValue = "DESC";
            }
            else
            {
                newValue = "ASC";
            }
            DirectionSorting = newValue;
        }

        private void Sorting(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
        {
            string sortExpression;
            string direction;
            sortExpression = e.SortExpression;
            direction = this.DirectionSorting;
            ChangeDirectionSorting(direction);
            dv.Sort = sortExpression + " " + direction;
            bindDataGrid(dv);
            //Inserisco nella session perchè mi servono quando si cambia pagina ---modificato da marco 15/01/2004
            Session["fieldSorting"] = sortExpression;
            Session["direction"] = direction;
        }

        private void DataGrdi1_OnItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {


        }

        private void bindDataGrid(DataView dataView)
        {
            DataGrid1.DataSource = dataView;
            DataGrid1.DataBind();
        }

        private void Datagrid1_PreRender(object sender, System.EventArgs e)
        {
            try
            {
                this.SetDocumentIndexFromQueryString((DataGrid)sender);

                if (!this.IsPostBack)
                {
                    // Impostazione visibilità controlli
                    this.SetControlsVisibility();

                    this.RefreshCountDocumenti();
                }

                for (int i = 0; i < this.DataGrid1.Items.Count; i++)
                {
                    if (this.DataGrid1.Items[i].ItemIndex >= 0)
                    {
                        //((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[13].Controls[1]).Attributes.Add("OnClick","ApriModalDialog();");
                        if (Session["listInArea"] != null)
                        {
                            listInArea = new Hashtable();
                            //24/01/2005
                            listInArea = (System.Collections.Hashtable)Session["listInArea"];
                            string chiave = ((Label)DataGrid1.Items[i].Cells[4].Controls[1]).Text + ((Label)DataGrid1.Items[i].Cells[7].Controls[1]).Text;
                            //if(listInArea.ContainsKey(Int32.Parse(((Label)this.DataGrid1.Items[i].Cells[9].Controls[1]).Text))	)
                            if (listInArea.ContainsKey(chiave))
                            {
                                // ((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[12].Controls[1]).Enabled = false;

                            }

                        }
                        ////verifico e customizzo il datagrid per la nuova ADL
                        //if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                        //{
                        //    //setto le proprietà video
                        //    ((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[12].Controls[1]).ToolTip = "Elimina questo documento da 'Area di lavoro'";
                        //    ((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[12].Controls[1]).ImageUrl = "../images/proto/cancella.gif";
                        //    ((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[12].Controls[1]).Attributes.Add("OnClick", "ApriModalDialogNewADL();");


                        //}
                        //if (((Label)DataGrid1.Items[i].Cells[16].Controls[1]).Text == "1")
                        //{
                        //    //setto le proprietà video
                        //    ((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[13].Controls[1]).ToolTip = "Elimina questo documento da 'Area di lavoro'";
                        //    ((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[13].Controls[1]).ImageUrl = "../images/proto/cancella.gif";
                        //    ((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[13].Controls[1]).Attributes.Add("OnClick", "ApriModalDialogNewADL();");
                        //    ((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[13].Controls[1]).CommandName = "EliminaADL";
                        //}

                        // inserire la giusta immagine
                        //if (((Label)DataGrid1.Items[i].Cells[18].Controls[1]).Text == "1")
                        //{
                        //    ((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[17].Controls[1]).ToolTip = "Elimina questo documento da 'Area di conservazione'";
                        //    ((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[17].Controls[1]).ImageUrl = "../images/proto/cancella.gif";
                        //    ((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[17].Controls[1]).CommandName = "EliminaAreaCons";
                        //}
                        //if (((Label)DataGrid1.Items[i].Cells[18].Controls[1]).Text == "2")
                        //{
                        //    ((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[17].Controls[1]).ToolTip = "Elimina questo documento da 'Area di conservazione'";
                        //    ((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[17].Controls[1]).ImageUrl = "../images/proto/cancella.gif";
                        //    ((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[17].Controls[1]).CommandName = "DocInFascicolo";
                        //}
                        this.SetForeColorColumnDescrDocumento(this.DataGrid1.Items[i], this.DataGrid1.SelectedIndex);
                    }
                }

                //Verifico se visualizzare o meno la colonna contatore della tipologia
                //modifica
                this.DataGrid1.Columns[6].Visible = isVisibleColEsitoPubblicazione();
                this.DataGrid1.Columns[2].Visible = isVisibleColContatore();
                //fine modifica
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
            }
        }

        /// <summary>
        /// Impostazione del colore del carattere per la prima colonna della griglia:
        /// rosso se doc protocollato, altrimenti grigio 
        /// </summary>
        /// <param name="item"></param>
        private void SetForeColorColumnDescrDocumento(DataGridItem item, int index)
        {
            Label lbl = item.Cells[4].Controls[1] as Label;

            if (lbl != null)
            {
                item.Cells[1].Font.Bold = true;

                if (lbl.Text.Equals(""))
                {
                    if (item.ItemIndex != index)
                        ((LinkButton)item.Cells[1].Controls[1]).ForeColor = Color.Black;
                    else
                        ((LinkButton)item.Cells[1].Controls[1]).ForeColor = Color.White;
                }
                else
                {
                    //string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    string idAmm = string.Empty;
                    if ((string)Session["AMMDATASET"] != null)
                        idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                    else
                        if (UserManager.getInfoUtente() != null && !string.IsNullOrEmpty(UserManager.getInfoUtente().idAmministrazione))
                            idAmm = UserManager.getInfoUtente().idAmministrazione;

                    DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    string segnAmm = "0";
                    if(!string.IsNullOrEmpty(idAmm))
                        segnAmm = ws.getSegnAmm(idAmm);
                    switch (segnAmm)
                    {
                        case "0":
                            ((LinkButton)item.Cells[1].Controls[1]).ForeColor = Color.Black;
                            break;

                        case "1":
                            ((LinkButton)item.Cells[1].Controls[1]).ForeColor = Color.Blue;
                            break;

                        case "2":
                        default:
                            ((LinkButton)item.Cells[1].Controls[1]).ForeColor = Color.Red;
                            break;
                    }
                }
                lbl = null;
            }

            string dataAnnull = ((TableCell)item.Cells[13]).Text;

            if(!string.IsNullOrEmpty(dataAnnull) && !dataAnnull.Equals("&nbsp;"))
            {
                item.ForeColor = Color.Red;
                ((LinkButton)item.Cells[1].Controls[1]).ForeColor = Color.Red;
                item.Font.Bold = true;
                item.Font.Strikeout = true;
            }
        }

        private void DataGrid1_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            //Utils.checkForDateNullOnItem(e.Item);
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //int indiceSel = this.DataGrid1.SelectedIndex;
                //this.SetForeColorColumnDescrDocumento(e.Item, indiceSel);

                string dataAnnull = ((TableCell)e.Item.Cells[13]).Text;

                try
                {
                    DateTime dt = Convert.ToDateTime(dataAnnull);
                    e.Item.ForeColor = Color.Red;
                    e.Item.Font.Bold = true;
                    e.Item.Font.Strikeout = true;
                }
                catch { }


                // gestione della icona dei dettagli -- tolgo icona se non c'è doc acquisito
                //string acquisitaImmagine = ((TableCell)e.Item.Cells[15]).Text;
                //ImageButton imgbtn = new ImageButton();
                //if (acquisitaImmagine != null && acquisitaImmagine.Equals("0"))
                //{
                //    //ImageButton imgbtn = new ImageButton();
                //    if (e.Item.Cells[11].Controls[1].GetType().Equals(typeof(ImageButton)))
                //    {
                //        imgbtn = (ImageButton)e.Item.Cells[11].Controls[1];
                //        imgbtn.Visible = false;
                //    }
                //}
                //else
                //{
                //    if (acquisitaImmagine != null)
                //    {
                //        string image = FileManager.getFileIcon(Page, acquisitaImmagine.Trim().ToLower());

                //        if (e.Item.Cells[11].Controls[1].GetType().Equals(typeof(ImageButton)))
                //        {
                //            imgbtn = (ImageButton)e.Item.Cells[11].Controls[1];
                //            imgbtn.ImageUrl = image;
                //        }
                //    }
                //}
            }


        }

        private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            Label lblDocSel = new Label();
            try
            {
                //    DocsPaWebCtrlLibrary.ImageButton imgbtn=new DocsPaWebCtrlLibrary.ImageButton();

                //    try
                //    {
                //        //if (e.CommandName.Equals("Area"))
                //        //{
                //        //    if (e.Item.Cells[12].Controls[1].GetType().Equals(typeof(System.Web.UI.WebControls.Label)))
                //        //        lblDocSel = (Label)e.Item.Cells[12].Controls[1];

                //        //    int keyN = Int32.Parse(lblDocSel.Text);
                //        //    //	Response.Write("<script language='javascript'>return val=window.confirm('Vuoi Inserire il Documento in area di lavoro ?')</script>");
                //        //    string h = Request.Form["hd1"];
                //        //    if (this.hd1.Value.Equals("Yes"))
                //        //    {
                //        //        schedaSel = DocumentManager.getDettaglioDocumento(this, this.ListaDoc[keyN].idProfile, this.ListaDoc[keyN].docNumber);

                //        //        //se ho attiva la nuova ADL devo invertire la funzionalità
                //        //        if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                //        //        {
                //        //            DocumentManager.eliminaDaAreaLavoro(this, schedaSel.systemId, null);
                //        //            //riavvio la ricerca
                //        //            string fromPage = Request.QueryString["from"].ToString();
                //        //            ClientScript.RegisterStartupScript(this.GetType(), "LanciaRic", "top.principale.document.location = 'gestioneRicDoc.aspx?tab=" + fromPage + "&ricADL=1&from=" + fromPage + "';", true);

                //        //        }
                //        //        else //normale comportamento
                //        //        {
                //        //            DocumentManager.addAreaLavoro(this, schedaSel);

                //        //            //setto le proprietà video
                //        //            ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[13].Controls[1]).ToolTip = "Elimina questo documento da 'Area di lavoro'";
                //        //            ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[13].Controls[1]).ImageUrl = "../images/proto/cancella.gif";
                //        //            ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[13].Controls[1]).Attributes.Add("OnClick", "ApriModalDialogNewADL();");
                //        //            ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[13].Controls[1]).CommandName = "EliminaADL";
                //        //            ((Label)(e.Item.Cells[16].Controls[1])).Text = "1";

                //        //            if (e.Item.Cells[13].Controls[1].GetType().Equals(typeof(DocsPaWebCtrlLibrary.ImageButton)))
                //        //            {
                //        //                //imgbtn = (DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[12].Controls[1];
                //        //                //imgbtn.Enabled = false;
                //        //                if (Session["listInArea"] != null)
                //        //                {
                //        //                    listInArea = (Hashtable)Session["listInArea"];
                //        //                    //if(listInArea.ContainsKey(/*keyN*/ListaDoc[keyN].numProt+ListaDoc[keyN].dataApertura)==false)
                //        //                    //	listInArea.Add(/*keyN*/ListaDoc[keyN].numProt+ListaDoc[keyN].dataApertura,ListaDoc[keyN]);
                //        //                    if (listInArea.ContainsKey(ListaDoc[keyN].docNumber) == false)
                //        //                        listInArea.Add(ListaDoc[keyN].docNumber, ListaDoc[keyN]);

                //        //                    Session["listInArea"] = listInArea;
                //        //                }
                //        //                else
                //        //                {
                //        //                    listInArea = new Hashtable();
                //        //                    listInArea.Add(/*keyN*/ListaDoc[keyN].numProt + ListaDoc[keyN].dataApertura, ListaDoc[keyN]);
                //        //                    Session["listInArea"] = listInArea;
                //        //                }
                //        //            }
                //        //        }
                //        //    }
                //        //}
                //        //if (e.CommandName.Equals("EliminaADL"))
                //        //{
                //        //        if(e.Item.Cells[12].Controls[1].GetType().Equals(typeof(System.Web.UI.WebControls.Label)))
                //        //        lblDocSel = (Label) e.Item.Cells[12].Controls[1];

                //        //    int  keyN = Int32.Parse(lblDocSel.Text);
                //        //    //	Response.Write("<script language='javascript'>return val=window.confirm('Vuoi Inserire il Documento in area di lavoro ?')</script>");
                //        //    string h=	Request.Form["hd1"];
                //        //    if (this.hd1.Value.Equals("Yes"))
                //        //    {
                //        //        schedaSel = DocumentManager.getDettaglioDocumento(this, this.ListaDoc[keyN].idProfile, this.ListaDoc[keyN].docNumber);
                //        //        DocumentManager.eliminaDaAreaLavoro(this, schedaSel.systemId, null);

                //        //        //se ho attiva la nuova ADL devo invertire la funzionalità
                //        //        if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                //        //        {
                //        //            string fromPage = Request.QueryString["from"].ToString();
                //        //            ClientScript.RegisterStartupScript(this.GetType(), "LanciaRic", "top.principale.document.location = 'gestioneRicDoc.aspx?tab=" + fromPage + "&ricADL=1&from=" + fromPage + "';", true);

                //        //        }
                //        //        else
                //        //        {
                //        //            //setto le proprietà video
                //        //            ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[13].Controls[1]).ToolTip = "Inserisci questo documento da 'Area di lavoro'";
                //        //            ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[13].Controls[1]).ImageUrl = "../images/proto/ins_area.gif";
                //        //            ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[13].Controls[1]).Attributes.Add("OnClick", "ApriModalDialogNew();");
                //        //            ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[13].Controls[1]).CommandName = "Area";
                //        //        }
                //        //        ((Label)(e.Item.Cells[16].Controls[1])).Text = "0";

                //        //        if (e.Item.Cells[12].Controls[1].GetType().Equals(typeof(DocsPaWebCtrlLibrary.ImageButton)))
                //        //        {
                //        //            //imgbtn = (DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[12].Controls[1];
                //        //            //imgbtn.Enabled = false;
                //        //            if (Session["listInArea"] != null)
                //        //            {
                //        //                listInArea = (Hashtable)Session["listInArea"];
                //        //                //if(listInArea.ContainsKey(/*keyN*/ListaDoc[keyN].numProt+ListaDoc[keyN].dataApertura)==false)
                //        //                //	listInArea.Add(/*keyN*/ListaDoc[keyN].numProt+ListaDoc[keyN].dataApertura,ListaDoc[keyN]);
                //        //                if (listInArea.ContainsKey(ListaDoc[keyN].docNumber) == false)
                //        //                    listInArea.Add(ListaDoc[keyN].docNumber, ListaDoc[keyN]);

                //        //                Session["listInArea"] = listInArea;
                //        //            }
                //        //            else
                //        //            {
                //        //                listInArea = new Hashtable();
                //        //                listInArea.Add(/*keyN*/ListaDoc[keyN].numProt + ListaDoc[keyN].dataApertura, ListaDoc[keyN]);
                //        //                Session["listInArea"] = listInArea;
                //        //            }
                //        //        }
                //        //    }
                //        //}
                //        //if (e.CommandName.Equals("AreaConservazione"))
                //        //{

                //        //    lblDocSel = new Label();
                //        //    if (e.Item.Cells[12].Controls[1].GetType().Equals(typeof(System.Web.UI.WebControls.Label)))
                //        //        lblDocSel = (Label)e.Item.Cells[12].Controls[1];

                //        //    int keyN = Int32.Parse(lblDocSel.Text);

                //        //    schedaSel = DocumentManager.getDettaglioDocumento(this, this.ListaDoc[keyN].idProfile, this.ListaDoc[keyN].docNumber);

                //        //    if (Convert.ToInt32(schedaSel.documenti[0].fileSize) > 0)
                //        //    {
                //        //        int isPrimaIstanza = DocumentManager.isPrimaConservazione(this, Safe.idPeople, Safe.idGruppo);
                //        //        if (isPrimaIstanza == 1)
                //        //        {
                //        //            string popup = "<script> alert('Si sta per creare una nuova istanza di conservazione')</script>";
                //        //            Page.RegisterClientScriptBlock("popUp", popup);

                //        //        }
                //        //        string sysId = DocumentManager.addAreaConservazione(this, this.ListaDoc[keyN].idProfile, "", this.ListaDoc[keyN].docNumber, Safe, "D");

                //        //        if (sysId != "-1")
                //        //        {
                //        //            int size_xml = DocumentManager.getItemSize(Page, schedaSel, sysId);
                //        //            int doc_size = Convert.ToInt32(schedaSel.documenti[0].fileSize);

                //        //            int numeroAllegati = schedaSel.allegati.Length;
                //        //            string fileName = schedaSel.documenti[0].fileName;
                //        //            string tipoFile = System.IO.Path.GetExtension(fileName);

                //        //            int size_allegati = 0;
                //        //            for (int i = 0; i < schedaSel.allegati.Length; i++)
                //        //            {
                //        //                size_allegati = size_allegati + Convert.ToInt32(schedaSel.allegati[i].fileSize);
                //        //            }
                //        //            int total_size = size_allegati + doc_size + size_xml;

                //        //            DocumentManager.insertSizeInItemCons(Page, sysId, total_size);

                //        //            DocumentManager.updateItemsConservazione(Page, tipoFile, Convert.ToString(numeroAllegati), sysId);


                //        //            ((Label)(e.Item.Cells[18].Controls[1])).Text = "1";
                //        //        }

                //        //    }
                //        //    else
                //        //    {
                //        //        string popup = "<script> alert('Il documento principale non ha alcun file associato, impossibile inserirlo in area conservazione')</script>";
                //        //        Page.RegisterClientScriptBlock("popUp", popup);
                //        //    }
                //        //}
                //        //if (e.CommandName.Equals("EliminaAreaCons"))
                //        //{
                //        //    lblDocSel = new Label();
                //        //    if (e.Item.Cells[12].Controls[1].GetType().Equals(typeof(System.Web.UI.WebControls.Label)))
                //        //        lblDocSel = (Label)e.Item.Cells[12].Controls[1];

                //        //    int keyN = Int32.Parse(lblDocSel.Text);

                //        //    schedaSel = DocumentManager.getDettaglioDocumento(this, this.ListaDoc[keyN].idProfile, this.ListaDoc[keyN].docNumber);

                //        //    if (DocumentManager.canDeleteAreaConservazione(Page, schedaSel.systemId, Safe.idPeople, Safe.idGruppo)==0)
                //        //    {
                //        //        string popup = "<script> alert('Impossibile eliminare il documento da Area di conservazione')</script>";
                //        //        Page.RegisterClientScriptBlock("alert", popup);
                //        //    }
                //        //    else
                //        //    {
                //        //        DocumentManager.eliminaDaAreaConservazione(Page, schedaSel.systemId, null, null, false, "");
                //        //        ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[17].Controls[1]).ToolTip = "Inserisci questo documento in 'Area di conservazione'";
                //        //        ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[17].Controls[1]).ImageUrl = "../images/proto/conservazione_d.gif";
                //        //        ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[17].Controls[1]).CommandName = "AreaConservazione";
                //        //        ((Label)(e.Item.Cells[18].Controls[1])).Text = "0";
                //        //    }  
                //        //}
                //        // visualizza doc in popup
                //        //if (e.CommandName.Equals("VisDoc"))
                //        //{
                //        //    if(e.Item.Cells[12].Controls[1].GetType().Equals(typeof(System.Web.UI.WebControls.Label)))
                //        //        lblDocSel = (Label) e.Item.Cells[12].Controls[1];
                //        //    int  keyN = Int32.Parse(lblDocSel.Text);
                //        //    //vis unificata
                //        //    schedaSel = DocumentManager.getDettaglioDocumento(this, this.ListaDoc[keyN].idProfile, this.ListaDoc[keyN].docNumber);
                //        //    DocumentManager.setDocumentoSelezionato(this, schedaSel);
                //        //    FileManager.setSelectedFile(this, schedaSel.documenti[0], false);
                //        //    ClientScript.RegisterStartupScript(this.GetType(), "lanciaVIs", "loadvisualizzaDoc('" + Session.SessionID + "','" +this.ListaDoc[keyN].docNumber+ "','" +this.ListaDoc[keyN].idProfile+ "');", true);
                //        //}
                //    }
                //    catch(Exception ex)
                //    {
                //        ErrorManager.redirectToErrorPage(this,ex);
                //}
                Session["aggiornaDatagrid"] = string.Empty;
                if (e.CommandName != "Page")
                {
                    //modifica
                    if (e.Item.Cells[12].Controls[1].GetType().Equals(typeof(System.Web.UI.WebControls.Label)))
                        lblDocSel = (Label)e.Item.Cells[12].Controls[1];
                    //fine modifica
                    int keyN = Int32.Parse(lblDocSel.Text);
                    this.SetCurrentDocumentIndexOnContext(keyN);
                    this.DataGrid1.SelectedIndex = keyN;
                }
                if (e.CommandName == "EliminaADL")
                {
                    Session["aggiornaDatagrid"] = "true";
                    //DocumentManager.removeDatagridDocumento(this);
                    SearchResultInfo[] idProfilesList = new SearchResultInfo[0];
                    ListaDoc = DocumentManager.getQueryInfoDocumentoPaging(Safe.idGruppo, Safe.idPeople, this, this.ListaFiltri, currentPage, out numTotPage, out nRec, false, false, true, false, out idProfilesList);
                    DocumentManager.setListaDocProt(this, ListaDoc);
                    this.CaricaDatagridProt(this.DataGrid1, ListaDoc);
                    this.RefreshCountDocumenti();
                }
            }
            catch (Exception ex)
            {
                Exception eApp = null;
                var err = "";
                string PagChiamante = "RicercaDoc";
                if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                    PagChiamante = "RicercaADLDoc";
                err = string.Format("{0} \r\n {1} \r\n {2}", "Pagina chiamante: " + PagChiamante, "Errore: " + ex.Message.ToString(), "StackTrace: " + ex.StackTrace.ToString());
                eApp = new Exception(err);

                //ErrorManager.OpenErrorPage(this, err,PagChiamante);
                ErrorManager.redirectToErrorPage(this, eApp);
            }
        }

        private void hd1_ServerChange(object sender, System.EventArgs e)
        {

        }

        private void insertAllADL_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {

                userRuolo = UserManager.getRuolo(this);
                this.ListaFiltri = DocumentManager.getFiltroRicDoc(this);
                DocsPAWA.DocsPaWR.InfoDocumento[] listaD = null;

                //gestione tasto back
                if (this.ListaFiltri != null) // && this.cb_All.Checked)
                {
                    Safe = new DocsPAWA.DocsPaWR.InfoUtente();
                    Safe = UserManager.getInfoUtente(this);
                    listaD = DocumentManager.getDocAll(Safe, this, this.ListaFiltri);
                }

                //DocsPaWR.InfoDocumento[] listaD=DocumentManager.getListaDocProt(this);
                //DocsPaWR.SchedaDocumento sch=new DocsPAWA.DocsPaWR.SchedaDocumento();

                //verifica per nuova ADL-- viene sostituita la funzionalità di inserisci con elimina
                if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                {

                    for (int h = 0; h < listaD.Length; h++)
                    {
                        //  sch = DocumentManager.getDettaglioDocumento(this, listaD[h].idProfile, listaD[h].docNumber);
                        DocumentManager.eliminaDaAreaLavoro(this, listaD[h].idProfile, null);
                    }

                    if (!this.IsClientScriptBlockRegistered("ADLInserted"))
                    {
                        string script = "<script>alert('Eliminazione dei documenti in ADL avvenuta correttamente.');</script>";
                        this.RegisterClientScriptBlock("ADLInserted", script);
                    }
                    //riavvio la ricerca
                    string fromPage = Request.QueryString["from"].ToString();
                    ClientScript.RegisterStartupScript(this.GetType(), "LanciaRic", "top.principale.document.location = 'gestioneRicDoc.aspx?tab=" + fromPage + "&ricADL=1&from=" + fromPage + "';", true);

                }
                else
                {
                    //normale gestione 
                    for (int h = 0; h < listaD.Length; h++)
                    {
                        //sch = DocumentManager.getDettaglioDocumento(this, listaD[h].idProfile, listaD[h].docNumber);
                        DocumentManager.addAreaLavoro(this, (DocsPaWR.InfoDocumento)listaD[h]);

                    }

                    if (!this.IsClientScriptBlockRegistered("ADLInserted"))
                    {
                        string script = "<script>alert('Inserimento dei documenti in ADL avvenuto correttamente.');</script>";
                        this.RegisterClientScriptBlock("ADLInserted", script);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
            }
        }


        private void DataGrid1_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            this.DataGrid1.SelectedIndex = -1;

            DataGrid1.CurrentPageIndex = e.NewPageIndex;
            numTotPage = 0;
            int nRec;
            SearchResultInfo[] idProfilesList = new SearchResultInfo[0];

            this.moButtons.UpdateSavedCheckingStatus();

            ListaDoc = DocumentManager.getQueryInfoDocumentoPaging(Safe.idGruppo, Safe.idPeople, this, this.ListaFiltri, e.NewPageIndex + 1, out numTotPage, out nRec, false, false, true, false, out idProfilesList);
            DocumentManager.setListaDocProt(this, ListaDoc);
            this.CaricaDatagridProt(this.DataGrid1, ListaDoc);
            /*this.DataGrid1.DataSource = ListaDoc ;
            this.DataGrid1.DataBind();*/

            //gestione tasto back
            int numPag = e.NewPageIndex + 1;
            DocumentManager.setMemoriaNumPag(this, numPag.ToString());

            this.SetCurrentPageOnContext(numPag);

            this.moButtons.UpdateItemCheckingStatus();

        }



        //class per caricare il datagrid.	
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        protected void PrintMsg(string msg)
        {
            this.titolo.Text = msg;
            if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                this.msgADL.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshCountDocumenti()
        {
            string msg = "Elenco documenti";

            if (this.ListaDoc != null)
                msg += " - Trovati " + this.nRec.ToString() + " elementi.";

            this.PrintMsg(msg);
        }

        /// <summary>
        /// Impostazione visibilità controlli
        /// </summary>
        private void SetControlsVisibility()
        {
            this.trHeader.Visible = true;

            // Spostati in bottoniera operazioni massive
            //this.btn_stampa.Visible = (this.ListaDoc != null && this.ListaDoc.Length > 0);
            //this.insertAllADL.Visible = this.btn_stampa.Visible;
            this.insertAllCons.Visible = (this.ListaDoc != null && this.ListaDoc.Length > 0);


            this.trBody.Visible = (this.DataGrid1.Items.Count > 0);

            if (this.DataGrid1.Items.Count > 0)
            {
                if (!UserManager.ruoloIsAutorized(this.Page, "DO_CONS"))
                {
                    //this.DataGrid1.Columns[16].Visible = false;
                    this.insertAllCons.Visible = false;
                }


                //Lista di documenti in deposito, non si può:
                //- inserire in area lavoro
                //- inserire in area conservazione
                //Trasferimento deposito
                //if (UserManager.ruoloIsAutorized(this, "GEST_ARCHIVIA"))
                //{
                //    foreach (DocsPAWA.DocsPaWR.FiltroRicerca[] filterArray in this.ListaFiltri)
                //        foreach (DocsPAWA.DocsPaWR.FiltroRicerca filterItem in filterArray)
                //        {
                //            if (filterItem.argomento.Equals("DEPOSITO") && !filterItem.valore.Equals(""))
                //            {
                //                if (filterItem.valore.Equals("D"))
                //                {
                //                    this.DataGrid1.Columns[12].Visible = false;
                //                    this.DataGrid1.Columns[15].Visible = false;
                //                    this.DataGrid1.Columns[16].Visible = false;
                //                    this.insertAllADL.Visible = false;
                //                    this.insertAllCons.Visible = false;
                //                }

                //            }
                //        }
                //}
            }
        }

        /// <summary>
        /// Impostazione visibilità colonna "Registro" che è 
        /// visualizzata solamente in caso di ricerca fulltext
        /// </summary>
        private void SetVisibilityColumnRegistro()
        {
            // Reperimento valore dal parametro in querystring "full_text"
            //this.DataGrid1.Columns[5].Visible = (!Convert.ToBoolean(Request.QueryString["full_text"]));
            //this.DataGrid1.Columns[7].Visible = (!Convert.ToBoolean(Request.QueryString["full_text"]));
            this.DataGrid1.Columns[8].Visible = (!Convert.ToBoolean(Request.QueryString["full_text"]));
        }

        private void AttatchGridPagingWaitControl()
        {
            DataGridPagingWaitControl.DataGridID = this.DataGrid1.ClientID;
            DataGridPagingWaitControl.WaitScriptCallback = "WaitDataGridCallback(eventTarget,eventArgument);";
        }

        private waiting.DataGridPagingWait DataGridPagingWaitControl
        {
            get
            {
                return this.FindControl("DataGridPagingWait1") as waiting.DataGridPagingWait;
            }
        }

        protected void DataGrid1_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {

                if (e.Item.Cells.Count > 0)
                {

                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }

            }


        }

        protected void insertAllCons_Click(object sender, ImageClickEventArgs e)
        {
            
            DocsPAWA.DocsPaWR.FiltroRicerca[][] filtroRicerca = DocumentManager.getFiltroRicDoc(this);
            //int isPrimaIstanza = DocumentManager.isPrimaConservazione(Page, UserManager.getInfoUtente(Page).idPeople, UserManager.getInfoUtente(Page).idGruppo);
            //if (isPrimaIstanza == 1)
            //{
            //    string popup = "<script> ('Si sta per creare una nuova istanza di conservazione')</script>";
            //    Page.RegisterClientScriptBlock("popUp", popup);
            //}
            bool result = DocumentManager.insertAllInCons(this, filtroRicerca, Safe);
            
            if (result)
            {
                SearchResultInfo[] idProfilesList;
                ListaDoc = DocumentManager.getQueryInfoDocumentoPaging(Safe.idGruppo, Safe.idPeople, this, this.ListaFiltri, currentPage, out numTotPage, out nRec, false, false, true, false, out idProfilesList);
                DocumentManager.setListaDocProt(this, ListaDoc);
                this.CaricaDatagridProt(this.DataGrid1, ListaDoc);
                Response.Write("<SCRIPT>alert(\"L\'operazione è andata a buon fine\");</SCRIPT>");
            }
        }
        //modiifca
        protected bool isVisibleColContatore()
        {
            bool result = false;
            if (Session["templateRicerca"] != null)
            {
                DocsPaWR.Templates template = (DocsPaWR.Templates)Session["templateRicerca"];
                foreach (DocsPaWR.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                {
                    if (oggettoCustom.DA_VISUALIZZARE_RICERCA == "1")
                    {

                        this.DataGrid1.Columns[2].HeaderText = oggettoCustom.DESCRIZIONE;
                        result = true;
                    }
                }
            }

            return result;
        }


        public string inserisciContatore(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string paramContatore)
        {
            string[] formatoContDaFunzione = paramContatore.Split('-');
            string[] formatoContDaSostituire = new string[] { "A", "A", "A" };

            //for (int i = 0; i < formatoContDaSostituire.Length; i++)
            //    formatoContDaSostituire[i] = string.Empty;

            formatoContDaFunzione.CopyTo(formatoContDaSostituire, 0);

            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return paramContatore;
            }

            //Imposto il contatore in funzione del formato
            string contatore = string.Empty;
            if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
            {


                contatore = oggettoCustom.FORMATO_CONTATORE;
                contatore = contatore.Replace("ANNO", formatoContDaSostituire[1].ToString());
                contatore = contatore.Replace("CONTATORE", formatoContDaSostituire[2].ToString());
                if (!string.IsNullOrEmpty(formatoContDaSostituire[0]))
                {
                    contatore = contatore.Replace("RF", formatoContDaSostituire[0].ToString());
                    contatore = contatore.Replace("AOO", formatoContDaSostituire[0].ToString());
                }
                else
                {
                    contatore = contatore.Replace("RF", "A");
                    contatore = contatore.Replace("AOO", "A");
                }
            }
            else
            {
                contatore = paramContatore;
            }
            return this.eliminaBlankSegnatura(contatore);

        }

        private string eliminaBlankSegnatura(string paramSegnatura)
        {
            char separatore = '|';
            string[] temp = paramSegnatura.Split('|');
            string appoggio = string.Empty;

            if (temp.Length == 1)
            {
                temp = paramSegnatura.Split('-');
                separatore = '-';
            }

            for (int i = 0; i < temp.Length; i++)
            {
                if (!temp[i].Equals("A"))
                {
                    appoggio += temp[i];
                    if (i != temp.Length - 1)
                        appoggio += separatore;
                }
            }
            return appoggio;
        }



        //fine modifica

        /// <summary>
        /// Prende i dati esistenti per le etichette dei protocolli (Inserita da Fabio)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void getLettereProtocolli()
        {
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
            DocsPAWA.DocsPaWR.InfoUtente infoUtente = session.getUserAmmSession();
            String idAmm = null;
            if (UserManager.getInfoUtente() != null && !string.IsNullOrEmpty(UserManager.getInfoUtente().idAmministrazione))
                idAmm = UserManager.getInfoUtente().idAmministrazione;
            this.etichette = wws.getEtichetteDocumenti(infoUtente, idAmm);
            this.arrivo = etichette[0].Descrizione; //Valore A
            this.partenza = etichette[1].Descrizione; //Valore P
            this.interno = etichette[2].Descrizione; //Valore I
            this.grigio = etichette[3].Descrizione; //Valore G
            this.allegato = etichette[4].Descrizione; //Valore ALL

        }

        protected bool isVisibleColEsitoPubblicazione()
        {
            bool result = false;
            if (Session["templateRicerca"] != null)
            {
                DocsPaWR.Templates template = (DocsPaWR.Templates)Session["templateRicerca"];
                result = wss.isExsistDocumentPubblicazione(template.ID_TIPO_ATTO);

            }

            return result;
        }


        public string esitoPubblicazione(string idProfile, bool visualizzaColonna)
        {
            string esito = string.Empty;
            if (visualizzaColonna)
            {
                DocsPaWR.PubblicazioneDocumenti pubblicazione = wss.getPubblicazioneDocumentiByIdProfile(idProfile);
                if (pubblicazione != null)
                {
                    if (!string.IsNullOrEmpty(pubblicazione.SYSTEM_ID))
                    {
                        if (pubblicazione.ESITO_PUBBLICAZIONE.Equals("1") &&
                            string.IsNullOrEmpty(pubblicazione.ERRORE_PUBBLICAZIONE))
                            esito = "Il documento è stato pubblicato";
                        else
                            if ((pubblicazione.ESITO_PUBBLICAZIONE.Equals("1") &&
                            !string.IsNullOrEmpty(pubblicazione.ERRORE_PUBBLICAZIONE))
                            || (pubblicazione.ESITO_PUBBLICAZIONE.Equals("0") &&
                            !string.IsNullOrEmpty(pubblicazione.ERRORE_PUBBLICAZIONE)))
                                esito = "Il documento non è stato pubblicato " + pubblicazione.ERRORE_PUBBLICAZIONE;
                            else
                                if ((pubblicazione.ESITO_PUBBLICAZIONE.Equals("0") &&
                                  string.IsNullOrEmpty(pubblicazione.ERRORE_PUBBLICAZIONE)))
                                    esito = "Il documento non è stato pubblicato";

                    }
                }
                else
                    esito = "Documento da pubblicare";
            }
            return esito;
        }
    }
}
