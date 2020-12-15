using System;

namespace Security.Core.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando esiste già un token per un utente
    /// </summary>
    [Serializable]
    public class TokenAlreadyExistException : Exception
    {
        static String messageFormat = "Esiste già un token per l'utente {0}";

        public TokenAlreadyExistException() { }
        public TokenAlreadyExistException(string message) : base(String.Format(messageFormat, message)) { }
        public TokenAlreadyExistException(string message, Exception inner) : base(String.Format(messageFormat, message), inner) { }
        protected TokenAlreadyExistException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
    
}
