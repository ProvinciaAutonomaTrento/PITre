// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.NotaAggregate.Events
{
    public class NotaCreataEvent : ElementCreatedEvent
    {
        public NotaCreataEvent()
        {
        }

        public string? IdIdAccessoRF { get; init; }

        public AutoreNota Autore { get; init; }

        public string IdOggettoAssociato { get; init; }

        public TipiOggettoEnum TipoOggettoAssociato { get; init; }

        public TipoAccessoNotaEnum TipoAccesso { get; init; }

        public ElementCreatedEvent ElementCreated { get; init; }
    }
}
