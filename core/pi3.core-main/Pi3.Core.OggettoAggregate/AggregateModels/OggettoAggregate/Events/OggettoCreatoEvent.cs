// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.OggettoAggregate.Events
{
    public class OggettoCreatoEvent : ElementCreatedEvent
    {
        public OggettoCreatoEvent()
        { }

        public string IdRegistro { get; init; }

        public string? CodiceRegistro { get; init; }

        public string? DescrizioneRegistro { get; init; }
    }
}
