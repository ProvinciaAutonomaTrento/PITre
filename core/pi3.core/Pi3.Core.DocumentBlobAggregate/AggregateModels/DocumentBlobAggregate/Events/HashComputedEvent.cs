// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.DocumentBlobAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentBlobAggregate.Events
{
    public class HashComputedEvent : Event
    {
        public HashComputedEvent()
        { }

        public byte[] Hash { get; init; }

        public HashNamesEnum HashName { get; init; }
    }
}
