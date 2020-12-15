using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.DocumentsAdvanced.FatturaEsitoNotifica
{
    [DataContract]
    public class FatturaEsitoNotificaResponse : Response
    {
        [DataMember]
        public string ResultMessage
        {
            get;
            set;
        }
    }
}