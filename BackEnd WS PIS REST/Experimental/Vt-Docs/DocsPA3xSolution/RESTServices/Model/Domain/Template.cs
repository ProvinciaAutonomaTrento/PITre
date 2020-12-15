using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{
    public class Template
    {
        /// <summary>
        /// System id del template
        /// </summary>
       public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del template
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Campi del template
        /// </summary>
        public Domain.Field[] Fields
        {
            get;
            set;
        }

        /// <summary>
        /// Diagramma di stato associato
        /// </summary>
        public Domain.StateDiagram StateDiagram
        {
            get;
            set;
        }

        /// <summary>
        /// D per documenti F per fascicoli
        /// </summary>
        public string Type
        {
            get;
            set;
        }
    }
}