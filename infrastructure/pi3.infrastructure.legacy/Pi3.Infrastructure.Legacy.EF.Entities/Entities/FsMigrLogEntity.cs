// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class FsMigrLogEntity
    {
        public DateTime DTA_MIGRAZIONE { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? DOCNUMBER { get; set; }
        public long? VERSIONID { get; set; }
        public string? CHA_ERRORE { get; set; }
        public string? PATH_OLD { get; set; }
        public string? PATH_NEW { get; set; }
        public string? VAR_MESSAGE { get; set; }

    }
}
