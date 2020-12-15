using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Rubrica
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class SaveElementiRequest : Request
    {
        /// <summary>
        /// 
        /// </summary>
        public SaveElementiRequest()
        {
            this.Elementi = new List<DettaglioElementoRubrica>();
        }

        /// <summary>
        /// 
        /// </summary>
        public List<DettaglioElementoRubrica> Elementi
        {
            get;
            set;
        }
    }
}
