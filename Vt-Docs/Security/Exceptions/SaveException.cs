using System;

namespace Security.Core.Exceptions
{
    /// <summary>
    /// Eccezione scatenata in caso di problemi durante il salvataggio del token nel data base
    /// </summary>
    [Serializable]
    public class SaveException : Exception
    {
        static String mex = "Errore durante il salvataggio del token di autenticazione nel Database";

        public SaveException() : base(mex) { }
        public SaveException(Exception e) : base("Si è verificato un errore non gestito. ", e) { }
        public SaveException(string message) : base(message) { }
        public SaveException(string message, Exception inner) : base(message, inner) { }
        protected SaveException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

}
