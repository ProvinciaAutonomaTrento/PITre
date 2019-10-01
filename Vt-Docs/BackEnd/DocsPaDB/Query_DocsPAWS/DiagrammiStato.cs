using System;
using System.Collections;
using System.Data;
using System.Threading;
using log4net;
using System.Collections.Generic;
using DocsPaUtils.Data;

namespace DocsPaDB.Query_DocsPAWS
{
	public class DiagrammiStato : DBProvider
	{
        private ILog logger = LogManager.GetLogger(typeof(DiagrammiStato));

		public DiagrammiStato(){}

		public void salvaDiagramma(DocsPaVO.DiagrammaStato.DiagrammaStato dg, string idAmm)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

			try
			{
				//Inserimento nella DPA_DIAGRAMMI_STATO
				dg.DESCRIZIONE = dg.DESCRIZIONE.Replace("'","''");
				string system_id = string.Empty;
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_DPA_DIAGRAMMI_STATO");		
				queryMng.setParam("colID",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
				queryMng.setParam("id",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_DIAGRAMMI_STATO"));
				queryMng.setParam("idAmm",idAmm);
				queryMng.setParam("descrizione",dg.DESCRIZIONE);
				string commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - salvaDiagramma - DiagrammiStato.cs - QUERY : "+commandText);
                logger.Debug("SQL - salvaDiagramma - DiagrammiStato.cs - QUERY : " + commandText);

				bool retValue=false;
				dbProvider.BeginTransaction();
				int rowsAffected;
				if (dbProvider.ExecuteNonQuery(commandText,out rowsAffected))
				{
					retValue=(rowsAffected>0);
                    if (retValue)
                    {
                        // Reperimento systemid
                        commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_DIAGRAMMI_STATO");
                        dbProvider.ExecuteScalar(out system_id, commandText);
                    }
                    else
                    {
                        dbProvider.RollbackTransaction();
                        return;
                    }
				}

				//Inserimento nella DPA_STATI degli stati 
				for(int i=0; i<dg.STATI.Count; i++)
				{
					DocsPaVO.DiagrammaStato.Stato st = (DocsPaVO.DiagrammaStato.Stato) dg.STATI[i];
					st.DESCRIZIONE = st.DESCRIZIONE.Replace("'","''");
					queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_DPA_STATI");		
					queryMng.setParam("colID",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
					queryMng.setParam("id",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_STATI"));
					queryMng.setParam("ID_DIAGRAMMA",system_id);
					queryMng.setParam("Var_descrizione",st.DESCRIZIONE);
                    queryMng.setParam("statoConsolidamento", ((int)st.STATO_CONSOLIDAMENTO).ToString());
                    queryMng.setParam("id_processo_firma", string.IsNullOrEmpty(st.ID_PROCESSO_FIRMA) ? "null" : st.ID_PROCESSO_FIRMA);
                    if (st.STATO_INIZIALE)
					{
						queryMng.setParam("Stato_iniziale","1");
						queryMng.setParam("Stato_finale","0");
					}
					if(st.STATO_FINALE)
					{
						queryMng.setParam("Stato_iniziale","0");
						queryMng.setParam("Stato_finale","1");
					}
					if(!st.STATO_FINALE && !st.STATO_INIZIALE)
					{
						queryMng.setParam("Stato_iniziale","0");
						queryMng.setParam("Stato_finale","0");
					}

                    if (st.CONVERSIONE_PDF)
                        queryMng.setParam("conversionePdf", "1");
                    else
                        queryMng.setParam("conversionePdf", "0");

                    if(st.NON_RICERCABILE)
                        queryMng.setParam("nonRicercabile", "1");
                    else
                        queryMng.setParam("nonRicercabile", "0");
                    
                    if (st.STATO_SISTEMA)
                        queryMng.setParam("statoSistema", "1");
                    else
                        queryMng.setParam("statoSistema", "0");
					commandText=queryMng.getSQL();
					System.Diagnostics.Debug.WriteLine("SQL - salvaDiagramma - DiagrammiStato.cs - QUERY : "+commandText);
                    logger.Debug("SQL - salvaDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
				}

				//Inserimento nella DPA_PASSI dei passi
				for(int i=0; i<dg.PASSI.Count; i++)
				{
					DocsPaVO.DiagrammaStato.Passo step = (DocsPaVO.DiagrammaStato.Passo) dg.PASSI[i];
					int id_stato_padre = 0;
					int id_stato_automatico = 0;
					string desc_stato_automatico = "";

					for(int j=0; j<step.SUCCESSIVI.Count; j++)
					{
						DocsPaVO.DiagrammaStato.Stato st = (DocsPaVO.DiagrammaStato.Stato) step.SUCCESSIVI[j];
						int system_id_st_padre	= 0;
						int system_id_st		= 0;
						step.STATO_PADRE.DESCRIZIONE = step.STATO_PADRE.DESCRIZIONE.Replace("'","''");
						st.DESCRIZIONE = st.DESCRIZIONE.Replace("'","''");

						queryMng =DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_SYSTEM_ID_DPA_STATI");	
						queryMng.setParam("idDiagramma",system_id);
						queryMng.setParam("descrizione",step.STATO_PADRE.DESCRIZIONE);
						commandText=queryMng.getSQL();
						System.Diagnostics.Debug.WriteLine("SQL - getDiagrammaById - DiagrammiStato.cs - QUERY : "+commandText);
                        logger.Debug("SQL - getDiagrammaById - DiagrammiStato.cs - QUERY : " + commandText);
                        DataSet ds = new DataSet();
						dbProvider.ExecuteQuery(ds,commandText);
						system_id_st_padre = Convert.ToInt32(ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString());

						queryMng =DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_SYSTEM_ID_DPA_STATI");	
						queryMng.setParam("idDiagramma",system_id);
						queryMng.setParam("descrizione",st.DESCRIZIONE);
						commandText=queryMng.getSQL();
						System.Diagnostics.Debug.WriteLine("SQL - getDiagrammaById - DiagrammiStato.cs - QUERY : "+commandText);
                        logger.Debug("SQL - getDiagrammaById - DiagrammiStato.cs - QUERY : " + commandText);

						ds = new DataSet();
						dbProvider.ExecuteQuery(ds,commandText);
						system_id_st = Convert.ToInt32(ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
						
						//Verifico se il passo che si vuole inserire ha uno stato automatico
						//in caso affermativo mi salvo il SystemId e la Descrizione dello stato automatico
						//e a fine ciclo effettuo l'inserimento in tabella DPA_PASSI
						if( ((DocsPaVO.DiagrammaStato.Stato) step.SUCCESSIVI[j]).DESCRIZIONE == step.DESCRIZIONE_STATO_AUTOMATICO)
						{
							id_stato_padre = system_id_st_padre;
							id_stato_automatico = system_id_st;
							desc_stato_automatico = st.DESCRIZIONE;
						}	

						queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_DPA_PASSI");		
						queryMng.setParam("colID",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
						queryMng.setParam("id",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_PASSI"));
						queryMng.setParam("id_stato",Convert.ToString(system_id_st_padre));
						queryMng.setParam("next_stato",Convert.ToString(system_id_st));
						queryMng.setParam("id_diagramma",system_id);
                        queryMng.setParam("stato_automatico_lf", system_id_st.ToString().Equals(step.ID_STATO_AUTOMATICO_LF)? "1" : "0");
                        commandText =queryMng.getSQL();
						System.Diagnostics.Debug.WriteLine("SQL - salvaDiagramma - DiagrammiStato.cs - QUERY : "+commandText);
                        logger.Debug("SQL - salvaDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
					}

					//Faccio l'UPDATE per inserire l'eventuale stato automatico
					if( id_stato_padre != 0)
					{
						queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_UPDATE_STATI_AUTOMATICI");		
						queryMng.setParam("id_stato_padre",Convert.ToString(id_stato_padre));
						queryMng.setParam("id_stato_automatico",Convert.ToString(id_stato_automatico));
						queryMng.setParam("desc_stato_automatico",desc_stato_automatico);
						commandText=queryMng.getSQL();
						System.Diagnostics.Debug.WriteLine("SQL - salvaDiagramma - DiagrammiStato.cs - QUERY : "+commandText);
                        logger.Debug("SQL - salvaDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
					}					
				}			
			}
			catch 
			{
				dbProvider.RollbackTransaction();
			}
			finally
			{
				dbProvider.Dispose();
			}				
		}


		public DocsPaVO.DiagrammaStato.DiagrammaStato getDiagrammaById(string idDiagramma)
		{
			DocsPaVO.DiagrammaStato.DiagrammaStato dg =  new DocsPaVO.DiagrammaStato.DiagrammaStato();
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
			
			try
			{
				//Selezione i diagrammi della specifica amministrazione
				DocsPaUtils.Query queryMng =DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_DIAGRAMMI_STATO");	
				queryMng.setParam("system_id",idDiagramma);
				string commandText=queryMng.getSQL();
				logger.Debug("SQL - getDiagrammaById - DiagrammiStato.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
	
				//Seleziono gli stati per lo specifico diagramma
				dg.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
				dg.ID_AMM = Convert.ToInt32(ds.Tables[0].Rows[0]["Id_Amm"].ToString());
				dg.DESCRIZIONE = ds.Tables[0].Rows[0]["Var_descrizione"].ToString();

				queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATI");	
				queryMng.setParam("id_diagramma",Convert.ToString(dg.SYSTEM_ID));
				commandText=queryMng.getSQL();
				logger.Debug("SQL - getDiagrammaById - DiagrammiStato.cs - QUERY : " + commandText);
                DataSet ds_1 = new DataSet();
				dbProvider.ExecuteQuery(ds_1,commandText);
				
				for(int j=0; j<ds_1.Tables[0].Rows.Count; j++)
				{
					DocsPaVO.DiagrammaStato.Stato st = new DocsPaVO.DiagrammaStato.Stato();
					st.SYSTEM_ID = Convert.ToInt32(ds_1.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
					st.ID_DIAGRAMMA = Convert.ToInt32(ds_1.Tables[0].Rows[j]["ID_DIAGRAMMA"].ToString());
					st.DESCRIZIONE = ds_1.Tables[0].Rows[j]["Var_descrizione"].ToString();
					if(ds_1.Tables[0].Rows[j]["Stato_iniziale"].ToString() == "0")
						st.STATO_INIZIALE = false;
					else
						st.STATO_INIZIALE = true;

					if(ds_1.Tables[0].Rows[j]["Stato_finale"].ToString() == "0")
						st.STATO_FINALE = false;
					else
						st.STATO_FINALE = true;

                    if (ds_1.Tables[0].Rows[j]["Conv_Pdf"].ToString() == "1")
                        st.CONVERSIONE_PDF = true;
                    else
                        st.CONVERSIONE_PDF = false;

                    if (ds_1.Tables[0].Rows[j]["Stato_consolidamento"] != DBNull.Value)
                    {
                        st.STATO_CONSOLIDAMENTO = (DocsPaVO.documento.DocumentConsolidationStateEnum)
                            Enum.Parse(typeof(DocsPaVO.documento.DocumentConsolidationStateEnum),
                                    ds_1.Tables[0].Rows[j]["Stato_consolidamento"].ToString(), true);
                    }

                    if (ds_1.Tables[0].Rows[j]["NON_RICERCABILE"].ToString() == "1")
                        st.NON_RICERCABILE = true;
                    else
                        st.NON_RICERCABILE = false;

                    if (ds_1.Tables[0].Rows[j]["ID_PROCESSO_FIRMA"] != DBNull.Value)
                    {
                        st.ID_PROCESSO_FIRMA = ds_1.Tables[0].Rows[j]["ID_PROCESSO_FIRMA"].ToString();
                    }

                    dg.STATI.Add(st);
				}

				//Seleziono i passi per lo specifico stato 
				for(int k=0; k<dg.STATI.Count; k++)
				{
					DocsPaVO.DiagrammaStato.Stato st = (DocsPaVO.DiagrammaStato.Stato) dg.STATI[k];
					
					queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_PASSI");	
					queryMng.setParam("id_stato",Convert.ToString(st.SYSTEM_ID));
					commandText=queryMng.getSQL();
					logger.Debug("SQL - getDiagrammaById - DiagrammiStato.cs - QUERY : " + commandText);

					DataSet ds_2 = new DataSet();
					dbProvider.ExecuteQuery(ds_2,commandText);

                    DocsPaVO.DiagrammaStato.Passo step = new DocsPaVO.DiagrammaStato.Passo();
                    if (ds_2.Tables[0].Rows.Count != 0)
                    {
                        //Proprietà del passo
                        step.ID_DIAGRAMMA = Convert.ToInt32(ds_2.Tables[0].Rows[0]["ID_DIAGRAMMA_PASSO"].ToString());
                        step.ID_STATO_AUTOMATICO = ds_2.Tables[0].Rows[0]["ID_STATO_AUTO_PASSO"].ToString();
                        step.DESCRIZIONE_STATO_AUTOMATICO = ds_2.Tables[0].Rows[0]["DESC_STATO_AUTO_PASSO"].ToString();

                        //Stato Padre del passo
                        DocsPaVO.DiagrammaStato.Stato st_1 = new DocsPaVO.DiagrammaStato.Stato();
                        st_1.SYSTEM_ID = Convert.ToInt32(ds_2.Tables[0].Rows[0]["ID_STATO_PADRE"].ToString());
                        st_1.ID_DIAGRAMMA = Convert.ToInt32(ds_2.Tables[0].Rows[0]["ID_DIAGRAMMA_STATO_PADRE"].ToString());
                        st_1.DESCRIZIONE = ds_2.Tables[0].Rows[0]["VAR_DESCRIZIONE_STATO_PADRE"].ToString();
                        if (ds_2.Tables[0].Rows[0]["STATO_INIZIALE_STATO_PADRE"].ToString() == "0")
                            st_1.STATO_INIZIALE = false;
                        else
                            st_1.STATO_INIZIALE = true;

                        if (ds_2.Tables[0].Rows[0]["STATO_FINALE_STATO_PADRE"].ToString() == "0")
                            st_1.STATO_FINALE = false;
                        else
                            st_1.STATO_FINALE = true;

                        if (ds_2.Tables[0].Rows[0]["CONV_PDF_STATO_PADRE"].ToString() == "1")
                            st_1.CONVERSIONE_PDF = true;
                        else
                            st_1.CONVERSIONE_PDF = false;

                        step.STATO_PADRE = st_1;

                        //Stati successivi del passo
                        for (int y = 0; y < ds_2.Tables[0].Rows.Count; y++)
                        {
                            DocsPaVO.DiagrammaStato.Stato st_2 = new DocsPaVO.DiagrammaStato.Stato();
                            st_2.SYSTEM_ID = Convert.ToInt32(ds_2.Tables[0].Rows[y]["ID_STATO_SUCC"].ToString());
                            st_2.ID_DIAGRAMMA = Convert.ToInt32(ds_2.Tables[0].Rows[y]["ID_DIAGRAMMA_STATO_SUCC"].ToString());
                            st_2.DESCRIZIONE = ds_2.Tables[0].Rows[y]["VAR_DESCRIZIONE_STATO_SUCC"].ToString();
                            if (ds_2.Tables[0].Rows[y]["STATO_INIZIALE_STATO_SUCC"].ToString() == "0")
                                st_2.STATO_INIZIALE = false;
                            else
                                st_2.STATO_INIZIALE = true;

                            if (ds_2.Tables[0].Rows[y]["STATO_FINALE_STATO_SUCC"].ToString() == "0")
                                st_2.STATO_FINALE = false;
                            else
                                st_2.STATO_FINALE = true;

                            if (ds_2.Tables[0].Rows[y]["CONV_PDF_STATO_SUCC"].ToString() == "1")
                                st_2.CONVERSIONE_PDF = true;
                            else
                                st_2.CONVERSIONE_PDF = false;

                            if (ds_2.Tables[0].Rows[y]["CHA_STATO_AUTOMATICO_LF"].ToString() == "1")
                                step.ID_STATO_AUTOMATICO_LF = st_2.SYSTEM_ID.ToString();

                            step.SUCCESSIVI.Add(st_2);
                        }
                    }

                    #region VECCHIA GESTIONE CON QUERY MULTIPLE
                    /*
					DocsPaVO.DiagrammaStato.Passo step =new DocsPaVO.DiagrammaStato.Passo();
					for(int y=0; y<ds_2.Tables[0].Rows.Count; y++)
					{
						step.ID_DIAGRAMMA = Convert.ToInt32(ds_2.Tables[0].Rows[y]["Id_diagramma"].ToString());
						step.ID_STATO_AUTOMATICO = ds_2.Tables[0].Rows[y]["ID_STATO_AUTO"].ToString();
						step.DESCRIZIONE_STATO_AUTOMATICO = ds_2.Tables[0].Rows[y]["DESC_STATO_AUTO"].ToString();

                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");	
						queryMng.setParam("system_id",ds_2.Tables[0].Rows[y]["ID_STATO"].ToString());
						commandText=queryMng.getSQL();
						System.Diagnostics.Debug.WriteLine("SQL - getDiagrammaById - DiagrammiStato.cs - QUERY : "+commandText);
                        logger.Debug("SQL - getDiagrammaById - DiagrammiStato.cs - QUERY : " + commandText);

						DataSet ds_3 = new DataSet();
						dbProvider.ExecuteQuery(ds_3,commandText);

						DocsPaVO.DiagrammaStato.Stato st_1 = new DocsPaVO.DiagrammaStato.Stato();
						st_1.SYSTEM_ID = Convert.ToInt32(ds_3.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
						st_1.ID_DIAGRAMMA = Convert.ToInt32(ds_3.Tables[0].Rows[0]["ID_DIAGRAMMA"].ToString());
						st_1.DESCRIZIONE = ds_3.Tables[0].Rows[0]["Var_descrizione"].ToString();
						if(ds_3.Tables[0].Rows[0]["Stato_iniziale"].ToString() == "0")
							st_1.STATO_INIZIALE = false;
						else
							st_1.STATO_INIZIALE = true;

						if(ds_3.Tables[0].Rows[0]["Stato_finale"].ToString() == "0")
							st_1.STATO_FINALE = false;
						else
							st_1.STATO_FINALE = true;

                        if (ds_3.Tables[0].Rows[0]["Conv_Pdf"].ToString() == "1")
                            st_1.CONVERSIONE_PDF = true;
                        else
                            st_1.CONVERSIONE_PDF = false;

						step.STATO_PADRE = st_1;

                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");	
						queryMng.setParam("system_id",ds_2.Tables[0].Rows[y]["Id_next_stato"].ToString());
						commandText=queryMng.getSQL();
						System.Diagnostics.Debug.WriteLine("SQL - getDiagrammaById - DiagrammiStato.cs - QUERY : "+commandText);
                        logger.Debug("SQL - getDiagrammaById - DiagrammiStato.cs - QUERY : " + commandText);

						DataSet ds_4 = new DataSet();
						dbProvider.ExecuteQuery(ds_4,commandText);

						DocsPaVO.DiagrammaStato.Stato st_2 = new DocsPaVO.DiagrammaStato.Stato();
						st_2.SYSTEM_ID = Convert.ToInt32(ds_4.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
						st_2.ID_DIAGRAMMA = Convert.ToInt32(ds_4.Tables[0].Rows[0]["ID_DIAGRAMMA"].ToString());
						st_2.DESCRIZIONE = ds_4.Tables[0].Rows[0]["Var_descrizione"].ToString();
						if(ds_4.Tables[0].Rows[0]["Stato_iniziale"].ToString() == "0")
							st_2.STATO_INIZIALE = false;
						else
							st_2.STATO_INIZIALE = true;

						if(ds_4.Tables[0].Rows[0]["Stato_finale"].ToString() == "0")
							st_2.STATO_FINALE = false;
						else
							st_2.STATO_FINALE = true;

                        if (ds_4.Tables[0].Rows[0]["Conv_Pdf"].ToString() == "1")
                            st_2.CONVERSIONE_PDF = true;
                        else
                            st_2.CONVERSIONE_PDF = false;
							
						step.SUCCESSIVI.Add(st_2);		
					
					}
                    */
                    #endregion VECCHIA GESTIONE CON QUERY MULTIPLE

                    if (step.STATO_PADRE != null)
						dg.PASSI.Add(step);
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
			return dg;
		}


		public ArrayList getDiagrammi(string idAmm)
		{
			ArrayList diagrammi = new ArrayList();
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();	

			try
			{
				//Selezione i diagrammi della specifica amministrazione
				DocsPaUtils.Query queryMng =DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_DIAGRAMMI_BY_ID_AMM");	
				queryMng.setParam("idAmm",idAmm);
				string commandText=queryMng.getSQL();
				logger.Debug("SQL - getDiagrammi - DiagrammiStato.cs - QUERY : " + commandText);

				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
	
				for(int i=0; i<ds.Tables[0].Rows.Count; i++)
				{
					//Seleziono gli stati per lo specifico diagramma
					DocsPaVO.DiagrammaStato.DiagrammaStato dg =  new DocsPaVO.DiagrammaStato.DiagrammaStato();

					dg.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
					dg.ID_AMM = Convert.ToInt32(ds.Tables[0].Rows[i]["Id_amm"].ToString());
					dg.DESCRIZIONE = ds.Tables[0].Rows[i]["Var_descrizione"].ToString();

					queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATI");	
					queryMng.setParam("id_diagramma",Convert.ToString(dg.SYSTEM_ID));
					commandText=queryMng.getSQL();
					logger.Debug("SQL - getDiagrammi - DiagrammiStato.cs - QUERY : " + commandText);

					DataSet ds_1 = new DataSet();
					dbProvider.ExecuteQuery(ds_1,commandText);

					for(int j=0; j<ds_1.Tables[0].Rows.Count; j++)
					{
						DocsPaVO.DiagrammaStato.Stato st = new DocsPaVO.DiagrammaStato.Stato();
						st.SYSTEM_ID = Convert.ToInt32(ds_1.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
						st.ID_DIAGRAMMA = Convert.ToInt32(ds_1.Tables[0].Rows[j]["ID_DIAGRAMMA"].ToString());
						st.DESCRIZIONE = ds_1.Tables[0].Rows[j]["Var_descrizione"].ToString();
						if(ds_1.Tables[0].Rows[j]["Stato_iniziale"].ToString() == "0")
							st.STATO_INIZIALE = false;
						else
							st.STATO_INIZIALE = true;

						if(ds_1.Tables[0].Rows[j]["Stato_finale"].ToString() == "0")
							st.STATO_FINALE = false;
						else
							st.STATO_FINALE = true;

                        if (ds_1.Tables[0].Rows[j]["Conv_Pdf"].ToString() == "1")
                            st.CONVERSIONE_PDF = true;
                        else
                            st.CONVERSIONE_PDF = false;

                        if (ds_1.Tables[0].Rows[j]["Stato_consolidamento"] != DBNull.Value)
                        {
                            st.STATO_CONSOLIDAMENTO = (DocsPaVO.documento.DocumentConsolidationStateEnum)
                                    Enum.Parse(typeof(DocsPaVO.documento.DocumentConsolidationStateEnum),
                                            ds_1.Tables[0].Rows[j]["Stato_consolidamento"].ToString(), true);
                        }

                        if (ds_1.Tables[0].Rows[j]["NON_RICERCABILE"].ToString() == "1")
                            st.NON_RICERCABILE = true;
                        else
                            st.NON_RICERCABILE = false;
                        if (!string.IsNullOrEmpty(ds_1.Tables[0].Rows[j]["CHA_STATO_SISTEMA"].ToString()) && ds_1.Tables[0].Rows[j]["CHA_STATO_SISTEMA"].ToString().Equals("1"))
                            st.STATO_SISTEMA = true;
                        else
                            st.STATO_SISTEMA = false;
                        if (ds_1.Tables[0].Rows[j]["ID_PROCESSO_FIRMA"] != DBNull.Value)
                        {
                            st.ID_PROCESSO_FIRMA = ds_1.Tables[0].Rows[j]["ID_PROCESSO_FIRMA"].ToString();
                        }
                        dg.STATI.Add(st);
					}

					//Seleziono i passi per lo specifico stato 
					for(int k=0; k<dg.STATI.Count; k++)
					{
						DocsPaVO.DiagrammaStato.Stato st = (DocsPaVO.DiagrammaStato.Stato) dg.STATI[k];
						queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_PASSI");	
						queryMng.setParam("id_stato",Convert.ToString(st.SYSTEM_ID));
						commandText=queryMng.getSQL();
						logger.Debug("SQL - getDgByIdTipoDoc - DiagrammiStato.cs - QUERY : " + commandText);

						DataSet ds_2 = new DataSet();
						dbProvider.ExecuteQuery(ds_2,commandText);

                        DocsPaVO.DiagrammaStato.Passo step = new DocsPaVO.DiagrammaStato.Passo();
                        if (ds_2.Tables[0].Rows.Count != 0)
                        {
                            //Proprietà del passo
                            step.ID_DIAGRAMMA = Convert.ToInt32(ds_2.Tables[0].Rows[0]["ID_DIAGRAMMA_PASSO"].ToString());
                            step.ID_STATO_AUTOMATICO = ds_2.Tables[0].Rows[0]["ID_STATO_AUTO_PASSO"].ToString();
                            step.DESCRIZIONE_STATO_AUTOMATICO = ds_2.Tables[0].Rows[0]["DESC_STATO_AUTO_PASSO"].ToString();

                            //Stato Padre del passo
                            DocsPaVO.DiagrammaStato.Stato st_1 = new DocsPaVO.DiagrammaStato.Stato();
                            st_1.SYSTEM_ID = Convert.ToInt32(ds_2.Tables[0].Rows[0]["ID_STATO_PADRE"].ToString());
                            st_1.ID_DIAGRAMMA = Convert.ToInt32(ds_2.Tables[0].Rows[0]["ID_DIAGRAMMA_STATO_PADRE"].ToString());
                            st_1.DESCRIZIONE = ds_2.Tables[0].Rows[0]["VAR_DESCRIZIONE_STATO_PADRE"].ToString();
                            if (ds_2.Tables[0].Rows[0]["STATO_INIZIALE_STATO_PADRE"].ToString() == "0")
                                st_1.STATO_INIZIALE = false;
                            else
                                st_1.STATO_INIZIALE = true;

                            if (ds_2.Tables[0].Rows[0]["STATO_FINALE_STATO_PADRE"].ToString() == "0")
                                st_1.STATO_FINALE = false;
                            else
                                st_1.STATO_FINALE = true;

                            if (ds_2.Tables[0].Rows[0]["CONV_PDF_STATO_PADRE"].ToString() == "1")
                                st_1.CONVERSIONE_PDF = true;
                            else
                                st_1.CONVERSIONE_PDF = false;
                            if (ds_2.Tables[0].Rows[0]["ID_PROCESSO_FIRMA"] != DBNull.Value)
                            {
                                st_1.ID_PROCESSO_FIRMA = ds_2.Tables[0].Rows[0]["ID_PROCESSO_FIRMA"].ToString();
                            }
                            step.STATO_PADRE = st_1;

                            //Stati successivi del passo
                            for (int y = 0; y < ds_2.Tables[0].Rows.Count; y++)
                            {
                                DocsPaVO.DiagrammaStato.Stato st_2 = new DocsPaVO.DiagrammaStato.Stato();
                                st_2.SYSTEM_ID = Convert.ToInt32(ds_2.Tables[0].Rows[y]["ID_STATO_SUCC"].ToString());
                                st_2.ID_DIAGRAMMA = Convert.ToInt32(ds_2.Tables[0].Rows[y]["ID_DIAGRAMMA_STATO_SUCC"].ToString());
                                st_2.DESCRIZIONE = ds_2.Tables[0].Rows[y]["VAR_DESCRIZIONE_STATO_SUCC"].ToString();
                                if (ds_2.Tables[0].Rows[y]["STATO_INIZIALE_STATO_SUCC"].ToString() == "0")
                                    st_2.STATO_INIZIALE = false;
                                else
                                    st_2.STATO_INIZIALE = true;

                                if (ds_2.Tables[0].Rows[y]["STATO_FINALE_STATO_SUCC"].ToString() == "0")
                                    st_2.STATO_FINALE = false;
                                else
                                    st_2.STATO_FINALE = true;

                                if (ds_2.Tables[0].Rows[y]["CONV_PDF_STATO_SUCC"].ToString() == "1")
                                    st_2.CONVERSIONE_PDF = true;
                                else
                                    st_2.CONVERSIONE_PDF = false;

                                if (ds_2.Tables[0].Rows[y]["CHA_STATO_AUTOMATICO_LF"].ToString() == "1")
                                    step.ID_STATO_AUTOMATICO_LF = st_2.SYSTEM_ID.ToString();

                                step.SUCCESSIVI.Add(st_2);
                            }
                        }

                        #region VECCHIA GESTIONE CON QUERY MULTIPLE
                        /*
						DocsPaVO.DiagrammaStato.Passo step =new DocsPaVO.DiagrammaStato.Passo();
						for(int y=0; y<ds_2.Tables[0].Rows.Count; y++)
						{
							step.ID_DIAGRAMMA = Convert.ToInt32(ds_2.Tables[0].Rows[y]["Id_diagramma"].ToString());
							step.ID_STATO_AUTOMATICO = ds_2.Tables[0].Rows[y]["ID_STATO_AUTO"].ToString();
							step.DESCRIZIONE_STATO_AUTOMATICO = ds_2.Tables[0].Rows[y]["DESC_STATO_AUTO"].ToString();

                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");	
							queryMng.setParam("system_id",ds_2.Tables[0].Rows[y]["ID_STATO"].ToString());
							commandText=queryMng.getSQL();
							System.Diagnostics.Debug.WriteLine("SQL - getDgByIdTipoDoc - DiagrammiStato.cs - QUERY : "+commandText);
                            logger.Debug("SQL - getDgByIdTipoDoc - DiagrammiStato.cs - QUERY : " + commandText);

							DataSet ds_3 = new DataSet();
							dbProvider.ExecuteQuery(ds_3,commandText);

							DocsPaVO.DiagrammaStato.Stato st_1 = new DocsPaVO.DiagrammaStato.Stato();
							st_1.SYSTEM_ID = Convert.ToInt32(ds_3.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
							st_1.ID_DIAGRAMMA = Convert.ToInt32(ds_3.Tables[0].Rows[0]["ID_DIAGRAMMA"].ToString());
							st_1.DESCRIZIONE = ds_3.Tables[0].Rows[0]["Var_descrizione"].ToString();
							if(ds_3.Tables[0].Rows[0]["Stato_iniziale"].ToString() == "0")
								st_1.STATO_INIZIALE = false;
							else
								st_1.STATO_INIZIALE = true;

							if(ds_3.Tables[0].Rows[0]["Stato_finale"].ToString() == "0")
								st_1.STATO_FINALE = false;
							else
								st_1.STATO_FINALE = true;

                            if (ds_3.Tables[0].Rows[0]["Conv_Pdf"].ToString() == "1")
                                st_1.CONVERSIONE_PDF = true;
                            else
                                st_1.CONVERSIONE_PDF = false;

							step.STATO_PADRE = st_1;

                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");	
							queryMng.setParam("system_id",ds_2.Tables[0].Rows[y]["Id_next_stato"].ToString());
							commandText=queryMng.getSQL();
							System.Diagnostics.Debug.WriteLine("SQL - getDgByIdTipoDoc - DiagrammiStato.cs - QUERY : "+commandText);
                            logger.Debug("SQL - getDgByIdTipoDoc - DiagrammiStato.cs - QUERY : " + commandText);

							DataSet ds_4 = new DataSet();
							dbProvider.ExecuteQuery(ds_4,commandText);

							DocsPaVO.DiagrammaStato.Stato st_2 = new DocsPaVO.DiagrammaStato.Stato();
							st_2.SYSTEM_ID = Convert.ToInt32(ds_4.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
							st_2.ID_DIAGRAMMA = Convert.ToInt32(ds_4.Tables[0].Rows[0]["ID_DIAGRAMMA"].ToString());
							st_2.DESCRIZIONE = ds_4.Tables[0].Rows[0]["Var_descrizione"].ToString();
							if(ds_4.Tables[0].Rows[0]["Stato_iniziale"].ToString() == "0")
								st_2.STATO_INIZIALE = false;
							else
								st_2.STATO_INIZIALE = true;

							if(ds_4.Tables[0].Rows[0]["Stato_finale"].ToString() == "0")
								st_2.STATO_FINALE = false;
							else
								st_2.STATO_FINALE = true;

                            if (ds_4.Tables[0].Rows[0]["Conv_Pdf"].ToString() == "1")
                                st_2.CONVERSIONE_PDF = true;
                            else
                                st_2.CONVERSIONE_PDF = false;
								
							step.SUCCESSIVI.Add(st_2);	
						}
                        */
                        #endregion VECCHIA GESTIONE CON QUERY MULTIPLE

                        if (step.STATO_PADRE != null)
							dg.PASSI.Add(step);
					}
					diagrammi.Add(dg);
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
			return diagrammi;
		}


		public bool isUniqueNameDiagramma(string nomeDiagramma)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
			
			try
			{
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_IS_UNIQUE_NOME_DIAGRAMMA");	
				queryMng.setParam("nomeDiagramma",nomeDiagramma);
				string commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - isUniqueNameDiagramma - DiagrammiStato.cs - QUERY : "+commandText);
                logger.Debug("SQL - isUniqueNameDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				
				if(ds.Tables[0].Rows.Count != 0)
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
			}						
		}


		public void delDiagramma(DocsPaVO.DiagrammaStato.DiagrammaStato dg)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

			try
			{
				DocsPaUtils.Query queryMng;
				string commandText;
								
				for(int i=0; i<dg.STATI.Count; i++)
				{
					DocsPaVO.DiagrammaStato.Stato st = (DocsPaVO.DiagrammaStato.Stato) dg.STATI[i];
					queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_DELETE_DPA_PASSI");		
					queryMng.setParam("id_passo",Convert.ToString(st.SYSTEM_ID));
					commandText=queryMng.getSQL();
					System.Diagnostics.Debug.WriteLine("SQL - delDiagramma - DiagrammiStato.cs - QUERY : "+commandText);
                    logger.Debug("SQL - delDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
					
					queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_DELETE_DPA_STATI");		
					queryMng.setParam("id_stato",Convert.ToString(st.SYSTEM_ID));
					commandText=queryMng.getSQL();
					System.Diagnostics.Debug.WriteLine("SQL - delDiagramma - DiagrammiStato.cs - QUERY : "+commandText);
                    logger.Debug("SQL - delDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
					
					queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_DELETE_DPA_DIAGRAMMI_DOC");		
					queryMng.setParam("id_stato",Convert.ToString(st.SYSTEM_ID));
					commandText=queryMng.getSQL();
					System.Diagnostics.Debug.WriteLine("SQL - delDiagramma - DiagrammiStato.cs - QUERY : "+commandText);
					dbProvider.ExecuteNonQuery(commandText);
				}

				queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_DELETE_DPA_DIAGRAMMI_STATO");		
				queryMng.setParam("system_id",Convert.ToString(dg.SYSTEM_ID));
				commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - delDiagramma - DiagrammiStato.cs - QUERY : "+commandText);
                logger.Debug("SQL - delDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);				
			}
			catch 
			{
				dbProvider.RollbackTransaction();
			}
			finally
			{
				dbProvider.Dispose();
			}			
		}


		public void updateDiagramma(DocsPaVO.DiagrammaStato.DiagrammaStato dg)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

			try
			{
				//Aggiorno il nome del Diagramma
				dg.DESCRIZIONE = dg.DESCRIZIONE.Replace("'","''");
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_UPDATE_DPA_DIAGRAMMI_STATO");		
				queryMng.setParam("descrizione",dg.DESCRIZIONE);
				queryMng.setParam("system_id",Convert.ToString(dg.SYSTEM_ID));
				string commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - updateDiagramma - ProfilazioneDinamica/Database/model.cs - QUERY : "+commandText);
                logger.Debug("SQL - updateDiagramma - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
				dbProvider.ExecuteNonQuery(commandText);

				//Aggiorno gli stati del diagramma, faccio un UPDATE se esistono già
				//altrimenti effettuo un inserimento
				for(int i=0; i<dg.STATI.Count; i++)
                {
					DocsPaVO.DiagrammaStato.Stato st = (DocsPaVO.DiagrammaStato.Stato) dg.STATI[i];
					st.DESCRIZIONE = st.DESCRIZIONE.Replace("'","''");
					queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_SYSTEM_ID_DPA_STATI_1");		
					queryMng.setParam("system_id",Convert.ToString(st.SYSTEM_ID));
					commandText=queryMng.getSQL();
					System.Diagnostics.Debug.WriteLine("SQL - updateDiagramma - ProfilazioneDinamica/Database/model.cs - QUERY : "+commandText);
                    logger.Debug("SQL - updateDiagramma - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    DataSet ds = new DataSet();
					dbProvider.ExecuteQuery(ds,commandText);

					//Faccio un UPDATE
					if(ds.Tables[0].Rows.Count != 0)
					{
						queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_UPDATE_DPA_STATI");		
						queryMng.setParam("descrizione",st.DESCRIZIONE);
						queryMng.setParam("system_id",Convert.ToString(st.SYSTEM_ID));
						
						if(st.STATO_INIZIALE)
						{
							queryMng.setParam("stato_iniziale","1");
							queryMng.setParam("stato_finale","0");							
						}

						if(st.STATO_FINALE)
						{
							queryMng.setParam("stato_iniziale","0");
							queryMng.setParam("stato_finale","1");							
						}	

						if(!st.STATO_FINALE && !st.STATO_INIZIALE)
						{
							queryMng.setParam("stato_iniziale","0");
							queryMng.setParam("stato_finale","0");							
						}

                        if (st.CONVERSIONE_PDF)
                            queryMng.setParam("conversionePdf", "1");
                        else
                            queryMng.setParam("conversionePdf", "0");

                        if (st.NON_RICERCABILE)
                            queryMng.setParam("nonRicercabile", "1");
                        else
                            queryMng.setParam("nonRicercabile", "0");

                        queryMng.setParam("statoConsolidamento", ((int)st.STATO_CONSOLIDAMENTO).ToString());
                        if(st.STATO_SISTEMA)
                            queryMng.setParam("statoSistema", "1");
                        else
                            queryMng.setParam("statoSistema", "0");
                        queryMng.setParam("id_processo_firma", string.IsNullOrEmpty(st.ID_PROCESSO_FIRMA) ? "null" : st.ID_PROCESSO_FIRMA);
                        commandText = queryMng.getSQL();
						System.Diagnostics.Debug.WriteLine("SQL - updateDiagramma - ProfilazioneDinamica/Database/model.cs - QUERY : "+commandText);
                        logger.Debug("SQL - updateDiagramma - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
					}
					//Faccio un INSERIMENTO
					else
					{
						queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_DPA_STATI");		
						queryMng.setParam("colID",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
						queryMng.setParam("id",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_STATI"));
						queryMng.setParam("ID_DIAGRAMMA",Convert.ToString(dg.SYSTEM_ID));
						queryMng.setParam("Var_descrizione",st.DESCRIZIONE);
                        queryMng.setParam("id_processo_firma", string.IsNullOrEmpty(st.ID_PROCESSO_FIRMA) ? "null" : st.ID_PROCESSO_FIRMA);
                        if (st.STATO_INIZIALE)
						{
							queryMng.setParam("Stato_iniziale","1");
							queryMng.setParam("Stato_finale","0");
						}
						if(st.STATO_FINALE)
						{
							queryMng.setParam("Stato_iniziale","0");
							queryMng.setParam("Stato_finale","1");
						}
						if(!st.STATO_FINALE && !st.STATO_INIZIALE)
						{
							queryMng.setParam("Stato_iniziale","0");
							queryMng.setParam("Stato_finale","0");
						}

                        if (st.CONVERSIONE_PDF)
                            queryMng.setParam("conversionePdf", "1");
                        else
                            queryMng.setParam("conversionePdf", "0");

                        if (st.NON_RICERCABILE)
                            queryMng.setParam("nonRicercabile", "1");
                        else
                            queryMng.setParam("nonRicercabile", "0");

                        queryMng.setParam("statoConsolidamento", ((int)st.STATO_CONSOLIDAMENTO).ToString());
                        if (st.STATO_SISTEMA)
                            queryMng.setParam("statoSistema", "1");
                        else
                            queryMng.setParam("statoSistema", "0");
						commandText=queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - updateDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                        logger.Debug("SQL - updateDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
					}
				}

				//In questo caso non distinguo se fare un UPDATE o un INSERIMENTO
				//cancello e reinserisco i passi legati allo specifico diagramma
				queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_DELETE_DPA_PASSI_1");		
				queryMng.setParam("idDiagramma",Convert.ToString(dg.SYSTEM_ID));
				commandText=queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - updateDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - updateDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
				
				for(int j=0; j<dg.PASSI.Count; j++)
				{
					DocsPaVO.DiagrammaStato.Passo step = (DocsPaVO.DiagrammaStato.Passo) dg.PASSI[j];
					step.STATO_PADRE.DESCRIZIONE = step.STATO_PADRE.DESCRIZIONE.Replace("'","''");
					int id_stato_padre = 0;
					int id_stato_automatico = 0;
					string desc_stato_automatico = "";

					for(int k=0; k<step.SUCCESSIVI.Count; k++)
					{
						DocsPaVO.DiagrammaStato.Stato st = (DocsPaVO.DiagrammaStato.Stato) step.SUCCESSIVI[k];
						st.DESCRIZIONE = st.DESCRIZIONE.Replace("'","''");

						int system_id_st_padre	= 0;
						int system_id_st		= 0;

						queryMng =DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_SYSTEM_ID_DPA_STATI");	
						queryMng.setParam("idDiagramma",Convert.ToString(dg.SYSTEM_ID));
						queryMng.setParam("descrizione",step.STATO_PADRE.DESCRIZIONE);
						commandText=queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - updateDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                        logger.Debug("SQL - updateDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                        DataSet ds = new DataSet();
						dbProvider.ExecuteQuery(ds,commandText);
						system_id_st_padre = Convert.ToInt32(ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString());

						queryMng =DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_SYSTEM_ID_DPA_STATI");	
						queryMng.setParam("idDiagramma",Convert.ToString(dg.SYSTEM_ID));
						queryMng.setParam("descrizione",st.DESCRIZIONE);
						commandText=queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - updateDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                        logger.Debug("SQL - updateDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                        ds = new DataSet();
						dbProvider.ExecuteQuery(ds,commandText);
						system_id_st = Convert.ToInt32(ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString());

						//Verifico se il passo che si vuole inserire ha uno stato automatico
						//in caso affermativo mi salvo il SystemId e la Descrizione dello stato automatico
						//e a fine ciclo effettuo l'inserimento in tabella DPA_PASSI
						if( ((DocsPaVO.DiagrammaStato.Stato) step.SUCCESSIVI[k]).DESCRIZIONE == step.DESCRIZIONE_STATO_AUTOMATICO)
						{
							id_stato_padre = system_id_st_padre;
							id_stato_automatico = system_id_st;
							desc_stato_automatico = st.DESCRIZIONE;
						}	

						queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_DPA_PASSI");		
						queryMng.setParam("colID",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
						queryMng.setParam("id",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_PASSI"));
						queryMng.setParam("id_stato",Convert.ToString(system_id_st_padre));
						queryMng.setParam("next_stato",Convert.ToString(system_id_st));
						queryMng.setParam("id_diagramma",Convert.ToString(dg.SYSTEM_ID));
                        queryMng.setParam("stato_automatico_lf", system_id_st.ToString().Equals(step.ID_STATO_AUTOMATICO_LF) ? "1" : "0");
                        commandText =queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - updateDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                        logger.Debug("SQL - updateDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
					}

					//Faccio l'UPDATE per inserire l'eventuale stato automatico
					if( id_stato_padre != 0)
					{
						queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_UPDATE_STATI_AUTOMATICI");		
						queryMng.setParam("id_stato_padre",Convert.ToString(id_stato_padre));
						queryMng.setParam("id_stato_automatico",Convert.ToString(id_stato_automatico));
						queryMng.setParam("desc_stato_automatico",desc_stato_automatico);
						commandText=queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - updateDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                        logger.Debug("SQL - updateDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
					}					
				}

				//Controllo se ci sono degli stati da cancellare
				queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATI");	
				queryMng.setParam("id_diagramma",Convert.ToString(dg.SYSTEM_ID));
				commandText=queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - updateDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - updateDiagramma - DiagrammiStato.cs - QUERY : " + commandText);

				DataSet ds_2 = new DataSet();
				dbProvider.ExecuteQuery(ds_2,commandText);

				if(ds_2.Tables[0].Rows.Count != dg.STATI.Count)
				{
					for(int i=0; i<dg.STATI.Count; i++)
					{
						DocsPaVO.DiagrammaStato.Stato st = (DocsPaVO.DiagrammaStato.Stato) dg.STATI[i];
						for(int j=0; j<ds_2.Tables[0].Rows.Count; j++)
						{
							if( st.SYSTEM_ID == Convert.ToInt32(ds_2.Tables[0].Rows[j]["SYSTEM_ID"].ToString()) ||
                                st.DESCRIZIONE.ToUpper() == ds_2.Tables[0].Rows[j]["VAR_DESCRIZIONE"].ToString().ToUpper()
                               )
							{
								ds_2.Tables[0].Rows[j].Delete();
								ds_2.AcceptChanges();
							}
						}
					}
					for(int k=0; k<ds_2.Tables[0].Rows.Count; k++)
					{
						queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_DELETE_DPA_STATI");		
						queryMng.setParam("id_stato",ds_2.Tables[0].Rows[k]["SYSTEM_ID"].ToString());
						commandText=queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - updateDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                        logger.Debug("SQL - updateDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                    }
				}
			}
			catch 
			{
				dbProvider.RollbackTransaction();
			}
			finally
			{
				dbProvider.Dispose();
			}			
		}


		public bool associaTipoDocDiagramma(string idTipoDoc, string idDiagramma)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

			try
			{
				//Verifico se esiste già un'associazione per lo specifico tipo di documento
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_ASSOCIA_TIPO_DOC_DIAGRAMMA");	
				queryMng.setParam("idTipoDoc",idTipoDoc);
				queryMng.setParam("idDiagramma",idDiagramma);
				string commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - associaTipoDocDiagramma - DiagrammiStato.cs - QUERY : "+commandText);
                logger.Debug("SQL - associaTipoDocDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);

				//Effettuo un UPDATE
				if(ds.Tables[0].Rows.Count != 0)
				{
					return true;
				}
				//Effettuo in INSERT
				else
				{
					queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_ASSOCIA_TIPO_DOC_DIAGRAMMA");		
					queryMng.setParam("colID",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
					queryMng.setParam("id",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_DIAGRAMMI"));
					queryMng.setParam("idTipoDoc",idTipoDoc);
					queryMng.setParam("idDiagramma",idDiagramma);
					commandText=queryMng.getSQL();
					System.Diagnostics.Debug.WriteLine("SQL - associaTipoDocDiagramma - ProfilazioneDinamica/Database/model.cs - QUERY : "+commandText);
                    logger.Debug("SQL - associaTipoDocDiagramma - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
					dbProvider.ExecuteNonQuery(commandText);
				}
			}
			catch 
			{
				dbProvider.RollbackTransaction();
			}
			finally
			{
				dbProvider.Dispose();
			}
			return false;
		}


        public bool getDocOrFascInStato(string idStato)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

			try
			{
				//Verifico se esistono documenti nello specifico stato
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_DOC_FASC_IN_STATO");	
				queryMng.setParam("idStato",idStato);
				string commandText=queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getDocOrFascInStato - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - getDocOrFascInStato - DiagrammiStato.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);	
				if(ds.Tables[0].Rows.Count != 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch 
			{
				return false;
			}
			finally
			{
				dbProvider.Dispose();
			}						
		}


		public void disassociaTipoDocDiagramma(string idTipoDoc)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

			try
			{
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_DISASSOCIA_TIPO_DOC_DIAGRAMMA");		
				queryMng.setParam("idTipoDoc",idTipoDoc);
				string commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - disassociaTipoDocDiagramma - DiagrammiStato.cs - QUERY : "+commandText);
                logger.Debug("SQL - disassociaTipoDocDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
			}
			catch 
			{
				dbProvider.RollbackTransaction();
			}
			finally
			{
				dbProvider.Dispose();				
			}			
		}


		public int getDiagrammaAssociato(string idTipoDoc)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
			int result = 0;
			try
			{
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_DIAGRAMMA_ASSOCIATO");	
				queryMng.setParam("idTipoDoc",idTipoDoc);
				string commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - getDiagrammaAssociato - DiagrammiStato.cs - QUERY : "+commandText);
                logger.Debug("SQL - getDiagrammaAssociato - DiagrammiStato.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);	
				if(ds.Tables[0].Rows.Count != 0)
					result = Convert.ToInt32(ds.Tables[0].Rows[0]["ID_DIAGRAMMA"].ToString());				
			}
			catch 
			{
				return result;
			}
			finally
			{
				dbProvider.Dispose();
			}	
			return result;
		}


		public bool isModificabile(int systemIdDiagramma)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

			try
			{
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_IS_MODIFICABILE");	
				queryMng.setParam("systemIdDiagramma",Convert.ToString(systemIdDiagramma));
				string commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - isModificabile - DiagrammiStato.cs - QUERY : "+commandText);
                logger.Debug("SQL - isModificabile - DiagrammiStato.cs - QUERY : " + commandText);

				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);	
				if(ds.Tables[0].Rows.Count != 0)
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
			}				
		}


		public ArrayList isStatoTrasmAuto(string idAmm, string idStato, string idTemplate)
		{
            ArrayList modelli = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

			if(idStato == "")
                return modelli;

            try
			{
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_IS_STATO_TRASM_AUTO");	
				queryMng.setParam("idStato",idStato);
				queryMng.setParam("idTemplate",idTemplate);
				string commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - isStatoTrasmAuto - DiagrammiStato.cs - QUERY : "+commandText);
                logger.Debug("SQL - isStatoTrasmAuto - DiagrammiStato.cs - QUERY : " + commandText);

				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);	
				if(ds.Tables[0].Rows.Count != 0)
				{
                    ModTrasmissioni modTrasm = new ModTrasmissioni();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (ds.Tables[0].Rows[i]["TRASM_AUT"].ToString() == "1")
                           modelli.Add(modTrasm.getModelloByID(idAmm, ds.Tables[0].Rows[i]["ID_MOD_TRASM"].ToString()));
                    }
                    return modelli;                    
				}
                return modelli;
			}
			catch 
			{
                return modelli;
			}
			finally
			{
				dbProvider.Dispose();				
			}			
		}


		public bool isStatoAuto(string idStato, string idDiagramma)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

			try
			{
				DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = getDiagrammaById(idDiagramma);
				for(int i=0; i<diagramma.PASSI.Count; i++)
				{
					DocsPaVO.DiagrammaStato.Passo passo = (DocsPaVO.DiagrammaStato.Passo) diagramma.PASSI[i];
					if(passo.ID_STATO_AUTOMATICO == idStato)
						return true;
				}
				return false;
			}
			catch 
			{
				return false;
			}
			finally
			{
				dbProvider.Dispose();				
			}				
		}


		public DocsPaVO.DiagrammaStato.DiagrammaStato getDgByIdTipoDoc(string systemIdTipoDoc, string idAmm)
		{
			DocsPaVO.DiagrammaStato.DiagrammaStato dg = null;
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
			
			try
			{
				
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_BY_ID_TIPO_DOC");	
				queryMng.setParam("id_tipo_atto",systemIdTipoDoc);
                queryMng.setParam("id_amm",idAmm);
				string commandText=queryMng.getSQL();
				logger.Debug("SQL - getDgByIdTipoDoc - DiagrammiStato.cs - QUERY : " + commandText);

				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				if(ds.Tables[0].Rows.Count != 0)
				{
					dg = new DocsPaVO.DiagrammaStato.DiagrammaStato();
					queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_DIAGRAMMI_STATO");	
					queryMng.setParam("system_id",ds.Tables[0].Rows[0]["ID_DIAGRAMMA"].ToString());
					commandText=queryMng.getSQL();
					logger.Debug("SQL - getDgByIdTipoDoc - DiagrammiStato.cs - QUERY : " + commandText);

					ds = new DataSet();
					dbProvider.ExecuteQuery(ds,commandText);

					dg.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
					dg.ID_AMM = Convert.ToInt32(ds.Tables[0].Rows[0]["Id_amm"].ToString());
					dg.DESCRIZIONE = ds.Tables[0].Rows[0]["Var_descrizione"].ToString();
					
					//Seleziono gli stati per lo specifico diagramma
					queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATI");	
					queryMng.setParam("id_diagramma",Convert.ToString(dg.SYSTEM_ID));
					commandText=queryMng.getSQL();
					logger.Debug("SQL - getDgByIdTipoDoc - DiagrammiStato.cs - QUERY : " + commandText);

					DataSet ds_1 = new DataSet();
					dbProvider.ExecuteQuery(ds_1,commandText);

					for(int j=0; j<ds_1.Tables[0].Rows.Count; j++)
					{
						DocsPaVO.DiagrammaStato.Stato st = new DocsPaVO.DiagrammaStato.Stato();
						st.SYSTEM_ID = Convert.ToInt32(ds_1.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
						st.ID_DIAGRAMMA = Convert.ToInt32(ds_1.Tables[0].Rows[j]["ID_DIAGRAMMA"].ToString());
						st.DESCRIZIONE = ds_1.Tables[0].Rows[j]["Var_descrizione"].ToString();
						if(ds_1.Tables[0].Rows[j]["Stato_iniziale"].ToString() == "0")
							st.STATO_INIZIALE = false;
						else
							st.STATO_INIZIALE = true;

						if(ds_1.Tables[0].Rows[j]["Stato_finale"].ToString() == "0")
							st.STATO_FINALE = false;
						else
							st.STATO_FINALE = true;

                        if (ds_1.Tables[0].Rows[j]["Conv_Pdf"].ToString() == "1")
                            st.CONVERSIONE_PDF = true;
                        else
                            st.CONVERSIONE_PDF = false;

                        if (ds_1.Tables[0].Rows[j]["Stato_consolidamento"] != DBNull.Value)
                        {
                            st.STATO_CONSOLIDAMENTO = (DocsPaVO.documento.DocumentConsolidationStateEnum)
                                    Enum.Parse(typeof(DocsPaVO.documento.DocumentConsolidationStateEnum),
                                    ds_1.Tables[0].Rows[j]["Stato_consolidamento"].ToString(), true);
                        }

                        if (ds_1.Tables[0].Rows[j]["NON_RICERCABILE"].ToString() == "1")
                            st.NON_RICERCABILE = true;
                        else
                            st.NON_RICERCABILE = false;
                        if (ds_1.Tables[0].Rows[j]["CHA_STATO_SISTEMA"].ToString().Equals("1"))
                            st.STATO_SISTEMA = true;
                        else
                            st.STATO_SISTEMA = false;
                        if (ds_1.Tables[0].Rows[j]["ID_PROCESSO_FIRMA"] != DBNull.Value)
                        {
                            st.ID_PROCESSO_FIRMA = ds_1.Tables[0].Rows[j]["ID_PROCESSO_FIRMA"].ToString();
                        }
                        dg.STATI.Add(st);
					}

					//Seleziono i passi per lo specifico stato 
					for(int k=0; k<dg.STATI.Count; k++)
					{
						DocsPaVO.DiagrammaStato.Stato st = (DocsPaVO.DiagrammaStato.Stato) dg.STATI[k];
						queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_PASSI");	
						queryMng.setParam("id_stato",Convert.ToString(st.SYSTEM_ID));
						commandText=queryMng.getSQL();
						logger.Debug("SQL - getDgByIdTipoDoc - DiagrammiStato.cs - QUERY : " + commandText);

						DataSet ds_2 = new DataSet();
						dbProvider.ExecuteQuery(ds_2,commandText);

                        DocsPaVO.DiagrammaStato.Passo step = new DocsPaVO.DiagrammaStato.Passo();
                        if (ds_2.Tables[0].Rows.Count != 0)
                        {
                            //Proprietà del passo
                            step.ID_DIAGRAMMA = Convert.ToInt32(ds_2.Tables[0].Rows[0]["ID_DIAGRAMMA_PASSO"].ToString());
                            step.ID_STATO_AUTOMATICO = ds_2.Tables[0].Rows[0]["ID_STATO_AUTO_PASSO"].ToString();
                            step.DESCRIZIONE_STATO_AUTOMATICO = ds_2.Tables[0].Rows[0]["DESC_STATO_AUTO_PASSO"].ToString();

                            //Stato Padre del passo
                            DocsPaVO.DiagrammaStato.Stato st_1 = new DocsPaVO.DiagrammaStato.Stato();
                            st_1.SYSTEM_ID = Convert.ToInt32(ds_2.Tables[0].Rows[0]["ID_STATO_PADRE"].ToString());
                            st_1.ID_DIAGRAMMA = Convert.ToInt32(ds_2.Tables[0].Rows[0]["ID_DIAGRAMMA_STATO_PADRE"].ToString());
                            st_1.DESCRIZIONE = ds_2.Tables[0].Rows[0]["VAR_DESCRIZIONE_STATO_PADRE"].ToString();
                            if (ds_2.Tables[0].Rows[0]["STATO_INIZIALE_STATO_PADRE"].ToString() == "0")
                                st_1.STATO_INIZIALE = false;
                            else
                                st_1.STATO_INIZIALE = true;

                            if (ds_2.Tables[0].Rows[0]["STATO_FINALE_STATO_PADRE"].ToString() == "0")
                                st_1.STATO_FINALE = false;
                            else
                                st_1.STATO_FINALE = true;

                            if (ds_2.Tables[0].Rows[0]["CONV_PDF_STATO_PADRE"].ToString() == "1")
                                st_1.CONVERSIONE_PDF = true;
                            else
                                st_1.CONVERSIONE_PDF = false;

                            if (ds_2.Tables[0].Rows[0]["STATO_CONS_STATO_PADRE"] != DBNull.Value)
                            {
                                st_1.STATO_CONSOLIDAMENTO = (DocsPaVO.documento.DocumentConsolidationStateEnum)
                                    Enum.Parse(typeof(DocsPaVO.documento.DocumentConsolidationStateEnum),
                                    ds_2.Tables[0].Rows[0]["STATO_CONS_STATO_PADRE"].ToString(), true);
                            }
                            if (ds_2.Tables[0].Rows[0]["ID_PROCESSO_FIRMA"] != DBNull.Value)
                            {
                                st_1.ID_PROCESSO_FIRMA = ds_2.Tables[0].Rows[0]["ID_PROCESSO_FIRMA"].ToString();
                            }
                            step.STATO_PADRE = st_1;

                            //Stati successivi del passo
                            for (int y = 0; y < ds_2.Tables[0].Rows.Count; y++)
                            {
                                DocsPaVO.DiagrammaStato.Stato st_2 = new DocsPaVO.DiagrammaStato.Stato();
                                st_2.SYSTEM_ID = Convert.ToInt32(ds_2.Tables[0].Rows[y]["ID_STATO_SUCC"].ToString());
                                st_2.ID_DIAGRAMMA = Convert.ToInt32(ds_2.Tables[0].Rows[y]["ID_DIAGRAMMA_STATO_SUCC"].ToString());
                                st_2.DESCRIZIONE = ds_2.Tables[0].Rows[y]["VAR_DESCRIZIONE_STATO_SUCC"].ToString();
                                if (ds_2.Tables[0].Rows[y]["STATO_INIZIALE_STATO_SUCC"].ToString() == "0")
                                    st_2.STATO_INIZIALE = false;
                                else
                                    st_2.STATO_INIZIALE = true;

                                if (ds_2.Tables[0].Rows[y]["STATO_FINALE_STATO_SUCC"].ToString() == "0")
                                    st_2.STATO_FINALE = false;
                                else
                                    st_2.STATO_FINALE = true;

                                if (ds_2.Tables[0].Rows[y]["CONV_PDF_STATO_SUCC"].ToString() == "1")
                                    st_2.CONVERSIONE_PDF = true;
                                else
                                    st_2.CONVERSIONE_PDF = false;

                                if (ds_2.Tables[0].Rows[y]["STATO_CONS_STATO_SUCC"] != DBNull.Value)
                                {
                                    st_2.STATO_CONSOLIDAMENTO = (DocsPaVO.documento.DocumentConsolidationStateEnum)
                                        Enum.Parse(typeof(DocsPaVO.documento.DocumentConsolidationStateEnum),
                                        ds_2.Tables[0].Rows[y]["STATO_CONS_STATO_SUCC"].ToString(), true);
                                }
                                if (ds_2.Tables[0].Rows[y]["STATO_SISTEMA_STATO_SUCC"].ToString().Equals("1"))
                                    st_2.STATO_SISTEMA = true;
                                else
                                    st_2.STATO_SISTEMA = false;

                                if (ds_2.Tables[0].Rows[y]["CHA_STATO_AUTOMATICO_LF"].ToString() == "1")
                                    step.ID_STATO_AUTOMATICO_LF = st_2.SYSTEM_ID.ToString();

                                step.SUCCESSIVI.Add(st_2);			
                            }
                        }

                        #region VECCHIA GESTIONE CON QUERY MULTIPLE
                        /*
                        DocsPaVO.DiagrammaStato.Passo step =new DocsPaVO.DiagrammaStato.Passo();
						for(int y=0; y<ds_2.Tables[0].Rows.Count; y++)
						{
							step.ID_DIAGRAMMA = Convert.ToInt32(ds_2.Tables[0].Rows[y]["Id_diagramma"].ToString());
							step.ID_STATO_AUTOMATICO = ds_2.Tables[0].Rows[y]["ID_STATO_AUTO"].ToString();
							step.DESCRIZIONE_STATO_AUTOMATICO = ds_2.Tables[0].Rows[y]["DESC_STATO_AUTO"].ToString();


                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");	
							queryMng.setParam("system_id",ds_2.Tables[0].Rows[y]["ID_STATO"].ToString());
							commandText=queryMng.getSQL();
							System.Diagnostics.Debug.WriteLine("SQL - getDgByIdTipoDoc - DiagrammiStato.cs - QUERY : "+commandText);
                            logger.Debug("SQL - getDgByIdTipoDoc - DiagrammiStato.cs - QUERY : " + commandText);

							DataSet ds_3 = new DataSet();
							dbProvider.ExecuteQuery(ds_3,commandText);

							DocsPaVO.DiagrammaStato.Stato st_1 = new DocsPaVO.DiagrammaStato.Stato();
							st_1.SYSTEM_ID = Convert.ToInt32(ds_3.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
							st_1.ID_DIAGRAMMA = Convert.ToInt32(ds_3.Tables[0].Rows[0]["ID_DIAGRAMMA"].ToString());
							st_1.DESCRIZIONE = ds_3.Tables[0].Rows[0]["Var_descrizione"].ToString();
							if(ds_3.Tables[0].Rows[0]["Stato_iniziale"].ToString() == "0")
								st_1.STATO_INIZIALE = false;
							else
								st_1.STATO_INIZIALE = true;

							if(ds_3.Tables[0].Rows[0]["Stato_finale"].ToString() == "0")
								st_1.STATO_FINALE = false;
							else
								st_1.STATO_FINALE = true;

                            if (ds_3.Tables[0].Rows[0]["Conv_Pdf"].ToString() == "1")
                                st_1.CONVERSIONE_PDF = true;
                            else
                                st_1.CONVERSIONE_PDF = false;

							step.STATO_PADRE = st_1;

                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");	
							queryMng.setParam("system_id",ds_2.Tables[0].Rows[y]["Id_next_stato"].ToString());
							commandText=queryMng.getSQL();
							System.Diagnostics.Debug.WriteLine("SQL - getDgByIdTipoDoc - DiagrammiStato.cs - QUERY : "+commandText);
                            logger.Debug("SQL - getDgByIdTipoDoc - DiagrammiStato.cs - QUERY : " + commandText);

							DataSet ds_4 = new DataSet();
							dbProvider.ExecuteQuery(ds_4,commandText);

							DocsPaVO.DiagrammaStato.Stato st_2 = new DocsPaVO.DiagrammaStato.Stato();
							st_2.SYSTEM_ID = Convert.ToInt32(ds_4.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
							st_2.ID_DIAGRAMMA = Convert.ToInt32(ds_4.Tables[0].Rows[0]["ID_DIAGRAMMA"].ToString());
							st_2.DESCRIZIONE = ds_4.Tables[0].Rows[0]["Var_descrizione"].ToString();
							if(ds_4.Tables[0].Rows[0]["Stato_iniziale"].ToString() == "0")
								st_2.STATO_INIZIALE = false;
							else
								st_2.STATO_INIZIALE = true;

							if(ds_4.Tables[0].Rows[0]["Stato_finale"].ToString() == "0")
								st_2.STATO_FINALE = false;
							else
								st_2.STATO_FINALE = true;

                            if (ds_4.Tables[0].Rows[0]["Conv_Pdf"].ToString() == "1")
                                st_2.CONVERSIONE_PDF = true;
                            else
                                st_2.CONVERSIONE_PDF = false;
								
							step.SUCCESSIVI.Add(st_2);							
						}
                        */
                        #endregion VECCHIA GESTIONE CON QUERY MULTIPLE
                        
                        if (step.STATO_PADRE != null)
							dg.PASSI.Add(step);
					}
				}
				else
				{
					return null;
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
			return dg;
		}


		public void salvaModificaStato(string docNumber, string idStato, DocsPaVO.DiagrammaStato.DiagrammaStato diagramma, string idUtente, DocsPaVO.utente.InfoUtente user,string dataScadenza)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

			try
			{
				
				//In ogni caso poichè è stata effettuata una modifica dello stato, elimino lo storico delle trasmissioni
				//che mi serviva per verificare le accettazioni per il passaggio di stato in automatico.
				DocsPaVO.DiagrammaStato.Stato statoOld = getStatoDoc (docNumber);
				if(statoOld != null)
					deleteStoricoTrasmDiagrammi(docNumber,Convert.ToString(statoOld.SYSTEM_ID));

				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_DOC_1");	
				queryMng.setParam("docNumber",docNumber);
				string commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStato - DiagrammiStato.cs - QUERY : "+commandText);
                logger.Debug("SQL - salvaModificaStato - DiagrammiStato.cs - QUERY : " + commandText);

				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				
				//Faccio un update
				if(ds.Tables[0].Rows.Count != 0)
				{
					if(ds.Tables[0].Rows[0]["Id_stato"].ToString() != idStato)
					{
						queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_UPDATE_DPA_DIAGRAMMI_DOC");		
						queryMng.setParam("idStato",idStato);
						queryMng.setParam("docNumber",docNumber);
						commandText=queryMng.getSQL();
						System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStato - DiagrammiStato.cs - QUERY : "+commandText);
                        logger.Debug("SQL - salvaModificaStato - DiagrammiStato.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
						
						//Per lo storico
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");	
						queryMng.setParam("system_id",ds.Tables[0].Rows[0][2].ToString());
						commandText=queryMng.getSQL();
						System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStato - DiagrammiStato.cs - QUERY : "+commandText);
                        logger.Debug("SQL - salvaModificaStato - DiagrammiStato.cs - QUERY : " + commandText);
                        DataSet oldStato = new DataSet();
						dbProvider.ExecuteQuery(oldStato,commandText);

                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");	
						queryMng.setParam("system_id",idStato);
						commandText=queryMng.getSQL();
						System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStato - DiagrammiStato.cs - QUERY : "+commandText);
                        logger.Debug("SQL - salvaModificaStato - DiagrammiStato.cs - QUERY : " + commandText);
                        DataSet newStato = new DataSet();
						dbProvider.ExecuteQuery(newStato,commandText);

						queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_DPA_DIAGRAMMI_STO");		
						queryMng.setParam("colID",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
						queryMng.setParam("id",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_DIAGRAMMI_STO"));
						queryMng.setParam("idUtente",idUtente);
						queryMng.setParam("docNumber",docNumber);
						queryMng.setParam("oldStato",oldStato.Tables[0].Rows[0]["Var_descrizione"].ToString().Replace("'","''"));
						queryMng.setParam("newStato",newStato.Tables[0].Rows[0]["Var_descrizione"].ToString().Replace("'","''"));
						queryMng.setParam("idPeople",user.idPeople);

                        //Se non c'è l'idGruppo stò facendo il cambio stato da amministrazione.
						queryMng.setParam("idRuolo",string.IsNullOrEmpty(user.idGruppo) ? "0" : user.idCorrGlobali);

                        string idPeopleDelegato = "0";
                        if (user.delegato!=null)
                            idPeopleDelegato = user.delegato.idPeople;
                        queryMng.setParam("idPeopleDelegato", idPeopleDelegato);

						commandText=queryMng.getSQL();
						System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStato - DiagrammiStato.cs  - QUERY : "+commandText);
                        logger.Debug("SQL - salvaModificaStato - DiagrammiStato.cs  - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
					}
				}
				//Faccio un inserimento
				else
				{
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");	
					queryMng.setParam("system_id",idStato);
					commandText=queryMng.getSQL();
					System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStato - DiagrammiStato.cs - QUERY : "+commandText);
                    logger.Debug("SQL - salvaModificaStato - DiagrammiStato.cs - QUERY : " + commandText);
                    DataSet stato = new DataSet();
					dbProvider.ExecuteQuery(stato,commandText);

					if(stato.Tables[0].Rows.Count != 0)
					{
						queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_DPA_DIAGRAMMI_DOC");		
						queryMng.setParam("colID",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
						queryMng.setParam("id",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_DIAGRAMMI_DOC"));
						queryMng.setParam("docNumber",docNumber);
						queryMng.setParam("idStato",idStato);
						queryMng.setParam("idDiagramma",Convert.ToString(diagramma.SYSTEM_ID));

                        

						commandText=queryMng.getSQL();
						System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStato - DiagrammiStato.cs  - QUERY : "+commandText);
                        logger.Debug("SQL - salvaModificaStato - DiagrammiStato.cs  - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
						
						//Per lo storico
						queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_DPA_DIAGRAMMI_STO");		
						queryMng.setParam("colID",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
						queryMng.setParam("id",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_DIAGRAMMI_STO"));
						queryMng.setParam("idUtente",idUtente);
						queryMng.setParam("docNumber",docNumber);
						queryMng.setParam("oldStato",stato.Tables[0].Rows[0]["Var_descrizione"].ToString().Replace("'","''"));
						queryMng.setParam("newStato",stato.Tables[0].Rows[0]["Var_descrizione"].ToString().Replace("'","''"));
						queryMng.setParam("idPeople",user.idPeople);
						queryMng.setParam("idRuolo",user.idCorrGlobali);

                        string idPeopleDelegato = "0";
                        if (user.delegato != null)
                            idPeopleDelegato = user.delegato.idPeople;
                        queryMng.setParam("idPeopleDelegato", idPeopleDelegato);

                        commandText=queryMng.getSQL();
						System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStato - DiagrammiStato.cs - QUERY : "+commandText);
                        logger.Debug("SQL - salvaModificaStato - DiagrammiStato.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
						
                        //Controllo che sia effettivamente uno stato iniziale
                        //In questo caso devo effettuare l'UPDATE della DT_SCADENZA nella PROFILE
                        //in funzione dei giorni impostati per il ciclo di vita di questo tipo di documento
                        if (stato.Tables[0].Rows[0]["STATO_INIZIALE"].ToString() == "1")
                        {
                            //Recupero l'id della Tipologia Documento
                            Model model = new Model();
                            String idTipoAtto = model.getIdTemplate(docNumber);
                            if (!string.IsNullOrEmpty(idTipoAtto))
                            {
                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_SCADENZE");
                                queryMng.setParam("idTipoAtto", idTipoAtto);
                                commandText = queryMng.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStato - DiagrammiStato.cs - QUERY : " + commandText);
                                logger.Debug("SQL - salvaModificaStato - DiagrammiStato.cs - QUERY : " + commandText);
                                DataSet scadenze = new DataSet();
                                dbProvider.ExecuteQuery(scadenze, commandText);
                                if (scadenze.Tables[0].Rows.Count != 0)
                                {
                                    string scadenza = scadenze.Tables[0].Rows[0]["GG_SCADENZA"].ToString();
                                    string preScadenza = scadenze.Tables[0].Rows[0]["GG_PRE_SCADENZA"].ToString();

                                    if (dataScadenza == "" && scadenza != "0")
                                    {
                                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_SCADENZA_PROFILE");
                                        queryMng.setParam("scadenza_gg", scadenza);
                                        queryMng.setParam("docNumber", docNumber);
                                        commandText = queryMng.getSQL();
                                        dbProvider.ExecuteNonQuery(commandText);
                                    }
                                    else
                                    {
                                        if (dataScadenza != null && dataScadenza != "")
                                        {
                                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_SCADENZA_PROFILE_1");
                                            queryMng.setParam("data", dataScadenza);
                                            queryMng.setParam("docNumber", docNumber);
                                            commandText = queryMng.getSQL();
                                            dbProvider.ExecuteNonQuery(commandText);
                                        }
                                    }                                    
                                }
                            }
                        }
					}
				}
			}
			catch 
			{
				dbProvider.RollbackTransaction();
			}
			finally
			{
				dbProvider.Dispose();
			}
		}


		public DocsPaVO.DiagrammaStato.Stato getStatoDoc(string docNumber)
		{
			DocsPaVO.DiagrammaStato.Stato stato = null;
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

			try
			{
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_DOC_1");	
				queryMng.setParam("docNumber",docNumber);
				string commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - getStatoDoc - DiagrammiStato.cs - QUERY : "+commandText);
                logger.Debug("SQL - getStatoDoc - DiagrammiStato.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				
				if(ds.Tables[0].Rows.Count != 0)
				{
					stato = new DocsPaVO.DiagrammaStato.Stato();
					stato.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["Id_stato"].ToString());
					stato.ID_DIAGRAMMA = Convert.ToInt32(ds.Tables[0].Rows[0]["Id_diagramma"].ToString());

                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");	
					queryMng.setParam("system_id",ds.Tables[0].Rows[0]["Id_stato"].ToString());
					commandText=queryMng.getSQL();
					System.Diagnostics.Debug.WriteLine("SQL - getStatoDoc - DiagrammiStato.cs - QUERY : "+commandText);
                    logger.Debug("SQL - getStatoDoc - DiagrammiStato.cs - QUERY : " + commandText);

					DataSet ds_1 = new DataSet();
					dbProvider.ExecuteQuery(ds_1,commandText);

					stato.DESCRIZIONE = ds_1.Tables[0].Rows[0]["Var_descrizione"].ToString();
					if(ds_1.Tables[0].Rows[0]["Stato_iniziale"].ToString()== "1")
						stato.STATO_INIZIALE = true;
					else
						stato.STATO_INIZIALE = false;

					if(ds_1.Tables[0].Rows[0]["Stato_finale"].ToString() == "1")
						stato.STATO_FINALE = true;
					else
						stato.STATO_FINALE = false;

                    if (ds_1.Tables[0].Rows[0]["Conv_Pdf"].ToString() == "1")
                        stato.CONVERSIONE_PDF = true;
                    else
                        stato.CONVERSIONE_PDF = false;

                    if (ds_1.Tables[0].Rows[0]["NON_RICERCABILE"].ToString() == "1")
                        stato.NON_RICERCABILE = true;
                    else
                        stato.NON_RICERCABILE = false;

                    if (ds_1.Tables[0].Rows[0]["ID_PROCESSO_FIRMA"] != DBNull.Value)
                    {
                        stato.ID_PROCESSO_FIRMA = ds_1.Tables[0].Rows[0]["ID_PROCESSO_FIRMA"].ToString();
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
			return stato;				
		}

        public DocsPaVO.DiagrammaStato.Stato GetStatoDocPrecedente(string docnumber)
        {
            DocsPaVO.DiagrammaStato.Stato stato = new DocsPaVO.DiagrammaStato.Stato();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_PRECEDENTE");
                queryMng.setParam("docnumber", docnumber);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - GetStatoDocPrecedente - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - GetStatoDocPrecedente - DiagrammiStato.cs - QUERY : " + commandText);

                DataSet ds_1 = new DataSet();
                if(this.ExecuteQuery(ds_1, commandText) && ds_1.Tables[0].Rows.Count != 0)
                {
                    stato.SYSTEM_ID = Convert.ToInt32(ds_1.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                    stato.ID_DIAGRAMMA = Convert.ToInt32(ds_1.Tables[0].Rows[0]["ID_DIAGRAMMA"].ToString());
                    stato.DESCRIZIONE = ds_1.Tables[0].Rows[0]["Var_descrizione"].ToString();
                    if (ds_1.Tables[0].Rows[0]["Stato_iniziale"].ToString() == "1")
                        stato.STATO_INIZIALE = true;
                    else
                        stato.STATO_INIZIALE = false;

                    if (ds_1.Tables[0].Rows[0]["Stato_finale"].ToString() == "1")
                        stato.STATO_FINALE = true;
                    else
                        stato.STATO_FINALE = false;

                    if (ds_1.Tables[0].Rows[0]["Conv_Pdf"].ToString() == "1")
                        stato.CONVERSIONE_PDF = true;
                    else
                        stato.CONVERSIONE_PDF = false;

                    if (ds_1.Tables[0].Rows[0]["NON_RICERCABILE"].ToString() == "1")
                        stato.NON_RICERCABILE = true;
                    else
                        stato.NON_RICERCABILE = false;

                    if (ds_1.Tables[0].Rows[0]["ID_PROCESSO_FIRMA"] != DBNull.Value)
                    {
                        stato.ID_PROCESSO_FIRMA = ds_1.Tables[0].Rows[0]["ID_PROCESSO_FIRMA"].ToString();
                    }
                }
            }
            catch(Exception e)
            {
                logger.Error("Errore in GetStatoDocPrecedente " + e);
            }
            return stato;
        }


        public string getStatoDocStorico(string docNumber)
		{
			string stato = "";
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
			
			try
			{
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_DOC_STORICO");	
				queryMng.setParam("docNumber",docNumber);
				string commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - getStatoDocStorico - DiagrammiStato.cs - QUERY : "+commandText);
                logger.Debug("SQL - getStatoDocStorico - DiagrammiStato.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				if(ds.Tables[0].Rows.Count != 0)
				{
					stato = ds.Tables[0].Rows[0]["Var_desc_new_stato"].ToString();
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
			return stato;	
		}


		public DataSet getDiagrammaStoricoDoc(string docNumber)
		{
			DataSet storico = new DataSet();
			DataTable dt = new DataTable();
			DataSet storico_1 = new DataSet();
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
			
			try
			{
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_DIAGRAMMA_STORICO_DOC");	
				queryMng.setParam("docNumber",docNumber);
				string commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - getDiagrammaStoricoDoc - DiagrammiStato.cs - QUERY : "+commandText);
                logger.Debug("SQL - getDiagrammaStoricoDoc - DiagrammiStato.cs - QUERY : " + commandText);
                dbProvider.ExecuteQuery(storico,commandText);

				dt.Columns.Add("Ruolo");
				dt.Columns.Add("Utente");
				dt.Columns.Add("Data");
				dt.Columns.Add("Vecchio stato");
				dt.Columns.Add("Nuovo Stato");
                			
				for(int i=0; i<storico.Tables[0].Rows.Count; i++)
				{
					DataRow dr = dt.NewRow();
					Utenti ut = new Utenti();
                    if (storico.Tables[0].Rows[i]["ID_RUOLO"].ToString() != null && storico.Tables[0].Rows[i]["ID_RUOLO"].ToString() != "")
                    {
                        DocsPaVO.utente.Ruolo ruolo = ut.GetRuoloEnabledAndDisabled(storico.Tables[0].Rows[i]["ID_RUOLO"].ToString());
                        dr["Ruolo"] = ruolo != null ? ruolo.descrizione : string.Empty;
                    }
                    else
                        dr["Ruolo"] = "";

                    Utenti utente = new Utenti();
                    string delegato = "";
                    if (storico.Tables[0].Columns.Contains("ID_PEOPLE_DELEGATO") && !string.IsNullOrEmpty((storico.Tables[0].Rows[i]["ID_PEOPLE_DELEGATO"].ToString())) && storico.Tables[0].Rows[i]["ID_PEOPLE_DELEGATO"].ToString() != "0")
                        delegato = utente.GetUtente(storico.Tables[0].Rows[i]["ID_PEOPLE_DELEGATO"].ToString()).descrizione;


                    if (storico.Tables[0].Rows[i]["ID_PEOPLE"].ToString() != null && storico.Tables[0].Rows[i]["ID_PEOPLE"].ToString() != "")
                    {
                        if (string.IsNullOrEmpty(delegato))
                        {
                            DocsPaVO.utente.Utente user = ut.GetUtenteNoFiltroDisabled(storico.Tables[0].Rows[i]["ID_PEOPLE"].ToString());
                            dr["Utente"] = !string.IsNullOrEmpty(user.descrizione.Trim()) ? user.descrizione : user.userId;
                        }
                        else
                            dr["Utente"] = delegato + "<br>Sostituto di " + (ut.GetUtenteNoFiltroDisabled(storico.Tables[0].Rows[i]["ID_PEOPLE"].ToString())).descrizione;
                    }
                    else
                        dr["Utente"] = "";

                    //Se l'id del ruolo è 0, ho inserito lo storico come amministratore
                    if (storico.Tables[0].Rows[i]["ID_RUOLO"].ToString() != null && storico.Tables[0].Rows[i]["ID_RUOLO"].ToString().Equals("0"))
                        dr["Utente"] = "Amministratore di sistema";

                    dr["Data"] = storico.Tables[0].Rows[i]["DTA_DATE"].ToString();
					dr["Vecchio stato"] = storico.Tables[0].Rows[i]["VAR_DESC_OLD_STATO"].ToString();
					dr["Nuovo Stato"] = storico.Tables[0].Rows[i]["VAR_DESC_NEW_STATO"].ToString();

                    
  				    dt.Rows.Add(dr);

				}

                storico_1.Tables.Add(dt);				
			}
			catch(Exception e) 
			{
				return null;
			}
			finally
			{
				dbProvider.Dispose();
			}
			return storico_1;					
		}


		public void salvaStoricoTrasmDiagrammi(string idTrasm, string docNumber, string idStato)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
			
			try
			{
				
				//Verifico se esiste già una trasmissione da controllare nella DPA_TRASM_DIAGR
				//In caso affermativo non effettuo nessuna operazione
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_ID_TRASM_DIAGR");		
				queryMng.setParam("docNumber",docNumber);
				queryMng.setParam("idStato",idStato);
				string commandText=queryMng.getSQL();
				DataSet trasm = new DataSet();
				dbProvider.ExecuteQuery(trasm,commandText);
				if(trasm.Tables[0].Rows.Count != 0)
					return;

				//Inserisco l'associazione stato-documento-tramissione
				queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_TRASM_STORICO");		
				queryMng.setParam("colID",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
				queryMng.setParam("id",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_TRASM_DIAGR"));
				queryMng.setParam("idTrasm",idTrasm);
				queryMng.setParam("docNumber",docNumber);
				queryMng.setParam("idStato",idStato);
				commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - salvaStoricoTrasmDiagrammi - DiagrammiStato.cs - QUERY : "+commandText);
                logger.Debug("SQL - salvaStoricoTrasmDiagrammi - DiagrammiStato.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
			}
			catch 
			{
				dbProvider.RollbackTransaction();
			}
			finally
			{
				dbProvider.Dispose();
			}
		}


		public void deleteStoricoTrasmDiagrammi(string docNumber, string idStato)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
			
			try
			{
				//Elimini eventuali associazioni presenti per lo specifico stato individuato dal system_id
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_DEL_TRASM_STORICO");	
				queryMng.setParam("idStato",idStato);
				queryMng.setParam("docNumber",docNumber);
				string commandText=queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - deleteStoricoTrasmDiagrammi - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - deleteStoricoTrasmDiagrammi - DiagrammiStato.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
			}
			catch 
			{
				dbProvider.RollbackTransaction();
			}
			finally
			{
				dbProvider.Dispose();
			}
		}


		public DocsPaVO.DiagrammaStato.Stato getStatoSuccessivoAutomatico(string docNumber)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
			try
			{
				//Recupero lo stato del documento e il diagramma a cui appartiene
				DocsPaVO.DiagrammaStato.Stato statoDoc = getStatoDoc(docNumber);
				DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = getDiagrammaById(Convert.ToString(statoDoc.ID_DIAGRAMMA));
				for(int k=0; k<diagramma.PASSI.Count; k++)
				{
					if( ((DocsPaVO.DiagrammaStato.Passo)diagramma.PASSI[k]).STATO_PADRE.SYSTEM_ID == statoDoc.SYSTEM_ID)
					{
						if( ((DocsPaVO.DiagrammaStato.Passo)diagramma.PASSI[k]).ID_STATO_AUTOMATICO != null || ((DocsPaVO.DiagrammaStato.Passo)diagramma.PASSI[k]).ID_STATO_AUTOMATICO != "")
						{
							//Recupero i dati dello stato automatico
                            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");	
							queryMng.setParam("system_id",((DocsPaVO.DiagrammaStato.Passo)diagramma.PASSI[k]).ID_STATO_AUTOMATICO);
							string commandText=queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - getStatoSuccessivoAutomatico - DiagrammiStato.cs - QUERY : " + commandText);
                            logger.Debug("SQL - getStatoSuccessivoAutomatico - DiagrammiStato.cs - QUERY : " + commandText);
                            DataSet ds_4 = new DataSet();
							dbProvider.ExecuteQuery(ds_4,commandText);

							DocsPaVO.DiagrammaStato.Stato st_auto = new DocsPaVO.DiagrammaStato.Stato();
							st_auto.SYSTEM_ID = Convert.ToInt32(ds_4.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
							st_auto.ID_DIAGRAMMA = Convert.ToInt32(ds_4.Tables[0].Rows[0]["ID_DIAGRAMMA"].ToString());
							st_auto.DESCRIZIONE = ds_4.Tables[0].Rows[0]["Var_descrizione"].ToString();
							if(ds_4.Tables[0].Rows[0]["Stato_iniziale"].ToString() == "0")
								st_auto.STATO_INIZIALE = false;
							else
								st_auto.STATO_INIZIALE = true;

							if(ds_4.Tables[0].Rows[0]["Stato_finale"].ToString() == "0")
								st_auto.STATO_FINALE = false;
							else
								st_auto.STATO_FINALE = true;

                            if (ds_4.Tables[0].Rows[0]["Conv_Pdf"].ToString() == "1")
                                st_auto.CONVERSIONE_PDF = true;
                            else
                                st_auto.CONVERSIONE_PDF = false;

                            if (ds_4.Tables[0].Rows[0]["NON_RICERCABILE"].ToString() == "1")
                                st_auto.NON_RICERCABILE = true;
                            else
                                st_auto.NON_RICERCABILE = false;

							return st_auto;
						}
					}
				}
				return null;		
			}
			catch 
			{
				dbProvider.RollbackTransaction();
				return null;
			}
			finally
			{
				dbProvider.Dispose();
			}
		}


		public bool isUltimaDaAccettare(string idTrasmissione)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

			try
			{
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_TRASM_DIAGR_AR_1");	
				queryMng.setParam("idTrasmissione",idTrasmissione);
				string commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - isUltimaDaAccettare - DiagrammiStato.cs - QUERY : "+commandText);
                logger.Debug("SQL - isUltimaDaAccettare - DiagrammiStato.cs - QUERY : " + commandText);
                DataSet ds_daAccettare = new DataSet();
                dbProvider.ExecuteQuery(ds_daAccettare, commandText);

                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_TRASM_DIAGR_AR_2");
                queryMng.setParam("idTrasmissione", idTrasmissione);
                commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - isUltimaDaAccettare - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - isUltimaDaAccettare - DiagrammiStato.cs - QUERY : " + commandText);
                DataSet ds_Rifiutate = new DataSet();
                dbProvider.ExecuteQuery(ds_Rifiutate, commandText);

                if (ds_daAccettare.Tables[0].Rows.Count == 0 && ds_Rifiutate.Tables[0].Rows.Count == 0)
					return true;
				else
					return false;
			}
			catch 
			{
				dbProvider.RollbackTransaction();
				return false;
			}
			finally
			{
				dbProvider.Dispose();				
			}		
		}


		public bool isDocumentiInStatoFinale(string idDiagramma, string idTemplate)
		{
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

			try
			{
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_DPA_DIAGRAMMI_DOC");	
				queryMng.setParam("idTemplate",idTemplate);
				queryMng.setParam("idDiagramma",idDiagramma);
				string commandText=queryMng.getSQL();
				System.Diagnostics.Debug.WriteLine("SQL - isDocumentiInStatoFinale - DiagrammiStato.cs - QUERY : "+commandText);
                logger.Debug("SQL - isDocumentiInStatoFinale - DiagrammiStato.cs - QUERY : " + commandText);

				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
                if (ds.Tables[0].Rows.Count != 0)
                    return false;
                
                return true;
			}
			catch 
			{
				dbProvider.RollbackTransaction();
				return false;
			}
			finally
			{
				dbProvider.Dispose();
			}		
		}

        
        public int getDiagrammaAssociatoFasc(string idTipoFasc)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            int result = 0;
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_DIAGRAMMA_ASSOCIATO_FASC");
                queryMng.setParam("idTipoFasc", idTipoFasc);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getDiagrammaAssociatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - getDiagrammaAssociatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                if (ds.Tables[0].Rows.Count != 0)
                    result = Convert.ToInt32(ds.Tables[0].Rows[0]["ID_DIAGRAMMA"].ToString());
            }
            catch
            {
                return result;
            }
            finally
            {
                dbProvider.Dispose();
            }
            return result;
        }


        public bool associaTipoFascDiagramma(string idTipoFasc, string idDiagramma)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //Verifico se esiste già un'associazione per lo specifico tipo di documento
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_ASSOCIA_TIPO_FASC_DIAGRAMMA");
                queryMng.setParam("idTipoFasc", idTipoFasc);
                queryMng.setParam("idDiagramma", idDiagramma);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - associaTipoFascDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - associaTipoFascDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                //Effettuo un UPDATE
                if (ds.Tables[0].Rows.Count != 0)
                {
                    return true;
                }
                //Effettuo in INSERT
                else
                {
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_ASSOCIA_TIPO_FASC_DIAGRAMMA");
                    queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_DIAGRAMMI"));
                    queryMng.setParam("idTipoFasc", idTipoFasc);
                    queryMng.setParam("idDiagramma", idDiagramma);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - associaTipoFascDiagramma -  DiagrammiStato.cs - QUERY : " + commandText);
                    logger.Debug("SQL - associaTipoFascDiagramma -  DiagrammiStato.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                }                
            }
            catch
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
            return false;
        }

       
        public bool isFascicoliInStatoFinale(string idDiagramma, string idTemplate)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_DPA_DIAGRAMMI_FASC");
                queryMng.setParam("idTemplate", idTemplate);
                queryMng.setParam("idDiagramma", idDiagramma);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - isFascicoliInStatoFinale - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - isFascicoliInStatoFinale - DiagrammiStato.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                if (ds.Tables[0].Rows.Count != 0)
                    return false;

                return true;
            }
            catch
            {
                dbProvider.RollbackTransaction();
                return false;
            }
            finally
            {
                dbProvider.Dispose();
            }
        }


        public void disassociaTipoFascDiagramma(string idTipoFasc)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_DISASSOCIA_TIPO_FASC_DIAGRAMMA");
                queryMng.setParam("idTipoFasc", idTipoFasc);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - disassociaTipoFascDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - disassociaTipoFascDiagramma - DiagrammiStato.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
            }
            catch
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        
        public DocsPaVO.DiagrammaStato.DiagrammaStato getDgByIdTipoFasc(string systemIdTipoFasc, string idAmm)
        {
            DocsPaVO.DiagrammaStato.DiagrammaStato dg = null;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {

                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_BY_ID_TIPO_FASC");
                queryMng.setParam("id_tipo_fasc", systemIdTipoFasc);
                queryMng.setParam("id_amm", idAmm);
                string commandText = queryMng.getSQL();
                logger.Debug("SQL - getDgByIdTipoFasc - DiagrammiStato.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    dg = new DocsPaVO.DiagrammaStato.DiagrammaStato();
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_DIAGRAMMI_STATO");
                    queryMng.setParam("system_id", ds.Tables[0].Rows[0]["ID_DIAGRAMMA"].ToString());
                    commandText = queryMng.getSQL();
                    logger.Debug("SQL - getDgByIdTipoFasc - DiagrammiStato.cs - QUERY : " + commandText);

                    ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);

                    dg.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                    dg.ID_AMM = Convert.ToInt32(ds.Tables[0].Rows[0]["Id_amm"].ToString());
                    dg.DESCRIZIONE = ds.Tables[0].Rows[0]["Var_descrizione"].ToString();

                    //Seleziono gli stati per lo specifico diagramma
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATI");
                    queryMng.setParam("id_diagramma", Convert.ToString(dg.SYSTEM_ID));
                    commandText = queryMng.getSQL();
                    logger.Debug("SQL - getDgByIdTipoFasc - DiagrammiStato.cs - QUERY : " + commandText);

                    DataSet ds_1 = new DataSet();
                    dbProvider.ExecuteQuery(ds_1, commandText);

                    for (int j = 0; j < ds_1.Tables[0].Rows.Count; j++)
                    {
                        DocsPaVO.DiagrammaStato.Stato st = new DocsPaVO.DiagrammaStato.Stato();
                        st.SYSTEM_ID = Convert.ToInt32(ds_1.Tables[0].Rows[j]["SYSTEM_ID"].ToString());
                        st.ID_DIAGRAMMA = Convert.ToInt32(ds_1.Tables[0].Rows[j]["ID_DIAGRAMMA"].ToString());
                        st.DESCRIZIONE = ds_1.Tables[0].Rows[j]["Var_descrizione"].ToString();
                        if (ds_1.Tables[0].Rows[j]["Stato_iniziale"].ToString() == "0")
                            st.STATO_INIZIALE = false;
                        else
                            st.STATO_INIZIALE = true;

                        if (ds_1.Tables[0].Rows[j]["Stato_finale"].ToString() == "0")
                            st.STATO_FINALE = false;
                        else
                            st.STATO_FINALE = true;

                        if (ds_1.Tables[0].Rows[j]["Conv_Pdf"].ToString() == "1")
                            st.CONVERSIONE_PDF = true;
                        else
                            st.CONVERSIONE_PDF = false;

                        if (ds_1.Tables[0].Rows[j]["Stato_consolidamento"] != DBNull.Value)
                        {
                            st.STATO_CONSOLIDAMENTO = (DocsPaVO.documento.DocumentConsolidationStateEnum)
                                        Enum.Parse(typeof(DocsPaVO.documento.DocumentConsolidationStateEnum),
                                                ds_1.Tables[0].Rows[j]["Stato_consolidamento"].ToString(), true);
                        }

                        if (ds_1.Tables[0].Rows[j]["NON_RICERCABILE"].ToString() == "1")
                            st.NON_RICERCABILE = true;
                        else
                            st.NON_RICERCABILE = false;

                        dg.STATI.Add(st);
                    }

                    //Seleziono i passi per lo specifico stato 
                    for (int k = 0; k < dg.STATI.Count; k++)
                    {
                        DocsPaVO.DiagrammaStato.Stato st = (DocsPaVO.DiagrammaStato.Stato)dg.STATI[k];
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_PASSI");
                        queryMng.setParam("id_stato", Convert.ToString(st.SYSTEM_ID));
                        commandText = queryMng.getSQL();
                        logger.Debug("SQL - getDgByIdTipoFasc - DiagrammiStato.cs - QUERY : " + commandText);

                        DataSet ds_2 = new DataSet();
                        dbProvider.ExecuteQuery(ds_2, commandText);

                        DocsPaVO.DiagrammaStato.Passo step = new DocsPaVO.DiagrammaStato.Passo();
                        if (ds_2.Tables[0].Rows.Count != 0)
                        {
                            //Proprietà del passo
                            step.ID_DIAGRAMMA = Convert.ToInt32(ds_2.Tables[0].Rows[0]["ID_DIAGRAMMA_PASSO"].ToString());
                            step.ID_STATO_AUTOMATICO = ds_2.Tables[0].Rows[0]["ID_STATO_AUTO_PASSO"].ToString();
                            step.DESCRIZIONE_STATO_AUTOMATICO = ds_2.Tables[0].Rows[0]["DESC_STATO_AUTO_PASSO"].ToString();

                            //Stato Padre del passo
                            DocsPaVO.DiagrammaStato.Stato st_1 = new DocsPaVO.DiagrammaStato.Stato();
                            st_1.SYSTEM_ID = Convert.ToInt32(ds_2.Tables[0].Rows[0]["ID_STATO_PADRE"].ToString());
                            st_1.ID_DIAGRAMMA = Convert.ToInt32(ds_2.Tables[0].Rows[0]["ID_DIAGRAMMA_STATO_PADRE"].ToString());
                            st_1.DESCRIZIONE = ds_2.Tables[0].Rows[0]["VAR_DESCRIZIONE_STATO_PADRE"].ToString();
                            if (ds_2.Tables[0].Rows[0]["STATO_INIZIALE_STATO_PADRE"].ToString() == "0")
                                st_1.STATO_INIZIALE = false;
                            else
                                st_1.STATO_INIZIALE = true;

                            if (ds_2.Tables[0].Rows[0]["STATO_FINALE_STATO_PADRE"].ToString() == "0")
                                st_1.STATO_FINALE = false;
                            else
                                st_1.STATO_FINALE = true;

                            if (ds_2.Tables[0].Rows[0]["CONV_PDF_STATO_PADRE"].ToString() == "1")
                                st_1.CONVERSIONE_PDF = true;
                            else
                                st_1.CONVERSIONE_PDF = false;

                            if (ds_2.Tables[0].Rows[0]["STATO_CONS_STATO_PADRE"] != DBNull.Value)
                            {
                                st_1.STATO_CONSOLIDAMENTO = (DocsPaVO.documento.DocumentConsolidationStateEnum)
                                    Enum.Parse(typeof(DocsPaVO.documento.DocumentConsolidationStateEnum),
                                            ds_2.Tables[0].Rows[0]["STATO_CONS_STATO_PADRE"].ToString(), true);
                            }

                            step.STATO_PADRE = st_1;

                            //Stati successivi del passo
                            for (int y = 0; y < ds_2.Tables[0].Rows.Count; y++)
                            {
                                DocsPaVO.DiagrammaStato.Stato st_2 = new DocsPaVO.DiagrammaStato.Stato();
                                st_2.SYSTEM_ID = Convert.ToInt32(ds_2.Tables[0].Rows[y]["ID_STATO_SUCC"].ToString());
                                st_2.ID_DIAGRAMMA = Convert.ToInt32(ds_2.Tables[0].Rows[y]["ID_DIAGRAMMA_STATO_SUCC"].ToString());
                                st_2.DESCRIZIONE = ds_2.Tables[0].Rows[y]["VAR_DESCRIZIONE_STATO_SUCC"].ToString();
                                if (ds_2.Tables[0].Rows[y]["STATO_INIZIALE_STATO_SUCC"].ToString() == "0")
                                    st_2.STATO_INIZIALE = false;
                                else
                                    st_2.STATO_INIZIALE = true;

                                if (ds_2.Tables[0].Rows[y]["STATO_FINALE_STATO_SUCC"].ToString() == "0")
                                    st_2.STATO_FINALE = false;
                                else
                                    st_2.STATO_FINALE = true;

                                if (ds_2.Tables[0].Rows[y]["CONV_PDF_STATO_SUCC"].ToString() == "1")
                                    st_2.CONVERSIONE_PDF = true;
                                else
                                    st_2.CONVERSIONE_PDF = false;

                                if (ds_2.Tables[0].Rows[y]["STATO_CONS_STATO_SUCC"] != DBNull.Value)
                                {
                                    st_2.STATO_CONSOLIDAMENTO = (DocsPaVO.documento.DocumentConsolidationStateEnum)
                                        Enum.Parse(typeof(DocsPaVO.documento.DocumentConsolidationStateEnum),
                                        ds_2.Tables[0].Rows[y]["STATO_CONS_STATO_SUCC"].ToString(), true);
                                }

                                step.SUCCESSIVI.Add(st_2);
                            }
                        }

                        #region VECCHIA GESTIONE CON QUERY MULTIPLE
                        /*
                        DocsPaVO.DiagrammaStato.Passo step = new DocsPaVO.DiagrammaStato.Passo();
                        for (int y = 0; y < ds_2.Tables[0].Rows.Count; y++)
                        {
                            step.ID_DIAGRAMMA = Convert.ToInt32(ds_2.Tables[0].Rows[y]["Id_diagramma"].ToString());
                            step.ID_STATO_AUTOMATICO = ds_2.Tables[0].Rows[y]["ID_STATO_AUTO"].ToString();
                            step.DESCRIZIONE_STATO_AUTOMATICO = ds_2.Tables[0].Rows[y]["DESC_STATO_AUTO"].ToString();


                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");
                            queryMng.setParam("system_id", ds_2.Tables[0].Rows[y]["ID_STATO"].ToString());
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - getDgByIdTipoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                            logger.Debug("SQL - getDgByIdTipoFasc - DiagrammiStato.cs - QUERY : " + commandText);

                            DataSet ds_3 = new DataSet();
                            dbProvider.ExecuteQuery(ds_3, commandText);

                            DocsPaVO.DiagrammaStato.Stato st_1 = new DocsPaVO.DiagrammaStato.Stato();
                            st_1.SYSTEM_ID = Convert.ToInt32(ds_3.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                            st_1.ID_DIAGRAMMA = Convert.ToInt32(ds_3.Tables[0].Rows[0]["ID_DIAGRAMMA"].ToString());
                            st_1.DESCRIZIONE = ds_3.Tables[0].Rows[0]["Var_descrizione"].ToString();
                            if (ds_3.Tables[0].Rows[0]["Stato_iniziale"].ToString() == "0")
                                st_1.STATO_INIZIALE = false;
                            else
                                st_1.STATO_INIZIALE = true;

                            if (ds_3.Tables[0].Rows[0]["Stato_finale"].ToString() == "0")
                                st_1.STATO_FINALE = false;
                            else
                                st_1.STATO_FINALE = true;

                            if (ds_3.Tables[0].Rows[0]["Conv_Pdf"].ToString() == "1")
                                st_1.CONVERSIONE_PDF = true;
                            else
                                st_1.CONVERSIONE_PDF = false;

                            step.STATO_PADRE = st_1;

                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");
                            queryMng.setParam("system_id", ds_2.Tables[0].Rows[y]["Id_next_stato"].ToString());
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - getDgByIdTipoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                            logger.Debug("SQL - getDgByIdTipoFasc - DiagrammiStato.cs - QUERY : " + commandText);

                            DataSet ds_4 = new DataSet();
                            dbProvider.ExecuteQuery(ds_4, commandText);

                            DocsPaVO.DiagrammaStato.Stato st_2 = new DocsPaVO.DiagrammaStato.Stato();
                            st_2.SYSTEM_ID = Convert.ToInt32(ds_4.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                            st_2.ID_DIAGRAMMA = Convert.ToInt32(ds_4.Tables[0].Rows[0]["ID_DIAGRAMMA"].ToString());
                            st_2.DESCRIZIONE = ds_4.Tables[0].Rows[0]["Var_descrizione"].ToString();
                            if (ds_4.Tables[0].Rows[0]["Stato_iniziale"].ToString() == "0")
                                st_2.STATO_INIZIALE = false;
                            else
                                st_2.STATO_INIZIALE = true;

                            if (ds_4.Tables[0].Rows[0]["Stato_finale"].ToString() == "0")
                                st_2.STATO_FINALE = false;
                            else
                                st_2.STATO_FINALE = true;

                            if (ds_4.Tables[0].Rows[0]["Conv_Pdf"].ToString() == "1")
                                st_2.CONVERSIONE_PDF = true;
                            else
                                st_2.CONVERSIONE_PDF = false;

                            step.SUCCESSIVI.Add(st_2);
                        }
                        */
                        #endregion VECCHIA GESTIONE CON QUERY MULTIPLE

                        if (step.STATO_PADRE != null)
                            dg.PASSI.Add(step);
                    }
                }
                else
                {
                    return null;
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
            return dg;
        }


        public void salvaModificaStatoFasc(string idProject, string idStato, DocsPaVO.DiagrammaStato.DiagrammaStato diagramma, string idUtente, DocsPaVO.utente.InfoUtente user, string dataScadenza)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {

                //In ogni caso poichè è stata effettuata una modifica dello stato, elimino lo storico delle trasmissioni
                //che mi serviva per verificare le accettazioni per il passaggio di stato in automatico.
                DocsPaVO.DiagrammaStato.Stato statoOld = getStatoFasc(idProject);
                if (statoOld != null)
                    deleteStoricoTrasmDiagrammiFasc(idProject, Convert.ToString(statoOld.SYSTEM_ID));

                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_FASC_1");
                queryMng.setParam("idProject", idProject);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - salvaModificaStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                //Faccio un update
                if (ds.Tables[0].Rows.Count != 0)
                {
                    if (ds.Tables[0].Rows[0]["Id_stato"].ToString() != idStato)
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_UPDATE_DPA_DIAGRAMMI_FASC");
                        queryMng.setParam("idStato", idStato);
                        queryMng.setParam("idProject", idProject);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaModificaStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);

                        //Per lo storico
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");
                        queryMng.setParam("system_id", ds.Tables[0].Rows[0][2].ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaModificaStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                        DataSet oldStato = new DataSet();
                        dbProvider.ExecuteQuery(oldStato, commandText);

                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");
                        queryMng.setParam("system_id", idStato);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaModificaStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                        DataSet newStato = new DataSet();
                        dbProvider.ExecuteQuery(newStato, commandText);

                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_DPA_DIAGRAMMI_STO_FASC");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_DIAGRAMMI_STO"));
                        queryMng.setParam("idUtente", idUtente);
                        queryMng.setParam("idProject", idProject);
                        queryMng.setParam("oldStato", oldStato.Tables[0].Rows[0]["Var_descrizione"].ToString().Replace("'", "''"));
                        queryMng.setParam("newStato", newStato.Tables[0].Rows[0]["Var_descrizione"].ToString().Replace("'", "''"));
                        queryMng.setParam("idPeople", user.idPeople);
                        queryMng.setParam("idRuolo", user.idCorrGlobali);

                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStatoFasc - DiagrammiStato.cs  - QUERY : " + commandText);
                        logger.Debug("SQL - salvaModificaStatoFasc - DiagrammiStato.cs  - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                    }
                }
                //Faccio un inserimento
                else
                {
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");
                    queryMng.setParam("system_id", idStato);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                    logger.Debug("SQL - salvaModificaStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                    DataSet stato = new DataSet();
                    dbProvider.ExecuteQuery(stato, commandText);

                    if (stato.Tables[0].Rows.Count != 0)
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_DPA_DIAGRAMMI_FASC");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_DIAGRAMMI"));
                        queryMng.setParam("idProject", idProject);
                        queryMng.setParam("idStato", idStato);
                        queryMng.setParam("idDiagramma", Convert.ToString(diagramma.SYSTEM_ID));

                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStatoFasc - DiagrammiStato.cs  - QUERY : " + commandText);
                        logger.Debug("SQL - salvaModificaStatoFasc - DiagrammiStato.cs  - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);

                        //Per lo storico
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_DPA_DIAGRAMMI_STO_FASC");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_DIAGRAMMI_STO"));
                        queryMng.setParam("idUtente", idUtente);
                        queryMng.setParam("idProject", idProject);
                        queryMng.setParam("oldStato", stato.Tables[0].Rows[0]["Var_descrizione"].ToString().Replace("'", "''"));
                        queryMng.setParam("newStato", stato.Tables[0].Rows[0]["Var_descrizione"].ToString().Replace("'", "''"));
                        queryMng.setParam("idPeople", user.idPeople);
                        queryMng.setParam("idRuolo", user.idCorrGlobali);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaModificaStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);

                        //Controllo che sia effettivamente uno stato iniziale
                        //In questo caso devo effettuare l'UPDATE della DT_SCADENZA nella PROFILE
                        //in funzione dei giorni impostati per il ciclo di vita di questo tipo di documento
                        if (stato.Tables[0].Rows[0]["STATO_INIZIALE"].ToString() == "1")
                        {
                            //Recupero l'id della Tipologia Fascicolo
                            ModelFasc modelFasc = new ModelFasc();
                            string idTipoFasc = modelFasc.getIdTemplateFasc(idProject);
                            if (!string.IsNullOrEmpty(idTipoFasc))
                            {
                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_SCADENZE_FASC");
                                queryMng.setParam("idTipoFasc", idTipoFasc);
                                commandText = queryMng.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - salvaModificaStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                                logger.Debug("SQL - salvaModificaStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                                DataSet scadenze = new DataSet();
                                dbProvider.ExecuteQuery(scadenze, commandText);
                                if (scadenze.Tables[0].Rows.Count != 0)
                                {
                                    string scadenza = scadenze.Tables[0].Rows[0]["GG_SCADENZA"].ToString();
                                    string preScadenza = scadenze.Tables[0].Rows[0]["GG_PRE_SCADENZA"].ToString();

                                    if (dataScadenza == "" && scadenza != "0")
                                    {
                                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_SCADENZA_PROJECT");
                                        queryMng.setParam("scadenza_gg", scadenza);
                                        queryMng.setParam("idProject", idProject);
                                        commandText = queryMng.getSQL();
                                        dbProvider.ExecuteNonQuery(commandText);
                                    }
                                    else
                                    {
                                        if (dataScadenza != null && dataScadenza != "")
                                        {
                                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_SCADENZA_PROJECT_1");
                                            queryMng.setParam("data", dataScadenza);
                                            queryMng.setParam("idProject", idProject);
                                            commandText = queryMng.getSQL();
                                            dbProvider.ExecuteNonQuery(commandText);
                                        }
                                    }                                    
                                }
                            }
                        }
                    }
                }                               
            }
            catch
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

       
        public DocsPaVO.DiagrammaStato.Stato getStatoFasc(string idProject)
        {
            DocsPaVO.DiagrammaStato.Stato stato = null;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_FASC_1");
                queryMng.setParam("idProject", idProject);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - getStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count != 0)
                {
                    stato = new DocsPaVO.DiagrammaStato.Stato();
                    stato.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["Id_stato"].ToString());
                    stato.ID_DIAGRAMMA = Convert.ToInt32(ds.Tables[0].Rows[0]["Id_diagramma"].ToString());

                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");
                    queryMng.setParam("system_id", ds.Tables[0].Rows[0]["Id_stato"].ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getStatoFasc - DiagrammiStato.cs - QUERY : " + commandText);

                    DataSet ds_1 = new DataSet();
                    dbProvider.ExecuteQuery(ds_1, commandText);

                    stato.DESCRIZIONE = ds_1.Tables[0].Rows[0]["Var_descrizione"].ToString();
                    if (ds_1.Tables[0].Rows[0]["Stato_iniziale"].ToString() == "1")
                        stato.STATO_INIZIALE = true;
                    else
                        stato.STATO_INIZIALE = false;

                    if (ds_1.Tables[0].Rows[0]["Stato_finale"].ToString() == "1")
                        stato.STATO_FINALE = true;
                    else
                        stato.STATO_FINALE = false;

                    if (ds_1.Tables[0].Rows[0]["Conv_Pdf"].ToString() == "1")
                        stato.CONVERSIONE_PDF = true;
                    else
                        stato.CONVERSIONE_PDF = false;

                    if (ds_1.Tables[0].Rows[0]["NON_RICERCABILE"].ToString() == "1")
                        stato.NON_RICERCABILE = true;
                    else
                        stato.NON_RICERCABILE = false;
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
            return stato;
        }

       
        public void deleteStoricoTrasmDiagrammiFasc(string idProject, string idStato)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //Elimini eventuali associazioni presenti per lo specifico stato individuato dal system_id
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_DEL_TRASM_STORICO_FASC");
                queryMng.setParam("idStato", idStato);
                queryMng.setParam("idProject", idProject);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - deleteStoricoTrasmDiagrammiFasc - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - deleteStoricoTrasmDiagrammiFasc - DiagrammiStato.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
            }
            catch
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        
        public ArrayList isStatoTrasmAutoFasc(string idAmm, string idStato, string idTipoFasc)
        {
            ArrayList modelli = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            if (idStato == "")
                return modelli;

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_IS_STATO_TRASM_AUTO_FASC");
                queryMng.setParam("idStato", idStato);
                queryMng.setParam("idTipoFasc", idTipoFasc);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - isStatoTrasmAutoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - isStatoTrasmAutoFasc - DiagrammiStato.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    ModTrasmissioni modTrasm = new ModTrasmissioni();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (ds.Tables[0].Rows[i]["TRASM_AUT"].ToString() == "1")
                            modelli.Add(modTrasm.getModelloByID(idAmm, ds.Tables[0].Rows[i]["ID_MOD_TRASM"].ToString()));
                    }
                    return modelli;
                }
                return modelli;
            }
            catch
            {
                return modelli;
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        
        public void salvaStoricoTrasmDiagrammiFasc(string idTrasm, string idProject, string idStato)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {

                //Verifico se esiste già una trasmissione da controllare nella DPA_TRASM_DIAGR
                //In caso affermativo non effettuo nessuna operazione
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_ID_TRASM_DIAGR_FASC");
                queryMng.setParam("idProject", idProject);
                queryMng.setParam("idStato", idStato);
                string commandText = queryMng.getSQL();
                DataSet trasm = new DataSet();
                dbProvider.ExecuteQuery(trasm, commandText);
                if (trasm.Tables[0].Rows.Count != 0)
                    return;

                //Inserisco l'associazione stato-documento-tramissione
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_TRASM_STORICO_FASC");
                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_TRASM_DIAGR"));
                queryMng.setParam("idTrasm", idTrasm);
                queryMng.setParam("idProject", idProject);
                queryMng.setParam("idStato", idStato);
                commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - salvaStoricoTrasmDiagrammiFasc - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - salvaStoricoTrasmDiagrammiFasc - DiagrammiStato.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
            }
            catch
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        
        public void salvaDataScadenzaFasc(string idProject, string dataScadenza, string idTipoFasc)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_SCADENZE_FASC");
                queryMng.setParam("idTipoFasc", idTipoFasc);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - salvaDataScadenzaFasc - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - salvaDataScadenzaFasc - DiagrammiStato.cs - QUERY : " + commandText);
                DataSet scadenze = new DataSet();
                dbProvider.ExecuteQuery(scadenze, commandText);
                if (scadenze.Tables[0].Rows.Count != 0)
                {
                    string scadenza = scadenze.Tables[0].Rows[0]["GG_SCADENZA"].ToString();
                    string preScadenza = scadenze.Tables[0].Rows[0]["GG_PRE_SCADENZA"].ToString();

                    if (dataScadenza == "" && scadenza != "0")
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_SCADENZA_PROJECT");
                        queryMng.setParam("scadenza_gg", scadenza);
                        queryMng.setParam("idProject", idProject);
                        commandText = queryMng.getSQL();
                    }
                    else
                    {
                        if (dataScadenza != null && dataScadenza != "")
                        {
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_SCADENZA_PROJECT_1");
                            queryMng.setParam("data", dataScadenza);
                            queryMng.setParam("idProject", idProject);
                            commandText = queryMng.getSQL();
                        }
                    }
                    dbProvider.ExecuteNonQuery(commandText);
                }
            }
            catch
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        
        public void salvaDataScadenzaDoc(string docNumber, string dataScadenza, string idTipoAtto)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_SCADENZE");
                queryMng.setParam("idTipoAtto", idTipoAtto);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - salvaDataScadenzaDoc - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - salvaDataScadenzaDoc - DiagrammiStato.cs - QUERY : " + commandText);
                DataSet scadenze = new DataSet();
                dbProvider.ExecuteQuery(scadenze, commandText);
                if (scadenze.Tables[0].Rows.Count != 0)
                {
                    string scadenza = scadenze.Tables[0].Rows[0]["GG_SCADENZA"].ToString();
                    string preScadenza = scadenze.Tables[0].Rows[0]["GG_PRE_SCADENZA"].ToString();

                    if (dataScadenza == "" && scadenza != "0")
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_SCADENZA_PROFILE");
                        queryMng.setParam("scadenza_gg", scadenza);
                        queryMng.setParam("docNumber", docNumber);
                        commandText = queryMng.getSQL();
                    }
                    else
                    {
                        if (dataScadenza != null && dataScadenza != "")
                        {
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_SCADENZA_PROFILE_1");
                            queryMng.setParam("data", dataScadenza);
                            queryMng.setParam("docNumber", docNumber);
                            commandText = queryMng.getSQL();
                        }
                    }
                    dbProvider.ExecuteNonQuery(commandText);
                }
            }
            catch
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        
        public DataSet getDiagrammaStoricoFasc(string idProject)
        {
            DataSet storico = new DataSet();
            DataTable dt = new DataTable();
            DataSet storico_1 = new DataSet();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_DIAGRAMMA_STORICO_FASC");
                queryMng.setParam("idProject", idProject);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getDiagrammaStoricoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - getDiagrammaStoricoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                dbProvider.ExecuteQuery(storico, commandText);

                dt.Columns.Add("Ruolo");
                dt.Columns.Add("Utente");
                dt.Columns.Add("Data");
                dt.Columns.Add("Vecchio stato");
                dt.Columns.Add("Nuovo Stato");

                for (int i = 0; i < storico.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    Utenti ut = new Utenti();
                    if (storico.Tables[0].Rows[i]["ID_RUOLO"].ToString() != null && storico.Tables[0].Rows[i]["ID_RUOLO"].ToString() != "")
                        dr["Ruolo"] = (ut.GetRuoloEnabledAndDisabled(storico.Tables[0].Rows[i]["ID_RUOLO"].ToString())).descrizione;
                    else
                        dr["Ruolo"] = "";

                    if (storico.Tables[0].Rows[i]["ID_PEOPLE"].ToString() != null && storico.Tables[0].Rows[i]["ID_PEOPLE"].ToString() != "")
                        dr["Utente"] = (ut.GetUtenteNoFiltroDisabled(storico.Tables[0].Rows[i]["ID_PEOPLE"].ToString())).descrizione;
                    else
                        dr["Utente"] = "";

                    dr["Data"] = storico.Tables[0].Rows[i]["DTA_DATE"].ToString();
                    dr["Vecchio stato"] = storico.Tables[0].Rows[i]["VAR_DESC_OLD_STATO"].ToString();
                    dr["Nuovo Stato"] = storico.Tables[0].Rows[i]["VAR_DESC_NEW_STATO"].ToString();
                    dt.Rows.Add(dr);
                }
                storico_1.Tables.Add(dt);
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }
            return storico_1;
        }

        
        public DocsPaVO.DiagrammaStato.Stato getStatoSuccessivoAutomaticoFasc(string idProject)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                //Recupero lo stato del fascicolo e il diagramma a cui appartiene
                DocsPaVO.DiagrammaStato.Stato statoFasc = getStatoFasc(idProject);
                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = getDiagrammaById(Convert.ToString(statoFasc.ID_DIAGRAMMA));
                for (int k = 0; k < diagramma.PASSI.Count; k++)
                {
                    if (((DocsPaVO.DiagrammaStato.Passo)diagramma.PASSI[k]).STATO_PADRE.SYSTEM_ID == statoFasc.SYSTEM_ID)
                    {
                        if (((DocsPaVO.DiagrammaStato.Passo)diagramma.PASSI[k]).ID_STATO_AUTOMATICO != null || ((DocsPaVO.DiagrammaStato.Passo)diagramma.PASSI[k]).ID_STATO_AUTOMATICO != "")
                        {
                            //Recupero i dati dello stato automatico
                            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_STATO_BY_ID");
                            queryMng.setParam("system_id", ((DocsPaVO.DiagrammaStato.Passo)diagramma.PASSI[k]).ID_STATO_AUTOMATICO);
                            string commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - getStatoSuccessivoAutomaticoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                            logger.Debug("SQL - getStatoSuccessivoAutomaticoFasc - DiagrammiStato.cs - QUERY : " + commandText);
                            DataSet ds_4 = new DataSet();
                            dbProvider.ExecuteQuery(ds_4, commandText);

                            DocsPaVO.DiagrammaStato.Stato st_auto = new DocsPaVO.DiagrammaStato.Stato();
                            st_auto.SYSTEM_ID = Convert.ToInt32(ds_4.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                            st_auto.ID_DIAGRAMMA = Convert.ToInt32(ds_4.Tables[0].Rows[0]["ID_DIAGRAMMA"].ToString());
                            st_auto.DESCRIZIONE = ds_4.Tables[0].Rows[0]["Var_descrizione"].ToString();
                            if (ds_4.Tables[0].Rows[0]["Stato_iniziale"].ToString() == "0")
                                st_auto.STATO_INIZIALE = false;
                            else
                                st_auto.STATO_INIZIALE = true;

                            if (ds_4.Tables[0].Rows[0]["Stato_finale"].ToString() == "0")
                                st_auto.STATO_FINALE = false;
                            else
                                st_auto.STATO_FINALE = true;

                            if (ds_4.Tables[0].Rows[0]["Conv_Pdf"].ToString() == "1")
                                st_auto.CONVERSIONE_PDF = true;
                            else
                                st_auto.CONVERSIONE_PDF = false;

                            if (ds_4.Tables[0].Rows[0]["NON_RICERCABILE"].ToString() == "1")
                                st_auto.NON_RICERCABILE = true;
                            else
                                st_auto.NON_RICERCABILE = false;

                            return st_auto;
                        }
                    }
                }
                return null;
            }
            catch
            {
                dbProvider.RollbackTransaction();
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public ArrayList getStatiPerRicerca(string idDiagramma, string docOrFasc)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            ArrayList stati = new ArrayList();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("GET_STATI_DG_PER_RICERCA");
                queryMng.setParam("idDiagramma", idDiagramma);
                if(docOrFasc.ToUpper().Equals("D"))
                    queryMng.setParam("docOrFasc", " and dpa_diagrammi.doc_number is not null ");
                if(docOrFasc.ToUpper().Equals("F"))
                    queryMng.setParam("docOrFasc", " and dpa_diagrammi.id_project is not null ");

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getStatiPerRicerca - DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - getStatiPerRicerca - DiagrammiStato.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.DiagrammaStato.Stato stato = new DocsPaVO.DiagrammaStato.Stato();
                        stato.SYSTEM_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID_STATO"].ToString());
                        stato.DESCRIZIONE = ds.Tables[0].Rows[i]["VAR_DESCRIZIONE"].ToString();

                        stati.Add(stato);
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
            return stati;
        }

        #region Visibilità ruolo /stati diagramma
        public List<DocsPaVO.DiagrammaStato.Visibility.AssRuoloStatiDiagramma> GetRuoliStatiDiagramma(int idDiagramma)
        {
            List<DocsPaVO.DiagrammaStato.Visibility.AssRuoloStatiDiagramma> listAssRuoloStatiDiag = new List<DocsPaVO.DiagrammaStato.Visibility.AssRuoloStatiDiagramma>();
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_ASS_RUOLI_STATI_DIAGRAMMA");
                query.setParam("idDiagramma",idDiagramma.ToString());
                string commandText = query.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - Select Associazione Ruoli diagramma di stato - DocsPaDB.Query_DocsPAWS.DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - GetRuoliStatiDiagramma - DocsPaDB.Query_DocsPAWS.DiagrammiStato.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        listAssRuoloStatiDiag.Add(new DocsPaVO.DiagrammaStato.Visibility.AssRuoloStatiDiagramma{
                            ID_GRUPPO = ds.Tables[0].Rows[i]["ID_GRUPPO"].ToString(),
                            ID_DIAGRAMMA = ds.Tables[0].Rows[i]["ID_DIAGRAMMA"].ToString(),
                            ID_STATO = ds.Tables[0].Rows[i]["ID_STATO"].ToString(),
                            CHA_NOT_VIS = ds.Tables[0].Rows[i]["CHA_NOT_VIS"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - GetRuoliStatiDiagramma - DocsPaDB.Query_DocsPAWS.DiagrammiStato.cs - Exception : " + ex.Message);
            }
            return listAssRuoloStatiDiag;
        }

        private DocsPaUtils.Data.ParameterSP CreateParameter(string name, object value)
        {
            return new DocsPaUtils.Data.ParameterSP(name, value);
        }

        public bool ModifyAssRuoloStatiDiagramma(List<DocsPaVO.DiagrammaStato.Visibility.AssRuoloStatiDiagramma> ListAssRoliStatiDiagramma)
        {
            int retProc = 0;
            this.BeginTransaction();
            try
            {
                if (databaseProvider.DBType.ToUpper().Equals("ORACLE"))
                {
                   
                    foreach (DocsPaVO.DiagrammaStato.Visibility.AssRuoloStatiDiagramma assRoleStatoDia in ListAssRoliStatiDiagramma)
                    {
                        // Creazione parametri per la Store Procedure
                        ArrayList parameters = new ArrayList();
                        parameters.Add(this.CreateParameter("idGruppo", assRoleStatoDia.ID_GRUPPO));
                        parameters.Add(this.CreateParameter("idStato", assRoleStatoDia.ID_STATO));
                        parameters.Add(this.CreateParameter("idDiagramma", assRoleStatoDia.ID_DIAGRAMMA));
                        parameters.Add(this.CreateParameter("chaNotVis", assRoleStatoDia.CHA_NOT_VIS));

                        //parameters.Add(new ParameterSP("idGruppo", Convert.ToInt32(assRoleStatoDia.ID_GRUPPO), 0, DirectionParameter.ParamInput, DbType.Int32));
                        //parameters.Add(new ParameterSP("idStato", Convert.ToInt32(assRoleStatoDia.ID_STATO), 0, DirectionParameter.ParamInput, DbType.Int32));
                        //parameters.Add(new ParameterSP("idDiagramma", Convert.ToInt32(assRoleStatoDia.ID_DIAGRAMMA), 0, DirectionParameter.ParamInput, DbType.Int32));
                        //parameters.Add(new ParameterSP("chaNotVis", Convert.ToInt32(assRoleStatoDia.CHA_NOT_VIS), 0, DirectionParameter.ParamInput, DbType.Int32));
                        //parameters.Add(new ParameterSP("result", 0, DirectionParameter.ParamOutput));
                       //retProc = this.ExecuteStoreProcedure("MODIFYASSRUOLOSTATODIAGRAMMA", parameters);
                       //retProc = this.ExecuteStoredProcedure("MODIFYASSRUOLOSTATODIAGRAMMA", parameters, null);

                        DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                        DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("COUNT_DIAGRAMMI");
                        query.setParam("idDiagramma", assRoleStatoDia.ID_DIAGRAMMA);
                        query.setParam("idGruppo", assRoleStatoDia.ID_GRUPPO);
                        query.setParam("idStato", assRoleStatoDia.ID_STATO);
                        string commandText = query.getSQL();
                        string count = string.Empty;
                        dbProvider.ExecuteScalar(out count, commandText);

                        if (!string.IsNullOrEmpty(count))
                        {
                            if (!count.Equals("0"))
                            {
                                if (assRoleStatoDia.CHA_NOT_VIS.Equals("0"))
                                {
                                    query = DocsPaUtils.InitQuery.getInstance().getQuery("DELETE_DIAGRAMMI");
                                    query.setParam("idDiagramma", assRoleStatoDia.ID_DIAGRAMMA);
                                    query.setParam("idGruppo", assRoleStatoDia.ID_GRUPPO);
                                    query.setParam("idStato", assRoleStatoDia.ID_STATO);
                                    commandText = query.getSQL();
                                    if (dbProvider.ExecuteNonQuery(commandText))
                                        retProc = 1;
                                }
                                else
                                {
                                    query = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_DIAGRAMMI");
                                    query.setParam("idDiagramma", assRoleStatoDia.ID_DIAGRAMMA);
                                    query.setParam("idGruppo", assRoleStatoDia.ID_GRUPPO);
                                    query.setParam("idStato", assRoleStatoDia.ID_STATO);
                                    commandText = query.getSQL();
                                    if (dbProvider.ExecuteNonQuery(commandText))
                                        retProc = 2;
                                }
                            }
                            else
                            {
                                if (assRoleStatoDia.CHA_NOT_VIS.Equals("1"))
                                {
                                    query = DocsPaUtils.InitQuery.getInstance().getQuery("INSERT_DIAGRAMMI");
                                    query.setParam("idDiagramma", assRoleStatoDia.ID_DIAGRAMMA);
                                    query.setParam("idGruppo", assRoleStatoDia.ID_GRUPPO);
                                    query.setParam("idStato", assRoleStatoDia.ID_STATO);
                                    commandText = query.getSQL();
                                    if (dbProvider.ExecuteNonQuery(commandText))
                                        retProc = 3;
                                }
                            }
                        }

                        if (retProc != 0 && retProc != 1 && retProc != 2 && retProc != 3)
                        {
                            this.RollbackTransaction();
                            return false;
                        }
                    }
                }
                this.CommitTransaction();
                
                return true;
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - Stored Procedure : MODIFYASSRUOLOSTATODIAGRAMMA - DocsPaDB.Query_DocsPAWS.DiagrammiStato.cs - Exception : " + ex.Message);
                this.RollbackTransaction();
                return false;
            }
        }

        public bool IsAssociatoRuoloDiagramma(string idDiagramma, string idRuolo)
        {
            try
            {
                string result = string.Empty;
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_ASS_RUOLO_DIAGRAMMA");
                query.setParam("idDiagramma", idDiagramma.ToString());
                query.setParam("idRuolo", idRuolo.ToString());
                string commandText = query.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - Select Associazione Ruolo diagaramma - DocsPaDB.Query_DocsPAWS.DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - IsAssociatoRuoloDiagramma - DocsPaDB.Query_DocsPAWS.DiagrammiStato.cs - QUERY : " + commandText);
                dbProvider.ExecuteScalar(out result, commandText);
                if (!string.IsNullOrEmpty(result) && Convert.ToInt32(result) < 1)
                    return true;
                else return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool IsAssociatoRuoloStatoDia(string idDiagramma, string idRuolo)
        {
            try
            {
                string result = string.Empty;
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_ASS_RUOLO_DIAGRAMMA");
                query.setParam("idDiagramma", idDiagramma.ToString());
                query.setParam("idRuolo", idRuolo.ToString());
                string commandText = query.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - Select Associazione Ruolo diagaramma - DocsPaDB.Query_DocsPAWS.DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - IsAssociatoRuoloDiagramma - DocsPaDB.Query_DocsPAWS.DiagrammiStato.cs - QUERY : " + commandText);
                dbProvider.ExecuteScalar(out result, commandText);
                if (string.IsNullOrEmpty(result) || Convert.ToInt32(result) == 0)
                    return true;
                else return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool IsAssociatoRuoloStatoDia(string idDiagramma, string idRuolo, string idStato)
        {
            try
            {
                string result = string.Empty;
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_ASS_RUOLO_STATO_DIAGRAMMA");
                query.setParam("idDiagramma", idDiagramma.ToString());
                query.setParam("idRuolo", idRuolo.ToString());
                query.setParam("idStato", idStato.ToString());
                string commandText = query.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - Select Associazione Ruolo stato del diagaramma - DocsPaDB.Query_DocsPAWS.DiagrammiStato.cs - QUERY : " + commandText);
                logger.Debug("SQL - IsAssociatoRuoloStatoDia - DocsPaDB.Query_DocsPAWS.DiagrammiStato.cs - QUERY : " + commandText);
                dbProvider.ExecuteScalar(out result, commandText);
                if (string.IsNullOrEmpty(result) || Convert.ToInt32(result) == 0)
                    return true;
                else return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public List<DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma> GetFasiStatiDiagramma(string idDiagramma, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma> faseStatiDiagramma = new List<DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma>();
            string query = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ASSOCIAZIONE_STATI_PHASES");
                    q.setParam("idDiagramma", idDiagramma);

                    query = q.getSQL();
                    logger.Debug("GetFasiStatiDiagramma: " + query);
                    if (dbProvider.ExecuteQuery(out ds, "fasi", query))
                    {
                        if (ds.Tables["fasi"] != null && ds.Tables["fasi"].Rows.Count > 0)
                        {
                            DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma faseStato;
                            foreach (DataRow row in ds.Tables["fasi"].Rows)
                            {
                                faseStato = new DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma();
                                faseStato.INTERMEDIATE_PHASE = row["CHA_INTERMEDIATE_PHASE"].ToString() == "0" ? false : true;
                                faseStato.POSITION_PHASE = row["POSITION_PHASE"].ToString();
                                faseStato.STATO = new DocsPaVO.DiagrammaStato.Stato()
                                {
                                    SYSTEM_ID =Convert.ToInt32(row["ID_STATO"].ToString()),
                                    STATO_INIZIALE = row["STATO_INIZIALE"].ToString() == "0" ? false : true
                                };
                                faseStato.PHASE = new DocsPaVO.DiagrammaStato.Phases()
                                {
                                    SYSTEM_ID = row["ID_PHASES"].ToString(),
                                    DESCRIZIONE = row["DESCRIZIONE_PHASE"].ToString()
                                };
                                faseStatiDiagramma.Add(faseStato);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetFasiStatiDiagramma " + e.Message);
                return null;
            }

            return faseStatiDiagramma;
        }

        /// <summary>
        /// Restituisce la fase il cui id passato in input e rispettivi stati del diagramma
        /// </summary>
        /// <param name="idDiagramma"></param>
        /// <param name="idFase"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public List<DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma> GetFaseDiagrammaByIdFase(string idDiagramma, string idFase, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma> faseStatiDiagramma = new List<DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma>();
            string query = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ASSOCIAZIONE_STATI_PHASES_BY_ID_FASE");
                    q.setParam("idDiagramma", idDiagramma);
                    q.setParam("idFase", idFase);

                    query = q.getSQL();
                    logger.Debug("GetFasiStatiDiagramma: " + query);
                    if (dbProvider.ExecuteQuery(out ds, "fasi", query))
                    {
                        if (ds.Tables["fasi"] != null && ds.Tables["fasi"].Rows.Count > 0)
                        {
                            DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma faseStato;
                            foreach (DataRow row in ds.Tables["fasi"].Rows)
                            {
                                faseStato = new DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma();
                                faseStato.INTERMEDIATE_PHASE = row["CHA_INTERMEDIATE_PHASE"].ToString() == "0" ? false : true;
                                faseStato.POSITION_PHASE = row["POSITION_PHASE"].ToString();

                                faseStato.STATO = new DocsPaVO.DiagrammaStato.Stato()
                                {
                                    SYSTEM_ID = Convert.ToInt32(row["ID_STATO"].ToString()),
                                    STATO_INIZIALE = row["STATO_INIZIALE"].ToString() == "0" ? false : true,
                                    DESCRIZIONE = row["DESCRIZIONE_STATO"].ToString()
                                };
                                faseStato.PHASE = new DocsPaVO.DiagrammaStato.Phases()
                                {
                                    SYSTEM_ID = row["ID_PHASES"].ToString(),
                                    DESCRIZIONE = row["DESCRIZIONE_PHASE"].ToString()
                                };
                                faseStatiDiagramma.Add(faseStato);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetFaseDiagrammaByIdFase " + e.Message);
                return null;
            }

            return faseStatiDiagramma;
        }

        public DocsPaVO.DiagrammaStato.Stato GetStatoById(string idStato, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.DiagrammaStato.Stato state = new DocsPaVO.DiagrammaStato.Stato();
            string query = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_STATI_BY_ID");
                    q.setParam("idStato", idStato);

                    query = q.getSQL();
                    logger.Debug("GetStatoById: " + query);
                    if (dbProvider.ExecuteQuery(out ds, "state", query))
                    {
                        if (ds.Tables["state"] != null && ds.Tables["state"].Rows.Count > 0)
                        {
                            DataRow row = ds.Tables["state"].Rows[0];
                            state.SYSTEM_ID = Convert.ToInt32(row["SYSTEM_ID"].ToString());
                            state.DESCRIZIONE = row["VAR_DESCRIZIONE"].ToString();
                            state.STATO_INIZIALE = row["Stato_iniziale"].ToString() == "1";
                            state.STATO_FINALE = row["Stato_finale"].ToString() == "1";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetStatoById " + e.Message);
                return null;
            }

            return state;
        }
        #endregion

        public ArrayList GetTrasmissioniStatoFasc(string idFascicolo, string idStato)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            ArrayList idTrasm = new ArrayList();

            try
            {
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_STATO_FASC_TRASMISSIONI");
                query.setParam("idFasc", idFascicolo);
                query.setParam("idStato", idStato);

                string command = query.getSQL();
                logger.Debug(command);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, command);

                if (ds.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        idTrasm.Add(row["ID_TRASM"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.DebugFormat(ex.Message, ex.StackTrace);
                return null;
            }

            return idTrasm;
        }

        public List<string> GetRagioniCambioStato(string idStato)
        {
            logger.Debug("BEGIN");
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            List<string> result = new List<string>();

            try
            {
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_STATO_RAGIONI");
                query.setParam("idStato", idStato);

                string command = query.getSQL();
                logger.Debug(command);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, command);

                if (ds.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        result.Add(row["VAR_DESC_RAGIONE"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.DebugFormat(ex.Message, ex.StackTrace);
                return null;
            }

            logger.Debug("END");
            return result;
        }

        public ArrayList GetIdDocsByDiagramStatus(string descStato, string descDiagramma, string codAmm) 
        {
            ArrayList retVal = new ArrayList();
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_IDDOCS_BY_DIAGRAMSTATUS");
                    query.setParam("descStato", descStato.Replace("'","''"));
                    query.setParam("descDiagramma",descDiagramma);
                    query.setParam("codAmm", codAmm);
                    string command = query.getSQL();
                    logger.Debug(command);
                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, command);

                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            retVal.Add(row["DOC_NUMBER"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }

            return retVal;
        }

    }
}
