using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.NotificationCenter;
using System.Data;
using System.Configuration;
using System.IO;
using log4net;

namespace DocsPaDB.Query.SpecializedItem
{
    public class SpecializedItemFollowExtApp : DBProvider, ISpecializedItem
    {
        private static ILog logger = LogManager.GetLogger(typeof(SpecializedItemFollowExtApp));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public string CreateSpecializedItem(Event e)
        {
            string specializedItem = string.Empty;

            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q;
                string queryString;
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ITEMS_SPECIALIZED_FOLLOW_EXT_APP");
                q.setParam("idEvent", e.SYSTEM_ID.ToString());
                queryString = q.getSQL();
                this.ExecuteQuery(out ds, "SpecializedItem", q.getSQL());
                CreateSpecializedItemObject(ds, ref specializedItem);
            }
            catch (Exception exc) 
            {
                // traccia l'eccezione nel file di log
                logger.Error(exc);
            }
            return specializedItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="specializedItem"> </param>
        public void CreateSpecializedItemObject(DataSet ds, ref string specializedItem)
        {
            try
            {
                System.Text.StringBuilder strbuilder = new StringBuilder();
                if (ds.Tables["SpecializedItem"] != null && ds.Tables["SpecializedItem"].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables["SpecializedItem"].Rows[0];
                    if (!string.IsNullOrEmpty(dr["VAR_DESC_OGGETTO"].ToString()))
                    {
                        strbuilder.Append(dr["VAR_DESC_OGGETTO"].ToString());
                    }
                }
                specializedItem = strbuilder.ToString();
            }
            catch (Exception exc) 
            {
                // traccia l'eccezione nel file di log
                logger.Error(exc);
            }
        }

    }
}
