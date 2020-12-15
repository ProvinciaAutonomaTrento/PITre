using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.amministrazione;
using log4net;
using System.Data;

namespace DocsPaDB.Query_DocsPAWS
{
    public class AdministrationAssertionEvent : DBProvider
    {
        private static ILog logger = LogManager.GetLogger(typeof(AdministrationAssertionEvent));

        public List<Assertion> GetListAssertionByAmm(string idAmm)
        {
            List<Assertion> listAssertion = new List<Assertion>();
            DataSet ds;
            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_EVENT_TYPE_ASSERTIONS");
                queryDef.setParam("idAmm", idAmm);
                string commandText = queryDef.getSQL();
                logger.Debug("Eseguo la query:\n" + commandText);

                if (this.ExecuteQuery(out ds, "ASSERTIONS", commandText))
                {
                    CreateObjectAssertion(ds, ref listAssertion);
                }
            }
            catch (Exception exc)
            {
                logger.Debug("Si è verificata un'eccezione durante l'esecuzione della query S_DPA_EVENT_TYPE_ASSERTIONS:\n" + exc.Message);
                return listAssertion;
            }
            return listAssertion;
        }

        public List<string> GetTypeConfigurableEvents(string idAmm)
        {
            List<string> listTypeEvent = new List<string>();
            DataSet ds;
            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_TYPE_EVENT_CONFIGURABLE");
                queryDef.setParam("idAmm", idAmm);
                string commandText = queryDef.getSQL();
                logger.Debug("Eseguo la query:\n" + commandText);

                if (this.ExecuteQuery(out ds, "TYPE_EVENT_CONF", commandText))
                {
                    CreateTypeEventConf(ds, ref listTypeEvent);
                }
            }
            catch (Exception exc)
            {
                logger.Debug("Si è verificata un'eccezione durante l'esecuzione della query S_TYPE_EVENT_CONFIGURABLE:\n" + exc.Message);
                return listTypeEvent;
            }
            return listTypeEvent;
        }

        private void CreateObjectAssertion(DataSet ds, ref List<Assertion> list)
        {
            list = new List<Assertion>();
            if (ds.Tables["ASSERTIONS"] != null && ds.Tables["ASSERTIONS"].Rows.Count > 0)
            {
                foreach(DataRow row in ds.Tables["ASSERTIONS"].Rows)
                {
                    Assertion assertion = new Assertion()
                    {
                        SYSTEM_ID = !string.IsNullOrEmpty(row["SYSTEM_ID"].ToString()) ? Convert.ToInt64(row["SYSTEM_ID"].ToString()) : 0,
                        ID_TYPE_EVENT = !string.IsNullOrEmpty(row["ID_TYPE_EVENT"].ToString()) ? Convert.ToInt64(row["ID_TYPE_EVENT"].ToString()) : 0,
                        DESC_TYPE_EVENT = row["DESC_TYPE_EVENT"].ToString(),
                        ID_AUR = !string.IsNullOrEmpty(row["ID_AUR"].ToString()) ? Convert.ToInt64(row["ID_AUR"].ToString()) : 0,
                        DESC_AUR = row["DESC_AUR"].ToString(),
                        TYPE_AUR = row["TYPE_AUR"].ToString(),
                        TYPE_NOTIFY = Convert.ToChar(row["TYPE_NOTIFY"].ToString()),
                        IS_EXERCISE = row["IS_EXERCISE"].ToString().Equals("0") ? false : true,
                        ID_AMM = !string.IsNullOrEmpty(row["ID_AMM"].ToString()) ? Convert.ToInt64(row["ID_AMM"].ToString()) : 0,
                        CONFIG_TYPE_EVENT = row["NOTIFY"].ToString()
                    };

                    list.Add(assertion);
                }
            }
        }

        /// <summary>
        /// Formats the list of configurable event types
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="list"></param>
        private void CreateTypeEventConf(DataSet ds, ref List<string> list)
        { 
            list = new List<string>();
            if (ds.Tables["TYPE_EVENT_CONF"] != null && ds.Tables["TYPE_EVENT_CONF"].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables["TYPE_EVENT_CONF"].Rows)
                {
                    list.Add(row["SYSTEM_ID"].ToString() + "#" + row["DESC_TYPE_EVENT"].ToString());
                }
            }
        }

        /// <summary>
        /// Insert a new assertion event.
        /// </summary>
        /// <param name="assertion"></param>
        public int InsertAssertionEvent(Assertion assertion)
        {
            string commandText;
            StringBuilder strbuilderForLog = new StringBuilder();
            int res;
            try
            {
                //preparo le info sull'asserzione per il log
                strbuilderForLog.Append("ID TIPO EVENTO: " + assertion.ID_TYPE_EVENT.ToString() + "\n" +
                    "DESCR. TIPO EVENTO: " + assertion.DESC_TYPE_EVENT.Replace("'", "''") + "\n" +
                    "ID AGGREGATORE : " + assertion.ID_AUR.ToString() + "\n" +
                    "TIPO AGGREGATORE: " + assertion.TYPE_AUR + "\n" +
                    "DESCR. AGGREGATORE: " + assertion.DESC_AUR.Replace("'", "''") + "\n" +
                    "TIPOLOGIA NOTIFICA: " + assertion.TYPE_NOTIFY.ToString() + "\n" +
                    "IN ESERCIZIO: " + (assertion.IS_EXERCISE ? "SI" : "NO") + "\n" +
                    "ID AMMINISTRAZIONE: " + assertion.ID_AMM.ToString() + "\n");

                //verifico se l'asserzione è gia presente su db
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_ASSERTIONS_COUNT_DUPLICATE");
                queryDef.setParam("idTypeEvent", assertion.ID_TYPE_EVENT.ToString());
                queryDef.setParam("idAur", assertion.ID_AUR.ToString());
                queryDef.setParam("typeAur", "'" + assertion.TYPE_AUR + "'");
                queryDef.setParam("idAmm", assertion.ID_AMM.ToString());
                commandText = queryDef.getSQL();
                string count;
                this.ExecuteScalar(out count, commandText);
                if (string.IsNullOrEmpty(count) || count.Equals("0")) // l'asserzione è presente sul db, quindi vado avanti nell'inserimento
                {
                    StringBuilder strbuilder = new StringBuilder();
                    strbuilder.Append("seq.nextval, ");
                    strbuilder.Append(assertion.ID_TYPE_EVENT.ToString() + ", ");
                    strbuilder.Append("'" + assertion.DESC_TYPE_EVENT.Replace("'", "''") + "', ");
                    strbuilder.Append(assertion.ID_AUR.ToString() + ", ");
                    strbuilder.Append("'" + assertion.DESC_AUR + "', ");
                    strbuilder.Append("'" + assertion.TYPE_AUR + "', ");
                    strbuilder.Append("'" + assertion.TYPE_NOTIFY.ToString() + "', ");
                    strbuilder.Append(assertion.IS_EXERCISE ? "'1', " : "'0', ");
                    strbuilder.Append(assertion.ID_AMM);
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_EVENT_TYPE_ASSERTIONS");
                    queryDef.setParam("value", strbuilder.ToString());
                    commandText = queryDef.getSQL();
                    logger.Debug("Eseguo la query:\n" + commandText);

                    if (!this.ExecuteNonQuery(commandText))
                    {
                        res = -1;//errore durante la insert
                        logger.Debug("ERRORE DURANTE LA CREAZIONE DELL'ASSERZIONE:\n" + strbuilderForLog.ToString());
                    }
                    else
                    {
                        res = 0;
                        logger.Debug("ASSERZIONE:\n" + strbuilderForLog.ToString() + "CORRETTAMENTE CREATA");

                    }
                }
                else
                {
                    res = 1;
                    logger.Debug("L'ASSERZIONE:\n" + strbuilderForLog.ToString() + "E' GIA' PRESENTE SUL DB");
                }
            }
            catch (Exception exc)
            {
                logger.Debug("SI E' VERIFICATA UN'ECCEZIONE DURANTE LA CREAZIONE DELL'ASSERZIONE:\n" + strbuilderForLog.ToString() +
                    "DETTAGLIO ECCEZIONE:\n" + exc.Message);
                res = -1;
            }
            return res;
        }

        public List<string> SearchAur(string typeAur, string code, string description, string idAmm)
        {
            DataSet ds;
            List<string> listAur = new List<string>();
            string commandText = string.Empty;
            try
            {
                DocsPaUtils.Query queryDef;
                //scelgo la query di search in base al tipo di aggregatore
                if (typeAur.Equals("TR"))
                {
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_SEARCH_AUR_TR");
                }
                else if (typeAur.Equals("RF"))
                {
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_SEARCH_AUR_RF");
                }
                else if (typeAur.Equals("UO"))
                {
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_SEARCH_AUR_UO");
                }
                else if (typeAur.Equals("R"))
                {
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_SEARCH_AUR_ROLE");
                }
                else return listAur;
                queryDef.setParam("idAmm", idAmm);
                queryDef.setParam("code", code);
                queryDef.setParam("description", description);
                commandText = queryDef.getSQL();
                logger.Debug("Eseguo la query:\n" + commandText);
                if (this.ExecuteQuery(out ds, "LIST_AUR", commandText))
                {
                    if (ds.Tables["LIST_AUR"] != null && ds.Tables["LIST_AUR"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["LIST_AUR"].Rows)
                        {
                            string aur = row["SYSTEM_ID"].ToString() + "#" + row["CODE"].ToString() + "#" +
                                row["DESCR"].ToString();
                            listAur.Add(aur);
                        }
                    }
                }
                return listAur;
            }
            catch (Exception exc)
            {
                logger.Debug("ERRORE DURANTE L'ESECUZIONE DELLA QUERY: " + exc.Message);
                return listAur;
            }

        }

        public int UpdateAssertionEvent(Assertion assertion)
        {
            string commandText;
            StringBuilder strbuilderForLog = new StringBuilder();
            int res = 0;
            try
            {
                //preparo le info sull'asserzione per il log
                strbuilderForLog.Append("ID TIPO EVENTO: " + assertion.ID_TYPE_EVENT.ToString() + "\n" +
                    "DESCR. TIPO EVENTO: " + assertion.DESC_TYPE_EVENT + "\n" +
                    "ID AGGREGATORE : " + assertion.ID_AUR.ToString() + "\n" +
                    "TIPO AGGREGATORE: " + assertion.TYPE_AUR + "\n" +
                    "DESCR. AGGREGATORE: " + assertion.DESC_AUR + "\n" +
                    "TIPOLOGIA NOTIFICA: " + assertion.TYPE_NOTIFY.ToString() + "\n" +
                    "IN ESERCIZIO: " + (assertion.IS_EXERCISE ? "SI" : "NO") + "\n" +
                    "ID AMMINISTRAZIONE: " + assertion.ID_AMM.ToString() + "\n");

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_EVENT_TYPE_ASSERTIONS");
                queryDef.setParam("idTypeEvent", assertion.ID_TYPE_EVENT.ToString());
                queryDef.setParam("descTypeEvent", assertion.DESC_TYPE_EVENT);
                queryDef.setParam("idAur", assertion.ID_AUR.ToString());
                queryDef.setParam("descAur", assertion.DESC_AUR);
                queryDef.setParam("typeAur", assertion.TYPE_AUR);
                queryDef.setParam("typeNotify", assertion.TYPE_NOTIFY.ToString());
                queryDef.setParam("isExercise", assertion.IS_EXERCISE ? "1" : "0");
                queryDef.setParam("idAmm", assertion.ID_AMM.ToString());
                queryDef.setParam("systemId", assertion.SYSTEM_ID.ToString());
                commandText = queryDef.getSQL();
                logger.Debug("Eseguo la query:\n" + commandText);

                if (!this.ExecuteNonQuery(commandText))
                {
                    res = -1;//errore durante la insert
                    logger.Debug("ERRORE DURANTE LA MODIFICA DELL'ASSERZIONE:\n" + strbuilderForLog.ToString());
                }
                return res;
            }
            catch (Exception exc)
            {
                logger.Debug("SI E' VERIFICATA UN'ECCEZIONE DURANTE LA MODIFICA DELL'ASSERZIONE:\n" + strbuilderForLog.ToString() +
                    "DETTAGLIO ECCEZIONE:\n" + exc.Message);
                return -1;
            }
        }

        /// <summary>
        /// Rimuove le asserzioni
        /// </summary>
        /// <param name="typeAur"></param>
        /// <param name="code"></param>
        /// <param name="description"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public bool RemoveAssertions(List<Assertion> assertions)
        {
            bool result = false;
            StringBuilder strbuilderForLog = new StringBuilder();
            try
            {
                string commandText = string.Empty;
                string listSystemIdAssertion = string.Empty;
                int rowAffected;
                DocsPaUtils.Query query;
                string resultLog = string.Empty;
                query = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_EVENT_TYPE_ASSERTIONS");
                foreach(Assertion assertion in assertions)
                {
                    listSystemIdAssertion += string.IsNullOrEmpty(listSystemIdAssertion) ? " '" +assertion.SYSTEM_ID.ToString() + "'" : " ,'" + assertion.SYSTEM_ID.ToString() +"'" ;
                }
                query.setParam("listSystemIdAssertion", listSystemIdAssertion);
                commandText = query.getSQL();
                this.ExecuteNonQuery(commandText, out rowAffected);
                if (rowAffected > 0)
                {
                    result = true;
                    resultLog = " ELIMINATA CORRETTAMENTE";
                }
                else
                {
                    result = false;
                    resultLog = " NON ELIMINATA CORRETTAMENTE";
                }
                //preparo le info sull'asserzione per il log
                foreach(Assertion assertion in assertions)
                {
                    strbuilderForLog.Append("ID TIPO EVENTO: " + assertion.ID_TYPE_EVENT.ToString() + "\n" +
                        "DESCR. TIPO EVENTO: " + assertion.DESC_TYPE_EVENT + "\n" +
                        "ID AGGREGATORE : " + assertion.ID_AUR.ToString() + "\n" +
                        "TIPO AGGREGATORE: " + assertion.TYPE_AUR + "\n" +
                        "DESCR. AGGREGATORE: " + assertion.DESC_AUR + "\n" +
                        "TIPOLOGIA NOTIFICA: " + assertion.TYPE_NOTIFY.ToString() + "\n" +
                        "IN ESERCIZIO: " + (assertion.IS_EXERCISE ? "SI" : "NO") + "\n" +
                        "ID AMMINISTRAZIONE: " + assertion.ID_AMM.ToString() + "\n");   
                    logger.Debug("ASSERZIONE:\n" + strbuilderForLog.ToString() + resultLog);
                }
            }
            catch (Exception exc)
            {
                logger.Debug("ECCEZIONE DURANTE L'ELIMINAZIONE DELLE ASSERZIONI: " + strbuilderForLog.ToString() +
                    "\n DETTAGLIO ECCEZIONE:" + exc.Message);
                return false;
            }
            return result;

        }

    }
}
