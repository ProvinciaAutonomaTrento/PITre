// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.CorrispondenteAggregate
{
    public class CanalePreferenzialeCorrispondente : Entity<string>
    {
        internal CanalePreferenzialeCorrispondente(string id)
        {
            this.Id = id;
        }
    }

    public class CanalePreferenzialeMailCorrispondente : CanalePreferenzialeCorrispondente
    {
        internal CanalePreferenzialeMailCorrispondente(string id) : base(id) { }
    }

    public class CanalePreferenzialeInteroperabilitaCorrispondente : CanalePreferenzialeCorrispondente
    {
        internal CanalePreferenzialeInteroperabilitaCorrispondente(string id) : base(id) { }
    }
}
