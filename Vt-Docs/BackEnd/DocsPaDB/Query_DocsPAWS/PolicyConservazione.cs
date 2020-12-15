using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using DocsPaUtils;
using DocsPaVO.FormatiDocumento;
using System.Data;
using DocsPaVO.ProfilazioneDinamica;
using System.Collections;

namespace DocsPaDB.Query_DocsPAWS
{
    public class PolicyConservazione
    {
        private ILog logger = LogManager.GetLogger(typeof(PolicyConservazione));

        public bool InsertNewPolicy(DocsPaVO.Conservazione.Policy policy)
        {
            bool result = true;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                //modifica SAB x gestione transazione (in sql server non recupera lo scope_identity se non c'è la transazione
                dbProvider.BeginTransaction();
                //using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                //using (dbProvider)
                {
                    Query query = InitQuery.getInstance().getQuery("I_POLICY_CONSERVAZIONE");
                    query.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    query.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("POLICY_CONSERVAZIONE"));
                    query.setParam("name", policy.nome);
                    query.setParam("idAmm", policy.idAmministrazione);
                    query.setParam("tipo", policy.tipo);
                    query.setParam("tipoClass", policy.tipoClassificazione);

                    string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];

                    if (policy.includiSottoNodi)
                    {
                        query.setParam("sottoNodi", "1");
                    }
                    else
                    {
                        query.setParam("sottoNodi", "");
                    }

                    if (policy.soloDigitali)
                    {
                        query.setParam("digitali", "1");
                    }
                    else
                    {
                        query.setParam("digitali", "");
                    }

                    if (policy.soloFirmati)
                    {
                        query.setParam("firmati", "1");
                    }
                    else
                    {
                        query.setParam("firmati", "");
                    }

                    if (!string.IsNullOrEmpty(policy.classificazione))
                    {
                        query.setParam("classificazione", policy.classificazione);
                    }
                    else
                    {
                        query.setParam("classificazione", "-1");
                    }

                    if (!string.IsNullOrEmpty(policy.idStatoDiagramma))
                    {
                        query.setParam("diagramma", policy.idStatoDiagramma);
                    }
                    else
                    {
                        query.setParam("diagramma", "-1");
                    }
                    if (!string.IsNullOrEmpty(policy.idTemplate))
                    {
                        query.setParam("template", policy.idTemplate);
                    }
                    else
                    {
                        query.setParam("template", "-1");
                    }
                    if (!string.IsNullOrEmpty(policy.idAOO))
                    {
                        query.setParam("aoo", policy.idAOO);
                    }
                    else
                    {
                        query.setParam("aoo", "-1");
                    }
                    if (!string.IsNullOrEmpty(policy.idRf))
                    {
                        query.setParam("rf", policy.idRf);
                    }
                    else
                    {
                        query.setParam("rf", "-1");
                    }
                    if (!string.IsNullOrEmpty(policy.idUoCreatore))
                    {
                        query.setParam("idUoCreatore", policy.idUoCreatore);
                    }
                    else
                    {
                        query.setParam("idUoCreatore", "-1");
                    }
                    if (policy.uoSottoposte)
                    {
                        query.setParam("uoSott", "1");
                    }
                    else
                    {
                        query.setParam("uoSott", null);
                    }

