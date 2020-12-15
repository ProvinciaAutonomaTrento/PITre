using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;
using System.ComponentModel;

namespace DocsPAWA.UserControls
{
    /// <summary>
    /// Questa classe si occupa di gestire il controllo della bottoniera inserita nelle 
    /// nuove pagine di ricerca documenti
    /// </summary>
    public partial class NewMassiveOperationButtons : System.Web.UI.UserControl
    {
        public EventHandler OnTerminateMassiveOperation;
        public string OnTerminateMassiveOperationJS;
        /// <summary>
        /// Enumerazione dei possibili tipi di oggetti riportati all'interno della griglia
        /// </summary>
        public enum ObjTypeEnum
        {
            D,      /// Documento
            P       /// Fascicolo
        };

        protected string JSFunction
        {
            get
            {
                if (string.IsNullOrEmpty(OnTerminateMassiveOperationJS)) return "";
                return OnTerminateMassiveOperationJS+";";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.documentConsolidationCtrl.UpdateSavedCheckingStatus = this.UpdateSavedCheckingStatus;
            this.documentConsolidationCtrl.ShowMessageDel = this.ShowMessage;
            // Se non si è in postback, viene inizializzata la pagina
            if (!IsPostBack)
            {
                this.Initialize();
            }
            else
            {
                if (hfOperationDone.Value.Equals("1"))
                {
                    hfOperationDone.Value = "0";
                    if (OnTerminateMassiveOperation != null) OnTerminateMassiveOperation(this, new EventArgs());
                }
            }
        }

        #region Inizializzazione della pagina

        /// <summary>
        /// Funzione per l'inizializzazione della pagina
        /// </summary>
        private void Initialize()
        {
            // Assegnazione dei tooltip dipendenti dal tipo di oggetti
            // contenuti nella griglia
            this.btnExport.ToolTip = String.Format(
                "Esporta i {0} selezionati",
                this.ObjType == ObjTypeEnum.D ? "documenti" : "fascicoli");

            this.btnWorkingArea.ToolTip = String.Format(
                "Salva i {0} selezionati nell'area di lavoro",
                this.ObjType == ObjTypeEnum.D ? "documenti" : "fascicoli");

            this.btnTransmit.ToolTip = String.Format(
                "Trasmetti i {0} selezionati",
                this.ObjType == ObjTypeEnum.D ? "documenti" : "fascicoli");

            this.btnStorage.ToolTip = String.Format(
                "Sposta i {0} selezionati nell'area di conservazione.",
                this.ObjType == ObjTypeEnum.D ? "documenti" : "documenti contenuti nei fascicoli");

           
            // Visualizzazione dei controlli in base al tipo di ricerca in cui
            // è stata inserita la bottoniera
            this.ShowControls();

            // Se non ci sono documenti o fascicoli selezionati, il controllo non deve essere
            // visibile
            if (MassiveOperationUtils.IsCollectionInitialized())
            {
                this.Visible = true;
                this.chkSelectDeselectAll.Enabled = true;
            }
            else
            {
               // this.Visible = false;
                this.Visible = true;
                this.chkSelectDeselectAll.Enabled = false;
            }

            this.hd_pag_chiam.Value = this.PAGINA_CHIAMANTE;
            this.hd_pag_num.Value = this.NUM_RESULT;
        }

        /// <summary>
        /// Funzione per la visualizzazione dei pulsanti in base al tipo di oggetto
        /// associato a questa bottoniera.
        /// </summary>
        private void ShowControls()
        {
            // Il ruolo associato all'utente
            Funzione[] functions;
            // Booleano utilizzato per indicare se l'utente è abilitato alla firma
            bool canSign = false;
            // Booleano utilizzato per indicare se l'utente è abilitato alla fascicolazione
            bool canFascicolare = false;
            // Booleano utilizzato per indicare se l'utente è abilitato alla trasmissione
            bool canTransmit = false;
            // Booleano utilizzato per indicare se l'utente è abilitato all'applicazione del timestamp
            bool canApplyTimestamp = false;
            // Boolenao utilizzato per indicare se l'utente è abilitato alla conversione PDF
            bool canConvert = false;
            // Booleano utilizzato per indicare se l'utente è abilitato a mettere un documento o un fascicolo
            // in adl
            bool canAddObjInADL = false;
            // Booleano utilizzato per indicare se l'utente è abilitato a rimuovere un documento o un fascicolo
            // dall' adl
            bool canRemoveObjInADL = false;
            // Booleano utilizzato per indicare se l'utente è abilitato ad effettuare l'inoltra massivo
            bool canDoMassiveInoltra = false;
            // Booleano utilizzato per indicare se l'utente è abilitato ad effettuare l'inserimento in area di conservazione
            bool canDoMassiveInsInStorageArea = false;
            // Booleano utilizzato per indicare se l'utente è abilitato ad effettuare la rimozione delle versioni
            bool canRemoveVersions = false;
            // Indica se l'utente è abilitato ad effettuare l'azione di consolidamento
            bool canDoConsolidation = false;
            // Indica se bisogna visualizzare il bottone della persinalizzazione griglia
            bool showGridPersonalization = false;

            // Reperimento della lista di funzioni associate al ruolo
            functions = UserManager.getRuolo(this.Page).funzioni;

            // Verifica delle autorizzazioni
            canSign = functions.Where(e => e.codice.Equals("DO_DOC_FIRMA")).Count() > 0 &&
                functions.Where(e => e.codice.Equals("MASSIVE_SIGN")).Count() > 0;
            canFascicolare = functions.Where(e => e.codice.Equals("FASC_INS_DOC")).Count() > 0 &&
                functions.Where(e => e.codice.Equals("MASSIVE_CLASSIFICATION")).Count() > 0;
            canTransmit = functions.Where(e => e.codice.Equals("DO_TRA_TRASMETTI")).Count() > 0 &&
                functions.Where(e => e.codice.Equals("MASSIVE_TRANSMISSION")).Count() > 0;
            canApplyTimestamp = functions.Where(e => e.codice.Equals("DO_TIMESTAMP")).Count() > 0 &&
                functions.Where(e => e.codice.Equals("MASSIVE_TIMESTAMP")).Count() > 0;
            canConvert = functions.Where(e => e.codice.Equals("MASSIVE_CONVERSION")).Count() > 0;
            canAddObjInADL = functions.Where(e => e.codice.Equals("MASSIVE_ADL")).Count() > 0 &&
                this.ObjType.Equals("D") ? functions.Where(e => e.codice.Equals("DOC_ADD_ADL")).Count() > 0 :
                                           functions.Where(e => e.codice.Equals("FASC_ADD_ADL")).Count() > 0 && String.IsNullOrEmpty(Request["ricADL"]);
            canRemoveObjInADL = functions.Where(e => e.codice.Equals("MASSIVE_ADL")).Count() > 0 &&
                this.ObjType.Equals("D") ? functions.Where(e => e.codice.Equals("DOC_ADD_ADL")).Count() > 0 :
                                           functions.Where(e => e.codice.Equals("FASC_ADD_ADL")).Count() > 0;
            canDoMassiveInoltra = functions.Where(e => e.codice.Equals("MASSIVE_INOLTRA")).Count() > 0;
            canDoMassiveInsInStorageArea = functions.Where(e => e.codice.Equals("DO_CONS")).Count() > 0;
            canRemoveVersions = functions.Where(e => e.codice.Equals("MASSIVE_REMOVE_VERSIONS")).Count() > 0;
            canDoConsolidation = functions.Count(e => e.codice.Equals("DO_CONSOLIDAMENTO")) > 0;
            showGridPersonalization = functions.Where(e => e.codice.Equals("GRID_PERSONALIZATION")).Count() > 0;
            // Se il tipo di oggetto è documento, bisogna visualizzare
            // i pulsanti di Firma, Fascicolazione, Timestamp, Conversione, Inoltra e Inserimento in Conservazione
            if (this.ObjType == ObjTypeEnum.D)
            {
                this.btnSign.Visible = canSign;
                this.btnFascicola.Visible = canFascicolare;
                this.btnTimeStamp.Visible = canApplyTimestamp;
                this.btnConvert.Visible = canConvert;
                this.btnInoltra.Visible = canDoMassiveInoltra;
                this.documentConsolidationCtrl.Visible = canDoConsolidation;
                this.btnEliminaVersioni.Visible = canRemoveVersions;
            }

            this.btnStorage.Visible = canDoMassiveInsInStorageArea;
            this.btnTransmit.Visible = canTransmit;
            this.btnWorkingArea.Visible = canAddObjInADL;
            this.btnRemoveWorkingArea.Visible = canRemoveObjInADL;
            //Rendo sempre invisibile il bottone btnCustomizeGrid poichè è stato spostato fuori dallo UserControl
            //this.btnCustomizeGrid.Visible = showGridPersonalization;

            if (!string.IsNullOrEmpty(this.PAGINA_CHIAMANTE) && (this.PAGINA_CHIAMANTE.Equals("StampaReg") || this.PAGINA_CHIAMANTE.Equals("StampaRep")))
            {
                this.btnFascicola.Visible = false;
                this.btnTransmit.Visible = false;
                this.btnEliminaVersioni.Visible = false;
                this.btnInoltra.Visible = false;
                this.btnConvert.Visible = false;
                this.btnWorkingArea.Visible = false;
                this.btnRemoveWorkingArea.Visible = false;
                this.documentConsolidationCtrl.Visible = false;


            }
        }

        #endregion

        #region Proprietà di pagina

        protected long SelectNumberToSelectAll { get; set; }

        /// <summary>
        /// L'id del datagrid con i risultati su cui effettuare operationi massive
        /// </summary>
        public string DataGridId { get; set; }

        /// <summary>
        /// Tipo di griglia di ricerca in cui è inserita la bottoniera.
        /// Questo campo viene utilizzato per la gestione delle griglie custom
        /// </summary>
        public GridTypeEnumeration GridType { get; set; }

        /// <summary>
        /// Identificativo della message box da utilizzare per mostrare messaggi
        /// all'utente
        /// </summary>
        public String MessageBoxID { get; set; }

        /// <summary>
        /// Tipo di griglia in cui è inserita la bottoniera
        /// </summary>
        public ObjTypeEnum ObjType { get; set; }

        /// <summary>
        /// Identificativo della griglia
        /// </summary>
        public String GridId { get; set; }

        public String TemplateID { get; set; }

        public String SearchId { get; set; }

        /// <summary>
        /// True se il controllo è inserito nella pagina dei documenti
        /// all'interno del fascicolo
        /// </summary>
        [DefaultValue(false)]
        public bool IsDocumentsInProject { get; set; }

        /// <summary>
        /// Indice della cella del datagrid  in cui è inserito il controllo con le checkbox
        /// per la selezione dell'item
        /// </summary>
        public int CheckBoxColumnIndex { get; set; }

        #endregion

        #region Gestione seleziona / deseleziona tutti

        protected void chkSelectDeselectAll_CheckedChanged(object sender, EventArgs e)
        {
            // Se lo stato del check è stato modificato da click sui checkbox
            // nella griglia, il campo nascosto hfSelectedFromGrid conterrà 1
            // ed in tal caso non bisogna cambiare lo stato
            if (this.hfSelectedFromGrid.Value.Equals("1"))
            {
                this.hfSelectedFromGrid.Value = "0";
                return;
            }

            // Se la checkbox è spuntata, selezione di tutti gli item,
            // altrimenti deselezione di tutti gli item
            if (this.chkSelectDeselectAll.Checked)
                this.SelectAll();
            else
                this.DeselectAll();
        }

        /// <summary>
        /// Funzione per la selezione di tutti gli item
        /// </summary>
        public void SelectAll()
        {
            // Dizionario in cui inserire i nuovi valori
            Dictionary<String, MassiveOperationTarget> temp = new Dictionary<string, MassiveOperationTarget>();

            // Scansione di tutti gli elementi del dizionario e impostazione a true
            // di tutti i valori
            foreach (string key in MassiveOperationUtils.ItemsStatus.Keys)
            {
                MassiveOperationTarget mot = MassiveOperationUtils.ItemsStatus[key].Clone;
                mot.Checked = true;
                temp.Add(key,mot);
            }

            // Salvataggio della nuova collection
            MassiveOperationUtils.ItemsStatus = temp;

            // Aggiornamento dello stato dei flag della griglia associata
            this.UpdateItemCheckingStatus();

            // Cambiamento del testo del controllo
            this.chkSelectDeselectAll.Text = "Deseleziona tutti";
            this.chkSelectDeselectAll.Checked = true;

            // Associazione degli eventi javascript ai pulsanti
            this.AssociateOpenFunctionToButtons();

        }

        /// <summary>
        /// Funzione per la deselezione di tutti gli elementi
        /// </summary>
        public void DeselectAll()
        {
            // Dizionario in cui inserire i nuovi valori
            Dictionary<String, MassiveOperationTarget> temp = new Dictionary<string, MassiveOperationTarget>();

            // Scansione di tutti gli elementi del dizionario e impostazione a false
            // di tutti i valori
            foreach (string key in MassiveOperationUtils.ItemsStatus.Keys)
            {
                MassiveOperationTarget mot = MassiveOperationUtils.ItemsStatus[key].Clone;
                mot.Checked = false;

                temp.Add(key, mot);
            }

            // Salvataggio della nuova collection
            MassiveOperationUtils.ItemsStatus = temp;

            // Aggiornamento dello stato dei flag della griglia associata
            this.UpdateItemCheckingStatus();

            // Cambiamento del testo del controllo
            this.chkSelectDeselectAll.Text = "Seleziona tutti";
            this.chkSelectDeselectAll.Checked = false;

            // Reset degli eventi javascript associati ai bottoni
            this.ResetOnClientClick();

        }

        #endregion

        #region Gestione stato di checking di un item

        /// <summary>
        /// Funzione per l'impostazione del valore di
        /// checking per un dato item
        /// </summary>
        /// <param name="objectId">Id dell'oggetto di cui modificare lo stato</param>
        /// <param name="state">Stato da associare all'oggetto (true per checked, false per unchecked)</param>
        public void SetState(string objectId, bool state)
        {
            // Impostazione dello stato
            MassiveOperationUtils.ItemsStatus[objectId].Checked = state;

            // Calcolo del numero di elementi checkati
            int elementChecked = MassiveOperationUtils.ItemsStatus.Values.Where(e => e.Checked == true).Count();

            // Se il dizionario degli stati contiene tutti true come valore,
            // viene flaggato Seleziona / Deseleziona tutto altrimenti viene
            // defleggato
            if (elementChecked == MassiveOperationUtils.ItemsStatus.Values.Count)
            {
                this.chkSelectDeselectAll.Checked = true;
                this.chkSelectDeselectAll.Text = "Deseleziona tutto";
            }
            else
            {
                this.chkSelectDeselectAll.Checked = false;
                this.chkSelectDeselectAll.Text = "Seleziona tutto";
            }

            // Se c'è almeno un elemento selezionato, viene impostato
            // l'OnClientClick in modo che ad una eventale pressione del
            // pulsante venga aperta la finestra di gestione per la specifica
            // funzione, altrimenti vengono azzerati gli eventi
            if (elementChecked > 0)
                this.AssociateOpenFunctionToButtons();
            else
                this.ResetOnClientClick();

        }

        /// <summary>
        /// Funzione per l'associazione delle funzioni javascript per l'apertura
        /// della finestra di gestione per l'operazione associata ai vari bottoni
        /// </summary>
        private void AssociateOpenFunctionToButtons()
        {
            // Bottone di firma
            this.btnSign.OnClientClick = "OpenFirmaMassivaDocumenti();";
            // Bottone di fascicolazione
            this.btnFascicola.OnClientClick = "OpenFascicolaMassivo();";
            // Bottone di trasmissione
            this.btnTransmit.OnClientClick = "OpenTrasmissioneMassiva();";
            // Bottone per Area di lavoro
            this.btnWorkingArea.OnClientClick = String.Format("OpenADLMassiva('{0}');", this.ObjType);
            // Bottone per rimuovi dall'Area di lavoro
            this.btnRemoveWorkingArea.OnClientClick = String.Format("OpenRemoveADLMassiva('{0}');", this.ObjType);
            // Bottone per l'esportazione
            if (this.IsDocumentsInProject)
                this.btnExport.OnClientClick = String.Format("OpenExportDialog('docInfasc');");
            else
                this.btnExport.OnClientClick = String.Format("OpenExportDialog('{0}');",
                    this.ObjType == ObjTypeEnum.D ? "doc" : "fasc");

            // Bottone per l'associazione del timestamp
            this.btnTimeStamp.OnClientClick = "OpenTimestampMassivo();";
            // Bottone per la conversione
            this.btnConvert.OnClientClick = "OpenConversioneMassivo();";
            // Bottone per l'inoltro
            this.btnInoltra.OnClientClick = "OpenInoltraDialog();";
            // Bottone per l'inserimento massivo in area di conservazione
            this.btnStorage.OnClientClick = "OpenStorageMassivo('" + this.ObjType.ToString() + "');";
            //Bottone per la rimozione delle versioni
            this.btnEliminaVersioni.OnClientClick = "OpenEliminaVersioni('" + this.ObjType.ToString() + "');";
            this.documentConsolidationCtrl.HasItemChecked = true;
        }

        /// <summary>
        /// Funzione per il reset dell'onclient click di tutti i pulsanti della bottoniera
        /// </summary>
        private void ResetOnClientClick()
        {
            // Bottone di firma
            this.btnSign.OnClientClick = String.Empty;
            // Bottone di fascicolazione
            this.btnFascicola.OnClientClick = String.Empty;
            // Bottone di trasmissione
            this.btnTransmit.OnClientClick = String.Empty;
            // Bottone per Area di lavoro
            this.btnWorkingArea.OnClientClick = String.Empty;
            // Bottone per Rimuovi dall'area di lavoro
            this.btnRemoveWorkingArea.OnClientClick = String.Empty;
            // Bottone per l'esportazione
            this.btnExport.OnClientClick = String.Empty;
            // Bottone per l'associazione del timestamp
            this.btnTimeStamp.OnClientClick = String.Empty;
            // Bottone per la conversione
            this.btnConvert.OnClientClick = String.Empty;
            // Bottone per l'inoltro
            this.btnInoltra.OnClientClick = String.Empty;
            //Bottone rimuovi versioni
            this.btnEliminaVersioni.OnClientClick = String.Empty;
            this.documentConsolidationCtrl.HasItemChecked = false;
        }

        #endregion

        #region Url completi alle pagine di gestione operazione

        protected string UrlToDocumentPage
        {
            get
            {
                return Utils.getHttpFullPath() + "/documento/gestioneDoc.aspx?tab=protocollo";
            }
        }

        protected string UrlToGridManagement
        {
            get
            {
                return Utils.getHttpFullPath() + "/Grids/NewGridPersonalization.aspx";
            }
        }

        /// <summary>
        /// Proprietà per la mappatura del path della finestra di 
        /// fascicolazione massiva
        /// </summary>
        /// <returns>Il path della finestra di fascicolazione massiva</returns>
        protected string GetFascicolazioneMassivaURL
        {
            get
            {
                return Utils.getHttpFullPath(this.Page) + "/MassiveOperation/FascicolazioneMassiva.aspx";
            }
        }

        /// <summary>
        /// Proprietà che restituisce il path della finestra di
        /// trasmissione rapida
        /// </summary>
        protected string GetTrasmissioneMassivaURL
        {
            get
            {
                return String.Format("{0}/MassiveOperation/TrasmissioneMassiva.aspx?objType={1}",
                    Utils.getHttpFullPath(this.Page),
                    this.ObjType.ToString());
            }
        }

        /// <summary>
        /// Url della finestra per l'eliminazione delle versioni
        /// </summary>
        protected string GetEliminaVersioniURL
        {
            get
            {
                return String.Format("{0}/MassiveOperation/RimuoviVersioni.aspx",
                       Utils.getHttpFullPath(this.Page));
            }
        }

        /// <summary>
        /// Proprità che restituisce il path della finestra di
        /// applicazione massiva del timestamp
        /// </summary>
        protected string GetTimestampMassivoURL
        {
            get
            {
                return String.Format("{0}/MassiveOperation/TimestampMassivo.aspx",
                    Utils.getHttpFullPath(this.Page));
            }
        }

        /// <summary>
        /// Proprietà che restituisce il path della finestra di
        /// firma digitale documenti
        /// </summary>
        protected string GetFirmaMassivaDocumentiURL
        {
            get
            {
                return String.Format("{0}/MassiveOperation/FirmaDigitaleMassiva.aspx",
                        Utils.getHttpFullPath(this.Page));
            }
        }

        /// <summary>
        /// Proprietà che restituisce il path della finestra di
        /// firma conversione massiva documenti
        /// </summary>
        protected string GetConversioneMassivaDocumentiURL
        {
            get
            {
                return String.Format("{0}/MassiveOperation/ConversionePDF.aspx",
                        Utils.getHttpFullPath(this.Page));
            }
        }

        /// <summary>
        /// Proprietà che restituisce il path della finestra di
        /// spostamento massivo di documenti o fascicoli nell'ADL
        /// </summary>
        protected string GetMassiveWorkingAreaURL
        {
            get
            {
                return String.Format("{0}/MassiveOperation/ADLMassiva.aspx",
                       Utils.getHttpFullPath(this.Page));
            }
        }

        /// <summary>
        /// Proprietà che restituisce il path della finestra di
        /// rimozione massiva di documenti o fascicoli dall'ADL
        /// </summary>
        protected string GetMassiveRemoveWorkingAreaURL
        {
            get
            {
                return String.Format("{0}/MassiveOperation/RemoveADLMassiva.aspx",
                       Utils.getHttpFullPath(this.Page));
            }
        }

        /// <summary>
        /// Proprietà che restituisce il path della finestra di
        /// esportazione dei risultati della ricerca
        /// </summary>
        protected string GetExportResultURL
        {
            get
            {
                return String.Format("{0}/exportDati/exportDatiSelection.aspx",
                       Utils.getHttpFullPath(this.Page));
            }
        }

        /// <summary>
        /// Url della finestra per l'inoltro massivo
        /// </summary>
        protected string GetInoltraMassivoURL
        {
            get
            {
                return String.Format("{0}/MassiveOperation/InoltroMassivo.aspx",
                       Utils.getHttpFullPath(this.Page));
            }
        }

        /// <summary>
        /// Url alla pagina per l'inserimento massivo di documento nell'area
        /// di conservazione
        /// </summary>
        protected string GetMassiveStorageURL
        {
            get
            {
                return String.Format("{0}/MassiveOperation/ConservazioneMassiva.aspx",
                    Utils.getHttpFullPath(this.Page));
            }
        }

        #endregion

        #region Gestione click sui pulsanti delle azioni massive

        protected void btnSign_Click(object sender, ImageClickEventArgs e)
        {
            if(String.IsNullOrEmpty(this.btnSign.OnClientClick))
                this.ShowMessage("Selezionare almeno un elemento per avviare la firma massiva");

        }

        protected void btnFascicola_Click(object sender, ImageClickEventArgs e)
        {
            if (String.IsNullOrEmpty(this.btnFascicola.OnClientClick))
                this.ShowMessage("Selezionare almeno un documento per avviare la fascicolazione massiva");

        }

        protected void btnTransmit_Click(object sender, ImageClickEventArgs e)
        {
            if (String.IsNullOrEmpty(this.btnTransmit.OnClientClick))
                this.ShowMessage(
                    String.Format("Selezionare almeno un {0} per avviare la trasmissione massiva",
                    this.ObjType == ObjTypeEnum.D ? "documento" : "fascicolo"));

        }

        protected void btnWorkingArea_Click(object sender, ImageClickEventArgs e)
        {
            if (String.IsNullOrEmpty(this.btnWorkingArea.OnClientClick))
                this.ShowMessage(
                    String.Format("Selezionare almeno un {0} per avviare lo spostamento massivo in area di lavoro",
                    this.ObjType == ObjTypeEnum.D ? "documento" : "fascicolo"));

        }

        protected void btnRemoveWorkingArea_Click(object sender, ImageClickEventArgs e)
        {
            if (String.IsNullOrEmpty(this.btnRemoveWorkingArea.OnClientClick))
                this.ShowMessage(
                    String.Format("Selezionare almeno un {0} per avviare la rimozione di oggetti da area di lavoro",
                    this.ObjType == ObjTypeEnum.D ? "documento" : "fascicolo"));

        }

        protected void btnExport_Click(object sender, ImageClickEventArgs e)
        {
            if (String.IsNullOrEmpty(this.btnExport.OnClientClick))
                this.ShowMessage(
                    String.Format("Selezionare almeno un {0} per esportare massivamente reportistica",
                    this.ObjType == ObjTypeEnum.D ? "documento" : "fascicolo"));

        }

        protected void btnTimeStamp_Click(object sender, ImageClickEventArgs e)
        {
            if (String.IsNullOrEmpty(this.btnTimeStamp.OnClientClick))
                this.ShowMessage("Selezionare almeno un documento per applicazione massivamente timestamp");

        }

        protected void btnConvert_Click(object sender, ImageClickEventArgs e)
        {
            if (String.IsNullOrEmpty(this.btnConvert.OnClientClick))
                this.ShowMessage("Selezionare almeno un documento per avviare la conversione PDF massiva");

        }

        protected void btnInoltra_Click(object sender, ImageClickEventArgs e)
        {
            if (String.IsNullOrEmpty(this.btnInoltra.OnClientClick))
                this.ShowMessage("Selezionare almeno un documento per avviare la creazione di un documento da inoltrare");

        }

        protected void btnElVersioni_Click(object sender, ImageClickEventArgs e)
        {
            if (String.IsNullOrEmpty(this.btnEliminaVersioni.OnClientClick))
                this.ShowMessage("Selezionare almeno un documento per avviare la rimozione delle versioni");
        }

        /// <summary>
        /// Al click sul pulsante della conservazione, vengono aggiunti i documenti nell'area di
        /// conservazione e viene mostrato l'esito dell'operazione.
        /// </summary>
        protected void btnStorage_Click(object sender, ImageClickEventArgs e)
        {
            if (String.IsNullOrEmpty(this.btnStorage.OnClientClick))
                this.ShowMessage("Selezionare almeno un documento per inserire massivamente in area di conservazione");

        }


        #endregion

        /// <summary>
        /// Funzione utilizzata per mostrare un messaggio all'utente
        /// </summary>
        /// <param name="message">Il messaggio da mostrare all'utente</param>
        public void ShowMessage(String message)
        {
            ((AjaxMessageBox)this.Page.FindControl(this.MessageBoxID)).ShowMessage(message);
        }

        /// <summary>
        /// Funzione per il salvataggio dello stato di flagging degli item del
        /// gatagrid
        /// </summary>
        public void UpdateSavedCheckingStatus()
        {
            // Il data grid
            DataGrid dataGrid;

            // La colonna con le checkbox per la selezione dell'item
            GridsCheckBox gridsCheckBox = null;

            // Reperimento del datagrid
            dataGrid = Page.FindControl(this.DataGridId) as DataGrid;

            // Salvataggio dello stato di flagging
            foreach (DataGridItem item in dataGrid.Items)
            {
                // Selezione dell'elemento GridsCheckBox
                foreach (Control control in item.Cells[this.CheckBoxColumnIndex].Controls)
                    if (control.GetType().BaseType.Name.Equals(typeof(GridsCheckBox).Name))
                        gridsCheckBox = control as GridsCheckBox;
            
                if(gridsCheckBox != null)
                    this.SetState(gridsCheckBox.Value, gridsCheckBox.Checked);

            }

        }

        /// <summary>
        /// Funzione per la reimpostazione dello stato di flagging degli item del
        /// datagrid
        /// </summary>
        public void UpdateItemCheckingStatus()
        {
            // Il datagrid
            DataGrid dataGrid;

            // La colonna con le checkbox per la selezione dell'item
            GridsCheckBox gridsCheckBox = null;

            // Aggiornamento dello stato di flagging
            dataGrid = Page.FindControl(this.DataGridId) as DataGrid;
            foreach (DataGridItem items in dataGrid.Items)
            {
                // Selezione dell'elemento GridsCheckBox
                foreach (Control control in items.Cells[this.CheckBoxColumnIndex].Controls)
                    if (control.GetType().BaseType.Name.Equals(typeof(GridsCheckBox).Name))
                        gridsCheckBox = control as GridsCheckBox;

                if(gridsCheckBox != null)
                    gridsCheckBox.Checked = MassiveOperationUtils.ItemsStatus[gridsCheckBox.Value].Checked;
            }

        }

        /// <summary>
        /// Funzione per l'inizializzazione del controllo
        /// </summary>
        /// <param name="systemIdList">Lista dei system id dei documenti o dei fascicoli</param>
        public void InitializeOrUpdateUserControl(SearchResultInfo[] systemIdList)
        {
            // Il dizinario utilizzato per contenere lo stato dei flag del data grid
            Dictionary<String, MassiveOperationTarget> temp;

            // Se si è in postback viene prelevato il dizionario salvato, 
            // altrimenti ne viene creato uno nuovo
            if (Page.IsPostBack)
                temp = MassiveOperationUtils.ItemsStatus;
            else
                temp = new Dictionary<string, MassiveOperationTarget>();

            // Inizializzazione della mappa con i system id degli oggetti e lo stato
            // di checking (in fase di inizializzazione tutti gli item sono deselezionati)
            foreach (SearchResultInfo syd in systemIdList)
                if (!temp.Keys.Contains(syd.Id))
                    temp.Add(syd.Id,new MassiveOperationTarget(syd.Id,syd.Codice));

            // Salvataggio del dizionario
            MassiveOperationUtils.ItemsStatus = temp;
        }

        public string PAGINA_CHIAMANTE
        {
            get
            {
                return this.GetStateValue("PAGINA_CHIAMANTE");
            }
            set
            {
                this.SetStateValue("PAGINA_CHIAMANTE", value);
            }
        }

        public string NUM_RESULT
        {
            get
            {
                return this.GetStateValue("NUM_RESULT");
            }
            set
            {
                this.SetStateValue("NUM_RESULT", value);
            }
        }

        protected string GetStateValue(string key)
        {
            if (this.ViewState[key] != null)
                return this.ViewState[key].ToString();
            else
                return string.Empty;
        }

        protected void SetStateValue(string key, string obj)
        {
            this.ViewState[key] = obj;
        }

        protected string UrlToDefaultGrid
        {
            get
            {
                return Utils.getHttpFullPath() + "/Grids/GridDefault.aspx";
            }
        }

        protected String PageToReload
        {
            get
            {
                String pageToReload = "NewTabSearchResult.aspx";
                if (Request.Url.LocalPath.Contains("NewDocListInProject.aspx"))
                    pageToReload = "NewDocListInProject.aspx";

                return pageToReload;
                     
            }
        }

        protected string UrlPreferredGrid
        {
            get
            {
                return Utils.getHttpFullPath() + "/Grids/GridPreferred.aspx";
            }
        }

        protected string UrlSaveGrid
        {
            get
            {
                return Utils.getHttpFullPath() + "/Grids/GridSave.aspx";
            }
        }


      
    }
}
