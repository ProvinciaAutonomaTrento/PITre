using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDatalLibrary;
using NttDataWA.UIManager;

namespace NttDataWA.Search
{
    public partial class SearchAutorizzazioniVersamento : System.Web.UI.Page
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

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OnlyNumbers", "OnlyNumbers();", true);
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
            this.InitializeRbl();
            this.InitializeDataObject();
            this.InitializeGridDatasource();

        }

        private void InitializeGridDatasource()
        {
            gvResultAUTH.DataSource = GetDatasourceDummygvResultAUTH();
            gvResultAUTH.DataBind();
            gvResultAUTH.Rows[0].Visible = false;

        }

        private void InitializeDataObject()
        {
            this.ddl_dtaDecorrenza.SelectedIndex = 0;
            this.ddl_dtaDecorrenza_SelectedIndexChanged(null, null);
            this.ddl_dtaDecorrenza.Text = string.Empty;

            this.ddl_dtaScadenza.SelectedIndex = 0;
            this.ddl_dtaScadenza_SelectedIndexChanged(null, null);
            this.ddl_dtaScadenza.Text = string.Empty;
        }

        private void InitializeRbl()
        {
            this.rblStateType.Items[0].Selected = true;
        }

        private void ResetControlPage()
        {
            txtCodiceUserInAuth.Text = string.Empty;
            txtDescrizioneUserInAuth.Text = string.Empty;
            TxtIdAutorizzazione.Text = string.Empty;
            dtaDecorrenza_TxtFrom.Text = string.Empty;
            dtaScadenza_TxtFrom.Text = string.Empty;
            this.InitializeRbl();
            this.InitializeDataObject();
            this.InitializeGridDatasource();
        }

        private object GetDatasourceDummygvResultAUTH()
        {

            List<DocsPaWR.ARCHIVE_AUTH_Authorization_Result> _dsDummylst = new List<DocsPaWR.ARCHIVE_AUTH_Authorization_Result>();
            ARCHIVE_AUTH_Authorization_Result _dsDummy = new ARCHIVE_AUTH_Authorization_Result();
            _dsDummy.System_ID = 0;
            _dsDummy.CodiceRubrica = string.Empty;
            _dsDummy.DescrizioneLivello = string.Empty;
            _dsDummy.DescrizioneRubrica = string.Empty;
            _dsDummy.DescrizioneRuolo = string.Empty;
            _dsDummy.DtaDecorrenza = DateTime.Now;
            _dsDummy.DtaScadenza = DateTime.Now;
            _dsDummy.Livello = 0;
            _dsDummy.People_ID = 0;
            _dsDummy.Ruolo_ID = 0;
            _dsDummylst.Add(_dsDummy);
            return _dsDummylst;
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.lit_dtaDecorrenza.Text = Utils.Languages.GetLabelFromCode("lit_dtaDecorrenza", language);
            this.litExpandUserInAuth.Text = Utils.Languages.GetLabelFromCode("litExpandUserInAuth", language);
            this.litUserInAuth.Text = Utils.Languages.GetLabelFromCode("litUserInAuth", language);
            this.LitAutorizzazioniVersamento.Text = Utils.Languages.GetLabelFromCode("LitAutorizzazioniVersamento", language);
            this.LitAutorizzazioneId.Text = Utils.Languages.GetLabelFromCode("LitAutorizzazioneId", language);
            this.btnClearFilterUserInAuth.Text = Utils.Languages.GetLabelFromCode("btnClearFilterUserInAuth", language);
            this.lit_dtaScadenza.Text = Utils.Languages.GetLabelFromCode("lit_dtaScadenza", language);
            this.dtaDecorrenza_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaDecorrenza_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.dtaScadenza_opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.dtaScadenza_opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.LitFiltriAuth.Text = Utils.Languages.GetLabelFromCode("LitFiltriAuth", language);
            this.LitStateTypeAuth.Text = Utils.Languages.GetLabelFromCode("LitStateTypeAuth", language);
            this.optAttiva.Text = Utils.Languages.GetLabelFromCode("optAttiva", language);
            this.optScaduta.Text = Utils.Languages.GetLabelFromCode("optScaduta", language);
            this.optTutti.Text = Utils.Languages.GetLabelFromCode("optTutti", language);
            this.SearchAuthorization.Text = Utils.Languages.GetLabelFromCode("SearchAuthorization", language);
            this.LitAuthResult.Text = Utils.Languages.GetLabelFromCode("LitAuthResult", language);
            this.lit_dtaDecorrenza.Text = Utils.Languages.GetLabelFromCode("lit_dtaDecorrenzaNoObl", language);
            this.lit_dtaScadenza.Text = Utils.Languages.GetLabelFromCode("lit_dtaScadenzaNoObl", language);
            this.litUserInAuth.Text = Utils.Languages.GetLabelFromCode("litUserInAuthNoObl", language);
            this.LitAutorizzazioniVersamento.Text = Utils.Languages.GetLabelFromCode("LitRicercaAUTHVersamento", language);
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

        protected void btnClearFilterUserInAuth_Click(object sender, EventArgs e)
        {
            ResetControlPage();
            upUtenteDetails.Update();
            upValidityTime.Update();
            upStateAuth.Update();
            UpnlGrid.Update();
            UpTranfetButtons.Update();
        }

        protected void SearchAuthorization_Click(object sender, EventArgs e)
        {
            //INIZIO:
            bool FiltroIDAuthorization = false;
            bool FiltroDtaDecorrezaAuthorization = false;
            bool FiltroDtaScadenzaAuthorization = false;
            bool FiltroUtenteAuthorization = false;

            if (!string.IsNullOrEmpty(this.idUserInAuth.Value))
            {
                FiltroUtenteAuthorization = true;
            }

            if (!string.IsNullOrEmpty(this.dtaDecorrenza_TxtFrom.Text))
            {
                FiltroDtaDecorrezaAuthorization = true;
            }

            if (!string.IsNullOrEmpty(this.dtaScadenza_TxtFrom.Text))
            {
                FiltroDtaScadenzaAuthorization = true;
            }

            if (!string.IsNullOrEmpty(this.TxtIdAutorizzazione.Text))
            {
                FiltroIDAuthorization = true;
            }

            //GetAll
            List<ARCHIVE_AUTH_Authorization> _lstOutput = UIManager.ArchiveManager.GetALLARCHIVE_Autorizations();
            //Ora la passo sotto linq:
            var query = _lstOutput.Where(x => x.System_ID != 0);

            if (FiltroIDAuthorization)
                query = query.Where(x => x.System_ID == Convert.ToInt32(TxtIdAutorizzazione.Text.Trim()));

            if (FiltroUtenteAuthorization)
                query = query.Where(x => x.People_ID == Convert.ToInt32(idUserInAuth.Value.Trim()));

            if (FiltroDtaDecorrezaAuthorization)
                query = query.Where(x => x.StartDate >= Convert.ToDateTime(dtaDecorrenza_TxtFrom.Text.Trim()));

            if (FiltroDtaScadenzaAuthorization)
                query = query.Where(x => x.EndDate <= Convert.ToDateTime(dtaScadenza_TxtFrom.Text.Trim()));

            switch (rblStateType.SelectedItem.Value)
            {
                //Attiva
                case "A":
                    query = query.Where(x => x.EndDate >= DateTime.Now);
                    break;
                //Scaduta
                case "S":
                    query = query.Where(x => x.EndDate <= DateTime.Now);
                    break;
                //Tutti
                case "T":
                    break;
            }

            if (query.Count() > 0)
                Transfor_and_Bind_DataSource(query.ToList());
            else
                InitializeGridDatasource();

            UpnlGrid.Update();

        }

        private void Transfor_and_Bind_DataSource(List<ARCHIVE_AUTH_Authorization> query)
        {
            //trasformo l'autorizzazione nel mio datasource per la griglia.
            List<ARCHIVE_AUTH_Authorization_Result> _dsource = new List<ARCHIVE_AUTH_Authorization_Result>();
            foreach (ARCHIVE_AUTH_Authorization _autAut in query)
            {
                Utente tempCorrSingle;
                tempCorrSingle = UserManager.GetUtenteByIdPeople(_autAut.People_ID.ToString());
                ARCHIVE_AUTH_Authorization_Result _result = new ARCHIVE_AUTH_Authorization_Result();
                _result.System_ID = (int)_autAut.System_ID;
                // _result.CodiceRubrica = tempCorrSingle.codiceRubrica;
                _result.DescrizioneRubrica = tempCorrSingle.descrizione;
                _result.DtaDecorrenza = (DateTime)_autAut.StartDate;
                _result.DtaScadenza = (DateTime)_autAut.EndDate;
                _dsource.Add(_result);
            }

            if (_dsource.Count > 0)
            {
                gvResultAUTH.DataSource = _dsource;
                gvResultAUTH.DataBind();
            }
            else
                InitializeGridDatasource();

            UpnlGrid.Update();
        }

        protected void btnAuthorizationDetails_Click(object sender, EventArgs e)
        {
            ImageButton ibtn1 = sender as ImageButton;
            GridViewRow row = (GridViewRow)ibtn1.NamingContainer;
            int _idAUTHResult = Convert.ToInt32(gvResultAUTH.DataKeys[row.RowIndex].Value);
            Session["PAGESTATE"] = "SEA";
            Session["ID_AUTORIZZAZIONE"] = _idAUTHResult;
            //DEVO VALORIZZARE LE SESSIONI!!
            //prendo le auth, poi lo farò performante:
            //FASCICOLI!!!!
            List<ARCHIVE_AUTH_AuthorizedObject> _tmpobjPRJ = UIManager.ArchiveManager.GetALLARCHIVE_AutorizedObject();
            _tmpobjPRJ = _tmpobjPRJ.Where(x => x.Authorization_ID == _idAUTHResult).Where(y => y.Profile_ID == 0).ToList();
            //qq = ;

            if (_tmpobjPRJ != null && _tmpobjPRJ.Count > 0)
            {
                List<DocsPaWR.ARCHIVE_AUTH_grid_project> _lstFasc = new List<ARCHIVE_AUTH_grid_project>();
                foreach (ARCHIVE_AUTH_AuthorizedObject _okj in _tmpobjPRJ)
                {
                    DocsPaWR.Fascicolo prj = null;
                    prj = ProjectManager.getFascicoloById(this, _okj.Project_ID.ToString());
                    string codice = prj.codice.Replace(@"\", @"\\");
                    DocsPaWR.ARCHIVE_AUTH_grid_project _tmpprj = new ARCHIVE_AUTH_grid_project();
                    _tmpprj.System_ID = Convert.ToInt32(prj.systemID);
                    _tmpprj.Codice = prj.codice;
                    _tmpprj.Registro = prj.codiceRegistroNodoTit;
                    _tmpprj.Descrizione = prj.descrizione;
                    _tmpprj.DataApertura = Convert.ToDateTime(prj.dataCreazione);
                    _tmpprj.DataChiusura = Convert.ToDateTime(prj.chiusura);
                    _lstFasc.Add(_tmpprj);
                }

                Session["PrjInSESSION"] = _lstFasc;
            }
            //DOCUMENTI!!!!!!!
            List<ARCHIVE_AUTH_AuthorizedObject> _tmpobjDOC = UIManager.ArchiveManager.GetALLARCHIVE_AutorizedObject().Where(x => x.Authorization_ID == _idAUTHResult).Where(y => y.Project_ID == 0).ToList();
            if (_tmpobjDOC != null && _tmpobjDOC.Count > 0)
            {
                List<DocsPaWR.ARCHIVE_AUTH_grid_document> _lstDocs = new List<ARCHIVE_AUTH_grid_document>();

                foreach (ARCHIVE_AUTH_AuthorizedObject _tmpdoc in _tmpobjDOC)
                {
                    string idNumRecord = string.Empty;
                    SchedaDocumento doc = DocumentManager.getDocumentDetails(this.Page, _tmpdoc.Profile_ID.ToString(), _tmpdoc.Profile_ID.ToString());
                    if (doc != null)
                    {
                        DocsPaWR.ARCHIVE_AUTH_grid_document _tmpDoc = new ARCHIVE_AUTH_grid_document();
                        if (doc.protocollo != null && !string.IsNullOrEmpty(doc.protocollo.numero))
                        {
                            idNumRecord += doc.protocollo.numero + " - " + doc.protocollo.dataProtocollazione + "<br/>";
                        }
                        else
                        {
                            idNumRecord += doc.docNumber + " - " + doc.dataCreazione + "<br/>";
                        }
                        _tmpDoc.ID_Protocollo = idNumRecord;
                        _tmpDoc.Registro = doc.registro != null ? doc.registro.codRegistro : "";
                        _tmpDoc.Tipo = doc.tipoProto;
                        _tmpDoc.Oggetto = doc.oggetto.descrizione;
                        _tmpDoc.Data = Convert.ToDateTime(doc.dataCreazione);
                        if (doc.tipoProto.Equals("P"))
                        {
                            _tmpDoc.Mittente_Destinatario = ((DocsPaWR.ProtocolloUscita)doc.protocollo).mittente.descrizione + " / " +
                                                              ((DocsPaWR.ProtocolloUscita)doc.protocollo).destinatari[0].descrizione;
                        }
                        if (doc.tipoProto.Equals("I"))
                        {
                            _tmpDoc.Mittente_Destinatario = ((DocsPaWR.ProtocolloInterno)doc.protocollo).mittente.descrizione + " / " +
                                                                    ((DocsPaWR.ProtocolloUscita)doc.protocollo).destinatari[0].descrizione;
                        }
                        if (doc.tipoProto.Equals("A"))
                        {
                            _tmpDoc.Mittente_Destinatario = ((DocsPaWR.ProtocolloEntrata)doc.protocollo).mittente.descrizione;
                        }
                        if (doc.tipoProto.Equals("G"))
                        {
                            _tmpDoc.Mittente_Destinatario = "";
                        }
                        _lstDocs.Add(_tmpDoc);
                    }
                }

                Session["DocInSESSION"] = _lstDocs;

            }
            Response.Redirect("~/Deposito/AutorizzazioniVersamento.aspx");
        }
    }
}