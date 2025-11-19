// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class MailElaborataEntity
    {
        public DateTime? DTA_ELAB { get; set; }
        public long? ID_PROFILE { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? ID_REGISTRO { get; set; }
        public long? ID_PEOPLE { get; set; }
        public string? CHA_RAGIONE_ELAB { get; set; }
        public string? VAR_NOTE { get; set; }
        public string? VAR_MESSAGE { get; set; }
        public string? VAR_EMAIL { get; set; }
    }
}
