using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.CreateDocumentAndAddInProject
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "CreateDocumentAndAddInProjectResponse"
    /// </summary>
    [DataContract]
    public class CreateDocumentAndAddInProjectResponse : Response
    {
        /// <summary>
        /// Dettaglio del documento creato
        /// </summary>
        [DataMember]
        public Domain.Document Document
        {
            get;
            set;
        }
    }
}