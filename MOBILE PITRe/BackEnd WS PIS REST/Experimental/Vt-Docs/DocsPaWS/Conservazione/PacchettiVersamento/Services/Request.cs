using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services
{
    /// <summary>
    /// Classe per l'incapsulamento dei pacchetti di richiesta per i servizi dei Pacchetti di Versamento
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/Request")]
    public class Request
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
        /// Token di autenticazione
        /// </summary>
        /// <remarks>
        /// Il token attesta che l'utente si è realmente autenticato al sistema per poter accedere ai servizi
        /// </remarks>
        public string AuthenticationToken
        {
            get;
            set;
        }
    }
}