using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using VtDocsWS.Domain;

namespace VtDocsWS.Services.PluginHash.GetHashMail
{
    public class GetHashMailResponse : Response
    {
        [DataMember]
        public DpaPluginHash DpaPluginHash
        {
            get;
            set;
        }
    }
}