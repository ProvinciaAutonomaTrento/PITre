using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.GetProjectStateDiagram
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetProjectStateDiagramResponse"
    /// </summary>
  [DataContract]
    public class GetProjectStateDiagramResponse : Response
    {
        /// <summary>
        /// Stato del diagramma del fascicolo
        /// </summary>
         [DataMember]
        public Domain.StateOfDiagram StateOfDiagram
        {
            get;
            set;
        }
    }
}