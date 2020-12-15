using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interoperability.Domain;

namespace BusinessLogic.interoperabilita.Semplificata.Exceptions
{
    /// <summary>
    /// Eccezione scatenata quando si verifica un errore durante il recupero delle informazioni sul mittente del messaggio
    /// di interoperabilità semplificata
    /// </summary>
    [Serializable]
    public class RetrivingSenderInfoException : Exception
    {
        public RetrivingSenderInfoException() { }
        public RetrivingSenderInfoException(string message) : base(message) { }
        public RetrivingSenderInfoException(string message, Exception inner) : base(message, inner) { }
        protected RetrivingSenderInfoException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

    }
}
