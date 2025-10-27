// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class EventDocumentEntity
    {
        public string? CHA_ERRORE { get; set; }
        public DateTime DTA_AZIONE { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? ID_PEOPLE_OPERATORE { get; set; }
        public long? ID_GRUPPO_OPERATORE { get; set; }
        public long? ID_AMMINISTRAZIONE { get; set; }
        public long? ID_OGGETTO { get; set; }
        public long? ID_TRASM { get; set; }
        public long? ID_PEOPLE_DELEGANTE { get; set; }
        public string? VAR_DESC_OGGETTO { get; set; }
        public string? VAR_DESC_AZIONE { get; set; }
        public string? VAR_OGGETTO { get; set; }
        public string? VAR_COD_AZIONE { get; set; }
    }
}
