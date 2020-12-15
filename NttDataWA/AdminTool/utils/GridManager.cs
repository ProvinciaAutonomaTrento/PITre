using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAAdminTool.DocsPaWR;
using  SAAdminTool.SiteNavigation;
using  SAAdminTool.UserControls;
using log4net;
using System.Web.SessionState;

namespace SAAdminTool.utils
{
    /// <summary>
    /// Questa classe fornisce funzioni di utilità per la gestione delle griglie
    /// </summary>
    public class GridManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(GridManager));
        #region Caricamento e salvataggio griglia. Gestione memorizzazione ultima griglia caricata

        /// <summary>
        /// Funzione per il reperimento di una griglia.
        /// </summary>
        /// <param name="gridId">Identificativo della griglia da caricare. Se null viene preso in considerazione il template id</param>
        /// <param name="gridType">Itentificativo della ricerca in cui è inserita la griglia</param>
        /// <param name="templateId">Identificativo del template di cui caricare i campi. Se null vengono caricati i soli campi standard</param>
        /// <returns>Le informazioni sulla griglia</returns>
        public static Grid LoadGrid(
            String gridId,
            GridTypeEnumeration gridType,
            List<String> templatesId)
        {
            // Valore da restituire
            Grid toReturn = null;

            try
            {
                toReturn = new DocsPaWR.DocsPaWebService().LoadGrid(
                    gridId,
                    gridType,
                    UserManager.getInfoUtente(),
                    UserManager.getRuolo(),
                    templatesId != null ? templatesId.ToArray() : null);

            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il reperimento delle informazioni sulla griglia di ricerca.", e);
                throw new Exception("Errore durante il reperimento delle informazioni sulla griglia.");
            }

            // Restituzione della griglia
            return toReturn;

        }

        /// <summary>
        /// Funzione per il salvataggio della griglia
        /// </summary>
        /// <param name="grid">Griglia da salvare</param>
        /// <param name="userInfo">Informazioni sull'utente proprietario della griglia</param>
        public static void SaveGrid(Grid grid, InfoUtente userInfo, String gridName, Boolean isTemporary, String isActive, String visibility)
        {
            try
            {
                DocsPaWebService wss = ProxyManager.getWS();
                if (!string.IsNullOrEmpty(isActive) && isActive.Equals("Y"))
                {
                    grid.GridId = wss.SaveGrid(
                    grid,
                    UserManager.getInfoUtente(),
                    UserManager.getRuolo(),
                    gridName,
                    isTemporary,
                    isActive,
                    visibility);
                }
                else
                {
                    wss.SaveGrid(
                    grid,
                    UserManager.getInfoUtente(),
                    UserManager.getRuolo(),
                    gridName,
                    isTemporary,
                    isActive,
                    visibility);
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il salvataggio delle informazioni sulla griglia di ricerca.", e);
                throw new Exception("Errore durante il salvatggio delle informazioni sulla griglia.");
            }

        }

        /// <summary>
        /// Funzione per il salvataggio della griglia
        /// </summary>
        /// <param name="grid">Griglia da salvare</param>
        /// <param name="userInfo">Informazioni sull'utente proprietario della griglia</param>
        public static void SaveNewGrid(Grid grid, InfoUtente userInfo, String gridName, String visibility, Boolean isPreferred)
        {
            try
            {
                DocsPaWebService wss = ProxyManager.getWS();
          //      if (isPreferred)
          //      {
                    grid.GridId = wss.SaveNewGrid(grid, userInfo, gridName, visibility, isPreferred);
         //       }
          //      else
         //       {
               //     wss.SaveNewGrid(grid, userInfo, gridName, visibility, isPreferred);
         //       }
            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il salvataggio delle informazioni sulla nuvoa griglia.", e);
                throw new Exception("Errore durante il salvataggio delle informazioni sulla nuvoa griglia.");
            }

        }

        /// <summary>
        /// Funzione per il salvataggio della griglia
        /// </summary>
        /// <param name="grid">Griglia da salvare</param>
        /// <param name="userInfo">Informazioni sull'utente proprietario della griglia</param>
        public static void ModifyGrid(Grid grid, InfoUtente userInfo, String visibility, Boolean isPreferred)
        {
            try
            {
                DocsPaWebService wss = ProxyManager.getWS();
                if (isPreferred)
                {
                    grid.GridId = wss.ModifyGrid(grid, userInfo, visibility, isPreferred);
                }
                else
                {
                    wss.ModifyGrid(grid, userInfo, visibility, isPreferred);
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il salvataggio delle informazioni sulla nuvoa griglia.", e);
                throw new Exception("Errore durante il salvataggio delle informazioni sulla nuvoa griglia.");
            }

        }

        /// <summary>
        /// Funzione per il caricamento della griglia standard
        /// </summary>
        /// <param name="gridType">Tipo di griglia da caricare</param>
        /// <returns>Griglia standard</returns>
        public static Grid GetStandardGridForUser(
            GridTypeEnumeration gridType)
        {
            try
            {
                DocsPaWebService ws = ProxyManager.getWS();
                InfoUtente userInfo = UserManager.getInfoUtente();

                return ws.GetStandardGridForUser(
                    userInfo,
                    gridType);

            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il reperimento della griglia standard.", e);
                throw new Exception("Errore durante il reperimento della griglia standard.", e);
            }
        }

        /// <summary>
        /// Funzione per il caricamento di una griglia salvata per una determinata
        /// ricerca rapida. Nel caso in cui non sia stata salvata alcuna griglia, viene
        /// restituita quella standard
        /// </summary>
        /// <param name="searchId">System id della ricerca rapida di cui caricarene la griglia associata</param>
        /// <param name="gridType">Tipo di griglia da caricare</param>
        /// <returns>Griglia relativa alla ricerca rapida segnalata</returns>
        public static Grid GetGridFromSearchId(
            string searchId,
            GridTypeEnumeration gridType)
        {
            try
            {
                DocsPaWebService ws = ProxyManager.getWS();
                InfoUtente userInfo = UserManager.getInfoUtente();

                return ws.GetGridFromSearchId(
                    userInfo,
                    searchId,
                    gridType);


            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il reperimento della griglia per la ricerca rapida.", e);
                throw new Exception("Errore durante il reperimento della griglia per la ricerca rapida.", e);
            }
        }

        /// <summary>
        /// Funzione per la verifica di presenza della funzione di personalizzazione griglia
        /// per l'installazione corrente
        /// </summary>
        /// <returns>True se è possibile abilitare la personalizzazione griglia</returns>
        public static bool ExistGridPersonalizationFunction()
        {
            try
            {
                DocsPaWebService ws = ProxyManager.getWS();

                return ws.ExistGridPersonalizationFunction();

            }
            catch (Exception e)
            {
                logger.Debug("Errore durante la verifica di presenza della funzione di personalizzazione griglia.", e);
                throw new Exception("Errore durante la verifica di presenza della funzione di personalizzazione griglia.");
            }
        }

        public static bool IsRoleEnabledToUseGrids()
        {
            try
            {
                // Reperimento della lista di funzioni associate al ruolo
                Funzione[] functions = UserManager.getRuolo().funzioni;

                return functions.Where(e => e.codice.Equals("GRID_PERSONALIZATION")).Count() > 0;
            }
            catch
            {

                return false;
            }
        }

        /// <summary>
        /// Funzione per la decodifica del tipo di griglia
        /// </summary>
        /// <param name="gridType">Tipo di ricerca in cui è inserita la griglia</param>
        /// <returns>Il tipo di griglia decodificato</returns>
        public static GridTypeEnumeration DecodeGridType(string gridType)
        {
            // Il valore da restituire
            GridTypeEnumeration toReturn = GridTypeEnumeration.NotRecognized;

            // Interpretazione del valore
            switch (gridType)
            {
                case "Document":
                    toReturn = GridTypeEnumeration.Document;
                    break;
                case "Project":
                    toReturn = GridTypeEnumeration.Project;
                    break;
                case "DocumentInProject":
                    toReturn = GridTypeEnumeration.DocumentInProject;
                    break;
                case "Transmission":
                    toReturn = GridTypeEnumeration.Transmission;
                    break;
            }

            if (toReturn == GridTypeEnumeration.NotRecognized)
                throw new Exception("Tipo griglia non riconosciuto.");

            // Restituzione del valore
            return toReturn;
        }

        /// <summary>
        /// Rimuove la griglia preferita
        /// </summary>
        /// <param name="type">Tipologia della griglia</param>
        /// <param name="userInfo">Informazioni sull'utente proprietario della griglia</param>
        public static void RemovePreferredTypeGrid(InfoUtente infoUser, GridTypeEnumeration gridType)
        {
            try
            {
                DocsPaWebService wss = ProxyManager.getWS();
                wss.RemovePreferredTypeGrid(infoUser, gridType);

            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il salvataggio delle informazioni sulla nuova griglia.", e);
                throw new Exception("Errore durante il salvataggio delle informazioni sulla nuvoa griglia.");
            }

        }

        public static void AddPreferredGrid(string gridId, InfoUtente infoUser, GridTypeEnumeration gridType)
        {
            DocsPaWebService wss = ProxyManager.getWS();
            wss.AddPreferredGrid(gridId, infoUser, gridType);
        }

        /// <summary>
        /// Griglia di ricerca da utilizzare per disegnare una griglia di ricerca
        /// </summary>
        public static Grid SelectedGrid
        {
            get
            {
                if (CallContextStack.CurrentContext==null || CallContextStack.CurrentContext.ContextState["SelectedGrid"] == null)
                {
                    return null;
                }
                else
                {
                    return CallContextStack.CurrentContext.ContextState["SelectedGrid"] as Grid;
                }
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["SelectedGrid"] = value;
            }
        }

        #endregion

        #region Funzioni per inizializzazione e rimpimento dataset

        /// <summary>
        /// Funzione per l'inizializzazione del data set in base ai campi definiti nella 
        /// griglia
        /// </summary>
        /// <param name="selectedGrid">La griglia su cui basare la creazione del dataset</param>
        /// <returns></returns>
        public static DataSet InitializeDataSet(Grid selectedGrid)
        {
            // Il dataset da restituire
            DataSet toReturn;

            // La tabella da aggiungere al dataset
            DataTable dataTable;

            // La colonna da aggiungere alla tabella
            DataColumn dataColumn;

            // Inizializzazione del dataset
            toReturn = new DataSet();
            dataTable = new DataTable();
            toReturn.Tables.Add(dataTable);

            List<Field> fields = selectedGrid.Fields.Where(e => e.GetType().Equals(typeof(Field))).ToList();

            // Creazione delle colonne
            foreach (Field field in fields)
            {
                dataColumn = new DataColumn(field.FieldId, typeof(String));
                //dataColumn.MaxLength = field.Length != "0"? Int32.Parse(field.Length) : -1 ;

                dataTable.Columns.Add(dataColumn);
            }

            // Colonna con il codice HEX da associare allo sfondo di una cella di template
            // che non appartiene ad un documento
            dataColumn = new DataColumn("BackColor", typeof(System.Drawing.Color));
            dataTable.Columns.Add(dataColumn);

            // Restituzione del dataset
            return toReturn;

        }

        /// <summary>
        /// Funzione per la ricerca del valore da assegnare al campo di profilazione dinamica specificato
        /// </summary>
        /// <param name="fieldName">Il nome del campo custom da valorizzare</param>
        /// <param name="template">Il template associato da cui prelevare il valore</param>
        /// <returns>Il valore da assegnare al campo</returns>
        public static String GetValueForCustomObject(string fieldName, Templates template)
        {
            // L'oggetto custom da cui prelevare i risultati
            OggettoCustom customObject;

            // Il testo da restituire
            StringBuilder toReturn;

            // Inizializzazione del valore da restituire
            toReturn = new StringBuilder();

            // Prelevamento dell'oggetto custom con l'etichetta specificata
            // Viene anche effettuata una verifica sul flag "DA_VISUALIZZARE_RICERCA"; non è necessario farlo perchè ora con le nuove griglie tutti i campi sono visualizzabili.
            customObject = template.ELENCO_OGGETTI.Where(e => (e.DESCRIZIONE.ToUpper() + e.SYSTEM_ID).Equals(fieldName.ToUpper())).FirstOrDefault();//-- &&
            //  e.DA_VISUALIZZARE_RICERCA == "1");

            // Se l'oggetto custom è una casella di selezione, vengono mergiati i valori
            // selezionati altrimenti il valore da restituire è il valore associato al campo
            if (customObject != null)
            {
                switch (customObject.TIPO.DESCRIZIONE_TIPO.ToUpper())
                {
                    case "CASELLADISELEZIONE":
                        foreach (String value in customObject.VALORI_SELEZIONATI)
                            if (!string.IsNullOrEmpty(value))
                                toReturn.Append(value + "; ");

                        if (toReturn.Length > 0)
                            toReturn.Remove(toReturn.Length - 2, 2);
                        break;

                    case "CORRISPONDENTE":
                        DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteBySystemIDDisabled(null, customObject.VALORE_DATABASE);
                        if (corr != null)
                            toReturn.Append(corr.codiceRubrica + " " + corr.descrizione);
                        break;

                    case "LINK":
                        if (customObject.VALORE_DATABASE.Contains("||||"))
                        {
                            int stop = customObject.VALORE_DATABASE.IndexOf("||||");
                            toReturn.Append(customObject.VALORE_DATABASE.Substring(0, stop));
                        }
                        else
                            toReturn.Append(customObject.VALORE_DATABASE);
                        break;
                    //aggiunto controllo del contatore con sottocontatore
                    case "CONTATORESOTTOCONTATORE":
                        if (customObject.VALORE_DATABASE != null && !customObject.VALORE_DATABASE.Equals(""))
                            toReturn.Append(customObject.VALORE_DATABASE + "-" + customObject.VALORE_SOTTOCONTATORE);
                        break;

                    default:
                        toReturn.Append(customObject.VALORE_DATABASE);
                        break;
                }
            }
            else
                // Altrimenti viene restituito un testo N.A. Tale valore verrà interpretato 
                // dall'evento databind del campo custom
                toReturn.Append("N.A.");

            // Restituzione del valore
            return toReturn.ToString();
        }

        /// <summary>
        /// Questa funzione viene utilizzata per troncare una stringa in base a quanto specificato
        /// nelle proprietà del campo.
        /// </summary>
        /// <param name="value">Il valore da troncare</param>
        /// <param name="fieldName">Il nome del campo</param>
        /// <param name="isTruncable">True se il campo è troncabile</param>
        /// <param name="maxLength">Massima lunghezza consentita per il campo</param>
        /// <returns>Il valore tagliato a cui sono eventualmente aggiunti tre punti di sospensione finali</returns>
        public static string TrimFieldValue(string value, string fieldName, bool isTruncable, int maxLength)
        {
            // Valore da restituire
            StringBuilder toReturn;

            toReturn = new StringBuilder(value);

            // Se il valore può essre troncato, la lunghezza massima è maggiore di zero e la lunghezza del valore è maggiore di
            // MaxLength, si procede con il troncamento
            if (isTruncable && maxLength > 0 && toReturn.Length > maxLength)
            {
                toReturn.Remove(maxLength - 1, toReturn.Length - maxLength);
                toReturn.Append("...");
            }

            // Restituzione del valore troncato
            return toReturn.ToString();
        }

        #endregion

        #region Funzioni per la creazione dei vari tipi di colonne per il datagrid di ricerca

        /// <summary>
        /// Funzione per la creazione di una bound column per il datagrid
        /// </summary>
        /// <param name="label">Testo da mostrare nell'instestazione della colonna</param>
        /// <param name="originalLabel">Testo da utilizzare per il binding dei dati</param>
        /// <param name="width">Larghezza della colonna</param>
        /// <param name="boundColumnName">Nome della colonna del dataset a cui legare la colonna da creare</param>
        /// <returns>La colonna customizzata</returns>
        public static BoundColumn GetBoundColumn(String label, String originalLabel, int width, String boundColumnName)
        {
            // La colonna da restituire
            BoundColumn toReturn;

            toReturn = new BoundColumn();
            toReturn.HeaderText = label;
            toReturn.Visible = true;
            toReturn.DataField = boundColumnName;
            toReturn.ItemStyle.Width = new Unit(width, UnitType.Pixel);
            toReturn.ItemStyle.Wrap = true;
            toReturn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            toReturn.ItemStyle.VerticalAlign = VerticalAlign.Middle;

            SetHeadStyle(toReturn, width);

            // Restituzione della colonna customizzata
            return toReturn;
        }

        /// <summary>
        /// Funzione per la creazione di una colonna template
        /// </summary>
        /// <param name="label">Nome da mostrare nell'header della colonna creata</param>
        /// <param name="width">Larghezza da assegnare alla colonna</param>
        /// <param name="boundColumnName">Nome della colonna DB da cui estrarre il valore da assegnare alla cella</param>
        /// <param name="backColor">Colore di sfondo da applicare alla cella quando è destinata a contenere dati di un template ma il documento nella riga non ha quel particolare template associato</param>
        /// <param name="position">Posizione occupata dalla colonna all'interno del datagrid</param>
        /// <returns>Colonna template</returns>
        //public static TemplateColumn GetTemplateColumn(String label, int width, String boundColumnName, String backColor, int position)
        //{
        //    // La colonna da restituire
        //    TemplateColumn toReturn;

        //    // Inizializzazione della colonna come colonna template
        //    toReturn = new TemplateColumn();

        //    // Il template da associare alla colonna è una colonna con colore di sfondo variabile
        //    toReturn.ItemTemplate = new BoundColumWithCellColor(boundColumnName, backColor, position);

        //    // Impostazione delle proprietà della colonna
        //    toReturn.HeaderText = label;
        //    toReturn.Visible = true;
        //    toReturn.ItemStyle.Width = new Unit(width, UnitType.Pixel);
        //    toReturn.ItemStyle.Wrap = true;
        //    toReturn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        //    toReturn.ItemStyle.VerticalAlign = VerticalAlign.Middle;

        //    // Impostazione dello stile dell'header
        //    SetHeadStyle(toReturn, width);

        //    // Restituzione della colonna customizzata
        //    return toReturn;
        //}

        /// <summary>
        /// Funzione per la creazione di una colonna link per il datagrid
        /// </summary>
        /// <param name="label">Testo da mostrare nell'instestazione della colonna</param>
        /// <param name="boundColumnName">Testo da utilizzare per il binding dei dati</param>
        /// <returns>La colonna customizzata</returns>
        public static HyperLinkColumn GetLinkColumn(String label, String boundColumnName, int width)
        {
            // La colonna da restituire
            HyperLinkColumn toReturn;

            // Inizializzazione della colonna
            toReturn = new HyperLinkColumn();
            toReturn.HeaderText = label;
            toReturn.Visible = true;
            toReturn.ItemStyle.Width = new Unit(width, UnitType.Pixel);
            toReturn.DataTextField = boundColumnName;
            toReturn.DataNavigateUrlField = "NavigateUrl";
            toReturn.Text = "Apri il dettaglio del documento.";
            toReturn.ItemStyle.Wrap = true;
            toReturn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            toReturn.ItemStyle.VerticalAlign = VerticalAlign.Middle;

            // Impostazione dello stile per l'header della colonna
            SetHeadStyle(toReturn, width);

            // Restituzione della colonna customizzata
            return toReturn;
        }

        /// <summary>
        /// Funzione per l'assegnazione dello stile all'header della colonna del datagrid
        /// </summary>
        /// <param name="column">Colonna per cui impostare lo stile</param>
        /// <param name="width">Larghezza da assegnare all'header</param>
        public static void SetHeadStyle(DataGridColumn column, int width)
        {
            column.HeaderStyle.Width = new Unit(width, UnitType.Pixel);
            column.HeaderStyle.Wrap = true;
            column.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            column.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
        }

        /// <summary>
        /// Funzione per la creazione di una colonna speciale.
        /// Per ora sono definite due colonne speciali: le icone di ricerca e le checkbox
        /// </summary>
        /// <param name="field">Informazioni sul campo speciale da creare</param>
        /// <param name="mobButtons">Riferimento alla bottoniera delle azioni massive</param>
        /// <param name="idColumnName">Nome della colonna del dataset da utlizzare per il reperimento dell'id dell'oggetto</param>
        /// <param name="onWorkingAreaOperationCompletedEventHandler">Evento da scatenare al completamento di una azione sull'area di lavoro</param>
        /// <param name="onStorageAreaOperationCompletedEventHandler">Evento da scatenare al completamento di una azione sull'area di conservazione</param>
        /// <param name="onViewImageCompleted">Evento da lanciare nel momento in cui viene visualizzata l'immagine del documento</param>
        /// <param name="onViewSignDetailsCompleted">Evento da lanciare nel momento in cui viene visualizzato il dettaglio del documento</param>
        /// <param name="page">Pagina chiamante. E' necessario specificarla per poter invocare le funzioni di creazione e inizializzazione di controlli custom a partire dal loro path virtuale</param>
        /// <param name="objectType">Tipo di oggetto contenuto nel datagrid</param>
        /// <param name="parentPage">Pagina in cui è contenuta la griglia di ricerca</param>
        /// <param name="columnIndex">Posizione in base 0 occupata dalla colonna all'interno del datagrid</param>
        /// <returns>Colonna speciale richiesta</returns>
        public static TemplateColumn GetSpecialColumn(
            SpecialField field,
            NewMassiveOperationButtons mobButtons,
            String idColumnName,
            EventHandler onWorkingAreaOperationCompletedEventHandler,
            EventHandler onStorageAreaOperationCompletedEventHandler,
            EventHandler onViewImageCompleted,
            EventHandler onViewSignDetailsCompleted,
            Page page,
            NewSearchIcons.ObjectTypeEnum objectType,
            NewSearchIcons.ParentPageEnum parentPage,
            int columnIndex)
        {
            // Colonna da restituire
            TemplateColumn toReturn = null;

            // A seconda del tipo di campo speciale,
            // viene richiamata la funzione adatta
            switch (field.FieldType)
            {
                case SpecialFieldsEnum.Icons:       // Icone di ricerca
                    toReturn = GetSearchIconColumn(
                        page,
                        onWorkingAreaOperationCompletedEventHandler,
                        onStorageAreaOperationCompletedEventHandler,
                        onViewImageCompleted,
                        onViewSignDetailsCompleted,
                        objectType,
                        parentPage,
                        idColumnName);
                    break;
                case SpecialFieldsEnum.CheckBox:    // Checkbox per la selezione dell'item
                    toReturn = GetCheckBoxTemplateColumn(
                        mobButtons.ID,
                        idColumnName,
                        page);
                    mobButtons.CheckBoxColumnIndex = columnIndex;
                    break;

            }

            // Restituzione della colonna
            return toReturn;
        }

        /// <summary>
        /// Funzione per la generazione di una colonna template contenente
        /// le icone di ricerca
        /// </summary>
        /// <param name="page">Pagina chimante. E' necessaria per l'invocazione della funzione di caricamento ed inizializzazione di uno user control a partire dal suo path virtuale</param>
        /// <param name="onWorkingAreaOperationCompletedEventHandler">Evento da scatenare ogni volta che viene completata un'operazione sull'area di lavoro</param>
        /// <param name="onStorageAreaOperationCompletedEventHandler">Evento da scatenare ogni volta che viene completata un'operazione sull'area di conservazione</param>
        /// <param name="onViewImageCompleted">Evento da lanciare nel momento in cui viene visualizzata l'immagine del documento</param>
        /// <param name="onViewSignDetailsCompleted">Evento da lanciare nel momento in cui viene visualizzato il dettaglio del documento</param>
        /// <param name="objectType">Tipo di oggetto per cui fornire operazioni</param>
        /// <param name="parentPage">Identificativo della pagina in cui deve essere inserito il controllo</param>
        /// <param name="idFieldName">Nome della colonna del dataset che conterrà il system id dell'oggetto restituito dalla ricerca</param>
        /// <returns>Colonna template con le icone di ricerca</returns>
        private static TemplateColumn GetSearchIconColumn(
            Page page,
            EventHandler onWorkingAreaOperationCompletedEventHandler,
            EventHandler onStorageAreaOperationCompletedEventHandler,
            EventHandler onViewImageCompleted,
            EventHandler onViewSignDetailsCompleted,
            NewSearchIcons.ObjectTypeEnum objectType,
            NewSearchIcons.ParentPageEnum parentPage,
            String idFieldName)
        {
            // Creazione della colonna delle icone
            IconTemplateColumn column = new IconTemplateColumn(
                page,
                onWorkingAreaOperationCompletedEventHandler,
                onStorageAreaOperationCompletedEventHandler,
                onViewImageCompleted,
                onViewSignDetailsCompleted,
                objectType,
                parentPage,
                idFieldName);

            // Creazione e inizializzazione della colonna template e sua restituzione
            TemplateColumn toReturn = new TemplateColumn();
            toReturn.ItemTemplate = column;
            toReturn.ItemStyle.Width = Unit.Pixel(25);

            // Restituzione della colonna creata
            return toReturn;
        }

        /// <summary>
        /// Funzione per la creazione della colonna template con i
        /// checkbox
        /// </summary>
        /// <param name="mobButtonsId">Id della bottoniera delle azioni massive</param>
        /// <param name="idColumnName">Nome della colonna del datasource in cui reperire l'id dell'oggetto contenuto in una riga del datagrid</param>
        /// <param name="page">Pagina in cui è inserito il controllo</param>
        /// <returns>La colonna template da inserire nel datagrid</returns>
        private static TemplateColumn GetCheckBoxTemplateColumn(
            String mobButtonsId,
            String idColumnName,
            Page page)
        {
            // Creazione della colonna con i checkbox
            CheckBoxTemplateColumn column = new CheckBoxTemplateColumn(mobButtonsId, idColumnName, page);

            // Creazione della template column e impostazione del template
            TemplateColumn toReturn = new TemplateColumn();
            toReturn.ItemTemplate = column;

            // Impostazione della larghezza della colonna (10 pixel)
            toReturn.ItemStyle.Width = Unit.Pixel(10);

            // Restituzione della colonna creata
            return toReturn;
        }

        #endregion

        #region Clonazione griglia e sui campi

        /// <summary>
        /// Funzione per la clonazione di una griglia
        /// </summary>
        /// <param name="gridToClone">Griglia da clonare</param>
        /// <returns>Griglia clonata</returns>
        public static Grid CloneGrid(Grid gridToClone)
        {
            // Griglia clonata da restituire
            Grid toReturn = new Grid();

            // Impostazione delle proprietà della griglia da restituire
            toReturn.GridId = gridToClone.GridId;
            toReturn.GridType = gridToClone.GridType;
            toReturn.RapidSearchId = gridToClone.RapidSearchId;
            toReturn.ColorForFieldWithotTemplate = gridToClone.ColorForFieldWithotTemplate;

            // Clonaggio dei system id dei template
            List<String> templatesId = new List<string>();
            if (gridToClone.TemplatesId != null)
                templatesId.AddRange(gridToClone.TemplatesId);

            toReturn.TemplatesId = templatesId.ToArray();

            // Clonaggio dei campi
            List<Field> fields = new List<Field>();
            foreach (Field field in gridToClone.Fields)
                if (field is SpecialField)
                    fields.Add(CloneSpecialFiels((SpecialField)field));
                else
                    fields.Add(CloneField(field));
            toReturn.Fields = fields.ToArray();

            // Clonaggio della direzione dell'ordinamento e per l'impostazione del
            // campo da utilizzare per l'ordinamento
            toReturn.OrderDirection = gridToClone.OrderDirection;
            if (gridToClone.FieldForOrder != null)
                toReturn.FieldForOrder = toReturn.Fields.Where(e => e.FieldId == gridToClone.FieldForOrder.FieldId).FirstOrDefault();

            // Restituzione della griglia clonata
            return toReturn;
        }

        /// <summary>
        /// Funzione per la clonazione di un campo speciale
        /// </summary>
        /// <param name="field">Campo speciale da clonare</param>
        /// <returns>Campo clonato</returns>
        private static Field CloneSpecialFiels(SpecialField field)
        {
            // Creazione dell'oggetto clocanto da restituire
            SpecialField toReturn = new SpecialField();

            // Copia delle proprietà
            // Le colonne speciali devono essere sempre visibili
            toReturn.Visible = true;
            toReturn.Position = field.Position;
            toReturn.Locked = field.Locked;
            toReturn.FieldType = field.FieldType;
            toReturn.OriginalLabel = field.OriginalLabel;
            toReturn.Label = field.Label;
            toReturn.FieldId = field.FieldId;
            toReturn.Width = field.Width;
            toReturn.MaxLength = field.MaxLength;

            // Restituzione del campo clonato
            return toReturn;

        }

        /// <summary>
        /// Funzione per il clonaggio di un campo della griglia
        /// </summary>
        /// <param name="field">Il campo da clonare</param>
        /// <returns>Campo clonato</returns>
        private static Field CloneField(Field field)
        {
            // Il campo da restituire
            Field toReturn = new Field();

            // Copia delle proprietà
            toReturn.CanAssumeMultiValues = field.CanAssumeMultiValues;
            toReturn.CustomObjectId = field.CustomObjectId;
            toReturn.FieldId = field.FieldId;
            toReturn.IsTruncable = field.IsTruncable;
            toReturn.Label = field.Label;
            toReturn.MaxLength = field.MaxLength;
            toReturn.OriginalLabel = field.OriginalLabel;
            toReturn.Position = field.Position;
            toReturn.Visible = field.Visible;
            toReturn.Width = field.Width;
            toReturn.OracleDbColumnName = field.OracleDbColumnName;
            toReturn.SqlServerDbColumnName = field.SqlServerDbColumnName;
            toReturn.AssociatedTemplateName = field.AssociatedTemplateName;
            toReturn.Locked = field.Locked;
            toReturn.IsCommonField = field.IsCommonField;
            toReturn.AssociatedTemplateId = field.AssociatedTemplateId;
            toReturn.IsNumber = field.IsNumber;
            
            // Restituzione del campo clonato
            return toReturn;

        }

        #endregion

        #region Funzioni per la gestione dei filtri di ordinamento (anche salvataggio del filtro impostato per la girglia attuale)

        /// <summary>
        /// Funzione per la creazione di un filtro di ricerca da utilizzare per l'ordinamento
        /// dei risultati restituiti dalla ricerca documenti
        /// </summary>
        /// <param name="fieldName">Nome del campo da utilizzare per l'ordinamento</param>
        /// <param name="orderDirection">Direzione da adottare per l'ordinamento</param>
        /// <returns>Lista dei filtri di ricerca da utilizzare per la ricerca</returns>
        public static List<FiltroRicerca> GetOrderFilterForDocument(String fieldId, String orderDirection)
        {
            // Lista dei fitri da restituire
            List<FiltroRicerca> toReturn = new List<FiltroRicerca>();

            // Campo da utilizzare per l'ordinamento
            Field field = null;

            // Recupero delle informazioni sul campo
            if (!String.IsNullOrEmpty(fieldId))
            {
                field = SelectedGrid.Fields.Where(e => e.FieldId == fieldId).FirstOrDefault();
            }

            // Se il campo è valorizzato, vengono creati i filtri
            if (field != null)
            {
                // Se il campo è standard, vengono creati i due filtri di ricerca per SQL e per ORACLE
                if (field.CustomObjectId == 0)
                {
                    toReturn.Add(new FiltroRicerca()
                        {
                            argomento = FiltriDocumento.ORACLE_FIELD_FOR_ORDER.ToString(),
                            valore = field.OracleDbColumnName,
                            nomeCampo = field.FieldId,
                        });

                    toReturn.Add(new FiltroRicerca()
                        {
                            argomento = FiltriDocumento.SQL_FIELD_FOR_ORDER.ToString(),
                            valore = field.SqlServerDbColumnName,
                            nomeCampo = field.FieldId
                        });
                }
                else
                {
                    // Creazione di un filtro per la profilazione
                    toReturn.Add(new FiltroRicerca()
                        {
                            argomento = FiltriDocumento.PROFILATION_FIELD_FOR_ORDER.ToString(),
                            valore = field.CustomObjectId.ToString(),
                            nomeCampo = field.FieldId
                        });
                }
            }


            string tempValue = string.Empty;

            if (field != null)
            {
                tempValue = field.FieldId;
            }

            // Aggiunta del filtro con indicazioni sulla direzione dell'ordinamento
            toReturn.Add(new FiltroRicerca()
                {
                    argomento = FiltriDocumento.ORDER_DIRECTION.ToString(),
                    nomeCampo = tempValue,
                    valore = orderDirection
                });


            // Restituzione dei filtri creati
            return toReturn;
        }

        /// <summary>
        /// Funzione per la creazione di un filtro di ricerca da utilizzare per l'ordinamento
        /// dei risultati restituiti dalla ricerca fascicoli
        /// </summary>
        /// <param name="fieldName">Nome del campo da utilizzare per la ricerca</param>
        /// <param name="orderDirection">Direzione dell'ordinamento</param>
        /// <returns>Lista dei filtri di ordinamento</returns>
        public static List<FiltroRicerca> GetOrderFilterForProject(String fieldId, String orderDirection)
        {
            // Lista dei fitri da restituire
            List<FiltroRicerca> toReturn = new List<FiltroRicerca>();

            // Campo da utilizzare per l'ordinamento
            Field field = null;

            // Recupero delle informazioni sul campo
            if (!String.IsNullOrEmpty(fieldId))
            {
                field = SelectedGrid.Fields.Where(e => e.FieldId == fieldId).FirstOrDefault();
            }

            // Se il campo è valorizzato, vengono creati i filtri
            if (field != null)
            {
                // Se il campo è standard, vengono creati i due filtri di ricerca per SQL e per ORACLE
                if (field.CustomObjectId == 0)
                {
                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.ORACLE_FIELD_FOR_ORDER.ToString(),
                        valore = field.OracleDbColumnName,
                        nomeCampo = field.FieldId,
                    });

                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.SQL_FIELD_FOR_ORDER.ToString(),
                        valore = field.SqlServerDbColumnName,
                        nomeCampo = field.FieldId
                    });
                }
                else
                {
                    // Creazione di un filtro per la profilazione
                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.PROFILATION_FIELD_FOR_ORDER.ToString(),
                        valore = field.CustomObjectId.ToString(),
                        nomeCampo = field.FieldId
                    });
                }
            }


            string tempValue = string.Empty;

            if (field != null)
            {
                tempValue = field.FieldId;
            }

            // Aggiunta del filtro con indicazioni sulla direzione dell'ordinamento
            toReturn.Add(new FiltroRicerca()
            {
                argomento = FiltriDocumento.ORDER_DIRECTION.ToString(),
                nomeCampo = tempValue,
                valore = orderDirection
            });


            // Restituzione dei filtri creati
            return toReturn;
        }

        /// <summary>
        /// Funzione per la restituzione del filtro da utilizzare per l'ordinamento della
        /// ricerca trasmissioni
        /// </summary>
        /// <param name="fieldName">Nome del campo per cui ordinare</param>
        /// <param name="orderDirection">Direzione di ordinamento</param>
        /// <returns>Lista dei filtri da utilizzare per la ricerca</returns>
        public static List<FiltroRicerca> GetOrderFilterForTransmission(String fieldName, String orderDirection)
        {
            // Lista dei fitri da restituire
            List<FiltroRicerca> toReturn = new List<FiltroRicerca>();

            // Campo da utilizzare per l'ordinamento
            Field field = null;

            // Recupero delle informazioni sul campo
            if (!String.IsNullOrEmpty(fieldName))
                field = SelectedGrid.Fields.Where(e => e.OriginalLabel == fieldName).FirstOrDefault();

            // Se il campo è valorizzato, vengono creati i filtri
            if (field != null)
            {
                // Se il campo è standard, vengono creati i due filtri di ricerca per SQL e per ORACLE
                if (field.CustomObjectId == 0)
                {
                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriTrasmissione.ORACLE_FIELD_FOR_ORDER.ToString(),
                        valore = field.OracleDbColumnName
                    });

                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriTrasmissione.SQL_FIELD_FOR_ORDER.ToString(),
                        valore = field.SqlServerDbColumnName
                    });
                }
                else
                {
                    // Creazione di un filtro per la profilazione
                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriTrasmissione.PROFILATION_FIELD_FOR_ORDER.ToString(),
                        valore = field.CustomObjectId.ToString()
                    });
                }
            }

            // Aggiunta del filtro con indicazioni sulla direzione dell'ordinamento
            toReturn.Add(new FiltroRicerca()
            {
                argomento = FiltriTrasmissione.ORDER_DIRECTION.ToString(),
                valore = orderDirection
            });

            // Restituzione dei filtri creati
            return toReturn;

        }

        /// <summary>
        /// Questa funzione viene richiamata quando si clicca sul pulsante Salva in una pagina di ricerca
        /// Il suo obiettivo è impostare l'eventuale ordinamento per la griglia attuale
        /// </summary>
        /// <param name="orderField">Nome del campo su cui compiere l'ordinamento</param>
        /// <param name="orderDirection">Direzione dell'ordinamento</param>
        public static void SetSearchFilter(string orderField, string orderDirection)
        {
            // Impostazione della direzione di ricerca
            if (orderDirection.ToUpper() == "ASC")
                SelectedGrid.OrderDirection = OrderDirectionEnum.Asc;
            else
                SelectedGrid.OrderDirection = OrderDirectionEnum.Desc;

            // Salvataggio del campo su cui compiere l'ordinamento
            Field orderFieldInfo = SelectedGrid.Fields.Where(e => e.Label.Equals(orderField)).FirstOrDefault();
            if (orderFieldInfo != null)
                SelectedGrid.FieldForOrder = orderFieldInfo;

        }

        #endregion

        #region Funzioni per la compilazione delle Drop down list con le informazioni sui campi relavi all'ordinamento

        /// <summary>
        /// Funzione per il popolamento della Drop Down List con i campi relativi all'ordinamento
        /// e per la selezione del filtro attualmente impostato
        /// </summary>
        /// <param name="grid">Griglia su cui basare la creazione e la selezione del filtro di ricerca</param>
        /// <param name="ddlOrder">Drop down in cui inserire i possibile filtri da utilizzare per l'ordinamento</param>
        /// <param name="ddlOrderDirection">Drop down in cui inserire la direzione dell'ordinamento</param>
        public static void CompileDdlOrderAndSetOrderFilterDocuments(Grid grid, DropDownList ddlOrder, DropDownList ddlOrderDirection)
        {

            // Cancellazione degli item delle due drop down list
            ddlOrder.Items.Clear();
            ddlOrderDirection.Items.Clear();

            // Recupero dei campi su cui è possibile ordinare
            List<Field> fields = new List<Field>();

            if (IsRoleEnabledToUseGrids())
            {
                fields = grid.Fields.Where(e => e.Visible && !e.Locked && !(e.FieldId.Equals("D9"))).OrderBy(e => e.Position).ToList();
            }
            else
            {
                fields = grid.Fields.Where(e => e.Visible && !String.IsNullOrEmpty(e.SqlServerDbColumnName)).OrderBy(e => e.Position).ToList();
            }

            // Inserimento del campo di ricerca standard
            ddlOrder.Items.Add(new ListItem("Data protocollazione / Creazione", "D9"));

            // Per ogni campo
            foreach (Field field in fields)
                // Inserimento delle informazioni sul campo nella ddl dei campi su cui è possibile ordinare
                ddlOrder.Items.Add(new ListItem(field.Label, field.FieldId));

            // Inserimento delle due direzioni di ordinamento nella drop down della direzione
            ddlOrderDirection.Items.Add(new ListItem("Crescente", "ASC"));
            ddlOrderDirection.Items.Add(new ListItem("Decrescente", "DESC"));

            // Per default l'ordinamento è per "Data protocollazione / Creazione", "Crescente"
            ddlOrder.SelectedIndex = 0;
            ddlOrderDirection.SelectedIndex = 1;

            // Selezione del filtro di ricerca documenti se selezionato
            if (grid.FieldForOrder != null)
            {
                ddlOrder.ClearSelection();
                ddlOrderDirection.ClearSelection();

                for (int i = 0; i < ddlOrder.Items.Count; i++)
                    if (ddlOrder.Items[i].Text.Equals(grid.FieldForOrder.Label))
                        ddlOrder.SelectedIndex = i;
            }

            // Selezione della direzione dell'ordinamento
            if (grid.OrderDirection == OrderDirectionEnum.Asc)
                ddlOrderDirection.SelectedIndex = 0;
            else
                ddlOrderDirection.SelectedIndex = 1;

        }

        /// <summary>
        /// Funzione per il popolamento della Drop Down List con i campi relativi all'ordinamento
        /// e per la selezione del filtro attualmente impostato
        /// </summary>
        /// <param name="grid">Griglia su cui basare la creazione e la selezione del filtro di ricerca</param>
        /// <param name="ddlOrder">Drop down in cui inserire i possibile filtri da utilizzare per l'ordinamento</param>
        /// <param name="ddlOrderDirection">Drop down in cui inserire la direzione dell'ordinamento</param>
        public static void CompileDdlOrderAndSetOrderFilterProjects(Grid grid, DropDownList ddlOrder, DropDownList ddlOrderDirection)
        {
            // Cancellazione degli item delle due drop down list
            ddlOrder.Items.Clear();
            ddlOrderDirection.Items.Clear();

            // Recupero dei campi su cui è possibile ordinare
            List<Field> fields = new List<Field>();

            if (IsRoleEnabledToUseGrids())
            {
                fields = grid.Fields.Where(e => e.Visible && !e.Locked && !(e.FieldId.Equals("P20"))).OrderBy(e => e.Position).ToList();
            }
            else
            {
                fields = grid.Fields.Where(e => e.Visible && !String.IsNullOrEmpty(e.SqlServerDbColumnName)).OrderBy(e => e.Position).ToList();
            }

            // Inserimento del campo di ricerca standard
            ddlOrder.Items.Add(new ListItem("Data creazione", "P20"));

            // Per ogni campo
            foreach (Field field in fields)
                // Inserimento delle informazioni sul campo nella ddl dei campi su cui è possibile ordinare
                ddlOrder.Items.Add(new ListItem(field.Label, field.FieldId));

            // Inserimento delle due direzioni di ordinamento nella drop down della direzione
            ddlOrderDirection.Items.Add(new ListItem("Crescente", "ASC"));
            ddlOrderDirection.Items.Add(new ListItem("Decrescente", "DESC"));

            // Per default l'ordinamento è per "Data protocollazione / Creazione", "Crescente"
            ddlOrder.SelectedIndex = 0;
            ddlOrderDirection.SelectedIndex = 1;

            // Selezione del filtro di ricerca documenti se selezionato
            if (grid.FieldForOrder != null)
            {
                ddlOrder.ClearSelection();
                ddlOrderDirection.ClearSelection();

                for (int i = 0; i < ddlOrder.Items.Count; i++)
                    if (ddlOrder.Items[i].Text.Equals(grid.FieldForOrder.Label))
                        ddlOrder.SelectedIndex = i;
            }

            // Selezione della direzione dell'ordinamento
            if (grid.OrderDirection == OrderDirectionEnum.Asc)
                ddlOrderDirection.SelectedIndex = 0;
            else
                ddlOrderDirection.SelectedIndex = 1;


        }

        /// <summary>
        /// Funzione per il popolamento della Drop Down List con i campi relativi all'ordinamento
        /// e per la selezione del filtro attualmente impostato
        /// </summary>
        /// <param name="grid">Griglia su cui basare la creazione e la selezione del filtro di ricerca</param>
        /// <param name="ddlOrder">Drop down in cui inserire i possibile filtri da utilizzare per l'ordinamento</param>
        /// <param name="ddlOrderDirection">Drop down in cui inserire la direzione dell'ordinamento</param>
        public static void CompileDdlOrderAndSetOrderFilterTransmission(Grid grid, DropDownList ddlOrder, DropDownList ddlOrderDirection)
        {
            // Recupero dei campi su cui è possibile ordinare
            List<Field> fields = grid.Fields.Where(e => e.Visible && !String.IsNullOrEmpty(e.OracleDbColumnName)).OrderBy(e => e.Position).ToList();

            // Cancellazione degli item delle due drop down list
            ddlOrder.Items.Clear();
            ddlOrderDirection.Items.Clear();

            // Inserimento del campo di ricerca standard
            ddlOrder.Items.Add(new ListItem("Data invio / creazione"));

            // Per ogni campo
            foreach (Field field in fields)
                // Inserimento delle informazioni sul campo nella ddl dei campi su cui è possibile ordinare
                ddlOrder.Items.Add(new ListItem(field.Label, field.OriginalLabel));

            // Inserimento delle due direzioni di ordinamento nella drop down della direzione
            ddlOrderDirection.Items.Add(new ListItem("Crescente", "ASC"));
            ddlOrderDirection.Items.Add(new ListItem("Decrescente", "DESC"));

            // Per default l'ordinamento è per "Data protocollazione / Creazione", "Crescente"
            ddlOrder.SelectedIndex = 0;
            ddlOrderDirection.SelectedIndex = 1;

            // Selezione del filtro di ricerca documenti se selezionato
            if (grid.FieldForOrder != null)
            {
                ddlOrder.ClearSelection();
                ddlOrderDirection.ClearSelection();

                for (int i = 0; i < ddlOrder.Items.Count; i++)
                    if (ddlOrder.Items[i].Text.Equals(grid.FieldForOrder.Label))
                        ddlOrder.SelectedIndex = i;

                // Selezione della direzione dell'ordinamento
                if (grid.OrderDirection == OrderDirectionEnum.Asc)
                    ddlOrderDirection.SelectedIndex = 0;
                else
                    ddlOrderDirection.SelectedIndex = 1;

            }

        }

        public static void CompileDdlOrderAndSetOrderFilterDocumentsWithCount(Grid grid, DropDownList ddlOrder, DropDownList ddlOrderDirection)
        {
            // Recupero dei campi su cui è possibile ordinare
            List<Field> fields = grid.Fields.Where(e => e.Visible && !String.IsNullOrEmpty(e.SqlServerDbColumnName)).OrderBy(e => e.Position).ToList();

            // Cancellazione degli item delle due drop down list
            ddlOrder.Items.Clear();
            ddlOrderDirection.Items.Clear();

            // Inserimento del campo di ricerca standard
            ddlOrder.Items.Add(new ListItem("Data protocollazione / Creazione"));

            // Per ogni campo
            foreach (Field field in fields)
                // Inserimento delle informazioni sul campo nella ddl dei campi su cui è possibile ordinare
                ddlOrder.Items.Add(new ListItem(field.Label, field.OriginalLabel));

            // Inserimento delle due direzioni di ordinamento nella drop down della direzione
            ddlOrderDirection.Items.Add(new ListItem("Crescente", "ASC"));
            ddlOrderDirection.Items.Add(new ListItem("Decrescente", "DESC"));

            // Per default l'ordinamento è per "Data protocollazione / Creazione", "Crescente"
            ddlOrder.SelectedIndex = 0;
            ddlOrderDirection.SelectedIndex = 1;

            // Selezione del filtro di ricerca documenti se selezionato
            if (grid.FieldForOrder != null)
            {
                ddlOrder.ClearSelection();
                ddlOrderDirection.ClearSelection();

                for (int i = 0; i < ddlOrder.Items.Count; i++)
                    if (ddlOrder.Items[i].Text.Equals(grid.FieldForOrder.Label))
                        ddlOrder.SelectedIndex = i;
            }

            // Selezione della direzione dell'ordinamento
            if (grid.OrderDirection == OrderDirectionEnum.Asc)
                ddlOrderDirection.SelectedIndex = 0;
            else
                ddlOrderDirection.SelectedIndex = 1;

            ddlOrder.Items.Add(new ListItem("Contatore", "-2"));
            ddlOrder.SelectedValue = "-2";

        }

        /// <summary>
        /// Funzione per il caricamento della griglia standard
        /// </summary>
        /// <param name="gridType">Tipo di griglia da caricare</param>
        /// <returns>Griglia standard</returns>
        public static Grid getUserGrid(
            GridTypeEnumeration gridType)
        {
            try
            {
                DocsPaWebService ws = ProxyManager.getWS();
                InfoUtente userInfo = UserManager.getInfoUtente();

                if (IsRoleEnabledToUseGrids())
                {

                    return ws.GetUserGridCustom(
                        userInfo,
                        gridType);
                }
                else
                {
                    return ws.GetStandardGridForUser(
                      userInfo,
                       gridType);
                }

            }
            catch (Exception e)
            {
                throw new Exception("Errore durante il reperimento della griglia standard.", e);
            }
        }

        public static bool RemoveGrid(GridBaseInfo gridBase)
        {
            Boolean result = false;
            DocsPaWebService ws = ProxyManager.getWS();
            InfoUtente userInfo = UserManager.getInfoUtente();
            return ws.RemoveGrid(gridBase, userInfo);
        }


        public static List<FiltroRicerca> GetOrderFilterForDocumentInProject()
        {
            // Lista dei fitri da restituire
            List<FiltroRicerca> toReturn = new List<FiltroRicerca>();

            // Campo da utilizzare per l'ordinamento
            Field field = null;

            field = SelectedGrid.Fields.Where(e => e.FieldId == GridManager.SelectedGrid.FieldForOrder.FieldId).FirstOrDefault();

            // Se il campo è valorizzato, vengono creati i filtri
            if (field != null)
            {
                // Se il campo è standard, vengono creati i due filtri di ricerca per SQL e per ORACLE
                if (field.CustomObjectId == 0)
                {
                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.ORACLE_FIELD_FOR_ORDER.ToString(),
                        valore = field.OracleDbColumnName,
                        nomeCampo = field.FieldId,
                    });

                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.SQL_FIELD_FOR_ORDER.ToString(),
                        valore = field.SqlServerDbColumnName,
                        nomeCampo = field.FieldId
                    });
                }
                else
                {
                    // Creazione di un filtro per la profilazione
                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.PROFILATION_FIELD_FOR_ORDER.ToString(),
                        valore = field.CustomObjectId.ToString(),
                        nomeCampo = field.FieldId
                    });
                }
            }


            string tempValue = string.Empty;

            if (field != null)
            {
                tempValue = field.FieldId;
            }

            // Aggiunta del filtro con indicazioni sulla direzione dell'ordinamento
            toReturn.Add(new FiltroRicerca()
            {
                argomento = FiltriDocumento.ORDER_DIRECTION.ToString(),
                nomeCampo = tempValue,
                valore = GridManager.SelectedGrid.OrderDirection.ToString()
            });


            // Restituzione dei filtri creati
            return toReturn;
        }



        #endregion

        /// <summary>
        /// Funzione per il restore della griglia standard relativa ad un utente per una specifica tipologia di ricerca
        /// </summary>
        /// <param name="standardGrid">Griglia standard</param>
        /// <param name="gridType">Tipo di griglia da restituire</param>
        /// <returns>Griglia standard</returns>
        /*        public static Grid RestoreStandardGridForUser(GridTypeEnumeration gridType)
                {
                    try
                    {
                        DocsPaWebService ws = ProxyManager.getWS();
                        InfoUtente userInfo = UserManager.getInfoUtente();
                        Ruolo role = UserManager.getRuolo();

                        return ws.RestoreStandardGridForUser(userInfo.idPeople, role.systemId, userInfo.idAmministrazione, gridType);

                    }
                    catch (Exception e)
                    {
                        logger.Debug("Errore durante il reperimento della griglia iniziale.", e);
                        throw new Exception("Errore durante il reperimento della griglia iniziale.", e);
                    }
                }
                */
        /// <summary>
        /// Funzione per il caricamento delle informazioni di base relative alle griglie salvate per un dato utente
        /// appartenente ad un dato ruolo definito per una certa amministrazione e per un particolare tipo di ricerca
        /// </summary>
        /// <param name="userId">Identificativo dell'utente</param>
        /// <param name="roleId">Identificativo del ruolo</param>
        /// <param name="administrationId">Identificativo dell'amministrazione</param>
        /// <param name="gridType">Tipo di griglia</param>
        ///<returns>Lista di oggetti con le informazioni di base sulle griglie  definite da un utente</returns>
        public static GridBaseInfo[] GetGridsBaseInfo(InfoUtente infoUtente, GridTypeEnumeration gridType, bool allGrids)
        {
            try
            {
                DocsPaWebService ws = ProxyManager.getWS();

                return ws.GetGridsBaseInfo(infoUtente, gridType, allGrids);

            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il reperimento delle informazioni di base sulle griglie.", e);
                throw new Exception("Errore durante il reperimento delle informazioni di base sulle griglie.", e);
            }

        }

        public static void RemovePreferred(String userId, String roleId, String administrationId, GridTypeEnumeration gridType)
        {
            DocsPaWebService ws = ProxyManager.getWS();

            ws.RemovePreferred(userId, roleId, administrationId, gridType);
        }
    }

    /// <summary>
    /// Questa classe rappresenta una colonna con celle di colore impostabile.
    /// Il colore verrà impostato se la cella, destinata a contenere valori assunti da un campo
    /// profilato, non è valorizzata in quanto il template non è associato al documento i cui dati
    /// sono riportati nella stessa riga.
    /// </summary>
    public class BoundColumWithCellColor : ITemplate
    {
        // Nome della colonna del dataset che contiene il dato da visualizzare
        String boundColumnName;
        // Colore da assegnare allo sfondo della colonna se il dati non è applicabile al documento i cui
        // dati sono contenuti nella stessa riga
        Color color;
        // Posizione occupata dalla colonna all'interno della griglia
        int columnPosition;

        /// <summary>
        /// Funzione per l'inizializzazione della colonna
        /// </summary>
        /// <param name="boundColumnName">Nome della colonna del dataset da cui prelevare il dato da visualizzare</param>
        /// <param name="backColor">Colore da assegnare allo sfondo della cella</param>
        /// <param name="columnPosition">Posizione occupata dalla colonna all'interno della griglia</param>
        public BoundColumWithCellColor(String boundColumnName, String backColor, int columnPosition)
        {
            this.boundColumnName = boundColumnName;
            this.color = ColorTranslator.FromHtml(String.Format("#{0}", String.IsNullOrEmpty(backColor) ? "000000" : backColor));
            this.columnPosition = columnPosition;
        }

        public void InstantiateIn(Control container)
        {
            Label lblText = new Label();
            lblText.ID = "lblText" + Guid.NewGuid();
            lblText.DataBinding += new EventHandler(this.lblText_DataBind);

            container.Controls.Add(lblText);

        }

        protected void lblText_DataBind(object sender, EventArgs e)
        {
            Label lblText = sender as Label;

            DataGridItem container = lblText.NamingContainer as DataGridItem;

            String tmp = DataBinder.Eval(container.DataItem, this.boundColumnName).ToString();

            if (tmp == "N.A.")
                container.Cells[this.columnPosition].BackColor = this.color;
            else
                lblText.Text = tmp;

        }


    }

    /// <summary>
    /// Questa classe rappresenta una colonna utilizzata per contenere le icone di
    /// ricerca
    /// </summary>
    public class IconTemplateColumn : ITemplate
    {
        // Pagina contenente il datagrid
        Page page;
        // Eventi da generare al cmpletamento di operazioni nell'area di lavoro 
        // e nell'area di conservazione
        EventHandler onTerminateWAOp, onTerminateSAOp;
        // Eventi da generare al completamento delle operazioni di visualizzazione
        // documento e visualizzazione del dettaglio della firma
        EventHandler onViewImageCompleted, onViewSignDetailsCompleted;
        // Tipo di oggetto contenuto nella griglia
        NewSearchIcons.ObjectTypeEnum objectType;
        // Pagina in cui è inserita la griglia
        NewSearchIcons.ParentPageEnum parentPage;
        // Nome della colonna contente l'id dell'oggetto
        String idFieldName;

        /// <summary>
        /// Funzione per la creazione della colonna con le icone di ricerca
        /// </summary>
        /// <param name="page">Pagina chiamante</param>
        /// <param name="onWorkingAreaOperationCompletedEventHandler">Evento da scatenare al completamento di una azione sull'area di lavoro</param>
        /// <param name="onStorageAreaOperationCompletedEventHandler">Evento da scatenare al completamento di una azione sull'area di conservazione</param>
        /// <param name="onViewImageCompleted">Evento da lanciare nel momento in cui viene visualizzata l'immagine del documento</param>
        /// <param name="onViewSignDetailsCompleted">Evento da lanciare nel momento in cui viene visualizzato il dettaglio del documento</param>
        /// <param name="objectType">Tipo di oggetto contenuto nella griglia di ricerca</param>
        /// <param name="parentPage">Pagina in cui è inserita la griglia</param>
        /// <param name="idFieldName">Nome della colonna del dataset contente l'id dell'oggetto</param>
        public IconTemplateColumn(
            Page page,
            EventHandler onWorkingAreaOperationCompletedEventHandler,
            EventHandler onStorageAreaOperationCompletedEventHandler,
            EventHandler onViewImageCompleted,
            EventHandler onViewSignDetailsCompleted,
            NewSearchIcons.ObjectTypeEnum objectType,
            NewSearchIcons.ParentPageEnum parentPage,
            String idFieldName)
        {
            this.page = page;
            this.onTerminateWAOp = onWorkingAreaOperationCompletedEventHandler;
            this.onTerminateSAOp = onStorageAreaOperationCompletedEventHandler;
            this.onViewImageCompleted = onViewImageCompleted;
            this.onViewSignDetailsCompleted = onViewSignDetailsCompleted;
            this.objectType = objectType;
            this.parentPage = parentPage;
            this.idFieldName = idFieldName;

        }

        public void InstantiateIn(Control container)
        {
            Control control = this.page.LoadControl("~/UserControls/NewSearchIcons.ascx");
            NewSearchIcons siIcons = control as NewSearchIcons;
            siIcons.Visible = true;
            siIcons.DataBinding += new EventHandler(this.BindSearchIcons);
            siIcons.ObjectType = this.objectType;
            siIcons.ParentPage = this.parentPage;
            siIcons.AjaxAlertBoxID = "mbAlert";
            siIcons.OnStorageAreaOperationCompleted = this.onTerminateSAOp;
            siIcons.OnWorkingAreaOperationCompleted = this.onTerminateWAOp;
            siIcons.ShowDocumentRemove = this.parentPage == NewSearchIcons.ParentPageEnum.SearchDocumentInProject;
            container.Controls.Add(siIcons);
            if (this.onViewImageCompleted != null)
                siIcons.OnViewImageCompleted += new EventHandler(this.onViewImageCompleted);
            if (this.onViewSignDetailsCompleted != null)
                siIcons.OnViewSignDetailsCompleted += new EventHandler(this.onViewSignDetailsCompleted);

        }

        protected void BindSearchIcons(object sender, EventArgs e)
        {
            NewSearchIcons icons = sender as NewSearchIcons;
            DataGridItem container = icons.NamingContainer as DataGridItem;
            icons.IsInStorageArea = Convert.ToBoolean(DataBinder.Eval(container.DataItem, "IsInStorageArea"));
            icons.IsInWorkingArea = Convert.ToBoolean(DataBinder.Eval(container.DataItem, "IsInWorkingArea"));
            icons.ObjectId = DataBinder.Eval(container.DataItem, this.idFieldName).ToString();
            icons.NavigateUrl = DataBinder.Eval(container.DataItem, "NavigateUrl").ToString();

            if (this.objectType == NewSearchIcons.ObjectTypeEnum.Document)
            {
                icons.FileExtension = DataBinder.Eval(container.DataItem, "FileExtension").ToString();
                icons.IsSigned = Convert.ToBoolean(DataBinder.Eval(container.DataItem, "IsSigned"));
            }

        }

    }

    /// <summary>
    /// Questa classe rappresenta la colonna dei checkbox
    /// </summary>
    public class CheckBoxTemplateColumn : ITemplate
    {
        // Id della bottoniera e nome della colonna del dataset che contiene l'id dell'oggetto
        String mobButtonsId, idColumnName;
        // Pagina chiamante
        Page page;

        /// <summary>
        /// Funzione per l'inizializzazione della colonna con i checkbox
        /// da inserire nella griglia
        /// </summary>
        /// <param name="mobButtonsId">Id della barra delle azioni massive</param>
        /// <param name="idColumnName">Nome della colonna in cui sarà reperibile l'id dell'oggetto cui fa riferimento la riga in cui inserire il controllo</param>
        /// <param name="page">Pagina in cui è inserito il controllo</param>
        public CheckBoxTemplateColumn(String mobButtonsId, String idColumnName, Page page)
        {
            this.mobButtonsId = mobButtonsId;
            this.idColumnName = idColumnName;
            this.page = page;

        }

        public void InstantiateIn(Control container)
        {
            // Caricamento dello user control con le checkbox
            Control control = page.LoadControl("~/UserControls/GridsCheckBox.ascx");

            // Conversione del controllo creato al tipo GridsCheckBox e impostazione
            // dell'id della bottoniera e dell'id della colonna con il system id dell'oggetto
            GridsCheckBox ctrl = control as GridsCheckBox;
            ctrl.MassiveOperationButtonsId = this.mobButtonsId;
            ctrl.SystemIdFieldName = this.idColumnName;
            ctrl.Visible = true;

            // Aggiunta del controllo appena creato al container
            container.Controls.Add(control);

        }




    }

}
