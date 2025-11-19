// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class PolicyEsecuzioneEntity
    {
        public DateTime? START_EXECUTE_DATE { get; set; }
        public DateTime? END_EXECUTE_DATE { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? ID_POLICY { get; set; }
        public long? N_OBJ_CONSERVATI { get; set; }
        public long? ID_ULTIMO_OBJ { get; set; }
        public long? ID_PRIMO_OBJ { get; set; }
        public long? ID_ISTANZA { get; set; }
        public long? ID_AMM { get; set; }
    }
}
