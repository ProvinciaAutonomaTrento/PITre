using System;

namespace BusinessLogic.Reporting.Exceptions
{
    /// <summary>
    /// Eccezione sollevata nel caso in cui il cast della request ad un suo tipo specifico fallisce
    /// </summary>
    [Serializable]
    public class RequestNotValidException : Exception
    {
        public RequestNotValidException() : base("Errore nell'analisi della richiesta.") { }

        /// <summary>
        /// Costruttore per l'inizializzazione di una eccezione di questo tipo a partire da un messaggio
        /// </summary>
        /// <param name="message">Informazioni aggiuntive sull'eccezione</param>
        public RequestNotValidException(string message) : base(message) { }

        /// <summary>
        /// Costruttore per l'inizializzazione di una eccezione di questo tipo a partire da un messaggio e
        /// da una eccezzione innestata
        /// </summary>
        /// <param name="message">Informazioni aggiuntive sull'eccezione</param>
        /// <param name="inner">Eccezione innestata</param>
        public RequestNotValidException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Costruttore per l'inizializzazione di una eccezione di questo tipo a partire da informazioni serializzabili
        /// e da un contesto
        /// </summary>
        /// <param name="info">Informazioni serializzabili</param>
        /// <param name="context">Contesto</param>
        protected RequestNotValidException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
