// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class DiagrammiStoEntity
    {
        public long SYSTEM_ID { get; set; }
        public long? DOC_NUMBER { get; set; }
        public DateTime? DTA_DATE { get; set; }
        public long? ID_PEOPLE { get; set; }
        public long? ID_RUOLO { get; set; }
        public long? ID_PROJECT { get; set; }
        public long? ID_PEOPLE_DELEGATO { get; set; }
        public string? ID_USER { get; set; }
        public string? VAR_DESC_OLD_STATO { get; set; }
        public string? VAR_DESC_NEW_STATO { get; set; }
    }
}
