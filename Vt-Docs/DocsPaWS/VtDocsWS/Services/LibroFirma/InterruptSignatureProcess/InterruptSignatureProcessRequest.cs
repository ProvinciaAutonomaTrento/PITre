using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.LibroFirma.InterruptSignatureProcess
{
    [DataContract]
    public class InterruptSignatureProcessRequest: Request
    {
        [DataMember]
        public string IdSignProcessInstance { get; set; }

        [DataMember]
        public string InterruptionNote { get; set; }
                
    }
}