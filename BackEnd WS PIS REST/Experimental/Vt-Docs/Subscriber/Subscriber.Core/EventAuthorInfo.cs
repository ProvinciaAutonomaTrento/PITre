using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Subscriber
{
    /// <summary>
    /// Informazioni relative all'autore dell'evento
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publishing")]
    public class EventAuthorInfo
    {
        /// <summary>
        /// Id dell'autore della pubblicazione dell'oggetto
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Nome dell'autore della pubblicazione dell'oggetto
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Id del ruolo dell'autore della pubblicazione dell'oggetto
        /// </summary>
        public string IdRole
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del ruolo autore della pubblicazione dell'oggetto
        /// </summary>
        public string RoleName
        {
            get;
            set;
        }
    }
}
