using System;
using System.Collections;
using System.ComponentModel;
using System.Web.Services;
using System.Xml.Serialization;
using System.Xml;
using System.Configuration;
using System.IO;
using System.Data;
using System.Threading;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    public class IndiceSistematico
    {
        private ILog logger = LogManager.GetLogger(typeof(IndiceSistematico));
        public IndiceSistematico() { }

        public void addNuovaVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice)
        {
            DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("INDS_INSERT_INDX_SIS");
                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_INDX_SIS"));
				queryMng.setParam("idAmm",voceIndice.idAmm);
				queryMng.setParam("voceIndice",voceIndice.voceIndice);
				string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - addNuovaVoceIndice - IndiceSistematico.cs - QUERY : " + commandText);
                logger.Debug("SQL - addNuovaVoceIndice - IndiceSistematico.cs - QUERY : " + commandText);
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

        public void removeVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("INDS_DELETE_INDX_SIS");
                queryMng.setParam("systemId", voceIndice.systemId);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - removeVoceIndice - IndiceSistematico.cs - QUERY : " + commandText);
                logger.Debug("SQL - removeVoceIndice - IndiceSistematico.cs - QUERY : " + commandText);
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

        public ArrayList getIndiceByIdAmm(string idAmm)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("INDS_GET_BY_IDAMM");
                queryMng.setParam("idAmm", idAmm);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getIndiceByIdAmm - IndiceSistematico.cs - QUERY : " + commandText);
                logger.Debug("SQL - getIndiceByIdAmm - IndiceSistematico.cs - QUERY : " + commandText);
				DataSet dsIndice = new DataSet();
                dbProvider.ExecuteQuery(dsIndice,commandText);
                ArrayList indice = new ArrayList();

                if (dsIndice.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < dsIndice.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice = new DocsPaVO.fascicolazione.VoceIndiceSistematico();
                        voceIndice.systemId = dsIndice.Tables[0].Rows[i]["SYSTEM_ID"].ToString();
                        voceIndice.idAmm = dsIndice.Tables[0].Rows[i]["ID_AMM"].ToString();
                        voceIndice.voceIndice = dsIndice.Tables[0].Rows[i]["VOCE_INDICE"].ToString();

                        indice.Add(voceIndice);
                    }
                }

                return indice;
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

        public void associaVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("INDS_INSERT_ASS_INDX_SIS");
                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_INDX_SIS"));
                queryMng.setParam("idProject", voceIndice.idProject);
                queryMng.setParam("idIndiceSis", voceIndice.systemId);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - associaVoceIndice - IndiceSistematico.cs - QUERY : " + commandText);
                logger.Debug("SQL - associaVoceIndice - IndiceSistematico.cs - QUERY : " + commandText);
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

        public void dissociaVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("INDS_DELETE_ASS_INDX_SIS");
                queryMng.setParam("idIndiceSis", voceIndice.systemId);
                queryMng.setParam("idProject", voceIndice.idProject);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - dissociaVoceIndice - IndiceSistematico.cs - QUERY : " + commandText);
                logger.Debug("SQL - dissociaVoceIndice - IndiceSistematico.cs - QUERY : " + commandText);
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

        public ArrayList getIndiceByIdProject(string idProject)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("INDS_GET_BY_IDPROJECT");
                queryMng.setParam("idProject", idProject);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getIndiceByIdProject - IndiceSistematico.cs - QUERY : " + commandText);
                logger.Debug("SQL - getIndiceByIdProject - IndiceSistematico.cs - QUERY : " + commandText);
				DataSet dsIndice = new DataSet();
                dbProvider.ExecuteQuery(dsIndice, commandText);
                ArrayList indice = new ArrayList();

                if (dsIndice.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < dsIndice.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice = new DocsPaVO.fascicolazione.VoceIndiceSistematico();
                        voceIndice.systemId = dsIndice.Tables[0].Rows[i]["SYSTEM_ID"].ToString();
                        voceIndice.idAmm = dsIndice.Tables[0].Rows[i]["ID_AMM"].ToString();
                        voceIndice.voceIndice = dsIndice.Tables[0].Rows[i]["VOCE_INDICE"].ToString();
                        voceIndice.idProject = dsIndice.Tables[0].Rows[i]["ID_PROJECT"].ToString();
                        indice.Add(voceIndice);
                    }
                }

                return indice;
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

        public ArrayList getCodNodiByIndice(string indice, string idTitolario)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("INDS_GET_NODI_BY_INDICE");
                queryMng.setParam("indice", indice.Replace("'","''"));
                if (idTitolario.IndexOf(",") != 0)
                {
                    queryMng.setParam("titolario", " AND PROJECT.ID_TITOLARIO in(" + idTitolario + ") ");
                }
                else
                {
                    queryMng.setParam("titolario", " AND PROJECT.ID_TITOLARIO = " + idTitolario + " ");
                }                
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getCodNodoByIndice - IndiceSistematico.cs - QUERY : " + commandText);
                logger.Debug("SQL - getCodNodoByIndice - IndiceSistematico.cs - QUERY : " + commandText);
                DataSet dsNodi = new DataSet();
                dbProvider.ExecuteQuery(dsNodi, commandText);
                ArrayList nodiDaIndice = new ArrayList();

                if (dsNodi.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < dsNodi.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.fascicolazione.VoceIndiceSistematico voce = new DocsPaVO.fascicolazione.VoceIndiceSistematico();
                        voce.voceIndice = indice;
                        voce.idProject = dsNodi.Tables[0].Rows[i]["SYSTEM_ID"].ToString();
                        voce.idTitolario = dsNodi.Tables[0].Rows[i]["ID_TITOLARIO"].ToString();
                        voce.idAmm = dsNodi.Tables[0].Rows[i]["ID_AMM"].ToString();
                        voce.codiceNodo = dsNodi.Tables[0].Rows[i]["VAR_CODICE"].ToString();
                        voce.descrizioneNodo = dsNodi.Tables[0].Rows[i]["DESCRIPTION"].ToString();
                        nodiDaIndice.Add(voce);
                    }
                }

                return nodiDaIndice;
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

        public DocsPaVO.fascicolazione.VoceIndiceSistematico existVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("INDS_EXSIST_VOCE_INDICE");
                queryMng.setParam("voceIndice", voceIndice.voceIndice);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - existVoceIndice - IndiceSistematico.cs - QUERY : " + commandText);
                logger.Debug("SQL - existVoceIndice - IndiceSistematico.cs - QUERY : " + commandText);
                DataSet dsVoceIndice = new DataSet();
                dbProvider.ExecuteQuery(dsVoceIndice, commandText);

                if (dsVoceIndice.Tables[0].Rows.Count != 0)
                {
                    DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndiceRicercato = new DocsPaVO.fascicolazione.VoceIndiceSistematico();
                    voceIndiceRicercato.voceIndice = dsVoceIndice.Tables[0].Rows[0]["VOCE_INDICE"].ToString();
                    voceIndiceRicercato.systemId = dsVoceIndice.Tables[0].Rows[0]["SYSTEM_ID"].ToString();
                    return voceIndiceRicercato;
                }
                else
                {
                    return null;
                }
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

        public bool isAssociataVoce(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice, bool suSingoloNodo)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("INDS_IS_ASSOCIATA_VOCE");
                if (suSingoloNodo)
                {
                    queryMng.setParam("idProject",  " ID_PROJECT = " + voceIndice.idProject + " AND ");
                    queryMng.setParam("idIndice",   "ID_INDICE_SIS = " + voceIndice.systemId);
                }
                else
                {
                    queryMng.setParam("idProject", "");
                    queryMng.setParam("idIndice", "ID_INDICE_SIS = " + voceIndice.systemId);
                }               
                
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - isAssociataVoce - IndiceSistematico.cs - QUERY : " + commandText);
                logger.Debug("SQL - isAssociataVoce - IndiceSistematico.cs - QUERY : " + commandText);
                DataSet dsVoceIndice = new DataSet();
                dbProvider.ExecuteQuery(dsVoceIndice, commandText);

                if (dsVoceIndice.Tables[0].Rows.Count != 0)
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
    }
}
