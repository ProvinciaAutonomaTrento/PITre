using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using System.Collections;

namespace NttDataWA.UIManager
{
    public class NotificationManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        #region Const

        private const string TRASM_DOC_ = "trasm_doc_";
        private const string TRASM_FOLDER_ = "trasm_folder_";
        private const string ALL_TYPE_DOCUMENT = "ALL";
        private const string LBL_EXPIRATION = "lblExpiration";
 
        #endregion

        #region Build label

        /// <summary>
        /// Returns a label
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string getLabel(string field)
        {
            string label = string.Empty;
            if (field.Contains(TagField.LABEL))
            {
                label = field.Substring(field.IndexOf(TagField.LABEL) + TagField.LABEL.Length, (field.IndexOf(TagField.LABEL_C) - (field.IndexOf(TagField.LABEL) + TagField.LABEL.Length)));
                if (label.Contains(TagField.COLOR_RED))
                    label = label.Replace(TagField.COLOR_RED, "").Replace(TagField.COLOR_RED_C, "");

                field = field.Replace(label, Utils.Languages.GetLabelFromCode(label, UIManager.UserManager.GetUserLanguage()));
                field = field.Replace(TagField.LABEL, "").Replace(TagField.LABEL_C, "");
            }
            if (field.Contains(TagField.COLOR_RED))
                field = field.Replace(TagField.COLOR_RED, "<font color='#FF0000'>").Replace(TagField.COLOR_RED_C, "</font>");
            else if (field.Contains(TagField.COLOR_RED_STRIKE))
                field = field.Replace(TagField.COLOR_RED_STRIKE, "<font color='#FF0000'><strike>").Replace(TagField.COLOR_RED_STRIKE_C, "</strike></font>");
            return field;
        }

        /// <summary>
        /// returns the labels for the fields of notification
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string getLabelNotifyField(Items item)
        {
            string result = string.Empty;
            string label = string.Empty;
            //if (!string.IsNullOrEmpty(item.ITEM1))
            //{
            //    result = NotificationManager.getLabel(item.ITEM1);
            //}
            //if (!string.IsNullOrEmpty(item.ITEM2))
            //{
            //    label = NotificationManager.getLabel(item.ITEM2);
            //    if (!string.IsNullOrEmpty(label))
            //        result = string.IsNullOrEmpty(result) ? label : result + " - " + label;
            //}

            if (!string.IsNullOrEmpty(item.ITEM3))
            {
                label = NotificationManager.getLabel(item.ITEM3);
                if (!string.IsNullOrEmpty(label))
                    if (label.Length > 50)
                    {
                        label = label.Substring(0, 50) + "...";
                    }
                result = " - " + label;
            }
            if (!string.IsNullOrEmpty(item.ITEM4))
            {
                label = NotificationManager.getLabel(item.ITEM4);
                if (!string.IsNullOrEmpty(label))
                    result = string.IsNullOrEmpty(result) ? label : result + " - " + label;
            }
            return result;
        }

         public static string getLabelNotifyFieldLink(Items item)
        {
            string result = string.Empty;
            string label = string.Empty;
            if (!string.IsNullOrEmpty(item.ITEM1))
            {
                result = NotificationManager.getLabel(item.ITEM1);
            }
            if (!string.IsNullOrEmpty(item.ITEM2))
            {
                label = NotificationManager.getLabel(item.ITEM2);
                if (!string.IsNullOrEmpty(label))
                    result = string.IsNullOrEmpty(result) ? label : result + " - " + label;
            }

            return result;
        }

        /// <summary>
        /// returns the label of the event type that triggered the notification
        /// </summary>
        /// <param name="eventTypeExtended"></param>
        /// <returns></returns>
        public static string getLabelTypeEvent(string eventTypeExtended)
        {
            string result = string.Empty;
            if (GetTypeEvent(eventTypeExtended).Equals(EventType.TRASM))
            {
                eventTypeExtended = eventTypeExtended.ToLower() + (eventTypeExtended.ToLower().Contains(TRASM_DOC_) ? " D" : " F");
                result = (eventTypeExtended.Replace(TRASM_DOC_, "T: ").Replace(TRASM_FOLDER_, "T: ")).Replace("_", " ");
                //result = Utils.Languages.GetLabelFromCode("IndexTrasmDocFolder", UIManager.UserManager.GetUserLanguage()) + eventTypeExtended;
            }
            else
            {
                result = Utils.Languages.GetLabelFromCode(eventTypeExtended, UIManager.UserManager.GetUserLanguage());
            }
            return result;
        }

