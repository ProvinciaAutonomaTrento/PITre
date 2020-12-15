using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.ricercaDoc;
using DocsPAWA.SiteNavigation;
using DocsPAWA.UserControls;
using DocsPAWA.UserControls.ScrollElementsList;
using DocsPAWA.utils;
using System.Drawing;

namespace DocsPAWA.fascicolo
{
    public partial class NewDocListInProject : CssPage
    {
        #region Event Handler
        protected static DocsPAWA.DocsPaWR.DocsPaWebService wws = ProxyManager.getWS();
        protected string SortExpression;

        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Se non si è in postback si eseguono l'inizializzazione della pagina,
            // la ricerca dei documenti se ci sono filtri selezionati
            if (!IsPostBack)
            {
                this.Result = null;
                // this.ResultProfilati = null;
                this.CellPosition = null;
                this.showGridPersonalization = GridManager.IsRoleEnabledToUseGrids();
                this.InitializePage();
                if (this.showGridPersonalization)
                {
                    this.InitializeButtons();
                    this.InitializePageSize();
                }
                this.btnSalvaGrid.Visible = this.showGridPersonalization;
                this.btnCustomizeGrid.Visible = this.showGridPersonalization;
                this.btnPreferredGrid.Visible = this.showGridPersonalization;
                if (this.showGridPersonalization)
                {
                    this.btnPreferredGrid.Attributes.Add("onmouseover", "this.src='../images/ricerca/ico_griglie_preferite_hover.gif'");
                    this.btnPreferredGrid.Attributes.Add("onmouseout", "this.src='../images/ricerca/ico_griglie_preferite.gif'");
                    this.btnCustomizeGrid.Attributes.Add("onmouseover", "this.src='../images/ricerca/ico_doc2_hover.gif'");
                    this.btnCustomizeGrid.Attributes.Add("onmouseout", "this.src='../images/ricerca/ico_doc2.gif'");
                    this.btnSalvaGrid.Attributes.Add("onmouseover", "this.src='../images/ricerca/ico_salva_griglia_hover.gif'");
                    this.btnSalvaGrid.Attributes.Add("onmouseout", "this.src='../images/ricerca/ico_salva_griglia.gif'");
                }
                SetTema();
                mobButtons.PAGINA_CHIAMANTE = "docInFasc";
                //    this.btnRestorePreferredGrid.Visible = this.showGridPersonalization;
            }
            else
            {
                if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.DocumentInProject)
                    GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.DocumentInProject);
                // altrimenti vengono visualizzazi risultati memorizzati se ce ne sono
                if (this.Result != null && this.Result.Length > 0)
                {
                    // Visualizzazione dei risultati
                    FascicoliManager.setFolderSelezionato(this.Page, this.Folder);
                    this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage, this.Labels);
                }
                else
                {
                    this.ShowGrid(GridManager.SelectedGrid, this.Labels);
                }
            }

            if (this.showGridPersonalization)
            {
                EnableDisableSave();
                //   EnableDisableRestore();
            }

        }

        protected void InitializeButtons()
        {
            // Associazione della funzione javascript per l'apertura della pagina
            // di personalizzazione della griglia

            string isAdl = string.Empty;

            if (!String.IsNullOrEmpty(Request["ricADL"]))
            {
                isAdl = "ricADL";
            }

            this.btnCustomizeGrid.OnClientClick = String.Format(
                "OpenGrids('{0}','{1}','{2}','{3}');",
                GridManager.SelectedGrid.GridType,
                String.Empty,
                String.Empty,
                isAdl);

            // Associazione della funzione javascript per l'apertura della pagina
            // di personalizzazione della griglia
            this.btnPreferredGrid.OnClientClick = String.Format(
                "OpenPreferredGrids('{0}','{1}');",
                GridManager.SelectedGrid.GridType,
                isAdl);

            //Cancellare quando tutto finito e utilizzare enabled
            this.btnSalvaGrid.OnClientClick = String.Format(
               "OpenSaveGrid('{0}','{1}');",
               GridManager.SelectedGrid.GridType,
               isAdl);
            //

            //Cancellare quando tutto finito e utilizzare enabled
            //       this.btnRestorePreferredGrid.OnClientClick = String.Format(
            //          "ReturnDefaultGrid('{0}','{1}');",
            //           GridManager.SelectedGrid.GridType,
            //          isAdl);
            //
        }

        /// <summary>
        /// Al cambio di pagina, vengono caricati i documenti per la pagina selezionati
        /// e vengono visualizzati
        /// </summary>
        protected void dgResult_SelectedPageIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {
            // Aggiornamento del numero di pagina memorizzato nel call context
            this.dgResult.CurrentPageIndex = e.NewPageIndex;
            this.SelectedPage = e.NewPageIndex + 1;

            // Ricerca dei documenti e visualizzazione dei risultati
            this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels, this.Folder, this.OrderFilters);

            // Aggiornamento dello stato di selezione degli item della griglia secondo quanto
            // memorizzato dalla bottoniera
            this.mobButtons.UpdateItemCheckingStatus();

        }

        protected void dgResult_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }

            if (this.showGridPersonalization)
            {
                //Posizione della freccetta nell'header
                if (e.Item.ItemType == ListItemType.Header)
                {
                    System.Web.UI.WebControls.Image arrow = new System.Web.UI.WebControls.Image();

                    arrow.BorderStyle = BorderStyle.None;

                    if (GridManager.SelectedGrid.OrderDirection == OrderDirectionEnum.Asc)
                    {
                        arrow.ImageUrl = "../images/ricerca/arrow_up.gif";
                    }
                    else
                    {
                        arrow.ImageUrl = "../images/ricerca/arrow_down.gif";
                    }

                    if (GridManager.SelectedGrid.FieldForOrder != null)
                    {
                        Field d = (Field)GridManager.SelectedGrid.Fields.Where(f => f.Visible && f.FieldId.Equals(GridManager.SelectedGrid.FieldForOrder.FieldId)).FirstOrDefault();
                        if (d != null)
                        {
                            int cell = this.CellPosition[d.FieldId];
                            e.Item.Cells[cell].Controls.Add(arrow);
                        }
                    }
                    //else
                    //{
                    //    //Campo Data protocollazione / Creazione
                    //    Field d = (Field)GridManager.SelectedGrid.Fields.Where(f => f.Visible && f.FieldId.Equals("D9")).FirstOrDefault();
                    //    if (d != null)
                    //    {
                    //        int cell = this.CellPosition[d.FieldId];
                    //        e.Item.Cells[cell].Controls.Add(arrow);
                    //    }
                    //}
                }
            }

        }

        /// <summary>
        /// Questo evento viene scatenato quando viene chiusa una maschera di azioni massive che prevede il ricaricamento
        /// dei risultati
        /// </summary>
        protected void OnTerminateMassiveOperation(object sender, EventArgs e)
        {
            this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels, this.Folder, this.OrderFilters);
            this.mobButtons.UpdateItemCheckingStatus();
            this.mdlPopupWait.Hide();
        }

        /// <summary>
        /// Questo evento viene associato dal questo controllo al checked changed
        /// delle checkbox per la selezione / deselezione dell'item
        /// </summary>
        protected void chkSelectDeselect_CheckedChanged(object sender, EventArgs e)
        {
            // Campo nascosto in cui è presente il system id dell'oggetto di cui cambiare
            // lo stato di selezione e checkbox per il cambio stato
            HiddenField hiddenField = null;
            CheckBox checkBox;

            // Casting della checkbox
            checkBox = sender as CheckBox;

            // Recupero del campo nascosto, situato nel parent della checkbox sender
            foreach (Control ctrl in checkBox.NamingContainer.Controls)
                if (ctrl.GetType().Equals(typeof(HiddenField)))
                    hiddenField = ctrl as HiddenField;

            // Impostazione dello stato di selezione dell'item selezionato
            this.mobButtons.SetState(hiddenField.Value, checkBox.Checked);

        }

        /// <summary>
        /// Evento scatenato quando viene conclusa un'operazione riguardante l'area di lavoro
        /// </summary>
        protected void OnWorkingAreaOperationCompleted(object sender, EventArgs e)
        {
            // Casting degli argomenti al tipo appropriato
            NewSearchIconsEventArgs args = e as NewSearchIconsEventArgs;

            // Se l'esito dell'operazione è positivo, viene aggiornato lo stato di inserimento
            // nell'area di lavoro del documento 
            if (args.Result)
            {
                SearchObject doc = this.Result.Where(d => d.SearchObjectID == args.ObjectId).FirstOrDefault();

                if (doc != null)
                {
                    string inAdl = doc.SearchObjectField.Where(t => t.SearchObjectFieldID.Equals("IN_ADL")).FirstOrDefault().SearchObjectFieldValue;
                    if (inAdl == "1")
                    {
                        doc.SearchObjectField.Where(t => t.SearchObjectFieldID.Equals("IN_ADL")).FirstOrDefault().SearchObjectFieldValue = "0";
                    }
                    else
                    {
                        doc.SearchObjectField.Where(t => t.SearchObjectFieldID.Equals("IN_ADL")).FirstOrDefault().SearchObjectFieldValue = "1";
                    }
                }
            }

        }

        /// <summary>
        /// Evento scatenato quando viene conclusa un'operazione riguardante l'area di conservazione
        /// </summary>
        protected void OnStorageAreaOperationCompleted(object sender, EventArgs e)
        {
            // Casting degli argomenti al tipo appropriato
            NewSearchIconsEventArgs args = e as NewSearchIconsEventArgs;

            // Se l'esito dell'operazione è positivo, viene aggiornato lo stato di inserimento
            // nell'area di conservazione del documento 
            if (args.Result)
            {
                SearchObject doc = this.Result.Where(d => d.SearchObjectID == args.ObjectId).FirstOrDefault();

                if (doc != null)
                {
                    string inConservazione = doc.SearchObjectField.Where(t => t.SearchObjectFieldID.Equals("IN_CONSERVAZIONE")).FirstOrDefault().SearchObjectFieldValue;
                    if (inConservazione == "1")
                    {
                        doc.SearchObjectField.Where(t => t.SearchObjectFieldID.Equals("IN_CONSERVAZIONE")).FirstOrDefault().SearchObjectFieldValue = "0";
                    }
                    else
                    {
                        doc.SearchObjectField.Where(t => t.SearchObjectFieldID.Equals("IN_CONSERVAZIONE")).FirstOrDefault().SearchObjectFieldValue = "1";
                    }
                }
            }
        }

        /// <summary>
        /// Evento scatenato quando viene visualizzata l'immagine associata al documento
        /// </summary>
        protected void OnViewImageCompleted(object sender, EventArgs e)
        {
            // Casting degli argomenti al tipo appropriato
            NewSearchIconsEventArgs args = e as NewSearchIconsEventArgs;

            // Evidenziazione della riga del documento
            this.SelectItem(args.ObjectId);

        }

        protected void OnViewSignDetailsCompleted(object sender, EventArgs e)
        {
            // Casting degli argomenti al tipo appropriato
            NewSearchIconsEventArgs args = e as NewSearchIconsEventArgs;

            // Evidenziazione della riga del documento
            this.SelectItem(args.ObjectId);

        }

        #endregion

        #region Funzioni di utilità

        /// <summary>
        /// Funzione per l'inizializzazione deu componenti
        /// </summary>
        private void InitializeComponent()
        {
            this.mobButtons.OnTerminateMassiveOperation += new EventHandler(OnTerminateMassiveOperation);
            this.mobButtons.OnTerminateMassiveOperationJS = "$find('mdlWait').show()";
        }

        /// <summary>
        /// Funzione responsabile dell'inizializzazione della pagina
        /// </summary>
        private void InitializePage()
        {
            // Startup della pagina
            Utils.startUp(this);

            this.CellPosition = new Dictionary<string, int>();

            // Inserimento Christian
            // Prelevamento del folder
            this.Folder = this.GetFolder();

            // Se il callcontext è valorizzato e si è tornati su questa pagina tramite
            // pressione del tasto Back non vengono ricaricati i filtri, le etichette e la
            // griglia ma ci si limita ad impostare a false l'id back
            if (CallContextStack.CurrentContext != null &&
                CallContextStack.CurrentContext.IsBack)
                // Azzeramento dell'Is back
                CallContextStack.CurrentContext.IsBack = false;
            else
            {
                // Inizializzazione della bottoniera
                this.mobButtons.InitializeOrUpdateUserControl(new SearchResultInfo[] { });

                // Prelevamento delle lettere di protocollo
                this.Labels = DocumentManager.GetLettereProtocolli(this);

                // Prelevamento dei filtri di ricerca
                this.SearchFilters = FiltriRicercaDocumenti.CurrentFilterSessionStorage.GetCurrentFilter();

                this.OrderFilters = FiltriRicercaDocumenti.CurrentFilterSessionOrderFilter.GetCurrentFilter();

                // Prelevamento del folder
                this.Folder = this.GetFolder();

                // Reset del count documenti, indice dell'elemento selezionato e numero di pagina
                this.RecordCount = 0;
                this.SelectedPage = 1;

                // Caricamento della griglia se non ce n'è una già selezionata
                if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.DocumentInProject)
                    GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.DocumentInProject);
            }

            string noResult = string.Empty;
            if (!String.IsNullOrEmpty(Request.QueryString["noRic"]) &&
              Request.QueryString["noRic"] == "1")
            {
                noResult = Request.QueryString["noRic"].ToString();
            }

            if (this.Folder != null && string.IsNullOrEmpty(noResult))
            {
                //
                // Viene impostato il systemId a 0 nel caso in cui il fascicolo ritorni dalla sessione con SystemID null - ticket 2115
                if (string.IsNullOrEmpty(this.Folder.systemID))
                    this.Folder.systemID = "0";
                this.SearchDocumentsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Labels, this.Folder, this.OrderFilters);
            }
            else
            {
                mobButtons.NUM_RESULT = "0";
                this.ShowGrid(GridManager.SelectedGrid, this.Labels);
            }

        }

        /// <summary>
        /// Funzione per il reperimento del folder in cui ricercare i documenti
        /// </summary>
        /// <returns>Il folder in cui ricercare i documenti</returns>
        private Folder GetFolder()
        {
            // Folder da restituire
            Folder toReturn;

            Hashtable hashFolder = FascicoliManager.getHashFolder(this);

            int idFolder = 0;
            if (!String.IsNullOrEmpty(Request["idFolder"]))
                Int32.TryParse(Request["idFolder"], out idFolder);

            // Reperimento del folder dall'hashFolder
            toReturn = hashFolder[idFolder] as Folder;

            // Restituzione del folder
            return toReturn;
            
        }

        /// <summary>
        /// Questa funzione si occupa di ricercare i documenti e di visualizzare 
        /// i dati
        /// </summary>
        private void SearchDocumentsAndDisplayResult(FiltroRicerca[][] searchFilters, int selectedPage, Grid selectedGrid, EtichettaInfo[] labels, Folder folder, FiltroRicerca[][] orderFilters)
        {
            // Lista dei documenti risultato della ricerca
            SearchObject[] result = null;

            // Numero di documenti restituiti dalla ricerca
            int recordNumber = 0;

            // Ricerca dei documenti
            result = this.SearchDocument(searchFilters, selectedPage, out recordNumber, folder, orderFilters);
            this.RecordCount = recordNumber;

            // Se ci sono risultati, vengono visualizzati
            if (result != null)
            {
                // Visualizzazione dei risultati
                this.ShowResult(selectedGrid, result, recordNumber, selectedPage, labels);
            }

            if (result != null && result.Length == 0)
            {
                // this.dgResult.DataSource = null;
                // this.dgResult.DataBind();
                this.ShowGrid(GridManager.SelectedGrid, this.Labels);
            }

            mobButtons.NUM_RESULT = recordNumber.ToString();
            
        }

        /// <summary>
        /// Funzione per la ricerca dei documenti
        /// </summary>
        /// <param name="searchFilters">Filtri da utilizzare per la ricerca</param>
        /// <param name="selectedPage">Pagina da caricare</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        /// <param name="folder">Folder in cui ricercare</param>
        /// <returns>Lista delle informazioni sui documenti restituiti dalla ricerca</returns>
        private SearchObject[] SearchDocument(FiltroRicerca[][] searchFilters, int selectedPage, out int recordNumber, Folder folder, FiltroRicerca[][] orderFilters)
        {
            // Documenti individuati dalla ricerca
            SearchObject[] toReturn;

            // Numero totale di pagine
            int pageNumbers;

            // Lista dei system id dei documenti restituiti dalla ricerca
            SearchResultInfo[] idProfiles;

            // Recupero dei campi della griglia impostati come visibili
            Field[] visibleArray = null;
            List<Field> visibleFieldsTemplate;

            if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.DocumentInProject)
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.DocumentInProject);

            visibleFieldsTemplate = GridManager.SelectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field)) && e.CustomObjectId != 0).ToList();

            if (visibleFieldsTemplate != null && visibleFieldsTemplate.Count > 0)
            {
                visibleArray = visibleFieldsTemplate.ToArray();
            }

            toReturn = FascicoliManager.getListaDocumentiPagingCustom(
                    this, 
                    folder, 
                    searchFilters, 
                    selectedPage, 
                    out pageNumbers, 
                    out recordNumber,
                    !IsPostBack,
                    this.showGridPersonalization,
                    false,
                    visibleArray,
                    null,
                    this.pageSize,
                    orderFilters,
                    out idProfiles);

            // Memorizzazione del numero di risultati restituiti dalla ricerca, del numero di pagine e dei risultati
            this.RecordCount = recordNumber;
            this.PageCount = pageNumbers;
            this.Result = toReturn;

            // Memorizzazione dei dati sullo scroller dei documenti in fascicolo
            ScrollManager.setInContextNewObjScrollElementsList(
                this.RecordCount,
                this.PageCount,
                this.pageSize,
                0,
                this.SelectedPage - 1,
                new ArrayList(this.Result),
                ObjScrollElementsList.EmunSearchContext.RICERCA_DOC_IN_FASC);

            // Impostazione del titolo nella pagina e del numero di documenti trovati
            this.lblTitle.Text = recordNumber + " documenti";

            // Se la lista dei documenti è popolata, viene inizializzata la barra
            if (idProfiles != null &&
                idProfiles.Length > 0)
                this.mobButtons.InitializeOrUpdateUserControl(idProfiles);

            // Restituzione della lista di documenti da visualizzare
            return toReturn;

        }

        /// <summary>
        /// Funzione per l'inizializzazione della griglia
        /// </summary>
        private void InitializeGrid(Grid selectedGrid)
        {
            // Lista dei campi della griglia da visualizzare ordinati per position crescente
            List<Field> gridFields;

            // Una colonna del datagrid
            DataGridColumn column = null;

            this.CellPosition.Clear();

            // Larghezza da assegnare alla griglia
            int gridWidth = 0;

            // Posizione occupata dal campo all'interno della griglia
            int position = 0;

            // Reperimento dei campi da visualizzare ordinati per Posizione crescente
            gridFields = selectedGrid.Fields.Where(e => e.Visible).OrderBy(f => f.Position).ToList();

            //Ripristino contatore senza griglie custom
            if (this.SearchFilters != null && this.SearchFilters.Length > 0)
            {
                Templates templateTemp = Session["templateRicerca"] as Templates;

                OggettoCustom customObjectTemp = new OggettoCustom();

                if (templateTemp != null && !this.showGridPersonalization)
                {
                    customObjectTemp = templateTemp.ELENCO_OGGETTI.Where(
                         e => e.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && e.DA_VISUALIZZARE_RICERCA == "1").FirstOrDefault();

                    Field d = new Field();

                    if (customObjectTemp != null)
                    {
                        d.AssociatedTemplateName = templateTemp.DESCRIZIONE;
                        d.CustomObjectId = customObjectTemp.SYSTEM_ID;
                        d.FieldId = "CONTATORE";
                        d.IsNumber = true;
                        d.Label = customObjectTemp.DESCRIZIONE;
                        d.OriginalLabel = customObjectTemp.DESCRIZIONE;
                        d.OracleDbColumnName = "to_number(getcontatoredocordinamento (a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "'))";
                        d.SqlServerDbColumnName = "@dbUser@.getContatoreDocOrdinamento(a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "')";
                        gridFields.Insert(2, d);
                    }
                    else
                    {
                        gridFields.Remove(d);
                    }
                }
            }

            ///

            // Cancellazione delle colonne del datagrid
            this.dgResult.Columns.Clear();

            // Per ogni campo della griglia...
            foreach (Field field in gridFields)
            {
                // Se il campo è un campo speciale,
                // Viene richiamata la funzione per la creazione della colonna
                // speciale appropriata
                if (field is SpecialField)
                    column = GridManager.GetSpecialColumn(
                        (SpecialField)field,
                        this.mobButtons,
                        "IdProfile",
                        this.OnWorkingAreaOperationCompleted,
                        this.OnStorageAreaOperationCompleted,
                        this.OnViewImageCompleted,
                        this.OnViewSignDetailsCompleted,
                        this,
                        NewSearchIcons.ObjectTypeEnum.Document,
                        NewSearchIcons.ParentPageEnum.SearchDocumentInProject,
                        position);
                else
                {
                    if (field.FieldId.Equals("CONTATORE"))
                    {
                        column = GridManager.GetBoundColumn(
                                field.Label,
                                field.OriginalLabel,
                                100,
                                field.FieldId);
                    }
                    else
                    {
                        // Altrimenti si procede con la creazione di una colonna normale
                        if (field.OriginalLabel.ToUpper().Equals("DOCUMENTO"))
                            column = GridManager.GetLinkColumn(field.Label, field.FieldId, field.Width);
                        else
                        {
                            column = GridManager.GetBoundColumn(
                                field.Label,
                                field.OriginalLabel,
                                field.Width,
                                field.FieldId);
                        }
                    }
                }

                column.SortExpression = field.FieldId;

                // ...aggiunta della colonna creata alla collezione della colonne del datagrid
                this.dgResult.Columns.Add(column);

                // ...aggiornamento della larghezza
                gridWidth += field.Width;

                if (!this.CellPosition.ContainsKey(field.FieldId))
                {
                    CellPosition.Add(field.FieldId, position);
                }
                // Aggiornamento della posizione
                position += 1;
            }

            // Impostazione della larghezza della griglia
            //this.dgResult.Width = new Unit(gridWidth, UnitType.Pixel);


        }

        /// <summary>
        /// Funzione per la visualizzazione dei risutati della ricerca
        /// </summary>
        /// <param name="result">I risultati della ricerca</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        private void ShowResult(Grid selectedGrid, SearchObject[] result, int recordNumber, int selectedPage, EtichettaInfo[] labels)
        {
            // Il dataset da associare al datagrid
            DataSet dataSet;

            // Impostazione del numero di elementi virtuale e del numero di pagina
            this.dgResult.VirtualItemCount = recordNumber;
            this.dgResult.CurrentPageIndex = selectedPage - 1;

            // Inizializzazione della griglia
            this.InitializeGrid(selectedGrid);

            // Creazione del dataset da associare al datagrid
            dataSet = GridManager.InitializeDataSet(selectedGrid);

            // Aggiunta dei campi IdProfile, NavigateUrl, FileExtension, IsInStorageArea,
            // IsInWorkingArea, IsSigned, ProtoType al dataset.
            dataSet.Tables[0].Columns.Add("IdProfile", typeof(String));
            dataSet.Tables[0].Columns.Add("NavigateUrl", typeof(String));
            dataSet.Tables[0].Columns.Add("FileExtension", typeof(String));
            dataSet.Tables[0].Columns.Add("IsInStorageArea", typeof(Boolean));
            dataSet.Tables[0].Columns.Add("IsInWorkingArea", typeof(Boolean));
            dataSet.Tables[0].Columns.Add("IsSigned", typeof(Boolean));
            dataSet.Tables[0].Columns.Add("ProtoType", typeof(String));

            if (this.SearchFilters != null && this.SearchFilters.Length > 0)
            {
                Templates templateTemp = Session["templateRicerca"] as Templates;

                OggettoCustom customObjectTemp = new OggettoCustom();

                if (templateTemp != null && !this.showGridPersonalization)
                {
                    customObjectTemp = templateTemp.ELENCO_OGGETTI.Where(
                         e => e.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && e.DA_VISUALIZZARE_RICERCA == "1").FirstOrDefault();
                    if (customObjectTemp != null)
                    {
                        dataSet.Tables[0].Columns.Add("CONTATORE", typeof(String));
                    }
                }
            }

            // Compilazione del dataset
            this.FillDataSet(dataSet.Tables[0], result, selectedGrid, labels);

            // Associazione del data set alla griglia e databind
            this.dgResult.DataSource = dataSet;
            this.dgResult.DataBind();

            mobButtons.NUM_RESULT = recordNumber.ToString();

            // Impostazione del titolo nella pagina e del numero di documenti trovati
            this.lblTitle.Text = String.Format(" Elenco documenti - Trovati {0} documenti.", recordNumber);

            string gridType = string.Empty;

            if (this.showGridPersonalization)
            {
                gridType = " [Griglia Standard] ";
                if (selectedGrid != null && string.IsNullOrEmpty(selectedGrid.GridId))
                {
                    gridType = " <span class=\"rosso\">[Griglia Temporanea]</span> ";
                }
                else
                {
                    if (!(selectedGrid.GridId).Equals("-1"))
                    {
                        gridType = " [" + selectedGrid.GridName + "] ";
                    }
                }
            }

            this.lblArea.Text = "Ricerca documenti" + gridType;

            this.SelectItem(SearchUtils.GetObjectId());

        }

        /// <summary>
        /// Funzione per la compilazione del datagrid da associare al datagrid
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="result"></param>
        private void FillDataSet(DataTable dataTable, SearchObject[] result, Grid selectedGrid, EtichettaInfo[] labels)
        {
            // Lista dei campi della griglia che risultano visibili
            List<Field> visibleFields;

            // La riga da aggiungere al dataset
            DataRow dataRow;

            // Valore da assegnare ad un campo
            String value = null;

            StringBuilder temp;

            // Il template di ricerca selezionato
            Templates template;

            // L'oggetto custom contatore
            OggettoCustom customObject = null;

            // Colore da assegnare alla descrizione del documento secondo quanto memorizzato nella'amministrazione
            String documentDescriptionColor;

            // Numero di caratteri a cui troncare il testo
            int maxLength = 0;

            // Recupero dei campi della griglia impostati come visibili
            visibleFields = selectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field))).ToList();

            // Prelevamento del template di ricerca eventualmente selezionato
            template = Session["templateRicerca"] as Templates;

            //Ripristino contatore senza griglie custom
            Templates templateTemp = Session["templateRicerca"] as Templates;

            OggettoCustom customObjectTemp = new OggettoCustom();

            if (template != null && !this.showGridPersonalization)
            {
                customObjectTemp = templateTemp.ELENCO_OGGETTI.Where(
                     e => e.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && e.DA_VISUALIZZARE_RICERCA == "1").FirstOrDefault();

                Field d = new Field();

                if (customObjectTemp != null)
                {
                    d.AssociatedTemplateName = templateTemp.DESCRIZIONE;
                    d.CustomObjectId = customObjectTemp.SYSTEM_ID;
                    d.FieldId = "CONTATORE";
                    d.IsNumber = true;
                    d.Label = customObjectTemp.DESCRIZIONE;
                    d.OriginalLabel = customObjectTemp.DESCRIZIONE;
                    d.OracleDbColumnName = "to_number(getcontatoredocordinamento (a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "'))";
                    d.SqlServerDbColumnName = "@dbUser@.getContatoreDocOrdinamento(a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "')";
                    visibleFields.Insert(2, d);
                }
                else
                {
                    visibleFields.Remove(d);
                }
            }


            // Individuazione del colore da assegnare alla descrizione del documento
            switch (new DocsPaWebService().getSegnAmm(UserManager.getInfoUtente(this).idAmministrazione))
            {
                case "0":
                    documentDescriptionColor = "Black";
                    break;
                case "1":
                    documentDescriptionColor = "Blue";
                    break;
                default:
                    documentDescriptionColor = "Red";
                    break;
            }

            // Per ogni risultato...
            foreach (SearchObject doc in result)
            {
                // ...viene inizializzata una nuova riga
                dataRow = dataTable.NewRow();

                // ...impostazione dell'id profile
                dataRow["IdProfile"] = doc.SearchObjectID;

                foreach (Field field in visibleFields)
                {
                    switch (field.FieldId)
                    {
                        //SEGNATURA
                        case "D8":
                            value = "<span style=\"color:Red; font-weight:bold;\">" + doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue + "</span>";
                            break;
                        //REGISTRO
                        case "D2":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //TIPO
                        case "D3":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_DOCUMENTO_PRINCIPALE")).FirstOrDefault().SearchObjectFieldValue;
                            string tempVal = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            if (!string.IsNullOrEmpty(value))
                            {
                                value = labels.Where(e => e.Codice == "ALL").FirstOrDefault().Descrizione;
                            }
                            else
                            {
                                value = labels.Where(e => e.Codice == tempVal).FirstOrDefault().Descrizione;
                            }
                            break;
                        //OGGETTO
                        case "D4":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //MITTENTE / DESTINATARIO
                        case "D5":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //MITTENTE
                        case "D6":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //DESTINATARI
                        case "D7":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //DATA
                        case "D9":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        // ESITO PUBBLICAZIONE
                        case "D10":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //DATA ANNULLAMENTO
                        case "D11":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //DOCUMENTO
                        case "D1":
                            // Inizializzazione dello stringbuilder con l'apertura del tag Span in
                            // cui inserire l'identiifcativo del documento
                            string numeroDocumento = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            string numeroProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue;
                            string dataProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
                            string dataApertura = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
                            string protTit = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("PROT_TIT")).FirstOrDefault().SearchObjectFieldValue;

                            temp = new StringBuilder("<span style=\"color:");
                            // Se il documento è un protocollo viene colorato in rosso altrimenti
                            // viene colorato in nero
                            temp.Append(String.IsNullOrEmpty(numeroProtocollo) ? "Black" : documentDescriptionColor);
                            // Il testo deve essere grassetto
                            temp.Append("; font-weight:bold;\">");

                            // Creazione dell'informazione sul documento
                            if (!String.IsNullOrEmpty(numeroProtocollo))
                            {
                                temp.Append(numeroProtocollo);
                                temp.Append("<br />");
                                temp.Append(dataProtocollo);
                            }
                            else
                            {
                                temp.Append(numeroDocumento);
                                temp.Append("<br />");
                                temp.Append(dataApertura);
                            }

                            if (!String.IsNullOrEmpty(protTit))
                                temp.Append("<br />" + protTit);

                            // Chiusura del tag span
                            temp.Append("</span>");

                            value = temp.ToString();
                            break;
                        //NUMERO PROTOCOLLO
                        case "D12":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //AUTORE
                        case "D13":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //DATA ARCHIVIAZIONE
                        case "D14":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //PERSONALE
                        case "D15":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                            {
                                value = "Si";
                            }
                            else
                            {
                                value = "No";
                            }
                            break;
                        //PRIVATO
                        case "D16":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                            {
                                value = "Si";
                            }
                            else
                            {
                                value = "No";
                            }
                            break;
                        //TIPOLOGIA
                        case "U1":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //NOTE
                        case "D17":
                            string valoreChiave;
                             valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_IS_PRESENT_NOTE");

                             if (!string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("1"))
                             {
                                 value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ESISTE_NOTA")).FirstOrDefault().SearchObjectFieldValue;
                             }
                             else
                             {
                                 value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                             }
                            break;
                        //CONTATORE
                        case "CONTATORE":
                            try
                            {
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                //verifico se si tratta di un contatore di reertorio
                                if (value.ToUpper().Equals("#CONTATORE_DI_REPERTORIO#"))
                                {
                                    //reperisco la segnatura di repertorio
                                    string dNumber = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                                    value = DocumentManager.getSegnaturaRepertorio(this.Page, dNumber, UserManager.getInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione).Codice);
                                }
                            }
                            catch(Exception e)
                            {
                                value="";
                            }
                        break;
                        //COD. FASCICOLI
                        case "D18":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //Nome e cognome autore
                        case "D19":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //Ruolo autore
                        case "D20":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //Data arrivo
                        case "D21":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //Stato del documento
                        case "D22":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        case "IMPRONTA":
                            // IMPRONTA FILE
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //OGGETTI CUSTOM
                        default:
                            if (!string.IsNullOrEmpty(doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue))
                            {
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                // se l'ggetto custom è un contatore di repertorio visualizzo la segnatura di repertorio
                                if (value.ToUpper().Equals("#CONTATORE_DI_REPERTORIO#"))
                                {
                                    string idDoc = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                                    value = DocumentManager.getSegnaturaRepertorio(this.Page, idDoc, UserManager.getInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione).Codice);
                                }
                            }
                            else
                            {
                                value = "";
                            }
                        break;
                    }

                    // Valorizzazione del campo fieldName
                    // Se il documento è annullato, viene mostrato un testo barrato, altrimenti
                    // viene mostrato così com'è
                    string dataAnnullamento = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D11")).FirstOrDefault().SearchObjectFieldValue;
                    if (!String.IsNullOrEmpty(dataAnnullamento))
                        dataRow[field.FieldId] = String.Format("<span style=\"text-decoration: line-through; color: Red;\">{0}</span>", value);
                    else
                        dataRow[field.FieldId] = value;

                }

                string tipoProto = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D3")).FirstOrDefault().SearchObjectFieldValue;

                // Impostazione del campo NavigateUrl se l'utente ha i diritti per
                // visualizzare il documento
                dataRow["NavigateUrl"] = this.GetUrlForDocumentDetails(UserManager.getInfoUtente(this), doc.SearchObjectID, tipoProto);

                // Impostazione dei campi FileExtension, IsInStorageArea,
                // IsInWorkingArea, IsSigned e ProtoType
                string immagineAcquisita = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D23")).FirstOrDefault().SearchObjectFieldValue;
                string inConservazione = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_CONSERVAZIONE")).FirstOrDefault().SearchObjectFieldValue;
                string inAdl = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_ADL")).FirstOrDefault().SearchObjectFieldValue;
                string isFirmato = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("CHA_FIRMATO")).FirstOrDefault().SearchObjectFieldValue;

                dataRow["FileExtension"] = !String.IsNullOrEmpty(immagineAcquisita) && immagineAcquisita != "0" ?
                    immagineAcquisita :
                    String.Empty;
                dataRow["IsInStorageArea"] = !String.IsNullOrEmpty(inConservazione) && inConservazione != "0" ? true : false;
                dataRow["IsInWorkingArea"] = !String.IsNullOrEmpty(inAdl) && inAdl != "0" ? true : false;
                dataRow["IsSigned"] = !String.IsNullOrEmpty(isFirmato) && isFirmato != "0" ? true : false;
                dataRow["ProtoType"] = tipoProto;

                // ...aggiunta della riga alla collezione delle righe
                dataTable.Rows.Add(dataRow);

            }

        }

        /// <summary>
        /// Funzione per la verifica dei diritti di accesso al documento da parte dell'utente
        /// e per la restituzione dell'eventuale link per la sua visualizzazione
        /// </summary>
        /// <param name="infoUtente">Informazioni sull'utente</param>
        /// <param name="idProfile">Id del documento</param>
        /// <param name="protoType">Tipo di documento</param>
        /// <returns>L'eventuale link per la visualizzazione del dettaglio del documento</returns>
        private String GetUrlForDocumentDetails(InfoUtente infoUtente, string idProfile, String protoType)
        {
            // Il link da resttiuire
            String toReturn = String.Empty;

            // I diritti dell'utente sul documento
            int rights;

            // Il messaggio restituito dalla funzione per la verfica dei diritti
            String message;

            // Recupero dei diritti sul documento
            rights = DocumentManager.verificaACL("D", idProfile, infoUtente, out message);

            // Se pari a 2, ha i diritti
            if (rights == 2)
            {
                // Viene rimosso l'eventuale fascicolo selezionato per la ricerca per evitare che
                // questo venga visualizzato come fascicolo nella maschera di fascicolazione rapida
                FascicoliManager.removeFascicoloSelezionatoFascRapida();

                // L'indirizzo da associare è ../documento/GestioneDoc.aspx?tab=protocollo&from=newRicDoc&idProfile=<idProfile>&protoType=<protoType>
                toReturn = String.Format("../documento/GestioneDoc.aspx?tab=protocollo&from=newRicDoc&idProfile={0}&protoType={1}",
                    idProfile,
                    protoType);
            }

            // Restituzione del valore calcolato
            return toReturn;
        }

        /// <summary>
        /// Funzione per la selezione dell'item con le informazioni sull'ultimo elemento
        /// di cui si è visto il dettaglio prima di abbandonare la pagina
        /// </summary>
        /// <param name="documentId">Id del documento da evidenziare</param>
        private void SelectItem(String id)
        {
            // Reperimento del documento selezionato, se esiste
            SearchObject doc = this.Result.Where(d => d.SearchObjectID == id).FirstOrDefault();

            // Se il documento è stato reperito con successo, viene selezionata la riga
            // corrispondente nella griglia
            if (doc != null)
            {
                int index = Array.IndexOf(this.Result, doc);

                this.dgResult.SelectedIndex = index;
            }
            else
                this.dgResult.SelectedIndex = -1;
        }

        protected bool isVisibleColEsitoPubblicazione()
        {
            bool result = false;
            if (Session["templateRicerca"] != null)
            {
                DocsPaWR.Templates template = (DocsPaWR.Templates)Session["templateRicerca"];
                result = wws.isExsistDocumentPubblicazione(template.ID_TIPO_ATTO);

            }

            return result;
        }

        /// <summary>
        /// Funzione per la visualizzazione dei risutati della ricerca
        /// </summary>
        /// <param name="result">I risultati della ricerca</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        private void ShowGrid(Grid selectedGrid, EtichettaInfo[] labels)
        {
            // Il dataset da associare al datagrid
            DataSet dataSet;

            // Inizializzazione della griglia
            this.InitializeGrid(selectedGrid);

            // Creazione del dataset da associare al datagrid
            dataSet = GridManager.InitializeDataSet(selectedGrid);

            // Aggiunta dei campi IdProfile, NavigateUrl, FileExtension, IsInStorageArea,
            // IsInWorkingArea, IsSigned, ProtoType al dataset.
            dataSet.Tables[0].Columns.Add("IdProfile", typeof(String));
            dataSet.Tables[0].Columns.Add("NavigateUrl", typeof(String));
            dataSet.Tables[0].Columns.Add("FileExtension", typeof(String));
            dataSet.Tables[0].Columns.Add("IsInStorageArea", typeof(Boolean));
            dataSet.Tables[0].Columns.Add("IsInWorkingArea", typeof(Boolean));
            dataSet.Tables[0].Columns.Add("IsSigned", typeof(Boolean));
            dataSet.Tables[0].Columns.Add("ProtoType", typeof(String));

            // Compilazione dell'header con tabella vuota
            //  this.FillDataSetHeader(dataSet.Tables[0], selectedGrid, labels);

            Templates templateTemp = Session["templateRicerca"] as Templates;

            OggettoCustom customObjectTemp = new OggettoCustom();

            if (templateTemp != null && !this.showGridPersonalization)
            {
                customObjectTemp = templateTemp.ELENCO_OGGETTI.Where(
                     e => e.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && e.DA_VISUALIZZARE_RICERCA == "1").FirstOrDefault();
                if (customObjectTemp != null)
                {
                    dataSet.Tables[0].Columns.Add("CONTATORE", typeof(String));
                }
            }

            // Associazione del data set alla griglia e databind
            this.dgResult.DataSource = dataSet;
            this.dgResult.DataBind();

            mobButtons.NUM_RESULT = "0";

            // Impostazione del titolo nella pagina e del numero di documenti trovati
            this.lblTitle.Text = String.Format(" Elenco documenti - Trovati {0} documenti.", "0");

            string gridType = string.Empty;

            if (this.showGridPersonalization)
            {
                gridType = " [Griglia Standard] ";
                if (selectedGrid != null && string.IsNullOrEmpty(selectedGrid.GridId))
                {
                    gridType = " <span class=\"rosso\">[Griglia Temporanea]</span> ";
                }
                else
                {
                    if (!(selectedGrid.GridId).Equals("-1"))
                    {
                        gridType = " [" + selectedGrid.GridName + "] ";
                    }
                }
            }

            this.lblArea.Text = "Ricerca documenti" + gridType;
         

        }

        protected void EnableDisableSave()
        {
            if (GridManager.SelectedGrid != null && !string.IsNullOrEmpty(GridManager.SelectedGrid.GridId) && GridManager.SelectedGrid.GridId.Equals("-1"))
            {
                this.btnSalvaGrid.Enabled = false;
            }
            else
            {
                this.btnSalvaGrid.Enabled = true;
            }
        }

        protected void Sort_Grid(Object sender, DataGridSortCommandEventArgs e)
        {
            SortExpression = e.SortExpression.ToString();

            Field d = (Field)GridManager.SelectedGrid.Fields.Where(f => f.Visible && f.FieldId.Equals(SortExpression)).FirstOrDefault();
            if (d != null)
            {
                if (GridManager.SelectedGrid.FieldForOrder != null && (GridManager.SelectedGrid.FieldForOrder.FieldId).Equals(d.FieldId))
                {
                    if (GridManager.SelectedGrid.OrderDirection == OrderDirectionEnum.Asc)
                    {
                        GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Desc;
                    }
                    else
                    {
                        GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Asc;
                    }
                }
                else
                {
                    if (GridManager.SelectedGrid.FieldForOrder == null && d.FieldId.Equals("D9"))
                    {
                        GridManager.SelectedGrid.FieldForOrder = d;
                        if (GridManager.SelectedGrid.OrderDirection == OrderDirectionEnum.Asc)
                        {
                            GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Desc;
                        }
                        else
                        {
                            GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Asc;
                        }
                    }
                    else
                    {
                        GridManager.SelectedGrid.FieldForOrder = d;
                        GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Asc;
                    }
                }
                GridManager.SelectedGrid.GridId = string.Empty;

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refresh_sort", "top.principale.document.iFrame_dx.location='tabPulsantiDoc.aspx';", true);

                //Da fare veloce e stampa registro

                this.mdlPopupWait.Show();

            }
        }

        protected bool GetGridPersonalization()
        {
            return this.showGridPersonalization;
        }

        protected int GetPageSize()
        {
            return this.pageSize;
        }

        protected void SetTema()
        {
            string Tema = string.Empty;
            string idAmm = string.Empty;
            if ((string)Session["AMMDATASET"] != null)
                idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            else
            {
                if (UserManager.getInfoUtente() != null)
                    idAmm = UserManager.getInfoUtente().idAmministrazione;
            }

            UserManager userM = new UserManager();
            Tema = userM.getCssAmministrazione(idAmm);
            if (!string.IsNullOrEmpty(Tema))
            {
                string[] colorsplit = Tema.Split('^');
                System.Drawing.ColorConverter colConvert = new ColorConverter();
                this.dgResult.HeaderStyle.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#" + colorsplit[2]);
                this.lblArea.ForeColor = (System.Drawing.Color)colConvert.ConvertFromString("#" + colorsplit[2]);
                this.lblTitle.ForeColor = (System.Drawing.Color)colConvert.ConvertFromString("#" + colorsplit[2]);
            }
            else
            {
                System.Drawing.ColorConverter colConvert = new ColorConverter();
                this.dgResult.HeaderStyle.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#810d06");
                this.lblArea.ForeColor = (System.Drawing.Color)colConvert.ConvertFromString("#810d06");
                this.lblTitle.ForeColor = (System.Drawing.Color)colConvert.ConvertFromString("#810d06");
            }
        }

        protected void InitializePageSize()
        {
            string valoreChiave;
            valoreChiave = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente(this).idAmministrazione, "FE_PAGING_ROW_DOCINFASC");
            int tempValue = 0;
            if (!string.IsNullOrEmpty(valoreChiave))
            {
                tempValue = Convert.ToInt32(valoreChiave);
                if (tempValue >= 20 || tempValue <= 50)
                {
                    this.pageSize = tempValue;
                }
            }
        }

        #endregion

        #region Proprietà di pagina

        /// <summary>
        /// Etichette di protocollo
        /// </summary>
        private EtichettaInfo[] Labels
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["Labels"] as EtichettaInfo[];
            }

            set
            {
                value[0].Codice = "A";
                value[1].Codice = "P";
                value[2].Codice = "I";
                value[3].Codice = "G";
                value[4].Codice = "ALL";

                CallContextStack.CurrentContext.ContextState["Labels"] = value;

            }
        }

        /// <summary>
        /// Folder in cui ricercare
        /// </summary>
        private Folder Folder
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["Folder"] as Folder;
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["Folder"] = value;
            }
        }

        /// <summary>
        /// Lista dei filtri di ricerca
        /// </summary>
        private FiltroRicerca[][] SearchFilters
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["SearchFilters"] as FiltroRicerca[][];
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["SearchFilters"] = value;
            }
        }

        /// <summary>
        /// Lista dei filtri di ricerca
        /// </summary>
        private FiltroRicerca[][] OrderFilters
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["OrderFilters"] as FiltroRicerca[][];
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["OrderFilters"] = value;
            }
        }

        /// <summary>
        /// Numero di pagina 
        /// </summary>
        private int SelectedPage
        {
            get
            {
                // Valore da restituire
                int toReturn = 1;

                if (CallContextStack.CurrentContext.ContextState.ContainsKey("SelectedPage"))
                    Int32.TryParse(
                        CallContextStack.CurrentContext.ContextState["SelectedPage"].ToString(),
                        out toReturn);

                return toReturn;
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["SelectedPage"] = value;

            }

        }

        /// <summary>
        /// Numero di record restituiti dalla ricerca
        /// </summary>
        public int RecordCount
        {
            get
            {
                // Valore da restituire
                int toReturn = 0;

                if (CallContextStack.CurrentContext.ContextState.ContainsKey("RecordCount"))
                    Int32.TryParse(
                        CallContextStack.CurrentContext.ContextState["RecordCount"].ToString(),
                        out toReturn);

                return toReturn;
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["RecordCount"] = value;
            }
        }

        /// <summary>
        /// Numero di pagine restituite dalla ricerca
        /// </summary>
        public int PageCount
        {
            get
            {
                // Valore da restituire
                int toReturn = 0;

                if (CallContextStack.CurrentContext.ContextState.ContainsKey("PageCount"))
                    Int32.TryParse(
                        CallContextStack.CurrentContext.ContextState["PageCount"].ToString(),
                        out toReturn);

                return toReturn;
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["PageCount"] = value;
            }
        }

        /// <summary>
        /// Risultati restituiti dalla ricerca.
        /// </summary>
        public SearchObject[] Result
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["Result"] as SearchObject[];
            }
            set
            {
                CallContextStack.CurrentContext.ContextState["Result"] = value;
            }

        }

        public bool showGridPersonalization
        {
            get
            {
                bool toReturn = false;
                if (CallContextStack.CurrentContext.ContextState["showGridPersonalization"] != null)
                    Boolean.TryParse(CallContextStack.CurrentContext.ContextState["showGridPersonalization"].ToString(), out toReturn);

                return toReturn;
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["showGridPersonalization"] = value;
            }
        }

        /// <summary>
        /// Posizione celle per ordinamento
        /// </summary>
        public Dictionary<string, int> CellPosition
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["cellPosition"] as Dictionary<string, int>;
            }
            set
            {
                CallContextStack.CurrentContext.ContextState["cellPosition"] = value;
            }

        }

        /// <summary>
        /// Numero di risultati per pagina
        /// </summary>
        public int pageSize
        {
            get
            {
                int toReturn = 20;
                if (CallContextStack.CurrentContext.ContextState["PageSizeDocumentInFolder"] != null)
                    toReturn = Convert.ToInt32(CallContextStack.CurrentContext.ContextState["PageSizeDocumentInFolder"].ToString());

                return toReturn;
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["PageSizeDocumentInFolder"] = value;
            }
        }

        #endregion
  
    }
}
