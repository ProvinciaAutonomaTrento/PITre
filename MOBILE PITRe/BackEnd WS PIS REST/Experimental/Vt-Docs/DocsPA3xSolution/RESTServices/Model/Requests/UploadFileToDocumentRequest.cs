using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class UploadFileToDocumentRequest
    {
        /// <summary>
        /// DocNumber del documento a cui associare il documento o una nuova versione
        /// </summary>
        public string IdDocument
        {
            get;
            set;
        }

        /// <summary>
        /// File da acquisire
        /// </summary>
        public Domain.File File
        {
            get;
            set;
        }

        /// <summary>
        /// Se true indica la creazione di un nuovo allegato per il documento
        /// </summary>
        public bool CreateAttachment
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione in caso di nuovo allegato
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Se true converte il file in pdf/a
        /// </summary>
        public bool CovertToPDFA
        {
            get;
            set;
        }

        /// <summary>
        /// Hash del file passato nell'attributo Domain.File File
        /// </summary>
        public string HashFile
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo di Attachment Esterno=E ; Utente=U; se null o Empy  = U
        /// </summary>
        public string AttachmentType
        {
            get;
            set;
        }

    }
}