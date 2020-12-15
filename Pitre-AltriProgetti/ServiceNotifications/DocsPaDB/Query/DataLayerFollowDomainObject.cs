using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.NotificationCenter;
using System.Data;
using System.Configuration;
using System.IO;
using log4net;

namespace DocsPaDB.Query
{
    public class DataLayerFollowDomainObject : DBProvider
    {

        private static ILog logger = LogManager.GetLogger(typeof(DataLayerFollowDomainObject));
        /// <summary>
        /// Returns the list of recipients who have decided to follow the object with id idObject.
        /// </summary>
        /// <param name="idObject"></param>
        /// <param name="domainObject"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public List<RecipientFollowDomainObject> GetRecipientsFollowDomainObject(string idObject, string domainObject, string idAmm)
        {
            List<RecipientFollowDomainObject> listRecipients = new List<RecipientFollowDomainObject>();
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_FOLLOW_OBJECT");
                q.setParam("idObject", idObject);
                q.setParam("domainObject", domainObject);
                q.setParam("idAmm", idAmm);
                string queryString = q.getSQL();
                if (!this.ExecuteQuery(out ds, "Recipients", q.getSQL()))
                {
                    return listRecipients;
                }
                this.CreateRecipientFollowDomainObj(ds, ref listRecipients);
            }
            catch (Exception exc)
            {
                // traccia l'eccezione nel file di log
                logger.Error(exc);
            }
            return listRecipients;
        }

        private void CreateRecipientFollowDomainObj(DataSet ds, ref List<RecipientFollowDomainObject> listRecipients)
        { 
            try
            {
                if (ds.Tables["Recipients"] != null && ds.Tables["Recipients"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["Recipients"].Rows)
                    {
                        DataRow dr = row;
                        RecipientFollowDomainObject recipient = new RecipientFollowDomainObject()
                        {
                            IDUSER = Utils.ConvertField(dr["ID_PEOPLE_FOLLOW"].ToString()),
                            IDROLE = Utils.ConvertField(dr["ID_ROLE_FOLLOW"].ToString()),
                            APPLICATION = (string.IsNullOrEmpty(dr["APP"].ToString()) ||
                                            dr["APP"].ToString().Equals(RecipientFollowDomainObject.INTERNAL)) ?
                                RecipientFollowDomainObject.INTERNAL : 
                                RecipientFollowDomainObject.EXTERNAL
                        };
                        listRecipients.Add(recipient);
                    }
                }
            }
            catch(Exception exc)
            {
                // traccia l'eccezione nel file di log
                logger.Error(exc);
            }
        }

    }
}