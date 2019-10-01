using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;


namespace NttDataWA.Popup
{
    public partial class SaveSearch : System.Web.UI.Page
    {

        #region fields

        private SearchManager schedaRicerca = null;
        protected bool showGridPersonalization;

        #endregion

        #region Properties

        private bool IsModify
        {
            get
            {
                return Request.QueryString["modify"] != null && !string.IsNullOrEmpty(idRicercaSalvata) ? true : false;
            }
        }

        private bool IsAdl
        {
            get
            {
                return Request.QueryString["IsAdl"] != null ? true : false;
            }
        }

        private string idRicercaSalvata
        {
            get
            {
                return Session["idRicercaSalvata"] as string;
            }
            set
            {
                Session["idRicercaSalvata"] = value;
            }
        }

        private string tipoRicercaSalvata
        {
            get
            {
                return Session["tipoRicercaSalvata"] as string;
            }
            set
            {
                Session["tipoRicercaSalvata"] = value;
            }
        }

        #endregion

        #region events

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                object obj = null;
                if ((obj = Session[SearchManager.SESSION_KEY]) != null)
                    schedaRicerca = (SearchManager)obj;

                //Pannello associazione griglie custom
                this.showGridPersonalization = RoleManager.GetRoleInSession().funzioni.Where(g => g.codice.Equals("GRID_PERSONALIZATION")).Count() > 0;

                if (!this.IsPostBack)
                {
                    this.InitializePage();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }

            this.RefreshScript();
        }

