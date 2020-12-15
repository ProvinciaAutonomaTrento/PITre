using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;

namespace NttDataWA.Mandate
{
    public partial class Mandate : System.Web.UI.Page
    {

        #region Properties

        private Dictionary<StatoModelloDelega, string> StatiModelloDelega
        {
            get
            {
                Dictionary<StatoModelloDelega, string> res = new Dictionary<StatoModelloDelega, string>();
                res.Add(StatoModelloDelega.NON_VALIDO, this.GetLabel("MandateNotValid"));
                res.Add(StatoModelloDelega.SCADUTO, this.GetLabel("MandateExpired"));
                res.Add(StatoModelloDelega.VALIDO, this.GetLabel("MandateValid"));
                return res;
            }
        }

        private bool SHOW_MODELLI_DELEGA
        {
            get
            {
                bool retValue = false;
                if (Session["Mandate.SHOW_MODELLI_DELEGA"] != null) retValue = bool.Parse(Session["Mandate.SHOW_MODELLI_DELEGA"].ToString());
                return retValue;
            }
            set
            {
                Session["Mandate.SHOW_MODELLI_DELEGA"] = value;
            }
        }

        private int NUM_OPZIONI_RB
        {
            get
            {
                if (this.SHOW_MODELLI_DELEGA)
                    return 3;
                else
                    return 2;
            }
        }

        protected DocsPaWR.InfoDelega[] ListaDeleghe
        {
            get
            {
                if (Session["ListaDeleghe"] != null)
                    return (DocsPaWR.InfoDelega[])Session["ListaDeleghe"];
                else
                    return new DocsPaWR.InfoDelega[0];
            }
            set
            {
                Session["ListaDeleghe"] = value;
            }
        }

        protected DocsPaWR.ModelloDelega[] ListaModelliDelega
        {
            get
            {
                if (Session["ListaModelliDelega"] != null)
                    return (DocsPaWR.ModelloDelega[])Session["ListaModelliDelega"];
                else
                    return new DocsPaWR.ModelloDelega[0];
            }
            set
            {
                Session["ListaModelliDelega"] = value;
            }
        }

        protected SearchDelegaInfo SearchDelegheMemento
        {
            get
            {
                if (Session["SearchDelegheMemento"] != null)
                    return (SearchDelegaInfo)Session["SearchDelegheMemento"];
                else
                    return null;
            }
            set
            {
                Session["SearchDelegheMemento"] = value;
            }
        }

        protected SearchModelloDelegaInfo SearchModelliDelegaMemento
        {
            get
            {
                if (Session["SearchModelliDelegaMemento"] != null)
                    return (SearchModelloDelegaInfo)Session["SearchModelliDelegaMemento"];
                else
                    return null;
            }
            set
            {
                Session["SearchModelliDelegaMemento"] = value;
            }
        }

        private string TipoDelega
        {
            get
            {
                return Session["Mandate.TipoDelega"] as string;
            }
            set
            {
                Session["Mandate.TipoDelega"] = value;
            }
        }

