using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VtDocsWS.Services.AdL.AddDocumentInAdLRuolo
{
    public class AddDocumentInAdLRuoloResponse : Response
    {
        [DataMember]
        public bool Result
        {
            get;
            set;
        }
    }
}