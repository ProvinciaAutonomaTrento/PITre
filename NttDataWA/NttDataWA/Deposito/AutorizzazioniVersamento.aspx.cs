using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDatalLibrary;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using System.Collections;

namespace NttDataWA.Deposito
{
    public partial class AutorizzazioniVersamento : System.Web.UI.Page
    {
        protected int Autorizzazione_system_id
        {
            get
            {
                int result = 0;

                if (HttpContext.Current.Session["Autorizzazione_system_id"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["Autorizzazione_system_id"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["Autorizzazione_system_id"] = value;
            }
        }

        #region  ARCHIVE_AUTH_Authorization Original in Context

        protected List<ARCHIVE_AUTH_Authorization> TransferAuthorizationOriginalInContext
        {
            get
            {
                List<ARCHIVE_AUTH_Authorization> result = null;
                if (HttpContext.Current.Session["TransferAuthorizationOriginalInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferAuthorizationOriginalInContext"] as List<ARCHIVE_AUTH_Authorization>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferAuthorizationOriginalInContext"] = value;
            }
        }

        protected List<ARCHIVE_AUTH_Authorization> TransferAuthorizationInContext
        {
            get
            {
                List<ARCHIVE_AUTH_Authorization> result = null;
                if (HttpContext.Current.Session["TransferAuthorizationInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferAuthorizationInContext"] as List<ARCHIVE_AUTH_Authorization>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferAuthorizationInContext"] = value;
            }
        }

        protected List<ARCHIVE_AUTH_AuthorizedObject> TransferAuthorizedObjectOriginalInContext
        {
            get
            {
                List<ARCHIVE_AUTH_AuthorizedObject> result = null;
                if (HttpContext.Current.Session["TransferAuthorizedObjectOriginalInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferAuthorizedObjectOriginalInContext"] as List<ARCHIVE_AUTH_AuthorizedObject>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferAuthorizedObjectOriginalInContext"] = value;
            }
        }

        protected List<ARCHIVE_AUTH_AuthorizedObject> TransferAuthorizedObjectInContext
        {
            get
            {
                List<ARCHIVE_AUTH_AuthorizedObject> result = null;
                if (HttpContext.Current.Session["TransferAuthorizedObjectInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferAuthorizedObjectInContext"] as List<ARCHIVE_AUTH_AuthorizedObject>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferAuthorizedObjectInContext"] = value;
            }
        }

        protected List<DocsPaWR.ARCHIVE_AUTH_grid_project> TransferProjectGridInContext
        {
            get
            {
                List<ARCHIVE_AUTH_grid_project> result = null;
                if (HttpContext.Current.Session["TransferProjectGridInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferProjectGridInContext"] as List<ARCHIVE_AUTH_grid_project>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferProjectGridInContext"] = value;
            }
        }

        protected List<DocsPaWR.ARCHIVE_AUTH_grid_document> TransferDocumentGridInContext
        {
            get
            {
                List<ARCHIVE_AUTH_grid_document> result = null;
                if (HttpContext.Current.Session["TransferDocumentGridInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferDocumentGridInContext"] as List<ARCHIVE_AUTH_grid_document>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferDocumentGridInContext"] = value;
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
                this.RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public string SetDataChiusura(object date)
        {
            if (date == null || date.ToString() == DateTime.MinValue.ToString())
            {
                return " ";
            }
            else
                return date.ToString();
        }

        private void ReadRetValueFromPopup()
        {
            //RETURN CONFIRM POPUP
            throw new NotImplementedException();
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OnlyNumbers", "OnlyNumbers();", true);
        }

        public enum PageState
        {
            NEW,
            MOD,
            SEA
        }

        public RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_DEP_OSITO;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }

        }

        private void InitializePage()
        {
            this.InitializeLanguage();

            this.InitializeDate();

            //DA CAMBIARE!!!!!
            if (Request.QueryString["PAGESTATE"] != null && Request.QueryString["PAGESTATE"].ToUpper() == "NEW")
            {
                ResetMySession();
                Session["PAGESTATE"] = "NEW";
                this.CallVisibilityByPageState();
                this.GetDataByPageState();
            }

            if (Session["PAGESTATE"] != null && Session["PAGESTATE"] != "SEA")
            {
                this.Autorizzazione_system_id = Convert.ToInt32(Session["ID_AUTORIZZAZIONE"]);
                this.CallVisibilityByPageState();
                this.GetDataByPageState();
            }

            if (Session["PAGESTATE"] != null && Session["PAGESTATE"] == "SEA")
            {
                this.Autorizzazione_system_id = Convert.ToInt32(Session["ID_AUTORIZZAZIONE"]);
                ResetControlPage();
                this.CallVisibilityByPageState();
                this.GetDataByPageState();
            }
        }

        private void InitializeDate()
        {
            this.ddl_dtaDecorrenza.SelectedIndex = 0;
            this.ddl_dtaDecorrenza_SelectedIndexChanged(null, null);
            this.lbl_dtaDecorrenzaFrom.Text = string.Empty;

            this.ddl_dtaScadenza.SelectedIndex = 0;
            this.ddl_dtaScadenza_SelectedIndexChanged(null, null);
            this.lbl_dtaScadenzaFrom.Text = string.Empty;

            // LoadDataGridDummy();
        }

        private void LoadDataGridDummy()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Caller Hub per il change della visibilità degli oggetti.
        /// </summary>
        /// <param name="p"></param>
        private void CallVisibilityByPageState()
        {

            if (Session["PAGESTATE"] == null)
            {
                VisibilityByPageState((PageState)Enum.Parse(typeof(PageState), "NEW"));
            }

            else
                VisibilityByPageState((PageState)Enum.Parse(typeof(PageState), Session["PAGESTATE"].ToString().ToUpper()));
        }

        /// <summary>
        /// Set the objects visibility by page state
        /// Page state:
        /// enum QPageState
        /// N= NEW
        /// M= MOD
        /// </summary>
        /// <param name="querystrig"></param>
        private void VisibilityByPageState(PageState state)
        {
            //Comune.
            TxtIdAutorizzazione.Enabled = false;
            switch (state)
            {
                //Page state new
                case (PageState.NEW):
                    //Pulsanti
                    btnClearFilterUserInAuth.Enabled = false;
                    btnEditUserInAuth.Enabled = false;
                    btnSearchDoc.Enabled = true;
                    btnSearchPrj.Enabled = true;
                    btnUserInAuthNuovo.Enabled = true;
                    //TextBox
                    txtCodiceUserInAuth.Enabled = true;
                    txtDescrizioneUserInAuth.Enabled = true;
                    TxtNoteUserInAuth.Enabled = true;
                    break;

                //Page state Modifiy
                case (PageState.MOD):
                    //Pulsanti
                    btnClearFilterUserInAuth.Enabled = false;
                    btnEditUserInAuth.Enabled = true;
                    btnSearchDoc.Enabled = true;
                    btnSearchPrj.Enabled = true;
                    btnUserInAuthNuovo.Enabled = false;
                    //Textbox
                    txtCodiceUserInAuth.Enabled = true;
                    txtDescrizioneUserInAuth.Enabled = true;
                    TxtNoteUserInAuth.Enabled = true;
                    break;
                case (PageState.SEA):
                    //Pulsanti
                    btnClearFilterUserInAuth.Enabled = false;
                    btnEditUserInAuth.Enabled = true;
                    btnSearchDoc.Enabled = true;
                    btnSearchPrj.Enabled = true;
                    btnUserInAuthNuovo.Enabled = false;
                    //Textbox
                    txtCodiceUserInAuth.Enabled = true;
                    txtDescrizioneUserInAuth.Enabled = true;
                    TxtNoteUserInAuth.Enabled = true;
                    break;

            }
        }

        private void GetDataByPageState()
        {
            if (this.Autorizzazione_system_id != 0)
            {
                //Autorizzazioni.
                this.TransferAuthorizationInContext = UIManager.ArchiveManager.GetALLARCHIVE_Autorizations().Where(x => x.System_ID == Autorizzazione_system_id).ToList();
                this.TransferAuthorizationOriginalInContext = UIManager.ArchiveManager.GetALLARCHIVE_Autorizations().Where(x => x.System_ID == Autorizzazione_system_id).ToList();
                //Oggetti collegati.
                this.TransferAuthorizedObjectOriginalInContext = UIManager.ArchiveManager.GetARCHIVE_AutorizedObjectBySystem_ID(Autorizzazione_system_id);
                this.TransferAuthorizedObjectInContext = UIManager.ArchiveManager.GetARCHIVE_AutorizedObjectBySystem_ID(Autorizzazione_system_id);
                //VISTE:
                if (TransferAuthorizedObjectInContext != null)
                {
                    AllineaProjectInContext(TransferProjectGridInContext, TransferAuthorizedObjectInContext);
                    AllineaDocumentInContext(TransferDocumentGridInContext, TransferAuthorizedObjectInContext);
                }

                TxtIdAutorizzazione.Text = TransferAuthorizationInContext[0].System_ID.ToString();
                txtDescrizioneUserInAuth.Text = TransferAuthorizationInContext[0].Note;

                dtaDecorrenza_TxtFrom.Text = Convert.ToDateTime(TransferAuthorizationInContext[0].StartDate).ToShortDateString();
                dtaScadenza_TxtFrom.Text = Convert.ToDateTime(TransferAuthorizationInContext[0].EndDate).ToShortDateString();

                Utente _myut = new Utente();
                _myut = UIManager.UserManager.GetUtenteByIdPeople(TransferAuthorizationInContext.Where(x => x.System_ID == this.Autorizzazione_system_id).FirstOrDefault().People_ID.ToString());
                txtCodiceUserInAuth.Text = _myut.userId;
                txtDescrizioneUserInAuth.Text = _myut.descrizione;
                this.idUserInAuth.Value = ((NttDataWA.DocsPaWR.Utente)(_myut)).idPeople;
            }

            this.LoadDataGrid();

        }

        private void ResetControlPage()
        {
            txtCodiceUserInAuth.Text = string.Empty;
            txtDescrizioneUserInAuth.Text = string.Empty;
            TxtIdAutorizzazione.Text = string.Empty;
            TxtNoteUserInAuth.Text = string.Empty;
            TransferAuthorizationInContext = new List<ARCHIVE_AUTH_Authorization>();
            TransferAuthorizationOriginalInContext = new List<ARCHIVE_AUTH_Authorization>();
            TransferAuthorizedObjectOriginalInContext = new List<ARCHIVE_AUTH_AuthorizedObject>();
            TransferAuthorizedObjectInContext = new List<ARCHIVE_AUTH_AuthorizedObject>();
            TransferProjectGridInContext = new List<ARCHIVE_AUTH_grid_project>();
            TransferDocumentGridInContext = new List<ARCHIVE_AUTH_grid_document>();
        }

        private void LoadDataGrid()
        {
            if (TransferAuthorizedObjectInContext != null)
            {
                //Bind delle griglie:
                var queryFasc = TransferProjectGridInContext;
                var queryDoc = TransferDocumentGridInContext;

                if (queryDoc != null && queryDoc.Count() > 0)
                {
                    gvDocumentInAUTH.DataSource = queryDoc.ToList();
                    gvDocumentInAUTH.DataBind();
                }
                else
                {
                    gvDocumentInAUTH.DataSource = GetDatasourceDummygvDocument();
                    gvDocumentInAUTH.DataBind();
                    gvDocumentInAUTH.Rows[0].Visible = false;
                }

                if (queryFasc != null && queryFasc.Count() > 0)
                {
                    gvProjectInAUTH.DataSource = queryFasc.ToList();
                    gvProjectInAUTH.DataBind();
                }
                else
                {
                    gvProjectInAUTH.DataSource = GetDatasourceDummygvProject();
                    gvProjectInAUTH.DataBind();
                    gvProjectInAUTH.Rows[0].Visible = false;
                }
            }
            else
            {
                gvDocumentInAUTH.DataSource = GetDatasourceDummygvDocument();
                gvDocumentInAUTH.DataBind();
                gvDocumentInAUTH.Rows[0].Visible = false;

                gvProjectInAUTH.DataSource = GetDatasourceDummygvProject();
                gvProjectInAUTH.DataBind();
                gvProjectInAUTH.Rows[0].Visible = false;
            }
        }

        private object GetDatasourceDummygvProject()
        {
            List<DocsPaWR.ARCHIVE_AUTH_grid_project> _dsDummylst = new List<DocsPaWR.ARCHIVE_AUTH_grid_project>();
            ARCHIVE_AUTH_grid_project _dsDummy = new ARCHIVE_AUTH_grid_project();
            _dsDummy.System_ID = 0;
            _dsDummy.Registro = string.Empty;
            _dsDummy.Codice = string.Empty;
            _dsDummy.Descrizione = string.Empty;
            _dsDummy.DataApertura = DateTime.Now;
            _dsDummy.DataChiusura = DateTime.Now;
            _dsDummylst.Add(_dsDummy);
            return _dsDummylst;
        }

        private object GetDatasourceDummygvDocument()
        {
            List<DocsPaWR.ARCHIVE_AUTH_grid_document> _dsDummylst = new List<ARCHIVE_AUTH_grid_document>();
            ARCHIVE_AUTH_grid_document _dsDummy = new ARCHIVE_AUTH_grid_document();
            _dsDummy.System_ID = 0;
            _dsDummy.Registro = string.Empty;
            _dsDummy.Tipo = string.Empty;
            _dsDummy.Oggetto = string.Empty;
            _dsDummy.Mittente_Destinatario = string.Empty;
            _dsDummy.ID_Protocollo = string.Empty;
            _dsDummy.Data = DateTime.Now;
            _dsDummylst.Add(_dsDummy);
            return _dsDummylst;
        }

        private void ResetMySession()
        {
            //RESET DELLA SESSIONE
            this.Session["ID_AUTORIZZAZIONE"] = null;
            Autorizzazione_system_id = 0;
            this.Session["PAGESTATE"] = null;
            this.idUserInAuth.Value = "";
            ResetControlPage();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.LitProjectInAuth.Text = Utils.Languages.GetLabelFromCode("LitProjectInAuth", language);
            this.LitDocumentInAuth.Text = Utils.Languages.GetLabelFromCode("LitDocumentInAuth", language);
            this.lit_dtaDecorrenza.Text = Utils.Languages.GetLabelFromCode("lit_dtaDecorrenza", language);
            this.lit_dtaScadenza.Text = Utils.Languages.GetLabelFromCode("lit_dtaScadenza", language);
            this.litExpandUserInAuth.Text = Utils.Languages.GetLabelFromCode("litExpandUserInAuth", language);
            this.LitUserInAuthNote.Text = Utils.Languages.GetLabelFromCode("LitUserInAuthNote", language);
            this.litUserInAuth.Text = Utils.Languages.GetLabelFromCode("litUserInAuth", language);
            this.LitAutorizzazioniVersamento.Text = Utils.Languages.GetLabelFromCode("LitAutorizzazioniVersamento", language);
            this.LitAutorizzazioneId.Text = Utils.Languages.GetLabelFromCode("LitAutorizzazioneId", language);
            this.btnUserInAuthNuovo.Text = Utils.Languages.GetLabelFromCode("btnUserInAuthNuovo", language);
            this.btnSearchDoc.Text = Utils.Languages.GetLabelFromCode("btnSearchDoc", language);
            this.btnSearchPrj.Text = Utils.Languages.GetLabelFromCode("btnSearchPrj", language);
            this.btnEditUserInAuth.Text = Utils.Languages.GetLabelFromCode("btnEditUserInAuth", language);
            this.btnClearFilterUserInAuth.Text = Utils.Languages.GetLabelFromCode("btnClearFilterUserInAuth", language);
            //Data Creazione
            this.dtaDecorrenza_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaDecorrenza_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);

            //Data Analisi completata
            this.dtaScadenza_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaScadenza_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);

            this.LitFiltriAuth.Text = Utils.Languages.GetLabelFromCode("LitFiltriAuth", language);

        }

        protected void ddl_dtaDecorrenza_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dtaDecorrenza.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.dtaDecorrenza_TxtFrom.ReadOnly = false;
                        this.dtaDecorrenza_TxtTo.Visible = false;
                        this.lbl_dtaDecorrenzaTo.Visible = false;
                        this.lbl_dtaDecorrenzaFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Oggi
                        this.lbl_dtaDecorrenzaTo.Visible = false;
                        this.dtaDecorrenza_TxtTo.Visible = false;
                        this.dtaDecorrenza_TxtFrom.ReadOnly = true;
                        this.dtaDecorrenza_TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                        this.lbl_dtaDecorrenzaFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        this.lbl_dtaDecorrenzaTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
                        break;
                }

                this.upValidityTime.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dtaScadenza_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.ddl_dtaScadenza.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.dtaScadenza_TxtFrom.ReadOnly = false;
                        this.dtaScadenza_TxtTo.Visible = false;
                        this.lbl_dtaScadenzaTo.Visible = false;
                        this.lbl_dtaScadenzaFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;

                    case 1: //Oggi
                        this.lbl_dtaScadenzaTo.Visible = false;
                        this.dtaScadenza_TxtTo.Visible = false;
                        this.dtaScadenza_TxtFrom.ReadOnly = true;
                        this.dtaScadenza_TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                        break;

                }

                this.upValidityTime.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                CustomTextArea caller = sender as CustomTextArea;
                string codeAddressBook = caller.Text;

                if (!string.IsNullOrEmpty(codeAddressBook))
                {
                    this.SearchCorrespondent(codeAddressBook, caller.ID);
                }
                else
                {
                    switch (caller.ID)
                    {
                        case "txtCodiceUserInAuth":
                            this.txtCodiceUserInAuth.Text = string.Empty;
                            this.txtDescrizioneUserInAuth.Text = string.Empty;
                            this.idUserInAuth.Value = string.Empty;
                            this.upUtenteDetails.Update();
                            break;
                    }
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
            DocsPaWR.Corrispondente corr = UIManager.AddressBookManager.getCorrispondenteRubrica(addressCode, this.CallType);
            if (corr == null)
            {
                switch (idControl)
                {
                    case "txtCodiceUserInAuth":
                        this.txtCodiceUserInAuth.Text = string.Empty;
                        this.txtDescrizioneUserInAuth.Text = string.Empty;
                        this.idUserInAuth.Value = string.Empty;
                        this.upUtenteDetails.Update();
                        break;
                }

                string msg = "ErrorTransmissionCorrespondentNotFound";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
            }
            else
            {
                switch (idControl)
                {
                    case "txtCodiceUserInAuth":
                        this.txtCodiceUserInAuth.Text = corr.codiceRubrica;
                        this.txtDescrizioneUserInAuth.Text = corr.descrizione;
                        this.idUserInAuth.Value = corr.systemId;
                        this.upUtenteDetails.Update();
                        // this.rblOwnerType.SelectedIndex = -1;
                        // this.rblOwnerType.Items.FindByValue(corr.tipoCorrispondente).Selected = true;
                        // this.upPnlCreatore.Update();
                        break;

                }
            }
        }

        protected void gvDocumentInAuthPageIndexChanging_click(object sender, GridViewPageEventArgs e)
        {
            //this..DataSource = this.TransferPolicyViewProjectsPolicyInContext.ToList();
            //this.gvProjectInPolicyTransfer.DataBind();
            //this.gvProjectInPolicyTransfer.PageIndex = e.NewPageIndex;
            //this.gvProjectInPolicyTransfer.DataBind();
            //this.upTransferProject.Update();
        }

        protected void gvProjectInAuthPageIndexChanging_click(object sender, GridViewPageEventArgs e)
        {
            //this..DataSource = this.TransferPolicyViewProjectsPolicyInContext.ToList();
            //this.gvProjectInPolicyTransfer.DataBind();
            //this.gvProjectInPolicyTransfer.PageIndex = e.NewPageIndex;
            //this.gvProjectInPolicyTransfer.DataBind();
            //this.upTransferProject.Update();
        }

        protected void btnUserInAuthNuovo_Click(object sender, EventArgs e)
        {
            if (this.TransferAuthorizedObjectInContext.Count > 0)
            {
                //Procedi
                if (ValidateControlPage())
                {
                    //GET DEI DATI:
                    //La faccio facile:
                    //Prima devo verificare se l'utente ne ha già un'altra e poi devo segnalarlo:
                    List<ARCHIVE_AUTH_Authorization> _lstControl = UIManager.ArchiveManager.GetALLARCHIVE_Autorizations();
                    if (_lstControl.Where(x => x.People_ID == Convert.ToInt32(this.idUserInAuth.Value)).Where(x => x.EndDate >= DateTime.Now).Count() > 0)
                    {
                        //Ho già un record buono , non posso farne un'altro:
                        string msgConfirm = "Autorizzazioni_InUser";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgConfirm.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgConfirm.Replace("'", @"\'") + "', 'error', '');}", true);
                    }
                    else
                    {
                        Int32 Idreturn = 0;
                        UIManager.ArchiveManager.InsertARCHIVE_Authorization(Convert.ToInt32(this.idUserInAuth.Value),
                                                                                               Convert.ToDateTime(dtaDecorrenza_TxtFrom.Text.Trim()),
                                                                                               Convert.ToDateTime(dtaScadenza_TxtFrom.Text.Trim()),
                                                                                               TxtNoteUserInAuth.Text.Trim(),
                                                                                               GetSqlConcatenate(TransferDocumentGridInContext),
                                                                                               GetSqlConcatenate(TransferProjectGridInContext),
                                                                                               ref Idreturn);
                        if (Idreturn > 0)
                        {
                            // E' andata bene, comunico e poi passo lo stato in modifica:
                            //Setto lo stato a MOD
                            Session["PAGESTATE"] = "MOD";
                            Session["ID_AUTORIZZAZIONE"] = Idreturn;
                            TxtIdAutorizzazione.Text = Idreturn.ToString();
                            this.GetDataByPageState();
                            CallVisibilityByPageState();
                            upUtenteDetails.Update();
                            upValidityTime.Update();
                            upDocumentInAUTH.Update();
                            upProjectInAuth.Update();
                            UpTranfetButtons.Update();
                            ////Message
                            //string msgConfirm = "InserAuthorizationInMerge";
                            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgConfirm.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgConfirm.Replace("'", @"\'") + "', 'error', '');}", true);
                            ////EDDAJE!
                        }
                    }
                }
            }
            else
            {
                string msgConfirm = "btnUserInAuthNuovo_Click";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgConfirm.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgConfirm.Replace("'", @"\'") + "', 'error', '');}", true);
            }

        }

        private string GetSqlConcatenate(List<ARCHIVE_AUTH_grid_project> TransferProjectGridInContext)
        {
            int brk = 0;
            string sqlIn = string.Empty;

            foreach (ARCHIVE_AUTH_grid_project _pol in TransferProjectGridInContext)
            {
                if (brk == 0)
                {
                    sqlIn = "" + _pol.System_ID;
                    brk++;
                }
                else
                    sqlIn += "," + _pol.System_ID;
            }
            sqlIn += "";

            return sqlIn;
        }

        private string GetSqlConcatenate(List<ARCHIVE_AUTH_grid_document> TransferDocumentGridInContext)
        {
            int brk = 0;
            string sqlIn = string.Empty;

            foreach (ARCHIVE_AUTH_grid_document _pol in TransferDocumentGridInContext)
            {
                if (brk == 0)
                {
                    sqlIn = "" + _pol.System_ID;
                    brk++;
                }
                else
                    sqlIn += "," + _pol.System_ID;
            }
            sqlIn += "";

            return sqlIn;
        }

        private bool ValidateControlPage()
        {
            //Controllo prima le textbox
            if (string.IsNullOrEmpty(txtCodiceUserInAuth.Text.Trim()) || string.IsNullOrEmpty(txtDescrizioneUserInAuth.Text)
                || string.IsNullOrEmpty(dtaDecorrenza_TxtFrom.Text) || string.IsNullOrEmpty(dtaScadenza_TxtFrom.Text))
            {
                string msgErrNewAUTHTransfer = "msgErrNewAUTHTransfer";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgErrNewAUTHTransfer.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgErrNewAUTHTransfer.Replace("'", @"\'") + "', 'error', '');}", true);
                return false;
            }

            if (Convert.ToDateTime(dtaScadenza_TxtFrom.Text.Trim()) <= Convert.ToDateTime(dtaDecorrenza_TxtFrom.Text.Trim()))
            {
                string msgErrNewAUTHTransferDate = "msgErrNewAUTHTransferDate";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgErrNewAUTHTransferDate.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgErrNewAUTHTransferDate.Replace("'", @"\'") + "', 'error', '');}", true);
                return false;
            }
            return true;
        }

