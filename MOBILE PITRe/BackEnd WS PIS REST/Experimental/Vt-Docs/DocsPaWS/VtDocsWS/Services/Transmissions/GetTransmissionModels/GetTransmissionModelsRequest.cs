using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Transmissions.GetTransmissionModels
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetTransmissionModelsRequest"
    /// </summary>
  [DataContract]
    public class GetTransmissionModelsRequest : Request
    {
        /// <summary>
        /// Tipo D per documenti e F per fascicoli
        /// </summary>
         [DataMember]
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Registri sui quali sono disponibili i modelli
        /// </summary>
         [DataMember]
        public Domain.Register[] Registers
        {
            get;
            set;
        }
    }
}