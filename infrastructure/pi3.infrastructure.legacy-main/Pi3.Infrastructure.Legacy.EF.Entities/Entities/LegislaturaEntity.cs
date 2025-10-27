// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class LegislaturaEntity
    {
        public long? SYSTEM_ID { get; set; }
        public string? VAR_COD_LEG { get; set; }
        public string? VAR_DESC_LEG { get; set; }
        public string? DTA_INIZIO { get; set; }
        public DateTime? DTA_FINE { get; set; }

    }
}
