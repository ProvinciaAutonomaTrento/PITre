using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.FilesUploader.UploadFile
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "UploadFile"
    /// </summary>
    [DataContract]
    public class UploadFileRequest : Request
    {
        /// <summary>
        /// File in upload
        /// </summary>
        [DataMember]
        public Domain.FileInUpload FileInUp
        {
            get;
            set;
        }
    }
}