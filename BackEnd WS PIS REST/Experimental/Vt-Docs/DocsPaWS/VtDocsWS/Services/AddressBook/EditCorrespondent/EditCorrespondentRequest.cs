using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.AddressBook.EditCorrespondent
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta del servizio di "EditCorrespondentRequest"
    /// </summary>
    [DataContract]
    public class EditCorrespondentRequest : Request
    {
        /// <summary>
        /// Corrispondente da modificare
        /// </summary>
        [DataMember]
        public Domain.Correspondent Correspondent
        {
            get;
            set;
        }
    }
}