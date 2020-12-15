using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;


namespace NttDataWA.Popup
{
    public partial class NotificationNoticeDays : System.Web.UI.Page
    {
        #region Property

        /// <summary>
        /// Giorni di preavviso
        /// </summary>
        public static string NoticeDays
        {
            get
            {
                if (HttpContext.Current.Session["NoticeDays"] != null)
                {
                    return HttpContext.Current.Session["NoticeDays"] as string;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Standard Method

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeLanguage();
                InitializePage();
            }
            RefreshScript();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            //Mev FI - Modifica label pulsante Annulla/Chiusura popup 
            this.NotificationNoticeDaysBtnCancel.Text = (UIManager.UserManager.IsAuthorizedFunctions(DBKeys.FE_CHECK_DISABLE_CONTROL_REMOVE_NOTIFY.ToString())) ?
                Utils.Languages.GetLabelFromCode("NotificationNoticeDaysBtnClose", language) :
                Utils.Languages.GetLabelFromCode("NotificationNoticeDaysBtnCancel", language);
            this.NotificationNoticeDaysBtnRemove.Text = Utils.Languages.GetLabelFromCode("NotificationNoticeDaysBtnRemove", language);
            this.NotificationNoticeDaysLt.Text = Utils.Languages.GetLabelFromCode("NotificationNoticeDaysLt", language);
            this.ltlDateNoticeDays.Text = Utils.Languages.GetLabelFromCode("NotificationNoticeDaysLbl", language);
            this.cbRemovePendingTransmission.Text = Utils.Languages.GetLabelFromCode("NotificationNoticeCbRemovePending", language);
        }

        private void InitializePage()
        {
            if (!string.IsNullOrEmpty(NoticeDays))
            {
                DateTime date = System.DateTime.Now.AddDays(-Convert.ToInt32(NoticeDays));
                int numberNotifyOverNoticeDays = (from notification in NotificationManager.ListAllNotify
                                                  where Utils.utils.verificaIntervalloDate(Utils.utils.formatDataDocsPa(date), notification.DTA_NOTIFY.ToString())
                                                  select notification).Count();
                this.NotificationNoticeDaysLt.Text = NotificationNoticeDaysLt.Text.Replace("@@", numberNotifyOverNoticeDays.ToString()).Replace("**", NoticeDays.ToString());
                this.txt_dateNoticeDays.Text = Utils.utils.formatDataDocsPa(date);
            }

            //Mev FI - Visualizzazione controllo pulsante rimozione notifiche
            if (UIManager.UserManager.IsAuthorizedFunctions(DBKeys.FE_CHECK_DISABLE_CONTROL_REMOVE_NOTIFY.ToString()))
            {
                //rimuove la visibilità del pulsante
                pnlVisibleRemoveNotifyButtom.Visible = false;
                //rimuove la visibilità della check
                pnlVisibleRemoveNotifyCheckBox.Visible = false;
                //rimuove la visibilità del messsage
                pnlVisibleRemoveNotifyMessage.Visible = false;
                // modifica titolo pulsante annulla/chiusura

            }
            else
            {
                //attiva la visibilità del pulsante
                pnlVisibleRemoveNotifyButtom.Visible = true;
                //attiva la visibilità della check
                pnlVisibleRemoveNotifyCheckBox.Visible = true;
                //attiva la visibilità del messsage
                pnlVisibleRemoveNotifyMessage.Visible = true;
                // modifica titolo pulsante annulla/chiusura


            }
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        #endregion

        #region Event hendler

        protected void NotificationNoticeDaysBtnRemove_Click(object sender, EventArgs e)
        {
            bool result = false;
            bool noNotifications = false;
            string returnValue = string.Empty;
            try
            {
                if (cbRemovePendingTransmission.Checked)
                {
                    result = NotificationManager.RemoveNotificationOfTransmissionNoWF(this.txt_dateNoticeDays.Text);
                    returnValue = "loadNotify";
                }
                else
                {
                    List<Notification> listNotificationOverNoticeDays = (from notification in NotificationManager.ListAllNotify
                                                                         where Utils.utils.verificaIntervalloDate(this.txt_dateNoticeDays.Text, notification.DTA_NOTIFY.ToString())
                                                                         select notification).ToList();
                    if (NotificationManager.RemoveNotification(listNotificationOverNoticeDays.ToArray()))
                    {
                        result = true;
                        returnValue="up";
                        NotificationManager.ListAllNotify = (from n in NotificationManager.ListAllNotify.Except(listNotificationOverNoticeDays) select n).ToList();
                        NotificationManager.ListNotifyFiltered = (from n in NotificationManager.ListNotifyFiltered.Except(listNotificationOverNoticeDays) select n).ToList();
                    }
                    if (listNotificationOverNoticeDays != null && listNotificationOverNoticeDays.Count == 0)
                        noNotifications = true;
                }
                if (result)
                {
                    ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('NotificationNoticeDays','"+ returnValue + "');", true);
                    return;
                }
                else
                {
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.fra_main.ajaxDialogModal('ErrorRemoveNotifications', 'error', '','');", true);
                    string script = string.Empty;
                    if (!noNotifications)
                        script = "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('ErrorRemoveNotifications', 'error', '',''); } else { parent.ajaxDialogModal('ErrorRemoveNotifications', 'error', '',''); }";
                    else
                        script = "if (parent.fra_main) { parent.fra_main.ajaxDialogModal('WarningRemoveNotifications', 'warning', '',''); } else { parent.ajaxDialogModal('WarningRemoveNotifications', 'warning', '',''); }";
                    //string script = "parent.ajaxDialogModal('ErrorRemoveNotifications', 'error', '','');";
                    ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "ajaxDialogModal", script, true);
                    return;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void NotificationNoticeDaysBtnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('NotificationNoticeDays','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #region Utils

        /// <summary>
        /// Seleziona gli specializedObject di notifiche a trasmissioni effettuate pendenti
        /// </summary>
        /// <param name="listNotification"></param>
        /// <returns></returns>
        private List<string> GetNotificationTransmissionPending(List<Notification> listNotification)
        {

            string[] listIdSpecializedObject = (from notification in listNotification
                                                where NotificationManager.GetTypeEvent(notification.TYPE_EVENT).Equals(NotificationManager.EventType.TRASM)
                                                select notification.ID_SPECIALIZED_OBJECT).ToArray();
            List<string> listIdSpecializedObjectPending = new List<string>();
            if (listIdSpecializedObject != null && listIdSpecializedObject.Count() > 0)
            {
                listIdSpecializedObjectPending = NotificationManager.GetIdTrasmPendingByNotificationsFilters(listIdSpecializedObject, UserManager.GetUserInSession().idPeople);
            }
            return listIdSpecializedObjectPending;
        }

        #endregion
    }
}