using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Registers.GetRegistersOrRF
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetRegistersOrRFResponse"
    /// </summary>
   [DataContract]
    public class GetRegistersOrRFResponse : Response
    {
        /// <summary>
        /// Lista dei registri/rf cercati
        /// </summary>
         [DataMember]
        public Domain.Register[] Registers
        {
            get;
            set;
        }
    }
}