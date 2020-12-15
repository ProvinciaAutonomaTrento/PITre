using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Oggetto response relativo al servizio di reperimento della todolist
    /// </summary>
    [Serializable()]
    public class GetMyToDoListResponse : Response
    {
        /// <summary>
        /// Contesto di paginazione di output
        /// </summary>
        public PagingContext PagingContext
        {
            get;
            set;
        }

        /// <summary>
        /// Lista delle trasmissioni ricevute
        /// </summary>
        public DocsPaVO.trasmissione.infoToDoList[] MyToDoList
        {
            get;
            set;
        }
    }
}
