// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects
{
    public class Visto : ValueObject
    {
        public Visto()
        { }

        public DateTime? Data { get; init; } = null;

        public string? IdDelegato { get; init; } = null;

        public string? UserIdDelegato { get; init; } = null;

        public string? CognomeDelegato { get; init; } = null;

        public string? NomeDelegato { get; init; } = null;
    }
}