        private int SelectedPage
        {
            get
            {
                int toReturn = 1;
                if (HttpContext.Current.Session["SelectedPage"] != null) Int32.TryParse(HttpContext.Current.Session["SelectedPage"].ToString(), out toReturn);
                if (toReturn < 1) toReturn = 1;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["SelectedPage"] = value;
            }
        }

        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    this.InitializePage();
                }
                else
                {
                    this.ReadRetValueFromPopup();
                }
                this.RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.proceed_delete.Value))
            {
                //Crea la lista delle deleghe da revocare, dopo averle revocate si ricarica
                //l'elenco delle deleghe assegnate
                ArrayList listaDeleghe = new ArrayList();
                for (int i = 0; i < this.gridViewResult.Rows.Count; i++)
                {
                    CheckBox chkSelection = this.gridViewResult.Rows[i].Cells[0].FindControl("cbSel") as CheckBox;
                    if (chkSelection != null && chkSelection.Checked)
                    {
                        DocsPaWR.InfoDelega delega = (DocsPaWR.InfoDelega)this.ListaDeleghe[i];
                        listaDeleghe.Add(delega);
                    }
                }
                string msg = string.Empty;
                if (DelegheManager.RevocaDelega(this, (DocsPaWR.InfoDelega[])listaDeleghe.ToArray(typeof(DocsPaWR.InfoDelega)), out msg))
                {
                    this.fillDeleghe("assegnate", this.MandateDdlState.SelectedValue);
                }
                else
                {
                    string messaggio = string.Empty;
                    if (string.IsNullOrEmpty(msg))
                        messaggio = "ErrorMandateBtnDelete";
                    else
                        messaggio = "ErrorCustom";
                    this.disabilitaCheckDataGrid();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + messaggio + "', 'error', '', '" + utils.FormatJs(msg) + "');", true);
                    return;
                }

                this.proceed_delete.Value = string.Empty;
                this.UpPnlButtons.Update();
            }

            if (!string.IsNullOrEmpty(this.NewMandate.ReturnValue))
            {
                this.fillDeleghe("assegnate", this.MandateDdlState.SelectedValue);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('NewMandate','');", true);
            }

            if (!string.IsNullOrEmpty(this.EditMandate.ReturnValue))
            {
                this.fillDeleghe("assegnate", this.MandateDdlState.SelectedValue);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('EditMandate','');", true);
            }
        }

        private void InitializePage()
        {
            this.ClearSessionProperties();
            this.InitializeLanguage();
            this.LoadKeys();
            this.fillComboRuoli(this.MandateDdlRole);
            this.MandateBtnSearch_Click(null, null);
        }

        private void ClearSessionProperties()
        {
            this.ListaDeleghe = null;
            this.ListaModelliDelega = null;
            this.SearchDelegheMemento = null;
            this.SearchModelliDelegaMemento = null;
            this.TipoDelega = "ricevute";
            this.SelectedPage = 1;
        }

        private void LoadKeys()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_MODELLI_DELEGA.ToString())) && int.Parse(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_MODELLI_DELEGA.ToString()))==1)
            {
                this.SHOW_MODELLI_DELEGA = true;
            }
        }

        protected void MandateLinkReceived_OnClick(object sender, EventArgs e)
        {
            try
            {
                this.MandateLiReceived.Attributes.Remove("class");
                this.MandateLiReceived.Attributes.Add("class", "prjIAmProfile");
                this.MandateLiAssigned.Attributes.Remove("class");
                this.MandateLiAssigned.Attributes.Add("class", "prjOther");
                this.MandateBtnExecute.Visible = true;
                this.MandateBtnNew.Visible = false;
                this.MandateBtnDelete.Visible = false;
                this.MandateBtnEdit.Visible = false;
                this.MandateLitName.Text = this.GetLabel("MandateLitName");
                this.PlcRoleSelect.Visible = false;
                this.TipoDelega = "ricevute";
                this.UpPnlRoleSelect.Update();
                this.UpPnlButtons.Update();
                this.UpnlTabHeader.Update();
                this.UpNameDelegate.Update();

                this.SelectedPage = 1;
                this.grid_pageindex.Value = "1";
                this.MandateBtnSearch_Click(null, null);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void MandateBtnNew_Click(object sender, EventArgs e)
        {
            Session["NewEditMandate_sel"] = null;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "NewEditMandate", "ajaxModalPopupNewMandate();", true);
        }

        protected void MandateBtnEdit_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "NewEditMandate", "ajaxModalPopupEditMandate();", true);
        }

        protected void MandateLinkAssigned_OnClick(object sender, EventArgs e)
        {
            try
            {
                this.MandateLiReceived.Attributes.Remove("class");
                this.MandateLiReceived.Attributes.Add("class", "prjOther");
                this.MandateLiAssigned.Attributes.Remove("class");
                this.MandateLiAssigned.Attributes.Add("class", "prjIAmProfile");
                this.MandateBtnExecute.Visible = false;
                this.MandateBtnNew.Visible = true;
                this.MandateBtnDelete.Visible = true;
                this.MandateBtnEdit.Visible = true;
                this.MandateLitName.Text = this.GetLabel("MandateLitName2");
                this.PlcRoleSelect.Visible = true;
                this.TipoDelega = "assegnate";
                this.UpPnlRoleSelect.Update();
                this.UpPnlButtons.Update();
                this.UpnlTabHeader.Update();
                this.UpNameDelegate.Update();

                this.SelectedPage = 1;
                this.grid_pageindex.Value = "1";
                this.MandateBtnSearch_Click(null, null);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private string GetLabel(string id)
        {
            return Utils.Languages.GetLabelFromCode(id, UIManager.UserManager.GetUserLanguage());
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.MandateLitMandate.Text = Utils.Languages.GetLabelFromCode("MandateLitMandate", language);
            this.MandateBtnExecute.Text = Utils.Languages.GetLabelFromCode("MandateBtnExecute", language);
            this.MandateBtnNew.Text = Utils.Languages.GetLabelFromCode("MandateBtnNew", language);
            this.MandateBtnDelete.Text = Utils.Languages.GetLabelFromCode("MandateBtnDelete", language);
            this.MandateBtnEdit.Text = Utils.Languages.GetLabelFromCode("MandateBtnEdit", language);
            this.MandateLinkReceived.Text = Utils.Languages.GetLabelFromCode("MandateLinkReceived", language);
            this.MandateLinkAssigned.Text = Utils.Languages.GetLabelFromCode("MandateLinkAssigned", language);
            this.MandateBtnSearch.Text = Utils.Languages.GetLabelFromCode("MandateBtnSearch", language);
            this.MandateLitState.Text = Utils.Languages.GetLabelFromCode("MandateLitState", language);
            this.MandateLitName.Text = Utils.Languages.GetLabelFromCode("MandateLitName", language);
            this.MandateLitRole.Text = Utils.Languages.GetLabelFromCode("MandateLitRole", language);
            this.NewMandate.Title = Utils.Languages.GetLabelFromCode("NewMandateTitle", language);
            this.EditMandate.Title = Utils.Languages.GetLabelFromCode("EditMandateTitle", language);
            this.optActive.Text = Utils.Languages.GetLabelFromCode("MandateOptActive", language);
            this.optSet.Text = Utils.Languages.GetLabelFromCode("MandateOptSet", language);
            this.optExpire.Text = Utils.Languages.GetLabelFromCode("MandateOptExpire", language);
            this.optAll.Text = Utils.Languages.GetLabelFromCode("MandateOptAll", language);
            this.gridViewResult.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("MandateGridSel", language);
            this.gridViewResult.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("MandateGridDecorrenza", language);
            this.gridViewResult.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("MandateGridScadenza", language);
            this.gridViewResult.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("MandateGridUtente", language);
            this.gridViewResult.Columns[4].HeaderText = Utils.Languages.GetLabelFromCode("MandateGridDelegato", language);
            this.gridViewResult.Columns[5].HeaderText = Utils.Languages.GetLabelFromCode("MandateGridRuolo", language);
            this.gridViewResult.Columns[6].HeaderText = Utils.Languages.GetLabelFromCode("MandateGridRuolo", language);
            this.gridViewResult.Columns[7].HeaderText = Utils.Languages.GetLabelFromCode("MandateGridUtente", language);
            this.gridViewResult.Columns[8].HeaderText = Utils.Languages.GetLabelFromCode("MandateGridDelegante", language);
            this.gridViewResult.Columns[9].HeaderText = Utils.Languages.GetLabelFromCode("MandateGridRuolo", language);
            this.gridViewResult.Columns[10].HeaderText = Utils.Languages.GetLabelFromCode("MandateGridRuoloDelegante", language);
            this.gridViewResult.Columns[15].HeaderText = Utils.Languages.GetLabelFromCode("MandateGridStato", language);
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void fillDeleghe(string tipoDelega, string statoDelega)
        {
            SearchDelegaInfo searchInfo = new SearchDelegaInfo();
            searchInfo.StatoDelega = statoDelega;
            searchInfo.TipoDelega = tipoDelega;
            this.searchDeleghe(searchInfo);
        }

        private void searchDeleghe(SearchDelegaInfo searchInfo)
        {
            this.SearchDelegheMemento = searchInfo;
            SearchPagingContext pagingContext = new SearchPagingContext();
            pagingContext.PageSize = this.gridViewResult.PageSize;
            pagingContext.Page = this.SelectedPage;
            this.ListaDeleghe = DelegheManager.SearchDeleghe(this, searchInfo, ref pagingContext);
            this.BindGridAndSelect(searchInfo.TipoDelega, searchInfo.StatoDelega);
            this.buildGridNavigator(pagingContext);
        }

        protected void BindGridAndSelect(string tipoDelega, string stato)
        {
            string statoDelega;
            string language = UIManager.UserManager.GetUserLanguage();

            DocsPaWR.InfoDelega[] data = this.ListaDeleghe;
            this.gridViewResult.DataSource = data;
            this.gridViewResult.DataBind();
            if (data.Length > 0)
            {
                this.gridViewResult.Visible = true;

                this.lbl_messaggio.Visible = false;
                //DELEGHE ASSEGNATE
                if (tipoDelega.Equals("assegnate"))
                {
                    this.gridViewResult.Columns[8].HeaderStyle.CssClass = "hidden";
                    this.gridViewResult.Columns[6].HeaderStyle.CssClass = "hidden";
                    this.gridViewResult.Columns[4].HeaderStyle.CssClass = "";
                    this.gridViewResult.Columns[10].HeaderStyle.CssClass = "";
                    this.gridViewResult.Columns[8].ItemStyle.CssClass = "hidden";
                    this.gridViewResult.Columns[6].ItemStyle.CssClass = "hidden";
                    this.gridViewResult.Columns[4].ItemStyle.CssClass = "";
                    this.gridViewResult.Columns[10].ItemStyle.CssClass = "";
                }
                //DELEGHE RICEVUTE
                if (tipoDelega.Equals("ricevute"))
                {
                    this.gridViewResult.Columns[4].HeaderStyle.CssClass = "hidden";
                    this.gridViewResult.Columns[6].HeaderStyle.CssClass = "hidden";
                    this.gridViewResult.Columns[8].HeaderStyle.CssClass = "";
                    this.gridViewResult.Columns[10].HeaderStyle.CssClass = "";
                    this.gridViewResult.Columns[4].ItemStyle.CssClass = "hidden";
                    this.gridViewResult.Columns[6].ItemStyle.CssClass = "hidden";
                    this.gridViewResult.Columns[8].ItemStyle.CssClass = "";
                    this.gridViewResult.Columns[10].ItemStyle.CssClass = "";
                }

                #region grafica elementi datagrid
                for (int i = 0; i < data.Length; i++)
                {
                    CheckBox chkBox = this.gridViewResult.Rows[i].Cells[0].FindControl("cbSel") as CheckBox;
                    chkBox.Visible = false;
                    RadioButton rdButton = this.gridViewResult.Rows[i].Cells[0].FindControl("rbSel") as RadioButton;
                    rdButton.Visible = false;
                    if (tipoDelega.Equals("assegnate"))
                        chkBox.Visible = true;
                    else
                        rdButton.Visible = true;
                    this.gridViewResult.Rows[i].Cells[4].Text = data[i].cod_utente_delegato + "<br />(" + data[i].cod_ruolo_delegato + ")";

                    DateTime dataDecorrenza = Utils.dateformat.ConvertToDate(data[i].dataDecorrenza);
                    DateTime dataScadenza = Utils.dateformat.ConvertToDate(data[i].dataScadenza);
                    
                    //data decorrenza
                    this.gridViewResult.Rows[i].Cells[1].Text = dataDecorrenza.ToShortDateString()
                                                                + "<br />" + dataDecorrenza.ToShortTimeString();

                    //data scadenza 
                    if (dataScadenza!=new DateTime())
                    {
                        if (dataScadenza > DateTime.Now)
                            this.gridViewResult.Rows[i].Font.Bold = true;

                        this.gridViewResult.Rows[i].Cells[2].Text = dataScadenza.ToShortDateString()
                                                                + "<br />" + dataScadenza.ToShortTimeString();
                    }
                    else
                        this.gridViewResult.Rows[i].Font.Bold = true;

                    if (data[i].utDelegatoDismesso.Equals("1"))
                    {
                        this.gridViewResult.Rows[i].Font.Strikeout = true;
                        this.gridViewResult.Rows[i].ToolTip = this.GetLabel("MandateUserDismiss");
                    }

                    statoDelega = Utils.Languages.GetLabelFromCode("MandateStateSetted", language);
                    if (dataDecorrenza < DateTime.Now && (dataScadenza==new DateTime() || dataScadenza > DateTime.Now))
                    {
                        statoDelega = Utils.Languages.GetLabelFromCode("MandateStateActive", language);
                    }
                    if (dataScadenza!=new DateTime() && dataScadenza < DateTime.Now)
                    {
                        statoDelega = Utils.Languages.GetLabelFromCode("MandateStateExpired", language);
                    }
                    this.gridViewResult.Rows[i].Cells[15].Text = statoDelega;
                }
                #endregion
                if (data.Length == 1)
                {
                    if (tipoDelega.Equals("assegnate"))
                    {
                        CheckBox chkBox2 = this.gridViewResult.Rows[0].Cells[0].FindControl("cbSel") as CheckBox;
                        chkBox2.Checked = true;
                        this.MandateBtnEdit.Enabled = true;
                        this.MandateBtnDelete.Enabled = true;
                        Session["NewEditMandate_sel"] = ListaDeleghe[0];
                    }
                    if (tipoDelega.Equals("ricevute"))
                    {
                        if (!stato.Equals("A"))
                        {
                            this.gridViewResult.Columns[0].HeaderStyle.CssClass = "hidden";
                            this.gridViewResult.Columns[0].ItemStyle.CssClass = "hidden";
                            this.MandateBtnExecute.Enabled = false;
                        }
                        else
                        {
                            RadioButton rdb = this.gridViewResult.Rows[0].Cells[0].FindControl("rbSel") as RadioButton;
                            rdb.Checked = true;
                            this.gridViewResult.Columns[0].HeaderStyle.CssClass = "";
                            this.gridViewResult.Columns[0].ItemStyle.CssClass = "";
                            this.MandateBtnExecute.Enabled = true;
                        }

                    }
                    if (tipoDelega.Equals("esercizio"))
                    {
                        this.gridViewResult.Columns[4].HeaderStyle.CssClass = "hidden";
                        this.gridViewResult.Columns[6].HeaderStyle.CssClass = "hidden";
                        this.gridViewResult.Columns[8].HeaderStyle.CssClass = "";
                        this.gridViewResult.Columns[10].HeaderStyle.CssClass = "";
                        this.gridViewResult.Columns[4].ItemStyle.CssClass = "hidden";
                        this.gridViewResult.Columns[6].ItemStyle.CssClass = "hidden";
                        this.gridViewResult.Columns[8].ItemStyle.CssClass = "";
                        this.gridViewResult.Columns[10].ItemStyle.CssClass = "";

                        this.MandateBtnExecute.Text = this.GetLabel("MandateBtnDismiss");
                        this.MandateBtnExecute.Enabled = true;
                        RadioButton rb = this.gridViewResult.Rows[0].Cells[0].FindControl("rbSel") as RadioButton;
                        rb.Checked = true;

                        this.MandateBtnNew.Visible = false;
                        this.MandateBtnEdit.Visible = false;
                        this.MandateBtnDelete.Visible = false;
                    }
                }
                else
                {
                    this.MandateBtnEdit.Enabled = false;
                    this.MandateBtnDelete.Enabled = false;
                    this.MandateBtnExecute.Enabled = false;
                }
            }
            else
            {
                this.gridViewResult.Visible = false;
                this.lbl_messaggio.Visible = true;
                if (tipoDelega == "ricevute")
                    this.lbl_messaggio.Text = this.GetLabel("MandateListNoReceived");
                if (tipoDelega == "assegnate")
                    this.lbl_messaggio.Text = this.GetLabel("MandateListNoAssigned");
                if (tipoDelega == "esercizio")
                    this.lbl_messaggio.Text = this.GetLabel("MandateListActive");
                this.MandateBtnExecute.Enabled = false;
                this.MandateBtnEdit.Enabled = false;
                this.MandateBtnDelete.Enabled = false;

            }
            this.MandateBtnNew.Enabled = true;
            this.UpPnlMessage.Update();
            this.UpnlGrid.Update();
            this.UpPnlButtons.Update();
        }

        protected void MandateBtnSearch_Click(object sender, EventArgs e)
        {
            SearchDelegaInfo searchInfo = new SearchDelegaInfo();
            InfoUtente infoUser = UserManager.GetInfoUser();
            if (infoUser.delegato == null)
            {
                searchInfo.TipoDelega = this.TipoDelega;
                searchInfo.StatoDelega = this.MandateDdlState.SelectedValue;
                if ("assegnate".Equals(searchInfo.TipoDelega))
                {
                    searchInfo.NomeDelegato = this.TxtName.Text;
                    searchInfo.IdRuoloDelegante = this.MandateDdlRole.SelectedValue;
                }
                else
                {
                    searchInfo.NomeDelegante = this.TxtName.Text;
                }
            }
            else
            {
                this.TipoDelega = "esercizio";

                this.MandateDdlState.Enabled = false;
                this.UpPnlState.Update();

                this.TxtName.Enabled = false;
                this.UpNameDelegate.Update();

                this.MandateDdlRole.Enabled = false;
                this.UpPnlRoleSelect.Update();

                this.MandateLinkAssigned.Visible = false;
                this.UpnlTabHeader.Update();

                searchInfo.TipoDelega = this.TipoDelega;
                searchInfo.StatoDelega = "";
            }
            this.searchDeleghe(searchInfo);
            this.visualizza_pulsanti();
        }

        private void fillComboRuoli(DropDownList ddl)
        {
            ddl.Items.Clear();
            DocsPaWR.Utente userHome = UserManager.GetUserInSession();
            if (userHome != null)
            {
                if (userHome.ruoli != null)
                {
                    ListItem item = new ListItem(this.GetLabel("MandateOptAllM"), "");
                    ddl.Items.Add(item);
                    for (int i = 0; i < userHome.ruoli.Length; i++)
                    {
                        ListItem temp = new ListItem(((DocsPaWR.Ruolo)userHome.ruoli[i]).descrizione.ToString(), ((DocsPaWR.Ruolo)userHome.ruoli[i]).systemId.ToString());
                        ddl.Items.Add(temp);
                    }
                }
            }
        }

        private void visualizza_pulsanti()
        {
            //DELEGHE ASSEGNATE
            if (this.TipoDelega=="assegnate")
            {
                this.MandateBtnNew.Visible = true;
                this.MandateBtnNew.Enabled = true;
                this.MandateBtnDelete.Visible = true;
                this.MandateBtnEdit.Visible = true;
                this.MandateBtnExecute.Visible = false;
                if (this.gridViewResult.Rows.Count == 1)
                {
                    CheckBox chkBox = this.gridViewResult.Rows[0].Cells[0].FindControl("cbSel") as CheckBox;
                    chkBox.Enabled = true;
                    chkBox.Checked = true;
                    this.MandateBtnDelete.Enabled = true;
                    this.MandateBtnEdit.Enabled = true;
                }
                else
                {
                    this.MandateBtnDelete.Enabled = false;
                    this.MandateBtnEdit.Enabled = false;
                }
            }
            //DELEGHE RICEVUTE
            else
            {
                this.MandateBtnExecute.Visible = true;
                this.MandateBtnNew.Visible = false;
                this.MandateBtnDelete.Visible = false;
                this.MandateBtnEdit.Visible = false;
                //Deleghe ricevute scadute, impostate, tutte (non attive)
                if (!this.MandateDdlState.SelectedValue.Equals("A"))
                {
                    this.gridViewResult.Columns[0].HeaderStyle.CssClass = "hidden"; 
                    this.gridViewResult.Columns[0].ItemStyle.CssClass = "hidden";
                    this.MandateBtnExecute.Enabled = false;
                }
                else
                {
                    if (this.gridViewResult.Rows.Count == 1)
                    {
                        RadioButton rdb = this.gridViewResult.Rows[0].Cells[0].FindControl("rbSel") as RadioButton;
                        rdb.Enabled = true;
                        rdb.Checked = true;
                        this.MandateBtnExecute.Enabled = true;
                    }
                    else
                        this.MandateBtnExecute.Enabled = false;
                }

            }
        }

        protected void cbSel_CheckedChanged(object sender, EventArgs e)
        {
            //pnl_nuovaDelega.Visible = false;
            //Se è stata selezionata almeno una delega non ancora scaduta sono attivi 
            //i pulsanti esercita e revoca, il pulsante modifica è invece attivo solo se si seleziona 
            //una e una sola delega attiva
            this.MandateBtnEdit.Enabled = false;
            this.MandateBtnDelete.Enabled = false;
            int elemSelModificabili = 0;
            int elemSelezionati = 0;
            int index = -1;
            for (int i = 0; i < this.gridViewResult.Rows.Count; i++)
            {
                GridViewRow item = this.gridViewResult.Rows[i];
                CheckBox chkSelection = item.Cells[0].FindControl("cbSel") as CheckBox;
                if (chkSelection != null && chkSelection.Checked)
                {
                    index = i;
                    elemSelezionati++;
                    if (string.IsNullOrEmpty(ListaDeleghe[i].dataScadenza))
                        elemSelModificabili++;
                    else
                    {
                        DateTime dataScadenza = Utils.dateformat.ConvertToDate(ListaDeleghe[i].dataScadenza);
                        if (dataScadenza.CompareTo(DateTime.Now) > 0)
                            elemSelModificabili++;
                    }
                }
                this.gridViewResult.Rows[i].Cells[15].Text = item.Cells[15].Text;
            }
            if (elemSelModificabili > 0)
            {
                this.MandateBtnDelete.Enabled = true;
            }
            if (elemSelezionati == 1)
            {
                Session["NewEditMandate_sel"] = ListaDeleghe[index];
                this.MandateBtnEdit.Enabled = true;
            }

            this.UpPnlButtons.Update();
        }

        protected void rbSel_CheckedChanged(object sender, EventArgs e)
        {
            //pnl_nuovaDelega.Visible = false;
            this.MandateBtnExecute.Enabled = true;
            this.UpPnlButtons.Update();
        }

        protected void MandateBtnExecute_Click(object sender, EventArgs e)
        {
            if (this.MandateBtnExecute.Text == this.GetLabel("MandateBtnExecute"))
            {
                this.EsercitaDelega();
            }
            if (this.MandateBtnExecute.Text == this.GetLabel("MandateBtnDismiss"))
            {
                this.DismettiDelega();
            }
        }

        private void EsercitaDelega()
        {
            GridViewRow item = null;

            //Ricerca delle delega selezionata che si vuole esercitare
            for (int i = 0; i < this.gridViewResult.Rows.Count; i++)
            {
                RadioButton rbSelection = this.gridViewResult.Rows[i].Cells[0].FindControl("rbSel") as RadioButton;
                if (rbSelection.Checked)
                    item = this.gridViewResult.Rows[i];
            }

            if (item != null)
            {
                DocsPaWR.InfoUtente infoUtDelegato = UserManager.GetInfoUser();
                //Imposto nuovo utente (delegante) per esercitare la delega
                DocsPaWR.UserLogin userLogin = new DocsPaWR.UserLogin();
                DocsPaWR.Utente utente = DelegheManager.getUtenteById(this, item.Cells[7].Text);
                userLogin.UserName = utente.userId;
                userLogin.Password = "";
                userLogin.IdAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                userLogin.IPAddress = this.Request.UserHostAddress;

                DocsPaWR.LoginResult loginResult;
                utente = DelegheManager.EsercitaDelega(this, userLogin, item.Cells[11].Text, item.Cells[9].Text, out loginResult);
                switch (loginResult)
                {
                    case DocsPaWR.LoginResult.OK:
                        if (utente != null)
                        {
                            utente.urlWA = utils.getHttpFullPath();
                            //Memorizzo le info sul delegato che serviranno nel momento in cui
                            //si dismette la delega
                            UserManager.setDelegato(infoUtDelegato);
                            DocsPaWR.Utente utenteDelegato = UserManager.GetUserInSession();
                            DocsPaWR.Ruolo ruoloDelegato = RoleManager.GetRoleInSession();
                            UserManager.setUtenteDelegato(this, utenteDelegato);
                            RoleManager.setRuoloDelegato(ruoloDelegato);

                            //Nuovo utente (delegante)
                            RoleManager.SetRoleInSession(utente.ruoli[0]);
                            UIManager.RegistryManager.SetRFListInSession(UIManager.UserManager.getListaRegistriWithRF(utente.ruoli[0].systemId, "1", ""));
                            UIManager.RegistryManager.SetRegAndRFListInSession(UIManager.UserManager.getListaRegistriWithRF(utente.ruoli[0].systemId, "", ""));
                            UserManager.SetUserInSession(utente);
                            Session["ESERCITADELEGA"] = true;

                            // disconnessione utente delegante se loggato
                            LoginManager.LogOut(utente.userId, utente.idAmministrazione, this.Session.SessionID);

                            //Rimuovo le notifiche del vecchio utente per far si che il centro notifiche venga aggiornato correttamente
                            HttpContext.Current.Session.Remove("ListAllNotify");
                            Response.Redirect("../Index.aspx");
                        }
                        break;
                    case DocsPaWR.LoginResult.USER_ALREADY_LOGGED_IN:
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningMandateUserAlreadyLoggedIn', 'warning', '');", true);
                        break;
                    case DocsPaWR.LoginResult.NO_RUOLI:
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningMandateNoRoles', 'warning', '');", true);
                        break;
                    default:
                        // Application Error
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorMandateGeneral', 'error', '');", true);
                        break;
                }
            }
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorMandateNoneSelected', 'error', '');", true);
        }

        private void DismettiDelega()
        {
            if (DelegheManager.DismettiDelega())
            {
                DocsPaWR.Utente utenteDelegato = UserManager.getUtenteDelegato();
                DocsPaWR.Ruolo ruoloDelegato = RoleManager.getRuoloDelegato();

                //Ripristino il vecchio infoUtente (del delegato)
                RoleManager.SetRoleInSession(utenteDelegato.ruoli[0]);
                UIManager.RegistryManager.SetRFListInSession(UIManager.UserManager.getListaRegistriWithRF(utenteDelegato.ruoli[0].systemId, "1", ""));
                UIManager.RegistryManager.SetRegAndRFListInSession(UIManager.UserManager.getListaRegistriWithRF(utenteDelegato.ruoli[0].systemId, "", ""));
                UserManager.SetUserInSession(utenteDelegato);
                Session.Remove("userDelegato");
                Session.Remove("userDataDelegato");
                Session.Remove("userRuoloDelegato");
                Session["ESERCITADELEGA"] = false;
                //Rimuovo le notifiche dell'utente delegante per far si che il centro notifiche venga aggiornato correttamente
                HttpContext.Current.Session.Remove("ListAllNotify");
                Response.Redirect("../Index.aspx");
            }
        }

        protected void MandateBtnDelete_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('ConfirmMandateBtnDelete', 'proceed_delete', '');", true);
        }

        private void disabilitaCheckDataGrid()
        {
            foreach (GridViewRow item in this.gridViewResult.Rows)
            {
                CheckBox chkSelection = item.Cells[0].FindControl("cbSel") as CheckBox;
                if (chkSelection != null && chkSelection.Checked)
                {
                    chkSelection.Checked = false;
                }
            }
        }

        protected void buildGridNavigator(DocsPaWR.SearchPagingContext pagingContext)
        {
            pagingContext.PageCount = (int)Math.Round(((double)pagingContext.RecordCount / (double)pagingContext.PageSize) + 0.49);
            
            this.plcNavigator.Controls.Clear();

            if (pagingContext.PageCount > 1)
            {
                Panel panel = new Panel();
                panel.CssClass = "recordNavigator2";

                for (int i = 1; i < pagingContext.PageCount + 1; i++)
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
                        btn.Text = i.ToString();
                        btn.Attributes["onclick"] = "$('#grid_pageindex').val($(this).text()); $('#btnChangePage').click(); return false;";
                        panel.Controls.Add(btn);
                    }
                }

                this.plcNavigator.Controls.Add(panel);
            }
        }

        protected void btnChangePage_Click(object sender, EventArgs e)
        {
            this.SelectedPage = int.Parse(this.grid_pageindex.Value);
            this.MandateBtnSearch_Click(null, null);
        }

    }
}