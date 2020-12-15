using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Listener
{
    /// <summary>
    /// Interfaccia listener per la pubblicazione dei contenuti
    /// </summary>
    public interface IListener
    {
        /// <summary>
        /// Azione di notifica di un evento che si è verificato nel sistema esterno
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta contenenti il canale di pubblicazione, i dettagli dell'evento e l'oggetto
        /// che l'ha generato 
        /// </param>
        /// <returns>
        /// Dati della risposta contenenti l'esito del calcolo delle regole di pubblicazione registrate per il canale
        /// </returns>
        ListenerResponse NotifyEvent(ListenerRequest request);
    }
}
