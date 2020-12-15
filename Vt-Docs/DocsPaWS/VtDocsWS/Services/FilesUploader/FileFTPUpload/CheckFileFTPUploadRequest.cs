using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VtDocsWS.Services.FilesUploader.FileFTPUpload
{
    [DataContract]
    public class CheckFileFTPUploadRequest : Request
    {
        [DataMember]
        public string IdQueue { get; set; }

        [DataMember]
        public string IdDocument { get; set; }
    }
}