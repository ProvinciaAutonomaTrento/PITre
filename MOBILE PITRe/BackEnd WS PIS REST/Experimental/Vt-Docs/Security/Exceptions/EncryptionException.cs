using System;

namespace Security.Core.Exceptions
{
    /// <summary>
    /// Eccezione sollevata al verificarsi di un errore nell'algoritmo di crypting
    /// </summary>
    [Serializable]
    public class EncryptionException : Exception
    {
        static String mex = "Si è verificato un errore nella cifratura del token di autenticazione";

        public EncryptionException() : base(mex) { }
        public EncryptionException(Exception e) : base("Si è verificato un errore non gestito.", e) { }
        public EncryptionException(string message) : base(message) { }
        public EncryptionException(string message, Exception inner) : base(message, inner) { }
        protected EncryptionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
   
}
