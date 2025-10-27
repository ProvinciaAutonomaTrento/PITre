// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class LibroFirmaEntity
    {
        public long ID_AREA { get; set; }
        public long RUOLO_TITOLARE { get; set; }
        public long UTENTE_TITOLARE { get; set; }
        public string? PREFERENZE_CN { get; set; }

    }
}
