using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class ExecuteTransmissionDocumentRequest
    {
        /// <summary>
        /// Id del documento da trasmettere
        /// </summary>
        public string IdDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Ragione di trasmissione
        /// </summary>
        public string TransmissionReason
        {
            get;
            set;
        }

        /// <summary>
        /// Destinatario
        /// </summary>
        public Domain.Correspondent Receiver
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se la trasmissione viene inserita nella todoList.
        /// </summary>
        public bool Notify
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del registro
        /// </summary>
        public string CodeReg
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo Trasmissione. S= Uno, T=tutti
        /// </summary>
        public string TransmissionType
        {
            get;
            set;
        }
    }
}