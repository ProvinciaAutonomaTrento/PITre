// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System.Collections.ObjectModel;

namespace Pi3.Core.AggregateModels.ElementAggregate.Entities
{
    public class ElementField : Entity<string>
    {
        #region Public Members

        internal ElementField(string id, TextValue name, string type) : this(id, name, type, null, null)
        {
        }

        internal ElementField(string id, TextValue name, string type, IElementFieldValue value, IReadOnlyDictionary<string, string>? metadata = null)
        {
            Id = id;
            Name = name;
            Type = type;
            Value = value;
            _metadata = new Dictionary<string, string>(metadata ?? new Dictionary<string, string>(), StringComparer.InvariantCultureIgnoreCase);
        }

        public string Id { get; protected set; }

        public TextValue Name { get; protected set; }

        internal void ChangeName(TextValue newName)
        {
            Name = newName;
        }

        public string Type { get; protected set; }

        public IReadOnlyDictionary<string, string> Metadata
        {
            get
            {
                return new ReadOnlyDictionary<string, string>(_metadata);
            }
        }

        public IElementFieldValue Value { get; protected set; }

        #endregion

        #region Private Members

        protected readonly new Dictionary<string, string> _metadata = null;

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

        internal void ChangeValue(IElementFieldValue value)
        {
            Value = value;
        }

        #endregion
    }
}
