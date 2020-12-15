using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.GetTemplatePrj
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetTemplatePrjRequest"
    /// </summary>
   [DataContract]
    public class GetTemplatePrjRequest : Request
    {
        /// <summary>
        /// Descrizione della tipologia del fascicolo
        /// </summary>
         [DataMember]
        public string DescriptionTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Id della tipologia di fascicolo
        /// </summary>
         [DataMember]
        public string IdTemplate
        {
            get;
            set;
        }
    }
}