using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetFileDocumentById
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetFileDocumentByIdResponse"
    /// </summary>
   [DataContract]
    public class GetFileDocumentByIdResponse : Response
    {
        /// <summary>
        /// File del documento richiesto
        /// </summary>
         [DataMember]
        public Domain.File File
        {
            get;
            set;
        }
    }
}