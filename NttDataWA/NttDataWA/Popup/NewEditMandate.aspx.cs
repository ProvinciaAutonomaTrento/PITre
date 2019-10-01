using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDatalLibrary;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;


namespace NttDataWA.Popup
{
    public partial class NewEditMandate : System.Web.UI.Page
    {

        public RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else
                    return RubricaCallType.CALLTYPE_PROTO_INT_DEST;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }

        }

        public InfoDelega DelegaSel
        {
            get
            {
                return HttpContext.Current.Session["NewEditMandate_sel"] as InfoDelega;
            }
            set
            {
                HttpContext.Current.Session["NewEditMandate_sel"] = value;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializePage();
            }
            else
            {
                this.ReadRetValueFromPopup();
            }
            this.RefreshScript();
        }

        private void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.proceed_permanent.Value))
            {
                if (this.DelegaSel == null)
                {
                    DocsPaWR.InfoDelega delega = (DocsPaWR.InfoDelega)(ViewState["delega"]);
                    if (DelegheManager.CreaNuovaDelega(delega))
                    {
                        this.CloseMask(true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorMandateInserting', 'error', '');", true);
                        this.proceed_permanent.Value = string.Empty;
                        this.UpPnlButtons.Update();
                    }
                }
                else
                {
                    this.Modifica();
                }
            }
        }

        protected void InitializePage()
        {
            RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
            this.txtDateFrom.Text = DateTime.Now.ToString("dd/MM/yyyy");

            this.InitializeLanguage();
            this.PopulateDdlRoles();
            this.SetAjaxAddressBook();
            this.CompileIfModify();
        }

        private void CompileIfModify()
        {
            if (this.DelegaSel != null)
            {
                // role
                this.MandateDdlRole.Items.Clear();
                ListItem item;
                DocsPaWR.Utente userHome = UserManager.GetUserInSession();
                if (userHome != null)
                {
                    if (userHome.ruoli != null)
                    {
                        if (userHome.ruoli.Length > 1)
                        {
                            item = new ListItem(this.GetLabel("MandateOptAllM"));
                            this.MandateDdlRole.Items.Add(item);
                        }
                        for (int i = 0; i < userHome.ruoli.Length; i++)
                        {
                            item = new ListItem(((DocsPaWR.Ruolo)userHome.ruoli[i]).descrizione.ToString(), ((DocsPaWR.Ruolo)userHome.ruoli[i]).systemId.ToString() + "_" + ((DocsPaWR.Ruolo)userHome.ruoli[i]).uo.systemId);
                            this.MandateDdlRole.Items.Add(item);
                        }
                    }
                }
                
                try
                {
                    //Ruolo tempRole = (Ruolo)AddressBookManager.GetCorrespondentBySystemId(this.DelegaSel.id_ruolo_delegante);
                    foreach (ListItem item2 in this.MandateDdlRole.Items)
                    {
                        if (item2.Text == this.DelegaSel.cod_ruolo_delegante)
                        {
                            item2.Selected = true;
                            break;
                        }
                    }
                }
                catch {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('ErrorNewEditMandateRoleNonExistent', 'error', '');", true);
                    return;
                }

                // delegato
                Corrispondente delegato = UserManager.getCorrispondentBySystemID(this.DelegaSel.id_people_corr_globali);
                this.idProprietario.Value = delegato.systemId;
                this.idPeople.Value = ((Utente)delegato).idPeople;
                this.txtCodiceProprietario.Text = delegato.codiceRubrica;
                this.txtDescrizioneProprietario.Text = delegato.descrizione;

                // ruoli delegato
                this.PopulateDdlUserRoles();
                for (int j = 0; j < this.DdlMandateUserRole.Items.Count; j++)
                    if (this.DdlMandateUserRole.Items[j].Value == this.DelegaSel.id_ruolo_delegato)
                        this.DdlMandateUserRole.Items[j].Selected = true;

                this.upPnlProprietario.Update();

                //impostazione data decorrenza
                DateTime dataDecorrenza = Utils.dateformat.ConvertToDate(this.DelegaSel.dataDecorrenza);
                this.txtDateFrom.Text = dataDecorrenza.ToShortDateString();
                string oraDecorrenza = dataDecorrenza.ToShortTimeString();
                if (!string.IsNullOrEmpty(oraDecorrenza))
                {
                    oraDecorrenza = oraDecorrenza.Substring(0, 2);
                    if (oraDecorrenza.EndsWith("."))
                        oraDecorrenza = "0" + oraDecorrenza.Substring(0, 1);
                    this.ddlHourFrom.SelectedIndex = Convert.ToInt32(oraDecorrenza)+1;
                }
                else
                    this.ddlHourFrom.SelectedIndex = 0;

                //impostazione data scadenza
                DateTime dataScadenza = Utils.dateformat.ConvertToDate(this.DelegaSel.dataScadenza);
                if (dataScadenza==new DateTime())
                {
                    this.txtDateTo.Text = string.Empty;
                    this.ddlHourTo.SelectedIndex = 0;
                }
                else
                {
                    this.txtDateTo.Text = dataScadenza.ToShortDateString();
                    string oraScadenza = "";

                    oraScadenza = dataScadenza.ToShortTimeString();

                    if (!string.IsNullOrEmpty(oraScadenza))
                    {
                        oraScadenza = oraScadenza.Substring(0, 2);
                        if (oraScadenza.EndsWith("."))
                            oraScadenza = "0" + oraScadenza.Substring(0, 1);
                        this.ddlHourTo.SelectedIndex = Convert.ToInt32(oraScadenza) + 1;
                    }
                    else
                        this.ddlHourTo.SelectedIndex = 0;
                }

                //delega attiva (non è possibile modificare la data di decorrenza di una delega attiva)
                if (dataDecorrenza < DateTime.Now && (dataScadenza==new DateTime() || dataScadenza > DateTime.Now))
                {
                    this.txtDateFrom.Enabled = false;
                    this.ddlHourFrom.Enabled = false;
                }
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnSave.Text = Utils.Languages.GetLabelFromCode("NewEditMandateBtnSave", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("NewEditMandateBtnClose", language);
            this.litMandateNewUser.Text = Utils.Languages.GetLabelFromCode("MandateNewUser", language);
            this.litMandateNewRole.Text = Utils.Languages.GetLabelFromCode("MandateNewRole", language);
            this.litRoleDelegate.Text = Utils.Languages.GetLabelFromCode("MandateNewRoleDelegate", language);
            this.litMandateNewDate.Text = Utils.Languages.GetLabelFromCode("MandateNewDate", language);
            this.litMandateNewEffectiveDate.Text = Utils.Languages.GetLabelFromCode("MandateNewEffectiveDate", language);
            this.litMandateNewExpireDate.Text = Utils.Languages.GetLabelFromCode("MandateNewExpireDate", language);
            this.litMandateNewHour.Text = Utils.Languages.GetLabelFromCode("MandateNewHour", language);
            this.litMandateNewHour2.Text = Utils.Languages.GetLabelFromCode("MandateNewHour", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("MandateNewAddressBook", language);
            this.ImgProprietarioAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("MandateNewAddressBook", language);
        }

        private string GetLabel(string id)
        {
            return Utils.Languages.GetLabelFromCode(id, UIManager.UserManager.GetUserLanguage());
        }

        protected void SetAjaxAddressBook()
        {
            string dataUser = RoleManager.GetRoleInSession().systemId;
            dataUser = dataUser + "-" + RoleManager.GetRoleInSession().registri[0].systemId;

            string callType = "CALLTYPE_CORR_INT_NO_UO"; // Destinatario su protocollo interno
            this.RapidProprietario.ContextKey = dataUser + "-" + UserManager.GetUserInSession().idAmministrazione + "-" + callType;
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void CloseMask(bool withReturnValue)
        {
            string retValue = withReturnValue ? "true" : string.Empty;
            string popupId = this.DelegaSel == null ? "NewMandate" : "EditMandate";
            //this.DelegaSel = null;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('"+popupId+"', '"+retValue+"');", true);        
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            this.CloseMask(false);
        }

        protected void ImgProprietarioAddressBook_Click(object sender, EventArgs e)
        {
            this.CallType = RubricaCallType.CALLTYPE_PROTO_INT_DEST;
            HttpContext.Current.Session["AddressBook.from"] = "F_X_X_S_3";
            HttpContext.Current.Session["AddressBook.EnableOnly"] = "P";
            HttpContext.Current.Session["AddDocInProject"] = true;
            // AddressBookChkCommonAddressBook
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpPnlSender", "ajaxModalPopupAddressBook();", true);
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            CustomTextArea caller = sender as CustomTextArea;
            string codeAddressBook = caller.Text;

            if (!string.IsNullOrEmpty(codeAddressBook))
            {
                this.SearchCorrespondent(codeAddressBook, caller.ID);
            }
            else
            {
                this.txtCodiceProprietario.Text = string.Empty;
                this.txtDescrizioneProprietario.Text = string.Empty;
                this.idProprietario.Value = string.Empty;
                this.idPeople.Value = string.Empty;
                this.plcRoleDelegate.Visible = false;
                this.upPnlProprietario.Update();
            }
        }

        protected void SearchCorrespondent(string addressCode, string idControl)
        {
            DocsPaWR.Corrispondente corr = UIManager.AddressBookManager.getCorrispondenteRubrica(addressCode, this.CallType);
            if (corr != null && corr.tipoCorrispondente=="P")
            {
                this.txtCodiceProprietario.Text = corr.codiceRubrica;
                this.txtDescrizioneProprietario.Text = corr.descrizione;
                this.idProprietario.Value = corr.systemId;
                this.idPeople.Value = ((Utente)corr).idPeople;
                this.PopulateDdlUserRoles();
                this.upPnlProprietario.Update();
            }
            else
            {
                this.txtCodiceProprietario.Text = string.Empty;
                this.txtDescrizioneProprietario.Text = string.Empty;
                this.idProprietario.Value = string.Empty;
                this.idPeople.Value = string.Empty;
                this.plcRoleDelegate.Visible = false;
                this.upPnlProprietario.Update();

                string msg = "ErrorTransmissionCorrespondentNotFound";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
            }
        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
            string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();

            if (atList != null && atList.Count > 0)
            {
                NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                Corrispondente tempCorrSingle;
                if (!corrInSess.isRubricaComune)
                    tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                else
                    tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                this.txtCodiceProprietario.Text = tempCorrSingle.codiceRubrica;
                this.txtDescrizioneProprietario.Text = tempCorrSingle.descrizione;
                this.idProprietario.Value = tempCorrSingle.systemId;
                this.idPeople.Value = ((Utente)tempCorrSingle).idPeople;
                this.PopulateDdlUserRoles();
                this.upPnlProprietario.Update();
            }

            HttpContext.Current.Session["AddressBook.At"] = null;
            HttpContext.Current.Session["AddressBook.Cc"] = null;
        }

        private void PopulateDdlRoles()
        {
            this.MandateDdlRole.Items.Clear();
            ListItem item;
            DocsPaWR.Utente userHome = UserManager.GetUserInSession();
            if (userHome != null)
            {
                if (userHome.ruoli != null)
                {
                    if (userHome.ruoli.Length > 1)
                    {
                        //Inserimento voce "Tutti"
                        item = new ListItem(this.GetLabel("MandateOptAllM"));
                        this.MandateDdlRole.Items.Add(item);
                    }
                    for (int i = 0; i < userHome.ruoli.Length; i++)
                    {
                        item = new ListItem(((DocsPaWR.Ruolo)userHome.ruoli[i]).descrizione.ToString(), ((DocsPaWR.Ruolo)userHome.ruoli[i]).systemId.ToString() + "_" + ((DocsPaWR.Ruolo)userHome.ruoli[i]).uo.systemId);
                        this.MandateDdlRole.Items.Add(item);
                    }
                }
            }
        }

        private void PopulateDdlUserRoles()
        {
            this.DdlMandateUserRole.Items.Clear();
            foreach (OrgRisultatoRicerca r in DelegheManager.GetRolesForUser(this.txtCodiceProprietario.Text))
                this.DdlMandateUserRole.Items.Add(new ListItem(r.DescParent, r.IDParent));

            this.plcRoleDelegate.Visible = true;
        }

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            if (this.DelegaSel == null)
            {
                this.confermaNuova();
            }
            else
            {
                this.confermaModifica();
            }
        }

        private void confermaNuova()
        {
            //Costruzione della data di scadenza nel formato "dd/MM/yyyy HH.mm.ss"
            string dta_scadenza = this.txtDateTo.Text;
            if (!string.IsNullOrEmpty(dta_scadenza) && dta_scadenza.Length <= 10)
            {
                if (!string.IsNullOrEmpty(this.ddlHourTo.SelectedValue))
                {
                    string ora = this.ddlHourTo.SelectedValue;
                    if (this.ddlHourTo.SelectedValue.Length == 1)
                        ora = "0" + this.ddlHourTo.SelectedValue;
                    dta_scadenza = dta_scadenza + " " + ora + ".00.00";
                }
                else
                    dta_scadenza = dta_scadenza + " " + System.DateTime.Now.Hour + ".00.00";
            }

            //Costruzione della data di decorrenza nel formato "dd/MM/yyyy HH.mm.ss"
            string dta_decorrenza = this.txtDateFrom.Text;
            if (!string.IsNullOrEmpty(dta_decorrenza) && dta_decorrenza.Length <= 10)
            {
                if (!string.IsNullOrEmpty(this.ddlHourFrom.SelectedValue))
                {
                    string ora = this.ddlHourFrom.SelectedValue;
                    if (this.ddlHourFrom.SelectedValue.Length == 1)
                        ora = "0" + this.ddlHourFrom.SelectedValue;
                    dta_decorrenza = dta_decorrenza + " " + ora + ".00.00";
                }
                else
                    dta_decorrenza = dta_decorrenza + " " + System.DateTime.Now.Hour + ".00.00";
            }

            if (System.Globalization.DateTimeFormatInfo.CurrentInfo.TimeSeparator.Contains(":"))
            {
                dta_decorrenza = dta_decorrenza.Replace('.', ':');
                dta_scadenza = dta_scadenza.Replace('.', ':');
            }

            //Verifica correttezza delle date
            //Data di scadenza nulla o vuota: caso delega permanente --> nessun controllo sulla data 
            //di scandenza, mentre invece si deve controllare che la data di decorrenza sia nel formato
            //corretto
            if (string.IsNullOrEmpty(dta_scadenza))
            {
                if (string.IsNullOrEmpty(dta_decorrenza) || !utils.isDate(dta_decorrenza))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningMandateDateFrom', 'warning', '');", true);
                    return;
                }
            }
            else
                //non si sta costruendo una delega permanete, quindi occorre controllare anche la data di scadenza
                if (!utils.isDate(dta_decorrenza) //la data di decorrenza è in formato corretto
                    || !utils.isDate(dta_scadenza) // la data di scadenza è in formato corretto
                    || !utils.verificaIntervalloDate(dta_scadenza, dta_decorrenza)) //verifica che la data di scadenza sia maggiore di quella di decorrenza
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningMandateDateInterval', 'warning', '');", true);
                    return;
                }

            //Verifica che la data di decorrenza sia odierna
            //
            if (!utils.verificaIntervalloDate(Convert.ToDateTime(dta_decorrenza).ToShortDateString(), System.DateTime.Now.ToShortDateString()))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningMandateDateIntervalIsCorrect', 'warning', '');", true);
                return;
            }

            //Verifica che sia stato selezionato un delegato
            if (string.IsNullOrEmpty(this.idProprietario.Value))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningMandateUserRequired', 'warning', '');", true);
                return;
            }
            else
                if (this.idPeople.Value == UserManager.GetInfoUser().idPeople)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningMandateUserIsCurrent', 'warning', '');", true);
                    return;
                }

            if (string.IsNullOrEmpty(this.DdlMandateUserRole.SelectedValue))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningMandateRoleRequired', 'warning', '');", true);
                return;
            }

            //Se tutto è a posto (date, delegato, delegante) si può creare la delega
            this.CreaDelega();
        }

        /// <summary>
        /// Creazione di una nuova delega
        /// </summary>
        private void CreaDelega()
        {
            DocsPaWR.InfoDelega delega = new DocsPaWR.InfoDelega();
            if (this.MandateDdlRole.Items.Count > 1 && this.MandateDdlRole.SelectedIndex == 0)
            {
                delega.id_ruolo_delegante = "0";
                delega.cod_ruolo_delegante = "TUTTI";
            }
            else
            {
                string[] valoriRuolo = this.MandateDdlRole.SelectedValue.ToString().Split('_');
                delega.id_ruolo_delegante = valoriRuolo[0];
                delega.cod_ruolo_delegante = this.MandateDdlRole.SelectedItem.Text;
            }

            delega.id_utente_delegante = UserManager.GetInfoUser().idPeople;
            DocsPaWR.Corrispondente corr = new DocsPaWR.Corrispondente();
            DocsPaWR.Ruolo ruolo = RoleManager.getRuoloByIdGruppo(RoleManager.getRuoloById(this.DdlMandateUserRole.SelectedValue).idGruppo);
            corr = UserManager.getCorrispondenteByIdPeople(this, UserManager.GetInfoUser().idPeople, DocsPaWR.AddressbookTipoUtente.INTERNO);
            delega.cod_utente_delegante = corr.codiceRubrica;
            delega.id_ruolo_delegato = ruolo.systemId;
            delega.id_uo_delegato = ruolo.uo.systemId;
            delega.cod_ruolo_delegato = ruolo.codiceRubrica;
            delega.id_utente_delegato = this.idPeople.Value;
            delega.cod_utente_delegato = this.txtCodiceProprietario.Text;
            string ora;
            if (this.ddlHourFrom.SelectedIndex == 0)
                ora = System.DateTime.Now.Hour.ToString();
            else
                ora = this.ddlHourFrom.SelectedValue;
            if (ora.Length == 1)
                ora = "0" + this.ddlHourFrom.SelectedValue;
            if (Convert.ToDateTime(this.txtDateFrom.Text).Date.ToShortDateString().Equals(DateTime.Now.ToShortDateString())
                 && ora.Equals(DateTime.Now.Hour.ToString()))
                delega.dataDecorrenza = Convert.ToDateTime(this.txtDateFrom.Text).Date.ToShortDateString() + " " + ora + "." + (DateTime.Now.Minute).ToString() + ".00";
            else
                delega.dataDecorrenza = Convert.ToDateTime(this.txtDateFrom.Text).Date.ToShortDateString() + " " + ora + ".00.00";

            if (this.ddlHourTo.SelectedIndex == 0)
                ora = System.DateTime.Now.Hour.ToString();
            else
                ora = this.ddlHourTo.SelectedValue;
            if (ora.Length == 1)
                ora = "0" + this.ddlHourTo.SelectedValue;
            if (!string.IsNullOrEmpty(this.txtDateTo.Text))
                delega.dataScadenza = Convert.ToDateTime(this.txtDateTo.Text).Date.ToShortDateString() + " " + ora + ".00.00";
            else
                delega.dataScadenza = string.Empty;

            if (System.Globalization.DateTimeFormatInfo.CurrentInfo.TimeSeparator.Contains(":"))
            {
                delega.dataDecorrenza = delega.dataDecorrenza.Replace('.', ':');
                delega.dataScadenza = delega.dataScadenza.Replace('.', ':');
            }

            //Verifica che non sia stata già creata una delega nello stesso periodo (univocità dell'assegnazione di responsabilità)
            if (DelegheManager.VerificaUnicaDelega(delega))
            {
                if (string.IsNullOrEmpty(this.txtDateTo.Text))
                {
                    ViewState.Add("delega", delega);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('ConfirmMandatePermanent', 'proceed_permanent', '');", true);
                    return;
                }
                else
                {
                    if (DelegheManager.CreaNuovaDelega(delega))
                    {
                        this.CloseMask(true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorMandateInserting', 'error', '');", true);
                        return;
                    }
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorMandateOverlappingPeriods', 'warning', '');", true);
                return;
            }

        }

        private void confermaModifica()
        {
            //Costruzione della data di scadenza nel formato "dd/MM/yyyy HH.mm.ss"
            string dta_scadenza = this.txtDateTo.Text;
            if (!string.IsNullOrEmpty(dta_scadenza) && dta_scadenza.Length <= 10)
            {
                if (!string.IsNullOrEmpty(this.ddlHourTo.SelectedValue))
                {
                    string ora = this.ddlHourTo.SelectedValue;
                    if (this.ddlHourTo.SelectedValue.Length == 1)
                        ora = "0" + this.ddlHourTo.SelectedValue;
                    dta_scadenza = dta_scadenza + " " + ora + ".00.00";
                }
                else
                    dta_scadenza = dta_scadenza + " " + System.DateTime.Now.Hour + ".00.00";
            }

            //Costruzione della data di decorrenza nel formato "dd/MM/yyyy HH.mm.ss"
            string dta_decorrenza = this.txtDateFrom.Text;
            if (!string.IsNullOrEmpty(dta_decorrenza) && dta_decorrenza.Length <= 10)
            {
                if (!string.IsNullOrEmpty(this.ddlHourFrom.SelectedValue))
                {
                    string ora = this.ddlHourFrom.SelectedValue;
                    if (this.ddlHourFrom.SelectedValue.Length == 1)
                        ora = "0" + this.ddlHourFrom.SelectedValue;
                    dta_decorrenza = dta_decorrenza + " " + ora + ".00.00";
                }
                else
                    dta_decorrenza = dta_decorrenza + " " + System.DateTime.Now.Hour + ".00.00";
            }

            //Verifica Date
            if (string.IsNullOrEmpty(dta_scadenza))
            {
                if (string.IsNullOrEmpty(dta_decorrenza) || !utils.isDate(dta_decorrenza))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningMandateDateFrom', 'warning', '');", true);
                    return;
                }
            }
            else
                if (!utils.isDate(dta_decorrenza)
                    || !utils.isDate(dta_scadenza)
                    || !utils.verificaIntervalloDate(dta_scadenza, dta_decorrenza)
                    || Convert.ToDateTime(dta_scadenza.Replace('.', ':')) < DateTime.Now
                    )
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningMandateDateInterval', 'warning', '');", true);
                    return;
                }

            //Verifica che sia stato selezionato un delegato
            if (string.IsNullOrEmpty(this.idProprietario.Value))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningMandateUserRequired', 'warning', '');", true);
                return;
            }
            else
                if (this.idPeople.Value == UserManager.GetInfoUser().idPeople)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningMandateUserIsCurrent', 'warning', '');", true);
                    return;
                }

            if (string.IsNullOrEmpty(this.txtDateTo.Text))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('ConfirmMandatePermanent', 'proceed_permanent', '');", true);
            }
            else
                Modifica();
        }

        private void Modifica()
        {
            string[] valoriRuolo = this.MandateDdlRole.SelectedValue.ToString().Split('_');
            string idUtenteOld = "";
            string idRuoloOld = "";
            string idRuoloDeleganteOld = "";
            string tipoDelega = "";
            string dataScadenzaOld = "";
            string dataDecorrenzaOld = "";

            //recupero le informazioni sulla delega che si vuole modificare
            DocsPaWR.Ruolo ruolo = RoleManager.getRuoloByIdGruppo(RoleManager.getRuoloById(this.DdlMandateUserRole.SelectedValue).idGruppo);
            if (this.DelegaSel.id_utente_delegato != this.idPeople.Value)
            {
                idUtenteOld = this.DelegaSel.id_utente_delegato;
                this.DelegaSel.id_utente_delegato = this.idPeople.Value;
                this.DelegaSel.cod_utente_delegato = this.txtCodiceProprietario.Text;
            }
            if (this.DelegaSel.id_ruolo_delegato != ruolo.systemId)
            {
                idRuoloOld = this.DelegaSel.id_ruolo_delegato;
                this.DelegaSel.id_ruolo_delegato = ruolo.systemId;
                this.DelegaSel.cod_ruolo_delegato = ruolo.codiceRubrica;
            }

            idRuoloDeleganteOld = this.DelegaSel.id_ruolo_delegante;
            this.DelegaSel.id_ruolo_delegante = valoriRuolo[0];
            if (valoriRuolo[0].ToUpper().Equals("TUTTI"))
                this.DelegaSel.id_ruolo_delegante = "0";

            if (idRuoloDeleganteOld.Equals(this.DelegaSel.id_ruolo_delegante))
                idRuoloDeleganteOld = "";

            //this.DelegaSel.cod_ruolo_delegante = ruolo.codiceRubrica;
            this.DelegaSel.cod_ruolo_delegante = this.MandateDdlRole.SelectedItem.Text;
            tipoDelega = "I";
            DateTime dataDecorrenza = Utils.dateformat.ConvertToDate(this.DelegaSel.dataDecorrenza);
            DateTime dataScadenza = Utils.dateformat.ConvertToDate(this.DelegaSel.dataScadenza);
            if (dataDecorrenza < DateTime.Now && (dataScadenza==new DateTime() || dataScadenza > DateTime.Now))
            {
                tipoDelega = "A";
            }
            if (dataScadenza!=new DateTime() && dataScadenza < DateTime.Now)
            {
                tipoDelega = "S";
            }

            dataScadenzaOld = this.DelegaSel.dataScadenza;
            dataDecorrenzaOld = this.DelegaSel.dataDecorrenza;
            string ora;
            if (this.ddlHourFrom.SelectedIndex == 0)
                ora = System.DateTime.Now.Hour.ToString();
            else
                ora = this.ddlHourFrom.SelectedValue;
            if (ora.Length == 1)
                ora = "0" + this.ddlHourFrom.SelectedValue;


            if (Convert.ToDateTime(this.txtDateFrom.Text).Date.ToShortDateString().Equals(DateTime.Now.ToShortDateString())
                  && ora.Equals(DateTime.Now.Hour.ToString()))
                this.DelegaSel.dataDecorrenza = Convert.ToDateTime(this.txtDateFrom.Text).Date.ToShortDateString() + " " + ora + "." + (DateTime.Now.Minute).ToString() + ".00";
            else
                this.DelegaSel.dataDecorrenza = Convert.ToDateTime(this.txtDateFrom.Text).Date.ToShortDateString() + " " + ora + ".00.00";

            if (this.ddlHourTo.SelectedIndex == 0)
                ora = System.DateTime.Now.Hour.ToString();
            else
                ora = this.ddlHourTo.SelectedValue;
            if (ora.Length == 1)
                ora = "0" + this.ddlHourTo.SelectedValue;
            if (!string.IsNullOrEmpty(this.txtDateTo.Text))
                this.DelegaSel.dataScadenza = Convert.ToDateTime(this.txtDateTo.Text).Date.ToShortDateString() + " " + ora + ".00.00";
            else
                this.DelegaSel.dataScadenza = string.Empty;

            if (System.Globalization.DateTimeFormatInfo.CurrentInfo.TimeSeparator.Contains(":"))
            {
                this.DelegaSel.dataDecorrenza = this.DelegaSel.dataDecorrenza.Replace('.', ':');
                this.DelegaSel.dataScadenza = this.DelegaSel.dataScadenza.Replace('.', ':');
            }
            if (DelegaSel.inEsercizio == "1" && tipoDelega.Equals("A"))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorMandateActiveUpdating', 'warning', '');", true);
                return;
            }
            else
            {
                if (DelegheManager.ModificaDelega(this.DelegaSel, tipoDelega, idRuoloOld, idUtenteOld, dataScadenzaOld, dataDecorrenzaOld, idRuoloDeleganteOld))
                {
                    this.CloseMask(true);
                }
                else
                {
                    //Se la modifica non è andata a buon fine, ripristino i valori originali
                    this.DelegaSel.dataScadenza = dataScadenzaOld;
                    this.DelegaSel.dataDecorrenza = dataDecorrenzaOld;
                    this.DelegaSel.id_ruolo_delegato = idRuoloOld;
                    this.DelegaSel.id_utente_delegato = idUtenteOld;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorMandateUpdating', 'error', '');", true);
                    return;
                }
            }

        }

    }


}