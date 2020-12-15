using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace DocsPaWS.VtDocsWS
{
    [DataContract]
    public class PISFaultException
    {
        public PISFaultException(string code, string description)
        {
            this.FaultCode = code;
            this.FaultDescription = description;
        }

        [DataMember]
        public string FaultCode { get; set; }

        [DataMember]
        public string FaultDescription { get; set; }
    }
}