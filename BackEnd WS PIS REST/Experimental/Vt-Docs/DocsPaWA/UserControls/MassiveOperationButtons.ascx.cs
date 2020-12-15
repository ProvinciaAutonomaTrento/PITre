using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.SiteNavigation;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.UserControls
{
    public partial class MassiveOperationButtons : System.Web.UI.UserControl
    {
        /// <summary>
        /// Enumerazione dei possibili tipi di oggetti riportati all'interno della griglia
        /// </summary>
        public enum ObjTypeEnum
        {
            D,      /// Documento
            P       /// Fascicolo
        };

        protected void Page_Load(object sender, EventArgs e)
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

            // Visualizzazione dei controlli in base al tipo di ricerca in cui
            // è stata inserita la bottoniera
            this.ShowControls();

            // Se non ci sono documenti o fascicoli selezionati, il controllo non deve essere
            // visibile
            if (MassiveOperationUtils.IsCollectionInitialized())
                this.Visible = true;
            else
                this.Visible = false;

            

            //// Se nel livello superiore dello stack è presente la variabile GoToDetails
            //// significa che è stato fatto un inoltro massivo.
            //if (CallContextStack.CurrentContext.SessionState["GoToDetails"] != null)
            //{
            //    // Viene rimossa la variabile dalla sessione
            //    CallContextStack.CurrentContext.SessionState.Remove("GoToDetails");
            //    // Viene immerso il codice javascript per redirezionare il fram di destra alla
            //    // pagina di dettaglio del documento creato
            //    Page.ClientScript.RegisterStartupScript(
            //        this.GetType(),
            //        "Redirect",
            //        "top.frame
            //}



        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (MassiveOperationUtils.ItemsStatus != null)
                this.AssociateJavascriptSelectUnselectAll();

        }

        protected long SelectNumberToSelectAll
        {
            get;
            set;
        }

        /// <summary>
        /// Se gli elementi visualizzati attualmente nel datagrid sono quelli che portano
        /// ad avere tutti gli elementi selezionati, allora viene registrato l'evento
        /// onclientclick di tutte le checkbox del datagrid
        /// </summary>
        private void AssociateJavascriptSelectUnselectAll()
        {
            // Booleano utilizzato per indicare se bisogna associare l'evento onclientclick
            // ai checkbox del datagrid
            bool associate = false;

            // Numero di elementi che bisogna checkare per poter
            // visualizzare il testo "Seleziona tutti"
            long selectAll = 0;

            // Il datagrid
            DataGrid dataGrid;

            // Lista dei checkbox attualmente visualizzati nella pagina
            List<CheckBox> checkboxList;

            // Reperimento del datagrid
            dataGrid = Page.FindControl(this.DataGridId) as DataGrid;

            // Reperimento dei checkbox di selezione presenti nella griglia
            checkboxList = new List<CheckBox>();
            foreach (DataGridItem item in dataGrid.Items)
                checkboxList.Add((CheckBox)item.FindControl(this.CheckBoxControlId));

            // Calcolo del numero di elementi che mancano per poter visualizzare
            // il messaggio Seleziona tutti
            selectAll = MassiveOperationUtils.ItemsStatus.Values.Where(e => e.Checked == false).Count();

            this.SelectNumberToSelectAll = selectAll;

            // Se selectAll è 0 allora bisogna associare la funzione
            if (selectAll == 0)
                associate = true;
            else
            {
                // ...altrimenti se il numero di elementi mancanti è pari
                // al numero di elementi selezionati nella pagina...
                if (selectAll <= dataGrid.PageSize)
                {
                    // ...allora si controlla se gli elementi mancanti sono quelli
                    // visualizzati nella pagina, bisogna associare la funzione
                    if (checkboxList != null &&
                        checkboxList.Where(e => e.Checked == false).Count() == selectAll)
                        associate = true;

                }
            }

            // Se si deve associare la funzione, si procede con
            // l'associazione
            if (associate)
                foreach (CheckBox checkBox in checkboxList)
                    checkBox.Attributes.Add(
                        "onclick",
                        String.Format("ceckSelectDeselectAll({0}, {1}, {2});",
                            checkBox.ClientID,
                            this.chkSelectDeselectAll.ClientID,
                            this.hfSelectedFromGrid.ClientID));

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
            // Booleano utilizzato per indicare se l'utente è abilitato ad effettuare l'inoltra massivo
            bool canDoMassiveInoltra = false;
            // Indica se l'utente è abilitato ad effettuare l'azione di consolidamento
            bool canDoConsolidation = false;
            // Booleano utilizzato per indicare se l'utente è abilitato ad effettuare la rimozione delle versioni
            bool canRemoveVersions = false;

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
                                           functions.Where(e => e.codice.Equals("FASC_ADD_ADL")).Count() > 0;
            canDoMassiveInoltra = functions.Where(e => e.codice.Equals("MASSIVE_INOLTRA")).Count() > 0;
            canRemoveVersions = functions.Where(e => e.codice.Equals("MASSIVE_REMOVE_VERSIONS")).Count() > 0;

            canDoConsolidation = functions.Count(e => e.codice.Equals("DO_CONSOLIDAMENTO")) > 0;

            // Se il tipo di oggetto è documento, bisogna visualizzare
            // i pulsanti di Firma, Fascicolazione, Timestamp e Conversione
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

            this.btnTransmit.Visible = canTransmit;
            this.btnWorkingArea.Visible = canAddObjInADL;
        }

        /// <summary>
        /// L'id del datagrid con i risultati su cui effettuare operationi massive
        /// </summary>
        public string DataGridId { get; set; }

        /// <summary>
        /// L'id del controllo check box all'interno della griglia
        /// </summary>
        public string CheckBoxControlId { get; set; }

        /// <summary>
        /// L'id della textbox con il system id dell'oggetto selezionato
        /// </summary>
        public string HiddenFieldControlId { get; set; }

        protected string UrlToDocumentPage
        {
            get
            {
                return Utils.getHttpFullPath() + "/documento/gestioneDoc.aspx?tab=protocollo";
            }
        }

        /// <summary>
        /// Il tipo di oggetto contenuto all'interno del data grid
        /// </summary>
        ObjTypeEnum objType;
        public ObjTypeEnum ObjType {
            get
            {
                return objType;
            }

            set
            {
                this.objType = value;
                MassiveOperationUtils.ObjectType = this.objType.Equals(ObjTypeEnum.D) ?
                    TrasmissioneTipoOggetto.DOCUMENTO :
                    TrasmissioneTipoOggetto.FASCICOLO;
            }
        }

        public void SetState(string value, bool state)
        {
            // Impostazione dello stato
            MassiveOperationUtils.ItemsStatus[value].Checked = state;

            // Se il dizionario degli stati contiene tutti true come valore,
            // viene flaggato Seleziona / Deseleziona tutto altrimenti viene
            // defleggato
            if (MassiveOperationUtils.ItemsStatus.Values.Where(e => e.Checked == true).Count() ==
                    MassiveOperationUtils.ItemsStatus.Values.Count)
            {
                this.chkSelectDeselectAll.Checked = true;
                this.chkSelectDeselectAll.Text = "Deseleziona tutto";
            }
            else
            {
                this.chkSelectDeselectAll.Checked = false;
                this.chkSelectDeselectAll.Text = "Seleziona tutto";
            }

        }

        /// <summary>
        /// Funzione per il salvataggio dello stato di flagging degli item del
        /// gatagrid
        /// </summary>
        public void UpdateSavedCheckingStatus()
        {
            // Il data grid
            DataGrid dataGrid;

            // La checkbox
            CheckBox checkBox;

            // Il campo nascosto
            HiddenField hiddenField;

            // Reperimento del datagrid
            dataGrid = Page.FindControl(this.DataGridId) as DataGrid;

            // Salvataggio dello stato di flagging
            foreach (DataGridItem item in dataGrid.Items)
            {
                checkBox = item.FindControl(this.CheckBoxControlId) as CheckBox;
                hiddenField = item.FindControl(this.HiddenFieldControlId) as HiddenField;

                this.SetState(hiddenField.Value, checkBox.Checked);

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

            // La checkbox
            CheckBox checkBox;

            // Il campo nascosto
            HiddenField hiddenField;

            // Aggiornamento dello stato di flagging
            dataGrid = Page.FindControl(this.DataGridId) as DataGrid;
            foreach (DataGridItem items in dataGrid.Items)
            {
                checkBox = items.FindControl(this.CheckBoxControlId) as CheckBox;
                hiddenField = items.FindControl(this.HiddenFieldControlId) as HiddenField;

                checkBox.Checked = MassiveOperationUtils.ItemsStatus[hiddenField.Value].Checked;
            }

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
                temp.Add(key, mot);
            }

            // Salvataggio della nuova collection
            MassiveOperationUtils.ItemsStatus = temp;

            // Aggiornamento dello stato dei flag della griglia associata
            this.UpdateItemCheckingStatus();

            // Cambiamento del testo del controllo
            this.chkSelectDeselectAll.Text = "Deseleziona tutti";

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
                temp.Add(key, MassiveOperationUtils.ItemsStatus[key].Clone);

            // Salvataggio della nuova collection
            MassiveOperationUtils.ItemsStatus = temp;

            // Aggiornamento dello stato dei flag della griglia associata
            this.UpdateItemCheckingStatus();

            // Cambiamento del testo del controllo
            this.chkSelectDeselectAll.Text = "Seleziona tutti";

        }

        /// <summary>
        /// Funzione per l'inizializzazione del controllo
        /// </summary>
        /// <param name="systemIdList">Lista dei system id dei documenti o dei fascicoli</param>
        /// 
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
                    temp.Add(syd.Id, new MassiveOperationTarget(syd.Id,syd.Codice));

            // Salvataggio del dizionario
            MassiveOperationUtils.ItemsStatus = temp;
        }

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
                return String.Format("{0}/FirmaDigitale/DialogFirmaDigitale.aspx",
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
        /// Funzione utilizzata per verificare se c'è almeno un elemento selezionato
        /// e se è possibile procedere con l'avvio della funzione massiva richiesta
        /// </summary>
        /// <param name="dialogName">Il nome da assegnare alla funestra da aprire</param>
        /// <param name="function">Lo script da registrare in caso di successo delle verifica</param>
        private void UpdateSavedCheckingStatusAndCheckAtLeastOneSelected(
            string dialogName,
            string function)
        {
            // Lo script da immergere
            string script = "alert('Selezionare almeno un risultato!');";

            // Aggiornamento dello stato di checking salvato
            this.UpdateSavedCheckingStatus();

            // Se ci sono elementi selezionati, viene immerso il codice per l'apertura della finestra
            // altrimenti viene immerso il codice per mostrare un avviso
            if (MassiveOperationUtils.GetSelectedItems().Count > 0)
                script = function;

            this.Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                dialogName,
                script,
                true);

        }

        protected void btnSign_Click(object sender, ImageClickEventArgs e)
        {
            this.UpdateSavedCheckingStatusAndCheckAtLeastOneSelected(
                "FirmaMassiva",
                "OpenFirmaMassivaDocumenti();");
        }

        protected void btnFascicola_Click(object sender, ImageClickEventArgs e)
        {
            this.UpdateSavedCheckingStatusAndCheckAtLeastOneSelected(
                "FascicolaMassivo",
                "OpenFascicolaMassivo();");

        }

        protected void btnTransmit_Click(object sender, ImageClickEventArgs e)
        {
            this.UpdateSavedCheckingStatusAndCheckAtLeastOneSelected(
                "TramettiMassivo",
                "OpenTrasmissioneMassiva();");

        }

        protected void btnTimeStamp_Click(object sender, ImageClickEventArgs e)
        {
            this.UpdateSavedCheckingStatusAndCheckAtLeastOneSelected(
                "TimestampMassivo",
                "OpenTimestampMassivo();");

        }

        protected void btnConvert_Click(object sender, ImageClickEventArgs e)
        {
            this.UpdateSavedCheckingStatusAndCheckAtLeastOneSelected(
                "ConvertMassivo",
                "OpenConversioneMassivo();");

        }

        protected void btnWorkingArea_Click(object sender, ImageClickEventArgs e)
        {
            this.UpdateSavedCheckingStatusAndCheckAtLeastOneSelected(
                "WorkingAreaMassivo",
                "OpenADLMassiva('" + this.ObjType + "');");

        }

        protected void btnExport_Click(object sender, ImageClickEventArgs e)
        {
            this.UpdateSavedCheckingStatusAndCheckAtLeastOneSelected(
                "ExportResult",
                String.Format("OpenExportDialog('{0}')",
                this.ObjType == ObjTypeEnum.D ? "doc" : "fasc"));
        }

        protected void btnInoltra_Click(object sender, ImageClickEventArgs e)
        {
            this.UpdateSavedCheckingStatusAndCheckAtLeastOneSelected(
                "InoltraMassivo",
                "OpenInoltraDialog();");
        }

        protected void btnElVersioni_Click(object sender, ImageClickEventArgs e)
        {
            this.UpdateSavedCheckingStatusAndCheckAtLeastOneSelected(
                "EliminaVersioni",
                "OpenEliminaVersioniDialog();");
        }

    }
}
