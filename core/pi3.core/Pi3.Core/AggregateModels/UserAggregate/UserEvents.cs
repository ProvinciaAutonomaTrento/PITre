// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.UserAggregate
{
    public class UserEmailChangedEvent : Event
    {
        public string Id { get; init; }

        public string NewEmail { get; init; }
    }

    public class DefaultRoleSettedEvent : Event
    {
        public string Id { get; init; }

        public string IdRole { get; init; }
    }

    public class IdUserAssignedEvent : Event
    {
        public string Id { get; init; }
    }

    public class RoleAddedEvent : Event
    {
        public string Id { get; init; }
        public string IdRole { get; init; }
        public string? RoleTitle { get; init; }

        public bool? SetAsDefault { get; init; }
    }

    public class UserCreatedEvent : Event
    {
        public string IdTenant { get; set; }
        public string Id { get; init; }
        public string Name { get; init; }
        public string FullName { get; init; }
        public DateTime CreationDate { get; init; }
    }

    public class UserProfileSettedEvent : Event
    {
        public string Id { get; init; }

        public UserProfileTypesEnum UserProfile { get; init; }
    }

    public class RoleRemovedEvent : Event
    {
        public string Id { get; init; }
        public string IdRole { get; init; }
    }
}
