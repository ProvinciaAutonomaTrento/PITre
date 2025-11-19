// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ExternalSystemEntity
    {
        public long SYSTEM_ID { get; set; }
        public long ID_SYSTEM_ROLE { get; set; }
        public long ID_AMM { get; set; }
        public string? VAR_TKN_TIME { get; set; }
        public string? VAR_DESC_ESTESA { get; set; }
        public string VAR_USER_ID { get; set; }
        public string? VAR_PIS_METHODS_ALLOWED { get; set; }
        public string VAR_CODE_APPLICATION { get; set; }
        public string? VAR_DESCRIZIONE { get; set; }

    }
}
