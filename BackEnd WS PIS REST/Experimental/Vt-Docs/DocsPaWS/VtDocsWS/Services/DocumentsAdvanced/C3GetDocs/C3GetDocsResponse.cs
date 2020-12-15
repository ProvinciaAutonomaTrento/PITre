using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.DocumentsAdvanced.C3GetDocs
{
    [DataContract]
    public class C3GetDocsResponse : Response
    {
        [DataMember]
        public Domain.C3Document[] Documents
        { get; set; }

        [DataMember]
        public string TotalDocuments { get; set; }

        [DataMember]
        public string TotalAttachments { get; set; }
    }
}