// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class PrefMobileEntity
    {
        public long SYSTEM_ID { get; set; }
        public long ID_PEOPLE_OWNER { get; set; }
        public long ID_CORR { get; set; }
        public long? ID_INTERNAL { get; set; }
        public string? DESC_CORR { get; set; }
        public string? CHA_TIPO_URP { get; set; }
        public string? CHA_TIPO_PREF { get; set; }
    }
}
