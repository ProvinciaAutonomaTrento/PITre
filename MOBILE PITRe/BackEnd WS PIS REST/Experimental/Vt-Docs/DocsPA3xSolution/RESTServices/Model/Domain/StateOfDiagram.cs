using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{
    public class StateOfDiagram
    {
        /// <summary>
        /// Id dello stato del diagramma
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Id del diagramma dello stato
        /// </summary>
        public string DiagramId
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione dello stato del diagramma
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Con valore true se è uno stato iniziale
        /// </summary>
        public bool InitialState
        {
            get;
            set;
        }

        /// <summary>
        /// Con valore true se è uno stato finale
        /// </summary>
        public bool FinaleState
        {
            get;
            set;
        }

    }
}