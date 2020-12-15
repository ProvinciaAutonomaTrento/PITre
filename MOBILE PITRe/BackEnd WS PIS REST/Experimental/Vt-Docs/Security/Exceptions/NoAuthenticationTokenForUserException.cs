using System;
namespace Security.Core.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando non viene rilevato un token per un dato utente o se il token è scaduto
    /// </summary>
    [Serializable]
    public class NoAuthenticationTokenForUserException : Exception
    {
        static String mex = "Non è stato rilevato alcun token per l'utente o token scaduto {0}";

        public NoAuthenticationTokenForUserException() { }
        public NoAuthenticationTokenForUserException(string message) : base(String.Format(mex, message)) { }
        public NoAuthenticationTokenForUserException(string message, Exception inner) : base(String.Format(mex, message), inner) { }
        protected NoAuthenticationTokenForUserException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

}
