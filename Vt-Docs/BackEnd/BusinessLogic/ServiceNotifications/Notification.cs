using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using DocsPaVO.Notification;
using DocsPaVO.utente;

namespace BusinessLogic.ServiceNotifications
{
    public class Notification
    {
        private static ILog logger = LogManager.GetLogger(typeof(Notification));

        /// <summary>
        /// Return the list of notifications that are directed to:
        /// 1. the couple user role.
        /// 2. user
        /// </summary>
        /// <returns></returns>
        public static List<DocsPaVO.Notification.Notification> ReadNotifications(string idPeople, string idGroup)
        {
            List<DocsPaVO.Notification.Notification> listNotifications = new List<DocsPaVO.Notification.Notification>();
            try
            {
                DocsPaDB.Query_DocsPAWS.NotificationDB notificationDB = new DocsPaDB.Query_DocsPAWS.NotificationDB();
                listNotifications = notificationDB.ReadNotifications(idPeople, idGroup);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.ServiceNotifications.Notification  - metodo: ReadNotifications ", e);
            }
            return listNotifications;
        }

        public static List<DocsPaVO.Notification.Notification> ReadNotificationsMobile(string idPeople, string idGroup, int requestedPage, int pageSize, out int totalRecordCount)
        {
            totalRecordCount = 0;
            List<DocsPaVO.Notification.Notification> listNotifications = new List<DocsPaVO.Notification.Notification>();
            try
            {
                DocsPaDB.Query_DocsPAWS.NotificationDB notificationDB = new DocsPaDB.Query_DocsPAWS.NotificationDB();
                listNotifications = notificationDB.ReadNotificationsMobile(idPeople, idGroup, requestedPage, pageSize, out totalRecordCount);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.ServiceNotifications.Notification  - metodo: ReadNotifications ", e);
            }
            return listNotifications;
        }

        /// <summary>
        /// Return the number of notifications that are direct to:
        /// 1. the couple user-roles different by role in input.
        /// 2. user
        /// </summary>
        /// <returns></returns>
        public static int GetNumberNotifyOtherRoles(string idPeople, string idGroup)
        {
            int count = 0;
            try
            {
                DocsPaDB.Query_DocsPAWS.NotificationDB notificationDB = new DocsPaDB.Query_DocsPAWS.NotificationDB();
                count = notificationDB.GetNumberNotificationOtherRoles(idPeople, idGroup);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.ServiceNotification.Notification  - metodo: GetNumberNotifyOtherRoles ", e);
            }
            return count;
        }

        /// <summary>
        /// removes the notification with idNotification selected for delete from front end
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public static bool RemoveNotifications(List<DocsPaVO.Notification.Notification> listNotifications)
        {
            bool res = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.NotificationDB notificationDB = new DocsPaDB.Query_DocsPAWS.NotificationDB();
                res = notificationDB.RemoveNotifications(listNotifications);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.Notification  - metodo: RemoveNotifications ", e);
            }
            return res;
        }

