using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.FilesUploader.FileFTPUpload
{
    [DataContract]
    public class FileFTPUploadResponse : Response
    {
        [DataMember]
        public Domain.FileFTPUploadInfo InfoUpload { get; set; }
    }
}