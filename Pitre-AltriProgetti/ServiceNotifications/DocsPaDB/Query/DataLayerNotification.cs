using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DocsPaVO.NotificationCenter;
using DocsPaDB.Query.SpecializedItem;
using System.Configuration;
using System.IO;
using log4net;

namespace DocsPaDB.Query 
{
    public class DataLayerNotification : DBProvider
    {
        private static ILog logger = LogManager.GetLogger(typeof(DataLayerNotification));
        /// <summary>
        /// Returns the items of notification
        /// </summary>
        /// <returns></returns>
        public Items CreateItems(Event e)
        {
            Items items = new Items();
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q;
                switch (e.DOMAIN_OBJECT)
                { 
                    case SupportStructures.ListDomainObject.DOCUMENT:
                    case SupportStructures.ListDomainObject.ALLEGATO:
                         q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ITEMS_DOCUMENT");
                         q.setParam("idAmm", e.ID_ADM.ToString());
                         items = new ItemsDocument();
                        break;
                    case SupportStructures.ListDomainObject.FASCICOLO:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ITEMS_FOLDER");
                        items = new ItemsFolder();
                        break;
                    default:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ITEMS_JOB");
                        items = new Items();
                        break;
                }
                q.setParam("systemId", e.ID_OBJECT.ToString());
                string queryString = q.getSQL();
                this.ExecuteQuery(out ds, "Items", q.getSQL());
                CreateItemsObject(ds, ref items);

            }
            catch (Exception exc)
            {
                // traccia l'eccezione nel file di log
                logger.Error(exc);
            }
            return items;
        }

        /// <summary>
        /// Returns the specialized content of notification
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public string CreateSpecializedItem(Event e)
        {
            string specializedItem = string.Empty;
            try
            {
                switch (Utils.GetTypeEvent(e.TYPE_EVENT))
                { 
                    case SupportStructures.EventType.TRASM:
                        specializedItem = new SpecializedItemTransmission().CreateSpecializedItem(e);
                        break;
                    case SupportStructures.EventType.ACCEPT_TRASM:
                        specializedItem = new SpecializedItemAcceptTrasm().CreateSpecializedItem(e);
                        break;
                    case SupportStructures.EventType.REJECT_TRASM:
                        specializedItem = new SpecializedItemRejectTrasm().CreateSpecializedItem(e);
                        break;
                    case SupportStructures.EventType.CHECK_TRASM:
                    case SupportStructures.EventType.MODIFIED_OBJECT_PROTO:
                    case SupportStructures.EventType.RECORD_PREDISPOSED:
                        specializedItem = new SpecializedItemBasicInfoDoc().CreateSpecializedItem(e);
                        break;
                    case SupportStructures.EventType.DOC_CAMBIO_STATO:
                        specializedItem = new SpecializedItemChangeState().CreateSpecializedItem(e);
                        break;
                    case SupportStructures.EventType.DOCUMENTOCONVERSIONEPDF:
                        specializedItem = new SpecializedItemConvertPdf().CreateSpecializedItem(e);
                        break;
                    case SupportStructures.EventType.NO_DELIVERY_SEND_SIMPLIFIED_INTEROPERABILITY:
                    case SupportStructures.EventType.EXCEPTION_SEND_SIMPLIFIED_INTEROPERABILITY:
                        specializedItem = new SpecializedItemIS().CreateSpecializedItem(e);
                        break;
                    // Modifica PEC 4 requisito 2
                    case SupportStructures.EventType.NO_DELIVERY_SEND_PEC:
                    // Distinzione della notifica di eccezione. Eredito il comportamento della no_delivery send pec.
                    case SupportStructures.EventType.EXCEPTION_INTEROPERABILITY_PEC:
                        specializedItem = new SpecializedItemPEC().CreateSpecializedItem(e);
                        break;
                    case SupportStructures.EventType.ANNULLA_PROTO:
                        specializedItem = new SpecializedItemAbortRecord().CreateSpecializedItem(e);
                        break;
                    case SupportStructures.EventType.FOLLOW_DOC_EXT_APP:
                    case SupportStructures.EventType.FOLLOW_FASC_EXT_APP:
                        specializedItem = new SpecializedItemFollowExtApp().CreateSpecializedItem(e);
                        break;
                    //Mev istanza di accesso
                    case SupportStructures.EventType.CREATED_FILE_ZIP_INSTANCE_ACCESS:
                    case SupportStructures.EventType.FAILED_CREATING_FILE_ZIP_INSTANCE_ACCESS:
                        specializedItem = new SpecializedItemInstanceAccess().CreateSpecializedItem(e);
                        break;
                    //MEV LIBRO FIRMA
                    case SupportStructures.EventType.INTERROTTO_PROCESSO:
                    case SupportStructures.EventType.CONCLUSIONE_PROCESSO:
                    case SupportStructures.EventType.TRONCAMENTO_PROCESSO:
                    case SupportStructures.EventType.PROCESSO_FIRMA:
                        specializedItem = new SpecializedItemSignatureProcesses().CreateSpecializedItem(e);
                        break;
                    default:
                        specializedItem = new SpecializedItemGeneric().CreateSpecializedItem(e);
                        break;
                }
            }
            catch (Exception exc)
            {
                // traccia l'eccezione nel file di log
                logger.Error(exc);
            }
            return specializedItem;
        }

