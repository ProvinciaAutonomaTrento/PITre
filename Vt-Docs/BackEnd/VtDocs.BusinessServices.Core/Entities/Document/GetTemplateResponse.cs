using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Oggetto request relativo al servizio di reperimento di un template documento
    /// </summary>
    [Serializable()]
    public class GetTemplateResponse : Response
    {
        /// <summary>
        /// Template richiesto
        /// </summary>
        public DocsPaVO.ProfilazioneDinamica.Templates Template
        {
            get;
            set;
        }
    }
}
