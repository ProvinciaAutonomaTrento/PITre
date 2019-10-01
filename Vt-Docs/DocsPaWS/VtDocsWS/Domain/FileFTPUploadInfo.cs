using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class FileFTPUploadInfo
    {
        [DataMember]
        public string IdQueue { get; set; }

        [DataMember]
        public string IdDocument { get; set; }

        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string CodeAdm { get; set; }
        [DataMember]
        public string Uploader { get; set; }
        [DataMember]
        public string UploaderRole { get; set; }
        [DataMember]
        public string Filename { get; set; }
        [DataMember]
        public string FileSize { get; set; }
        [DataMember]
        public string HashFile { get; set; }
        [DataMember]
        public string FTPPath { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public string VersionId { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string FSPath { get; set; }



    }
}