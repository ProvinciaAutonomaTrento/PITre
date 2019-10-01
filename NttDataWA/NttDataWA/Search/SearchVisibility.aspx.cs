using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using System.Data;
using NttDataWA.Utils;
using NttDatalLibrary;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Collections;
using System.Collections.Specialized;


namespace NttDataWA.Search
{
    public partial class SearchVisibility : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.DropDownList ddl_Contatori;
        protected Table table;
        protected DocsPaWR.FiltroRicerca[][] qV;
        protected DocsPaWR.FiltroRicerca fV1;
        protected DocsPaWR.FiltroRicerca[] fVList;       
        public struct DocumentsVisibility
        {
            public string Ruolo { get; set; }
            public string Diritti { get; set; }
            public string CodiceRubrica { get; set; }
            public string Tipo { get; set; }
            public string DataFine { get; set; }
            public bool Rimosso { get; set; }
            public string Note { get; set; }
            public string IdCorr { get; set; }
            public string DataInsSecurity { get; set; }
            public string NoteSecurity { get; set; }
            public bool ShowHistory { get; set; }
            public string IdCorrGlobbRole { get; set; }
            public string TipoDiritto { get; set; }
            public string DiSistema { get; set; }
        }

        private enum GrdVisibility
        {
            tipo = 0,
            ruolo = 1,
            motivo = 2,
            diritti = 3,
            datadiritto = 4,
            datafine = 5,
            noteacquisizione = 6,
            rimuovi = 7,
            storia = 8,
            rimosso = 9,
            note = 10,
            idcorr = 11
        }
        protected void Page_Load(object sender, EventArgs e)
        {   
            if (!this.IsPostBack)
            {                
                this.InitializeLanguage();
                this.InitializePage();
                this.PopulateDDLRegistry(this.Role);
                CaricaTipologia(this.DocumentDdlTypeDocument);
                this.LoadGridVisibility(null, 0);                
            }
            else
            {
                this.ReadRetValueFromPopup();
            }

            this.RefreshScript();
        }
     

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        /// <summary>
        /// Funzione per l'inizializzazione del data set in base ai campi definiti nella 
        /// griglia
        /// </summary>
        /// <param name="selectedGrid">La griglia su cui basare la creazione del dataset</param>
        /// <returns></returns>
        public GridView HeaderGridView(Grid selectedGrid, Templates templateTemp, bool showGridPersonalization, GridView grid)
        {
            try
            {
                int position = 0;
                List<Field> fields = selectedGrid.Fields.Where(e => e.Visible).OrderBy(f => f.Position).ToList();
                OggettoCustom customObjectTemp = new OggettoCustom();

                if (templateTemp != null && !showGridPersonalization)
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
                        fields.Insert(2, d);
                    }
                    else
                        fields.Remove(d);
                }

                grid.Columns.Clear();

                // Creazione delle colonne
                foreach (Field field in fields)
                {
                    BoundField column = null;
                    ButtonField columnHL = null;
                    TemplateField columnCKB = null;
                    if (field.OriginalLabel.ToUpper().Equals("DOCUMENTO"))
                    {
                        columnHL = GridManager.GetLinkColumn(field.Label,
                            field.FieldId,
                            field.Width);
                        columnHL.SortExpression = field.FieldId;
                    }
                    else
                    {

                        if (field is SpecialField)
                        {
                            switch (((SpecialField)field).FieldType)
                            {
                                case SpecialFieldsEnum.Icons:
                                    columnCKB = GridManager.GetBoundColumnIcon(field.Label, field.Width, field.FieldId);
                                    columnCKB.SortExpression = field.FieldId;
                                    break;
                                case SpecialFieldsEnum.CheckBox:
                                    {
                                        columnCKB = GridManager.GetBoundColumnCheckBox(field.Label, field.Width, field.FieldId);
                                        columnCKB.SortExpression = field.FieldId;
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (field.FieldId)
                            {
                                case "CONTATORE":
                                    {
                                        column = GridManager.GetBoundColumn(
                                            field.Label,
                                            field.OriginalLabel,
                                            100,
                                            field.FieldId);
                                        column.SortExpression = field.FieldId;
                                        break;
                                    }

                                default:
                                    {
                                        column = GridManager.GetBoundColumn(
                                         field.Label,
                                         field.OriginalLabel,
                                         field.Width,
                                         field.FieldId);
                                        column.SortExpression = field.FieldId;
                                        break;
                                    }
                            }
                        }
                    }



                    if (columnCKB != null)
                        grid.Columns.Add(columnCKB);
                    else
                        if (column != null)
                            grid.Columns.Add(column);
                        else
                            grid.Columns.Add(columnHL);



                    if (!this.CellPosition.ContainsKey(field.FieldId))
                    {
                        CellPosition.Add(field.FieldId, position);
                    }
                    // Aggiornamento della posizione
                    position += 1;
                }
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IdProject", "IdProject"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsInStorageArea", "IsInStorageArea"));
                grid.Columns.Add(GridManager.GetBoundColumnNascosta("IsInWorkingArea", "IsInWorkingArea"));



