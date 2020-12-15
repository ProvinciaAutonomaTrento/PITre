using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Reporting.Exceptions
{
    /// <summary>
    /// Eccezione sollevata al verificarsi di un problema nella generazione di un report
    /// </summary>
    [Serializable]
    public class ReportGenerationFailedException : Exception
    {
        public ReportGenerationFailedException() { }

        /// <summary>
        /// Costruttore per l'inizializzazione di una eccezione di questo tipo a partire da un messaggio
        /// </summary>
        /// <param name="message">Informazioni aggiuntive sull'eccezione</param>
        public ReportGenerationFailedException(string message) : base(message) { }

        /// <summary>
        /// Costruttore per l'inizializzazione di una eccezione di questo tipo a partire da un messaggio e
        /// da una eccezzione innestata
        /// </summary>
        /// <param name="message">Informazioni aggiuntive sull'eccezione</param>
        /// <param name="inner">Eccezione innestata</param>
        public ReportGenerationFailedException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Costruttore per l'inizializzazione di una eccezione di questo tipo a partire da informazioni serializzabili
        /// e da un contesto
        /// </summary>
        /// <param name="info">Informazioni serializzabili</param>
        /// <param name="context">Contesto</param>
        protected ReportGenerationFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
