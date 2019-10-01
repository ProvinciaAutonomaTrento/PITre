using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Amministrazione.Manager;
using SAAdminTool.DocsPaWR;
using System.ComponentModel;
using SAAdminTool.AdminTool.Manager;
using SAAdminTool.SiteNavigation;
using SAAdminTool.utils;

namespace SAAdminTool.UserControls
{
    public partial class PannelloRicercaModelliTrasmissione : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.txtVisibilitaCodice.Text))
                this.txtVisibilitaDescrizione.Text = String.Empty;
            if (String.IsNullOrEmpty(this.txtDestinatariCodice.Text))
                this.txtDestinatariDescrizione.Text = String.Empty;


            // Inizializzazione della sezione di ricerca
            if (!IsPostBack)
                this.InitializeSearchSection();

            // Compilazione dei controlli con codice e descrizione se da impostare
            // se è presente in sessione CodeTextBox
            if (Session["CodeTextBox"] != null)
                this.CompileCodeAndDescription();

        }

        #region Creazione filtri ricerca

        /// <summary>
        /// Funzione per la generaizone dei filtri da utilizzare per la ricerca
        /// </summary>
        /// <returns>Array dei filtri di ricerca</returns>
        public FiltroRicerca[] CreateSearchFilters()
        {
            List<FiltroRicerca> filters = new List<FiltroRicerca>();

            #region Codice modello

            if (!String.IsNullOrEmpty(this.txtCodice.Text))
                filters.Add(this.GetFilter(FiltriModelliTrasmissione.CODICE_MODELLO, this.txtCodice.Text));

            #endregion

            #region Descrizione modello

            if (!String.IsNullOrEmpty(this.txtModello.Text))
                filters.Add(this.GetFilter(FiltriModelliTrasmissione.DESCRIZIONE_MODELLO, this.txtModello.Text));

            #endregion

            #region Note

            if (!String.IsNullOrEmpty(this.txtNote.Text))
                filters.Add(this.GetFilter(FiltriModelliTrasmissione.NOTE, this.txtNote.Text));

            #endregion

            #region Tipo trasmissione

            if (this.ddlTipoTrasmissione.SelectedIndex > 0)
                filters.Add(this.GetFilter(FiltriModelliTrasmissione.TIPO_TRASMISSIONE, this.ddlTipoTrasmissione.SelectedValue));

            #endregion

            #region Registro

            if (this.ddlRegistri.SelectedIndex > 0)
                filters.Add(this.GetFilter(FiltriModelliTrasmissione.ID_REGISTRO, this.ddlRegistri.SelectedValue));

            #endregion

            #region Ragione trasmissione

            if (this.ddlRagioniTrasmissione.SelectedIndex > 0)
                filters.Add(this.GetFilter(FiltriModelliTrasmissione.ID_RAGIONE_TRASMISSIONE, this.ddlRagioniTrasmissione.SelectedValue));

            #endregion

            #region Corrispondente per visibilità

            if (!String.IsNullOrEmpty(this.txtVisibilitaCodice.Text) && !String.IsNullOrEmpty(this.txtVisibilitaDescrizione.Text))
                filters.Add(this.GetFilter(FiltriModelliTrasmissione.CODICE_CORR_PER_VISIBILITA, this.txtVisibilitaCodice.Text));

            #endregion

            #region Corrispondente per destinatario

            if (!String.IsNullOrEmpty(this.txtDestinatariCodice.Text) && !String.IsNullOrEmpty(this.txtDestinatariDescrizione.Text))
                filters.Add(this.GetFilter(FiltriModelliTrasmissione.CODICE_CORR_PER_DESTINATARIO, this.txtDestinatariCodice.Text));

            #endregion

            #region Ruolo destinatario disabilitato

            if (this.chkRuoloDestDisabilitato.Checked)
                filters.Add(this.GetFilter(FiltriModelliTrasmissione.RUOLI_DEST_DISABLED, String.Empty));

            #endregion

            #region Ruolo Destinatario inibito

            if (this.chkRuoloDestInibito.Checked)
                filters.Add(this.GetFilter(FiltriModelliTrasmissione.RUOLI_DISABLED_RIC_TRASM, String.Empty));

            #endregion

            #region Ricerca modelli creati dall'utente

            if (this.rblSearchScope.SelectedValue == "OnlyUser")
                filters.Add(this.GetFilter(FiltriModelliTrasmissione.MODELLI_CREATI_DA_UTENTE, String.Empty));

            #endregion

            #region Ricerca modelli creati dall'amministratore

            if (this.rblSearchScope.SelectedValue == "OnlyAdmin")
                filters.Add(this.GetFilter(FiltriModelliTrasmissione.MODELLI_CREATI_DA_AMMINISTRATORE, String.Empty));

            #endregion

            return filters.ToArray();
        }

        /// <summary>
        /// Funzione di utilità per la generazione di un filtro
        /// </summary>
        /// <param name="filterType">Tipo di filtro</param>
        /// <param name="value">Valore da utilizzare per filtrare i risultati</param>
        /// <returns>Filtro inizializzato</returns>
        private FiltroRicerca GetFilter(FiltriModelliTrasmissione filterType, String value)
        {
            return new FiltroRicerca()
            {
                argomento = filterType.ToString(),
                valore = value
            };

        }

        #endregion

        #region Gestione pulsanti soluzione codice e ricerca da rubrica per la sezione dei filtri di ricerca

        protected void imgSrcVisibilita_Click(object sender, ImageClickEventArgs e)
        {
            Registro reg = new Registro();
            reg.systemId = this.ddlRegistry.SelectedValue.ToString();
            reg.codRegistro = this.ddlRegistry.SelectedItem.Text.ToString();

            // Ricerca dei corrispondenti con il codice inserito. Se ne vengono restituiti più di uno
            // viene preso il primo
            ElementoRubrica[] elems = this.ResolveCode(this.txtVisibilitaCodice.Text, true, reg, RubricaCallType.CALLTYPE_MITT_MODELLO_TRASM);
            if (elems != null && elems.Length > 0)
            {
                this.txtVisibilitaCodice.Text = elems[0].codice;
                this.txtVisibilitaDescrizione.Text = elems[0].descrizione;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "noCorr", "alert('Corrispondente non trovato.');", true);
                this.txtVisibilitaCodice.Text = String.Empty;
                this.txtVisibilitaDescrizione.Text = String.Empty;
            }
        }

        protected void imgRubricaVisibilita_Click(object sender, ImageClickEventArgs e)
        {
            Registro reg = (SAAdminTool.DocsPaWR.Registro)Session["userRegistro"];
            if (reg == null)
                reg = new Registro();

            reg.systemId = this.ddlRegistry.SelectedValue.ToString();
            reg.codRegistro = this.ddlRegistry.SelectedItem.Text.ToString();
            Session.Add("userRegistro", reg);

            // Registrazione script per l'apertura della rubrica e salvataggio in sessione di un valore che
            // consenta di capire che alla chiusura della finestra della rubrica deve essere compilato il
            // codice e la descrizione per la visibilità
            Page.ClientScript.RegisterStartupScript(this.GetType(), "abForVisibility", "_ApriRubrica();", true);
            Session["CodeTextBox"] = this.txtVisibilitaCodice.ID;
        }

        protected void imgSrcDestinatari_Click(object sender, ImageClickEventArgs e)
        {
            // Ricerca dei corrispondenti con il codice inserito. Se ne vengono restituiti più di uno
            // viene preso il primo
            ElementoRubrica[] elems = this.ResolveCode(this.txtDestinatariCodice.Text, false, null, RubricaCallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI);
            if (elems != null && elems.Length > 0)
            {
                this.txtDestinatariCodice.Text = elems[0].codice;
                this.txtDestinatariDescrizione.Text = elems[0].descrizione;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "noCorr", "alert('Corrispondente non trovato.');", true);
                this.txtDestinatariCodice.Text = String.Empty;
                this.txtDestinatariDescrizione.Text = String.Empty;
            }
        }

        protected void imgRubricaDestinatari_Click(object sender, ImageClickEventArgs e)
        {
            // Registrazione script per l'apertura della rubrica e salvataggio in sessione di un valore che
            // consenta di capire che alla chiusura della finestra della rubrica deve essere compilato il
            // codice e la descrizione per i destinatari
            Page.ClientScript.RegisterStartupScript(this.GetType(), "abForReceivers", "ApriRubricaSelDest();", true);
            Session["CodeTextBox"] = this.txtDestinatariCodice.ID;
        }

        /// <summary>
        /// Funzione per la risoluzione di un codice corrispondente
        /// </summary>
        /// <param name="code">Codice da risolvere</param>
        /// <param name="searchOnlyRoles">True se bisogna ricercare solo nei ruoli</param>
        /// <param name="registro">Registro in cui ricercare</param>
        /// <param name="callType">Call type da utilizzare per la ricerca</param>
        /// <returns>Array di oggetti con le informazioni sul corrispondenti trovati</returns>
        private ElementoRubrica[] ResolveCode(String code, bool searchOnlyRoles, Registro registro, RubricaCallType callType)
        {
            ElementoRubrica[] corrSearch = null;
            if (!String.IsNullOrEmpty(code))
            {
                ParametriRicercaRubrica qco = new ParametriRicercaRubrica();
                UserManager.setQueryRubricaCaller(ref qco);
                qco.codice = code.Trim();
                qco.tipoIE = SAAdminTool.DocsPaWR.AddressbookTipoUtente.INTERNO;
                qco.calltype = callType;
                //cerco su tutti i tipi utente:
                if (ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] != null &&
                    ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] == "1")
                    qco.doListe = true;
                qco.doRuoli = true;
                if (searchOnlyRoles)
                {
                    qco.doUtenti = false;
                    qco.doUo = false;
                }
                else
                {
                    qco.doUtenti = true;
                    qco.doUo = true;
                }

                qco.queryCodiceEsatta = true;

                corrSearch = UserManager.getElementiRubrica(this.Page, qco);
            }

            return corrSearch;

        }

        protected void txtVisibilitaCodice_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.txtVisibilitaCodice.Text.Trim()))
                this.txtVisibilitaDescrizione.Text = String.Empty;
        }

        protected void txtDestinatariCodice_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.txtDestinatariCodice.Text.Trim()))
                this.txtDestinatariDescrizione.Text = String.Empty;
        }

        #endregion

        /// <summary>
        /// Funzione per l'inizializzazione della sezione di ricerca
        /// </summary>
        private void InitializeSearchSection()
        {

            // Registrazione eventi onchange javascript per le text box dei codici
            this.txtDestinatariCodice.Attributes["onchange"] = String.Format("javascript:clearCorrData(this, '{0}');", this.txtDestinatariDescrizione.ClientID);
            this.txtVisibilitaCodice.Attributes["onchange"] = String.Format("javascript:clearCorrData(this, '{0}');", this.txtVisibilitaDescrizione.ClientID);

            DocsPaWebService ws = new DocsPaWebService();
            ws.Timeout = Timeout.Infinite;
            //string[] amm = ((string)Session["AMMDATASET"]).Split('@');
            //string codAmm = amm[0];
            //String idAmm = ws.getIdAmmByCod(codAmm);
            String idAmm = UserManager.getInfoUtente().idAmministrazione;

            // Registri
            this.ddlRegistri.Items.Clear();
            this.ddlRegistri.Items.Add(String.Empty);
            OrganigrammaManager theManager = new OrganigrammaManager();
            theManager.ListaRegistriRF(idAmm, null, String.Empty);
            foreach (OrgRegistro item in theManager.getListaRegistri())
                this.ddlRegistri.Items.Add(new ListItem(item.Descrizione, item.IDRegistro));

            // Ragioni di trasmissione
            this.ddlRagioniTrasmissione.Items.Clear();
            this.ddlRagioniTrasmissione.Items.Add(String.Empty);
            RagioneTrasmissione[] rag = SAAdminTool.AdminTool.Manager.ModelliTrasmManager.getlistRagioniTrasm(idAmm, false, String.Empty);
            foreach (RagioneTrasmissione item in rag)
                this.ddlRagioniTrasmissione.Items.Add(new ListItem(item.descrizione, item.systemId));

            // Impostazione del css per i bottoni
            this.btnFind.CssClass = this.ButtonCss;
            this.btnExport.CssClass = this.ButtonCss;
            this.btnFindAndReplace.CssClass = this.ButtonCss;

            // Visualizzazione dei controlli in base al contesto
            this.ShowControl();

            // Caricamento dei registri
            this.LoadRegistryInformation();

        }

        /// <summary>
        /// Funzione per la visualizzazione dei controlli in base al contesto
        /// </summary>
        private void ShowControl()
        {
            if (this.UserType == UserTypeEnum.Administrator)
            {
                // L'amministratore deve visualizzare anche il filtro per lo scope di ricerca e quello per la ricerca mittente
                this.pnlRange.Visible = true;
                this.pnlVisibilita.Visible = true;

                this.pnlVisibilita.Style.Add("float", "left");
                //this.pnlVisibilita.Style.Add(HtmlTextWriterStyle.Width, "49%");
                this.pnlDestinatari.Style.Add("float", "left");
                //this.pnlVisibilita.Style.Add(HtmlTextWriterStyle.Width, "48%");

            }
            else
            {
                //this.pnlVisibilita.Style.Add(HtmlTextWriterStyle.Width, "48%");

            }

        }

        /// <summary>
        /// Metodo per il caricamento dei registri
        /// </summary>
        private void LoadRegistryInformation()
        {
            // Se ci si trova in amministrazione, vengono caricati tutti i registri dell'amministrazione
            if (Session["AMMDATASET"] != null)
            {
                string[] amm = ((string)Session["AMMDATASET"]).Split('@');
                string codAmm = amm[0];

                OrgRegistro[] registri = UserManager.getRegistriByCodAmm(codAmm, "0");

                foreach (var reg in registri)
                    this.ddlRegistry.Items.Add(new ListItem(reg.Descrizione, reg.IDRegistro));

            }
            else
            {
                // Vengono caricati i soli registri ed RF per il ruolo dell'utente
                Registro[] registri = UserManager.GetRegistriByRuolo(this.Page, UserManager.getRuolo().systemId);

                foreach (var reg in registri)
                    this.ddlRegistry.Items.Add(new ListItem(reg.descrizione, reg.systemId));

            }

            this.ddlRegistry.SelectedIndex = 0;

        }


        /// <summary>
        /// Funzione per la compilazione delle caselle di codice e descrizione 
        /// </summary>
        private void CompileCodeAndDescription()
        {
            switch (Session["CodeTextBox"].ToString())
            {
                case "txtVisibilitaCodice":     // Variabile di sessione impostata: selMittDaRubrica
                    // Se l'elemento in sessione è un array di ElementoRubrica
                    if (Session["selMittDaRubrica"] != null && Session["selMittDaRubrica"] is ElementoRubrica[])
                    {
                        ElementoRubrica[] elems = Session["selMittDaRubrica"] as ElementoRubrica[];

                        // Compilazione dei campi codice e descrizione del corrispondente per visibilità (se ne è stato selezionato
                        // più di uno, viene preso il primo)
                        this.txtVisibilitaCodice.Text = elems[0].codice;
                        this.txtVisibilitaDescrizione.Text = elems[0].descrizione;
                    }
                    break;
                case "txtDestinatariCodice":    // Variabile di sessione impostata: selDestDaRubrica
                    // Se l'elemento in sessione è un array di ElementoRubrica
                    if (Session["SelDescFindModelli"] != null && Session["SelDescFindModelli"] is ElementoRubrica[])
                    {
                        ElementoRubrica[] elems = Session["SelDescFindModelli"] as ElementoRubrica[];

                        // Compilazione dei campi codice e descrizione del corrispondente per destinatario (se ne è stato selezionato
                        // più di uno, viene preso il primo)
                        this.txtDestinatariCodice.Text = elems[0].codice;
                        this.txtDestinatariDescrizione.Text = elems[0].descrizione;
                    }

                    break;
            }

            // Pulizia sessione
            Session.Remove("CodeTextBox");
            Session.Remove("selMittDaRubrica");
            Session.Remove("SelDescFindModelli");

        }

        protected void btnFindAndReplace_Click(object sender, EventArgs e)
        {
            Session.Add("ClickFindAR", "click");

            // Salvataggio filtri di ricerca nel context
            ModelliTrasmManager.SearchFilters = this.CreateSearchFilters();

            // Registrazione script apertura popup
            Page.ClientScript.RegisterStartupScript(this.GetType(), "FindAndReplace", popup.FindAndReplaceModelliTrasmissione.ScriptToOpenPage, true);

        }

        protected void btnFind_Click(object sender, EventArgs e)
        {
            if (this.Search != null)
                this.Search(sender, e);
        }

        public bool EnableEsportAndFindButtons
        {
            set
            {
                this.btnExport.Enabled = value;
                this.btnFindAndReplace.Enabled = value;
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            // Recupero dell informazioni sull'utente / amministratore loggato
            InfoUtente userInfo = null;
            if (this.UserType == UserTypeEnum.Administrator)
            {
                SessionManager sm = new SessionManager();
                userInfo = sm.getUserAmmSession();
                string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];
                userInfo.idAmministrazione = new DocsPaWebService().getIdAmmByCod(codiceAmministrazione);
            }
            else
                userInfo = UserManager.getInfoUtente();

            // Inizializzazione del call context
            if (CallContextStack.CurrentContext == null)
                CallContextStack.CurrentContext = new CallContext("ReportingContext");

            // Salvataggio della request nel call context
            PrintReportRequest request =
                new PrintReportRequest()
                {
                    ContextName = this.SearchContext,
                    SearchFilters = this.CreateSearchFilters(),
                    UserInfo = userInfo
                };

            ReportingUtils.PrintRequest = request;

            Page.ClientScript.RegisterStartupScript(this.GetType(), "openReport", ReportingUtils.GetOpenReportPageScript(false), true);

        }

        /// <summary>
        /// Css da associare ai pulsanti cerca ed esporta
        /// </summary>
        public String ButtonCss { get; set; }

        /// <summary>
        /// Evento da lanciare al click sul pulsante Cerca
        /// </summary>
        public event EventHandler Search;

        /// <summary>
        /// Enumerazione dei possibili profili utente che possono utilizzare questo controllo
        /// </summary>
        public enum UserTypeEnum
        {
            User,
            Administrator
        }

        /// <summary>
        /// Contesto in cui è richiamata la ricerca
        /// </summary>
        [DefaultValue(UserTypeEnum.User)]
        public UserTypeEnum UserType { get; set; }

        /// <summary>
        /// Informazioni sul risultato della ricerca
        /// </summary>
        public String SearchResult
        {
            set
            {
                this.lblNoResult.Text = value;
                this.pnlNoResult.Visible = !String.IsNullOrEmpty(value.Trim());
            }
        }

        /// <summary>
        /// Contesto di ricerca da utilizzare per la generazione della reportistica
        /// </summary>
        public String SearchContext { get; set; }



    }
}
