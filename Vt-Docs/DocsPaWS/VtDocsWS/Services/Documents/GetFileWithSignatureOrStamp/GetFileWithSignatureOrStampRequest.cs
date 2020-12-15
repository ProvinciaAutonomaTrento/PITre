using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetFileWithSignatureOrStamp
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GGetFileWithSignatureOrStampRequest"
    /// </summary>
   [DataContract]
    public class GetFileWithSignatureOrStampRequest : Request
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
        /// Se true aggiunge il timbro al file
        /// </summary>
         [DataMember]
        public bool WithStamp
        {
            get;
            set;
        }
    }
}