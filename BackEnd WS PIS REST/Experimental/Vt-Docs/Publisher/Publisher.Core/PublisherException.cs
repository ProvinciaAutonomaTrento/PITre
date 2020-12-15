using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Publisher
{
    /// <summary>
    /// Eccezione di pubblicazione
    /// </summary>
    public class PublisherException : ApplicationException
    {
        public PublisherException(string errorCode, string errorDescription)
            : base(errorDescription)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Codice dell'errore riscontrato
        /// </summary>
        public string ErrorCode
        {
            get;
            protected set;
        }
    }
}
