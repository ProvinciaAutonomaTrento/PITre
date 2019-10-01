using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using System.Xml.Linq;
using NttDatalLibrary;
using NttDataWA.Utils;

namespace NttDataWA.Home
{
    public partial class Task : System.Web.UI.Page
    {
        #region Properties

        private string SelectedRow
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["Task.selectedRow"] != null)
                {
                    result = HttpContext.Current.Session["Task.selectedRow"] as String;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["Task.selectedRow"] = value;
            }
        }

        private DocsPaWR.Task TaskSelected
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

        /// <summary>
        /// Number of result in page
        /// </summary>
        public int PageSize
        {
            get
            {
                int result = 10;
                if (HttpContext.Current.Session["pageSizeDocument"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["pageSizeDocument"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["pageSizeDocument"] = value;
            }
        }

        private SchedaDocumento SchedaDocContributo
        {
            get
            {
                SchedaDocumento result = null;
                if (HttpContext.Current.Session["SchedaDocContributo"] != null)
                {
                    result = HttpContext.Current.Session["SchedaDocContributo"] as SchedaDocumento;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["SchedaDocContributo"] = value;
            }
        }

        private int SelectedPage
        {
            get
            {
                int toReturn = 1;
                if (HttpContext.Current.Session["selectedPage"] != null) Int32.TryParse(HttpContext.Current.Session["selectedPage"].ToString(), out toReturn);
                if (toReturn < 1) toReturn = 1;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["selectedPage"] = value;
            }
        }

        private List<DocsPaWR.Task> ListTask
        {
            get
            {
                List<DocsPaWR.Task> result = null;
                if (HttpContext.Current.Session["ListTask"] != null)
                {
                    result = HttpContext.Current.Session["ListTask"] as List<DocsPaWR.Task>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ListTask"] = value;
            }

        }

        #endregion

        #region Costanti

        private const string CLOSE_POPUP_COMPLETE_TASK_FROM_DAY = "closePopupCompleteTaskFromDay";
        private const string CLOSE_POPUP_DAY_IN_TIMETABLE = "closePopupDayInTimetable";
        private const string RICEVUTI = "R";
        private const string ASSEGNATI = "A";

        #endregion

        #region Standard Method

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.InitializeLanguage();
                this.InitializePage();
            }
            else
            {
                ReadRetValueFromPopup();
            }
            this.RefreshScript();
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CombineRowsHover", "CombineRowsHover();", true);
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.headerHomeLblRole.Text = Utils.Languages.GetLabelFromCode("HeaderHomeLblRole", language);
            this.RblTypeTask.Items[0].Text = Utils.Languages.GetLabelFromCode("TaskRblTypeTaskReceived", language);
            this.RblTypeTask.Items[1].Text = Utils.Languages.GetLabelFromCode("TaskRblTypeTaskAssigned", language);
            this.CompleteTask.Title = Utils.Languages.GetLabelFromCode("CompleteTaskTitle", language);
            this.ReopenTask.Title = Utils.Languages.GetLabelFromCode("ReopenTaskTitle", language);
            this.CompleteTaskFromDay.Title = Utils.Languages.GetLabelFromCode("CompleteTaskTitle", language);
            this.AnswerChooseRecipient.Title = Utils.Languages.GetLabelFromCode("AnswerChooseRecipient", language);
            this.cbxTaskChiusi.Text = Utils.Languages.GetLabelFromCode("TaskViewTaskClosed", language);
            this.ImageViewGrid.ToolTip = Utils.Languages.GetLabelFromCode("TaskImageViewGrid", language);
            this.ImageViewCalendar.ToolTip = Utils.Languages.GetLabelFromCode("TaskImageViewCalendar", language);
        }

        private void InitializePage()
        {
            this.ClearSessionProperties();
            this.LoadRolesUser();
            this.ImageViewGrid.Enabled = false;
            this.GridViewResult_Bind();
        }

        private void ClearSessionProperties()
        {
            this.TaskSelected = null;
            HttpContext.Current.Session["Answer.DocumentWIP"] = null;
            this.SchedaDocContributo = null;
            this.NoteCompleteTask = null;
        }

        protected void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.CompleteTask.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('CompleteTask','');", true);
                GridViewRow row = this.gridViewResult.Rows[Convert.ToInt32(SelectedRow)];
                string idTask = (row.FindControl("idTask") as Label).Text;
                string idTrasmSingola = (row.FindControl("idTrasmSingola") as Label).Text;
                string idProfile = (row.FindControl("idProfile") as Label).Text;
                string idProject = (row.FindControl("idProject") as Label).Text;
                string idProfileReview = (row.FindControl("idProfileReview") as Label).Text;
                string idStatoTask = (row.FindControl("idStatoTask") as Label).Text;
                DocsPaWR.Task task = new DocsPaWR.Task() { ID_TASK = idTask, ID_TRASM_SINGOLA = idTrasmSingola, ID_PROFILE = idProfile, ID_PROJECT = idProject, ID_PROFILE_REVIEW = idProfileReview };
                task.STATO_TASK = new StatoTask() { ID_STATO_TASK = idStatoTask };
                string note = UIManager.TaskManager.TagIdContributo.LABEL_ATTIVITA_CONCLUSA + System.DateTime.Now.ToString() + UIManager.TaskManager.TagIdContributo.LABEL_ATTIVITA_CONCLUSA_C;
                if (!string.IsNullOrEmpty(idProfileReview))
                    note += TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO + idProfileReview + TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO_C;
                if (!string.IsNullOrEmpty(this.NoteCompleteTask))
                    note +=  Utils.Languages.GetLabelFromCode("TaskNoteTrasm", UIManager.UserManager.GetUserLanguage()) + " ";
                note += TaskManager.TagIdContributo.LABEL_TEXT_WRAP;
                task.STATO_TASK.NOTE_LAVORAZIONE = this.NoteCompleteTask;
                if (UIManager.TaskManager.ChiudiLavorazioneTask(task, note))
                {
                    this.GridViewResult_Bind();
                    this.UpnlGrid.Update();
                }
                else
                {
                    string msg = "ErrorCloseTask";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                }
            }
            if (!string.IsNullOrEmpty(this.ReopenTask.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ReopenTask','');", true);
                if (this.RiapriLavorazione())
                {
                    this.GridViewResult_Bind();
                    this.UpnlGrid.Update();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorTaskRiaperturaLavorazione', 'error', '');} else {parent.ajaxDialogModal('ErrorTaskRiaperturaLavorazione', 'error', '');}", true);
                }
                this.TaskSelected = null;
            }
            if (!string.IsNullOrEmpty(this.HiddenCancelTask.Value))
            {
                this.HiddenCancelTask.Value = string.Empty;
                GridViewRow row = this.gridViewResult.Rows[Convert.ToInt32(SelectedRow)];

                string idTask = (row.FindControl("idTask") as Label).Text;
                string idTrasmSingola = (row.FindControl("idTrasmSingola") as Label).Text;
                string idProfile = (row.FindControl("idProfile") as Label).Text;
                string idProject = (row.FindControl("idProject") as Label).Text;
                DocsPaWR.Task task = new DocsPaWR.Task() { ID_TASK = idTask, ID_TRASM_SINGOLA = idTrasmSingola, ID_PROFILE = idProfile, ID_PROJECT = idProject };
                if (UIManager.TaskManager.AnnullaTask(task))
                {
                    this.GridViewResult_Bind();
                    this.UpnlGrid.Update();
                }
                else
                {
                    string msg = "ErrorBlockTask";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                }
                return;
            }
            if (!string.IsNullOrEmpty(this.HiddenRemoveTask.Value))
            {
                this.HiddenRemoveTask.Value = string.Empty;
                GridViewRow row = this.gridViewResult.Rows[Convert.ToInt32(SelectedRow)];

                string idTask = (row.FindControl("idTask") as Label).Text;
                string idStatoTask = (row.FindControl("idStatoTask") as Label).Text;
                DocsPaWR.Task task = new DocsPaWR.Task() { ID_TASK = idTask };
                task.STATO_TASK = new StatoTask() { ID_STATO_TASK = idStatoTask };
                if (UIManager.TaskManager.ChiudiTask(task))
                {
                    this.GridViewResult_Bind();
                    this.UpnlGrid.Update();
                }
                else
                {
                    string msg = "ErrorRemoveTask";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                }
                return;
            }
            if (!string.IsNullOrEmpty(this.AnswerChooseRecipient.ReturnValue))
            {
                SchedaDocumento docWIP = HttpContext.Current.Session["Answer.DocumentWIP"] as SchedaDocumento;
                ((DocsPaWR.ProtocolloEntrata)SchedaDocContributo.protocollo).mittente = ((DocsPaWR.ProtocolloEntrata)docWIP.protocollo).mittente;
                DocumentManager.setSelectedRecord(SchedaDocContributo);
                this.AnswerChooseRecipient.ReturnValue = string.Empty;
                HttpContext.Current.Session["Answer.DocumentWIP"] = null;
                SchedaDocContributo = null;
                GridViewRow row = this.gridViewResult.Rows[Convert.ToInt32(SelectedRow)];
                TaskSelected = new DocsPaWR.Task()
                {
                    ID_TASK = (row.FindControl("idTask") as Label).Text,
                    ID_PROFILE = (row.FindControl("idProfile") as Label).Text,
                    ID_PROJECT = (row.FindControl("idProject") as Label).Text,
                    ID_RAGIONE_TRASM = (row.FindControl("idRagione") as Label).Text,
                    RUOLO_MITTENTE = new Ruolo() { idGruppo = (row.FindControl("idGruppoMittente") as Label).Text },
                    UTENTE_MITTENTE = new Utente() { idPeople = (row.FindControl("idPeopleMittente") as Label).Text }
                };
                #region navigation
                string idObject = !string.IsNullOrEmpty((row.FindControl("idProfile") as Label).Text) ? (row.FindControl("idProfile") as Label).Text : (row.FindControl("idProject") as Label).Text;
                List<Navigation.NavigationObject> navigationList1 = Navigation.NavigationUtils.GetNavigationList();
                Navigation.NavigationObject actualPage1 = new Navigation.NavigationObject();
                actualPage1.IdObject = idObject;
                actualPage1.OriginalObjectId = idObject;
                actualPage1.NumPage = this.gridViewResult.PageIndex.ToString();
                actualPage1.PageSize = this.gridViewResult.PageSize.ToString();

                actualPage1.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME_TASK.ToString(), string.Empty);
                actualPage1.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.HOME_TASK.ToString(), true, this.Page);
                actualPage1.CodePage = Navigation.NavigationUtils.NamePage.HOME_TASK.ToString();

                actualPage1.Page = "TASK.ASPX";
                navigationList1.Add(actualPage1);
                Navigation.NavigationUtils.SetNavigationList(navigationList1);
                #endregion
                Response.Redirect("~/Document/Document.aspx");
            }
            if (!string.IsNullOrEmpty(this.hiddenValueTask.Value))
            {
                DocsPaWR.Task task = (from t in this.ListTask
                                      where t.ID_TASK.Equals(this.hiddenValueTask.Value)
                                      select t).FirstOrDefault();
                this.TaskDayInTimetable = new List<DocsPaWR.Task>() { task };
                this.hiddenValueTask.Value = string.Empty;
                this.UpPnlButtons.Update();
                string dataScadenza = Utils.dateformat.ConvertToDate(task.STATO_TASK.DATA_SCADENZA).ToString("dd MMMM yyyy");
                this.TypeTask = (this.RblTypeTask.SelectedValue.Equals("0") ? "R" : "A");
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "DayInTimetable", "ajaxModalPopupTitleDayInTimetable('" + dataScadenza + "');", true);
            }
            if (!string.IsNullOrEmpty(HiddenDataScadenza.Value))
            {
                this.TaskDayInTimetable = (from t in this.ListTask
                                           where !string.IsNullOrEmpty(t.STATO_TASK.DATA_SCADENZA) &&
                                           Utils.dateformat.ConvertToDate(t.STATO_TASK.DATA_SCADENZA).ToShortDateString().Equals(HiddenDataScadenza.Value)
                                           select t).ToList();
                string dataScadenza = Utils.dateformat.ConvertToDate(HiddenDataScadenza.Value).ToString("dd MMMM yyyy");
                this.HiddenDataScadenza.Value = string.Empty;
                this.UpPnlButtons.Update();
                this.TypeTask = (this.RblTypeTask.SelectedValue.Equals("0") ? "R" : "A");
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "DayInTimetable", "ajaxModalPopupTitleDayInTimetable('" + dataScadenza + "');", true);
            }
            if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_COMPLETE_TASK_FROM_DAY)))
            {
                if (!string.IsNullOrEmpty(this.CompleteTaskFromDay.ReturnValue))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('CompleteTaskFromDay','');", true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_DayInTimetable').contentWindow.closeCompleteTaskPopup();", true);
                    return;
                }
            }
            if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_DAY_IN_TIMETABLE)))
            {
                this.Calendar_Bind();
                this.UpnlCalendar.Update();
            }
            if (!string.IsNullOrEmpty(DayInTimetable.ReturnValue))
            {
                if (DayInTimetable.ReturnValue.Equals("document"))
                {
                    Response.Redirect("../Document/Document.aspx");
                }
                else if (DayInTimetable.ReturnValue.Equals("project"))
                {
                    Response.Redirect("~/Project/project.aspx");
                }
            }
            else
            {
                this.TaskSelected = null;
                HttpContext.Current.Session["Answer.DocumentWIP"] = null;
                SchedaDocContributo = null;
            }
        }

        #endregion

        #region Calendar

        private void Calendar_Bind()
        {
            try
            {
                this.ListTask = this.GetListaTask();
                this.calendar.SelectedDate = DateTime.Now;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void Calendar_PreRender(object sender, EventArgs e)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            if (language.ToUpper() == "ENGLISH")
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            }
        }

        protected void Calendar_SelectionChanged(Object sender, EventArgs e)
        {
            foreach (DateTime day in calendar.SelectedDates)
            {
                string dataScadenza = Utils.dateformat.ConvertToDate(day.ToString()).ToString("dd MMMM yyyy");
                this.TaskDayInTimetable = (from t in this.ListTask
                                           where !string.IsNullOrEmpty(t.STATO_TASK.DATA_SCADENZA) &&
                                           Utils.dateformat.ConvertToDate(t.STATO_TASK.DATA_SCADENZA).ToShortDateString().Equals(day.Date.ToShortDateString())
                                           select t).ToList();
                this.TypeTask = (this.RblTypeTask.SelectedValue.Equals("0") ? "R" : "A");
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "DayInTimetable", "ajaxModalPopupTitleDayInTimetable('" + dataScadenza + "');", true);
            }
            calendar.SelectedDates.Clear();
        }

        protected void Calendar_DayRender(object sender, DayRenderEventArgs e)
        {
            CalendarDay d = ((DayRenderEventArgs)e).Day;
            TableCell c = ((DayRenderEventArgs)e).Cell;

            if (this.ListTask != null && this.ListTask.Count > 0)
            {
                List<DocsPaWR.Task> task = (from t in this.ListTask
                                            where !string.IsNullOrEmpty(t.STATO_TASK.DATA_SCADENZA) &&
                                            Utils.dateformat.ConvertToDate(t.STATO_TASK.DATA_SCADENZA).ToShortDateString().Equals(d.Date.ToShortDateString())
                                            select t).ToList();
                if (task != null && task.Count > 0)
                {
                    //Ordino per gravità dei task
                    task = task.OrderBy(t => t.STATO_TASK.DATA_LAVORAZIONE).ToList();
                    CultureInfo ci = new CultureInfo("it-IT");
                    string today = DateTime.Now.ToString(ci);
                    string imgUrl = "../Images/Icons/";
                    Image image = new Image();

                    //Visualizzo l'immagine nel calendario in base alla criticità
                    if ((from t in task
                         where (t.STATO_TASK.STATO == StatoAvanzamento.Riaperto)
                             && !Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(t.STATO_TASK.DATA_SCADENZA).ToShortDateString(), Utils.dateformat.ConvertToDate(today).ToShortDateString())
                         select t).FirstOrDefault() != null)
                    {
                        imgUrl += "task_scaduto_riaperto.png";
                    }
                    else if ((from t in task where (t.STATO_TASK.STATO == StatoAvanzamento.Aperto)
                             && !Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(t.STATO_TASK.DATA_SCADENZA).ToShortDateString(), Utils.dateformat.ConvertToDate(today).ToShortDateString())
                         select t).FirstOrDefault() != null)
                    {
                        imgUrl += "task_scaduto_aperto.png";
                    }
                    else if ((from t in task
                              where (t.STATO_TASK.STATO == StatoAvanzamento.Riaperto)
                                  && Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(t.STATO_TASK.DATA_SCADENZA).ToShortDateString(), Utils.dateformat.ConvertToDate(today).ToShortDateString())
                              select t).FirstOrDefault() != null)
                    {
                        imgUrl += "task_in_corso_riaperto.png";
                    }
                    else if ((from t in task
                              where (t.STATO_TASK.STATO == StatoAvanzamento.Aperto)
                                  && Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(t.STATO_TASK.DATA_SCADENZA).ToShortDateString(), Utils.dateformat.ConvertToDate(today).ToShortDateString())
                              select t).FirstOrDefault() != null)
                    {
                        imgUrl += "task_in_corso.png";
                    }
                    else if ((from t in task where t.STATO_TASK.STATO == StatoAvanzamento.Lavorato && 
                                  !Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(t.STATO_TASK.DATA_SCADENZA).ToShortDateString(), Utils.dateformat.ConvertToDate(t.STATO_TASK.DATA_LAVORAZIONE).ToShortDateString())
                              select t).FirstOrDefault() != null)
                    {
                        imgUrl += "task_chiuso_oltre_scadenza.png";
                    }
                    else
                    {
                        imgUrl += "task_completato_in_tempo.png";
                    }
                    image.ImageUrl = imgUrl;
                    c.Controls.Add(image);

                    LinkButton lnk;
                    for (int i = 0; i < task.Count && i <= 1; i++)
                    {
                        lnk = new LinkButton();
                        if (i == 1)
                        {
                            lnk.Text = "<p> ... </p>";
                            //Apro il dettaglio di tutti i task del giorno selezionato
                            lnk.Attributes["onClick"] = "$('#HiddenDataScadenza').val('" + d.Date.ToShortDateString() + "'); __doPostBack('UpPnlButtons');";
                        }
                        else
                        {

                            lnk.Text = "<p>" + task[i].DESCRIPTION_OBJECT + "</p>";
                            //Apro il dettaglio del singolo task su cui ho cliccato
                            lnk.Attributes["onClick"] = "$('#hiddenValueTask').val('" + task[i].ID_TASK + "'); __doPostBack('UpPnlButtons');";
                        }
                        c.Controls.Add(lnk);
                    }
                }
            }
        }

        #endregion

        #region Grid Manager

        private void GridViewResult_Bind()
        {
            try
            {
                List<DocsPaWR.Task> listaTask = this.GetListaTask();
                List<DocsPaWR.Task> listRes = new List<DocsPaWR.Task>();
                listRes.AddRange(listaTask);

                int dtRowsCount = listaTask.Count;
                int index = 1;
                if (dtRowsCount > 0)
                {
                    for (int i = 0; i < dtRowsCount; i++)
                    {
                        DocsPaWR.Task d = new DocsPaWR.Task();
                        d = listaTask[i];
                        listRes.Insert(index, d);
                        index += 2;
                    }
                }
                PagedDataSource dsP = new PagedDataSource();
                dsP.DataSource = listRes;
                dsP.AllowPaging = false;
                /*
                if (listRes.Count > 0 && ((float)listRes.Count / this.PageSize) <= (SelectedPage - 1))
                    SelectedPage = (listRes.Count % PageSize) > 0 ?
                        (listRes.Count / PageSize) + 1 : (listRes.Count / PageSize);
                else if (listRes == null || listRes.Count == 0)
                    SelectedPage = 1;
                //SelectedPage -= 1;
                dsP.CurrentPageIndex = SelectedPage - 1;
                dsP.PageSize = PageSize;
                */
                this.gridViewResult.DataSource = dsP;
                this.gridViewResult.DataBind();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }

        }

        protected void gridViewResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string typeTask = (this.RblTypeTask.SelectedValue.Equals("0") ? "R" : "A");
                    DocsPaWR.Task task = e.Row.DataItem as DocsPaWR.Task;
                    if (task.STATO_TASK.STATO == StatoAvanzamento.Aperto || task.STATO_TASK.STATO == StatoAvanzamento.Riaperto)
                    {
                        (e.Row.FindControl("ImgRemoveTask") as CustomImageButton).Visible = false;
                        (e.Row.FindControl("ImgRiapriLavorazione") as CustomImageButton).Visible = false;
                        (e.Row.FindControl("ImgCreaContributo") as CustomImageButton).Visible = (typeTask.Equals(RICEVUTI) && string.IsNullOrEmpty(task.ID_PROFILE_REVIEW));
                        (e.Row.FindControl("ImgCloseTask") as CustomImageButton).Visible = typeTask.Equals(RICEVUTI);
                        (e.Row.FindControl("ImgBlockTask") as CustomImageButton).Visible = typeTask.Equals(ASSEGNATI);
                    }
                    else
                    {
                        (e.Row.FindControl("ImgCreaContributo") as CustomImageButton).Visible = false;
                        (e.Row.FindControl("ImgCloseTask") as CustomImageButton).Visible = false;
                        (e.Row.FindControl("ImgBlockTask") as CustomImageButton).Visible = false;
                        (e.Row.FindControl("ImgRiapriLavorazione") as CustomImageButton).Visible = typeTask.Equals(ASSEGNATI);
                        (e.Row.FindControl("ImgRemoveTask") as CustomImageButton).Visible = typeTask.Equals(ASSEGNATI);
                    }
                    (e.Row.FindControl("ImgViewDocument") as CustomImageButton).Visible = !string.IsNullOrEmpty(task.ID_PROFILE);
                    (e.Row.FindControl("ImageViewProject") as CustomImageButton).Visible = !string.IsNullOrEmpty(task.ID_PROJECT);
                    (e.Row.FindControl("ImgViewContributo") as CustomImageButton).Visible = !string.IsNullOrEmpty(task.ID_PROFILE_REVIEW);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void gridViewResult_PreRender(object sender, EventArgs e)
        {
            try
            {
                bool visualizzaRicevuti = this.RblTypeTask.SelectedIndex == 0;
                int cellsCount = 0;
                foreach (DataControlField td in gridViewResult.Columns)
                {
                    if (td.HeaderText.Equals(Utils.Languages.GetLabelFromCode("TaskAssegnatario", UIManager.UserManager.GetUserLanguage())))
                    {
                        td.Visible = visualizzaRicevuti;
                    }
                    if (td.HeaderText.Equals(Utils.Languages.GetLabelFromCode("TaskNoteApertura", UIManager.UserManager.GetUserLanguage())))
                    {
                        td.Visible = visualizzaRicevuti;
                    }
                    if (td.HeaderText.Equals(Utils.Languages.GetLabelFromCode("TaskNoteLavorazione", UIManager.UserManager.GetUserLanguage())))
                    {
                        td.Visible = !visualizzaRicevuti;
                    }
                    if (td.HeaderText.Equals(Utils.Languages.GetLabelFromCode("TaskDestinatario", UIManager.UserManager.GetUserLanguage())))
                    {
                        td.Visible = !visualizzaRicevuti;
                    }

                    if (td.HeaderText.Equals(Utils.Languages.GetLabelFromCode("TaskDataChiusura", UIManager.UserManager.GetUserLanguage())))
                    {
                        td.Visible = !visualizzaRicevuti;
                    }
                    if (td.Visible) cellsCount++;
                }

                bool alternateRow = false;
                int indexCellIcons = -1;

                if (cellsCount > 0)
                {
                    for (int i = 1; i < gridViewResult.Rows.Count; i = i + 2)
                    {

                        gridViewResult.Rows[i].CssClass = "NormalRow";
                        if (alternateRow) gridViewResult.Rows[i].CssClass = "AltRow";
                        alternateRow = !alternateRow;

                        for (int j = 0; j < gridViewResult.Rows[i].Cells.Count; j++)
                        {
                            bool found = false;
                            foreach (Control c in gridViewResult.Rows[i].Cells[j].Controls)
                            {
                                if (c.ID == "ImgViewDocument")
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                                gridViewResult.Rows[i].Cells[j].Visible = false;
                            else
                            {
                                gridViewResult.Rows[i].Cells[j].ColumnSpan = cellsCount - 1;
                                gridViewResult.Rows[i].Cells[j].Attributes["style"] = "text-align: right;";
                                indexCellIcons = j;
                            }
                        }
                    }

                    alternateRow = false;
                    for (int i = 0; i < gridViewResult.Rows.Count; i = i + 2)
                    {
                        gridViewResult.Rows[i].CssClass = "NormalRow";
                        if (alternateRow) gridViewResult.Rows[i].CssClass = "AltRow";
                        alternateRow = !alternateRow;

                        for (int j = 0; j < gridViewResult.Rows[i].Cells.Count; j++)
                        {
                            bool found = false;
                            foreach (Control c in gridViewResult.Rows[i].Cells[j].Controls)
                            {
                                if (c.ID == "ImgViewDocument")
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (found)
                                gridViewResult.Rows[i].Cells[j].Visible = false;
                            else
                                gridViewResult.Rows[i].Cells[j].Attributes["style"] = "text-align: center;";
                        }
                    }
                    if (indexCellIcons > -1)
                        gridViewResult.HeaderRow.Cells[indexCellIcons].Visible = false;
                    if (gridViewResult.HeaderRow != null)
                    {
                        for (int j = 0; j < gridViewResult.HeaderRow.Cells.Count; j++)
                            gridViewResult.HeaderRow.Cells[j].Attributes["style"] = "text-align: center;";
                    }
                }

                if (!string.IsNullOrEmpty(this.SelectedRow))
                {
                    for (int i = 0; i < gridViewResult.Rows.Count; i++)
                    {
                        if (this.gridViewResult.Rows[i].RowIndex == Int32.Parse(this.SelectedRow))
                        {
                            this.gridViewResult.Rows[i].Attributes.Remove("class");
                            this.gridViewResult.Rows[i].CssClass = "selectedrow";
                            this.gridViewResult.Rows[i - 1].Attributes.Remove("class");
                            this.gridViewResult.Rows[i - 1].CssClass = "selectedrow";
                        }
                    }

                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected string GetDataScadenza(DocsPaWR.Task task)
        {
            if (!string.IsNullOrEmpty(task.STATO_TASK.DATA_SCADENZA))
                return Utils.dateformat.ConvertToDate(task.STATO_TASK.DATA_SCADENZA).ToString("dd/MM/yyyy");
            else
                return string.Empty;
        }

        protected void gridViewResult_ItemCreated(Object sender, GridViewRowEventArgs e)
        {
            try
            {

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GridView_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            SelectedRow = (e.CommandSource as LinkButton) != null ? (((e.CommandSource as Control).Parent.Parent as GridViewRow).RowIndex + 1).ToString() :
                 ((e.CommandSource as Control).Parent.Parent as GridViewRow).RowIndex.ToString();
            string idTask = string.Empty;
            string idTrasmSingola = string.Empty;
            string idProfile = string.Empty;
            string idProject = string.Empty;
            string idObject = string.Empty;
            GridViewRow row = ((e.CommandSource as Control).Parent.Parent as GridViewRow);
            switch (e.CommandName)
            {
                case "ViewContributo":
                    string idReview = (row.FindControl("idProfileReview") as Label).Text;
                    #region navigation
                    List<Navigation.NavigationObject> navigationList2 = Navigation.NavigationUtils.GetNavigationList();
                    Navigation.NavigationObject actualPage2 = new Navigation.NavigationObject();
                    actualPage2.IdObject = idReview;
                    actualPage2.OriginalObjectId = idReview;
                    actualPage2.NumPage = this.gridViewResult.PageIndex.ToString();
                    actualPage2.PageSize = this.gridViewResult.PageSize.ToString();
                    actualPage2.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME_TASK.ToString(), string.Empty);
                    actualPage2.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.HOME_TASK.ToString(), true, this.Page);
                    actualPage2.CodePage = Navigation.NavigationUtils.NamePage.HOME_TASK.ToString();

                    actualPage2.Page = "TASK.ASPX";
                    navigationList2.Add(actualPage2);
                    Navigation.NavigationUtils.SetNavigationList(navigationList2);
                    #endregion
                    SchedaDocumento schedaDocumento = UIManager.DocumentManager.getDocumentDetails(this.Page, idReview, idReview);
                    DocumentManager.setSelectedRecord(schedaDocumento);
                    Response.Redirect("../Document/Document.aspx");
                    return;
                case "ViewObjectTask":
                    idTask = (row.FindControl("idTask") as Label).Text;

                    idObject = (row.FindControl("idProfile") as Label).Text;
                    if (!string.IsNullOrEmpty(idObject))
                    {
                        #region navigation
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                        actualPage.IdObject = idObject;
                        actualPage.OriginalObjectId = idObject;
                        actualPage.NumPage = this.gridViewResult.PageIndex.ToString();
                        actualPage.PageSize = this.gridViewResult.PageSize.ToString();

                        actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME_TASK.ToString(), string.Empty);
                        actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.HOME_TASK.ToString(), true, this.Page);
                        actualPage.CodePage = Navigation.NavigationUtils.NamePage.HOME_TASK.ToString();

                        actualPage.Page = "TASK.ASPX";
                        navigationList.Add(actualPage);
                        Navigation.NavigationUtils.SetNavigationList(navigationList);

                        SelectedRow = (e.CommandSource as LinkButton) != null ? (((e.CommandSource as Control).Parent.Parent as GridViewRow).RowIndex + 1).ToString() :
                             ((e.CommandSource as Control).Parent.Parent as GridViewRow).RowIndex.ToString();
                        #endregion
                        SchedaDocumento schedaDoc = UIManager.DocumentManager.getDocumentDetails(this.Page, idObject, idObject);
                        DocumentManager.setSelectedRecord(schedaDoc);
                        Response.Redirect("../Document/Document.aspx");
                        return;
                    }
                    else
                    {
                        idObject = (row.FindControl("idProject") as Label).Text;
                        #region navigation
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                        actualPage.IdObject = idObject;
                        actualPage.OriginalObjectId = idObject;
                        actualPage.NumPage = this.gridViewResult.PageIndex.ToString();
                        actualPage.PageSize = this.gridViewResult.PageSize.ToString();

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
                        Response.Redirect("~/Project/project.aspx");
                        return;
                    }
                case "RemoveTask":
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmRemoveTask', 'HiddenRemoveTask', '');", true);
                    break;
                case "AnnullaTask":
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmCancelTask', 'HiddenCancelTask', '');", true);
                    break;
                case "CloseTask":
                    //Completa la lavorazione
                    string idContributo = (row.FindControl("idProfileReview") as Label).Text;
                    bool requiredReview = string.IsNullOrEmpty(idContributo) && (row.FindControl("contributoObbligatorio") as Label).Text.Equals("1");
                    if (requiredReview)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('WarningTaskRequiredReview', 'warning', '','',null,null,'')", true);
                        return;
                    }
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CompleteTask", "ajaxModalPopupCompleteTask();", true);
                    break;
                case "RiapriLavorazione":
                    idTask = (row.FindControl("idTask") as Label).Text;
                    idTrasmSingola = (row.FindControl("idTrasmSingola") as Label).Text;
                    idProfile = (row.FindControl("idProfile") as Label).Text;
                    idProject = (row.FindControl("idProject") as Label).Text;
                    string idProfileReview = (row.FindControl("idProfileReview") as Label).Text;
                    string dataScadenza = (row.FindControl("dataScadenza") as Label).Text;
                    TaskSelected = new DocsPaWR.Task() 
                    { 
                        ID_TASK = idTask, 
                        ID_TRASM_SINGOLA = idTrasmSingola,
                        ID_PROFILE = idProfile, 
                        ID_PROJECT = idProject,
                        ID_PROFILE_REVIEW = idProfileReview,
                    };
                    this.TaskSelected.STATO_TASK = new StatoTask()
                    {
                        DATA_SCADENZA = dataScadenza
                    };
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ReopenTask", "ajaxModalPopupReopenTask();", true);
                    break;
                case "CreaContributo":
                    idTask = (row.FindControl("idTask") as Label).Text;
                    idObject = !string.IsNullOrEmpty((row.FindControl("idProfile") as Label).Text) ? (row.FindControl("idProfile") as Label).Text : (row.FindControl("idProject") as Label).Text;
                    string error = this.CreaContributo(row);
                    if (string.IsNullOrEmpty(error))
                    {
                        #region navigation
                        List<Navigation.NavigationObject> navigationList1 = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject actualPage1 = new Navigation.NavigationObject();
                        actualPage1.IdObject = idObject;
                        actualPage1.OriginalObjectId = idObject;
                        actualPage1.NumPage = this.gridViewResult.PageIndex.ToString();
                        actualPage1.PageSize = this.gridViewResult.PageSize.ToString();

                        actualPage1.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME_TASK.ToString(), string.Empty);
                        actualPage1.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.HOME_TASK.ToString(), true, this.Page);
                        actualPage1.CodePage = Navigation.NavigationUtils.NamePage.HOME_TASK.ToString();

                        actualPage1.Page = "TASK.ASPX";
                        navigationList1.Add(actualPage1);
                        Navigation.NavigationUtils.SetNavigationList(navigationList1);
                        #endregion
                        TaskSelected = new DocsPaWR.Task()
                        {
                            ID_TASK = idTask,
                            ID_PROFILE = (row.FindControl("idProfile") as Label).Text,
                            ID_PROJECT = (row.FindControl("idProject") as Label).Text,
                            ID_RAGIONE_TRASM = (row.FindControl("idRagione") as Label).Text,
                            RUOLO_MITTENTE = new Ruolo() { idGruppo = (row.FindControl("idGruppoMittente") as Label).Text },
                            UTENTE_MITTENTE = new Utente() { idPeople = (row.FindControl("idPeopleMittente") as Label).Text }
                        };
                        Response.Redirect("~/Document/Document.aspx");
                    }
                    else if (error == "AnswerChooseRecipient")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "AnswerChooseRecipient", "ajaxModalPopupAnswerChooseRecipient();", true);
                    }
                    else
                    {
                        TaskSelected = null;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('" + error.Replace("'", @"\'") + "', 'warning', '','',null,null,'')", true);
                    }
                    break;
            }
        }

        private bool RiapriLavorazione()
        {
            bool retValue = false;
            try
            {
                if (TaskManager.RiapriLavorazione(this.TaskSelected))
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

        private string CreaContributo(GridViewRow row)
        {
            try
            {
                string msg = string.Empty;
                string idProject = (row.FindControl("idProject") as Label).Text;
                string idProfile = (row.FindControl("idProfile") as Label).Text;
                Templates templateToMerge = null;
                SchedaDocumento document = UIManager.DocumentManager.NewSchedaDocumento();
                document.oggetto = new Oggetto();
                document.tipoProto = "G";
                string idTipoAtto = (row.FindControl("idTipoAtto") as Label).Text;
                if (!string.IsNullOrEmpty(idTipoAtto))
                {
                    //Devo aggiungere controlli di visibilità della tipologia
                    document.template = UIManager.DocumentManager.getTemplateById(idTipoAtto, UserManager.GetInfoUser());
                    document.oggetto.descrizione = document.template.DESCRIZIONE + " - ";
                    templateToMerge = !string.IsNullOrEmpty(idProfile) ? ProfilerDocManager.getTemplateDettagli(idProfile) : UIManager.ProfilerProjectManager.getTemplateFascDettagli(idProject);
                    if (templateToMerge != null)
                    {
                        document.template = MappingTemplates(templateToMerge, document.template);
                    }
                }

                if (!string.IsNullOrEmpty(idProject))
                {
                    Fascicolo fascicolo = UIManager.ProjectManager.getFascicoloById(idProject);
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
                    SchedaDocumento schedaDocDiPartenza = DocumentManager.getDocumentDetails(this, idProfile, idProfile);
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
                                    this.SchedaDocContributo = document;
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return ex.Message;
            }

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

        #region Event Button

        protected void ImageViewGrid_Click(object sender, EventArgs e)
        {
            try
            {
                this.gridViewResult.Visible = true;
                this.calendar.Visible = false;
                this.GridViewResult_Bind();
                this.ImageViewGrid.Enabled = false;
                this.ImageViewCalendar.Enabled = true;
                this.UpnlViewMode.Update();
                this.UpnlGrid.Update();
                this.UpnlCalendar.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ImageViewCalendar_Click(object sender, EventArgs e)
        {
            try
            {
                this.gridViewResult.Visible = false;
                this.calendar.Visible = true;
                this.Calendar_Bind();
                this.ImageViewGrid.Enabled = true;
                this.ImageViewCalendar.Enabled = false;
                this.UpnlViewMode.Update();
                this.UpnlGrid.Update();
                this.UpnlCalendar.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void RblTypeTask_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.SelectedRow = string.Empty;
                this.SelectedPage = 0;

                this.pnlCbxTaskChiusi.Visible = this.RblTypeTask.SelectedValue == "0";
                this.UpCbxTaskChiusi.Update();

                if (this.gridViewResult.Visible)
                {
                    this.GridViewResult_Bind();
                    this.UpnlGrid.Update();
                }
                else
                {
                    this.Calendar_Bind();
                    this.UpnlCalendar.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void CbxTaskChiusi_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.gridViewResult.Visible)
                {
                    this.GridViewResult_Bind();
                    this.UpnlGrid.Update();
                }
                else
                {
                    this.Calendar_Bind();
                    this.UpnlCalendar.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }

        }
        #endregion

        #region Utils

        private void LoadRolesUser()
        {
            try
            {
                this.ddlRolesUser.Items.Clear();
                ListItem item;
                Ruolo[] role = UIManager.UserManager.GetUserInSession().ruoli;
                foreach (Ruolo r in role)
                {
                    item = new ListItem();
                    item.Value = r.systemId;
                    item.Text = r.descrizione;
                    this.ddlRolesUser.Items.Add(item);
                }

                this.ddlRolesUser.SelectedValue = RoleManager.GetRoleInSession().systemId;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private List<DocsPaWR.Task> GetListaTask()
        {
            List<DocsPaWR.Task> listaTask = new List<DocsPaWR.Task>();
            if (this.RblTypeTask.SelectedIndex == 0)
                listaTask = UIManager.TaskManager.GetListaTaskRicevuti(this.cbxTaskChiusi.Checked);
            else
                listaTask = UIManager.TaskManager.GetListaTaskAssegnati();
            return listaTask;
        }

        protected string GetAssegnatario(DocsPaWR.Task task)
        {
            if (task.RUOLO_MITTENTE != null)
                return task.UTENTE_MITTENTE.descrizione + " (" + task.RUOLO_MITTENTE.descrizione + ")";
            else
                return string.Empty;
        }

        protected string GetDestinatario(DocsPaWR.Task task)
        {
            if (task.RUOLO_DESTINATARIO != null)
                return task.UTENTE_DESTINATARIO.descrizione + " (" + task.RUOLO_DESTINATARIO.descrizione + ")";
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

        protected string GetNoteLavorazione(DocsPaWR.Task task)
        {
            string note = task.STATO_TASK.NOTE_LAVORAZIONE;
            string language = UIManager.UserManager.GetUserLanguage();
            int indexFrom = note.IndexOf(TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO) + TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO.Length;
            int indexTo = note.IndexOf(TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO_C) - (note.IndexOf(TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO) + TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO.Length);
            string idContributo = string.Empty;
            string[] split = null;
            string note1 = string.Empty;
            if (indexTo > 0)
            {
                idContributo = note.Substring(indexFrom, indexTo);
                note = note.Replace(TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO, Utils.Languages.GetLabelFromCode("TaskIdContributo", language));
                note = note.Replace(TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO_C, "");
                note = note.Replace(TaskManager.TagIdContributo.LABEL_ATTIVITA_CONCLUSA, Utils.Languages.GetLabelFromCode("TaskNote", language));
                split = note.Split(new string[] { idContributo }, StringSplitOptions.None);
            }
            string note2 = string.Empty;
            if (split != null && split.Length > 1)
            {
                note1 = split[0] + idContributo;
                note2 = Utils.Languages.GetLabelFromCode("TaskNoteTrasm", UIManager.UserManager.GetUserLanguage()) + split[1];
            }

            return (note1 + note2);
        }
        #endregion
    }
}