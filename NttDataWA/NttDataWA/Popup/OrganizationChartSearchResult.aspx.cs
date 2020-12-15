using System;
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

namespace NttDataWA.Popup
{
    public partial class OrganizationChartSearchResult : System.Web.UI.Page
    {

        #region Properties

        private string Codice
        {
            get
            {
                return HttpContext.Current.Session["OrganizationChart_Code"] as string;
            }
            set
            {
                HttpContext.Current.Session["OrganizationChart_Code"] = value;
            }
        }

        private string Descrizione
        {
            get
            {
                return HttpContext.Current.Session["OrganizationChart_Description"] as string;
            }
            set
            {
                HttpContext.Current.Session["OrganizationChart_Description"] = value;
            }
        }

        private string IdAmm
        {
            get
            {
                return HttpContext.Current.Session["OrganizationChart_IdAmm"] as string;
            }
            set
            {
                HttpContext.Current.Session["OrganizationChart_IdAmm"] = value;
            }
        }

        private string Tipo
        {
            get
            {
                return HttpContext.Current.Session["OrganizationChart_Type"] as string;
            }
            set
            {
                HttpContext.Current.Session["OrganizationChart_Type"] = value;
            }
        }

        private bool SearchHistoricized
        {
            get
            {
                bool retValue = false;
                if (HttpContext.Current.Session["OrganizationChart_SearchHistoricized"]!=null)
                    retValue = (bool)HttpContext.Current.Session["OrganizationChart_SearchHistoricized"];
                return retValue;
            }
            set
            {
                HttpContext.Current.Session["OrganizationChart_SearchHistoricized"] = value;
            }
        }

        private bool AlreadyClosed
        {
            get
            {
                bool retValue = false;
                if (HttpContext.Current.Session["OrganizationChart_AlreadyClosed"] != null)
                    retValue = (bool)HttpContext.Current.Session["OrganizationChart_AlreadyClosed"];
                return retValue;
            }
            set
            {
                HttpContext.Current.Session["OrganizationChart_AlreadyClosed"] = value;
            }
        }

        private DataSet dsRisultato
        {
            get
            {
                return HttpContext.Current.Session["OrganizationChart_dsRisultato"] as DataSet;
            }
            set
            {
                HttpContext.Current.Session["OrganizationChart_dsRisultato"] = value;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !this.AlreadyClosed)
            {
                this.InitializePage();
            }

            this.RefreshScript();
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.CloseMask();
        }

        #endregion

        #region Methods

        protected void InitializePage()
        {
            this.InitializeLanguage();
            this.InizializeSearch();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.dg_listaRicerca.Columns[0].HeaderText = "ID";
            this.dg_listaRicerca.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("OrganizationChartSearchResultCode", language);
            this.dg_listaRicerca.Columns[3].HeaderText = "IDParent";
        }

        private string GetLabel(string id)
        {
            return Utils.Languages.GetLabelFromCode(id, UIManager.UserManager.GetUserLanguage());
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        private void CloseMask()
        {
            this.CloseMask(string.Empty);
        }

        private void CloseMask(string retValue)
        {
            this.AlreadyClosed = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('OrganizationChartSearchResult', '" + retValue + "');", true);
        }

        private void InizializeSearch()
        {
            int num_trovati = 0;
            string returnValue = string.Empty;
            string codice = this.Codice.Trim();
            string descrizione = this.Descrizione.Trim();
            string idAmm = this.IdAmm.Trim();
            string tipo = this.Tipo.Trim();
            bool searchHistoricized = this.SearchHistoricized;

            if (!string.IsNullOrEmpty(idAmm))
            {
                this.pageTitle.Text = this.GetLabel("OrganizationChartSearchResult") + codice + " " + descrizione + "";
                this.pageTitle.Text += ", " + dsRisultato.Tables[0].Rows.Count.ToString() + " records.";

                this.SetHeaderDG(tipo);

                DataView dv = dsRisultato.Tables["LISTA_RICERCA"].DefaultView;
                dv.Sort = "Descrizione ASC";
                this.dg_listaRicerca.DataSource = dv;
                this.dg_listaRicerca.DataBind();
            }
        }

        private void SetHeaderDG(string tipo)
        {
            switch (tipo)
            {
                case "U":
                    this.dg_listaRicerca.Columns[2].HeaderText = this.GetLabel("OrganizationChartSearchResultUODescription");
                    this.dg_listaRicerca.Columns[4].HeaderText = this.GetLabel("OrganizationChartSearchResultUOSupDescription");
                    break;
                case "R":
                    this.dg_listaRicerca.Columns[2].HeaderText = this.GetLabel("OrganizationChartSearchResultRoleDescription");
                    this.dg_listaRicerca.Columns[4].HeaderText = this.GetLabel("OrganizationChartSearchResultRoleSupDescription");
                    break;
                case "PN":
                    this.dg_listaRicerca.Columns[2].HeaderText = this.GetLabel("OrganizationChartSearchResultNameDescription");
                    this.dg_listaRicerca.Columns[4].HeaderText = this.GetLabel("OrganizationChartSearchResultNameSupDescription");
                    break;
                case "PC":
                    this.dg_listaRicerca.Columns[2].HeaderText = this.GetLabel("OrganizationChartSearchResultSurnameDescription");
                    this.dg_listaRicerca.Columns[4].HeaderText = this.GetLabel("OrganizationChartSearchResultSurnameSupDescription");
                    break;
            }
        }

        private void InitializeDataSetRisultato()
        {
            dsRisultato = new DataSet();
            DataColumn dc;
            dsRisultato.Tables.Add("LISTA_RICERCA");
            dc = new DataColumn("IDCorrGlob");
            dsRisultato.Tables["LISTA_RICERCA"].Columns.Add(dc);
            dc = new DataColumn("Codice");
            dsRisultato.Tables["LISTA_RICERCA"].Columns.Add(dc);
            dc = new DataColumn("Descrizione");
            dsRisultato.Tables["LISTA_RICERCA"].Columns.Add(dc);
            dc = new DataColumn("IDParent");
            dsRisultato.Tables["LISTA_RICERCA"].Columns.Add(dc);
            dc = new DataColumn("DescParent");
            dsRisultato.Tables["LISTA_RICERCA"].Columns.Add(dc);
        }

        #endregion

    }
}