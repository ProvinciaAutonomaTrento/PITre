using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.EditProject
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "EditProjectResponse"
    /// </summary>
   [DataContract]
    public class EditProjectResponse : Response
    {
       /// <summary>
       /// Fascicolo modificato
       /// </summary>
         [DataMember]
        public VtDocsWS.Domain.Project Project
        {
            get;
            set;
        }
    }
}