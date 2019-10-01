using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Collections;
using System.Data;

namespace DocsPaDB.Query_DocsPAWS
{
    public class Integrazioni : DBProvider
    {
        private static ILog logger = LogManager.GetLogger(typeof(Integrazioni));
        #region Mibact Bacheca
        public ArrayList MIBACT_BACHECA_getDocsDaNotificare(string statoInvia, string statoAggiorna, string campoNCirc)
        {
            ArrayList retVal = new ArrayList();
            try{
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("MIBACT_GET_C_NOTIFY");
            q.setParam("statinotifica", string.Format("'{0}','{1}'",statoInvia.ToUpper(), statoAggiorna.ToUpper()));
            q.setParam("statoinvia", statoInvia.ToUpper());
            q.setParam("descncircolare", campoNCirc.ToUpper());
            string queryString = q.getSQL();
            logger.Debug(queryString);
            DocsPaVO.ExternalServices.MIBACT_Bacheca_info infoBacheca;
            DataSet dataset = new DataSet();
            this.ExecuteQuery(out dataset, "BACHECADOCS", queryString);
            if (dataset.Tables["BACHECADOCS"] != null && dataset.Tables["BACHECADOCS"].Rows.Count > 0)
            {
                logger.Debug("Righe: " + dataset.Tables["BACHECADOCS"].Rows.Count);
                foreach (DataRow r in dataset.Tables["BACHECADOCS"].Rows)
                {
                    infoBacheca = new DocsPaVO.ExternalServices.MIBACT_Bacheca_info();
                    infoBacheca.doc_titolo = r["DOC_TITOLO"].ToString();
                    infoBacheca.doc_data_creazione = r["DOC_DATA_CREAZIONE"].ToString();
                    infoBacheca.doc_protocollo = r["DOC_PROTOCOLLO"].ToString();
                    infoBacheca.doc_n_circolare = r["DOC_N_CIRCOLARE"].ToString();
                    infoBacheca.doc_autore = r["DOC_AUTORE"].ToString();
                    infoBacheca.servizio_codice = r["SERVIZIO_CODICE"].ToString();
                    infoBacheca.servizio_descrizione = r["SERVIZIO_DESCRIZIONE"].ToString();
                    infoBacheca.ufficio_emittente_codice = r["UFFECOD_ESPIIDAOO"].ToString();
                    infoBacheca.espi_codice = r["ESPI_CODICE"].ToString();
                    infoBacheca.espi_id_aoo = r["UFFECOD_ESPIIDAOO"].ToString();
                    //infoBacheca.circolare_annullata = r["CODE_AMM"].ToString();
                    infoBacheca.statoCircolare = r["STATO"].ToString();
                    infoBacheca.annullamento = r["DOC_DATA_ANNULLA"].ToString();
                    //infoBacheca.nuova_circolare = r["CODE_AMM"].ToString();
                    infoBacheca.id_amministrazione = r["ID_AMMINISTRAZIONE"].ToString();
                    infoBacheca.id_utente = r["ID_UTENTE"].ToString();
                    infoBacheca.id_gruppo = r["ID_GRUPPO"].ToString();
                    retVal.Add(infoBacheca);                    
                }
            }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = null;
            }
            return retVal;
        }

        public ArrayList MIBACT_BACHECA_GetFileInfoDoc(string idDoc)
        {
            ArrayList retVal = new ArrayList();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("MIBACT_GET_INFOFILE_DOC");
                q.setParam("idDoc", idDoc);
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DocsPaVO.ExternalServices.MIGR_File_Info file_info;
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "MGR_FS_FI", queryString);
                if (dataset.Tables["MGR_FS_FI"] != null && dataset.Tables["MGR_FS_FI"].Rows.Count > 0)
                {
                    logger.Debug("Righe: " + dataset.Tables["MGR_FS_FI"].Rows.Count);
                    foreach (DataRow r in dataset.Tables["MGR_FS_FI"].Rows)
                    {
                        file_info = new DocsPaVO.ExternalServices.MIGR_File_Info();
                        file_info.PathOld = r["PATH"].ToString();
                        file_info.VersionId = r["VERSION_ID"].ToString();
                        file_info.Docnumber = r["DOCNUMBER"].ToString();
                        file_info.Filesize = r["FILE_SIZE"].ToString();
                        file_info.ImprontaComp = r["VAR_IMPRONTA"].ToString();
                        file_info.Ext = r["EXT"].ToString();
                        file_info.NomeOriginale = r["VAR_NOMEORIGINALE"].ToString();
                        file_info.DataFileAcq = r["DATA_ACQ_FILE"].ToString();
                        file_info.DataVersCreazione = r["DATA_CREA_VERS"].ToString();
                        file_info.Version = r["VERSION"].ToString();
                        file_info.VersionLabel = r["VERSION_LABEL"].ToString();
                        file_info.IdPeopleAutore = r["ID_PEOPLE"].ToString();
                        file_info.IdCorrRuoloAutore = r["ID_CORR_RUOLO"].ToString();
                        file_info.MessaggioLog = r["DESCRIZIONE"].ToString();
                        
                        retVal.Add(file_info);
                    }
                }
                else
                {
                    retVal = null;
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = null;
            }
            return retVal;
        }
        #endregion

