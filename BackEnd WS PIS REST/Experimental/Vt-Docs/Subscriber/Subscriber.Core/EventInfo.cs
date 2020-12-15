using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Subscriber
{
    /// <summary>
    /// Dettagli dell'evento da cui viene generata la pubblicazione del relativo oggetto
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publishing")]
    public class EventInfo
    {
        /// <summary>
        /// Nome dell'evento verificatosi nel sistema esterno
        /// </summary>
        public string EventName
        {
            get;
            set;
        }

        /// <summary>
        /// Data in cui l'evento si è verificato
        /// </summary>
        public DateTime EventDate
        {
            get;
            set;
        }

        /// <summary>
        /// Informazioni relative all'autore dell'evento
        /// </summary>
        public EventAuthorInfo Author
        {
            get;
            set;
        }

        /// <summary>
        /// Oggetto pubblicato dal sistema esterno
        /// </summary>
        public PublishedObject PublishedObject
        {
            get;
            set;
        }
    }
}
