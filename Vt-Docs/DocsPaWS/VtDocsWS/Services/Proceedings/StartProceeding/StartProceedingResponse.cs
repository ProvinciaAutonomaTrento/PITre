using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Proceedings.StartProceeding
{
    [DataContract]
    public class StartProceedingResponse : Response
    {
        [DataMember]
        public string IdProject { get; set; }

        [DataMember]
        public string IdDocument { get; set; }
    }
}