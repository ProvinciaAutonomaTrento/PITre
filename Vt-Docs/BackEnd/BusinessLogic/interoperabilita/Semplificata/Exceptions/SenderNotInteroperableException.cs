using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.interoperabilita.Semplificata.Exceptions
{
    /// <summary>
    /// Eccezione scatenata se, durante la preparazione della richiesta di interoperabilità il mittente risulta non
    /// interoperante.
    /// </summary>
    [Serializable]
    public class SenderNotInteroperableException : Exception
    {
        public SenderNotInteroperableException() { }
        public SenderNotInteroperableException(string message) : base(message) { }
        public SenderNotInteroperableException(string message, Exception inner) : base(message, inner) { }
        protected SenderNotInteroperableException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