                    if (policy.tipo.Equals("D"))
                    {
                        if (policy.arrivo)
                        {
                            query.setParam("arrivo", "1");
                        }
                        else
                        {
                            query.setParam("arrivo", "0");
                        }
                        if (policy.partenza)
                        {
                            query.setParam("partenza", "1");
                        }
                        else
                        {
                            query.setParam("partenza", "0");
                        }
                        if (policy.interno)
                        {
                            query.setParam("interno", "1");
                        }
                        else
                        {
                            query.setParam("interno", "0");
                        }
                        if (policy.grigio)
                        {
                            query.setParam("grigio", "1");
                        }
                        else
                        {
                            query.setParam("grigio", "0");
                        }
                        if (!string.IsNullOrEmpty(policy.dataCreazioneDa))
                        {
                            query.setParam("dataCDa", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa));
                        }
                        else
                        {
                            if (dbType.ToUpper().Equals("SQL")) 
                            {
                                query.setParam("dataCDa", "NULL");
                            }
                            else
                            {
                                query.setParam("dataCDa", "''");
                            }
                        }
                        if (!string.IsNullOrEmpty(policy.dataCreazioneA))
                        {
                            query.setParam("dataCA", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneA));
                        }
                        else
                        {
                            if (dbType.ToUpper().Equals("SQL"))
                            {
                                query.setParam("dataCA", "NULL");
                            }
                            else
                            {
                                query.setParam("dataCA", "''");
                            }
                        }
                        if (!string.IsNullOrEmpty(policy.dataProtocollazioneDa))
                        {
                            query.setParam("dataPDa", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneDa));
                        }
                        else
                        {
                            if (dbType.ToUpper().Equals("SQL"))
                            {
                                query.setParam("dataPDa", "NULL");
                            }
                            else
                            {
                                query.setParam("dataPDa", "''");
                            }
                        }
                        if (!string.IsNullOrEmpty(policy.dataProtocollazioneA))
                        {
                            query.setParam("dataPA", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneA));
                        }
                        else
                        {
                            if (dbType.ToUpper().Equals("SQL"))
                            {
                                query.setParam("dataPA", "NULL");
                            }
                            else
                            {
                                query.setParam("dataPA", "''");
                            }
                        }

                        if (!string.IsNullOrEmpty(policy.tipoDataCreazione))
                        {
                            query.setParam("tipoDataC", policy.tipoDataCreazione);
                        }
                        else
                        {
                            query.setParam("tipoDataC", null);
                        }
                        if (!string.IsNullOrEmpty(policy.tipoDataProtocollazione))
                        {
                            query.setParam("tipoDataP", policy.tipoDataProtocollazione);
                        }
                        else
                        {
                            query.setParam("tipoDataP", null);
                        }

                    }
                    else
                    {
                        if (policy.tipo.Equals("R") || policy.tipo.Equals("C"))
                        {

                            if (!string.IsNullOrEmpty(policy.dataCreazioneDa))
                            {
                                query.setParam("dataCDa", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa));
                            }
                            else
                            {
                                query.setParam("dataCDa", "''");
                            }
                            if (!string.IsNullOrEmpty(policy.dataCreazioneA))
                            {
                                query.setParam("dataCA", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneA));
                            }
                            else
                            {
                                query.setParam("dataCA", "''");
                            }
                            if (!string.IsNullOrEmpty(policy.tipoDataCreazione))
                            {
                                query.setParam("tipoDataC", policy.tipoDataCreazione);
                            }
                            else
                            {
                                query.setParam("tipoDataC", null);
                            }
                        }
                        else
                        {
                            query.setParam("dataCDa", "''");
                            query.setParam("dataCA", "''");
                            query.setParam("tipoDataC", null);
                        }
                        query.setParam("grigio", "0");
                        query.setParam("interno", "0");
                        query.setParam("partenza", "0");
                        query.setParam("arrivo", "0");


                        query.setParam("dataPDa", "''");
                        query.setParam("dataPA", "''");

                        query.setParam("tipoDataP", null);
                    }
                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - InsertNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    logger.Debug("SQL - InsertNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);

                    if (policy.FormatiDocumento != null && policy.FormatiDocumento.Count > 0)
                    {
                        string idPolicy = string.Empty;
                        // Recupero id e inserimento campo CLOB
                        string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("POLICY_CONSERVAZIONE");
                        System.Diagnostics.Debug.WriteLine("SQL - InsertNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + sql);
                        logger.Debug("SQL - InsertNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + sql);
                        dbProvider.ExecuteScalar(out idPolicy, sql);
                        foreach (SupportedFileType temp in policy.FormatiDocumento)
                        {
                            query = InitQuery.getInstance().getQuery("I_DPA_ASS_POLICY_TYPE");
                            query.setParam("id_policy", idPolicy);
                            query.setParam("id_type", (temp.SystemId).ToString());
                            commandText = query.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - InsertNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                            logger.Debug("SQL - InsertNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);
                        }
                    }

                    //Inserisci campi profilati
                    if (policy.template != null && policy.template.ELENCO_OGGETTI != null && policy.template.ELENCO_OGGETTI.Count > 0)
                    {
                        string idPolicy = string.Empty;
                        // Recupero id 
                        string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("POLICY_CONSERVAZIONE");
                        System.Diagnostics.Debug.WriteLine("SQL - InsertNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + sql);
                        logger.Debug("SQL - InsertNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + sql);
                        dbProvider.ExecuteScalar(out idPolicy, sql);
                        foreach (OggettoCustom obj in policy.template.ELENCO_OGGETTI)
                        {
                            if (!string.IsNullOrEmpty(obj.VALORE_DATABASE))
                            {
                                query = InitQuery.getInstance().getQuery("I_DPA_ASS_POLICY_PROFILAZIONE");
                                query.setParam("id_policy", idPolicy);
                                query.setParam("id_template", (policy.template.SYSTEM_ID).ToString());
                                query.setParam("obj_id", (obj.SYSTEM_ID).ToString());
                                query.setParam("obj_value", obj.VALORE_DATABASE);
                                commandText = query.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - InsertNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                                logger.Debug("SQL - InsertNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                                dbProvider.ExecuteNonQuery(commandText);
                            }
                        }
                    }
                    //dbProvider.CommitTransaction();
                }


            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }

            if (result)
                dbProvider.CommitTransaction();
            else
                dbProvider.RollbackTransaction();
            return result;
        }

        static Dictionary<string, DocsPaVO.Conservazione.ElListaPolicy> policyCache = new Dictionary<string, DocsPaVO.Conservazione.ElListaPolicy>();

        public bool SvuotaCachePolicy(string idAmm, string tipo)
        {
            bool result = true;
            try
            {
                string key = string.Format("{0}§{1}", idAmm, tipo);

                if (policyCache.ContainsKey(key))
                {
                    policyCache.Remove(key);
                }
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        public DocsPaVO.Conservazione.Policy[] GetListaPolicy(int idAmm, string tipo)
        {
            //mi creo la chiave IDAMM/tipo
            string key = String.Format("{0}§{1}", idAmm, tipo);

            //Se la policy è contenuta in cache la prendo, controllo la data, se è stata eseguita meno di 3 minuti fa la prendo per buona, se no la rieseguo.
            if (policyCache.Keys.Contains(key))
            {
                if (policyCache[key].esecuzioneQuery > DateTime.Now.AddMinutes(-3))
                {
                    logger.Debug("Query della lista policy eseguita di recente. Prelevo le info dalla cache.");
                    return policyCache[key].policyList;
                }
                else
                    policyCache.Remove(key);
            }

            DocsPaVO.Conservazione.Policy[] result = null;
            try
            {
                DataSet ds = new DataSet();
                DataSet ds2 = new DataSet();
                DataSet ds3 = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    //Prendo le policy
                    Query query = InitQuery.getInstance().getQuery("S_POLICY_CONSERVAZIONE");
                    query.setParam("idAmm", idAmm.ToString());
                    query.setParam("tipo", tipo);
                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - GetListaPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    logger.Debug("SQL - GetListaPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    dbProvider.ExecuteQuery(ds, query.getSQL());
                    //FINE QUERY

                    //Prendo i formati delle policy
                    Query query2 = InitQuery.getInstance().getQuery("S_FORMATI_POLICY_CONSERVAZIONE");
                    query2.setParam("idAmm", idAmm.ToString());
                    query2.setParam("tipo", tipo);
                    string commandText2 = query2.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - GetListaPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    logger.Debug("SQL - GetListaPolicy - DocsPaDB/Conservazione.cs - QUERY : S_FORMATI_POLICY_CONSERVAZIONE " + commandText);
                    dbProvider.ExecuteQuery(ds2, query2.getSQL());
                    //FINE SECONDA QUERY

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        result = new DocsPaVO.Conservazione.Policy[ds.Tables[0].Rows.Count];
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DocsPaVO.Conservazione.Policy tempPolicy = new DocsPaVO.Conservazione.Policy();
                            tempPolicy.system_id = ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString();
                            tempPolicy.nome = ds.Tables[0].Rows[i]["NOME"].ToString();
                            tempPolicy.classificazione = ds.Tables[0].Rows[i]["CLASSIFICA"].ToString();
                            tempPolicy.idAmministrazione = ds.Tables[0].Rows[i]["ID_AMM"].ToString();
                            tempPolicy.idRf = ds.Tables[0].Rows[i]["ID_RF"].ToString();
                            tempPolicy.idStatoDiagramma = ds.Tables[0].Rows[i]["ID_STATO"].ToString();
                            tempPolicy.idUtenteRuolo = ds.Tables[0].Rows[i]["ID_PEOPLE"].ToString();
                            tempPolicy.idTemplate = ds.Tables[0].Rows[i]["ID_TEMPLATE"].ToString();
                            tempPolicy.tipo = ds.Tables[0].Rows[i]["TIPO"].ToString();

                            if (!string.IsNullOrEmpty(tempPolicy.idTemplate))
                            {
                                //Prendo i formati delle policy
                                Query query3 = InitQuery.getInstance().getQuery("S_TEMPLATES_POLICY_CONSERVAZIONE_BY_ID");
                                query3.setParam("idPolicy", tempPolicy.system_id);
                                string commandText3 = query3.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - GetListaPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                                logger.Debug("SQL - GetListaPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                                dbProvider.ExecuteQuery(ds3, query3.getSQL());
                                //FINE TERZA QUERY
                                if (ds3 != null && ds3.Tables[0].Rows.Count > 0)
                                {
                                    Templates template = null;
                                    if (!string.IsNullOrEmpty(tempPolicy.idTemplate) && !tempPolicy.idTemplate.Equals("-1"))
                                    {
                                        if (tempPolicy.tipo.Equals("D"))
                                        {
                                            DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                                            template = model.getTemplateById(tempPolicy.idTemplate);
                                        }
                                        else
                                        {
                                            DocsPaDB.Query_DocsPAWS.ModelFasc modelFasc = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                                            template = modelFasc.getTemplateFascById(tempPolicy.idTemplate);
                                        }
                                        if (template != null)
                                        {
                                            for (int t = 0; t < ds3.Tables[0].Rows.Count; t++)
                                            {
                                                OggettoCustom obj = new OggettoCustom();
                                                obj.SYSTEM_ID = Convert.ToInt32(ds3.Tables[0].Rows[t]["ID_OBJ_CUSTOM"]);
                                                obj.VALORE_DATABASE = ds3.Tables[0].Rows[t]["valore"].ToString();

                                                if (template.ELENCO_OGGETTI != null && template.ELENCO_OGGETTI.Count > 0)
                                                {
                                                    for (int j = 0; j < template.ELENCO_OGGETTI.Count; j++)
                                                    {
                                                        if (((OggettoCustom)template.ELENCO_OGGETTI[j]).SYSTEM_ID == obj.SYSTEM_ID)
                                                        {
                                                            ((OggettoCustom)template.ELENCO_OGGETTI[j]).VALORE_DATABASE = obj.VALORE_DATABASE;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        tempPolicy.template = template;
                                    }
                                }
                            }

                            tempPolicy.idGruppo = ds.Tables[0].Rows[i]["ID_GRUPPO"].ToString();
                            tempPolicy.codiceUtente = ds.Tables[0].Rows[i]["VAR_COD_RUBRICA"].ToString();
                            tempPolicy.tipoClassificazione = ds.Tables[0].Rows[i]["TIPO_CLASSIFICAZIONE"].ToString();
                            tempPolicy.tipoDataCreazione = ds.Tables[0].Rows[i]["TIPO_DATA_CREAZIONE"].ToString();
                            tempPolicy.tipoDataProtocollazione = ds.Tables[0].Rows[i]["TIPO_DATA_PROTO"].ToString();
                            tempPolicy.idUoCreatore = ds.Tables[0].Rows[i]["ID_UO_CREATORE"].ToString();
                            string uoSottoposte = ds.Tables[0].Rows[i]["UO_SOTTOPOSTE"].ToString();
                            if (!string.IsNullOrEmpty(uoSottoposte) && uoSottoposte.Equals("1"))
                            {
                                tempPolicy.uoSottoposte = true;
                            }
                            else
                            {
                                tempPolicy.uoSottoposte = false;
                            }

                            string invioInConservazione = ds.Tables[0].Rows[i]["STATO_INVIATA"].ToString();
                            if (!string.IsNullOrEmpty(invioInConservazione) && invioInConservazione.Equals("1"))
                            {
                                tempPolicy.statoInviato = true;
                            }
                            else
                            {
                                tempPolicy.statoInviato = false;
                            }

                            // MEV 1.5 CS F02_01 conversione automatica
                            string conversione = ds.Tables[0].Rows[i]["STATO_CONVERSIONE"].ToString();
                            if (!string.IsNullOrEmpty(conversione) && conversione.Equals("1"))
                            {
                                tempPolicy.statoConversione = true;
                            }
                            else
                            {
                                tempPolicy.statoConversione = false;
                            }
                            // Fine MEV 1.5 CS F02_01 conversione automatica

                            tempPolicy.FormatiDocumento = new List<SupportedFileType>();

                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["ARRIVO"].ToString()) && ds.Tables[0].Rows[i]["ARRIVO"].ToString().Equals("1"))
                            {
                                tempPolicy.arrivo = true;
                            }
                            else
                            {
                                tempPolicy.arrivo = false;
                            }
                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["PARTENZA"].ToString()) && ds.Tables[0].Rows[i]["PARTENZA"].ToString().Equals("1"))
                            {
                                tempPolicy.partenza = true;
                            }
                            else
                            {
                                tempPolicy.partenza = false;
                            }
                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["INTERNO"].ToString()) && ds.Tables[0].Rows[i]["INTERNO"].ToString().Equals("1"))
                            {
                                tempPolicy.interno = true;
                            }
                            else
                            {
                                tempPolicy.interno = false;
                            }
                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["GRIGIO"].ToString()) && ds.Tables[0].Rows[i]["GRIGIO"].ToString().Equals("1"))
                            {
                                tempPolicy.grigio = true;
                            }
                            else
                            {
                                tempPolicy.grigio = false;
                            }
                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["CONSOLIDAZIONE"].ToString()) && ds.Tables[0].Rows[i]["CONSOLIDAZIONE"].ToString().Equals("1"))
                            {
                                tempPolicy.consolidazione = true;
                            }
                            else
                            {
                                tempPolicy.consolidazione = false;
                            }

                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["INCLUDI_SOTTONODI"].ToString()) && ds.Tables[0].Rows[i]["INCLUDI_SOTTONODI"].ToString().Equals("1"))
                            {
                                tempPolicy.includiSottoNodi = true;
                            }
                            else
                            {
                                tempPolicy.includiSottoNodi = false;
                            }

                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["SOLO_DIGITALI"].ToString()) && ds.Tables[0].Rows[i]["SOLO_DIGITALI"].ToString().Equals("1"))
                            {
                                tempPolicy.soloDigitali = true;
                            }
                            else
                            {
                                tempPolicy.soloDigitali = false;
                            }

                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["SOLO_FIRMATI"].ToString()) && ds.Tables[0].Rows[i]["SOLO_FIRMATI"].ToString().Equals("1"))
                            {
                                tempPolicy.soloFirmati = true;
                            }
                            else
                            {
                                tempPolicy.soloFirmati = false;
                            }

                            tempPolicy.periodoAnnualeGiorno = ds.Tables[0].Rows[i]["P_A_GIORNI"].ToString();
                            tempPolicy.periodoAnnualeMese = ds.Tables[0].Rows[i]["P_A_MESE"].ToString();
                            tempPolicy.periodoAnnualeOre = ds.Tables[0].Rows[i]["P_A_ORE"].ToString();
                            tempPolicy.periodoAnnualeMinuti = ds.Tables[0].Rows[i]["P_A_MINUTI"].ToString();
                            tempPolicy.tipoConservazione = ds.Tables[0].Rows[i]["TIPO_CONSERVAZIONE"].ToString();

                            
                            tempPolicy.idAOO = ds.Tables[0].Rows[i]["ID_AOO"].ToString();
                            tempPolicy.dataCreazioneDa = ds.Tables[0].Rows[i]["DTA_DATA_CREAZIONE_DA"].ToString();
                            tempPolicy.dataCreazioneA = ds.Tables[0].Rows[i]["DTA_DATA_CREAZIONE_A"].ToString();
                            tempPolicy.dataProtocollazioneDa = ds.Tables[0].Rows[i]["DTA_DATA_PROTO_DA"].ToString();
                            tempPolicy.dataProtocollazioneA = ds.Tables[0].Rows[i]["DTA_DATA_PROTO_A"].ToString();
                            tempPolicy.tipoPeriodo = ds.Tables[0].Rows[i]["PERIODO_TIPO"].ToString();
                            tempPolicy.periodoGiornalieroNGiorni = ds.Tables[0].Rows[i]["P_G_GIORNI"].ToString();
                            tempPolicy.periodoGiornalieroOre = ds.Tables[0].Rows[i]["P_G_ORA_HH"].ToString();
                            tempPolicy.periodoGiornalieroMinuti = ds.Tables[0].Rows[i]["P_G_ORA_MM"].ToString();
                            tempPolicy.periodoMensileGiorni = ds.Tables[0].Rows[i]["P_M_GIORNI"].ToString();
                            tempPolicy.periodoMensileOre = ds.Tables[0].Rows[i]["P_M_ORA_HH"].ToString();
                            tempPolicy.periodoMensileMinuti = ds.Tables[0].Rows[i]["P_M_ORA_MM"].ToString();
                            tempPolicy.periodoSettimanaleOre = ds.Tables[0].Rows[i]["P_S_ORA_HH"].ToString();
                            tempPolicy.periodoSettimanaleMinuti = ds.Tables[0].Rows[i]["P_S_ORA_MM"].ToString();
                            tempPolicy.idRuolo = ds.Tables[0].Rows[i]["ID_RUOLO"].ToString();
                            tempPolicy.avvisoMesi = ds.Tables[0].Rows[i]["MESI_AVVISO"].ToString();
                            // MEV CS 1.5
                            tempPolicy.avvisoMesiLegg = ds.Tables[0].Rows[i]["MESI_AVVISO_LEGGIBILITA"].ToString();
                            // fine MEV CS 1.5

                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["P_S_LUNEDI"].ToString()) && ds.Tables[0].Rows[i]["P_S_LUNEDI"].ToString().Equals("1"))
                            {
                                tempPolicy.periodoSettimanaleLunedi = true;
                            }
                            else
                            {
                                tempPolicy.periodoSettimanaleLunedi = false;
                            }
                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["P_S_MARTEDI"].ToString()) && ds.Tables[0].Rows[i]["P_S_MARTEDI"].ToString().Equals("1"))
                            {
                                tempPolicy.periodoSettimanaleMartedi = true;
                            }
                            else
                            {
                                tempPolicy.periodoSettimanaleMartedi = false;
                            }
                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["P_S_MERCOLEDI"].ToString()) && ds.Tables[0].Rows[i]["P_S_MERCOLEDI"].ToString().Equals("1"))
                            {
                                tempPolicy.periodoSettimanaleMercoledi = true;
                            }
                            else
                            {
                                tempPolicy.periodoSettimanaleMercoledi = false;
                            }
                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["P_S_GIOVEDI"].ToString()) && ds.Tables[0].Rows[i]["P_S_GIOVEDI"].ToString().Equals("1"))
                            {
                                tempPolicy.periodoSettimanaleGiovedi = true;
                            }
                            else
                            {
                                tempPolicy.periodoSettimanaleGiovedi = false;
                            }
                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["P_S_VENERDI"].ToString()) && ds.Tables[0].Rows[i]["P_S_VENERDI"].ToString().Equals("1"))
                            {
                                tempPolicy.periodoSettimanaleVenerdi = true;
                            }
                            else
                            {
                                tempPolicy.periodoSettimanaleVenerdi = false;
                            }
                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["P_S_SABATO"].ToString()) && ds.Tables[0].Rows[i]["P_S_SABATO"].ToString().Equals("1"))
                            {
                                tempPolicy.periodoSettimanaleSabato = true;
                            }
                            else
                            {
                                tempPolicy.periodoSettimanaleSabato = false;
                            }
                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["P_S_DOMENICA"].ToString()) && ds.Tables[0].Rows[i]["P_S_DOMENICA"].ToString().Equals("1"))
                            {
                                tempPolicy.periodoSettimanaleDomenica = true;
                            }
                            else
                            {
                                tempPolicy.periodoSettimanaleDomenica = false;
                            }

                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["ATTIVA"].ToString()) && ds.Tables[0].Rows[i]["ATTIVA"].ToString().Equals("1"))
                            {
                                tempPolicy.periodoAttivo = true;
                            }
                            else
                            {
                                tempPolicy.periodoAttivo = false;
                            }

                            if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                            {
                                for (int t = 0; t < ds2.Tables[0].Rows.Count; t++)
                                {
                                    if (tempPolicy.system_id.Equals(ds2.Tables[0].Rows[t]["id_policy"].ToString()))
                                    {
                                        SupportedFileType temp = new SupportedFileType();
                                        temp.SystemId = Convert.ToInt32(ds2.Tables[0].Rows[t]["ID_TYPE"]);
                                        temp.FileExtension = ds2.Tables[0].Rows[t]["FILE_EXTENSION"].ToString();
                                        tempPolicy.FormatiDocumento.Add(temp);
                                    }
                                }
                            }

                            result[i] = tempPolicy;
                        }
                    }

                }
            }
            catch (Exception e)
            {
                result = null;
                logger.Debug(e.Message);
            }

            //Aggiunge l'item in cache
            if (!policyCache.Keys.Contains(key))
            {
                policyCache.Add(key, new DocsPaVO.Conservazione.ElListaPolicy
                {
                    esecuzioneQuery = DateTime.Now,
                    policyList = result
                });
            }

            return result;
        }




        public bool DeletePolicy(string idPolicy)
        {
            bool result = true;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    // Reperimento dell'id della griglia standard
                    Query query = InitQuery.getInstance().getQuery("D_DELETE_POLICY_CONSERVAZIONE");
                    query.setParam("idPolicy", idPolicy);
                    dbProvider.ExecuteNonQuery(query.getSQL());

                    // Reperimento dell'id della griglia standard
                    Query query2 = InitQuery.getInstance().getQuery("D_DELETE_DPA_ASS_POLICY_TYPE");
                    query2.setParam("idPolicy", idPolicy);
                    dbProvider.ExecuteNonQuery(query2.getSQL());

                    // Reperimento dell'id della griglia standard
                    Query query3 = InitQuery.getInstance().getQuery("D_DELETE_DPA_ASS_POLICY_PROFILAZIONE");
                    query3.setParam("idPolicy", idPolicy);
                    dbProvider.ExecuteNonQuery(query3.getSQL());

                    dbProvider.CommitTransaction();
                }

            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanzaConservazione"></param>
        /// <returns></returns>
        public DocsPaVO.Conservazione.Policy GetPolicyByIdIstanzaConservazione(string idIstanzaConservazione)
        {
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_POLICY_CONSERVAZIONE_BY_ID_ISTANZA_CONSERVAZIONE");
                query.setParam("systemId", idIstanzaConservazione);

                string commandText = query.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - GetPolicyByIdIstanzaConservazione - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                logger.Debug("SQL - GetPolicyByIdIstanzaConservazione - DocsPaDB/Conservazione.cs - QUERY : " + commandText);

                string field;
                if (!dbProvider.ExecuteScalar(out field, commandText))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                if (!string.IsNullOrEmpty(field))
                    return this.GetPolicyById(field);
                else
                    return null;
            }
        }

        public DocsPaVO.Conservazione.Policy GetPolicyValidazioneByIdIstanzaConservazione(string idIstanzaConservazione)
        {

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_POLICY_VALID_CONSERVAZIONE_BY_ID_ISTANZA_CONSERVAZIONE");
                query.setParam("systemId", idIstanzaConservazione);

                string commandText = query.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - GetPolicyValidazioneByIdIstanzaConservazione - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                logger.Debug("SQL - GetPolicyValidazioneByIdIstanzaConservazione - DocsPaDB/Conservazione.cs - QUERY : " + commandText);

                string field;
                if (!dbProvider.ExecuteScalar(out field, commandText))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                if (!string.IsNullOrEmpty(field))
                    return this.GetPolicyById(field);
                else
                    return null;
            }

        }

        public DocsPaVO.Conservazione.Policy GetPolicyById(string idPolicy)
        {
            DocsPaVO.Conservazione.Policy result = null;
            try
            {
                DataSet ds = new DataSet();
                DataSet ds2 = new DataSet();
                DataSet ds3 = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    //Prendo le policy
                    Query query = InitQuery.getInstance().getQuery("S_POLICY_CONSERVAZIONE_BY_ID");
                    query.setParam("idPolicy", idPolicy);
                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - GetListaPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    logger.Debug("SQL - GetListaPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    dbProvider.ExecuteQuery(ds, query.getSQL());
                    //FINE QUERY

                    //Prendo i formati delle policy
                    Query query2 = InitQuery.getInstance().getQuery("S_FORMATI_POLICY_CONSERVAZIONE_BY_ID");
                    query2.setParam("idPolicy", idPolicy);
                    string commandText2 = query2.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - GetListaPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    logger.Debug("SQL - GetListaPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    dbProvider.ExecuteQuery(ds2, query2.getSQL());
                    //FINE SECONDA QUERY

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        result = new DocsPaVO.Conservazione.Policy();
                        DocsPaVO.Conservazione.Policy tempPolicy = new DocsPaVO.Conservazione.Policy();
                        tempPolicy.system_id = ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString();
                        tempPolicy.nome = ds.Tables[0].Rows[0]["NOME"].ToString();
                        tempPolicy.classificazione = ds.Tables[0].Rows[0]["CLASSIFICA"].ToString();
                        tempPolicy.idAmministrazione = ds.Tables[0].Rows[0]["ID_AMM"].ToString();
                        tempPolicy.idRf = ds.Tables[0].Rows[0]["ID_RF"].ToString();
                        tempPolicy.idStatoDiagramma = ds.Tables[0].Rows[0]["ID_STATO"].ToString();
                        tempPolicy.idUtenteRuolo = ds.Tables[0].Rows[0]["ID_PEOPLE"].ToString();
                        tempPolicy.idTemplate = ds.Tables[0].Rows[0]["ID_TEMPLATE"].ToString();
                        tempPolicy.tipo = ds.Tables[0].Rows[0]["TIPO"].ToString();

                        if (!string.IsNullOrEmpty(tempPolicy.idTemplate))
                        {
                            //Prendo i formati delle policy
                            Query query3 = InitQuery.getInstance().getQuery("S_TEMPLATES_POLICY_CONSERVAZIONE_BY_ID");
                            query3.setParam("idPolicy", idPolicy);
                            string commandText3 = query3.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - GetListaPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                            logger.Debug("SQL - GetListaPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                            dbProvider.ExecuteQuery(ds3, query3.getSQL());
                            //FINE TERZA QUERY
                            if (ds3 != null && ds3.Tables[0].Rows.Count > 0)
                            {
                                Templates template = null;
                                if (!string.IsNullOrEmpty(tempPolicy.idTemplate) && !tempPolicy.idTemplate.Equals("-1"))
                                {
                                    if (tempPolicy.tipo.Equals("D"))
                                    {
                                        DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                                        template = model.getTemplateById(tempPolicy.idTemplate);
                                    }
                                    else
                                    {
                                        DocsPaDB.Query_DocsPAWS.ModelFasc modelFasc = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                                        template = modelFasc.getTemplateFascById(tempPolicy.idTemplate);
                                    }
                                    if (template != null)
                                    {
                                        for (int t = 0; t < ds3.Tables[0].Rows.Count; t++)
                                        {
                                            OggettoCustom obj = new OggettoCustom();
                                            obj.SYSTEM_ID = Convert.ToInt32(ds3.Tables[0].Rows[t]["ID_OBJ_CUSTOM"]);
                                            obj.VALORE_DATABASE = ds3.Tables[0].Rows[t]["valore"].ToString();

                                            if (template.ELENCO_OGGETTI != null && template.ELENCO_OGGETTI.Count > 0)
                                            {
                                                for (int j = 0; j < template.ELENCO_OGGETTI.Count; j++)
                                                {
                                                    if (((OggettoCustom)template.ELENCO_OGGETTI[j]).SYSTEM_ID == obj.SYSTEM_ID)
                                                    {
                                                        ((OggettoCustom)template.ELENCO_OGGETTI[j]).VALORE_DATABASE = obj.VALORE_DATABASE;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    tempPolicy.template = template;
                                }
                            }
                        }


                        tempPolicy.idGruppo = ds.Tables[0].Rows[0]["ID_GRUPPO"].ToString();
                        tempPolicy.codiceUtente = ds.Tables[0].Rows[0]["VAR_COD_RUBRICA"].ToString();
                        tempPolicy.tipoClassificazione = ds.Tables[0].Rows[0]["TIPO_CLASSIFICAZIONE"].ToString();
                        tempPolicy.tipoDataCreazione = ds.Tables[0].Rows[0]["TIPO_DATA_CREAZIONE"].ToString();
                        tempPolicy.tipoDataProtocollazione = ds.Tables[0].Rows[0]["TIPO_DATA_PROTO"].ToString();
                        tempPolicy.idUoCreatore = ds.Tables[0].Rows[0]["ID_UO_CREATORE"].ToString();
                        string uoSottoposte = ds.Tables[0].Rows[0]["UO_SOTTOPOSTE"].ToString();
                        if (!string.IsNullOrEmpty(uoSottoposte) && uoSottoposte.Equals("1"))
                        {
                            tempPolicy.uoSottoposte = true;
                        }
                        else
                        {
                            tempPolicy.uoSottoposte = false;
                        }

                        string invioInConservazione = ds.Tables[0].Rows[0]["STATO_INVIATA"].ToString();
                        if (!string.IsNullOrEmpty(invioInConservazione) && invioInConservazione.Equals("1"))
                        {
                            tempPolicy.statoInviato = true;
                        }
                        else
                        {
                            tempPolicy.statoInviato = false;
                        }

                        // MEV 1.5 CS F02_01 conversione automatica
                        string conversione = ds.Tables[0].Rows[0]["STATO_CONVERSIONE"].ToString();
                        if (!string.IsNullOrEmpty(conversione) && conversione.Equals("1"))
                        {
                            tempPolicy.statoConversione = true;
                        }
                        else
                        {
                            tempPolicy.statoConversione = false;
                        }
                        // Fine MEV 1.5 CS F02_01 conversione automatica

                        tempPolicy.FormatiDocumento = new List<SupportedFileType>();

                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["ARRIVO"].ToString()) && ds.Tables[0].Rows[0]["ARRIVO"].ToString().Equals("1"))
                        {
                            tempPolicy.arrivo = true;
                        }
                        else
                        {
                            tempPolicy.arrivo = false;
                        }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["PARTENZA"].ToString()) && ds.Tables[0].Rows[0]["PARTENZA"].ToString().Equals("1"))
                        {
                            tempPolicy.partenza = true;
                        }
                        else
                        {
                            tempPolicy.partenza = false;
                        }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["INTERNO"].ToString()) && ds.Tables[0].Rows[0]["INTERNO"].ToString().Equals("1"))
                        {
                            tempPolicy.interno = true;
                        }
                        else
                        {
                            tempPolicy.interno = false;
                        }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["GRIGIO"].ToString()) && ds.Tables[0].Rows[0]["GRIGIO"].ToString().Equals("1"))
                        {
                            tempPolicy.grigio = true;
                        }
                        else
                        {
                            tempPolicy.grigio = false;
                        }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["CONSOLIDAZIONE"].ToString()) && ds.Tables[0].Rows[0]["CONSOLIDAZIONE"].ToString().Equals("1"))
                        {
                            tempPolicy.consolidazione = true;
                        }
                        else
                        {
                            tempPolicy.consolidazione = false;
                        }

                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["INCLUDI_SOTTONODI"].ToString()) && ds.Tables[0].Rows[0]["INCLUDI_SOTTONODI"].ToString().Equals("1"))
                        {
                            tempPolicy.includiSottoNodi = true;
                        }
                        else
                        {
                            tempPolicy.includiSottoNodi = false;
                        }

                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["SOLO_DIGITALI"].ToString()) && ds.Tables[0].Rows[0]["SOLO_DIGITALI"].ToString().Equals("1"))
                        {
                            tempPolicy.soloDigitali = true;
                        }
                        else
                        {
                            tempPolicy.soloDigitali = false;
                        }

                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["SOLO_FIRMATI"].ToString()) && ds.Tables[0].Rows[0]["SOLO_FIRMATI"].ToString().Equals("1"))
                        {
                            tempPolicy.soloFirmati = true;
                        }
                        else
                        {
                            tempPolicy.soloFirmati = false;
                        }

                        tempPolicy.periodoAnnualeGiorno = ds.Tables[0].Rows[0]["P_A_GIORNI"].ToString();
                        tempPolicy.periodoAnnualeMese = ds.Tables[0].Rows[0]["P_A_MESE"].ToString();
                        tempPolicy.periodoAnnualeOre = ds.Tables[0].Rows[0]["P_A_ORE"].ToString();
                        tempPolicy.periodoAnnualeMinuti = ds.Tables[0].Rows[0]["P_A_MINUTI"].ToString();
                        tempPolicy.tipoConservazione = ds.Tables[0].Rows[0]["TIPO_CONSERVAZIONE"].ToString();

                       
                        tempPolicy.idAOO = ds.Tables[0].Rows[0]["ID_AOO"].ToString();
                        tempPolicy.dataCreazioneDa = ds.Tables[0].Rows[0]["DTA_DATA_CREAZIONE_DA"].ToString();
                        tempPolicy.dataCreazioneA = ds.Tables[0].Rows[0]["DTA_DATA_CREAZIONE_A"].ToString();
                        tempPolicy.dataProtocollazioneDa = ds.Tables[0].Rows[0]["DTA_DATA_PROTO_DA"].ToString();
                        tempPolicy.dataProtocollazioneA = ds.Tables[0].Rows[0]["DTA_DATA_PROTO_A"].ToString();
                        tempPolicy.tipoPeriodo = ds.Tables[0].Rows[0]["PERIODO_TIPO"].ToString();
                        tempPolicy.periodoGiornalieroNGiorni = ds.Tables[0].Rows[0]["P_G_GIORNI"].ToString();
                        tempPolicy.periodoGiornalieroOre = ds.Tables[0].Rows[0]["P_G_ORA_HH"].ToString();
                        tempPolicy.periodoGiornalieroMinuti = ds.Tables[0].Rows[0]["P_G_ORA_MM"].ToString();
                        tempPolicy.periodoMensileGiorni = ds.Tables[0].Rows[0]["P_M_GIORNI"].ToString();
                        tempPolicy.periodoMensileOre = ds.Tables[0].Rows[0]["P_M_ORA_HH"].ToString();
                        tempPolicy.periodoMensileMinuti = ds.Tables[0].Rows[0]["P_M_ORA_MM"].ToString();
                        tempPolicy.periodoSettimanaleOre = ds.Tables[0].Rows[0]["P_S_ORA_HH"].ToString();
                        tempPolicy.periodoSettimanaleMinuti = ds.Tables[0].Rows[0]["P_S_ORA_MM"].ToString();
                        tempPolicy.idRuolo = ds.Tables[0].Rows[0]["ID_RUOLO"].ToString();
                        tempPolicy.avvisoMesi = ds.Tables[0].Rows[0]["MESI_AVVISO"].ToString();
                        // MEV CS 1.5
                        tempPolicy.avvisoMesiLegg = ds.Tables[0].Rows[0]["MESI_AVVISO_LEGGIBILITA"].ToString();
                        // fine MEV CS 1.5

                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["P_S_LUNEDI"].ToString()) && ds.Tables[0].Rows[0]["P_S_LUNEDI"].ToString().Equals("1"))
                        {
                            tempPolicy.periodoSettimanaleLunedi = true;
                        }
                        else
                        {
                            tempPolicy.periodoSettimanaleLunedi = false;
                        }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["P_S_MARTEDI"].ToString()) && ds.Tables[0].Rows[0]["P_S_MARTEDI"].ToString().Equals("1"))
                        {
                            tempPolicy.periodoSettimanaleMartedi = true;
                        }
                        else
                        {
                            tempPolicy.periodoSettimanaleMartedi = false;
                        }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["P_S_MERCOLEDI"].ToString()) && ds.Tables[0].Rows[0]["P_S_MERCOLEDI"].ToString().Equals("1"))
                        {
                            tempPolicy.periodoSettimanaleMercoledi = true;
                        }
                        else
                        {
                            tempPolicy.periodoSettimanaleMercoledi = false;
                        }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["P_S_GIOVEDI"].ToString()) && ds.Tables[0].Rows[0]["P_S_GIOVEDI"].ToString().Equals("1"))
                        {
                            tempPolicy.periodoSettimanaleGiovedi = true;
                        }
                        else
                        {
                            tempPolicy.periodoSettimanaleGiovedi = false;
                        }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["P_S_VENERDI"].ToString()) && ds.Tables[0].Rows[0]["P_S_VENERDI"].ToString().Equals("1"))
                        {
                            tempPolicy.periodoSettimanaleVenerdi = true;
                        }
                        else
                        {
                            tempPolicy.periodoSettimanaleVenerdi = false;
                        }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["P_S_SABATO"].ToString()) && ds.Tables[0].Rows[0]["P_S_SABATO"].ToString().Equals("1"))
                        {
                            tempPolicy.periodoSettimanaleSabato = true;
                        }
                        else
                        {
                            tempPolicy.periodoSettimanaleSabato = false;
                        }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["P_S_DOMENICA"].ToString()) && ds.Tables[0].Rows[0]["P_S_DOMENICA"].ToString().Equals("1"))
                        {
                            tempPolicy.periodoSettimanaleDomenica = true;
                        }
                        else
                        {
                            tempPolicy.periodoSettimanaleDomenica = false;
                        }

                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["ATTIVA"].ToString()) && ds.Tables[0].Rows[0]["ATTIVA"].ToString().Equals("1"))
                        {
                            tempPolicy.periodoAttivo = true;
                        }
                        else
                        {
                            tempPolicy.periodoAttivo = false;
                        }

                        if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                        {

                            for (int t = 0; t < ds2.Tables[0].Rows.Count; t++)
                            {
                                if (tempPolicy.system_id.Equals(ds2.Tables[0].Rows[t]["id_policy"].ToString()))
                                {
                                    SupportedFileType temp = new SupportedFileType();
                                    temp.SystemId = Convert.ToInt32(ds2.Tables[0].Rows[t]["ID_TYPE"]);
                                    temp.FileExtension = ds2.Tables[0].Rows[t]["FILE_EXTENSION"].ToString();
                                    tempPolicy.FormatiDocumento.Add(temp);
                                }
                            }
                        }

                        result = tempPolicy;
                    }
                }
            }
            catch (Exception e)
            {
                result = null;
                logger.Debug(e.Message);
            }

            return result;
        }

        public bool SavePeriodPolicy(DocsPaVO.Conservazione.Policy policy)
        {
            bool result = true;
            try
            {

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    Query query = InitQuery.getInstance().getQuery("U_PERIODO_CONSERVAZIONE");
                    query.setParam("idPolicy", policy.system_id);

                    if (policy.consolidazione)
                    {
                        query.setParam("consolidazione", "1");
                    }
                    else
                    {
                        query.setParam("consolidazione", string.Empty);
                    }

                    if (policy.statoInviato)
                    {
                        query.setParam("statoInv", "1");
                    }
                    else
                    {
                        query.setParam("statoInv", string.Empty);
                    }

                    // MEV CS 1.5 Conversione Automatica
                    if (policy.statoConversione)
                    {
                        query.setParam("statoConv", "1");
                    }
                    else
                    {
                        query.setParam("statoConv", string.Empty);
                    }
                    // Fine MEV CS 1.5 Conversione Automatica

                    query.setParam("periodo", policy.tipoPeriodo);
                    query.setParam("numGiorni", policy.periodoGiornalieroNGiorni);
                    query.setParam("oraGiorni", policy.periodoGiornalieroOre);
                    query.setParam("minutiGiorni", policy.periodoGiornalieroMinuti);

                    if (policy.periodoSettimanaleLunedi)
                    {
                        query.setParam("lunedi", "1");
                    }
                    else
                    {
                        query.setParam("lunedi", string.Empty);
                    }
                    if (policy.periodoSettimanaleMartedi)
                    {
                        query.setParam("martedi", "1");
                    }
                    else
                    {
                        query.setParam("martedi", string.Empty);
                    }
                    if (policy.periodoSettimanaleMercoledi)
                    {
                        query.setParam("mercoledi", "1");
                    }
                    else
                    {
                        query.setParam("mercoledi", string.Empty);
                    }
                    if (policy.periodoSettimanaleGiovedi)
                    {
                        query.setParam("giovedi", "1");
                    }
                    else
                    {
                        query.setParam("giovedi", string.Empty);
                    }
                    if (policy.periodoSettimanaleVenerdi)
                    {
                        query.setParam("venerdi", "1");
                    }
                    else
                    {
                        query.setParam("venerdi", string.Empty);
                    }
                    if (policy.periodoSettimanaleSabato)
                    {
                        query.setParam("sabato", "1");
                    }
                    else
                    {
                        query.setParam("sabato", string.Empty);
                    }
                    if (policy.periodoSettimanaleDomenica)
                    {
                        query.setParam("domenica", "1");
                    }
                    else
                    {
                        query.setParam("domenica", string.Empty);
                    }

                    query.setParam("mesiGiorni", policy.periodoMensileGiorni);
                    query.setParam("oreMese", policy.periodoMensileOre);
                    query.setParam("minutiMese", policy.periodoMensileMinuti);
                    if (!string.IsNullOrEmpty(policy.idRuolo))
                    {
                        query.setParam("idRuolo", policy.idRuolo);
                    }
                    else
                    {
                        query.setParam("idRuolo", "-1");
                    }

                    if (!string.IsNullOrEmpty(policy.idUtenteRuolo))
                    {
                        query.setParam("idPeople", policy.idUtenteRuolo);
                    }
                    else
                    {
                        query.setParam("idPeople", "-1");
                    }

                    query.setParam("numMesi", policy.avvisoMesi);
                    // MEV CS 1.5
                    query.setParam("numMesiLegg", policy.avvisoMesiLegg);
                    // fine MEV CS 1.5

                    if (policy.periodoAttivo)
                    {
                        query.setParam("attiva", "1");
                    }
                    else
                    {
                        query.setParam("attiva", string.Empty);
                    }

                    query.setParam("oreSettimana", policy.periodoSettimanaleOre);
                    query.setParam("minutiSettimana", policy.periodoSettimanaleMinuti);

                    query.setParam("annoGiorni", policy.periodoAnnualeGiorno);
                    query.setParam("annoMese", policy.periodoAnnualeMese);
                    query.setParam("annoOre", policy.periodoAnnualeOre);
                    query.setParam("annoMinuti", policy.periodoAnnualeMinuti);
                    query.setParam("tipoConservazione", policy.tipoConservazione);

                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - SavePeriodPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    logger.Debug("SQL - SavePeriodPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);

                    dbProvider.CommitTransaction();
                }


            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }

            return result;
        }


        public bool ModifyNewPolicy(DocsPaVO.Conservazione.Policy policy)
        {
            bool result = true;
            try
            {

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    Query query = InitQuery.getInstance().getQuery("U_POLICY_CONSERVAZIONE");
                    query.setParam("idPolicy", policy.system_id);
                    query.setParam("nome", policy.nome);
                    query.setParam("tipoClass", policy.tipoClassificazione);

                    query.setParam("tipo", policy.tipo);

                    if (!string.IsNullOrEmpty(policy.classificazione))
                    {
                        query.setParam("classificazione", policy.classificazione);
                    }
                    else
                    {
                        query.setParam("classificazione", "-1");
                    }

                    if (policy.includiSottoNodi)
                    {
                        query.setParam("sottoNodi", "1");
                    }
                    else
                    {
                        query.setParam("sottoNodi", "");
                    }

                    if (policy.soloDigitali)
                    {
                        query.setParam("digitali", "1");
                    }
                    else
                    {
                        query.setParam("digitali", "");
                    }

                    if (policy.soloFirmati)
                    {
                        query.setParam("firmati", "1");
                    }
                    else
                    {
                        query.setParam("firmati", "");
                    }

                    if (!string.IsNullOrEmpty(policy.idTemplate))
                    {
                        query.setParam("template", policy.idTemplate);
                    }
                    else
                    {
                        query.setParam("template", "-1");
                    }

                    if (!string.IsNullOrEmpty(policy.idStatoDiagramma))
                    {
                        query.setParam("diagramma", policy.idStatoDiagramma);
                    }
                    else
                    {
                        query.setParam("diagramma", "-1");
                    }
                    if (!string.IsNullOrEmpty(policy.idAOO))
                    {
                        query.setParam("aoo", policy.idAOO);
                    }
                    else
                    {
                        query.setParam("aoo", "-1");
                    }
                    if (!string.IsNullOrEmpty(policy.idRf))
                    {
                        query.setParam("rf", policy.idRf);
                    }
                    else
                    {
                        query.setParam("rf", "-1");
                    }
                    if (!string.IsNullOrEmpty(policy.idUoCreatore))
                    {
                        query.setParam("idUoCreatore", policy.idUoCreatore);
                    }
                    else
                    {
                        query.setParam("idUoCreatore", "-1");
                    }
                    if (policy.uoSottoposte)
                    {
                        query.setParam("uoSott", "1");
                    }
                    else
                    {
                        query.setParam("uoSott", null);
                    }

                    if (policy.tipo.Equals("D"))
                    {
                        if (policy.arrivo)
                        {
                            query.setParam("arrivo", "1");
                        }
                        else
                        {
                            query.setParam("arrivo", "0");
                        }
                        if (policy.partenza)
                        {
                            query.setParam("partenza", "1");
                        }
                        else
                        {
                            query.setParam("partenza", "0");
                        }
                        if (policy.interno)
                        {
                            query.setParam("interno", "1");
                        }
                        else
                        {
                            query.setParam("interno", "0");
                        }
                        if (policy.grigio)
                        {
                            query.setParam("grigio", "1");
                        }
                        else
                        {
                            query.setParam("grigio", "0");
                        }

                        if (!string.IsNullOrEmpty(policy.dataCreazioneDa))
                        {
                            query.setParam("dataCDa", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa));
                        }
                        else
                        {
                            query.setParam("dataCDa", "''");
                        }
                        if (!string.IsNullOrEmpty(policy.dataCreazioneA))
                        {
                            query.setParam("dataCA", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneA));
                        }
                        else
                        {
                            query.setParam("dataCA", "''");
                        }
                        if (!string.IsNullOrEmpty(policy.dataProtocollazioneDa))
                        {
                            query.setParam("dataPDa", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneDa));
                        }
                        else
                        {
                            query.setParam("dataPDa", "''");
                        }
                        if (!string.IsNullOrEmpty(policy.dataProtocollazioneA))
                        {
                            query.setParam("dataPA", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneA));
                        }
                        else
                        {
                            query.setParam("dataPA", "''");
                        }

                        if (!string.IsNullOrEmpty(policy.tipoDataCreazione))
                        {
                            query.setParam("tipoDataC", policy.tipoDataCreazione);
                        }
                        else
                        {
                            query.setParam("tipoDataC", null);
                        }
                        if (!string.IsNullOrEmpty(policy.tipoDataProtocollazione))
                        {
                            query.setParam("tipoDataP", policy.tipoDataProtocollazione);
                        }
                        else
                        {
                            query.setParam("tipoDataP", null);
                        }


                    }
                    else
                    {
                        if (policy.tipo.Equals("R") || policy.tipo.Equals("C"))
                        {
                            if (!string.IsNullOrEmpty(policy.tipoDataCreazione))
                            {
                                query.setParam("tipoDataC", policy.tipoDataCreazione);
                            }
                            else
                            {
                                query.setParam("tipoDataC", null);
                            }
                            if (!string.IsNullOrEmpty(policy.dataCreazioneDa))
                            {
                                query.setParam("dataCDa", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa));
                            }
                            else
                            {
                                query.setParam("dataCDa", "''");
                            }
                            if (!string.IsNullOrEmpty(policy.dataCreazioneA))
                            {
                                query.setParam("dataCA", DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneA));
                            }
                            else
                            {
                                query.setParam("dataCA", "''");
                            }
                        }
                        else
                        {
                            query.setParam("dataCDa", "''");
                            query.setParam("dataCA", "''");
                            query.setParam("tipoDataC", null);
                        }
                        query.setParam("grigio", "0");
                        query.setParam("interno", "0");
                        query.setParam("partenza", "0");
                        query.setParam("arrivo", "0");


                        query.setParam("dataPDa", "''");
                        query.setParam("dataPA", "''");

                        query.setParam("tipoDataP", null);
                    }
                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - ModifyNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    logger.Debug("SQL - ModifyNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);

                    // Reperimento dell'id della griglia standard
                    Query query2 = InitQuery.getInstance().getQuery("D_DELETE_DPA_ASS_POLICY_TYPE");
                    query2.setParam("idPolicy", policy.system_id);
                    dbProvider.ExecuteNonQuery(query2.getSQL());

                    if (policy.FormatiDocumento != null && policy.FormatiDocumento.Count > 0)
                    {

                        foreach (SupportedFileType temp in policy.FormatiDocumento)
                        {
                            query = InitQuery.getInstance().getQuery("I_DPA_ASS_POLICY_TYPE");
                            query.setParam("id_policy", policy.system_id);
                            query.setParam("id_type", (temp.SystemId).ToString());
                            commandText = query.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - ModifyNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                            logger.Debug("SQL - ModifyNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);
                        }
                    }

                    // Reperimento dell'id della griglia standard
                    Query query3 = InitQuery.getInstance().getQuery("D_DELETE_DPA_ASS_POLICY_PROFILAZIONE");
                    query3.setParam("idPolicy", policy.system_id);
                    dbProvider.ExecuteNonQuery(query3.getSQL());

                    //Inserisci campi profilati
                    if (policy.template != null && policy.template.ELENCO_OGGETTI != null && policy.template.ELENCO_OGGETTI.Count > 0)
                    {
                        foreach (OggettoCustom obj in policy.template.ELENCO_OGGETTI)
                        {
                            if (!string.IsNullOrEmpty(obj.VALORE_DATABASE))
                            {
                                query = InitQuery.getInstance().getQuery("I_DPA_ASS_POLICY_PROFILAZIONE");
                                query.setParam("id_policy", policy.system_id);
                                query.setParam("id_template", (policy.template.SYSTEM_ID).ToString());
                                query.setParam("obj_id", (obj.SYSTEM_ID).ToString());
                                query.setParam("obj_value", obj.VALORE_DATABASE);
                                commandText = query.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - ModifyNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                                logger.Debug("SQL - ModifyNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                                dbProvider.ExecuteNonQuery(commandText);
                            }
                        }
                    }

                    dbProvider.CommitTransaction();
                }


            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }

            return result;
        }


        public DocsPaVO.Conservazione.ExecutionPolicy GetLastExecutionPolicyByIdPolicy(string idPolicy)
        {
            DocsPaVO.Conservazione.ExecutionPolicy result = null;
            try
            {
                DataSet ds = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    //Prendo le policy
                    Query query = InitQuery.getInstance().getQuery("S_GET_LAST_EXECUTION_POLICY_BY_IDPOLICY");
                    query.setParam("idPolicy", idPolicy);
                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - GetExecutionPolicyById - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    logger.Debug("SQL - GetExecutionPolicyById - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    dbProvider.ExecuteQuery(ds, query.getSQL());
                    //FINE QUERY

                    if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                        result = new DocsPaVO.Conservazione.ExecutionPolicy();
                        result.idExecution = ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString();
                        result.idPolicy = ds.Tables[0].Rows[0]["ID_POLICY"].ToString();
                        result.idIstanza = ds.Tables[0].Rows[0]["ID_ISTANZA"].ToString();
                        result.idFirstDocumentProcessed = ds.Tables[0].Rows[0]["ID_PRIMO_OBJ"].ToString();
                        result.idLastDocumentProcessed = ds.Tables[0].Rows[0]["ID_ULTIMO_OBJ"].ToString();
                        result.docNumberProcessed = ds.Tables[0].Rows[0]["N_OBJ_CONSERVATI"].ToString();
                        result.startExecutionDate = ds.Tables[0].Rows[0]["START_EXECUTE_DATE"].ToString();
                        result.endExecutionDate = ds.Tables[0].Rows[0]["END_EXECUTE_DATE"].ToString();
                        result.idAmm = ds.Tables[0].Rows[0]["ID_AMM"].ToString();
                        if (string.IsNullOrEmpty(result.endExecutionDate))
                        {
                            result.pending = true;
                        }
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                result = null;
                logger.Debug(e.Message);
            }

            return result;
        }

        public bool InsertExecutionPolicy(DocsPaVO.Conservazione.ExecutionPolicy execution)
        {
            bool result = true;
            try
            {

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    Query query = InitQuery.getInstance().getQuery("I_DPA_POLICY_ESECUZIONE");
                    query.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    query.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_POLICY_ESECUZIONE"));
                    query.setParam("idPolicy", execution.idPolicy);
                    query.setParam("startExecuteDate", DocsPaDbManagement.Functions.Functions.ToDate(execution.startExecutionDate));
                    query.setParam("endExecuteDate", DocsPaDbManagement.Functions.Functions.ToDate(execution.endExecutionDate));
                    query.setParam("nDoc", execution.docNumberProcessed);
                    query.setParam("idLastDocument", execution.idLastDocumentProcessed);
                    query.setParam("idFirstDocument", execution.idFirstDocumentProcessed);
                    query.setParam("idIstanza", execution.idIstanza);
                    query.setParam("idAmm", execution.idAmm);

                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - InsertExecutionPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    logger.Debug("SQL - InsertExecutionPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);

                    dbProvider.CommitTransaction();
                }


            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }

            return result;
        }

        public bool InsertDettExecutionPolicy(string idPolicy, string idProfile, string idProject, string esito, string errore)
        {
            bool result = true;
            try
            {

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    Query query = InitQuery.getInstance().getQuery("I_ITEMS_EXECUTIONS_POLICY");
                    query.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    query.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ITEMS_CONSERVAZIONE"));
                    query.setParam("idPolicy", idPolicy);
                    query.setParam("idProfile", idProfile);
                    if (!string.IsNullOrEmpty(idProject))
                    {
                        query.setParam("idProject", idProject);
                    }
                    else
                    {
                        query.setParam("idProject", "NULL");
                    }
                    query.setParam("esito", esito);
                    query.setParam("errore", errore);

                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - InsertNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    logger.Debug("SQL - InsertNewPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);

                    dbProvider.CommitTransaction();
                }


            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }

            return result;
        }

    }
}
