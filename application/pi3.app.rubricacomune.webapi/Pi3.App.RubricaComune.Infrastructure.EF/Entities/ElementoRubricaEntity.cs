// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.RubricaComune.Infrastructure.EF.Entities
{
    public class ElementoRubricaEntity
    {
        public long ID { get; set; }
        public long IDUTENTECREATORE { get; set; }
        public string DESCRIZIONE { get; set; } = null!;
        public string CODICE { get; set; } = null!;

        public DateTime? DATACREAZIONE { get; set; }
        public DateTime? DATAULTIMAMODIFICA { get; set; }
        public long? IDAMMINISTRAZIONE { get; set; }
        public string? CHA_PUBBLICATO { get; set; }
        public string? PROVINCIA { get; set; }
        public string? TELEFONO { get; set; }
        public string? FAX { get; set; }
        public string? INDIRIZZO { get; set; }
        public string? AOO { get; set; }
        public string? CAP { get; set; }
        public string? CITTA { get; set; }
        public string? NAZIONE { get; set; }
        public string? VAR_COD_PI { get; set; }
        public string? VAR_COD_FISC { get; set; }
        public string? AMMINISTRAZIONE { get; set; }
        public string? URL { get; set; }
        public string? TIPOCORRISPONDENTE { get; set; }        
    }
}
