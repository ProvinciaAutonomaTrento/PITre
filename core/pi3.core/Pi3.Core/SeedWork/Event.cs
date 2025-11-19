// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pi3.Core.SeedWork
{
    public abstract class Event : ValueObject, IEvent
    {
        public Event()
        {
            IdEvent = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }
        
        public dynamic Id
        {
            get
            {
                if (this._ownerAggregate != null && this._ownerAggregate.Id != null)
                    return this._ownerAggregate.Id;
                else
                    return this._id;
            }
            init
            {
                if (this._ownerAggregate == null ||
                    (this._ownerAggregate != null && this._ownerAggregate.Id != null))
                    this._id = value;
            }
        }

        public Guid IdEvent
        {
            get;
            init;
        }

        public DateTime CreatedAt
        {
            get;
            init;
        }

        public int Version
        {
            get;
            init;
        }

        void IEvent.SetOwnerAggregate(dynamic ownerAggregate)
        {
            this._ownerAggregate = ownerAggregate;
        }

        protected dynamic _id;

        [JsonIgnore]
        protected dynamic _ownerAggregate;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Id;
            yield return this.IdEvent;
            yield return this.CreatedAt;
            yield return this.Version;
        }
    }
}
