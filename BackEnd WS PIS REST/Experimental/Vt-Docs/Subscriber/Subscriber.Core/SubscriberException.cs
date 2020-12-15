using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber
{
    /// <summary>
    /// Custom exception per la gestione delle eccezioni delle regole
    /// </summary>
    public class SubscriberException : ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorDescription"></param>
        public SubscriberException(string errorCode, string errorDescription) : base(errorDescription)
        {   
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorDescription"></param>
        /// <param name="innerException"></param>
        public SubscriberException(string errorCode, string errorDescription, Exception innerException)
                                : base(errorDescription, innerException)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Codice dell'errore
        /// </summary>
        public string ErrorCode
        {
            get;
            protected set;
        }
    }
}
