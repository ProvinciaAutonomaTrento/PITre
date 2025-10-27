// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class AssociazioneTemplatesTmpEntity
    {
        public long? SYSTEM_ID { get; set; }

        public string? DOC_NUMBER { get; set; }

        public long? ID_OGGETTO { get; set; }

        public string? VALORE_OGGETTO_DB { get; set; }

        public string? VAR_SEGNATURA { get; set; }
    }
}
