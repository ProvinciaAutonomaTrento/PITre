using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Oggetto request per il servizio SaveVersion
    /// </summary>
    [Serializable()]
    public class SaveVersioneRequest : Request
    {
        /// <summary>
        /// Versione da salvare
        /// </summary>
        public DocsPaVO.documento.FileRequest FileRequest
        {
            get;
            set;
        }
    }
}
