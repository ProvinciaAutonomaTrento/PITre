using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.DocumentsAdvanced.SendDocumentAdvanced
{
    [DataContract]
    public class SendDocumentAdvancedRequest : Request
    {
        [DataMember]
        public string IdDocument { get; set; }

        [DataMember]
        public string Signature { get; set; }

        [DataMember]
        public string IdRegister { get; set; }

        [DataMember]
        public string CodeRegister { get; set; }

        [DataMember]
        public string SenderMail { get; set; }

        [DataMember]
        public Domain.Correspondent[] Recipients { get; set; }
    }
}