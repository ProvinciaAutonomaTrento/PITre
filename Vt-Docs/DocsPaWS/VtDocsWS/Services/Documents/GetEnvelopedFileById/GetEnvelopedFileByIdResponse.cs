using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetEnvelopedFileById
{
    [DataContract]
    public class GetEnvelopedFileByIdResponse : Response
    {
        /// <summary>
        /// File del documento richiesto
        /// </summary>
        [DataMember]
        public Domain.File File
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del file originale, per capire se è un pdf, p7m o altro.
        /// </summary>
        [DataMember]
        public string OriginalFileName { get; set; }

        /// <summary>
        /// True se il documento è firmato
        /// </summary>
        [DataMember]
        public bool IsDocumentSigned { get; set; }

        /// <summary>
        /// Estensione del file
        /// </summary>
        [DataMember]
        public string FileExtension { get; set; }
    }
}