using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.GetTemplatesProjects
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetTemplatesProjectsResponse"
    /// </summary>
    [DataContract]
    public class GetTemplatesProjectsResponse : Response
    {
         [DataMember]
        public VtDocsWS.Domain.Template[] Templates
        {
            get;
            set;
        }
    }
}