// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class InteroperabilitySettingEntity
    {
        public long? REGISTRYID { get; set; }
        public long? ROLEID { get; set; }
        public long? USERID { get; set; }
        public long? ISENABLEDINTEROPERABILITY { get; set; }
        public string? MANAGEMENTMODE { get; set; }
        public long? KEEPPRIVATE { get; set; }
    }
}
