using System;

namespace Security.Core.Exceptions
{
    /// <summary>
    /// Eccezione sollevata in caso di fallimento nel caricamento del token
    /// </summary>
    [Serializable]
    public class RemoveException : Exception
    {
        static String mex = "Errore durante l'eliminazione del token dal DB";

        public RemoveException() : base(mex) { }
        public RemoveException(Exception e) : base("Si è verificato un errore non gestito.", e) { }
        public RemoveException(string message) : base(message) { }
        public RemoveException(string message, Exception inner) : base(message, inner) { }
        protected RemoveException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

}
