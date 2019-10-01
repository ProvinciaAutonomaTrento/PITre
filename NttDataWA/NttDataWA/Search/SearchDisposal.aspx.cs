using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;

namespace NttDataWA.Search
{
    public partial class SearchDisposal : System.Web.UI.Page
    {


        #region Properties
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
        public int PagesCount
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

        private string SelectedRow
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["selectedRow"] != null)
                {
                    result = HttpContext.Current.Session["selectedRow"] as String;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["selectedRow"] = value;
            }
        }

        #endregion

        #region events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    this.InitializePage();
                }
                this.RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        //**************************************************************************************************
        //  LIMITI STEFANO
        //  30/07/2013
        //  Eliminazione del valore della TxtTo al cambio dell'index delle dropdownlist,
        //  in particolare alla scelta dell'indice rappresentante "Valore singolo" e "Oggi".
        //  Modifica effettuata poichè sel la textbox non è vuota viene usato il valore a discapito
        //  dell'indice selezionato nella corrispondente dropdowlist
        //**************************************************************************************************

        protected void ddl_dtaCreazione_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dtaCreazione.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.dtaCreazione_TxtFrom.ReadOnly = false;
                        this.dtaCreazione_TxtTo.Visible = false;
                        this.dtaCreazione_TxtTo.Text = string.Empty;
                        this.lbl_dtaCreazioneTo.Visible = false;
                        this.lbl_dtaCreazioneFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.dtaCreazione_TxtFrom.ReadOnly = false;
                        this.dtaCreazione_TxtTo.ReadOnly = false;
                        this.lbl_dtaCreazioneTo.Visible = true;
                        this.lbl_dtaCreazioneFrom.Visible = true;
                        this.dtaCreazione_TxtTo.Visible = true;
                        this.lbl_dtaCreazioneFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaCreazioneTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 2: //Oggi
                        this.lbl_dtaCreazioneTo.Visible = false;
                        this.dtaCreazione_TxtTo.Visible = false;
                        this.dtaCreazione_TxtTo.Text = string.Empty;
                        this.dtaCreazione_TxtFrom.ReadOnly = true;
                        this.dtaCreazione_TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                        this.lbl_dtaCreazioneFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaCreazioneTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 3: //Settimana corrente
                        this.lbl_dtaCreazioneTo.Visible = true;
                        this.dtaCreazione_TxtTo.Visible = true;
                        this.dtaCreazione_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.dtaCreazione_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.dtaCreazione_TxtTo.ReadOnly = true;
                        this.dtaCreazione_TxtFrom.ReadOnly = true;
                        this.lbl_dtaCreazioneFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaCreazioneTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 4: //Mese corrente
                        this.lbl_dtaCreazioneTo.Visible = true;
                        this.dtaCreazione_TxtTo.Visible = true;
                        this.dtaCreazione_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.dtaCreazione_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.dtaCreazione_TxtTo.ReadOnly = true;
                        this.dtaCreazione_TxtFrom.ReadOnly = true;
                        this.lbl_dtaCreazioneFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaCreazioneTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                }

                this.upPnlIntervals.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dtaProposto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dtaProposto.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.dtaProposto_TxtFrom.ReadOnly = false;
                        this.dtaProposto_TxtTo.Visible = false;
                        this.dtaProposto_TxtTo.Text = string.Empty;
                        this.lbl_dtaPropostoTo.Visible = false;
                        this.lbl_dtaPropostoFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.dtaProposto_TxtFrom.ReadOnly = false;
                        this.dtaProposto_TxtTo.ReadOnly = false;
                        this.lbl_dtaPropostoTo.Visible = true;
                        this.lbl_dtaPropostoFrom.Visible = true;
                        this.dtaProposto_TxtTo.Visible = true;
                        this.lbl_dtaPropostoFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaPropostoTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 2: //Oggi
                        this.lbl_dtaPropostoTo.Visible = false;
                        this.dtaProposto_TxtTo.Visible = false;
                        this.dtaProposto_TxtTo.Text = string.Empty;
                        this.dtaProposto_TxtFrom.ReadOnly = true;
                        this.dtaProposto_TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                        break;
                    case 3: //Settimana corrente
                        this.lbl_dtaPropostoTo.Visible = true;
                        this.dtaProposto_TxtTo.Visible = true;
                        this.dtaProposto_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.dtaProposto_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.dtaProposto_TxtTo.ReadOnly = true;
                        this.dtaProposto_TxtFrom.ReadOnly = true;
                        this.lbl_dtaPropostoFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaPropostoTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 4: //Mese corrente
                        this.lbl_dtaPropostoTo.Visible = true;
                        this.dtaProposto_TxtTo.Visible = true;
                        this.dtaProposto_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.dtaProposto_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.dtaProposto_TxtTo.ReadOnly = true;
                        this.dtaProposto_TxtFrom.ReadOnly = true;
                        this.lbl_dtaPropostoFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaPropostoTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                }

                this.upPnlIntervals.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dtaApprovato_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dtaApprovato.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.dtaApprovato_TxtFrom.ReadOnly = false;
                        this.dtaApprovato_TxtTo.Visible = false;
                        this.dtaApprovato_TxtTo.Text = string.Empty;
                        this.lbl_dtaApprovatoTo.Visible = false;
                        this.lbl_dtaApprovatoFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.dtaApprovato_TxtFrom.ReadOnly = false;
                        this.dtaApprovato_TxtTo.ReadOnly = false;
                        this.lbl_dtaApprovatoTo.Visible = true;
                        this.lbl_dtaApprovatoFrom.Visible = true;
                        this.dtaApprovato_TxtTo.Visible = true;
                        this.lbl_dtaApprovatoFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaApprovatoTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 2: //Oggi
                        this.lbl_dtaApprovatoTo.Visible = false;
                        this.dtaApprovato_TxtTo.Visible = false;
                        this.dtaApprovato_TxtTo.Text = string.Empty;
                        this.dtaApprovato_TxtFrom.ReadOnly = true;
                        this.dtaApprovato_TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                        break;
                    case 3: //Settimana corrente
                        this.lbl_dtaApprovatoTo.Visible = true;
                        this.dtaApprovato_TxtTo.Visible = true;
                        this.dtaApprovato_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.dtaApprovato_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.dtaApprovato_TxtTo.ReadOnly = true;
                        this.dtaApprovato_TxtFrom.ReadOnly = true;
                        this.lbl_dtaApprovatoFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaApprovatoTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 4: //Mese corrente
                        this.lbl_dtaApprovatoTo.Visible = true;
                        this.dtaApprovato_TxtTo.Visible = true;
                        this.dtaApprovato_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.dtaApprovato_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.dtaApprovato_TxtTo.ReadOnly = true;
                        this.dtaApprovato_TxtFrom.ReadOnly = true;
                        this.lbl_dtaApprovatoFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaApprovatoTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                }

                this.upPnlIntervals.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dtaEseguito_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dtaEseguito.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.dtaEseguito_TxtFrom.ReadOnly = false;
                        this.dtaEseguito_TxtTo.Visible = false;
                        this.dtaEseguito_TxtTo.Text = string.Empty;
                        this.lbl_dtaEseguitoTo.Visible = false;
                        this.lbl_dtaEseguitoFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.dtaEseguito_TxtFrom.ReadOnly = false;
                        this.dtaEseguito_TxtTo.ReadOnly = false;
                        this.lbl_dtaEseguitoTo.Visible = true;
                        this.lbl_dtaEseguitoFrom.Visible = true;
                        this.dtaEseguito_TxtTo.Visible = true;
                        this.lbl_dtaEseguitoFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaEseguitoTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 2: //Oggi
                        this.lbl_dtaEseguitoTo.Visible = false;
                        this.dtaEseguito_TxtTo.Visible = false;
                        this.dtaEseguito_TxtTo.Text = string.Empty;
                        this.dtaEseguito_TxtFrom.ReadOnly = true;
                        this.dtaEseguito_TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                        break;
                    case 3: //Settimana corrente
                        this.lbl_dtaEseguitoTo.Visible = true;
                        this.dtaEseguito_TxtTo.Visible = true;
                        this.dtaEseguito_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.dtaEseguito_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.dtaEseguito_TxtTo.ReadOnly = true;
                        this.dtaEseguito_TxtFrom.ReadOnly = true;
                        this.lbl_dtaEseguitoFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaEseguitoTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 4: //Mese corrente
                        this.lbl_dtaEseguitoTo.Visible = true;
                        this.dtaEseguito_TxtTo.Visible = true;
                        this.dtaEseguito_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.dtaEseguito_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.dtaEseguito_TxtTo.ReadOnly = true;
                        this.dtaEseguito_TxtFrom.ReadOnly = true;
                        this.lbl_dtaEseguitoFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaEseguitoTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                }

                this.upPnlIntervals.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dtaAnalisiCompletata_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dtaAnalisiCompletata.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.dtaAnalisiCompletata_TxtFrom.ReadOnly = false;
                        this.dtaAnalisiCompletata_TxtTo.Visible = false;
                        this.dtaAnalisiCompletata_TxtTo.Text = string.Empty;
                        this.lbl_dtaAnalisiCompletataTo.Visible = false;
                        this.lbl_dtaAnalisiCompletataFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.dtaAnalisiCompletata_TxtFrom.ReadOnly = false;
                        this.dtaAnalisiCompletata_TxtTo.ReadOnly = false;
                        this.lbl_dtaAnalisiCompletataTo.Visible = true;
                        this.lbl_dtaAnalisiCompletataFrom.Visible = true;
                        this.dtaAnalisiCompletata_TxtTo.Visible = true;
                        this.lbl_dtaAnalisiCompletataFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaAnalisiCompletataTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 2: //Oggi
                        this.lbl_dtaAnalisiCompletataTo.Visible = false;
                        this.dtaAnalisiCompletata_TxtTo.Visible = false;
                        this.dtaAnalisiCompletata_TxtTo.Text = string.Empty;
                        this.dtaAnalisiCompletata_TxtFrom.ReadOnly = true;
                        this.dtaAnalisiCompletata_TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                        this.lbl_dtaAnalisiCompletataFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaAnalisiCompletataTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 3: //Settimana corrente
                        this.lbl_dtaAnalisiCompletataTo.Visible = true;
                        this.dtaAnalisiCompletata_TxtTo.Visible = true;
                        this.dtaAnalisiCompletata_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.dtaAnalisiCompletata_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.dtaAnalisiCompletata_TxtTo.ReadOnly = true;
                        this.dtaAnalisiCompletata_TxtFrom.ReadOnly = true;
                        this.lbl_dtaAnalisiCompletataFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaAnalisiCompletataTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                    case 4: //Mese corrente
                        this.lbl_dtaAnalisiCompletataTo.Visible = true;
                        this.dtaAnalisiCompletata_TxtTo.Visible = true;
                        this.dtaAnalisiCompletata_TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.dtaAnalisiCompletata_TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.dtaAnalisiCompletata_TxtTo.ReadOnly = true;
                        this.dtaAnalisiCompletata_TxtFrom.ReadOnly = true;
                        this.lbl_dtaAnalisiCompletataFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaAnalisiCompletataTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                }

                this.upPnlIntervals.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        //**************************************************************************************************
        //  FINE Modifiche
        //**************************************************************************************************

        #endregion

        #region methods

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        protected void BuildGridNavigator()
        {
            try
            {
                int countPage = this.PagesCount;

                int val = this.RecordCount % this.PageSize;
                if (val == 0)
                {
                    countPage = countPage - 1;
                }

                if (countPage > 1)
                {
                    Panel panel = new Panel();
                    panel.EnableViewState = true;
                    panel.CssClass = "recordNavigator";

                    int startFrom = 1;
                    if (this.SelectedPage > 6) startFrom = this.SelectedPage - 5;

                    int endTo = 10;
                    if (this.SelectedPage > 6) endTo = this.SelectedPage + 5;
                    if (endTo > countPage) endTo = countPage;

                    if (startFrom > 1)
                    {
                        LinkButton btn = new LinkButton();
                        btn.EnableViewState = true;
                        btn.Text = "...";
                        btn.Attributes["onclick"] = " $('#grid_pageindex').val(" + (startFrom - 1) + "); __doPostBack('upPnlGridIndexes', ''); return false;";
                        panel.Controls.Add(btn);
                    }

                    for (int i = startFrom; i <= endTo; i++)
                    {
                        if (i == this.SelectedPage)
                        {
                            Literal lit = new Literal();
                            lit.Text = "<span>" + i.ToString() + "</span>";
                            panel.Controls.Add(lit);
                        }
                        else
                        {
                            LinkButton btn = new LinkButton();
                            btn.EnableViewState = true;
                            btn.Text = i.ToString();
                            btn.Attributes["onclick"] = " $('#grid_pageindex').val($(this).text()); __doPostBack('upPnlGridIndexes', ''); return false;";
                            panel.Controls.Add(btn);
                        }
                    }

                    if (endTo < countPage)
                    {
                        LinkButton btn = new LinkButton();
                        btn.EnableViewState = true;
                        btn.Text = "...";
                        btn.Attributes["onclick"] = " $('#grid_pageindex').val(" + endTo + "); __doPostBack('upPnlGridIndexes', ''); return false;";
                        panel.Controls.Add(btn);
                    }
                }
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
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OnlyNumbers", "OnlyNumbers();", true);
        }

        private void InitializePage()
        {
            this.InitializeLanguage();
            this.LoadMassiveOperation();
            InitializeRblANDDropDownANDGrid();
        }

        private void InitializeRblANDDropDownANDGrid()
        {
            this.rblStateType.Items[0].Selected = true;
            LoadDataGridDummy();
            this.ddl_idDisposal_C.SelectedIndex = 0;
            this.ddl_idDisposal_C_SelectedIndexChanged(null, null);
            this.txt_initIdDisposal_C.Text = string.Empty;
            this.ddl_dtaCreazione.SelectedIndex = 0;
            this.ddl_dtaCreazione_SelectedIndexChanged(null, null);
            this.dtaCreazione_TxtFrom.Text = string.Empty;
            this.ddl_dtaProposto.SelectedIndex = 0;
            this.ddl_dtaProposto_SelectedIndexChanged(null, null);
            this.dtaProposto_TxtFrom.Text = string.Empty;
            this.ddl_dtaApprovato.SelectedIndex = 0;
            this.ddl_dtaApprovato_SelectedIndexChanged(null, null);
            this.dtaApprovato_TxtFrom.Text = string.Empty;
            this.ddl_dtaEseguito.SelectedIndex = 0;
            this.ddl_dtaEseguito_SelectedIndexChanged(null, null);
            this.dtaEseguito_TxtFrom.Text = string.Empty;
            this.ddl_dtaAnalisiCompletata.SelectedIndex = 0;
            this.ddl_dtaAnalisiCompletata_SelectedIndexChanged(null, null);
            this.dtaAnalisiCompletata_TxtFrom.Text = string.Empty;
        }

        public string SetNumDocDisposal(object eff)
        {
            return (eff != null ? eff.ToString() + "/" : "");// + (cop != null ? cop.ToString() : "");
        }

        public Int32 SetTotDocDisposal(object eff, object cop)
        {
            return (eff != null ? Convert.ToInt32(eff) : 0);// + (cop != null ? Convert.ToInt32(cop) : 0);
        }

        private void RemoveFilters()
        {
            this.TxtDescrizioneScarto.Text = string.Empty;
            InitializeRblANDDropDownANDGrid();
        }

        private void LoadDataGridDummy()
        {
            this.gvResultDisposal.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerResultSearchDisposal();
            this.gvResultDisposal.DataBind();
            this.gvResultDisposal.Rows[0].Visible = false;
            this.transferLblNumeroScarti.Text = this.GetLabel("transferLblNumeroScarti");
            this.transferLblNumeroScarti.Text = this.transferLblNumeroScarti.Text.Replace("{0}", "0");
        }

        private void LoadMassiveOperation()
        {
            string title = string.Empty;
        }

        protected void SearchDocumentDdlMassiveOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveExportData", "ajaxModalPopupMassiveExportData();", true);
        }

        protected void ddl_idDisposal_C_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (this.ddl_idDisposal_C.SelectedIndex)
                {
                    case 0: //Valore singolo

                        this.txt_initIdDisposal_C.ReadOnly = false;
                        this.txt_fineIdDisposal_C.Visible = false;
                        this.LtlIdDisposalA.Visible = false;
                        this.LtlIdDisposalDA.Visible = false;
                        this.txt_fineIdDisposal_C.Text = string.Empty;
                        break;
                    case 1: //Intervallo
                        this.txt_initIdDisposal_C.ReadOnly = false;
                        this.txt_fineIdDisposal_C.ReadOnly = false;
                        this.LtlIdDisposalA.Visible = true;
                        this.LtlIdDisposalDA.Visible = true;
                        this.txt_fineIdDisposal_C.Visible = true;
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            //Parte ID E DESCRIZIONE
            this.LtlIdScartoSearch.Text = Utils.Languages.GetLabelFromCode("LtlIdScartoSearch", language);
            this.ddl_idDisposal_C.Items[0].Text = Utils.Languages.GetLabelFromCode("ddl_idDocumento_C0", language);
            this.ddl_idDisposal_C.Items[1].Text = Utils.Languages.GetLabelFromCode("ddl_idDocumento_C1", language);
            this.LtlIdDisposalDA.Text = Utils.Languages.GetLabelFromCode("LtlIdDisposalDA", language);
            this.LtlIdDisposalA.Text = Utils.Languages.GetLabelFromCode("LtlIdDisposalA", language);
            //FINE
            this.LitSearchDisposal.Text = Utils.Languages.GetLabelFromCode("LitSearchDisposal", language);
            this.SearchDisposalSearch.Text = Utils.Languages.GetLabelFromCode("SearchLabelButton", language);
            this.SearchDisposalRemoveFilters.Text = Utils.Languages.GetLabelFromCode("SearchLabelRemoveFiltersButton", language);
            //Data Creazione
            this.lit_dtaCreazione.Text = Utils.Languages.GetLabelFromCode("lit_dtaCreazione", language);
            this.dtaCreazione_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaCreazione_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.dtaCreazione_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.dtaCreazione_opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.dtaCreazione_opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            //Data Analisi completata
            this.lit_dtaAnalisiCompletata.Text = Utils.Languages.GetLabelFromCode("lit_dtaAnalisiCompletata", language);
            this.dtaAnalisiCompletata_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaAnalisiCompletata_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.dtaAnalisiCompletata_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.dtaAnalisiCompletata_opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.dtaAnalisiCompletata_opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            //fine data crezione.
            //Data Proposto
            this.lit_dtaProposto.Text = Utils.Languages.GetLabelFromCode("lit_dtaProposto", language);
            this.dtaProposto_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaProposto_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.dtaProposto_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.dtaProposto_opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.dtaProposto_opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            //FIne data proposto
            //Data Approvazione
            this.lit_dtaApprovato.Text = Utils.Languages.GetLabelFromCode("lit_dtaApprovato", language);
            this.dtaApprovato_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaApprovato_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.dtaApprovato_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.dtaApprovato_opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.dtaApprovato_opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            //Data Esecuzione
            this.lit_dtaEseguito.Text = Utils.Languages.GetLabelFromCode("lit_dtaEseguito", language);
            this.dtaEseguito_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaEseguito_opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.dtaEseguito_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.dtaEseguito_opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.dtaEseguito_opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);

            //Massive
            this.LtlIdScartoSearchDescrizione.Text = Utils.Languages.GetLabelFromCode("LtlIdScartoSearchDescrizione", language);
            //OptionButton stati ricerca
            this.LitStateType.Text = Utils.Languages.GetLabelFromCode("LitStateTypeScarto", language);
            this.optCompletati.Text = Utils.Languages.GetLabelFromCode("optCompletati", language);
            this.optErrori.Text = Utils.Languages.GetLabelFromCode("optErrori", language);
            this.optTutti.Text = Utils.Languages.GetLabelFromCode("optTutti", language);

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

        protected void gvResultDisposal_PreRender(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < gvResultDisposal.Rows.Count; i++)
                {
                    if (gvResultDisposal.Rows[i].DataItemIndex >= 0)
                    {
                        int System_id = Convert.ToInt32(((System.Web.UI.WebControls.GridView)(sender)).DataKeys[i].Value);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void gvResultDisposal_SelectedIndexChanging(object sender, EventArgs e)
        {
            try
            {

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GetAllFilterForGetDisposal()
        {
            try
            {
                gvResultDisposal.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerResultSearchDisposal();
                gvResultDisposal.DataBind();
                gvResultDisposal.Rows[0].Visible = false;
                //Prendo tutti i transfer.
                String st_indefinizione = string.Empty;
                String st_analisicompletata = string.Empty;
                String st_proposto = string.Empty;
                String st_approvato = string.Empty;
                String st_inesecuzione = string.Empty;
                String st_effettuato = string.Empty;
                String st_inerrore = string.Empty;

                //**************************************************************************************************
                //  LIMITI STEFANO
                //  30/07/2013
                //  Modifiche varie su tutti i filtri ricerca:
                //  - correzioni errori sul codice
                //  - aggiunta dell'ora 23:59:59 per tutti i campi TxtTo equivalente alla seconda data di ricerca
                //**************************************************************************************************

                #region   data creazione
                //************************************************
                //Calendar data creazione
                //************************************************
                if (!string.IsNullOrEmpty(dtaCreazione_TxtFrom.Text) && (!string.IsNullOrEmpty(dtaCreazione_TxtTo.Text)))
                {
                    st_indefinizione += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                       (k => k.Value == "IN DEFINIZIONE").Select(k => k.Key).FirstOrDefault() +
                                        " AND DateTime >= Convert(DATETIME,'" + dtaCreazione_TxtFrom.Text.Trim() + "',103)" +
                                        " AND DateTime <= Convert(DATETIME,'" + dtaCreazione_TxtTo.Text.Trim() + " 23:59:59',103)";
                }
                else if (string.IsNullOrEmpty(dtaCreazione_TxtFrom.Text) && (!string.IsNullOrEmpty(dtaCreazione_TxtTo.Text)))
                {
                    st_indefinizione += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                       (k => k.Value == "IN DEFINIZIONE").Select(k => k.Key).FirstOrDefault() +
                                        " AND DateTime <= Convert(DATETIME,'" + dtaCreazione_TxtTo.Text.Trim() + " 23:59:59',103)";
                }
                else if (!string.IsNullOrEmpty(dtaCreazione_TxtFrom.Text) && (string.IsNullOrEmpty(dtaCreazione_TxtTo.Text)))
                {
                    //Verifico se è selezionata la Single Text
                    if (ddl_dtaCreazione.SelectedIndex == 0 || ddl_dtaCreazione.SelectedIndex == 2)
                    {
                        st_indefinizione += "DisposalStateType_ID= " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                                (k => k.Value == "IN DEFINIZIONE").Select(k => k.Key).FirstOrDefault() +
                                           " AND DateTime between Convert(DATETIME,'" + dtaCreazione_TxtFrom.Text.Trim() + " 00:00:00',103)" +
                                           " AND Convert(DATETIME,'" + dtaCreazione_TxtFrom.Text.Trim() + " 23:59:59',103)";

                    }
                    else
                        st_indefinizione += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                           (k => k.Value == "IN DEFINIZIONE").Select(k => k.Key).FirstOrDefault() +
                                            " AND DateTime >= Convert(DATETIME,'" + dtaCreazione_TxtFrom.Text.Trim() + "',103)";
                }
                //************************************************
                // FINE
                //************************************************
                #endregion

                #region data RICERCA completata
                //************************************************
                //Calendar data RICERCA completata
                //************************************************
                if (!string.IsNullOrEmpty(dtaAnalisiCompletata_TxtFrom.Text) && (!string.IsNullOrEmpty(dtaAnalisiCompletata_TxtTo.Text)))
                {
                    st_analisicompletata += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                       (k => k.Value == "RICERCA COMPLETATA").Select(k => k.Key).FirstOrDefault() +
                                        " AND DateTime >= Convert(DATETIME,'" + dtaAnalisiCompletata_TxtFrom.Text.Trim() + "',103)" +
                                        " AND DateTime <= Convert(DATETIME,'" + dtaAnalisiCompletata_TxtTo.Text.Trim() + " 23:59:59',103)";
                }
                else if (string.IsNullOrEmpty(dtaAnalisiCompletata_TxtFrom.Text) && (!string.IsNullOrEmpty(dtaAnalisiCompletata_TxtTo.Text)))
                {
                    //Verifico se è selezionata la Single Text
                    st_analisicompletata += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                       (k => k.Value == "RICERCA COMPLETATA").Select(k => k.Key).FirstOrDefault() +
                                        " AND DateTime <= Convert(DATETIME,'" + dtaAnalisiCompletata_TxtTo.Text.Trim() + " 23:59:59',103)";
                }
                else if (!string.IsNullOrEmpty(dtaAnalisiCompletata_TxtFrom.Text) && (string.IsNullOrEmpty(dtaAnalisiCompletata_TxtTo.Text)))
                {
                    //Verifico se è selezionata la Single Text
                    if (ddl_dtaAnalisiCompletata.SelectedIndex == 0 || ddl_dtaAnalisiCompletata.SelectedIndex == 2)
                    {
                        st_analisicompletata += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                          (k => k.Value == "RICERCA COMPLETATA").Select(k => k.Key).FirstOrDefault() +
                                           " AND DateTime between Convert(DATETIME,'" + dtaAnalisiCompletata_TxtFrom.Text.Trim() + " 00:00:00',103)" +
                                           " AND Convert(DATETIME,'" + dtaAnalisiCompletata_TxtFrom.Text.Trim() + " 23:59:59',103)";
                    }
                    else
                        st_analisicompletata += "DisposalStateType_ID == " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                           (k => k.Value == "RICERCA COMPLETATA").Select(k => k.Key).FirstOrDefault() +
                                            " AND DateTime >= Convert(DATETIME,'" + dtaAnalisiCompletata_TxtFrom.Text.Trim() + "',103)";
                }

                //************************************************
                // FINE
                //************************************************
                #endregion

                #region  analisi proposta
                //************************************************
                //Calendar data analisi proposta
                //************************************************
                if (!string.IsNullOrEmpty(dtaProposto_TxtFrom.Text) && (!string.IsNullOrEmpty(dtaProposto_TxtTo.Text)))
                {
                    st_proposto += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                       (k => k.Value == "PROPOSTO").Select(k => k.Key).FirstOrDefault() +
                                        " AND DateTime >= Convert(DATETIME,'" + dtaProposto_TxtFrom.Text.Trim() + "',103)" +
                                        " AND DateTime <= Convert(DATETIME,'" + dtaProposto_TxtTo.Text.Trim() + " 23:59:59',103)";
                }
                else if (string.IsNullOrEmpty(dtaProposto_TxtFrom.Text) && (!string.IsNullOrEmpty(dtaProposto_TxtTo.Text)))
                {
                    st_proposto += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                        (k => k.Value == "PROPOSTO").Select(k => k.Key).FirstOrDefault() +
                                         " AND DateTime <= Convert(DATETIME,'" + dtaProposto_TxtTo.Text.Trim() + " 23:59:59',103)";
                }
                else if (!string.IsNullOrEmpty(dtaProposto_TxtFrom.Text) && (string.IsNullOrEmpty(dtaProposto_TxtTo.Text)))
                {
                    //Verifico se è selezionata la Single Text
                    if (ddl_dtaProposto.SelectedIndex == 0 || ddl_dtaProposto.SelectedIndex == 2)
                    {
                        st_proposto += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                          (k => k.Value == "PROPOSTO").Select(k => k.Key).FirstOrDefault() +
                                          " AND DateTime between Convert(DATETIME,'" + dtaProposto_TxtFrom.Text.Trim() + " 00:00:00',103)" +
                                          " AND Convert(DATETIME,'" + dtaProposto_TxtFrom.Text.Trim() + " 23:59:59',103)";
                    }
                    else
                        st_proposto += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                             (k => k.Value == "PROPOSTO").Select(k => k.Key).FirstOrDefault() +
                                              " AND DateTime >= Convert(DATETIME,'" + dtaProposto_TxtFrom.Text.Trim() + "',103)";
                }
                ////************************************************
                //// FINE
                ////************************************************
                #endregion

                #region analisi Approvata
                ////************************************************
                ////Calendar data analisi Approvata
                ////************************************************
                if (!string.IsNullOrEmpty(dtaApprovato_TxtFrom.Text) && (!string.IsNullOrEmpty(dtaApprovato_TxtTo.Text)))
                {
                    st_approvato += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                        (k => k.Value == "APPROVATO").Select(k => k.Key).FirstOrDefault() +
                                         " AND DateTime >= Convert(DATETIME,'" + dtaApprovato_TxtFrom.Text.Trim() + "',103)" +
                                         " AND DateTime <= Convert(DATETIME,'" + dtaApprovato_TxtTo.Text.Trim() + " 23:59:59',103)";
                }
                else if (string.IsNullOrEmpty(dtaApprovato_TxtFrom.Text) && (!string.IsNullOrEmpty(dtaApprovato_TxtTo.Text)))
                {
                    st_approvato += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                      (k => k.Value == "APPROVATO").Select(k => k.Key).FirstOrDefault() +
                                       " AND DateTime <= Convert(DATETIME,'" + dtaApprovato_TxtTo.Text.Trim() + " 23:59:59',103)";
                }
                else if (!string.IsNullOrEmpty(dtaApprovato_TxtFrom.Text) && (string.IsNullOrEmpty(dtaApprovato_TxtTo.Text)))
                {
                    //Verifico se è selezionata la Single Text
                    if (ddl_dtaApprovato.SelectedIndex == 0 || ddl_dtaApprovato.SelectedIndex == 2)
                    {
                        st_approvato += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                          (k => k.Value == "APPROVATO").Select(k => k.Key).FirstOrDefault() +
                                          " AND DateTime between Convert(DATETIME,'" + dtaApprovato_TxtFrom.Text.Trim() + " 00:00:00',103)" +
                                          " AND Convert(DATETIME,'" + dtaApprovato_TxtFrom.Text.Trim() + " 23:59:59',103)";
                    }
                    else
                        st_approvato += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                        (k => k.Value == "APPROVATO").Select(k => k.Key).FirstOrDefault() +
                                         " AND DateTime >= Convert(DATETIME,'" + dtaApprovato_TxtFrom.Text.Trim() + "',103)";
                }
                ////************************************************
                //// FINE
                ////************************************************
                #endregion

                #region In esecuzione
                ////************************************************
                ////Calendar data  In esecuzione
                ////************************************************
                if (!string.IsNullOrEmpty(dtaEseguito_TxtFrom.Text) && (!string.IsNullOrEmpty(dtaEseguito_TxtTo.Text)))
                {
                    st_inesecuzione += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                       (k => k.Value == "IN ESECUZIONE").Select(k => k.Key).FirstOrDefault() +
                                        " AND DateTime >= Convert(DATETIME,'" + dtaEseguito_TxtFrom.Text.Trim() + "',103)" +
                                        " AND DateTime <= Convert(DATETIME,'" + dtaEseguito_TxtTo.Text.Trim() + " 23:59:59',103)";
                }
                else if (string.IsNullOrEmpty(dtaEseguito_TxtFrom.Text) && (!string.IsNullOrEmpty(dtaEseguito_TxtTo.Text)))
                {
                    st_inesecuzione += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                     (k => k.Value == "IN ESECUZIONE").Select(k => k.Key).FirstOrDefault() +
                                      " AND DateTime <= Convert(DATETIME,'" + dtaEseguito_TxtTo.Text.Trim() + " 23:59:59',103)";
                }
                else if (!string.IsNullOrEmpty(dtaEseguito_TxtFrom.Text) && (string.IsNullOrEmpty(dtaEseguito_TxtTo.Text)))
                {
                    //Verifico se è selezionata la Single Text
                    if (ddl_dtaEseguito.SelectedIndex == 0 || ddl_dtaEseguito.SelectedIndex == 2)
                    {
                        st_inesecuzione += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                          (k => k.Value == "IN ESECUZIONE").Select(k => k.Key).FirstOrDefault() +
                                             " AND DateTime between Convert(DATETIME,'" + dtaEseguito_TxtFrom.Text.Trim() + " 00:00:00',103)" +
                                          " AND Convert(DATETIME,'" + dtaEseguito_TxtFrom.Text.Trim() + " 23:59:59',103)";
                    }
                    else
                        st_inesecuzione += "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                              (k => k.Value == "IN ESECUZIONE").Select(k => k.Key).FirstOrDefault() +
                                               " AND DateTime >= Convert(DATETIME,'" + dtaEseguito_TxtFrom.Text.Trim() + "',103)";
                }
                ////************************************************
                //// FINE
                ////************************************************
                #endregion

                //**************************************************************************************************
                //  FINE Modifiche varie su tutti i filtri ricerca
                //**************************************************************************************************

                int Finger = 0;
                ////FILTRO IN BASE ALLE OPZIONI DI STATO:
                switch (rblStateType.SelectedItem.Value)
                {
                    //Completi
                    case "C":
                        st_effettuato = "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                                         (k => k.Value == "EFFETTUATO").Select(k => k.Key).FirstOrDefault() +
                                          " AND DateTime >= Convert(DATETIME,'01/01/1900',103)" +
                                          " AND DateTime <= Convert(DATETIME,'01/01/2050',103)";
                        Finger = 1;
                        break;
                    //In errore
                    case "E":
                        st_inerrore = "DisposalStateType_ID = " + UIManager.ArchiveManager._dictionaryDisposalState.Where
                            (k => k.Value == "IN ERRORE").Select(k => k.Key).FirstOrDefault() +
                             " AND DateTime >= Convert(DATETIME,'01/01/1900',103)" +
                             " AND DateTime <= Convert(DATETIME,'01/01/2050',103)";
                        Finger = 2;
                        break;
                    //Tutti
                    case "T":
                        Finger = 3;
                        break;
                }

                List<ARCHIVE_DisposalForSearch> _atr = UIManager.ArchiveManager.GetAllARCHIVE_DisposalFilterForSearch(ConcatenateSelect(st_indefinizione),
                                                                                                                       ConcatenateSelect(st_analisicompletata),
                                                                                                                       ConcatenateSelect(st_proposto),
                                                                                                                       ConcatenateSelect(st_approvato),
                                                                                                                       ConcatenateSelect(st_inesecuzione),
                                                                                                                       ConcatenateSelect(st_effettuato),
                                                                                                                       ConcatenateSelect(st_inerrore),
                                                                                                                       Finger);


                //IDAMM
                var query = _atr.Where(x => x.ID_Amministrazione == Convert.ToInt32(UserManager.GetInfoUser().idAmministrazione));

                ////IDDisposal
                if (!string.IsNullOrEmpty(txt_fineIdDisposal_C.Text.Trim()) && !string.IsNullOrEmpty(txt_initIdDisposal_C.Text.Trim()))
                {
                    query = query.Where(x => x.System_id >= Convert.ToInt32(txt_initIdDisposal_C.Text.Trim()) && x.System_id <= Convert.ToInt32(txt_fineIdDisposal_C.Text.Trim()));
                }
                else if (!string.IsNullOrEmpty(txt_fineIdDisposal_C.Text.Trim()) && string.IsNullOrEmpty(txt_initIdDisposal_C.Text.Trim()))
                {
                    query = query.Where(x => x.System_id <= Convert.ToInt32(txt_fineIdDisposal_C.Text.Trim()));
                }
                else if (string.IsNullOrEmpty(txt_fineIdDisposal_C.Text.Trim()) && !string.IsNullOrEmpty(txt_initIdDisposal_C.Text.Trim()))
                {
                    query = query.Where(x => x.System_id == Convert.ToInt32(txt_initIdDisposal_C.Text.Trim()));
                }

                ////Descrizione
                if (!string.IsNullOrEmpty(TxtDescrizioneScarto.Text.Trim()))
                {
                    query = query.Where(x => x.Description.ToUpper().Contains(TxtDescrizioneScarto.Text.Trim().ToUpper()));
                }

                if (query != null)
                {
                    if (query.ToList().Count() > 0)
                    {
                        gvResultDisposal.DataSource = query.Cast<DocsPaWR.ARCHIVE_DisposalForSearch>().ToList();
                        gvResultDisposal.DataBind();
                    }
                    else
                    {
                        gvResultDisposal.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerResultSearchDisposal();
                        gvResultDisposal.DataBind();
                        gvResultDisposal.Rows[0].Visible = false;
                    }
                }
                UpnlGrid.Update();
                UpnlNumeroDisposal.Update();
                this.transferLblNumeroScarti.Text = this.GetLabel("transferLblNumeroScarti");
                this.transferLblNumeroScarti.Text = this.transferLblNumeroScarti.Text.Replace("{0}", (gvResultDisposal.Rows.Count == 1) ? "0" : gvResultDisposal.Rows.Count.ToString());
            }
            catch
            {

            }
        }

        private string ConcatenateSelect(string st_indefinizione)
        {
            string select = string.Empty;

            if (st_indefinizione != string.Empty)
            {
                select += "(";
                select += " select Disposal_ID, DateTime,";
                select += " DisposalStateType_ID";
                select += " from DOCSADM.ARCHIVE_DisposalState";
                select += " where ";
                select += st_indefinizione + ")";
            }
            return select;
        }

        #endregion

        protected void SearchDisposalSearch_Click(object sender, EventArgs e)
        {
            this.GetAllFilterForGetDisposal();
        }

        /// <summary>
        /// Semplice concatenatore per una SQL con clausola IN dinamica.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string GetSQLinStringFromPolicy(List<Int32> list)
        {
            int brk = 0;
            string sqlIn = string.Empty;

            foreach (int _pol in list)
            {
                if (brk == 0)
                {
                    sqlIn = "" + _pol;
                    brk++;
                }
                else
                    sqlIn += "," + _pol;
            }
            sqlIn += "";

            return sqlIn;
        }

        protected void btnDisposalDetails_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn1 = sender as ImageButton;
            GridViewRow row = (GridViewRow)ibtn1.NamingContainer;
            int _idScartoResult = Convert.ToInt32(gvResultDisposal.DataKeys[row.RowIndex].Value);
            Session["PAGESTATE"] = "SEA";
            Session["ID_SCARTO"] = _idScartoResult;
            Response.Redirect("~/Deposito/Scarto.aspx");
        }

        protected void SearchDisposalRemoveFilters_Click(object sender, EventArgs e)
        {
            RemoveFilters();
            UpContainer.Update();
            upRadioButtonStati.Update();
        }
    }
}