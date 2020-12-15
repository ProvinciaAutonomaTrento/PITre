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
    }
}
