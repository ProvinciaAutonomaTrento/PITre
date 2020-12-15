using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interoperability.Domain;

namespace BusinessLogic.interoperabilita.Semplificata.Exceptions
{
    /// <summary>
    /// Eccezione scatenata quando si verifica un errore durante il caricamento delle informazioni su alcuni dei destinatari
    /// della trasmissione del documento creato a seguito della richiesta di interoperabilità
    /// </summary>
    [Serializable]
    public class RetrivingSomeTransmissionReceiverException : Exception
    {
        public RetrivingSomeTransmissionReceiverException() { }
        public RetrivingSomeTransmissionReceiverException(string message) : base(message) { }
        public RetrivingSomeTransmissionReceiverException(string message, Exception inner) : base(message, inner) { }
        protected RetrivingSomeTransmissionReceiverException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
        public RetrivingSomeTransmissionReceiverException(String message, List<ReceiverInfo> receivers) : base(message)
        {
            this.Receivers = receivers;
 
        }


        public List<ReceiverInfo> Receivers { get; set; }
    }
}
