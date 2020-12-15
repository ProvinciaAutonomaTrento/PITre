using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.CAPServices.Authenticate
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "Authenticate"
    /// </summary>
    [DataContract]
    public class AuthenticateResponse : Response
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

        /// <summary>
        /// Utente
        /// </summary>
        [DataMember]
        public Domain.Correspondent User
        {
            get;
            set;
        }
    }
}