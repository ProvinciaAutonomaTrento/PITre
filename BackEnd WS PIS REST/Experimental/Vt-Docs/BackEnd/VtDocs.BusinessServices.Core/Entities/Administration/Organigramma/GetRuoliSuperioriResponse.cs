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
    public class GetRuoliSuperioriResponse : Response
    {
        /// <summary>
        /// 
        /// </summary>
        public List<RuoloInUO> Superiori
        {
            get;
            set;
        }
    }
}
