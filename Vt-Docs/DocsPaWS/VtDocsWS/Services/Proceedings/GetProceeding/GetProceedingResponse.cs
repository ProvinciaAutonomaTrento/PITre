using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Proceedings.GetProceeding
{
    [DataContract]
    public class GetProceedingResponse : Response
    {
        [DataMember]
        public Domain.Phase[] Phases { get; set; }

        [DataMember]
        public Domain.Project Proceeding { get; set; }

        [DataMember]
        public Domain.DocInProceeding[] Documents { get; set; }

        [DataMember]
        public int Status { get; set; }
    }
}