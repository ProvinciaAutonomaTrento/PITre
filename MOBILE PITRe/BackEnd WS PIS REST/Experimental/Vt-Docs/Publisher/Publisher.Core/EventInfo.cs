using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Publisher
{
    /// <summary>
    /// Rappresenta un oggetto monitorato nel sistema
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publisher")]
    public class EventInfo
    {
        /// <summary>
        /// Id dell'evento
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Id del canele di pubblicazione dell'evento
        /// </summary>
        public int IdChannel
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo dell'oggetto monitorato
        /// </summary>
        public string ObjectType
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del template dell'oggetto monitorato
        /// </summary>
        public string ObjectTemplateName
        {
            get;
            set;
        }

        /// <summary>
        /// Nome evento verificatosi nel sistem
        /// </summary>
        public string EventName
        {
            get;
            set;
        }

        /// <summary>
        /// Classe per la mappatura dei dati dell'oggetto
        /// </summary>
        public string DataMapperFullClass
        {
            get;
            set;
        }

        /// <summary>
        /// Se true, al verificarsi dell'evento legato al documento deve essere caricato il file
        /// </summary>
        public bool LoadFileIfDocumentType
        {
            get;
            set;
        }
    }
}