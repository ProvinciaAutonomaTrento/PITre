using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDatalLibrary;
using NttDataWA.Utils;

namespace NttDataWA.Project
{
    public partial class Events : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
                Fascicolo Prj = UIManager.ProjectManager.getProjectInSession();

                if ((Prj.systemID != null && !string.IsNullOrEmpty(Prj.systemID)) && ProjectManager.CheckRevocationAcl())
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}", true);
                    return;
                }

                if (!IsPostBack)
                {
                    this.InitializeLanguage();
                    this.InitializePage();
                }
                this.ReApplyScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ReApplyScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void InitializePage()
        {
            Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
            this.litDescription.Text = Server.HtmlEncode(fascicolo.descrizione);

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_ENABLE_FILTER_EVENT.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_ENABLE_FILTER_EVENT.ToString()).Equals("1"))
            {
                this.PnlFilterAuthorAction.Visible = true;
            }

            this.Session["logFascicolo"] = string.Empty;
            this.LoadAction();
            this.loadGrid(null);
            this.UpdPanelEvents.Update();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.litDescriptionText.Text = Utils.Languages.GetLabelFromCode("EventsLitDescription", language);
            this.BtnFilter.Text = Utils.Languages.GetLabelFromCode("EventBtnFilter", language);
            this.BtnRemoveFilter.Text = Utils.Languages.GetLabelFromCode("EventBtnRemoveFilter", language);
            this.EventsLblData.Text = Utils.Languages.GetLabelFromCode("EventsLblData", language);
            this.SignatureA4.Title = Utils.Languages.GetLabelFromCode("PopupSignatureA4", language);
            this.opt0.Text = Utils.Languages.GetLabelFromCode("EventsOpt0", language);
            this.opt1.Text = Utils.Languages.GetLabelFromCode("EventsOpt1", language);
            this.opt2.Text = Utils.Languages.GetLabelFromCode("EventsOpt2", language);
            this.opt3.Text = Utils.Languages.GetLabelFromCode("EventsOpt3", language);
            this.opt4.Text = Utils.Languages.GetLabelFromCode("EventsOpt4", language);

            this.GridEvents.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("EventsLblData", language);
            this.GridEvents.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("EventsLblUser", language);
            this.GridEvents.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("EventsLblAction", language);

            this.EventsFrom.Text = Utils.Languages.GetLabelFromCode("EventsOneField", language);
            this.EventsTo.Text = Utils.Languages.GetLabelFromCode("EventsTo", language);

            this.LtlAuthor.Text = Utils.Languages.GetLabelFromCode("VisibilityLtlAuthor", language);
            this.optRole.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptRole", language);
            this.optUser.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptUser", language);
            this.LtlEvent.Text = Utils.Languages.GetLabelFromCode("EventsLblAction", language);
            this.ddlEvent.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("EventDllEvent", language));
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddFilterAddressBookTitle", language);
        }

        /// <summary>
        /// carica il datagrid dei log
        /// </summary>
        /// <param name="filter">se null restituisce tutti i log</param>
        private void loadGrid(FilterVisibility[] filter)
        {
            try
            {
                DocumentoLogDocumento[] logFascicolo = null;

                Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
                Folder folder = UIManager.ProjectManager.getFolder(this, fascicolo);

                logFascicolo = DocumentManager.getStoriaLog(fascicolo.systemID, folder.systemID, "FASCICOLO", filter);


                if (logFascicolo != null)
                {
                    logFascicolo = (from l in logFascicolo
                                    where l.chaEsito.Equals("1")
                                    select l).ToArray<DocumentoLogDocumento>();

                    foreach (DocumentoLogDocumento doclog in logFascicolo)
                    {
                        doclog.dataAzione = doclog.dataAzione.Replace('.', ':');
                        doclog.userIdOperatore = doclog.descProduttore;
                    }
                    GridEvents.DataSource = logFascicolo;
                    Session["logFascicolo"] = logFascicolo;

                    this.GridEvents.PageIndex = 0;
                    this.GridEvents.SelectedIndex = 0;
                    this.GridEvents.DataBind();

                    GridEvents.PageIndex = 0;
                    this.UpdPanelEvents.Update();

                    string language = UIManager.UserManager.GetUserLanguage();
                    this.GridEvents.HeaderRow.Cells[0].Text = Utils.Languages.GetLabelFromCode("EventsLblData", language);
                    this.GridEvents.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("EventsLblUser", language);
                    this.GridEvents.HeaderRow.Cells[2].Text = Utils.Languages.GetLabelFromCode("EventsLblAction", language);
                }
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
       
        protected void BtnFilter_Click(object sender, EventArgs e)
        {
            try {
                caricaRisultati();
                UpdPanelEvents.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DdlDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.DdlDate.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.TxtFrom.ReadOnly = false;
                        this.TxtTo.Visible = false;
                        this.EventsTo.Visible = false;
                        this.EventsFrom.Text = Utils.Languages.GetLabelFromCode("EventsOneField", language);
                        break;
                    case 1: //Intervallo
                        this.TxtFrom.ReadOnly = false;
                        this.TxtTo.ReadOnly = false;
                        this.EventsTo.Visible = true;
                        this.EventsFrom.Visible = true;
                        this.TxtTo.Visible = true;
                        this.EventsFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        break;
                    case 2: //Oggi
                        this.EventsTo.Visible = false;
                        this.TxtTo.Visible = false;
                        this.TxtFrom.ReadOnly = true;
                        this.TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                        break;
                    case 3: //Settimana corrente
                        this.EventsTo.Visible = true;
                        this.TxtTo.Visible = true;
                        this.TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.TxtTo.ReadOnly = true;
                        this.TxtFrom.ReadOnly = true;
                        break;
                    case 4: //Mese corrente
                        this.EventsTo.Visible = true;
                        this.TxtTo.Visible = true;
                        this.TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.TxtTo.ReadOnly = true;
                        this.TxtFrom.ReadOnly = true;
                        break;
                }

                this.UpContainer.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void caricaRisultati()
        {
            try
            {
                List<FilterVisibility> filterArrayVis = new List<FilterVisibility>();
                FilterVisibility vis = null;

                #region DATE
                //Filtro sulla data
                switch (DdlDate.SelectedItem.Value)
                {
                    case "0": //Valore singolo
                        if (!string.IsNullOrEmpty(this.TxtFrom.Text))
                        {
                            vis = new FilterVisibility();
                            vis.Type = FilterVisibility2.DATE;
                            vis.Value = this.TxtFrom.Text;
                            filterArrayVis.Add(vis);
                        }
                        break;

                    case "1": // Intervallo
                        if (!string.IsNullOrEmpty(this.TxtFrom.Text) && !string.IsNullOrEmpty(this.TxtTo.Text))
                        {
                            vis = new FilterVisibility();
                            vis.Type = FilterVisibility2.DATE_FROM;
                            vis.Value = this.TxtFrom.Text;
                            filterArrayVis.Add(vis);
                            vis = new FilterVisibility();
                            vis.Type = FilterVisibility2.DATE_TO;
                            vis.Value = this.TxtTo.Text;
                            filterArrayVis.Add(vis);
                        }
                        break;

                    case "2": // Oggi
                        if (!string.IsNullOrEmpty(this.TxtFrom.Text))
                        {
                            vis = new FilterVisibility();
                            vis.Type = FilterVisibility2.DATE;
                            vis.Value = this.TxtFrom.Text;
                            filterArrayVis.Add(vis);
                        }
                        break;

                    case "3": //Settimana corrente
                        if (!string.IsNullOrEmpty(TxtFrom.Text) && !string.IsNullOrEmpty(this.TxtTo.Text))
                        {
                            vis = new FilterVisibility();
                            vis.Type = FilterVisibility2.DATE_FROM;
                            vis.Value = this.TxtFrom.Text;
                            filterArrayVis.Add(vis);
                            vis = new FilterVisibility();
                            vis.Type = FilterVisibility2.DATE_TO;
                            vis.Value = this.TxtTo.Text;
                            filterArrayVis.Add(vis);
                        }
                        break;

                    case "4": //Mese corrente
                        if (!string.IsNullOrEmpty(this.TxtFrom.Text) && !string.IsNullOrEmpty(this.TxtTo.Text))
                        {
                            vis = new FilterVisibility();
                            vis.Type = FilterVisibility2.DATE_MONTH;
                            filterArrayVis.Add(vis);
                        }
                        break;

                }
                #endregion

                #region AUTORE EVENTO

                if (!string.IsNullOrEmpty(this.txt_codAuthor_E.Text))
                {
                    vis = new FilterVisibility();
                    if (this.rblOwnerType.SelectedValue.Equals("P"))
                        vis.Type = FilterVisibility2.USER;
                    else
                        vis.Type = FilterVisibility2.ROLE;
                    vis.Value = this.IdRecipient.Value;
                    filterArrayVis.Add(vis);
                }

                #endregion

                #region  AZIONE

                if (!this.ddlEvent.SelectedValue.Equals(string.Empty))
                {
                    vis = new FilterVisibility();
                    vis.Type = FilterVisibility2.CAUSE;
                    vis.Value = this.ddlEvent.SelectedValue;
                    filterArrayVis.Add(vis);
                }

                #endregion

                loadGrid(filterArrayVis.ToArray());
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void BtnRemoveFilter_Click(object sender, EventArgs e)
        {
            try {
                string language = UIManager.UserManager.GetUserLanguage();
                this.DdlDate.SelectedIndex = -1;
                this.TxtFrom.Text = string.Empty;
                this.TxtTo.Text = string.Empty;
                this.TxtFrom.ReadOnly = false;
                this.TxtTo.Visible = false;
                this.EventsTo.Visible = false;
                this.EventsFrom.Text = Utils.Languages.GetLabelFromCode("EventsOneField", language);
                this.txt_codAuthor_E.Text = string.Empty;
                this.txt_descrAuthor_E.Text = string.Empty;
                this.IdRecipient.Value = string.Empty;
                this.rblOwnerType.SelectedIndex = -1;
                this.rblOwnerType.Items.FindByValue("R").Selected = true;
                this.ddlEvent.SelectedIndex = 0;
                loadGrid(null);
                UpContainer.Update();
                panelButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GridEvents_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try {
                GridEvents.PageIndex = e.NewPageIndex;
                GridEvents.SelectedIndex = e.NewPageIndex;
                this.GridEvents.DataSource = Session["logFascicolo"];
                this.GridEvents.DataBind();
                UpContainer.Update();
                panelButtons.Update();
                UpdPanelEvents.Update();
                UpUserControlHeaderProject.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DocumentImgRecipientAddressBookAuthor_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                this.CallType = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
                HttpContext.Current.Session["AddressBook.from"] = "T_S_R_S";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpProtocollo", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void txt_author_E_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                CustomTextArea caller = sender as CustomTextArea;
                string codeAddressBook = this.txt_codAuthor_E.Text;

                if (!string.IsNullOrEmpty(codeAddressBook))
                {
                    this.SearchCorrespondent(codeAddressBook, caller.ID);
                }
                else
                {
                    this.txt_codAuthor_E.Text = string.Empty;
                    this.txt_descrAuthor_E.Text = string.Empty;
                    this.IdRecipient.Value = string.Empty;
                    this.UpPnlAuthor.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchCorrespondent(string addressCode, string idControl)
        {
            RubricaCallType calltype = RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
            DocsPaWR.Corrispondente corr = UIManager.AddressBookManager.getCorrispondenteRubrica(addressCode, calltype);

            if (corr == null)
            {
                this.txt_codAuthor_E.Text = string.Empty;
                this.txt_descrAuthor_E.Text = string.Empty;
                this.IdRecipient.Value = string.Empty;
                this.UpPnlAuthor.Update();
            }
            else
            {
                this.txt_codAuthor_E.Text = corr.codiceRubrica;
                this.txt_descrAuthor_E.Text = corr.descrizione;
                this.IdRecipient.Value = corr.systemId;
                this.rblOwnerType.SelectedIndex = -1;
                this.rblOwnerType.Items.FindByValue(corr.tipoCorrispondente).Selected = true;
                this.UpPnlAuthor.Update();
            }
        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try
            {
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> ccList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.Cc"];
                string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();

                if (atList != null && atList.Count > 0)
                {
                    NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                    Corrispondente tempCorrSingle;
                    if (!corrInSess.isRubricaComune)
                        tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                    else
                        tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                    this.txt_codAuthor_E.Text = tempCorrSingle.codiceRubrica;
                    this.txt_descrAuthor_E.Text = tempCorrSingle.descrizione;
                    this.IdRecipient.Value = tempCorrSingle.systemId;
                    this.rblOwnerType.SelectedIndex = -1;
                    this.rblOwnerType.Items.FindByValue(tempCorrSingle.tipoCorrispondente).Selected = true;
                    this.UpPnlAuthor.Update();
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

        private void LoadAction()
        {
            List<infoOggetto> listAction = DocumentManager.GetLogAttiviByOggetto("FASCICOLO");
            this.ddlEvent.Items.Add(new ListItem() { Value = string.Empty, Text = string.Empty });

            if (listAction != null && listAction.Count > 0)
            {
                foreach (DocsPaWR.infoOggetto action in listAction)
                {
                    ListItem li = new ListItem();
                    li.Value = action.Codice;
                    li.Text = action.Descrizione;
                    this.ddlEvent.Items.Add(li);
                }
            }
        }
    }
}