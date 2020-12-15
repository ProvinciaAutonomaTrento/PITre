using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.AddressBook.SearchCorrespondents
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "SearchCorrespondentsResponse"
    /// </summary>
   [DataContract]
    public class SearchCorrespondentsResponse : Response
    {
       /// <summary>
       /// Corrispondenti cercati
       /// </summary>
        [DataMember]
        public VtDocsWS.Domain.Correspondent[] Correspondents
        {
            get;
            set;
        }
    }
}