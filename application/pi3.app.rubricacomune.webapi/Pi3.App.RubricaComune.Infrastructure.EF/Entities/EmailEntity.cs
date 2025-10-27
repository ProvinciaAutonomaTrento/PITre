// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.RubricaComune.Infrastructure.EF.Entities
{
    public class EmailEntity
    {
        public long IDELEMENTORUBRICA { get; set; }
        public long? PREFERITA { get; set; }
        public string EMAIL { get; set; } = null!;
        public string? NOTE { get; set; }
    }
}