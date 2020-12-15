using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.GetTemplatePrj
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetTemplatePrjResponse"
    /// </summary>
   [DataContract]
    public class GetTemplatePrjResponse : Response
    {
        /// <summary>
        /// Dettaglio del template richiesto
        /// </summary>
         [DataMember]
        public Domain.Template Template
        {
            get;
            set;
        }
    }
}