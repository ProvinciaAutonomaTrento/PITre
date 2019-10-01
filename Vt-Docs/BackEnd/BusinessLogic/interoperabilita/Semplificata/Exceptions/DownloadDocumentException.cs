using System;

namespace BusinessLogic.interoperabilita.Semplificata.Exceptions
{
    /// <summary>
    /// Eccezione rilanciata se si verifica un errore durante l'associazione di un file ad un documento
    /// </summary>
    [Serializable]
    public class DownloadDocumentException : Exception
    {
        public DownloadDocumentException() { }
        public DownloadDocumentException(string message) : base(message) { }
        public DownloadDocumentException(string message, Exception inner) : base(message, inner) { }
        protected DownloadDocumentException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
