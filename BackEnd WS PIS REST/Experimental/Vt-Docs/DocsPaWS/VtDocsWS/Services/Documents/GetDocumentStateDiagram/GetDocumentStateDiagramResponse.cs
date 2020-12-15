using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetDocumentStateDiagram
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetDocumentsInProjectResponse"
    /// </summary>
   [DataContract]
    public class GetDocumentStateDiagramResponse : Response
    {
        /// <summary>
        /// Stato del diagramma del documento
        /// </summary>
         [DataMember]
        public Domain.StateOfDiagram StateOfDiagram
        {
            get;
            set;
        }
    }
}