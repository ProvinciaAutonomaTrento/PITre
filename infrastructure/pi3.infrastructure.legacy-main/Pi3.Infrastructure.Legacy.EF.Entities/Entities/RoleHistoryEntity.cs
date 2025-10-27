// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class RoleHistoryEntity
    {
        public long SYSTEM_ID { get; set; }
        public long ORIGINAL_CORR_ID { get; set; }
        public string ACTION { get; set; }
        public string ROLE_DESCRIPTION { get; set; }
        public long ROLE_TYPE_ID { get; set; }
        public DateTime ACTION_DATE { get; set; }
        public long UO_ID { get; set; }
        public string? UO_DESCRIPTION_ { get; set; }
        public string? ROLE_TYPE_DESCRIPTION_ { get; set; }
        public long ROLE_ID { get; set; }
    }
}
