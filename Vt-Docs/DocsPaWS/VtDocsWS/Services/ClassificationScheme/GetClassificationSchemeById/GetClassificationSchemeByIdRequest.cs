using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.ClassificationScheme.GetClassificationSchemeById
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetClassificationSchemeByIdRequest"
    /// </summary>
   [DataContract]
    public class GetClassificationSchemeByIdRequest : Request
    {
       /// <summary>
       /// Id titolario
       /// </summary>
        [DataMember]
        public string IdClassificationScheme
        {
            get;
            set;
        }
    }
}