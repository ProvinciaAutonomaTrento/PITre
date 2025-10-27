// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.MarcaTemporale
{
    public class MarcaTsrResponse : ValueObject
    {
        public DateTime DataOraMarca { get; init; }

        public string SerialNumberMarca { get; init; } = null!;

        public string TSAName { get; init; } = null!;

        public byte[] Marca { get; init; } = null!;
    }
}
