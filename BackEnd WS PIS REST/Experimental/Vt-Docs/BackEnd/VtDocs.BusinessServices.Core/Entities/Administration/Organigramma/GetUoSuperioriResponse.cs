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
    public class GetUOSuperioriResponse : Response
    {
        /// <summary>
        /// Lista delle UO superiori
        /// </summary>
        public List<UOByQualifica> UOs
        {
            get;
            set;
        }
    }
}
