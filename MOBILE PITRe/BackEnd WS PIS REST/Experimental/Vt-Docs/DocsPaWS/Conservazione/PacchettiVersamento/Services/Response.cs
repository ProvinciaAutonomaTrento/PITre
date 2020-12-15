using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services
{
    /// <summary>
    /// Classe per l'incapsulamento dei pacchetti di risposta di tutti i BusinessServices
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/Response")]
    public class Response
    {
        /// <summary>
        /// Indica l'esito dell'invocazione al servizio
        /// </summary>
        public bool Success
        {
            get;
            set;
        }

        /// <summary>
        /// Eventuale errore riscontrato nell'invocazione al servizio 
        /// </summary>
        /// <remarks>
        /// Valorizzato solo se Success è false
        /// </remarks>
        public ResponseError Error
        {
            get;
            set;
        }
    }
}