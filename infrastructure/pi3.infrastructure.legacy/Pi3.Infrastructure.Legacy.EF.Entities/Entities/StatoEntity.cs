// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class StatoEntity
    {
        public long SYSTEM_ID { get; set; }
        public long? ID_DIAGRAMMA { get; set; }
        public string? VAR_DESCRIZIONE { get; set; }
        public long? STATO_INIZIALE { get; set; }
        public long? STATO_FINALE { get; set; }
        public long? CONV_PDF { get; set; }
        public string? STATO_CONSOLIDAMENTO { get; set; }
        public long? NON_RICERCABILE { get; set; }
        public string? CHA_STATO_SISTEMA { get; set; }
        public string? DISABILITA_SALVA_DOCUMENTO { get; set; }
        public long? ID_PROCESSO_FIRMA { get; set; }
        public string? CHA_PUBB_SELECT_FILES { get; set; }
    }
}
