using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SAAdminTool.UserControls
{
    /// <summary>
    /// User control per la ricerca dei documenti consolidati
    /// </summary>
    public partial class DocumentConsolidationSearchPanel : System.Web.UI.UserControl
    {

        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.PreRender += new System.EventHandler(this.document_consolidation_PreRender);
        }

        private void document_consolidation_PreRender(object sender, EventArgs e)
        {
                //carico il corrispondente selezionato, se esiste
                if (Session["rubrica.campoCorrispondente"] != null)
                {
                    SAAdminTool.DocsPaWR.Corrispondente corr = (SAAdminTool.DocsPaWR.Corrispondente)Session["rubrica.campoCorrispondente"];
                    if (corr != null)
                    {
                        ctlUtenteRuoloConsolidante.DESCRIZIONE_TEXT = corr.descrizione;
                        ctlUtenteRuoloConsolidante.CODICE_TEXT = corr.codiceRubrica;
                        ctlUtenteRuoloConsolidante.ID_CORRISPONDENTE = corr.systemId;
                        Session.Remove("rubrica.campoCorrispondente");
                    }
                }

                else
                {
                    if (!string.IsNullOrEmpty(ctlUtenteRuoloConsolidante.CODICE_TEXT) && !string.IsNullOrEmpty(ctlUtenteRuoloConsolidante.DESCRIZIONE_TEXT) && ctlUtenteRuoloConsolidante.TIPO.Equals("U"))
                    {
                        ctlUtenteRuoloConsolidante.DESCRIZIONE_TEXT = "";
                        ctlUtenteRuoloConsolidante.CODICE_TEXT = "";
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "errore_uo", "alert('Inserire soltato utenti e ruoli')", true);
                    }
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.HasDocumentConsolidationRights)
            {
                this.Visible = false;
            }
            else
            {
                this.Visible = true;

                // Filtro di ricerca su corrispondenti interni
                this.ctlUtenteRuoloConsolidante.TIPO_CORRISPONDENTE = "INTERNI";
                this.ctlUtenteRuoloConsolidante.FILTRO = "NO_UO";

                if (!this.IsPostBack)
                {
                    this.PerformActionSelectDataConsolidamento();
                }
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cboDataConsolidamento_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.PerformActionSelectDataConsolidamento();
        }

        /// <summary>
        /// Azione di selezione tipo filtro per data
        /// </summary>
        protected void PerformActionSelectDataConsolidamento()
        {
            switch (this.cboDataConsolidamento.SelectedValue)
            {
                case "0":
                    this.txtDataConsolidamento.Visible = true;
                    this.txtDataConsolidamento.btn_Cal.Visible = true;
                    this.txtDataConsolidamento.btn_Cal.Enabled = true;
                    this.txtDataConsolidamento.txt_Data.Visible = true;
                    this.txtDataConsolidamento.txt_Data.Enabled = true;
                    this.txtDataConsolidamentoFinale.Visible = false;
                    this.txtDataConsolidamentoFinale.btn_Cal.Visible = false;
                    this.txtDataConsolidamentoFinale.txt_Data.Visible = false;
                    this.txtDataConsolidamentoFinale.txt_Data.Text = string.Empty;
                    break;

                case "1":
                    this.txtDataConsolidamento.Visible = true;
                    this.txtDataConsolidamento.btn_Cal.Visible = true;
                    this.txtDataConsolidamento.btn_Cal.Enabled = true;
                    this.txtDataConsolidamento.txt_Data.Visible = true;
                    this.txtDataConsolidamento.txt_Data.Enabled = true;
                    this.txtDataConsolidamentoFinale.Visible = true;
                    this.txtDataConsolidamentoFinale.btn_Cal.Visible = true;
                    this.txtDataConsolidamentoFinale.btn_Cal.Enabled = true;
                    this.txtDataConsolidamentoFinale.txt_Data.Visible = true;
                    this.txtDataConsolidamentoFinale.txt_Data.Enabled = true;
                    break;

                case "2":
                    this.txtDataConsolidamento.Visible = true;
                    this.txtDataConsolidamento.btn_Cal.Visible = true;
                    this.txtDataConsolidamento.btn_Cal.Enabled = false;
                    this.txtDataConsolidamento.txt_Data.Visible = true;
                    this.txtDataConsolidamento.txt_Data.Text = SAAdminTool.DocumentManager.toDay();
                    this.txtDataConsolidamento.txt_Data.Enabled = false;
                    this.txtDataConsolidamentoFinale.Visible = false;
                    this.txtDataConsolidamentoFinale.btn_Cal.Visible = false;
                    this.txtDataConsolidamentoFinale.txt_Data.Visible = false;
                    break;

                case "3":
                    this.txtDataConsolidamento.Visible = true;
                    this.txtDataConsolidamento.btn_Cal.Enabled = false;
                    this.txtDataConsolidamento.txt_Data.Text = SAAdminTool.DocumentManager.getFirstDayOfWeek();
                    this.txtDataConsolidamento.txt_Data.Enabled = false;
                    this.txtDataConsolidamentoFinale.Visible = true;
                    this.txtDataConsolidamentoFinale.btn_Cal.Visible = true;
                    this.txtDataConsolidamentoFinale.btn_Cal.Enabled = false;
                    this.txtDataConsolidamentoFinale.txt_Data.Text = SAAdminTool.DocumentManager.getLastDayOfWeek();
                    this.txtDataConsolidamentoFinale.txt_Data.Visible = true;
                    this.txtDataConsolidamentoFinale.txt_Data.Enabled = false;
                    //this.txtDataConsolidamentoFinale.txt_Data.Text = string.Empty;
                    break;

                case "4":
                    this.txtDataConsolidamento.Visible = true;
                    this.txtDataConsolidamento.btn_Cal.Enabled = false;
                    this.txtDataConsolidamento.txt_Data.Text = SAAdminTool.DocumentManager.getFirstDayOfMonth();
                    this.txtDataConsolidamento.txt_Data.Enabled = false;
                    this.txtDataConsolidamentoFinale.Visible = true;
                    this.txtDataConsolidamentoFinale.btn_Cal.Visible = true;
                    this.txtDataConsolidamentoFinale.btn_Cal.Enabled = false;
                    this.txtDataConsolidamentoFinale.txt_Data.Text = SAAdminTool.DocumentManager.getLastDayOfMonth();
                    this.txtDataConsolidamentoFinale.txt_Data.Visible = true;
                    this.txtDataConsolidamentoFinale.txt_Data.Enabled = false;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected DocsPaWR.FiltroRicerca FindFilter(DocsPaWR.FiltroRicerca[] filters, string key)
        {
            return filters.Where(e => e.argomento == key).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        public virtual void LoadFilters(DocsPaWR.FiltroRicerca[] filters)
        {
            DocsPaWR.FiltroRicerca filter = this.FindFilter(filters, DocsPaWR.FiltriDocumento.STATO_CONSOLIDAMENTO.ToString());

            if (filter != null)
            {
                foreach (string itm in filter.valore.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries ))
                {
                    ListItem item = this.lstFiltriConsolidamento.Items.FindByValue(itm);

                    if (item != null)
                        item.Selected = true;
                }
            }

            bool rangeFilter = false;

            filter = this.FindFilter(filters, DocsPaWR.FiltriDocumento.DATA_CONSOLIDAMENTO_DA.ToString());

            if (filter != null)
                this.txtDataConsolidamento.Text = filter.valore;

            filter = this.FindFilter(filters, DocsPaWR.FiltriDocumento.DATA_CONSOLIDAMENTO_A.ToString());

            if (filter != null)
            {
                this.txtDataConsolidamentoFinale.Text = filter.valore;
                rangeFilter = true;
            }

          /*  if (rangeFilter)
                this.cboDataConsolidamento.SelectedValue = "1";
            else
                this.cboDataConsolidamento.SelectedValue = "0";
           * */

            this.PerformActionSelectDataConsolidamento();

            // Ricerca per utente consolidante
            filter = this.FindFilter(filters, DocsPaWR.FiltriDocumento.ID_UTENTE_CONSOLIDANTE.ToString());

            string idCorrispondente = string.Empty;

            if (filter != null)
            {
                idCorrispondente = filter.valore;
            }
            else
            {
                // Ricerca per ruolo consolidante
                filter = this.FindFilter(filters, DocsPaWR.FiltriDocumento.ID_RUOLO_CONSOLIDANTE.ToString());

                if (filter != null)
                {
                    idCorrispondente = filter.valore;
                }
            }

            if (!string.IsNullOrEmpty(idCorrispondente))
            {
                DocsPaWR.Corrispondente result = ProxyManager.getWS().AddressbookGetCorrispondenteBySystemId(idCorrispondente);

                if (result != null)
                {
                    if (!ctlUtenteRuoloConsolidante.TIPO.Equals("U"))
                    {
                        this.ctlUtenteRuoloConsolidante.ID_CORRISPONDENTE = idCorrispondente;
                        this.ctlUtenteRuoloConsolidante.CODICE_TEXT = result.codiceRubrica;
                        this.ctlUtenteRuoloConsolidante.DESCRIZIONE_TEXT = result.descrizione;
                    }
                }
            }
        }

        /// <summary>
        /// Costruzione dei filtri di ricerca
        /// </summary>
        /// <returns></returns>
        public virtual DocsPaWR.FiltroRicerca[] GetFilters()
        {
            List<DocsPaWR.FiltroRicerca> filters = new List<DocsPaWR.FiltroRicerca>();

            DocsPaWR.FiltroRicerca filterItem = null;

            foreach (ListItem itm in lstFiltriConsolidamento.Items)
            {
                if (itm.Selected)
                {
                    if (filterItem == null) 
                        filterItem = new DocsPaWR.FiltroRicerca { argomento = DocsPaWR.FiltriDocumento.STATO_CONSOLIDAMENTO.ToString() };

                    if (!string.IsNullOrEmpty(filterItem.valore))
                        filterItem.valore += "|";

                    filterItem.valore += itm.Value;
                }
            }

            if (filterItem != null)
                filters.Add(filterItem);

            DateTime date1;
            if (DateTime.TryParse(this.txtDataConsolidamento.Text, out date1))
            {
                filters.Add(new DocsPaWR.FiltroRicerca
                {
                    argomento = DocsPaWR.FiltriDocumento.DATA_CONSOLIDAMENTO_DA.ToString(),
                    valore = this.txtDataConsolidamento.Text 
                });
            }

            DateTime date2;
            if (DateTime.TryParse(this.txtDataConsolidamentoFinale.Text, out date2))
            {
                filters.Add(new DocsPaWR.FiltroRicerca
                {
                    argomento = DocsPaWR.FiltriDocumento.DATA_CONSOLIDAMENTO_A.ToString(),
                    valore = this.txtDataConsolidamentoFinale.Text
                });
            }

            if (!string.IsNullOrEmpty(this.ctlUtenteRuoloConsolidante.ID_CORRISPONDENTE))
            {
                string filterName = string.Empty;

                DocsPaWR.Corrispondente temp = new DocsPaWR.Corrispondente();

                if (string.IsNullOrEmpty(ctlUtenteRuoloConsolidante.TIPO))
                {
                    temp = ProxyManager.getWS().AddressbookGetCorrispondenteBySystemId(ctlUtenteRuoloConsolidante.ID_CORRISPONDENTE);
                }

                if (ctlUtenteRuoloConsolidante.TIPO == "P" || (temp.tipoCorrispondente!=null && temp.tipoCorrispondente.Equals("P")))
                {
                    filterName = DocsPaWR.FiltriDocumento.ID_UTENTE_CONSOLIDANTE.ToString();
                }
                else
                {
                    if (ctlUtenteRuoloConsolidante.TIPO == "R" || (temp.tipoCorrispondente!=null && temp.tipoCorrispondente.Equals("R")))
                    {
                        filterName = DocsPaWR.FiltriDocumento.ID_RUOLO_CONSOLIDANTE.ToString();
                    }
                }

                if (!string.IsNullOrEmpty(filterName))
                {
                    filters.Add(new DocsPaWR.FiltroRicerca
                    {
                        argomento = filterName,
                        valore = this.ctlUtenteRuoloConsolidante.ID_CORRISPONDENTE
                    });
                }
            }

            return filters.ToArray();
        }

        /// <summary>
        /// Verifica se il consolidamento è abilitato in amministrazione
        /// </summary>
        protected bool HasDocumentConsolidationRights
        {
            get
            {
                if (this.ViewState["HasDocumentConsolidationRights"] == null)
                    this.ViewState["HasDocumentConsolidationRights"] = ProxyManager.getWS().HasDocumentConsolidationRights(UserManager.getInfoUtente());
                return (bool)this.ViewState["HasDocumentConsolidationRights"];
            }
        }

        protected void lstFiltriConsolidamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFiltriConsolidamento.SelectedValue == "0" && !this.lstFiltriConsolidamento.Items[1].Selected && !this.lstFiltriConsolidamento.Items[2].Selected)
            {
               // lstFiltriConsolidamento.Items[1].Enabled = false;
              //  lstFiltriConsolidamento.Items[1].Selected = false;
               // lstFiltriConsolidamento.Items[2].Enabled = false;
              //  lstFiltriConsolidamento.Items[2].Selected = false;
             /*   if (cboDataConsolidamento.Items[0].Selected || cboDataConsolidamento.Items[2].Selected)
                { txtDataConsolidamento.txt_Data.Text = string.Empty; }
                else
                {
                    txtDataConsolidamento.txt_Data.Text = string.Empty;
                    txtDataConsolidamentoFinale.txt_Data.Text = string.Empty;
                }
                cboDataConsolidamento.Enabled = false;
                txtDataConsolidamento.txt_Data.Enabled = false;
                txtDataConsolidamento.EnableBtnCal = false;
                ctlUtenteRuoloConsolidante.DISABLED_CORR = true;
                ctlUtenteRuoloConsolidante.ID_CORRISPONDENTE = string.Empty;
              * */
                this.pnl_data_cons.Visible = false;
                txtDataConsolidamento.txt_Data.Text = string.Empty;
                txtDataConsolidamentoFinale.txt_Data.Text = string.Empty;
                ctlUtenteRuoloConsolidante.DESCRIZIONE_TEXT = string.Empty;
                ctlUtenteRuoloConsolidante.CODICE_TEXT = string.Empty;
                this.cboDataConsolidamento.SelectedIndex = 0;
                txtDataConsolidamento.txt_Data.Enabled = true;
                txtDataConsolidamentoFinale.txt_Data.Enabled = true;
                txtDataConsolidamento.btn_Cal.Enabled = true;
                txtDataConsolidamentoFinale.btn_Cal.Enabled = true;
          
            }
            else
            {
                if (!this.lstFiltriConsolidamento.Items[0].Selected && !this.lstFiltriConsolidamento.Items[1].Selected && !this.lstFiltriConsolidamento.Items[2].Selected)
                {
                    this.pnl_data_cons.Visible = false;
                    txtDataConsolidamento.txt_Data.Text = string.Empty;
                    txtDataConsolidamentoFinale.txt_Data.Text = string.Empty;
                    ctlUtenteRuoloConsolidante.DESCRIZIONE_TEXT = string.Empty;
                    ctlUtenteRuoloConsolidante.CODICE_TEXT = string.Empty;
                    this.cboDataConsolidamento.SelectedIndex = 0;
                    txtDataConsolidamento.txt_Data.Enabled = true;
                    txtDataConsolidamentoFinale.txt_Data.Enabled = true;
                    txtDataConsolidamento.btn_Cal.Enabled = true;
                    txtDataConsolidamentoFinale.btn_Cal.Enabled = true;
                    
                }
                /*   lstFiltriConsolidamento.Items[1].Enabled = true;
                   lstFiltriConsolidamento.Items[2].Enabled = true;
                   cboDataConsolidamento.Enabled = true;
                   txtDataConsolidamento.txt_Data.Enabled = true;
                   txtDataConsolidamento.EnableBtnCal = true;
                   ctlUtenteRuoloConsolidante.DISABLED_CORR = false;
                   */
                else
                {
                    this.pnl_data_cons.Visible = true;
                    ctlUtenteRuoloConsolidante.DISABLED_CORR = false;
                }

            }
        }

        public string getValueDdlIntervallo()
        {
            string result = "";

            result = this.cboDataConsolidamento.SelectedValue;

            return result;
        }

        public void setStateDdlIntervallo(string val)
        {
            this.cboDataConsolidamento.SelectedIndex = Int32.Parse(val);
            switch (val)
            {
                case "0":
                    this.txtDataConsolidamento.Visible = true;
                    this.txtDataConsolidamento.btn_Cal.Visible = true;
                    this.txtDataConsolidamento.btn_Cal.Enabled = true;
                    this.txtDataConsolidamento.txt_Data.Visible = true;
                    this.txtDataConsolidamento.txt_Data.Enabled = true;
                    this.txtDataConsolidamentoFinale.Visible = false;
                    this.txtDataConsolidamentoFinale.btn_Cal.Visible = false;
                    this.txtDataConsolidamentoFinale.txt_Data.Visible = false;
                    this.txtDataConsolidamentoFinale.txt_Data.Text = string.Empty;
                    break;

                case "1":
                    this.txtDataConsolidamento.Visible = true;
                    this.txtDataConsolidamento.btn_Cal.Visible = true;
                    this.txtDataConsolidamento.btn_Cal.Enabled = true;
                    this.txtDataConsolidamento.txt_Data.Visible = true;
                    this.txtDataConsolidamento.txt_Data.Enabled = true;
                    this.txtDataConsolidamentoFinale.Visible = true;
                    this.txtDataConsolidamentoFinale.btn_Cal.Visible = true;
                    this.txtDataConsolidamentoFinale.btn_Cal.Enabled = true;
                    this.txtDataConsolidamentoFinale.txt_Data.Visible = true;
                    this.txtDataConsolidamentoFinale.txt_Data.Enabled = true;
                    break;

                case "2":
                    this.txtDataConsolidamento.Visible = true;
                    this.txtDataConsolidamento.btn_Cal.Visible = true;
                    this.txtDataConsolidamento.btn_Cal.Enabled = false;
                    this.txtDataConsolidamento.txt_Data.Visible = true;
                    this.txtDataConsolidamento.txt_Data.Enabled = false;
                    this.txtDataConsolidamentoFinale.Visible = false;
                    this.txtDataConsolidamentoFinale.btn_Cal.Visible = false;
                    this.txtDataConsolidamentoFinale.txt_Data.Visible = false;
                    break;

                case "3":
                    this.txtDataConsolidamento.Visible = true;
                    this.txtDataConsolidamento.btn_Cal.Enabled = false;
                    this.txtDataConsolidamento.txt_Data.Enabled = false;
                    this.txtDataConsolidamentoFinale.Visible = true;
                    this.txtDataConsolidamentoFinale.btn_Cal.Visible = true;
                    this.txtDataConsolidamentoFinale.btn_Cal.Enabled = false;
                    this.txtDataConsolidamentoFinale.txt_Data.Visible = true;
                    this.txtDataConsolidamentoFinale.txt_Data.Enabled = false;
                    //this.txtDataConsolidamentoFinale.txt_Data.Text = string.Empty;
                    break;

                case "4":
                    this.txtDataConsolidamento.Visible = true;
                    this.txtDataConsolidamento.btn_Cal.Enabled = false;
                    this.txtDataConsolidamento.txt_Data.Enabled = false;
                    this.txtDataConsolidamentoFinale.Visible = true;
                    this.txtDataConsolidamentoFinale.btn_Cal.Visible = true;
                    this.txtDataConsolidamentoFinale.btn_Cal.Enabled = false;
                    this.txtDataConsolidamentoFinale.txt_Data.Visible = true;
                    this.txtDataConsolidamentoFinale.txt_Data.Enabled = false;
                    break;
            }
        }

        public void setStateResaerchConsolidation(SAAdminTool.DocsPaWR.FiltroRicerca f)
        {
            foreach (string itm in f.valore.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            if (itm == "0")
            {
                this.pnl_data_cons.Visible = false;
                this.lstFiltriConsolidamento.Items[0].Selected = true;
            }
            else
            {
                this.pnl_data_cons.Visible = true;
                if (itm == "1")
                {
                    this.lstFiltriConsolidamento.Items[1].Selected = true;
                }
                else
                {
                    this.lstFiltriConsolidamento.Items[2].Selected = true;
                }
            }
        }

        public void setDataConsolidamentoDa(string data)
        {
            this.txtDataConsolidamento.txt_Data.Text = data;
        }

        public void setDataConsolidamentoA(string data)
        {
            this.txtDataConsolidamentoFinale.txt_Data.Text = data;
        }


    }
}