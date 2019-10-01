using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class MassiveCollate : System.Web.UI.Page
    {

        #region Properties

        private bool IsFasc
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["objType"]) && Request.QueryString["objType"].Equals("D") ? false : true;
            }
        }

        protected Dictionary<String, String> ListCheck
        {
            get
            {
                Dictionary<String, String> result = null;
                if (HttpContext.Current.Session["listCheck"] != null)
                {
                    result = HttpContext.Current.Session["listCheck"] as Dictionary<String, String>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listCheck"] = value;
            }
        }

        private string ReturnValue
        {
            get
            {
                //Laura 19 Marzo
                if ((HttpContext.Current.Session["ReturnValuePopup"]) != null)
                    return HttpContext.Current.Session["ReturnValuePopup"].ToString();
                else
                    return string.Empty;
            }
        }

        #endregion

        #region Events

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

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.CloseMask(true);
        }


        protected void BtnConfirm_Click(object sender, EventArgs e)
        {
            Fascicolo project = ProjectManager.getFascicoloSelezionatoFascRapida(this);
            if (project.pubblico)
            {
                string msgConfirm = "WarningDocumentConfirmPublicFolder";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddenPublicFolder', '');", true);
            }
            else
            {
                this.FascicolaDocumenti();
            }
        }

        protected void FascicolaDocumenti()
        {

            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            // Il report da mostrare all'utente
            MassiveOperationReport report;

            // Lista di system id degli elementi selezionati
            List<MassiveOperationTarget> selectedItem;

            // Il fascicolo selezionato per la fascicolazione rapida
            Fascicolo project;

            // Valore utilizzato per indicare che è possibile procedere con la fascicolazione
            bool canProceed = true;

            // Inizializzazione del report
            report = new MassiveOperationReport();

            // Il messaggio di errore
            StringBuilder errorMessage = new StringBuilder("Impossibile eseguire la fascicolazione:");

            // Recupero della lista dei system id dei documenti selezionati
            selectedItem = MassiveOperationUtils.GetSelectedItems();

            // Recupero del fascicolo selezionato per la fascicolazione rapida
            project = ProjectManager.getFascicoloSelezionatoFascRapida(this);


            // Se il fascicolo non è valorizzato non si può procedere
            if (project == null)
            {
                canProceed = false;
                errorMessage.AppendLine(" Selezionare un fascicolo in cui fascicolare.");
            }

            // Se non sono stati selezionati documenti, non si può procedere con la fascicolazione
            if (selectedItem == null ||
                selectedItem.Count == 0)
            {
                canProceed = false;
                errorMessage.AppendLine("- Selezionare almeno un documento da fascicolare.");
            }

            // Se non è possibile continuare, viene salvata una nuova riga per il report
            // e ne viene impostato l'esito negativo
            if (!canProceed)
                report.AddReportRow(
                    "N.A.",
                    MassiveOperationReport.MassiveOperationResultEnum.KO,
                    errorMessage.ToString());
            else
                // Altrimenti si procede con la fascicolazione
                this.ProceedWithOperation(selectedItem, project, report);
            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
            report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);
            this.generateReport(report, "Fascicolazione massiva");


            this.plcProject.Visible = false;
            this.UpCodFasc.Update();
        }

        protected void BtnReport_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "reallowOp", "reallowOp();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "visualReport", "parent.ajaxModalPopupMassiveReport();", true);
        }

        protected void txt_CodFascicolo_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "reallowOp", "reallowOp();", true);

            try
            {
                ProjectManager.removeFascicoloSelezionatoFascRapida(this);

                if (!string.IsNullOrEmpty(this.txt_CodFascicolo.Text))
                {
                    this.SearchProjectRegistro();
                }
                else
                {
                    this.txt_CodFascicolo.Text = string.Empty;
                    this.txt_DescFascicolo.Text = string.Empty;
                    this.IdProject.Value = string.Empty;
                    //Laura 25 Marzo
                    ProjectManager.setProjectInSessionForRicFasc(null);
                    ProjectManager.setProjectInSessionForRicFasc(String.Empty);
                }

                this.UpCodFasc.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #region Methods

        private void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.HiddenPublicFolder.Value))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
                this.HiddenPublicFolder.Value  = string.Empty;
                this.FascicolaDocumenti();
                return;
            }
            if (!string.IsNullOrEmpty(this.MassiveReport.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveReport','');", true);
            }
        }

        protected void InitializePage()
        {
            ProjectManager.removeFascicoloSelezionatoFascRapida(this);
            ProjectManager.removeFolderSelezionato(this);

            this.InitializeLanguage();
            this.InitializeList();
            this.SetAjaxDescriptionProject();

            this.BtnReport.Visible = false;
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnConfirm.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnConfirm", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnClose", language);
            this.BtnReport.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
            this.MassiveReport.Title = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
            this.litMessage.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserAskConfirm", language);
            this.LtlCodFascGenProc.Text = Utils.Languages.GetLabelFromCode("LtlCodFascGenProc", language);
            this.grdReport.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserGridResult", language);
            this.grdReport.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserGridDetails", language);
            this.btnclassificationschema.AlternateText = Utils.Languages.GetLabelFromCode("btnclassificationschema", language);
            this.btnclassificationschema.ToolTip = Utils.Languages.GetLabelFromCode("btnclassificationschema", language);
            this.DocumentImgSearchProjects.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgSearchProjects", language);
            this.DocumentImgSearchProjects.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgSearchProjects", language);
        }

        public void InitializeList()
        {
            Dictionary<String, MassiveOperationTarget> temp = new Dictionary<string, MassiveOperationTarget>();

            // Inizializzazione della mappa con i system id degli oggetti e lo stato
            // di checking (in fase di inizializzazione tutti gli item sono deselezionati)
            foreach (KeyValuePair<string, string> item in this.ListCheck)
                if (!temp.Keys.Contains(item.Key))
                    temp.Add(item.Key, new MassiveOperationTarget(item.Key, item.Value));

            // Salvataggio del dizionario
            MassiveOperationUtils.ItemsStatus = temp;
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        private void CloseMask(bool withReturnValue)
        {
            string retValue = withReturnValue ? "true" : "false";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('MassiveCollate', '" + retValue + "');", true);
        }

        protected void generateReport(MassiveOperationReport report, string titolo)
        {
            this.generateReport(report, titolo, IsFasc);
        }

        public void generateReport(MassiveOperationReport report, string titolo, bool isFasc)
        {
            this.grdReport.DataSource = report.GetDataSet();
            this.grdReport.DataBind();
            this.pnlReport.Visible = true;
            this.upReport.Update();

            string template = (isFasc) ? "../xml/massiveOp_formatPdfExport_fasc.xml" : "../xml/massiveOp_formatPdfExport.xml";
            template = "../xml/massiveOp_formatPdfExport.xml";
            report.GenerateDataSetForExport(Server.MapPath(template), titolo);

            this.plcMessage.Visible = false;
            this.UpPnlMessage.Update();

            this.BtnConfirm.Enabled = false;
            this.BtnReport.Visible = true;
            this.UpPnlButtons.Update();
        }
        /*
        protected void SearchProjectRegistro()
        {
            Registro registro = RegistryManager.GetRegistryInSession();
            this.txt_DescFascicolo.Text = string.Empty;
            string codClassifica = string.Empty;
            Fascicolo project = null;

            if (string.IsNullOrEmpty(this.txt_CodFascicolo.Text))
            {
                this.txt_DescFascicolo.Text = string.Empty;
                return;
            }

            //FASCICOLAZIONE IN SOTTOFASCICOLI
            DocsPaWR.Fascicolo[] listaFasc = this.getFascicoli(registro);

            if (listaFasc != null)
            {
                if (listaFasc.Length > 0)
                {
                    //caso 1: al codice digitato corrisponde un solo fascicolo
                    if (listaFasc.Length == 1)
                    {
                        this.IdProject.Value = listaFasc[0].systemID;
                        this.txt_DescFascicolo.Text = listaFasc[0].descrizione;
                        if (listaFasc[0].tipo.Equals("G"))
                        {
                            codClassifica = listaFasc[0].codice;
                            project = listaFasc[0];
                        }
                        else
                        {
                            //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                            DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                            string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                            codClassifica = codiceGerarchia;
                        }
                    }
                    else
                    {
                        //caso 2: al codice digitato corrispondono piu fascicoli
                        codClassifica = this.txt_CodFascicolo.Text;
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
                        Fascicolo chiusoFasc = ProjectManager.getFascicoloDaCodice(this.Page, this.txt_CodFascicolo.Text);
                        if (chiusoFasc != null && !string.IsNullOrEmpty(chiusoFasc.stato) && chiusoFasc.stato.Equals("C"))
                        {
                            string msg = "WarningDocumentFileNoOpen";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        else
                        {
                            string msg = "WarningDocumentCodFileNoFound";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        this.txt_DescFascicolo.Text = string.Empty;
                        this.txt_CodFascicolo.Text = string.Empty;
                        this.IdProject.Value = string.Empty;
                    }
                }
            }

            if (!string.IsNullOrEmpty(codClassifica))
            {
                if (project == null)
                {
                    project = ProjectManager.getFascicoloDaCodice(this.Page,codClassifica);
                }

                string key =Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_BLOCCA_CLASS.ToString());
            
                if (project != null && project.systemID != null && project.isFascConsentita != null && project.isFascConsentita == "0" && !string.IsNullOrEmpty(key) && key.Equals("1"))
                {
                    //nuovoDoc = false;
                    string msgDesc = "WarningDocumentNoDocumentInsert";
                    this.txt_DescFascicolo.Text = string.Empty;
                    this.txt_CodFascicolo.Text = string.Empty;
                    this.IdProject.Value = string.Empty;
                    ProjectManager.setFascicoloSelezionatoFascRapida(null);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }
                else
                {
                    ProjectManager.setFascicoloSelezionatoFascRapida(project);
                }
            }
        }
        */

        protected void SearchProjectRegistro()
        {
            Registro registro = RegistryManager.GetRegistryInSession();
            this.txt_DescFascicolo.Text = string.Empty;
            string codClassifica = string.Empty;
            Fascicolo project = null;

            if (string.IsNullOrEmpty(this.txt_CodFascicolo.Text))
            {
                this.txt_DescFascicolo.Text = string.Empty;
                return;
            }
            if (this.txt_CodFascicolo.Text.IndexOf("//") > -1)
            {
                #region FASCICOLAZIONE IN SOTTOFASCICOLI
                string codice = string.Empty;
                string descrizione = string.Empty;
                DocsPaWR.Fascicolo SottoFascicolo = getFolder(registro, ref codice, ref descrizione);
                if (SottoFascicolo != null)
                {

                    if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
                    {
                        txt_DescFascicolo.Text = descrizione;
                        txt_CodFascicolo.Text = codice;
                        project = SottoFascicolo;
                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, SottoFascicolo.idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                        ProjectManager.setProjectInSessionForRicFasc(gerClassifica[gerClassifica.Length - 1].codice);
                        ProjectManager.setFascicoloSelezionatoFascRapida(this, SottoFascicolo);
                    }
                    else
                    {

                        //string msg = @"Attenzione, sottofascicolo non presente.";
                        string msg = "WarningDocumentSubFileNoFound";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        this.txt_DescFascicolo.Text = string.Empty;
                        this.txt_CodFascicolo.Text = string.Empty;
                        project = null;
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
                    this.txt_DescFascicolo.Text = string.Empty;
                    this.txt_CodFascicolo.Text = string.Empty;
                    project = null;
                    ProjectManager.setProjectInSessionForRicFasc(null);
                    ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                }

                #endregion
            }
            else
            {
                DocsPaWR.Fascicolo[] listaFasc = getFascicoli(registro);


                if (listaFasc != null)
                {
                    if (listaFasc.Length > 0)
                    {
                        //caso 1: al codice digitato corrisponde un solo fascicolo
                        if (listaFasc.Length == 1)
                        {
                            this.txt_DescFascicolo.Text = listaFasc[0].descrizione;
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
                            //Laura 25 Marzo
                            project = listaFasc[0];
                            ProjectManager.setProjectInSessionForRicFasc(codClassifica);
                            ProjectManager.setFascicoloSelezionatoFascRapida(this, listaFasc[0]);
                        }
                        else
                        {
                            codClassifica = this.txt_CodFascicolo.Text;
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
                            project = listaFasc[0];
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
                            Fascicolo chiusoFasc = ProjectManager.getFascicoloDaCodice(this.Page, this.txt_CodFascicolo.Text);
                            if (chiusoFasc != null && !string.IsNullOrEmpty(chiusoFasc.stato) && chiusoFasc.stato.Equals("C"))
                            {
                                //string msg = @"Attenzione, il fascicolo scelto è chiuso. Pertanto il documento non può essere inserito nel fascicolo selezionato.";
                                string msg = "WarningClassificationsFileNoOpen";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            else
                            {
                                //string msg = @"Attenzione, codice fascicolo non presente.";
                                string msg = "WarningClassificationsCodFileNoFound";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            this.txt_DescFascicolo.Text = string.Empty;
                            this.txt_CodFascicolo.Text = string.Empty;
                            //Laura 25 Marzo
                            ProjectManager.setProjectInSessionForRicFasc(null);
                            ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                            return;
                        }
                    }
                    string key = Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_BLOCCA_CLASS.ToString());

                    if (project != null && project.systemID != null && !string.IsNullOrEmpty(key) && key.Equals("1"))
                    {
                        if (project.tipo.Equals("G") && project.isFascConsentita != null && project.isFascConsentita == "0")
                        {
                            string msgDesc = project.isFascicolazioneConsentita ? "WarningDocumentNoDocumentInsert" : "WarningDocumentNoDocumentInsertClassification";
                            this.txt_DescFascicolo.Text = string.Empty;
                            this.txt_CodFascicolo.Text = string.Empty;
                            this.IdProject.Value = string.Empty;
                            ProjectManager.setFascicoloSelezionatoFascRapida(null);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return;
                        }
                        if (project.tipo.Equals("P") && !project.isFascicolazioneConsentita)
                        {
                            string msgDesc = "WarningDocumentNoDocumentInsertFolder";
                            this.txt_DescFascicolo.Text = string.Empty;
                            this.txt_CodFascicolo.Text = string.Empty;
                            this.IdProject.Value = string.Empty;
                            ProjectManager.setFascicoloSelezionatoFascRapida(null);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return;
                        }
                    }
                }
            }
        }

        private DocsPaWR.Fascicolo getFolder(DocsPaWR.Registro registro, ref string codice, ref string descrizione)
        {
            DocsPaWR.Folder[] listaFolder = null;
            DocsPaWR.Fascicolo fasc = null;
            string separatore = "//";
            int posSep = this.txt_CodFascicolo.Text.IndexOf("//");
            if (this.txt_CodFascicolo.Text != string.Empty && posSep > -1)
            {

                string codiceFascicolo = txt_CodFascicolo.Text.Substring(0, posSep);
                string descrFolder = txt_CodFascicolo.Text.Substring(posSep + separatore.Length);

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
            if (!this.txt_CodFascicolo.Text.Equals(""))
            {
                string codiceFascicolo = this.txt_CodFascicolo.Text;
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

        protected void SetAjaxDescriptionProject()
        {
            string dataUser = RoleManager.GetRoleInSession().idGruppo;
            dataUser = dataUser + "-" + RegistryManager.GetRegistryInSession().systemId;
            if (UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione) != null)
            {
                this.RapidSenderDescriptionProject.ContextKey = dataUser + "-" + UserManager.GetUserInSession().idAmministrazione + "-" + UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione).ID + "-" + UserManager.GetUserInSession().idPeople + "-" + UIManager.UserManager.GetUserInSession().systemId;
            }
        }

        /// <summary>
        /// Funzione per la fascicolazione dei documenti
        /// </summary>
        /// <param name="selectedItem">Lista dei system id dei documenti selezionati</param>
        /// <param name="project">Il fascicolo su cui fascicolare</param>
        /// <param name="reportTable">Tabella del report in cui inserire le informazioni sull'esito degli import</param>
        private void ProceedWithOperation(List<MassiveOperationTarget> selectedItem, Fascicolo project, MassiveOperationReport report)
        {
            // Un booleano utilizzato per indicare che un documento è già
            // fascicolato in un determinato fascicolo
            bool isAlreadyClassifficated;

            // Per indicare se il documento è bloccato
            bool isBlocked;

            // Il risultato della fascicolazione
            bool classificationResult;

            // Il messaggio da agigungere al report
            string message;

            bool isAnnullato;

            // Il risultato da aggiungere al report
            MassiveOperationReport.MassiveOperationResultEnum result;
            DocsPaWebService ws = new DocsPaWebService();
            // Per ogni documento da fascicolare...
            foreach (MassiveOperationTarget mot in selectedItem)
            {
                // Inizializzazione di messaggio e risultato
                message = String.Empty;
                result = MassiveOperationReport.MassiveOperationResultEnum.OK;

                InfoUtente infoUtente = UserManager.GetInfoUser();

                // ... si verifica se il documento è già fascicolato nel fascicolo selezionato
                isAlreadyClassifficated = DocumentManager.IsDocumentInFolderOrSubFolder(this, infoUtente, mot.Id, project);

                //Verifico se il documento è in checkout
                isBlocked = CheckInOut.CheckInOutServices.IsCheckedOutDocument(mot.Id, mot.Id, infoUtente, true,DocumentManager.getSelectedRecord());

                isAnnullato = ws.IsDocAnnullatoByIdProfile(mot.Id);

                string accessRight = ws.getAccessRightDocBySystemID(mot.Id, infoUtente);
                // ...se il documento è già fascicolato, viene aggiunto un messaggio adeguato al report
                if (isAlreadyClassifficated)
                {
                    message = "Il documento risulta già fascicolato nel fascicolo specificato o non è stato possibile recuperare informazioni sui fascicoli.";
                    result = MassiveOperationReport.MassiveOperationResultEnum.AlreadyWorked;
                }
                else if (isBlocked)
                {
                    message = "Il documento risulta bloccato.";
                    result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                }
                else if (accessRight.Equals("20"))
                {
                    result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                    message = String.Format("Il documento è in attesa di accettazione, quindi non può essere fascicolato");
                }
                else if (isAnnullato)
                {
                    result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                    message = String.Format("Il documento risulta annullato, quindi non può essere fascicolato");
                }
                else
                {
                    // ...altrimenti si procede con la classificazione
                    classificationResult = DocumentManager.FascicolaDocumentoAM(
                        this,
                        mot.Id,
                        project,
                        out message);

                    // Impostazione di un messaggio adeguato in base all'esito della
                    // fascicolazione
                    if (classificationResult)
                        message = "Documento fascicolato con successo.";
                    else
                    {
                        result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                        message = "Errore durante la fascicolazione del documento.";

                    }
                }

                // Aggiunta della riga al report
                report.AddReportRow(mot.Codice, result, message);
            }
        }

        #endregion

    }
}