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
    public class NewDocumentoRequest : Request
    {
        /// <summary>
        /// Indica la tipologia documento desiderata
        /// </summary>
        public DocumentTypesEnum DocumentType
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il servizio dovrà restituire anche la lista dei registri di protocollo visibili dall'utente
        /// </summary>
        public bool GetRegistri
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il servizio dovrà restituire anche la lista dei mezzi di spedizione visibili dall'utente
        /// </summary>
        public bool GetMezzoSpedizione
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il servizio dovrà restituire anche la lista delle tipologie documentali disponibili
        /// </summary>
        public bool GetTipologieDocumento
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il servizio dovrà restituire anche la lista dei modelli trasmissione disponibili
        /// </summary>
        public bool GetModelliTrasmissione
        {
            get;
            set;
        }
    }
}
