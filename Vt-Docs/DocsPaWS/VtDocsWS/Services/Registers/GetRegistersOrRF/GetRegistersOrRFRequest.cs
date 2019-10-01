using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Registers.GetRegistersOrRF
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetRegistersOrRFRequest"
    /// </summary>
   [DataContract]
    public class GetRegistersOrRFRequest : Request
    {
        /// <summary>
        /// Codice del ruolo
        /// </summary>
         [DataMember]
        public string CodeRole
        {
            get;
            set;
        }

        /// <summary>
        /// Id del ruolo
        /// </summary>
         [DataMember]
        public string IdRole
        {
            get;
            set;
        }

        /// <summary>
        /// Con il valore true prende solo i registri
        /// </summary>
         [DataMember]
        public bool OnlyRegisters
        {
            get;
            set;
        }

        /// <summary>
        /// Con il valore true prende solo gli rf
        /// </summary>
         [DataMember]
        public bool OnlyRF
        {
            get;
            set;
        }
    }
}