using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Oggetto restituito dal servizio di download del file
    /// </summary>
    [Serializable()]
    public class DownloadFileResponse : Response
    {
        /// <summary>
        /// 
        /// </summary>
        public DocsPaVO.documento.FileDocumento File
        {
            get;
            set;
        }
    }
}
