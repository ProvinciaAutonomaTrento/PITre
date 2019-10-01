using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class UploadFileRequest : Request
    {
        /// <summary>
        /// 
        /// </summary>
        public DocsPaVO.documento.FileRequest FileRequest
        {
            get;
            set;
        }

        ///// <summary>
        ///// Identificativo del documento su cui effettuare l'upload del file
        ///// </summary>
        //public string IdProfile
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        ///// Identificativo della versione specifica del documento su cui effettuare l'upload del file
        ///// </summary>
        ///// <remarks>
        ///// Se null, specifica di effettuare contestualmente l'inserimento di una nuova versione del documento
        ///// </remarks>
        //public string VersionId
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Note della versione
        ///// </summary>
        //public string VersionDescription
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Nome del file da inviare
        ///// </summary>
        //public string FileName
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Contenuto binario del file
        /// </summary>
        public byte[] Content
        {
            get;
            set;
        }

        /// <summary>
        /// MimeType del file
        /// </summary>
        public string ContentType
        {
            get;
            set;
        }

        ///// <summary>
        ///// Determina se per il file inviato esiste un corrispondente cartaceo
        ///// </summary>
        //public bool IsCartaceo
        //{
        //    get;
        //    set;
        //}
    }
}
