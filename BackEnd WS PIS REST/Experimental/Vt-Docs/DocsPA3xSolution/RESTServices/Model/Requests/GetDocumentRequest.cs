using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class GetDocumentRequest
    {
        /// <summary>
        /// Id del documento
        /// </summary>
        public string IdDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Segnatura del documento
        /// </summary>
        public string Signature
        {
            get;
            set;
        }

        /// <summary>
        /// Se true oltre ai metadati restituisce anche il contenuto del file principale e allegati
        /// </summary>
        public bool GetFile
        {
            get;
            set;
        }

        /// <summary>
        /// Se pari a uno, restituisce il file con l'estensione della firma (p7m)
        /// </summary>
        public string GetFileWithSignature
        {
            get;
            set;
        }

    }
}