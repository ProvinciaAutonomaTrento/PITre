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
using System.Linq;

namespace DocsPAWA.ricercaDoc
{
    /// <summary>
    /// Summary description for FiltriRicercaDocumenti.
    /// </summary>
    public class FiltriRicercaDocumenti : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.Label lblIDDocumento;
        protected System.Web.UI.HtmlControls.HtmlGenericControl pnlContainer;
        protected System.Web.UI.WebControls.DropDownList cboTypeIDDocumento;
        protected System.Web.UI.WebControls.TextBox txtInitIDDocumento;
        protected System.Web.UI.WebControls.TextBox txtEndIDDocumento;
        protected System.Web.UI.WebControls.DropDownList cboTypeDataCreazione;
        //protected DocsPaWebCtrlLibrary.DateMask txtInitDataCreazione;
        //protected DocsPaWebCtrlLibrary.DateMask txtEndDataCreazione;
        protected DocsPAWA.UserControls.Calendar txtInitDataCreazione;
        protected DocsPAWA.UserControls.Calendar txtEndDataCreazione;
        protected System.Web.UI.WebControls.Label lblInitIDDocumento;
        protected System.Web.UI.WebControls.Label lblEndIDDocumento;
        protected System.Web.UI.WebControls.DropDownList cboTypeNumProtocollo;
        protected System.Web.UI.WebControls.TextBox txtInitNumProtocollo;
        protected System.Web.UI.WebControls.TextBox txtEndNumProtocollo;
        protected System.Web.UI.WebControls.DropDownList cboTypeDataProtocollo;
        //protected DocsPaWebCtrlLibrary.DateMask txtInitDataProtocollo;
        //protected DocsPaWebCtrlLibrary.DateMask txtEndDataProtocollo;
        protected DocsPAWA.UserControls.Calendar txtInitDataProtocollo;
        protected DocsPAWA.UserControls.Calendar txtEndDataProtocollo;
        protected System.Web.UI.WebControls.TextBox txtOggetto;
        protected System.Web.UI.WebControls.TextBox txtCodMittDest;
        protected System.Web.UI.WebControls.TextBox txtDescrMittDest;
        protected System.Web.UI.WebControls.Button btnOK;
        protected System.Web.UI.WebControls.Button btnClose;
        protected System.Web.UI.HtmlControls.HtmlTable tblButtonsContainer;
        protected System.Web.UI.HtmlControls.HtmlTable tblContainer;
        protected System.Web.UI.HtmlControls.HtmlTable tblTipoDocumento;
        protected System.Web.UI.HtmlControls.HtmlTable tblIDDocumento;
        protected System.Web.UI.HtmlControls.HtmlTable tblDataCreazioneDocumento;
        protected System.Web.UI.HtmlControls.HtmlTable tblNumeroProtocollo;
        protected System.Web.UI.HtmlControls.HtmlTable tblDataProtocollo;
        protected System.Web.UI.HtmlControls.HtmlTable tblOggetto;
        protected System.Web.UI.HtmlControls.HtmlTable tblMittenteDestinatario;
        protected System.Web.UI.HtmlControls.HtmlTable tblTipologia;
        protected System.Web.UI.WebControls.Label lblTipoDocumento;
        protected System.Web.UI.WebControls.Label lblDataCreazione;
        protected System.Web.UI.WebControls.Label lblInitDataCreazione;
        protected System.Web.UI.WebControls.Label lblEndDataCreazione;
        protected System.Web.UI.WebControls.Label lblNumeroProtocollo;
        protected System.Web.UI.WebControls.Label lblInitNumProtocollo;
        protected System.Web.UI.WebControls.Label lblEndNumProtocollo;
        protected System.Web.UI.WebControls.Label lblDataProtocollo;
        protected System.Web.UI.WebControls.Label lblInitDataProtocollo;
        protected System.Web.UI.WebControls.Label lblEndDataProtocollo;
        protected System.Web.UI.WebControls.Label lblOggetto;
        protected System.Web.UI.WebControls.Label lblMittDest;
        protected System.Web.UI.WebControls.Label lblAnnoProtocollo;
        protected System.Web.UI.WebControls.TextBox txtAnnoProtocollo;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtSystemIDMittDest;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtRubricaCallTypeCorrInt;
        protected System.Web.UI.WebControls.Image btnRubrica;
        protected System.Web.UI.WebControls.RadioButtonList rbListTipoDocumento;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoDoc;
        protected System.Web.UI.WebControls.ImageButton btn_CampiPersonalizzati;
        DocsPAWA.ricercaDoc.SchedaRicerca sRic = null;
        private const string KEY_SCHEDA_RICERCA = "RicercaDocInFasc";
        private string prov = string.Empty;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoFileAcquisiti;
        protected System.Web.UI.WebControls.CheckBox cb_firmato;
        protected System.Web.UI.WebControls.CheckBox cb_nonFirmato;

