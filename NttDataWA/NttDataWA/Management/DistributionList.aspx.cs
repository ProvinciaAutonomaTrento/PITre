using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDatalLibrary;
using NttDataWA.Utils;

namespace NttDataWA.Management
{
    public partial class DistributionList : System.Web.UI.Page
    {

        protected DataSet dsListe;
        protected string idUtente;
        protected DataSet dsCorrispondenti;

        protected void Page_Load(object sender, EventArgs e)
        {
            idUtente = UserManager.GetInfoUser().idPeople;
            if (!this.IsPostBack)
            {
                this.InitializaPage();
                this.RowSelected = null;
                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_NEW_RUBRICA_VELOCE.ToString())) && (Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_NEW_RUBRICA_VELOCE.ToString())).Equals("1"))
                {
                    this.EnableAjaxAddressBook = true;
                    this.InitializeAddressBooks();
                }
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]))
                {
                    this.AjaxAddressBookMinPrefixLenght = int.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]);
                }
                if (ViewState["dsCorr"] != null && ((DataSet)ViewState["dsCorr"]).Tables.Count > 0)
                    ((DataSet)ViewState["dsCorr"]).Tables[0].Rows.Clear();
            }
            else
            {
                ReadRetValueFromPopup();
            }

            this.RefreshScript();
        }

        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }


        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.DistributionListLink.Text = Utils.Languages.GetLabelFromCode("DistributionListLink", language);
            this.DistributionListTitle.Text = Utils.Languages.GetLabelFromCode("DistributionListTitle", language);
            this.grdList.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("DistributionListDescr", language);
            this.grdList.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("DistributionListSelect", language);
            this.grdList.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("DistributionListDelete", language);
            this.gridViewResult.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("DistributionListDescr", language);
            this.gridViewResult.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("DistributionListDelete", language);
            this.rbOnlyMe.Text = Utils.Languages.GetLabelFromCode("DistributionListLitMakeAvailableOnlyMe", language);
            this.rbRole.Text = Utils.Languages.GetLabelFromCode("DistributionListLitMakeAvailableAllRole", language);
            this.rbOnlyMe.Checked = true;
            this.ListBtnNew.Text = Utils.Languages.GetLabelFromCode("DistributionListBtnNew", language);
            this.ListBtnSave.Text = Utils.Languages.GetLabelFromCode("DistributionListBtnSave", language);
            //this.ListBntAnnulla.Text = Utils.Languages.GetLabelFromCode("DistributionListBtnCancel", language);
            this.lblDisp.Text = Utils.Languages.GetLabelFromCode("DistributionListMakeAvailable", language);
            this.LitCodLista.Text = Utils.Languages.GetLabelFromCode("DistributionListCodList", language);
            this.LitDescrLista.Text = Utils.Languages.GetLabelFromCode("DistributionListNameList", language);
            this.LitCorr.Text = Utils.Languages.GetLabelFromCode("DistributionListLitCorr", language);
            //this.lblDett.Text = Utils.Languages.GetLabelFromCode("DistributionListDetail", language);
            this.DocumentImgSenderAddressBook.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgCollocationAddressBook", language);
            this.DocumentImgSenderAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgCollocationAddressBook", language);
            this.DocumentImgAddNewCorrispondent.AlternateText = Utils.Languages.GetLabelFromCode("AddressBookGetLabelAdd", language);
            this.DocumentImgAddNewCorrispondent.ToolTip = Utils.Languages.GetLabelFromCode("AddressBookGetLabelAdd", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
        }

        protected void InitializeAddressBooks()
        {
            if (this.EnableAjaxAddressBook)
            {
                this.SetAjaxAddressBook();
            }
        }

        protected void SetAjaxAddressBook()
        {
            //  this.RapidSender.MinimumPrefixLength = this.AjaxAddressBookMinPrefixLenght;

            string dataUser = UIManager.RoleManager.GetRoleInSession().systemId;
            dataUser = dataUser + "-" + UIManager.RoleManager.GetRoleInSession().registri[0].systemId;
            string callType = string.Empty;
            callType = "CALLTYPE_LISTE_DISTRIBUZIONE";

            this.RapidSender.ContextKey = dataUser + "-" + UIManager.UserManager.GetInfoUser().idAmministrazione + "-" + callType;
        }

        private void InitializaPage()
        {

            this.InitializeLanguage();

            this.ListBtnSave.Enabled = false;
            //this.ListBntAnnulla.Enabled = false;

            this.rbOnlyMe.Text = this.rbOnlyMe.Text.Replace("@usr@", UserManager.GetUserInSession().descrizione);
            this.rbRole.Text = this.rbRole.Text.Replace("@grp@", UserManager.GetSelectedRole().descrizione);
            Session.Remove("listaSelected");

            dsListe = UserManager.getListePerRuoloUt();
            grdList.DataSource = dsListe;
            grdList.DataBind();

            this.upPnlGridList.Update();

            //this.UpdPnDispVis.Visible = false;
            this.UpdPnDisp.Update();
            //this.panelDetail.Visible = false;
            this.UpdPanelDetail.Update();

            //this.panelResult.Visible = false;


            DataTable dt = new DataTable();
            dt.Columns.Add("ID_DPA_CORR");
            dt.Columns.Add("VAR_DESC_CORR");
            dt.Columns.Add("VAR_COD_RUBRICA");
            dt.Columns.Add("CHA_TIPO_IE");
            dt.Columns.Add("CHA_DISABLED_TRASM");
            dsCorrispondenti = new DataSet();
            dsCorrispondenti.Tables.Add(dt);
            ViewState.Add("dsCorr", dsCorrispondenti);

        }

        protected void gridViewResult_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Erase":

                    this.RowSelectedResult = (GridViewRow)(((CustomImageButton)e.CommandSource).NamingContainer);
                    if (Session["listaSelected"] != null && !string.IsNullOrEmpty(Session["listaSelected"].ToString()))
                    {
                        string idLista = Session["listaSelected"].ToString();

                        string msgConfirm = "ConfirmDistributionListDeleteCorr";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'deleteCorr');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'deleteCorr');}", true);
                    }
                    else
                    {
                        if (RowSelected != null)
                        {
                            ((DataSet)ViewState["dsCorr"]).Tables[0].Rows[(gridViewResult.PageSize * gridViewResult.PageIndex) + RowSelected.RowIndex].Delete();
                        }
                        else
                        {
                            GridViewRow rowRow = (GridViewRow)(((CustomImageButton)e.CommandSource).NamingContainer);
                            ((DataSet)ViewState["dsCorr"]).Tables[0].Rows[(gridViewResult.PageSize * gridViewResult.PageIndex) + rowRow.RowIndex].Delete();
                        }
                        gridViewResult.DataSource = ((DataSet)ViewState["dsCorr"]);
                        gridViewResult.DataBind();
                        this.UpnlGrid.Update();
                    }
                    break;

            }


        }

        protected void grdList_PreRender(object sender, EventArgs e)
        {
            try
            {
                bool alternateRow = false;

                if (grdList.Columns.Count > 0)
                {
                    for (int i = 0; i < grdList.Rows.Count; i++)
                    {

                        grdList.Rows[i].CssClass = "NormalRow";
                        if (alternateRow) grdList.Rows[i].CssClass = "AltRow";
                        alternateRow = !alternateRow;
                    }
                    if (this.RowSelected != null)
                    {
                        for (int i = 0; i < grdList.Rows.Count; i++)
                        {
                            if (this.grdList.Rows[i].RowIndex == this.RowSelected.RowIndex)
                            {
                                this.grdList.Rows[i].Attributes.Remove("class");
                                this.grdList.Rows[i].CssClass = "selectedrow";
                            }
                        }

                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void grdList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int selRow = 0;
            string idLista = "";

            switch (e.CommandName)
            {
                case "Select":

                    //Session.Remove("selCorrDaRubrica");

                    this.EnableDibledButtons(true);

                    this.RowSelected = (GridViewRow)(((CustomImageButton)e.CommandSource).NamingContainer);
                    idLista = ((HiddenField)RowSelected.Cells[1].FindControl("listId")).Value;
                    Session["listaSelected"] = idLista;

                    selRow = RowSelected.DataItemIndex;
                    this.grdList.SelectedRowStyle.BackColor = System.Drawing.Color.Yellow;

                    TxtCodList.Visible = true;
                    TxtCodList.Text = "";

                    string codList = UserManager.getCodiceLista(idLista);
                    TxtCodList.Text = codList;
                    TxtCodList.ReadOnly = true;

                    string nomeLista = UserManager.getNomeLista(this, codList, UserManager.GetInfoUser().idAmministrazione);
                    TxtDescList.Visible = true;
                    TxtDescList.Text = nomeLista;

                    ViewState.Add("dsCorr", UserManager.getCorrispondentiLista(idLista));
                    gridViewResult.DataSource = ((DataSet)ViewState["dsCorr"]);
                    gridViewResult.DataBind();

                    //this.panelResult.Visible = true;
                    this.gridViewResult.Visible = true;
                    this.UpnlGrid.Update();

                    this.ListBtnSave.Enabled = true;
                    //this.ListBtnNew.Enabled = true;
                    //this.ListBntAnnulla.Enabled = true;
                    this.UpDocumentButtons.Update();

                    if (UserManager.getRuoloOrUserLista(idLista) != null)
                    {
                        //this.rblDisp.SelectedValue = "grp";
                        this.rbRole.Checked = true;
                        this.rbOnlyMe.Checked = false;
                    }
                    else
                    {
                        //this.rblDisp.SelectedValue = "usr";
                        this.rbOnlyMe.Checked = true;
                        this.rbRole.Checked = false;
                    }

                    //this.panelDetail.Visible = true;
                    this.UpdPanelDetail.Update();
                    //this.UpdPnDispVis.Visible = true;
                    this.UpdPnDisp.Update();

                    this.UpPnlRadio.Update();
                    break;

                case "Erase":

                    //Session.Remove("selCorrDaRubrica");

                    this.RowSelected = (GridViewRow)(((CustomImageButton)e.CommandSource).NamingContainer);
                    idLista = ((HiddenField)RowSelected.Cells[1].FindControl("listId")).Value;

                    string msgConfirm = "ConfirmDistributionListDelete";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'deleteList');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'deleteList');}", true);

                    Session["ListId"] = idLista;

                    //this.ListBtnSave.Enabled = false;
                    //this.ListBntAnnulla.Enabled = false;
                    this.UpDocumentButtons.Update();

                    break;
            }
        }



        protected void ReadRetValueFromPopup()
        {


            if (this.deleteList.Value == "true")
            {
                string listId = Convert.ToString(Session["ListId"]).ToString();
                deleteSelectedList(listId);
                Session["ListId"] = string.Empty;
                this.deleteList.Value = string.Empty;
                this.TxtCodList.Text = "";
                this.TxtDescList.Text = "";
                this.TxtCodeSender.Text = "";
                this.TxtDescriptionSender.Text = "";
                this.UpdPanelDetail.Update();
                this.rbRole.Checked = false;
                this.rbOnlyMe.Checked = true;
                //this.UpdPnDispVis.Visible = false;
                this.UpdPnDisp.Update();

                //this.panelDetail.Visible = false;

                this.RowSelected = null;
                //this.panelResult.Visible = false;
                if (ViewState["dsCorr"] != null && ((DataSet)ViewState["dsCorr"]).Tables.Count > 0)
                    ((DataSet)ViewState["dsCorr"]).Tables[0].Rows.Clear();
                this.gridViewResult.Visible = false;
                this.ListBtnSave.Enabled = false;
                this.upPnlGridList.Update();
                this.UpDocumentButtons.Update();
            }

            if (this.deleteCorr.Value == "true")
            {
                string idLista = Session["listaSelected"].ToString();
                int rigaSelezionata = (gridViewResult.PageSize * gridViewResult.PageIndex) + RowSelectedResult.RowIndex;
                if (RowSelectedResult.DataItemIndex == 0 && gridViewResult.Rows.Count == 1 && gridViewResult.PageIndex != 0)
                    gridViewResult.PageIndex = gridViewResult.PageIndex - 1;
                if (!String.IsNullOrEmpty(idLista))
                {
                    ((DataSet)ViewState["dsCorr"]).Tables[0].Rows[rigaSelezionata].Delete();
                    ((DataSet)ViewState["dsCorr"]).Tables[0].AcceptChanges();
                    UserManager.modificaLista(((DataSet)ViewState["dsCorr"]), idLista);
                    gridViewResult.DataSource = ((DataSet)ViewState["dsCorr"]);
                    gridViewResult.DataBind();

                    //this.panelResult.Visible = true;

                    if (gridViewResult.Rows.Count == 0)
                    {
                        UserManager.deleteListaDistribuzione(idLista);
                        dsListe = UserManager.getListePerRuoloUt();
                        grdList.DataSource = dsListe;
                        grdList.DataBind();
                        this.upPnlGridList.Update();
                        this.TxtCodList.Text = "";
                        this.TxtDescList.Text = "";
                        this.TxtCodeSender.Text = "";
                        this.TxtDescriptionSender.Text = "";
                        this.UpdPanelDetail.Update();
                        this.rbRole.Checked = false;
                        this.rbOnlyMe.Checked = true;
                        //this.UpdPnDispVis.Visible = false;
                        this.UpdPnDisp.Update();
                        //this.UpdPnDispVis.Visible = false;                       
                        //this.panelDetail.Visible = false;          
                        if (ViewState["dsCorr"] != null && ((DataSet)ViewState["dsCorr"]).Tables.Count > 0)
                            ((DataSet)ViewState["dsCorr"]).Tables[0].Rows.Clear();
                        this.gridViewResult.Visible = false;
                        this.UpnlGrid.Update();
                        //this.panelResult.Visible = false;                       

                        grdList.SelectedIndex = -1;
                    }

                    this.UpnlGrid.Update();
                }
                else
                {
                    ((DataSet)ViewState["dsCorr"]).Tables[0].Rows[rigaSelezionata].Delete();
                    gridViewResult.DataSource = ((DataSet)ViewState["dsCorr"]);
                    gridViewResult.DataBind();
                    this.UpnlGrid.Update();
                    //gridViewResult.Visible = true;
                }
                //Commentata la seguente riga di codice:si perde l'informazione dell'elemento selezionato a sinistra e quindi non elimina il prossimo elemento
                //Session["listaSelected"] = string.Empty;
                this.deleteCorr.Value = string.Empty;
                //this.EnableDibledButtons(false);

                this.UpDocumentButtons.Update();
                this.UpPnlRadio.Update();
            }


            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeFrame", "resizeIframe();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
        }

        private void deleteSelectedList(string idLista)
        {

            if (!String.IsNullOrEmpty(idLista))
            {
                Session.Remove("selCorrDaRubrica");

                grdList.PageIndex = 0;
                UserManager.deleteListaDistribuzione(idLista);
                dsListe = UserManager.getListePerRuoloUt();
                grdList.DataSource = dsListe;
                grdList.DataBind();
                this.upPnlGridList.Update();

                //gridViewResult.Visible = false;

                grdList.SelectedIndex = -1;
                this.deleteList.Value = "";
            }
        }


        protected void ListBtnNew_Click(object sender, EventArgs e)
        {
            Session.Remove("selCorrDaRubrica");
            this.RowSelected = null;

            this.grdList.SelectedIndex = -1;
            this.gridViewResult.SelectedIndex = -1;

            this.grdList.PageIndex = 0;
            this.gridViewResult.PageIndex = 0;

            this.TxtCodeSender.Text = string.Empty;
            //TxtCodeSender.Visible = true;

            this.TxtDescList.Text = string.Empty;
            this.TxtDescList.Visible = true;
            //TxtDescList.BackColor = System.Drawing.Color.White;            

            this.TxtCodList.Text = string.Empty;
            this.TxtCodList.ReadOnly = false;

            if (ViewState["dsCorr"] != null && ((DataSet)ViewState["dsCorr"]).Tables.Count > 0)
                ((DataSet)ViewState["dsCorr"]).Tables[0].Rows.Clear();

            //verificare se far comparire sempre l'intestazione della tabella
            this.gridViewResult.DataSource = ((DataSet)ViewState["dsCorr"]);
            this.gridViewResult.DataBind();
            this.gridViewResult.Visible = true;
            //this.panelResult.Visible = true;


            //this.UpdPnDispVis.Visible = true;
            this.UpdPnDisp.Update();
            //this.panelDetail.Visible = true;
            this.UpdPanelDetail.Update();

            this.rbOnlyMe.Checked = true;
            this.rbRole.Checked = false;

            //this.ListBtnNew.Enabled = false;
            this.ListBtnSave.Enabled = true;
            //this.ListBntAnnulla.Enabled = true;

            this.UpDocumentButtons.Update();

            this.EnableDibledButtons(true);
            this.UpPnlRadio.Update();
            this.UpnlGrid.Update();
            //this.panelDetail.Visible = true;
            this.UpdPanelDetail.Update();
            this.upPnlGridList.Update();

        }

        private void EnableDibledButtons(bool enable)
        {
            this.rbOnlyMe.Enabled = enable;
            this.rbRole.Enabled = enable;
            this.TxtCodList.ReadOnly = !enable;
            this.TxtDescList.ReadOnly = !enable;
            this.TxtCodeSender.ReadOnly = !enable;
            this.TxtDescriptionSender.ReadOnly = !enable;
            this.DocumentImgSenderAddressBook.Enabled = enable;
            this.DocumentImgAddNewCorrispondent.Enabled = enable;

        }

        protected void ListBtnSave_Click(object sender, EventArgs e)
        {
            Session.Remove("selCorrDaRubrica");
            string msgDesc = "";
            if (this.TxtCodList.Text.Contains("(") || this.TxtCodList.Text.Contains(")"))
            {
                msgDesc = "WarningDistributionListCodeInvalid";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                return;
            }
            if (this.RowSelected != null)
            {
                if (TxtDescList.Text == "" || TxtDescList.Text.Trim() == "" || ((DataSet)ViewState["dsCorr"]).Tables[0].Rows.Count == 0 || TxtCodList.Text == "" || TxtCodList.Text.Trim() == "")
                {
                    msgDesc = "WarningDistributionListInsert";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }

                //Modifica di una lista esistente
                if (this.RowSelected == null)
                {
                    msgDesc = "WarningDistributionListSelected";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }


                //string idLista = ((HiddenField)grdList.SelectedRow.Cells[1].FindControl("listId")).Value;
                string idLista = ((HiddenField)grdList.Rows[this.RowSelected.RowIndex].Cells[1].FindControl("listId")).Value;
                string nomeLista = TxtDescList.Text.Replace("'", "''");
                string codiceLista = TxtCodList.Text.Replace("'", "''");

                if (nomeLista != UserManager.getNomeLista(this, UserManager.getCodiceLista(idLista), UserManager.GetInfoUser().idAmministrazione).Replace("'", "''"))
                {
                    if (!UserManager.isUniqueNomeLista(nomeLista))
                    {
                        msgDesc = "WarningDistributionListName";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                }

                if (this.rbOnlyMe.Checked)
                {
                    UserManager.modificaListaUser(((DataSet)ViewState["dsCorr"]), idLista, nomeLista, codiceLista, idUtente);
                }
                else
                {
                    string idGruppo = RoleManager.GetRoleInSession().idGruppo;
                    UserManager.modificaListaGruppo(((DataSet)ViewState["dsCorr"]), idLista, nomeLista, codiceLista, idGruppo);
                }

                msgDesc = "WarningDistributionListOk";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'check', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'check', '');}", true);

                grdList.SelectedIndex = -1;

                //this.panelDetail.Visible = false;
                this.UpdPanelDetail.Update();
                ////this.UpnlGrid.Visible = false;
                this.UpnlGrid.Update();

                dsListe = UserManager.getListePerRuoloUt();
                grdList.DataSource = dsListe;
                grdList.DataBind();
                this.upPnlGridList.Update();
            }
            else
            {
                //Inserimento di una nuova lista
                if (TxtDescList.Text == "" || TxtDescList.Text.Trim() == "" || ((DataSet)ViewState["dsCorr"]).Tables[0].Rows.Count == 0 || TxtCodList.Text == "" || TxtCodList.Text.Trim() == "")
                {
                    msgDesc = "WarningDistributionListInsert";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }
                else
                {
                    string nomeLista = TxtDescList.Text.Replace("'", "''");
                    string codiceLista = TxtCodList.Text.Replace("'", "''");

                    if (!UserManager.isUniqueCod(codiceLista, UserManager.GetInfoUser().idAmministrazione))
                    {
                        msgDesc = "WarningDistributionListCode";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                    if (!UserManager.isUniqueNomeLista(nomeLista))
                    {
                        msgDesc = "WarningDistributionListName";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }

                    string idGruppo = RoleManager.GetRoleInSession().idGruppo;

                    if (this.rbOnlyMe.Checked)
                    {
                        UserManager.salvaLista(((DataSet)ViewState["dsCorr"]), nomeLista, codiceLista, idUtente, UserManager.GetInfoUser().idAmministrazione, "no");
                    }
                    else
                    {
                        UserManager.salvaLista(((DataSet)ViewState["dsCorr"]), nomeLista, codiceLista, idGruppo, UserManager.GetInfoUser().idAmministrazione, "yes");
                    }


                    //msgDesc = "WarningDistributionListOk";
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    grdList.SelectedIndex = -1;

                    dsListe = UserManager.getListePerRuoloUt();
                    grdList.DataSource = dsListe;
                    grdList.DataBind();
                    this.TxtCodList.ReadOnly = false;
                    this.RowSelected = null;
                    this.upPnlGridList.Update();

                    //this.panelDetail.Visible = false;
                    this.UpdPanelDetail.Update();
                    //this.UpnlGrid.Visible = false;
                    this.UpnlGrid.Update();

                }
            }

            //this.ListBtnNew.Enabled = true;
            //this.ListBtnSave.Enabled = false;
            //this.ListBntAnnulla.Enabled = false;
            //this.UpDocumentButtons.Update();

            //TxtCodList.ReadOnly = true;
            this.UpdPanelDetail.Update();

        }

        //protected void ListBntAnnulla_Click(object sender, EventArgs e)
        //{
        //    Session.Remove("selCorrDaRubrica");

        //    this.TxtCodList.Text = "";
        //    this.TxtDescList.Text = "";
        //    this.TxtCodeSender.Text = "";
        //    this.TxtDescriptionSender.Text = "";
        //    this.UpdPanelDetail.Update();
        //    this.rbRole.Checked = false;
        //    this.rbOnlyMe.Checked = true;
        //    this.UpdPnDisp.Update();
        //    this.ListBtnNew.Enabled = true;
        //    this.ListBtnSave.Enabled = false;
        //    this.ListBntAnnulla.Enabled = false;
        //    this.UpDocumentButtons.Update();
        //}

        protected void DocumentImgSenderAddressBook_Click(object sender, ImageClickEventArgs e)
        {
            this.CallType = RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE;

            HttpContext.Current.Session["AddressBook.from"] = "DISTRIBUTIONLIST";

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AddressBook", "ajaxModalPopupAddressBook();", true);
        }

        public RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }

        }

        protected void TxtCodeSender_TextChanged(object sender, EventArgs e)
        {
            DocsPaWR.Corrispondente corr = null;
            if (TxtCodeSender.Text != "")
            {
                corr = AddressBookManager.getCorrispondenteRubrica(TxtCodeSender.Text, DocsPaWR.RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE);
            }
            if (corr == null)
            {
                if (TxtCodeSender.Text != "")
                {
                    string msg = "WarningDocumentCorrNotFound";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                }
                TxtDescriptionSender.Text = "";
            }
            else
            {
                TxtDescriptionSender.Text = corr.descrizione;
            }
        }

        private bool verificaDuplicazioneCorr(DocsPaWR.Corrispondente corr)
        {
            if (ViewState["dsCorr"] != null && ((DataSet)ViewState["dsCorr"]).Tables[0] != null && ((DataSet)ViewState["dsCorr"]).Tables[0].Rows != null && ((DataSet)ViewState["dsCorr"]).Tables[0].Rows.Count != 0)
            {
                for (int i = 0; i < ((DataSet)ViewState["dsCorr"]).Tables[0].Rows.Count; i++)
                {
                    if (corr != null && corr.systemId == ((DataSet)ViewState["dsCorr"]).Tables[0].Rows[i][0].ToString())
                        return true;
                }
            }
            return false;
        }

        private void addCorrSelDaRubrica(List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList)
        {
            if (atList != null && atList.Count > 0)
                foreach (NttDataWA.Popup.AddressBook.CorrespondentDetail a in atList)
                {
                    Corrispondente tempCorrSingle;

                    if (!a.isRubricaComune)
                        tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(a.SystemID);
                    else
                        tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(a.CodiceRubrica);

                    if (tempCorrSingle != null && !verificaDuplicazioneCorr(tempCorrSingle))
                    {
                        if (ViewState["dsCorr"] == null)
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add("ID_DPA_CORR");
                            dt.Columns.Add("VAR_DESC_CORR");
                            dt.Columns.Add("VAR_COD_RUBRICA");
                            dt.Columns.Add("CHA_TIPO_IE");
                            dt.Columns.Add("CHA_DISABLED_TRASM");
                            dsCorrispondenti = new DataSet();
                            dsCorrispondenti.Tables.Add(dt);
                            ViewState.Add("dsCorr", dsCorrispondenti);
                        }

                        DataRow dr = ((DataSet)ViewState["dsCorr"]).Tables[0].NewRow();
                        dr[0] = tempCorrSingle.systemId;
                        dr[1] = tempCorrSingle.descrizione;
                        dr[2] = tempCorrSingle.codiceRubrica;
                        dr[3] = tempCorrSingle.tipoIE;
                        if (tempCorrSingle.disabledTrasm)
                            dr[4] = "1";
                        else
                            dr[4] = "0";

                        ((DataSet)ViewState["dsCorr"]).Tables[0].Rows.Add(dr);
                    }
                }
            gridViewResult.DataSource = ((DataSet)ViewState["dsCorr"]);
            gridViewResult.DataBind();
            this.UpnlGrid.Update();
            TxtDescriptionSender.Text = "";
        }

        protected void DocumentImgAddNewCorrispondent_Click(object sender, ImageClickEventArgs e)
        {
            Session.Remove("selCorrDaRubrica");

            DocsPaWR.Corrispondente corr = null;

            if (!string.IsNullOrEmpty(this.TxtCodeSender.Text) && !string.IsNullOrEmpty(this.TxtDescriptionSender.Text))
            {
                corr = AddressBookManager.getCorrispondenteRubrica(TxtCodeSender.Text, DocsPaWR.RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE);

                if (corr != null && !string.IsNullOrEmpty(corr.systemId))
                {
                    if (!verificaDuplicazioneCorr(corr))
                    {
                        if (ViewState["dsCorr"] == null)
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add("ID_DPA_CORR");
                            dt.Columns.Add("VAR_DESC_CORR");
                            dt.Columns.Add("VAR_COD_RUBRICA");
                            dt.Columns.Add("CHA_TIPO_IE");
                            dt.Columns.Add("CHA_DISABLED_TRASM");
                            dsCorrispondenti = new DataSet();
                            dsCorrispondenti.Tables.Add(dt);
                            ViewState.Add("dsCorr", dsCorrispondenti);
                        }

                        DataRow dr = ((DataSet)ViewState["dsCorr"]).Tables[0].NewRow();
                        dr[0] = corr.systemId;
                        dr[1] = corr.descrizione;
                        if (corr.disabledTrasm)
                            dr[4] = "1";
                        else
                            dr[4] = "0";
                        ((DataSet)ViewState["dsCorr"]).Tables[0].Rows.Add(dr);
                        gridViewResult.DataSource = ((DataSet)ViewState["dsCorr"]);
                        gridViewResult.DataBind();
                        TxtCodeSender.Text = "";
                        TxtDescriptionSender.Text = "";
                        this.UpnlGrid.Update();

                    }
                    else
                    {
                        this.TxtCodeSender.Text = "";
                        this.TxtDescriptionSender.Text = "";
                        this.UpnlGrid.Update();
                    }
                }
            }
        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
            string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();
            if (atList != null && atList.Count > 0)
            {
                NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                if (atList != null && atList.Count > 0)
                {
                    if (addressBookCallFrom == "DISTRIBUTIONLIST")
                    {
                        addCorrSelDaRubrica(atList);
                        this.TxtCodeSender.Text = string.Empty;
                        this.TxtDescriptionSender.Text = string.Empty;
                        this.idCorrispondente.Value = string.Empty;
                        this.UpdPanelDetail.Update();
                    }
                }
            }
            HttpContext.Current.Session["AddressBook.At"] = null;
            HttpContext.Current.Session["AddressBook.Cc"] = null;
        }

        protected string GetViewLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode("ClassificationsTooltipCell0", language);
        }

        protected string GetRemoveLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode("AttachmentsBtnRemove", language);
        }

        protected void gridViewResult_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            Session.Remove("selCorrDaRubrica");
            gridViewResult.PageIndex = e.NewPageIndex;
            gridViewResult.DataSource = ((DataSet)ViewState["dsCorr"]);
            gridViewResult.DataBind();

            this.UpnlGrid.Update();
        }

        protected void grdList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            Session.Remove("selCorrDaRubrica");
            grdList.SelectedIndex = -1;
            grdList.PageIndex = e.NewPageIndex;
            dsListe = UserManager.getListePerRuoloUt();
            grdList.DataSource = dsListe;
            grdList.DataBind();
            //this.panelDetail.Visible = false;

            this.TxtCodList.Text = "";
            this.TxtDescList.Text = "";
            this.TxtCodeSender.Text = "";
            this.TxtDescriptionSender.Text = "";

            this.rbOnlyMe.Checked = true;
            this.rbRole.Checked = false;

            this.UpdPanelDetail.Update();
            //this.UpdPnDispVis.Visible = false;
            this.UpdPnDisp.Update();

            //this.panelResult.Visible = false;   
            this.gridViewResult.Visible = false;
            this.UpnlGrid.Update();

            this.upPnlGridList.Update();
            this.RowSelected = null;

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

        protected GridViewRow RowSelectedResult
        {
            get
            {
                return HttpContext.Current.Session["RowSelectedResult"] as GridViewRow;
            }
            set
            {
                HttpContext.Current.Session["RowSelectedResult"] = value;
            }
        }

        private bool EnableAjaxAddressBook
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableAjaxAddressBook"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableAjaxAddressBook"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableAjaxAddressBook"] = value;
            }
        }

        private int AjaxAddressBookMinPrefixLenght
        {
            get
            {
                int result = 3;
                if (HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"] = value;
            }
        }
    }

}
