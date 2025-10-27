// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class DesktopAppEntity
    {
        public string NOME { get; set; }
        public string? VERSIONE { get; set; }
        public string? PATH { get; set; }
        public string? DESCRIZIONE { get; set; }
    }
}
