using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using NttDataWA.Utils;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDataWA.UserControls;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class NewProject : System.Web.UI.Page
    {
        private string ReturnValue
        {
            set
            {
                Session["ReturnValuePopup"] = value;
            }
        }

        private void RemovePropertySearchCorrespondentIntExtWithDisabled()
        {
            HttpContext.Current.Session.Remove("searchCorrespondentIntExtWithDisabled");
        }

        private bool EnableStateDiagram
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableStateDiagramProject"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableStateDiagramProject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableStateDiagramProject"] = value;
            }
        }

        private DiagrammaStato StateDiagram
        {
            get
            {
                DiagrammaStato result = null;
                if (HttpContext.Current.Session["stateDiagramProject"] != null)
                {
                    result = HttpContext.Current.Session["stateDiagramProject"] as DiagrammaStato;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["stateDiagramProject"] = value;
            }
        }
        //profilazione dinamica
        private DocsPaWR.Templates TemplateProject
        {
            get
            {
                Templates result = null;
                if (HttpContext.Current.Session["templateProject2"] != null)
                {
                    result = HttpContext.Current.Session["templateProject2"] as Templates;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["templateProject2"] = value;
            }
        }

        private bool CustomProject
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["customProject"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["customProject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["customProject"] = value;
            }
        }

        /// <summary>
        /// numero di caratteri nella nota di testo
        /// </summary>
        private int MaxLenghtProject
        {
            get
            {
                int result = 2000;
                if (HttpContext.Current.Session["MaxLenghtProject"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["MaxLenghtProject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["MaxLenghtNote"] = value;
            }
        }

        /// <summary>
        /// auotcomplete per la ricerca della nota
        /// </summary>
        protected int AutocompleteMinimumPrefixLength
        {
            get
            {
                int result = 3;
                if (HttpContext.Current.Session["AutocompleteMinimumPrefixLength"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["AutocompleteMinimumPrefixLength"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["AutocompleteMinimumPrefixLength"] = value;
            }
        }

        /// <summary>
        /// numero di caratteri nella nota di testo
        /// </summary>
        private int MaxLenghtNote
        {
            get
            {
                int result = 2000;
                if (HttpContext.Current.Session["MaxLenghtNote"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["MaxLenghtNote"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["MaxLenghtNote"] = value;
            }
        }

        public RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_PROTO_IN;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
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


        #region constanti

        private const string DIRITTI = "2";
        private const string ATTIVO = "1";
        private const string START_SPECIAL_STYLE = "<span class=\"redStrike\">";
        private const string END_SPECIAL_STYLE = "</span>";

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.LoadKey();
                this.InitializeLanguage();
                this.InitializePage();
            }
            else
            {
                if (!string.IsNullOrEmpty(this.AddressBook.ReturnValue))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('AddressBook','')", true);
                }

                if (!string.IsNullOrEmpty(this.Note.ReturnValue))
                {
                    this.FetchNote();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('Note','');", true);
                }

                if (this.CustomProject)
                {
                    this.PnlTypeDocument.Controls.Clear();
                    if (!string.IsNullOrEmpty(this.projectDdlTipologiafascicolo.SelectedValue))
                    {
                        if (this.TemplateProject == null || !this.TemplateProject.SYSTEM_ID.Equals(this.projectDdlTipologiafascicolo.SelectedValue))
                        {
                            this.TemplateProject = ProfilerProjectManager.getTemplateFascById(this.projectDdlTipologiafascicolo.SelectedItem.Value);
                        }
                        if (this.CustomProject)
                        {
                            this.PopulateProfiledDocument();
                        }
                    }
                }
            }

            this.RefreshScripts();
        }

        private void LoadKey()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_DESC_FASC.ToString())))
            {
                this.MaxLenghtProject = int.Parse(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_DESC_FASC.ToString()));
            }
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.FE_MAX_LENGTH_NOTE.ToString()]))
            {
                this.MaxLenghtNote = int.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.FE_MAX_LENGTH_NOTE.ToString()]);
            }
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, DBKeys.ENABLE_FASCICOLO_PUBBLICO.ToString())) && !Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, DBKeys.ENABLE_FASCICOLO_PUBBLICO.ToString()).Equals("0"))
            {
                this.ProjectCheckPublic.Visible = true;
            }
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamicaFasc.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamicaFasc.ToString()].Equals("1"))
            {
                this.CustomProject = true;
            }
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()].Equals("1"))
            {
                this.EnableStateDiagram = true;
            }
        }

        private void InitializePage()
        {
            ProjectManager.setProjectInSession(null);
            DocsPaWR.UnitaOrganizzativa _uo = UIManager.RoleManager.GetRoleInSession().uo;
            this.projectTxtCodiceCollocazione.Text = _uo.codiceRubrica;
            this.idProjectCollocation.Value = _uo.systemId;
            this.projectTxtDescrizioneCollocazione.Text = _uo.descrizione;
            this.projectTxtdata.Text = (string)DateTime.Now.ToShortDateString();

            this.LoadTipologiaFascicolo();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.NewProjectSave.Text = Utils.Languages.GetLabelFromCode("NewProjectSave", language);
            this.NewProjectClose.Text = Utils.Languages.GetLabelFromCode("NewProjectClose", language);
            this.projectLblDescrizione.Text = Utils.Languages.GetLabelFromCode("projectLblDescrizione", language);
            this.projectLtrDescrizione.Text = Utils.Languages.GetLabelFromCode("DocumentLitVisibleNotesChars", language) + " ";
            this.projectLitVisibleNotes.Text = Utils.Languages.GetLabelFromCode("DocumentNoteNoneVisible", language);
            this.projectImgNotedetails.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgNotedetails", language);
            this.projectLitVisibleNotesChars.Text = Utils.Languages.GetLabelFromCode("DocumentLitVisibleNotesChars", language);
            this.projectLblCollocazioneFisica.Text = Utils.Languages.GetLabelFromCode("projectLblCollocazioneFisica", language);
            this.projectLblDataCollocazioneFisica.Text = Utils.Languages.GetLabelFromCode("projectLblDataCollocazioneFisica", language);
            this.projectLblCartaceo.Text = Utils.Languages.GetLabelFromCode("projectLblCartaceo", language);
            this.ProjectCheckPrivate.Text = Utils.Languages.GetLabelFromCode("ProjectCheckPrivate", language);
            this.ProjectCheckPrivate.ToolTip = Utils.Languages.GetLabelFromCode("ProjectCheckPrivateTooltip", language);
            this.ProjectCheckPublic.Text = Utils.Languages.GetLabelFromCode("ProjectCheckPublic", language);
            this.ProjectCheckPublic.ToolTip = Utils.Languages.GetLabelFromCode("ProjectCheckPublicTooltip", language);
            this.DocumentImgCollocationAddressBook.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgCollocationAddressBook", language);
            this.DocumentImgCollocationAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgCollocationAddressBook", language);
            this.Note.Title = Utils.Languages.GetLabelFromCode("BaseMasterNotes", language);
            this.projectLblTipoFascicolo.Text = Utils.Languages.GetLabelFromCode("projectLblTipoFascicolo", language);
            this.projectDdlTipologiafascicolo.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("projectDdlTipologiaFascciolo", language));
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
        }

        private void RefreshScripts()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshprojectTxtDescrizione", "charsLeft('projectTxtDescrizione', " + this.MaxLenghtProject + ", '" + this.projectLtrDescrizione.Text.Replace("'", "\'") + "');", true);
            this.projectTxtDescrizione_chars.Attributes["rel"] = "projectTxtDescrizione_" + this.MaxLenghtProject + "_" + this.projectLtrDescrizione.Text;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshNoteChars", "charsLeft('TxtNote', " + this.MaxLenghtNote + ", '');", true);
            this.TxtNote_chars.Attributes["rel"] = "TxtNote_" + this.MaxLenghtNote + "_" + this.projectLitVisibleNotes.Text;
        }

        private void InitializeAjaxAddressBook()
        {
            string dataUser = UIManager.RoleManager.GetRoleInSession().idGruppo;
            Registro reg = RegistryManager.GetRegistryInSession();
            if (reg == null)
            {
                reg = RoleManager.GetRoleInSession().registri[0];
            }

            dataUser = dataUser + "-" + reg.systemId;

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]))
                this.RapidCollocazione.MinimumPrefixLength = int.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]);

            dataUser = UIManager.RoleManager.GetRoleInSession().systemId + "-" + reg.systemId;
            string callType = "CALLTYPE_GESTFASC_LOCFISICA";
            this.RapidCollocazione.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
        }

        /// <summary>
        /// carica la tipologia dei fascicoli in nella drop down
        /// </summary>
        private void LoadTipologiaFascicolo()
        {
            try
            {
                string _idAmministrazione = UIManager.UserManager.GetUserInSession().idAmministrazione;
                string _idGruppo = UIManager.RoleManager.GetRoleInSession().idGruppo;
                Templates[] template = UIManager.ProjectManager.getTipologiaFascicoloByRuolo(_idAmministrazione, _idGruppo, DIRITTI);

                projectDdlTipologiafascicolo.Items.Add(new ListItem(string.Empty, string.Empty));
                if (template.Length > 0)
                {
                    foreach (Templates t in template)
                    {
                        if (!t.IPER_FASC_DOC.Equals(ATTIVO))
                        {
                            projectDdlTipologiafascicolo.Items.Add(new ListItem(t.DESCRIZIONE, t.SYSTEM_ID.ToString()));
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void NewProjectSave_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                if (this.SaveProject())
                {
                    Fascicolo project = UIManager.ProjectManager.getProjectInSession();
                    this.ReturnValue = project.codice + "#" + project.descrizione;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('NewProject', 'up');} else {parent.closeAjaxModal('NewProject', 'up');};", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private bool SaveProject()
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(projectTxtDescrizione.Text))
            {
                Registro registro = UIManager.RegistryManager.GetRegistryInSession();
                if (registro != null)
                    registro = RoleManager.GetRoleInSession().registri[0];

                Utente user = UIManager.UserManager.GetUserInSession();
                Ruolo role = UIManager.RoleManager.GetRoleInSession();
                InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
                OrgTitolario titolario = UIManager.ClassificationSchemeManager.getTitolarioAttivo(user.idAmministrazione);
                FascicolazioneClassificazione[] fascicolazioneclassificazione = UIManager.ProjectManager.getFascicolazioneClassificazione(user.idAmministrazione, role.idGruppo, infoUtente.idPeople, registro, this.Project.codice, false, titolario.ID);

                Fascicolo fascicolo = setFascicolo(new Fascicolo(), titolario.ID, fascicolazioneclassificazione[0].idRegistroNodoTit, fascicolazioneclassificazione[0].systemID);

                fascicolo.idUoLF = this.idProjectCollocation.Value;
                fascicolo.descrizioneUOLF = this.projectTxtDescrizioneCollocazione.Text;
                fascicolo.varCodiceRubricaLF = this.projectTxtCodiceCollocazione.Text;

                if (this.ProjectCheckPrivate.Checked)
                {
                    fascicolo.privato = "1";
                }
                else
                {
                    fascicolo.privato = "0";
                }

                fascicolo.pubblico = this.ProjectCheckPublic.Checked;

                if (!string.IsNullOrEmpty(this.projectTxtdata.Text))
                {
                    fascicolo.dtaLF = this.projectTxtdata.Text;
                }
                else
                {
                    fascicolo.dtaLF = string.Empty;
                }

                //PROFILAZIONE DINAMICA
                if (!string.IsNullOrEmpty(this.projectDdlTipologiafascicolo.SelectedValue))
                {
                    fascicolo.template = this.PopulateTemplate();

                    if (ProfilerDocManager.verificaCampiObbligatori(this.TemplateProject))
                    {
                        string msgDesc = "WarningProjectRequestfields";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return false;
                    }

                    string customMessaget = string.Empty;
                    string messag = ProfilerDocManager.verificaOkContatore(this.TemplateProject, out customMessaget);
                    if (messag != string.Empty)
                    {
                        if (!string.IsNullOrEmpty(customMessaget) && messag.Equals("CUSTOMERROR"))
                        {
                            string msgDesc = "WarningDocumentCustom";
                            string errFormt = Server.UrlEncode(customMessaget);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');}; ", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + messag.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + messag.Replace("'", @"\'") + "', 'warning', '" + messag.Replace("'", @"\'") + "');}", true);
                        }
                        return false;
                    }
                }

                Fascicolo prj = ProjectManager.getProjectInSession();
                if (prj != null && prj.noteFascicolo != null && prj.noteFascicolo.Length > 0)
                {
                    fascicolo.noteFascicolo = prj.noteFascicolo;
                }


                UIManager.ProjectManager.setProjectInSession(fascicolo);

                msg = string.Empty;
                this.SaveNote(out msg);
                if (!string.IsNullOrEmpty(msg))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'error');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'error');}", true);
                    return false;
                }

                //creazione del fascicolo
                fascicolo = UIManager.ProjectManager.newFascicolo(fascicolazioneclassificazione[0], fascicolo, infoUtente, role);
                string errore = UIManager.ProjectManager.getEsitoCreazioneFascicolo();

                switch (errore)
                {
                    case "OK": msg = string.Empty;
                        {
                            fascicolo = UIManager.ProjectManager.getFascicoloById(fascicolo.systemID, UIManager.UserManager.GetInfoUser());
                            fascicolo.template = ProfilerProjectManager.getTemplateFascDettagli(fascicolo.systemID);
                            UIManager.ProjectManager.setProjectInSession(fascicolo);
                            //this.UpdateProjectCreation();
                            //this.aggiornaPanel();

                            List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                            Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                            actualPage.IdObject = fascicolo.systemID;
                            actualPage.SearchFilters = null;
                            actualPage.Type = string.Empty;
                            actualPage.Page = "PROJECT.ASPX";
                            actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), true, this.Page);
                            actualPage.CodePage = Navigation.NavigationUtils.NamePage.PROJECT.ToString();
                            actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), string.Empty);
                            navigationList.Add(actualPage);
                            Navigation.NavigationUtils.SetNavigationList(navigationList);
                            //this.FromClassification = false;
                            break;
                        }
                    case "GENERIC_ERROR": msg = "ErrorProjectGenericError"; break;
                    case "FASCICOLO_GIA_PRESENTE": msg = "ErrorProjectFascicoloGiaPresente"; break;
                    case "FORMATO_FASCICOLATURA_NON_PRESENTE": msg = "ErrorProjectFormatoFascicolaturaNonPresente"; break;
                    default: msg = "ErrorProjectDefaul"; break;
                }

                if (!string.IsNullOrEmpty(msg))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'info', null,null,null,null,'');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'info', null,null,null,null,'');}  ", true);
                    return false;
                }
                UIManager.ProjectManager.setProjectInSession(fascicolo);
            }
            else
            {
                msg = "ErrorProjectFieldRequired";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning');}", true);
                return false;
            }
            return true;
        }

        private Fascicolo setFascicolo(Fascicolo newFascicolo, string _idTitolario, string _idRegistroNodoTitolario, string _idNodoSelezionato)
        {
            newFascicolo.descrizione = projectTxtDescrizione.Text.Replace(Environment.NewLine, " ");
            newFascicolo.idRegistro = _idRegistroNodoTitolario;
            newFascicolo.idTitolario = _idTitolario;
            newFascicolo.idRegistroNodoTit = _idRegistroNodoTitolario;
            newFascicolo.codUltimo = string.Empty;
            newFascicolo.cartaceo = projectCkCartaceo.Checked;
            newFascicolo.isFascConsentita = "1";
            newFascicolo.controllato = "0";
            newFascicolo.idUoLF = this.idProjectCollocation.Value;
            newFascicolo.descrizioneUOLF = this.projectTxtDescrizioneCollocazione.Text;
            newFascicolo.varCodiceRubricaLF = this.projectTxtCodiceCollocazione.Text;
            newFascicolo.dtaLF = projectTxtdata.Text;
            newFascicolo.InAreaLavoro = "0";
            newFascicolo.apertura = System.DateTime.Now.ToString();

            return newFascicolo;
        }

        protected void NewProjectClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('NewProject', '');} else {parent.closeAjaxModal('NewProject', '');};", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void RblTypeNote_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.ddlNoteRF.Visible = false;
                this.txtNoteAutoComplete.Visible = false;

                ListItem item = this.RblTypeNote.Items.FindByValue("RF");
                //Se è presente il bottone di selezione esclusiva "RF" si verifica quanti sono gli
                //RF associati al ruolo dell'utente
                if (item != null && RblTypeNote.SelectedIndex == 2)
                {
                    //DocsPaWR.Registro[] registriRf = UserManager.getListaRegistriWithRF(RoleManager.GetRoleInSession().systemId, "1", "");
                    DocsPaWR.Registro[] registriRf = RegistryManager.GetRFListInSession();
                    //Se un ruolo appartiene a più di un RF, allora selezionando dal menù il valore RF
                    //l'utente deve selezionare su quale degli RF creare la nota
                    if (registriRf != null && registriRf.Length > 0)
                    {
                        //Se l'inserimento della nota avviene durante la protocollazione 
                        //ed è impostato nella segnatura il codice del RF, la selezione del RF dal quale
                        //prendere il codice sarà mantenuta valida anche per l'eventuale inserimento delle note
                        //in questo caso non si deve presentare la popup di selezione del RF
                        if (this.ddlNoteRF != null)
                            this.LoadNoteRF(registriRf);

                        this.txtNoteAutoComplete.Visible = false;

                        if (UserManager.IsAuthorizedFunctions("RICERCA_NOTE_ELENCO"))
                        {
                            this.txtNoteAutoComplete.Enabled = false;
                            this.txtNoteAutoComplete.Visible = true;
                            this.ddlNoteRF_SelectedIndexChanged(null, null);
                        }
                    }
                }
                this.UpPnlNote.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void LoadNoteRF(DocsPaWR.Registro[] listaRF)
        {
            try
            {
                this.txtNoteAutoComplete.Text = "";
                this.ddlNoteRF.Items.Clear();
                if (listaRF != null && listaRF.Length > 0)
                {
                    this.ddlNoteRF.Visible = true;
                    this.txtNoteAutoComplete.Visible = true;

                    if (listaRF.Length == 1)
                    {
                        ListItem item = new ListItem();
                        item.Value = listaRF[0].systemId;
                        item.Text = listaRF[0].codRegistro;
                        this.ddlNoteRF.Items.Add(item);
                        this.EnableNoteAutoComplete();
                    }
                    else
                    {
                        ListItem itemVuoto = new ListItem();
                        itemVuoto.Value = "";
                        itemVuoto.Text = Utils.Languages.GetLabelFromCode("DocumentNoteSelectAnRF", UIManager.UserManager.GetUserLanguage());
                        this.ddlNoteRF.Items.Add(itemVuoto);
                        foreach (DocsPaWR.Registro regis in listaRF)
                        {
                            ListItem item = new ListItem();
                            item.Value = regis.systemId;
                            item.Text = regis.codRegistro;
                            this.ddlNoteRF.Items.Add(item);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private void EnableNoteAutoComplete()
        {
            Session.Add("RFNote", "OK^" + this.ddlNoteRF.SelectedItem.Value + "^" + this.ddlNoteRF.SelectedItem.Text);
            this.txtNoteAutoComplete.Enabled = true;
            this.autoComplete1.ContextKey = ddlNoteRF.SelectedItem.Value;
            this.autoComplete1.MinimumPrefixLength = this.AutocompleteMinimumPrefixLength;
            this.txtNoteAutoComplete.Text = "";
        }

        protected void ddlNoteRF_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if ((this.ddlNoteRF.Items.Count > 1 && this.ddlNoteRF.SelectedIndex != 0) || (this.ddlNoteRF.Items.Count == 1))
                {
                    this.EnableNoteAutoComplete();
                    //this.TxtNote.Text = "";
                }
                else
                {
                    this.txtNoteAutoComplete.Text = "";
                    this.txtNoteAutoComplete.Enabled = false;
                    Session.Add("RFNote", "");
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtCodeCollocation_OnTextChanged(object sender, EventArgs e)
        {

            try
            {
                if (!string.IsNullOrEmpty(this.projectTxtCodiceCollocazione.Text))
                {
                    this.setDescCorrispondente(this.projectTxtCodiceCollocazione.Text, true);
                }
                else
                {
                    this.projectTxtCodiceCollocazione.Text = string.Empty;
                    this.projectTxtDescrizioneCollocazione.Text = string.Empty;
                    this.idProjectCollocation.Value = string.Empty;
                }

                this.UpProjectPhisycCollocation.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DocumentImgCollocationAddressBook_Click(object sender, EventArgs e)
        {
            try
            {
                this.CallType = RubricaCallType.CALLTYPE_EDITFASC_LOCFISICA;


                HttpContext.Current.Session["AddressBook.from"] = "COLLOCATION";

                //ScriptManager.RegisterStartupScript(this, this.GetType(), "AddressBook", "parent.ajaxModalPopupAddressBook();", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpProjectPhisycCollocation", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void setDescCorrispondente(string codiceRubrica, bool fineValidita)
        {
            try
            {
                DocsPaWR.Corrispondente corr = null;

                if (!string.IsNullOrEmpty(codiceRubrica))
                {
                    corr = AddressBookManager.GetCorrispondenteInterno(codiceRubrica, fineValidita);
                }

                if (corr != null && !string.IsNullOrEmpty(corr.descrizione) && corr.GetType().Equals(typeof(DocsPaWR.UnitaOrganizzativa)))
                {
                    this.projectTxtDescrizioneCollocazione.Text = corr.descrizione;
                    this.projectTxtCodiceCollocazione.Text = corr.codiceRubrica;
                    this.idProjectCollocation.Value = corr.systemId;
                }
                else
                {
                    this.projectTxtDescrizioneCollocazione.Text = string.Empty;
                    this.projectTxtCodiceCollocazione.Text = string.Empty;
                    this.idProjectCollocation.Value = string.Empty;
                    string msgDesc = "WarningDocumentCorrNotFound";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                }
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private void SaveNote(out string msg)
        {
            if (this.TxtNote.Text.Length > this.MaxLenghtNote)
            {
                msg = "ErrorDocumentNoteMaxLength";
                return;
            }
            else
            {
                // Se la nota non contiene testo, viene ripristinato il vecchio valore
                // e viene mostrato un messaggio di errore
                if (String.IsNullOrEmpty(this.TxtNote.Text.Trim()) && this.GetLastNote() != null)
                {
                    this.TxtNote.Text = this.GetLastNote().Testo;
                    msg = "ErrorDocumentNoteEmpty";
                    return;
                }

                DocsPaWR.Registro[] registriRf = RegistryManager.GetRFListInSession();
                // verifico se è stata selezionata una nota di RF e se si sia selezionato un RF corretto nel caso di utenti con 2 RF almeno
                if (registriRf != null && registriRf.Length > 1 && this.RblTypeNote.SelectedValue.Equals("RF") && string.IsNullOrEmpty(this.ddlNoteRF.SelectedValue))
                {
                    msg = "ErrorDocumentNoneRF";
                    return;
                }

                // Se i dati risultano modificati, viene creata una nuova nota
                this.InsertNote();
                this.FetchNote();
            }

            msg = string.Empty;
        }

        /// <summary>
        /// Creazione nuova nota a seguito di una modifica dei dati
        /// </summary>
        protected void InsertNote()
        {
            try
            {
                InfoNota nota = new InfoNota();

                if (this.RblTypeNote.SelectedItem != null)
                    nota.TipoVisibilita = (TipiVisibilitaNotaEnum)Enum.Parse(typeof(TipiVisibilitaNotaEnum), this.RblTypeNote.SelectedItem.Value, true);
                else
                    nota.TipoVisibilita = TipiVisibilitaNotaEnum.Tutti;

                nota.Testo = this.TxtNote.Text;

                if (nota.TipoVisibilita == TipiVisibilitaNotaEnum.RF)
                {
                    nota.IdRfAssociato = this.ddlNoteRF.SelectedValue;
                }

                // Se la nota contiene del testo (vengono eliminati anche i ritorni a capo ai lati della stringa)
                if (!String.IsNullOrEmpty(this.TxtNote.Text.Trim()) && (this.GetLastNote() == null || (this.GetLastNote() != null && this.TxtNote.Text.Trim() != this.GetLastNote().Testo)))
                    nota = this.InsertNote(nota);
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Caricamento dati
        /// </summary>
        public virtual void FetchNote()
        {
            // Reperimento ultima nota
            InfoNota nota = this.GetLastNote();

            if (nota != null)
            {
                // Impostazione dell'autore dell'ultima nota
                if (string.IsNullOrEmpty(this.GetAuthorLastNote(nota)))
                    this.projectLitNoteAuthor.Text = string.Empty;
                else
                    this.projectLitNoteAuthor.Text = "<br />" + Utils.Languages.GetLabelFromCode("DocumentNoteAuthor", UIManager.UserManager.GetUserLanguage()) + " " + this.GetAuthorLastNote(nota);

                this.RblTypeNote.SelectedValue = nota.TipoVisibilita.ToString();
                this.RblTypeNote_SelectedIndexChanged(null, null);
                if (nota.TipoVisibilita.ToString() == "RF") this.ddlNoteRF.SelectedValue = nota.IdRfAssociato;
                this.TxtNote.Text = nota.Testo;

                // Impostazione numero note visibili dall'utente corrente
                this.SetVisibleNote(this.CountNote());
            }
            else
            {
                this.ClearNote();
                this.SetVisibleNote(0);
            }

            this.UpPnlNote.Update();
        }

        /// <summary>
        /// Reperimento ultima nota inserita per un documento in ordine cronologico
        /// </summary>
        /// <returns></returns>
        private InfoNota GetLastNote()
        {
            try
            {
                InfoNota retValue = null;
                Fascicolo prg = UIManager.ProjectManager.getProjectInSession();
                if (prg != null && prg.noteFascicolo != null)
                {
                    foreach (InfoNota nota in prg.noteFascicolo)
                    {
                        if (!nota.DaRimuovere)
                        {
                            retValue = nota;
                            break;
                        }
                    }
                }

                return retValue;
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Reperimento dell'autore dell'ultima nota
        /// </summary>
        /// <param name="ultimaNota"></param>
        /// <returns></returns>
        protected string GetAuthorLastNote(InfoNota ultimaNota)
        {
            string autore = string.Empty;

            if (ultimaNota.UtenteCreatore != null)
            {

                if (!string.IsNullOrEmpty(ultimaNota.UtenteCreatore.DescrizioneUtente))
                    autore = ultimaNota.UtenteCreatore.DescrizioneUtente;

                if (autore != string.Empty && !string.IsNullOrEmpty(ultimaNota.UtenteCreatore.DescrizioneRuolo))
                    autore = string.Concat(autore, " (", ultimaNota.UtenteCreatore.DescrizioneRuolo, ")");

                if (!string.IsNullOrEmpty(ultimaNota.DescrPeopleDelegato))
                {
                    string temp = ultimaNota.DescrPeopleDelegato + "<br />" + Utils.Languages.GetLabelFromCode("DocumentNoteAuthorDelegatedBy", UIManager.UserManager.GetUserLanguage()) + " " + autore;
                    autore = temp;
                }
            }

            return autore;
        }

        public virtual void ClearNote()
        {
            this.SetVisibleNote(0);
            this.projectLitVisibleNotes.Text = string.Empty;
            this.projectLitNoteAuthor.Text = string.Empty;
            //this.RblTypeNote.SelectedValue = TipiVisibilitaNotaEnum.Tutti.ToString();
            this.TxtNote.Text = string.Empty;
            this.ddlNoteRF.Visible = false;
            this.txtNoteAutoComplete.Visible = false;
        }

        /// <summary>
        /// Impostazione messaggio di visibilità delle note
        /// </summary>
        /// <param name="countNote"></param>
        protected virtual void SetVisibleNote(int countNote)
        {
            string language = UIManager.UserManager.GetUserLanguage();

            if (countNote == 0)
                this.projectLitVisibleNotes.Text = Utils.Languages.GetLabelFromCode("DocumentNoteNoneVisible", language);
            else
            {
                string format = string.Empty;

                if (countNote == 1)
                    format = "{0} " + Utils.Languages.GetLabelFromCode("DocumentNoteVisibleOne", language);
                else
                    format = "{0} " + Utils.Languages.GetLabelFromCode("DocumentNoteVisibleMore", language);

                this.projectLitVisibleNotes.Text = string.Format(format, countNote.ToString());
            }
        }

        public int CountNote()
        {
            int count = 0;
            Fascicolo fasc = UIManager.ProjectManager.getProjectInSession();
            if (fasc != null)
            {
                foreach (InfoNota item in fasc.noteFascicolo)
                    if (!item.DaRimuovere)
                        count++;
            }
            return count;
        }

        /// <summary>
        /// Aggiornamento in batch delle sole note
        /// </summary>
        protected virtual void UpdateNote(Fascicolo fasc)
        {
            AssociazioneNota oggettoAssociato = new AssociazioneNota();
            oggettoAssociato.TipoOggetto = OggettiAssociazioniNotaEnum.Fascicolo;
            oggettoAssociato.Id = fasc.systemID;

            // Inserimento della nota creata
            string msg = string.Empty;
            this.SaveNote(out msg);
            if (!string.IsNullOrEmpty(msg))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'error');} else {parent.ajaxDialogModal('" + msg.Replace("'", "\\'") + "', 'error');}", true);
                return;
            }

            // Aggiornamento delle note sul backend
            fasc.noteFascicolo = UIManager.ProjectManager.getProjectInSession().noteFascicolo;
            fasc.noteFascicolo = DocumentManager.UpdateNote(oggettoAssociato, fasc.noteFascicolo);
        }

        /// <summary>
        /// Inserimento di una nuova nota da associare ad un documento / fascicolo
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        public InfoNota InsertNote(InfoNota nota)
        {
            try
            {
                nota.DaInserire = true;
                nota.Id = Guid.NewGuid().ToString();
                nota.DataCreazione = DateTime.Now;
                nota.UtenteCreatore = new InfoUtenteCreatoreNota();

                InfoUtente utente = UserManager.GetInfoUser();
                nota.UtenteCreatore.IdUtente = utente.idPeople;
                nota.UtenteCreatore.DescrizioneUtente = utente.userId;
                if (utente.delegato != null)
                {
                    nota.IdPeopleDelegato = utente.delegato.idPeople;
                    nota.DescrPeopleDelegato = utente.delegato.userId;
                }
                Ruolo ruolo = RoleManager.GetRoleInSession();
                nota.UtenteCreatore.IdRuolo = ruolo.idGruppo;
                nota.UtenteCreatore.DescrizioneRuolo = ruolo.descrizione;

                Fascicolo prj = UIManager.ProjectManager.getProjectInSession();

                // Inserimento della nota nella scheda documento (come primo elemento della lista, 
                // solo se il testo della nota da inserire ed il tipo di visibilità sono differenti
                // da quelli dell'ultima nota inserita)
                if (!String.IsNullOrEmpty(nota.Testo.Trim()) && prj != null &&
                    (prj.noteFascicolo == null || prj.noteFascicolo.Length == 0 ||
                    !prj.noteFascicolo[0].Testo.Trim().Equals(nota.Testo.Trim())
                    || !prj.noteFascicolo[0].TipoVisibilita.Equals(nota.TipoVisibilita)))
                {
                    List<InfoNota> note = null;
                    if (prj.noteFascicolo == null)
                    {
                        note = new List<InfoNota>();
                    }
                    else
                    {
                        note = new List<InfoNota>(prj.noteFascicolo);
                    }

                    note.Insert(0, nota);
                    prj.noteFascicolo = note.ToArray();
                    UIManager.ProjectManager.setProjectInSession(prj);
                }

                return nota;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        #region tipologia fascicoli
        protected void ProjectDdlTypeDocument_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            if (!string.IsNullOrEmpty(this.projectDdlTipologiafascicolo.SelectedValue))
            {
                if (this.CustomProject)
                {

                    this.TemplateProject = ProfilerProjectManager.getTemplateFascById(this.projectDdlTipologiafascicolo.SelectedItem.Value);
                    if (this.TemplateProject != null)
                    {
                        if (this.TemplateProject.PRIVATO != null && this.TemplateProject.PRIVATO == "1")
                        {
                            this.ProjectCheckPrivate.Checked = true;
                            this.UpProjectPrivate.Update();
                        }

                        Fascicolo fasc = ProjectManager.getProjectInSession();
                        string dtaScadenza = string.Empty;
                        if (this.EnableStateDiagram)
                        {
                            this.DocumentDdlStateDiagram.ClearSelection();
                            //Verifico se esiste un diagramma di stato associato al tipo di documento
                            this.StateDiagram = DiagrammiManager.getDgByIdTipoFasc(this.projectDdlTipologiafascicolo.SelectedItem.Value, UIManager.UserManager.GetInfoUser().idAmministrazione);
                            if (this.StateDiagram != null)
                            {
                                this.PnlStateDiagram.Visible = true;
                                this.popolaComboBoxStatiSuccessivi(null, this.StateDiagram);

                                if (this.TemplateProject != null && !string.IsNullOrEmpty(this.TemplateProject.SCADENZA) && this.TemplateProject.SCADENZA != "0")
                                {
                                    if (fasc != null)
                                    {
                                        dtaScadenza = fasc.dtaScadenza;
                                    }
                                    this.PnlDocumentStateDiagramDate.Visible = true;
                                    this.PnlScadenza.Visible = true;
                                    this.DocumentStateDiagramDataValue.Text = dtaScadenza;

                                    DateTime dataOdierna = System.DateTime.Now;
                                    int scadenza = Convert.ToInt32(this.TemplateProject.SCADENZA);
                                    DateTime dataCalcolata = dataOdierna.AddDays(scadenza);
                                    this.PnlScadenza.Visible = true;
                                    if (string.IsNullOrEmpty(dtaScadenza))
                                    {
                                        this.DocumentStateDiagramDataValue.Text = utils.formatDataDocsPa(dataCalcolata);

                                        //this.DocumentInWorking.dataScadenza = Utils.formatDataDocsPa(dataCalcolata);
                                    }
                                    else
                                    {
                                        this.DocumentStateDiagramDataValue.Text = dtaScadenza.Substring(0, 10);
                                        this.DocumentStateDiagramDataValue.ReadOnly = true;
                                    }

                                }
                                else
                                {
                                    this.PnlDocumentStateDiagramDate.Visible = false;
                                    this.PnlScadenza.Visible = false;
                                    this.DocumentStateDiagramDataValue.Text = string.Empty;
                                }
                            }
                            else
                            {
                                this.PnlStateDiagram.Visible = false;
                            }
                        }
                    }
                }
                this.PopulateProfiledDocument();
            }
            else
            {
                this.TemplateProject = null;
                //this.ProjectImgHistoryTipology.Visible = false;
                if (this.EnableStateDiagram)
                {
                    this.DocumentDdlStateDiagram.ClearSelection();
                    this.PnlStateDiagram.Visible = false;
                }
                this.PnlTypeDocument.Controls.Clear();
            }

            this.UpPnlTypeDocument.Update();
        }

        private void popolaComboBoxStatiSuccessivi(DocsPaWR.Stato stato, DocsPaWR.DiagrammaStato diagramma)
        {
            List<string> idStatiSuccessivi = new List<string>();
            int selectedIndex = this.DocumentDdlStateDiagram.SelectedIndex;
            //Inizializzazione
            this.DocumentDdlStateDiagram.Items.Clear();
            ListItem itemEmpty = new ListItem();
            this.DocumentDdlStateDiagram.Items.Add(itemEmpty);

            //Popola la combo con gli stati iniziali del diagramma
            if (stato == null)
            {
                this.LitActualStateDiagram.Text = string.Empty;
                for (int i = 0; i < diagramma.STATI.Length; i++)
                {
                    DocsPaWR.Stato st = (DocsPaWR.Stato)diagramma.STATI[i];

                    if (st.STATO_INIZIALE && DiagrammiManager.IsRuoloAssociatoStatoDia(diagramma.SYSTEM_ID.ToString(), UIManager.RoleManager.GetRoleInSession().idGruppo, st.SYSTEM_ID.ToString()))
                    {
                        ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                        this.DocumentDdlStateDiagram.Items.Add(item);
                        if (st.STATO_SISTEMA)
                            item.Enabled = false;
                    }
                }
                if (this.DocumentDdlStateDiagram.Items.Count == 2)
                    selectedIndex = 1;
            }
            //Popola la combo con i possibili stati, successivi a quello passato
            else
            {
                for (int i = 0; i < diagramma.PASSI.Length; i++)
                {
                    DocsPaWR.Passo step = (DocsPaWR.Passo)diagramma.PASSI[i];
                    if (step.STATO_PADRE.SYSTEM_ID == stato.SYSTEM_ID)
                    {
                        for (int j = 0; j < step.SUCCESSIVI.Length; j++)
                        {
                            DocsPaWR.Stato st = (DocsPaWR.Stato)step.SUCCESSIVI[j];
                            if (DiagrammiManager.IsRuoloAssociatoStatoDia(diagramma.SYSTEM_ID.ToString(), UIManager.RoleManager.GetRoleInSession().idGruppo, st.SYSTEM_ID.ToString()))
                            {
                                ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                                if (st.STATO_SISTEMA)
                                    item.Enabled = false;
                                this.DocumentDdlStateDiagram.Items.Add(item);
                                idStatiSuccessivi.Add(st.SYSTEM_ID.ToString());
                            }
                        }
                    }
                }
            }

            if (selectedIndex < this.DocumentDdlStateDiagram.Items.Count)
                this.DocumentDdlStateDiagram.SelectedIndex = selectedIndex;
        }

        private void PopulateProfiledDocument()
        {
            this.PnlTypeDocument.Controls.Clear();
            this.inserisciComponenti(true);
        }
        private void inserisciComponenti(bool readOnly)
        {
            this.RemovePropertySearchCorrespondentIntExtWithDisabled();
            if (this.TemplateProject.OLD_OGG_CUSTOM.Length < 1)
            {
                this.TemplateProject.OLD_OGG_CUSTOM = new StoricoProfilatiOldValue[this.TemplateProject.ELENCO_OGGETTI.Length];
            }

            ArrayList dirittiCampiRuolo = ProfilerProjectManager.getDirittiCampiTipologiaFasc(UIManager.RoleManager.GetRoleInSession().idGruppo, this.TemplateProject.SYSTEM_ID.ToString());

            for (int i = 0, index = 0; i < this.TemplateProject.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)this.TemplateProject.ELENCO_OGGETTI[i];

                ProfilerProjectManager.addNoRightsCustomObject(dirittiCampiRuolo, oggettoCustom);

                switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                {
                    case "CampoDiTesto":
                        this.inserisciCampoDiTesto(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "CasellaDiSelezione":
                        this.inserisciCasellaDiSelezione(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "MenuATendina":
                        this.inserisciMenuATendina(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "SelezioneEsclusiva":
                        this.inserisciSelezioneEsclusiva(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "Contatore":
                        this.inserisciContatore(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "Data":
                        this.inserisciData(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "Corrispondente":
                        this.inserisciCorrispondente(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "Link":
                        this.inserisciLink(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "ContatoreSottocontatore":
                        this.inserisciContatoreSottocontatore(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "OggettoEsterno":
                        this.inserisciOggettoEsterno(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "Separatore":
                        this.inserisciCampoSeparatore(oggettoCustom);
                        break;
                }

            }

            //controlla che vi sia almeno un campo visibile per attivare il pulsante per lo storico
            int btn_HistoryIsVisible = 0;
            foreach (AssDocFascRuoli diritti in dirittiCampiRuolo)
            {
                if (!diritti.VIS_OGG_CUSTOM.Equals("0"))
                    ++btn_HistoryIsVisible;
            }
        }

        public void inserisciOggettoEsterno(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            Label etichetta = new Label();
            etichetta.EnableViewState = true;

            if ("SI".Equals(oggettoCustom.CAMPO_OBBLIGATORIO))
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE;
            }
            etichetta.CssClass = "weight";
            UserControls.IntegrationAdapter intAd = (UserControls.IntegrationAdapter)this.LoadControl("../UserControls/IntegrationAdapter.ascx");
            intAd.ID = oggettoCustom.SYSTEM_ID.ToString();
            intAd.View = UserControls.IntegrationAdapterView.INSERT_MODIFY;
            intAd.ManualInsertCssClass = "txt_textdata_counter_disabled_red";
            intAd.EnableViewState = true;
            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, intAd, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);
            //if ((this.DocumentInWorking != null && !string.IsNullOrEmpty(this.DocumentInWorking.systemId) && !string.IsNullOrEmpty(this.DocumentInWorking.accessRights)) && ((this.DocumentInWorking.accessRights == "45" || readOnly)))
            //    intAd.View = UserControls.IntegrationAdapterView.READ_ONLY;
            intAd.ConfigurationValue = oggettoCustom.CONFIG_OBJ_EST;
            IntegrationAdapterValue value = new IntegrationAdapterValue(oggettoCustom.CODICE_DB, oggettoCustom.VALORE_DATABASE, oggettoCustom.MANUAL_INSERT);
            intAd.Value = value;

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;

            if (etichetta.Visible)
            {
                HtmlGenericControl parDesc = new HtmlGenericControl("p");
                parDesc.Controls.Add(etichetta);
                parDesc.EnableViewState = true;
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (intAd.Visible)
            {
                divColValue.Controls.Add(intAd);
                divRowValue.Controls.Add(divColValue);
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }

        }

        private void inserisciCampoDiTesto(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            DocsPaWR.StoricoProfilatiOldValue oldObjText = new StoricoProfilatiOldValue();

            Label etichettaCampoDiTesto = new Label();
            etichettaCampoDiTesto.EnableViewState = true;

            CustomTextArea txt_CampoDiTesto = new CustomTextArea();
            txt_CampoDiTesto.EnableViewState = true;

            if (oggettoCustom.MULTILINEA.Equals("SI"))
            {
                if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                {
                    etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE + " *";
                }
                else
                {
                    etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;
                }
                etichettaCampoDiTesto.CssClass = "weight";

                txt_CampoDiTesto.CssClass = "txt_textarea";
                txt_CampoDiTesto.CssClassReadOnly = "txt_textarea_disabled";

                if (string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_LINEE))
                {
                    txt_CampoDiTesto.Height = 55;
                }
                else
                {
                    txt_CampoDiTesto.Rows = Convert.ToInt32(oggettoCustom.NUMERO_DI_LINEE);
                }

                if (string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_CARATTERI))
                {
                    txt_CampoDiTesto.MaxLength = 150;
                }
                else
                {
                    txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                }

                txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;
                txt_CampoDiTesto.TextMode = TextBoxMode.MultiLine;


            }
            else
            {
                if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                {
                    etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE + " *";
                }
                else
                {
                    etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;
                }
                etichettaCampoDiTesto.CssClass = "weight";

                if (!string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_CARATTERI))
                {
                    //ATTENZIONE : La lunghezza della textBox non è speculare al numero massimo di
                    //caratteri che l'utente inserisce.
                    if (((Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI) * 6) <= 400))
                    {
                        txt_CampoDiTesto.Width = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI) * 6;
                    }
                    txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                }
                txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;
                txt_CampoDiTesto.CssClass = "txt_input_full";
                txt_CampoDiTesto.CssClassReadOnly = "txt_input_full_disabled";
                txt_CampoDiTesto.TextMode = TextBoxMode.SingleLine;


            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;


            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaCampoDiTesto, txt_CampoDiTesto, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && (ProjectManager.getProjectInSession().accessRights == "45"  || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C"))))
            {
                txt_CampoDiTesto.ReadOnly = true;
            }


            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID))
            {
                //txt_CampoDiTesto.Attributes.Add("onClick", "window.document.getElementById('" + txt_CampoDiTesto.ClientID + "').focus();");
                if (this.TemplateProject.OLD_OGG_CUSTOM[index] == null)//se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
                {
                    //blocco storico profilazione campi di testo 
                    //salvo il valore corrente del campo di testo in oldObjCustom.
                    oldObjText.IDTemplate = this.TemplateProject.ID_TIPO_ATTO;
                    oldObjText.ID_Doc_Fasc = ProjectManager.getProjectInSession().systemID;
                    oldObjText.ID_Oggetto = oggettoCustom.SYSTEM_ID.ToString();
                    oldObjText.Valore = oggettoCustom.VALORE_DATABASE;
                    oldObjText.Tipo_Ogg_Custom = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                    InfoUtente user = UserManager.GetInfoUser();
                    oldObjText.ID_People = user.idPeople;
                    oldObjText.ID_Ruolo_In_UO = user.idCorrGlobali;
                    this.TemplateProject.OLD_OGG_CUSTOM[index] = oldObjText;
                }
            }

            if (etichettaCampoDiTesto.Visible)
            {
                HtmlGenericControl parDesc = new HtmlGenericControl("p");
                parDesc.Controls.Add(etichettaCampoDiTesto);
                parDesc.EnableViewState = true;
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (txt_CampoDiTesto.Visible)
            {
                divColValue.Controls.Add(txt_CampoDiTesto);
                divRowValue.Controls.Add(divColValue);
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }


        }

        private void inserisciContatoreSottocontatore(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, ArrayList dirittiCampiRuolo)
        {
            bool paneldll = false;

            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                return;

            Label etichettaContatoreSottocontatore = new Label();
            etichettaContatoreSottocontatore.EnableViewState = true;
            etichettaContatoreSottocontatore.Text = oggettoCustom.DESCRIZIONE;
            etichettaContatoreSottocontatore.CssClass = "weight";

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaContatoreSottocontatore);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);



            //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
            //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
            //del contatore come da formato prescelto e in readOnly
            Label etichettaDDL = new Label();
            etichettaDDL.EnableViewState = true;
            DropDownList ddl = new DropDownList();
            ddl.EnableViewState = true;

            string language = UIManager.UserManager.GetUserLanguage();
            ddl.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));

            Panel divRowDll = new Panel();
            divRowDll.CssClass = "row";
            divRowDll.EnableViewState = true;

            Panel divRowEtiDll = new Panel();
            divRowEtiDll.CssClass = "row";
            divRowEtiDll.EnableViewState = true;

            HtmlGenericControl parDll = new HtmlGenericControl("p");
            parDll.EnableViewState = true;

            if (string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                Ruolo ruoloUtente = RoleManager.GetRoleInSession();
                Registro[] registriRfVisibili = UIManager.RegistryManager.GetListRegistriesAndRF(ruoloUtente.systemId, string.Empty, string.Empty);

                Panel divColDllEti = new Panel();
                divColDllEti.CssClass = "col";
                divColDllEti.EnableViewState = true;

                Panel divColDll = new Panel();
                divColDll.CssClass = "col";
                divColDll.EnableViewState = true;

                switch (oggettoCustom.TIPO_CONTATORE)
                {
                    case "T":
                        break;
                    case "A":
                        paneldll = true;
                        etichettaDDL.Text = "&nbsp;AOO&nbsp;";
                        etichettaDDL.CssClass = "weight";
                        etichettaDDL.Width = 50;
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "chzn-select-deselect";
                        ddl.Width = 240;

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "0" && !((Registro)registriRfVisibili[i]).Sospeso)
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Text = string.Empty;
                            it.Value = string.Empty;
                            ddl.Items.Add(it);
                            ddl.SelectedValue = string.Empty;
                        }
                        else
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                        if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45")
                        {
                            ddl.Enabled = false;
                        }

                        parDll.Controls.Add(etichettaDDL);
                        divColDllEti.Controls.Add(parDll);
                        divRowEtiDll.Controls.Add(divColDllEti);

                        divColDll.Controls.Add(ddl);
                        divRowDll.Controls.Add(divColDll);
                        break;
                    case "R":
                        paneldll = true;
                        etichettaDDL.Text = "&nbsp;RF&nbsp;";
                        etichettaDDL.CssClass = "weight";
                        etichettaDDL.Width = 50;
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "chzn-select-deselect";
                        ddl.Width = 240;

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "1" && ((Registro)registriRfVisibili[i]).rfDisabled == "0")
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Value = string.Empty;
                            it.Text = string.Empty;
                            ddl.Items.Add(it);
                            ddl.SelectedValue = string.Empty;
                        }
                        else
                        {
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                        }
                        if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45")
                        {
                            ddl.Enabled = false;
                        }

                        parDll.Controls.Add(etichettaDDL);
                        divColDllEti.Controls.Add(parDll);
                        divRowEtiDll.Controls.Add(divColDllEti);

                        divColDll.Controls.Add(ddl);
                        divRowDll.Controls.Add(divColDll);
                        break;
                }
            }

            //Imposto il contatore in funzione del formato
            CustomTextArea contatore = new CustomTextArea();
            CustomTextArea sottocontatore = new CustomTextArea();
            CustomTextArea dataInserimentoSottocontatore = new CustomTextArea();
            contatore.EnableViewState = true;
            sottocontatore.EnableViewState = true;
            dataInserimentoSottocontatore.EnableViewState = true;
            //contatore.Attributes.Add("onClick", "window.document.getElementById('" + contatore.ClientID + "').focus();");
            //sottocontatore.Attributes.Add("onClick", "window.document.getElementById('" + sottocontatore.ClientID + "').focus();");
            //dataInserimentoSottocontatore.Attributes.Add("onClick", "window.document.getElementById('" + dataInserimentoSottocontatore.ClientID + "').focus();");
            contatore.ID = oggettoCustom.SYSTEM_ID.ToString();
            sottocontatore.ID = oggettoCustom.SYSTEM_ID.ToString() + "_sottocontatore";
            dataInserimentoSottocontatore.ID = oggettoCustom.SYSTEM_ID.ToString() + "_dataSottocontatore";
            if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
            {
                contatore.Text = oggettoCustom.FORMATO_CONTATORE;
                sottocontatore.Text = oggettoCustom.FORMATO_CONTATORE;
                //if (Session["templateRiproposto"] != null)
                //{
                //    contatore.Text = contatore.Text.Replace("ANNO", "");
                //    contatore.Text = contatore.Text.Replace("CONTATORE", "");
                //    contatore.Text = contatore.Text.Replace("RF", "");
                //    contatore.Text = contatore.Text.Replace("AOO", "");

                //    sottocontatore.Text = sottocontatore.Text.Replace("ANNO", "");
                //    sottocontatore.Text = sottocontatore.Text.Replace("CONTATORE", "");
                //    sottocontatore.Text = sottocontatore.Text.Replace("RF", "");
                //    sottocontatore.Text = sottocontatore.Text.Replace("AOO", "");
                //}
                //else
                //{
                if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE) && !string.IsNullOrEmpty(oggettoCustom.VALORE_SOTTOCONTATORE))
                {
                    contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO);
                    contatore.Text = contatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);

                    sottocontatore.Text = sottocontatore.Text.Replace("ANNO", oggettoCustom.ANNO);
                    sottocontatore.Text = sottocontatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_SOTTOCONTATORE);

                    if (!string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) && oggettoCustom.ID_AOO_RF != "0")
                    {
                        Registro reg = UserManager.getRegistroBySistemId(this, oggettoCustom.ID_AOO_RF);
                        if (reg != null)
                        {
                            contatore.Text = contatore.Text.Replace("RF", reg.codRegistro);
                            contatore.Text = contatore.Text.Replace("AOO", reg.codRegistro);

                            sottocontatore.Text = sottocontatore.Text.Replace("RF", reg.codRegistro);
                            sottocontatore.Text = sottocontatore.Text.Replace("AOO", reg.codRegistro);
                        }
                    }
                }
                else
                {
                    contatore.Text = contatore.Text.Replace("ANNO", "");
                    contatore.Text = contatore.Text.Replace("CONTATORE", "");
                    contatore.Text = contatore.Text.Replace("RF", "");
                    contatore.Text = contatore.Text.Replace("AOO", "");

                    sottocontatore.Text = sottocontatore.Text.Replace("ANNO", "");
                    sottocontatore.Text = sottocontatore.Text.Replace("CONTATORE", "");
                    sottocontatore.Text = sottocontatore.Text.Replace("RF", "");
                    sottocontatore.Text = sottocontatore.Text.Replace("AOO", "");
                }
                //}
            }
            else
            {
                contatore.Text = oggettoCustom.VALORE_DATABASE;
                sottocontatore.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
            }

            Panel divRowCounter = new Panel();
            divRowCounter.CssClass = "row";
            divRowCounter.EnableViewState = true;

            Panel divColCountCounter = new Panel();
            divColCountCounter.CssClass = "col_full";
            divColCountCounter.EnableViewState = true;
            divColCountCounter.Controls.Add(contatore);
            divColCountCounter.Controls.Add(sottocontatore);
            divRowCounter.Controls.Add(divColCountCounter);

            if (!string.IsNullOrEmpty(oggettoCustom.DATA_INSERIMENTO))
            {
                dataInserimentoSottocontatore.Text = oggettoCustom.DATA_INSERIMENTO;

                Panel divColCountAbort = new Panel();
                divColCountAbort.CssClass = "col";
                divColCountAbort.EnableViewState = true;
                divColCountAbort.Controls.Add(dataInserimentoSottocontatore);
                divRowCounter.Controls.Add(divColCountAbort);
            }

            CheckBox cbContaDopo = new CheckBox();
            cbContaDopo.EnableViewState = true;

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloContatoreSottocontatore(etichettaContatoreSottocontatore, contatore, sottocontatore, dataInserimentoSottocontatore, cbContaDopo, etichettaDDL, ddl, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);

            if (etichettaContatoreSottocontatore.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            contatore.ReadOnly = true;
            contatore.CssClass = "txt_input_half";
            contatore.CssClassReadOnly = "txt_input_half_disabled";

            sottocontatore.ReadOnly = true;
            sottocontatore.CssClass = "txt_input_half";
            sottocontatore.CssClassReadOnly = "txt_input_half_disabled";

            dataInserimentoSottocontatore.ReadOnly = true;
            dataInserimentoSottocontatore.CssClass = "txt_input_full";
            dataInserimentoSottocontatore.CssClassReadOnly = "txt_input_full_disabled";
            dataInserimentoSottocontatore.Visible = false;


            //Inserisco il cb per il conta dopo
            if (oggettoCustom.CONTA_DOPO == "1")
            {
                cbContaDopo.ID = oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo";
                cbContaDopo.Checked = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                cbContaDopo.ToolTip = "Attiva / Disattiva incremento del contatore al salvataggio dei dati.";


                if ((!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE)) || ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45")
                {
                    cbContaDopo.Checked = false;
                    cbContaDopo.Visible = false;
                    cbContaDopo.Enabled = false;
                }

                Panel divColCountAfter = new Panel();
                divColCountAfter.CssClass = "col";
                divColCountAfter.EnableViewState = true;
                divColCountAfter.Controls.Add(cbContaDopo);
                divRowDll.Controls.Add(divColCountAfter);
            }

            if (paneldll)
            {
                this.PnlTypeDocument.Controls.Add(divRowEtiDll);
                this.PnlTypeDocument.Controls.Add(divRowDll);
            }

            if (contatore.Visible || sottocontatore.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowCounter);
            }
        }

        private void inserisciCasellaDiSelezione(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            DocsPaWR.StoricoProfilatiOldValue casellaSelOldObj = new StoricoProfilatiOldValue();
            Label etichettaCasellaSelezione = new Label();
            etichettaCasellaSelezione.EnableViewState = true;

            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE;
            }

            etichettaCasellaSelezione.Width = Unit.Percentage(100);
            etichettaCasellaSelezione.CssClass = "weight";

            CheckBoxList casellaSelezione = new CheckBoxList();
            casellaSelezione.EnableViewState = true;
            casellaSelezione.ID = oggettoCustom.SYSTEM_ID.ToString();
            int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreElenco = ((ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                if (i < oggettoCustom.VALORI_SELEZIONATI.Length)
                {
                    string valoreSelezionato = (string)(oggettoCustom.VALORI_SELEZIONATI[i]);
                    if (valoreElenco.ABILITATO == 1 || (valoreElenco.ABILITATO == 0 && !string.IsNullOrEmpty(valoreSelezionato)))
                    {
                        //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                        if (valoreElenco.ABILITATO == 0 && !string.IsNullOrEmpty(valoreSelezionato))
                            valoreElenco.ABILITATO = 1;

                        casellaSelezione.Items.Add(new ListItem(valoreElenco.VALORE, valoreElenco.VALORE));
                        //Valore di default
                        if (valoreElenco.VALORE_DI_DEFAULT.Equals("SI"))
                        {
                            valoreDiDefault = i;
                        }
                    }
                }
            }

            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                casellaSelezione.RepeatDirection = RepeatDirection.Horizontal;
            }
            else
            {
                casellaSelezione.RepeatDirection = RepeatDirection.Vertical;
            }
            if (valoreDiDefault != -1)
            {
                casellaSelezione.SelectedIndex = valoreDiDefault;
            }

            if (oggettoCustom.VALORI_SELEZIONATI != null)
            {
                this.impostaSelezioneCaselleDiSelezione(oggettoCustom, casellaSelezione);
            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaCasellaSelezione);
            parDesc.EnableViewState = true;



            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColDesc.EnableViewState = true;



            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaCasellaSelezione, casellaSelezione, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45")
            {
                casellaSelezione.Enabled = false;
            }

            if (etichettaCasellaSelezione.Visible)
            {
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (casellaSelezione.Visible)
            {

                divColValue.Controls.Add(casellaSelezione);
                divRowValue.Controls.Add(divColValue);

                this.PnlTypeDocument.Controls.Add(divRowValue);
            }

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID))
            {
                if (this.TemplateProject.OLD_OGG_CUSTOM[index] == null) //se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
                {
                    //blocco storico profilazione casella di selezione 
                    casellaSelOldObj.Tipo_Ogg_Custom = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                    casellaSelOldObj.ID_Oggetto = oggettoCustom.SYSTEM_ID.ToString();
                    //per questo oggetto faccio un merge dei valori selezionati e salvo la stringa nel db
                    for (int x = 0; x < oggettoCustom.VALORI_SELEZIONATI.Length; x++)
                    {
                        if (!string.IsNullOrEmpty(oggettoCustom.VALORI_SELEZIONATI[x]))
                            casellaSelOldObj.Valore += string.IsNullOrEmpty(casellaSelOldObj.Valore) ?
                                oggettoCustom.VALORI_SELEZIONATI[x] : "*#?" + oggettoCustom.VALORI_SELEZIONATI[x];
                    }
                    InfoUtente user = UserManager.GetInfoUser();
                    casellaSelOldObj.IDTemplate = this.TemplateProject.ID_TIPO_ATTO;
                    casellaSelOldObj.ID_Doc_Fasc = ProjectManager.getProjectInSession().systemID;
                    casellaSelOldObj.ID_People = user.idPeople;
                    casellaSelOldObj.ID_Ruolo_In_UO = user.idCorrGlobali;
                    this.TemplateProject.OLD_OGG_CUSTOM[index] = casellaSelOldObj;
                }
            }
        }

        private void inserisciMenuATendina(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            DocsPaWR.StoricoProfilatiOldValue menuOldObj = new StoricoProfilatiOldValue();
            Label etichettaMenuATendina = new Label();
            etichettaMenuATendina.EnableViewState = true;
            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE;
            }
            etichettaMenuATendina.CssClass = "weight";

            int maxLenght = 0;
            DropDownList menuATendina = new DropDownList();
            menuATendina.EnableViewState = true;
            menuATendina.ID = oggettoCustom.SYSTEM_ID.ToString();
            int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                //Valori disabilitati/abilitati
                if (valoreOggetto.ABILITATO == 1 || (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE))
                {
                    //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                    if (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE)
                        valoreOggetto.ABILITATO = 1;

                    menuATendina.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                    //Valore di default
                    if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                    {
                        valoreDiDefault = i;
                    }

                    if (maxLenght < valoreOggetto.VALORE.Length)
                    {
                        maxLenght = valoreOggetto.VALORE.Length;
                    }
                }
            }
            menuATendina.CssClass = "chzn-select-deselect";
            string language = UIManager.UserManager.GetUserLanguage();
            menuATendina.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));
            menuATendina.Width = maxLenght + 250;

            if (valoreDiDefault != -1)
            {
                menuATendina.SelectedIndex = valoreDiDefault;
            }
            if (!(valoreDiDefault != -1 && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI")))
            {
                menuATendina.Items.Insert(0, "");
            }
            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                menuATendina.SelectedIndex = this.impostaSelezioneMenuATendina(oggettoCustom.VALORE_DATABASE, menuATendina);
            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaMenuATendina);
            parDesc.EnableViewState = true;



            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;




            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaMenuATendina, menuATendina, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);

            if (etichettaMenuATendina.Visible)
            {
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }


            if (menuATendina.Visible)
            {
                divColValue.Controls.Add(menuATendina);
                divRowValue.Controls.Add(divColValue);
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }




            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45")
            {
                menuATendina.Enabled = false;
            }

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID))
            {
                if (this.TemplateProject.OLD_OGG_CUSTOM[index] == null)//se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
                {
                    //blocco storico profilazione del campo menu di selezione 
                    //salvo il valore corrente del campo menu di selezione in oldObjCustom.
                    menuOldObj.Tipo_Ogg_Custom = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                    menuOldObj.ID_Oggetto = oggettoCustom.SYSTEM_ID.ToString();
                    menuOldObj.Valore = oggettoCustom.VALORE_DATABASE;
                    InfoUtente user = UserManager.GetInfoUser();
                    menuOldObj.ID_People = user.idPeople;
                    menuOldObj.ID_Ruolo_In_UO = user.idCorrGlobali;
                    menuOldObj.IDTemplate = this.TemplateProject.ID_TIPO_ATTO;
                    Fascicolo fasc = ProjectManager.getProjectInSession();
                    menuOldObj.ID_Doc_Fasc = fasc.systemID;
                    this.TemplateProject.OLD_OGG_CUSTOM[index] = menuOldObj;
                }
            }
        }

        private void inserisciSelezioneEsclusiva(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            DocsPaWR.StoricoProfilatiOldValue selezEsclOldObj = new StoricoProfilatiOldValue();
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            Label etichettaSelezioneEsclusiva = new Label();
            etichettaSelezioneEsclusiva.EnableViewState = true;
            CustomImageButton cancella_selezioneEsclusiva = new CustomImageButton();
            string language = UIManager.UserManager.GetUserLanguage();
            cancella_selezioneEsclusiva.AlternateText = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Reset", language);
            cancella_selezioneEsclusiva.ToolTip = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Reset", language);
            cancella_selezioneEsclusiva.EnableViewState = true;

            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE;
            }

            cancella_selezioneEsclusiva.ID = "_" + oggettoCustom.SYSTEM_ID.ToString();
            cancella_selezioneEsclusiva.ImageUrl = "../Images/Icons/clean_field_custom.png";
            cancella_selezioneEsclusiva.OnMouseOutImage = "../Images/Icons/clean_field_custom.png";
            cancella_selezioneEsclusiva.OnMouseOverImage = "../Images/Icons/clean_field_custom_hover.png";
            cancella_selezioneEsclusiva.ImageUrlDisabled = "../Images/Icons/clean_field_custom_disabled.png";
            cancella_selezioneEsclusiva.CssClass = "clickable";
            cancella_selezioneEsclusiva.Click += cancella_selezioneEsclusiva_Click;
            etichettaSelezioneEsclusiva.CssClass = "weight";

            RadioButtonList selezioneEsclusiva = new RadioButtonList();
            selezioneEsclusiva.EnableViewState = true;
            selezioneEsclusiva.ID = oggettoCustom.SYSTEM_ID.ToString();
            int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                //Valori disabilitati/abilitati
                if (valoreOggetto.ABILITATO == 1 || (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE))
                {
                    //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                    if (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE)
                        valoreOggetto.ABILITATO = 1;

                    selezioneEsclusiva.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                    //Valore di default
                    if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                    {
                        valoreDiDefault = i;
                    }
                }
            }

            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                selezioneEsclusiva.RepeatDirection = RepeatDirection.Horizontal;
            }
            else
            {
                selezioneEsclusiva.RepeatDirection = RepeatDirection.Vertical;
            }
            if (valoreDiDefault != -1)
            {
                selezioneEsclusiva.SelectedIndex = valoreDiDefault;
            }
            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                selezioneEsclusiva.SelectedIndex = impostaSelezioneEsclusiva(oggettoCustom.VALORE_DATABASE, selezioneEsclusiva);
            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            Panel divColImage = new Panel();
            divColImage.CssClass = "col-right-no-margin";
            divColImage.EnableViewState = true;

            divColImage.Controls.Add(cancella_selezioneEsclusiva);

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaSelezioneEsclusiva);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);

            divRowDesc.Controls.Add(divColDesc);
            divRowDesc.Controls.Add(divColImage);


            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;

            divColValue.Controls.Add(selezioneEsclusiva);
            divRowValue.Controls.Add(divColValue);



            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSelezioneEsclusiva(etichettaSelezioneEsclusiva, selezioneEsclusiva, cancella_selezioneEsclusiva, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);

            if (etichettaSelezioneEsclusiva.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (selezioneEsclusiva.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45")
            {
                selezioneEsclusiva.Enabled = false;
                cancella_selezioneEsclusiva.Enabled = false;
            }

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID))
            {
                if (this.TemplateProject.OLD_OGG_CUSTOM[index] == null) //se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
                {
                    //blocco storico profilazione del campo di selezione esclusiva 
                    //salvo il valore corrente del campo di selezione esclusiva in oldObjCustom.
                    selezEsclOldObj.IDTemplate = this.TemplateProject.ID_TIPO_ATTO;
                    Fascicolo fasc = ProjectManager.getProjectInSession();
                    selezEsclOldObj.ID_Doc_Fasc = fasc.systemID;
                    selezEsclOldObj.ID_Oggetto = oggettoCustom.SYSTEM_ID.ToString();
                    selezEsclOldObj.Tipo_Ogg_Custom = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                    InfoUtente user = UserManager.GetInfoUser();
                    selezEsclOldObj.ID_People = user.idPeople;
                    selezEsclOldObj.ID_Ruolo_In_UO = user.idCorrGlobali;
                    selezEsclOldObj.Valore = oggettoCustom.VALORE_DATABASE;
                    this.TemplateProject.OLD_OGG_CUSTOM[index] = selezEsclOldObj;
                }
            }
        }

        private void inserisciContatore(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, ArrayList dirittiCampiRuolo)
        {
            bool paneldll = false;
            bool contaDopo = false;
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                return;

            Panel divColCountAfter = new Panel();

            Label etichettaContatore = new Label();
            etichettaContatore.Text = oggettoCustom.DESCRIZIONE;
            etichettaContatore.CssClass = "weight";
            etichettaContatore.EnableViewState = true;

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaContatore);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);



            //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
            //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
            //del contatore come da formato prescelto e in readOnly
            Label etichettaDDL = new Label();
            etichettaDDL.EnableViewState = true;
            etichettaDDL.Width = 50;
            DropDownList ddl = new DropDownList();
            ddl.EnableViewState = true;

            string language = UIManager.UserManager.GetUserLanguage();
            ddl.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));

            Panel divRowDll = new Panel();
            divRowDll.CssClass = "row";
            divRowDll.EnableViewState = true;

            Panel divRowEtiDll = new Panel();
            divRowEtiDll.CssClass = "row";
            divRowEtiDll.EnableViewState = true;

            HtmlGenericControl parDll = new HtmlGenericControl("p");
            parDll.EnableViewState = true;

            if (string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {

                Ruolo ruoloUtente = RoleManager.GetRoleInSession();
                Registro[] registriRfVisibili = UIManager.RegistryManager.GetListRegistriesAndRF(ruoloUtente.systemId, string.Empty, string.Empty);

                Panel divColDllEti = new Panel();
                divColDllEti.CssClass = "col";
                divColDllEti.EnableViewState = true;

                Panel divColDll = new Panel();
                divColDll.CssClass = "col";
                divColDll.EnableViewState = true;

                switch (oggettoCustom.TIPO_CONTATORE)
                {
                    case "T":
                        break;
                    case "A":
                        paneldll = true;
                        etichettaDDL.Text = "&nbsp;AOO&nbsp;";
                        etichettaDDL.CssClass = "weight";
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "chzn-select-deselect";
                        ddl.Width = 240;

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "0" && !((Registro)registriRfVisibili[i]).Sospeso)
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Text = string.Empty;
                            it.Value = string.Empty;
                            ddl.Items.Add(it);
                            ddl.SelectedValue = string.Empty;
                        }
                        else
                        {
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                        }
                        if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45")
                        {
                            ddl.Enabled = false;
                        }

                        parDll.Controls.Add(etichettaDDL);
                        divColDllEti.Controls.Add(parDll);
                        divRowEtiDll.Controls.Add(divColDllEti);

                        divColDll.Controls.Add(ddl);
                        divRowDll.Controls.Add(divColDll);

                        break;
                    case "R":
                        paneldll = true;
                        etichettaDDL.Text = "&nbsp;RF&nbsp;";
                        etichettaDDL.CssClass = "weight";
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "chzn-select-deselect";
                        ddl.Width = 240;

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "1" && ((Registro)registriRfVisibili[i]).rfDisabled == "0")
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Value = string.Empty;
                            it.Text = string.Empty;
                            ddl.Items.Add(it);
                            ddl.SelectedValue = string.Empty;
                        }
                        else
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                        if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45")
                        {
                            ddl.Enabled = false;
                        }

                        ddl.CssClass = "chzn-select-deselect";

                        parDll.Controls.Add(etichettaDDL);
                        divColDllEti.Controls.Add(parDll);
                        divRowEtiDll.Controls.Add(divColDllEti);

                        divColDll.Controls.Add(ddl);
                        divRowDll.Controls.Add(divColDll);
                        break;

                }
            }

            //Imposto il contatore in funzione del formato
            CustomTextArea contatore = new CustomTextArea();
            contatore.EnableViewState = true;
            contatore.ID = oggettoCustom.SYSTEM_ID.ToString();
            contatore.CssClass = "txt_textdata_counter";
            contatore.CssClassReadOnly = "txt_textdata_counter_disabled";
            if (oggettoCustom.FORMATO_CONTATORE != "")
            {
                contatore.Text = oggettoCustom.FORMATO_CONTATORE;
                if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                {
                    //controllo se il contatore è custom in tal caso visualizzo anno accademico e non il semplice anno solare
                    if (!string.IsNullOrEmpty(oggettoCustom.ANNO_ACC))
                    {
                        string IntervalloDate = oggettoCustom.DATA_INIZIO.Substring(6, 4) + oggettoCustom.DATA_FINE.Substring(5, 5);
                        contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO_ACC);
                    }
                    else
                    { contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO); }
                    contatore.Text = contatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);
                    string codiceAmministrazione = UserManager.getInfoAmmCorrente(UIManager.UserManager.GetInfoUser().idAmministrazione).Codice;
                    contatore.Text = contatore.Text.Replace("COD_AMM", codiceAmministrazione);
                    contatore.Text = contatore.Text.Replace("COD_UO", oggettoCustom.CODICE_DB);

                    if (!string.IsNullOrEmpty(oggettoCustom.DATA_INSERIMENTO))
                    {
                        int fine = oggettoCustom.DATA_INSERIMENTO.LastIndexOf(".");
                        if (fine == -1)
                            fine = oggettoCustom.DATA_INSERIMENTO.LastIndexOf(":");
                        contatore.Text = contatore.Text.Replace("gg/mm/aaaa hh:mm", oggettoCustom.DATA_INSERIMENTO.Substring(0, fine));
                        contatore.Text = contatore.Text.Replace("gg/mm/aaaa", oggettoCustom.DATA_INSERIMENTO.Substring(0, 10));
                    }

                    if (!string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) && oggettoCustom.ID_AOO_RF != "0")
                    {
                        Registro reg = UserManager.getRegistroBySistemId(this, oggettoCustom.ID_AOO_RF);
                        if (reg != null)
                        {
                            contatore.Text = contatore.Text.Replace("RF", reg.codRegistro);
                            contatore.Text = contatore.Text.Replace("AOO", reg.codRegistro);
                        }
                    }
                }
                else
                {
                    contatore.Text = "";
                }
            }
            else
            {
                contatore.Text = oggettoCustom.VALORE_DATABASE;
            }

            Panel divRowCounter = new Panel();
            divRowCounter.CssClass = "row";
            divRowCounter.EnableViewState = true;

            Panel divColCountCounter = new Panel();
            divColCountCounter.CssClass = "col";
            divColCountCounter.EnableViewState = true;


            CheckBox cbContaDopo = new CheckBox();
            cbContaDopo.EnableViewState = true;
            //Pulsante annulla
            CustomButton btn_annulla = new CustomButton();
            btn_annulla.EnableViewState = true;
            btn_annulla.ID = "btn_a_" + oggettoCustom.SYSTEM_ID.ToString();
            btn_annulla.Text = Utils.Languages.GetLabelFromCode("BtnAbortProflier", language);
            btn_annulla.Visible = false;
            btn_annulla.CssClass = "buttonAbort";
            btn_annulla.OnMouseOver = "buttonAbortHover";
            btn_annulla.CssClassDisabled = "buttonAbortDisable";

            Panel divColCountAbort = new Panel();
            divColCountAbort.CssClass = "col-right-no-margin-no-top";
            divColCountAbort.EnableViewState = true;


            //if (!String.IsNullOrEmpty(this.DocumentInWorking.docNumber))
            //{
            //    btn_annulla.Click += this.OnBtnCounterAbort_Click;
            //}


            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloContatore(etichettaContatore, contatore, cbContaDopo, etichettaDDL, ddl, oggettoCustom, this.TemplateProject, btn_annulla, dirittiCampiRuolo);


            if (etichettaContatore.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            contatore.ReadOnly = true;
            //if (oggettoCustom != null && !String.IsNullOrEmpty(oggettoCustom.DATA_ANNULLAMENTO) && this.EnableRepertory)
            //{
            //    contatore.Text += " -- Annullato il :" + oggettoCustom.DATA_ANNULLAMENTO;
            //    contatore.CssClassReadOnly = "txt_textdata_counter_disabled_red";
            //    this.UpPnlTypeDocument.Update();
            //}
            //contatore.Enabled = false;
            //Inserisco il cb per il conta dopo
            if (oggettoCustom.CONTA_DOPO == "1")
            {
                cbContaDopo.ID = oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo";
                cbContaDopo.Checked = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                cbContaDopo.ToolTip = "Attiva / Disattiva incremento del contatore al salvataggio dei dati.";

                if ((!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE) && ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && ProjectManager.getProjectInSession().accessRights == "45"))
                {
                    cbContaDopo.Checked = false;
                    cbContaDopo.Visible = false;
                    cbContaDopo.Enabled = false;
                }
                else
                {
                    contaDopo = true;
                }

                divColCountAfter.CssClass = "col";
                divColCountAfter.EnableViewState = true;
                divColCountAfter.Controls.Add(cbContaDopo);
                //divRowDll.Controls.Add(divColCountAfter);
            }


            divColCountCounter.Controls.Add(contatore);
            divRowCounter.Controls.Add(divColCountCounter);



            divColCountAbort.Controls.Add(btn_annulla);
            divRowCounter.Controls.Add(divColCountAbort);



            if (paneldll)
            {
                this.PnlTypeDocument.Controls.Add(divRowEtiDll);
                this.PnlTypeDocument.Controls.Add(divRowDll);

            }

            if (contaDopo)
            {
                divRowCounter.Controls.Add(divColCountAfter);
            }

            if (contatore.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowCounter);
            }

        }

        //protected void OnBtnCounterAbort_Click(object sender, EventArgs e)
        //{
        //    string idOggetto = (((CustomButton)sender).ID);
        //    this.IdObjectCustom = idOggetto;
        //  ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpPnlTypeDocument", "ajaxModalPopupAbortCounter();", true);
        //}

        private void inserisciData(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            //Per il momento questo tipo di campo è stato implementato con tre semplici textBox
            //Sarebbe opportuno creare un oggetto personalizzato, che espone le stesse funzionalità
            //della textBox, ma che mi permette di gestire la data con i tre campi separati.
            DocsPaWR.StoricoProfilatiOldValue dataOldOb = new StoricoProfilatiOldValue();
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            Label etichettaData = new Label();
            etichettaData.EnableViewState = true;

            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaData.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaData.Text = oggettoCustom.DESCRIZIONE;
            }
            etichettaData.CssClass = "weight";

            UserControls.Calendar data = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            data.EnableViewState = true;
            data.ID = oggettoCustom.SYSTEM_ID.ToString();
            data.VisibleTimeMode = ProfilerDocManager.getVisibleTimeMode(oggettoCustom);
            data.SetEnableTimeMode();

            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                data.Text = oggettoCustom.VALORE_DATABASE;
            }

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaData, data, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID) && (ProjectManager.getProjectInSession().accessRights == "45"  || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C"))))
            {
                data.ReadOnly = true;
            }

            if (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().systemID))
            {
                if (this.TemplateProject.OLD_OGG_CUSTOM[index] == null) //se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
                {
                    //blocco storico profilazione campo data
                    dataOldOb.IDTemplate = this.TemplateProject.ID_TIPO_ATTO;
                    Fascicolo fasc = ProjectManager.getProjectInSession();
                    dataOldOb.ID_Doc_Fasc = fasc.systemID;
                    dataOldOb.ID_Oggetto = oggettoCustom.SYSTEM_ID.ToString();
                    dataOldOb.Valore = oggettoCustom.VALORE_DATABASE;
                    dataOldOb.Tipo_Ogg_Custom = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                    InfoUtente user = UserManager.GetInfoUser();
                    dataOldOb.ID_People = user.idPeople;
                    dataOldOb.ID_Ruolo_In_UO = user.idCorrGlobali;
                    this.TemplateProject.OLD_OGG_CUSTOM[index] = dataOldOb;
                }
            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaData);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);

            if (etichettaData.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col";
            divColValue.EnableViewState = true;

            divColValue.Controls.Add(data);
            divRowValue.Controls.Add(divColValue);

            if (data.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void inserisciLink(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            UserControls.LinkDocFasc link = (UserControls.LinkDocFasc)this.LoadControl("../UserControls/LinkDocFasc.ascx");
            link.EnableViewState = true;
            link.From = "P";
            link.ID = oggettoCustom.SYSTEM_ID.ToString();
            link.IsEsterno = (oggettoCustom.TIPO_LINK.Equals("ESTERNO"));
            link.IsFascicolo = ("FASCICOLO".Equals(oggettoCustom.TIPO_OBJ_LINK));

            if ("SI".Equals(oggettoCustom.CAMPO_OBBLIGATORIO))
            {
                link.TxtEtiLinkDocOrFasc = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                link.TxtEtiLinkDocOrFasc = oggettoCustom.DESCRIZIONE;
            }
            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(link.TxtEtiLinkDocOrFasc, link, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);
            Fascicolo sd = ProjectManager.getProjectInSession();
            if (sd != null && !string.IsNullOrEmpty(sd.systemID) && (!string.IsNullOrEmpty(sd.accessRights) && (sd.accessRights == "45")))
            {
                link.IsInsertModify = false;
            }

            //bool greyWithDocNum = "G".Equals(sd.tipoProto) && !string.IsNullOrEmpty(sd.docNumber);
            //if (!string.IsNullOrEmpty(this.DocumentInWorking.systemId) && (!string.IsNullOrEmpty(sd.accessRights) && (accessRights == "45" || readOnly)))
            //{
            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                link.HideLink = true;
            }
            //}
            //else
            //{
            //    link.HideLink = false;
            //}

            link.Value = oggettoCustom.VALORE_DATABASE;

            if (link.Visible)
            {
                this.PnlTypeDocument.Controls.Add(link);
            }
        }

        private void inserisciCampoSeparatore(DocsPaWR.OggettoCustom oggettoCustom)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            Label etichettaCampoSeparatore = new Label();
            etichettaCampoSeparatore.CssClass = "weight";
            etichettaCampoSeparatore.EnableViewState = true;
            etichettaCampoSeparatore.Text = oggettoCustom.DESCRIZIONE.ToUpper();

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col_full_line";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaCampoSeparatore);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);

            if (etichettaCampoSeparatore.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }
        }

        private void inserisciCorrispondente(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, ArrayList dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            DocsPaWR.StoricoProfilatiOldValue corrOldOb = new StoricoProfilatiOldValue();

            UserControls.CorrespondentCustom corrispondente = (UserControls.CorrespondentCustom)this.LoadControl("../UserControls/CorrespondentCustom.ascx");
            corrispondente.EnableViewState = true;
            corrispondente.PageCaller = "Popup";

            corrispondente.TxtEtiCustomCorrespondent = oggettoCustom.DESCRIZIONE;

            corrispondente.TypeCorrespondentCustom = oggettoCustom.TIPO_RICERCA_CORR;
            corrispondente.ID = oggettoCustom.SYSTEM_ID.ToString();

            //Da amministrazione è stato impostato un ruolo di default per questo campo.
            if (!string.IsNullOrEmpty(oggettoCustom.ID_RUOLO_DEFAULT) && oggettoCustom.ID_RUOLO_DEFAULT != "0")
            {
                DocsPaWR.Ruolo ruolo = RoleManager.getRuoloById(oggettoCustom.ID_RUOLO_DEFAULT);
                if (ruolo != null)
                {
                    corrispondente.IdCorrespondentCustom = ruolo.systemId;
                    corrispondente.TxtCodeCorrespondentCustom = ruolo.codiceRubrica;
                    corrispondente.TxtDescriptionCorrespondentCustom = ruolo.descrizione;
                }
                oggettoCustom.ID_RUOLO_DEFAULT = "0";
            }

            //Il campo è valorizzato.
            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                DocsPaWR.Corrispondente corr_1 = AddressBookManager.getCorrispondenteBySystemIDDisabled(oggettoCustom.VALORE_DATABASE);
                if (corr_1 != null)
                {
                    corrispondente.IdCorrespondentCustom = corr_1.systemId;
                    corrispondente.TxtCodeCorrespondentCustom = corr_1.codiceRubrica.ToString();
                    corrispondente.TxtDescriptionCorrespondentCustom = corr_1.descrizione.ToString();
                    oggettoCustom.VALORE_DATABASE = corr_1.systemId;
                }
            }

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(corrispondente.TxtEtiCustomCorrespondent, corrispondente, oggettoCustom, this.TemplateProject, dirittiCampiRuolo);

            if (corrispondente.Visible)
            {
                this.PnlTypeDocument.Controls.Add(corrispondente);
            }

        }


        public void impostaDirittiRuoloSulCampo(object etichetta, object campo, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, ArrayList dirittiCampiRuolo)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneDocManager.getDirittiCampoTipologiaDoc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "CampoDiTesto":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO") || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C")))
                            {
                                ((CustomTextArea)campo).ReadOnly = true;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((CustomTextArea)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "CasellaDiSelezione":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO") || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C")))
                            {
                                ((System.Web.UI.WebControls.CheckBoxList)campo).Enabled = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.CheckBoxList)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "MenuATendina":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO") || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C")))
                            {
                                ((System.Web.UI.WebControls.DropDownList)campo).Enabled = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.DropDownList)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "SelezioneEsclusiva":
                            //Per la selezione esclusiva è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                            break;
                        case "Contatore":
                            //Per il contatore è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                            break;
                        case "Data":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO") || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C")))
                            {
                                ((UserControls.Calendar)campo).ReadOnly = true;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((UserControls.Calendar)campo).Visible = false;
                                ((UserControls.Calendar)campo).VisibleTimeMode = UserControls.Calendar.VisibleTimeModeEnum.Nothing;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Corrispondente":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO") || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C")))
                            {
                                ((UserControls.CorrespondentCustom)campo).CODICE_READ_ONLY = true;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            else
                            {
                                ((UserControls.CorrespondentCustom)campo).CODICE_READ_ONLY = false;
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((UserControls.CorrespondentCustom)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Link":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO") || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C")))
                            {
                                ((UserControls.LinkDocFasc)campo).IsInsertModify = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            else
                            {
                                ((UserControls.LinkDocFasc)campo).IsInsertModify = true;
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((UserControls.LinkDocFasc)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            else
                            {
                                ((UserControls.LinkDocFasc)campo).IsInsertModify = true;
                            }


                            break;
                        case "OggettoEsterno":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO") || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C")))
                            {
                                ((UserControls.IntegrationAdapter)campo).View = UserControls.IntegrationAdapterView.READ_ONLY;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            else
                            {
                                ((UserControls.IntegrationAdapter)campo).View = UserControls.IntegrationAdapterView.INSERT_MODIFY;
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((UserControls.IntegrationAdapter)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                    }
                }
            }
        }

        public void impostaDirittiRuoloSelezioneEsclusiva(object etichetta, object campo, object button, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, ArrayList dirittiCampiRuolo)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneDocManager.getDirittiCampoTipologiaDoc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO")  || (ProjectManager.getProjectInSession() != null && !string.IsNullOrEmpty(ProjectManager.getProjectInSession().stato) && ProjectManager.getProjectInSession().stato.Equals("C")))
                    {
                        ((System.Web.UI.WebControls.RadioButtonList)campo).Enabled = false;
                        ((CustomImageButton)button).Visible = false;
                        oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                    }
                    if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                        ((System.Web.UI.WebControls.RadioButtonList)campo).Visible = false;
                        ((CustomImageButton)button).Visible = false;
                        oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                    }
                }
            }
        }

        public void impostaDirittiRuoloContatoreSottocontatore(object etichettaContatoreSottocontatore, object contatore, object sottocontatore, object dataInserimentoSottocontatore, object checkBox, object etichettaDDL, object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                    {
                        ((System.Web.UI.WebControls.CheckBox)checkBox).Enabled = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Enabled = false;

                        //Se il contatore è solo visibile deve comunque scattare se :
                        //1. Contatore di tipologia senza conta dopo
                        //2. Contatore di AOO senza conta dopo e con una sola scelta
                        //3. Contatore di RF senza conta dopo e con una sola scelta
                        if (
                            (oggettoCustom.TIPO_CONTATORE == "T" && oggettoCustom.CONTA_DOPO == "0")
                            ||
                            (oggettoCustom.TIPO_CONTATORE == "A" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            ||
                           (oggettoCustom.TIPO_CONTATORE == "R" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            )
                        {
                            oggettoCustom.CONTA_DOPO = "0";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        else
                        {
                            oggettoCustom.CONTA_DOPO = "1";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                        }
                    }
                    if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichettaContatoreSottocontatore).Visible = false;
                        ((System.Web.UI.WebControls.Label)etichettaDDL).Visible = false;
                        ((CustomTextArea)contatore).Visible = false;
                        ((CustomTextArea)sottocontatore).Visible = false;
                        ((CustomTextArea)dataInserimentoSottocontatore).Visible = false;
                        ((System.Web.UI.WebControls.CheckBox)checkBox).Visible = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Visible = false;

                        //Se il contatore non è nè visibile nè modificabile deve comunque scattare se :
                        //1. Contatore di tipologia senza conta dopo
                        //2. Contatore di AOO senza conta dopo e con una sola scelta
                        //3. Contatore di RF senza conta dopo e con una sola scelta
                        if (
                            (oggettoCustom.TIPO_CONTATORE == "T" && oggettoCustom.CONTA_DOPO == "0")
                            ||
                            (oggettoCustom.TIPO_CONTATORE == "A" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            ||
                           (oggettoCustom.TIPO_CONTATORE == "R" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            )
                        {
                            oggettoCustom.CONTA_DOPO = "0";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        else
                        {
                            oggettoCustom.CONTA_DOPO = "1";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                        }
                    }
                }
            }
        }

        private int impostaSelezioneEsclusiva(string valore, RadioButtonList rbl)
        {
            for (int i = 0; i < rbl.Items.Count; i++)
            {
                if (rbl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }

        private void impostaSelezioneCaselleDiSelezione(DocsPaWR.OggettoCustom objCustom, CheckBoxList cbl)
        {
            for (int i = 0; i < objCustom.VALORI_SELEZIONATI.Length; i++)
            {
                for (int j = 0; j < cbl.Items.Count; j++)
                {
                    if ((string)objCustom.VALORI_SELEZIONATI[i] == cbl.Items[j].Text)
                    {
                        cbl.Items[j].Selected = true;
                    }
                }
            }
        }

        private int impostaSelezioneMenuATendina(string valore, DropDownList ddl)
        {
            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }

        private void impostaDirittiRuoloContatore(object etichettaContatore, object campo, object checkBox, object etichettaDDL, object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, Button btn_annulla, ArrayList dirittiCampiRuolo)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                    {
                        ((System.Web.UI.WebControls.CheckBox)checkBox).Enabled = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Enabled = false;

                        //Se il contatore è solo visibile deve comunque scattare se :
                        //1. Contatore di tipologia senza conta dopo
                        //2. Contatore di AOO senza conta dopo e con una sola scelta
                        //3. Contatore di RF senza conta dopo e con una sola scelta
                        if (
                            (oggettoCustom.TIPO_CONTATORE == "T" && oggettoCustom.CONTA_DOPO == "0")
                            ||
                            (oggettoCustom.TIPO_CONTATORE == "A" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            ||
                           (oggettoCustom.TIPO_CONTATORE == "R" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            )
                        {
                            oggettoCustom.CONTA_DOPO = "0";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        else
                        {
                            oggettoCustom.CONTA_DOPO = "1";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                        }
                    }
                    if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichettaContatore).Visible = false;
                        ((System.Web.UI.WebControls.Label)etichettaDDL).Visible = false;
                        ((CustomTextArea)campo).Visible = false;
                        ((System.Web.UI.WebControls.CheckBox)checkBox).Visible = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Visible = false;

                        //Se il contatore non è nè visibile nè modificabile deve comunque scattare se :
                        //1. Contatore di tipologia senza conta dopo
                        //2. Contatore di AOO senza conta dopo e con una sola scelta
                        //3. Contatore di RF senza conta dopo e con una sola scelta
                        if (
                            (oggettoCustom.TIPO_CONTATORE == "T" && oggettoCustom.CONTA_DOPO == "0")
                            ||
                            (oggettoCustom.TIPO_CONTATORE == "A" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            ||
                           (oggettoCustom.TIPO_CONTATORE == "R" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            )
                        {
                            oggettoCustom.CONTA_DOPO = "0";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        else
                        {
                            oggettoCustom.CONTA_DOPO = "1";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                        }
                    }

                    btn_annulla.Visible = false;
                }
            }
        }

        public void impostaDirittiRuoloContatoreSottocontatore(object etichettaContatoreSottocontatore, object contatore, object sottocontatore, object dataInserimentoSottocontatore, object checkBox, object etichettaDDL, object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, ArrayList dirittiCampiRuolo)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                    {
                        ((System.Web.UI.WebControls.CheckBox)checkBox).Enabled = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Enabled = false;

                        //Se il contatore è solo visibile deve comunque scattare se :
                        //1. Contatore di tipologia senza conta dopo
                        //2. Contatore di AOO senza conta dopo e con una sola scelta
                        //3. Contatore di RF senza conta dopo e con una sola scelta
                        if (
                            (oggettoCustom.TIPO_CONTATORE == "T" && oggettoCustom.CONTA_DOPO == "0")
                            ||
                            (oggettoCustom.TIPO_CONTATORE == "A" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            ||
                           (oggettoCustom.TIPO_CONTATORE == "R" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            )
                        {
                            oggettoCustom.CONTA_DOPO = "0";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        else
                        {
                            oggettoCustom.CONTA_DOPO = "1";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                        }
                    }
                    if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichettaContatoreSottocontatore).Visible = false;
                        ((System.Web.UI.WebControls.Label)etichettaDDL).Visible = false;
                        ((CustomTextArea)contatore).Visible = false;
                        ((CustomTextArea)sottocontatore).Visible = false;
                        ((CustomTextArea)dataInserimentoSottocontatore).Visible = false;
                        ((System.Web.UI.WebControls.CheckBox)checkBox).Visible = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Visible = false;

                        //Se il contatore non è nè visibile nè modificabile deve comunque scattare se :
                        //1. Contatore di tipologia senza conta dopo
                        //2. Contatore di AOO senza conta dopo e con una sola scelta
                        //3. Contatore di RF senza conta dopo e con una sola scelta
                        if (
                            (oggettoCustom.TIPO_CONTATORE == "T" && oggettoCustom.CONTA_DOPO == "0")
                            ||
                            (oggettoCustom.TIPO_CONTATORE == "A" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            ||
                           (oggettoCustom.TIPO_CONTATORE == "R" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            )
                        {
                            oggettoCustom.CONTA_DOPO = "0";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        else
                        {
                            oggettoCustom.CONTA_DOPO = "1";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                        }
                    }
                }
            }
        }

        protected void cancella_selezioneEsclusiva_Click(object sender, EventArgs e)
        {
            string idOggetto = (((CustomImageButton)sender).ID).Substring(1);
            ((RadioButtonList)this.PnlTypeDocument.FindControl(idOggetto)).SelectedIndex = -1;
            ((RadioButtonList)this.PnlTypeDocument.FindControl(idOggetto)).EnableViewState = true;
            for (int i = 0; i < this.TemplateProject.ELENCO_OGGETTI.Length; i++)
            {
                if (((DocsPaWR.OggettoCustom)this.TemplateProject.ELENCO_OGGETTI[i]).SYSTEM_ID.ToString().Equals(idOggetto))
                {
                    ((DocsPaWR.OggettoCustom)this.TemplateProject.ELENCO_OGGETTI[i]).VALORE_DATABASE = "-1";
                }
            }
            this.UpPnlTypeDocument.Update();
        }

        protected Templates PopulateTemplate()
        {

            Templates result = this.TemplateProject;
            if (result != null)
            {
                for (int i = 0; i < result.ELENCO_OGGETTI.Length; i++)
                {
                    DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)result.ELENCO_OGGETTI[i];
                    this.salvaValoreCampo(oggettoCustom, oggettoCustom.SYSTEM_ID.ToString());
                }
                this.TemplateProject = result;
            }
            return result;
        }

        private void salvaValoreCampo(DocsPaWR.OggettoCustom oggettoCustom, string idOggetto)
        {
            //In questo metodo, oltre al controllo si salvano i valori dei campi inseriti 
            //dall'utente nel template in sessione. Solo successivamente, quanto verra' salvato 
            //il documento i suddetti valori verranno riportai nel Db vedi metodo "btn_salva_Click" della "docProfilo.aspx"

            //Label_Avviso.Visible = false;
            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
            {
                case "CampoDiTesto":
                    CustomTextArea textBox = (CustomTextArea)this.PnlTypeDocument.FindControl(idOggetto);
                    if (textBox != null)
                    {
                        oggettoCustom.VALORE_DATABASE = textBox.Text;
                    }
                    else
                    {
                        oggettoCustom.VALORE_DATABASE = string.Empty;
                    }
                    break;
                case "CasellaDiSelezione":
                    CheckBoxList casellaSelezione = (CheckBoxList)this.PnlTypeDocument.FindControl(idOggetto);
                    //Nessuna selezione
                    if (casellaSelezione != null)
                    {
                        if (casellaSelezione.SelectedIndex == -1 && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                        {
                            for (int i = 0; i < oggettoCustom.VALORI_SELEZIONATI.Length; i++)
                                oggettoCustom.VALORI_SELEZIONATI[i] = null;
                            return;
                        }

                        //Controllo eventuali selezioni
                        oggettoCustom.VALORI_SELEZIONATI = new string[oggettoCustom.ELENCO_VALORI.Length];
                        oggettoCustom.VALORE_DATABASE = string.Empty;

                        for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
                        {
                            DocsPaWR.ValoreOggetto valoreOggetto = (DocsPaWR.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i];
                            foreach (ListItem valoreSelezionato in casellaSelezione.Items)
                            {
                                if (valoreOggetto.VALORE == valoreSelezionato.Text && valoreSelezionato.Selected)
                                    oggettoCustom.VALORI_SELEZIONATI[i] = valoreSelezionato.Text;
                            }
                        }
                    }
                    else
                    {
                        //Controllo eventuali selezioni
                        oggettoCustom.VALORI_SELEZIONATI = new string[oggettoCustom.ELENCO_VALORI.Length];
                        oggettoCustom.VALORE_DATABASE = string.Empty;
                    }
                    break;
                case "MenuATendina":
                    DropDownList dropDwonList = (DropDownList)this.PnlTypeDocument.FindControl(idOggetto);
                    if (dropDwonList != null)
                    {
                        oggettoCustom.VALORE_DATABASE = dropDwonList.SelectedItem.Text;
                    }
                    else
                    {
                        oggettoCustom.VALORE_DATABASE = string.Empty;
                    }
                    break;
                case "SelezioneEsclusiva":
                    RadioButtonList radioButtonList = (RadioButtonList)this.PnlTypeDocument.FindControl(idOggetto);
                    if (radioButtonList != null)
                    {
                        if ((oggettoCustom.VALORE_DATABASE == "-1" && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI")))
                        {
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                            return;
                        }
                        if (oggettoCustom.VALORE_DATABASE == "-1")
                        {
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                        }
                        else
                        {
                            if (radioButtonList.SelectedItem != null)
                                oggettoCustom.VALORE_DATABASE = radioButtonList.SelectedItem.Text;
                        }
                    }
                    else
                    {
                        oggettoCustom.VALORE_DATABASE = string.Empty;
                    }
                    break;
                case "Data":
                    UserControls.Calendar data = (UserControls.Calendar)this.PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    if (data != null)
                    {
                        if (string.IsNullOrEmpty(data.Text) && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                        {
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                            return;
                        }
                        if (string.IsNullOrEmpty(data.Text))
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                        else
                            oggettoCustom.VALORE_DATABASE = data.Text;
                    }
                    else
                    {
                        oggettoCustom.VALORE_DATABASE = string.Empty;
                    }
                    break;
                case "Corrispondente":
                    UserControls.CorrespondentCustom corr = (UserControls.CorrespondentCustom)this.PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    if (corr != null)
                    {
                        oggettoCustom.VALORE_DATABASE = corr.IdCorrespondentCustom;
                    }
                    else
                    {
                        oggettoCustom.VALORE_DATABASE = string.Empty;
                    }
                    break;
                case "Contatore":
                case "ContatoreSottocontatore":
                    if (string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
                    {
                        switch (oggettoCustom.TIPO_CONTATORE)
                        {
                            case "A":
                                DropDownList ddlAoo = (DropDownList)this.PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu"); if (ddlAoo != null)
                                {
                                    oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
                                }
                                break;
                            case "R":
                                DropDownList ddlRf = (DropDownList)this.PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu"); if (ddlRf != null)
                                {
                                    oggettoCustom.ID_AOO_RF = ddlRf.SelectedValue;
                                }
                                break;
                        }
                        if (oggettoCustom.CONTA_DOPO == "1")
                        {
                            CheckBox cbContaDopo = (CheckBox)this.PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo");
                            if (cbContaDopo != null)
                            {
                                oggettoCustom.CONTATORE_DA_FAR_SCATTARE = cbContaDopo.Checked;
                            }
                        }
                        else
                        {
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE) && oggettoCustom.FORMATO_CONTATORE.LastIndexOf("COD_UO") != -1)
                        {
                            if (UIManager.RoleManager.GetRoleInSession() != null && RoleManager.GetRoleInSession().uo != null)
                                oggettoCustom.CODICE_DB = RoleManager.GetRoleInSession().uo.codice;
                        }
                    }
                    break;
                case "Link":
                    LinkDocFasc link = (LinkDocFasc)this.PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    oggettoCustom.VALORE_DATABASE = link.Value;
                    break;
                case "OggettoEsterno":
                    IntegrationAdapter intAd = (IntegrationAdapter)this.PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    IntegrationAdapterValue value = intAd.Value;
                    if (value != null)
                    {
                        oggettoCustom.VALORE_DATABASE = value.Descrizione;
                        oggettoCustom.CODICE_DB = value.Codice;
                        oggettoCustom.MANUAL_INSERT = value.ManualInsert;
                    }
                    else
                    {
                        oggettoCustom.VALORE_DATABASE = "";
                        oggettoCustom.CODICE_DB = "";
                        oggettoCustom.MANUAL_INSERT = false;
                    }
                    break;
            }
        }
        #endregion

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try
            {
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> ccList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.Cc"];
                string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();
                bool esterniInListaInt = false;

                if (atList != null && atList.Count > 0)
                {
                    NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                    Corrispondente corr = null;
                    string idAmm = UIManager.UserManager.GetInfoUser().idAmministrazione;
                    foreach (NttDataWA.Popup.AddressBook.CorrespondentDetail addressBookCorrespondent in atList)
                    {

                        if (!addressBookCorrespondent.isRubricaComune)
                        {
                            corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(addressBookCorrespondent.SystemID);
                        }
                        else
                        {
                            corr = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(addressBookCorrespondent.CodiceRubrica);
                        }

                    }
                    if (addressBookCallFrom == "COLLOCATION")
                    {
                        this.idProjectCollocation.Value = corr.systemId;
                        this.projectTxtCodiceCollocazione.Text = corr.codiceRubrica;
                        this.projectTxtDescrizioneCollocazione.Text = corr.descrizione;
                        this.UpProjectPhisycCollocation.Update();
                    }
                    else if (addressBookCallFrom == "CUSTOM")
                    {
                        if (HttpContext.Current.Session["idCustomObjectCustomCorrespondent"] != null)
                        {
                            if (HttpContext.Current.Session["idCustomObjectCustomCorrespondent"].ToString() != "")
                            {
                                UserControls.CorrespondentCustom userCorr = (UserControls.CorrespondentCustom)this.PnlTypeDocument.FindControl(HttpContext.Current.Session["idCustomObjectCustomCorrespondent"].ToString());
                                userCorr.TxtCodeCorrespondentCustom = corrInSess.CodiceRubrica;
                                userCorr.TxtDescriptionCorrespondentCustom = corrInSess.Descrizione.Replace(START_SPECIAL_STYLE, "").Replace(END_SPECIAL_STYLE, "");
                                userCorr.IdCorrespondentCustom = corrInSess.SystemID;
                            }
                        }
                        this.UpPanelTipologiaFascicolo.Update();
                    }
                }
                HttpContext.Current.Session["AddressBook.At"] = null;
                HttpContext.Current.Session["AddressBook.Cc"] = null;
                if (esterniInListaInt)
                {
                    string msgDesc = "WarningDocumentCorrExtInListProtInt";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);

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