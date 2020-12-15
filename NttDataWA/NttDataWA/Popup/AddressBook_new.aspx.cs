using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;
using NttDatalLibrary;


namespace NttDataWA.Popup
{
    public partial class AddressBook_new : System.Web.UI.Page
    {

        #region Properties

        private bool ActiveCodeDescriptionAdministrationSender
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["ActiveCodeDescriptionAdministrationSender"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["ActiveCodeDescriptionAdministrationSender"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ActiveCodeDescriptionAdministrationSender"] = value;
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

        public DocsPaWR.Corrispondente corr
        {
            get
            {
                return Session["AddressBook_details_corr"] as DocsPaWR.Corrispondente;
            }
            set
            {
                Session["AddressBook_details_corr"] = value;
            }
        }

        public List<DocsPaWR.MailCorrispondente> CaselleList
        {
            get
            {
                return Session["AddressBook_details_gvCaselle"] as List<DocsPaWR.MailCorrispondente>;
            }
            set
            {
                Session["AddressBook_details_gvCaselle"] = value;
            }
        }

        /// <summary>
        /// Mantiene informazioni di base a partire dalla quale creare il nuovo corrispondente 
        /// dal tab documenti per un predisposto in arrivo di tipo interop
        /// </summary>
        public DocsPaWR.Corrispondente NewSender
        {
            get
            {
                DocsPaWR.Corrispondente result = null;
                if (HttpContext.Current.Session["newSender"] != null)
                {
                    result = HttpContext.Current.Session["newSender"] as DocsPaWR.Corrispondente;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["newSender"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    ClearSessionData();
                    this.LoadKeys();
                    this.InitLanguage();
                    this.InitPage();
                    this.InitComponent();
                }

                this.RefreshScripts();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// Viene precompilata una parte della maschera. Questo viene eseguito quando stiamo inserendo un nuovo corrispondente
        /// dal tab documenti per un predisposto in arrivo di tipo Interop (vecchia K2, caso di un solo corrispondente con la stessa email)
        /// </summary>
        private void InitComponent()
        {
            if (this.NewSender != null)
            {
                this.txt_email.Enabled = false;
                this.txtCasella.Enabled = false;
                this.txtNote.Enabled = false;
                this.imgAggiungiCasella.Enabled = false;
                MailCorrispondente mail = (from m in this.NewSender.Emails where m.Email.Equals(this.NewSender.email) select m).FirstOrDefault();
                if (mail != null)
                {
                    HttpContext.Current.Session.Remove("AddressBook_details_gvCaselle");
                    CaselleList = new List<MailCorrispondente>();
                    mail.Principale = "1";
                    CaselleList.Add(mail);
                }
                this.dd_canpref.Enabled = false;
                if (DocumentManager.getSelectedRecord().interop.Equals("S"))
                    this.ddl_tipoCorr.Enabled = false;
                this.dd_canpref.SelectedIndex = dd_canpref.Items.IndexOf(dd_canpref.Items.FindByValue(NewSender.canalePref.systemId));
                this.gvCaselle.DataSource = CaselleList;
                this.gvCaselle.DataBind();
                this.gvCaselle.Enabled = false;

            }
        }

        private void LoadKeys()
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE.ToString()]))
            {
                this.ActiveCodeDescriptionAdministrationSender = true;
            }
        }

        private void InitPage()
        {
            this.LoadRegistries();
            this.LoadCorrespondentType();
            this.set_mode(this.ddl_tipoCorr.SelectedValue);
            this.BindGridViewCaselle();
            this.SetAjaxCap();
        }

        private void SetAjaxCap()
        {
            this.RapidUoCap.ContextKey = this.uo_citta.Text;
        }

        private void RefreshScripts()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen_deselect", "$('.chzn-select-deselect').chosen({ allow_single_deselect: true, no_results_text: '" + utils.FormatJs(this.GetLabel("GenericChosenSelectNone")) + "' });", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen", "$('.chzn-select').chosen({ no_results_text: '" + utils.FormatJs(this.GetLabel("GenericChosenSelectNone")) + "' });", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.BtnInsert.Text = Utils.Languages.GetLabelFromCode("GenericBtnInsert", language);
            this.lbl_registro.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsRegistry", language);
            this.lbl_indirizzo.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsAddress", language);
            this.lbl_cap.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsZipCode", language);
            this.lbl_citta.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCity", language);
            this.lbl_provincia.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsDistrict", language);
            this.lbl_local.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsPlace", language);
            this.lbl_nazione.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCountry", language);
            this.lbl_telefono.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsPhone", language);
            this.lbl_telefono2.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsPhone2", language);
            this.lbl_fax.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsFax", language);
            this.lbl_codfisc.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsTaxId", language);
            this.lbl_partita_iva.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCommercialId", language);
            this.lbl_codice_ipa.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsIpaCode", language);
            this.lbluser_indirizzo.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsAddress", language);
            this.lbluser_cap.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsZipCode", language);
            this.lbluser_citta.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCity", language);
            this.lbluser_provincia.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsDistrict", language);
            this.lbluser_local.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsPlace", language);
            this.lbluser_nazione.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCountry", language);
            this.lbluser_telefono.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsPhone", language);
            this.lbluser_telefono2.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsPhone2", language);
            this.lbluser_fax.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsFax", language);
            this.lbluser_codfisc.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsTaxId", language);
            this.lbluser_partita_iva.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCommercialId", language);
            this.lbl_email.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsEmail", language);
            this.lbl_codAOO.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCodAOO", language);
            this.lbl_codAmm.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCodAdmin", language);
            this.lblNote.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsNoteEmail", language);
            this.lbl_note.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsNote", language);
            this.lbluser_note.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsNote", language);
            this.lbl_preferredChannel.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsPreferredChannel", language);
            this.lbl_tipocorr.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsType", language);
            this.lbl_CodRubrica.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsAddressBookCode", language);
            this.lbl_descrizione.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsAddressBookDescription", language);
            this.lbl_DescR.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsAddressBookDescription", language);
            this.lbl_titolo.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsTitolo", language);
            this.lbl_nome.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsName", language);
            this.lbl_cognome.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsSurname", language);
            this.lbl_luogonascita.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsBirthplace", language);
            this.lbl_dataNascita.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsBirthday", language);
            this.ddl_titolo.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectOptionNeutral", language));
            this.ddl_registri.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectOptionNeutral", language));
            this.ddl_tipoCorr.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectOptionNeutral", language));
            this.imgAggiungiCasella.AlternateText = Utils.Languages.GetLabelFromCode("imgAggiungiCasella", language);
            this.imgAggiungiCasella.ToolTip = Utils.Languages.GetLabelFromCode("imgAggiungiCasella", language);
            this.dd_canpref.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectOptionNeutral", language));

            this.cbInteroperanteRGS.Text = Utils.Languages.GetLabelFromCode("CorrispondentInteroperanteRGS", language);
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            try {
                if(this.NewSender != null)
                    this.NewSender = null;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                this.CloseMask(string.Empty);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ClearSessionData()
        {
            this.CaselleList = null;
        }

        protected void CloseMask(string returnValue)
        {
            this.ClearSessionData();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "parent.closeAjaxModal('AddressBook_New', '" + returnValue + "', parent);", true);
        }

        private DocsPaWR.Registro[] getListaRegistri()
        {
            //prendo i registri/RF visibili al ruolo dell'utente
            this.ddl_registri.Enabled = true;
            //DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistriWithRF(this, "", "");
            DocsPaWR.Registro[] userRegistri = RegistryManager.GetRegAndRFListInSession();
            return userRegistri;
        }

        protected void LoadRegistries()
        {
            DocsPaWR.Registro[] userRegistri = this.getListaRegistri();

            if (userRegistri != null && userRegistri.Length > 0)
            {
                //Spaziatura per RF
                string strText = "";
                //int contatoreRegRF = 0;
                for (short iff = 0; iff < 3; iff++)
                {
                    strText += " ";
                }

                //Aggiunta registri o rf
                for (int i = 0; i < userRegistri.Length; i++)
                {
                    if (!userRegistri[i].Sospeso)
                    {
                        if (userRegistri[i].chaRF == "1")
                        {
                            //RF
                            if (UserManager.IsAuthorizedFunctions("DO_INS_CORR_RF"))
                            {
                                string testo = strText + userRegistri[i].codRegistro;
                                ListItem item = new ListItem();
                                item.Text = testo;
                                item.Value = userRegistri[i].systemId + "_" + userRegistri[i].rfDisabled + "_" + userRegistri[i].idAOOCollegata;
                                this.ddl_registri.Items.Add(item);
                            }
                        }
                        else
                        {
                            //Registro
                            if (UserManager.IsAuthorizedFunctions("DO_INS_CORR_REG") && !userRegistri[i].flag_pregresso)
                            {
                                string testo = userRegistri[i].codRegistro;
                                ListItem item = new ListItem();
                                item.Text = testo;
                                item.Value = userRegistri[i].systemId + "_" + userRegistri[i].rfDisabled + "_" + userRegistri[i].idAOOCollegata;
                                this.ddl_registri.Items.Add(item);
                            }

                        }
                    }
                }

                //Aggiunta voce su tutti i registri o rf
                if (UserManager.IsAuthorizedFunctions("DO_INS_CORR_TUTTI"))
                {
                    ListItem item = new ListItem();
                    item.Text = "TUTTI";
                    item.Value = "";
                    this.ddl_registri.Items.Add(item);
                    this.ddl_registri.SelectedIndex = this.ddl_registri.Items.IndexOf(this.ddl_registri.Items.FindByText("TUTTI"));
                }
            }
        }

        protected void LoadCorrespondentType()
        {
            this.ddl_tipoCorr.Items.Add(new ListItem("UO", "U"));
            this.ddl_tipoCorr.Items.Add(new ListItem("RUOLO", "R"));
            this.ddl_tipoCorr.Items.Add(new ListItem("PERSONA", "P"));
        }

        protected void ddl_registri_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                bool rfPresente = false;

                for (int li = 0; li < this.ddl_registri.Items.Count; li++)
                {
                    string valore = this.ddl_registri.Items[li].Value;
                    string[] arrayValori = valore.Split('_');
                    if (arrayValori != null && arrayValori.Length > 1)
                    {
                        if (!string.IsNullOrEmpty(arrayValori[1]))
                        {
                            rfPresente = true;
                            break;
                        }
                    }
                }

                if (rfPresente)
                {
                    this.lbl_registro.Text = this.GetLabel("CorrespondentDetailsRegistryRF");
                }
                else
                {
                    this.lbl_registro.Text = this.GetLabel("CorrespondentDetailsRegistry");
                }

                int indice_selezionato = this.ddl_registri.SelectedIndex;

                ListItem item = null;
                string valore2 = null;
                string[] arrayValori2 = null;

                if (indice_selezionato != -1)
                {
                    item = this.ddl_registri.Items[indice_selezionato];
                    valore2 = item.Value;
                    arrayValori2 = valore2.Split('_');
                }

                if (arrayValori2 != null && arrayValori2.Length > 1)
                {
                    if (!string.IsNullOrEmpty(arrayValori2[1]))
                    {
                        if (arrayValori2[1] == "1")
                        {
                            this.BtnInsert.Enabled = false;
                            this.BtnInsert.ToolTip = this.GetLabel("AddressBookInsertDisabled");
                        }
                        else
                        {
                            this.BtnInsert.Enabled = true;
                            this.BtnInsert.ToolTip = this.GetLabel("GenericBtnInsert");
                        }

                    }
                    else
                    {
                        this.BtnInsert.Enabled = true;
                        this.BtnInsert.ToolTip = this.GetLabel("GenericBtnInsert");
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void rdbPrincipale_ChecekdChanged(object sender, EventArgs e)
        {
            try {
                string mailSelect = (((sender as RadioButton).Parent.Parent as System.Web.UI.WebControls.GridViewRow).FindControl("txtEmailCorr") as TextBox).Text;
                List<DocsPaWR.MailCorrispondente> listCaselle = CaselleList;
                foreach (DocsPaWR.MailCorrispondente c in listCaselle)
                {
                    if (c.Email.Trim().Equals(mailSelect.Trim()))
                        c.Principale = "1";
                    else
                        c.Principale = "0";
                }
                CaselleList = listCaselle as List<DocsPaWR.MailCorrispondente>;
                gvCaselle.DataSource = CaselleList;
                gvCaselle.DataBind();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void imgEliminaCasella_Click(object sender, ImageClickEventArgs e)
        {
            try {
                bool isComboMain = (((sender as System.Web.UI.WebControls.ImageButton).Parent.Parent as System.Web.UI.WebControls.GridViewRow).
                                        FindControl("rdbPrincipale") as RadioButton).Checked;
                //se presenti più caselle e si tenta di eliminare una casella settata come principale il sistema avvisa l'utente
                if (isComboMain && CaselleList.Count > 1)
                {
                    string msg = "WarningAddressBookDeletingPrimaryEmail";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                    return;
                }
                int indexRowDelete = ((sender as System.Web.UI.WebControls.ImageButton).Parent.Parent as System.Web.UI.WebControls.GridViewRow).RowIndex;
                CaselleList.RemoveAt(indexRowDelete);
                gvCaselle.DataSource = CaselleList;
                gvCaselle.DataBind();
                if (CaselleList.Count < 1 && !MultiCasellaManager.IsEnabledMultiMail(RoleManager.GetRoleInSession().idAmministrazione))
                {
                    txtCasella.Enabled = true;
                    txtNote.Enabled = true;
                    imgAggiungiCasella.Enabled = true;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void imgAggiungiCasella_Click(object sender, ImageClickEventArgs e)
        {
            try {
                //verifico che l'indirizzo non sia vuoto
                if (string.IsNullOrEmpty(this.txtCasella.Text))
                {
                    string msg = "WarningAddressBookEmailNotInserted";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                    return;
                }

                //verifico il formato dell'indirizzo mail
                string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
                if (!System.Text.RegularExpressions.Regex.Match(this.txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), pattern).Success)
                {
                    string msg = "WarningAddressBookEmailNotInserted";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                    return;
                }

                //verifico che la casella non sia già stata associata al corrispondente       
                if (CaselleList != null)
                {
                    foreach (DocsPaWR.MailCorrispondente c in CaselleList)
                    {
                        if (c.Email.Trim().Equals(this.txtCasella.Text.Trim()))
                        {
                            string msg = "WarningAddressBookEmailAlreadyInserted";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                            return;
                        }
                    }
                }
                CaselleList.Add(new DocsPaWR.MailCorrispondente()
                {
                    Email = this.txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                    Note = txtNote.Text,
                    Principale = gvCaselle.Rows.Count < 1 ? "1" : "0"
                });
                if (CaselleList.Count > 0)
                {
                    //this.txtCasella.Enabled = false;
                    //txtNote.Enabled = false;
                    //imgAggiungiCasella.Enabled = false;
                }
                this.gvCaselle.DataSource = CaselleList;
                this.gvCaselle.DataBind();

                this.txtCasella.Text = string.Empty;
                this.txtNote.Text = string.Empty;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void txtEmailCorr_TextChanged(object sender, EventArgs e)
        {
            try {
                string newMail = (sender as System.Web.UI.WebControls.TextBox).Text;
                int rowModify = ((sender as System.Web.UI.WebControls.TextBox).Parent.Parent as System.Web.UI.WebControls.GridViewRow).RowIndex;
                CaselleList[rowModify].Email = newMail;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void txtNoteMailCorr_TextChanged(object sender, EventArgs e)
        {
            try {
                string newNote = (sender as System.Web.UI.WebControls.TextBox).Text;
                int rowModify = ((sender as System.Web.UI.WebControls.TextBox).Parent.Parent as System.Web.UI.WebControls.GridViewRow).RowIndex;
                CaselleList[rowModify].Note = newNote;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected bool TypeMailCorrEsterno(string typeMail)
        {
            return (typeMail.Equals("1")) ? true : false;
        }

        protected void BtnInsert_Click(object sender, EventArgs e)
        {
            try {
                string corr_type = ddl_tipoCorr.SelectedValue;

                int indxMail = this.dd_canpref.Items.IndexOf(this.dd_canpref.Items.FindByText("MAIL"));
                int indxInterop = this.dd_canpref.Items.IndexOf(this.dd_canpref.Items.FindByText("INTEROPERABILITA"));

                if (this.ddl_registri.SelectedValue.Equals("") && !UserManager.IsAuthorizedFunctions("DO_INS_CORR_TUTTI"))
                {
                    string msg = "WarningAddressBookInsertNotAllowed";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                    return;
                }

                if (this.txt_CodRubrica.Text == "" ||
                    (corr_type == "U" && this.uo_descrizione.Text.Trim() == "") ||
                    (corr_type == "R" && this.txt_DescR.Text.Trim() == "") ||
                    (corr_type == "P" && (this.txt_cognome.Text.Trim() == "" || this.txt_nome.Text.Trim() == ""))||
                    (dd_canpref.SelectedIndex == indxInterop && (this.txt_codAmm.Text.Equals(String.Empty) ||
                    this.txt_codAOO.Text.Equals(String.Empty))))
                {
                    string msg = "WarningAddressBookInsertObligatory";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                    return;
                }
                else
                {
                    if (CaselleList == null || (CaselleList as List<DocsPaWR.MailCorrispondente>).Count < 1)
                    {
                        //verifico che l'indirizzo non sia vuoto
                        if (string.IsNullOrEmpty(txtCasella.Text))
                        {
                            if (dd_canpref.SelectedItem.Text.Equals("MAIL") || dd_canpref.SelectedItem.Text.Equals("INTEROPERABILITA"))
                            {
                                string msg = "WarningAddressBookEmailNotInserted";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                                return;
                            }
                        }

                        //verifico il formato dell'indirizzo mail
                        string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
                        if (!string.IsNullOrEmpty(txtCasella.Text) && !System.Text.RegularExpressions.Regex.Match(txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), pattern).Success)
                        {
                            string msg = "WarningAddressBookEmailInvalid";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                            return;
                        }
                        if (!string.IsNullOrEmpty(txtCasella.Text))
                        {
                            (CaselleList as List<DocsPaWR.MailCorrispondente>).Add(new DocsPaWR.MailCorrispondente()
                            {
                                Email = txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                Note = txtNote.Text,
                                Principale = "1"
                            });
                        }
                    }

                    if (!codice_rubrica_valido(this.txt_CodRubrica.Text) || this.txt_CodRubrica.Text.Contains("(") || this.txt_CodRubrica.Text.Contains(")"))
                    {
                        string msg = "WarningAddressBookCodeInvalid";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        return;
                    }

                    if (NewSender != null && txt_CodRubrica.Text.Contains("INTEROP"))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningChangeCodAddressBook', 'warning', '');", true);
                        return;
                    }

                    if (this.txt_codAmm.Text != "" && !codice_rubrica_valido(this.txt_codAmm.Text))
                    {
                        string msg = "WarningAddressBookCodeAmmInvalid";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        return;
                    }
                    if (this.txt_codAOO.Text != "" && !codice_rubrica_valido(this.txt_codAOO.Text))
                    {
                        string msg = "WarningAddressBookCodeAooInvalid";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        return;
                    }

                    //verifica del formato della data di nascita
                    if (this.txt_dataNascita.Text != string.Empty && !utils.isDate(this.txt_dataNascita.Text))
                    {
                        string msg = "WarningAddressBookBirthdayInvalid";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        return;
                    }

                    if ((this.uo_cap != null && !this.uo_cap.Text.Equals("") && !utils.isNumeric(uo_cap.Text))
                        || (this.user_cap != null && !this.user_cap.Text.Equals("") && !utils.isNumeric(user_cap.Text)))
                    {
                        string msg = "WarningAddressBookZipcodeInvalid";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        return;
                    }

                    //Verifica della correttezza del codice fiscale
                    if (corr_type.Equals("U"))
                    {
                        if ((this.uo_codfisc != null && !this.uo_codfisc.Text.Trim().Equals("")) && ((this.uo_codfisc.Text.Trim().Length == 11 && utils.CheckVatNumber(this.uo_codfisc.Text.Trim()) != 0) || (this.uo_codfisc.Text.Trim().Length == 16 && utils.CheckTaxCode(this.uo_codfisc.Text.Trim()) != 0) || (this.uo_codfisc.Text.Trim().Length != 11 && this.uo_codfisc.Text.Trim().Length != 16)))
                        {
                            string msg = "WarningAddressBookTaxIdInvalid";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                            return;
                        }

                        //Verifica della correttezza della partita iva
                        if (this.uo_partita_iva != null && !this.uo_partita_iva.Text.Equals("") && utils.CheckVatNumber(uo_partita_iva.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0)
                        {
                            string msg = "WarningAddressBookVatInvalid";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                            return;
                        }
                    }
                    else
                    {
                        if (this.user_codfisc != null && !this.user_codfisc.Text.Equals("") && utils.CheckTaxCode(this.user_codfisc.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0)
                        {
                            string msg = "WarningAddressBookTaxIdInvalid";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                            return;
                        }


                        if (this.user_partita_iva != null && !this.user_partita_iva.Text.Equals("") && utils.CheckVatNumber(this.user_partita_iva.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0)
                        {
                            string msg = "WarningAddressBookVatInvalid";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                            return;
                        }
                    }



                    if ((this.uo_telefono == null || this.uo_telefono.Text.Equals(""))
                        && !(this.uo_telefono2 == null || this.uo_telefono2.Text.Equals(""))
                        || (this.user_telefono == null || this.user_telefono.Text.Equals(""))
                        && !(this.user_telefono2 == null || this.user_telefono2.Text.Equals("")))
                    {
                        string msg = "WarningAddressBookPhoneNotInserted";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        return;
                    }

                    if (this.ddl_registri != null && this.ddl_registri.Items.Count >= 0)
                    {
                        if (!(this.ddl_registri.SelectedValue.Equals("")))
                        {
                            string idRegistroCorrente = "";

                            if (ddl_registri.SelectedValue != string.Empty)
                            {
                                char[] sep = { '_' };
                                string[] datiSelezione = ddl_registri.SelectedValue.Split(sep);

                                idRegistroCorrente = datiSelezione[0];
                                DocsPaWR.Registro registro_corrente = UserManager.getRegistroBySistemId(this, idRegistroCorrente);
                                if (registro_corrente.Sospeso)
                                {
                                    string msg = "WarningAddressBookRegistrySuspended";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '', '" + utils.FormatJs(registro_corrente.descrizione) + "');", true);
                                    return;
                                }
                            }
                        }
                    }
                }

                DocsPaWR.InfoUtente iu = UserManager.GetInfoUser();
                DocsPaWR.Registro reg_corr = null;

                string idRegistro = string.Empty;
                if (ddl_registri != null && ddl_registri.SelectedIndex >= 0)
                {
                    if (ddl_registri.SelectedValue != string.Empty)
                    {
                        char[] sep = { '_' };
                        string[] datiSelezione = ddl_registri.SelectedValue.Split(sep);

                        idRegistro = datiSelezione[0];
                    }
                }

                DocsPaWR.Corrispondente corr = new DocsPaWR.Corrispondente();
                DocsPaWR.Corrispondente res = null;


                //creo l'oggetto canale
                DocsPaWR.Canale canale = new DocsPaWR.Canale();
                canale.systemId = this.dd_canpref.SelectedItem.Value;

                switch (corr_type)
                {
                    case "U":
                        DocsPaWR.UnitaOrganizzativa uo = new DocsPaWR.UnitaOrganizzativa();
                        uo.tipoCorrispondente = "U";
                        uo.codiceCorrispondente = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        uo.codiceRubrica = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        uo.codiceAmm = this.txt_codAmm.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        uo.codiceAOO = this.txt_codAOO.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        uo.descrizione = this.uo_descrizione.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        uo.idAmministrazione = iu.idAmministrazione;
                        uo.localita = this.user_local.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        uo.idRegistro = idRegistro;
                        uo.interoperanteRGS = this.cbInteroperanteRGS.Checked;


                        //Aggiunto per considerare il caso in cui l'utente venga aggiunto dalla maschera di K2(caso un solo corrispondente)
                        if (NewSender != null)
                            uo.oldDescrizione = NewSender.oldDescrizione;

                        uo = (DocsPaWR.UnitaOrganizzativa)AddressBookManager.ValorizeInfoCorr(uo, uo_indirizzo.Text.Trim(), uo_citta.Text.Trim(), uo_cap.Text.Trim(), uo_provincia.Text.Trim(), uo_nazione.Text.Trim(), uo_telefono.Text.Trim(), uo_telefono2.Text.Trim(), uo_fax.Text.Trim()
                            , uo_codfisc.Text.Trim(), uo_note.Text.Trim(), uo_local.Text.Trim(), string.Empty, string.Empty, string.Empty, uo_partita_iva.Text.Trim(), uo_codice_ipa.Text.Trim());

                        uo.dettagli = true;
                        uo.canalePref = canale;
                        //mail
                        if (CaselleList != null && (CaselleList as List<MailCorrispondente>).Count > 0)
                        {
                            foreach (MailCorrispondente c in (CaselleList as List<MailCorrispondente>))
                            {
                                if (c.Principale.Equals("1"))
                                {
                                    uo.email = c.Email.Trim();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            uo.email = string.Empty;
                        }
                        res = UserManager.addressbookInsertCorrispondente(this, uo, null);
                        if (res != null && (!string.IsNullOrEmpty(res.systemId)))
                        {
                            if (!MultiCasellaManager.InsertMailCorrispondenteEsterno((CaselleList as List<MailCorrispondente>), res.systemId))
                            {
                                string msg = "ErrorAddressBookMultiEmailInsert";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                                return;
                            }
                        }
                        break;

                    case "R":
                        res = new DocsPaWR.Corrispondente();
                        DocsPaWR.Ruolo ruolo = new DocsPaWR.Ruolo();
                        ruolo.tipoCorrispondente = "R";
                        ruolo.codiceCorrispondente = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        ruolo.codiceRubrica = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        ruolo.descrizione = this.txt_DescR.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        ruolo.idRegistro = idRegistro;
                        ruolo.codiceAmm = this.txt_codAmm.Text;
                        ruolo.codiceAOO = this.txt_codAOO.Text;
                        ruolo.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                        ruolo.interoperanteRGS = this.cbInteroperanteRGS.Checked;
                        DocsPaWR.UnitaOrganizzativa parent_uo = new UnitaOrganizzativa();
                        parent_uo.descrizione = "";
                        parent_uo.systemId = "0";
                        ruolo.canalePref = canale;
                        //mail
                        if (CaselleList != null && (CaselleList as List<MailCorrispondente>).Count > 0)
                        {
                            foreach (MailCorrispondente c in (CaselleList as List<MailCorrispondente>))
                            {
                                if (c.Principale.Equals("1"))
                                {
                                    ruolo.email = c.Email.Trim();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            ruolo.email = string.Empty;
                        }
                        res = UserManager.addressbookInsertCorrispondente(this, ruolo, parent_uo);
                        if (res != null && (!string.IsNullOrEmpty(res.systemId)))
                        {
                            if (!MultiCasellaManager.InsertMailCorrispondenteEsterno((CaselleList as List<MailCorrispondente>), res.systemId))
                            {
                                string msg = "ErrorAddressBookMultiEmailInsert";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                                return;
                            }
                        }
                        break;

                    case "P":
                        res = new DocsPaWR.Corrispondente();
                        DocsPaWR.Utente utente = new DocsPaWR.Utente();
                        utente.codiceCorrispondente = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        utente.codiceRubrica = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        utente.cognome = this.txt_cognome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        utente.nome = this.txt_nome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        utente.codiceAmm = this.txt_codAmm.Text;
                        utente.codiceAOO = this.txt_codAOO.Text;
                        utente.descrizione = this.ddl_titolo.Text + this.txt_cognome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()) + this.txt_nome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                        utente.luogoDINascita = this.txt_luogoNascita.Text;
                        utente.dataNascita = this.txt_dataNascita.Text;
                        utente.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                        utente.titolo = this.ddl_titolo.Text;
                        utente.idRegistro = idRegistro;
                        utente.tipoCorrispondente = this.ddl_tipoCorr.SelectedItem.Value;
                        utente.canalePref = canale;
                        utente.interoperanteRGS = this.cbInteroperanteRGS.Checked;
                        //mail
                        if (CaselleList != null && (CaselleList as List<MailCorrispondente>).Count > 0)
                        {
                            foreach (MailCorrispondente c in (CaselleList as List<MailCorrispondente>))
                            {
                                if (c.Principale.Equals("1"))
                                {
                                    utente.email = c.Email.Trim();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            utente.email = string.Empty;
                        }
                        if ((user_indirizzo.Text != null && !user_indirizzo.Equals("")) ||
                                (user_cap.Text != null && !user_cap.Equals("")) ||
                                (user_citta.Text != null && !user_citta.Equals("")) ||
                                (user_provincia.Text != null && !user_provincia.Equals("")) ||
                                (user_nazione.Text != null && !user_nazione.Equals("")) ||
                                (user_telefono.Text != null && !user_telefono.Equals("")) ||
                                (user_telefono2 != null && !user_telefono2.Equals("")) ||
                                (user_fax.Text != null && !user_fax.Equals("")) ||
                                (user_codfisc.Text != null && !user_codfisc.Equals("")) ||
                                (user_note.Text != null && !user_note.Equals("")) ||
                                 user_partita_iva.Text != null && !user_partita_iva.Equals(""))
                        {
                            utente.dettagli = true;

                            utente = (DocsPaWR.Utente)AddressBookManager.ValorizeInfoCorr(utente, user_indirizzo.Text.Trim(), user_citta.Text.Trim(), user_cap.Text.Trim(), user_provincia.Text.Trim(), user_nazione.Text.Trim(), user_telefono.Text.Trim(), user_telefono2.Text.Trim(), user_fax.Text.Trim()
                                , user_codfisc.Text.Trim(), user_note.Text.Trim(), user_local.Text.Trim(), ddl_titolo.SelectedValue, txt_luogoNascita.Text, txt_dataNascita.Text, user_partita_iva.Text.Trim(), user_codice_ipa.Text.Trim());
                        }
                        res = UserManager.addressbookInsertCorrispondente(this, utente, null);
                        if (res != null && (!string.IsNullOrEmpty(res.systemId)))
                        {
                            if (!MultiCasellaManager.InsertMailCorrispondenteEsterno((CaselleList as List<MailCorrispondente>), res.systemId))
                            {
                                string msg = "ErrorAddressBookMultiEmailInsert";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                                return;
                            }
                        }
                        break;

                }

                if (res != null && res.errore == null)
                {
                    if (this.NewSender != null)
                    {
                        NewSender = UIManager.AddressBookManager.GetCorrespondentBySystemId(res.systemId);
                        if (NewSender == null || string.IsNullOrEmpty(NewSender.systemId))
                        {
                            //

                            NewSender = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(res.codiceRubrica);
                        }
                    }
                    this.CloseMask(res.systemId);
                }
                else
                {
                    string msg = "WarningAddressBookInsertError";
                    string err = string.Empty;
                    if (res != null && res.errore != null) err = res.errore;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '', '" + utils.FormatJs(err) + "');", true);
                    return;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private bool codice_rubrica_valido(string cod)
        {
            if (cod == null || cod.Trim() == "")
                return false;

            System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex(@"^[0-9A-Za-z_\ \.\-]+$");
            return rx.IsMatch(cod);
        }

        protected void ddl_tipoCorr_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try {
                this.set_mode(this.ddl_tipoCorr.SelectedValue);
                if (this.dd_canpref.SelectedIndex == -1 || this.dd_canpref.SelectedIndex == 0)
                {
                    this.starCodAOO.Visible = false;
                    this.starCodAmm.Visible = false;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void dd_canpref_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                this.starCodAOO.Visible = false;
                this.starEmail.Visible = false;
                this.starCodAmm.Visible = false;
                this.cbInteroperanteRGS.Visible = false;
                this.cbInteroperanteRGS.Checked = false;

                switch (dd_canpref.SelectedItem.Text)
                {
                    case "MAIL":
                        this.starEmail.Visible = true;
                        break;

                    case "INTEROPERABILITA":
                        this.starEmail.Visible = true;
                        this.starCodAOO.Visible = true;
                        this.starCodAmm.Visible = true;

                        if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_ENABLE_FLUSSO_AUTOMATICO.ToString())) &&
                            Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_ENABLE_FLUSSO_AUTOMATICO.ToString()).Equals("1"))
                            this.cbInteroperanteRGS.Visible = true;
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void set_mode(string m)
        {
            switch (m)
            {
                case "U":
                    pnlUO.Visible = true;
                    pnlRuolo.Visible = false;
                    pnlUtente.Visible = false;
                    break;

                case "R":
                    pnlUO.Visible = false;
                    pnlRuolo.Visible = true;
                    pnlUtente.Visible = false;
                    break;

                case "P":
                    pnlUO.Visible = false;
                    pnlRuolo.Visible = false;
                    pnlUtente.Visible = true;
                    break;
            }
            if(NewSender == null || (NewSender != null && dd_canpref != null && dd_canpref.Items.Count==0))
                this.LoadPreferredChannels();
            this.LoadTitles();
        }


        private void LoadPreferredChannels()
        {
            this.dd_canpref.Items.Clear();

            this.dd_canpref.Items.Add("");
            string idAmm = UserManager.GetInfoUser().idAmministrazione;
            DocsPaWR.MezzoSpedizione[] m_sped = AddressBookManager.GetAmmListaMezzoSpedizione(idAmm, false);

            // Non deve essere possibile creare un corrispondente con canale preferenziale impostato
            // a interoperabilità semplificata
            m_sped = m_sped.Where(ms => ms.chaTipoCanale != "S").ToArray();

            if (m_sped != null && m_sped.Length > 0)
            {
                foreach (DocsPaWR.MezzoSpedizione m_spediz in m_sped)
                {
                    ListItem item = new ListItem(m_spediz.Descrizione, m_spediz.IDSystem);
                    this.dd_canpref.Items.Add(item);
                }
            }
        }

        private void LoadTitles()
        {
            this.ddl_titolo.Items.Clear();
            this.ddl_titolo.Items.Add("");

            string[] listaTitoli = AddressBookManager.GetListaTitoli();
            foreach (string tit in listaTitoli)
            {
                this.ddl_titolo.Items.Add(tit);
            }
        }

        protected void BindGridViewCaselle()
        {
            if (CaselleList == null)
                CaselleList = new List<DocsPaWR.MailCorrispondente>();

            gvCaselle.DataSource = CaselleList;
            gvCaselle.DataBind();
        }

        protected void uo_cap_TextChanged(object sender, System.EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            this.RapidUoCap.MinimumPrefixLength = 3;
            try
            {
                //this.uo_citta.Text = string.Empty;
                //this.uo_provincia.Text = string.Empty;
                if (!string.IsNullOrEmpty(this.uo_cap.Text))
                {
                    string comune = string.Empty;
                    string[] infoComune = this.uo_cap.Text.Split('-');
                    string cap = infoComune[0].Trim();
                    if (infoComune.Count() > 1)
                    {
                        comune = infoComune[1].Trim();

                        InfoComune info = AddressBookManager.GetCapComuni(cap, comune);
                        if (info != null && !string.IsNullOrEmpty(info.COMUNE))
                        {
                            this.uo_cap.Text = info.CAP;
                            this.uo_citta.Text = info.COMUNE;
                            this.uo_provincia.Text = info.PROVINCIA;
                            this.RapidUoCap.ContextKey = info.COMUNE;
                            this.UpPnlUoInfoComune.Update();
                        }
                    }
                }
                this.uo_citta.Focus();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void uo_citta_TextChanged(object sender, System.EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            try
            {
                string comune = this.uo_citta.Text;
                //this.uo_provincia.Text = string.Empty;
                this.RapidUoCap.ContextKey = comune;
                this.uo_local.Focus();
                if (!string.IsNullOrEmpty(this.uo_citta.Text))
                {
                    InfoComune c = AddressBookManager.GetProvinciaComune(comune);
                    if (c != null && !string.IsNullOrEmpty(c.COMUNE))
                    {
                        this.uo_citta.Text = c.COMUNE;
                        this.uo_provincia.Text = c.PROVINCIA;
                        this.RapidUoCap.MinimumPrefixLength = 0;
                        this.uo_cap.Text = string.Empty;
                        this.uo_cap.Focus();
                        this.UpPnlUoInfoComune.Update();
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void user_cap_TextChanged(object sender, System.EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            try
            {
                //this.user_citta.Text = string.Empty;
                //this.user_provincia.Text = string.Empty;
                if (!string.IsNullOrEmpty(this.user_cap.Text))
                {
                    string comune = string.Empty;
                    string[] infoComune = this.user_cap.Text.Split('-');
                    string cap = infoComune[0].Trim();
                    if (infoComune.Count() > 1)
                    {
                        comune = infoComune[1].Trim();
                        InfoComune info = AddressBookManager.GetCapComuni(cap, comune);
                        if (info != null && !string.IsNullOrEmpty(info.COMUNE))
                        {
                            this.user_cap.Text = info.CAP;
                            this.user_citta.Text = info.COMUNE;
                            this.user_provincia.Text = info.PROVINCIA;
                            this.RapidUserCap.ContextKey = info.COMUNE;
                            this.UpPnlUoInfoComune.Update();
                        }
                    }
                }
                this.user_citta.Focus();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void user_citta_TextChanged(object sender, System.EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            try
            {
                string comune = this.user_citta.Text;
                //this.user_provincia.Text = string.Empty;
                this.RapidUserCap.ContextKey = comune;
                this.user_provincia.Focus();
                if (!string.IsNullOrEmpty(this.user_citta.Text))
                {
                    InfoComune c = AddressBookManager.GetProvinciaComune(comune);
                    if (c != null && !string.IsNullOrEmpty(c.COMUNE))
                    {
                        this.user_citta.Text = c.COMUNE;
                        this.user_provincia.Text = c.PROVINCIA;
                        this.RapidUserCap.MinimumPrefixLength = 0;
                        this.user_cap.Text = string.Empty;
                        this.user_cap.Focus();
                        this.UpPnlUserInfoComune.Update();
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
    }
}