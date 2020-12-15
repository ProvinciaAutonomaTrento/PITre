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
    public class SpecializedItemIS : DBProvider, ISpecializedItem
    {

        private static ILog logger = LogManager.GetLogger(typeof(SpecializedItemIS));
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
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ITEMS_SPECIALIZED_IS");
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
                    if (!string.IsNullOrEmpty(dr["DESC_OBJECT"].ToString()))
                    {
                        strbuilder.Append(SupportStructures.TagItem.LINE + SupportStructures.TagItem.LABEL +
                           "lblObjectDescription" + SupportStructures.TagItem.CLOSE_LABEL + dr["DESC_OBJECT"].ToString() + SupportStructures.TagItem.CLOSE_LINE);
                   
                    }
                    if (!string.IsNullOrEmpty(dr["DESC_SENDER"].ToString()))
                    {
                        strbuilder.Append(SupportStructures.TagItem.LINE + SupportStructures.TagItem.LABEL +
                            "lblSender" + SupportStructures.TagItem.CLOSE_LABEL + dr["DESC_SENDER"].ToString() + SupportStructures.TagItem.CLOSE_LINE);
                    }
                    if (!string.IsNullOrEmpty(dr["TIPO_ATTO"].ToString()))
                    {
                        strbuilder.Append(SupportStructures.TagItem.LINE + SupportStructures.TagItem.LABEL +
                            "lblDocType" + SupportStructures.TagItem.CLOSE_LABEL + dr["TIPO_ATTO"].ToString() + SupportStructures.TagItem.CLOSE_LINE);
                    }
                    if (!string.IsNullOrEmpty(dr["DETAILS"].ToString()))
                    {
                        string [] tmp = dr["DETAILS"].ToString().Split(new string[]{"<br/>"}, StringSplitOptions.None );
                        tmp[0] = tmp[0].Split(new string[] { ":" }, 2, StringSplitOptions.None)[1].Trim();
                        tmp[1] = tmp[1].Split(new string[] { ":" }, 2, StringSplitOptions.None)[1].Trim();
                        strbuilder.Append(SupportStructures.TagItem.LINE + SupportStructures.TagItem.LABEL +
                            "lblDetailReceivedIS" + SupportStructures.TagItem.CLOSE_LABEL + tmp[0] + SupportStructures.TagItem.CLOSE_LINE +
                            SupportStructures.TagItem.LINE + SupportStructures.TagItem.LABEL +
                            "lblSendRecipientIS" + SupportStructures.TagItem.CLOSE_LABEL + tmp[1] + SupportStructures.TagItem.CLOSE_LINE);
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
