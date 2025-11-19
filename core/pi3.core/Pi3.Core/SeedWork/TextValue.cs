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
    public class TextValue : ValueObject
    {
        public TextValue() : base()
        { }
        
        public TextValue(string value) : this(value, null)
        {
        }

        public TextValue(string value, params MultiLanguageTextValue[] multiLanguageTextValues)
        {
            this.Value = value;
            this.MultilanguageTextValues = multiLanguageTextValues?.ToList().AsReadOnly();
        }

        public TextValue(params MultiLanguageTextValue[] multiLanguageTextValues)
        {
            this.MultilanguageTextValues = multiLanguageTextValues?.ToList().AsReadOnly();
        }

        public string Value { get; init; }

        public IReadOnlyList<MultiLanguageTextValue>? MultilanguageTextValues { get; init; } = null;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Value;
            yield return this.MultilanguageTextValues;
        }

        public override string ToString()
        {
            if (this.MultilanguageTextValues != null)
            {
                if (!string.IsNullOrWhiteSpace(this.Value))
                    return $"{this.Value}, {string.Join(", ", this.MultilanguageTextValues.Select(v => v.Value))}";
                else
                    return string.Join(", ", this.MultilanguageTextValues.Select(v => v.Value));
            }
            else
                return this.Value ?? String.Empty;
        }
    }
}
