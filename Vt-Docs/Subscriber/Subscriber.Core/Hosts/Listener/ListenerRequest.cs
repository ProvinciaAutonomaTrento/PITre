using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Listener
{
    /// <summary>
    /// Dati di input forniti dal sistema esterno al listener
    /// </summary>
    [Serializable()]
    public class ListenerRequest
    {
        /// <summary>
        /// Dati del canale di pubblicazione
        /// </summary>
        public ChannelInfo ChannelInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Dati dell'evento che si è verificato nel sistema esterno
        /// </summary>
        public EventInfo EventInfo
        {
            get;
            set;
        }
    }
}
