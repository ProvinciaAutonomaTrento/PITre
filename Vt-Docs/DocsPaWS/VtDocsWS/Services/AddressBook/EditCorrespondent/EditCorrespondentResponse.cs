using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.AddressBook.EditCorrespondent
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "EditCorrespondentResponse"
    /// </summary>
    [DataContract]
    public class EditCorrespondentResponse : Response
    {
        /// <summary>
        /// Corrispondente modificato
        /// </summary>
        [DataMember]
        public Domain.Correspondent Correspondent
        {
            get;
            set;
        }
    }
}