using System;
using System.Collections;
using System.Data;
using System.Threading;
using log4net;
using DocsPaVO.filtri;
using DocsPaVO.filtri.trasmissione;
using System.Text;
using System.Collections.Generic;
using DocsPaVO.Modelli_Trasmissioni;
using System.Linq;
using DocsPaVO.utente;

namespace DocsPaDB.Query_DocsPAWS
{
	
	public class ModTrasmissioni: DBProvider
	{
        private ILog logger = LogManager.GetLogger(typeof(ModTrasmissioni));
		private Mutex semaforo = new Mutex();

		public ModTrasmissioni(){}

		//OK
		public ArrayList getModelliPerTrasm(string idAmm,DocsPaVO.utente.Registro[] registri,string idPeople,string idCorrGlobali,string idTipoDoc,string idDiagramma,string idStato,string cha_tipo_oggetto, bool AllReg)
		{
			ArrayList idModelli = new ArrayList();
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

            string paramRegistri = string.Empty;
            if (registri.Length != 0)
            {
                paramRegistri = " AND mod.id_registro in (";
                for (int i = 0; i < registri.Length; i++)
                {
                    paramRegistri += ((DocsPaVO.utente.Registro)registri[i]).systemId;
                    paramRegistri += ",";
                }
                if (AllReg)
                {
                    paramRegistri += "0,";
                }
                
                paramRegistri = paramRegistri.Substring(0, paramRegistri.Length - 1) + ")";
            }

			try
			{
				//PRIMA QUERY = Prende i modelli con single = 1 sul registro e non associati
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI_PER_TRASM");		
				queryMng.setParam("idAmm",idAmm);
				if(paramRegistri != string.Empty)
                    queryMng.setParam("paramRegistri", paramRegistri);
				queryMng.setParam("cha_tipo_oggetto",cha_tipo_oggetto);
				string commandText=queryMng.getSQL();				
                logger.Debug("SQL - getModelliPerTrasm - ModTrasmissioni.cs - QUERY : " + commandText);

				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				if(ds.Tables[0].Rows.Count != 0)
				{
					for(int i=0; i<ds.Tables[0].Rows.Count; i++)
					{
						DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
						mod.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
						mod.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
                        mod.CODICE = "MT_" + mod.SYSTEM_ID;//ds.Tables[0].Rows[i]["CODICE"].ToString();
                        mod.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                        mod.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                        mod.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                        mod.NO_NOTIFY = ds.Tables[0].Rows[i]["NO_NOTIFY"].ToString();
                		idModelli.Add(mod);
					}
				}

				//SECONDA QUERY = Prende i modelli con single = 0 con mittente uguale a utente loggato e non associati
				queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI_PER_TRASM_1");		
				queryMng.setParam("idPeople",idPeople);
				queryMng.setParam("idCorrGlobali",idCorrGlobali);
				queryMng.setParam("idAmm",idAmm);
				if (paramRegistri != string.Empty)
                    queryMng.setParam("paramRegistri", paramRegistri);
				queryMng.setParam("cha_tipo_oggetto",cha_tipo_oggetto);
				commandText=queryMng.getSQL();				
                logger.Debug("SQL - getModelliPerTrasm - ModTrasmissioni.cs - QUERY : " + commandText);

				ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				if(ds.Tables[0].Rows.Count != 0)
				{
					for(int i=0; i<ds.Tables[0].Rows.Count; i++)
					{
						DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
						mod.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
						mod.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
                        mod.CODICE = "MT_" + mod.SYSTEM_ID;//ds.Tables[0].Rows[i]["CODICE"].ToString();

                        mod.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                        mod.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                        mod.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                        mod.NO_NOTIFY = ds.Tables[0].Rows[i]["NO_NOTIFY"].ToString();

                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MITTENTE");
                        queryMng.setParam("param1", ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                        commandText = queryMng.getSQL();
                        logger.Debug("SQL - getModelliPerTrasm - ModTrasmissioni.cs - QUERY : " + commandText);

                        DataSet ds1 = new DataSet();
                        dbProvider.ExecuteQuery(ds1, commandText);

                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            mod.MITTENTE = new ArrayList();
                            for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                            {
                                DocsPaVO.Modelli_Trasmissioni.MittDest mittente = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                                mittente.SYSTEM_ID = Convert.ToInt32(ds1.Tables[0].Rows[k]["SYSTEM_ID"].ToString());
                                mittente.ID_MODELLO = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_MODELLO"].ToString());
                                mittente.CHA_TIPO_MITT_DEST = ds1.Tables[0].Rows[k]["CHA_TIPO_MITT_DEST"].ToString();
                                mittente.VAR_COD_RUBRICA = ds1.Tables[0].Rows[k]["VAR_COD_RUBRICA"].ToString();
                                mittente.ID_RAGIONE = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_RAGIONE"].ToString());
                                mittente.CHA_TIPO_TRASM = ds1.Tables[0].Rows[k]["CHA_TIPO_TRASM"].ToString();
                                mittente.VAR_NOTE_SING = ds1.Tables[0].Rows[k]["VAR_NOTE_SING"].ToString();
                                mittente.DESCRIZIONE = ds1.Tables[0].Rows[k]["VAR_DESC_CORR"].ToString();
                                mittente.CHA_TIPO_URP = ds1.Tables[0].Rows[k]["CHA_TIPO_URP"].ToString();
                                mittente.ID_CORR_GLOBALI = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_CORR_GLOBALI"].ToString());
                                mittente.NASCONDI_VERSIONI_PRECEDENTI = (ds1.Tables[0].Rows[k]["HIDE_DOC_VERSIONS"].ToString() == "1");
                                mod.MITTENTE.Add(mittente);
                            }
                        }


						idModelli.Add(mod);
					}
				}

				if(!string.IsNullOrEmpty(idTipoDoc))
				{
					//TERZA QUERY = Prende i modelli con single = 1 sul registro e associati
					queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI_PER_TRASM_2");		
					queryMng.setParam("idAmm",idAmm);
				    if (paramRegistri != string.Empty)
                        queryMng.setParam("paramRegistri", paramRegistri);
					queryMng.setParam("cha_tipo_oggetto",cha_tipo_oggetto);
					queryMng.setParam("idTipoDoc",idTipoDoc);
					string query = "";
					if(idDiagramma != "")
					{
						query +=	" and DPA_ASS_DIAGRAMMI.id_diagramma="+idDiagramma;
					}
					if(idStato != "")
					{
                        query += " and DPA_ASS_DIAGRAMMI.id_stato=" + idStato;
					}
					
					query += ")";
					queryMng.setParam("condizione",query);						
					
					commandText=queryMng.getSQL();					
                    logger.Debug("SQL - getModelliPerTrasm - ModTrasmissioni.cs - QUERY : " + commandText);

					ds = new DataSet();
					dbProvider.ExecuteQuery(ds,commandText);	
					if(ds.Tables[0].Rows.Count != 0)
					{
						for(int i=0; i<ds.Tables[0].Rows.Count; i++)
						{
							DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
							mod.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
							mod.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
                            mod.CODICE = "MT_" + mod.SYSTEM_ID;//ds.Tables[0].Rows[i]["CODICE"].ToString();

                            mod.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                            mod.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                            mod.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                            mod.NO_NOTIFY = ds.Tables[0].Rows[i]["NO_NOTIFY"].ToString();
                            idModelli.Add(mod);
						}
					}
				}

				//QUARTA QUERY = Prende i modelli con single = 0 con mittente uguale a utente loggato e associati
                if (!string.IsNullOrEmpty(idTipoDoc))
				{
					queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI_PER_TRASM_3");		
					queryMng.setParam("idPeople",idPeople);
					queryMng.setParam("idCorrGlobali",idCorrGlobali);
					queryMng.setParam("idAmm",idAmm);
					if (paramRegistri != string.Empty)
                        queryMng.setParam("paramRegistri", paramRegistri);
					queryMng.setParam("cha_tipo_oggetto",cha_tipo_oggetto);
					queryMng.setParam("idTipoDoc",idTipoDoc);
					string query = "";
					if(idDiagramma != "")
					{
						query +=	" and DPA_ASS_DIAGRAMMI.id_diagramma="+idDiagramma;
					}
					if(idStato != "")
					{
                        query += " and DPA_ASS_DIAGRAMMI.id_stato=" + idStato;
					}
					
					query += ")";
					queryMng.setParam("condizione",query);						
					
					commandText=queryMng.getSQL();					
                    logger.Debug("SQL - getModelliPerTrasm - ModTrasmissioni.cs - QUERY : " + commandText);

					ds = new DataSet();
					dbProvider.ExecuteQuery(ds,commandText);	
					if(ds.Tables[0].Rows.Count != 0)
					{
						for(int i=0; i<ds.Tables[0].Rows.Count; i++)
						{
							DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
							mod.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
							mod.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
                            mod.CODICE = "MT_" + mod.SYSTEM_ID;
                            mod.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                            mod.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                            mod.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                            mod.NO_NOTIFY = ds.Tables[0].Rows[i]["NO_NOTIFY"].ToString();

                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MITTENTE");
                            queryMng.setParam("param1", ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                            commandText = queryMng.getSQL();
                            logger.Debug("SQL - getModelliPerTrasm - ModTrasmissioni.cs - QUERY : " + commandText);

                            DataSet ds1 = new DataSet();
                            dbProvider.ExecuteQuery(ds1, commandText);

                            if (ds1.Tables[0].Rows.Count > 0)
                            {
                                mod.MITTENTE = new ArrayList();
                                for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                                {
                                    DocsPaVO.Modelli_Trasmissioni.MittDest mittente = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                                    mittente.SYSTEM_ID = Convert.ToInt32(ds1.Tables[0].Rows[k]["SYSTEM_ID"].ToString());
                                    mittente.ID_MODELLO = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_MODELLO"].ToString());
                                    mittente.CHA_TIPO_MITT_DEST = ds1.Tables[0].Rows[k]["CHA_TIPO_MITT_DEST"].ToString();
                                    mittente.VAR_COD_RUBRICA = ds1.Tables[0].Rows[k]["VAR_COD_RUBRICA"].ToString();
                                    mittente.ID_RAGIONE = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_RAGIONE"].ToString());
                                    mittente.CHA_TIPO_TRASM = ds1.Tables[0].Rows[k]["CHA_TIPO_TRASM"].ToString();
                                    mittente.VAR_NOTE_SING = ds1.Tables[0].Rows[k]["VAR_NOTE_SING"].ToString();
                                    mittente.DESCRIZIONE = ds1.Tables[0].Rows[k]["VAR_DESC_CORR"].ToString();
                                    mittente.CHA_TIPO_URP = ds1.Tables[0].Rows[k]["CHA_TIPO_URP"].ToString();
                                    mittente.ID_CORR_GLOBALI = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_CORR_GLOBALI"].ToString());
                                    mittente.NASCONDI_VERSIONI_PRECEDENTI = (ds1.Tables[0].Rows[k]["HIDE_DOC_VERSIONS"].ToString() == "1");
                                    mod.MITTENTE.Add(mittente);
                                }
                            }


                            idModelli.Add(mod);
						}
					}
				}
			}
			catch
			{
                return idModelli;
			}
			finally
			{
				dbProvider.Dispose();				
			}
			return idModelli;
		}


        public ArrayList getModelliPerTrasm(string idAmm, DocsPaVO.utente.Registro[] registri, string idPeople, string idCorrGlobali, string idTipoDoc, string idDiagramma, string idStato, string cha_tipo_oggetto, string systemId, string idRuoloUtente, bool AllReg, string accessrights)
        {
            ArrayList idModelli = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            string paramRegistri = string.Empty;
            DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();
            if (registri.Length != 0 && !obj.isFiltroAooEnabled())
            {
                paramRegistri = " AND mod.id_registro in (";
                for (int i = 0; i < registri.Length; i++)
                {
                    paramRegistri += ((DocsPaVO.utente.Registro)registri[i]).systemId;
                    paramRegistri += ",";
                }
                if (AllReg)
                {
                    paramRegistri += "0,";
                }

                paramRegistri = paramRegistri.Substring(0, paramRegistri.Length - 1) + ")";
            }

            try
            {
                //PRIMA QUERY = Prende i modelli con single = 1 sul registro e non associati
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI_PER_TRASM_AUTORIZZATI");
                queryMng.setParam("idAmm", idAmm);
                if (paramRegistri != string.Empty)
                    queryMng.setParam("paramRegistri", paramRegistri);
                queryMng.setParam("cha_tipo_oggetto", cha_tipo_oggetto);

                string paramautorizzato = string.Empty;
                if (!string.IsNullOrEmpty(systemId))
                {
                    if (dbType.ToUpper().Equals("SQL"))
                    {
                        string userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession();
                        paramautorizzato += " and " + userDb + ".getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1";
                        //queryMng.setParam("paramautorizzato", " and " + userDb + ".getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1");
                    }
                    else
                    {
                        paramautorizzato += " and getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1";
                        //queryMng.setParam("paramautorizzato", " and getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1");
                    }
                }
                else
                {
                    if (cha_tipo_oggetto == "D")
                    {
                        paramautorizzato += " and (select count(system_id) from dpa_modelli_mitt_dest where id_modello = mod.system_id and hide_doc_versions = '1') = 0";
                        //queryMng.setParam("paramautorizzato", " and (select count(system_id) from dpa_modelli_mitt_dest where id_modello = mod.system_id and hide_doc_versions = '1') = 0");
                    }
                }
                //Condizione per eliminare i modelli che hanno fra i loro destinatari almeno un ruolo disabilitato alla ricezione delle trasmissioni
                paramautorizzato += getRoleDisabledTrasmCondition("mod", false);
                queryMng.setParam("paramautorizzato", paramautorizzato);

                string commandText = queryMng.getSQL();
                logger.Debug("SQL - getModelliPerTrasm - ModTrasmissioni.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
                        mod.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                        mod.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
                        mod.CODICE = "MT_" + mod.SYSTEM_ID;//ds.Tables[0].Rows[i]["CODICE"].ToString();
                        mod.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                        mod.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                        mod.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                        mod.NO_NOTIFY = ds.Tables[0].Rows[i]["NO_NOTIFY"].ToString();
                        mod.MANTIENI_LETTURA = ds.Tables[0].Rows[i]["CHA_MANTIENI_LETTURA"].ToString();

                        //
                        // Mev Cessione Diritti - Mantieni Scrittura
                        if(ds.Tables[0].Columns.Contains("CHA_MANTIENI_SCRITTURA"))
                        mod.MANTIENI_SCRITTURA = ds.Tables[0].Rows[i]["CHA_MANTIENI_SCRITTURA"].ToString();
                        // End Mev
                        //
                        idModelli.Add(mod);
                    }
                }

                //SECONDA QUERY = Prende i modelli con single = 0 con mittente uguale a utente loggato e non associati
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI_PER_TRASM_AUTORIZZATI_1");
                queryMng.setParam("idPeople", idPeople);
                queryMng.setParam("idCorrGlobali", idCorrGlobali);
                queryMng.setParam("idAmm", idAmm);
                if (paramRegistri != string.Empty)
                    queryMng.setParam("paramRegistri", paramRegistri);
                queryMng.setParam("cha_tipo_oggetto", cha_tipo_oggetto);
                queryMng.setParam("systemId", systemId);

                paramautorizzato = string.Empty;
                if (!string.IsNullOrEmpty(systemId))
                {
                    if (dbType.ToUpper().Equals("SQL"))
                    {
                        string userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession();
                        paramautorizzato += " and " + userDb + ".getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1";
                        //queryMng.setParam("paramautorizzato", " and " + userDb + ".getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id,"+accessrights+")=1");
                    }
                    else
                    {
                        paramautorizzato += " and getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1";
                        //queryMng.setParam("paramautorizzato", " and getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1");
                    }
                }
                else
                {
                    if (cha_tipo_oggetto == "D")
                    {
                        paramautorizzato += " and (select count(system_id) from dpa_modelli_mitt_dest where id_modello = mod.system_id and hide_doc_versions = '1') = 0";
                        //queryMng.setParam("paramautorizzato", " and (select count(system_id) from dpa_modelli_mitt_dest where id_modello = mod.system_id and hide_doc_versions = '1') = 0");
                    }
                }
                //Condizione per eliminare i modelli che hanno fra i loro destinatari almeno un ruolo disabilitato alla ricezione delle trasmissioni
                paramautorizzato += getRoleDisabledTrasmCondition("mod", false);
                queryMng.setParam("paramautorizzato", paramautorizzato);

                commandText = queryMng.getSQL();
                logger.Debug("SQL - getModelliPerTrasm - ModTrasmissioni.cs - QUERY : " + commandText);

                ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
                        mod.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                        mod.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
                        mod.CODICE = "MT_" + mod.SYSTEM_ID;
                        mod.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                        mod.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                        mod.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                        mod.NO_NOTIFY = ds.Tables[0].Rows[i]["NO_NOTIFY"].ToString();

                        mod.MANTIENI_LETTURA = ds.Tables[0].Rows[i]["CHA_MANTIENI_LETTURA"].ToString();

                        //
                        // Mev Cessione Diritti - Mantieni Scrittura
                        if (ds.Tables[0].Columns.Contains("CHA_MANTIENI_SCRITTURA"))
                        mod.MANTIENI_SCRITTURA = ds.Tables[0].Rows[i]["CHA_MANTIENI_SCRITTURA"].ToString();
                        // End Mev
                        //

                        
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MITTENTE");
                        queryMng.setParam("param1", ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                        commandText = queryMng.getSQL();
                        logger.Debug("SQL - getModelliPerTrasm - ModTrasmissioni.cs - QUERY : " + commandText);

                        DataSet ds1 = new DataSet();
                        dbProvider.ExecuteQuery(ds1, commandText);

                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            mod.MITTENTE = new ArrayList();
                            for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                            {
                                DocsPaVO.Modelli_Trasmissioni.MittDest mittente = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                                mittente.SYSTEM_ID = Convert.ToInt32(ds1.Tables[0].Rows[k]["SYSTEM_ID"].ToString());
                                mittente.ID_MODELLO = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_MODELLO"].ToString());
                                mittente.CHA_TIPO_MITT_DEST = ds1.Tables[0].Rows[k]["CHA_TIPO_MITT_DEST"].ToString();
                                mittente.VAR_COD_RUBRICA = ds1.Tables[0].Rows[k]["VAR_COD_RUBRICA"].ToString();
                                mittente.ID_RAGIONE = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_RAGIONE"].ToString());
                                mittente.CHA_TIPO_TRASM = ds1.Tables[0].Rows[k]["CHA_TIPO_TRASM"].ToString();
                                mittente.VAR_NOTE_SING = ds1.Tables[0].Rows[k]["VAR_NOTE_SING"].ToString();
                                mittente.DESCRIZIONE = ds1.Tables[0].Rows[k]["VAR_DESC_CORR"].ToString();
                                mittente.CHA_TIPO_URP = ds1.Tables[0].Rows[k]["CHA_TIPO_URP"].ToString();
                                mittente.ID_CORR_GLOBALI = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_CORR_GLOBALI"].ToString());
                                mittente.NASCONDI_VERSIONI_PRECEDENTI = (ds1.Tables[0].Rows[k]["HIDE_DOC_VERSIONS"].ToString() == "1");
                                mod.MITTENTE.Add(mittente);
                            }
                        }


                        idModelli.Add(mod);
                    }
                }

                if (!string.IsNullOrEmpty(idTipoDoc))
                {
                    //TERZA QUERY = Prende i modelli con single = 1 sul registro e associati
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI_PER_TRASM_AUTORIZZATI_2");
                    queryMng.setParam("idAmm", idAmm);
                    if (paramRegistri != string.Empty)
                        queryMng.setParam("paramRegistri", paramRegistri);
                    queryMng.setParam("cha_tipo_oggetto", cha_tipo_oggetto);
                    queryMng.setParam("idTipoDoc", idTipoDoc);
                    queryMng.setParam("idRuolo", idCorrGlobali);
                    queryMng.setParam("idPeople", idPeople);
                    queryMng.setParam("systemId", systemId);

                    paramautorizzato = string.Empty;
                    if (!string.IsNullOrEmpty(systemId))
                    {
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            string userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession();
                            paramautorizzato += " and " + userDb + ".getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1";
                            //queryMng.setParam("paramautorizzato", " and " + userDb + ".getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id,"+accessrights+")=1");
                        }
                        else
                        {
                            paramautorizzato += " and getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1";
                            //queryMng.setParam("paramautorizzato", " and getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1");
                        }
                    }
                    else
                    {
                        if (cha_tipo_oggetto == "D")
                        {
                            paramautorizzato += " and (select count(system_id) from dpa_modelli_mitt_dest where id_modello = mod.system_id and hide_doc_versions = '1') = 0";
                            //queryMng.setParam("paramautorizzato", " and (select count(system_id) from dpa_modelli_mitt_dest where id_modello = mod.system_id and hide_doc_versions = '1') = 0");
                        }
                    }
                    //Condizione per eliminare i modelli che hanno fra i loro destinatari almeno un ruolo disabilitato alla ricezione delle trasmissioni
                    paramautorizzato += getRoleDisabledTrasmCondition("mod", false);
                    queryMng.setParam("paramautorizzato", paramautorizzato);
                    
                    string query = "";
                    if (idDiagramma != "")
                    {
                        query += " and DPA_ASS_DIAGRAMMI.id_diagramma=" + idDiagramma;
                    }
                    if (idStato != "")
                    {
                        query += " and DPA_ASS_DIAGRAMMI.id_stato=" + idStato;
                    }

                    query += ")";
                    queryMng.setParam("condizione", query);

                    commandText = queryMng.getSQL();
                    logger.Debug("SQL - getModelliPerTrasm - ModTrasmissioni.cs - QUERY : " + commandText);

                    ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
                            mod.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                            mod.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
                            mod.CODICE = "MT_" + mod.SYSTEM_ID;//ds.Tables[0].Rows[i]["CODICE"].ToString();

                            mod.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                            mod.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                            mod.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                            mod.NO_NOTIFY = ds.Tables[0].Rows[i]["NO_NOTIFY"].ToString();
                            mod.MANTIENI_LETTURA = ds.Tables[0].Rows[i]["ID_CORR_GLOBALI"].ToString();

