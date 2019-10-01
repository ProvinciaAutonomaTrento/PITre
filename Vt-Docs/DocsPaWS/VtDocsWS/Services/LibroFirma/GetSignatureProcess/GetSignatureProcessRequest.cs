using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.LibroFirma.GetSignatureProcess
{
    [DataContract]
    public class GetSignatureProcessRequest : Request
    {
        [DataMember]
        public string IdProcess { get; set; }
    }
}