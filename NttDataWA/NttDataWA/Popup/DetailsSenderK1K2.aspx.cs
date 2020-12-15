using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    public partial class DetailsSenderK1K2 : System.Web.UI.Page
    {

        #region Properties

        private List<Corrispondente> listCorrespondentSameMail
        {
            get
            {
                if (HttpContext.Current.Session["listCorrespondentSameMail"] != null)
                {
                    return HttpContext.Current.Session["listCorrespondentSameMail"] as List<Corrispondente>;
                }
                return null;
            }
            set
            {
                HttpContext.Current.Session["listCorrespondentSameMail"] = value;
            }
        }

        private bool IsZoom
        {
            get
            {
                if (HttpContext.Current.Session["isZoom"] != null)
                    return (bool)HttpContext.Current.Session["isZoom"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["isZoom"] = value;
            }
        }

        private bool OpenZoomFromDetailsSender
        {
            get
            {
                if (HttpContext.Current.Session["OpenZoomFromDetailsSender"] != null)
                    return (bool)HttpContext.Current.Session["OpenZoomFromDetailsSender"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["OpenZoomFromDetailsSender"] = value;
            }
        }


        private List<Corrispondente> listObj
        {
            get
            {
                if (HttpContext.Current.Session["listObj"] != null)
                {
                    return HttpContext.Current.Session["listObj"] as List<Corrispondente>;
                }
                return null;
            }
            set
            {
                HttpContext.Current.Session["listObj"] = value;
            }
        }

        private string NewIdCorr
        {
            get
            {
                if (HttpContext.Current.Session["newIdCorr"] != null)
                {
                    return HttpContext.Current.Session["newIdCorr"] as String;
                }
                return null;
            }
            set
            {
                HttpContext.Current.Session["newIdCorr"] = value;
            }
        }

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

        /// <summary>
        /// Indica se è necessario modficare i bottoni del documento che indicano nessina/una/più presenze del corrispondente in rubrica nel caso di k1-k2
        /// </summary>
        private bool UpdateDocumentProfileButton
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["UpdateDocumentProfileButton"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["UpdateDocumentProfileButton"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["UpdateDocumentProfileButton"] = value;
            }
        }

        private bool NewCorrespondent
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["newCorrespondent"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["newCorrespondent"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["newCorrespondent"] = value;
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

        private bool IsModified
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsModified"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["IsModified"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["IsModified"] = value;
            }
        }

        private bool IsModifiedChannel
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsModifiedChannel"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["IsModifiedChannel"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["IsModifiedChannel"] = value;
            }
        }

        private bool IsModifiedCaselle
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsModifiedCaselle"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["IsModifiedCaselle"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["IsModifiedCaselle"] = value;
            }
        }

        private Dictionary<string, string> Parameter
        {
            get
            {
                Dictionary<string, string> result = null;
                if (HttpContext.Current.Session["Parameter"] != null)
                {
                    result = HttpContext.Current.Session["Parameter"] as Dictionary<string, string>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["Parameter"] = value;
            }
        }

        private bool IsEmailReadOnly
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsEmailReadOnly"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["IsEmailReadOnly"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["IsEmailReadOnly"] = value;
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

        private DocsPaWR.Corrispondente SenderDetail
        {
            get
            {
                return Session["senderDetail"] as DocsPaWR.Corrispondente;
            }
            set
            {
                Session["senderDetail"] = value;
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

        private SchedaDocumento DocumentInWorking
        {
            get
            {
                SchedaDocumento result = null;
                if (HttpContext.Current.Session["document"] != null)
                {
                    result = HttpContext.Current.Session["document"] as SchedaDocumento;
                }
                return result;
            }
        }


        #endregion

        #region global variable

        private static string viewDetails;
        private static string hideDetails;
        private static string btnSaveAndRecord;
        private static string btnCreateAndRecord;

        // di default si suppone che le liste sono disabilitate
        private int flagListe=0;	
		

        #endregion

        #region Standard Method

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.LoadInconFile();
                    this.LoadKeys();
                    this.InitLanguage();
                    this.InitPage();
                    this.drawDettagliCorr();
                    enablePnl_Email(false);
                }
                else
                {
                    // detect action from popup confirm
                    if (this.proceed_delete.Value == "true") { this.DeleteProceed(); return; }
                    if (string.IsNullOrEmpty(this.proceed_delete.Value))
                    {
                        this.proceed_delete.Value = "false";
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                    }
                }

                this.RefreshScripts();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void LoadInconFile()
        {
            string url = ResolveUrl(FileManager.getFileIcon(this.Page,FileManager.getEstensioneIntoSignedFile( DocumentManager.getSelectedRecord().documenti[0].fileName)));
            this.btnViewFileDocument.ImageUrl = url;
            this.btnViewFileDocument.OnMouseOverImage = url;
            this.btnViewFileDocument.OnMouseOutImage = url;
            this.btnViewFileDocument.ImageUrlDisabled = url;
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
            if (this.SenderDetail != null)
            {
                if (this.SenderDetail.tipoIE == "E")
                {
                    if (SenderDetail.inRubricaComune) this.setCampiReadOnly(true);
                }
                if (this.SenderDetail.tipoIE != "E" || !UserManager.IsAuthorizedFunctions("GEST_RUBRICA"))
                {
                    this.setCampiReadOnly(true);
                }

                this.viewOtherDetailsLink.Text = viewDetails;

                #region K1

                if (Parameter["tipoAvviso"].Equals("avviso1"))
                {
                    //Emanuela: se sono nella maschera di k1 e il mezzo di spedizione è di tipo Mail, mostro il bottone Occasionale e Protocolla
                    SchedaDocumento schedaDoc = this.DocumentInWorking;
                    if (!IsPostBack && UserManager.IsAuthorizedFunctions("DO_OCC_IN_K1") && schedaDoc.mezzoSpedizione.Equals("9"))
                    {
                        this.BtnSaveOccasionalAndRecord.Enabled = true;
                        this.BtnSaveOccasionalAndRecord.Visible = true;
                        this.BtnRecord.Enabled = false;
                        this.detailsCorrespondent.Style.Add(HtmlTextWriterStyle.Width, "100%");
                    }
                    if (DocumentInWorking.interop.Equals("S"))
                        this.ddl_tipoCorr.Enabled = false;
                    this.listCorrespondent.Visible = false;
                    enablePnl_Email(false);
                    this.BtnRecord.ToolTip = btnCreateAndRecord;
                }

                #endregion

                #region k2

                if (Parameter["tipoAvviso"].Equals("avviso2"))
                {
                    bool visibleBtnAddAdd = UserManager.IsAuthorizedFunctions("GEST_RUBRICA");
                    string systemIdCorrespondent = string.Empty;
                    this.listCorrespondent.Visible = true;
                    RepListCorrespond_Bind();
                    if (repListCorrespond != null)
                    {
                        (repListCorrespond.Items[0].FindControl("rd") as CheckBox).Checked = true;
                         systemIdCorrespondent = (repListCorrespond.Items[0].FindControl("systemIdCorrespondent") as HiddenField).Value;
                         bool enabled = (repListCorrespond.Items.Count > 1 && !systemIdCorrespondent.Equals(Parameter["systemId"])); 
                        (repListCorrespond.Items[0].FindControl("btnAddCorrespondent") as ImageButton).Enabled = true;
                        (repListCorrespond.Items[0].FindControl("btnAddCorrespondent") as ImageButton).Visible = visibleBtnAddAdd;
                        (repListCorrespond.Items[0].FindControl("btnDeleteCorrespondent") as ImageButton).Enabled = enabled;
                        SenderDetail = (from corr in listCorrespondentSameMail where corr.systemId.Equals(systemIdCorrespondent) select corr).FirstOrDefault();
                        setCampiReadOnly(true);
                        enablePnl_Email(false);
                        NewCorrespondent = false;
                        this.BtnRecord.ToolTip = btnSaveAndRecord;
                        this.detailsCorrespondent.Style.Add(HtmlTextWriterStyle.Width, "59%");
                        this.detailsCorrespondent.Style.Add("float", "right");

                    }
                }
                #endregion

            }
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            if (this.SenderDetail != null)
            {
                switch (this.SenderDetail.tipoCorrispondente)
                {
                    case "L":
                        this.lbl_nomeCorr.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsTitleList", language);
                        break;
                    case "F":
                        this.lbl_nomeCorr.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsTitleRF", language);
                        break;
                    default:
                        this.lbl_nomeCorr.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsTitle", language);
                        break;
                }
            }
            //this.DetailsSenderK1K2LtrWarningK1K2.Text = Utils.Languages.GetLabelFromCode("DetailsSenderK1K2LtrWarningK1K2", language);
            if (Parameter != null && Parameter["tipoAvviso"].Equals("avviso1"))
                this.DetailsSenderK1K2Lt.Text = Utils.Languages.GetLabelFromCode("DetailsSenderK1K2LtModify", language);
            else if(Parameter != null)
                this.DetailsSenderK1K2Lt.Text = Utils.Languages.GetLabelFromCode("DetailsSenderK1K2LtChoice", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("DetailSenderK1K2BtnClose", language);
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
            this.lbl_email.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsEmail", language);
            this.lbl_codAOO.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCodAOO", language);
            this.lbl_codAmm.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCodAdmin", language);
            this.lblNote.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsNoteEmail", language);
            this.lbl_note.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsNote", language);
            this.lbl_preferredChannel.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsPreferredChannel", language);
            this.lbl_tipocorr.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsType", language);
            this.lbl_CodRubrica.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsAddressBookCode", language);
            this.lbl_descrizione.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsAddressBookDescription", language);
            this.lbl_CodR.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsAddressBookCode", language);
            this.lbl_DescR.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsAddressBookDescription", language);
            this.lbl_titolo.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsTitolo", language);
            this.lbl_nome.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsName", language);
            this.lbl_cognome.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsSurname", language);
            this.lbl_luogonascita.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsBirthplace", language);
            this.lbl_dataNascita.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsBirthday", language);
            this.lbl_Ruoli.Text = Utils.Languages.GetLabelFromCode("CorrespondentDetailsRoles", language);
            this.dg_listCorr.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("CorrespondentDetailsCode", language);
            this.dg_listCorr.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("CorrespondentDetailsDescription", language);
            this.ddl_tipoCorr.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectOptionNeutral", language));
            this.ddl_titolo.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectOptionNeutral", language));
            this.dd_canpref.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectOptionNeutral", language));
            this.imgAggiungiCasella.AlternateText = Utils.Languages.GetLabelFromCode("imgAggiungiCasella", language);
            this.imgAggiungiCasella.ToolTip = Utils.Languages.GetLabelFromCode("imgAggiungiCasella", language);
            viewDetails = Utils.Languages.GetLabelFromCode("DetailsSenderK1K2ViewDetails", language);
            hideDetails = Utils.Languages.GetLabelFromCode("DetailsSenderK1K2HideDetails", language);
            btnSaveAndRecord = Utils.Languages.GetLabelFromCode("DetailsSenderK1K2BtnTooltipSaveAndRecord", language);
            btnCreateAndRecord = Utils.Languages.GetLabelFromCode("DetailsSenderK1K2BtnTooltipCreateAndRecord", language);
            this.BtnRecord.Text = Utils.Languages.GetLabelFromCode("DetailsSenderK1K2BtnConfirm", language);
            this.BtnSaveOccasionalAndRecord.Text = Utils.Languages.GetLabelFromCode("DetailsSenderK1K2BtnSaveOccasionalAndRecord", language);
            this.BtnSaveOccasionalAndRecord.ToolTip = Utils.Languages.GetLabelFromCode("DetailsSenderK1K2BtnSaveOccasionalAndRecordTooltip", language);
        }

        #endregion

        protected string GetLabelDelete()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode("imgCasellaDelete", language);
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        private bool codice_rubrica_valido(string cod)
        {
            if (cod == null || cod.Trim() == "")
                return false;

            System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex(@"^[0-9A-Za-z_\ \.\-]+$");
            return rx.IsMatch(cod);
        }



        #region event hendler
        protected void BtnClose_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                this.CloseMask("abort");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnRecord_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                string message = string.Empty;
                string corr_type = ddl_tipoCorr.SelectedValue;
                string newIdCorr = "";
                string typeMessage = string.Empty;
                DatiModificaCorr datiModifica = new DatiModificaCorr();
                if (this.verificaSelezione(ref message) || ((CustomButton)sender).Text.Equals(this.BtnSaveOccasionalAndRecord.Text))
                {
                    //Emanuela: gestione utente occasionale
                    if (!((CustomButton)sender).Text.Equals(this.BtnSaveOccasionalAndRecord.Text))
                    {

                        switch (corr_type)
                        {
                            case "U":
                                if (DocumentInWorking.interop.Equals("S") || this.ddl_tipoCorr.SelectedValue.Equals("U"))
                                    this.modifyUO(ref datiModifica);
                                else if ((DocumentInWorking.interop.Equals("P") || DocumentInWorking.interop.Equals("E")) && (this.ddl_tipoCorr.SelectedValue.Equals("P")))
                                {
                                    modifyUtente(ref datiModifica);
                                }
                                else if ((DocumentInWorking.interop.Equals("P") || DocumentInWorking.interop.Equals("E")) && (this.ddl_tipoCorr.SelectedValue.Equals("R")))
                                {
                                    modifyRuolo(ref datiModifica);
                                }
                                break;
                            case "R":
                                this.modifyRuolo(ref datiModifica);
                                break;
                            case "P":
                                this.modifyUtente(ref datiModifica);
                                break;
                        }

                        //operazione andata a buon fine
                    }// fine if Emanuela occasionale
                    string idCorr = "";
                    #region Ci sono + di un corrispondente
                    if (Parameter.ContainsKey("same_mail"))
                    {
                        string codUpper = datiModifica.codRubrica.ToUpper();
                        #region Non scelgo niente
                        if (NewCorrespondent)
                        {
                            if (codUpper.Contains("INTEROP"))
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningChangeCodAddressBook', 'warning', '');", true);
                                return;
                            }
                            if (codUpper.Contains("@"))
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningCodeAddressBookNotContains@', 'warning', '');", true);
                                return;
                            }
                            #region Ho l'avviso 1
                            if (Parameter["tipoAvviso"].Equals("avviso1"))
                            {
                                if (UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "M", out newIdCorr, out message))
                                {
                                    switch (message)
                                    {
                                        case "OK":
                                            InsertComboMailsCorr(newIdCorr, ref message);
                                            message = "";
                                            break;
                                        default:
                                            message = "WarningAddressBookModifyKo";
                                            typeMessage = "error";
                                            break;
                                    }

                                    if (message != null && message != "")
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + message + "', '" + typeMessage + "', '');", true);
                                        return;
                                    }

                                    AddressBookManager.ResetCodRubCorrIterop(newIdCorr, datiModifica.codRubrica);
                                    DocumentManager.UpdateDocArrivoFromInterop(Parameter["systemId"], newIdCorr);
                                    if (newIdCorr != null)
                                    {
                                        NewIdCorr = newIdCorr;
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningCodeAddressBookExisting', 'warning', '');", true);
                                        return;
                                    }
                                }
                                else
                                {
                                    if (message != null && message != "")
                                    {
                                        message = "WarningAddressBookModifyKo";
                                        typeMessage = "error";
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + message + "', '" + typeMessage + "', '');", true);
                                    }
                                }
                            }
                            #endregion
                            #region Ho l'avviso 2
                            else if (Parameter["tipoAvviso"].Equals("avviso2"))
                            {
                                //creo l'oggetto canale
                                DocsPaWR.Canale canale = new DocsPaWR.Canale();
                                canale.systemId = this.dd_canpref.SelectedItem.Value;
                                #region Sono Interop
                                if (this.DocumentInWorking.interop.Equals("S"))
                                {
                                    //Creo il nuovo corrispondente e lo associ al documento
                                    UnitaOrganizzativa uo = new UnitaOrganizzativa();
                                    uo.tipoIE = Parameter["tipoIE"];
                                    //uo.tipoCorrispondente = t; // utilizzando questo prende S e genera eccezione dopo.
                                    uo.tipoCorrispondente = "U";
                                    uo.codiceAmm = datiModifica.codiceAmm;
                                    uo.codiceAOO = datiModifica.codiceAoo;
                                    uo.email = datiModifica.email;
                                    uo.codiceRubrica = datiModifica.codRubrica;
                                    uo.canalePref = canale;
                                    uo.descrizione = datiModifica.descCorr;
                                    uo.oldDescrizione = Parameter["OldDescrizioneMitt"];
                                    if (this.ddl_registri.SelectedValue != null)
                                        uo.idRegistro = this.ddl_registri.SelectedValue.Split('_')[0];
                                    else
                                        uo.idRegistro = string.Empty;
                                    uo.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                                    uo.localita = datiModifica.localita;
                                    if ((txt_indirizzo.Text != null && !txt_indirizzo.Equals("")) ||
                                            (txt_cap.Text != null && !txt_cap.Equals("")) ||
                                            (txt_citta.Text != null && !txt_citta.Equals("")) ||
                                            (txt_provincia.Text != null && !txt_provincia.Equals("")) ||
                                            (txt_nazione.Text != null && !txt_nazione.Equals("")) ||
                                            (txt_telefono.Text != null && !txt_telefono.Equals("")) ||
                                            (txt_telefono2 != null && !txt_telefono2.Equals("")) ||
                                            (txt_fax.Text != null && !txt_fax.Equals("")) ||
                                            (txt_local.Text != null && !txt_local.Equals("")) ||
                                            (txt_codfisc.Text != null && !txt_codfisc.Equals("")) ||
                                            (txt_note.Text != null && !txt_note.Equals("")) ||
                                             txt_partita_iva.Text != null && !txt_partita_iva.Equals(""))
                                    {
                                        uo = (DocsPaWR.UnitaOrganizzativa)AddressBookManager.ValorizeInfoCorr(uo, txt_indirizzo.Text.Trim(), txt_citta.Text.Trim(), txt_cap.Text.Trim(), txt_provincia.Text.Trim(), txt_nazione.Text.Trim(), txt_telefono.Text.Trim(), txt_telefono2.Text.Trim(), txt_fax.Text.Trim()
                                             , txt_codfisc.Text.Trim(), txt_note.Text.Trim(), txt_local.Text.Trim(), string.Empty, string.Empty, string.Empty, txt_partita_iva.Text.Trim(), "");
                                        uo.dettagli = true;
                                    }
                                    uo.canalePref = canale;

                                    Corrispondente newCorrispondente = UserManager.addressbookInsertCorrispondente(this, uo,
                                                                                                                    null);
                                    idCorr = newCorrispondente.systemId;
                                    InsertComboMailsCorr(idCorr, ref message);
                                    NewIdCorr = idCorr;
                                }
                                #endregion
                                #region Non sono Interop
                                else if (DocumentInWorking.interop.Equals("P") || DocumentInWorking.interop.Equals("E"))
                                {
                                    switch (corr_type)
                                    {
                                        case "U":

                                            //Creo il nuovo corrispondente e lo associ al documento
                                            UnitaOrganizzativa uo = new UnitaOrganizzativa();

                                            uo.tipoIE = Parameter["tipoIE"];
                                            uo.tipoCorrispondente = "U";
                                            uo.codiceAmm = datiModifica.codiceAmm;
                                            uo.codiceAOO = datiModifica.codiceAoo;
                                            uo.email = datiModifica.email;
                                            uo.codiceRubrica = datiModifica.codRubrica;
                                            uo.canalePref = canale;
                                            uo.descrizione = datiModifica.descCorr;
                                            if (this.ddl_registri.SelectedValue != null)
                                                uo.idRegistro = this.ddl_registri.SelectedValue.Split('_')[0];
                                            else
                                                uo.idRegistro = string.Empty;
                                            uo.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                                            uo.localita = datiModifica.localita;

                                            if ((txt_indirizzo.Text != null && !txt_indirizzo.Equals("")) ||
                                            (txt_cap.Text != null && !txt_cap.Equals("")) ||
                                            (txt_citta.Text != null && !txt_citta.Equals("")) ||
                                            (txt_provincia.Text != null && !txt_provincia.Equals("")) ||
                                            (txt_nazione.Text != null && !txt_nazione.Equals("")) ||
                                            (txt_telefono.Text != null && !txt_telefono.Equals("")) ||
                                            (txt_telefono2 != null && !txt_telefono2.Equals("")) ||
                                            (txt_fax.Text != null && !txt_fax.Equals("")) ||
                                            (txt_local.Text != null && !txt_local.Equals("")) ||
                                            (txt_codfisc.Text != null && !txt_codfisc.Equals("")) ||
                                            (txt_note.Text != null && !txt_note.Equals("")) ||
                                             txt_partita_iva.Text != null && !txt_partita_iva.Equals(""))
                                            {
                                                uo = (DocsPaWR.UnitaOrganizzativa)AddressBookManager.ValorizeInfoCorr(uo, txt_indirizzo.Text.Trim(), txt_citta.Text.Trim(), txt_cap.Text.Trim(), txt_provincia.Text.Trim(), txt_nazione.Text.Trim(), txt_telefono.Text.Trim(), txt_telefono2.Text.Trim(), txt_fax.Text.Trim()
                                                    , txt_codfisc.Text.Trim(), txt_note.Text.Trim(), txt_local.Text.Trim(), string.Empty, string.Empty, string.Empty, txt_partita_iva.Text.Trim(), string.Empty);
                                                uo.dettagli = true;
                                            }
                                            uo.canalePref = canale;

                                            codUpper = datiModifica.codRubrica.ToUpper();
                                            if (codUpper.Contains("INTEROP"))
                                            {
                                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningChangeCodAddressBook', 'warning', '');", true);
                                                return;
                                            }
                                            if (codUpper.Contains("@"))
                                            {
                                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningCodeAddressBookNotContains@', 'warning', '');", true);
                                                return;
                                            }

                                            Corrispondente newCorrispondente = UserManager.addressbookInsertCorrispondente(this, uo,
                                                                                                                            null);
                                            idCorr = newCorrispondente.systemId;
                                            InsertComboMailsCorr(idCorr, ref message);
                                            if (idCorr != null)
                                            {
                                                NewIdCorr = idCorr;
                                            }
                                            else
                                            {
                                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningCodeAddressBookExisting', 'warning', '');", true);
                                                return;
                                            }

                                            break;

                                        case "R":

                                            Corrispondente res = new Corrispondente();
                                            DocsPaWR.Ruolo ruolo = new Ruolo();
                                            ruolo.tipoCorrispondente = "R";
                                            ruolo.codiceCorrispondente = datiModifica.codRubrica;
                                            ruolo.codiceRubrica = datiModifica.codRubrica;
                                            ruolo.descrizione = datiModifica.descCorr;
                                            if (this.ddl_registri.SelectedValue != null)
                                                ruolo.idRegistro = this.ddl_registri.SelectedValue.Split('_')[0];
                                            else
                                                ruolo.idRegistro = string.Empty;
                                            ruolo.email = datiModifica.email;
                                            ruolo.codiceAmm = datiModifica.codiceAmm;
                                            ruolo.codiceAOO = datiModifica.codiceAoo;
                                            ruolo.localita = datiModifica.localita;
                                            ruolo.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                                            UnitaOrganizzativa parent_uo = new UnitaOrganizzativa();
                                            parent_uo.descrizione = "";
                                            parent_uo.systemId = "0";

                                            ruolo.canalePref = canale;
                                            res = UserManager.addressbookInsertCorrispondente(this, ruolo, parent_uo);
                                            idCorr = res.systemId;
                                            InsertComboMailsCorr(idCorr, ref message);

                                            if (idCorr != null)
                                            {
                                                NewIdCorr = idCorr;
                                            }
                                            else
                                            {
                                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningCodeAddressBookExisting', 'warning', '');", true);
                                                return;
                                            }
                                            break;

                                        case "P":
                                            res = new Corrispondente();
                                            Utente utente = new Utente();
                                            utente.codiceCorrispondente = datiModifica.codRubrica;
                                            utente.codiceRubrica = datiModifica.codRubrica;
                                            utente.cognome = datiModifica.cognome;
                                            utente.nome = datiModifica.nome;
                                            utente.email = datiModifica.email;
                                            utente.codiceAmm = datiModifica.codiceAmm;
                                            utente.codiceAOO = datiModifica.codiceAoo;
                                            utente.descrizione = datiModifica.descCorr;
                                            utente.luogoDINascita = datiModifica.luogoNascita;
                                            utente.dataNascita = datiModifica.dataNascita;
                                            utente.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                                            utente.titolo = datiModifica.titolo;
                                            if (this.ddl_registri.SelectedValue != null)
                                                utente.idRegistro = this.ddl_registri.SelectedValue.Split('_')[0];
                                            else
                                                utente.idRegistro = string.Empty;
                                            utente.tipoCorrispondente = datiModifica.tipoCorrispondente;
                                            utente.canalePref = canale;

                                            if ((txt_indirizzo.Text != null && !txt_indirizzo.Equals("")) ||
                                            (txt_cap.Text != null && !txt_cap.Equals("")) ||
                                            (txt_citta.Text != null && !txt_citta.Equals("")) ||
                                            (txt_provincia.Text != null && !txt_provincia.Equals("")) ||
                                            (txt_nazione.Text != null && !txt_nazione.Equals("")) ||
                                            (txt_telefono.Text != null && !txt_telefono.Equals("")) ||
                                            (txt_telefono2 != null && !txt_telefono2.Equals("")) ||
                                            (txt_fax.Text != null && !txt_fax.Equals("")) ||
                                            (txt_codfisc.Text != null && !txt_codfisc.Equals("")) ||
                                            (txt_note.Text != null && !txt_note.Equals("")) ||
                                                txt_partita_iva.Text != null && !txt_partita_iva.Equals(""))
                                            {
                                                utente.dettagli = true;

                                                utente = (DocsPaWR.Utente)AddressBookManager.ValorizeInfoCorr(utente, txt_indirizzo.Text.Trim(), txt_citta.Text.Trim(), txt_cap.Text.Trim(), txt_provincia.Text.Trim(), txt_nazione.Text.Trim(), txt_telefono.Text.Trim(), txt_telefono2.Text.Trim(), txt_fax.Text.Trim()
                                                    , txt_codfisc.Text.Trim(), txt_note.Text.Trim(), txt_local.Text.Trim(), ddl_titolo.SelectedValue, txt_luogoNascita.Text, txt_dataNascita.Text, txt_partita_iva.Text.Trim(), string.Empty);
                                            }

                                            res = UserManager.addressbookInsertCorrispondente(this, utente, null);
                                            idCorr = res.systemId;
                                            InsertComboMailsCorr(idCorr, ref message);

                                            if (idCorr != null)
                                            {
                                                NewIdCorr = idCorr;
                                            }
                                            else
                                            {
                                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningCodeAddressBookExisting', 'warning', '');", true);
                                                return;
                                            }
                                            break;
                                    }
                                }
                                #endregion

                            }
                            #endregion
                        }
                        #endregion
                        #region Scelgo
                        else
                        {
                            NewIdCorr = SenderDetail.systemId;
                            //return;
                        }
                        #endregion
                    }
                    #endregion

                    #region C'è zero o un solo corrispondente
                    else
                    {
                        //Emanuela:gestione occasionale
                        if (!((CustomButton)sender).Text.Equals(this.BtnSaveOccasionalAndRecord.Text))
                        {
                            //creo l'oggetto canale
                            Canale canale = new Canale();
                            canale.systemId = this.dd_canpref.SelectedItem.Value;

                            switch (corr_type)
                            {
                                case "U":
                                    //Creo il nuovo corrispondente e lo associ al documento
                                    UnitaOrganizzativa uo = new UnitaOrganizzativa();

                                    uo.tipoIE = Parameter["tipoIE"];
                                    //uo.tipoCorrispondente = t; // utilizzando questo prende S e genera eccezione dopo.
                                    uo.tipoCorrispondente = "U";
                                    uo.codiceAmm = datiModifica.codiceAmm;
                                    uo.codiceAOO = datiModifica.codiceAoo;
                                    uo.email = datiModifica.email;
                                    uo.codiceRubrica = datiModifica.codRubrica;
                                    uo.canalePref = canale;
                                    uo.descrizione = datiModifica.descCorr;
                                    uo.localita = datiModifica.localita;
                                    uo.oldDescrizione = Parameter["OldDescrizioneMitt"];
                                    if (this.ddl_registri.SelectedValue != null)
                                        uo.idRegistro = this.ddl_registri.SelectedValue.Split('_')[0];
                                    else
                                        uo.idRegistro = string.Empty;
                                    uo.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;

                                    if ((txt_indirizzo.Text != null && !txt_indirizzo.Equals("")) ||
                                                 (txt_cap.Text != null && !txt_cap.Equals("")) ||
                                                 (txt_citta.Text != null && !txt_citta.Equals("")) ||
                                                 (txt_provincia.Text != null && !txt_provincia.Equals("")) ||
                                                 (txt_nazione.Text != null && !txt_nazione.Equals("")) ||
                                                 (txt_telefono.Text != null && !txt_telefono.Equals("")) ||
                                                 (txt_telefono2 != null && !txt_telefono2.Equals("")) ||
                                                 (txt_fax.Text != null && !txt_fax.Equals("")) ||
                                                 (txt_local.Text != null && !txt_local.Equals("")) ||
                                                 (txt_codfisc.Text != null && !txt_codfisc.Equals("")) ||
                                                 (txt_note.Text != null && !txt_note.Equals("")) ||
                                                  txt_partita_iva.Text != null && !txt_partita_iva.Equals(""))
                                    {
                                        uo = (DocsPaWR.UnitaOrganizzativa)AddressBookManager.ValorizeInfoCorr(uo, txt_indirizzo.Text.Trim(), txt_citta.Text.Trim(), txt_cap.Text.Trim(), txt_provincia.Text.Trim(), txt_nazione.Text.Trim(), txt_telefono.Text.Trim(), txt_telefono2.Text.Trim(), txt_fax.Text.Trim()
                                            , txt_codfisc.Text.Trim(), txt_note.Text.Trim(), txt_local.Text.Trim(), string.Empty, string.Empty, string.Empty, txt_partita_iva.Text.Trim(), string.Empty);
                                        uo.dettagli = true;
                                    }

                                    string codUpper = datiModifica.codRubrica.ToUpper();

                                    if (codUpper.Contains("INTEROP"))
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningChangeCodAddressBook', 'warning', '');", true);
                                        return;
                                    }
                                    if (codUpper.Contains("@"))
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningCodeAddressBookNotContains@', 'warning', '');", true);
                                        return;
                                    }

                                    if (!Parameter["tipoAvviso"].Equals("2") && datiModifica.idRegistro.Equals(Parameter["IdRegistro"]))
                                    {
                                        // Se il codice è già presente in rubrica per lo specifico registro
                                        // non si deve procedere
                                        if (!AddressBookManager.IsCodRubricaPresente(datiModifica.codRubrica, corr_type, UserManager.GetInfoUser().idAmministrazione, datiModifica.idRegistro, datiModifica.inRubricaComune))
                                        {
                                            UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "M", out idCorr, out message);
                                            switch (message)
                                            {
                                                case "OK":
                                                    InsertComboMailsCorr(idCorr, ref message);
                                                    message = "";
                                                    break;
                                                default:
                                                    message = "WarningAddressBookModifyKo";
                                                    typeMessage = "error";
                                                    break;
                                            }

                                            if (message != null && message != "")
                                            {
                                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + message + "', '" + typeMessage + "', '');", true);
                                                return;
                                            }

                                            AddressBookManager.ResetCodRubCorrIterop(idCorr, datiModifica.codRubrica);
                                            DocumentManager.UpdateDocArrivoFromInterop(Parameter["systemId"], idCorr);
                                        }
                                        else
                                            idCorr = null;

                                        if (idCorr != null)
                                        {
                                            NewIdCorr = idCorr;
                                        }
                                        else
                                        {
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningCodeAddressBookExisting', 'warning', '');", true);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        Corrispondente newCorrispondente;

                                        // Se ci si trova nel caso di inserimento di un nuovo corrispondente, si procede 
                                        // con l'inserimento altrimenti viene utilizzato il corrispondente proposto
                                        if (NewCorrespondent)
                                            newCorrispondente = UserManager.addressbookInsertCorrispondente(this, uo, null);
                                        else
                                            newCorrispondente = AddressBookManager.getCorrispondenteByCodRubrica(uo.codiceRubrica, false);

                                        idCorr = newCorrispondente.systemId;
                                        InsertComboMailsCorr(idCorr, ref message);

                                        if (idCorr != null)
                                        {
                                            NewIdCorr = idCorr;
                                        }
                                        else
                                        {
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningCodeAddressBookExisting', 'warning', '');", true);
                                            return;
                                        }

                                    }
                                    break;

                                case "R":
                                    Corrispondente res = new Corrispondente();
                                    Ruolo ruolo = new Ruolo();
                                    ruolo.tipoCorrispondente = "R";
                                    ruolo.codiceCorrispondente = datiModifica.codRubrica;
                                    ruolo.codiceRubrica = datiModifica.codRubrica;
                                    ruolo.descrizione = datiModifica.descCorr;
                                    if (this.ddl_registri.SelectedValue != null)
                                        ruolo.idRegistro = this.ddl_registri.SelectedValue.Split('_')[0];
                                    else
                                        ruolo.idRegistro = string.Empty; 
                                    ruolo.email = datiModifica.email;
                                    ruolo.codiceAmm = datiModifica.codiceAmm;
                                    ruolo.codiceAOO = datiModifica.codiceAoo;
                                    ruolo.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                                    DocsPaWR.UnitaOrganizzativa parent_uo = new UnitaOrganizzativa();
                                    parent_uo.descrizione = "";
                                    parent_uo.systemId = "0";

                                    ruolo.canalePref = canale;
                                    //res = UserManager.addressbookInsertCorrispondente(this, ruolo, parent_uo);
                                    //idCorr = res.systemId;
                                    string codUpp = datiModifica.codRubrica.ToUpper();
                                    if (!Parameter["tipoAvviso"].Equals("avviso2") && datiModifica.codRubrica == Parameter["codiceRubrica"])
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningChangeCodAddressBook', 'warning', '');", true);
                                        return;
                                    }
                                    if (codUpp.Contains("INTEROP"))
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningChangeCodAddressBook', 'warning', '');", true);
                                        return;
                                    }
                                    if (codUpp.Contains("@"))
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningCodeAddressBookNotContains@', 'warning', '');", true);
                                        return;
                                    }

                                    if (!Parameter["tipoAvviso"].Equals("avviso2") && datiModifica.idRegistro.Equals(Parameter["IdRegistro"]))
                                    {
                                        // Se il codice è già presente in rubrica per lo specifico registro
                                        // non si deve procedere
                                        if (!AddressBookManager.IsCodRubricaPresente(datiModifica.codRubrica, corr_type, UserManager.GetInfoUser().idAmministrazione, datiModifica.idRegistro, datiModifica.inRubricaComune))
                                        {
                                            UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "D", out idCorr, out message);
                                            switch (message)
                                            {
                                                case "OK":
                                                    MultiBoxManager.DeleteMailCorrispondenteEsterno(idCorr);
                                                    message = "";
                                                    break;
                                                default:
                                                    message = "WarningAddressBookModifyKo";
                                                    typeMessage = "error";
                                                    break;
                                            }


                                            if (message != null && message != "")
                                            {
                                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + message + "', '" + typeMessage + "', '');", true);
                                                return;
                                            }
                                            //ws.ResetCodRubCorrIterop(idCorr, datiModifica.codRubrica);
                                            Corrispondente newCorrispondente = UserManager.addressbookInsertCorrispondente(this, ruolo, parent_uo);
                                            idCorr = newCorrispondente.systemId;
                                            InsertComboMailsCorr(idCorr, ref message);
                                            DocumentManager.UpdateDocArrivoFromInterop(Parameter["systemId"], idCorr);
                                        }
                                        else
                                        {
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningCodeAddressBookExisting', 'warning', '');", true);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        Corrispondente newCorrispondente;

                                        // Se ci si trova nel caso di inserimento di un nuovo corrispondente, si procede 
                                        // con l'inserimento altrimenti viene utilizzato il corrispondente proposto
                                        if (NewCorrespondent)
                                            newCorrispondente = UserManager.addressbookInsertCorrispondente(this, ruolo, parent_uo);
                                        else
                                            newCorrispondente = UserManager.getCorrispondentBySystemID(datiModifica.idCorrGlobali);

                                        idCorr = newCorrispondente.systemId;
                                        InsertComboMailsCorr(idCorr, ref message);
                                    }

                                    if (idCorr != null)
                                    {
                                        NewIdCorr = idCorr;
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningCodeAddressBookExisting', 'warning', '');", true);
                                        return;
                                    }
                                    break;

                                case "P":
                                    res = new Corrispondente();
                                    Utente utente = new Utente();
                                    utente.codiceCorrispondente = datiModifica.codRubrica;
                                    utente.codiceRubrica = datiModifica.codRubrica;
                                    utente.cognome = datiModifica.cognome;
                                    utente.nome = datiModifica.nome;
                                    utente.email = datiModifica.email;
                                    utente.codiceAmm = datiModifica.codiceAmm;
                                    utente.codiceAOO = datiModifica.codiceAoo;
                                    utente.descrizione = datiModifica.descCorr;
                                    utente.luogoDINascita = datiModifica.luogoNascita;
                                    utente.dataNascita = datiModifica.dataNascita;
                                    utente.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                                    utente.titolo = datiModifica.titolo;
                                    if (this.ddl_registri.SelectedValue != null)
                                        utente.idRegistro = this.ddl_registri.SelectedValue.Split('_')[0];
                                    else
                                        utente.idRegistro = string.Empty;
                                    utente.tipoCorrispondente = "P";
                                    utente.canalePref = canale;

                                    if ((txt_indirizzo.Text != null && !txt_indirizzo.Equals("")) ||
                                                (txt_cap.Text != null && !txt_cap.Equals("")) ||
                                                (txt_citta.Text != null && !txt_citta.Equals("")) ||
                                                (txt_provincia.Text != null && !txt_provincia.Equals("")) ||
                                                (txt_nazione.Text != null && !txt_nazione.Equals("")) ||
                                                (txt_telefono.Text != null && !txt_telefono.Equals("")) ||
                                                (txt_telefono2 != null && !txt_telefono2.Equals("")) ||
                                                (txt_fax.Text != null && !txt_fax.Equals("")) ||
                                                (txt_codfisc.Text != null && !txt_codfisc.Equals("")) ||
                                                (txt_note.Text != null && !txt_note.Equals("")) ||
                                                    txt_partita_iva.Text != null && !txt_partita_iva.Equals(""))
                                    {
                                        utente.dettagli = true;

                                        utente = (DocsPaWR.Utente)AddressBookManager.ValorizeInfoCorr(utente, txt_indirizzo.Text.Trim(), txt_citta.Text.Trim(), txt_cap.Text.Trim(), txt_provincia.Text.Trim(), txt_nazione.Text.Trim(), txt_telefono.Text.Trim(), txt_telefono2.Text.Trim(), txt_fax.Text.Trim()
                                            , txt_codfisc.Text.Trim(), txt_note.Text.Trim(), txt_local.Text.Trim(), ddl_titolo.SelectedValue, txt_luogoNascita.Text, txt_dataNascita.Text, txt_partita_iva.Text.Trim(), string.Empty);
                                    }

                                    if (datiModifica.codRubrica.Contains("INTEROP"))
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningChangeCodAddressBook', 'warning', '');", true);
                                        return;
                                    }

                                    if (datiModifica.codRubrica.Contains("@"))
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningCodeAddressBookNotContains@', 'warning', '');", true);
                                        return;
                                    }

                                    if (!Parameter["tipoAvviso"].Equals("avviso2") && datiModifica.idRegistro.Equals(Parameter["IdRegistro"]))
                                    {
                                        // Se il codice è già presente in rubrica per lo specifico registro
                                        // non si deve procedere
                                        if (!AddressBookManager.IsCodRubricaPresente(datiModifica.codRubrica, corr_type, UserManager.GetInfoUser().idAmministrazione, datiModifica.idRegistro, datiModifica.inRubricaComune))
                                        {
                                            UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "M",
                                                                                        out idCorr,
                                                                                        out message);
                                            switch (message)
                                            {
                                                case "OK":
                                                    InsertComboMailsCorr(idCorr, ref message);
                                                    message = "";
                                                    break;
                                                default:
                                                    message = "WarningAddressBookModifyKo";
                                                    typeMessage = "error";
                                                    break;
                                            }

                                            if (message != null && message != "")
                                            {
                                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + message + "', '" + typeMessage + "', '');", true);
                                                return;
                                            }
                                            AddressBookManager.ResetCodRubCorrIterop(idCorr, datiModifica.codRubrica);
                                            DocumentManager.UpdateDocArrivoFromInterop(Parameter["systemId"], idCorr);
                                        }
                                        else
                                        {
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningCodeAddressBookExisting', 'warning', '');", true);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        Corrispondente newCorrispondente;

                                        // Se ci si trova nel caso di inserimento di un nuovo corrispondente, si procede 
                                        // con l'inserimento altrimenti viene utilizzato il corrispondente proposto
                                        if (NewCorrespondent)
                                            newCorrispondente = UserManager.addressbookInsertCorrispondente(this, utente, null);
                                        else
                                            newCorrispondente = UserManager.getCorrispondentBySystemID(datiModifica.idCorrGlobali);

                                        idCorr = newCorrispondente.systemId;
                                        InsertComboMailsCorr(idCorr, ref message);
                                    }

                                    if (idCorr != null)
                                    {
                                        NewIdCorr = idCorr;
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningCodeAddressBookExisting', 'warning', '');", true);
                                        return;
                                    }
                                    break;
                            }
                        }//fine if Emanuela occasionale
                        #region Creazione corrispondente occasionale
                        else //Se il corrispondente è di tipo occasionale
                        {
                            datiModifica.idCorrGlobali = Parameter["systemId"];
                            datiModifica.codRubrica = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                            datiModifica.codiceAmm = this.txt_codAmm.Text;
                            datiModifica.descCorr = this.txt_email.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                            datiModifica.tipoCorrispondente = "O";

                            if (UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "M", out idCorr, out message))
                            {
                                switch (message)
                                {
                                    case "OK":
                                        message = "";
                                        break;
                                    default:
                                        message = "WarningAddressBookModifyKo";
                                        typeMessage = "error";
                                        break;
                                }

                                if (message != null && message != "")
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + message + "', '" + typeMessage + "', '');", true);
                                    return;
                                }
                                string sysid_doc = DocumentInWorking.systemId;
                                DocumentManager.UpdateDocArrivoFromInteropOccasionale(sysid_doc, idCorr);
                                if (idCorr != null)
                                {
                                    NewIdCorr = idCorr;
                                }
                                else
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('WarningAddressBookModifyKo', 'error', '');", true);
                                }
                            }
                        }
                        #endregion
                        #endregion
                    }
                }

                else
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + utils.FormatJs(message) + "', 'warning', '');", true);
                        return;
                    }

                }

                CloseMask("record");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnAddCorrespondent_Click(object sender, EventArgs e)
        {
            try {
                setCampiReadOnly(false);
                NewCorrespondent = true;
                this.BtnRecord.ToolTip = btnCreateAndRecord;
                this.IsEmailReadOnly = true;
                this.EnableInsertMail();
                this.txt_CodRubrica.Text = string.Empty;
                this.txt_CodR.Text = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                this.PlaceHolderAddCorrespondent.Visible = true;
                this.UpButtons.Update();
                this.UpdatePanelDetailsCorrespondentRole.Update();
                this.UpdatePanelDetails.Update();
                this.UpdatePanelTypeCorr.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void repListCorrespond_Bound(object sender, RepeaterItemEventArgs e)
        {
            try {
                if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                    return;
                RadioButton rd = (RadioButton)e.Item.FindControl("rd");
                string script = "SetSingleRadioButton('" + rd.ClientID + "',this)";
                rd.Attributes.Add("onclick", script);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void rbCorrespondent_OnCheckedChanged(object sender, EventArgs e)
        {
            try {
                bool visibleBtnAddAdd = UserManager.IsAuthorizedFunctions("GEST_RUBRICA");
                string systemIdCorrespondent = string.Empty;
                bool enabled = repListCorrespond.Items.Count > 1;
                foreach (RepeaterItem item in repListCorrespond.Items)
                {
                    bool check = (item.FindControl("rd") as RadioButton).Checked;
                    if (check)
                    {
                        systemIdCorrespondent = (item.FindControl("systemIdCorrespondent") as HiddenField).Value;
                        (item.FindControl("btnAddCorrespondent") as ImageButton).Enabled = true;
                        (item.FindControl("btnAddCorrespondent") as ImageButton).Visible = visibleBtnAddAdd;
                        if (!systemIdCorrespondent.Equals(Parameter["systemId"]))
                            (item.FindControl("btnDeleteCorrespondent") as ImageButton).Enabled = enabled;
                    }
                    else
                    {
                        (item.FindControl("btnAddCorrespondent") as ImageButton).Enabled = false;
                        (item.FindControl("btnAddCorrespondent") as ImageButton).Visible = visibleBtnAddAdd;
                        (item.FindControl("btnDeleteCorrespondent") as ImageButton).Enabled = false;
                    }
                }
                SenderDetail = (from corr in listCorrespondentSameMail where corr.systemId.Equals(systemIdCorrespondent) select corr).FirstOrDefault();
                this.drawDettagliCorr();
                setCampiReadOnly(true);
                enablePnl_Email(false);
                NewCorrespondent = false;
                this.PlaceHolderAddCorrespondent.Visible = false;
                this.BtnRecord.ToolTip = btnSaveAndRecord;
                this.UpButtons.Update();
                this.UpdatePanelDetails.Update();
                this.UpdatePanelTypeCorr.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public void viewOtherDetailsLink_Click(object sender, EventArgs e)
        {
            try {
                this.otherDetails.Visible = (this.otherDetails.Visible) ? false : true;
                if (this.otherDetails.Visible)
                    this.viewOtherDetailsLink.Text = hideDetails;
                else
                    this.viewOtherDetailsLink.Text = viewDetails;
                enablePnl_Email(false);
                this.UpPanelOtherDetails.Update();
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
                this.IsModifiedCaselle = true;

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
                this.IsModifiedCaselle = true;
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
                this.IsModifiedCaselle = true;
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
                this.IsModifiedCaselle = true;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void imgEliminaCasella_Click(object sender, ImageClickEventArgs e)
        {
            try  {
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
                this.IsModifiedCaselle = true;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_tipoCorr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                switch (((DropDownList)sender).SelectedValue)
                {
                    case "P":
                        pnlRuolo.Visible = false;
                        this.pnlStandard.Visible = true;
                        this.panelInStandard2.Visible = true;
                        this.pnlInStandard1.Visible = true;
                        pnl_infonascita.Visible = true;
                        pnl_nome_cogn.Visible = true;
                        txt_nome.ReadOnly = false;
                        txt_cognome.ReadOnly = false;
                        pnl_descrizione.Visible = false;
                        pnl_titolo.Visible = true;
                        pnl_indirizzo.Visible = true;
                        break;
                    case "U":
                        pnlRuolo.Visible = false;
                        this.pnlStandard.Visible = true;
                        this.panelInStandard2.Visible = true;
                        this.pnlInStandard1.Visible = true;
                        pnl_infonascita.Visible = false;
                        pnl_nome_cogn.Visible = false;
                        pnl_descrizione.Visible = true;
                        pnl_titolo.Visible = false;
                        pnl_indirizzo.Visible = true;
                        break;
                    case "R":
                        pnl_infonascita.Visible = false;
                        pnl_nome_cogn.Visible = false;
                        pnl_descrizione.Visible = true;
                        pnl_titolo.Visible = false;
                        pnl_indirizzo.Visible = false;
                        break;
                }

                if (this.BtnSaveOccasionalAndRecord.Enabled)
                {
                    this.BtnSaveOccasionalAndRecord.Enabled = false;
                    NewCorrespondent = true;
                    this.BtnRecord.Enabled = true;
                    DisableAutoPostBack();
                    this.UpButtons.Update();
                }
                this.UpdatePanelDetailsCorrespondentRole.Update();
                this.UpdatePanelTypeCorr.Update();
                this.UpdatePanelDetails.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void dd_canpref_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try {
                this.setVisibilityPanelStar();

                //SE L'UTENTE STA MODIFICANDO IL CANALE PREFERENZIALE DEL CORRISPONDENTE
                if (this.SenderDetail.canalePref.systemId != dd_canpref.SelectedItem.Value)
                {
                    this.IsModifiedChannel = true;
                }
                else
                {
                    this.IsModifiedChannel = false;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnDelete_Click(object sender, EventArgs e)
        {

            try {
                proceed_delete.Value = "false";
                string msg = "";
                Registro r = null;
                if (this.SenderDetail.idRegistro != "")
                {
                    r = RegistryManager.getRegistroBySistemId(this.SenderDetail.idRegistro);
                    if ((r.chaRF == "0") && !UserManager.IsAuthorizedFunctions("DO_MOD_CORR_REG"))
                    {
                        msg = "WarningAddressBookDeleteNotAllowed";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        return;
                    }
                    else if ((r.chaRF == "1") && !UserManager.IsAuthorizedFunctions("DO_MOD_CORR_RF"))
                    {
                        msg = "WarningAddressBookDeleteNotAllowed";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                        return;
                    }
                }
                else if (!UserManager.IsAuthorizedFunctions("DO_MOD_CORR_TUTTI"))
                {
                    msg = "WarningAddressBookDeleteNotAllowed";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '');", true);
                    return;
                }

                msg = "ConfirmAddressBookDeleteCorrespondent";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('" + utils.FormatJs(msg) + "', 'proceed_delete', '" + utils.FormatJs(this.GetLabel("AddressBookDetailsConfirmDelete")) + "');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnViewFileDocument_Onclick(object sender, EventArgs e)
        {
            try
            {
                this.IsZoom = true;
                this.OpenZoomFromDetailsSender = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDocumentViewer", "parent.ajaxModalPopupDocumentViewer();", true);
                NttDataWA.Popup.DocumentViewer.OpenDocumentViewer = true;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
        

        #endregion

        #region Grid manager

        private void RepListCorrespond_Bind()
        {
            string allRegistri = "";
            //Registro[] reg = UserManager.getListaRegistriWithRF(this, "", "");
            Registro[] reg = RegistryManager.GetRegAndRFListInSession();
            foreach (Registro registro in reg)
            {
                allRegistri += registro.systemId + "','";
            }
            if (!string.IsNullOrEmpty(allRegistri))
                allRegistri = allRegistri.Substring(0, allRegistri.Length - 3);
            else
            {
                //ovvero la UserManager.getListaRegistriWithRF(this, "1", idAOOCOLL) 
                //non ritorna alcun RF o reg...
                allRegistri += Parameter["idAOOCOLL"];

            }
            DataSet ds = new DataSet();
            string email = SenderDetail.email;
            if (!string.IsNullOrEmpty((this.DocumentInWorking.protocollo as ProtocolloEntrata).emailMittente) &&
                (from e in SenderDetail.Emails where e.Email.Equals((this.DocumentInWorking.protocollo as ProtocolloEntrata).emailMittente) select e).FirstOrDefault() != null)
            {
                email = (this.DocumentInWorking.protocollo as ProtocolloEntrata).emailMittente;
            }
            if (this.DocumentInWorking.interop.ToString().Equals("P") || this.DocumentInWorking.interop.ToString().Equals("E"))
                ds = AddressBookManager.GetCorrByEmail(email, allRegistri);
            else
                ds = AddressBookManager.GetCorrByEmailAndDescr(email, SenderDetail.oldDescrizione, allRegistri);
            listCorrespondentSameMail = new List<Corrispondente>();
            if (ds.Tables[0].Rows.Count > 0)
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Corrispondente corr = new Corrispondente();
                    corr.systemId = row["SYSTEM_ID"].ToString();
                    corr.descrizione = row["VAR_DESC_CORR"].ToString();
                    corr.codiceRubrica = row["VAR_COD_RUBRICA"].ToString();
                    corr.email = SenderDetail.email;
                    if (this.DocumentInWorking.interop.ToString().Equals("P") || this.DocumentInWorking.interop.ToString().Equals("E"))
                        corr.idRegistro = row["ID_REGISTRO"] != DBNull.Value ? RegistryManager.getRegistroBySistemId(row["ID_REGISTRO"].ToString()).codRegistro : String.Empty;
                    else
                        corr.idRegistro = row["VAR_COD_REGISTRO"].ToString();
                    listCorrespondentSameMail.Add(corr);
                }
            listObj = new List<Corrispondente>();
            listObj =
            (from corr in listCorrespondentSameMail
             select
                 new Corrispondente { descrizione = corr.descrizione, codiceRubrica = "- " + corr.codiceRubrica, idRegistro = "(" + (string.IsNullOrEmpty(corr.idRegistro) ? "TUTTI[RC]" : corr.idRegistro) + ")", systemId = corr.systemId }).ToList();

            this.repListCorrespond.DataSource = listObj;
            this.repListCorrespond.DataBind();

            bool visibleBtnAddAdd = UserManager.IsAuthorizedFunctions("GEST_RUBRICA");
            foreach (RepeaterItem item in repListCorrespond.Items)
                    (item.FindControl("btnAddCorrespondent") as ImageButton).Visible = visibleBtnAddAdd;
        }


        protected void gvCaselle_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && this.IsEmailReadOnly)
            {
                (e.Row.FindControl("txtEmailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = true;
                (e.Row.FindControl("txtNoteMailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = true;
                (e.Row.FindControl("rdbPrincipale") as System.Web.UI.WebControls.RadioButton).Enabled = false;
                (e.Row.FindControl("imgEliminaCasella") as System.Web.UI.WebControls.ImageButton).Enabled = false;
            }
            else if (e.Row.RowType == DataControlRowType.DataRow && (!this.IsEmailReadOnly))
            {
                if (this.SenderDetail != null && this.SenderDetail.inRubricaComune)
                {
                    (e.Row.FindControl("txtEmailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = true;
                    (e.Row.FindControl("txtNoteMailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = true;
                    (e.Row.FindControl("rdbPrincipale") as System.Web.UI.WebControls.RadioButton).Enabled = false;
                    (e.Row.FindControl("imgEliminaCasella") as System.Web.UI.WebControls.ImageButton).Enabled = false;
                }
                else
                {
                    (e.Row.FindControl("txtEmailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = false;
                    (e.Row.FindControl("txtNoteMailCorr") as System.Web.UI.WebControls.TextBox).ReadOnly = false;
                    (e.Row.FindControl("rdbPrincipale") as System.Web.UI.WebControls.RadioButton).Enabled = true;
                    (e.Row.FindControl("imgEliminaCasella") as System.Web.UI.WebControls.ImageButton).Enabled = true;
                }
            }
        }



        #endregion

        protected void CloseMask(string returnValue)
        {
            string idPopup = Parameter["tipoAvviso"].Equals("avviso2") ? "DetailsSenderK2" : "DetailsSenderK1";
            this.DeleteProperty();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "parent.closeAjaxModal('" + idPopup + "', '" + returnValue + "', parent);", true);
        }

        private void setCampiReadOnly(bool enabled)
        {
            this.txt_codAmm.ReadOnly = enabled;
            this.txt_codAOO.ReadOnly = enabled;
            this.txt_email.ReadOnly = true;
            this.txt_CodR.ReadOnly = enabled;
            this.txt_DescR.ReadOnly = enabled;
            if(DocumentInWorking.interop.Equals("S"))
                this.ddl_tipoCorr.Enabled = false;
            else
                this.ddl_tipoCorr.Enabled = !enabled;
            this.txt_CodRubrica.ReadOnly = enabled;
            this.txt_descrizione.ReadOnly = enabled;
            this.txt_indirizzo.ReadOnly = enabled;
            this.txt_cap.ReadOnly = enabled;
            this.txt_citta.ReadOnly = enabled;
            this.txt_local.ReadOnly = enabled;
            this.txt_provincia.ReadOnly = enabled;
            this.txt_nazione.ReadOnly = enabled;
            this.txt_telefono.ReadOnly = enabled;
            this.txt_telefono2.ReadOnly = enabled;
            this.txt_fax.ReadOnly = enabled;
            this.txt_codfisc.ReadOnly = enabled;
            this.txt_partita_iva.ReadOnly = enabled;
            this.txt_nome.ReadOnly = enabled;
            this.txt_cognome.ReadOnly = enabled;
            this.txt_note.ReadOnly = enabled;
            this.dd_canpref.Enabled = false;
            this.txt_luogoNascita.ReadOnly = enabled;
            this.txt_dataNascita.ReadOnly = enabled;
            this.ddl_titolo.Enabled = !enabled;
            this.txtNote.ReadOnly = enabled;
            this.txtCasella.ReadOnly = enabled;
            this.imgAggiungiCasella.Enabled = !enabled;
        }

        private void drawDettagliCorr()
        {
            if (this.SenderDetail != null)
            {
                SenderDetail = AddressBookManager.GetCorrespondentBySystemId(SenderDetail.systemId);
                if (Parameter["tipoCorrispondente"] == "L" || Parameter["tipoCorrispondente"] == "F")
                {
                    this.pnl_registro.Visible = false;
                    this.pnl_email.Visible = false;
                    this.pnlStandard.Visible = false;
                    this.pnlRuolo.Visible = false;
                    this.pnlRuoliUtente.Visible = false;
                    this.PanelListaCorrispondenti.Visible = true;

                    ArrayList listaCorrispondenti = new ArrayList();

                    if (Parameter["tipoCorrispondente"] == "L")
                    {
                        this.lbl_nomeLista.Text = UserManager.getNomeLista(this, this.SenderDetail.codiceRubrica, UserManager.GetInfoUser().idAmministrazione) + " (" + Parameter["codiceRubrica"] + ")";
                        listaCorrispondenti = UserManager.getCorrispondentiByCodLista(this, Parameter["codiceRubrica"], UserManager.GetInfoUser().idAmministrazione);
                    }
                    else
                    {
                        this.lbl_nomeLista.Text = UserManager.getNomeRF(this, Parameter["codiceRubrica"]);
                        listaCorrispondenti = UserManager.getCorrispondentiByCodRF(this, Parameter["codiceRubrica"]);
                    }

                    this.dg_listCorr.DataSource = this.creaDataTable(listaCorrispondenti);
                    this.dg_listCorr.DataBind();
                }
                else
                {
                    // registro è popolato solo per i corrisp esterni
                    if (this.SenderDetail.tipoIE != null && this.SenderDetail.tipoIE.Equals("E"))
                    {
                        this.pnl_registro.Visible = true;

                        if (this.SenderDetail.idRegistro == null || (this.SenderDetail.idRegistro != null && this.SenderDetail.idRegistro.Trim() == ""))
                        {
                            this.lbl_registro.Text = this.GetLabel("CorrespondentDetailsRegistry");
                            this.lit_registro.Text = "TUTTI [RC]";
                            this.ddl_registri.Visible = false;
                            this.lit_registro.Visible = true;
                        }
                        else
                        {
                            this.ddl_registri.Visible = true;
                            this.lit_registro.Visible = false;
                            DocsPaWR.Registro regCorr = UserManager.getRegistroBySistemId(this, this.SenderDetail.idRegistro);
                            if (regCorr != null)
                            {
                                LoadRegistries(regCorr.codRegistro);
                                this.lit_registro.Text = regCorr.codRegistro;
                                if (regCorr.chaRF == "0")
                                    this.lbl_registro.Text = this.GetLabel("CorrespondentDetailsRegistry");
                                else
                                    this.lbl_registro.Text = this.GetLabel("CorrespondentDetailsRF");
                            }
                        }

                        //this.txt_email.Visible = NewCorrespondent? false : true;
                        //this.plcNoteMail.Visible = true;
                        //this.IsEmailReadOnly = true;
                        //this.EnableInsertMail();
                        //this.CaselleList = null;
                        //this.BindGridViewCaselle(this.SenderDetail);
                    }
                    //else
                    //{
                    //    this.txt_email.Visible = true;
                    //    this.txtCasella.Visible = false;
                    //    this.plcNoteMail.Visible = false;
                    //    this.imgAggiungiCasella.Visible = false;
                    //    this.updPanelMail.Visible = false;
                    //}

                    this.txt_email.Visible = false;
                    this.plcNoteMail.Visible = true;
                    this.IsEmailReadOnly = true;
                    this.EnableInsertMail();
                    this.CaselleList = null;
                    this.BindGridViewCaselle(this.SenderDetail);

                    // tipo corrispondente
                    this.LoadCorrespondentTypes();

                    // titoli
                    this.LoadTitles();

                    // canale preferenziale
                    this.LoadPreferredChannels(false);
                    if (this.SenderDetail.tipoIE == "E")
                    {
                        this.plcPreferredChannel.Visible = true;
                        if (this.SenderDetail.canalePref != null)
                        {
                            this.dd_canpref.SelectedIndex = this.dd_canpref.Items.IndexOf(this.dd_canpref.Items.FindByValue(this.SenderDetail.canalePref.systemId));
                            this.setVisibilityPanelStar();
                        }
                    }
                    else
                    {
                        this.plcPreferredChannel.Visible = false;
                    }


                    // campi rubrica
                    if (!(this.SenderDetail is DocsPaWR.Ruolo))
                    {
                        this.pnlStandard.Visible = true;
                        this.panelInStandard2.Visible = true;
                        this.pnlInStandard1.Visible = true;
                        this.pnlRuolo.Visible = false;
                        this.pnl_indirizzo.Visible = true;

                        if (this.SenderDetail is DocsPaWR.Utente)
                        {
                            this.pnl_nome_cogn.Visible = true;

                            // campi visibili solo nel caso di utente esterno
                            if (this.SenderDetail.tipoIE.Equals("E"))
                            {
                                this.pnl_titolo.Visible = true;
                                this.pnl_infonascita.Visible = true;
                            }
                            DocsPaWR.Utente u = (DocsPaWR.Utente)this.SenderDetail;

                            // dati utente
                            this.txt_cognome.Text = u.cognome;
                            this.txt_nome.Text = u.nome;

                            string id_amm = UserManager.GetInfoUser().idAmministrazione;
                            DocsPaWR.ElementoRubrica[] ers = UserManager.GetRuoliUtente(this, id_amm, u.codiceRubrica);
                            this.lblRuoli.Text = "";

                            if (ers.Length > 0)
                            {
                                foreach (DocsPaWR.ElementoRubrica er in ers)
                                {
                                    this.lblRuoli.Text += (er.descrizione + "<br />");
                                }
                                this.lblRuoli.Visible = true;
                                this.lbl_Ruoli.Visible = true;
                            }

                            if (!this.SenderDetail.tipoIE.Equals("E"))
                                this.pnlRuoliUtente.Visible = true;

                            this.pnl_descrizione.Visible = false;
                            this.pnl_titolo.Visible = true;
                            this.pnl_nome_cogn.Visible = true;
                            this.pnl_infonascita.Visible = true;
                        }
                        else
                        {
                            this.pnl_descrizione.Visible = true;
                            this.pnl_titolo.Visible = false;
                            this.pnl_nome_cogn.Visible = false;
                            this.pnl_infonascita.Visible = false;
                        }
                    }
                    else
                    {
                        this.pnlStandard.Visible = true;
                        this.panelInStandard2.Visible = false;
                        this.pnlInStandard1.Visible = false;
                        this.pnlRuolo.Visible = true;
                        this.pnl_descrizione.Visible = false;
                        this.pnl_titolo.Visible = false;
                        this.pnl_nome_cogn.Visible = false;
                        this.pnl_infonascita.Visible = false;
                        this.pnl_indirizzo.Visible = false;
                        DocsPaWR.Ruolo u = (DocsPaWR.Ruolo)this.SenderDetail;

                        this.txt_CodR.Text = u.codiceRubrica;
                        this.txt_DescR.Text = u.descrizione;
                    }


                    DocsPaWR.CorrespondentDetails corrDetails = AddressBookManager.GetCorrespondentDetails(SenderDetail.systemId);
                    if (corrDetails != null)
                    {
                        this.ddl_titolo.SelectedIndex = this.ddl_titolo.Items.IndexOf(this.ddl_titolo.Items.FindByText(corrDetails.Title));
                        this.txt_indirizzo.Text = corrDetails.Address;
                        this.txt_cap.Text = corrDetails.ZipCode;
                        this.txt_citta.Text = corrDetails.City;
                        this.txt_provincia.Text = corrDetails.District;
                        this.txt_local.Text = corrDetails.Place;
                        this.txt_nazione.Text = corrDetails.Country;
                        this.txt_telefono.Text = corrDetails.Phone;
                        this.txt_telefono2.Text = corrDetails.Phone2;
                        this.txt_fax.Text = corrDetails.Fax;
                        this.txt_codfisc.Text = corrDetails.TaxId;
                        this.txt_partita_iva.Text = corrDetails.CommercialId;
                        this.txt_note.Text = corrDetails.Note;
                    }

                    this.txt_CodRubrica.Text = SenderDetail.codiceRubrica;

                    if (this.ActiveCodeDescriptionAdministrationSender && !string.IsNullOrEmpty(SenderDetail.codDescAmministrizazione))
                        this.txt_descrizione.Text = SenderDetail.codDescAmministrizazione + SenderDetail.descrizione;
                    else
                        this.txt_descrizione.Text = SenderDetail.descrizione;

                    this.txt_email.Text = SenderDetail.email;

                    if (!string.IsNullOrEmpty(SenderDetail.email) && (SenderDetail.Emails == null || SenderDetail.Emails.Length == 0))
                        this.txtCasella.Text = SenderDetail.email;

                    this.txt_codAmm.Text = SenderDetail.codiceAmm;
                    this.txt_codAOO.Text = SenderDetail.codiceAOO;
                    this.txt_nome.Text = SenderDetail.nome;
                    this.txt_cognome.Text = SenderDetail.cognome;
                    this.txt_dataNascita.Text = SenderDetail.dataNascita;
                    this.txt_luogoNascita.Text = SenderDetail.luogoDINascita;
                }
            }
        }

        private DocsPaWR.Registro[] getListaRegistri()
        {
            //prendo i registri/RF visibili al ruolo dell'utente
            this.ddl_registri.Enabled = true;
            //DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistriWithRF(this, "", "");
            DocsPaWR.Registro[] userRegistri = RegistryManager.GetRegAndRFListInSession();
            return userRegistri;
        }


        protected void LoadRegistries(string codiceRegistroSelezionato)
        {
            DocsPaWR.Registro[] userRegistri = this.getListaRegistri();
            ddl_registri.Items.Clear();
            if (userRegistri != null && userRegistri.Length > 0)
            {
                //Spaziatura per RF
                string strText = "";
                //int contatoreRegRF = 0;
                for (short iff = 0; iff < 3; iff++)
                {
                    strText += " ";
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
                                if (codiceRegistroSelezionato.Equals(userRegistri[i].codRegistro))
                                    this.ddl_registri.SelectedValue = item.Value;
                            }
                        }
                        else
                        {
                            //Registro
                            if ((UserManager.IsAuthorizedFunctions("DO_INS_CORR_REG") && !userRegistri[i].flag_pregresso))
                            {
                                string testo = userRegistri[i].codRegistro;
                                ListItem item = new ListItem();
                                item.Text = testo;
                                item.Value = userRegistri[i].systemId + "_" + userRegistri[i].rfDisabled + "_" + userRegistri[i].idAOOCollegata;
                                this.ddl_registri.Items.Add(item);
                                if (codiceRegistroSelezionato.Equals(userRegistri[i].codRegistro))
                                    this.ddl_registri.SelectedValue = item.Value;
                            }

                        }
                    }
                }
            }
        }




        protected string GetIdRb(Corrispondente c)
        {
            return c.systemId;
        }

        private void enablePnl_Email(bool enable)
        {
            this.txt_email.Enabled = enable;
            this.txtCasella.Enabled = enable;
            this.txtNote.Enabled = enable;
            this.imgAggiungiCasella.Enabled = enable;
            this.gvCaselle.Enabled = enable;
            this.dd_canpref.Enabled = enable;
        }

        private DataTable creaDataTable(ArrayList listaCorrispondenti)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CODICE");
            dt.Columns.Add("DESCRIZIONE");

            for (int i = 0; i < listaCorrispondenti.Count; i++)
            {
                DocsPaWR.Corrispondente c = (DocsPaWR.Corrispondente)listaCorrispondenti[i];
                DataRow dr = dt.NewRow();
                if (c.disabledTrasm)
                {
                    dr[0] = "<span style=\"color:red\">" + c.codiceRubrica + "</span>";
                    dr[1] = "<span style=\"color:red\">" + c.descrizione + "</span>";
                }
                else
                {
                    dr[0] = c.codiceRubrica;
                    dr[1] = c.descrizione;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private void setVisibilityPanelStar()
        {
            this.starCodAOO.Visible = false;
            this.starEmail.Visible = false;
            this.starCodAmm.Visible = false;

            switch (this.dd_canpref.SelectedItem.Text)
            {
                case "MAIL":
                    this.starEmail.Visible = true;
                    break;

                case "INTEROPERABILITA":
                    this.starEmail.Visible = true;
                    this.starCodAOO.Visible = true;
                    this.starCodAmm.Visible = true;
                    break;
            }
        }

        private void LoadCorrespondentTypes()
        {
            if (ddl_tipoCorr.Items.Count == 0)
            {
                this.ddl_tipoCorr.Items.Add(new ListItem("UO", "U"));
                this.ddl_tipoCorr.Items.Add(new ListItem("RUOLO", "R"));
                this.ddl_tipoCorr.Items.Add(new ListItem("PERSONA", "P"));
            }

            if (this.SenderDetail != null)
            {
                if (this.SenderDetail is DocsPaWR.UnitaOrganizzativa)
                    this.ddl_tipoCorr.SelectedIndex = 0;
                else if (this.SenderDetail is DocsPaWR.Utente)
                    this.ddl_tipoCorr.SelectedIndex = 2;
                else if (this.SenderDetail is DocsPaWR.RaggruppamentoFunzionale)
                {
                    this.ddl_tipoCorr.Items.Add(new ListItem("RAGGRUPPAMENTO FUNZIONALE", "F"));
                    this.ddl_tipoCorr.SelectedIndex = 3;
                }
                else
                    this.ddl_tipoCorr.SelectedIndex = 1;
            }
        }

        private void LoadPreferredChannels(bool showIs)
        {
            this.dd_canpref.Items.Add("");
            string idAmm = UserManager.GetInfoUser().idAmministrazione;
            DocsPaWR.MezzoSpedizione[] m_sped = AddressBookManager.GetAmmListaMezzoSpedizione(idAmm, false);
            if (m_sped != null && m_sped.Length > 0)
            {
                foreach (DocsPaWR.MezzoSpedizione m_spediz in m_sped)
                {
                    if (!showIs)
                    {
                        if (!m_spediz.chaTipoCanale.ToUpper().Equals("S"))
                        {
                            ListItem item = new ListItem(m_spediz.Descrizione, m_spediz.IDSystem);
                            this.dd_canpref.Items.Add(item);
                        }
                    }
                    else
                    {
                        ListItem item = new ListItem(m_spediz.Descrizione, m_spediz.IDSystem);
                        this.dd_canpref.Items.Add(item);
                    }
                }
            }
        }

        private void LoadTitles()
        {
            ddl_titolo.Items.Add("");

            string[] listaTitoli = AddressBookManager.GetListaTitoli();
            foreach (string tit in listaTitoli)
            {
                ddl_titolo.Items.Add(tit);
            }
        }

        

        private void RefreshScripts()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen_deselect", "$('.chzn-select-deselect').chosen({ allow_single_deselect: true, no_results_text: '" + utils.FormatJs(this.GetLabel("GenericChosenSelectNone")) + "' });", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen", "$('.chzn-select').chosen({ no_results_text: '" + utils.FormatJs(this.GetLabel("GenericChosenSelectNone")) + "' });", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        
        protected void DeleteProceed()
        {
            string msg = string.Empty;
            string message = string.Empty;
            DatiModificaCorr datiModifica = new DatiModificaCorr();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                if (!string.IsNullOrEmpty(this.proceed_delete.Value))
                {
                    this.proceed_delete.Value = "";
                    // prendo la system_id del corrispondente da eliminare
                    string idCorrGlobali = this.SenderDetail.systemId;

                    // popolo l'oggetto DatiModificaCorr necessario all'esecuzione della procedura 
                    datiModifica.idCorrGlobali = idCorrGlobali;

                    int flagListe = 0;
                    if (this.EnableDistributionLists)
                        flagListe = 1;

                    // operazione andata a buon fine
                    if (UserManager.DeleteModifyCorrispondenteEsterno(this, datiModifica, flagListe, "D", out message))
                    {
                        switch (message)
                        {
                            case "OK":
                                msg = "WarningAddressBookDeleteOk";
                                break;
                            default:
                                msg = "WarningAddressBookDeleteKo";
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'info', '');", true);
                                break;
                        }
                        
                        if (message == "OK")
                        {
                            listObj = (from corr in listObj where corr.systemId != idCorrGlobali select corr).ToList<Corrispondente>();
                            this.repListCorrespond.DataSource = listObj;
                            this.repListCorrespond.DataBind();
                            string systemIdCorrespondent = string.Empty;
                            (repListCorrespond.Items[0].FindControl("rd") as RadioButton).Checked = true;
                            systemIdCorrespondent = (repListCorrespond.Items[0].FindControl("systemIdCorrespondent") as HiddenField).Value;
                            (repListCorrespond.Items[0].FindControl("btnAddCorrespondent") as ImageButton).Enabled = true;
                            if (repListCorrespond.Items.Count > 1 && !systemIdCorrespondent.Equals(Parameter["systemIdCorrespondent"]))
                            {
                                (repListCorrespond.Items[0].FindControl("btnDeleteCorrespondent") as ImageButton).Enabled = true;
                            }
                            else
                                UpdateDocumentProfileButton = true;
                            SenderDetail = (from corr in listCorrespondentSameMail where corr.systemId.Equals(systemIdCorrespondent) select corr).FirstOrDefault();
                            drawDettagliCorr();
                            this.PlaceHolderAddCorrespondent.Visible = false;
                            this.UpdatePanelDetailsCorrespondentRole.Update();
                            this.UpdateRepListCorrespond.Update();
                            this.UpdatePanelDetails.Update();
                            this.UpdatePanelTypeCorr.Update();
                        }
                    }
                    else
                    {
                        switch (message)
                        {
                            case "ERROR":
                                msg = "WarningAddressBookDeleteKo";
                                break;
                            case "NOTOK":
                                DataSet dsListe = AddressBookManager.isCorrInListaDistr(idCorrGlobali);
                                msg = "WarningAddressBookDeleteKoLists";
                                if (dsListe != null)
                                {
                                    if (dsListe.Tables.Count > 0)
                                    {
                                        DataTable tab = dsListe.Tables[0];
                                        if (tab.Rows.Count > 0)
                                        {
                                            message = "Attenzione, utente presente nelle seguenti liste di distribuzione<br />";
                                            for (int i = 0; i < tab.Rows.Count; i++)
                                            {
                                                message += tab.Rows[i]["var_desc_corr"].ToString();
                                                if (!string.IsNullOrEmpty(tab.Rows[i]["prop"].ToString()))
                                                    message += " creata da " + tab.Rows[i]["prop"].ToString();
                                                else
                                                    if (!string.IsNullOrEmpty(tab.Rows[i]["ruolo"].ToString()))
                                                        message += " creata per il ruolo " + tab.Rows[i]["ruolo"].ToString();
                                                message += "<br />";
                                            }
                                        }
                                    }
                                }
                                break;
                            default:
                                msg = "WarningAddressBookDeleteKo";
                                break;
                        }
                        if (string.IsNullOrEmpty(message))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'info', '');", true);
                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'info', '', '" + utils.FormatJs(message) + "');", true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void DataChangedHandler(object sender, System.EventArgs e)
        {
            try {
                if (this.BtnSaveOccasionalAndRecord.Enabled)
                {
                    this.BtnSaveOccasionalAndRecord.Enabled = false;
                    NewCorrespondent = true;
                    this.BtnRecord.Enabled = true;
                    DisableAutoPostBack();
                    this.UpButtons.Update();
                }  
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void DisableAutoPostBack()
        {
            txt_CodR.AutoPostBack = false;
            txt_DescR.AutoPostBack = false;
            txt_CodRubrica.AutoPostBack = false;
            txt_descrizione.AutoPostBack = false;
            txt_codAOO.AutoPostBack = false;
            txt_codAmm.AutoPostBack = false;
            txt_email.AutoPostBack = false;
            ddl_titolo.AutoPostBack = false;
            txt_nome.AutoPostBack = false;
            txt_cognome.AutoPostBack = false;
            txt_luogoNascita.AutoPostBack = false;
            txt_dataNascita.AutoPostBack = false;
            txt_indirizzo.AutoPostBack = false;
            txt_citta.AutoPostBack = false;
            txt_provincia.AutoPostBack = false;
            txt_local.AutoPostBack = false;
            txt_nazione.AutoPostBack = false;
            txt_telefono.AutoPostBack = false;
            txt_telefono2.AutoPostBack = false;
            txt_fax.AutoPostBack = false;
            txt_cap.AutoPostBack = false;
            txt_codfisc.AutoPostBack = false;
            txt_partita_iva.AutoPostBack = false;
            txt_note.AutoPostBack = false;
        }

        private void DeleteProperty()
        {
            HttpContext.Current.Session.Remove("listCorrespondentSameMail");
            HttpContext.Current.Session.Remove("listObj");
            HttpContext.Current.Session.Remove("ActiveCodeDescriptionAdministrationSender");
            HttpContext.Current.Session.Remove("newCorrespondent");
            HttpContext.Current.Session.Remove("enableDistributionLists");
            HttpContext.Current.Session.Remove("senderDetail");
            HttpContext.Current.Session.Remove("Parameter");
            this.CaselleList = null;
            this.IsModified = false;
            this.IsModifiedChannel = false;
            this.IsModifiedCaselle = false;
            this.IsEmailReadOnly = false;
        }


        private bool verificaSelezione(ref string msg)
        {
            string corr_type = this.ddl_tipoCorr.SelectedValue;
            bool resultCheck = true;

            int indxMail = this.dd_canpref.Items.IndexOf(this.dd_canpref.Items.FindByText("MAIL"));
            int indxInterop = this.dd_canpref.Items.IndexOf(this.dd_canpref.Items.FindByText("INTEROPERABILITA"));

            if (!codice_rubrica_valido(this.txt_CodRubrica.Text))
            {
                msg = "WarningAddressBookCodeInvalid";
                resultCheck = false;
            }

            if ((corr_type == "U" && this.txt_descrizione.Text.Trim() == "") ||
                (corr_type == "R" && this.txt_descrizione.Text.Trim() == "") ||
                (corr_type == "P" && (this.txt_cognome.Text.Trim() == "" || this.txt_nome.Text.Trim() == "")) ||
                (this.dd_canpref.SelectedIndex == indxInterop && (this.txt_codAmm.Text.Equals(String.Empty) ||
                    this.txt_codAOO.Text.Equals(String.Empty))))
            {
                msg = "WarningAddressBookModifyObligatory";
                resultCheck = false;
            }

            //controlli mail
            if (updPanelMail.Visible)
            {
                string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
                //se attivo il multicasella
                if (gvCaselle.Rows.Count < 1 && (dd_canpref.SelectedIndex == indxMail || dd_canpref.SelectedIndex == indxInterop))
                {
                    //verifico che l'indirizzo non sia vuoto
                    if (string.IsNullOrEmpty(txtCasella.Text))
                    {
                        msg = "WarningAddressBookEmailNotInserted";
                        resultCheck = false;
                    }

                    //verifico il formato dell'indirizzo mail

                    if (!System.Text.RegularExpressions.Regex.Match(txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), pattern).Success)
                    {
                        msg = "WarningAddressBookEmailInvalid";
                        resultCheck = false;
                    }
                    if (resultCheck)
                    {
                        CaselleList.Add(new DocsPaWR.MailCorrispondente()
                        {
                            Email = txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                            Note = txtNote.Text,
                            Principale = "1"
                        });
                        txt_email.Text = txtCasella.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                    }
                }
                else
                {
                    foreach (GridViewRow row in gvCaselle.Rows)
                    {
                        //verifico che l'indirizzo non sia vuoto
                        if (string.IsNullOrEmpty((row.FindControl("txtEmailCorr") as System.Web.UI.WebControls.TextBox).Text))
                        {
                            msg = "WarningAddressBookEmailNotInserted";
                            resultCheck = false;
                            row.Cells[1].Focus();
                            break;
                        }
                        //verifico il formato dell'indirizzo mail
                        if (!System.Text.RegularExpressions.Regex.Match((row.FindControl("txtEmailCorr") as System.Web.UI.WebControls.TextBox).Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())
                            , pattern).Success)
                        {
                            msg = "WarningAddressBookEmailInvalid";
                            resultCheck = false;
                            row.Cells[1].Focus();
                            break;
                        }
                    }
                    if (resultCheck)
                    {
                        bool princ = false;
                        //scrivo in txt_email la casella di posta principale
                        foreach (DocsPaWR.MailCorrispondente c in CaselleList)
                        {
                            if (c.Principale.Equals("1"))
                            {
                                princ = true;
                                this.txt_email.Text = c.Email.Trim();
                                break;
                            }
                        }
                        if (!princ) // Nel caso vengano eliminate tutte le caselle per un corrispondente con canale no interoperante/no mail, allora svuoto anche txt_email
                            txt_email.Text = string.Empty;
                    }

                }
            }
            else
            {
                // no multicasella(corrispondenti non esterni
                if ((dd_canpref.SelectedIndex == indxMail && this.txt_email.Text.Equals(String.Empty))
                || (dd_canpref.SelectedIndex == indxInterop && this.txt_email.Text.Equals(String.Empty)))
                {
                    msg = "WarningAddressBookEmailNotInserted";
                    resultCheck = false;
                }
            }

            if (pnlStandard.Visible)//caso utenti e Uo
            {
                if ((this.txt_telefono == null || this.txt_telefono.Text.Equals(""))
                    && !(this.txt_telefono2 == null || this.txt_telefono2.Text.Equals("")))
                {
                    msg = "WarningAddressBookPhoneNotInserted";
                    resultCheck = false;
                }

                //verifica del corretto formato della data di nascita nel caso in cui non sia stata cancellata
                if (this.txt_dataNascita.Text != string.Empty && !utils.isDate(this.txt_dataNascita.Text))
                {
                    msg = "WarningAddressBookBirthdayInvalid";
                    resultCheck = false;
                }

                if (this.txt_cap != null && !this.txt_cap.Text.Equals("") && !utils.isNumeric(this.txt_cap.Text))
                {
                    msg = "WarningAddressBookZipcodeInvalid";
                    resultCheck = false;
                }

                if (this.txt_provincia != null && !this.txt_provincia.Text.Equals("") && !utils.isCorrectProv(this.txt_provincia.Text))
                {
                    msg = "WarningAddressBookDistrictInvalid";
                    resultCheck = false;
                }

                if (corr_type.Equals("P"))
                {
                    if (this.txt_codfisc != null && !this.txt_codfisc.Text.Equals("") && (utils.CheckTaxCode(this.txt_codfisc.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0))
                    {
                        msg = "WarningAddressBookTaxIdInvalid";
                        resultCheck = false;
                    }
                }
                else
                    if (corr_type.Equals("U"))
                    {
                        if ((this.txt_codfisc != null && !this.txt_codfisc.Text.Trim().Equals("")) && ((this.txt_codfisc.Text.Trim().Length == 11 && utils.CheckVatNumber(this.txt_codfisc.Text.Trim()) != 0) || (this.txt_codfisc.Text.Trim().Length == 16 && utils.CheckTaxCode(this.txt_codfisc.Text.Trim()) != 0) || (this.txt_codfisc.Text.Trim().Length != 11 && this.txt_codfisc.Text.Trim().Length != 16)))
                        {
                            msg = "WarningAddressBookTaxIdInvalid";
                            resultCheck = false;
                        }
                    }

                if (this.txt_partita_iva != null && !this.txt_partita_iva.Text.Equals("") && (utils.CheckVatNumber(this.txt_partita_iva.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray())) != 0))
                {
                    msg = "WarningAddressBookVatInvalid";
                    resultCheck = false;
                }

                if (!updPanelMail.Visible) // per il multicasella non ripeto i controlli sulla mail
                {
                    if (dd_canpref.SelectedIndex == indxMail || dd_canpref.SelectedIndex == indxInterop)
                    {
                        if (string.IsNullOrEmpty(this.txt_email.Text) || txt_email.Text.Trim().Equals(string.Empty) || !utils.IsValidEmail(this.txt_email.Text.Trim()))
                        {
                            msg = "WarningAddressBookEmailNotInserted";
                            resultCheck = false;
                        }
                    }
                }
            }

            return resultCheck;
        }

        /// <summary>
        /// reperimento dei dati per la modifica di una uo
        /// </summary>
        /// <param name="datiModifica"></param>
        private void modifyUO(ref DatiModificaCorr datiModifica)
        {
            datiModifica.idCorrGlobali = this.SenderDetail.systemId;
            datiModifica.descCorr = this.txt_descrizione.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codiceAoo = this.txt_codAOO.Text;
            datiModifica.codiceAmm = this.txt_codAmm.Text;
            datiModifica.email = this.txt_email.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codice = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codRubrica = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.nome = "";
            datiModifica.cognome = "";
            datiModifica.indirizzo = this.txt_indirizzo.Text;
            datiModifica.cap = this.txt_cap.Text;
            datiModifica.provincia = this.txt_provincia.Text;
            datiModifica.nazione = this.txt_nazione.Text;
            datiModifica.citta = this.txt_citta.Text;
            datiModifica.codFiscale = this.txt_codfisc.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.partitaIva = this.txt_partita_iva.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.telefono = this.txt_telefono.Text;
            datiModifica.telefono2 = this.txt_telefono2.Text;
            datiModifica.note = this.txt_note.Text;
            datiModifica.fax = this.txt_fax.Text;
            datiModifica.localita = this.txt_local.Text;
            datiModifica.idCanalePref = this.dd_canpref.SelectedItem.Value;
            datiModifica.luogoNascita = string.Empty;
            datiModifica.dataNascita = string.Empty;
            datiModifica.titolo = string.Empty;
            
            if (this.ddl_registri.SelectedValue != null)
                datiModifica.idRegistro = this.ddl_registri.SelectedValue.Split('_')[0];
            else
                datiModifica.idRegistro = string.Empty;
        }

        /// <summary>
        /// reperimento dei dati per la modifica di un ruolo
        /// </summary>
        /// <param name="datiModifica"></param>
        private void modifyRuolo(ref DatiModificaCorr datiModifica)
        {
            datiModifica.idCorrGlobali = this.SenderDetail.systemId;
            datiModifica.descCorr = this.txt_descrizione.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codiceAoo = this.txt_codAOO.Text;
            datiModifica.codiceAmm = this.txt_codAmm.Text;
            datiModifica.email = this.txt_email.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codice = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codRubrica = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.nome = String.Empty;
            datiModifica.cognome = String.Empty;
            datiModifica.indirizzo = String.Empty;
            datiModifica.cap = String.Empty;
            datiModifica.provincia = String.Empty;
            datiModifica.nazione = String.Empty;
            datiModifica.citta = String.Empty;
            datiModifica.codFiscale = String.Empty;
            datiModifica.partitaIva = String.Empty;
            datiModifica.telefono = String.Empty;
            datiModifica.telefono2 = String.Empty;
            datiModifica.note = String.Empty;
            datiModifica.fax = String.Empty;
            datiModifica.idCanalePref = dd_canpref.SelectedItem.Value;
            datiModifica.dataNascita = String.Empty;
            datiModifica.luogoNascita = String.Empty;
            datiModifica.titolo = String.Empty;

            if (this.ddl_registri.SelectedValue != null)
                datiModifica.idRegistro = this.ddl_registri.SelectedValue.Split('_')[0];
            else
                datiModifica.idRegistro = string.Empty;
        }

        /// <summary>
        /// reperimento dei dati per la modifica di un utente
        /// </summary>
        /// <param name="datiModifica"></param>
        private void modifyUtente(ref DatiModificaCorr datiModifica)
        {
            datiModifica.idCorrGlobali = this.SenderDetail.systemId;
            datiModifica.codiceAoo = this.txt_codAOO.Text;
            datiModifica.codiceAmm = this.txt_codAmm.Text;
            datiModifica.email = this.txt_email.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codice = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.codRubrica = this.txt_CodRubrica.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.nome = this.txt_nome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.cognome = this.txt_cognome.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.titolo = this.ddl_titolo.SelectedItem.Value;
            if (!string.IsNullOrEmpty(datiModifica.titolo))
                datiModifica.descCorr = datiModifica.titolo + " " + datiModifica.cognome + " " + datiModifica.nome;
            else
                datiModifica.descCorr = datiModifica.cognome + " " + datiModifica.nome;
            datiModifica.indirizzo = this.txt_indirizzo.Text;
            datiModifica.cap = this.txt_cap.Text;
            datiModifica.provincia = this.txt_provincia.Text;
            datiModifica.nazione = this.txt_nazione.Text;
            datiModifica.citta = this.txt_citta.Text;
            datiModifica.codFiscale = this.txt_codfisc.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.partitaIva = this.txt_partita_iva.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
            datiModifica.telefono = this.txt_telefono.Text;
            datiModifica.telefono2 = this.txt_telefono2.Text;
            datiModifica.note = this.txt_note.Text;
            datiModifica.fax = this.txt_fax.Text;
            datiModifica.localita = this.txt_local.Text;
            datiModifica.idCanalePref = dd_canpref.SelectedItem.Value;
            datiModifica.luogoNascita = this.txt_luogoNascita.Text;
            datiModifica.dataNascita = this.txt_dataNascita.Text;
            datiModifica.tipoCorrispondente = "P";

            if (this.ddl_registri.SelectedValue != null)
                datiModifica.idRegistro = this.ddl_registri.SelectedValue.Split('_')[0];
            else
                datiModifica.idRegistro = string.Empty;
        }

        protected bool InsertComboMailsCorr(string idCorrispondente, ref string msg)
        {
            bool res = true;

            if (!string.IsNullOrEmpty(idCorrispondente))
            {
                // modifico eventualmente la lista delle caselle associate al corrispondente esterno
                if (CaselleList.Count > 0)
                {
                    res = MultiCasellaManager.InsertMailCorrispondenteEsterno(CaselleList, idCorrispondente);
                    if (!res)
                    {
                        msg = "ErrorAddressBookMultiEmailModify";
                    }
                }
                if (CaselleList.Count == 0)
                {
                    res = MultiCasellaManager.DeleteMailCorrispondenteEsterno(idCorrispondente);
                    if (!res)
                    {
                        msg = "ErrorAddressBookMultiEmailModify";
                    }
                }
            }

            return res;
        }

        protected bool TypeMailCorrEsterno(string typeMail)
        {
            return (typeMail.Equals("1")) ? true : false;
        }

        protected void BindGridViewCaselle(Corrispondente SenderDetail)
        {
            if (CaselleList == null)
                CaselleList = MultiCasellaManager.GetMailCorrispondenteEsterno(SenderDetail.systemId);
            gvCaselle.DataSource = CaselleList;
            gvCaselle.DataBind();

            //se è disabilitato il multicasella, dopo l'immissione di una casella disabilito il pulsante aggiungi.
            if (this.CaselleList.Count > 0 && !MultiCasellaManager.IsEnabledMultiMail(RoleManager.GetRoleInSession().idAmministrazione))
            {
                txtCasella.Enabled = false;
                txtNote.Enabled = false;
                imgAggiungiCasella.Enabled = false;
            }
        }

        protected void EnableInsertMail()
        {
            if (this.IsEmailReadOnly)
            {
                this.txtCasella.Enabled = false;
                this.txtNote.Enabled = false;
                this.imgAggiungiCasella.Enabled = false;
            }
            else
            {
                if (this.SenderDetail != null && !this.SenderDetail.inRubricaComune)
                {
                    this.txtCasella.Enabled = true;
                    this.txtNote.Enabled = true;
                    this.imgAggiungiCasella.Enabled = true;
                }
            }
        }
        
    }
}