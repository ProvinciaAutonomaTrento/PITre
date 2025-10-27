// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class LdapSyncHistoryEntity
    {
        public DateTime SYNC_DATE { get; set; }
        public long SYSTEM_ID { get; set; }
        public long ID_AMM { get; set; }
        public long ITEMS_SYNCRONIZED { get; set; }
        public string? ERROR_DETAILS { get; set; }
        public string USER_ID { get; set; }

    }
}
