using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// Stato del diagramma
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class StateOfDiagram
    {
        /// <summary>
        /// Id dello stato del diagramma
        /// </summary>
        [DataMember]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Id del diagramma dello stato
        /// </summary>
        [DataMember]
        public string DiagramId
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione dello stato del diagramma
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Con valore true se è uno stato iniziale
        /// </summary>
        [DataMember]
        public bool InitialState
        {
            get;
            set;
        }

        /// <summary>
        /// Con valore true se è uno stato finale
        /// </summary>
        [DataMember]
        public bool FinaleState
        {
            get;
            set;
        }

    }
}