// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;


namespace DocsPaDB.Query_DocsPAWS;

public class DocumentMetadata : DBProvider
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(DocumentMetadata));

    public List<DocsPaVO.DocumentMetadata.EventDocument> GetEventDocument()
    {
        logger.Debug("INIZIO Metodo GetEventDocument in DocsPaDb.Query_DocsPAWS.DocumentMetadata");
        List<DocsPaVO.DocumentMetadata.EventDocument> eventDocuments = new List<DocsPaVO.DocumentMetadata.EventDocument>();
        DocsPaVO.DocumentMetadata.EventDocument eventDocument = null;
        try
        {
            DataSet ds = new DataSet();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_EVENT_DOCUMENT");
            string query = q.getSQL();
            logger.Debug("GetEventDocument: " + query);

            bool result = true;
            using (DBProvider dbProvider = new DBProvider())
                result = dbProvider.ExecuteQuery(out ds, "eventDocument", query);

            if (result)
            {
                if (ds.Tables["eventDocument"] != null && ds.Tables["eventDocument"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["eventDocument"].Rows)
                    {
                        eventDocument = new DocsPaVO.DocumentMetadata.EventDocument()
                        {
                            SystemId = !string.IsNullOrEmpty(row["SYSTEM_ID"].ToString()) ? row["SYSTEM_ID"].ToString() : string.Empty,
                            IdPeopleOperatore = !string.IsNullOrEmpty(row["ID_PEOPLE_OPERATORE"].ToString()) ? row["ID_PEOPLE_OPERATORE"].ToString() : string.Empty,
                            IdGruppoOperatore = !string.IsNullOrEmpty(row["ID_GRUPPO_OPERATORE"].ToString()) ? row["ID_GRUPPO_OPERATORE"].ToString() : string.Empty,
                            IdAmministrazione = !string.IsNullOrEmpty(row["ID_AMMINISTRAZIONE"].ToString()) ? row["ID_AMMINISTRAZIONE"].ToString() : string.Empty,
                            Oggetto = !string.IsNullOrEmpty(row["VAR_OGGETTO"].ToString()) ? row["VAR_OGGETTO"].ToString() : string.Empty,
                            IdOggetto = !string.IsNullOrEmpty(row["ID_OGGETTO"].ToString()) ? row["ID_OGGETTO"].ToString() : string.Empty,
                            DescrizioneOggetto = !string.IsNullOrEmpty(row["VAR_DESC_OGGETTO"].ToString()) ? row["VAR_DESC_OGGETTO"].ToString() : string.Empty,
                            CodiceAzione = !string.IsNullOrEmpty(row["VAR_COD_AZIONE"].ToString()) ? row["VAR_COD_AZIONE"].ToString() : string.Empty,
                            DescrizioneAzione = !string.IsNullOrEmpty(row["VAR_DESC_AZIONE"].ToString()) ? row["VAR_DESC_AZIONE"].ToString() : string.Empty,
                            IdTrasmissione = !string.IsNullOrEmpty(row["ID_TRASM"].ToString()) ? row["ID_TRASM"].ToString() : string.Empty,
                            DataAzione = !string.IsNullOrEmpty(row["DTA_AZIONE"].ToString()) ? row["DTA_AZIONE"].ToString() : string.Empty,
                            IdPeopleDelegante = !string.IsNullOrEmpty(row["ID_PEOPLE_DELEGANTE"].ToString()) ? row["ID_PEOPLE_DELEGANTE"].ToString() : string.Empty
                        };

                        eventDocuments.Add(eventDocument);
                    }
                }
            }
        }
        catch (Exception exc)
        {
            logger.Error("Errore in DocsPaDb.Query_DocsPAWS.DocumentMetadata - Metodo GetEventDocument", exc);
            return null;
        }

        logger.Information("FINR Metodo GetEventDocument in DocsPaDb.Query_DocsPAWS.DocumentMetadata");

        return eventDocuments;
    }

    public DocsPaVO.DocumentMetadata.MetadatiDocumento GetMetadatiDocumento(string idProfile)
    {
        logger.Debug("INIZIO Metodo GetMetadatiDocumento in DocsPaDb.Query_DocsPAWS.DocumentMetadata");
        DocsPaVO.DocumentMetadata.MetadatiDocumento metadati = null;
        try
        {
            DataSet ds = new DataSet();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_METADATI_DOCUMENTO");
            q.setParam("idProfile", idProfile);
            string query = q.getSQL();
            logger.Debug("GetMetadatiDocumento: " + query);
            bool result = true;
            using (DBProvider dbProvider = new DBProvider())
            {
                result = dbProvider.ExecuteQuery(out ds, "metadati", query);
                if (result)
                {
                    if (ds.Tables["metadati"] != null && ds.Tables["metadati"].Rows.Count > 0)
                    {
                        metadati = new DocsPaVO.DocumentMetadata.MetadatiDocumento()
                        {
                            SystemId = ds.Tables["metadati"].Rows[0]["SYSTEM_ID"].ToString(),
                            IdProfile = ds.Tables["metadati"].Rows[0]["ID_PROFILE"].ToString(),
                            IdVersion = ds.Tables["metadati"].Rows[0]["ID_VERSION"].ToString(),
                            MetadatiXML = dbProvider.GetLargeText("DPA_METADATI_DOCUMENTO", ds.Tables["metadati"].Rows[0]["SYSTEM_ID"].ToString(), "METADATI_XML")
                        };
                    }
                }
            }
        }
        catch(Exception e)
        {
            logger.Error("Errore in DocsPaDb.Query_DocsPAWS.DocumentMetadata - Metodo GetMetadatiDocumento", e);
            metadati = null;
        }

        logger.Debug("FINE Metodo GetMetadatiDocumento in DocsPaDb.Query_DocsPAWS.DocumentMetadata");
        return metadati;
    }

    public bool DeleteMetadatiDocumento(string systemId)
    {
        logger.Debug("INIZIO Metodo DeleteMetadatiDocumento in DocsPaDb.Query_DocsPAWS.DocumentMetadata");
        bool result = true;
        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_METADATI_DOCUMENTO");
            q.setParam("SystemId", systemId);

            string query = q.getSQL();
            logger.Debug("DeleteMetadatiDocumento: " + query);
            using (DBProvider dbProvider = new DBProvider())
                result = dbProvider.ExecuteNonQuery(query);
        }
        catch (Exception exc)
        {
            logger.Error("Errore in DocsPaDb.Query_DocsPAWS.DocumentMetadata - Metodo DeleteMetadatiDocumento", exc);
            return false;
        }

        logger.Information("FINE Metodo DeleteMetadatiDocumento in DocsPaDb.Query_DocsPAWS.DocumentMetadata");

        return result;
    }

    public bool InsertMetadatiDocumento(DocsPaVO.DocumentMetadata.MetadatiDocumento metadati)
    {
        logger.Debug("INIZIO Metodo InsertMetadatiDocumento in DocsPaDb.Query_DocsPAWS.DocumentMetadata");
        bool result = true;
        string id = string.Empty;
        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_METADATI_DOCUMENTO");
            ArrayList parameters = new ArrayList();
            if (DBType.ToUpper().Equals("ORACLE"))
                id = DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_METADATI_DOCUMENTO");
            q.setParam("SystemId", id);
            q.setParam("IdProfile", metadati.IdProfile);
            q.setParam("IdVersion", metadati.IdVersion);
            q.setParam("DataAzione", DocsPaDbManagement.Functions.Functions.ToDate(metadati.DataAzione));
            q.setParam("CodiceAzione", metadati.CodiceAzione);
            q.setParam("DescrizioneAzione", metadati.DescrizioneAzione.Replace("'", "''"));
            q.setParam("DescrizioneOggetto", metadati.DescrizioneOggetto.Replace("'", "''"));

            string query = q.getSQL();
            logger.Debug("InsertMetadatiDocumento: " + query);
            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.BeginTransaction();
                result = dbProvider.ExecuteNonQuery(query);
                if (result)
                {
                    string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_METADATI_DOCUMENTO");
                    logger.Debug(sql);

                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(sql))
                    {
                        while (reader.Read())
                        {
                            id = reader.GetValue(0).ToString();
                        }
                    }
                    if(!string.IsNullOrEmpty(id))
                        result = dbProvider.SetLargeText("DPA_METADATI_DOCUMENTO", id, "METADATI_XML", metadati.MetadatiXML);
                    else
                        result = false;
                }

                if (result)
                    dbProvider.CommitTransaction();
                else
                    dbProvider.RollbackTransaction();
            }  
            
        }
        catch (Exception exc)
        {
            logger.Error("Errore in DocsPaDb.Query_DocsPAWS.DocumentMetadata - Metodo InsertMetadatiDocumento", exc);
            return false;
        }

        logger.Information("FINE Metodo InsertMetadatiDocumento in DocsPaDb.Query_DocsPAWS.DocumentMetadata");

        return result;
    }

    public bool DeleteEventDocument(string systemId)
    {
        logger.Debug("INIZIO Metodo DeleteMetadatiDocumento in DocsPaDb.Query_DocsPAWS.DocumentMetadata");
        bool result = true;
        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_EVENT_DOCUMENT");
            q.setParam("SystemId", systemId);

            string query = q.getSQL();
            logger.Debug("DeleteMetadatiDocumento: " + query);
            using (DBProvider dbProvider = new DBProvider())
                result = dbProvider.ExecuteNonQuery(query);
        }
        catch (Exception exc)
        {
            logger.Error("Errore in DocsPaDb.Query_DocsPAWS.DocumentMetadata - Metodo DeleteMetadatiDocumento", exc);
            return false;
        }

        logger.Information("FINE Metodo DeleteMetadatiDocumento in DocsPaDb.Query_DocsPAWS.DocumentMetadata");

        return result;
    }

    public bool UpdateEventDocument(string systemId)
    {
        logger.Debug("INIZIO Metodo UpdateEventDocument in DocsPaDb.Query_DocsPAWS.DocumentMetadata");
        bool result = true;
        int rowsAffected = 0;
        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_EVENT_DOCUMENT");
            q.setParam("SystemId", systemId);

            string query = q.getSQL();
            logger.Debug("UpdateEventDocument: " + query);
            using (DBProvider dbProvider = new DBProvider())
                result = dbProvider.ExecuteNonQuery(query, out rowsAffected);
        }
        catch (Exception exc)
        {
            logger.Error("Errore in DocsPaDb.Query_DocsPAWS.DocumentMetadata - Metodo UpdateEventDocument", exc);
            return false;
        }

        logger.Information("FINE Metodo UpdateEventDocument in DocsPaDb.Query_DocsPAWS.DocumentMetadata");

        return result;
    }

    public List<DocsPaVO.documento.InfoDocumento> GetInfoDocInFascicolo(string idFascicolo)
    {
        logger.Debug("INIZIO Metodo GetInfoDocInFascicolo in DocsPaDb.Query_DocsPAWS.DocumentMetadata");
        List<DocsPaVO.documento.InfoDocumento> infoDocs = new List<DocsPaVO.documento.InfoDocumento>();
        DocsPaVO.documento.InfoDocumento infoDoc = null;
        DataSet ds = new DataSet();
        try
        {
            string queryFolderString = "";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project10");

            q.setParam("param1", idFascicolo);

            queryFolderString = q.getSQL();
            using (DBProvider dbProvider = new DBProvider())
                dbProvider.ExecuteQuery(ds, "FOLDER", queryFolderString);

            string queryDocString = "";
            DocsPaUtils.Query qSelect = DocsPaUtils.InitQuery.getInstance().getQuery("S_ProjectComponents4");

            for (int j = 0; j < ds.Tables["FOLDER"].Rows.Count; j++)
            {
                queryDocString = queryDocString + ds.Tables["FOLDER"].Rows[j]["SYSTEM_ID"].ToString();
                if (j < ds.Tables["FOLDER"].Rows.Count - 1)
                {
                    queryDocString = queryDocString + ",";
                }
            }
            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DOC_IN_FASCICOLO");
            q.setParam("idFascicolo", queryDocString);

            string query = q.getSQL();
            logger.Debug("GetInfoDocInFascicolo: " + query);

            bool result = true;
            using (DBProvider dbProvider = new DBProvider())
                result = dbProvider.ExecuteQuery(out ds, "infoDoc", query);

            if (result)
            {
                if (ds.Tables["infoDoc"] != null && ds.Tables["infoDoc"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["infoDoc"].Rows)
                    {
                        infoDoc = new DocsPaVO.documento.InfoDocumento()
                        {
                            docNumber = !string.IsNullOrEmpty(row["SYSTEM_ID"].ToString()) ? row["SYSTEM_ID"].ToString() : string.Empty,
                            acquisitaImmagine = !string.IsNullOrEmpty(row["VAR_IMPRONTA"].ToString()) ? row["VAR_IMPRONTA"].ToString() : string.Empty,
                            segnatura = !string.IsNullOrEmpty(row["SEGNATURA"].ToString()) ? row["SEGNATURA"].ToString() : string.Empty
                        };

                        infoDocs.Add(infoDoc);
                    }

                    ds.Dispose();
                }
            }
        }
        catch (Exception e)
        {
            logger.Error("Errore in DocsPaDb.Query_DocsPAWS.DocumentMetadata - Metodo GetInfoDocInFascicolo", e);
            return null;
        }
        logger.Debug("FINE Metodo GetInfoDocInFascicolo in DocsPaDb.Query_DocsPAWS.DocumentMetadata");
        return infoDocs;
    }

    public DocsPaVO.documento.InfoDocumento GetInfoDocInFascicoloByDocnumber(string idProfile)
    {
        logger.Debug("INIZIO Metodo GetInfoDocInFascicoloByDocnumber in DocsPaDb.Query_DocsPAWS.DocumentMetadata");
        DocsPaVO.documento.InfoDocumento infoDoc = null;
        DataSet ds = new DataSet();
        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_INFO_DOC_BY_ID");
            q.setParam("idProfile", idProfile);

            string query = q.getSQL();
            logger.Debug("GetInfoDocInFascicoloByDocnumber: " + query);

            bool result = true;
            using (DBProvider dbProvider = new DBProvider())
                result = dbProvider.ExecuteQuery(out ds, "infoDoc", query);

            if (result)
            {
                if (ds.Tables["infoDoc"] != null && ds.Tables["infoDoc"].Rows.Count > 0)
                {
                    DataRow row = ds.Tables["infoDoc"].Rows[0];
                    infoDoc = new DocsPaVO.documento.InfoDocumento()
                    {
                        docNumber = !string.IsNullOrEmpty(row["SYSTEM_ID"].ToString()) ? row["SYSTEM_ID"].ToString() : string.Empty,
                        acquisitaImmagine = !string.IsNullOrEmpty(row["VAR_IMPRONTA"].ToString()) ? row["VAR_IMPRONTA"].ToString() : string.Empty,
                        segnatura = !string.IsNullOrEmpty(row["SEGNATURA"].ToString()) ? row["SEGNATURA"].ToString() : string.Empty
                    };

                    ds.Dispose();
                }
            }
        }
        catch (Exception e)
        {
            logger.Error("Errore in DocsPaDb.Query_DocsPAWS.DocumentMetadata - Metodo GetInfoDocInFascicoloByDocnumber", e);
            return null;
        }
        logger.Debug("FINE Metodo GetInfoDocInFascicoloByDocnumber in DocsPaDb.Query_DocsPAWS.DocumentMetadata");
        return infoDoc;
    }

    public DocsPaVO.DocumentMetadata.MetadatiFascicolo GetMetadatiFascicolo(string idProject)
    {
        logger.Debug("INIZIO Metodo GetMetadatiFascicolo in DocsPaDb.Query_DocsPAWS.DocumentMetadata");
        DocsPaVO.DocumentMetadata.MetadatiFascicolo metadati = null;
        try
        {
            DataSet ds = new DataSet();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_METADATI_FASCICOLO");
            q.setParam("IdProject", idProject);
            string query = q.getSQL();
            logger.Debug("GetMetadatiFascicolo: " + query);
            bool result = true;
            using (DBProvider dbProvider = new DBProvider())
            {
                result = dbProvider.ExecuteQuery(out ds, "metadati", query);
                if (result)
                {
                    if (ds.Tables["metadati"] != null && ds.Tables["metadati"].Rows.Count > 0)
                    {
                        metadati = new DocsPaVO.DocumentMetadata.MetadatiFascicolo()
                        {
                            SystemId = ds.Tables["metadati"].Rows[0]["SYSTEM_ID"].ToString(),
                            IdProject = ds.Tables["metadati"].Rows[0]["ID_PROJECT"].ToString(),
                            MetadatiXML = dbProvider.GetLargeText("DPA_METADATI_FASCICOLO", ds.Tables["metadati"].Rows[0]["SYSTEM_ID"].ToString(), "METADATI_XML")
                        };
                    }
                }
            }
        }
        catch (Exception e)
        {
            logger.Error("Errore in DocsPaDb.Query_DocsPAWS.GetMetadatiFascicolo - Metodo GetMetadatiDocumento", e);
            metadati = null;
        }

        logger.Debug("FINE Metodo GetMetadatiFascicolo in DocsPaDb.Query_DocsPAWS.DocumentMetadata");
        return metadati;
    }

    public bool DeleteMetadatiFascicolo(string systemId)
    {
        logger.Debug("INIZIO Metodo DeleteMetadatiFascicolo in DocsPaDb.Query_DocsPAWS.DocumentMetadata");
        bool result = true;
        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_METADATI_FASCICOLO");
            q.setParam("SystemId", systemId);

            string query = q.getSQL();
            logger.Debug("DeleteMetadatiFascicolo: " + query);
            using (DBProvider dbProvider = new DBProvider())
                result = dbProvider.ExecuteNonQuery(query);
        }
        catch (Exception exc)
        {
            logger.Error("Errore in DocsPaDb.Query_DocsPAWS.DocumentMetadata - Metodo DeleteMetadatiFascicolo", exc);
            return false;
        }

        logger.Information("FINE Metodo DeleteMetadatiFascicolo in DocsPaDb.Query_DocsPAWS.DocumentMetadata");

        return result;
    }

    public bool InsertMetadatiFascicolo(DocsPaVO.DocumentMetadata.MetadatiFascicolo metadati)
    {
        logger.Debug("INIZIO Metodo InsertMetadatiFascicolo in DocsPaDb.Query_DocsPAWS.DocumentMetadata");
        bool result = true;
        string id = string.Empty;
        try
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_METADATI_FASCICOLO");
            ArrayList parameters = new ArrayList();
            if (DBType.ToUpper().Equals("ORACLE"))
                id = DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_METADATI_FASCICOLO");
            q.setParam("SystemId", id);
            q.setParam("IdProject", metadati.IdProject);
            q.setParam("DataAzione", DocsPaDbManagement.Functions.Functions.ToDate(metadati.DataAzione));
            q.setParam("CodiceAzione", metadati.CodiceAzione);
            q.setParam("DescrizioneAzione", metadati.DescrizioneAzione.Replace("'", "''"));
            q.setParam("DescrizioneOggetto", metadati.DescrizioneOggetto.Replace("'", "''"));

            string query = q.getSQL();
            logger.Debug("InsertMetadatiDocumento: " + query);
            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.BeginTransaction();
                result = dbProvider.ExecuteNonQuery(query);
                if (result)
                {
                    string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_METADATI_FASCICOLO");
                    logger.Debug(sql);

                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(sql))
                    {
                        while (reader.Read())
                        {
                            id = reader.GetValue(0).ToString();
                        }
                    }
                    if (!string.IsNullOrEmpty(id))
                        result = dbProvider.SetLargeText("DPA_METADATI_FASCICOLO", id, "METADATI_XML", metadati.MetadatiXML);
                    else
                        result = false;
                }

                if (result)
                    dbProvider.CommitTransaction();
                else
                    dbProvider.RollbackTransaction();
            }

        }
        catch (Exception exc)
        {
            logger.Error("Errore in DocsPaDb.Query_DocsPAWS.DocumentMetadata - Metodo InsertMetadatiFascicolo", exc);
            return false;
        }

        logger.Information("FINE Metodo InsertMetadatiFascicolo in DocsPaDb.Query_DocsPAWS.DocumentMetadata");

        return result;
    }
}
