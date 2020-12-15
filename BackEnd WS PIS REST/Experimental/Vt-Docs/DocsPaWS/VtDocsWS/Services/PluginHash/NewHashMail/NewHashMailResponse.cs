using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VtDocsWS.Services.PluginHash.NewHashMail
{
    public class NewHashMailResponse : Response
    {
        [DataMember]
        public bool Result
        {
            get;
            set;
        }
    }
}