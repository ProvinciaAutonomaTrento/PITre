using System;

namespace MText.Exceptions
{
    /// <summary>
    /// Eccezione sollevata al verificarsi di un problema durante il reperimento 
    /// </summary>
    class DocumentRetrivingException : Exception
    {
        public DocumentRetrivingException(Exception ex)
            : base("Errore durante il reperimento dell'url del documento M/Text", ex)
        { }


    }
}