        protected void rbl_share_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.PlcGrids.Visible)
            {
                string idGrigliaOld = this.ddl_ric_griglie.SelectedValue;
                this.ddl_ric_griglie.Items.Clear();
                string visibility = rbl_share.SelectedValue;
                InfoUtente infoUtente = UserManager.GetInfoUser();

                bool allGrids = true;

                //Vuol dire c'è una griglia temporanea
                if (GridManager.SelectedGrid != null && string.IsNullOrEmpty(GridManager.SelectedGrid.GridId))
                {
                    ListItem it = new ListItem("Griglia temporanea", "-2");
                    this.ddl_ric_griglie.Items.Add(it);
                }

                if (visibility.Equals("grp"))
                {
                    allGrids = false;
                }

                GridBaseInfo[] listGrid = GridManager.GetGridsBaseInfo(infoUtente, GridManager.SelectedGrid.GridType, allGrids);

                bool present = false;
                if (listGrid != null && listGrid.Length > 0)
                {
                    foreach (GridBaseInfo gb in listGrid)
                    {
                        ListItem it = new ListItem(gb.GridName, gb.GridId);
                        this.ddl_ric_griglie.Items.Add(it);
                        if (gb.GridId.Equals(idGrigliaOld))
                        {
                            present = true;
                        }
                    }
                    if (present)
                    {
                        this.ddl_ric_griglie.SelectedValue = idGrigliaOld;
                    }

                }
            }

            this.UpPnlMakeAvailable.Update();
            this.UpPnlGrids.Update();
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            this.CloseMask(false);
        }

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

            if (string.IsNullOrEmpty(this.txtTitle.Text))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSaveSearchTitle', 'warning', '', null, null, null, 'reallowOp();');", true);
            }
            else
            {
                if (schedaRicerca != null && schedaRicerca.ProprietaNuovaRicerca != null)
                {
                    schedaRicerca.ProprietaNuovaRicerca.Titolo = this.txtTitle.Text;
                    if (rbl_share.Items[0].Selected)
                        schedaRicerca.ProprietaNuovaRicerca.Condivisione = SearchManager.NuovaRicerca.ModoCondivisione.Utente;
                    else if (rbl_share.Items[1].Selected)
                        schedaRicerca.ProprietaNuovaRicerca.Condivisione = SearchManager.NuovaRicerca.ModoCondivisione.Ruolo;

                    try
                    {
                        string msg = string.Empty;
                        string custom = string.Empty;
                        string pagina = string.Empty;
                        string _searchKey = schedaRicerca.getSearchKey();
                        System.Web.UI.Page currentPg = schedaRicerca.Pagina;
                        string adl = this.IsAdl ? "1" : "0";

                        if (string.IsNullOrEmpty(_searchKey))
                            pagina = currentPg.ToString();
                        else
                            pagina = _searchKey;

                        //Verifico se salvare anche la griglia
                        string idGriglia = string.Empty;
                        Grid tempGrid = new Grid();
                        if (ddl_ric_griglie.Visible)
                        {
                            idGriglia = ddl_ric_griglie.SelectedValue;
                            if (!string.IsNullOrEmpty(idGriglia) && idGriglia.Equals("-2"))
                            {
                                tempGrid = GridManager.CloneGrid(GridManager.SelectedGrid);
                            }
                        }

                        if (IsModify)
                        {
                            schedaRicerca.ProprietaNuovaRicerca.Id = Int32.Parse(this.idRicercaSalvata);
                            if (!schedaRicerca.verificaNomeModifica(this.txtTitle.Text, infoUtente, pagina, this.idRicercaSalvata))
                            {
                                schedaRicerca.Modifica(null, this.showGridPersonalization, idGriglia, tempGrid, GridManager.SelectedGrid.GridType);
                                msg = "InfoSaveSearchTitleModify";
                                custom = utils.FormatJs(this.txtTitle.Text);
                                DropDownList ddl = (DropDownList)schedaRicerca.Pagina.FindControl("ddlSavedSearches");
                                this.idRicercaSalvata = schedaRicerca.ProprietaNuovaRicerca.Id.ToString();
                                schedaRicerca.ProprietaNuovaRicerca = null;
                            }
                            else
                            {
                                msg = "ErrorSaveSearchTitleExists";
                            }
                        }
                        else
                        {
                            this.schedaRicerca.Tipo = this.tipoRicercaSalvata;
                            if (!schedaRicerca.verificaNome(this.txtTitle.Text, infoUtente, pagina, adl))
                            {
                                if (this.IsAdl)
                                {
                                    schedaRicerca.Salva("ricADL", this.showGridPersonalization, idGriglia, tempGrid, GridManager.SelectedGrid.GridType);
                                }
                                else
                                {
                                    schedaRicerca.Salva(null, this.showGridPersonalization, idGriglia, tempGrid, GridManager.SelectedGrid.GridType);
                                }

                                msg = "InfoSaveSearchTitleSave";
                                custom = utils.FormatJs(this.txtTitle.Text);
                                DropDownList ddl = (DropDownList)schedaRicerca.Pagina.FindControl("ddlSavedSearches");
                                this.idRicercaSalvata = schedaRicerca.ProprietaNuovaRicerca.Id.ToString();
                                schedaRicerca.ProprietaNuovaRicerca = null;
                            }
                            else
                            {
                                msg = "ErrorSaveSearchTitleExists";
                            }
                        }

                        if (string.IsNullOrEmpty(msg))
                            this.CloseMask(true);
                        else
                        {
                            if (string.IsNullOrEmpty(custom))
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msg + "', 'error', '', null, null, null, 'reallowOp();');", true);
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + msg + "', 'info', '', '" + utils.FormatJs(custom) + "', null, null, 'reallowOp();');", true);
                                this.CloseMask(true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSaveSearch', 'error', '', null, null, null, 'reallowOp();');", true);
                    }
                }
            }
        }

        #endregion

        #region methods

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
        }

        private void InitializePage()
        {
            this.InitializeLanguage();
            this.PopulateFields();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            if (this.IsModify)
                this.BtnSave.Text = Utils.Languages.GetLabelFromCode("GenericBtnModify", language);
            else
                this.BtnSave.Text = Utils.Languages.GetLabelFromCode("GenericBtnSave", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.litTitle.Text = Utils.Languages.GetLabelFromCode("ModifySearchTitle", language);
            this.litMakeAvailable.Text = Utils.Languages.GetLabelFromCode("ModifySearchMakeAvailable", language);
            this.optUsr.Text = Utils.Languages.GetLabelFromCode("ModifySearchOptUsr", language).Replace("@usr@", UserManager.GetUserInSession().descrizione);
            this.optGrp.Text = Utils.Languages.GetLabelFromCode("ModifySearchOptGrp", language).Replace("@grp@", RoleManager.GetRoleInSession().descrizione);
            this.litGrids.Text = Utils.Languages.GetLabelFromCode("ModifySearchGrids", language);
        }

        private void PopulateFields()
        {
            InfoUtente infoUtente = UserManager.GetInfoUser();

            if (this.IsAdl)
                rbl_share.Items[1].Enabled = false;

            if (this.IsModify)
            {
                // modify
                DocsPaWR.SearchItem itemOld = SearchManager.GetItemSearch(Int32.Parse(idRicercaSalvata));
                this.txtTitle.Text = itemOld.descrizione;
                this.schedaRicerca.Tipo = itemOld.tipo;
                if (!string.IsNullOrEmpty(itemOld.owner_idPeople.ToString()))
                {
                    this.rbl_share.SelectedValue = "usr";
                }
                else
                {
                    this.rbl_share.SelectedValue = "grp";
                }

                if (schedaRicerca != null && schedaRicerca.ProprietaNuovaRicerca != null)
                {
                    if (this.schedaRicerca.ProprietaNuovaRicerca.Condivisione == SearchManager.NuovaRicerca.ModoCondivisione.Utente)
                    {
                        this.rbl_share.Items[0].Selected = true;
                        this.rbl_share.Items[1].Selected = false;
                    }
                    else
                    {
                        this.rbl_share.Items[0].Selected = false;
                        this.rbl_share.Items[1].Selected = true;
                    }

                    this.UpPnlMakeAvailable.Update();


                    this.PlcGrids.Visible = this.showGridPersonalization;
                    if (!IsPostBack && this.showGridPersonalization)
                    {
                        this.ddl_ric_griglie.Items.Clear();

                        // c'è una griglia temporanea
                        if (GridManager.SelectedGrid != null && string.IsNullOrEmpty(GridManager.SelectedGrid.GridId))
                        {
                            ListItem it = new ListItem("Griglia temporanea", "-2");
                            this.ddl_ric_griglie.Items.Add(it);
                        }

                        string visibility = rbl_share.SelectedValue;
                        bool allGrids = true;

                        if (visibility.Equals("grp"))
                        {
                            allGrids = false;
                        }

                        GridBaseInfo[] listGrid = GridManager.GetGridsBaseInfo(infoUtente, GridManager.SelectedGrid.GridType, allGrids);
                        Dictionary<string, GridBaseInfo> tempIdGrid = new Dictionary<string, GridBaseInfo>();
                        if (listGrid != null && listGrid.Length > 0)
                        {
                            foreach (GridBaseInfo gb in listGrid)
                            {
                                ListItem it = new ListItem(gb.GridName, gb.GridId);
                                this.ddl_ric_griglie.Items.Add(it);
                                tempIdGrid.Add(gb.GridId, gb);
                            }
                            if (!string.IsNullOrEmpty(schedaRicerca.gridId) && tempIdGrid != null && tempIdGrid.Count > 0)
                            {
                                if (tempIdGrid.ContainsKey(schedaRicerca.gridId))
                                {
                                    this.ddl_ric_griglie.SelectedValue = schedaRicerca.gridId;
                                }
                            }
                        }
                    }
                    this.UpPnlGrids.Update();
                }
            }
            else
            {
                // save
                this.rbl_share.SelectedValue = "usr";

                //Pannello associazione griglie custom
                this.PlcGrids.Visible = this.showGridPersonalization;
                if (!IsPostBack && this.showGridPersonalization)
                {
                    string visibility = this.rbl_share.SelectedValue;

                    bool allGrids = true;

                    //Vuol dire c'è una griglia temporanea
                    if (GridManager.SelectedGrid != null && string.IsNullOrEmpty(GridManager.SelectedGrid.GridId))
                    {
                        ListItem it = new ListItem("Griglia temporanea", "-2");
                        this.ddl_ric_griglie.Items.Add(it);
                    }

                    if (visibility.Equals("grp"))
                    {
                        allGrids = false;
                    }

                    GridBaseInfo[] listGrid = GridManager.GetGridsBaseInfo(infoUtente, GridManager.SelectedGrid.GridType, allGrids);

                    if (listGrid != null && listGrid.Length > 0)
                    {
                        foreach (GridBaseInfo gb in listGrid)
                        {
                            ListItem it = new ListItem(gb.GridName, gb.GridId);
                            this.ddl_ric_griglie.Items.Add(it);
                        }
                    }
                }
            }
        }

        protected void CloseMask(bool withReturnValue)
        {
            string retValue = "";
            if (withReturnValue)
                retValue = "true";

            if (this.IsModify)
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('ModifySearch', '" + retValue + "');", true);
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('SaveSearch', '" + retValue + "');", true);
        }

        #endregion

    }
}