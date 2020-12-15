using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using System.Drawing;
using System.Data;
using NttDataWA.UIManager;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class GridPersonalization : System.Web.UI.Page
    {
        /// <summary>
        /// Griglia in fase di modifica
        /// </summary>
        private Grid TemporaryGrid
        {
            get
            {
                return (Grid)HttpContext.Current.Session["Griglia"];
            }
            set
            {
                HttpContext.Current.Session["Griglia"] = value;
            }
        }

        private static Grid SelectedGrid
        {
            get
            {

                return (Grid)HttpContext.Current.Session["SelectedGrid"];

            }

            set
            {
                HttpContext.Current.Session["SelectedGrid"] = value;
            }
        }

        private bool ProfilazioneDinamicaDoc
        {
            get
            {

                return (bool)HttpContext.Current.Session["ProfilazioneDinamicaDoc"];

            }

            set
            {
                HttpContext.Current.Session["ProfilazioneDinamicaDoc"] = value;
            }
        }

        private bool ProfilazioneDinamicaFasc
        {
            get
            {

                return (bool)HttpContext.Current.Session["ProfilazioneDinamicaFasc"];

            }

            set
            {
                HttpContext.Current.Session["ProfilazioneDinamicaFasc"] = value;
            }
        }

        private string ReturnValue
        {
            get
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["ReturnValuePopup"].ToString()))
                    return HttpContext.Current.Session["ReturnValuePopup"].ToString();
                else
                    return string.Empty;
            }
        }

        // INTEGRAZIONE PITRE-PARER
        // Se true, è attivo l'invio in conservazione al sistema SACER
        // Se false, è attivo l'invio in conservazione al Centro Servizi
        private bool IsConservazioneSACER
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["isConservazioneSACER"] != null)
                {
                    return (bool)HttpContext.Current.Session["isConservazioneSACER"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["isConservazioneSACER"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    this.InitializePage();
                }
                else
                {
                    if (!string.IsNullOrEmpty(grid_rowindex.Value) &&
                      int.Parse(grid_rowindex.Value) > -1)
                    {
                        GridPersonalizzata_SelectedIndexChanging(new object(), new GridViewSelectEventArgs(int.Parse(grid_rowindex.Value)));
                        grid_rowindex.Value = "-1";
                    }

                    if (!string.IsNullOrEmpty(gridTemplate_index.Value) &&
                      int.Parse(gridTemplate_index.Value) > -1)
                    {
                        gridTemplates_SelectedIndexChanging(new object(), new GridViewSelectEventArgs(int.Parse(gridTemplate_index.Value)));
                        gridTemplate_index.Value = "-1";
                    }

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
                }
                this.upButtons.Update();
                this.UpnlGrigliaTemplate.Update();
                this.UpnlGrigliaPersonalizzata.Update();

                this.upPnlButtons.Update();

                this.RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        private void InitializePage()
        {
            this.InitializaKey();
            this.InitializeLabel();
            this.InitializeValue();

        }

        private void InitializaKey()
        {
            this.ProfilazioneDinamicaDoc = false;
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()]) &&
                System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()].Equals("1"))
            {
                this.ProfilazioneDinamicaDoc = true;
            }
            this.ProfilazioneDinamicaFasc = false;
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamicaFasc.ToString()]) &&
                System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamicaFasc.ToString()].Equals("1"))
            {
                ProfilazioneDinamicaFasc = true;
            }
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_WA_CONSERVAZIONE.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_WA_CONSERVAZIONE.ToString()).Equals("1"))
            {
                this.IsConservazioneSACER = true;
        }
        }

        private void InitializeValue()
        {
            this.btn_up_field.Enabled = false;
            this.btn_down_field.Enabled = false;
            this.InitData();
        }

        private void InitializeLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.lblOrdina.Text = Utils.Languages.GetLabelFromCode("lblOrdina", language);
            this.lblTitle.Text = Utils.Languages.GetLabelFromCode("lblTitle", language);
            this.lblEtichetta.Text = Utils.Languages.GetLabelFromCode("lblEtichetta", language);
            this.lblLarghezza.Text = Utils.Languages.GetLabelFromCode("lblLarghezza", language);
            this.Lblfieldview.Text = Utils.Languages.GetLabelFromCode("Lblfieldview", language);
            this.lbl_type_template.Text = Utils.Languages.GetLabelFromCode("lbl_type_template", language);
            this.GridPersonalizationBtnClose.Text = Utils.Languages.GetLabelFromCode("GridPersonalizationClose", language);
            this.GridPersonalizationBtnInserisci.Text = Utils.Languages.GetLabelFromCode("GridPersonalizationInsert", language);
            this.btn_up_field.ToolTip = Utils.Languages.GetLabelFromCode("btn_up_field", language);
            this.btn_up_field.AlternateText = Utils.Languages.GetLabelFromCode("btn_up_field", language);
            this.btn_down_field.ToolTip = Utils.Languages.GetLabelFromCode("btn_down_field", language);
            this.btn_down_field.AlternateText = Utils.Languages.GetLabelFromCode("btn_down_field", language);
        }


        private void loadGrigliaField()
        {
            InfoUtente infoutente = UserManager.GetInfoUser();
            Ruolo ruolo = RoleManager.GetRoleInSession();
            string language = UIManager.UserManager.GetUserLanguage();
            if (SelectedGrid != null)
            {
                this.TemporaryGrid = GridManager.CloneGrid(SelectedGrid);
            }
            else
            {
                if (this.TemporaryGrid != null)

                    this.TemporaryGrid = GridManager.getUserGrid(this.TemporaryGrid.GridType, infoutente, ruolo);
                else
                    this.TemporaryGrid = GridManager.getUserGrid(GridTypeEnumeration.Document, infoutente, ruolo);

                SelectedGrid = this.TemporaryGrid;
            }
            this.GridPersonalizzata.DataSource = this.TemporaryGrid.Fields;
            this.GridPersonalizzata.DataBind();
            GridPersonalizzata.SelectedIndex = -1;
            infoutente = null;
            ruolo = null;
        }

        protected void InitData()
        {
            this.loadGrigliaField();

            if ((SelectedGrid.GridType == GridTypeEnumeration.Document && this.ProfilazioneDinamicaDoc) || (SelectedGrid.GridType == GridTypeEnumeration.DocumentInProject && this.ProfilazioneDinamicaDoc) ||
                (SelectedGrid.GridType == GridTypeEnumeration.Project &&
                ProfilazioneDinamicaFasc))
            {
                this.LoadTemplates(this.TemporaryGrid.GridType);
            }
            else
            {
                this.lbl_type_template.Visible = false;
            }

            bool IsRoleEnabledToUseGrids = UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION");
            if (SelectedGrid.GridType == GridTypeEnumeration.Project)
                GridManager.CompileDdlOrderAndSetOrderFilterProjects(this.TemporaryGrid, this.ddlOrder, this.ddlAscDesc, IsRoleEnabledToUseGrids);
            else
                GridManager.CompileDdlOrderAndSetOrderFilterDocuments(this.TemporaryGrid, this.ddlOrder, this.ddlAscDesc, IsRoleEnabledToUseGrids);
            string language = UIManager.UserManager.GetUserLanguage();
            this.GridPersonalizzata.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("GridPersonalizzaraHeader1", language);
            if (this.lbl_type_template.Visible)
            {
                if (gridTemplates != null && gridTemplates.HeaderRow != null)
                {
                    this.lbl_type_template.Visible = false;
                    gridTemplates.HeaderRow.Cells[0].Text = Utils.Languages.GetLabelFromCode("gridTemplatesHeader0", language);
                    gridTemplates.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("gridTemplatesHeader1", language);
                }
            }
        }

        protected void UpField(object sender, EventArgs e)
        {
            //try
            //{
                int position = Convert.ToInt32(this.selectedFieldPosition.Value);
                Field up = this.TemporaryGrid.Fields[position];
                Field down = this.TemporaryGrid.Fields[position - 1];
                this.TemporaryGrid.Fields[position] = down;
                this.TemporaryGrid.Fields[position - 1] = up;
                this.TemporaryGrid.Fields[position].Position = position;
                this.TemporaryGrid.Fields[position - 1].Position = position - 1;
                this.GridPersonalizzata.DataSource = this.TemporaryGrid.Fields;
                this.GridPersonalizzata.DataBind();

                this.selectedFieldPosition.Value = (position - 1).ToString();
                if ((position - 1) == 0)
                    this.btn_up_field.Enabled = false;

                this.btn_down_field.Enabled = true;
                this.upButtons.Update();
                UpnlGrigliaPersonalizzata.Update();
                UpnlGrigliaTemplate.Update();
                GridPersonalizzata.SelectedIndex = position - 1;
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        protected void DownField(object sender, EventArgs e)
        {
            //try
            //{
                int position = Convert.ToInt32(this.selectedFieldPosition.Value);
                Field up = this.TemporaryGrid.Fields[position];
                Field down = this.TemporaryGrid.Fields[position + 1];
                this.TemporaryGrid.Fields[position] = down;
                this.TemporaryGrid.Fields[position + 1] = up;
                this.TemporaryGrid.Fields[position].Position = position;
                this.TemporaryGrid.Fields[position + 1].Position = position + 1;
                this.GridPersonalizzata.DataSource = this.TemporaryGrid.Fields;
                this.GridPersonalizzata.DataBind();

                this.selectedFieldPosition.Value = (position + 1).ToString();
                if ((GridPersonalizzata.Rows.Count - 1) == (position + 1))
                    this.btn_down_field.Enabled = false;

                this.btn_up_field.Enabled = true;
                this.upButtons.Update();
                UpnlGrigliaPersonalizzata.Update();
                UpnlGrigliaTemplate.Update();
                GridPersonalizzata.SelectedIndex = position + 1;
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        protected String GetText(Field field)
        {
            string result = field.Label;
            if (!String.IsNullOrEmpty(field.AssociatedTemplateName))
            {
                if(!(field.AssociatedTemplateName.ToUpper() == "CAMPI COMUNI"))
                    result += " - " + field.AssociatedTemplateName;
                else
                {
                    string language = UIManager.UserManager.GetUserLanguage();
                    if (language.ToUpper() != "ITALIAN")
                        result += " - " + Utils.Languages.GetLabelFromCode("BaseMasterLblSearchCommonFields", language);
                }
            }
            return result;
        }

        protected bool GetChecked(Field field)
        {
            return field.Visible;
        }

        protected bool GetEnable(Field field)
        {
            bool result = true;
            if (field.Locked)
            {
                result = false;
            }
            return result;
        }

        protected String GetFieldID(Field field)
        {
            return field.FieldId;
        }

        protected String GetNameTemplate(Templates temp)
        {
            return temp.DESCRIZIONE;
        }

        protected String GetTemplateID(Templates temp)
        {
            return (temp.SYSTEM_ID).ToString();
        }

        protected bool GetTemplateVisible(Templates temp)
        {
            bool result = false;
            if (this.TemporaryGrid.TemplatesId != null && this.TemporaryGrid.TemplatesId.Length > 0)
            {
                if (this.TemporaryGrid.TemplatesId.Contains((temp.SYSTEM_ID).ToString()))
                {
                    result = true;
                }
            }
            return result;
        }

        protected void EnabledButtunUpDown(GridViewRow dgItem)
        {
            this.btn_up_field.Enabled = true;
            if (dgItem.RowIndex == 0)
                this.btn_up_field.Enabled = false;

            this.btn_down_field.Enabled = true;
            if (dgItem.RowIndex == this.GridPersonalizzata.Rows.Count - 1)
                this.btn_down_field.Enabled = false;

            this.upButtons.Update();
        }


        protected void gridTemplates_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            //try
            //{
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                hfFieldId.Value = ((Label)this.gridTemplates.Rows[e.NewSelectedIndex].FindControl("SYSTEM_ID_TEMPLATE")).Text;
                if (((CheckBox)this.gridTemplates.Rows[e.NewSelectedIndex].FindControl("chkSelectedTemplate")).Checked)
                {
                    addTemplateFields(hfFieldId.Value);
                }
                else
                    removeTemplateFields(hfFieldId.Value);
                this.gridTemplate_index.Value = "-1";
                this.btn_up_field.Enabled = false;
                this.btn_down_field.Enabled = false;
                this.hfFieldId.Value = string.Empty;
                this.upButtons.Update();
                gridTemplates.SelectedIndex = e.NewSelectedIndex;
                this.GridPersonalizzata.DataSource = this.TemporaryGrid.Fields;
                this.GridPersonalizzata.DataBind();
                //ViewDettaglioFiled(string.Empty, this.TemporaryGrid.Fields[e.NewSelectedIndex].Locked);
                UpnlGrigliaTemplate.Update();
                UpnlGrigliaPersonalizzata.Update();
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        protected void addTemplateFields(string system_id)
        {
            // Dettagli del template da aggiungere
            Templates template;

            // Lista dei diritti del ruolo suoi campi
            AssDocFascRuoli[] right;

            // Lista dei campi presenti nella griglia
            List<Field> fields;


            // Creazione della lista dei campi presenti nella griglia
            fields = new List<Field>(this.TemporaryGrid.Fields);
            InfoUtente infoutente = UserManager.GetInfoUser();
            string idGruppo = RoleManager.GetRoleInSession().idGruppo;
            // Caricamento del template
            if (this.TemporaryGrid.GridType == GridTypeEnumeration.Project)
            {
                template = ProjectManager.getTemplateFascById(system_id, infoutente);
                right = ProjectManager.getDirittiCampiTipologiaFasc(idGruppo, system_id);
            }
            else
            {
                template = DocumentManager.getTemplateById(system_id, infoutente);
                right = DocumentManager.getDirittiCampiTipologiaDoc(idGruppo, system_id);
            }

            // Ricalcolo degli indici posizionali dei campi
            int nextPosition = 0;
            for (nextPosition = 0; nextPosition < fields.Count; nextPosition++)
                fields[nextPosition].Position = nextPosition;

            foreach (OggettoCustom obj in template.ELENCO_OGGETTI)
            {
                AssDocFascRuoli ro = right.Where(o => o.ID_OGGETTO_CUSTOM == obj.SYSTEM_ID.ToString() && o.VIS_OGG_CUSTOM == "1").FirstOrDefault();
                bool number = false;
                // Se la griglia non contiene già un campo con system id pari a quello
                // che si sta tentando di inserire, viene inserito
                if (fields.Where(f => f.CustomObjectId == obj.SYSTEM_ID).Count() == 0 && ro != null)
                {

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                    {
                        number = true;
                    }

                    Field field = new Field()
                    {
                        FieldId = String.Format("T{0}", obj.SYSTEM_ID),
                        Label = obj.DESCRIZIONE,
                        OriginalLabel = obj.DESCRIZIONE,
                        Visible = false,
                        CanAssumeMultiValues = obj.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("CASELLADISELEZIONE"),
                        IsTruncable = true,
                        CustomObjectId = obj.SYSTEM_ID,
                        AssociatedTemplateName = template.DESCRIZIONE,
                        AssociatedTemplateId = template.SYSTEM_ID.ToString(),
                        //MaxLength = 100,
                        // Default a 100
                        MaxLength = -1,
                        Width = 100,
                        Position = nextPosition++,
                        OracleDbColumnName = obj.SYSTEM_ID.ToString(),
                        SqlServerDbColumnName = obj.SYSTEM_ID.ToString(),
                        IsCommonField = !String.IsNullOrEmpty(obj.CAMPO_COMUNE) && obj.CAMPO_COMUNE == "1",
                        IsNumber = number
                    };

                    if (obj.ELENCO_VALORI != null && obj.ELENCO_VALORI.Count() > 0)
                    {
                        List<FieldValue> fieldValues = new List<FieldValue>();
                        foreach (ValoreOggetto o in obj.ELENCO_VALORI)
                            fieldValues.Add(new FieldValue() { Value = o.VALORE, ColorBG = o.COLOR_BG });
                        field.Values = fieldValues.ToArray();
                    }

                    fields.Add(field);
                }
            }

            // Impostazione della nuova lista dei campi per la griglia attuale
            this.TemporaryGrid.Fields = fields.ToArray();
            List<String> idTemplates = new List<String>();
            if (this.TemporaryGrid.TemplatesId != null)
                idTemplates.AddRange(this.TemporaryGrid.TemplatesId);
            idTemplates.Add(template.SYSTEM_ID.ToString());
            this.TemporaryGrid.TemplatesId = idTemplates.ToArray();

            //this.box_fields.Update();
        }

        protected void removeTemplateFields(string system_id)
        {
            // Lista dei campi definiti per la griglia attuale
            List<Field> fields;

            // Template da rimuovere
            Templates template;
            InfoUtente infoutente = UserManager.GetInfoUser();

            // Caricamento del template
            if (this.TemporaryGrid.GridType.Equals(GridTypeEnumeration.Project))
                template = ProjectManager.getTemplateFascById(system_id, infoutente);
            else
                template = DocumentManager.getTemplateById(system_id, infoutente);
            // Inizializzazione della lista dei campi del template
            fields = new List<Field>(this.TemporaryGrid.Fields);

            // Rimozione dalla lista dei campi, dei campi presenti nel template
            foreach (OggettoCustom obj in template.ELENCO_OGGETTI)
            {
                Field tempField = fields.Where(f => f.CustomObjectId.Equals(obj.SYSTEM_ID) && f.AssociatedTemplateId != null && f.AssociatedTemplateId.Equals(template.SYSTEM_ID.ToString())).FirstOrDefault();
                if (tempField != null)
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

        /// <summary>
        /// Al clicm su questo pulsante vengono salvate le impostazioni per la griglia corrente
        /// </summary>
        protected void GridPersonalizationBtnInserisci_Click(object sender, EventArgs e)
        {
            //try
            //{
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                SelectedGrid = this.TemporaryGrid;

                SelectedGrid.FieldForOrder = SelectedGrid.Fields.Where(
                    f => f.FieldId.Equals(this.ddlOrder.SelectedItem.Value)).FirstOrDefault();

                // Impostazione della direzione di ordinamento
                if (this.ddlAscDesc.SelectedValue.ToUpper() == "ASC")
                    SelectedGrid.OrderDirection = OrderDirectionEnum.Asc;
                else
                    SelectedGrid.OrderDirection = OrderDirectionEnum.Desc;


                SelectedGrid.GridId = string.Empty;
                this.TemporaryGrid = null;
                UpnlGrigliaPersonalizzata.Update();
                upButtons.Update();
                upPnlButtons.Update();
                UpnlGrigliaTemplate.Update();
                close("up");
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        protected void ddlWidth_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SaveCurrentFieldProperties();
        }

        /// <summary>
        /// Questa funzione salva le informazioni relative al campo attualmente visualizzato
        /// </summary>
        private void SaveCurrentFieldProperties()
        {
            //// Il campo di cui salvare le proprietà
            //Field field = this.TemporaryGrid.Fields.Where(
            //    f => f.FieldId == this.hfFieldId.Value).FirstOrDefault();

            //if (field != null)
            //{
            //    int width;
            //    Int32.TryParse(this.ddlWidth.SelectedItem.Value, out width);
            //    field.Width = width;
            //    if (!string.IsNullOrEmpty(this.txtLabel.Text) &&
            //        !string.IsNullOrWhiteSpace(txtLabel.Text))
            //        field.Label = this.txtLabel.Text;
            //    field.Visible = true;
            //    TemporaryGrid.Fields[field.Position] = field;
            //}


            // Il campo di cui salvare le proprietà
            Field field;

            // Recupero delle proprietà del campo selezionato
            field = this.TemporaryGrid.Fields.Where(
                f => f.FieldId == this.hfFieldId.Value).FirstOrDefault();

            // Aggiornamento dei dati sul campo
            int maxLength, width;
            Int32.TryParse(this.ddlWidth.SelectedItem.Value, out maxLength);
            Int32.TryParse(this.ddlWidth.SelectedItem.Value, out width);
            // Eliminazione default a 100
            //field.MaxLength = maxLength > 0 ? maxLength : -1;
            field.MaxLength = maxLength > 0 ? maxLength : 100;
            field.Width = width;
            field.Visible = true;
            if (!String.IsNullOrEmpty(this.txtLabel.Text))
                field.Label = this.txtLabel.Text;


        }


        /// <summary>
        /// Questa funzione salva le informazioni relative al campo attualmente visualizzato
        /// </summary>
        private void RemoveCurrentFieldProperties()
        {
            // Il campo di cui salvare le proprietà
            Field field = this.TemporaryGrid.Fields.Where(
                f => f.FieldId == this.hfFieldId.Value).FirstOrDefault();

            if (field != null)
            {
                field.Visible = false;
                TemporaryGrid.Fields[field.Position] = field;
            }

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
                case GridTypeEnumeration.DocumentInProject:
                    this.LoadDocumentTemplates();
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
            List<Templates> templates = new List<Templates>();
            this.lbl_type_template.Text = "Aggiungi i campi delle tipologie di documento";
            string language = UIManager.UserManager.GetUserLanguage();

            // Caricamento dei tipi atto
            TipologiaAtto[] tempAtto = DocumentManager.getTipoAttoPDInsRic(
                this,
                UserManager.GetInfoUser().idAmministrazione,
                RoleManager.GetRoleInSession().idGruppo,
                "1");

            foreach (TipologiaAtto ta in tempAtto)
            {
                Templates tempT = new Templates();
                tempT.DESCRIZIONE = ta.descrizione;
                tempT.SYSTEM_ID = Convert.ToInt32(ta.systemId);
                if (language != null && language.ToUpper() != "ITALIAN")
                {
                    if(tempT.DESCRIZIONE.ToUpper() == "CAMPI COMUNI")
                        tempT.DESCRIZIONE = Utils.Languages.GetLabelFromCode("BaseMasterLblSearchCommonFields", language);
                }

                templates.Add(tempT);
            }

            this.gridTemplates.DataSource = templates.ToArray();
            this.gridTemplates.DataBind();
        }

        /// <summary>
        /// Funzione per il caricamento dei template relativi ai fascicoli
        /// </summary>
        private void LoadProjectTemplates()
        {
            this.lbl_type_template.Text = "Aggiungi i campi delle tipologie di fascicolo";

            // Caricamento dei tipi atto
            this.gridTemplates.DataSource = ProjectManager.getTipoFascFromRuolo(
                 UserManager.GetInfoUser().idAmministrazione,
                 RoleManager.GetRoleInSession().idGruppo,
                 "1");

            this.gridTemplates.DataBind();
        }

        protected void ChangeFieldOrder(object sender, EventArgs e)
        {
            //try
            //{
                Field d = (Field)TemporaryGrid.Fields.Where(f => f.FieldId.Equals(this.ddlOrder.SelectedValue)).FirstOrDefault();
                if (d != null)
                    this.TemporaryGrid.FieldForOrder = d;
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        protected void ChangeAscDescOrder(object sender, EventArgs e)
        {
            //try
            //{
                this.TemporaryGrid.OrderDirection = OrderDirectionEnum.Desc;
                if (ddlAscDesc.SelectedValue.Equals("ASC"))
                    this.TemporaryGrid.OrderDirection = OrderDirectionEnum.Asc;
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        protected void GridPersonalizationBtnClose_Click(object sender, EventArgs e)
        {
            //try
            //{
                close(string.Empty);
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        private void close(string parametroChiusura)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('GrigliaPersonalizzata', '" + parametroChiusura + "');} else {parent.closeAjaxModal('GrigliaPersonalizzata', '" + parametroChiusura + "');};", true);
        }

        protected void GridPersonalizzata_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //try
            //{
                if (e.Row.RowType == DataControlRowType.DataRow &&
                  (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate))
                {
                    if (TemporaryGrid.Fields[e.Row.RowIndex].FieldId == "C2")
                    {
                        e.Row.Visible = false;
                    }
                    else
                    {
                        CheckBox chkBxSelect = (CheckBox)e.Row.Cells[0].FindControl("checkDocumento");
                        CheckBox chkBxHeader = (CheckBox)this.GridPersonalizzata.HeaderRow.FindControl("cb_selectall");
                        chkBxSelect.Attributes["onclick"] = string.Format("javascript:ChildClick(this,'{0}');", chkBxHeader.ClientID);
                    }

                    // Visibilità campi conservazione
                    if (!this.IsConservazioneSACER)
                    {
                        if(
                            TemporaryGrid.Fields[e.Row.RowIndex].FieldId == "CODICE_POLICY" ||
                            TemporaryGrid.Fields[e.Row.RowIndex].FieldId == "CONTATORE_POLICY" ||
                            TemporaryGrid.Fields[e.Row.RowIndex].FieldId == "DATA_ESECUZIONE_POLICY"
                          )
                        {
                            e.Row.Visible = false;
                        }
                    }
                }
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        protected void GridPersonalizzata_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //try
            //{
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes["onclick"] = "disallowOp('Content2'); $('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "'); __doPostBack('UpnlGrigliaPersonalizzata', ''); return false;";
                }
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        protected void gridTemplates_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes["onclick"] = "disallowOp('Content2'); $('#gridTemplate_index').val('" + e.Row.RowIndex.ToString() + "'); __doPostBack('UpnlGrigliaTemplate', ''); return false;";
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GridPersonalizzata_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                GridPersonalizzata.PageIndex = e.NewPageIndex;
                grid_rowindex.Value = "-1";
                loadGrigliaField();
                upButtons.Update();
                UpPnlDettaglio.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GridPersonalizzata_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            //try
            //{
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                this.ddlWidth.SelectedValue = (TemporaryGrid.Fields[e.NewSelectedIndex].Width).ToString();
                this.selectedFieldPosition.Value = (e.NewSelectedIndex).ToString();
                Label systemIdLabel = (Label)this.GridPersonalizzata.Rows[e.NewSelectedIndex].FindControl("SYSTEM_ID");
                this.hfFieldId.Value = systemIdLabel.Text;
                EnabledButtunUpDown(GridPersonalizzata.Rows[e.NewSelectedIndex]);
                GridPersonalizzata.SelectedIndex = e.NewSelectedIndex;

                if ((((CheckBox)GridPersonalizzata.Rows[e.NewSelectedIndex].FindControl("checkDocumento")).Checked))
                {
                    ViewDettaglioFiled(TemporaryGrid.Fields[e.NewSelectedIndex].Label, TemporaryGrid.Fields[e.NewSelectedIndex]);
                    SaveCurrentFieldProperties();
                }
                else
                {
                    ViewDettaglioFiled(string.Empty, null);
                    RemoveCurrentFieldProperties();
                }

                bool IsRoleEnabledToUseGrids = UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION");
                if (SelectedGrid.GridType == GridTypeEnumeration.Project)
                    GridManager.CompileDdlOrderAndSetOrderFilterProjects(this.TemporaryGrid, this.ddlOrder, this.ddlAscDesc, IsRoleEnabledToUseGrids);
                else
                    GridManager.CompileDdlOrderAndSetOrderFilterDocuments(this.TemporaryGrid, this.ddlOrder, this.ddlAscDesc, IsRoleEnabledToUseGrids);


                UpnlGrigliaPersonalizzata.Update();
                UpnlGrigliaTemplate.Update();
                UpPnlDettaglio.Update();
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        private void ViewDettaglioFiled(string label, Field field)
        {
            this.txtLabel.Text = label;
            if (field != null)
            {
                this.txtLabel.Enabled = !field.Locked;
            }
            else
            {
                this.txtLabel.Enabled = true;
            }
            if (field != null && field.FieldId.Equals("C1"))
            {
                this.ddlWidth.Enabled = true;
            }
            else
            {
                if (field != null)
                {
                    this.ddlWidth.Enabled = !field.Locked;
                }
                else
                {
                    this.ddlWidth.Enabled = true;
                }
            }
        }

    }
}