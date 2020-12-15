using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.AddressBook.GetOpportunity
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetOpportunityResponse"
    /// </summary>
    [DataContract]
    public class GetOpportunityResponse : Response
    {
        /// <summary>
        /// Corrispondente cercato
        /// </summary>
        [DataMember]
        public string[] IdOpportunityList
        {
            get;
            set;
        }
    }
}