using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.AddressBook.GetCorrespondent
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta del servizio di "GetCorrespondentRequest"
    /// </summary>
   [DataContract]
    public class GetCorrespondentRequest : Request
    {
        /// <summary>
        /// Id del corrispondente
        /// </summary>
        [DataMember]
        public string IdCorrespondent
        {
            get;
            set;
        }
    }
}