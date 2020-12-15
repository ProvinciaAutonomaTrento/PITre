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
    public class SaveDocumentoResponse : Response
    {
        /// <summary>
        /// 
        /// </summary>
        public DocsPaVO.documento.SchedaDocumento Documento
        {
            get;
            set;
        }

        /// <summary>
        /// Stato corrente del documento
        /// </summary>
        public DocsPaVO.DiagrammaStato.Stato StatoCorrente
        {
            get;
            set;
        }
    }
}
