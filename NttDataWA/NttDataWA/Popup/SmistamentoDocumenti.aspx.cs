using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;
using System.Collections;
using NttDatalLibrary;
using System.Text;
using System.Globalization;
using System.Data;
using NttDataWA.UserControls;

namespace NttDataWA.Popup
{
    public partial class SmistamentoDocumenti : System.Web.UI.Page
    {

        protected System.Web.UI.HtmlControls.HtmlControl segn_repert;
        protected int heightUoApp;
        protected int heightUoInf;
        private string messError = "";
        private const string UP_PANEL_BUTTONS = "UpPnlButtons";
        private const string CLOSE_ZOOM = "closeZoom";

        #region Session

        private bool EnabledLibroFirma
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["EnabledLibroFirma"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["EnabledLibroFirma"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["EnabledLibroFirma"] = value;
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

        private DocsPaWR.Fascicolo Project
        {
            get
            {
                Fascicolo result = null;
                if (HttpContext.Current.Session["project"] != null)
                {
                    result = HttpContext.Current.Session["project"] as Fascicolo;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["project"] = value;
            }
        }

        private bool EnableBlockClassification
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableBlockClassification"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableBlockClassification"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableBlockClassification"] = value;
            }
        }

        /// <summary>
        /// valore di ritorno della popup del rifiuto trasmissione
        /// </summary>
        private string ReturnValueRejectTransm
        {
            get
            {
                if (HttpContext.Current.Session["RejectTransmissions"] != null)
                    return HttpContext.Current.Session["RejectTransmissions"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["RejectTransmissions"] = value;

            }
        }

        /// <summary>
        /// valore di ritorno della popup project
        /// </summary>
        private string ReturnValueProject
        {
            get
            {
                if (HttpContext.Current.Session["ReturnValuePopup"] != null)
                    return HttpContext.Current.Session["ReturnValuePopup"].ToString();
                else
                    return string.Empty;
            }
        }

        private SchedaDocumento DocumentInWorking
        {
            get
            {
                SchedaDocumento result = null;
                if (HttpContext.Current.Session["selectedRecord"] != null)
                {
                    result = HttpContext.Current.Session["selectedRecord"] as SchedaDocumento;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["selectedRecord"] = value;
            }
        }

        private SchedaDocumento DocumentInWorkingForSearchFolder
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
            set
            {
                HttpContext.Current.Session["document"] = value;
            }
        }

        private bool IsZoom
        {
            get
            {
                return (bool)HttpContext.Current.Session["isZoom"];
            }

            set
            {
                HttpContext.Current.Session["isZoom"] = value;
            }
        }
        private bool IsPreviousVersion
        {
            get
            {
                if (HttpContext.Current.Session["IsPreviousVersion"] != null) return (bool)HttpContext.Current.Session["IsPreviousVersion"];
                return false;
            }

            set
            {
                HttpContext.Current.Session["IsPreviousVersion"] = value;
            }
        }

        private int IndexRowGrdUoApp
        {
            get
            {
                if (HttpContext.Current.Session["indexRowGrdUoApp"] != null)
                    return (int)HttpContext.Current.Session["indexRowGrdUoApp"];
                else
                    return -1;
            }
            set
            {
                HttpContext.Current.Session["indexRowGrdUoApp"] = value;
            }
        }

        private string TypeURPGrdUoApp
        {
            get
            {
                if (HttpContext.Current.Session["typeURP"] != null)
                    return HttpContext.Current.Session["typeURP"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["typeURP"] = value;
            }
        }

        private string IdGrdUoApp
        {
            get
            {
                if (HttpContext.Current.Session["idGrdUoApp"] != null)
                    return HttpContext.Current.Session["idGrdUoApp"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["idGrdUoApp"] = value;
            }
        }

        private bool ClosePopupSmistamento
        {
            get
            {
                if (HttpContext.Current.Session["ClosePopupSmistamento"] != null)
                    return (bool)HttpContext.Current.Session["ClosePopupSmistamento"];
                else
                    return false;
            }

            set
            {
                HttpContext.Current.Session["ClosePopupSmistamento"] = value;
            }
        }
        #endregion

        #region Initialize

        protected void InitializePage()
        {
            RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
            this.Registry = RoleManager.GetRoleInSession().registri[0];
            this.InitializeLanguage();
            this.SetAjaxAddressBook();
            this.GestioneFascicolazione();
            //Settaggio flag Visualizza Documento
            this.SetFlagVisualizzaDocumento();

            if (System.Configuration.ConfigurationManager.AppSettings["SMISTA_VISUALIZZA_SELEZIONI"] != null && System.Configuration.ConfigurationManager.AppSettings["SMISTA_VISUALIZZA_SELEZIONI"].ToString().Equals("1"))
            {
                this.btn_selezioniSmistamento.Visible = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString()).Equals("1"))
            {
                this.EnabledLibroFirma = true;
            }
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_BLOCCA_CLASS.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_BLOCCA_CLASS.ToString()).Equals("1"))
            {
                this.EnableBlockClassification = true;
            }
        }

        protected void SetAjaxAddressBook()
        {
            string dataUser = RoleManager.GetRoleInSession().systemId;
            dataUser = dataUser + "-" + RoleManager.GetRoleInSession().registri[0].systemId;

            string callType = "CALLTYPE_CORR_INT_NO_UO"; // Destinatario su protocollo interno
            this.RapidSenderDescriptionProject.ContextKey = dataUser + "-" + UserManager.GetUserInSession().idAmministrazione + "-" + callType;
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSmistamento", "resizeSmistamento();", true);

            string maxLength = Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, DBKeys.FE_MAX_LENGTH_DESC_TRASM.ToString());
            maxLength = string.IsNullOrEmpty(maxLength) || Int64.Parse(maxLength) > 2000 ? "2000" : maxLength;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshprojectTxtNote", "charsLeft('txtNoteGen', '" + maxLength + "', '" + this.LtrNote.Text.Replace("'", "\'") + "');", true);
            this.txtNoteGen_chars.Attributes["rel"] = "txtNoteGen_'" + maxLength + "'_" + this.LtrNote.Text;
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnSmistamentoZoom.Text = Utils.Languages.GetLabelFromCode("BtnSmistamentoZoom", language);
            this.BtnSmistamentoDetSign.Text = Utils.Languages.GetLabelFromCode("BtnSmistamentoDetSign", language);
            this.BtnSmistamentoAdlU.Text = Utils.Languages.GetLabelFromCode("DocumentBtnAdL", language);
            this.BtnSmistamentoAdlR.Text = Utils.Languages.GetLabelFromCode("DocumentBtnAdLRole", language);
            this.BtnSmistamentoAccept.Text = Utils.Languages.GetLabelFromCode("BtnSmistamentoAccept", language);
            this.BtnSmistamentoAcceptLF.Text = Languages.GetLabelFromCode("ViewDetailNotifyBtnAcceptLF", language);
            this.BtnSmistamentoAcceptLF.ToolTip = Languages.GetLabelFromCode("ViewDetailNotifyBtnAcceptLFTooltip", language);
            this.BtnSmistamentoReject.Text = Utils.Languages.GetLabelFromCode("BtnSmistamentoReject", language);
            this.BtnSmistamentoSmista.Text = Utils.Languages.GetLabelFromCode("BtnSmistamentoSmista", language);
            this.BtnSmistamentoClose.Text = Utils.Languages.GetLabelFromCode("BtnSmistamentoClose", language);
            this.LitNoteIndividual.Text = Utils.Languages.GetLabelFromCode("LitNoteIndividual", language);
            this.LitNoteGeneral.Text = Utils.Languages.GetLabelFromCode("LitNoteGeneral", language);
            this.DocumentLitTransmRapid.Text = Utils.Languages.GetLabelFromCode("DocumentLitTransmRapid", language);
            this.DocumentLitClassificationRapid.Text = Utils.Languages.GetLabelFromCode("DocumentLitClassificationRapid", language);
            this.OpenTitolario.Title = Utils.Languages.GetLabelFromCode("TitleClassificationScheme", language);
            this.SearchProject.Title = Utils.Languages.GetLabelFromCode("SearchProjectTitle", language);

            this.ddl_trasm_rapida.Attributes["data-placeholder"] = Utils.Languages.GetLabelFromCode("TransmissionDdlTransmissionsModel", language);
            this.btn_first.AlternateText = Utils.Languages.GetLabelFromCode("btn_firstSmistaDocumenti", language);
            this.btn_first.ToolTip = Utils.Languages.GetLabelFromCode("btn_firstSmistaDocumenti", language);
            this.btn_previous.AlternateText = Utils.Languages.GetLabelFromCode("btn_previousSmistaDocumenti", language);
            this.btn_previous.ToolTip = Utils.Languages.GetLabelFromCode("btn_previousSmistaDocumenti", language);
            this.btn_next.AlternateText = Utils.Languages.GetLabelFromCode("btn_nextSmistaDocumenti", language);
            this.btn_next.ToolTip = Utils.Languages.GetLabelFromCode("btn_nextSmistaDocumenti", language);
            this.btn_last.AlternateText = Utils.Languages.GetLabelFromCode("btn_lastSmistaDocumenti", language);
            this.btn_last.ToolTip = Utils.Languages.GetLabelFromCode("btn_lastSmistaDocumenti", language);
            this.DigitalSignDetails.Title = Utils.Languages.GetLabelFromCode("DigitalSignDetailsTitle", language);
            this.ltlRepertorio.Text = Utils.Languages.GetLabelFromCode("DocumentLitRepertory", language);
            this.ltlTipologia.Text = Utils.Languages.GetLabelFromCode("DocumentLitTypeDocumentHead", language);
            this.ltl_dataCreazione.Text = Utils.Languages.GetLabelFromCode("SmistamentoDataCreazione", language);
            this.ltl_attachments.Text = Utils.Languages.GetLabelFromCode("SmistamentoLtlAttachments", language);
            this.ltl_versions.Text = Utils.Languages.GetLabelFromCode("SmistamentoLtlVersions", language);
            this.ltl_descRagTrasm.Text = Utils.Languages.GetLabelFromCode("SmistamentoLtlDescRagTrasm", language);
            this.ltl_mittente.Text = Utils.Languages.GetLabelFromCode("SmistamentoLtlMittente", language);
            this.ltl_destinatario.Text = Utils.Languages.GetLabelFromCode("SmistamentoLtlDestinatario", language);
            this.ltl_oggetto.Text = Utils.Languages.GetLabelFromCode("SmistamentoLtlOggetto", language);
            this.ltl_mitt_trasm.Text = Utils.Languages.GetLabelFromCode("SmistamentoLtlMittTrasm", language);
            this.chk_mantieniSel.Text = Utils.Languages.GetLabelFromCode("SmistamentoChkMantieniSel", language);
            this.chk_mantieniSel.ToolTip = Utils.Languages.GetLabelFromCode("SmistamentoChkMantieniSelToolTip", language);
            this.chk_showDoc.Text = Utils.Languages.GetLabelFromCode("SmistamentoChkShowDoc", language);
            this.chk_showDoc.ToolTip = Utils.Languages.GetLabelFromCode("SmistamentoChkShowDocToolTip", language);
            this.btn_clearFlag.ToolTip = Utils.Languages.GetLabelFromCode("SmistamentoBtnClearFlag", language);
            this.btnclassificationschema.ToolTip = Utils.Languages.GetLabelFromCode("SmistamentoBtnClassificationSchema", language);
            this.DocumentImgSearchProjects.ToolTip = Utils.Languages.GetLabelFromCode("SmistamentoDocumentImgSearchProjects", language);
            this.btn_selezioniSmistamento.ToolTip = Utils.Languages.GetLabelFromCode("SmistamentoBtnSelezioniSmistamento", language);

            this.LtrNote.Text = Utils.Languages.GetLabelFromCode("DocumentLitVisibleNotesChars", language) + " ";
            this.ImgAddProjects.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgAddProjects", language);
            this.ImgAddProjects.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgAddProjects", language);
            this.ViewNoteGen.Title = Utils.Languages.GetLabelFromCode("ViewNoteGen", language);
            this.ViewNoteInd.Title = Utils.Languages.GetLabelFromCode("ViewNoteInd", language);
            this.DocumentImgNoteGenDetail.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgNoteGenDetail", language);
            this.DocumentImgNoteIndDetail.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgNoteIndDetail", language);
        }

        #endregion

        /// <summary>
        /// Verifica se si posseggono i diritti sul documento
        /// </summary>
        /// <returns></returns>
        //private bool CheckACLDocument()
        //{
        //    string errorMessage = "";

        //    SmistaDocManager docManager = this.GetSmistaDocManager();
        //    DocsPaWR.DocumentoSmistamento docCorrente = docManager.GetCurrentDocument(false);
        //    DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentDetailsNoDataCheck(this, docCorrente.IDDocumento, docCorrente.DocNumber, out errorMessage);

        //    if (errorMessage != "")
        //    {
        //        //string msg = "ChainsAnswerNoRights";
        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + errorMessage.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + errorMessage.Replace("'", @"\'") + "', 'warning', '');}", true);
        //        return false;
        //    }
        //    else return true;
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            SmistaDocManager docManager = this.GetSmistaDocManager();

            if (!IsPostBack  && !ClosePopupSmistamento)
            {
                this.InitializePage();

                if (docManager.GetDocumentCount() == 0)
                {
                    //Nessun documento da smistare.
                    string msg = "WarningSmistaDocTrasmNotExists";
                    this.ClosePopupSmistamento = true;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "closeAJM", "parent.closeAjaxModal('SmistamentoDocumenti','up');", true);
                    return;
                }

