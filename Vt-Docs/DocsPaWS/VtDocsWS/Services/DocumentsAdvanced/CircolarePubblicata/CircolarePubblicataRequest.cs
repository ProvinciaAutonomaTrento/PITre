using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.DocumentsAdvanced.CircolarePubblicata
{
    [DataContract]
    public class CircolarePubblicataRequest
    {
        [DataMember]
        public string CodeAdm { get; set; }

        [DataMember]
        public string EspiCodice { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}