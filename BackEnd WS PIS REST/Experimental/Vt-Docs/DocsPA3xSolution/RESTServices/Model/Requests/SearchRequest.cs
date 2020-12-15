using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class SearchRequest
    {
        /// <summary>
        /// Filtri di ricerca
        /// </summary>
        public Domain.Filter[] Filters
        {
            get;
            set;
        }

        /// <summary>
        /// Pagina che si desidera visualizzare
        /// </summary>
        public int PageNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Quanti elementi sono presenti nella pagina se la ricerca è paginata
        /// </summary>
        public int ElementsInPage
        {
            get;
            set;
        }
                
    }
}