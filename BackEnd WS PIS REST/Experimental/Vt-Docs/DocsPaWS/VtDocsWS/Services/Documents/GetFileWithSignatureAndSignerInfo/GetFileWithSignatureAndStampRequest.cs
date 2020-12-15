using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetFileWithSignatureAndSignerInfo
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GGetFileWithSignatureOrStampRequest"
    /// </summary>
    [DataContract]
    public class GetFileWithSignatureAndSignerInfoRequest : Request
    {
        /// <summary>
        /// Id del documento
        /// </summary>
        [DataMember]
        public string IdDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Segnatura del documento
        /// </summary>
        [DataMember]
        public string Signature
        {
            get;
            set;
        }

        /// <summary>
        /// Se true aggiunge la segnatura al file
        /// </summary>
        [DataMember]
        public bool WithSignature
        {
            get;
            set;
        }

        /// <summary>
        /// Posizione della Segnatura
        /// </summary>
        [DataMember]
        public string SignaturePosition
        {
            get;
            set;
        }

        /// <summary>
        /// Font della Segnatura
        /// </summary>
        [DataMember]
        public string SignatureFont
        {
            get;
            set;
        }

        /// <summary>
        /// Colore della Segnatura
        /// </summary>
        [DataMember]
        public string SignatureColor
        {
            get;
            set;
        }

        /// <summary>
        /// Segnatura Verticale (disattivata per default)
        /// </summary>
        [DataMember]
        public bool SignatureVertical
        {
            get;
            set;
        }

        /// <summary>
        /// Se true aggiunge i dettagli di firma al file
        /// </summary>
        [DataMember]
        public bool WithSignerInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Se true Mette i dati di firma sull'ultima pagina
        /// </summary>
        [DataMember]
        public bool SignerInfoOnLastPage
        {
            get;
            set;
        }

    }
}