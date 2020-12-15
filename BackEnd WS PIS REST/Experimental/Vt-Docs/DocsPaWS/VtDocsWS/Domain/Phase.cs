using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// Fase diagramma di stato
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Phase
    {
        /// <summary>
        /// Descrizione della fase
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se la fase è selezionata
        /// </summary>
        [DataMember]
        public bool Selected
        {
            get;
            set;
        }
    }
}