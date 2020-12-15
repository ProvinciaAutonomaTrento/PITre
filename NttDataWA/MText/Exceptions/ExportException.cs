using System;

namespace MText.Exceptions
{
    /// <summary>
    /// Eccezione sollevata al verificarsi di un errore durante l'esportazione del documento
    /// </summary>
    class ExportException : Exception
    {
        public ExportException(Exception ex)
            : base("Errore durante l'esportazione del documento nel formato richiesto.", ex)
        { 
        }
    }
}
