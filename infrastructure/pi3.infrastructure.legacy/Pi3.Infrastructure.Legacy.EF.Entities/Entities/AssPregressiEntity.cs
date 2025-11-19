// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class AssPregressiEntity
    {
        public long? SYSTEM_ID { get; set; }

        public long? ID_PREGRESSO { get; set; }

        public long? ID_REGISTRO { get; set; }

        public long? ID_DOCUMENTO { get; set; }

        public long? ID_UTENTE { get; set; }

        public long? ID_RUOLO { get; set; }

        public string? TIPO_OPERAZIONE { get; set; }

        public DateTime? DATA { get; set; }

        public string? ERRORE { get; set; }

        public string? ESITO { get; set; }

        public string? ID_NUM_PROTO_EXCEL { get; set; }
    }
}
