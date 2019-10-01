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
    public class TimestampDoc
    {
        private ILog logger = LogManager.GetLogger(typeof(TimestampDoc));
        public void saveTSR(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.areaConservazione.OutputResponseMarca resultMarca, DocsPaVO.documento.FileRequest fileRequest)
        {
            DocsPaDB.DBProvider dbProvider = new DBProvider();

            try
            {
                if (resultMarca != null)
                    logger.Debug("Marca non nulla");
                else
                    logger.Debug("Marca nulla");
                if(fileRequest != null)
                    logger.Debug("FileRequest non nullo");
                else
                    logger.Debug("FileRequest nullo");

                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_TIMESTAMP_DOC");
                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_TIMESTAMP_DOC"));
                if (!string.IsNullOrEmpty(fileRequest.docNumber))
                    queryMng.setParam("docNumber",fileRequest.docNumber);
                else
                    queryMng.setParam("docNumber", "");

                if (!string.IsNullOrEmpty(fileRequest.versionId))
                    queryMng.setParam("versionId",fileRequest.versionId);
                else
                    queryMng.setParam("versionId", "");

                if (!string.IsNullOrEmpty(infoUtente.idPeople))    
                    queryMng.setParam("idPeople",infoUtente.idPeople);
                else
                    queryMng.setParam("idPeople", "");

                if (!string.IsNullOrEmpty(resultMarca.docm_date))
                    queryMng.setParam("dataCreazione", DocsPaDbManagement.Functions.Functions.ToDate(resultMarca.docm_date));
                else
                    queryMng.setParam("dataCreazione", "");

                if (!string.IsNullOrEmpty(resultMarca.dsm))
                    queryMng.setParam("dataScadenza",DocsPaDbManagement.Functions.Functions.ToDate(resultMarca.dsm));
                else
                    queryMng.setParam("dataScadenza", "");
                
                if(!string.IsNullOrEmpty(resultMarca.sernum))
                    queryMng.setParam("numSerie",resultMarca.sernum);
                else
                    queryMng.setParam("numSerie", "");

                if (!string.IsNullOrEmpty(resultMarca.snCertificato))
                    queryMng.setParam("snCertificato",resultMarca.snCertificato);
                else
                    queryMng.setParam("snCertificato","");

                if (!string.IsNullOrEmpty(resultMarca.algHash))
                    queryMng.setParam("algHash", resultMarca.algHash.Replace("'", "''"));
                else
                    queryMng.setParam("algHash", "SHA256");

                if (!string.IsNullOrEmpty(resultMarca.TSA.O))
                    queryMng.setParam("soggetto", resultMarca.TSA.O.Replace("'", "''"));
                else
                    queryMng.setParam("soggetto", "");

                if (!string.IsNullOrEmpty(resultMarca.TSA.C))
                    queryMng.setParam("paese", resultMarca.TSA.C);
                else
                    queryMng.setParam("paese", "");

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - saveTSR - DocsPaDB/TimestampDoc.cs - QUERY : " + commandText);
                logger.Debug("SQL - saveTSR - DocsPaDB/TimestampDoc.cs - QUERY : " + commandText);

                dbProvider.BeginTransaction();
                dbProvider.ExecuteNonQuery(commandText);

                string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_TIMESTAMP_DOC");
                string id = string.Empty;
                System.Diagnostics.Debug.WriteLine("SQL - saveTSR - DocsPaDB/TimestampDoc.cs - QUERY : " + sql);
                logger.Debug("SQL - saveTSR - DocsPaDB/TimestampDoc.cs - QUERY : " + sql);
                dbProvider.ExecuteScalar(out id, sql);
                if (!string.IsNullOrEmpty(id))
                {
                    dbProvider.SetLargeText("DPA_TIMESTAMP_DOC", id, "TSR_FILE", resultMarca.marca); 
                }
                dbProvider.CommitTransaction();
            }
            catch(Exception ex)
            {
                logger.Debug("SQL - setTimestamp - DocsPaDB/TimestampDoc.cs - Exception : " + ex.Message);
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public ArrayList getTimestampsDoc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fileRequest)
        {
            DocsPaDB.DBProvider dbProvider = new DBProvider();
            ArrayList timestampsDoc = new ArrayList();

            try
            {
                if (fileRequest != null && !string.IsNullOrEmpty(fileRequest.versionId) && !string.IsNullOrEmpty(fileRequest.docNumber))
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TIMESTAMP_DOC");
                    queryMng.setParam("versionId", fileRequest.versionId);
                    queryMng.setParam("docNumber", fileRequest.docNumber);

                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTimestampsDoc - DocsPaDB/TimestampDoc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTimestampsDoc - DocsPaDB/TimestampDoc.cs - QUERY : " + commandText);
                    DataSet dataSet = new DataSet();
                    dbProvider.ExecuteQuery(dataSet, commandText);

                    if (dataSet.Tables[0].Rows.Count != 0)
                    {
                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                        {
                            DocsPaVO.documento.TimestampDoc timestampDoc = new DocsPaVO.documento.TimestampDoc();
                            setTimestamp(ref timestampDoc, dataSet, i);
                            timestampsDoc.Add(timestampDoc);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - setTimestamp - DocsPaDB/TimestampDoc.cs - Exception : " + ex.Message);
                dbProvider.RollbackTransaction();
            }

            return timestampsDoc;
            
        }

        /// <summary>
        /// Ritorna il numero di timestamp associati al documento (il documento è identificato tramite DocNumber e VersionID)
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public int getCountTimestampsDoc(DocsPaVO.utente.InfoUtente infoUtente, string docNumber, string versionId)
        {
            DocsPaDB.DBProvider dbProvider = new DBProvider();
            int numTimestamp = 0;

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_COUNT_DPA_TIMESTAMP_DOC");
                queryMng.setParam("versionId", versionId);
                queryMng.setParam("docNumber", docNumber);

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getCountTimestampsDoc - DocsPaDB/TimestampDoc.cs - QUERY : " + commandText);
                logger.Debug("SQL - getCountTimestampsDoc - DocsPaDB/TimestampDoc.cs - QUERY : " + commandText);
                DataSet dataSet = new DataSet();
                dbProvider.ExecuteQuery(dataSet, commandText);

                if (dataSet.Tables[0].Rows.Count > 0 && dataSet.Tables[0].Columns.Contains("NumTimestamp"))
                {
                    numTimestamp = int.Parse(dataSet.Tables[0].Rows[0]["NumTimestamp"].ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - getCountTimestampsDoc - DocsPaDB/TimestampDoc.cs - Exception : " + ex.Message);
                
            }

            return numTimestamp;

        }

        /// <summary>
        /// Prelievo dei timestamp a partire dal docnumber. I timestamp sono riferiti solo all'ultima versione (funzione getmaxver)
        /// </summary>
        /// <param name="docnumber"></param>
        /// <returns></returns>
        public ArrayList getTimestampDocLastVersionLite(string docnumber)
        {
            DocsPaDB.DBProvider dbProvider = new DBProvider();
            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
            ArrayList timestampsDoc = new ArrayList();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TIMESTAMP_DOC_LASTVER_LITE");
                queryMng.setParam("docNumber", docnumber);
                if (dbType.ToUpper() == "SQL")
                {
                    queryMng.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                }
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTimestampsDoc - DocsPaDB/TimestampDoc.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTimestampsDoc - DocsPaDB/TimestampDoc.cs - QUERY : " + commandText);
                DataSet dataSet = new DataSet();
                dbProvider.ExecuteQuery(dataSet, commandText);

                if (dataSet.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.documento.TimestampDoc timestampDoc = new DocsPaVO.documento.TimestampDoc();
                        setTimestamp(ref timestampDoc, dataSet, i);
                        timestampsDoc.Add(timestampDoc);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - setTimestamp - DocsPaDB/TimestampDoc.cs - Exception : " + ex.Message);
                dbProvider.RollbackTransaction();
            }
            return timestampsDoc;
        }

        private void setTimestamp(ref DocsPaVO.documento.TimestampDoc timestampDoc, DataSet dataSet, int rowNumber)
        {
            try
            {
                if (dataSet.Tables[0].Columns.Contains("SYSTEM_ID"))
                    timestampDoc.SYSTEM_ID = dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID"].ToString();
                if (dataSet.Tables[0].Columns.Contains("DOC_NUMBER"))
                    timestampDoc.DOC_NUMBER = dataSet.Tables[0].Rows[rowNumber]["DOC_NUMBER"].ToString();
                if (dataSet.Tables[0].Columns.Contains("VERSION_ID"))
                    timestampDoc.VERSION_ID = dataSet.Tables[0].Rows[rowNumber]["VERSION_ID"].ToString();
                if (dataSet.Tables[0].Columns.Contains("ID_PEOPLE"))
                    timestampDoc.ID_PEOPLE = dataSet.Tables[0].Rows[rowNumber]["ID_PEOPLE"].ToString();
                if (dataSet.Tables[0].Columns.Contains("DTA_CREAZIONE"))
                    timestampDoc.DTA_CREAZIONE = dataSet.Tables[0].Rows[rowNumber]["DTA_CREAZIONE"].ToString();
                if (dataSet.Tables[0].Columns.Contains("DTA_SCADENZA"))
                    timestampDoc.DTA_SCADENZA = dataSet.Tables[0].Rows[rowNumber]["DTA_SCADENZA"].ToString();
                if (dataSet.Tables[0].Columns.Contains("NUM_SERIE"))
                    timestampDoc.NUM_SERIE = dataSet.Tables[0].Rows[rowNumber]["NUM_SERIE"].ToString();
                if (dataSet.Tables[0].Columns.Contains("S_N_CERTIFICATO"))
                    timestampDoc.S_N_CERTIFICATO = dataSet.Tables[0].Rows[rowNumber]["S_N_CERTIFICATO"].ToString();
                if (dataSet.Tables[0].Columns.Contains("ALG_HASH"))
                    timestampDoc.ALG_HASH = dataSet.Tables[0].Rows[rowNumber]["ALG_HASH"].ToString();
                if (dataSet.Tables[0].Columns.Contains("SOGGETTO"))
                    timestampDoc.SOGGETTO = dataSet.Tables[0].Rows[rowNumber]["SOGGETTO"].ToString();
                if (dataSet.Tables[0].Columns.Contains("PAESE"))
                    timestampDoc.PAESE = dataSet.Tables[0].Rows[rowNumber]["PAESE"].ToString();
                if (dataSet.Tables[0].Columns.Contains("TSR_FILE"))
                    timestampDoc.TSR_FILE = dataSet.Tables[0].Rows[rowNumber]["TSR_FILE"].ToString();
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - setTimestamp - DocsPaDB/TimestampDoc.cs - Exception : " + ex.Message);
            }
        }
    }
}
