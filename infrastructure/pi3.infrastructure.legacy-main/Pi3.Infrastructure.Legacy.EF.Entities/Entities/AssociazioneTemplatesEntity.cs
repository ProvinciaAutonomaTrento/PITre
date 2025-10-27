// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class AssociazioneTemplatesEntity
    {
        public long SYSTEM_ID { get; set; }

        public long? ID_OGGETTO { get; set; }

        public long? ID_TEMPLATE { get; set; }

        public string? DOC_NUMBER { get; set; }
        public long? ID_DOCNUMBER { get; set; }

        public string? VALORE_OGGETTO_DB { get; set; }

        public int? ANNO { get; set; }

        public long? ID_AOO_RF { get; set; }

        public string? CODICE_DB { get; set; }

        public long? MANUAL_INSERT { get; set; }

        public long? VALORE_SC { get; set; }

        public DateTime? DTA_INS { get; set; }

        public DateTime? DTA_ANNULLAMENTO { get; set; }

        public string? ANNO_ACC { get; set; }
        
        public string? VAR_SEGNATURA { get; set; }
    }
}
