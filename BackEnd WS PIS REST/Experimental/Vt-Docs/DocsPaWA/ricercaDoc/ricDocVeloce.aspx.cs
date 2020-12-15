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
using DocsPAWA.utils;
using System.Collections.Generic;

namespace DocsPAWA.ricercaDoc
{
    /// <summary>
    /// Summary description for ricDocVeloce.
    /// </summary>
    public class ricDocVeloce : DocsPAWA.CssPage
    {
        protected DocsPAWA.DocsPaWR.Utente userHome;
        protected DocsPAWA.DocsPaWR.Ruolo userRuolo;
        protected DocsPAWA.DocsPaWR.InfoUtente Safe;
        protected System.Web.UI.HtmlControls.HtmlForm f_Ricerca;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
        protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
        protected DocsPAWA.DocsPaWR.Documento[] InfoDocV;
        protected System.Web.UI.WebControls.DropDownList droplistLimitaRisultatiRicerca;
        protected System.Web.UI.WebControls.TextBox txt_oggetto;
        protected System.Web.UI.WebControls.Label lblFiltri;
        protected System.Web.UI.WebControls.Label lblOggetto;
        protected System.Web.UI.WebControls.RadioButton optSearchOggetto;
        protected System.Web.UI.WebControls.RadioButton optSearchFullText;
        protected System.Web.UI.WebControls.Label lblRicercaPer;
        protected System.Web.UI.WebControls.TextBox txtTestoContenuto;
        protected System.Web.UI.WebControls.Label lblTestoContenuto;
        protected System.Web.UI.WebControls.Panel pnlSearchFullText;
        protected System.Web.UI.WebControls.Panel pnlSearchOptions;
        protected System.Web.UI.WebControls.Panel pnlSearchOggetto;
        protected System.Web.UI.WebControls.ImageButton Imagebutton1;
        protected System.Web.UI.WebControls.Button butt_ricerca;
        protected System.Web.UI.WebControls.ImageButton enterKeySimulator;
        protected System.Web.UI.HtmlControls.HtmlInputHidden fullTextAlertMessage;
        protected System.Web.UI.WebControls.Panel pnlSearchStorage;
        protected System.Web.UI.WebControls.DropDownList ddl_Ric_Salvate;
        protected System.Web.UI.WebControls.ImageButton btn_Canc_Ric;
        protected Utilities.MessageBox mb_ConfirmDelete;
        protected System.Web.UI.WebControls.Label lblSearch;
        protected System.Web.UI.HtmlControls.HtmlInputControl clTesto;
        protected int caratteriDisponibili = 2000;
        protected bool change_from_grid;
        protected string numResult;

        private const string KEY_SCHEDA_RICERCA = "RicercaDocVeloce";


        #region per memorizzare i registri selezionati
        protected DocsPAWA.DocsPaWR.Registro[] listaReg;
        #endregion
        private SchedaRicerca schedaRicerca = null;

        protected void RicercaVeloce()
        {
            //if (this.txt_oggetto.Text != "")
            //{
            try
            {

                //filtro di ricerca


                //lista filtri di ricerca (1 per oggetto, 1 per corrispondente)
                qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];

                qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
                //qV[1] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];


                fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];

