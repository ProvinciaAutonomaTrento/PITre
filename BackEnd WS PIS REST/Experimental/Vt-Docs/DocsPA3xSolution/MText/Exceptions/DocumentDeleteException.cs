using System;

namespace MText.Exceptions
{
    /// <summary>
    /// Eccezione sollevata al verificarsi di un errore durante la rimozione di un documento M/Text
    /// </summary>
    class DocumentDeleteException : Exception
    {
        public DocumentDeleteException(Exception e)
            : base("Errore durante la cancellazione del documento.", e)
        { }
    }
}
