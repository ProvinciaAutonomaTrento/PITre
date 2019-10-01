using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.NotificationCenter;
using System.Data;
using System.Collections;
using System.IO;
using System.Configuration;
using log4net;

namespace DocsPaDB.Query.SpecializedItem
{
    public class SpecializedItemTransmission : DBProvider, ISpecializedItem
    {
        private static ILog logger = LogManager.GetLogger(typeof(SpecializedItemTransmission));
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
                string sender = string.Empty;
                switch (e.DOMAIN_OBJECT)
                {
                    case SupportStructures.ListDomainObject.DOCUMENT:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ITEMS_SPECIALIZED_TRANSMISSION_DOCUMENT");
                        q.setParam("idTrasmSing", e.ID_SPECIALIZED_OBJECT.ToString());
                        queryString = q.getSQL();
                        this.ExecuteQuery(out ds, "SpecializedItem", q.getSQL());
                        break;
                    case SupportStructures.ListDomainObject.FASCICOLO:
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ITEMS_SPECIALIZED_TRANSMISSION_FOLDER");
                        q.setParam("idTrasmSing", e.ID_SPECIALIZED_OBJECT.ToString());
                        queryString = q.getSQL();
                        this.ExecuteQuery(out ds, "SpecializedItemFolder", q.getSQL());
                        break;
                }
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
                DataRow dr = null;
                bool isFolder = false;
                if (ds.Tables["SpecializedItem"] != null && ds.Tables["SpecializedItem"].Rows.Count > 0)
                {
                   dr = ds.Tables["SpecializedItem"].Rows[0];
                }
                else if(ds.Tables["SpecializedItemFolder"] != null && ds.Tables["SpecializedItemFolder"].Rows.Count > 0)
                {
                    isFolder = true;
                    dr = ds.Tables["SpecializedItemFolder"].Rows[0];
                }
                if(dr != null)
                {
                    if (!string.IsNullOrEmpty(dr["DESC_OBJECT"].ToString()))
                    {
                        strbuilder.Append(SupportStructures.TagItem.LINE + SupportStructures.TagItem.LABEL +
                           "lblObjectDescription" + SupportStructures.TagItem.CLOSE_LABEL + dr["DESC_OBJECT"].ToString() + SupportStructures.TagItem.CLOSE_LINE);
                   
                    }
                    if (!isFolder && !string.IsNullOrEmpty(dr["DESC_SENDER"].ToString()))
                    {
                        strbuilder.Append(SupportStructures.TagItem.LINE + SupportStructures.TagItem.LABEL +
                            "lblSender" + SupportStructures.TagItem.CLOSE_LABEL + dr["DESC_SENDER"].ToString() + SupportStructures.TagItem.CLOSE_LINE);
                    }
                    if (!string.IsNullOrEmpty(dr["NOTE_GENERALI"].ToString()))
                    {
                        //le note generali vengono troncate a 1000 caratteri
                        if (dr["NOTE_GENERALI"].ToString().Length > 1000)
                        {
                            dr["NOTE_GENERALI"] = dr["NOTE_GENERALI"].ToString().Substring(0, 1400) + "...";
                        }
                        strbuilder.Append(SupportStructures.TagItem.LINE + SupportStructures.TagItem.LABEL +
                            "lblGeneralNote" + SupportStructures.TagItem.CLOSE_LABEL + dr["NOTE_GENERALI"].ToString() + SupportStructures.TagItem.CLOSE_LINE);
                    }
                    if (!string.IsNullOrEmpty(dr["NOTE_SINGOLE"].ToString()))
                    {
                        //le note individuali vengono troncate a 1000 caratteri
                        if (dr["NOTE_SINGOLE"].ToString().Length > 500)
                        {
                            dr["NOTE_SINGOLE"] = dr["NOTE_SINGOLE"].ToString().Substring(0, 250) + "...";
                        }
                        strbuilder.Append(SupportStructures.TagItem.LINE + SupportStructures.TagItem.LABEL +
                            "lblIndividualNote" + SupportStructures.TagItem.CLOSE_LABEL + dr["NOTE_SINGOLE"].ToString() +
                            SupportStructures.TagItem.CLOSE_LINE);
                    }
                    if (!string.IsNullOrEmpty(dr["DTA_SCADENZA"].ToString()))
                    {
                        strbuilder.Append(SupportStructures.TagItem.LINE + SupportStructures.TagItem.LABEL +
                            "lblExpiration" + SupportStructures.TagItem.CLOSE_LABEL + 
                            Convert.ToDateTime(dr["DTA_SCADENZA"].ToString()).ToShortDateString() +
                            SupportStructures.TagItem.CLOSE_LINE);
                    }
                    if (!isFolder)
                    {
                        if (!string.IsNullOrEmpty(dr["TIPO_ATTO"].ToString()))
                        {
                            strbuilder.Append(SupportStructures.TagItem.LINE + SupportStructures.TagItem.LABEL +
                                "lblDocType" + SupportStructures.TagItem.CLOSE_LABEL + dr["TIPO_ATTO"].ToString() + SupportStructures.TagItem.CLOSE_LINE);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dr["TIPO_FASC"].ToString()))
                        {
                            strbuilder.Append(SupportStructures.TagItem.LINE + SupportStructures.TagItem.LABEL +
                                "lblFascType" + SupportStructures.TagItem.CLOSE_LABEL + dr["TIPO_FASC"].ToString() + SupportStructures.TagItem.CLOSE_LINE);
                        }
                    }
                    specializedItem = strbuilder.ToString();
                }
            }
            catch (Exception exc) 
            {
                // traccia l'eccezione nel file di log
                logger.Error(exc);
            }
        }

    }
}
