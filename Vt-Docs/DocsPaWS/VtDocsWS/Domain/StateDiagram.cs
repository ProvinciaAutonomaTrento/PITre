using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// Diagramma di stato
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class StateDiagram
    {
        /// <summary>
        /// System id del diagramma
        /// </summary>
        [DataMember]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del diagramma
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Stati del diagramma
        /// </summary>
        [DataMember]
        public StateOfDiagram[] StateOfDiagram
        {
            get;
            set;
        }
    }
}