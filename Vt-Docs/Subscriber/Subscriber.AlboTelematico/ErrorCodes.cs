using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.AlboTelematico
{
    /// <summary>
    /// Questa classe elenca gli errori previsti per la pubblicazione su albo telematico
    /// </summary>
    internal sealed class ErrorCodes
    {
        /// <summary>
        /// Formato numerico non valido
        /// </summary>
        public const string FORMAT_NUMBER_ERROR = "FORMAT_NUMBER_ERROR";

        /// <summary>
        /// Indica che si è verificato un errore nell'invocazione dei servizi dell'albo telematico
        /// </summary>
        public const string ALBO_TELEMATICO_SERVICES_ERROR = "ALBO_TELEMATICO_SERVICES_ERROR";

        /// <summary>
        /// Indica che si è verificato un errore nella modifica del diagramma di stato tramite i nuovi WS lato PITRE
        /// </summary>
        public const string WS_MODIFY_DIAGRAM_STATE_ERROR = "WS_MODIFY_DIAGRAM_STATE_ERROR";

        /// <summary>
        /// Indica che si è verificato un errore nella modifica della tipologia atto tramite i nuovi WS lato PITRE
        /// </summary>
        public const string WS_MODIFY_TEMPLATE_ERROR = "WS_MODIFY_TEMPLATE_ERROR";

        /// <summary>
        /// Indica che si è verificato un errore nell' invocazione del servizio di ALT per notificare la presenza di un documento da PUBBLICARE/ANNULLARE/REVOCARE
        /// </summary>
        public const string WS_GETDOCUMENT_ERROR = "WS_GETDOCUMENT_ERROR";
        /// <summary>
        /// Il documento non si trova in uno stato che genera notifica ad ALT (PUBBLICARE/ANNULLARE/REVOCARE)
        /// </summary>
        public const string STATE_DISCARDS_ERROR = "STATE_DISCARDS_ERROR";

        /// <summary>
        /// Il documento non può essere annullato perchè pubblicato da più di 5 giorni
        /// </summary>
        public const string VOID_DOCUMENT_ERROR = "VOID_DOCUMENT_ERROR";

        /// <summary>
        /// Il documento è senza revoca
        /// </summary>
        public const string REVOCATION_DOCUMENT_ERROR = "REVOCATION_DOCUMENT_ERROR";

        ///<summary>
        ///Il documento non può essere pubblicato perchè l'estensione del file non è valida
        ///</summary>
        public const string EXTENSION_FILE_ERROR = "EXTENSION_FILE_ERROR";

        /// <summary>
        /// Indica che si è verificato un errore nell'invocazione del servizio per il reperimento di un token di autenticazione.
        /// </summary>
        public const string WS_GET_AUTHENTICATION_TOKEN_ERROR = "WS_GET_AUTHENTICATION_TOKEN_ERROR";

    }
}