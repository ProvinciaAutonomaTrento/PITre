// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class CollMSpedizDocumentoEntity
    {
        public long SYSTEM_ID { get; set; }

        public long IDAMM { get; set; }

        public long? ID_RUOLO { get; set; }

        public long? ID_UTENTE { get; set; }

        public long ID_DOCUMENTTYPES { get; set; }

        public long ID_PROFILE { get; set; }
    }
}
