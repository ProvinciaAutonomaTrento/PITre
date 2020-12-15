using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.PrjDocImport
{
    /// <summary>
    /// Classe utilizzata per contenere i dati estratti da
    /// una sorgente dati relativi alle varie tipologie di documento
    /// </summary>
    public class DocumentRowDataContainer
    {
        /// <summary>
        /// Lista dei dati relativi a protocolli in Arrivo
        /// </summary>
        public List<DocumentRowData> InDocument;

        /// <summary>
        /// Lista dei dati relativi a protocolli in Uscita
        /// </summary>
        public List<DocumentRowData> OutDocument;

        /// <summary>
        /// Lista dei dati relativi a protocolli Interni
        /// </summary>
        public List<DocumentRowData> OwnDocument;

        /// <summary>
        /// Lista dei dati relativi a documenti grigi
        /// </summary>
        public List<DocumentRowData> GrayDocument;

        /// <summary>
        /// Lista dei dati relativi ad Allegati
        /// </summary>
        public List<DocumentRowData> AttachmentDocument;

    }
}
