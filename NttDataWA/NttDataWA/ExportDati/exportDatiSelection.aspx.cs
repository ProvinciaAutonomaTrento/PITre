using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;


namespace NttDataWA.ExportDati
{
    public partial class exportDatiSelection : System.Web.UI.Page
    {
        #region Constant

        private const string PDF = "PDF";
        private const string XLS = "XLS";
        private const string ODS = "ODS";
        private const string DOC = "doc";
        private const string FASC = "fasc";
        private const string EXP_DOC_IN_CEST = "docInCest";
        private const string EXP_DOC_IN_FASC = "docInfasc";
        private const string EXP_TRASM = "trasm";
        private const string EXP_TO_DO_LIST = "toDoList";
        private const string EXP_NOTIFY = "notify";
        private const string EXP_VISIBILITA_PROCESSI = "exportVisibilita";
        #endregion

        #region Fields

        public static string componentType = Constans.TYPE_SMARTCLIENT;

        #endregion

        #region Properties

        private bool IsFasc
        {
            get
            {
                return Request.QueryString["objType"].Equals("D") ? false : true;
            }
        }

        protected Dictionary<String, String> ListCheck
        {
            get
            {
                Dictionary<String, String> result = null;
                if (HttpContext.Current.Session["listCheck"] != null)
                {
                    result = HttpContext.Current.Session["listCheck"] as Dictionary<String, String>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listCheck"] = value;
            }
        }

        public bool ExportDaModello
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["ExportDaModello"] != null)
                {
                    result = (bool)HttpContext.Current.Session["ExportDaModello"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ExportDaModello"] = value;
            }
        }

        public bool ExportEnable
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["ExportEnable"] != null)
                {
                    result = (bool)HttpContext.Current.Session["ExportEnable"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ExportEnable"] = value;
            }
        }


