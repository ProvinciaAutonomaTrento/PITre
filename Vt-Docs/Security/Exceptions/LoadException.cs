using System;

namespace Security.Core.Exceptions
{
    /// <summary>
    /// Eccezione sollevata in caso di problemi nel caricamento delle informazioni sul token da data base
    /// </summary>
    [Serializable]
    public class LoadException : Exception
    {
        static String mex = "Errore durante il caricamento delle informazioni sul token";

        public LoadException() : base(mex) { }
        public LoadException(string message) : base(message) { }
        public LoadException(Exception e) : base("Si è verificato un errore non gestito.", e) { }
        public LoadException(string message, Exception inner) : base(message, inner) { }
        protected LoadException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
   
}
