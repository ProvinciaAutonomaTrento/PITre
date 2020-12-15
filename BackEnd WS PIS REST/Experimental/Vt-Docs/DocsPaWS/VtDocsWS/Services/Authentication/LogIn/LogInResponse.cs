using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Authentication.LogIn
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "LogIn"
    /// </summary>
   [DataContract]
    public class LogInResponse : Response
    {
        /// <summary>
        /// Restituzione del token di autenticazione criptato 
        /// </summary>
        /// <remarks>
        /// Il token è generato dal sistema solo se l'autenticazione dell'utente è andata a buon fine.
        /// Essendo un token che prevede la scadenza esplicita, per rimuoverlo dal sistema sarà necessario
        /// richiamare il servizio di "LogOut" al termine del suo utilizzo.
        /// </remarks>
         [DataMember]
        public string AuthenticationToken
        {
            get;
            set;
        }
    }
}