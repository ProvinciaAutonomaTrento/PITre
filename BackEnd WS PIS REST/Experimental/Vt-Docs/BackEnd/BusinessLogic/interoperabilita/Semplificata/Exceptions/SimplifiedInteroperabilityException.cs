using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interoperability.Domain;
using System.Runtime.Serialization;

namespace BusinessLogic.interoperabilita.Semplificata.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando si verifica un problema durante la gestione della richiesta di interoperabilità
    /// </summary>
    [Serializable]
    [DataContract]
    public class SimplifiedInteroperabilityException : Exception
    {
        public SimplifiedInteroperabilityException() { this.Requests = new List<SingleRequest>(); }
        public SimplifiedInteroperabilityException(string message) : base(message) { this.Requests = new List<SingleRequest>(); }
        public SimplifiedInteroperabilityException(string message, Exception inner) : base(message, inner) { this.Requests = new List<SingleRequest>(); }
        protected SimplifiedInteroperabilityException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { this.Requests = new List<SingleRequest>(); }

        /// <summary>
        /// Lista delle singole elaborazioni su cui sono stati rilevati dei problemi
        /// </summary>
        public List<SingleRequest> Requests { get; set; }
    }

    /// <summary>
    /// Informazioni sull'errore verificato per una elaborazione
    /// </summary>
    public class SingleRequest
    {
        /// <summary>
        /// Lista delle informazioni sui destinatari che hanno generato problemi
        /// </summary>
        public List<ReceiverInfo> ReceiverInfoes { get; set; }

        /// <summary>
        /// Lista delle eccezioni
        /// </summary>
        public String ErrorMessage { get; set; }
    }
}
