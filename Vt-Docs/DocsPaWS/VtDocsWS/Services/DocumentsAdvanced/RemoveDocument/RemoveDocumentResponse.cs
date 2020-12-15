using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.DocumentsAdvanced.RemoveDocument
{
    [DataContract]
    public class RemoveDocumentResponse : Response
    {
        [DataMember]
        public string ResultMessage
        {
            get;
            set;
        }
    }
}