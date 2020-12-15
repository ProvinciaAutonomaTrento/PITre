using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Authentication.LogIn
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "LogIn"
    /// </summary>
    [DataContract]
    public class LogInRequest
    {

        /// <summary>
        /// Codice amministrazione
        /// </summary>
        [DataMember]
        public string CodeAdm
        {
            get;
            set;
        }

        /// <summary>
        /// Nome utente per l'autenticazione
        /// </summary>
        [DataMember]
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Password per l'autenticazione
        /// </summary>
        [DataMember]
        public string Password
        {
            get;
            set;
        }

    }
}