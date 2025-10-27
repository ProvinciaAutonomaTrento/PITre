// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class AnagraficaEventiEntity
    {
        public long ID_EVENTO { get; set; }

        public string VAR_COD_AZIONE { get; set; } = null!;

        public string? DESCRIZIONE { get; set; }

        public string? CHA_TIPO_EVENTO { get; set; }

        public string GRUPPO { get; set; } = null!;

        public string? CHA_AUTOMATICO { get; set; }

        public string? CHA_IGNORA_ORDINE { get; set; }
    }
}
