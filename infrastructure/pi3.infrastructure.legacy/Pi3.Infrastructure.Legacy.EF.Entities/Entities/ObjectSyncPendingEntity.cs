// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ObjectSyncPendingEntity
    {
        public string? TYPE { get; set; }
        public long? ID_DOC_OR_FASC { get; set; }
        public long? ID_GRUPPO_TO_SYNC { get; set; }
    }
}
