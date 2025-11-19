// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Pi3.Core.AggregateModels.ElementAggregate.Exceptions;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.SeedWork;

namespace Pi3.Core.AggregateModels.ElementAggregate.Entities
{
    public class ElementProfile : Entity<string>
    {
        #region Public Members

        internal ElementProfile(string id, TextValue name) : this(id, name, null)
        {
        }

        internal ElementProfile(string id, TextValue name, IReadOnlyDictionary<string, string>? metadata = null)
        {
            Id = id;
            Name = name;
            _metadata = new Dictionary<string, string>(metadata ?? new Dictionary<string, string>(), StringComparer.InvariantCultureIgnoreCase);
            _fields = new List<ElementField>();
        }

        public string Id { get; protected set; }

        public TextValue Name { get; protected set; }

        public IReadOnlyDictionary<string, string> Metadata
        {
            get
            {
                return new ReadOnlyDictionary<string, string>(_metadata);
            }
        }

        public IReadOnlyList<ElementField> Fields
        {
            get
            {
                return _fields.AsReadOnly();
            }
        }

        #endregion

        #region Private Members

        protected readonly new List<ElementField> _fields = null;
        protected readonly new Dictionary<string, string> _metadata = null;

        internal void ChangeName(TextValue newName)
        {
            Name = newName;
        }

        internal void SetMetadata(string name, string value)
        {
            if (_metadata.ContainsKey(name))
                _metadata[name] = value;
            else
                _metadata.Add(name, value);
        }

        internal void RemoveMetadata(string name)
        {
            if (_metadata.ContainsKey(name))
                _metadata.Remove(name);
        }

        protected ElementField FindField(string id)
        {
            var field = _fields.FirstOrDefault(f => f.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));

            if (field == null)
                throw new ProfileFieldNotFoundPi3Exception(id);

            return field;
        }

        protected void AssertFieldExists(string id)
        {
            var field = _fields.FirstOrDefault(f => f.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));

            if (field != null)
                throw new ProfileFieldAlreadyExistsPi3Exception(id);
        }

        internal void AddField(string id, TextValue name, string type)
        {
            AddField(id, name, type, null, null);
        }

        internal void AddField(string id, TextValue name, string type, IElementFieldValue value, IReadOnlyDictionary<string, string>? metadata = null)
        {
            AssertFieldExists(id);

            var field = new ElementField(id, name, type, value, metadata);

            _fields.Add(field);
        }

        internal void RemoveField(string id)
        {
            var field = FindField(id);

            _fields.Remove(field);
        }

        internal void SetFieldMetadata(string id, string name, string value)
        {
            var field = FindField(id);

            field.SetMetadata(name, value);
        }

        internal void RemoveFieldMetadata(string id, string name)
        {
            var field = FindField(id);

            field.RemoveMetadata(name);
        }

        internal void ChangeFieldValue(string id, IElementFieldValue value)
        {
            var field = FindField(id);

            field.ChangeValue(value);
        }

        #endregion
    }
}
