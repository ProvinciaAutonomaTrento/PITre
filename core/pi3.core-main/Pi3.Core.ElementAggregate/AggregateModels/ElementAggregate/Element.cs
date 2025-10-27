// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.SeedWork;
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.Entities;
using Pi3.Core.AggregateModels.ElementAggregate.Exceptions;

namespace Pi3.Core.AggregateModels.ElementAggregate
{
    public abstract class Element : AggregateRoot<string>
    {
        #region Public Members
         

        public string IdTenant { get; protected set; }

        public string TypeName { get; protected set; }

        public DateTime CreationDate { get; protected set; }

        public TextValue Name { get; protected set; }

        public TextValue? Description { get; protected set; }

        public virtual void AssignId(string id)
        {
            if (!string.IsNullOrWhiteSpace(Id))
                throw new NotSupportedException();

            ApplyChange(new ElementIdAssignedEvent() { Id = id });
        }
        
        public virtual void ChangeName(TextValue newName)
        {
            ApplyChange(new ElementNameChangedEvent() { Id = this.Id, NewName = newName });
        }

        public virtual void ChangeDescription(TextValue newDescription)
        {
            ApplyChange(new ElementDescriptionChangedEvent() { Id = this.Id, NewDescription = newDescription});
        }

        public virtual void AddProfile(string id, TextValue name, IReadOnlyDictionary<string, string>? metadata = null)
        {
            ApplyChange(new ElementProfileAddedEvent() { Id = this.Id, IdProfile = id, Name = name,  Metadata = metadata });
        }

        public virtual void AddProfileField(string id, string idField, TextValue fieldName, string fieldType, IElementFieldValue fieldValue, IReadOnlyDictionary<string, string>? fieldMetadata = null)
        {
            ApplyChange(new ElementProfileFieldAddedEvent()
            {
                Id = this.Id,
                IdProfile = id,
                IdField = idField,
                FieldName = fieldName,
                FieldType = fieldType,                
                FieldValue = fieldValue,
                FieldMetadata = fieldMetadata
            });
        }

        public virtual void ChangeProfileFieldValue(string id, string idField, IElementFieldValue fieldValue)
        {
            ApplyChange(new ElementProfileFieldValueChangedEvent() { Id = this.Id, IdProfile = id, IdField = idField, FieldValue = fieldValue });
        }

        public virtual void RemoveProfileField(string id, string idField)
        {
            ApplyChange(new ElementProfileFieldRemovedEvent() { Id = this.Id, IdProfile = id, IdField = idField });
        }

        public IReadOnlyList<ElementProfile> Profiles
        {
            get
            {
                return this._profiles.AsReadOnly();
            }
        }

        public virtual void RemoveProfile(string id)
        {
            ApplyChange(new ElementProfileRemovedEvent() { Id = this.Id, IdProfile = id });
        }

        public virtual void AddKeyword(TextValue keyword)
        {
            ApplyChange(new ElementKeywordAddedEvent() { Id = this.Id, Keyword = keyword });
        }

        public virtual void RemoveKeyword(TextValue keyword)
        {
            ApplyChange(new ElementKeywordRemovedEvent() { Id = this.Id, Keyword = keyword });
        }

        public IReadOnlyList<TextValue> Keywords
        {
            get
            {
                return this._keywords.AsReadOnly();
            }
        }

        #endregion

        #region Private Members

        protected List<ElementProfile> _profiles;
        protected List<TextValue> _keywords;
        
        protected Element() : base()
        {
            //this._committedChanges = new List<dynamic>();

            this.TypeName = this.GetType().Name;
        }

        protected Element(string id, string idTenant, string typeName, DateTime creationDate, TextValue name, TextValue? description = null)
            : this()
        {
            this.ApplyChange(new ElementCreatedEvent()
            {
                Id = id,
                IdTenant = idTenant,
                TypeName = typeName,
                CreationDate = creationDate,
                Name = name,
                Description = description
            });
        }

        protected Element(string idTenant, string typeName, DateTime creationDate, TextValue name, TextValue? description = null)
            : this(null, idTenant, typeName, creationDate, name, description)
        {
        }

        protected virtual void Handle(ElementCreatedEvent @event)
        {
            this.Id = @event.Id;
            this.IdTenant = @event.IdTenant;
            this.TypeName = @event.TypeName ?? this.GetType().Name;
            this.CreationDate = @event.CreationDate;
            this.Name = @event.Name;
            this.Description = @event.Description;
            this._profiles = new List<ElementProfile>();
            this._keywords = new List<TextValue>();
        }

        protected virtual void Handle(ElementIdAssignedEvent @event)
        {
            this.Id = @event.Id;
        }

        protected virtual void Handle(ElementNameChangedEvent @event)
        {
            this.Name = @event.NewName;
        }

        protected virtual void Handle(ElementDescriptionChangedEvent @event)
        {
            this.Description = @event.NewDescription;
        }

        protected virtual void Handle(ElementProfileAddedEvent @event)
        {
            if (_profiles.Count(c => c.Id == @event.IdProfile) == 0)
                _profiles.Add(new ElementProfile(@event.IdProfile, @event.Name, @event.Metadata));
        }

        protected void Handle(ElementProfileFieldAddedEvent @event)
        {
            var profile = this.FindProfile(@event.IdProfile);

            profile.AddField(@event.IdField, @event.FieldName, @event.FieldType, @event.FieldValue, @event.FieldMetadata);
        }

        protected void Handle(ElementProfileFieldRemovedEvent @event)
        {
            var profile = this.FindProfile(@event.Id);

            profile.RemoveField(@event.IdField);
        }

        protected virtual void Handle(ElementProfileFieldValueChangedEvent @event)
        {
            var profile = this.FindProfile(@event.IdProfile);

            profile.ChangeFieldValue(@event.IdField, @event.FieldValue);
        }

        protected virtual void Handle(ElementProfileRemovedEvent @event)
        {
            var profile = this.FindProfile(@event.IdProfile);

            this._profiles.Remove(profile);
        }

        protected virtual void Handle(ElementKeywordAddedEvent @event)
        {
            this._keywords.Add(@event.Keyword);
        }

        protected virtual void Handle(ElementKeywordRemovedEvent @event)
        {
            this._keywords.Remove(@event.Keyword);
        }

        protected ElementProfile FindProfile(string id)
        {
            var profile = this._profiles.FirstOrDefault(f => f.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));
            
            if (profile == null)
                throw new ProfileFieldNotFoundPi3Exception(id);

            return profile;
        }

        protected void AssertProfileExists(string id)
        {
            var field = this._profiles.FirstOrDefault(p => p.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));

            if (field != null)
                throw new ProfileFieldNotFoundPi3Exception(id);
        }

        #endregion
    }
}
