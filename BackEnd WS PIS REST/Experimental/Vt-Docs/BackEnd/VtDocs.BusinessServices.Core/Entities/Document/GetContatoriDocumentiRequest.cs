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
    public class GetContatoriDocumentiRequest : Request
    {
        /// <summary>
        /// 
        /// </summary>
        public int AnnoCreazione
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco dell'unità organizzativa per la quale serve reperire i contatori
        /// </summary>
        public string IdUO
        {
            get;
            set;
        }

        /// <summary>
        /// Lista dei tipi documento (descrizioni) per le quali reperire i contatori
        /// </summary>
        public List<string> DescrizioneTipiDocumento
        {
            get;
            set;
        }
    }
}
