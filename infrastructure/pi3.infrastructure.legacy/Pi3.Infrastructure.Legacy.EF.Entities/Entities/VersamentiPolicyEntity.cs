// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class VersamentiPolicyEntity
    {
        public long? ID_POLICY { get; set; }

        public long? ID_PROFILE { get; set; }

        public DateTime? DATA_ESECUZIONE_POLICY { get; set; }

        public long? NUM_ESECUZIONE_POLICY { get; set; }
    }
}
