using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Publisher
{
    /// <summary>
    /// Errore riscontrato nella pubblicazione
    /// </summary>
    [Serializable()]
    public class ErrorInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int IdInstance
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorCode
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorDescription
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorStack
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ErrorDate
        {
            get;
            set;
        }
    }
}
