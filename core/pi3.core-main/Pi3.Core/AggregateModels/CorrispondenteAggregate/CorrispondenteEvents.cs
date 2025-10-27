// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.CorrispondenteAggregate
{
    public class DescriptionChangedEvent : Event
    {
        public DescriptionChangedEvent()
        { }

        public string Id { get; init; }

        public TextValue NewDescription { get; init; }
    }

    public class EmailAddedEvent : Event
    {
        public EmailAddedEvent()
        { }

        public string Id { get; init; }

        public string IdEmail { get; init; }

        public string IndirizzoEmail { get; init; }

        public bool Principale { get; init; }

        public TextValue Note { get; init; }
    }

    public class EmailRemovedEvent : Event
    {
        public EmailRemovedEvent()
        { }

        public string Id { get; init; }

        public string IdEmail { get; init; }
    }

    public class EmailModifiedEvent : Event
    {
        public EmailModifiedEvent()
        { }

        public string Id { get; init; }

        public string IdEmail { get; init; }

        public string IndirizzoEmail { get; init; }

        public TextValue Note { get; init; }
    }

    public class CodiceAmministrazioneSettedEvent : Event
    {
        public CodiceAmministrazioneSettedEvent()
        { }

        public string Id { get; init; }

        public string? NewCodiceAmministrazione { get; init; }
    }

    public class CodiceAOOSettedEvent : Event
    {
        public CodiceAOOSettedEvent()
        { }

        public string Id { get; init; }

        public string? NewCodiceAOO { get; init; }
    }

    public class EmailPrincipaleSettedEvent : Event
    {
        public string Id { get; init; }

        public string IndirizzoEmail { get; init; }
    }

    public class CanalePreferenzialeSettedEvent : Event
    {
        public string Id { get; init; }

        public string IdCanalePreferenziale { get; init; }
    }

    public class CanalePreferenzialeMailSettedEvent : Event
    {
        public string Id { get; init; }

        public string IdCanalePreferenziale { get; init; }
    }

    public class CanalePreferenzialeInteroperabilitaSettedEvent : Event
    {
        public string Id { get; init; }

        public string IdCanalePreferenziale { get; init; }
    }

    public class DataFineSettedEvent : Event
    {
        public DataFineSettedEvent()
        { }

        public string Id { get; init; }

        public DateTime? NewDataFine { get; init; }
    }

    public class RubricaComuneSettedEvent : Event
    {
        public RubricaComuneSettedEvent()
        { }

        public string Id { get; init; }

        public bool? NewRubricaComune { get; init; }
    }

    public class RubricaEsternaSettedEvent : Event
    {
        public RubricaEsternaSettedEvent()
        { }

        public string Id { get; init; }

        public string? NewRubricaEsterna { get; init; }
    }

    public class CodiceSettedEvent : Event
    {
        public CodiceSettedEvent()
        { }

        public string Id { get; init; }

        public string? Codice { get; init; }
    }

    public class IdOldSettedEvent : Event
    {
        public IdOldSettedEvent()
        { }

        public string Id { get; init; }

        public string? IdOld { get; init; }
    }

    public class DescriptionOldSettedEvent : Event
    {
        public DescriptionOldSettedEvent()
        { }

        public string Id { get; init; }

        public string? DescriptionOld { get; init; }
    }

    public class InteropUrlSettedEvent : Event
    {
        public InteropUrlSettedEvent()
        { }

        public string Id { get; init; }

        public string? InteropUrl { get; init; }
    }
}
