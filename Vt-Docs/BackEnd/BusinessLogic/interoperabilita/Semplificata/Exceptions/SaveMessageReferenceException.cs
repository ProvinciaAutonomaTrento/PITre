using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.interoperabilita.Semplificata.Exceptions
{
    /// <summary>
    /// Eccezione sollevata nel caso in cui si sia verificata una eccezione nel salvataggio di un riferimento
    /// alla richiesta di interoperabilità
    /// </summary>
    [Serializable]
    public class SaveMessageReferenceException : Exception
    {
        public SaveMessageReferenceException() { }
        public SaveMessageReferenceException(string message) : base(message) { }
        public SaveMessageReferenceException(string message, Exception inner) : base(message, inner) { }
        protected SaveMessageReferenceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
