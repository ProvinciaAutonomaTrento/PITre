using DocsPaDbManagement.Functions;
using DocsPaVO.Plugin;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace DocsPaDB.Query_DocsPAWS
{
    public class PluginHashDB : DBProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(PluginHashDB));

        public DpaPluginHash GetHashMail(string hashFile)
        {
            DpaPluginHash dpaPluginHash = null;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_PLUGIN_HASH");
                q.setParam("hashFile", hashFile);
                if (dbType.ToUpper() == "SQL")
                {
                    q.setParam("dbuser", getUserDB());
                }
                query = q.getSQL();
                if (this.ExecuteQuery(out ds, "dpa_plugin_hash", query))
                {
                    BuildDpaPluginHashObject(ds, ref dpaPluginHash);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dpaPluginHash;
        }

        public bool NewHashMail(string idProfile, string idPeople, string hashFile)
        {
            bool retValue = false;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_PLUGIN_HASH");
                q.setParam("param1", idProfile);
                q.setParam("param2", idPeople);
                q.setParam("param3", DocsPaDbManagement.Functions.Functions.GetDate());
                q.setParam("param4", hashFile);
                string commandText = q.getSQL();
                logger.Debug(commandText);

                retValue = dbProvider.ExecuteNonQuery(commandText);
            }

            return retValue;
        }

        #region Utils

        private void BuildDpaPluginHashObject(DataSet ds, ref DpaPluginHash dpaPluginHash)
        {
            try
            {
                if (ds.Tables["dpa_plugin_hash"] != null && ds.Tables["dpa_plugin_hash"].Rows.Count > 0)
                {
                    if(ds.Tables["dpa_plugin_hash"].Rows != null && ds.Tables["dpa_plugin_hash"].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables["dpa_plugin_hash"].Rows[0];
                        dpaPluginHash = new DpaPluginHash()
                        {
                            idProfile = !string.IsNullOrEmpty(row["ID_PROFILE"].ToString()) ? row["ID_PROFILE"].ToString() : string.Empty,
                            dataElaborazione = Convert.ToDateTime(row["DTA_ELAB"].ToString(), new System.Globalization.CultureInfo("it-IT")),
                            idPeople = !string.IsNullOrEmpty(row["ID_PEOPLE"].ToString()) ? row["ID_PEOPLE"].ToString() : string.Empty,
                            systemId = !string.IsNullOrEmpty(row["SYSTEM_ID"].ToString()) ? row["SYSTEM_ID"].ToString() : string.Empty,
                            hashFile = !string.IsNullOrEmpty(row["HASH_FILE"].ToString()) ? row["HASH_FILE"].ToString() : string.Empty
                        };
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public string getUserDB()
        {
            return Functions.GetDbUserSession();
        }

        #endregion
    }
}
