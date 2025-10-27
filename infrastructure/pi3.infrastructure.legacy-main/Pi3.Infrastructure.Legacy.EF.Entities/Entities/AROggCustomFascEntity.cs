// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class AROggCustomFascEntity
    {
        public long SYSTEM_ID { get; set; }

        public long? ID_TEMPLATE { get; set; }

        public long? ID_OGGETTO_CUSTOM { get; set; }

        public long? ID_RUOLO { get; set; }

        public long? INS_MOD { get; set; }

        public long? VIS { get; set; }
    }
}
