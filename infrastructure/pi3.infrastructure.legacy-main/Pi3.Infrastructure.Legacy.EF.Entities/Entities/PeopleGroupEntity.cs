// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class PeopleGroupEntity
    {
        public long? GROUPS_SYSTEM_ID { get; set; }
        public long? PEOPLE_SYSTEM_ID { get; set; }
        public string? CHA_UTENTE_RIF { get; set; }
        public string? CHA_PREFERITO { get; set; }
        public DateTime? DTA_FINE { get; set; }
    }
}
