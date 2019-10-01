using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDatalLibrary;
using NttDataWA.Utils;
using System.Data;
using System.Configuration;

namespace NttDataWA
{
    public partial class Index : System.Web.UI.Page
    {
        #region Property

        private string ReturnValue
        {
            get
            {
                if ((HttpContext.Current.Session["ReturnValuePopup"]) != null)
                    return HttpContext.Current.Session["ReturnValuePopup"].ToString();
                else
                    return string.Empty;
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

        private int SelectedPage
        {
            get
            {
                if (HttpContext.Current.Session["SelectedPageIndex"] != null)
                    return (int)HttpContext.Current.Session["SelectedPageIndex"];
                else
                    return 1;
            }
            set
            {
                HttpContext.Current.Session["SelectedPageIndex"] = value;
            }
        }

        private Notification Notification
        {
            get
            {
                if (HttpContext.Current.Session["Notification"] != null)
                    return (Notification)HttpContext.Current.Session["Notification"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["Notification"] = value;
            }
        }

        public bool OtherOperation
        {
            get
            {
                if (HttpContext.Current.Session["OtherOperation"] != null)
                    return (Boolean)HttpContext.Current.Session["OtherOperation"];
                else
                    return false;
            }
            set
            {
                HttpContext.Current.Session["OtherOperation"] = value;
            }
        }

        private List<string> ListNotifySelected
        {
            get
            {
                if (HttpContext.Current.Session["ListNotifySelected"] != null)
                    return (List<string>)HttpContext.Current.Session["ListNotifySelected"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ListNotifySelected"] = value;
            }
        }

        private bool ExpandAll
        {
            get
            {
                if (HttpContext.Current.Session["ExpandAll"] != null)
                    return (Boolean)HttpContext.Current.Session["ExpandAll"];
                else
                    return false;
            }
            set
            {
                HttpContext.Current.Session["ExpandAll"] = value;
            }
        }

        private Notification NotificationToRemove
        {
            get
            {
                if (HttpContext.Current.Session["NotificationToRemove"] != null)
                    return (Notification)HttpContext.Current.Session["NotificationToRemove"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["NotificationToRemove"] = value;
            }
        }

        private string OrderBy
        {
            get
            {
                if ((HttpContext.Current.Session["OrderBy"]) != null)
                    return HttpContext.Current.Session["OrderBy"].ToString();
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["OrderBy"] = value;
            }
        }

        private bool IsFirstTime
        {
            get
            {
                if ((HttpContext.Current.Session["IsFirstTime"]) != null)
                    return false;
                else
                    return true;
            }
            set
            {
                HttpContext.Current.Session["IsFirstTime"] = value;
            }
        }

        private bool DisservizioSession
        {
            get
            {
                if ((HttpContext.Current.Session["Disservizio"]) != null)
                    return true;
                else
                    return false;
            }
            set
            {
                HttpContext.Current.Session["Disservizio"] = value;
            }
        }

        private bool UpdatePageAfterRemoveNotification
        {
            get
            {
                if (HttpContext.Current.Session["UpdatePageAfterRemoveNotification"] != null)
                    return (bool)HttpContext.Current.Session["UpdatePageAfterRemoveNotification"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["UpdatePageAfterRemoveNotification"] = value;
            }
        }

        #endregion

        #region Remove property

        private void ClearSession()
        {

            HttpContext.Current.Session.Remove("ListAllNotify");
            HttpContext.Current.Session.Remove("ListNotifyFiltered");
            HttpContext.Current.Session.Remove("ListNotifySelected");
            HttpContext.Current.Session.Remove("SelectedPageIndex");
        }

        private void RemoveLastDocumentsView()
        {
            HttpContext.Current.Session.Remove("LastDocumentsView");
        }

        private void RemoveExpandAllInSession()
        {
            HttpContext.Current.Session.Remove("ExpandAll");
        }

        private void RemoveIsRoleChanged()
        {
            HttpContext.Current.Session.Remove("IsRoleChanged");
        }

        private void RemoveIsZoom()
        {
            HttpContext.Current.Session.Remove("isZoom");
        }

        private void RemoveNotificationInSession()
        {
            HttpContext.Current.Session.Remove("Notification");
        }

        private void RemoveTransmissionInSession()
        {
            HttpContext.Current.Session.Remove("Transmission");
        }

        private void RemoveNotificationToRemove()
        {
            HttpContext.Current.Session.Remove("NotificationToRemove");
        }

        #endregion

        #region Const

        private const string ORDER_BY_DATE_DESCENDING = "1";
        private const string ORDER_BY_DATE_ASCENDING = "2";
        private const string ORDER_BY_TYPE_EVENT = "3";
        private const string ORDER_BY_PRODUTTORE = "4";
        private const string ORDER_BY_ID_OBJECT = "5";
        private const int PAGE_SIZE = 10;
        private const string UPDATE_PANEL_GRID_INDEXES = "upPnlGridIndexes";
        private const string UPDATE_PANEL_REP_LIST_NOTIFY = "UpdatePanelRepListNotify";
        private const string VIEW_NOTIFY_DETAILS = "viewNotifyDetails";
        private const string LINE = "<line>";
        private const string LINE_C = "</line>";
        private const string UP_PANEL_BUTTONS = "UpPnlButtons";
        private const string CLOSE_ZOOM = "closeZoom";
        private const string CLOSE_ADD_FILTER_NOTIFICATION_CENTER = "closeAddFilterNotificationCenter";
        private const string CLOSE_POPUP_VIEW_DETAIL_NOTIFY = "closePopupViewDetailNotify";
        private const string SELECT_NOTIFICATION = "selectNotification";
        private const string CLOSE_POPUP_OBJECT = "closePopupObject";
        private const string CLOSE_POPUP_ADDRESS_BOOK = "closePopupAddressBook";
        private const string CLOSE_POPUP_NOTIFICATION_NOTICE_DAYS = "closePopupNotificationNoticeDays";
        private const string LOAD_NOTIFY = "loadNotify";
        private const string CLOSE_POPUP_SEARCH_PROJECT = "closePopupSearchProject";
        private const string CLOSE_POPUP_OPEN_TITOLARIO = "closePopupOpenTitolario";

        #endregion

        #region global variable

        private static string operational;
        private static string information;
        private static string tooltipGoToDocument;
        private static string tooltipGoToFolder;
        private static string tooltipRemoveNotify;

        #endregion

        #region Standard method
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoginManager.IniInitializesLanguages();
                GridManager.SelectedGrid = null;
                InitializeLanguage();

                if (!this.DisservizioSession)
                {
                    Disservizio diss = UIManager.AdministrationManager.GetDisservizio();
                    if (diss != null && !string.IsNullOrEmpty(diss.stato) && !string.IsNullOrEmpty(diss.testo_cortesia) && !diss.stato.Equals("disattivo"))
                    {
                        
                        this.js_code.Text += "ajaxModalPopupDisservizio();";
                      //  ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "", "ajaxModalPopupDisservizio();", true);
                        
                        DocumentManager.setSelectedRecord(null);
                        InitializePage(false);
                        this.DisservizioSession = true;
                        return;
                    }
                }

                this.DisservizioSession = true;

                InitializePage(true);
                // pulisce la sessione dopo un redirect derivante da perdita dei diritti
                DocumentManager.setSelectedRecord(null);


            }
            else
            {
                this.ReadRetValueFromPopup();
            }

            this.RefreshScript();
        }

        private void InitializePage(bool disservizio)
        {
            //Se non è la prima volta che si entra nell'home page reimposto i filtri precedentemente impostati
            if (IsFirstTime && disservizio)
            {
                IsFirstTime = false;
                //Rimuovo la lista degli ultimi documenti visualizzati dal ruolo precedente
                RemoveLastDocumentsView();
                this.VerificaDeleghe();
                this.ApriConnettoreSocket();
            }
            if (NotificationManager.ListAllNotify != null && NotificationManager.ListAllNotify.Count > 0)
            {
                LoadRolesUser();
                List<Notification> list = NotificationManager.ReadNotifications(UserManager.GetUserInSession().idPeople, RoleManager.GetRoleInSession().idGruppo);
                //Se il tipo evento non è presente in FILTERS_OPERATIONAL e non è presente nella lista delle notifiche precedenti,
                //li aggiungo nella nuova lista di eventi lista in modo da vederli selezionati nel filtro
                FiltersNotifications.FILTERS_OPERATIONAL = FiltersNotifications.GetFiltersOperational();
                List<string> listTypeEventOperational = (from n in list
                                                         where n.TYPE_NOTIFICATION == NotificationManager.NotificationType.OPERATIONAL && !FiltersNotifications.FILTERS_OPERATIONAL.Contains(n.TYPE_EVENT) &&
                                                         (((from not in NotificationManager.ListAllNotify where n.TYPE_EVENT.Equals(not.TYPE_EVENT) select not.TYPE_EVENT).ToList()).Count() < 1)
                                                         select n.TYPE_EVENT).ToList();
                FiltersNotifications.FILTERS_OPERATIONAL.AddRange(listTypeEventOperational.Distinct());
                //Se il tipo evento non è presente in FILTERS_INFORMATION  e non è presente nella lista delle notifiche precedenti,
                //li aggiungo nella nuova lista di eventi in modo da vederli selezionati nel filtro
                FiltersNotifications.FILTERS_INFORMATION = FiltersNotifications.GetFiltersInformation();
                List<string> listTypeEventInformation = (from n in list
                                                         where n.TYPE_NOTIFICATION == NotificationManager.NotificationType.INFORMATION && !FiltersNotifications.FILTERS_INFORMATION.Contains(n.TYPE_EVENT) &&
                                                         (((from not in NotificationManager.ListAllNotify where n.TYPE_EVENT.Equals(not.TYPE_EVENT) select not.TYPE_EVENT).ToList()).Count() < 1)
                                                         select n.TYPE_EVENT).ToList();
                FiltersNotifications.FILTERS_INFORMATION.AddRange(listTypeEventInformation.Distinct());
                NotificationManager.ListAllNotify = list;
                NotificationManager.ListNotifyFiltered = list;
                if (list != null && list.Count > 0)
                {
                    List<Notification> l = (from d in NotificationManager.ListNotifyFiltered where d.TYPE_NOTIFICATION == NotificationManager.NotificationType.OPERATIONAL select d).ToList<Notification>();
                    if (l != null && l.Count > 0)
                    {
                        var operationalFilters = (from n in l
                                                  where n.DOMAINOBJECT.ToUpper() == "DOCUMENTO" && NotificationManager.GetTypeEvent(n.TYPE_EVENT).Equals(NotificationManager.EventType.TRASM)
                                                  select n).ToList();
                        if (operationalFilters.Count == 0)
                        {
                            this.IndexImgSmista.Enabled = false;
                        }
                    }
                    else
                    {
                        this.IndexImgSmista.Enabled = false;
                    }
                }
                else
                {
                    this.IndexImgSmista.Enabled = false;
                }

                LoadNumberNotify();
                LoadNumberNotifyInTheRole();
                ChkNotifyOperational_Bind();
                ChkNotifyInformation_Bind();

                #region Setting filters

                if (FiltersNotifications.GetFiltersReadNoRead().Count > 0)
                {
                    foreach (KeyValuePair<string, bool> keyval in FiltersNotifications.GetFiltersReadNoRead())
                    {
                        if (keyval.Key.Equals(FiltersNotifications.READ))
                        {
                            this.IndexChkRead.Checked = keyval.Value;
                        }
                        else if (keyval.Key.Equals(FiltersNotifications.NOREAD))
                        {
                            this.IndexChkNotRead.Checked = keyval.Value;
                        }
                    }
                }
                if (FiltersNotifications.GetFiltersDomainObject().Count > 0)
                {
                    FiltersNotifications.FILTERS_DOMAIN_OBJECT = FiltersNotifications.GetFiltersDomainObject();
                    foreach (KeyValuePair<string, bool> keyval in FiltersNotifications.GetFiltersDomainObject())
                    {
                        if (keyval.Key.Equals(FiltersNotifications.DOCUMENT))
                        {
                            this.IndexChkDoc.Checked = keyval.Value;
                        }
                        else if (keyval.Key.Equals(FiltersNotifications.FOLDER))
                        {
                            this.IndexChkProj.Checked = keyval.Value;
                        }
                        else if (keyval.Key.Equals(FiltersNotifications.OTHER))
                        {
                            this.IndexChkOther.Checked = keyval.Value;
                        }
                    }
                }

                if (FiltersNotifications.FILTERS_OPERATIONAL.Count > 0)
                {
                    this.IndexChkOperational.Checked = true;
                    foreach (ListItem item in IndexCblOperational.Items)
                    {
                        item.Selected = FiltersNotifications.FILTERS_OPERATIONAL.Contains(item.Value);
                    }
                }
                else
                {
                    this.IndexChkOperational.Checked = false;
                }

                if (FiltersNotifications.FILTERS_INFORMATION.Count > 0)
                {
                    this.IndexChkInformation.Checked = true;
                    foreach (ListItem item in IndexCblInformation.Items)
                    {
                        item.Selected = FiltersNotifications.FILTERS_INFORMATION.Contains(item.Value);
                    }
                }
                else
                {
                    this.IndexChkInformation.Checked = false;
                }
                if (NotificationManager.Filters != null)
                {
                    this.IndexImgRemoveFilter.Enabled = true;
                }

                this.ddlOrderBy.SelectedValue = OrderBy;

                #endregion

                NotificationManager.ApplyFilters();
                NotificationManager.ApplyFiltersNotifications();
                OrderListNotifyFiltered();
            }
            else
            {
                if (UserManager.IsAuthorizedFunctions("HOME_EXPAND_ALL"))
                {
                    this.ExpandAll = true;
                }
                LoadRolesUser();
                LoadNotify();
                LoadNumberNotify();
                LoadNumberNotifyInTheRole();
                ChkNotifyOperational_Bind();
                ChkNotifyInformation_Bind();
                UpdateFilterOperational();
                UpdateFilterInformation();
                UpdateFilterObject();
                UpdateFilterReadNotification();
            }

            RepListNotifyAll_Bind();
            if (NotificationManager.NotificationOverNoticeDays() && disservizio)
            {
                if (!IsPostBack)
                {
                    this.js_code.Text += "ajaxModalPopupNotificationNoticeDays();";
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "modalPopup", "ajaxModalPopupNotificationNoticeDays();", true);
                }
            }
            //questa property rimane in sessione quando dal tab allegati faccio back e torno nella ricerca; va rimossa
            HttpContext.Current.Session.Remove("selectedAttachmentId");

            if (UIManager.UserManager.IsAuthorizedFunctions("DO_DELETE_NOTIFY"))
            {
                IndexImgDelete.Visible = true;
                cbSelectAllNotify.Visible = true;
            }
            else
            {
                IndexImgDelete.Visible = false;
                cbSelectAllNotify.Visible = false;
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.headerHomeLblRole.Text = Utils.Languages.GetLabelFromCode("HeaderHomeLblRole", language);
            this.notificationCenterLblNotifyOtherRole.Text = Utils.Languages.GetLabelFromCode("IndexNotificationCenterLblNotifyOtherRole", language);
            this.IndexChkDoc.Text = Utils.Languages.GetLabelFromCode("IndexChkDoc", language);
            this.IndexChkProj.Text = Utils.Languages.GetLabelFromCode("IndexChkProj", language);
            this.IndexChkOther.Text = Utils.Languages.GetLabelFromCode("IndexChkOther", language);
            operational = Utils.Languages.GetLabelFromCode("IndexChkOperational", language);
            information = Utils.Languages.GetLabelFromCode("IndexChkInformation", language);
            this.IndexImgRefresh.ToolTip = Utils.Languages.GetLabelFromCode("IndexImgRefreshTooltip", language);
            this.IndexImgRefresh.AlternateText = Utils.Languages.GetLabelFromCode("IndexImgRefreshTooltip", language);
            this.IndexImgAddFilter.ToolTip = Utils.Languages.GetLabelFromCode("IndexImgAddFilterTooltip", language);
            this.IndexImgAddFilter.AlternateText = Utils.Languages.GetLabelFromCode("IndexImgAddFilterTooltip", language);
            this.IndexImgRemoveFilter.ToolTip = Utils.Languages.GetLabelFromCode("IndexImgRemoveFilterTooltip", language);
            this.IndexImgRemoveFilter.AlternateText = Utils.Languages.GetLabelFromCode("IndexImgRemoveFilterTooltip", language);
            this.IndexImgSmista.ToolTip = Utils.Languages.GetLabelFromCode("IndexImgSmistaTooltip", language);
            this.IndexImgSmista.AlternateText = Utils.Languages.GetLabelFromCode("IndexImgSmistaTooltip", language);
            this.IndexImgDelete.ToolTip = Utils.Languages.GetLabelFromCode("IndexImgDeleteTooltip", language);
            this.IndexImgDelete.AlternateText = Utils.Languages.GetLabelFromCode("IndexImgDeleteTooltip", language);
            this.IndexImgExport.ToolTip = Utils.Languages.GetLabelFromCode("IndexImgExportTooltip", language);
            this.IndexImgExport.AlternateText = Utils.Languages.GetLabelFromCode("IndexImgExportTooltip", language);
            this.ddlOrderByDateNotifyDescending.Text = Utils.Languages.GetLabelFromCode("IndexDdlOrderByDateNotifyDescending", language);
            this.ddlOrderByDateNotifyAscending.Text = Utils.Languages.GetLabelFromCode("IndexDdlOrderByDateNotifyAscending", language);
            this.ddlOrderByProduttore.Text = Utils.Languages.GetLabelFromCode("IndexDdlOrderByProduttore", language);
            this.ddlOrderByTypeEvent.Text = Utils.Languages.GetLabelFromCode("IndexDdlOrderByTypeEvent", language);
            this.ddlOrderByIdObject.Text = Utils.Languages.GetLabelFromCode("IndexDdlOrderByIdObject", language);
            this.IndexChkRead.Text = Utils.Languages.GetLabelFromCode("IndexChkRead", language);
            this.IndexChkNotRead.Text = Utils.Languages.GetLabelFromCode("IndexChkNotRead", language);
            this.DocumentViewer.Title = Utils.Languages.GetLabelFromCode("TitleDocumentViewerPopup", language);
            this.AddFilterNotificationCenter.Title = Utils.Languages.GetLabelFromCode("AddFilterNotificationCenterTitle", language);
            tooltipGoToDocument = Utils.Languages.GetLabelFromCode("IndexDetailsDocTooltip", UIManager.UserManager.GetUserLanguage());
            tooltipGoToFolder = Utils.Languages.GetLabelFromCode("IndexDetailsProjTooltip", UIManager.UserManager.GetUserLanguage());
            tooltipRemoveNotify = Utils.Languages.GetLabelFromCode("IndexImgRemoveNotifyTooltip", UIManager.UserManager.GetUserLanguage());
            this.ViewDetailNotify.Title = Utils.Languages.GetLabelFromCode("ViewDetailNotifyTitle", language);
            this.cbSelectAllNotify.ToolTip = Utils.Languages.GetLabelFromCode("ViewDetailNotifycbSelectAllNotifyTooltip", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddFilterAddressBookTitle", language);
            this.Object.Title = Utils.Languages.GetLabelFromCode("AddFilterObjectTitle", language);
            this.NotificationNoticeDays.Title = Utils.Languages.GetLabelFromCode("NotificationNoticeDaysTitle", language);
            this.SmistamentoDocumenti.Title = Utils.Languages.GetLabelFromCode("SmistamentoDocumenti", language);
            this.ExportDati.Title = utils.FormatJs(Utils.Languages.GetLabelFromCode("SearchDocumentExportDatiTitle", language));
            this.litTreeExpandAll.Text = Utils.Languages.GetLabelFromCode("IndexLtExpandAll", language);
            this.litTreeCollapseAll.Text = Utils.Languages.GetLabelFromCode("IndexLtCollapseAll", language);
            this.DigitalSignDetails.Title = Utils.Languages.GetLabelFromCode("DigitalSignDetailsTitle", language);
            this.Disservizio.Title = Utils.Languages.GetLabelFromCode("DisservizioTitle", language);
            this.VerificaConnettoreSocket.Title = Utils.Languages.GetLabelFromCode("VerificaConnettoreSocketTitle", language);
            this.SearchProject.Title = Utils.Languages.GetLabelFromCode("SearchProjectTitle", language);
            this.OpenTitolario.Title = Utils.Languages.GetLabelFromCode("TitleClassificationScheme", language);
        }

        private void ReadRetValueFromPopup()
        {

            if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals("closePopupDisservizio")))
            {
                InitializePage(true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('Disservizio','');", true);
                return;
            }

            if (!string.IsNullOrEmpty(this.mandate_exercise.Value))
            {
                Response.Redirect("Mandate/Mandate.aspx?t=index");
            }

            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_PANEL_BUTTONS))
            {
                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_ADD_FILTER_NOTIFICATION_CENTER)))
                {
                    if (!string.IsNullOrEmpty(this.AddFilterNotificationCenter.ReturnValue))
                    {
                        NotificationManager.ApplyFilters();
                        NotificationManager.ApplyFiltersNotifications();
                        OrderListNotifyFiltered();
                        this.IndexImgRemoveFilter.Enabled = true;
                        this.UpPnlBAction.Update();

                        RepListNotifyAll_Bind();
                        this.UpdatePanelRepListNotify.Update();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('AddFilterNotificationCenter','');", true);
                    }
                    return;
                }

                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(SELECT_NOTIFICATION)))
                {
                    UpdateListSelectNotify();
                    this.UpdatePanelRepListNotify.Update();
                    return;
                }

                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_ZOOM)))
                {
                    RemoveIsZoom();
                    if (UpdatePageAfterRemoveNotification)
                    {
                        HttpContext.Current.Session.Remove("UpdatePageAfterRemoveNotification");
                        ChkNotifyOperational_Bind();
                        this.UpdatePanelFilterOperational.Update();
                        ChkNotifyInformation_Bind();
                        this.UpdatePanelFilterInformation.Update();
                        LoadNumberNotifyInTheRole();
                        this.UpdatePanelNumberNotifyInTheRole.Update();
                        RepListNotifyAll_Bind();
                        this.UpdatePanelRepListNotify.Update();
                    }
                    return;
                }

                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(VIEW_NOTIFY_DETAILS)))
                {
                    RepListNotify_ViewNotifyDetail();
                    return;
                }

                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_VIEW_DETAIL_NOTIFY)))
                {

                    if ((!string.IsNullOrEmpty(this.ViewDetailNotify.ReturnValue) && this.ViewDetailNotify.ReturnValue.Equals("OPEN_PROJECT")) ||
                        (!string.IsNullOrEmpty(this.SmistamentoDocumenti.ReturnValue) && this.SmistamentoDocumenti.ReturnValue.Equals("OPEN_PROJECT")))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ViewDetailNotify','');", true);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SmistamentoDocumenti','');", true);
                        Response.Redirect(this.ResolveUrl("~/Project/Project.aspx?t=c"));
                        return;
                    }

                    RemoveNotificationInSession();
                    RemoveTransmissionInSession();
                    HttpContext.Current.Session.Remove("TrasmToElaborate");
                    if (!string.IsNullOrEmpty(this.grid_pageindex.Value))
                    {
                        this.SelectedPage = int.Parse(this.grid_pageindex.Value);
                    }
                    LoadNotify();
                    LoadNumberNotify();
                    LoadNumberNotifyInTheRole();
                    ChkNotifyOperational_Bind();
                    ChkNotifyInformation_Bind();
                    UpdateFilterOperational();
                    UpdateFilterInformation();
                    NotificationManager.ApplyFilters();
                    NotificationManager.ApplyFiltersNotifications();
                    OrderListNotifyFiltered();
                    RepListNotifyAll_Bind();
                    this.UpdatePanelRepListNotify.Update();
                    this.UpdatePanelFilterSx.Update();
                    return;
                }

                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_OBJECT)))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_AddFilterNotificationCenter').contentWindow.closeObjectPopup();", true);
                    return;
                }

                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_ADDRESS_BOOK)))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_AddFilterNotificationCenter').contentWindow.closeAddressBookPopup();", true);
                }
                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_SEARCH_PROJECT)))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_ViewDetailNotify').contentWindow.closeSearchProject();", true);
                }

                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_OPEN_TITOLARIO)))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_ViewDetailNotify').contentWindow.closeOpenTitolario();", true);
                }

                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_NOTIFICATION_NOTICE_DAYS)))
                {
                    if (!string.IsNullOrEmpty(this.NotificationNoticeDays.ReturnValue))
                    {
                        if (this.NotificationNoticeDays.ReturnValue.Equals(LOAD_NOTIFY))
                        {
                            UpdatePage(false);
                        }
                        else
                        {
                            ChkNotifyOperational_Bind();
                            ChkNotifyInformation_Bind();
                            this.UpdatePanelFilterSx.Update();
                            LoadNumberNotifyInTheRole();
                            this.UpdatePanelNumberNotifyInTheRole.Update();
                            RepListNotifyAll_Bind();
                            this.UpdatePanelRepListNotify.Update();
                        }
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('NotificationNoticeDays','');", true);
                    }
                    NotificationManager.DeleteNoticeDays();
                    return;
                }

            }

            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UPDATE_PANEL_GRID_INDEXES))
            {
                this.SelectedPage = int.Parse(this.grid_pageindex.Value);
                RepListNotifyAll_Bind();
                this.UpdatePanelRepListNotify.Update();
                return;
            }

            if (!string.IsNullOrEmpty(this.HiddenRemoveNotifications.Value))
            {
                RemoveSelectedNotifications();
                this.HiddenRemoveNotifications.Value = string.Empty;
                return;
            }

            if (!string.IsNullOrEmpty(this.HiddenRemoveNotification.Value))
            {

                this.HiddenRemoveNotification.Value = string.Empty;
                if (NotificationManager.RemoveNotification(new Notification[] { NotificationToRemove }))
                {
                    if (ListNotifySelected != null)
                        ListNotifySelected.Remove(NotificationToRemove.ID_NOTIFY);
                    NotificationManager.ListNotifyFiltered.Remove(NotificationToRemove);
                    NotificationManager.ListAllNotify.Remove(NotificationToRemove);
                    if (NotificationToRemove.TYPE_NOTIFICATION == NotificationManager.NotificationType.OPERATIONAL)
                    {
                        ChkNotifyOperational_Bind();
                        this.UpdatePanelFilterOperational.Update();
                    }
                    else
                    {
                        ChkNotifyInformation_Bind();
                        this.UpdatePanelFilterInformation.Update();
                    }
                    RemoveNotificationToRemove();
                    LoadNumberNotifyInTheRole();
                    this.UpdatePanelNumberNotifyInTheRole.Update();
                    RepListNotifyAll_Bind();
                    this.UpdatePanelRepListNotify.Update();
                }
                else
                {
                    RemoveNotificationToRemove();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('ErrorRemoveNotification', 'error', '','');", true);
                    return;
                }
                return;
            }
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        /// <summary>
        /// Carica il numero di eventi notificati ad altri ruoli dell'utente in sessione
        /// </summary>
        private void LoadNumberNotify()
        {
            try
            {
                if (this.containerNotificationCenterLblNotifyOtherRole.Visible)
                {
                    int numberNotify = NotificationManager.GetNumberNotifyOtherRoles(UserManager.GetUserInSession().idPeople, RoleManager.GetRoleInSession().idGruppo);
                    this.notificationCenterLblNotifyOtherRole.Text = Utils.Languages.GetLabelFromCode("IndexNotificationCenterLblNotifyOtherRole", UIManager.UserManager.GetUserLanguage()) + " (" + numberNotify + ")";
                }
                this.notificationCenterLblNotifyInTheRole.Text = Utils.Languages.GetLabelFromCode("IndexLblNotifyInTheRole", UIManager.UserManager.GetUserLanguage()) + " (" + NotificationManager.ListAllNotify.Count + ")";
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void LoadNumberNotifyInTheRole()
        {
            try
            {
                this.notificationCenterLblNotifyInTheRole.Text = Utils.Languages.GetLabelFromCode("IndexLblNotifyInTheRole", UIManager.UserManager.GetUserLanguage()) + " (" + NotificationManager.ListAllNotify.Count + ")";
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// Prende dal Backend le notifiche per l'utente in sessione
        /// </summary>
        private void LoadNotify()
        {
            NotificationManager.ListAllNotify = NotificationManager.ReadNotifications(UserManager.GetUserInSession().idPeople, RoleManager.GetRoleInSession().idGruppo);
            NotificationManager.ListNotifyFiltered = NotificationManager.ListAllNotify;

            if (NotificationManager.ListNotifyFiltered != null && NotificationManager.ListNotifyFiltered.Count > 0)
            {
                List<Notification> l = (from d in NotificationManager.ListNotifyFiltered where d.TYPE_NOTIFICATION == NotificationManager.NotificationType.OPERATIONAL select d).ToList<Notification>();
                if (l != null && l.Count > 0)
                {
                    var operationalFilters = (from n in l
                                              where n.DOMAINOBJECT.ToUpper() == "DOCUMENTO" && NotificationManager.GetTypeEvent(n.TYPE_EVENT).Equals(NotificationManager.EventType.TRASM)
                                              select n).ToList();
                    if (operationalFilters.Count == 0)
                    {
                        this.IndexImgSmista.Enabled = false;
                    }
                    else
                    {
                        this.IndexImgSmista.Enabled = true;
                    }
                }
                else
                {
                    this.IndexImgSmista.Enabled = false;
                }
            }
            else
            {
                this.IndexImgSmista.Enabled = false;
            }
        }

        private void LoadRolesUser()
        {
            try
            {
                ListItem item;
                Ruolo[] role = UIManager.UserManager.GetUserInSession().ruoli;
                foreach (Ruolo r in role)
                {
                    item = new ListItem();
                    item.Value = r.systemId;
                    item.Text = r.descrizione;
                    this.ddlRolesUser.Items.Add(item);
                }
                if (role.Count() == 1)
                    this.containerNotificationCenterLblNotifyOtherRole.Visible = false;

                this.ddlRolesUser.SelectedValue = RoleManager.GetRoleInSession().systemId;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        protected void IndexImgSmista_Click(object sender, ImageClickEventArgs e)
        {
            //Lnr 23/05/2013 - 07/06/2013 disabilito tasto smista se non ho trasm operative di tipo D
            HttpContext.Current.Session["SMISTA_DOC_MANAGER"] = null;
            HttpContext.Current.Session.Remove("ClosePopupSmistamento");
            List<Notification> l = (from d in NotificationManager.ListNotifyFiltered where d.TYPE_NOTIFICATION == NotificationManager.NotificationType.OPERATIONAL select d).ToList<Notification>();
            if (l != null && l.Count > 0)
            {
                var operationalFilters = (from n in l
                                          where n.DOMAINOBJECT.ToUpper() == "DOCUMENTO" && NotificationManager.GetTypeEvent(n.TYPE_EVENT).Equals(NotificationManager.EventType.TRASM)
                                          select n).ToList();
                if (operationalFilters.Count > 0)
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "", "ajaxModalPopupSmistamentoDocumenti();", true);
            }
        }

        #region Bind
        private void RepListNotifyAll_Bind()
        {
            try
            {
                PagedDataSource dsP = new PagedDataSource();
                dsP.DataSource = NotificationManager.ListNotifyFiltered;
                dsP.AllowPaging = true;
                if (NotificationManager.ListNotifyFiltered.Count > 0 && ((float)NotificationManager.ListNotifyFiltered.Count / PAGE_SIZE) <= (SelectedPage - 1))
                    SelectedPage = (NotificationManager.ListNotifyFiltered.Count % PAGE_SIZE) > 0 ?
                        (NotificationManager.ListNotifyFiltered.Count / PAGE_SIZE) + 1 : (NotificationManager.ListNotifyFiltered.Count / PAGE_SIZE);
                //SelectedPage -= 1;
                dsP.CurrentPageIndex = SelectedPage - 1;
                dsP.PageSize = PAGE_SIZE;

                this.repListNotify.DataSource = dsP;
                this.repListNotify.DataBind();
                if (ListNotifySelected != null && ListNotifySelected.Count() > 0)
                {
                    foreach (RepeaterItem item in repListNotify.Items)
                    {
                        (item.FindControl("IndexChkRemoveNotify") as CheckBox).Checked = (from n in ListNotifySelected
                                                                                          where n.Equals((item.FindControl("NotifyId") as HiddenField).Value)
                                                                                          select n).Count() > 0;


                    }
                }
                GridNavigator_Bind(PAGE_SIZE, NotificationManager.ListNotifyFiltered.Count());
                if (ExpandAll)
                {
                    foreach (RepeaterItem item in this.repListNotify.Items)
                    {
                        (item.FindControl("containerNotifyDetails") as HtmlGenericControl).Attributes["style"] = "display:block";
                        (item.FindControl("containerListNotifyBt") as HtmlGenericControl).Attributes["class"] = "containerListNotifyBtWithDetails";

                        (item.FindControl("ExpandCollapse") as CustomImageButton).ImageUrl = "Images/Icons/expanded.png";
                        (item.FindControl("ExpandCollapse") as CustomImageButton).ImageUrlDisabled = "Images/Icons/expanded_disabled.png";
                        (item.FindControl("ExpandCollapse") as CustomImageButton).OnMouseOutImage = "Images/Icons/expanded.png";
                        (item.FindControl("ExpandCollapse") as CustomImageButton).OnMouseOverImage = "Images/Icons/expanded_hover.png";
                        (item.FindControl("ExpandCollapse") as CustomImageButton).AlternateText = Utils.Languages.GetLabelFromCode("HomeCollapseNotify", UIManager.UserManager.GetUserLanguage());
                        (item.FindControl("ExpandCollapse") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("HomeCollapseNotify", UIManager.UserManager.GetUserLanguage());


                    }
                    this.UpdatePanelRepListNotify.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GridNavigator_Bind(int pagesize, int recordNum)
        {
            int pageCount = (int)Math.Round(((double)recordNum / (double)pagesize) + 0.49);
            this.plcNavigator.Controls.Clear();
            Panel panel = new Panel();
            if (pageCount > 1)
            {

                panel.CssClass = "recordNavigator2";

                for (int i = 1; i < pageCount + 1; i++)
                {
                    if (i == SelectedPage)
                    {
                        Literal lit = new Literal();
                        lit.Text = "<span class=\"linkPageHome\">" + i.ToString() + "</span>";
                        panel.Controls.Add(lit);
                    }
                    else
                    {
                        LinkButton btn = new LinkButton();
                        btn.Text = i.ToString();
                        btn.CssClass = "linkPageHome";
                        btn.Attributes["onclick"] = "$('#grid_pageindex').val(" + i + "); __doPostBack('upPnlGridIndexes', 'changePage');  return false;";
                        panel.Controls.Add(btn);
                    }
                }
            }
            this.plcNavigator.Controls.Add(panel);
            this.upPnlGridIndexes.Update();
        }

        protected void RepListNotify_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                (e.Item.FindControl("containerNotifyDetails") as HtmlGenericControl).Attributes["style"] = "display:none";
                (e.Item.FindControl("containerNoteNotify") as HtmlGenericControl).Attributes["style"] = "display:none";
                ((HtmlGenericControl)e.Item.FindControl("containerNoteNotify")).Attributes["class"] = "containerNoteNotify_" + e.Item.ItemIndex;
                (e.Item.FindControl("nameNotify") as HtmlGenericControl).Attributes["onClick"] = "$('#grid_rowindex').val('" + e.Item.ItemIndex + "');__doPostBack('UpPnlButtons','viewNotifyDetails');return false;";
                (e.Item.FindControl("ExpandCollapse") as CustomImageButton).Attributes["onClick"] = "$('#grid_rowindex').val('" + e.Item.ItemIndex + "');__doPostBack('UpPnlButtons','viewNotifyDetails');return false;";
                (e.Item.FindControl("ExpandCollapse") as CustomImageButton).AlternateText = Utils.Languages.GetLabelFromCode("HomeExpandNotify", UIManager.UserManager.GetUserLanguage());
                (e.Item.FindControl("ExpandCollapse") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("HomeExpandNotify", UIManager.UserManager.GetUserLanguage());
                (e.Item.FindControl("IndexChkRemoveNotify") as CheckBox).Attributes["onClick"] = "$('#grid_rowindex').val('" + e.Item.ItemIndex + "');__doPostBack('UpPnlButtons','selectNotification');return false;";
                ((HtmlGenericControl)e.Item.FindControl("containerNotifyDetails")).Attributes["class"] = "containerNotifyDetails_" + e.Item.ItemIndex;
                (e.Item.FindControl("IndexImgAdd") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("IndexBtnImgAddTooltip", UIManager.UserManager.GetUserLanguage());
                CustomImageButton buttonTypeDoc = (e.Item.FindControl("btnTypeDoc") as CustomImageButton);
                CustomImageButton buttonDetailsDocProj = (e.Item.FindControl("IndexImgDetailsDocument") as CustomImageButton);


                Notification notification = e.Item.DataItem as Notification;
                string outImg = string.Empty;
                string outImg2 = string.Empty;
                if (notification != null)
                {
                    if (notification.READ_NOTIFICATION == NotificationManager.ReadNotification.NO_READ)
                        (e.Item.FindControl("labelContainerTop") as HtmlGenericControl).Attributes["class"] = "labelContainerTop";
                    if (notification.COLOR.Equals(NotificationManager.Color.BLUE))
                    {
                        if (notification.TYPE_NOTIFICATION == NotificationManager.NotificationType.OPERATIONAL)
                        {
                            (e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).Attributes["style"] = "background-image: url('Images/Common/blu.png'); float:left;width:116px;height:34px;";
                            (e.Item.FindControl("boxNotifyHomeDx") as HtmlGenericControl).Attributes["style"] = "background-image: url('Images/Common/blu_1.png');   height:34px;width:100%;";
                            outImg = "Images/Common/blu.png";
                            outImg2 = "Images/Common/blu_1.png";
                            //(e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).Attributes.Add("onMouseOver", "changeBgImage('Images/Common/blu_hover.png', '" + (e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).ClientID.ToString() + "');");
                            //(e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).Attributes.Add("onMouseOut", "changeBgImage('" + outImg + "', '" + (e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).ClientID.ToString() + "');");

                            (e.Item.FindControl("boxNotifyHomeDx") as HtmlGenericControl).Attributes.Add("onMouseOver", "changeBgImage('Images/Common/blu_1_hover.png', '" + (e.Item.FindControl("boxNotifyHomeDx") as HtmlGenericControl).ClientID.ToString() + "','Images/Common/blu_hover.png','" + (e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).ClientID.ToString() + "');");
                            (e.Item.FindControl("boxNotifyHomeDx") as HtmlGenericControl).Attributes.Add("onMouseOut", "changeBgImage('" + outImg2 + "', '" + (e.Item.FindControl("boxNotifyHomeDx") as HtmlGenericControl).ClientID.ToString() + "','" + outImg + "', '" + (e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).ClientID.ToString() + "');");
                        }
                        else
                        {
                            (e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).Attributes["style"] = "background-image: url('Images/Common/celeste.png'); float:left;width:116px;height:34px;";
                            (e.Item.FindControl("boxNotifyHomeDx") as HtmlGenericControl).Attributes["style"] = "background-image: url('Images/Common/celeste_1.png');   height:34px;width:100%;";
                            outImg = "Images/Common/celeste.png";
                            outImg2 = "Images/Common/celeste_1.png";
                            //(e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).Attributes.Add("onMouseOver", "changeBgImage('Images/Common/blu_hover.png', '" + (e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).ClientID.ToString() + "');");
                            //(e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).Attributes.Add("onMouseOut", "changeBgImage('" + outImg + "', '" + (e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).ClientID.ToString() + "');");

                            (e.Item.FindControl("boxNotifyHomeDx") as HtmlGenericControl).Attributes.Add("onMouseOver", "changeBgImage('Images/Common/celeste_1_hover.png', '" + (e.Item.FindControl("boxNotifyHomeDx") as HtmlGenericControl).ClientID.ToString() + "','Images/Common/celeste_hover.png','" + (e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).ClientID.ToString() + "');");
                            (e.Item.FindControl("boxNotifyHomeDx") as HtmlGenericControl).Attributes.Add("onMouseOut", "changeBgImage('" + outImg2 + "', '" + (e.Item.FindControl("boxNotifyHomeDx") as HtmlGenericControl).ClientID.ToString() + "','" + outImg + "', '" + (e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).ClientID.ToString() + "');");
                        }
                    }
                    else
                    {
                        //(e.Item.FindControl("containerListNotifyTop") as HtmlGenericControl).Attributes["style"] = "background-image:url(Images/Common/bg_normal_notify_reject.png);";
                        //outImg = "Images/Common/bg_normal_notify_reject.png";
                        (e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).Attributes["style"] = "background-image: url('Images/Common/rosso.png'); float:left;width:116px;height:34px;";
                        (e.Item.FindControl("boxNotifyHomeDx") as HtmlGenericControl).Attributes["style"] = "background-image: url('Images/Common/rosso_1.png');   height:34px;width:100%;";

                        outImg = "Images/Common/rosso.png";
                        outImg2 = "Images/Common/rosso_1.png";

                        //(e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).Attributes.Add("onMouseOver", "changeBgImage('Images/Common/rosso_hover.png', '" + (e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).ClientID.ToString() + "');");
                        //(e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).Attributes.Add("onMouseOut", "changeBgImage('" + outImg + "', '" + (e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).ClientID.ToString() + "');");

                        (e.Item.FindControl("boxNotifyHomeDx") as HtmlGenericControl).Attributes.Add("onMouseOver", "changeBgImage('Images/Common/rosso_1_hover.png', '" + (e.Item.FindControl("boxNotifyHomeDx") as HtmlGenericControl).ClientID.ToString() + "','Images/Common/rosso_hover.png', '" + (e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).ClientID.ToString() + "');");
                        (e.Item.FindControl("boxNotifyHomeDx") as HtmlGenericControl).Attributes.Add("onMouseOut", "changeBgImage('" + outImg2 + "', '" + (e.Item.FindControl("boxNotifyHomeDx") as HtmlGenericControl).ClientID.ToString() + "','" + outImg + "', '" + (e.Item.FindControl("boxNotifyHomeSx") as HtmlGenericControl).ClientID.ToString() + "');");
                    }
                    //(e.Item.FindControl("containerListNotifyTop") as HtmlGenericControl).Attributes.Add("onMouseOver", "changeBgImage('Images/Common/bg_normal_notify_hover.png', '" + (e.Item.FindControl("containerListNotifyTop") as HtmlGenericControl).ClientID.ToString() + "');");
                    //(e.Item.FindControl("containerListNotifyTop") as HtmlGenericControl).Attributes.Add("onMouseOut", "changeBgImage('"+outImg+"', '" + (e.Item.FindControl("containerListNotifyTop") as HtmlGenericControl).ClientID.ToString() + "');");
                    if (UIManager.UserManager.IsAuthorizedFunctions("DO_DELETE_NOTIFY"))
                    {
                        (e.Item.FindControl("IndexImgRemoveNotify") as CustomImageButton).Visible = true;
                        (e.Item.FindControl("IndexChkRemoveNotify") as CheckBox).Visible = true;
                        (e.Item.FindControl("IndexImgRemoveNotify") as CustomImageButton).ToolTip = tooltipRemoveNotify;
                    }
                    else
                    {
                        (e.Item.FindControl("IndexChkRemoveNotify") as CheckBox).Visible = false;
                        (e.Item.FindControl("IndexImgRemoveNotify") as CustomImageButton).Visible = false;
                    }
                    switch (notification.DOMAINOBJECT)
                    {
                        case FiltersNotifications.DOCUMENT:
                        case FiltersNotifications.ATTACH:
                            {
                                if (!string.IsNullOrEmpty(notification.EXTENSION))
                                {
                                    string url = ResolveUrl(FileManager.getFileIcon(this.Page, notification.EXTENSION));
                                    buttonTypeDoc.ImageUrl = url;
                                    buttonTypeDoc.OnMouseOverImage = url;
                                    buttonTypeDoc.OnMouseOutImage = url;
                                    buttonTypeDoc.ImageUrlDisabled = url;

                                }
                                else
                                {
                                    buttonTypeDoc.Visible = false;
                                }
                                buttonDetailsDocProj.ImageUrl = "Images/Icons/search_response_documents.png";
                                buttonDetailsDocProj.OnMouseOverImage = "Images/Icons/search_response_documents_hover.png";
                                buttonDetailsDocProj.OnMouseOutImage = "Images/Icons/search_response_documents.png";
                                buttonDetailsDocProj.ImageUrlDisabled = "Images/Icons/search_response_documents_disabled.png";
                                buttonDetailsDocProj.ToolTip = tooltipGoToDocument;

                                if (notification.SIGNED == '1')
                                    (e.Item.FindControl("btnSignatureDetails") as CustomImageButton).Visible = true;
                                break;
                            }
                        case FiltersNotifications.FOLDER:
                            {
                                (e.Item.FindControl("btnTypeDoc") as CustomImageButton).Visible = false;
                                buttonDetailsDocProj.ImageUrl = "Images/Icons/project.png";
                                buttonDetailsDocProj.OnMouseOverImage = "Images/Icons/project_hover.png";
                                buttonDetailsDocProj.OnMouseOutImage = "Images/Icons/project.png";
                                buttonDetailsDocProj.ImageUrlDisabled = "Images/Icons/project_disabled.png";
                                buttonDetailsDocProj.ToolTip = tooltipGoToFolder;

                                break;
                            }
                        default:
                            {

                                (e.Item.FindControl("btnTypeDoc") as CustomImageButton).Visible = false;
                                break;
                            }
                    }
                    if (string.IsNullOrEmpty(notification.NOTES))
                    {
                        CustomImageButton button = e.Item.FindControl("IndexImgAdd") as CustomImageButton;
                        button.Style.Add("visibility", "visible");
                        (e.Item.FindControl("containerNoteNotify") as HtmlGenericControl).Attributes["style"] = "display:none";
                    }
                    else
                    {
                        CustomImageButton button = e.Item.FindControl("IndexImgAdd") as CustomImageButton;
                        button.Style.Add("visibility", "hidden");
                        if (UserManager.IsAuthorizedFunctions("HOME_NOTES"))
                        {
                            (e.Item.FindControl("containerNoteNotify") as HtmlGenericControl).Attributes["style"] = "display:block";
                            (e.Item.FindControl("txtNoteNotify") as CustomTextArea).Text = notification.NOTES;
                            (e.Item.FindControl("containerNoteHomeSx") as HtmlGenericControl).Attributes["class"] = "containerNoteHomeSxNote";
                        }
                    }
                }
                if (!EnableButtonViewDetailTrasmissione(notification.TYPE_EVENT))
                    (e.Item.FindControl("IndexImgDetailsNotify") as CustomImageButton).Attributes["style"] = "visibility:hidden;";
            }
        }

        protected bool GetEnableNote()
        {
            return UserManager.IsAuthorizedFunctions("HOME_NOTES");
        }

        private void ChkNotifyOperational_Bind()
        {
            try
            {
                List<Notification> l = (from d in NotificationManager.ListAllNotify where d.TYPE_NOTIFICATION == NotificationManager.NotificationType.OPERATIONAL select d).ToList<Notification>();
                if (l != null && l.Count > 0)
                {
                    this.containerOperationalNotify.Attributes["style"] = "display:block";
                    this.IndexChkOperational.Text = operational + " (" + l.Count + ")";
                    var operationalFilters = (from n in l
                                              group n by n.TYPE_EVENT into groupNotification
                                              select new
                                              {
                                                  TEXT = NotificationManager.getLabelTypeEvent(groupNotification.Key) +
                                                      " (" + groupNotification.Count() + ")",
                                                  VALUE = groupNotification.Key
                                              });
                    List<ListItem> listItem = new List<ListItem>();
                    foreach (var filter in operationalFilters)
                    {
                        listItem.Add(new ListItem()
                        {
                            Text = filter.TEXT,
                            Value = filter.VALUE,
                            Selected = (from i in this.IndexCblOperational.Items.Cast<ListItem>()
                                        where i.Value.Equals(filter.VALUE)
                                        select i).Count() > 0 ? (from i in this.IndexCblOperational.Items.Cast<ListItem>()
                                                                 where
                                                                     i.Value.Equals(filter.VALUE)
                                                                 select i.Selected).FirstOrDefault() : true

                        }
                             );
                    }

                    this.IndexCblOperational.Items.Clear();
                    this.IndexCblOperational.Items.AddRange(listItem.ToArray());
                }
                else
                    this.containerOperationalNotify.Attributes["style"] = "display:none";
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ChkNotifyInformation_Bind()
        {
            try
            {
                List<Notification> l = (from n in NotificationManager.ListAllNotify where n.TYPE_NOTIFICATION == NotificationManager.NotificationType.INFORMATION select n).ToList<Notification>();
                if (l != null && l.Count > 0)
                {
                    this.containerInformationNotify.Attributes["style"] = "display:block";
                    this.IndexChkInformation.Text = information + " (" + l.Count + ")";
                    var informationFilters = (from n in l
                                              group n by n.TYPE_EVENT into groupNotification
                                              select new
                                              {
                                                  TEXT = NotificationManager.getLabelTypeEvent(groupNotification.Key) +
                                                      " (" + groupNotification.Count() + ")",
                                                  VALUE = groupNotification.Key
                                              });
                    List<ListItem> listItem = new List<ListItem>();
                    foreach (var filter in informationFilters)
                    {
                        listItem.Add(new ListItem()
                        {
                            Text = filter.TEXT,
                            Value = filter.VALUE,
                            Selected = (from i in this.IndexCblInformation.Items.Cast<ListItem>()
                                        where i.Value.Equals(filter.VALUE)
                                        select i).Count() > 0 ? (from i in this.IndexCblInformation.Items.Cast<ListItem>()
                                                                 where
                                                                     i.Value.Equals(filter.VALUE)
                                                                 select i.Selected).FirstOrDefault() : true

                        }
                            );
                    }

                    this.IndexCblInformation.Items.Clear();
                    this.IndexCblInformation.Items.AddRange(listItem.ToArray());
                }
                else
                    this.containerInformationNotify.Attributes["style"] = "display:none";
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected string getLabelField(Notification notification)
        {
            return NotificationManager.getLabelNotifyField(notification.ITEMS);
        }

        protected string getLabelFieldLink(Notification notification)
        {
            return NotificationManager.getLabelNotifyFieldLink(notification.ITEMS);
        }


        protected string GetLabelSpecializedField(Notification notification)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(notification.ITEM_SPECIALIZED))
            {
                string[] splitL = notification.ITEM_SPECIALIZED.Split(new string[] { LINE }, StringSplitOptions.None);
                foreach (string s in splitL)
                {
                    result += NotificationManager.getLabel(s);
                }
                result = result.Replace(LINE_C, "<br />").Replace(LINE, "");
            }
            string id_dta_Notify = Utils.Languages.GetLabelFromCode("lblIdNotification", UIManager.UserManager.GetUserLanguage()) + notification.ID_NOTIFY + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + Utils.Languages.GetLabelFromCode("lblDta_notify", UIManager.UserManager.GetUserLanguage()) + " " + notification.DTA_NOTIFY + "</br>";
            return id_dta_Notify + result;
        }

        protected string GetHeader(Notification notification)
        {
            string result = NotificationManager.getLabelTypeEvent(notification.TYPE_EVENT);
            int index = NotificationManager.ListNotifyFiltered.Select((item, i) => new { not = item, index = i }).First(item => item.not.ID_NOTIFY.Equals(notification.ID_NOTIFY)).index;
            NotificationManager.ListNotifyFiltered[index].TEXT_SORTING = result;
            string formatString = notification.PRODUCER;

            if (!string.IsNullOrEmpty(notification.PRODUCER))
            {
                if (notification.PRODUCER.EndsWith(")"))
                {
                    string role = string.Empty;
                    string name = string.Empty;
                    int startPosition = notification.PRODUCER.LastIndexOf("(");
                    int endTagStartPosition = notification.PRODUCER.LastIndexOf(")");

                    role = formatString.Substring(startPosition + 1, endTagStartPosition - 1 - startPosition);
                    name = formatString.Substring(0, startPosition - 1);
                    formatString = name + " <span class=\"homeItalic\">(" + role + ")</span>";
                }
            }
            if (!string.IsNullOrEmpty(notification.PRODUCER))
                result = string.IsNullOrEmpty(result) ? notification.PRODUCER : result + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + "<img src=\"Images/Icons/home_user_ico.png\" alt=\"\" /> " + formatString;

            return result;
        }

        /// <summary>
        /// Aggiorna la pagina in seguito all'applicazione di un filtro
        /// </summary>
        /// <param name="changedRole">E' true se si devono reimpostare tutti i filtri dovuto ad un cambio di ruolo</param>
        private void UpdatePage(bool updateNotify)
        {
            ClearSession();
            LoadNotify();
            LoadNumberNotify();
            LoadNumberNotifyInTheRole();
            if (updateNotify)
            {
                ClearFilter();
            }
            else
            {
                ChkNotifyOperational_Bind();
                ChkNotifyInformation_Bind();
                UpdateFilterOperational();
                UpdateFilterInformation();
                NotificationManager.ApplyFilters();
                NotificationManager.ApplyFiltersNotifications();
                OrderListNotifyFiltered();
                cbSelectAllNotify.Checked = false;
            }
            RepListNotifyAll_Bind();
            this.UpdatePanelRepListNotify.Update();
            this.UpdatePanelFilterSx.Update();
        }

        /// <summary>
        /// Reimposta tutti i filtri al default
        /// </summary>
        private void ClearFilter()
        {
            this.IndexCblOperational.Items.Clear();
            this.IndexCblInformation.Items.Clear();
            this.IndexChkInformation.Checked = true;
            this.IndexChkOperational.Checked = true;
            ChkNotifyOperational_Bind();
            ChkNotifyInformation_Bind();
            UpdateFilterOperational();
            UpdateFilterInformation();
            ResetChkReadAndNotRead();
            UpdateFilterReadNotification();
            ResetFilterTypeObject();
            UpdateFilterObject();
            this.cbSelectAllNotify.Checked = false;
            NotificationManager.DeleteFiltersNotifications();
            this.IndexImgRemoveFilter.Enabled = false;
            this.UpPnlBAction.Update();
            ResetDdlOrderBy();
            this.UpdatePanelDdlOrderBy.Update();
        }

        #endregion

        #region Event hendler

        #region Filter

        public void HomeDdlRoles_SelectedIndexChange(object sender, EventArgs e)
        {
            Ruolo[] roles = UIManager.UserManager.GetUserInSession().ruoli;
            Ruolo role = (from r in roles where r.systemId.Equals(this.ddlRolesUser.SelectedValue) select r).FirstOrDefault();
            UIManager.RoleManager.SetRoleInSession(role);
            UIManager.RegistryManager.SetRegAndRFListInSession(UIManager.UserManager.getListaRegistriWithRF(role.systemId,"",""));
            UIManager.RegistryManager.SetRFListInSession(UIManager.UserManager.getListaRegistriWithRF(role.systemId, "1", ""));
            ClearSession();
            IsFirstTime = true;
            RemoveExpandAllInSession();

            //Rimuovo la lista degli ultimi documenti visualizzati dal ruolo precedente
            RemoveLastDocumentsView();

            NotificationManager.DeleteFiltersNotifications();
            Response.Redirect("~/Index.aspx");
            Response.End();
            return;
        }

        protected void DdlOrderBy_SelectedIndexChange(object sender, EventArgs e)
        {
            try
            {
                string orderBy = this.ddlOrderBy.SelectedValue;
                OrderBy = orderBy;
                switch (orderBy)
                {
                    case ORDER_BY_DATE_DESCENDING:
                        NotificationManager.ListNotifyFiltered = (from n in NotificationManager.ListNotifyFiltered orderby n.DTA_EVENT descending select n).ToList<Notification>();
                        break;
                    case ORDER_BY_DATE_ASCENDING:
                        NotificationManager.ListNotifyFiltered = (from n in NotificationManager.ListNotifyFiltered orderby n.DTA_EVENT ascending select n).ToList<Notification>();
                        break;
                    case ORDER_BY_TYPE_EVENT:
                        NotificationManager.ListNotifyFiltered = (from n in NotificationManager.ListNotifyFiltered orderby n.TEXT_SORTING ascending select n).ToList<Notification>();
                        break;
                    case ORDER_BY_PRODUTTORE:
                        NotificationManager.ListNotifyFiltered = (from n in NotificationManager.ListNotifyFiltered orderby n.PRODUCER ascending select n).ToList<Notification>();
                        break;
                    case ORDER_BY_ID_OBJECT:
                        NotificationManager.ListNotifyFiltered = (from n in NotificationManager.ListNotifyFiltered orderby n.ID_OBJECT descending, n.DTA_EVENT descending select n).ToList<Notification>();
                        break;
                }
                RepListNotifyAll_Bind();
                this.UpdatePanelRepListNotify.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }



        protected void IndexChkRead_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!this.IndexChkRead.Checked && !this.IndexChkNotRead.Checked)
                {
                    ResetChkReadAndNotRead();
                }
                UpdateFilterReadNotification();
                NotificationManager.ApplyFilters();
                NotificationManager.ApplyFiltersNotifications();
                OrderListNotifyFiltered();
                RepListNotifyAll_Bind();
                this.UpdatePanelRepListNotify.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void IndexChkOperational_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                /* EMANUELA 09-06-2015: SU RICHIESTA DI LUCA LUCIANI, ELIMINATO IL CONTROLLO
                //Se non ho impostato alcun filtro per il tipo di notifica, li reimposto entrambi a checked per non mostrare la griglia vuota
                if (!IndexChkOperational.Checked && (this.IndexCblInformation.Items.Count < 0 || this.IndexCblInformation.SelectedItem == null))
                {
                    ResetFilterTypeNotify();
                    UpdateFilterOperational();
                    UpdateFilterInformation();
                }
                else
                {
                    foreach (ListItem item in this.IndexCblOperational.Items)
                    {
                        (item).Selected = IndexChkOperational.Checked;
                        UpdateFilterOperational();
                    }
                }
                 * */

                foreach (ListItem item in this.IndexCblOperational.Items)
                {
                    (item).Selected = IndexChkOperational.Checked;
                    UpdateFilterOperational();
                }

                NotificationManager.ApplyFilters();
                NotificationManager.ApplyFiltersNotifications();
                OrderListNotifyFiltered();
                RepListNotifyAll_Bind();
                this.UpdatePanelRepListNotify.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void IndexChkInformation_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                /* EMANUELA 09-06-2015: SU RICHIESTA DI LUCA LUCIANI, ELIMINATO IL CONTROLLO
                //Se non ho impostato alcun filtro per il tipo di notifica, li reimposto entrambi a checked per non mostrare la griglia vuota
                if (!IndexChkInformation.Checked && (this.IndexCblOperational.Items.Count < 0 || this.IndexCblOperational.SelectedItem == null))
                {
                    ResetFilterTypeNotify();
                    UpdateFilterOperational();
                    UpdateFilterInformation();
                }
                else
                {
                    foreach (ListItem item in this.IndexCblInformation.Items)
                    {
                        (item).Selected = IndexChkInformation.Checked;
                        UpdateFilterInformation();
                    }
                }
                 * */

                foreach (ListItem item in this.IndexCblInformation.Items)
                {
                    (item).Selected = IndexChkInformation.Checked;
                    UpdateFilterInformation();
                }

                NotificationManager.ApplyFilters();
                NotificationManager.ApplyFiltersNotifications();
                OrderListNotifyFiltered();
                RepListNotifyAll_Bind();
                this.UpdatePanelRepListNotify.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        protected void IndexCblInformation_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.IndexCblInformation.SelectedItem == null)
                {
                    this.IndexChkInformation.Checked = false;
                    /* EMANUELA 09-06-2015: SU RICHIESTA DI LUCA LUCIANI, ELIMINATO IL CONTROLLO
                    //Se non ho impostato alcun filtro per il tipo di notifica, li reimposto entrambi a checked per non mostrare la griglia vuota
                    if (this.IndexCblOperational.Items.Count < 0 || this.IndexCblOperational.SelectedItem == null)
                    {
                        ResetFilterTypeNotify();
                        UpdateFilterOperational();
                    }
                     * */
                }
                else
                {
                    this.IndexChkInformation.Checked = true;
                }
                UpdateFilterInformation();
                NotificationManager.ApplyFilters();
                NotificationManager.ApplyFiltersNotifications();
                OrderListNotifyFiltered();
                RepListNotifyAll_Bind();
                this.UpdatePanelRepListNotify.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void IndexCblOperational_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.IndexCblOperational.SelectedItem == null)
                {
                    this.IndexChkOperational.Checked = false;

                    /* EMANUELA 09-06-2015: SU RICHIESTA DI LUCA LUCIANI, ELIMINATO IL CONTROLLO
                    //Se non ho impostato alcun filtro per il tipo di notifica, li reimposto entrambi a checked per non mostrare la griglia vuota
                    if (this.IndexCblInformation.Items.Count < 0 || this.IndexCblInformation.SelectedItem == null)
                    {
                        ResetFilterTypeNotify();
                        UpdateFilterInformation();
                    }
                     */
                }
                else
                {
                    this.IndexChkOperational.Checked = true;
                }
                UpdateFilterOperational();
                NotificationManager.ApplyFilters();
                NotificationManager.ApplyFiltersNotifications();
                OrderListNotifyFiltered();
                RepListNotifyAll_Bind();
                this.UpdatePanelRepListNotify.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void IndexCkbFilterObject_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!this.IndexChkDoc.Checked && !this.IndexChkProj.Checked && !this.IndexChkOther.Checked)
                {
                    ResetFilterTypeObject();
                }
                UpdateFilterObject();
                NotificationManager.ApplyFilters();
                NotificationManager.ApplyFiltersNotifications();
                OrderListNotifyFiltered();
                RepListNotifyAll_Bind();
                this.UpdatePanelRepListNotify.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #region Buttom

        protected void IndexImgRefresh_OnClick(object sender, EventArgs e)
        {
            try
            {
                UpdatePage(false);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ExpandAll_Click(object sender, EventArgs e)
        {
            ExpandAll = true;
            foreach (RepeaterItem item in this.repListNotify.Items)
            {
                (item.FindControl("containerNotifyDetails") as HtmlGenericControl).Attributes["style"] = "display:block";
                (item.FindControl("containerListNotifyBt") as HtmlGenericControl).Attributes["class"] = "containerListNotifyBtWithDetails";

                string idNotify = (item.FindControl("NotifyId") as HiddenField).Value;
                Notification notification = (from n in NotificationManager.ListNotifyFiltered where n.ID_NOTIFY.Equals(idNotify) select n).FirstOrDefault();
                if (notification.READ_NOTIFICATION == NotificationManager.ReadNotification.NO_READ && notification.DOMAINOBJECT.Equals(NotificationManager.ListDomainObject.JOB))
                {
                    NotificationManager.ChangeStateReadNotification(notification, false);
                    int indexListFilteres = NotificationManager.ListNotifyFiltered.Select((item1, i) => new { not = item1, index = i }).First(item1 => item1.not.ID_NOTIFY.Equals(idNotify)).index;
                    NotificationManager.ListNotifyFiltered[indexListFilteres].READ_NOTIFICATION = '1';
                    int indexListAll = NotificationManager.ListAllNotify.Select((item1, i) => new { not = item1, index = i }).First(item1 => item1.not.ID_NOTIFY.Equals(idNotify)).index;
                    NotificationManager.ListAllNotify[indexListAll].READ_NOTIFICATION = '1';
                    (item.FindControl("labelContainerTop") as HtmlGenericControl).Attributes["class"] = "";
                }
                (item.FindControl("ExpandCollapse") as CustomImageButton).ImageUrl = "Images/Icons/expanded.png";
                (item.FindControl("ExpandCollapse") as CustomImageButton).ImageUrlDisabled = "Images/Icons/expanded_disabled.png";
                (item.FindControl("ExpandCollapse") as CustomImageButton).OnMouseOutImage = "Images/Icons/expanded.png";
                (item.FindControl("ExpandCollapse") as CustomImageButton).OnMouseOverImage = "Images/Icons/expanded_hover.png";
                (item.FindControl("ExpandCollapse") as CustomImageButton).AlternateText = Utils.Languages.GetLabelFromCode("HomeCollapseNotify", UIManager.UserManager.GetUserLanguage());
                (item.FindControl("ExpandCollapse") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("HomeCollapseNotify", UIManager.UserManager.GetUserLanguage());
            }
            this.UpdatePanelRepListNotify.Update();
        }

        protected void CollapseAll_Click(object sender, EventArgs e)
        {
            ExpandAll = false;
            foreach (RepeaterItem item in this.repListNotify.Items)
            {
                (item.FindControl("containerNotifyDetails") as HtmlGenericControl).Attributes["style"] = "display:none";
                (item.FindControl("containerListNotifyBt") as HtmlGenericControl).Attributes["class"] = "containerListNotifyBt";

                (item.FindControl("ExpandCollapse") as CustomImageButton).ImageUrl = "Images/Icons/collapsed.png";
                (item.FindControl("ExpandCollapse") as CustomImageButton).ImageUrlDisabled = "Images/Icons/collapsed_disabled.png";
                (item.FindControl("ExpandCollapse") as CustomImageButton).OnMouseOutImage = "Images/Icons/collapsed.png";
                (item.FindControl("ExpandCollapse") as CustomImageButton).OnMouseOverImage = "Images/Icons/collapsed_hover.png";
                (item.FindControl("ExpandCollapse") as CustomImageButton).AlternateText = Utils.Languages.GetLabelFromCode("HomeExpandNotify", UIManager.UserManager.GetUserLanguage());
                (item.FindControl("ExpandCollapse") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("HomeExpandNotify", UIManager.UserManager.GetUserLanguage());

            }
            this.UpdatePanelRepListNotify.Update();
        }

        protected void IndexImgRemoveFilter_OnClick(object sender, EventArgs e)
        {
            try
            {
                NotificationManager.DeleteFiltersNotifications();
                NotificationManager.ApplyFilters();
                NotificationManager.ApplyFiltersNotifications();
                OrderListNotifyFiltered();
                RepListNotifyAll_Bind();
                this.IndexImgRemoveFilter.Enabled = false;
                this.UpPnlBAction.Update();
                this.UpdatePanelRepListNotify.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void IndexImgRemove_OnClick(object sender, EventArgs e)
        {
            try
            {
                if (ListNotifySelected != null && ListNotifySelected.Count() > 0)
                {
                    List<Notification> listNotify = new List<Notification>();
                    listNotify = (from n in NotificationManager.ListNotifyFiltered
                                  where ListNotifySelected.Contains(n.ID_NOTIFY)
                                  select n).ToList();
                    if (listNotify.Count > 0)
                    {
                        if ((from notification in listNotify where !string.IsNullOrEmpty(notification.NOTES) select notification).ToList().Count > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmRemoveNotificationsWithNotes', 'HiddenRemoveNotifications', '');", true);
                            return;
                        }
                    }

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmRemoveNotifications', 'HiddenRemoveNotifications', '');", true);
                    return;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void IndexImgSmista_OnClick(object sender, EventArgs e)
        {

        }

        protected void SelectAllNotify_CheckedChanged(object sender, EventArgs e)
        {
            ListNotifySelected = new List<string>();
            if (this.cbSelectAllNotify.Checked)
            {
                ListNotifySelected = (from n in NotificationManager.ListNotifyFiltered select n.ID_NOTIFY).ToList();
                foreach (RepeaterItem item in repListNotify.Items)
                {
                    (item.FindControl("IndexChkRemoveNotify") as CheckBox).Checked = true;
                }
                this.cbSelectAllNotify.ToolTip = Utils.Languages.GetLabelFromCode("ViewDetailNotifycbDeselectAllNotifyTooltip", UIManager.UserManager.GetUserLanguage());
            }
            else
            {
                ListNotifySelected = null;
                foreach (RepeaterItem item in repListNotify.Items)
                {
                    (item.FindControl("IndexChkRemoveNotify") as CheckBox).Checked = false;
                    this.cbSelectAllNotify.ToolTip = Utils.Languages.GetLabelFromCode("ViewDetailNotifycbSelectAllNotifyTooltip", UIManager.UserManager.GetUserLanguage());
                }
            }
            this.UpPnlBAction.Update();
            this.UpdatePanelRepListNotify.Update();
        }

        protected void RepListNotify_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string idNotify = (e.Item.FindControl("NotifyId") as HiddenField).Value;
            Notification notification = (from n in NotificationManager.ListNotifyFiltered where n.ID_NOTIFY.Equals(idNotify) select n).FirstOrDefault();
            switch (e.CommandName)
            {
                case "ViewDetailsDocument":
                case "viewLinkObject":
                    string domainObject = notification.DOMAINOBJECT;

                    #region navigation
                    List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                    Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                    actualPage.IdObject = notification.ID_NOTIFY;
                    actualPage.OriginalObjectId = notification.ID_NOTIFY;
                    actualPage.NumPage = this.SelectedPage.ToString();
                    actualPage.ListNotification = NotificationManager.ListNotifyFiltered;
                    actualPage.PageSize = PAGE_SIZE.ToString();

                    actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.HOME.ToString(), string.Empty);
                    actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.HOME.ToString(), true, this.Page);
                    actualPage.CodePage = Navigation.NavigationUtils.NamePage.HOME.ToString();

                    actualPage.Page = "INDEX.ASPX";
                    actualPage.DxTotalNumberElement = NotificationManager.ListNotifyFiltered.Count.ToString();
                    int pageCount = (int)Math.Round(((double)NotificationManager.ListNotifyFiltered.Count / (double)PAGE_SIZE) + 0.49);
                    actualPage.DxTotalPageNumber = pageCount.ToString();
                    actualPage.ViewResult = true;
                    actualPage.FromNotifyCenter = true;

                    int indexElement = ((e.Item.ItemIndex + 1)) + PAGE_SIZE * (this.SelectedPage - 1);
                    actualPage.DxPositionElement = indexElement.ToString();

                    navigationList.Add(actualPage);
                    Navigation.NavigationUtils.SetNavigationList(navigationList);
                    #endregion

                    if (domainObject.Equals(FiltersNotifications.DOCUMENT) || domainObject.Equals(FiltersNotifications.ATTACH))
                    {
                        if (CheckACL(notification.ID_OBJECT, notification.DOMAINOBJECT))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('RevocationAclIndex', 'warning', '','',null,null,'')", true);
                            return;
                        }
                        SchedaDocumento schedaDoc = UIManager.DocumentManager.getDocumentDetails(this.Page, notification.ID_OBJECT, notification.ID_OBJECT);
                        DocumentManager.setSelectedRecord(schedaDoc);


                        if (notification.READ_NOTIFICATION == NotificationManager.ReadNotification.NO_READ)
                        {
                            bool isNotEnabledSetDataVistaGrd = !string.IsNullOrEmpty(ConfigurationManager.AppSettings[WebConfigKeys.SET_DATA_VISTA_GRD.ToString()])
                                    && ConfigurationManager.AppSettings[WebConfigKeys.SET_DATA_VISTA_GRD.ToString()] == "1";
                            if (!NotificationManager.GetTypeEvent(notification.TYPE_EVENT).Equals(NotificationManager.EventType.TRASM) && isNotEnabledSetDataVistaGrd)
                            {
                                NotificationManager.CheckNotification(notification);
                            }
                            else
                            {
                                NotificationManager.ChangeStateReadNotification(notification, isNotEnabledSetDataVistaGrd);
                            }
                        }


                        Response.Redirect("Document/Document.aspx");
                        Response.End();
                        return;
                    }
                    else if (domainObject.Equals(FiltersNotifications.FOLDER))
                    {
                        if (CheckACL(notification.ID_OBJECT, notification.DOMAINOBJECT))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('RevocationAclIndex', 'warning', '','',null,null,'')", true);
                            return;
                        }
                        if (notification.READ_NOTIFICATION == NotificationManager.ReadNotification.NO_READ)
                        {
                            bool isNotEnabledSetDataVistaGrd = !string.IsNullOrEmpty(ConfigurationManager.AppSettings[WebConfigKeys.SET_DATA_VISTA_GRD.ToString()])
                                   && ConfigurationManager.AppSettings[WebConfigKeys.SET_DATA_VISTA_GRD.ToString()] == "1";
                            if (!NotificationManager.GetTypeEvent(notification.TYPE_EVENT).Equals(NotificationManager.EventType.TRASM) && isNotEnabledSetDataVistaGrd)
                            {
                                NotificationManager.CheckNotification(notification);
                            }
                            else
                            {
                                NotificationManager.ChangeStateReadNotification(notification, isNotEnabledSetDataVistaGrd);
                            }
                        }

                        Fascicolo fascicolo = UIManager.ProjectManager.getFascicoloById(notification.ID_OBJECT);
                        if (fascicolo != null)
                        {
                            fascicolo.template = ProfilerProjectManager.getTemplateFascDettagli(fascicolo.systemID);
                        }
                        ProjectManager.setProjectInSession(fascicolo);
                        //ProjectManager.setProjectInSession(
                        //    ProjectManager.GetFascicolo(ProjectManager.getInfoFascicoloDaFascicolo(
                        //    ProjectManager.getProjectInSession())));
                        Response.Redirect("~/Project/project.aspx");
                    }
                    break;

                case "RemoveNotify":
                    if (notification != null)
                    {
                        if (!string.IsNullOrEmpty(notification.NOTES))
                        {
                            NotificationToRemove = notification;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmRemoveNotificationWithNotes', 'HiddenRemoveNotification', '');", true);
                            return;
                        }
                        else
                        {
                            //EMANUELA 25/02/2015 : INSERITO MESSAGGIO DI CONFERMA PER LA RIMOZIONE DELLA NOTIFICA
                            NotificationToRemove = notification;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmRemoveNotification', 'HiddenRemoveNotification', '');", true);
                            return;

                            /*
                            if (NotificationManager.RemoveNotification(new Notification[] { notification }))
                            {
                                if (ListNotifySelected != null)
                                    ListNotifySelected.Remove(notification.ID_NOTIFY);
                                NotificationManager.ListNotifyFiltered.Remove(notification);
                                NotificationManager.ListAllNotify.Remove(notification);
                                if (notification.TYPE_NOTIFICATION == NotificationManager.NotificationType.OPERATIONAL)
                                {
                                    ChkNotifyOperational_Bind();
                                    this.UpdatePanelFilterOperational.Update();
                                }
                                else
                                {
                                    ChkNotifyInformation_Bind();
                                    this.UpdatePanelFilterInformation.Update();
                                }
                                LoadNumberNotifyInTheRole();
                                this.UpdatePanelNumberNotifyInTheRole.Update();
                                RepListNotifyAll_Bind();
                                this.UpdatePanelRepListNotify.Update();
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('ErrorRemoveNotification', 'error', '','');", true);
                                return;
                            }
                             * */
                        }
                    }
                    break;

                case "ViewDocument":
                    this.IsZoom = true;
                    string objectId = notification.ID_OBJECT;
                    if (CheckACL(objectId, notification.DOMAINOBJECT))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('RevocationAclIndex', 'warning', '','',null,null,'')", true);
                        return;
                    }
                    UIManager.DocumentManager.setSelectedRecord(UIManager.DocumentManager.getDocumentDetails(this.Page, objectId, objectId));
                    FileManager.setSelectedFile(UIManager.DocumentManager.getSelectedRecord().documenti[0]);
                    if (notification.READ_NOTIFICATION == NotificationManager.ReadNotification.NO_READ)
                    {
                        bool isNotEnabledSetDataVistaGrd = !string.IsNullOrEmpty(ConfigurationManager.AppSettings[WebConfigKeys.SET_DATA_VISTA_GRD.ToString()])
                                   && ConfigurationManager.AppSettings[WebConfigKeys.SET_DATA_VISTA_GRD.ToString()] == "1";
                        if (!NotificationManager.GetTypeEvent(notification.TYPE_EVENT).Equals(NotificationManager.EventType.TRASM) && isNotEnabledSetDataVistaGrd)
                        {
                            if (NotificationManager.CheckNotification(notification))
                            {
                                if (ListNotifySelected != null)
                                    ListNotifySelected.Remove(notification.ID_NOTIFY);
                                NotificationManager.ListNotifyFiltered.Remove(notification);
                                NotificationManager.ListAllNotify.Remove(notification);
                                UpdatePageAfterRemoveNotification = true;
                            }
                        }
                        else
                        {
                            if (NotificationManager.ChangeStateReadNotification(notification, isNotEnabledSetDataVistaGrd))
                            {
                                if (ListNotifySelected != null)
                                    ListNotifySelected.Remove(notification.ID_NOTIFY);
                                NotificationManager.ListNotifyFiltered.Remove(notification);
                                NotificationManager.ListAllNotify.Remove(notification);
                                UpdatePageAfterRemoveNotification = true;
                            }
                            else
                            {
                                int indexListFiltered = NotificationManager.ListNotifyFiltered.Select((item, i) => new { not = item, index = i }).First(item => item.not.ID_NOTIFY.Equals(idNotify)).index;
                                NotificationManager.ListNotifyFiltered[indexListFiltered].READ_NOTIFICATION = '1';
                                int indexListAll = NotificationManager.ListAllNotify.Select((item, i) => new { not = item, index = i }).First(item => item.not.ID_NOTIFY.Equals(idNotify)).index;
                                NotificationManager.ListAllNotify[indexListAll].READ_NOTIFICATION = '1';
                                (e.Item.FindControl("labelContainerTop") as HtmlGenericControl).Attributes["class"] = "";
                            }
                        }
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDocumentViewer", "ajaxModalPopupDocumentViewer();", true);
                    NttDataWA.Popup.DocumentViewer.OpenDocumentViewer = true;
                    break;

                case "ViewDetailsNotify":
                    if (notification.DOMAINOBJECT.Equals(FiltersNotifications.DOCUMENT) || notification.DOMAINOBJECT.Equals(FiltersNotifications.ATTACH))
                    {
                        //controllo ACL documento prima di aprire la popup di dettaglio notifica
                        if (CheckACL(notification.ID_OBJECT, notification.DOMAINOBJECT))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('RevocationAclIndex', 'warning', '','',null,null,'')", true);
                            return;
                        }
                        Notification = notification;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupViewDetailNotify", "ajaxModalPopupViewDetailNotify();", true);
                    }
                    else if (notification.DOMAINOBJECT.Equals(FiltersNotifications.FOLDER))
                    {
                        if (CheckACL(notification.ID_OBJECT, notification.DOMAINOBJECT))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('RevocationAclIndex', 'warning', '','',null,null,'')", true);
                            return;
                        }
                        ProjectManager.setProjectInSession(null);
                        Notification = notification;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupViewDetailNotify", "ajaxModalPopupViewDetailNotify();", true);
                    }
                    break;

                case "AddNote":
                    {
                        CustomImageButton button = e.Item.FindControl("IndexImgAdd") as CustomImageButton;
                        button.Style.Add("visibility", "hidden");
                        this.grid_rowindex.Value = e.Item.ItemIndex.ToString();
                        this.RepListNotify_ViewNotifyDetailNote();
                        (e.Item.FindControl("containerNoteNotify") as HtmlGenericControl).Attributes["style"] = "display:block";
                        if (UserManager.IsAuthorizedFunctions("HOME_NOTES"))
                        {
                            (e.Item.FindControl("containerNoteHomeSx") as HtmlGenericControl).Attributes["class"] = "containerNoteHomeSxNote";
                        }
                        string jsCode = "document.getElementById('txtNoteNotify').focus(); $(function () {$('.containerNoteNotify_" + e.Item.ItemIndex + "').show('slide',{direction: 'up' }, 100);})";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ViewNoteDetails", jsCode, true);
                        this.UpdatePanelRepListNotify.Update();
                        break;
                    }
                case "RemoveNotesNotify":
                    {
                        if (NotificationManager.UpdateNoteNotification(idNotify, string.Empty))
                        {
                            NotificationManager.ListAllNotify[NotificationManager.ListAllNotify.Select((item, i) => new { not = item, index = i }).First(item => item.not.ID_NOTIFY.Equals(idNotify)).index].NOTES = string.Empty;
                            NotificationManager.ListNotifyFiltered[NotificationManager.ListNotifyFiltered.Select((item, i) => new { not = item, index = i }).First(item => item.not.ID_NOTIFY.Equals(idNotify)).index].NOTES = string.Empty;
                            CustomImageButton button = e.Item.FindControl("IndexImgAdd") as CustomImageButton;
                            button.Style.Add("visibility", "visible");
                            if (UserManager.IsAuthorizedFunctions("HOME_NOTES"))
                            {
                                (e.Item.FindControl("containerNoteHomeSx") as HtmlGenericControl).Attributes.Remove("class");
                            }
                            string jsCode = "$(function () {$('.containerNoteNotify_" + e.Item.ItemIndex + "').hide('slide',{direction: 'up' }, 100);})";
                            (e.Item.FindControl("containerNoteNotify") as HtmlGenericControl).Attributes["style"] = "display:none";
                            (e.Item.FindControl("txtNoteNotify") as CustomTextArea).Text = string.Empty;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ViewNoteDetails", jsCode, true);
                            this.UpdatePanelRepListNotify.Update();
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('ErrorRemoveNoteNotifyIndex', 'error', '','',null,null,'')", true);
                            return;
                        }
                        break;
                    }
                case "SaveNotesNotify":
                    {
                        string note = (e.Item.FindControl("txtNoteNotify") as CustomTextArea).Text;
                        if (NotificationManager.UpdateNoteNotification(idNotify, note))
                        {
                            NotificationManager.ListAllNotify[NotificationManager.ListAllNotify.Select((item, i) => new { not = item, index = i }).First(item => item.not.ID_NOTIFY.Equals(idNotify)).index].NOTES = note;
                            NotificationManager.ListNotifyFiltered[NotificationManager.ListNotifyFiltered.Select((item, i) => new { not = item, index = i }).First(item => item.not.ID_NOTIFY.Equals(idNotify)).index].NOTES = note;
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('ErrorSaveNoteNotifyIndex', 'error', '','',null,null,'')", true);
                            return;
                        }
                        break;
                    }
                case "SignatureDetails":
                    if (CheckACL(notification.ID_OBJECT, notification.DOMAINOBJECT))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('RevocationAclIndex', 'warning', '','',null,null,'')", true);
                        return;
                    }
                    UIManager.DocumentManager.setSelectedRecord(UIManager.DocumentManager.getDocumentDetails(this.Page, notification.ID_OBJECT, notification.ID_OBJECT));
                    FileManager.setSelectedFile(UIManager.DocumentManager.getSelectedRecord().documenti[0]);
                    DocumentManager.removeSelectedNumberVersion();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDigitalSignDetails", "ajaxModalPopupDigitalSignDetails();", true);
                    break;
            }
        }

        /// <summary>
        /// Visualizza/Nasconde il pannello contenente i dettagli della notifica
        /// </summary>
        protected void RepListNotify_ViewNotifyDetail()
        {
            string itemIndex = this.grid_rowindex.Value;
            string jsCode = string.Empty;

            if ((this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("containerListNotifyBt") as HtmlGenericControl).Attributes["class"].Equals("containerListNotifyBt"))
            {
                jsCode = "$(function () {$('.containerNotifyDetails_" + itemIndex + "').show('slide',{direction: 'up' }, 100);});";
                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("containerNotifyDetails") as HtmlGenericControl).Attributes["style"] = "display:block";
                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("containerListNotifyBt") as HtmlGenericControl).Attributes["class"] = "containerListNotifyBtWithDetails";
                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).ImageUrl = "Images/Icons/expanded.png";
                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).ImageUrlDisabled = "Images/Icons/expanded_disabled.png";
                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).OnMouseOutImage = "Images/Icons/expanded.png";
                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).OnMouseOverImage = "Images/Icons/expanded_hover.png";
                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).AlternateText = Utils.Languages.GetLabelFromCode("HomeCollapseNotify", UIManager.UserManager.GetUserLanguage());
                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("HomeCollapseNotify", UIManager.UserManager.GetUserLanguage());
            }
            else
            {
                jsCode = "$(function () {$('.containerNotifyDetails_" + itemIndex + "').hide('slide',{direction: 'up' }, 100);});";
                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("containerListNotifyBt") as HtmlGenericControl).Attributes["class"] = "containerListNotifyBt";
                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("containerNotifyDetails") as HtmlGenericControl).Attributes["style"] = "display:none";


                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).ImageUrl = "Images/Icons/collapsed.png";
                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).ImageUrlDisabled = "Images/Icons/collapsed_disabled.png";
                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).OnMouseOutImage = "Images/Icons/collapsed.png";
                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).OnMouseOverImage = "Images/Icons/collapsed_hover.png";
                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).AlternateText = Utils.Languages.GetLabelFromCode("HomeExpandNotify", UIManager.UserManager.GetUserLanguage());
                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("HomeExpandNotify", UIManager.UserManager.GetUserLanguage());
            }
            string idNotify = (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("NotifyId") as HiddenField).Value;
            Notification notification = (from n in NotificationManager.ListNotifyFiltered where n.ID_NOTIFY.Equals(idNotify) select n).FirstOrDefault();
            if (notification.READ_NOTIFICATION == NotificationManager.ReadNotification.NO_READ && notification.DOMAINOBJECT.Equals(NotificationManager.ListDomainObject.JOB))
            {
                NotificationManager.ChangeStateReadNotification(notification, false);
                int indexListFilteres = NotificationManager.ListNotifyFiltered.Select((item, i) => new { not = item, index = i }).First(item => item.not.ID_NOTIFY.Equals(idNotify)).index;
                NotificationManager.ListNotifyFiltered[indexListFilteres].READ_NOTIFICATION = '1';
                int indexListAll = NotificationManager.ListAllNotify.Select((item, i) => new { not = item, index = i }).First(item => item.not.ID_NOTIFY.Equals(idNotify)).index;
                NotificationManager.ListAllNotify[indexListAll].READ_NOTIFICATION = '1';
                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("labelContainerTop") as HtmlGenericControl).Attributes["class"] = "";
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ViewDetailsTrasmission", jsCode, true);
            this.UpdatePanelRepListNotify.Update();
        }

        /// <summary>
        /// Visualizza/Nasconde il pannello contenente i dettagli della notifica
        /// </summary>
        protected void RepListNotify_ViewNotifyDetailNote()
        {
            string itemIndex = this.grid_rowindex.Value;
            string jsCode = string.Empty;


            jsCode = "$(function () {$('.containerNotifyDetails_" + itemIndex + "').show('slide',{direction: 'up' }, 100);});";
            (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("containerNotifyDetails") as HtmlGenericControl).Attributes["style"] = "display:block";
            (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("containerListNotifyBt") as HtmlGenericControl).Attributes["class"] = "containerListNotifyBtWithDetails";
            (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).ImageUrl = "Images/Icons/expanded.png";
            (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).ImageUrlDisabled = "Images/Icons/expanded_disabled.png";
            (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).OnMouseOutImage = "Images/Icons/expanded.png";
            (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).OnMouseOverImage = "Images/Icons/expanded_hover.png";
            (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).AlternateText = Utils.Languages.GetLabelFromCode("HomeCollapseNotify", UIManager.UserManager.GetUserLanguage());
            (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("ExpandCollapse") as CustomImageButton).ToolTip = Utils.Languages.GetLabelFromCode("HomeCollapseNotify", UIManager.UserManager.GetUserLanguage());

            string idNotify = (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("NotifyId") as HiddenField).Value;
            Notification notification = (from n in NotificationManager.ListNotifyFiltered where n.ID_NOTIFY.Equals(idNotify) select n).FirstOrDefault();
            if (notification.READ_NOTIFICATION == NotificationManager.ReadNotification.NO_READ && notification.DOMAINOBJECT.Equals(NotificationManager.ListDomainObject.JOB))
            {
                NotificationManager.ChangeStateReadNotification(notification, false);
                int indexListFilteres = NotificationManager.ListNotifyFiltered.Select((item, i) => new { not = item, index = i }).First(item => item.not.ID_NOTIFY.Equals(idNotify)).index;
                NotificationManager.ListNotifyFiltered[indexListFilteres].READ_NOTIFICATION = '1';
                int indexListAll = NotificationManager.ListAllNotify.Select((item, i) => new { not = item, index = i }).First(item => item.not.ID_NOTIFY.Equals(idNotify)).index;
                NotificationManager.ListAllNotify[indexListAll].READ_NOTIFICATION = '1';
                (this.repListNotify.Items[Convert.ToInt32(itemIndex)].FindControl("labelContainerTop") as HtmlGenericControl).Attributes["class"] = "";
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ViewDetailsTrasmission", jsCode, true);
            this.UpdatePanelRepListNotify.Update();
        }

        #endregion

        #endregion

        #region Utils

        private void ResetFilterTypeObject()
        {
            this.IndexChkDoc.Checked = true;
            this.IndexChkProj.Checked = true;
            this.IndexChkOther.Checked = true;
        }

        private void ResetFilterTypeNotify()
        {
            this.IndexChkOperational.Checked = true;
            foreach (ListItem item in this.IndexCblOperational.Items)
            {
                item.Selected = true;
            }
            this.IndexChkInformation.Checked = true;
            foreach (ListItem item in this.IndexCblInformation.Items)
            {
                item.Selected = true;
            }
            this.UpdatePanelFilterInformation.Update();
            this.UpdatePanelFilterOperational.Update();
        }

        private void ResetDdlOrderBy()
        {
            this.ddlOrderBy.SelectedValue = ORDER_BY_DATE_DESCENDING;
        }

        private void ResetChkReadAndNotRead()
        {
            this.IndexChkNotRead.Checked = true;
            this.IndexChkRead.Checked = true;
        }

        private void UpdateFilterOperational()
        {
            List<string> list = new List<string>();
            list = (from i in this.IndexCblOperational.Items.Cast<ListItem>() where i.Selected select i.Value).ToList();
            FiltersNotifications.GetFiltersOperationalEmpty();
            FiltersNotifications.FILTERS_OPERATIONAL = list;
            FiltersNotifications.SaveFiltersOperational();
        }

        private void UpdateFilterInformation()
        {
            List<string> list = new List<string>();
            list = (from i in this.IndexCblInformation.Items.Cast<ListItem>() where i.Selected select i.Value).ToList();
            FiltersNotifications.GetFiltersInformationEmpty();
            FiltersNotifications.FILTERS_INFORMATION = list;
            FiltersNotifications.SaveFiltersInformation();
        }

        private void UpdateFilterObject()
        {
            FiltersNotifications.FILTERS_DOMAIN_OBJECT = FiltersNotifications.GetFiltersDomainObjectEmpty();
            FiltersNotifications.FILTERS_DOMAIN_OBJECT.Add(FiltersNotifications.DOCUMENT, this.IndexChkDoc.Checked);
            FiltersNotifications.FILTERS_DOMAIN_OBJECT.Add(FiltersNotifications.FOLDER, this.IndexChkProj.Checked);
            FiltersNotifications.FILTERS_DOMAIN_OBJECT.Add(FiltersNotifications.OTHER, this.IndexChkOther.Checked);
            FiltersNotifications.SaveFiltersDomainObject(); ;
        }

        private void UpdateFilterReadNotification()
        {
            FiltersNotifications.GetFiltersReadNoReadEmpty();
            FiltersNotifications.FILTERS_READ_NOREAD.Add(FiltersNotifications.READ, this.IndexChkRead.Checked);
            FiltersNotifications.FILTERS_READ_NOREAD.Add(FiltersNotifications.NOREAD, this.IndexChkNotRead.Checked);
            FiltersNotifications.SaveFiltersDomainReadNoRead();
        }

        private void OrderListNotifyFiltered()
        {
            string orderBy = this.ddlOrderBy.SelectedValue;
            switch (orderBy)
            {
                case ORDER_BY_DATE_DESCENDING:
                    NotificationManager.ListNotifyFiltered = (from n in NotificationManager.ListNotifyFiltered orderby n.DTA_EVENT descending select n).ToList<Notification>();
                    break;
                case ORDER_BY_DATE_ASCENDING:
                    NotificationManager.ListNotifyFiltered = (from n in NotificationManager.ListNotifyFiltered orderby n.DTA_EVENT ascending select n).ToList<Notification>();
                    break;
                case ORDER_BY_TYPE_EVENT:
                    NotificationManager.ListNotifyFiltered = (from n in NotificationManager.ListNotifyFiltered orderby n.TEXT_SORTING ascending select n).ToList<Notification>();
                    break;
                case ORDER_BY_PRODUTTORE:
                    NotificationManager.ListNotifyFiltered = (from n in NotificationManager.ListNotifyFiltered orderby n.PRODUCER ascending select n).ToList<Notification>();
                    break;
                case ORDER_BY_ID_OBJECT:
                    NotificationManager.ListNotifyFiltered = (from n in NotificationManager.ListNotifyFiltered orderby n.ID_OBJECT descending, n.DTA_EVENT descending select n).ToList<Notification>();
                    break;
            }
        }

        /// <summary>
        /// Controllo ACL documento
        /// </summary>
        /// <param name="idObject"></param>
        /// <returns></returns>
        private bool CheckACL(string idObject, string typeObject)
        {
            bool result = false;
            if (typeObject.Equals(NotificationManager.ListDomainObject.DOCUMENT))
            {
                //DocumentManager.setSelectedRecord(DocumentManager.getDocumentDetailsNoSecurity(this.Page, idObject, idObject));
                SchedaDocumento schedaDoc = new SchedaDocumento();
                schedaDoc.systemId = idObject;
                DocumentManager.setSelectedRecord(schedaDoc);
                result = DocumentManager.CheckRevocationAcl();
                DocumentManager.setSelectedRecord(null);
            }
            else if (typeObject.Equals(NotificationManager.ListDomainObject.FOLDER))
            {
                //ProjectManager.setProjectInSession(ProjectManager.getFascicoloById(idObject));
                Fascicolo proj = new Fascicolo();
                proj.systemID = idObject;
                ProjectManager.setProjectInSession(proj);
                result = ProjectManager.CheckRevocationAcl();
                if (result)
                    ProjectManager.setProjectInSession(null);
            }
            return result;
        }

        /// <summary>
        /// Aggiorna la lista contenente gli id delle notifiche selezionate nella griglia
        /// </summary>
        private void UpdateListSelectNotify()
        {
            string itemIndex = this.grid_rowindex.Value;
            RepeaterItem item = this.repListNotify.Items[Convert.ToInt32(itemIndex)];
            string idNotify = (item.FindControl("NotifyId") as HiddenField).Value;
            if ((item.FindControl("IndexChkRemoveNotify") as CheckBox).Checked)
            {
                if (ListNotifySelected == null)
                {
                    ListNotifySelected = new List<string>();
                }
                ListNotifySelected.Add(idNotify);
            }
            else
            {
                ListNotifySelected.Remove(idNotify);
                if (this.cbSelectAllNotify.Checked)
                {
                    this.cbSelectAllNotify.Checked = false;
                    this.cbSelectAllNotify.ToolTip = Utils.Languages.GetLabelFromCode("ViewDetailNotifycbSelectAllNotifyTooltip", UIManager.UserManager.GetUserLanguage());
                    this.UpPnlBAction.Update();
                }
            }
        }

        /// <summary>
        /// Rimuove tutte le notifiche della griglia per cui è checked la corrispondente checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveSelectedNotifications()
        {
            try
            {
                List<Notification> listNotify = new List<Notification>();
                listNotify = (from n in NotificationManager.ListNotifyFiltered
                              where ListNotifySelected.Contains(n.ID_NOTIFY)
                              select n).ToList();
                if (listNotify.Count > 0)
                {
                    if (NotificationManager.RemoveNotification(listNotify.ToArray<Notification>()))
                    {
                        ListNotifySelected = null;
                        this.cbSelectAllNotify.Checked = false;
                        this.UpPnlBAction.Update();
                        NotificationManager.ListAllNotify = (from n in NotificationManager.ListAllNotify.Except(listNotify) select n).ToList();
                        NotificationManager.ListNotifyFiltered = (from n in NotificationManager.ListNotifyFiltered.Except(listNotify) select n).ToList();
                        ChkNotifyOperational_Bind();
                        ChkNotifyInformation_Bind();
                        this.UpdatePanelFilterSx.Update();
                        LoadNumberNotifyInTheRole();
                        this.UpdatePanelNumberNotifyInTheRole.Update();
                        RepListNotifyAll_Bind();
                        this.UpdatePanelRepListNotify.Update();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('ErrorRemoveNotifications', 'error', '','');", true);
                        return;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }

        }

        /// <summary>
        ///Verifica se, per il tipo trasmissione in input, è abilitato il bottone per visualizzare il dettaglio della notifica
        /// </summary>
        /// <param name="eventTypeExtended"></param>
        /// <returns></returns>
        private bool EnableButtonViewDetailTrasmissione(string eventTypeExtended)
        {
            if (!NotificationManager.GetTypeEvent(eventTypeExtended).Equals(NotificationManager.EventType.TRASM) &&
                 System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.SET_DATA_VISTA_GRD.ToString()].Equals("2"))
            {
                return true;
            }
            else
                if (!NotificationManager.GetTypeEvent(eventTypeExtended).Equals(NotificationManager.EventType.TRASM))
                {
                    return false;
                }
                else
                    return true;
        }

        #endregion

        #region Deleghe

        private void VerificaDeleghe()
        {
            int numDelega = DelegheManager.VerificaDelega();

            if (numDelega > 0 && Session["ESERCITADELEGA"] == null)
            {
                if (!IsPostBack)
                {
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "setTimeout(function(){parent.fra_main.ajaxConfirmModal('ConfirmMandateExercise', 'mandate_exercise', '" + utils.FormatJs(this.GetLabel("BaseMasterDelegate")) + "');}, 1500);", true);
                    string label = Utils.Languages.GetLabelFromCode("BaseMasterDelegate", UIManager.UserManager.GetUserLanguage());
                    this.js_code.Text += "ajaxConfirmModal('ConfirmMandateExercise', 'mandate_exercise', '" + utils.FormatJs(label) + "');";
                    this.UpPnlJs.Update();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.fra_main.ajaxConfirmModal('ConfirmMandateExercise', 'mandate_exercise', '');", true);
                }
            }
        }

        #endregion

        #region Connettore Socket 

        private void ApriConnettoreSocket()
        {

            if (!String.IsNullOrEmpty(UserManager.getComponentType(Request.UserAgent)) && Constans.TYPE_SOCKET.Equals(UserManager.getComponentType(Request.UserAgent)))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "script", "$(function () { CheckConnector(); });", true);
            }

        }


        #endregion
    }
}
