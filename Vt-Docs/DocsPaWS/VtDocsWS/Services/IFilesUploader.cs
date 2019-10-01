using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace VtDocsWS.Services
{
    /// <summary>
    ///  Definizione dei servizi per FilesUpload dei Product Integration Services.  
    /// </summary>
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface IFilesUploader
    {
        [OperationContract]
        VtDocsWS.Services.FilesUploader.UploaderVersion.UploaderVersionResponse GetUploaderVersion(VtDocsWS.Services.FilesUploader.UploaderVersion.UploaderVersionRequest request);

        [OperationContract]
        VtDocsWS.Services.FilesUploader.UploadFile.UploadFileResponse UploadFile(VtDocsWS.Services.FilesUploader.UploadFile.UploadFileRequest request);

        [OperationContract]
        VtDocsWS.Services.FilesUploader.UploadFile.UploadFileResponse DeleteFileInUpload(VtDocsWS.Services.FilesUploader.UploadFile.UploadFileRequest request);

        [OperationContract]
        VtDocsWS.Services.FilesUploader.UploadFile.UploadFileResponse AddNewFile(VtDocsWS.Services.FilesUploader.UploadFile.UploadFileRequest request);

        [OperationContract]
        VtDocsWS.Services.FilesUploader.GetFilesState.GetFilesStateResponse GetFilesState(VtDocsWS.Services.FilesUploader.GetFilesState.GetFilesStateRequest request);

        [OperationContract]
        VtDocsWS.Services.FilesUploader.FileFTPUpload.FileFTPUploadResponse NotifyFileFTPUpload(VtDocsWS.Services.FilesUploader.FileFTPUpload.NotifyFileFTPUploadRequest request);

        [OperationContract]
        VtDocsWS.Services.FilesUploader.FileFTPUpload.FileFTPUploadResponse CheckFileFTPUpload(VtDocsWS.Services.FilesUploader.FileFTPUpload.CheckFileFTPUploadRequest request);
    }
}