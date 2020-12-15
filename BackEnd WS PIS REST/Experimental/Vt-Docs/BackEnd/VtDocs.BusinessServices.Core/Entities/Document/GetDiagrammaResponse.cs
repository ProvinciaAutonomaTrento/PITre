using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Classe response per il metodo "GetDiagramma"
    /// </summary>
    [Serializable()]
    public class GetDiagrammaResponse : Response
    {
        /// <summary>
        /// Dati del diagramma di stato
        /// </summary>
        public DocsPaVO.DiagrammaStato.DiagrammaStato Diagramma
        {
            get;
            set;
        }

        /// <summary>
        /// Stato in cui si trova correntemente il documento
        /// </summary>
        public DocsPaVO.DiagrammaStato.Stato StatoDocumento
        {
            get;
            set;
        }
    }
}
