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
    public class GetDocumentoResponse : Response
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
        /// Dati dello stato corrente del documento
        /// </summary>
        public DocsPaVO.DiagrammaStato.Stato StatoCorrente
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool EnableMittMult
        {
            get;
            set;
        }

        /// <summary>
        /// Lista mezzi di spedizione visibili per l'utente
        /// </summary>
        public DocsPaVO.amministrazione.MezzoSpedizione[] MezziSpedizione
        {
            get;
            set;
        }
    }
}
