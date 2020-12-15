using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetTemplatesDocuments
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetTemplatesDocumentsResponse"
    /// </summary>
   [DataContract]
    public class GetTemplatesDocumentsResponse : Response
    {
        /// <summary>
        /// Tutti i template dei documenti
        /// </summary>
         [DataMember]
        public Domain.Template[] Templates
        {
            get;
            set;
        }
    }
}