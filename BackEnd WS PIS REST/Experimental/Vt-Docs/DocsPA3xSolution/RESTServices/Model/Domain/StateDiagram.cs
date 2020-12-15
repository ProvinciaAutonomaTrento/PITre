using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{
    public class StateDiagram
    {
        /// <summary>
        /// System id del diagramma
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del diagramma
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Stati del diagramma
        /// </summary>
        public StateOfDiagram[] StateOfDiagram
        {
            get;
            set;
        }
    }
}