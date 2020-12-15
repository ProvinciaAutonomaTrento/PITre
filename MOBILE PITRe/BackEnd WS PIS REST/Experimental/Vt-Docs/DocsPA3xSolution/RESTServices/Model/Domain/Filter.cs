using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{

    public enum FilterTypeEnum
    {
        String,
        Bool,
        Number,
        Date
    }

    /// <summary>
    /// Filtro di ricerca
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// Nome del filtro
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Valore del filtro
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Nel caso di template popolare il template con i campi di interesse
        /// </summary>
        public Template Template
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del filtro
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo filtro
        /// </summary>
        public FilterTypeEnum Type
        {
            get;
            set;
        }
    }

}
