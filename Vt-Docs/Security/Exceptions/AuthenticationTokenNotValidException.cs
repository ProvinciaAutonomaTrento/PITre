using System;

namespace Security.Core.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando il token non è valido
    /// </summary>
    [Serializable]
    public class AuthenticationTokenNotValidException : Exception
    {
        static String mex = "Formato token di autenticazione non valido: parametri obbligatori non presenti.";

        public AuthenticationTokenNotValidException() : base(mex) { }
        public AuthenticationTokenNotValidException(string message) : base(message) { }
        public AuthenticationTokenNotValidException(string message, Exception inner) : base(message, inner) { }
        protected AuthenticationTokenNotValidException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
    
}
