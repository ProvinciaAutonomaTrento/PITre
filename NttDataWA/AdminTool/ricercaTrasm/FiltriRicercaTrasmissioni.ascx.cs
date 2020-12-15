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
using SAAdminTool.DocsPaWR;

namespace SAAdminTool.ricercaTrasm
{
    /// <summary>
    /// Usercontrol per la proposta dei filtri nella ricerca delle trasmissioni
    /// </summary>
    public partial class FiltriRicercaTrasmissioni : System.Web.UI.UserControl
    {
        /// <summary>
        /// Chiave di sessione relativa al mantenimento dei filtri in sessione immessi
        /// nei campi della UI
        /// </summary>
        public const string CURRENT_UI_FILTERS_SESSION_KEY = "FiltriRicercaTrasmissioni.UIData";

        /// <summary>
        /// 
        /// </summary>
        /// 
        // Constanti che identificano la tipologia di filtro selezionata
        private const string FILTER_TYPE_ARRIVO = "A";
        private const string FILTER_TYPE_PARTENZA = "P";
        private const string FILTER_TYPE_INTERNO = "I";
        private const string FILTER_TYPE_GRIGIO = "G";
        private const string FILTER_TYPE_PREDISPOSTO = "PR";
        private const string FILTER_TYPE_TUTTI = "T";
        private const string RANGE_FILTER_TYPE_INTERVAL = "I";
        private const string RANGE_FILTER_TYPE_SINGLE = "S";

        protected string eti_arrivo;
        protected string eti_partenza;
        protected string eti_interno;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            getLettereProtocolli();

            this.Response.Expires = -1;

            this.InitRangeFilterItems();

            if (!this.IsPostBack)
            {
                if (Session[ricercaTrasm.FiltriRicercaTrasmissioni.CURRENT_UI_FILTERS_SESSION_KEY] == null)
                {
                    CampiCheckBox();
                }

                this.AddControlsClientAttribute();

                // Caricamento dati
                this.Fetch();

                // Abilitazione / disabilitazione campi range filtri per data trasmissione
                this.EnableRangeFilterControls(this.cboTypeDataTrasmissione);

                // Abilitazione / disabilitazione campi range filtri per data scadenza trasmissione
                this.EnableRangeFilterControls(this.cboTypeDataScadenzaTrasmissione);

                // Azione di selezione del tipo di mittente rubrica
                this.PerformActionSelectTipoMittente();

                // Azione di selezione ordinamento
                this.PerformActionSelectTipoOrdinamento();

                //abilita disabilita filtro tipo documento
                //this.EnableFilterTipoDoc(TipoOggettoTrasmissione);

                if (Session["TrasmNonViste"] != null && Session["TrasmNonViste"].ToString()!= "")
                {
                    this.chkElemNonLetti.Checked = true;
                }

                
                

            }

            if (this.cboTypeDataTrasmissione.SelectedIndex == 1)
            {
                this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.Visible = true;
                this.GetCalendarControl("txtInitDataTrasmissione").btn_Cal.Visible = true;
                this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.Visible = true;
                this.GetCalendarControl("txtEndDataTrasmissione").btn_Cal.Visible = true;
            }
            else
            {
                this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.Visible = true;
                this.GetCalendarControl("txtInitDataTrasmissione").btn_Cal.Visible = true;
                this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.Visible = false;
                this.GetCalendarControl("txtEndDataTrasmissione").btn_Cal.Visible = false;
            }

            if (this.cboTypeDataScadenzaTrasmissione.SelectedIndex == 1)
            {
                this.GetCalendarControl("txtInitDataScadenzaTrasmissione").txt_Data.Visible = true;
                this.GetCalendarControl("txtInitDataScadenzaTrasmissione").btn_Cal.Visible = true;
                this.GetCalendarControl("txtEndDataScadenzaTrasmissione").txt_Data.Visible = true;
                this.GetCalendarControl("txtEndDataScadenzaTrasmissione").btn_Cal.Visible = true;
            }
            else
            {
                this.GetCalendarControl("txtInitDataScadenzaTrasmissione").txt_Data.Visible = true;
                this.GetCalendarControl("txtInitDataScadenzaTrasmissione").btn_Cal.Visible = true;
                this.GetCalendarControl("txtEndDataScadenzaTrasmissione").txt_Data.Visible = false;
                this.GetCalendarControl("txtEndDataScadenzaTrasmissione").btn_Cal.Visible = false;
            }

        }
        //private void EnableFilterTipoDoc(string tipoRicerca)
        //{
        //    this.lblTipoDocumento.Visible = (tipoRicerca == "D");
        //    this.rbListTipoDocumento.Visible = (tipoRicerca == "D");

