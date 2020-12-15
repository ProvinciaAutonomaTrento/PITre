using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using DocsPaVO.UploadFiles;
using System.Data;
using log4net;
using System.IO;

namespace DocsPaDB.Query_DocsPAWS
{
    public class UploadFiles : DBProvider
    {
        #region Const

        private ILog logger = LogManager.GetLogger(typeof(LibroFirma));

        #endregion

        #region select

        public FileInUpload GetFileInUpload(string strIdentity, DocsPaVO.utente.InfoUtente infoUtente)
        {
            //hashFile non viene più considerato
            logger.Debug("INIZIO Metodo GetFileInUpload in DocsPaDb.Query_DocsPAWS.UploadFiles");
            FileInUpload retVal = null;

            try
            {
                string query;
                DataSet ds = new DataSet();

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_UPLOAD_FILE");
                //q.setParam("fileHash", hashFile);
                q.setParam("identity", strIdentity);
                q.setParam("IdUtente", infoUtente.idPeople);
                //q.setParam("fileName", fileName);
                //q.setParam("fileDescription", fileDescription);

                query = q.getSQL();
                logger.Debug("GetFileInUpload: " + query);

                if (this.ExecuteQuery(out ds, "UPLOAD_FILE", query))
                {
                    if (ds.Tables["UPLOAD_FILE"] != null && ds.Tables["UPLOAD_FILE"].Rows.Count > 0)
                    {
                        retVal = new FileInUpload();

                        foreach (DataRow row in ds.Tables["UPLOAD_FILE"].Rows)
                        {
                            retVal.IdUtente = row["Id_People"].ToString();
                            retVal.IdRuolo = row["Id_Ruolo"].ToString();
                            retVal.ChunkNumber = Convert.ToInt32(row["File_Part"].ToString());
                            //retVal.FileHash = hashFile;
                            retVal.FileName = row["File_Name"].ToString();
                            retVal.FileDescription = row["File_Description"].ToString();
                            retVal.FileSenderPath = row["Sender_Path"].ToString();
                            retVal.FileSize = Convert.ToInt64(row["File_Size"].ToString());
                            retVal.MachineName = row["Sender_Name"].ToString();
                            retVal.TotalChunkNumber = Convert.ToInt32(row["Total_Part"].ToString());
                            retVal.RepositoryPath = row["File_Path"].ToString();
                            retVal.Order = Convert.ToInt32(row["Num_Order"].ToString());
                            retVal.StrIdentity = row["UnicStr"].ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo GetFileInUpload in DocsPaDb.Query_DocsPAWS.UploadFiles " + e.Message);
                return null;
            }

            logger.Debug("FINE Metodo GetFileInUpload in DocsPaDb.Query_DocsPAWS.UploadFiles");
            return retVal;
        }

        public List<FileInUpload> GetListFilesInUpload(string machineName, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<FileInUpload> listOnServer = new List<FileInUpload>();

            try
            {
                string query;
                DataSet ds = new DataSet();

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_UPLOAD_FILE_LIST");
                q.setParam("IdUtente", infoUtente.idPeople);
                q.setParam("MachineName", machineName);

                query = q.getSQL();
                logger.Debug("GetListFilesInUpload: " + query);

                if (this.ExecuteQuery(out ds, "UPLOAD_FILE_LIST", query))
                {
                    if (ds.Tables["UPLOAD_FILE_LIST"] != null && ds.Tables["UPLOAD_FILE_LIST"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["UPLOAD_FILE_LIST"].Rows)
                        {
                            FileInUpload retVal = new FileInUpload();

                            retVal.IdUtente = row["Id_People"].ToString();
                            retVal.IdRuolo = row["Id_Ruolo"].ToString();
                            retVal.ChunkNumber = Convert.ToInt32(row["File_Part"].ToString());
                            retVal.FileHash = row["File_Hash"].ToString();
                            retVal.FileName = row["File_Name"].ToString();
                            retVal.FileDescription = row["File_Description"].ToString();
                            retVal.FileSenderPath = row["Sender_Path"].ToString();
                            retVal.FileSize = Convert.ToInt64(row["File_Size"].ToString());
                            retVal.MachineName = row["Sender_Name"].ToString();
                            retVal.TotalChunkNumber = Convert.ToInt32(row["Total_Part"].ToString());
                            retVal.RepositoryPath = row["File_Path"].ToString();
                            retVal.Order = Convert.ToInt32(row["Num_Order"].ToString());
                            retVal.StrIdentity = row["UnicStr"].ToString();

                            listOnServer.Add(retVal);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo GetListFilesInUpload in DocsPaDb.Query_DocsPAWS.UploadFiles " + e.Message);
            }

            return listOnServer;
        }

        public List<FileInUpload> GetListFilesUploaded(DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<FileInUpload> listFilesOnServer = new List<FileInUpload>();

            try
            {
                string query;
                DataSet ds = new DataSet();

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_BIGFILE_LIST");
                q.setParam("IdUtente", infoUtente.idPeople);
                q.setParam("IdRuolo", infoUtente.idGruppo);
                //q.setParam("not", (incomplete?"":"not"));

                query = q.getSQL();
                logger.Debug("GetListFilesCompletati: " + query);

                if (this.ExecuteQuery(out ds, "UPLOADED_FILE_LIST", query))
                {
                    if (ds.Tables["UPLOADED_FILE_LIST"] != null && ds.Tables["UPLOADED_FILE_LIST"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["UPLOADED_FILE_LIST"].Rows)
                        {
                            FileInUpload retVal = new FileInUpload();

                            retVal.IdUtente = row["Id_People"].ToString();
                            retVal.IdRuolo = row["Id_Ruolo"].ToString();
                            retVal.ChunkNumber = Convert.ToInt32(row["File_Part"].ToString());
                            retVal.FileHash = row["File_Hash"].ToString();
                            retVal.FileName = row["File_Name"].ToString();
                            retVal.FileDescription = row["File_Description"].ToString();
                            retVal.FileSenderPath = row["Sender_Path"].ToString();
                            retVal.FileSize = Convert.ToInt64(row["File_Size"].ToString());
                            retVal.MachineName = row["Sender_Name"].ToString();
                            retVal.TotalChunkNumber = Convert.ToInt32(row["Total_Part"].ToString());
                            retVal.RepositoryPath = row["File_Path"].ToString();
                            retVal.Order = Convert.ToInt32(row["Num_Order"].ToString());
                            retVal.StrIdentity = row["UnicStr"].ToString();

                            listFilesOnServer.Add(retVal);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo GetListFilesCompletati in DocsPaDb.Query_DocsPAWS.UploadFiles " + e.Message);
            }

            return listFilesOnServer;
        }

        #endregion

        #region insert-update

        /// <summary>
        /// Creazione di un file in upload per l'utente loggato
        /// </summary>
        /// <param name="fileInUpload"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public bool InsertFileInUpload(FileInUpload fileInUpload, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retVal = false;
            logger.Debug("Inizio Metodo InsertFileInUpload in DocsPaDb.Query_DocsPAWS.UploadFiles");

            if (fileInUpload != null)
            {
                try
                {
                    BeginTransaction();

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_UPLOAD_FILE");
                    string idProcesso = string.Empty;

                    q.setParam("Id_People", infoUtente.idPeople);
                    q.setParam("Id_Ruolo", fileInUpload.IdRuolo);
                    q.setParam("File_Hash", fileInUpload.FileHash);
                    q.setParam("Sender_Name", fileInUpload.MachineName);
                    q.setParam("Sender_Path", fileInUpload.FileSenderPath); 
                    q.setParam("File_Name", fileInUpload.FileName);
                    q.setParam("File_Description", fileInUpload.FileDescription);
                    q.setParam("File_Size", fileInUpload.FileSize.ToString());
                    q.setParam("File_Part", fileInUpload.ChunkNumber.ToString());
                    q.setParam("Total_Part", fileInUpload.TotalChunkNumber.ToString());
                    q.setParam("File_Path", fileInUpload.RepositoryPath);
                    q.setParam("Ordine", fileInUpload.Order.ToString());
                    q.setParam("identity", fileInUpload.StrIdentity);

                    string query = q.getSQL();
                    logger.Debug("InsertFileInUpload: " + query);
                    if (ExecuteNonQuery(query))
                    {
                        CommitTransaction();
                        retVal = true;
                    }
                    else
                    {
                        throw new Exception("Errore durante la creazione del record nella tabella DPA_UPLOAD_FILE: " + query);
                    }
                }
                catch (Exception e)
                {
                    RollbackTransaction();
                    logger.Error("Errore in DocsPaDb.Query_DocsPAWS.UploadFiles - Metodo InsertFileInUpload", e);
                    retVal = false;
                }

            }
            
            logger.Debug("Fine Metodo InsertFileInUpload in DocsPaDb.Query_DocsPAWS.UploadFiles");
            return retVal;
        }

        /// <summary>
        /// Aggiornamento del record relativo al file in upload
        /// </summary>
        /// <param name="fileInUpload"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public bool UpdateFileInUpload(FileInUpload fileInUpload, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retVal = false;
            logger.Debug("Inizio Metodo UploadFileInUpload in DocsPaDb.Query_DocsPAWS.UploadFiles");

            if (fileInUpload != null)
            {
                try
                {
                    BeginTransaction();

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_UPLOAD_FILE");
                    string idProcesso = string.Empty;

                    q.setParam("identity", fileInUpload.StrIdentity);
                    //q.setParam("fileName", fileInUpload.FileName);
                    //q.setParam("fileDescription", fileInUpload.FileDescription);

                    q.setParam("File_Name", fileInUpload.FileName);
                    q.setParam("File_Description", fileInUpload.FileDescription);
                    q.setParam("File_Part", fileInUpload.ChunkNumber.ToString());

                    q.setParam("IdRuolo", fileInUpload.IdRuolo);
                    q.setParam("IdUtente", infoUtente.idPeople);
                    //q.setParam("fileHash", fileInUpload.FileHash);
                    q.setParam("TotalChunk", fileInUpload.TotalChunkNumber.ToString());
                    q.setParam("File_Path", fileInUpload.RepositoryPath);
                    q.setParam("Ordine", fileInUpload.Order.ToString());

                    if (fileInUpload.ChunkNumber == fileInUpload.TotalChunkNumber)
                    {
                        if (dbType == "SQL")
                            q.setParam("End_Date", "GETDATE()");
                        else
                            q.setParam("End_Date", "sysDate");
                    }
                    else
                    {
                        q.setParam("End_Date", "null");
                    }

                    string query = q.getSQL();
                    logger.Debug("UploadFileInUpload: " + query);
                    if (ExecuteNonQuery(query))
                    {
                        CommitTransaction();
                        retVal = true;
                    }
                    else
                    {
                        throw new Exception("Errore durante l'aggiornamento del record nella tabella DPA_UPLOAD_FILE: " + query);
                    }
                }
                catch (Exception e)
                {
                    RollbackTransaction();
                    logger.Error("Errore in DocsPaDb.Query_DocsPAWS.UploadFiles - Metodo UploadFileInUpload", e);
                    retVal = false;
                }

            }

            logger.Debug("Fine Metodo UploadFileInUpload in DocsPaDb.Query_DocsPAWS.UploadFiles");
            return retVal;
        }
        #endregion

        public bool DeleteFileInUpload(string fileName, string fileDescription, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("INIZIO Metodo GetFileInUpload in DocsPaDb.Query_DocsPAWS.UploadFiles");

            bool retValue = true;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_UPLOAD_FILE");
                //q.setParam("fileHash", hashFile);
                q.setParam("IdUtente", infoUtente.idPeople);
                q.setParam("fileName", fileName);
                q.setParam("fileDescription", fileDescription);

                string query = q.getSQL();
                logger.Debug("DeleteFileInUpload: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante la rimozione del file in upload: " + query);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo DeleteFileInUpload in DocsPaDb.Query_DocsPAWS.UploadFiles: " + e.Message);
                return false;
            }

            logger.Debug("Fine Metodo DeleteFileInUpload in DocsPaDb.Query_DocsPAWS.UploadFiles");
            return retValue;
        }
    }
}
