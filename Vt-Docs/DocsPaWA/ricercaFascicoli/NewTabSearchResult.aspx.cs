using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.SiteNavigation;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;
using System.Data;
using System.Text;
using DocsPAWA.UserControls;
using DocsPAWA.UserControls.ScrollElementsList;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace DocsPAWA.ricercaFascicoli
{
    public partial class NewTabSearchResult : CssPage
    {
        protected string paginaChiamante;
        protected static DocsPAWA.DocsPaWR.DocsPaWebService wws = ProxyManager.getWS();
        protected string SortExpression;

        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

         /// <summary>
        /// Con il page load viene eseguita la ricerca se non si è in
        /// postback altrimenti vengono visualizzati risultati
        /// </summary>
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
                setPaginaChiamante();
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
                //    this.btnRestorePreferredGrid.Visible = this.showGridPersonalization;
            }
            else
            {
                if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Project)
                    GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Project);
                // altrimenti vengono visualizzazi risultati memorizzati se ce ne sono
                if (this.Result != null && this.Result.Length > 0)
                {
                    // Visualizzazione dei risultati
                    this.ShowResult(GridManager.SelectedGrid, this.Result, this.RecordCount, this.SelectedPage);
                }
                else
                {
                    this.ShowGrid(GridManager.SelectedGrid);
                }
            }

            if (this.showGridPersonalization)
            {
                EnableDisableSave();
                //   EnableDisableRestore();
            }
        }

        /// <summary>
        /// Al cambio di pagina, vengono caricati e visualizzati i fascicoli per la pagina selezionata
        /// </summary>
        protected void dgResult_SelectedPageIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {
            // Aggiornamento del numero di pagina memorizzato nel call context
            this.dgResult.CurrentPageIndex = e.NewPageIndex;
            this.SelectedPage = e.NewPageIndex + 1;

            // Ricerca dei fascicoli e visualizzazione dei risultati
            this.SearchProjectsAndDisplayResult(
                this.SearchFilters, 
                this.SelectedPage, 
                GridManager.SelectedGrid, 
                this.Classification, 
                this.Registry, 
                this.AllClassification);

            // Aggiornamento dello stato di selezione degli item della griglia secondo quanto
            // memorizzato dalla bottoniera
            this.mobButtons.UpdateItemCheckingStatus();

        }

          protected void Page_PreRender(object sender, EventArgs e)
        {
            if (this.ResetCheckbox)
            {
                this.mobButtons.DeselectAll();
                this.ResetCheckbox = false;
            }
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
                }
            }

        }

        protected void OnTerminateMassiveOperation(object sender, EventArgs e)
        {
            this.SearchProjectsAndDisplayResult(this.SearchFilters, 
                this.SelectedPage, 
                GridManager.SelectedGrid, 
                this.Classification, 
                this.Registry, 
                this.AllClassification);
            this.mobButtons.UpdateItemCheckingStatus();
            this.mdlPopupWait.Hide();

            this.ResetCheckbox = true;

            //Risolve il problema della ricerca adl quando si rimuovono gli elementi dall'area di lavoro massivamente
            if (!String.IsNullOrEmpty(Request.QueryString["ricADL"]) && Request.QueryString["ricADL"] == "1")
            {
                SearchObject[] resultTemp = null;
                InfoUtente userInfo;
                userInfo = UserManager.getInfoUtente(this);
                if (this.SearchFilters != null)
                {
                    this.SearchFilters = this.GetFilters();
                }
                int pageNumbers = 0;
                int recordNumberTemp = 0;
                SearchResultInfo[] idProjects = null;
                this.Registry = UserManager.getRegistroSelezionato(this);

                // Recupero dei campi della griglia impostati come visibili
                Field[] visibleArray = null;
                List<Field> visibleFieldsTemplate;

                visibleFieldsTemplate = GridManager.SelectedGrid.Fields.Where(t => t.Visible && e.GetType().Equals(typeof(Field)) && t.CustomObjectId != 0).ToList();

                if (visibleFieldsTemplate != null && visibleFieldsTemplate.Count > 0)
                {
                    visibleArray = visibleFieldsTemplate.ToArray();
                }

                resultTemp = FascicoliManager.getListaFascicoliPagingCustom(this, this.Classification, this.Registry, this.SearchFilters[0], this.AllClassification, this.SelectedPage, out pageNumbers, out recordNumberTemp, this.dgResult.PageSize, !IsPostBack, out idProjects, null, this.showGridPersonalization, false, visibleArray, null,true);

                if (recordNumberTemp < this.oldResult)
                {
                    string varQuery = string.Empty;
                    if (!String.IsNullOrEmpty(Request["idClass"]))
                    {
                        varQuery = varQuery + "&idClass=" + Request["idClass"];
                    }
                    if (!String.IsNullOrEmpty(Request["tabRes"]))
                    {
                        varQuery = varQuery + "&tabRes=" + Request["tabRes"];
                    }
                    ClientScript.RegisterStartupScript(this.GetType(), "refresh_adl", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1" + varQuery + "';", true);
                }
            }
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
            string idFasc = string.Empty;
            // Se l'esito dell'operazione è positivo, viene aggiornato lo stato di inserimento
            // nell'area di lavoro del fascicolo 
            if (args.Result)
            {
                SearchObject fasc = this.Result.Where(d => d.SearchObjectID == args.ObjectId).FirstOrDefault();

                if (fasc != null)
                {
                    //Recupero id Fascicolo per eliminazione dalla lista selezionati in Massive Operation
                    idFasc = fasc.SearchObjectID;
                    string inAdl = fasc.SearchObjectField.Where(t => t.SearchObjectFieldID.Equals("IN_ADL")).FirstOrDefault().SearchObjectFieldValue;
                    if (inAdl == "1")
                    {
                        fasc.SearchObjectField.Where(t => t.SearchObjectFieldID.Equals("IN_ADL")).FirstOrDefault().SearchObjectFieldValue = "0";
                    }
                    else
                    {
                        fasc.SearchObjectField.Where(t => t.SearchObjectFieldID.Equals("IN_ADL")).FirstOrDefault().SearchObjectFieldValue = "1";
                    }
                }
            }

            // Se si è in Area di Lavoro, viene rieseguita la ricerca
            if (this.SearchInADL)
            {
                this.dgResult.DataSource = null;
                this.dgResult.DataBind();
                this.SearchProjectsAndDisplayResult(
                    this.SearchFilters,
                    this.SelectedPage,
                    GridManager.SelectedGrid,
                    this.Classification,
                    this.Registry,
                    this.AllClassification);
                //Andrea De Marco - Elimino facsicolo da lista selezionati in Massive Operation
                if (!string.IsNullOrEmpty(idFasc))
                {
                    this.mobButtons.SetState(idFasc, false);
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
            //  if (args.Result)
            // {
            SearchObject doc = this.Result.Where(d => d.SearchObjectID == args.ObjectId).FirstOrDefault();

            if (doc != null)
            {
                string inConservazione = doc.SearchObjectField.Where(t => t.SearchObjectFieldID.Equals("P13")).FirstOrDefault().SearchObjectFieldValue;
                if (inConservazione == "1")
                {
                    doc.SearchObjectField.Where(t => t.SearchObjectFieldID.Equals("P13")).FirstOrDefault().SearchObjectFieldValue = "0";
                }
                else
                {
                    doc.SearchObjectField.Where(t => t.SearchObjectFieldID.Equals("P13")).FirstOrDefault().SearchObjectFieldValue = "1";
                }
                //      }
            }
        }


        #region Funzioni di utilità

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

            // Se la query string contiene ricAdl, significa che ci si trova in ricerca ADL
            if (!String.IsNullOrEmpty(Request.QueryString["ricADL"]) &&
            Request["ricADL"] == "1")
                this.SearchInADL = true;
            else
                this.SearchInADL = false;

            // Se il callcontext è valorizzato e si è tornati su questa pagina tramite
            // pressione del tasto Back non vengono ricaricati i filtri, del registro, 
            // della classificazione e la griglia ma ci si limita ad impostare a false l'is back
            if (CallContextStack.CurrentContext != null &&
                CallContextStack.CurrentContext.IsBack)
            {
                // Azzeramento dell'Is back
                CallContextStack.CurrentContext.IsBack = false;
                // Caricamento della griglia se non ce n'è una già selezionata
                if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Project)
                    GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Project);
            }
            else
            {
                // Inizializzazione della bottoniera
                this.mobButtons.InitializeOrUpdateUserControl(new SearchResultInfo[] { });

                // Prelevamento dei filtri di ricerca
                this.SearchFilters = this.GetFilters();

                // Impostazione dell'oggetto con le informazioni sulla classificazione
                int classificationId = 0;
                Int32.TryParse(Request.QueryString["idClass"], out classificationId);
                this.Classification = this.GetClassification(classificationId);

                // Impostazione del registro
                this.Registry = UserManager.getRegistroSelezionato(this);

                // Impostazione del flag per indicare se bisogna ricercare fra i figli del fascicolo
                this.AllClassification = FascicoliManager.getAllClassValue(this);

                // Reset del count fascicoli, indice dell'elemento selezionato e numero di pagina
                this.RecordCount = 0;
                this.SelectedPage = 1;

                // Caricamento della griglia
                if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Project)
                    GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Project);

            }

            // Se la lista filtri è valorizzata, si procede con la ricerca e la visualizzazione
            // dei risultati. Viene inoltre gestito il back
            if (this.SearchFilters != null || (this.Request.QueryString["newFasc"] != null && this.Request.QueryString["newFasc"].Equals("1")))
            {
                string noResult = string.Empty;
                if (!String.IsNullOrEmpty(Request.QueryString["noRic"]) &&
                  Request.QueryString["noRic"] == "1")
                {
                    noResult = Request.QueryString["noRic"].ToString();
                }

                if (this.SearchFilters != null && string.IsNullOrEmpty(noResult))
                {
                    this.SearchProjectsAndDisplayResult(this.SearchFilters, this.SelectedPage, GridManager.SelectedGrid, this.Classification, this.Registry, this.AllClassification);
                }
                else
                {
                    mobButtons.NUM_RESULT = "0";
                    this.ShowGrid(GridManager.SelectedGrid);
                }
            }
            else
            {
                mobButtons.NUM_RESULT = "0";
                this.ShowGrid(GridManager.SelectedGrid);
            }
        }

        /// <summary>
        /// Funzione per il recupero dei filtri da utilizzare per la ricerca
        /// </summary>
        /// <returns>Filtri da utilizzare per la ricerca</returns>
        private FiltroRicerca[][] GetFilters()
        {
            // Valore da restituire
            FiltroRicerca[][] toReturn = null;

            if (FascicoliManager.getFiltroRicFasc(this) != null)
                toReturn = FascicoliManager.getFiltroRicFasc(this);
            else
                if (FascicoliManager.getMemoriaFiltriRicFasc(this) != null)
                    toReturn = FascicoliManager.getMemoriaFiltriRicFasc(this);

            return toReturn;
        }

        /// <summary>
        /// Funzione per il recupero dell'oggetto classificazione da utilizzare per la ricerca
        /// </summary>
        /// <param name="classificationId">Id classificazione. Viene passato nel querystring</param>
        /// <returns>Oggetto classificazione da utilizzare per la ricerca dei fascicoli</returns>
        private FascicolazioneClassificazione GetClassification(int classificationId)
        {
            // Valore da restituire
            FascicolazioneClassificazione toReturn = null;

            // Se classificationId è diverso da 0, viene prelevato il valore salvato in sessione
            if(classificationId != 0)
                toReturn = FascicoliManager.getClassificazioneSelezionata(this);

            // Se in sessione c'è classificaSelezionata, l'oggetto di classificazione
            // viene caricato da WS
            if(toReturn == null && Session["classificaSelezionata"] != null)
                try
                {
                    toReturn = FascicoliManager.fascicolazioneGetTitolario2(
                        this,
                        ((FascicolazioneClassifica)Session["classificaSelezionata"]).codice,
                        false,
                        ((FascicolazioneClassifica)Session["classificaSelezionata"]).idTitolario)[0];
                }
                catch (Exception e) { }

            //if (FascicoliManager.getMemoriaClassificaRicFasc(this) != null)
            //    toReturn = FascicoliManager.getMemoriaClassificaRicFasc(this);

            return toReturn;
        }

        /// <summary>
        /// Questa funzione si occupa di effettuare la ricerca fascicoli e di visualizzare i fascicoli
        /// </summary>
        /// <param name="searchFilters">Filetri di ricerca</param>
        /// <param name="selectedPage">Pagina da visualizzare</param>
        /// <param name="selectedGrid">Griglia da utilizzare per la visualizzazione dei dati</param>
        /// <param name="classification">Oggetto classificazione da utilizzare per la ricerca</param>
        /// <param name="registry">Registro selezionato</param>
        /// <param name="allClassification">True se bisogna ricercare anche nei sottofascicoli</param>
        private void SearchProjectsAndDisplayResult(FiltroRicerca[][] searchFilters, int selectedPage, Grid selectedGrid, FascicolazioneClassificazione classification, Registro registry, bool allClassification)
        {
            // Lista dei fascicoli risultato della ricerca
            SearchObject[] result = null;

            // Numero di fascicoli restituiti dalla ricerca
            int recordNumber = 0;

			/* ABBATANGELI GIANLUIGI
             * il nuovo parametro outOfMaxRowSearchable è true se raggiunto il numero 
             * massimo di riche accettate in risposta ad una ricerca */
            bool outOfMaxRowSearchable;
            // Ricerca dei fascicoli
            result = this.SearchProject(searchFilters, classification, registry, allClassification, selectedPage, out recordNumber, out outOfMaxRowSearchable);
            
			if (outOfMaxRowSearchable && recordNumber > 1)
            {
                utils.AlertPostLoad.OutOfMaxRowSearchable(Page, recordNumber);
                recordNumber = 0;
            }
			else
            {
                outOfMaxRowSearchable = false;
                this.RecordCount = recordNumber;

                if (!IsPostBack)
                {
                    this.oldResult = recordNumber;
                }

                if (this.Request.QueryString["newFasc"] != null && this.Request.QueryString["newFasc"].Equals("1"))
                {
					//da vedere domani
				  /*  result = new DocsPAWA.DocsPaWR.SearchObject[1];
					result[0] = FascicoliManager.getFascicoloSelezionato(this);
					//non mi fido del metodo precedente, secondo me non prende tutti i dati
					result[0] = FascicoliManager.getFascicoloById(this.Page, result[0].systemID);
					this.RecordCount = 1;
					recordNumber = 1;
					SearchResultInfo[] idProjects = new SearchResultInfo[1];
					SearchResultInfo tempFascNew = new SearchResultInfo();
					tempFascNew.Id = result[0].systemID;
					tempFascNew.Codice = result[0].codice;
					idProjects[0] = tempFascNew;
				   
					this.mobButtons.InitializeOrUpdateUserControl(idProjects);
				   * */

					result = new DocsPAWA.DocsPaWR.SearchObject[1];
					Fascicolo tempFasc = FascicoliManager.getFascicoloSelezionato(this);
					result[0] = FascicoliManager.GetObjectFascicoloBySystemId(tempFasc.systemID, UserManager.getInfoUtente(this));

					SearchResultInfo[] idProjects = new SearchResultInfo[1];
					SearchResultInfo tempFascNew = new SearchResultInfo();
					tempFascNew.Id = result[0].SearchObjectID;
					tempFascNew.Codice = result[0].SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P3")).FirstOrDefault().SearchObjectFieldValue;
					idProjects[0] = tempFascNew;

					this.mobButtons.InitializeOrUpdateUserControl(idProjects);

				}
            }

            // Se ci sono risultati, vengono visualizzati
            if (result != null)
                // Visualizzazione dei risultati
                this.ShowResult(selectedGrid, result, recordNumber, selectedPage);

            if ((result != null && result.Length == 0) || outOfMaxRowSearchable)
            {
                this.ShowGrid(GridManager.SelectedGrid);
            }

            mobButtons.NUM_RESULT = recordNumber.ToString();
        }

        /// <summary>
        /// Funzione per la ricerca dei fascicoli
        /// </summary>
        /// <param name="searchFilters">Filtri di ricerca</param>
        /// <param name="classification">Oggetto classificazione da utilizzare per la ricerca dei fascicoli</param>
        /// <param name="registry">Registro selezionato</param>
        /// <param name="allClassification">True se bisogna ricercare anche nei sottofascicoli</param>
        /// <param name="selectedPage">Pagina da visualizzare</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        /// <returns>Lista delle informazioni sui fascicoli restituiti dalla ricerca</returns>
        private SearchObject[] SearchProject(FiltroRicerca[][] searchFilters, FascicolazioneClassificazione classification, Registro registry, bool allClassification, int selectedPage, out int recordNumber, out bool outOfMaxRowSearchable)
        {
            // Fascicoli individuati dalla ricerca
            SearchObject[] toReturn;

            // Informazioni sull'utente
            InfoUtente userInfo;

            // Numero totale di pagine
            int pageNumbers;

            // Lista dei system id dei fascicoli restituiti dalla ricerca
            SearchResultInfo[] idProjects;

            // Prelevamento delle informazioni sull'utente
            userInfo = UserManager.getInfoUtente(this);

            // Recupero dei campi della griglia impostati come visibili
            Field[] visibleArray = null;
            List<Field> visibleFieldsTemplate;

            if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Project)
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Project);

            visibleFieldsTemplate = GridManager.SelectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field)) && e.CustomObjectId != 0).ToList();

            if (visibleFieldsTemplate != null && visibleFieldsTemplate.Count > 0)
            {
                visibleArray = visibleFieldsTemplate.ToArray();
            }

            // Caricamento dei fascicoli
            toReturn = FascicoliManager.getListaFascicoliPagingCustom(this, this.Classification, this.Registry, this.SearchFilters[0], this.AllClassification, this.SelectedPage, out pageNumbers, out recordNumber, this.pageSize, !IsPostBack, out idProjects, null, this.showGridPersonalization, false, visibleArray, null,true);
			/* ABBATANGELI GIANLUIGI
             * outOfMaxRowSearchable viene impostato a true se getQueryInfoDocumentoPagingCustom
             * restituisce pageNumbers = -2 (raggiunto il numero massimo di righe possibili come risultato di ricerca)*/
            outOfMaxRowSearchable = (pageNumbers == -2);
            if (this.Request.QueryString["newFasc"] != null && this.Request.QueryString["newFasc"].Equals("1"))
            {
                recordNumber = 1;
            }
            // Memorizzazione del numero di risultati restituiti dalla ricerca, del numero di pagina e dei risultati
            this.RecordCount = recordNumber;
            this.PageCount = pageNumbers;
            this.Result = toReturn;

            //da ricerche salvate se trova zero elementi l'oggetto REsult è null, invece 
            //da ricerca nomrale anche se zero risultati torna un elemento vuoto ma non null
            if (this.Result == null)
            {
                this.Result = new SearchObject[] { new SearchObject() };
            }

            // Memorizzazione dei dati sullo scroller dei fascicoli
            ScrollManager.setInContextNewObjScrollElementsList(
                this.RecordCount,
                this.PageCount,
                this.pageSize,
                0,
                this.SelectedPage - 1,
                new ArrayList(this.Result),
                ObjScrollElementsList.EmunSearchContext.RICERCA_FASCICOLI);
           
            // Se la lista dei fascicoli è popolata, viene inizializzata la bottoniera
            if (idProjects != null)
                this.mobButtons.InitializeOrUpdateUserControl(idProjects);

            // Restituzione della lista di fascicoli da visualizzare
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

            // Larghezza da assegnare alla griglia
            int gridWidth = 0;

            // Posizione assunta dal campo all'interno della griglia
            int position = 0;

            // Reperimento dei campi da visualizzare ordinati per Posizione crescente
            gridFields = selectedGrid.Fields.Where(e => e.Visible).OrderBy(f => f.Position).ToList();

            //Ripristino contatore senza griglie custom
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
                    d.OracleDbColumnName = "to_number(getContatoreFascContatore (a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "'))";
                    d.SqlServerDbColumnName = "@dbUser@.getContatoreFascContatore(a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "')";
                    gridFields.Insert(2, d);
                }
                else
                {
                    gridFields.Remove(d);
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
                        "IdProject",
                        this.OnWorkingAreaOperationCompleted,
                        this.OnStorageAreaOperationCompleted,
                        null,
                        null,
                        this,
                        NewSearchIcons.ObjectTypeEnum.Project,
                        NewSearchIcons.ParentPageEnum.SearchProject,
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
                        if (field.OriginalLabel.ToUpper().Equals("CODICE"))
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

                // ...aggiornamento posizione
                position += 1;

            }

            // Impostazione della larghezza della griglia
            //this.dgResult.Width = new Unit(gridWidth, UnitType.Pixel);

        }

        /// <summary>
        /// Funzione per la visualizzazione dei risutati della ricerca
        /// </summary>
        /// <param name="selectedGrid">Griglia da utilizzare per la visualizzazione dei risultati</param>
        /// <param name="result">I risultati della ricerca</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        /// <param name="selectedPage">Pagina visualizzata</param>
        private void ShowResult(Grid selectedGrid, SearchObject[] result, int recordNumber, int selectedPage)
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
            
            // Aggiunta dei campi IdProject, IsInStorageArea,
            // IsInWorkingArea, NavigateUrl.
            dataSet.Tables[0].Columns.Add("IdProject", typeof(String));
            dataSet.Tables[0].Columns.Add("IsInStorageArea", typeof(Boolean));
            dataSet.Tables[0].Columns.Add("IsInWorkingArea", typeof(Boolean));
            dataSet.Tables[0].Columns.Add("NavigateUrl", typeof(String));

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

            // Compilazione del dataset
            this.FillDataSet(dataSet.Tables[0], result, selectedGrid);

            // Associazione del data set alla griglia e databind
            this.dgResult.DataSource = dataSet;
            this.dgResult.DataBind();

            mobButtons.NUM_RESULT = recordNumber.ToString();

            // Impostazione del titolo nella pagina e del numero di documenti trovati
            this.lblTitle.Text = String.Format(" Elenco fascicoli - Trovati {0} fascicoli.", recordNumber);

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

            if (!String.IsNullOrEmpty(Request.QueryString["ricADL"]) &&
               Request.QueryString["ricADL"] == "1")
            {
                this.lblArea.Text = "AREA DI LAVORO" + gridType;
            }
            else
            {
               this.lblArea.Text = "Ricerca fascicoli" + gridType;
            }

            this.SelectItem();

        }

        /// <summary>
        /// Funzione per la compilazione del dataset da associare al datagrid
        /// </summary>
        /// <param name="dataTable">Data table da compilare</param>
        /// <param name="result">Risultati della ricerca da visualizzare</param>
        /// <param name="selectedGrid">Griglia da visualizzare</param>
        private void FillDataSet(DataTable dataTable, SearchObject[] result, Grid selectedGrid)
        {
            // Lista dei campi della griglia che risultano visibili
            List<Field> visibleFields;

            // La riga da aggiungere al dataset
            DataRow dataRow;

            // Valore da assegnare ad un campo
            String value = null;

            // Il template di ricerca selezionato
            Templates template;

            // L'oggetto custom contatore
            OggettoCustom customObject = null;

            StringBuilder temp;

            // Numero di caratteri a cui troncare il testo
            int maxLength = 0;

            // Recupero dei campi della griglia impostati come visibili
            visibleFields = selectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field))).ToList();

            // Prelevamento del template di ricerca eventualmente selezionato
            template = Session["templateRicerca"] as Templates;

            OggettoCustom customObjectTemp = new OggettoCustom();
            //Ripristino contatore senza griglie custom
            Templates templateTemp = Session["templateRicerca"] as Templates;

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
            
            // Per ogni risultato...
            foreach (SearchObject prj in result)
            {
                // ...viene inizializzata una nuova riga
                dataRow = dataTable.NewRow();

                // ...impostazione dell'id project
                dataRow["IdProject"] = prj.SearchObjectID;

                foreach (Field field in visibleFields)
                {
                    switch (field.FieldId)
                    {
                            //APERTURA
                        case "P5":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                            //CARTACEO
                        case "P11":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                            {
                                value = "Si";
                            }
                            else
                            {
                                value = "No";
                            }
                            break;
                            //CHIUSURA
                        case "P6":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                            //CODICE
                        case "P3":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            temp = new StringBuilder("<span style=\"color:Black;\">");
                            // viene colorato in nero il link
                            temp.Append(value);
                            temp.Append("</span>");

                            value = temp.ToString();
                            break;
                            //CODICE CLASSIFICA
                        case "P2":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                            //AOO
                        case "P7":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                            //DESCRIZIONE
                        case "P4":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                            // IN ARCHIVIO
                        case "P12":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                            {
                                value = "Si";
                            }
                            else
                            {
                                value = "No";
                            }
                            break;
                            //IN CONSERVAZIONE
                        case "P13":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                            {
                                value = "Si";
                            }
                            else
                            {
                                value = "No";
                            }
                            break;
                            //NOTE
                        case "P8":
                              string valoreChiave;
                             valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_IS_PRESENT_NOTE");

                             if (!string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("1"))
                             {
                                 value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ESISTE_NOTA")).FirstOrDefault().SearchObjectFieldValue;
                             }
                             else
                             {
                                 value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                             }
                            break;
                            // NUMERO
                        case "P14":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                            //NUMERO MESI IN CONSERVAZIONE
                        case "P15":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                            // PRIVATO
                        case "P9":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                            {
                                value = "Si";
                            }
                            else
                            {
                                value = "No";
                            }
                            break;
                            // STATO
                        case "P16":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                            // TIPO
                        case "P1":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                            // TIPOLOGIA
                        case "U1":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                            //TITOLARIO
                        case "P10":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //Nome e cognome autore
                        case "P17":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                            //desc ruolo autore
                        case "P18":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //desc uo autore
                        case "P19":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                            //Data creazione
                        case "P20":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //Collocazione fisica
                        case "P22":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                            //CONTATORE
                        case "CONTATORE":
                            try
                            {
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            }
                            catch (Exception e)
                            {
                                value = string.Empty;
                            }
                            break;
                        case "P23":
                            // Atipicità
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        case "COD_EXT_APP":
                            // CODICE ATTIVITA ESTERNA
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            break;
                        //OGGETTI CUSTOM
                        default:
                            try
                            {
                                if (!string.IsNullOrEmpty(prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue))
                                {
                                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(field.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                                }
                                else
                                {
                                    value = "";
                                }
                            }
                            catch (Exception e)
                            {
                                value = "";
                            }
                            break;
                    }

                    // Valorizzazione del campo fieldName
                    dataRow[field.FieldId] = value;

                }

                string inConservazione = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P13")).FirstOrDefault().SearchObjectFieldValue;

                string inAdl = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("IN_ADL")).FirstOrDefault().SearchObjectFieldValue;

                // Impostazione dei campi IsInStorageArea, IsInWorkingArea e NavigateUrl
                dataRow["IsInStorageArea"] = !String.IsNullOrEmpty(inConservazione) && inConservazione != "0" ? true : false;
                dataRow["IsInWorkingArea"] = !String.IsNullOrEmpty(inAdl) && inAdl != "0" ? true : false;
                dataRow["NavigateUrl"] = this.GetUrlForProjectDetails(UserManager.getInfoUtente(), prj.SearchObjectID);

                // ...aggiunta della riga alla collezione delle righe
                dataTable.Rows.Add(dataRow);

            }

        }

        /// <summary>
        /// Funzione per la verifica dei diritti di accesso al fascicolo da parte dell'utente
        /// e per la restituzione dell'eventuale link per la sua visualizzazione
        /// </summary>
        /// <param name="infoUtente">Informazioni sull'utente</param>
        /// <param name="idProject">Id del fascicolo</param>
        /// <returns>L'eventuale link per la visualizzazione del dettaglio del fascicolo</returns>
        private String GetUrlForProjectDetails(InfoUtente infoUtente, string idProject)
        {
            // Il link da restituire
            String toReturn = String.Empty;

            // I diritti dell'utente sul documento
            int rights;

            // Il messaggio restituito dalla funzione per la verfica dei diritti
            String message;

            // Recupero dei diritti sul documento
            rights = DocumentManager.verificaACL("F", idProject, infoUtente, out message);

            // Se pari a 2, ha i diritti
            if (rights == 2)
            {
                // Viene rimosso l'eventuale fascicolo selezionato per la ricerca per evitare che
                // questo venga visualizzato come fascicolo nella maschera di fascicolazione rapida
                FascicoliManager.removeFascicoloSelezionatoFascRapida();

                // L'indirizzo da associare è ../fascicolo/GestioneFasc.aspx?from=newRicFasc&idProject=<idProject>
                toReturn = String.Format("../fascicolo/GestioneFasc.aspx?from=newRicFasc&idProject={0}",
                    idProject);
            }

            // Restituzione del valore calcolato
            return toReturn;
        }

        /// <summary>
        /// Funzione per la selezione dell'item con le informazioni sull'ultimo elemento
        /// di cui si è visto il dettaglio prima di abbandonare la pagina
        /// </summary>
        private void SelectItem()
        {
            // Reperimento del fascicolo selezionato, se esiste
            String selectedId = SearchUtils.GetObjectId();
            // Reperimento del documento selezionato, se esiste
            SearchObject prj = this.Result.Where(d => d.SearchObjectID == selectedId).FirstOrDefault();

            // Se il fascicolo è stato reperito con successo, viene selezionata la riga
            // corrispondente nella griglia
            if (prj != null)
            {
                int index = Array.IndexOf(this.Result, prj);

                this.dgResult.SelectedIndex = index;
            }
            else
                this.dgResult.SelectedIndex = -1;
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
                    if (GridManager.SelectedGrid.FieldForOrder == null && d.FieldId.Equals("P20"))
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
                string adl = string.Empty;
                if (SearchInADL)
                {
                    adl = "&ricADL=1";
                }

                string tabPagina = this.mobButtons.PAGINA_CHIAMANTE;

                if (!string.IsNullOrEmpty(tabPagina))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refresh_sort", "top.principale.document.iFrame_sx.location='gestioneRicFasc.aspx?gridper=" + GridManager.SelectedGrid.GridType.ToString() + "&tab=" + tabPagina + adl + "&numRes=" + this.mobButtons.NUM_RESULT + "';", true);
                }

                //Da fare veloce e stampa registro

                this.mdlPopupWait.Show();

            }
        }

        protected bool GetGridPersonalization()
        {
            return this.showGridPersonalization;
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
            }
            else
            {
                System.Drawing.ColorConverter colConvert = new ColorConverter();
                this.dgResult.HeaderStyle.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#810d06");
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

       
        protected int GetPageSize()
        {
            return this.pageSize;
        }


         protected void InitializePageSize()
        {
            string valoreChiave;
            valoreChiave = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente(this).idAmministrazione, "FE_PAGING_ROW_PROJECT");
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

        /// <summary>
        /// Funzione per la visualizzazione dei risutati della ricerca
        /// </summary>
        /// <param name="result">I risultati della ricerca</param>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        private void ShowGrid(Grid selectedGrid)
        {
            // Il dataset da associare al datagrid
            DataSet dataSet;

            // Inizializzazione della griglia
            this.InitializeGrid(selectedGrid);

            // Creazione del dataset da associare al datagrid
            dataSet = GridManager.InitializeDataSet(selectedGrid);

            // Aggiunta dei campi IdProfile, NavigateUrl, FileExtension, IsInStorageArea,
            // IsInWorkingArea, IsSigned, ProtoType al dataset.
            dataSet.Tables[0].Columns.Add("IdProject", typeof(String));
            dataSet.Tables[0].Columns.Add("IsInStorageArea", typeof(Boolean));
            dataSet.Tables[0].Columns.Add("IsInWorkingArea", typeof(Boolean));
            dataSet.Tables[0].Columns.Add("NavigateUrl", typeof(String));
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
            this.lblTitle.Text = String.Format(" Elenco fascicoli - Trovati {0} fascicoli.", "0");

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

            if (!String.IsNullOrEmpty(Request.QueryString["ricADL"]) &&
               Request.QueryString["ricADL"] == "1")
            {
                this.lblArea.Text = "AREA DI LAVORO" + gridType;
            }
            else
            {
                this.lblArea.Text = "Ricerca fascicoli" + gridType;
            }

        }

        #endregion

        #region Proprietà di pagina

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
        /// Classificazione da utilizzare per la ricerca fascicoli
        /// </summary>
        public FascicolazioneClassificazione Classification
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["Classification"] as FascicolazioneClassificazione;           
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["Classification"] = value;
            }

        }

        /// <summary>
        /// Registro selezionato
        /// </summary>
        public Registro Registry
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["Registry"] as Registro;
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["Registry"] = value;
            }
        }

        /// <summary>
        /// True  se biusogna ricercare anche fra i figli del fascicolo
        /// </summary>
        public bool AllClassification 
        {
            get
            {
                // Valore da restituire
                bool toReturn = false;

                if (CallContextStack.CurrentContext.ContextState.ContainsKey("AllClassification"))
                    Boolean.TryParse(
                        CallContextStack.CurrentContext.ContextState["AllClassification"].ToString(),
                        out toReturn);

                return toReturn;
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["AllClassification"] = value;
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

        #endregion

        public void setPaginaChiamante()
        {
            if (Request.QueryString["tabRes"] != string.Empty && Request.QueryString["tabRes"] != null)
            {
                this.paginaChiamante = Request.QueryString["tabRes"];
                mobButtons.PAGINA_CHIAMANTE = this.paginaChiamante;
            }
        }


        public string getPaginaChiamante()
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(this.paginaChiamante))
            {
                result = this.paginaChiamante;
            }

            return result;
        }

        /// <summary>
        /// True se ci si trova in ricerca fascicoli in adl
        /// </summary>
        public bool SearchInADL
        {
            get
            {
                bool toReturn = false;
                if (CallContextStack.CurrentContext.ContextState["SearchInADL"] != null)
                    Boolean.TryParse(CallContextStack.CurrentContext.ContextState["SearchInADL"].ToString(), out toReturn);

                return toReturn;
            }

            set 
            {
                CallContextStack.CurrentContext.ContextState["SearchInADL"] = value;
            }
        }

        /// <summary>
        /// Utilizzato per il refresh se eliminiamo massivamente da area di lavoro
        /// </summary>
        public int oldResult
        {
            get
            {
                int toReturn = 0;
                if (CallContextStack.CurrentContext.ContextState["oldResult"] != null)
                    int.TryParse(CallContextStack.CurrentContext.ContextState["oldResult"].ToString(), out toReturn);

                return toReturn;
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["oldResult"] = value;
            }
        }

        /// <summary>
        /// Ruolo abilitato alle grigle custum
        /// </summary>
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
                if (CallContextStack.CurrentContext.ContextState["PageSizeProject"] != null)
                    toReturn = Convert.ToInt32(CallContextStack.CurrentContext.ContextState["PageSizeProject"].ToString());

                return toReturn;
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["PageSizeProject"] = value;
            }
        }


        [DefaultValue(false)]
        public bool ResetCheckbox { get; set; }

    }
}
