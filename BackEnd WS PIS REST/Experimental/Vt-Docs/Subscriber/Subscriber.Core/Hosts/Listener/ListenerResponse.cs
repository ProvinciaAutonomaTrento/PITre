using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Listener
{
    /// <summary>
    /// Esito ed informazioni aggiuntive restituite dal listener a seguito della pubblicazione di contenuti
    /// </summary>
    [Serializable()]
    public class ListenerResponse
    {
        /// <summary>
        /// Errore verificatosi nel listener
        /// </summary>
        public ErrorInfo Error
        {
            get;
            set;
        }

        /// <summary>
        /// Esito delle rules applicate dal listener
        /// </summary>
        public Rules.RuleResponse[] RuleResponseList
        {
            get;
            set;
        }
    }
}
