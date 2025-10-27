// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class AnagraficaFunzioniEntity
    {
        public string COD_FUNZIONE { get; set; } = null!;

        public string? VAR_DESC_FUNZIONE { get; set; }

        public string? CHA_TIPO_FUNZ { get; set; }

        public string? DISABLED { get; set; }
    }
}
