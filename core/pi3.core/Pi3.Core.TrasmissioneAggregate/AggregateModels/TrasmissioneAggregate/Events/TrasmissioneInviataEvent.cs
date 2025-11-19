// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate.Events
{
    public class TrasmissioneInviataEvent : Event
    {
        public TrasmissioneInviataEvent()
        { }

        public DateTime DataInvio { get; init; }

        public InviaBehavior? InviaBehavior { get; init; }
    }

}
