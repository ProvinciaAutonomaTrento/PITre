using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Registers.GetRegisterOrRF
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetRegisterOrRFRequest"
    /// </summary>
    [DataContract]
    public class GetRegisterOrRFRequest : Request
    {
        /// <summary>
        /// Codice del registro
        /// </summary>
         [DataMember]
        public string CodeRegister
        {
            get;
            set;
        }

        /// <summary>
        /// Id del registro
        /// </summary>
         [DataMember]
        public string IdRegister
        {
            get;
            set;
        }
    }
}