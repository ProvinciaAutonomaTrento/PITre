using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// Modello di trasmissione
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class TransmissionModel
    {
        /// <summary>
        /// System id del modello
        /// </summary>
        [DataMember]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del modello
        /// </summary>
        [DataMember]
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo del modello (D documenti/ F fascicoli)
        /// </summary>
        [DataMember]
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del modello
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

    }
}