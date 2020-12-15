using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento
{
    /// <summary>
    /// Classe di eccezione utilizzata dai servizi di versamento
    /// </summary>
    public class PacchettiException : ApplicationException
    {
        public PacchettiException(string errorCode, string errorDescription)
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