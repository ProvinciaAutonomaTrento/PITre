// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class SchemaProcessoFirmaEntity
    {
        public string? TICK { get; set; }
        public string? CHA_MODELLO { get; set; }
        public DateTime? DTA_CREAZIONE { get; set; }
        public long ID_PROCESSO { get; set; }
        public long? RUOLO_AUTORE { get; set; }
        public long? UTENTE_AUTORE { get; set; }
        public long? ELIMINATO { get; set; }
        public long? ID_AMM { get; set; }
        public long? ID_STATO_INTERRUZIONE { get; set; }
        public string NOME { get; set; }
    }
}
