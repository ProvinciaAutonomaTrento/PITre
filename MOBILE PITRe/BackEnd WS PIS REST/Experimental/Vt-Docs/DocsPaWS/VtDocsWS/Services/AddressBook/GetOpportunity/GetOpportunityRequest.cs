using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.AddressBook.GetOpportunity
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta del servizio di "GetOpportunityRequest"
    /// </summary>
   [DataContract]
    public class GetOpportunityRequest : Request
    {
        /// <summary>
        /// Codice Utente
        /// </summary>
        [DataMember]
        public string CodiceUtente
        {
            get;
            set;
        }
    }
}