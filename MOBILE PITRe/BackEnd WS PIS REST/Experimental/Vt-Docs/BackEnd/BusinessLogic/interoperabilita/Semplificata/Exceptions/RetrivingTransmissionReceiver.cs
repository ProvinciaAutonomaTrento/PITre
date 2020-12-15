using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.interoperabilita.Semplificata.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando si verifica un errore durante il caricamento delle informazioni sui destinatari
    /// interni per la trasmissione del documento creato a seguito dell'analisi di una richiesta di interoperabilità
    /// </summary>
    [Serializable]
    public class RetrivingTransmissionReceiverException : Exception
    {
        public RetrivingTransmissionReceiverException() { }
        public RetrivingTransmissionReceiverException(string message) : base(message) { }
        public RetrivingTransmissionReceiverException(string message, Exception inner) : base(message, inner) { }
        protected RetrivingTransmissionReceiverException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
