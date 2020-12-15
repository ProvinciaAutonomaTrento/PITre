using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Transmissions.GiveUpRights
{
    [DataContract]
    public class GiveUpRightsResponse : Response
    {
        [DataMember]
        public string ResultMessage
        {
            get;
            set;
        }
       
    }
}