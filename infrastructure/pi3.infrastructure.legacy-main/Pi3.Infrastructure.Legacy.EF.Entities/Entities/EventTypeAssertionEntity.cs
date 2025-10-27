// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class EventTypeAssertionEntity
    {
        public string TYPE_NOTIFY { get; set; }
        public string? IS_EXERCISE { get; set; }
        public long SYSTEM_ID { get; set; }
        public long ID_TYPE_EVENT { get; set; }
        public long? ID_AUR { get; set; }
        public long ID_AMM { get; set; }
        public string? DESC_TYPE_EVENT { get; set; }
        public string? TYPE_AUR { get; set; }
        public string? DESC_AUR { get; set; }

    }
}