                //filtro Oggetto
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = FiltriDocumento.OGGETTO.ToString();//"OGGETTO";//DocsPaWR.FiltroRicerca .listaArgomenti.OGGETTO.ToString();
                fV1.valore = this.txt_oggetto.Text;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                //filtro per prendere tutti i documenti (arrivo, partenza, interni, grigi, da protocollare, stampe registro)
                // non utilizzato (sostituito dai filtri che seguono)
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                //fV1.valore="T";
                fV1.valore = "tipo";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                // filtro protocolli in arrivo
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
                //fV1.valore="T";
                fV1.valore = "true";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                // filtro protocolli in partenza
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
                //fV1.valore="T";
                fV1.valore = "true";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                // filtro protocolli interni
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
                //fV1.valore="T";
                fV1.valore = "true";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                // filtro grigi
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
                //fV1.valore="T";
                fV1.valore = "true";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                // filtro predisposti alla protocollazione
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                //fV1.valore="T";
                fV1.valore = "false";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                //filtro di ricerca documenti (drop down list)
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                if (this.droplistLimitaRisultatiRicerca.SelectedValue == "DOC_DATA_ODIERNA")
                {
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
                    fV1.valore = DateTime.Now.ToShortDateString();
                }
                else
                {
                    fV1.argomento = DocsPaWR.FiltriDocumento.VISUALIZZA_TOP_N_DOCUMENTI.ToString();
                    fV1.valore = this.droplistLimitaRisultatiRicerca.SelectedValue;
                }
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                //ABBATANGELI GIANLUIGI - Filtro per nascondere doc di altre applicazioni
                if (System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"] != null && !System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"].Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.COD_EXT_APP.ToString();
                    fV1.valore = (System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"]);
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                //qV[0]=fVList;

                //fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[6]; 

                //filtro per prendere solo i documenti protocollati
                //fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
                //fV1.argomento=DocsPaWR.FiltriDocumento.DA_PROTOCOLLARE.ToString();
                //fV1.valore= "0";  //corrisponde a 'false'
                //fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                ////filtro per prendere solo i documenti in arrivo o partenza
                //fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
                //fV1.argomento=DocsPaWR.FiltriDocumento.TIPO.ToString();
                //fV1.valore="T";
                //fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.RICERCA_FULL_TEXT.ToString();
                fV1.valore = (this.optSearchFullText.Checked ? "true" : "false");
                //fV1.valore = (this.optSearchFullText.Checked ? "1" : "0");
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //fV1.argomento = DocsPaWR.FiltriDocumento.TESTO_RICERCA_FULL_TEXT.ToString();
                fV1.argomento = DocsPaWR.FiltriDocumento.SEARCH_DOCUMENT_SIMPLE.ToString();
                fV1.valore = this.txtTestoContenuto.Text;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                //string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"].ToUpper();
                //string order = string.Empty;
                //if (dbType.ToUpper().Equals("SQL"))
                //    order = "A.DATA ";
                //else
                //    order = "nvl(A.DTA_PROTO,A.CREATION_TIME) ";
                //fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //fV1.argomento=DocsPaWR.FiltriDocumento.CONDIZIONE_ORDINAMENTO.ToString();
                //fV1.valore=order;
                //fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                fV1.valore = listaRegistriPerRicerca();
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                #region filtro RICERCA IN AREA LAVORO
                if ((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1"))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOC_IN_ADL.ToString();
                    fV1.valore = UserManager.getInfoUtente(this).idPeople.ToString() + "@" + UserManager.getRuolo(this).systemId.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region Ordinamento

                // Reperimento del filtro da utilizzare per la griglia
                List<FiltroRicerca> filterList;

                if (GridManager.SelectedGrid.FieldForOrder != null)
                {
                    Field field = GridManager.SelectedGrid.FieldForOrder;
                    filterList = GridManager.GetOrderFilterForDocument(
                        field.FieldId,
                        GridManager.SelectedGrid.OrderDirection.ToString());
                }
                else
                    filterList = GridManager.GetOrderFilterForDocument(String.Empty, "DESC");

                // Se la lista è valorizzata vengono aggiunti i filtri
                if (filterList != null)
                    foreach (FiltroRicerca filter in filterList)
                        fVList = Utils.addToArrayFiltroRicerca(fVList, filter);

                #endregion


                qV[0] = fVList;

                DocumentManager.setFiltroRicDoc(this, qV);
                DocumentManager.removeDatagridDocumento(this);
                DocumentManager.removeListaDocProt(this);

            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
            //}		
        }

        private string listaRegistriPerRicerca()
        {
            DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistri(this);
            string inCondition = " ";

            for (int i = 0; i < userRegistri.Length; i++)
            {
                inCondition = inCondition + userRegistri[i].systemId;

                if (i < userRegistri.Length - 1)
                {
                    inCondition = inCondition + " , ";
                }
            }
            //inCondition = inCondition + " )";

            return inCondition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //		protected void Butt_ricerca_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        //		{
        //			
        //		
        //		SearchDocuments();
        //
        //		}

        private void Page_Load(object sender, System.EventArgs e)
        {
            //			// Put user code to initialize the page here
            //			userHome=(DocsPaVO.utente.utente) Session["userData"];
            //			userRuolo = (DocsPaVO.utente.ruolo) Session["userRuolo"];
            //			//chiamo il ws della ricerca

            Utils.startUp(this);

            if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Document)
            {
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["gridper"] != string.Empty && Request.QueryString["gridper"] != null)
                {
                    change_from_grid = true;
                }
                else
                {
                    change_from_grid = false;
                }
            }

            if (Request.QueryString["numRes"] != string.Empty && Request.QueryString["numRes"] != null)
            {
                this.numResult = Request.QueryString["numRes"];
            }
            else
            {
                this.numResult = string.Empty;
            }

            schedaRicerca = (SchedaRicerca)Session[SchedaRicerca.SESSION_KEY];
            if (schedaRicerca == null)
            {
                //Inizializzazione della scheda di ricerca per la gestione delle 
                //ricerche salvate
                DocsPaWR.Utente utente = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
                DocsPaWR.Ruolo ruolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];

                schedaRicerca = new SchedaRicerca(KEY_SCHEDA_RICERCA, utente, ruolo, this);
                Session[SchedaRicerca.SESSION_KEY] = schedaRicerca;
            }
            schedaRicerca.Pagina = this;
            if (!IsPostBack)
            {
                DocsPAWA.DocsPaWR.InfoUtente info = new DocsPAWA.DocsPaWR.InfoUtente();
                info = UserManager.getInfoUtente(this.Page);


                string valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_OGGETTO");
                if (!string.IsNullOrEmpty(valoreChiave))
                    caratteriDisponibili = int.Parse(valoreChiave);

                txt_oggetto.MaxLength = caratteriDisponibili;
                clTesto.Value = caratteriDisponibili.ToString();
                txt_oggetto.Attributes.Add("onKeyUp", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','DESCRIZIONE'," + clTesto.ClientID + ")");
                txt_oggetto.Attributes.Add("onchange", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','DESCRIZIONE'," + clTesto.ClientID + ")");
                //verifica se nuova ADL
                if ((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1") && (!IsPostBack))
                {
                    schedaRicerca.ElencoRicercheADL("D", true, ddl_Ric_Salvate, null);
                }
                else
                {
                    schedaRicerca.ElencoRicerche("D", true, ddl_Ric_Salvate);
                }
                //focus sul campo oggetto 
                this.SetControlFocus(this.txt_oggetto.ClientID);

                // Visualizzazione pagina di ricerca nella selezione 
                // di un criterio di ricerca salvato
                this.ddl_Ric_Salvate.Attributes.Add("onChange", "OnChangeSavedFilter();");

                // inizializzazione controlli di ricerca
                this.InitSearchOptions();

                // Impostazione messaggio per ricerca fulltext
                this.SetFullTextAlertMessage();

                #region filtro: Limitazione risultati ricerca

                this.droplistLimitaRisultatiRicerca.Visible = true;
                this.droplistLimitaRisultatiRicerca.SelectedIndex = 0;

                DocsPaWR.Configurazione limitaRisultati;
                if (DocsPAWA.UserManager.getParametroConfigurazione(this.Page) != null)
                {
                    limitaRisultati = DocsPAWA.UserManager.getParametroConfigurazione(this.Page);
                    if (limitaRisultati.valore.Equals("0")) this.droplistLimitaRisultatiRicerca.Visible = false;
                }

                #endregion
            }
            tastoInvio();
            //new ADL
            if ((!IsPostBack) &&
                (Request.QueryString["ricADL"] != null) &&
                (Request.QueryString["ricADL"] == "1") &&
                SiteNavigation.CallContextStack.CurrentContext.SessionState.Count > 0 &&
                SiteNavigation.CallContextStack.CurrentContext.SessionState["SchedaRicerca"] == null
                )
            {
                lblSearch.Text = "Ricerche Salvate Area di Lavoro";
                this.butt_ricerca_Click(null, null);
            }

            string new_search = string.Empty;
            if (ViewState["new_search"] != null)
            {
                new_search = ViewState["new_search"] as string;
                ViewState["new_search"] = null;
            }

            if (change_from_grid && string.IsNullOrEmpty(new_search))
            {
                if (schedaRicerca != null && schedaRicerca.FiltriRicerca != null)
                {
                    PopulateField(schedaRicerca.FiltriRicerca);
                }
                change_from_grid = false;
                this.SearchDocuments();
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
            this.ddl_Ric_Salvate.SelectedIndexChanged += new System.EventHandler(this.ddl_Ric_Salvate_SelectedIndexChanged);
            this.btn_Canc_Ric.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Canc_Ric_Click);
            this.enterKeySimulator.Click += new System.Web.UI.ImageClickEventHandler(this.butt_ricerca_Click);
            this.optSearchOggetto.CheckedChanged += new System.EventHandler(this.optSearchOggetto_CheckedChanged);
            this.optSearchFullText.CheckedChanged += new System.EventHandler(this.optSearchFullText_CheckedChanged);
            this.butt_ricerca.Click += new System.EventHandler(this.butt_ricerca_Click);
            this.mb_ConfirmDelete.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.mb_ConfirmDelete_GetMessageBoxResponse);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        //		private void txt_oggetto_Load(object sender, System.EventArgs e)
        //		{
        //			if(IsPostBack)
        //			{
        //				this.SearchDocuments();
        //			}
        //		}


        /// <summary>
        /// verifica se sono stati immessi i parametri di filtro obbligatori
        /// </summary>
        /// <returns></returns>
        private bool ContainsFilterCriteria()
        {
            bool retValue = false;
            //modifica anomalia risolta il 28/09/2011 afiordi
            if (ConfigSettings.IsEnabledFullTextSearch() == "1")
            {
            if (this.optSearchOggetto.Checked)
                retValue = (this.txt_oggetto.Text.Trim() != "" || this.droplistLimitaRisultatiRicerca.SelectedValue != "0");
            else
                retValue = (this.txtTestoContenuto.Text.Trim() != "");
            }else
                retValue  = true;

            return retValue;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchDocuments()
        {
            try
            {
                this.SetPageOnCurrentContext();

                if (Session["FULL_TEXT_CONTEXT"] != null)
                    Session.Remove("FULL_TEXT_CONTEXT");

                // viene effettuata la ricerca solo se 
                // sono stati immessi i parametri di filtro obbligatori
                if (this.ContainsFilterCriteria())
                {
                    TextBox fullTextObject = null;
                    if (this.optSearchOggetto.Checked)
                        fullTextObject = this.txt_oggetto;
                    else
                        fullTextObject = this.txtTestoContenuto;

                    // Controllo lunghezza oggetto inserito
                    if (fullTextObject.Text.Trim() != string.Empty && !FullTextSearch.Configurations.CheckTextMinLenght(fullTextObject.Text))
                    {
                        string message = string.Format("alert('Immettere almeno {0} caratteri');", FullTextSearch.Configurations.FullTextMinTextLenght.ToString());
                        this.RegisterClientScript("CheckTextMinLenght", message);
                        fullTextObject.Focus();
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }

                    this.RicercaVeloce();
                }

                if (qV == null)
                {
                    this.RegisterClientScript("SearchDocumentCriteria", "alert('Inserire almeno un criterio di ricerca');");

                    if (this.optSearchOggetto.Checked)
                        this.txt_oggetto.Focus();
                    else
                        this.txtTestoContenuto.Focus();

                    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                }
                else
                {
                    Safe = new DocsPAWA.DocsPaWR.InfoUtente();
                    Safe = UserManager.getInfoUtente(this);

                    schedaRicerca.FiltriRicerca = qV;
                    DocumentManager.setFiltroRicDoc(this, qV);
                    DocumentManager.removeDatagridDocumento(this);
                    DocumentManager.removeListaDocProt(this);
                    Session.Remove("listInArea");

                    string destinationPageUrl = string.Empty;
                    if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                        destinationPageUrl = "NewTabSearchResult.aspx?ricADL=1&from=veloce&tabRes=veloce";
                    else
                        destinationPageUrl = "NewTabSearchResult.aspx?tabRes=veloce";

                    if (this.optSearchFullText.Checked)
                        destinationPageUrl += "?full_text=true";

                    ViewState["new_search"] = "true";

                    Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location = '" + destinationPageUrl + "';</script>");
                }

            }
            catch (System.Exception exception)
            {
                ErrorManager.redirect(this, exception);
            }
        }

        //		private void enterKeySimulator_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        //		{
        //			this.Butt_ricerca_Click(null,null);
        //			
        //		}

        /// <summary>
        /// Impostazione messaggio di warning per la ricerca fulltext
        /// </summary>
        private void SetFullTextAlertMessage()
        {
            string alertMessage = string.Empty;

            if (this.optSearchFullText.Checked && FullTextSearch.Configurations.FullTextAlertMessageEnabled)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("Attenzione!");
                sb.Append(Environment.NewLine + Environment.NewLine);
                sb.Append("Questa ricerca è basata sulle regole di gestione della visibilità del documentale.");
                sb.Append(Environment.NewLine);
                sb.Append("Si vedranno tutti i documenti visibili dall utente indipendentemente dal ruolo selezionato e");
                sb.Append(Environment.NewLine);
                sb.Append("sarà possibile visualizzare i dettagli dei soli documenti visibili al ruolo corrente.");

                // Reperimento numero massimo di record ristituito dalla ricerca fulltext
                // e, se impostato il limite, visualizzazione messaggio all'utente
                int maxRows = DocumentManager.FullTextSearchMaxRows(this, UserManager.getInfoUtente());
                if (maxRows > 0)
                {
                    sb.Append(Environment.NewLine + Environment.NewLine);
                    sb.Append("La ricerca è limitata ad un massimo di " + maxRows.ToString() + " documenti.");
                }
                alertMessage = sb.ToString();
            }

            this.fullTextAlertMessage.Value = alertMessage;
        }

        private void butt_ricerca_Click(object sender, EventArgs e)
        {
            this.SearchDocuments();

        }

        private void enterKeySimulator_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.SearchDocuments();
        }

        /// <summary>
        /// Gestione inizializzazione controlli grafici di ricerca
        /// in funzione dell'abilitazione o meno della ricerca fulltext
        /// </summary>
        private void InitSearchOptions()
        {
            // verifica se è attivata la ricerca fulltext da web.config
            bool canSearchFullText = (ConfigSettings.IsEnabledFullTextSearch() != "0");

            // impostazione visibilità radio buttons di ricerca
            // (solo se fulltext=true)
            this.SetVisibleSearchOptions(canSearchFullText);

            // valore di default
            this.optSearchOggetto.Checked = true;

            // abilitazione del pannello corrente
            this.SetControlsEnabled(this.optSearchOggetto.Checked);
        }

        private void SetVisibleSearchOptions(bool isVisible)
        {
            this.pnlSearchOptions.Visible = isVisible;
            this.pnlSearchFullText.Visible = isVisible;
        }

        /// <summary>
        /// Impostazione visibilità campi relativamente
        /// all'abilitazione o meno della ricerca fulltext
        /// </summary>
        private void SetControlsEnabled(bool searchOggetto)
        {
            this.pnlSearchOggetto.Visible = searchOggetto;
            this.pnlSearchFullText.Visible = !searchOggetto;
        }

        private void optSearchOggetto_CheckedChanged(object sender, System.EventArgs e)
        {
            this.SetFullTextAlertMessage();

            this.SetControlsEnabled(true);

            this.SetControlFocus(this.txt_oggetto.ClientID);
        }

        private void optSearchFullText_CheckedChanged(object sender, System.EventArgs e)
        {
            this.SetFullTextAlertMessage();

            this.SetControlsEnabled(false);

            this.SetControlFocus(this.txtTestoContenuto.ClientID);
        }

        /// <summary>
        /// Impostazione del focus su un controllo
        /// </summary>
        /// <param name="controlID"></param>
        private void SetControlFocus(string controlID)
        {
            this.RegisterClientScript("SetFocus", "SetControlFocus('" + controlID + "');");
        }

        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.ClientScript.IsStartupScriptRegistered(this.GetType(), scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.ClientScript.RegisterStartupScript(this.GetType(), scriptKey, scriptString);
            }
        }

        public void tastoInvio()
        {
            Utils.DefaultButton(this, ref txt_oggetto, ref butt_ricerca);
            Utils.DefaultButton(this, ref droplistLimitaRisultatiRicerca, ref butt_ricerca);
            Utils.DefaultButton(this, ref txtTestoContenuto, ref butt_ricerca);
        }

        protected bool PopulateField(DocsPAWA.DocsPaWR.FiltroRicerca[][] qV)
        {
            try
            {
                if (qV == null || qV.Length == 0)
                    return false;
                DocsPaWR.FiltroRicerca[] filters = qV[0];
                if (this.Session["itemUsedSearch"] != null)
                {
                    ddl_Ric_Salvate.SelectedIndex = Convert.ToInt32(this.Session["itemUsedSearch"]);
                }

                foreach (DocsPAWA.DocsPaWR.FiltroRicerca aux in filters)
                {
                    if (aux.argomento == DocsPaWR.FiltriDocumento.OGGETTO.ToString())
                    {
                        this.txt_oggetto.Text = aux.valore;
                    }

                    if (aux.argomento == DocsPaWR.FiltriDocumento.VISUALIZZA_TOP_N_DOCUMENTI.ToString())
                    {
                        this.droplistLimitaRisultatiRicerca.SelectedValue = aux.valore;
                    }
                }
                return true;
            }

            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
                return false;
            }
        }

        private void mb_ConfirmDelete_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                try
                {
                    schedaRicerca.Cancella(Int32.Parse(ddl_Ric_Salvate.SelectedValue));
                    Response.Write("<script>alert(\"I criteri di ricerca sono stati rimossi\");window.location.href = window.location.href;</script>");
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert(\"Impossibile rimuovere i criteri di ricerca. Errore: " + ex.Message + "\");window.location.href = window.location.href;</script>");
                }
            }
        }

