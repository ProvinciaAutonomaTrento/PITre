// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.CorrispondenteAggregate;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.UOCorrispondenteAggregate
{
    public class UOCorrispondenteCreatedEvent : ElementCreatedEvent
    {
        public UOCorrispondenteCreatedEvent() : base()
        {
        }
        public string? IdRegistro { get; init; }

    }

    public class IndirizzoChangedEvent : Event
    {
        public string Id { get; init; }

        public IndirizzoCorrispondente NewIndirizzo { get; init; }
    }

    public class CodiceFiscaleSettedEvent : Event
    {
        public CodiceFiscaleSettedEvent()
        { }

        public string Id { get; init; }

        public string? NewCodiceFiscale { get; init; }
    }

    public class PartitaIvaSettedEvent : Event
    {
        public PartitaIvaSettedEvent()
        { }

        public string Id { get; init; }

        public string? NewPartitaIva { get; init; }
    }

    public class CodiceIpaSettedEvent : Event
    {
        public CodiceIpaSettedEvent()
        { }

        public string Id { get; init; }

        public string? NewCodiceIpa { get; init; }
    }

    public class NoteSettedEvent : Event
    {
        public NoteSettedEvent()
        { }

        public string Id { get; init; }

        public string? NewNote { get; init; }
    }
}