        /// <summary>
        /// Returns the contracted form of the event type
        /// </summary>
        /// <param name="eventTypeExtended">event type in extended form</param>
        /// <returns></returns>
        public static string GetTypeEvent(string eventTypeExtended)
        {
            if (eventTypeExtended.Contains(EventType.ACCEPT_TRASM + "_"))
                return EventType.ACCEPT_TRASM;
            else if (eventTypeExtended.Contains(EventType.REJECT_TRASM + "_"))
                return EventType.REJECT_TRASM;
            else if (eventTypeExtended.Contains(EventType.CHECK_TRASM + "_"))
                return EventType.CHECK_TRASM;
            else if (eventTypeExtended.Contains(EventType.TRASM + "_"))
                return EventType.TRASM;
            else
                return eventTypeExtended;
        }  

        #endregion

        #region Services Backend

        public static List<Notification> ReadNotifications(string idPeople, string idGroup)
        {
            try
            {
               return docsPaWS.ReadNotifications(idPeople, idGroup).ToList<Notification>();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool RemoveNotification(Notification [] notifications)
        {
            bool result = false;
            try
            {
                result = docsPaWS.RemoveNotifications(notifications);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }

        /// <summary>
        /// Restituisce il numero di eventi notificati ad altri ruoli dell'utente in sessione
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGroup"></param>
        /// <returns></returns>
        public static int GetNumberNotifyOtherRoles(string idPeople, string idGroup)
        {
            try
            {
                return docsPaWS.GetNumberNotifyOtherRoles(idPeople, idGroup);     
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return 0;
            }
        }
    
        /// <summary>
        /// Aggiorna le notifiche legate al system id del documento/fascicolo in base alle modifiche subite da quest'ultimo
        /// </summary>
        /// <param name="typeOperation">Elenco del tipo di operazioni di modifica effettuate sull'oggetto del dominio(documento/fascicolo)</param>
        /// <param name="idObject">Id del documento/fascicolo</param>
        /// <param name="domainObject">Specifica se si tratta di un documento o fascicolo</param>
        public static void ModifyNotification(InfoUtente infoUser, TypeOperation[] typeOperation, string idObject, string domainObject)
        {
            try
            {
                docsPaWS.ModifyNotifications(infoUser, typeOperation, idObject, domainObject);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public delegate void ModifyNotificationDelegate(InfoUtente infoUser, TypeOperation[] typeOperation, string idObject, string domainObject);

        /// <summary>
        /// Cambia lo stato della notifica da non letta a letta; in caso in cui l'amministrazione non ha abilitato
        /// il tasto visto, la notifica viene rimossa
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGroup"></param>
        /// <returns></returns>
        public static bool ChangeStateReadNotification(Notification notify, bool isNotEnabledSetDataVistaGrd)
        {
            bool result = false;
            try
            {
                result = docsPaWS.ChangeStateReadNotification(notify, isNotEnabledSetDataVistaGrd);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }

        /// <summary>
        /// Restituisce l'id dei documenti che rispettano le condizioni impostate nel filtro in input
        /// </summary>
        /// <param name="idObject"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<string> GetDocumentByNotificationsFilters(string [] idObject, NotificationsFilters filter)
        {
            try
            {
                return docsPaWS.GetDocumentByNotificationsFilters(idObject, filter).ToList<String>();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }


        /// <summary>
        /// Resituisce l'id delle trasmissioni singole con workflow in attesa di accettazione/rifiuto
        /// </summary>
        /// <param name="idSpecializedObject"></param>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static List<string> GetIdTrasmPendingByNotificationsFilters(string[] idSpecializedObject, string idPeople)
        {
            try
            {
                return docsPaWS.GetIdTransmPendingByNotificationsFilters(idSpecializedObject, idPeople).ToList<String>();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Prende il numero di giorni oltre i quali la notifica viene segnalata come "vecchia"
        /// </summary>
        /// <returns></returns>
        public static string GetNoticeDaysNotification()
        {
            string noticeDays = string.Empty;
            try
            {
                noticeDays = docsPaWS.GetNoticeDaysNotification(RoleManager.GetRoleInSession().idAmministrazione);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return noticeDays;
        }

        /// <summary>
        /// Rimuove le notifiche di trasmissioni che non necessitano accettazione
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool RemoveNotificationOfTransmissionNoWF(string date)
        {
            bool result = false;
            try
            {
                result = docsPaWS.RemoveNotificationOfTransmissionNoWF(UserManager.GetUserInSession().idPeople, RoleManager.GetRoleInSession().idGruppo, date);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }

        /// <summary>
        /// Salva/Svuota la nota nella notifica
        /// </summary>
        /// <param name="idNotify"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public static bool UpdateNoteNotification(string idNotify, string note)
        {
            bool result = false;
            try
            {
                result = docsPaWS.UpdateNoteNotification(idNotify, note);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return result;
        }

        #endregion

        #region structs

        public struct ListDomainObject
        {
            public const string DOCUMENT = "DOCUMENTO";
            public const string FOLDER = "FASCICOLO";
            public const string JOB = "JOB";
        }

        /// <summary>
        /// Tipo notifica(informativa, operativa)
        /// </summary>
        public struct NotificationType
        {
            public const char OPERATIONAL = 'O';
            public const char INFORMATION = 'I';
        }

        public struct Multiplicity
        {
            public const string ONE = "ONE";
            public const string ALL = "ALL";
        }

        public struct EventType
        {
            public const string TRASM = "TRASM";
            public const string ACCEPT_TRASM = "ACCEPT_TRASM";
            public const string REJECT_TRASM = "REJECT_TRASM";
            public const string CHECK_TRASM = "CHECK_TRASM";
        }

        public struct TagField
        {
            public const string LABEL = "<label>";
            public const string LABEL_C = "</label>";
            public const string COLOR_RED = "<colorRed>";
            public const string COLOR_RED_C = "</colorRed>";
            public const string COLOR_RED_STRIKE = "<colorRedStrike>";
            public const string COLOR_RED_STRIKE_C = "</colorRedStrike>";
            public const string LINE = "<line>";
            public const string LINE_C = "</line>";
        }

        public struct ReadNotification
        {
            public const char READ = '1';
            public const char NO_READ = '0';
        }

        public struct Color
        {
            public const string RED = "RED";
            public const string BLUE = "BLUE";
        }

        #endregion

        #region property

        /// <summary>
        /// E' l'array di oggetti di tipo notifica contente tutte le notifiche
        /// </summary>
        public static List<Notification> ListAllNotify
        {
            get
            {
                if (HttpContext.Current.Session["ListAllNotify"] != null)
                    return HttpContext.Current.Session["ListAllNotify"] as List<Notification>;
                else return new List<Notification>();
            }
            set
            {
                HttpContext.Current.Session["ListAllNotify"] = value;
            }
        }

        /// <summary>
        /// E' l'array di oggetti di tipo notifica contente le notifiche filtrate
        /// </summary>
        public static List<Notification> ListNotifyFiltered
        {
            get
            {
                if (HttpContext.Current.Session["ListNotifyFiltered"] != null)
                    return HttpContext.Current.Session["ListNotifyFiltered"] as List<Notification>;
                else return new List<Notification>();
            }
            set
            {
                HttpContext.Current.Session["ListNotifyFiltered"] = value;
            }
        }

        public static NotificationsFilters Filters
        {
            get
            {
                if (HttpContext.Current.Session["Filters"] != null)
                {
                    return HttpContext.Current.Session["Filters"] as NotificationsFilters;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                HttpContext.Current.Session["Filters"] = value;
            }
        }

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
            set
            {
                HttpContext.Current.Session["NoticeDays"] = value;
            }
        }

        #endregion

        #region Delete property

        /// <summary>
        /// Elimina i filtri applicati sulle Notifiche mediante la popup
        /// </summary>
        public static void DeleteFiltersNotifications()
        {
            HttpContext.Current.Session.Remove("Filters");
        }

        public static void DeleteNoticeDays()
        {
            HttpContext.Current.Session.Remove("NoticeDays");
        }

        #endregion

        #region Filters

        /// <summary>
        /// Apply filters to the notification list, and returns the filtered list
        /// </summary>
        /// <returns></returns>
        public static void ApplyFilters()
        {
            List<Notification> listFiltered = new List<Notification>();
            ListNotifyFiltered = new List<Notification>();
            try
            {

                if (ListAllNotify.Count > 0)
                {
                    //*********************************************filter for read / unread*****************************************

                    bool read = false, noread = false;
                    foreach (KeyValuePair<string, bool> keyval in FiltersNotifications.GetFiltersReadNoRead())
                    {
                        if (keyval.Key.Equals(FiltersNotifications.READ))
                        {
                            read = keyval.Value;
                        }
                        else if (keyval.Key.Equals(FiltersNotifications.NOREAD))
                        {
                            noread = keyval.Value;
                        }
                    }
                    // I take the complete list of notifications
                    if (read && noread)
                    {
                        ListNotifyFiltered.AddRange(ListAllNotify);
                    }
                    // I take the list of notifications read
                    else if (read)
                    { 
                        ListNotifyFiltered = (from n in ListAllNotify where n.READ_NOTIFICATION == '1' select n).ToList();
                    }
                    // I take the list of unread notifications
                    else if (noread)
                    {
                        ListNotifyFiltered = (from n in ListAllNotify where n.READ_NOTIFICATION == '0' select n).ToList();
                    }

                    //************************************** end filter for read / unread *****************************************


                    //************************************** Filter by object type domain *****************************************

                    if (ListNotifyFiltered.Count == 0) // if you can not find items in the list of filtered notifications go out of the method
                        return;

                    bool document = false, folder = false, other = false;
                    foreach (KeyValuePair<string, bool> keyval in FiltersNotifications.GetFiltersDomainObject())
                    {
                        if (keyval.Key.Equals(FiltersNotifications.DOCUMENT))
                        {
                            document = keyval.Value;
                        }
                        else if (keyval.Key.Equals(FiltersNotifications.FOLDER))
                        {
                            folder = keyval.Value;
                        }
                        else if (keyval.Key.Equals(FiltersNotifications.OTHER))
                        {
                            other = keyval.Value;
                        }
                    }

                    if (document)
                    {
                        if ((from n in ListNotifyFiltered where n.DOMAINOBJECT.Equals(ListDomainObject.DOCUMENT) select n).Count() > 0)
                        {
                            listFiltered.AddRange((from n in ListNotifyFiltered where n.DOMAINOBJECT.Equals(ListDomainObject.DOCUMENT) select n).ToList());
                        }
                    }
                    if (folder)
                    {
                        if((from n in ListNotifyFiltered where n.DOMAINOBJECT.Equals(ListDomainObject.FOLDER) select n).Count() > 0)
                        {
                            listFiltered.AddRange((from n in ListNotifyFiltered where n.DOMAINOBJECT.Equals(ListDomainObject.FOLDER) select n).ToList());
                        }
                    }
                    if (other)
                    {
                        if ((from n in ListNotifyFiltered
                             where (!n.DOMAINOBJECT.Equals(ListDomainObject.DOCUMENT)
                                 && !n.DOMAINOBJECT.Equals(ListDomainObject.FOLDER))
                             select n).Count() > 0)
                        {
                            listFiltered.AddRange((from n in ListNotifyFiltered
                                                   where (!n.DOMAINOBJECT.Equals(ListDomainObject.DOCUMENT)
                                                       && !n.DOMAINOBJECT.Equals(ListDomainObject.FOLDER))
                                                   select n).ToList());
                        }
                    }

                    ListNotifyFiltered.Clear();
                    ListNotifyFiltered.AddRange(listFiltered);

                    //************************************** end filter by object type domain *************************************


                    //************************************** filter by operating notifications *************************************

                    if (ListNotifyFiltered.Count == 0) // if you can not find items in the list of filtered notifications go out of the method
                        return;
                    listFiltered.Clear(); // I empty the list of support
                    if (FiltersNotifications.GetFiltersOperational().Count > 0)
                    {
                        foreach (string typeEvent in FiltersNotifications.GetFiltersOperational())
                        {
                            if ((from n in ListNotifyFiltered
                                 where n.TYPE_EVENT.Equals(typeEvent) &&
                                     n.TYPE_NOTIFICATION == NotificationType.OPERATIONAL
                                 select n).Count() > 0)
                            {
                                listFiltered.AddRange((from n in ListNotifyFiltered
                                                       where n.TYPE_EVENT.Equals(typeEvent) &&
                                                           n.TYPE_NOTIFICATION == NotificationType.OPERATIONAL
                                                       select n).ToList());
                            }
                        }
                    }

                    //************************************** end filter by operating notifications *************************************


                    //*************************************** filter by informational notifications ************************************
                    
                    if (FiltersNotifications.GetFiltersInformation().Count > 0)
                    {
                        foreach (string typeEvent in FiltersNotifications.GetFiltersInformation())
                        {
                            if ((from n in ListNotifyFiltered
                                 where n.TYPE_EVENT.Equals(typeEvent) &&
                                     n.TYPE_NOTIFICATION == NotificationType.INFORMATION
                                 select n).Count() > 0)
                            {
                                listFiltered.AddRange((from n in ListNotifyFiltered
                                                       where n.TYPE_EVENT.Equals(typeEvent) &&
                                                           n.TYPE_NOTIFICATION == NotificationType.INFORMATION
                                                       select n).ToList());
                            }
                        }
                    }

                    //*************************************** end filter by informational notifications ************************************

                    ListNotifyFiltered = listFiltered; //final list of notifications after applying all filters
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Applica i filtri impostati sulle notifiche mediante la popup
        /// </summary>
        public static void ApplyFiltersNotifications()
        {
            try
            {
                if (Filters != null)
                {
                    List<Notification> listFiltered = new List<Notification>();
                    //************************************** Filter by Notes*****************************************

                    if (!string.IsNullOrEmpty(Filters.NOTES.Trim()))
                    {
                        listFiltered = (from notification in ListNotifyFiltered where notification.NOTES.ToLower().Contains(Filters.NOTES.ToLower()) select notification).ToList();
                        ListNotifyFiltered.Clear();
                        ListNotifyFiltered.AddRange(listFiltered);
                    }

                    //************************************** end filter by Notes*****************************************

                    //************************************** Filter by Type document,file Acquired and type file acquired*****************************************
                    bool filterTypeDocument = !string.IsNullOrEmpty(Filters.TYPE_DOCUMENT);
                    bool filterFileAcquired = Filters.DOCUMENT_ACQUIRED || Filters.DOCUMENT_SIGNED || Filters.DOCUMENT_UNSIGNED;
                    if (filterTypeDocument || filterFileAcquired || !string.IsNullOrEmpty(Filters.TYPE_FILE_ACQUIRED))
                    {
                        string[] listIdObject =  (from notification in ListNotifyFiltered
                                                  where notification.DOMAINOBJECT.Equals(NotificationManager.ListDomainObject.DOCUMENT)
                                                  select notification.ID_OBJECT).ToArray();

                        List<string> listIdObjectFiltered = new List<string>();
                        if (listIdObject != null && listIdObject.Count() > 0)
                        {
                            listIdObjectFiltered = NotificationManager.GetDocumentByNotificationsFilters(listIdObject, Filters);
                        }
                        listFiltered = (from notification in ListNotifyFiltered
                                        where !notification.DOMAINOBJECT.Equals(NotificationManager.ListDomainObject.DOCUMENT) ||
                                        !string.IsNullOrEmpty((from idObject in listIdObjectFiltered
                                                                where notification.ID_OBJECT.Equals(idObject)
                                                                select idObject).FirstOrDefault())
                                        select notification).ToList();
                        ListNotifyFiltered.Clear();
                        ListNotifyFiltered = listFiltered;
                    }
                    //*************************************** end filter by Type document,file Acquired and type file acquired************************************

                    //************************************** Filter by Transmission Pending*****************************************

                    if (Filters.PENDING)
                    {
                        string [] listIdSpecializedObject = (from notification in ListNotifyFiltered
                                                             where GetTypeEvent(notification.TYPE_EVENT).Equals(EventType.TRASM)
                                                             select notification.ID_SPECIALIZED_OBJECT).ToArray();
                        List<string> listIdSpecializedObjectFiltered = new List<string>();
                        if (listIdSpecializedObject != null && listIdSpecializedObject.Count() > 0)
                        {
                            listIdSpecializedObjectFiltered = NotificationManager.GetIdTrasmPendingByNotificationsFilters(listIdSpecializedObject, UserManager.GetUserInSession().idPeople);
                        }
                        listFiltered = (from notification in ListNotifyFiltered
                                        where !string.IsNullOrEmpty((from idSpecialized in listIdSpecializedObjectFiltered
                                                               where notification.ID_SPECIALIZED_OBJECT.Equals(idSpecialized)
                                                               select idSpecialized).FirstOrDefault())
                                        select notification).ToList();
                        ListNotifyFiltered.Clear();
                        ListNotifyFiltered = listFiltered;
                    }

                    //*************************************** end filter by Transmission Pending************************************
                    //************************************** Filter by Date Event*****************************************
                    if (!string.IsNullOrEmpty(Filters.DATE_EVENT_FROM) && !string.IsNullOrEmpty(Filters.DATE_EVENT_TO))
                    {
                        listFiltered = (from notification in ListNotifyFiltered
                                        where Utils.utils.verificaIntervalloDateSenzaOra(Utils.utils.formatDataDocsPa(notification.DTA_EVENT), Filters.DATE_EVENT_FROM) &&
                                               Utils.utils.verificaIntervalloDateSenzaOra(Filters.DATE_EVENT_TO, Utils.utils.formatDataDocsPa(notification.DTA_EVENT))
                                        select notification).ToList();
                        ListNotifyFiltered.Clear();
                        ListNotifyFiltered.AddRange(listFiltered);
                    }
                    else
                        if (!string.IsNullOrEmpty(Filters.DATE_EVENT_FROM))
                        {
                            listFiltered = (from notification in ListNotifyFiltered
                                            where Utils.utils.formatDataDocsPa(notification.DTA_EVENT).Equals(Filters.DATE_EVENT_FROM)
                                            select notification).ToList();
                            ListNotifyFiltered.Clear();
                            ListNotifyFiltered.AddRange(listFiltered);
                        }
                    //*************************************** end filter by Date Event ************************************


                    //************************************** Filter by Date Expiration*****************************************
                    if (!string.IsNullOrEmpty(Filters.DATE_EXPIRE_FROM) && !string.IsNullOrEmpty(Filters.DATE_EXPIRE_TO))
                    {
                        listFiltered = (from notification in ListNotifyFiltered
                                        where Utils.utils.verificaIntervalloDateSenzaOra(GetDateExpiration(notification.ITEM_SPECIALIZED), Filters.DATE_EXPIRE_FROM) &&
                                               Utils.utils.verificaIntervalloDateSenzaOra(Filters.DATE_EXPIRE_TO, GetDateExpiration(notification.ITEM_SPECIALIZED))
                                        select notification).ToList();
                        ListNotifyFiltered.Clear();
                        ListNotifyFiltered.AddRange(listFiltered);
                    }
                    else
                        if (!string.IsNullOrEmpty(Filters.DATE_EXPIRE_FROM))
                        {
                            listFiltered = (from notification in ListNotifyFiltered
                                            where GetDateExpiration(notification.ITEM_SPECIALIZED).Equals(Filters.DATE_EXPIRE_FROM)
                                            select notification).ToList();
                            ListNotifyFiltered.Clear();
                            ListNotifyFiltered.AddRange(listFiltered);
                        }
                    //*************************************** end filter by Date Expiration************************************

                    //************************************** Filter by Authors *****************************************
                    if (!string.IsNullOrEmpty(Filters.AUTHOR_DESCRIPTION))
                    {
                        listFiltered = (from notification in ListNotifyFiltered where notification.PRODUCER.Contains(Filters.AUTHOR_DESCRIPTION) select notification).ToList();
                        ListNotifyFiltered.Clear();
                        ListNotifyFiltered.AddRange(listFiltered);
                    }
                    //*************************************** end filter by Authors ************************************

                    //************************************** Filter by Object *****************************************
                    if (!string.IsNullOrEmpty(Filters.OBJECT))
                    {
                        listFiltered = (from notification in ListNotifyFiltered where notification.ITEMS.ITEM3.ToUpper().Contains(Filters.OBJECT.ToUpper()) select notification).ToList();
                        ListNotifyFiltered.Clear();
                        ListNotifyFiltered.AddRange(listFiltered);
                    }
                    //*************************************** end filter by Object ************************************
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

        }

        /// <summary>
        /// Prende la data scadenza all'interno della label
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        private static string GetDateExpiration(string label)
        {
            string dateExpiration = string.Empty;

            if (!string.IsNullOrEmpty(label) && label.Contains(LBL_EXPIRATION))
            {
                dateExpiration = label.Substring(label.IndexOf(LBL_EXPIRATION) + (LBL_EXPIRATION.Length + TagField.LABEL_C.Length), 10);
            }
            return dateExpiration;
        }

        #endregion

        #region Actions details notification

        /// <summary>
        /// Check Notification
        /// </summary>
        /// <returns></returns>
        public static bool CheckNotification(Notification notification)
        {
            bool result = false;
            try
            {
                if (NotificationManager.GetTypeEvent(notification.TYPE_EVENT).Equals(EventType.TRASM))
                {
                    Trasmissione trasm = (Trasmissione)HttpContext.Current.Session["Transmission"];
                    if (trasm != null)
                    { 
                        result = notification.DOMAINOBJECT.Equals(NotificationManager.ListDomainObject.DOCUMENT) ? 
                            TrasmManager.setdatavistaSP_TV(UserManager.GetInfoUser(), trasm.infoDocumento.docNumber, "D",
                            trasm.infoDocumento.idRegistro, trasm.systemId) : TrasmManager.setdatavistaSP_TV(UserManager.GetInfoUser(), trasm.infoFascicolo.idFascicolo, "F",
                            trasm.infoFascicolo.idRegistro, trasm.systemId);
                    }
                }
                else
                { 
                    result = docsPaWS.CheckNotification(notification, UserManager.GetInfoUser());
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return result;
            }

            return result;
        }

        /// <summary>
        /// Rimuove la notifica dal centro notifiche applicando la molteplicità uno tutti
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public static bool RemoveNotification(Notification notification)
        {
            bool result = false;
            try
            {
                result = docsPaWS.CheckNotification(notification, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return result;
            }

            return result;
        }

        #endregion

        #region Notice days notification

        public static bool NotificationOverNoticeDays()
        {
            bool result = false;
            string noticeDays = GetNoticeDaysNotification();
            if (!string.IsNullOrEmpty(noticeDays))
            {
                DateTime date = System.DateTime.Now.AddDays(-Convert.ToInt32(noticeDays));
                if (((from notification in ListAllNotify
                      where utils.verificaIntervalloDate(Utils.utils.formatDataDocsPa(date), notification.DTA_NOTIFY.ToString())
                      select notification).FirstOrDefault()) != null)
                {
                    result = true;
                    NoticeDays = noticeDays;
                }
            }
            return result;
        }

        #endregion
    }


    /// <summary>
    /// This class implements the logic of filters to apply to notifications
    /// </summary>
    public static class FiltersNotifications
    {
        #region const

        public const string DOCUMENT = "DOCUMENTO";
        public const string FOLDER = "FASCICOLO";
        public const string ATTACH = "ALLEGATO";
        public const string OTHER = "OTHER";
        public const string READ = "READ";
        public const string NOREAD = "NOREAD";

        #endregion

        #region filter

        public static Dictionary<string, bool> FILTERS_READ_NOREAD;

        public static Dictionary<string, bool> FILTERS_DOMAIN_OBJECT;

        public static List<string> FILTERS_OPERATIONAL;

        public static List<string> FILTERS_INFORMATION;

        #endregion

        #region session management
        
        //public static void SaveFilters()
        //{
        //    if (FILTERS_READ_NOREAD == null || FILTERS_READ_NOREAD.Count == 0)
        //    {
        //        FILTERS_READ_NOREAD = new Dictionary<string, bool>();
        //        FILTERS_READ_NOREAD.Add(READ, true);
        //        FILTERS_READ_NOREAD.Add(NOREAD, true);
        //    }

        //    if (FILTERS_DOMAIN_OBJECT == null || FILTERS_DOMAIN_OBJECT.Count == 0)
        //    {
        //        FILTERS_DOMAIN_OBJECT = new Dictionary<string, bool>();
        //        FILTERS_DOMAIN_OBJECT.Add(DOCUMENT, true);
        //        FILTERS_DOMAIN_OBJECT.Add(FOLDER, true);
        //        FILTERS_DOMAIN_OBJECT.Add(OTHER, true);
        //    }
        //    HttpContext.Current.Session.Add("FILTERS_READ_NOREAD", FILTERS_READ_NOREAD);
        //    HttpContext.Current.Session.Add("FILTERS_DOMAIN_OBJECT", FILTERS_DOMAIN_OBJECT);
        //    HttpContext.Current.Session.Add("FILTERS_OPERATIONAL", FILTERS_OPERATIONAL);
        //    HttpContext.Current.Session.Add("FILTERS_INFORMATION", FILTERS_INFORMATION);
        //}

        public static void SaveFiltersOperational()
        {
            HttpContext.Current.Session.Add("FILTERS_OPERATIONAL", FILTERS_OPERATIONAL);
        }

        public static void SaveFiltersInformation()
        {
            HttpContext.Current.Session.Add("FILTERS_INFORMATION", FILTERS_INFORMATION);
        }

        public static void SaveFiltersDomainObject()
        {
            HttpContext.Current.Session.Add("FILTERS_DOMAIN_OBJECT", FILTERS_DOMAIN_OBJECT);
        }

        public static void SaveFiltersDomainReadNoRead()
        {
            HttpContext.Current.Session.Add("FILTERS_READ_NOREAD", FILTERS_READ_NOREAD);
        }

        public static void DeleteFilters()
        {
            HttpContext.Current.Session.Remove("FILTERS_READ_NOREAD");
            HttpContext.Current.Session.Remove("FILTERS_DOMAIN_OBJECT");
            HttpContext.Current.Session.Remove("FILTERS_OPERATIONAL");
            HttpContext.Current.Session.Remove("FILTERS_INFORMATION");
        }

        public static Dictionary<string, bool> GetFiltersDomainObject()
        {
            if (HttpContext.Current.Session["FILTERS_DOMAIN_OBJECT"] != null &&
                (HttpContext.Current.Session["FILTERS_DOMAIN_OBJECT"] as Dictionary<string, bool>).Count > 0)
            {
                return HttpContext.Current.Session["FILTERS_DOMAIN_OBJECT"] as Dictionary<string, bool>;
            }
            else
            {
                return new Dictionary<string, bool>();
            }
        }

        public static Dictionary<string, bool> GetFiltersReadNoRead()
        {
            if (HttpContext.Current.Session["FILTERS_READ_NOREAD"] != null)
            {
                return HttpContext.Current.Session["FILTERS_READ_NOREAD"] as Dictionary<string, bool>;
            }
            else
            {
                return new Dictionary<string, bool>();
            }
        }

        public static List<string> GetFiltersOperational()
        {
            if (HttpContext.Current.Session["FILTERS_OPERATIONAL"] != null)
            {
                return HttpContext.Current.Session["FILTERS_OPERATIONAL"] as List<string>;
            }
            else
            {
                return new List<string>();
            }
        }

        public static List<string> GetFiltersInformation()
        {
            if (HttpContext.Current.Session["FILTERS_INFORMATION"] != null)
            {
                return HttpContext.Current.Session["FILTERS_INFORMATION"] as List<string>;
            }
            else
            {
                return new List<string>();
            }
        }

        public static Dictionary<string, bool> GetFiltersDomainObjectEmpty()
        {
            FILTERS_DOMAIN_OBJECT = new Dictionary<string, bool>();
            return FILTERS_DOMAIN_OBJECT;
        }

        public static Dictionary<string, bool> GetFiltersReadNoReadEmpty()
        {
            FILTERS_READ_NOREAD = new Dictionary<string, bool>();
            return FILTERS_READ_NOREAD;
        }
        public static List<string> GetFiltersOperationalEmpty()
        {
            FILTERS_OPERATIONAL = new List<string>();
            return FILTERS_OPERATIONAL;
        }
        public static List<string> GetFiltersInformationEmpty()
        {
            FILTERS_INFORMATION = new List<string>();
            return FILTERS_INFORMATION;
        }
        #endregion
    
    }

    }

