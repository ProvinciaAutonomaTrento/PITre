using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.LogIn
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "LogIn"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/LogInRequest")]
    public class LogInRequest
    {
        /// <summary>
        /// Nome utente per l'autenticazione
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Password per l'autenticazione
        /// </summary>
        public string Password
        {
            get;
            set;
        }
    }
}