using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Token.GetToken
{
    [DataContract]
    public class GetTokenResponse : Response 
    {
        [DataMember]
        public string AuthenticationToken { get; set; }

        [DataMember]
        public string TokenDuration { get; set; }

        [DataMember]
        public string Username { get; set; }
    }
}