        /// <summary>
        /// Builds the list of recipients from the dataset to be processed
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="listEvent"></param>
        public void CreateItemsObject(DataSet ds, ref Items items)
        {
            try
            {
                if (ds.Tables["Items"] != null && ds.Tables["Items"].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables["Items"].Rows[0];
                    items.ID_OBJECT = !string.IsNullOrEmpty(dr["ID_OBJECT"].ToString()) ? dr["ID_OBJECT"].ToString() : string.Empty;
                    items.ITEM_DESCRIPTION = !string.IsNullOrEmpty(dr["ITEM_DESCRIPTION"].ToString()) ? dr["ITEM_DESCRIPTION"].ToString() : string.Empty;
                    if (items.GetType() == typeof(ItemsDocument))
                    {
                        (items as ItemsDocument).SEGNATURA = !string.IsNullOrEmpty(dr["SEGNATURA"].ToString()) ? SupportStructures.TagItem.COLORRED + 
                            dr["SEGNATURA"].ToString() + SupportStructures.TagItem.CLOSE_COLORRED : string.Empty;
                        
                        if (!string.IsNullOrEmpty(dr["NUM_REPERTORIO"].ToString()))
                        {
                            string[] split = dr["NUM_REPERTORIO"].ToString().Split(new string[] { "#" }, StringSplitOptions.None);
                            if (split[split.Length-1].Equals("1"))
                            {

                                (items as ItemsDocument).LIST_NUMBER = SupportStructures.TagItem.LABEL +
                                    "lblRepertorio" + SupportStructures.TagItem.CLOSE_LABEL + SupportStructures.TagItem.COLORRED_STRIKE +
                                    dr["NUM_REPERTORIO"].ToString().Substring(0, dr["NUM_REPERTORIO"].ToString().Length - 2) + SupportStructures.TagItem.CLOSE_COLORRED_STRIKE;
                            }
                            else
                            {
                                (items as ItemsDocument).LIST_NUMBER = SupportStructures.TagItem.LABEL +
                                    "lblRepertorio" + SupportStructures.TagItem.CLOSE_LABEL + SupportStructures.TagItem.COLORRED +
                                    dr["NUM_REPERTORIO"].ToString().Substring(0, dr["NUM_REPERTORIO"].ToString().Length - 2) + SupportStructures.TagItem.CLOSE_COLORRED;
                            }
                        }
                        else
                        {
                            (items as ItemsDocument).LIST_NUMBER = string.Empty;
                        }
                        
                        string typeProto = string.Empty;
                        switch (dr["TYPE_PROTO"].ToString())
                        {
                            case SupportStructures.TypeProtocol.ARRIVO:
                                typeProto = " (" + SupportStructures.TagItem.LABEL + SupportStructures.TypeProtocol.LABEL_ARRIVO + 
                                    SupportStructures.TagItem.CLOSE_LABEL + ")";
                                break;
                            case SupportStructures.TypeProtocol.PARTENZA:
                                typeProto = " (" +  SupportStructures.TagItem.LABEL + 
                                SupportStructures.TypeProtocol.LABEL_PARTENZA +  SupportStructures.TagItem.CLOSE_LABEL + ")";
                                break;
                            case SupportStructures.TypeProtocol.INTERNO:
                                typeProto = " (" + SupportStructures.TagItem.LABEL +
                                SupportStructures.TypeProtocol.LABEL_INTERNO + SupportStructures.TagItem.CLOSE_LABEL + ")";
                                break;
                            case SupportStructures.TypeProtocol.GRIGIO:
                                typeProto = " (" + SupportStructures.TagItem.LABEL + 
                                    SupportStructures.TypeProtocol.LABEL_GRIGIO + SupportStructures.TagItem.CLOSE_LABEL + ")";
                                break;
                            case SupportStructures.TypeProtocol.STAMPAREG:
                                typeProto = " (" + SupportStructures.TagItem.LABEL + SupportStructures.TypeProtocol.LABEL_STAMPAREG + 
                                    SupportStructures.TagItem.CLOSE_LABEL + ")";
                                break;
                        }
                        if (!string.IsNullOrEmpty(dr["SEGNATURA"].ToString()))
                        {
                            (items as ItemsDocument).SEGNATURA += typeProto;
                        }
                        else
                        {
                            items.ID_OBJECT += typeProto; 
                        }
                    }
                    else if (items.GetType() == typeof(ItemsFolder))
                    {
                        items.ID_OBJECT = !string.IsNullOrEmpty(dr["ID_OBJECT"].ToString()) ? dr["ID_OBJECT"].ToString() : string.Empty;
                        items.ITEM_DESCRIPTION = !string.IsNullOrEmpty(dr["ITEM_DESCRIPTION"].ToString()) ? dr["ITEM_DESCRIPTION"].ToString() : string.Empty;
                    }
                }
            }
            catch (Exception exc)
            {
                // traccia l'eccezione nel file di log
                logger.Error(exc);
            }
        }

