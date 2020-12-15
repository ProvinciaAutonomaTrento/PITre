using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetFileDocumentById
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetFileDocumentByIdRequest"
    /// </summary>
  [DataContract]
    public class GetFileDocumentByIdRequest : Request
    {
        /// <summary>
        /// DocNumber del documento
        /// </summary>
         [DataMember]
        public string IdDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Id della versione, opzionale, se vuoto prende l'ultima versione
        /// </summary>
         [DataMember]
        public string VersionId
        {
            get;
            set;
        }
    }
}