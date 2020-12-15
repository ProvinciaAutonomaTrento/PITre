using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// Registro
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Register
    {
        /// <summary>
        /// Id del registro
        /// </summary>
        [DataMember]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del registro
        /// </summary>
        [DataMember]
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Indica del il registro è un rf
        /// </summary>
        [DataMember]
        public bool IsRF
        {
            get;
            set;
        }

        /// <summary>
        /// Indica lo stato del registro chiuso/aperto/giallo
        /// </summary>
        [DataMember]
        public string State
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la descrizione del registro/rf
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }
    }
}