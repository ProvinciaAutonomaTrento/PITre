using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.LibroFirma.GetSignProcessInstance
{
    [DataContract]
    public class GetSignProcessInstanceResponse : Response
    {
        [DataMember]
        public Domain.SignBook.SignatureProcessInstance ProcessInstance { get; set; }
    }
}