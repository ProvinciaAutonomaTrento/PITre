using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.AddressBook.GetCorrespondentFilters
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetCorrespondentFiltersResponse"
    /// </summary>
    [DataContract]
    public class GetCorrespondentFiltersResponse : Response
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