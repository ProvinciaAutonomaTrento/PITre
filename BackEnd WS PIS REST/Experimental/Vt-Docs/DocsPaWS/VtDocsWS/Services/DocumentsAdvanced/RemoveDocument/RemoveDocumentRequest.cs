using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.DocumentsAdvanced.RemoveDocument
{
    [DataContract]
    public class RemoveDocumentRequest : Request
    {
        [DataMember]
        public string IdDocument
        {
            get;
            set;
        }

        [DataMember]
        public string RemovalNote
        {
            get;
            set;
        }
    }
}