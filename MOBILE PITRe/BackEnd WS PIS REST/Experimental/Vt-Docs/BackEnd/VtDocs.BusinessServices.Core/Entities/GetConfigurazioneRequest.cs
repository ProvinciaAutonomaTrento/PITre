using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetConfigurazioneRequest : Request
    {
        /// <summary>
        /// 
        /// </summary>
        public string IdAmministrazione
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string CodiceChiave
        {
            get;
            set;
        }
    }
}