        protected DocsPAWA.DocsPaWR.EtichettaInfo[] etichette;

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
            this.rbListTipoDocumento.SelectedIndexChanged += new System.EventHandler(this.rbListTipoDocumento_SelectedIndexChanged);
            this.cboTypeIDDocumento.SelectedIndexChanged += new System.EventHandler(this.cboFilterType_SelectedIndexChanged);
            this.cboTypeDataCreazione.SelectedIndexChanged += new System.EventHandler(this.cboFilterType_SelectedIndexChanged);
            this.cboTypeNumProtocollo.SelectedIndexChanged += new System.EventHandler(this.cboFilterType_SelectedIndexChanged);
            this.cboTypeDataProtocollo.SelectedIndexChanged += new System.EventHandler(this.cboFilterType_SelectedIndexChanged);
            this.txtCodMittDest.TextChanged += new System.EventHandler(this.txtCodMittDest_TextChanged);
            this.ddl_tipoDoc.SelectedIndexChanged += new System.EventHandler(this.ddl_tipoDoc_SelectedIndexChanged);
            this.btn_CampiPersonalizzati.Click += new System.Web.UI.ImageClickEventHandler(this.btn_CampiPersonalizzati_Click);
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            this.Unload += new System.EventHandler(this.Page_Unload);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Page_PreRender);

        }
        #endregion

        #region Gestione javascript

        /// <summary>
        /// Associazione funzioni javascript agli eventi client dei controlli
        /// </summary>
        private void AddControlsClientAttribute()
        {
            this.txtInitIDDocumento.Attributes.Add("onKeyPress", "ValidateNumericKey();");
            this.txtEndIDDocumento.Attributes.Add("onKeyPress", "ValidateNumericKey();");

            this.txtInitNumProtocollo.Attributes.Add("onKeyPress", "ValidateNumericKey();");
            this.txtEndNumProtocollo.Attributes.Add("onKeyPress", "ValidateNumericKey();");

            this.txtAnnoProtocollo.Attributes.Add("onKeyPress", "ValidateNumericKey();");

            string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);
            if (use_new_rubrica == "1")
                this.btnRubrica.Attributes.Add("onClick", "ShowDialogRubrica();");
            else
                this.btnRubrica.Attributes.Add("onClick", "ApriRubrica('ric_C','flt_vis_fasc');");

            btnRubrica.Attributes["onMouseOver"] = "javascript:ImpostaCursore (1,'" + btnRubrica.ClientID + "');";
            btnRubrica.Attributes["onMouseOut"] = "javascript:ImpostaCursore (0,'" + btnRubrica.ClientID + "');";

            this.btnClose.Attributes.Add("onClick", "CloseWindow(false);");
        }

        /// <summary>
        /// Impostazione del focus su un controllo
        /// </summary>
        /// <param name="controlID"></param>
        private void SetControlFocus(string controlID)
        {
            this.RegisterClientScript("SetFocus", "document.frmFiltriRicercaDocumenti." + controlID + ".focus();");
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

        #endregion

        #region Caricamento e gestione dati UI

        // Constanti che identificano la tipologia di filtro selezionata
        private const string FILTER_TYPE_ARRIVO = "A";
        private const string FILTER_TYPE_PARTENZA = "P";
        private const string FILTER_TYPE_INTERNO = "I";
        private const string FILTER_TYPE_GRIGIO = "G";
        private const string FILTER_TYPE_TUTTI = "T";
        private const string RANGE_FILTER_TYPE_INTERVAL = "I";
        private const string RANGE_FILTER_TYPE_SINGLE = "S";

        /// <summary>
        /// Impostazione lunghezza massima campi testo
        /// </summary>
        private void SetFieldsMaxLenght()
        {
            if (this.txtAnnoProtocollo.Visible)
                this.txtAnnoProtocollo.MaxLength = 4;
        }

        /// <summary>
        /// Impostazione valori di default
        /// </summary>
        private void SetDefaultValues()
        {
            this.rbListTipoDocumento.SelectedValue = FILTER_TYPE_TUTTI;
        }

        /// <summary>
        /// Verifica se è abilitata la gestione del protocollo interno
        /// </summary>
        /// <returns></returns>
        private bool IsEnabledFiltersProtocolloInterno()
        {
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            return ws.IsInternalProtocolEnabled(UserManager.getInfoUtente(this).idAmministrazione);
        }

        private ListItem[] GetListItemsTipiDocumento()
        {

            ArrayList items = new ArrayList();
            if (prov.Equals("Cestino"))
            {
                items.Add(new ListItem("Pred. " + etichette[0].Etichetta, FILTER_TYPE_ARRIVO));
                items.Add(new ListItem("Pred. " + etichette[1].Etichetta, FILTER_TYPE_PARTENZA));

                if (this.IsEnabledFiltersProtocolloInterno())
                    // Abilitazione dei filtri per protocollo interno
                    // solo se l'amministrazione lo prevede
                    items.Add(new ListItem("Pred. " + etichette[2].Etichetta, FILTER_TYPE_INTERNO));

                items.Add(new ListItem("Non protocollato", FILTER_TYPE_GRIGIO));
                items.Add(new ListItem("Tutti", FILTER_TYPE_TUTTI));
            }
            else
            {
                items.Add(new ListItem(etichette[0].Etichetta, FILTER_TYPE_ARRIVO));
                items.Add(new ListItem(etichette[1].Etichetta, FILTER_TYPE_PARTENZA));

                if (this.IsEnabledFiltersProtocolloInterno())
                    // Abilitazione dei filtri per protocollo interno
                    // solo se l'amministrazione lo prevede
                    items.Add(new ListItem(etichette[2].Etichetta, FILTER_TYPE_INTERNO));

                items.Add(new ListItem("Non Protocollato", FILTER_TYPE_GRIGIO));
                items.Add(new ListItem("Tutti", FILTER_TYPE_TUTTI));
            }

            ListItem[] retValue = new ListItem[items.Count];
            items.CopyTo(retValue);
            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        private DocsPAWA.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (DocsPAWA.UserControls.Calendar)this.FindControl(controlId);
        }

        private ListItem[] GetListItemsTipiSelezione()
        {
            ListItem[] items = new ListItem[2];
            items[0] = new ListItem("Valore singolo", RANGE_FILTER_TYPE_SINGLE);
            items[1] = new ListItem("Intervallo", RANGE_FILTER_TYPE_INTERVAL);
            return items;
        }

        private void FillListTipiDocumento()
        {
            this.rbListTipoDocumento.Items.AddRange(this.GetListItemsTipiDocumento());
        }

        private void FillComboTipiSelezione()
        {
            this.cboTypeIDDocumento.Items.AddRange(this.GetListItemsTipiSelezione());
            this.cboTypeDataCreazione.Items.AddRange(this.GetListItemsTipiSelezione());
            this.cboTypeNumProtocollo.Items.AddRange(this.GetListItemsTipiSelezione());
            this.cboTypeDataProtocollo.Items.AddRange(this.GetListItemsTipiSelezione());
        }

        /// <summary>
        /// Caricamento dati corrispondente selezionato dalla rubrica
        /// </summary>
        private void FillDatiCorrispondenteDaRubrica()
        {
            DocsPaWR.Corrispondente selectedCorr = null;
            string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);
            if (use_new_rubrica == "1")
                selectedCorr = RubricaWrapper.GetCorrispondenteDaRubrica();
            else
                selectedCorr = UserManager.getCorrispondenteSelezionato(this);

            if (selectedCorr != null)
            {
                // Rimozione da sessione dell'oggetto corrispondente
                // selezionato dalla rubrica
                if (use_new_rubrica == "1")
                    RubricaWrapper.RemoveCorrispondenteDaRubrica();

                this.txtSystemIDMittDest.Value = selectedCorr.systemId;
                this.txtCodMittDest.Text = selectedCorr.codiceRubrica;
                this.txtDescrMittDest.Text = selectedCorr.descrizione;

                selectedCorr = null;
            }
            else
            {
                this.txtSystemIDMittDest.Value = string.Empty;
                this.txtCodMittDest.Text = string.Empty;
                this.txtDescrMittDest.Text = string.Empty;
            }
        }

        /// <summary>
        /// Impostazione della tipologia di ricerca in rubrica
        /// a seconda della tipologia di documento selezionato
        /// </summary>
        private void SetValueRubricaCallType()
        {
            if (this.rbListTipoDocumento.SelectedValue == FILTER_TYPE_INTERNO)
                this.txtRubricaCallTypeCorrInt.Value = "true";
            else
                this.txtRubricaCallTypeCorrInt.Value = "false";
        }

        #endregion

        #region Gestione eventi UI

        #region Gestione eventi pulsanti

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            this.ApplyFilter();
        }

        #endregion

        #region Gestione eventi pagina

        private void Page_Load(object sender, System.EventArgs e)
        {

            getLettereProtocolli();

            Response.Expires = -1;

            if (Request.QueryString["prov"] != null) ;
            prov = Request.QueryString["prov"];

            // Inizializzazione hashtable per la gestione
            // dell'abilitazione / disabilitazione campi 
            // di filtro per intervallo di dati
            this.InitRangeFilterItems();
            sRic = (DocsPAWA.ricercaDoc.SchedaRicerca)Session[DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY];
            //schedaRicerca = (SchedaRicerca)Session[SchedaRicerca.SESSION_KEY];
            if (sRic == null)
            {
                //Inizializzazione della scheda di ricerca per la gestione delle 
                //ricerche salvate
                DocsPaWR.Utente utente = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
                DocsPaWR.Ruolo ruolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];

                sRic = new SchedaRicerca(KEY_SCHEDA_RICERCA, utente, ruolo, this);
                Session[SchedaRicerca.SESSION_KEY] = sRic;
            }
            sRic.Pagina = this;

            if (!IsPostBack)
            {
                // Associazione funzioni javascript agli eventi client dei controlli
                this.AddControlsClientAttribute();

                // Caricamento drop down list per la tipologia documento
                this.CaricaComboTipologiaAtto(this.ddl_tipoDoc);

                //Caricamento drop down list per il tipo di file acquisito
                this.caricaComboTipoFileAcquisiti();

                // Caricamento tipologie di documento
                this.FillListTipiDocumento();

                // Caricamento combo tipi selezione
                this.FillComboTipiSelezione();

                // Impostazione lunghezza massima campi testo
                this.SetFieldsMaxLenght();

                // Impostazione valori di default
                this.SetDefaultValues();

                // Ripristino di eventuali filtri già memorizzati in sessione
                this.RestoreUIDataFromSession();

                // Abilitazione / disabilitazione controlli relativamente
                // alla tipologia di filtro selezionata
                this.EnableTipiFiltro(this.rbListTipoDocumento.SelectedValue);

                string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);
                if (use_new_rubrica == "1")
                    RubricaWrapper.RemoveCorrispondenteDaRubrica();
                else
                    UserManager.removeCorrispondentiSelezionati(this);
            }

            //PROFILAZIONE DINAMICA
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
            {
                verificaCampiPersonalizzati();
            }
            else
            {
                btn_CampiPersonalizzati.Visible = false;
            }
            //FINE PROFILAZIONE DINAMICA


            if (this.cboTypeDataCreazione.SelectedIndex == 1)
            {
                this.GetCalendarControl("txtInitDataCreazione").btn_Cal.Visible = true;
                this.GetCalendarControl("txtInitDataCreazione").txt_Data.Visible = true;
                this.GetCalendarControl("txtEndDataCreazione").btn_Cal.Visible = true;
                this.GetCalendarControl("txtEndDataCreazione").txt_Data.Visible = true;
            }
            else
            {
                this.GetCalendarControl("txtInitDataCreazione").btn_Cal.Visible = true;
                this.GetCalendarControl("txtInitDataCreazione").txt_Data.Visible = true;
                this.GetCalendarControl("txtEndDataCreazione").btn_Cal.Visible = false;
                this.GetCalendarControl("txtEndDataCreazione").txt_Data.Visible = false;
            }

            if (this.cboTypeDataProtocollo.SelectedIndex == 1)
            {
                this.GetCalendarControl("txtInitDataProtocollo").btn_Cal.Visible = true;
                this.GetCalendarControl("txtInitDataProtocollo").txt_Data.Visible = true;
                this.GetCalendarControl("txtEndDataProtocollo").btn_Cal.Visible = true;
                this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Visible = true;
            }
            else
            {
                this.GetCalendarControl("txtInitDataProtocollo").btn_Cal.Visible = true;
                this.GetCalendarControl("txtInitDataProtocollo").txt_Data.Visible = true;
                this.GetCalendarControl("txtEndDataProtocollo").btn_Cal.Visible = false;
                this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Visible = false;
            }

        }

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            // Caricamento dati corrispondente
            this.FillDatiCorrispondenteDaRubrica();
            //
            //			// Impostazione focus su codice mittente / destinatario
            //			this.SetControlFocus(this.txtCodMittDest.ID);

            if (!this.Page.IsClientScriptBlockRegistered("imposta_cursore"))
            {
                this.Page.RegisterClientScriptBlock("imposta_cursore",
                    "<script language=\"javascript\">\n" +
                    "function ImpostaCursore (t, ctl)\n{\n" +
                    "document.getElementById(ctl).style.cursor = (t == 0) ? 'default' : 'hand';\n" +
                    "}\n</script>\n");
            }
        }

        private void CaricaComboTipologiaAtto(DropDownList ddl)
        {
            DocsPaWR.TipologiaAtto[] listaTipologiaAtto;
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1" && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null)
            {
                listaTipologiaAtto = DocumentManager.getTipoAttoPDInsRic(this, UserManager.getInfoUtente(this).idAmministrazione, UserManager.getRuolo(this).idGruppo, "1");
            }
            else
            {
                listaTipologiaAtto = DocumentManager.getListaTipologiaAtto(this);
            }
            ddl.Items.Clear();
            ddl.Items.Add("");
            if (listaTipologiaAtto != null)
            {
                for (int i = 0; i < listaTipologiaAtto.Length; i++)
                {
                    ddl.Items.Add(listaTipologiaAtto[i].descrizione);
                    ddl.Items[i + 1].Value = listaTipologiaAtto[i].systemId;
                }
            }
        }

        private void btn_CampiPersonalizzati_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            RegisterStartupScript("Apri", "<script>apriPopupAnteprima();</script>");
        }

        private void Page_Unload(object sender, System.EventArgs e)
        {
            this.DisposeRangeFilterItems();
        }

        #endregion

        #region Gestione eventi controllo "rbListTipoDocumento"

        private void rbListTipoDocumento_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            RadioButtonList radioButtonList = (RadioButtonList)sender;
            this.EnableTipiFiltro(radioButtonList.SelectedValue);

            //			if (radioButtonList.SelectedValue==FILTER_TYPE_INTERNO)
            //			{
            //				// Rimozione da sessione dell'oggetto corrispondente
            //				// selezionato dalla rubrica
            //				RubricaWrapper.RemoveCorrispondenteDaRubrica();
            //
            //				this.txtSystemIDMittDest.Value=string.Empty;
            //				this.txtCodMittDest.Text=string.Empty;
            //				this.txtDescrMittDest.Text=string.Empty;
            //			}
        }

        #endregion

        #region Gestione eventi controllo "txtCodMittDest"

        private void txtCodMittDest_TextChanged(object sender, System.EventArgs e)
        {
            string codiceMittDest = this.txtCodMittDest.Text;
            DocsPaWR.Corrispondente corrispondente = null;

            if (codiceMittDest != string.Empty)
            {
                // Reperimento oggetto corrispondente dal codice immesso dall'utente
                corrispondente = RubricaWrapper.GetCorrispondenteDaCodice(this, codiceMittDest);

                if (corrispondente == null)
                {
                    this.RegisterClientScript("", "alert('Codice rubrica non trovato');");
                    //this.SetControlFocus(this.txtCodMittDest.ID);

                    RubricaWrapper.RemoveCorrispondenteDaRubrica();
                }
                else
                {
                    RubricaWrapper.SetCorrispondenteDaRubrica(corrispondente);
                }

                //this.SetControlFocus(this.txtCodMittDest.ID);
            }

            //			Viene effettuato nel PreRender
            //			this.FillDatiCorrispondenteDaRubrica();
        }

        #endregion

        #region Gestione eventi controllo "btnRubrica"

        private void btnRubrica_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //			// Caricamento dati corrispondente
            //			this.FillDatiCorrispondenteDaRubrica();
            //
            //			// Impostazione focus su codice mittente / destinatario
            //			this.SetControlFocus(this.txtCodMittDest.ID);
        }

        #endregion

        #endregion

        #region Gestione abilitazione / disabilitazione campi UI

        private void EnableTipiFiltro(string selectedFilterItemType)
        {

            this.tblIDDocumento.Visible = false;
            this.tblDataCreazioneDocumento.Visible = false;
            this.tblNumeroProtocollo.Visible = false;
            this.tblDataProtocollo.Visible = false;
            this.tblOggetto.Visible = false;
            this.tblMittenteDestinatario.Visible = false;
            //  this.tblTipologia.Visible = false;

            switch (selectedFilterItemType)
            {
                case FILTER_TYPE_ARRIVO:
                    if (prov.Equals("Cestino"))
                    {
                        this.tblIDDocumento.Visible = true;
                        this.tblDataCreazioneDocumento.Visible = true;
                        this.EnableRangeFilterControls(this.cboTypeIDDocumento);
                        this.EnableRangeFilterControls(this.cboTypeDataCreazione);
                        this.tblTipologia.Visible = false;
                    }
                    else
                    {
                        this.tblNumeroProtocollo.Visible = true;
                        this.tblDataProtocollo.Visible = true;
                        this.lblMittDest.Text = "Mittente:";
                        this.tblMittenteDestinatario.Visible = true;
                        this.EnableRangeFilterControls(this.cboTypeNumProtocollo);
                        this.EnableRangeFilterControls(this.cboTypeDataProtocollo);
                    }
                    this.tblOggetto.Visible = true;

                    break;

                case FILTER_TYPE_PARTENZA:
                case FILTER_TYPE_INTERNO:
                    if (prov.Equals("Cestino"))
                    {
                        this.tblIDDocumento.Visible = true;
                        this.tblDataCreazioneDocumento.Visible = true;
                        this.EnableRangeFilterControls(this.cboTypeIDDocumento);
                        this.EnableRangeFilterControls(this.cboTypeDataCreazione);
                        this.tblTipologia.Visible = false;
                    }
                    else
                    {
                        this.tblNumeroProtocollo.Visible = true;
                        this.tblDataProtocollo.Visible = true;
                        this.lblMittDest.Text = "Destinatario:";
                        this.tblMittenteDestinatario.Visible = true;

                        this.EnableRangeFilterControls(this.cboTypeNumProtocollo);
                        this.EnableRangeFilterControls(this.cboTypeDataProtocollo);
                    }
                    this.tblOggetto.Visible = true;

                    break;

                case FILTER_TYPE_GRIGIO:
                    this.tblIDDocumento.Visible = true;
                    this.tblDataCreazioneDocumento.Visible = true;
                    this.tblOggetto.Visible = true;

                    this.EnableRangeFilterControls(this.cboTypeIDDocumento);
                    this.EnableRangeFilterControls(this.cboTypeDataCreazione);

                    break;

                case FILTER_TYPE_TUTTI:
                    this.tblDataCreazioneDocumento.Visible = true;
                    this.tblOggetto.Visible = true;
                    if (prov.Equals("Cestino"))
                    {
                        this.tblIDDocumento.Visible = true;
                        this.tblTipologia.Visible = false;
                    }
                    //  this.tblTipologia.Visible = true;

                    this.EnableRangeFilterControls(this.cboTypeIDDocumento);
                    this.EnableRangeFilterControls(this.cboTypeDataCreazione);

                    break;
            }

            this.SetValueRubricaCallType();
        }


        #region Gestione abilitazione / disabilitazione campi di filtro per intervallo di dati

        private Hashtable _rangeFilterItems = null;

        private void InitRangeFilterItems()
        {
            this._rangeFilterItems = new Hashtable();

            this._rangeFilterItems.Add(this.cboTypeIDDocumento,
                this.CreateRangeFilterInnerHT(this.lblInitIDDocumento,
                this.txtInitIDDocumento,
                this.lblEndIDDocumento,
                this.txtEndIDDocumento));


            this._rangeFilterItems.Add(this.cboTypeDataProtocollo,
                this.CreateRangeFilterInnerHT(this.lblInitDataProtocollo,
                this.GetCalendarControl("txtInitDataProtocollo").txt_Data,
                this.lblEndDataProtocollo,
                this.GetCalendarControl("txtEndDataProtocollo").txt_Data));


            this._rangeFilterItems.Add(this.cboTypeNumProtocollo,
                this.CreateRangeFilterInnerHT(this.lblInitNumProtocollo,
                this.txtInitNumProtocollo,
                this.lblEndNumProtocollo,
                this.txtEndNumProtocollo));

            this._rangeFilterItems.Add(this.cboTypeDataCreazione,
                this.CreateRangeFilterInnerHT(this.lblInitDataCreazione,
                this.GetCalendarControl("txtInitDataCreazione").txt_Data,
                this.lblEndDataCreazione,
                this.GetCalendarControl("txtEndDataCreazione").txt_Data));
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

        private void cboFilterType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            EnableRangeFilterControls((DropDownList)sender);
        }

        #endregion

        #endregion

        #region ProfilazioneDinamica
        private void verificaCampiPersonalizzati()
        {
            DocsPaWR.Templates template = new DocsPAWA.DocsPaWR.Templates();
            if (!ddl_tipoDoc.SelectedValue.Equals(""))
            {
                template = (DocsPAWA.DocsPaWR.Templates)Session["templateRicerca"];
                if (Session["templateRicerca"] == null)
                {
                    template = ProfilazioneDocManager.getTemplatePerRicerca((UserManager.getInfoUtente(this)).idAmministrazione, ddl_tipoDoc.SelectedItem.Text, this);
                    Session.Add("templateRicerca", template);
                }
                if (template != null && !(ddl_tipoDoc.SelectedItem.Text.ToUpper()).Equals(template.DESCRIZIONE.ToUpper()))
                {
                    template = ProfilazioneDocManager.getTemplatePerRicerca((UserManager.getInfoUtente(this)).idAmministrazione, ddl_tipoDoc.SelectedItem.Text, this);
                    Session.Add("templateRicerca", template);
                }
            }
            if (template != null && template.SYSTEM_ID == 0)
            {
                btn_CampiPersonalizzati.Visible = false;
            }
            else
            {
                if (template != null && template.ELENCO_OGGETTI.Length != 0)
                {
                    btn_CampiPersonalizzati.Visible = true;
                }
                else
                {
                    btn_CampiPersonalizzati.Visible = false;
                }
            }
        }

        private void ddl_tipoDoc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Session.Remove("templateRicerca");
            sRic.RimuoviFiltro(DocsPAWA.DocsPaWR.FiltriDocumento.PROFILAZIONE_DINAMICA.ToString());
            verificaCampiPersonalizzati();
        }
        #endregion


        #region Validazione dati immessi

        /// <summary>
        /// Validazione dati immessi ai fini del filtro
        /// </summary>
        /// <param name="validationItems"></param>
        /// <returns></returns>
        private bool IsValidData(out ArrayList validationItems,
                                 out string firstInvalidControlID)
        {
            validationItems = new ArrayList();
            firstInvalidControlID = string.Empty;

            string currentFilterType = this.rbListTipoDocumento.SelectedValue;

            switch (currentFilterType)
            {
                case FILTER_TYPE_ARRIVO:
                case FILTER_TYPE_PARTENZA:
                    this.ValidateNumericRange("Numero protocollo",
                                                this.txtInitNumProtocollo,
                                                this.txtEndNumProtocollo,
                                                validationItems,
                                                ref firstInvalidControlID);

                    this.ValidateDateRange("Data protocollo",
                                            this.GetCalendarControl("txtInitDataProtocollo").txt_Data,
                                            this.GetCalendarControl("txtEndDataProtocollo").txt_Data,
                                            validationItems,
                                            ref firstInvalidControlID);

                    break;

                case FILTER_TYPE_GRIGIO:
                    this.ValidateNumericRange("ID documento",
                                            this.txtInitIDDocumento,
                                            this.txtEndIDDocumento,
                                            validationItems,
                                            ref firstInvalidControlID);

                    this.ValidateDateRange("Data creazione",
                                            this.GetCalendarControl("txtInitDataCreazione").txt_Data,
                                            this.GetCalendarControl("txtEndDataCreazione").txt_Data,
                                            validationItems,
                                            ref firstInvalidControlID);

                    break;

                case FILTER_TYPE_TUTTI:
                    this.ValidateDateRange("Data creazione",
                                            this.GetCalendarControl("txtInitDataCreazione").txt_Data,
                                            this.GetCalendarControl("txtEndDataCreazione").txt_Data,
                                            validationItems,
                                            ref firstInvalidControlID);

                    break;
            }

            return (validationItems.Count == 0);
        }

        /// <summary>
        /// Validazione range di dati numerici
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="initText"></param>
        /// <param name="endText"></param>
        /// <param name="validationItems"></param>
        /// <param name="firstInvalidControlID"></param>
        private void ValidateNumericRange(string fieldName,
                                            TextBox initText,
                                            TextBox endText,
                                            ArrayList validationItems,
                                            ref string firstInvalidControlID)
        {
            bool isValidInitNumber = false;
            bool isValidEndNumber = false;

            if (initText.Text.Length > 0)
            {
                isValidInitNumber = this.IsValidNumber(initText);

                if (!isValidInitNumber)
                {
                    validationItems.Add(fieldName + " non valido");

                    if (firstInvalidControlID == string.Empty)
                        firstInvalidControlID = initText.ID;
                }
            }

            if (endText.Visible && endText.Text.Length > 0)
            {
                isValidEndNumber = this.IsValidNumber(endText);

                if (!isValidEndNumber)
                {
                    validationItems.Add(fieldName + " non valido");

                    if (firstInvalidControlID == string.Empty)
                        firstInvalidControlID = endText.ID;
                }
            }

            // Validazione range di dati
            if (isValidInitNumber && isValidEndNumber &&
                int.Parse(initText.Text) > int.Parse(endText.Text))
            {
                validationItems.Add(fieldName + " iniziale maggiore di quello finale");

                if (firstInvalidControlID == string.Empty)
                    firstInvalidControlID = endText.ID;
            }

        }

        /// <summary>
        /// Validazione valore numerico
        /// </summary>
        /// <param name="numberText"></param>
        /// <returns></returns>
        private bool IsValidNumber(TextBox numberText)
        {
            bool retValue = true;

            try
            {
                int.Parse(numberText.Text);
            }
            catch
            {
                retValue = false;
            }

            return retValue;
        }

        /// <summary>
        /// Validazione range di date
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="initDate"></param>
        /// <param name="endDate"></param>
        /// <param name="validationItems"></param>
        /// <param name="firstInvalidControlID"></param>
        private void ValidateDateRange(string fieldName,
                                        DocsPaWebCtrlLibrary.DateMask initDate,
                                        DocsPaWebCtrlLibrary.DateMask endDate,
                                        ArrayList validationItems,
                                        ref string firstInvalidControlID)
        {
            bool isValidInitDate = false;
            bool isValidEndDate = false;

            if (initDate.Text.Length > 0)
            {
                // Validazione data iniziale
                isValidInitDate = this.IsValidDate(initDate);

                if (!isValidInitDate)
                {
                    validationItems.Add(fieldName + " iniziale non valida");

                    if (firstInvalidControlID == string.Empty)
                        firstInvalidControlID = initDate.ID;
                }
            }

            if (endDate.Visible && endDate.Text.Length > 0)
            {
                // Validazione data finale
                isValidEndDate = this.IsValidDate(endDate);

                if (!isValidEndDate)
                {
                    validationItems.Add(fieldName + " finale non valida");

                    if (firstInvalidControlID == string.Empty)
                        firstInvalidControlID = endDate.ID;
                }
            }

            // Validazione range di dati
            if (isValidInitDate && isValidEndDate &&
                DateTime.Parse(initDate.Text) > DateTime.Parse(endDate.Text))
            {
                validationItems.Add(fieldName + " iniziale maggiore di quella finale");

                if (firstInvalidControlID == string.Empty)
                    firstInvalidControlID = endDate.ID;
            }
        }

        /// <summary>
        /// Validazione singola data
        /// </summary>
        /// <param name="dateMask"></param>
        /// <returns></returns>
        private bool IsValidDate(DocsPaWebCtrlLibrary.DateMask dateMask)
        {
            bool retValue = false;

            if (dateMask.Text.Length > 0)
                retValue = DocsPAWA.Utils.isDate(dateMask.Text);

            return retValue;
        }

        #endregion

        #region Gestione oggetti di filtro

        /// <summary>
        /// Creazione oggetti di filtro per oggetto documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterTipoDocumento(ArrayList filterItems)
        {
            DocsPaWR.FiltroRicerca filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
            filterItem.valore = this.rbListTipoDocumento.SelectedValue;
            filterItems.Add(filterItem);
            filterItem = null;
        }

        /// <summary>
        /// Creazione oggetti di filtro per numero protocollo
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterNumProtocollo(ArrayList filterItems)
        {
            bool rangeFilterInterval = (this.cboTypeNumProtocollo.SelectedValue == RANGE_FILTER_TYPE_INTERVAL);

            DocsPaWR.FiltroRicerca filterItem = null;

            if (this.txtInitNumProtocollo.Text.Length > 0)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();

                if (rangeFilterInterval)
                    filterItem.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();

                filterItem.valore = this.txtInitNumProtocollo.Text;
                filterItems.Add(filterItem);
                filterItem = null;
            }

            if (rangeFilterInterval && this.txtEndNumProtocollo.Text.Length > 0)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
                filterItem.valore = this.txtEndNumProtocollo.Text;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per data protocollo
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterDataProtocollo(ArrayList filterItems)
        {
            bool rangeFilterInterval = (this.cboTypeDataProtocollo.SelectedValue == RANGE_FILTER_TYPE_INTERVAL);

            DocsPaWR.FiltroRicerca filterItem = null;

            if (this.GetCalendarControl("txtInitDataProtocollo").txt_Data.Text.Length > 0)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();

                if (rangeFilterInterval)
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString();

                filterItem.valore = this.GetCalendarControl("txtInitDataProtocollo").txt_Data.Text;
                filterItems.Add(filterItem);
                filterItem = null;
            }

            if (rangeFilterInterval && this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text.Length > 0)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
                filterItem.valore = this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text;
                filterItems.Add(filterItem);
                filterItem = null;
            }

            if (this.txtAnnoProtocollo.Text.Length > 0)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
                filterItem.valore = this.txtAnnoProtocollo.Text;
                filterItems.Add(filterItem);
                filterItem = null;
            }
            else
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
                filterItem.valore = "";
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per id documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterIDDocumento(ArrayList filterItems)
        {
            bool rangeFilterInterval = (this.cboTypeIDDocumento.SelectedValue == RANGE_FILTER_TYPE_INTERVAL);

            DocsPaWR.FiltroRicerca filterItem = null;

            if (this.txtInitIDDocumento.Text.Length > 0)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();

                if (rangeFilterInterval)
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();

                filterItem.valore = this.txtInitIDDocumento.Text;
                filterItems.Add(filterItem);
                filterItem = null;
            }

            if (rangeFilterInterval && this.txtEndIDDocumento.Text.Length > 0)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
                filterItem.valore = this.txtEndIDDocumento.Text;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per data creazione documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterDataCreazioneDocumento(ArrayList filterItems)
        {
            bool rangeFilterInterval = (this.cboTypeDataCreazione.SelectedValue == RANGE_FILTER_TYPE_INTERVAL);

            DocsPaWR.FiltroRicerca filterItem = null;

            if (this.GetCalendarControl("txtInitDataCreazione").txt_Data.Text.Length > 0)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();

                if (rangeFilterInterval)
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString();

                filterItem.valore = this.GetCalendarControl("txtInitDataCreazione").txt_Data.Text;
                filterItems.Add(filterItem);
                filterItem = null;
            }

            if (rangeFilterInterval && this.GetCalendarControl("txtEndDataCreazione").txt_Data.Text.Length > 0)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                filterItem.valore = this.GetCalendarControl("txtEndDataCreazione").txt_Data.Text;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per tipologia documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterTipologiaDocumento(ArrayList filterItems)
        {
            if (this.ddl_tipoDoc.SelectedIndex > 0)
            {
                DocsPaWR.FiltroRicerca filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString();
                filterItem.valore = this.ddl_tipoDoc.SelectedItem.Value;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per oggetto documento
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterOggettoDocumento(ArrayList filterItems)
        {
            if (this.txtOggetto.Text.Length > 0)
            {
                DocsPaWR.FiltroRicerca filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.OGGETTO.ToString();
                filterItem.valore = this.txtOggetto.Text;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        /* ABBATANGELI GIANLUIGI */
        /// <summary>
        /// Creazione filtro per applicazione
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterApplicazioneDocumento(ArrayList filterItems)
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"]))
            {
                DocsPaWR.FiltroRicerca filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.COD_EXT_APP.ToString();
                filterItem.valore = (System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"]);//this.ddl_extApp.SelectedItem.Value;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        /// <summary>
        /// Creazione oggetti di filtro per soggetto mittente / destinatario
        /// </summary>
        /// <param name="filterItems"></param>
        private void AddFilterMittDestDocumento(ArrayList filterItems)
        {
            DocsPaWR.FiltroRicerca filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();

            string filterArgID = string.Empty;
            string filterArgDescr = string.Empty;

            switch (this.rbListTipoDocumento.SelectedValue)
            {
                case FILTER_TYPE_ARRIVO:
                    filterArgID = DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString();
                    filterArgDescr = DocsPaWR.FiltriDocumento.MITT_DEST.ToString();
                    break;
                case FILTER_TYPE_PARTENZA:
                case FILTER_TYPE_INTERNO:
                    filterArgID = DocsPaWR.FiltriDocumento.ID_DESTINATARIO.ToString();
                    filterArgDescr = DocsPaWR.FiltriDocumento.ID_DESCR_DESTINATARIO.ToString();
                    break;
            }

            if (this.txtDescrMittDest.Text.Length > 0 &&
                this.txtSystemIDMittDest.Value.Length > 0)
            {
                filterItem.argomento = filterArgID;
                filterItem.valore = this.txtSystemIDMittDest.Value;
                filterItems.Add(filterItem);
            }
            else if (this.txtDescrMittDest.Text.Length > 0)
            {
                filterItem.argomento = filterArgDescr;
                filterItem.valore = this.txtDescrMittDest.Text;
                filterItems.Add(filterItem);
            }

            filterItem = null;
        }

        /// <summary>
        /// Creazione oggetti filtro
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.FiltroRicerca[][] GetFilters()
        {
            ArrayList filterItems = new ArrayList();

            this.AddFilterTipoDocumento(filterItems);

            switch (this.rbListTipoDocumento.SelectedValue)
            {
                case FILTER_TYPE_ARRIVO:
                case FILTER_TYPE_PARTENZA:
                case FILTER_TYPE_INTERNO:
                    if (prov.Equals("Cestino"))
                    {
                        this.AddFilterDataCreazioneDocumento(filterItems);
                        this.AddFilterIDDocumento(filterItems);
                    }
                    else
                    {
                        this.AddFilterNumProtocollo(filterItems);
                        this.AddFilterDataProtocollo(filterItems);
                        this.AddFilterMittDestDocumento(filterItems);
                    }
                    break;

                case FILTER_TYPE_GRIGIO:
                    this.AddFilterIDDocumento(filterItems);
                    this.AddFilterDataCreazioneDocumento(filterItems);

                    break;

                case FILTER_TYPE_TUTTI:
                    this.AddFilterDataCreazioneDocumento(filterItems);
                    if (prov.Equals("Cestino"))
                    {
                        this.AddFilterIDDocumento(filterItems);
                    }
                    break;
            }

            this.AddFilterOggettoDocumento(filterItems);
            this.AddFilterTipologiaDocumento(filterItems);
            this.AddProfilazioneDinamica(filterItems);
            this.AddFilterFileFirmato(filterItems);
            this.AddFilterTipoFileAcquisito(filterItems);

            if (!GridManager.IsRoleEnabledToUseGrids())
            {
                if (ddl_tipoDoc.SelectedItem != null && !string.IsNullOrEmpty(ddl_tipoDoc.SelectedItem.Text))
                {
                    DocsPAWA.DocsPaWR.Templates template = ProfilazioneDocManager.getTemplatePerRicerca((UserManager.getInfoUtente(this)).idAmministrazione, ddl_tipoDoc.SelectedItem.Text, this);
                    if (template != null)
                    {
                        OggettoCustom customObjectTemp = new OggettoCustom();
                        customObjectTemp = template.ELENCO_OGGETTI.Where(
                        r => r.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && r.DA_VISUALIZZARE_RICERCA == "1").
                        FirstOrDefault();
                        DocsPAWA.DocsPaWR.FiltroRicerca fV1;
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.CONTATORE_GRIGLIE_NO_CUSTOM.ToString();
                        fV1.valore = customObjectTemp.TIPO_CONTATORE;
                        fV1.nomeCampo = template.SYSTEM_ID.ToString();
                        filterItems.Add(fV1);

                        // Creazione di un filtro per la profilazione
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = FiltriDocumento.PROFILATION_FIELD_FOR_ORDER.ToString();
                        fV1.valore = customObjectTemp.SYSTEM_ID.ToString();
                        fV1.nomeCampo = customObjectTemp.DESCRIZIONE;
                        filterItems.Add(fV1);
                    }


                }
            }

            DocsPaWR.FiltroRicerca[] initArray = new DocsPAWA.DocsPaWR.FiltroRicerca[filterItems.Count];
            filterItems.CopyTo(initArray);
            filterItems = null;

            DocsPaWR.FiltroRicerca[][] retValue = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
            retValue[0] = initArray;
            return retValue;
        }

        private void AddProfilazioneDinamica(ArrayList filterItems)
        {
            if (sRic != null && sRic.FiltriRicerca != null)
            {
                bool found = false;
                DocsPaWR.FiltroRicerca aux = null;
                for (int i = 0; !found && i < sRic.FiltriRicerca[0].Length; i++)
                {
                    aux = sRic.FiltriRicerca[0][i];
                    if (aux != null && aux.argomento == DocsPAWA.DocsPaWR.FiltriDocumento.PROFILAZIONE_DINAMICA.ToString())
                        found = true;
                    else aux = null;
                }

                if (aux != null)
                {
                    DocsPaWR.FiltroRicerca filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    filterItem.argomento = aux.argomento;
                    filterItem.valore = aux.valore;
                    filterItem.template = (DocsPaWR.Templates)Session["templateRicerca"];
                    filterItems.Add(filterItem);
                    filterItem = null;
                }
            }
        }

        private void ApplyFilter()
        {
            ArrayList validationItems;
            string firstInvalidControlID;

            // Validazione dati immessi
            if (this.IsValidData(out validationItems, out firstInvalidControlID))
            {
                // Creazione oggetti filtro
                DocsPaWR.FiltroRicerca[][] filters = GetFilters();

                if (filters != null)
                {
                    // Memorizzazione in sessione dei dati della UI
                    this.PersistUIDataOnSession();

                    // Memorizzazione filtri di ricerca in sessione
                    CurrentFilterSessionStorage.SetCurrentFilter(filters);
                }

                // Chiusura finestra di dialogo
                this.RegisterClientScript("CloseWindow", "CloseWindow(true);");
            }
            else
            {
                string validationMessage = string.Empty;

                foreach (string item in validationItems)
                {
                    if (validationMessage != string.Empty)
                        validationMessage += @"\n";

                    validationMessage += " - " + item;
                }

                if (validationMessage != string.Empty)
                    validationMessage = "Sono state rilevate le seguenti incongruenze: " +
                        @"\n" + @"\n" + validationMessage;

                this.RegisterClientScript("ValidationMessage", "alert('" + validationMessage + "');");

                // impostazione del focus sul primo controllo non valido
                //this.SetControlFocus(firstInvalidControlID);				
            }
        }

        #endregion

        #region Gestione memorizzazione / ripristino dati UI

        /// <summary>
        /// Memorizzazione dati in sessione
        /// relativamente ai filtri per protocollo
        /// </summary>
        private void PersistUIDataItemsProtocollo()
        {
            UIDataStorage.SetUIDataItemValue(this.cboTypeNumProtocollo.ID, this.cboTypeNumProtocollo.SelectedValue);
            UIDataStorage.SetUIDataItemValue(this.txtInitNumProtocollo.ID, this.txtInitNumProtocollo.Text);
            if (this.cboTypeNumProtocollo.SelectedValue == RANGE_FILTER_TYPE_INTERVAL)
                UIDataStorage.SetUIDataItemValue(this.txtEndNumProtocollo.ID, this.txtEndNumProtocollo.Text);

            UIDataStorage.SetUIDataItemValue(this.cboTypeDataProtocollo.ID, this.cboTypeDataProtocollo.SelectedValue);
            UIDataStorage.SetUIDataItemValue(this.GetCalendarControl("txtInitDataProtocollo").txt_Data.ID, this.GetCalendarControl("txtInitDataProtocollo").txt_Data.Text);
            if (this.cboTypeDataProtocollo.SelectedValue == RANGE_FILTER_TYPE_INTERVAL)
                UIDataStorage.SetUIDataItemValue(this.GetCalendarControl("txtEndDataProtocollo").txt_Data.ID, this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text);
            UIDataStorage.SetUIDataItemValue(this.txtAnnoProtocollo.ID, this.txtAnnoProtocollo.Text);

            UIDataStorage.SetUIDataItemValue(this.txtOggetto.ID, this.txtOggetto.Text);

            UIDataStorage.SetUIDataItemValue(this.txtSystemIDMittDest.ID, this.txtSystemIDMittDest.Value);
            UIDataStorage.SetUIDataItemValue(this.txtCodMittDest.ID, this.txtCodMittDest.Text);
            UIDataStorage.SetUIDataItemValue(this.txtDescrMittDest.ID, this.txtDescrMittDest.Text);
        }

        /// <summary>
        /// Memorizzazione dati in sessione
        /// relativamente ai filtri per doc grigio
        /// </summary>
        private void PersistUIDataItemsGrigio()
        {
            UIDataStorage.SetUIDataItemValue(this.cboTypeIDDocumento.ID, this.cboTypeIDDocumento.SelectedValue);
            UIDataStorage.SetUIDataItemValue(this.txtInitIDDocumento.ID, this.txtInitIDDocumento.Text);
            if (this.cboTypeIDDocumento.SelectedValue == RANGE_FILTER_TYPE_INTERVAL)
                UIDataStorage.SetUIDataItemValue(this.txtEndIDDocumento.ID, this.txtEndIDDocumento.Text);

            UIDataStorage.SetUIDataItemValue(this.cboTypeDataCreazione.ID, this.cboTypeDataCreazione.SelectedValue);
            UIDataStorage.SetUIDataItemValue(this.GetCalendarControl("txtInitDataCreazione").txt_Data.ID, this.GetCalendarControl("txtInitDataCreazione").txt_Data.Text);
            if (this.cboTypeDataCreazione.SelectedValue == RANGE_FILTER_TYPE_INTERVAL)
                UIDataStorage.SetUIDataItemValue(this.GetCalendarControl("txtEndDataCreazione").txt_Data.ID, this.GetCalendarControl("txtEndDataCreazione").txt_Data.Text);

            UIDataStorage.SetUIDataItemValue(this.txtOggetto.ID, this.txtOggetto.Text);
        }

        private void PersistUIDataItemsTutti()
        {
            UIDataStorage.SetUIDataItemValue(this.cboTypeIDDocumento.ID, this.cboTypeIDDocumento.SelectedValue);
            UIDataStorage.SetUIDataItemValue(this.txtInitIDDocumento.ID, this.txtInitIDDocumento.Text);
            if (this.cboTypeIDDocumento.SelectedValue == RANGE_FILTER_TYPE_INTERVAL)
                UIDataStorage.SetUIDataItemValue(this.txtEndIDDocumento.ID, this.txtEndIDDocumento.Text);

            UIDataStorage.SetUIDataItemValue(this.cboTypeDataCreazione.ID, this.cboTypeDataCreazione.SelectedValue);
            UIDataStorage.SetUIDataItemValue(this.GetCalendarControl("txtInitDataCreazione").txt_Data.ID, this.GetCalendarControl("txtInitDataCreazione").txt_Data.Text);
            if (this.cboTypeDataCreazione.SelectedValue == RANGE_FILTER_TYPE_INTERVAL)
                UIDataStorage.SetUIDataItemValue(this.GetCalendarControl("txtEndDataCreazione").txt_Data.ID, this.GetCalendarControl("txtEndDataCreazione").txt_Data.Text);

            UIDataStorage.SetUIDataItemValue(this.txtOggetto.ID, this.txtOggetto.Text);
        }

        /// <summary>
        /// Memorizzazione dati UI in sessione
        /// </summary>
        private void PersistUIDataOnSession()
        {
            UIDataStorage.ClearUIDataItems();

            string tipoDocumento = this.rbListTipoDocumento.SelectedValue;

            UIDataStorage.SetUIDataItemValue(this.rbListTipoDocumento.ID, tipoDocumento);

            if (tipoDocumento == FILTER_TYPE_ARRIVO ||
                tipoDocumento == FILTER_TYPE_PARTENZA ||
                tipoDocumento == FILTER_TYPE_INTERNO)
                this.PersistUIDataItemsProtocollo();

            else if (tipoDocumento == FILTER_TYPE_GRIGIO)
                this.PersistUIDataItemsGrigio();

            else if (tipoDocumento == FILTER_TYPE_TUTTI)
                this.PersistUIDataItemsTutti();

            //Tipologia Documento
            UIDataStorage.SetUIDataItemValue(this.ddl_tipoDoc.ID, this.ddl_tipoDoc.SelectedValue);
            //tipologia file acquisito
            UIDataStorage.SetUIDataItemValue(this.ddl_tipoFileAcquisiti.ID, this.ddl_tipoFileAcquisiti.SelectedValue);
            //file firmati e non
            UIDataStorage.SetUIDataItemValue(this.cb_nonFirmato.ID, Convert.ToString(this.cb_nonFirmato.Checked));
            UIDataStorage.SetUIDataItemValue(this.cb_firmato.ID, Convert.ToString(this.cb_firmato.Checked));
        }

        /// <summary>
        /// Ripristino dati relativi al filtro per protocollo
        /// </summary>
        private void RestoreUIDataItemsProtocollo()
        {
            if (UIDataStorage.ContainUIDataItemValue(this.cboTypeNumProtocollo.ID))
                this.cboTypeNumProtocollo.SelectedValue = UIDataStorage.GetUIDataItemValue(this.cboTypeNumProtocollo.ID);
            if (UIDataStorage.ContainUIDataItemValue(this.txtInitNumProtocollo.ID))
                this.txtInitNumProtocollo.Text = UIDataStorage.GetUIDataItemValue(this.txtInitNumProtocollo.ID);
            if (UIDataStorage.ContainUIDataItemValue(this.txtEndNumProtocollo.ID))
                this.txtEndNumProtocollo.Text = UIDataStorage.GetUIDataItemValue(this.txtEndNumProtocollo.ID);

            if (UIDataStorage.ContainUIDataItemValue(this.cboTypeDataProtocollo.ID))
                this.cboTypeDataProtocollo.SelectedValue = UIDataStorage.GetUIDataItemValue(this.cboTypeDataProtocollo.ID);
            if (UIDataStorage.ContainUIDataItemValue(this.GetCalendarControl("txtInitDataProtocollo").txt_Data.ID))
                this.GetCalendarControl("txtInitDataProtocollo").txt_Data.Text = UIDataStorage.GetUIDataItemValue(this.GetCalendarControl("txtInitDataProtocollo").txt_Data.ID);
            if (UIDataStorage.ContainUIDataItemValue(this.GetCalendarControl("txtEndDataProtocollo").txt_Data.ID))
                this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text = UIDataStorage.GetUIDataItemValue(this.GetCalendarControl("txtEndDataProtocollo").txt_Data.ID);
            if (UIDataStorage.ContainUIDataItemValue(this.txtAnnoProtocollo.ID))
                this.txtAnnoProtocollo.Text = UIDataStorage.GetUIDataItemValue(this.txtAnnoProtocollo.ID);

            if (UIDataStorage.ContainUIDataItemValue(this.txtOggetto.ID))
                this.txtOggetto.Text = UIDataStorage.GetUIDataItemValue(this.txtOggetto.ID);

            if (UIDataStorage.ContainUIDataItemValue(this.txtSystemIDMittDest.ID))
                this.txtSystemIDMittDest.Value = UIDataStorage.GetUIDataItemValue(this.txtSystemIDMittDest.ID);
            if (UIDataStorage.ContainUIDataItemValue(this.txtCodMittDest.ID))
                this.txtCodMittDest.Text = UIDataStorage.GetUIDataItemValue(this.txtCodMittDest.ID);
            if (UIDataStorage.ContainUIDataItemValue(this.txtDescrMittDest.ID))
                this.txtDescrMittDest.Text = UIDataStorage.GetUIDataItemValue(this.txtDescrMittDest.ID);
        }

        /// <summary>
        /// Ripristino dati relativi al filtro per doc grigio
        /// </summary>
        private void RestoreUIDataItemsGrigio()
        {
            if (UIDataStorage.ContainUIDataItemValue(this.cboTypeIDDocumento.ID))
                this.cboTypeIDDocumento.SelectedValue = UIDataStorage.GetUIDataItemValue(this.cboTypeIDDocumento.ID);
            if (UIDataStorage.ContainUIDataItemValue(this.txtInitIDDocumento.ID))
                this.txtInitIDDocumento.Text = UIDataStorage.GetUIDataItemValue(this.txtInitIDDocumento.ID);
            if (UIDataStorage.ContainUIDataItemValue(this.txtEndIDDocumento.ID))
                this.txtEndIDDocumento.Text = UIDataStorage.GetUIDataItemValue(this.txtEndIDDocumento.ID);

            if (UIDataStorage.ContainUIDataItemValue(this.cboTypeDataCreazione.ID))
                this.cboTypeDataCreazione.SelectedValue = UIDataStorage.GetUIDataItemValue(this.cboTypeDataCreazione.ID);
            if (UIDataStorage.ContainUIDataItemValue(this.GetCalendarControl("txtInitDataCreazione").txt_Data.ID))
                this.GetCalendarControl("txtInitDataCreazione").txt_Data.Text = UIDataStorage.GetUIDataItemValue(this.GetCalendarControl("txtInitDataCreazione").txt_Data.ID);
            if (UIDataStorage.ContainUIDataItemValue(this.GetCalendarControl("txtEndDataCreazione").txt_Data.ID))
                this.GetCalendarControl("txtEndDataCreazione").txt_Data.Text = UIDataStorage.GetUIDataItemValue(this.GetCalendarControl("txtEndDataCreazione").txt_Data.ID);

            if (UIDataStorage.ContainUIDataItemValue(this.txtOggetto.ID))
                this.txtOggetto.Text = UIDataStorage.GetUIDataItemValue(this.txtOggetto.ID);
        }

        private void RestoreUIDataItemsTutti()
        {
            if (UIDataStorage.ContainUIDataItemValue(this.cboTypeIDDocumento.ID))
                this.cboTypeIDDocumento.SelectedValue = UIDataStorage.GetUIDataItemValue(this.cboTypeIDDocumento.ID);
            if (UIDataStorage.ContainUIDataItemValue(this.txtInitDataCreazione.ID))
                this.txtInitIDDocumento.Text = UIDataStorage.GetUIDataItemValue(this.txtInitIDDocumento.ID);
            if (UIDataStorage.ContainUIDataItemValue(this.txtEndDataCreazione.ID))
                this.txtEndIDDocumento.Text = UIDataStorage.GetUIDataItemValue(this.txtEndIDDocumento.ID);

            if (UIDataStorage.ContainUIDataItemValue(this.cboTypeDataCreazione.ID))
                this.cboTypeDataCreazione.SelectedValue = UIDataStorage.GetUIDataItemValue(this.cboTypeDataCreazione.ID);
            if (UIDataStorage.ContainUIDataItemValue(this.GetCalendarControl("txtInitDataCreazione").txt_Data.ID))
                this.GetCalendarControl("txtInitDataCreazione").txt_Data.Text = UIDataStorage.GetUIDataItemValue(this.GetCalendarControl("txtInitDataCreazione").txt_Data.ID);
            if (UIDataStorage.ContainUIDataItemValue(this.GetCalendarControl("txtEndDataCreazione").txt_Data.ID))
                this.GetCalendarControl("txtEndDataCreazione").txt_Data.Text = UIDataStorage.GetUIDataItemValue(this.GetCalendarControl("txtEndDataCreazione").txt_Data.ID);

            if (UIDataStorage.ContainUIDataItemValue(this.txtOggetto.ID))
                this.txtOggetto.Text = UIDataStorage.GetUIDataItemValue(this.txtOggetto.ID);

        }

        /// <summary>
        /// Ripristino dati UI da sessione
        /// </summary>
        private void RestoreUIDataFromSession()
        {
            if (UIDataStorage.ContainUIDataItemValue(this.rbListTipoDocumento.ID))
                this.rbListTipoDocumento.SelectedValue = UIDataStorage.GetUIDataItemValue(this.rbListTipoDocumento.ID);

            string tipoDocumento = this.rbListTipoDocumento.SelectedValue;

            if (tipoDocumento == FILTER_TYPE_ARRIVO ||
                tipoDocumento == FILTER_TYPE_PARTENZA ||
                tipoDocumento == FILTER_TYPE_INTERNO)
                this.RestoreUIDataItemsProtocollo();

            else if (tipoDocumento == FILTER_TYPE_GRIGIO)
                this.RestoreUIDataItemsGrigio();

            else if (tipoDocumento == FILTER_TYPE_TUTTI)
                this.RestoreUIDataItemsTutti();

            //Tipologia Documento
            if (UIDataStorage.ContainUIDataItemValue(this.ddl_tipoDoc.ID))
                this.ddl_tipoDoc.SelectedValue = UIDataStorage.GetUIDataItemValue(this.ddl_tipoDoc.ID);
            //file firmati e non
            if (UIDataStorage.ContainUIDataItemValue(this.cb_firmato.ID))
                this.cb_firmato.Checked = Convert.ToBoolean(UIDataStorage.GetUIDataItemValue(this.cb_firmato.ID));
            if (UIDataStorage.ContainUIDataItemValue(this.cb_nonFirmato.ID))
                this.cb_nonFirmato.Checked = Convert.ToBoolean(UIDataStorage.GetUIDataItemValue(this.cb_nonFirmato.ID));
            //tipologia file acquisito
            if (UIDataStorage.ContainUIDataItemValue(this.ddl_tipoFileAcquisiti.ID))
                this.ddl_tipoFileAcquisiti.SelectedValue = UIDataStorage.GetUIDataItemValue(this.ddl_tipoFileAcquisiti.ID);

        }

        #endregion

        #region Classi interne

        /// <summary>
        /// Classe per la memorizzazione dei dati della UI in sessione
        /// </summary>
        public sealed class UIDataStorage
        {
            private const string SESSION_KEY_UI_DATA = "FiltriRicercaDocumenti.UIData";

            private UIDataStorage()
            {
            }

            public static void SetUIDataItemValue(string controlID, string controlValue)
            {
                Hashtable ht = HttpContext.Current.Session[SESSION_KEY_UI_DATA] as Hashtable;

                if (ht == null)
                    ht = Hashtable.Synchronized(new Hashtable());

                if (ht.ContainsKey(controlID))
                    ht[controlID] = controlValue;
                else
                    ht.Add(controlID, controlValue);

                HttpContext.Current.Session[SESSION_KEY_UI_DATA] = ht;
            }

            public static bool ContainUIDataItemValue(string controlID)
            {
                Hashtable ht = HttpContext.Current.Session[SESSION_KEY_UI_DATA] as Hashtable;
                return (ht != null && ht.ContainsKey(controlID));
            }

            public static string GetUIDataItemValue(string controlID)
            {
                string retValue = null;

                if (ContainUIDataItemValue(controlID))
                    retValue = (string)((Hashtable)HttpContext.Current.Session[SESSION_KEY_UI_DATA])[controlID];

                return retValue;
            }

            public static void RemoveUIDataItem(string controlID)
            {
                if (ContainUIDataItemValue(controlID))
                    ((Hashtable)HttpContext.Current.Session[SESSION_KEY_UI_DATA]).Remove(controlID);
            }

            public static void ClearUIDataItems()
            {
                Hashtable ht = HttpContext.Current.Session[SESSION_KEY_UI_DATA] as Hashtable;

                if (ht != null)
                {
                    ht.Clear();
                    HttpContext.Current.Session.Remove(SESSION_KEY_UI_DATA);
                }
            }
        }

        /// <summary>
        /// Classe per la gestione degli oggetti di filtro in sessione
        /// </summary>
        public sealed class CurrentFilterSessionStorage
        {
            private const string SESSION_KEY = "CurrentFilterSessionStorage.CurrentFilter";

            private CurrentFilterSessionStorage()
            {
            }

            public static DocsPAWA.DocsPaWR.FiltroRicerca[][] GetCurrentFilter()
            {
                return (DocsPAWA.DocsPaWR.FiltroRicerca[][])HttpContext.Current.Session[SESSION_KEY];
            }

            public static void SetCurrentFilter(DocsPAWA.DocsPaWR.FiltroRicerca[][] filter)
            {
                HttpContext.Current.Session[SESSION_KEY] = filter;
            }

            public static void RemoveCurrentFilter()
            {
                // Rimozione dati UI da sessione
                UIDataStorage.ClearUIDataItems();

                // Rimozione oggetti di filtro
                HttpContext.Current.Session.Remove(SESSION_KEY);
            }
        }

        /// <summary>
        /// Classe wrapper per la gestione dei corrispondenti 
        /// </summary>
        public sealed class RubricaWrapper
        {
            private const string SESSION_KEY = "CurrentCorrSessionStorage.CurrentCorr";

            private RubricaWrapper()
            {
            }

            /// <summary>
            /// Reperimento di un corrispondente in base ad un codice rubrica fornito in ingresso
            /// </summary>
            /// <param name="page"></param>
            /// <param name="codCorrispondente"></param>
            /// <returns></returns>
            public static DocsPAWA.DocsPaWR.Corrispondente GetCorrispondenteDaCodice(Page page, string codCorrispondente)
            {
                DocsPaWR.Corrispondente retValue = null;

                if (codCorrispondente != null)
                {
                    retValue = DocsPAWA.UserManager.getCorrispondente(page, codCorrispondente, true);
                }

                return retValue;
            }

            /// <summary>
            /// Reperimento da sessione dell'oggetto corrispondente selezionato da rubrica
            /// </summary>
            /// <returns></returns>
            public static DocsPAWA.DocsPaWR.Corrispondente GetCorrispondenteDaRubrica()
            {
                return (DocsPAWA.DocsPaWR.Corrispondente)HttpContext.Current.Session[SESSION_KEY];
            }

            /// <summary>
            /// Impostazione in sessione dell'oggetto corrispondente selezionato da rubrica
            /// </summary>
            /// <param name="corr"></param>
            public static void SetCorrispondenteDaRubrica(DocsPAWA.DocsPaWR.Corrispondente corr)
            {
                HttpContext.Current.Session[SESSION_KEY] = corr;
            }

            /// <summary>
            /// Rimozione da sessione dell'oggetto corrispondente selezionato da rubrica
            /// </summary>
            public static void RemoveCorrispondenteDaRubrica()
            {
                HttpContext.Current.Session.Remove(SESSION_KEY);
            }
        }

        /// <summary>
        /// Classe per la gestione degli oggetti di filtro in sessione
        /// </summary>
        public sealed class CurrentFilterSessionOrderFilter
        {
            private const string SESSION_KEY = "CurrentFilterSessionOrderFilter.CurrentFilter";

            private CurrentFilterSessionOrderFilter()
            {
            }

            public static DocsPAWA.DocsPaWR.FiltroRicerca[][] GetCurrentFilter()
            {
                return (DocsPAWA.DocsPaWR.FiltroRicerca[][])HttpContext.Current.Session[SESSION_KEY];
            }

            public static void SetCurrentFilter(DocsPAWA.DocsPaWR.FiltroRicerca[][] filter)
            {
                HttpContext.Current.Session[SESSION_KEY] = filter;
            }

            public static void RemoveCurrentFilter()
            {
                // Rimozione dati UI da sessione
                UIDataStorage.ClearUIDataItems();

                // Rimozione oggetti di filtro
                HttpContext.Current.Session.Remove(SESSION_KEY);
            }
        }

        #endregion

        private void caricaComboTipoFileAcquisiti()
        {
            ArrayList tipoFile = new ArrayList();
            tipoFile = DocumentManager.getExtFileAcquisiti(this);
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

        private void AddFilterTipoFileAcquisito(ArrayList filterItems)
        {
            if (this.ddl_tipoFileAcquisiti.SelectedIndex > 0)
            {
                DocsPaWR.FiltroRicerca filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.TIPO_FILE_ACQUISITO.ToString();
                filterItem.valore = this.ddl_tipoFileAcquisiti.SelectedItem.Value;
                filterItems.Add(filterItem);
                filterItem = null;
            }
        }

        private void AddFilterFileFirmato(ArrayList filterItems)
        {
            if (this.cb_firmato.Checked)
            {
                //cerco documenti firmati
                if (!cb_nonFirmato.Checked)
                {
                    DocsPaWR.FiltroRicerca filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.FIRMATO.ToString();
                    filterItem.valore = "1";
                    filterItems.Add(filterItem);
                    filterItem = null;
                }
                else
                {//cerco documenti che abbiano un file acquisito, sia esso firmato o meno.
                    DocsPaWR.FiltroRicerca filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.FIRMATO.ToString();
                    filterItem.valore = "2";
                    filterItems.Add(filterItem);
                    filterItem = null;
                }
            }
            else
            {
                //cerco i documenti non firmati
                if (cb_nonFirmato.Checked)
                {
                    DocsPaWR.FiltroRicerca filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.FIRMATO.ToString();
                    filterItem.valore = "0";
                    filterItems.Add(filterItem);
                    filterItem = null;
                }

            }

        }

        //INSERITA DA FABIO PRENDE LE ETICHETTE DEI PROTOCOLLI
        private void getLettereProtocolli()
        {
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
            DocsPAWA.DocsPaWR.InfoUtente infoUtente = session.getUserAmmSession();
            String idAmm = null;
            if (UserManager.getInfoUtente() != null && !string.IsNullOrEmpty(UserManager.getInfoUtente().idAmministrazione))
                idAmm = UserManager.getInfoUtente().idAmministrazione;
            this.etichette = wws.getEtichetteDocumenti(infoUtente, idAmm);
        }
    }
}