        protected void btnSearchDoc_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenAddDoc", "ajaxModalPopupOpenAddDoc();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnSearchPrj_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "SearchProject", "ajaxModalPopupSearchProject();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnEditUserInAuth_Click(object sender, EventArgs e)
        {
            if (this.TransferAuthorizedObjectInContext.Count > 0)
            {
                //Procedi
                {
                    Int32 Idreturn = this.Autorizzazione_system_id;
                    UIManager.ArchiveManager.UpdateARCHIVE_Authorization(Convert.ToInt32(this.idUserInAuth.Value),
                                                                                           Convert.ToDateTime(dtaDecorrenza_TxtFrom.Text.Trim()),
                                                                                           Convert.ToDateTime(dtaScadenza_TxtFrom.Text.Trim()),
                                                                                           TxtNoteUserInAuth.Text.Trim(),
                                                                                           GetSqlConcatenate(TransferDocumentGridInContext),
                                                                                           GetSqlConcatenate(TransferProjectGridInContext),
                                                                                           ref Idreturn);
                    if (Idreturn > 0)
                    {
                        // E' andata bene, comunico e poi passo lo stato in modifica:
                        //Setto lo stato a MOD
                        Session["PAGESTATE"] = "MOD";
                        Session["ID_AUTORIZZAZIONE"] = Idreturn;
                        TxtIdAutorizzazione.Text = Idreturn.ToString();
                        //this.GetDataByPageState();
                        CallVisibilityByPageState();
                        upUtenteDetails.Update();
                        upValidityTime.Update();
                        upDocumentInAUTH.Update();
                        upProjectInAuth.Update();
                        UpTranfetButtons.Update();
                        ////Message
                        //string msgConfirm = "InserAuthorizationInMerge";
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgConfirm.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgConfirm.Replace("'", @"\'") + "', 'error', '');}", true);
                        ////EDDAJE!
                    }

                }
            }
            else
            {
                string msgConfirm = "btnUserInAuthNuovo_Click";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgConfirm.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgConfirm.Replace("'", @"\'") + "', 'error', '');}", true);
            }
        }

        protected void btnClearFilterUserInAuth_Click(object sender, EventArgs e)
        {

        }

        protected void ImgUserInAuthAddressBook_Click(object sender, EventArgs e)
        {
            try
            {
                this.CallType = RubricaCallType.CALLTYPE_DEP_OSITO;
                HttpContext.Current.Session["AddressBook.from"] = "F_X_X_S";
                HttpContext.Current.Session["AddressBook.EnableOnly"] = "P";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AddressBook", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
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

                    this.txtCodiceUserInAuth.Text = tempCorrSingle.codiceRubrica;
                    this.txtDescrizioneUserInAuth.Text = tempCorrSingle.descrizione;
                    this.idUserInAuth.Value = ((NttDataWA.DocsPaWR.Utente)(tempCorrSingle)).idPeople;
                    this.upUtenteDetails.Update();
                }


                HttpContext.Current.Session["AddressBook.At"] = null;
                HttpContext.Current.Session["AddressBook.Cc"] = null;
            }
            catch (System.Exception ex)
            {
                //UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnDocumentPostback_Click(object sender, EventArgs e)
        {
            if (Session["DocInSESSION"] != null)
            {
                AllineaDocumentInContext((List<DocsPaWR.ARCHIVE_AUTH_grid_document>)Session["DocInSESSION"], this.TransferAuthorizedObjectInContext);
                gvDocumentInAUTH.DataSource = this.TransferDocumentGridInContext;
                gvDocumentInAUTH.DataBind();
                //Distruggo
                Session["DocInSESSION"] = null;

                this.Autorizzazione_system_id = Convert.ToInt32(Session["Autorizzazione_system_id"]);
                //this.GetDataByPageState();
                this.upDocumentInAUTH.Update();
                this.upProjectInAuth.Update();
                this.UpTranfetButtons.Update();
            }
        }

        protected void btnProjectPostback_Click(object sender, EventArgs e)
        {
            if (Session["PrjInSESSION"] != null)
            {
                //Allineo a get fatta cpon la lista degli object che ho in pancia:
                AllineaProjectInContext((List<DocsPaWR.ARCHIVE_AUTH_grid_project>)Session["PrjInSESSION"], this.TransferAuthorizedObjectInContext);
                gvProjectInAUTH.DataSource = this.TransferProjectGridInContext;
                gvProjectInAUTH.DataBind();
                //Distruggo
                Session["PrjInSESSION"] = null;
                this.Autorizzazione_system_id = Convert.ToInt32(Session["Autorizzazione_system_id"]);
                // this.GetDataByPageState();
                this.upDocumentInAUTH.Update();
                this.upProjectInAuth.Update();
                this.UpTranfetButtons.Update();
            }
        }


        private void AllineaProjectInContext(List<ARCHIVE_AUTH_grid_project> list, List<ARCHIVE_AUTH_AuthorizedObject> TransferAuthorizedObjectInContext)
        {
            //Allineo i Fasc
            //1) Verifica dei fascicoli selezionati, ne ho selezionato alcuni già presenti?
            string responseFromMerge = string.Empty;

            if (list != null && list.Count > 0)
            {
                foreach (ARCHIVE_AUTH_grid_project _tmpitem in list)
                {
                    if (this.TransferAuthorizedObjectInContext != null &&
                        this.TransferAuthorizedObjectInContext.Where(x => x.Project_ID == _tmpitem.System_ID).Count() > 0)
                    {
                        responseFromMerge += " - " + _tmpitem.System_ID;
                    }
                    else
                    {
                        ARCHIVE_AUTH_AuthorizedObject _aaObjs = new ARCHIVE_AUTH_AuthorizedObject();
                        _aaObjs.Authorization_ID = this.Autorizzazione_system_id;
                        _aaObjs.Project_ID = _tmpitem.System_ID;
                        _aaObjs.Profile_ID = 0;
                        _aaObjs.System_ID = 1000;
                        if (TransferAuthorizedObjectInContext != null)
                            this.TransferAuthorizedObjectInContext.Add(_aaObjs);
                        else
                        {
                            this.TransferAuthorizedObjectInContext = new List<ARCHIVE_AUTH_AuthorizedObject>();
                            this.TransferAuthorizedObjectInContext.Add(_aaObjs);
                        }
                        if (TransferProjectGridInContext != null)
                            this.TransferProjectGridInContext.Add(_tmpitem);
                        else
                        {
                            this.TransferProjectGridInContext = new List<ARCHIVE_AUTH_grid_project>();
                            this.TransferProjectGridInContext.Add(_tmpitem);
                        }
                    }
                }
            }
            else
            {
                if (Session["PrjInSESSION"] != null)
                {
                    //Allineo a get fatta cpon la lista degli object che ho in pancia:
                    this.TransferProjectGridInContext = (List<DocsPaWR.ARCHIVE_AUTH_grid_project>)Session["PrjInSESSION"];
                    gvProjectInAUTH.DataSource = this.TransferProjectGridInContext;
                    gvProjectInAUTH.DataBind();
                    //Distruggo
                    Session["PrjInSESSION"] = null;
                }
            }

            if (!string.IsNullOrEmpty(responseFromMerge))
            {
                string msgConfirm = "AllineaFascicoliInContext";
                msgConfirm.Replace("@@", responseFromMerge);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgConfirm.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgConfirm.Replace("'", @"\'") + "', 'error', '');}", true);
            }
        }

        private void AllineaDocumentInContext(List<ARCHIVE_AUTH_grid_document> list, List<ARCHIVE_AUTH_AuthorizedObject> TransferAuthorizedObjectInContext)
        {
            //Allineo i Fasc
            //1) Verifica dei fascicoli selezionati, ne ho selezionato alcuni già presenti?
            string responseFromMerge = string.Empty;
            if (list != null && list.Count > 0)
            {
                foreach (ARCHIVE_AUTH_grid_document _tmpitem in list)
                {
                    if (this.TransferAuthorizedObjectInContext != null &&
                        this.TransferAuthorizedObjectInContext.Where(x => x.Profile_ID == _tmpitem.System_ID).Count() > 0)
                    {
                        responseFromMerge += " - " + _tmpitem.System_ID;
                    }
                    else
                    {
                        ARCHIVE_AUTH_AuthorizedObject _aaObjs = new ARCHIVE_AUTH_AuthorizedObject();
                        _aaObjs.Authorization_ID = this.Autorizzazione_system_id;
                        _aaObjs.Project_ID = 0;
                        _aaObjs.Profile_ID = _tmpitem.System_ID;
                        _aaObjs.System_ID = 1000;
                        if (TransferAuthorizedObjectInContext != null)
                            this.TransferAuthorizedObjectInContext.Add(_aaObjs);
                        else
                        {
                            this.TransferAuthorizedObjectInContext = new List<ARCHIVE_AUTH_AuthorizedObject>();
                            this.TransferAuthorizedObjectInContext.Add(_aaObjs);
                        }
                        if (TransferDocumentGridInContext != null)
                            this.TransferDocumentGridInContext.Add(_tmpitem);
                        else
                        {
                            this.TransferDocumentGridInContext = new List<ARCHIVE_AUTH_grid_document>();
                            this.TransferDocumentGridInContext.Add(_tmpitem);
                        }

                    }

                    if (!string.IsNullOrEmpty(responseFromMerge))
                    {
                        string msgConfirm = "AllineaDocumentiInContext";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgConfirm.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgConfirm.Replace("'", @"\'") + "', 'error', '');}", true);
                    }
                }
            }
            else
            {
                if (Session["DocInSESSION"] != null)
                {
                    TransferDocumentGridInContext = (List<DocsPaWR.ARCHIVE_AUTH_grid_document>)Session["DocInSESSION"];
                    gvDocumentInAUTH.DataSource = this.TransferDocumentGridInContext;
                    gvDocumentInAUTH.DataBind();
                    //Distruggo
                    Session["DocInSESSION"] = null;
                }
            }
        }

        protected void btnPrjRemove_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn1 = sender as ImageButton;
            GridViewRow row = (GridViewRow)ibtn1.NamingContainer;
            int _idDocResult = Convert.ToInt32(gvProjectInAUTH.DataKeys[row.RowIndex].Value);
            var _todeleteobj = TransferAuthorizedObjectInContext.Where(x => x.Project_ID == _idDocResult).FirstOrDefault();
            var _todeletegrid = TransferProjectGridInContext.Where(x => x.System_ID == _idDocResult).FirstOrDefault();
            TransferAuthorizedObjectInContext.Remove(_todeleteobj);
            TransferProjectGridInContext.Remove(_todeletegrid);
            gvProjectInAUTH.DataSource = TransferProjectGridInContext;
            gvProjectInAUTH.DataBind();
            upProjectInAuth.Update();
        }

        protected void btnDocRemove_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn1 = sender as ImageButton;
            GridViewRow row = (GridViewRow)ibtn1.NamingContainer;
            int _idDocResult = Convert.ToInt32(gvDocumentInAUTH.DataKeys[row.RowIndex].Value);
            var _todeleteobj = TransferAuthorizedObjectInContext.Where(x => x.Profile_ID == _idDocResult).FirstOrDefault();
            var _todeletegrid = TransferDocumentGridInContext.Where(x => x.System_ID == _idDocResult).FirstOrDefault();
            TransferAuthorizedObjectInContext.Remove(_todeleteobj);
            TransferDocumentGridInContext.Remove(_todeletegrid);
            gvDocumentInAUTH.DataSource = TransferDocumentGridInContext;
            gvDocumentInAUTH.DataBind();
            upDocumentInAUTH.Update();
        }

        protected void btnDocDetails_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn1 = sender as ImageButton;
            GridViewRow row = (GridViewRow)ibtn1.NamingContainer;
            int _idDocument = Convert.ToInt32(gvDocumentInAUTH.DataKeys[row.RowIndex].Value);
            SchedaDocumento doc = DocumentManager.getDocumentDetails(this.Page, _idDocument.ToString(), _idDocument.ToString());
            UIManager.DocumentManager.setSelectedRecord(doc);
            Response.Redirect("~/Document/Document.aspx");
        }

        protected void btnPrjDetails_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn1 = sender as ImageButton;
            GridViewRow row = (GridViewRow)ibtn1.NamingContainer;
            int _idPrj = Convert.ToInt32(gvProjectInAUTH.DataKeys[row.RowIndex].Value);
            Fascicolo prj = ProjectManager.getFascicoloById(this, _idPrj.ToString());
            UIManager.ProjectManager.setProjectInSession(prj);
            Response.Redirect("~/Project/project.aspx");

        }

        protected void gvProjectInAUTHTransferPageIndexChanging_click(object sender, GridViewPageEventArgs e)
        {
            this.gvProjectInAUTH.DataSource = this.TransferProjectGridInContext.ToList();
            this.gvProjectInAUTH.DataBind();
            this.gvProjectInAUTH.PageIndex = e.NewPageIndex;
            this.gvProjectInAUTH.DataBind();
            this.upProjectInAuth.Update();
        }

        protected void gvDocumentInAUTHTransferPageIndexChanging_click(object sender, GridViewPageEventArgs e)
        {
            this.gvDocumentInAUTH.DataSource = this.TransferDocumentGridInContext.ToList();
            this.gvDocumentInAUTH.DataBind();
            this.gvDocumentInAUTH.PageIndex = e.NewPageIndex;
            this.gvDocumentInAUTH.DataBind();
            this.upDocumentInAUTH.Update();
        }

    }
}