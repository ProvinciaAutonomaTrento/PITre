using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.SearchProjects
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "SearchProjectsRequest"
    /// </summary>
    [DataContract]
    public class SearchProjectsRequest : Request
    {
        /// <summary>
        /// Filtri di ricerca
        /// </summary>
        [DataMember]
        public Domain.Filter[] Filters
        {
            get;
            set;
        }

        /// <summary>
        /// Pagina che si desidera visualizzare
        /// </summary>
        [DataMember]
        public int? PageNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Quanti elementi sono presenti nella pagina se la ricerca è paginata
        /// </summary>
        [DataMember]
        public int? ElementsInPage
        {
            get;
            set;
        }

        /// <summary>
        /// Se true la relativa response conterrà il numero totale di fascicoli ottenuti nella ricerca
        /// </summary>
        [DataMember]
        public bool GetTotalProjectsNumber
        {
            get;
            set;
        }
    }
}