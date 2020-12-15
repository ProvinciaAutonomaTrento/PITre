using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;
using System.Drawing;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.Grids
{
    public partial class NewGridPersonalization : CssPage
    {
        protected InfoUtente infoUtente;

        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);

            this.infoUtente = UserManager.getInfoUtente(this);

            if (!IsPostBack)
            {
                InitButton();
                InitData();
                SetTema();
                string result = string.Empty;
                if (Request.QueryString["tabRes"] != string.Empty && Request.QueryString["tabRes"] != null)
                {
                    result = Request.QueryString["tabRes"].ToString();
                }
                this.hid_tab_est.Value = result;
            }
        }

        public void InitButton()
        {
            this.btn_up_field.Enabled = false;
            this.btn_down_field.Enabled = false;
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
                this.tr1.BgColor = colorsplit[2];
                this.lblTitle.ForeColor = System.Drawing.Color.White;
                System.Drawing.ColorConverter colConvert = new ColorConverter();
                this.gridField.HeaderStyle.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#" + colorsplit[2]);
                this.gridField.HeaderStyle.ForeColor = System.Drawing.Color.White;
                this.gridTemplates.HeaderStyle.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#" + colorsplit[2]);
                this.gridTemplates.HeaderStyle.ForeColor = System.Drawing.Color.White;
                this.btn_up_field.ImageUrl = "../App_Themes/" + colorsplit[0] + "/up_grid.gif";
                this.btn_down_field.ImageUrl = "../App_Themes/" + colorsplit[0] + "/down_grid.gif";

                this.btn_up_field.Attributes.Add("onmouseover", "this.src='" + "../App_Themes/" + colorsplit[0] + "/up_grid_selected.gif'");
                this.btn_up_field.Attributes.Add("onmouseout", "this.src='" + "../App_Themes/" + colorsplit[0] + "/up_grid.gif'");
                this.btn_down_field.Attributes.Add("onmouseover", "this.src='" + "../App_Themes/" + colorsplit[0] + "/down_grid_selected.gif'");
                this.btn_down_field.Attributes.Add("onmouseout", "this.src='" + "../App_Themes/" + colorsplit[0] + "/down_grid.gif'");
            }
            else
            {
                this.tr1.BgColor = "810d06";
                System.Drawing.ColorConverter colConvert = new ColorConverter();
                this.gridField.HeaderStyle.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#810d06");
                this.gridField.HeaderStyle.ForeColor = System.Drawing.Color.White;
                this.gridTemplates.HeaderStyle.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#810d06");
                this.gridTemplates.HeaderStyle.ForeColor = System.Drawing.Color.White;
                this.btn_up_field.ImageUrl = "../App_Themes/TemaRosso" + "/up_grid.gif";
                this.btn_down_field.ImageUrl = "../App_Themes/TemaRosso" + "/down_grid.gif";
                this.btn_up_field.Attributes.Add("onmouseover", "this.src='../App_Themes/TemaRosso/up_grid_selected.gif'");
                this.btn_up_field.Attributes.Add("onmouseout", "this.src='../App_Themes/TemaRosso/up_grid.gif'");
                this.btn_down_field.Attributes.Add("onmouseover", "this.src='../App_Themes/TemaRosso/down_grid_selected.gif'");
                this.btn_down_field.Attributes.Add("onmouseout", "this.src='../App_Themes/TemaRosso/down_grid.gif'");
            }
        }

        protected void ChangeCheckClick(object sender, EventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            TableCell cell = (TableCell)box.Parent;
            DataGridItem dgItem = (DataGridItem)cell.Parent;
            if (this.selectedFieldPosition != null && !String.IsNullOrEmpty(this.selectedFieldPosition.Value) && Convert.ToInt32(this.selectedFieldPosition.Value) != dgItem.ItemIndex)
            {
                this.gridField.Items[Convert.ToInt32(this.selectedFieldPosition.Value)].Attributes.Add("onmouseout", "this.className='RowOverFirst';");
                this.gridField.Items[Convert.ToInt32(this.selectedFieldPosition.Value)].CssClass = "no_selected_check";
                if (!string.IsNullOrEmpty(this.hfFieldId.Value))
                {
                    SaveCurrentFieldProperties();
                }
            }
            bool locked = TemporaryGrid.Fields[dgItem.ItemIndex].Locked;
            if (!locked)
            {
                this.txtLabel.Text = TemporaryGrid.Fields[dgItem.ItemIndex].Label;
                this.txtLabel.Visible = true;
                this.lblEtichetta.Visible = true;
                this.lblLarghezza.Visible = true;
                this.ddlWidth.Visible = true;
                this.ddlWidth.SelectedValue = (TemporaryGrid.Fields[dgItem.ItemIndex].Width).ToString();
            }
            else
            {
                this.txtLabel.Text = string.Empty;
                this.txtLabel.Visible = false;
                this.lblEtichetta.Visible = false;
                this.lblLarghezza.Visible = false;
                this.ddlWidth.Visible = false;
            }
            this.gridField.Items[dgItem.ItemIndex].Attributes.Remove("onmouseout");
            this.gridField.Items[dgItem.ItemIndex].CssClass = "selected_check";
            EnabledButtunUpDown(dgItem);
            this.selectedFieldPosition.Value = (dgItem.ItemIndex).ToString();
            TemporaryGrid.Fields[dgItem.ItemIndex].Visible = box.Checked;
            Label systemIdLabel = (Label)this.gridField.Items[dgItem.ItemIndex].FindControl("SYSTEM_ID");
            this.hfFieldId.Value = systemIdLabel.Text;
            this.value_fields.Update();
            if (box.Checked)
            {
                if (this.TemporaryGrid.Fields.Where(g => g.Visible == true).Count() == this.TemporaryGrid.Fields.Length)
                {
                    this.checkAll.Text = "Deseleziona tutti";
                    this.checkAll.Checked = true;
                    this.chkAllPanel.Update();
                }
            }
            else
            {
                if (this.TemporaryGrid.Fields.Where(g => g.Visible == false).Count() < this.TemporaryGrid.Fields.Length)
                {
                    this.checkAll.Text = "Seleziona tutti";
                    this.checkAll.Checked = false;
                    this.chkAllPanel.Update();
                }
            }
            this.UpdateGraphic(this.TemporaryGrid);
        }

        protected void SelectRowField(object sender, EventArgs e)
        {
            LinkButton link = (LinkButton)sender;
            TableCell cell = (TableCell)link.Parent;
            DataGridItem dgItem = (DataGridItem)cell.Parent;
            if (this.selectedFieldPosition != null && !String.IsNullOrEmpty(this.selectedFieldPosition.Value) && Convert.ToInt32(this.selectedFieldPosition.Value) != dgItem.ItemIndex)
            {
                this.gridField.Items[Convert.ToInt32(this.selectedFieldPosition.Value)].Attributes.Add("onmouseout", "this.className='RowOverFirst';");
                this.gridField.Items[Convert.ToInt32(this.selectedFieldPosition.Value)].CssClass = "no_selected_check";
                if (!string.IsNullOrEmpty(this.hfFieldId.Value))
                {
                    SaveCurrentFieldProperties();
                }
            }
            bool locked = TemporaryGrid.Fields[dgItem.ItemIndex].Locked;
            if (!locked)
            {
                this.txtLabel.Text = TemporaryGrid.Fields[dgItem.ItemIndex].Label;
                this.txtLabel.Visible = true;
                this.lblEtichetta.Visible = true;
                this.lblLarghezza.Visible = true;
                this.ddlWidth.Visible = true;
            }
            else
            {
                this.txtLabel.Text = string.Empty;
                this.txtLabel.Visible = false;
                this.lblEtichetta.Visible = false;
                this.lblLarghezza.Visible = false;
                this.ddlWidth.Visible = false;
            }
            this.ddlWidth.SelectedValue = (TemporaryGrid.Fields[dgItem.ItemIndex].Width).ToString();
            this.gridField.Items[dgItem.ItemIndex].Attributes.Remove("onmouseout");
            this.gridField.Items[dgItem.ItemIndex].CssClass = "selected_check";
            EnabledButtunUpDown(dgItem);
            this.selectedFieldPosition.Value = (dgItem.ItemIndex).ToString();
            Label systemIdLabel = (Label)this.gridField.Items[dgItem.ItemIndex].FindControl("SYSTEM_ID");
            this.hfFieldId.Value = systemIdLabel.Text;
            this.value_fields.Update();
        }

        protected void InitData()
        {
            if (GridManager.SelectedGrid != null)
            {
                this.TemporaryGrid = GridManager.CloneGrid(GridManager.SelectedGrid);
            }
            else
            {
                if (this.TemporaryGrid != null)
                {

                    this.TemporaryGrid = GridManager.getUserGrid(this.TemporaryGrid.GridType);
                }
                else
                {
                    this.TemporaryGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);
                }
                GridManager.SelectedGrid = this.TemporaryGrid;
            }
            this.gridField.DataSource = this.TemporaryGrid.Fields;
            this.gridField.DataBind();

            if ((GridManager.SelectedGrid.GridType == GridTypeEnumeration.Project && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1" && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null) || (GridManager.SelectedGrid.GridType != GridTypeEnumeration.Project && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1" && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null))
            {

                LoadTemplates(this.TemporaryGrid.GridType);
            }
            else
            {
                lbl_type_template.Visible = false;
                this.box_type_document.Visible = false;
            }
            
            if (GridManager.SelectedGrid.GridType == GridTypeEnumeration.Project)
            {
                GridManager.CompileDdlOrderAndSetOrderFilterProjects(this.TemporaryGrid, this.ddlOrder, this.ddlAscDesc);
               
            }
            else
            {
                GridManager.CompileDdlOrderAndSetOrderFilterDocuments(this.TemporaryGrid, this.ddlOrder, this.ddlAscDesc);
            }
            this.upOrder.Update();

            if (this.TemporaryGrid.Fields.Where(g => g.Visible == true).Count() == this.TemporaryGrid.Fields.Length)
            {
                this.checkAll.Text = "Deseleziona tutti";
                this.checkAll.Checked = true;
                this.chkAllPanel.Update();
            }
            else
            {
                if (this.TemporaryGrid.Fields.Where(g => g.Visible == false).Count() < this.TemporaryGrid.Fields.Length)
                {
                    this.checkAll.Text = "Seleziona tutti";
                    this.checkAll.Checked = false;
                    this.chkAllPanel.Update();
                }
            }
        }

        protected void UpField(object sender, EventArgs e)
        {
            int position = Convert.ToInt32(this.selectedFieldPosition.Value);
            Field up = this.TemporaryGrid.Fields[position];
            Field down = this.TemporaryGrid.Fields[position - 1];
            this.TemporaryGrid.Fields[position] = down;
            this.TemporaryGrid.Fields[position - 1] = up;
            this.TemporaryGrid.Fields[position].Position = position;
            this.TemporaryGrid.Fields[position - 1].Position = position - 1;
            this.gridField.DataSource = this.TemporaryGrid.Fields;
            this.gridField.DataBind();
            this.box_fields.Update();
            this.gridField.Items[position - 1].Attributes.Remove("onmouseout");
            this.gridField.Items[position - 1].CssClass = "selected_check";
            this.gridField.Items[position].Attributes.Add("onmouseout", "this.className='RowOverFirst';");
            this.gridField.Items[position].CssClass = "no_selected_check";
            this.selectedFieldPosition.Value = (position - 1).ToString();
            if ((position - 1) == 0)
            {
                this.btn_up_field.Enabled = false;

            }
            this.btn_down_field.Enabled = true;
            this.upButtons.Update();
        }

        protected void DownField(object sender, EventArgs e)
        {
            int position = Convert.ToInt32(this.selectedFieldPosition.Value);
            Field up = this.TemporaryGrid.Fields[position];
            Field down = this.TemporaryGrid.Fields[position + 1];
            this.TemporaryGrid.Fields[position] = down;
            this.TemporaryGrid.Fields[position + 1] = up;
            this.TemporaryGrid.Fields[position].Position = position;
            this.TemporaryGrid.Fields[position + 1].Position = position + 1;
            this.gridField.DataSource = this.TemporaryGrid.Fields;
            this.gridField.DataBind();
            this.box_fields.Update();
            this.gridField.Items[position + 1].Attributes.Remove("onmouseout");
            this.gridField.Items[position + 1].CssClass = "selected_check";
            this.gridField.Items[position].Attributes.Add("onmouseout", "this.className='RowOverFirst';");
            this.gridField.Items[position].CssClass = "no_selected_check";
            this.selectedFieldPosition.Value = (position + 1).ToString();
            if ((gridField.Items.Count - 1) == (position + 1))
            {
                this.btn_down_field.Enabled = false;
            }
            this.btn_up_field.Enabled = true;
            this.upButtons.Update();
        }

        protected String GetText(Field field)
        {
            string result = field.Label;
            if (!String.IsNullOrEmpty(field.AssociatedTemplateName))
            {
                result += " - " + field.AssociatedTemplateName;
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

        protected void DataGridFieldCreated(object sender, DataGridItemEventArgs e)
        {
            if (!(e.Item.ItemType == ListItemType.Pager))
            {
                e.Item.Attributes.Add("onmouseover", "this.className='RowOverSelected';");
                e.Item.Attributes.Add("onmouseout", "this.className='RowOverFirst';");
            }
        }

        protected void EnabledButtunUpDown(DataGridItem dgItem)
        {
            if (dgItem.ItemIndex == 0)
            {
                this.btn_up_field.Enabled = false;
            }
            else
            {
                this.btn_up_field.Enabled = true;
            }
            if (dgItem.ItemIndex == this.gridField.Items.Count - 1)
            {
                this.btn_down_field.Enabled = false;
            }
            else
            {
                this.btn_down_field.Enabled = true;
            }
            this.upButtons.Update();
        }

        protected void ChangeCheckClickTemplate(object sender, EventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            TableCell cell = (TableCell)box.Parent;
            DataGridItem dgItem = (DataGridItem)cell.Parent;
            Label a = (Label)this.gridTemplates.Items[dgItem.ItemIndex].FindControl("SYSTEM_ID_TEMPLATE");
            string system_id = a.Text;

            if (!string.IsNullOrEmpty(this.hfFieldId.Value))
            {
                SaveCurrentFieldProperties();
            }

            if (box.Checked)
            {
                addTemplateFields(system_id);
                this.checkAll.Text = "Seleziona tutti";
                this.checkAll.Checked = false;
                this.chkAllPanel.Update();
            }
            else
            {
                removeTemplateFields(system_id);
            }

            this.selectedFieldPosition.Value = string.Empty;
            this.btn_up_field.Enabled = false;
            this.btn_down_field.Enabled = false;
            this.hfFieldId.Value = string.Empty;
            this.upButtons.Update();

            this.gridField.DataSource = this.TemporaryGrid.Fields;
            this.gridField.DataBind();
            this.box_fields.Update();
            this.txtLabel.Text = string.Empty;
            this.txtLabel.Visible = false;
            this.lblEtichetta.Visible = false;
            this.lblLarghezza.Visible = false;
            this.ddlWidth.Visible = false;
            this.value_fields.Update();

            if (this.TemporaryGrid.Fields.Where(g => g.Visible == true).Count() == this.TemporaryGrid.Fields.Length)
            {
                this.checkAll.Text = "Deseleziona tutti";
                this.checkAll.Checked = true;
                this.chkAllPanel.Update();
            }

            this.mdlPopupWait.Hide();

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

            // Caricamento del template
            if (this.TemporaryGrid.GridType == GridTypeEnumeration.Project)
            {
                template = ProfilazioneFascManager.getTemplateFascById(system_id, this);
                right = (AssDocFascRuoli[])ProfilazioneFascManager.getDirittiCampiTipologiaFasc(UserManager.getRuolo().idGruppo, system_id, this).ToArray(typeof(AssDocFascRuoli));
            }
            else
            {
                template = ProfilazioneDocManager.getTemplateById(system_id, this);
                right = (AssDocFascRuoli[])ProfilazioneDocManager.getDirittiCampiTipologiaDoc(UserManager.getRuolo().idGruppo, system_id, this).ToArray(typeof(AssDocFascRuoli));
               
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
                    });
                }
            }

            // Impostazione della nuova lista dei campi per la griglia attuale
            this.TemporaryGrid.Fields = fields.ToArray();
            List<String> idTemplates = new List<String>();
            if (this.TemporaryGrid.TemplatesId != null)
                idTemplates.AddRange(this.TemporaryGrid.TemplatesId);
            idTemplates.Add(template.SYSTEM_ID.ToString());
            this.TemporaryGrid.TemplatesId = idTemplates.ToArray();

            this.box_fields.Update();
        }

        protected void removeTemplateFields(string system_id)
        {
            // Lista dei campi definiti per la griglia attuale
            List<Field> fields;

            // Template da rimuovere
            Templates template;

            // Caricamento del template
            if (this.TemporaryGrid.GridType == GridTypeEnumeration.Project)
                template = ProfilazioneFascManager.getTemplateFascById(system_id, this);
            else
                template = ProfilazioneDocManager.getTemplateById(system_id, this);

            // Inizializzazione della lista dei campi del template
            fields = new List<Field>(this.TemporaryGrid.Fields);

            // Rimozione dalla lista dei campi, dei campi presenti nel template
            foreach (OggettoCustom obj in template.ELENCO_OGGETTI)
            {
                Field tempField = fields.Where(f => f.CustomObjectId.Equals(obj.SYSTEM_ID) && f.AssociatedTemplateId!=null && f.AssociatedTemplateId.Equals(template.SYSTEM_ID.ToString())).FirstOrDefault();
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

        protected void SelectDeselectAll(object sender, EventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            if (box.Checked)
            {
                foreach (Field tempF in this.TemporaryGrid.Fields)
                {
                    if (!tempF.Locked)
                    {
                        tempF.Visible = true;
                    }
                }
                this.checkAll.Text = "Deseleziona tutti";
            }
            else
            {
                foreach (Field tempF in this.TemporaryGrid.Fields)
                {
                    if (!tempF.Locked)
                    {
                        tempF.Visible = false;
                    }
                }
                this.checkAll.Text = "Seleziona tutti";
            }
            this.chkAllPanel.Update();

            if (!string.IsNullOrEmpty(this.hfFieldId.Value))
            {
                SaveCurrentFieldProperties();
            }

            this.gridField.DataSource = this.TemporaryGrid.Fields;
            this.gridField.DataBind();
            this.selectedFieldPosition.Value = string.Empty;
            this.hfFieldId.Value = string.Empty;
            this.box_fields.Update();
            this.btn_up_field.Enabled = false;
            this.btn_down_field.Enabled = false;
            this.upButtons.Update();
            this.txtLabel.Text = string.Empty;
            this.txtLabel.Visible = false;
            this.lblEtichetta.Visible = false;
            this.lblLarghezza.Visible = false;
            this.ddlWidth.Visible = false;
            this.value_fields.Update();
            this.mdlPopupWait.Hide();
            this.UpdateGraphic(this.TemporaryGrid);
        }

        /// <summary>
        /// Al clicm su questo pulsante vengono salvate le impostazioni per la griglia corrente
        /// </summary>
        protected void btnSaveGridSettings_Click(object sender, EventArgs e)
        {
            // Salvataggio dei dati relativi all'eventuale campo selezionato
            // if (this.pnlFieldProperties.Visible)
            this.SaveCurrentFieldProperties();

            // La griglia temporanea diventa la griglia attuale
            GridManager.SelectedGrid = this.TemporaryGrid;

            //// Impostazione del campo su cui compiere l'ordinamento
            //if (this.ddlOrder.SelectedIndex == 0)
            //    // Se l'oggetto legato alla griglia è un documento, viene impostato a null
            //    // il filtro altrimenti viene impostato un filtro pari a "DTA_CREAZIONE"
            //    if (this.TemporaryGrid.GridType == GridTypeEnumeration.Project)
            //        this.TemporaryGrid.FieldForOrder = new Field()
            //        {
            //            OracleDbColumnName = "DTA_CREAZIONE",
            //            SqlServerDbColumnName = "DTA_CREAZIONE",
            //            CustomObjectId = 0
            //        };
            //    else
            //        GridManager.SelectedGrid.FieldForOrder = null;
            //else
            //{
                // Altrimenti, viene prelevato il campo da utilizzare per l'ordinamento
                GridManager.SelectedGrid.FieldForOrder = GridManager.SelectedGrid.Fields.Where(
                    f => f.FieldId.Equals(this.ddlOrder.SelectedItem.Value)).FirstOrDefault();

        //    }

            // Impostazione della direzione di ordinamento
            if (this.ddlAscDesc.SelectedValue.ToUpper() == "ASC")
                GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Asc;
            else
                GridManager.SelectedGrid.OrderDirection = OrderDirectionEnum.Desc;


            GridManager.SelectedGrid.GridId = string.Empty;
            this.TemporaryGrid = null;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(),"closeSave","close_and_save();",true);
            
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

            if (field != null)
            {
                int maxLength, width;
                Int32.TryParse(this.ddlWidth.SelectedItem.Value, out width);
                field.Width = width;
                if (!String.IsNullOrEmpty(this.txtLabel.Text))
                    field.Label = this.txtLabel.Text;
            }

        }

        /// <summary>
        /// Funzione responsabile dell'aggiornamento delle informazioni visualizzate sulla griglia
        /// </summary>
        /// <param name="grid">La griglia da visualizzare</param>
        private void UpdateGraphic(Grid grid)
        {

            // Creazione del data source con le informazioni sui campi utilizzabili per l'ordinamento
            // Viene creato un datasource con due colonne: 
            // - Text che conterrà nome del campo e nome del template cui appartiene il campo
            // - Id del campo
            DataTable dataTable = new DataTable();
            DataRow dataRow;
            dataTable.Columns.Add("Text", typeof(String));
            dataTable.Columns.Add("Id", typeof(String));

            string stardOrder = string.Empty;
            if (this.TemporaryGrid.GridType == GridTypeEnumeration.Project)
            {
                stardOrder = "P20";
            }
            else
            {
                stardOrder = "D9";
            }

            List<Field> fields = grid.Fields.Where(e => (e.Visible && !e.Locked) || e.FieldId.Equals(stardOrder)).OrderBy(f => f.Position).ToList();

            bool presentField = false;


            foreach (Field field in fields)
            {
                dataRow = dataTable.NewRow();
                dataRow["Text"] = String.IsNullOrEmpty(field.AssociatedTemplateName) ? field.Label :
                    String.Format("{0} ({1})", field.Label, field.AssociatedTemplateName);
                dataRow["Id"] = field.FieldId;
                dataTable.Rows.Add(dataRow);
                if (this.TemporaryGrid.FieldForOrder != null && this.TemporaryGrid.FieldForOrder.FieldId.Equals(field.FieldId))
                {
                    presentField = true;
                }
            }

            // Azzeramento degli item della DDL dell'ordinamento
            this.ddlOrder.Items.Clear();

            // Associazione della sorgente dati alla DDL dell'ordinamento
            this.ddlOrder.DataSource = dataTable;
            this.ddlOrder.DataBind();

            if (this.TemporaryGrid.FieldForOrder != null)
            {
                if (presentField)
                {
                    this.ddlOrder.SelectedValue = this.TemporaryGrid.FieldForOrder.FieldId;
                    this.ddlAscDesc.SelectedValue = this.TemporaryGrid.OrderDirection.ToString().ToUpper();
                }
                else
                {
                    this.ddlOrder.SelectedValue = stardOrder;
                    this.ddlAscDesc.SelectedValue = "DESC";
                }
            }
            this.upOrder.Update();
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
            Templates[] templates = null;
            TipologiaAtto[] tempAtto;
            this.lbl_type_template.Text = "Aggiungi i campi delle tipologie di documento";

            // Caricamento dei tipi atto
            tempAtto =  DocumentManager.getTipoAttoPDInsRic(
                this,
                UserManager.getInfoUtente(this).idAmministrazione,
                UserManager.getRuolo(this).idGruppo,
                "1");
            
            if(tempAtto!=null && tempAtto.Length>0)
            {
                templates = new Templates[tempAtto.Length];
            }

            int i = 0;
            foreach (TipologiaAtto ta in tempAtto)
            {
                Templates tempT = new Templates();
                tempT.DESCRIZIONE = ta.descrizione;
                tempT.SYSTEM_ID = Convert.ToInt32(ta.systemId);
                templates[i] = tempT;
                i++;
            }

            
            this.gridTemplates.DataSource = templates;
            this.gridTemplates.DataBind();
        }

        /// <summary>
        /// Funzione per il caricamento dei template relativi ai fascicoli
        /// </summary>
        private void LoadProjectTemplates()
        {
            // Lista dei template disponibili per i fascicoli
            Templates[] templates;

            this.lbl_type_template.Text = "Aggiungi i campi delle tipologie di fascicolo";

            // Caricamento dei tipi atto
            templates = (Templates[])(ProfilazioneFascManager.getTipoFascFromRuolo(
                UserManager.getInfoUtente(this).idAmministrazione,
                UserManager.getRuolo(this).idGruppo,
                "1",
                this).ToArray(typeof(Templates)));

            this.gridTemplates.DataSource = templates;
            this.gridTemplates.DataBind();
        }

        protected void ChangeFieldOrder(object sender, EventArgs e)
        {
            Field d = (Field)TemporaryGrid.Fields.Where(f => f.FieldId.Equals(this.ddlOrder.SelectedValue)).FirstOrDefault();
            if (d != null)
            {
                this.TemporaryGrid.FieldForOrder = d;
            }
        }

        protected void ChangeAscDescOrder(object sender, EventArgs e)
        {
            if (ddlAscDesc.SelectedValue.Equals("ASC"))
            {
                this.TemporaryGrid.OrderDirection = OrderDirectionEnum.Asc;
            }
            else
            {
                this.TemporaryGrid.OrderDirection = OrderDirectionEnum.Desc;
            }
        }


    }
}