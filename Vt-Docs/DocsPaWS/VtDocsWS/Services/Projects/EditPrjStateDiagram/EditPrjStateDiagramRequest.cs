using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.EditPrjStateDiagram
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "EditPrjStateDiagramRequest"
    /// </summary>
   [DataContract]
    public class EditPrjStateDiagramRequest : Request
    {
        /// <summary>
        /// Id del fascicolo
        /// </summary>
         [DataMember]
        public string IdProject
        {
            get;
            set;
        }

        /// <summary>
        /// Id del titolario
        /// </summary>
         [DataMember]
        public string ClassificationSchemeId
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del fascicolo
        /// </summary>
         [DataMember]
        public string CodeProject
        {
            get;
            set;
        }

        /// <summary>
        /// Stato del diagramma che verrà salvato per il fascicolo
        /// </summary>
         [DataMember]
        public string StateOfDiagram
        {
            get;
            set;
        }
    }
}