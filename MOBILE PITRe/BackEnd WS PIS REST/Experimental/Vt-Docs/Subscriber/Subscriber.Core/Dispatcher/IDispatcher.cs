using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Dispatcher
{
    /// <summary>
    /// Interfaccia per la pubblicazione di oggetti
    /// </summary>
    public interface IDispatcher : IDisposable
    {
        /// <summary>
        /// Metodo per la pubblicazione dei contenuti
        /// </summary>
        /// <param name="data">
        /// Dati da pubblicare
        /// </param>
        void Dispatch(object data);
    }
}
