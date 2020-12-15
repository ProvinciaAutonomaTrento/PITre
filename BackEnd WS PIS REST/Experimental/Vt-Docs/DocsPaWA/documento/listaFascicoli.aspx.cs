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
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.documento
{
    /// <summary>
    /// Summary description for listaFascicoliDiClassifica.
    /// </summary>
    public class listaFascicoliDiClassifica : DocsPAWA.CssPage
    {
        protected System.Web.UI.WebControls.DataGrid DataGrid1;

        #region sezione variabili globali
        private enum tipoVisualizzazioneFascicoli
        {
            tvfUndefined, tvfDaAreaDiLavoro, tvfProcedimentali, tvfGenerali, tvfAll
        }
        private const string QUERYSTRING_tipoVisualizzazione = "tipoVisualizzazione";
        private const string QUERYSTRING_codClassifica = "codClassifica";
        private const string QUERYSTRING_idTitolario = "idTitolario";
        private tipoVisualizzazioneFascicoli m_tipoVisualizzazioneFascicoli;
        private string m_codClassificaSelezionata;
        private string idTitolario;
        private string pagina;
        private DataTable m_dataTableFascicoli;
        private Hashtable m_hashTableFascicoli;
        private Hashtable HT_fascicoli;
        private Hashtable HT_fascicoliSelezionati;
        protected DocsPAWA.dataSet.DataSetRFasc dataSetRFasc1;
        protected System.Web.UI.WebControls.Label lbl_tipoLista;
        protected System.Web.UI.HtmlControls.HtmlTableCell TD1;
        protected System.Web.UI.WebControls.ImageButton btn_deleteADL;
        protected System.Web.UI.WebControls.ImageButton btn_insertAll;
        protected System.Web.UI.WebControls.CheckBox chkSelectDeselectAll;
        protected System.Web.UI.WebControls.Panel pnl_title;
        protected System.Web.UI.WebControls.Label LabelMsg;
        protected System.Web.UI.HtmlControls.HtmlTableCell TD2;
        protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_cn;
        protected System.Web.UI.HtmlControls.HtmlTableCell Td5;
        protected bool enableFiltriFasc;
        protected bool showChilds;
        protected string call_type;
        protected int currentPage = 1;

        //private DocsPAWA.DocsPaWR.Fascicolo m_fascicoloSelezionato;
        #endregion

        #region sezione template pagina
        private void getParametri()
        {
            //recupera i parametri dalla session o vengono
            //costruiti appositamente in base ai parametri
            //ricevuti.
            try
            {
                //recupero sempre i parametri dalla query string
                getParametriQueryString();

                if (!IsPostBack)
                {
                    //primo caricamento pagina:
                    //creo i dati che dovr‡ gestire la pagina
                    //e li memorizzo in session, in
                    //modo che diventino disponibili tra le sequenze
                    //di postbak
                    //getParametriDatiPaginePrecedenti();
                    buildParametriPagina();
                    setParametriPaginaInSession();
                }
                else
                {
                    //post back:
                    //recupero eventuali dati gi‡ creati per la 
                    //pagina e memorizzati nel view state e in session
                    getParametriPaginaViewState();
                    getParametriPaginaInSession();
                }
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void getParametriPaginaViewState()
        {
            try
            {

            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void getParametriPaginaInSession()
        {
            //recupero i parametri che devono essere
            //disponibili alla pagina tra le varie 
            //sequenze di postback
            try
            {
                m_dataTableFascicoli = FascicoliManager.getDatagridFascicolo(this);
                m_hashTableFascicoli = FascicoliManager.getHashFascicoli(this);
                HT_fascicoli = FascicoliManager.getHashFascicoliADL(this);
                HT_fascicoliSelezionati = FascicoliManager.getHashFascicoliSelezionati(this);
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void getParametriQueryString()
        {
            //recupera i parametri dalla query string
            try
            {
                if (Request.QueryString != null)
                {
                    idTitolario = this.Request.QueryString["idTitolario"];

                    if (this.Request.QueryString["enableFiltriFasc"] != null)
                        this.enableFiltriFasc = true;

                    string appoQsTipo = Request.QueryString[QUERYSTRING_tipoVisualizzazione];
                    if (appoQsTipo != null)
                    {
                        this.call_type = appoQsTipo;
                        if (appoQsTipo.Equals(tipoVisualizzazioneFascicoli.tvfDaAreaDiLavoro.ToString()))
                        {
                            m_tipoVisualizzazioneFascicoli = tipoVisualizzazioneFascicoli.tvfDaAreaDiLavoro;
                        }
                        else if (appoQsTipo.Equals(tipoVisualizzazioneFascicoli.tvfProcedimentali.ToString()))
                        {
                            m_tipoVisualizzazioneFascicoli = tipoVisualizzazioneFascicoli.tvfProcedimentali;
                        }
                        else if (appoQsTipo.Equals(tipoVisualizzazioneFascicoli.tvfUndefined.ToString()))
                        {
                            m_tipoVisualizzazioneFascicoli = tipoVisualizzazioneFascicoli.tvfUndefined;
                        }
                        else if (appoQsTipo.Equals(tipoVisualizzazioneFascicoli.tvfGenerali.ToString()))
                        {
                            m_tipoVisualizzazioneFascicoli = tipoVisualizzazioneFascicoli.tvfGenerali;
                        }
                        else if (appoQsTipo.Equals(tipoVisualizzazioneFascicoli.tvfAll.ToString()))
                        {
                            m_tipoVisualizzazioneFascicoli = tipoVisualizzazioneFascicoli.tvfAll;
                        }
                    }
                    else
                    {
                        m_tipoVisualizzazioneFascicoli = tipoVisualizzazioneFascicoli.tvfUndefined;
                    }

                    string appoQsCod = this.Server.UrlDecode(Request.QueryString[QUERYSTRING_codClassifica]);

                    if (appoQsCod != null && appoQsCod != "")
                    {
                        m_codClassificaSelezionata = appoQsCod.ToString();
                    }
                    else
                    {
                        m_codClassificaSelezionata = null;
                    }

                    if (this.Request.QueryString["showChilds"] != null)
                    {
                        if (this.Request.QueryString["showChilds"] == "S")
                            showChilds = true;
                        else
                            showChilds = false;
                    }
                }
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void getParametriDatiPaginePrecedenti()
        {
            //recupero parametri dalla sessione impostati
            //da altre pagine
            try
            {

            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        //		private void caricamentoControlliPagina()
        //		{
        //			//eseguo l'inizializzazione dei controlli
        //			//sulla pagina
        //			try
        //			{
        //
        //			}
        //			catch(System.Exception es) 
        //			{
        //				ErrorManager.redirect(this, es);
        //			} 
        //		}

        private void gestioneParametri()
        {
            //gestisce i parametri per il rendering della 
            //pagina.
            try
            {
                //controllo inserito quando la pagina viene richiamata dalla finestra del nuovo fascicolo. in tal caso non viene visualizzata la griglia
                if (m_tipoVisualizzazioneFascicoli.Equals(tipoVisualizzazioneFascicoli.tvfUndefined) && (m_codClassificaSelezionata == null || m_codClassificaSelezionata.Equals("")))
                {
                    this.pnl_title.Visible = false;
                    return;
                }

                if (!IsPostBack)
                {
                    //se sono nel primo caricamento della pagina,
                    //eseguo il bind della griglia con la sorgente dati creata
                    bindDataGrid();
                }
                else
                {

                }
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void buildParametriPagina()
        {
            //creo i dati che dovr‡ gestire la pagina
            try
            {
                //eseguo l'inizializzazione dei controlli
                //sulla pagina
                //caricamentoControlliPagina();
                creazioneDataTableFascicoli();
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void setParametriPaginaInSession()
        {
            //memorizzo i parametri che dovranno essere
            //disponibili alla pagina tra le varie 
            //sequenze di postback
            try
            {
                FascicoliManager.setDatagridFascicolo(this, m_dataTableFascicoli);
                FascicoliManager.setHashFascicoli(this, m_hashTableFascicoli);
                FascicoliManager.setHashFascicoliADL(this, HT_fascicoli);
                FascicoliManager.setHashFascicoliSelezionati(this, HT_fascicoliSelezionati);
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void scaricaVariabiliSessione()
        {
            //scarica Variabili Sessione
            try
            {

            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void scaricaPagina()
        {
            //esegue la pulizia della session
            //e scarica la pagina
            try
            {
                scaricaVariabiliSessione();

                string funct = " window.close(); ";
                Response.Write("<script> " + funct + "</script>");
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void addChiamateJScriptAiComandi()
        {
            //per i controlli HTML lato client, aggiunge le chiamate 
            //ai gestori di evento javascript

        }

        #endregion

        #region sezione eventi webcontrols
        private void Page_PreRender(object sender, System.EventArgs e)
        {

            //per i controlli HTML lato client, aggiunge le chiamate 
            //ai gestori di evento javascript
            if (!IsPostBack)
            {
                addChiamateJScriptAiComandi();
            }
          

        }

        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                Utils.startUp(this);

                if (!Page.IsPostBack)
                {
                    this.AttatchGridPagingWaitControl();
                    this.chkSelectDeselectAll.Checked = false;
                    //HT_fascicoliSelezionati = new Hashtable();
                    
                }
                bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                if (Request.QueryString["newFasc"] != null && this.Request.QueryString["newFasc"].ToString().Equals("1"))
                {
                    Session["newFasc"] = "1";
                }

                //recupera i parametri dalla session o vengono costruiti appositamente in base ai parametri ricevuti.
                getParametri();

                //gestisce i parametri per il rendering della pagina.
                gestioneParametri();

                this.pagina = this.Request.QueryString["pagina"];
                //if (this.pagina == "ricfascADL")
                //{
                //    this.DataGrid1.Columns[0].Visible = true;
                //    if(!Page.IsPostBack)
                //        this.SelectAllCheck(false);
                //}
                //else
                //{
                //    this.DataGrid1.Columns[0].Visible = false;
                //    this.chkSelectDeselectAll.Visible = false;
                //}

                this.DataGrid1.Columns[0].Visible = true;
                //if (!Page.IsPostBack)
                //    this.SelectAllCheck(false);

                if (FascicoliManager.getFascicoloSelezionato(this) != null)
                {
                    //					if (Session["newFasc"] !=null && Session["newFasc"] == "1")
                    //					{
                    //						this.iFrame_cn.NavigateTo = "../fascicolo/fascDettagliFasc.aspx";
                    //					}
                    //					else
                    //					{
                    this.iFrame_cn.NavigateTo = "../fascicolo/fascDettagliFasc.aspx";
                    //					}
                }


                //salvo i check spuntati alla pagina cliccata in precedenza
                foreach (DataGridItem dgItem in DataGrid1.Items)
                {
                    if (HT_fascicoliSelezionati == null)
                        HT_fascicoliSelezionati = FascicoliManager.getHashFascicoliSelezionati(this);


                    if (HT_fascicoliSelezionati != null)
                    {
                        CheckBox checkBox = dgItem.FindControl("checkFasc") as CheckBox;
                        Label lbl_key = (Label)dgItem.FindControl("Label10");

                        if (lbl_key != null && checkBox != null)
                        {
                            if (checkBox.Checked)//se Ë spuntato lo inserisco
                            {
                                if (!HT_fascicoliSelezionati.ContainsKey(lbl_key.Text))
                                {
                                    HT_fascicoliSelezionati.Add(lbl_key.Text, HT_fascicoli[lbl_key.Text]);
                                }
                            }
                            else //se non Ë selezionato vedo se Ë in hashtable, in caso lo rimuovo
                            {
                                if (HT_fascicoliSelezionati.ContainsKey(lbl_key.Text))
                                {
                                    HT_fascicoliSelezionati.Remove(lbl_key.Text);
                                }
                            }
                        }
                    }
                }
                if (HT_fascicoliSelezionati != null)
                    FascicoliManager.setHashFascicoliSelezionati(this, HT_fascicoliSelezionati);


                /*   if (!IsPostBack)
                   {
                       FascicoliManager.removeMemoriaFiltriRicFasc(this);
                       FascicoliManager.removeFiltroRicFasc(this);
                   }*/
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void DataGrid1_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            this.DataGrid1.CurrentPageIndex = e.NewPageIndex;
            this.DataGrid1.SelectedIndex = -1;
            FascicoliManager.removeFascicoloSelezionato(this);

            DataGrid1.CurrentPageIndex = e.NewPageIndex;

            int numTotPage = 0;
            int pageNumbers = 0;
            int recordNumberTemp = 0;
            int nRec;
            SearchResultInfo[] idProfilesList = new SearchResultInfo[0];
            DocsPaWR.Fascicolo[] listaFascicoli = null;
            DocsPaWR.Registro registro = UserManager.getRegistroSelezionato(this);

            FascicolazioneClassificazione Classification = new FascicolazioneClassificazione();

            if (ViewState["classificazione"] != null)
            {
                Classification = ViewState["classificazione"] as FascicolazioneClassificazione;
            }

            DocsPaWR.FiltroRicerca[][] filtroRicerca = null;

            if (this.enableFiltriFasc)
                filtroRicerca = FascicoliManager.getFiltroRicFasc(this);
            else
                filtroRicerca = getFiltroRicerca();

            byte[] dati = null;
            if (ViewState["datiExcel"] != null)
            {
                dati = ViewState["datiExcel"] as byte[];
            }

            listaFascicoli = FascicoliManager.getListaFascicoliPaging(this, Classification, registro, filtroRicerca[0], true, e.NewPageIndex + 1, out pageNumbers, out recordNumberTemp, DataGrid1.PageSize, false, out idProfilesList, dati);

            //gestione tasto back
            int numPag = e.NewPageIndex + 1;

            if (listaFascicoli != null)
            {
                if (listaFascicoli.Length > 0)
                {
                    for (int i = 0; i < listaFascicoli.Length; i++)
                    {
                        DocsPaWR.Fascicolo fasc = listaFascicoli[i];
                        /*       DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, fasc.idClassificazione, UserManager.getUtente(this).idAmministrazione);
                               string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                               //						DateTime dataApertura=Utils.formatStringToDate(fasc.apertura);
                               //						DateTime dataChiusura=Utils.formatStringToDate(fasc.chiusura);
                         * */
                        string dataApertura = fasc.apertura;
                        string dataChiusura = fasc.chiusura;
                        try
                        {
                            if (!m_hashTableFascicoli.ContainsKey(fasc.systemID))
                                m_hashTableFascicoli.Add(fasc.systemID, fasc);
                            if (!HT_fascicoli.ContainsKey(fasc.systemID.ToUpper()))
                                HT_fascicoli.Add(fasc.systemID.ToUpper(), fasc);
                        }
                        catch
                        { }
                        this.dataSetRFasc1.element1.Addelement1Row(fasc.stato, fasc.descrizione, dataApertura, dataChiusura, fasc.tipo, i, fasc.codiceGerarchia, fasc.codice, fasc.codLegislatura, fasc.systemID, fasc.contatore, fasc.inConservazione);
                    }
                    m_dataTableFascicoli = this.dataSetRFasc1.Tables[0];

                    if (!this.call_type.Equals(tipoVisualizzazioneFascicoli.tvfAll.ToString()))
                    {
                        this.btn_deleteADL.Visible = true;
                    }
                    else
                    {
                        this.btn_deleteADL.Visible = false;
                    }
                    this.btn_insertAll.Visible = true;
                }
            }

            bindDataGrid();
        }

        private void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            DataGridItem row = grid.SelectedItem;
            //string keyFasc = ((Label)grid.Items[grid.SelectedIndex].Cells[8].Controls[1]).Text;
            //int i=Int32.Parse(keyFasc);
            string systemIdFasc = ((Label)grid.Items[grid.SelectedIndex].Cells[11].Controls[1]).Text;
            int i = Int32.Parse(systemIdFasc);

            DocsPaWR.Fascicolo fascicolo = FascicoliManager.getFascicoloById(this, (m_hashTableFascicoli[i.ToString()]).ToString());
            FascicoliManager.setFascicoloSelezionato(this, fascicolo);

            clientScriptAggiornaFascicolo(fascicolo);
            this.iFrame_cn.NavigateTo = "../fascicolo/fascDettagliFasc.aspx";
            //Session["res"]= true; // metto a true per poter classificare
        }

        #endregion

        #region sezione routine specifiche pagina
        public string getDecodeForStato(string stato)
        {
            return FascicoliManager.decodeStatoFasc(this, stato);
        }

        public string getDecodeForTipo(string stato)
        {
            return FascicoliManager.decodeTipoFasc(this, stato);
        }

        private DocsPAWA.DocsPaWR.FascicolazioneClassificazione getClassificazione()
        {
            DocsPaWR.FascicolazioneClassificazione retValue;
            //DocsPaWR.FascicolazioneClassificazione[] titolario=FascicoliManager.fascicolazioneGetTitolario(this,m_codClassificaSelezionata,false);
            DocsPaWR.FascicolazioneClassificazione[] titolario = FascicoliManager.fascicolazioneGetTitolario2(this, m_codClassificaSelezionata, false, idTitolario);
            if (titolario.Length > 0)
            {
                retValue = titolario[0];
            }
            else
            {
                retValue = null;
            }
            return retValue;
        }

        private DocsPAWA.DocsPaWR.FiltroRicerca[][] getFiltroRicerca()
        {
            try
            {
                DocsPaWR.FiltroRicerca[][] qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
                DocsPaWR.FiltroRicerca[] fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];
                DocsPaWR.FiltroRicerca fV1;

                #region Filtro tipo fascicolo
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.TIPO_FASCICOLO.ToString();
                switch (m_tipoVisualizzazioneFascicoli)
                {
                    case tipoVisualizzazioneFascicoli.tvfProcedimentali:
                        fV1.valore = "P";
                        break;

                    case tipoVisualizzazioneFascicoli.tvfGenerali:
                        fV1.valore = "G";
                        break;

                    case tipoVisualizzazioneFascicoli.tvfAll:
                        break;

                }

                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                #endregion

                #region Filtro diTitolario
                if (this.idTitolario != null && this.idTitolario != string.Empty)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_TITOLARIO.ToString();
                    fV1.valore = this.idTitolario;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region Filtro su codice classificazione fascicolo

                if (m_codClassificaSelezionata != null && m_codClassificaSelezionata != string.Empty)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.CODICE_CLASSIFICA.ToString();
                    fV1.valore = m_codClassificaSelezionata;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                #endregion

                #region Filtro registro
                if (UserManager.getRegistroSelezionato(this) != null)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_REGISTRO.ToString();
                    fV1.valore = UserManager.getRegistroSelezionato(this).systemId;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region ADL
                if (ViewState["ADL"] != null && ViewState["ADL"].ToString().Equals("true"))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.DOC_IN_FASC_ADL.ToString();
                    fV1.valore = UserManager.getInfoUtente(this).idPeople.ToString() + "@" + UserManager.getRuolo(this).systemId.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                qV[0] = fVList;

                return qV;
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
                return null;
            }
        }

        private void creazioneDataTableFascicoliDaAreaLavoro()
        {
            /*  DocsPaWR.FascicolazioneClassificazione classificazione = getClassificazione();
              DocsPaWR.FiltroRicerca[][] filtroRicerca = getFiltroRicerca();

              DocsPaWR.Fascicolo[] listaFascicoli = FascicoliManager.getFascicoliInAreaLavoro(this);
              if (listaFascicoli != null)
              {
                  if (listaFascicoli.Length > 0)
                  {
                      m_hashTableFascicoli = new Hashtable(listaFascicoli.Length);
                      HT_fascicoli = new Hashtable(listaFascicoli.Length);

                      for (int i = 0; i < listaFascicoli.Length; i++)
                      {
                          DocsPaWR.Fascicolo fasc = listaFascicoli[i];
                          DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, fasc.idClassificazione, UserManager.getUtente(this).idAmministrazione);
                          string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                          try
                          {
                              if (!m_hashTableFascicoli.ContainsKey(fasc.systemID.ToUpper()))
                                  m_hashTableFascicoli.Add(fasc.systemID.ToUpper(), fasc);

                              if (!HT_fascicoli.ContainsKey(fasc.systemID.ToUpper()))
                                  HT_fascicoli.Add(fasc.systemID.ToUpper(), fasc);
                          }
                          catch { }

                          //DateTime dataApertura=Utils.formatStringToDate(fasc.apertura);
                          //DateTime dataChiusura=Utils.formatStringToDate(fasc.chiusura);
                          string dataApertura = fasc.apertura;
                          string dataChiusura = fasc.chiusura;
                          this.dataSetRFasc1.element1.Addelement1Row(fasc.stato, fasc.descrizione, dataApertura, dataChiusura, fasc.tipo, i, codiceGerarchia, fasc.codice, fasc.codLegislatura, fasc.systemID, fasc.contatore, fasc.inConservazione);
                      }
                      m_dataTableFascicoli = this.dataSetRFasc1.Tables[0];*/

            ViewState["ADL"] = "true" as string;
            DocsPaWR.FiltroRicerca[][] filtroRicerca = null;

            DocsPaWR.Fascicolo[] listaFascicoli = null;

            if (this.enableFiltriFasc)
                filtroRicerca = FascicoliManager.getFiltroRicFasc(this);
            else
                filtroRicerca = getFiltroRicerca();

            DocsPaWR.Registro registro = UserManager.getRegistroSelezionato(this);

            FascicolazioneClassificazione Classification = new FascicolazioneClassificazione();

            int pageNumbers = 0;
            int recordNumberTemp = 0;
            SearchResultInfo[] idProjects = null;
            int SelectedPage = 1;

            FiltroRicerca[][] qV;
            qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
            qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
            FiltroRicerca[] fVList;
            FiltroRicerca fV1;
            fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];
            fVList = filtroRicerca[0];
            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriFascicolazione.DOC_IN_FASC_ADL.ToString();
            fV1.valore = UserManager.getInfoUtente(this).idPeople.ToString() + "@" + UserManager.getRuolo(this).systemId.ToString();
            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            qV[0] = fVList;

            FascicoliManager.setFiltroRicFasc(this, qV);

            listaFascicoli = FascicoliManager.getListaFascicoliPaging(this, Classification, registro, fVList, true, SelectedPage, out pageNumbers, out recordNumberTemp, DataGrid1.PageSize, true, out idProjects, null);
            /*     }
                 else
                 {
                     if (Session["newFasc"] == null)
                     {
                   
                         listaFascicoli = FascicoliManager.getListaFascicoli(this, classificazione, filtroRicerca[0], showChilds, registro, dati);
                     }
                 }*/
            this.DataGrid1.VirtualItemCount = recordNumberTemp;

            this.DataGrid1.CurrentPageIndex = currentPage - 1;


            if (listaFascicoli != null)
            {
                if (listaFascicoli.Length > 0)
                {
                    m_hashTableFascicoli = new Hashtable(listaFascicoli.Length);
                    HT_fascicoli = new Hashtable(listaFascicoli.Length);

                    for (int i = 0; i < listaFascicoli.Length; i++)
                    {
                        DocsPaWR.Fascicolo fasc = listaFascicoli[i];
                        string dataApertura = fasc.apertura;
                        string dataChiusura = fasc.chiusura;










                        this.dataSetRFasc1.element1.Addelement1Row(fasc.stato, fasc.descrizione, dataApertura, dataChiusura, fasc.tipo, i, fasc.codiceGerarchia, fasc.codice, fasc.codLegislatura, fasc.systemID, fasc.contatore, fasc.inConservazione);




                    }
                    m_dataTableFascicoli = this.dataSetRFasc1.Tables[0];
                }

                if (idProjects != null && idProjects.Length > 0)
                {
                    for (int i = 0; i < idProjects.Length; i++)
                    {
                        try
                        {
                            if (!m_hashTableFascicoli.ContainsKey(idProjects[i].Id))
                                m_hashTableFascicoli.Add(idProjects[i].Id, idProjects[i].Id);
                            if (!HT_fascicoli.ContainsKey(idProjects[i].Id.ToUpper()))
                                HT_fascicoli.Add(idProjects[i].Id.ToUpper(), idProjects[i].Id);
                        }
                        catch
                        {

                        }
                    }
                }

                if (!this.call_type.Equals(tipoVisualizzazioneFascicoli.tvfAll.ToString()))
                {
                    this.btn_deleteADL.Visible = true;
                }
                else
                {
                    this.btn_deleteADL.Visible = false;
                }
                this.btn_insertAll.Visible = true;

                if (listaFascicoli == null || listaFascicoli.Length == 0)
                {
                    Response.Redirect("../resultBlankPage.aspx?messaggio=Fascicoli non presenti");
                }
            }
        }

        private void creazioneDataTableFascicoliDiClassifica(string codClassifica)
        {
            DocsPaWR.FascicolazioneClassificazione classificazione = null;
            DocsPaWR.FiltroRicerca[][] filtroRicerca = null;

            //new
            if (codClassifica != null && !codClassifica.Equals(""))
            {
                DocsPaWR.FascicolazioneClassificazione[] titolario = FascicoliManager.fascicolazioneGetTitolario2(this, codClassifica, false, idTitolario);
                //Controlli non pi˘ necessari perchË c'Ë la possiblit‡ di scelta del titolario se il codice inserto Ë presente su pi˘ di un titolario
                if (titolario != null && titolario.Length == 1)
                    classificazione = titolario[0];
            }
            FascicoliManager.setClassificazioneSelezionata(this, classificazione);
            // end new	

            DocsPaWR.Fascicolo[] listaFascicoli = null;

            if (this.enableFiltriFasc)
                filtroRicerca = FascicoliManager.getFiltroRicFasc(this);
            else
                filtroRicerca = getFiltroRicerca();

            byte[] dati = null;
            if (Session["DatiExcel"] != null)
            {
                dati = (byte[])Session["DatiExcel"];
                ViewState["datiExcel"] = dati as byte[];
                Session.Remove("DatiExcel");
            }


            DocsPaWR.Registro registro = UserManager.getRegistroSelezionato(this);

            FascicolazioneClassificazione Classification = new FascicolazioneClassificazione();

            if (codClassifica != null && codClassifica != "")
            {
                //listaFascicoli = FascicoliManager.getListaFascicoli(this, classificazione, filtroRicerca[0], showChilds, dati);

                int classificationId = 0;
                Int32.TryParse(codClassifica, out classificationId);
                Classification = GetClassification(classificationId);
                ViewState["classificazione"] = Classification as FascicolazioneClassificazione;
            }









            int pageNumbers = 0;
            int recordNumberTemp = 0;
            SearchResultInfo[] idProjects = null;
            int SelectedPage = 1;

            listaFascicoli = FascicoliManager.getListaFascicoliPaging(this, Classification, registro, filtroRicerca[0], true, SelectedPage, out pageNumbers, out recordNumberTemp, DataGrid1.PageSize, true, out idProjects, dati);
            /*     }
                 else
                 {
                     if (Session["newFasc"] == null)
                     {
                   
                         listaFascicoli = FascicoliManager.getListaFascicoli(this, classificazione, filtroRicerca[0], showChilds, registro, dati);
                     }
                 }*/
            this.DataGrid1.VirtualItemCount = recordNumberTemp;

            this.DataGrid1.CurrentPageIndex = currentPage - 1;

            if (listaFascicoli != null)
            {
                if (listaFascicoli.Length > 0)
                {
                    m_hashTableFascicoli = new Hashtable(listaFascicoli.Length);
                    HT_fascicoli = new Hashtable(listaFascicoli.Length);
                    for (int i = 0; i < listaFascicoli.Length; i++)
                    {
                        DocsPaWR.Fascicolo fasc = listaFascicoli[i];
                        /*      DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, fasc.idClassificazione, UserManager.getUtente(this).idAmministrazione);
                              string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                              //						DateTime dataApertura=Utils.formatStringToDate(fasc.apertura);
                              //						DateTime dataChiusura=Utils.formatStringToDate(fasc.chiusura);
                         * */
                        string dataApertura = fasc.apertura;
                        string dataChiusura = fasc.chiusura;


                        this.dataSetRFasc1.element1.Addelement1Row(fasc.stato, fasc.descrizione, dataApertura, dataChiusura, fasc.tipo, i, fasc.codiceGerarchia, fasc.codice, fasc.codLegislatura, fasc.systemID, fasc.contatore, fasc.inConservazione);







                    }
                    m_dataTableFascicoli = this.dataSetRFasc1.Tables[0];


                    if (!this.call_type.Equals(tipoVisualizzazioneFascicoli.tvfAll.ToString()))
                    {
                        this.btn_deleteADL.Visible = true;
                    }
                    else
                    {
                        this.btn_deleteADL.Visible = false;
                    }
                    this.btn_insertAll.Visible = true;
                }

                if (idProjects != null && idProjects.Length > 0)
                {
                    for (int i = 0; i < idProjects.Length; i++)
                    {
                        try
                        {
                            if (!m_hashTableFascicoli.ContainsKey(idProjects[i].Id))
                                m_hashTableFascicoli.Add(idProjects[i].Id, idProjects[i].Id);
                            if (!HT_fascicoli.ContainsKey(idProjects[i].Id.ToUpper()))
                                HT_fascicoli.Add(idProjects[i].Id.ToUpper(), idProjects[i].Id);
                        }
                        catch
                        {

                        }
                    }
                }

            }
        }

        private void creazioneDataTableFascicoli()
        {
            try
            {
                string descTipoLista;

                if (m_tipoVisualizzazioneFascicoli == tipoVisualizzazioneFascicoli.tvfDaAreaDiLavoro)
                {
                    //creazione datatable per visualizzazione fascicoli dell'area di lavoro
                    creazioneDataTableFascicoliDaAreaLavoro();
                    descTipoLista = "Fascicoli in area di lavoro";
                }
                else
                {
                    //creazione datatable per visualizzazione fascicoli della classifica specificata
                    creazioneDataTableFascicoliDiClassifica(m_codClassificaSelezionata);
                    descTipoLista = "Fascicoli di classifica";
                }
                this.lbl_tipoLista.Text = descTipoLista;
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void visibilit‡Griglia(bool visibile)
        {
            string colorTd;
            string colorTd2;
            if (visibile)
            {
                colorTd = "#810d06";
                colorTd2 = "#c08582";
            }
            else
            {
                colorTd = "#ffffff";
                colorTd2 = "#ffffff";
            }

            this.TD1.BgColor = colorTd;
            this.TD2.BgColor = colorTd2;
            this.DataGrid1.Visible = visibile;

            if (!visibile)
            {
                Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location='../resultBlankPage.aspx?messaggio=Fascicoli non presenti&titolo=Fascicoli di classifica';</script>");
            }
        }

        private void bindDataGrid()
        {
            if (m_dataTableFascicoli != null)
            {
                DataGrid1.DataSource = m_dataTableFascicoli.DefaultView;
                DataGrid1.DataBind();
                visibilit‡Griglia(true);
            }
            else
            {
                visibilit‡Griglia(false);
            }
        }

        private void clientScriptAggiornaFascicolo(DocsPAWA.DocsPaWR.Fascicolo fascicolo)
        {
            string script = "<script>window.parent.frames(0).document.IframeTabs.document.docClassifica.txt_codFasc.disabled=false;window.parent.frames(0).document.IframeTabs.document.docClassifica.txt_codFasc.value='" +
                fascicolo.codice.Replace(@"\", @"\\") + "';</script>";

            Page.RegisterStartupScript("codFasc", script);

            string script1 = "<script>window.parent.frames(0).document.IframeTabs.document.docClassifica.h_codFasc.value='" + fascicolo.codice.Replace(@"\", @"\\") + "';</script>";

            Page.RegisterStartupScript("h_codFasc", script1);
        }

        /// <summary>
        /// Funzione per il recupero dell'oggetto classificazione da utilizzare per la ricerca
        /// </summary>
        /// <param name="classificationId">Id classificazione. Viene passato nel querystring</param>
        /// <returns>Oggetto classificazione da utilizzare per la ricerca dei fascicoli</returns>
        private FascicolazioneClassificazione GetClassification(int classificationId)
        {
            // Valore da restituire
            FascicolazioneClassificazione toReturn = null;

            // Se classificationId Ë diverso da 0, viene prelevato il valore salvato in sessione
            if (classificationId != 0)
                toReturn = FascicoliManager.getClassificazioneSelezionata(this);

            // Se in sessione c'Ë classificaSelezionata, l'oggetto di classificazione
            // viene caricato da WS
            if (toReturn == null && Session["classificaSelezionata"] != null)
                try
                {
                    toReturn = FascicoliManager.fascicolazioneGetTitolario2(
                        this,
                        ((FascicolazioneClassifica)Session["classificaSelezionata"]).codice,
                        false,
                        ((FascicolazioneClassifica)Session["classificaSelezionata"]).idTitolario)[0];
                }
                catch (Exception e) { }

            //if (FascicoliManager.getMemoriaClassificaRicFasc(this) != null)
            //    toReturn = FascicoliManager.getMemoriaClassificaRicFasc(this);

            return toReturn;
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
            this.dataSetRFasc1 = new DocsPAWA.dataSet.DataSetRFasc();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetRFasc1)).BeginInit();
            this.btn_deleteADL.Click += new System.Web.UI.ImageClickEventHandler(this.btn_deleteADL_Click);
            this.btn_insertAll.Click += new System.Web.UI.ImageClickEventHandler(this.btn_insertAll_Click);
            this.DataGrid1.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
            this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_PageIndexChanged);
            this.DataGrid1.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.DataGrid1_SortCommand);
            this.DataGrid1.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGrid1_ItemDataBound);
            this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
            this.chkSelectDeselectAll.CheckedChanged += new System.EventHandler(this.chkSelectDeselectAll_CheckedChanged);
            // 
            // dataSetRFasc1
            // 
            this.dataSetRFasc1.DataSetName = "DataSetRFasc";
            this.dataSetRFasc1.Locale = new System.Globalization.CultureInfo("en-US");
            this.Load += new System.EventHandler(this.Page_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataSetRFasc1)).EndInit();

        }
        #endregion

        #region Eventi vari

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

        private void DataGrid1_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
        {
            string sortExpression;
            string direction;
            sortExpression = e.SortExpression;
            direction = this.DirectionSorting;
            ChangeDirectionSorting(direction);
            m_dataTableFascicoli.DefaultView.Sort = sortExpression + " " + direction;
            bindDataGrid();
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

        private void DataGrid1_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                CheckBox checkBox = e.Item.FindControl("checkFasc") as CheckBox;
                Label key = e.Item.FindControl("Label10") as Label;
                //prendo la hashTable dei checkbox ceccati
                HT_fascicoliSelezionati = FascicoliManager.getHashFascicoliSelezionati(this);
                if (HT_fascicoliSelezionati != null)
                {
                    if (HT_fascicoliSelezionati.ContainsKey(key.Text))
                    {
                        checkBox.Checked = true;
                    }
                }
            }
        }

        private void chkSelectDeselectAll_CheckedChanged(object sender, System.EventArgs e)
        {
            this.SelectAllCheck(((CheckBox)sender).Checked);
            /*            if (this.allChecked.Checked)
                        {
                            for (int i = 0; i < this.DataGrid1.Items.Count; i++)
                            {
                                ((CheckBox)this.DataGrid1.Items[i].Cells[0].Controls[1]).Checked = true;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < this.DataGrid1.Items.Count; i++)
                            {
                                ((CheckBox)this.DataGrid1.Items[i].Cells[0].Controls[1]).Checked = false;
                            }
                        }
             */
        }

        private void SelectAllCheck(bool value)
        {
            DataGridItemCollection gridItems = this.DataGrid1.Items;

            getParametriPaginaInSession();
            //if (ViewState["HTfascSel"] != null)
            //{ 
            m_hashTableFascicoli = FascicoliManager.getHashFascicoli(this);

            //   foreach (DataRow row in m_dataTableFascicoli.Rows)
            foreach (DictionaryEntry Item in m_hashTableFascicoli)
            {
                //HT_fascicoliSelezionati = (Hashtable)ViewState["HTfascSel"];
                if (HT_fascicoliSelezionati == null)
                    HT_fascicoliSelezionati = new Hashtable(m_hashTableFascicoli.Count);

                string IdFasc = Item.Key.ToString();
                if (value)
                {
                    if (!HT_fascicoliSelezionati.ContainsKey(IdFasc))
                        HT_fascicoliSelezionati.Add(IdFasc, HT_fascicoli[IdFasc]);
                }

                else
                {
                    if (HT_fascicoliSelezionati.ContainsKey(IdFasc))
                        HT_fascicoliSelezionati.Remove(IdFasc);

                }
            }
            FascicoliManager.setHashFascicoliSelezionati(this, HT_fascicoliSelezionati);

            foreach (DataGridItem gridItem in gridItems)
            {
                CheckBox checkBox = gridItem.Cells[0].Controls[0].FindControl("checkFasc") as CheckBox;

                if (checkBox != null)
                    checkBox.Checked = value;
            }
        }

        protected void checkFasc_OnCheckedChanged(object sender, System.EventArgs e)
        {
            //todo :add this.HT_fascicoli.add o remove
            // todo: put in session HT_fascicoli

            string IdFasc;
            CheckBox checkBox;

            // Casting della checkbox
            checkBox = sender as CheckBox;
            IdFasc = ((HiddenField)checkBox.Parent.FindControl("hfFascId")).Value;

            if (HT_fascicoliSelezionati == null)
            {
                HT_fascicoliSelezionati = new Hashtable(m_dataTableFascicoli.Rows.Count);
            }

            if (checkBox.Checked)
            {
                if (!HT_fascicoliSelezionati.ContainsKey(IdFasc))
                {
                    HT_fascicoliSelezionati.Add(IdFasc, HT_fascicoli[IdFasc]);
                }
            }
            else
            {
                if (HT_fascicoliSelezionati.ContainsKey(IdFasc))
                {
                    HT_fascicoliSelezionati.Remove(IdFasc);
                }
            }

            FascicoliManager.setHashFascicoliSelezionati(this, HT_fascicoliSelezionati);


        }

        private void btn_insertAll_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string messaggio = "";
            DocsPaWR.SchedaDocumento doc = DocumentManager.getDocumentoSelezionato(this);
            DocsPaWebService ws = new DocsPaWebService();

            //Controllo i diritti sul documento
            string accessRight = ws.getAccessRightDocBySystemID(doc.systemId, UserManager.getInfoUtente(this));
            if (accessRight.Equals("20"))
            {
                Response.Write("<script>alert('Il documento Ë in attesa di accettazione, quindi non puÚ essere fascicolato');</script>");
                return;
            }
            bool noSel = false;
            //if (this.DataGrid1.Items.Count > 0)
            //{
            //    for (int i = 0; i < this.DataGrid1.Items.Count; i++)
            //    {
            //        if (((CheckBox)this.DataGrid1.Items[i].Cells[0].Controls[1]).Checked)
            //            noSel = true;
            //    }
            //}

            if (HT_fascicoliSelezionati != null)
            {
                if (HT_fascicoliSelezionati.Count == 0)
                {
                    Response.Write("<script>alert('Selezionare almeno un fascicolo per la classificazione in ADL');</script>");
                    return;
                }

                if (HT_fascicoliSelezionati.Count > 0)
                {

                    //for (int i = 0; i < this.DataGrid1.Items.Count; i++)
                    //{
                    //    bool check = ((CheckBox)this.DataGrid1.Items[i].Cells[0].Controls[1]).Checked;
                    //    string system_id_fasc = ((Label)this.DataGrid1.Items[i].Cells[11].Controls[1]).Text;
                    //    if (check == true && HT_fascicoli.Contains(system_id_fasc))
                    //        HT_fascicoliSelezionati.Add(system_id_fasc, HT_fascicoli[system_id_fasc]);
                    //}

                    //Codice commentato per gestire la selezione dei fascicoli nei quali classificare       
                    //for (int i = 0; i < this.DataGrid1.Items.Count; i++)
                    //{
                    //    bool check = ((CheckBox)this.DataGrid1.Items[i].Cells[0].Controls[1]).Checked;
                    //    if (check == false)
                    //    {
                    //        string systemId = ((Label)this.DataGrid1.Items[i].Cells[11].Controls[1]).Text;
                    //        if (systemId == "" || systemId == null)
                    //            return;
                    //        this.HT_fascicoli.Remove(systemId);
                    //    }
                    //}

                    //foreach (DictionaryEntry entry in this.HT_fascicoli)
                    foreach (DictionaryEntry entry in HT_fascicoliSelezionati)
                    {
                        string systemId = (string)entry.Key;
                        DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloById(this, systemId);
                        bool control = true;

                        DocsPaWR.Folder selectedFolder;
                        bool isInFolder = false;
                        string codice = fasc.codice.Replace(@"\", @"\\");
                        if (fasc != null)
                        {
                            //se lo stato del fascicolo Ë chiuso non si deve inserire il documento
                            if (fasc.stato == "C")
                            {
                                messaggio += "Attenzione! Il fascicolo " + codice + " Ë chiuso!\\n";
                                control = false;
                            }
                            if (fasc.accessRights != null && Convert.ToInt32(fasc.accessRights) <= 45 && control)
                            {
                                messaggio += "Attenzione! Il fascicolo " + codice + " Ë in sola lettura!\\n";
                                control = false;
                            }

                            //Ricavo la folder dal fascicolo selezionato					
                            selectedFolder = FascicoliManager.getFolder(this, fasc);

                            bool inserted = false;

                            if (selectedFolder != null && control)
                            {
                                String message = String.Empty;
                                //Inserisco/Classifico il documento nel folder/sottoFascicolo selezionato
                                inserted = DocumentManager.addDocumentoInFolder(this, doc.systemId, selectedFolder.systemID, false, out isInFolder, out message);
                            }

                            //dopo la classifica rimuovo la Folder Selezionata
                            //FascicoliManager.removeFolderSelezionato(this);

                            if (isInFolder && control) // SE il doc Ë gi‡ nella folder indicata
                            {
                                messaggio += "Attenzione! Il documento Ë gi‡ classificato nel fascicolo " + codice + "!\\n";
                                control = false;
                            }
                            else if (inserted)
                            {
                                messaggio += "Documento classificato correttamente nel fascicolo " + codice + "!\\n";
                            }
                            else
                            {
                                messaggio += "Errore in inserimento del documento nel fascicolo " + codice + "!\\n";
                            }

                            //string pageName = Request.Url.Segments[Request.Url.Segments.Length - 1].ToString();
                            //Response.Write("<script>top.principale.iFrame_dx.document.location='tabDoc.aspx';window.document.location='" + pageName + "';</script>");

                        }
                        else
                        {
                            messaggio += "Attenzione! codice fascicolo " + codice + " non presente!\\n";
                        }
                    }
                    Response.Write("<script>alert('" + messaggio + "');</script>");

                    // ritorno alla pagina principale
                    // string pageName = Request.Url.Segments[Request.Url.Segments.Length - 1].ToString();
                    // Response.Write("<script>top.principale.iFrame_dx.document.location='tabDoc.aspx';window.document.location='" + pageName + "';</script>");
                    Response.Write("<script>top.principale.iFrame_dx.document.location='tabDoc.aspx';top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=classifica';</script>");
                }
            }
            else
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "seleziona fascicoli", "alert('Selezionare almeno un fascicolo')", true);
            }
        }

        private void btn_deleteADL_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (HT_fascicoliSelezionati != null)
            {
                bool no_fasc = false;
                if (this.DataGrid1.Items.Count == 0)
                {
                    Response.Redirect("../resultBlankPage.aspx?messaggio=Fascicoli non presenti&titolo=Fascicoli di classifica");
                }

                int found = -1;
                if (HT_fascicoliSelezionati != null && HT_fascicoliSelezionati.Count > 0)
                {
                    foreach (DictionaryEntry entry in HT_fascicoliSelezionati)
                    {
                        string systemId = (string)entry.Key;
                        DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloById(this, systemId);
                        if (HT_fascicoliSelezionati.ContainsKey(systemId))
                        {
                            int int_systemId = Int32.Parse(systemId);
                            //DocsPaWR.Fascicolo fascicolo = (DocsPAWA.DocsPaWR.Fascicolo)m_hashTableFascicoli[int_systemId];

                            //elimino il fasc da area di lavoro
                            DocumentManager.eliminaDaAreaLavoro(this, null, fasc);

                            //aggiorno il dataGrid
                            m_hashTableFascicoli.Remove(int_systemId);
                            found = int_systemId;

                            no_fasc = true;
                        }
                    }

                }
                if (found != -1)
                {
                    HT_fascicoliSelezionati.Clear();
                    FascicoliManager.setHashFascicoliSelezionati(this, HT_fascicoliSelezionati);
                    FascicoliManager.setHashFascicoli(this, m_hashTableFascicoli);
                    if (m_dataTableFascicoli.DefaultView.Count == 0)
                    {
                        m_hashTableFascicoli.Clear();
                        m_dataTableFascicoli.Clear();
                        FascicoliManager.removeDatagridFascicolo(this);
                        FascicoliManager.removeHashFascicoli(this);
                        Response.Redirect("../resultBlankPage.aspx?messaggio=Fascicoli non presenti");
                    }
                    else
                    {
                        DataGrid1.CurrentPageIndex = 0;
                        creazioneDataTableFascicoliDaAreaLavoro();
                        this.bindDataGrid();
                    }
                }
            }
            else
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "seleziona fascicoli", "alert('Selezionare almeno un fascicolo')", true);
            }
            //}
        }

        private void DataGrid1_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            //			DataGrid grid=(DataGrid)source;
            //			DataGridItem row=grid.SelectedItem;
            //			string keyFasc=((Label)grid.Items[grid.SelectedIndex].Cells[7].Controls[1]).Text;
            //			int i=Int32.Parse(keyFasc);
            //			
            //			DocsPaWR.Fascicolo fascicolo=(DocsPAWA.DocsPaWR.Fascicolo)m_hashTableFascicoli[i];
            //			FascicoliManager.setFascicoloSelezionato(this,fascicolo);
            //			this.iFrame_cn.NavigateTo="../fascicolo/fascDettagliFasc.aspx";
        }

        private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (e.CommandName == "Update")
            {
                DataGrid grid = (DataGrid)source;
                DataGridItem row = grid.SelectedItem;
                string keyFasc = ((Label)grid.Items[e.Item.ItemIndex].Cells[8].Controls[1]).Text;
                int i = Int32.Parse(keyFasc);
                DocsPaWR.Fascicolo fascicolo = FascicoliManager.getFascicoloById(this, (m_hashTableFascicoli[i.ToString()]).ToString());
                FascicoliManager.setFascicoloSelezionato(this, fascicolo);
                this.iFrame_cn.NavigateTo = "../fascicolo/fascDettagliFasc.aspx";
            }
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

        #endregion

    }
}
