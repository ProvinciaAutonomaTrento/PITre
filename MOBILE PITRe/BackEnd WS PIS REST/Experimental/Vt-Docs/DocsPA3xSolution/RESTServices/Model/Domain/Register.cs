using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{
    public class Register
    {
        /// <summary>
        /// Id del registro
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del registro
        /// </summary>
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Indica del il registro è un rf
        /// </summary>
        public bool IsRF
        {
            get;
            set;
        }

        /// <summary>
        /// Indica lo stato del registro chiuso/aperto/giallo
        /// </summary>
        public string State
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la descrizione del registro/rf
        /// </summary>
        public string Description
        {
            get;
            set;
        }
    }
}