// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class PianoConsAssTempoConsEntity
    {
        public long SYSTEM_ID { get; set; }
        public string TEMPO_CONSERVAZIONE { get; set; }
        public long? TEMPO_CONSERVAZIONE_IN_ANNI { get; set; }
    }
}
