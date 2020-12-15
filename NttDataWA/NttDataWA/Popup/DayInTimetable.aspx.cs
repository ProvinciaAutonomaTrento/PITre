using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDatalLibrary;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class DayInTimetable : System.Web.UI.Page
    {
        #region Properties

        private List<DocsPaWR.Task> TaskDayInTimetable
        {
            get
            {
                if (HttpContext.Current.Session["TaskDayInTimetable"] != null)
                {
                    return HttpContext.Current.Session["TaskDayInTimetable"] as List<DocsPaWR.Task>;
                }
                return null;
            }
            set
            {
                HttpContext.Current.Session["TaskDayInTimetable"] = value;
            }
        }

        private DocsPaWR.Task TaskSelectedInDay
        {
            get
            {
                if (HttpContext.Current.Session["Task"] != null)
                {
                    return HttpContext.Current.Session["Task"] as DocsPaWR.Task;
                }
                return null;
            }
            set
            {
                HttpContext.Current.Session["Task"] = value;
            }
        }
        private string NoteCompleteTask
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["NoteCompleteTask"] != null)
                {
                    result = HttpContext.Current.Session["NoteCompleteTask"] as String;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["NoteCompleteTask"] = value;
            }
        }

        private string TypeTask
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["TypeTask"] != null)
                {
                    result = HttpContext.Current.Session["TypeTask"] as String;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TypeTask"] = value;
            }
        }
        #endregion

        #region Const

        private const string RICEVUTI = "R";
        private const string ASSEGNATI = "A";

        #endregion

        #region Standard method
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializeLanguage();
                this.InitializePage();
            }
            else
            {
                this.ReadRetValueFromPopup();
                this.RefreshScript();
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.DayInTimetableClose.Text = Utils.Languages.GetLabelFromCode("DayInTimetableClose", language);
            this.ReopenTask.Title = Utils.Languages.GetLabelFromCode("ReopenTaskTitle", language);
        }

        private void InitializePage()
        {
            RepListTask_Bind();
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        private void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.ReopenTask.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ReopenTask','');", true);
                if (this.RiapriLavorazione())
                {
                    this.TaskDayInTimetable.Remove(this.TaskSelectedInDay);
                    this.RepListTask_Bind();
                    this.UpRepiterTask.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpdateExpand", "UpdateExpand();", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorTaskRiaperturaLavorazione', 'error', '');} else {parent.ajaxDialogModal('ErrorTaskRiaperturaLavorazione', 'error', '');}", true);
                }
                this.TaskSelectedInDay = null;
                return;
            }
            if (!string.IsNullOrEmpty(this.HiddenCancelTask1.Value))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                this.HiddenCancelTask1.Value = string.Empty;
                DocsPaWR.Task task = TaskSelectedInDay;
                this.TaskSelectedInDay = null;
                this.HiddenCancelTask1.Value = string.Empty;
                if (UIManager.TaskManager.AnnullaTask(task))
                {
                    this.TaskDayInTimetable.Remove(task);
                    this.RepListTask_Bind();
                    this.UpRepiterTask.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpdateExpand", "UpdateExpand();", true);
                }
                else
                {
                    string msg = "ErrorBlockTask";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                }
                return;
            }
            if (!string.IsNullOrEmpty(this.HiddenRemoveTask1.Value))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                this.HiddenRemoveTask1.Value = string.Empty;
                DocsPaWR.Task task = TaskSelectedInDay;
                this.TaskSelectedInDay = null;
                this.HiddenCancelTask1.Value = string.Empty;
                if (UIManager.TaskManager.ChiudiTask(task))
                {
                    this.TaskDayInTimetable.Remove(task);
                    this.RepListTask_Bind();
                    this.UpRepiterTask.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpdateExpand", "UpdateExpand();", true);
                }
                else
                {
                    string msg = "ErrorRemoveTask";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                }
                return;
            }
            this.TaskSelectedInDay = null;
        }
        #endregion

        #region Event button

        protected void DayInTimetableClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('DayInTimetable','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void btnCompleteTaskPostback_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                DocsPaWR.Task task = TaskSelectedInDay;
                this.TaskSelectedInDay = null;
                string note = UIManager.TaskManager.TagIdContributo.LABEL_ATTIVITA_CONCLUSA + System.DateTime.Now.ToString() + UIManager.TaskManager.TagIdContributo.LABEL_ATTIVITA_CONCLUSA_C;
                if (!string.IsNullOrEmpty(task.ID_PROFILE_REVIEW))
                    note += TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO + task.ID_PROFILE_REVIEW + TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO_C;
                if (!string.IsNullOrEmpty(this.NoteCompleteTask))
                    note += this.NoteCompleteTask + " ";

                if (UIManager.TaskManager.ChiudiLavorazioneTask(task, note))
                {
                    this.TaskDayInTimetable.Remove(task);
                    this.RepListTask_Bind();
                    this.UpRepiterTask.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpdateExpand", "UpdateExpand();", true);
                }
                else
                {
                    string msg = "ErrorCloseTask";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #region Repiter


        private void RepListTask_Bind()
        {
            if (this.TaskDayInTimetable != null && this.TaskDayInTimetable.Count > 0)
            {
                TaskDayInTimetable = TaskDayInTimetable.OrderBy(c => c.STATO_TASK.DATA_LAVORAZIONE).ToList();
                this.repListTask.DataSource = this.TaskDayInTimetable;
                this.repListTask.DataBind();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.closeAjaxModal('DayInTimetable',''); parent.ajaxDialogModal('WarningDayInTimetableNoItem', 'warning', '','',null,null,'')", true);
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeAJM", "parent.closeAjaxModal('DayInTimetable','');", true); ;
            }
        }

        protected void RepListTask_DataBinding(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    DocsPaWR.Task task = e.Item.DataItem as DocsPaWR.Task;
                    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("it-IT");
                    string today = DateTime.Now.ToString(ci);
                    string imgUrl = "../Images/Icons/";

                    if (task.STATO_TASK.STATO == StatoAvanzamento.Aperto || task.STATO_TASK.STATO == StatoAvanzamento.Riaperto)
                    {
                        if (task.STATO_TASK.STATO == StatoAvanzamento.Aperto)
                            imgUrl += !Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(task.STATO_TASK.DATA_SCADENZA).ToShortDateString(), Utils.dateformat.ConvertToDate(today).ToShortDateString()) ? "task_scaduto_aperto.png" : "task_in_corso.png";
                        else
                            imgUrl += !Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(task.STATO_TASK.DATA_SCADENZA).ToShortDateString(), Utils.dateformat.ConvertToDate(today).ToShortDateString()) ? "task_scaduto_riaperto.png" : "task_in_corso_riaperto.png";
                        (e.Item.FindControl("pnlDataCompletamento") as Panel).Visible = false;
                        (e.Item.FindControl("ImgRemoveTask") as CustomImageButton).Visible = false;
                        (e.Item.FindControl("ImgRiapriLavorazione") as CustomImageButton).Visible = false;
                        (e.Item.FindControl("ImgCreaContributo") as CustomImageButton).Visible = (this.TypeTask.Equals(RICEVUTI) && string.IsNullOrEmpty(task.ID_PROFILE_REVIEW));
                        (e.Item.FindControl("ImgCloseTask") as CustomImageButton).Visible = this.TypeTask.Equals(RICEVUTI);
                        (e.Item.FindControl("ImgBlockTask") as CustomImageButton).Visible = this.TypeTask.Equals(ASSEGNATI);
                    }
                    else
                    {
                        imgUrl += !Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(task.STATO_TASK.DATA_SCADENZA).ToShortDateString(), Utils.dateformat.ConvertToDate(task.STATO_TASK.DATA_LAVORAZIONE).ToShortDateString()) ? "task_chiuso_oltre_scadenza.png" : "task_completato_in_tempo.png";
                        (e.Item.FindControl("ImgCreaContributo") as CustomImageButton).Visible = false;
                        (e.Item.FindControl("ImgCloseTask") as CustomImageButton).Visible = false;
                        (e.Item.FindControl("ImgBlockTask") as CustomImageButton).Visible = false;
                        (e.Item.FindControl("ImgRemoveTask") as CustomImageButton).Visible = this.TypeTask.Equals(ASSEGNATI);
                        (e.Item.FindControl("ImgRiapriLavorazione") as CustomImageButton).Visible = this.TypeTask.Equals(ASSEGNATI);
                    }

                    (e.Item.FindControl("imgStatoTask") as Image).ImageUrl = imgUrl;
                    (e.Item.FindControl("ImgViewDocument") as CustomImageButton).Visible = !string.IsNullOrEmpty(task.ID_PROFILE);
                    (e.Item.FindControl("ImageViewProject") as CustomImageButton).Visible = !string.IsNullOrEmpty(task.ID_PROJECT);
                    (e.Item.FindControl("ImgViewContributo") as CustomImageButton).Visible = !string.IsNullOrEmpty(task.ID_PROFILE_REVIEW);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void RepListTask_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            string idTask = (e.Item.FindControl("idTask") as HiddenField).Value;
            DocsPaWR.Task task = (from t in this.TaskDayInTimetable where t.ID_TASK.Equals(idTask) select t).FirstOrDefault();
            string idObject = string.Empty;
            switch (e.CommandName)
            {
                case "ViewContributo":
                    string idReview = task.ID_PROFILE_REVIEW;
                    #region navigation
                    List<Navigation.NavigationObject> navigationList2 = Navigation.NavigationUtils.GetNavigationList();
                    Navigation.NavigationObject actualPage2 = new Navigation.NavigationObject();
                    actualPage2.IdObject = idReview;
                    actualPage2.OriginalObjectId = idReview;
                    actualPage2.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME_TASK.ToString(), string.Empty);
                    actualPage2.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.HOME_TASK.ToString(), true, this.Page);
                    actualPage2.CodePage = Navigation.NavigationUtils.NamePage.HOME_TASK.ToString();
                    actualPage2.Page = "TASK.ASPX";
                    navigationList2.Add(actualPage2);
                    Navigation.NavigationUtils.SetNavigationList(navigationList2);
                    #endregion
                    SchedaDocumento schedaDocumento = UIManager.DocumentManager.getDocumentDetails(this.Page, idReview, idReview);
                    UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "closeAJM", "parent.closeAjaxModal('DayInTimetable','document');", true);
                    return;
                case "ViewObjectTask":
                    idTask = task.ID_TASK;
                    idObject = task.ID_PROFILE;
                    if (!string.IsNullOrEmpty(idObject))
                    {
                        #region navigation
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                        actualPage.IdObject = idObject;
                        actualPage.OriginalObjectId = idObject;
                        actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME_TASK.ToString(), string.Empty);
                        actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.HOME_TASK.ToString(), true, this.Page);
                        actualPage.CodePage = Navigation.NavigationUtils.NamePage.HOME_TASK.ToString();

                        actualPage.Page = "TASK.ASPX";
                        navigationList.Add(actualPage);
                        Navigation.NavigationUtils.SetNavigationList(navigationList);
                        #endregion
                        SchedaDocumento schedaDoc = UIManager.DocumentManager.getDocumentDetails(this.Page, idObject, idObject);
                        UIManager.DocumentManager.setSelectedRecord(schedaDoc);
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "closeAJM", "parent.closeAjaxModal('DayInTimetable','document');", true);
                        return;
                    }
                    else
                    {
                        idObject = task.ID_PROJECT;
                        #region navigation
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                        actualPage.IdObject = idObject;
                        actualPage.OriginalObjectId = idObject;
                        actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME_TASK.ToString(), string.Empty);
                        actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.HOME_TASK.ToString(), true, this.Page);
                        actualPage.CodePage = Navigation.NavigationUtils.NamePage.HOME_TASK.ToString();
                        actualPage.Page = "TASK.ASPX";
                        navigationList.Add(actualPage);
                        Navigation.NavigationUtils.SetNavigationList(navigationList);

                        #endregion
                        Fascicolo fascicolo = UIManager.ProjectManager.getFascicoloById(idObject);
                        if (fascicolo != null)
                            fascicolo.template = ProfilerProjectManager.getTemplateFascDettagli(fascicolo.systemID);
                        UIManager.ProjectManager.setProjectInSession(fascicolo);
                        if (fascicolo == null || ProjectManager.CheckRevocationAcl())
                        {
                            ProjectManager.setProjectInSession(null);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('RevocationAclIndex', 'warning', '','',null,null,'')", true);
                            return;
                        }
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "closeAJM", "parent.closeAjaxModal('DayInTimetable','project');", true);
                        return;
                    }
                case "RemoveTask":
                    this.TaskSelectedInDay = task;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('ConfirmRemoveTask', 'HiddenRemoveTask1', '');", true);
                    break;
                case "AnnullaTask":
                    this.TaskSelectedInDay = task;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('ConfirmCancelTask', 'HiddenCancelTask1', '');", true);
                    break;
                case "CloseTask":
                    this.TaskSelectedInDay = task;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CompleteTask", "parent.ajaxModalPopupCompleteTaskFromDay();", true);
                    break;
                case "RiapriLavorazione":
                    this.TaskSelectedInDay = task;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ReopenTask", "ajaxModalPopupReopenTask();", true);
                    break;
                case "CreaContributo":
                    idTask = task.ID_TASK;
                    idObject = !string.IsNullOrEmpty(task.ID_PROFILE) ? task.ID_PROFILE : task.ID_PROJECT;
                    string error = this.CreaContributo(task);
                    if (string.IsNullOrEmpty(error))
                    {
                        #region navigation
                        List<Navigation.NavigationObject> navigationList1 = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject actualPage1 = new Navigation.NavigationObject();
                        actualPage1.IdObject = idObject;
                        actualPage1.OriginalObjectId = idObject;
                        actualPage1.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME_TASK.ToString(), string.Empty);
                        actualPage1.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.HOME_TASK.ToString(), true, this.Page);
                        actualPage1.CodePage = Navigation.NavigationUtils.NamePage.HOME_TASK.ToString();

                        actualPage1.Page = "TASK.ASPX";
                        navigationList1.Add(actualPage1);
                        Navigation.NavigationUtils.SetNavigationList(navigationList1);
                        #endregion
                        HttpContext.Current.Session["Task"] = task;
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "closeAJM", "parent.closeAjaxModal('DayInTimetable','document');", true);
                    }
                    else if (error == "AnswerChooseRecipient")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "AnswerChooseRecipient", "ajaxModalPopupAnswerChooseRecipient();", true);
                    }
                    else
                    {
                        HttpContext.Current.Session["Task"] = null;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('" + error.Replace("'", @"\'") + "', 'warning', '','',null,null,'')", true);
                    }
                    break;
            }
        }


        protected string GetAssegnatario(DocsPaWR.Task task)
        {
            if (task.RUOLO_MITTENTE != null)
                return task.UTENTE_MITTENTE.descrizione + " (" + task.RUOLO_MITTENTE.descrizione + ")";
            else
                return string.Empty;
        }

        protected string GetIdDocCodFasc(DocsPaWR.Task task)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(task.ID_PROFILE))
                result = task.ID_PROFILE;
            else
                result = task.COD_PROJECT;

            return result;
        }
        #endregion

        #region Utils

        private bool RiapriLavorazione()
        {
            bool retValue = false;
            try
            {
                if (TaskManager.RiapriLavorazione(this.TaskSelectedInDay))
                {
                    retValue = true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return retValue;
        }


        private string CreaContributo(DocsPaWR.Task task)
        {
            string msg = string.Empty;
            Templates templateToMerge = null;
            SchedaDocumento document = UIManager.DocumentManager.NewSchedaDocumento();
            document.oggetto = new Oggetto();
            document.tipoProto = "G";
            if (!string.IsNullOrEmpty(task.ID_TIPO_ATTO))
            {
                //Devo aggiungere controlli di visibilità della tipologia
                document.template = UIManager.DocumentManager.getTemplateById(task.ID_TIPO_ATTO, UserManager.GetInfoUser());
                document.oggetto.descrizione = document.template.DESCRIZIONE + " - ";
                templateToMerge = !string.IsNullOrEmpty(task.ID_PROFILE) ? ProfilerDocManager.getTemplateDettagli(task.ID_PROFILE) : UIManager.ProfilerProjectManager.getTemplateFascDettagli(task.ID_PROJECT);
                if (templateToMerge != null)
                {
                    document.template = MappingTemplates(templateToMerge, document.template);
                }
            }

            if (!string.IsNullOrEmpty(task.ID_PROJECT))
            {
                Fascicolo fascicolo = UIManager.ProjectManager.getFascicoloById(task.ID_PROJECT);
                UIManager.ProjectManager.setProjectInSession(fascicolo);
                if (fascicolo == null || ProjectManager.CheckRevocationAcl())
                {
                    ProjectManager.setProjectInSession(null);
                    msg = "RevocationAclIndex";
                    return msg;
                }
                document.oggetto.descrizione += fascicolo.descrizione;
                HttpContext.Current.Session["DocumentAnswerFromProject"] = true;
            }
            else
            {
                SchedaDocumento schedaDocDiPartenza = DocumentManager.getDocumentDetails(this, task.ID_PROFILE, task.ID_PROFILE);
                document.oggetto.descrizione += schedaDocDiPartenza.oggetto.descrizione;
                DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(schedaDocDiPartenza);
                document.rispostaDocumento = infoDoc;
                switch (schedaDocDiPartenza.tipoProto)
                {
                    case "A":
                        document.tipoProto = "P";
                        document.protocollo = new DocsPaWR.ProtocolloUscita();
                        document.registro = schedaDocDiPartenza.registro;
                        ((DocsPaWR.ProtocolloUscita)document.protocollo).destinatari = new DocsPaWR.Corrispondente[1];
                        ((DocsPaWR.ProtocolloUscita)document.protocollo).destinatari[0] = new DocsPaWR.Corrispondente();
                        ((DocsPaWR.ProtocolloUscita)document.protocollo).destinatari[0] = ((DocsPaWR.ProtocolloEntrata)schedaDocDiPartenza.protocollo).mittente;
                        if (EnableSenderDefault())
                        {
                            DocsPaWR.Corrispondente corr = RoleManager.GetRoleInSession().uo;
                            ((DocsPaWR.ProtocolloUscita)document.protocollo).mittente = corr;
                        }
                        break;
                    case "P":
                        document.tipoProto = "A";
                        document.protocollo = new DocsPaWR.ProtocolloEntrata();
                        document.registro = schedaDocDiPartenza.registro;
                        if (EnableSenderDefault())
                        {
                            if (((DocsPaWR.ProtocolloUscita)schedaDocDiPartenza.protocollo).destinatari.Count() > 1)
                            {
                                //this.SchedaDocContributo = document;
                                UIManager.DocumentManager.setSelectedRecord(schedaDocDiPartenza);
                                msg = "AnswerChooseRecipient";
                                return msg;
                            }
                            else
                                ((DocsPaWR.ProtocolloEntrata)document.protocollo).mittente = ((DocsPaWR.ProtocolloUscita)schedaDocDiPartenza.protocollo).destinatari[0];
                        }
                        break;
                    case "I":
                        document.tipoProto = "I";
                        document.protocollo = new DocsPaWR.ProtocolloInterno();
                        document.registro = schedaDocDiPartenza.registro;
                        if (EnableSenderDefault())
                        {
                            ((DocsPaWR.ProtocolloInterno)document.protocollo).mittente = ((DocsPaWR.ProtocolloInterno)schedaDocDiPartenza.protocollo).mittente;
                        }
                        ((DocsPaWR.ProtocolloInterno)document.protocollo).destinatari = ((DocsPaWR.ProtocolloInterno)schedaDocDiPartenza.protocollo).destinatari;
                        break;
                }
            }
            UIManager.DocumentManager.setSelectedRecord(document);
            return msg;
        }

        private bool EnableSenderDefault()
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.MITTENTE_DEFAULT.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.MITTENTE_DEFAULT.ToString()].Equals("1"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Templates MappingTemplates(Templates templateOrigin, Templates templateDest)
        {
            DocsPaWR.OggettoCustom oggettoCustomTemp;
            for (int i = 0; i < templateDest.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)templateDest.ELENCO_OGGETTI[i];
                oggettoCustomTemp = (from ogg in templateOrigin.ELENCO_OGGETTI
                                     where ogg.TIPO.DESCRIZIONE_TIPO.Equals(oggettoCustom.TIPO.DESCRIZIONE_TIPO) && ogg.DESCRIZIONE.Equals(oggettoCustom.DESCRIZIONE)
                                     select ogg).FirstOrDefault();
                if (oggettoCustomTemp != null)
                {
                    oggettoCustom.VALORE_DATABASE = oggettoCustomTemp.VALORE_DATABASE;
                    oggettoCustom.VALORI_SELEZIONATI = oggettoCustomTemp.VALORI_SELEZIONATI;
                }
            }
            return templateDest;
        }

        #endregion


    }
}