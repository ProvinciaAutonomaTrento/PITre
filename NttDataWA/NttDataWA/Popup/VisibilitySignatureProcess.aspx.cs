using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class VisibilitySignatureProcess : System.Web.UI.Page
    {
        private List<Corrispondente> ListaVisibilitaProcesso
        {
            get
            {
                if (HttpContext.Current.Session["ListaVisibilitaProcesso"] != null)
                    return (List<Corrispondente>)HttpContext.Current.Session["ListaVisibilitaProcesso"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ListaVisibilitaProcesso"] = value;
            }
        }

        private ProcessoFirma ProcessoDiFirmaSelected
        {
            get
            {
                if (HttpContext.Current.Session["ProcessoDiFirmaSelected"] != null)
                    return (ProcessoFirma)HttpContext.Current.Session["ProcessoDiFirmaSelected"];
                else
                    return null;
            }
        }

        private List<ProcessoFirma> ListaProcessiDiFirma
        {
            get
            {
                if (HttpContext.Current.Session["ListaProcessiDiFirma"] != null)
                    return (List<ProcessoFirma>)HttpContext.Current.Session["ListaProcessiDiFirma"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ListaProcessiDiFirma"] = value;
            }
        }

        public RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_PROTO_INT_DEST;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }
        }

        private int SelectedPage
        {
            get
            {
                int toReturn = 1;
                if (HttpContext.Current.Session["selectedPageVisibilityProcess"] != null) Int32.TryParse(HttpContext.Current.Session["selectedPageVisibilityProcess"].ToString(), out toReturn);
                if (toReturn < 1) toReturn = 1;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["selectedPageVisibilityProcess"] = value;
            }
        }

        private List<FiltroProcessoFirma> FiltroRicerca
        {
            get
            {
                return HttpContext.Current.Session["FiltroRicercaVisibilitySignatureProcess"] as List<FiltroProcessoFirma>;
            }
            set
            {
                HttpContext.Current.Session["FiltroRicercaVisibilitySignatureProcess"] = value;
            }
        }

        private const string CLOSE_POPUP_ADD_FILTER_VISIBILITY = "closeAddFilterVisibility";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.InitializeLanguage();
                this.InitializaPage();
            }
            else
            {
                BuildTableProcessRole();
                ReadRetValueFromPopup();
            }
            RefreshScript();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("VisibilitySignatureProcessBtnClose", language);
            //this.BtnConfirm.Text = Utils.Languages.GetLabelFromCode("VisibilitySignatureProcessBtnConfirm", language);
            this.LitVisibilitySignatureProcessCorr.Text = Utils.Languages.GetLabelFromCode("LitVisibilitySignatureProcessCorr", language);
            this.IndexImgAddFilter.ToolTip = Utils.Languages.GetLabelFromCode("IndexImgAddFilterTooltip", language);
            this.IndexImgRemoveFilter.ToolTip = Utils.Languages.GetLabelFromCode("IndexImgRemoveFilterTooltip", language);
            this.AddFilterVisibilitySignatureProcess.Title = Utils.Languages.GetLabelFromCode("VisibilitySignatureAddFilterTitle", language);
            this.LtlTipoVisibilita.Text = Utils.Languages.GetLabelFromCode("VisibilitySignatureLtlTipoVisibilita", language);
            this.rb_monitoratore.Text = Utils.Languages.GetLabelFromCode("VisibilitySignatureMonitoratore", language);
            this.rb_proponente.Text = Utils.Languages.GetLabelFromCode("VisibilitySignatureProponente", language);
            this.BtnAssegnaVisibilita.Text = Utils.Languages.GetLabelFromCode("VisibilitySignatureBtnAssegnaVisibilita", language);
            this.LtlAssegnaVisibilita.Text = Utils.Languages.GetLabelFromCode("VisibilitySignatureLtlAssegnaVisibilita", language);
            this.BtnEsportaVisibilita.Text = Utils.Languages.GetLabelFromCode("VisibilitySignatureBtnEsportaVisibilita", language);
            this.ExportDati.Title = Utils.Languages.GetLabelFromCode("VisibilitySignatureEsportaVisibilita", language);
        }

        private void InitializaPage()
        {
            this.FiltroRicerca = null;
            // LoadListaCorr();
            BuildTableProcessRole();
            SetAjaxAddressBook();
            //GridViewResult_Bind();
        }

        private void BuildTableProcessRole()
        {
            this.plcGridProcessRole.Controls.Clear();
            List<VisibilitaProcessoRuolo> visibilita = new List<VisibilitaProcessoRuolo>();
            if (this.ProcessoDiFirmaSelected != null && !string.IsNullOrEmpty(this.ProcessoDiFirmaSelected.idProcesso))
            {
                visibilita = LoadListaCorr(this.ProcessoDiFirmaSelected.idProcesso);
                Table table = CreateTable(ProcessoDiFirmaSelected, visibilita);
                this.plcGridProcessRole.Controls.Add(table);
            }
            else
            {
                foreach (ProcessoFirma processo in this.ListaProcessiDiFirma)
                {
                    if (processo.passi != null && processo.passi.Count() > 0)
                    {
                        visibilita = LoadListaCorr(processo.idProcesso);
                        Table table = CreateTable(processo, visibilita);
                        this.plcGridProcessRole.Controls.Add(table);
                    }
                }
            }
            this.UpPnlGridProcessRole.Update();
        }

        private Table CreateTable(ProcessoFirma processo, List<VisibilitaProcessoRuolo> visibilita)
        {
            Table tbl = new Table();
            tbl.ID = "table_" + processo.idProcesso;
            tbl.CssClass = "tbl_rounded_custom";

            // header
            TableRow rowProcesso = new TableRow();

            TableCell cell1 = new TableCell();
            cell1.ColumnSpan = 3;
            cell1.ID = "cellProcess_" + processo.idProcesso;
            cell1.CssClass = "th first";
            cell1.Text = processo.nome;
            rowProcesso.Controls.Add(cell1);
            rowProcesso.ID = "rowProcesso_" + processo.idProcesso;

            tbl.Controls.Add(rowProcesso);

            TableRow rowRuoli;
            foreach (VisibilitaProcessoRuolo vis in visibilita)
            {
                rowRuoli = new TableRow();

                TableCell cellRoleDescription = new TableCell();
                cellRoleDescription.ID = "roleDescription_" + processo.idProcesso + "_" + vis.ruolo.systemId;
                cellRoleDescription.CssClass = "role";
                cellRoleDescription.Text = vis.ruolo.descrizione + " (" + vis.ruolo.codiceRubrica + ")";
                rowRuoli.Cells.Add(cellRoleDescription);

                TableCell cellTipoVis = new TableCell();
                cellTipoVis.ID = "tipoVisibilita_" + processo.idProcesso + "_" + vis.ruolo.idGruppo;
                cellTipoVis.CssClass = "center";

                DropDownList ddlTipoVis = new DropDownList();
                ddlTipoVis.EnableViewState = false;
                ddlTipoVis.ID = "ddlTipoVis_" + processo.idProcesso + "_" + vis.ruolo.idGruppo;
                ddlTipoVis.CssClass = "chzn-select-deselect";
                ddlTipoVis.AutoPostBack = true;
                ddlTipoVis.Width = 150;
                ddlTipoVis.Attributes.Add("onchange", "disallowOp('ContentPlaceHolderContent');");
                ddlTipoVis.SelectedIndexChanged += new EventHandler(ddlTipoVis_SelectedIndexChanged);
                ddlTipoVis.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("VisibilitySignatureProponente", UserManager.GetUserLanguage()), TipoVisibilita.PROPONENTE.ToString()));
                ddlTipoVis.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("VisibilitySignatureMonitoratore", UserManager.GetUserLanguage()), TipoVisibilita.MONITORATORE.ToString()));

                ddlTipoVis.SelectedValue = vis.tipoVisibilita.ToString();

                cellTipoVis.Controls.Add(ddlTipoVis);

                rowRuoli.Cells.Add(cellTipoVis);

                TableCell cellAction = new TableCell();
                cellAction.ID = "rimuovi_" + processo.idProcesso + "_" + vis.ruolo.idGruppo;
                cellAction.CssClass = "center";

                CustomImageButton btnDelete = new CustomImageButton();
                btnDelete.ID = "btnDelete_" + processo.idProcesso + "_" + vis.ruolo.idGruppo;
                btnDelete.ImageUrl = "../Images/Icons/delete.png";
                btnDelete.OnMouseOutImage = "../Images/Icons/delete.png";
                btnDelete.OnMouseOverImage = "../Images/Icons/delete_hover.png";
                btnDelete.ImageUrlDisabled = "../Images/Icons/delete.png";
                btnDelete.CssClass = "clickable";
                btnDelete.Click += BtnRimuoviVisibilita_Click;
                btnDelete.Attributes.Add("onClick", "disallowOp('ContentPlaceHolderContent');");
                cellAction.Controls.Add(btnDelete);

                rowRuoli.Cells.Add(cellAction);

                tbl.Controls.Add(rowRuoli);
            }

            return tbl;
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        private void ReadRetValueFromPopup()
        {
            if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_ADD_FILTER_VISIBILITY)))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                if (!string.IsNullOrEmpty(this.AddFilterVisibilitySignatureProcess.ReturnValue))
                {
                    this.IndexImgRemoveFilter.Enabled = true;
                    //this.LoadListCorr();
                    //GridViewResult_Bind();
                    BuildTableProcessRole();
                    this.UpPnlFilter.Update();
                }
            }
        }
        protected void BtnConfirm_Click(object sender, EventArgs e)
        {

        }

        protected void SetAjaxAddressBook()
        {
            string dataUser = UIManager.RoleManager.GetRoleInSession().systemId;
            Registro reg = RegistryManager.GetRegistryInSession();
            if (reg == null)
            {
                reg = RoleManager.GetRoleInSession().registri[0];
            }
            dataUser = dataUser + "-" + reg.systemId;

            string callType = "CALLTYPE_IN_ONLY_ROLE";
            this.RapidCorr.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
        }

        protected void BtnAddRole_Click(object sender, EventArgs e)
        {
            try
            {
                Ruolo ruolo = new Ruolo();
                ruolo.idGruppo = this.idCorr.Value;
                ruolo.codiceRubrica = this.TxtCodeCorr.Text;
                ruolo.descrizione = this.TxtDescriptionCorr.Text;
                TipoVisibilita tipoVis = (TipoVisibilita)Enum.Parse(typeof(TipoVisibilita), RblTipoVisibilita.SelectedValue);

                List<VisibilitaProcessoRuolo> visibilita = new List<VisibilitaProcessoRuolo>();
                if (this.ProcessoDiFirmaSelected != null && !string.IsNullOrEmpty(this.ProcessoDiFirmaSelected.idProcesso))
                {
                    visibilita.Add(new VisibilitaProcessoRuolo()
                    {
                        idProcesso = this.ProcessoDiFirmaSelected.idProcesso,
                        ruolo = ruolo,
                        tipoVisibilita = tipoVis
                    });
                }
                else
                {
                    foreach (ProcessoFirma processo in this.ListaProcessiDiFirma)
                    {
                        if (processo.passi != null && processo.passi.Count() > 0)
                        {
                            visibilita.Add(new VisibilitaProcessoRuolo()
                            {
                                idProcesso = this.ProcessoDiFirmaSelected.idProcesso,
                                ruolo = ruolo,
                                tipoVisibilita = tipoVis
                            });
                        }
                    }
                }
                //AGGIUNGO SUL DB
                if (UIManager.SignatureProcessesManager.InsertVisibilitaProcesso(visibilita))
                {
                    //AGGIORNO LA LISTA IN SESSIONE ED AGGIORNO LA GRIGLIA
                    this.TxtCodeCorr.Text = string.Empty;
                    this.TxtDescriptionCorr.Text = string.Empty;
                    this.BtnAssegnaVisibilita.Enabled = false;
                    this.UpdPnlCorr.Update();
                    this.UpPnlButtons.Update();
                }
                else
                {
                    string msg = "ErrorVisibilitySignatureProcessCorrispondent";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                }
                BuildTableProcessRole();
            }
            catch (Exception ex)
            {
                string msg = "ErrorVisibilitySignatureProcessCorrispondent";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('VisibilitySignatureProcess','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnAddressBook_Click(object sender, EventArgs e)
        {
            this.CallType = RubricaCallType.CALLTYPE_CORR_INT;
            HttpContext.Current.Session["AddressBook.from"] = "VISIBILITY_SIGNATURE_PROCESS";
            HttpContext.Current.Session["AddressBook.EnableOnly"] = "R";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAddressBook", "parent.ajaxModalPopupAddressBook();", true);
        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                if (atList != null && atList.Count > 0)
                {
                    List<VisibilitaProcessoRuolo> visibilita = new List<VisibilitaProcessoRuolo>();
                    Ruolo ruolo;
                    foreach (NttDataWA.Popup.AddressBook.CorrespondentDetail corr in atList)
                    {
                        ruolo = new Ruolo();
                        ruolo = RoleManager.getRuoloById(corr.SystemID);
                        TipoVisibilita tipoVis = (TipoVisibilita)Enum.Parse(typeof(TipoVisibilita), RblTipoVisibilita.SelectedValue);

                        if (this.ProcessoDiFirmaSelected != null && !string.IsNullOrEmpty(this.ProcessoDiFirmaSelected.idProcesso))
                        {
                            visibilita.Add(new VisibilitaProcessoRuolo()
                            {
                                idProcesso = this.ProcessoDiFirmaSelected.idProcesso,
                                ruolo = ruolo,
                                tipoVisibilita = tipoVis
                            });
                        }
                        else
                        {
                            foreach (ProcessoFirma processo in this.ListaProcessiDiFirma)
                            {
                                if (processo.passi != null && processo.passi.Count() > 0)
                                {
                                    visibilita.Add(new VisibilitaProcessoRuolo()
                                    {
                                        idProcesso = processo.idProcesso,
                                        ruolo = ruolo,
                                        tipoVisibilita = tipoVis
                                    });
                                }
                            }
                        }
                    }
                    if (UIManager.SignatureProcessesManager.InsertVisibilitaProcesso(visibilita))
                    {

                    }
                    BuildTableProcessRole();
                }
                HttpContext.Current.Session["AddressBook.At"] = null;
                HttpContext.Current.Session["AddressBook.Cc"] = null;

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void IndexImgAddFilter_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAddFilterVisibilitySignatureProcess", "parent.ajaxModalPopupAddFilterVisibilitySignatureProcess();", true);
            }
            catch (Exception ex)
            {

            }
        }

        protected void IndexImgRemoveFilter_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.IndexImgRemoveFilter.Enabled = false;
            this.FiltroRicerca = null;
            //LoadListCorr();
            //GridViewResult_Bind();
            BuildTableProcessRole();
            this.UpPnlFilter.Update();
        }

        protected void ddlTipoVis_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            try
            {
                DropDownList ddlTipoVis = (DropDownList)this.plcGridProcessRole.FindControl((((DropDownList)sender).ID));
                string idTipoVis = (((DropDownList)sender).ID);
                string idProcesso = idTipoVis.Split('_')[1];
                string idGruppo = idTipoVis.Split('_')[2];

                TipoVisibilita tipoVisibilita = (TipoVisibilita)Enum.Parse(typeof(TipoVisibilita), ddlTipoVis.SelectedValue);

                if (!UIManager.SignatureProcessesManager.UpdateTipoVisibilitaProcesso(idProcesso, idGruppo, tipoVisibilita))
                {
                    string msg = "ErrorVisibilitySignatureProcessDeleteCorr";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                }
                else
                {
                    this.BuildTableProcessRole();
                }
            }
            catch (Exception ex)
            {
                string msg = "ErrorVisibilitySignatureProcessDeleteCorr";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        protected void BtnRimuoviVisibilita_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            try
            {
                string idBtnRimuoviBisibilita = (((CustomImageButton)sender).ID);
                string idProcesso = idBtnRimuoviBisibilita.Split('_')[1];
                string idGruppo = idBtnRimuoviBisibilita.Split('_')[2];

                if (!UIManager.SignatureProcessesManager.RimuoviVisibilitaProcesso(idProcesso, idGruppo))
                {
                    string msg = "ErrorVisibilitySignatureProcessDeleteCorr";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                }
                else
                {
                    this.BuildTableProcessRole();
                }
            }
            catch (Exception ex)
            {
                string msg = "ErrorVisibilitySignatureProcessDeleteCorr";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }

        }

        private void GridViewResult_Bind()
        {
            this.GridViewResult.DataSource = this.ListaVisibilitaProcesso;
            this.GridViewResult.DataBind();
            this.UpnlGrid.Update();
        }

        protected void GridViewResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
        }

        protected void GridViewResult_PreRender(object sender, EventArgs e)
        {
        }



        protected void GridViewResult_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.GridViewResult.PageIndex = e.NewPageIndex;
                GridViewResult_Bind();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BuildGridNavigator()
        {
            try
            {
                this.plcNavigator.Controls.Clear();

                int countPage = (int)Math.Round(((double)(this.ListaVisibilitaProcesso.Count * 2) / (double)this.GridViewResult.PageSize) + 0.49);

                int val = (this.ListaVisibilitaProcesso.Count * 2) % this.GridViewResult.PageSize;
                //if (val == 0)
                //{
                //    countPage = countPage - 1;
                //}

                if (countPage > 1)
                {
                    Panel panel = new Panel();
                    panel.EnableViewState = true;
                    panel.CssClass = "recordNavigator";

                    int startFrom = 1;
                    if (int.Parse(this.grid_pageindex.Value) > 6) startFrom = int.Parse(this.grid_pageindex.Value) - 5;

                    int endTo = 10;
                    if (int.Parse(this.grid_pageindex.Value) > 6) endTo = int.Parse(this.grid_pageindex.Value) + 5;
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
                        if (i == int.Parse(this.grid_pageindex.Value))
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

                    this.plcNavigator.Controls.Add(panel);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GridViewResult_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void BtnAssegnaVisibilita_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            try
            {
                Ruolo ruolo = new Ruolo();
                ruolo.idGruppo = this.idCorr.Value;
                ruolo.codiceRubrica = this.TxtCodeCorr.Text;
                ruolo.descrizione = this.TxtDescriptionCorr.Text;
                TipoVisibilita tipoVis = (TipoVisibilita)Enum.Parse(typeof(TipoVisibilita), RblTipoVisibilita.SelectedValue);

                List<VisibilitaProcessoRuolo> visibilita = new List<VisibilitaProcessoRuolo>();
                if (this.ProcessoDiFirmaSelected != null && !string.IsNullOrEmpty(this.ProcessoDiFirmaSelected.idProcesso))
                {
                    visibilita.Add(new VisibilitaProcessoRuolo()
                    {
                        idProcesso = this.ProcessoDiFirmaSelected.idProcesso,
                        ruolo = ruolo,
                        tipoVisibilita = tipoVis
                    });
                }
                else
                {
                    foreach (ProcessoFirma processo in this.ListaProcessiDiFirma)
                    {
                        if (processo.passi != null && processo.passi.Count() > 0)
                        {
                            visibilita.Add(new VisibilitaProcessoRuolo()
                            {
                                idProcesso = processo.idProcesso,
                                ruolo = ruolo,
                                tipoVisibilita = tipoVis
                            });
                        }
                    }
                }
                //AGGIUNGO SUL DB
                if (UIManager.SignatureProcessesManager.InsertVisibilitaProcesso(visibilita))
                {
                    //AGGIORNO LA LISTA IN SESSIONE ED AGGIORNO LA GRIGLIA
                    this.TxtCodeCorr.Text = string.Empty;
                    this.TxtDescriptionCorr.Text = string.Empty;
                    this.BtnAssegnaVisibilita.Enabled = false;
                    this.UpdPnlCorr.Update();
                    this.UpPnlButtons.Update();
                }
                else
                {
                    string msg = "ErrorVisibilitySignatureProcessCorrispondent";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                }
                BuildTableProcessRole();
            }
            catch (Exception ex)
            {
                string msg = "ErrorVisibilitySignatureProcessDeleteCorr";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        protected void ImgDeleteVisibility_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            try
            {
                CustomImageButton btnIm = (CustomImageButton)sender;
                GridViewRow row = (GridViewRow)btnIm.Parent.Parent;
                //int rowIndex = row.RowIndex;

                string idCorr = (row.FindControl("systemIdCorr") as Label).Text;
                if (UIManager.SignatureProcessesManager.RimuoviVisibilitaProcesso(this.ProcessoDiFirmaSelected.idProcesso, idCorr))
                {
                    this.ListaVisibilitaProcesso = (from c in this.ListaVisibilitaProcesso where !c.systemId.Equals(idCorr) select c).ToList();
                    this.GridViewResult_Bind();
                }
                else
                {
                    string msg = "ErrorVisibilitySignatureProcessDeleteCorr";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                    return;
                }
            }
            catch (Exception ex)
            {
                string msg = "ErrorVisibilitySignatureProcessDeleteCorr";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            try
            {
                string codice = TxtCodeCorr.Text;
                this.TxtCodeCorr.Text = string.Empty;
                this.TxtDescriptionCorr.Text = string.Empty;
                if (!string.IsNullOrEmpty(codice))
                {
                    RubricaCallType calltype = RubricaCallType.CALLTYPE_PROTO_INT_MITT;
                    ElementoRubrica[] listaCorr = null;
                    Corrispondente corr = null;
                    UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
                    listaCorr = UIManager.AddressBookManager.getElementiRubricaMultipli(codice, calltype, true);
                    if (listaCorr != null && (listaCorr.Count() == 1))
                    {
                        if (listaCorr.Count() == 1)
                        {
                            corr = UIManager.AddressBookManager.getCorrispondenteRubrica(codice, calltype);
                        }
                        if (corr == null)
                        {
                            string msg = "ErrorTransmissionCorrespondentNotFound";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        if (!corr.tipoCorrispondente.Equals("R"))
                        {
                            string msg = "WarningCorrespondentAsRole";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        else
                        {
                            this.TxtCodeCorr.Text = corr.codiceRubrica;
                            this.TxtDescriptionCorr.Text = corr.descrizione;
                            this.idCorr.Value = ((DocsPaWR.Ruolo)corr).idGruppo;
                            this.BtnAssegnaVisibilita.Enabled = true;
                            this.UpPnlButtons.Update();
                        }
                    }
                    else
                    {
                        string msg = "ErrorTransmissionCorrespondentNotFound";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                }
                this.UpdPnlCorr.Update();
            }
            catch (Exception ex)
            {
                string msg = "ErrorSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        private List<VisibilitaProcessoRuolo> LoadListaCorr(string idProcesso)
        {
            return UIManager.SignatureProcessesManager.GetVisibilitaProcesso(idProcesso, FiltroRicerca);
        }

        protected string GetDescriptionCorr(Corrispondente corr)
        {
            return corr.descrizione + "(" + corr.codiceRubrica + ")";
        }
    }
}