        #region BigFiles FTP

        public bool BigFilesFTP_InsertIntoTable(DocsPaVO.ExternalServices.FileFtpUpInfo infoFile)
        {
            bool retval = true;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPABIGFILE");
                    string values = "";

                    values += infoFile.IdDocument;
                    values += "," + infoFile.IdAmm;
                    values += "," +infoFile.UploaderRoleId;
                    values += "," +infoFile.UploaderId;
                    values += ",'" +infoFile.FileName;
                    values += "','" +infoFile.FileSize;
                    values += "','" + infoFile.HashFile;
                    values += "','" + infoFile.FTPPath+"',";

                    q.setParam("values", values);
                    q.setParam("iddoc", infoFile.IdDocument);
                    string queryString = q.getSQL();
                    logger.Debug(queryString);

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

        public ArrayList BigFilesFTP_GetFilesToTransfer()
        {
            ArrayList retval = new ArrayList();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPABIGFILE");
            q.setParam("condizione", " UPPER(A.FILEUPSTATUS) = 'PENDING' ");
            string queryString = q.getSQL();
            logger.Debug(queryString);
            // Inserire oggetto Evento CDS
            DocsPaVO.ExternalServices.FileFtpUpInfo infoFile;
            DataSet dataset = new DataSet();
            this.ExecuteQuery(out dataset, "BFFTPINFO", queryString);
            if (dataset.Tables["BFFTPINFO"] != null && dataset.Tables["BFFTPINFO"].Rows.Count > 0)
            {
                foreach (DataRow r in dataset.Tables["BFFTPINFO"].Rows)
                {
                    //Inserire associazione oggetto CDS
                    infoFile = new DocsPaVO.ExternalServices.FileFtpUpInfo();
                    infoFile.IdQueue = r["IDQUEUE"].ToString();
                    infoFile.IdDocument = r["IDDOCUMENT"].ToString();
                    infoFile.Description = r["DESCRIPTION"].ToString();
                    infoFile.IdAmm = r["ID_AMM"].ToString();
                    infoFile.CodeAdm = r["CODEADM"].ToString();
                    infoFile.UploaderId = r["UPLOADERID"].ToString();
                    infoFile.Uploader = r["UPLOADER"].ToString();
                    infoFile.UploaderRoleId = r["UPLOADERROLEID"].ToString();
                    infoFile.UploaderRole = r["UPLOADERROLE"].ToString();
                    infoFile.FileName = r["FILENAME"].ToString();
                    infoFile.FileSize = r["FILESIZE"].ToString();
                    infoFile.HashFile = r["HASHFILE"].ToString();
                    infoFile.FTPPath = r["FTPPATH"].ToString();
                    infoFile.Status = r["STATUS"].ToString();
                    infoFile.ErrorMessage = r["ERRORMESSAGE"].ToString();
                    infoFile.VersionId = r["VERSIONID"].ToString();
                    infoFile.Version = r["VERSION"].ToString();
                    infoFile.FSPath = r["FSPATH"].ToString();
                    // comunico il doc_Root per far copiare i file
                    if (string.IsNullOrEmpty(infoFile.FSPath))
                        infoFile.FSPath = System.Configuration.ConfigurationManager.AppSettings["UPLOAD_BIGFILE_REPOSITORY"];

                    
                    if(string.IsNullOrEmpty(infoFile.FSPath))
                        infoFile.FSPath = System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"];

                    retval.Add(infoFile);
                }
            }
            else retval = null;
            return retval;
        }

        public bool BigFilesFTP_updateTable(DocsPaVO.ExternalServices.FileFtpUpInfo infoFile)
        {
            bool retval = true;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPABIGFILE");

                    q.setParam("stato", infoFile.Status);
                    q.setParam("messErrore", infoFile.ErrorMessage);
                    q.setParam("pathFS", infoFile.FSPath);
                    q.setParam("idQueue", infoFile.IdQueue);
                    string queryString = q.getSQL();
                    logger.Debug(queryString);

                    retval = this.ExecuteNonQuery(queryString);
                    if (infoFile.Status == "OK") 
                    {
                        DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("U_Versions");
                        q2.setParam("param1", "SUBVERSION = 'A'");
                        q2.setParam("param2", "VERSION_ID = " + infoFile.VersionId);
                        queryString = q2.getSQL();
                        logger.Debug(queryString);
                        retval = this.ExecuteNonQuery(queryString);

                        DocsPaUtils.Query q3 = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROFILE_EXT");
                        // controllare le operazioni sul nome del file.
                        string filenameX = infoFile.FileName.ToLower();
                        string cha_firmato = "0", cha_tipo_firma = "N";
                        if (filenameX.ToUpper().Contains(".P7M"))
                        {
                            cha_firmato = "1";
                            cha_tipo_firma = "C";
                        }

                        while (filenameX.EndsWith(".p7m") || filenameX.EndsWith(".tsd"))
                        {
                            filenameX = filenameX.Substring(0, filenameX.Length - 4);
                        }

                        q3.setParam("ext", "'"+filenameX.Substring(filenameX.LastIndexOf('.') + 1)+"'");
                        q3.setParam("docnumber", infoFile.IdDocument);
                        queryString = q3.getSQL();
                        logger.Debug(queryString);
                        retval = this.ExecuteNonQuery(queryString);

                        DocsPaUtils.Query q4 = DocsPaUtils.InitQuery.getInstance().getQuery("U_Components");
                        q4.setParam("param1", string.Format(
                            "PATH = '{0}', FILE_SIZE = {1}, VAR_IMPRONTA = '{2}', VAR_NOMEORIGINALE='{3}', ID_PEOPLE_PUTFILE={4}, DTA_FILE_ACQUIRED = SYSDATE, CHA_TIPO_FIRMA = '{5}', CHA_FIRMATO = '{6}'",
                            infoFile.FSPath, infoFile.FileSize, infoFile.HashFile.ToUpper(), infoFile.FileName, infoFile.UploaderId, cha_tipo_firma, cha_firmato));
                        q4.setParam("param2", "VERSION_ID = " + infoFile.VersionId);
                        queryString = q4.getSQL();
                        logger.Debug(queryString);
                        retval = this.ExecuteNonQuery(queryString);
                        
                    
                    }
                }

                
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retval = false;
            }
            return retval;
        }

        public DocsPaVO.ExternalServices.FileFtpUpInfo BigFilesFTP_GetInfoFile(string idQueue, string idDoc)
        {
            DocsPaVO.ExternalServices.FileFtpUpInfo retVal = null;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPABIGFILE");
                if (!string.IsNullOrEmpty(idQueue))
                    q.setParam("condizione", " A.SYSTEM_ID = " + idQueue);
                else
                    q.setParam("condizione", " A.IDPROFILE = " + idDoc + " ORDER BY A.SYSTEM_ID DESC");

                string queryString = q.getSQL();
                logger.Debug(queryString);
                // Inserire oggetto Evento CDS
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "BFFTPINFO", queryString);
                if (dataset.Tables["BFFTPINFO"] != null && dataset.Tables["BFFTPINFO"].Rows.Count > 0)
                {
                    retVal = new DocsPaVO.ExternalServices.FileFtpUpInfo();
                    retVal.IdQueue = dataset.Tables["BFFTPINFO"].Rows[0]["IDQUEUE"].ToString();
                    retVal.IdDocument = dataset.Tables["BFFTPINFO"].Rows[0]["IDDOCUMENT"].ToString();
                    retVal.Description = dataset.Tables["BFFTPINFO"].Rows[0]["DESCRIPTION"].ToString();
                    retVal.IdAmm = dataset.Tables["BFFTPINFO"].Rows[0]["ID_AMM"].ToString();
                    retVal.CodeAdm = dataset.Tables["BFFTPINFO"].Rows[0]["CODEADM"].ToString();
                    retVal.UploaderId = dataset.Tables["BFFTPINFO"].Rows[0]["UPLOADERID"].ToString();
                    retVal.Uploader = dataset.Tables["BFFTPINFO"].Rows[0]["UPLOADER"].ToString();
                    retVal.UploaderRoleId = dataset.Tables["BFFTPINFO"].Rows[0]["UPLOADERROLEID"].ToString();
                    retVal.UploaderRole = dataset.Tables["BFFTPINFO"].Rows[0]["UPLOADERROLE"].ToString();
                    retVal.FileName = dataset.Tables["BFFTPINFO"].Rows[0]["FILENAME"].ToString();
                    retVal.FileSize = dataset.Tables["BFFTPINFO"].Rows[0]["FILESIZE"].ToString();
                    retVal.HashFile = dataset.Tables["BFFTPINFO"].Rows[0]["HASHFILE"].ToString();
                    retVal.FTPPath = dataset.Tables["BFFTPINFO"].Rows[0]["FTPPATH"].ToString();
                    retVal.Status = dataset.Tables["BFFTPINFO"].Rows[0]["STATUS"].ToString();
                    retVal.ErrorMessage = dataset.Tables["BFFTPINFO"].Rows[0]["ERRORMESSAGE"].ToString();
                    retVal.VersionId = dataset.Tables["BFFTPINFO"].Rows[0]["VERSIONID"].ToString();
                    retVal.Version = dataset.Tables["BFFTPINFO"].Rows[0]["VERSION"].ToString();
                    retVal.FSPath = dataset.Tables["BFFTPINFO"].Rows[0]["FSPATH"].ToString();
                                        
                }
            }
            catch (Exception ex) {
                logger.Error(ex);
                retVal = null;
            }
            return retVal;
        }
        #endregion

    }
}
