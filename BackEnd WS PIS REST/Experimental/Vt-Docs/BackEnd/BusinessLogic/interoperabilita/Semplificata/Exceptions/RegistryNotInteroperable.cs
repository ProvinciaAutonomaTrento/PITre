using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.interoperabilita.Semplificata.Exceptions
{
    /// <summary>
    /// Eccezione scatenata se il registro su cui creare un predisposto non è interoperante
    /// </summary>
    [Serializable]
    public class RegistryNotInteroperableException : Exception
    {
        public RegistryNotInteroperableException() { }
        public RegistryNotInteroperableException(string message) : base(message) { }
        public RegistryNotInteroperableException(string message, Exception inner) : base(message, inner) { }
        protected RegistryNotInteroperableException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
