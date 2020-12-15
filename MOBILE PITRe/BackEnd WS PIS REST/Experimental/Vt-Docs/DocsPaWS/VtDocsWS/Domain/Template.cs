using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// Tipologia documento o fascicolo
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Template
    {
        /// <summary>
        /// System id del template
        /// </summary>
        [DataMember]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del template
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Campi del template
        /// </summary>
        [DataMember]
        public Domain.Field[] Fields
        {
            get;
            set;
        }

        /// <summary>
        /// Diagramma di stato associato
        /// </summary>
        [DataMember]
        public Domain.StateDiagram StateDiagram
        {
            get;
            set;
        }

        /// <summary>
        /// D per documenti F per fascicoli
        /// </summary>
        [DataMember]
        public string Type
        {
            get;
            set;
        }
    }
}