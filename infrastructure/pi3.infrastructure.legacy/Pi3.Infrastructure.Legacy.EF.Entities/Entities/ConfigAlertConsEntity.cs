// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ConfigAlertConsEntity
    {
        public long SYSTEM_ID { get; set; }

        public long? ID_AMM { get; set; }

        public string? VAR_SERVER_SMTP { get; set; }

        public long? NUM_PORTA_SMTP { get; set; }

        public string? CHA_SMTP_SSL { get; set; }

        public string? VAR_USER_MAIL { get; set; }

        public string? VAR_PWD_MAIL { get; set; }

        public string? VAR_MAIL_NOTIFICA { get; set; }

        public string? VAR_MAIL_DESTINATARIO { get; set; }

        public string? CHA_ALERT_LEGGIBILITA_SCADENZA { get; set; }

        public long? NUM_LEGG_SCADENZA_TERMINE { get; set; }

        public long? NUM_LEGG_SCADENZA_TOLLERANZA { get; set; }

        public string? CHA_ALERT_LEGGIBILITA_MAX_DOC { get; set; }

        public long? NUM_LEGGIBILITA_MAX_DOC_PERC { get; set; }

        public string? CHA_ALERT_LEGGIBILITA_SING { get; set; }

        public long? NUM_LEGG_SING_MAX_OPER { get; set; }

        public long? NUM_LEGG_SING_PERIODO_MON { get; set; }

        public string? CHA_ALERT_DOWNLOAD { get; set; }

        public long? NUM_DOWNLOAD_MAX_OPER { get; set; }

        public long? NUM_DOWNLOAD_PERIODO_MON { get; set; }

        public string? CHA_ALERT_SFOGLIA { get; set; }

        public long? NUM_SFOGLIA_MAX_OPER { get; set; }

        public long? NUM_SFOGLIA_PERIODO_MON { get; set; }
        public string? PROVIDER_ID { get; set; }
        public string? MS_TENANT_ID { get; set; }
        public string? MS_CLIENT_ID { get; set; }
        public string? MS_CLIENT_SEC { get; set; }
    }
}
