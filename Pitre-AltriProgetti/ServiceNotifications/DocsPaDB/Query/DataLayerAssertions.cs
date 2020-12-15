using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.NotificationCenter;
using System.Data;
using System.IO;
using System.Configuration;
using log4net;

namespace DocsPaDB.Query
{
    public class DataLayerAssertions : DBProvider
    {
        private static ILog logger = LogManager.GetLogger(typeof(DataLayerAssertions));
        public List<Aggregator> ListAggregatorRole(string role)
        {
            List<Aggregator> listAggregator = new List<Aggregator>();
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AGGREGATORS_ROLE");
                q.setParam("idRole", role);
                string queryString = q.getSQL();
                if (this.ExecuteQuery(out ds, "AggregatorRole", q.getSQL()))
                {
                    CreateObjectAggregatorRole(ds, ref listAggregator);
                }
            }
            catch (Exception exc)
            {
                // traccia l'eccezione nel file di log
                logger.Error(exc);
            }
            return listAggregator;
        }

        private void CreateObjectAggregatorRole(DataSet ds, ref List<Aggregator> listAggregator)
        {
            if (ds.Tables["AggregatorRole"] != null && ds.Tables["AggregatorRole"].Rows.Count > 0)
            {
                DataRow row = ds.Tables["AggregatorRole"].Rows[0];
                Aggregator aggregator = new Aggregator() { 
                    IDAUR = !string.IsNullOrEmpty(row["ID_AMM"].ToString()) ? row["ID_AMM"].ToString() : "0",
                    TYPEAUR = SupportStructures.TypeAggregator.AMM 
                };
                listAggregator.Add(aggregator);
                
                aggregator = new Aggregator()
                {
                    IDAUR = !string.IsNullOrEmpty(row["ID_TIPO_RUOLO"].ToString()) ? row["ID_TIPO_RUOLO"].ToString() : "0",
                    TYPEAUR = SupportStructures.TypeAggregator.TR
                };
                listAggregator.Add(aggregator);

                aggregator = new Aggregator()
                {
                    IDAUR = !string.IsNullOrEmpty(row["ID_UO"].ToString()) ? row["ID_UO"].ToString() : "0",
                    TYPEAUR = SupportStructures.TypeAggregator.UO
                };
                listAggregator.Add(aggregator);

                aggregator = new Aggregator()
                {
                    IDAUR = !string.IsNullOrEmpty(row["ID_GRUPPO"].ToString()) ? row["ID_GRUPPO"].ToString() : "0",
                    TYPEAUR = SupportStructures.TypeAggregator.R
                };
                listAggregator.Add(aggregator);

                aggregator = new Aggregator()
                {
                    IDAUR = !string.IsNullOrEmpty(row["ID_RF"].ToString()) ? row["ID_RF"].ToString() : "0",
                    TYPEAUR = SupportStructures.TypeAggregator.RF
                };
                listAggregator.Add(aggregator);
            }
        }

        public List<Aggregator> ListAggregatorTypeEvent(string idTypeEvent, string idAmm)
        {
            List<Aggregator> listAggregator = new List<Aggregator>();
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ALL_AGGREGATORS_TYPE_EVENT");
                q.setParam("idTypeEvent", idTypeEvent);
                q.setParam("idAmm", idAmm);
                string queryString = q.getSQL();
                if (this.ExecuteQuery(out ds, "Aggregators", q.getSQL()))
                {
                    CreateObjectsAggregatorTypeEvent(ds, ref listAggregator);
                }
                return listAggregator;
            }
            catch (Exception exc)
            {
                logger.Error(exc);
            }
            return listAggregator;
        }

        private void CreateObjectsAggregatorTypeEvent(DataSet ds, ref List<Aggregator> listAggregator)
        {
            if (ds.Tables["Aggregators"] != null && ds.Tables["Aggregators"].Rows.Count > 0)
            {
                Aggregator aggregator;
                foreach (DataRow row in ds.Tables["Aggregators"].Rows)
                {
                    aggregator = new Aggregator()
                    {
                        IDAUR = !string.IsNullOrEmpty(row["ID_AUR"].ToString()) ? row["ID_AUR"].ToString() : "0",
                        TYPEAUR = !string.IsNullOrEmpty(row["TYPE_AUR"].ToString()) ? row["TYPE_AUR"].ToString() : string.Empty,
                        TYPENOTIFY = row["TYPE_NOTIFY"].ToString()
                    };
                    listAggregator.Add(aggregator);
                }
            }
        }

    }
}