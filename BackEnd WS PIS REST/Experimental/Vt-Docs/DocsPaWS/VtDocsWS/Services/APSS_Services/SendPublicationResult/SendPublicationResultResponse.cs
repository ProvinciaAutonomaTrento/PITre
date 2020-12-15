using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.APSS_Services.SendPublicationResult
{
    [DataContract]
    public class SendPublicationResultResponse : Response
    {
        [DataMember]
        public string OperationResult { get; set; }
    }
}