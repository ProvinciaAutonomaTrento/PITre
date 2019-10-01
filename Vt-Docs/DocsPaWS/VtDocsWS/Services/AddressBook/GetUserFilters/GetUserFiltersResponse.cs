using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.AddressBook.GetUserFilters
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetUserFiltersResponse"
    /// </summary>
    [DataContract]
    public class GetUserFiltersResponse : Response
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