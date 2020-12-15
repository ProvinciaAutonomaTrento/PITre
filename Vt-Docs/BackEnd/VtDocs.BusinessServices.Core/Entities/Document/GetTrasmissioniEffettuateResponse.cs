using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    ///  Oggetto response relativo al servizio di reperimento delle trasmissioni effettuate per un documento
    /// </summary>
    [Serializable()]
    public class GetTrasmissioniEffettuateResponse : Response
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
        /// Lista delle trasmissioni effettuate
        /// </summary>
        public DocsPaVO.trasmissione.InfoTrasmissione[] Trasmissioni
        {
            get;
            set;
        }
    }
}