                return grid;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }


       
        
        
        protected void SearchDocumentDdlIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.SearchDocumentDdlIn.SelectedValue))
            {
                switch (this.SearchDocumentDdlIn.SelectedValue)
                {
                    case "0": //Protocollati
                        this.PhlSearchIDDocument.Visible = false;
                        PhlSearchProto.Visible = true;
                        this.phlSearchTypologyDoc.Visible = false; 
                        break;
                    case "1": //Non Protocollati 
                        this.PhlSearchIDDocument.Visible = true;
                        PhlSearchProto.Visible = false;
                        this.phlSearchTypologyDoc.Visible = false; 
                        break;
                    case "2": // Ricerca per tipologia 
                        this.PhlSearchIDDocument.Visible = false;
                        PhlSearchProto.Visible = false;
                        this.phlSearchTypologyDoc.Visible = true; 
                        break;
                }
            }
        }

        protected bool GetGridPersonalization()
        {
            return this.ShowGridPersonalization;
        }

        protected string GetTitle()
        {
            return this.VisibilityHistory.Title;
        }

        protected void ImgHistory_Click(object sender, EventArgs e)
        {
            try
            {
                UIManager.DocumentManager.setSelectedRecord(DocumentManager.getDocumentDetailsNoSecurity(this.Page,Session["idProfileVisibilitySearch"].ToString(),Session["idProfileVisibilitySearch"].ToString()));
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpdPanelVisibility", "ajaxModalPopupVisibilityHistory();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
 
        protected void SearchProjectSearch_Click(object sender, EventArgs e)
        {        
            //cerco idProfile partendo dai dati inseriti
            string idDocProt = string.Empty;
            int idProfile = 0;            
            bool numeroRisultati = true;
            DocsPaWR.InfoDocumento[] ListaDoc = null;
            string inArchivio = "-1";
           

            switch (SearchDocumentDdlIn.SelectedValue.ToString())
            {
                case "0":
                    if (!string.IsNullOrEmpty(this.TxtNumProto.Text) && (!string.IsNullOrEmpty(this.TxtYear.Text)) )
                    {
                        idDocProt = TxtNumProto.Text.Trim();
                    }
                    else
                    {
                        string msgDesc = "AlertSearchVisibilityProt";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                    idProfile = UserManager.getIdProfileByData(UserManager.GetInfoUser(), idDocProt, TxtYear.Text.Trim(), DdlRegistries.SelectedValue, out inArchivio);
                    break;

                case "1":
                    int result;
                    bool isNum = false;
                    if (!string.IsNullOrEmpty(this.TxtIDDocument.Text) )
                    {
                        idDocProt = TxtIDDocument.Text.Trim(); // "1999935" 
                        if (int.TryParse(idDocProt, out result))
                            isNum = true; 

                    }
                    if (string.IsNullOrEmpty(this.TxtIDDocument.Text) || !isNum)
                    {
                        string msgDesc = "AlertSearchVisibilityNum";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                    idProfile = UserManager.getIdProfileByData(UserManager.GetInfoUser(), idDocProt, null, null, out inArchivio);
                    break;

                case "2":
                    // parametri in input: 
                    //1) tipologia documento 2)tipo contatore 3)AOO o RF 4) Numero contatore
                    if (DocumentDdlTypeDocument.SelectedIndex == 0)
                    {
                      
                        string msgDesc = "AlertSearchVisibilityType";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                       
                        this.panel_Contenuto.Visible = false;
                        this.pnl_RFAOO.Visible = false;
                        this.pnlAnno.Visible = false;
                        this.pnlNumero.Visible = false;
                        return;
                    }
                    DropDownList ddl = (DropDownList)panel_Contenuto.FindControl("ddl_Contatori");
                    if (ddl != null && ddl.SelectedValue == "")
                    {
                        Response.Write("<script>alert('Attenzione selezionare un contatore.')</script>");
                        this.panel_Contenuto.Visible = true;
                        this.pnl_RFAOO.Visible = false;
                        return;
                    }
                    if (string.IsNullOrEmpty(this.TxtAnno.Text))
                    {
                        string msgDesc = "AlertSearchVisibilityTypeYearNumCont";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    
                        this.pnl_RFAOO.Visible = true;
                        this.lblAooRF.Visible = true;
                        this.ddlAooRF.Visible = true;
                        return;
                    }
                    if (string.IsNullOrEmpty(this.TxtNumero.Text))
                    {
                        string msgDesc = "AlertSearchVisibilityTypeYearNumCont";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    
                        this.pnl_RFAOO.Visible = true;
                        this.lblAooRF.Visible = true;
                        this.ddlAooRF.Visible = true;
                        return;
                    }

                    DocsPaWR.Templates template = (DocsPaWR.Templates)Session["template"];

                    for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
                    {
                        DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                        if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("Contatore"))
                        {
                            if (ddl != null && ddl.SelectedIndex != -1)
                            {
                                if (oggettoCustom.SYSTEM_ID == Convert.ToInt32(ddl.SelectedValue))
                                {
                                    oggettoCustom.VALORE_DATABASE = this.TxtNumero.Text + "@" + this.TxtNumero.Text;
                                    oggettoCustom.ID_AOO_RF = this.ddlAooRF.SelectedValue;
                                }
                            }
                            else
                            {
                                oggettoCustom.VALORE_DATABASE = this.TxtNumero.Text + "@" + this.TxtNumero.Text;
                                oggettoCustom.ID_AOO_RF = this.ddlAooRF.SelectedValue;
                            }
                        }
                        else
                        {
                            // poichè la ricerca deve essere fatta per un solo contatore, metto a
                            // stringa vuota il valore di tutti gli altri oggetti del template
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                            oggettoCustom.ID_AOO_RF = string.Empty;
                        }
                        //}
                    }

                    qV = new DocsPaWR.FiltroRicerca[1][];
                    qV[0] = new DocsPaWR.FiltroRicerca[1];
                    fVList = new DocsPaWR.FiltroRicerca[0];

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
                    fV1.valore = this.TxtAnno.Text;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
                    fV1.valore = "true";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
                    fV1.valore = "true";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
                    fV1.valore = "true";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
                    fV1.valore = "true";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString();
                    fV1.valore = this.DocumentDdlTypeDocument.SelectedItem.Value;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.FROM_RICERCA_VIS.ToString();
                    fV1.valore = "1";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.PROFILAZIONE_DINAMICA.ToString();
                    fV1.template = template;
                    fV1.valore = "Profilazione Dinamica";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    qV[0] = fVList;

                    int numTotPage = 0;
                    int nRec = 0;
                    SearchResultInfo[] idProfileList;
                    ListaDoc = DocumentManager.getQueryInfoDocumentoPaging(UserManager.GetInfoUser().idGruppo, UserManager.GetInfoUser().idPeople, this, qV, 1, out numTotPage, out nRec, false, false, false, false, out idProfileList);

                    if (ListaDoc.Length > 1)
                    {
                        // non dovrebbe succedere ma per errori di inserimento nel DB, potrebbe 
                        // accadere che questa query restituisca più di un risultato (se il numero
                        // del contatore è valorizzato)--> in questo caso si restituisce
                        // solo il primo documento trovato.

                        if (!string.IsNullOrEmpty(this.TxtNumero.Text))
                        {
                            idProfile = Convert.ToInt32(ListaDoc[0].idProfile);
                            inArchivio = ListaDoc[0].inArchivio;
                        }
                        else
                            numeroRisultati = false;
                    }
                    else
                    {
                        if (ListaDoc.Length != 0)
                        {
                            idProfile = Convert.ToInt32(ListaDoc[0].idProfile);
                            inArchivio = ListaDoc[0].inArchivio;
                        }
                        else
                            idProfile = 0;
                    }
                    break;
            }

            if (numeroRisultati)
            {
                if (idProfile > 0 || inArchivio == "1")
                    this.LoadGridVisibility(null, idProfile);
                else
                {
                    string msgDesc = "AlertSearchVisibilityNoRes";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    this.GridDocuments.DataSource = null;
                    this.GridDocuments.DataBind();
                    this.UpdPanelVisibility.Update();
                    return;
                }
            }
            else
            {
                string msgDesc = "AlertSearchVisibilityNoRes";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                return;
            }   
            
            
            Session["idProfileVisibilitySearch"] = idProfile.ToString();
        }

  
        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void InitializePage()
        {            
            this.InitializeLanguage();
            this.LoadKeys();             
            this.PopulateDdlSearchDocument();
            this.LinkSearchVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
        }
        
     
        /// <summary>
        /// Initializes application labels
        /// </summary>
          
        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();          
            this.GridDocuments.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityType", language);
            this.GridDocuments.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityRoleUser", language);
            this.GridDocuments.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityMotive", language);
            this.GridDocuments.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityRights", language);
            this.GridDocuments.Columns[4].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityDateStarting", language);
            this.GridDocuments.Columns[5].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityDateEnding", language);
            this.GridDocuments.Columns[6].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityNotes", language);
            this.GridDocuments.Columns[8].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityHistory", language);
            this.GridDocuments.Columns[9].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityRemoved", language);
            this.GridDocuments.Columns[10].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityNotes", language);
            this.GridDocuments.Columns[12].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityDetails", language);
            this.VisibilityHistory.Title = Utils.Languages.GetLabelFromCode("VisibilityTitle", language); 
            this.LitSearchProject.Text = Utils.Languages.GetLabelFromCode("SearchVisibilityTitle", language);            
            this.VisibilityLblDelVis.Text = Utils.Languages.GetLabelFromCode("VisibilityLblDelVis", language);
            this.GridDocuments.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityType", language);
            this.GridDocuments.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityRoleUser", language);
            this.GridDocuments.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityMotive", language);
            this.GridDocuments.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityRights", language);
            this.GridDocuments.Columns[4].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityDateStarting", language);
            this.GridDocuments.Columns[5].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityDateEnding", language);
            this.GridDocuments.Columns[6].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityNotes", language);
            this.GridDocuments.Columns[8].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityHistory", language);
            this.GridDocuments.Columns[9].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityRemoved", language);
            this.GridDocuments.Columns[10].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityNotes", language);
            this.GridDocuments.Columns[12].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityDetails", language);
            
            ///////////////////////////////////////////////////////////////////////////////////////
            this.SearchProjectSearch.Text = Utils.Languages.GetLabelFromCode("SearchLabelButton", language);
            this.SearchProjectRemove.Text = Utils.Languages.GetLabelFromCode("SearchLabelRemoveFiltersButton", language);
            this.litRegistry.Text = Utils.Languages.GetLabelFromCode("SearchProjectRegistry", language);
            this.DdlRegistries.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language);
            this.SearchIDDocument.Text = Utils.Languages.GetLabelFromCode("SearchIDDocument", language);
            this.litYear.Text = Utils.Languages.GetLabelFromCode("SearchProjectYear", language);
            this.litNum.Text = Utils.Languages.GetLabelFromCode("LblAddFilterNumProtocol", language);
            this.SearchDocumentLitTypology.Text = Utils.Languages.GetLabelFromCode("SearchDocumentLitTypology", language);
            this.DocumentDdlTypeDocument.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("GenericChosenSelectOne", language);
            this.LinkSearchVisibility.Text = Utils.Languages.GetLabelFromCode("LinkSearchVisibility", language);
            this.VisibilityRemove.Title = Utils.Languages.GetLabelFromCode("VisibilityRemove", language);
       }

        private void LoadKeys()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_DESC_FASC.ToString())))
            {
                this.MaxLenghtProject = int.Parse(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_DESC_FASC.ToString()));
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_CONS"))
            {
                this.AllowConservazione = true;
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_ADD_ADL"))
            {
                this.AllowADL = true;
            }

            this.ShowGridPersonalization = UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION");

            this.InitializePageSize();

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()]) && !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamicaFasc.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamicaFasc.ToString()].Equals("1"))
            {
                this.CustomDocuments = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()].Equals("1"))
            {
                this.EnableStateDiagram = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]))
            { 
                this.SearchProjectRemove.Visible = false;
            }
        }

        protected void InitializePageSize()
        {
            string keyValue = Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_PAGING_ROW_PROJECT.ToString());
            int tempValue = 0;
            if (!string.IsNullOrEmpty(keyValue))
            {
                tempValue = Convert.ToInt32(keyValue);
                if (tempValue >= 20 || tempValue <= 50)
                {
                    this.PageSize = tempValue;
                }
            }
        } 
 

        #region Properties

        protected int MaxLenghtProject
        {
            get
            {
                int result = 2000;
                if (HttpContext.Current.Session["maxLenghtProject"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["maxLenghtProject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["maxLenghtProject"] = value;
            }
        }

        private bool ShowGridPersonalization
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["showGridPersonalization"] != null)
                {
                    return (bool)HttpContext.Current.Session["showGridPersonalization"];
                }
                return result;

            }
            set
            {
                HttpContext.Current.Session["showGridPersonalization"] = value;
            }
        }

        /// <summary>
        /// Number of result in page
        /// </summary>
        public int PageSize
        {
            get
            {
                int result = 20;
                if (HttpContext.Current.Session["pageSizeDocument"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["pageSizeDocument"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["pageSizeDocument"] = value;
            }
        }

        /// <summary>
        /// Risultati restituiti dalla ricerca.
        /// </summary>
        public SearchObject[] Result
        {
            get
            {
                return HttpContext.Current.Session["result"] as SearchObject[];
            }
            set
            {
                HttpContext.Current.Session["result"] = value;
            }
        }

        private FiltroRicerca[][] SearchFilters
        {
            get
            {
                return (FiltroRicerca[][])HttpContext.Current.Session["filtroRicerca"];
            }
            set
            {
                HttpContext.Current.Session["filtroRicerca"] = value;
            }
        }

        private int RecordCount
        {
            get
            {
                int toReturn = 0;
                if (HttpContext.Current.Session["recordCount"] != null) Int32.TryParse(HttpContext.Current.Session["recordCount"].ToString(), out toReturn);
                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["recordCount"] = value;
            }
        }

        private int SelectedPage
        {
            get
            {
                int toReturn = 1;
                if (HttpContext.Current.Session["selectedPage"] != null) Int32.TryParse(HttpContext.Current.Session["selectedPage"].ToString(), out toReturn);
                if (toReturn < 1) toReturn = 1;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["selectedPage"] = value;
            }
        }

        /// <summary>
        /// Numero di pagine restituiti dalla ricerca
        /// </summary>
        public int PageCount
        {
            get
            {
                int toReturn = 1;

                if (HttpContext.Current.Session["PageCount"] != null)
                {
                    Int32.TryParse(
                        HttpContext.Current.Session["PageCount"].ToString(),
                        out toReturn);
                }

                return toReturn;
            }

            set
            {
                HttpContext.Current.Session["PageCount"] = value;
            }
        }

        private DataTable GrigliaResult
        {
            get
            {
                return (DataTable)HttpContext.Current.Session["GrigliaResult"];

            }
            set
            {
                HttpContext.Current.Session["GrigliaResult"] = value;
            }
        }

        private bool AllowConservazione
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["AllowConservazione"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["AllowConservazione"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["AllowConservazione"] = value;
            }
        }

        private bool AllowADL
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["AllowADL"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["AllowADL"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["AllowADL"] = value;
            }
        }

        private bool IsZoom
        {
            get
            {
                if (HttpContext.Current.Session["isZoom"] != null)
                    return (bool)HttpContext.Current.Session["isZoom"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["isZoom"] = value;
            }
        }

        /// <summary>
        /// Posizione celle per ordinamento
        /// </summary>
        public Dictionary<string, int> CellPosition
        {
            get
            {
                return HttpContext.Current.Session["cellPosition"] as Dictionary<string, int>;
            }
            set
            {
                HttpContext.Current.Session["cellPosition"] = value;
            }

        }

        private bool CustomDocuments
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["customDocuments"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["customDocuments"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["customDocuments"] = value;
            }
        }

        private bool EnableStateDiagram
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableStateDiagram"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableStateDiagram"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableStateDiagram"] = value;
            }
        }

        private DocsPaWR.Templates Template
        {
            get
            {
                Templates result = null;
                if (HttpContext.Current.Session["template"] != null)
                {
                    result = HttpContext.Current.Session["template"] as Templates;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["template"] = value;
            }
        }

        /// <summary>
        /// Classificazione da utilizzare per la ricerca fascicoli
        /// </summary>
        public FascicolazioneClassificazione Classification
        {
            get
            {
                return HttpContext.Current.Session["classification"] as FascicolazioneClassificazione;
            }

            set
            {
                HttpContext.Current.Session["classification"] = value;
            }

        }

        private DocsPaWR.Registro Registry
        {
            get
            {
                DocsPaWR.Registro result = null;
                if (HttpContext.Current.Session["registry"] != null)
                {
                    result = HttpContext.Current.Session["registry"] as DocsPaWR.Registro;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["registry"] = value;
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

                if (HttpContext.Current.Session["allClassification"]!=null)
                    Boolean.TryParse(
                        HttpContext.Current.Session["allClassification"].ToString(),
                        out toReturn);

                return toReturn;
            }

            set
            {
                HttpContext.Current.Session["allClassification"] = value;
            }
        }

        public string IdCustomObjectCustomCorrespondent
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["idCustomObjectCustomCorrespondent"] != null)
                {
                    result = HttpContext.Current.Session["idCustomObjectCustomCorrespondent"] as string;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["idCustomObjectCustomCorrespondent"] = value;
            }
        }

        private DocsPaWR.Fascicolo Project
        {
            get
            {
                Fascicolo result = null;
                if (HttpContext.Current.Session["project"] != null)
                {
                    result = HttpContext.Current.Session["project"] as Fascicolo;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["project"] = value;
            }
        }

        /// <summary>
        /// valore di ritorno della popup del titolario
        /// </summary>
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

        private DocsPaWR.Ruolo Role
        {
            get
            {
                return UIManager.RoleManager.GetRoleInSession();
            }
            set
            {
                HttpContext.Current.Session["role"] = value;
            }
        }

        #endregion



    
        protected void LoadGridVisibility(FilterVisibility[] ListFiltered , int idProfile )
        {
            bool cercaRimossi=true;
            this.lblResult.Text = "";
            this.lblResult.Visible = false;
           
            if (idProfile <= 0)            {
               
                this.UpdPanelVisibility.Update();
                return;
            }              
           
            this.VisibilityList = DocumentManager.GetSimpliedListVisibilityWithFilters(this, idProfile.ToString(), /* "1999935"*/ cercaRimossi, ListFiltered);

            // Create and initialize a generic list
            List<DocumentsVisibility> DocDir = new List<DocumentsVisibility>();
            DocumentsVisibility DocVis = new DocumentsVisibility();

            List<DocumentsVisibility> DocDirFiltered = new List<DocumentsVisibility>();

            if (this.VisibilityList != null)
            {
                for (int i = 0; i < this.VisibilityList.Length; i++)
                {
                    string descrSoggetto = UserManager.GetCorrespondingDescription(this, this.VisibilityList[i].soggetto);
                    string Corr = GetTipoCorr(this.VisibilityList[i]);
                    bool Removed = this.VisibilityList[i].deleted;
                    string Diritti = this.GetRightDescription(this.VisibilityList[i].accessRights);

                    DocVis.Ruolo = descrSoggetto;
                    DocVis.Diritti = this.setTipoDiritto(this.VisibilityList[i]);
                    DocVis.CodiceRubrica = this.VisibilityList[i].soggetto.codiceRubrica;
                    DocVis.Tipo = Corr;
                    DocVis.DataFine = this.VisibilityList[i].soggetto.dta_fine;
                    DocVis.TipoDiritto = Diritti;
                    DocVis.Rimosso = Removed;
                    DocVis.Note = this.VisibilityList[i].note;
                    DocVis.IdCorr = this.VisibilityList[i].personorgroup;
                    DocVis.DataInsSecurity = this.VisibilityList[i].dtaInsSecurity;
                    DocVis.NoteSecurity = this.VisibilityList[i].noteSecurity;
                    DocVis.DiSistema = this.VisibilityList[i].DiSistema;

                    DocDir.Add(DocVis);
                }

                // INC000000596988 - problema con copie visibilità
                // filtro la lista per rimuovere eventuali duplicati
                DocDirFiltered = DocDir.GroupBy(d => d.Ruolo).Select(x => x.FirstOrDefault()).ToList();

                Session["idProfileVisibilitySearch"] = idProfile.ToString();
            }       
            
            //this.DocumentsVisibilityList = DocDir;          
            this.DocumentsVisibilityList = DocDirFiltered;
            this.GridDocuments.DataSource = this.DocumentsVisibilityList;
            this.GridDocuments.DataBind();            
            this.UpdPanelVisibility.Update();

        }
 
        protected void GridDocuments_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            Literal LblDetails = new Literal();
            Literal LblDetailsInfo = new Literal();
            LblDetails = (Literal)GridDocuments.Rows[RowSelected.DataItemIndex].Cells[(int)GrdVisibility.tipo].FindControl("LblDetails");
            LblDetailsInfo = (Literal)GridDocuments.Rows[RowSelected.DataItemIndex].Cells[(int)GrdVisibility.tipo].FindControl("LblDetailsInfo");
            LblDetailsInfo.Text = "";
            LblDetails.Text = "";
            this.UpContainer.Update();
            this.UpdPanelVisibility.Update();
        }

        protected void GridDocuments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridDocuments.PageIndex = e.NewPageIndex;
            this.GridDocuments.DataSource = this.DocumentsVisibilityList;
            this.GridDocuments.DataBind();
            this.UpdPanelVisibility.Update();
        }

        protected void GridDocuments_PreRender(object sender, EventArgs e)
        {
            //Verifica se l'utente è abilitato alla funzione di editing ACL
            bool IsACLauthorized = UserManager.IsAuthorizedFunctions("ACL_RIMUOVI");
            bool ripristina = false;

            string language = UIManager.UserManager.GetUserLanguage();
            string removeTooltip = Utils.Languages.GetLabelFromCode("VisibilityRemoveTooltip", language);
            string restoreTooltip = Utils.Languages.GetLabelFromCode("VisibilityRestoreTooltip", language);

            CustomImageButton ImgType = new CustomImageButton();
            HiddenField hdnCodRubrica = new HiddenField();
            HiddenField diSistema = new HiddenField();                
            Literal LblDetails = new Literal();
            HiddenField hdnTipo = new HiddenField();
            CustomImageButton ImgDelete = new CustomImageButton();
            Image ImgTipo = new Image();
            Label LblEndDate = new Label();
            Label LblRemoved = new Label();
            Label LblDiritto = new Label();

            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

            for (int i = 0; i < GridDocuments.Rows.Count; i++)
            {
                if (GridDocuments.Rows[i].DataItemIndex >= 0)
                {
                    hdnTipo = (HiddenField)GridDocuments.Rows[i].Cells[(int)GrdVisibility.tipo].FindControl("hdnTipo");
                    ImgTipo = (Image)GridDocuments.Rows[i].Cells[(int)GrdVisibility.tipo].FindControl("imgTipo");
                    LblEndDate = (Label)GridDocuments.Rows[i].Cells[(int)GrdVisibility.datafine].FindControl("LblEndDate");
                    LblRemoved = (Label)GridDocuments.Rows[i].Cells[(int)GrdVisibility.rimosso].FindControl("LblRemoved");
                    LblDiritto = (Label)GridDocuments.Rows[i].Cells[(int)GrdVisibility.diritti].FindControl("LblDiritto");
                    ImgDelete = (CustomImageButton)GridDocuments.Rows[i].Cells[(int)GrdVisibility.rimuovi].FindControl("ImgDelete");
                    diSistema = (HiddenField)GridDocuments.Rows[i].Cells[(int)GrdVisibility.tipo].FindControl("hdnDiSistema");

                    if (!string.IsNullOrEmpty(LblEndDate.Text))
                    {
                        GridDocuments.Rows[i].ForeColor = System.Drawing.Color.Gray;
                        GridDocuments.Rows[i].Font.Bold = false;
                        GridDocuments.Rows[i].Font.Strikeout = false;
                    }

                    //Se Acl è revocata allora la riga è di colore rosso e barrata
                    int Removed;
                    if (LblRemoved.Text.ToUpper() == "TRUE") Removed = 1; else Removed = 0;

                    if (!string.IsNullOrEmpty(LblRemoved.Text) && Removed == 1)
                    {
                        GridDocuments.Rows[i].ForeColor = System.Drawing.Color.Red;
                        GridDocuments.Rows[i].Font.Bold = true;
                        GridDocuments.Rows[i].Font.Strikeout = true;
                        ImgDelete.ImageUrl = "../Images/Icons/ico_risposta.gif";
                        ImgDelete.CommandName = "Ripristina";
                        ImgDelete.ToolTip = restoreTooltip;
                        ImgDelete.OnMouseOutImage = "../Images/Icons/ico_risposta.gif";
                        ImgDelete.OnMouseOverImage = "../Images/Icons/ico_risposta.gif";

                        ripristina = true;
                    }
                    else
                    {
                        ImgDelete.ToolTip = removeTooltip;
                    }

                    //Se l'utente è proprietario del documento, non è MAI possibile rimuovere i diritti.
                    //Se l'utente ha la funzione di "editing ACL" può rimuovere i diritti anche ad altri ruoli/utenti.
                    string Diritto = LblDiritto.Text;
                    int IdCorr = Convert.ToInt32(((System.Web.UI.WebControls.GridView)(sender)).DataKeys[i].Value);

                    ImgDelete.Visible = false;

                    if (hdnTipo.Value.Equals("UTENTE"))
                    {
                        string cssClass = "nopointer";
                        if (i % 2 == 1) cssClass += " AltRow";
                        GridDocuments.Rows[i].CssClass = cssClass;

                        if (infoUtente.idPeople != IdCorr.ToString() && !ripristina && IsACLauthorized && !this.AbortDocument)
                            ImgDelete.Visible = false;

                        if (Diritto.Equals(Utils.Languages.GetLabelFromCode("VisibilityLabelOwner", language)))
                            ImgDelete.Visible = false;

                        ImgTipo.ImageUrl = "~/Images/Icons/user_icon.png";
                    }
                    else
                    {
                        if (!diSistema.Value.Equals("1"))
                        {
                            string jsOnClick = "$('#rowIndex').val('" + GridDocuments.Rows[i].RowIndex.ToString() + "'); $('#btnDetails').click();";
                            GridDocuments.Rows[i].Cells[0].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[1].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[2].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[3].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[4].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[5].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[6].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[8].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[9].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[10].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[11].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[12].Attributes["onclick"] = jsOnClick;
                        }
                        else
                        {
                            string cssClass = "nopointer";
                            if (i % 2 == 1) cssClass += " AltRow";
                            GridDocuments.Rows[i].CssClass = cssClass;
                        }
                        if (infoUtente.idGruppo != IdCorr.ToString() && !ripristina && IsACLauthorized && !this.AbortDocument)
                            ImgDelete.Visible = false;

                        if (Diritto.Equals("PROPRIETARIO"))
                            ImgDelete.Visible = false;

                        ImgTipo.ImageUrl = "~/Images/Icons/role2_icon.png";
                    }
                }

                if (ripristina) ImgDelete.Visible = false;

                if (diSistema.Value.Equals("1"))
                {
                    ripristina = false;
                    ImgDelete.Visible = false;
                    
                    ImgTipo.ImageUrl = "~/Images/Icons/external_system_icon.png";
                }
            }

            //i client senza acl non devono vedere la colonna
            GridDocuments.Columns[4].Visible = IsACLauthorized;
        }


        protected void VisibilityBtnFilter_Click(object sender, EventArgs e)
        {
            
            this.GridDocuments.PageIndex = 0;
            this.GridDocuments.DataSource = this.DocumentsVisibilityList;
            this.GridDocuments.DataBind();
            this.UpdPanelVisibility.Update();

            

        }

        

        protected void GridDocuments_Details(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.rowIndex.Value))
            {
                int selRow = int.Parse(this.rowIndex.Value);
                if (selRow == this.GridDocuments.SelectedIndex) selRow = -1;

                this.GridDocuments.SelectedIndex = selRow;
                this.RowSelected = this.GridDocuments.SelectedRow;
                if (this.DocumentsVisibilityFilters == null || this.DocumentsVisibilityFilters.Count == 0)
                    this.GridDocuments.DataSource = this.DocumentsVisibilityList;
                else
                    this.GridDocuments.DataSource = this.DocumentsVisibilityFilters;
                this.GridDocuments.DataBind();

                if (selRow >= 0)
                {
                    HiddenField hdnCodRubrica = new HiddenField();
                    Literal LblDetails = new Literal();
                    LblDetails = (Literal)GridDocuments.Rows[selRow].Cells[(int)GrdVisibility.tipo].FindControl("VisibilityLblDetails");
                    hdnCodRubrica = (HiddenField)GridDocuments.Rows[selRow].Cells[(int)GrdVisibility.tipo].FindControl("hdnCodRubrica");
                    this.GetDetailsCorresponding(hdnCodRubrica.Value);
                }

                this.UpdPanelVisibility.Update();
                this.UpContainer.Update();
            }
        }



        protected void GridDocuments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int selRow = 0;

            switch (e.CommandName)
            {
                case "Select":
                    this.RowSelected = (GridViewRow)(((CustomImageButton)e.CommandSource).NamingContainer);
                    selRow = RowSelected.DataItemIndex;
                    this.GridDocuments.SelectedRowStyle.BackColor = System.Drawing.Color.Yellow;
                    break;
                case "Erase":
                    this.RowSelected = (GridViewRow)(((CustomImageButton)e.CommandSource).NamingContainer);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "VisibilityRemove", "ajaxModalPopupVisibilityRemove();", true);
                    break;
                case "Ripristina":
                    this.RowSelected = (GridViewRow)(((CustomImageButton)e.CommandSource).NamingContainer);
                    this.RestoreACL();
                    this.UpdPanelVisibility.Update();
                    break;
                case "Cancel":
                    this.GridDocuments.EditIndex = -1;
                    this.UpdPanelVisibility.Update();
                    break;
            }
        }

        private void GetDetailsCorresponding(string CodiceRubrica)
        {

            //Build object queryCorrispondente
            DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPaWR.AddressbookQueryCorrispondente();

            qco.codiceRubrica = CodiceRubrica;
            qco.getChildren = true;
            qco.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
            qco.fineValidita = true;

            //Internal corresponding
            qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;
            DocsPaWR.Corrispondente[] listaCorr = UserManager.getListCorrispondent(qco);

            //Visualize information of users
            string st_listaCorr = "";
            DocsPaWR.Corrispondente cor;

            for (int i = 0; i < listaCorr.Length; i++)
            {
                cor = (DocsPaWR.Corrispondente)listaCorr[i];
                if (cor.dta_fine != string.Empty)
                    st_listaCorr += "<li><span class=\"corr_grey\">" + ((DocsPaWR.Corrispondente)listaCorr[i]).descrizione + "</span></li>\n";
                else
                    st_listaCorr += "<li>" + ((DocsPaWR.Corrispondente)listaCorr[i]).descrizione + "</li>\n";
            }
            if (!string.IsNullOrEmpty(st_listaCorr))
                st_listaCorr = "<ul>\n"
                            + st_listaCorr
                            + "</ul>\n";

            Literal LblDetails = new Literal();
            Literal LblDetailsInfo = new Literal();
            Literal lblDetailsUser = new Literal();

            LblDetails = (Literal)GridDocuments.Rows[RowSelected.RowIndex].Cells[(int)GrdVisibility.ruolo].FindControl("LblDetails");
            LblDetailsInfo = (Literal)GridDocuments.Rows[RowSelected.RowIndex].Cells[(int)GrdVisibility.ruolo].FindControl("LblDetailsInfo");
            lblDetailsUser = (Literal)GridDocuments.Rows[RowSelected.RowIndex].Cells[(int)GrdVisibility.ruolo].FindControl("lblDetailsUser");

            if (st_listaCorr.Length > 0)
            {
                lblDetailsUser.Visible = true;
                LblDetails.Visible = true;
                LblDetailsInfo.Visible = true;
                lblDetailsUser.Text = Utils.Languages.GetLabelFromCode("VisibilityRoleDetails", UIManager.UserManager.GetUserLanguage());
                LblDetails.Text = st_listaCorr;
                GridDocuments.Rows[RowSelected.RowIndex].Cells[(int)GrdVisibility.ruolo].FindControl("divDetails").Visible = true;
            }
            else
            {
                LblDetails.Visible = false;
                LblDetailsInfo.Visible = false;
                lblDetailsUser.Visible = false;
                LblDetails.Text = "";
                GridDocuments.Rows[RowSelected.RowIndex].Cells[(int)GrdVisibility.ruolo].FindControl("divDetails").Visible = false;
            }

            this.UpContainer.Update();
            this.UpdPanelVisibility.Update();
        }

        protected string PersonOrGroup()
        {
            //Verify if user or role
            string personOrGroup;

            HiddenField hdnTipo = new HiddenField();
            hdnTipo = (HiddenField)GridDocuments.Rows[RowSelected.RowIndex].Cells[(int)GrdVisibility.tipo].FindControl("hdnTipo");

            if (hdnTipo.Value.ToUpper().Equals("UTENTE"))
                personOrGroup = "U";
            else
                personOrGroup = "R";

            return personOrGroup;
        }

        private void RestoreACL()
        {
            string personOrGroup = PersonOrGroup();

            DocumentoDiritto[] ListDocDir = VisibilityList;
            DocsPaWR.DocumentoDiritto docDiritti = ListDocDir[RowSelected.DataItemIndex];
            
            //Laura 15 Aprile   --Il metodo RestoreACL è stato modificato, ora prende in ingresso anche il tipo di oggetto. In questo caso la ricerca di visibilità viene fatta soltanto per i documenti (D)            
            bool result = DocumentManager.RestoreACL(docDiritti, personOrGroup, UserManager.GetInfoUser(), "D");
            if (result)
            {
                if (Session["idProfileVisibilitySearch"] != null)
                    this.LoadGridVisibility(null, Convert.ToInt32(Session["idProfileVisibilitySearch"]));
                
                this.GridDocuments.SelectedIndex = -1;
            }         

        }

        protected string GetTipoCorr(DocumentoDiritto docDirit)
        {
            try
            {
                string rtn = "";
                if (docDirit.soggetto.tipoCorrispondente.Equals("P"))
                    rtn = "UTENTE";
                else if (docDirit.soggetto.tipoCorrispondente.Equals("R"))
                    rtn = "RUOLO";
                else if (docDirit.soggetto.tipoCorrispondente.Equals("U"))
                    rtn = "U.O.";
                return rtn;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        private string setTipoDiritto(DocumentoDiritto docDir)
        {
            string star = "";
            string RetVal = "";

            string language = UIManager.UserManager.GetUserLanguage();

            if (docDir.hideDocVersions)
                star = Environment.NewLine + "*";

            switch (docDir.tipoDiritto)
            {
                case DocumentoTipoDiritto.TIPO_ACQUISITO:
                    if (docDir.accessRights == (int)NttDataWA.Utils.HMdiritti.HDdiritti_Waiting)
                        RetVal = Utils.Languages.GetLabelFromCode("VisibilityLabelPending", language) + star;
                    else
                        RetVal = Utils.Languages.GetLabelFromCode("VisibilityLabelAcquired", language) + star;
                    break;
                case DocumentoTipoDiritto.TIPO_PROPRIETARIO:
                case DocumentoTipoDiritto.TIPO_DELEGATO:
                    RetVal = Utils.Languages.GetLabelFromCode("VisibilityLabelOwner", language) + star;
                    break;
                case DocumentoTipoDiritto.TIPO_TRASMISSIONE:
                    RetVal = Utils.Languages.GetLabelFromCode("VisibilityLabelTransmission", language) + star;
                    break;
                case DocumentoTipoDiritto.TIPO_TRASMISSIONE_IN_FASCICOLO:
                    RetVal = Utils.Languages.GetLabelFromCode("VisibilityLabelFolder", language) + star;
                    break;
                case DocumentoTipoDiritto.TIPO_SOSPESO:
                    RetVal = "SOSPESO" + star;
                    break;
                case DocumentoTipoDiritto.TIPO_CONSERVAZIONE:
                    RetVal = Utils.Languages.GetLabelFromCode("VisibilityLabelPreservation", language) + star;
                    break;
            }

            return RetVal;

        }

        /// <summary>
        /// Metodo per la generazione di una descrizione estesa del tipo diritto
        /// </summary>
        /// <param name="accessRight">Diritto di accesso</param>
        /// <returns>Descrizione del tipo di diritto</returns>
        protected String GetRightDescription(int accessRight)
        {
            String retVal = String.Empty;
            string language = UIManager.UserManager.GetUserLanguage();

            switch (accessRight)
            {
                case 0:
                case 255:
                case 63:
                    retVal = Utils.Languages.GetLabelFromCode("VisibilityLabelRW", language);
                    break;
                case 45:
                case 20:
                    retVal = Utils.Languages.GetLabelFromCode("VisibilityLabelReadOnly", language);
                    break;
                default:
                    break;

            }

            return retVal;

        }

        private void PopulateDdlSearchDocument()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            ListItem item = new ListItem();
            item.Text = Utils.Languages.GetLabelFromCode("ItemSearchDocumentRecords", language);
            item.Value = "0";
            this.SearchDocumentDdlIn.Items.Add(item);
            item = new ListItem();
            item.Text = Utils.Languages.GetLabelFromCode("ItemSearchDocumentNotRecords", language);
            item.Value = "1";
            this.SearchDocumentDdlIn.Items.Add(item);
            item = new ListItem();
            item.Text = Utils.Languages.GetLabelFromCode("ItemSearchDocumentType", language);
            item.Value = "2";
            this.SearchDocumentDdlIn.Items.Add(item);         
            this.SearchDocumentDdlIn.SelectedValue = "0";
        }

        protected void PopulateDDLRegistry(DocsPaWR.Ruolo role)
        {
            foreach (DocsPaWR.Registro reg in role.registri)
            {
                if (!reg.flag_pregresso)
                {
                    ListItem item = new ListItem();
                    item.Text = reg.codRegistro;
                    item.Value = reg.systemId;
                    this.DdlRegistries.Items.Add(item);
                }
            }

            //if (this.DdlRegistries.Items.Count == 1)
            //{
            //    this.UpPnlRegistry.Update();
            //}
        }

        private void CaricaTipologia(DropDownList ddl)
        {
            DocsPaWR.Templates[] listaTemplates;
            listaTemplates = DocumentManager.getTipoAttoTrasfDeposito(this, UserManager.GetInfoUser().idAmministrazione, false);
            ddl.Items.Clear();
            ddl.Items.Add("");
            int cont = 0;
            if (listaTemplates != null)
            {
                for (int i = 0; i < listaTemplates.Length; i++)
                {
                    DocsPaWR.Templates templ = listaTemplates[i];
                    if (templ.ABILITATO_SI_NO.Equals("1") && templ.IN_ESERCIZIO.ToUpper().Equals("SI"))
                    {
                        ddl.Items.Add(templ.DESCRIZIONE);
                        ddl.Items[cont + 1].Value = templ.SYSTEM_ID.ToString();
                        cont++;
                    }
                }
            }
        }

        protected void DocumentDdlTypeDocument_OnSelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(DocumentDdlTypeDocument.SelectedValue))
            {
                Session.Remove("template");
                panel_Contenuto.Controls.Clear();
                this.panel_Contenuto.Visible = false;
                this.pnl_RFAOO.Visible = false;
                this.pnlAnno.Visible = false;
                this.pnlNumero.Visible = false;
                phlSearchTypologyAttr.Visible = false;
                pnlSearchTypologyAttr.Update();
            }
            else
            {
                phlSearchTypologyAttr.Visible = true; 
                this.pnlAnno.Visible = true;
                this.panel_Contenuto.Visible = true;
                this.pnl_RFAOO.Visible = true;
                this.ddlAooRF.Visible = false;
                this.lblAooRF.Visible = false;
                this.pnlNumero.Visible = true;
                this.TxtAnno.Text = "";
                this.TxtNumero.Text = "";
                string idTemplate = DocumentDdlTypeDocument.SelectedValue;
                DocsPaWR.Templates templateInSessione = (DocsPaWR.Templates)Session["template"];
                if (!string.IsNullOrEmpty(idTemplate) && templateInSessione != null && !string.IsNullOrEmpty(templateInSessione.SYSTEM_ID.ToString()))
                {
                    if (DocumentDdlTypeDocument.SelectedValue != templateInSessione.SYSTEM_ID.ToString())
                    {
                        Session.Remove("template");
                        panel_Contenuto.Controls.Clear();
                    }
                    panel_Contenuto.Controls.Clear();
                }
                if (idTemplate != "")
                {
                    DocsPaWR.Templates template = ProfilerDocManager.getTemplateById(idTemplate);
                    if (template != null)
                    {
                        Session.Add("template", template);
                        ddlAooRF.Items.Clear();
                        inizializzaPanelContenuto();
                        this.TxtAnno.Visible = true;
                        this.TxtAnno.Text = "";
                        this.lblAnno.Visible = true;

                    }
                    else
                    {
                        pnl_RFAOO.Visible = false;
                    }
                }
            }
        }

        protected void SearchProjectRemove_Click(object sender, EventArgs e)
        {           
                Session.Remove("template");
                Session.Remove("idProfileVisibilitySearch");                  
                this.TxtAnno.Text = "";
                this.TxtIDDocument.Text = "";                
                this.DdlRegistries.ClearSelection();
                CaricaTipologia(this.DocumentDdlTypeDocument);
                this.TxtNumero.Text = "";
                this.TxtYear.Text = "";
                this.TxtNumProto.Text = "";
                SearchDocumentDdlIn.SelectedValue = "0";
                this.PhlSearchIDDocument.Visible = false;
                PhlSearchProto.Visible = true;
                this.phlSearchTypologyDoc.Visible = false; 
                LoadGridVisibility(null, 0);
                UpContainer.Update();              
            
        }
        //Recupera i contatori per una scelta tipologia di documento e li inserisce nella 
        //dropdownlist ddl_Contatori
        private void inizializzaPanelContenuto()
        {
            //pnl_RFAOO.Visible = false;
            if (Session["template"] != null)
            {
                DocsPaWR.Templates template = (DocsPaWR.Templates)Session["template"];
                table = new Table();
                table.ID = "table_Contatori";
                TableCell cell_2 = new TableCell();
                int numContatori = 0;
                string testoUnicoContatore = "";
                string idUnicoContatore = "";
                DocsPaWR.OggettoCustom oggettoUnico = null;
                ddl_Contatori = new DropDownList();
                ddl_Contatori.ID = "ddl_Contatori";
                //ddl_Contatori.Font.Size = FontUnit.Point(7);                
                ddl_Contatori.Font.Name = "Verdana";
                foreach (DocsPaWR.OggettoCustom oggetto in template.ELENCO_OGGETTI)
                {
                    //if (oggetto.REPERTORIO == "1")
                    //{
                    //rendo visibili i pannelli
                    if (oggetto.TIPO.DESCRIZIONE_TIPO == "Contatore")
                    {
                        if (oggetto.DESCRIZIONE.Equals(""))
                        {
                            return;
                        }
                        //testoUnicoContatore e idUnicoContatore servono nel caso in cui sia presente un solo
                        //contatore, in questo caso non visualizzo la dropdownlist ma una semplice label
                        testoUnicoContatore = oggetto.DESCRIZIONE.ToString();
                        idUnicoContatore = oggetto.SYSTEM_ID.ToString();
                        oggettoUnico = oggetto;
                        ddl_Contatori.Items.Add(new ListItem(oggetto.DESCRIZIONE.ToString(), oggetto.SYSTEM_ID.ToString()));
                        numContatori++;
                    }
                    //}
                }
                if (oggettoUnico != null)
                {
                    TableRow row = new TableRow();
                    row.ID = "row_Contatori";
                    TableCell cell_1 = new TableCell();
                    TableCell cell_3 = new TableCell();
                    if (numContatori > 1)
                    {
                        ListItem emptyCont = new ListItem();
                        emptyCont.Value = "";
                        emptyCont.Text = "";
                        ddl_Contatori.Items.Add(emptyCont);
                        ddl_Contatori.SelectedValue = "";

                        this.ddlAooRF.Visible = false;

                        cell_1.Controls.Add(ddl_Contatori);
                        ddl_Contatori.AutoPostBack = true;
                        this.ddl_Contatori.SelectedIndexChanged += new System.EventHandler(this.ddl_Contatori_SelectedIndexChanged);
                    }
                    else
                    {
                        Label lblContatore = new Label();
                        lblContatore.ID = "lblContatore";
                        //lblContatore.Font.Size = FontUnit.Point(7);
                        lblContatore.CssClass = "titolo_scheda";
                        lblContatore.Font.Name = "Verdana";
                        lblContatore.Text = testoUnicoContatore;
                        cell_1.Controls.Add(lblContatore);
                        Label lblContatoreID = new Label();
                        lblContatoreID.ID = "lblContID";
                        lblContatoreID.Text = idUnicoContatore;
                        lblContatoreID.Visible = false;
                        cell_3.Controls.Add(lblContatoreID);
                        ddl_Contatori.Visible = false;
                        if (ddlAooRF.SelectedIndex == -1)
                        {
                            //DocsPaWR.Ruolo ruoloUtente = UserManager.GetSelectedRole();
                            //DocsPaWR.Registro[] registriRfVisibili = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "", "");
                            DocsPaWR.Registro[] registriRfVisibili = RegistryManager.GetRegAndRFListInSession();
                            switch (oggettoUnico.TIPO_CONTATORE)
                            {
                                case "T":
                                    break;
                                case "A":
                                    lblAooRF.Text = "&nbsp;AOO";
                                    ////Aggiungo un elemento vuoto
                                    ListItem it = new ListItem();
                                    it.Value = "";
                                    it.Text = "";
                                    ddlAooRF.Items.Add(it);
                                    //Distinguo se è un registro o un rf
                                    for (int i = 0; i < registriRfVisibili.Length; i++)
                                    {
                                        ListItem item = new ListItem();
                                        if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "0")
                                        {
                                            item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                            item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                            ddlAooRF.Items.Add(item);
                                        }
                                    }
                                    ddlAooRF.Width = 100;
                                    this.pnl_RFAOO.Visible = true;
                                    break;
                                case "R":
                                    lblAooRF.Text = "&nbsp;RF";
                                    ////Aggiungo un elemento vuoto
                                    ListItem it_1 = new ListItem();
                                    it_1.Value = "";
                                    it_1.Text = "";
                                    ddlAooRF.Items.Add(it_1);
                                    //Distinguo se è un registro o un rf
                                    for (int i = 0; i < registriRfVisibili.Length; i++)
                                    {
                                        ListItem item = new ListItem();
                                        if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "1" && ((DocsPaWR.Registro)registriRfVisibili[i]).rfDisabled == "0")
                                        {
                                            item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                            item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                            ddlAooRF.Items.Add(item);
                                        }
                                    }
                                    ddlAooRF.Width = 100;
                                    this.pnl_RFAOO.Visible = true;
                                    break;
                            }
                        }
                    }
                    row.Cells.Add(cell_1);
                    if (cell_3 != null)
                        row.Cells.Add(cell_3);
                    row.Cells.Add(cell_2);
                    table.Rows.Add(row);

                    panel_Contenuto.Controls.Add(table);
                    this.panel_Contenuto.Visible = true;
                    this.phlSearchTypologyAttr.Visible = true;
                    pnlSearchTypologyAttr.Update();
                }
                //this.btn_ricerca.Visible = true;
            }
        }

        //Se il contatore è di tipo AOO o rf recupera la lista di AOO o la lista di rf 
        //e li inserisci nella dropdownlist ddlAooRF
        private void ddl_Contatori_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (ddl_Contatori.SelectedValue == "")
                this.lblAooRF.Visible = false;
            ddlAooRF.Items.Clear();
            //this.pnl_RFAOO.Visible = false;
            this.panel_Contenuto.Visible = true;
            this.pnl_RFAOO.Visible = true;
            this.pnlAnno.Visible = true;
            this.pnlNumero.Visible = true;
            this.TxtAnno.Text = "";
            Session["aoo_rf"] = "";

            Session.Remove("template");
            string idTemplate = DocumentDdlTypeDocument.SelectedValue;
            DocsPaWR.Templates template = ProfilerDocManager.getTemplateById(idTemplate);
            Session.Add("template", template);

            // DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
            foreach (DocsPaWR.OggettoCustom oggetto in template.ELENCO_OGGETTI)
            {
                if (oggetto.TIPO.DESCRIZIONE_TIPO == "Contatore")
                {
                    if (oggetto.DESCRIZIONE.Equals(""))
                    {
                        return;
                    }

                    if (oggetto.SYSTEM_ID.ToString().Equals(ddl_Contatori.SelectedItem.Value))
                    {
                        //DocsPaWR.Ruolo ruoloUtente = UserManager.GetSelectedRole();
                        //DocsPaWR.Registro[] registriRfVisibili = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "", "");
                        DocsPaWR.Registro[] registriRfVisibili = RegistryManager.GetRegAndRFListInSession();
                        this.pnl_RFAOO.Visible = false;
                        this.lblAooRF.Visible = true;
                        switch (oggetto.TIPO_CONTATORE)
                        {
                            case "T":
                                this.pnl_RFAOO.Visible = false;
                                break;
                            case "A":
                                lblAooRF.Text = "&nbsp;AOO";
                                ////Aggiungo un elemento vuoto
                                ListItem it = new ListItem();
                                it.Value = "";
                                it.Text = "";
                                ddlAooRF.Items.Add(it);
                                //Distinguo se è un registro o un rf
                                for (int i = 0; i < registriRfVisibili.Length; i++)
                                {
                                    ListItem item = new ListItem();
                                    if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "0")
                                    {
                                        item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                        item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                        ddlAooRF.Items.Add(item);
                                    }
                                }
                                ddlAooRF.Width = 100;
                                this.pnl_RFAOO.Visible = true;
                                this.ddlAooRF.Visible = true;
                                break;
                            case "R":
                                lblAooRF.Text = "&nbsp;RF";
                                ////Aggiungo un elemento vuoto
                                ListItem it_1 = new ListItem();
                                it_1.Value = "";
                                it_1.Text = "";
                                ddlAooRF.Items.Add(it_1);
                                //Distinguo se è un registro o un rf
                                for (int i = 0; i < registriRfVisibili.Length; i++)
                                {
                                    ListItem item = new ListItem();
                                    if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "1" && ((DocsPaWR.Registro)registriRfVisibili[i]).rfDisabled == "0")
                                    {
                                        item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                        item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                        ddlAooRF.Items.Add(item);
                                    }
                                }
                                ddlAooRF.Width = 100;
                                this.pnl_RFAOO.Visible = true;
                                this.ddlAooRF.Visible = true;
                                break;
                        }

                    }
                    else
                    {
                        // poichè la ricerca deve essere fatta per un solo contatore, metto a
                        // stringa vuota il valore di tutti gli altri oggetti del template
                        oggetto.VALORE_DATABASE = string.Empty;
                    }
                }
            }
        }
      
        protected void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.VisibilityRemove.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('VisibilityRemove','');", true);
                if (Session["idProfileVisibilitySearch"] != null)
                    this.LoadGridVisibility(null, Convert.ToInt32(Session["idProfileVisibilitySearch"]));
                this.UpContainer.Update();
            }
        }


        #region Sessions
 

        protected DocumentoDiritto[] VisibilityList
        {
            get
            {
                DocumentoDiritto[] result = null;
                if (HttpContext.Current.Session["visibilityList"] != null)
                {
                    result = HttpContext.Current.Session["visibilityList"] as DocumentoDiritto[];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["visibilityList"] = value;
            }
        }

        protected List<DocumentsVisibility> DocumentsVisibilityList
        {
            get
            {
                List<DocumentsVisibility> result = null;
                if (HttpContext.Current.Session["documentsVisibility"] != null)
                {
                    result = HttpContext.Current.Session["documentsVisibility"] as List<DocumentsVisibility>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["documentsVisibility"] = value;
            }
        }
        protected List<DocumentsVisibility> DocumentsVisibilityFilters
        {
            get
            {
                List<DocumentsVisibility> result = null;
                if (HttpContext.Current.Session["documentsVisibilityFilter"] != null)
                {
                    result = HttpContext.Current.Session["documentsVisibilityFilter"] as List<DocumentsVisibility>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["documentsVisibilityFilter"] = value;
            }
        }

        protected GridViewRow RowSelected
        {
            get
            {
                return HttpContext.Current.Session["RowSelected"] as GridViewRow;
            }
            set
            {
                HttpContext.Current.Session["RowSelected"] = value;
            }
        }

        private bool AbortDocument
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["abortDocument"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["abortDocument"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["abortDocument"] = value;
            }
        }

        private string isPersonOrGroup
        {
            get
            {
                return HttpContext.Current.Session["isPersonOrGroup"] as string;
            }
            set
            {
                HttpContext.Current.Session["isPersonOrGroup"] = value;
            }
        }

        #endregion


    }
}