using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VtDocsWS.Services.AdL.AddDocumentInAdLRuolo
{
    public class AddDocumentInAdLRuoloRequest : Request
    {
        [DataMember]
        public string IdProfile
        {
            get;
            set;
        }

        [DataMember]
        public string TipoProto
        {
            get;
            set;
        }

        [DataMember]
        public string IdRegistro
        {
            get;
            set;
        }
    }
}