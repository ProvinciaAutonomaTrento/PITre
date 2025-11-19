// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ConfigStampaConsEntity
    {
        public long SYSTEM_ID { get; set; }

        public long ID_AMM { get; set; }

        public long? DISABLED { get; set; }

        public long? PRINTER_ROLE_ID { get; set; }

        public long? PRINTER_USER_ID { get; set; }

        public long? PRINT_FREQ { get; set; }

        public DateTime? DTA_LAST_PRINT { get; set; }

        public DateTime? DTA_NEXT_PRINT { get; set; }

        public long? ID_LAST_PRINTED_EVENT { get; set; }

        public long? PRINT_HOUR { get; set; }
    }
}
