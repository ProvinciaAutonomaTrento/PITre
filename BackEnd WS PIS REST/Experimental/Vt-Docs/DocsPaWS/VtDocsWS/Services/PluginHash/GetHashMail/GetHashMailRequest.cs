using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VtDocsWS.Services.PluginHash.GetHashMail
{
    public class GetHashMailRequest : Request
    {
        [DataMember]
        public string HashFile
        {
            get;
            set;
        }
    }
}