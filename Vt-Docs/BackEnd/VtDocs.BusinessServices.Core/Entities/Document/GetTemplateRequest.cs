using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    ///  Oggetto request relativo al servizio di reperimento delle trasmissioni effettuate per un documento
    /// </summary>
    [Serializable()]
    public class GetTemplateRequest : Request
    {
        /// <summary>
        /// Identificativo univoco del template richiesto
        /// </summary>
        public string IdTemplate
        {
            get;
            set;
        }
    }
}
