using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.ClassificationScheme.GetAllClassificationSchemes
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetAllClassificationSchemesResponse"
    /// </summary>
   [DataContract]
    public class GetAllClassificationSchemesResponse : Response
    {
       /// <summary>
       /// Titolari
       /// </summary>
        [DataMember]
        public Domain.ClassificationScheme[] ClassificationSchemes
        {
            get;
            set;
        }
    }
}