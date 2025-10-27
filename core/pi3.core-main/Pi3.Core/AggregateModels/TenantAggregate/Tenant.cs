// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.UserAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TenantAggregate
{
    public class Tenant : AggregateRoot<string>
    {
        #region Public Members

        public Tenant(string id, string name, string description)
        {
            this.ApplyChange(new TenantCreatedEvent()
            {
                Id = id,
                Name = name,
                Description = description
            });
        }

        public Tenant(string name, string description)
            : this(null, name, description)
        {
        }

        public Tenant(string name)
            : this(null, name, null)
        {
        }

        public string Name { get; protected set; }

        public string Description { get; protected set; }

        public void AssignId(string id)
        {
            this.ApplyChange(new IdTenantAssignedEvent()
            {
                Id = id
            });
        }

        #endregion

        #region Private Members

        protected void Handle(TenantCreatedEvent @event)
        {
            this.Id = @event.Id;
            this.Name = @event.Name;
            this.Description = @event.Description;
        }

        protected void Handle(IdTenantAssignedEvent @event)
        {
            this.Id = @event.Id;
        }

        #endregion
    }
}
