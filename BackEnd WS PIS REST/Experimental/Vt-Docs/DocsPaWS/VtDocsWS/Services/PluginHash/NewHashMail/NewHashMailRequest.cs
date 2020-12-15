using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using VtDocsWS.Domain;

namespace VtDocsWS.Services.PluginHash.NewHashMail
{
    public class NewHashMailRequest : Request
    {
        [DataMember]
        public string HashFile
        {
            get;
            set;
        }

        [DataMember]
        public string IdPeople
        {
            get;
            set;
        }

        [DataMember]
        public string IdProfile
        {
            get;
            set;
        }
    }
}