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
using DocsPAWA.SiteNavigation;
using System.Linq;
using DocsPAWA.utils;

namespace DocsPAWA.documento
{
	/// <summary>
	/// Summary description for docAllegati.
	/// </summary>
	public class docAllegati : DocsPAWA.CssPage
	{
        protected System.Web.UI.WebControls.DataGrid grdAllegati;
		protected DocsPaWebCtrlLibrary.ImageButton btn_rimuoviAlleg;
		protected DocsPaWebCtrlLibrary.ImageButton btn_sostituisciDocPrinc;
		protected DocsPaWebCtrlLibrary.ImageButton btn_aggiungiAreaLav;
		protected DocsPaWebCtrlLibrary.ImageButton btn_modifAlleg;
		protected DocsPaWebCtrlLibrary.ImageButton btn_aggAlleg;
		protected System.Web.UI.WebControls.Label lbl_message;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtSavedVersionId;
        protected System.Web.UI.HtmlControls.HtmlLink styleLink;
        protected System.Web.UI.WebControls.RadioButtonList rblFilter;
            /// <summary>
        /// 
        /// </summary>
		private DocsPAWA.DocsPaWR.SchedaDocumento _schedaDocumento = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			Utils.startUp(this);
			
			this._schedaDocumento = (DocsPAWA.DocsPaWR.SchedaDocumento) DocumentManager.getDocumentoSelezionato(this);

            Session.Remove("refreshDxPageVisualizzatore");

            // Inizializzazione controllo verifica acl
            if ((this.SchedaDocumento != null) && 
                (this.SchedaDocumento.inCestino != "1") && 
                (this.SchedaDocumento.systemId != null))
            {
                this.InitializeControlAclDocumento();
            }

			if (!this.Page.IsPostBack)
			{
                // Reperimento VersionId dell'allegato precedentemente selezionato
                string versionId = this.PopCurrentIdOnContext();

                this.SelectPageAllegato(versionId);

				this.BindGrid();

                // Azione di selezione dell'allegato modificato
                this.PerformSelectionAllegato(versionId);
			}
		}