                            //
                            // Mev Cessione Diritti - Mantieni Scrittura
                            // Non è chiaro come prende il mantieni lettura
                            if (ds.Tables[0].Columns.Contains("CHA_MANTIENI_SCRITTURA"))
                            mod.MANTIENI_SCRITTURA = ds.Tables[0].Rows[i]["CHA_MANTIENI_SCRITTURA"].ToString();
                            // End Mev
                            //
                            idModelli.Add(mod);
                        }
                    }
                }

                //QUARTA QUERY = Prende i modelli con single = 0 con mittente uguale a utente loggato e associati
                if (!string.IsNullOrEmpty(idTipoDoc))
                {
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI_PER_TRASM_AUTORIZZATI_3");
                    queryMng.setParam("idPeople", idPeople);
                    queryMng.setParam("idCorrGlobali", idCorrGlobali);
                    queryMng.setParam("idAmm", idAmm);
                    if (paramRegistri != string.Empty)
                        queryMng.setParam("paramRegistri", paramRegistri);
                    queryMng.setParam("cha_tipo_oggetto", cha_tipo_oggetto);
                    queryMng.setParam("idTipoDoc", idTipoDoc);
                    queryMng.setParam("systemId", systemId);

                    paramautorizzato = string.Empty;
                    if (!string.IsNullOrEmpty(systemId))
                    {
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            string userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession();
                            paramautorizzato += " and " + userDb + ".getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1";
                            //queryMng.setParam("paramautorizzato", " and " + userDb + ".getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id,"+accessrights+")=1");
                        }
                        else
                        {
                            paramautorizzato += " and getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1";
                            //queryMng.setParam("paramautorizzato", " and getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1");
                        }
                    }
                    else
                    {
                        if (cha_tipo_oggetto == "D")
                        {
                            paramautorizzato += " and (select count(system_id) from dpa_modelli_mitt_dest where id_modello = mod.system_id and hide_doc_versions = '1') = 0";
                            //queryMng.setParam("paramautorizzato", " and (select count(system_id) from dpa_modelli_mitt_dest where id_modello = mod.system_id and hide_doc_versions = '1') = 0");
                        }
                    }
                    //Condizione per eliminare i modelli che hanno fra i loro destinatari almeno un ruolo disabilitato alla ricezione delle trasmissioni
                    paramautorizzato += getRoleDisabledTrasmCondition("mod", false);
                    queryMng.setParam("paramautorizzato", paramautorizzato);

                    string query = "";
                    if (idDiagramma != "")
                    {
                        query += " and DPA_ASS_DIAGRAMMI.id_diagramma=" + idDiagramma;
                    }
                    if (idStato != "")
                    {
                        query += " and DPA_ASS_DIAGRAMMI.id_stato=" + idStato;
                    }

                    query += ")";
                    queryMng.setParam("condizione", query);

                    commandText = queryMng.getSQL();
                    logger.Debug("SQL - getModelliPerTrasm - ModTrasmissioni.cs - QUERY : " + commandText);

                    ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
                            mod.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                            mod.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
                            mod.CODICE = "MT_" + mod.SYSTEM_ID;//ds.Tables[0].Rows[i]["CODICE"].ToString();

                            mod.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                            mod.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                            mod.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                            mod.NO_NOTIFY = ds.Tables[0].Rows[i]["NO_NOTIFY"].ToString();
                            mod.MANTIENI_LETTURA = ds.Tables[0].Rows[i]["ID_CORR_GLOBALI"].ToString();
                            
                            //
                            // Mev Cessine Diritti - Mantieni Scrittura
                            if (ds.Tables[0].Columns.Contains("CHA_MANTIENI_SCRITTURA"))
                            mod.MANTIENI_SCRITTURA = ds.Tables[0].Rows[i]["CHA_MANTIENI_SCRITTURA"].ToString();
                            // End Mev
                            //

                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MITTENTE");
                            queryMng.setParam("param1", ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                            commandText = queryMng.getSQL();
                            logger.Debug("SQL - getModelliPerTrasm - ModTrasmissioni.cs - QUERY : " + commandText);

                            DataSet ds1 = new DataSet();
                            dbProvider.ExecuteQuery(ds1, commandText);

                            if (ds1.Tables[0].Rows.Count > 0)
                            {
                                mod.MITTENTE = new ArrayList();
                                for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                                {
                                    DocsPaVO.Modelli_Trasmissioni.MittDest mittente = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                                    mittente.SYSTEM_ID = Convert.ToInt32(ds1.Tables[0].Rows[k]["SYSTEM_ID"].ToString());
                                    mittente.ID_MODELLO = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_MODELLO"].ToString());
                                    mittente.CHA_TIPO_MITT_DEST = ds1.Tables[0].Rows[k]["CHA_TIPO_MITT_DEST"].ToString();
                                    mittente.VAR_COD_RUBRICA = ds1.Tables[0].Rows[k]["VAR_COD_RUBRICA"].ToString();
                                    mittente.ID_RAGIONE = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_RAGIONE"].ToString());
                                    mittente.CHA_TIPO_TRASM = ds1.Tables[0].Rows[k]["CHA_TIPO_TRASM"].ToString();
                                    mittente.VAR_NOTE_SING = ds1.Tables[0].Rows[k]["VAR_NOTE_SING"].ToString();
                                    mittente.DESCRIZIONE = ds1.Tables[0].Rows[k]["VAR_DESC_CORR"].ToString();
                                    mittente.CHA_TIPO_URP = ds1.Tables[0].Rows[k]["CHA_TIPO_URP"].ToString();
                                    mittente.ID_CORR_GLOBALI = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_CORR_GLOBALI"].ToString());
                                    mittente.NASCONDI_VERSIONI_PRECEDENTI = (ds1.Tables[0].Rows[k]["HIDE_DOC_VERSIONS"].ToString() == "1");
                                    mod.MITTENTE.Add(mittente);
                                }
                            }


                            idModelli.Add(mod);
                        }
                    }
                }
            }
            catch
            {
                return idModelli;
            }
            finally
            {
                dbProvider.Dispose();                
            }
            return idModelli;
        }


        public bool isUniqueCodModelloTrasm(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_IS_UNIQUE_COD_MODELLO_TRASM");
                string parametri = "ID_AMM = " + modello.ID_AMM + " AND ID_REGISTRO = " + modello.ID_REGISTRO;
                if (modello.ID_PEOPLE != null && !modello.ID_PEOPLE.Equals(""))
                    parametri += " AND ID_PEOPLE = " + modello.ID_PEOPLE;
                parametri += " AND CHA_TIPO_OGGETTO = '" + modello.CHA_TIPO_OGGETTO + "'";
                parametri += " AND SINGLE = " + modello.SINGLE;
                parametri += " AND CODICE = '" + modello.CODICE + "'";
                queryMng.setParam("parametri", parametri);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - isUniqueCodModelloTrasm - ModTrasmissioni.cs - QUERY : " + commandText);
                logger.Debug("SQL - isUniqueCodModelloTrasm - ModTrasmissioni.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count != 0)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                dbProvider.Dispose();
                //semaforo.ReleaseMutex();
            }
        }

		//OK
		public string salvaModello(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modelloTrasmissione)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
            string dbID_MittDest = string.Empty;
            bool notificheUtImpostate = false;
            string result = string.Empty;

			try
			{
				//semaforo.WaitOne();
				if(Convert.ToString(modelloTrasmissione.SYSTEM_ID)=="0")
				{
					bool retValue=false;
					int rowsAffected;
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_SALVA_MODELLO");

                    if (!dbType.ToUpper().Equals("SQL"))
                    {
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MODELLI_TRASM"));
                    }

					queryMng.setParam("param0",modelloTrasmissione.ID_AMM);
					queryMng.setParam("param1",modelloTrasmissione.NOME.Replace("'","''"));
					queryMng.setParam("param2",modelloTrasmissione.CHA_TIPO_OGGETTO);
                    if (modelloTrasmissione.ID_REGISTRO == "") {
                        queryMng.setParam("param3", "0");
                    }
                    else
    					queryMng.setParam("param3",modelloTrasmissione.ID_REGISTRO);
					if(modelloTrasmissione.VAR_NOTE_GENERALI!=null && modelloTrasmissione.VAR_NOTE_GENERALI!="")
						queryMng.setParam("param4",modelloTrasmissione.VAR_NOTE_GENERALI.Replace("'","''"));
					queryMng.setParam("param5",modelloTrasmissione.SINGLE);                    
					
					if(modelloTrasmissione.ID_PEOPLE == null || modelloTrasmissione.ID_PEOPLE == "")
                        queryMng.setParam("param6","null");
					else
						queryMng.setParam("param6",modelloTrasmissione.ID_PEOPLE);

                    if (modelloTrasmissione.CEDE_DIRITTI != null)
                        queryMng.setParam("cedeDir", modelloTrasmissione.CEDE_DIRITTI);
                    else
                        queryMng.setParam("cedeDir", "0");                        
                    
                    if(modelloTrasmissione.ID_PEOPLE_NEW_OWNER == null || modelloTrasmissione.ID_PEOPLE_NEW_OWNER == "")
                        queryMng.setParam("idpeople_new_owner", "null");
                    else                        
                        queryMng.setParam("idpeople_new_owner", modelloTrasmissione.ID_PEOPLE_NEW_OWNER);

                    if (modelloTrasmissione.ID_GROUP_NEW_OWNER == null || modelloTrasmissione.ID_GROUP_NEW_OWNER == "")
                        queryMng.setParam("idgroup_new_owner", "null");
                    else
                        queryMng.setParam("idgroup_new_owner", modelloTrasmissione.ID_GROUP_NEW_OWNER);

                    queryMng.setParam("no_notify", modelloTrasmissione.NO_NOTIFY);
                    
                    if (!string.IsNullOrEmpty(modelloTrasmissione.MANTIENI_LETTURA))
                        queryMng.setParam("mantieniLettura", modelloTrasmissione.MANTIENI_LETTURA);
                    else
                        queryMng.setParam("mantieniLettura", "0");

                    //
                    // Mev Cessione Diritti - Mantieni Scrittura
                    if (!string.IsNullOrEmpty(modelloTrasmissione.MANTIENI_SCRITTURA))
                        queryMng.setParam("mantieniScrittura", modelloTrasmissione.MANTIENI_SCRITTURA);
                    else
                        queryMng.setParam("mantieniScrittura", "0");
                    // End Mev Cessione Diritti - Mantieni Scrittura
                    //

                    //////////queryMng.setParam("hideDocVersions", (modelloTrasmissione.NASCONDI_VERSIONI_PRECEDENTI ? "'1'" : "NULL"));

					string commandText=queryMng.getSQL();					
                    logger.Debug("SQL - salvaModello - ModTrasmissioni.cs - QUERY : " + commandText);

					string idModello = string.Empty;
					if (dbProvider.ExecuteNonQuery(commandText,out rowsAffected))
					{
						retValue=(rowsAffected>0);
								
                        if (retValue)
						{
                            // Reperimento systemid
                            commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_MODELLI_TRASM");
                            dbProvider.ExecuteScalar(out idModello, commandText);
                            result = "I^" + idModello;
                            modelloTrasmissione.SYSTEM_ID = Convert.ToInt32(idModello);
						}
					}

					//inserisco il mittente solo se non è tutta la AOO
					//single="0" se RUOLO
					//single="1" se tutta AOO
                    //if (!dbType.ToUpper().Equals("SQL"))
                        this.insertSalvaModello(modelloTrasmissione);
				}
				else
				{
					bool retValue=false;
					int rowsAffected;
					DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_UPDATE_MODELLO");		
					queryMng.setParam("param0",modelloTrasmissione.ID_AMM);
					queryMng.setParam("param1",modelloTrasmissione.NOME.Replace("'","''"));
					queryMng.setParam("param2",modelloTrasmissione.CHA_TIPO_OGGETTO);
                    queryMng.setParam("no_notify", modelloTrasmissione.NO_NOTIFY);
                    if (modelloTrasmissione.ID_REGISTRO != null && modelloTrasmissione.ID_REGISTRO != String.Empty)
                    {
                        queryMng.setParam("param3", modelloTrasmissione.ID_REGISTRO);
                    }
                    else {
                        queryMng.setParam("param3", "0");
                    }
					
					if(modelloTrasmissione.VAR_NOTE_GENERALI!=null && modelloTrasmissione.VAR_NOTE_GENERALI!="")
						queryMng.setParam("param4",modelloTrasmissione.VAR_NOTE_GENERALI.Replace("'","''"));
					
					queryMng.setParam("param5",modelloTrasmissione.SINGLE);
					queryMng.setParam("param6",modelloTrasmissione.SYSTEM_ID.ToString());
					
					if(modelloTrasmissione.ID_PEOPLE == null || modelloTrasmissione.ID_PEOPLE == "")
						queryMng.setParam("param7","null");
					else
						queryMng.setParam("param7",modelloTrasmissione.ID_PEOPLE);

                    // cessione diritti
                    if (modelloTrasmissione.CEDE_DIRITTI != null && modelloTrasmissione.CEDE_DIRITTI != "")
                        queryMng.setParam("cedeDir", modelloTrasmissione.CEDE_DIRITTI);
                    else
                        queryMng.setParam("cedeDir", "");

                    if (modelloTrasmissione.ID_PEOPLE_NEW_OWNER != null && modelloTrasmissione.ID_PEOPLE_NEW_OWNER != "")
                        queryMng.setParam("idpeople_new_owner", modelloTrasmissione.ID_PEOPLE_NEW_OWNER);
                    else
                        queryMng.setParam("idpeople_new_owner", "null");

                    if (modelloTrasmissione.ID_GROUP_NEW_OWNER != null && modelloTrasmissione.ID_GROUP_NEW_OWNER != "")
                        queryMng.setParam("idgroup_new_owner", modelloTrasmissione.ID_GROUP_NEW_OWNER);
                    else
                        queryMng.setParam("idgroup_new_owner", "null");

                    if (!string.IsNullOrEmpty(modelloTrasmissione.MANTIENI_LETTURA))
                        queryMng.setParam("mantieniLettura", modelloTrasmissione.MANTIENI_LETTURA);
                    else
                        queryMng.setParam("mantieniLettura", "0");

                    //
                    // MEV Cessione Diritti - Mantieni Scrittura
                    if (!string.IsNullOrEmpty(modelloTrasmissione.MANTIENI_SCRITTURA))
                        queryMng.setParam("mantieniScrittura", modelloTrasmissione.MANTIENI_SCRITTURA);
                    else
                        queryMng.setParam("mantieniScrittura", "0");
                    // End MEV Cessione Diritti - Mantieni Scrittura
                    //

                    //////////queryMng.setParam("hideDocVersions", (modelloTrasmissione.NASCONDI_VERSIONI_PRECEDENTI ? "'1'" : "NULL"));

					string commandText=queryMng.getSQL();
					logger.Debug("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/model.cs - QUERY : "+commandText);

					dbProvider.ExecuteNonQuery(commandText,out rowsAffected);
					retValue=(rowsAffected>0);

                    if (retValue)
                    {
                        result = "U^" + modelloTrasmissione.SYSTEM_ID.ToString();

                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_DELETE_DPA_MODELLI_MITT_DEST");
                        queryMng.setParam("idModTrasmissione", Convert.ToString(modelloTrasmissione.SYSTEM_ID));
                        commandText = queryMng.getSQL();
                        logger.Debug("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                        retValue = (rowsAffected > 0);
                        rowsAffected = 0;

                        if (retValue)
                        {
                            // elimino anche tutti i record della DPA_MITT_DEST_CON_NOTIFICA
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_DEL_UT_CON_NOTIFICA");
                            queryMng.setParam("id", "ID_MODELLO = " + Convert.ToString(modelloTrasmissione.SYSTEM_ID));
                            commandText = queryMng.getSQL();
                            logger.Debug("UtentiConNotificaTrasm QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                        retValue = (rowsAffected > 0);
                        }

                        if (modelloTrasmissione.SINGLE != "1")
                        {
                            for (int j = 0; j < modelloTrasmissione.MITTENTE.Count; j++)
                            {
                                DocsPaVO.Modelli_Trasmissioni.MittDest Mittente = (DocsPaVO.Modelli_Trasmissioni.MittDest)modelloTrasmissione.MITTENTE[j];
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_SALVA_MODELLI_MITT_DEST");
                            queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                            queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MODELLI_MITT_DEST"));
                            queryMng.setParam("param0", modelloTrasmissione.SYSTEM_ID.ToString());
                            queryMng.setParam("param1", Mittente.CHA_TIPO_MITT_DEST);
                            queryMng.setParam("param2", Mittente.ID_CORR_GLOBALI.ToString());
                            queryMng.setParam("param3", Mittente.ID_RAGIONE.ToString());
                            queryMng.setParam("param4", Mittente.CHA_TIPO_TRASM);
                            if (Mittente.VAR_NOTE_SING != null && Mittente.VAR_NOTE_SING != "")
                                queryMng.setParam("param5", Mittente.VAR_NOTE_SING.Replace("'", "''"));
                            queryMng.setParam("param6", Mittente.CHA_TIPO_URP);
                            queryMng.setParam("param7", Mittente.SCADENZA.ToString());
                            queryMng.setParam("param8", (Mittente.NASCONDI_VERSIONI_PRECEDENTI ? "1" : "NULL"));
                            commandText = queryMng.getSQL();
                            logger.Debug("SQL - salvaModello - ModTrasmissioni.cs - QUERY : " + commandText);

                            dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                            retValue = (rowsAffected > 0);
                        }
                        }

                        //inserisco i destinatari
                        for (int i = 0; i < modelloTrasmissione.RAGIONI_DESTINATARI.Count; i++)
                        {
                            DocsPaVO.Modelli_Trasmissioni.RagioneDest RagDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modelloTrasmissione.RAGIONI_DESTINATARI[i];
                            for (int l = 0; l < RagDest.DESTINATARI.Count; l++)
                            {
                                DocsPaVO.Modelli_Trasmissioni.MittDest MittenteDestinatario = (DocsPaVO.Modelli_Trasmissioni.MittDest)RagDest.DESTINATARI[l];
                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_SALVA_MODELLI_MITT_DEST");
                                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MODELLI_MITT_DEST"));
                                queryMng.setParam("param0", modelloTrasmissione.SYSTEM_ID.ToString());
                                queryMng.setParam("param1", MittenteDestinatario.CHA_TIPO_MITT_DEST);
                                queryMng.setParam("param2", MittenteDestinatario.ID_CORR_GLOBALI.ToString());
                                queryMng.setParam("param3", MittenteDestinatario.ID_RAGIONE.ToString());
                                queryMng.setParam("param4", MittenteDestinatario.CHA_TIPO_TRASM);
                                if (MittenteDestinatario.VAR_NOTE_SING != null && MittenteDestinatario.VAR_NOTE_SING != "")
                                    queryMng.setParam("param5", MittenteDestinatario.VAR_NOTE_SING.Replace("'", "''"));
                                queryMng.setParam("param6", MittenteDestinatario.CHA_TIPO_URP);
                                queryMng.setParam("param7", MittenteDestinatario.SCADENZA.ToString());
                                queryMng.setParam("param8", (MittenteDestinatario.NASCONDI_VERSIONI_PRECEDENTI ? "1" : "NULL"));
                                commandText = queryMng.getSQL();
                                logger.Debug("SQL - salvaModello - ModTrasmissioni.cs - QUERY : " + commandText);

                                dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                                retValue = (rowsAffected > 0);
                                if (retValue)
                                {
                                    // Reperimento systemid
                                    commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_MODELLI_MITT_DEST");
                                    dbProvider.ExecuteScalar(out dbID_MittDest, commandText);
                                    MittenteDestinatario.SYSTEM_ID = Convert.ToInt32(dbID_MittDest);
                                    MittenteDestinatario.ID_MODELLO = modelloTrasmissione.SYSTEM_ID;
                                    RagDest.DESTINATARI[l] = MittenteDestinatario;
                                }
                            }
                            modelloTrasmissione.RAGIONI_DESTINATARI[i] = RagDest;
                        }
                    }
				}

                //gestione notifiche utenti
                modelloTrasmissione = this.UtentiConNotificaTrasm(modelloTrasmissione, null, null, "SET");                
			}
			catch(Exception ex)
			{
                logger.Debug("ERRORE salvaModello - ModTrasmissioni.cs : " + ex.Message);
			}
			finally
			{
				dbProvider.Dispose();
			}
            return result;
		}

        public void deleteModello(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modelloTrasmissione)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            string idpeople = string.Empty;

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_DELETE_MODELLO2");
                queryMng.setParam("idAmm", modelloTrasmissione.ID_AMM);
                queryMng.setParam("nome", modelloTrasmissione.NOME.ToUpper().Replace("'", "''"));
                queryMng.setParam("chaTipoOggetto", modelloTrasmissione.CHA_TIPO_OGGETTO);
                queryMng.setParam("idReg", modelloTrasmissione.ID_REGISTRO);
                queryMng.setParam("note", modelloTrasmissione.VAR_NOTE_GENERALI);
                queryMng.setParam("single", modelloTrasmissione.SINGLE);
                queryMng.setParam("chaCediDiritti", modelloTrasmissione.CEDE_DIRITTI);

                if (modelloTrasmissione.ID_PEOPLE != null && modelloTrasmissione.ID_PEOPLE != "")
                    idpeople += " AND ID_PEOPLE = " + modelloTrasmissione.ID_PEOPLE;
                if (modelloTrasmissione.ID_PEOPLE_NEW_OWNER != null && modelloTrasmissione.ID_PEOPLE_NEW_OWNER != "")
                    idpeople += " AND ID_PEOPLE_NEW_OWNER = " + modelloTrasmissione.ID_PEOPLE_NEW_OWNER;
                if (modelloTrasmissione.ID_GROUP_NEW_OWNER != null && modelloTrasmissione.ID_GROUP_NEW_OWNER != "")
                    idpeople += " AND ID_GROUP_NEW_OWNER = " + modelloTrasmissione.ID_GROUP_NEW_OWNER;
                queryMng.setParam("idPeople", idpeople);
                string comText = queryMng.getSQL();

                dbProvider.ExecuteNonQuery(comText);
            }
            catch (Exception e)
            {
                logger.Debug("Errore nel metodo deleteModello di ModTrasmissioni: " + e);
            }
        }

        public int findModelSystemId()
        {
            int result = 0;
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
            string res = string.Empty;

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_NEXT_VAL");
                string comText = queryMng.getSQL();
                dbProvider.ExecuteScalar(out res, comText);
                if (res != string.Empty && Convert.ToInt32(res) > 0)
                    result = Convert.ToInt32(res);
            }
            catch (Exception e)
            {
                logger.Debug("Errore nel metodo findModelSystemId di ModTrasmissioni: " + e);
            }
            return result;
        }

        public void insertSalvaModello(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modelloTrasmissione)
        {
            string commandText = string.Empty;
            int rowsAffected;
            bool retValue = false;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            bool notificheUtImpostate = false;
            string dbID_MittDest = string.Empty;

            if (modelloTrasmissione.SINGLE != "1")
            {
                for (int j = 0; j < modelloTrasmissione.MITTENTE.Count; j++)
                {
                    DocsPaVO.Modelli_Trasmissioni.MittDest Mittente = (DocsPaVO.Modelli_Trasmissioni.MittDest)modelloTrasmissione.MITTENTE[j];
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_SALVA_MODELLI_MITT_DEST");
                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MODELLI_MITT_DEST"));
                queryMng.setParam("param0", modelloTrasmissione.SYSTEM_ID.ToString());
                queryMng.setParam("param1", Mittente.CHA_TIPO_MITT_DEST);
                queryMng.setParam("param2", Mittente.ID_CORR_GLOBALI.ToString());
                queryMng.setParam("param3", Mittente.ID_RAGIONE.ToString());
                queryMng.setParam("param4", Mittente.CHA_TIPO_TRASM);
                if (Mittente.VAR_NOTE_SING != null && Mittente.VAR_NOTE_SING != "")
                    queryMng.setParam("param5", Mittente.VAR_NOTE_SING.Replace("'", "''"));
                queryMng.setParam("param6", Mittente.CHA_TIPO_URP);
                queryMng.setParam("param7", Mittente.SCADENZA.ToString());
                queryMng.setParam("param8", (Mittente.NASCONDI_VERSIONI_PRECEDENTI ? "1" : "NULL"));
                commandText = queryMng.getSQL();
                logger.Debug("SQL - salvaModello - ModTrasmissioni.cs - QUERY : " + commandText);

                dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                retValue = (rowsAffected > 0);
                }
                //if (retValue)
                //    dbProvider.CommitTransaction();
                //else
                //    dbProvider.RollbackTransaction();
            }

            //inserisco i destinatari
            for (int i = 0; i < modelloTrasmissione.RAGIONI_DESTINATARI.Count; i++)
            {
                DocsPaVO.Modelli_Trasmissioni.RagioneDest RagDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modelloTrasmissione.RAGIONI_DESTINATARI[i];
                for (int l = 0; l < RagDest.DESTINATARI.Count; l++)
                {
                    DocsPaVO.Modelli_Trasmissioni.MittDest MittenteDestinatario = (DocsPaVO.Modelli_Trasmissioni.MittDest)RagDest.DESTINATARI[l];
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_SALVA_MODELLI_MITT_DEST");
                    queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MODELLI_MITT_DEST"));
                    queryMng.setParam("param0", modelloTrasmissione.SYSTEM_ID.ToString());
                    queryMng.setParam("param1", MittenteDestinatario.CHA_TIPO_MITT_DEST);
                    queryMng.setParam("param2", MittenteDestinatario.ID_CORR_GLOBALI.ToString());
                    queryMng.setParam("param3", MittenteDestinatario.ID_RAGIONE.ToString());
                    queryMng.setParam("param4", MittenteDestinatario.CHA_TIPO_TRASM);
                    if (MittenteDestinatario.VAR_NOTE_SING != null && MittenteDestinatario.VAR_NOTE_SING != "")
                        queryMng.setParam("param5", MittenteDestinatario.VAR_NOTE_SING.Replace("'", "''"));
                    queryMng.setParam("param6", MittenteDestinatario.CHA_TIPO_URP);
                    queryMng.setParam("param7", MittenteDestinatario.SCADENZA.ToString());
                    queryMng.setParam("param8", (MittenteDestinatario.NASCONDI_VERSIONI_PRECEDENTI ? "1" : "NULL"));
                    commandText = queryMng.getSQL();
                    logger.Debug("SQL - salvaModello - ModTrasmissioni.cs - QUERY : " + commandText);

                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    retValue = (rowsAffected > 0);
                    if (retValue)
                    {
                        // Reperimento systemid
                        commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_MODELLI_MITT_DEST");
                        dbProvider.ExecuteScalar(out dbID_MittDest, commandText);
                        MittenteDestinatario.SYSTEM_ID = Convert.ToInt32(dbID_MittDest);
                        MittenteDestinatario.ID_MODELLO = Convert.ToInt32(modelloTrasmissione.SYSTEM_ID.ToString());
                        RagDest.DESTINATARI[l] = MittenteDestinatario;

                        notificheUtImpostate = (MittenteDestinatario.UTENTI_NOTIFICA != null);
                        //dbProvider.CommitTransaction();
                    }
                    //else
                    //    dbProvider.RollbackTransaction();								
                }
                modelloTrasmissione.RAGIONI_DESTINATARI[i] = RagDest;
            }

            //gestione notifiche utenti
            if (notificheUtImpostate)
                modelloTrasmissione = this.UtentiConNotificaTrasm(modelloTrasmissione, null, null, "SET");    
        }

		//OK
		public ArrayList getModelliByAmm(string idAmm)
		{
			ArrayList modelli = new ArrayList();
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

			try
			{
				//semaforo.WaitOne();
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI");		
				queryMng.setParam("param1",idAmm);
                queryMng.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
				string commandText=queryMng.getSQL();				
                logger.Debug("SQL - getModelliByAmm - ModTrasmissioni.cs - QUERY : " + commandText);

				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				
				for(int i=0;i<ds.Tables[0].Rows.Count;i++)
				{
					//dati modello
					DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
					modello.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
					modello.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
					modello.CHA_TIPO_OGGETTO = ds.Tables[0].Rows[i]["CHA_TIPO_OGGETTO"].ToString();
					modello.ID_REGISTRO = ds.Tables[0].Rows[i]["ID_REGISTRO"].ToString();
					modello.VAR_NOTE_GENERALI = ds.Tables[0].Rows[i]["VAR_NOTE_GENERALI"].ToString();
					modello.ID_PEOPLE = ds.Tables[0].Rows[i]["ID_PEOPLE"].ToString();
					modello.SINGLE = ds.Tables[0].Rows[i]["SINGLE"].ToString();
					modello.ID_AMM = ds.Tables[0].Rows[i]["ID_AMM"].ToString();
                    modello.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                    modello.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                    modello.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                    modello.NO_NOTIFY = ds.Tables[0].Rows[i]["NO_NOTIFY"].ToString();
                    modello.CODICE = "MT_" + modello.SYSTEM_ID;

                    //////////if (ds.Tables[0].Rows[i]["HIDE_DOC_VERSIONS"] != DBNull.Value)
                    //////////    modello.NASCONDI_VERSIONI_PRECEDENTI = (ds.Tables[0].Rows[i]["HIDE_DOC_VERSIONS"].ToString() == "1");
                    
					//SINGLE="0" il mittente è un ruolo e viene istanziato l'oggetto mittente
					//SINGLE="1" il mittente è tutto la AOO (registro)
					if(modello.SINGLE=="0")
					{
						//dati mittente
						queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MITTENTE");		
						queryMng.setParam("param1",ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
						commandText=queryMng.getSQL();						
                        logger.Debug("SQL - getModelliByAmm - ModTrasmissioni.cs - QUERY : " + commandText);

						DataSet ds1= new DataSet();
						dbProvider.ExecuteQuery(ds1,commandText);

						if(ds1.Tables[0].Rows.Count>0)
						{
                            modello.MITTENTE = new ArrayList();
                            for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                            {
							DocsPaVO.Modelli_Trasmissioni.MittDest mittente = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                                mittente.SYSTEM_ID = Convert.ToInt32(ds1.Tables[0].Rows[k]["SYSTEM_ID"].ToString());
                                mittente.ID_MODELLO = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_MODELLO"].ToString());
                                mittente.CHA_TIPO_MITT_DEST = ds1.Tables[0].Rows[k]["CHA_TIPO_MITT_DEST"].ToString();
                                mittente.VAR_COD_RUBRICA = ds1.Tables[0].Rows[k]["VAR_COD_RUBRICA"].ToString();
                                mittente.ID_RAGIONE = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_RAGIONE"].ToString());
                                mittente.CHA_TIPO_TRASM = ds1.Tables[0].Rows[k]["CHA_TIPO_TRASM"].ToString();
                                mittente.VAR_NOTE_SING = ds1.Tables[0].Rows[k]["VAR_NOTE_SING"].ToString();
                                mittente.DESCRIZIONE = ds1.Tables[0].Rows[k]["VAR_DESC_CORR"].ToString();
                                mittente.CHA_TIPO_URP = ds1.Tables[0].Rows[k]["CHA_TIPO_URP"].ToString();
                                mittente.ID_CORR_GLOBALI = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_CORR_GLOBALI"].ToString());
                                mittente.NASCONDI_VERSIONI_PRECEDENTI = (ds1.Tables[0].Rows[k]["HIDE_DOC_VERSIONS"].ToString() == "1");
                                modello.MITTENTE.Add(mittente);
                            }
						}
					}

					//dati destinatari
					queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_DESTINATARI");		
					queryMng.setParam("param1",ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                    queryMng.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
					commandText=queryMng.getSQL();					
                    logger.Debug("SQL - getModelloByID - ModTrasmissioni.cs - QUERY : " + commandText);

					DataSet ds2 = new DataSet();
					dbProvider.ExecuteQuery(ds2,commandText);

					DocsPaVO.Modelli_Trasmissioni.RagioneDest rgDest = null;
					for(int j=0; j<ds2.Tables[0].Rows.Count; j++)
					{
						if(rgDest != null && rgDest.RAGIONE == ds2.Tables[0].Rows[j]["VAR_DESC_RAGIONE"].ToString())
						{
							DocsPaVO.Modelli_Trasmissioni.MittDest destinatario = new DocsPaVO.Modelli_Trasmissioni.MittDest();
							destinatario.SYSTEM_ID = Convert.ToInt32(ds2.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
							destinatario.ID_MODELLO = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_MODELLO"].ToString());
							destinatario.CHA_TIPO_MITT_DEST = ds2.Tables[0].Rows[j]["CHA_TIPO_MITT_DEST"].ToString();
							destinatario.VAR_COD_RUBRICA = ds2.Tables[0].Rows[j]["VAR_COD_RUBRICA"].ToString();
							destinatario.ID_RAGIONE = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_RAGIONE"].ToString());
							destinatario.CHA_TIPO_TRASM = ds2.Tables[0].Rows[j]["CHA_TIPO_TRASM"].ToString();
							destinatario.VAR_NOTE_SING = ds2.Tables[0].Rows[j]["VAR_NOTE_SING"].ToString();
							destinatario.DESCRIZIONE = ds2.Tables[0].Rows[j]["VAR_DESC_CORR"].ToString();
							destinatario.CHA_TIPO_URP = ds2.Tables[0].Rows[j]["CHA_TIPO_URP"].ToString();
							destinatario.ID_CORR_GLOBALI = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_CORR_GLOBALI"].ToString());
                            if (ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != null && ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != "")
                                destinatario.SCADENZA = Convert.ToInt32(ds2.Tables[0].Rows[j]["SCADENZA"].ToString());
                            else
                                destinatario.SCADENZA = 0;
                            destinatario.NASCONDI_VERSIONI_PRECEDENTI = (ds2.Tables[0].Rows[j]["HIDE_DOC_VERSIONS"].ToString() == "1");
                            rgDest.DESTINATARI.Add(destinatario);
						}
						else
						{
							if(rgDest != null)
								modello.RAGIONI_DESTINATARI.Add(rgDest);
							rgDest = new DocsPaVO.Modelli_Trasmissioni.RagioneDest();
							rgDest.RAGIONE = ds2.Tables[0].Rows[j]["VAR_DESC_RAGIONE"].ToString();
							rgDest.CHA_TIPO_RAGIONE =ds2.Tables[0].Rows[j]["CHA_TIPO_RAGIONE"].ToString();
							DocsPaVO.Modelli_Trasmissioni.MittDest destinatario = new DocsPaVO.Modelli_Trasmissioni.MittDest();
							destinatario.SYSTEM_ID = Convert.ToInt32(ds2.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
							destinatario.ID_MODELLO = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_MODELLO"].ToString());
							destinatario.CHA_TIPO_MITT_DEST = ds2.Tables[0].Rows[j]["CHA_TIPO_MITT_DEST"].ToString();
							destinatario.VAR_COD_RUBRICA = ds2.Tables[0].Rows[j]["VAR_COD_RUBRICA"].ToString();
							destinatario.ID_RAGIONE = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_RAGIONE"].ToString());
							destinatario.CHA_TIPO_TRASM = ds2.Tables[0].Rows[j]["CHA_TIPO_TRASM"].ToString();
							destinatario.VAR_NOTE_SING = ds2.Tables[0].Rows[j]["VAR_NOTE_SING"].ToString();
							destinatario.DESCRIZIONE = ds2.Tables[0].Rows[j]["VAR_DESC_CORR"].ToString();
							destinatario.CHA_TIPO_URP = ds2.Tables[0].Rows[j]["CHA_TIPO_URP"].ToString();
							destinatario.ID_CORR_GLOBALI = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_CORR_GLOBALI"].ToString());
                            if (ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != null && ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != "")
                                destinatario.SCADENZA = Convert.ToInt32(ds2.Tables[0].Rows[j]["SCADENZA"].ToString());
                            else
                                destinatario.SCADENZA = 0;
                            destinatario.NASCONDI_VERSIONI_PRECEDENTI = (ds2.Tables[0].Rows[j]["HIDE_DOC_VERSIONS"].ToString() == "1");
                            rgDest.DESTINATARI.Add(destinatario);				
						}					
					}
					modello.RAGIONI_DESTINATARI.Add(rgDest);


					if(modello.MITTENTE != null)
					{
						modelli.Add(modello);
					}
					else
					{
						if(modello.SINGLE == "1")
							modelli.Add(modello);
					}
				}
			}
			catch
			{
				return null;
			}
			finally
			{
				dbProvider.Dispose();
				//semaforo.ReleaseMutex();
			}
			return modelli;
		}

		//OK
		public ArrayList getModelliByAmmPaging(string idAmm, int nPagina, string ricerca, string codice, out int numTotPag)
		{
			ArrayList modelli = new ArrayList();
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
			numTotPag = 0;
			try
			{
				//semaforo.WaitOne();
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI");		
				queryMng.setParam("param1",idAmm);
                string whereCond  = "";
                if (ricerca == "")
                    whereCond = " AND ID_PEOPLE IS NULL ";
                else
                   whereCond = " AND ID_PEOPLE IS NULL AND UPPER(NOME) like UPPER('%" + ricerca.Replace("'","''") + "%')";
                if (codice != null && !codice.Equals(""))
                {
                    string cond = codice.Substring(codice.IndexOf("_") + 1);
                    whereCond += " AND SYSTEM_ID = '" + cond.ToUpper().Replace("'", "''") + "'";
                }
				queryMng.setParam("param2",whereCond);

				string commandText=queryMng.getSQL();				
                logger.Debug("SQL - getModelliByAmm - ModTrasmissioni.cs - QUERY : " + commandText);

				DataSet ds_1 = new DataSet();
				dbProvider.ExecuteQuery(ds_1,commandText);
				numTotPag = ds_1.Tables[0].Rows.Count;
				
				DataSet ds = new DataSet();
				if(nPagina == 0)
					dbProvider.ExecutePaging(out ds,nPagina,15,commandText);
				else
					dbProvider.ExecutePaging(out ds,(nPagina*15),(nPagina+1)*15,commandText);

				for(int i=0;i<ds.Tables[0].Rows.Count;i++)
				{
					//dati modello
					DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
					modello.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
					modello.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
					modello.CHA_TIPO_OGGETTO = ds.Tables[0].Rows[i]["CHA_TIPO_OGGETTO"].ToString();
					modello.ID_REGISTRO = ds.Tables[0].Rows[i]["ID_REGISTRO"].ToString();
					modello.VAR_NOTE_GENERALI = ds.Tables[0].Rows[i]["VAR_NOTE_GENERALI"].ToString();
					modello.ID_PEOPLE = ds.Tables[0].Rows[i]["ID_PEOPLE"].ToString();
					modello.SINGLE = ds.Tables[0].Rows[i]["SINGLE"].ToString();
					modello.ID_AMM = ds.Tables[0].Rows[i]["ID_AMM"].ToString();
                    modello.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                    modello.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                    modello.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                    modello.NO_NOTIFY = ds.Tables[0].Rows[i]["NO_NOTIFY"].ToString();
                    modello.CODICE = "MT_" + modello.SYSTEM_ID;

                    ////////////if (ds.Tables[0].Rows[i]["HIDE_DOC_VERSIONS"] != DBNull.Value)
                    ////////////    modello.NASCONDI_VERSIONI_PRECEDENTI = (ds.Tables[0].Rows[i]["HIDE_DOC_VERSIONS"].ToString() == "1");

					//SINGLE="0" il mittente è un ruolo e viene istanziato l'oggetto mittente
					//SINGLE="1" il mittente è tutto la AOO (registro)
					if(modello.SINGLE=="0")
					{
						//dati mittente
						queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MITTENTE");		
						queryMng.setParam("param1",ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
						commandText=queryMng.getSQL();						
                        logger.Debug("SQL - getModelliByAmm - ModTrasmissioni.cs - QUERY : " + commandText);

						DataSet ds1= new DataSet();
						dbProvider.ExecuteQuery(ds1,commandText);

						if(ds1.Tables[0].Rows.Count>0)
						{
                            modello.MITTENTE = new ArrayList();
                            for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                            {
							DocsPaVO.Modelli_Trasmissioni.MittDest mittente = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                                mittente.SYSTEM_ID = Convert.ToInt32(ds1.Tables[0].Rows[k]["SYSTEM_ID"].ToString());
                                mittente.ID_MODELLO = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_MODELLO"].ToString());
                                mittente.CHA_TIPO_MITT_DEST = ds1.Tables[0].Rows[k]["CHA_TIPO_MITT_DEST"].ToString();
                                mittente.VAR_COD_RUBRICA = ds1.Tables[0].Rows[k]["VAR_COD_RUBRICA"].ToString();
                                mittente.ID_RAGIONE = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_RAGIONE"].ToString());
                                mittente.CHA_TIPO_TRASM = ds1.Tables[0].Rows[k]["CHA_TIPO_TRASM"].ToString();
                                mittente.VAR_NOTE_SING = ds1.Tables[0].Rows[k]["VAR_NOTE_SING"].ToString();
                                mittente.DESCRIZIONE = ds1.Tables[0].Rows[k]["VAR_DESC_CORR"].ToString();
                                mittente.CHA_TIPO_URP = ds1.Tables[0].Rows[k]["CHA_TIPO_URP"].ToString();
                                mittente.ID_CORR_GLOBALI = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_CORR_GLOBALI"].ToString());
                                mittente.NASCONDI_VERSIONI_PRECEDENTI = (ds1.Tables[0].Rows[k]["HIDE_DOC_VERSIONS"].ToString() == "1");
                                modello.MITTENTE.Add(mittente);
                            }
						}
					}

					//dati destinatari
					queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_DESTINATARI");		
					queryMng.setParam("param1",ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                    queryMng.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

					commandText=queryMng.getSQL();					
                    logger.Debug("SQL - getModelloByID - ModTrasmissioni.cs - QUERY : " + commandText);

					DataSet ds2 = new DataSet();
					dbProvider.ExecuteQuery(ds2,commandText);

					DocsPaVO.Modelli_Trasmissioni.RagioneDest rgDest = null;
					for(int j=0; j<ds2.Tables[0].Rows.Count; j++)
					{
						if(rgDest != null && rgDest.RAGIONE == ds2.Tables[0].Rows[j]["VAR_DESC_RAGIONE"].ToString())
						{
							DocsPaVO.Modelli_Trasmissioni.MittDest destinatario = new DocsPaVO.Modelli_Trasmissioni.MittDest();
							destinatario.SYSTEM_ID = Convert.ToInt32(ds2.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
							destinatario.ID_MODELLO = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_MODELLO"].ToString());
							destinatario.CHA_TIPO_MITT_DEST = ds2.Tables[0].Rows[j]["CHA_TIPO_MITT_DEST"].ToString();
							destinatario.VAR_COD_RUBRICA = ds2.Tables[0].Rows[j]["VAR_COD_RUBRICA"].ToString();
							destinatario.ID_RAGIONE = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_RAGIONE"].ToString());
							destinatario.CHA_TIPO_TRASM = ds2.Tables[0].Rows[j]["CHA_TIPO_TRASM"].ToString();
							destinatario.VAR_NOTE_SING = ds2.Tables[0].Rows[j]["VAR_NOTE_SING"].ToString();
							destinatario.DESCRIZIONE = ds2.Tables[0].Rows[j]["VAR_DESC_CORR"].ToString();
							destinatario.CHA_TIPO_URP = ds2.Tables[0].Rows[j]["CHA_TIPO_URP"].ToString();
							destinatario.ID_CORR_GLOBALI = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_CORR_GLOBALI"].ToString());
                            if (ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != null && ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != "")
                                destinatario.SCADENZA = Convert.ToInt32(ds2.Tables[0].Rows[j]["SCADENZA"].ToString());
                            else
                                destinatario.SCADENZA = 0;
                            destinatario.NASCONDI_VERSIONI_PRECEDENTI = (ds2.Tables[0].Rows[j]["HIDE_DOC_VERSIONS"].ToString() == "1");
							rgDest.DESTINATARI.Add(destinatario);
						}
						else
						{
							if(rgDest != null)
								modello.RAGIONI_DESTINATARI.Add(rgDest);
							rgDest = new DocsPaVO.Modelli_Trasmissioni.RagioneDest();
							rgDest.RAGIONE = ds2.Tables[0].Rows[j]["VAR_DESC_RAGIONE"].ToString();
							rgDest.CHA_TIPO_RAGIONE =ds2.Tables[0].Rows[j]["CHA_TIPO_RAGIONE"].ToString();
							DocsPaVO.Modelli_Trasmissioni.MittDest destinatario = new DocsPaVO.Modelli_Trasmissioni.MittDest();
							destinatario.SYSTEM_ID = Convert.ToInt32(ds2.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
							destinatario.ID_MODELLO = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_MODELLO"].ToString());
							destinatario.CHA_TIPO_MITT_DEST = ds2.Tables[0].Rows[j]["CHA_TIPO_MITT_DEST"].ToString();
							destinatario.VAR_COD_RUBRICA = ds2.Tables[0].Rows[j]["VAR_COD_RUBRICA"].ToString();
							destinatario.ID_RAGIONE = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_RAGIONE"].ToString());
							destinatario.CHA_TIPO_TRASM = ds2.Tables[0].Rows[j]["CHA_TIPO_TRASM"].ToString();
							destinatario.VAR_NOTE_SING = ds2.Tables[0].Rows[j]["VAR_NOTE_SING"].ToString();
							destinatario.DESCRIZIONE = ds2.Tables[0].Rows[j]["VAR_DESC_CORR"].ToString();
							destinatario.CHA_TIPO_URP = ds2.Tables[0].Rows[j]["CHA_TIPO_URP"].ToString();
							destinatario.ID_CORR_GLOBALI = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_CORR_GLOBALI"].ToString());
                            if (ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != null && ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != "")
                                destinatario.SCADENZA = Convert.ToInt32(ds2.Tables[0].Rows[j]["SCADENZA"].ToString());
                            else
                                destinatario.SCADENZA = 0;
                            destinatario.NASCONDI_VERSIONI_PRECEDENTI = (ds2.Tables[0].Rows[j]["HIDE_DOC_VERSIONS"].ToString() == "1");
                            rgDest.DESTINATARI.Add(destinatario);				
						}					
					}
					modello.RAGIONI_DESTINATARI.Add(rgDest);
					if(modello.MITTENTE != null)
					{
						modelli.Add(modello);
					}
					else
					{
						if(modello.SINGLE == "1")
							modelli.Add(modello);
					}
				}
			}
			catch
			{
                return modelli;
			}
			finally
			{
				dbProvider.Dispose();
				//semaforo.ReleaseMutex();
			}
			return modelli;
		}

        public ArrayList getModelliByDdlAmmPaging(string idAmm, int nPagina, DocsPaVO.filtri.FiltroRicerca[] filtriRicerca, out int numTotPag)
        {
            ArrayList modelli = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            numTotPag = 0;
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI");
                queryMng.setParam("param1", idAmm);

                String whereCond = GenerateWhereConditionForModels(filtriRicerca);
                
                queryMng.setParam("param2", whereCond);
                queryMng.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                string commandText = queryMng.getSQL();
                logger.Debug("SQL - getModelliByAmm - ModTrasmissioni.cs - QUERY : " + commandText);

                DataSet ds_1 = new DataSet();
                dbProvider.ExecuteQuery(ds_1, commandText);
                numTotPag = ds_1.Tables[0].Rows.Count;

                DataSet ds = new DataSet();
                if (nPagina == 0)
                    dbProvider.ExecutePaging(out ds, nPagina, 15, commandText);
                else
                    dbProvider.ExecutePaging(out ds, (nPagina * 15), (nPagina + 1) * 15, commandText);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //dati modello
                    DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
                    modello.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                    modello.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
                    modello.CHA_TIPO_OGGETTO = ds.Tables[0].Rows[i]["CHA_TIPO_OGGETTO"].ToString();
                    modello.ID_REGISTRO = ds.Tables[0].Rows[i]["ID_REGISTRO"].ToString();
                    modello.VAR_NOTE_GENERALI = ds.Tables[0].Rows[i]["VAR_NOTE_GENERALI"].ToString();
                    modello.ID_PEOPLE = ds.Tables[0].Rows[i]["ID_PEOPLE"].ToString();
                    modello.SINGLE = ds.Tables[0].Rows[i]["SINGLE"].ToString();
                    modello.ID_AMM = ds.Tables[0].Rows[i]["ID_AMM"].ToString();
                    modello.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                    modello.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                    modello.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                    modello.NO_NOTIFY = ds.Tables[0].Rows[i]["NO_NOTIFY"].ToString();
                    modello.CODICE = "MT_" + modello.SYSTEM_ID;

                    if (ds.Tables[0].Columns.Contains("Invalid"))
                        modello.Valid = ds.Tables[0].Rows[i]["Invalid"].ToString() == "0" ? true : false;

                    ////////////if (ds.Tables[0].Rows[i]["HIDE_DOC_VERSIONS"] != DBNull.Value)
                    ////////////    modello.NASCONDI_VERSIONI_PRECEDENTI = (ds.Tables[0].Rows[i]["HIDE_DOC_VERSIONS"].ToString() == "1");

                    //SINGLE="0" il mittente è un ruolo e viene istanziato l'oggetto mittente
                    //SINGLE="1" il mittente è tutto la AOO (registro)
                    if (modello.SINGLE == "0")
                    {
                        //dati mittente
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MITTENTE");
                        queryMng.setParam("param1", ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                        commandText = queryMng.getSQL();
                        logger.Debug("SQL - getModelliByAmm - ModTrasmissioni.cs - QUERY : " + commandText);

                        DataSet ds1 = new DataSet();
                        dbProvider.ExecuteQuery(ds1, commandText);

                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            modello.MITTENTE = new ArrayList();
                            for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                            {
                                DocsPaVO.Modelli_Trasmissioni.MittDest mittente = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                                mittente.SYSTEM_ID = Convert.ToInt32(ds1.Tables[0].Rows[k]["SYSTEM_ID"].ToString());
                                mittente.ID_MODELLO = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_MODELLO"].ToString());
                                mittente.CHA_TIPO_MITT_DEST = ds1.Tables[0].Rows[k]["CHA_TIPO_MITT_DEST"].ToString();
                                mittente.VAR_COD_RUBRICA = ds1.Tables[0].Rows[k]["VAR_COD_RUBRICA"].ToString();
                                mittente.ID_RAGIONE = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_RAGIONE"].ToString());
                                mittente.CHA_TIPO_TRASM = ds1.Tables[0].Rows[k]["CHA_TIPO_TRASM"].ToString();
                                mittente.VAR_NOTE_SING = ds1.Tables[0].Rows[k]["VAR_NOTE_SING"].ToString();
                                mittente.DESCRIZIONE = ds1.Tables[0].Rows[k]["VAR_DESC_CORR"].ToString();
                                mittente.CHA_TIPO_URP = ds1.Tables[0].Rows[k]["CHA_TIPO_URP"].ToString();
                                mittente.ID_CORR_GLOBALI = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_CORR_GLOBALI"].ToString());
                                mittente.NASCONDI_VERSIONI_PRECEDENTI = (ds1.Tables[0].Rows[k]["HIDE_DOC_VERSIONS"].ToString() == "1");
                                modello.MITTENTE.Add(mittente);
                            }
                        }
                    }

                    //dati destinatari
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_DESTINATARI");
                    queryMng.setParam("param1", ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                    queryMng.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                    commandText = queryMng.getSQL();
                    logger.Debug("SQL - getModelloByID - ModTrasmissioni.cs - QUERY : " + commandText);

                    DataSet ds2 = new DataSet();
                    dbProvider.ExecuteQuery(ds2, commandText);

                    DocsPaVO.Modelli_Trasmissioni.RagioneDest rgDest = null;
                    for (int j = 0; j < ds2.Tables[0].Rows.Count; j++)
                    {
                        if (rgDest != null && rgDest.RAGIONE == ds2.Tables[0].Rows[j]["VAR_DESC_RAGIONE"].ToString())
                        {
                            DocsPaVO.Modelli_Trasmissioni.MittDest destinatario = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                            destinatario.SYSTEM_ID = Convert.ToInt32(ds2.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
                            destinatario.ID_MODELLO = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_MODELLO"].ToString());
                            destinatario.CHA_TIPO_MITT_DEST = ds2.Tables[0].Rows[j]["CHA_TIPO_MITT_DEST"].ToString();
                            destinatario.VAR_COD_RUBRICA = ds2.Tables[0].Rows[j]["VAR_COD_RUBRICA"].ToString();
                            destinatario.ID_RAGIONE = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_RAGIONE"].ToString());
                            destinatario.CHA_TIPO_TRASM = ds2.Tables[0].Rows[j]["CHA_TIPO_TRASM"].ToString();
                            destinatario.VAR_NOTE_SING = ds2.Tables[0].Rows[j]["VAR_NOTE_SING"].ToString();
                            destinatario.DESCRIZIONE = ds2.Tables[0].Rows[j]["VAR_DESC_CORR"].ToString();
                            destinatario.CHA_TIPO_URP = ds2.Tables[0].Rows[j]["CHA_TIPO_URP"].ToString();
                            destinatario.ID_CORR_GLOBALI = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_CORR_GLOBALI"].ToString());
                            if (ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != null && ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != "")
                                destinatario.SCADENZA = Convert.ToInt32(ds2.Tables[0].Rows[j]["SCADENZA"].ToString());
                            else
                                destinatario.SCADENZA = 0;
                            destinatario.NASCONDI_VERSIONI_PRECEDENTI = (ds2.Tables[0].Rows[j]["HIDE_DOC_VERSIONS"].ToString() == "1");
                            rgDest.DESTINATARI.Add(destinatario);
                        }
                        else
                        {
                            if (rgDest != null)
                                modello.RAGIONI_DESTINATARI.Add(rgDest);
                            rgDest = new DocsPaVO.Modelli_Trasmissioni.RagioneDest();
                            rgDest.RAGIONE = ds2.Tables[0].Rows[j]["VAR_DESC_RAGIONE"].ToString();
                            rgDest.CHA_TIPO_RAGIONE = ds2.Tables[0].Rows[j]["CHA_TIPO_RAGIONE"].ToString();
                            DocsPaVO.Modelli_Trasmissioni.MittDest destinatario = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                            destinatario.SYSTEM_ID = Convert.ToInt32(ds2.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
                            destinatario.ID_MODELLO = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_MODELLO"].ToString());
                            destinatario.CHA_TIPO_MITT_DEST = ds2.Tables[0].Rows[j]["CHA_TIPO_MITT_DEST"].ToString();
                            destinatario.VAR_COD_RUBRICA = ds2.Tables[0].Rows[j]["VAR_COD_RUBRICA"].ToString();
                            destinatario.ID_RAGIONE = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_RAGIONE"].ToString());
                            destinatario.CHA_TIPO_TRASM = ds2.Tables[0].Rows[j]["CHA_TIPO_TRASM"].ToString();
                            destinatario.VAR_NOTE_SING = ds2.Tables[0].Rows[j]["VAR_NOTE_SING"].ToString();
                            destinatario.DESCRIZIONE = ds2.Tables[0].Rows[j]["VAR_DESC_CORR"].ToString();
                            destinatario.CHA_TIPO_URP = ds2.Tables[0].Rows[j]["CHA_TIPO_URP"].ToString();
                            destinatario.ID_CORR_GLOBALI = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_CORR_GLOBALI"].ToString());
                            if (ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != null && ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != "")
                                destinatario.SCADENZA = Convert.ToInt32(ds2.Tables[0].Rows[j]["SCADENZA"].ToString());
                            else
                                destinatario.SCADENZA = 0;
                            destinatario.NASCONDI_VERSIONI_PRECEDENTI = (ds2.Tables[0].Rows[j]["HIDE_DOC_VERSIONS"].ToString() == "1");
                            rgDest.DESTINATARI.Add(destinatario);
                        }
                    }
                    modello.RAGIONI_DESTINATARI.Add(rgDest);
                    if (modello.MITTENTE != null)
                    {
                        modelli.Add(modello);
                    }
                    else
                    {
                        if (modello.SINGLE == "1")
                            modelli.Add(modello);
                    }
                }
            }
            catch
            {
                return modelli;
            }
            finally
            {
                dbProvider.Dispose();
                //semaforo.ReleaseMutex();
            }
            return modelli;
        }

        

        public string getModelloSystemId()
        {
            string nextVal = "";
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_NEXT_VAL");
            dbProvider.ExecuteScalar(out nextVal, queryMng.getSQL());
            if (dbType.ToUpper().Equals("SQL"))
                nextVal = (Convert.ToInt32(nextVal) + 1).ToString();

            return nextVal;
        }

		//Tutti i modelli utente e ruolo
		public ArrayList getModelliUtente(DocsPaVO.utente.Utente utente, DocsPaVO.utente.InfoUtente infoUt, DocsPaVO.filtri.FiltroRicerca[] filtriRicerca)
		{
			ArrayList modelli = new ArrayList();
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

			try
			{
				string idAmm = utente.idAmministrazione;
				string idPeople = utente.idPeople;
				
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI_1");		
				queryMng.setParam("idAmm",idAmm);
				queryMng.setParam("idPeople",idPeople);
				queryMng.setParam("idCorrGlobali",infoUt.idCorrGlobali);
                queryMng.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                string condRicerca="";
                condRicerca = this.GenerateWhereConditionForModels(filtriRicerca);
                //if (filtriRicerca != null)
                //{
                //    foreach (DocsPaVO.filtri.FiltroRicerca filtro in filtriRicerca)
                //    {
                //        if (filtro != null)
                //        {
                //            switch (filtro.argomento)
                //            {
                //                case "CODICE_MODELLO":
                //                    if (filtro.valore != null && !filtro.valore.Equals(""))
                //                    {
                //                        string cond = filtro.valore.Substring(filtro.valore.IndexOf("_") + 1);
                //                        condRicerca += " AND D.SYSTEM_ID = '" + cond.ToUpper().Replace("'", "''") + "'";
                //                    }
                //                    break;

                //                case "DESCRIZIONE_MODELLO":
                //                    if (filtro.valore != null && !filtro.valore.Equals(""))
                //                    {
                //                        condRicerca = " AND UPPER(NOME) like UPPER('%" + filtro.valore.Replace("'", "''") + "%')";
                //                    }
                //                    break;

                //                case "RUOLI_DISABLED_RIC_TRASM":
                //                    condRicerca += getRoleDisabledTrasmCondition("D", true);
                //                    break;
                //            }
                //        }
                //    }
                //}

                queryMng.setParam("param1", condRicerca);

				string commandText=queryMng.getSQL();				
                logger.Debug("SQL - getModelliUtente - ModTrasmissioni.cs - QUERY : " + commandText);

				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);

				if(ds.Tables[0].Rows.Count != 0)
				{
					for(int i=0;i<ds.Tables[0].Rows.Count;i++)
					{
						//dati modello
						DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
						modello.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
						modello.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
						modello.CHA_TIPO_OGGETTO = ds.Tables[0].Rows[i]["CHA_TIPO_OGGETTO"].ToString();
						modello.ID_REGISTRO = ds.Tables[0].Rows[i]["ID_REGISTRO"].ToString();
						modello.VAR_NOTE_GENERALI = ds.Tables[0].Rows[i]["VAR_NOTE_GENERALI"].ToString();
						modello.ID_PEOPLE = ds.Tables[0].Rows[i]["ID_PEOPLE"].ToString();
						modello.SINGLE = ds.Tables[0].Rows[i]["SINGLE"].ToString();
						modello.ID_AMM = ds.Tables[0].Rows[i]["ID_AMM"].ToString();
                        modello.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                        modello.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                        modello.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                        modello.NO_NOTIFY = ds.Tables[0].Rows[i]["NO_NOTIFY"].ToString();
                        modello.CODICE = "MT_" + modello.SYSTEM_ID;
                        modello.MANTIENI_LETTURA = ds.Tables[0].Rows[i]["CHA_MANTIENI_LETTURA"].ToString();

                        //
                        // MEV Cessione Dirirtti - mantieni Scrittura
                        if (ds.Tables[0].Columns.Contains("CHA_MANTIENI_SCRITTURA"))
                        modello.MANTIENI_SCRITTURA = ds.Tables[0].Rows[i]["CHA_MANTIENI_SCRITTURA"].ToString();
                        // End Mev
                        //

                        // Se il dataset contiene la colonna invalid, viene compilato l'attributo corrispondente
                        if (ds.Tables[0].Columns.Contains("invalid"))
                            modello.Valid = ds.Tables[0].Rows[i]["invalid"].ToString() == "0" ? true : false;

                        // Se il dataset contiene la colonna mittNum, viene compilato l'attributo corrispondente
                        if (ds.Tables[0].Columns.Contains("mittNum"))
                            modello.NumMittenti = Convert.ToInt32(ds.Tables[0].Rows[i]["mittNum"].ToString());

                        //////////if (ds.Tables[0].Rows[i]["HIDE_DOC_VERSIONS"] != DBNull.Value)
                        //////////    modello.NASCONDI_VERSIONI_PRECEDENTI = (ds.Tables[0].Rows[i]["HIDE_DOC_VERSIONS"].ToString() == "1");
						modelli.Add(modello);
					}
				}

				/*
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI");		
                queryMng.setParam("param1",idAmm);
				string whereCond = " AND ID_PEOPLE ="+ idPeople;
				
				queryMng.setParam("param2",whereCond);
				string commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - getModelliUtente - ModTrasmissioni.cs - QUERY : "+commandText);
				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);

				if(ds.Tables[0].Rows.Count != 0)
				{
					for(int i=0;i<ds.Tables[0].Rows.Count;i++)
					{
						//dati modello
						DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
						modello.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
						modello.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
						modello.CHA_TIPO_OGGETTO = ds.Tables[0].Rows[i]["CHA_TIPO_OGGETTO"].ToString();
						modello.ID_REGISTRO = ds.Tables[0].Rows[i]["ID_REGISTRO"].ToString();
						modello.VAR_NOTE_GENERALI = ds.Tables[0].Rows[i]["VAR_NOTE_GENERALI"].ToString();
						modello.ID_PEOPLE = ds.Tables[0].Rows[i]["ID_PEOPLE"].ToString();
						modello.SINGLE = ds.Tables[0].Rows[i]["SINGLE"].ToString();
						modello.ID_AMM = ds.Tables[0].Rows[i]["ID_AMM"].ToString();

						modelli.Add(modello);
					}
				}
				*/
			}
			catch
			{
				return null;
			}
			finally
			{
				dbProvider.Dispose();
				//semaforo.ReleaseMutex();
			}
			return modelli;
		}

        /// <summary>
        /// Metodo per la generazione della clausola where per il recupero di modelli di trasmissione che rispettano certi
        /// criteri di ricerca
        /// </summary>
        /// <param name="filtriRicerca">Filtri da utilizzare per la costruzione della clausola</param>
        /// <returns>Clausola where</returns>
        private String GenerateWhereConditionForModels(DocsPaVO.filtri.FiltroRicerca[] filtriRicerca)
        {
            // Filtri di ricerca
            String whereCond = String.Empty;
            // Clausola exist (per ora ne viene creata una per ogni filtro che necessita di altre tabelle)
            String existClause = " AND EXISTS (SELECT 'x' FROM dpa_modelli_mitt_dest, dpa_corr_globali WHERE dpa_modelli_trasm.system_id = dpa_modelli_mitt_dest.id_modello{0})";
            StringBuilder whereClause = new StringBuilder();

            if (filtriRicerca != null)
            {
                foreach (FiltroRicerca filtro in filtriRicerca)
                {
                    // Parsing del filtro di ricerca e aggiunta della condizione di filtro
                    listaArgomentiModelliTrasmissione filter =
                        (listaArgomentiModelliTrasmissione)
                        Enum.Parse(typeof(listaArgomentiModelliTrasmissione), filtro.argomento, true);

                    switch (filter)
                    {
                        // Codice modello
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO:
                            if (!String.IsNullOrEmpty(filtro.valore))
                            {
                                string cond = filtro.valore.Substring(filtro.valore.IndexOf("_") + 1);
                                whereCond += String.Format(" AND dpa_modelli_trasm.system_id LIKE '%{0}%'", cond.ToUpper().Replace("'", "''"));
                            }
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.DESCRIZIONE_MODELLO:
                            if (String.IsNullOrEmpty(filtro.valore))
                                whereCond = " AND ID_PEOPLE IS NULL ";
                            else
                                whereCond += String.Format(" AND UPPER(dpa_modelli_trasm.nome) LIKE UPPER('%{0}%')", filtro.valore.Replace("'", "''"));
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.RUOLI_DISABLED_RIC_TRASM:
                            whereClause.AppendFormat(existClause, " AND (dpa_modelli_mitt_dest.cha_tipo_urp = 'R' AND dpa_modelli_mitt_dest.cha_tipo_mitt_dest = 'D' AND dpa_modelli_mitt_dest.id_corr_globali = dpa_corr_globali.system_id AND dpa_corr_globali.cha_disabled_trasm ='1')");
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.NOTE:
                            whereCond += String.Format(" AND UPPER(dpa_modelli_trasm.var_note_generali) LIKE UPPER('%{0}%')", filtro.valore.Replace("'", "''"));
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.TIPO_TRASMISSIONE:
                            whereCond += String.Format(" AND UPPER(dpa_modelli_trasm.cha_tipo_oggetto) = '{0}'", filtro.valore.ToUpper());
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.ID_REGISTRO:
                            whereCond += String.Format(" AND dpa_modelli_trasm.id_registro = {0}", filtro.valore);
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.ID_RAGIONE_TRASMISSIONE:
                            whereClause.AppendFormat(String.Format(existClause, " AND dpa_modelli_mitt_dest.id_ragione = {0}"), filtro.valore);
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_CORR_PER_VISIBILITA:
                            whereClause.AppendFormat(String.Format(existClause, " AND (UPPER(dpa_corr_globali.var_codice) = UPPER('{0}') AND dpa_corr_globali.system_id = dpa_modelli_mitt_dest.id_corr_globali AND UPPER(dpa_modelli_mitt_dest.cha_tipo_mitt_dest) = 'M')"), filtro.valore);
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_CORR_PER_DESTINATARIO:
                            whereClause.AppendFormat(String.Format(existClause, " AND (UPPER(dpa_corr_globali.var_codice) = UPPER('{0}') AND dpa_corr_globali.system_id = dpa_modelli_mitt_dest.id_corr_globali AND UPPER(dpa_modelli_mitt_dest.cha_tipo_mitt_dest) = 'D')"), filtro.valore);
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.RUOLI_DEST_DISABLED:
                            whereClause.AppendFormat(existClause, " AND (dpa_modelli_mitt_dest.cha_tipo_urp = 'R' AND dpa_modelli_mitt_dest.cha_tipo_mitt_dest = 'D' AND dpa_modelli_mitt_dest.id_corr_globali = dpa_corr_globali.system_id AND dpa_corr_globali.dta_fine IS NOT NULL)");
                            break;
                        case listaArgomentiModelliTrasmissione.MODELLI_CREATI_DA_UTENTE:
                            whereClause.AppendFormat(existClause, " AND (dpa_modelli_trasm.id_people IS NOT NULL)");
                            break;
                        case listaArgomentiModelliTrasmissione.MODELLI_CREATI_DA_AMMINISTRATORE:
                            whereClause.AppendFormat(existClause, " AND (dpa_modelli_trasm.id_people IS NULL)");
                            break;
                        default:
                            break;
                    }

                }

            }

            // Se esistono dei filtri sulla clausola exist, viene aggiunto alla lista dei filtri
            if (whereClause.Length > 0)
                whereCond += whereClause; //String.Format(existClause, whereClause.ToString());
            return whereCond;
        }

		//OK
		public DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione getModelloByID(string idAmm,string idModello)
		{
			DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = null;
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
			
			try
			{
				//semaforo.WaitOne();
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI");		
				queryMng.setParam("param1",idAmm);
				string whereCond = " AND SYSTEM_ID ="+ idModello;
				queryMng.setParam("param2",whereCond);
                queryMng.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
				string commandText=queryMng.getSQL();				
                logger.Debug("SQL - getModelloByID - ModTrasmissioni.cs - QUERY : " + commandText);

				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);

				if(ds.Tables[0].Rows.Count>0)
				{
					//dati modello
					modello = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
					modello.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
					modello.NOME = ds.Tables[0].Rows[0]["NOME"].ToString();
					modello.CHA_TIPO_OGGETTO = ds.Tables[0].Rows[0]["CHA_TIPO_OGGETTO"].ToString();
                    //if (ds.Tables[0].Rows[0]["ID_REGISTRO"].ToString() == "") {
                    //    modello.ID_REGISTRO = "";
                    //}
                    //else
					    modello.ID_REGISTRO = ds.Tables[0].Rows[0]["ID_REGISTRO"].ToString();
					modello.VAR_NOTE_GENERALI = ds.Tables[0].Rows[0]["VAR_NOTE_GENERALI"].ToString();
					modello.ID_PEOPLE = ds.Tables[0].Rows[0]["ID_PEOPLE"].ToString();
					modello.SINGLE = ds.Tables[0].Rows[0]["SINGLE"].ToString();
					modello.ID_AMM = ds.Tables[0].Rows[0]["ID_AMM"].ToString();
                    modello.CEDE_DIRITTI = ds.Tables[0].Rows[0]["CHA_CEDE_DIRITTI"].ToString();
                    modello.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[0]["ID_PEOPLE_NEW_OWNER"].ToString();
                    modello.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[0]["ID_GROUP_NEW_OWNER"].ToString();
                    modello.NO_NOTIFY = ds.Tables[0].Rows[0]["NO_NOTIFY"].ToString();
                    modello.CODICE = "MT_" + modello.SYSTEM_ID;
                    modello.MANTIENI_LETTURA = ds.Tables[0].Rows[0]["CHA_MANTIENI_LETTURA"].ToString();

                    //
                    // MEV Cessione Diritti - Mantieni Scrittura
                    if (ds.Tables[0].Columns.Contains("CHA_MANTIENI_SCRITTURA"))
                    modello.MANTIENI_SCRITTURA = ds.Tables[0].Rows[0]["CHA_MANTIENI_SCRITTURA"].ToString();
                    // End MEV
                    //

                    //////////if (ds.Tables[0].Rows[0]["HIDE_DOC_VERSIONS"] != DBNull.Value)
                    //////////    modello.NASCONDI_VERSIONI_PRECEDENTI = (ds.Tables[0].Rows[0]["HIDE_DOC_VERSIONS"].ToString() == "1");

					//SINGLE="0" il mittente è un ruolo e viene istanziato l'oggetto mittente
					//SINGLE="1" il mittente è tutto la AOO (registro)
					if(modello.SINGLE=="0")
					{
						//dati mittente
						queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MITTENTE");		
						queryMng.setParam("param1",idModello);
						commandText=queryMng.getSQL();						
                        logger.Debug("SQL - getModelloByID - ModTrasmissioni.cs - QUERY : " + commandText);

						DataSet ds1= new DataSet();
						dbProvider.ExecuteQuery(ds1,commandText);
						
						if(ds1.Tables[0].Rows.Count>0)
						{
                            modello.MITTENTE = new ArrayList();
                            for (int i = 0; i<ds1.Tables[0].Rows.Count; i++)
                            {
							DocsPaVO.Modelli_Trasmissioni.MittDest mittente = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                                mittente.SYSTEM_ID = Convert.ToInt32(ds1.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                                mittente.ID_MODELLO = Convert.ToInt32(ds1.Tables[0].Rows[i]["ID_MODELLO"].ToString());
                                mittente.CHA_TIPO_MITT_DEST = ds1.Tables[0].Rows[i]["CHA_TIPO_MITT_DEST"].ToString();
                                mittente.VAR_COD_RUBRICA = ds1.Tables[0].Rows[i]["VAR_COD_RUBRICA"].ToString();
                                mittente.ID_RAGIONE = Convert.ToInt32(ds1.Tables[0].Rows[i]["ID_RAGIONE"].ToString());
                                mittente.CHA_TIPO_TRASM = ds1.Tables[0].Rows[i]["CHA_TIPO_TRASM"].ToString();
                                mittente.VAR_NOTE_SING = ds1.Tables[0].Rows[i]["VAR_NOTE_SING"].ToString();
                                mittente.DESCRIZIONE = ds1.Tables[0].Rows[i]["VAR_DESC_CORR"].ToString();
                                mittente.CHA_TIPO_URP = ds1.Tables[0].Rows[i]["CHA_TIPO_URP"].ToString();
                                mittente.ID_CORR_GLOBALI = Convert.ToInt32(ds1.Tables[0].Rows[i]["ID_CORR_GLOBALI"].ToString());
                                mittente.NASCONDI_VERSIONI_PRECEDENTI = (ds1.Tables[0].Rows[i]["HIDE_DOC_VERSIONS"].ToString() == "1");
                                modello.MITTENTE.Add(mittente);
                            }
						}
					}
					
					//dati destinatari
					queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_DESTINATARI");		
					queryMng.setParam("param1",idModello);
                    queryMng.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
					commandText=queryMng.getSQL();					
                    logger.Debug("SQL - getModelloByID - ModTrasmissioni.cs - QUERY : " + commandText);

					DataSet ds2 = new DataSet();
					dbProvider.ExecuteQuery(ds2,commandText);
					
					DocsPaVO.Modelli_Trasmissioni.RagioneDest rgDest = null;
					for(int j=0; j<ds2.Tables[0].Rows.Count; j++)
					{
						if(rgDest != null && rgDest.RAGIONE == ds2.Tables[0].Rows[j]["VAR_DESC_RAGIONE"].ToString())
						{
							DocsPaVO.Modelli_Trasmissioni.MittDest destinatario = new DocsPaVO.Modelli_Trasmissioni.MittDest();
							destinatario.SYSTEM_ID = Convert.ToInt32(ds2.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
							destinatario.ID_MODELLO = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_MODELLO"].ToString());
							destinatario.CHA_TIPO_MITT_DEST = ds2.Tables[0].Rows[j]["CHA_TIPO_MITT_DEST"].ToString();
							destinatario.VAR_COD_RUBRICA = ds2.Tables[0].Rows[j]["VAR_COD_RUBRICA"].ToString();
							destinatario.ID_RAGIONE = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_RAGIONE"].ToString());
							destinatario.CHA_TIPO_TRASM = ds2.Tables[0].Rows[j]["CHA_TIPO_TRASM"].ToString();
							destinatario.VAR_NOTE_SING = ds2.Tables[0].Rows[j]["VAR_NOTE_SING"].ToString();
							destinatario.DESCRIZIONE = ds2.Tables[0].Rows[j]["VAR_DESC_CORR"].ToString();
							destinatario.CHA_TIPO_URP = ds2.Tables[0].Rows[j]["CHA_TIPO_URP"].ToString();
							destinatario.ID_CORR_GLOBALI = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_CORR_GLOBALI"].ToString());
                            if (ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != null && ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != "")
                                destinatario.SCADENZA = Convert.ToInt32(ds2.Tables[0].Rows[j]["SCADENZA"].ToString());
                            else
                                destinatario.SCADENZA = 0;
                            destinatario.NASCONDI_VERSIONI_PRECEDENTI = (ds2.Tables[0].Rows[j]["HIDE_DOC_VERSIONS"].ToString() == "1");

                            // Impostazione stato di disabilitazione ed inibizione del destinatario
                            destinatario.Disabled = !String.IsNullOrEmpty(ds2.Tables[0].Rows[j]["DTA_FINE"].ToString());
                            if (destinatario.CHA_TIPO_URP == "R")
                                destinatario.Inhibited = (ds2.Tables[0].Rows[j]["CHA_DISABLED_TRASM"].ToString() == "1");
							rgDest.DESTINATARI.Add(destinatario);                           
						}
						else
						{
							if(rgDest != null)
								modello.RAGIONI_DESTINATARI.Add(rgDest);
							rgDest = new DocsPaVO.Modelli_Trasmissioni.RagioneDest();
							rgDest.RAGIONE = ds2.Tables[0].Rows[j]["VAR_DESC_RAGIONE"].ToString();
							rgDest.CHA_TIPO_RAGIONE = ds2.Tables[0].Rows[j]["CHA_TIPO_RAGIONE"].ToString();
							DocsPaVO.Modelli_Trasmissioni.MittDest destinatario = new DocsPaVO.Modelli_Trasmissioni.MittDest();
							destinatario.SYSTEM_ID = Convert.ToInt32(ds2.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
							destinatario.ID_MODELLO = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_MODELLO"].ToString());
							destinatario.CHA_TIPO_MITT_DEST = ds2.Tables[0].Rows[j]["CHA_TIPO_MITT_DEST"].ToString();
							destinatario.VAR_COD_RUBRICA = ds2.Tables[0].Rows[j]["VAR_COD_RUBRICA"].ToString();
							destinatario.ID_RAGIONE = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_RAGIONE"].ToString());
							destinatario.CHA_TIPO_TRASM = ds2.Tables[0].Rows[j]["CHA_TIPO_TRASM"].ToString();
							destinatario.VAR_NOTE_SING = ds2.Tables[0].Rows[j]["VAR_NOTE_SING"].ToString();
							destinatario.DESCRIZIONE = ds2.Tables[0].Rows[j]["VAR_DESC_CORR"].ToString();
							destinatario.CHA_TIPO_URP = ds2.Tables[0].Rows[j]["CHA_TIPO_URP"].ToString();
							destinatario.ID_CORR_GLOBALI = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_CORR_GLOBALI"].ToString());
                            if (ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != null && ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != "")
                                destinatario.SCADENZA = Convert.ToInt32(ds2.Tables[0].Rows[j]["SCADENZA"].ToString());
                            else
                                destinatario.SCADENZA = 0;
                            destinatario.NASCONDI_VERSIONI_PRECEDENTI = (ds2.Tables[0].Rows[j]["HIDE_DOC_VERSIONS"].ToString() == "1");

                            // Impostazione stato di disabilitazione ed inibizione del destinatario
                            destinatario.Disabled = !String.IsNullOrEmpty(ds2.Tables[0].Rows[j]["DTA_FINE"].ToString());
                            if (destinatario.CHA_TIPO_URP == "R")
                                destinatario.Inhibited = (ds2.Tables[0].Rows[j]["CHA_DISABLED_TRASM"].ToString() == "1");

							rgDest.DESTINATARI.Add(destinatario);
						}					
					}
					modello.RAGIONI_DESTINATARI.Add(rgDest);

                    modello = this.UtentiConNotificaTrasm(modello, null, null, "GET");
				}
			}
			catch
			{
				return null;
			}
			finally
			{
				dbProvider.Dispose();
				//semaforo.ReleaseMutex();
			}
			return modello;
		}

        /// <summary>
        /// Reperisce i dati di modello da il "NOME" dato e ne crea una lista di oggetti di tipo "MODELLO"
        /// </summary>
        /// <param name="idAmm">SYSTEM_ID dell'amministrazione</param>
        /// <param name="nome">NOME dei modelli da ricercare</param>
        /// <param name="tipoOggetto">Ricerca tra... : possibili valori "D" (documenti), "F" (fascicoli)</param>
        /// <param name="idRegistro">SYSTEM_ID del registro</param>
        /// <param name="idRuoloMittente">ID_CORR_GLOB ruolo mittente</param>
        /// <param name="idUtenteMittente">ID_CORR_GLOB utente mittente</param>
        /// <returns>Lista di oggetti Modello (questo perchè è possibile salvare più modelli di trasmissione con lo stesso nome e stesso mittente)</returns>
        public ArrayList getModelliByName(string idAmm, string nome, string tipoOggetto, string idRegistro, string idUtenteMittente, string idRuoloMittente)
        {
            ArrayList modelli = new ArrayList();
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
            string idModelloCorrente = string.Empty;
            string queryWhere = string.Empty;
            string queryUtenteRuolo = string.Empty;

            try 
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI_BY_NAME");

                queryMng.setParam("nome", nome.ToUpper().Replace("'","''"));
                queryMng.setParam("idAmm", idAmm);
                queryMng.setParam("tipoOggetto", tipoOggetto);

                if (idRuoloMittente != null && idRuoloMittente != string.Empty)
                    queryUtenteRuolo = "and ( DPA_MODELLI_MITT_DEST.id_corr_globali =0 or DPA_MODELLI_MITT_DEST.id_corr_globali =" + idRuoloMittente + " )";
                if (idUtenteMittente != null && idUtenteMittente != string.Empty)
                {
                    if (queryUtenteRuolo != string.Empty)
                        queryUtenteRuolo = " and ((DPA_MODELLI_TRASM.id_people =" + idUtenteMittente + " or DPA_MODELLI_TRASM.id_people is null) and ( DPA_MODELLI_MITT_DEST.id_corr_globali =0 or DPA_MODELLI_MITT_DEST.id_corr_globali =" + idRuoloMittente + " )) ";
                    else
                        queryUtenteRuolo = "and (DPA_MODELLI_TRASM.id_people =" + idUtenteMittente + " or DPA_MODELLI_TRASM.id_people is null) ";
                }
                queryMng.setParam("idCorrGlob", queryUtenteRuolo);

                if (idRegistro != null && idRegistro != string.Empty)
                    queryMng.setParam("idRegistro", "AND ID_REGISTRO = " + idRegistro);
                else
                    queryMng.setParam("idRegistro", "");


                string commandText = queryMng.getSQL();				
                logger.Debug(commandText);

				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //dati modello
                    DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
                    modello.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                    idModelloCorrente = modello.SYSTEM_ID.ToString();
                    modello.NOME = ds.Tables[0].Rows[0]["NOME"].ToString();
                    modello.CHA_TIPO_OGGETTO = ds.Tables[0].Rows[0]["CHA_TIPO_OGGETTO"].ToString();
                    modello.ID_REGISTRO = ds.Tables[0].Rows[0]["ID_REGISTRO"].ToString();
                    modello.VAR_NOTE_GENERALI = ds.Tables[0].Rows[0]["VAR_NOTE_GENERALI"].ToString();
                    modello.ID_PEOPLE = ds.Tables[0].Rows[0]["ID_PEOPLE"].ToString();
                    modello.SINGLE = ds.Tables[0].Rows[0]["SINGLE"].ToString();
                    modello.ID_AMM = ds.Tables[0].Rows[0]["ID_AMM"].ToString();
                    modello.CEDE_DIRITTI = ds.Tables[0].Rows[0]["CHA_CEDE_DIRITTI"].ToString();
                    modello.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[0]["ID_PEOPLE_NEW_OWNER"].ToString();
                    modello.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[0]["ID_GROUP_NEW_OWNER"].ToString();
                    modello.NO_NOTIFY = ds.Tables[0].Rows[0]["NO_NOTIFY"].ToString();
                    ////////////if (ds.Tables[0].Rows[i]["HIDE_DOC_VERSIONS"] != DBNull.Value)
                    ////////////    modello.NASCONDI_VERSIONI_PRECEDENTI = (ds.Tables[0].Rows[0]["HIDE_DOC_VERSIONS"].ToString() == "1");

                    if (modello.SINGLE == "0")
                    {
                        //dati mittente
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MITTENTE");
                        queryMng.setParam("param1", idModelloCorrente);
                        commandText = queryMng.getSQL();
                        logger.Debug(commandText);

                        DataSet ds1 = new DataSet();
                        dbProvider.ExecuteQuery(ds1, commandText);

                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            modello.MITTENTE = new ArrayList();
                            for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                            {
                            DocsPaVO.Modelli_Trasmissioni.MittDest mittente = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                                mittente.SYSTEM_ID = Convert.ToInt32(ds1.Tables[0].Rows[k]["SYSTEM_ID"].ToString());
                                mittente.ID_MODELLO = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_MODELLO"].ToString());
                                mittente.CHA_TIPO_MITT_DEST = ds1.Tables[0].Rows[k]["CHA_TIPO_MITT_DEST"].ToString();
                                mittente.VAR_COD_RUBRICA = ds1.Tables[0].Rows[k]["VAR_COD_RUBRICA"].ToString();
                                mittente.ID_RAGIONE = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_RAGIONE"].ToString());
                                mittente.CHA_TIPO_TRASM = ds1.Tables[0].Rows[k]["CHA_TIPO_TRASM"].ToString();
                                mittente.VAR_NOTE_SING = ds1.Tables[0].Rows[k]["VAR_NOTE_SING"].ToString();
                                mittente.DESCRIZIONE = ds1.Tables[0].Rows[k]["VAR_DESC_CORR"].ToString();
                                mittente.CHA_TIPO_URP = ds1.Tables[0].Rows[k]["CHA_TIPO_URP"].ToString();
                                mittente.ID_CORR_GLOBALI = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_CORR_GLOBALI"].ToString());
                                mittente.NASCONDI_VERSIONI_PRECEDENTI = (ds1.Tables[0].Rows[k]["HIDE_DOC_VERSIONS"].ToString() == "1");
                                modello.MITTENTE.Add(mittente);
                            }
                        }
                    }

                    //dati destinatari
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_DESTINATARI");
                    queryMng.setParam("param1", idModelloCorrente);
                    queryMng.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                    commandText = queryMng.getSQL();
                    logger.Debug(commandText);

                    DataSet ds2 = new DataSet();
                    dbProvider.ExecuteQuery(ds2, commandText);

                    DocsPaVO.Modelli_Trasmissioni.RagioneDest rgDest = null;
                    for (int j = 0; j < ds2.Tables[0].Rows.Count; j++)
                    {
                        if (rgDest != null && rgDest.RAGIONE == ds2.Tables[0].Rows[j]["VAR_DESC_RAGIONE"].ToString())
                        {
                            DocsPaVO.Modelli_Trasmissioni.MittDest destinatario = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                            destinatario.SYSTEM_ID = Convert.ToInt32(ds2.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
                            destinatario.ID_MODELLO = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_MODELLO"].ToString());
                            destinatario.CHA_TIPO_MITT_DEST = ds2.Tables[0].Rows[j]["CHA_TIPO_MITT_DEST"].ToString();
                            destinatario.VAR_COD_RUBRICA = ds2.Tables[0].Rows[j]["VAR_COD_RUBRICA"].ToString();
                            destinatario.ID_RAGIONE = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_RAGIONE"].ToString());
                            destinatario.CHA_TIPO_TRASM = ds2.Tables[0].Rows[j]["CHA_TIPO_TRASM"].ToString();
                            destinatario.VAR_NOTE_SING = ds2.Tables[0].Rows[j]["VAR_NOTE_SING"].ToString();
                            destinatario.DESCRIZIONE = ds2.Tables[0].Rows[j]["VAR_DESC_CORR"].ToString();
                            destinatario.CHA_TIPO_URP = ds2.Tables[0].Rows[j]["CHA_TIPO_URP"].ToString();
                            destinatario.ID_CORR_GLOBALI = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_CORR_GLOBALI"].ToString());
                            if (ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != null && ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != "")
                                destinatario.SCADENZA = Convert.ToInt32(ds2.Tables[0].Rows[j]["SCADENZA"].ToString());
                            else
                                destinatario.SCADENZA = 0;
                            destinatario.NASCONDI_VERSIONI_PRECEDENTI = (ds2.Tables[0].Rows[j]["HIDE_DOC_VERSIONS"].ToString() == "1");
                            rgDest.DESTINATARI.Add(destinatario);
                        }
                        else
                        {
                            if (rgDest != null)
                                modello.RAGIONI_DESTINATARI.Add(rgDest);
                            rgDest = new DocsPaVO.Modelli_Trasmissioni.RagioneDest();
                            rgDest.RAGIONE = ds2.Tables[0].Rows[j]["VAR_DESC_RAGIONE"].ToString();
                            rgDest.CHA_TIPO_RAGIONE = ds2.Tables[0].Rows[j]["CHA_TIPO_RAGIONE"].ToString();
                            DocsPaVO.Modelli_Trasmissioni.MittDest destinatario = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                            destinatario.SYSTEM_ID = Convert.ToInt32(ds2.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
                            destinatario.ID_MODELLO = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_MODELLO"].ToString());
                            destinatario.CHA_TIPO_MITT_DEST = ds2.Tables[0].Rows[j]["CHA_TIPO_MITT_DEST"].ToString();
                            destinatario.VAR_COD_RUBRICA = ds2.Tables[0].Rows[j]["VAR_COD_RUBRICA"].ToString();
                            destinatario.ID_RAGIONE = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_RAGIONE"].ToString());
                            destinatario.CHA_TIPO_TRASM = ds2.Tables[0].Rows[j]["CHA_TIPO_TRASM"].ToString();
                            destinatario.VAR_NOTE_SING = ds2.Tables[0].Rows[j]["VAR_NOTE_SING"].ToString();
                            destinatario.DESCRIZIONE = ds2.Tables[0].Rows[j]["VAR_DESC_CORR"].ToString();
                            destinatario.CHA_TIPO_URP = ds2.Tables[0].Rows[j]["CHA_TIPO_URP"].ToString();
                            destinatario.ID_CORR_GLOBALI = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_CORR_GLOBALI"].ToString());
                            if (ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != null && ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != "")
                                destinatario.SCADENZA = Convert.ToInt32(ds2.Tables[0].Rows[j]["SCADENZA"].ToString());
                            else
                                destinatario.SCADENZA = 0;
                            destinatario.NASCONDI_VERSIONI_PRECEDENTI = (ds2.Tables[0].Rows[j]["HIDE_DOC_VERSIONS"].ToString() == "1");
                            rgDest.DESTINATARI.Add(destinatario);
                        }
                    }

                    modello.RAGIONI_DESTINATARI.Add(rgDest);

                    modello = this.UtentiConNotificaTrasm(modello, null, null, "GET");

                    modelli.Add(modello);
                }
            }
            catch
            {
                modelli = null;
            }

            return modelli;
        }

		//OK
		public void CancellaModello(string idAmm, string idModello)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

			try
			{
				semaforo.WaitOne();
				int rowsAffected;
				bool retValue=false;
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_DELETE_MODELLO");		
				queryMng.setParam("param1",idModello);
				queryMng.setParam("param2",idAmm);
				dbProvider.BeginTransaction();
				string commandText=queryMng.getSQL();				
                logger.Debug("SQL - CancellaModello - ModTrasmissioni.cs - QUERY : " + commandText);

				dbProvider.ExecuteNonQuery(commandText,out rowsAffected);
				retValue=(rowsAffected>0);

				if (retValue)
					dbProvider.CommitTransaction();
				else
					dbProvider.RollbackTransaction();
	

				queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_DELETE_MODELLI_MITT_DEST");		
				queryMng.setParam("param1",idModello);
				dbProvider.BeginTransaction();
				commandText=queryMng.getSQL();				
                logger.Debug("SQL - CancellaModello - ModTrasmissioni.cs - QUERY : " + commandText);

                rowsAffected = 0;
				dbProvider.ExecuteNonQuery(commandText,out rowsAffected);
				retValue=(rowsAffected>0);

				if (retValue)
					dbProvider.CommitTransaction();
				else
					dbProvider.RollbackTransaction();


                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_DEL_UT_CON_NOTIFICA");
                queryMng.setParam("id", "ID_MODELLO = " + idModello);
                dbProvider.BeginTransaction();
                commandText = queryMng.getSQL();                
                logger.Debug("SQL - CancellaModello - ModTrasmissioni.cs - QUERY : " + commandText);

                rowsAffected = 0;
                dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                retValue = (rowsAffected > 0);

                if (retValue)
                    dbProvider.CommitTransaction();
                else
                    dbProvider.RollbackTransaction();
			}
			catch 
			{
				dbProvider.RollbackTransaction();				
			}
			finally
			{
				dbProvider.Dispose();
				semaforo.ReleaseMutex();
			}	
		}

		//OK
		public void CancellaDestModello(string idRagione,string varCodRubrica,string idModello)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

			try
			{
				semaforo.WaitOne();
			
				int rowsAffected;
				bool retValue=false;
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("MT_DELETE_DEST_MODELLI");		
				queryMng.setParam("param1",idModello);
				queryMng.setParam("param2",varCodRubrica);
				queryMng.setParam("param3",idRagione);
				dbProvider.BeginTransaction();
				string commandText=queryMng.getSQL();				
                logger.Debug("SQL - CancellaDestModello - ModTrasmissioni.cs - QUERY : " + commandText);

				dbProvider.ExecuteNonQuery(commandText,out rowsAffected);
				retValue=(rowsAffected>0);

				if (retValue)
					dbProvider.CommitTransaction();
				else
					dbProvider.RollbackTransaction();


                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_DEL_UT_CON_NOTIFICA");
                queryMng.setParam("id", "ID_MODELLO = " + idModello);
                dbProvider.BeginTransaction();
                commandText = queryMng.getSQL();                
                logger.Debug("SQL - CancellaModello - ModTrasmissioni.cs - QUERY : " + commandText);

                dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                retValue = (rowsAffected > 0);

                if (retValue)
                    dbProvider.CommitTransaction();
                else
                    dbProvider.RollbackTransaction();
			}
			catch 
			{
				dbProvider.RollbackTransaction();				
			}
			finally
			{
				dbProvider.Dispose();
				semaforo.ReleaseMutex();
			}	
		}

        /// <summary>
        /// GIUGNO 2008 - Adamo
        /// MODELLI DI TRASMISSIONE:
        /// Gestione della notifica trasmissione degli utenti inserito nei modelli di trasmissione
        /// </summary>
        /// <param name="objTrasm">Oggetto Modelli_Trasmissioni.ModelloTrasmissione</param>
        /// <param name="operazione">Tipo di operazione da effettuare sul db:   'GET' = reperimento dati,   'SET' = modifica dei dati </param>
        /// <returns>L'oggetto stesso passato come parametro. Oggetto NULL se ci sono state eccezioni nel metodo</returns>
        public DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione UtentiConNotificaTrasm(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione objModTrasm, ArrayList utentiDaInserire, ArrayList utentiDaCancellare, string operazione)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaUtils.Query query;
            DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm utNotifica;
            string commandText = string.Empty;
            DataSet ds;

            DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
            modello = objModTrasm;

            try
            {                
                foreach (DocsPaVO.Modelli_Trasmissioni.RagioneDest ragioneDest in objModTrasm.RAGIONI_DESTINATARI)                
                {
                    foreach (DocsPaVO.Modelli_Trasmissioni.MittDest mittDest in ragioneDest.DESTINATARI)
                    {
                        // DESTINATARIO RUOLO
                        if (mittDest.CHA_TIPO_MITT_DEST.Equals("D") && mittDest.CHA_TIPO_URP.Equals("R"))
                        {
                            switch (operazione)
                            {
                                case "GET": // reperimento dati

                                    query = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_UTENTI_CON_NOTIFICA");
                                    query.setParam("id_mod_mitt_dest", mittDest.SYSTEM_ID.ToString());
                                    query.setParam("id_corr_glob_ruolo", mittDest.ID_CORR_GLOBALI.ToString());
                                    query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                                    commandText = query.getSQL();
                                    logger.Debug("UtentiConNotificaTrasm QUERY : " + commandText);
                                    ds = new DataSet();
                                    dbProvider.ExecuteQuery(out ds, commandText);
                                    if (ds.Tables[0].Rows.Count > 0)
                                    {
                                        foreach (DataRow row in ds.Tables[0].Rows)
                                        {
                                            utNotifica = new DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm();
                                            utNotifica.ID_PEOPLE = row["ID"].ToString();
                                            utNotifica.CODICE_UTENTE = row["CODICE"].ToString();
                                            utNotifica.NOME_COGNOME_UTENTE = row["UTENTE"].ToString();
                                            utNotifica.FLAG_NOTIFICA = row["FLAG_NOTIFICA"].ToString();

                                            mittDest.UTENTI_NOTIFICA.Add(utNotifica);
                                        }
                                    }
                                    break;

                                case "SET": // modifica dei dati

                                    //// prima elimina tutti i record sul db
                                    //query = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_DEL_UT_CON_NOTIFICA");
                                    //query.setParam("id", "ID_MODELLO_MITT_DEST = " + mittDest.SYSTEM_ID.ToString());
                                    //commandText = query.getSQL();
                                    //logger.Debug("UtentiConNotificaTrasm QUERY : " + commandText);
                                    //dbProvider.ExecuteNonQuery(commandText);

                                    // quindi inserisco i record sul db
                                    foreach (DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm ut in mittDest.UTENTI_NOTIFICA)
                                    {
                                        if (utentiDaInserire != null && utentiDaInserire.Count > 0)
                                        {
                                            foreach (DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm utIns in utentiDaInserire)
                                            {
                                                string resSystemId = "0";
                                                string queryStr = "SELECT SYSTEM_ID FROM DPA_MODELLI_DEST_CON_NOTIFICA WHERE ID_PEOPLE = " + utIns.ID_PEOPLE + " AND ID_MODELLO_MITT_DEST = " + mittDest.SYSTEM_ID.ToString() + " AND ID_MODELLO = " + mittDest.ID_MODELLO.ToString();
                                                dbProvider.ExecuteScalar(out resSystemId, queryStr);
                                                if (resSystemId == null)
                                                {
                                                    query = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_INS_UT_CON_NOTIFICA");

                                                    query.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                                    query.setParam("system_id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MODELLI_MITT_DEST"));
                                                    query.setParam("id_mod_mitt_dest", mittDest.SYSTEM_ID.ToString());
                                                    query.setParam("id_people", utIns.ID_PEOPLE);
                                                    query.setParam("id_modello", mittDest.ID_MODELLO.ToString());
                                                    commandText = query.getSQL();
                                                    logger.Debug("UtentiConNotificaTrasm QUERY : " + commandText);
                                                    dbProvider.ExecuteNonQuery(commandText);
                                                }
                                            }
                                        }

                                        if (utentiDaCancellare != null && utentiDaCancellare.Count > 0)
                                        {
                                            foreach (DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm utCanc in utentiDaCancellare)
                                            {
                                                query = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_DEL_UT_CON_NOTIFICA");
                                                query.setParam("id", "ID_MODELLO_MITT_DEST = " + mittDest.SYSTEM_ID.ToString() + " AND ID_PEOPLE = " + utCanc.ID_PEOPLE);
                                                commandText = query.getSQL();
                                                logger.Debug("UtentiConNotificaTrasm QUERY : " + commandText);
                                                dbProvider.ExecuteNonQuery(commandText);
                                            }
                                        }

                                        if (utentiDaInserire == null && utentiDaCancellare == null)
                                        {
                                            if (ut.FLAG_NOTIFICA.Equals("1"))
                                            {
                                                query = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_INS_UT_CON_NOTIFICA");

                                                query.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                                query.setParam("system_id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MODELLI_MITT_DEST"));
                                                query.setParam("id_mod_mitt_dest", mittDest.SYSTEM_ID.ToString());
                                                query.setParam("id_people", ut.ID_PEOPLE);
                                                query.setParam("id_modello", mittDest.ID_MODELLO.ToString());
                                                commandText = query.getSQL();
                                                logger.Debug("UtentiConNotificaTrasm QUERY : " + commandText);
                                                dbProvider.ExecuteNonQuery(commandText);
                                            }
                                        }
                                    }
                                    break;
                            }
                        }

                        // DESTINATARIO UTENTE
                        if (mittDest.CHA_TIPO_MITT_DEST.Equals("D") && mittDest.CHA_TIPO_URP.Equals("P"))
                        {
                            switch (operazione)
                            {
                                case "GET": // reperimento dati
                                    query = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_UTENTI_CON_NOTIFICA_P");
                                    query.setParam("id_mod_mitt_dest", mittDest.SYSTEM_ID.ToString());
                                    query.setParam("id_corr_glob", mittDest.ID_CORR_GLOBALI.ToString());
                                    query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                                    commandText = query.getSQL();
                                    logger.Debug("UtentiConNotificaTrasm QUERY : " + commandText);
                                    ds = new DataSet();
                                    dbProvider.ExecuteQuery(out ds, commandText);
                                    if (ds.Tables[0].Rows.Count > 0)
                                    {
                                        foreach (DataRow row in ds.Tables[0].Rows)
                                        {
                                            utNotifica = new DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm();
                                            utNotifica.ID_PEOPLE = row["ID"].ToString();
                                            utNotifica.CODICE_UTENTE = row["CODICE"].ToString();
                                            utNotifica.NOME_COGNOME_UTENTE = row["UTENTE"].ToString();
                                            utNotifica.FLAG_NOTIFICA = row["FLAG_NOTIFICA"].ToString();

                                            mittDest.UTENTI_NOTIFICA.Add(utNotifica);
                                        }
                                    }
                                    break;

                                case "SET": // modifica dei dati
                                    // prima elimina tutti i record sul db
                                    query = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_DEL_UT_CON_NOTIFICA");
                                    query.setParam("id", "ID_MODELLO_MITT_DEST = " + mittDest.SYSTEM_ID.ToString());
                                    commandText = query.getSQL();
                                    logger.Debug("UtentiConNotificaTrasm QUERY : " + commandText);
                                    dbProvider.ExecuteNonQuery(commandText);

                                    // quindi inserisco i record sul db
                                    foreach (DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm ut in mittDest.UTENTI_NOTIFICA)
                                    {
                                        if (ut.FLAG_NOTIFICA.Equals("1"))
                                        {
                                            query = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_INS_UT_CON_NOTIFICA");

                                            query.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                            query.setParam("system_id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MODELLI_MITT_DEST"));
                                            query.setParam("id_mod_mitt_dest", mittDest.SYSTEM_ID.ToString());
                                            query.setParam("id_people", ut.ID_PEOPLE);
                                            query.setParam("id_modello", mittDest.ID_MODELLO.ToString());
                                            commandText = query.getSQL();
                                            logger.Debug("UtentiConNotificaTrasm QUERY : " + commandText);
                                            dbProvider.ExecuteNonQuery(commandText);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {                
                objModTrasm = modello;
            }
            finally
            {
                dbProvider.Dispose();
            }
            return objModTrasm;
        }

        /// <summary>
        /// Salva i dati di cessione dei diritti su modello di trasmissione
        /// </summary>
        /// <param name="objTrasm">Oggetto Modelli_Trasmissioni.ModelloTrasmissione</param>        
        /// <returns>True se esito positivo</returns>
        public bool SalvaCessioneDirittiSuModelliTrasm(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione objTrasm)
        {
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query query;
                string commandText = string.Empty;

                query = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_CESSIONE_DIRITTI");

                if(objTrasm.CEDE_DIRITTI!=null && (objTrasm.CEDE_DIRITTI!="" || objTrasm.CEDE_DIRITTI!=string.Empty))
                    query.setParam("flag_cessione", objTrasm.CEDE_DIRITTI);
                else
                    query.setParam("flag_cessione", "0");

                if (objTrasm.ID_PEOPLE_NEW_OWNER != null && (objTrasm.ID_PEOPLE_NEW_OWNER != "" || objTrasm.ID_PEOPLE_NEW_OWNER != string.Empty))
                    query.setParam("id_people_new_owner", objTrasm.ID_PEOPLE_NEW_OWNER);
                else
                    query.setParam("id_people_new_owner", "NULL");

                if (objTrasm.ID_GROUP_NEW_OWNER != null && (objTrasm.ID_GROUP_NEW_OWNER != "" || objTrasm.ID_GROUP_NEW_OWNER != string.Empty))
                    query.setParam("id_group_new_owner", objTrasm.ID_GROUP_NEW_OWNER);
                else
                    query.setParam("id_group_new_owner", "NULL");

                query.setParam("queryWhere", "SYSTEM_ID = " + objTrasm.SYSTEM_ID.ToString());
                
                commandText = query.getSQL();
                logger.Debug("SalvaCessioneDirittiSuModelliTrasm QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public ArrayList getModelliByAmmConRicerca(string idAmm, string codiceRicerca, string tipoRicerca)
        {
            ArrayList modelli = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI");
                queryMng.setParam("param1", idAmm);
                queryMng.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                if (codiceRicerca != "")
                {
                    if (tipoRicerca.Equals("C"))
                    {
                       // string codice = codiceRicerca.Substring(3);
                        queryMng.setParam("param2", " AND SYSTEM_ID=" + codiceRicerca);
                    }
                    else
                    {
                        queryMng.setParam("param2", " AND UPPER(NOME) like UPPER('%" + codiceRicerca.Replace("'", "''") + "%')");
                    }
                }
                string commandText = queryMng.getSQL();
                logger.Debug("SQL - getModelliByAmmConRicerca - ModTrasmissioni.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //dati modello
                    DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
                    modello.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                    modello.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
                    modello.CHA_TIPO_OGGETTO = ds.Tables[0].Rows[i]["CHA_TIPO_OGGETTO"].ToString();
                    modello.ID_REGISTRO = ds.Tables[0].Rows[i]["ID_REGISTRO"].ToString();
                    modello.VAR_NOTE_GENERALI = ds.Tables[0].Rows[i]["VAR_NOTE_GENERALI"].ToString();
                    modello.ID_PEOPLE = ds.Tables[0].Rows[i]["ID_PEOPLE"].ToString();
                    modello.SINGLE = ds.Tables[0].Rows[i]["SINGLE"].ToString();
                    modello.ID_AMM = ds.Tables[0].Rows[i]["ID_AMM"].ToString();
                    modello.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                    modello.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                    modello.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                    modello.NO_NOTIFY = ds.Tables[0].Rows[0]["NO_NOTIFY"].ToString();
                    modello.CODICE = "MT_" + modello.SYSTEM_ID;

                    ////////////if (ds.Tables[0].Rows[i]["HIDE_DOC_VERSIONS"] != DBNull.Value)
                    ////////////    modello.NASCONDI_VERSIONI_PRECEDENTI = (ds.Tables[0].Rows[i]["HIDE_DOC_VERSIONS"].ToString() == "1");

                    //SINGLE="0" il mittente è un ruolo e viene istanziato l'oggetto mittente
                    //SINGLE="1" il mittente è tutto la AOO (registro)
                    if (modello.SINGLE == "0")
                    {
                        //dati mittente
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MITTENTE");
                        queryMng.setParam("param1", ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                        commandText = queryMng.getSQL();
                        logger.Debug("SQL - getModelliByAmm - ModTrasmissioni.cs - QUERY : " + commandText);

                        DataSet ds1 = new DataSet();
                        dbProvider.ExecuteQuery(ds1, commandText);

                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            modello.MITTENTE = new ArrayList();
                            for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                            {
                                DocsPaVO.Modelli_Trasmissioni.MittDest mittente = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                                mittente.SYSTEM_ID = Convert.ToInt32(ds1.Tables[0].Rows[k]["SYSTEM_ID"].ToString());
                                mittente.ID_MODELLO = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_MODELLO"].ToString());
                                mittente.CHA_TIPO_MITT_DEST = ds1.Tables[0].Rows[k]["CHA_TIPO_MITT_DEST"].ToString();
                                mittente.VAR_COD_RUBRICA = ds1.Tables[0].Rows[k]["VAR_COD_RUBRICA"].ToString();
                                mittente.ID_RAGIONE = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_RAGIONE"].ToString());
                                mittente.CHA_TIPO_TRASM = ds1.Tables[0].Rows[k]["CHA_TIPO_TRASM"].ToString();
                                mittente.VAR_NOTE_SING = ds1.Tables[0].Rows[k]["VAR_NOTE_SING"].ToString();
                                mittente.DESCRIZIONE = ds1.Tables[0].Rows[k]["VAR_DESC_CORR"].ToString();
                                mittente.CHA_TIPO_URP = ds1.Tables[0].Rows[k]["CHA_TIPO_URP"].ToString();
                                mittente.ID_CORR_GLOBALI = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_CORR_GLOBALI"].ToString());
                                mittente.NASCONDI_VERSIONI_PRECEDENTI = (ds1.Tables[0].Rows[k]["HIDE_DOC_VERSIONS"].ToString() == "1");
                                modello.MITTENTE.Add(mittente);
                            }
                        }
                    }

                    //dati destinatari
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_DESTINATARI");
                    queryMng.setParam("param1", ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                    queryMng.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                    commandText = queryMng.getSQL();
                    logger.Debug("SQL - getModelloByID - ModTrasmissioni.cs - QUERY : " + commandText);

                    DataSet ds2 = new DataSet();
                    dbProvider.ExecuteQuery(ds2, commandText);

                    DocsPaVO.Modelli_Trasmissioni.RagioneDest rgDest = null;
                    for (int j = 0; j < ds2.Tables[0].Rows.Count; j++)
                    {
                        if (rgDest != null && rgDest.RAGIONE == ds2.Tables[0].Rows[j]["VAR_DESC_RAGIONE"].ToString())
                        {
                            DocsPaVO.Modelli_Trasmissioni.MittDest destinatario = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                            destinatario.SYSTEM_ID = Convert.ToInt32(ds2.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
                            destinatario.ID_MODELLO = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_MODELLO"].ToString());
                            destinatario.CHA_TIPO_MITT_DEST = ds2.Tables[0].Rows[j]["CHA_TIPO_MITT_DEST"].ToString();
                            destinatario.VAR_COD_RUBRICA = ds2.Tables[0].Rows[j]["VAR_COD_RUBRICA"].ToString();
                            destinatario.ID_RAGIONE = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_RAGIONE"].ToString());
                            destinatario.CHA_TIPO_TRASM = ds2.Tables[0].Rows[j]["CHA_TIPO_TRASM"].ToString();
                            destinatario.VAR_NOTE_SING = ds2.Tables[0].Rows[j]["VAR_NOTE_SING"].ToString();
                            destinatario.DESCRIZIONE = ds2.Tables[0].Rows[j]["VAR_DESC_CORR"].ToString();
                            destinatario.CHA_TIPO_URP = ds2.Tables[0].Rows[j]["CHA_TIPO_URP"].ToString();
                            destinatario.ID_CORR_GLOBALI = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_CORR_GLOBALI"].ToString());
                            if (ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != null && ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != "")
                                destinatario.SCADENZA = Convert.ToInt32(ds2.Tables[0].Rows[j]["SCADENZA"].ToString());
                            else
                                destinatario.SCADENZA = 0;
                            destinatario.NASCONDI_VERSIONI_PRECEDENTI = (ds2.Tables[0].Rows[j]["HIDE_DOC_VERSIONS"].ToString() == "1");
                            rgDest.DESTINATARI.Add(destinatario);
                        }
                        else
                        {
                            if (rgDest != null)
                                modello.RAGIONI_DESTINATARI.Add(rgDest);
                            rgDest = new DocsPaVO.Modelli_Trasmissioni.RagioneDest();
                            rgDest.RAGIONE = ds2.Tables[0].Rows[j]["VAR_DESC_RAGIONE"].ToString();
                            rgDest.CHA_TIPO_RAGIONE = ds2.Tables[0].Rows[j]["CHA_TIPO_RAGIONE"].ToString();
                            DocsPaVO.Modelli_Trasmissioni.MittDest destinatario = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                            destinatario.SYSTEM_ID = Convert.ToInt32(ds2.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
                            destinatario.ID_MODELLO = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_MODELLO"].ToString());
                            destinatario.CHA_TIPO_MITT_DEST = ds2.Tables[0].Rows[j]["CHA_TIPO_MITT_DEST"].ToString();
                            destinatario.VAR_COD_RUBRICA = ds2.Tables[0].Rows[j]["VAR_COD_RUBRICA"].ToString();
                            destinatario.ID_RAGIONE = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_RAGIONE"].ToString());
                            destinatario.CHA_TIPO_TRASM = ds2.Tables[0].Rows[j]["CHA_TIPO_TRASM"].ToString();
                            destinatario.VAR_NOTE_SING = ds2.Tables[0].Rows[j]["VAR_NOTE_SING"].ToString();
                            destinatario.DESCRIZIONE = ds2.Tables[0].Rows[j]["VAR_DESC_CORR"].ToString();
                            destinatario.CHA_TIPO_URP = ds2.Tables[0].Rows[j]["CHA_TIPO_URP"].ToString();
                            destinatario.ID_CORR_GLOBALI = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_CORR_GLOBALI"].ToString());
                            if (ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != null && ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != "")
                                destinatario.SCADENZA = Convert.ToInt32(ds2.Tables[0].Rows[j]["SCADENZA"].ToString());
                            else
                                destinatario.SCADENZA = 0;
                            destinatario.NASCONDI_VERSIONI_PRECEDENTI = (ds2.Tables[0].Rows[j]["HIDE_DOC_VERSIONS"].ToString() == "1");
                            rgDest.DESTINATARI.Add(destinatario);
                        }
                    }
                    modello.RAGIONI_DESTINATARI.Add(rgDest);


                    if (modello.MITTENTE != null)
                    {
                        modelli.Add(modello);
                    }
                    else
                    {
                        if (modello.SINGLE == "1")
                            modelli.Add(modello);
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
                //semaforo.ReleaseMutex();
            }
            return modelli;
        }

        public ArrayList getModelliAssDiagramma(string idTipo, string idDiagramma, string stato, string idAmm, bool selezionati, string tipo)
        {
            ArrayList modelli = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_MODELLI_ASS_DIAGRAMMI");
                string filtro = string.Empty;
                if (stato != string.Empty)
                {
                    if (selezionati)
                    {
                        if (tipo == "D")
                            filtro = " d.ID_DIAGRAMMA=" + idDiagramma + " and d.ID_STATO=" + stato + " and m.SYSTEM_ID=d.ID_MOD_TRASM and d.ID_TIPO_DOC=" + idTipo + " ";
                        else
                            filtro = " d.ID_DIAGRAMMA=" + idDiagramma + " and d.ID_STATO=" + stato + " and m.SYSTEM_ID=d.ID_MOD_TRASM and d.ID_TIPO_FASC=" + idTipo + " ";
                    }
                    else
                    {
                        if (tipo == "D")
                        {
                            filtro = " m.SYSTEM_ID not in (" +
                               "select m.SYSTEM_ID from dpa_ass_diagrammi d, DPA_MODELLI_TRASM m where d.ID_DIAGRAMMA=" + idDiagramma + " and d.ID_STATO=" + stato + " and d.ID_TIPO_DOC=" + idTipo + " and m.system_id=d.id_mod_trasm) ";
                        }
                        else
                        {
                            filtro = " m.SYSTEM_ID not in (" +
                                "select m.SYSTEM_ID from dpa_ass_diagrammi d, DPA_MODELLI_TRASM m where d.ID_DIAGRAMMA=" + idDiagramma + " and d.ID_STATO=" + stato + " and d.ID_TIPO_FASC=" + idTipo + " and m.system_id=d.id_mod_trasm) ";
                        }
                    }
                }
                else
                {
                    if (selezionati)
                    {
                        if (tipo == "D")
                            filtro = " (d.ID_DIAGRAMMA=" + idDiagramma + " or d.ID_DIAGRAMMA is null) and d.ID_STATO is null and m.SYSTEM_ID=d.ID_MOD_TRASM and d.ID_TIPO_DOC=" + idTipo + " ";
                        else
                            filtro = " (d.ID_DIAGRAMMA=" + idDiagramma + " or d.ID_DIAGRAMMA is null) and d.ID_STATO is null and m.SYSTEM_ID=d.ID_MOD_TRASM and d.ID_TIPO_FASC=" + idTipo + " ";
                    }
                    else
                    {
                        if (tipo == "D")
                        {
                            filtro = " m.SYSTEM_ID not in (" +
                                "select m.SYSTEM_ID from dpa_ass_diagrammi d, DPA_MODELLI_TRASM m where (d.ID_DIAGRAMMA=" + idDiagramma + " or d.ID_DIAGRAMMA is null) and d.ID_STATO is null and d.ID_TIPO_DOC=" + idTipo + " and m.system_id=d.id_mod_trasm) ";
                        }
                        else
                        {
                            filtro = " m.SYSTEM_ID not in (" +
                               "select m.SYSTEM_ID from dpa_ass_diagrammi d, DPA_MODELLI_TRASM m where (d.ID_DIAGRAMMA=" + idDiagramma + " or d.ID_DIAGRAMMA is null) and d.ID_STATO is null and d.ID_TIPO_FASC=" + idTipo + " and m.system_id=d.id_mod_trasm) ";
                        }
                    }
                }

                queryMng.setParam("idAmm", idAmm);
               // queryMng.setParam("idTipoDoc", tipoDoc);
                queryMng.setParam("filters", filtro);

                string commandText = queryMng.getSQL();

                logger.Debug("SQL - getModelliAssDiagramma - ModTrasmissioni.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //dati modello
                    DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
                    modello.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                    modello.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
                    modello.CHA_TIPO_OGGETTO = ds.Tables[0].Rows[i]["CHA_TIPO_OGGETTO"].ToString();
                    modello.ID_REGISTRO = ds.Tables[0].Rows[i]["ID_REGISTRO"].ToString();
                    modello.VAR_NOTE_GENERALI = ds.Tables[0].Rows[i]["VAR_NOTE_GENERALI"].ToString();
                    modello.ID_PEOPLE = ds.Tables[0].Rows[i]["ID_PEOPLE"].ToString();
                    modello.SINGLE = ds.Tables[0].Rows[i]["SINGLE"].ToString();
                    modello.ID_AMM = ds.Tables[0].Rows[i]["ID_AMM"].ToString();
                    modello.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                    modello.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                    modello.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                    modello.CODICE = "MT_" + modello.SYSTEM_ID;

                    //SINGLE="0" il mittente è un ruolo e viene istanziato l'oggetto mittente
                    //SINGLE="1" il mittente è tutto la AOO (registro)
                    if (modello.SINGLE == "0")
                    {
                        //dati mittente
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MITTENTE");
                        queryMng.setParam("param1", ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                        commandText = queryMng.getSQL();
                        logger.Debug("SQL - getModelliByAmm - ModTrasmissioni.cs - QUERY : " + commandText);

                        DataSet ds1 = new DataSet();
                        dbProvider.ExecuteQuery(ds1, commandText);

                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            modello.MITTENTE = new ArrayList();
                            for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                            {
                                DocsPaVO.Modelli_Trasmissioni.MittDest mittente = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                                mittente.SYSTEM_ID = Convert.ToInt32(ds1.Tables[0].Rows[k]["SYSTEM_ID"].ToString());
                                mittente.ID_MODELLO = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_MODELLO"].ToString());
                                mittente.CHA_TIPO_MITT_DEST = ds1.Tables[0].Rows[k]["CHA_TIPO_MITT_DEST"].ToString();
                                mittente.VAR_COD_RUBRICA = ds1.Tables[0].Rows[k]["VAR_COD_RUBRICA"].ToString();
                                mittente.ID_RAGIONE = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_RAGIONE"].ToString());
                                mittente.CHA_TIPO_TRASM = ds1.Tables[0].Rows[k]["CHA_TIPO_TRASM"].ToString();
                                mittente.VAR_NOTE_SING = ds1.Tables[0].Rows[k]["VAR_NOTE_SING"].ToString();
                                mittente.DESCRIZIONE = ds1.Tables[0].Rows[k]["VAR_DESC_CORR"].ToString();
                                mittente.CHA_TIPO_URP = ds1.Tables[0].Rows[k]["CHA_TIPO_URP"].ToString();
                                mittente.ID_CORR_GLOBALI = Convert.ToInt32(ds1.Tables[0].Rows[k]["ID_CORR_GLOBALI"].ToString());
                                mittente.NASCONDI_VERSIONI_PRECEDENTI = (ds1.Tables[0].Rows[k]["HIDE_DOC_VERSIONS"].ToString() == "1");
                                modello.MITTENTE.Add(mittente);
                            }
                        }
                    }

                    //dati destinatari
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_DESTINATARI");
                    queryMng.setParam("param1", ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                    queryMng.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                    commandText = queryMng.getSQL();
                    logger.Debug("SQL - getModelloByID - ModTrasmissioni.cs - QUERY : " + commandText);

                    DataSet ds2 = new DataSet();
                    dbProvider.ExecuteQuery(ds2, commandText);

                    DocsPaVO.Modelli_Trasmissioni.RagioneDest rgDest = null;
                    for (int j = 0; j < ds2.Tables[0].Rows.Count; j++)
                    {
                        if (rgDest != null && rgDest.RAGIONE == ds2.Tables[0].Rows[j]["VAR_DESC_RAGIONE"].ToString())
                        {
                            DocsPaVO.Modelli_Trasmissioni.MittDest destinatario = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                            destinatario.SYSTEM_ID = Convert.ToInt32(ds2.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
                            destinatario.ID_MODELLO = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_MODELLO"].ToString());
                            destinatario.CHA_TIPO_MITT_DEST = ds2.Tables[0].Rows[j]["CHA_TIPO_MITT_DEST"].ToString();
                            destinatario.VAR_COD_RUBRICA = ds2.Tables[0].Rows[j]["VAR_COD_RUBRICA"].ToString();
                            destinatario.ID_RAGIONE = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_RAGIONE"].ToString());
                            destinatario.CHA_TIPO_TRASM = ds2.Tables[0].Rows[j]["CHA_TIPO_TRASM"].ToString();
                            destinatario.VAR_NOTE_SING = ds2.Tables[0].Rows[j]["VAR_NOTE_SING"].ToString();
                            destinatario.DESCRIZIONE = ds2.Tables[0].Rows[j]["VAR_DESC_CORR"].ToString();
                            destinatario.CHA_TIPO_URP = ds2.Tables[0].Rows[j]["CHA_TIPO_URP"].ToString();
                            destinatario.ID_CORR_GLOBALI = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_CORR_GLOBALI"].ToString());
                            if (ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != null && ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != "")
                                destinatario.SCADENZA = Convert.ToInt32(ds2.Tables[0].Rows[j]["SCADENZA"].ToString());
                            else
                                destinatario.SCADENZA = 0;
                            destinatario.NASCONDI_VERSIONI_PRECEDENTI = (ds2.Tables[0].Rows[j]["HIDE_DOC_VERSIONS"].ToString() == "1");
                            rgDest.DESTINATARI.Add(destinatario);

                        }
                        else
                        {
                            if (rgDest != null)
                                modello.RAGIONI_DESTINATARI.Add(rgDest);
                            rgDest = new DocsPaVO.Modelli_Trasmissioni.RagioneDest();
                            rgDest.RAGIONE = ds2.Tables[0].Rows[j]["VAR_DESC_RAGIONE"].ToString();
                            rgDest.CHA_TIPO_RAGIONE = ds2.Tables[0].Rows[j]["CHA_TIPO_RAGIONE"].ToString();
                            DocsPaVO.Modelli_Trasmissioni.MittDest destinatario = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                            destinatario.SYSTEM_ID = Convert.ToInt32(ds2.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
                            destinatario.ID_MODELLO = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_MODELLO"].ToString());
                            destinatario.CHA_TIPO_MITT_DEST = ds2.Tables[0].Rows[j]["CHA_TIPO_MITT_DEST"].ToString();
                            destinatario.VAR_COD_RUBRICA = ds2.Tables[0].Rows[j]["VAR_COD_RUBRICA"].ToString();
                            destinatario.ID_RAGIONE = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_RAGIONE"].ToString());
                            destinatario.CHA_TIPO_TRASM = ds2.Tables[0].Rows[j]["CHA_TIPO_TRASM"].ToString();
                            destinatario.VAR_NOTE_SING = ds2.Tables[0].Rows[j]["VAR_NOTE_SING"].ToString();
                            destinatario.DESCRIZIONE = ds2.Tables[0].Rows[j]["VAR_DESC_CORR"].ToString();
                            destinatario.CHA_TIPO_URP = ds2.Tables[0].Rows[j]["CHA_TIPO_URP"].ToString();
                            destinatario.ID_CORR_GLOBALI = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_CORR_GLOBALI"].ToString());
                            if (ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != null && ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != "")
                                destinatario.SCADENZA = Convert.ToInt32(ds2.Tables[0].Rows[j]["SCADENZA"].ToString());
                            else
                                destinatario.SCADENZA = 0;
                            rgDest.DESTINATARI.Add(destinatario);
                        }
                    }
                    modello.RAGIONI_DESTINATARI.Add(rgDest);


                    if (modello.MITTENTE != null)
                    {
                        modelli.Add(modello);
                    }
                    else
                    {
                        if (modello.SINGLE == "1")
                            modelli.Add(modello);
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }
            return modelli;
        }

        private string getRoleDisabledTrasmCondition(string tableModelName, bool roleDisabled)
        {
            string condition = string.Empty;

            if (!String.IsNullOrEmpty(tableModelName))
            {
                condition = " and ";
                if(roleDisabled)
                    condition += "exists ( ";
                else
                    condition += "not exists ( ";
                
                condition +=    "select 'x' from dpa_modelli_mitt_dest, dpa_corr_globali " +
                                "where " +
                                "id_modello = " + tableModelName + ".system_id " +
                                "and " +
                                "dpa_modelli_mitt_dest.CHA_TIPO_URP = 'R' " +
                                "and " +
                                "dpa_modelli_mitt_dest.cha_tipo_mitt_dest = 'D' " +
                                "and " +
                                "dpa_modelli_mitt_dest.id_corr_globali = dpa_corr_globali.system_id " +
                                "and " +
                                "dpa_corr_globali.cha_disabled_trasm = '1') ";
            }
            return condition;
        }

        public ArrayList getModelliPerTrasmLite(string idAmm, DocsPaVO.utente.Registro[] registri, string idPeople, string idCorrGlobali, string idTipoDoc, string idTipoFasc, string idDiagramma, string idStato, string cha_tipo_oggetto, string systemId, string idRuoloUtente, bool AllReg, string accessrights)
        {
            ArrayList idModelli = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            string paramRegistri = string.Empty;
            DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();
            if (registri.Length != 0 && !obj.isFiltroAooEnabled())
            {
                paramRegistri = " AND mod.id_registro in (";
                for (int i = 0; i < registri.Length; i++)
                {
                    paramRegistri += ((DocsPaVO.utente.Registro)registri[i]).systemId;
                    paramRegistri += ",";
                }
                if (AllReg)
                {
                    paramRegistri += "0,";
                }

                paramRegistri = paramRegistri.Substring(0, paramRegistri.Length - 1) + ")";
            }

            try
            {
                //PRIMA QUERY = Prende i modelli con single = 1 sul registro e non associati
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI_PER_TRASM_AUTORIZZATI");
                queryMng.setParam("idAmm", idAmm);
                if (paramRegistri != string.Empty)
                    queryMng.setParam("paramRegistri", paramRegistri);
                else 
                    queryMng.setParam("paramRegistri", "");
                queryMng.setParam("cha_tipo_oggetto", cha_tipo_oggetto);

                string paramautorizzato = string.Empty;
                if (!string.IsNullOrEmpty(systemId))
                {
                    if (dbType.ToUpper().Equals("SQL"))
                    {
                        string userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession();
                        paramautorizzato += " and " + userDb + ".getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1 and " + userDb + ".isvalidmodellotrasmissione(mod.system_id) = 0 ";
                        //queryMng.setParam("paramautorizzato", " and " + userDb + ".getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1");
                    }
                    else
                    {
                        paramautorizzato += " and getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1 and isvalidmodellotrasmissione(mod.system_id) = 0 ";
                        //queryMng.setParam("paramautorizzato", " and getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1");
                    }
                }
                else
                {
                    if (cha_tipo_oggetto == "D")
                    {
                        paramautorizzato += String.Format(" and (select count(system_id) from dpa_modelli_mitt_dest where id_modello = mod.system_id and hide_doc_versions = '1') = 0 and {0}isvalidmodellotrasmissione(mod.system_id) = 0 ", dbType.ToUpper().Equals("SQL") ? DocsPaDbManagement.Functions.Functions.GetDbUserSession() + "." : String.Empty);
                        //queryMng.setParam("paramautorizzato", " and (select count(system_id) from dpa_modelli_mitt_dest where id_modello = mod.system_id and hide_doc_versions = '1') = 0");
                    }
                }
                //Condizione per eliminare i modelli che hanno fra i loro destinatari almeno un ruolo disabilitato alla ricezione delle trasmissioni
                paramautorizzato += getRoleDisabledTrasmCondition("mod", false);
                queryMng.setParam("paramautorizzato", paramautorizzato);

                string commandText = queryMng.getSQL();
                logger.Debug("SQL - getModelliPerTrasm - ModTrasmissioni.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
                        mod.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                        mod.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
                        mod.CODICE = "MT_" + mod.SYSTEM_ID;//ds.Tables[0].Rows[i]["CODICE"].ToString();
                        mod.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                        mod.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                        mod.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                        mod.NO_NOTIFY = ds.Tables[0].Rows[i]["NO_NOTIFY"].ToString();
                        mod.MANTIENI_LETTURA = ds.Tables[0].Rows[i]["CHA_MANTIENI_LETTURA"].ToString();
                        //
                        // MEV Cessione Diritti Mantieni Scrittura
                        if (ds.Tables[0].Columns.Contains("CHA_MANTIENI_SCRITTURA"))
                        mod.MANTIENI_SCRITTURA = ds.Tables[0].Rows[i]["CHA_MANTIENI_SCRITTURA"].ToString();
                        // End MEV
                        //
                        idModelli.Add(mod);
                    }
                }

                //SECONDA QUERY = Prende i modelli con single = 0 con mittente uguale a utente loggato e non associati
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI_PER_TRASM_AUTORIZZATI_1");
                queryMng.setParam("idPeople", idPeople);
                queryMng.setParam("idCorrGlobali", idCorrGlobali);
                queryMng.setParam("idAmm", idAmm);
                if (paramRegistri != string.Empty)
                    queryMng.setParam("paramRegistri", paramRegistri);
                else
                    queryMng.setParam("paramRegistri", "");
                queryMng.setParam("cha_tipo_oggetto", cha_tipo_oggetto);
                queryMng.setParam("systemId", systemId);

                paramautorizzato = string.Empty;
                if (!string.IsNullOrEmpty(systemId))
                {
                    if (dbType.ToUpper().Equals("SQL"))
                    {
                        string userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession();
                        paramautorizzato += " and " + userDb + ".getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1";
                        //queryMng.setParam("paramautorizzato", " and " + userDb + ".getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id,"+accessrights+")=1");
                    }
                    else
                    {
                        paramautorizzato += " and getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1";
                        //queryMng.setParam("paramautorizzato", " and getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1");
                    }
                }
                else
                {
                    if (cha_tipo_oggetto == "D")
                    {
                        paramautorizzato += " and (select count(system_id) from dpa_modelli_mitt_dest where id_modello = mod.system_id and hide_doc_versions = '1') = 0";
                        //queryMng.setParam("paramautorizzato", " and (select count(system_id) from dpa_modelli_mitt_dest where id_modello = mod.system_id and hide_doc_versions = '1') = 0");
                    }
                }
                //Condizione per eliminare i modelli che hanno fra i loro destinatari almeno un ruolo disabilitato alla ricezione delle trasmissioni
                paramautorizzato += getRoleDisabledTrasmCondition("mod", false);
                queryMng.setParam("paramautorizzato", paramautorizzato);

                commandText = queryMng.getSQL();
                logger.Debug("SQL - getModelliPerTrasm - ModTrasmissioni.cs - QUERY : " + commandText);

                ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
                        mod.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                        mod.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
                        mod.CODICE = "MT_" + mod.SYSTEM_ID;//ds.Tables[0].Rows[i]["CODICE"].ToString();
                        mod.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                        mod.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                        mod.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                        mod.NO_NOTIFY = ds.Tables[0].Rows[i]["NO_NOTIFY"].ToString();
                        mod.MANTIENI_LETTURA = ds.Tables[0].Rows[i]["CHA_MANTIENI_LETTURA"].ToString();
                        //
                        // MEV Cessione Diritti Mantieni Scrittura
                        if (ds.Tables[0].Columns.Contains("CHA_MANTIENI_SCRITTURA"))
                        mod.MANTIENI_SCRITTURA = ds.Tables[0].Rows[i]["CHA_MANTIENI_SCRITTURA"].ToString();
                        // End MEV
                        //
                        idModelli.Add(mod);
                    }
                }

                if (!string.IsNullOrEmpty(idTipoDoc) || !string.IsNullOrEmpty(idTipoFasc))
                {
                    //TERZA QUERY = Prende i modelli con single = 1 sul registro e associati
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI_PER_TRASM_AUTORIZZATI_2");
                    queryMng.setParam("idAmm", idAmm);
                    if (paramRegistri != string.Empty)
                        queryMng.setParam("paramRegistri", paramRegistri);
                    else
                        queryMng.setParam("paramRegistri", "");
                    queryMng.setParam("cha_tipo_oggetto", cha_tipo_oggetto);
                    //queryMng.setParam("idTipoDoc", idTipoDoc);
                    queryMng.setParam("tipoAssDiagramma", string.IsNullOrEmpty(idTipoFasc) ? "DPA_ASS_DIAGRAMMI.id_tipo_doc=" + idTipoDoc : "DPA_ASS_DIAGRAMMI.id_tipo_fasc=" + idTipoFasc);
                    queryMng.setParam("idRuolo", idCorrGlobali);
                    queryMng.setParam("idPeople", idPeople);
                    queryMng.setParam("systemId", systemId);

                    paramautorizzato = string.Empty;
                    if (!string.IsNullOrEmpty(systemId))
                    {
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            string userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession();
                            paramautorizzato += " and " + userDb + ".getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1";
                            //queryMng.setParam("paramautorizzato", " and " + userDb + ".getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id,"+accessrights+")=1");
                        }
                        else
                        {
                            paramautorizzato += " and getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1";
                            //queryMng.setParam("paramautorizzato", " and getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1");
                        }
                    }
                    else
                    {
                        if (cha_tipo_oggetto == "D")
                        {
                            paramautorizzato += " and (select count(system_id) from dpa_modelli_mitt_dest where id_modello = mod.system_id and hide_doc_versions = '1') = 0";
                            //queryMng.setParam("paramautorizzato", " and (select count(system_id) from dpa_modelli_mitt_dest where id_modello = mod.system_id and hide_doc_versions = '1') = 0");
                        }
                    }
                    //Condizione per eliminare i modelli che hanno fra i loro destinatari almeno un ruolo disabilitato alla ricezione delle trasmissioni
                    paramautorizzato += getRoleDisabledTrasmCondition("mod", false);
                    queryMng.setParam("paramautorizzato", paramautorizzato);

                    string query = "";
                    if (idDiagramma != "")
                    {
                        query += " and DPA_ASS_DIAGRAMMI.id_diagramma=" + idDiagramma;
                    }
                    if (idStato != "")
                    {
                        query += " and DPA_ASS_DIAGRAMMI.id_stato=" + idStato;
                    }

                    query += ")";
                    queryMng.setParam("condizione", query);

                    commandText = queryMng.getSQL();
                    logger.Debug("SQL - getModelliPerTrasm - ModTrasmissioni.cs - QUERY : " + commandText);

                    ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
                            mod.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                            mod.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
                            mod.CODICE = "MT_" + mod.SYSTEM_ID;//ds.Tables[0].Rows[i]["CODICE"].ToString();
                            mod.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                            mod.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                            mod.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                            mod.NO_NOTIFY = ds.Tables[0].Rows[i]["NO_NOTIFY"].ToString();
                            mod.MANTIENI_LETTURA = ds.Tables[0].Rows[i]["CHA_MANTIENI_LETTURA"].ToString();
                            
                            //
                            // MEV Cessione Diritti Mantieni Scrittura
                            // Non è chiaro come venga prelevato il mantieni lettura
                            if (ds.Tables[0].Columns.Contains("CHA_MANTIENI_SCRITTURA"))
                            mod.MANTIENI_SCRITTURA = ds.Tables[0].Rows[i]["CHA_MANTIENI_SCRITTURA"].ToString();
                            // End MEV
                            //
                            
                            idModelli.Add(mod);
                        }
                    }
                }

                //QUARTA QUERY = Prende i modelli con single = 0 con mittente uguale a utente loggato e associati
                if (!string.IsNullOrEmpty(idTipoDoc) || !string.IsNullOrEmpty(idTipoFasc))
                {
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI_PER_TRASM_AUTORIZZATI_3");
                    queryMng.setParam("idPeople", idPeople);
                    queryMng.setParam("idCorrGlobali", idCorrGlobali);
                    queryMng.setParam("idAmm", idAmm);
                    if (paramRegistri != string.Empty)
                        queryMng.setParam("paramRegistri", paramRegistri);
                    else
                        queryMng.setParam("paramRegistri", "");
                    queryMng.setParam("cha_tipo_oggetto", cha_tipo_oggetto);
                    //queryMng.setParam("idTipoDoc", idTipoDoc);
                    queryMng.setParam("tipoAssDiagramma", string.IsNullOrEmpty(idTipoFasc) ? "DPA_ASS_DIAGRAMMI.id_tipo_doc=" + idTipoDoc : "DPA_ASS_DIAGRAMMI.id_tipo_fasc=" + idTipoFasc);
                    queryMng.setParam("systemId", systemId);

                    paramautorizzato = string.Empty;
                    if (!string.IsNullOrEmpty(systemId))
                    {
                        if (dbType.ToUpper().Equals("SQL"))
                        {
                            string userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession();
                            paramautorizzato += " and " + userDb + ".getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1";
                            //queryMng.setParam("paramautorizzato", " and " + userDb + ".getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id,"+accessrights+")=1");
                        }
                        else
                        {
                            paramautorizzato += " and getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1";
                            //queryMng.setParam("paramautorizzato", " and getIfModelloAutorizzato(" + idRuoloUtente + ", " + idPeople + ", " + systemId + ", mod.system_id," + accessrights + ")=1");
                        }
                    }
                    else
                    {
                        if (cha_tipo_oggetto == "D")
                        {
                            paramautorizzato += " and (select count(system_id) from dpa_modelli_mitt_dest where id_modello = mod.system_id and hide_doc_versions = '1') = 0";
                            //queryMng.setParam("paramautorizzato", " and (select count(system_id) from dpa_modelli_mitt_dest where id_modello = mod.system_id and hide_doc_versions = '1') = 0");
                        }
                    }
                    //Condizione per eliminare i modelli che hanno fra i loro destinatari almeno un ruolo disabilitato alla ricezione delle trasmissioni
                    paramautorizzato += getRoleDisabledTrasmCondition("mod", false);
                    queryMng.setParam("paramautorizzato", paramautorizzato);

                    string query = "";
                    if (idDiagramma != "")
                    {
                        query += " and DPA_ASS_DIAGRAMMI.id_diagramma=" + idDiagramma;
                    }
                    if (idStato != "")
                    {
                        query += " and DPA_ASS_DIAGRAMMI.id_stato=" + idStato;
                    }

                    query += ")";
                    queryMng.setParam("condizione", query);

                    commandText = queryMng.getSQL();
                    logger.Debug("SQL - getModelliPerTrasm - ModTrasmissioni.cs - QUERY : " + commandText);

                    ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
                            mod.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                            mod.NOME = ds.Tables[0].Rows[i]["NOME"].ToString();
                            mod.CODICE = "MT_" + mod.SYSTEM_ID;//ds.Tables[0].Rows[i]["CODICE"].ToString();

                            mod.CEDE_DIRITTI = ds.Tables[0].Rows[i]["CHA_CEDE_DIRITTI"].ToString();
                            mod.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[i]["ID_PEOPLE_NEW_OWNER"].ToString();
                            mod.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[i]["ID_GROUP_NEW_OWNER"].ToString();
                            mod.NO_NOTIFY = ds.Tables[0].Rows[i]["NO_NOTIFY"].ToString();
                            mod.MANTIENI_LETTURA = ds.Tables[0].Rows[i]["CHA_MANTIENI_LETTURA"].ToString();
                            
                            //
                            // MEV Cessione Diritti Mantieni Scrittura
                            // Non è chiaro come venga prelevato il mantieni lettura
                            if (ds.Tables[0].Columns.Contains("CHA_MANTIENI_SCRITTURA"))
                            mod.MANTIENI_SCRITTURA = ds.Tables[0].Rows[i]["CHA_MANTIENI_SCRITTURA"].ToString();
                            // End MEV
                            //
                            idModelli.Add(mod);
                        }
                    }
                }
            }
            catch
            {
                return idModelli;
            }
            finally
            {
                dbProvider.Dispose();                
            }
            return idModelli;
        }

        //OK
        public DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione getModelloByIDSoloConNotifica(string idAmm, string idModello)
        {
            DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = null;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //semaforo.WaitOne();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MODELLI");
                queryMng.setParam("param1", idAmm);
                string whereCond = " AND SYSTEM_ID =" + idModello;
                queryMng.setParam("param2", whereCond);
                queryMng.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                string commandText = queryMng.getSQL();
                logger.Debug("SQL - getModelloByID - ModTrasmissioni.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    //dati modello
                    modello = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
                    modello.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                    modello.NOME = ds.Tables[0].Rows[0]["NOME"].ToString();
                    modello.CHA_TIPO_OGGETTO = ds.Tables[0].Rows[0]["CHA_TIPO_OGGETTO"].ToString();
                    //if (ds.Tables[0].Rows[0]["ID_REGISTRO"].ToString() == "") {
                    //    modello.ID_REGISTRO = "";
                    //}
                    //else
                    modello.ID_REGISTRO = ds.Tables[0].Rows[0]["ID_REGISTRO"].ToString();
                    modello.VAR_NOTE_GENERALI = ds.Tables[0].Rows[0]["VAR_NOTE_GENERALI"].ToString();
                    modello.ID_PEOPLE = ds.Tables[0].Rows[0]["ID_PEOPLE"].ToString();
                    modello.SINGLE = ds.Tables[0].Rows[0]["SINGLE"].ToString();
                    modello.ID_AMM = ds.Tables[0].Rows[0]["ID_AMM"].ToString();
                    modello.CEDE_DIRITTI = ds.Tables[0].Rows[0]["CHA_CEDE_DIRITTI"].ToString();
                    modello.ID_PEOPLE_NEW_OWNER = ds.Tables[0].Rows[0]["ID_PEOPLE_NEW_OWNER"].ToString();
                    modello.ID_GROUP_NEW_OWNER = ds.Tables[0].Rows[0]["ID_GROUP_NEW_OWNER"].ToString();
                    modello.NO_NOTIFY = ds.Tables[0].Rows[0]["NO_NOTIFY"].ToString();
                    modello.CODICE = "MT_" + modello.SYSTEM_ID;
                    modello.MANTIENI_LETTURA = ds.Tables[0].Rows[0]["CHA_MANTIENI_LETTURA"].ToString();

                    //
                    // MEV Cessione Diritti - Mantieni Scrittura
                    if (ds.Tables[0].Columns.Contains("CHA_MANTIENI_SCRITTURA"))
                    modello.MANTIENI_SCRITTURA = ds.Tables[0].Rows[0]["CHA_MANTIENI_SCRITTURA"].ToString();
                    // End Mev
                    //

                    //////////if (ds.Tables[0].Rows[0]["HIDE_DOC_VERSIONS"] != DBNull.Value)
                    //////////    modello.NASCONDI_VERSIONI_PRECEDENTI = (ds.Tables[0].Rows[0]["HIDE_DOC_VERSIONS"].ToString() == "1");

                    //SINGLE="0" il mittente è un ruolo e viene istanziato l'oggetto mittente
                    //SINGLE="1" il mittente è tutto la AOO (registro)
                    if (modello.SINGLE == "0")
                    {
                        //dati mittente
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_MITTENTE");
                        queryMng.setParam("param1", idModello);
                        commandText = queryMng.getSQL();
                        logger.Debug("SQL - getModelloByID - ModTrasmissioni.cs - QUERY : " + commandText);

                        DataSet ds1 = new DataSet();
                        dbProvider.ExecuteQuery(ds1, commandText);

                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            modello.MITTENTE = new ArrayList();
                            for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                            {
                                DocsPaVO.Modelli_Trasmissioni.MittDest mittente = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                                mittente.SYSTEM_ID = Convert.ToInt32(ds1.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                                mittente.ID_MODELLO = Convert.ToInt32(ds1.Tables[0].Rows[i]["ID_MODELLO"].ToString());
                                mittente.CHA_TIPO_MITT_DEST = ds1.Tables[0].Rows[i]["CHA_TIPO_MITT_DEST"].ToString();
                                mittente.VAR_COD_RUBRICA = ds1.Tables[0].Rows[i]["VAR_COD_RUBRICA"].ToString();
                                mittente.ID_RAGIONE = Convert.ToInt32(ds1.Tables[0].Rows[i]["ID_RAGIONE"].ToString());
                                mittente.CHA_TIPO_TRASM = ds1.Tables[0].Rows[i]["CHA_TIPO_TRASM"].ToString();
                                mittente.VAR_NOTE_SING = ds1.Tables[0].Rows[i]["VAR_NOTE_SING"].ToString();
                                mittente.DESCRIZIONE = ds1.Tables[0].Rows[i]["VAR_DESC_CORR"].ToString();
                                mittente.CHA_TIPO_URP = ds1.Tables[0].Rows[i]["CHA_TIPO_URP"].ToString();
                                mittente.ID_CORR_GLOBALI = Convert.ToInt32(ds1.Tables[0].Rows[i]["ID_CORR_GLOBALI"].ToString());
                                mittente.NASCONDI_VERSIONI_PRECEDENTI = (ds1.Tables[0].Rows[i]["HIDE_DOC_VERSIONS"].ToString() == "1");
                                modello.MITTENTE.Add(mittente);
                            }
                        }
                    }

                    //dati destinatari
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("MT_GET_DESTINATARI");
                    queryMng.setParam("param1", idModello);
                    queryMng.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                    commandText = queryMng.getSQL();
                    logger.Debug("SQL - getModelloByID - ModTrasmissioni.cs - QUERY : " + commandText);

                    DataSet ds2 = new DataSet();
                    dbProvider.ExecuteQuery(ds2, commandText);

                    DocsPaVO.Modelli_Trasmissioni.RagioneDest rgDest = null;
                    for (int j = 0; j < ds2.Tables[0].Rows.Count; j++)
                    {
                        if (rgDest != null && rgDest.RAGIONE == ds2.Tables[0].Rows[j]["VAR_DESC_RAGIONE"].ToString())
                        {
                            DocsPaVO.Modelli_Trasmissioni.MittDest destinatario = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                            destinatario.SYSTEM_ID = Convert.ToInt32(ds2.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
                            destinatario.ID_MODELLO = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_MODELLO"].ToString());
                            destinatario.CHA_TIPO_MITT_DEST = ds2.Tables[0].Rows[j]["CHA_TIPO_MITT_DEST"].ToString();
                            destinatario.VAR_COD_RUBRICA = ds2.Tables[0].Rows[j]["VAR_COD_RUBRICA"].ToString();
                            destinatario.ID_RAGIONE = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_RAGIONE"].ToString());
                            destinatario.CHA_TIPO_TRASM = ds2.Tables[0].Rows[j]["CHA_TIPO_TRASM"].ToString();
                            destinatario.VAR_NOTE_SING = ds2.Tables[0].Rows[j]["VAR_NOTE_SING"].ToString();
                            destinatario.DESCRIZIONE = ds2.Tables[0].Rows[j]["VAR_DESC_CORR"].ToString();
                            destinatario.CHA_TIPO_URP = ds2.Tables[0].Rows[j]["CHA_TIPO_URP"].ToString();
                            destinatario.ID_CORR_GLOBALI = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_CORR_GLOBALI"].ToString());
                            if (ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != null && ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != "")
                                destinatario.SCADENZA = Convert.ToInt32(ds2.Tables[0].Rows[j]["SCADENZA"].ToString());
                            else
                                destinatario.SCADENZA = 0;
                            destinatario.NASCONDI_VERSIONI_PRECEDENTI = (ds2.Tables[0].Rows[j]["HIDE_DOC_VERSIONS"].ToString() == "1");

                            // Impostazione stato di disabilitazione ed inibizione del destinatario
                            destinatario.Disabled = !String.IsNullOrEmpty(ds2.Tables[0].Rows[j]["DTA_FINE"].ToString());
                            if (destinatario.CHA_TIPO_URP == "R")
                                destinatario.Inhibited = (ds2.Tables[0].Rows[j]["CHA_DISABLED_TRASM"].ToString() == "1");
                            rgDest.DESTINATARI.Add(destinatario);
                        }
                        else
                        {
                            if (rgDest != null)
                                modello.RAGIONI_DESTINATARI.Add(rgDest);
                            rgDest = new DocsPaVO.Modelli_Trasmissioni.RagioneDest();
                            rgDest.RAGIONE = ds2.Tables[0].Rows[j]["VAR_DESC_RAGIONE"].ToString();
                            rgDest.CHA_TIPO_RAGIONE = ds2.Tables[0].Rows[j]["CHA_TIPO_RAGIONE"].ToString();
                            DocsPaVO.Modelli_Trasmissioni.MittDest destinatario = new DocsPaVO.Modelli_Trasmissioni.MittDest();
                            destinatario.SYSTEM_ID = Convert.ToInt32(ds2.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
                            destinatario.ID_MODELLO = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_MODELLO"].ToString());
                            destinatario.CHA_TIPO_MITT_DEST = ds2.Tables[0].Rows[j]["CHA_TIPO_MITT_DEST"].ToString();
                            destinatario.VAR_COD_RUBRICA = ds2.Tables[0].Rows[j]["VAR_COD_RUBRICA"].ToString();
                            destinatario.ID_RAGIONE = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_RAGIONE"].ToString());
                            destinatario.CHA_TIPO_TRASM = ds2.Tables[0].Rows[j]["CHA_TIPO_TRASM"].ToString();
                            destinatario.VAR_NOTE_SING = ds2.Tables[0].Rows[j]["VAR_NOTE_SING"].ToString();
                            destinatario.DESCRIZIONE = ds2.Tables[0].Rows[j]["VAR_DESC_CORR"].ToString();
                            destinatario.CHA_TIPO_URP = ds2.Tables[0].Rows[j]["CHA_TIPO_URP"].ToString();
                            destinatario.ID_CORR_GLOBALI = Convert.ToInt32(ds2.Tables[0].Rows[j]["ID_CORR_GLOBALI"].ToString());
                            if (ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != null && ds2.Tables[0].Rows[j]["SCADENZA"].ToString() != "")
                                destinatario.SCADENZA = Convert.ToInt32(ds2.Tables[0].Rows[j]["SCADENZA"].ToString());
                            else
                                destinatario.SCADENZA = 0;
                            destinatario.NASCONDI_VERSIONI_PRECEDENTI = (ds2.Tables[0].Rows[j]["HIDE_DOC_VERSIONS"].ToString() == "1");

                            // Impostazione stato di disabilitazione ed inibizione del destinatario
                            destinatario.Disabled = !String.IsNullOrEmpty(ds2.Tables[0].Rows[j]["DTA_FINE"].ToString());
                            if (destinatario.CHA_TIPO_URP == "R")
                                destinatario.Inhibited = (ds2.Tables[0].Rows[j]["CHA_DISABLED_TRASM"].ToString() == "1");

                            rgDest.DESTINATARI.Add(destinatario);
                        }
                    }
                    modello.RAGIONI_DESTINATARI.Add(rgDest);

                    modello = this.UtentiConNotificaTrasmSoloConNotifica(modello, null, null, "GET");
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
                //semaforo.ReleaseMutex();
            }
            return modello;
        }

        /// <summary>
        /// GIUGNO 2008 - Adamo
        /// MODELLI DI TRASMISSIONE:
        /// Gestione della notifica trasmissione degli utenti inserito nei modelli di trasmissione
        /// </summary>
        /// <param name="objTrasm">Oggetto Modelli_Trasmissioni.ModelloTrasmissione</param>
        /// <param name="operazione">Tipo di operazione da effettuare sul db:   'GET' = reperimento dati,   'SET' = modifica dei dati </param>
        /// <returns>L'oggetto stesso passato come parametro. Oggetto NULL se ci sono state eccezioni nel metodo</returns>
        public DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione UtentiConNotificaTrasmSoloConNotifica(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione objModTrasm, ArrayList utentiDaInserire, ArrayList utentiDaCancellare, string operazione)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaUtils.Query query;
            DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm utNotifica;
            string commandText = string.Empty;
            DataSet ds;

            DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
            modello = objModTrasm;

            try
            {
                foreach (DocsPaVO.Modelli_Trasmissioni.RagioneDest ragioneDest in objModTrasm.RAGIONI_DESTINATARI)
                {
                    foreach (DocsPaVO.Modelli_Trasmissioni.MittDest mittDest in ragioneDest.DESTINATARI)
                    {
                        // DESTINATARIO RUOLO
                        if (mittDest.CHA_TIPO_MITT_DEST.Equals("D") && mittDest.CHA_TIPO_URP.Equals("R"))
                        {
                            switch (operazione)
                            {
                                case "GET": // reperimento dati

                                    query = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_UTENTI_CON_NOTIFICA_SOLO_NOTIFICA");
                                    query.setParam("id_mod_mitt_dest", mittDest.SYSTEM_ID.ToString());
                                    query.setParam("id_corr_glob_ruolo", mittDest.ID_CORR_GLOBALI.ToString());
                                    query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                                    commandText = query.getSQL();
                                    logger.Debug("UtentiConNotificaTrasm QUERY : " + commandText);
                                    ds = new DataSet();
                                    dbProvider.ExecuteQuery(out ds, commandText);
                                    if (ds.Tables[0].Rows.Count > 0)
                                    {
                                        foreach (DataRow row in ds.Tables[0].Rows)
                                        {
                                            utNotifica = new DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm();
                                            utNotifica.ID_PEOPLE = row["ID"].ToString();
                                            utNotifica.CODICE_UTENTE = row["CODICE"].ToString();
                                            utNotifica.NOME_COGNOME_UTENTE = row["UTENTE"].ToString();
                                            utNotifica.FLAG_NOTIFICA = row["FLAG_NOTIFICA"].ToString();

                                            mittDest.UTENTI_NOTIFICA.Add(utNotifica);
                                        }
                                    }
                                    break;

                                case "SET": // modifica dei dati

                                    //// prima elimina tutti i record sul db
                                    //query = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_DEL_UT_CON_NOTIFICA");
                                    //query.setParam("id", "ID_MODELLO_MITT_DEST = " + mittDest.SYSTEM_ID.ToString());
                                    //commandText = query.getSQL();
                                    //logger.Debug("UtentiConNotificaTrasm QUERY : " + commandText);
                                    //dbProvider.ExecuteNonQuery(commandText);

                                    // quindi inserisco i record sul db
                                    foreach (DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm ut in mittDest.UTENTI_NOTIFICA)
                                    {
                                        if (utentiDaInserire != null && utentiDaInserire.Count > 0)
                                        {
                                            foreach (DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm utIns in utentiDaInserire)
                                            {
                                                string resSystemId = "0";
                                                string queryStr = "SELECT SYSTEM_ID FROM DPA_MODELLI_DEST_CON_NOTIFICA WHERE ID_PEOPLE = " + utIns.ID_PEOPLE + " AND ID_MODELLO_MITT_DEST = " + mittDest.SYSTEM_ID.ToString() + " AND ID_MODELLO = " + mittDest.ID_MODELLO.ToString();
                                                dbProvider.ExecuteScalar(out resSystemId, queryStr);
                                                if (resSystemId == null)
                                                {
                                                    query = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_INS_UT_CON_NOTIFICA");

                                                    query.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                                    query.setParam("system_id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MODELLI_MITT_DEST"));
                                                    query.setParam("id_mod_mitt_dest", mittDest.SYSTEM_ID.ToString());
                                                    query.setParam("id_people", utIns.ID_PEOPLE);
                                                    query.setParam("id_modello", mittDest.ID_MODELLO.ToString());
                                                    commandText = query.getSQL();
                                                    logger.Debug("UtentiConNotificaTrasm QUERY : " + commandText);
                                                    dbProvider.ExecuteNonQuery(commandText);
                                                }
                                            }
                                        }

                                        if (utentiDaCancellare != null && utentiDaCancellare.Count > 0)
                                        {
                                            foreach (DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm utCanc in utentiDaCancellare)
                                            {
                                                query = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_DEL_UT_CON_NOTIFICA");
                                                query.setParam("id", "ID_MODELLO_MITT_DEST = " + mittDest.SYSTEM_ID.ToString() + " AND ID_PEOPLE = " + utCanc.ID_PEOPLE);
                                                commandText = query.getSQL();
                                                logger.Debug("UtentiConNotificaTrasm QUERY : " + commandText);
                                                dbProvider.ExecuteNonQuery(commandText);
                                            }
                                        }

                                        if (utentiDaInserire == null && utentiDaCancellare == null)
                                        {
                                            if (ut.FLAG_NOTIFICA.Equals("1"))
                                            {
                                                query = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_INS_UT_CON_NOTIFICA");

                                                query.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                                query.setParam("system_id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MODELLI_MITT_DEST"));
                                                query.setParam("id_mod_mitt_dest", mittDest.SYSTEM_ID.ToString());
                                                query.setParam("id_people", ut.ID_PEOPLE);
                                                query.setParam("id_modello", mittDest.ID_MODELLO.ToString());
                                                commandText = query.getSQL();
                                                logger.Debug("UtentiConNotificaTrasm QUERY : " + commandText);
                                                dbProvider.ExecuteNonQuery(commandText);
                                            }
                                        }
                                    }
                                    break;
                            }
                        }

                        // DESTINATARIO UTENTE
                        if (mittDest.CHA_TIPO_MITT_DEST.Equals("D") && mittDest.CHA_TIPO_URP.Equals("P"))
                        {
                            switch (operazione)
                            {
                                case "GET": // reperimento dati
                                    query = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_UTENTI_CON_NOTIFICA_P");
                                    query.setParam("id_mod_mitt_dest", mittDest.SYSTEM_ID.ToString());
                                    query.setParam("id_corr_glob", mittDest.ID_CORR_GLOBALI.ToString());
                                    query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                                    commandText = query.getSQL();
                                    logger.Debug("UtentiConNotificaTrasm QUERY : " + commandText);
                                    ds = new DataSet();
                                    dbProvider.ExecuteQuery(out ds, commandText);
                                    if (ds.Tables[0].Rows.Count > 0)
                                    {
                                        foreach (DataRow row in ds.Tables[0].Rows)
                                        {
                                            utNotifica = new DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm();
                                            utNotifica.ID_PEOPLE = row["ID"].ToString();
                                            utNotifica.CODICE_UTENTE = row["CODICE"].ToString();
                                            utNotifica.NOME_COGNOME_UTENTE = row["UTENTE"].ToString();
                                            utNotifica.FLAG_NOTIFICA = row["FLAG_NOTIFICA"].ToString();

                                            mittDest.UTENTI_NOTIFICA.Add(utNotifica);
                                        }
                                    }
                                    break;

                                case "SET": // modifica dei dati
                                    // prima elimina tutti i record sul db
                                    query = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_DEL_UT_CON_NOTIFICA");
                                    query.setParam("id", "ID_MODELLO_MITT_DEST = " + mittDest.SYSTEM_ID.ToString());
                                    commandText = query.getSQL();
                                    logger.Debug("UtentiConNotificaTrasm QUERY : " + commandText);
                                    dbProvider.ExecuteNonQuery(commandText);

                                    // quindi inserisco i record sul db
                                    foreach (DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm ut in mittDest.UTENTI_NOTIFICA)
                                    {
                                        if (ut.FLAG_NOTIFICA.Equals("1"))
                                        {
                                            query = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_INS_UT_CON_NOTIFICA");

                                            query.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                            query.setParam("system_id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MODELLI_MITT_DEST"));
                                            query.setParam("id_mod_mitt_dest", mittDest.SYSTEM_ID.ToString());
                                            query.setParam("id_people", ut.ID_PEOPLE);
                                            query.setParam("id_modello", mittDest.ID_MODELLO.ToString());
                                            commandText = query.getSQL();
                                            logger.Debug("UtentiConNotificaTrasm QUERY : " + commandText);
                                            dbProvider.ExecuteNonQuery(commandText);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objModTrasm = modello;
            }
            finally
            {
                dbProvider.Dispose();
            }
            return objModTrasm;
        }

        #region Find and replace

        /// <summary>
        /// Funzione per la ricerca e la sostituzione di un ruolo con un altro
        /// </summary>
        /// <param name="request">Informazioni sulla richiesta</param>
        /// <returns>Una risposta opportunamente compilata</returns>
        public FindAndReplaceResponse FindAndReplaceRuoliInModelliTrasmissione(
            FindAndReplaceRequest request)
        {
            FindAndReplaceResponse response = new FindAndReplaceResponse();

            // Selezione dell'operazione da compiere
            switch (request.Operation)
            {
                case FindAndReplaceRequest.FindAndReplaceEnum.Find:  // Ricerca
                    // Ricerca di tutti i modelli che soddisfano i criteri di ricerca passati ed estrazione
                    // di una collection con i dati di interesse e verifica della possibilità di effettuare l'operazione
                    // di sostituzione sui modelli individuati
                    response.Models = this.FindModelliTrasmissione(request.SearchFilters, request.UserInfo, request.IsAdministrator);
                    this.AnalyzeModelliTrasmissione(response.Models, request.RoleToReplace);
                    break;
                case FindAndReplaceRequest.FindAndReplaceEnum.Replace:
                    // Applicazione delle azioni richieste e restituzione di un report
                    // con l'esito effettivo dell'azione
                    response.Models = this.ExecuteFindAndReplace(request);
                    break;
                default:
                    break;
            }

            return response;
        }

        /// <summary>
        /// Funzione per la ricerca dei modelli di trasmissione che soddisfano determinati criteri di ricerca
        /// </summary>
        /// <param name="searchFilters">Filtri di ricerca</param>
        /// <param name="userInfo">Informazioni sull'utente</param>
        /// <param name="administrator">True se l'utente è un amministratore</param>
        /// <returns>Collection con le informazioni di base sui modelli di trasmissione individuati dalla ricerca</returns>
        private ModelloTrasmissioneSearchResultCollection FindModelliTrasmissione(FiltroRicerca[] searchFilters, DocsPaVO.utente.InfoUtente userInfo, bool administrator)
        {
            // Collection da restituire
            ModelloTrasmissioneSearchResultCollection retVal = new ModelloTrasmissioneSearchResultCollection();

            using (DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query query = null;

                // Recupero della query da eseguire (se l'utente è un amministratore viene eseguira la S_FIND_MODELLI
                // altrimenti viene eseguita la S_FIND_MODELLI_USER
                if (administrator)
                    query = DocsPaUtils.InitQuery.getInstance().getQuery("S_FIND_MODELLI");
                else
                {
                    query = DocsPaUtils.InitQuery.getInstance().getQuery("S_FIND_MODELLI_USER");
                    query.setParam("corrId", userInfo.idCorrGlobali);
                    query.setParam("idPeople", userInfo.idPeople);
                }

                // Impostazione dell'id dell'amministrazione e dei filtri di ricerca
                query.setParam("ammId", userInfo.idAmministrazione);
                String whereCond = this.GenerateWhereConditionForModels(searchFilters);
                query.setParam("filterCond", whereCond);

                DataSet dataSet = new DataSet();
                // Esecuzione della query ed analisi dei risultati
                if (dbProvider.ExecuteQuery(dataSet, query.getSQL()) && dataSet != null)
                    retVal = this.AnalyzeDataSetAndCreateModelliTrasmissioneCollection(dataSet);
            }

            return retVal;
        }

        /// <summary>
        /// Funzione per l'analisi dei risultati restituiti dalla query e costruzione della collection dei 
        /// </summary>
        /// <param name="dataSet">Dataset con i dati da analizzare</param>
        /// <returns>Collection con le informazioni sui modelli di trasmissione</returns>
        private ModelloTrasmissioneSearchResultCollection AnalyzeDataSetAndCreateModelliTrasmissioneCollection(DataSet dataSet)
        {
            ModelloTrasmissioneSearchResultCollection retVal = new ModelloTrasmissioneSearchResultCollection();

            // Modello in costruzione
            ModelloTrasmissioneSearchResult model = null;

            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                // Se IdModello della riga corrente è diverso da quello del modello in costruzione,
                // vuol dire che si sta analizzando un nuovo modello
                if (model == null || dataRow["IdModello"].ToString() != model.IdModello)
                {
                    model = new ModelloTrasmissioneSearchResult(
                        dataRow["IdModello"].ToString(),
                        dataRow["Nome"].ToString());


                    model.Destinatari.Add(new DestinatarioLite() 
                        {
                            Id = dataRow["IdMittDest"].ToString(),
                            TipoTrasmissione = dataRow["TipoTrasmissione"].ToString(),
                            Corrispondente = new DocsPaVO.utente.Ruolo(
                                dataRow["IdCorrispondente"].ToString(), 
                                String.Empty, 
                                String.Empty, 
                                dataRow["NumeroLivello"].ToString(), 
                                String.Empty, 
                                null, 
                                null)
                        });

                    // Recupero dei  mittenti per il modello
                    this.LoadMittentModelloFindAndReplace(model);
            
                    retVal.Add(model);

                }
                else
                {
                    
                    model.Destinatari.Add(new DestinatarioLite()
                    {
                        Id = dataRow["IdMittDest"].ToString(),
                        TipoTrasmissione = dataRow["TipoTrasmissione"].ToString(),
                        Corrispondente = new DocsPaVO.utente.Ruolo(
                            dataRow["IdCorrispondente"].ToString(),
                            String.Empty,
                            String.Empty,
                            dataRow["NumeroLivello"].ToString(),
                            String.Empty,
                            null,
                            null)
                    });

                }
            }

            // Restituzione collection costruita
            return retVal;

        }

        /// <summary>
        /// Funzione per il caricamento dei mittenti di un modello di trasmissione
        /// </summary>
        /// <param name="modelToUpdate">Modello di trasmissione di cui caricare i mittenti</param>
        private void LoadMittentModelloFindAndReplace(ModelloTrasmissioneSearchResult modelToUpdate)
        {
            using (DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("GET_MITT_MODELLO_FIND_AND_REPLACE");
                query.setParam("modelId", modelToUpdate.IdModello);

                DataSet dataSet = new DataSet();
                if (dbProvider.ExecuteQuery(out dataSet, query.getSQL()))
                    foreach (DataRow mitt in dataSet.Tables[0].Rows)
                        modelToUpdate.Mittenti.Add(new MittenteLite()
                            {
                                Id = mitt["system_id"].ToString(),
                                Livel = mitt["numerolivello"].ToString()
                            });

            }

            
        }

        /// <summary>
        /// Metodo per l'analisi e dei modelli di trasmissione restituiti da una ricerca di tipo find&replace.
        /// </summary>
        /// <param name="modelloTrasmissioneSearchResultCollection">Collection da analizzare</param>
        private void AnalyzeModelliTrasmissione(ModelloTrasmissioneSearchResultCollection modelloTrasmissioneSearchResultCollection, DocsPaVO.rubrica.ElementoRubrica roleToReplace)
        {
            // Il modello potrebbe subire modifiche se il ruolo da sostituire compare fra i destinatari
            foreach (var modello in modelloTrasmissioneSearchResultCollection)
            {
                bool containsCorr = modello.Destinatari.Where(e => e.Corrispondente.systemId == roleToReplace.systemId).Count() > 0;
                modello.Message = String.Format("Il modello {0} modifiche in quanto il ruolo da sostituire{1}compare fra i destinatari.",
                    containsCorr ? "potrebbe subire" : "non subirà", containsCorr ? " " : " non ");

                modello.SyntheticResult = containsCorr ?
                    ModelloTrasmissioneSearchResult.ModelloTrasmissioneSearchResultSynthetic.OK :
                    ModelloTrasmissioneSearchResult.ModelloTrasmissioneSearchResultSynthetic.KO;

            }
             
        }

        /// <summary>
        /// Metodo per l'esecuzione dell'azione di ricerca e sostituzione. A partire dalle informazioni sui modelli trovati,
        /// dalle informazioni sul ruolo da sostituire e dalle informazioni sul ruolo da utilizzare per la sostituzione,
        /// viene eseguito un replace ovunque compaia, nei destinatari, il ruolo da sostituire
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Collection con le informazioni sull'esito dell'operazione</returns>
        private ModelloTrasmissioneSearchResultCollection ExecuteFindAndReplace(FindAndReplaceRequest request)
        {
            // Recupero delle informazioni di dettaglio sul ruolo da sostituire e sul ruolo con cui sostituire
            Ruolo newRole, oldRole;
            
            using (DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti())
            {
                oldRole = ut.getRuoloById(request.RoleToReplace.systemId);
                newRole = ut.getRuoloById(request.NewRole.systemId);
            }

            // Recupero della lista di utenti sotto il ruolo con cui sostituire
            InfoUtente[] users = null;

            using (Amministrazione amm = new Amministrazione())
            {
                users = (InfoUtente[])amm.getListaUtentiInRuolo(newRole, true).ToArray(typeof(InfoUtente));
            }

            // Recupero del livello del ruolo da sostituire e con cui sostituire
            int newRoleLivel, roleToReplaceLivel;
            newRoleLivel = Int32.Parse(newRole.livello);
            roleToReplaceLivel = Int32.Parse(oldRole.livello);

            // Analisi delle righe
            foreach (var model in request.Models)
            {
                // Verifica della possibilità di eseguire l'azione
                String message = String.Empty;
                bool canContinue = this.CanExecuteReplace(model.Destinatari, model.Mittenti, request.NewRole.systemId, newRoleLivel, roleToReplaceLivel, request.RoleToReplace.systemId, model.IdModello, out message);

                if (!canContinue)
                {
                    model.Message = message;
                    model.SyntheticResult = ModelloTrasmissioneSearchResult.ModelloTrasmissioneSearchResultSynthetic.KO;
                }
                else
                {
                    this.ReplaceDestinatariInModelloTrasmissione(model.IdModello, request.RoleToReplace.systemId, request.NewRole.systemId,
                        model.Destinatari.Where(e => e.Corrispondente.systemId == request.RoleToReplace.systemId).First(),
                        users, request.CopyNotes);

                    // Impostazione messaggio di successo
                    model.Message = "Modello di trasmissione modificato con successo";
                    model.SyntheticResult = ModelloTrasmissioneSearchResult.ModelloTrasmissioneSearchResultSynthetic.OK;
                }

            }

            return request.Models;

        }

        /// <summary>
        /// Metodo per la verifica della possibiltà di effettuare la sostituzione in un determinato modello
        /// </summary>
        /// <param name="destinatari">Lista dei destinatari del modello di trasmissione</param>
        /// <param name="senderRoles">Lista dei mittenti del modello di trasmissione</param>
        /// <param name="newRoleCorrId">System id del ruolo con cui sostituire</param>
        /// <param name="newRoleLivel">Posizione del ruolo da utilizzare per la sostituzione all'interno dell'amministrazione</param>
        /// <param name="roleToReplaceLivel">Posizione del ruolo da sostituire all'interno dell'amministrazione</param>
        /// <param name="roleToReplaceCorrId">System id del ruolo da sostituire</param>
        /// <param name="idModello">Id del modello</param>
        /// <param name="message">Dettagli della verifica</param>
        /// <returns>True se la verifica ha avuto esito positivo</returns>
        private bool CanExecuteReplace(List<DestinatarioLite> destinatari, List<MittenteLite> senderRoles, string newRoleCorrId, int newRoleLivel, int roleToReplaceLivel, string roleToReplaceCorrId, String idModello, out string message)
        {
            StringBuilder mex = new StringBuilder();
            bool retVal = true;

            // Se il ruolo non compare nella lista dei destinatari -> ko
            if (destinatari.Where(e => e.Corrispondente.systemId == roleToReplaceCorrId).Count() == 0)
            {
                retVal = false;
                mex.AppendLine("Il ruolo da sostituire non compare nella lista dei destinatari");
            }

            // Se fra i destinatari compare anche il ruolo con cui sostituire -> ko
            if (destinatari.Where(e => e.Corrispondente.systemId == newRoleCorrId).Count() > 0)
            {
                retVal = false;
                mex.AppendLine("Il ruolo da utilizzare per la sostituzione compare nella lista dei destinatari");
            }

            if (retVal)
            {
                // Recupero del destinatario da sostituire
                DestinatarioLite dest = destinatari.Where(e => e.Corrispondente.systemId == roleToReplaceCorrId).First();

                // Se la ragione di trasmissione prevede delle restrizioni che non vengono rispettate dal ruolo con cui sostituire e dal ruolo mittente -> ko
                if (dest.TipoTrasmissione != "T")
                {
                    bool res = true;
                    String details = String.Empty;

                    switch (dest.TipoTrasmissione)
                    {
                        case "S":       // Sottoposti -> il livello del ruolo da utilizzare per la sostituzione deve essere superiore al numero di livello dei ruoli mittente
                            res = senderRoles.Where(e => newRoleLivel < Int32.Parse(e.Livel)).Count() != senderRoles.Count;
                            details = "Il ruolo da utilizzare per la sostituzione deve essere gerarchicamente superiore ai ruoli mittenti";
                            break;

                        case "P":       // Parilivello -> il livello del ruolo da utilizzare per la sostituzione deve essere uguale al numero di livello dei ruoli mittente
                            res = senderRoles.Where(e => newRoleLivel == Int32.Parse(e.Livel)).Count() != senderRoles.Count;
                            details = "Il ruolo da utilizzare per la sostituzione deve essere gerarchicamente un parilivello dei ruoli mittenti";
                            break;

                        case "I":       // Inferiori -> il livello del ruolo da utilzzare per la sostituzione deve essere inferiore al numero di livello dei ruoli mittente
                            res = senderRoles.Where(e => newRoleLivel > Int32.Parse(e.Livel)).Count() != senderRoles.Count;
                            details = "Il ruolo da utilizzare per la sostituzione deve essere gerarchicamente inferiore ai ruoli mittenti";
                            break;

                    }

                    retVal &= !res;
                    mex.AppendFormat(res ? "Violazione delle restrizioni imposte dalla ragione di trasmissione utilizzata. {0}" : String.Empty, details);

                }

            }

            message = mex.ToString();
            return retVal;
            
        }

        /// <summary>
        /// Metodo per la sostituzione del vecchio ruolo con il nuovo
        /// </summary>
        /// <param name="idModello">Id del modello da modificare</param>
        /// <param name="idCorrGlobaleOldRole">Id corr globale del ruolo da sostituire</param>
        /// <param name="idCorrGlobaleNewRole">Id corr globale del ruolo con cui sostituire</param>
        /// <param name="destinatario">Informazioni sulla trasmissione da modificare</param>
        /// <param name="users">Utenti del nuovo ruolo</param>
        /// <param name="copyNotes">True se bisogna copiare anche le note</param>
        private void ReplaceDestinatariInModelloTrasmissione(String idModello, String idCorrGlobaleOldRole, String idCorrGlobaleNewRole, DestinatarioLite destinatario, InfoUtente[] users, bool copyNotes)
        {
            DocsPaUtils.Query query = null;
            using (DBProvider dbProvider = new DBProvider())
            {
                // Eliminazione delle informazioni relative ai destinatari, legati al ruolo da sostituire, da notificare 
                query = DocsPaUtils.InitQuery.getInstance().getQuery("DELETE_DESTINATARI_MODELLO_FIND_AND_REPLACE");
                query.setParam("modelloMittDest", destinatario.Id);
                query.setParam("idModello", idModello);
                dbProvider.ExecuteNonQuery(query.getSQL());

                // Inserimento delle informazioni relative agli utenti legati al ruolo con cui sostituire, da notificare
                foreach (var user in users)
                {
                    query = DocsPaUtils.InitQuery.getInstance().getQuery("INSERT_DESTINATARI_MODELLO_FIND_AND_REPLACE");
                    query.setParam("id", "SEQ.NEXTVAL");
                    query.setParam("idModelloMittDest", destinatario.Id);
                    query.setParam("idPeople", user.idPeople);
                    query.setParam("idModello", idModello);
                    dbProvider.ExecuteNonQuery(query.getSQL());

                }

                // Aggiornamento dell'id del ruolo destinatario della trasmissione
                query = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_MODELLO_FIND_AND_REPLACE");

                query.setParam("idCorrGlobale", idCorrGlobaleNewRole);
                query.setParam("idMittDest", destinatario.Id);
                query.setParam("oldIdCorrGlobali", idCorrGlobaleOldRole);
                query.setParam("copyNote", copyNotes ? String.Empty : ", var_note_sing = ''");
                dbProvider.ExecuteNonQuery(query.getSQL());
            }

        }

       
        
        #endregion

        #region Aggiornamento ruolo mittente / destinatario dei modelli di trasmissione e cancellazione modelli visibili solo al ruolo

        /// <summary>
        /// Metodo per l'aggiornamento dell'associazione fra modello di trasmissione e ruolo
        /// </summary>
        /// <param name="newCorrGlobId">Id corr globali del nuovo ruolo</param>
        /// <param name="oldCorrGlobId">Id corr globali del vecchio ruolo</param>
        /// <returns>Esito dell'operazione</returns>
        public bool UpdateTransmissionModelsAssociation(String newCorrGlobId, String oldCorrGlobId)
        {
            // Valore da restituire
            bool retVal = false;

            using (DBProvider dbProvider = new DBProvider())
            {
                // Recupero della query da eseguire
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_ID_CORR_GLOB_IN_TRANSMISSION_MODELS");

                // Impostazione degli id del ruolo vecchio e di quello nuovo
                query.setParam("newCorrGlobId", newCorrGlobId);
                query.setParam("oldCorrGlobId", oldCorrGlobId);

                // Esecuzione query
                retVal = dbProvider.ExecuteNonQuery(query.getSQL());
            }

            return retVal;
        }

        /// <summary>
        /// Metodo per la rimozione di tutti i modelli di trasmissione visibili solo ad uno specifico ruolo
        /// </summary>
        /// <param name="oldRoleCorrGlobId">Id corr globali del ruolo di cui eliminare i modelli</param>
        /// <returns>True se l'operazione va a buon fine</returns>
        public bool RemoveTransmissionModelsVisibleOnlyToRole(String oldRoleCorrGlobId)
        {
            // Valore da restituire
            bool retVal = true;

            using (DBProvider dbProvider = new DBProvider())
            {
                // Creazione dei parametri per la store
                DocsPaUtils.Data.ParameterSP parameter = new DocsPaUtils.Data.ParameterSP("roleCorrBGlobId", oldRoleCorrGlobId, DocsPaUtils.Data.DirectionParameter.ParamInput);
                ArrayList parameters = new ArrayList();
                parameters.Add(parameter);

                try
                {
                    // Esecuzione della store
                    retVal = dbProvider.ExecuteStoreProcedure("RemoveTransModelVisibileToRole", parameters) == 0;
                }
                catch (Exception e)
                {
                    retVal = false;
                    logger.Debug("Cancellazione  modelli di trasmissione visibili unicamente al ruolo.", e);
                }
            }

            return retVal;

        }

        #endregion

    }
}
