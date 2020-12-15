using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.LogIn
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "LogIn"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/LogInResponse")]
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
        public string AuthenticationToken
        {
            get;
            set;
        }
    }
}