using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class SearchFiltersOnlyRequest
    {
        /// <summary>
        /// Filtri di ricerca
        /// </summary>
        public Domain.Filter[] Filters
        {
            get;
            set;
        }
    }
}