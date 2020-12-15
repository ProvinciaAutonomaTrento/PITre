using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.AddressBook.SearchUsers
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "SearchUsers"
    /// </summary>
   [DataContract]
    public class SearchUsersRequest : Request
    {
       /// <summary>
       /// Filtri di ricerca
       /// </summary>
        [DataMember]
        public VtDocsWS.Domain.Filter[] Filters
        {
            get;
            set;
        }
    }
}