using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Deleghe;
using System.Data;
using System.Threading;
using DocsPaUtils;
using DocsPaVO.utente;
using DocsPaVO.ricerche;
using DocsPaDB.Utils;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    public class ModDeleghe : DBProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(ModDeleghe));
        private Mutex semaforo;
        private string DATE_FORMAT = "";

        public ModDeleghe()
        {
            semaforo = new Mutex();
        }

        public List<ModelloDelega> searchModelliDelegaPaging(DocsPaVO.utente.InfoUtente utente, List<Ruolo> ruoli,SearchModelloDelegaInfo searchInfo,SearchPagingContext pagingContext)
        {
            string idPeople = utente.idPeople;
            string conditions = buildConditions(searchInfo);
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add("idUtente", idPeople);
            paramList.Add("param1", conditions);
            PagingQuery pagingQuery = new PagingQuery("S_COUNT_MODELLI_DELEGA", "S_GET_MODELLI_DELEGA_PAGING", pagingContext, paramList);
            string commandText = pagingQuery.Query.getSQL();
            logger.Debug("QUERY : " + commandText);
            DataSet ds = new DataSet();
            ExecuteQuery(ds, commandText);
            List<ModelloDelega> res = new List<ModelloDelega>();
            if (ds.Tables[0].Rows.Count != 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ModelloDelega temp=buildModelloDelega(ds.Tables[0].Rows[i],ruoli);
                    res.Add(temp);
                }
            }
            return res;
        }

        public List<ModelloDelega> searchModelliDelega(DocsPaVO.utente.InfoUtente utente, List<Ruolo> ruoli, SearchModelloDelegaInfo searchInfo)
        {
            string idPeople = utente.idPeople;
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_MODELLI_DELEGA");
            queryMng.setParam("idUtente", idPeople);
            string condRicerca = buildConditions(searchInfo);
            queryMng.setParam("param1", condRicerca);
            string commandText = queryMng.getSQL();
            logger.Debug("QUERY : " + commandText);
            DataSet ds = new DataSet();
            ExecuteQuery(ds, commandText);
            List<ModelloDelega> res = new List<ModelloDelega>();
            if (ds.Tables[0].Rows.Count != 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ModelloDelega temp = buildModelloDelega(ds.Tables[0].Rows[i], ruoli);
                    res.Add(temp);
                }
            }
            return res;
        }

        public ModelloDelega getModelloDelegaById(string idModello)
        {
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_MODELLI_DELEGA_BY_ID");
            queryMng.setParam("idModello", idModello);
            string commandText = queryMng.getSQL();
            logger.Debug("QUERY : " + commandText);
            DataSet ds = new DataSet();
            ExecuteQuery(ds, commandText);
            if (ds.Tables[0].Rows.Count != 0)
            {
                ModelloDelega temp = buildModelloDelega(ds.Tables[0].Rows[0],null);
                return temp;
            }
            else
            {
                return null;
            }
        }

        public bool updateModelloDelega(ModelloDelega modelloDelega)
        {
            bool result = true;
            try
            {
                Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_MODELLO_DELEGA");
                q.setParam("idDelegato", modelloDelega.IdUtenteDelegato);
                q.setParam("idRuoloDelegato", modelloDelega.IdRuoloDelegato);
                if(!string.IsNullOrEmpty(modelloDelega.IdRuoloDelegante)){
                    q.setParam("idRuoloDelegante", modelloDelega.IdRuoloDelegante);
                }else{
                    q.setParam("idRuoloDelegante","NULL");
                }
                q.setParam("dataInizio",getToDate(modelloDelega.DataInizio,DATE_FORMAT));
                q.setParam("dataFine",getToDate(modelloDelega.DataFine,DATE_FORMAT));
                q.setParam("intervallo",""+modelloDelega.Intervallo);
                q.setParam("nome","'"+modelloDelega.Nome.Replace("'", "''")+"'");
                q.setParam("idModello", modelloDelega.Id);
                string queryString = q.getSQL();
                logger.Debug("Update modello di delega con id "+modelloDelega.Id+": ");
                logger.Debug(queryString);
                if (!ExecuteNonQuery(queryString))
                {
                    result = false;
                    RollbackTransaction();
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }
            return result;
        }

        public bool insertModelloDelega(ModelloDelega modelloDelega)
        {
            bool result = true;
            try
            {
                string insertList = modelloDelega.IdUtenteDelegante + ",";
                if (!string.IsNullOrEmpty(modelloDelega.IdRuoloDelegante))
                {
                    insertList += modelloDelega.IdRuoloDelegante + ",";
                }
                else
                {
                    insertList += "NULL,";
                }
                insertList += modelloDelega.IdUtenteDelegato + ",";
                insertList += modelloDelega.IdRuoloDelegato + ",";
                insertList += getToDate(modelloDelega.DataInizio,DATE_FORMAT) + ",";
                insertList += getToDate(modelloDelega.DataFine, DATE_FORMAT) + ",";
                insertList += modelloDelega.Intervallo + ",";
                insertList += "'"+modelloDelega.Nome.Replace("'", "''")+"'";
                Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_INSERT_MODELLO_DELEGA");
                q.setParam("colId", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                q.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(""));
                q.setParam("insertList",insertList);
                string queryString = q.getSQL();
                logger.Debug("Inserimento nuovo modello di delega: ");
                logger.Debug(queryString);
                if (!ExecuteNonQuery(queryString))
                {
                    result = false;
                    RollbackTransaction();
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }
            return result;
        }

        public bool deleteModelliDelega(List<string> idModelli)
        {
            try
            {
                semaforo.WaitOne();
                int rowsAffected;
                bool retValue = false;
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("D_DELETE_MODELLO_DELEGA");
                string ids = "";
                int index=0;
                foreach(string tempId in idModelli){
                    ids = ids + tempId;
                    if (index < idModelli.Count - 1) ids = ids + ",";
                    index++;
                }
                queryMng.setParam("idModelli", ids);
                BeginTransaction();
                string commandText = queryMng.getSQL();
                logger.Debug("QUERY : " + commandText);

                ExecuteNonQuery(commandText, out rowsAffected);
                retValue = (rowsAffected > 0);

                if (retValue)
                    CommitTransaction();
                else
                    RollbackTransaction();
                return retValue;
            }
            catch
            {
                RollbackTransaction();
                return false;
            }
            finally
            {
                Dispose();
                semaforo.ReleaseMutex();
            }
        }

        private ModelloDelega buildModelloDelega(DataRow row,List<Ruolo> ruoli)
        {
            ModelloDelega res = new ModelloDelega(ruoli);
            if (row["DTA_INIZIO"].GetType()!=typeof(DBNull))
            {
                Type tp=row["DTA_INIZIO"].GetType();
                res.DataInizio = (DateTime)row["DTA_INIZIO"];
            }
            if (row["DTA_FINE"].GetType() != typeof(DBNull))
            {
                res.DataFine = (DateTime)row["DTA_FINE"];
            }
            res.Id = row["SYSTEM_ID"].ToString();
            res.IdUtenteDelegante=row["ID_PEOPLE_DELEGANTE"].ToString();
            if (row["ID_RUOLO_DELEGANTE"].GetType() != typeof(DBNull))
            {
                res.IdRuoloDelegante = row["ID_RUOLO_DELEGANTE"].ToString();
            }
            res.DescrRuoloDelegante = row["VAR_DESC_CORR"].ToString();
            res.IdUtenteDelegato = row["ID_PEOPLE_DELEGATO"].ToString();
            res.IdRuoloDelegato = row["ID_RUOLO_DELEGATO"].ToString();
            if (row["INTERVALLO"].GetType() != typeof(DBNull))
            {
                res.Intervallo = Int32.Parse(row["INTERVALLO"].ToString());
            }
            res.Nome = row["NOME"].ToString();
            res.DescrUtenteDelegato = row["FULL_NAME"].ToString();
            return res;
        }

        private string getToDate(DateTime value,string format)
        {
            if (value.Year==1) return "NULL";
            string temp = value.ToString(format);
            return DocsPaDbManagement.Functions.Functions.ToDate(temp);
        }

        private string buildConditions(SearchModelloDelegaInfo searchInfo)
        {
            string condRicerca = "";
            if (searchInfo != null)
            {
                if (!string.IsNullOrEmpty(searchInfo.Nome))
                {
                    condRicerca += " AND UPPER(A.NOME) LIKE '%" + searchInfo.Nome.ToUpper().Replace("'", "''") + "%'";
                }
                if (!string.IsNullOrEmpty(searchInfo.IdRuoloDelegante))
                {
                    condRicerca += " AND A.ID_RUOLO_DELEGANTE = " + searchInfo.IdRuoloDelegante;
                }
                if (!string.IsNullOrEmpty(searchInfo.NomeDelegato))
                {
                    condRicerca += " AND UPPER(B.VAR_DESC_CORR) LIKE '%" + searchInfo.NomeDelegato.ToUpper().Replace("'", "''") + "%'";
                }
                if (searchInfo.DataInizio.CompareTo(DateTime.MinValue) > 0)
                {
                    DateTime endDate = searchInfo.DataInizio.AddDays(1).AddSeconds(-1);
                    condRicerca += " AND A.DTA_INIZIO BETWEEN " + getToDate(searchInfo.DataInizio, DATE_FORMAT) + " AND " + getToDate(endDate, DATE_FORMAT);
                }
            }
            return condRicerca;
        }
    }
}
