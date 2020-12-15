using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Oggetto Request del servizio per la cancellazione di un documento
    /// </summary>
    [Serializable()]
    public class DeleteDocumentoRequest : Request
    {
        /// <summary>
        /// Identificativo univoco del documento da rimuovere
        /// </summary>
        public string IdDocumento
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se inserire o meno il documento in cestino
        /// </summary>
        public bool MettiInCestino
        {
            get;
            set;
        }

        /// <summary>
        /// Note relativo al motivo di inserimento del documento in cestino
        /// </summary>
        public string NoteCestino
        {
            get;
            set;
        }
    }
}
