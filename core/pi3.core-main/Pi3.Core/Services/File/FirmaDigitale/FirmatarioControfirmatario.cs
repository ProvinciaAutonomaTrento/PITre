// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.FirmaDigitale
{
    public partial class FirmatarioControfirmatario
    {
        public DatiFirma DatiFirma { get; init; } = null!;

        public Marca Marca { get; init; } = null!;
    }
}
