using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;
using System.Data;
using System.Drawing;
using DocsPAWA.SiteNavigation;
using DocsPaUtils.LogsManagement;
using log4net;

namespace DocsPAWA.Grids
{
    public partial class GridPersonalization : CssPage
    {
        private static ILog logger = LogManager.GetLogger(typeof(Grid));
        protected string colore = string.Empty;

        /// <summary>
        /// Griglia in fase di modifica
        /// </summary>
        private Grid TemporaryGrid
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["TemporaryGrid"] as Grid;
            }
            set
            {
                CallContextStack.CurrentContext.ContextState["TemporaryGrid"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Inizializzazione della pagina
            Utils.startUp(this);

            // Se non si è in postback viene inizializzata la pagina
            if (!IsPostBack)
            {
                this.InitializePage();
                string result = string.Empty;
                if (Request.QueryString["tabRes"] != string.Empty && Request.QueryString["tabRes"] != null)
                {
                    result = Request.QueryString["tabRes"].ToString();
                }
                this.hid_tab_est.Value = result;
            }

        }

        /// <summary>
        /// Funzione per l'inizializzazione della pagina.
        /// </summary>
        private void InitializePage()
        {
            // Caricamento della griglia attuale
            this.TemporaryGrid = GridManager.CloneGrid(GridManager.SelectedGrid);

            // Caricamento dei template
            try
            {
                this.LoadTemplates(this.TemporaryGrid.GridType);
            }
            catch (Exception e)
            {
                logger.Debug(e);
                this.ambMessageBox.ShowMessage("Si è verificato un errore durante il reperimento dei modelli di trasmissione.");
            }

            // Se la griglia è associata ad una ricerca, le liste dei campi non possono
            // essere modificate
            if (!String.IsNullOrEmpty(this.TemporaryGrid.RapidSearchId)
                && this.TemporaryGrid.RapidSearchId != "-1")
                this.pnlTemplateManagement.Enabled = false;

            // Impostazione della direzione di ordinamento
            if (this.TemporaryGrid.OrderDirection == OrderDirectionEnum.Asc)
                this.ddlAscDesc.SelectedIndex = 0;
            else
                this.ddlAscDesc.SelectedIndex = 1;

            // Impostazione del colore da assegnare alle celle custom
            this.txtColor.Text = this.TemporaryGrid.ColorForFieldWithotTemplate;
            this.hfBackColor.Value = this.txtColor.Text;

            // Aggiornamento della sezione grafica
            this.UpdateGraphic(this.TemporaryGrid);

        }

        /// <summary>
        /// Questa funzione carica i template realtivi a documenti o fascicoli visibili 
        /// all'utente
        /// </summary>
        /// <param name="gridTypeEnumeration">Tipo di ricerca in cui è inserita la griglia</param>
        private void LoadTemplates(GridTypeEnumeration gridTypeEnumeration)
        {
            switch (gridTypeEnumeration)
            {
                case GridTypeEnumeration.Document:
                    this.LoadDocumentTemplates();
                    break;
                case GridTypeEnumeration.Project:
                    this.LoadProjectTemplates();
                    break;
                case GridTypeEnumeration.Transmission:
                    break;

            }

        }

        /// <summary>
        /// Funzione per il caricamento dei template per la ricerca documenti
        /// </summary>
        private void LoadDocumentTemplates()
        {
            // Lista dei template disponibili per i documenti
            TipologiaAtto[] templates;

            // Caricamento dei tipi atto
            templates = DocumentManager.getTipoAttoPDInsRic(
                this,
                UserManager.getInfoUtente(this).idAmministrazione,
                UserManager.getRuolo(this).idGruppo,
                "1");

            // Se è stato trovato almeno un tipo atto, viene inserito un item vuoto
            if (templates.Length > 0)
                this.ddlVisibleTemplates.Items.Add(new ListItem(String.Empty, String.Empty));

            // Popolamento delle liste di modelli
            foreach (TipologiaAtto template in templates)
                // Se il template è presente nella lista dei template presenti
                // nella griglia, vengono inserite le informazioni sul template
                // nella lista dei template utilizzati, altrimenti viene
                // aggiunto alla lista dei template disponibili
                if (this.TemporaryGrid.TemplatesId != null &&
                    this.TemporaryGrid.TemplatesId.Where(e => e.Equals(template.systemId)).Count() > 0)
                    this.lstTemplates.Items.Add(
                        new ListItem(template.descrizione, template.systemId));
                else
                    this.ddlVisibleTemplates.Items.Add(
                        new ListItem(template.descrizione, template.systemId));
        }

        /// <summary>
        /// Funzione per il caricamento dei template relativi ai fascicoli
        /// </summary>
        private void LoadProjectTemplates()
        {
            // Lista dei template disponibili per i fascicoli
            Templates[] templates;

            // Caricamento dei tipi atto
            templates = (Templates[])(ProfilazioneFascManager.getTipoFascFromRuolo(
                UserManager.getInfoUtente(this).idAmministrazione,
                UserManager.getRuolo(this).idGruppo,
                "1",
                this).ToArray(typeof(Templates)));

            if (templates.Length > 0)
                this.ddlVisibleTemplates.Items.Add(new ListItem("", ""));

            // Popolamento delle liste di modelli
            foreach (Templates template in templates)
                // Se il template è presente nella lista dei template presenti
                // nella griglia, vengono inserite le informazioni sul template
                // nella lista dei template utilizzati, altrimenti viene
                // aggiunto alla lista dei template disponibili
                if (this.TemporaryGrid.TemplatesId != null &&
                    this.TemporaryGrid.TemplatesId.Where(e => e.Equals(template.SYSTEM_ID.ToString())).Count() > 0)
                    this.lstTemplates.Items.Add(
                        new ListItem(template.DESCRIZIONE, template.SYSTEM_ID.ToString()));
                else
                    this.ddlVisibleTemplates.Items.Add(
                        new ListItem(template.DESCRIZIONE, template.SYSTEM_ID.ToString()));
        }

        /// <summary>
        /// Funzione responsabile dell'aggiornamento delle informazioni visualizzate sulla griglia
        /// </summary>
        /// <param name="grid">La griglia da visualizzare</param>
        private void UpdateGraphic(Grid grid)
        {
            // Binding dei campi visibili ordinati per posizione occupata
            this.gvFields.DataSource = grid.Fields.OrderBy(e => e.Position);
            this.gvFields.DataBind();

            // Creazione del data source con le informazioni sui campi utilizzabili per l'ordinamento
            // Viene creato un datasource con due colonne: 
            // - Text che conterrà nome del campo e nome del template cui appartiene il campo
            // - Id del campo
            DataTable dataTable = new DataTable();
            DataRow dataRow;
            dataTable.Columns.Add("Text", typeof(String));
            dataTable.Columns.Add("Id", typeof(String));

            List<Field> fields = grid.Fields.Where(e => e.Visible && !String.IsNullOrEmpty(e.SqlServerDbColumnName)).OrderBy(f => f.Position).ToList();
            foreach (Field field in fields)
            {
                dataRow = dataTable.NewRow();
                dataRow["Text"] = String.IsNullOrEmpty(field.AssociatedTemplateName) ? field.Label :
                    String.Format("{0} ({1})", field.Label, field.AssociatedTemplateName);
                dataRow["Id"] = field.FieldId;
                dataTable.Rows.Add(dataRow);
            }

            // Azzeramento degli item della DDL dell'ordinamento
            this.ddlOrder.Items.Clear();
            
            // Inserimento di un item vuoto
            // Se l'oggetto cui è legata la griglia è un documento,
            // viene aggiunto un item con testo "Data Protocollazione / Protocollazione"
            // altrimenti ne viene impostato uno pari a "Data creazione"
            switch (this.TemporaryGrid.GridType)
            {
                case GridTypeEnumeration.Document:
                    this.ddlOrder.Items.Add("Data protocollazione / Creazione");
                    break;
                case GridTypeEnumeration.Project:
                    this.ddlOrder.Items.Add("Data creazione");
                    break;

            }

            // Associazione della sorgente dati alla DDL dell'ordinamento
            this.ddlOrder.DataSource = dataTable;
            this.ddlOrder.DataBind();

            // Selezione del criterio di ricerca selezionato (se ce n'è uno)
            if (this.TemporaryGrid.FieldForOrder != null)
                for (int i = 0; i < this.ddlOrder.Items.Count; i++)
                    if (this.ddlOrder.Items[i].Text == String.Format("{0} ({1})", 
                        this.TemporaryGrid.FieldForOrder.Label, 
                        this.TemporaryGrid.FieldForOrder.AssociatedTemplateName))
                    {
                        this.ddlOrder.SelectedIndex = i;
                        break;
                    }

            // Se il la ddl del campo da utilizzare per l'ordinamento è
            // ha come indice selezionato 0 o -1, viene cancellato il campo dalle
            // impostazioni della griglia
            if (this.ddlOrder.SelectedIndex < 1)
                this.TemporaryGrid.FieldForOrder = null;
        }

        /// <summary>
        /// Al click sul link si deve aprire il popup delle proprietà
        /// </summary>
        protected void lbProperties_Click(object sender, EventArgs e)
        {
            // Casting del sender a link
            Button obj = sender as Button;

            // Se il pannello delle proprietà è visualizzato, viene
            // eseguito il salvataggio delle impostazioni relative al campo
            // in modifica
            if (this.pnlFieldProperties.Visible)
                this.SaveCurrentFieldProperties();

            // Caricamento e visualizzazione delle proprietà relative al campo
            // legato al pulsante cliccato
            this.CompileCurrentFieldProperties(obj.CommandArgument);

            // Aggiornamento della grafica
            this.UpdateGraphic(this.TemporaryGrid);

            // Visualizzazione popup
            this.pnlFieldProperties.Visible = true;

        }

        /// <summary>
        /// Questa funzione salva le informazioni relative al campo attualmente visualizzato
        /// </summary>
        private void SaveCurrentFieldProperties()
        {
            // Il campo di cui salvare le proprietà
            Field field;

            // Recupero delle proprietà del campo selezionato
            field = this.TemporaryGrid.Fields.Where(
                f => f.FieldId == this.hfFieldId.Value).FirstOrDefault();

            // Aggiornamento dei dati sul campo
            int maxLength, width;
            Int32.TryParse(this.txtLength.Text, out maxLength);
            Int32.TryParse(this.ddlWidth.SelectedItem.Value, out width);
            // Eliminazione default a 100
            //field.MaxLength = maxLength > 0 ? maxLength : -1;
            field.MaxLength = maxLength > 0 ? maxLength : 100;
            field.Width = width;
            field.Visible = this.chkVisible.Checked;
            if (!String.IsNullOrEmpty(this.txtLabel.Text))
                field.Label = this.txtLabel.Text;

        }

        /// <summary>
        /// Questa funzione carica le informazioni salvate per un campo
        /// e le visualizza nella sezione delle proprietà relative ai campi
        /// </summary>
        /// <param name="fieldId">Id del campo di cui visualizzare le proprietà</param>
        private void CompileCurrentFieldProperties(String fieldId)
        {
            // Recupero delle proprietà del campo
            Field field = this.TemporaryGrid.Fields.Where(g => g.FieldId == fieldId).FirstOrDefault();

            // Impostazione della sezione con le proprietà del campo
            if (field.IsTruncable)
                this.txtLength.Text = field.MaxLength.ToString();
                // Default a 100
                //this.txtLength.Text = field.MaxLength > 0 ? field.MaxLength.ToString() : "";
            else
                this.txtLength.Enabled = false;

            this.ddlWidth.SelectedValue = field.Width.ToString();
            this.chkVisible.Checked = field.Visible;
            this.hfFieldId.Value = field.FieldId;
            this.txtLabel.Text = field.Label;
            this.lblTitle.Text = String.Format("Proprietà del campo {0}", field.Label);

            // Se il campo è Locked vengono disabilitate tutte le caselle di testo
            if (field.Locked)
            {
                this.txtLabel.Enabled = false;
                this.txtLength.Enabled = false;
                this.ddlWidth.Enabled = false;
                this.chkVisible.Enabled = false;
            }
            else
            {
                this.txtLabel.Enabled = true;
                this.txtLength.Enabled = true;
                this.ddlWidth.Enabled = true;
                this.chkVisible.Enabled = true;
            }

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
                this.tr1.BgColor = colorsplit[2];
                this.lblTitle.ForeColor = System.Drawing.Color.White;
            }
            else
                this.tr1.BgColor = "810d06";

        }