        //}
        //
        private void FillListTipiDocumento()
        {
            this.rbListTipoDocumento.Items.AddRange(this.GetListItemsTipiDocumento());
        }
        /// <summary>
        /// Verifica se è abilitata la gestione del protocollo interno
        /// </summary>
        /// <returns></returns>
        private bool IsEnabledFiltersProtocolloInterno()
        {
            DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            return ws.IsInternalProtocolEnabled(SAAdminTool.UserManager.getInfoUtente().idAmministrazione);
        }
        private ListItem[] GetListItemsTipiDocumento()
        {
            ArrayList items = new ArrayList();
            items.Add(new ListItem(this.eti_arrivo, FILTER_TYPE_ARRIVO));
            items.Add(new ListItem(this.eti_partenza, FILTER_TYPE_PARTENZA));

            if (this.IsEnabledFiltersProtocolloInterno())
                // Abilitazione dei filtri per protocollo interno
                // solo se l'amministrazione lo prevede
                items.Add(new ListItem("Interno", FILTER_TYPE_INTERNO));

            items.Add(new ListItem("Non Protocollato", FILTER_TYPE_GRIGIO));
            items.Add(new ListItem("Predisposto", FILTER_TYPE_PREDISPOSTO));
            items.Add(new ListItem("Tutti", FILTER_TYPE_TUTTI));

            ListItem[] retValue = new ListItem[items.Count];
            items.CopyTo(retValue);
            return retValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Caricamento dati iniziali maschera
        /// </summary>
        private void Fetch()
        {
            // Caricamento ragioni trasmissione
            this.FetchRagioniTrasmissione();

            // Caricamento tipi di filtro per range
            this.FetchRangeFilterTypes();

            // Caricamento tipologie di documento
            this.FillListTipiDocumento();

            //Caricamento tipo file acquisito
            this.FetchTipoFileAcquisiti();
        }

        /// <summary>
        /// Caricamento tipi di filtro per range
        /// </summary>
        private void FetchRangeFilterTypes()
        {
            //ListItem[] filterTypes = this.GetListItemsTipiSelezione();

            // Caricamento tipi filtro per data trasmissione
            this.cboTypeDataTrasmissione.Items.AddRange(this.GetListItemsTipiSelezione());

            // Caricamento tipi filtro per data scadenza trasmissione
            this.cboTypeDataScadenzaTrasmissione.Items.AddRange(this.GetListItemsTipiSelezione());
        }

        /// <summary>
        /// Caricamento ragioni trasmissione
        /// </summary>
        private void FetchRagioniTrasmissione()
        {
            foreach (RagioneTrasmissione item in this.GetRagioniTrasmissione())
                this.cboRagioniTrasmissione.Items.Add(new ListItem(item.descrizione, item.systemId));

            if (this.cboRagioniTrasmissione.Items.Count > 0)
                this.cboRagioniTrasmissione.Items.Insert(0, new ListItem(string.Empty, string.Empty));
        }

        //Caricamento tipo file acquisito
        private void FetchTipoFileAcquisiti()
        {
            ArrayList tipoFile = new ArrayList();
            tipoFile = DocumentManager.getExtFileAcquisiti(this.Page);
            bool firmati = false;
            for (int i = 0; i < tipoFile.Count; i++)
            {
                //if (tipoFile[i].ToString().Contains("P7M"))
                //{
                //    if (!firmati)
                //    {
                //        ListItem item = new ListItem(tipoFile[i].ToString().Substring(tipoFile[i].ToString().Length - 3));
                //        this.ddl_tipoFileAcquisiti.Items.Add(item);
                //        firmati = true;
                //    }
                //}
                //else
                //{
                if (!tipoFile[i].ToString().Contains("P7M"))
                {
                    ListItem item = new ListItem(tipoFile[i].ToString());
                    this.ddl_tipoFileAcquisiti.Items.Add(item);
                }
                //}
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        private SAAdminTool.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (SAAdminTool.UserControls.Calendar)this.FindControl(controlId);
        }

        /// <summary>
        /// Reperimento ragioni trasmissione
        /// </summary>
        /// <returns></returns>
        private RagioneTrasmissione[] GetRagioniTrasmissione()
        {
            return TrasmManager.getListaRagioni(this.Page, null, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ListItem[] GetListItemsTipiSelezione()
        {
            ListItem[] items = new ListItem[2];
            items[0] = new ListItem("Valore singolo", RANGE_FILTER_TYPE_SINGLE);
            items[1] = new ListItem("Intervallo", RANGE_FILTER_TYPE_INTERVAL);
            return items;
        }

        #region Gestione abilitazione / disabilitazione campi di filtro per intervallo di dati

        private Hashtable _rangeFilterItems = null;

        private void InitRangeFilterItems()
        {
            this._rangeFilterItems = new Hashtable();

            this._rangeFilterItems.Add(this.cboTypeDataTrasmissione,
                this.CreateRangeFilterInnerHT(this.lblInitDataTrasmissione,
                this.GetCalendarControl("txtInitDataTrasmissione").txt_Data,
                this.lblEndDataTrasmissione,
                this.GetCalendarControl("txtEndDataTrasmissione").txt_Data));

            this._rangeFilterItems.Add(this.cboTypeDataScadenzaTrasmissione,
                    this.CreateRangeFilterInnerHT(this.lblInitDataScadenzaTrasmissione,
                            this.GetCalendarControl("txtInitDataScadenzaTrasmissione").txt_Data,
                            this.lblEndDataScadenzaTrasmissione,
                            this.GetCalendarControl("txtEndDataScadenzaTrasmissione").txt_Data));
        }

        private void DisposeRangeFilterItems()
        {
            this._rangeFilterItems.Clear();
            this._rangeFilterItems = null;
        }

        private Hashtable CreateRangeFilterInnerHT(Label initLabel,
            TextBox initText,
            Label endLabel,
            TextBox endText)
        {
            Hashtable retValue = new Hashtable();
            retValue.Add("INIT_LABEL", initLabel);
            retValue.Add("INIT_TEXT", initText);
            retValue.Add("END_LABEL", endLabel);
            retValue.Add("END_TEXT", endText);
            return retValue;
        }

        private void EnableRangeFilterControls(DropDownList cboFilterType)
        {
            bool intervalFilterEnabled = (cboFilterType.SelectedValue == RANGE_FILTER_TYPE_INTERVAL);
            Hashtable innerHT = (Hashtable)this._rangeFilterItems[cboFilterType];

            Label initLabel = (Label)innerHT["INIT_LABEL"];
            TextBox initText = (TextBox)innerHT["INIT_TEXT"];
            Label endLabel = (Label)innerHT["END_LABEL"];
            TextBox endText = (TextBox)innerHT["END_TEXT"];

            initLabel.Visible = intervalFilterEnabled;
            initText.Visible = true;
            endLabel.Visible = intervalFilterEnabled;
            endText.Visible = intervalFilterEnabled;
        }

        protected void cboFilterType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            EnableRangeFilterControls((DropDownList)sender);
        }

        #endregion

        #region Gestione javascript

        /// <summary>
        /// Associazione funzioni javascript agli eventi client dei controlli
        /// </summary>
        private void AddControlsClientAttribute()
        {
            this.btnShowRubrica.Attributes.Add("onClick", "ShowDialogRubrica('" + this.txtTipoCorrispondente.ClientID + "');");
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

        #endregion

        #region Gestione rubrica

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnShowRubrica_Click(object sender, ImageClickEventArgs e)
        {
            this.FillDatiCorrispondenteDaRubrica();
        }

        /// <summary>
        /// Caricamento dati corrispondente selezionato dalla rubrica
        /// </summary>
        private void FillDatiCorrispondenteDaRubrica()
        {
            DocsPaWR.Corrispondente selectedCorr = UserManager.getCorrispondenteSelezionato(this.Page);

            if (selectedCorr != null)
            {
                this.txtSystemIdUtenteMittente.Value = selectedCorr.systemId;
                this.txtCodiceUtenteMittente.Text = selectedCorr.codiceRubrica;
                this.txtDescrizioneUtenteMittente.Text = selectedCorr.descrizione;

                selectedCorr = null;

                UserManager.removeCorrispondentiSelezionati(this.Page);
            }
            else
            {
                this.txtSystemIdUtenteMittente.Value = string.Empty;
                this.txtCodiceUtenteMittente.Text = string.Empty;
                this.txtDescrizioneUtenteMittente.Text = string.Empty;
            }
        }

        protected void optListTipiMittente_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PerformActionSelectTipoMittente();
        }

        /// <summary>
        /// Azione di selezione del tipo di mittente richiesto
        /// </summary>
        private void PerformActionSelectTipoMittente()
        {
            this.txtTipoCorrispondente.Value = this.optListTipiMittente.SelectedItem.Value;
        }

        /// <summary>
        /// Azione di selezione del tipo di mittente richiesto
        /// </summary>
        private void PerformActionSelectTipoOrdinamento()
        {
            //   this.ddl_ordinamento.SelectedItem.Value;
        }

        /// <summary>
        /// Azione di selezione tipo documento
        /// </summary>
        private void PerformActionSelectTipoDocumento()
        {
            string selectedValue = this.rbListTipoDocumento.SelectedValue;


        }

        protected void chkDocumenti_CheckedChanged(object sender, System.EventArgs e)
        {
            if (chkDocumenti.Checked)
                this.rbListTipoDocumento.SelectedValue = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.rbListTipoDocumento.ClientID, FILTER_TYPE_TUTTI);
            else
                this.rbListTipoDocumento.SelectedIndex = -1;
            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.chkDocumenti.ClientID, this.chkDocumenti.Checked.ToString());
        }

        protected void chkElemNonLetti_CheckedChanged(object sender, System.EventArgs e)
        {
            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.chkElemNonLetti.ClientID, this.chkElemNonLetti.Checked.ToString());
        }

