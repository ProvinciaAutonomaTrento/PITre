// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ElementAggregate.Events
{
    public class ElementCreatedEvent : Event
    {
        public ElementCreatedEvent()
        { }

        public string IdTenant { get; init; }

        public string TypeName { get; init; }

        public DateTime CreationDate { get; init; }

        public TextValue Name { get; init; }

        public TextValue? Description { get; init; }
    }

    public class ElementIdAssignedEvent : Event
    {
        public ElementIdAssignedEvent()
        { }
    }

    public class ElementNameChangedEvent : Event
    {
        public ElementNameChangedEvent()
        { }

        public TextValue NewName { get; init; }
    }

    public class ElementDescriptionChangedEvent : Event
    {
        public ElementDescriptionChangedEvent()
        { }

        public TextValue NewDescription { get; init; }
    }

    public class ElementProfileAddedEvent : Event
    {
        public ElementProfileAddedEvent()
        { }

        public string IdProfile { get; init; }

        public TextValue Name { get; init; }

        public IReadOnlyDictionary<string, string>? Metadata { get; init; } = null;
    }

    public class ElementProfileFieldAddedEvent : Event
    {
        public ElementProfileFieldAddedEvent()
        { }

        public string IdProfile { get; init; }

        public string IdField { get; init; }

        public TextValue FieldName { get; init; }

        public string FieldType { get; init; }

        public IElementFieldValue FieldValue { get; init; }

        public IReadOnlyDictionary<string, string>? FieldMetadata { get; init; } = null;
    }

    public class ElementProfileFieldRemovedEvent : Event
    {
        public ElementProfileFieldRemovedEvent()
        { }

        public string IdProfile { get; init; }

        public string IdField { get; init; }
    }

    public class ElementProfileFieldValueChangedEvent : Event
    {
        public ElementProfileFieldValueChangedEvent()
        { }

        public string IdProfile { get; init; }

        public string IdField { get; init; }

        public IElementFieldValue FieldValue { get; init; }
    }

    public class ElementProfileRemovedEvent : Event
    {
        public ElementProfileRemovedEvent()
        { }

        public string IdProfile { get; init; }
    }

    public class ElementKeywordAddedEvent : Event
    {
        public ElementKeywordAddedEvent()
        { }

        public TextValue Keyword { get; init; }
    }

    public class ElementKeywordRemovedEvent : Event
    {
        public ElementKeywordRemovedEvent()
        { }

        public TextValue Keyword { get; init; }
    }
}
