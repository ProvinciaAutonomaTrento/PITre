// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.SeedWork
{
    public class MultiLanguageTextValue : ValueObject
    {
        public MultiLanguageTextValue() : base()
        {
        }

        public MultiLanguageTextValue(string language, string value)
        {
            Language = language;
            Value = value;
        }

        public string Language { get; init; }

        public string Value { get; init; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Language;
            yield return this.Value;
        }

        public override string ToString()
        {
            return (this.Value ?? String.Empty);
        }
    }
}
