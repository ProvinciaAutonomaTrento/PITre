using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Subscriber.Rules
{
    /// <summary>
    /// Esito dell'applicazione di una regola di pubblicazione
    /// </summary>
    [Serializable()]
    public class RuleResponse
    {
        /// <summary>
        /// Metadati della regola
        /// </summary>
        public RuleInfo Rule
        {
            get;
            set;
        }
    }
}
