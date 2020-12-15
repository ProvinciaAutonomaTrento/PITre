using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.AddressBook.AddCorrespondent
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "AddCorrespondentResponse"
    /// </summary>
    [DataContract]
    public class AddCorrespondentResponse : Response
    {
        /// <summary>
        /// Corrispondente creato
        /// </summary>
         [DataMember]
        public Domain.Correspondent Correspondent
        {
            get;
            set;
        }
    }
}