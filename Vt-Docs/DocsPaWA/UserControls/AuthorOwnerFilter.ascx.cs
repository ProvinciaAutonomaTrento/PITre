using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.SiteNavigation;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.UserControls
{
    public partial class AuthorOwnerFilter : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.btnShowAddressBook.OnClientClick = String.Format("javascript:OpenAddressBook('{0}', '{1}');", this.rblCorrType.ClientID, this.ClientID);

            // Ripristino dei dati memorizzati
            if (!IsPostBack)
                this.RestoreFields();

            // Ripristino dei dati se presenti
            if (Session["CorrForOwnerAuthor"] != null && Session["ControlSelected"] != null && Session["ControlSelected"].ToString().Equals(this.ClientID))
            {
                Session["ControlSelected"] = null;
                this.SelectedCorr = Session["CorrForOwnerAuthor"] as DocsPaWR.Corrispondente;
                Session.Remove("CorrForOwnerAuthor");
                this.FillDatiCorrispondenteDaRubrica();
            }
        }

        /// <summary>
        /// Al cambio del testo viene risolto il codice immesso
        /// </summary>
        protected void txtCodiceUtenteCreatore_TextChanged(object sender, EventArgs e)
        {
            String corrCode = this.txtCorrCode.Text;
            DocsPaWR.Corrispondente corr = null;

            if (!String.IsNullOrEmpty(corrCode))
            {
                // Reperimento oggetto corrispondente dal codice immesso dall'utente
                corr = this.GetCorrispondenteDaCodice(corrCode);

                if (corr == null)
                {
                    this.Page.ClientScript.RegisterStartupScript(
                        this.GetType(),
                        "CodiceRubricaNonTrovato",
                        "alert('Codice rubrica non trovato');",
                        true);
                    this.SelectedCorr = null;
                }
                else
                {
                    this.SelectedCorr = corr;

                    if (corr.GetType().Equals(typeof(DocsPaWR.Utente)))
                        this.rblCorrType.SelectedValue = "P";
                    else if (corr.GetType().Equals(typeof(DocsPaWR.Ruolo)))
                        this.rblCorrType.SelectedValue = "R";
                    else if (corr.GetType().Equals(typeof(DocsPaWR.UnitaOrganizzativa)))
                        this.rblCorrType.SelectedValue = "U";
                }
                this.txtCorrCode.Focus();

                this.FillDatiCorrispondenteDaRubrica();
            }
            else
            {
                this.txtCorrCode.Text = "";
                this.txtCorrDescription.Text = "";

                this.SelectedCorr = null;

            }
        }

        protected void btnShowAddressBook_Click(object sender, ImageClickEventArgs e)
        {
            this.FillDatiCorrispondenteDaRubrica();
        }

        /// <summary>
        /// Al cambio della selezione, viene abilitato / disabilitato il checkbox per l'estensione
        /// della ricerca agli storicizzati
        /// </summary>
        protected void rblCorrType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PerformSelectionChanged();

        }

        #region Control properties

        /// <summary>
        /// Enumerazione delle possibili configurazioni del controllo
        /// </summary>
        public enum AuthorOwnerFilterType
        {
            /// <summary>
            /// Ricerca per proprietario
            /// </summary>
            Owner,
            /// <summary>
            /// Ricerca per autore
            /// </summary>
            Author
        }

        /// <summary>
        /// Configurazione del controllo da utilizzare
        /// </summary>
        private AuthorOwnerFilterType _authorOwnerFilterType = AuthorOwnerFilterType.Author;
        public AuthorOwnerFilterType ControlType
        {
            get
            {
                return this._authorOwnerFilterType;
            }

            set
            {
                this._authorOwnerFilterType = value;
                this.chkExtendToHistoricized.Checked = false;
                switch (this._authorOwnerFilterType)
                {
                    case AuthorOwnerFilterType.Owner:
                        this.lblLabel.Text = "Proprietario:";
                        this.chkExtendToHistoricized.Visible = false;
                        this.rblCorrType.Items[0].Enabled = false;
                        break;
                    case AuthorOwnerFilterType.Author:
                        this.lblLabel.Text = "Creatore:";
                        this.chkExtendToHistoricized.Visible = true;
                        this.rblCorrType.Items[0].Enabled = true;
                        break;
                    default:
                        this.lblLabel.Text = "Creatore:";
                        this.chkExtendToHistoricized.Visible = true;
                        break;
                }
            }
        }

        #endregion

        #region Metodi di supporto

        /// <summary>
        /// Caricamento dati corrispondente selezionato dalla rubrica
        /// </summary>
        private void FillDatiCorrispondenteDaRubrica()
        {
            DocsPaWR.Corrispondente selectedCorr = this.SelectedCorr;
            if (selectedCorr != null)
            {
                this.txtCorrCode.Text = selectedCorr.codiceRubrica;
                this.txtCorrDescription.Text = selectedCorr.descrizione;

                switch (selectedCorr.tipoCorrispondente)
                {
                    case "P":
                        this.rblCorrType.SelectedValue = "P";
                        break;
                    case "R":
                        this.rblCorrType.SelectedValue = "R";
                        break;
                    case "U":
                        this.rblCorrType.SelectedValue = "U";
                        break;
                    default:
                        this.rblCorrType.SelectedValue = "U";
                        break;
                }
                selectedCorr = null;
                this.PerformSelectionChanged();
            }
            else
            {
                this.txtCorrCode.Text = string.Empty;
                this.txtCorrDescription.Text = string.Empty;
            }
        }

        /// <summary>
        /// Reperimento di un corrispondente in base ad un codice rubrica fornito in ingresso
        /// </summary>
        private DocsPaWR.Corrispondente GetCorrispondenteDaCodice(String corrCode)
        {
            DocsPaWR.Corrispondente retValue = null;

            if (!String.IsNullOrEmpty(corrCode))
                retValue = UserManager.getCorrispondente(this.Page, corrCode, true);

            return retValue;
        }

        /// <summary>
        /// Metodo per la creazione della lista filtri relativa alla ricerca per proprietario
        /// </summary>
        /// <returns>Lista dei filtri per ricerca per proprietario</returns>
        public List<FiltroRicerca> GetOwnerFilters()
        {
            List<FiltroRicerca> retCollection = new List<FiltroRicerca>();

            if (!String.IsNullOrEmpty(this.txtCorrCode.Text))
                retCollection.Add(new FiltroRicerca() { argomento = "ID_OWNER", valore = this.SelectedCorr.systemId });
            else if (!String.IsNullOrEmpty(this.txtCorrDescription.Text))
                retCollection.Add(new FiltroRicerca() { argomento = "DESC_OWNER", valore = this.txtCorrDescription.Text });

            retCollection.Add(new FiltroRicerca() { argomento = "EXTEND_TO_HISTORICIZED_OWNER", valore = this.chkExtendToHistoricized.Checked.ToString() });

            retCollection.Add(new FiltroRicerca() { argomento = "CORR_TYPE_OWNER", valore = this.rblCorrType.SelectedValue });

            return retCollection;
        }

        /// <summary>
        /// Metodo per la creazione della lista filtri relativa alla ricerca per autore
        /// </summary>
        /// <returns>Lista dei filtri per ricerca per autore</returns>
        public List<FiltroRicerca> GetAuthorFilters()
        {
            List<FiltroRicerca> retCollection = new List<FiltroRicerca>();

            if (!String.IsNullOrEmpty(this.txtCorrCode.Text))
                retCollection.Add(new FiltroRicerca() { argomento = "ID_AUTHOR", valore = this.SelectedCorr.systemId });
            else if (!String.IsNullOrEmpty(this.txtCorrDescription.Text))
                retCollection.Add(new FiltroRicerca() { argomento = "DESC_AUTHOR", valore = this.txtCorrDescription.Text });

            retCollection.Add(new FiltroRicerca() { argomento = "EXTEND_TO_HISTORICIZED_AUTHOR", valore = this.chkExtendToHistoricized.Checked.ToString() });

            retCollection.Add(new FiltroRicerca() { argomento = "CORR_TYPE_AUTHOR", valore = this.rblCorrType.SelectedValue });

            return retCollection;
        }

        /// <summary>
        /// Metodo per l'impostazione della maschera del controllo in base al valore selezionato
        /// nella lista degli option button
        /// </summary>
        private void PerformSelectionChanged()
        {
            this.chkExtendToHistoricized.Checked = false;
            if (this.rblCorrType.SelectedValue == "R")
                this.chkExtendToHistoricized.Enabled = true;
            else
                this.chkExtendToHistoricized.Enabled = false;
        }

        /// <summary>
        /// Metodo per il restore dei filtri memorizzati
        /// </summary>
        private void RestoreFields()
        {
            // Ripristino di codice e / o descrizione del corrispondente
            if (this.SelectedCorr != null)
            {
                this.txtCorrCode.Text = this.SelectedCorr.codiceRubrica;
                this.txtCorrDescription.Text = this.SelectedCorr.descrizione;
            }
            else
                this.txtCorrDescription.Text = this.SelectedCorrDescription;

            // Ripristino del radio button selezionato
            if (!String.IsNullOrEmpty(this.SelectedCorrType))
            {
                this.rblCorrType.SelectedValue = this.SelectedCorrType;
                this.PerformSelectionChanged();
            }

            // Ripristino dello stato di checking per l'estensione della ricerca agli storicizzati
            this.chkExtendToHistoricized.Checked = this.ExtendSearchToHistoricized;

        }

        #endregion

        #region Variabili di call context

        /// <summary>
        /// Corrispondente selezionato
        /// </summary>
        public DocsPaWR.Corrispondente SelectedCorr
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState[this.ClientID] as DocsPaWR.Corrispondente;
            }
            set
            {
                CallContextStack.CurrentContext.ContextState[this.ClientID] = value;
            }
        }

        public bool ExtendSearchToHistoricized
        {
            get
            {
                bool retVal = false;
                if (CallContextStack.CurrentContext.ContextState[this.ClientID + "_h"] != null)
                    retVal = Convert.ToBoolean(CallContextStack.CurrentContext.ContextState[this.ClientID + "_h"]);

                return retVal;
            }
            set
            {
                CallContextStack.CurrentContext.ContextState[this.ClientID + "_h"] = value;
            }
        }

        public String SelectedCorrDescription
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState[this.ClientID + "_d"] as String;
            }
            set
            {
                CallContextStack.CurrentContext.ContextState[this.ClientID + "_d"] = value;
            }

        }

        public String SelectedCorrType
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState[this.ClientID + "_c"] as String;
            }
            set
            {
                CallContextStack.CurrentContext.ContextState[this.ClientID + "_c"] = value;
            }

        }

        #endregion

        #region Metodo accedibili per l'interscambio dati

        /// <summary>
        /// Metodo per la costruzione dei parametri di ricerca
        /// </summary>
        /// <returns>Lista dei filtri</returns>
        public List<FiltroRicerca> GetFiltersList()
        {
            List<FiltroRicerca> filters = new List<FiltroRicerca>();

            switch (this.ControlType)
            {
                case AuthorOwnerFilterType.Owner:
                    filters = this.GetOwnerFilters();
                    break;
                case AuthorOwnerFilterType.Author:
                    filters = this.GetAuthorFilters();
                    break;
            }

            return filters;

        }

        /// <summary>
        /// Metodo da richiamare per l'annullamento dei filtri impostati
        /// </summary>
        public void DeleteFilters()
        {
            this.txtCorrCode.Text = string.Empty;
            this.txtCorrDescription.Text = string.Empty;
            this.chkExtendToHistoricized.Checked = false;
        }

        /// <summary>
        /// Metodo da richiamare per salvare i filtri 
        /// </summary>
        public void SaveFilters()
        {
            // Se la casella del codice del corrispondente è vuota ma è popolata
            // quella della descrizione, viene memorizzata la descrizione
            if (String.IsNullOrEmpty(this.txtCorrCode.Text) && !String.IsNullOrEmpty(this.txtCorrDescription.Text))
                this.SelectedCorrDescription = this.txtCorrDescription.Text;

            // Memorizzazione dello stato di flagging del checkbox per l'estensione
            // della ricerca agli storicizzati
            this.ExtendSearchToHistoricized = this.chkExtendToHistoricized.Checked;

            // Salvataggio del radio button selezionato
            this.SelectedCorrType = this.rblCorrType.SelectedValue;

        }

        /// <summary>
        /// Pulizia dei filtri
        /// </summary>
        public void ClearFilters()
        {
            // Pulizia dei filtri
            this.SelectedCorrDescription = String.Empty;
            this.ExtendSearchToHistoricized = false;
            this.chkExtendToHistoricized.Checked = false;
            this.SelectedCorr = null;

            this.txtCorrDescription.Text = String.Empty;
            this.txtCorrCode.Text = String.Empty;

        }

        #endregion




    }
}
