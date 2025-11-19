// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.Events
{
    public class DestinatarioAddedEvent : Event
    {
        public DestinatarioAddedEvent() { }

        public string Id { get; init; }

        public string IdDestinatario { get; init; }

        public TipiDestinatariEnum TipoDestinatario { get; init; }

        public TextValue? Descrizione { get; init; }

        public bool? Esterno { get; init; }
    }
}
