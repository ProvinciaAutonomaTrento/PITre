using System;

namespace Security.Core.Exceptions
{
    /// <summary>
    /// Eccezione sollevata al verificarsi di un problema generico derivante dall'esecuzione di una store procedure
    /// </summary>
    [Serializable]
    public class GenericException : Exception
    {
        static String mex = "Si è verificato il seguente errore non gestito. {0}";

        public GenericException() { }
        public GenericException(string message) : base(String.Format(mex, message)) { }
        public GenericException(string message, Exception inner) : base(String.Format(mex, message), inner) { }
        protected GenericException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
