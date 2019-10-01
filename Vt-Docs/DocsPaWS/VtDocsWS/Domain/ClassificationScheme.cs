using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// Titolario
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class ClassificationScheme
    {
        /// <summary>
        /// System id del titolario
        /// </summary>
        [DataMember]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del titolario
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il titolario è attivo
        /// </summary>
        [DataMember]
        public bool Active
        {
            get;
            set;
        }

    }
}