        /// <summary>
        /// Al clicm su questo pulsante vengono salvate le impostazioni per la griglia corrente
        /// </summary>
        protected void btnSaveGridSettings_Click(object sender, EventArgs e)
        {
            // Salvataggio dei dati relativi all'eventuale campo selezionato
            if (this.pnlFieldProperties.Visible)
                this.SaveCurrentFieldProperties();

            // La griglia temporanea diventa la griglia attuale
            GridManager.SelectedGrid = this.TemporaryGrid;

            // Impostazione del campo su cui compiere l'ordinamento
            if (this.ddlOrder.SelectedIndex == 0)
                // Se l'oggetto legato alla griglia è un documento, viene impostato a null
                // il filtro altrimenti viene impostato un filtro pari a "DTA_CREAZIONE"
                if(this.TemporaryGrid.GridType == GridTypeEnumeration.Project)
                    this.TemporaryGrid.FieldForOrder = new Field() 
                        {
                            OracleDbColumnName = "DTA_CREAZIONE",
                            SqlServerDbColumnName = "DTA_CREAZIONE",
                            CustomObjectId = 0
                        };
                else
                    GridManager.SelectedGrid.FieldForOrder = null;
            else
            {
                // Altrimenti, viene prelevato il campo da utilizzare per l'ordinamento
                GridManager.SelectedGrid.FieldForOrder = GridManager.SelectedGrid.Fields.Where(
                    f => f.FieldId.Equals(this.ddlOrder.SelectedItem.Value)).FirstOrDefault();
                
            }

            // Impostazione della direzione di ordinamento
            if (this.ddlAscDesc.SelectedValue == "Asc")
                GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Asc;
            else
                GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Desc;

            // Salvataggio del colore di sfondo
            GridManager.SelectedGrid.ColorForFieldWithotTemplate = this.hfBackColor.Value;

            // Reset della griglia temporanea
            this.TemporaryGrid = null;

            try
            {
                // Salvataggio delle informazioni sulla griglia
           //     GridManager.SaveGrid(GridManager.SelectedGrid, UserManager.getInfoUtente(this));
            }
            catch (Exception ex)
            {
                this.ambMessageBox.ShowMessage(ex.Message);
            }

        }

