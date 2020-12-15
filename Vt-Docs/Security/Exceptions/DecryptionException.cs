using System;

namespace Security.Core.Exceptions
{
    /// <summary>
    /// Eccezione sollevata al presentarsi di un errore nell'algortmo di decrypting
    /// </summary>
    [Serializable]
    public class DecryptionException : Exception
    {
        static String mex = "Si è verificato un errore nella decifratura del token di autenticazione.";

        public DecryptionException() : base(mex) { }
        public DecryptionException(Exception e) : base("Si è verificato un errore non gestito.", e) { }
        public DecryptionException(string message) : base(message) { }
        public DecryptionException(string message, Exception inner) : base(message, inner) { }
        protected DecryptionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
    
}
