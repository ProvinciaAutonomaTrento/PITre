// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class IstanzaProcFirmaStoEntity
    {
        public string? CHA_CAMBIO_STATO_DIAG { get; set; }
        public DateTime? DTA_DATE { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? DOC_NUMBER { get; set; }
        public long? ID_ISTANZA_PROCESSO { get; set; }
        public long? ID_PEOPLE { get; set; }
        public long? ID_RUOLO { get; set; }
        public long? ID_PEOPLE_DELEGATO { get; set; }
        public string? ID_USER { get; set; }
        public string? VAR_DESC_AZIONE { get; set; }

    }
}
