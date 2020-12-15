using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DocsPaVO.NotificationCenter;
using System.Configuration;
using System.IO;
using log4net;

namespace DocsPaDB.Query.SpecializedItem
{
    class SpecializedItemPEC : DBProvider, ISpecializedItem
    {
        private static ILog logger = LogManager.GetLogger(typeof(SpecializedItemPEC));
        // Modifica PEC 4, requisito 2

        public string CreateSpecializedItem(Event e)
        {

            string specializedItem = string.Empty;

            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q;
                string queryString;
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ITEMS_SPECIALIZED_PEC");
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

        public void CreateSpecializedItemObject(DataSet ds, ref string specializedItem)
        {
            try
            {
                System.Text.StringBuilder strbuilder = new StringBuilder();
                if (ds.Tables["SpecializedItem"] != null && ds.Tables["SpecializedItem"].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables["SpecializedItem"].Rows[0];
                    if (!string.IsNullOrEmpty(dr["DESC_OGGETTO"].ToString()))
                    {
                        strbuilder.Append(SupportStructures.TagItem.LINE + SupportStructures.TagItem.LABEL +
                           "lblObjectDescription" + SupportStructures.TagItem.CLOSE_LABEL + dr["DESC_OGGETTO"].ToString() + SupportStructures.TagItem.CLOSE_LINE);
                   
                    }
                    if (!string.IsNullOrEmpty(dr["DESC_SENDER"].ToString()))
                    {
                        strbuilder.Append(SupportStructures.TagItem.LINE + SupportStructures.TagItem.LABEL +
                            "lblSender" + SupportStructures.TagItem.CLOSE_LABEL + dr["DESC_SENDER"].ToString() + SupportStructures.TagItem.CLOSE_LINE);
                    }
                    if (!string.IsNullOrEmpty(dr["DESC_AZIONELOG"].ToString()))
                    {
                        strbuilder.Append(SupportStructures.TagItem.LINE + dr["DESC_AZIONELOG"].ToString() + SupportStructures.TagItem.CLOSE_LINE);
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
