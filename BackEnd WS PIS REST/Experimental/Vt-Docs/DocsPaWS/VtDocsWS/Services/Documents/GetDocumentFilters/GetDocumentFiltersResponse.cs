using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VtDocsWS.Services;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetDocumentFilters
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetDocumentFiltersResponse"
    /// </summary>
    [DataContract]
    public class GetDocumentFiltersResponse : Response
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