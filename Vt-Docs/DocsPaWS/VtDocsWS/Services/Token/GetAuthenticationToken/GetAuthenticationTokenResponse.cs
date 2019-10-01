using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Token.GetAuthenticationToken
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetAuthenticationTokenResponse"
    /// </summary>
    [DataContract]
    public class GetAuthenticationTokenResponse : Response
    {
        /// <summary>
        /// File del documento richiesto con in più i dati di firma
        /// </summary>
        [DataMember]
        public string AuthenticationToken
        {
            get;
            set;
        }
    }
}