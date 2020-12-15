using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.APSS_Services.SendPublicationResult
{
    [DataContract]
    public class SendPublicationResultRequest : Request
    {
        [DataMember]
        public string IdDocument { get; set; }

        [DataMember]
        public string PublicationResult { get; set; }

        [DataMember]
        public string ResultReason { get; set; }
    }
}