        private void ddl_Ric_Salvate_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                string gridTempId = string.Empty;

                schedaRicerca.Seleziona(Int32.Parse(ddl_Ric_Salvate.SelectedValue), out gridTempId);

                if (!string.IsNullOrEmpty(gridTempId) && GridManager.IsRoleEnabledToUseGrids())
                {
                    schedaRicerca.gridId = gridTempId;
                    Grid tempGrid = GridManager.GetGridFromSearchId(schedaRicerca.gridId, GridTypeEnumeration.Document);
                    if (tempGrid != null)
                    {
                        GridManager.SelectedGrid = tempGrid;
                    }
                }

                qV = schedaRicerca.FiltriRicerca;

                if (ddl_Ric_Salvate.SelectedIndex > 0)
                {
                    Session.Add("itemUsedSearch", ddl_Ric_Salvate.SelectedIndex.ToString());
                }

                if (PopulateField(qV))
                {
                    DocumentManager.setFiltroRicDoc(this, qV);
                    DocumentManager.removeDatagridDocumento(this);
                    DocumentManager.removeListaDocProt(this);
                    //Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'tabRisultatiRicDoc.aspx';</script>");	
                    if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=veloce&tabRes=veloce';</script>");
                    else
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?&tabRes=veloce';</script>");
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                msg = msg.Replace("\"", "\\\"");
                Response.Write("<script>alert(\"" + msg + "\");window.location.href = window.location.href;</script>");
            }
        }

        private void btn_Canc_Ric_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (ddl_Ric_Salvate.SelectedIndex > 0)
            {
                //Chiedi conferma su popup
                string id = ddl_Ric_Salvate.SelectedValue;
                DocsPaWR.DocsPaWebService docspaws = ProxyManager.getWS();
                DocsPaWR.SearchItem item = docspaws.RecuperaRicerca(Int32.Parse(id));
                DocsPaWR.Ruolo ruolo = null;
                if (item.owner_idGruppo != 0)
                    ruolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];
                string msg = "Il criterio di ricerca con nome '" + ddl_Ric_Salvate.SelectedItem.ToString() + "' verrà rimosso.\\n";
                msg += (ruolo != null) ? "Attenzione! Il criterio di ricerca è condiviso con il ruolo '" + ruolo.descrizione + "'.\\n" : "";
                msg += "Confermi l'operazione?";
                msg = msg.Replace("\"", "\\\"");
                if (this.Session["itemUsedSearch"] != null)
                {
                    Session.Remove("itemUsedSearch");
                }
                mb_ConfirmDelete.Confirm(msg);
            }
        }

        #region Gestione CallContext

        /// <summary>
        /// Impostazione numero pagina corrente del contesto di ricerca
        /// </summary>
        private void SetPageOnCurrentContext()
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            currentContext.PageNumber = 1;
        }

        #endregion
    }
}
