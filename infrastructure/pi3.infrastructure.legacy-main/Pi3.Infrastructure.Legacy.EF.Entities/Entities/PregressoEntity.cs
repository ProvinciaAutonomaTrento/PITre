// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class PregressoEntity
    {
        public DateTime? DATA_ESECUZIONE { get; set; }
        public DateTime? DATA_FINE { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? ID_AMM { get; set; }
        public long? ID_UTENTE_CREATORE { get; set; }
        public long? ID_RUOLO_CREATORE { get; set; }
        public long? NUM_DOC { get; set; }
        public String? DESCRIZIONE { get; set; }
    }
}
