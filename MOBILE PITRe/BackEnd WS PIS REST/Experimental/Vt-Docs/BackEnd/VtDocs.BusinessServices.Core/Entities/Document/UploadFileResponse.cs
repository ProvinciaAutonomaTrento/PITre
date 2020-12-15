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
    public class UploadFileResponse : Response
    {
        /// <summary>
        /// Indica se per l'upload del file è stato necessario aggiungere una nuova versione al documento
        /// </summary>
        public bool IsNewVersion
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DocsPaVO.documento.FileRequest FileRequest
        {
            get;
            set;
        }
    }
}
