using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.GetProject
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetProjectResponse"
    /// </summary>
   [DataContract]
    public class GetProjectResponse : Response
    {
        /// <summary>
        /// Fascicolo richiesto
        /// </summary>
         [DataMember]
        public Domain.Project Project
        {
            get;
            set;
        }
    }
}