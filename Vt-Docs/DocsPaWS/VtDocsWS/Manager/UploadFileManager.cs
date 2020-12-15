using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using VtDocsWS.WebServices;
using log4net;

namespace VtDocsWS.Manager
{
    public class UploadFileManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(UploadFileManager));

        public static string GetUploaderVersion()
        {
            string retVal = string.Empty;
            DocsPaDB.Query_DocsPAWS.UploadFiles uploadFilesDb = new DocsPaDB.Query_DocsPAWS.UploadFiles();
            retVal = uploadFilesDb.GetUploaderVersion();

            return retVal;
        }

        /// <summary>
        /// Inizio o riprendo l'upload di un file per l'utente loggato
        /// </summary>
        /// <param name="request"></param>
        /// <param name="infoUtente"></param>
        /// <returns>Response</returns>
        public static Services.FilesUploader.UploadFile.UploadFileResponse UploadFile(Services.FilesUploader.UploadFile.UploadFileRequest request, DocsPaVO.utente.InfoUtente infoUtente)
        {
            Services.FilesUploader.UploadFile.UploadFileResponse response = new Services.FilesUploader.UploadFile.UploadFileResponse();
            DocsPaDB.Query_DocsPAWS.UploadFiles uploadFilesDb = new DocsPaDB.Query_DocsPAWS.UploadFiles();
            Domain.FileInUpload fileInUpClient = request.FileInUp;

            try
            {
                if (fileInUpClient != null)
                {
                    //DocsPaVO.UploadFiles.FileInUpload fileInUpServer = uploadFilesDb.GetFileInUpload(fileInUpClient.fileHash, infoUtente);
                    DocsPaVO.UploadFiles.FileInUpload fileInUpServer = uploadFilesDb.GetFileInUpload(fileInUpClient.strIdentity, infoUtente);

                    if (fileInUpServer != null && fileInUpClient.chunkNumber > -1)
                    {
                        if (fileInUpServer.ChunkNumber >= fileInUpClient.chunkNumber)
                        {
                            throw new PisException("FILE_ALREDY_EXIST");
                        }

                        if (string.IsNullOrEmpty(fileInUpServer.RepositoryPath))
                        {
                            string pathOnServer = (System.Configuration.ConfigurationManager.AppSettings["UPLOAD_BIGFILE_REPOSITORY"] != null && !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["UPLOAD_BIGFILE_REPOSITORY"].ToString()) ? System.Configuration.ConfigurationManager.AppSettings["UPLOAD_BIGFILE_REPOSITORY"] : string.Empty);
                            if (!string.IsNullOrEmpty(pathOnServer))
                            {
                                pathOnServer = pathOnServer + @"\" + request.CodeAdm + @"\" + infoUtente.idPeople;
                                if (!string.IsNullOrEmpty(fileInUpClient.fileDescription))
                                    pathOnServer = pathOnServer + @"\" + fileInUpClient.fileDescription;

                                Directory.CreateDirectory(pathOnServer);

                                fileInUpServer.RepositoryPath = pathOnServer;
                            }
                        }

                        if (Directory.Exists(fileInUpServer.RepositoryPath))
                        {
                            string pathSulServer = fileInUpServer.RepositoryPath + @"\" + fileInUpClient.fileName;
                            FileStream file = new FileStream(pathSulServer, FileMode.Create);
                            file.Write(fileInUpClient.fileContent, 0, fileInUpClient.fileContent.Length);
                            file.Flush();
                            file.Close();

                            fileInUpServer.ChunkNumber = fileInUpClient.chunkNumber;
                            fileInUpServer.TotalChunkNumber = fileInUpClient.totalChunkNumber;
                            fileInUpServer.FileName = fileInUpClient.fileName;
                            fileInUpServer.FileDescription = fileInUpClient.fileDescription;
                            fileInUpServer.Order = fileInUpClient.order;
                            fileInUpServer.IdRuolo = fileInUpClient.idRuolo;

                            if (uploadFilesDb.UpdateFileInUpload(fileInUpServer, infoUtente))
                            {
                                if (fileInUpServer.ChunkNumber == fileInUpServer.TotalChunkNumber - 1)
                                {
                                    Compression.CompressionManager compressionManager = new Compression.CompressionManager();
                                    string[] volumesFileZip = new string[fileInUpServer.TotalChunkNumber];
                                    string[] fileNamePart = fileInUpServer.FileName.Split('.');
                                    string fileExt = fileNamePart[fileNamePart.Length - 1];
                                    //string numeroFileZip = fileNamePart[fileNamePart.Length - 2];
                                    string partialFileName = string.Empty;

                                    for (int j = 0; j < fileNamePart.Length - 2; j++)
                                    {
                                        partialFileName = partialFileName + fileNamePart[j] + ".";
                                    }

                                    for (int i = 0; i < fileInUpServer.TotalChunkNumber; i++)
                                    {
                                        volumesFileZip[i] = (partialFileName + i.ToString() + "." + fileExt);
                                    }

                                    string[] splitName;
                                    if (fileInUpServer.FileSenderPath.Contains('/'))
                                        splitName = fileInUpServer.FileSenderPath.Split('/');
                                    else
                                        splitName = fileInUpServer.FileSenderPath.Split('\\');

                                    string fileNameUncompressed = splitName[splitName.Length - 1];

                                    if (compressionManager.DecompressArchiveVolumes(fileInUpServer.RepositoryPath, fileNameUncompressed, volumesFileZip, true))
                                    {
                                        fileInUpServer.ChunkNumber = fileInUpServer.TotalChunkNumber;
                                        fileInUpServer.FileName = fileNameUncompressed;
                                        uploadFilesDb.UpdateFileInUpload(fileInUpServer, infoUtente);
                                        response.Success = true;
                                    }
                                }
                                else
                                {
                                    response.Success = true;
                                }
                            }
                        }
                        else
                        {
                            throw new PisException("SERVER_PATH_NOT_FOUND");
                        }
                    }
                    else
                    {
                        fileInUpServer = new DocsPaVO.UploadFiles.FileInUpload();

                        string pathOnServer = (System.Configuration.ConfigurationManager.AppSettings["UPLOAD_BIGFILE_REPOSITORY"] != null && !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["UPLOAD_BIGFILE_REPOSITORY"].ToString()) ? System.Configuration.ConfigurationManager.AppSettings["UPLOAD_BIGFILE_REPOSITORY"] : string.Empty);

                        if (!string.IsNullOrEmpty(pathOnServer))
                        {
                            if (string.IsNullOrEmpty(fileInUpClient.strIdentity))
                                fileInUpClient.strIdentity = (fileInUpClient.fileDescription.ToUpper().TrimStart().TrimEnd() + "#" + fileInUpClient.fileName.ToUpper().TrimStart().TrimEnd());

                            pathOnServer = pathOnServer + @"\" + request.CodeAdm + @"\" + infoUtente.idPeople;
                            Directory.CreateDirectory(pathOnServer);

                            string pathFileOnServer = pathOnServer + @"\" + fileInUpClient.fileName;
                            FileStream file = new FileStream(pathFileOnServer, FileMode.Create);
                            file.Write(fileInUpClient.fileContent, 0, fileInUpClient.fileContent.Length);
                            file.Flush();
                            file.Close();

                            fileInUpServer.ChunkNumber = 0;
                            fileInUpServer.FileHash = fileInUpClient.fileHash;
                            fileInUpServer.FileName = fileInUpClient.fileName;
                            fileInUpServer.FileDescription = fileInUpClient.fileDescription;
                            fileInUpServer.FileSenderPath = fileInUpClient.fileSenderPath;
                            fileInUpServer.FileSize = fileInUpClient.fileSize;
                            fileInUpServer.MachineName = fileInUpClient.machineName;
                            fileInUpServer.TotalChunkNumber = fileInUpClient.totalChunkNumber;
                            fileInUpServer.RepositoryPath = pathOnServer;
                            fileInUpServer.Order = fileInUpClient.order;
                            fileInUpServer.IdRuolo = fileInUpClient.idRuolo;
                            fileInUpServer.StrIdentity = fileInUpClient.strIdentity;

                            if (uploadFilesDb.InsertFileInUpload(fileInUpServer, infoUtente))
                            {
                                response.StrIdentity = fileInUpServer.StrIdentity;
                                response.Success = true;
                            }
                        }
                        else
                        {
                            throw new PisException("SERVER_CONFIGURATION_ERROR");
                        }
                    }
                }
                else
                {
                    throw new PisException("PARAMETER_FILEINUP_MISSING");
                }
            }
            catch (PisException pisEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }

            return response;
        }

        /// <summary>
        /// Cancellazione di tutte le parti relative al file in upload
        /// </summary>
        /// <param name="request"></param>
        /// <param name="infoUtente"></param>
        /// <returns>Response</returns>
        public static Services.FilesUploader.UploadFile.UploadFileResponse DeleteFileInUpload(Services.FilesUploader.UploadFile.UploadFileRequest request, DocsPaVO.utente.InfoUtente infoUtente)
        {
            Services.FilesUploader.UploadFile.UploadFileResponse response = new Services.FilesUploader.UploadFile.UploadFileResponse();
            DocsPaDB.Query_DocsPAWS.UploadFiles uploadFilesDb = new DocsPaDB.Query_DocsPAWS.UploadFiles();
            Domain.FileInUpload fileInUpClient = request.FileInUp;

            try
            {
                if (fileInUpClient != null)
                {
                    //DocsPaVO.UploadFiles.FileInUpload fileInUpServer = uploadFilesDb.GetFileInUpload(fileInUpClient.fileHash, infoUtente);
                    DocsPaVO.UploadFiles.FileInUpload fileInUpServer = uploadFilesDb.GetFileInUpload(fileInUpClient.strIdentity, infoUtente);

                    if (fileInUpServer != null)
                    {
                        string pathSulServer = string.Empty;

                        if (fileInUpServer != null && !string.IsNullOrEmpty(fileInUpServer.StrIdentity))
                        {
                            pathSulServer = fileInUpServer.RepositoryPath; // +@"\" + fileInUpClient.fileName;
                        }
                        else
                        {
                            pathSulServer = (System.Configuration.ConfigurationManager.AppSettings["UPLOAD_BIGFILE_REPOSITORY"] != null && !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["UPLOAD_BIGFILE_REPOSITORY"].ToString()) ? System.Configuration.ConfigurationManager.AppSettings["UPLOAD_BIGFILE_REPOSITORY"] : string.Empty);
                        }

                        if (!string.IsNullOrEmpty(pathSulServer) && Directory.Exists(fileInUpServer.RepositoryPath))
                        {
                            string[] fileNamePart = fileInUpServer.FileName.Split('.');
                            string fileExt = fileNamePart[fileNamePart.Length - 1];
                            string partialFileName = fileInUpServer.FileName.Substring(0, fileInUpServer.FileName.Length - (fileExt.Length + 2));

                            for (int i = 0; i < fileInUpServer.TotalChunkNumber; i++)
                            {
                                string completeFileName = @"\" + partialFileName + i.ToString() + "." + fileExt;
                                File.Delete(pathSulServer + completeFileName);
                            }

                            //uploadFilesDb.DeleteFileInUpload(fileInUpClient.fileHash, infoUtente);
                            uploadFilesDb.DeleteFileInUpload(fileInUpClient.fileName, fileInUpClient.fileDescription, infoUtente);

                            response.Success = true;
                        }
                        else
                        {
                            uploadFilesDb.DeleteFileInUpload(fileInUpClient.fileName, fileInUpClient.fileDescription, infoUtente);
                        }
                    }
                    else
                    {
                        throw new PisException("PARAMETER_FILEINUP_MISSING");
                    }
                }
                else
                {
                    throw new PisException("FILE_NOT_EXIST");
                }
            }
            catch (PisException pisEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }

            return response;
        }

        /// <summary>
        /// Nuovo file nella lista degli upload
        /// </summary>
        /// <param name="request"></param>
        /// <param name="infoUtente"></param>
        /// <returns>Response</returns>
        public static Services.FilesUploader.UploadFile.UploadFileResponse AddNewFile(Services.FilesUploader.UploadFile.UploadFileRequest request, DocsPaVO.utente.InfoUtente infoUtente)
        {
            Services.FilesUploader.UploadFile.UploadFileResponse response = new Services.FilesUploader.UploadFile.UploadFileResponse();
            DocsPaDB.Query_DocsPAWS.UploadFiles uploadFilesDb = new DocsPaDB.Query_DocsPAWS.UploadFiles();
            Domain.FileInUpload fileInUpClient = request.FileInUp;

            try
            {
                if (fileInUpClient != null)
                {
                    //DocsPaVO.UploadFiles.FileInUpload fileInUpServer = uploadFilesDb.GetFileInUpload(fileInUpClient.fileHash, infoUtente);
                    if (string.IsNullOrEmpty(fileInUpClient.strIdentity))
                        fileInUpClient.strIdentity = (fileInUpClient.fileDescription.ToUpper().TrimStart().TrimEnd() + "#" + fileInUpClient.fileName.ToUpper().TrimStart().TrimEnd());

                    DocsPaVO.UploadFiles.FileInUpload fileInUpServer = uploadFilesDb.GetFileInUpload(fileInUpClient.strIdentity, infoUtente);

                    if (fileInUpServer != null && !string.IsNullOrEmpty(fileInUpServer.StrIdentity))
                    {
                        throw new PisException("FILE_ALREDY_EXIST");
                    }

                    fileInUpServer = new DocsPaVO.UploadFiles.FileInUpload();

                    fileInUpServer.ChunkNumber = -1;
                    fileInUpServer.FileHash = fileInUpClient.fileHash;
                    fileInUpServer.FileName = fileInUpClient.fileName;
                    fileInUpServer.FileDescription = fileInUpClient.fileDescription;
                    fileInUpServer.FileSenderPath = fileInUpClient.fileSenderPath;
                    fileInUpServer.FileSize = fileInUpClient.fileSize;
                    fileInUpServer.MachineName = fileInUpClient.machineName;
                    fileInUpServer.TotalChunkNumber = 0;
                    fileInUpServer.RepositoryPath = string.Empty;
                    fileInUpServer.Order = fileInUpClient.order;
                    fileInUpServer.IdRuolo = fileInUpClient.idRuolo;
                    fileInUpServer.StrIdentity = fileInUpClient.strIdentity;

                    if (uploadFilesDb.InsertFileInUpload(fileInUpServer, infoUtente))
                    {
                        response.StrIdentity = fileInUpServer.StrIdentity;
                        response.Success = true;
                    }
                }
                else
                {
                    throw new PisException("PARAMETER_FILEINUP_MISSING");
                }
            }
            catch (PisException pisEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }

            return response;
        }

        /// <summary>
        /// Fornisco informazione su eventuali upload in sospeso per l'utente loggato
        /// </summary>
        /// <param name="request"></param>
        /// <param name="infoUtente"></param>
        /// <returns>Response</returns>
        public static Services.FilesUploader.GetFilesState.GetFilesStateResponse GetFilesState(Services.FilesUploader.GetFilesState.GetFilesStateRequest request, DocsPaVO.utente.InfoUtente infoUtente)
        {
            Services.FilesUploader.GetFilesState.GetFilesStateResponse response = new Services.FilesUploader.GetFilesState.GetFilesStateResponse();

            List<DocsPaVO.UploadFiles.FileInUpload> listFile = new List<DocsPaVO.UploadFiles.FileInUpload>();

            try
            {
                DocsPaDB.Query_DocsPAWS.UploadFiles uploadFilesDb = new DocsPaDB.Query_DocsPAWS.UploadFiles();

                listFile = uploadFilesDb.GetListFilesInUpload(request.MachineName, infoUtente);

                response.FileInUp = NewListFilesStateClient(listFile);
                response.Success = true;
            }
            catch (PisException pisEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }

            return response;
        }

        private static List<Domain.FileInUpload> NewListFilesStateClient(List<DocsPaVO.UploadFiles.FileInUpload> listFile)
        {
            List<Domain.FileInUpload> newList = new List<Domain.FileInUpload>();

            foreach (DocsPaVO.UploadFiles.FileInUpload fileInUpServer in listFile)
            {
                Domain.FileInUpload fileInUpClient = new Domain.FileInUpload();

                fileInUpClient.idRuolo = fileInUpServer.IdRuolo;
                fileInUpClient.chunkNumber = fileInUpServer.ChunkNumber;
                fileInUpClient.fileHash = fileInUpServer.FileHash;
                fileInUpClient.fileName = fileInUpServer.FileName;
                fileInUpClient.fileDescription = fileInUpServer.FileDescription;
                fileInUpClient.fileSenderPath = fileInUpServer.FileSenderPath;
                fileInUpClient.fileSize = fileInUpServer.FileSize;
                fileInUpClient.machineName = fileInUpServer.MachineName;
                fileInUpClient.totalChunkNumber = fileInUpServer.TotalChunkNumber;
                fileInUpClient.order = fileInUpServer.Order;
                fileInUpClient.strIdentity = fileInUpServer.StrIdentity;

                newList.Add(fileInUpClient);
            }

            return newList;
        }

        public static Services.FilesUploader.FileFTPUpload.FileFTPUploadResponse NotifyFileFTPUpload(Services.FilesUploader.FileFTPUpload.NotifyFileFTPUploadRequest request)
        {
            Services.FilesUploader.FileFTPUpload.FileFTPUploadResponse response = new Services.FilesUploader.FileFTPUpload.FileFTPUploadResponse();
            try
            {
                bool retval1 = false;
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "NotifyFileFTPUpload");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                //TODO
                int sizeInt = 0, maxSize = 104857600;
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FE_DO_BIG_FILE_MAX")))
                {
                    if(!Int32.TryParse(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FE_DO_BIG_FILE_MAX"), out maxSize))
                        maxSize = 104857600;
                }

                if(!string.IsNullOrEmpty(request.FileSize))
                {
                    if (Int32.TryParse(request.FileSize, out sizeInt))
                    {
                        if (sizeInt > maxSize)
                            throw new Exception("Dimensione del file troppo grande");
                    }
                    else { throw new Exception("Dimensione file non valida"); }
                }
                else
                {
                    throw new Exception("Dimensione del file obbligatoria");
                }


                DocsPaVO.ExternalServices.FileFtpUpInfo ftpInfo = new DocsPaVO.ExternalServices.FileFtpUpInfo();
                ftpInfo.CodeAdm = request.CodeAdm;
                ftpInfo.IdAmm = infoUtente.idAmministrazione;
                ftpInfo.UploaderId = infoUtente.idPeople;
                ftpInfo.UploaderRoleId = infoUtente.idGruppo;
                ftpInfo.IdDocument = request.IdDocument;
                ftpInfo.FileName = request.FileName;
                ftpInfo.HashFile = request.FileHash;
                ftpInfo.FileSize = request.FileSize;
                ftpInfo.FTPPath = request.FTPPath;

                retval1 = BusinessLogic.Amministrazione.SistemiEsterni.BigFilesFTP_InsertIntoTable(ftpInfo);
                if (retval1)
                {
                    DocsPaVO.ExternalServices.FileFtpUpInfo ftpInfoResponse = BusinessLogic.Amministrazione.SistemiEsterni.BigFileFTP_GetInfoFileFTP("", request.IdDocument);
                    Domain.FileFTPUploadInfo retInfo = Utils.getDomFileFTPInfo(ftpInfoResponse);
                    response.InfoUpload = retInfo;
                }

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.FilesUploader.FileFTPUpload.FileFTPUploadResponse CheckFileFTPUpload(Services.FilesUploader.FileFTPUpload.CheckFileFTPUploadRequest request)
        {
            Services.FilesUploader.FileFTPUpload.FileFTPUploadResponse response = new Services.FilesUploader.FileFTPUpload.FileFTPUploadResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "CheckFileFTPUpload");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                DocsPaVO.ExternalServices.FileFtpUpInfo ftpInfoResponse = BusinessLogic.Amministrazione.SistemiEsterni.BigFileFTP_GetInfoFileFTP(request.IdQueue, request.IdDocument);
                Domain.FileFTPUploadInfo retInfo = Utils.getDomFileFTPInfo(ftpInfoResponse);
                response.InfoUpload = retInfo;

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }
    }
}