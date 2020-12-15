using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{
    public class Role
    {
        /// <summary>
        /// System id del ruolo (id gruppo no id corr globali)
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del ruolo
        /// </summary>
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del ruolo
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Registri associati al ruolo
        /// </summary>
        public Register[] Registers
        {
            get;
            set;
        }
    }
}