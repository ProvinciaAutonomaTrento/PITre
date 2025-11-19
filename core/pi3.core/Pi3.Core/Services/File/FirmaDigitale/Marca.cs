// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.FirmaDigitale
{
    public class Marca : ValueObject
    {
        public string TSANameIssuer { get; init; } = null!;

        public string TSANameSubject { get; init; } = null!;

        public DateTime? TSdateTime { get; init; }

        public string TSimprint { get; init; } = null!;

        public string TSserialNumber { get; init; } = null!;

        public System.DateTime? DataFineValiditaCert { get; init; }

        public System.DateTime? DataInizioValiditaCert { get; init; }
    }
}