        private string GetCssAmministrazione()
        {
            string Tema = string.Empty;
            if ((string)Session["AMMDATASET"] != null)
            {
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                DocsPAWA.UserManager userM = new UserManager();
                Tema = userM.getCssAmministrazione(idAmm);
            }
            else
            {
                if (UserManager.getInfoUtente() != null)
                {
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    DocsPAWA.UserManager userM = new UserManager();
                    Tema = userM.getCssAmministrazione(idAmm);
                }
            }
            return Tema;
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsBack
        {
            get
            {
                return SiteNavigation.CallContextStack.CurrentContext.IsBack;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_PreRender(object sender, System.EventArgs e)
        {
            string Tema = GetCssAmministrazione();

            if (Tema != null && !Tema.Equals(""))
            {
                string[] realTema = Tema.Split('^');
                this.styleLink.Href = "../App_Themes/" + realTema[0] + "/" + realTema[0] + ".css";
            }
            else
                this.styleLink.Href = "../App_Themes/TemaRosso/TemaRosso.css";

            // Impostazione visibilità colonne datagrid
            this.SetGridColumnsVisibility();

            // Gestione abilitazione / disabilitazione pulsanti
            this.EnableButtons();

            // Impostazione visibilità griglia
            this.grdAllegati.Visible = (this.SchedaDocumento.allegati.Length > 0);
            this.lbl_message.Visible = (!this.grdAllegati.Visible);
            if (!Page.IsPostBack)
            {
                string filterExternalSystem = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_FILTRO_ALLEGATI_ESTERNI");
                if (string.IsNullOrEmpty(filterExternalSystem) || (!filterExternalSystem.Equals("1")))
                {
                    foreach (ListItem f in rblFilter.Items)
                    {
                        if (f.Value.Equals("esterni"))
                        {
                            rblFilter.Items.Remove(f);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Impostazione visibilità colonne datagrid
        /// </summary>
        private void SetGridColumnsVisibility()
        {
            this.grdAllegati.Columns[COL_NAVIGATE_TO_ALLEGATO].Visible = this.IsEnabledProfilazioneAllegato;
        }

        /// <summary>
        /// 
        /// </summary>
        private void DisableButtonsPerConsolidation()
        {
            this.btn_modifAlleg.Enabled = false;
            this.btn_rimuoviAlleg.Enabled = false;
            this.btn_sostituisciDocPrinc.Enabled = false;
            this.btn_aggAlleg.Enabled = false;
        }

        /// <summary>
        /// Disabilitazione di tutti i pulsanti
        /// </summary>
        private void DisableAllButtons()
        {
            this.btn_modifAlleg.Enabled = false;
            this.btn_rimuoviAlleg.Enabled = false;
            this.btn_sostituisciDocPrinc.Enabled = false;
            this.btn_aggiungiAreaLav.Enabled = false;
            this.btn_aggAlleg.Enabled = false;
        }

        /// <summary>
        /// Gestione abilitazione / disabilitazione pulsanti
        /// </summary>
        protected virtual void EnableButtons()
        {
            this.DisableAllButtons();

            if ((!this.DocumentoInCestino && !this.ProtocolloAnnullato) || 
                (SchedaDocumento.inArchivio != null && SchedaDocumento.inArchivio != "1"))
            {
                // Se il documento non è in cestino

                // L'inserimento dell'allegato è sempre consentito
                this.btn_aggAlleg.Enabled = true;

                this.btn_aggiungiAreaLav.Enabled = (this.SchedaDocumento != null && this.SchedaDocumento.systemId != null);

                this.btn_sostituisciDocPrinc.Enabled = (!string.IsNullOrEmpty(this.SchedaDocumento.systemId) && (this.SchedaDocumento.protocollo == null || (this.SchedaDocumento.protocollo != null && string.IsNullOrEmpty(this.SchedaDocumento.protocollo.numero))) && !CheckInOut.CheckInOutServices.IsCheckedOutDocument(this.SchedaDocumento.systemId, this.SchedaDocumento.docNumber, UserManager.getInfoUtente(), true));

                if (this.grdAllegati.SelectedIndex > -1)
                {
                    this.btn_modifAlleg.Enabled = true;

                    this.btn_rimuoviAlleg.Enabled = !(this.SchedaDocumento.protocollo != null && !string.IsNullOrEmpty(this.SchedaDocumento.protocollo.segnatura));
                }
            }

            this.verificaHMdiritti();

            //abilitazione delle funzioni in base al ruolo
            UserManager.disabilitaFunzNonAutorizzate(this);

            if(UserManager.isFiltroAooEnabled(this))
            {
                if (this.SchedaDocumento != null && this.SchedaDocumento.protocollo != null)
                {
                    DocsPaWR.Registro[] userRegistri = DocsPAWA.UserManager.getListaRegistri(this.Page);
                    if (btn_aggAlleg.Enabled)
                    {
                        btn_aggAlleg.Enabled = UserManager.verifyRegNoAOO(SchedaDocumento, userRegistri);
                    }

                    if (btn_modifAlleg.Enabled)
                    {
                        btn_modifAlleg.Enabled = UserManager.verifyRegNoAOO(SchedaDocumento, userRegistri);
                    }

                    if (btn_rimuoviAlleg.Enabled)
                    {
                        btn_rimuoviAlleg.Enabled = UserManager.verifyRegNoAOO(SchedaDocumento, userRegistri);
                    }

                    if(btn_sostituisciDocPrinc.Enabled)
                    {
                        btn_sostituisciDocPrinc.Enabled = UserManager.verifyRegNoAOO(SchedaDocumento, userRegistri);
                    }
                }
            }

            // Controllo su stato documento consolidato
            if (this.SchedaDocumento.ConsolidationState != null &&
                this.SchedaDocumento.ConsolidationState.State >= DocsPaWR.DocumentConsolidationStateEnum.Step1)
            {
                this.DisableButtonsPerConsolidation();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void verificaHMdiritti()
        {
            //disabilitazione dei bottoni in base all'autorizzazione di HM 
            //sul documento
            if (this.SchedaDocumento != null && !string.IsNullOrEmpty(this.SchedaDocumento.accessRights))
            {
                if (UserManager.disabilitaButtHMDiritti(this.SchedaDocumento.accessRights))
                {
                    // Bottoni che devono essere disabilitati in caso di diritti di sola lettura
                    this.DisableAllButtons();
                }
            }
        }

        /// <summary>
        /// Scheda documento corrente
        /// </summary>
        protected DocsPaWR.SchedaDocumento SchedaDocumento
        {
            get
            {
                return this._schedaDocumento;
            }
        }

        /// <summary>
        /// Verifica se il documento è in cestino
        /// </summary>
        protected bool DocumentoInCestino
        {
            get
            {
                return (!string.IsNullOrEmpty(this.SchedaDocumento.inCestino) && 
                    this.SchedaDocumento.inCestino == "1");
            }
        }

        /// <summary>
        /// Verifica se il protocollo è stato annullato
        /// </summary>
        protected bool ProtocolloAnnullato
        {
            get
            {
                bool retValue = false;
                
                if (this.SchedaDocumento.protocollo != null)
                {
                    DocsPaWR.ProtocolloAnnullato protAnnull = this.SchedaDocumento.protocollo.protocolloAnnullato;

                    retValue = (protAnnull != null && 
                        !string.IsNullOrEmpty(protAnnull.dataAnnullamento));
                }
                
                return retValue;
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
            this.grdAllegati.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.grdAllegati_pager);
			this.btn_aggAlleg.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aggAlleg_Click);
			this.btn_modifAlleg.Click += new System.Web.UI.ImageClickEventHandler(this.btn_modifAlleg_Click);
			this.btn_aggiungiAreaLav.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aggiungiAreaLav_Click);
			this.btn_sostituisciDocPrinc.Click += new System.Web.UI.ImageClickEventHandler(this.btn_sostituisciDocPrinc_Click);
			this.btn_rimuoviAlleg.Click += new System.Web.UI.ImageClickEventHandler(this.btn_rimuoviAlleg_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);
            this.rblFilter.SelectedIndexChanged += new EventHandler(rblFilter_SelectedIndexChanged); 

            // Se è attiva l'interoperabilità semplificata, viene aggiunta una voce che consente di
            // filtrare per ricevute Interoperabilità semplificata
            if(InteroperabilitaSemplificataManager.IsEnabledSimpInterop)
                this.rblFilter.Items.Add(
                    new ListItem(
                        InteroperabilitaSemplificataManager.SearchItemDescriprion, 
                        InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId));

            string labelDerivati = System.Configuration.ConfigurationManager.AppSettings["LABEL_ALLEGATI_DERIVATI"];
            if (!string.IsNullOrEmpty(labelDerivati))
            {
                this.rblFilter.Items.Add(
                    new ListItem(
                                  labelDerivati,
                                  "derivati"));
            }

		}
		#endregion

		#region Gestione DataGrid

        /// <summary>
        /// Costanti che identificano gli indici delle colonne del datagrid
        /// </summary>
        private const int COL_DOC_NUMBER = 0;
        private const int COL_VERSION_ID = 1;
        private const int COL_VERSION_LABEL = 2;
        private const int COL_DESCRIZIONE = 3;
        private const int COL_NUMERO_PAGINE = 4;
        private const int COL_NAVIGATE_TO_ALLEGATO = 6;
        private const int COL_SELECT_ALLEGATO = 7;

        /// <summary>
        /// Path delle icone visualizzate nell'imagebutton
        /// per la navigazione verso il dettaglio dell'allegato
        /// </summary>
        private const string NAVIGATE_ICON_PATH = "../images/proto/dett_lente.gif";
        private const string NAVIGATE_ICON_PATH_ACQUIRED = "../images/proto/dett_lente_doc.gif";

        /// <summary>
        /// Reperimento path dell'icona per la navigazione verso l'allegato
        /// </summary>
        /// <param name="allegato"></param>
        protected string GetNavigateIconPath(DocsPaWR.Allegato allegato)
        {
            string path = string.Empty;
            
            int result;
            if (Int32.TryParse(allegato.fileSize, out result))
            {
                if (result > 0)
                    path = NAVIGATE_ICON_PATH_ACQUIRED;
                else
                    path = NAVIGATE_ICON_PATH;
            }
            else
                path = NAVIGATE_ICON_PATH;

            return path;
        }

        /// <summary>
        /// Funzione per determinare se bisogna visualizzare il controllo per collegare l'allegato al
        /// documento da cui è stato generato per Inoltro
        /// </summary>
        /// <param name="allegato">Informazioni sull'allegato</param>
        /// <returns>True se bisogna visualizzare il controllo</returns>
        protected bool GetIsVisibileSourceButton(DocsPaWR.Allegato allegato)
        {
            // Se non si è in postback, viene individuata e memorizzata l'autorizzazione a 
            // visualizzare il controllo (funzionalità di inoltro massivo
            if(!IsPostBack)
                this.IsRoleAuthorizedMassiveForwarding = UserManager.getRuolo(this).funzioni.Where(
                    e => e.codice.Equals("MASSIVE_INOLTRA")).Count() > 0;

            // E' possibile visualizzare la freccia solo se si è abilitati ad effettuare l'inoltro massivo e
            // se la sorgente esiste
            return this.IsRoleAuthorizedMassiveForwarding &&
                !String.IsNullOrEmpty(allegato.ForwardingSource) && allegato.ForwardingSource != "-1";
        }

        /// <summary>
        /// Proprietà utilizzata per indicare se il ruolo è abilitato ad utilizzare l'inoltro massivo
        /// </summary>
        private bool IsRoleAuthorizedMassiveForwarding
        {
            get
            {
                bool toReturn = false;
                
                if(CallContextStack.CurrentContext.SessionState["MassiveFW"] != null)
                    toReturn = Boolean.Parse(CallContextStack.CurrentContext.SessionState["MassiveFW"].ToString());

                return toReturn;
            }

            set
            {
                CallContextStack.CurrentContext.SessionState["MassiveFW"] = value;
            }
        }

        /// <summary>
        /// Associazione dati griglia
        /// </summary>
		public void BindGrid()
		{
            //string filterTipologiaAllegato = FilterTipologia();
            if (this.SchedaDocumento != null)
			{
                if (this.SchedaDocumento.repositoryContext == null)
                {
                    // Reperimento degli allegati dal server solamente
                    // se non si sta inserendo un nuovo documento
                    this.SchedaDocumento.allegati = DocumentManager.getAllegati(this.SchedaDocumento, this.rblFilter.SelectedValue, InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId);
                }

                //PEC n.2051 - Gestione lista vuota allegati 
                //<
                if (this.SchedaDocumento.allegati.Count() == 0)
                {
                    this.grdAllegati.DataSource = null;
                    this.grdAllegati.DataBind();
                    return;
                }
                //>

                this.grdAllegati.DataSource = this.SchedaDocumento.allegati;
                this.grdAllegati.DataBind();
                //per documenti inoltrati
                if (SchedaDocumento.predisponiProtocollazione && !rblFilter.SelectedItem.Value.Equals("user") && !rblFilter.SelectedItem.Value.Equals("all"))
                {
                    this.grdAllegati.DataSource = null;
                    this.grdAllegati.DataBind();
                }
                //int min = this.grdAllegati.CurrentPageIndex;

                
                //for (int i = 0; i < grdAllegati.Items.Count; i++)
                //{
                //    string autore;
                //    if (!string.IsNullOrEmpty(this.SchedaDocumento.allegati[i].idPeopleDelegato) && this.SchedaDocumento.allegati[i].idPeopleDelegato != "0")
                //        autore = this.SchedaDocumento.allegati[i].idPeopleDelegato + "\nDelegato da " + this.SchedaDocumento.allegati[i].autore;
                //    else
                //        autore = this.SchedaDocumento.allegati[i].autore;
                //    grdAllegati.Items[i].ToolTip = autore;

                //    //DateTime dataInserimento;
                //    //if (DateTime.TryParse(SchedaDocumento.allegati[i].dataInserimento, out dataInserimento))
                //    //{
                //    //    if (dataInserimento > DateTime.MinValue && dataInserimento < DateTime.MaxValue)
                //    //        grdAllegati.Items[i].Cells[2].Text += "<br>-------<br>" + dataInserimento.ToString("dd/MM/yyyy");
                //    //}
                //}
			}
		}

        /*
        private string FilterTipologia()
        {
            if (this.rblFilter.SelectedValue.Equals("pec"))
            {
                return " WHERE exists (" + 
                    "(SELECT 'x' FROM dpa_notifica dn WHERE dn.docnumber=" + SchedaDocumento.docNumber +" and dn.version_id=C.VERSION_ID)" +
                    "UNION" +
                    "(SELECT 'x' FROM dpa_notifica dn WHERE dn.docnumber=" + SchedaDocumento.docNumber + " and C.OGGETTO LIKE 'Ricevuta di ritorno delle Mail%')" +
                    ")";
            }
            else if (this.rblFilter.SelectedValue.Equals("user"))
            {
                return "WHERE not exists (" +
                        "(SELECT 'x' FROM dpa_notifica dn WHERE dn.docnumber=" + SchedaDocumento.docNumber + " and dn.version_id=C.VERSION_ID)" +
                        "UNION" +
                        "(SELECT 'x' FROM dpa_notifica dn WHERE dn.docnumber=" + SchedaDocumento.docNumber + " and (C.OGGETTO LIKE 'Ricevuta di ritorno delle Mail%' Or C.OGGETTO LIKE 'Ricevuta di avvenuta%' Or C.OGGETTO LIKE 'Ricevuta di mancata consegna%')" +
                        "))";
            }
            else if (this.rblFilter.SelectedValue == InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId)
            {
                return " WHERE exists (" +
                    "(SELECT 'x' FROM dpa_notifica dn WHERE dn.docnumber=" + SchedaDocumento.docNumber + " and dn.version_id=C.VERSION_ID)" +
                    "UNION" +
                    "(SELECT 'x' FROM dpa_notifica dn WHERE dn.docnumber=" + SchedaDocumento.docNumber + " and (C.OGGETTO LIKE 'Ricevuta di avvenuta%' OR C.OGGETTO LIKE 'Ricevuta di mancata consegna%'))" +
                    ")";
            }
            else if(this.rblFilter.SelectedValue.Equals("esterni"))
            {
                return " WHERE exists (SELECT 'x' FROM VERSIONS v, Profile p WHERE v.CHA_ALLEGATI_ESTERNO = '1'  AND v.docnumber =c.docNumber) ";
            }
            else
            {
                return string.Empty;
            }        
        }
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="allegato"></param>
        /// <returns></returns>
        protected string GetCodiceAllegato(DocsPaWR.Allegato allegato)
        {
            return allegato.versionLabel;
        }

        /// <summary>
        /// Reperimento dell'autore o utente delegato alla creazione dell'allegato
        /// </summary>
        /// <param name="allegato"></param>
        /// <returns></returns>
        protected string GetAutoreOrDelegato(DocsPaWR.Allegato allegato)
        {
            string autore;
            if (!string.IsNullOrEmpty(allegato.idPeopleDelegato) && allegato.idPeopleDelegato != "0")
                autore = allegato.idPeopleDelegato + "\nDelegato da " + allegato.autore;
            else
                autore = allegato.autore;

            return autore;
        }

        /// <summary>
        /// Reperimento della data di inserimento dell'allegato
        /// </summary>
        /// <param name="allegato"></param>
        /// <returns></returns>
        protected string GetDataInserimentoAllegatoAsString(DocsPaWR.Allegato allegato)
        {
            string retValue = string.Empty;

            DateTime dataInserimento;
            if (DateTime.TryParse(allegato.dataInserimento, out dataInserimento))
            {
                if (dataInserimento > DateTime.MinValue && dataInserimento < DateTime.MaxValue)
                    return dataInserimento.ToString("dd/MM/yyyy");
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void grdAllegati_pager(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
            this.grdAllegati.CurrentPageIndex = e.NewPageIndex;

            this.BindGrid();

			// Azione di selezione primo allegato disponibile
			this.PerformSelectionFirstAllegato();
		}

        /// <summary>
        /// Azione di selezione primo allegato disponibile
        /// </summary>
        private void PerformSelectionFirstAllegato()
        {
            if (this.grdAllegati.Items.Count > 0)
                this.grdAllegati.SelectedIndex = 0;
            else
                this.grdAllegati.SelectedIndex = -1;

            this.PerformSelectionAllegato();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdAllegati_ItemCreated(object sender, DataGridItemEventArgs e)
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
        protected void grdAllegati_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PerformSelectionAllegato();
        }

        protected void rblFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            //PEC n.2051 - inizializza il pager
            //<
            this.grdAllegati.CurrentPageIndex = 0;
            //>
            BindGrid();
            // Impostazione dell'allegato come elemento selezionato
            this.PerformSelectionAllegato();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void grdAllegati_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "NavigateDocument")
            {
                // Impostazione dell'allegato come elemento selezionato
                this.grdAllegati.SelectedIndex = e.Item.ItemIndex;

                this.NavigateDocument();
            }
            else if (e.CommandName == "Select")
            {
                
            }

            if (e.CommandName == "GoToSource")
                this.NavigateDocument(e.CommandArgument.ToString());
            
        }



        /// <summary>
        /// Reperimento allegato correntemente selezionato
        /// </summary>
        /// <returns></returns>
        protected DocsPaWR.Allegato GetSelectedAllegato()
        {
            DocsPaWR.Allegato retValue = null;

            // Reperimento versionId allegato
            string versionId = this.GetSelectedVersionId();

            if (!string.IsNullOrEmpty(versionId))
            {
                foreach (DocsPaWR.Allegato allegato in this.SchedaDocumento.allegati)
                {
                    if (allegato.versionId.Equals(versionId))
                    {
                        retValue = allegato;
                        break;
                    }
                }
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento DocNumber dell'allegato correntemente selezionato
        /// </summary>
        /// <returns></returns>
        protected string GetSelectedDocNumber()
        {
            if (this.grdAllegati.Items.Count > 0 && this.grdAllegati.SelectedItem != null)
                return this.grdAllegati.SelectedItem.Cells[0].Text;
            else
                return string.Empty;
        }

        /// <summary>
        /// Reperimento versionId dell'allegato correntemente selezionato
        /// </summary>
        protected string GetSelectedVersionId()
        {
            if (this.grdAllegati.Items.Count > 0 && this.grdAllegati.SelectedItem != null)
                return this.grdAllegati.SelectedItem.Cells[1].Text;
            else
                return string.Empty;
        }

        protected string DoScambiaWithNewComment()
        {
            string scambiaOggetto = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_GESTIONE_SCAMBIA_ALLEGATI");
            if (string.IsNullOrEmpty (scambiaOggetto))
                return "0";

            if (scambiaOggetto == "1")
                return "1";
            else
                return "0";
        }
        /// <summary>
        /// Navigazione verso il dettaglio del documento allegato
        /// </summary>
        protected virtual void NavigateDocument()
        {
            DocsPaWR.Allegato allegato = this.GetSelectedAllegato();

            if (allegato != null)
            {
                DocsPaWR.SchedaDocumento schedaDocumentoAllegato = null;

                if (this.DocumentoInCestino)
                    schedaDocumentoAllegato = DocumentManager.getDettaglioDocumentoDaCestino(this, allegato.docNumber, allegato.docNumber);
                else
                    schedaDocumentoAllegato = DocumentManager.getDettaglioDocumento(this, allegato.docNumber, allegato.docNumber);

                if (schedaDocumentoAllegato != null)
                {
                    DocumentManager.setDocumentoSelezionato(this, schedaDocumentoAllegato);
                    
                    // Impostazione del versionId dell'allegato selezionato nel contesto corrente
                    this.SetCurrentIdOnContext(allegato.versionId);

                    // Navigazione verso il documento allegato,
                    // indicando di forzare la crezione di un nuovo contesto e
                    // se il documento è stato inserito in cestino
                    string url = string.Format("<script>window.open('gestioneDoc.aspx?tab=profilo&allegato=true&forceNewContext=true&daCestino={0}','principale');</script>", schedaDocumentoAllegato.inCestino);
                    this.Response.Write(url);
                }
            }
        }

        /// <summary>
        /// Funzione utilizzata per redirezionare la navigazione verso il documento origine dell'inoltro
        /// </summary>
        protected virtual void NavigateDocument(String docId)
        {
            // Reperimento scheda del documento
            DocsPaWR.SchedaDocumento doc = DocumentManager.getDettaglioDocumento(this, docId, String.Empty);

            // Salvataggio del documento selezionato
            DocumentManager.setDocumentoSelezionato(this, doc);

            // Navigazione verso il documento allegato,
            // indicando di forzare la crezione di un nuovo contesto e
            // se il documento è stato inserito in cestino
            ClientScript.RegisterStartupScript(
                this.GetType(),
                "GoToDoc",
                "window.open('gestioneDoc.aspx?tab=profilo&forceNewContext=true','principale');",
                true);
            
        }

        ///// <summary>
        ///// Reperimento oggetto InfoDocumento per l'allegato
        ///// </summary>
        ///// <param name="allegato"></param>
        ///// <returns></returns>
        //protected string InCestino(DocsPaWR.SchedaDocumento schedaDocumento)
        //{
        //    if (schedaDocumento.inCestino == null)
        //        return string.Empty;
        //    else
        //        return schedaDocumento.inCestino;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="versionId"></param>
        protected void SetCurrentIdOnContext(string versionId)
        {
            // Impostazione del versionId dell'allegato selezionato nel contesto corrente
            if (SiteNavigation.CallContextStack.CurrentContext != null &&
                SiteNavigation.CallContextStack.CurrentContext.ContextName == SiteNavigation.NavigationKeys.DOCUMENTO)
            {
                SiteNavigation.CallContextStack.CurrentContext.ContextState["versionId"] = versionId;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string PopCurrentIdOnContext()
        {
            string versionId = string.Empty;

            if (SiteNavigation.CallContextStack.CurrentContext != null &&
                SiteNavigation.CallContextStack.CurrentContext.ContextName == SiteNavigation.NavigationKeys.DOCUMENTO && 
                SiteNavigation.CallContextStack.CurrentContext.ContextState.ContainsKey("versionId"))
            {
                versionId = SiteNavigation.CallContextStack.CurrentContext.ContextState["versionId"].ToString();

                SiteNavigation.CallContextStack.CurrentContext.ContextState.Remove("versionId");
            }

            return versionId;
        }

        /// <summary>
        /// Selezione pagina del datagrid in cui viene visualizzato l'allegato richiesto
        /// </summary>
        /// <param name="versionId">
        /// Id dell'allegato richiesto
        /// </param>
        protected void SelectPageAllegato(string versionId)
        {
            if (versionId != string.Empty)
            {
                int index = 0;

                foreach (DocsPaWR.Allegato allegato in this.SchedaDocumento.allegati)
                {
                    if (allegato.versionId == versionId)
                    {
                        this.grdAllegati.CurrentPageIndex = System.Math.Abs(index / this.grdAllegati.PageSize);
                        break;
                    }

                    index++;
                }
            }
            else
                this.grdAllegati.CurrentPageIndex = 0;
        }

        /// <summary>
        /// Azione di selezione dell'allegato richiesto
        /// </summary>
        /// <param name="versionId">
        /// Id dell'allegato richiesto
        /// </param>
        protected virtual void PerformSelectionAllegato(string versionId)
        {
            if (versionId != string.Empty)
            {
                foreach (DataGridItem item in this.grdAllegati.Items)
                {
                    if (item.Cells[1].Text == versionId)
                    {
                        this.grdAllegati.SelectedIndex = item.ItemIndex;

                        this.PerformSelectionAllegato();

                        break;
                    }
                }
            }
            else
                this.PerformSelectionFirstAllegato();
        }

        /// <summary>
        /// Azione di selezione allegato correntemente selezionato
        /// </summary>
        protected virtual void PerformSelectionAllegato()
        {
            // Reperimento oggetto allegato corrente
            DocsPaWR.Allegato allegato = this.GetSelectedAllegato();

            if (allegato != null)
            {
                FileManager.setSelectedFile(this, allegato);

                // Impostazione del versionId dell'allegato selezionato nel contesto corrente
                this.SetCurrentIdOnContext(allegato.versionId);

                //gestione etichette pdf
                Session.Add("allegato", true);

                this.btn_modifAlleg.Enabled = true;

                if (this.SchedaDocumento.repositoryContext != null ||
                    (this.SchedaDocumento.protocollo != null &&
                    !string.IsNullOrEmpty(this.SchedaDocumento.protocollo.segnatura)))
                {
                    this.btn_rimuoviAlleg.Enabled = false;
                    this.btn_sostituisciDocPrinc.Enabled = false;
                }
                else
                {
                    this.btn_rimuoviAlleg.Enabled = true;
                    this.btn_sostituisciDocPrinc.Enabled = true;
                }
            }
            else
            {
                // Nessun allegato selezionato
                FileManager.setSelectedFile(this, null);
                this.SetCurrentIdOnContext(string.Empty);
            }
        }

        /// <summary>
        /// Indica se la profilazione dell'allegato è abilitata o meno
        /// </summary>
        protected bool IsEnabledProfilazioneAllegato
        {
            get
            {
                const string VIEW_STATE_KEY = "IsEnabledProfilazioneAllegato";

                bool isEnabled = false;

                if (this.ViewState[VIEW_STATE_KEY] != null)
                {
                    isEnabled = Convert.ToBoolean(this.ViewState[VIEW_STATE_KEY]);
                }
                else
                {
                    DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
                    isEnabled = ws.IsEnabledProfilazioneAllegati();
                    this.ViewState.Add(VIEW_STATE_KEY, isEnabled);
                }
                
                return (isEnabled && !this.OnSessionRepository);
            }
        }

        /// <summary>
        /// Indica se il documento correntemente visualizzato
        /// non è ancora stato inserito, ovvero se si è in sessionrepository
        /// </summary>
        protected bool OnSessionRepository
        {
            get
            {
                return (this.SchedaDocumento.repositoryContext != null);
            }
        }

		#endregion

		#region Handler pulsanti

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void btn_aggiungiAreaLav_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			try 
			{
                if (!this.GetControlAclDocumento().AclRevocata)
                {
                    DocumentManager.addAreaLavoro(this, this.SchedaDocumento);
                    string msg = "Documento aggiunto all'area di lavoro";
                    Response.Write("<script>alert(\"" + msg + "\");</script>");
                }
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

        /// <summary>
        /// Handler inserimento allegato
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_aggAlleg_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.SelectPageAllegato(this.txtSavedVersionId.Value);

            // Associazione dati
            grdAllegati.SelectedIndex = 0;
            this.BindGrid();

            // Azione di selezione dell'allegato modificato
            this.PerformSelectionAllegato(this.txtSavedVersionId.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void btn_rimuoviAlleg_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.SelectPageAllegato(this.txtSavedVersionId.Value);

            // Associazione dati
            this.BindGrid();

            // Azione di selezione primo allegato disponibile
            this.PerformSelectionFirstAllegato();
		}
	
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void btn_modifAlleg_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            // Associazione dati
            this.BindGrid();
		}

		/// <summary>
		/// Registrazione script client
		/// </summary>
		/// <param name="scriptKey"></param>
		/// <param name="scriptValue"></param>
		private void RegisterClientScript(string scriptKey,string scriptValue)
		{
			if (!this.Page.ClientScript.IsStartupScriptRegistered(this.GetType(), scriptKey))
			{
				string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";

                this.Page.ClientScript.RegisterStartupScript(this.GetType(), scriptKey, scriptString);
			}
		}
		
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void btn_sostituisciDocPrinc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            bool isCheckedOut = this.IsDocumentCheckedOut();

			try 
			{
                if (!this.GetControlAclDocumento().AclRevocata &&
                    !isCheckedOut)
                {
                    DocsPaWR.Allegato allegato = this.GetSelectedAllegato();

                    if (allegato != null)
                    {
                        DocumentManager.scambiaAllegato(this, this.SchedaDocumento.documenti[0], allegato);

                        this._schedaDocumento = DocumentManager.getDettaglioDocumento(this, 
                                    DocumentManager.getInfoDocumento(this.SchedaDocumento).idProfile, 
                                    DocumentManager.getInfoDocumento(this.SchedaDocumento).docNumber);

                        DocumentManager.setDocumentoSelezionato(this, this.SchedaDocumento);
                        
                        this.BindGrid();

                        this.PerformSelectionAllegato();
                    }
                }
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}

            if (isCheckedOut)
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "NonScambiabile",
                    "alert('Non è possibile effettuare lo scambio perché il documento pricipale oppure almeno uno dei suoi allegati risulta bloccato.');",
                    true);

		}

        /// <summary>
        /// Funzione per verificare se il documento è bloccato
        /// </summary>
        /// <returns>True se il documento è bloccato</returns>
        private bool IsDocumentCheckedOut()
        {
            return CheckInOut.CheckInOutServices.IsCheckedOutDocument(
                this.SchedaDocumento.systemId,
                this.SchedaDocumento.docNumber,
                UserManager.getInfoUtente(),
                true);
        }
        
        #endregion

        #region Gestione controllo acl documento

        /// <summary>
        /// Inizializzazione controllo verifica acl
        /// </summary>
        protected virtual void InitializeControlAclDocumento()
        {
            AclDocumento ctl = this.GetControlAclDocumento();
            ctl.IdDocumento = DocumentManager.getDocumentoSelezionato().systemId;
            ctl.OnAclRevocata += new EventHandler(this.OnAclDocumentoRevocata);
        }

        /// <summary>
        /// Reperimento controllo acldocumento
        /// </summary>
        /// <returns></returns>
        protected AclDocumento GetControlAclDocumento()
        {
            return (AclDocumento)this.FindControl("aclDocumento");
        }

        /// <summary>
        /// Listener evento OnAclDocumentoRevocata
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAclDocumentoRevocata(object sender, EventArgs e)
        {
            // Redirect alla homepage di docspa
            SiteNavigation.CallContextStack.Clear();
            SiteNavigation.NavigationContext.RefreshNavigation();
            string script = "<script>window.open('../GestioneRuolo.aspx','principale','width=410,height=300,scrollbars=NO');</script>";
            Response.Write(script);
        }

        #endregion
    }

}
