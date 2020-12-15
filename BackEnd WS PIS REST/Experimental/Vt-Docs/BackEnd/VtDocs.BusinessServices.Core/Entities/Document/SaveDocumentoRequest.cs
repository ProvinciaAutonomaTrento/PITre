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
    public class SaveDocumentoRequest : Request
    {
        /// <summary>
        /// 
        /// </summary>
        public DocsPaVO.documento.SchedaDocumento Documento
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo del fascicolo in cui inserire il documento
        /// </summary>
        public string IdFascicolo
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo del modello trasmissione utilizzato per inviare il documento
        /// </summary>
        public string IdModelloTrasmissione
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco dello stato del workflow cui il documento deve essere impostato
        /// </summary>
        public string IdStatoDestinazione
        {
            get;
            set;
        }

        /// <summary>
        /// Nome univoco dello stato del workflow cui il documento deve essere impostato
        /// </summary>
        /// <remarks>
        /// Utile per gli utilizzatori che dispongono del solo nome dello stato.
        /// Da utilizzare in alternativa all'attributo "IdStatoDestinazione"
        /// </remarks>
        public string NomeStatoDestinazione
        {
            get;
            set;
        }
    }
}
