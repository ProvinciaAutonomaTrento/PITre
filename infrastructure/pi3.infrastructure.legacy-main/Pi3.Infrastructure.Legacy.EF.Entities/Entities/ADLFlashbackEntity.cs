// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ADLFlashbackEntity
    {
        public long? SYSTEM_ID { get; set; }

        public long? ID_PEOPLE { get; set; }

        public long? ID_RUOLO_IN_UO { get; set; }

        public long? ID_PROFILE { get; set; }

        public long? ID_PROJECT { get; set; }

        public DateTime? DTA_INS { get; set; }

        public string? CHA_TIPO_DOC { get; set; }

        public string? CHA_TIPO_FASC { get; set; }

        public long? ID_REGISTRO { get; set; }
    }
}
