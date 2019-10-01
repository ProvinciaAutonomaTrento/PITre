using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using System.ComponentModel;
using NttDataWA.Utils;
using System.Web;
using System.Data;


namespace NttDataWA.UserControls
{
    public partial class PannelloRicercaModelliTrasmissione : System.Web.UI.UserControl
    {
        static DataSet reportDataSet = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.txtVisibilitaCodice.Text))
                this.txtVisibilitaDescrizione.Text = String.Empty;
            if (String.IsNullOrEmpty(this.txtDestinatariCodice.Text))
                this.txtDestinatariDescrizione.Text = String.Empty;


            // Inizializzazione della sezione di ricerca
            if (!IsPostBack)
            {
                InitializeLanguage();
                this.InitializeSearchSection();
            }

            //// Compilazione dei controlli con codice e descrizione se da impostare
            //// se è presente in sessione CodeTextBox
            //if (Session["CodeTextBox"] != null)
            //    this.CompileCodeAndDescription();

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

        public RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_PROTO_IN;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }

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
                string msg = "WarningModelliNoCorrispondente";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                
                this.txtVisibilitaCodice.Text = String.Empty;
                this.txtVisibilitaDescrizione.Text = String.Empty;
            }
        }

        protected void imgRubricaVisibilita_Click(object sender, ImageClickEventArgs e)
        {
            Registro reg = (NttDataWA.DocsPaWR.Registro)Session["userRegistro"];
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
                string msg = "WarningModelliNoCorrispondente";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                
                this.txtDestinatariCodice.Text = String.Empty;
                this.txtDestinatariDescrizione.Text = String.Empty;
            }
        }

        protected void imgRubricaDestinatari_Click(object sender, ImageClickEventArgs e)
        {
            // Registrazione script per l'apertura della rubrica e salvataggio in sessione di un valore che
            // consenta di capire che alla chiusura della finestra della rubrica deve essere compilato il
            // codice e la descrizione per i destinatari
            

            Session["CodeTextBox"] = this.txtDestinatariCodice.ID;

            try
            {
                this.CallType = RubricaCallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI;
                HttpContext.Current.Session["AddressBook.from"] = "F_X_X_S";
                
                HttpContext.Current.Session["fromModelli"] = true;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AddressBook", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
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
                qco.tipoIE = NttDataWA.DocsPaWR.AddressbookTipoUtente.INTERNO;
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

           
            
            String idAmm = UserManager.GetInfoUser().idAmministrazione;

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
            RagioneTrasmissione[] rag = NttDataWA.UIManager.ModelliTrasmManager.getlistRagioniTrasm(idAmm, false, String.Empty);
            foreach (RagioneTrasmissione item in rag)
                this.ddlRagioniTrasmissione.Items.Add(new ListItem(item.descrizione, item.systemId));

            

            // Caricamento dei registri
            //this.LoadRegistryInformation();

        }

        

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
           
            this.imgSrcDestinatari.AlternateText = Utils.Languages.GetLabelFromCode("imgSrcDestinatari", language);
            this.imgSrcDestinatari.ToolTip = Utils.Languages.GetLabelFromCode("imgSrcDestinatari", language);
            this.imgRubricaDestinatari.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgCustomCorrespondentAddressBookCustom", language);
            this.imgRubricaDestinatari.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgCustomCorrespondentAddressBookCustom", language);
            this.ddlTipoTrasmissione.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language));
            this.ddlRegistri.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language));
            this.ddlRagioniTrasmissione.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language));
            this.ddlRegistry.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language));
                
        }

        /// <summary>
        /// Metodo per il caricamento dei registri
        /// </summary>
        //private void LoadRegistryInformation()
        //{
        //    // Se ci si trova in amministrazione, vengono caricati tutti i registri dell'amministrazione
        //    if (Session["AMMDATASET"] != null)
        //    {
        //        string[] amm = ((string)Session["AMMDATASET"]).Split('@');
        //        string codAmm = amm[0];

        //        OrgRegistro[] registri = UserManager.getRegistriByCodAmm(codAmm, "0");

        //        foreach (var reg in registri)
        //            this.ddlRegistry.Items.Add(new ListItem(reg.Descrizione, reg.IDRegistro));

        //    }
        //    else
        //    {
        //        // Vengono caricati i soli registri ed RF per il ruolo dell'utente
        //        Registro[] registri = UserManager.GetRegistriByRuolo(this.Page, RoleManager.GetRoleInSession().systemId);

        //        foreach (var reg in registri)
        //            this.ddlRegistry.Items.Add(new ListItem(reg.descrizione, reg.systemId));

        //    }

        //    this.ddlRegistry.SelectedIndex = 0;

        //}


        ///// <summary>
        ///// Funzione per la compilazione delle caselle di codice e descrizione 
        ///// </summary>
        //private void CompileCodeAndDescription()
        //{
        //    try
        //    {

        //        List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];

        //        if (HttpContext.Current.Session["AddressBook.from"] != null)
        //        {
        //            string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();

        //            switch (addressBookCallFrom)
        //            {
        //                case "F_X_X_S":
        //                    if (atList != null && atList.Count > 0)
        //                    {
        //                        NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
        //                        Corrispondente tempCorrSingle;
        //                        if (!corrInSess.isRubricaComune)
        //                            tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
        //                        else
        //                            tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

        //                        this.txtDestinatariCodice.Text = tempCorrSingle.codiceRubrica;
        //                        this.txtDestinatariDescrizione.Text = tempCorrSingle.descrizione;
        //                        HttpContext.Current.Session["AddressBook.At"] = null;
        //                        HttpContext.Current.Session["AddressBook.Cc"] = null;
        //                        // Pulizia sessione
        //                        Session.Remove("CodeTextBox");
        //                        Session.Remove("selMittDaRubrica");
        //                        Session.Remove("SelDescFindModelli");
                                
        //                    }
        //                    else
        //                    {
        //                        // Pulizia sessione
        //                        Session.Remove("CodeTextBox");
        //                        Session.Remove("selMittDaRubrica");
        //                        Session.Remove("SelDescFindModelli");
        //                    }
        //                    break;

        //            }


        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        UIManager.AdministrationManager.DiagnosticError(ex);
        //        return;
        //    }

            

        //}

    


        private PrintReportRequestDataset RequestPrintReport
        {
            get
            {
                if (HttpContext.Current.Session["requestPrintReport"] != null)
                    return (PrintReportRequestDataset)HttpContext.Current.Session["requestPrintReport"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["requestPrintReport"] = value;
            }
        }


        public void RefreshCorrespondent()
        {
            this.UpPnlSearchTransmission.Update();
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

        public string destCodice
        {
            set
            {
                this.txtDestinatariCodice.Text = value;
            }
        }

        public string destDescrizione
        {
            set
            {
                this.txtDestinatariDescrizione.Text = value;
            }
        }

        public string idDestinatario
        {
            set
            {
                this.IdDestinatario.Value = value;
            }
        }

        /// <summary>
        /// Contesto di ricerca da utilizzare per la generazione della reportistica
        /// </summary>
        public String SearchContext { get; set; }



    }
}
