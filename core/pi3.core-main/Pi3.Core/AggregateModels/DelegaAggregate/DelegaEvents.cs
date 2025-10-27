// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DelegaAggregate
{
    public class UtenteDeleganteAssignedEvent : Event
    {
        public UtenteDeleganteAssignedEvent() : base()
        {
        }
        public string Id { get; init; }

        public string IdUtente { get; init; }

        public string? UserId { get; init; }

        public string? Cognome { get; init; }

        public string? Nome { get; init; }

    }

    public class UtenteDelegatoAssignedEvent : Event
    {
        public UtenteDelegatoAssignedEvent() : base()
        {
        }
        public string Id { get; init; }

        public string IdUtente { get; init; }

        public string? UserId { get; init; }

        public string? Cognome { get; init; }

        public string? Nome { get; init; }

    }

    public class GruppoDeleganteAssignedEvent : Event
    {
        public GruppoDeleganteAssignedEvent() : base()
        {
        }
        public string Id { get; init; }

        public string IdGruppo { get; init; }

        public string? Codice { get; init; }

        public string? Descrizione { get; init; }
    }

    public class GruppoDelegatoAssignedEvent : Event
    {
        public GruppoDelegatoAssignedEvent() : base()
        {
        }
        public string Id { get; init; }

        public string IdGruppo { get; init; }

        public string? Codice { get; init; }

        public string? Descrizione { get; init; }
    }

    public class RevocataEvent : Event
    {
        public RevocataEvent() : base()
        {
        }
        public string Id { get; init; }
    }

    public class EsercitataEvent : Event
    {
        public EsercitataEvent() : base()
        {
        }
        public string Id { get; init; }

        public bool InEsercizio { get; init; }
    }

    public class DismessaEvent : Event
    {
        public DismessaEvent() : base()
        {
        }
        public string Id { get; init; }

        public bool InEsercizio { get; init; }
    }

    public class DataDecorrenzaChangeEvent : Event
    {
        public DataDecorrenzaChangeEvent() : base()
        {
        }
        public string Id { get; init; }

        public DateTime NewData { get; init; }
    }

    public class DataScadenzaChangeEvent : Event
    {
        public DataScadenzaChangeEvent() : base()
        {
        }
        public string Id { get; init; }

        public DateTime? NewData { get; init; }
    }
}
