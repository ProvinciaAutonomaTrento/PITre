using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Subscriber
{
    /// <summary>
    /// Dettagli di un errore che si è verificato nel Subscriber
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publishing")]
    public class ErrorInfo
    {
        /// <summary>
        /// Identificativo univoco dell'errore
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione dell'errore
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Origine dell'errore
        /// </summary>
        public string Stack
        {
            get;
            set;
        }
    }
}
