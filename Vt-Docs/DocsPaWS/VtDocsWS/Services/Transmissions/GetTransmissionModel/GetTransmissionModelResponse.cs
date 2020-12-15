using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Transmissions.GetTransmissionModel
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetTransmissionModelResponse"
    /// </summary>
    [DataContract]
    public class GetTransmissionModelResponse : Response
    {
        /// <summary>
        /// Modello di trasmissione cercato
        /// </summary>
         [DataMember]
        public Domain.TransmissionModel TransmissionModel
        {
            get;
            set;
        }
    }
}