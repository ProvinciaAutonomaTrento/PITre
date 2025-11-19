// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.UserAggregate
{
    public class User : Element
    {
        #region Public Members

        protected User() : base()
        {
        }

        public User(string idTenant, DateTime creationDate, TextValue name, TextValue? description = null)
                : base(null, idTenant, "User", creationDate, name, description)
        {
        }

        public User(string id, string idTenant, DateTime creationDate, TextValue name, TextValue? description = null)
            : base(id, idTenant, "User", creationDate, name, description)
        {
        }

        //public override IEnumerable<dynamic> AsChanges()
        //{
        //    var changes = (List<IEvent>)base.AsChanges();

        //    if (!string.IsNullOrWhiteSpace(this.Email))
        //        changes.Add(new UserEmailChangedEvent()
        //        {
        //            Id = this.Id,
        //            NewEmail = this.Email
        //        });

        //    changes.Add(new UserProfileSettedEvent()
        //    {
        //        Id = this.Id,
        //        UserProfile = this.UserProfile
        //    });

        //    return changes;
        //}

        public virtual void ChangeEmail(string newEmail)
        {
            this.ApplyChange(new UserEmailChangedEvent() { Id = this.Id, NewEmail = newEmail });
        }

        public string Email { get; protected set; }

        public UserProfileTypesEnum UserProfile { get; protected set; }

        public void SetUserProfile(UserProfileTypesEnum userProfile)
        {
            this.ApplyChange(new UserProfileSettedEvent()
            {
                Id = this.Id,
                UserProfile = userProfile
            });
        }

        public void SetAsDefaultRole(string id)
        {
            this.ApplyChange(new DefaultRoleSettedEvent()
            {
                Id = this.Id,
                IdRole = id
            });
        }

        public void AddRole(string id, string? title = null, bool? setAsDefault = false)
        {
            this.ApplyChange(new RoleAddedEvent()
            {
                Id = this.Id,
                IdRole = id,
                RoleTitle = title,
                SetAsDefault = setAsDefault
            });
        }

        public void RemoveRole(string idRole)
        {
            this.ApplyChange(new RoleRemovedEvent()
            {
                Id = this.Id,
                IdRole = idRole
            });
        }

        public IReadOnlyList<Role> Roles
        {
            get
            {
                return this._roles.AsReadOnly();
            }
        }

        #endregion

        #region Private Members

        protected readonly List<Role> _roles = new List<Role>();

        protected virtual void Handle(UserEmailChangedEvent @event)
        {
            this.Email = @event.NewEmail;
        }

        protected virtual void Handle(UserProfileSettedEvent @event)
        {
            this.UserProfile = @event.UserProfile;
        }

        protected virtual void Handle(RoleAddedEvent @event)
        {
            this._roles.Add(new Role(@event.IdRole, @event.RoleTitle));
        }

        protected virtual void Handle(RoleRemovedEvent @event)
        {
            var idx = this._roles.FindIndex(r => r.Id.Equals(@event.Id, StringComparison.InvariantCultureIgnoreCase));
            
            this._roles.RemoveAt(idx);
        }

        protected virtual void Handle(DefaultRoleSettedEvent @event)
        {
            this._roles.ForEach(r => r.SetAsDefault(false));
            this._roles.First(r => r.Id.Equals(@event.IdRole)).SetAsDefault(true);
        }

        #endregion
    }
}
