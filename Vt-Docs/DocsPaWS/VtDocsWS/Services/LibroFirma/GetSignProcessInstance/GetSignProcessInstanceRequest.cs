using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.LibroFirma.GetSignProcessInstance
{
    [DataContract]
    public class GetSignProcessInstanceRequest : Request
    {
        [DataMember]
        public string IdProcessInstance { get; set; }
    }
}