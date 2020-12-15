using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Transmissions.GetTransmissionModels
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetTransmissionModelResponse"
    /// </summary>
   [DataContract]
    public class GetTransmissionModelsResponse : Response
    {
        /// <summary>
        /// Modelli di trasmissione cercati
        /// </summary>
         [DataMember]
        public Domain.TransmissionModel[] TransmissionModels
        {
            get;
            set;
        }
    }
}