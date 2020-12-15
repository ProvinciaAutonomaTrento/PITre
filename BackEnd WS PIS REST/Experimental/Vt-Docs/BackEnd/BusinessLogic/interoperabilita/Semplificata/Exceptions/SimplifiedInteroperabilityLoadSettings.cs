using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.interoperabilita.Semplificata.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando si verifica un problema durante il caricamento delle impostazioni relative 
    /// all'interoperabilità semplificata relativamente ad un registro
    /// </summary>
    [Serializable]
    public class SimplifiedInteroperabilityLoadSettingsException : Exception
    {
        public SimplifiedInteroperabilityLoadSettingsException() { }
        public SimplifiedInteroperabilityLoadSettingsException(string message) : base(message) { }
        public SimplifiedInteroperabilityLoadSettingsException(string message, Exception inner) : base(message, inner) { }
        protected SimplifiedInteroperabilityLoadSettingsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
