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
using System.Collections.Generic;
using DocsPAWA.UserControls;
using System.Drawing;
using log4net;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;
using DocsPAWA.NotificationCenterReference;


namespace DocsPAWA.TodoList
{
    public partial class toDoList : DocsPAWA.CssPage
    {
        private ILog logger = LogManager.GetLogger(typeof(toDoList));
        protected DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

        /// <summary>
        /// Indica la tipologia di ricerca trasmissioni
        /// </summary>
        private string _docOrFasc = String.Empty;

        // gestione todolist
        protected bool _isActivedTDLNotice = false;
        protected string _noticeDays = string.Empty;
        protected string _trasmOverNoticeDays = string.Empty;
        protected string _datePost = string.Empty;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[] filter;
        protected System.Web.UI.WebControls.Label lbl_Vis;
        protected System.Web.UI.WebControls.ImageButton IMG_VIS;

        protected DocsPAWA.DocsPaWR.Ruolo userRuolo;
        protected DocsPAWA.DocsPaWR.Utente userHome;

        protected DocsPAWA.DocsPaWR.Trasmissione[] listaTrasmRic;
        //private Amministrazione.UserControl.ScrollKeeper skDgTemplate;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if ((ConfigSettings.getKey("T_DO_SMISTA") != null
                && ConfigSettings.getKey("T_DO_SMISTA") != "0"))
            {
                if (!UserManager.ruoloIsAutorized(this, "DO_SMISTA"))
                    this.btn_smista.Visible = false;
            }
            else this.btn_smista.Visible = false;

            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalRecordCount"></param>
        protected void SetTotalRecordCount(int totalRecordCount, int trasmNonViste)
        {
            titolo.Text = "Elenco trasmissioni - Trovati " + totalRecordCount.ToString() + " elementi";
            if (trasmNonViste > 0)
            {

                btn_NonLetti.Visible = true;
                titolo.Text += " -";
                btn_NonLetti.Text = trasmNonViste.ToString() + " non letti";
            }
            else
                btn_NonLetti.Visible = false;
        }

        protected void btn_NonLetti_Click(object sender, EventArgs e)
        {
            //Imposta il filtro inserendo la ricerca dei soli documenti non letti
            //array contenitore degli array filtro di ricerca
            DocsPaWR.FiltroRicerca[][] qV;
            DocsPaWR.FiltroRicerca fV1;
            DocsPaWR.FiltroRicerca[] fVList;

            qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
            qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];

            if (ricercaTrasm.DialogFiltriRicercaTrasmissioni.CurrentFilters != null)
                fVList = ricercaTrasm.DialogFiltriRicercaTrasmissioni.CurrentFilters;
            else
                fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];

            #region filtro "TO DO LIST"
            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TODO_LIST.ToString();
            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            #endregion

            #region filtro "NO SOTTOPOSTI"
            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.NO_CERCA_INFERIORI.ToString();
            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            #endregion

            #region filtro "ELEMENTI NON LETTI"
            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.ELEMENTI_NON_VISTI.ToString();
            fV1.valore = "1";
            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            #endregion
            qV[0] = fVList;

            DocumentManager.setFiltroRicTrasm(this, qV[0]);

            this.FetchToDoList(this.GetCurrentContextPage(), grdDoc);
            btn_NonLetti.Visible = false;
            Session.Add("TrasmNonViste", "T");
            Response.Write("<SCRIPT>top.principale.iFrame_sx.document.location='../sceltaRuoloNew.aspx';</SCRIPT>");
        }

        /// <summary>
        /// Lista degli oggetti InfoToDoList relativi alla pagina correntemente visualizzata
        /// </summary>
        protected DocsPaWR.infoToDoList[] CurrentPageList
        {
            get
            {
                if (this.ViewState["CurrentPageList"] != null)
                    return (DocsPaWR.infoToDoList[])this.ViewState["CurrentPageList"];
                else
                    return new DocsPaWR.infoToDoList[0];
            }
            set
            {
                this.ViewState["CurrentPageList"] = value;
            }
        }

        /// <summary>
        /// Caricamento dati trasmissioni per la pagina corrente
        /// </summary>
        /// <param name="requestedPage"></param>
        protected DocsPaWR.infoToDoList[] GetTodolistData(int requestedPage, int pageSize, out int recordCount, out int trasmNonViste)
        {
            return TrasmManager.getMyNewTodolist(getListaRegistri(UserManager.getRuolo().registri),
                        this.filter,
                        requestedPage, this.grdDoc.PageSize, out recordCount, out trasmNonViste);
        }

        /// <summary>
        /// Associazione dati e selezione elemento griglia
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="data"></param>
        protected void BindGridAndSelect(DataGrid grid, int pageIndex, int recordCount)
        {
            DocsPaWR.infoToDoList[] data = this.CurrentPageList;
            grid.SelectedIndex = -1;
            grid.VirtualItemCount = recordCount;
            grid.CurrentPageIndex = pageIndex;
            grid.DataSource = data;
            grid.DataBind();

            // Tentativo di ripristino della trasmissione selezionata (se è presente un contesto precedente)
            string prevIdTrasm = this.GetCurrentContextIdTrasm();

            int selectedIndex = -1;

            foreach (DataGridItem item in grid.Items)
            {
                selectedIndex++;

                if (prevIdTrasm.Equals(data[selectedIndex].sysIdTrasm))
                {
                    grid.SelectedIndex = selectedIndex;
                    break;
                }

            }
        }

        /// <summary>
        /// Caricamento dati todolist
        /// </summary>
        /// <param name="requestedPage"></param>
        protected void FetchToDoList(int requestedPage, DataGrid dataGrid)
        {
            //recupero filtri di ricerca trasm se presenti
            this.filter = DocumentManager.getFiltroRicTrasm(this);

            int recordCount;
            int trasmNonViste;
            this.CurrentPageList = this.GetTodolistData(requestedPage, this.grdDoc.PageSize, out recordCount, out trasmNonViste);

            //associo la segnatura di repertorio ai documenti della tdl
            foreach (DocsPaWR.infoToDoList infoTdl in this.CurrentPageList)
            {
                if(!string.IsNullOrEmpty(infoTdl.sysIdDoc))
                    infoTdl.videoSegnRepertorio = DocumentManager.getSegnaturaRepertorio(this.Page, infoTdl.sysIdDoc, UserManager.getInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione).Codice);
            }
            if (CurrentPageList.Length == 0 && recordCount > 0)
            {
                // Se per la pagina richiesta non sono più disponibili elementi, 
                // viene ricercata l'ultima pagina disponibile
                int lastPage = (recordCount / dataGrid.PageSize);
                if ((recordCount % dataGrid.PageSize) > 0)
                    lastPage++;
                requestedPage = lastPage;

                this.CurrentPageList = this.GetTodolistData(requestedPage, this.grdDoc.PageSize, out recordCount, out trasmNonViste);
            }

            this.BindGridAndSelect(dataGrid, (requestedPage - 1), recordCount);

            dataGrid.Visible = (dataGrid.Items.Count > 0);

            if (dataGrid.Visible)
            {
                int numFasc = this.ColorElementRow();
                this.btn_smista.Visible = true;
                if (numFasc == recordCount)
                    this.btn_smista.Visible = false;
                //se in TDL sono presenti solo documenti annullati e fascicoli, nascondo il pulsante 'Smista'
                else
                {
                    bool isOnlyDocDeleted = true;
                    foreach (DocsPaWR.infoToDoList infoElem in this.CurrentPageList)
                    {
                        if (!string.IsNullOrEmpty(infoElem.sysIdDoc) && (!ws.IsDocAnnullatoByIdProfile(infoElem.sysIdDoc)))
                        {
                            isOnlyDocDeleted = false;
                            break;
                        }
                    }
                    if (isOnlyDocDeleted)
                        btn_smista.Visible = false;
                }

                //this.btn_smista.Visible = (this._docOrFasc == "D");
                this.btn_stampa.Visible = true; // (this._docOrFasc == "D"); 
                this.btn_rimuoviTDL.Visible = true;
                this.btn_rimuoviTDL.Attributes.Add("onclick", "svuotaTDLPage('','','" + this._docOrFasc + "','N','');");

                // gestione dello svuotamento della TDL
                this.isNoticeActived();
                if (this._isActivedTDLNotice && 
                    (!this._trasmOverNoticeDays.Equals(string.Empty) && !this._trasmOverNoticeDays.Equals("0"))
                    && Session["APRIDELEGHE"] == null)
                    ClientScript.RegisterStartupScript(this.GetType(), "apreSvuotaTDL", "<script>svuotaTDLPage('" + this._noticeDays + "','" + this._trasmOverNoticeDays + "','" + this._docOrFasc + "','Y','" + this._datePost + "');</script>");
                if (Session["APRIDELEGHE"] != null)
                    Session.Remove("APRIDELEGHE");

                //La colonna del datagrid per visualizzare la segnatura del repertorio è abilitata solo se è attiva la chiave di db associata
                grdDoc.Columns[7].Visible = this.ShowSegnaturaRepertori();

                // Le colonne con la descrizione della tipologia legata al documento / fascicolo è visibile solo se è attiva la visualizzazione
                // della segnatura in to do list
                grdDoc.Columns[5].Visible = this.ShowSegnaturaRepertori();

                // La colonna della scadenza è visibile solo se non è attiva la visualizzazione della segnatura in to do list
                grdDoc.Columns[4].Visible = !this.ShowSegnaturaRepertori();
            }
            else
            {
                this.btn_smista.Visible = false;
                this.btn_stampa.Visible = false;
                this.btn_rimuoviTDL.Visible = false;
            }

            this.SetTotalRecordCount(recordCount, trasmNonViste);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.Page.MaintainScrollPositionOnPostBack = true;

            Utils.startUp(this);
            Page.Response.Expires = -1;

            userRuolo = UserManager.getRuolo(this);
            userHome = UserManager.getUtente(this);

            if (!this.IsPostBack)
            {
                try
                {
                    this.AttatchGridPagingWaitControl();
                    //sempre "R" in quanto la pagina recupera solo le trasm ricevute
                    Session["tiporic"] = "R";
                    int ps = TrasmManager.getGrdTodolistPageSize(this);
                    DataGrid dataGrid = null;
                    this.grdDoc.PageSize = ps;
                    dataGrid = this.grdDoc;
                    this.FetchToDoList(this.GetCurrentContextPage(), dataGrid);
                }
                catch (Exception ex)
                {
                    ErrorManager.redirect(this, ex);
                }
            }
            AdminTool.UserControl.ScrollKeeper skDgTemplate = new AdminTool.UserControl.ScrollKeeper();
            skDgTemplate.WebControl = "DivDGList";
            this.form1.Controls.Add(skDgTemplate);

        }



        /// <summary>
        /// Bug paginazione datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnDataGridItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdDoc_PageIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {
            int newPage = e.NewPageIndex + 1;
            this.FetchToDoList(newPage, (DataGrid)sender);

            this.SetCurrentContext(newPage, string.Empty);
            ((DataGrid)sender).SelectedIndex = -1;
        }

        protected void grdDoc_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            bool CtrlRimossooAcl = false;
            try
            {
                if (e.CommandName != "Page")
                {
                    string errorMessage = "";
                    int VerificaPermessi = -1;
                    bool EsitoVerifica = false;

                    DocsPAWA.DocsPaWR.infoToDoList infoDoc = (DocsPAWA.DocsPaWR.infoToDoList)this.CurrentPageList[e.Item.ItemIndex];
                    listaTrasmRic = TrasmManager.trasmGetDettaglioTrasmissione(userHome, userRuolo, this.CurrentPageList[e.Item.ItemIndex]);
                    string docOrFasc = "";
                    if (listaTrasmRic[0] != null)
                    {
                        if (listaTrasmRic[0].tipoOggetto == DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
                            docOrFasc = "D";
                        else
                            docOrFasc = "F";

                        //if (listaTrasmRic[0].tipoOggetto.Equals(DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO))
                        //    docOrFasc = "D";
                        //else
                        //    docOrFasc = "F";
                    }
                    if (e.CommandName == "eliminaDaTodoList"
                        || e.CommandName == "dettaglio"
                        || e.CommandName == "scheda"
                        || e.CommandName == "Rimuovi")
                    {

                        if (docOrFasc == "D")
                            VerificaPermessi = DocumentManager.verificaACL("D", infoDoc.sysIdDoc, UserManager.getInfoUtente(), out errorMessage);
                        if (docOrFasc == "F")
                            VerificaPermessi = DocumentManager.verificaACL("F", infoDoc.sysIdFasc, UserManager.getInfoUtente(), out errorMessage);
                        if (VerificaPermessi != 2)
                        {
                            Response.Write("<script language=javascript>alert('" + errorMessage + "');</script>");
                            if (TrasmManager.rimuoviToDoListACL(listaTrasmRic[0].trasmissioniSingole[0].trasmissioneUtente[0].systemId, listaTrasmRic[0].trasmissioniSingole[0].systemId, UserManager.getInfoUtente(this).idPeople))
                                this.FetchToDoList(this.GetCurrentContextPage(), this.grdDoc);

                        }
                    }

                    if (VerificaPermessi > 1)
                    {
                        #region Comando rimuovi                       
                        if (e.CommandName == "eliminaDaTodoList")
                        {
                            if (listaTrasmRic[0].infoDocumento != null)
                            {
                                DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDettaglioDocumentoNoDataVista(this, listaTrasmRic[0].infoDocumento.idProfile, listaTrasmRic[0].infoDocumento.docNumber);
                                DocumentManager.setDocumentoSelezionato(schedaDoc);
                                if (!Page.IsStartupScriptRegistered("ApriFinestraRimuovi"))
                                {
                                    Page.RegisterStartupScript("ApriFinestraRimuoviProfilo", "<script>ApriFinestraRimuoviProfilo('toDoList');</script>");
                                }
                            }
                        }
                        #endregion

                        #region Comando rimuovi
                        if (e.CommandName == "Rimuovi")
                        {
                            if (listaTrasmRic[0].infoDocumento != null)
                            {
                                DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDettaglioDocumentoNoDataVista(this, listaTrasmRic[0].infoDocumento.idProfile, listaTrasmRic[0].infoDocumento.docNumber);
                                DocumentManager.setDocumentoSelezionato(schedaDoc);
                                if (!Page.ClientScript.IsStartupScriptRegistered("ApriFinestraRimuovi"))
                                {
                                    ClientScript.RegisterStartupScript(this.GetType(), "rimuovi", "ApriFinestraRimuoviProfilo('toDoList');", true);
                                    /* Page.RegisterStartupScript("ApriFinestraRimuoviProfilo", "<script>ApriFinestraRimuoviProfilo('toDoList');</script>");*/
                                }
                            }
                        }
                        #endregion

                        #region Comando DettaglioTrasmissione
                        if (e.CommandName == "dettaglio")
                        {
                            if (docOrFasc == "D")
                            {
                                if (listaTrasmRic[0].infoDocumento != null)
                                {
                                    TrasmManager.setDocTrasmSel(this, listaTrasmRic[0]);

                                    ClientScript.RegisterStartupScript(this.GetType(), "dettaglioTrasm", "<script>dettaglioTrasm('../documento/tabTrasmissioniRic.aspx?nomeForm=todolist');</script>");
                                }
                                else
                                {
                                    Response.Write("<script language=javascript>alert('Con il ruolo corrente non  possibile visualizzare\\nla scheda selezionata.');</script>");
                                }
                            }
                            if (docOrFasc == "F")
                            {
                                TrasmManager.setDocTrasmSel(this, listaTrasmRic[0]);
                                string provenienza = Request.QueryString["home"];
                                if (provenienza.Equals("Y"))
                                {
                                    Session.Add("ProvenienzaHome", "home");
                                }
                                else if (provenienza.Equals("N"))
                                {
                                    Session.Add("ProvenienzaHome", "ricTrasm");
                                }
                                // ClientScript.RegisterStartupScript(this.GetType(), "dettaglioTrasm", "<script>dettaglioTrasm('../documento/tabTrasmissioniRic.aspx');</script>");
                                ClientScript.RegisterStartupScript(this.GetType(), "dettaglioTrasm", "<script>dettaglioTrasm('../fascicolo/tabTrasmissioniRicFasc.aspx?nomeForm=todolist');</script>");
                            }
                        }

                        #endregion

                        #region scheda documento/fascicolo
                        if (e.CommandName == "scheda")
                        {
                            if ((listaTrasmRic != null) && (listaTrasmRic.Length > 0)) TrasmManager.setDocTrasmSel(this, listaTrasmRic[0]);

                            if (listaTrasmRic[0] != null)
                            {
                                if (docOrFasc == "F")
                                {
                                    #region Comando Apri Scheda Fascicolo
                                    if (listaTrasmRic[0].infoFascicolo.GetType().Equals(typeof(DocsPAWA.DocsPaWR.InfoFascicolo)))
                                    {
                                        // Aggiornamento contesto
                                        this.SetCurrentContext(((DataGrid)sender).CurrentPageIndex + 1, listaTrasmRic[0].systemId);

                                        //Inserisco in sessione l'oggetto per lo scorrimento continuo della lista
                                        //UserControls.ScrollElementsList.ScrollManager.setInSessionNewObjScrollElementsList(grdFasc.VirtualItemCount, grdFasc.PageCount, grdFasc.PageSize, e.Item.ItemIndex, grdFasc.CurrentPageIndex, new ArrayList(CurrentPageList), UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC_TO_DO_LIST);
                                        UserControls.ScrollElementsList.ScrollManager.setInContextNewObjScrollElementsList(grdDoc.VirtualItemCount, grdDoc.PageCount, grdDoc.PageSize, e.Item.ItemIndex, grdDoc.CurrentPageIndex, new ArrayList(CurrentPageList), UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC_TO_DO_LIST);

                                        DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);

                                        DocsPaWR.Fascicolo fascicolo = FascicoliManager.getFascicolo(this, infoUtente.idGruppo, infoUtente.idPeople, this.listaTrasmRic[0].infoFascicolo);
                                        if (fascicolo != null)
                                        {
                                            FascicoliManager.setFascicoloSelezionato(this, fascicolo);
                                            FascicoliManager.setInfoFascicolo(this, this.listaTrasmRic[0].infoFascicolo);
                                            Session.Remove("Tipo_obj");
                                            Session.Add("fascicolo", "TODOLIST");
                                            Response.Write("<script language='javascript'>top.principale.location='" + this.GetUrl() + "';</script>");
                                        }
                                        else
                                        {
                                            Response.Write("<script language=javascript>alert('Con il ruolo corrente non è possibile visualizzare\\nil fascicolo selezionato.');</script>");
                                        }
                                    }
                                    #endregion

                                }
                                if (docOrFasc == "D")
                                {
                                    #region Apri Scheda Documento
                                    if (listaTrasmRic[0].infoDocumento != null)
                                    {
                                        if ((listaTrasmRic[0].infoDocumento != null && listaTrasmRic[0].infoDocumento.numProt != null && listaTrasmRic[0].infoDocumento.numProt != "") //PROTOCOLLATI SU REGISTRI NON AUTORIZZATI
                                            || (listaTrasmRic[0].infoDocumento != null &&
                                                !(listaTrasmRic[0].infoDocumento.numProt != null
                                            && listaTrasmRic[0].infoDocumento.numProt != "")
                                            && (listaTrasmRic[0].infoDocumento.idRegistro != null
                                                && listaTrasmRic[0].infoDocumento.idRegistro != "" &&
                                                listaTrasmRic[0].infoDocumento.daProtocollare == "1"))) //PREDISPOSTI SU REGISTRI NON AUTORIZZATI
                                        {
                                            //verifica se il ruolo selezionato ha la visibilità sul registro del documento 
                                            EsitoVerifica = VerificaAutorizzazioneSuRegistro(listaTrasmRic[0].infoDocumento.idRegistro);
                                        }
                                        else
                                        {
                                            //se il documento è grigio non occorre effettuare verifiche sui registri.
                                            EsitoVerifica = true;
                                        }
                                        if (EsitoVerifica)
                                        {
                                            DocumentManager.setRisultatoRicerca(this, listaTrasmRic[0].infoDocumento);

                                            // Impostazione contesto chiamante
                                            Session.Remove("Tipo_obj");
                                            Session.Remove("dictionaryCorrispondente");

                                            // Aggiornamento contesto
                                            this.SetCurrentContext(((DataGrid)sender).CurrentPageIndex + 1, listaTrasmRic[0].systemId);

                                            //Inserisco in sessione l'oggetto per lo scorrimento continuo della lista
                                            //UserControls.ScrollElementsList.ScrollManager.setInSessionNewObjScrollElementsList(grdDoc.VirtualItemCount, grdDoc.PageCount, grdDoc.PageSize, e.Item.ItemIndex, grdDoc.CurrentPageIndex, new ArrayList(CurrentPageList), UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC_TO_DO_LIST);
                                            UserControls.ScrollElementsList.ScrollManager.setInContextNewObjScrollElementsList(grdDoc.VirtualItemCount, grdDoc.PageCount, grdDoc.PageSize, e.Item.ItemIndex, grdDoc.CurrentPageIndex, new ArrayList(CurrentPageList), UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC_TO_DO_LIST);

                                            Response.Write("<script language='javascript'>top.principale.location='" + this.GetUrl() + "';</script>");
                                        }
                                        else
                                        {
                                            Response.Write("<script language=javascript>alert('Con il ruolo corrente non  possibile visualizzare\\nla scheda selezionata.');</script>");
                                        }
                                    }
                                    else
                                    {
                                        Response.Write("<script language=javascript>alert('Con il ruolo corrente non  possibile visualizzare\\nla scheda selezionata.');</script>");
                                    }
                                    #endregion chiudi scheda documento
                                }

                            }
                        }
                        #endregion
                    }

                    //gestione visualizzatore unificato
                    if (e.CommandName == "VisDoc")
                    {
                        //vis unificata
                        DocsPAWA.DocsPaWR.SchedaDocumento schedaSel = DocumentManager.getDettaglioDocumento(this, listaTrasmRic[0].infoDocumento.idProfile, listaTrasmRic[0].infoDocumento.docNumber);
                        DocumentManager.setDocumentoSelezionato(this, schedaSel);
                        FileManager.setSelectedFile(this, schedaSel.documenti[0], false);
                        ClientScript.RegisterStartupScript(this.GetType(), "lanciaVIs", "loadvisualizzaDoc('" + Session.SessionID + "','" + schedaSel.docNumber + "','" + schedaSel.systemId + "');", true);
                    }
                }
                else
                    this.AttatchGridPagingWaitControl();
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_smista_Click(object sender, ImageClickEventArgs e)
        {
            DocsPAWA.smistaDoc.SmistaDocSessionManager.ReleaseSmistaDocManager();
            if (!this.ClientScript.IsStartupScriptRegistered("apriModalDialog"))
            {
                string scriptString = "<SCRIPT>ApriSmistamento()</SCRIPT>";
                this.ClientScript.RegisterStartupScript(this.GetType(), "apriModalDialog", scriptString);

            }
        }

        #region utils

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetUrl()
        {
            string url = " ";
            if (listaTrasmRic[0] != null)
            {
                if ((listaTrasmRic[0].infoFascicolo != null) && (!string.IsNullOrEmpty(listaTrasmRic[0].infoFascicolo.idFascicolo)))
                {
                    url = "../fascicolo/gestioneFasc.aspx?tab=documenti";
                }
                else
                    if (!string.IsNullOrEmpty(listaTrasmRic[0].infoDocumento.numProt))
                    {
                        url = "../documento/gestioneDoc.aspx?tab=protocollo";
                    }
                    else
                    {
                        url = "../documento/gestioneDoc.aspx?tab=profilo";
                    }
                return url;
            }
            else
            {
                return url;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        protected string getListaRegistri(DocsPAWA.DocsPaWR.Registro[] reg)
        {
            string registri = "0";

            if (reg.Length > 0)
            {
                foreach (DocsPAWA.DocsPaWR.Registro registro in reg)
                {
                    registri = registri + "," + registro.systemId;
                }
                return registri;
            }
            else return registri;
        }

        /// <summary>
        /// 
        /// </summary>
        private int ColorElementRow()
        {
            int numFasc = 0;
            this.grdDoc.Columns[8].ItemStyle.Font.Bold = true;
            for (int i = 0; i < this.grdDoc.Items.Count; i++)
            {
                //imposto colore e stile della colonna segnatura di repertorio
                this.grdDoc.Items[i].Cells[7].Font.Bold = true;
                this.grdDoc.Items[i].Cells[7].ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);

                ImageButton imgDettaglio = (ImageButton)this.grdDoc.Items[i].Cells[8].FindControl("imgDettaglio");
                this.grdDoc.Items[i].Cells[8].ForeColor = System.Drawing.Color.FromArgb(130, 130, 130);
                if (!imgDettaglio.AlternateText.StartsWith("Id") && !imgDettaglio.AlternateText.StartsWith("Fasc"))
                {
                    //è un protocollo
                    this.grdDoc.Items[i].Cells[8].ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    
                    //verfica se annullato
                    string idDocVerify = this.grdDoc.Items[i].Cells[15].Text;
                    //idDocVerify = ((HiddenField)this.grdDoc.Items[i].Parent.FindControl("hfDocId")).Value;
                    if (ws.IsDocAnnullatoByIdProfile(idDocVerify))
                    {
                        this.grdDoc.Items[i].Cells[0].CssClass = "proto_annullato";
                        this.grdDoc.Items[i].Cells[1].CssClass = "proto_annullato";
                        this.grdDoc.Items[i].Cells[2].CssClass = "proto_annullato";
                        if (!this.grdDoc.Items[i].Cells[3].Text.Equals("&nbsp;"))
                        {
                            this.grdDoc.Items[i].Cells[3].CssClass = "proto_annullato";
                        }
                        if (!this.grdDoc.Items[i].Cells[4].Text.Equals("&nbsp;"))
                        {
                            this.grdDoc.Items[i].Cells[4].CssClass = "proto_annullato";
                        }
                        this.grdDoc.Items[i].Cells[6].CssClass = "proto_annullato";
                        this.grdDoc.Items[i].Cells[8].CssClass = "proto_annullato";
                        this.grdDoc.Items[i].Cells[9].CssClass = "proto_annullato";

                    }
                    else
                    {
                        ((LinkButton)this.grdDoc.Items[i].Cells[0].Controls[1]).ForeColor = System.Drawing.Color.Black;
                    }
                }
                else
                {
                    ((LinkButton)this.grdDoc.Items[i].Cells[0].Controls[1]).ForeColor = System.Drawing.Color.Black;
                    if (imgDettaglio.AlternateText.StartsWith("Fasc"))
                        numFasc++;
                    //    //è un doc grigio
                    //    this.grdDoc.Items[i].Cells[6].ForeColor = System.Drawing.Color.FromArgb(130, 130, 130);
                }

                //Documento o fascicolo non letto viene evidenziato in grassetto
                string visto = this.grdDoc.Items[i].Cells[12].Text;
                if (visto == "0")
                {
                    this.grdDoc.Items[i].Font.Bold = true;
                }


            }
            return numFasc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool isNoticeActived()
        {
            TodoList.TodoListManager manager = new DocsPAWA.TodoList.TodoListManager(this.userRuolo, this.userHome, this._docOrFasc, false);
            this._isActivedTDLNotice = manager.isNoticeActived(out this._noticeDays, out this._trasmOverNoticeDays, out this._datePost);
            return this._isActivedTDLNotice;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        private bool VerificaAutorizzazioneSuRegistro(string idRegistro)
        {
            bool result = false;
            try
            {
                if (UserManager.isFiltroAooEnabled(this))
                    result = true;
                else
                {
                    if (idRegistro != null && idRegistro != "")
                    {
                        DocsPaWR.Registro[] RegRuoloSel = ((DocsPAWA.DocsPaWR.Ruolo)UserManager.getRuolo(this)).registri;
                        foreach (DocsPAWA.DocsPaWR.Registro reg in RegRuoloSel)
                        {
                            if (idRegistro == reg.systemId)
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
            return result;
        }
        #endregion

        #region Wait Control
        private void AttatchGridPagingWaitControl()
        {
            DataGridPagingWaitControl.DataGridID = this.grdDoc.ClientID;

            DataGridPagingWaitControl.WaitScriptCallback = "WaitDataGridCallback(eventTarget,eventArgument);";
        }

        private waiting.DataGridPagingWait DataGridPagingWaitControl
        {
            get
            {
                return this.FindControl("DataGridPagingWait1") as waiting.DataGridPagingWait;
            }
        }
        #endregion

        #region Gestione CallContext

        /// <summary>
        /// Reperimento numero pagina selezionata nel contesto corrente
        /// </summary>
        /// <returns></returns>
        protected int GetCurrentContextPage()
        {
            int currentPage = 1;

            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext != null &&
                currentContext.ContextName == SiteNavigation.NavigationKeys.PAGINA_INIZIALE)
            {
                // Ripristino pagina precedentemente visualizzata
                currentPage = currentContext.PageNumber;
            }

            return currentPage;
        }

        /// <summary>
        /// Reperimento id trasmissione selezionata nel contesto corrente
        /// </summary>
        /// <returns></returns>
        protected string GetCurrentContextIdTrasm()
        {
            string idTrasm = string.Empty;

            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext != null &&
                currentContext.ContextName == SiteNavigation.NavigationKeys.PAGINA_INIZIALE &&
                currentContext.IsBack &&
                currentContext.QueryStringParameters.ContainsKey("idTrasm"))
            {
                // Ripristino trasmissione precedentemente selezionata
                idTrasm = currentContext.QueryStringParameters["idTrasm"].ToString();
            }

            return idTrasm;
        }

        /// <summary>
        /// Aggiornamento contesto corrente
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="idTrasmissione"></param>
        private void SetCurrentContext(int currentPage, string idTrasmissione)
        {
            try
            {
                //Se ho cliccato su homepage, viene forzato il contesto di pagina_iniziali, perchè altrimenti 
                //dopo il clear l'oggetto CurrentContext è null, ma solo se arrivo da HomePAge succede.
                if (SiteNavigation.CallContextStack.CurrentContext == null)
                {
                    string url = DocsPAWA.Utils.getHttpFullPath() + "/GestioneRuolo.aspx";
                    SiteNavigation.CallContext context = new DocsPAWA.SiteNavigation.CallContext(SiteNavigation.NavigationKeys.PAGINA_INIZIALE, url);
                    context.ContextFrameName = "top.principale";
                    SiteNavigation.CallContextStack.SetCurrentContext(context, true);
                }

                SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

                if (currentContext.ContextName == SiteNavigation.NavigationKeys.PAGINA_INIZIALE)
                {
                    currentContext.PageNumber = currentPage;

                    if (idTrasmissione != string.Empty)
                        currentContext.QueryStringParameters["idTrasm"] = idTrasmissione;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel SetCurrentContext della TODOLIST: " + ex.Message);
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_stampa_Click(object sender, ImageClickEventArgs e)
        {
            if (!this.ClientScript.IsStartupScriptRegistered("apriStampaRisultatoRicerca"))
            {
                string scriptString = "<SCRIPT>StampaRisultatoRicerca('" + _docOrFasc + "')</SCRIPT>";
                this.ClientScript.RegisterStartupScript(this.GetType(), "apriStampaRisultatoRicerca", scriptString);

            }
        }

        protected void grdDoc_PreRender(object sender, EventArgs e)
        {
            bool rimuoviDocToDoList = false;
            rimuoviDocToDoList = UserManager.ruoloIsAutorized(this, "DO_TODOLIST_RIMUOVI");
            int lung = 0;
            string docOrFasc = "";
            ImageButton imgDettaglio;
            if (System.Configuration.ConfigurationManager.AppSettings["TRUNCATESTRING_MITDEST"] != null)
            {
                lung = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["TRUNCATESTRING_MITDEST"]);
            }
            foreach (DataGridItem item in grdDoc.Items)
            {
                imgDettaglio = (ImageButton)item.Cells[8].FindControl("imgDettaglio");
                if (imgDettaglio.AlternateText.StartsWith("Fasc"))
                {
                    docOrFasc = "F";
                }
                else
                    docOrFasc = "D";

                //La colonna del datagrid per rimuovere un documento è visibile solo 
                //se l'utente è abilitato alla funzione di rimozione. 
                if (rimuoviDocToDoList)
                {
                    grdDoc.Columns[10].Visible = true;
                    if (!docOrFasc.Equals("D"))
                    {
                        ((ImageButton)item.Cells[10].Controls[1]).Visible = false;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.Cells[2].Text) && (item.Cells[2].Text.ToUpper()).Equals("INTEROPERABILITA") && imgDettaglio.AlternateText.StartsWith("Id"))
                        {
                            ((ImageButton)item.Cells[10].Controls[1]).Visible = true;
                        }
                        else
                        {
                            ((ImageButton)item.Cells[10].Controls[1]).Visible = false;
                        }
                    }
                }
                else
                {
                    grdDoc.Columns[10].Visible = false;
                }

                //if (docOrFasc == "F")
                //{
                //    ((ImageButton)item.Cells[8].Controls[1]).ImageUrl = "../images/cartellaA.jpg";
                //    ((ImageButton)item.Cells[8].Controls[1]).BorderWidth = 0;
                //}

                //gestione Vis Unificata
                //if ((System.Configuration.ConfigurationManager.AppSettings["GRD_VIS_UNIFICATA"] != null) && (System.Configuration.ConfigurationManager.AppSettings["GRD_VIS_UNIFICATA"] == "0"))
                //{
                //    this.grdDoc.Columns[9].Visible = false;
                //}
                //else
                //{
                //    string firmato = item.Cells[13].Text;
                //    // gestione della icona dei dettagli -- tolgo icona se non c'è doc acquisito

                //    string acquisitaImmagine = ((TableCell)item.Cells[11]).Text;
                //    ImageButton imgbtn = new ImageButton();

                //    if ((acquisitaImmagine != null && acquisitaImmagine.Equals("0")) || docOrFasc == "F")
                //    {
                //        if (item.Cells[9].Controls[1].GetType().Equals(typeof(ImageButton)))
                //        {
                //            imgbtn = (ImageButton)item.Cells[9].Controls[1];
                //            imgbtn.Visible = false;
                //        }
                //    }
                //    else
                //    {   
                //        if (acquisitaImmagine != null)
                //        {
                //            string pathImage = FileManager.getFileIcon(Page, acquisitaImmagine.Trim().ToLower());
                //            if (item.Cells[9].Controls[1].GetType().Equals(typeof(ImageButton)))
                //            {
                //                imgbtn = (ImageButton)item.Cells[9].Controls[1];
                //                imgbtn.Visible = true;
                //                imgbtn.ImageUrl = pathImage;
                //            }
                //        }
                //    }
                //}
                if (lung > 0)
                {

                    string testo = item.Cells[9].Text;
                    if (testo.Contains("<br> ------- <br>"))
                    {
                        int lungOggetto = testo.IndexOf("<br> ------- <br>");
                        string oggetto = "";
                        string mittDest = "";
                        oggetto = testo.Substring(0, lungOggetto);
                        oggetto = DocsPAWA.Utils.TruncateString(oggetto);
                        int iniziomit = testo.LastIndexOf("<br>");
                        mittDest = testo.Substring(iniziomit + 4);
                        if (mittDest != "")
                            mittDest = DocsPAWA.Utils.TruncateString_MittDest(mittDest);
                        testo = oggetto + "<br> ------- <br>" + mittDest;
                    }
                    else
                        testo = DocsPAWA.Utils.TruncateString(testo);
                    item.Cells[9].Text = testo;
                }

                if (item.ItemIndex == grdDoc.SelectedIndex)
                {
                    string idDocVerify = item.Cells[8].Text;
                    int lungDoc = idDocVerify.IndexOf("<br>");
                    if (lungDoc != -1)
                    {
                        idDocVerify = (item.Cells[8].Text).Substring(0, lungDoc);
                    }
                    if (ws.IsDocAnnullatoByIdProfile(idDocVerify))
                    {
                        item.Cells[0].CssClass = "proto_annullato";
                    }
                    else
                    {
                        ((LinkButton)item.Cells[0].Controls[1]).ForeColor = Color.White;
                    }
                }

                // Se il testo dell'oggetto contiene Non si possiedono i diritti...
                if (item.Cells[9].Text.Contains("Non si possiedono i diritti"))
                    // ...il bottone di visualizzazione del documento deve essere nascosto
                    ((IconeRicerca)item.FindControl("UserControlRic")).HaveVisibilityRights = false;

            }
        }

        protected void grdDoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool CtrlRimossooAcl = false;
            try
            {
                    string errorMessage = "";
                    int VerificaPermessi = -1;
                    bool EsitoVerifica = false;

                    DocsPAWA.DocsPaWR.infoToDoList infoDoc = (DocsPAWA.DocsPaWR.infoToDoList)this.CurrentPageList[this.grdDoc.SelectedIndex];
                    listaTrasmRic = TrasmManager.trasmGetDettaglioTrasmissione(userHome, userRuolo, this.CurrentPageList[this.grdDoc.SelectedIndex]);
                    string docOrFasc = "";
                    if (listaTrasmRic[0] != null)
                    {
                        if (!string.IsNullOrEmpty(infoDoc.sysIdFasc))
                            docOrFasc = "F";
                        if (!string.IsNullOrEmpty(infoDoc.sysIdDoc))
                            docOrFasc = "D";
                    }

                        if (docOrFasc == "D")
                            VerificaPermessi = DocumentManager.verificaACL("D", infoDoc.sysIdDoc, UserManager.getInfoUtente(), out errorMessage);
                        if (docOrFasc == "F")
                            VerificaPermessi = DocumentManager.verificaACL("F", infoDoc.sysIdFasc, UserManager.getInfoUtente(), out errorMessage);
                        if (VerificaPermessi != 2)
                        {
                            Response.Write("<script language=javascript>alert('" + errorMessage + "');</script>");
                            if (TrasmManager.rimuoviToDoListACL(listaTrasmRic[0].trasmissioniSingole[0].trasmissioneUtente[0].systemId, listaTrasmRic[0].trasmissioniSingole[0].systemId, UserManager.getInfoUtente(this).idPeople))
                                this.FetchToDoList(this.GetCurrentContextPage(), this.grdDoc);

                        }

                    if (VerificaPermessi > 1)
                    {                     
                        #region scheda documento/fascicolo
                            if ((listaTrasmRic != null) && (listaTrasmRic.Length > 0)) TrasmManager.setDocTrasmSel(this, listaTrasmRic[0]);

                            if (listaTrasmRic[0] != null)
                            {
                                if (docOrFasc == "F")
                                {
                                    #region Comando Apri Scheda Fascicolo
                                    if (listaTrasmRic[0].infoFascicolo.GetType().Equals(typeof(DocsPAWA.DocsPaWR.InfoFascicolo)))
                                    {
                                        // Aggiornamento contesto
                                        this.SetCurrentContext(((DataGrid)sender).CurrentPageIndex + 1, listaTrasmRic[0].systemId);

                                        //Inserisco in sessione l'oggetto per lo scorrimento continuo della lista
                                        //UserControls.ScrollElementsList.ScrollManager.setInSessionNewObjScrollElementsList(grdFasc.VirtualItemCount, grdFasc.PageCount, grdFasc.PageSize, e.Item.ItemIndex, grdFasc.CurrentPageIndex, new ArrayList(CurrentPageList), UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC_TO_DO_LIST);
                                        UserControls.ScrollElementsList.ScrollManager.setInContextNewObjScrollElementsList(grdDoc.VirtualItemCount, grdDoc.PageCount, grdDoc.PageSize, grdDoc.SelectedIndex, grdDoc.CurrentPageIndex, new ArrayList(CurrentPageList), UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC_TO_DO_LIST);

                                        DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);

                                        DocsPaWR.Fascicolo fascicolo = FascicoliManager.getFascicolo(this, infoUtente.idGruppo, infoUtente.idPeople, this.listaTrasmRic[0].infoFascicolo);
                                        if (fascicolo != null)
                                        {
                                            FascicoliManager.setFascicoloSelezionato(this, fascicolo);
                                            FascicoliManager.setInfoFascicolo(this, this.listaTrasmRic[0].infoFascicolo);
                                            Session.Remove("Tipo_obj");
                                            Response.Write("<script language='javascript'>top.principale.location='" + this.GetUrl() + "';</script>");

                                        }
                                        else
                                        {
                                            Response.Write("<script language=javascript>alert('Con il ruolo corrente non è possibile visualizzare\\nil fascicolo selezionato.');</script>");
                                        }
                                    }
                                    #endregion

                                }
                                if (docOrFasc == "D")
                                {
                                    #region Apri Scheda Documento
                                    if (listaTrasmRic[0].infoDocumento != null)
                                    {
                                        if ((listaTrasmRic[0].infoDocumento != null && listaTrasmRic[0].infoDocumento.numProt != null && listaTrasmRic[0].infoDocumento.numProt != "") //PROTOCOLLATI SU REGISTRI NON AUTORIZZATI
                                            || (listaTrasmRic[0].infoDocumento != null &&
                                                !(listaTrasmRic[0].infoDocumento.numProt != null
                                            && listaTrasmRic[0].infoDocumento.numProt != "")
                                            && (listaTrasmRic[0].infoDocumento.idRegistro != null
                                                && listaTrasmRic[0].infoDocumento.idRegistro != "" &&
                                                listaTrasmRic[0].infoDocumento.daProtocollare == "1"))) //PREDISPOSTI SU REGISTRI NON AUTORIZZATI
                                        {
                                            //verifica se il ruolo selezionato ha la visibilità sul registro del documento 
                                            EsitoVerifica = VerificaAutorizzazioneSuRegistro(listaTrasmRic[0].infoDocumento.idRegistro);
                                        }
                                        else
                                        {
                                            //se il documento è grigio non occorre effettuare verifiche sui registri.
                                            EsitoVerifica = true;
                                        }
                                        if (EsitoVerifica)
                                        {
                                            DocumentManager.setRisultatoRicerca(this, listaTrasmRic[0].infoDocumento);

                                            // Impostazione contesto chiamante
                                            Session.Remove("Tipo_obj");

                                            // Aggiornamento contesto
                                            this.SetCurrentContext(((DataGrid)sender).CurrentPageIndex + 1, listaTrasmRic[0].systemId);

                                            //Inserisco in sessione l'oggetto per lo scorrimento continuo della lista
                                            //UserControls.ScrollElementsList.ScrollManager.setInSessionNewObjScrollElementsList(grdDoc.VirtualItemCount, grdDoc.PageCount, grdDoc.PageSize, e.Item.ItemIndex, grdDoc.CurrentPageIndex, new ArrayList(CurrentPageList), UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC_TO_DO_LIST);
                                            UserControls.ScrollElementsList.ScrollManager.setInContextNewObjScrollElementsList(grdDoc.VirtualItemCount, grdDoc.PageCount, grdDoc.PageSize, this.grdDoc.SelectedIndex, grdDoc.CurrentPageIndex, new ArrayList(CurrentPageList), UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC_TO_DO_LIST);

                                            Response.Write("<script language='javascript'>top.principale.location='" + this.GetUrl() + "';</script>");
                                        }
                                        else
                                        {
                                            Response.Write("<script language=javascript>alert('Con il ruolo corrente non  possibile visualizzare\\nla scheda selezionata.');</script>");
                                        }
                                    }
                                    else
                                    {
                                        Response.Write("<script language=javascript>alert('Con il ruolo corrente non  possibile visualizzare\\nla scheda selezionata.');</script>");
                                    }
                                    #endregion chiudi scheda documento
                                }

                            }
                        #endregion
                    }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        /// <summary>
        /// Metodo utilizzato per indicare se è attiva la visualizzazione della segnatura di repertorio in to do list
        /// </summary>
        /// <returns>True se è attiva</returns>
        protected bool ShowSegnaturaRepertori()
        {
            return utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "VIS_SEGNATURA_REPERTORI").Equals("1");
        }
    }
}
