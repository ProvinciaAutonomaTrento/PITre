using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Oggetto request relativo al servizio di creazione di un allegato ad un documento
    /// </summary>
    [Serializable()]
    public class CreateAllegatoRequest : Request
    {
        /// <summary>
        /// Oggetto allegato da inserire
        /// </summary>
        public DocsPaVO.documento.Allegato Allegato
        {
            get;
            set;
        }
    }
}
