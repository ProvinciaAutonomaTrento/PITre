using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace VtDocsWS.Services.Authentication.Authenticate
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "Authenticate"
    /// </summary>
    [DataContract]
    public class AuthenticateRequest : Request
    {
        /// <summary>
        /// Password utente
        /// </summary>
        [DataMember]
        public string Password
        {
            get;
            set;
        }
    }
}