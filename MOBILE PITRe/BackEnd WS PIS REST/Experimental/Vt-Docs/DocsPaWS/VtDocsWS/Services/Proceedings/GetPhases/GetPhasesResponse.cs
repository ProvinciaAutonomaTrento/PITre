using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Proceedings.GetPhases
{
    [DataContract]
    public class GetPhasesResponse : Response
    {
        public Domain.Phase[] Phases { get; set; }
    }
}