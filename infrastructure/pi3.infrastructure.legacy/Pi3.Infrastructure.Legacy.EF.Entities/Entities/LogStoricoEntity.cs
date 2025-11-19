// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class LogStoricoEntity
    {
        public long SYSTEM_ID { get; set; }
        public long? ID_PEOPLE_OPERATORE { get; set; }
        public long? ID_GRUPPO_OPERATORE { get; set; }
        public long? ID_AMM { get; set; }
        public long? ID_OGGETTO { get; set; }
        public string? USERID_OPERATORE { get; set; }
        public string? VAR_OGGETTO { get; set; }
        public string? VAR_DESC_OGGETTO { get; set; }
        public string? VAR_COD_AZIONE { get; set; }
        public string? CHA_ESITO { get; set; }
        public string? VAR_DESC_AZIONE { get; set; }
        public string? VAR_COD_WORKING_APPLICATION { get; set; }
        public string? DESC_PRODUCER { get; set; }
        public long? ID_PEOPLE_DELEGANTE { get; set; }
        public DateTime? DTA_AZIONE { get; set; }
    }
}