        public bool docInFasc
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session[EXP_DOC_IN_FASC] != null)
                {
                    result = (bool)HttpContext.Current.Session[EXP_DOC_IN_FASC];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session[EXP_DOC_IN_FASC] = value;
            }
        }

        /// <summary>
        /// Lista dei filtri di ricerca
        /// </summary>
        private FiltroRicerca[][] SearchFilters
        {
            get
            {
                return HttpContext.Current.Session["filtroRicerca"] as FiltroRicerca[][];
            }

            set
            {
                HttpContext.Current.Session["filtroRicerca"] = value;
            }
        }
        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            componentType = UserManager.getComponentType(Request.UserAgent);
            this.InitApplet();

            if (!IsPostBack)
            {
                this.InitializePage();

                this.hd_export.Value = Request.QueryString["export"];

                if (!string.IsNullOrEmpty(Request.QueryString["docOrFasc"]))
                {
                    this.hd_export.Value = EXP_DOC_IN_FASC;
                }

                if (this.hd_export.Value.Equals("rubrica") || this.hd_export.Value.Equals("searchAddressBook"))
                {
                  
                    //nel caso dell'export della rubrica non si selezionano i campi e 
                    //l'export è solo in excel.
                    this.panel_listaCampi.Visible = false;
                    this.lbl_selezionaCampo.Visible = false;
                    this.cb_selezionaTutti.Visible = false;
                    this.rbl_XlsOrPdf.SelectedIndex = 1;
                    //this.rbl_XlsOrPdf.Items.RemoveAt(2);
                    this.rbl_XlsOrPdf.Items.RemoveAt(0);

                    this.txt_titolo.Visible = false;
                    this.litAssociateTitle.Visible = false;
                }
                else
                {
                    this.lbl_selezionaCampo.Visible = true;
                    this.cb_selezionaTutti.Visible = true;
                    this.rbl_XlsOrPdf.SelectedIndex = 0;
                    this.rbl_XlsOrPdf.Enabled = true;
                    this.caricaGridViewAndControls();
                }

                if (this.ExportDaModello)
                {
                    DocsPaWR.FiltroRicerca[][] filtri = null;
                    if (DocumentManager.getFiltroRicDoc() != null)
                        filtri = DocumentManager.getFiltroRicDoc();
                    else
                        filtri = DocumentManager.getMemoriaFiltriRicDoc();

                    if (filtri != null)
                    {
                        foreach (DocsPaWR.FiltroRicerca[] filtroUno in filtri)
                        {
                            foreach (DocsPaWR.FiltroRicerca filtroDue in filtroUno)
                            {
                                switch (filtroDue.argomento)
                                {
                                    case "TIPO_ATTO":
                                        ListItem modello = new ListItem();
                                        modello.Value = "Model";
                                        modello.Text = "Modello";
                                        if (this.rbl_XlsOrPdf.Items.Count == 3)
                                            this.rbl_XlsOrPdf.Items.Add(modello);
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            this.RefreshScript();
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.CloseMask(false);
        }

        protected void BtnExport_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.Export();
        }

        protected void rbl_XlsOrPdf_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.caricaGridViewAndControls();
        }

        protected void cb_selezionaTutti_CheckedChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            if (this.cb_selezionaTutti.Checked)
            {
                for (int i = 0; i < this.gv_listaCampi.Rows.Count; i++)
                    ((System.Web.UI.WebControls.CheckBox)this.gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = true;
            }
            else
            {
                for (int i = 0; i < this.gv_listaCampi.Rows.Count; i++)
                    ((System.Web.UI.WebControls.CheckBox)this.gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = false;
            }
        }

        #endregion

        #region Methods

        protected void LoadKeys()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_EXPORT_DA_MODELLO.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_EXPORT_DA_MODELLO.ToString()) == "1")
            {
                this.ExportDaModello = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.GV_EXPORT_ENABLE.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.GV_EXPORT_ENABLE.ToString()] == "1")
            {
                this.ExportEnable = true;
            }
        }

        private void CloseMask(bool withReturnValue)
        {
            string retValue = withReturnValue ? "true" : "false";
            if (Request.QueryString["export"] != null && (Request.QueryString["export"] == "rubrica" || Request.QueryString["export"] == "exportVisibilita"))
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('ExportDati', '" + retValue + "', parent);", true);
            else if (Request.QueryString["export"] == "searchAddressBook")
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('ExportSearch', '" + retValue + "', parent);", true);
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('ExportDati', '" + retValue + "');", true);
        }

        protected void InitializePage()
        {
            this.InitializeLanguage();
            this.InitializeList();
            this.LoadKeys();
        }

        private void InitApplet()
        {
            if (componentType == Constans.TYPE_APPLET || componentType == Constans.TYPE_SOCKET)
                this.plcApplet.Visible = true;
            else
            {
                Control ShellWrapper = Page.LoadControl("../ActivexWrappers/ShellWrapper.ascx");
                this.plcActiveX.Controls.Add(ShellWrapper);

                Control AdoStreamWrapper = Page.LoadControl("../ActivexWrappers/AdoStreamWrapper.ascx");
                this.plcActiveX.Controls.Add(AdoStreamWrapper);

                Control FsoWrapper = Page.LoadControl("../ActivexWrappers/FsoWrapper.ascx");
                this.plcActiveX.Controls.Add(FsoWrapper);

                this.plcActiveX.Visible = true;
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnExport.Text = Utils.Languages.GetLabelFromCode("GenericBtnExport", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.litSelectFormat.Text = Utils.Languages.GetLabelFromCode("ExportDatiSelectFormat", language);
            this.litAssociateTitle.Text = Utils.Languages.GetLabelFromCode("ExportDatiAssociateTitle", language);
            this.lbl_selezionaCampo.Text = Utils.Languages.GetLabelFromCode("ExportDatiSelectField", language);
            this.cb_selezionaTutti.Text = Utils.Languages.GetLabelFromCode("ExportDatiSelectAll", language);
            this.gv_listaCampi.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("ExportDatiGridField0", language);
            this.gv_listaCampi.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("ExportDatiGridField1", language);
            this.gv_listaCampi.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("ExportDatiGridField2", language);
            this.gv_listaCampi.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("ExportDatiGridField3", language);
            this.gv_listaCampi.Columns[4].HeaderText = Utils.Languages.GetLabelFromCode("ExportDatiGridField4", language);
        }

        public void InitializeList()
        {
            if (this.ListCheck != null)
            {
                Dictionary<String, MassiveOperationTarget> temp = new Dictionary<string, MassiveOperationTarget>();

                // Inizializzazione della mappa con i system id degli oggetti e lo stato
                // di checking (in fase di inizializzazione tutti gli item sono deselezionati)
                foreach (KeyValuePair<string, string> item in this.ListCheck)
                    if (!temp.Keys.Contains(item.Key))
                        temp.Add(item.Key, new MassiveOperationTarget(item.Key, item.Value));

                // Salvataggio del dizionario
                MassiveOperationUtils.ItemsStatus = temp;
            }
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        protected String GetLongDescription(DataRowView row)
        {
            String retVal = String.Empty;

            try
            {
                retVal = row["ID"].ToString();
            }
            catch
            {
                // Non è necessarion fare nulla
            }

            return retVal;
        }

        private void caricaGridViewAndControls()
        {
            //Documenti
            if (this.rbl_XlsOrPdf.SelectedValue.Equals(PDF) && this.hd_export.Value.Equals(DOC))
            {
                this.caricaListaCampiDocumento(false, false);
                this.disabilitaCheckBoxGridView();
                this.cb_selezionaTutti.Visible = false;
            }

            if ((this.rbl_XlsOrPdf.SelectedValue.Equals(XLS) || this.rbl_XlsOrPdf.SelectedValue.Equals(ODS)) && this.hd_export.Value.Equals(DOC))
            {
                this.caricaListaCampiDocumento(true, true);
                this.cb_selezionaTutti.Visible = true;
            }

            if (this.rbl_XlsOrPdf.SelectedValue == "Model" && this.hd_export.Value.Equals(DOC))
            {
                this.caricaListaCampiDocumento(true, true);
                this.cb_selezionaTutti.Visible = true;
            }


            //Doc in Cestino
            if (this.rbl_XlsOrPdf.SelectedValue.Equals(PDF) && this.hd_export.Value.Equals(EXP_DOC_IN_CEST))
            {
                this.rbl_XlsOrPdf.Items[2].Enabled = false;
                this.caricaListaCampiDocInCestino(false);
                this.disabilitaCheckBoxGridView();
                this.cb_selezionaTutti.Visible = false;
            }

            if (this.rbl_XlsOrPdf.SelectedValue.Equals(XLS) && this.hd_export.Value.Equals(EXP_DOC_IN_CEST))
            {
                this.rbl_XlsOrPdf.Items[2].Enabled = false;
                this.caricaListaCampiDocInCestino(false);
                this.cb_selezionaTutti.Visible = true;
            }

            //Doc in Fasc
            if (this.rbl_XlsOrPdf.SelectedValue.Equals(PDF) && this.hd_export.Value.Equals(EXP_DOC_IN_FASC))
            {
                this.caricaListaCampiDocumento(false, false);
                this.disabilitaCheckBoxGridView();
                this.cb_selezionaTutti.Visible = false;
            }

            if ((this.rbl_XlsOrPdf.SelectedValue.Equals(XLS) || this.rbl_XlsOrPdf.SelectedValue.Equals(ODS)) && this.hd_export.Value.Equals(EXP_DOC_IN_FASC))
            {
                this.docInFasc = true;
                this.caricaListaCampiDocumento(true, true);
                this.cb_selezionaTutti.Visible = true;
            }

            //Fascicoli
            if (this.rbl_XlsOrPdf.SelectedValue.Equals(PDF) && this.hd_export.Value.Equals(FASC))
            {
                this.caricaListaCampiFascicolo(false, false);
                this.disabilitaCheckBoxGridView();
                this.cb_selezionaTutti.Visible = false;
            }

            if ((this.rbl_XlsOrPdf.SelectedValue.Equals(XLS) || this.rbl_XlsOrPdf.SelectedValue.Equals(ODS)) && this.hd_export.Value.Equals(FASC))
            {
                this.caricaListaCampiFascicolo(true, true);
                this.cb_selezionaTutti.Visible = true;
            }

            //Trasmissioni
            if (this.rbl_XlsOrPdf.SelectedValue.Equals(PDF) && this.hd_export.Value.Equals(EXP_TO_DO_LIST))
            {
                this.rbl_XlsOrPdf.Items[2].Enabled = false;
                this.caricaListaCampiTrasmissioniDoc();
                this.disabilitaCheckBoxGridView();
                this.cb_selezionaTutti.Visible = false;
            }

            if (this.rbl_XlsOrPdf.SelectedValue.Equals(XLS) &&this.hd_export.Value.Equals(EXP_TO_DO_LIST))
            {
                this.rbl_XlsOrPdf.Items[2].Enabled = false;
                this.caricaListaCampiTrasmissioniDoc();
                this.cb_selezionaTutti.Visible = true;
            }

            if (this.hd_export.Value.Equals(EXP_NOTIFY))
            {
                //this.rbl_XlsOrPdf.Items[2].Enabled = false;
                this.LoadListNotificationFields();
                if (this.rbl_XlsOrPdf.SelectedValue.Equals(PDF))
                {
                    this.disabilitaCheckBoxGridView();
                    this.cb_selezionaTutti.Visible = false;
                }
                else
                {
                    this.cb_selezionaTutti.Visible = true;
                }
            }

            if(this.hd_export.Value.Equals(EXP_VISIBILITA_PROCESSI))
            {
                this.CaricaListaCampiVisibilitaProcessi();
                if (this.rbl_XlsOrPdf.SelectedValue.Equals(PDF))
                {
                    this.disabilitaCheckBoxGridView();
                    this.cb_selezionaTutti.Visible = false;
                }
                else
                {
                    this.cb_selezionaTutti.Visible = true;
                }
            }

            if (this.hd_export.Value.Equals(EXP_TRASM))
            {
                FiltroRicerca[] filterTrasm;

                filterTrasm = DocumentManager.getFiltroRicTrasm();
                if (filterTrasm == null)
                    filterTrasm = this.SearchFilters[0];

                if (filterTrasm != null && filterTrasm.Count() > 0)
                {
                    string tipoOgetto = (from i in filterTrasm where i.argomento.Equals(DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString()) select i.valore).FirstOrDefault();
                    if(tipoOgetto == "D")
                        this.caricaListaCampiTrasmissioniDoc();
                    else
                        this.caricaListaCampiTrasmissioniFasc();
                }
                else
                {
                    this.caricaListaCampiTrasmissioniDoc();
                }
                if (this.rbl_XlsOrPdf.SelectedValue.Equals(PDF))
                {
                    this.disabilitaCheckBoxGridView();
                    this.cb_selezionaTutti.Visible = false;
                }
                else
                {
                    this.cb_selezionaTutti.Visible = true;
                }
            }
            //Abilitazione dell'esportazione personalizzata
            if (this.ExportEnable)
            {
                this.panel_listaCampi.Visible = true;
                this.lbl_selezionaCampo.Visible = true;
            }
            else
            {
                this.panel_listaCampi.Visible = false;
                this.cb_selezionaTutti.Visible = false;
                this.lbl_selezionaCampo.Visible = false;
            }

            //Documenti
            if (this.rbl_XlsOrPdf.SelectedValue.Equals(PDF) && this.hd_export.Value == "scarto")
            {
                this.rbl_XlsOrPdf.Items[2].Enabled = false;
                this.caricaListaCampiScarto();
                this.disabilitaCheckBoxGridView();
                this.cb_selezionaTutti.Visible = false;
            }

            if (this.rbl_XlsOrPdf.SelectedValue.Equals(XLS) && this.hd_export.Value == "scarto")
            {
                this.rbl_XlsOrPdf.Items[2].Enabled = false;
                this.caricaListaCampiScarto();
                this.cb_selezionaTutti.Visible = true;
            }

            // Export rubrica
            if (this.hd_export.Value == "rubrica" || this.hd_export.Value == "searchAddressBook")
            {
                this.panel_listaCampi.Visible = false;
                this.lbl_selezionaCampo.Visible = false;
                this.cb_selezionaTutti.Visible = false;
            }
        }

        /// <summary>
        /// Carica i campi da stampare
        /// </summary>
        /// <param name="profilazione"></param>
        /// <param name="esportazioneExcel">True se è richiesta una esportazione excel</param>
        private void caricaListaCampiDocumento(bool profilazione, bool esportazioneExcel)
        {
            this.gv_listaCampi.Columns[0].Visible = true;
            this.gv_listaCampi.Columns[1].Visible = true;
            this.gv_listaCampi.Columns[4].Visible = true;
            DataTable dt = new DataTable();
            dt.Columns.Add("CAMPO_STANDARD");
            dt.Columns.Add("CAMPO_COMUNE");
            dt.Columns.Add("CAMPI");
            // Aggiunta colonna per il system id del campo
            dt.Columns.Add("ID");
            dt.Columns.Add("VISIBILE");


            // Se è una esportazione Excel, vengono caricati i campi della griglia
            if (esportazioneExcel && GridManager.IsRoleEnabledToUseGrids())
                this.ShowGridFiels(dt);
            else
                this.CaricaListaCampiDocumentoPreesistente(dt, profilazione, esportazioneExcel);


            dt.AcceptChanges();
            this.gv_listaCampi.DataSource = dt;
            this.gv_listaCampi.DataBind();
            this.gv_listaCampi.Visible = true;
            this.gv_listaCampi.Columns[0].Visible = false;
            this.gv_listaCampi.Columns[1].Visible = false;

            //Elimino la selezione di default di tutti i campi che non sono quelli standar
            if (!esportazioneExcel && !GridManager.IsRoleEnabledToUseGrids())
                for (int i = 9; i < this.gv_listaCampi.Rows.Count; i++)
                    ((System.Web.UI.WebControls.CheckBox)this.gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = false;

            // Se è stata richiesta l'esportazione in formato excel e non sono attive le griglie...
            if (esportazioneExcel && !GridManager.IsRoleEnabledToUseGrids())
                for (int i = 0; i < this.gv_listaCampi.Rows.Count; i++)
                    ((System.Web.UI.WebControls.CheckBox)this.gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = true;

            //Griglie custom
            if (esportazioneExcel && GridManager.IsRoleEnabledToUseGrids())
            {
                for (int i = 0; i < this.gv_listaCampi.Rows.Count; i++)
                {
                    string visibile = this.gv_listaCampi.Rows[i].Cells[4].Text;
                    if (!string.IsNullOrEmpty(visibile) && visibile.ToUpper().Equals("TRUE"))
                    {
                        ((System.Web.UI.WebControls.CheckBox)this.gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = true;
                    }
                    else
                    {
                        ((System.Web.UI.WebControls.CheckBox)this.gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = false;
                    }

                }
            }

            this.gv_listaCampi.Columns[4].Visible = false;
        }

        /// <summary>
        /// Funzione utilizzata per mostrare i campi della griglia
        /// </summary>
        /// <param name="dataTable">Data table in cui inserire i nomi dei campi</param>
        private void ShowGridFiels(DataTable dataTable)
        {
            bool allChecked = true;
            // Caricamento della griglia
            if (GridManager.SelectedGrid == null)
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);

            foreach (Field field in GridManager.SelectedGrid.Fields.Where(e => !e.Locked).OrderBy(e => e.Position).ToList())
            {
                DataRow row = dataTable.NewRow();
                row["CAMPO_STANDARD"] = field.CustomObjectId == 0 ? "0" : field.CustomObjectId.ToString();
                row["CAMPO_COMUNE"] = "0";
                row["CAMPI"] = field.Label;
                row["ID"] = field.FieldId;
                row["VISIBILE"] = (field.Visible).ToString();
                dataTable.Rows.Add(row);
                if (!field.Visible)
                {
                    allChecked = false;
                }
            }

            if (allChecked)
            {
                this.cb_selezionaTutti.Checked = true;
            }
            else
            {
                this.cb_selezionaTutti.Checked = false;
            }
        }

        private void CaricaListaCampiDocumentoPreesistente(DataTable dt, bool profilazione, bool esportazioneExcel)
        {
            //Aggiungo i campi standar del documento
            DataRow rw1 = dt.NewRow();
            rw1[0] = "1";
            rw1[1] = "0";
            rw1[2] = "Registro";
            rw1[3] = "D2";
            dt.Rows.Add(rw1);
            DataRow rw2 = dt.NewRow();
            rw2[0] = "1";
            rw2[1] = "0";
            rw2[2] = "Prot. / Id. Doc.";
            rw2[3] = "D1";
            dt.Rows.Add(rw2);
            DataRow rw3 = dt.NewRow();
            rw3[0] = "1";
            rw3[1] = "0";
            rw3[2] = "Data";
            rw3[3] = "D9";
            dt.Rows.Add(rw3);
            DataRow rw4 = dt.NewRow();
            rw4[0] = "1";
            rw4[1] = "0";
            rw4[2] = "Oggetto";
            rw4[3] = "D4";
            dt.Rows.Add(rw4);
            DataRow rw5 = dt.NewRow();
            rw5[0] = "1";
            rw5[1] = "0";
            rw5[2] = "Tipo";
            rw5[3] = "D3";
            dt.Rows.Add(rw5);
            DataRow rw6 = dt.NewRow();
            rw6[0] = "1";
            rw6[1] = "0";
            rw6[2] = "Mitt. / Dest.";
            rw6[3] = "D5";
            dt.Rows.Add(rw6);
            DataRow rw7 = dt.NewRow();
            rw7[0] = "1";
            rw7[1] = "0";
            rw7[2] = "Cod. Fascicoli";
            rw7[3] = "D18";
            dt.Rows.Add(rw7);
            DataRow rw8 = dt.NewRow();
            rw8[0] = "1";
            rw8[1] = "0";
            rw8[2] = "Annullato";
            rw8[3] = "D11";
            dt.Rows.Add(rw8);
            DataRow rw9 = dt.NewRow();
            rw9[0] = "1";
            rw9[1] = "0";
            rw9[2] = "File";
            rw9[3] = "D23";
            dt.Rows.Add(rw9);

            // Se è richiesta una esportazione excel...
            if (esportazioneExcel)
            {
                DataRow rw10 = dt.NewRow();
                rw10[0] = "1";
                rw10[1] = "0";
                rw10[2] = "Note";
                rw10[3] = "D17";
                dt.Rows.Add(rw10);
            }

            //Verifico che si vogliono o meno visualizzare anche i campi del profilo di documento
            if (profilazione)
            {
                //Verifico che è stata selezionata una tipologia ed in caso affermativo
                //rendo possibile la selezione anche dei campi profilati dinamicamente
                DocsPaWR.FiltroRicerca[][] filtri = null;
                if (DocumentManager.getFiltroRicDoc() != null)
                    filtri = DocumentManager.getFiltroRicDoc();
                else
                    filtri = DocumentManager.getMemoriaFiltriRicDoc();

                if (filtri == null)
                    filtri = (DocsPaWR.FiltroRicerca[][])HttpContext.Current.Session["ricFasc.listaFiltri"];

                if (this.docInFasc)
                {
                    if (Session["SearchFilters"] != null)
                    {
                        filtri = Session["SearchFilters"] as FiltroRicerca[][];
                    }
                }

                if (filtri != null)
                {
                    foreach (DocsPaWR.FiltroRicerca[] filtroUno in filtri)
                    {
                        foreach (DocsPaWR.FiltroRicerca filtroDue in filtroUno)
                        {
                            switch (filtroDue.argomento)
                            {
                                case "TIPO_ATTO":
                                    DocsPaWR.Templates template = ProfilazioneDocManager.getTemplateById(filtroDue.valore, this);
                                    foreach (DocsPaWR.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                                    {
                                        DataRow rw = dt.NewRow();
                                        rw[0] = oggettoCustom.SYSTEM_ID;
                                        rw[1] = oggettoCustom.CAMPO_COMUNE;
                                        rw[2] = oggettoCustom.DESCRIZIONE;
                                        rw[3] = "A" + oggettoCustom.SYSTEM_ID;
                                        dt.Rows.Add(rw);
                                    }
                                    break;

                                case "CONTATORE_GRIGLIE_NO_CUSTOM":
                                    DataRow rf = dt.NewRow();
                                    rf[0] = "1";
                                    rf[1] = "0";
                                    rf[2] = "Contatore";
                                    rf[3] = "CONTATORE";
                                    dt.Rows.Add(rf);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void disabilitaCheckBoxGridView()
        {
            for (int i = 0; i < this.gv_listaCampi.Rows.Count; i++)
            {
                ((System.Web.UI.WebControls.CheckBox)gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = true;
                ((System.Web.UI.WebControls.CheckBox)gv_listaCampi.Rows[i].Cells[3].Controls[1]).Enabled = false;
            }
        }

        private void caricaListaCampiDocInCestino(bool profilazione)
        {
            this.gv_listaCampi.Columns[0].Visible = true;
            this.gv_listaCampi.Columns[1].Visible = true;
            DataTable dt = new DataTable();
            dt.Columns.Add("CAMPO_STANDARD");
            dt.Columns.Add("CAMPO_COMUNE");
            dt.Columns.Add("CAMPI");
            // Aggiunta colonna per il system id del campo
            dt.Columns.Add("ID");

            //Aggiungo i campi per i documenti in cestino
            DataRow rw1 = dt.NewRow();
            rw1[0] = "1";
            rw1[1] = "0";
            rw1[2] = "Registro";
            dt.Rows.Add(rw1);
            DataRow rw2 = dt.NewRow();
            rw2[0] = "1";
            rw2[1] = "0";
            rw2[2] = "Prot. / Id. Doc.";
            dt.Rows.Add(rw2);
            DataRow rw3 = dt.NewRow();
            rw3[0] = "1";
            rw3[1] = "0";
            rw3[2] = "Data";
            dt.Rows.Add(rw3);
            DataRow rw4 = dt.NewRow();
            rw4[0] = "1";
            rw4[1] = "0";
            rw4[2] = "Oggetto";
            dt.Rows.Add(rw4);
            DataRow rw5 = dt.NewRow();
            rw5[0] = "1";
            rw5[1] = "0";
            rw5[2] = "Tipo";
            dt.Rows.Add(rw5);
            DataRow rw6 = dt.NewRow();
            rw6[0] = "1";
            rw6[1] = "0";
            rw6[2] = "Mitt. / Dest.";
            dt.Rows.Add(rw6);
            DataRow rw7 = dt.NewRow();
            rw7[0] = "1";
            rw7[1] = "0";
            rw7[2] = "Motivo Rimozione";
            dt.Rows.Add(rw7);

            dt.AcceptChanges();
            this.gv_listaCampi.DataSource = dt;
            this.gv_listaCampi.DataBind();
            this.gv_listaCampi.Visible = true;
            this.gv_listaCampi.Columns[0].Visible = false;
            this.gv_listaCampi.Columns[1].Visible = false;
        }

        private void caricaListaCampiFascicolo(bool profilazione, bool esportazioneExcel)
        {
            this.gv_listaCampi.Columns[0].Visible = true;
            this.gv_listaCampi.Columns[1].Visible = true;
            this.gv_listaCampi.Columns[4].Visible = true;
            DataTable dt = new DataTable();
            dt.Columns.Add("CAMPO_STANDARD");
            dt.Columns.Add("CAMPO_COMUNE");
            dt.Columns.Add("CAMPI");
            // Aggiunta colonna per il system id del campo
            dt.Columns.Add("ID");
            dt.Columns.Add("VISIBILE");

            // Se è una esportazione Excel, vengono visualizzati i campi della griglia
            if (esportazioneExcel && GridManager.IsRoleEnabledToUseGrids())
                this.ShowGridFiels(dt);
            else
                this.caricaListaCampiFascicoloPreesistente(dt, profilazione, esportazioneExcel);

            dt.AcceptChanges();
            this.gv_listaCampi.DataSource = dt;
            this.gv_listaCampi.DataBind();
            this.gv_listaCampi.Visible = true;
            this.gv_listaCampi.Columns[0].Visible = false;
            this.gv_listaCampi.Columns[1].Visible = false;

            //Elimino la selezione di default di tutti i campi che non sono quelli standar
            if (!esportazioneExcel && !GridManager.IsRoleEnabledToUseGrids())
                for (int i = 8; i < this.gv_listaCampi.Rows.Count; i++)
                    ((System.Web.UI.WebControls.CheckBox)this.gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = false;

            // Se è stata richiesta l'esportazione in formato excel e non sono attive le griglie...
            if (esportazioneExcel && !GridManager.IsRoleEnabledToUseGrids())
                for (int i = 0; i < this.gv_listaCampi.Rows.Count; i++)
                    ((System.Web.UI.WebControls.CheckBox)this.gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = true;

            //Griglie custom
            if (esportazioneExcel && GridManager.IsRoleEnabledToUseGrids())
            {
                for (int i = 0; i < this.gv_listaCampi.Rows.Count; i++)
                {
                    string visibile = this.gv_listaCampi.Rows[i].Cells[4].Text;
                    if (!string.IsNullOrEmpty(visibile) && visibile.ToUpper().Equals("TRUE"))
                    {
                        ((System.Web.UI.WebControls.CheckBox)this.gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = true;
                    }
                    else
                    {
                        ((System.Web.UI.WebControls.CheckBox)this.gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = false;
                    }

                }
            }

            this.gv_listaCampi.Columns[4].Visible = false;
        }

        private void caricaListaCampiFascicoloPreesistente(DataTable dt, bool profilazione, bool esportazioneExcel)
        {
            //Aggiungo i campi standar del fascicolo
            DataRow rw1 = dt.NewRow();
            rw1[0] = "1";
            rw1[1] = "0";
            rw1[2] = "Registro";
            rw1[3] = "P7";
            dt.Rows.Add(rw1);

            DataRow rw2 = dt.NewRow();
            rw2[0] = "1";
            rw2[1] = "0";
            rw2[2] = "Tipo";
            rw2[3] = "P1";
            dt.Rows.Add(rw2);

            DataRow rw3 = dt.NewRow();
            rw3[0] = "1";
            rw3[1] = "0";
            rw3[2] = "Codice";
            rw3[3] = "P3";
            dt.Rows.Add(rw3);

            DataRow rw4 = dt.NewRow();
            rw4[0] = "1";
            rw4[1] = "0";
            rw4[2] = "Descrizione";
            rw4[3] = "P4";
            dt.Rows.Add(rw4);

            DataRow rw5 = dt.NewRow();
            rw5[0] = "1";
            rw5[1] = "0";
            rw5[2] = "Data Apertura";
            rw5[3] = "P5";
            dt.Rows.Add(rw5);

            DataRow rw6 = dt.NewRow();
            rw6[0] = "1";
            rw6[1] = "0";
            rw6[2] = "Data Chiusura";
            rw6[3] = "P6";
            dt.Rows.Add(rw6);

            DataRow rw7 = dt.NewRow();
            rw7[0] = "1";
            rw7[1] = "0";
            rw7[2] = "Collocazione Fisica";
            rw7[3] = "P22";
            dt.Rows.Add(rw7);

            DataRow rw8 = dt.NewRow();
            rw8[0] = "1";
            rw8[1] = "0";
            rw8[2] = "Tipologia";
            rw8[3] = "U1";
            dt.Rows.Add(rw8);

            // Se è richiesta l'esportazione in excel...
            if (esportazioneExcel)
            {
                // ...aggiungo il campo note
                DataRow rw9 = dt.NewRow();
                rw9[0] = "1";
                rw9[1] = "0";
                rw9[2] = "Note";
                rw9[3] = "P8";
                dt.Rows.Add(rw9);
            }

            //Verifico che si vogliono o meno visualizzare anche i campi del profilo di un fascicolo
            if (profilazione)
            {
                //Verifico che è stata selezionata una tipologia ed in caso affermativo
                //rendo possibile la selezione anche dei campi profilati dinamicamente
                DocsPaWR.FiltroRicerca[][] filtri = null;
                if (ProjectManager.getFiltroRicFasc() != null)
                    filtri = ProjectManager.getFiltroRicFasc();
                else
                    filtri = ProjectManager.getMemoriaFiltriRicFasc();

                if (filtri != null)
                {
                    foreach (DocsPaWR.FiltroRicerca[] filtroUno in filtri)
                    {
                        foreach (DocsPaWR.FiltroRicerca filtroDue in filtroUno)
                        {
                            switch (filtroDue.argomento)
                            {
                                case "TIPOLOGIA_FASCICOLO":
                                    DocsPaWR.Templates template = ProfilazioneDocManager.getTemplateById(filtroDue.valore, this);
                                    foreach (DocsPaWR.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                                    {
                                        DataRow rw = dt.NewRow();
                                        rw[0] = oggettoCustom.SYSTEM_ID;
                                        rw[1] = oggettoCustom.CAMPO_COMUNE;
                                        rw[2] = oggettoCustom.DESCRIZIONE;
                                        rw[3] = "A" + oggettoCustom.SYSTEM_ID;
                                        dt.Rows.Add(rw);
                                    }
                                    break;

                                case "CONTATORE_GRIGLIE_NO_CUSTOM":
                                    DataRow rf = dt.NewRow();
                                    rf[0] = "1";
                                    rf[1] = "0";
                                    rf[2] = "Contatore";
                                    rf[3] = "CONTATORE";
                                    dt.Rows.Add(rf);
                                    break;
                            }
                        }
                    }
                }
            }

        }

        private void caricaListaCampiTrasmissioniDoc()
        {
            this.gv_listaCampi.Columns[0].Visible = true;
            this.gv_listaCampi.Columns[1].Visible = true;
            DataTable dt = new DataTable();
            dt.Columns.Add("CAMPO_STANDARD");
            dt.Columns.Add("CAMPO_COMUNE");
            dt.Columns.Add("CAMPI");
            // Aggiunta colonna per il system id del campo
            dt.Columns.Add("ID");

            //Aggiungo i campi standar del fascicolo
            DataRow rw1 = dt.NewRow();
            rw1[0] = "1";
            rw1[1] = "0";
            rw1[2] = "Data Trasm.";
            dt.Rows.Add(rw1);

            DataRow rw2 = dt.NewRow();
            rw2[0] = "1";
            rw2[1] = "0";
            rw2[2] = "Documento Trasmesso";
            dt.Rows.Add(rw2);

            DataRow rw3 = dt.NewRow();
            rw3[0] = "1";
            rw3[1] = "0";
            rw3[2] = "Mittenti";
            dt.Rows.Add(rw3);

            DataRow rw4 = dt.NewRow();
            rw4[0] = "1";
            rw4[1] = "0";
            rw4[2] = "Destinatari";
            dt.Rows.Add(rw4);

            dt.AcceptChanges();
            this.gv_listaCampi.DataSource = dt;
            this.gv_listaCampi.DataBind();
            this.gv_listaCampi.Visible = true;
            this.gv_listaCampi.Columns[0].Visible = false;
            this.gv_listaCampi.Columns[1].Visible = false;
        }

        private void caricaListaCampiTrasmissioniFasc()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.gv_listaCampi.Columns[0].Visible = true;
            this.gv_listaCampi.Columns[1].Visible = true;
            DataTable dt = new DataTable();
            dt.Columns.Add("CAMPO_STANDARD");
            dt.Columns.Add("CAMPO_COMUNE");
            dt.Columns.Add("CAMPI");
            // Aggiunta colonna per il system id del campo
            dt.Columns.Add("ID");

            //Aggiungo i campi standar del fascicolo
            DataRow rw1 = dt.NewRow();
            rw1[0] = "1";
            rw1[1] = "0";
            rw1[2] = Utils.Languages.GetLabelFromCode("ExportDatiSelectionDataTrasm", language);
            rw1[3] = "DATA_INVIO";
            dt.Rows.Add(rw1);

            DataRow rw2 = dt.NewRow();
            rw2[0] = "1";
            rw2[1] = "0";
            rw2[2] = Utils.Languages.GetLabelFromCode("ExportDatiSelectionCodFasc", language);
            rw2[3] = "COD_FASCICOLO";
            dt.Rows.Add(rw2);

            DataRow rw3 = dt.NewRow();
            rw3[0] = "1";
            rw3[1] = "0";
            rw3[2] = Utils.Languages.GetLabelFromCode("ExportDatiSelectionDescFasc", language);
            rw3[3] = "DESC_FASCICOLO";
            dt.Rows.Add(rw3);

            DataRow rw4 = dt.NewRow();
            rw4[0] = "1";
            rw4[1] = "0";
            rw4[2] = Utils.Languages.GetLabelFromCode("ExportDatiSelectionDataApertura", language);
            rw4[3] = "DATA_APERTURA";
            dt.Rows.Add(rw4);

            dt.AcceptChanges();
            this.gv_listaCampi.DataSource = dt;
            this.gv_listaCampi.DataBind();
            this.gv_listaCampi.Visible = true;
            this.gv_listaCampi.Columns[0].Visible = false;
            this.gv_listaCampi.Columns[1].Visible = false;
        }

        /// <summary>
        /// Colonne per esporta da centro notifiche
        /// </summary>
        private void LoadListNotificationFields()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CAMPO_STANDARD");
            dt.Columns.Add("CAMPO_COMUNE");
            dt.Columns.Add("CAMPI");

            //Search label field in language file
            string language = UIManager.UserManager.GetUserLanguage();
            string labelA = Utils.Languages.GetLabelFromCode("EventsLblAction", language);
            string labelB = Utils.Languages.GetLabelFromCode("EventsLblUser", language);
            string labelC = Utils.Languages.GetLabelFromCode("EventsLblDtaEvent", language);
            string labelD = Utils.Languages.GetLabelFromCode("ViewDetailNotifyDocFasc", language);
            string labelE = Utils.Languages.GetLabelFromCode("LblObjectDescriptionExport", language);
            string labelF = Utils.Languages.GetLabelFromCode("DistributionListDetail", language);
            //Add fields
            DataRow rwA = dt.NewRow();
            rwA[0] = "1";
            rwA[1] = "0";
            rwA[2] = labelA;

            DataRow rwB = dt.NewRow();
            rwB[0] = "1";
            rwB[1] = "0";
            rwB[2] = labelB;

            DataRow rwC = dt.NewRow();
            rwC[0] = "1";
            rwC[1] = "0";
            rwC[2] = labelC;

            DataRow rwD = dt.NewRow();
            rwD[0] = "1";
            rwD[1] = "0";
            rwD[2] = labelD;

            DataRow rwE = dt.NewRow();
            rwE[0] = "1";
            rwE[1] = "0";
            rwE[2] = labelE;

            DataRow rwF = dt.NewRow();
            rwF[0] = "1";
            rwF[1] = "0";
            rwF[2] = labelF;
            
            dt.Rows.Add(rwA);
            dt.Rows.Add(rwB);
            dt.Rows.Add(rwC);
            dt.Rows.Add(rwD);
            dt.Rows.Add(rwE);
            dt.Rows.Add(rwF);

            dt.AcceptChanges();
            this.gv_listaCampi.DataSource = dt;
            this.gv_listaCampi.DataBind();
            this.gv_listaCampi.Visible = true;
            this.gv_listaCampi.Columns[0].Visible = false;
            this.gv_listaCampi.Columns[1].Visible = false;
        }

        private void caricaListaCampiScarto()
        {
            this.gv_listaCampi.Columns[0].Visible = true;
            this.gv_listaCampi.Columns[1].Visible = true;
            DataTable dt = new DataTable();
            dt.Columns.Add("CAMPO_STANDARD");
            dt.Columns.Add("CAMPO_COMUNE");
            dt.Columns.Add("CAMPI");
            // Aggiunta colonna per il system id del campo
            dt.Columns.Add("ID");

            //Aggiungo i campi standar del documento
            DataRow rw1 = dt.NewRow();
            rw1[0] = "1";
            rw1[1] = "0";
            rw1[2] = "Tipo";
            dt.Rows.Add(rw1);
            DataRow rw2 = dt.NewRow();
            rw2[0] = "1";
            rw2[1] = "0";
            rw2[2] = "Codice Classificazione";
            dt.Rows.Add(rw2);
            DataRow rw3 = dt.NewRow();
            rw3[0] = "1";
            rw3[1] = "0";
            rw3[2] = "Codice";
            dt.Rows.Add(rw3);
            DataRow rw4 = dt.NewRow();
            rw4[0] = "1";
            rw4[1] = "0";
            rw4[2] = "Descrizione";
            dt.Rows.Add(rw4);
            DataRow rw5 = dt.NewRow();
            rw5[0] = "1";
            rw5[1] = "0";
            rw5[2] = "Data Chiusura";
            dt.Rows.Add(rw5);
            DataRow rw6 = dt.NewRow();
            rw6[0] = "1";
            rw6[1] = "0";
            rw6[2] = "Mesi conservazione";
            dt.Rows.Add(rw6);
            DataRow rw7 = dt.NewRow();
            rw7[0] = "1";
            rw7[1] = "0";
            rw7[2] = "Mesi di chiusura";
            dt.Rows.Add(rw7);
            dt.AcceptChanges();
            this.gv_listaCampi.DataSource = dt;
            this.gv_listaCampi.DataBind();
            this.gv_listaCampi.Visible = true;
            this.gv_listaCampi.Columns[0].Visible = false;
            this.gv_listaCampi.Columns[1].Visible = false;

            DocsPaWR.InfoScarto infoScarto = new DocsPaWR.InfoScarto();
            infoScarto = ProjectManager.getIstanzaScarto(this);
            if (infoScarto != null)
            {
                txt_titolo.Text = infoScarto.descrizione + "---" + infoScarto.note;
                txt_titolo.Enabled = false;
            }
        }

        private void CaricaListaCampiVisibilitaProcessi()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CAMPO_STANDARD");
            dt.Columns.Add("CAMPO_COMUNE");
            dt.Columns.Add("CAMPI");

            //Search label field in language file
            string language = UIManager.UserManager.GetUserLanguage();
            string labelA = Utils.Languages.GetLabelFromCode("VisibilitaProcessiNomeProcesso", language);
            string labelB = Utils.Languages.GetLabelFromCode("VisibilitaProcessiCodiceRuolo", language);
            string labelC = Utils.Languages.GetLabelFromCode("VisibilitaProcessiDescrizioneRuolo", language);
            string labelD = Utils.Languages.GetLabelFromCode("VisibilitaProcessiTipoVisibilita", language);
            string labelE = Utils.Languages.GetLabelFromCode("VisibilitySignatureNotificaConclusione", language);
            string labelF = Utils.Languages.GetLabelFromCode("VisibilitySignatureNotificaInterruzione", language);
            string labelG = Utils.Languages.GetLabelFromCode("VisibilitySignatureNotificaErrore", language);

            //Add fields
            DataRow rwA = dt.NewRow();
            rwA[0] = "1";
            rwA[1] = "0";
            rwA[2] = labelA;

            DataRow rwB = dt.NewRow();
            rwB[0] = "1";
            rwB[1] = "0";
            rwB[2] = labelB;

            DataRow rwC = dt.NewRow();
            rwC[0] = "1";
            rwC[1] = "0";
            rwC[2] = labelC;

            DataRow rwD = dt.NewRow();
            rwD[0] = "1";
            rwD[1] = "0";
            rwD[2] = labelD;

            DataRow rwE = dt.NewRow();
            rwE[0] = "1";
            rwE[1] = "0";
            rwE[2] = labelE;

            DataRow rwF = dt.NewRow();
            rwF[0] = "1";
            rwF[1] = "0";
            rwF[2] = labelF;

            DataRow rwG = dt.NewRow();
            rwG[0] = "1";
            rwG[1] = "0";
            rwG[2] = labelG;

            dt.Rows.Add(rwA);
            dt.Rows.Add(rwB);
            dt.Rows.Add(rwC);
            dt.Rows.Add(rwD);
            dt.Rows.Add(rwE);
            dt.Rows.Add(rwF);
            dt.Rows.Add(rwG);


            dt.AcceptChanges();
            this.gv_listaCampi.DataSource = dt;
            this.gv_listaCampi.DataBind();
            this.gv_listaCampi.Visible = true;
            this.gv_listaCampi.Columns[0].Visible = false;
            this.gv_listaCampi.Columns[1].Visible = false;
        }

        private void Export()
        {
            string oggetto = this.hd_export.Value;
            string tipologia = string.Empty;
            string titolo = this.txt_titolo.Text;
            DocsPaWR.InfoUtente userInfo = UserManager.GetInfoUser();
            DocsPaWR.Ruolo userRuolo = RoleManager.GetRoleInSession();
            DocsPaWR.FileDocumento file = new DocsPaWR.FileDocumento();
            string docOrFasc = Request.QueryString["docOrFasc"];
            string componentCall = string.Empty;

            // Lista dei system id degli elementi selezionati           
            String[] objSystemId = null;

            // Se nella request c'è il valore fromMassiveOperation...
            if (Request.QueryString["fromMassiveOperation"] != null)
            {
                List<MassiveOperationTarget> temp = MassiveOperationUtils.GetSelectedItems();
                objSystemId = new String[temp.Count];
                for (int i = 0; i < temp.Count; i++) objSystemId[i] = temp[i].Id;
            }

            // Reperimento dalla sessione del contesto di ricerca fulltext
            DocsPaWR.FullTextSearchContext context = Session["FULL_TEXT_CONTEXT"] as DocsPaWR.FullTextSearchContext;

            if (this.rbl_XlsOrPdf.SelectedValue.Equals(PDF))
                tipologia = PDF;
            else if (this.rbl_XlsOrPdf.SelectedValue.Equals(XLS))
                tipologia = XLS;
            else if (this.rbl_XlsOrPdf.SelectedValue == "Model")
                tipologia = "Model";
            else if (this.rbl_XlsOrPdf.SelectedValue.Equals(ODS))
                tipologia = ODS;
            try
            {

                //Se la tipologia scelta è "XLS" recupero i campi scelti dall'utente
                if (tipologia.Equals(XLS) || tipologia.Equals(ODS))
                {
                    ArrayList campiSelezionati = this.getCampiSelezionati();

                    if ((campiSelezionati.Count != 0) || (this.hd_export.Value.Equals("rubrica") || (this.hd_export.Value.Equals("searchAddressBook"))))
                    {
                        exportDatiManager manager = new exportDatiManager(oggetto, tipologia, titolo, userInfo, userRuolo, context, docOrFasc, campiSelezionati, objSystemId);
                        file = manager.Export();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningExportDatiNoneFieldSelected', 'warning', '');", true);
                    }
                }
                else
                {
                    if (tipologia == "Model")
                    {
                        ArrayList campiSelezionati = getCampiSelezionati();

                        if ((campiSelezionati.Count != 0))
                        {
                            exportDatiManager manager = new exportDatiManager(oggetto, tipologia, titolo, userInfo, userRuolo, context, docOrFasc, campiSelezionati, objSystemId);
                            file = manager.Export();
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningExportDatiNoneFieldSelected', 'warning', '');", true);
                        }
                    }
                    else
                    {
                        if (tipologia.Equals(PDF))
                        {
                            exportDatiManager manager = new exportDatiManager(oggetto, tipologia, titolo, userInfo, userRuolo, context, docOrFasc, objSystemId);
                            file = manager.Export();
                        }
                    }
                }


                if (file == null || file.content == null || file.content.Length == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('InfoExportDatiNoneFound', 'info', '');", true);
                }
                else
                {
                    switch (componentType)
                    {
                        case (Constans.TYPE_SOCKET):
                            componentCall = "OpenFileSocket('" + tipologia + "');";
                            break;
                        case (Constans.TYPE_APPLET):
                            componentCall = "OpenFileApplet('" + tipologia + "');";
                            break;
                        case (Constans.TYPE_SMARTCLIENT):
                        case (Constans.TYPE_ACTIVEX):
                            componentCall = "OpenFileActiveX('" + tipologia + "');";
                            break;
                        default:
                            componentCall = "OpenFileActiveX('" + tipologia + "');";
                            break;
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "openFile", componentCall, true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorCustom', 'error', '', '" + utils.FormatJs(ex.Message) + "');", true);
            }
        }

        private ArrayList getCampiSelezionati()
        {
            ArrayList campiSelezionati = new ArrayList();
            for (int i = 0; i < this.gv_listaCampi.Rows.Count; i++)
            {
                if (((System.Web.UI.WebControls.CheckBox)this.gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked)
                {
                    DocsPaWR.CampoSelezionato campoSelezionato = new DocsPaWR.CampoSelezionato();
                    campoSelezionato.campoStandard = this.gv_listaCampi.Rows[i].Cells[0].Text;
                    campoSelezionato.campoComune = this.gv_listaCampi.Rows[i].Cells[1].Text;
                    campoSelezionato.nomeCampo = HttpUtility.HtmlDecode(gv_listaCampi.Rows[i].Cells[2].Text);
                    campoSelezionato.fieldID = (((HiddenField)this.gv_listaCampi.Rows[i].Cells[3].FindControl("hfLongDescriuption")).Value).ToString();

                    int id = 0;
                    Int32.TryParse(((HiddenField)this.gv_listaCampi.Rows[i].Cells[3].FindControl("hfLongDescriuption")).Value, out id);
                    campoSelezionato.SystemId = id;

                    campiSelezionati.Add(campoSelezionato);
                }
            }
            return campiSelezionati;
        }

        #endregion

        public static string httpFullPath
        {
            get
            {
                return utils.getHttpFullPath();
            }
        }
    }
}