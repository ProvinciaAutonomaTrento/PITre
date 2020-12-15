using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.AddressBook.SearchCorrespondents
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta del servizio di "SearchCorrespondentsRequest"
    /// </summary>
   [DataContract]
    public class SearchCorrespondentsRequest : Request
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
    }
}