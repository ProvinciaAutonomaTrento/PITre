// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class NotaEntity
    {
        public long SYSTEM_ID { get; set; }

        public string? TESTO { get; set; }

        public DateTime DATACREAZIONE { get; set; }

        public long IDUTENTECREATORE { get; set; }

        public long IDRUOLOCREATORE { get; set; }

        public string TIPOVISIBILITA { get; set; } = null!;

        public string TIPOOGGETTOASSOCIATO { get; set; } = null!;

        public long IDOGGETTOASSOCIATO { get; set; }

        public long? IDPEOPLEDELEGATO { get; set; }

        public long? IDRFASSOCIATO { get; set; }
    }
}
