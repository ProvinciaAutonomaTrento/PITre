// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class AnagraficaLogEntity
    {
        public long SYSTEM_ID { get; set; }

        public string? VAR_CODICE { get; set; }

        public string? VAR_DESCRIZIONE { get; set; }

        public string? VAR_OGGETTO { get; set; }

        public string? VAR_METODO { get; set; }

        public string? MULTIPLICITY { get; set; }

        public int? ID_AMM { get; set; }

        public string? NOTIFICATION { get; set; }

        public string? CONFIGURABLE { get; set; }

        public string? NOTIFICATION_RECIPIENTS { get; set; }

        public string? COLOR { get; set; }

        public string? FOLLOW_CONFIG { get; set; }

        public string? FOLLOW { get; set; }
    }
}
