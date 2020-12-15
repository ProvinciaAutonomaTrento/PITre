using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.SearchProjects
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "SearchProjectsResponse"
    /// </summary>
    [DataContract]
    public class SearchProjectsResponse : Response
    {
        /// <summary>
        /// Fascicoli cercati
        /// </summary>
        [DataMember]
        public Domain.Project[] Projects
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale dei fascicoli trovati
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public int? TotalProjectsNumber
        {
            get;
            set;
        }
    }
}