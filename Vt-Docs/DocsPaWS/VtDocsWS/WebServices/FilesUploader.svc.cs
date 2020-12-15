using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Runtime.Serialization;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using log4net;
using System.Web;
using DocsPaWS.VtDocsWS;
using VtDocsWS.Services;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace VtDocsWS.WebServices
{
    /// <summary>
    /// Metodi per la gestione dei file di grandi dimensioni
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class FilesUploader : IFilesUploader
    {
        private ILog logger = LogManager.GetLogger(typeof(FilesUploader));


        /// <summary>
        /// Restituisce la versione accettata dal sistema
        /// </summary>
        /// <param name="request"></param>
        /// <returns>string</returns>
        public VtDocsWS.Services.FilesUploader.UploaderVersion.UploaderVersionResponse GetUploaderVersion(VtDocsWS.Services.FilesUploader.UploaderVersion.UploaderVersionRequest versionRequest)
        {
            logger.Info("BEGIN GetUploaderVersion");

            string result = string.Empty;
            result = Manager.UploadFileManager.GetUploaderVersion();

            VtDocsWS.Services.FilesUploader.UploaderVersion.UploaderVersionResponse responseVersion = new Services.FilesUploader.UploaderVersion.UploaderVersionResponse();
            responseVersion.uploaderVersion = result;

            logger.Info("END GetUploaderVersion");

            return responseVersion;
        }

        /// <summary>
        /// Invio delle parti di file in upload
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.FilesUploader.UploadFile.UploadFileResponse UploadFile(Services.FilesUploader.UploadFile.UploadFileRequest request)
        {
            logger.Info("BEGIN UploadFile");

            Request newRequest = new Request();
            newRequest.UserName = request.UserName;
            newRequest.AuthenticationToken = request.AuthenticationToken;
            newRequest.CodeAdm = request.CodeAdm;

            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(newRequest, "");

            Services.FilesUploader.UploadFile.UploadFileResponse response = Manager.UploadFileManager.UploadFile(request, infoUtente);

            //DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            //BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "ADDELEMINLF", request.IdPasso, "Creazione nuovo elemento in libro firma per il passo. " + request.IdPasso + " in modalità " + request.Modalita, esito);

            logger.Info("END UploadFile");

            Utils.CheckFaultException(response);

            response.IsSuccess = response.Success;

            return response;
        }

        /// <summary>
        /// Invio delle parti di file in upload
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.FilesUploader.UploadFile.UploadFileResponse DeleteFileInUpload(Services.FilesUploader.UploadFile.UploadFileRequest request)
        {
            logger.Info("BEGIN DeleteFileInUpload");

            Request newRequest = new Request();
            newRequest.UserName = request.UserName;
            newRequest.AuthenticationToken = request.AuthenticationToken;
            newRequest.CodeAdm = request.CodeAdm;

            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(newRequest, "");

            Services.FilesUploader.UploadFile.UploadFileResponse response = Manager.UploadFileManager.DeleteFileInUpload(request, infoUtente);

            //DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            //BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "ADDELEMINLF", request.IdPasso, "Creazione nuovo elemento in libro firma per il passo. " + request.IdPasso + " in modalità " + request.Modalita, esito);

            logger.Info("END DeleteFileInUpload");

            Utils.CheckFaultException(response);

            response.IsSuccess = response.Success;

            return response;
        }

        /// <summary>
        /// Aggiunge nuovo file dalla lista degli upload
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.FilesUploader.UploadFile.UploadFileResponse AddNewFile(Services.FilesUploader.UploadFile.UploadFileRequest request)
        {
            logger.Info("BEGIN AddNewFile");

            Request newRequest = new Request();
            newRequest.UserName = request.UserName;
            newRequest.AuthenticationToken = request.AuthenticationToken;
            newRequest.CodeAdm = request.CodeAdm;

            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(newRequest, "");

            Services.FilesUploader.UploadFile.UploadFileResponse response = Manager.UploadFileManager.AddNewFile(request, infoUtente);

            logger.Info("END AddNewFile");

            Utils.CheckFaultException(response);

            response.IsSuccess = response.Success;

            return response;
        }

        /// <summary>
        /// Restituisce lo stato dell'upload precedente
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.FilesUploader.GetFilesState.GetFilesStateResponse GetFilesState(Services.FilesUploader.GetFilesState.GetFilesStateRequest request)
        {
            logger.Info("BEGIN GetFilesState");

            Request newRequest = new Request();
            newRequest.UserName = request.UserName;
            newRequest.AuthenticationToken = request.AuthenticationToken;
            newRequest.CodeAdm = request.CodeAdm;

            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(newRequest, "");

            Services.FilesUploader.GetFilesState.GetFilesStateResponse response = Manager.UploadFileManager.GetFilesState(request, infoUtente);
            
            //DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            //BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "ADDELEMINLF", request.IdIstanzaPasso, "Creazione nuovo elemento in libro firma per il passo. " + request.IdIstanzaPasso + " in modalità " + request.OrdinePasso, esito);
            logger.Info("END GetFilesState");

            Utils.CheckFaultException(response);

            response.IsSuccess = response.Success;

            return response;
        }

        public Services.FilesUploader.FileFTPUpload.FileFTPUploadResponse NotifyFileFTPUpload(Services.FilesUploader.FileFTPUpload.NotifyFileFTPUploadRequest request)
        {
            logger.Info("Begin NotifyFileFTPUpload");
            Services.FilesUploader.FileFTPUpload.FileFTPUploadResponse response = Manager.UploadFileManager.NotifyFileFTPUpload(request);
            Utils.CheckFaultException(response);
            logger.Info("END NotifyFileFTPUpload");
            return response;
        }

        public Services.FilesUploader.FileFTPUpload.FileFTPUploadResponse CheckFileFTPUpload(Services.FilesUploader.FileFTPUpload.CheckFileFTPUploadRequest request)
        {
            logger.Info("Begin CheckFileFTPUpload");
            Services.FilesUploader.FileFTPUpload.FileFTPUploadResponse response = Manager.UploadFileManager.CheckFileFTPUpload(request);
            Utils.CheckFaultException(response);
            logger.Info("END CheckFileFTPUpload");
            return response;
        }
    }
}
