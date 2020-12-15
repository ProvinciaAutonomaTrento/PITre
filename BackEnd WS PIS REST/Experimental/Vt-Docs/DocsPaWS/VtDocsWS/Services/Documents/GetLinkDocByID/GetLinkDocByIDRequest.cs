using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetLinkDocByID
{
    [DataContract]
    public class GetLinkDocByIDRequest : Request
    {
        [DataMember]
        public string IdDocument
        {
            get;
            set;
        }
    }
}