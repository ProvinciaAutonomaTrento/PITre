// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.CorrispondenteAggregate;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.PersonaCorrispondenteAggregate
{
    public class PersonaCorrispondenteCreatedEvent : ElementCreatedEvent
    {
        public PersonaCorrispondenteCreatedEvent() : base()
        {
        }
        public string? IdRegistro { get; init; }

        public string Nome { get; init; }

        public string Cognome { get; init; }
    }

    public class NomeChangedEvent : Event
    {
        public NomeChangedEvent()
        { }

        public string Id { get; init; }

        public string NewNome { get; init; }
    }

    public class CognomeChangedEvent : Event
    {
        public CognomeChangedEvent()
        { }

        public string Id { get; init; }

        public string NewCognome { get; init; }
    }

    public class DataNascitaSettedEvent : Event
    {
        public DataNascitaSettedEvent()
        { }

        public string Id { get; init; }

        public DateTime? NewDataNascita { get; init; }
    }

    public class LuogoNascitaSettedEvent : Event
    {
        public LuogoNascitaSettedEvent()
        { }

        public string Id { get; init; }

        public string? NewLuogoNascita { get; init; }
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

    public class NoteSettedEvent : Event
    {
        public NoteSettedEvent()
        { }

        public string Id { get; init; }

        public string? NewNote { get; init; }
    }

    public class TitoloSettedEvent : Event
    {
        public TitoloSettedEvent()
        { }

        public string Id { get; init; }

        public string? Titolo { get; init; }
    }

}
