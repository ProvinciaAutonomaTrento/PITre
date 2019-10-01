using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.AddressBook.AddCorrespondent
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta del servizio di "AddCorrespondentRequest"
    /// </summary>
    [DataContract]
    public class AddCorrespondentRequest : Request
    {
        /// <summary>
        /// Corrispondente che deve essere creato
        /// </summary>
        [DataMember]
        public Domain.Correspondent Correspondent
        {
            get;
            set;
        }
    }
}