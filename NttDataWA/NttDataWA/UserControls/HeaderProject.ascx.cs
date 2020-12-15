using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using System.ComponentModel;
using NttDataWA.UIManager;
using System.Web.UI.HtmlControls;

namespace NttDataWA.UserControls
{
    public partial class HeaderProject : System.Web.UI.UserControl
    {
        //***************************************************************
        //GIORDANO IACOZZILLI
        //17/07/2013
        //Gestione dell'icona della copia del docuemnto/fascicolo in deposito.
        //***************************************************************
        [Browsable(true)]
        public string FlagCopyInArchive
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["FlagCopyInArchive"] != null)
                {
                    result = HttpContext.Current.Session["FlagCopyInArchive"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["FlagCopyInArchive"] = value;
            }
        }
        public string SelectedPhaseId
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["SelectedPhaseId"] != null)
                {
                    result = HttpContext.Current.Session["SelectedPhaseId"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["SelectedPhaseId"] = value;
            }
        }
        //***************************************************************
        //FINE
        //***************************************************************

        private ArrayList MissingRolesList
        {
            get
            {
                ArrayList result = null;
                if (HttpContext.Current.Session["changeStateMissingRoles"] != null)
                {
                    result = HttpContext.Current.Session["changeStateMissingRoles"] as ArrayList;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["changeStateMissingRoles"] = value;
            }
        }

        public Stato SelectedState
        {
            get
            {
                Stato result = null;
                if (HttpContext.Current.Session["SelectedState"] != null)
                {
                    result = HttpContext.Current.Session["SelectedState"] as Stato;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["SelectedState"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializeLabel();
                    //***************************************************************
                    //GIORDANO IACOZZILLI
                    //17/07/2013
                    //Gestione dell'icona della copia del docuemnto/fascicolo in deposito.
                    //***************************************************************
                    try
                    {
                        if (this.FlagCopyInArchive == "1")
                        {
                            this.phNeutro.Visible = true;
                            this.cimgbttIsCopy.ToolTip = "Copia del fascicolo in archivio corrente";
                        }
                    }
                    catch
                    {
                        //Questo è per Veltri.
                    }
                    //***************************************************************
                    //FINE
                    //***************************************************************
                    Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
                    if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.systemID))
                    {
                        this.viewLabel(true);
                        this.projectLblIdGenerato.Text = fascicolo.systemID;
                        this.projectLblCodiceGenerato.Text = fascicolo.codice;
                        switch (fascicolo.stato)
                        {
                            case "A":
                                {
                                    this.projectLblStatoGenerato.Text = Utils.Languages.GetLabelFromCode("prjectStatoRegistroAperto", UIManager.UserManager.GetUserLanguage());
                                    this.projectLblStatoGenerato.CssClass = "open";
                                    break;
                                }
                            case "C":
                                {
                                    this.projectLblStatoGenerato.Text = Utils.Languages.GetLabelFromCode("prjectStatoRegistroChiuso", UIManager.UserManager.GetUserLanguage());
                                    this.projectLblStatoGenerato.CssClass = "close";
                                    break;
                                }
                            default:
                                {
                                    this.projectLblStatoGenerato.Text = Utils.Languages.GetLabelFromCode("prjectStatoRegistroGiallo", UIManager.UserManager.GetUserLanguage());
                                    this.projectLblStatoGenerato.CssClass = "giallo";
                                    break;
                                }

                        }

                        Registro registro = UIManager.RegistryManager.getRegistroBySistemId(fascicolo.idRegistroNodoTit);
                        if (registro == null)
                        {
                            registro = UIManager.RegistryManager.getRegistroBySistemId(fascicolo.idRegistro);
                        }

                        if (registro != null)
                            this.projectLblRegistroGenerato.Text = registro.codRegistro;

                        OrgTitolario titolario = UIManager.ClassificationSchemeManager.getTitolario(fascicolo.idTitolario);
                        string language = UIManager.UserManager.GetUserLanguage();
                        if (titolario != null)
                        {
                            switch (titolario.Stato)
                            {
                                case OrgStatiTitolarioEnum.Attivo:
                                    this.projectLblTitolarioGenerato.ToolTip = Utils.Languages.GetLabelFromCode("ProjectLblTitolarioAttivo", language).Replace("@@", titolario.DescrizioneLite);
                                    this.projectLblTitolarioGenerato.Text = ellipsis(projectLblTitolarioGenerato.ToolTip, 20);
                                    break;

                                default:
                                    this.projectLblTitolarioGenerato.ToolTip = UIManager.ClassificationSchemeManager.getTitolario(fascicolo.idTitolario).Descrizione;
                                    this.projectLblTitolarioGenerato.Text = ellipsis(projectLblTitolarioGenerato.ToolTip, 20);
                                    break;
                            }
                        }

                        Fascicolo _tempfascicolo = UIManager.ProjectManager.getClassificazioneById(fascicolo.idClassificazione);
                        this.projectLblClassificaGenerato.ToolTip = _tempfascicolo.codice + " - " + _tempfascicolo.descrizione;
                        this.projectLblClassificaGenerato.Text = ellipsis(projectLblClassificaGenerato.ToolTip, 50);
                        this.projectImgConservazione.Enabled = true;
                        this.projectImgStampaEtichette.Enabled = true;

                        if (fascicolo.folderSelezionato == null)
                            fascicolo.folderSelezionato = UIManager.GridManager.GetFolderByIdFasc(UIManager.UserManager.GetInfoUser(), fascicolo);
                        UIManager.ProjectManager.setProjectInSession(fascicolo);

                        if (fascicolo.template != null && !string.IsNullOrEmpty(fascicolo.template.SYSTEM_ID.ToString()) && fascicolo.template.SYSTEM_ID != 0)
                        {
                            this.ProjectLitTypeDocumentHead.Visible = true;
                            this.ProjectLitTypeDocumentValue.Visible = true;
                            this.ProjectLitTypeDocumentValue.Text = fascicolo.template.DESCRIZIONE.Length < 25 ? fascicolo.template.DESCRIZIONE: fascicolo.template.DESCRIZIONE.Substring(0, 25)+" ...";
                            this.ProjectLitTypeDocumentValue.ToolTip = fascicolo.template.DESCRIZIONE;
                        }
                        else
                        {
                            this.ProjectLitTypeDocumentHead.Visible = false;
                            this.ProjectLitTypeDocumentValue.Visible = false;
                        }
                    }
                    else
                    {
                        this.viewLabel(false);
                    }

                    this.LoadKeys();
                    this.VisibiltyRoleFunctions();
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.HiddenFaseDiagramma.Value))
                    {
                        this.SelectedPhaseId = this.HiddenFaseDiagramma.Value;
                        this.HiddenFaseDiagramma.Value = string.Empty;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupPhases", "ajaxModalPopupPhases();", true);
                    }

                    if (!string.IsNullOrEmpty(this.Phases.ReturnValue))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('Phases','');", true);

                        // Verifico se esistono ruoli mancanti
                        //this.MissingRolesList = DiagrammiManager.ChangeStateGetMissingRoles(UIManager.ProjectManager.getProjectInSession().systemID, this.Phases.ReturnValue);
                        this.SelectedState = DiagrammiManager.GetStatoById(this.Phases.ReturnValue);
                        if (this.SelectedState != null && this.SelectedState.STATO_FINALE)
                        {
                            this.projectLblStatoGenerato.Text = Utils.Languages.GetLabelFromCode("prjectStatoRegistroChiuso", UIManager.UserManager.GetUserLanguage());
                            this.projectLblStatoGenerato.CssClass = "close";
                        }
                        else
                        {
                            this.projectLblStatoGenerato.Text = Utils.Languages.GetLabelFromCode("prjectStatoRegistroAperto", UIManager.UserManager.GetUserLanguage());
                            this.projectLblStatoGenerato.CssClass = "open";
                        }
                        

                        if (this.MissingRolesList != null && this.MissingRolesList.Count > 0)
                        {
                            // Esistono ruoli mancanti
                            this.SelectedState = DiagrammiManager.GetStatoById(this.Phases.ReturnValue);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupMissingRoles", "ajaxModalPopupMissingRoles();", true);
                        }
                        else
                        {
                            // Non esistono ruoli mancanti
                            // E' possibile inviare le trasmissioni salvate
                            DiagrammiManager.ChangeStateSendTransmissions(this.Phases.ReturnValue);
                        }
                        this.UpHeaderProject.Update();
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void LoadKeys()
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]))
            {
                this.projectImgStampaEtichette.Visible = false;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_WA_CONSERVAZIONE.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_WA_CONSERVAZIONE.ToString()).Equals("1"))
            {
                this.projectImgConservazione.Visible = false;
            }
        }

        private void VisibiltyRoleFunctions()
        {
            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_CONS"))
            {
                this.projectImgConservazione.Visible = false;
            }

        }

        private void viewLabel(bool visibile)
        {
            this.projectLblCodice.Visible = visibile;
            this.projectLblId.Visible = visibile;
            this.projectlblStato.Visible = visibile;
            this.projectLblTitolario.Visible = visibile;
            this.projectlblClassifica.Visible = visibile;
            this.projectLblRegistroSelezionato.Visible = visibile;
        }
        private string ellipsis(string text, int lenghtMax)
        {
            if (text.Length > lenghtMax)
                return text.Remove(lenghtMax) + "...";
            return text;
        }

        private void InitializeLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            this.projectLblCodice.Text = Utils.Languages.GetLabelFromCode("projectLblCodice", language);
            this.projectLblId.Text = Utils.Languages.GetLabelFromCode("projectLblId", language);
            this.projectLblTitolario.Text = Utils.Languages.GetLabelFromCode("projectLblTitolario", language);
            this.projectlblClassifica.Text = Utils.Languages.GetLabelFromCode("projectlblClassifica", language);
            this.projectLblRegistroSelezionato.Text = Utils.Languages.GetLabelFromCode("projectLblRegistroSelezionato", language);
            this.projectImgStampaEtichette.ToolTip = Utils.Languages.GetLabelFromCode("projectImgStampaEtichette", language);
            this.projectImgStampaEtichette.AlternateText = Utils.Languages.GetLabelFromCode("projectImgStampaEtichette", language);
            this.projectImgConservazione.ToolTip = Utils.Languages.GetLabelFromCode("projectImgConservazione", language);
            this.HistoryPreserved.Title = Utils.Languages.GetLabelFromCode("HistoryPreservedTitle", language);
            this.ProjectLitTypeDocumentHead.Text = Utils.Languages.GetLabelFromCode("DocumentLitTypeDocumentHead", language);
            this.projectlblStato.Text = Utils.Languages.GetLabelFromCode("projectlblStato", language);
            this.Phases.Title = Utils.Languages.GetLabelFromCode("PhasesTitle", language);
            this.MissingRoles.Title = Utils.Languages.GetLabelFromCode("MissingRolesTitle", language);
        }
        public void RefreshHeader()
        {
            Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();

            if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.systemID))
            {
                viewLabel(true);
                this.projectLblIdGenerato.Text = fascicolo.systemID;
                this.projectLblCodiceGenerato.Text = fascicolo.codice;
                switch (fascicolo.stato)
                {
                    case "A":
                        {
                            this.projectLblStatoGenerato.Text = Utils.Languages.GetLabelFromCode("prjectStatoRegistroAperto", UIManager.UserManager.GetUserLanguage());
                            this.projectLblStatoGenerato.CssClass = "open";
                            break;
                        }
                    case "C":
                        {
                            this.projectLblStatoGenerato.Text = Utils.Languages.GetLabelFromCode("prjectStatoRegistroChiuso", UIManager.UserManager.GetUserLanguage());
                            this.projectLblStatoGenerato.CssClass = "close";
                            break;
                        }
                    default:
                        {
                            this.projectLblStatoGenerato.Text = Utils.Languages.GetLabelFromCode("prjectStatoRegistroGiallo", UIManager.UserManager.GetUserLanguage());
                            this.projectLblStatoGenerato.CssClass = "giallo";
                            break;
                        }

                }
                Registro registro = UIManager.RegistryManager.getRegistroBySistemId(fascicolo.idRegistroNodoTit);
                if (registro == null)
                {
                    registro = UIManager.RegistryManager.getRegistroBySistemId(fascicolo.idRegistro);
                }

                if (registro != null)
                    this.projectLblRegistroGenerato.Text = registro.codRegistro;

                string language = UIManager.UserManager.GetUserLanguage();
                OrgTitolario titolario = UIManager.ClassificationSchemeManager.getTitolario(fascicolo.idTitolario);

                switch (titolario.Stato) 
                {
                    case OrgStatiTitolarioEnum.Attivo:
                        this.projectLblTitolarioGenerato.ToolTip = Utils.Languages.GetLabelFromCode("ProjectLblTitolarioAttivo", language).Replace("@@", titolario.DescrizioneLite);
                        this.projectLblTitolarioGenerato.Text = ellipsis(projectLblTitolarioGenerato.ToolTip, 20);
                        break;

                    default:
                        this.projectLblTitolarioGenerato.ToolTip = UIManager.ClassificationSchemeManager.getTitolario(fascicolo.idTitolario).Descrizione;
                        this.projectLblTitolarioGenerato.Text = ellipsis(projectLblTitolarioGenerato.ToolTip, 20);
                        break;
                }

                Fascicolo _tempfascicolo = UIManager.ProjectManager.getClassificazioneById(fascicolo.idClassificazione);
                this.projectLblClassificaGenerato.ToolTip = _tempfascicolo.codice + " - " + _tempfascicolo.descrizione;
                this.projectLblClassificaGenerato.Text = ellipsis(projectLblClassificaGenerato.ToolTip, 50);
                this.projectImgConservazione.Enabled = true;
                this.projectImgStampaEtichette.Enabled = true;

                if (fascicolo.folderSelezionato == null)
                    fascicolo.folderSelezionato = UIManager.GridManager.GetFolderByIdFasc(UIManager.UserManager.GetInfoUser(), fascicolo);
                UIManager.ProjectManager.setProjectInSession(fascicolo);

                if (fascicolo.template != null && !string.IsNullOrEmpty(fascicolo.template.SYSTEM_ID.ToString()) && fascicolo.template.SYSTEM_ID != 0)
                {
                    this.ProjectLitTypeDocumentHead.Visible = true;
                    this.ProjectLitTypeDocumentValue.Visible = true;
                    this.ProjectLitTypeDocumentValue.Text = fascicolo.template.DESCRIZIONE; this.ProjectLitTypeDocumentValue.Text = fascicolo.template.DESCRIZIONE.Length < 25 ? fascicolo.template.DESCRIZIONE : fascicolo.template.DESCRIZIONE.Substring(0, 25) + " ...";
                    this.ProjectLitTypeDocumentValue.ToolTip = fascicolo.template.DESCRIZIONE;
                }
                else
                {
                    this.ProjectLitTypeDocumentHead.Visible = false;
                    this.ProjectLitTypeDocumentValue.Visible = false;
                }
            }
            this.UpHeaderProject.Update();
        }

        public void BuildFasiDiagramma(List<AssPhaseStatoDiagramma> phasesState, string idStatoAttuale, List<string> idStatiSuccessivi)
        {
            containerProjectBottom.Attributes["style"] = "height: 60px";
            this.pnlFasiDiagrammaStato.Controls.Clear();
            this.pnlFasiDiagrammaStato.Visible = true;
            List<string> idPhases = phasesState.Select(x => x.PHASE.SYSTEM_ID).Distinct().ToList();
            foreach (string idPhase in idPhases)
            {
                Phases p = (from p2 in phasesState where p2.PHASE.SYSTEM_ID.Equals(idPhase) select p2.PHASE).FirstOrDefault();

                HtmlGenericControl divFase = new HtmlGenericControl();
                string cssClass = GetCssClassFase(phasesState, p, idStatoAttuale, idStatiSuccessivi);

                HtmlGenericControl anchor = new HtmlGenericControl("a");
                anchor.Attributes.Add("href", "#");
                anchor.InnerText = p.DESCRIZIONE;
                anchor.Attributes.Add("onclick", "$('#HiddenFaseDiagramma').val('" + p.SYSTEM_ID + "');__doPostBack('UpHiddenField');return false;");

                LinkButton lnk = new LinkButton();
                lnk.Attributes.Add("href", "#");
                lnk.Text = p.DESCRIZIONE;
                lnk.CssClass = "clickable";
                lnk.Attributes.Add("onclick", "$('#HiddenFaseDiagramma').val('" + p.SYSTEM_ID + "');__doPostBack('UpHiddenField');return false;");

                HtmlGenericControl li = new HtmlGenericControl("li");
                li.ID = p.SYSTEM_ID;
                li.Attributes.Add("class", cssClass);
                li.Controls.Add(lnk);


                divFase.Controls.Add(li);
                this.pnlFasiDiagrammaStato.Controls.Add(divFase);
            }

            this.UpnlFasiDiagrammaStato.Update();
        }

        private string GetCssClassFase(List<AssPhaseStatoDiagramma> phasesState, Phases fase, string idStatoAttuale, List<string> idStatiSuccessivi)
        {
            AssPhaseStatoDiagramma assPhaseStato = (from p1 in phasesState where p1.PHASE.SYSTEM_ID.Equals(fase.SYSTEM_ID) select p1).FirstOrDefault();

            bool faseIniziale = assPhaseStato.POSITION_PHASE.Equals("1");
            bool faseSelezionata = (from p1 in phasesState where p1.PHASE.SYSTEM_ID.Equals(fase.SYSTEM_ID) && p1.STATO.SYSTEM_ID.ToString().Equals(idStatoAttuale) select p1).FirstOrDefault() != null;
            bool faseSuccessiva = false;
            if (idStatiSuccessivi != null)
                faseSuccessiva = (from p1 in phasesState where p1.PHASE.SYSTEM_ID.Equals(fase.SYSTEM_ID) && idStatiSuccessivi.Contains(p1.STATO.SYSTEM_ID.ToString()) select p1).FirstOrDefault() != null;

            string cssClass = string.Empty;
            if (faseIniziale)
            {
                if (faseSelezionata)
                    cssClass = assPhaseStato.INTERMEDIATE_PHASE ? "phaseInitSelectedIntermedia" : "phaseInitSelected";
                else
                    cssClass = assPhaseStato.INTERMEDIATE_PHASE ? "phaseInitDisabledIntermedia" : "phaseInitDisabled";
            }
            else
            {
                if (faseSelezionata)
                    cssClass = assPhaseStato.INTERMEDIATE_PHASE ? "phaseSelectedIntermedia" : "phaseSelected";
                else if (faseSuccessiva)
                    cssClass = assPhaseStato.INTERMEDIATE_PHASE ? "phaseNextIntermedia" : "phaseNext";
                else
                    cssClass = assPhaseStato.INTERMEDIATE_PHASE ? "phaseDisabledIntermedia" : "phaseDisabled";
            }
            return cssClass;
        }
        #region Printer

        protected void ProjectImgStampaEtichette_Click(object o, EventArgs e)
        {
            //try
            //{
            //    this.EnumLabel = "1";
            //    PrintLabel_alreadyPrinted = false;
            //    PrintLabel_alreadyPrinted2 = false;
            //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "print_labels", "ajaxModalPopupPrintLabel();", true);
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}


            try
            {
                DocsPaWR.FileDocumento fileRep = ProjectManager.reportFascette(UIManager.ProjectManager.getProjectInSession().codice, null);

                HttpContext.Current.Session["visualReportAlreadyDownloaded" + Session.SessionID] = null;
                FileManager.setSelectedFileReport(this.Page, fileRep, "../popup");
                if (fileRep != null)
                {
                    exportDatiSessionManager session = new exportDatiSessionManager();
                    session.SetSessionExportFile(fileRep);
                }

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }

        }

        public string EnumLabel
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["enumLabel"] != null)
                {
                    result = HttpContext.Current.Session["enumLabel"].ToString();
                }
                return result;

            }
            set
            {
                HttpContext.Current.Session["enumLabel"] = value;
            }
        }

        public bool PrintLabel_alreadyPrinted
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["printlabel_alreadyprinted"] != null)
                {
                    result = Boolean.Parse(HttpContext.Current.Session["printlabel_alreadyprinted"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["printlabel_alreadyprinted"] = value.ToString();
            }
        }

        public bool PrintLabel_alreadyPrinted2
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["printlabel_alreadyprinted2"] != null)
                {
                    result = Boolean.Parse(HttpContext.Current.Session["printlabel_alreadyprinted2"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["printlabel_alreadyprinted2"] = value.ToString();
            }
        }
    #endregion
    }
}
