// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class FirmatarioEntity
    {
        public long SYSTEM_ID { get; set; }
        public string? VAR_COGNOME { get; set; }
        public string? VAR_NOME { get; set; }
        public string? VAR_COD_FISCALE { get; set; }
        public string? VAR_TITOLARE { get; set; }
        public string? VAR_RUOLO { get; set; }
        public string? VAR_SERIE { get; set; }
        public string? VAR_EMITTENTE_C { get; set; }
        public string? VAR_EMITTENTE_O { get; set; }
        public string? VAR_EMITTENTE_OU { get; set; }
        public string? VAR_EMITTENTE_CN { get; set; }

    }
}
