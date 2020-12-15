using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Notification;
using System.Data;
using DocsPaDbManagement.Functions;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    public class NotificationDB : DBProvider
    {
        #region Const
        private const string PREDISPOSTO = "PRED";
        private const string NON_PROTOCOLLATO = "G";
        private ILog logger = LogManager.GetLogger(typeof(NotificationDB));
        #endregion
     
        #region Query

        /// <summary>
        /// Return the list of notifications that are directed to:
        /// 1. the couple user role.
        /// 2. user
        /// </summary>
        /// <returns></returns>
        public List<Notification> ReadNotifications(string idPeople, string idGroup)
        {
            List<Notification> listNotifications = new List<Notification>();
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_NOTIFY");
                q.setParam("idPeople", idPeople);
                q.setParam("idGroup", idGroup);
                if (dbType.ToUpper() == "SQL")
                {
                    q.setParam("dbuser", getUserDB());
                }
                query = q.getSQL();
                if (this.ExecuteQuery(out ds, "notifications", query))
                {
                    BuildNotificationObject(ds, ref listNotifications);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return listNotifications;
        }


        public List<Notification> ReadNotificationsMobile(string idPeople, string idGroup, int requestedPage, int pageSize, out int totalRecordCount)
        {
            totalRecordCount = 0;
            List<Notification> listNotifications = new List<Notification>();
            try
            {
                totalRecordCount = GetCountNotifications(idPeople, idGroup);
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_NOTIFY_PAGE");
                q.setParam("idPeople", idPeople);
                q.setParam("idGruppo", idGroup);

                int numTotPage = (totalRecordCount / pageSize);
                if (numTotPage != 0)
                {
                    if ((totalRecordCount % numTotPage) > 0) numTotPage++;
                }
                else numTotPage = 1;

                int startRow = ((requestedPage * pageSize) - pageSize) + 1;
                int endRow = (startRow - 1) + pageSize;

                q.setParam("startRow", startRow.ToString());
                q.setParam("endRow", endRow.ToString());

                if (dbType.ToUpper() == "SQL")
                {
                    q.setParam("dbuser", getUserDB());
                }
                query = q.getSQL();
                if (this.ExecuteQuery(out ds, "notifications", query))
                {
                    BuildNotificationObject(ds, ref listNotifications);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return listNotifications;
        }

        private int GetCountNotifications(string idPeople, string idGruppo)
        {
            int result = 0;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_NOTIFY_COUNT");
                q.setParam("idPeople", idPeople);
                q.setParam("idGruppo", idGruppo);
                query = q.getSQL();

                string field;
                if (ExecuteScalar(out field, query))
                    result = Convert.ToInt32(field);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        /// <summary>
        /// Builds the list of objects from the dataset notification
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="listNotification"></param>
        private void BuildNotificationObject(DataSet ds, ref List<Notification> listNotification)
        {
            try
            {
                if (ds.Tables["notifications"] != null && ds.Tables["notifications"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["notifications"].Rows)
                    {
                        Notification notification = new Notification()
                        {
                            ID_NOTIFY = !string.IsNullOrEmpty(row["ID_NOTIFY"].ToString()) ? row["ID_NOTIFY"].ToString() : string.Empty,
                            ID_EVENT = !string.IsNullOrEmpty(row["ID_EVENT"].ToString()) ? row["ID_EVENT"].ToString() : string.Empty,
                            PRODUCER = !string.IsNullOrEmpty(row["PRODUCER"].ToString()) ? row["PRODUCER"].ToString() : string.Empty,
                            DTA_EVENT = Convert.ToDateTime(row["DTA_EVENT"].ToString(), new System.Globalization.CultureInfo("it-IT")),
                            DTA_NOTIFY = Convert.ToDateTime(row["DTA_NOTIFY"].ToString(), new System.Globalization.CultureInfo("it-IT")),
                            ID_PEOPLE = !string.IsNullOrEmpty(row["ID_PEOPLE"].ToString()) ? row["ID_PEOPLE"].ToString() : string.Empty,
                            ID_GROUP = !string.IsNullOrEmpty(row["ID_GROUP"].ToString()) ? row["ID_GROUP"].ToString() : string.Empty,
                            TYPE_NOTIFICATION = !string.IsNullOrEmpty(row["TYPE_NOTIFICATION"].ToString()) ?
                                Convert.ToChar(row["TYPE_NOTIFICATION"].ToString()) : ' ',
                            TYPE_EVENT = !string.IsNullOrEmpty(row["TYPE_EVENT"].ToString()) ? row["TYPE_EVENT"].ToString() : string.Empty,
                            MULTIPLICITY = !string.IsNullOrEmpty(row["MULTIPLICITY"].ToString()) ? row["MULTIPLICITY"].ToString() : string.Empty,
                            DOMAINOBJECT = !string.IsNullOrEmpty(row["DOMAINOBJECT"].ToString()) ? row["DOMAINOBJECT"].ToString() : string.Empty,
                            ID_OBJECT = !string.IsNullOrEmpty(row["ID_OBJECT"].ToString()) ? row["ID_OBJECT"].ToString() : string.Empty,
                            ID_SPECIALIZED_OBJECT = !string.IsNullOrEmpty(row["ID_SPECIALIZED_OBJECT"].ToString()) ?
                                row["ID_SPECIALIZED_OBJECT"].ToString() : string.Empty,
                            READ_NOTIFICATION = !string.IsNullOrEmpty(row["READ_NOTIFICATION"].ToString()) ?
                                Convert.ToChar(row["READ_NOTIFICATION"].ToString()) : '0',
                            ITEMS = new Items()
                            {
                                ITEM1 = !string.IsNullOrEmpty(row["ITEM1"].ToString()) ? row["ITEM1"].ToString() : string.Empty,
                                ITEM2 = !string.IsNullOrEmpty(row["ITEM2"].ToString()) ? row["ITEM2"].ToString() : string.Empty,
                                ITEM3 = !string.IsNullOrEmpty(row["ITEM3"].ToString()) ? row["ITEM3"].ToString() : string.Empty,
                                ITEM4 = !string.IsNullOrEmpty(row["ITEM4"].ToString()) ? row["ITEM4"].ToString() : string.Empty
                            },
                            ITEM_SPECIALIZED = !string.IsNullOrEmpty(row["ITEM_SPECIALIZED"].ToString()) ? row["ITEM_SPECIALIZED"].ToString() : "BLUE",
                            COLOR = !string.IsNullOrEmpty(row["COLOR"].ToString()) ? row["COLOR"].ToString() : string.Empty,
                            NOTES = !string.IsNullOrEmpty(row["NOTES"].ToString()) ? row["NOTES"].ToString() : string.Empty,
                            TEXT_SORTING = string.Empty,
                            EXTENSION = (string.IsNullOrEmpty(row["EXTENSION"].ToString()) ||
                                row["EXTENSION"].ToString().Equals("0")) ? string.Empty : row["EXTENSION"].ToString(),
                            SIGNED = !row.Table.Columns.Contains("SIGNED") || (string.IsNullOrEmpty(row["SIGNED"].ToString()) ||
                                row["SIGNED"].ToString().Equals("0")) ? '0' : Convert.ToChar(row["SIGNED"].ToString())
                        };

                        listNotification.Add(notification);
                    }
                    //dalla lista devo filtrare le notifiche associate ad eventi di interesse di applicazioni esterne.
                    if (listNotification != null && listNotification.Count > 0)
                    {
                        listNotification = (from n in listNotification
                                            where (!n.TYPE_EVENT.Equals("FOLLOW_DOC_EXT_APP") &&
                                                !n.TYPE_EVENT.Equals("FOLLOW_FASC_EXT_APP"))
                                            select n).ToList();
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// removes the notification with idNotification selected for delete from front end
        /// </summary>
        /// <param name="idNotification"></param>
        /// <returns></returns>
        public bool RemoveNotifications(List<Notification> listNotification, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                BeginTransaction();
                int rowAffected;
                string idPeopleDelegato = infoUtente.delegato != null && !string.IsNullOrEmpty(infoUtente.delegato.idPeople) ? infoUtente.delegato.idPeople : string.Empty;
                string setIdPeopleDelegato = string.Empty;
                foreach (Notification notification in listNotification)
                {
                    result = false;
                    string query;
                    DataSet ds = new DataSet();
                    DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_NOTIFY");
                    q1.setParam("idNotification", notification.ID_NOTIFY);
                    query = q1.getSQL();
                    this.ExecuteNonQuery(query, out  rowAffected);
                    if (rowAffected > 0)
                        result = true;

                    if (result && IsTrasm(notification.TYPE_EVENT))
                    {
                        ds = new DataSet();
                        DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("U_DATE_REMOVE_FROM_NC");
                        q2.setParam("idTrasmSing", notification.ID_SPECIALIZED_OBJECT);
                        q2.setParam("idPeople", notification.ID_PEOPLE);

                        if (!string.IsNullOrEmpty(idPeopleDelegato))
                            setIdPeopleDelegato = " ,CHA_RIMOZIONE_DELEGATO = '1', ID_PEOPLE_DELEGATO=" + idPeopleDelegato;

                        q2.setParam("setIdPeopleDelegato", setIdPeopleDelegato);

                        query = q2.getSQL();
                        if (!this.ExecuteNonQuery(query, out  rowAffected))
                        {
                            logger.Info("Errore in DocsPaDb.Query_DocsPAWS.NotificationDB - Metodo RemoveNotifications : errore query U_DATE_REMOVE_FROM_NC con ID TRASMISSIONE SINGOLA " + notification.ID_SPECIALIZED_OBJECT);
                        }
                        //this.ExecuteNonQuery(query, out  rowAffected);
                        //if (rowAffected > 0)  
                        //  result = true;
                        //else result = false;

                    }
                    if (result)
                    {
                        result = InsertIntoHistory(notification);
                    }
                    else
                    {
                        RollbackTransaction();
                        break;
                    }
                }
                if (result)
                {
                    this.CommitTransaction();
                }
                else
                {
                    this.RollbackTransaction();
                }
            }
            catch (Exception exc)
            {
                RollbackTransaction();
                throw exc;
            }
            return result;
        }

        /// <summary>
        /// Rimuove le notifiche di trasmissioni che non necessitano accettazione antecedenti la data specificata
        /// </summary>
        /// <param name="idObject"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool RemoveNotificationOfTransmissionNoWF(string idPeople, string idGroup, string date)
        {
            bool result = true;
            try
            {
                BeginTransaction();
                List<string> listIdNotification = new List<string>();
                DataSet ds = new DataSet();
                string query = string.Empty;
                string condition = string.Empty;
                int rowAffected;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_NOTIFY_NO_WF");
                q.setParam("param1", idPeople);
                q.setParam("param2", idGroup);
                q.setParam("param3", DocsPaDbManagement.Functions.Functions.ToDate(date, false));
                query = q.getSQL();
                if (this.ExecuteQuery(out ds, "notifications", query))
                {
                    if (ds.Tables["notifications"] != null && ds.Tables["notifications"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["notifications"].Rows)
                        {
                            listIdNotification.Add(row["SYSTEM_ID"].ToString());
                        }
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_NOTIFY_HISTORY_MULTIPLE");
                        string listSystemId = string.Empty;
                        foreach (string id in listIdNotification)
                        {
                            listSystemId += string.IsNullOrEmpty(listSystemId) ? " '" + id + "'" : " ,'" + id + "'";
                        }
                        condition = "SYSTEM_ID IN (" + listSystemId + ")";
                        q.setParam("condition", condition);
                        if (this.ExecuteNonQuery(q.getSQL()))
                        {
                            foreach (string systemId in listIdNotification)
                            {
                                q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_NOTIFY");
                                q.setParam("idNotification", systemId);
                                if (this.ExecuteNonQuery(q.getSQL()))
                                    result = true;
                                else
                                {
                                    result = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            result = false;
                        }
                        if (result)
                        {
                            foreach (DataRow row in ds.Tables["notifications"].Rows)
                            {
                                ds = new DataSet();
                                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DATE_REMOVE_FROM_NC");
                                q.setParam("idTrasmSing", row["ID_SPECIALIZED_OBJECT"].ToString());
                                q.setParam("idPeople", idPeople);
                                query = q.getSQL();
                                if (!this.ExecuteNonQuery(query, out  rowAffected))
                                {
                                    logger.Info("Errore in DocsPaDb.Query_DocsPAWS.NotificationDB - Metodo RemoveNotifications : errore query U_DATE_REMOVE_FROM_NC con ID TRASMISSIONE SINGOLA " + row["ID_SPECIALIZED_OBJECT"].ToString());
                                }
                            //        result = true;
                            //    else
                            //    {
                            //        result = false;
                            //        break;
                            //    }
                            }
                        }
                        else
                        {
                            RollbackTransaction();
                        }
                    }
                }
                if (result)
                {
                    this.CommitTransaction();
                }
                else
                {
                    RollbackTransaction();
                }

            }
            catch (Exception e)
            {
                RollbackTransaction();
                result = false;
                throw e;
            }
            return result;
        }

        /// <summary>
        /// copy of the notification removed from the notification center in the history
        /// </summary>
        public bool InsertIntoHistory(DocsPaVO.Notification.Notification notification)
        {
            try
            {
                int res;
                string values = string.Empty;
                string query;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_NOTIFY_HISTORY");
                System.Text.StringBuilder strBuilder = new StringBuilder();
                if (DBType.ToUpper().Equals("ORACLE"))
                    strBuilder.Append("seq.nextval, ");
                strBuilder.Append(notification.ID_NOTIFY + ", ");
                strBuilder.Append(notification.ID_EVENT + ", ");
                strBuilder.Append("'" + notification.PRODUCER.Replace("'", "''") + "', ");
                strBuilder.Append(notification.ID_PEOPLE + ", ");
                strBuilder.Append(notification.ID_GROUP + ", ");
                strBuilder.Append("'" + notification.TYPE_NOTIFICATION + "', ");
                strBuilder.Append(DocsPaDbManagement.Functions.Functions.ToDate(notification.DTA_NOTIFY.ToString()) + ", ");
                //if (dbType.ToUpper() == "SQL")
                //    strBuilder.Append("CONVERT(DATETIME,'" + notification.DTA_NOTIFY.ToShortDateString() + " " + notification.DTA_NOTIFY.ToLongTimeString() + "',103), ");
                //else
                //    strBuilder.Append("to_date('" + notification.DTA_NOTIFY.ToShortDateString() + " " + notification.DTA_NOTIFY.ToLongTimeString() + "','DD/MM/YYYY  HH24:MI:SS'), ");
                strBuilder.Append("'" + notification.ITEMS.ITEM1.Replace("'", "''") + "', ");
                strBuilder.Append("'" + notification.ITEMS.ITEM2.Replace("'", "''") + "', ");
                strBuilder.Append("'" + notification.ITEMS.ITEM3.Replace("'", "''") + "', ");
                strBuilder.Append("'" + notification.ITEMS.ITEM4.Replace("'", "''") + "', ");
                strBuilder.Append("'" + notification.MULTIPLICITY.Replace("'", "''") + "', ");
                strBuilder.Append("'" + notification.ITEM_SPECIALIZED.Replace("'", "''") + "', ");
                strBuilder.Append("'" + notification.TYPE_EVENT + "', ");
                strBuilder.Append("'" + notification.DOMAINOBJECT + "', ");
                strBuilder.Append(notification.ID_OBJECT + ", ");
                strBuilder.Append(notification.ID_SPECIALIZED_OBJECT + ", ");
                strBuilder.Append(DocsPaDbManagement.Functions.Functions.ToDate(notification.DTA_EVENT.ToString()) + ", ");
                //if (dbType.ToUpper() == "SQL")
                //    strBuilder.Append("CONVERT(DATETIME,'" + notification.DTA_EVENT.ToShortDateString() + " " + notification.DTA_EVENT.ToLongTimeString() + "',103), ");
                //else
                //    strBuilder.Append("to_date('" + notification.DTA_EVENT.ToShortDateString() + " " + notification.DTA_EVENT.ToLongTimeString() + "','DD/MM/YYYY  HH24:MI:SS'), ");
                strBuilder.Append("'" + notification.READ_NOTIFICATION + "', ");
                strBuilder.Append("'" + notification.NOTES + "' ");
                values = strBuilder.ToString();
                q.setParam("values", values);
                query = q.getSQL();
                this.ExecuteNonQuery(query, out res);
                if (res > 0)
                    return true;
                else return false;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Rimuove la note nella notifica
        /// </summary>
        /// <param name="idNotify"></param>
        /// <returns></returns>
        public bool UpdateNoteNotification(string idNotify, string note)
        {
            bool result = false;
            try
            {
                int rowAffected;
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_NOTIFY_NOTES");
                q.setParam("idNotification", idNotify);
                q.setParam("notes", note.Replace("'", "''"));
                query = q.getSQL();
                this.ExecuteNonQuery(query, out  rowAffected);
                if (rowAffected > 0)
                    result = true;
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        /// <summary>
        /// Return the number of notifications that are direct to:
        /// 1. the couple user-roles different by role in input.
        /// 2. user
        /// </summary>
        /// <returns></returns>
        public int GetNumberNotificationOtherRoles(string idPeople, string idGroup)
        {
            string outParam = string.Empty;
            try
            {
                string query;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_NOTIFY_NUMBER_NOTIFY_OTHER_ROLES");
                q.setParam("idPeople", idPeople);
                q.setParam("idGroup", idGroup);
                query = q.getSQL();
                this.ExecuteScalar(out outParam, query);
                {

                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return Convert.ToInt32(outParam);
        }

        /// <summary>
        /// Cambia lo stato della notifica da non letta a letta, oppure rimuove la notifica nel caso in cui
        /// l'amministrazione non ha abilitato il tasto visto e la notifica è legata ad una di trasmissione che non necessita accettazione
        /// o che non sia con tipo ragione INTEROPERABILITA PITRE
        /// </summary>
        /// <returns>Ritorna true se la notifica è stata rimossa</returns>
        public bool ChangeStateReadNotification(Notification notification, bool isNotEnabledSetDataVistaGrd)
        {
            logger.Info("Inizio Metodo ChangeStateReadNotification in DocsPaDb.Query_DocsPAWS.NotificationDB");
            bool result = false;
            string outParam = string.Empty;
            try
            {
                string query;
                //Se l'amministrazione non ha abilitato il tasto visto e la notifica non è relativa ad una trasmissione di tipo workflow o
                //interoperabilita pitre rimuovo la notifica.
                if (isNotEnabledSetDataVistaGrd)
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_RAGIONE_TRASM_BY_ID_NOTIFY");
                    q.setParam("idNotify", notification.ID_NOTIFY);
                    query = q.getSQL();
                    this.ExecuteScalar(out outParam, query);
                    if (!string.IsNullOrEmpty(outParam))
                    {
                        result = true;
                    }
                }
                if (result)
                {
                    //Se la moltiplicità è di tipo ONE rimuovo in tutto il ruolo altrimenti al solo utente
                    if (notification.MULTIPLICITY.Equals(DocsPaVO.Notification.Multiplicity.ONE))
                    {
                        result = CheckNotification(notification.ID_EVENT);
                    }
                    else
                    {
                        result = CheckNotification(notification.ID_EVENT, notification.ID_PEOPLE, notification.ID_GROUP);
                    }
                }
                else
                {
                    //se è abilitato il tasto visto oppure non è una trasmissione del tipo elencato in alto si cambia lo stato
                    //da letta a non letta
                    SetNotificationAsRead(notification);
                }
            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.NotificationDB - Metodo ChangeStateReadNotification", exc);
                throw exc;
            }
            logger.Info("Fine Metodo ChangeStateReadNotification in DocsPaDb.Query_DocsPAWS.NotificationDB");
            return result;
        }

        /// <summary>
        /// Modifica lo stato della notifica da non letta a letta
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public bool SetNotificationAsRead(Notification notification)
        {
            logger.Info("Inizio Metodo SetNotificationAsRead in DocsPaDb.Query_DocsPAWS.NotificationDB");
            bool result = false;
            string outParam = string.Empty;
            int rowAffected = 0;
            try
            {
                string query;DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_NOTIFY_READ_NOTIFICATION");
                q.setParam("idNotify", notification.ID_NOTIFY);
                query = q.getSQL();
                this.ExecuteNonQuery(query, out  rowAffected);
            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.NotificationDB - Metodo SetNotificationAsRead", exc);
                throw exc;
            }
            logger.Info("Fine Metodo SetNotificationAsRead in DocsPaDb.Query_DocsPAWS.NotificationDB");
            return result;
        }

        /// <summary>
        /// Delete a notification to the dpa_notify and applies the multiplicity one all
        /// </summary>
        /// <param name="idEvent"></param>
        /// <param name="multiplicity"></param>
        /// <returns></returns>
        public bool CheckNotification(string idEvent, string idPeople = "", string idGroup = "")
        {
            bool result = true;
            try
            {
                BeginTransaction();
                string query;
                if (string.IsNullOrEmpty(idPeople))
                {
                    string condition = "ID_EVENT = " + idEvent;
                    if (InsertIntoHistoryMultiple(condition))
                    {
                        DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_NOTIFY_MULTIPLICITY_ONE");

                        string idGruppo = string.Empty;
                        if (!string.IsNullOrEmpty(idGroup))
                            idGruppo = " AND ID_GROUP_RECEIVER = " + idGroup;

                        q.setParam("idEvent", idEvent);
                        q.setParam("idGruppo", idGruppo);
                        query = q.getSQL();
                        if (this.ExecuteNonQuery(query))
                        {
                            CommitTransaction();
                        }
                        else
                        {
                            RollbackTransaction();
                            result = false;
                        }
                    }
                    else
                    {
                        RollbackTransaction();
                        result = false;
                    }
                }
                else
                {
                    string condition = "ID_EVENT = " + idEvent + " AND ID_PEOPLE_RECEIVER = " + idPeople + " AND ID_GROUP_RECEIVER = " + idGroup;
                    if (InsertIntoHistoryMultiple(condition))
                    {
                        DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_NOTIFY_MULTIPLICITY_ALL");
                        q.setParam("idEvent", idEvent);
                        q.setParam("idPeople", idPeople);
                        q.setParam("idGroup", idGroup);
                        query = q.getSQL();
                        if (this.ExecuteNonQuery(query))
                        {
                            CommitTransaction();
                        }
                        else
                        {
                            RollbackTransaction();
                            result = false;
                        }
                    }
                    else
                    {
                        RollbackTransaction();
                        result = false;
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        /// <summary>
        /// Storicizza tutte le notifiche generate dall'evento con valore idEvent
        /// </summary>
        /// <param name="idEvent"></param>
        /// <returns></returns>
        private bool InsertIntoHistoryMultiple(string condition)
        {
            try
            {
                int res;
                string values = string.Empty;
                string query;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_NOTIFY_HISTORY_MULTIPLE");
                q.setParam("condition", condition);
                query = q.getSQL();
                this.ExecuteNonQuery(query, out res);
                if (res > 0)
                    return true;
                else return false;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Ritorna la lista degli id dei documenti
        /// </summary>
        /// <param name="idObject"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<string> GetDocumentByNotificationsFilters(string[] idObject, DocsPaVO.Notification.NotificationsFilters filter)
        {
            List<string> listIdObject = new List<string>();
            try
            {
                DataSet ds = new DataSet();
                string query = string.Empty;
                string condition = string.Empty;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROFILE_SYSTEM_ID_BY_FILTER_NOTIFICATION");
                condition = BuildConditionNotificationsFilters(idObject, filter);
                q.setParam("condition", condition);
                query = q.getSQL();
                if (this.ExecuteQuery(out ds, "idObject", query))
                {
                    if (ds.Tables["idObject"] != null && ds.Tables["idObject"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["idObject"].Rows)
                        {
                            listIdObject.Add(row["SYSTEM_ID"].ToString());
                        }
                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            return listIdObject;
        }

        /// <summary>
        /// Prende l'id delle trasmissioni singole con workflow in attesa di accettazione/rifiuto
        /// </summary>
        /// <param name="idSpecializedObject"></param>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public List<string> GetIdTrasmPendingByNotificationsFilters(string[] idSpecializedObject, string idPeople)
        { 
            List<string> listIdSpecializedObject = new List<string>();
            try
            {
                DataSet ds = new DataSet();
                string query = string.Empty;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TRASM_UTENTE_ID_TRASM_SINGOLA_BY_FILTER_NOTIFICATION");
                string listIdTransUser= string.Empty;
                foreach (string id in idSpecializedObject)
                {
                    listIdTransUser += string.IsNullOrEmpty(listIdTransUser) ? " '" + id + "'" : " ,'" + id + "'";
                }
                q.setParam("listIdTransUser", listIdTransUser);
                q.setParam("idPeople", idPeople);
                query = q.getSQL();
                if (this.ExecuteQuery(out ds, "idSpecializedObject", query))
                {
                    if (ds.Tables["idSpecializedObject"] != null && ds.Tables["idSpecializedObject"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["idSpecializedObject"].Rows)
                        {
                            listIdSpecializedObject.Add(row["ID_TRASM_SINGOLA"].ToString());
                        }
                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            return listIdSpecializedObject;
        }

        /// <summary>
        /// Restituisce il numero di giorni oltre i quali la notifica viene segnalata come "vecchia"
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public string GetNoticeDaysNotification(string idAmm)
        {
            string noticeDays = string.Empty;
            try
            {
                DataSet ds = new DataSet();
                string query = string.Empty;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministra2");
                q.setParam("param1", "CHA_ATTIVA_GG_PERM_TODOLIST,NUM_GG_PERM_TODOLIST");
                q.setParam("param2", idAmm);
                query = q.getSQL();
                this.ExecuteQuery(out ds, "AMM", query);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables["AMM"].Rows[0]["CHA_ATTIVA_GG_PERM_TODOLIST"].ToString() != null &&
                        ds.Tables["AMM"].Rows[0]["CHA_ATTIVA_GG_PERM_TODOLIST"].ToString().Equals("1"))
                    {
                        noticeDays = ds.Tables["AMM"].Rows[0]["NUM_GG_PERM_TODOLIST"].ToString();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return noticeDays;
        }

       

        #endregion

        /// <summary>
        /// Effettua l'update, delle notifiche, dei campi soggetti a variazioni
        /// </summary>
        /// <param name="notificationList">Lista delle notifiche di cui si vuole aggiornare i campi</param>
        /// <returns></returns>
         public bool UpdateNotifications(List<Notification> notificationList)
         {
             bool result = false;
             try
             {
                 BeginTransaction();
                 int rowAffected;
                 string query;
                 foreach (Notification notification in notificationList)
                 {

                     DataSet ds = new DataSet();
                     DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_NOTIFY");
                     q.setParam("idNotification", notification.ID_NOTIFY);
                     q.setParam("field_1", notification.ITEMS.ITEM1.Replace("'", "''"));
                     q.setParam("field_2", notification.ITEMS.ITEM2.Replace("'", "''"));
                     q.setParam("field_3", notification.ITEMS.ITEM3.Replace("'", "''").Replace("°", Functions.convertDegre()));
                     q.setParam("field_4", notification.ITEMS.ITEM4.Replace("'", "''"));
                     q.setParam("specializeItem", notification.ITEM_SPECIALIZED.Replace("'", "''").Replace("°", Functions.convertDegre()));
                     query = q.getSQL();
                     this.ExecuteNonQuery(query, out  rowAffected);
                     if (rowAffected > 0)
                     {
                         result = true;
                     }
                     else
                     {
                         result = false;
                     }
                 }
                 if (result)
                 {
                     this.CommitTransaction();
                 }
                 else
                 {
                     this.RollbackTransaction();
                 }
             }
             catch (Exception exc)
             {
                 RollbackTransaction();
                 throw exc;
             }
             return result;
         }

        /// <summary>
        /// Seleziona le notifiche con id_object e domain_Object specificato
        /// </summary>
        /// <param name="idObject"></param>
        /// <param name="domainObject"></param>
        /// <returns></returns>
         public List<Notification> GetNotificationsByIdObject(string idObject, string domainObject)
         {
             List<Notification> listNotifications = new List<Notification>();
             try
             {
                 string query;
                 DataSet ds = new DataSet();
                 DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_NOTIFY_BY_ID_OBJECT");
                 q.setParam("idObject", idObject);
                 q.setParam("domainObject", domainObject);
                 query = q.getSQL();
                 if (this.ExecuteQuery(out ds, "notifications", query))
                 {
                     BuildNotificationObject(ds, ref listNotifications);
                 }
             }
             catch (Exception exc)
             {
                 throw exc;
             }
             return listNotifications;
         }
    
        #region utils

        /// <summary>
        /// Returns true if the event type is broadcast being transmitted
        /// </summary>
        /// <param name="typeEventExtended"></param>
        /// <returns></returns>
        private bool IsTrasm(string typeEventExtended)
        {
            if (typeEventExtended.StartsWith("TRASM_"))
                return true;
            else return false;

        }

        /// <summary>
        /// Costruisce la clausula where per la query che si occupa di recuperare i system_id dei documenti rispentanti i filtri applicati
        /// </summary>
        /// <param name="idObject"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private string BuildConditionNotificationsFilters(string[] idObject, NotificationsFilters filter)
        {
            string condition = string.Empty;
            string listSystemId = string.Empty;
            string UserDB = string.Empty;

            if (dbType.ToUpper() == "SQL")
                UserDB = getUserDB();

            condition += " (SYSTEM_ID IN(";
            if (idObject != null && idObject.Count() > 0)
            {
                int i = 0;
                foreach (string id in idObject)
                {
                    condition += id;
                    if (i < idObject.Length - 1)
                    {
                        if (i % 998 == 0 && i > 0)
                        {
                            condition += ") OR SYSTEM_ID IN (";
                        }
                        else
                        {
                            condition += ", ";
                        }
                    }
                    else
                    {
                        condition += ")";
                    }
                    i++;
                }
            }
            else
            {
                condition += ")";
            }
            condition += ") AND p.SYSTEM_ID = c.DOCNUMBER";
 


            //foreach (string id in idObject)
            //{
            //    listSystemId += string.IsNullOrEmpty(listSystemId) ? " '" + id + "'" : " ,'" + id + "'";
            //}
            //condition = "SYSTEM_ID IN (" + listSystemId + ") AND p.SYSTEM_ID = c.DOCNUMBER";

            #region Filter by TYPE DOCUMENT

            if (!string.IsNullOrEmpty(filter.TYPE_DOCUMENT))
            {
                if (filter.TYPE_DOCUMENT.Equals(PREDISPOSTO))
                {
                    condition += " AND p.CHA_TIPO_PROTO IN ('P', 'A', 'I') AND p.NUM_PROTO IS NULL";
                }
                else
                {
                    condition += filter.TYPE_DOCUMENT.Equals(NON_PROTOCOLLATO) ? " AND p.CHA_TIPO_PROTO='" + filter.TYPE_DOCUMENT + "'" :
                        " AND p.CHA_TIPO_PROTO='" + filter.TYPE_DOCUMENT + "' AND p.NUM_PROTO IS NOT NULL";
                }
            }

            #endregion

            #region Filter by FILE ACQUIRED

            string fileAcquired = string.Empty;
            if (filter.DOCUMENT_ACQUIRED)
            {
                fileAcquired = " AND (c.FILE_SIZE > 0";
            }
            if (filter.DOCUMENT_SIGNED)
            {
                if (!string.IsNullOrEmpty(UserDB))
                {
                    fileAcquired += string.IsNullOrEmpty(fileAcquired) ? " AND ( " + UserDB + ".GETCHAFIRMATO(p.DOCNUMBER) = '1'" : "OR  " + UserDB + ".GETCHAFIRMATO(p.DOCNUMBER) = '1'";
                }
                else
                {
                    fileAcquired += string.IsNullOrEmpty(fileAcquired) ? " AND (GETCHAFIRMATO(p.DOCNUMBER) = '1'" : "OR GETCHAFIRMATO(p.DOCNUMBER) = '1'";
                }
            }
            if (filter.DOCUMENT_UNSIGNED)
            {
                if (!string.IsNullOrEmpty(UserDB))
                {
                    fileAcquired += string.IsNullOrEmpty(fileAcquired) ? " AND ( " + UserDB + ".GETCHAFIRMATO(p.DOCNUMBER) = '0'" : " OR " + UserDB + ".GETCHAFIRMATO(p.DOCNUMBER) = '0'";
                }
                else
                {
                    fileAcquired += string.IsNullOrEmpty(fileAcquired) ? " AND (GETCHAFIRMATO(p.DOCNUMBER) = '0'" : " OR GETCHAFIRMATO(p.DOCNUMBER) = '0'";
                }
            }
            if (!string.IsNullOrEmpty(fileAcquired))
            {
                fileAcquired += ")";
            }
            condition += fileAcquired;

            #endregion

            #region Filter by TYPE FILE ACQUIRED

            if (!string.IsNullOrEmpty(filter.TYPE_FILE_ACQUIRED))
            {
                if (!string.IsNullOrEmpty(UserDB))
                {
                    condition += " AND UPPER( " + UserDB + ".GETCHAIMG(p.DOCNUMBER)) = '" + filter.TYPE_FILE_ACQUIRED + "'";
                }
                else
                {
                    condition += " AND UPPER(GETCHAIMG(p.DOCNUMBER)) = '" + filter.TYPE_FILE_ACQUIRED + "'";
                }
            }
            #endregion

            return condition;
        }

        public string getUserDB()
        {
            return Functions.GetDbUserSession();
        }

        #endregion

        #region Follow Domain Object

        public bool FollowDomainObjectManager(DocsPaVO.Notification.FollowDomainObject followObject)
        {
            bool res = false;
            try
            {

                if (this.dbType.ToUpper().Equals("SQL")) //non implementato per SQL SERVER
                {
                    res = true;
                    return res;
                }


                    this.BeginTransaction();

                    logger.Debug("INIZIO SP_FOLLOW_DOMAINOBJECT");

                    // Creazione parametri per la Store Procedure
                    System.Collections.ArrayList parameters = new System.Collections.ArrayList();
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("idProject", !string.IsNullOrEmpty(followObject.IdProject) ? followObject.IdProject : "0"));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("idProfile", !string.IsNullOrEmpty(followObject.IdProfile) ? followObject.IdProfile : "0"));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("operation", Convert.ToInt32(followObject.Operation).ToString()));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("idPeople", !string.IsNullOrEmpty(followObject.IdPeople) ? followObject.IdPeople : "0"));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("idGroup", !string.IsNullOrEmpty(followObject.IdGroup) ? followObject.IdGroup : "0"));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("idAmm", !string.IsNullOrEmpty(followObject.IdAmm) ? followObject.IdAmm : "0"));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("application", !string.IsNullOrEmpty(followObject.App) ? followObject.App : string.Empty));
                    int result = this.ExecuteStoreProcedure("SP_FOLLOW_DOMAINOBJECT", parameters);
                    if (result == 0)
                    {
                        res = true;
                        logger.Debug("STORE PROCEDURE SP_FOLLOW_DOMAINOBJECT: esito positivo");
                    }
                    else
                    {
                        logger.Debug("STORE PROCEDURE SP_FOLLOW_DOMAINOBJECT: esito negativo");
                    }
                    this.CommitTransaction();
                
            }

            catch (Exception exc)
            {
                logger.Debug("Eccezione in DocsPaDB.Query_DocsPAWS.NotificationDB.FollowDomainObjectManager(...):\nStack trace:\n" + exc.StackTrace);
                this.RollbackTransaction();
            }
            return res;
        
        }

        #endregion
    }

}
