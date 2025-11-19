// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class FlussoProceduraleEntity
    {
        public DateTime? DTA_ARRIVO { get; set; }
        public DateTime? DTA_REGISTRO { get; set; }
        public long SYSTEM_ID { get; set; }
        public long ID_MESSAGGIO { get; set; }
        public long ID_PROFILE { get; set; }
        public long? NUMERO_REGISTRO { get; set; }
        public string? ID_PROCESSO { get; set; }
        public string NOME_REGISTRO { get; set; }

    }
}
