// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox
{
    internal class InfoRegEntity
    {
        public string? VAR_CODICE { get; internal set; }
        public long SYSTEM_ID { get; internal set; }
        public long? ID_AMM { get; internal set; }
        public string? VAR_USER_MAIL { get; internal set; }
        public string? VAR_PWD_MAIL { get; internal set; }
        public string? VAR_SERVER_SMTP { get; internal set; }
        public long? NUM_PORTA_SMTP { get; internal set; }
        public string? VAR_EMAIL_REGISTRO { get; internal set; }
        public string? VAR_CODICE_AMM { get; internal set; }
        public string? VAR_USER_SMTP { get; internal set; }
        public string? CHA_STR_SEGNATURA { get; internal set; }
        public string? VAR_PWD_SMTP { get; internal set; }
        public string? CHA_POP_SSL { get; internal set; }
        public string? CHA_SMTP_SSL { get; internal set; }
        public string? CHA_SMTP_STA { get; internal set; }
        public string? VAR_SERVER_IMAP { get; internal set; }
        public long? NUM_PORTA_IMAP { get; internal set; }
        public string? VAR_TIPO_CONNESSIONE { get; internal set; }
        public string? VAR_INBOX_IMAP { get; internal set; }
        public string? VAR_BOX_MAIL_ELABORATE { get; internal set; }
        public string? VAR_MAIL_NON_ELABORATE { get; internal set; }
        public string? CHA_IMAP_SSL { get; internal set; }
        public string? VAR_SOLO_MAIL_PEC { get; internal set; }
        public long? NUM_PORTA_POP { get; internal set; }
        public string? VAR_SERVER_POP { get; internal set; }
        public string? PROVIDER_ID { get; internal set; }
        public string? MS_TENANT_ID { get; internal set; }
        public string? MS_CLIENT_ID { get; internal set; }
        public string? MS_CLIENT_SEC { get; internal set; }
        public string? MS_FOLD_TO_READ { get; internal set; }
    }
}
