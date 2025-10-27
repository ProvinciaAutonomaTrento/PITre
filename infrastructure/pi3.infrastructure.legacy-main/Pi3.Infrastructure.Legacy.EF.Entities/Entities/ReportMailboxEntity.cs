// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ReportMailboxEntity
    {
        public long ID { get; set; }

        public long ID_CHECK_MAILBOX { get; set; }

        public string? MAILID { get; set; }

        public string? TYPE { get; set; }

        public string? RECEIPT { get; set; }

        public DateTime? DATE_MAIL { get; set; }

        public string? FROM_MAIL { get; set; }

        public string? ERROR { get; set; }

        public long? COUNT_ATTACHMENTS { get; set; }

        public string? SUBJECT { get; set; }
    }
}
