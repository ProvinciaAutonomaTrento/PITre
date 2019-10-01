using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VtDocsWS.Services.AdL.AddDocumentInAdLUtente
{
    public class AddDocumentInAdLUtenteResponse : Response
    {
        [DataMember]
        public bool Result
        {
            get;
            set;
        }
    }
}