// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class SwitchServiceEntity
    {
        public string SERVICE_NAME { get; set; }

        public string CHA_USE_DATA_PORTAL_API { get; set; }

        public string? CATEGORY { get; set; }

        public string? RESPONSE_AS_RAW { get; set; }
    }
}
