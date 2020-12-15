using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetDocument
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetDocumentResponse"
    /// </summary>
   [DataContract]
    public class GetDocumentResponse : Response
    {
        /// <summary>
        /// Dettaglio del documento
        /// </summary>
         [DataMember]
        public Domain.Document Document
        {
            get;
            set;
        }
    }
}