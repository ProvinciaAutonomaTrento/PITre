using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Proceedings.GetPhases
{
    [DataContract]
    public class GetPhasesRequest : Request
    {
        [DataMember]
        public string IdProject { get; set; }
    }
}