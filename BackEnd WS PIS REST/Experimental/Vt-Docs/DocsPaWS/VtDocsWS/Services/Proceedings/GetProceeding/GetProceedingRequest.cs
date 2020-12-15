using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Proceedings.GetProceeding
{
    [DataContract]
    public class GetProceedingRequest : Request
    {
        [DataMember]
        public String IdProject { get; set; }
    }
}