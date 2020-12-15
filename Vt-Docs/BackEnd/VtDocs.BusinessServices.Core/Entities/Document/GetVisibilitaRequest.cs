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
    public class GetVisibilitaRequest : Request
    {
        /// <summary>
        /// Identificativo del documento
        /// </summary>
        public string IdDocumento
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se visualizzare o meno gli elementi di visibilità rimossi
        /// </summary>
        public bool VisualizzaRevocati
        {
            get;
            set;
        }
    }
}
