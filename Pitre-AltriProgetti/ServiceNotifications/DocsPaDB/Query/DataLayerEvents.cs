using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DocsPaVO.NotificationCenter;
using System.Configuration;
using System.IO;
using log4net;

namespace DocsPaDB.Query
{
    public class DataLayerEvents : DBProvider
    {
        private static ILog logger = LogManager.GetLogger(typeof(DataLayerEvents));
        /// <summary>
        /// Returns the list of events to be processed
        /// </summary>
        /// <returns></returns>
        public List<Event> SelectEventsToBeProcessed()
        {
            List<Event> listEvent = new List<Event>();
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_LOG_DPA_ANAGRAFICA_LOG");
                string queryString = q.getSQL();
                OpenConnection();
                this.BeginTransaction();
                if (!this.ExecuteQuery(out ds, "RecordEvents", q.getSQL()))
                {
                    this.RollbackTransaction();
                    return listEvent;
                }
                else
                {
                    this.CreateEventObjects(ds, ref listEvent);
                }
            }
            catch (Exception exc)
            {
                logger.Error(exc);
            }
            finally
            {
                if(listEvent.Count < 1)
                    //da eliminare in questo punto del codice
                    this.RollbackTransaction();
                CloseConnection();
            }
            return listEvent;
        }

        /// <summary>
        /// Updates the list of events correctly processatyi and commit the transaction
        /// </summary>
        /// <param name="listEvents"></param>
        public void UnlockEventQueue(List<Event> listEvents)
        {
            try
            {
                System.Text.StringBuilder strBuilder = new StringBuilder();
                string query = string.Empty;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_LOG");
                string queryString = q.getSQL();
                if (listEvents != null && listEvents.Count > 0)
                {
                    foreach (Event e in listEvents)
                    {
                        if (e.NOTIFIED)
                        {
                            if (strBuilder.Length > 0)
                            {
                                strBuilder.Append(", " + e.SYSTEM_ID);
                            }
                            else
                            {
                                strBuilder.Append(e.SYSTEM_ID);
                            }
                        }
                    }
                }

                if (strBuilder.Length > 0)
                {
                    q.setParam("condition", strBuilder.ToString());
                    query = q.getSQL();
                    if (!this.ExecuteNonQuery(query))
                    {
                        this.RollbackTransaction();
                        CloseConnection();
                    }
                }

                this.CommitTransaction();
                CloseConnection();
            }
            catch (Exception exc) 
            {
                // traccia l'eccezione nel file di log
                logger.Error(exc);
                this.RollbackTransaction();
                CloseConnection();
            }
        }

        /// <summary>
        /// Builds the list of events from the dataset to be processed.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="listEvent"></param>
        private void CreateEventObjects(DataSet ds, ref List<Event> listEvent)
        {
            try
            {
                if (ds.Tables["RecordEvents"] != null && ds.Tables["RecordEvents"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["RecordEvents"].Rows)
                    {
                        DataRow dr = row;
                        Event e = new Event()
                        {
                            SYSTEM_ID = Utils.ConvertField(dr["SYSTEM_ID"].ToString()),
                            TYPE_EVENT = dr["TYPE_EVENT"].ToString(),
                            CONFIGURATION_EVENT_TYPE = !string.IsNullOrEmpty(dr["NOTIFY"].ToString()) ? dr["NOTIFY"].ToString() : string.Empty,
                            NOTIFICATION_RECIPIENTS = !string.IsNullOrEmpty(dr["NOTIFICATION_RECIPIENTS"].ToString()) ? dr["NOTIFICATION_RECIPIENTS"].ToString() : string.Empty,
                            MULTIPLICITY = !string.IsNullOrEmpty(dr["MULTIPLICITY"].ToString()) ? dr["MULTIPLICITY"].ToString() : string.Empty,
                            ID_SPECIALIZED_OBJECT = !string.IsNullOrEmpty(dr["ID_SPECIALIZED_OBJECT"].ToString()) ?
                                Utils.ConvertField(dr["ID_SPECIALIZED_OBJECT"].ToString()) : 0,
                            DTA_EVENT = dr["DTA_EVENT"].ToString(),
                            ID_OBJECT = Utils.ConvertField(dr["ID_OBJECT"].ToString()),
                            ACTORS = new Actors(new Producer()
                            {
                                DES_PRODUCER = dr["DESC_PRODUCER"].ToString(),
                                IDROLE = Utils.ConvertField(dr["IDROLE"].ToString()),
                                IDUSER = Utils.ConvertField(dr["IDUSER"].ToString()),
                                IDUSERDELEGATOR = !string.IsNullOrEmpty(dr["ID_PEOPLE_DELEGANTE"].ToString()) ? Utils.ConvertField(dr["ID_PEOPLE_DELEGANTE"].ToString()) : 0
                            }
                            ),
                            ID_ADM = Utils.ConvertField(dr["ID_ADM"].ToString()),
                            DOMAIN_OBJECT = !string.IsNullOrEmpty(dr["DOMAIN_OBJECT"].ToString()) ? dr["DOMAIN_OBJECT"].ToString() : string.Empty,
                            COLOR = !string.IsNullOrEmpty(dr["COLOR"].ToString()) ? dr["COLOR"].ToString() : "BLUE",
                            ID_TYPE_EVENT = dr["ID_TYPE_EVENT"].ToString(),
                            FOLLOW_DOMAIN_OBJECT = (!string.IsNullOrEmpty(dr["FOLLOW"].ToString()) && dr["FOLLOW"].ToString().Equals("1")) ? true : false
                        };
                        if (string.IsNullOrEmpty(e.MULTIPLICITY))
                        {
                            e.MULTIPLICITY = this.MultiplicityEvent(e.TYPE_EVENT, e.ID_SPECIALIZED_OBJECT);
                        }
                        listEvent.Add(e);
                    }
                }
            }
            catch (Exception exc)
            {
                // traccia l'eccezione nel file di log
                logger.Error(exc);

                listEvent = new List<Event>();
                throw exc;
            }
        }
        ///<summary>
        /// Returns the multiplicity for those events whose multiplicity value is tied to the object specialized
        /// </summary>
        /// <param name="notificationRecipients"></param>
        /// <param name="idSpecializedObject"></param>
        /// <returns></returns>
        private string MultiplicityEvent(string eventType, int idSpecializedObject)
        {
            string multiplicity = string.Empty;
            try
            {
                switch(Utils.GetTypeEvent(eventType))
                {
                    case SupportStructures.EventType.TRASM:
                        DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_MULTIPLICITY_EVENT_TRASM");
                        q.setParam("idTrasmSing", idSpecializedObject.ToString());
                        this.ExecuteScalar(out multiplicity, q.getSQL());
                        break;

                    default:
                        break;
                }
            }
            catch(Exception exc)
            {
                // traccia l'eccezione nel file di log
                logger.Error(exc);
            }
            if (!string.IsNullOrEmpty(multiplicity))
            {
                if (multiplicity.Equals("T"))
                    return SupportStructures.MULTIPLICITY.ALL;
                else if (multiplicity.Equals("S"))
                    return SupportStructures.MULTIPLICITY.ONE;
            }
            return multiplicity;
        }
    }
}
