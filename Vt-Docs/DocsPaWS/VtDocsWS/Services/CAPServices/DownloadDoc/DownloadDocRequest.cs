using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace VtDocsWS.Services.CAPServices.DownloadDoc
{
    [DataContract]
    public class DownloadDocRequest
    {
        [DataMember]
        public string AuthenticationToken { get; set; }

        [DataMember]
        public string IdDocument { get; set; }

    }
}