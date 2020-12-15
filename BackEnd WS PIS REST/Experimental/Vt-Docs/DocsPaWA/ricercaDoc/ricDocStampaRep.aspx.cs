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
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;
using System.Collections.Generic;
using System.Linq;

namespace DocsPAWA.ricercaDoc
{
    public partial class ricDocStampaRep : DocsPAWA.CssPage
    {
        protected RegistroRepertorio[] repertori;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
        protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
        private const string KEY_SCHEDA_RICERCA = "StampaRep";
        protected string numResult;
        public SchedaRicerca schedaRicerca = null;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            string idGruppo = UserManager.getInfoUtente(this).idGruppo;
            repertori = RegistriRepertorioUtils.GetRegistriesWithAooOrRfSup(idGruppo, idGruppo);

            if (!Page.IsPostBack)
            {
                this.AddControlsClientAttribute();

                this.setListaRepertori();

                this.FillComboFilterTypes();                
            }

            if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Document)
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);

            if (Request.QueryString["numRes"] != string.Empty && Request.QueryString["numRes"] != null)
                this.numResult = Request.QueryString["numRes"];
            else
                this.numResult = string.Empty;
            
            this.InitRangeFilterItems();

            this.EnableRangeFilterControls(this.cboFilterTypeNumRepertorio);
            this.EnableRangeFilterControls(this.cboFilterTypeDataStampa);
            this.tastoInvio();

            schedaRicerca = (SchedaRicerca)Session[SchedaRicerca.SESSION_KEY];
            if (schedaRicerca == null)
            {
                DocsPAWA.DocsPaWR.Utente userHome = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
                DocsPAWA.DocsPaWR.Ruolo userRuolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];
                schedaRicerca = new SchedaRicerca(KEY_SCHEDA_RICERCA, userHome, userRuolo, this);
                Session[SchedaRicerca.SESSION_KEY] = schedaRicerca;
            }

            schedaRicerca.Pagina = this;

            if (!Page.IsPostBack && schedaRicerca != null && schedaRicerca.FiltriRicerca != null)
            {
                PopulateField(schedaRicerca.FiltriRicerca);
                if (Ricerca())
                {
                    if (String.IsNullOrEmpty(ddl_repertori.SelectedValue))
                    {
                        Response.Write("<script>alert('Selezionare un repertorio');top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }

                    string altro = string.Empty;
                    if (!string.IsNullOrEmpty(this.numResult) && this.numResult.Equals("0"))
                        altro = "&noRic=1";

                    DocumentManager.setFiltroRicDoc(this, qV);
                    DocumentManager.removeDatagridDocumento(this);
                    DocumentManager.removeListaNonDocProt(this);
                    ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?from=StampaReg&tabRes=StampaReg" + altro + "';", true);

                }
            }                            
        }

        #region Gestione javascript

        /// <summary>
        /// Associazione funzioni javascript agli eventi client dei controlli
        /// </summary>
        private void AddControlsClientAttribute()
        {
            this.txtInitNumRepertorio.Attributes.Add("onKeyPress", "ValidateNumericKey();");
            this.txtEndNumRepertorio.Attributes.Add("onKeyPress", "ValidateNumericKey();");
            this.txtAnnoRepertorio.Attributes.Add("onKeyPress", "ValidateNumericKey();");
        }

        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }

        #endregion

        #region Gestione filtri stampa repertorio
        
        // Costanti che definiscono le tipologie di filtro disponibili
        private const string RANGE_FILTER_TYPE_INTERVAL = "I";
        private const string RANGE_FILTER_TYPE_SINGLE = "S";

        private ListItem[] GetListItemsTipiSelezione()
        {
            ListItem[] items = new ListItem[2];
            items[0] = new ListItem("Valore singolo", RANGE_FILTER_TYPE_SINGLE);
            items[1] = new ListItem("Intervallo", RANGE_FILTER_TYPE_INTERVAL);
            return items;
        }

        private DocsPAWA.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (DocsPAWA.UserControls.Calendar)this.FindControl(controlId);
        }

        private void FillComboFilterTypes()
        {
            this.cboFilterTypeNumRepertorio.Items.AddRange(this.GetListItemsTipiSelezione());
            this.cboFilterTypeDataStampa.Items.AddRange(this.GetListItemsTipiSelezione());
        }

        private Hashtable _rangeFilterItems = null;

        private void InitRangeFilterItems()
        {
            this._rangeFilterItems = new Hashtable();

            this._rangeFilterItems.Add(this.cboFilterTypeNumRepertorio,
                this.CreateRangeFilterInnerHT(this.lblInitNumRepertorio,
                                                this.txtInitNumRepertorio,
                                                this.lblEndNumRepertorio,
                                                this.txtEndNumRepertorio));


            this._rangeFilterItems.Add(this.cboFilterTypeDataStampa,
                this.CreateRangeFilterInnerHT(this.lblInitDataStampa,
                                                this.GetCalendarControl("txtInitDataStampa").txt_Data,
                                                this.lblEndDataStampa,
                                                this.GetCalendarControl("txtEndDataStampa").txt_Data));
        }

        private void DisposeRangeFilterItems()
        {
            this._rangeFilterItems.Clear();
            this._rangeFilterItems = null;
        }

        private Hashtable CreateRangeFilterInnerHT(Label initLabel, TextBox initText, Label endLabel, TextBox endText)
        {
            Hashtable retValue = new Hashtable();
            retValue.Add("INIT_LABEL", initLabel);
            retValue.Add("INIT_TEXT", initText);
            retValue.Add("END_LABEL", endLabel);
            retValue.Add("END_TEXT", endText);
            return retValue;
        }

        private void EnableRangeFilterControls(DropDownList cboFilterType)
        {
            bool intervalFilterEnabled = (cboFilterType.SelectedValue == RANGE_FILTER_TYPE_INTERVAL);
            Hashtable innerHT = (Hashtable)this._rangeFilterItems[cboFilterType];

            Label initLabel = (Label)innerHT["INIT_LABEL"];
            TextBox initText = (TextBox)innerHT["INIT_TEXT"];
            Label endLabel = (Label)innerHT["END_LABEL"];
            TextBox endText = (TextBox)innerHT["END_TEXT"];

            initLabel.Visible = intervalFilterEnabled;
            initText.Visible = true;
            endLabel.Visible = intervalFilterEnabled;
            endText.Visible = intervalFilterEnabled;
        }

        protected void cboFilterTypeNumRepertorio_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableRangeFilterControls((DropDownList)sender);

            if (this.cboFilterTypeDataStampa.SelectedIndex == 0)
            {
                this.GetCalendarControl("txtEndDataStampa").Visible = false;
                this.GetCalendarControl("txtEndDataStampa").btn_Cal.Visible = false;
                this.GetCalendarControl("txtEndDataStampa").txt_Data.Visible = false;
            }
            else
            {
                this.GetCalendarControl("txtEndDataStampa").Visible = true;
                this.GetCalendarControl("txtEndDataStampa").btn_Cal.Visible = true;
                this.GetCalendarControl("txtEndDataStampa").txt_Data.Visible = true;
            }
        }

        protected void cboFilterTypeDataStampa_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableRangeFilterControls((DropDownList)sender);

            if (this.cboFilterTypeDataStampa.SelectedIndex == 0)
            {
                this.GetCalendarControl("txtEndDataStampa").Visible = false;
                this.GetCalendarControl("txtEndDataStampa").btn_Cal.Visible = false;
                this.GetCalendarControl("txtEndDataStampa").txt_Data.Visible = false;
            }
            else
            {
                this.GetCalendarControl("txtEndDataStampa").Visible = true;
                this.GetCalendarControl("txtEndDataStampa").btn_Cal.Visible = true;
                this.GetCalendarControl("txtEndDataStampa").txt_Data.Visible = true;
            }
        }

        #endregion

        #region ddl repertori - RF / AOO
        
        private void setListaRepertori()
        {
            if (repertori != null)
            {
                ddl_repertori.Items.Clear();
                ListItem item = new ListItem();
                ddl_repertori.Items.Add(item);
                foreach (RegistroRepertorio rep in repertori)
                {
                    item = new ListItem(rep.TipologyDescription, rep.CounterId);
                    ddl_repertori.Items.Add(item);
                }
            }
        }

        protected void ddl_repertori_SelectedIndexChanged(object sender, EventArgs e)
        {
            pupoluateDdlRfAoo();
        }

        protected void pupoluateDdlRfAoo()
        {
            if (repertori != null && repertori.Length != 0 && !String.IsNullOrEmpty(ddl_repertori.SelectedValue))
            {
                RegistroRepertorio repertorio = repertori.Where(rep => rep.CounterId.Equals(ddl_repertori.SelectedValue)).FirstOrDefault();
                if (repertorio != null && repertorio.SingleSettings.Length != 0)
                {
                    ddl_aoo_rf.Items.Clear();
                    ddl_aoo_rf.Items.Add(new ListItem("Tutti", "TUTTI"));
                    foreach (RegistroRepertorioSingleSettings r in repertorio.SingleSettings)
                    {
                        if (!String.IsNullOrEmpty(r.RegistryId))
                        {
                            ListItem item = new ListItem(r.RegistryOrRfDescription, r.RegistryId);
                            ddl_aoo_rf.Items.Add(item);
                        }
                        if (!String.IsNullOrEmpty(r.RFId))
                        {
                            ListItem item = new ListItem(r.RegistryOrRfDescription, r.RFId);
                            ddl_aoo_rf.Items.Add(item);
                        }
                    }
                }
            }
            else
            {
                ddl_aoo_rf.Items.Clear();
            }
        }

        #endregion
                
        #region Utility

        public void tastoInvio()
        {
            Utils.DefaultButton(this, ref txtInitNumRepertorio, ref butt_ricerca);
            Utils.DefaultButton(this, ref txtEndNumRepertorio, ref butt_ricerca);
            Utils.DefaultButton(this, ref txtAnnoRepertorio, ref butt_ricerca);
            Utils.DefaultButton(this, ref this.GetCalendarControl("txtInitDataStampa").txt_Data, ref butt_ricerca);
            Utils.DefaultButton(this, ref this.GetCalendarControl("txtEndDataStampa").txt_Data, ref butt_ricerca);
        }

        protected void PopulateField(DocsPAWA.DocsPaWR.FiltroRicerca[][] qV)
        {
            try
            {
                if (qV != null || qV.Length > 0)
                {
                    DocsPaWR.FiltroRicerca[] filters = qV[0];

                    foreach (DocsPAWA.DocsPaWR.FiltroRicerca aux in filters)
                    {
                        if (aux.argomento == DocsPaWR.FiltriStampaRegistro.ID_REPERTORIO.ToString())
                        {
                            this.ddl_repertori.SelectedValue = aux.valore;
                            pupoluateDdlRfAoo();
                        }

                        if (aux.argomento == DocsPaWR.FiltriDocumento.REP_RF_AOO.ToString() && aux.valore != "0")
                        {
                            this.ddl_aoo_rf.SelectedValue = aux.valore;
                        }

                        if (aux.argomento == DocsPaWR.FiltriStampaRegistro.ANNO_REP_STAMPA.ToString())
                        {
                            this.txtAnnoRepertorio.Text = aux.valore;
                        }

                        if (aux.argomento == DocsPaWR.FiltriStampaRegistro.NUM_REP_STAMPA_DAL.ToString())
                        {
                            this.cboFilterTypeNumRepertorio.SelectedValue = RANGE_FILTER_TYPE_INTERVAL;
                            this.txtInitNumRepertorio.Text = aux.valore;
                        }

                        if (aux.argomento == DocsPaWR.FiltriStampaRegistro.NUM_REP_STAMPA.ToString())
                        {
                            this.txtInitNumRepertorio.Text = aux.valore;
                        }

                        if (aux.argomento == DocsPaWR.FiltriStampaRegistro.NUM_REP_STAMPA_AL.ToString())
                        {
                            this.cboFilterTypeNumRepertorio.SelectedValue = RANGE_FILTER_TYPE_INTERVAL;
                            this.txtEndNumRepertorio.Text = aux.valore;
                            this.txtEndNumRepertorio.Visible = true;
                        }

                        if (aux.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REP_DAL.ToString())
                        {
                            this.cboFilterTypeDataStampa.SelectedValue = RANGE_FILTER_TYPE_INTERVAL;
                            this.GetCalendarControl("txtInitDataStampa").txt_Data.Text = aux.valore;
                        }

                        if (aux.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REP.ToString())
                        {
                            this.GetCalendarControl("txtInitDataStampa").txt_Data.Text = aux.valore;
                        }

                        if (aux.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REP_AL.ToString())
                        {
                            this.cboFilterTypeDataStampa.SelectedValue = RANGE_FILTER_TYPE_INTERVAL;
                            this.GetCalendarControl("txtEndDataStampa").txt_Data.Text = aux.valore;
                            this.GetCalendarControl("txtEndDataStampa").Visible = true;
                        }

                        if (aux.argomento == DocsPaWR.FiltriStampaRegistro.REP_FIRMATO.ToString())
                        {
                            if(aux.valore.Equals("1"))
                                this.rbl_TipiStampe.SelectedValue = "FIRMATE";
                            if(aux.valore.Equals("0"))
                                this.rbl_TipiStampe.SelectedValue = "NON_FIRMATE";                            
                        }
                    }
                }
            }

            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        #endregion Utility

        #region Ricerca

        private void AddFilterNumRepertorio(ref DocsPAWA.DocsPaWR.FiltroRicerca[] filterItems)
        {
            bool rangeFilterInterval = (this.cboFilterTypeNumRepertorio.SelectedValue == RANGE_FILTER_TYPE_INTERVAL);

            DocsPaWR.FiltroRicerca filterItem = null;

            if (this.txtInitNumRepertorio.Text.Length > 0)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();

                if (rangeFilterInterval)
                    filterItem.argomento = DocsPaWR.FiltriStampaRegistro.NUM_REP_STAMPA_DAL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriStampaRegistro.NUM_REP_STAMPA.ToString();

                filterItem.valore = this.txtInitNumRepertorio.Text;
                filterItems = Utils.addToArrayFiltroRicerca(filterItems, filterItem);
                filterItem = null;
            }

            if (rangeFilterInterval && this.txtEndNumRepertorio.Text.Length > 0)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriStampaRegistro.NUM_REP_STAMPA_AL.ToString();
                filterItem.valore = this.txtEndNumRepertorio.Text;
                filterItems = Utils.addToArrayFiltroRicerca(filterItems, filterItem);
                filterItem = null;
            }

            if (this.txtAnnoRepertorio.Text.Length > 0)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriStampaRegistro.ANNO_REP_STAMPA.ToString();
                filterItem.valore = this.txtAnnoRepertorio.Text;
                filterItems = Utils.addToArrayFiltroRicerca(filterItems, filterItem);
                filterItem = null;
            }
        }

        private void AddFilterDataStampa(ref DocsPAWA.DocsPaWR.FiltroRicerca[] filterItems)
        {
            bool rangeFilterInterval = (this.cboFilterTypeDataStampa.SelectedValue == RANGE_FILTER_TYPE_INTERVAL);

            DocsPaWR.FiltroRicerca filterItem = null;

            if (this.GetCalendarControl("txtInitDataStampa").txt_Data.Text.Length > 0)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();

                if (rangeFilterInterval)
                    filterItem.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REP_DAL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REP.ToString();

                filterItem.valore = this.GetCalendarControl("txtInitDataStampa").txt_Data.Text;
                filterItems = Utils.addToArrayFiltroRicerca(filterItems, filterItem);
                filterItem = null;
            }

            if (rangeFilterInterval && this.GetCalendarControl("txtEndDataStampa").txt_Data.Text.Length > 0)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REP_AL.ToString();
                filterItem.valore = this.GetCalendarControl("txtEndDataStampa").txt_Data.Text;
                filterItems = Utils.addToArrayFiltroRicerca(filterItems, filterItem);
                filterItem = null;
            }
        }

        protected bool Ricerca()
        {
            try
            {
                qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
                fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];

                //ID_REPERTORIO
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriStampaRegistro.ID_REPERTORIO.ToString();
                fV1.valore = ddl_repertori.SelectedValue;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                //REPERTORIO
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.REP_RF_AOO.ToString();

                if (!String.IsNullOrEmpty(ddl_aoo_rf.SelectedValue))
                {
                    if (ddl_aoo_rf.SelectedValue.ToUpper().Equals("TUTTI"))
                    {
                        string valore = string.Empty;
                        foreach (ListItem item in ddl_aoo_rf.Items)
                        {
                            if(!item.Value.ToUpper().Equals("TUTTI"))
                                valore += item.Value +",";
                        }
                        if (valore.EndsWith(","))
                            valore = valore.Substring(0, valore.Length - 1);
                        fV1.valore = valore;
                    }
                    else
                    {
                        fV1.valore = ddl_aoo_rf.SelectedValue;
                    }
                }
                else
                {
                    fV1.valore = "0";
                }

                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                //TIPO DOCUMENTO
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                fV1.valore = "C";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                //NUMERO E ANNO REPERTORIO
                this.AddFilterNumRepertorio(ref fVList);

                //DATA STAMPA REPERTORIO
                this.AddFilterDataStampa(ref fVList);

                //FIRMATO
                if (rbl_TipiStampe.SelectedValue == "FIRMATE")
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = FiltriStampaRegistro.REP_FIRMATO.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (rbl_TipiStampe.SelectedValue == "NON_FIRMATE")
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = FiltriStampaRegistro.REP_FIRMATO.ToString();
                    fV1.valore = "0";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                //ABBATANGELI GIANLUIGI - Filtro per nascondere doc di altre applicazioni
                if (System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"] != null && !System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"].Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.COD_EXT_APP.ToString();
                    fV1.valore = (System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"]);
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                // Reperimento del filtro da utilizzare per la griglia
                List<FiltroRicerca> filterList;

                if (GridManager.SelectedGrid.FieldForOrder != null)
                {
                    Field field = GridManager.SelectedGrid.FieldForOrder;
                    filterList = GridManager.GetOrderFilterForDocument(
                        field.FieldId,
                        GridManager.SelectedGrid.OrderDirection.ToString());
                }
                else
                    filterList = GridManager.GetOrderFilterForDocument(String.Empty, "DESC");

                // Se la lista è valorizzata vengono aggiunti i filtri
                if (filterList != null)
                    foreach (FiltroRicerca filter in filterList)
                        fVList = Utils.addToArrayFiltroRicerca(fVList, filter);

                qV[0]=fVList;

                DocumentManager.setFiltroRicDoc(this, qV);
                DocumentManager.removeDatagridDocumento(this);
                DocumentManager.removeListaDocProt(this);	

                return true;
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
                return false;
            }
        }

        protected void butt_ricerca_Click(object sender, EventArgs e)
        {
            ArrayList validationItems=null;
			string firstInvalidControlID=string.Empty;

            if (this.IsValidData(out validationItems, out firstInvalidControlID))
            {
                if (Ricerca())
                {
                    schedaRicerca.FiltriRicerca = qV;
                    DocumentManager.setFiltroRicDoc(this, qV);
                    DocumentManager.removeDatagridDocumento(this);
                    DocumentManager.removeListaNonDocProt(this);
                    Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?tabRes=StampaRep';</script>");
                }
            }
            else
            {
                string validationMessage = string.Empty;

                foreach (string item in validationItems)
                {
                    if (validationMessage != string.Empty)
                        validationMessage += @"\n";

                    validationMessage += " - " + item;
                }

                if (validationMessage != string.Empty)
                    validationMessage = "Sono state rilevate le seguenti incongruenze: " +
                        @"\n" + @"\n" + validationMessage;

                this.RegisterClientScript("ValidationMessage", "alert('" + validationMessage + "');");
                
                Response.Write("<script language='javascript'>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
            }
        }

        #endregion

        #region Validazione

        private bool IsValidData(out ArrayList validationItems, out string firstInvalidControlID)
        {
            validationItems = new ArrayList();
            firstInvalidControlID = string.Empty;

            // Validazione selezione repertorio
            if (String.IsNullOrEmpty(ddl_repertori.SelectedValue))
                validationItems.Add("Selezionare un repertorio");
                
            // Validazione numero repertorio
            this.ValidateNumericRange("Numero repertorio", this.txtInitNumRepertorio, this.txtEndNumRepertorio, validationItems, ref firstInvalidControlID);

            // Validazione anno immesso
            if (this.txtAnnoRepertorio.Text != string.Empty && !this.IsValidNumber(this.txtAnnoRepertorio))
                validationItems.Add("Anno repertorio non valido");

            // Validazione data stampa
            this.ValidateDateRange("Data stampa", this.GetCalendarControl("txtInitDataStampa").txt_Data, this.GetCalendarControl("txtEndDataStampa").txt_Data, validationItems, ref firstInvalidControlID);

            return (validationItems.Count == 0);
        }

        private void ValidateNumericRange(string fieldName, TextBox initText, TextBox endText, ArrayList validationItems, ref string firstInvalidControlID)
        {
            bool isValidInitNumber = false;
            bool isValidEndNumber = false;

            if (initText.Text.Length > 0)
            {
                isValidInitNumber = this.IsValidNumber(initText);

                if (!isValidInitNumber)
                {
                    validationItems.Add(fieldName + " non valido");

                    if (firstInvalidControlID == string.Empty)
                        firstInvalidControlID = initText.ID;
                }
            }

            if (endText.Visible && endText.Text.Length > 0)
            {
                if (endText.Visible)
                {
                    isValidEndNumber = this.IsValidNumber(endText);

                    if (!isValidEndNumber)
                    {
                        validationItems.Add(fieldName + " non valido");

                        if (firstInvalidControlID == string.Empty)
                            firstInvalidControlID = endText.ID;
                    }
                }
            }

            if (initText.Text.Length > 0 || (endText.Visible && endText.Text.Length > 0))
            {
                if (initText.Text.Equals(string.Empty))
                    validationItems.Add(fieldName + " iniziale mancante");

                else if (endText.Visible && endText.Text.Equals(string.Empty))
                    validationItems.Add(fieldName + " finale mancante");
            }

            // Validazione range di dati
            if (isValidInitNumber && isValidEndNumber &&
                int.Parse(initText.Text) > int.Parse(endText.Text))
            {
                validationItems.Add(fieldName + " iniziale maggiore di quello finale");

                if (firstInvalidControlID == string.Empty)
                    firstInvalidControlID = endText.ID;
            }

        }

        private void ValidateDateRange(string fieldName, DocsPaWebCtrlLibrary.DateMask initDate, DocsPaWebCtrlLibrary.DateMask endDate, ArrayList validationItems, ref string firstInvalidControlID)
        {
            bool isValidInitDate = false;
            bool isValidEndDate = false;

            if (initDate.Text.Length > 0)
            {
                // Validazione data iniziale
                isValidInitDate = this.IsValidDate(initDate);

                if (!isValidInitDate)
                {
                    validationItems.Add(fieldName + " iniziale non valida");

                    if (firstInvalidControlID == string.Empty)
                        firstInvalidControlID = initDate.ID;
                }
            }

            if (endDate.Visible && endDate.Text.Length > 0)
            {
                // Validazione data finale
                isValidEndDate = this.IsValidDate(endDate);

                if (!isValidEndDate)
                {
                    validationItems.Add(fieldName + " finale non valida");

                    if (firstInvalidControlID == string.Empty)
                        firstInvalidControlID = endDate.ID;
                }
            }

            if (initDate.Text.Length > 0 || (endDate.Visible && endDate.Text.Length > 0))
            {
                if (initDate.Text.Equals(string.Empty))
                    validationItems.Add(fieldName + " iniziale mancante");

                else if (endDate.Visible && endDate.Text.Equals(string.Empty))
                    validationItems.Add(fieldName + " finale mancante");
            }

            // Validazione range di dati
            if (isValidInitDate && isValidEndDate && DateTime.Parse(initDate.Text) > DateTime.Parse(endDate.Text))
            {
                validationItems.Add(fieldName + " iniziale maggiore di quella finale");

                if (firstInvalidControlID == string.Empty)
                    firstInvalidControlID = endDate.ID;
            }
        }

        private bool IsValidNumber(TextBox numberText)
        {
            bool retValue = true;

            try
            {
                int.Parse(numberText.Text);
            }
            catch
            {
                retValue = false;
            }

            return retValue;
        }

        private bool IsValidDate(DocsPaWebCtrlLibrary.DateMask dateMask)
        {
            bool retValue = false;

            if (dateMask.Text.Length > 0)
                retValue = DocsPAWA.Utils.isDate(dateMask.Text);

            return retValue;
        }
        
        #endregion
    }
}