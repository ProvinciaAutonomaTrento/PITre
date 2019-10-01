using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Publisher
{
    /// <summary>
    /// Codici errore
    /// </summary>
    public sealed class ErrorCodes
    {
        /// <summary>
        /// Errore non gestito
        /// </summary>
        public const string UNHANDLED_ERROR = "UNHANDLED_ERROR";

        /// <summary>
        /// Errore operazione non consentita
        /// </summary>
        public const string OPERATION_NOT_ALLOWED = "OPERATION_NOT_ALLOWED";

        /// <summary>
        /// Errore operazione non consentita in quanto il servizio risulta attivo
        /// </summary>
        public const string OPERATION_NOT_ALLOWED_PER_SERVICE_STARTED = "OPERATION_NOT_ALLOWED_PER_SERVICE_STARTED";

        /// <summary>
        /// Errore operazione non consentita in quanto il servizio risulta fermo
        /// </summary>
        public const string OPERATION_NOT_ALLOWED_PER_SERVICE_STOPPED = "OPERATION_NOT_ALLOWED_PER_SERVICE_STOPPED";

        /// <summary>
        /// Errore nell'esecuzione della SP
        /// </summary>
        public const string SP_EXECUTION_ERROR = "SP_EXECUTION_ERROR";

        /// <summary>
        /// Canale di pubblicazione non trovato
        /// </summary>
        public const string PUBLISH_CHANNEL_NOT_FOUND = "PUBLISH_CHANNEL_NOT_FOUND";

        /// <summary>
        /// Tipo oggetto non gestito o non valido
        /// </summary>
        public const string INVALID_OBJECT_TYPE = "INVALID_OBJECT_TYPE";

        /// <summary>
        /// Utente non trovato
        /// </summary>
        public const string USER_NOT_FOUND = "USER_NOT_FOUND";

        /// <summary>
        /// Ruolo non trovato
        /// </summary>
        public const string ROLE_NOT_FOUND = "ROLE_NOT_FOUND";

        /// <summary>
        /// Tipo oggetto mapper non gestito o non valido
        /// </summary>
        public const string INVALID_MAPPER_OBJECT_TYPE = "INVALID_MAPPER_OBJECT_TYPE";

        /// <summary>
        /// Errore nel mapping dell'oggetto da pubblicare
        /// </summary>
        public const string MAP_OBJECT_ERROR = "MAP_OBJECT_ERROR";

        /// <summary>
        /// Codice per la gestione dell'errore relativo all'avvio di un canale di pubblicazione su server remoto
        /// </summary>
        public const string START_CHANNEL_ON_REMOTE_SERVER_ERROR = "START_CHANNEL_ON_REMOTE_SERVER_ERROR";

        /// <summary>
        /// Codice per la gestione dell'errore relativo al fermo di un canale di pubblicazione su server remoto
        /// </summary>
        public const string STOP_CHANNEL_ON_REMOTE_SERVER_ERROR = "STOP_CHANNEL_ON_REMOTE_SERVER_ERROR";
    }
}
