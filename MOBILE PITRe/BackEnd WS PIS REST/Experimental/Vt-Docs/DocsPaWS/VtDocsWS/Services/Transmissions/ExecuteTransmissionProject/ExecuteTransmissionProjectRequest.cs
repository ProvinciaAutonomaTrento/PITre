using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Transmissions.ExecuteTransmissionProject
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "ExecuteTransmissionProject"
    /// </summary>
    [DataContract]
    public class ExecuteTransmissionProjectRequest : Request
    {
        /// <summary>
        /// Id del fascicolo da trasmettere
        /// </summary>
        [DataMember]
        public string IdProject
        {
            get;
            set;
        }

        /// <summary>
        /// Ragione di trasmissione
        /// </summary>
        [DataMember]
        public string TransmissionReason
        {
            get;
            set;
        }

        /// <summary>
        /// destinatario
        /// </summary>
        [DataMember]
        public Domain.Correspondent Receiver
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se la trasmissione viene inserita nella todoList.
        /// </summary>
        [DataMember]
        public bool Notify
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del registro
        /// </summary>
        [DataMember]
        public string CodeReg
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo Trasmissione. S= Uno, T=tutti
        /// </summary>
        [DataMember]
        public string TransmissionType
        {
            get;
            set;
        }
    }
}