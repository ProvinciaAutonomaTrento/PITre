using System;

namespace Security.Core.Exceptions
{
    /// <summary>
    /// Eccezione sollevata al verificarsi di un problema durante l'instanziazione di un manager di autenticazione.
    /// </summary>
    [Serializable]
    public class AuthManagerNotFound : Exception
    {
        private static String mex = "Non è stato possibile instanziare il gestore di token identificato dal tipo '{0}'";

        public AuthManagerNotFound() { }
        public AuthManagerNotFound(string message) : base(String.Format(mex, message)) { }
        public AuthManagerNotFound(string message, Exception inner) : base(String.Format(mex, message), inner) { }
        protected AuthManagerNotFound(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

}
