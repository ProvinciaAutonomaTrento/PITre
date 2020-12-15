using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Publisher.VtDocs
{
    /// <summary>
    /// Criteri di ricerca dei log nel sistema documentale
    /// </summary>
    [Serializable()]
    public class LogCriteria
    {
        public AdminInfo Admin
        {
            get;
            set;
        }
                
        //public DateTime FromDate
        //{
        //    get;
        //    set;
        //}

        //public DateTime ToDate
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// 
        /// </summary>
        public int FromLogId
        {
            get;
            set;
        }
        
        public string ObjectType
        {
            get;
            set;
        }

        public string ObjectTemplateName
        {
            get;
            set;
        }

        public string EventName
        {
            get;
            set;
        }
    }
}
