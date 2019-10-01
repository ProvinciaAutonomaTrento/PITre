using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.GetProjectFilters
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetProjectFiltersResponse"
    /// </summary>
    [DataContract]
    public class GetProjectFiltersResponse : Response
    {
        /// <summary>
        /// Lista dei filtri applicabili
        /// </summary>
        [DataMember]
        public Domain.Filter[] Filters
        {
            get;
            set;
        }
    }
}