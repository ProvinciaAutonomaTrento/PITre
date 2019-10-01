using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace VtDocsWS.Services.CAPServices.Authenticate
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "Authenticate"
    /// </summary>
    [DataContract]
    public class AuthenticateRequest 
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

        [DataMember]
        public string Username
        { get; set; }

        [DataMember]
        public string Email
        { get; set; }
    }
}