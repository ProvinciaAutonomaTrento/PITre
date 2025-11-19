// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.NotaRFAggregate.Events
{
    public class NotaRFCreatedEvent : ElementCreatedEvent
    {
        public NotaRFCreatedEvent() : base()
        {
        }

        public string IdRF { get; init; }

        public string? CodiceRF { get; init; }

        public TextValue? DescrizioneRF { get; init; }
    }
}
