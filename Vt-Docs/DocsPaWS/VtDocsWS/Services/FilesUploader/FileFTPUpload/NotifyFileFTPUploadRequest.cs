using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VtDocsWS.Services.FilesUploader.FileFTPUpload
{
    [DataContract]
    public class NotifyFileFTPUploadRequest : Request
    {
        [DataMember]
        public string IdDocument { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string FileHash { get; set; }
        [DataMember]
        public string FileSize { get; set; }
        [DataMember]
        public string FTPPath { get; set; }

    }
}