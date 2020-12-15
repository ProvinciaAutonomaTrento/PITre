using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{
    public class ClassificationScheme
    {
        /// <summary>
        /// System id del titolario
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del titolario
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il titolario è attivo
        /// </summary>
        public bool Active
        {
            get;
            set;
        }
    }
}