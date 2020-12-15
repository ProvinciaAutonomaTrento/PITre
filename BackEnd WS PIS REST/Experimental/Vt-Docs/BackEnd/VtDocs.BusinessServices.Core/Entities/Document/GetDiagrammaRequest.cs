using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Classe request per il metodo "GetDiagramma"
    /// </summary>
    [Serializable()]
    public class GetDiagrammaRequest : Request
    {
        /// <summary>
        /// Identificativo univoco del diagramma da reperire
        /// </summary>
        public string IdDiagramma
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione univoca del diagramma da reperire
        /// </summary>
        public string DescrizioneDiagramma
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco del template del documento da cui reperire gli stati
        /// </summary>
        public string IdTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco del documento da cui reperire i dati relativi allo stato corrente
        /// </summary>
        /// <remarks>
        /// Dato facoltativo
        /// </remarks>
        public string IdDocumento
        {
            get;
            set;
        }
    }
}
