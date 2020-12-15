using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.DocumentsAdvanced.CaricaLottoInPi3
{
    [DataContract]
    public class CaricaLottoInPi3Request : Request
    {
        /// <summary>
        /// Nel caso di protocollo specificare il registro
        /// </summary>
        [DataMember]
        public string CodeRegister
        {
            get;
            set;
        }

        /// <summary>
        /// Documento che si vuole creare
        /// </summary>
        [DataMember]
        public Domain.Document Document
        {
            get;
            set;
        }

        /// <summary>
        /// Id Sdi del lotto di fatture
        /// </summary>
        [DataMember]
        public string IdentificativoSdI
        {
            get;
            set;
        }

        /// <summary>
        /// Codice dell'RF in cui si vuole protocollare (opzionale)
        /// </summary>
        [DataMember]
        public string CodeRF
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del fascicolo nel quale fascicolare il documento, il codice prende soltanto i fascicoli nei titolari attivi
        /// </summary>
        [DataMember]
        public string CodeProject
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
        /// Destinatario
        /// </summary>
        [DataMember]
        public Domain.Correspondent TransmissionReceiver
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se la trasmissione viene inserita nella todoList.
        /// </summary>
        [DataMember]
        public bool TransmissionNotify
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del registro
        /// </summary>
        [DataMember]
        public string TransmissionCodeReg
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