using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.interoperabilita.Semplificata.Exceptions
{
    /// <summary>
    /// Eccezione generata quando viene riscontarato in problema durante l'esecuzione della
    /// trasmissione del documento ai destinatari
    /// </summary>
    [Serializable]
    public class ExecuteTransmissionException : Exception
    {
        public ExecuteTransmissionException() { }
        public ExecuteTransmissionException(string message) : base(message) { }
        public ExecuteTransmissionException(string message, Exception inner) : base(message, inner) { }
        protected ExecuteTransmissionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
