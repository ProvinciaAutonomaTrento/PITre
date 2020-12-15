using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Mobile;
using DocsPaVO.utente;
using DocsPaUtils;
using log4net;
using System.Data;
using DocsPaVO.Mobile.Responses;

namespace DocsPaDB.Query_DocsPAWS.Mobile
{
    public class RubricaMobile : DBProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(RubricaMobile));

        public List<LightUserInfo> GetListaUtentiInterni(string descrizione, InfoUtente infoUtente,int numMaxResults,string idRegistro)
        {
            logger.Info("BEGIN");
            logger.Debug("descrizione: " + descrizione+" idAmministrazione: "+infoUtente.idAmministrazione);
            List<LightUserInfo> res=new List<LightUserInfo>();
            Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_RICERCA_ELEMENTI_RUBRICA_MOBILE");
            if (dbType.ToUpper() == "SQL")
            {
                string userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession();
                queryDef.setParam("docsadm", userDb);
            }
            logger.Debug("query: " + queryDef);
            queryDef.setParam("param1", infoUtente.userId);
            queryDef.setParam("param2", infoUtente.idAmministrazione);
            queryDef.setParam("param3", descrizione);
            queryDef.setParam("param4", idRegistro);
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);
            DataSet ds = new DataSet();
            if (this.ExecuteQuery(out ds, "rubrica",commandText))
            {
                int i = 0;
                foreach (DataRow row in ds.Tables["rubrica"].Rows)
                {
                    res.Add(FillLightUserInfo(row));
                    if (i > numMaxResults - 1) break;
                    i++;
                }
            }
            return res;
        }

        public List<RicercaSmistamentoElement> GetListaElementiInterni(string descrizione, InfoUtente infoUtente, int numMaxResults, int numMaxResultsForCategory, string idRegistro)
        {
            logger.Info("BEGIN");
            logger.Debug("descrizione: " + descrizione + " idAmministrazione: " + infoUtente.idAmministrazione);
            List<RicercaSmistamentoElement> res = new List<RicercaSmistamentoElement>();
            // MEV SMISTAMENTO
            // se numMaxResults = -99, sto ricercando gli utenti appartenenti al ruolo
            string query = "S_RICERCA_ELEMENTI_RUBRICA_MOBILE_2";
            if (numMaxResults == -99)
                query = "S_RICERCA_RUBRICA_MOBILE_UTENTI_RUOLO";

            //Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_RICERCA_ELEMENTI_RUBRICA_MOBILE_2");
            Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery(query);
            if (dbType.ToUpper() == "SQL")
            {
                string userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession();
                queryDef.setParam("docsadm", userDb);
            }
            logger.Debug("query: " + queryDef);
            queryDef.setParam("param1", infoUtente.idAmministrazione);
            queryDef.setParam("param2", descrizione);
            queryDef.setParam("param3", idRegistro);
            queryDef.setParam("param4", numMaxResultsForCategory.ToString());
            queryDef.setParam("param5", infoUtente.userId);
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);
            DataSet ds = new DataSet();
            if (this.ExecuteQuery(out ds, "rubrica", commandText))
            {
                int i = 0;
                foreach (DataRow row in ds.Tables["rubrica"].Rows)
                {
                    res.Add(FillRicercaSmistamentoElement(row));
                    if (!(numMaxResults == -99))
                    {
                        if (i > numMaxResults - 1) break;
                        i++;
                    }
                }
            }
            return res;
        }

        private LightUserInfo FillLightUserInfo(DataRow row)
        {
            return new LightUserInfo()
            {
                IdPeople = row["id_people"].ToString(),
                Descrizione = row["var_desc_corr"].ToString(),
                UserId = row["var_cod_rubrica"].ToString()
            };
        }

        private RicercaSmistamentoElement FillRicercaSmistamentoElement(DataRow row)
        {
            RicercaSmistamentoElement res= new RicercaSmistamentoElement();
            string tipo=row["cha_tipo_urp"].ToString();
            if("P".Equals(tipo)){
                res.Type = SmistamentoNodeType.UTENTE;
                res.IdUtente=row["id_people"].ToString();
                res.IdRuolo = row["id_ruolo"].ToString(); 
                res.IdUO = row["id_uo"].ToString(); 
                res.DescrizioneUtente = row["var_desc_corr"].ToString();
                res.DescrizioneRuolo = null;
                res.DescrizioneUO = null;
            }else if("R".Equals(tipo)){
                // MEV SMISTAMENTO
                // aggiungo alla ricerca gli utenti nel ruolo selezionato
                string idPeople = row["id_people"].ToString();
                if (idPeople.Equals("0"))
                {
                    res.Type = SmistamentoNodeType.RUOLO;
                    res.IdRuolo = row["id_ruolo"].ToString();
                    res.IdUO = row["id_uo"].ToString();
                    res.DescrizioneRuolo = row["var_desc_corr"].ToString();
                    res.DescrizioneUO = null;
                }
                // aggiunta
                else
                {
                    res.Type = SmistamentoNodeType.UTENTE;
                    res.IdUtente = row["id_people"].ToString();
                    res.IdRuolo = row["id_ruolo"].ToString();
                    res.IdUO = row["id_uo"].ToString();
                    res.DescrizioneUtente = row["var_desc_corr"].ToString();
                    res.DescrizioneRuolo = row["id_ruolo"].ToString();
                    res.DescrizioneUO = null;
                }
                // fine aggiunta
            }else{
                res.Type = SmistamentoNodeType.UO;
                res.IdUO=row["id_uo"].ToString();
                res.DescrizioneUO = row["var_desc_corr"].ToString();
            };
            logger.Debug(res.Type.ToString() + " - " + res.DescrizioneUtente + " - " + res.DescrizioneRuolo);
            return res;
        }

        #region preferiti mobile
        public bool PrefMobile_Insert(DocsPaVO.Mobile.InfoPreferito infoPref)
        {
            bool retval = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_PREF_MOBILE");
                q.setParam("idpeopleowner", infoPref.IdPeopleOwner);
                // inserimento veloce anche in mancanze di idInternal o Idcorr

                if (!string.IsNullOrWhiteSpace(infoPref.IdCorrespondent) )
                {
                    q.setParam("idcorr", infoPref.IdCorrespondent);
                    if (string.IsNullOrEmpty(infoPref.IdInternal) && infoPref.TipoURP != "U")
                    {
                        q.setParam("idinternal", (Int32.Parse(infoPref.IdCorrespondent) -1).ToString() );
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(infoPref.IdInternal))
                    {
                        q.setParam("idinternal", infoPref.IdInternal);
                        q.setParam("idcorr", (Int32.Parse(infoPref.IdInternal) + 1).ToString());
                    }
                    else { throw new Exception("mancano i parametri"); }
                }
                
                //q.setParam("idcorr", infoPref.IdCorrespondent);
                //if (!string.IsNullOrEmpty(infoPref.IdInternal))
                //    q.setParam("idinternal", infoPref.IdInternal);
                //else
                //    q.setParam("idinternal", "null");

                q.setParam("desccorr", infoPref.DescCorrespondent);
                if (!string.IsNullOrEmpty(infoPref.TipoURP))
                    q.setParam("cha_tipo_urp", infoPref.TipoURP);
                else
                    q.setParam("cha_tipo_urp", "P");


                if (!string.IsNullOrEmpty(infoPref.TipoPref))
                    q.setParam("cha_tipo_pref", infoPref.TipoPref);
                else
                    q.setParam("cha_tipo_pref", "A");

                string queryString = q.getSQL();
                logger.Debug(queryString);

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    retval = this.ExecuteNonQuery(queryString);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retval = false;
            }

            return retval;
        }

        public bool PrefMobile_Delete(DocsPaVO.Mobile.InfoPreferito infoPref)
        {
            bool retval = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_PREF_MOBILE");
                string condizione = "";
                if (!string.IsNullOrEmpty(infoPref.SystemId))
                    condizione = " SYSTEM_ID = " + infoPref.SystemId;
                else
                {
                    if (!string.IsNullOrEmpty(infoPref.IdCorrespondent))
                        condizione = string.Format(" Id_People_owner = {0} and id_corr = {1}", infoPref.IdPeopleOwner, infoPref.IdCorrespondent);
                    else
                        condizione = string.Format(" Id_People_owner = {0} and Id_Internal = {1}", infoPref.IdPeopleOwner, infoPref.IdInternal);

                }

                q.setParam("condizione", condizione);
                string queryString = q.getSQL();
                logger.Debug(queryString);

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    retval = this.ExecuteNonQuery(queryString);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retval = false;
            }

            return retval;
        }

        public List<InfoPreferito> PrefMobile_getList(string idpeopleowner, bool soloP, string tipoPref)
        {
            List<InfoPreferito> retval = new List<InfoPreferito>();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_PREF_MOBILE");
                string condizione = " WHERE ID_PEOPLE_OWNER = " + idpeopleowner;
                if (!string.IsNullOrEmpty(tipoPref))
                {
                    condizione += " AND CHA_TIPO_PREF = '" + tipoPref + "'";
                }
                if (soloP)
                {
                    condizione += " AND CHA_TIPO_URP = 'P'";
                }
                q.setParam("condizione", condizione);
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DocsPaVO.Mobile.InfoPreferito infoPref;
                DataSet dataset = new DataSet();

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    this.ExecuteQuery(out dataset, "INFOPREF", queryString);
                    if (dataset.Tables["INFOPREF"] != null && dataset.Tables["INFOPREF"].Rows.Count > 0)
                    {
                        foreach (DataRow r in dataset.Tables["INFOPREF"].Rows)
                        {
                            infoPref = new DocsPaVO.Mobile.InfoPreferito();
                            infoPref.SystemId = r["SYSTEM_ID"].ToString();
                            infoPref.IdPeopleOwner = r["ID_PEOPLE_OWNER"].ToString();
                            infoPref.IdCorrespondent = r["ID_CORR"].ToString();
                            infoPref.IdInternal = r["ID_INTERNAL"].ToString();
                            infoPref.DescCorrespondent = r["DESC_CORR"].ToString();
                            infoPref.TipoURP = r["CHA_TIPO_URP"].ToString();
                            infoPref.TipoPref = r["CHA_TIPO_PREF"].ToString();

                            retval.Add(infoPref);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex); retval = null;
            }
            return retval;
        }

        #endregion
    }
}
