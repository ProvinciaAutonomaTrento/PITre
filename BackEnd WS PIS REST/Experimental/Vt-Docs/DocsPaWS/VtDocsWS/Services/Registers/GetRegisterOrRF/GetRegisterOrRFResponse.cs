using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Registers.GetRegisterOrRF
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetRegisterOrRFResponse"
    /// </summary>
   [DataContract]
    public class GetRegisterOrRFResponse : Response
    {
        /// <summary>
        /// Dettaglio del registro
        /// </summary>
         [DataMember]
        public Domain.Register Register
        {
            get;
            set;
        }
    }
}