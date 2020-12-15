using System;

namespace MText.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando si verifica un'eccezione durante il caricamento della lista dei databindings
    /// definiti in M/Text
    /// </summary>
    class DataBindingsListRetriveException : Exception
    {
        public DataBindingsListRetriveException(Exception e): base("Errore durante il reperimento della lista dei tipi di documento MText", e)
        { }
    }
}
