// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class CanaliEntity
    {
        public long SYSTEM_ID { get; set; }

        public string? VAR_SERVER_SMTP { get; set; }

        public int? NUM_PORTA_SMTP { get; set; }

        public string? VAR_SERVER_POP { get; set; }

        public int? NUM_PORTA_POP { get; set; }

        public string? VAR_DESC_CANALE { get; set; }

        public int? ID_DOCUMENTTYPE { get; set; }
    }
}
