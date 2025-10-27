// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class LogTipoAttoEntity
    {
        public long? ID_TIPO_ATTO { get; set; }
        public string? DESCRIZIONE { get; set; }
        public string? IN_ESERCIZIO { get; set; }
        public DateTime? DTA_MODIFICA { get; set; }
    }
}
