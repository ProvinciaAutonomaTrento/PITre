// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class MailRegistriEntity
    {
        public long SYSTEM_ID { get; set; }
        public long ID_REGISTRO { get; set; }
        public string? VAR_PRINCIPALE { get; set; }
        public string? VAR_EMAIL_REGISTRO { get; set; }
        public string? VAR_USER_MAIL { get; set; }
        public string? VAR_PWD_MAIL { get; set; }
        public string? VAR_SERVER_SMTP { get; set; }
        public string? CHA_SMTP_SSL { get; set; }
        public string? CHA_POP_SSL { get; set; }
        public long? NUM_PORTA_SMTP { get; set; }
        public string? CHA_SMTP_STA { get; set; }
        public string? VAR_SERVER_POP { get; set; }
        public long? NUM_PORTA_POP { get; set; }
        public string? VAR_USER_SMTP { get; set; }
        public string? VAR_PWD_SMTP { get; set; }
        public string? VAR_INBOX_IMAP { get; set; }
        public string? VAR_SERVER_IMAP { get; set; }
        public long? NUM_PORTA_IMAP { get; set; }
        public string? VAR_TIPO_CONNESSIONE { get; set; }
        public string? VAR_BOX_MAIL_ELABORATE { get; set; }
        public string? VAR_MAIL_NON_ELABORATE { get; set; }
        public string? CHA_IMAP_SSL { get; set; }
        public string? VAR_SOLO_MAIL_PEC { get; set; }
        public string? CHA_RICEVUTA_PEC { get; set; }
        public string? VAR_NOTE { get; set; }
        public string? VAR_MAIL_RIC_PENDENTE { get; set; }
        public string? CHA_SALVA_MAIL_LOC { get; set; }
        public string? VAR_MESSAGE_SEND_MAIL { get; set; }
        public string? CHA_OVERWRITE_MESSAGE_AMM { get; set; }
        public string? PROVIDER_ID { get; set; }
        public string? MS_TENANT_ID { get; set; }
        public string? MS_CLIENT_ID { get; set; }
        public string? MS_CLIENT_SEC { get; set; }
        public string? MS_FOLD_TO_READ { get; set; }
    }
}