        /// <summary>
        /// Writes the notifications associated with the event.
        /// </summary>
        /// <param name="e"></param>
        /// <returns>Returns true if the notifications have been properly generated false otherwise</returns>
        public bool WriteEventNotifications(Event e)
        {
            bool res = true;
            string castDate = ConfigurationManager.AppSettings["CAST_DATE"];
            try
            {
                if (e.NOTIFICATIONS != null && e.NOTIFICATIONS.Count > 0)
                {
                    this.BeginTransaction();
                    foreach (Notification notification in e.NOTIFICATIONS)
                    {
                        DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_NOTIFY");
                        System.Text.StringBuilder strBuilder = new StringBuilder();
                        if (DBType.ToUpper().Equals("ORACLE"))
                            strBuilder.Append("seq.nextval,");
                        strBuilder.Append(e.SYSTEM_ID.ToString() + ",");
                        strBuilder.Append("'" + e.ACTORS.PRODUCER.DES_PRODUCER.Replace("'", "''") + "',");
                        strBuilder.Append(notification.RECIPIENT.ID_USER.ToString() + ",");
                        strBuilder.Append(notification.RECIPIENT.ID_ROLE.ToString() + ",");
                        strBuilder.Append("'" + notification.TYPE_NOTIFY + "',");
                        strBuilder.Append(DocsPaDbManagement.Functions.Functions.GetDate() + ",");
                        if (notification.ITEMS.GetType() == typeof(ItemsDocument))
                        {

                            strBuilder.Append("'" + (notification.ITEMS as ItemsDocument).ID_OBJECT.Replace("'", "''") + "',");
                            strBuilder.Append("'" + (notification.ITEMS as ItemsDocument).SEGNATURA.Replace("'", "''") + "',");
                            strBuilder.Append("'" + (notification.ITEMS as ItemsDocument).ITEM_DESCRIPTION.Replace("'", "''").Replace("°", DocsPaDbManagement.Functions.Functions.convertDegre()) + "',");
                            strBuilder.Append("'" + (notification.ITEMS as ItemsDocument).LIST_NUMBER.Replace("'", "''") + "',");
                        }
                        else if (notification.ITEMS.GetType() == typeof(ItemsFolder))
                        {
                            strBuilder.Append("'" + (notification.ITEMS as ItemsFolder).ID_OBJECT.Replace("'", "''") + "',");
                            strBuilder.Append("null,");
                            strBuilder.Append("'" + (notification.ITEMS as ItemsFolder).ITEM_DESCRIPTION.Replace("'", "''").Replace("°", DocsPaDbManagement.Functions.Functions.convertDegre()) + "',");
                            strBuilder.Append("null,");
                        }
                        strBuilder.Append("'" + e.MULTIPLICITY + "',");
                        strBuilder.Append("'" + notification.ITEMS.ITEM_SPECIALIZED.Replace("'", "''").Replace("°", DocsPaDbManagement.Functions.Functions.convertDegre()) + "',");
                        strBuilder.Append("'" + e.TYPE_EVENT + "',");
                        strBuilder.Append("'" + e.DOMAIN_OBJECT + "',");
                        strBuilder.Append(e.ID_OBJECT + ",");
                        strBuilder.Append(e.ID_SPECIALIZED_OBJECT + ",");
                        strBuilder.Append( DocsPaDbManagement.Functions.Functions.ToDate(e.DTA_EVENT) + ",");
                        strBuilder.Append("'0',");
                        strBuilder.Append("'" + e.COLOR + "',");
                        strBuilder.Append("null");
                        q.setParam("value", strBuilder.ToString());
                        string query = q.getSQL();

                        if (!this.ExecuteNonQuery(query))
                        {
                            this.RollbackTransaction();
                            CloseConnection();
                            return false;
                        }
                    }
                    this.CommitTransaction();
                    CloseConnection();
                }
            }
            catch (Exception exc)
            {
                // traccia l'eccezione nel file di log
                logger.Error(exc);
                this.RollbackTransaction();
                CloseConnection();
                res = false;
            }

            return res;
        }
    }
}
