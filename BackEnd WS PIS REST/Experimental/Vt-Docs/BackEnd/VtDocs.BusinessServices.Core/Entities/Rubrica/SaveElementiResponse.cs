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
    public class SaveElementiResponse : Response
    {
        /// <summary>
        /// 
        /// </summary>
        public SaveElementiResponse()
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
