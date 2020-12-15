using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.interoperabilita.Semplificata.Exceptions
{
    /// <summary>
    /// Eccezione scatenata quando avviene un errore durante la creazione del documento predisposto
    /// </summary>
    [Serializable]
    public class CreatingDocumentException : Exception
    {
        public CreatingDocumentException() { }
        public CreatingDocumentException(string message) : base(message) { }
        public CreatingDocumentException(string message, Exception inner) : base(message, inner) { }
        protected CreatingDocumentException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
