// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ContentElementAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.ContentElementAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ContentElementAggregate.Events
{
    public class RelatedContentElementAddedEvent : Event
    {
        public RelatedContentElementAddedEvent()
        { }

        public string IdRelatedElement { get; init; }

        public bool AsParent { get; init; }
    }

    public class RelatedContentElementRemovedEvent : Event
    {
        public RelatedContentElementRemovedEvent()
        { }

        public string IdRelatedElement { get; init; }
    }

    public class ContentElementClassificationAddedEvent : Event
    {
        public ContentElementClassificationAddedEvent()
        { }

        public string IdClassification { get; init; }

        public TextValue? Name { get; init; } = null;

        public string? Code { get; init; } = null;
    }

    public class ContentElementClassificationRemovedEvent : Event
    {
        public ContentElementClassificationRemovedEvent()
        { }

        public string IdClassification { get; init; }
    }

    public class ContentElementReservedEvent : Event
    {
        public ContentElementReservedEvent()
        { }

        public DateTime? ReservedDate { get; init; } 

        public string? ReserveIdUser { get; init; } 
    }

    public class ContentElementUnreservedEvent : Event
    {
        public ContentElementUnreservedEvent()
        { }
    }

    public class PublicPermissionSettedEvent : Event
    {
        public PublicPermissionSettedEvent()
        { }

        public ContentElementRightTypesEnum RightType { get; init; }

        public bool ApplyToChilds { get; init; }
    }

    public class PublicPermissionRemovedEvent : Event
    {
        public PublicPermissionRemovedEvent()
        { }
    }

    public class OwnerPermissionSettedEvent : Event
    {
        public OwnerPermissionSettedEvent()
        { }

        public string IdMember { get; init; }

        public string MemberName { get; init; }

        public ContentElementMemberTypesEnum MemberType { get; init; }

        public ContentElementRightTypesEnum RightType { get; init; }

        public bool ApplyToChilds { get; init; }
    }

    public class OwnerPermissionRemovedEvent : Event
    {
        public OwnerPermissionRemovedEvent()
        { }

        public string IdMember { get; init; }

        public string MemberName { get; init; }

        public ContentElementMemberTypesEnum MemberType { get; init; }
    }

    public class MemberPermissionSettedEvent : Event
    {
        public MemberPermissionSettedEvent()
        { }

        public string IdMember { get; init; }

        public string MemberName { get; init; }

        public ContentElementMemberTypesEnum MemberType { get; init; }

        public ContentElementRightTypesEnum RightType { get; init; }

        public bool ApplyToChilds { get; init; }
    }

    public class MemberPermissionRemovedEvent : Event
    {
        public MemberPermissionRemovedEvent()
        { }

        public string IdMember { get; init; }

        public string MemberName { get; init; }

        public ContentElementMemberTypesEnum MemberType { get; init; }
    }
}
