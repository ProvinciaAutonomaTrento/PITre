using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities
{
    /// <summary>
    /// Contesto di paginazione
    /// </summary>
    [Serializable()]
    public class PagingContext
    {
        /// <summary>
        /// 
        /// </summary>
        public int CurrentPageNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int PageSize
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ItemsCount
        {
            get;
            set;
        }
    }
}
