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

namespace NttDataWA.Document
{
    public partial class Events : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
                if (!DocumentManager.IsNewDocument() && DocumentManager.CheckRevocationAcl())
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
                this.SetAjaxAddressBook();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
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

            string callType = "CALLTYPE_CORR_INT_NO_UO";
            this.RapidRecipient.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
        }
        private void ReApplyScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void InitializePage()
        {
            SchedaDocumento doc = DocumentManager.getSelectedRecord();
            this.litObject.Text = doc.oggetto.descrizione;

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_ENABLE_FILTER_EVENT.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_ENABLE_FILTER_EVENT.ToString()).Equals("1"))
            {
                this.PnlFilterAuthorAction.Visible = true;
            }

            if (doc.tipoProto.Equals("A"))
            {
                this.container.Attributes.Add("class", "borderOrange");
                this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabOrangeDxBorder");
                this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabOrange");

            }
            else
            {
                if (doc.tipoProto.Equals("P"))
                {
                    this.container.Attributes.Add("class", "borderGreen");

                    this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabGreenDxBorder");
                    this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabGreen");

                }
                else
                {
                    if (doc.tipoProto.Equals("I"))
                    {

                        this.container.Attributes.Add("class", "borderBlue");
                        this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabBlueDxBorder");
                        this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabBlue");

                    }
                    else
                    {
                        this.container.Attributes.Add("class", "borderGrey");
                        this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabGreyDxBorder");
                        this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabGrey");
                    }
                }
            }

            this.Session["logDocumento"] = string.Empty;
            this.LoadAction();
            this.loadGrid(null);
            this.UpdPanelVisibility.Update();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.litSubject.Text = Utils.Languages.GetLabelFromCode("VisibilitySubject", language);
            this.BtnFilter.Text = Utils.Languages.GetLabelFromCode("EventBtnFilter", language);
            this.BtnRemoveFilter.Text = Utils.Languages.GetLabelFromCode("EventBtnRemoveFilter", language);
            this.EventsLblData.Text = Utils.Languages.GetLabelFromCode("EventsLblData", language);
            this.SignatureA4.Title = Utils.Languages.GetLabelFromCode("PopupSignatureA4", language);
            this.opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);

            this.GridEvents.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("EventsLblData", language);
            this.GridEvents.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("EventsLblUser", language);
            this.GridEvents.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("EventsLblAction", language);

            this.VisibilityFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.VisibilityTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
            this.LtlAuthor.Text = Utils.Languages.GetLabelFromCode("VisibilityLtlAuthor", language);
            this.optRole.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptRole", language);
            this.optUser.Text = Utils.Languages.GetLabelFromCode("SearchProjectOwnerTypeOptUser", language);
            this.LtlEvent.Text = Utils.Languages.GetLabelFromCode("EventsLblAction", language);
            this.ddlEvent.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("EventDllEvent", language));
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddFilterAddressBookTitle", language);
            this.PrintLabel.Title = Utils.Languages.GetLabelFromCode("PrintLabelPopUpTitle", language);
        }

        /// <summary>
        /// carica il datagrid dei log
        /// </summary>
        /// <param name="filter">se null restituisce tutti i log</param>
        private void loadGrid(FilterVisibility[] filter)
        {
            DocumentoLogDocumento[] logDocumento = null;
            string oggetto = (UIManager.DocumentManager.getSelectedRecord() != null && UIManager.DocumentManager.getSelectedRecord().documentoPrincipale != null) ? "ALLEGATO" : "DOCUMENTO";
            if (filter == null)
                logDocumento = DocumentManager.getStoriaLog(UIManager.DocumentManager.getSelectedRecord().systemId, oggetto);
            else
                logDocumento = DocumentManager.getStoriaLog(DocumentManager.getSelectedRecord().systemId, "", oggetto, filter);

            if (logDocumento != null)
            {
                logDocumento = (from l in logDocumento
                                where l.chaEsito.Equals("1")
                                select l).ToArray<DocumentoLogDocumento>();

                foreach (DocumentoLogDocumento doclog in logDocumento)
                {
                    doclog.dataAzione = doclog.dataAzione.Replace('.', ':');
                    doclog.userIdOperatore = doclog.descProduttore;
                }
                this.GridEvents.DataSource = logDocumento;
                this.Session["logDocumento"] = logDocumento;
                
                this.GridEvents.PageIndex = 0;
                this.GridEvents.SelectedIndex = 0;
                this.GridEvents.DataBind();
                
                this.UpdPanelVisibility.Update();

                string language = UIManager.UserManager.GetUserLanguage();
                this.GridEvents.HeaderRow.Cells[0].Text = Utils.Languages.GetLabelFromCode("EventsLblData", language);
                this.GridEvents.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("EventsLblUser", language);
                this.GridEvents.HeaderRow.Cells[2].Text = Utils.Languages.GetLabelFromCode("EventsLblAction", language);
            }
        }

        protected void BtnFilter_Click(object sender, EventArgs e)
        {
            try {
                this.caricaRisultati();
                this.UpdPanelVisibility.Update();
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
                        this.VisibilityTo.Visible = false;
                        this.VisibilityFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.TxtFrom.ReadOnly = false;
                        this.TxtTo.ReadOnly = false;
                        this.VisibilityTo.Visible = true;
                        this.VisibilityFrom.Visible = true;
                        this.TxtTo.Visible = true;
                        this.VisibilityFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        break;
                    case 2: //Oggi
                        this.VisibilityTo.Visible = false;
                        this.TxtTo.Visible = false;
                        this.TxtFrom.ReadOnly = true;
                        this.TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                        break;
                    case 3: //Settimana corrente
                        this.VisibilityTo.Visible = true;
                        this.TxtTo.Visible = true;
                        this.TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.TxtTo.ReadOnly = true;
                        this.TxtFrom.ReadOnly = true;
                        break;
                    case 4: //Mese corrente
                        this.VisibilityTo.Visible = true;
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
            List<FilterVisibility> filterArrayVis = new List<FilterVisibility>();
            FilterVisibility vis = null;

            #region DATA
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
            #endregion;

            #region AUTORE EVENTO

            if (!string.IsNullOrEmpty(this.txt_codAuthor_E.Text))
            {
                vis = new FilterVisibility();
                if(this.rblOwnerType.SelectedValue.Equals("P"))
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

        protected void BtnRemoveFilter_Click(object sender, EventArgs e)
        {
            try {
                string language = UIManager.UserManager.GetUserLanguage();
                this.DdlDate.SelectedIndex = -1;
                this.TxtFrom.Text = string.Empty;
                this.TxtTo.Text = string.Empty;
                this.TxtFrom.ReadOnly = false;
                this.TxtTo.Visible = false;
                this.VisibilityTo.Visible = false;
                this.VisibilityFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
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
                this.GridEvents.DataSource = Session["logDocumento"];
                this.GridEvents.DataBind();
                UpContainer.Update();
                panelButtons.Update();
                UpdPanelVisibility.Update();
                UpcontainerDocumentTabLeftBorder.Update();
                UpUserControlHeaderDocument.Update();
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
            List<infoOggetto> listAction = DocumentManager.GetLogAttiviByOggetto("DOCUMENTO");
            this.ddlEvent.Items.Add(new ListItem() { Value = string.Empty, Text = string.Empty });

            if (listAction != null && listAction.Count > 0)
            {
                foreach (DocsPaWR.infoOggetto action in listAction)
                {
                    ListItem li = new ListItem();
                    li.Value =action.Codice;
                    li.Text = action.Descrizione;
                    this.ddlEvent.Items.Add(li);
                }
            }
        }
    }
}