        /// <summary>
        /// Change the notification status from unread to read
        /// </summary>
        /// <returns></returns>
        public static bool ChangeStateReadNotification(DocsPaVO.Notification.Notification notify, bool isNotEnabledSetDataVistaGrd)
        {
            bool res = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.NotificationDB notificationDB = new DocsPaDB.Query_DocsPAWS.NotificationDB();
                res = notificationDB.ChangeStateReadNotification(notify, isNotEnabledSetDataVistaGrd);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.ServiceNotifications.Notification  - metodo: ChangeStateReadNotification ", e);
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool CheckNotification(DocsPaVO.Notification.Notification notification)
        {
            bool res = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.NotificationDB notificationDB = new DocsPaDB.Query_DocsPAWS.NotificationDB();
                if (notification.MULTIPLICITY.Equals(DocsPaVO.Notification.Multiplicity.ONE))
                {
                    res = notificationDB.CheckNotification(notification.ID_EVENT, string.Empty, notification.ID_GROUP);
                }
                else
                {
                    res = notificationDB.CheckNotification(notification.ID_EVENT, notification.ID_PEOPLE, notification.ID_GROUP);
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.ServiceNotifications.Notification  - metodo: CheckNotification ", e);
            }
            return res;
        }

        public static List<string> GetDocumentByNotificationsFilters(string[] idObject, DocsPaVO.Notification.NotificationsFilters filter)
        {
            List<string> listIdObject = new List<string>();
            try
            {
                DocsPaDB.Query_DocsPAWS.NotificationDB notificationDB = new DocsPaDB.Query_DocsPAWS.NotificationDB();
                listIdObject = notificationDB.GetDocumentByNotificationsFilters(idObject, filter);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.ServiceNotifications.Notification  - metodo: GetDocumentByNotificationsFilters ", e);
            }
            return listIdObject;
        }

        /// <summary>
        /// Prende l'id delle trasmissioni singole con workflow in attesa di accettazione/rifiuto
        /// </summary>
        /// <param name="idSpecializedObject"></param>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static List<string> GetIdTrasmPendingByNotificationsFilters(string[] idSpecializedObject, string idPeople)
        {
            List<string> listIdSpecializedObject = new List<string>();
            try
            {
                DocsPaDB.Query_DocsPAWS.NotificationDB notificationDB = new DocsPaDB.Query_DocsPAWS.NotificationDB();
                listIdSpecializedObject = notificationDB.GetIdTrasmPendingByNotificationsFilters(idSpecializedObject, idPeople);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.ServiceNotifications.Notification  - metodo: GetDocumentByNotificationsFilters ", e);
            }
            return listIdSpecializedObject;
        }

        public static string GetNoticeDaysNotification(string idAmm)
        {
            string noticeDays = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.NotificationDB notificationDB = new DocsPaDB.Query_DocsPAWS.NotificationDB();
                noticeDays = notificationDB.GetNoticeDaysNotification(idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.ServiceNotifications.Notification  - metodo: GetDocumentByNotificationsFilters ", e);
            }
            return noticeDays;
        }

        /// <summary>
        /// Rimuove le notifiche di trasmissioni che non necessitano accettazione antecedenti la data specificata
        /// </summary>
        /// <param name="idObject"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool RemoveNotificationOfTransmissionNoWF(string idPeople, string idGroup, string date)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.NotificationDB notificationDB = new DocsPaDB.Query_DocsPAWS.NotificationDB();
                result = notificationDB.RemoveNotificationOfTransmissionNoWF(idPeople, idGroup, date);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.ServiceNotifications.Notification  - metodo: RemoveNotificationOfTransmissionNoWF ", e);
            }
            return result;
        }

        /// <summary>
        /// Salva/Svuota la nota nella notifica
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGroup"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool UpdateNoteNotification(string idNotify, string note)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.NotificationDB notificationDB = new DocsPaDB.Query_DocsPAWS.NotificationDB();
                result = notificationDB.UpdateNoteNotification(idNotify, note);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.ServiceNotifications.Notification  - metodo: RemoveNoteNotification ", e);
            }
            return result;
        }

        #region Modify notification

        /// <summary>
        /// Aggiorna le notifiche legate all'system id del documento/fascicolo in base alle modifiche subite da quest'ultimo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="typeOperation"></param>
        /// <param name="idObject"></param>
        /// <param name="domainObject"></param>
        /// <returns></returns>
        public static bool ModifyNotifications(InfoUtente infoUtente, TypeOperation[] typeOperation, string idObject, string domainObject)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.NotificationDB notificationDB = new DocsPaDB.Query_DocsPAWS.NotificationDB();
                List<DocsPaVO.Notification.Notification> notificationList = notificationDB.GetNotificationsByIdObject(idObject, domainObject);
                if (notificationList != null && notificationList.Count > 0)
                {

                    DocsPaVO.documento.SchedaDocumento schedaDoc = null;
                    DocsPaVO.fascicolazione.Fascicolo project = null;
                    if (notificationList[0].DOMAINOBJECT.Equals(ListDomainObject.DOCUMENT))
                    {
                        schedaDoc = Documenti.DocManager.getDettaglioPerNotificaAllegati(infoUtente, notificationList[0].ID_OBJECT, notificationList[0].ID_OBJECT);
                    }
                    else if (notificationList[0].DOMAINOBJECT.Equals(ListDomainObject.FOLDER))
                    {
                        project = Fascicoli.FascicoloManager.getFascicoloById(notificationList[0].ID_OBJECT, infoUtente);
                    }


                    DocsPaVO.Notification.Notification newNotification = ExecuteChangeItems(infoUtente, typeOperation, notificationList[0], schedaDoc, project);
                    
                    foreach (DocsPaVO.Notification.Notification notification in notificationList)
                    {
                        //Tutte le notifiche relative ad un documento o fascicolo hanno i valori dei campi item uguali
                        notification.ITEMS = newNotification.ITEMS;

                        //Le notifiche relative ad un documento o fascicolo posso avere ITEM_SPECIALIZED diverso(esempio note individuali o generali)
                        notification.ITEM_SPECIALIZED = ExecuteChangeSpecializedItem(infoUtente, typeOperation, notification, schedaDoc, project).ITEM_SPECIALIZED;
                    }
                    result = notificationDB.UpdateNotifications(notificationList);
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.ServiceNotifications.Notification  - metodo: ModifyNotification ", e);
            }
            return result;
        }

        /// <summary>
        /// Aggiorna i campi, che sono stati modificati, sull'oggetto notifica in base all'elenco del tipo di operazioni di modifica in input
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="typeOperation"></param>
        /// <param name="oldNotification"></param>
        /// <returns>Restituisce la notifica con i campi variati aggiornati</returns>
        private static DocsPaVO.Notification.Notification ExecuteChangeItems(InfoUtente infoUtente, TypeOperation[] typeOperation,
            DocsPaVO.Notification.Notification oldNotification, DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.fascicolazione.Fascicolo project)
        {
            String dataAnnullamento = String.Empty;
            DocsPaVO.Notification.Notification newNotification = oldNotification;
            try
            {
                if (schedaDoc != null)
                {
                  
                    foreach (TypeOperation operation in typeOperation)
                    {
                        switch (operation)
                        {
                            case TypeOperation.CHANGE_OBJECT:
                                newNotification.ITEMS.ITEM3 = schedaDoc.oggetto.descrizione;
                                break;
                            case TypeOperation.CHANGE_TYPE_DOC:
                                string segnaturaRepertorio = Documenti.DocManager.GetSegnaturaRepertorio(schedaDoc.systemId, infoUtente.idAmministrazione, false, out dataAnnullamento);
                                if (!string.IsNullOrEmpty(segnaturaRepertorio))
                                {
                                    newNotification.ITEMS.ITEM4 = TagItem.LABEL + "lblRepertorio" + TagItem.CLOSE_LABEL +
                                        TagItem.COLORRED + segnaturaRepertorio + TagItem.CLOSE_COLORRED;
                                }
                                break;
                            case TypeOperation.RECORD_PREDISPOSED:
                                newNotification.ITEMS.ITEM1 = schedaDoc.systemId;
                                newNotification.ITEMS.ITEM2 = TagItem.COLORRED + schedaDoc.protocollo.segnatura +
                                    TagItem.CLOSE_COLORRED + GetLabelTypeProto(schedaDoc.tipoProto);
                                break;
                            case TypeOperation.CHANGE_TYPE_PROTO:
                                newNotification.ITEMS.ITEM1 = schedaDoc.systemId + GetLabelTypeProto(schedaDoc.tipoProto);
                                break;
                            case TypeOperation.ABORT_RECORD:
                                newNotification.ITEMS.ITEM2= TagItem.COLORRED_STRIKE+ schedaDoc.protocollo.segnatura +
                                    TagItem.CLOSE_COLORRED_STRIKE + GetLabelTypeProto(schedaDoc.tipoProto);
                                break;
                            case TypeOperation.ABORT_COUNTER_REPERTOIRE:
                                newNotification.ITEMS.ITEM4 = oldNotification.ITEMS.ITEM4.Replace(TagItem.COLORRED,TagItem.COLORRED_STRIKE).
                                    Replace(TagItem.CLOSE_COLORRED, TagItem.CLOSE_COLORRED_STRIKE);
                                break;
                            }
                    }
                }
                else if (project != null)
                {
                   
                    foreach (TypeOperation operation in typeOperation)
                    {
                        switch (operation)
                        {
                            case TypeOperation.CHANGE_OBJECT:
                                newNotification.ITEMS.ITEM3 = project.descrizione;
                                break;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Info("Errore in BusinessLogic.ServiceNotifications.Notification  - metodo: ExecuteChangeNotifications", exc);
                throw exc;
            }
            return newNotification;
        }

        private static DocsPaVO.Notification.Notification ExecuteChangeSpecializedItem(InfoUtente infoUtente, TypeOperation[] typeOperation,
           DocsPaVO.Notification.Notification oldNotification, DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.fascicolazione.Fascicolo project)
        {
            String dataAnnullamento = String.Empty;
            DocsPaVO.Notification.Notification newNotification = oldNotification;
            try
            {
                if (schedaDoc != null)
                {
                    foreach (TypeOperation operation in typeOperation)
                    {
                        switch (operation)
                        {
                            case TypeOperation.CHANGE_OBJECT:
                                newNotification.ITEM_SPECIALIZED = UpdateSpecializeItem(oldNotification.ITEM_SPECIALIZED, schedaDoc.oggetto.descrizione, "lblObjectDescription");
                                break;
                            case TypeOperation.CHANGE_TYPE_DOC:
                                newNotification.ITEM_SPECIALIZED = UpdateSpecializeItem(oldNotification.ITEM_SPECIALIZED, schedaDoc.tipologiaAtto.descrizione, "lblDocType");
                                break;
                            case TypeOperation.CHANGE_SENDER:
                                string mittente = string.Empty;
                                switch (schedaDoc.tipoProto)
                                {
                                    case "P":
                                        mittente = (schedaDoc.protocollo as DocsPaVO.documento.ProtocolloUscita).mittente.descrizione;
                                        break;
                                    case "A":
                                        mittente = (schedaDoc.protocollo as DocsPaVO.documento.ProtocolloEntrata).mittente.descrizione;
                                        break;
                                    case "I":
                                        mittente = (schedaDoc.protocollo as DocsPaVO.documento.ProtocolloInterno).mittente.descrizione;
                                        break;
                                }
                                newNotification.ITEM_SPECIALIZED = UpdateSpecializeItem(oldNotification.ITEM_SPECIALIZED, mittente, "lblSender");
                                break;
                        }
                    }
                }
                else if (project != null)
                {
                    foreach (TypeOperation operation in typeOperation)
                    {
                        switch (operation)
                        {
                            case TypeOperation.CHANGE_OBJECT:
                                newNotification.ITEM_SPECIALIZED = UpdateSpecializeItem(oldNotification.ITEM_SPECIALIZED, project.descrizione, "lblObjectDescription");
                                break;
                            case TypeOperation.CHANGE_TYPE_PROJ:
                                DocsPaVO.ProfilazioneDinamica.Templates templete = ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascDettagli(project.systemID);
                                if (templete != null && !string.IsNullOrEmpty(templete.DESCRIZIONE))
                                    newNotification.ITEM_SPECIALIZED = UpdateSpecializeItem(oldNotification.ITEM_SPECIALIZED, templete.DESCRIZIONE, "lblFascType");
                                break;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Info("Errore in BusinessLogic.ServiceNotifications.Notification  - metodo: ExecuteChangeNotifications", exc);
                throw exc;
            }
            return newNotification;
        }

        /// <summary>
        /// Costruisce la label per il tipo del protocollo da inserire in ITEM2 dell'oggetto ITEM in Notification
        /// </summary>
        /// <param name="typeProto"></param>
        /// <returns></returns>
        private static string GetLabelTypeProto(string typeProto)
        {
            string labelTypeProto = string.Empty;
            switch (typeProto)
            {
                case TypeProtocol.ARRIVO:
                    labelTypeProto = " (" + TagItem.LABEL + TypeProtocol.LABEL_ARRIVO + TagItem.CLOSE_LABEL + ")";
                    break;
                case TypeProtocol.PARTENZA:
                    labelTypeProto = " (" + TagItem.LABEL + TypeProtocol.LABEL_PARTENZA + TagItem.CLOSE_LABEL + ")";
                    break;
                case TypeProtocol.INTERNO:
                    labelTypeProto = " (" + TagItem.LABEL + TypeProtocol.LABEL_INTERNO + TagItem.CLOSE_LABEL + ")";
                    break;
                case TypeProtocol.GRIGIO:
                    labelTypeProto = " (" + TagItem.LABEL + TypeProtocol.LABEL_GRIGIO + TagItem.CLOSE_LABEL + ")";
                    break;
                case TypeProtocol.STAMPAREG:
                    labelTypeProto = " (" + TagItem.LABEL + TypeProtocol.LABEL_STAMPAREG + TagItem.CLOSE_LABEL + ")";
                    break;
            }
            return labelTypeProto;
        }

        /// <summary>
        /// Aggiorna lo specializeItem di notification:se l'item era presente nel vecchio valore lo aggiorna, altrimenti lo inserisce
        /// </summary>
        /// <param name="specializeItem"></param>
        /// <param name="newValue"></param>
        /// <param name="typeLabel"></param>
        /// <returns></returns>
        private static string UpdateSpecializeItem(string specializeItem, string newValue, string typeLabel)
        {
            if (specializeItem.Contains(typeLabel))
            {
                string[] splitL = specializeItem.Split(new string[] { typeLabel + TagItem.CLOSE_LABEL }, StringSplitOptions.None);
                string oldValue = splitL[1].Split(new string[] {TagItem.CLOSE_LINE }, StringSplitOptions.None)[0];
                specializeItem = specializeItem.Replace(oldValue, newValue);
            }
            else
            {
                specializeItem += TagItem.LINE + TagItem.LABEL + 
                    typeLabel + TagItem.CLOSE_LABEL + newValue + TagItem.CLOSE_LINE;
            }

            return specializeItem;
        }

        public static string GetSignatureIntoNotification(string item2)
        {
            string signature = string.Empty;
            if (item2.Contains(TagItem.COLORRED))
            {
                string[] splitL = item2.Split(new string[] { TagItem.COLORRED }, StringSplitOptions.None);
                signature = splitL[1].Split(new string[] { TagItem.CLOSE_COLORRED }, StringSplitOptions.None)[0];
            }
            else if (item2.Contains(TagItem.COLORRED_STRIKE))
            {
                string[] splitL = item2.Split(new string[] { TagItem.COLORRED_STRIKE }, StringSplitOptions.None);
                signature = splitL[1].Split(new string[] { TagItem.CLOSE_COLORRED_STRIKE }, StringSplitOptions.None)[0];
            }

            return signature;
        }

        #endregion

        #region Follow Domain Object

        public static bool Follow(DocsPaVO.Notification.FollowDomainObject followObject)
        {
            DocsPaDB.Query_DocsPAWS.NotificationDB notificationDB = new DocsPaDB.Query_DocsPAWS.NotificationDB();
            return notificationDB.FollowDomainObjectManager(followObject);
        }

        #endregion
    }
}
