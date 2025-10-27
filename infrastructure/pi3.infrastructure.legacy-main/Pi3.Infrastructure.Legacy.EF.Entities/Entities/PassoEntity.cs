// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class PassoEntity
    {
        public string? CHA_STATO_AUTOMATICO_LF { get; set; }
        public long? ID_EVENT { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? ID_STATO { get; set; }
        public long? ID_NEXT_STATO { get; set; }
        public long? ID_DIAGRAMMA { get; set; }
        public long? ID_STATO_AUTO { get; set; }
        public string? DESC_STATO_AUTO { get; set; }
    }
}
