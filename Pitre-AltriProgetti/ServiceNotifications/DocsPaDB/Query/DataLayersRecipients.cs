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
    public class DataLayersRecipients : DBProvider
    {
        private static ILog logger = LogManager.GetLogger(typeof(DataLayersRecipients));
        /// <summary>
        /// Returns the list of recipients
        /// </summary>
        /// <returns></returns>
        public List<Recipient> BuildRecipients(Event e)
        {
            List<Recipient> listRecipients = new List<Recipient>();
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q;
                string queryString = string.Empty;
                string idObject = (e.ID_SPECIALIZED_OBJECT == null || 
                    e.ID_SPECIALIZED_OBJECT == 0 ||
                    e.TYPE_EVENT.Equals(SupportStructures.EventType.FOLLOW_FASC_EXT_APP))? 
                    e.ID_OBJECT.ToString() : e.ID_SPECIALIZED_OBJECT.ToString();
                switch (e.NOTIFICATION_RECIPIENTS)
                {
                    case DocsPaVO.NotificationCenter.SupportStructures.BuildingOptionsRecipients.TRANSMISSION_RECIPIENTS:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RECIPIENTS_TRANSMISSION");
                        q.setParam("idSpecializedObject", idObject);
                        queryString = q.getSQL();
                        if (!this.ExecuteQuery(out ds, "Recipients", q.getSQL()))
                        {
                            return listRecipients;
                        }
                        break;
                    case DocsPaVO.NotificationCenter.SupportStructures.BuildingOptionsRecipients.S_SENDER_USERS_ROLE_SENDER_TRANSMISSION:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SENDER_USERS_ROLE_SENDER_TRANSMISSION");
                        q.setParam("idSpecializedObject", idObject);
                        queryString = q.getSQL();
                        if (!this.ExecuteQuery(out ds, "Recipients", q.getSQL()))
                        {
                            return listRecipients;
                        }
                        break;
                    case SupportStructures.BuildingOptionsRecipients.SENDER_TRANSMISSION:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SENDER_TRANSMISSION");
                        q.setParam("idSpecializedObject", idObject);
                        queryString = q.getSQL();
                        if (!this.ExecuteQuery(out ds, "Recipients", q.getSQL()))
                        {
                            return listRecipients;
                        }
                        break;
                    case SupportStructures.BuildingOptionsRecipients.ACL_DOMAINOBJECT:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ACL_DOMAINOBJECT");
                        q.setParam("idSpecializedObject", idObject);
                        queryString = q.getSQL();
                        if (!this.ExecuteQuery(out ds, "Recipients", q.getSQL()))
                        {
                            return listRecipients;
                        }
                        break;
                    case SupportStructures.BuildingOptionsRecipients.S_PRODUCER_EVENT:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PRODUCER_EVENT");
                        q.setParam("idEvent", e.SYSTEM_ID.ToString());
                        queryString = q.getSQL();
                        if (!this.ExecuteQuery(out ds, "Recipients", q.getSQL()))
                        {
                            return listRecipients;
                        }
                        break;
                    case SupportStructures.BuildingOptionsRecipients.USERS_ROLE_PRODUCER:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_USERS_ROLE_PRODUCER");
                        q.setParam("idEvent", e.SYSTEM_ID.ToString());
                        queryString = q.getSQL();
                        if (!this.ExecuteQuery(out ds, "Recipients", q.getSQL()))
                        {
                            return listRecipients;
                        }
                        break;
                    case SupportStructures.BuildingOptionsRecipients.USERS_ROLE_IN_PROCESS:
                        listRecipients = GetRecipientsUserInProcess(e.TYPE_EVENT, e.DTA_EVENT, idObject);
                        break;
                    case SupportStructures.BuildingOptionsRecipients.USER_RECIPIENT_TASK:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_USER_RECIPIENT_TASK");
                        q.setParam("idSpecializedObject", e.ID_SPECIALIZED_OBJECT.ToString());
                        queryString = q.getSQL();
                        if (!this.ExecuteQuery(out ds, "Recipients", q.getSQL()))
                        {
                            return listRecipients;
                        }
                        break;
                }
                this.CreateRecipientstObjects(ds, ref listRecipients);
            }
            catch (Exception exc)
            {
                // traccia l'eccezione nel file di log
                logger.Error(exc);
            }

            return listRecipients;
        }

        /// <summary>
        ///Builds the list of recipients from the dataset to be processed
        /// </summary>
        public void CreateRecipientstObjects(DataSet ds, ref List<Recipient> listRecipients)
        {
            try
            {
                if (ds.Tables["Recipients"] != null && ds.Tables["Recipients"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["Recipients"].Rows)
                    {
                        Recipient recipient = new Recipient(Utils.ConvertField(row["ID_GRUPPO"].ToString()),
                                                Utils.ConvertField(row["ID_PEOPLE"].ToString()));
                        listRecipients.Add(recipient);
                    }
                }
            }
            catch (Exception exc)
            {
                // traccia l'eccezione nel file di log
                logger.Error(exc);
                listRecipients = new List<Recipient>();
                throw exc;
            }
        }

        /// <summary>
        /// Estra gli utenti coinvolti nel processo che devono essere notificati
        /// </summary>
        /// <param name="typeEvent"></param>
        /// <param name="dateEvent"></param>
        /// <param name="idObject"></param>
        /// <returns></returns>
        public List<Recipient> GetRecipientsUserInProcess(string typeEvent, string dateEvent, string idObject)
        {
            List<Recipient> listRecipients = new List<Recipient>();
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q;
                string queryString = string.Empty;
                string tipoEvento = Utils.GetTypeEvent(typeEvent);
                string conditionNotification = string.Empty;
                string statoPasso = string.Empty;
                #region RUOLO TITOLARE
                //Notifica al titolare del passo;
                //La notifica viene inviata all'utente che ha effettuato l'operazione; se invece il passo è automatico invio a tutto il ruolo
                switch (tipoEvento)
                {
                    case SupportStructures.EventType.TRONCAMENTO_PROCESSO:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_USERS_ROLE_HOLDER_IN_PROCESS_ACTIVE");
                        break;
                    case SupportStructures.EventType.PROCESSO_FIRMA:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_USERS_ROLE_HOLDER_PASSO_IN_ERRORE");
                        if (typeEvent.Equals(SupportStructures.EventType.PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO))
                            statoPasso = " AND PA.STATO_PASSO = 'LOOK'";
                        break;
                    default:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_USERS_ROLE_HOLDER_IN_PROCESS");
                        break;
                }

                q.setParam("typeEvent", typeEvent.ToString());
                q.setParam("dateEvent", DocsPaDbManagement.Functions.Functions.ToDate(dateEvent));
                q.setParam("idObject", idObject.ToString());
                q.setParam("statoPasso", statoPasso);

                queryString = q.getSQL();
                if (this.ExecuteQuery(out ds, "Recipients", queryString))
                {
                    if (ds.Tables["Recipients"] != null && ds.Tables["Recipients"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["Recipients"].Rows)
                        {
                            Recipient recipient = new Recipient(Utils.ConvertField(row["ID_GRUPPO"].ToString()),
                                                    Utils.ConvertField(row["ID_PEOPLE"].ToString()));
                            bool isPresent = (from r in listRecipients
                                              where r.ID_ROLE.Equals(recipient.ID_ROLE) && r.ID_USER.Equals(recipient.ID_USER)
                                              select r).FirstOrDefault() != null;
                            if (!isPresent)
                                listRecipients.Add(recipient);
                        }
                    }
                }
                #endregion

                #region RUOLO PROPONENTE
                //Estraggo il ruolo avviatore per la notifica
                switch (tipoEvento)
                {
                    case SupportStructures.EventType.TRONCAMENTO_PROCESSO:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_USER_ROLE_PROPONENT_IN_PROCESS_ACTIVE");
                        break;
                    case SupportStructures.EventType.PROCESSO_FIRMA:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_USER_ROLE_PROPONENT_IN_PROCESS_ACTIVE");
                        if (typeEvent.Equals(SupportStructures.EventType.PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO))
                            conditionNotification = "AND NOTIFICA_ERRORE = '1'";
                        if (typeEvent.Equals(SupportStructures.EventType.PROCESSO_FIRMA_DESTINATARI_NON_INTEROP))
                            conditionNotification = "AND NOTIFICA_DEST_NON_INTEROP = '1'";
                        break;
                    case SupportStructures.EventType.CONCLUSIONE_PROCESSO:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_USER_ROLE_PROPONENT_IN_PROCESS");
                        conditionNotification = " NOTIFICA_CONCLUSO = '1'";
                        break;
                    case SupportStructures.EventType.INTERROTTO_PROCESSO:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_USER_ROLE_PROPONENT_IN_PROCESS");
                        conditionNotification = " NOTIFICA_INTERROTTO = '1'";
                        break;
                }
                q.setParam("conditionNotification", conditionNotification);
                q.setParam("dateEvent", DocsPaDbManagement.Functions.Functions.ToDate(dateEvent));
                q.setParam("idObject", idObject.ToString());
                queryString = q.getSQL();
                if (this.ExecuteQuery(out ds, "Recipients", q.getSQL()))
                {
                    if (ds.Tables["Recipients"] != null && ds.Tables["Recipients"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["Recipients"].Rows)
                        {
                            Recipient recipient = new Recipient(Utils.ConvertField(row["ID_GRUPPO"].ToString()),
                                                    Utils.ConvertField(row["ID_PEOPLE"].ToString()));
                            bool isPresent = (from r in listRecipients
                                              where r.ID_ROLE.Equals(recipient.ID_ROLE) && r.ID_USER.Equals(recipient.ID_USER)
                                              select r).FirstOrDefault() != null;
                            if (!isPresent)
                                listRecipients.Add(recipient);
                        }
                    }
                }
                #endregion

                #region MONITORATORE
                conditionNotification = string.Empty;
                switch (tipoEvento)
                {
                    case SupportStructures.EventType.PROCESSO_FIRMA:
                        if (typeEvent.Equals(SupportStructures.EventType.PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO))
                            conditionNotification = " AND NOTIFICA_ERRORE = '1'";
                        if (typeEvent.Equals(SupportStructures.EventType.PROCESSO_FIRMA_DESTINATARI_NON_INTEROP))
                            conditionNotification = " AND NOTIFICA_DEST_NON_INTEROP = '1'";
                        break;
                    case SupportStructures.EventType.CONCLUSIONE_PROCESSO:
                        conditionNotification = " AND NOTIFICA_CONCLUSO = '1'";
                        break;
                    case SupportStructures.EventType.INTERROTTO_PROCESSO:
                        conditionNotification = " AND NOTIFICA_INTERROTTO = '1'";
                        break;
                }
                //Qualunque notifica va anche notificata al monitoratore del processo fatta eccezione di quella dei destinatari interoperanti
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ROLE_MONITORATORE_PROCESS");
                q.setParam("dateEvent", DocsPaDbManagement.Functions.Functions.ToDate(dateEvent));
                q.setParam("idObject", idObject.ToString());
                q.setParam("conditionNotification", conditionNotification);
                queryString = q.getSQL();
                if (this.ExecuteQuery(out ds, "Recipients", q.getSQL()))
                {
                    if (ds.Tables["Recipients"] != null && ds.Tables["Recipients"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["Recipients"].Rows)
                        {
                            Recipient recipient = new Recipient(Utils.ConvertField(row["ID_GRUPPO"].ToString()), Utils.ConvertField(row["ID_PEOPLE"].ToString()));
                            bool isPresent = (from r in listRecipients
                                              where r.ID_ROLE.Equals(recipient.ID_ROLE) && r.ID_USER.Equals(recipient.ID_USER)
                                              select r).FirstOrDefault() != null;
                            if (!isPresent)
                                listRecipients.Add(recipient);
                        }
                    }
                }
                #endregion
            }
            catch (Exception exc)
            {
                // traccia l'eccezione nel file di log
                File.AppendAllText("C:\\ServiceNotification\\ErrorTrace.txt", "---------------------\nException in " +
                    ConfigurationManager.AppSettings["name_service"] + "\n" +
                    exc.StackTrace + "\n\n" + exc.Message + "\n---------------------\n\n");

                listRecipients = new List<Recipient>();
                throw exc;
            }

            return listRecipients;
        }
    }
}
