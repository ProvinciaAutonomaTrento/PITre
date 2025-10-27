// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.Events
{
    public class AutoreRuoloAssignedEvent : Event
    {
        public AutoreRuoloAssignedEvent() { }

        public string? CodiceGruppo { get; init; }

        public string Id { get; init; }

        public string IdGruppo { get; init; }

        public TextValue? Descrizione { get; init; }
    }
}
