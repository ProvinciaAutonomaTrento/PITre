// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class AssPolicyProfilazioneEntity
    {
        public long? ID_POLICY { get; set; }

        public long? ID_TEMPLATE { get; set; }

        public long? ID_OBJ_CUSTOM { get; set; }

        public string? VALORE { get; set; }
    }
}
