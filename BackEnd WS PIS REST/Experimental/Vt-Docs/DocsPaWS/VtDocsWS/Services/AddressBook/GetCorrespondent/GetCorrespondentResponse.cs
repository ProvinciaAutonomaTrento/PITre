using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.AddressBook.GetCorrespondent
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetCorrespondentResponse"
    /// </summary>
    [DataContract]
    public class GetCorrespondentResponse : Response
    {
        /// <summary>
        /// Corrispondente cercato
        /// </summary>
        [DataMember]
        public Domain.Correspondent Correspondent
        {
            get;
            set;
        }
    }
}