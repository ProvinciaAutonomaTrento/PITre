using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// Ruolo
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Role
    {
        /// <summary>
        /// System id del ruolo (id gruppo no id corr globali)
        /// </summary>
        [DataMember]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del ruolo
        /// </summary>
        [DataMember]
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del ruolo
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Registri associati al ruolo
        /// </summary>
        [DataMember]
        public Register[] Registers
        {
            get;
            set;
        }
    }
}