        /// <summary>
        /// Alla pressione su questo pulsante, viene aggiunto il template selezionato alla lista
        /// dei template inseriti nella griglia (questo comporta anche una aggiunta dei 
        /// campi appartenti al template)
        /// </summary>
        protected void btnAddTemplateFields_Click(object sender, EventArgs e)
        {
            // Dettagli del template da aggiungere
            Templates template;

            // Lista dei diritti del ruolo suoi campi
            AssDocFascRuoli[] right;

            // Lista dei campi presenti nella griglia
            List<Field> fields;

            // L'item selezionato nella drop down list dei template presenti
            ListItem item;

            // Recupero dell'item selezionato nella ddl dei template visibili
            item = this.ddlVisibleTemplates.SelectedItem;

            // Creazione della lista dei campi presenti nella griglia
            fields = new List<Field>(this.TemporaryGrid.Fields);

            // Ricalcolo degli indici posizionali dei campi
            int nextPosition = 0;
            for (nextPosition = 0; nextPosition < fields.Count; nextPosition++)
                fields[nextPosition].Position = nextPosition;

            try
            {
                // Caricamento del template
                if (this.TemporaryGrid.GridType == GridTypeEnumeration.Document)
                {
                    template = ProfilazioneDocManager.getTemplateById(item.Value, this);
                    right = (AssDocFascRuoli[])ProfilazioneDocManager.getDirittiCampiTipologiaDoc(UserManager.getRuolo().idGruppo, item.Value, this).ToArray(typeof(AssDocFascRuoli));
                }
                else
                {
                    template = ProfilazioneFascManager.getTemplateFascById(item.Value, this);
                    right = (AssDocFascRuoli[])ProfilazioneFascManager.getDirittiCampiTipologiaFasc(UserManager.getRuolo().idGruppo, item.Value, this).ToArray(typeof(AssDocFascRuoli));
                }

                foreach (OggettoCustom obj in template.ELENCO_OGGETTI)
                {
                    AssDocFascRuoli ro = right.Where(o => o.ID_OGGETTO_CUSTOM == obj.SYSTEM_ID.ToString() && o.VIS_OGG_CUSTOM == "1").FirstOrDefault();

                    // Se la griglia non contiene già un campo con system id pari a quello
                    // che si sta tentando di inserire, viene inserito
                    if(fields.Where(f => f.CustomObjectId == obj.SYSTEM_ID).Count() == 0 && ro != null)
                        fields.Add(new Field()
                        {
                            FieldId = String.Format("T{0}", obj.SYSTEM_ID),
                            Label = obj.DESCRIZIONE,
                            OriginalLabel = obj.DESCRIZIONE,
                            Visible = false,
                            CanAssumeMultiValues = obj.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("CASELLADISELEZIONE"),
                            IsTruncable = true,
                            CustomObjectId = obj.SYSTEM_ID,
                            AssociatedTemplateName = template.DESCRIZIONE,
                            //MaxLength = 100,
                            // Default a 100
                            MaxLength = -1,
                            Width = 100,
                            Position = nextPosition++,
                            OracleDbColumnName = obj.SYSTEM_ID.ToString(),
                            SqlServerDbColumnName = obj.SYSTEM_ID.ToString(),
                            IsCommonField = !String.IsNullOrEmpty(obj.CAMPO_COMUNE) && obj.CAMPO_COMUNE == "1"
                        });
                }

                // Impostazione della nuova lista dei campi per la griglia attuale
                this.TemporaryGrid.Fields = fields.ToArray();
                List<String> idTemplates = new List<String>();
                if (this.TemporaryGrid.TemplatesId != null)
                    idTemplates.AddRange(this.TemporaryGrid.TemplatesId);
                idTemplates.Add(template.SYSTEM_ID.ToString());
                this.TemporaryGrid.TemplatesId = idTemplates.ToArray();
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                this.ambMessageBox.ShowMessage("Errore durante il recupero dei dettagli sul modello selezionato.");
            }

            this.btnAddTemplateFields.Enabled = false;
            this.ddlVisibleTemplates.SelectedIndex = -1;
            this.lstTemplates.Items.Add(item);
            this.ddlVisibleTemplates.Items.Remove(item);

            // Bindind dei campi con la reorder list
            this.UpdateGraphic(this.TemporaryGrid);

        }

