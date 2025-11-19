// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class TipoFunzioneEntity
    {
        public long SYSTEM_ID { get; set; }
        public string? VAR_COD_TIPO { get; set; }
        public string? VAR_DESC_TIPO_FUN { get; set; }
        public string? CHA_VIS { get; set; }
        public long? ID_AMM { get; set; }
    }
}