        protected void chkFasciscoli_CheckedChanged(object sender, System.EventArgs e)
        {
            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.chkFasciscoli.ClientID, this.chkFasciscoli.Checked.ToString());
        }

        protected void chkAccettazione_CheckedChanged(object sender, System.EventArgs e)
        {
            if (chkAccettazione.Checked)
                Session.Add("TrasmNonAccettate", "T");
            else
                Session.Remove("TrasmNonAccettate");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rbListTipoDocumento_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PerformActionSelectTipoDocumento();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtCodiceUtenteMittente_TextChanged(object sender, EventArgs e)
        {
            string codiceMittDest = this.txtCodiceUtenteMittente.Text;
            DocsPaWR.Corrispondente corrispondente = null;

            if (codiceMittDest != string.Empty)
            {
                // Reperimento oggetto corrispondente dal codice immesso dall'utente
                corrispondente = this.GetCorrispondenteDaCodice(codiceMittDest);

                if (corrispondente == null)
                {
                    this.RegisterClientScript("CodiceRubricaNonTrovato", "alert('Codice rubrica non trovato');");

                    UserManager.removeCorrispondentiSelezionati(this.Page);
                }
                else
                {
                    UserManager.setCorrispondenteSelezionato(this.Page, corrispondente);

                    // Impostazione del tipo corrispondente corretto
                    if (corrispondente.GetType().Equals(typeof(DocsPaWR.Utente)))
                        this.optListTipiMittente.SelectedValue = "P";
                    else if (corrispondente.GetType().Equals(typeof(DocsPaWR.Ruolo)))
                        this.optListTipiMittente.SelectedValue = "R";
                    else if (corrispondente.GetType().Equals(typeof(DocsPaWR.UnitaOrganizzativa)))
                        this.optListTipiMittente.SelectedValue = "U";
                }

                this.txtCodiceUtenteMittente.Focus();

                // Caricamento dati corrispondente
                this.FillDatiCorrispondenteDaRubrica();

                // Azione di selezione tipologia mittente
                this.PerformActionSelectTipoMittente();
            }
        }

        /// <summary>
        /// Reperimento di un corrispondente in base ad un codice rubrica fornito in ingresso
        /// </summary>
        /// <param name="page"></param>
        /// <param name="codCorrispondente"></param>
        /// <returns></returns>
        private Corrispondente GetCorrispondenteDaCodice(string codCorrispondente)
        {
            DocsPaWR.Corrispondente retValue = null;

            if (codCorrispondente != null)
                retValue = UserManager.getCorrispondente(this.Page, codCorrispondente, true);

            return retValue;
        }

        #endregion

        #region Gestione filtri

        /// <summary>
        /// Creazione oggetti filtro
        /// </summary>
        /// <returns></returns>
        public DocsPaWR.FiltroRicerca[] GetFilters()
        {
            ArrayList filterItems = new ArrayList();
            bool result = true;

            // Filtri per trasmissione non lette
            this.AddFilterElementiNonLetti(filterItems);

            // Filtri per documenti acquisiti
            this.AddFilterDocumentiAcquisiti(filterItems);

            // Filtri per documenti firmati
            this.AddFilterDocumentiFirmati(filterItems);

            //Filtri per trasmissioni in attesa di accettazione
            this.AddFilterTrasmissioniAccettate(filterItems);

            // Filtri per tipologia trasmissione
            this.AddFilterTipoOggettoTrasmissione(filterItems);

            // Filtri per ragione trasmissione
            this.AddFilterRagioneTrasmissione(filterItems);

            // Filtri per mittente trasmissione
            this.AddFilterMittente(filterItems);

            // Filtri per oggetto
            this.AddFilterOggettoDocumento(filterItems);

            // Filtri per ordinamento
            this.AddFilterOrdinamento(filterItems);

            //Filtro tipoDocumento
            this.AddFilterTipoDocumento(filterItems);

            // Filtri per data trasmissione
            result = this.AddFilterDataTrasmissioneDocumento(filterItems);

            //filtri per tipo file acquisito
            this.AddFilterTipoFileAcquisito(filterItems);

            if (result)
                // Filtri per data scadenza trasmissione
                result = this.AddFilterDataScadenzaTrasmissione(filterItems);

            if (result)
                return (DocsPaWR.FiltroRicerca[])filterItems.ToArray(typeof(DocsPaWR.FiltroRicerca));
            else
                return null;
        }

        /// <summary>
        /// Creazione oggetti di filtro per oggetto documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterTipoDocumento(ArrayList filterItems)
        {
            DocsPaWR.FiltroRicerca filterItem = new SAAdminTool.DocsPaWR.FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
            filterItem.valore = this.rbListTipoDocumento.SelectedValue;
            filterItems.Add(filterItem);
            filterItem = null;
        }
        /// <summary>
        /// Creazione oggetti di filtro per ragione trasmissione documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterRagioneTrasmissione(ArrayList filterItems)
        {
            if (this.cboRagioniTrasmissione.SelectedIndex > 0)
            {
                DocsPaWR.FiltroRicerca filterItem = new SAAdminTool.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.RAGIONE.ToString();
                filterItem.valore = this.cboRagioniTrasmissione.SelectedItem.Text;
                filterItems.Add(filterItem);
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per data trasmissione documento
        /// </summary>
        /// <param name="filterItems"></param>
        private bool AddFilterDataTrasmissioneDocumento(ArrayList filterItems)
        {
            bool result = true;
            bool rangeFilterInterval = (this.cboTypeDataTrasmissione.SelectedValue == RANGE_FILTER_TYPE_INTERVAL);

            DocsPaWR.FiltroRicerca filterItem = null;

            if (this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.Text.Length > 0)
            {
                filterItem = new SAAdminTool.DocsPaWR.FiltroRicerca();

                if (rangeFilterInterval)
                    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_SUCCESSIVA_AL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_IL.ToString();

                filterItem.valore = this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.Text.Trim();
                filterItems.Add(filterItem);
                filterItem = null;
            }

            if (rangeFilterInterval && this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.Text.Length > 0)
            {
                filterItem = new SAAdminTool.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_PRECEDENTE_IL.ToString();
                filterItem.valore = this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.Text.Trim();
                filterItems.Add(filterItem);
                filterItem = null;
            }

            if (this.cboTypeDataTrasmissione.SelectedIndex > 0 && !this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.Text.Equals("") && !this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.Text.Equals(""))
            {
                if (Utils.verificaIntervalloDate(this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.Text, this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.Text))
                {
                    Response.Write("<script>alert('Verificare valori inseriti per Data Trasmissione!');</script>");
                    return false;
                }
            }
            return result;
        }

        /// <summary>
        /// Creazione oggetti di filtro per data scadenza trasmissione documento
        /// </summary>
        /// <param name="filterItems"></param>
        private bool AddFilterDataScadenzaTrasmissione(ArrayList filterItems)
        {
            bool result = true;
            bool rangeFilterInterval = (this.cboTypeDataScadenzaTrasmissione.SelectedValue == RANGE_FILTER_TYPE_INTERVAL);

            DocsPaWR.FiltroRicerca filterItem = null;

            if (this.GetCalendarControl("txtInitDataScadenzaTrasmissione").txt_Data.Text.Length > 0)
            {
                filterItem = new SAAdminTool.DocsPaWR.FiltroRicerca();

                if (rangeFilterInterval)
                    filterItem.argomento = DocsPaWR.FiltriTrasmissione.SCADENZA_SUCCESSIVA_AL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriTrasmissione.SCADENZA_PRECEDENTE_IL.ToString();

                filterItem.valore = this.GetCalendarControl("txtInitDataScadenzaTrasmissione").txt_Data.Text.Trim();
                filterItems.Add(filterItem);
                filterItem = null;
            }

            if (rangeFilterInterval && this.GetCalendarControl("txtEndDataScadenzaTrasmissione").txt_Data.Text.Length > 0)
            {
                filterItem = new SAAdminTool.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriTrasmissione.SCADENZA_PRECEDENTE_IL.ToString();
                filterItem.valore = this.GetCalendarControl("txtEndDataScadenzaTrasmissione").txt_Data.Text.Trim();
                filterItems.Add(filterItem);
                filterItem = null;
            }

            if (this.cboTypeDataScadenzaTrasmissione.SelectedIndex > 0 && !this.GetCalendarControl("txtInitDataScadenzaTrasmissione").txt_Data.Text.Equals("") && !this.GetCalendarControl("txtEndDataScadenzaTrasmissione").txt_Data.Text.Equals(""))
            {
                if (Utils.verificaIntervalloDate(this.GetCalendarControl("txtInitDataScadenzaTrasmissione").txt_Data.Text, this.GetCalendarControl("txtEndDataScadenzaTrasmissione").txt_Data.Text))
                {
                    Response.Write("<script>alert('Verificare valori inseriti per Data Scadenza Trasmissione!');</script>");
                    return false;
                }
            }
            return result;
        }

        /// <summary>
        /// Creazione filtro su mittente trasmissione
        /// </summary>
        /// <param name="filterItem"></param>
        private void AddFilterMittente(ArrayList filterItem)
        {
            FiltroRicerca filter = null;

            if (!string.IsNullOrEmpty(this.txtCodiceUtenteMittente.Text))
            {
                filter = new FiltroRicerca();
                filter.argomento = this.GetArgomento(true, this.optListTipiMittente.SelectedValue);
                if (!this.optListTipiMittente.SelectedValue.Equals("U"))
                    filter.valore = this.txtCodiceUtenteMittente.Text.Trim();
                else
                    filter.valore = this.txtSystemIdUtenteMittente.Value.Trim();
            }
            else if (!string.IsNullOrEmpty(this.txtDescrizioneUtenteMittente.Text))
            {
                filter = new SAAdminTool.DocsPaWR.FiltroRicerca();
                filter.argomento = this.GetArgomento(false, this.optListTipiMittente.SelectedValue);
                filter.valore = this.txtDescrizioneUtenteMittente.Text.Trim();
            }

            if (filter != null)
                filterItem.Add(filter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codRubrica"></param>
        /// <param name="tipoCorr"></param>
        /// <returns></returns>
        private string GetArgomento(bool codRubrica, string tipoCorr)
        {
            string argomento = string.Empty;

            switch (tipoCorr)
            {
                case "R":
                    if (codRubrica)
                        argomento = SAAdminTool.DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_MITT_RUOLO.ToString();
                    else
                        argomento = SAAdminTool.DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_RUOLO.ToString();
                    break;

                case "P":
                    if (codRubrica)
                        argomento = SAAdminTool.DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_MITT_UTENTE.ToString();
                    else
                        argomento = SAAdminTool.DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_UTENTE.ToString();
                    break;

                case "U":
                    if (codRubrica)
                        argomento = SAAdminTool.DocsPaWR.FiltriTrasmissioneNascosti.ID_UO_MITT.ToString();
                    else
                        argomento = SAAdminTool.DocsPaWR.FiltriTrasmissioneNascosti.MITTENTE_UO.ToString();
                    break;
            }

            return argomento;
        }

        /// <summary>
        /// 
        /// </summary>
        //private string TipoOggettoTrasmissione
        //{
        //    get
        //    {
        //        string tipoOggetto = this.Request.QueryString["tipoOggetto"];
        //        if (string.IsNullOrEmpty(tipoOggetto))
        //            tipoOggetto = "D";
        //        return tipoOggetto;
        //    }
        //}

        /// <summary>
        /// Creazione oggetti di filtro per le trasmissione non ancora lette
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterElementiNonLetti(ArrayList filterItems)
        {
            if (this.chkElemNonLetti.Checked)
            {
                DocsPaWR.FiltroRicerca filterItem = new SAAdminTool.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.ELEMENTI_NON_VISTI.ToString();
                filterItem.valore = "1";
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per documenti acquisiti
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterDocumentiAcquisiti(ArrayList filterItems)
        {
            if (this.chkAcquisiti.Checked)
            {
                DocsPaWR.FiltroRicerca filterItem = new SAAdminTool.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DOCUMENTI_ACQUISITI.ToString();
                filterItem.valore = "1";
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per documenti firmati
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterDocumentiFirmati(ArrayList filterItems)
        {
            if (this.chkFirmati.Checked)
            {
                if (!this.cb_nonFirmato.Checked)
                {
                    DocsPaWR.FiltroRicerca filterItem = new SAAdminTool.DocsPaWR.FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DOCUMENTI_FIRMATI.ToString();
                    filterItem.valore = "1";
                    filterItems.Add(filterItem);
                    filterItem = null;
                }
                else //se sono entrambi selezionati cerco i documenti che abbiano un file acquisito, siano essi firmati o meno.
                {
                    DocsPaWR.FiltroRicerca filterItem = new SAAdminTool.DocsPaWR.FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DOCUMENTI_FIRMATI.ToString();
                    filterItem.valore = "2";
                    filterItems.Add(filterItem);
                    filterItem = null;
                }
            }
            else
            {
                if (this.cb_nonFirmato.Checked)
                {
                    DocsPaWR.FiltroRicerca filterItem = new SAAdminTool.DocsPaWR.FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DOCUMENTI_FIRMATI.ToString();
                    filterItem.valore = "0";
                    filterItems.Add(filterItem);
                    filterItem = null;
                }
            }
        }

        //Creazione oggetti di filtro per tipo file acquisito

        private void AddFilterTipoFileAcquisito(ArrayList filterItems)
        {
            if (this.ddl_tipoFileAcquisiti.SelectedIndex > 0)
            {
                DocsPaWR.FiltroRicerca filterItem = new SAAdminTool.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.TIPO_FILE_ACQUISITO.ToString();
                filterItem.valore = this.ddl_tipoFileAcquisiti.SelectedItem.Value;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per trasmissioni in attesa di accettazione
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterTrasmissioniAccettate(ArrayList filterItems)
        {
            if (this.chkAccettazione.Checked)
            {
                Session.Add("TrasmNonAccettate", "T");
                DocsPaWR.FiltroRicerca filterItem = new SAAdminTool.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONI_ACCETTATE.ToString();
                filterItem.valore = "1";
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }


        /// <summary>
        /// Creazione oggetti di filtro per oggetto trasmissione
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterTipoOggettoTrasmissione(ArrayList filterItems)
        {
            //if (!string.IsNullOrEmpty(this.TipoOggettoTrasmissione))
            //{
            DocsPaWR.FiltroRicerca filterItem = new SAAdminTool.DocsPaWR.FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString();
            if (this.chkDocumenti.Checked && !this.chkFasciscoli.Checked)
                filterItem.valore = "D";
            if (this.chkFasciscoli.Checked && !this.chkDocumenti.Checked)
                filterItem.valore = "F";
            if (this.chkFasciscoli.Checked && this.chkDocumenti.Checked)
                filterItem.valore = "T";
            filterItems.Add(filterItem);
            filterItem = null;

            //}
        }

        /// <summary>
        /// Creazione oggetti di filtro per oggetto documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterOggettoDocumento(ArrayList filterItems)
        {
            if (!string.IsNullOrEmpty(this.txtOggetto.Text))
            {
                DocsPaWR.FiltroRicerca filterItem = new SAAdminTool.DocsPaWR.FiltroRicerca();

                if (this.chkDocumenti.Checked && !this.chkFasciscoli.Checked)
                {
                    //if (this.TipoOggettoTrasmissione.Equals("D"))
                    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.OGGETTO_DOCUMENTO_TRASMESSO.ToString();
                    filterItem.valore = this.txtOggetto.Text.Trim();
                    filterItems.Add(filterItem);
                    filterItem = null;
                }
                //else if (this.TipoOggettoTrasmissione.Equals("F"))
                if (this.chkFasciscoli.Checked && !this.chkDocumenti.Checked)
                {
                    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.OGGETTO_FASCICOLO_TRASMESSO.ToString();
                    filterItem.valore = this.txtOggetto.Text.Trim();
                    filterItems.Add(filterItem);
                    filterItem = null;
                }
                if (this.chkDocumenti.Checked && this.chkFasciscoli.Checked)
                {
                    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.OGGETTO_DOCUMENTO_TRASMESSO.ToString();
                    filterItem.valore = this.txtOggetto.Text.Trim();
                    filterItems.Add(filterItem);
                    filterItem = null;
                    DocsPaWR.FiltroRicerca filterItem2 = new SAAdminTool.DocsPaWR.FiltroRicerca();
                    filterItem2.argomento = DocsPaWR.FiltriTrasmissioneNascosti.OGGETTO_FASCICOLO_TRASMESSO.ToString();
                    filterItem2.valore = this.txtOggetto.Text.Trim();
                    filterItems.Add(filterItem2);
                    filterItem2 = null;
                }
                
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per ordinamento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterOrdinamento(ArrayList filterItems)
        {
            if (this.ddl_ordinamento.SelectedItem != null)
            {
                DocsPaWR.FiltroRicerca filterItem = new SAAdminTool.DocsPaWR.FiltroRicerca();

                if (this.ddl_ordinamento.SelectedItem.Value.Equals("DTA_INVIO_DESC"))
                    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DTA_INVIO_DESC.ToString();
                else if (this.ddl_ordinamento.SelectedItem.Value.Equals("DTA_SCAD_DESC"))
                    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DTA_SCAD_DESC.ToString();
                else if (this.ddl_ordinamento.SelectedItem.Value.Equals("DTA_SCAD_ASC"))
                    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DTA_SCAD_ASC.ToString();
                else if (this.ddl_ordinamento.SelectedItem.Value.Equals("DTA_INVIO_ASC"))
                    filterItem.argomento = DocsPaWR.FiltriTrasmissioneNascosti.DTA_INVIO_ASC.ToString();

                filterItem.valore = this.ddl_ordinamento.SelectedItem.Value;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        #endregion

        #region Gestione memorizzazione / ripristino dati di filtro in sessione per la UI

        /// <summary>
        /// Rimozione filtri in sessione
        /// </summary>
        public static void RemoveCurrentFilters()
        {
            if (HttpContext.Current.Session[ricercaTrasm.FiltriRicercaTrasmissioni.CURRENT_UI_FILTERS_SESSION_KEY] != null)
                HttpContext.Current.Session.Remove(ricercaTrasm.FiltriRicercaTrasmissioni.CURRENT_UI_FILTERS_SESSION_KEY);
        }

        /// <summary>
        /// Persistenza filtri immessi nei campi della UI
        /// </summary>
        public void PersistCurrentFilters()
        {
            if (this.chkDocumenti.Checked)
                UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.chkDocumenti.ClientID, this.chkDocumenti.Checked.ToString());

            if (this.chkFasciscoli.Checked)
                UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.chkFasciscoli.ClientID, this.chkFasciscoli.Checked.ToString());

            if (this.chkElemNonLetti.Checked)
                UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.chkElemNonLetti.ClientID, this.chkElemNonLetti.Checked.ToString());

            //if (this.chkAcquisiti.Checked)
                UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.chkAcquisiti.ClientID, this.chkAcquisiti.Checked.ToString());

           // if (this.chkFirmati.Checked)
                UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.chkFirmati.ClientID, this.chkFirmati.Checked.ToString());

            //if(this.cb_nonFirmato.Checked)
                UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.cb_nonFirmato.ClientID, this.cb_nonFirmato.Checked.ToString());

            if (this.chkAccettazione.Checked)
                UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.chkAccettazione.ClientID, this.chkAccettazione.Checked.ToString());

            if (this.rbListTipoDocumento.SelectedIndex > -1)
                UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.rbListTipoDocumento.ClientID, this.rbListTipoDocumento.SelectedItem.Value);

            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.cboRagioniTrasmissione.ClientID, this.cboRagioniTrasmissione.SelectedItem.Value);

            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.cboTypeDataTrasmissione.ClientID, this.cboTypeDataTrasmissione.SelectedItem.Value);
            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.ClientID, this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.Text);
            if (this.cboTypeDataTrasmissione.SelectedItem.Value == RANGE_FILTER_TYPE_INTERVAL)
                UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.ClientID, this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.Text);

            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.cboTypeDataScadenzaTrasmissione.ClientID, this.cboTypeDataScadenzaTrasmissione.SelectedItem.Value);
            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.GetCalendarControl("txtInitDataScadenzaTrasmissione").txt_Data.ClientID, this.GetCalendarControl("txtInitDataScadenzaTrasmissione").txt_Data.Text);
            if (this.cboTypeDataScadenzaTrasmissione.SelectedItem.Value == RANGE_FILTER_TYPE_INTERVAL)
                UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.GetCalendarControl("txtEndDataScadenzaTrasmissione").txt_Data.ClientID, this.GetCalendarControl("txtEndDataScadenzaTrasmissione").txt_Data.Text);

            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.optListTipiMittente.ClientID, this.optListTipiMittente.SelectedItem.Value);
            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.txtSystemIdUtenteMittente.ClientID, this.txtSystemIdUtenteMittente.Value);
            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.txtCodiceUtenteMittente.ClientID, this.txtCodiceUtenteMittente.Text);
            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.txtDescrizioneUtenteMittente.ClientID, this.txtDescrizioneUtenteMittente.Text);

            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.txtOggetto.ClientID, this.txtOggetto.Text);

            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.ddl_ordinamento.ClientID, this.ddl_ordinamento.SelectedItem.Value);
            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.ddl_tipoFileAcquisiti.ClientID, this.ddl_tipoFileAcquisiti.SelectedItem.Value);
        }

        /// <summary>
        /// Ripristino filtri nei campi della UI
        /// </summary>
        public void RestoreCurrentFilters()
        {
            string valore = "";
            valore = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.chkDocumenti.ClientID);
            if (!string.IsNullOrEmpty(valore))
                if (valore.ToUpper() == "TRUE")
                    this.chkDocumenti.Checked = true;
            valore = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.chkFasciscoli.ClientID);
            if (!string.IsNullOrEmpty(valore))
                if (valore.ToUpper() == "TRUE")
                    this.chkFasciscoli.Checked = true;
            valore = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.chkElemNonLetti.ClientID);
            if (!string.IsNullOrEmpty(valore))
                if (valore.ToUpper() == "TRUE")
                    this.chkElemNonLetti.Checked = true;
            valore = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.chkAcquisiti.ClientID);
            if (!string.IsNullOrEmpty(valore))
                if (valore.ToUpper() == "TRUE")
                    this.chkAcquisiti.Checked = true;
            valore = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.chkFirmati.ClientID);
            if (!string.IsNullOrEmpty(valore))
                if (valore.ToUpper() == "TRUE")
                    this.chkFirmati.Checked = true;
            valore = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.cb_nonFirmato.ClientID);
            if (!string.IsNullOrEmpty(valore))
                if (valore.ToUpper() == "TRUE")
                    this.cb_nonFirmato.Checked = true;

            this.rbListTipoDocumento.SelectedValue = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.rbListTipoDocumento.ClientID, FILTER_TYPE_TUTTI);
            if (Session["TrasmDocPredisposti"] != null && Convert.ToBoolean(Session["TrasmDocPredisposti"]))
            {
                this.rbListTipoDocumento.SelectedValue = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.rbListTipoDocumento.ClientID, FILTER_TYPE_PREDISPOSTO);
            }
            else
                this.rbListTipoDocumento.SelectedValue = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.rbListTipoDocumento.ClientID, FILTER_TYPE_TUTTI);
            if (Session["TrasmNonAccettate"] != null && Session["TrasmNonAccettate"] != "")
            {
                this.chkAccettazione.Checked = true;
            }
            this.cboRagioniTrasmissione.SelectedValue = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.cboRagioniTrasmissione.ClientID);

            this.cboTypeDataTrasmissione.SelectedValue = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.cboTypeDataTrasmissione.ClientID);
            this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.Text = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.GetCalendarControl("txtInitDataTrasmissione").txt_Data.ClientID);
            if (this.cboTypeDataTrasmissione.SelectedItem.Value == RANGE_FILTER_TYPE_INTERVAL)
                this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.Text = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.GetCalendarControl("txtEndDataTrasmissione").txt_Data.ClientID);
            this.EnableRangeFilterControls(this.cboTypeDataTrasmissione);

            this.cboTypeDataScadenzaTrasmissione.SelectedValue = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.cboTypeDataScadenzaTrasmissione.ClientID);
            this.GetCalendarControl("txtInitDataScadenzaTrasmissione").txt_Data.Text = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.GetCalendarControl("txtInitDataScadenzaTrasmissione").txt_Data.ClientID);
            if (this.cboTypeDataScadenzaTrasmissione.SelectedItem.Value == RANGE_FILTER_TYPE_INTERVAL)
                this.GetCalendarControl("txtEndDataScadenzaTrasmissione").txt_Data.Text = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.GetCalendarControl("txtEndDataScadenzaTrasmissione").txt_Data.ClientID);
            this.EnableRangeFilterControls(this.cboTypeDataScadenzaTrasmissione);

            if (UIFiltersDataStorage.ContainsData(CURRENT_UI_FILTERS_SESSION_KEY, this.optListTipiMittente.ClientID))
                this.optListTipiMittente.SelectedValue = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.optListTipiMittente.ClientID);
            this.txtSystemIdUtenteMittente.Value = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.txtSystemIdUtenteMittente.ClientID);
            this.txtCodiceUtenteMittente.Text = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.txtCodiceUtenteMittente.ClientID);
            this.txtDescrizioneUtenteMittente.Text = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.txtDescrizioneUtenteMittente.ClientID);
            this.ddl_tipoFileAcquisiti.SelectedValue = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.ddl_tipoFileAcquisiti.ClientID);

            this.txtOggetto.Text = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.txtOggetto.ClientID);
            this.ddl_ordinamento.ClearSelection();
            string ddlValue = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.ddl_ordinamento.ClientID);

            if (ddlValue != null && ddlValue != "")
            {
                for (int i = 0; i < this.ddl_ordinamento.Items.Count; i++)
                {
                    if (this.ddl_ordinamento.Items[i].Value.ToLower().Equals(ddlValue.ToLower()))
                        this.ddl_ordinamento.Items[i].Selected = true;

                }
            }
        }


        public void CampiCheckBox()
        {
            if (Session["TrasmNonViste"] == null)
            {
                this.chkFasciscoli.Checked = true;
                this.chkDocumenti.Checked = true;
            }
            else
            {
                if (Session["TrasmNonViste"].ToString() == "T")
                {
                    this.chkFasciscoli.Checked = true;
                    this.chkDocumenti.Checked = true;
                }
                if (Session["TrasmNonViste"].ToString() == "F")
                {
                    this.chkFasciscoli.Checked = true;
                    this.chkDocumenti.Checked = false;
                }
                if (Session["TrasmNonViste"].ToString() == "D")
                {
                    this.chkFasciscoli.Checked = false;
                    this.chkDocumenti.Checked = true;
                }
            }
            if (Session["TrasmDocPredisposti"] != null)
            {
                this.chkFasciscoli.Checked = false;
                this.chkDocumenti.Checked = true;
            }
            if (Session["TrasmNonAccettate"] != null)
            {
                this.chkFasciscoli.Checked = false;
                this.chkDocumenti.Checked = false;
            }
        }

        #endregion

        /// <summary>
        /// Classe per la memorizzazione dei dati della UI in sessione
        /// </summary>
        protected sealed class UIFiltersDataStorage
        {
            private UIFiltersDataStorage()
            {
            }

            public static void SetData(string sessionKey, string controlID, string controlValue)
            {
                Hashtable ht = HttpContext.Current.Session[sessionKey] as Hashtable;

                if (ht == null)
                    ht = Hashtable.Synchronized(new Hashtable());

                if (ht.ContainsKey(controlID))
                    ht[controlID] = controlValue;
                else
                    ht.Add(controlID, controlValue);

                HttpContext.Current.Session[sessionKey] = ht;
            }

            public static bool ContainsData(string sessionKey, string controlID)
            {
                Hashtable ht = HttpContext.Current.Session[sessionKey] as Hashtable;
                return (ht != null && ht.ContainsKey(controlID));
            }

            public static string GetData(string sessionKey, string controlID, string defaultValue)
            {
                string retValue = GetData(sessionKey, controlID);

                if (string.IsNullOrEmpty(retValue))
                    retValue = defaultValue;

                return retValue;
            }

            public static string GetData(string sessionKey, string controlID)
            {
                string retValue = null;

                if (ContainsData(sessionKey, controlID))
                    retValue = (string)((Hashtable)HttpContext.Current.Session[sessionKey])[controlID];

                return retValue;
            }

            public static void RemoveData(string sessionKey, string controlID)
            {
                if (ContainsData(sessionKey, controlID))
                    ((Hashtable)HttpContext.Current.Session[sessionKey]).Remove(controlID);
            }
        }

        //INSERITA DA FABIO (Prendi etichette protocolli)
        private void getLettereProtocolli()
        {
            DocsPaWR.Corrispondente cr = (SAAdminTool.DocsPaWR.Corrispondente)this.Session["userData"];
            string idAmm = cr.idAmministrazione;
            SAAdminTool.DocsPaWR.EtichettaInfo[] etichette;
            SAAdminTool.DocsPaWR.InfoUtente infoUtente = new SAAdminTool.DocsPaWR.InfoUtente();
            SAAdminTool.DocsPaWR.DocsPaWebService wws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            etichette = wws.getEtichetteDocumenti(infoUtente, idAmm);
            this.eti_arrivo = etichette[0].Etichetta; //Valore A
            this.eti_partenza = etichette[1].Etichetta; //Valore P
            this.eti_interno = etichette[2].Etichetta;//Valore I
        }


    }
}
