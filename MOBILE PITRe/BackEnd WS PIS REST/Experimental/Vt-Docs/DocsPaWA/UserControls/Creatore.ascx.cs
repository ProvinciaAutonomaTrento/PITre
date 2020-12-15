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
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.UserControls
{
    /// <summary>
    /// Usercontrol per la proposta dei filtri nella ricerca delle trasmissioni
    /// </summary>
    public partial class Creatore : System.Web.UI.UserControl
    {
        /// <summary>
        /// Chiave di sessione relativa al mantenimento dei filtri in sessione immessi
        /// nei campi della UI
        /// </summary>
        public const string CURRENT_UI_FILTERS_SESSION_KEY = "Creatore.UIData";

        /// <summary>
        /// 
        /// </summary>
        private const string RANGE_FILTER_TYPE_INTERVAL = "I";
        private const string RANGE_FILTER_TYPE_SINGLE = "S";
        private bool optListCreatorChanged = false;
        private bool first = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Expires = -1;

            if (!this.IsPostBack)
            {
                this.AddControlsClientAttribute();

                // Azione di selezione del tipo di creatore
                 this.PerformActionSelectTipoCreatore();

                // this.RemoveCurrentFilters();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                //this.RemoveCurrentFilters();
                DocsPaWR.Corrispondente selectedCorr = UserManager.getCreatoreSelezionato(this.Page);
                if (selectedCorr != null)
                {
                    if (!optListCreatorChanged)
                    {
                        if (selectedCorr.tipoCorrispondente.Equals("P"))
                        {
                            DocsPaWR.Utente ut = (DocsPaWR.Utente)selectedCorr;
                            this.txtSystemIdUtenteCreatore.Value = ut.idPeople;
                        }
                        else
                            this.txtSystemIdUtenteCreatore.Value = selectedCorr.systemId;
                        this.txtCodiceUtenteCreatore.Text = selectedCorr.codiceRubrica;
                        this.txtDescrizioneUtenteCreatore.Text = selectedCorr.descrizione;
                        this.optListTipiCreatore.SelectedValue = selectedCorr.tipoCorrispondente;
                    }

                    this.PersistCurrentFilters();
                }
            }
        }

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
            DocsPaWR.Corrispondente selectedCorr = UserManager.getCreatoreSelezionato(this.Page);
            if (selectedCorr != null)
            {
                switch (selectedCorr.tipoCorrispondente)
                {
                    case "P":
                        DocsPaWR.Utente ut=(DocsPaWR.Utente)selectedCorr; 
                        this.txtSystemIdUtenteCreatore.Value = ut.idPeople;
                        this.txtCodiceUtenteCreatore.Text = ut.codiceRubrica;
                        this.txtDescrizioneUtenteCreatore.Text = ut.descrizione;
                    break;

                    default:
                        this.txtSystemIdUtenteCreatore.Value = selectedCorr.systemId;
                        this.txtCodiceUtenteCreatore.Text = selectedCorr.codiceRubrica;
                        this.txtDescrizioneUtenteCreatore.Text = selectedCorr.descrizione;
                    break;
                }
                selectedCorr = null;
            }
            else
            {
                this.txtSystemIdUtenteCreatore.Value = string.Empty;
                this.txtCodiceUtenteCreatore.Text = string.Empty;
                this.txtDescrizioneUtenteCreatore.Text = string.Empty;
            }
        }

        protected void optListTipiCreatore_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PerformActionSelectTipoCreatore();
            this.optListCreatorChanged = true;
            this.txtCodiceUtenteCreatore.Text = "";
            this.txtDescrizioneUtenteCreatore.Text = "";
        }

        /// <summary>
        /// Azione di selezione del tipo di mittente richiesto
        /// </summary>
        private void PerformActionSelectTipoCreatore()
        {
            this.txtTipoCorrispondente.Value = this.optListTipiCreatore.SelectedItem.Value;
            //this.txtCodiceUtenteCreatore.Text = "";
            //this.txtDescrizioneUtenteCreatore.Text = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtCodiceUtenteCreatore_TextChanged(object sender, EventArgs e)
        {
            string codiceCreat = this.txtCodiceUtenteCreatore.Text;
            DocsPaWR.Corrispondente corrispondente = null;

            if (codiceCreat != string.Empty)
            {
                // Reperimento oggetto corrispondente dal codice immesso dall'utente
                corrispondente = this.GetCorrispondenteDaCodice(codiceCreat);

                if (corrispondente == null)
                {
                    this.RegisterClientScript("CodiceRubricaNonTrovato", "alert('Codice rubrica non trovato');");
                    UserManager.removeCreatoreSelezionato(this.Page);
                }
                else
                {
                    UserManager.setCreatoreSelezionato(this.Page, corrispondente);

                    // Impostazione del tipo corrispondente corretto
                    if (corrispondente.GetType().Equals(typeof(DocsPaWR.Utente)))
                        this.optListTipiCreatore.SelectedValue = "P";
                    else if (corrispondente.GetType().Equals(typeof(DocsPaWR.Ruolo)))
                        this.optListTipiCreatore.SelectedValue = "R";
                    else if (corrispondente.GetType().Equals(typeof(DocsPaWR.UnitaOrganizzativa)))
                        this.optListTipiCreatore.SelectedValue = "U";
                }
                this.txtCodiceUtenteCreatore.Focus();

                // Caricamento dati corrispondente
                this.FillDatiCorrispondenteDaRubrica();
            }
            else
            {
                this.txtCodiceUtenteCreatore.Text = "";
                this.txtDescrizioneUtenteCreatore.Text = "";
                UserManager.removeCreatoreSelezionato(this.Page);
           
            }
        }
        

        /// <summary>
        /// Reperimento di un corrispondente in base ad un codice rubrica fornito in ingresso
        /// </summary>
        /// <param name="page"></param>
        /// <param name="codCorrispondente"></param>
        /// <returns></returns>
        private DocsPaWR.Corrispondente GetCorrispondenteDaCodice(string codCorrispondente)
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

            // Filtri per creatore
            this.AddFilterCreatore(filterItems);

            return (DocsPaWR.FiltroRicerca[]) filterItems.ToArray(typeof(DocsPaWR.FiltroRicerca));
        }

        public DocsPaWR.FiltroRicerca GetFilter()
        {
            FiltroRicerca filterItem = new FiltroRicerca();

            // Filtri per creatore
            filterItem = this.AddFilterCreatore();

            return filterItem;
        }


        /// <summary>
        /// Creazione filtro su creatore
        /// </summary>
        /// <param name="filterItem"></param>
        private FiltroRicerca AddFilterCreatore()
        {
            FiltroRicerca filter = null;

            if (!string.IsNullOrEmpty(this.txtCodiceUtenteCreatore.Text))
            {
                filter = new FiltroRicerca();
                filter.argomento = this.GetArgomento(true, this.optListTipiCreatore.SelectedValue);
                filter.valore = this.txtSystemIdUtenteCreatore.Value.Trim();
            }
            else if (!string.IsNullOrEmpty(this.txtDescrizioneUtenteCreatore.Text))
            {
                filter = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filter.argomento = this.GetArgomento(false, this.optListTipiCreatore.SelectedValue);
                filter.valore = this.txtDescrizioneUtenteCreatore.Text.Trim();
            }
            UserManager.removeCreatoreSelezionato(this.Page);
            return filter;
        }


        /// <summary>
        /// Creazione filtro su creatore
        /// </summary>
        /// <param name="filterItem"></param>
        private void AddFilterCreatore(ArrayList filterItem)
        {
            FiltroRicerca filter = null;

            if (!string.IsNullOrEmpty(this.txtCodiceUtenteCreatore.Text))
            {
                filter = new FiltroRicerca();
                filter.argomento = this.GetArgomento(true, this.optListTipiCreatore.SelectedValue);
                if (!this.optListTipiCreatore.SelectedValue.Equals("U"))
                    filter.valore = this.txtCodiceUtenteCreatore.Text.Trim();
                else
                    filter.valore = this.txtSystemIdUtenteCreatore.Value.Trim();                 
            }
            else if (!string.IsNullOrEmpty(this.txtDescrizioneUtenteCreatore.Text))
            {
                filter = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filter.argomento = this.GetArgomento(false, this.optListTipiCreatore.SelectedValue);
                filter.valore = this.txtDescrizioneUtenteCreatore.Text.Trim();
            }

            if (filter!=null)
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
                        argomento = DocsPAWA.DocsPaWR.FiltriDocumento.ID_RUOLO_CREATORE.ToString();
                    else
                        argomento = DocsPAWA.DocsPaWR.FiltriDocumento.DESC_RUOLO_CREATORE.ToString();
                    break;

                case "P":
                    if (codRubrica)
                        argomento = DocsPAWA.DocsPaWR.FiltriDocumento.ID_PEOPLE_CREATORE.ToString();
                    else
                        argomento = DocsPAWA.DocsPaWR.FiltriDocumento.DESC_PEOPLE_CREATORE.ToString();
                    break;

                case "U":
                    if (codRubrica)
                        argomento = DocsPAWA.DocsPaWR.FiltriDocumento.ID_UO_CREATORE.ToString();
                    else
                        argomento = DocsPAWA.DocsPaWR.FiltriDocumento.DESC_UO_CREATORE.ToString();
                    break;
            }

            return argomento;
        }

        #endregion

        #region Gestione memorizzazione / ripristino dati di filtro in sessione per la UI

        /// <summary>
        /// Rimozione filtri in sessione
        /// </summary>
        public void RemoveCurrentFilters()
        {
            if (HttpContext.Current.Session[UserControls.Creatore.CURRENT_UI_FILTERS_SESSION_KEY] != null)
                HttpContext.Current.Session.Remove(UserControls.Creatore.CURRENT_UI_FILTERS_SESSION_KEY);
        }

        /// <summary>
        /// Persistenza filtri immessi nei campi della UI
        /// </summary>
        public void PersistCurrentFilters()
        {
            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.optListTipiCreatore.ClientID, this.optListTipiCreatore.SelectedItem.Value);
            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.txtSystemIdUtenteCreatore.ClientID, this.txtSystemIdUtenteCreatore.Value);
            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.txtCodiceUtenteCreatore.ClientID, this.txtCodiceUtenteCreatore.Text);
            UIFiltersDataStorage.SetData(CURRENT_UI_FILTERS_SESSION_KEY, this.txtDescrizioneUtenteCreatore.ClientID, this.txtDescrizioneUtenteCreatore.Text);
        }

        /// <summary>
        /// Ripristino filtri nei campi della UI
        /// </summary>
        public void RestoreCurrentFilters()
        {
            if (UIFiltersDataStorage.ContainsData(CURRENT_UI_FILTERS_SESSION_KEY, this.optListTipiCreatore.ClientID))
                this.optListTipiCreatore.SelectedValue = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.optListTipiCreatore.ClientID);
            this.txtSystemIdUtenteCreatore.Value = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.txtSystemIdUtenteCreatore.ClientID);
            this.txtCodiceUtenteCreatore.Text = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.txtCodiceUtenteCreatore.ClientID);
            this.txtDescrizioneUtenteCreatore.Text = UIFiltersDataStorage.GetData(CURRENT_UI_FILTERS_SESSION_KEY, this.txtDescrizioneUtenteCreatore.ClientID);
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
        
        public string ID_CREATORE
        {
            get
            {
                return txtSystemIdUtenteCreatore.ToString();
            }
            set
            {
                txtSystemIdUtenteCreatore.Value = value;
            }
        }

        public string CODICE_CREATORE
        {
            get
            {
                return txtCodiceUtenteCreatore.Text;
            }
            set
            {
                txtCodiceUtenteCreatore.Text = value;
            }
        }

        public string DESC_CREATORE
        {
            get
            {
                return txtDescrizioneUtenteCreatore.Text;
            }
            set
            {
                txtDescrizioneUtenteCreatore.Text = value;
            }
        }

        public void Clear()
        {
            this.txtCodiceUtenteCreatore.Text = "";
            this.txtDescrizioneUtenteCreatore.Text = "";
            UserManager.removeCreatoreSelezionato(this.Page);
        }

    }
}
