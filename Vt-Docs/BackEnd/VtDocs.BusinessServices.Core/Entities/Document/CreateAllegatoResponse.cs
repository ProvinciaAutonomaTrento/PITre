using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Oggetto response del servizio di creazione dell'allegato di un documento
    /// </summary>
    [Serializable()]
    public class CreateAllegatoResponse : Response
    {
        /// <summary>
        /// Allegato creato
        /// </summary>
        public DocsPaVO.documento.Allegato Allegato
        {
            get;
            set;
        }
    }
}
