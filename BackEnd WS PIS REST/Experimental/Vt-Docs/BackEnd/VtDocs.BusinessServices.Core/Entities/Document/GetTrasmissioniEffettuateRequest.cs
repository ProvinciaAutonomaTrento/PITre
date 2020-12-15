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
    public class GetTrasmissioniEffettuateRequest : Request
    {
        /// <summary>
        /// Contesto di paginazione
        /// </summary>
        public PagingContext PagingContext
        {
            get;
            set;
        }

        /// <summary>
        /// Id documento da cui reperire le trasmissioni effettuate
        /// </summary>
        public string IdDocumento
        {
            get;
            set;
        }
    }
}