        /// <summary>
        /// Alla pressione di questo pulsante, viene rimosso dalla lista dei template
        /// inseriti, il template selezionato (questo comporta anche una rimozione dei campi
        /// appartenenti al template)
        /// </summary>
        protected void btnRemoveTemplateFields_Click(object sender, EventArgs e)
        {

            // Lista dei campi definiti per la griglia attuale
            List<Field> fields;

            // Template da rimuovere
            Templates template;

            // Recupero dell'item selezionato nella ddl dei modelli inseriti nella ricerca
            ListItem item = this.lstTemplates.SelectedItem;
            item.Selected = false;

            try
            {
                // Caricamento del template
                if (this.TemporaryGrid.GridType == GridTypeEnumeration.Document)
                    template = ProfilazioneDocManager.getTemplateById(item.Value, this);
                else
                    template = ProfilazioneFascManager.getTemplateFascById(item.Value, this);

                // Inizializzazione della lista dei campi del template
                fields = new List<Field>(this.TemporaryGrid.Fields);

                // Rimozione dalla lista dei campi, dei campi presenti nel template
                foreach (OggettoCustom obj in template.ELENCO_OGGETTI)
                {
                    Field tempField = fields.Where(f => f.CustomObjectId.Equals(obj.SYSTEM_ID)).FirstOrDefault(); 
                    if(tempField != null)
                        fields.Remove(tempField);
                }

                // Rimozione dell'id template dalla lista dei template
                if (this.TemporaryGrid.TemplatesId != null)
                {
                    List<String> temp = new List<string>(this.TemporaryGrid.TemplatesId);
                    temp.Remove(template.SYSTEM_ID.ToString());
                    this.TemporaryGrid.TemplatesId = temp.ToArray();
                }

                // Rinumerazione della posizione occupata dai campi
                for (int i = 0; i < fields.Count; i++)
                    fields[i].Position = i;

                // Impostazione della lista dei campi
                this.TemporaryGrid.Fields = fields.ToArray();
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                this.ambMessageBox.ShowMessage("Errore durante il recupero del dettaglio del modello da rimuovere.");

            }

            // Rimozione del template dalla ddl dei template presenti nella griglia
            // e sua aggiunta nella ddl dei template disponibili
            this.lstTemplates.Items.Remove(item);
            this.ddlVisibleTemplates.Items.Add(item);
            this.lstTemplates.SelectedIndex = -1;

            // Disabilitazione del pulsante di rimozione template
            this.btnRemoveTemplateFields.Enabled = false;

            // Bindind dei campi con la reorder list
            this.UpdateGraphic(this.TemporaryGrid);

        }

