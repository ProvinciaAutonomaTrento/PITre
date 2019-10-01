using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Proceedings.AddDocToProceeding
{
    public class AddDocToProceedingResponse : Response
    {
        [DataMember]
        public String IdDocument { get; set; }
    }
}