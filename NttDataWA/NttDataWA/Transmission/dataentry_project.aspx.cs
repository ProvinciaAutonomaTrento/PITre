using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDatalLibrary;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;
using log4net;

namespace NttDataWA.Transmission
{
    public partial class dataentry_project : System.Web.UI.Page
    {
        #region fields
        //private string PROTOCOL = "P";
        //private string NOTPROTOCOL = "N";
        private string idProfile = string.Empty;
        private string idTransmission = string.Empty;
        private string docNumber = string.Empty;
        protected DocsPaWR.TrasmissioneSingola[] listTrasmSing;
        private ILog logger = LogManager.GetLogger(typeof(dataentry_project));
        private ArrayList trasm_strutture_vuote = new ArrayList();
        protected int numeroRuoliDestInTrasmissione = 0;
        protected int numeroUtentiConNotifica = 0;
        protected string idPeopleNewOwner = string.Empty;

        //Andrea
        private string messError = "";
        private string MessError_DestPerCodice = "";
        private ArrayList listaExceptionTrasmissioni1 = new ArrayList();
        private ArrayList listaExceptionTrasmissioni = new ArrayList();
        //End Andrea

        #endregion

        #region properties

        private DocsPaWR.InfoUtente InfoUser
        {
            get
            {
                DocsPaWR.InfoUtente result = null;
                if (HttpContext.Current.Session["infoUser"] != null)
                {
                    result = HttpContext.Current.Session["infoUser"] as DocsPaWR.InfoUtente;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["infoUser"] = value;
            }
        }

        private DocsPaWR.Utente UserLog
        {
            get
            {
                DocsPaWR.Utente result = null;
                if (HttpContext.Current.Session["user"] != null)
                {
                    result = HttpContext.Current.Session["user"] as DocsPaWR.Utente;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["user"] = value;
            }
        }

        private DocsPaWR.Ruolo Role
        {
            get
            {
                DocsPaWR.Ruolo result = null;
                if (HttpContext.Current.Session["role"] != null)
                {
                    result = HttpContext.Current.Session["role"] as DocsPaWR.Ruolo;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["role"] = value;
            }
        }

        private DocsPaWR.Registro Registry
        {
            get
            {
                DocsPaWR.Registro result = null;
                if (HttpContext.Current.Session["registry"] != null)
                {
                    result = HttpContext.Current.Session["registry"] as DocsPaWR.Registro;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["registry"] = value;
            }
        }

        private DocsPaWR.Trasmissione Transmission
        {
            get
            {
                DocsPaWR.Trasmissione result = null;
                if (HttpContext.Current.Session["Transmission_dataentry"] != null)
                {
                    result = HttpContext.Current.Session["Transmission_dataentry"] as DocsPaWR.Trasmissione;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["Transmission_dataentry"] = value;
            }
        }

        private DocsPaWR.ModelloTrasmissione Template
        {
            get
            {
                DocsPaWR.ModelloTrasmissione result = null;
                if (HttpContext.Current.Session["Transmission_template"] != null)
                {
                    result = HttpContext.Current.Session["Transmission_template"] as DocsPaWR.ModelloTrasmissione;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["Transmission_template"] = value;
            }
        }

        private bool SaveButNotTransmit
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["SaveButNotTransmit"] != null)
                {
                    result = (bool)HttpContext.Current.Session["SaveButNotTransmit"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["SaveButNotTransmit"] = value;
            }
        }

        private bool ViewCodeTransmissionModels
        {
            get
            {
                bool result = false;
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.VISUALIZZA_CODICE_MODELLI_TRASM.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.VISUALIZZA_CODICE_MODELLI_TRASM.ToString()].Equals("1")) result = false;
                return result;
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

        private bool EnableDistributionLists
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableDistributionLists"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableDistributionLists"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableDistributionLists"] = value;
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

        private int NoteCharsAvailable
        {
            get
            {
                int result = 250;
                if (HttpContext.Current.Session["NoteCharsAvailable"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["NoteCharsAvailable"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["NoteCharsAvailable"] = value;
            }
        }

        private bool TransferRightsInRole
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["TransferRightsInRole"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["TransferRightsInRole"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferRightsInRole"] = value;
            }
        }

        private bool ShowsUserLocation
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["ShowsUserLocation"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["ShowsUserLocation"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ShowsUserLocation"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
                idTransmission = (this.Request.QueryString["idTransmission"] != null && this.Request.QueryString["idTransmission"].Length > 0) ? this.Request.QueryString["idTransmission"] : "";


                if (!IsPostBack)
                {
                    this.InfoUser = UIManager.UserManager.GetInfoUser();
                    this.UserLog = UIManager.UserManager.GetUserInSession();
                    this.Role = UIManager.RoleManager.GetRoleInSession();

                    this.Transmission = null;
                    this.Template = null;
                    this.releaseMemoriaNotificaUt();

                    Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
                    this.litDescription.Text = Server.HtmlEncode(fascicolo.descrizione);

                    //se è null la trasmissione è nuova altrimenti è in modifica
                    //string tipo =
                    this.Transmission = TrasmManager.GetTransmission(this, idTransmission, "F");
                    string language = UIManager.UserManager.GetUserLanguage();
                    if (this.Transmission == null)
                    {
                        this.Transmission = new DocsPaWR.Trasmissione();
                        this.Transmission.tipoOggetto = TrasmissioneTipoOggetto.FASCICOLO;
                        this.Transmission.systemId = null;
                        this.Transmission.ruolo = this.Role;
                        this.Transmission.utente = UserManager.GetUserInSession();
                        this.litPageTitle.Text = Utils.Languages.GetLabelFromCode("TransmissionNewTitleTxt", language).ToUpper();
                    }
                    else
                    {
                        this.gestioneMemoriaNotificaUt();
                        this.Transmission.daAggiornare = true;
                        this.litPageTitle.Text = Utils.Languages.GetLabelFromCode("TransmissionEditTitleTxt", language).ToUpper();
                        this.TxtNote.Text = this.Transmission.noteGenerali;
                        this.ShowTransmissions();
                    }

                    this.InitializePage();
                    this.VisibiltyRoleFunctions();
                }
                else
                {
                    if (((ScriptManager)Master.FindControl("ScriptManager1")).IsInAsyncPostBack)
                    {
                        // detect action from async postback
                        switch (((ScriptManager)Master.FindControl("ScriptManager1")).AsyncPostBackSourceElementID)
                        {
                            case "uppnlReason":
                                UpdateReasonInfo();
                                RefreshCheckbox();
                                EnableRecipient();
                                this.ReApplyScripts();
                                return;
                            case "UpPnlRecipient":
                                this.BuildTransmission();
                                  this.ResetFields();
                                this.EnableButtons();
                                this.ReApplyScripts();
                                return;
                            case "upPnlTransmissionsModel":
                                if (!string.IsNullOrEmpty(this.DdlTransmissionsModel.SelectedValue))
                                {
                                    this.BuildTransmissionFromModel();
                                    this.ShowTransmissions();
                                    this.EnableButtons();
                                }

                                this.LoadTransmissionModels();
                                this.upPnlTransmissionsModel.Update();
                                this.ReApplyScripts();

                                return;
                            case "upPnlTransmissionSaved":
                                if (this.Transmission.cessione != null && this.Transmission.cessione.docCeduto && !string.IsNullOrEmpty(this.Transmission.dataInvio))
                                    if (!this.Transmission.mantieniLettura)
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}", true);
                                    else
                                    {
                                        ProjectManager.setProjectInSession(ProjectManager.GetProjectByCode(RegistryManager.getRegistroBySistemId(ProjectManager.getProjectInSession().idRegistro), ProjectManager.getProjectInSession().codice));
                                        Response.Redirect("../Project/TransmissionsP.aspx");
                                    }
                                else
                                    Response.Redirect("../Project/TransmissionsP.aspx");
                                return;
                        }

                        if (((ScriptManager)Master.FindControl("ScriptManager1")).AsyncPostBackSourceElementID.IndexOf("btnAddressBookPostback") > 0)
                        {
                            List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                            bool esternoPresenteInLista = false;
                            if (atList != null && atList.Count > 0)
                            {
                                foreach (NttDataWA.Popup.AddressBook.CorrespondentDetail addressBookCorrespondent in atList)
                                {
                                    this.TxtCodeRecipientTransmission.Text = addressBookCorrespondent.CodiceRubrica;
                                    this.TxtDescriptionRecipient.Text = addressBookCorrespondent.Descrizione;
                                    this.IdRecipient.Value = addressBookCorrespondent.SystemID;

                                    if (addressBookCorrespondent.Tipo == "L")
                                    {

                                        string idAmm = UserManager.GetInfoUser().idAmministrazione;
                                        ArrayList listaCorr = UserManager.getCorrispondentiByCodLista(this.Page, addressBookCorrespondent.CodiceRubrica, idAmm);
                                        if (listaCorr != null && listaCorr.Count > 0)
                                        {
                                            DocsPaWR.Trasmissione trasmissioni = this.Transmission;

                                            ElementoRubrica[] ers = new ElementoRubrica[listaCorr.Count];

                                            for (int i = 0; i < listaCorr.Count; i++)
                                            {
                                                DocsPaWR.ElementoRubrica er_1 = new DocsPaWR.ElementoRubrica();
                                                DocsPaWR.Corrispondente c = (DocsPaWR.Corrispondente)listaCorr[i];
                                                er_1 = UserManager.getElementoRubrica(this, c.codiceRubrica);
                                                if (er_1 != null && !er_1.disabledTrasm)
                                                    ers[i] = er_1;
                                            }
                                            int coutStartErs = ers.Length;

                                            //Andrea
                                            for (int i = 0; i < listaCorr.Count; i++)
                                            {
                                                DocsPaWR.Corrispondente c = (DocsPaWR.Corrispondente)listaCorr[i];
                                                if (c != null && !string.IsNullOrEmpty(c.tipoIE) && c.tipoIE == "I")
                                                {
                                                    if (c != null && !string.IsNullOrEmpty(c.codiceRubrica))
                                                    {
                                                        trasmissioni = addTrasmissioneSingola(this.Transmission, c);
                                                    }
                                                }
                                                else esternoPresenteInLista = true;
                                            }

                                            foreach (string s in listaExceptionTrasmissioni1)
                                            {
                                                MessError_DestPerCodice = MessError_DestPerCodice + s + "\\n";
                                            }

                                            if (listaExceptionTrasmissioni1 != null && listaExceptionTrasmissioni1.Count != 0)
                                            {
                                                listaExceptionTrasmissioni1 = new ArrayList();
                                            }

                                            if (MessError_DestPerCodice != "")
                                            {
                                                Session.Add("MessError_DestPerCodice", MessError_DestPerCodice);
                                            }
                                            //End Andrea

                                            this.Transmission = trasmissioni;

                                            this.ShowTransmissions();
                                        }
                                    }
                                    else
                                    {
                                        this.BuildTransmission();
                                    }
                                    //corr = new Corrispondente();
                                    //corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);


                                    this.TxtCodeRecipientTransmission.Text = string.Empty;
                                    this.TxtDescriptionRecipient.Text = string.Empty;
                                    this.IdRecipient.Value = string.Empty;
                                }
                            }

                            HttpContext.Current.Session["AddressBook.At"] = null;
                            HttpContext.Current.Session["AddressBook.Cc"] = null;
                            this.ResetFields();
                            this.EnableButtons();
                            if (esternoPresenteInLista)
                            {
                                string msgDesc = "WarningDocumentCorrExtInListProtInt";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);

                            }
                            this.UpPnlRecipient.Update();
                        }

                        if (
                    ((ScriptManager)Master.FindControl("ScriptManager1")).AsyncPostBackSourceElementID == this.TxtCodeRecipientTransmission.ClientID
                    || ((ScriptManager)Master.FindControl("ScriptManager1")).AsyncPostBackSourceElementID == this.TxtDescriptionRecipient.ClientID
                    || ((ScriptManager)Master.FindControl("ScriptManager1")).AsyncPostBackSourceElementID.IndexOf("TxtCodeRecipientTransmission") > 0
                    || ((ScriptManager)Master.FindControl("ScriptManager1")).AsyncPostBackSourceElementID.IndexOf("TxtDescriptionRecipient") > 0
                    || ((ScriptManager)Master.FindControl("ScriptManager1")).AsyncPostBackSourceElementID.IndexOf("btnRecipient") > 0
                    )
                        {

                            CustomTextArea caller = sender as CustomTextArea;
                            string codeAddressBook = this.TxtCodeRecipientTransmission.Text;

                            if (!string.IsNullOrEmpty(codeAddressBook))
                            {
                                this.SearchCorrespondent(codeAddressBook, this.TxtCodeRecipientTransmission.ID);
                            }
                            //else
                            //{
                            //    string msg = "ErrorTransmissionCorrespondentNotFound";
                            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            //}

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "focus", "$('#" + this.TxtNote.ClientID + "').focus();", true);
                            this.ShowTransmissions();
                            this.EnableButtons();
                            this.ReApplyScripts();
                            return;
                        }

                        // detect action from popup confirm
                        if (this.proceed_personal.Value == "true") { this.BeginTransmissionButCheckOwner(); return; }
                        if (this.proceed_private.Value == "true") { this.BeginTransmissionButCheckOwner(); return; }
                        if (this.proceed_ownership.Value == "true") { this.BeginTransmissionIfAllowed(); return; }

                        // reset hidden forms
                        this.proceed_personal.Value = "";
                        this.proceed_private.Value = "";
                        this.proceed_ownership.Value = "";
                    }

                    this.ShowTransmissions();
                    this.EnableButtons();
                    this.ReApplyScripts();
                }
        }

        protected void RefreshCheckbox()
        {
            try
            {
                this.ShowTransmissions();

                Table tbl = ((Table)GetControlById(this.plcTransmissions, "tblTransmissions"));
                ArrayList selectedID = new ArrayList();
                if (tbl != null)
                {

                    foreach (TableRow row in tbl.Rows)
                    {
                        if (row.ID != null && row.ID.StartsWith("trasmDetails_"))
                        {
                            CheckBox chk = ((CheckBox)GetControlById(row, row.ID.Replace("trasmDetails_", "chkCanc_")));
                            if (chk.Checked) selectedID.Add(chk.ID.Replace("chkCanc_", ""));
                        }
                    }
                }

                //if (selectedID.Count > 0)
                //{
                //    for (int i = selectedID.Count - 1; i >= 0; i--)
                //    {
                //        setDaEliminare = (this.Transmission.trasmissioniSingole[i].systemId != null && !this.Transmission.trasmissioniSingole[i].systemId.Equals(""));

                //        if (this.Transmission.systemId != null)  // sono in "Modifica"
                //            setDaEliminare = false;

                //        this.Transmission = TrasmManager.removeTrasmSingola(this.Transmission, Convert.ToInt32(selectedID[i]), setDaEliminare);

                //        this.removeHashNotificaUt(Convert.ToInt32(selectedID[i]));
                //    }

                //    ShowTransmissions();
                //}
                //else
                //{
                //    string msg = "ErrorTransmissionNoneSelected";
                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                //}

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void VisibiltyRoleFunctions()
        {
            if (!UIManager.UserManager.IsAuthorizedFunctions("TRAS_NUOVA_DA_TEMPL"))
            {
                this.PnlTransmissionTemplates.Visible = false;
                this.upPnlTransmissionTemplates.Update();
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("TRAS_SALVA_TEMPL"))
            {
                this.TransmissionsBtnSaveTemplate.Visible = false;
                this.panelButtons.Update();
            }
        }

        protected void InitializePage()
        {
            this.InitializeLanguage();
            this.LoadKeys();
            this.LoadTransmissionModels();
            this.LoadReasons();
            this.InitializeAddressBooks();
            this.EnableTransferRights();
            this.EnableRecipient();
            this.EnableButtons();
            this.ResetFields();

            this.TxtNote.MaxLength = this.NoteCharsAvailable;
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.TemplateSave.Title = Utils.Languages.GetLabelFromCode("TransmissionTemplateSave", language);
            this.TemplateSaveNewOwner.Title = Utils.Languages.GetLabelFromCode("TransmissionTemplateSaveNewOwner", language);
            this.SaveNewOwner.Title = Utils.Languages.GetLabelFromCode("TransmissionSaveNewOwner", language);
            this.TransmitNewOwner.Title = Utils.Languages.GetLabelFromCode("TransmissionTransmitNewOwner", language);
            this.litDescriptionText.Text = Utils.Languages.GetLabelFromCode("TransmissionLitDescription", language);
            this.TransmissionLitReason.Text = Utils.Languages.GetLabelFromCode("TransmissionLitReason", language);
            this.ddlReason.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("TransmissionLitReasonExtended", language);
            this.chkTransferRights.Text = Utils.Languages.GetLabelFromCode("TransmissionChkTransferRights", language);
            this.TransmissionLitRecipient.Text = Utils.Languages.GetLabelFromCode("TransmissionLitRecipient", language);
            this.TransmissionLitNote.Text = Utils.Languages.GetLabelFromCode("TransmissionLitNote", language);
            this.TransmissionLitTemplates.Text = Utils.Languages.GetLabelFromCode("TransmissionLitTemplates", language);
            this.DdlTransmissionsModel.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("TransmissionDdlTransmissionsModel", language);
            this.TransmissionsBtnTransmit.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnTransmit", language);
            this.TransmissionsBtnRemove.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnRemove", language);
            this.TransmissionsBtnSave.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnSave", language);
            this.TransmissionsBtnSaveTemplate.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnSaveTemplate", language);
            this.dtEntryDocImgAddressBookUser.AlternateText = Utils.Languages.GetLabelFromCode("dtEntryDocImgAddressBookUser", language);
            this.dtEntryDocImgAddressBookUser.ToolTip = Utils.Languages.GetLabelFromCode("dtEntryDocImgAddressBookUser", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
        }

        private string GetLabel(string labelId)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(labelId, language);
        }

        private void ReApplyScripts()
        {
            this.ReApplyChosenScript();
            this.ReApplyDatePickerScript();
            this.ReApplyTipsyScript();
        }

        protected void LoadKeys()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_NEW_RUBRICA_VELOCE.ToString())) && (Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_NEW_RUBRICA_VELOCE.ToString()).Equals("1")))
            {
                this.EnableAjaxAddressBook = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]))
            {
                this.AjaxAddressBookMinPrefixLenght = int.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]);
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LISTE_DISTRIBUZIONE.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LISTE_DISTRIBUZIONE.ToString()].Equals("1"))
            {
                this.EnableDistributionLists = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_MAX_LENGTH_DESC_TRASM.ToString())))
            {
                this.NoteCharsAvailable = int.Parse(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_MAX_LENGTH_DESC_TRASM.ToString()));
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(this.InfoUser.idAmministrazione, DBKeys.FE_CEDI_DIRITTI_IN_RUOLO.ToString())))
            {
                this.TransferRightsInRole = true;
            }

        }

        private void EnableTransferRights()
        {
            this.parTransferRights.Visible = this.checkIsAutorizedEditingACL();
        }

        /// <summary>
        /// GESTIONE CESSIONE DIRITTI:
        /// verifica se l'utente è abilitato alla funzione ABILITA_CEDI_DIRITTI_FASC
        /// </summary>
        private bool checkIsAutorizedEditingACL()
        {
            return UserManager.IsAuthorizedFunctions("ABILITA_CEDI_DIRITTI_FASC");
        }

        protected void InitializeAddressBooks()
        {
            this.SetAjaxAddressBook();
        }

        protected void SetAjaxAddressBook()
        {
            this.RapidRecipient.MinimumPrefixLength = this.AjaxAddressBookMinPrefixLenght;

            string dataUser = this.Role.systemId;
            if (this.Registry == null)
            {
                this.Registry = RegistryManager.GetRegistryInSession();
            }
            dataUser = dataUser + "-" + this.Registry.systemId;
            RubricaCallType callTypeAdd = GetCallType("ajax");
            string callType = callTypeAdd.ToString(); // Destinatario su protocollo interno
            this.RapidRecipient.ContextKey = dataUser + "-" + this.UserLog.idAmministrazione + "-" + callType;
        }

        private void EnableRecipient()
        {
            EnableRecipient(!string.IsNullOrEmpty(this.ddlReason.SelectedValue));
        }

        private void EnableRecipient(bool value)
        {
            this.TxtCodeRecipientTransmission.Enabled = value;
            this.TxtDescriptionRecipient.Enabled = value;

            this.dtEntryDocImgAddressBookUser.Enabled = value;
            this.UpPnlIconAddress.Update();

            this.SetAjaxAddressBook();
            this.UpPnlRecipient.Update();
        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            //try {
            //    CustomTextArea caller = sender as CustomTextArea;
            //    string codeAddressBook = this.TxtCodeRecipientTransmission.Text;

            //    if (!string.IsNullOrEmpty(codeAddressBook))
            //    {
            //        this.SearchCorrespondent(codeAddressBook, caller.ID);
            //    }
            //    else
            //    {
            //        string msg = "ErrorTransmissionCorrespondentNotFound";
            //        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
            //    }

            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "focus", "$('#" + this.TxtNote.ClientID + "').focus();", true);
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        protected RubricaCallType GetCallType(string idControl)
        {
            RubricaCallType calltype = new RubricaCallType();

            if (!string.IsNullOrEmpty(this.ddlReason.SelectedValue))
            {
                RagioneTrasmissione rt = TrasmManager.getReasonById(this.ddlReason.SelectedValue);
                switch (rt.tipoDestinatario.ToString("g").Substring(0, 1))
                {
                    case "T":
                        calltype = DocsPaWR.RubricaCallType.CALLTYPE_TRASM_ALL;
                        break;
                    case "I":
                        calltype = DocsPaWR.RubricaCallType.CALLTYPE_TRASM_INF;
                        break;
                    case "S":
                        calltype = DocsPaWR.RubricaCallType.CALLTYPE_TRASM_SUP;
                        break;
                    case "P":
                        calltype = DocsPaWR.RubricaCallType.CALLTYPE_TRASM_PARILIVELLO;
                        break;
                }
            }

            return calltype;
        }

        protected void SearchCorrespondent(string addressCode, string idControl)
        {
            bool esternoPresenteInLista = false;
            DocsPaWR.ParametriRicercaRubrica qco = new DocsPaWR.ParametriRicercaRubrica();
            AddressBookManager.setQueryRubricaCaller(ref qco);
            qco.codice = addressCode.Trim();
            qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
            //cerco su tutti i tipi utente:
            if (System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LISTE_DISTRIBUZIONE.ToString()] != null && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LISTE_DISTRIBUZIONE.ToString()] == "1")
                qco.doListe = true;
            qco.doRuoli = true;
            qco.doUtenti = true;
            qco.doUo = true;
            qco.doRF = false;
            //query per codice  esatta, no like.
            qco.calltype = this.GetCallType(idControl);
            qco.queryCodiceEsatta = true;

            Fascicolo fasc = ProjectManager.getProjectInSession();
            if (fasc != null)
            {
                qco.ObjectType = "F:" + fasc.idClassificazione;

                ArrayList listaCorr = UserManager.getCorrispondentiByCodLista(this.Page, addressCode.Trim(), UIManager.UserManager.GetInfoUser().idAmministrazione);
                if (listaCorr != null && listaCorr.Count > 0)
                {
                    DocsPaWR.Trasmissione trasmissioni = this.Transmission;

                    ElementoRubrica[] ers = new ElementoRubrica[listaCorr.Count];

                    for (int i = 0; i < listaCorr.Count; i++)
                    {
                        DocsPaWR.ElementoRubrica er_1 = new DocsPaWR.ElementoRubrica();
                        DocsPaWR.Corrispondente c = (DocsPaWR.Corrispondente)listaCorr[i];
                        er_1 = UserManager.getElementoRubrica(this, c.codiceRubrica);
                        if (er_1 != null && !er_1.disabledTrasm)
                            ers[i] = er_1;
                    }
                    int coutStartErs = ers.Length;

                    //Andrea
                    for (int i = 0; i < listaCorr.Count; i++)
                    {
                        DocsPaWR.Corrispondente c = (DocsPaWR.Corrispondente)listaCorr[i];
                       if (c!=null && !string.IsNullOrEmpty(c.tipoIE) && c.tipoIE.ToUpper() == "I")
                     {
                         if (c != null && !string.IsNullOrEmpty(c.codiceRubrica) && !c.disabledTrasm)
                         {
                             trasmissioni = addTrasmissioneSingola(this.Transmission, c);
                         }
                     }
                     else esternoPresenteInLista = true;
                    }

                    foreach (string s in listaExceptionTrasmissioni1)
                    {
                        MessError_DestPerCodice = MessError_DestPerCodice + s + "\\n";
                    }

                    if (listaExceptionTrasmissioni1 != null && listaExceptionTrasmissioni1.Count != 0)
                    {
                        listaExceptionTrasmissioni1 = new ArrayList();
                    }

                    if (MessError_DestPerCodice != "")
                    {
                        Session.Add("MessError_DestPerCodice", MessError_DestPerCodice);
                    }
                    //End Andrea

                    this.Transmission = trasmissioni;

                    this.ShowTransmissions();
                    this.ResetFields();
                    this.EnableButtons();
                }
                else
                {

                    DocsPaWR.ElementoRubrica[] corrSearch = UserManager.getElementiRubrica(this.Page, qco);

                    if (corrSearch != null && corrSearch.Length > 0)
                    {
                        if (corrSearch[0].tipo != "R" || (corrSearch[0].tipo == "R" && !corrSearch[0].disabledTrasm))
                        {
                            this.TxtCodeRecipientTransmission.Text = corrSearch[0].codice;
                            this.TxtDescriptionRecipient.Text = corrSearch[0].descrizione;
                            this.IdRecipient.Value = corrSearch[0].systemId;
                            this.BuildTransmission();
                            this.ResetFields();
                            this.EnableButtons();
                        }
                        else
                        {

                            this.TxtCodeRecipientTransmission.Text = string.Empty;
                            this.TxtDescriptionRecipient.Text = string.Empty;
                            this.IdRecipient.Value = string.Empty;
                            string msg = "ErrorTransmissionCorrespondentNotFound";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                        }
                    }
                }
            }
            if (esternoPresenteInLista)
            {
                string msgDesc = "WarningDocumentCorrExtInListProtInt";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);

            }
            this.UpPnlRecipient.Update();
        }

        private void BuildTransmission()
        {
            if (!string.IsNullOrEmpty(this.IdRecipient.Value))
            {
            //    DocsPaWR.ParametriRicercaRubrica qco = new DocsPaWR.ParametriRicercaRubrica();
            //    UserManager.setQueryRubricaCaller(ref qco);
            //    qco.codice = this.TxtCodeRecipientTransmission.Text.Trim();
            //    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
            //    qco.doRuoli = true;
            //    qco.doUtenti = true;
            //    qco.doUo = true;
            //    qco.doRF = false;
            //    qco.queryCodiceEsatta = true;

                DocsPaWR.ParametriRicercaRubrica qco = new DocsPaWR.ParametriRicercaRubrica();
                AddressBookManager.setQueryRubricaCaller(ref qco);
                qco.codice = this.TxtCodeRecipientTransmission.Text.Trim();
                qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                //cerco su tutti i tipi utente:
                if (this.EnableDistributionLists)
                    qco.doListe = true;
                qco.doRuoli = true;
                qco.doUtenti = true;
                qco.doUo = true;
                qco.doRF = false;
                //query per codice  esatta, no like.
                qco.queryCodiceEsatta = true;

                // cerco su tutti i tipi utente
                if (this.EnableDistributionLists) qco.doListe = true;

                DocsPaWR.RagioneTrasmissione rt = TrasmManager.getReasonById(this.ddlReason.SelectedValue);
                string gerarchia_trasm = rt.tipoDestinatario.ToString("g").Substring(0, 1);

                switch (gerarchia_trasm)
                {
                    case "T":
                        qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_TRASM_ALL;
                        break;
                    case "I":
                        qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_TRASM_INF;
                        break;
                    case "S":
                        qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_TRASM_SUP;
                        break;
                    case "P":
                        qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_TRASM_PARILIVELLO;
                        break;
                }

                Fascicolo prj = ProjectManager.getProjectInSession();
                if (prj != null)
                {
                    qco.ObjectType = "F:" + prj.idClassificazione;


                    DocsPaWR.ElementoRubrica[] corrSearch = UserManager.getElementiRubrica(this.Page, qco);
                    if (corrSearch != null && corrSearch.Length > 0)
                    {
                        if (corrSearch[0].tipo != "R" || (corrSearch[0].tipo == "R" && !corrSearch[0].disabledTrasm))
                        {
                            this.ImpostaDestinatario(corrSearch, qco);
                            this.ReApplyScripts();
                        }
                        else
                        {
                            string msg = "ErrorTransmissionRoleDisabled";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                    }
                    else
                    {
                        string msg = "ErrorTransmissionNoneRecipient";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }

                    corrSearch = null;
                }
            }

          
        }

        private void ImpostaDestinatario(DocsPaWR.ElementoRubrica[] corrSearch, DocsPaWR.ParametriRicercaRubrica prr)
        {
            string t_avviso = string.Empty;
            DocsPaWR.Corrispondente corr;

            // verifica liste di distribuzione
            if (corrSearch[0].tipo.Equals("L"))
            {
                if (this.EnableDistributionLists)
                {
                    string idAmm = UserManager.GetInfoUser().idAmministrazione;
                    ArrayList listaCorr = UserManager.getCorrispondentiByCodLista(this.Page, prr.codice, idAmm);
                    if (listaCorr != null && listaCorr.Count > 0)
                    {
                        DocsPaWR.Trasmissione trasmissioni = this.Transmission;

                        ElementoRubrica[] ers = new ElementoRubrica[listaCorr.Count];

                        for (int i = 0; i < listaCorr.Count; i++)
                        {
                            DocsPaWR.ElementoRubrica er_1 = new DocsPaWR.ElementoRubrica();
                            DocsPaWR.Corrispondente c = (DocsPaWR.Corrispondente)listaCorr[i];
                            er_1 = UserManager.getElementoRubrica(this, c.codiceRubrica);
                            if (er_1 != null && !er_1.disabledTrasm)
                                ers[i] = er_1;
                        }
                        int coutStartErs = ers.Length;

                        //Andrea
                        for (int i = 0; i < listaCorr.Count; i++)
                        {
                            DocsPaWR.Corrispondente c = (DocsPaWR.Corrispondente)listaCorr[i];
                            if (c != null && !string.IsNullOrEmpty(c.codiceRubrica))
                            {
                                trasmissioni = addTrasmissioneSingola(this.Transmission, c);
                            }
                        }

                        foreach (string s in listaExceptionTrasmissioni1)
                        {
                            MessError_DestPerCodice = MessError_DestPerCodice + s + "\\n";
                        }

                        if (listaExceptionTrasmissioni1 != null && listaExceptionTrasmissioni1.Count != 0)
                        {
                            listaExceptionTrasmissioni1 = new ArrayList();
                        }

                        if (MessError_DestPerCodice != "")
                        {
                            Session.Add("MessError_DestPerCodice", MessError_DestPerCodice);
                        }
                        //End Andrea

                        this.Transmission = trasmissioni;
                    }
                }
                else
                {
                    string msg = "ErrorTransmissionListsDisabled";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }
            }
            else
            {
                if (corrSearch[0].tipo.Equals("F"))
                {
                    ArrayList listaCorr = UserManager.getCorrispondentiByCodRF(this.Page, corrSearch[0].codice);
                    if (listaCorr != null && listaCorr.Count > 0)
                    {
                        DocsPaWR.Trasmissione trasmissioni = this.Transmission;
                        ElementoRubrica[] ers = new ElementoRubrica[listaCorr.Count];
                        for (int i = 0; i < listaCorr.Count; i++)
                        {
                            DocsPaWR.ElementoRubrica er_1 = new DocsPaWR.ElementoRubrica();
                            DocsPaWR.Corrispondente c = (DocsPaWR.Corrispondente)listaCorr[i];
                            er_1 = UserManager.getElementoRubrica(this.Page, c.codiceRubrica);
                            ers[i] = er_1;
                        }
                        int coutStartErs = ers.Length;


                        //Andrea
                        for (int i = 0; i < listaCorr.Count; i++)
                        {
                            DocsPaWR.Corrispondente c = (DocsPaWR.Corrispondente)listaCorr[i];
                            trasmissioni = addTrasmissioneSingola(this.Transmission, c);
                        }

                        foreach (string s in listaExceptionTrasmissioni1)
                        {
                            MessError_DestPerCodice = MessError_DestPerCodice + s + "\\n";
                        }

                        if (listaExceptionTrasmissioni1 != null && listaExceptionTrasmissioni1.Count != 0)
                        {
                            listaExceptionTrasmissioni1 = new ArrayList();
                        }

                        if (MessError_DestPerCodice != "")
                        {
                            Session.Add("MessError_DestPerCodice", MessError_DestPerCodice);
                        }
                        //End Andrea

                        this.Transmission = trasmissioni;
                    }
                }
                else
                {
                    DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPaWR.AddressbookQueryCorrispondente();
                    qco.codiceRubrica = prr.codice;
                    qco.getChildren = false;
                    qco.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                    qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.fineValidita = true;

                    Corrispondente[] list = UserManager.getListaCorrispondenti(this.Page, qco);
                    if (list == null || list.Length > 0)
                    {
                        corr = UserManager.getListaCorrispondenti(this.Page, qco)[0];
                    }
                    else
                    {
                        string msg = "ErrorTransmissionNoUser";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }


                    this.Transmission = this.addTrasmissioneSingola(this.Transmission, corr);
                }
            }


            //Andrea
            foreach (string s in listaExceptionTrasmissioni1)
            {
                MessError_DestPerCodice = MessError_DestPerCodice + s + "\\n";
            }

            if (listaExceptionTrasmissioni1 != null && listaExceptionTrasmissioni1.Count != 0)
            {
                listaExceptionTrasmissioni1 = new ArrayList();
            }

            if (MessError_DestPerCodice != "")
            {
                Session.Add("MessError_DestPerCodice", MessError_DestPerCodice);
            }
            //End Andrea


            this.ShowTransmissions();
        }

        private void BuildTransmissionFromModel()
        {
            if (!string.IsNullOrEmpty(this.DdlTransmissionsModel.SelectedValue))
            {
                this.Template = TransmissionModelsManager.GetTemplateById(UserManager.GetInfoUser().idAmministrazione, this.DdlTransmissionsModel.SelectedValue);

                if (this.Template != null)
                    this.Transmission.NO_NOTIFY = this.Template.NO_NOTIFY;

                // prima di aggiungere il modello verifico le ragioni cessione
                if (this.esisteRagTrasmCessioneInTemplate(this.Template))
                {
                    this.DdlTransmissionsModel.SelectedIndex = 0;
                    this.upPnlTransmissionsModel.Update();
                    return;
                }

                // gestione della cessione diritti
                if (this.Template.CEDE_DIRITTI != null && this.Template.CEDE_DIRITTI.Equals("1"))
                {
                    DocsPaWR.CessioneDocumento objCessione = new DocsPaWR.CessioneDocumento();

                    objCessione.docCeduto = true;
                    objCessione.idPeople = UserManager.GetInfoUser().idPeople;
                    objCessione.idRuolo = UserManager.GetInfoUser().idGruppo;
                    objCessione.idPeopleNewPropr = this.Template.ID_PEOPLE_NEW_OWNER;
                    objCessione.idRuoloNewPropr = this.Template.ID_GROUP_NEW_OWNER;
                    objCessione.userId = UserManager.GetInfoUser().userId;

                    if (!string.IsNullOrEmpty(this.Template.MANTIENI_SCRITTURA) && !string.IsNullOrEmpty(this.Template.MANTIENI_LETTURA))
                        if (Convert.ToBoolean(int.Parse(this.Template.MANTIENI_SCRITTURA)))
                        {
                            this.Transmission.mantieniLettura = true;
                            this.Transmission.mantieniScrittura = true;
                        }
                        else
                        {
                            this.Transmission.mantieniScrittura = false;
                            if (Convert.ToBoolean(int.Parse(this.Template.MANTIENI_LETTURA)))
                                this.Transmission.mantieniLettura = true;
                            else
                                this.Transmission.mantieniLettura = false;
                        }

                    this.Transmission.cessione = objCessione;
                }

                //Parametri della trasmissione
                this.Transmission.noteGenerali = this.Template.VAR_NOTE_GENERALI;

                this.Transmission.tipoOggetto = DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
                this.Transmission.infoFascicolo = ProjectManager.getInfoFascicoloDaFascicolo(UIManager.ProjectManager.getProjectInSession());

                //Parametri delle trasmissioni singole
                for (int i = 0; i < this.Template.RAGIONI_DESTINATARI.Length; i++)
                {
                    DocsPaWR.RagioneDest ragDest = (DocsPaWR.RagioneDest)this.Template.RAGIONI_DESTINATARI[i];
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {

                        DocsPaWR.MittDest mittDest = (DocsPaWR.MittDest)destinatari[j];
                        DocsPaWR.RagioneTrasmissione ragione = TrasmManager.getReasonById(mittDest.ID_RAGIONE.ToString());
                        DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                        if (corr != null) //corr nullo se non esiste o se è stato disabilitato                           

                            //Andrea - try - catch
                            try
                            {
                                this.Transmission = addTrasmissioneSingola(this.Transmission, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, mittDest);
                            }
                            catch (ExceptionTrasmissioni e)
                            {
                                //Aggiungo l'errore alla lista
                                listaExceptionTrasmissioni.Add(e.Messaggio);
                            }
                        //End Andrea
                    }

                }

                //Andrea
                foreach (string s in listaExceptionTrasmissioni) messError = messError + s + "\\n";

                if (messError != "")
                {
                    Session.Add("MessError", messError);
                }
                //End Andrea

                if (this.Template.CEDE_DIRITTI != null && this.Template.CEDE_DIRITTI.Equals("1"))
                    this.Transmission.trasmissioniSingole[0].ragione.cessioneImpostata = true;


                ResetFields();
                EnableButtons();
            }
        }

        private void ResetFields()
        {
            this.ddlReason.SelectedIndex = -1;
            this.TxtCodeRecipientTransmission.Text = string.Empty;
            this.TxtDescriptionRecipient.Text = string.Empty;
            this.IdRecipient.Value = string.Empty;
            this.DdlTransmissionsModel.SelectedIndex = -1;
            this.imgReason.AlternateText = string.Empty;
            this.imgReason.ToolTip = string.Empty;

            this.EnableRecipient();
            this.uppnlReason.Update();
            this.upPnlGeneral.Update();
            this.ReApplyScripts();
        }

        private ArrayList GetTransmissionModels()
        {
            string idDiagram = string.Empty;
            string idState = string.Empty;
            string idFasc = string.Empty;
            string accessRights = string.Empty;
            string idTipoFasc = string.Empty;


            Fascicolo prj = UIManager.ProjectManager.getProjectInSession();

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID))
            {
                accessRights = ProjectManager.getProjectInSession().accessRights;
                idFasc = ProjectManager.getProjectInSession().systemID;
            }



            if ((!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamicaFasc.ToString()])
                && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamicaFasc.ToString()].Equals("1")) &&
                prj != null && !string.IsNullOrEmpty(prj.systemID) && prj.template != null && prj.template.SYSTEM_ID != 0)
            {
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()].Equals("1"))
                {
                    DiagrammaStato dia = DiagrammiManager.getDgByIdTipoFasc(ProjectManager.getProjectInSession().template.SYSTEM_ID.ToString(), this.InfoUser.idAmministrazione);
                    if (dia != null)
                    {
                        idDiagram = dia.SYSTEM_ID.ToString();
                        idTipoFasc = prj.template.SYSTEM_ID.ToString();
                        DocsPaWR.Stato stato = DiagrammiManager.getStatoFasc(prj.systemID);
                        if (stato != null)
                            idState = stato.SYSTEM_ID.ToString();
                    }
                }
            }

            Registro reg = UIManager.RegistryManager.getRegistroBySistemId(prj.idRegistroNodoTit);
            if (reg == null)
            {
                reg = UIManager.RegistryManager.getRegistroBySistemId(prj.idRegistro);
            }
            if (reg == null)
            {
                reg = UIManager.RoleManager.GetRoleInSession().registri[0];
            }
            accessRights = prj.accessRights;
            Registro[] listReg = new Registro[1];

            listReg[0] = reg;
            this.Registry = reg;

            return new ArrayList(UIManager.TransmissionModelsManager.GetTransmissionModelsLiteFasc(this.UserLog.idAmministrazione, listReg, this.UserLog.idPeople, this.InfoUser.idCorrGlobali, idTipoFasc, idDiagram, idState, "F", idFasc, this.Role.idGruppo, true, accessRights));

        }

        private void LoadTransmissionModels()
        {
            this.DdlTransmissionsModel.Items.Clear();

            ArrayList idModelli = this.GetTransmissionModels();
            for (int i = 0; i < idModelli.Count; i++)
            {
                ModelloTrasmissione mod = (ModelloTrasmissione)idModelli[i];

                ListItem li = new ListItem(mod.NOME, mod.SYSTEM_ID.ToString());
                if (this.ViewCodeTransmissionModels) li.Text += " (" + mod.CODICE + ")";

                this.DdlTransmissionsModel.Items.Add(li);
            }
            this.DdlTransmissionsModel.Items.Insert(0, string.Empty);
        }

        private bool checkRagione(DocsPaWR.Trasmissione trasm)
        {
            if (trasm != null)
            {
                if (trasm.trasmissioniSingole != null)
                {

                    for (int i = 0; i < trasm.trasmissioniSingole.Length; i++)
                    {
                        if (trasm.trasmissioniSingole[i].ragione.descrizione.Equals("RISPOSTA"))
                            return true;
                    }
                }
            }

            return false;
        }

        private void LoadReasons()
        {
            Fascicolo prj = ProjectManager.getProjectInSession();
            RagioneTrasmissione[] listaRagioni = TrasmManager.getListaRagioniFasc(prj);

            if (listaRagioni != null && listaRagioni.Length > 0)
            {
                for (int i = 0; i < listaRagioni.Length; i++)
                {
                    if (!(listaRagioni[i].tipoRisposta != null && !listaRagioni[i].tipoRisposta.Equals("")))
                    {
                        if (checkRagione(this.Transmission))
                            continue;
                    }

                    // se la ragione prevede cessione ('W' o 'R') e l'utente non è abilitato alla cessione,
                    // allora non aggiunge ragioni alla DDL
                    if (!listaRagioni[i].prevedeCessione.Equals("N") && !this.checkIsAutorizedEditingACL())
                    {
                        continue;
                    }
                    else
                    {
                        ListItem newItem = new ListItem(listaRagioni[i].descrizione, listaRagioni[i].systemId);
                        this.ddlReason.Items.Add(newItem);
                    }
                }

                if (this.checkIsAutorizedEditingACL())
                {
                    this.chkTransferRights.Checked = false;
                    this.chkTransferRights.Enabled = false;
                }
            }
        }

        private void UpdateReasonInfo()
        {
            this.imgReason.AlternateText = "";
            this.imgReason.ToolTip = "";
            RagioneTrasmissione reason = null;

            if (!string.IsNullOrEmpty(this.ddlReason.SelectedValue))
            {
                reason = TrasmManager.getReasonById(this.ddlReason.SelectedValue);
                this.imgReason.AlternateText = reason.note;
                this.imgReason.ToolTip = reason.note;
                this.TxtCodeRecipientTransmission.Focus();
            }

            // GESTIONE CESSIONE DIRITTI
            this.gestioneCbxCessione(reason);

            if (this.checkIsAutorizedEditingACL())
                this.chkTransferRights_CheckedChanged(null, null);

            this.uppnlReason.Update();
            this.ReApplyScripts();
        }

        /// <summary>
        /// Imposta lo stato della combobox della cessione dei diritti
        /// </summary>
        /// <param name="ragione">oggetto Ragione</param>
        private void gestioneCbxCessione(DocsPaWR.RagioneTrasmissione ragione)
        {
            if (ragione != null && this.checkIsAutorizedEditingACL())
            {
                switch (ragione.prevedeCessione)
                {
                    case "N":
                        this.chkTransferRights.Checked = false;
                        this.chkTransferRights.Enabled = false;
                        break;
                    case "W":
                        this.chkTransferRights.Checked = false;
                        this.chkTransferRights.Enabled = true;
                        break;
                    case "R":
                        this.chkTransferRights.Checked = true;
                        this.chkTransferRights.Enabled = false;
                        break;
                    default:
                        this.chkTransferRights.Checked = false;
                        this.chkTransferRights.Enabled = false;
                        break;
                }
            }
        }

        protected void chkTransferRights_CheckedChanged(object sender, EventArgs e)
        {
            try {
                if (!string.IsNullOrEmpty(this.ddlReason.SelectedValue))
                {
                    DocsPaWR.RagioneTrasmissione reason = TrasmManager.getReasonById(this.ddlReason.SelectedValue);
                    reason.cessioneImpostata = this.chkTransferRights.Checked;
                    if (reason.cessioneImpostata && !string.IsNullOrEmpty(reason.mantieniLettura) && reason.mantieniLettura == "1")
                    {
                        this.Transmission.mantieniLettura = true;
                    }
                    // Mev Cessione Diritti - mantieni scrittura
                    if (reason.cessioneImpostata && !string.IsNullOrEmpty(reason.mantieniScrittura) && reason.mantieniScrittura == "1")
                    {
                        this.Transmission.mantieniScrittura = true;
                    }
                    // End Mev
                }

                if (sender != null)
                {
                    this.ReApplyScripts();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ReApplyChosenScript()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen_deselect", "$('.chzn-select-deselect').chosen({ allow_single_deselect: true, no_results_text: 'Nessun risultato trovato' });", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen", "$('.chzn-select').chosen({ no_results_text: 'Nessun risultato trovato' });", true);
        }

        private void ReApplyDatePickerScript()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "datepicker", "DatePicker();", true);
        }

        private void ReApplyTipsyScript()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "tipsy", "Tipsy();", true);
        }

        private DocsPaWR.Corrispondente[] queryUtenti(DocsPaWR.Corrispondente corr)
        {
            //costruzione oggetto queryCorrispondente
            DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPaWR.AddressbookQueryCorrispondente();

            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;

            qco.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
            qco.fineValidita = true;

            //corrispondenti interni
            qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;

            return UserManager.getListaCorrispondenti(this.Page, qco);
        }

        private DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPaWR.Trasmissione trasmissione, DocsPaWR.Corrispondente corr)
        {
            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                {
                    DocsPaWR.TrasmissioneSingola ts = (DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                    if (ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                    {
                        if (ts.daEliminare)
                        {
                            ((DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                            return trasmissione;
                        }
                        else
                            return trasmissione;
                    }
                }
            }

            // prima di aggiungere la trasm.ne singola verifico le ragioni cessione
            if (this.esisteRagTrasmCessioneInTrasm(trasmissione))
            {
                // non si può inserire nient'altro, avvisa...
                string msg = "ErrorTransmissionOnlyTransferRights";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.top.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                this.ddlReason.SelectedIndex = 0;
                this.uppnlReason.Update();
                return trasmissione;
            }

            // verifica se cessione a ruolo
            if (!(corr is DocsPaWR.Ruolo) && !(corr is DocsPaWR.UnitaOrganizzativa) && this.chkTransferRights.Checked)
            {
                this.inviaMsgNoRuoli();
                return trasmissione;
            }

            // Aggiungo la trasmissione singola
            DocsPaWR.TrasmissioneSingola trasmissioneSingola = new DocsPaWR.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = "S";
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = TrasmManager.getReasonById(this.ddlReason.SelectedValue);

            // Imposta la cessione sulla ragione
            if (this.checkIsAutorizedEditingACL())
                trasmissioneSingola.ragione.cessioneImpostata = this.chkTransferRights.Checked;


            // Aggiungo la lista di trasmissioniUtente
            if (corr is DocsPaWR.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
                DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr);
                if (listaUtenti.Length == 0)
                //Andrea
                {
                    trasmissioneSingola = null;
                    listaExceptionTrasmissioni1.Add("Non è presente alcun utente per la Trasmissione al ruolo: "
                                                    + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + ".");
                }
                //End Andrea
                else
                {
                    //ciclo per utenti se dest è gruppo o ruolo
                    for (int i = 0; i < listaUtenti.Length; i++)
                    {
                        DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPaWR.TrasmissioneUtente();
                        trasmissioneUtente.utente = (DocsPaWR.Utente)listaUtenti[i];
                        trasmissioneUtente.daNotificare = TrasmManager.getTxRuoloUtentiChecked();
                        trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                    }
                }
            }

            if (corr is DocsPaWR.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
                DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPaWR.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPaWR.Utente)corr;
                //Andrea - if - else
                if (trasmissioneUtente.utente == null)
                {
                    listaExceptionTrasmissioni1.Add("L utente: " + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + " è inesistente.");
                }
                //End Andrea
                else
                {
                    //trasmissioneUtente.daNotificare = true;
                    trasmissioneUtente.daNotificare = TrasmManager.getTxRuoloUtentiChecked();
                    trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                }
            }

            if (corr is DocsPaWR.UnitaOrganizzativa)
            {
                DocsPaWR.UnitaOrganizzativa theUo = (DocsPaWR.UnitaOrganizzativa)corr;
                DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = new DocsPaWR.AddressbookQueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                qca.ruolo = UserManager.GetSelectedRole();
                qca.queryCorrispondente = new AddressbookQueryCorrispondente();
                qca.queryCorrispondente.fineValidita = true;

                DocsPaWR.Ruolo[] ruoli = UserManager.getRuoliRiferimentoAutorizzati(this.Page, qca, theUo);

                //Andrea
                if (ruoli == null || ruoli.Length == 0)
                {
                    listaExceptionTrasmissioni1.Add("Manca un ruolo di riferimento per la UO: "
                                                        + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                        + ".");
                }
                //End Andrea
                else
                {

                    foreach (DocsPaWR.Ruolo r in ruoli)
                        trasmissione = addTrasmissioneSingola(trasmissione, r);
                }

                if (listaExceptionTrasmissioni1 != null && listaExceptionTrasmissioni1.Count > 0)
                {
                    string message = string.Empty;
                    foreach (string err in listaExceptionTrasmissioni1)
                    {
                        message = message + err + "<br/>";
                    }
                    string msgDesc = "WarningDocumentCustom";
                    string errFormt = Server.UrlEncode(message);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');}; ", true);
                }
                return trasmissione;
            }

            if (trasmissioneSingola != null)
                trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);
            else
            {
                // In questo caso questa trasmissione non può avvenire perché la struttura non ha utenti
                trasm_strutture_vuote.Add(String.Format("{0} ({1})", corr.descrizione, corr.codiceRubrica));
            }

            return trasmissione;
        }

        private bool esisteRagTrasmCessioneInTrasm(DocsPaWR.Trasmissione trasmissione)
        {
            bool retValue = false;

            // verifica solo se le trasm.ni singole sono più di una
            if (trasmissione.trasmissioniSingole != null && trasmissione.trasmissioniSingole.Length > 0)
            {
                // conta quante trasm.ni singole hanno la ragione con la cessione impostata
                int trasmConCessione = 0;
                foreach (DocsPaWR.TrasmissioneSingola trasmS in trasmissione.trasmissioniSingole)
                    if (trasmS.ragione.cessioneImpostata)
                        trasmConCessione++;


                if (!string.IsNullOrEmpty(this.ddlReason.SelectedValue))
                {
                    DocsPaWR.RagioneTrasmissione ragioneAttuale = TrasmManager.getReasonById(this.ddlReason.SelectedValue);

                    // non esistono ragioni con cessione, se l'attuale è con cessione avvisa che non si può...
                    if (this.checkIsAutorizedEditingACL())
                    {
                        if (!ragioneAttuale.prevedeCessione.Equals("N") && this.chkTransferRights.Checked)
                        {
                            string msg = "ErrorTransmissionOnlyTransferRights2";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            retValue = true;
                        }
                    }
                }
                else if (trasmConCessione > 0) //sono state già inserite ragioni con cessione 
                {
                    retValue = true;
                }
            }

            return retValue;
        }

        private bool esisteRagTrasmCessioneInTemplate(DocsPaWR.ModelloTrasmissione modello)
        {
            bool retValue = false;

            // verifica solo se le trasm.ni singole sono più di una
            if (this.Transmission.trasmissioniSingole != null && this.Transmission.trasmissioniSingole.Length > 0)
            {
                // conta quante trasm.ni singole hanno la ragione con la cessione impostata
                int trasmConCessione = 0;
                foreach (DocsPaWR.TrasmissioneSingola trasmS in this.Transmission.trasmissioniSingole)
                    if (trasmS.ragione.cessioneImpostata)
                        trasmConCessione++;

                /*
                                if (trasmConCessione>0) //sono state già inserite ragioni con cessione 
                                {
                                    // non si può inserire nient'altro, avvisa...
                                    string msg = @"Attenzione! Poichè la trasmissione creata prevede la cessione dei diritti, non è possibile inserire altro .";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "top.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', 'Trasmissione');", true);
                                    retValue = true;
                                }
                                else
                                {
                                    // non esistono ragioni con cessione, se l'attuale è con cessione avvisa che non si può...
                                    if (this.checkIsAutorizedEditingACL())
                                    {
                                        if (modello.CEDE_DIRITTI != null && modello.CEDE_DIRITTI.Equals("1"))
                                        {
                                            string msg = @"Attenzione! non è possibile inserire ragioni di trasmissione con cessione insieme a ragioni senza cessione.";
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "top.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', 'Trasmissione');", true);
                                            retValue = true;
                                        }
                                    }
                                }
                 */
                if (trasmConCessione > 0 || (this.checkIsAutorizedEditingACL() && modello.CEDE_DIRITTI != null && modello.CEDE_DIRITTI.Equals("1")))
                {
                    string msg = "ErrorTransmissionOnlyTransferRights2";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    retValue = true;
                }
            }

            return retValue;
        }

        private void ShowTransmissions()
        {
            this.plcTransmissions.Controls.Clear();

            if (this.Transmission != null && this.Transmission.trasmissioniSingole != null && this.Transmission.trasmissioniSingole.Length > 0)
            {
                Table table = this.GetTransmissionTable();

                for (int i = 0; i < this.Transmission.trasmissioniSingole.Length; i++)
                {
                    this.BuildSingleTransmission(ref table, this.Transmission.trasmissioniSingole[i].corrispondenteInterno, i);
                }

                this.plcTransmissions.Controls.Add(table);
            }

            //gestione flag notifiche
            this.gestioneMemoriaNotificaUt();

            this.upPnlTransmissionBuilt.Update();
        }

        private Control GetControlById(Control owner, string controlID)
        {
            Control myControl = null;
            // cycle all controls
            if (owner.Controls.Count > 0)
            {
                foreach (Control c in owner.Controls)
                {
                    myControl = GetControlById(c, controlID);
                    if (myControl != null) return myControl;
                }
            }
            if (controlID.Equals(owner.ID)) return owner;
            return null;
        }

        private string formatBlankValue(string valore)
        {
            string retValue = "&nbsp;";

            if (valore != null && valore != "")
            {
                retValue = valore;
            }

            return retValue;
        }

        private Table GetTransmissionTable()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            Table tbl = new Table();
            tbl.ID = "tblTransmissions";
            tbl.CssClass = "tbl_rounded";
            tbl.EnableViewState = true;

            {// header                

                TableRow row = new TableRow();
                row.EnableViewState = true;

                TableCell cell1 = new TableCell();
                cell1.ColumnSpan = 7;
                cell1.CssClass = "th first";
                cell1.Text = Utils.Languages.GetLabelFromCode("TransmissionLitDetailsRecipient", language);
                cell1.EnableViewState = true;
                row.Controls.Add(cell1);

                tbl.Controls.Add(row);
            }

            {// recipient header
                TableRow row = new TableRow();
                row.CssClass = "header";
                row.EnableViewState = true;

                TableCell cell1 = new TableCell();
                cell1.ColumnSpan = 2;
                cell1.CssClass = "first";
                cell1.Text = Utils.Languages.GetLabelFromCode("TransmissionLitDescription", language);
                cell1.EnableViewState = true;
                row.Controls.Add(cell1);

                TableCell cell2 = new TableCell();
                cell2.CssClass = "center trasmDetailReason";
                cell2.Text = Utils.Languages.GetLabelFromCode("TransmissionLitReason", language);
                cell2.EnableViewState = true;
                row.Controls.Add(cell2);

                TableCell cell3 = new TableCell();
                cell3.CssClass = "center trasmDetailType";
                cell3.Text = Utils.Languages.GetLabelFromCode("TransmissionLitType", language);
                cell3.EnableViewState = true;
                row.Controls.Add(cell3);

                TableCell cell4 = new TableCell();
                cell4.CssClass = "trasmDetailNote";
                cell4.Text = Utils.Languages.GetLabelFromCode("TransmissionLitNoteShort", language);
                cell4.EnableViewState = true;
                row.Controls.Add(cell4);

                TableCell cell5 = new TableCell();
                cell5.CssClass = "center trasmDetailDate";
                cell5.Text = Utils.Languages.GetLabelFromCode("TransmissionLitExpire", language);
                cell5.EnableViewState = true;
                row.Controls.Add(cell5);

                ImageButton imgBtn = new ImageButton();
                imgBtn.ImageUrl = "~/Images/Icons/delete.png";
                imgBtn.AlternateText = Utils.Languages.GetLabelFromCode("TransmissionLitDelete", language);
                imgBtn.ToolTip = Utils.Languages.GetLabelFromCode("TransmissionLitDelete", language);
                imgBtn.EnableViewState = true;
                imgBtn.Click += new ImageClickEventHandler(this.DeleteSingleTransmission);
                imgBtn.ID = "imgBtn";
                cell5.Controls.Add(imgBtn);
                UpdatePanel upPnl = new UpdatePanel();
                upPnl.EnableViewState = true;
                upPnl.UpdateMode = UpdatePanelUpdateMode.Always;
                upPnl.ContentTemplateContainer.Controls.Add(imgBtn);

                TableCell cell6 = new TableCell();
                cell6.CssClass = "center trasmDetailDelete";
                cell6.Controls.Add(upPnl);
                cell6.EnableViewState = true;
                row.Controls.Add(cell6);

                tbl.Controls.Add(row);
            }

            return tbl;
        }

        private TableRow GetSingleTransmissionRow(int id)
        {
            TableRow row = new TableRow();
            row.ID = "trasmDetails_" + id;
            row.EnableViewState = true;

            TableCell cellCheck = new TableCell();
            cellCheck.ID = "trasmDetailsCheck_" + id;
            cellCheck.CssClass = "center check";
            cellCheck.EnableViewState = true;
            row.Cells.Add(cellCheck);

            TableCell cellRecipient = new TableCell();
            cellRecipient.ID = "trasmDetailsRecipient_" + id;
            cellRecipient.EnableViewState = true;
            row.Cells.Add(cellRecipient);

            TableCell cellReason = new TableCell();
            cellReason.ID = "trasmDetailsReason_" + id;
            cellReason.CssClass = "center";
            cellReason.EnableViewState = true;
            row.Cells.Add(cellReason);

            TableCell cellType = new TableCell();
            cellType.ID = "trasmDetailsType_" + id;
            cellType.EnableViewState = true;
            row.Cells.Add(cellType);

            TableCell cellNote = new TableCell();
            cellNote.ID = "trasmDetailsNote_" + id;
            cellNote.EnableViewState = true;
            row.Cells.Add(cellNote);

            TableCell cellExpire = new TableCell();
            cellExpire.ID = "trasmDetailsExpire_" + id;
            cellExpire.CssClass = "date";
            cellExpire.EnableViewState = true;
            row.Cells.Add(cellExpire);

            TableCell cellDelete = new TableCell();
            cellDelete.ID = "trasmDetailsDelete_" + id;
            cellDelete.CssClass = "center check";
            cellDelete.EnableViewState = true;
            row.Cells.Add(cellDelete);

            return row;
        }
        
        private void BuildSingleTransmission(ref Table tbl, DocsPaWR.Corrispondente Corr, int idCorrispondente)
        {
            TableRow row = this.GetSingleTransmissionRow(idCorrispondente);
            if ((idCorrispondente + 1) % 2 == 0) row.CssClass = "alternate";



            {// main recipient
                ((TableCell)GetControlById(row, "trasmDetailsRecipient_" + idCorrispondente)).Text = "<strong>" + formatBlankValue(Corr.descrizione) + "</strong>";
                ((TableCell)GetControlById(row, "trasmDetailsReason_" + idCorrispondente)).Text = formatBlankValue(this.Transmission.trasmissioniSingole[idCorrispondente].ragione.descrizione);

                if (Corr.GetType() == typeof(DocsPaWR.Ruolo))
                {
                    // cell checkbox select all
                    this.drawSelectAll(ref row, idCorrispondente.ToString(), this.Transmission.trasmissioniSingole[idCorrispondente].trasmissioneUtente.Length);

                    // cell type
                    string language = UIManager.UserManager.GetUserLanguage();
                    DropDownList ddl_type = new DropDownList();
                    ddl_type.ID = "ddl_type_" + idCorrispondente;
                    ddl_type.Width = 110;
                    ddl_type.CssClass = "chzn-select-deselect";
                    ddl_type.EnableViewState = true;
                    ddl_type.Items.Add(Utils.Languages.GetLabelFromCode("TransmissionDdlTypeMulti", language));
                    ddl_type.Items[0].Value = "T";
                    ddl_type.Items.Add(Utils.Languages.GetLabelFromCode("TransmissionDdlTypeSingle", language));
                    ddl_type.Items[1].Value = "S";
                    //ddl_type.AutoPostBack = true;
                    ddl_type.SelectedIndexChanged += new EventHandler(this.ddl_type_SelectedIndexChanged);

                    if (this.Transmission.trasmissioniSingole[idCorrispondente].tipoTrasm == "T")
                        ddl_type.Items[0].Selected = true;
                    else
                        ddl_type.Items[1].Selected = true;

                    ((TableCell)GetControlById(row, "trasmDetailsType_" + idCorrispondente)).Controls.Add(ddl_type);
                }

                {// cell note        

                    CustomTextArea txt_NoteInd = new CustomTextArea();
                    txt_NoteInd.ID = "txt_NoteInd_" + idCorrispondente;
                    txt_NoteInd.CssClass = "txt_textdata";
                    
                    txt_NoteInd.Width = 150;
                    txt_NoteInd.MaxLength = 250;
                    txt_NoteInd.Text = this.Transmission.trasmissioniSingole[idCorrispondente].noteSingole;
                    //txt_NoteInd.AutoPostBack = false;
                    txt_NoteInd.EnableViewState = true;
                    txt_NoteInd.TextChanged += new EventHandler(this.txt_NoteInd_TextChanged);
                    ((TableCell)GetControlById(row, "trasmDetailsNote_" + idCorrispondente)).Controls.Add(txt_NoteInd);                   
                    
                   
                }

                {// cell expire date
                    CustomTextArea txt_expire = new CustomTextArea();
                    txt_expire.ID = "txt_expire_" + idCorrispondente;
                    txt_expire.CssClass = "txt_date datepicker";
                    txt_expire.CssClassReadOnly = "txt_date_disabled";
                    txt_expire.EnableViewState = true;
                    txt_expire.MaxLength = 10;
                    txt_expire.Text = this.Transmission.trasmissioniSingole[idCorrispondente].dataScadenza;
                    if (!this.Transmission.trasmissioniSingole[idCorrispondente].ragione.tipo.Equals("W"))
                    {
                        txt_expire.ReadOnly = true;
                    }
                    //txt_expire.AutoPostBack = true;
                    txt_expire.TextChanged += new EventHandler(this.txt_expire_TextChanged);
                    txt_expire.AutoPostBack = true;
                    ((TableCell)GetControlById(row, "trasmDetailsExpire_" + idCorrispondente)).Controls.Add(txt_expire);
                }

                {// cell delete checkbox
                    CheckBox chkDelete = new CheckBox();
                    chkDelete.ID = "chkCanc_" + idCorrispondente;
                    chkDelete.EnableViewState = true;
                    //chkDelete.AutoPostBack = true;
                    chkDelete.CheckedChanged += new EventHandler(this.chkDelete_CheckedChanged);
                    //chkDelete.Enabled = this.IsTransmissionErasable(this.Transmission.trasmissioniSingole[idCorrispondente]);

                    ((TableCell)GetControlById(row, "trasmDetailsDelete_" + idCorrispondente)).Controls.Add(chkDelete);
                }
            }

            tbl.Controls.Add(row);


            //this.DrawPreviousVersions(ref tbl, Corr, idCorrispondente);
            this.DrawUsers(ref tbl, Corr, idCorrispondente);
        }

        private void ddl_type_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try {
                int idCorrispondente = Int32.Parse(((DropDownList)sender).ID.Substring(9));
                DocsPaWR.Trasmissione trasmissione = this.Transmission;
                trasmissione.trasmissioniSingole[idCorrispondente].tipoTrasm = ((DropDownList)sender).SelectedItem.Value;
                trasmissione.trasmissioniSingole[idCorrispondente].daAggiornare = true;

                this.ReApplyScripts();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
        
        private void txt_NoteInd_TextChanged(object sender, System.EventArgs e)
        {
            try {
                int idCorrispondente = Int32.Parse(((TextBox)sender).ID.Substring(12));
                DocsPaWR.Trasmissione trasmissione = this.Transmission;
                trasmissione.trasmissioniSingole[idCorrispondente].noteSingole = ((TextBox)sender).Text;
                trasmissione.trasmissioniSingole[idCorrispondente].daAggiornare = true;

                this.ReApplyScripts();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
       
        private void txt_expire_TextChanged(object sender, System.EventArgs e)
        {
            try {
                if (Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(((TextBox)sender).Text).ToShortDateString(), System.DateTime.Now.ToShortDateString()))
                {

                    int idCorrispondente = Int32.Parse(((TextBox)sender).ID.Replace("txt_expire_", ""));
                    DocsPaWR.Trasmissione trasmissione = this.Transmission;
                    trasmissione.trasmissioniSingole[idCorrispondente].dataScadenza = ((TextBox)sender).Text;
                    trasmissione.trasmissioniSingole[idCorrispondente].daAggiornare = true;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningCompleteTaskExpireDateValid', 'warning', '','',null,null,'')", true);
                    ((TextBox)sender).Text = string.Empty;
                    return;
                }

                this.ReApplyScripts();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// Verifica se una trasmissione singola possa essere cancellata
        /// (in presenza di un systemid)
        /// </summary>
        /// <param name="trasmissioneSingola"></param>
        /// <returns></returns>
        private bool IsTransmissionErasable(DocsPaWR.TrasmissioneSingola trasmissioneSingola)
        {
            return (trasmissioneSingola.systemId == null);
        }

        private void drawSelectAll(ref TableRow row, string idCorrispondente, int numUtenti)
        {
            //seleziona tutti
            CheckBox chkSelAll = new CheckBox();
            chkSelAll.ID = "chkSelAll_" + idCorrispondente + "_" + numUtenti.ToString();
            chkSelAll.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            //chkSelAll.AutoPostBack = true;
            //chkSelAll.CheckedChanged += new EventHandler(this.chkSelAll_CheckedChanged);
            if (numUtenti == 1)
            {
                chkSelAll.Checked = true;
                chkSelAll.Enabled = false;
            }
            else
            {
                chkSelAll.Checked = TrasmManager.getTxRuoloUtentiChecked();
            }
            chkSelAll.Attributes["onclick"] = "chkSelAll(this);";
            ((TableCell)GetControlById(row, "trasmDetailsCheck_" + idCorrispondente)).Controls.Add(chkSelAll);
        }

        private void chkSelAll_CheckedChanged(object sender, System.EventArgs e)
        {
            try {
                string IdCheckSelAll = ((CheckBox)sender).ID;
                char[] IdSeparator = { '_' };
                int rowSingola = Int32.Parse((IdCheckSelAll.Split(IdSeparator)[1]));
                int numUtenti = Int32.Parse((IdCheckSelAll.Split(IdSeparator)[2]));

                for (int i = 0; i < numUtenti; i++)
                {
                    string userSystemId = this.Transmission.trasmissioniSingole[rowSingola].trasmissioneUtente[i].utente.systemId;

                    this.Transmission.trasmissioniSingole[rowSingola].trasmissioneUtente[i].daNotificare = ((CheckBox)(sender)).Checked;
                    this.modifyHashNotificaUt(rowSingola, this.Transmission.trasmissioniSingole[rowSingola].trasmissioneUtente[i].utente.idPeople, ((CheckBox)(sender)).Checked);
                    ((CheckBox)GetControlById(this.plcTransmissions, "chkNotifica_" + rowSingola + "_" + userSystemId + "_" + i)).Checked = ((CheckBox)(sender)).Checked;
                }

                this.ReApplyScripts();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        //private void DrawPreviousVersions(ref Table tbl, DocsPaWR.Corrispondente Corr, int idCorrispondente)
        //{
        //    int numeroCorr = Convert.ToInt32(idCorrispondente);
        //    DocsPaWR.SchedaDocumento documentoCorrente = DocumentManager.getSelectedRecord();

        //    if (documentoCorrente != null &&
        //        documentoCorrente.ConsolidationState != null &&
        //        documentoCorrente.ConsolidationState.State > DocsPaWR.DocumentConsolidationStateEnum.None)
        //    {
        //        // Flag nascondi versioni precedenti
        //        CheckBox chkHideDocumentPreviousVersions = new CheckBox();
        //        chkHideDocumentPreviousVersions.ID = "chkHideDocumentPreviousVersions_" + idCorrispondente;
        //        chkHideDocumentPreviousVersions.EnableViewState = true;
        //        chkHideDocumentPreviousVersions.Text = this.GetLabel("TransmissionHideDocumentPreviousVersions");
        //        chkHideDocumentPreviousVersions.TextAlign = TextAlign.Right;
        //        //chkHideDocumentPreviousVersions.AutoPostBack = true;

        //        if (documentoCorrente.previousVersionsHidden)
        //        {
        //            // Per l'utente corrente risultano nascoste le versioni prececenti a quella corrente,
        //            // pertanto il flag nascondi versioni è disabilitato e impostato per default
        //            chkHideDocumentPreviousVersions.Checked = true;
        //            chkHideDocumentPreviousVersions.Enabled = false;
        //            this.Transmission.trasmissioniSingole[numeroCorr].hideDocumentPreviousVersions = true;
        //        }
        //        else
        //        {
        //            chkHideDocumentPreviousVersions.CheckedChanged += new EventHandler(this.chkHideDocumentPreviousVersions_CheckedChanged);
        //            chkHideDocumentPreviousVersions.Checked = this.Transmission.trasmissioniSingole[numeroCorr].hideDocumentPreviousVersions;
        //            chkHideDocumentPreviousVersions.Enabled = true;
        //        }

        //        TableRow tr = new TableRow();
        //        tr.EnableViewState = true;
        //        if ((idCorrispondente + 1) % 2 == 0) tr.CssClass = "alternate";

        //        TableCell td = new TableCell();
        //        td.CssClass = "right notify";
        //        td.EnableViewState = true;
        //        td.ColumnSpan = 7;

        //        td.Controls.Add(chkHideDocumentPreviousVersions);
        //        tr.Cells.Add(td);
        //        tbl.Rows.Add(tr);
        //    }
        //}

        //protected void chkHideDocumentPreviousVersions_CheckedChanged(object sender, EventArgs e)
        //{
        //    CheckBox chkHidePreviousVersions = (CheckBox)sender;
        //    string idCheckNascondiVersioni = chkHidePreviousVersions.ID;

        //    int rowTxSingola = Int32.Parse(idCheckNascondiVersioni.Split(new char[1] { '_' })[1]);
        //    this.Transmission.trasmissioniSingole[rowTxSingola].hideDocumentPreviousVersions = chkHidePreviousVersions.Checked;

        //    this.ReApplyScripts();
        //}

        private void DrawUsers(ref Table tbl, DocsPaWR.Corrispondente Corr, int idCorrispondente)
        {
            DocsPaWR.TrasmissioneUtente[] trasmissioniUtente = this.Transmission.trasmissioniSingole[idCorrispondente].trasmissioneUtente;
            for (int j = 0; j < trasmissioniUtente.Length; j++)
            {
                DocsPaWR.Utente utente = trasmissioniUtente[j].utente;

                if (!UserManager.isUserDisabled(utente.codiceRubrica, utente.idAmministrazione))
                {

                    TableRow tru = new TableRow();
                    tru.EnableViewState = true;
                    if ((idCorrispondente + 1) % 2 == 0) tru.CssClass = "alternate";
                    tru.ID = "rowuser_" + idCorrispondente + "_" + utente.systemId + "_" + j.ToString();

                    TableCell tc = new TableCell();
                    tc.ColumnSpan = 7;
                    tc.EnableViewState = true;

                    string userDetails = utente.descrizione;
                    if (this.ShowsUserLocation && !string.IsNullOrEmpty(utente.sede)) userDetails += " (" + utente.sede + ")";

                    CheckBox chkNotifica = new CheckBox();
                    chkNotifica.ID = "chkNotifica_" + idCorrispondente + "_" + utente.systemId + "_" + j.ToString();
                    chkNotifica.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    chkNotifica.Text = userDetails;
                    chkNotifica.TextAlign = TextAlign.Right;
                    //chkNotifica.AutoPostBack = true;
                    chkNotifica.EnableViewState = true;
                    chkNotifica.Attributes["onclick"] = "chkNotifica(this);";
                    chkNotifica.CheckedChanged += new EventHandler(this.chkNotifica_CheckedChanged);

                    if (trasmissioniUtente.Length == 1)
                    {
                        //se l'utente è solo uno, di default disabilito il controllo e lo seleziono.
                        //Si deve trasmettere ad almeno ad un utente del ruolo.
                        chkNotifica.Enabled = false;
                        chkNotifica.Checked = true;
                        trasmissioniUtente[j].daNotificare = true;
                    }
                    else
                    {
                        bool valNotifica = this.selezionaNotifica(trasmissioniUtente[j], idCorrispondente);

                        chkNotifica.Checked = valNotifica;
                        trasmissioniUtente[j].daNotificare = valNotifica;
                    }


                    tc.Controls.Add(chkNotifica);
                    tru.Cells.Add(tc);
                    tbl.Rows.Add(tru);
                }
            }
        }

        private void chkNotifica_CheckedChanged(object sender, System.EventArgs e)
        {
            try {
                string IdCheckNotifica = ((CheckBox)sender).ID;
                char[] IdSeparator = { '_' };
                int rowSingola = FindSingleTransmissionIndex(Int32.Parse((IdCheckNotifica.Split(IdSeparator)[1])));
                rowSingola = Int32.Parse((IdCheckNotifica.Split(IdSeparator)[1]));
                int rowUtente = Int32.Parse((IdCheckNotifica.Split(IdSeparator)[3]));

                this.Transmission.trasmissioniSingole[rowSingola].trasmissioneUtente[rowUtente].daNotificare = ((CheckBox)(sender)).Checked;
                this.modifyHashNotificaUt(rowSingola, this.Transmission.trasmissioniSingole[rowSingola].trasmissioneUtente[rowUtente].utente.idPeople, ((CheckBox)(sender)).Checked);

                if (!((CheckBox)(sender)).Checked) ((CheckBox)GetControlById(this.plcTransmissions, "chkSelAll_" + IdCheckNotifica.Split(IdSeparator)[1] + "_" + this.Transmission.trasmissioniSingole[int.Parse(IdCheckNotifica.Split(IdSeparator)[1])].trasmissioneUtente.Length)).Checked = false;

                this.ReApplyScripts();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private int FindSingleTransmissionIndex(int id)
        {
            for (int i = 0; i < this.Transmission.trasmissioniSingole.Length; i++)
            {
                if (this.Transmission.trasmissioniSingole[i].systemId == id.ToString()) return i;
            }

            return -1;
        }

        protected void DeleteSingleTransmission(object sender, ImageClickEventArgs e)
        {
            try {
                Table tbl = ((Table)GetControlById(this.plcTransmissions, "tblTransmissions"));
                ArrayList selectedID = new ArrayList();
                bool setDaEliminare = true;

                foreach (TableRow row in tbl.Rows)
                {
                    if (row.ID != null && row.ID.StartsWith("trasmDetails_"))
                    {
                        CheckBox chk = ((CheckBox)GetControlById(row, row.ID.Replace("trasmDetails_", "chkCanc_")));
                        if (chk.Checked) selectedID.Add(chk.ID.Replace("chkCanc_", ""));
                    }
                }

                if (selectedID.Count > 0)
                {
                    for (int i = selectedID.Count - 1; i >= 0; i--)
                    {
                        setDaEliminare = (this.Transmission.trasmissioniSingole[i].systemId != null && !this.Transmission.trasmissioniSingole[i].systemId.Equals(""));

                        if (this.Transmission.systemId != null)  // sono in "Modifica"
                            setDaEliminare = false;

                        this.Transmission = TrasmManager.removeTrasmSingola(this.Transmission, Convert.ToInt32(selectedID[i]), setDaEliminare);

                        this.removeHashNotificaUt(Convert.ToInt32(selectedID[i]));
                    }

                    ShowTransmissions();
                }
                else
                {
                    string msg = "ErrorTransmissionNoneSelected";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                }

                this.ReApplyScripts();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void chkDelete_CheckedChanged(object sender, EventArgs e)
        {
            try {
                this.ReApplyScripts();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void DisableAllButtons()
        {
            this.TransmissionsBtnTransmit.Enabled = false;
            this.TransmissionsBtnRemove.Enabled = false;
            this.TransmissionsBtnSave.Enabled = false;
            this.TransmissionsBtnSaveTemplate.Enabled = false;
        }

        protected void EnableButtons()
        {
            this.DisableAllButtons();

            if (this.Transmission != null)
            {
                this.TransmissionsBtnSave.Enabled = (this.Transmission.trasmissioniSingole != null);
                this.TransmissionsBtnTransmit.Enabled = (this.Transmission.trasmissioniSingole != null);
                this.TransmissionsBtnSaveTemplate.Enabled = (this.Transmission.trasmissioniSingole != null);

                if (this.Transmission.systemId != null) this.TransmissionsBtnRemove.Enabled = true;
            }

            this.panelButtons.Update();
        }

        protected void TransmissionsBtnSave_Click(object sender, EventArgs e)
        {
            //try {
                this.SaveButNotTransmit = true;
                this.Transmissions_Click(sender, e);
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        protected void TransmissionsBtnTransmit_Click(object sender, EventArgs e)
        {
            //try {
                this.SaveButNotTransmit = false;
                this.Transmissions_Click(sender, e);
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        protected void dtEntryDocImgAddressBookUser_Click(object sender, EventArgs e)
        {
            try {
                this.CallType = this.GetCallType("AddressBook");
                HttpContext.Current.Session["AddressBook.from"] = "T_N_R_S";
                HttpContext.Current.Session["AddressBook.type"] = "F";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpPnlSender", "ajaxModalPopupAddressBook();", true);
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
                //List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];

                //if (atList != null && atList.Count > 0)
                //{
                //    foreach (NttDataWA.Popup.AddressBook.CorrespondentDetail addressBookCorrespondent in atList)
                //    {
                //        this.TxtCodeRecipientTransmission.Text = addressBookCorrespondent.CodiceRubrica;
                //        this.TxtDescriptionRecipient.Text = addressBookCorrespondent.Descrizione;
                //        this.IdRecipient.Value = addressBookCorrespondent.SystemID;

                //        if (addressBookCorrespondent.Tipo == "L")
                //        {

                //            string idAmm = UserManager.GetInfoUser().idAmministrazione;
                //            ArrayList listaCorr = UserManager.getCorrispondentiByCodLista(this.Page, addressBookCorrespondent.CodiceRubrica, idAmm);
                //            if (listaCorr != null && listaCorr.Count > 0)
                //            {
                //                DocsPaWR.Trasmissione trasmissioni = this.Transmission;

                //                ElementoRubrica[] ers = new ElementoRubrica[listaCorr.Count];

                //                for (int i = 0; i < listaCorr.Count; i++)
                //                {
                //                    DocsPaWR.ElementoRubrica er_1 = new DocsPaWR.ElementoRubrica();
                //                    DocsPaWR.Corrispondente c = (DocsPaWR.Corrispondente)listaCorr[i];
                //                    er_1 = UserManager.getElementoRubrica(this, c.codiceRubrica);
                //                    if (er_1 != null && !er_1.disabledTrasm)
                //                        ers[i] = er_1;
                //                }
                //                int coutStartErs = ers.Length;

                //                //Andrea
                //                for (int i = 0; i < listaCorr.Count; i++)
                //                {
                //                    DocsPaWR.Corrispondente c = (DocsPaWR.Corrispondente)listaCorr[i];
                //                    if (c != null && !string.IsNullOrEmpty(c.codiceRubrica))
                //                    {
                //                        trasmissioni = addTrasmissioneSingola(this.Transmission, c);
                //                    }
                //                }

                //                foreach (string s in listaExceptionTrasmissioni1)
                //                {
                //                    MessError_DestPerCodice = MessError_DestPerCodice + s + "\\n";
                //                }

                //                if (listaExceptionTrasmissioni1 != null && listaExceptionTrasmissioni1.Count != 0)
                //                {
                //                    listaExceptionTrasmissioni1 = new ArrayList();
                //                }

                //                if (MessError_DestPerCodice != "")
                //                {
                //                    Session.Add("MessError_DestPerCodice", MessError_DestPerCodice);
                //                }
                //                //End Andrea

                //                this.Transmission = trasmissioni;

                //                this.ShowTransmissions();
                //            }
                //        }
                //        else
                //        {
                //            this.BuildTransmission();
                //        }
                //        //corr = new Corrispondente();
                //        //corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);


                //        this.TxtCodeRecipientTransmission.Text = string.Empty;
                //        this.TxtDescriptionRecipient.Text = string.Empty;
                //        this.IdRecipient.Value = string.Empty;
                //    }
                //}

                //HttpContext.Current.Session["AddressBook.At"] = null;
                //HttpContext.Current.Session["AddressBook.Cc"] = null;
                //this.ResetFields();
                //this.EnableButtons();
                //this.UpPnlRecipient.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void Transmissions_Click(object sender, EventArgs e)
        {
            if (!this.hasNotificaUt())
            {
                string msg = "ErrorTransmissionNoneUserSelected";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
            }
            else
            {
                if (UIManager.ProjectManager.getProjectInSession().privato == "1" && !this.SaveButNotTransmit)
                {
                    string msg = "ConfirmTransmissionPrivateProject";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) { parent.fra_main.ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'proceed_private', '" + utils.FormatJs(this.GetLabel("TransmissionConfirm")) + "');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'proceed_private', '" + utils.FormatJs(this.GetLabel("TransmissionConfirm")) + "');}", true);
                }
                else
                {

                    this.BeginTransmissionButCheckOwner();
                    this.HeaderProject.RefreshHeader();
                    this.DdlTransmissionsModel.SelectedIndex = -1;
                    this.upPnlTransmissionsModel.Update();
                    this.EnableButtons();
                }
            }
        }

        private void BeginTransmissionButCheckOwner()
        {
            Fascicolo prj = ProjectManager.getProjectInSession();
            if (this.isTransferringRigts(prj))
            {
                if (this.numeroRuoliDestInTrasmissione > 0) this.aprePopUpSalvaNuovoProprietario();
            }
            else
            {
                BeginTransmissionIfAllowed();
            }
        }

        private void BeginTransmissionIfAllowed()
        {
            try
            {
                if (this.TransferRightsInRole && this.esisteRagTrasmCessioneInTrasm(this.Transmission))
                {
                    string accessRights = string.Empty;
                    string idGruppoTrasm = string.Empty;
                    string tipoDiritto = string.Empty;
                    string IDObject = string.Empty;

                    bool isPersonOwner = false;
                    IDObject = ProjectManager.getProjectInSession().systemID;
                    DocsPaWebService ws = new DocsPaWebService();
                    ws.SelectSecurity(IDObject, UserManager.GetInfoUser().idPeople, "= 0", out accessRights, out idGruppoTrasm, out tipoDiritto);
                    isPersonOwner = (accessRights.Equals("0"));
                    if (!isPersonOwner)
                    {
                        string idProprietario = this.GetAnagUtenteProprietario();
                        Utente _utproprietario = UserManager.GetUtenteByIdPeople(idProprietario);
                        string msg = "ConfirmTransmissionProceedOwnership";
                        string input = _utproprietario.cognome + " " + _utproprietario.nome;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "top.ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'proceed_ownership', '', '" + input.Replace("'", @"\'") + "');", true);
                    }
                    else
                    {
                        this.PerformActionTransmitProject();
                    }
                }
                else
                {
                    this.PerformActionTransmitProject();
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Azione di trasmissione del fascicolo corrente
        /// </summary>
        private void PerformActionTransmitProject()
        {
            DocsPaWR.Trasmissione trasmEff = this.Transmission;

            if (trasmEff != null)
            {
                if (trasmEff.utente != null && string.IsNullOrEmpty(trasmEff.utente.idAmministrazione)) trasmEff.utente.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                if (trasmEff.infoFascicolo == null) trasmEff.infoFascicolo = ProjectManager.getInfoFascicoloDaFascicolo(ProjectManager.getProjectInSession());
                trasmEff.noteGenerali = this.TxtNote.Text;

                trasmEff = this.ExtendVisibility(trasmEff);

                InfoUtente infoUser = UserManager.GetInfoUser();

                if (string.IsNullOrEmpty(trasmEff.dataInvio))
                {
                    if (this.SaveButNotTransmit)
                    {
                        trasmEff = TrasmManager.saveTrasm(this, trasmEff);
                    }
                    else
                    {
                        trasmEff = TrasmManager.saveExecuteTrasm(this, trasmEff, infoUser);
                    }
                }

                if (trasmEff.ErrorSendingEmails)
                {
                    string msg = "ErrorTransmissionSendingEmails";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '', null, null, null, '$(\\'#btnGoTransmissions\\').click();');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '', null, null, null, '$(\\'#btnGoTransmissions\\').click();');}", true);
                }
                else
                {
                    TrasmManager.PerformAutomaticStateChange(trasmEff);

                    this.Transmission = null;

                    if (trasmEff.cessione != null && trasmEff.cessione.docCeduto && !string.IsNullOrEmpty(trasmEff.dataInvio))
                        if (!trasmEff.mantieniLettura)
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}", true);
                        else
                        {
                            DocumentManager.setSelectedRecord(DocumentManager.getDocumentDetails(this, DocumentManager.getSelectedRecord().docNumber, DocumentManager.getSelectedRecord().docNumber));
                            Response.Redirect("../Project/TransmissionsP.aspx");
                        }
                    else
                        Response.Redirect("../Project/TransmissionsP.aspx");
                }
            }
        }

        protected void btnGoTransmissions_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Project/TransmissionsP.aspx", false);
        }

        /// <summary>
        /// Check if visibility is extended: seems valued only for models 
        /// </summary>
        /// <param name="trasmEff"></param>
        /// <returns></returns>
        private Trasmissione ExtendVisibility(Trasmissione trasmEff)
        {
            if (this.extend_visibility.Value == "false")
            {
                TrasmissioneSingola[] appoTrasmSingole = new TrasmissioneSingola[trasmEff.trasmissioniSingole.Length];

                for (int i = 0; i < trasmEff.trasmissioniSingole.Length; i++)
                {
                    TrasmissioneSingola trasmSing = new TrasmissioneSingola();
                    trasmSing = trasmEff.trasmissioniSingole[i];
                    trasmSing.ragione.eredita = "0";
                    appoTrasmSingole[i] = trasmSing;
                }

                trasmEff.trasmissioniSingole = appoTrasmSingole;
            }
            return trasmEff;
        }

        /// <summary>
        /// Get dell'idProprietario del fascicolo
        /// </summary>
        /// <returns></returns>
        private string GetAnagUtenteProprietario()
        {
            FascicoloDiritto[] listaVisibilita = null;
            Fascicolo prj = ProjectManager.getProjectInSession();
            string idProprietario = string.Empty;
            InfoFascicolo infoFasc = new InfoFascicolo();
            string rootFolder = ProjectManager.GetRootFolderFasc(prj);
            listaVisibilita = ProjectManager.getListaVisibilitaSemplificata(infoFasc, true, rootFolder);

            if (listaVisibilita != null && listaVisibilita.Length > 0)
            {
                for (int i = 0; i < listaVisibilita.Length; i++)
                {
                    if (listaVisibilita[i].accessRights == 0)
                    {
                        return idProprietario = listaVisibilita[i].personorgroup;
                    }
                }
            }

            return "";
        }

        private void ResetHiddenFields()
        {
            this.extend_visibility.Value = "";
            this.proceed_personal.Value = "";
            this.proceed_private.Value = "";
            this.proceed_ownership.Value = "";
        }

        protected void TransmissionsBtnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (!TrasmManager.deleteTrasm(this.Transmission))
                {
                    if (!ClientScript.IsStartupScriptRegistered("errorInDelete"))
                    {
                        string msg = "ErrorTransmissionWhileDelete";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                    }
                }
            }
            catch
            {
                if (!ClientScript.IsStartupScriptRegistered("errorInDelete"))
                {
                    string msg = "ErrorTransmissionWhileDelete";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                }
            }

            if (!ClientScript.IsStartupScriptRegistered("redirect"))
            {
                Response.Redirect("../Project/TransmissionsP.aspx");
            }
        }

        private bool isTransferringRigts(Fascicolo fasc)
        {
            bool retValue = false;

            // gestione cessione diritti
            if (this.esisteRagTrasmCessioneInTrasm(this.Transmission))
            {
                // verifica la proprietà del fascicolo da parte dell'utente
                if (this.utenteProprietario(fasc.systemID))
                {
                    // verifica se esistono ruoli tra i destinatari
                    this.verificaRuoliDestInTrasmissione();

                    switch (this.numeroRuoliDestInTrasmissione)
                    {
                        case 0:
                            // non ci sono ruoli tra i destinatari! avvisa...
                            this.inviaMsgNoRuoli();
                            retValue = true;
                            break;

                        case 1:
                            // ce n'è 1, verifica se un solo utente del ruolo ha la notifica...
                            this.utentiConNotifica();
                            if (this.numeroUtentiConNotifica > 1)
                                retValue = true;
                            else
                            {
                                // 1 solo utente con notifica, il sistema ha già memorizzato il suo id_people...
                                if (this.Transmission.cessione == null) this.Transmission.cessione = new CessioneDocumento();

                                this.Transmission.cessione.docCeduto = true;
                                this.Transmission.cessione.idPeople = UserManager.GetInfoUser().idPeople;
                                this.Transmission.cessione.idRuolo = UserManager.GetInfoUser().idGruppo;
                                this.Transmission.cessione.idPeopleNewPropr = this.idPeopleNewOwner;
                                this.Transmission.cessione.idRuoloNewPropr = ((DocsPaWR.Ruolo)this.Transmission.trasmissioniSingole[0].corrispondenteInterno).idGruppo;
                                this.Transmission.cessione.userId = UserManager.GetInfoUser().userId;

                                if (this.Template != null)
                                {
                                    this.Template.CEDE_DIRITTI = "1";
                                    this.Template.ID_PEOPLE_NEW_OWNER = this.Transmission.cessione.idPeopleNewPropr;
                                    this.Template.ID_GROUP_NEW_OWNER = this.Transmission.cessione.idRuoloNewPropr;
                                }
                            }
                            break;

                        default:
                            // ce ne sono + di 1, quindi ne fa scegliere uno...                                    
                            retValue = true;
                            break;
                    }
                }
            }

            return retValue;
        }

        protected void TransmissionsBtnSaveTemplate_Click(object sender, EventArgs e)
        {
            try {
                Fascicolo prj = ProjectManager.getProjectInSession();

                // gestione flag notifiche
                this.impostaHashNotificheInObjTrasm();

                // Controllo la presenza di destinatari
                if (this.Transmission.trasmissioniSingole == null || this.Transmission.trasmissioniSingole.Length <= 0)
                {
                    string msg = "ErrorTransmissionNoneRecipient";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }

                // save new model
                this.Template = null;
                this.Template = TrasmManager.getModelloTrasmNuovo(this.Transmission, prj.idRegistroNodoTit);

                if (this.Transmission.infoFascicolo == null) this.Transmission.infoFascicolo = ProjectManager.getInfoFascicoloDaFascicolo(ProjectManager.getProjectInSession());

                this.Transmission.noteGenerali = this.TxtNote.Text;

                if (this.isTransferringRigts(prj))
                {
                    if (this.numeroRuoliDestInTrasmissione > 0) this.aprePopUpSalvaModTrasmNuovoProprietario();
                }
                else
                {
                    this.aprePopUpSalvaModTrasm();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void aprePopUpSalvaModTrasm()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "TransmissionTemplateSave", "ajaxModalPopupTemplateSave();", true);
        }

        private DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPaWR.Trasmissione trasmissione, DocsPaWR.Corrispondente corr, DocsPaWR.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, DocsPaWR.MittDest mittDest)
        {
            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                {
                    DocsPaWR.TrasmissioneSingola ts = (DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];

                    if (corr != null)
                        if (ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                        {
                            if (ts.daEliminare)
                            {
                                ((DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                                return trasmissione;
                            }
                            else
                                return trasmissione;
                        }
                }
            }

            // Aggiungo la trasmissione singola
            DocsPaWR.TrasmissioneSingola trasmissioneSingola = new DocsPaWR.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = tipoTrasm;
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragione;
            trasmissioneSingola.noteSingole = note;

            //Imposto la data di scadenza
            if (scadenza > 0)
            {
                //string dataScadenza = "";
                System.DateTime data = System.DateTime.Now.AddDays(scadenza);
                trasmissioneSingola.dataScadenza = dateformat.formatDataDocsPa(data);
            }

            // Aggiungo la lista di trasmissioniUtente
            if (corr is DocsPaWR.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
                DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr);

                //Andrea
                if (listaUtenti.Length == 0)
                {
                    trasmissioneSingola = null;
                    throw new ExceptionTrasmissioni("Non è presente alcun utente per la Trasmissione al ruolo: "
                                                    + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + ".");
                }
                else
                {
                    //ciclo per utenti se dest è gruppo o ruolo
                    for (int i = 0; i < listaUtenti.Length; i++)
                    {
                        DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPaWR.TrasmissioneUtente();
                        trasmissioneUtente.utente = (DocsPaWR.Utente)listaUtenti[i];
                        trasmissioneUtente.daNotificare = this.selezionaNotificaDaModello(mittDest, trasmissioneUtente.utente); //TrasmManager.getTxRuoloUtentiChecked();
                        trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                    }
                }
            }

            if (corr is DocsPaWR.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
                DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPaWR.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPaWR.Utente)corr;
                trasmissioneUtente.daNotificare = this.selezionaNotificaDaModello(mittDest, trasmissioneUtente.utente);

                //Andrea
                if (trasmissioneUtente.utente == null)
                {
                    throw new ExceptionTrasmissioni("L utente: " + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + " è inesistente.");
                }
                //End Andrea
                else
                    trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
            }

            if (corr is DocsPaWR.UnitaOrganizzativa)
            {
                DocsPaWR.UnitaOrganizzativa theUo = (DocsPaWR.UnitaOrganizzativa)corr;
                DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = new DocsPaWR.AddressbookQueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                qca.ruolo = UserManager.GetSelectedRole();
                qca.queryCorrispondente = new AddressbookQueryCorrispondente();
                qca.queryCorrispondente.fineValidita = true;

                //DocsPaWR.Ruolo[] ruoli = UserManager.getListaRuoliInUO(this, theUo, UserManager.getInfoUtente());
                DocsPaWR.Ruolo[] ruoli = UserManager.getRuoliRiferimentoAutorizzati(this, qca, theUo);

                //Andrea
                if (ruoli.Length == 0)
                {
                    throw new ExceptionTrasmissioni("Manca un ruolo di riferimento per la UO: "
                                                        + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                        + ".");
                }
                //End Andrea
                else
                {
                    foreach (DocsPaWR.Ruolo r in ruoli)
                        if (r != null && r.systemId != null)
                            trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm, scadenza, mittDest);
                }
                return trasmissione;
            }

            if (trasmissioneSingola != null)
                trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);


            return trasmissione;
        }

        private bool selezionaNotificaDaModello(DocsPaWR.MittDest mittDest, DocsPaWR.Utente utente)
        {
            bool retValue = false;

            try
            {
                if (!mittDest.CHA_TIPO_URP.Equals("U"))
                {
                    if (mittDest.UTENTI_NOTIFICA != null)
                    {
                        foreach (DocsPaWR.UtentiConNotificaTrasm utNot in mittDest.UTENTI_NOTIFICA)
                        {
                            if (utente.idPeople.Equals(utNot.ID_PEOPLE))
                                return Convert.ToBoolean(utNot.FLAG_NOTIFICA.Replace("1", "true").Replace("0", "false"));
                        }
                    }
                    else
                    {
                        retValue = TrasmManager.getTxRuoloUtentiChecked();
                    }
                }
                else
                {
                    retValue = TrasmManager.getTxRuoloUtentiChecked();
                }
            }
            catch
            {
                retValue = false;
            }

            return retValue;
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

        #region Cessione dei diritti sull'oggetto

        /// <summary>
        /// Verifica se esistono RUOLI tra i destinatari della trasmissione 
        /// ed imposta quanti sono
        /// </summary>
        private void verificaRuoliDestInTrasmissione()
        {
            foreach (DocsPaWR.TrasmissioneSingola trasm in this.Transmission.trasmissioniSingole)
            {
                if (trasm.tipoDest.ToString().ToUpper().Equals("RUOLO"))
                    this.numeroRuoliDestInTrasmissione++;
            }
        }

        /// <summary>
        /// Apre una finestra per selezionare il ruolo a cui cedere i diritti di proprietà sull'oggetto
        /// </summary>
        private void aprePopUpSalvaModTrasmNuovoProprietario()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "TransmissionTemplateSaveNewOwner", "ajaxModalPopupTemplateSaveNewOwner();", true);
        }

        /// <summary>
        /// Apre una finestra per selezionare il ruolo a cui cedere i diritti di proprietà sull'oggetto
        /// </summary>
        private void aprePopUpSalvaNuovoProprietario()
        {
            if (this.SaveButNotTransmit)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "TransmissionSaveNewOwner", "ajaxModalPopupSaveNewOwner();", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "TransmissionTransmitNewOwner", "ajaxModalPopupTransmitNewOwner();", true);
            }
        }


        /// <summary>
        /// Verifica se l'utente è il PROPRIETARIO dell'oggetto
        /// </summary>
        /// <returns>True, False</returns>
        private bool utenteProprietario(string docNumber)
        {
            if (this.TransferRightsInRole)
            {
                return true;
            }
            else
            {
                return DocumentManager.dirittoProprietario(docNumber, UserManager.GetInfoUser());
            }

        }

        /// <summary>
        /// Invia un messaggio a video che avvisa l'utente che tra i destinatari della trasmissioni
        /// non ci sono ruoli
        /// </summary>
        private void inviaMsgNoRuoli()
        {
            string msg = "ErrorTransmissionTransferRightsAlmostRole";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '', 600, 500);} else { parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '', 600, 500);}", true);
        }

        /// <summary>
        /// Verifica se ci sono utenti con notifica, quanti sono e, nel caso di 1, ne memorizza l'ID
        /// </summary>
        private void utentiConNotifica()
        {
            foreach (DocsPaWR.TrasmissioneSingola trasm in this.Transmission.trasmissioniSingole)
                foreach (DocsPaWR.TrasmissioneUtente trasmUt in trasm.trasmissioneUtente)
                    if (trasmUt.daNotificare)
                    {
                        this.numeroUtentiConNotifica++;
                        this.idPeopleNewOwner = trasmUt.utente.idPeople; // memorizza l'id people... da utilizzare nel caso di un solo utente con notifica                        
                    }
        }

        /// <summary>
        /// Imposta il flag di notifica utente
        /// </summary>
        /// <param name="trasmUt"></param>
        /// <returns></returns>
        private bool selezionaNotifica(DocsPaWR.TrasmissioneUtente trasmUt, int indice)
        {
            //reperimento valore flag da hashTable in sessione (se esiste)
            bool retValue = this.drawTableFlagNotificaUt(indice, trasmUt.utente.idPeople, trasmUt.daNotificare);

            //se esiste la cessione dei diritti, la notifica è legata all'utente (che acquisirà i diritti) impostato precedentemente
            if (this.Transmission.cessione != null && this.Transmission.cessione.idPeopleNewPropr != null)
                if (trasmUt.utente.idPeople == this.Transmission.cessione.idPeopleNewPropr)
                    retValue = true;
                else
                    retValue = false;

            return retValue;
        }

        /// <summary>
        /// Imposta il flag di notifica utente quando la trasmissione è presa da un modello
        /// </summary>
        /// <param name="trasmUt"></param>
        /// <returns></returns>
        private bool selezionaNotificaDaModello(DocsPaWR.Corrispondente Corr, DocsPaWR.Utente utente, DocsPaWR.ModelloTrasmissione modello)
        {
            bool retValue = false;

            try
            {
                if (modello.RAGIONI_DESTINATARI != null)
                {
                    foreach (DocsPaWR.RagioneDest rag_dest in modello.RAGIONI_DESTINATARI)
                    {
                        foreach (DocsPaWR.MittDest mitt_dest in rag_dest.DESTINATARI)
                        {
                            if (mitt_dest.CHA_TIPO_URP.Equals("R"))
                            {
                                if (mitt_dest.ID_CORR_GLOBALI.ToString().Equals(Corr.systemId))
                                {
                                    if (mitt_dest.UTENTI_NOTIFICA != null)
                                    {
                                        foreach (DocsPaWR.UtentiConNotificaTrasm utNot in mitt_dest.UTENTI_NOTIFICA)
                                        {
                                            if (utente.idPeople.Equals(utNot.ID_PEOPLE))
                                                return Convert.ToBoolean(utNot.FLAG_NOTIFICA.Replace("1", "true").Replace("0", "false"));
                                        }
                                    }
                                    else
                                    {
                                        // imposta comunque a true
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                retValue = false;
            }

            return retValue;
        }

        #endregion

        #region Gestione memoria dei flag "Notifica utente"

        /// <summary>
        /// Metodo principale per la gestione della memoria per flag notifiche
        /// </summary>
        private void gestioneMemoriaNotificaUt()
        {
            if (this.Transmission != null && !existMemoriaNotificaUt())
                this.createHashNotificaUt();

            if (this.Transmission != null && existMemoriaNotificaUt())
                this.verificaAddTrasmS();
        }

        /// <summary>
        /// Verifica se sono state aggiunte delle nuove trasmissioni singole;
        /// in tal caso aggiorna la memoria dei flag delle notifiche
        /// </summary>
        private void verificaAddTrasmS()
        {
            ArrayList listaNotUt = this.getMemoriaNotificaUt();

            if (
                this.Transmission != null
                && listaNotUt != null
                && this.Transmission.trasmissioniSingole != null
                && this.Transmission.trasmissioniSingole.Length > listaNotUt.Count
            ) addHashNotificaUt();
        }

        /// <summary>
        /// Verifica se esiste una memoria (Session "MEM_NOT_UT") per i flag delle notifiche
        /// </summary>
        /// <returns></returns>
        private bool existMemoriaNotificaUt()
        {
            bool exist = false;

            if (System.Web.HttpContext.Current.Session["MEM_NOT_UT"] != null)
                exist = true;

            return exist;
        }

        /// <summary>
        /// Imposta la memoria (Session "MEM_NOT_UT") per i flag delle notifiche
        /// </summary>
        /// <param name="lista_notifiche_ut">lista delle notifiche</param>
        private void setMemoriaNotificaUt(ArrayList lista_notifiche_ut)
        {
            if (System.Web.HttpContext.Current.Session["MEM_NOT_UT"] == null)
                System.Web.HttpContext.Current.Session.Add("MEM_NOT_UT", lista_notifiche_ut);
        }

        /// <summary>
        /// Reperisce la memoria (Session "MEM_NOT_UT") per i flag delle notifiche
        /// </summary>
        /// <returns></returns>
        private ArrayList getMemoriaNotificaUt()
        {
            ArrayList listaNoticheUt = new ArrayList();
            if (System.Web.HttpContext.Current.Session["MEM_NOT_UT"] != null)
                listaNoticheUt = (ArrayList)System.Web.HttpContext.Current.Session["MEM_NOT_UT"];
            return listaNoticheUt;
        }

        /// <summary>
        /// Rilascia la memoria (Session "MEM_NOT_UT") per i flag delle notifiche
        /// </summary>
        private void releaseMemoriaNotificaUt()
        {
            System.Web.HttpContext.Current.Session.Remove("MEM_NOT_UT");
        }

        /// <summary>
        /// Crea la lista delle notifiche rispetto alle trasmissioni singole
        /// </summary>
        private void createHashNotificaUt()
        {
            Hashtable hashT;
            ArrayList lista = new ArrayList();

            if (this.Transmission != null && this.Transmission.trasmissioniSingole != null)
            {
                foreach (DocsPaWR.TrasmissioneSingola trasmS in this.Transmission.trasmissioniSingole)
                {
                    hashT = new Hashtable();
                    foreach (DocsPaWR.TrasmissioneUtente trasmU in trasmS.trasmissioneUtente)
                    {
                        //ABBATANGELI GIANLUIGI - se esiste già un utente in lista genera errore
                        if (!hashT.Contains(trasmU.utente.idPeople))
                            hashT.Add(trasmU.utente.idPeople, trasmU.daNotificare);
                    }
                    lista.Add(hashT);
                }
            }

            if (lista != null && lista.Count > 0)
                this.setMemoriaNotificaUt(lista);
        }

        /// <summary>
        /// Modifica la lista delle notifiche
        /// </summary>
        /// <param name="indiceTrasmSingola">Indice della Trasm.ne Singola</param>
        /// <param name="IDUtente">ID People dell'utente</param>
        /// <param name="valoreFlag">Valore da impostare</param>
        private void modifyHashNotificaUt(int indiceTrasmSingola, string IDUtente, bool valoreFlag)
        {
            ArrayList lista = new ArrayList();
            Hashtable hashT;

            if (existMemoriaNotificaUt())
            {
                lista = this.getMemoriaNotificaUt();
                if (lista != null && lista.Count > 0)
                {
                    hashT = (Hashtable)lista[indiceTrasmSingola];
                    hashT[IDUtente] = valoreFlag;
                }

                this.releaseMemoriaNotificaUt();
                this.setMemoriaNotificaUt(lista);
            }
        }

        /// <summary>
        /// Aggiunge dati alla lista delle notifiche rispetto alle trasmissioni singole
        /// </summary>
        private void addHashNotificaUt()
        {
            Hashtable hashT;
            DocsPaWR.TrasmissioneSingola trasmS;

            ArrayList currentList = this.getMemoriaNotificaUt();

            int currentListCount = currentList.Count;
            int currentTrasmSCount = this.Transmission.trasmissioniSingole.Length;

            for (int i = currentListCount + 1; i <= currentTrasmSCount; i++)
            {
                trasmS = this.Transmission.trasmissioniSingole[i - 1];

                hashT = new Hashtable();
                foreach (DocsPaWR.TrasmissioneUtente trasmU in trasmS.trasmissioneUtente)
                {
                    hashT.Add(trasmU.utente.idPeople, trasmU.daNotificare);
                }

                if (hashT != null && hashT.Count > 0)
                    currentList.Add(hashT);
            }

            this.releaseMemoriaNotificaUt();
            this.setMemoriaNotificaUt(currentList);
        }

        /// <summary>
        /// Elimina un item dalla lista delle notifiche
        /// </summary>
        /// <param name="indiceArray">Indice della lista da rimuovere</param>
        private void removeHashNotificaUt(int indiceArray)
        {
            ArrayList currentList = this.getMemoriaNotificaUt();
            try
            {
                currentList.RemoveAt(indiceArray);
            }
            catch { }
            this.releaseMemoriaNotificaUt();
            if (currentList != null) this.setMemoriaNotificaUt(currentList);
        }

        /// <summary>
        /// Reperisce il valore della notifica rispetto alla chiave passata
        /// </summary>
        /// <param name="indiceArray">Indice dell'item della lista da modificare</param>
        /// <param name="key">chiave da modificare (IdPeople dell'utente della Trasm.ne utente)</param>
        /// <returns></returns>
        private bool getValueHashNotificaUt(int indiceArray, string key)
        {
            bool retValue = false;

            Hashtable hashT = (Hashtable)this.getMemoriaNotificaUt()[indiceArray];
            //if (hashT.Count > indiceArray)
            //{
                try
                {
                    retValue = (bool)hashT[key];
                }
                catch { }
            //}

            return retValue;
        }

        /// <summary>
        /// Imposta il valore del check nella fase di generazione della tabella delle trasmissioni singole
        /// </summary>
        /// <param name="indiceArray">Indice della lista</param>
        /// <param name="idUtente">IDPeople dell'utente</param>
        /// <param name="currentValue">Valore corrente di default in caso di lista non impostata</param>
        /// <returns></returns>
        private bool drawTableFlagNotificaUt(int indiceArray, string idUtente, bool currentValue)
        {
            bool retValue = currentValue;

            if (this.existMemoriaNotificaUt())
                if (indiceArray <= this.getMemoriaNotificaUt().Count - 1)
                    retValue = this.getValueHashNotificaUt(indiceArray, idUtente);

            return retValue;
        }

        /// <summary>
        /// Riversa i dati della lista delle notifiche in memoria nell'oggetto Trasmisissione in fase di salvataggio dei dati 
        /// </summary>
        private void impostaHashNotificheInObjTrasm()
        {
            int indiceTrasmS = 0;

            foreach (DocsPaWR.TrasmissioneSingola trasmS in this.Transmission.trasmissioniSingole)
            {
                Hashtable hashT = (Hashtable)this.getMemoriaNotificaUt()[indiceTrasmS];

                if (hashT.Count > 0)
                {
                    foreach (DocsPaWR.TrasmissioneUtente trasmU in trasmS.trasmissioneUtente)
                    {
                        trasmU.daNotificare = this.getValueHashNotificaUt(indiceTrasmS, trasmU.utente.idPeople);
                    }
                }

                indiceTrasmS++;
            }
        }

        private bool hasNotificaUt()
        {
            ArrayList n = this.getMemoriaNotificaUt();
            bool userChecked;
            if (n.Count > 0)
            {
                foreach (Hashtable ht in n)
                {
                    userChecked = false;
                    foreach (DictionaryEntry d in ht)
                    {
                        if ((bool)d.Value)
                        {
                            userChecked = true;
                            break;
                        }
                    }
                    if (!userChecked)
                        return false;
                }
            }

            return true;
        }

        #endregion
    }
}