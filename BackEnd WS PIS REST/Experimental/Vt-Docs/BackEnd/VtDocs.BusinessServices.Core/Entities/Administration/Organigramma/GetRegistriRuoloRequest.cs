using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetRegistriRuoloRequest : Request
    {
        /// <summary>
        /// Identificativo del ruolo
        /// </summary>
        public string IdRuolo
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string IdUO
        {
            get;
            set;
        }
    }
}
