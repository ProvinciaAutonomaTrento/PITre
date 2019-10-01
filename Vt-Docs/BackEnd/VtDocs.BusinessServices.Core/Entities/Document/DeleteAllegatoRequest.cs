using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    [Serializable()]
    public class DeleteAllegatoRequest : Request
    {

        /// <summary>
        /// Id del documento principale
        /// </summary>
        public int idDocumento
        {
            get;
            set;
        }

        /// <summary>
        /// Id del documento allegato da eliminare
        /// </summary>
        public int idDocumentoAllegato
        {
            get;
            set;
        }
    }
}
