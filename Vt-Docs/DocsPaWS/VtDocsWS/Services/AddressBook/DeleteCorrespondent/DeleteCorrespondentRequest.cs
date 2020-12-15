using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.AddressBook.DeleteCorrespondent
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta del servizio di "DeleteCorrespondentRequest"
    /// </summary>
    [DataContract]
    public class DeleteCorrespondentRequest : Request
    {
        [DataMember]
        public string CorrespondentId
        {
            get;
            set;
        }
    }
}