        /// <summary>
        /// Spostamento del campo in su se non è già il primo campo
        /// </summary>
        protected void btnUp_Click(object sender, EventArgs e)
        {
            // Il campo da spostare
            Field field;

            int i;
            // Recupero delle proprietà del campo selezionato
            for (i = 0; i < this.TemporaryGrid.Fields.Length; i++)
                if (this.TemporaryGrid.Fields[i].FieldId == this.hfFieldId.Value)
                    break;

            // Se i è maggiore di 0, viene scambiato l'elemento in posizione i con
            // quello in posizione i - 1
            if (i > 0)
            {
                Field tempField = this.TemporaryGrid.Fields[i];
                int tempPosition = tempField.Position;
                this.TemporaryGrid.Fields[i] = this.TemporaryGrid.Fields[i - 1];
                this.TemporaryGrid.Fields[i - 1] = tempField;
                this.TemporaryGrid.Fields[i].Position = tempPosition;
                this.TemporaryGrid.Fields[i - 1].Position = tempPosition - 1;
            }

            // Aggiornamento della grafica
            this.UpdateGraphic(this.TemporaryGrid);

        }

        /// <summary>
        /// Spostamento del campo in basso se non è stata raggiunta l'ultima posizione
        /// </summary>
        protected void btnDown_Click(object sender, EventArgs e)
        {
            // Il campo da spostare
            Field field;

            int i;
            // Recupero delle proprietà del campo selezionato
            for (i = 0; i < this.TemporaryGrid.Fields.Length; i++)
                if (this.TemporaryGrid.Fields[i].FieldId == this.hfFieldId.Value)
                    break;

            // Se i è minore del numero di campi, viene scambiato l'elemento in posizione i con
            // quello in posizione i + 1
            if (i < this.TemporaryGrid.Fields.Length - 1)
            {
                Field tempField = this.TemporaryGrid.Fields[i];
                int tempPosition = tempField.Position;
                this.TemporaryGrid.Fields[i] = this.TemporaryGrid.Fields[i + 1];
                this.TemporaryGrid.Fields[i + 1] = tempField;
                this.TemporaryGrid.Fields[i + 1].Position = tempPosition + 1;
                this.TemporaryGrid.Fields[i].Position = tempPosition;
            }

            // Aggiornamento della grafica
            this.UpdateGraphic(this.TemporaryGrid);
        }

