using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Proceedings.StartProceeding
{
    [DataContract]
    public class StartProceedingRequest : Request
    {

        [DataMember]
        public Domain.Proceeding Proceeding { get; set; }

        [DataMember]
        public byte[] Content { get; set; }

        [DataMember]
        public string UserId { get; set; }
    }
}