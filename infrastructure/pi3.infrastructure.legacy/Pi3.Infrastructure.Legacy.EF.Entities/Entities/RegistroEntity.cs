// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class RegistroEntity
    {
        public string? ANNO_PREG { get; set; }
        public string? CHA_AUTO_INTEROP { get; set; }
        public string? CHA_AUTOMATICO { get; set; }
        public string? CHA_DISABILITATO { get; set; }
        public string? CHA_IMAP_SSL { get; set; }
        public string? CHA_POP_SSL { get; set; }
        public string? CHA_RF { get; set; }
        public string? CHA_RICEVUTA_PEC { get; set; }
        public string? CHA_SMTP_SSL { get; set; }
        public string? CHA_SMTP_STA { get; set; }
        public string? CHA_STATO { get; set; }
        public string? CODICE_CLASSIFICAZIONE { get; set; }
        public string? CODICE_UAC { get; set; }
        public long? DIRITTO_RUOLO_AOO { get; set; }
        public DateTime? DTA_CLOSE { get; set; }
        public DateTime? DTA_OPEN { get; set; }
        public DateTime? DTA_ULTIMO_PROTO { get; set; }
        public string? FLAG_WSPIA { get; set; }
        public long? ID_AMM { get; set; }
        public long? ID_AOO_COLLEGATA { get; set; }
        public long? ID_GRUPPO_FACTORY { get; set; }
        public long? ID_PEOPLE_AOO { get; set; }
        public long? ID_PEOPLE_FACTORY { get; set; }
        public long? ID_RUOLO_AOO { get; set; }
        public long? ID_RUOLO_RESP { get; set; }
        public long? ID_UTENTE_RESP { get; set; }
        public long? INVIO_RICEVUTA_MANUALE { get; set; }
        public long? NUM_PORTA_IMAP { get; set; }
        public long? NUM_PORTA_POP { get; set; }
        public long? NUM_PORTA_SMTP { get; set; }
        public long? NUM_RIF { get; set; }
        public long SYSTEM_ID { get; set; }
        public string? VAR_BOX_MAIL_ELABORATE { get; set; }
        public string? VAR_CODICE { get; set; }
        public string? VAR_CODICE_AOO_IPA { get; set; }
        public string? VAR_CODICE_IPA { get; set; }
        public string? VAR_DESC_REGISTRO { get; set; }
        public string? VAR_EMAIL_REGISTRO { get; set; }
        public string? VAR_INBOX_IMAP { get; set; }
        public string? VAR_MAIL_NON_ELABORATE { get; set; }
        public string? VAR_MAIL_RIC_PENDENTE { get; set; }
        public string? VAR_PREG { get; set; }
        public string? VAR_PWD_MAIL { get; set; }
        public string? VAR_PWD_SMTP { get; set; }
        public string? VAR_SERVER_IMAP { get; set; }
        public string? VAR_SERVER_POP { get; set; }
        public string? VAR_SERVER_SMTP { get; set; }
        public string? VAR_SOLO_MAIL_PEC { get; set; }
        public string? VAR_TIPO_CONNESSIONE { get; set; }
        public string? VAR_USER_MAIL { get; set; }
        public string? VAR_USER_SMTP { get; set; }
        public string? PROVIDER_ID { get; set; }
        public string? MS_TENANT_ID { get; set; }
        public string? MS_CLIENT_ID { get; set; }
        public string? MS_CLIENT_SEC { get; set; }
        public string? MS_FOLD_TO_READ { get; set; }
    }
}
