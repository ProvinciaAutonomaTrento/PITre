using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services
{
    /// <summary>
    /// Classe per l'incapsulamento degli eventuali errori riscontrati
    /// nell'esecuzione del servizio
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/ResponseError")]
    public class ResponseError
    {
        /// <summary>
        /// Codice dell'errore
        /// </summary>
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione dell'errore
        /// </summary>
        public string Description
        {
            get;
            set;
        }
    }
}