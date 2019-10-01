using System;

namespace MText.Exceptions
{
    /// <summary>
    /// Eccezione sollevata durante la creazione di un documento M/Text
    /// </summary>
    class DocumentCreationException : Exception
    {
        public DocumentCreationException(Exception ex) : base("Errore durante la creazione del documento.", ex) { }
    }
}