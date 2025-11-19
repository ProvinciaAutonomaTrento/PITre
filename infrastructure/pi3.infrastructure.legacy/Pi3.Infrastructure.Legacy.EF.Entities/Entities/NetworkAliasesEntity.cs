// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class NetworkAliasesEntity
    {
        public long? SYSTEM_ID { get; set; }
        public string? NETWORK_ID { get; set; }
        public long? NETWORK_TYPE { get; set; }
        public long? PERSONORGROUP { get; set; }
        public long? PARENT_ORG { get; set; }
    }
}