        /// <summary>
        /// In base all'indice dell'elemento selezionato nella ddl dei campi visibili, viene 
        /// abilitato o disabilitato il bottone per l'aggiunta del template alla lista dei 
        /// template
        /// </summary>
        protected void ddlVisibleTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Se l'indice selezionato è 0 il bottone di aggiunta deve essere nascosto
            if (((DropDownList)sender).SelectedIndex == 0)
                this.btnAddTemplateFields.Enabled = false;
            else
                this.btnAddTemplateFields.Enabled = true;

        }

        /// <summary>
        /// Prima del prerender viene ordinato il contenuto della Drop down list
        /// </summary>
        protected void ddlVisibleTemplates_PreRender(object sender, EventArgs e)
        {
            ListItem[] items = this.OrderCollection(((DropDownList)sender).Items);
            this.ddlVisibleTemplates.Items.Clear();
            this.ddlVisibleTemplates.Items.AddRange(items);
 
        }

        /// <summary>
        /// Questa funzione si occupa di ordinare gli elementi di una drop down list
        /// </summary>
        /// <param name="listItemCollection">Gli elementi da ordinare</param>
        private ListItem[] OrderCollection(ListItemCollection listItemCollection)
        {
            // Lista degli item ordinati
            List<ListItem> itemsLabel = new List<ListItem>();

            // Compilazione della lista degli item
            foreach (ListItem item in listItemCollection)
                itemsLabel.Add(item);

            // Ordinamento della lista e restituzione della lista ordinata
            return itemsLabel.OrderBy(e => e.Text).ToArray();

        }

        /// <summary>
        /// Al cambio dell'indice dell'elemento selezionato nella lista dei template inseriti
        /// nella griglia, viene abilitato o disabilitato il bottone per la rimozione del 
        /// template
        /// </summary>
        protected void lstTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Se l'indice selezionato è -1, il bottone di rimozione viene disabilitato
            if (((ListBox)sender).SelectedIndex == -1)
                this.btnRemoveTemplateFields.Enabled = false;
            else
                this.btnRemoveTemplateFields.Enabled = true;
        }

        /// <summary>
        /// Al prerender, vengono ordinati gli elementi della lista
        /// </summary>
        protected void lstTemplates_PreRender(object sender, EventArgs e)
        {
            ListItem[] items = this.OrderCollection(((ListBox)sender).Items);
            this.lstTemplates.Items.Clear();
            this.lstTemplates.Items.AddRange(items);
        }

        /// <summary>
        /// Funzione per la creazione del testo da visualizzare nella descrizione del campo nel
        /// datagrid con la lista dei campi
        /// </summary>
        /// <param name="field">Il campo per cui restituire il testo</param>
        protected String GetText(Field field)
        {
            return String.Format("{0} - {1} ({2})", 
                field.Label,
                field.AssociatedTemplateName,
                field.Visible ? "Visualizzato" : "Nascosto");
        }

        /// <summary>
        /// Funzione per la determinazione del colore da assegnare al testo del campo
        /// </summary>
        /// <param name="field">Il campo per cui restituire il colore del testo</param>
        protected Color GetForeColor(Field field)
        {
            return field.Visible ? Color.Black : Color.Gray;

        }

        /// <summary>
        /// Funzione per la restituzione dell'id del campo selezionato
        /// </summary>
        /// <param name="field">Camp per ciu restituire l'id</param>
        /// <returns></returns>
        protected String GetFieldId(Field field)
        {
            return field.FieldId;
        }


    }
}
