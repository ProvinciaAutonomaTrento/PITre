// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.MarcaTemporale
{
    public class MarcaTsdResponse : ValueObject
    {
        public DateTime DataOraMarca { get; init; }

        public string SerialNumberMarca { get; init; } = null!;

        public string TSAName { get; init; } = null!;

        public FileMarcatura FileMarcato { get; init; } = null!;
    }
}
