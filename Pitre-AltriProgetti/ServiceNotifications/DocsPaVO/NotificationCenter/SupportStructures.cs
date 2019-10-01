using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.NotificationCenter
{
    public static class SupportStructures
    {
        public struct TypeAggregator
        {
            public const string AMM = "AMM";
            public const string RF = "RF";
            public const string TR = "TR";
            public const string R = "R";
            public const string UO = "UO";
        }

        public struct ListDomainObject
        {
            public const string DOCUMENT = "DOCUMENTO";
            public const string ALLEGATO = "ALLEGATO";
            public const string FASCICOLO = "FASCICOLO";
            public const string JOB = "JOB";
        }

        /// <summary>
        /// Indica tutte le opzioni di configurazione per un tipo evento
        /// </summary>
        public struct ConfigurationEventType
        {
            public const string CONF = "CON";
            public const string OBB = "OBB";
            public const string NN = "NN";
        }

        /// <summary>
        /// Lista delle possibili logiche di individuazione dei destinatari della notifica.
        /// </summary>
        public struct BuildingOptionsRecipients
        {
            public const string TRANSMISSION_RECIPIENTS = "TRANSMISSION_RECIPIENTS";
            public const string SENDER_TRANSMISSION = "SENDER_TRANSMISSION";
            public const string ACL_DOMAINOBJECT = "ACL_DOMAINOBJECT";
            public const string S_SENDER_USERS_ROLE_SENDER_TRANSMISSION = "S_SENDER_USERS_ROLE_SENDER_TRANSMISSION";
            public const string S_PRODUCER_EVENT = "S_PRODUCER_EVENT";
            public const string USERS_ROLE_PRODUCER = "USERS_ROLE_PRODUCER";
            public const string USERS_ROLE_IN_PROCESS = "USERS_ROLE_IN_PROCESS";
            public const string USER_RECIPIENT_TASK = "USER_RECIPIENT_TASK";
        }

        /// <summary>
        /// Tipo notifica(informativa, operativa)
        /// </summary>
        public struct NotificationType
        {
            public const char OPERATIONAL = 'O';
            public const char INFORMATION = 'I';
        }

        public struct MULTIPLICITY
        {
            public const string ONE = "ONE";
            public const string ALL = "ALL";
        }

        public struct EventType
        {
            public const string TRASM = "TRASM";
            public const string ACCEPT_TRASM = "ACCEPT_TRASM";
            public const string REJECT_TRASM = "REJECT_TRASM";
            public const string CHECK_TRASM = "CHECK_TRASM";
            public const string MODIFIED_OBJECT_PROTO = "MODIFIED_OBJECT_PROTO";
            public const string ANNULLA_PROTO = "ANNULLA_PROTO";
            public const string DOC_CAMBIO_STATO = "DOC_CAMBIO_STATO";
            public const string DOCUMENTOCONVERSIONEPDF = "DOCUMENTOCONVERSIONEPDF";
            public const string NO_DELIVERY_SEND_SIMPLIFIED_INTEROPERABILITY = "NO_DELIVERY_SEND_SIMPLIFIED_INTEROPERABILITY";
            public const string RECORD_PREDISPOSED = "RECORD_PREDISPOSED";
            // Modifica PEC 4 requisito 2
            public const string NO_DELIVERY_SEND_PEC = "NO_DELIVERY_SEND_PEC";
            public const string EXCEPTION_INTEROPERABILITY_PEC = "EXCEPTION_INTEROPERABILITY_PEC";

            public const string FOLLOW_DOC_EXT_APP = "FOLLOW_DOC_EXT_APP";
            public const string FOLLOW_FASC_EXT_APP = "FOLLOW_FASC_EXT_APP";
            public const string EXCEPTION_SEND_SIMPLIFIED_INTEROPERABILITY = "EXCEPTION_SEND_SIMPLIFIED_INTEROPERABILITY";

            //Per MEV istanza di accesso
            public const string CREATED_FILE_ZIP_INSTANCE_ACCESS = "CREATED_FILE_ZIP_INSTANCE_ACCESS";
            public const string FAILED_CREATING_FILE_ZIP_INSTANCE_ACCESS = "FAILED_CREATING_FILE_ZIP_INSTANCE_ACCESS";

            // Per mev Libro firma
            public const string INTERROTTO_PROCESSO = "INTERROTTO_PROCESSO";
            public const string CONCLUSIONE_PROCESSO = "CONCLUSIONE_PROCESSO";
            public const string TRONCAMENTO_PROCESSO = "TRONCAMENTO_PROCESSO";
            public const string PROCESSO_FIRMA = "PROCESSO_FIRMA";
            public const string PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO = "PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO";
            public const string PROCESSO_FIRMA_DESTINATARI_NON_INTEROP = "PROCESSO_FIRMA_DESTINATARI_NON_INTEROP";
        }

        public struct TypeProtocol
        {
            public const string INTERNO = "I";
            public const string ARRIVO = "A";
            public const string PARTENZA = "P";
            public const string GRIGIO = "G";
            public const string STAMPAREG = "R";
            public const string LABEL_INTERNO = "lblInterno";
            public const string LABEL_ARRIVO = "lblArrivo";
            public const string LABEL_PARTENZA = "lblPartenza";
            public const string LABEL_GRIGIO = "lblGrigio";
            public const string LABEL_STAMPAREG = "lblStampaReg";
        }

        public struct TagItem
        {
            public const string LINE = "<line>";
            public const string CLOSE_LINE = "</line>";
            public const string LABEL = "<label>";
            public const string CLOSE_LABEL = "</label>";
            public const string COLORRED = "<colorRed>";
            public const string CLOSE_COLORRED = "</colorRed>";
            public const string COLORRED_STRIKE = "<colorRedStrike>";
            public const string CLOSE_COLORRED_STRIKE = "</colorRedStrike>";
        }
    }
}
