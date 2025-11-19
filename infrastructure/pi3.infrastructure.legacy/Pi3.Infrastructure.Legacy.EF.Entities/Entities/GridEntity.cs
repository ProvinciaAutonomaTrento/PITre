// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class GridEntity
    {
        public string? IS_SEARCH_GRID { get; set; }
        public string? CHA_VISIBILE_A_UTENTE_O_RUOLO { get; set; }
        public string? SERIALIZED_GRID { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? USER_ID_CREATORE { get; set; }
        public long? ROLE_ID_CREATORE { get; set; }
        public long? ADMINISTRATION_ID { get; set; }
        public long? SEARCH_ID { get; set; }
        public string? GRID_NAME { get; set; }
        public string? TYPE_GRID { get; set; }

    }
}
