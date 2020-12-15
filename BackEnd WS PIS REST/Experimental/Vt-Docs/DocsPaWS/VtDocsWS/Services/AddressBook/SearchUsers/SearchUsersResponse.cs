using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.AddressBook.SearchUsers
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "SearchUsers"
    /// </summary>
    [DataContract]
    public class SearchUsersResponse : Response
    {
        /// <summary>
        /// Utenti cercati
        /// </summary>
         [DataMember]
        public VtDocsWS.Domain.User[] Users
        {
            get;
            set;
        }
    }
}