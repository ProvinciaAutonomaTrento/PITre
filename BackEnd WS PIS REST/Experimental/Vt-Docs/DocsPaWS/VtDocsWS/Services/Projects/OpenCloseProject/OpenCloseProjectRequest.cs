using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.OpenCloseProject
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "OpenCloseProjectRequest"
    /// </summary>
    [DataContract]
    public class OpenCloseProjectRequest : Request
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
        /// O per Open, C per Close
        /// </summary>
         [DataMember]
        public string Action
        {
            get;
            set;
        }
    }
}