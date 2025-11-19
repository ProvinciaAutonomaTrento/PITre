// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class DocsPaEntity
    {
        public long SYSTEM_ID { get; set; }
        public DateTime? DTA_INSTALL { get; set; }
        public DateTime? DTA_UPDATE { get; set; }
        public string? ID_VERSIONS_I { get; set; }
        public string? ID_VERSIONS_U { get; set; }
        public string? VAR_MESSAGGIO_LOGIN { get; set; }
    }
}
