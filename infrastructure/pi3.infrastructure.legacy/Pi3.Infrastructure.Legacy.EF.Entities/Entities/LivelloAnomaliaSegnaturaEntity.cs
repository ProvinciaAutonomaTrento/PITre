// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class LivelloAnomaliaSegnaturaEntity
    {
        public long SYSTEM_ID { get; set; }
        public string? VAR_EMAIL { get; set; }
        public string? VAR_ANOMALIA { get; set; }
        public string? CHA_LIVELLO_ECCEZIONE { get; set; }
    }
}
