using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.AddAttachment
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "UploadFileToDocumentRequest"
    /// </summary>
   [DataContract]
    public class AddAttachmentRequest : Request
    {

        /// <summary>
        /// DocNumber del documento a cui associare il documento o una nuova versione
        /// </summary>
        [DataMember]
        public string IdDocument
        {
            get;
            set;
        }

        /// <summary>
        /// File da acquisire
        /// </summary>
        [DataMember]
        public Domain.File File
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione in caso di nuovo allegato
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Se true converte il file in pdf/a
        /// </summary>
        [DataMember]
        public bool CovertToPDFA
        {
            get;
            set;
        }

        /// <summary>
        /// Hash del file passato nell'attributo Domain.File File
        /// </summary>
        [DataMember]
        public string HashFile
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo di Attachment Esterno=E ; Utente=U; se null o Empy  = U
        /// </summary>
        [DataMember]
        public string AttachmentType
        {
            get;
            set;
        }
    }
}