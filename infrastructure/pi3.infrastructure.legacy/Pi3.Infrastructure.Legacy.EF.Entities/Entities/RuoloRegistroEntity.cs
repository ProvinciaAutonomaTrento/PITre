// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class RuoloRegistroEntity
    {
        public long SYSTEM_ID { get; set; }
        public long? ID_REGISTRO { get; set; }
        public long? ID_RUOLO_IN_UO { get; set; }
        public string? CHA_PREFERITO { get; set; }
        public string? CHA_RIFERIMENTO { get; set; }
        public DateTime? DTA_INIZIO { get; set; }
        public DateTime? DTA_FINE { get; set; }
        public DateTime? DTA_ASS_VISIBILITA { get; set; }
        public string? CHA_PROTOCOLLO_ABILITATO { get; set; }
    }
}
