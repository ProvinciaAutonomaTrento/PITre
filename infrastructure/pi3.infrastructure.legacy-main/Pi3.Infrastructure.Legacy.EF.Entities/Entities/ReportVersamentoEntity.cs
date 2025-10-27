// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ReportVersamentoEntity
    {
        public long? ID_AMM { get; set; }

        public string? VAR_FIXED_RECIPIENTS { get; set; }

        public string? CHA_ATTIVA_STRUTTURA { get; set; }

        public string? CHA_MAIL_STRUTTURA { get; set; }

        public string? MAIL_SUBJECT { get; set; }

        public string? MAIL_BODY { get; set; }

        public string? VAR_SMTP_SERVER { get; set; }    

        public string? VAR_MAIL_FROM { get; set; }

        public string? VAR_USERNAME_SMTP { get; set; }

        public string? VAR_PASSWORD_SMTP { get; set; }

        public long? VAR_PORT_SMTP { get; set; }

        public string? CHA_SSL { get; set; }

        public string? CHA_MAIL_POLICY { get; set; }

        public string? VAR_POLICY_MAIL_SUBJECT { get; set; }

        public string? VAR_POLICY_MAIL_BODY { get; set; }

    }
}
