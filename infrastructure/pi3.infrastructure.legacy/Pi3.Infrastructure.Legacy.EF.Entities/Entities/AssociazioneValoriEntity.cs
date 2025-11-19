// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class AssociazioneValoriEntity
    {
        public long SYSTEM_ID { get; set; }

        public string? DESCRIZIONE_VALORE { get; set; }

        public string? VALORE { get; set; }

        public string? VALORE_DI_DEFAULT { get; set; }

        public long? ID_OGGETTO_CUSTOM { get; set; }

        public long? ABILITATO { get; set; }

        public string? COLOR_BG { get; set; }
    }
}
