using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Rules
{
    /// <summary>
    /// Interfaccia per la definizione e il calcolo di una regola di pubblicazione di un oggetto
    /// </summary>
    public interface IRule : IDisposable
    {
        /// <summary>
        /// Inizializzazione della regola di pubblicazione
        /// </summary>
        /// <param name="request">
        /// Dati della request provenienti dal listener
        /// </param>
        /// <param name="ruleInfo">
        /// Metadati della regola da elaborare
        /// </param>
        void InitializeRule(Subscriber.Listener.ListenerRequest request, Subscriber.RuleInfo ruleInfo);

        /// <summary>
        /// Nome della regola di pubblicazione
        /// </summary>
        string RuleName
        {
            get;
        }

        /// <summary>
        /// Metodo per il calcolo della regola per la pubblicazione di un oggetto
        /// <remarks>
        /// Nota per gli utilizzatori: al termine del calcolo è necessario valorizzare l'attributo Response 
        /// riportando l'esito del calcolo della regola
        /// </remarks>
        /// </summary>
        void Execute();

        /// <summary>
        /// Esito del calcolo della regola di pubblicazione
        /// </summary>
        RuleResponse Response
        {
            get;
        }
    }
}
