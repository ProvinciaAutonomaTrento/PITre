// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class CheckMailboxEntity
    {
        public long ID { get; set; }

        public long IDJOB { get; set; }

        public long IDUSER { get; set; }

        public long IDROLE { get; set; }

        public long IDREG { get; set; }

        public string MAIL { get; set; } = null!;

        public short? ELABORATE { get; set; }

        public short? TOTAL { get; set; }

        public string CONCLUDED { get; set; } = null!;

        public string? MAILUSERID { get; set; }

        public string? ERRORMESSAGE { get; set; }

        public string? MAILSERVER { get; set; }

        public DateTime? DTA_CONCLUDED { get; set; }
    }
}
