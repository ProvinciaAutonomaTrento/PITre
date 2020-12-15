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
    public class SaveVersioneResponse : Response
    {
        /// <summary>
        /// Versione salvata
        /// </summary>
        public DocsPaVO.documento.FileRequest FileRequest
        {
            get;
            set;
        }
    }
}