                //se non è abilitata la visualizzazione della segnatura di repertorio, devo modificare la tabella per la visualizzazione
                if (!NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, "VIS_SEGNATURA_REPERTORI").Equals("1"))
                {
                    //rendo non visibile le colonne dedicate alla segnatura di repertorio ed alla tipologia documentale
                    //this.pnlSmistaSignRep.Visible = false;
                    this.upPnlInfoDocument.Update();
                }

                // verifica se esistono le ragioni di trasmissione utili allo smistamento
                DocsPaWR.RagioneTrasmissione[] listaRagioniSmistamento = null;

                if (this.CheckExistenzaRagioniSmistamento(out listaRagioniSmistamento))
                {
                    // Gestione navigazione UO (imposta campo nascosto)
                    this.ImpostaHiddenNavigaUO();

                    //settaggio campo invisibili per la gestione della nuova chiave di web.config
                    //UT_TX_RUOLI_CHECKED, checked decide se glu utenti di un ruolo devono comparire checked di default o no.
                    setCampoNascostoGestioneCheckUtenti();

                    //settaggio campi radioButton invisibili per la gestione della data di scadenza
                    setTipoRagioneSmistamento(listaRagioniSmistamento);

                    // Associazione dati UI
                    this.FillDataDocumentoTrasmesso();

                    // Visualizzione pannello pulsanti di navigazione
                    this.ShowPanelNavigationButtons();

                  

                }
                else
                {
                    if (!this.Page.ClientScript.IsStartupScriptRegistered("AlertModalDialog"))
                    {
                        string msg = "WarningSmistaDocReasonsTrasmNotExists";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "closeAJM", "parent.closeAjaxModal('SmistamentoDocumenti','up');", true);
                        return;
                    }
                }

                //Nodo titolario scelto
                DocsPaWR.FascicolazioneClassificazione classificazione = DocumentManager.getClassificazioneSelezionata(this);
                if (classificazione != null)
                {
                    this.TxtCodeProject.Text = classificazione.codice;
                    DocsPaWR.Fascicolo Fasc = getFascicolo();
                    ProjectManager.setFascicoloSelezionatoFascRapida(Fasc);
                    DocumentManager.setClassificazioneSelezionata(this, null);
                }

                this.VisibilityRoleFunctions();
            }
            else
            {
                this.ReadRetValueFromPopupTitolario();
                this.ReadRetValueFromPopupZoom();
                this.ReadRetValueFromSearch();
                this.ReadValueFromPopupSelection();
                this.ReadValueFromReject();
                this.ReadValueFromPopupViewNote();
            }

           
            this.UpPnlProject.Update();
            this.UpPnlProjectRapid.Update();
            this.upPnlInfoDocument.Update();

            //string language = UIManager.UserManager.GetUserLanguage();


            //abilito pannello per la trasmissione rapida
            this.UpPnlProjectRapid.Visible = false;
            if (cfg_Smista_Abilita_Trasm_Rapida())
            {
                this.UpPnlProjectRapid.Visible = true;
            }
            this.UpPnlProjectRapid.Update();

            //controllo abilitazione bottone per visualizzare le selezioni effettuate per lo smistamento
            if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_SELEZIONI_SMISTAMENTO.ToString())) || !Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_SELEZIONI_SMISTAMENTO.ToString()).Equals("1"))
            {
                this.btn_selezioniSmistamento.Visible = false;
            }
            this.LblClassRequired.Visible = false;
            this.ImgAddProjects.Visible = false;
            this.UpPnlProjectRapid.Update();
            if (docManager.GetDocumentCount() > 0)
            {
                DocsPaWR.DocumentoSmistamento docSmistamento = docManager.GetCurrentDocument(false);
                if (docManager.IsTrasmissioneConWorkflow(docManager.GetCurrentDocumentPosition() - 1))
                {
                    this.BtnSmistamentoReject.Enabled = true;

                    string TrasmId = docManager.GetIdTrasmissione(docManager.GetCurrentDocumentPosition() - 1);
                    DocsPaWR.Trasmissione trasmissione = TrasmManager.GetTransmission(this, TrasmId, "D");
                    string TrasmSingId = docManager.GetIdTrasmissioneSingola(docManager.GetCurrentDocumentPosition() - 1);
                    DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                    System.Collections.Generic.List<DocsPaWR.TrasmissioneSingola> list = new System.Collections.Generic.List<TrasmissioneSingola>(trasmissione.trasmissioniSingole);
                    List<DocsPaWR.TrasmissioneSingola> trasmSingoleUtente = list.Where(i => i.systemId == TrasmSingId).ToList();
                    TrasmissioneSingola trasmSing = trasmSingoleUtente.Where(i => i.systemId == TrasmSingId).FirstOrDefault();
                    if (this.EnabledLibroFirma && LibroFirmaManager.CanInsertInLibroFirma(trasmSing, trasmissione.infoDocumento.docNumber))
                    {
                        this.BtnSmistamentoAcceptLF.Visible = true;
                    }

                    this.BtnSmistamentoAccept.Text = Utils.Languages.GetLabelFromCode("SmistamentoBtnSmistamentoAccept", UserManager.GetUserLanguage());
                    this.BtnSmistamentoAccept.ToolTip = Utils.Languages.GetLabelFromCode("SmistamentoBtnSmistamentoAcceptTooltip", UserManager.GetUserLanguage());

                    if (trasmSing != null && trasmSing.ragione != null && trasmSing.ragione.fascicolazioneObbligatoria)
                    {
                        this.LblClassRequired.Visible = true;
                        this.ImgAddProjects.Visible = true;
                    }
                }
                else
                {
                    this.BtnSmistamentoReject.Enabled = false;
                    this.BtnSmistamentoAcceptLF.Visible = false;
                    this.BtnSmistamentoAccept.Text = Utils.Languages.GetLabelFromCode("SmistamentoBtnSmistamentoView", UserManager.GetUserLanguage());
                    this.BtnSmistamentoAccept.ToolTip = Utils.Languages.GetLabelFromCode("SmistamentoBtnSmistamentoViewTooltip", UserManager.GetUserLanguage());
                }

                try
                {
                    this.CheckDocWorkAreaAndSetLabelsButtons();

                }
                catch (Exception ex)
                {
                    string msg = "ErrorSmistaDocDetails";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                }
            }

            if (Session["NoteGenSmista"] != null)
            {
                this.txtNoteGen.Text = Session["NoteGenSmista"].ToString();
                Session.Remove("NoteGenSmista");
            }


            if (Session["MessError"] != null)
            {
                messError = Session["MessError"].ToString();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + messError.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + messError.Replace("'", @"\'") + "', 'error', '');}", true);
                Session.Remove("MessError");
            }

            this.RefreshScript();

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
        }

        private void ReadValueFromPopupViewNote()
        {
            if(!string.IsNullOrEmpty(this.ViewNoteGen.ReturnValue))
            {
                this.txtNoteGen.Text = this.TxtNoteViewer;
                this.txtNoteGen.ToolTip = this.TxtNoteViewer;
                this.upPnlNote.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ViewNoteGen','');", true);
            }
        }

        private void ReadValueFromReject()
        {
            if (!string.IsNullOrEmpty(this.RejectTransmissions.ReturnValue))
            {
                try
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('RejectTransmissions','');", true);

                    SmistaDocManager docManager = this.GetSmistaDocManager();

                    // Reperimento dello stato della trasmissione utente
                    // per controllo ulteriore allo scopo di impedire il rifiuto di un documento già accettato / rifiutato
                    DocsPaWR.StatoTrasmissioneUtente statoTrasmissione = docManager.GetStatoTrasmissioneCorrente();

                    this.accettaRifiuta(TrasmissioneTipoRisposta.RIFIUTO);

                }
                catch
                {
                    string msg = "ErrorRejectDocument";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                }
                // Gestione del mantenimento dei flag selezionati e dei tasti di default
                //this.ChkMantieniSelezione();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('RejectTransmissions','');", true);
            }
        }

        private void ReadValueFromPopupSelection()
        {
            if (!string.IsNullOrEmpty(this.SmistaDocSelectionsDetails.ReturnValue))
            {
                this.gestisciPopUpSelezioneSmistamento();
            }
        }

        private void VisibilityRoleFunctions()
        {
            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_ADD_ADL"))
            {
                this.BtnSmistamentoAdlU.Visible = false;
                this.BtnSmistamentoAdlR.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_ADL_ROLE"))
            {
                this.BtnSmistamentoAdlR.Visible = false;
            }
        }


        private void ReadRetValueFromSearch()
        {
            //Laura 13 Marzo
            if (!string.IsNullOrEmpty(this.SearchProject.ReturnValue))
            {
                //if (!String.IsNullOrEmpty(DocumentInWorking.systemId))
                //{

                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SearchProject','');", true);

                //    //TxtCodeProject.Text = string.Empty;
                //    //TxtDescriptionProject.Text = string.Empty;
                //    ////creazioneDataTableFascicoli();
                //    ////UpNFascicoli.Update();
                //    ////UpGrid.Update();
                //    //UpPnlProject.Update();                        
                //    //}
                //    //else {
                //    //string msg = "ResultFascicolazione";

                //    //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg + "', 'info', '', '" + HttpContext.Current.Session["ReturnValuePopup"].ToString() + "');} else {parent.ajaxDialogModal('" + msg + "', 'info', '', '" + HttpContext.Current.Session["ReturnValuePopup"].ToString() + "');}", true);
                //    //}
                //}

                if (this.ReturnValue.Split('#').Length > 1)
                {
                    this.TxtCodeProject.Text = this.ReturnValue.Split('#').First();
                    this.TxtDescriptionProject.Text = this.ReturnValue.Split('#').Last();
                    this.UpPnlProject.Update();
                    TxtCodeProject_OnTextChanged(new object(), new EventArgs());
                }
                else
                    //Laura 19 Marzo
                    if (this.ReturnValue.Contains("//"))
                    {
                        this.TxtCodeProject.Text = this.ReturnValue;
                        this.TxtDescriptionProject.Text = "";
                        this.UpPnlProject.Update();
                        TxtCodeProject_OnTextChanged(new object(), new EventArgs());
                    }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SearchProject','');", true);
            }
        }

        private void ReadRetValueFromPopupTitolario()
        {
            if (!string.IsNullOrEmpty(this.OpenTitolario.ReturnValue))
            {
                if (this.ReturnValue.Split('#').Length > 1)
                {
                    this.TxtCodeProject.Text = this.ReturnValue.Split('#').First();
                    this.TxtDescriptionProject.Text = this.ReturnValue.Split('#').Last();
                    this.UpPnlProject.Update();
                    TxtCodeProject_OnTextChanged(new object(), new EventArgs());
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenTitolario','')", true);
            }

        }

        #region Fascicolazione rapida

        private void GestioneFascicolazione()
        {
            this.UpPnlProjectRapid.Visible = false;

            if (this.IsEnabledFascicolazione())
            {
                this.UpPnlProjectRapid.Visible = true;
                this.GestVisibilitaFrecceNavigaUO();
            }
            this.UpPnlProject.Update();
            this.UpPnlProjectRapid.Update();
        }

        private void ResetFascicolazione()
        {
            ProjectManager.removeFascicoloSelezionatoFascRapida(this);
            this.TxtCodeProject.Text = "";
            this.TxtDescriptionProject.Text = "";
            this.UpPnlProject.Update();
        }


        /// <summary>
        /// Rimozione di tutte le selezioni effettuate nelle UO visualizzate
        /// </summary>
        private void ClearSelections()
        {
            DocsPaWR.UOSmistamento uoAppartenenza = this.GetSmistaDocManager().GetUOAppartenenza();

            this.ClearSelectionsUO(uoAppartenenza);
        }

        private void ClearSelectionsUO(DocsPaWR.UOSmistamento uo)
        {
            uo.FlagCompetenza = false;
            uo.FlagConoscenza = false;

            if (uo.Ruoli != null)
                foreach (DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
                    this.ClearSelectionsRuoli(ruolo);

            if (uo.UoInferiori != null)
                foreach (DocsPaWR.UOSmistamento uoInf in uo.UoInferiori)
                    this.ClearSelectionsUO(uoInf);
        }

        private void ClearSelectionsRuoli(DocsPaWR.RuoloSmistamento ruolo)
        {
            ruolo.FlagCompetenza = false;
            ruolo.FlagConoscenza = false;
            ruolo.datiAggiuntiviSmistamento = new DocsPaWR.datiAggiuntiviSmistamento();

            if (ruolo.Utenti != null)
                foreach (DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
                    this.ClearSelectionsUtenti(utente);
        }

        private void ClearSelectionsUtenti(DocsPaWR.UtenteSmistamento utente)
        {
            utente.FlagCompetenza = false;
            utente.FlagConoscenza = false;
            utente.datiAggiuntiviSmistamento = new DocsPaWR.datiAggiuntiviSmistamento();
        }
        #endregion

        #region pulsanti navigazione
        protected void btn_first_Click(object sender, ImageClickEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            SmistaDocManager docManager = this.GetSmistaDocManager();

            if (docManager.MoveFirstDocument())
            {
                if (!this.chk_mantieniSel.Checked)
                {
                    // In navigazione tra documenti, 
                    // rimozione delle selezioni effettuate dall'utente
                    // qualora non sia attivo il flag mantieni selezione
                    this.ClearSelections();
                }

                //Remove note correnti
                if (Session["datiAggiuntivi"] != null)
                {

                    DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = (DocsPaWR.datiAggiuntiviSmistamento[])Session["datiAggiuntivi"];

                    foreach (DocsPaWR.datiAggiuntiviSmistamento appoggioDatiAggiuntivi in datiAggiuntivi)
                    {
                        appoggioDatiAggiuntivi.NoteIndividuali = "";
                        appoggioDatiAggiuntivi.dtaScadenza = "";
                        appoggioDatiAggiuntivi.tipoTrasm = "";
                    }
                }

                DocumentManager.setSelectedNumberVersion("0");
                DocumentManager.RemoveSelectedAttachId();

                // Gestione del mantenimento dei flag selezionati e dei tasti di default
                this.ChkMantieniSelezione();

                this.FillDataDocumentoTrasmesso();
                this.SetFlagDestinatarioRaggiutoDaTrasm(this.grdUOApp, this.hdUOapp.Value);
                this.upPnlInfoDocument.Update();
                this.UpPnlViewer.Update();

                this.refreshDataButton();
            }
        }

        protected void btn_previous_Click(object sender, ImageClickEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            SmistaDocManager docManager = this.GetSmistaDocManager();

            if (docManager.MovePreviousDocument())
            {
                if (!this.chk_mantieniSel.Checked)
                {
                    // In navigazione tra documenti, 
                    // rimozione delle selezioni effettuate dall'utente
                    // qualora non sia attivo il flag mantieni selezione
                    this.ClearSelections();
                }

                //Remove note correnti
                if (Session["datiAggiuntivi"] != null)
                {

                    DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = (DocsPaWR.datiAggiuntiviSmistamento[])Session["datiAggiuntivi"];

                    foreach (DocsPaWR.datiAggiuntiviSmistamento appoggioDatiAggiuntivi in datiAggiuntivi)
                    {
                        appoggioDatiAggiuntivi.NoteIndividuali = "";
                        appoggioDatiAggiuntivi.dtaScadenza = "";
                        appoggioDatiAggiuntivi.tipoTrasm = "";
                    }
                }

                DocumentManager.setSelectedNumberVersion("0");
                DocumentManager.RemoveSelectedAttachId();

                // Gestione del mantenimento dei flag selezionati e dei tasti di default
                this.ChkMantieniSelezione();

                this.FillDataDocumentoTrasmesso();
                this.SetFlagDestinatarioRaggiutoDaTrasm(this.grdUOApp, this.hdUOapp.Value);
                this.upPnlInfoDocument.Update();
                this.UpPnlViewer.Update();

                this.refreshDataButton();
            }
        }

        private void refreshDataButton()
        {
            SmistaDocManager docManager = this.GetSmistaDocManager();

            if (docManager.GetDocumentCount() > 0)
            {
                DocsPaWR.DocumentoSmistamento docSmistamento = docManager.GetCurrentDocument(false);
                this.LblClassRequired.Visible = false;
                this.ImgAddProjects.Visible = false;
                if (docManager.IsTrasmissioneConWorkflow(docManager.GetCurrentDocumentPosition() - 1))
                {
                    this.BtnSmistamentoReject.Enabled = true;

                    string TrasmId = docManager.GetIdTrasmissione(docManager.GetCurrentDocumentPosition() - 1);
                    DocsPaWR.Trasmissione trasmissione = TrasmManager.GetTransmission(this, TrasmId, "D");
                    string TrasmSingId = docManager.GetIdTrasmissioneSingola(docManager.GetCurrentDocumentPosition() - 1);
                    DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                    System.Collections.Generic.List<DocsPaWR.TrasmissioneSingola> list = new System.Collections.Generic.List<TrasmissioneSingola>(trasmissione.trasmissioniSingole);
                    List<DocsPaWR.TrasmissioneSingola> trasmSingoleUtente = list.Where(i => i.systemId == TrasmSingId).ToList();
                    TrasmissioneSingola trasmSing = trasmSingoleUtente.Where(i => i.systemId == TrasmSingId).FirstOrDefault();
                    if (this.EnabledLibroFirma && LibroFirmaManager.CanInsertInLibroFirma(trasmSing, trasmissione.infoDocumento.docNumber))
                    {
                        this.BtnSmistamentoAcceptLF.Visible = true;
                    }

                    this.BtnSmistamentoAccept.Text = "Accetta";
                    this.BtnSmistamentoAccept.ToolTip = "Accetta la trasmissione, imposta il documento come VISTO e lo toglie dalla lista delle COSE DA FARE";

                    if (trasmSing != null && trasmSing.ragione != null && trasmSing.ragione.fascicolazioneObbligatoria)
                    {
                        this.LblClassRequired.Visible = true;
                        this.ImgAddProjects.Visible = true;
                    }
                }
                else
                {
                    this.BtnSmistamentoReject.Enabled = false;
                    this.BtnSmistamentoAcceptLF.Visible = false;
                    this.BtnSmistamentoAccept.Text = "Visto";
                    this.BtnSmistamentoAccept.ToolTip = "Imposta il documento come VISTO e lo toglie dalla lista delle COSE DA FARE";
                }
            }

            this.UpPnlButtons.Update();
        }

        protected void btn_next_Click(object sender, ImageClickEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            SmistaDocManager docManager = this.GetSmistaDocManager();

            if (docManager.MoveNextDocument())
            {
                if (!this.chk_mantieniSel.Checked)
                {
                    // In navigazione tra documenti, 
                    // rimozione delle selezioni effettuate dall'utente
                    // qualora non sia attivo il flag mantieni selezione
                    this.ClearSelections();
                }

                //Remove note correnti
                if (Session["datiAggiuntivi"] != null)
                {

                    DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = (DocsPaWR.datiAggiuntiviSmistamento[])Session["datiAggiuntivi"];

                    foreach (DocsPaWR.datiAggiuntiviSmistamento appoggioDatiAggiuntivi in datiAggiuntivi)
                    {
                        appoggioDatiAggiuntivi.NoteIndividuali = "";
                        appoggioDatiAggiuntivi.dtaScadenza = "";
                        appoggioDatiAggiuntivi.tipoTrasm = "";
                    }
                }

                DocumentManager.setSelectedNumberVersion("0");
                DocumentManager.RemoveSelectedAttachId();

                // Gestione del mantenimento dei flag selezionati e dei tasti di default
                this.ChkMantieniSelezione();

                this.FillDataDocumentoTrasmesso();
                this.SetFlagDestinatarioRaggiutoDaTrasm(this.grdUOApp, this.hdUOapp.Value);
                this.upPnlInfoDocument.Update();
                this.updatePanelUOAppartenenza.Update();
                this.updatePanelUOInf.Update();
                this.UpPnlViewer.Update();

                this.refreshDataButton();
            }
        }

        protected void btn_last_Click(object sender, ImageClickEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            SmistaDocManager docManager = this.GetSmistaDocManager();

            if (docManager.MoveLastDocument())
            {
                if (!this.chk_mantieniSel.Checked)
                {
                    // In navigazione tra documenti, 
                    // rimozione delle selezioni effettuate dall'utente
                    // qualora non sia attivo il flag mantieni selezione
                    this.ClearSelections();
                }

                //Remove note correnti
                if (Session["datiAggiuntivi"] != null)
                {

                    DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = (DocsPaWR.datiAggiuntiviSmistamento[])Session["datiAggiuntivi"];

                    foreach (DocsPaWR.datiAggiuntiviSmistamento appoggioDatiAggiuntivi in datiAggiuntivi)
                    {
                        appoggioDatiAggiuntivi.NoteIndividuali = "";
                        appoggioDatiAggiuntivi.dtaScadenza = "";
                        appoggioDatiAggiuntivi.tipoTrasm = "";
                    }
                }

                DocumentManager.setSelectedNumberVersion("0");
                DocumentManager.RemoveSelectedAttachId();

                // Gestione del mantenimento dei flag selezionati e dei tasti di default
                this.ChkMantieniSelezione();

                this.FillDataDocumentoTrasmesso();
                this.SetFlagDestinatarioRaggiutoDaTrasm(this.grdUOApp, this.hdUOapp.Value);
                this.upPnlInfoDocument.Update();
                this.UpPnlViewer.Update();

                this.refreshDataButton();
            }
        }

        #endregion

        private void SetFlagVisualizzaDocumento()
        {
            string valoreChiave;
            valoreChiave = Utils.InitConfigurationKeys.GetValue("0", "FE_VISUAL_DOC_SMISTAMENTO");
            if (valoreChiave.Equals("1"))
            {
                this.chk_showDoc.Checked = true;
                //this.chk_showDoc_CheckedChanged(this.chk_showDoc, new EventArgs());
            }
        }

        /// <summary>
        /// Gestione visualizzione pannello pulsanti di navigazione
        /// </summary>
        private void ShowPanelNavigationButtons()
        {
            string docNumber = Request.QueryString["DOC_NUMBER"];
            pnl_navigationButtons.Visible = (docNumber == null || docNumber.Equals(string.Empty));
        }

        private void FillDataDocumentoTrasmesso()
        {
            SmistaDocManager docManager = this.GetSmistaDocManager();

            if (docManager.GetDocumentCount() > 0)
            {
                DocsPaWR.DocumentoSmistamento docSmistamento = docManager.GetCurrentDocument(false);

                if (this.DocumentInWorking == null || this.DocumentInWorking.systemId != docSmistamento.IDDocumento)
                {
                    // DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDettaglioDocumentoNoDataVista(this, docSmistamento.IDDocumento, docSmistamento.DocNumber);
                    DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentListVersions(this, docSmistamento.IDDocumento, docSmistamento.DocNumber);
                    DocumentManager.SetDataVistaSP(schedaDocumento.systemId);
                    UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                    this.DocumentInWorking = schedaDocumento;
                    FileManager.setSelectedFile(schedaDocumento.documenti[0]);
                    DocumentManager.setSelectedRecord(schedaDocumento);
                }

                //se abilitata la segnatura di repertorio, inizializzo il il campo
                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.VIS_SEGNATURA_REPERTORI.ToString())) && Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.VIS_SEGNATURA_REPERTORI.ToString()).Equals("1"))
                {

                    if (!string.IsNullOrEmpty(docSmistamento.DocNumber))
                    {
                        this.lbl_segn_repertorio.Text = DocumentManager.getSegnaturaRepertorio(docSmistamento.DocNumber, AdministrationManager.AmmGetInfoAmmCorrente(UserManager.GetInfoUser().idAmministrazione).Codice);

                        if (!string.IsNullOrEmpty(this.lbl_segn_repertorio.Text))
                        {
                            this.PlcType.Visible = true;
                            // Recupero dell'eventuale tipologia associata al documento / fascicolo
                            this.lblTipology.Text = docSmistamento.TipologyDescription;
                        }
                        else
                        {
                            this.PlcType.Visible = false;
                        }
                        
                    }
                }

                lbl_dataCreazione.Text = docSmistamento.DataCreazione;

                if (docSmistamento.Segnatura == null || docSmistamento.Segnatura.Equals(string.Empty))
                {
                    this.lbl_segnatura.CssClass = "weight";
                    lbl_segnatura.Text = docSmistamento.IDDocumento;
                }
                else
                {
                    lbl_segnatura.Text = docSmistamento.Segnatura;
                    this.lbl_segnatura.CssClass = "redWeight";
                }

                if (docSmistamento.Oggetto.Length > 60)
                {
                    lbl_oggetto.Text = docSmistamento.Oggetto.Substring(0, 60) + "...";
                    lbl_oggetto.ToolTip = docSmistamento.Oggetto;
                }
                else
                {
                    lbl_oggetto.Text = docSmistamento.Oggetto;
                    lbl_oggetto.ToolTip = string.Empty;
                }

                if (docSmistamento.MittenteDocumento != null && docSmistamento.MittenteDocumento != string.Empty && docSmistamento.MittenteDocumento != "")
                {
                    this.PlcSender.Visible = true;
                    lbl_mittente.Text = docSmistamento.MittenteDocumento;
                    lbl_mittente.ToolTip = docSmistamento.MittenteDocumento;
                }
                else
                {
                    this.PlcSender.Visible = false;
                    lbl_mittente.Text = string.Empty;
                }

                int docIndex = docManager.GetCurrentDocumentPosition();
                string mittTrasm = docManager.getMittenteTrasmissione(docIndex - 1);
                this.lbl_mitt_trasm.Text = mittTrasm;
                this.lbl_mitt_trasm.ToolTip = mittTrasm;

                string listaDest = this.GetDestinatari(docSmistamento);
                if (!string.IsNullOrEmpty(listaDest))
                {
                    this.PlcRecipient.Visible = true;
                    if (listaDest.Length > 60)
                    {
                        this.lbl_destinatario.Text = listaDest.Substring(0, 60) + "...";
                        this.lbl_destinatario.ToolTip = listaDest;
                    }
                    else
                        this.lbl_destinatario.Text = listaDest;
                }
                else
                {
                    this.PlcRecipient.Visible = false;
                    this.lbl_destinatario.Text = string.Empty;
                }

                this.lbl_versioni.Text = docSmistamento.Versioni;
                this.lbl_allegati.Text = docSmistamento.Allegati;
                this.lbl_descRagTrasm.Text = docManager.GetDescRagioneTrasm(docManager.GetCurrentDocumentPosition() - 1);

                docSmistamento = null;

                int config = 0;
                if (cfg_Smista_Abilita_Trasm_Rapida())
                    config = 1;

                if (config == 1)
                {
                    bool mantieniSelezione = this.chk_mantieniSel.Checked;

                    string idModello = string.Empty;

                    if (mantieniSelezione)
                        idModello = this.ddl_trasm_rapida.SelectedValue;

                    // Caricamento modelli trasmissione rapida per il documento
                    this.caricaModelliTrasmRapida();

                    if (mantieniSelezione)
                        // Ripristino modello trasmissione rapida (se presente)
                        this.ddl_trasm_rapida.SelectedValue = idModello;
                }


                // Rimozione contenuto documento firmato dalla sessione
                this.SetSignedDocumentOnSession(null);

                // Caricamento griglie destinatari
                this.FillGridDestinatari();

                // Aggiornamento indice documento corrente
                this.RefreshDocumentCounter();

                // Gestione abilitazione / disabilitazione pulsanti di navigazione
                this.EnabledNavigationButtons();

                // Gestione abilitazione / disabilitazione pulsante dettagli firma
                this.EnableButtonDettagliFirma();


                //Viene autopopolato il campo relativo alle note generali
                this.txtNoteGen.Text = docManager.GetNoteGenerali(docManager.GetCurrentDocumentPosition() - 1);
                this.txtNoteGen.ToolTip = this.txtNoteGen.Text;

                //Viene autopopolato il campo relativo alle note individuali e reso non editabile
                this.txtAreaNoteInd.Text = docManager.GetNoteIndividuali(docManager.GetCurrentDocumentPosition() - 1);
                this.txtAreaNoteInd.ToolTip = this.txtAreaNoteInd.Text;
                this.upPnlNote.Update();
                
                // Visualizzazione file
                this.ShowDocumentFile(this.chk_showDoc.Checked);

                // pulsanti di default
                this.EnableButtonsDefault();

                // pulsante rifiuta
                this.EnableButtonRifiuta();

                //pulsante accetta e inserisci in libro firma
                this.EnableButtonAccettaLF();

                // imposta il text del bottone ADLU
                this.CheckDocWorkAreaAndSetLabelsButtons();
            }
            else
            {
                //Nessun documento da smistare.
                string msg = "WarningSmistaDocTrasmNotExists";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "closeAJM", "parent.closeAjaxModal('SmistamentoDocumenti','up');", true);

            }


        }

        public void RefreshZoom()
        {
            this.chk_showDoc.Checked = true;
            this.BtnSmistamentoZoom.Enabled = true;
            this.BtnSmistamentoZoom.Enabled = true;
            this.UpPnlCheckDoc.Update();


            this.UpPnlButtons.Update();
        }

        public void RefreshZoom2()
        {

            this.chk_showDoc.Checked = false;

            this.BtnSmistamentoZoom.Enabled = false;
            this.UpPnlCheckDoc.Update();
            this.UpPnlButtons.Update();
        }

        /// <summary>
        /// Formattazione dei destinatari del documento
        /// </summary>
        /// <param name="docSmistamento"></param>
        /// <returns></returns>
        private string GetDestinatari(DocsPaWR.DocumentoSmistamento docSmistamento)
        {
            string retValue = string.Empty;

            foreach (string dest in docSmistamento.DestinatariDocumento)
            {
                if (retValue != string.Empty)
                    retValue += "; ";

                retValue += dest;
            }

            return retValue;
        }

        /// <summary>
        /// Gestione del tasto di rifiuto
        /// </summary>
        private void EnableButtonRifiuta()
        {
            SmistaDocManager docManager = this.GetSmistaDocManager();

            if (docManager.IsTrasmissioneConWorkflow(docManager.GetCurrentDocumentPosition() - 1))
            {
                // La trasmissione prevede workflow
                this.BtnSmistamentoReject.Enabled = true;
                //this.RegisterClientScriptBtnRifiuta(); //Lnr 10/06/2013
            }
            else
            {
                this.BtnSmistamentoReject.Enabled = false;
            }
        }

        private void EnableButtonAccettaLF()
        {

            SmistaDocManager docManager = this.GetSmistaDocManager();
            if (docManager.IsTrasmissioneConWorkflow(docManager.GetCurrentDocumentPosition() - 1))
            {
                string TrasmId = docManager.GetIdTrasmissione(docManager.GetCurrentDocumentPosition() - 1);
                DocsPaWR.Trasmissione trasmissione = TrasmManager.GetTransmission(this, TrasmId, "D");
                string TrasmSingId = docManager.GetIdTrasmissioneSingola(docManager.GetCurrentDocumentPosition() - 1);
                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                System.Collections.Generic.List<DocsPaWR.TrasmissioneSingola> list = new System.Collections.Generic.List<TrasmissioneSingola>(trasmissione.trasmissioniSingole);
                List<DocsPaWR.TrasmissioneSingola> trasmSingoleUtente = list.Where(i => i.systemId == TrasmSingId).ToList();
                TrasmissioneSingola trasmSing = trasmSingoleUtente.Where(i => i.systemId == TrasmSingId).FirstOrDefault();
                if (this.EnabledLibroFirma && LibroFirmaManager.CanInsertInLibroFirma(trasmSing, trasmissione.infoDocumento.docNumber))
                {
                    this.BtnSmistamentoAcceptLF.Visible = true;
                }
                else
                {
                    this.BtnSmistamentoAcceptLF.Visible = false;
                }
            }
            else
            {
                this.BtnSmistamentoAcceptLF.Visible = false;
            }
        }

        private void CheckDocWorkAreaAndSetLabelsButtons()
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                SmistaDocManager docManager = this.GetSmistaDocManager();
                DocsPaWR.DocumentoSmistamento docSmistamento = docManager.GetCurrentDocument(false);

                int isInAdl = DocumentManager.isDocInADL(docSmistamento.IDDocumento, this);
                if (isInAdl == 1)
                {
                    this.BtnSmistamentoAdlU.Text = Utils.Languages.GetLabelFromCode("DocumentBtnAdLRemove", language);
                }
                else
                {
                    this.BtnSmistamentoAdlU.Text = Utils.Languages.GetLabelFromCode("SmistaDocumentBtnAdL", language);
                }
                int isDocInADLRole = DocumentManager.isDocInADLRole(docSmistamento.IDDocumento, this);
                if (isDocInADLRole == 1)
                {
                    this.BtnSmistamentoAdlR.Text = Utils.Languages.GetLabelFromCode("DocumentBtnAdLRoleRemove", language);
                    this.BtnSmistamentoAdlU.Enabled = false;
                }
                else
                {
                    this.BtnSmistamentoAdlR.Text = Utils.Languages.GetLabelFromCode("DocumentBtnAdLRole", language);
                }
                this.UpPnlButtons.Update();
            }
            catch (Exception ex)
            {
                string msg = "ErrorSmistaDocDetails";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
            }
        }

        /// <summary>
        /// Gestione script client per il pulsante di rifiuta
        /// </summary>
        //private bool IsTrasmAcceptedOrRejected()
        //{
        //    SmistaDocManager docManager = this.GetSmistaDocManager();
        //    bool result = true;
        //    // Reperimento dello stato della trasmissione corrente
        //    // per impedire di rifiutare il documento qualora sia già stato accettato / rifiutato
        //    DocsPaWR.StatoTrasmissioneUtente statoTrasmissione = docManager.GetStatoTrasmissioneCorrente();

        //    if (statoTrasmissione.Accettata || statoTrasmissione.Rifiutata)
        //    {
        //        string msg = "";
        //        if (statoTrasmissione.Rifiutata)
        //            msg = "WarningSmistaDocRejectTrasm";
        //        else
        //            msg = "WarningSmistaDocAcceptTrasm";

        //        result = false;
        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
        //    }
        //    return result;
        //}

        /// <summary>
        /// Abilita i tasti di default (Smista e Visto)
        /// </summary>
        private void EnableButtonsDefault()
        {
            this.BtnSmistamentoSmista.Enabled = true;
            this.BtnSmistamentoAdlR.Enabled = true;
            this.BtnSmistamentoAdlU.Enabled = true;
        }

        /// <summary>
        /// // Caricamento griglie destinatari
        /// </summary>
        private void FillGridDestinatari()
        {

            if (!IsPostBack)
            {
                SmistaDocManager docManager = this.GetSmistaDocManager();

                DocsPaWR.UOSmistamento uoApp = docManager.GetUOAppartenenza();

                if (uoApp != null && this.hdUOapp.Value.Equals(""))
                    this.hdUOapp.Value = uoApp.ID;

                this.indxUoSel = this.hdUOapp.Value;

                // Caricamento griglia uo di appartenenza
                if (uoApp != null && uoApp.Ruoli.Length > 0)
                {
                    this.FillDataGridUOApp(uoApp);
                }

                // Caricamento griglia uo inferiori
                DocsPaWR.UOSmistamento[] uoInferiori = docManager.GetUOInferiori();

                if (uoInferiori != null && uoInferiori.Length > 0)
                {
                    this.grdUOInf.Visible = true;




                    this.FillDataGridUOInferiori(uoInferiori);



                }

            }

            // gestione naviga UO
            this.GestVisibilitaFrecceNavigaUO();
        }

        /// <summary>
        /// Gestione della spunta Mantieni selezione
        /// </summary>
        private void ChkMantieniSelezione()
        {
            DocumentManager.setSelectedNumberVersion("0");
            DocumentManager.RemoveSelectedAttachId();

            if (!chk_mantieniSel.Checked)
            {
                this.ResetDetail();

                // gestione naviga UO
                if (this.IsEnabledNavigazioneUO())
                {
                    if (this.grdUOApp.Visible && this.grdUOApp.Rows.Count > 0)
                    {
                        if (!this.GetId(this.grdUOApp.Rows[0]).Equals(this.hdUOapp.Value))
                            this.FillGridDestinatariDefault();
                    }
                    else
                        this.FillGridDestinatariDefault();
                }
            }

            // gestione fascicolazione rapida
            //if (this.IsEnabledFascicolazione())
            //    this.ResetFascicolazione();
        }

        /// <summary>
        /// Rimozione delle selezioni effettuate nei datagrid correntemente visualizzate
        /// </summary>
        private void ResetDetail()
        {
            // Rimozione delle selezioni effettuate negli oggetti dello smistamento
            this.ClearSelections();

            foreach (GridViewRow row in this.grdUOApp.Rows)
            {
                CheckBox chkComp = this.GetGridItemControl<CheckBox>(row, "chkComp");
                CheckBox chkCC = this.GetGridItemControl<CheckBox>(row, "chkCC");
                CheckBox chkNotifica = this.GetGridItemControl<CheckBox>(row, "chkNotifica");

                chkComp.Checked = false;
                chkCC.Checked = false;
                chkNotifica.Checked = false;

                if (this.grdUOApp.DataKeys[row.RowIndex].Values["type"].ToString() == "R")
                    this.RefreshItemUoAppartenenza(row, false);
            }

            foreach (GridViewRow row in this.grdUOInf.Rows)
            {
                CheckBox chkComp = this.GetGridItemControl<CheckBox>(row, "chkComp");
                CheckBox chkCC = this.GetGridItemControl<CheckBox>(row, "chkCC");

                chkComp.Checked = false;
                chkCC.Checked = false;

                CustomImageButton imgNote = this.GetGridItemControl<CustomImageButton>(row, "imgNote");
                imgNote.Visible = (chkComp.Checked || chkCC.Checked);
            }

            // gestione fascicolazione rapida
            if (this.IsEnabledFascicolazione())
                this.ResetFascicolazione();

            if (this.UpPnlTransmissionsModel.Visible)
            {
                this.ddl_trasm_rapida.SelectedIndex = -1;
                this.GetSmistaDocManager().GetUOAppartenenza().UoSmistaTrasAutomatica = null;
            }


            this.ResetNoteGenerali();

            this.updatePanelUOAppartenenza.Update();
            this.updatePanelUOInf.Update();
            this.UpPnlCheckDoc.Update();
            this.upPnlInfoDocument.Update();
            this.UpPnlTransmissionsModel.Update();
            this.UpPnlProjectRapid.Update();
            this.UpPnlProject.Update();
        }

        private void ResetNoteGenerali()
        {
            this.txtNoteGen.Text = string.Empty;
        }

        private bool IsEnabledFascicolazione()
        {
            return UserManager.IsAuthorizedFunctions("FASC_SMISTA");
        }

        protected void RefreshItemUoAppartenenza(GridViewRow row, bool onCheckNotificaTutti)
        {
            string id = this.GetId(row);
            string type = this.GetTipoURP(row);
            CheckBox chkComp = this.GetGridItemControl<CheckBox>(row, "chkComp");
            CheckBox chkCC = this.GetGridItemControl<CheckBox>(row, "chkCC");
            CheckBox chkNotifica = this.GetGridItemControl<CheckBox>(row, "chkNotifica");
            CustomImageButton imgNote = this.GetGridItemControl<CustomImageButton>(row, "imgNote");
            Label hd_disablendTrasm = this.GetGridItemControl<Label>(row, "hd_disablendTrasm");

            CheckBox chkNotificaTuttoRuolo = GetCheckNotificaTuttoRuolo(row);

            this.RefreshItemUoAppartenenza(id, type, chkComp, chkCC, chkNotifica, imgNote, onCheckNotificaTutti, hd_disablendTrasm, chkNotificaTuttoRuolo);
        }

        protected void RefreshItemUoAppartenenza(string id, string type, CheckBox chkComp, CheckBox chkCC, CheckBox chkNotifica, CustomImageButton imgNote, bool onCheckNotificaTutti, Label hd_disablendTrasm, CheckBox chkNotificaTuttoRuolo)
        {
            // Determina, tramite il valore del campo nascosto "hdCheckedUtenti",
            // se in fase di selezione di un ruolo deve o meno selezionare in automatico
            // la notifica su tutti gli utenti dello stesso
            bool checkNotificaUtenti;
            bool.TryParse(this.hdCheckedUtenti.Value, out checkNotificaUtenti);

            if (!onCheckNotificaTutti)
            {
                chkNotifica.Enabled = (chkComp.Checked || chkCC.Checked);
                chkNotifica.Checked = checkNotificaUtenti && chkNotifica.Enabled;
            }


            if (type == "R")
            {
                // Selezione ruolo
                imgNote.Visible = (chkComp.Checked || chkCC.Checked);

                bool utentiRuolo = false;

                //Disabilitazione dei cb in quanto il ruolo risulata disabilitato alla ricezione delle trasmissioni
                if (hd_disablendTrasm != null && hd_disablendTrasm.Text == "1")
                {
                    chkComp.Checked = false;
                    chkComp.Enabled = false;
                    chkCC.Checked = false;
                    chkCC.Enabled = false;
                    chkNotifica.Checked = false;
                    chkNotifica.Enabled = false;
                }

                int contatore = 0;
                CheckBox lastUtenteNotifica = null;

                foreach (GridViewRow row in this.grdUOApp.Rows)
                {
                    if (utentiRuolo)
                    {
                        string typeUtente = this.GetTipoURP(row);

                        if (typeUtente == "P")
                        {
                            CheckBox chkCompUtente = this.GetGridItemControl<CheckBox>(row, "chkComp");
                            CheckBox chkCCUtente = this.GetGridItemControl<CheckBox>(row, "chkCC");
                            CheckBox chkNotificaUtente = this.GetGridItemControl<CheckBox>(row, "chkNotifica");

                            contatore++;
                            lastUtenteNotifica = chkNotificaUtente;

                            CustomImageButton imgNoteUtente = this.GetGridItemControl<CustomImageButton>(row, "imgNote");

                            if (onCheckNotificaTutti)
                            {
                                chkNotificaUtente.Checked = chkNotifica.Checked;
                            }
                            else
                            {
                                chkCompUtente.Enabled = this.IsEnabledOptionCompPerGerarchia(row) && !chkComp.Checked && !chkCC.Checked;
                                chkCompUtente.Checked = false;

                                chkCCUtente.Enabled = this.IsEnabledOptionCCPerGerarchia(row) && !chkComp.Checked && !chkCC.Checked;
                                chkCCUtente.Checked = false;

                                chkNotificaUtente.Enabled = (chkComp.Checked || chkCC.Checked);
                                chkNotificaUtente.Checked = checkNotificaUtenti && chkNotifica.Checked;

                                imgNoteUtente.Visible = false;


                            }
                        }
                        else
                        {
                            utentiRuolo = false;

                            if (contatore == 1)
                            {
                                lastUtenteNotifica.Checked = chkComp.Checked || chkCC.Checked;
                            }
                            break;
                        }
                    }
                    else if (this.GetId(row) == id)
                    {
                        utentiRuolo = true;
                    }
                }

                if (utentiRuolo && contatore == 1)
                {
                    lastUtenteNotifica.Checked = chkComp.Checked || chkCC.Checked;
                }
            }
            else if (type == "P")
            {
                // Selezione riga utente
                if ((chkComp.Checked || chkCC.Checked) && chkComp.Enabled && chkCC.Enabled)
                    // Il check è disabilitato nel caso in cui venga marcato direttamente
                    // competenza o conoscenza per l'utente
                    chkNotifica.Enabled = false;
                else
                {
                    // Il check è abilitato se i flag competenza e conoscenza sono disabilitati e non marcati
                    chkNotifica.Enabled = (!chkComp.Enabled && !chkCC.Enabled);

                    //Se il check notifica del ruolo è abilitato, abilito anche quello ad utente
                    if(chkNotificaTuttoRuolo != null)
                        chkNotifica.Enabled = chkNotificaTuttoRuolo.Enabled;
                }
                imgNote.Visible = (chkComp.Checked || chkCC.Checked);


            }
        }

        /// <summary>
        /// Gestione abilitazione / disabilitazione pulsante per 
        /// la visualizzaione dei dettagli di firma del documento
        /// </summary>
        private void EnableButtonDettagliFirma()
        {
            try
            {
                bool buttonEnabled = false;
                if (!string.IsNullOrEmpty(DocumentManager.getSelectedAttachId()))
                {
                    FileRequest file = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId());
                    buttonEnabled = file.firmato.Equals("1");
                }
                else
                {
                    DocsPaWR.DocumentoSmistamento docSmistamento = this.GetSmistaDocManager().GetCurrentDocument(false);

                    if (docSmistamento.ImmagineDocumento != null)
                    {
                        if (docSmistamento.ImmagineDocumento.signatureResult != null)
                            // Se sono presenti i dettagli di firma, abilita il pulsante
                            buttonEnabled = true;

                        else if (docSmistamento.ImmagineDocumento.name.ToLower().EndsWith(".p7m"))
                            // Se non sono presenti i dettagli di firma, si verifica l'estensione del
                            // nome del file, se ".p7m" abilita il pulsante
                            buttonEnabled = true;
                    }
                }
                this.BtnSmistamentoDetSign.Enabled = buttonEnabled;
            }
            catch (Exception err) { }
        }

        private void EnabledNavigationButtons()
        {
            SmistaDocManager docManager = this.GetSmistaDocManager();
            int docIndex = docManager.GetCurrentDocumentPosition();
            int docLastIndex = docManager.GetDocumentCount();

            this.btn_previous.Enabled = (docIndex > 1);
            this.btn_next.Enabled = (docIndex < docLastIndex);
            this.btn_first.Enabled = this.btn_previous.Enabled;
            this.btn_last.Enabled = this.btn_next.Enabled;

            if (this.btn_first.Enabled)
            {
                this.btn_first.ImageUrl = "~/Images/Icons/smistamento_left_left.png";
                this.btn_previous.ImageUrl = "~/Images/Icons/smistamento_left.png";
            }
            else
            {
                this.btn_first.ImageUrl = "~/Images/Icons/smistamento_left_left_disabled.png";
                this.btn_previous.ImageUrl = "~/Images/Icons/smistamento_left_disabled.png";
            }

            if (this.btn_last.Enabled)
            {
                this.btn_last.ImageUrl = "~/Images/Icons/smistamento_right_right.png";
                this.btn_next.ImageUrl = "~/Images/Icons/smistamento_right.png";
            }
            else
            {
                this.btn_last.ImageUrl = "~/Images/Icons/smistamento_right_right_disabled.png";
                this.btn_next.ImageUrl = "~/Images/Icons/smistamento_right_disabled.png";
            }
        }

        /// <summary>
        /// Impostazione contenuto file firmato digitalmente in sessione
        /// </summary>
        /// <param name="fileDocumento"></param>
        private void SetSignedDocumentOnSession(DocsPaWR.FileDocumento fileDocumento)
        {
            if (fileDocumento != null)
                DocumentManager.SetSignedDocument(fileDocumento);
            else
                DocumentManager.RemoveSignedDocument();
        }

        private void RefreshDocumentCounter()
        {
            SmistaDocManager docManager = this.GetSmistaDocManager();
            lbl_contatore.Text = Convert.ToString(docManager.GetCurrentDocumentPosition()) + " / " + Convert.ToString(docManager.GetDocumentCount());
            this.upPnlInfoDocument.Update();
        }

        private void caricaModelliTrasmRapida()
        {
            this.ddl_trasm_rapida.Items.Clear();

            string idAmm = UserManager.GetInfoUser().idAmministrazione;
            string idPeople = UserManager.GetInfoUser().idPeople;
            string idCorrGlobali = UserManager.GetInfoUser().idCorrGlobali;
            string idRuoloUtente = UserManager.GetInfoUser().idGruppo;
            string idTipoDoc = "";
            string idDiagramma = "";
            string idStato = "";
            ////string errorMessage = "";

            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            SmistaDocManager docManager = this.GetSmistaDocManager();
            DocsPaWR.DocumentoSmistamento docCorrente = docManager.GetCurrentDocument(false);

            if (this.DocumentInWorking == null || this.DocumentInWorking.systemId != docCorrente.IDDocumento)
            {
                //  DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDettaglioDocumentoNoDataVista(this, docCorrente.IDDocumento, docCorrente.DocNumber);
                DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentListVersions(this, docCorrente.IDDocumento, docCorrente.DocNumber);
                DocumentManager.SetDataVistaSP(schedaDocumento.systemId);
                DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                this.DocumentInWorking = schedaDocumento;
            }

            string settingValue = System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"];

            if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
            {
                if (this.DocumentInWorking.tipologiaAtto != null)
                {
                    if (this.DocumentInWorking.template != null && this.DocumentInWorking.template.SYSTEM_ID.ToString() != "")
                    {
                        idTipoDoc = this.DocumentInWorking.template.SYSTEM_ID.ToString();

                        if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                        {
                            DocsPaWR.DiagrammaStato dg = DiagrammiManager.getDgByIdTipoDoc(this.DocumentInWorking.tipologiaAtto.systemId, idAmm);
                            if (dg != null)
                            {
                                idDiagramma = dg.SYSTEM_ID.ToString();
                                DocsPaWR.Stato stato = DiagrammiManager.GetStateDocument(this.DocumentInWorking.docNumber);
                                if (stato != null)
                                    idStato = stato.SYSTEM_ID.ToString();
                            }
                        }
                    }
                }
            }

            DocsPaWR.Registro[] registri = null;
            if (docCorrente.TipoDocumento == "G")
            {
                registri = ((DocsPaWR.Ruolo)RoleManager.GetRoleInSession()).registri;
            }
            else
            {
                registri = new DocsPaWR.Registro[1];
                registri[0] = this.DocumentInWorking.registro;
            }
            ArrayList idModelli = new ArrayList(ws.getModelliPerTrasmLite(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, "D", this.DocumentInWorking.systemId, idRuoloUtente, false, this.DocumentInWorking.accessRights));

            if (this.ddl_trasm_rapida.Items.Count == 0)
            {
                System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
                li.Text = " ";
                this.ddl_trasm_rapida.Items.Add(li);
            }

            for (int i = 0; i < idModelli.Count; i++)
            {
                DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)idModelli[i];
                System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
                li.Value = mod.SYSTEM_ID.ToString();
                li.Text = mod.NOME;
                if (System.Configuration.ConfigurationManager.AppSettings["VISUALIZZA_CODICE_MODELLI_TRASM"] != null && System.Configuration.ConfigurationManager.AppSettings["VISUALIZZA_CODICE_MODELLI_TRASM"] == "1")
                    li.Text += " (" + mod.CODICE + ")";
                this.ddl_trasm_rapida.Items.Add(li);
            }
        }

        private bool cfg_Smista_Abilita_Trasm_Rapida()
        {
            string valoreChiaveDB = Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, "FE_SMISTA_ABILITA_TRASM_RAPIDA");
            // Se il valore è diverso da null oppure è "1" allora ritorna true, altrimenti torna false;
            return (valoreChiaveDB != null && valoreChiaveDB.Equals("1")) ? true : false;
        }

        private void gestisciPopUpSelezioneSmistamento()
        {
            if (this.SmistaDocSelectionsDetails.ReturnValue == "smista")
            {
                this.smistaDocumento();
            }
            else
            {
                this.navigaUoSelezionata(this.SmistaDocSelectionsDetails.ReturnValue);
                this.GestVisibilitaFrecceNavigaUO();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SmistaDocSelectionsDetails','');", true);
            }
        }

        private void navigaUoSelezionata(string idUo)
        {
            bool selezioneUoApp = false;
            bool selezioneUoInf = false;
            bool selezioneUtenteRuolo = false;

            SmistaDocManager docManager = this.GetSmistaDocManager();
            DocsPaWR.UOSmistamento uoAppartenenza = docManager.GetUOAppartenenza();
            string idUOPadre = this.getUOPadre(idUo);
            DocsPaWR.UOSmistamento uoSelezionata = this.getUoSelezionata(uoAppartenenza, idUo);

            uoSelezionata.Selezionata = true;

            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            if (uoSelezionata.UoInferiori == null || uoSelezionata.UoInferiori.Length == 0)
                uoSelezionata.UoInferiori = ws.GetUOInferiori(idUo, docManager.getMittenteSmistamento());
            if (uoSelezionata.ID == uoAppartenenza.ID)
                idUOPadre = uoSelezionata.ID;
            this.UpdateFlagUo(idUOPadre, out selezioneUoApp, out selezioneUoInf, out selezioneUtenteRuolo);
            this.navigaUO_down(idUo);
            this.setFlagDataGrid();
            this.indxUoSel = idUo;
        }

        /// <summary>
        /// Indice dell'UO correntemente selezionata come di appartenenza
        /// </summary>
        /// <remarks>
        /// Nella navigazione UO inferiore / superiore, l'indice cambia e 
        /// diventa quello dell'UO su cui si clicca il pulsante "Naviga Inferiori"
        /// o quello dell'UO parent quando si clicca sul pulsante "Naviga Superiori"
        /// </remarks>
        protected string indxUoSel
        {
            get
            {
                if (this.ViewState["indxUoSel"] != null)
                    return this.ViewState["indxUoSel"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["indxUoSel"] = value;
            }
        }

        private void UpdateFlagUo(string idUo, out bool selezioneUoApp, out  bool selezioneUoInf, out bool selezioneUtenteRuolo)
        {
            SmistaDocManager docManager = this.GetSmistaDocManager();

            DocsPaWR.UOSmistamento uo = this.getUoSelezionata(docManager.GetUOAppartenenza(), idUo);

            this.UpdateFlagUOAppartenenza(uo, out selezioneUoApp);

            this.UpdateFlagUOInferiori(uo.UoInferiori, out selezioneUoInf);

            this.UpadateFlagUtentiRuoloSelezionato(uo, out selezioneUtenteRuolo);

        }

        private void UpadateFlagUtentiRuoloSelezionato(DocsPaWR.UOSmistamento uoApp, out bool selezioneUtenteRuolo)
        {
            selezioneUtenteRuolo = false;

            if (this.grdUOApp.Visible && this.grdUOApp.Rows.Count > 0)
            {
                SmistaDocManager docManager = this.GetSmistaDocManager();

                // gestione naviga UO
                if (!this.grdUOApp.DataKeys[0].Values["id"].ToString().Equals(this.hdUOapp.Value))
                    docManager.FillCurrentUO_NavigaUO(this.grdUOApp.DataKeys[0].Values["id"].ToString(), RoleManager.GetRoleInSession(), UserManager.GetUserInSession(), UserManager.GetInfoUser());



                //Controllo se per i ruoli selezionati delle UO di Appartenenza sia selezionato almeno un utente
                //Caso che si puo' verificare in quanto adesso la notifica puo' essere abilitata o meno da web.config
                foreach (DocsPaWR.RuoloSmistamento ruolo in uoApp.Ruoli)
                {
                    if (ruolo.FlagCompetenza || ruolo.FlagConoscenza)
                    {
                        int i = 0;
                        for (i = 0; i < ruolo.Utenti.Length; i++)
                        {
                            DocsPaWR.UtenteSmistamento utente = (DocsPaWR.UtenteSmistamento)ruolo.Utenti[i];
                            if (utente.FlagCompetenza || utente.FlagConoscenza)
                                break;
                        }
                        if (i == ruolo.Utenti.Length)
                        {
                            selezioneUtenteRuolo = true;
                            break;
                        }
                    }
                }
            }

            this.updatePanelUOAppartenenza.Update();
            this.updatePanelUOInf.Update();
            this.UpPnlCheckDoc.Update();
            this.upPnlInfoDocument.Update();
            this.UpPnlTransmissionsModel.Update();
            this.UpPnlProjectRapid.Update();
            this.UpPnlProject.Update();
        }

        private void UpdateFlagUOAppartenenza(DocsPaWR.UOSmistamento uoApp, out bool selezioneUoApp)
        {
            //bool retValue = false;
            selezioneUoApp = false;

            if (this.grdUOApp.Visible && this.grdUOApp.Rows.Count > 0)
            {
                SmistaDocManager docManager = this.GetSmistaDocManager();

                // gestione naviga UO                
                if (!this.grdUOApp.DataKeys[0].Values["id"].ToString().Equals(this.hdUOapp.Value))
                    docManager.FillCurrentUO_NavigaUO(this.grdUOApp.DataKeys[0].Values["id"].ToString(), RoleManager.GetRoleInSession(), UserManager.GetUserInSession(), UserManager.GetInfoUser());

                string itemID = string.Empty;
                string type = string.Empty;
                bool flagComp = false;
                bool flagCC = false;
                bool flagNotifica = false;

                foreach (GridViewRow row in this.grdUOApp.Rows)
                {
                    this.LoadItemValues(row, false, out itemID, out type, out flagComp, out flagCC, out flagNotifica);

                    if ((type.Equals("R") || type.Equals("P")))
                    {
                        this.SetFlagUO(true, uoApp, type, itemID, flagComp, flagCC, flagNotifica, row.DataItemIndex);
                        selezioneUoApp = true;
                    }
                }
            }
            this.updatePanelUOAppartenenza.Update();
            this.updatePanelUOInf.Update();
            this.UpPnlCheckDoc.Update();
            this.upPnlInfoDocument.Update();
            this.UpPnlTransmissionsModel.Update();
            this.UpPnlProjectRapid.Update();
            this.UpPnlProject.Update();
        }

        private void SetFlagUO(bool isUOAppartenenza,
                                DocsPaWR.UOSmistamento uo,
                                string type,
                                string id,
                                bool flagComp,
                                bool flagCC,
                                bool flagNotifica,
                                int indice)
        {
            switch (type)
            {
                case "U": // ----------------------- Unità organizzative -----------------
                    uo.FlagCompetenza = flagComp;
                    uo.FlagConoscenza = flagCC;

                    // Impostazione flag per i ruoli della UO
                    this.SetFlagRuoliUO(isUOAppartenenza, uo, flagComp, flagCC);
                    break;

                case "R": // ----------------------- Ruoli -------------------------------
                    foreach (DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
                    {
                        if (ruolo.ID.Equals(id))
                        {
                            ruolo.FlagCompetenza = flagComp;
                            ruolo.FlagConoscenza = flagCC;

                            return;
                        }
                    }
                    break;

                case "P": // ----------------------- Utenti ------------------------------
                    string uniqueID = string.Empty;

                    foreach (DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
                    {
                        foreach (DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
                        {
                            uniqueID = ruolo.ID + "_" + utente.ID;

                            if (uniqueID.Equals(id))
                            {
                                if (!flagComp && !flagCC && flagNotifica)
                                {
                                    // Se per l'utente risulta selezionata solamente l'opzione di notifica,
                                    // vengono impostati gli stessi valori dei flag del ruolo di appartenenza
                                    // NB: è il caso in cui lo smistamento è a ruolo
                                    utente.FlagCompetenza = ruolo.FlagCompetenza;
                                    utente.FlagConoscenza = ruolo.FlagConoscenza;
                                }
                                else
                                {
                                    // Selezione esplicita dell'utente
                                    // NB: è il caso in cui lo smistamento è direttamente ad utente
                                    utente.FlagCompetenza = flagComp;
                                    utente.FlagConoscenza = flagCC;
                                }

                                return;
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Impostazione flag per i ruoli e utenti della uo
        /// </summary>
        /// <param name="uo"></param>
        /// <param name="flagComp"></param>
        /// <param name="flagCC"></param>
        private void SetFlagRuoliUO(bool isUOAppartenenza, DocsPaWR.UOSmistamento uo, bool flagComp, bool flagCC)
        {
            foreach (DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
            {
                if (ruolo.RuoloRiferimento && isUOAppartenenza)
                {
                    ruolo.FlagCompetenza = flagComp;
                    ruolo.FlagConoscenza = flagCC;

                    this.SetFlagUtentiRuolo(ruolo, flagComp, flagCC);
                }
            }
        }

        private void LoadItemValues(GridViewRow row,
            bool inf,
                                    out string id,
                                    out string type,
                                    out bool flagComp,
                                    out bool flagCC,
                                    out bool flagNotifica)
        {
            //id = this.grdUOApp.DataKeys[0].Values["id"].ToString();
            //type = this.grdUOApp.DataKeys[0].Values["type"].ToString();
            if (inf)
            {
                id = this.grdUOInf.DataKeys[row.RowIndex].Values["id"].ToString();
                type = this.grdUOInf.DataKeys[row.RowIndex].Values["type"].ToString();
            }
            else
            {
                id = this.grdUOApp.DataKeys[row.RowIndex].Values["id"].ToString();
                type = this.grdUOApp.DataKeys[row.RowIndex].Values["type"].ToString();
            }
            flagComp = false;
            flagCC = false;
            flagNotifica = false;

            CheckBox chkComp = this.GetGridItemControl<CheckBox>(row, "chkComp");// row.Cells[3].FindControl("chkComp") as CheckBox;
            CheckBox chkCC = this.GetGridItemControl<CheckBox>(row, "chkCC"); //row.Cells[3].FindControl("chkCC") as CheckBox;
            CheckBox chkNotifica = this.GetGridItemControl<CheckBox>(row, "chkNotifica"); //row.Cells[3].FindControl("chkNotifica") as CheckBox;

            if (chkComp != null && chkCC != null && chkComp.Visible && chkCC.Visible)
            {
                flagComp = chkComp.Checked;
                flagCC = chkCC.Checked;

                // NB: condizione su null necessaria in quanto la griglia uo inferiori non ha il flag di notifica
                flagNotifica = (chkNotifica != null && chkNotifica.Checked);
            }
            this.updatePanelUOAppartenenza.Update();
            this.updatePanelUOInf.Update();
            this.UpPnlCheckDoc.Update();
            this.upPnlInfoDocument.Update();
            this.UpPnlTransmissionsModel.Update();
            this.UpPnlProjectRapid.Update();
            this.UpPnlProject.Update();
        }

        /// <summary>
        /// Impostazione flag per gli utenti di un ruolo
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="flagComp"></param>
        /// <param name="flagCC"></param>
        private void SetFlagUtentiRuolo(DocsPaWR.RuoloSmistamento ruolo, bool flagComp, bool flagCC)
        {
            foreach (DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
            {
                utente.FlagCompetenza = flagComp;
                utente.FlagConoscenza = flagCC;
            }
        }

        private void UpdateFlagUOInferiori(DocsPaWR.UOSmistamento[] uoInferiori, out bool selezioneUoInf)
        {
            selezioneUoInf = false;

            if (this.grdUOInf.Visible && this.grdUOInf.Rows.Count > 0)
            {
                SmistaDocManager docManager = this.GetSmistaDocManager();

                // gestione naviga UO
                if (this.grdUOApp.Visible && this.grdUOApp.Rows.Count > 0)
                    if (!this.grdUOApp.DataKeys[0].Values["id"].ToString().Equals(this.hdUOapp.Value))
                        docManager.FillUOInf_NavigaUO(this.grdUOApp.DataKeys[0].Values["id"].ToString(), RoleManager.GetRoleInSession(), UserManager.GetUserInSession(), UserManager.GetInfoUser());

                string itemID = string.Empty;
                string type = string.Empty;
                bool flagComp = false;
                bool flagCC = false;
                bool flagNotifica = false;

                foreach (GridViewRow row in this.grdUOInf.Rows)
                {
                    this.LoadItemValues(row, true, out itemID, out type, out flagComp, out flagCC, out flagNotifica);

                    if (type.Equals("U"))
                    {
                        foreach (DocsPaWR.UOSmistamento currentUO in uoInferiori)
                        {
                            bool uoChanged = false;

                            if (!currentUO.Selezionata)
                            {
                                if (currentUO.ID.Equals(itemID))
                                {
                                    uoChanged = (flagComp != currentUO.FlagCompetenza || flagCC != currentUO.FlagConoscenza);

                                    currentUO.FlagCompetenza = flagComp;
                                    currentUO.FlagConoscenza = flagCC;

                                    foreach (DocsPaWR.RuoloSmistamento ruolo in currentUO.Ruoli)
                                    {
                                        // Smistamento del documento solamente ai ruoli di riferimento
                                        if (
                                            (ruolo.RuoloRiferimento) &&
                                                (
                                                    (flagComp && (this.hdTipoDestCompetenza.Value == string.Empty || ruolo.Gerarchia == this.hdTipoDestCompetenza.Value)) ||
                                                    (flagCC && (this.hdTipoDestConoscenza.Value == string.Empty || ruolo.Gerarchia == this.hdTipoDestConoscenza.Value))
                                                )
                                           )
                                        {
                                            ruolo.FlagCompetenza = flagComp;
                                            ruolo.FlagConoscenza = flagCC;

                                            SetFlagUtentiRuolo(ruolo, flagComp, flagCC);

                                            selezioneUoInf = true;
                                        }
                                        else
                                        {
                                            ruolo.FlagCompetenza = false;
                                            ruolo.FlagConoscenza = false;

                                            SetFlagUtentiRuolo(ruolo, false, false);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (currentUO.ID.Equals(itemID))
                                {
                                    uoChanged = (flagComp != currentUO.FlagCompetenza || flagCC != currentUO.FlagConoscenza);

                                    currentUO.FlagCompetenza = flagComp;
                                    currentUO.FlagConoscenza = flagCC;

                                    foreach (DocsPaWR.RuoloSmistamento ruolo in currentUO.Ruoli)
                                    {
                                        if (ruolo.RuoloRiferimento)
                                        {
                                            if (uoChanged || (!ruolo.FlagCompetenza && !ruolo.FlagConoscenza))
                                            {
                                                ruolo.FlagCompetenza = flagComp;
                                                ruolo.FlagConoscenza = flagCC;

                                                foreach (DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
                                                {
                                                    if (uoChanged || (!utente.FlagCompetenza && !utente.FlagConoscenza))
                                                    {
                                                        utente.FlagCompetenza = flagComp;
                                                        utente.FlagConoscenza = flagCC;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this.updatePanelUOAppartenenza.Update();
            this.updatePanelUOInf.Update();
            this.UpPnlCheckDoc.Update();
            this.upPnlInfoDocument.Update();
            this.UpPnlTransmissionsModel.Update();
            this.UpPnlProjectRapid.Update();
            this.UpPnlProject.Update();
        }

        private void setFlagDataGrid()
        {
            DocsPaWR.UOSmistamento uoSelezionata = (DocsPaWR.UOSmistamento)Session["UoSelezionata"];

            int i = 0;

            if (uoSelezionata.Ruoli != null && uoSelezionata.Ruoli.Length > 0)
            {
                foreach (DocsPaWR.RuoloSmistamento ruolo in uoSelezionata.Ruoli)
                {
                    if (ruolo.Utenti != null && ruolo.Utenti.Length > 0)
                    {
                        i++;

                        CheckBox chkComp = this.GetGridItemControl<CheckBox>(this.grdUOApp.Rows[i], "chkComp");
                        CheckBox chkCC = this.GetGridItemControl<CheckBox>(this.grdUOApp.Rows[i], "chkCC");
                        CheckBox chkNotifica = this.GetGridItemControl<CheckBox>(this.grdUOApp.Rows[i], "chkNotifica");
                        CustomImageButton imgNote = this.GetGridItemControl<CustomImageButton>(this.grdUOApp.Rows[i], "imgNote");

                        chkComp.Checked = ruolo.FlagCompetenza;
                        chkCC.Checked = ruolo.FlagConoscenza;

                        chkNotifica.Checked = false;
                        chkNotifica.Enabled = (ruolo.FlagCompetenza || ruolo.FlagConoscenza);
                        imgNote.Visible = (ruolo.FlagCompetenza || ruolo.FlagConoscenza);

                        foreach (DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
                        {
                            i++;

                            bool compEnabledPerGerarchia = this.IsEnabledOptionCompPerGerarchia(this.grdUOApp.Rows[i]);
                            bool ccEnabledPerGerarchia = this.IsEnabledOptionCCPerGerarchia(this.grdUOApp.Rows[i]);

                            chkComp = this.GetGridItemControl<CheckBox>(this.grdUOApp.Rows[i], "chkComp");
                            chkCC = this.GetGridItemControl<CheckBox>(this.grdUOApp.Rows[i], "chkCC");
                            chkNotifica = this.GetGridItemControl<CheckBox>(this.grdUOApp.Rows[i], "chkNotifica");
                            imgNote = this.GetGridItemControl<CustomImageButton>(this.grdUOApp.Rows[i], "imgNote");

                            if (ruolo.FlagCompetenza || ruolo.FlagConoscenza)
                                // Se è stata fatta una selezione a livello di ruolo,
                                // il check per l'utente non è impostato
                                chkComp.Checked = false;
                            else
                                chkComp.Checked = utente.FlagCompetenza;

                            chkComp.Enabled = compEnabledPerGerarchia && (!ruolo.FlagCompetenza && !ruolo.FlagConoscenza);

                            if (ruolo.FlagCompetenza || ruolo.FlagConoscenza)
                                // Se è stata fatta una selezione a livello di ruolo,
                                // il check per l'utente non è impostato
                                chkCC.Checked = false;
                            else
                                chkCC.Checked = utente.FlagConoscenza;

                            chkCC.Enabled = ccEnabledPerGerarchia && (!ruolo.FlagCompetenza && !ruolo.FlagConoscenza);

                            // Flag di notifica checked solo se per l'utente è stata 
                            // effettuata una selezione
                            chkNotifica.Checked = (utente.FlagCompetenza || utente.FlagConoscenza);
                            chkNotifica.Enabled = (ruolo.FlagCompetenza || ruolo.FlagConoscenza);

                            imgNote.Visible = (utente.FlagCompetenza || utente.FlagConoscenza) &&
                                              (!ruolo.FlagCompetenza && !ruolo.FlagConoscenza);
                        }
                    }
                }
            }

            i = 0;

            if (uoSelezionata.UoInferiori != null && uoSelezionata.UoInferiori.Length > 0)
            {
                foreach (DocsPaWR.UOSmistamento uo in uoSelezionata.UoInferiori)
                {
                    CheckBox chkComp = this.GetGridItemControl<CheckBox>(this.grdUOInf.Rows[i], "chkComp");
                    CheckBox chkCC = this.GetGridItemControl<CheckBox>(this.grdUOInf.Rows[i], "chkCC");
                    CustomImageButton imgNote = this.GetGridItemControl<CustomImageButton>(this.grdUOInf.Rows[i], "imgNote");

                    chkComp.Checked = uo.FlagCompetenza;
                    chkCC.Checked = uo.FlagConoscenza;
                    imgNote.Visible = (uo.FlagCompetenza || uo.FlagConoscenza);

                    i++;
                }
            }

        }

        /// <summary>
        /// Metodo per il recupero del fascicolo da codice
        /// </summary>
        /// <returns></returns>
        private DocsPaWR.Fascicolo getFascicolo()
        {
            DocsPaWR.Fascicolo fascicoloSelezionato = null;
            string codiceFascicolo = string.Empty;
            string descrizione = string.Empty;
            DocsPaWR.Registro registro = this.Registry;
            if (!this.TxtCodeProject.Text.Equals(""))
            {
                if (this.TxtCodeProject.Text.IndexOf("//") > -1)
                {
                    fascicoloSelezionato = getFolder(UserManager.getRegistroSelezionato(this), ref codiceFascicolo, ref descrizione);
                }
                else
                {
                    codiceFascicolo = TxtCodeProject.Text;

                    //OLD: fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice(this, codiceFascicolo);

                    DocsPaWR.Fascicolo[] FascS = ProjectManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "I");

                    if (FascS != null && FascS.Length > 0 && FascS[0] != null)
                    {
                        fascicoloSelezionato = (DocsPaWR.Fascicolo)FascS[0];
                    }
                }
            }
            if (fascicoloSelezionato != null)
            {
                return fascicoloSelezionato;
            }
            else
            {
                return null;
            }
        }

        public void setTipoRagioneSmistamento(DocsPaWR.RagioneTrasmissione[] listaRagSmistamento)
        {
            hdCheckCompetenza.Value = listaRagSmistamento[0].tipo.ToString(); // tipo_ragione della COMPETENZA

            hdCheckConoscenza.Value = listaRagSmistamento[1].tipo.ToString();  // tipo_ragione della CONOSCENZA

            hdTipoDestCompetenza.Value = setTipoDestRagione((DocsPaWR.TramissioneTipoGerarchia)(listaRagSmistamento[0].tipoDestinatario));

            hdTipoDestConoscenza.Value = setTipoDestRagione((DocsPaWR.TramissioneTipoGerarchia)(listaRagSmistamento[1].tipoDestinatario));

            hdDescrCompetenza.Value = listaRagSmistamento[0].descrizione.ToString(); // descrizione rgione COMPETENZA

            hdDescrConoscenza.Value = listaRagSmistamento[1].descrizione.ToString(); // descrizione ragione CONOSCENZA           
        }

        private string setTipoDestRagione(DocsPaWR.TramissioneTipoGerarchia tipoDirittoRagione)
        {
            string tipoDest = String.Empty;

            switch (tipoDirittoRagione)
            {
                case DocsPaWR.TramissioneTipoGerarchia.TUTTI:
                    tipoDest = "";
                    break;

                case DocsPaWR.TramissioneTipoGerarchia.INFERIORE:
                    tipoDest = "0";
                    break;

                case DocsPaWR.TramissioneTipoGerarchia.SUPERIORE:
                    tipoDest = "1";
                    break;

                case DocsPaWR.TramissioneTipoGerarchia.PARILIVELLO:
                    tipoDest = "2";
                    break;
            }

            return tipoDest;
        }

        public void setCampoNascostoGestioneCheckUtenti()
        {
            if (TrasmManager.getTxRuoloUtentiChecked())
                hdCheckedUtenti.Value = "true";
            else
                hdCheckedUtenti.Value = "false";

        }

        public SmistaDocManager GetSmistaDocManager()
        {
            string idDocumento = Request.QueryString["DOC_NUMBER"];

            if (idDocumento == null) idDocumento = ""; //LNR verificare cmq dal vecchio all'inizio è empty non null

            // Reperimento oggetto "DocumentoTrasmesso" corrente
            return SmistaDocSessionManager.GetSmistaDocManager(idDocumento);
        }

        private UOSmistamento getUoSelezionata(DocsPaWR.UOSmistamento uo, string id)
        {
            DocsPaWR.UOSmistamento uoSel = new DocsPaWR.UOSmistamento();
            if (uo.ID == id)
            {
                uoSel = uo;
            }
            else
            {
                if (uo.UoInferiori != null && uo.UoInferiori.Length > 0)
                {
                    foreach (DocsPaWR.UOSmistamento uoInf in uo.UoInferiori)
                    {
                        if (string.IsNullOrEmpty(uoSel.ID))
                        {
                            uoSel = getUoSelezionata(uoInf, id);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return uoSel;
        }

        /// <summary>
        /// Caricamento griglia uo di appartenenza
        /// </summary>
        /// <param name="uoApp"></param>
        private void FillDataGridUOApp(DocsPaWR.UOSmistamento uoApp)
        {
            DataSet ds = this.CreateGridDataSet();

            this.FillGridDataSet(ds, uoApp, true);

            this.grdUOApp.DataSource = ds;
            this.grdUOApp.DataBind();

            this.SetRowControlsVisibility(this.grdUOApp.Rows[0], false);

            // Gestione abilitazione / disabilitazione checkbox 
            // in funzione delle regole di gerarchia delle ragioni trasmissione
            this.RefreshOptionsEnabledPerGerarchia(this.grdUOApp);

            DisabledOrEnabledCbComFGridUo(this.grdUOApp);

            SetFlagDestinatarioRaggiutoDaTrasm(this.grdUOApp, this.hdUOapp.Value);

            this.updatePanelUOAppartenenza.Update();

        }

        private DataSet CreateGridDataSet()
        {
            DataSet retValue = new DataSet();

            DataTable dt = new DataTable("GRID_TABLE");
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("IDCORR", typeof(string));
            dt.Columns.Add("TYPE", typeof(string));
            dt.Columns.Add("DESCRIZIONE", typeof(string));
            dt.Columns.Add("DESCR", typeof(string));
            dt.Columns.Add("PARENT", typeof(string));
            dt.Columns.Add("ROWINDEX", typeof(Int32));
            dt.Columns.Add("GERARCHIA", typeof(string));
            dt.Columns.Add("DISABLED_TRASM", typeof(string));
            retValue.Tables.Add(dt);

            return retValue;
        }


        private void FillGridDataSetUoInfNoRif(DataSet ds, UOSmistamento uo, bool isUOAppartenenza)
        {
            DataTable dt = ds.Tables["GRID_TABLE"];

            DocsPaWR.RuoloSmistamento[] ruoloRiferimento = uo.Ruoli.Where(e => e.RuoloRiferimento).ToArray();
            bool isDisabledTrasm = ruoloRiferimento.Length > 0 ? false : true;

            this.AppendDataRow(dt, "U", uo.ID, uo.Descrizione, "", "", isDisabledTrasm, uo.ID);


        }

        /// <summary>
        /// Caricamento dataset utilizzato per le griglie
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="uo"></param>
        private void FillGridDataSet(DataSet ds, UOSmistamento uo, bool isUOAppartenenza)
        {
            DataTable dt = ds.Tables["GRID_TABLE"];

            this.AppendDataRow(dt, "U", uo.ID, uo.Descrizione, "", "", false, uo.ID);

            if (isUOAppartenenza)
            {
                foreach (DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
                {


                    if (ruolo.Utenti != null && ruolo.Utenti.Length > 0)
                    {
                        this.AppendDataRow(dt, "R", ruolo.ID, ruolo.Descrizione, ruolo.ID, ruolo.Gerarchia, ruolo.disabledTrasm, ruolo.ID);



                        foreach (DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
                        {
                            this.AppendDataRow(dt, "P", ruolo.ID + "_" + utente.ID, utente.Denominazione, ruolo.ID, ruolo.Gerarchia, ruolo.disabledTrasm, utente.IDCorrGlobali);
                        }
                    }
                }
            }
        }

        private void AppendDataRow(DataTable dt, string type, string id, string descrizione, string padre, string superiore, bool disabledTrasm, string idCorr)
        {
            DataRow row = dt.NewRow();
            row["ID"] = id;
            row["IDCORR"] = idCorr;
            row["TYPE"] = type;
            row["DESCRIZIONE"] = this.GetImage(type) + " " + descrizione;
            row["DESCR"] = descrizione;
            row["PARENT"] = padre;
            row["GERARCHIA"] = superiore;
            row["DISABLED_TRASM"] = disabledTrasm ? "1" : "0";
            dt.Rows.Add(row);

            row = null;
        }

        private string GetImage(string rowType)
        {
            string retValue = string.Empty;
            string spaceIndent = string.Empty;

            switch (rowType)
            {
                case "U":
                    retValue = "uo_icon";
                    break;

                case "R":
                    retValue = "role2_icon";
                    spaceIndent = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    break;

                case "P":
                    retValue = "user_icon";
                    spaceIndent = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    break;
            }

            retValue = spaceIndent + "<img src='../images/icons/" + retValue + ".png' border='0'>";

            return retValue;
        }

        /// <summary>
        /// Impostazione visibilità controlli di un elemento del datagrid (sia appartenenza che inferiori)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="visible"></param>
        private void SetRowControlsVisibility(GridViewRow row, bool visible)
        {
            CheckBox radio = this.GetGridItemControl<CheckBox>(row, "chkComp");
            if (radio != null)
                radio.Visible = visible;

            radio = this.GetGridItemControl<CheckBox>(row, "chkCC");
            if (radio != null)
                radio.Visible = visible;

            radio = this.GetGridItemControl<CheckBox>(row, "chkNotifica");
            if (radio != null)
                radio.Visible = visible;

            CustomImageButton imgNote = this.GetGridItemControl<CustomImageButton>(row, "imgNote");

            if (imgNote != null)
                imgNote.Visible = visible;

            imgNote = this.GetGridItemControl<CustomImageButton>(row, "ImgButtonTrasmesso");
            if (imgNote != null)
                imgNote.Visible = visible;
        }

        protected T GetGridItemControl<T>(GridViewRow row, string controlId) where T : Control
        {
            Control ctrl = row.FindControl(controlId);

            if (ctrl == null)
                return default(T);
            else
                return (T)ctrl;
        }

        /// <summary>
        /// Gestione abilitazione / disabilitazione opzioni in funzione
        /// delle regole di gerarchia delle ragioni trasmissione
        /// </summary>
        /// <param name="dataGrid"></param>
        protected void RefreshOptionsEnabledPerGerarchia(GridView gridView)
        {
            foreach (GridViewRow row in gridView.Rows)
            {
                this.RefreshOptionsEnabledPerGerarchia(row);
            }
        }

        protected virtual void RefreshOptionsEnabledPerGerarchia(GridViewRow row)
        {
            CheckBox chkComp = this.GetGridItemControl<CheckBox>(row, "chkComp");
            chkComp.Enabled = this.IsEnabledOptionCompPerGerarchia(row);

            CheckBox chkCC = this.GetGridItemControl<CheckBox>(row, "chkCC");
            chkCC.Enabled = this.IsEnabledOptionCCPerGerarchia(row);
        }

        /// <summary>
        /// Verifica se l'opzione conoscenza di un elemento della griglia è abilitata o meno
        /// in funzione delle regole di gerarchia delle ragioni di trasmissione 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected bool IsEnabledOptionCCPerGerarchia(GridViewRow row)
        {
            bool isEnabled = true;

            CheckBox chkCC = this.GetGridItemControl<CheckBox>(row, "chkCC");

            if (chkCC != null)
            {
                if (!string.IsNullOrEmpty(this.hdTipoDestConoscenza.Value))
                {
                    //prendo la gerarchia dell'elemento corrente presente in ultima
                    //posizione nel datagrid in ultima colonna
                    Label hdGerarchia = this.GetGridItemControl<Label>(row, "hdGerarchia");

                    isEnabled = (string.IsNullOrEmpty(hdGerarchia.Text) ||
                                 this.hdTipoDestConoscenza.Value == hdGerarchia.Text);
                }
            }

            return isEnabled;
        }

        /// <summary>
        /// Verifica se l'opzione competenza di un elemento della griglia è abilitata o meno
        /// in funzione delle regole di gerarchia delle ragioni di trasmissione
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected bool IsEnabledOptionCompPerGerarchia(GridViewRow row)
        {
            bool isEnabled = true;

            CheckBox chkComp = this.GetGridItemControl<CheckBox>(row, "chkComp");

            if (chkComp != null)
            {
                if (!string.IsNullOrEmpty(this.hdTipoDestCompetenza.Value))
                {
                    //prendo la gerarchia dell'elemento corrente presente in ultima
                    //posizione nel datagrid in ultima colonna
                    Label hdGerarchia = this.GetGridItemControl<Label>(row, "hdGerarchia");

                    isEnabled = (string.IsNullOrEmpty(hdGerarchia.Text) ||
                                 this.hdTipoDestCompetenza.Value == hdGerarchia.Text);
                }
            }

            return isEnabled;
        }

        #region Gestione navigazione UO

        /// <summary>
        /// Imposta nell'inizializzazione dei dati il campo nascosto che indica
        ///  se è attiva la funzione di navigazione UO
        /// </summary>
        private void ImpostaHiddenNavigaUO()
        {
            if (IsEnabledNavigazioneUO())
                this.hdIsEnabledNavigaUO.Value = "1";
            else
                this.hdIsEnabledNavigaUO.Value = "0";
        }

        /// <summary>
        /// Verifica se è attiva la chiave SMISTA_NAVIGA_UO
        /// </summary>
        /// <returns></returns>
        private bool IsEnabledNavigazioneUO()
        {
            SmistaDocManager docManager = new SmistaDocManager();
            return docManager.IsEnabledNavigazioneUO();
        }

        protected void grdUOApp_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            bool selezioneUoApp = false;
            bool selezioneUoInf = false;
            bool selezioneUtenteRuolo = false;
            GridViewRow row = null;
            // int index = -1;

            switch (e.CommandName)
            {
                case "navigaUO_up":
                    row = (GridViewRow)(((CustomImageButton)e.CommandSource).NamingContainer);
                    // index = Convert.ToInt32(e.CommandArgument);
                    //row = this.grdUOApp.Rows[index];
                    string id = this.GetId(row);

                    this.UpdateFlagUo(id, out selezioneUoApp, out selezioneUoInf, out selezioneUtenteRuolo);
                    this.navigaUO_up(id);
                    this.setFlagDataGrid();

                    // Impostazione id UO parent
                    this.indxUoSel = this.getUOPadre(id);
                    this.GestVisibilitaFrecceNavigaUO();

                    break;
                case "imgNote":
                    row = (GridViewRow)(((CustomImageButton)e.CommandSource).NamingContainer);
                    string type = this.grdUOApp.DataKeys[row.RowIndex].Values["type"].ToString();

                    if (type.Equals("U"))
                    {
                        this.SetRowControlsVisibility(row, false);
                    }

                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CheckBox chkComp = this.GetGridItemControl<CheckBox>(row, "chkComp");
                        CheckBox chkCC = this.GetGridItemControl<CheckBox>(row, "chkCC");
                        CheckBox chkNotifica = this.GetGridItemControl<CheckBox>(row, "chkNotifica");

                        if (!type.Equals("U"))
                        {
                            string Id = string.Empty;
                            //string[] ids = e.Row.Cells[0].Text.Split('_');
                            string[] ids = this.grdUOApp.DataKeys[row.RowIndex].Values["id"].ToString().Split('_');

                            if (type.Equals("R"))
                            {
                                Id = ids[0];
                            }
                            else if (type.Equals("P"))
                            {
                                Id = ids[1];
                            }

                            // Impostazione script client per il pulsante di inserimento dati aggiuntivi trasmissione
                            CustomImageButton imgNote = this.GetGridItemControl<CustomImageButton>(row, "imgNote");

                            this.IndexRowGrdUoApp = row.RowIndex;
                            this.IdGrdUoApp = Id;
                            this.TypeURPGrdUoApp = type;
                            this.UpdateFlagUo(this.indxUoSel, out selezioneUoApp, out selezioneUoInf, out selezioneUtenteRuolo);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "NoteSmistamento", "ajaxModalPopupNoteSmistamento();", true);

                        }

                        if (chkComp != null)
                            chkComp.ToolTip = hdDescrCompetenza.Value;

                        if (chkCC != null)
                            chkCC.ToolTip = hdDescrConoscenza.Value;
                    }

                    // gestione naviga UO
                    if (this.IsEnabledNavigazioneUO())
                    {
                        if (type.Equals("U"))
                        {
                            row.Cells[3].Visible = true;
                            if (this.hdUOapp.Value.Equals(this.grdUOApp.DataKeys[row.RowIndex].Values["id"].ToString()))
                            {
                                row.Cells[3].Text = "";
                            }
                        }
                        else
                            row.Cells[3].Text = "";
                    }
                    else
                        row.Cells[3].Visible = false;

                    break;
            }

        }


        protected void grdUOInf_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            bool selezioneUoApp = false;
            bool selezioneUoInf = false;
            bool selezioneUtenteRuolo = false;
            GridViewRow row = null;

            switch (e.CommandName)
            {
                case "navigaUO_down":
                    // int index = Convert.ToInt32(e.CommandArgument);
                    row = (GridViewRow)(((CustomImageButton)e.CommandSource).NamingContainer);
                    //row = this.grdUOInf.Rows[index];
                    string id = this.GetIdUoInf(row);

                    //devo memorizzare gli utenti/ruoli selezionati della uo padre che ho selezionato
                    SmistaDocManager docManager = this.GetSmistaDocManager();
                    DocsPaWR.UOSmistamento uoAppartenenza = docManager.GetUOAppartenenza();
                    string idUOPadre = this.getUOPadre(id);
                    DocsPaWR.UOSmistamento uoSelezionata = this.getUoSelezionata(uoAppartenenza, id);

                    CheckBox chkComp = this.GetGridItemControl<CheckBox>(row, "chkComp");
                    CheckBox chkCC = this.GetGridItemControl<CheckBox>(row, "chkCC");

                    if (uoSelezionata.FlagConoscenza.Equals(chkCC.Checked) &&
                        uoSelezionata.FlagCompetenza.Equals(chkComp.Checked))
                        uoSelezionata.Selezionata = true;
                    else
                        uoSelezionata.Selezionata = false;

                    DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
                    if (uoSelezionata.UoInferiori == null || uoSelezionata.UoInferiori.Length == 0)
                        uoSelezionata.UoInferiori = ws.GetUOInferiori(id, docManager.getMittenteSmistamento());

                    this.UpdateFlagUo(idUOPadre, out selezioneUoApp, out selezioneUoInf, out selezioneUtenteRuolo);
                    this.navigaUO_down(id);
                    this.SetFlagDestinatarioRaggiutoDaTrasm(this.grdUOApp, id);
                    this.setFlagDataGrid();
                    this.indxUoSel = id;

                    if (this.hdCountNavigaDown.Value == "0")
                    {
                        this.hdCountNavigaDown.Value = "1";
                        uoSelezionata.Selezionata = true;
                    }

                    // this.GestVisibilitaFrecceNavigaUO();

                    break;
                case "imgNote":
                    row = (GridViewRow)(((CustomImageButton)e.CommandSource).NamingContainer);
                    id = this.grdUOInf.DataKeys[row.RowIndex].Values["id"].ToString();

                    //if (type.Equals("U"))
                    //{
                    //    this.SetRowControlsVisibility(row, false);
                    //}

                    //if (row.RowType == DataControlRowType.DataRow)
                    //{
                    //    chkComp = this.GetGridItemControl<CheckBox>(row, "chkComp");
                    //    chkCC = this.GetGridItemControl<CheckBox>(row, "chkCC");
                    //    CheckBox chkNotifica = this.GetGridItemControl<CheckBox>(row, "chkNotifica");

                    //    if (!type.Equals("U"))
                    //    {
                    //        string Id = string.Empty;
                    //        //string[] ids = e.Row.Cells[0].Text.Split('_');
                    //        string[] ids = this.grdUOInf.DataKeys[row.RowIndex].Values["id"].ToString().Split('_');


                    //            Id = ids[0];


                    //        // Impostazione script client per il pulsante di inserimento dati aggiuntivi trasmissione
                    //        CustomImageButton imgNote = this.GetGridItemControl<CustomImageButton>(row, "imgNote");

                    this.IndexRowGrdUoApp = row.RowIndex;
                    this.TypeURPGrdUoApp = "U";
                    this.IdGrdUoApp = id;
                    this.UpdateFlagUo(this.indxUoSel, out selezioneUoApp, out selezioneUoInf, out selezioneUtenteRuolo);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "NoteSmistamento", "ajaxModalPopupNoteSmistamento();", true);
                    //    }                   
                    //}

                    break;
            }
        }

        private void FillUOApp_NavigaUO(string idUO)
        {

            SmistaDocManager docManager = this.GetSmistaDocManager();
            docManager.FillCurrentUO_NavigaUO(idUO, RoleManager.GetRoleInSession(), UserManager.GetUserInSession(), UserManager.GetInfoUser());

            DocsPaWR.UOSmistamento uoApp = docManager.GetUOAppartenenza();

            DocsPaWR.UOSmistamento uoSelezionata = this.getUoSelezionata(uoApp, idUO);

            if (uoSelezionata != null) // && uoSelezionata.Ruoli.Length > 0)
            {
                this.grdUOApp.Visible = true;
                this.FillDataGridUOApp(uoSelezionata);
            }
            Session["UoSelezionata"] = uoSelezionata;

        }

        private bool FillUOInf_NavigaUO(string idUO)
        {
            bool retValue = false;
            SmistaDocManager docManager = new SmistaDocManager();

            docManager.FillUOInf_NavigaUO(idUO, RoleManager.GetRoleInSession(), UserManager.GetUserInSession(), UserManager.GetInfoUser());

            DocsPaWR.UOSmistamento uoSel = (DocsPaWR.UOSmistamento)Session["UoSelezionata"];
            DocsPaWR.UOSmistamento[] uoInferiori = uoSel.UoInferiori;

            if (uoInferiori != null && uoInferiori.Length > 0)
            {

                this.grdUOInf.Visible = true;


                this.FillDataGridUOInferiori(uoInferiori);
                retValue = true;




            }
            else
            {

                this.grdUOInf.Visible = false;
            }

            return retValue;
        }

        private void navigaUO_down(string idUO)
        {
            // carica dati UO corrente
            this.FillUOApp_NavigaUO(idUO);
            // carica dati UO inferiori
            this.FillUOInf_NavigaUO(idUO);

            this.updatePanelUOAppartenenza.Update();
            this.updatePanelUOInf.Update();
        }

        private void navigaUO_up(string idUO)
        {
            // carica dati UO corrente
            string idUOPadre = this.getUOPadre(idUO);
            this.FillUOApp_NavigaUO(idUOPadre);
            // carica dati UO inferiori
            this.FillUOInf_NavigaUO(idUOPadre);

            this.updatePanelUOAppartenenza.Update();
            this.updatePanelUOInf.Update();
        }

        private string getUOPadre(string idUO)
        {
            string id_Uo_padre = string.Empty;
            SmistaDocManager docManager = new SmistaDocManager();
            ArrayList lista = new ArrayList();
            lista = docManager.ListaIDUOParent(idUO);
            if (lista.Count > 0)
                id_Uo_padre = lista[1].ToString();

            return id_Uo_padre;
        }

        private void GestVisibilitaFrecceNavigaUO()
        {
            SmistaDocManager docManager = this.GetSmistaDocManager();
            this.GestVisibilitaFrecceNavigaUOApp(docManager);
            this.GestVisibilitaFrecceNavigaUOInf(docManager);
            this.updatePanelUOAppartenenza.Update();
            this.updatePanelUOInf.Update();
        }

        private void GestVisibilitaFrecceNavigaUOApp(SmistaDocManager docManager)
        {
            if (this.grdUOApp.Visible && this.grdUOApp.Rows.Count > 0)
            {

                bool readOnly = Convert.ToInt32(DocumentInWorking.accessRights) == Convert.ToInt32(HMdiritti.HMdiritti_Read);
                for (int i = 0; i < this.grdUOApp.Rows.Count; i++)
                {
                    if (this.IsEnabledNavigazioneUO())
                    {
                        string tipo = this.GetTipoURP(this.grdUOApp.Rows[i]);

                        if (tipo.Equals("U"))
                        {
                            this.grdUOApp.Rows[i].Cells[3].Visible = true;
                            if (this.hdUOapp.Value.Equals(this.grdUOApp.DataKeys[i].Values["id"].ToString()))
                                this.grdUOApp.Rows[i].Cells[3].Text = "";
                        }
                        else
                            this.grdUOApp.Rows[i].Cells[3].Text = "";
                    }
                    else
                        this.grdUOApp.Rows[i].Cells[3].Visible = false;

                    CheckBox chkComp = this.GetGridItemControl<CheckBox>(this.grdUOApp.Rows[i], "chkComp");
                    if (readOnly && !docManager.IsTrasmissioneConWorkflow(docManager.GetCurrentDocumentPosition() - 1))
                    {
                        chkComp.Checked = false;
                        chkComp.Enabled = false;
                    }
                    else
                    {
                        chkComp.Enabled = true;
                    }
                }
            }
        }

        private void GestVisibilitaFrecceNavigaUOInf(SmistaDocManager docManager)
        {
            if (this.grdUOInf.Visible && this.grdUOInf.Rows.Count > 0)
            {
                bool readOnly = Convert.ToInt32(DocumentInWorking.accessRights) == Convert.ToInt32(HMdiritti.HMdiritti_Read);
                for (int i = 0; i < this.grdUOInf.Rows.Count; i++)
                {
                    if (!this.IsEnabledNavigazioneUO())
                        this.grdUOInf.Rows[i].Cells[3].Visible = false;

                    CheckBox chkComp = this.GetGridItemControl<CheckBox>(this.grdUOInf.Rows[i], "chkComp");
                    if (readOnly && !docManager.IsTrasmissioneConWorkflow(docManager.GetCurrentDocumentPosition() - 1))
                    {
                        chkComp.Checked = false;
                        chkComp.Enabled = false;
                    }
                    else
                    {
                        chkComp.Enabled = true;
                    }
                }
            }
        }

        private void FillGridDestinatariDefault()
        {

            SmistaDocManager docManager = new SmistaDocManager();
            docManager.FillDestinatariDefault(RoleManager.GetRoleInSession(), UserManager.GetUserInSession(), UserManager.GetInfoUser());

            DocsPaWR.UOSmistamento uoApp = docManager.GetUOAppartenenza();

            // Caricamento griglia uo di appartenenza
            if (uoApp != null) // && uoApp.Ruoli.Length > 0)
            {
                SmistaDocSessionManager.SetSessionUoApp(uoApp);
                this.grdUOApp.Visible = true;
                this.FillDataGridUOApp(uoApp);

                // Impostazione dell'indice dell'UO di appartenenza
                this.indxUoSel = uoApp.ID;
            }
            else
            {
                this.grdUOApp.Visible = false;

                this.indxUoSel = string.Empty;
            }

            // Caricamento griglia uo inferiori
            DocsPaWR.UOSmistamento[] uoInferiori = docManager.GetUOInferiori();

            if (uoInferiori != null && uoInferiori.Length > 0)
            {
                this.grdUOInf.Visible = true;




                this.FillDataGridUOInferiori(uoInferiori);



            }

        }

        /// <summary>
        /// Caricamento griglia UO destinatari
        /// </summary>
        /// <param name="uoInferiori"></param>
        private void FillDataGridUOInferiori(DocsPaWR.UOSmistamento[] uoInferiori)
        {
            DataSet ds = this.CreateGridDataSet();

            foreach (DocsPaWR.UOSmistamento uo in uoInferiori)
            {
                // verifico se esiste un ruolo di riferimento
                // se non esiste devo disabilitare lo smistamento e consentire la sola navigazione
                DocsPaWR.RuoloSmistamento[] ruoloRiferimento = uo.Ruoli.Where(e => e.RuoloRiferimento).ToArray();

                if (ruoloRiferimento != null & (ruoloRiferimento.Length > 0))
                    this.FillGridDataSet(ds, uo, false);
                else
                    this.FillGridDataSetUoInfNoRif(ds, uo, false);
            }

            this.grdUOInf.DataSource = ds;
            this.grdUOInf.DataBind();

            DisabledOrEnabledCbComFGridUo(this.grdUOInf);
            DisableSmistaNoRuoloRif(this.grdUOInf);

            this.updatePanelUOInf.Update();
        }

        /// <summary>
        /// Quando un documento è in sola lettura, non deve essere possibile smistare per competenza
        /// </summary>
        /// <param name="grid"></param>
        private void DisabledOrEnabledCbComFGridUo(GridView grid)
        {
            bool readOnly = Convert.ToInt32(DocumentInWorking.accessRights) == Convert.ToInt32(HMdiritti.HMdiritti_Read);
            SmistaDocManager docManager = this.GetSmistaDocManager();
            foreach (GridViewRow row in grid.Rows)
            {

                CheckBox chkComp = this.GetGridItemControl<CheckBox>(row, "chkComp");
                if (readOnly && !docManager.IsTrasmissioneConWorkflow(docManager.GetCurrentDocumentPosition() - 1))
                {
                    chkComp.Checked = false;
                    chkComp.Enabled = false;
                }
                else
                {
                    chkComp.Enabled = true;
                }
            }
        }

        private void SetFlagDestinatarioRaggiutoDaTrasm(GridView grid, string idUo)
        {
            SmistaDocManager docManager = new SmistaDocManager();
            List<string> idCorrRaggiunti = docManager.GetIdDestinatariTrasmDocInUo(idUo, this.DocumentInWorking.docNumber);
            if (idCorrRaggiunti != null && idCorrRaggiunti.Count > 0)
            {
                string tipo = string.Empty;
                foreach (GridViewRow row in grid.Rows)
                {
                    tipo = this.GetTipoURP(row);
                    if (tipo != "U")
                    {
                        CustomImageButton img = this.GetGridItemControl<CustomImageButton>(row, "ImgButtonTrasmesso");
                        img.Visible = IsDestRaggiuntoDaTrasm(row, tipo, idCorrRaggiunti);
                    }
                }
            }
            else
            {
                string tipo = string.Empty;
                foreach (GridViewRow row in grid.Rows)
                {
                    tipo = this.GetTipoURP(row);
                    if (tipo != "U")
                    {
                        CustomImageButton img = this.GetGridItemControl<CustomImageButton>(row, "ImgButtonTrasmesso");
                        img.Visible = false;
                    }
                }
            }
        }

        /// <summary>
        /// Disabilita i checkbox relativi alle UO senza ruolo di riferimento
        /// </summary>
        /// <param name="grid"></param>
        private void DisableSmistaNoRuoloRif(GridView grid)
        {
            foreach (GridViewRow row in grid.Rows)
            {
                Label lbl = this.GetGridItemControl<Label>(row, "hd_isAllowedSmista");

                CheckBox chkComp = this.GetGridItemControl<CheckBox>(row, "chkComp");
                CheckBox chkCC = this.GetGridItemControl<CheckBox>(row, "chkCC");

                if (lbl.Text.Equals("1"))
                {
                    chkComp.Visible = false;
                    chkCC.Visible = false;
                }
                else
                {
                    chkComp.Visible = true;
                    chkCC.Visible = true;
                }
            }
        }

        private bool existNavigaUOInf(string idUO)
        {
            bool retValue = false;

            try
            {
                int intID = Convert.ToInt32(idUO);

                SmistaDocManager docManager = new SmistaDocManager();
                retValue = docManager.ExistUoInf(idUO, RoleManager.GetRoleInSession(), UserManager.GetUserInSession(), UserManager.GetInfoUser());
            }
            catch
            {
                retValue = false;
            }

            return retValue;
        }

        private void AlertNoUOInf()
        {
            string msg = "WarningSmistaDocNoUoInf";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
        }

        private void RipristinaFrecce_UOApp()
        {
            string type = string.Empty;

            foreach (GridViewRow row in this.grdUOApp.Rows)
            {
                type = this.GetTipoURP(row);

                if (type.Equals("U"))
                {
                    row.Cells[3].Visible = true;
                    if (this.hdUOapp.Value.Equals(this.GetId(row)))
                        row.Cells[3].Text = "";
                }
                else
                    row.Cells[3].Text = "";
            }
        }

        /// <summary>
        /// Reperimento ID elemento griglia
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetId(GridViewRow row)
        {
            string tipo = this.GetTipoURP(row);

            if (tipo == "R" || tipo == "U")
                return this.grdUOApp.DataKeys[row.RowIndex].Values["id"].ToString();
            else if (tipo == "P")
                return this.grdUOApp.DataKeys[row.RowIndex].Values["id"].ToString().Split('_')[1];
            else
                return null;
        }

        /// <summary>
        /// Reperimento ID elemento griglia grdUOInf
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetIdUoInf(GridViewRow row)
        {
            string tipo = this.GetTipoURPInf(row);

            if (tipo == "R" || tipo == "U")
                return this.grdUOInf.DataKeys[row.RowIndex].Values["id"].ToString();
            else if (tipo == "P")
                return this.grdUOInf.DataKeys[row.RowIndex].Values["id"].ToString().Split('_')[1];
            else
                return null;
        }

        /// <summary>
        /// Reperimento tipo elemento griglia
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetTipoURP(GridViewRow row)
        {
            return this.grdUOApp.DataKeys[row.RowIndex].Values["type"].ToString();
            //return row.Cells[1].Text;
        }

        private string GetTipoURPInf(GridViewRow row)
        {
            return this.grdUOInf.DataKeys[row.RowIndex].Values["type"].ToString();
            //return row.Cells[1].Text;
        }

        #endregion

        private bool CheckExistenzaRagioniSmistamento(out DocsPaWR.RagioneTrasmissione[] listaRagioniSmistamento)
        {
            bool ret = false;
            SmistaDocManager docManager = new SmistaDocManager();
            listaRagioniSmistamento = docManager.GetListaRagioniSmistamento(UserManager.GetInfoUser());
            if (listaRagioniSmistamento.Length > 0)
            {
                ret = true;
                SmistaDocSessionManager.SetSessionRagTrasm();
            }
            return ret;
        }

        protected void ImgAddProjects_Click(object sender, EventArgs e)
        {
            try
            {
                SmistaDocManager docManager = this.GetSmistaDocManager();
                string TrasmId = docManager.GetIdTrasmissione(docManager.GetCurrentDocumentPosition() - 1);
                Trasmissione trasm = TrasmManager.GetTransmission(this, TrasmId, "D");
                TrasmManager.setSelectedTransmission(trasm);
                SchedaDocumento schedaDoc = UIManager.DocumentManager.getDocumentDetailsNoSecurity(this.Page, trasm.infoDocumento.docNumber, trasm.infoDocumento.docNumber);
                DocumentManager.setSelectedRecord(schedaDoc);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeAJM", "parent.closeAjaxModal('SmistamentoDocumenti','OPEN_PROJECT');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtCodeProject_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            try
            {
                if (!string.IsNullOrEmpty(this.TxtCodeProject.Text))
                {
                    this.SearchProjectT();
                    this.GestVisibilitaFrecceNavigaUO();
                }
                else
                {
                    this.IdProject.Value = string.Empty;
                    this.TxtCodeProject.Text = string.Empty;
                    this.TxtDescriptionProject.Text = string.Empty;
                }
                cercaClassificazioneDaCodice(this.TxtCodeProject.Text);
                this.UpPnlProject.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        private bool cercaClassificazioneDaCodice(string codClassificazione)
        {
            bool res = false;
            DocsPaWR.Fascicolo[] listaFasc;
            if (!string.IsNullOrEmpty(codClassificazione))
            {
                listaFasc = this.getFascicolo(RoleManager.GetRoleInSession().registri[0], codClassificazione);

                DocsPaWR.FascicolazioneClassificazione[] FascClass = ProjectManager.fascicolazioneGetTitolario2(this, codClassificazione, false, this.getIdTitolario(codClassificazione, listaFasc));
                if (FascClass != null && FascClass.Length != 0)
                {
                    HttpContext.Current.Session["classification"] = FascClass[0];
                }
                else
                {
                    HttpContext.Current.Session["classification"] = null;
                }
            }

            return res;
        }

        private DocsPaWR.Fascicolo[] getFascicolo(DocsPaWR.Registro registro, string codClassificazione)
        {
            DocsPaWR.Fascicolo[] listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codClassificazione, registro, "I");
            return listaFasc;
        }

        private string getIdTitolario(string codClassificazione, DocsPaWR.Fascicolo[] listaFasc)
        {
            if (listaFasc != null && listaFasc.Length > 0)
            {
                DocsPaWR.Fascicolo fasc = (DocsPaWR.Fascicolo)listaFasc[0];
                return fasc.idTitolario;
            }
            else
            {
                return null;
            }
        }
        private void SearchProjectT()
        {
            try
            {
                string idAmministrazione = UIManager.UserManager.GetUserInSession().idAmministrazione;
                string idTitolario = UIManager.ClassificationSchemeManager.getTitolarioAttivo(idAmministrazione).ID;
                Registro registro = UIManager.RegistryManager.GetRegistryInSession();// .getRegistroBySistemId(projectDdlRegistro.SelectedValue);
                if (this.TxtCodeProject.Text.IndexOf("//") > -1)
                {
                    #region FASCICOLAZIONE IN SOTTOFASCICOLI
                    string codice = string.Empty;
                    string descrizione = string.Empty;
                    DocsPaWR.Fascicolo SottoFascicolo = getFolder(this.Registry, ref codice, ref descrizione);
                    if (SottoFascicolo != null)
                    {

                        if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
                        {
                            TxtDescriptionProject.Text = descrizione;
                            TxtCodeProject.Text = codice;
                            this.IdProject.Value = SottoFascicolo.systemID;
                            ProjectManager.setFascicoloSelezionatoFascRapida(SottoFascicolo);
                        }
                        else
                        {

                            //string msg = @"Attenzione, sottofascicolo non presente.";
                            string msg = "WarningDocumentSubFileNoFound";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            this.TxtDescriptionProject.Text = string.Empty;
                            this.TxtCodeProject.Text = string.Empty;
                            this.IdProject.Value = string.Empty;
                        }
                    }
                    else
                    {
                        //string msg = @"Attenzione, sottofascicolo non presente.";
                        string msg = "WarningDocumentSubFileNoFound";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        this.TxtDescriptionProject.Text = string.Empty;
                        this.TxtCodeProject.Text = string.Empty;
                        this.IdProject.Value = string.Empty;

                    }

                    #endregion
                }
                else
                {
                    Fascicolo projectList = UIManager.ProjectManager.GetProjectByCode(registro, TxtCodeProject.Text);
                    //Fascicolo projectList = UIManager.ProjectManager.GetProjectByCodeRedAndClassScheme(registro, TxtCodeProject.Text, idTitolario);
                    if (projectList != null && projectList.tipo == "G" && projectList.idTitolario != idTitolario)
                    {
                        projectList = null;
                        projectList = UIManager.ProjectManager.GetProjectByCodeRedAndClassScheme(registro, TxtCodeProject.Text, idTitolario);
                    }
                    
                    if (projectList == null)
                    {
                        this.IdProject.Value = string.Empty;
                        this.TxtCodeProject.Text = string.Empty;
                        this.TxtDescriptionProject.Text = string.Empty;
                        string msg = "WarningProjectNotFound";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                    }
                    //EMANUELA 19-01-2015:AGGIUNTO CONTROLLO FASCICOLO CHIUSO
                    if (projectList != null && !string.IsNullOrEmpty(projectList.stato) && projectList.stato.Equals("C"))
                    {
                        //string msg = @"Attenzione, il fascicolo scelto è chiuso. Pertanto il documento non può essere inserito nel fascicolo selezionato.";
                        this.IdProject.Value = string.Empty;
                        this.TxtCodeProject.Text = string.Empty;
                        this.TxtDescriptionProject.Text = string.Empty;
                        string msg = "WarningDocumentFileNoOpen";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                    else
                    {
                        this.IdProject.Value = projectList.systemID;
                        this.TxtCodeProject.Text = projectList.codice;
                        this.TxtDescriptionProject.Text = projectList.descrizione;
                        ProjectManager.setFascicoloSelezionatoFascRapida(projectList);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private DocsPaWR.Fascicolo[] getFascicolo(DocsPaWR.Registro registro)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            if (!string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                string codiceFascicolo = TxtCodeProject.Text;
                listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "I");
            }
            if (listaFasc != null)
            {
                return listaFasc;
            }
            else
            {
                return null;
            }
        }

        protected void SearchProjectNoRegistro()
        {

            this.TxtDescriptionProject.Text = string.Empty;
            if (string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                this.TxtDescriptionProject.Text = string.Empty;
                return;
            }
            //su DocProfilo devo cercare senza condizione sul registro.
            //Basta che il fascicolo sia visibile al ruolo loggato

            if (this.TxtCodeProject.Text.IndexOf("//") > -1)
            {

                string codice = string.Empty;
                string descrizione = string.Empty;

                DocsPaWR.Fascicolo SottoFascicolo = getFolder(null, ref codice, ref descrizione);
                if (SottoFascicolo != null)
                {
                    if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
                    {
                        TxtDescriptionProject.Text = descrizione;
                        TxtCodeProject.Text = codice;
                        this.Project = SottoFascicolo;
                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, SottoFascicolo.idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                        // DocsPaWR.Fascicolo fascForRicFasc = ProjectManager.getFascicoloById(gerClassifica[gerClassifica.Length - 1].systemId);
                        ProjectManager.setProjectInSessionForRicFasc(gerClassifica[gerClassifica.Length - 1].codice);
                        ProjectManager.setFascicoloSelezionatoFascRapida(this, SottoFascicolo);

                    }
                    else
                    {
                        //string msg = @"Attenzione, sottofascicolo non presente.";
                        string msg = "WarningDocumentSubFileNoFound";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                        this.TxtDescriptionProject.Text = string.Empty;
                        this.TxtCodeProject.Text = string.Empty;

                    }
                }
                else
                {
                    //string msg = @"Attenzione, sottofascicolo non presente.";
                    string msg = "WarningDocumentSubFileNoFound";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    this.TxtDescriptionProject.Text = string.Empty;
                    this.TxtCodeProject.Text = string.Empty;
                }
            }
            else
            {

                DocsPaWR.Fascicolo[] listaFasc = getFascicolo(this.Registry);
                string codClassifica = string.Empty;
                if (listaFasc != null)
                {
                    if (listaFasc.Length > 0)
                    {
                        //caso 1: al codice digitato corrisponde un solo fascicolo
                        if (listaFasc.Length == 1)
                        {
                            this.TxtDescriptionProject.Text = listaFasc[0].descrizione;
                            //metto il fascicolo in sessione
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                codClassifica = listaFasc[0].codice;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);

                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }
                            this.Project = listaFasc[0];
                            ProjectManager.setProjectInSessionForRicFasc(codClassifica);
                            ProjectManager.setFascicoloSelezionatoFascRapida(this, listaFasc[0]);
                            //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                            //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'N');");
                        }
                        else
                        {
                            codClassifica = this.TxtCodeProject.Text;
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                //codClassifica = codClassifica;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }
                            //Page.RegisterStartupScript("openListaFasc","<SCRIPT>ApriSceltaFascicolo();</SCRIPT>");
                            //Session.Add("hasRegistriNodi",hasRegistriNodi);

                            //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                            //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'Y');");

                            //Da Fare
                            //RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli2('" + codClassifica + "', 'Y')</script>");                            

                            return;
                        }
                    }
                    else
                    {
                        //caso 0: al codice digitato non corrisponde alcun fascicolo
                        if (listaFasc.Length == 0)
                        {
                            //Provo il caso in cui il fascicolo è chiuso
                            Fascicolo chiusoFasc = ProjectManager.getFascicoloDaCodice(this.Page, this.TxtCodeProject.Text);
                            if (chiusoFasc != null && !string.IsNullOrEmpty(chiusoFasc.stato) && chiusoFasc.stato.Equals("C"))
                            {
                                //string msg = @"Attenzione, il fascicolo scelto è chiuso. Pertanto il documento non può essere inserito nel fascicolo selezionato.";
                                string msg = "WarningDocumentFileNoOpen";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            else
                            {
                                //string msg = @"Attenzione, codice fascicolo non presente.";
                                string msg = "WarningDocumentCodFileNoFound";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                            }

                            this.TxtDescriptionProject.Text = string.Empty;
                            this.TxtCodeProject.Text = string.Empty;
                        }
                        //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                        //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', '');");
                    }
                }
            }
        }

        private DocsPaWR.Fascicolo getFolder(DocsPaWR.Registro registro, ref string codice, ref string descrizione)
        {
            DocsPaWR.Folder[] listaFolder = null;
            DocsPaWR.Fascicolo fasc = null;
            string separatore = "//";
            int posSep = this.TxtCodeProject.Text.IndexOf("//");
            if (this.TxtCodeProject.Text != string.Empty && posSep > -1)
            {

                string codiceFascicolo = TxtCodeProject.Text.Substring(0, posSep);
                string descrFolder = TxtCodeProject.Text.Substring(posSep + separatore.Length);

                listaFolder = ProjectManager.getListaFolderDaCodiceFascicolo(this, codiceFascicolo, descrFolder, registro);
                if (listaFolder != null && listaFolder.Length > 0)
                {
                    //calcolo fascicolazionerapida
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    fasc = ProjectManager.getFascicoloById(listaFolder[0].idFascicolo, infoUtente);

                    if (fasc != null)
                    {
                        //folder selezionato è l'ultimo
                        fasc.folderSelezionato = listaFolder[listaFolder.Length - 1];
                    }
                    codice = fasc.codice + separatore;
                    descrizione = fasc.descrizione + separatore;
                    for (int i = 0; i < listaFolder.Length; i++)
                    {
                        codice += listaFolder[i].descrizione + "/";
                        descrizione += listaFolder[i].descrizione + "/";
                    }
                    codice = codice.Substring(0, codice.Length - 1);
                    descrizione = descrizione.Substring(0, descrizione.Length - 1);

                }
            }
            if (fasc != null)
            {

                return fasc;

            }
            else
            {
                return null;
            }
        }

        private DocsPaWR.Fascicolo[] getFascicoli(DocsPaWR.Registro registro)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            if (!this.TxtCodeProject.Text.Equals(""))
            {
                string codiceFascicolo = TxtCodeProject.Text;
                listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "I");
            }
            if (listaFasc != null)
            {
                return listaFasc;
            }
            else
            {
                return null;
            }
        }

        protected void SearchProjectRegistro()
        {
            this.TxtDescriptionProject.Text = string.Empty;
            string codClassifica = string.Empty;

            if (string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                this.TxtDescriptionProject.Text = string.Empty;
                this.Project = null;
                ProjectManager.setProjectInSessionForRicFasc(null);
                ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                return;
            }

            //FASCICOLAZIONE IN SOTTOFASCICOLI

            if (this.TxtCodeProject.Text.IndexOf("//") > -1)
            {
                #region FASCICOLAZIONE IN SOTTOFASCICOLI
                string codice = string.Empty;
                string descrizione = string.Empty;
                DocsPaWR.Fascicolo SottoFascicolo = getFolder(this.Registry, ref codice, ref descrizione);
                if (SottoFascicolo != null)
                {

                    if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
                    {
                        TxtDescriptionProject.Text = descrizione;
                        TxtCodeProject.Text = codice;
                        this.Project = SottoFascicolo;
                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, SottoFascicolo.idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                        ProjectManager.setProjectInSessionForRicFasc(gerClassifica[gerClassifica.Length - 1].codice);
                        ProjectManager.setFascicoloSelezionatoFascRapida(this, SottoFascicolo);
                    }
                    else
                    {

                        //string msg = @"Attenzione, sottofascicolo non presente.";
                        string msg = "WarningDocumentSubFileNoFound";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        this.TxtDescriptionProject.Text = string.Empty;
                        this.TxtCodeProject.Text = string.Empty;
                        this.Project = null;
                        ProjectManager.setProjectInSessionForRicFasc(null);
                        ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                    }
                }
                else
                {
                    Session["validCodeFasc"] = "false";

                    //string msg = @"Attenzione, sottofascicolo non presente.";
                    string msg = "WarningDocumentSubFileNoFound";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    this.TxtDescriptionProject.Text = string.Empty;
                    this.TxtCodeProject.Text = string.Empty;
                    this.Project = null;
                    ProjectManager.setProjectInSessionForRicFasc(null);
                    ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                }

                #endregion
            }
            else
            {
                DocsPaWR.Fascicolo[] listaFasc = getFascicoli(this.Registry);

                if (listaFasc != null)
                {
                    if (listaFasc.Length > 0)
                    {
                        //caso 1: al codice digitato corrisponde un solo fascicolo
                        if (listaFasc.Length == 1)
                        {
                            this.Project = listaFasc[0];
                            this.TxtDescriptionProject.Text = listaFasc[0].descrizione;
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                codClassifica = listaFasc[0].codice;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }
                            ProjectManager.setProjectInSessionForRicFasc(codClassifica);
                            ProjectManager.setFascicoloSelezionatoFascRapida(this, listaFasc[0]);
                        }
                        else
                        {
                            //caso 2: al codice digitato corrispondono piu fascicoli
                            codClassifica = this.TxtCodeProject.Text;
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                //codClassifica = codClassifica;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }

                            ////Da Fare
                            //RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codClassifica + "', 'Y')</script>");
                            return;
                        }
                    }
                    else
                    {
                        //caso 0: al codice digitato non corrisponde alcun fascicolo
                        if (listaFasc.Length == 0)
                        {
                            //Provo il caso in cui il fascicolo è chiuso
                            Fascicolo chiusoFasc = ProjectManager.getFascicoloDaCodice(this.Page, this.TxtCodeProject.Text);
                            if (chiusoFasc != null && !string.IsNullOrEmpty(chiusoFasc.stato) && chiusoFasc.stato.Equals("C"))
                            {
                                //string msg = @"Attenzione, il fascicolo scelto è chiuso. Pertanto il documento non può essere inserito nel fascicolo selezionato.";
                                string msg = "WarningDocumentFileNoOpen";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            else
                            {
                                //string msg = @"Attenzione, codice fascicolo non presente.";
                                string msg = "WarningDocumentCodFileNoFound";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            this.TxtDescriptionProject.Text = string.Empty;
                            this.TxtCodeProject.Text = string.Empty;
                            this.Project = null;
                            ProjectManager.setProjectInSessionForRicFasc(null);
                            ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handler evento di selezione delle opzioni delle uo inferiori
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OpzioniUoInferiori_CheckedChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            this.PerformActionCheckOpzioniUoInferiori((CheckBox)sender);
            //Lnr 11/06/2013
            // this.GestVisibilitaFrecceNavigaUO();
        }

        /// <summary>
        /// Logica di gestione dell'azione di selezione di uno dei CheckBox relativi alle UO inferiori
        /// </summary>
        /// <param name="sender"></param>
        protected void PerformActionCheckOpzioniUoInferiori(CheckBox sender)
        {
            // Ricerca del container di tipo DataGridItem
            Control parent = sender.Parent;
            do
            {
                parent = parent.Parent;
            }
            while (parent != null && parent.GetType() != typeof(GridViewRow));

            GridViewRow row = (GridViewRow)parent;

            CheckBox chkComp = this.GetGridItemControl<CheckBox>(row, "chkComp");
            CheckBox chkCC = this.GetGridItemControl<CheckBox>(row, "chkCC");
            CustomImageButton imgNote = this.GetGridItemControl<CustomImageButton>(row, "imgNote");
            imgNote.Visible = (chkComp.Checked || chkCC.Checked);

            if (sender == chkComp)
            {
                if (chkComp.Checked)
                    chkCC.Checked = false;
            }
            else if (sender == chkCC)
            {
                if (chkCC.Checked)
                    chkComp.Checked = false;
            }
        }

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            this.GestVisibilitaFrecceNavigaUO();
            this.EnableButtonDettagliFirma();
            this.upPnlNote.Update();
            this.upPnlInfoDocument.Update();
            this.UpPnlCheckDoc.Update();
            this.UpPnlButtons.Update();

        }

        /// <summary>
        /// Handler evento di selezione delle opzioni del ruolo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OpzioniUoAppartenenza_CheckedChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            this.PerformActionCheckOpzioniUoAppartenenza((CheckBox)sender);
            //Lnr 11/06/2013
            //this.GestVisibilitaFrecceNavigaUO();
        }

        protected void PerformActionCheckOpzioniUoAppartenenza(CheckBox sender)
        {
            // Ricerca del container di tipo DataGridItem
            Control parent = sender.Parent;

            do
            {
                parent = parent.Parent;
            }
            while (parent != null && parent.GetType() != typeof(GridViewRow));

            GridViewRow row = (GridViewRow)parent;

            CheckBox chkComp = this.GetGridItemControl<CheckBox>(row, "chkComp");
            CheckBox chkCC = this.GetGridItemControl<CheckBox>(row, "chkCC");
            CheckBox chkNotifica = this.GetGridItemControl<CheckBox>(row, "chkNotifica");
            CustomImageButton imgNote = this.GetGridItemControl<CustomImageButton>(row, "imgNote");

            // Indica, se true, che il sender dell'evento è il checkBox di selezione tutti / nessuno 
            // del checkbox di notifica agli utenti del ruolo
            bool performSelectNotificaTutti = false;

            if (sender == chkComp)
            {
                if (chkComp.Checked)
                    chkCC.Checked = false;
            }
            else if (sender == chkCC)
            {
                if (chkCC.Checked)
                    chkComp.Checked = false;
            }
            else if (sender == chkNotifica)
            {
                // Il sender è il flag di seleziona / deseleziona tutti
                performSelectNotificaTutti = true;
            }

            // Se si è effettuata una selezione tra comp, cc e notifica di un utente o ruolo,
            // disabilita automaticamente le selezioni dell'uo di appartenenza
            MarkUoAsChanged(this.GetId(row), this.GetSmistaDocManager().GetUOAppartenenza());

            this.RefreshItemUoAppartenenza(row, performSelectNotificaTutti);

        }

        /// <summary>
        /// Rimuove le selezioni per competenza / conoscenza dell'uo di appartenenza
        /// dell'id utente o ruolo 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uo"></param>
        private bool MarkUoAsChanged(string id, DocsPaWR.UOSmistamento uo)
        {
            bool found = false;

            foreach (DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
            {
                if (ruolo.Utenti.Count(e => e.ID == id) > 0)
                {
                    uo.FlagCompetenza = false;
                    uo.FlagConoscenza = false;

                    found = true;

                }

                if (!found)
                {
                    if (uo.UoInferiori != null)
                    {
                        // Ricerca id tra le uo inferiori
                        foreach (DocsPaWR.UOSmistamento uoInferiore in uo.UoInferiori)
                        {
                            found = MarkUoAsChanged(id, uoInferiore);

                            if (found)
                            {
                                uoInferiore.FlagCompetenza = false;
                                uoInferiore.FlagConoscenza = false;
                                //    break;
                            }
                        }
                    }
                }
            }

            return found;
        }

        protected void CloseMask(bool withReturnValue)
        {
            string retValue = string.Empty;
            if (withReturnValue) retValue = "true";

            //ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('Visibility', '" + retValue + "');} else {parent.closeAjaxModal('SmistamentoDocumenti', '" + retValue + "');};", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAJM", "parent.closeAjaxModal('SmistamentoDocumenti', '" + retValue + "');", true);
        }

        protected void SmistaDocClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            try
            {
                this.CloseMask(false);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void chk_showDoc_CheckedChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            this.ShowDocumentFile(false);
            this.UpPnlViewer.Update();
            // gestione naviga UO
            this.GestVisibilitaFrecceNavigaUO();
        }

        /// <summary>
        /// Visualizzazione file
        /// </summary>
        private void ShowDocumentFile(bool content)
        {
            SmistaDocManager docManager = this.GetSmistaDocManager();
            DocsPaWR.DocumentoSmistamento docSmistamento = docManager.GetCurrentDocument(content);
            if (this.DocumentInWorking == null || this.DocumentInWorking.systemId != docSmistamento.IDDocumento)
            {
                // DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDettaglioDocumentoNoDataVista(this, docSmistamento.IDDocumento, docSmistamento.DocNumber);
                DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentListVersions(this, docSmistamento.IDDocumento, docSmistamento.DocNumber);
                DocumentManager.SetDataVistaSP(schedaDocumento.systemId);

                UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                this.DocumentInWorking = schedaDocumento;
                FileManager.setSelectedFile(schedaDocumento.documenti[0]);
                DocumentManager.setSelectedRecord(schedaDocumento);
            }
            if (!this.chk_showDoc.Checked)
            {
                this.BtnSmistamentoZoom.Enabled = false;
                this.IsZoom = false;
                Session["nodoc"] = "0";
                this.ViewDocument.ShowDocumentAcquired(false);
            }
            else
            {
                // Gestione abilitazione / disabilitazione pulsante dettagli firma
                this.EnableButtonDettagliFirma();

                if (docSmistamento.ImmagineDocumento != null)
                {
                    this.BtnSmistamentoZoom.Enabled = true;

                    //if ((System.Configuration.ConfigurationManager.AppSettings["VIS_UNIFICATA"] != null) && (System.Configuration.ConfigurationManager.AppSettings["VIS_UNIFICATA"] == "1"))
                    //{
                    //    Session["nodoc"] = "1";
                    //    this.ViewDocument.InitializeContent();
                    //}
                    this.ViewDocument.ShowDocumentAcquired(true);
                    this.chk_showDoc.Checked = true;
                    this.BtnSmistamentoZoom.Enabled = true;
                    this.UpPnlButtons.Update();
                    this.UpPnlCheckDoc.Update();
                }
                else
                {
                    this.BtnSmistamentoZoom.Enabled = false;
                    this.IsZoom = false;
                    Session["nodoc"] = "0";
                    this.ViewDocument.ShowDocumentAcquired(false);
                }


            }
        }



        //protected void btnclassificationschema_Click(object sender, ImageClickEventArgs e)
        //{
        //    try
        //    {
        //        this.Provenienza = "SMISTADOC";
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenTitolarioFromSmista", "ajaxModalPopupOpenTitolarioFromSmista();", true);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        UIManager.AdministrationManager.DiagnosticError(ex);
        //        return;
        //    }
        //}

        protected void DocumentImgSearchProjects_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // this.Provenienza = "SMISTADOC";
                this.DocumentInWorkingForSearchFolder = this.DocumentInWorking;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "SearchProject", "ajaxModalPopupSearchProject();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ReadRetValueFromPopupZoom()
        {
            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_PANEL_BUTTONS))
            {
                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_ZOOM)))
                {
                    RemoveIsZoom();
                    return;
                }
            }
        }

        private void RemoveIsZoom()
        {
            HttpContext.Current.Session.Remove("isZoom");
        }

        protected void grdUOInf_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void BtnSmistamentoClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideIframe", "$('iframe').hide();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "closeAJM", "parent.closeAjaxModal('SmistamentoDocumenti','up');", true);
        }

        protected void btnProjectPostback_Click(object sender, EventArgs e)
        {

        }

        protected void btnNoteSmistamento_Click(object sender, EventArgs e)
        {

        }

        protected void BtnSmistamentoZoom_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            this.IsZoom = true;
            SmistaDocManager docManager = this.GetSmistaDocManager();
            DocsPaWR.DocumentoSmistamento docCorrente = docManager.GetCurrentDocument(false);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDocumentViewer", "parent.ajaxModalPopupDocumentViewer();", true);
            NttDataWA.Popup.DocumentViewer.OpenDocumentViewer = true;
        }

        protected void BtnSmistamentoAdlU_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            try
            {
                int isInAdl = DocumentManager.isDocInADL(this.DocumentInWorking.systemId, this);
                string msg = string.Empty;
                string language = UIManager.UserManager.GetUserLanguage();
                if (isInAdl == 1)
                {
                    DocumentManager.eliminaDaAreaLavoro(this.Page, this.DocumentInWorking.systemId, null);
                    this.BtnSmistamentoAdlU.Text = Utils.Languages.GetLabelFromCode("DocumentBtnAdL", language);
                    //msg = "Documento rimosso dall'Area di Lavoro";
                    msg = "CheckDocumentRemoveAdL";
                }
                else
                {
                    DocumentManager.addAreaLavoro(this, DocumentInWorking);
                    this.BtnSmistamentoAdlU.Text = Utils.Languages.GetLabelFromCode("DocumentBtnAdLRemove", language);
                    //msg = "Documento aggiunto all'Area di Lavoro";
                    msg = "CheckDocumentAddAdL";
                }

                this.FascicolaDocumento();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'check', '')", true);
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_aggiunto", "alert(\"" + msg + "\");", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void BtnSmistamentoAdlR_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            try
            {
                int isInAdl = DocumentManager.isDocInADLRole(this.DocumentInWorking.systemId, this);
                string msg = string.Empty;
                string language = UIManager.UserManager.GetUserLanguage();
                if (isInAdl == 1)
                {
                    DocumentManager.eliminaDaAreaLavoroRole(this.Page, this.DocumentInWorking.systemId, null);
                    this.BtnSmistamentoAdlR.Text = Utils.Languages.GetLabelFromCode("DocumentBtnAdLRole", language);
                    this.BtnSmistamentoAdlU.Enabled = true;
                    msg = "CheckDocumentRemoveAdL";
                    this.BtnSmistamentoAdlU.Text = Utils.Languages.GetLabelFromCode("DocumentBtnAdL", language);
                }
                else
                {
                    DocumentManager.addAreaLavoroRole(this, DocumentInWorking);
                    this.BtnSmistamentoAdlR.Text = Utils.Languages.GetLabelFromCode("DocumentBtnAdLRoleRemove", language);
                    this.BtnSmistamentoAdlU.Text = Utils.Languages.GetLabelFromCode("SmistaDocumentBtnAdL", language);
                    this.BtnSmistamentoAdlU.Enabled = false;
                    msg = "CheckDocumentAddAdL";
                }

                this.FascicolaDocumento();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'check', '')", true);
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_aggiunto", "alert(\"" + msg + "\");", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private bool accettaRifiuta(DocsPaWR.TrasmissioneTipoRisposta tipoRisp)
        {
            bool rtn = true;
            string errore = string.Empty;
            DocsPaWR.TrasmissioneSingola trasmSing = null;
            DocsPaWR.TrasmissioneUtente trasmUtente = null;
            SmistaDocManager docManager = this.GetSmistaDocManager();
            DocsPaWR.DocumentoSmistamento infoDocumento = docManager.GetCurrentDocument(false);
            string TrasmId = docManager.GetIdTrasmissione(docManager.GetCurrentDocumentPosition() - 1);
            DocsPaWR.Trasmissione trasmissione = TrasmManager.GetTransmission(this, TrasmId, "D");
            string TramUtenteId = docManager.GetIdTrasmissioneUtente(docManager.GetCurrentDocumentPosition() - 1);
            string TrasmSingId = docManager.GetIdTrasmissioneSingola(docManager.GetCurrentDocumentPosition() - 1);

            // Reperimento dello stato della trasmissione corrente
            // per impedire di rifiutare il documento qualora sia già stato accettato / rifiutato
            DocsPaWR.StatoTrasmissioneUtente statoTrasmissione = docManager.GetStatoTrasmissioneCorrente();

            if (statoTrasmissione.Accettata || statoTrasmissione.Rifiutata)
            {
                string msg = "";
                if (statoTrasmissione.Rifiutata)
                    msg = "WarningSmistaDocRejectTrasm";
                else
                    msg = "WarningSmistaDocAcceptTrasm";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                return false;
            }

            //Remove note correnti
            if (Session["datiAggiuntivi"] != null)
            {

                DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = (DocsPaWR.datiAggiuntiviSmistamento[])Session["datiAggiuntivi"];

                foreach (DocsPaWR.datiAggiuntiviSmistamento appoggioDatiAggiuntivi in datiAggiuntivi)
                {
                    appoggioDatiAggiuntivi.NoteIndividuali = "";
                    appoggioDatiAggiuntivi.dtaScadenza = "";
                    appoggioDatiAggiuntivi.tipoTrasm = "";
                }
            }

            if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
            {
                if (string.IsNullOrEmpty(this.ReturnValueRejectTransm.ToString().Trim()))
                {
                    string msg = "ErrorTransmissionNoteReject";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', 'Accettazione');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', 'Accettazione');}", true);

                    return false;
                }

             
            }


            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
            System.Collections.Generic.List<DocsPaWR.TrasmissioneSingola> list = new System.Collections.Generic.List<TrasmissioneSingola>(trasmissione.trasmissioniSingole);
            List<DocsPaWR.TrasmissioneSingola> trasmSingoleUtente = list.Where(e => e.systemId == TrasmSingId).ToList();
            DocsPaWR.Utente utenteCorrente = (DocsPaWR.Utente)UserManager.getCorrispondenteByIdPeople(this, infoUtente.idPeople, AddressbookTipoUtente.INTERNO);
            trasmSing = trasmSingoleUtente.Where(e => e.systemId == TrasmSingId).FirstOrDefault();

            //if (trasmSing == null)
            //{
            //    // Se non è stata trovata la trasmissione come destinatario ad utente, 
            //    // cerca quella con destinatario ruolo corrente dell'utente
            //    trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.RUOLO).ToList();
            //    trasmSing = TrasmManager.RoleTransmissionWithHistoricized(trasmSingoleUtente, infoUtente.idCorrGlobali);
            //}

            if (trasmSing != null)
            {
                //Nel caso di ragione con classificazione obbligatoria verifico che sia stato inserito il fascicolo
                if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.ACCETTAZIONE && trasmSing.ragione != null && trasmSing.ragione.fascicolazioneObbligatoria)
                {
                    string msg = string.Empty;
                    if (string.IsNullOrEmpty(this.TxtCodeProject.Text))
                    {
                        msg = "WarningDocumentRequestProject";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return false;
                    }
                    DocsPaWR.Fascicolo fasc = this.Project;
                    if (fasc != null && fasc.systemID != null && this.EnableBlockClassification)
                    {
                        if (fasc.tipo.Equals("G") && fasc.isFascConsentita != null && fasc.isFascConsentita == "0")
                        {
                            string msgDesc = fasc.isFascicolazioneConsentita ? "WarningDocumentNoDocumentInsert" : "WarningDocumentNoDocumentInsertClassification";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return false;
                        }
                        if (fasc.tipo.Equals("P") && !fasc.isFascicolazioneConsentita)
                        {
                            string msgDesc = "WarningDocumentNoDocumentInsertFolder";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return false;
                        }
                    }
                }
                //MODIFICA PRE PROBLEMA ACCETTAZIONE TRASMISSIONE INVIATA SIA A RUOLO CHE UTENTE
                trasmUtente = trasmSing.trasmissioneUtente.Where(e => e.systemId == TramUtenteId).FirstOrDefault();

                //data Accettazione /Rifiuto
                if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
                {
                    trasmUtente.dataRifiutata = dateformat.getDataOdiernaDocspa(); //getDataCorrente();
                    trasmUtente.noteRifiuto = this.ReturnValueRejectTransm.ToString().Trim();
                }
                else
                {
                    trasmUtente.dataAccettata = dateformat.getDataOdiernaDocspa(); //getDataCorrente();
                }

                //tipoRisposta
                trasmUtente.tipoRisposta = tipoRisp;

                if (!TrasmManager.executeAccRif(this, trasmUtente, TrasmId, null, out errore))
                {
                    rtn = false;
                    trasmUtente.dataRifiutata = null;
                    trasmUtente.dataAccettata = null;

                    string msg = "ErrorCustom";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '', '" + errore.Replace("'", @"\'") + "');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '', '" + errore.Replace("'", @"\'") + "');}", true);
                }
                else
                {
                    this.FascicolaDocumento();
                    this.verificaDiagrammiDiStato();
                    this.ProgressStatus();
                }

            }

            this.GestVisibilitaFrecceNavigaUO();

            ReturnValueRejectTransm = string.Empty;

            return rtn;

        }

        protected void BtnSmistamentoAccept_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            SmistaDocManager docManager = this.GetSmistaDocManager();
            if (docManager.IsTrasmissioneConWorkflow(docManager.GetCurrentDocumentPosition() - 1))
            {
                this.accettaRifiuta(TrasmissioneTipoRisposta.ACCETTAZIONE);
            }
            else
            {
                DocsPaWR.InfoDocumento infoDocumento = DocumentManager.getInfoDocumento(DocumentInWorking);

                if (TrasmManager.setdatavistaSP_TV(UserManager.GetInfoUser(), infoDocumento.docNumber, "D", infoDocumento.idRegistro, docManager.GetIdTrasmissione(docManager.GetCurrentDocumentPosition() - 1)))
                {
                    if (this.BtnSmistamentoAccept.Text.ToUpper().Equals("ACCETTA"))
                    {
                        this.verificaDiagrammiDiStato();
                    }
                    this.FascicolaDocumento();
                    this.ProgressStatus();
                }
                this.GestVisibilitaFrecceNavigaUO();
            }


            // Gestione del mantenimento dei flag selezionati e dei tasti di default
            Session.Add("NoteGenSmista", this.txtNoteGen.Text);
            //this.ChkMantieniSelezione();
            this.txtNoteGen.Text = Session["NoteGenSmista"].ToString();
            Session.Remove("NoteGenSmista");
        }

        protected void BtnSmistamentoAcceptLF_Click(object sender, System.EventArgs e)
        {
            bool rtn = true;
            string errore = string.Empty;
            DocsPaWR.TrasmissioneSingola trasmSing = null;
            DocsPaWR.TrasmissioneUtente trasmUtente = null;
            SmistaDocManager docManager = this.GetSmistaDocManager();
            DocsPaWR.DocumentoSmistamento infoDocumento = docManager.GetCurrentDocument(false);
            string TrasmId = docManager.GetIdTrasmissione(docManager.GetCurrentDocumentPosition() - 1);
            DocsPaWR.Trasmissione trasmissione = TrasmManager.GetTransmission(this, TrasmId, "D");
            string TramUtenteId = docManager.GetIdTrasmissioneUtente(docManager.GetCurrentDocumentPosition() - 1);
            string TrasmSingId = docManager.GetIdTrasmissioneSingola(docManager.GetCurrentDocumentPosition() - 1);
            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
            System.Collections.Generic.List<DocsPaWR.TrasmissioneSingola> list = new System.Collections.Generic.List<TrasmissioneSingola>(trasmissione.trasmissioniSingole);
            List<DocsPaWR.TrasmissioneSingola> trasmSingoleUtente = list.Where(i => i.systemId == TrasmSingId).ToList();
            DocsPaWR.Utente utenteCorrente = (DocsPaWR.Utente)UserManager.getCorrispondenteByIdPeople(this, infoUtente.idPeople, AddressbookTipoUtente.INTERNO);
            trasmSing = trasmSingoleUtente.Where(i => i.systemId == TrasmSingId).FirstOrDefault();

            // Reperimento dello stato della trasmissione corrente
            // per impedire di rifiutare il documento qualora sia già stato accettato / rifiutato
            DocsPaWR.StatoTrasmissioneUtente statoTrasmissione = docManager.GetStatoTrasmissioneCorrente();

            if (statoTrasmissione.Accettata || statoTrasmissione.Rifiutata)
            {
                string msg = "";
                if (statoTrasmissione.Rifiutata)
                    msg = "WarningSmistaDocRejectTrasm";
                else
                    msg = "WarningSmistaDocAcceptTrasm";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
            }

            //Remove note correnti
            if (Session["datiAggiuntivi"] != null)
            {

                DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = (DocsPaWR.datiAggiuntiviSmistamento[])Session["datiAggiuntivi"];

                foreach (DocsPaWR.datiAggiuntiviSmistamento appoggioDatiAggiuntivi in datiAggiuntivi)
                {
                    appoggioDatiAggiuntivi.NoteIndividuali = "";
                    appoggioDatiAggiuntivi.dtaScadenza = "";
                    appoggioDatiAggiuntivi.tipoTrasm = "";
                }
            }

            if (trasmSing != null)
            {
                //Nel caso di ragione con classificazione obbligatoria verifico che sia stato inserito il fascicolo
                if (trasmSing.ragione != null && trasmSing.ragione.fascicolazioneObbligatoria)
                {
                    string msg = string.Empty;
                    if (string.IsNullOrEmpty(this.TxtCodeProject.Text))
                    {
                        msg = "WarningDocumentRequestProject";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                    DocsPaWR.Fascicolo fasc = this.Project;
                    if (fasc != null && fasc.systemID != null && this.EnableBlockClassification)
                    {
                        if (fasc.tipo.Equals("G") && fasc.isFascConsentita != null && fasc.isFascConsentita == "0")
                        {
                            string msgDesc = fasc.isFascicolazioneConsentita ? "WarningDocumentNoDocumentInsert" : "WarningDocumentNoDocumentInsertClassification";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return;
                        }
                        if (fasc.tipo.Equals("P") && !fasc.isFascicolazioneConsentita)
                        {
                            string msgDesc = "WarningDocumentNoDocumentInsertFolder";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return;
                        }
                    }
                }
                //MODIFICA PRE PROBLEMA ACCETTAZIONE TRASMISSIONE INVIATA SIA A RUOLO CHE UTENTE
                trasmUtente = trasmSing.trasmissioneUtente.Where(i => i.systemId == TramUtenteId).FirstOrDefault();

                    trasmUtente.dataAccettata = dateformat.getDataOdiernaDocspa(); //getDataCorrente();

                //tipoRisposta
                    trasmUtente.tipoRisposta = TrasmissioneTipoRisposta.ACCETTAZIONE;

                if (!TrasmManager.executeAccRif(this, trasmUtente, TrasmId, null, out errore))
                {
                    rtn = false;
                    trasmUtente.dataRifiutata = null;
                    trasmUtente.dataAccettata = null;

                    string msg = "ErrorCustom";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '', '" + errore.Replace("'", @"\'") + "');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '', '" + errore.Replace("'", @"\'") + "');}", true);
                }
                else
                {
                    LibroFirmaManager.InserimentoInLibroFirma(this.Page, trasmissione, TrasmSingId);

                    this.FascicolaDocumento();
                    this.verificaDiagrammiDiStato();
                    this.ProgressStatus();
                }

            }

            this.GestVisibilitaFrecceNavigaUO();

            ReturnValueRejectTransm = string.Empty;

            // Gestione del mantenimento dei flag selezionati e dei tasti di default
            Session.Add("NoteGenSmista", this.txtNoteGen.Text);
            //this.ChkMantieniSelezione();
            this.txtNoteGen.Text = Session["NoteGenSmista"].ToString();
            Session.Remove("NoteGenSmista");
        }

        /// <summary>
        /// Avanzamento navigazione
        /// </summary>
        private void ProgressStatus()
        {
            SmistaDocManager docManager = this.GetSmistaDocManager();

            if (docManager.GetDocumentCount() == 1)
            {
                SmistaDocSessionManager.ReleaseSmistaDocManager();


                string msg = "WarningSmistaDocTrasmNotExists";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'warning');}", true);
                this.CloseMask(false);

            }
            else
            {
                docManager.RemoveDocument();

                if (docManager.GetDocumentCount() == 1 || (docManager.GetCurrentDocumentPosition() > docManager.GetDocumentCount()))
                {
                    docManager.MoveAbsoluteDocument(docManager.GetCurrentDocumentPosition() - 1);
                }
                else
                {
                    docManager.MoveAbsoluteDocument(docManager.GetCurrentDocumentPosition());
                }
                DocumentManager.setSelectedNumberVersion("0");
                DocumentManager.RemoveSelectedAttachId();

                this.ChkMantieniSelezione();
                this.FillDataDocumentoTrasmesso();
                this.SetFlagDestinatarioRaggiutoDaTrasm(this.grdUOApp, this.hdUOapp.Value);
                this.RefreshDocumentCounter();
                this.upPnlInfoDocument.Update();

                this.refreshDataButton();
            }
        }

        private void verificaDiagrammiDiStato()
        {
            //Verifico l'abilitazione dei diagrammi di stato
            if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
            {
                SmistaDocManager mng = this.GetSmistaDocManager();
                string idTrasmissione = mng.GetIdTrasmissione(mng.GetCurrentDocumentPosition() - 1);

                DocsPaWR.DocumentoSmistamento infoDocumento = mng.GetCurrentDocument(false);

                //E' importante che l'accettazione della trasmiossione corrente sia fatta prima di questo tipo di verifica
                if (DiagrammiManager.isUltimaDaAccettare(idTrasmissione, this))
                {
                    DocsPaWR.Stato statoSucc = DiagrammiManager.getStatoSuccessivoAutomatico(infoDocumento.DocNumber);
                    DocsPaWR.Stato statoCorr = DiagrammiManager.GetStateDocument(infoDocumento.DocNumber);
                    if (statoSucc != null)
                    {
                        if (statoSucc.STATO_FINALE)
                        {
                            //Controllo se non è bloccato il documento principale o un suo allegato
                            if (CheckInOut.CheckInOutServices.IsCheckedOutDocument(infoDocumento.IDDocumento, infoDocumento.DocNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord()))
                            {
                                string msg = "WarningSmistaDocInCheckOut";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                                return;
                            }

                        }
                        else
                        {
                            //Cambio stato
                            DocsPaWR.DiagrammaStato dg = DiagrammiManager.getDiagrammaById(Convert.ToString(statoSucc.ID_DIAGRAMMA));
                            DiagrammiManager.salvaModificaStato(infoDocumento.DocNumber, Convert.ToString(statoSucc.SYSTEM_ID), dg, UserManager.GetInfoUser().userId, UserManager.GetInfoUser(), "", this);

                            //Cancellazione storico trasmissioni
                            DiagrammiManager.deleteStoricoTrasmDiagrammi(infoDocumento.DocNumber, Convert.ToString(statoCorr.SYSTEM_ID));
                            //Verifico se il nuovo stato ha delle trasmissioni automatiche
                            DocsPaWR.Stato stato = DiagrammiManager.GetStateDocument(infoDocumento.DocNumber);
                            string idTipoDoc = ProfilerDocManager.getIdTemplate(infoDocumento.DocNumber);
                            if (idTipoDoc != "")
                            {
                                ArrayList modelli = new ArrayList(DiagrammiManager.isStatoTrasmAuto(UserManager.GetInfoUser().idAmministrazione, Convert.ToString(stato.SYSTEM_ID), idTipoDoc));
                                for (int i = 0; i < modelli.Count; i++)
                                {
                                    DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                                    if (mod.SINGLE == "1")
                                    {
                                        DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(DocumentInWorking);
                                        TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), infoDoc, this);
                                    }
                                    else
                                    {
                                        for (int k = 0; k < mod.MITTENTE.Length; k++)
                                        {
                                            if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == RoleManager.GetRoleInSession().systemId)
                                            {
                                                DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(DocumentInWorking);
                                                TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), infoDoc, this);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void FascicolaDocumento()
        {
            if (this.IsEnabledFascicolazione())
            {
                SmistaDocManager docManager = this.GetSmistaDocManager();
                DocsPaWR.DocumentoSmistamento docsmista = docManager.GetCurrentDocument(false);
                DocsPaWR.Fascicolo fascicolo = ProjectManager.getFascicoloSelezionatoFascRapida(this);
                if (fascicolo != null)
                {
                    DocsPaWR.Folder folder = null;
                    if (fascicolo.folderSelezionato != null)
                        folder = fascicolo.folderSelezionato;
                    else
                        folder = ProjectManager.getFolder(this, fascicolo);

                    if (folder != null)
                    {
                        string idFolder = folder.systemID;
                        string idDoc = docsmista.IDDocumento;
                        bool isInFolder = false;
                        String message = String.Empty;
                        bool docInserted = DocumentManager.addDocumentoInFolder(this, idDoc, idFolder, false, out isInFolder, out message);

                        if (!docInserted)
                            this.AlertErroreFascicolazione();
                    }
                }
            }
        }

        private void AlertErroreFascicolazione()
        {
            string msg = "WarningSmistaDocProjectError";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
        }

        protected void BtnSmistamentoReject_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RejectTransmissions", "ajaxModalPopupRejectTransmissions();", true);
        }

        private void smistaDocumento()
        {
            string msg = "";

            try
            {
                bool selezioneUoApp = false;
                bool selezioneUoInf = false;
                bool selezioneUtenteRuolo = false;

                SmistaDocManager docManager = this.GetSmistaDocManager();
                DocsPaWR.DocumentoSmistamento docSmistamento = docManager.GetCurrentDocument(false);
                this.UpdateFlagUo(this.indxUoSel, out selezioneUoApp, out selezioneUoInf, out selezioneUtenteRuolo);

                string alertMessage = string.Empty;

                //Se per un ruolo selezionato non è stato anche selezionato almeno un utente scatta l'alert
                if (selezioneUtenteRuolo)
                {
                    msg = "WarningSmistaDocAlmostOneCheckedUser";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }

                // NB: 
                // IL CONTROLLO SULLE SELEZIONI VIENE COMMENTATO LATO FRONTEND IN 
                // QUANTO VIENE EFFETTUATO CORRETTAMENTE LATO BACKEND, IL QUALE CORRETTAMENTE
                // NON SI FIDA DELL'INPUT CHE ARRIVA DALL'ESTERNO E RICONTROLLA I DATI.
                // BENCHE' SAREBBE COSA BUONA E GIUSTA FARE TALI CONTROLLI ANCHE DA FRONTEND, 
                // MA ATTUALMENTE LA FUNZIONE DI CONTROLLO NON è UTILIZZABILE ED E' RIFARE

                //Gestione NoteGenerali se è popolato il campo di testo
                docManager.setNoteGenarali(txtNoteGen.Text);

                DocsPaWR.EsitoSmistamentoDocumento[] retValue = docManager.SmistaDocumento();

                bool notaSmistamento = false;   // codice 99
                string descNota = "Avviso\\n\\nil documento [" + docSmistamento.Segnatura + "] non è stato trasmesso ai seguenti ruoli poichè operano su registro differente:\\n\\n";

                bool erroreSmistamento = false; // codice > 0
                string descrizioneErroreSmistamento = string.Empty;

                // lettura valori di ritorno dello smistamento del documento corrente
                foreach (DocsPaWR.EsitoSmistamentoDocumento item in retValue)
                {
                    if (item.CodiceEsitoSmistamento.Equals(-1))
                    {
                        // Codice -1: 
                        // nessun destinatario selezionato per lo smistamento
                        erroreSmistamento = true;
                        descrizioneErroreSmistamento = item.DescrizioneEsitoSmistamento.Replace("'", @"\'");
                    }

                    if (!item.CodiceEsitoSmistamento.Equals(0) && !item.CodiceEsitoSmistamento.Equals(99) && !item.CodiceEsitoSmistamento.Equals(999))
                        erroreSmistamento = true;

                    if (!item.CodiceEsitoSmistamento.Equals(0) && item.CodiceEsitoSmistamento.Equals(99))
                    {
                        notaSmistamento = true;
                        descNota += " - smistamento a " + item.DenominazioneDestinatario.Replace("'", "\\'") + ":\\n" + item.DescrizioneEsitoSmistamento + "\\n\\n";
                    }

                    if (!item.CodiceEsitoSmistamento.Equals(0) && item.CodiceEsitoSmistamento.Equals(999))
                    {
                        erroreSmistamento = true;
                        notaSmistamento = true;
                    }

                }

                // movenext del documento da smistare o uscita dallo smistamento
                if (!erroreSmistamento && !notaSmistamento)
                {
                    this.FascicolaDocumento();
                    this.verificaDiagrammiDiStato();
                    this.ProgressStatus();

                    if (!this.chk_mantieniSel.Checked)
                    {
                        // rimozione delle selezioni effettuate dall'utente qualora non sia attivo il flag mantieni selezione
                        this.ClearSelections();
                    }
                }
                else
                {
                    if (notaSmistamento)
                    {
                        this.FascicolaDocumento();
                        this.ProgressStatus();

                        if (!this.chk_mantieniSel.Checked)
                        {
                            // rimozione delle selezioni effettuate dall'utente qualora non sia attivo il flag mantieni selezione
                            this.ClearSelections();
                        }
                    }
                    else
                    {
                        if (descrizioneErroreSmistamento != string.Empty)
                        {
                            msg = "WarningSmistaDocError";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                        }
                    }
                }


                // Gestione del mantenimento dei flag selezionati e dei tasti di default
                Session.Add("NoteGenSmista", this.txtNoteGen.Text);
                //this.ChkMantieniSelezione();
                this.txtNoteGen.Text = Session["NoteGenSmista"].ToString();
                Session.Remove("NoteGenSmista");
            }
            catch (Exception ex)
            {
                msg = "WarningSmistaDocError";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);

            }
            finally
            {
                //rilascio le risorse allocate per il settaggio e la gestione dei dati aggiuntivi
                //dello smistamento (note individuali e data scadenza)
                this.GestVisibilitaFrecceNavigaUO();
            }
        }

        protected void BtnSmistamentoSmista_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            /*			Ragioni di trasmissione per lo smistamento:
			 *			
			 *			-------------------------------------------------------------------------
			 *			NOME:					COMPETENZA			|		CONOSCENZA	
			 *			-------------------------------------------------------------------------
			 *			VISIBILITA':				NO				|			NO
			 *			TIPO:					WORKFLOW			|		SENZA WORKFLOW
			 *			DIRITTI:				LETURA / SCRITTURA	|		LETTURA
			 *			DESTINATARI:			SOLO SOTTOPOSTI		|		SOLO SOTTOPOSTI
			 *			E' UNA RISPOSTA:			NO				|			NO
			 *			PREVEDE RISPOSTA:			NO				|			NO
			 *			EREDITA':					NO				|			NO
			 *			-------------------------------------------------------------------------
			 * */
            this.smistaDocumento();

            //Remove note correnti
            if (Session["datiAggiuntivi"] != null)
            {

                DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = (DocsPaWR.datiAggiuntiviSmistamento[])Session["datiAggiuntivi"];

                foreach (DocsPaWR.datiAggiuntiviSmistamento appoggioDatiAggiuntivi in datiAggiuntivi)
                {
                    appoggioDatiAggiuntivi.NoteIndividuali = "";
                    appoggioDatiAggiuntivi.dtaScadenza = "";
                    appoggioDatiAggiuntivi.tipoTrasm = "";
                }
            }
        }

        /// <summary>
        /// Evento visualizzazione dettagli di firma
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSmistamentoDetSign_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            DocsPaWR.DocumentoSmistamento docSmistamento = this.GetSmistaDocManager().GetCurrentDocument(true);

            if (this.BtnSmistamentoDetSign.Enabled)
            {
                this.SetSignedDocumentOnSession(docSmistamento.ImmagineDocumento);

                //La popup dovrà essere implementata LNR
                //if (!this.ClientScript.IsClientScriptBlockRegistered("ShowDettagliFirma"))
                //    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ShowDettagliFirma", "<script>ShowMaskDettagliFirma();</script>");

                string errorMessage = string.Empty;
                SmistaDocManager docManager = this.GetSmistaDocManager();
                DocsPaWR.DocumentoSmistamento docCorrente = docManager.GetCurrentDocument(false);
                if (this.DocumentInWorking == null || this.DocumentInWorking.systemId != docCorrente.IDDocumento)
                {
                    DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentListVersions(this, docCorrente.IDDocumento, docCorrente.DocNumber);
                    DocumentManager.SetDataVistaSP(schedaDocumento.systemId);
                    DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                    this.DocumentInWorking = schedaDocumento;
                }

                UIManager.DocumentManager.setSelectedRecord(this.DocumentInWorking);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDigitalSignDetails", "ajaxModalPopupDigitalSignDetails();", true);
            }
            else
            {
                this.SetSignedDocumentOnSession(null);
            }

            // gestione naviga UO
            this.GestVisibilitaFrecceNavigaUO();
        }

        protected void btn_selezioniSmistamento_Click(object sender, ImageClickEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            bool selezioneUoApp = false;
            bool selezioneUoInf = false;
            bool selezioneUtenteRuolo = false;

            this.UpdateFlagUo(this.indxUoSel, out selezioneUoApp, out selezioneUoInf, out selezioneUtenteRuolo);

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "SmistaDocSelectionsDetails", "ajaxModalPopupSmistaDocSelectionsDetails();", true);

        }

        protected void btnSmistaDocSelectionsDetailsPostback_Click(object sender, EventArgs e)
        {

        }

        protected void ddl_trasm_rapida_SelectedIndexChanged(object sender, EventArgs e)
        {
            SmistaDocManager docManager = this.GetSmistaDocManager();
            DocsPaWR.UOSmistamento uoAppartenenza = docManager.GetUOAppartenenza();
            uoAppartenenza.UoSmistaTrasAutomatica = null;
            DocsPaWR.ModelloTrasmissione modello = new DocsPaWR.ModelloTrasmissione();
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            ArrayList uoTrasm = new ArrayList();
            List<string> uoRiferimento = new List<string>();

            int indexSel = ddl_trasm_rapida.SelectedIndex;
            if (indexSel > 0)
            {
                modello = ws.getModelloByID(UserManager.GetInfoUser().idAmministrazione, ddl_trasm_rapida.SelectedValue);
                //Emanuela 21-05-2014: inserisco le note generali del modello slezionato
                //if (string.IsNullOrEmpty(this.txtNoteGen.Text))
                if(!string.IsNullOrEmpty(modello.VAR_NOTE_GENERALI))
                    this.txtNoteGen.Text = modello.VAR_NOTE_GENERALI;
                //if (!string.IsNullOrEmpty(this.txtNoteGen.Text))
                //    Session.Add("NoteGenSmista", this.txtNoteGen.Text);
                Session.Remove("NoteGenSmista");
                this.upPnlNote.Update();
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                {
                    DocsPaWR.RagioneDest ragDest = (DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {
                        DocsPaWR.UOSmistamento uo = new DocsPaWR.UOSmistamento();
                        DocsPaWR.RuoloSmistamento ruolo = new DocsPaWR.RuoloSmistamento();
                        DocsPaWR.MittDest mittDest = (DocsPaWR.MittDest)destinatari[j];
                        switch (mittDest.CHA_TIPO_MITT_DEST)
                        {
                            case "D":
                                if (mittDest.CHA_TIPO_URP == "R")
                                {
                                    bool isOneUtenteNotifica = false;
                                    ruolo.ID = Convert.ToString(mittDest.ID_CORR_GLOBALI);
                                    ruolo.Descrizione = mittDest.DESCRIZIONE;
                                    ruolo.Codice = mittDest.VAR_COD_RUBRICA;
                                    ruolo.ragioneTrasmRapida = ragDest.RAGIONE;
                                    ruolo.Registri = ws.SmistaGetRegistriRuolo(ruolo.ID);
                                    ruolo.chaTipoTrasm = mittDest.CHA_TIPO_TRASM;
                                    ruolo.datiAggiuntiviSmistamento = new DocsPaWR.datiAggiuntiviSmistamento();
                                    ruolo.datiAggiuntiviSmistamento.NoteIndividuali = mittDest.VAR_NOTE_SING;
                                    ruolo.datiAggiuntiviSmistamento.tipoTrasm = mittDest.CHA_TIPO_TRASM;
                                    ArrayList utentiConNotifica = new ArrayList();
                                    for (int k = 0; k < mittDest.UTENTI_NOTIFICA.Length; k++)
                                    {
                                        DocsPaWR.UtentiConNotificaTrasm utenteNotifica = (DocsPaWR.UtentiConNotificaTrasm)mittDest.UTENTI_NOTIFICA[k];
                                        if (utenteNotifica.FLAG_NOTIFICA == "1")
                                        {
                                            DocsPaWR.UtenteSmistamento utenteSmistamento = new DocsPaWR.UtenteSmistamento();
                                            utenteSmistamento.Denominazione = utenteNotifica.NOME_COGNOME_UTENTE;
                                            utenteSmistamento.ID = utenteNotifica.ID_PEOPLE;
                                            utenteSmistamento.UserID = utenteNotifica.CODICE_UTENTE;
                                            this.getDatiAggiuntiviUtenteSmistamento(ref utenteSmistamento, utenteSmistamento.ID);
                                            utentiConNotifica.Add(utenteSmistamento);
                                            isOneUtenteNotifica = true;
                                        }
                                    }
                                    if (isOneUtenteNotifica)
                                    {
                                        DocsPaWR.UtenteSmistamento[] utenti = new DocsPaWR.UtenteSmistamento[utentiConNotifica.Count];
                                        utentiConNotifica.CopyTo(utenti);
                                        ruolo.Utenti = utenti;
                                        DocsPaWR.RuoloSmistamento[] ruoli = { ruolo };
                                        uo.Ruoli = ruoli;
                                        uoTrasm.Add(uo);
                                    }
                                }
                                if (mittDest.CHA_TIPO_URP == "U")
                                {
                                    uo.ragioneTrasmRapida = ragDest.RAGIONE;
                                    uo.ID = Convert.ToString(mittDest.ID_CORR_GLOBALI);
                                    uo.Codice = mittDest.VAR_COD_RUBRICA;
                                    uo.Descrizione = mittDest.DESCRIZIONE;
                                    uo.Ruoli = ws.getRuoliUoSmistamento(uo.ID);
                                    if (uo.Ruoli.Length > 0)
                                    {
                                        foreach (DocsPaWR.RuoloSmistamento role in uo.Ruoli)
                                        {
                                            role.datiAggiuntiviSmistamento = new DocsPaWR.datiAggiuntiviSmistamento();
                                            role.datiAggiuntiviSmistamento.tipoTrasm = mittDest.CHA_TIPO_TRASM;
                                            role.datiAggiuntiviSmistamento.NoteIndividuali = mittDest.VAR_NOTE_SING;
                                        }
                                        uoTrasm.Add(uo);
                                    }
                                    else
                                    {
                                        uoRiferimento.Add("Manca un ruolo di riferimento per la UO: "
                                                    + uo.Codice + " (" + uo.Descrizione + ")"
                                                    + ".");
                                    }
                                }
                                if (mittDest.CHA_TIPO_URP == "P")
                                {
                                    DocsPaWR.UtenteSmistamento ut = new DocsPaWR.UtenteSmistamento();
                                    //OLD:  ut.ID = Convert.ToString(mittDest.ID_CORR_GLOBALI);
                                    //LULUCIANI
                                    DocsPaWR.UtentiConNotificaTrasm utenteNotifica = (DocsPaWR.UtentiConNotificaTrasm)mittDest.UTENTI_NOTIFICA[0];
                                    if (utenteNotifica != null && !string.IsNullOrEmpty(utenteNotifica.ID_PEOPLE))
                                        ut.ID = Convert.ToString(utenteNotifica.ID_PEOPLE);
                                    else throw new Exception("Errore impossibile trovare l'utente con notifica con ID: " + mittDest.ID_CORR_GLOBALI);
                                    //END LULUCIANI                                   
                                    ut.ragioneTrasmRapida = ragDest.RAGIONE;
                                    ut.UserID = mittDest.VAR_COD_RUBRICA;
                                    ut.Denominazione = mittDest.DESCRIZIONE;
                                    ut.datiAggiuntiviSmistamento = new DocsPaWR.datiAggiuntiviSmistamento();
                                    ut.datiAggiuntiviSmistamento.NoteIndividuali = mittDest.VAR_NOTE_SING;
                                    ut.datiAggiuntiviSmistamento.tipoTrasm = mittDest.CHA_TIPO_TRASM;
                                    this.getDatiAggiuntiviUtenteSmistamento(ref ut, ut.ID);
                                    ruolo.Utenti = new DocsPaWR.UtenteSmistamento[] { ut };
                                    uo.Ruoli = new DocsPaWR.RuoloSmistamento[] { ruolo };
                                    uoTrasm.Add(uo);
                                }
                                break;
                            case "UT_P":
                                DocsPaWR.Corrispondente corrispondente = new DocsPaWR.Corrispondente();
                                corrispondente = TrasmManager.getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, this.DocumentInWorking, null, this);
                                DocsPaWR.UtenteSmistamento[] utentiNot = ws.getUtentiRuoloSmistamento(corrispondente.systemId, "U");
                                utentiNot[0].ragioneTrasmRapida = ragDest.RAGIONE;
                                ruolo.Utenti = utentiNot;
                                DocsPaWR.RuoloSmistamento[] ruoliSmistamento = { ruolo };
                                uo.Ruoli = ruoliSmistamento;
                                foreach (DocsPaWR.RuoloSmistamento role in uo.Ruoli)
                                {
                                    role.datiAggiuntiviSmistamento = new DocsPaWR.datiAggiuntiviSmistamento();
                                    role.datiAggiuntiviSmistamento.tipoTrasm = mittDest.CHA_TIPO_TRASM;
                                    role.datiAggiuntiviSmistamento.NoteIndividuali = mittDest.VAR_NOTE_SING;
                                }
                                uoTrasm.Add(uo);
                                break;
                            case "UO_P":
                                DocsPaWR.Corrispondente corrUo = new DocsPaWR.Corrispondente();
                                corrUo = TrasmManager.getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, this.DocumentInWorking, null, this);
                                uo.ragioneTrasmRapida = ragDest.RAGIONE;
                                uo.ID = corrUo.systemId;
                                uo.Codice = corrUo.codiceCorrispondente;
                                uo.Descrizione = corrUo.descrizione;
                                //prendo dolo i ruoli di riferimento come avviene nella trasmissione da modello
                                uo.Ruoli = ws.getRuoliUoSmistamento(uo.ID);
                                if (uo.Ruoli.Length > 0)
                                {
                                    foreach (DocsPaWR.RuoloSmistamento role in uo.Ruoli)
                                    {
                                        role.datiAggiuntiviSmistamento = new DocsPaWR.datiAggiuntiviSmistamento();
                                        role.datiAggiuntiviSmistamento.tipoTrasm = mittDest.CHA_TIPO_TRASM;
                                        role.datiAggiuntiviSmistamento.NoteIndividuali = mittDest.VAR_NOTE_SING;
                                    }
                                    uoTrasm.Add(uo);
                                }
                                break;
                            default:
                                DocsPaWR.Corrispondente corr = new DocsPaWR.Corrispondente();
                                corr = TrasmManager.getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, this.DocumentInWorking, null, this);
                                if (corr != null)
                                {
                                    ruolo.ID = corr.systemId;
                                    ruolo.Descrizione = corr.descrizione;
                                    ruolo.Codice = corr.codiceCorrispondente;
                                    ruolo.ragioneTrasmRapida = ragDest.RAGIONE;
                                    ruolo.Registri = ws.SmistaGetRegistriRuolo(ruolo.ID);
                                    DocsPaWR.UtenteSmistamento[] utentiNotifica = ws.getUtentiRuoloSmistamento(ruolo.ID, "R");
                                    ruolo.Utenti = utentiNotifica;
                                    DocsPaWR.RuoloSmistamento[] ruoliSmista = { ruolo };
                                    uo.Ruoli = ruoliSmista;
                                    if (utentiNotifica.Length > 0)
                                    {
                                        foreach (DocsPaWR.RuoloSmistamento role in uo.Ruoli)
                                        {
                                            role.datiAggiuntiviSmistamento = new DocsPaWR.datiAggiuntiviSmistamento();
                                            role.datiAggiuntiviSmistamento.tipoTrasm = mittDest.CHA_TIPO_TRASM;
                                            role.datiAggiuntiviSmistamento.NoteIndividuali = mittDest.VAR_NOTE_SING;
                                        }
                                        uoTrasm.Add(uo);
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            if (uoTrasm.Count > 0)
            {
                DocsPaWR.UOSmistamento[] uoTrasmRapida = new DocsPaWR.UOSmistamento[uoTrasm.Count];
                uoTrasm.CopyTo(uoTrasmRapida);
                uoAppartenenza.UoSmistaTrasAutomatica = uoTrasmRapida;
                if (!string.IsNullOrEmpty(modello.NO_NOTIFY) && modello.NO_NOTIFY.Equals("1"))
                    uoAppartenenza.modelloNoNotify = true;
                else
                    uoAppartenenza.modelloNoNotify = false;
            }

            if (uoRiferimento != null && uoRiferimento.Count != 0)
            {
                string msgError = string.Empty;
                foreach (string s in uoRiferimento)
                {
                    msgError = msgError + s + "\\n";
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorMessageUORiferimento', 'error', '', '" + msgError + "');} else {parent.ajaxDialogModal('ErrorMessageUORiferimento', 'error', '', '" + msgError + "');}", true);
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + ErrorMessageUORiferimento.Replace("@@",utils.FormatJs(msgError)) + "', 'warning', '');} else {parent.ajaxDialogModal('" + ErrorMessageUORiferimento.Replace("@@",utils.FormatJs(msgError)) + "', 'warning', '');}", true);
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgError.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgError.Replace("'", @"\'") + "', 'warning', '');}", true);
                uoRiferimento = new List<string>();
            }
        }

        private void getDatiAggiuntiviUtenteSmistamento(ref DocsPaWR.UtenteSmistamento utenteSmistamento, string idUtente)
        {
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            ws.getDatiAggiuntiviUtenteSmistamento(ref utenteSmistamento, idUtente);
        }

        protected void btnRejectTransmission_Click(object sender, EventArgs e)
        {

        }

        protected void btn_clearFlag_Click(object sender, ImageClickEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            // Rimozione di tutte le selezioni effettuate nelle UO visualizzate
            this.ResetDetail();
            this.GestVisibilitaFrecceNavigaUO();
        }

        private string ReturnValue
        {
            get
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["ReturnValuePopup"].ToString()))
                    return HttpContext.Current.Session["ReturnValuePopup"].ToString();
                else
                    return string.Empty;
            }
        }

        public string TxtNoteViewer
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["TxtNoteViewer"] != null)
                {
                    result = HttpContext.Current.Session["TxtNoteViewer"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TxtNoteViewer"] = value;
            }
        }


        protected void DocumentImgNoteGenDetail_Click(object o, EventArgs e)
        {
            try
            {
                this.TxtNoteViewer = this.txtNoteGen.Text;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ViewObject", "ajaxModalPopupViewNoteGen();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DocumentImgNoteIndDetail_Click(object o, EventArgs e)
        {
            try
            {
                this.TxtNoteViewer = this.txtAreaNoteInd.Text;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ViewObject", "ajaxModalPopupViewNoteInd();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public bool IsDestRaggiuntoDaTrasm(GridViewRow row, string tipo, List<string> idCorrTrasmissioneDocList)
        {
            string idCorrGlobali = this.grdUOApp.DataKeys[row.RowIndex].Values["idcorr"].ToString();
            string id = this.grdUOApp.DataKeys[row.RowIndex].Values["id"].ToString();
            string idPoeple = string.Empty;
            if (tipo == "P")
                idPoeple = GetId(row);
            foreach (string idCorrTrasm in idCorrTrasmissioneDocList)
            {
                if (tipo == "R" && idCorrGlobali.Equals(idCorrTrasm.Split('_')[0]))
                    return true;
                //Nel caso di utente devo controllare se ci sono trasm a utente o a ruolo
                if (tipo == "P")
                {
                    //Controllo trasm a ruolo
                    if (id.Equals(idCorrTrasm))
                        return true;

                    //Controllo la trasmissione a utente
                    if ((idCorrGlobali + "_" + idPoeple).Equals(idCorrTrasm))
                        return true;
                }
            }
            return false;
        }

        public CheckBox GetCheckNotificaTuttoRuolo(GridViewRow rows)
        {
            CheckBox chkNotificaTutti = null;
            string type = this.GetTipoURP(rows);
            string idCorrGlobali = string.Empty;
            if (type.Equals("R"))
                return this.GetGridItemControl<CheckBox>(rows, "chkNotifica");

            if (type.Equals("P"))
                idCorrGlobali = this.grdUOApp.DataKeys[rows.RowIndex].Values["id"].ToString().Split('_')[0];

            foreach (GridViewRow row in this.grdUOApp.Rows)
            {
                if (this.grdUOApp.DataKeys[row.RowIndex].Values["type"].ToString() == "R" && GetId(row).Equals(idCorrGlobali))
                {
                    chkNotificaTutti = this.GetGridItemControl<CheckBox>(row, "chkNotifica");
                    break;
                }
            }
            return chkNotificaTutti;
        }
    }
}
