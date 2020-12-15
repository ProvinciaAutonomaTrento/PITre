using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber
{
    /// <summary>
    /// Codici di errore ammessi nel Subscriber
    /// </summary>
    public sealed class ErrorCodes
    {
        /// <summary>
        /// Errore non gestito
        /// </summary>
        public const string UNHANDLED_ERROR = "UNHANDLED_ERROR";

        /// <summary>
        /// Il campo del profilo risulta mancante
        /// </summary>
        public const string MISSING_FIELD = "MISSING_FIELD";

        /// <summary>
        /// Il modello non risulta applicabile alla regola
        /// </summary>
        public const string INVALID_TEMPLATE_PER_RULE = "INVALID_TEMPLATE_PER_RULE";

        /// <summary>
        /// Tipo specificato per una regola non valido
        /// </summary>
        public const string INVALID_RULE_TYPE = "INVALID_RULE_TYPE";

        /// <summary>
        /// Errore nell'esecuzione della SP
        /// </summary>
        public const string SP_EXECUTION_ERROR = "SP_EXECUTION_ERROR";

        /// <summary>
        /// 
        /// </summary>
        public const string SEND_MAIL_ERROR = "SEND_MAIL_ERROR";

        /// <summary>
        /// Opzione della regola non definita
        /// </summary>
        public const string RULE_OPTION_NOT_DEFINIED = "OPTION_NOT_DEFINIED";
    }
}
